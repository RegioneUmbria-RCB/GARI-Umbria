Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Xml
Imports System.Text


Public Class MacrousoObject

    Dim _CodMacrouso As String
    Dim _SupMacrouso As String
    Dim _Utilizzo As List(Of UtilizzoObject)

    Public Property CodMacrouso As String
        Get
            Return _CodMacrouso
        End Get
        Set(ByVal value As String)
            _CodMacrouso = value
        End Set
    End Property

    Public Property SupMacrouso As String
        Get
            Return _SupMacrouso
        End Get
        Set(ByVal value As String)
            _SupMacrouso = value
        End Set
    End Property

    Public Property Utilizzo As List(Of UtilizzoObject)
        Get
            Return _Utilizzo
        End Get
        Set(ByVal value As List(Of UtilizzoObject))
            _Utilizzo = value
        End Set
    End Property

End Class

Public Class UtilizzoObject

    Dim _SpecieCod As String
    Dim _VarietaCod As String
    Dim _SupUtilizzo As String

    Public Property SpecieCod As String
        Get
            Return _SpecieCod
        End Get
        Set(ByVal value As String)
            _SpecieCod = value
        End Set
    End Property

    Public Property VarietaCod As String
        Get
            Return _VarietaCod
        End Get
        Set(ByVal value As String)
            _VarietaCod = value
        End Set
    End Property

    Public Property SupUtilizzo As String
        Get
            Return _SupUtilizzo
        End Get
        Set(ByVal value As String)
            _SupUtilizzo = value
        End Set
    End Property

End Class



#Region "VerificatoreCatasto"

Public Class cls_ParticellaVerifica

    '#############################################
    Dim _Piva As String
    Dim _Prov As String
    Dim _Com As String
    Dim _Sezione As String
    Dim _Foglio As String
    Dim _Numero As String
    Dim _Subalterno As String
    Dim _SupCatastale As String
    Dim _TitoloDatePossesso As String
    'Dim _DatePossesso As String
    Dim _SupCondotta As String
    Dim _Macrouso As String
    Dim _SupMacrouso As String
    Dim _Zona As String
    Dim _Utilizzo As String

    '#######################################################################
    '#######################################################################
    '######  PROPRIETA'  ###################################################
    '#######################################################################
    '#######################################################################
#Region "Proprieta"

    Public Property Piva As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Prov As String
        Get
            Return _Prov
        End Get
        Set(ByVal value As String)
            _Prov = value
        End Set
    End Property

    Public Property Com As String
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property

    Public Property Sezione As String
        Get
            Return _Sezione
        End Get
        Set(ByVal value As String)
            _Sezione = value
        End Set
    End Property

    Public Property Foglio As String
        Get
            Return _Foglio
        End Get
        Set(ByVal value As String)
            _Foglio = value
        End Set
    End Property

    Public Property Numero As String
        Get
            Return _Numero
        End Get
        Set(ByVal value As String)
            _Numero = value
        End Set
    End Property

    Public Property Subalterno As String
        Get
            Return _Subalterno
        End Get
        Set(ByVal value As String)
            _Subalterno = value
        End Set
    End Property

    Public Property SupCatastale As String
        Get
            Return _SupCatastale
        End Get
        Set(ByVal value As String)
            _SupCatastale = value
        End Set
    End Property

    Public Property TitoloDatePossesso As String
        Get
            Return _TitoloDatePossesso
        End Get
        Set(ByVal value As String)
            _TitoloDatePossesso = value
        End Set
    End Property

    'Public Property DatePossesso As String
    '    Get
    '        Return _DatePossesso
    '    End Get
    '    Set(ByVal value As String)
    '        _DatePossesso = value
    '    End Set
    'End Property

    'Public Property TitoloPossesso As String
    '    Get
    '        Return _TitoloPossesso
    '    End Get
    '    Set(ByVal value As String)
    '        _TitoloPossesso = value
    '    End Set
    'End Property

    Public Property SupCondotta As String
        Get
            Return _SupCondotta
        End Get
        Set(ByVal value As String)
            _SupCondotta = value
        End Set
    End Property

    Public Property Macrouso As String
        Get
            Return _Macrouso
        End Get
        Set(ByVal value As String)
            _Macrouso = value
        End Set
    End Property

    Public Property SupMacrouso As String
        Get
            Return _SupMacrouso
        End Get
        Set(ByVal value As String)
            _SupMacrouso = value
        End Set
    End Property

    Public Property Zona As String
        Get
            Return _Zona
        End Get
        Set(ByVal value As String)
            _Zona = value
        End Set
    End Property

    Public Property Utilizzo As String
        Get
            Return _Utilizzo
        End Get
        Set(ByVal value As String)
            _Utilizzo = value
        End Set
    End Property

#End Region

End Class


Public Class cls_VerificatoreCatasto

    '#######################################################################
    '#######################################################################
    '######  FUNZIONI  ###################################################
    '#######################################################################
    '#######################################################################
#Region "Funzioni"

    '#############################################
    Public Function Genera_DT_Particelle() As DataTable

        Dim DT As New DataTable

        DT.Columns.Add(New DataColumn("Flag_Presente", GetType(Boolean)))
        DT.Columns.Add(New DataColumn("Flag_Modifica", GetType(Boolean)))
        DT.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("sigla_prov", GetType(String)))
        DT.Columns.Add(New DataColumn("com_des", GetType(String)))
        DT.Columns.Add(New DataColumn("PROV", GetType(String)))
        DT.Columns.Add(New DataColumn("COM", GetType(String)))
        DT.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DT.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        DT.Columns.Add(New DataColumn("supcatastale", GetType(String)))
        DT.Columns.Add(New DataColumn("supcatastale_new", GetType(String)))
        DT.Columns.Add(New DataColumn("possesso", GetType(String)))
        DT.Columns.Add(New DataColumn("possesso_new", GetType(String)))
        DT.Columns.Add(New DataColumn("supcondotta", GetType(String)))
        DT.Columns.Add(New DataColumn("supcondotta_new", GetType(String)))
        DT.Columns.Add(New DataColumn("macrouso", GetType(String)))
        DT.Columns.Add(New DataColumn("macrouso_new", GetType(String)))
        DT.Columns.Add(New DataColumn("supmacrouso", GetType(String)))
        DT.Columns.Add(New DataColumn("supmacrouso_new", GetType(String)))
        DT.Columns.Add(New DataColumn("zona", GetType(String)))
        DT.Columns.Add(New DataColumn("zona_new", GetType(String)))
        DT.Columns.Add(New DataColumn("utilizzi", GetType(String)))
        DT.Columns.Add(New DataColumn("utilizzi_new", GetType(String)))

        Return DT

    End Function

    '#############################################
    Public Sub Inserisci_DT_Particelle(ByRef DT As DataTable, _
                                           ByVal Flag_Presente As String, _
                                           ByVal Flag_Modifica As String, _
                                           ByVal Sa_Nome As String, _
                                           ByVal Provincia As String, _
                                           ByVal Comune As String, _
                                           ByVal Prov As String, _
                                            ByVal Com As String, _
                                            ByVal Sezione As String, _
                                            ByVal Foglio As String, _
                                            ByVal Numero As String, _
                                            ByVal Subalterno As String, _
                                            ByVal SupCatastale As String, _
                                            ByVal SupCatastaleNEW As String, _
                                            ByVal Possesso As String, _
                                            ByVal PossessoNEW As String, _
                                            ByVal SupCondotta As String, _
                                            ByVal SupCondottaNEW As String, _
                                            ByVal Macrouso As String, _
                                            ByVal MacrousoNEW As String, _
                                            ByVal SupMacrouso As String, _
                                            ByVal SupMacrousoNEW As String, _
                                            ByVal Zona As String, _
                                            ByVal ZonaNEW As String, _
                                            ByVal Utilizzi As String, _
                                            ByVal UtilizziNEW As String)


        Dim DR As DataRow
        'Dim UNID_APP As String = ""
        'Dim UNID_APP_OLD As String = ""

        'Key_Appezzamento_SET(UNID_APP, Piva, Sa_Cod, Appezza, ID_Reg)

        DR = DT.NewRow

        'DR.Item("UNID_APP") = UNID_APP

        DR.Item("Flag_Presente") = Flag_Presente
        DR.Item("Flag_Modifica") = Flag_Modifica
        DR.Item("Sa_Nome") = Sa_Nome

        DR.Item("sigla_prov") = Provincia
        DR.Item("com_des") = Comune

        DR.Item("Prov") = Prov
        DR.Item("Com") = Com
        DR.Item("Sezione") = Sezione
        DR.Item("Foglio") = Foglio
        DR.Item("Numero") = Numero
        DR.Item("Subalterno") = Subalterno

        DR.Item("SupCatastale") = SupCatastale
        DR.Item("SupCatastale_NEW") = SupCatastaleNEW
        DR.Item("Possesso") = Possesso
        DR.Item("Possesso_NEW") = PossessoNEW
        DR.Item("SupCondotta") = SupCondotta
        DR.Item("SupCondotta_NEW") = SupCondottaNEW
        DR.Item("Macrouso") = Macrouso
        DR.Item("Macrouso_NEW") = MacrousoNEW
        DR.Item("SupMacrouso") = SupMacrouso
        DR.Item("SupMacrouso_NEW") = SupMacrousoNEW
        DR.Item("Zona") = Zona
        DR.Item("Zona_NEW") = ZonaNEW
        DR.Item("Utilizzi") = Utilizzi
        DR.Item("Utilizzi_NEW") = UtilizziNEW

        DT.Rows.Add(DR)

    End Sub

    '#############################################
    'restituisce il datatable con il risultato della verifica
    'prepara anche il log
    Public Sub Genera_Log_Catasto(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                   ByRef LogCatasto As StringBuilder, _
                                  ByRef DT As DataTable, _
                                ByVal ListParticelle As List(Of cls_ParticellaVerifica), _
                                ByVal Piva As String)

        Dim objParticella As cls_ParticellaVerifica
        Dim objImpPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
        Dim objZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objUtilizzi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim objTitolo As New AgronicaCoreMetaSchemaDAL.Titolo_Possesso_R
        Dim DT_ImpPart As DataTable
        Dim DT_Macrousi As DataTable
        Dim DT_Zone As DataTable
        Dim DT_Utilizzi As DataTable
        Dim sigla_prov As String = ""
        Dim com_des As String = ""
        Dim pro_des As String = ""
        Dim i, j As Integer

        Dim Sa_Cod_Temp As Integer = 0
        Dim Inizio_Temp As Date = CDate("12/12/1902")
        Dim Fine_Temp As Date = CDate("12/12/1903")
        Dim flag_modifica As Boolean
        Dim sa_nome As String = ""
        Dim SupCatastale As Double
        Dim TitoloPossesso As String
        Dim DatePossesso As String
        Dim Possesso As String = ""
        Dim SupCondotta As Double
        Dim Macrouso As String
        Dim SupMacrouso As Double
        Dim Zona As String
        Dim Utilizzo As String


        DT = Genera_DT_Particelle()


        If Not IsNothing(ListParticelle) AndAlso ListParticelle.Count > 0 Then

            Dim DrImpPart() As DataRow
            Dim DrMacrousi() As DataRow
            Dim DrZone() As DataRow
            Dim DrUtilizzi() As DataRow

            'controllo se la particella esiste già in archivio per l'impresa
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)


            DT_ImpPart = objImpPart.Leggi3(0, _
                                              Piva, _
                                              0, _
                                              0, _
                                              "", _
                                              "", _
                                              "", _
                                              0, _
                                              0, _
                                              "", _
                                               "", _
                                               " ImpresexParticelle.Piva, ImpresexParticelle.Sa_Cod, ImpresexParticelle.Validita_Inizio, ImpresexParticelle.Validita_Fine", _
                                                objParametri_Server)

            DT_Macrousi = objMacrousi.Leggi(Piva, _
                                            "", _
                                            "", _
                                            "", _
                                            0, _
                                            0, _
                                            "", _
                                            0, _
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                            "", _
                                            " Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Des", _
                                            objParametri_Server)

            DT_Zone = objZone.Leggi(0, _
                                    "", _
                                    "", _
                                    "", _
                                    0, _
                                    0, _
                                    "", _
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                    "", _
                                    " ParticelleCatastali.Prov, ParticelleCatastali.Com, ParticelleCatastali.Sezione, ParticelleCatastali.Foglio, ParticelleCatastali.Numero, ParticelleCatastali.Subalterno, Descrizione ", _
                                    objParametri_Server)


            DT_Utilizzi = objUtilizzi.Leggi(Piva, _
                                            "", _
                                            "", _
                                            "", _
                                            0, _
                                            0, _
                                            "", _
                                            0, _
                                            0, 0, _
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                            "", _
                                            " Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Des_Agea, Cul_Des_Agea ", _
                                            objParametri_Server)


            objParametri_Server.ResettaFinestra()


            'per ogni particella
            For i = 0 To ListParticelle.Count - 1

                sa_nome = ""
                Possesso = ""

                objParticella = ListParticelle(i)

                If Not IsNothing(objParticella) Then

                    objIstat.ComuneProvinciaSigla_from_codISTAT(objParticella.Prov, _
                                                            objParticella.Com, _
                                                            sigla_prov, _
                                                            pro_des, _
                                                            com_des, _
                                                            "", "", _
                                                            objParametri_Server _
                                                            )

                    DrImpPart = DT_ImpPart.Select(" Prov = '" & objParticella.Prov & "'" & _
                                                  " AND Com = '" & objParticella.Com & "'" & _
                                                  " AND Sezione = '" & objParticella.Sezione & "'" & _
                                                  " AND Foglio = " & objParticella.Foglio & "" & _
                                                  " AND Numero = " & objParticella.Numero & "" & _
                                                  " AND Subalterno = '" & objParticella.Subalterno & "'")

                    If Not IsNothing(DrImpPart) AndAlso DrImpPart.Length > 0 Then

                        '----------------------------------------------
                        '------------- la particella esiste -----------
                        '-----------------------------------------------

                        For j = 0 To DrImpPart.Length - 1

                            With DrImpPart(j)

                                flag_modifica = False

                                'per ogni centro e/o possesso

                                SupCatastale = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare(.Item("Ettari"), .Item("Are"), .Item("Centiare"))

                                If SupCatastale <> objParticella.SupCatastale Then
                                    flag_modifica = True
                                End If

                                If Sa_Cod_Temp = 0 Or Sa_Cod_Temp <> .Item("sa_cod") Then
                                    'primo giro o cambio centro
                                    sa_nome += .Item("sa_nome") + " "
                                    Sa_Cod_Temp = .Item("sa_cod")
                                End If

                                If (Inizio_Temp = CDate("12/12/1902") And Fine_Temp = CDate("12/12/1903")) Or _
                                    (Inizio_Temp <> .Item("Validita_Inizio") And Fine_Temp <> .Item("Validita_Fine")) Then
                                    'primo giro o cambio possessi
                                    Inizio_Temp = .Item("Validita_Inizio")
                                    Fine_Temp = .Item("Validita_Fine")
                                    SupCondotta = .Item("Sup_Condotta")
                                    DatePossesso = "Dal " + CStr(.Item("Validita_Inizio")) + " Al " + CStr(.Item("Validita_Fine"))
                                    TitoloPossesso = objTitolo.TitoloPossessoDes_from_TitoloPossessoCod(.Item("TitoloPossesso"))
                                    Possesso += TitoloPossesso + " " + DatePossesso + " Sup.condotta " + CStr(SupCondotta)
                                End If




                                'Macrouso=
                                'SupMacrouso =
                                'Zona =
                                'Utilizzo =

                            End With

                        Next

                        Inserisci_DT_Particelle(DT, _
                                               True, _
                                                 flag_modifica, _
                                                 sa_nome, _
                                                  sigla_prov, _
                                                  com_des, _
                                                    objParticella.Prov, _
                                                    objParticella.Com, _
                                                    objParticella.Sezione, _
                                                    objParticella.Foglio, _
                                                    objParticella.Numero, _
                                                    objParticella.Subalterno, _
                                                    SupCatastale, _
                                                   objParticella.SupCatastale, _
                                                   Possesso, _
                                                   objParticella.TitoloDatePossesso, _
                                                   0, _
                                                   objParticella.SupCondotta, _
                                                    "", _
                                                   objParticella.Macrouso, _
                                                   "", _
                                                   objParticella.SupMacrouso, _
                                                  "", _
                                                   objParticella.Zona, _
                                                 "", _
                                                   objParticella.Utilizzo)

                    Else
                        '----------------------------------------------
                        '--------- la particella non esiste -----------
                        '-----------------------------------------------

                        Inserisci_DT_Particelle(DT, _
                                                 False, _
                                                  False, _
                                                 "", _
                                                    sigla_prov, _
                                                    com_des, _
                                                      objParticella.Prov, _
                                                      objParticella.Com, _
                                                      objParticella.Sezione, _
                                                      objParticella.Foglio, _
                                                      objParticella.Numero, _
                                                      objParticella.Subalterno, _
                                                     0, _
                                                     objParticella.SupCatastale, _
                                                     "", _
                                                     objParticella.TitoloDatePossesso, _
                                                     0, _
                                                     objParticella.SupCondotta, _
                                                     "", _
                                                     objParticella.Macrouso, _
                                                     0, _
                                                     objParticella.SupMacrouso, _
                                                     "", _
                                                     objParticella.Zona, _
                                                     "", _
                                                     objParticella.Utilizzo)


                    End If 'controllo esistenza particella

                End If

            Next 'list particelle

        End If


    End Sub



#End Region


End Class


#End Region


#Region "SincronizzatoreCatasto"


'Public Enum enum_ImportazioneTipo
'    AnagrafeER = 1
'    Agrea = 2
'End Enum

Public Class cls_SincronizzatoreCatasto


    Public Sub Gestione_Sincronizzazione_Anagrafica(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Import As StringBuilder,
                                                    ByRef XmlDoc As XmlDocument,
                                                    ByRef XMLImpresa As XmlElement,
                                                    ByRef XMLCentro As XmlElement,
                                                    ByVal Utente_Username As String,
                                                    ByVal Utente_Password As String,
                                                    ByVal ProgressivoGIAS As Integer,
                                                    ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                    ByVal CodiceChiaveCliente As Integer,
                                                    ByVal ValCod_CodiceChiaveCliente As String,
                                                    ByVal Piva As String,
                                                    ByVal CUAA As String,
                                                    ByVal Piva_Padre As String,
                                                    ByVal Rag_Soc As String,
                                                    ByVal Indirizzo_Via As String,
                                                    ByVal Indirizzo_Cap As String,
                                                    ByVal Indirizzo_Prov As String,
                                                    ByVal Indirizzo_Com As String,
                                                    Path As String,
                                                    SchedaValidazione As String,
                                                    dataValidazione As Date,
                                                    detentoreFascicolo As String)

        Dim NomeRoutine As String = "cls_Importazioni.Gestione_Sincronizzazione_Anagrafica(): "

        Dim XmlUtente As System.Xml.XmlElement
        Dim XmlFascicolo As System.Xml.XmlElement
        'Dim XmlImpresa As System.Xml.XmlElement
        'Dim XmlCentro As System.Xml.XmlElement
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Try

            '-------------------------------
            '----- UTENTE
            '-------------------------------

            XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc,
                                                    Utente_Username,
                                                    Utente_Password,
                                                    ProgressivoGIAS)
            XmlDoc.AppendChild(XmlUtente)

            Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
            Dim DataChiusuraFascicolo As Date = AGRODATAFINE
            Dim DataInizioMandato As Date = AGRODATAINIZIO
            Dim DataFineMandato As Date = AGRODATAFINE

            '-------------------------------
            '----- IMPRESA
            '-------------------------------
            Dim Codice_Socio As String = "#"
            Dim Sup_Totale As String = "#"
            Dim Tipo_Gerarchia As String = enum_TipoImpresaGerarchia.Impresa
            Dim Titolo_Possesso As String = enum_TitoloPossesso.Proprieta
            Dim Validita_Inizio As String = "#"
            Dim Validita_Fine As String = "#"
            Dim Indirizzo_Frazione As String = "#"
            Dim cf_tecnico_referente As String = "#"
            Dim codice_cliente As String = "#"
            Dim codice_fornitore As String = "#"
            Dim codice_fornitore_2 As String = "#"
            Dim codice_fornitore_3 As String = "#"
            Dim codice_ausl As String = "#"
            Dim codice_Libro_Soci As String = "#"
            Dim Data_Iscrizione_Libro_Soci As String = "#"
            Dim i_Chiave_Cliente As String = ""

            If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'l'impresa è già presente in archivio
                'leggo i dati che potrebbero essere valorizzati e che quindi non vanno sovrascritti
                Dim objImpCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim DTCodici As DataTable

                DTCodici = objImpCodici.LeggiJoinCompleto(Piva,
                                                        0,
                                                        "", "",
                                                        objParametri_Server)

                If Not IsNothing(DTCodici) AndAlso DTCodici.Rows.Count > 0 Then
                    For i = 0 To DTCodici.Rows.Count - 1
                        If i = 0 Then
                            Sup_Totale = DTCodici.Rows(i).Item("Sup_Totale")
                            Tipo_Gerarchia = DTCodici.Rows(i).Item("TipoImpresaGerarchia")
                            'Validita_Inizio = DTCodici.Rows(i).Item("Validita_Inizio")
                            'Validita_Fine = DTCodici.Rows(i).Item("Validita_Fine")
                            Indirizzo_Frazione = DTCodici.Rows(i).Item("frz_des")
                        End If

                        Select Case DTCodici.Rows(i).Item("id_cod")

                            Case CodiceChiaveCliente
                                'mantengo il valore già salvato in archivio
                                i_Chiave_Cliente = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Socio
                                Codice_Socio = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.TitoloPossesso
                                Titolo_Possesso = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Tecnico
                                cf_tecnico_referente = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Cliente
                                codice_cliente = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Fornitore
                                codice_fornitore = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Fornitore_2
                                codice_fornitore_2 = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Fornitore_3
                                codice_fornitore_3 = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Ausl
                                codice_ausl = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Libro_Soci
                                codice_Libro_Soci = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Data_Iscrizione_Libro_Soci
                                Data_Iscrizione_Libro_Soci = DTCodici.Rows(i).Item("val_cod")
                        End Select

                    Next
                End If
                If i_Chiave_Cliente = "" Then
                    'sono in modifica e l'azienda non ha un val_cod del codice cliente valorizzato
                    '-> imposto quello inviato dall'import
                    i_Chiave_Cliente = ValCod_CodiceChiaveCliente
                End If
            Else
                'sono in scrittura, azienda non presente
                'valorizzato il val_cod del codice cliente con il valore inviato dall'importatore 
                '(ad esempio myISWSResponse.azienda.idAzienda per import agrea
                'myFascicolo.datiAnagrafici.idAzienda per import anagrafe)
                i_Chiave_Cliente = ValCod_CodiceChiaveCliente
            End If 'codici

            If i_Chiave_Cliente = "" Then
                'se dall'import non è stato inviato un codice, salvo val_cod = id_cod
                i_Chiave_Cliente = CodiceChiaveCliente
            End If


            XMLImpresa = objXML.Xml_Pubblico_Impresa(TipoOperazione,
                                                     Piva,
                                                     Rag_Soc,
                                                     CUAA,
                                                     CUAA,
                                                     Codice_Socio,
                                                     Sup_Totale,
                                                     Piva_Padre,
                                                     Tipo_Gerarchia,
                                                     Titolo_Possesso,
                                                     Validita_Inizio, Validita_Fine,
                                                     Indirizzo_Via,
                                                     Indirizzo_Frazione,
                                                     Indirizzo_Cap,
                                                     "#",
                                                     "#",
                                                     "#", "#",
                                                     Indirizzo_Com,
                                                     Indirizzo_Prov,
                                                     "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                     "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                     "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                     cf_tecnico_referente,
                                                     codice_cliente,
                                                     codice_fornitore,
                                                     codice_fornitore_2,
                                                     codice_fornitore_3,
                                                     codice_ausl,
                                                     i_Chiave_Cliente,
                                                     XmlDoc,
                                                     codice_Libro_Soci,
                                                    Data_Iscrizione_Libro_Soci)
            XmlUtente.AppendChild(XMLImpresa)

            XmlFascicolo = objXML.Xml_Pubblico_Fascicolo(Path,
                                                        SchedaValidazione,
                                                        dataValidazione,
                                                        detentoreFascicolo,
                                                        DataInizioMandato,
                                                        DataFineMandato,
                                                        DataAperturaFascicolo,
                                                        DataChiusuraFascicolo,
                                                        XmlDoc)
            XMLImpresa.AppendChild(XmlFascicolo)

            'Dim SaCod As Integer = 0
            'Dim SaNome As String = ""

            'If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
            '    SaCod = New AgronicaCoreAnagrafeDAL.Centri_Codici_Read().RecuperaSaCodImpresaByIdAziendaFascicolo( _
            '                                        CodiceChiaveCliente, _
            '                                        Piva, _
            '                                        myISWSResponse.azienda.idAzienda, _
            '                                        SaNome, _
            '                                        objParametri_Server)
            '    If SaNome = "" Then
            '        SaNome = "Centro n.01"
            '    End If
            'Else
            '    SaNome = "Centro n.01"
            'End If

            'XmlCentro = objXML.Xml_Pubblico_CentroAziendale(TipoOperazione, _
            '                                                SaCod, _
            '                                                SaNome, _
            '                                                1, _
            '                                                "#", "#", "#", "#", "#", "#", _
            '                                                myISWSResponse.azienda.indirizzo, _
            '                                                myISWSResponse.azienda.descComune, _
            '                                                myISWSResponse.azienda.cap, _
            '                                                myISWSResponse.azienda.descComune, _
            '                                                myISWSResponse.azienda.descProvincia, _
            '                                                "#", "#", _
            '                                                myISWSResponse.azienda.codIstatCom, _
            '                                                myISWSResponse.azienda.codIstatProv, _
            '                                                "#", "#", "#", "#", "#", "#", "#", "#", "#", _
            '                                                myISWSResponse.azienda.idAzienda, _
            '                                                XmlDoc)

            'XmlImpresa.AppendChild(XmlCentro)

            '-------------------------------
            '----- CENTRO
            '-------------------------------
            Dim Flag_CreaCentro As Boolean = False

            If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
                '        Dim SaCodByIdAziendaFascicolo As Integer = 0
                'non lo faccio più
                'SaCodByIdAziendaFascicolo = RecuperaSaCodImpresaByIdAziendaFascicolo(CodiceChiaveCliente, _
                '                                                                     myFascicolo.datiAnagrafici.partitaIva, _
                '                                                                     myFascicolo.datiAnagrafici.idAzienda, _
                '                                                                     objParametri_Server)

                'verifico se c'è almeno un centro in archivio, in caso contrario
                'preparo l'xml di creazione
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(Piva,
                                            0,
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente,
                    'la preparazione dell'xml del centro, avviene nella gestione catasto
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    Flag_CreaCentro = True
                End If
            Else
                'azienda non presente, devo creare tutto
                Flag_CreaCentro = True
            End If

            If Flag_CreaCentro = True Then
                Log_Import.Append("L'impresa non ha centri, verrà creato un centro e un magazzino." & vbCrLf)

                Dim SaCod As Integer = 0
                Dim SaNome As String = "Centro n.01"
                XMLCentro = objXML.Xml_Pubblico_CentroAziendale(enum_TipoOperazioneDB.Scrittura,
                                                                SaCod,
                                                                SaNome,
                                                                1,
                                                                "#", "#", "#", "#", "#", "#",
                                                                Indirizzo_Via,
                                                                 "#",
                                                                Indirizzo_Cap,
                                                                "#",
                                                                 "#",
                                                                "#", "#",
                                                                Indirizzo_Com,
                                                                Indirizzo_Prov,
                                                                "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                                i_Chiave_Cliente,
                                                                XmlDoc)


                XMLImpresa.AppendChild(XMLCentro)

                Dim XmlFabbricato As XmlElement
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
                                                            Indirizzo_Via,
                                                            "#",
                                                            Indirizzo_Cap,
                                                            "#",
                                                            "#",
                                                            "#", "#",
                                                            Indirizzo_Com,
                                                            Indirizzo_Prov,
                                                            i_Chiave_Cliente,
                                                            XmlDoc)

                XMLCentro.AppendChild(XmlFabbricato)
            End If


        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try


    End Sub

    '#####################################################################################################
    Public Sub Gestione_Sincronizzazione_Anagrafica(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                    ByRef Log_Errori As StringBuilder, _
                                                    ByRef Log_Import As StringBuilder, _
                                                    ByRef XmlDoc As XmlDocument, _
                                                    ByRef XMLImpresa As XmlElement, _
                                                    ByRef XMLCentro As XmlElement, _
                                                    ByVal Utente_Username As String, _
                                                    ByVal Utente_Password As String, _
                                                    ByVal ProgressivoGIAS As Integer, _
                                                    ByVal TipoOperazione As enum_TipoOperazioneDB, _
                                                    ByVal CodiceChiaveCliente As Integer, _
                                                    ByVal ValCod_CodiceChiaveCliente As String, _
                                                    ByVal Piva As String, _
                                                    ByVal CUAA As String, _
                                                    ByVal Piva_Padre As String, _
                                                    ByVal Rag_Soc As String, _
                                                    ByVal Indirizzo_Via As String, _
                                                    ByVal Indirizzo_Cap As String, _
                                                    ByVal Indirizzo_Prov As String, _
                                                    ByVal Indirizzo_Com As String)

        Dim NomeRoutine As String = "cls_Importazioni.Gestione_Sincronizzazione_Anagrafica(): "

        Dim XmlUtente As System.Xml.XmlElement
        'Dim XmlImpresa As System.Xml.XmlElement
        'Dim XmlCentro As System.Xml.XmlElement
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Try

            '-------------------------------
            '----- UTENTE
            '-------------------------------

            XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc, _
                                                    Utente_Username, _
                                                    Utente_Password, _
                                                    ProgressivoGIAS)
            XmlDoc.AppendChild(XmlUtente)

            '-------------------------------
            '----- IMPRESA
            '-------------------------------
            Dim Codice_Socio As String = "#"
            Dim Sup_Totale As String = "#"
            Dim Tipo_Gerarchia As String = enum_TipoImpresaGerarchia.Impresa
            Dim Titolo_Possesso As String = enum_TitoloPossesso.Proprieta
            Dim Validita_Inizio As String = "#"
            Dim Validita_Fine As String = "#"
            Dim Indirizzo_Frazione As String = "#"
            Dim cf_tecnico_referente As String = "#"
            Dim codice_cliente As String = "#"
            Dim codice_fornitore As String = "#"
            Dim codice_fornitore_2 As String = "#"
            Dim codice_fornitore_3 As String = "#"
            Dim codice_ausl As String = "#"
            Dim codice_Libro_Soci As String = "#"
            Dim Data_Iscrizione_Libro_Soci As String = "#"
            Dim i_Chiave_Cliente As String = ""

            If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'l'impresa è già presente in archivio
                'leggo i dati che potrebbero essere valorizzati e che quindi non vanno sovrascritti
                Dim objImpCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim DTCodici As DataTable

                DTCodici = objImpCodici.LeggiJoinCompleto(Piva, _
                                                        0, _
                                                        "", "", _
                                                        objParametri_Server)

                If Not IsNothing(DTCodici) AndAlso DTCodici.Rows.Count > 0 Then
                    For i = 0 To DTCodici.Rows.Count - 1
                        If i = 0 Then
                            Sup_Totale = DTCodici.Rows(i).Item("Sup_Totale")
                            Tipo_Gerarchia = DTCodici.Rows(i).Item("TipoImpresaGerarchia")
                            'Validita_Inizio = DTCodici.Rows(i).Item("Validita_Inizio")
                            'Validita_Fine = DTCodici.Rows(i).Item("Validita_Fine")
                            Indirizzo_Frazione = DTCodici.Rows(i).Item("frz_des")
                        End If

                        Select Case DTCodici.Rows(i).Item("id_cod")

                            Case CodiceChiaveCliente
                                'mantengo il valore già salvato in archivio
                                i_Chiave_Cliente = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Socio
                                Codice_Socio = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.TitoloPossesso
                                Titolo_Possesso = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Tecnico
                                cf_tecnico_referente = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Cliente
                                codice_cliente = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Fornitore
                                codice_fornitore = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Fornitore_2
                                codice_fornitore_2 = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Fornitore_3
                                codice_fornitore_3 = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Ausl
                                codice_ausl = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Codice_Libro_Soci
                                codice_Libro_Soci = DTCodici.Rows(i).Item("val_cod")

                            Case enum_CodiciAnagrafe.Data_Iscrizione_Libro_Soci
                                Data_Iscrizione_Libro_Soci = DTCodici.Rows(i).Item("val_cod")
                        End Select

                    Next
                End If
                If i_Chiave_Cliente = "" Then
                    'sono in modifica e l'azienda non ha un val_cod del codice cliente valorizzato
                    '-> imposto quello inviato dall'import
                    i_Chiave_Cliente = ValCod_CodiceChiaveCliente
                End If
            Else
                'sono in scrittura, azienda non presente
                'valorizzato il val_cod del codice cliente con il valore inviato dall'importatore 
                '(ad esempio myISWSResponse.azienda.idAzienda per import agrea
                'myFascicolo.datiAnagrafici.idAzienda per import anagrafe)
                i_Chiave_Cliente = ValCod_CodiceChiaveCliente
            End If 'codici

            If i_Chiave_Cliente = "" Then
                'se dall'import non è stato inviato un codice, salvo val_cod = id_cod
                i_Chiave_Cliente = CodiceChiaveCliente
            End If


            XMLImpresa = objXML.Xml_Pubblico_Impresa(TipoOperazione, _
                                                     Piva, _
                                                     Rag_Soc, _
                                                     CUAA, _
                                                     CUAA, _
                                                     Codice_Socio, _
                                                     Sup_Totale, _
                                                     Piva_Padre, _
                                                     Tipo_Gerarchia, _
                                                     Titolo_Possesso, _
                                                     Validita_Inizio, Validita_Fine, _
                                                     Indirizzo_Via, _
                                                     Indirizzo_Frazione, _
                                                     Indirizzo_Cap, _
                                                     "#", _
                                                     "#", _
                                                     "#", "#", _
                                                     Indirizzo_Com, _
                                                     Indirizzo_Prov, _
                                                     "#", "#", "#", "#", "#", "#", "#", "#", "#", _
                                                     "#", "#", "#", "#", "#", "#", "#", "#", "#", _
                                                     "#", "#", "#", "#", "#", "#", "#", "#", "#", _
                                                     cf_tecnico_referente, _
                                                     codice_cliente, _
                                                     codice_fornitore, _
                                                     codice_fornitore_2, _
                                                     codice_fornitore_3, _
                                                     codice_ausl, _
                                                     i_Chiave_Cliente, _
                                                     XmlDoc, _
                                                     codice_Libro_Soci, _
                                                    Data_Iscrizione_Libro_Soci)
            XmlUtente.AppendChild(XMLImpresa)


            'Dim SaCod As Integer = 0
            'Dim SaNome As String = ""

            'If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
            '    SaCod = New AgronicaCoreAnagrafeDAL.Centri_Codici_Read().RecuperaSaCodImpresaByIdAziendaFascicolo( _
            '                                        CodiceChiaveCliente, _
            '                                        Piva, _
            '                                        myISWSResponse.azienda.idAzienda, _
            '                                        SaNome, _
            '                                        objParametri_Server)
            '    If SaNome = "" Then
            '        SaNome = "Centro n.01"
            '    End If
            'Else
            '    SaNome = "Centro n.01"
            'End If

            'XmlCentro = objXML.Xml_Pubblico_CentroAziendale(TipoOperazione, _
            '                                                SaCod, _
            '                                                SaNome, _
            '                                                1, _
            '                                                "#", "#", "#", "#", "#", "#", _
            '                                                myISWSResponse.azienda.indirizzo, _
            '                                                myISWSResponse.azienda.descComune, _
            '                                                myISWSResponse.azienda.cap, _
            '                                                myISWSResponse.azienda.descComune, _
            '                                                myISWSResponse.azienda.descProvincia, _
            '                                                "#", "#", _
            '                                                myISWSResponse.azienda.codIstatCom, _
            '                                                myISWSResponse.azienda.codIstatProv, _
            '                                                "#", "#", "#", "#", "#", "#", "#", "#", "#", _
            '                                                myISWSResponse.azienda.idAzienda, _
            '                                                XmlDoc)

            'XmlImpresa.AppendChild(XmlCentro)

            '-------------------------------
            '----- CENTRO
            '-------------------------------
            Dim Flag_CreaCentro As Boolean = False

            If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
                '        Dim SaCodByIdAziendaFascicolo As Integer = 0
                'non lo faccio più
                'SaCodByIdAziendaFascicolo = RecuperaSaCodImpresaByIdAziendaFascicolo(CodiceChiaveCliente, _
                '                                                                     myFascicolo.datiAnagrafici.partitaIva, _
                '                                                                     myFascicolo.datiAnagrafici.idAzienda, _
                '                                                                     objParametri_Server)

                'verifico se c'è almeno un centro in archivio, in caso contrario
                'preparo l'xml di creazione
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(Piva, _
                                            0, _
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                            "", "", _
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente,
                    'la preparazione dell'xml del centro, avviene nella gestione catasto
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    Flag_CreaCentro = True
                End If
            Else
                'azienda non presente, devo creare tutto
                Flag_CreaCentro = True
            End If

            If Flag_CreaCentro = True Then
                Log_Import.Append("L'impresa non ha centri, verrà creato un centro e un magazzino." & vbCrLf)

                Dim SaCod As Integer = 0
                Dim SaNome As String = "Centro n.01"
                XMLCentro = objXML.Xml_Pubblico_CentroAziendale(enum_TipoOperazioneDB.Scrittura, _
                                                                SaCod, _
                                                                SaNome, _
                                                                1, _
                                                                "#", "#", "#", "#", "#", "#", _
                                                                Indirizzo_Via, _
                                                                 "#", _
                                                                Indirizzo_Cap, _
                                                                "#", _
                                                                 "#", _
                                                                "#", "#", _
                                                                Indirizzo_Com, _
                                                                Indirizzo_Prov, _
                                                                "#", "#", "#", "#", "#", "#", "#", "#", "#", _
                                                                i_Chiave_Cliente, _
                                                                XmlDoc)


                XMLImpresa.AppendChild(XMLCentro)

                Dim XmlFabbricato As XmlElement
                XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura, _
                                                            "0", _
                                                            "Magazzino n.01", _
                                                            20, _
                                                            "#", "#", "#", _
                                                            "#", _
                                                            "#", _
                                                            "#", _
                                                            AGRODATAINIZIO, _
                                                            AGRODATAFINE, _
                                                            Indirizzo_Via, _
                                                            "#", _
                                                            Indirizzo_Cap, _
                                                            "#", _
                                                            "#", _
                                                            "#", "#", _
                                                            Indirizzo_Com, _
                                                            Indirizzo_Prov, _
                                                            i_Chiave_Cliente, _
                                                            XmlDoc)

                XMLCentro.AppendChild(XmlFabbricato)
            End If


        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try


    End Sub

    '#####################################################################################################
    'chiamata per ogni particella
    Public Sub Gestione_Sincronizzazione_Catasto( _
                    ByVal TipoImport As enum_TipoImportazioneAnagrafe, _
                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                     ByRef XmlDoc As XmlDocument, _
                     ByRef XMLImpresa As XmlElement, _
                    ByRef XMLCentro As XmlElement, _
                      ByRef Str_Particelle As StringBuilder, _
                    ByRef Str_ParticelleEsistenti As StringBuilder, _
                    ByRef Str_ParticelleMancanti As StringBuilder, _
                    ByRef Str_ParticelleUtilizzi As StringBuilder, _
                    ByRef Str_ParticelleZone As StringBuilder, _
                    ByRef Str_ParticelleMacrousi As StringBuilder, _
                    ByRef num_ParticelleEsistenti As Integer, _
                    ByRef num_ParticelleMancanti As Integer, _
                     ByRef HT_ZoneXPart As Hashtable, _
                    ByRef HT_MacrousiXPart As Hashtable, _
                    ByRef HT_CodificaZoneMancanti As Hashtable, _
                     ByVal TipoOperazione As enum_TipoOperazioneDB, _
                     ByVal CodiceChiaveCliente As Integer, _
                    ByVal ValCod_CodiceChiaveCliente As String, _
                     ByVal Opt_Particelle_1Insert2Modifica As Integer, _
                     ByVal HT_PartCentri As Hashtable, _
                     ByVal Piva As String, _
                    ByVal Indirizzo_Via As String, _
                    ByVal Indirizzo_Cap As String, _
                    ByVal Indirizzo_Prov As String, _
                    ByVal Indirizzo_Com As String, _
                     ByVal PROV As String, _
                     ByVal COM As String, _
                     ByVal Sezione As String, _
                     ByVal Foglio As String, _
                     ByVal Numero As String, _
                     ByVal Subalterno As String, _
                     ByVal SupCatastale As Double, _
                     ByVal TitoloPossesso As Integer, _
                     ByVal supConduzione As Double, _
                    ByVal PossessoInizio As Date, _
                    ByVal PossessoFine As Date, _
                    ByVal Vet_CodZona() As String, _
                    ByVal ListaMacrouso As List(Of MacrousoObject), _
                    ByVal objXML As AgronicaCoreXML.AnagrafeXML, _
                    ByVal objImpresexParticelle As AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R, _
                    ByVal objCentri As AgronicaCoreAnagrafeDAL.CentriAziendali_Read, _
                    ByVal objCodifica As AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone, _
                    ByVal objZonexParticelle As AgronicaCoreAnagrafeDAL.ZonexParticelle_R, _
                    ByVal objMacrousixParticelle As AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R, _
                    ByVal objPartxMacrousixUtilizzi As AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R _
                     )

        Dim NomeRoutine As String = "cls_Importazioni.Gestione_Sincronizzazione_Catasto(): "

        '     ByVal SaCodByIdAziendaFascicolo As Integer, _

        'VARIABILI USATE INTERNAMENTE
        ' ByRef num_ParticelleSupCatModifica As Integer, _
        Dim XmlParticella As XmlElement
        Dim XmlZona, XmlMacrouso As XmlElement
        Dim XmlUtilizzo As XmlElement

        Dim TipoOperazione_Particella As enum_TipoOperazioneDB
        Dim key_part As String
        Dim strSezione As String
        Dim strSubalterno As String
        Dim Ettari As Double = 0
        Dim Are As Double = 0
        Dim Centiare As Double = 0
        Dim Sup_Catastale_OLD As Double
        Dim Str_Particella_InCorso As String

        Dim TipoOpZonaxParticella As enum_TipoOperazioneDB
        Dim bEsisteZona As Boolean
        Dim chiave_zonexpart As String
        Dim Cod_Zona_Origine As String
        Dim CodiceZonaGIAS As Integer

        Dim TipoOpMacrousixParticella As enum_TipoOperazioneDB
        Dim bEsisteMacrouso As Boolean
        Dim chiave_macrousoxpart As String
        Dim CodiceMacrousoGIAS As String
        Dim SupMacrouso As Double
        Dim Macrouso As MacrousoObject

        Dim TipoOp_PartxMacrousoxUtil As enum_TipoOperazioneDB
        Dim EsisteMacrousoxUtil As Boolean

        Dim Specie_Cod As String
        Dim Varieta_Cod As String
        Dim Utilizzo_Sup As Double

        Dim vet_SaCod(0) As Integer
        Dim DT_SaCod, Dt_Centri As DataTable
        Dim Sa_Cod_Scelto As Integer = 0
        Dim sa_nome, sup_bosco, sup_prati, validita_inizio, validita_fine, frazione As String
        Dim sacod As Integer
        ' Dim FiltroCentroProvCom As String

        Dim DataModifica As Date = Nothing
        Dim debug As Boolean = False
        Dim i, j, z, y As Integer

        Dim Titolo_Possesso As String = "#"
        Dim Cod_Operatore As String = "#"
        Dim Tipo_Attivita As String = "#"
        Dim Id_Chiave_Cliente As String = ""

        Try

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            'azzero
            Sup_Catastale_OLD = 0

            ''x debug
            'If (Particella.foglio = "81" And Particella.numero = "32") Or _
            '        (Particella.foglio = "81" And Particella.numero = "135") Then
            '    Debug = True
            'End If

            If IsNothing(Sezione) Or Sezione = "" Then
                strSezione = "0"
            Else
                strSezione = Sezione
            End If

            If IsNothing(Subalterno) Or Subalterno = "" Or Subalterno = "000" Then
                strSubalterno = "0"
            Else
                strSubalterno = Subalterno
            End If


            Str_Particella_InCorso = PROV + " " + _
                                    COM + " " + _
                                    strSezione + " " + _
                                    Foglio + " " + _
                                    Numero + " " + _
                                    strSubalterno

            'If SaCodByIdAziendaFascicolo <> 0 Then

            '    'qui non ci entra più, poichè non ricavo più SaCodByIdAziendaFascicolo 
            '    'viene usato il sa_cod selezionato nell'interfaccia

            '    vet_SaCod(0) = SaCodByIdAziendaFascicolo
            '    'il sa_cod è stato ricavato, dall'id_azienda salvato in corrispondenza del codice cliente sul centro aziendale
            '    'controllo che sia diverso da 0 perchè la query fa il where secco sul sa_cod (senza controllo <>0)

            '    If objImpresexParticelle.Esiste_Particella(Piva, _
            '                                               SaCodByIdAziendaFascicolo, _
            '                                               PROV, _
            '                                               COM, _
            '                                               strSezione, _
            '                                               Foglio, _
            '                                               Numero, _
            '                                               strSubalterno, _
            '                                               DataModifica, _
            '                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
            '                                                "", _
            '                                                objParametri_Server) Then

            '        TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica
            '    Else
            '        TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
            '    End If
            'Else 'SaCodByIdAziendaFascicolo

            If TipoOperazione = enum_TipoOperazioneDB.Modifica Then

                key_part = PROV + "|" + _
                          COM + "|" + _
                          strSezione + "|" + _
                          Foglio + "|" + _
                          Numero + "|" + _
                          strSubalterno

                If HT_PartCentri.Contains(key_part) Then
                    'la particella è nuova (non presente) e 
                    'ho scelto da interfaccia il centro in cui inserirla
                    Sa_Cod_Scelto = HT_PartCentri(key_part)
                    vet_SaCod(0) = Sa_Cod_Scelto
                    TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
                Else

                    'non ho mappato da interfaccia il centro.
                    'è il caso agrea
                    'devo capire se la particella è già presente in archivio
                    'o se la particella è nuova

                    'cerco  il sa_cod dalla tabella impresexparticelle
                    '(così verrà aggiornata in tutti i centri in cui è presente)

                    ''non cerco le particelle attive alla data di oggi, perchè magari alcune sono chiuse e le devo estendere
                    ''objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Date.Today, Date.Today)
                    'objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

                    'filtro_CAB_bagnacavallo = " (sa_cod = 73400377)"

                    DT_SaCod = objImpresexParticelle.DistinctCentri_Particella( _
                                                        Piva, _
                                                        0, _
                                                        PROV, _
                                                        COM, _
                                                        strSezione, _
                                                        Foglio, _
                                                        Numero, _
                                                        strSubalterno, _
                                                        "", "", _
                                                            objParametri_Server)

                    'spostato a fine gestione catasto
                    ' objParametri_Server.ResettaFinestra()

                    If Not IsNothing(DT_SaCod) AndAlso DT_SaCod.Rows.Count > 0 Then
                        Str_ParticelleEsistenti.Append(Str_Particella_InCorso + vbCrLf)
                        num_ParticelleEsistenti += 1

                        Sup_Catastale_OLD = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare( _
                                                DT_SaCod.Rows(0).Item("Ettari"), DT_SaCod.Rows(0).Item("Are"), DT_SaCod.Rows(0).Item("Centiare"))

                        TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica

                        ReDim vet_SaCod(DT_SaCod.Rows.Count - 1)
                        For z = 0 To DT_SaCod.Rows.Count - 1
                            vet_SaCod(z) = DT_SaCod.Rows(z).Item("Sa_Cod")
                        Next

                    Else
                        Str_ParticelleMancanti.Append(Str_Particella_InCorso + vbCrLf)
                        num_ParticelleMancanti += 1
                        'non esiste la particella in impresexparticelle
                        TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
                        ''commento   vet_SaCod(0) = 0
                        ''perchè devo battezzare un centro in cui salvare la particella
                        ''cerco il primo in ordine alfabetico
                        'vet_SaCod(0) = objCentri.SaCod_PrimoInOrdineAlfabeticoSaNome(Piva, objParametri_Server)
                        'lascio vet_SaCod(0) = 0
                        vet_SaCod(0) = 0
                        'viene cercato dopo un sa_cod
                    End If

                End If 'ht part centri

            Else
                vet_SaCod(0) = 0
                'sono in scrittura dell'azienda, il sa_cod è 0 
                '(verrà creato dal core e poi propagato alle particelle)
                TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
            End If

            'End If 'SaCodByIdAziendaFascicolo

            Dim Flag_Scrivi_Particelle As Boolean = True

            'se la particella non è presente, quindi da inserire
            'verifico se l'utente ha scelto SOLO di modificare le particelle già esistenti
            'If TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura And _
            '    Me.Rbl_Particelle_InsertModifica.SelectedValue = 1 Then
            If TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura And _
              Opt_Particelle_1Insert2Modifica = 2 Then
                'PRIMA:
                'Me.Rbl_Particelle.SelectedValue = 1 -> SOLO modifica
                'Me.Rbl_Particelle.SelectedValue = 0 -> inserimento e modifica
                'DOPO:
                'Me.Rbl_Particelle.SelectedValue = 2-> SOLO modifica
                'Me.Rbl_Particelle.SelectedValue = 1 -> inserimento e modifica
                Flag_Scrivi_Particelle = False
            End If

            If Flag_Scrivi_Particelle = True Then

                For z = 0 To vet_SaCod.Length - 1

                    'ok, ora occorre gestire il vettore di sa_cod
                    sacod = vet_SaCod(z)

                    'sa_nome = "_Centro Unico CAB"
                    'sa_nome = "Centro Aziendale"
                    sa_nome = "#"
                    sup_bosco = "#"
                    sup_prati = "#"
                    validita_inizio = "#"
                    validita_fine = "#"
                    frazione = "#"

                    'in caso di scrittura, ho già creato l'xml del centro (vedi sopra)
                    If TipoOperazione = enum_TipoOperazioneDB.Modifica Then

                        If sacod <> 0 Then

                            'leggo i dati del centro x evitare di sovrascriverli con altri
                            Dt_Centri = objCentri.Leggi(Piva, _
                                                        sacod, _
                                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                          "", "", _
                                                          objParametri_Server)

                            If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                                sa_nome = Dt_Centri.Rows(0).Item("sa_nome")
                                sup_bosco = Dt_Centri.Rows(0).Item("sup_bosco")
                                sup_prati = Dt_Centri.Rows(0).Item("sup_prati")
                                validita_inizio = Dt_Centri.Rows(0).Item("validita_inizio")
                                validita_fine = Dt_Centri.Rows(0).Item("validita_fine")
                                frazione = Dt_Centri.Rows(0).Item("frz_des")
                            End If

                        Else
                            'il sa_cod è 0 perchè l'impresa non è presente in archivio
                            'oppure l'impresa è presente, ma non è presente alcun centro
                            '( nel caso di import da anagrafe l'interfaccia di import non ha inviato alcun sa_cod nella HT
                            'oppure è stato cercato nell'archivio e non è stato trovato)
                            'oppure è il caso di agrea e va battezzato un centro

                            '1 prova: cerco un centro in cui è presente una particella in quel foglio
                            'se ci sono più centri prende il primo in ordine alfabetico
                            Dt_Centri = objImpresexParticelle.LeggiJoinCentriIndirizzi( _
                                                                0, _
                                                                Piva, _
                                                                0, _
                                                                0, _
                                                                PROV, COM, "", Foglio, 0, "", _
                                                                 "", " Sa_Nome ASC ", _
                                                                  objParametri_Server)

                            If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                                sacod = Dt_Centri.Rows(0).Item("sa_cod")
                                sa_nome = Dt_Centri.Rows(0).Item("sa_nome")
                                sup_bosco = Dt_Centri.Rows(0).Item("sup_bosco")
                                sup_prati = Dt_Centri.Rows(0).Item("sup_prati")
                                validita_inizio = Dt_Centri.Rows(0).Item("validita_inizio")
                                validita_fine = Dt_Centri.Rows(0).Item("validita_fine")
                                frazione = Dt_Centri.Rows(0).Item("frz_des")
                            Else
                                'non ci sono centri che hanno particelle con quel foglio
                                'cerco centri che hanno particelle con quel prov com
                                'se ci sono più centri prende il primo in ordine alfabetico
                                Dt_Centri = objImpresexParticelle.LeggiJoinCentriIndirizzi( _
                                                             0, _
                                                             Piva, _
                                                             0, _
                                                             0, _
                                                             PROV, COM, "", 0, 0, "", _
                                                              "", " Sa_Nome ASC ", _
                                                               objParametri_Server)

                                ''importo la particella nel primo centro che trovo con quel prov e com
                                'FiltroCentroProvCom = " Indirizzi.com_cod_istat = '" + Agro_SQL_SaveText(COM) + "'" + _
                                '                        " AND Indirizzi.pro_cod_istat = '" + Agro_SQL_SaveText(PROV) + "'"
                                'Dt_Centri = objCentri.Leggi(Piva, _
                                '                            0, _
                                '                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                '                              FiltroCentroProvCom, "", _
                                '                              objParametri_Server)

                                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                                    sacod = Dt_Centri.Rows(0).Item("sa_cod")
                                    sa_nome = Dt_Centri.Rows(0).Item("sa_nome")
                                    sup_bosco = Dt_Centri.Rows(0).Item("sup_bosco")
                                    sup_prati = Dt_Centri.Rows(0).Item("sup_prati")
                                    'validita_inizio = Dt_Centri.Rows(0).Item("centro_validita_inizio")
                                    'validita_fine = Dt_Centri.Rows(0).Item("centro_validita_fine")
                                    validita_inizio = Dt_Centri.Rows(0).Item("validita_inizio")
                                    validita_fine = Dt_Centri.Rows(0).Item("validita_fine")
                                    frazione = Dt_Centri.Rows(0).Item("frz_des")
                                Else
                                    'non ci sono  centri che hanno particelle con quel prov com
                                    'allora prendo il primo centro che trovo in ordine alfabetico
                                    Dt_Centri = objCentri.Leggi(Piva, _
                                                          0, _
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                            "", " Sa_Nome ASC ", _
                                                            objParametri_Server)

                                    If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                                        sacod = Dt_Centri.Rows(0).Item("sa_cod")
                                        sa_nome = Dt_Centri.Rows(0).Item("sa_nome")
                                        sup_bosco = Dt_Centri.Rows(0).Item("sup_bosco")
                                        sup_prati = Dt_Centri.Rows(0).Item("sup_prati")
                                        validita_inizio = Dt_Centri.Rows(0).Item("validita_inizio")
                                        validita_fine = Dt_Centri.Rows(0).Item("validita_fine")
                                        frazione = Dt_Centri.Rows(0).Item("frz_des")
                                    End If 'lettura centri

                                End If 'lettura con prov com

                            End If

                        End If 'controllo sa_cod

                        If TipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                            'questo è il caso di inserimento impresa
                            'l'import deve creare il centro e mettere le particelle sotto di questo
                            '(l'xml pubblico del centro è stato creato in genera stringone anagrafe)
                            debug = True
                        Else

                            If TipoOperazione = enum_TipoOperazioneDB.Modifica And sacod = 0 Then
                                'questo è il caso di modifica impresa con centro mancante
                                'l'import deve creare il centro e mettere le particelle sotto di questo
                                '(l'xml pubblico del centro è stato creato in genera stringone anagrafe)
                                debug = True
                            Else

                                'Dim objCentriCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                                'Dim DTCodici As DataTable

                                'DTCodici = objCentriCodici.Leggi(Piva, sacod, _
                                '                              0, "", "", _
                                '                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                '                             "", "", _
                                '                             objParametri_Server)

                                'If IsNothing(DTCodici) And DTCodici.Rows.Count > 0 Then
                                '    For i = 0 To DTCodici.Rows.Count - 1

                                '        Select Case DTCodici.Rows(i).Item("id_cod")

                                '            Case CodiceChiaveCliente
                                '                'mantengo il valore già salvato in archivio
                                '                Id_Chiave_Cliente = DTCodici.Rows(i).Item("val_cod")

                                '            Case enum_CodiciAnagrafe.TitoloPossesso
                                '                Titolo_Possesso = DTCodici.Rows(i).Item("val_cod")

                                '            Case enum_CodiciAnagrafe.CodiceCentro_Attuale
                                '                Cod_Operatore = DTCodici.Rows(i).Item("val_cod")

                                '            Case enum_CodiciAnagrafe.TipoAttivita
                                '                Tipo_Attivita = DTCodici.Rows(i).Item("val_cod")

                                '        End Select
                                '    Next
                                'End If

                                'If Id_Chiave_Cliente = "" Then
                                '    Id_Chiave_Cliente = ValCod_CodiceChiaveCliente
                                'End If

                                'se metto nel centro TipoOperazione = modifica
                                'poi mi inserisce tanti codici anagrafe sul centro quante sono le particelle!
                                'metto in centro in lattura, tanto serve solo per l'import/sincro del catasto
                                XMLCentro = objXML.Xml_Pubblico_CentroAziendale(enum_TipoOperazioneDB.Lettura, _
                                                                   sacod, _
                                                                   sa_nome, _
                                                                   Titolo_Possesso, _
                                                                   sup_bosco, sup_prati, _
                                                                   Cod_Operatore, _
                                                                   Tipo_Attivita, _
                                                                   validita_inizio, validita_fine, _
                                                                   Indirizzo_Via, _
                                                                   frazione, _
                                                                   Indirizzo_Cap, _
                                                                   "#", _
                                                                   "#", _
                                                                   "#", "#", _
                                                                   Indirizzo_Com, _
                                                                   Indirizzo_Prov, _
                                                                   "#", "#", "#", "#", "#", "#", "#", "#", "#", _
                                                                   Id_Chiave_Cliente, _
                                                                   XmlDoc) 'myFascicolo.datiAnagrafici.idAzienda
                                XMLImpresa.AppendChild(XMLCentro)

                            End If 'tipo modifica e sa_cod =0

                        End If 'tipo scrittura

                    End If 'se impresa in modifica

                    '-----------------------------------------------

                    'Dim iConduzione As Integer = 0
                    'iConduzione = Ricava_iConduzione(UltimaPartDellaRipetizione)

                    'TitoloPossesso = 0
                    'Select Case Tipo_Importazione
                    '    Case enum_ImportazioneTipo.AnagrafeER
                    '        TitoloPossesso = Converti_TitoliPossesso_Anagrafe(UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).formaPossesso)
                    '    Case enum_ImportazioneTipo.Agrea
                    '        TitoloPossesso = Converti_TitoliPossesso_Agrea(UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).formaPossesso)
                    '    Case Else
                    '        TitoloPossesso = 0
                    'End Select

                    'Dim supCatasto As Double = 0
                    'Dim supConduzione As Double = 0

                    'If Not IsNothing(SupCatastale) Then
                    '    supCatasto = SupCatastale / 10000.0

                    '    If Math.Abs(Sup_Catastale_OLD - supCatasto) < 0.00001 Then
                    '        'non significativa, dovuta al visual studio che fa vedere una sfilza di decimali
                    '    Else
                    '        'traccio nel log che è cambiata, giusto per fare dei test
                    '        Str_ParticelleSupCatModifica.Append(Str_Particella_InCorso + ": Sup cambiata da " + CStr(Sup_Catastale_OLD) + " a " + CStr(supCatasto) + vbCrLf + vbCrLf)
                    '        If supCatasto = 0 Then
                    '            supCatasto = Sup_Catastale_OLD
                    '            Str_ParticelleSupCatModifica.Append(Str_Particella_InCorso + ": la superficie non verrà cambiata per non mettere 0." + vbCrLf + vbCrLf)
                    '        Else
                    '            num_ParticelleSupCatModifica += 1
                    '        End If
                    '    End If

                    'End If

                    'If Not IsNothing(UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).supPossesso) Then
                    '    supConduzione = UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).supPossesso / 10000.0
                    'End If

                    If SupCatastale = 0 Then
                        Str_Particelle.Append(Str_Particella_InCorso & ": la superficie catastale è = 0 ha!" + vbCrLf)
                        If Sup_Catastale_OLD <> 0 Then
                            Str_Particelle.Append("La precedente superficie catastale era di " & CStr(Sup_Catastale_OLD) & " ha" + vbCrLf)
                        End If
                    Else
                        objXML.EttariAreCentiare_from_Ettari(SupCatastale, Ettari, Are, Centiare)
                    End If

                    If supConduzione = 0 Then
                        Str_Particelle.Append(Str_Particella_InCorso & ": la superficie condotta è = 0 ha!" + vbCrLf)
                    End If

                    'Dim PossessoInizio As String = "#"
                    'Dim PossessoFine As String = "#"

                    'If UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).dtInizio <> "" Then
                    '    PossessoInizio = UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).dtInizio
                    'End If

                    'If UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).dtFine <> "" Then
                    '    PossessoFine = UltimaPartDellaRipetizione.conduzione.contratti(iConduzione).dtFine
                    'End If
                    '-----------------------------------------------

                    XmlParticella = objXML.Xml_Pubblico_Particella(TipoOperazione_Particella, _
                                                                   "0", _
                                                                   COM, _
                                                                   PROV, _
                                                                   strSezione, _
                                                                   Foglio, _
                                                                   Numero, _
                                                                   strSubalterno, _
                                                                   "#", _
                                                                   Ettari, _
                                                                   Are, _
                                                                   Centiare, _
                                                                   TitoloPossesso, _
                                                                   "#", "#", "#", "#", _
                                                                   supConduzione, _
                                                                   PossessoInizio, _
                                                                   PossessoFine, _
                                                                   XmlDoc)

                    XMLCentro.AppendChild(XmlParticella)

                    If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica And _
                      XMLCentro.GetAttribute("codice_centro") = "0" Then
                        debug = True
                    End If


                    '-------------------------------
                    '----- ZONE

                    If Not IsNothing(Vet_CodZona) AndAlso Vet_CodZona.Length > 0 Then
                        ' If UltimaPartDellaRipetizione.conduzione.zoneLength <> 0 Then

                        ' For j = 0 To UltimaPartDellaRipetizione.conduzione.zoneLength - 1
                        For j = 0 To Vet_CodZona.Length - 1

                            'Cod_Zona_Fascicolo = UltimaPartDellaRipetizione.conduzione.zone(j).codZona
                            Cod_Zona_Origine = Vet_CodZona(j)

                            CodiceZonaGIAS = objCodifica.ConvertiZona(Cod_Zona_Origine, _
                                                                      "", objParametri_Server)
                            If CodiceZonaGIAS <> 0 Then

                                chiave_zonexpart = PROV + "|" + _
                                                  COM + "|" + _
                                                  strSezione + "|" + _
                                                  Foglio + "|" + _
                                                  Numero + "|" + _
                                                  strSubalterno + "|" + _
                                                  CStr(CodiceZonaGIAS)

                                If Not HT_ZoneXPart.Contains(chiave_zonexpart) Then
                                    'particella x zona non ancora inserita nell'xml

                                    HT_ZoneXPart.Add(chiave_zonexpart, "")

                                    If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                                        bEsisteZona = False
                                        bEsisteZona = objZonexParticelle.Esiste_ZonaxParticella(PROV, _
                                                                                              COM, _
                                                                                              strSezione, _
                                                                                              Foglio, _
                                                                                              Numero, _
                                                                                              strSubalterno, _
                                                                                              CodiceZonaGIAS, _
                                                                                              "", _
                                                                                              objParametri_Server)
                                    Else
                                        ' se la particella non esiste, allora non esiste nemmeno ZonexParticelle
                                        bEsisteZona = False
                                    End If

                                    TipoOpZonaxParticella = enum_TipoOperazioneDB.Scrittura

                                    If bEsisteZona = True Then
                                        TipoOpZonaxParticella = enum_TipoOperazioneDB.Modifica
                                    End If

                                    XmlZona = objXML.Xml_Pubblico_Zona(TipoOpZonaxParticella, _
                                                                       CodiceZonaGIAS.ToString, _
                                                                       "0", _
                                                                       XmlDoc)

                                    XmlParticella.AppendChild(XmlZona)

                                Else
                                    'particella x zona già inserita nell'xml
                                    'non considerare
                                    debug = True
                                End If
                            Else
                                'codice zona non trovato
                                debug = True
                                ' Str_ParticelleZone.Append("Codice Zona non trovato nella tabella di Codifica: " + Cod_Zona_Fascicolo + vbCrLf)
                                If Not HT_CodificaZoneMancanti.Contains(Cod_Zona_Origine) Then
                                    HT_CodificaZoneMancanti.Add(Cod_Zona_Origine, 0)
                                End If
                            End If

                        Next 'zone
                    Else
                        'non ci sono zone
                        debug = True
                        Str_ParticelleZone.Append(Str_Particella_InCorso + ": non ha zone." + vbCrLf)
                    End If

                    '-------------------------------
                    '----- MACROUSI
                    '-------------------------------
                    If Not IsNothing(ListaMacrouso) AndAlso ListaMacrouso.Count > 0 Then

                        For j = 0 To ListaMacrouso.Count - 1

                            Macrouso = ListaMacrouso(j)

                            If Not IsNothing(Macrouso) Then

                                'CodiceMacrousoGIAS = Right(UltimaPartDellaRipetizione.conduzione.macrousi(j).codMacrouso, 3)
                                'il right è già fatto, ma lo rifaccio per sicurezza
                                CodiceMacrousoGIAS = Right(Macrouso.CodMacrouso, 3)

                                If CodiceMacrousoGIAS <> "" And CodiceMacrousoGIAS <> "0" Then

                                    chiave_macrousoxpart = PROV + "|" + _
                                                            COM + "|" + _
                                                            strSezione + "|" + _
                                                            Foglio + "|" + _
                                                            Numero + "|" + _
                                                            strSubalterno + "|" + _
                                                            CStr(CodiceMacrousoGIAS)

                                    If Not HT_MacrousiXPart.Contains(chiave_macrousoxpart) Then
                                        'particella x macrouso non ancora inserita nell'xml

                                        HT_MacrousiXPart.Add(chiave_macrousoxpart, "")

                                        'SupMacrouso = CDbl(UltimaPartDellaRipetizione.conduzione.macrousi(j).supMacrouso) / 10000.0
                                        ' SupMacrouso = CDbl(Macrouso.SupMacrouso) / 10000.0
                                        'superficie passata già divisa
                                        SupMacrouso = CDbl(Macrouso.SupMacrouso)

                                        If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                                            bEsisteMacrouso = False
                                            bEsisteMacrouso = objMacrousixParticelle.Esiste_ParticelleCatastalixMacrousi(Piva, _
                                                                                                                         PROV, _
                                                                                                                         COM, _
                                                                                                                         strSezione, _
                                                                                                                         Foglio, _
                                                                                                                         Numero, _
                                                                                                                         strSubalterno, _
                                                                                                                         CodiceMacrousoGIAS, _
                                                                                                                         "", _
                                                                                                                         objParametri_Server)
                                        Else
                                            ' se la particella non esiste, allora non esiste nemmeno ParticellexMacrousi
                                            bEsisteMacrouso = False
                                        End If

                                        TipoOpMacrousixParticella = enum_TipoOperazioneDB.Scrittura
                                        If bEsisteMacrouso Then
                                            TipoOpMacrousixParticella = enum_TipoOperazioneDB.Modifica
                                        End If

                                        XmlMacrouso = objXML.Xml_Pubblico_Macrouso(TipoOpMacrousixParticella, _
                                                                                   Piva, _
                                                                                   CodiceMacrousoGIAS.ToString, _
                                                                                   SupMacrouso.ToString, _
                                                                                   #1/1/1900#, #12/31/2100#, XmlDoc)
                                        XmlParticella.AppendChild(XmlMacrouso)


                                        '-------------------------------
                                        '----- UTILIZZI

                                        If Not IsNothing(Macrouso.Utilizzo) AndAlso Macrouso.Utilizzo.Count > 0 Then

                                            'For z = 0 To myISWSResponse.possessi(i).macrousi(j).utilizzi.Length - 1
                                            For y = 0 To Macrouso.Utilizzo.Count - 1

                                                'Specie_Cod = myISWSResponse.possessi(i).macrousi(j).utilizzi(z).codColtura
                                                'Varieta_Cod = myISWSResponse.possessi(i).macrousi(j).utilizzi(z).codVarieta
                                                'Utilizzo_Sup = CDbl(myISWSResponse.possessi(i).macrousi(j).utilizzi(z).supUtilizzo) / 10000.0

                                                Specie_Cod = Macrouso.Utilizzo(y).SpecieCod
                                                Varieta_Cod = Macrouso.Utilizzo(y).VarietaCod

                                                Select Case TipoImport
                                                    Case -1
                                                        'gestire qui i casi che vanno divisi per 10000
                                                        Utilizzo_Sup = Macrouso.Utilizzo(y).SupUtilizzo / 10000.0
                                                    Case Else
                                                        Utilizzo_Sup = Macrouso.Utilizzo(y).SupUtilizzo
                                                End Select

                                                If TipoOpMacrousixParticella = enum_TipoOperazioneDB.Modifica Then
                                                    EsisteMacrousoxUtil = False
                                                    EsisteMacrousoxUtil = objPartxMacrousixUtilizzi.Esiste_PartxMacrousoxUtilizzo( _
                                                                                                                Piva, _
                                                                                                                PROV, _
                                                                                                                COM, _
                                                                                                                strSezione, _
                                                                                                                Foglio, _
                                                                                                                Numero, _
                                                                                                                strSubalterno, _
                                                                                                                CodiceMacrousoGIAS, _
                                                                                                                Specie_Cod, _
                                                                                                                Varieta_Cod, _
                                                                                                                "", _
                                                                                                                objParametri_Server)
                                                Else
                                                    ' se la particella non esiste, allora non esiste nemmeno ParticellexMacrousi
                                                    EsisteMacrousoxUtil = False
                                                End If

                                                TipoOp_PartxMacrousoxUtil = enum_TipoOperazioneDB.Scrittura
                                                If EsisteMacrousoxUtil = True Then
                                                    TipoOp_PartxMacrousoxUtil = enum_TipoOperazioneDB.Modifica
                                                End If

                                                XmlUtilizzo = objXML.Xml_Pubblico_Utilizzo(TipoOp_PartxMacrousoxUtil, _
                                                                                           Piva, _
                                                                                           Specie_Cod.ToString, _
                                                                                           Varieta_Cod.ToString, _
                                                                                           Format(Utilizzo_Sup, "0.0000"), _
                                                                                           #1/1/1900#, #12/31/2100#, "#", XmlDoc)

                                                XmlMacrouso.AppendChild(XmlUtilizzo)

                                            Next
                                        Else
                                            'non ci sono utilizzi
                                            Str_ParticelleUtilizzi.Append(Str_Particella_InCorso + " - Macrouso: " & CodiceMacrousoGIAS & ": utilizzi non presenti." + vbCrLf)
                                        End If 'utilizzi

                                    Else
                                        'particella x macrouso già inserita nell'xml
                                        'non considerare
                                        debug = True
                                    End If

                                Else
                                    'codice macrouso errato
                                    debug = True
                                    Str_ParticelleMacrousi.Append(Str_Particella_InCorso + ": codice Macrouso vuoto oppure =0." + vbCrLf)
                                End If

                            Else
                                debug = True
                                Str_ParticelleMacrousi.Append(Str_Particella_InCorso + ": non ha Macrousi." + vbCrLf)
                            End If

                        Next

                    Else
                        debug = True
                        Str_ParticelleMacrousi.Append(Str_Particella_InCorso + ": non ha Macrousi." + vbCrLf)
                    End If

                Next 'centri

            Else
                debug = True
            End If 'if temporaneo x non creazione particelle

            objParametri_Server.ResettaFinestra()

        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try

    End Sub



End Class

#End Region
