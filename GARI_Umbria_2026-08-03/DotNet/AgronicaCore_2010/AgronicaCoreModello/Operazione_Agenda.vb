Imports System.IO
Imports System.Security.Cryptography
Imports System.Transactions
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModello.AppHelper
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

' Giulia: 13/10/2017: Trasformate tutte le Properties in Auto-Implemented Properties per maggiore chiarezza e sintesi
'       per info: https://docs.microsoft.com/en-us/dotnet/visual-basic/programming-guide/language-features/procedures/auto-implemented-properties


Namespace OperazioneAgenda_Temp

    Public Class Operazione_Agenda

        Protected _Movimenti As List(Of Movimento)
        Protected _Note As List(Of Nota)
        Protected _Agenda_Riferimenti As List(Of Movimento_Dettaglio_Riferimento)
        Protected _GHG_Registrazioni As List(Of GHG_Registrazione)

        Sub New()

            Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            Id_Agenda = 0
            Data = AGRODATAINIZIO
            Piva = ""
            Sa_Cod = 0
            Lav_Cod = 0
            Des_Lib = ""
            Id_Attivita = 0
            Audit_Cod = 0
            Raccoglitore_Cod = 0
            Pratica_Cod = 0

            Blocco_Flag = 0
            Blocco_Username = ""
            Blocco_Data = AGRODATAINIZIO

            Linea_Cod = 0
            Preparazione_Cod = 0
            Id_Trasformazione = 0
            Tipo_Accettazione = 0
            Stato_Export = 0
            Stato_Export_2 = 0
            Tipo_Visibilita = 0
            ChkCoge_Manuale = 0
            Modulo = 0
            Split = 0


            Origine = ""
            Stato_Cod = 0
            DaRemoto = 0

            GisWkt = ""
            GisWktSistemaRiferimento = ""
            GisWktGps = ""
            GisTipoEntita_cod = 0
            GisLayerCod = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            _Movimenti = New List(Of Movimento)
            _Note = New List(Of Nota)
            _Agenda_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)
        End Sub

        Sub New(ByVal pivaInput As String, ByVal lavCod As Integer, ByVal dataOperazione As Date)

            Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            Id_Agenda = 0
            Data = dataOperazione
            Piva = pivaInput
            Sa_Cod = 0
            Lav_Cod = lavCod
            Des_Lib = ""
            Id_Attivita = 0
            Audit_Cod = 0
            Raccoglitore_Cod = 0
            Pratica_Cod = 0

            Blocco_Flag = 0
            Blocco_Username = ""
            Blocco_Data = AGRODATAINIZIO

            Linea_Cod = 0
            Preparazione_Cod = 0
            Id_Trasformazione = 0
            Tipo_Accettazione = 0
            Stato_Export = 0
            Stato_Export_2 = 0
            Tipo_Visibilita = 0
            ChkCoge_Manuale = 0
            Modulo = 0
            Split = 0

            Origine = ""
            Stato_Cod = 0
            DaRemoto = 0

            GisWkt = ""
            GisWktSistemaRiferimento = ""
            GisWktGps = ""
            GisTipoEntita_cod = 0
            GisLayerCod = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            _Movimenti = New List(Of Movimento)
            _Note = New List(Of Nota)
            _Agenda_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)
        End Sub

        Public Property Tipo_Operazione As Integer

        Public Property Piva As String

        Public Property Sa_Cod As Integer

        Public Property Id_Agenda As Integer

        Public Property Lav_Cod As Integer

        Public Property Des_Lib As String

        Public Property Blocco_Flag As Integer

        Public Property Blocco_Data As Date

        Public Property Blocco_Username As String

        Public Property Data As Date

        Public Property Linea_Cod As Integer

        Public Property Preparazione_Cod As Integer

        Public Property Id_Trasformazione As Integer

        Public Property Tipo_Accettazione As Integer

        Public Property Stato_Export As Integer

        Public Property Stato_Export_2 As Integer

        Public Property Tipo_Visibilita As Integer

        Public Property ChkCoge_Manuale As Integer

        Public Property Id_Attivita As Integer

        Public Property Modulo As Integer

        Public Property Audit_Cod As Integer

        Public Property Raccoglitore_Cod As Integer

        Public Property Pratica_Cod As Integer

        Public Property Split As Integer

        Public Property Origine As String

        Public Property Stato_Cod As Integer

        Public Property DaRemoto As Integer

        Public Property GisWkt As String

        Public Property GisWktSistemaRiferimento As String

        Public Property GisWktGps As String
        Public Property GisTipoEntita_cod As Integer
        Public Property GisLayerCod As Integer

        Public Property Data_Creazione As DateTime

        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property Invia_App As Integer 'DT: non esiste in tabella Agenda, serve per il ribaltamento su Ricette_Operazioni

        Public Property Movimenti() As List(Of Movimento)
            Get
                Return _Movimenti
            End Get
            Set(ByVal value As List(Of Movimento))
                _Movimenti = value
            End Set
        End Property

        Public Property Note() As List(Of Nota)
            Get
                Return _Note
            End Get
            Set(ByVal value As List(Of Nota))
                _Note = value
            End Set
        End Property

        Public Property Agenda_Riferimenti() As List(Of Movimento_Dettaglio_Riferimento)
            Get
                Return _Agenda_Riferimenti
            End Get
            Set(ByVal value As List(Of Movimento_Dettaglio_Riferimento))
                _Agenda_Riferimenti = value
            End Set
        End Property


        Public Property GHG_Registrazioni() As List(Of GHG_Registrazione)
            Get
                Return _GHG_Registrazioni
            End Get
            Set(ByVal value As List(Of GHG_Registrazione))
                _GHG_Registrazioni = value
            End Set
        End Property

    End Class


    '########################################################################################################

    Public Class Agenda_Operazione_Helper


        Public Function Genera_Agenda_Da_OperazioneRicetta(ByVal ricettaTipo As Integer,
                                                           ByVal dataRicetta As Date,
                                                           ByVal piva As String,
                                                           ByVal saCod As Integer,
                                                           ByVal baseCode As Integer,
                                                           ByVal topCode As Integer,
                                                           ByVal strRicettaOperazione As String,
                                                           ByRef strErr As String,
                                                           ByRef objParametriServer As AgronicaCoreParametri
                                                           ) As Operazione_Agenda


            Dim xmlDocOperazione As New XmlDocument
            Dim xmlRicettaOperazione As XmlElement

            Dim ricettaOperazioneCod As Integer
            Dim ricettaOperazioneDes As String
            Dim dataMovimento As Date
            Dim lavCod As Integer
            Dim numProtocollo As Integer
            Dim disciplinarePubblicoPrivato As Integer
            Dim extraIntMov As Integer
            Dim mezzo As Integer
            Dim emCod As Integer
            Dim effPerc As Integer

            Dim cauMov As String
            Dim hashCauMov As Hashtable

            Dim ricettaDettaglioCod As Integer
            Dim elemCod As Integer = 0
            Dim proCod As Integer = 0
            Dim matCod As Integer = 0
            Dim udmCod As Integer = 0
            Dim qta As Decimal = 0.0
            Dim extraInt As Integer
            Dim prezzoUnitario As Decimal
            Dim tempoCarenza As Integer
            Dim doseEtichetta As String
            Dim principiAttivi As String
            Dim classiTossicologiche As String
            Dim doseEtichettaValue As String
            Dim lotto As String
            Dim turnoCod As Integer
            Dim idAttivita As Integer
            Dim qualificaCod As Integer
            Dim tariffaCod As Integer

            Dim N As Decimal
            Dim P As Decimal
            Dim K As Decimal
            Dim M As Decimal
            Dim NnettoxHa As Decimal
            Dim NutilexHa As Decimal
            Dim efficienza As Decimal

            Dim avCod As Integer
            Dim avGru As Integer
            Dim sogliaCod As Integer
            Dim sogliaDes As String
            Dim sogliaQta As Decimal

            Dim qtaRil As Decimal

            Dim programmazioneEntitaCod As Integer
            Dim qtaDest As Decimal = 0.0
            Dim qta2Dest As Decimal = 0.0
            Dim quotaDistribuzione As Decimal = 0.0

            Dim pivaDest As String
            Dim saCodDest As Integer
            Dim appezzaDest As Integer
            Dim idRegDest As Integer

            xmlDocOperazione.LoadXml(strRicettaOperazione)
            xmlRicettaOperazione = xmlDocOperazione.SelectSingleNode("Ricetta_Operazione")

            ricettaOperazioneCod = CInt(xmlRicettaOperazione.GetAttribute("ricetta_operazione_cod"))
            ricettaOperazioneDes = CStr(xmlRicettaOperazione.GetAttribute("ricetta_operazione_des"))
            lavCod = CInt(xmlRicettaOperazione.GetAttribute("lav_cod"))
            dataMovimento = CDate(xmlRicettaOperazione.GetAttribute("validita_inizio"))
            numProtocollo = CInt(xmlRicettaOperazione.GetAttribute("num_protocollo"))
            disciplinarePubblicoPrivato = CInt(xmlRicettaOperazione.GetAttribute("disciplinare_pubblicoprivato"))
            extraIntMov = CInt(xmlRicettaOperazione.GetAttribute("extra_int"))
            emCod = CInt(xmlRicettaOperazione.GetAttribute("em_cod"))
            effPerc = CInt(xmlRicettaOperazione.GetAttribute("eff_perc"))
            mezzo = CInt(xmlRicettaOperazione.GetAttribute("mezzo"))

            Select Case ricettaTipo
                Case enum_TipoRicetta.PUA
                    'nelle ricette PUA l'epoca è salvata in em_cod
                    extraIntMov = emCod
                    dataMovimento = dataRicetta
            End Select

            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------
            Dim Agenda As Operazione_Agenda
            'Dim Nota As Nota
            Dim Movimento As Movimento
            Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
            Dim Movimento_Dettaglio As Movimento_Dettaglio
            Dim Movimento_Destinazione As Movimento_Destinazione

            Agenda = New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Id_Agenda = 0,
                .Data = dataMovimento,
                .Piva = piva,
                .Sa_Cod = saCod,
                .Lav_Cod = lavCod,
                .Des_Lib = ricettaOperazioneDes,
                .BaseCode = baseCode,
                .TopCode = topCode
            }



            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------
            Agenda.Movimenti = New List(Of Movimento)


            Dim xmlDatiRicettaDettagli As XmlElement
            Dim xmlRicettaDettaglio As XmlElement
            Dim xmLsRicettaDettaglio As XmlNodeList

            Dim xmLsRicettaDestinazione As XmlNodeList
            Dim xmlRicettaDestinazione As XmlElement

            Dim xmlDatiRicettaDettaglioTecnico As XmlElement
            Dim xmLsRicettaDettaglioTecnico As XmlNodeList
            Dim xmlRicettaDettaglioTecnico As XmlElement

            Dim xmLsRicettaDettaglioTecnico2 As XmlNodeList
            Dim xmlRicettaDettaglioTecnico2 As XmlElement

            xmlDatiRicettaDettaglioTecnico = xmlRicettaOperazione.SelectSingleNode("DatiRicetta_Dettagli_Tecnici")


            xmlDatiRicettaDettagli = xmlRicettaOperazione.SelectSingleNode("DatiRicetta_Dettagli")
            xmLsRicettaDettaglio = xmlDatiRicettaDettagli.GetElementsByTagName("Ricetta_Dettaglio")

            hashCauMov = New Hashtable

            'ricavo i cau_mov distinct x creare i movimenti in agenda poi
            For det = 0 To xmLsRicettaDettaglio.Count - 1
                xmlRicettaDettaglio = xmLsRicettaDettaglio.Item(det)
                cauMov = xmlRicettaDettaglio.GetAttribute("cau_mov")
                If Not hashCauMov.ContainsKey(cauMov) Then
                    hashCauMov.Add(cauMov, "")
                End If
            Next

            Dim cauMovKey As String

            For Each key In hashCauMov.Keys

                cauMovKey = key

                Movimento = New Movimento With {
                    .Piva = Agenda.Piva,
                    .Sa_Cod = Agenda.Sa_Cod,
                    .Data = Agenda.Data,
                    .Lav_Cod = lavCod,
                    .Cau_Mov = cauMovKey,
                    .Mov_Desc = "",
                    .Mezzo = mezzo,
                    .Num_Protocollo = numProtocollo,
                    .Disciplinare_PubblicoPrivato = disciplinarePubblicoPrivato,
                    .Extra_Int = extraIntMov,
                    .BaseCode = baseCode,
                    .TopCode = topCode
                }



                '---------------------------------------------------
                ' DETTAGLIO TECNICO (ACQUA)

                Movimento.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

                Select Case Movimento.Cau_Mov

                    Case CAU_LAVORAZIONE, CAU_TRATTAMENTO

                        If xmlDatiRicettaDettaglioTecnico IsNot Nothing Then
                            xmLsRicettaDettaglioTecnico = xmlDatiRicettaDettaglioTecnico.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")
                            For dettecn = 0 To xmLsRicettaDettaglioTecnico.Count - 1
                                xmlRicettaDettaglioTecnico = xmLsRicettaDettaglioTecnico.Item(dettecn)

                                Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                                qtaRil = xmlRicettaDettaglioTecnico.GetAttribute("qta_ril")

                                'avCod = XMLRicettaDettaglioTecnico2.GetAttribute("av_cod")
                                'avGru = XMLRicettaDettaglioTecnico2.GetAttribute("av_gru")
                                'sogliaCod = XMLRicettaDettaglioTecnico2.GetAttribute("soglia_cod")
                                'sogliaDes = XMLRicettaDettaglioTecnico2.GetAttribute("soglia_des")
                                'sogliaQta = XMLRicettaDettaglioTecnico2.GetAttribute("soglia_quantita")

                                Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                                Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                                Movimento_Dettaglio_Tecnico.Data = Agenda.Data

                                'Movimento_Dettaglio_Tecnico.Av_Cod = avCod
                                'Movimento_Dettaglio_Tecnico.Av_Gru = avGru

                                Movimento_Dettaglio_Tecnico.BaseCode = baseCode
                                Movimento_Dettaglio_Tecnico.TopCode = topCode

                                Movimento.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                            Next
                        End If

                End Select



                '---------------------------------------------------
                ' DETTAGLI

                Movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                'seleziono i dettagli del cau_mov corrente
                Dim xmLsRicettaDettTmp As XmlNodeList
                Dim xmlRicettaDettTmp As XmlElement

                xmLsRicettaDettTmp = xmlDatiRicettaDettagli.SelectNodes("child::Ricetta_Dettaglio[@cau_mov='" & cauMovKey & "']")

                For dett = 0 To xmLsRicettaDettTmp.Count - 1

                    xmlRicettaDettTmp = xmLsRicettaDettTmp.Item(dett)

                    ricettaDettaglioCod = xmlRicettaDettTmp.GetAttribute("ricetta_dettaglio_cod")
                    cauMov = xmlRicettaDettTmp.GetAttribute("cau_mov")
                    elemCod = xmlRicettaDettTmp.GetAttribute("elem_cod")
                    proCod = xmlRicettaDettTmp.GetAttribute("pro_cod")
                    matCod = xmlRicettaDettTmp.GetAttribute("mat_cod")
                    extraInt = xmlRicettaDettTmp.GetAttribute("extra_int")
                    qta = xmlRicettaDettTmp.GetAttribute("qta")
                    udmCod = xmlRicettaDettTmp.GetAttribute("udm_cod")

                    prezzoUnitario = xmlRicettaDettTmp.GetAttribute("prezzo_unitario")
                    tempoCarenza = xmlRicettaDettTmp.GetAttribute("tempocarenza")
                    doseEtichetta = xmlRicettaDettTmp.GetAttribute("doseetichetta")
                    principiAttivi = xmlRicettaDettTmp.GetAttribute("principiattivi")
                    classiTossicologiche = xmlRicettaDettTmp.GetAttribute("classitossicologiche")
                    doseEtichettaValue = xmlRicettaDettTmp.GetAttribute("doseetichetta_value")
                    lotto = xmlRicettaDettTmp.GetAttribute("lotto")
                    turnoCod = xmlRicettaDettTmp.GetAttribute("turno_cod")
                    idAttivita = xmlRicettaDettTmp.GetAttribute("id_attivita")
                    qualificaCod = xmlRicettaDettTmp.GetAttribute("qualifica_cod")
                    tariffaCod = xmlRicettaDettTmp.GetAttribute("tariffa_cod")

                    Select Case ricettaTipo
                        Case enum_TipoRicetta.PUA
                            'nelle ricette PUA la dose è salvata in litri o chili, ma devo salvare nell'agenda in m3 o quintali
                            If extraInt = 19 Then
                                qta = qta / 1000        'm3
                            ElseIf extraInt = 4 Then
                                qta = qta / 100         'q
                            End If
                    End Select

                    Movimento_Dettaglio = New Movimento_Dettaglio With {
                        .Piva = Agenda.Piva,
                        .Sa_Cod = Agenda.Sa_Cod,
                        .Data = Agenda.Data,
                        .Lav_Cod = lavCod,
                        .Cau_Mov = cauMov,
                        .Elem_Cod = elemCod,
                        .Pro_Cod = proCod,
                        .Mat_Cod = matCod,
                        .Lotto = lotto,
                        .TempoCarenza = tempoCarenza,
                        .DoseEtichetta = doseEtichetta,
                        .PrincipiAttivi = principiAttivi,
                        .CLassiTossicologiche = classiTossicologiche,
                        .DoseEtichetta_Value = doseEtichettaValue,
                        .Turno_Cod = turnoCod,
                        .ID_Attivita = idAttivita,
                        .Qualifica_Cod = qualificaCod,
                        .Tariffa_Cod = tariffaCod,
                        .Extra_Int = extraInt,
                        .Udm_Cod = udmCod,
                        .Qta = qta,
                        .Prezzo_Unitario = prezzoUnitario,
                        .BaseCode = baseCode,
                        .TopCode = topCode
                    }



                    '---------------------------------------------------
                    ' DESTINAZIONI

                    Movimento_Dettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                    xmLsRicettaDestinazione = xmlRicettaDettTmp.GetElementsByTagName("Ricetta_Destinazione")


                    For dest = 0 To xmLsRicettaDestinazione.Count - 1

                        xmlRicettaDestinazione = xmLsRicettaDestinazione.Item(dest)

                        Movimento_Destinazione = New Movimento_Destinazione

                        programmazioneEntitaCod = xmlRicettaDestinazione.GetAttribute("programmazione_entita_cod")
                        pivaDest = xmlRicettaDestinazione.GetAttribute("piva")
                        saCodDest = xmlRicettaDestinazione.GetAttribute("sa_cod")
                        appezzaDest = xmlRicettaDestinazione.GetAttribute("appezza")
                        idRegDest = xmlRicettaDestinazione.GetAttribute("id_reg")

                        qtaDest = xmlRicettaDestinazione.GetAttribute("qta")
                        quotaDistribuzione = xmlRicettaDestinazione.GetAttribute("quotadistribuzione")

                        ' controllo che sia stato ribaltato l'appezzamento e recupero la chiave
                        Dim objRegImpProgr As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
                        Dim dtRegImp As DataTable
                        dtRegImp = objRegImpProgr.Leggi(Agenda.Piva,
                                                        saCod,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        programmazioneEntitaCod,
                                                        "", "",
                                                        objParametriServer)

                        If Not IsNothing(dtRegImp) AndAlso dtRegImp.Rows.Count <> 0 Then
                            pivaDest = dtRegImp.Rows.Item(0).Item("piva")
                            saCodDest = dtRegImp.Rows.Item(0).Item("sa_cod")
                            appezzaDest = dtRegImp.Rows.Item(0).Item("Appezza")
                            idRegDest = dtRegImp.Rows.Item(0).Item("Id_Reg")
                        End If

                        'sup trattata non ancora gestita nelle ricette
                        'leggo il sup_app (quando ribalto il planning app e imp hanno stessa sup = sup entita planning)
                        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                        qta2Dest = objApp.Superficie_from_PivaSaCodAppezza(pivaDest, saCodDest, appezzaDest, objParametriServer)

                        'verificare scarico magazzino
                        Movimento_Destinazione.Piva = pivaDest
                        Movimento_Destinazione.Sa_Cod = saCodDest
                        Movimento_Destinazione.Appezza = appezzaDest
                        Movimento_Destinazione.Id_Destinazione = idRegDest

                        'HACK: anche se viene impostato a scopo di lettura, non sarà salvato su db se l'oggetto viene creato a scopo scrittura .
                        Movimento_Destinazione.Programmazione_Entita_Cod = programmazioneEntitaCod


                        '??????????
                        Movimento_Destinazione.Tipo = 0

                        Select Case Movimento_Dettaglio.Cau_Mov
                            Case CAU_SCARICO
                                Movimento_Destinazione.Tipo = MAGAZZINO
                        End Select

                        Movimento_Destinazione.Qta2 = qta2Dest
                        Movimento_Destinazione.Qta = qtaDest

                        Movimento_Destinazione.BaseCode = baseCode
                        Movimento_Destinazione.TopCode = topCode

                        Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                    Next


                    '---------------------------------------------------
                    ' DETTAGLIO TECNICO

                    Movimento_Dettaglio.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)


                    xmLsRicettaDettaglioTecnico2 = xmlRicettaDettTmp.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")


                    For dettecn2 = 0 To xmLsRicettaDettaglioTecnico2.Count - 1

                        xmlRicettaDettaglioTecnico2 = xmLsRicettaDettaglioTecnico2.Item(dettecn2)

                        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                        N = xmlRicettaDettaglioTecnico2.GetAttribute("n")
                        P = xmlRicettaDettaglioTecnico2.GetAttribute("p")
                        K = xmlRicettaDettaglioTecnico2.GetAttribute("k")
                        M = xmlRicettaDettaglioTecnico2.GetAttribute("mg")

                        NnettoxHa = xmlRicettaDettaglioTecnico2.GetAttribute("nnettoxha")
                        NutilexHa = xmlRicettaDettaglioTecnico2.GetAttribute("nutilexha")

                        efficienza = xmlRicettaDettaglioTecnico2.GetAttribute("efficienza")

                        Select Case ricettaTipo
                            Case enum_TipoRicetta.PUA
                                efficienza = effPerc / 100
                        End Select

                        qtaRil = xmlRicettaDettaglioTecnico2.GetAttribute("qta_ril")

                        avCod = xmlRicettaDettaglioTecnico2.GetAttribute("av_cod")
                        avGru = xmlRicettaDettaglioTecnico2.GetAttribute("av_gru")
                        sogliaCod = xmlRicettaDettaglioTecnico2.GetAttribute("soglia_cod")
                        sogliaDes = xmlRicettaDettaglioTecnico2.GetAttribute("soglia_des")
                        sogliaQta = xmlRicettaDettaglioTecnico2.GetAttribute("soglia_quantita")

                        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                        Movimento_Dettaglio_Tecnico.Data = Agenda.Data

                        Movimento_Dettaglio_Tecnico.N = N
                        Movimento_Dettaglio_Tecnico.P = P
                        Movimento_Dettaglio_Tecnico.K = K
                        Movimento_Dettaglio_Tecnico.M = M
                        Movimento_Dettaglio_Tecnico.Efficienza = efficienza

                        Movimento_Dettaglio_Tecnico.Av_Cod = avCod
                        Movimento_Dettaglio_Tecnico.Av_Gru = avGru
                        Movimento_Dettaglio_Tecnico.Soglia_Cod = sogliaCod
                        Movimento_Dettaglio_Tecnico.Soglia_Des = sogliaDes
                        Movimento_Dettaglio_Tecnico.Soglia_Quantita = sogliaQta




                        Movimento_Dettaglio_Tecnico.BaseCode = baseCode
                        Movimento_Dettaglio_Tecnico.TopCode = topCode

                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                    Next

                    Movimento.Movimenti_Dettagli.Add(Movimento_Dettaglio)

                Next

                Agenda.Movimenti.Add(Movimento)

            Next


            ''------------------------------------------------
            ''----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
            ''------------------------------------------------

            'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

            'Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

            ''---------------------------------------------------
            ''----- Acqua  
            ''---------------------------------------------------

            'Dim SuperficieTotaleCentro As Decimal = 0

            'For i = 0 To Dt.Rows.Count - 1
            '    SuperficieTotaleCentro = SuperficieTotaleCentro + Dt.Rows(i).Item("sup_imp")
            'Next

            'Dim Acqua As Decimal = 0
            'Dim AcquaTot As Decimal = 0
            'If Me.Txt_Acqua_Difesa.Text <> "" Then
            '    'se è stata scelta la dose d'acqua/ha salvo il dato negativo
            '    Select Case Me.RBL_Acqua_Formulati.SelectedValue
            '        Case "0"
            '            Acqua = CDbl(Me.Txt_Acqua_Difesa.Text)
            '            AcquaTot = Acqua
            '        Case "1"
            '            Acqua = -CDbl(Me.Txt_Acqua_Difesa.Text)
            '            AcquaTot = Math.Abs(Acqua) * SuperficieTotaleCentro
            '    End Select
            'End If

            'Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
            'Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
            'Movimento_Dettaglio_Tecnico.Data = CDate(Me.Txt_Data_Interventi.Text)
            'Movimento_Dettaglio_Tecnico.Qta_Ril = CDbl(Acqua)
            'Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
            'Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

            'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

            ''------------------------------------------------
            ''----- MOVIMENTI DETTAGLI
            ''------------------------------------------------

            'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)



            Return Agenda

        End Function


        ''' <summary>
        ''' flagUsaOraReale = True => nella colonna Ora della Tab Movimenti mette effettivamente l'Ora passata, altrimenti mette la data di Scadenza
        ''' </summary>
        ''' <param name="Agenda">Oggetto Agenda</param>
        ''' <param name="objParametri"></param>
        ''' <param name="flagUsaOraReale">True = nella colonna Ora della Tab Movimenti mette effettivamente l'Ora passata, altrimenti mette la data di Scadenza</param>
        ''' <param name="idServizio">Default = enum_Id_Servizio.GiasOnline</param>
        ''' <param name="flagScriviSempreLotto">True = nella Movimenti_Dettagli scrivo sempre il lotto per i FITOFARMACI se passato; se non presente mette "INDEFINITO"</param>
        ''' <param name="documentoPrevisionale">True = Documento con data futura, ma che va registrato normalmente, come se fosse in data di oggi</param>
        ''' <param name="scriviRiferimenti">True = Scrive Mov_Dettagli_Riferimenti se passati</param>
        ''' <returns>Id_Agenda assegnata all'operazione scritta</returns>
        ''' <remarks>flagUsaOraReale = True => nella colonna Ora della Tab Movimenti mette effettivamente l'Ora passata, altrimenti mette la data di Scadenza</remarks>
        Public Function Scrivi(ByRef Agenda As Operazione_Agenda,
                               ByVal objParametri As AgronicaCoreParametri,
                               Optional ByVal flagUsaOraReale As Boolean = False,
                               Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                               Optional ByVal flagScriviSempreLotto As Boolean = False,
                               Optional ByVal documentoPrevisionale As Boolean = False,
                               Optional ByVal scriviRiferimenti As Boolean = False,
                               Optional ByVal IdAgendaCopiata_xLOG As String = "",
                               Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.gias
                               ) As Integer

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim idAgenda As Integer
            Dim i As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'AGENDA
                '---------------------------

                idAgenda = Agenda.Id_Agenda

                If idAgenda <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    idAgenda = objSequenze.NuovoId_Tabella("Agenda",
                                                           Agenda.BaseCode,
                                                           Agenda.TopCode,
                                                           objParametri)

                    objSequenze = Nothing

                End If

                Dim pianificazione As String = ""
                If Agenda.Data > CDate(Now) AndAlso Not documentoPrevisionale Then
                    pianificazione = Gias.Pianificazione & " "
                End If

                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                objAgenda.Scrivi(Agenda.Piva,
                                 Agenda.Sa_Cod,
                                 idAgenda,
                                 Agenda.Lav_Cod,
                                 Agenda.Linea_Cod,
                                 Agenda.Preparazione_Cod,
                                 Agenda.Id_Trasformazione,
                                 pianificazione & Agenda.Des_Lib,
                                 Agenda.Tipo_Accettazione,
                                 Agenda.Blocco_Flag,
                                 Agenda.Blocco_Username,
                                 Agenda.Blocco_Data,
                                 Validita_Inizio:=Agenda.Data,
                                 Validita_Fine:=AGRODATAFINE,
                                 objParametri:=objParametri,
                                 Audit_Cod:=Agenda.Audit_Cod,
                                 Stato_Export:=Agenda.Stato_Export,
                                 Stato_Export_2:=Agenda.Stato_Export_2,
                                 Tipo_Visibilita:=Agenda.Tipo_Visibilita,
                                 ChkCoge_Manuale:=Agenda.ChkCoge_Manuale,
                                 Id_Attivita:=Agenda.Id_Attivita,
                                 Modulo:=Agenda.Modulo,
                                 Data_creazione:=Agenda.Data_Creazione,
                                 username_creazione:=Agenda.Username_Creazione,
                                 Raccoglitore_Cod:=Agenda.Raccoglitore_Cod,
                                 Split:=Agenda.Split,
                                 Pratica_Cod:=Agenda.Pratica_Cod,
                                 Origine:=Agenda.Origine,
                                 Stato_Cod:=Agenda.Stato_Cod,
                                 DaRemoto:=Agenda.DaRemoto
                                )

                objAgenda = Nothing

                If Agenda.GisWkt <> "" Then
                    Dim scriviGIS As New GisHelper

                    Dim operazione = My.Resources.AgronicaCoreModelloRes.Operazione
                    Dim data = My.Resources.AgronicaCoreModelloRes.Data
                    Dim ElementoGrafico_Des As String = operazione & "§ " & Agenda.Des_Lib & "|" & data & "§ " & Agenda.Data.ToShortDateString & "|"

                    Dim EsitoGis As RispostaStandard =
                        scriviGIS.ScriviDatoCartografico(
                            objParametri,
                            0,
                            0,
                            Agenda.Piva,
                            Agenda.Sa_Cod,
                            0,
                            0,
                            idAgenda,
                            0,
                            Agenda.GisWkt,
                            Agenda.GisWktSistemaRiferimento,
                            Agenda.GisWktGps,
                            Agenda.GisLayerCod,
                            Agenda.GisTipoEntita_cod,
                            0, 0, enum_TipoOperazioneDB.Scrittura, ElementoGrafico_Des, swapLatLong:=True
                       )

                    If Not EsitoGis.RispostaOK Then
                        Throw New Exception("[ Agenda_Operazione_Helper.ScriviDatoCartografico() ] : " & EsitoGis.Errore)
                    End If

                End If


                If True Then

                    '---------------------------
                    'AGENDA LOG 
                    '---------------------------

                    Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                    objAgronicaLogAgendaW.Scrivi(Agenda.Data,
                                                 Agenda.Tipo_Operazione,
                                                 Agenda.Des_Lib & IdAgendaCopiata_xLOG,
                                                 idAgenda,
                                                 Agenda.Piva,
                                                 Agenda.Sa_Cod,
                                                 Agenda.Lav_Cod,
                                                 CInt(idServizio),
                                                 objParametri,
                                                 Agenda,
                                                 CInt(origine),
                                                 Agenda.Raccoglitore_Cod)

                    objAgronicaLogAgendaW = Nothing

                    '---------------------------
                    'NOTE 
                    '---------------------------
                    If Not IsNothing(Agenda.Note) AndAlso Agenda.Note.Count > 0 Then

                        For i = 0 To Agenda.Note.Count - 1

                            Agenda.Note(i).Id_Agenda = idAgenda

                            Dim objNota As New Agenda_Note_Helper
                            objNota.Scrivi(Agenda.Note(i), objParametri)
                            objNota = Nothing

                        Next

                    End If

                    '---------------------------
                    'MOVIMENTI
                    '---------------------------

                    If Not IsNothing(Agenda.Movimenti) AndAlso Agenda.Movimenti.Count > 0 Then

                        For i = 0 To Agenda.Movimenti.Count - 1

                            Agenda.Movimenti(i).Id_Agenda = idAgenda

                            Dim objMovimento As New Agenda_Movimenti_Helper
                            objMovimento.Scrivi(Agenda.Movimenti(i), objParametri, flagUsaOraReale, flagScriviSempreLotto, documentoPrevisionale)
                            objMovimento = Nothing

                        Next

                    End If

                    '------------------------------
                    'MOVIMENTI DETTAGLI RIFERIMENTI
                    '------------------------------

                    If scriviRiferimenti AndAlso Not IsNothing(Agenda.Agenda_Riferimenti) Then

                        For i = 0 To Agenda.Agenda_Riferimenti.Count - 1

                            Agenda.Agenda_Riferimenti(i).Id_Agenda = idAgenda

                            Dim objMovDetRiferimenti = New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                            objMovDetRiferimenti.Scrivi(Agenda.Agenda_Riferimenti(i).Piva,
                                                        Agenda.Agenda_Riferimenti(i).Sa_Cod,
                                                        Agenda.Agenda_Riferimenti(i).Id_Agenda,
                                                        Agenda.Agenda_Riferimenti(i).Id_Mov,
                                                        Agenda.Agenda_Riferimenti(i).Id_Mov_Det,
                                                        Agenda.Agenda_Riferimenti(i).Lav_Cod,
                                                        Agenda.Agenda_Riferimenti(i).Cau_Mov,
                                                        Agenda.Agenda_Riferimenti(i).Piva_Rif,
                                                        Agenda.Agenda_Riferimenti(i).Sa_Cod_Rif,
                                                        Agenda.Agenda_Riferimenti(i).Id_Agenda_Rif,
                                                        Agenda.Agenda_Riferimenti(i).Id_Mov_Rif,
                                                        Agenda.Agenda_Riferimenti(i).Id_Mov_Det_Rif,
                                                        Agenda.Agenda_Riferimenti(i).Lav_Cod_Rif,
                                                        Agenda.Agenda_Riferimenti(i).Cau_Mov_Rif,
                                                        0,
                                                        Agenda.Data,
                                                        AGRODATAFINE,
                                                        objParametri)

                            objMovDetRiferimenti = Nothing

                        Next

                    End If

                End If

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Operazione_Helper.scrivi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return idAgenda


        End Function

        Public Function LeggiLista(ByVal piva As String, ByVal lavCod As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of Operazione_Agenda)

            Dim nomeRoutine As String = "Agenda_Operazione_Helper.LeggiLista()"

            Dim listaAgenda As New List(Of Operazione_Agenda)

            Dim flagConnessione As Boolean = False

            Try

                '---------------------------
                Utility.VerificaApriConnessione(objParametri, flagConnessione)
                '---------------------------


                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
                Dim dtAgenda As DataTable

                dtAgenda = objAgenda.Leggi(CStr(piva),
                                           0,
                                           0,
                                           CInt(lavCod),
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           "",
                                           objParametri)
                objAgenda = Nothing

                For i = 0 To dtAgenda.Rows.Count - 1

                    'Tipo_Operazione = IIf(ForDelete, enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Lettura)
                    Dim Operazione = New Operazione_Agenda With {
                        .Tipo_Operazione = enum_TipoOperazioneDB.Lettura,
                        .Piva = dtAgenda.Rows(i).Item("Piva"),
                        .Sa_Cod = dtAgenda.Rows(i).Item("Sa_Cod"),
                        .Id_Agenda = dtAgenda.Rows(i).Item("Id_Agenda"),
                        .Lav_Cod = dtAgenda.Rows(i).Item("Lav_Cod"),
                        .Data = dtAgenda.Rows(i).Item("Validita_Inizio"),
                        .Des_Lib = dtAgenda.Rows(i).Item("Des_Lib"),
                        .Blocco_Flag = dtAgenda.Rows(i).Item("Blocco_Flag"),
                        .Blocco_Data = dtAgenda.Rows(i).Item("Blocco_Data"),
                        .Blocco_Username = dtAgenda.Rows(i).Item("Blocco_Username"),
                        .Data_Creazione = CDate(dtAgenda.Rows(i).Item("Data_Creazione")),
                        .Data_Modifica = CDate(dtAgenda.Rows(i).Item("Data_Modifica")),
                        .Username_Creazione = dtAgenda.Rows(i).Item("Username_Creazione"),
                        .Username_Modifica = dtAgenda.Rows(i).Item("Username_Modifica")
                    }

                    If Not IsDBNull(dtAgenda.Rows(i).Item("LINEA_COD")) Then
                        Operazione.Linea_Cod = dtAgenda.Rows(i).Item("LINEA_COD")
                    Else
                        Operazione.Linea_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("PREPARAZIONE_COD")) Then
                        Operazione.Preparazione_Cod = dtAgenda.Rows(i).Item("PREPARAZIONE_COD")
                    Else
                        Operazione.Preparazione_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("ID_TRASFORMAZIONE")) Then
                        Operazione.Id_Trasformazione = dtAgenda.Rows(i).Item("ID_TRASFORMAZIONE")
                    Else
                        Operazione.Id_Trasformazione = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Tipo_Accettazione")) Then
                        Operazione.Tipo_Accettazione = dtAgenda.Rows(i).Item("Tipo_Accettazione")
                    Else
                        Operazione.Tipo_Accettazione = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export")) Then
                        Operazione.Stato_Export = dtAgenda.Rows(i).Item("Stato_Export")
                    Else
                        Operazione.Stato_Export = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export_2")) Then
                        Operazione.Stato_Export_2 = dtAgenda.Rows(i).Item("Stato_Export_2")
                    Else
                        Operazione.Stato_Export_2 = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export_2")) Then
                        Operazione.Stato_Export_2 = dtAgenda.Rows(i).Item("Stato_Export_2")
                    Else
                        Operazione.Stato_Export_2 = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Tipo_Visibilita")) Then
                        Operazione.Tipo_Visibilita = dtAgenda.Rows(i).Item("Tipo_Visibilita")
                    Else
                        Operazione.Tipo_Visibilita = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("ChkCoge_Manuale")) Then
                        Operazione.ChkCoge_Manuale = dtAgenda.Rows(i).Item("ChkCoge_Manuale")
                    Else
                        Operazione.ChkCoge_Manuale = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Id_Attivita")) Then
                        Operazione.Id_Attivita = dtAgenda.Rows(i).Item("Id_Attivita")
                    Else
                        Operazione.Id_Attivita = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Modulo")) Then
                        Operazione.Modulo = dtAgenda.Rows(i).Item("Modulo")
                    Else
                        Operazione.Modulo = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Audit_Cod")) Then
                        Operazione.Audit_Cod = dtAgenda.Rows(i).Item("Audit_Cod")
                    Else
                        Operazione.Audit_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Raccoglitore_Cod")) Then
                        Operazione.Raccoglitore_Cod = dtAgenda.Rows(i).Item("Raccoglitore_Cod")
                    Else
                        Operazione.Raccoglitore_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Split")) Then
                        Operazione.Split = dtAgenda.Rows(i).Item("Split")
                    Else
                        Operazione.Split = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Pratica_Cod")) Then
                        Operazione.Pratica_Cod = dtAgenda.Rows(i).Item("Pratica_Cod")
                    Else
                        Operazione.Pratica_Cod = 0
                    End If


                    If Not IsDBNull(dtAgenda.Rows(i).Item("Origine")) Then
                        Operazione.Origine = dtAgenda.Rows(i).Item("Origine")
                    Else
                        Operazione.Origine = ""
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Cod")) Then
                        Operazione.Stato_Cod = dtAgenda.Rows(i).Item("Stato_Cod")
                    Else
                        Operazione.Stato_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("DaRemoto")) Then
                        Operazione.DaRemoto = dtAgenda.Rows(i).Item("DaRemoto")
                    Else
                        Operazione.DaRemoto = 0
                    End If

                    '--------------------------------------------------------
                    '-------- NOTE ------------------------------------------
                    '--------------------------------------------------------
                    Dim objNote = New Agenda_Note_Helper
                    Dim listaNote As IList(Of Nota)

                    listaNote = objNote.Leggi(Operazione.Id_Agenda, objParametri)
                    objNote = Nothing

                    If Not IsNothing(listaNote) Then
                        Operazione.Note = listaNote
                    End If

                    '--------------------------------------------------------
                    '-------- MOVIMENTI -------------------------------------
                    '--------------------------------------------------------
                    Dim objMovimenti = New Agenda_Movimenti_Helper
                    Dim listaMovimenti As List(Of Movimento)

                    listaMovimenti = objMovimenti.Leggi(Operazione.Piva,
                                                        Operazione.Sa_Cod,
                                                        Operazione.Id_Agenda,
                                                        objParametri)
                    objMovimenti = Nothing

                    If Not IsNothing(listaMovimenti) Then
                        Operazione.Movimenti = listaMovimenti
                    End If

                    listaAgenda.Add(Operazione)

                    '--------------------------------------------------------
                    '-------- RIFERIMENTI -------------------------------------
                    '--------------------------------------------------------
                    Dim objRiferimenti = New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                    Dim listaRiferimenti As List(Of Movimento_Dettaglio_Riferimento)

                    listaRiferimenti = objRiferimenti.LeggiRiferimentiAgenda(Operazione.Piva, 0, Operazione.Id_Agenda, 0, "", objParametri)

                    objRiferimenti = Nothing

                    If Not IsNothing(listaRiferimenti) Then
                        Operazione.Agenda_Riferimenti = listaRiferimenti
                    End If

                Next

            Catch ex As Exception

                Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return listaAgenda

        End Function

        Public Function LeggiLista_DaRaccoglitore(ByVal Piva As String, ByVal Raccoglitore_Cod As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of Operazione_Agenda)

            Dim nomeRoutine As String = "Agenda_Operazione_Helper.LeggiLista_DaRaccoglitore()"

            Dim listaAgenda As New List(Of Operazione_Agenda)

            Dim flagConnessione As Boolean = False

            Try

                '---------------------------
                Utility.VerificaApriConnessione(objParametri, flagConnessione)
                '---------------------------


                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
                Dim dtAgenda As DataTable

                dtAgenda = objAgenda.Leggi(CStr(Piva),
                                           0,
                                           0,
                                           0,
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           " Agenda.Id_Agenda ",
                                           objParametri,
                                           Raccoglitore_Cod:=Raccoglitore_Cod)

                objAgenda = Nothing

                For i = 0 To dtAgenda.Rows.Count - 1

                    'Tipo_Operazione = IIf(ForDelete, enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Lettura)
                    Dim Operazione = New Operazione_Agenda With {
                        .Tipo_Operazione = enum_TipoOperazioneDB.Lettura,
                        .Piva = dtAgenda.Rows(i).Item("Piva"),
                        .Sa_Cod = dtAgenda.Rows(i).Item("Sa_Cod"),
                        .Id_Agenda = dtAgenda.Rows(i).Item("Id_Agenda"),
                        .Lav_Cod = dtAgenda.Rows(i).Item("Lav_Cod"),
                        .Data = dtAgenda.Rows(i).Item("Validita_Inizio"),
                        .Des_Lib = dtAgenda.Rows(i).Item("Des_Lib"),
                        .Blocco_Flag = dtAgenda.Rows(i).Item("Blocco_Flag"),
                        .Blocco_Data = dtAgenda.Rows(i).Item("Blocco_Data"),
                        .Blocco_Username = dtAgenda.Rows(i).Item("Blocco_Username"),
                        .Data_Creazione = CDate(dtAgenda.Rows(i).Item("Data_Creazione")),
                        .Data_Modifica = CDate(dtAgenda.Rows(i).Item("Data_Modifica")),
                        .Username_Creazione = dtAgenda.Rows(i).Item("Username_Creazione"),
                        .Username_Modifica = dtAgenda.Rows(i).Item("Username_Modifica")
                    }

                    If Not IsDBNull(dtAgenda.Rows(i).Item("LINEA_COD")) Then
                        Operazione.Linea_Cod = dtAgenda.Rows(i).Item("LINEA_COD")
                    Else
                        Operazione.Linea_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("PREPARAZIONE_COD")) Then
                        Operazione.Preparazione_Cod = dtAgenda.Rows(i).Item("PREPARAZIONE_COD")
                    Else
                        Operazione.Preparazione_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("ID_TRASFORMAZIONE")) Then
                        Operazione.Id_Trasformazione = dtAgenda.Rows(i).Item("ID_TRASFORMAZIONE")
                    Else
                        Operazione.Id_Trasformazione = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Tipo_Accettazione")) Then
                        Operazione.Tipo_Accettazione = dtAgenda.Rows(i).Item("Tipo_Accettazione")
                    Else
                        Operazione.Tipo_Accettazione = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export")) Then
                        Operazione.Stato_Export = dtAgenda.Rows(i).Item("Stato_Export")
                    Else
                        Operazione.Stato_Export = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export_2")) Then
                        Operazione.Stato_Export_2 = dtAgenda.Rows(i).Item("Stato_Export_2")
                    Else
                        Operazione.Stato_Export_2 = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export_2")) Then
                        Operazione.Stato_Export_2 = dtAgenda.Rows(i).Item("Stato_Export_2")
                    Else
                        Operazione.Stato_Export_2 = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Tipo_Visibilita")) Then
                        Operazione.Tipo_Visibilita = dtAgenda.Rows(i).Item("Tipo_Visibilita")
                    Else
                        Operazione.Tipo_Visibilita = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("ChkCoge_Manuale")) Then
                        Operazione.ChkCoge_Manuale = dtAgenda.Rows(i).Item("ChkCoge_Manuale")
                    Else
                        Operazione.ChkCoge_Manuale = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Id_Attivita")) Then
                        Operazione.Id_Attivita = dtAgenda.Rows(i).Item("Id_Attivita")
                    Else
                        Operazione.Id_Attivita = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Modulo")) Then
                        Operazione.Modulo = dtAgenda.Rows(i).Item("Modulo")
                    Else
                        Operazione.Modulo = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Audit_Cod")) Then
                        Operazione.Audit_Cod = dtAgenda.Rows(i).Item("Audit_Cod")
                    Else
                        Operazione.Audit_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Raccoglitore_Cod")) Then
                        Operazione.Raccoglitore_Cod = dtAgenda.Rows(i).Item("Raccoglitore_Cod")
                    Else
                        Operazione.Raccoglitore_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Split")) Then
                        Operazione.Split = dtAgenda.Rows(i).Item("Split")
                    Else
                        Operazione.Split = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Pratica_Cod")) Then
                        Operazione.Pratica_Cod = dtAgenda.Rows(i).Item("Pratica_Cod")
                    Else
                        Operazione.Pratica_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Origine")) Then
                        Operazione.Origine = dtAgenda.Rows(i).Item("Origine")
                    Else
                        Operazione.Origine = ""
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Cod")) Then
                        Operazione.Stato_Cod = dtAgenda.Rows(i).Item("Stato_Cod")
                    Else
                        Operazione.Stato_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("DaRemoto")) Then
                        Operazione.DaRemoto = dtAgenda.Rows(i).Item("DaRemoto")
                    Else
                        Operazione.DaRemoto = 0
                    End If

                    '--------------------------------------------------------
                    '-------- NOTE ------------------------------------------
                    '--------------------------------------------------------
                    Dim objNote = New Agenda_Note_Helper
                    Dim listaNote As IList(Of Nota)

                    listaNote = objNote.Leggi(Operazione.Id_Agenda, objParametri)
                    objNote = Nothing

                    If Not IsNothing(listaNote) Then
                        Operazione.Note = listaNote
                    End If

                    '--------------------------------------------------------
                    '-------- MOVIMENTI -------------------------------------
                    '--------------------------------------------------------
                    Dim objMovimenti = New Agenda_Movimenti_Helper
                    Dim listaMovimenti As List(Of Movimento)

                    listaMovimenti = objMovimenti.Leggi(Operazione.Piva,
                                                        Operazione.Sa_Cod,
                                                        Operazione.Id_Agenda,
                                                        objParametri)
                    objMovimenti = Nothing

                    If Not IsNothing(listaMovimenti) Then
                        Operazione.Movimenti = listaMovimenti
                    End If

                    listaAgenda.Add(Operazione)

                    '--------------------------------------------------------
                    '-------- RIFERIMENTI -------------------------------------
                    '--------------------------------------------------------
                    Dim objRiferimenti = New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                    Dim listaRiferimenti As List(Of Movimento_Dettaglio_Riferimento)

                    listaRiferimenti = objRiferimenti.LeggiRiferimentiAgenda(Operazione.Piva, 0, Operazione.Id_Agenda, 0, "", objParametri)

                    objRiferimenti = Nothing

                    If Not IsNothing(listaRiferimenti) Then
                        Operazione.Agenda_Riferimenti = listaRiferimenti
                    End If

                Next

            Catch ex As Exception

                Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return listaAgenda

        End Function

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal lavCod As Integer,
                              ByVal objParametri As AgronicaCoreParametri,
                              Optional ByVal opzioniLetturaAgenda As Opzioni_Lettura_Agenda = Nothing,
                              Optional ByVal filtroAggiuntivo As String = ""
                              ) As Operazione_Agenda

            If IsNothing(opzioniLetturaAgenda) Then
                opzioniLetturaAgenda = New Opzioni_Lettura_Agenda
            End If

            Dim nomeRoutine As String = "Agenda_Operazione_Helper.Leggi()"

            Dim Operazione As Operazione_Agenda = Nothing

            Dim flagConnessione As Boolean = False

            Try

                '---------------------------
                Utility.VerificaApriConnessione(objParametri, flagConnessione)
                '---------------------------


                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
                Dim dtAgenda As DataTable

                dtAgenda = objAgenda.Leggi(CStr(piva),
                                           CInt(saCod),
                                           CInt(idAgenda),
                                           CInt(lavCod),
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           filtroAggiuntivo,
                                           "",
                                           objParametri)
                objAgenda = Nothing

                For i = 0 To dtAgenda.Rows.Count - 1

                    'Tipo_Operazione = IIf(ForDelete, enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Lettura)
                    Operazione = New Operazione_Agenda With {
                        .Tipo_Operazione = enum_TipoOperazioneDB.Lettura,
                        .Piva = dtAgenda.Rows(i).Item("Piva"),
                        .Sa_Cod = dtAgenda.Rows(i).Item("Sa_Cod"),
                        .Id_Agenda = dtAgenda.Rows(i).Item("Id_Agenda"),
                        .Lav_Cod = dtAgenda.Rows(i).Item("Lav_Cod"),
                        .Tipo_Accettazione = dtAgenda.Rows(i).Item("Tipo_Accettazione"),
                        .Data = dtAgenda.Rows(i).Item("Validita_Inizio"),
                        .Des_Lib = dtAgenda.Rows(i).Item("Des_Lib"),
                        .Blocco_Flag = dtAgenda.Rows(i).Item("Blocco_Flag"),
                        .Blocco_Data = dtAgenda.Rows(i).Item("Blocco_Data"),
                        .Blocco_Username = dtAgenda.Rows(i).Item("Blocco_Username"),
                        .Data_Creazione = CDate(dtAgenda.Rows(i).Item("Data_Creazione")),
                        .Data_Modifica = CDate(dtAgenda.Rows(i).Item("Data_Modifica")),
                        .Username_Creazione = dtAgenda.Rows(i).Item("Username_Creazione"),
                        .Username_Modifica = dtAgenda.Rows(i).Item("Username_Modifica")
                    }

                    If Not IsDBNull(dtAgenda.Rows(i).Item("LINEA_COD")) Then
                        Operazione.Linea_Cod = dtAgenda.Rows(i).Item("LINEA_COD")
                    Else
                        Operazione.Linea_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("PREPARAZIONE_COD")) Then
                        Operazione.Preparazione_Cod = dtAgenda.Rows(i).Item("PREPARAZIONE_COD")
                    Else
                        Operazione.Preparazione_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("ID_TRASFORMAZIONE")) Then
                        Operazione.Id_Trasformazione = dtAgenda.Rows(i).Item("ID_TRASFORMAZIONE")
                    Else
                        Operazione.Id_Trasformazione = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Tipo_Accettazione")) Then
                        Operazione.Tipo_Accettazione = dtAgenda.Rows(i).Item("Tipo_Accettazione")
                    Else
                        Operazione.Tipo_Accettazione = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export")) Then
                        Operazione.Stato_Export = dtAgenda.Rows(i).Item("Stato_Export")
                    Else
                        Operazione.Stato_Export = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export_2")) Then
                        Operazione.Stato_Export_2 = dtAgenda.Rows(i).Item("Stato_Export_2")
                    Else
                        Operazione.Stato_Export_2 = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Export_2")) Then
                        Operazione.Stato_Export_2 = dtAgenda.Rows(i).Item("Stato_Export_2")
                    Else
                        Operazione.Stato_Export_2 = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Tipo_Visibilita")) Then
                        Operazione.Tipo_Visibilita = dtAgenda.Rows(i).Item("Tipo_Visibilita")
                    Else
                        Operazione.Tipo_Visibilita = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("ChkCoge_Manuale")) Then
                        Operazione.ChkCoge_Manuale = dtAgenda.Rows(i).Item("ChkCoge_Manuale")
                    Else
                        Operazione.ChkCoge_Manuale = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Id_Attivita")) Then
                        Operazione.Id_Attivita = dtAgenda.Rows(i).Item("Id_Attivita")
                    Else
                        Operazione.Id_Attivita = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Modulo")) Then
                        Operazione.Modulo = dtAgenda.Rows(i).Item("Modulo")
                    Else
                        Operazione.Modulo = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Audit_Cod")) Then
                        Operazione.Audit_Cod = dtAgenda.Rows(i).Item("Audit_Cod")
                    Else
                        Operazione.Audit_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Raccoglitore_Cod")) Then
                        Operazione.Raccoglitore_Cod = dtAgenda.Rows(i).Item("Raccoglitore_Cod")
                    Else
                        Operazione.Raccoglitore_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Split")) Then
                        Operazione.Split = dtAgenda.Rows(i).Item("Split")
                    Else
                        Operazione.Split = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Pratica_Cod")) Then
                        Operazione.Pratica_Cod = dtAgenda.Rows(i).Item("Pratica_Cod")
                    Else
                        Operazione.Pratica_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Origine")) Then
                        Operazione.Origine = dtAgenda.Rows(i).Item("Origine")
                    Else
                        Operazione.Origine = ""
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("Stato_Cod")) Then
                        Operazione.Stato_Cod = dtAgenda.Rows(i).Item("Stato_Cod")
                    Else
                        Operazione.Stato_Cod = 0
                    End If

                    If Not IsDBNull(dtAgenda.Rows(i).Item("DaRemoto")) Then
                        Operazione.DaRemoto = dtAgenda.Rows(i).Item("DaRemoto")
                    Else
                        Operazione.DaRemoto = 0
                    End If



                    '--------------------------------------------------------
                    '-------- GPS -------------------------------------------
                    '--------------------------------------------------------
                    Dim operazioniConCoordinate = New List(Of Integer) From {LAVCOD_VISITA, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_PIOGGE, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA}

                    If opzioniLetturaAgenda.LeggiGps AndAlso operazioniConCoordinate.Contains(Operazione.Lav_Cod) Then

                        Dim leggiGIS As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
                        Dim DTgeo As DataTable = leggiGIS.LeggiCoordinateOperazioneAgenda(Operazione.Piva, Operazione.Id_Agenda, "", objParametri)
                        If Not IsNothing(DTgeo) AndAlso DTgeo.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(DTgeo.Rows(0).Item("geo")) Then

                            Dim wktHelp As New AgronicaConversioneCartografiaGias.FormatsConverter.WKT
                            Dim DatiCartograficiOriginali_WGS84 As List(Of AgronicaGIS2012.Commons.xyz) = wktHelp.CreaCoordinateDaWkt(DTgeo.Rows(0).Item("geo"))
                            Dim strGeo As String = DatiCartograficiOriginali_WGS84(0).Y.ToString().Replace(",", ".") & ", " & DatiCartograficiOriginali_WGS84(0).X.ToString().Replace(",", ".")

                            Operazione.GisWkt = strGeo
                            Operazione.GisWktGps = DTgeo.Rows(0).Item("flag_GPS")
                            Operazione.GisWktSistemaRiferimento = "-1"
                        End If

                    End If

                    '--------------------------------------------------------
                    '-------- NOTE ------------------------------------------
                    '--------------------------------------------------------

                    If opzioniLetturaAgenda.LeggiNote Then

                        Dim objNote = New Agenda_Note_Helper
                        Dim listaNote As IList(Of Nota)

                        listaNote = objNote.Leggi(idAgenda, objParametri)
                        objNote = Nothing

                        If Not IsNothing(listaNote) Then
                            Operazione.Note = listaNote
                        End If

                    End If

                    '--------------------------------------------------------
                    '-------- MOVIMENTI -------------------------------------
                    '--------------------------------------------------------

                    If opzioniLetturaAgenda.LeggiMovimenti Then

                        Dim objMovimenti = New Agenda_Movimenti_Helper
                        Dim listaMovimenti As List(Of Movimento)

                        listaMovimenti = objMovimenti.Leggi(Operazione.Piva,
                                                            Operazione.Sa_Cod,
                                                            Operazione.Id_Agenda,
                                                            objParametri,
                                                            opzioniLetturaMovimenti:=opzioniLetturaAgenda.OpzioniLetturaMovimenti)

                        objMovimenti = Nothing

                        If Not IsNothing(listaMovimenti) Then
                            Operazione.Movimenti = listaMovimenti
                        End If

                    End If

                    '--------------------------------------------------------
                    '-------- RIFERIMENTI -----------------------------------
                    '--------------------------------------------------------

                    If opzioniLetturaAgenda.LeggiRiferimenti Then

                        Dim objRiferimenti = New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                        Dim listaRiferimenti As List(Of Movimento_Dettaglio_Riferimento)

                        listaRiferimenti = objRiferimenti.LeggiRiferimentiAgenda(Operazione.Piva, 0, Operazione.Id_Agenda, 0, "", objParametri)

                        objRiferimenti = Nothing

                        If Not IsNothing(listaRiferimenti) Then
                            Operazione.Agenda_Riferimenti = listaRiferimenti
                        End If

                    End If

                Next

            Catch ex As Exception

                Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return Operazione

        End Function

        Private Function IsOnlyFirstSomm(ByVal sommRzxa As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda, ByRef GiasContext As Gias_DeveloperServer_Entities) As Boolean
            Dim idPres = sommRzxa.Id_Ricetta
            Dim idRigaPres = sommRzxa.Id_RigaRicetta
            Dim hasLinkedRecords = False
            Dim isFirst = GiasContext.Ricette_Zoo_Agenda.
                Where(Function(rza) rza.IdRicetta = idPres AndAlso rza.IdAgenda = idRigaPres).
                Select(Function(rza) rza.Numero_Somm).FirstOrDefault = 1

            If Not isFirst Then Return False

            Dim Riette_Zoo_gruppoRic = GiasContext.Ricette_Zoo.Where(Function(rz) rz.IdRicetta = idPres).FirstOrDefault
            If Riette_Zoo_gruppoRic IsNot Nothing Then
                Dim gruppoRic = Riette_Zoo_gruppoRic.Gruppo_Ricetta
                Dim sommSuccessive = GiasContext.Ricette_Zoo.Where(Function(rz) rz.Gruppo_Ricetta = gruppoRic AndAlso rz.IdRicetta <> idPres).ToList

                Dim sommIds = sommSuccessive.Select(Function(rz) rz.IdRicetta).ToList()
                hasLinkedRecords = GiasContext.Ricette_ZooxAgenda.Any(Function(rzxa) sommIds.Contains(rzxa.Id_Ricetta))
            End If


            Return Not hasLinkedRecords

        End Function

        Private Sub DeleteSomministrazioniSuccessive(ByVal sommRzxa As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda,
                                                     ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                     ByRef objP_Server As AgronicaCoreParametri)
            Dim idPres = sommRzxa.Id_Ricetta
            Dim gruppoRic = GiasContext.Ricette_Zoo.
                Where(Function(rz) rz.IdRicetta = idPres).
                FirstOrDefault.Gruppo_Ricetta

            Dim gruppoSomm = GiasContext.Ricette_Zoo.
                Where(Function(rz) rz.Gruppo_Ricetta = gruppoRic).ToArray

            Dim ricZoo_W As New AgronicaCoreContabBIZ.Ricette_Zoo_W
            ricZoo_W.EliminaRicetteCollegate(gruppoSomm, GiasContext, objP_Server)
        End Sub

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal cancellaAggancioRicetta As Boolean,
                                 ByVal objParametri As AgronicaCoreParametri,
                                 Optional ByVal logCancellazione As Boolean = True,
                                 Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                 Optional ByVal CancellaOperazioneSingolaNoCorrelate As Boolean = False,
                                 Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.gias,
                                 Optional scope As TransactionScope = Nothing,
                                 Optional GiasContext As Gias_DeveloperServer_Entities = Nothing
                                 ) As Boolean

            Const nomeRoutine = "Agenda_Operazione_Helper.Cancella()"

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)
                Dim objAgendaR As New AgronicaCoreContabDAL.Agenda_R

                Dim dt As DataTable = objAgendaR.Leggi(piva, 0, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                '---------------------------
                'BROGLIACCIO COLLEGATO

                'La cancellazione delle agende originate su "Demetra" comporta l'eliminazione del relativo brogliaccio e l'aggiornamento dello stato "cancellato" su app_dati.
                'La cancellazione delle agende originate su "Pua"/"NewAgri" non comporta l'eliminazione del relativo brogliaccio e viene impostato a 0 l'Id_Agenda salvato nel Riferimento dell'APP_Dati.
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                    If Not IsDBNull(dt.Rows(0).Item("Origine")) Then

                        Dim Ricetta_Cod As Integer = 0
                        Dim Ricetta_Operazione_Cod As Integer = 0
                        Dim APP_Ricetta_Operazione_ID As String = String.Empty

                        Dim objRicettexAgenda As New AgronicaCoreContabDAL.RicettexAgenda_R
                        Dim dtRicettexAgenda As DataTable

                        dtRicettexAgenda = objRicettexAgenda.Leggi_Join_Ricette_Operazioni(0,
                                              0,
                                              idAgenda,
                                              AGRODATAINIZIO,
                                              AGRODATAFINE,
                                              "", "",
                                              objParametri)

                        If dtRicettexAgenda IsNot Nothing AndAlso dtRicettexAgenda.Rows.Count > 0 Then

                            Ricetta_Cod = dtRicettexAgenda.Rows(0).Item("Ricetta_Cod")
                            Ricetta_Operazione_Cod = dtRicettexAgenda.Rows(0).Item("Ricetta_Operazione_Cod")
                            APP_Ricetta_Operazione_ID = dtRicettexAgenda.Rows(0).Item("APP_Ricetta_Operazione_ID")

                            If Ricetta_Cod > 0 AndAlso Ricetta_Operazione_Cod > 0 Then

                                Select Case dt.Rows(0).Item("Origine").ToString().ToUpper()
                                    Case enum_OrigineApp.Demetra
                                        'solo se cancellazione vera di agenda, non update
                                        If logCancellazione Then

                                            Dim r_Read As New AgronicaCoreContabBIZ.Ricette_R
                                            Dim r_Write As New AgronicaCoreContabBIZ.Ricette_W
                                            Dim DatiRicetta As String = r_Read.Ricetta_Leggi(Ricetta_Cod, "", 0, 0, 0, True, objParametri)
                                            If DatiRicetta <> "" Then
                                                r_Write.Ricetta_Scrivi(DatiRicetta, Ricetta_Cod, objParametri)
                                            End If

                                        End If
                                    Case enum_OrigineApp.GiasApp
                                        If logCancellazione Then

                                            Dim utenti_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session?("ASG_objParametri_Utenti")
                                            If objParametri_Utenti IsNot Nothing Then

                                                Dim valoreImpostazione = utenti_impostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra, objParametri_Utenti.SuperUserUsername, objParametri_Utenti)

                                                If valoreImpostazione = "1" Then
                                                    Dim r_Read As New AgronicaCoreContabBIZ.Ricette_R
                                                    Dim r_Write As New AgronicaCoreContabBIZ.Ricette_W
                                                    Dim DatiRicetta As String = r_Read.Ricetta_Leggi(Ricetta_Cod, "", 0, 0, 0, True, objParametri)
                                                    If DatiRicetta <> "" Then
                                                        r_Write.Ricetta_Scrivi(DatiRicetta, Ricetta_Cod, objParametri)
                                                    End If
                                                End If
                                            End If
                                        End If

                                    Case enum_OrigineApp.PUA
                                        'aggiorno APP_Dati pulendo il riferimento all'agenda

                                        If Not String.IsNullOrEmpty(APP_Ricetta_Operazione_ID) Then

                                            Dim Riferimento As String = String.Format("{0}|{1}", 0, Ricetta_Cod)

                                            Dim objAppDati As New AgronicaCoreContabDAL.APP_Dati_W

                                            objAppDati.Aggiorna_Riferimento_DatiAPP(APP_Ricetta_Operazione_ID, Riferimento, objParametri)
                                        End If

                                End Select
                            End If


                        End If


                    End If
                End If

                '---------------------------
                'NOTE 

                Dim objNota As New Agenda_Note_Helper
                objNota.Cancella(idAgenda, 0, objParametri)
                objNota = Nothing

                'Grilli 26/07/2019 Messo perché nell'XML di creazione dell'agenda il dato non c'è e quindi in caso di modifica perderemmo l'informazione
                If Not CancellaOperazioneSingolaNoCorrelate Then
                    '---------------------------
                    'RICETTE 
                    If cancellaAggancioRicetta Then
                        Dim objRicetta As New Agenda_Ricette_Helper
                        objRicetta.Cancella(idAgenda, 0, 0, objParametri)
                        objRicetta = Nothing
                    End If
                End If

                'Grilli 26/07/2019 Messo perché nell'XML di creazione dell'agenda il dato non c'è e quindi in caso di modifica perderemmo l'informazione
                If Not CancellaOperazioneSingolaNoCorrelate Then
                    '---------------------------
                    'COSTI CDG 
                    Dim objCDG_r As New AgronicaCoreContabDAL.CDG_DAL_R
                    Dim lista_id_cdg As List(Of Integer) = objCDG_r.Leggi_IdCDG_Da_IdAgenda(piva, idAgenda, objParametri)

                    Dim objCDG_w As New AgronicaCoreContabDAL.CDG_DAL_W
                    For Each id_cdg As Integer In lista_id_cdg
                        objCDG_w.CancellaCDG_Dettagli(piva, id_cdg, objParametri)
                        objCDG_w.CancellaCDG_Testata(piva, id_cdg, objParametri)
                    Next
                End If

                'Grilli 26/07/2019 Messo perché nell'XML di creazione dell'agenda il dato non c'è e quindi in caso di modifica perderemmo l'informazione
                If Not CancellaOperazioneSingolaNoCorrelate AndAlso dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    '---------------------------
                    'ZOO
                    Dim ZooBIZ As New AgronicaCoreAnagrafeBIZ.Zoo

                    Dim data As Date = dt.Rows(0).Item("Validita_inizio")
                    Dim desLib As String = dt.Rows(0).Item("Des_Lib")
                    Dim lavCod As Long = dt.Rows(0).Item("Lav_Cod")

                    If lavCod >= 3000 AndAlso lavCod <= 3999 Then 'lista lavcod dello zoo
                        If scope Is Nothing Then
                            Dim scopeOption As New TransactionScopeOption
                            Dim transactionOptions As New TransactionOptions()
                            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted
                            'transactionOptions.Timeout = TransactionManager.MaximumTimeout

                            scope = New TransactionScope(scopeOption, transactionOptions)
                        End If

                        If GiasContext Is Nothing Then
                            Dim gefutils As New Gias_EF_Utility
                            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
                            GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                        End If

                        Select Case CInt(lavCod)
                            Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI, LAVCOD_NASCITA_ANIMALI

                                Try
                                    'MOVIMENTO ZOO
                                    Dim MovTestataZoo = (From movz In GiasContext.Movimenti_Zoo
                                                         Where movz.Piva = piva AndAlso movz.Id_Agenda = idAgenda
                                                         Select movz).ToArray
                                    If Not IsNothing(MovTestataZoo) Then
                                        Dim movimentiZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
                                        movimentiZoo_W.Elimina(MovTestataZoo, GiasContext, objParametri)
                                    End If

                                    'CAPI COINVOLTI
                                    Dim Movimenti_Animali = (From Movimenti In GiasContext.Movimenti
                                                             Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On Movimenti.PIVA Equals Movimenti_Dettagli.PIVA And
                                                                                                                          Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                                                                                                                          Movimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov
                                                             Where (Movimenti.Cau_Mov = CAU_CARICO_CONSISTENZE OrElse Movimenti.Cau_Mov = CAU_ANIMALE) AndAlso
                                                                 Movimenti.PIVA = piva AndAlso
                                                                 Movimenti.Id_Agenda = idAgenda AndAlso
                                                                 Movimenti_Dettagli.Cod_Progetto <> 0
                                                             Select Movimenti_Dettagli
                                                           ).Distinct

                                    For Each mov_an In Movimenti_Animali
                                        Elimina_Animale(mov_an.PIVA, mov_an.Cod_Progetto, objParametri)
                                    Next


                                    GiasContext.SaveChanges()
                                    'EF6: AcceptAllChanges (Accepts the changes on all associated entries in the ObjectStateManager so their resultant state is either unchanged or detached.
                                    '                       This method iterates all the ObjectStateEntry objects within the ObjectStateManager that are Added or Modified, and then sets the state of the entry to Unchanged. The Deleted items become detached.)
                                    'Qui avrebbe senso fare questa operazione solo se è stato disabilitato il comportamento di default del SaveChanges(che internamente chiama AcceptAllChanges)
                                    'ma visto che è stato chiamato il SaveChanges default, non ha senso farla perché SaveChanges() == SaveChanges(SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
                                    'GiasContext.AcceptAllChanges()
                                    scope.Complete()
                                    scope.Dispose()

                                Catch ex As Exception

                                    scope.Dispose()
                                    Throw New Exception(ex.Message)

                                Finally

                                    GiasContext.Dispose()

                                End Try

                            Case LAVCOD_MORTE_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI

                                Try
                                    'MOVIMENTO ZOO
                                    Dim MovTestataZoo = (From movz In GiasContext.Movimenti_Zoo
                                                         Where movz.Piva = piva AndAlso movz.Id_Agenda = idAgenda
                                                         Select movz).ToArray
                                    If Not IsNothing(MovTestataZoo) Then
                                        Dim movimentiZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
                                        movimentiZoo_W.Elimina(MovTestataZoo, GiasContext, objParametri)
                                    End If

                                    Dim Movimenti_Animali = (From Movimenti In GiasContext.Movimenti
                                                             Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On Movimenti.PIVA Equals Movimenti_Dettagli.PIVA And
                                                                                                                              Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                                                                                                                              Movimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov
                                                             Where (Movimenti.Cau_Mov = CAU_SCARICO_CONSISTENZE OrElse Movimenti.Cau_Mov = CAU_ANIMALE) AndAlso
                                                                     Movimenti.PIVA = piva AndAlso
                                                                     Movimenti.Id_Agenda = idAgenda AndAlso
                                                                     Movimenti_Dettagli.Cod_Progetto <> 0
                                                             Select Movimenti_Dettagli
                                                               ).Distinct

                                    If Movimenti_Animali.Count > 0 Then

                                        Dim listCodProgetto = (From m In Movimenti_Animali Select m.Cod_Progetto).Distinct.ToList

                                        Dim Animali = (From Zoo_Animali In GiasContext.Zoo_Animali
                                                       Where Zoo_Animali.PIVA = piva AndAlso
                                                                        listCodProgetto.Contains(Zoo_Animali.Cod_Progetto)).ToList()

                                        Dim Animale_Distinte = (From Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte
                                                                Where Zoo_Animali_Distinte.PIVA = piva AndAlso
                                                                        listCodProgetto.Contains(Zoo_Animali_Distinte.Cod_Animale)).ToList()



                                        For Each mov_an In Movimenti_Animali
                                            ZooBIZ.Riapri_Validita_Animale_Fine2(Animali, Animale_Distinte, mov_an.PIVA, mov_an.Cod_Progetto, GiasContext, objParametri)
                                            ZooBIZ.Aggiorna_Causale_Morte_Massiva(Animali, mov_an.PIVA, mov_an.Cod_Progetto, 0, GiasContext, objParametri)
                                            ZooBIZ.Aggiorna_Patologia_Capo_Massiva(Animali, mov_an.PIVA, mov_an.Cod_Progetto, 0, GiasContext, objParametri)
                                            ZooBIZ.Aggiorna_Note_Capo_Massiva(Animali, mov_an.PIVA, mov_an.Cod_Progetto, "", GiasContext, objParametri)
                                        Next

                                    End If

                                    GiasContext.SaveChanges()
                                    'EF6: AcceptAllChanges (Accepts the changes on all associated entries in the ObjectStateManager so their resultant state is either unchanged or detached.
                                    '                       This method iterates all the ObjectStateEntry objects within the ObjectStateManager that are Added or Modified, and then sets the state of the entry to Unchanged. The Deleted items become detached.)
                                    'Qui avrebbe senso fare questa operazione solo se è stato disabilitato il comportamento di default del SaveChanges(che internamente chiama AcceptAllChanges)
                                    'ma visto che è stato chiamato il SaveChanges default, non ha senso farla perché SaveChanges() == SaveChanges(SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
                                    'GiasContext.AcceptAllChanges()
                                    scope.Complete()
                                    scope.Dispose()

                                Catch ex As Exception

                                    scope.Dispose()
                                    Throw New Exception(ex.Message)

                                Finally

                                    GiasContext.Dispose()

                                End Try

                            Case LAVCOD_SPOSTAMENTI_ZOO

                                Try

                                    Dim listaCodAnimali = (From Movimenti In GiasContext.Movimenti
                                                           Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On Movimenti.PIVA Equals Movimenti_Dettagli.PIVA And
                                                                                                                              Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                                                                                                                              Movimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov
                                                           Join Mov_Destinazioni In GiasContext.Mov_Destinazioni On Mov_Destinazioni.Piva Equals Movimenti_Dettagli.PIVA And
                                                                                                                              Mov_Destinazioni.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                                                                                                                              Mov_Destinazioni.Id_Mov Equals Movimenti_Dettagli.Id_Mov
                                                           Where (Movimenti.Cau_Mov = CAU_SCARICO_CONSISTENZE OrElse Movimenti.Cau_Mov = CAU_ANIMALE) AndAlso
                                                                     Movimenti.PIVA = piva AndAlso
                                                                     Movimenti.Id_Agenda = idAgenda AndAlso
                                                                     Movimenti_Dettagli.Cod_Progetto <> 0
                                                           Select Movimenti_Dettagli.Cod_Progetto, Mov_Destinazioni.Id_Destinazione).Distinct

                                    Dim listaRaggruppamenti As List(Of Integer) = (From r In listaCodAnimali Select r.Id_Destinazione).Distinct.ToList

                                    Dim raggruppamenti = (From ragg In GiasContext.Stalla_Raggruppamenti Where listaRaggruppamenti.Contains(ragg.Raggruppamento_Cod)).ToList

                                    For Each codAnimale In listaCodAnimali
                                        Dim raggruppamento = (From r In raggruppamenti Where r.Raggruppamento_Cod = codAnimale.Id_Destinazione).FirstOrDefault
                                        If raggruppamento IsNot Nothing AndAlso raggruppamento.Flag_BDN = 1 Then
                                            Dim capo = (From zoo In GiasContext.Zoo_Animali Where zoo.Cod_Progetto = codAnimale.Cod_Progetto).FirstOrDefault
                                            capo.Validato = 0
                                        End If
                                    Next

                                    GiasContext.SaveChanges()
                                    'EF6: AcceptAllChanges (Accepts the changes on all associated entries in the ObjectStateManager so their resultant state is either unchanged or detached.
                                    '                       This method iterates all the ObjectStateEntry objects within the ObjectStateManager that are Added or Modified, and then sets the state of the entry to Unchanged. The Deleted items become detached.)
                                    'Qui avrebbe senso fare questa operazione solo se è stato disabilitato il comportamento di default del SaveChanges(che internamente chiama AcceptAllChanges)
                                    'ma visto che è stato chiamato il SaveChanges default, non ha senso farla perché SaveChanges() == SaveChanges(SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
                                    'GiasContext.AcceptAllChanges()
                                    scope.Complete()
                                    scope.Dispose()

                                Catch ex As Exception
                                    scope.Dispose()
                                    Throw New Exception(ex.Message)
                                Finally
                                    GiasContext.Dispose()
                                End Try

                            Case LAVCOD_CUREMEDICAMENTI_ANIMALI
                                Try
                                    ' Se si tratta di una prima somministrazione e le successive non sono ancora confermate, cancella tutto;
                                    ' Altrimenti elimina solo il link dell'operazione di Agenda
                                    Dim rzxa = GiasContext.Ricette_ZooxAgenda.Where(Function(x) x.Id_Agenda = idAgenda).FirstOrDefault
                                    If rzxa IsNot Nothing Then
                                        If IsOnlyFirstSomm(rzxa, GiasContext) Then
                                            DeleteSomministrazioniSuccessive(rzxa, GiasContext, objParametri)
                                        Else
                                            GiasContext.Ricette_ZooxAgenda.Remove(rzxa)
                                        End If
                                    End If

                                    GiasContext.SaveChanges()
                                    scope.Complete()
                                    scope.Dispose()

                                Catch ex As Exception
                                    scope.Dispose()
                                    Throw New Exception(ex.Message)
                                Finally
                                    GiasContext.Dispose()
                                End Try

                        End Select
                    End If

                End If

                '---------------------------
                'MOVIMENTI 

                Dim objMovimento As New Agenda_Movimenti_Helper
                objMovimento.Cancella(piva, 0, idAgenda, 0, objParametri)
                objMovimento = Nothing

                'Grilli 26/07/2019 Messo perché nell'XML di creazione dell'agenda il dato non c'è e quindi in caso di modifica perderemmo l'informazione
                If Not CancellaOperazioneSingolaNoCorrelate Then
                    '---------------------------
                    'GIS
                    '---------------------------
                    Dim ge As New AgronicaCoreGisDAL.GIS_Entita_R()
                    Dim DTGisEntita As DataTable = ge.LeggiDB(objParametri.PivaSuperUser, 0, 0, piva, saCod, 0, 0, 0, "", "", "", 0, 0, "", idAgenda, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                    If Not IsNothing(DTGisEntita) AndAlso DTGisEntita.Rows.Count > 0 AndAlso IsNumeric(DTGisEntita.Rows(0).Item("Entita_Cod")) AndAlso CInt(DTGisEntita.Rows(0).Item("Entita_Cod")) <> 0 Then

                        Dim geg As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R()
                        Dim DTGisElemGrafici As DataTable = geg.Leggi(objParametri.PivaSuperUser, 0, DTGisEntita.Rows(0).Item("Entita_Cod"), 0, enumFromatoCartograficoConvertito.WKT, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)


                        If Not IsNothing(DTGisElemGrafici) AndAlso DTGisElemGrafici.Rows.Count > 0 AndAlso IsNumeric(DTGisElemGrafici.Rows(0).Item("ElementoGrafico_Cod")) AndAlso CInt(DTGisElemGrafici.Rows(0).Item("ElementoGrafico_Cod")) <> 0 Then
                            Dim scriviGIS As New GisHelper
                            Dim EsitoGis As RispostaStandard = scriviGIS.ScriviDatoCartografico(objParametri,
                                                                                                0, 0,
                                                                                                piva, saCod,
                                                                                                0, 0,
                                                                                                idAgenda, 0,
                                                                                                "", "-1", 0,
                                                                                                DTGisElemGrafici.Rows(0).Item("LayerElementiGrafici_Cod"),
                                                                                                DTGisEntita.Rows(0).Item("TipoEntita_Cod"),
                                                                                                DTGisEntita.Rows(0).Item("Entita_Cod"),
                                                                                                DTGisElemGrafici.Rows(0).Item("ElementoGrafico_Cod"),
                                                                                                enum_TipoOperazioneDB.Cancellazione
                                                                                                )

                            If Not EsitoGis.RispostaOK Then
                                Throw New Exception("[ " & nomeRoutine & " ] : " & EsitoGis.Errore)
                            End If

                            scriviGIS = Nothing
                        End If
                    End If

                End If

                '---------------------------
                'PRATICA (WORKFLOW)

                If Not CancellaOperazioneSingolaNoCorrelate AndAlso dt.Rows.Count > 0 Then
                    If Not IsDBNull(dt.Rows(0).Item("Pratica_Cod")) Then
                        Dim praticaCod As Integer = CInt(dt.Rows(0).Item("Pratica_Cod"))
                        If praticaCod <> 0 Then
                            Dim objPraticaW As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                            Dim messaggioErrorePratica As String = ""
                            Dim eliminataPratica = objPraticaW.Elimina_Pratica(praticaCod, objParametri, messaggioErrorePratica)
                            If Not eliminataPratica Then
                                Throw New Exception("[ " & nomeRoutine & " ] : " & messaggioErrorePratica)
                            End If
                        End If

                    End If

                End If

                '---------------------------
                'DOCUMENTI ALLEGATI
                '---------------------------
                'Nota: uso il campo logCancellazione per differenziare la modifica dalla cancellazione.
                'In caso di modifica i documenti allegati non vanno eliminati

                If logCancellazione Then

                    Dim res As Boolean = CancellaDocumentiAllegati(idAgenda, objParametri)

                    If Not res Then
                        Throw New Exception("[ " & nomeRoutine & " ] : " & Gias.ErroreCancellazioneDocumentiAllegati)
                    End If

                End If

                ''---------------------------
                ''ELIMINO RIFERIMENTO AL REINNESCO NELL'INSTALLAZIONE TRAPPOLE      
                ''---------------------------
                If Not CancellaOperazioneSingolaNoCorrelate AndAlso dt.Rows.Count > 0 Then
                    Dim Lav_Cod As Integer = dt.Rows(0).Item("Lav_Cod")

                    If Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then

                        Dim xFiltroAggiuntivo As String = "Lav_Cod = " & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA

                        Dim objMovimentiDettagliRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                        objMovimentiDettagliRif.Cancella(piva, saCod, 0, 0, 0, xFiltroAggiuntivo, objParametri, Id_Agenda_Rif:=idAgenda)
                    End If
                End If

                ''---------------------------
                ''AGENDA LOG 
                ''---------------------------
                If logCancellazione AndAlso dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then


                    Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W

                    Dim Raccoglitore_Cod As Integer = 0

                    If Not IsDBNull(dt.Rows(0).Item("Raccoglitore_Cod")) AndAlso IsNumeric(dt.Rows(0).Item("Raccoglitore_Cod")) Then
                        Raccoglitore_Cod = dt.Rows(0).Item("Raccoglitore_Cod")
                    End If

                    objAgronicaLogAgendaW.Scrivi(dt.Rows(0).Item("Validita_inizio"),
                                                 enum_TipoOperazioneDB.Cancellazione,
                                                 dt.Rows(0).Item("Des_Lib"),
                                                 idAgenda,
                                                 piva,
                                                 dt.Rows(0).Item("Sa_Cod"),
                                                 dt.Rows(0).Item("Lav_Cod"),
                                                 CInt(idServizio),
                                                 objParametri,
                                                 Origine:=CInt(origine),
                                                 Raccoglitore_Cod:=Raccoglitore_Cod)

                    objAgronicaLogAgendaW = Nothing
                End If

                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                objAgenda.Cancella(piva, 0, idAgenda, "", objParametri)
                objAgenda = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Operazione_Helper.cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Private Function CancellaDocumentiAllegati(ByVal Id_Agenda As Integer,
                                                   ByRef objParametri_Server As AgronicaCoreParametri
                                                   ) As Boolean


            Dim msgErr As String = ""
            Dim id_alert_entita As Integer = 0
            Dim res As Boolean = True

            'Lettura Id_Alert_Entita da If_Agenda
            Dim objR As New AgronicaCoreScadenziario.Alert_Entita_R


            Dim xFiltro_Aggiuntivo As String = ""
            xFiltro_Aggiuntivo = "Id_Agenda = " & Id_Agenda

            Dim dt_Entita As DataTable = objR.Leggi(0, objParametri_Server, xFiltro_Aggiuntivo)
            Dim bDeleteAggancioRicetta As Boolean = False

            Try

                'Controllo presenza dati
                If dt_Entita.Rows.Count > 0 AndAlso Id_Agenda <> 0 Then

                    For Each dr_entita As DataRow In dt_Entita.Rows

                        If IsNumeric(dr_entita.Item("Ricetta_Operazione_cod")) Then
                            id_alert_entita = dr_entita.Item("Id_Alert_Entita")
                        Else
                            id_alert_entita = 0
                        End If

                        bDeleteAggancioRicetta = False 'Inizializzazione
                        If IsNumeric(dr_entita.Item("Ricetta_Operazione_cod")) Then
                            If CInt((dr_entita.Item("Ricetta_Operazione_cod"))) <> 0 Then
                                bDeleteAggancioRicetta = True
                            End If
                        End If

                        Select Case bDeleteAggancioRicetta

                            Case True

                                'Update Tabella Alert_Entita --> Id_Agenda = 0
                                Dim objEntita_W As New AgronicaCoreScadenziario.Alert_Entita_W
                                res = objEntita_W.Reset_Agenda(CInt((dr_entita.Item("Ricetta_Operazione_cod"))), objParametri_Server)

                            Case False

                                'Controllo Coerenza
                                If id_alert_entita <> 0 Then

                                    'Cancellazione Documento

                                    Dim LeggiConfSiti As New Configurazione_Siti_R
                                    Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
                                    Dim path As String = dt_Conf.Rows(0).Item("Valore")

                                    'controllo se ho un allegato associato
                                    If Not IsDBNull(dr_entita.Item("Allegati_Documenti_cod")) AndAlso dr_entita.Item("Allegati_Documenti_cod") <> 0 Then
                                        'leggo l'allegato 
                                        Dim objAllegato_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
                                        Dim DT_Allegato As DataTable = objAllegato_R.Leggi(dt_Entita.Rows(0).Item("Allegati_Documenti_cod"),
                                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                "", "", objParametri_Server)

                                        If DT_Allegato.Rows.Count > 0 Then

                                            'controllo se esiste il file 
                                            If Not IsDBNull(DT_Allegato.Rows(0).Item("Sottocartella")) AndAlso Not IsDBNull(DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")) Then

                                                Dim sottocartella As String = DT_Allegato.Rows(0).Item("Sottocartella")
                                                Dim nomefile As String = DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")

                                                If File.Exists(path & sottocartella & "/" & nomefile) Then
                                                    File.Delete(path & sottocartella & "/" & nomefile)
                                                Else
                                                    'Provo senza path
                                                    If File.Exists(sottocartella & "/" & nomefile) Then
                                                        File.Delete(sottocartella & "/" & nomefile)
                                                    End If
                                                End If

                                            End If

                                            'cancello il record Allegati
                                            Dim objAllegati_W As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                                            objAllegati_W.Cancella(dt_Entita.Rows(0).Item("Allegati_Documenti_cod"), "", objParametri_Server)

                                        End If

                                    End If



                                    'lettura il record Elenco
                                    Dim objElenco_R As New AgronicaCoreScadenziario.Alert_Elenco_R
                                    Dim DT_Elenco As DataTable = objElenco_R.Leggi(0, 0, objParametri_Server, "Id_Alert_Entita = " & id_alert_entita)

                                    If DT_Elenco.Rows.Count > 0 Then

                                        'cancello il record Elenco
                                        Dim objELenco_W As New AgronicaCoreScadenziario.Alert_Elenco_W
                                        objELenco_W.Cancella(DT_Elenco.Rows(0).Item("Id_Elenco"), objParametri_Server)

                                    End If

                                    'cancello i record indicixentita
                                    Dim objEntitaxIndici_W As New AgronicaCoreScadenziario.Alert_Indice_W
                                    objEntitaxIndici_W.CancellaEntitaxIndice("", id_alert_entita, objParametri_Server)


                                    'cancello il record Entita
                                    Dim objEntita_W As New AgronicaCoreScadenziario.Alert_Entita_W
                                    objEntita_W.Cancella(id_alert_entita, objParametri_Server)


                                    If Not res Then
                                        Return False
                                    End If

                                End If

                        End Select


                    Next

                End If


            Catch ex As Exception

                Return False

            Finally


            End Try

            Return res

        End Function


        Public Function Elimina_Animale(ByVal Piva As String,
                                    ByVal Cod_Progetto As Integer,
                                    ByRef objParametriServer As AgronicaCoreParametri)

            Dim r As New RispostaStandard

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            'Dim scope As New TransactionScope()
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Using scope As New TransactionScope()

                Try

                    Dim Zoo_Animali_Distinte = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = Piva AndAlso a.Cod_Animale = Cod_Progetto).ToArray
                    Dim Zoo_AnimalixStati_Accrescimento = (From a In GiasContext.Zoo_AnimalixStati_Accrescimento Where a.PIVA = Piva AndAlso a.Cod_Progetto = Cod_Progetto).ToArray
                    Dim zooAnimale = (From a In GiasContext.Zoo_Animali Where a.PIVA = Piva AndAlso a.Cod_Progetto = Cod_Progetto).FirstOrDefault()

                    If zooAnimale IsNot Nothing AndAlso zooAnimale.Cod_Progetto <> 0 Then

                        Dim obj_Zoo_Animali_W As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                        Dim obj_Zoo_Animali_Distinte_W As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte
                        Dim obj_Zoo_AnimalixStati_Accrescimento_W As New AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento

                        For Each distinta In Zoo_Animali_Distinte
                            obj_Zoo_Animali_Distinte_W.Elimina(distinta, GiasContext, objParametriServer)
                        Next

                        For Each accrescimento In Zoo_AnimalixStati_Accrescimento
                            obj_Zoo_AnimalixStati_Accrescimento_W.Elimina(accrescimento, GiasContext, objParametriServer)
                        Next

                        obj_Zoo_Animali_W.Elimina(zooAnimale, GiasContext, objParametriServer)

                        GiasContext.SaveChanges()

                    End If

                    r.RispostaStringa = ""
                    r.RispostaOK = True

                    scope.Complete()

                Catch ex As Exception

                    r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                    r.RispostaOK = False
                    scope.Dispose()
                    Throw New Exception(r.Errore)

                Finally

                    'scope.Dispose()
                    GiasContext.Dispose()

                End Try

            End Using

            Return r

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         Optional ByVal lavCod As Integer? = Nothing,
                                         Optional ByVal desLib As String = Nothing,
                                         Optional ByVal bloccoFlag As Integer? = Nothing,
                                         Optional ByVal bloccoData As Date? = Nothing,
                                         Optional ByVal bloccoUsername As String = Nothing,
                                         Optional ByVal validitaInizio As Date? = Nothing,
                                         Optional ByVal validitaFine As Date? = Nothing,
                                         Optional ByVal lineaCod As Integer? = Nothing,
                                         Optional ByVal preparazioneCod As Integer? = Nothing,
                                         Optional ByVal idTrasformazione As Integer? = Nothing,
                                         Optional ByVal tipoAccettazione As Integer? = Nothing,
                                         Optional ByVal statoExport As Integer? = Nothing,
                                         Optional ByVal statoExport2 As Integer? = Nothing,
                                         Optional ByVal tipoVisibilita As Integer? = Nothing,
                                         Optional ByVal chkCogeManuale As Integer? = Nothing,
                                         Optional ByVal idAttivita As Integer? = Nothing,
                                         Optional ByVal modulo As Integer? = Nothing,
                                         Optional ByVal auditCod As Integer? = Nothing,
                                         Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                         Optional ByVal usernameModifica As String = "",
                                         Optional ByVal raccoglitoreCod As Integer? = Nothing,
                                         Optional ByVal split As Integer? = Nothing,
                                         Optional ByVal praticaCod As Integer? = Nothing,
                                         Optional ByVal origine As String = Nothing,
                                         Optional ByVal statoCod As Integer? = Nothing,
                                         Optional ByVal daRemoto As Integer? = Nothing) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W

                xRisp = objAgendaW.ModificaPuntuale(piva, saCod, idAgenda, objParametri,
                                                    Lav_Cod:=lavCod,
                                                    Des_Lib:=desLib,
                                                    Blocco_Flag:=bloccoFlag,
                                                    Blocco_Data:=bloccoData,
                                                    Blocco_Username:=bloccoUsername,
                                                    Validita_Inizio:=validitaInizio,
                                                    Validita_Fine:=validitaFine,
                                                    Linea_Cod:=lineaCod,
                                                    Preparazione_Cod:=preparazioneCod,
                                                    Id_Trasformazione:=idTrasformazione,
                                                    Tipo_Accettazione:=tipoAccettazione,
                                                    Stato_Export:=statoExport,
                                                    Stato_Export_2:=statoExport2,
                                                    Tipo_Visibilita:=tipoVisibilita,
                                                    ChkCoge_Manuale:=chkCogeManuale,
                                                    Id_Attivita:=idAttivita,
                                                    Modulo:=modulo,
                                                    Audit_Cod:=auditCod,
                                                    Data_Modifica:=dataModifica,
                                                    Username_Modifica:=usernameModifica,
                                                    Raccoglitore_Cod:=raccoglitoreCod,
                                                    Split:=split,
                                                    Pratica_Cod:=praticaCod,
                                                    Origine:=origine,
                                                    Stato_Cod:=statoCod,
                                                    DaRemoto:=daRemoto
                                                    )

                objAgendaW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Operazione_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         ByVal objAgenda As Operazione_Agenda
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W

                xRisp = objAgendaW.ModificaPuntuale(piva, saCod, idAgenda, objParametri,
                                                    Lav_Cod:=objAgenda.Lav_Cod,
                                                    Des_Lib:=objAgenda.Des_Lib,
                                                    Blocco_Flag:=objAgenda.Blocco_Flag,
                                                    Blocco_Data:=objAgenda.Blocco_Data,
                                                    Blocco_Username:=objAgenda.Blocco_Username,
                                                    Validita_Inizio:=objAgenda.Data,
                                                    Linea_Cod:=objAgenda.Linea_Cod,
                                                    Preparazione_Cod:=objAgenda.Preparazione_Cod,
                                                    Id_Trasformazione:=objAgenda.Id_Trasformazione,
                                                    Tipo_Accettazione:=objAgenda.Tipo_Accettazione,
                                                    Stato_Export:=objAgenda.Stato_Export,
                                                    Stato_Export_2:=objAgenda.Stato_Export_2,
                                                    Tipo_Visibilita:=objAgenda.Tipo_Visibilita,
                                                    ChkCoge_Manuale:=objAgenda.ChkCoge_Manuale,
                                                    Id_Attivita:=objAgenda.Id_Attivita,
                                                    Modulo:=objAgenda.Modulo,
                                                    Audit_Cod:=objAgenda.Audit_Cod,
                                                    Data_Modifica:=objAgenda.Data_Modifica,
                                                    Username_Modifica:=objAgenda.Username_Modifica,
                                                    Raccoglitore_Cod:=objAgenda.Raccoglitore_Cod,
                                                    Split:=objAgenda.Split,
                                                    Pratica_Cod:=objAgenda.Pratica_Cod,
                                                    Origine:=objAgenda.Origine,
                                                    Stato_Cod:=objAgenda.Stato_Cod,
                                                    DaRemoto:=objAgenda.DaRemoto
                                                    )

                objAgendaW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Operazione_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Private Function XML_Agenda_Agenda(ByRef logErrori As String,
                                           ByRef Agenda As Operazione_Agenda
                                           ) As String

            Dim dataXml As XmlElement
            Dim xmlDoc As New XmlDocument

            Try '-----------------------------------------------------------------------------

                dataXml = xmlDoc.CreateElement("Agenda")

                dataXml.SetAttribute("TipoOperazioneDB", CStr(Agenda.Tipo_Operazione))

                dataXml.SetAttribute(LCase("piva"), Agenda.Piva)
                dataXml.SetAttribute(LCase("sa_cod"), CStr(Agenda.Sa_Cod))
                dataXml.SetAttribute(LCase("id_agenda"), CStr(Agenda.Id_Agenda))
                dataXml.SetAttribute(LCase("des_lib"), Agenda.Des_Lib)
                dataXml.SetAttribute(LCase("lav_cod"), CStr(Agenda.Lav_Cod))
                dataXml.SetAttribute(LCase("id_attivita"), CStr(Agenda.Id_Attivita))
                dataXml.SetAttribute(LCase("tipo_accettazione"), CStr(Agenda.Tipo_Accettazione))
                dataXml.SetAttribute(LCase("linea_cod"), CStr(Agenda.Linea_Cod))
                dataXml.SetAttribute(LCase("preparazione_cod"), CStr(Agenda.Preparazione_Cod))
                dataXml.SetAttribute(LCase("id_trasformazione"), CStr(Agenda.Id_Trasformazione))
                dataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(Agenda.Data), "dd/MM/yyyy"))
                dataXml.SetAttribute(LCase("validita_fine"), Format(CDate(AGRODATAFINE), "dd/MM/yyyy"))
                dataXml.SetAttribute(LCase("basecode"), CStr(Agenda.BaseCode))
                dataXml.SetAttribute(LCase("topcode"), CStr(Agenda.TopCode))
                dataXml.SetAttribute(LCase("Blocco_Flag"), CStr(Agenda.Blocco_Flag))
                dataXml.SetAttribute(LCase("Blocco_Data"), Format(CDate(Agenda.Blocco_Data), "dd/MM/yyyy"))
                dataXml.SetAttribute(LCase("Blocco_Username"), CStr(Agenda.Blocco_Username))
                dataXml.SetAttribute(LCase("Audit_cod"), CStr(Agenda.Audit_Cod))
                dataXml.SetAttribute(LCase("Stato_Export"), CStr(Agenda.Stato_Export))
                dataXml.SetAttribute(LCase("Stato_Export_2"), CStr(Agenda.Stato_Export_2))
                dataXml.SetAttribute(LCase("Tipo_Visibilita"), CStr(Agenda.Tipo_Visibilita))
                dataXml.SetAttribute(LCase("ChkCoge_Manuale"), CStr(Agenda.ChkCoge_Manuale))
                dataXml.SetAttribute(LCase("Id_Attivita"), CStr(Agenda.Id_Attivita))
                dataXml.SetAttribute(LCase("Modulo"), CStr(Agenda.Modulo))
                dataXml.SetAttribute(LCase("Raccoglitore_Cod"), CStr(Agenda.Raccoglitore_Cod))
                dataXml.SetAttribute(LCase("Split"), CStr(Agenda.Split))
                dataXml.SetAttribute(LCase("Pratica_Cod"), CStr(Agenda.Pratica_Cod))
                dataXml.SetAttribute(LCase("Origine"), CStr(Agenda.Origine))
                dataXml.SetAttribute(LCase("Invia_App"), CStr(Agenda.Invia_App))
                dataXml.SetAttribute(LCase("Stato_Cod"), CStr(Agenda.Stato_Cod))
                dataXml.SetAttribute(LCase("DaRemoto"), CStr(Agenda.DaRemoto))


            Catch ex As Exception '---------------------------------------------------------------

                logErrori += " (XML_Agenda_Agenda) : " & ex.Message
                Throw New Exception("[ Agenda_Operazione_Helper.XML_Agenda_Agenda() ] : " & ex.Message)

            End Try '-----------------------------------------------------------------------------
            xmlDoc.AppendChild(dataXml)
            Return xmlDoc.InnerXml

        End Function


        Private Function XML_2_Agenda_Movimento(ByRef logErrori As String,
                                                ByVal xmlDoc As XmlDocument,
                                                ByVal Agenda As Operazione_Agenda,
                                                ByVal Movimento As Movimento
                                                ) As XmlElement

            Dim nodoXml As XmlElement

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            nodoXml = xmlDoc.CreateElement("Movimento")

            'Imposto gli attributi
            nodoXml.SetAttribute("TipoOperazioneDB", CStr(Agenda.Tipo_Operazione))

            nodoXml.SetAttribute(LCase("piva"), Agenda.Piva)
            nodoXml.SetAttribute(LCase("sa_cod"), CStr(Agenda.Sa_Cod))
            nodoXml.SetAttribute(LCase("id_agenda"), CStr(Agenda.Id_Agenda))
            nodoXml.SetAttribute(LCase("id_mov"), CStr(Movimento.Id_Mov))
            nodoXml.SetAttribute(LCase("Cod_RisUm"), CStr(Movimento.Cod_Risum))
            nodoXml.SetAttribute(LCase("Cau_Mov"), Movimento.Cau_Mov)
            nodoXml.SetAttribute(LCase("Mov_Desc"), Movimento.Mov_Desc)
            nodoXml.SetAttribute(LCase("Data_Movimento"), Format(Movimento.Data, "dd/MM/yyyy"))
            nodoXml.SetAttribute(LCase("ora"), Format(Movimento.Ora, "HH:mm"))
            nodoXml.SetAttribute(LCase("Scadenza"), Format(AGRODATAFINE, "dd/MM/yyyy"))
            nodoXml.SetAttribute(LCase("Scadenza_Extra"), Format(AGRODATAINIZIO, "dd/MM/yyyy"))
            nodoXml.SetAttribute(LCase("Doc_Numero_Sin"), "")
            nodoXml.SetAttribute(LCase("Doc_Numero"), CStr(Movimento.Doc_Numero))
            nodoXml.SetAttribute(LCase("Doc_Numero_Des"), "")
            nodoXml.SetAttribute(LCase("Num_Protocollo"), CStr(Movimento.Num_Protocollo))
            nodoXml.SetAttribute(LCase("Num_Protocollo_Decimal"), CStr(Movimento.Num_Protocollo_Decimal))
            nodoXml.SetAttribute(LCase("Disciplinare_PubblicoPrivato"), CStr(Movimento.Disciplinare_PubblicoPrivato))
            nodoXml.SetAttribute(LCase("colli"), CStr(0))
            nodoXml.SetAttribute(LCase("peso"), CStr(0.0))
            nodoXml.SetAttribute(LCase("Aspetto"), "")
            nodoXml.SetAttribute(LCase("Causale_Trasporto"), "")
            nodoXml.SetAttribute(LCase("Tipo_Sconto"), CStr(0))
            nodoXml.SetAttribute(LCase("Cod_IndirizzoRisUm"), CStr(0))
            nodoXml.SetAttribute(LCase("Cod_Destinazione"), CStr(0))
            nodoXml.SetAttribute(LCase("Cod_IndirizzoDestinazione"), CStr(0))
            nodoXml.SetAttribute(LCase("Mezzo"), CStr(Movimento.Mezzo))
            nodoXml.SetAttribute(LCase("Cod_Vettore"), CStr(0))
            nodoXml.SetAttribute(LCase("Cod_IndirizzoVettore"), CStr(0))
            nodoXml.SetAttribute(LCase("Natura_Beni"), "")
            nodoXml.SetAttribute(LCase("Modalita"), CStr(Movimento.Modalita))
            nodoXml.SetAttribute(LCase("Tara_Veicolo"), CStr(0))
            nodoXml.SetAttribute(LCase("Tara_Imballi"), CStr(0))
            nodoXml.SetAttribute(LCase("Tipo_Peso"), CStr(0))
            nodoXml.SetAttribute(LCase("Username_Note"), "")
            nodoXml.SetAttribute(LCase("Extra_Str"), "")
            nodoXml.SetAttribute(LCase("Extra_Int"), CStr(Movimento.Extra_Int))
            nodoXml.SetAttribute(LCase("Extra_Date"), CStr(AGRODATAINIZIO))
            nodoXml.SetAttribute(LCase("validita_inizio"), Format(Movimento.Data, "dd/MM/yyyy"))
            nodoXml.SetAttribute(LCase("validita_fine"), Format(AGRODATAFINE, "dd/MM/yyyy"))
            nodoXml.SetAttribute(LCase("basecode"), CStr(Movimento.BaseCode))
            nodoXml.SetAttribute(LCase("topcode"), CStr(Movimento.TopCode))
            nodoXml.SetAttribute(LCase("Modalita_Applicazione"), CStr(Movimento.Modalita_Applicazione))

            'Restituisco in uscita 
            Return nodoXml

        End Function


        Public Function XML_Agenda_MovimentoDettaglioTecnico(
                            ByVal tipoOperazioneDb As enum_TipoOperazioneDB,
                            Optional ByVal piva As String = "00000000000",
                            Optional ByVal saCod As Integer = 0,
                            Optional ByVal idAgenda As Integer = 0,
                            Optional ByVal idMov As Integer = 0,
                            Optional ByVal idMovDet As Integer = 0,
                            Optional ByVal idRegDettaglio As Integer = 0,
                            Optional ByVal qtaRil As Decimal = 0,
                            Optional ByVal dataRil As String = "0",
                            Optional ByVal dittaCod As Integer = 0,
                            Optional ByVal dettCod As Integer = 0,
                            Optional ByVal idInsetto As Integer = 0,
                            Optional ByVal ffClasse As Integer = 0,
                            Optional ByVal dose As Decimal = 0,
                            Optional ByVal Mg As Decimal = 0,
                            Optional ByVal N As Decimal = 0,
                            Optional ByVal K As Decimal = 0,
                            Optional ByVal P As Decimal = 0,
                            Optional ByVal parziale As Integer = 0,
                            Optional ByVal nitrati As Integer = 0,
                            Optional ByVal freatimetro As Decimal = 0,
                            Optional ByVal piezo1 As Decimal = 0,
                            Optional ByVal piezo2 As Decimal = 0,
                            Optional ByVal piezo3 As Decimal = 0,
                            Optional ByVal piezo4 As Decimal = 0,
                            Optional ByVal siglaAv As String = "0",
                            Optional ByVal trapNum As Integer = 0,
                            Optional ByVal inn1Data As String = "0",
                            Optional ByVal inn2Data As String = "0",
                            Optional ByVal inn3Data As String = "0",
                            Optional ByVal inn4Data As String = "0",
                            Optional ByVal avCod As Integer = 0,
                            Optional ByVal avGru As Integer = 0,
                            Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                            Optional ByVal validitaFine As Date = AGRODATAFINE,
                            Optional ByVal baseCode As Integer = 0,
                            Optional ByVal topCode As Integer = 200000000,
                            Optional ByVal lotto As String = ""
                            ) As String

            Dim xmlDoc As New XmlDocument
            Dim xmlTxt As XmlElement

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico")

            'Imposto gli attributi
            xmlTxt.SetAttribute("TipoOperazioneDB", CStr(tipoOperazioneDb))
            xmlTxt.SetAttribute(LCase("piva"), piva)
            xmlTxt.SetAttribute(LCase("sa_cod"), CStr(saCod))
            xmlTxt.SetAttribute(LCase("id_agenda"), CStr(idAgenda))
            xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(idMov))
            xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(idMovDet))
            xmlTxt.SetAttribute(LCase("ID_Reg_Dettaglio"), CStr(idRegDettaglio))
            xmlTxt.SetAttribute(LCase("Qta_Ril"), CStr(qtaRil))
            xmlTxt.SetAttribute(LCase("Data_Ril"), dataRil)
            xmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(dittaCod))
            xmlTxt.SetAttribute(LCase("Dett_Cod"), CStr(dettCod))
            xmlTxt.SetAttribute(LCase("ID_Insetto"), CStr(idInsetto))
            xmlTxt.SetAttribute(LCase("FF_Classe"), CStr(ffClasse))
            xmlTxt.SetAttribute(LCase("Dose"), CStr(dose))
            xmlTxt.SetAttribute(LCase("Mg"), CStr(Mg))
            xmlTxt.SetAttribute(LCase("N"), CStr(N))
            xmlTxt.SetAttribute(LCase("K"), CStr(K))
            xmlTxt.SetAttribute(LCase("P"), CStr(P))
            xmlTxt.SetAttribute(LCase("Parziale"), CStr(parziale))
            xmlTxt.SetAttribute(LCase("Nitrati"), CStr(nitrati))
            xmlTxt.SetAttribute(LCase("Freatimetro"), CStr(freatimetro))
            xmlTxt.SetAttribute(LCase("Piezo1"), CStr(piezo1))
            xmlTxt.SetAttribute(LCase("Piezo2"), CStr(piezo2))
            xmlTxt.SetAttribute(LCase("Piezo3"), CStr(piezo3))
            xmlTxt.SetAttribute(LCase("Piezo4"), CStr(piezo4))
            xmlTxt.SetAttribute(LCase("Sigla_AV"), siglaAv)
            xmlTxt.SetAttribute(LCase("Trap_Num"), CStr(trapNum))
            xmlTxt.SetAttribute(LCase("Inn1_Data"), inn1Data)
            xmlTxt.SetAttribute(LCase("Inn2_Data"), inn2Data)
            xmlTxt.SetAttribute(LCase("Inn3_Data"), inn3Data)
            xmlTxt.SetAttribute(LCase("Inn4_Data"), inn4Data)
            xmlTxt.SetAttribute(LCase("Av_Cod"), CStr(avCod))
            xmlTxt.SetAttribute(LCase("Av_Gru"), CStr(avGru))
            xmlTxt.SetAttribute(LCase("validita_inizio"), Format(validitaInizio, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("validita_fine"), Format(validitaFine, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("basecode"), CStr(baseCode))
            xmlTxt.SetAttribute(LCase("topcode"), CStr(topCode))
            xmlTxt.SetAttribute(LCase("lotto"), CStr(lotto))

            'Imposto XmlTxt come figlio del documento principale
            xmlDoc.AppendChild(xmlTxt)

            'Restituisco in uscita la stringa creata
            Return xmlDoc.InnerXml

        End Function


        Private Function XML_Agenda_MovimentoDettaglio(
                           ByVal tipoOperazioneDb As enum_TipoOperazioneDB,
                           Optional ByVal piva As String = "00000000000",
                           Optional ByVal saCod As Integer = 0,
                           Optional ByVal idAgenda As Integer = 0,
                           Optional ByVal idMov As Integer = 0,
                           Optional ByVal idMovDet As Integer = 0,
                           Optional ByVal elemCod As Integer = 0,
                           Optional ByVal proCod As Integer = 0,
                           Optional ByVal matCod As Integer = 0,
                           Optional ByVal movDetDes As String = "",
                           Optional ByVal udmCod As Integer = 0,
                           Optional ByVal extraInt As Integer = 0,
                           Optional ByVal qta As Decimal = 0,
                           Optional ByVal codIva As Integer = 0,
                           Optional ByVal sconto As Decimal = 0,
                           Optional ByVal prezzoUnitario As Decimal = 0,
                           Optional ByVal codConto As Integer = 0,
                           Optional ByVal codProgetto As Integer = 0,
                           Optional ByVal faseCod As Integer = 0,
                           Optional ByVal contabilizzato As Integer = 1,
                           Optional ByVal pendente As Integer = enum_Pendenza.MovESENTE,
                           Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                           Optional ByVal validitaFine As Date = AGRODATAFINE,
                           Optional ByVal baseCode As Integer = 0,
                           Optional ByVal topCode As Integer = 200000000,
                           Optional ByVal idDestinazione As Integer = 0,
                           Optional ByVal cauMov As String = "",
                           Optional ByVal oldQta As Decimal = 0,
                           Optional ByVal oldPrezzoUnitario As Decimal = 0,
                           Optional ByVal calCod As Integer = 0,
                           Optional ByVal lotto As String = "",
                           Optional ByVal anno As Integer = 1900,
                           Optional ByVal udmCodExtra As Integer = 0,
                           Optional ByVal qtaExtra As Decimal = 0,
                           Optional ByVal tempoCarenza As Integer = 0,
                           Optional ByVal doseEtichetta As String = "",
                           Optional ByVal principiAttivi As String = "",
                           Optional ByVal classiTossicologiche As String = "",
                           Optional ByVal doseEtichettaValue As String = "",
                           Optional ByVal qualificaCod As Integer = 0,
                           Optional ByVal tariffaCod As Integer = 0,
                           Optional ByVal idAttivita As Integer = 0,
                           Optional ByVal qtaExtraTotale As Decimal = 0,
                           Optional ByVal mezzoDet As Integer = 0,
                           Optional ByVal buffer As String = "",
                           Optional ByVal extraStr As String = "",
                           Optional ByVal principiAttiviPercAbb As String = "",
                           Optional ByVal principiAttiviPesi As String = "",
                           Optional ByVal polverulento As Integer = 0
                          ) As String

            Dim xmlDoc As New XmlDocument
            Dim xmlTxt As XmlElement

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio")

            'Imposto gli attributi
            xmlTxt.SetAttribute("TipoOperazioneDB", CStr(tipoOperazioneDb))
            xmlTxt.SetAttribute(LCase("piva"), piva)
            xmlTxt.SetAttribute(LCase("sa_cod"), CStr(saCod))
            xmlTxt.SetAttribute(LCase("id_agenda"), CStr(idAgenda))
            xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(idMov))
            xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(idMovDet))
            xmlTxt.SetAttribute(LCase("Elem_Cod"), CStr(elemCod))
            xmlTxt.SetAttribute(LCase("Pro_Cod"), CStr(proCod))
            xmlTxt.SetAttribute(LCase("Mat_Cod"), CStr(matCod))
            xmlTxt.SetAttribute(LCase("Mov_Det_Des"), movDetDes)
            xmlTxt.SetAttribute(LCase("Udm_Cod"), CStr(udmCod))
            xmlTxt.SetAttribute(LCase("Extra_Int"), CStr(extraInt))
            xmlTxt.SetAttribute(LCase("Qta"), CStr(qta))
            xmlTxt.SetAttribute(LCase("Cod_IVA"), CStr(codIva))
            xmlTxt.SetAttribute(LCase("Sconto"), CStr(sconto))
            xmlTxt.SetAttribute(LCase("Prezzo_Unitario"), CStr(prezzoUnitario))
            xmlTxt.SetAttribute(LCase("Cod_Conto"), CStr(codConto))
            xmlTxt.SetAttribute(LCase("Cod_Progetto"), CStr(codProgetto))
            xmlTxt.SetAttribute(LCase("Fase_Cod"), CStr(faseCod))
            xmlTxt.SetAttribute(LCase("Contabilizzato"), CStr(contabilizzato))
            xmlTxt.SetAttribute(LCase("Pendente"), CStr(pendente))
            xmlTxt.SetAttribute(LCase("validita_inizio"), Format(validitaInizio, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("validita_fine"), Format(validitaFine, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("basecode"), CStr(baseCode))
            xmlTxt.SetAttribute(LCase("topcode"), CStr(topCode))
            xmlTxt.SetAttribute(LCase("ID_Destinazione"), CStr(idDestinazione))
            xmlTxt.SetAttribute(LCase("Cau_Mov"), cauMov)
            xmlTxt.SetAttribute(LCase("Old_Qta"), CStr(oldQta))
            xmlTxt.SetAttribute(LCase("Old_Prezzo_Unitario"), CStr(oldPrezzoUnitario))
            xmlTxt.SetAttribute(LCase("Cal_Cod"), CStr(calCod))
            xmlTxt.SetAttribute(LCase("Lotto"), CStr(lotto))
            xmlTxt.SetAttribute(LCase("id_attivita"), CInt(idAttivita))
            xmlTxt.SetAttribute(LCase("Anno"), CInt(anno))
            xmlTxt.SetAttribute(LCase("udm_cod_extra"), CInt(udmCodExtra))
            xmlTxt.SetAttribute(LCase("qta_extra"), CDec(qtaExtra))
            xmlTxt.SetAttribute(LCase("TempoCarenza"), CInt(tempoCarenza))
            xmlTxt.SetAttribute(LCase("DoseEtichetta"), CStr(doseEtichetta))
            xmlTxt.SetAttribute(LCase("PrincipiAttivi"), CStr(principiAttivi))
            xmlTxt.SetAttribute(LCase("PrincipiAttiviPercAbb"), CStr(principiAttiviPercAbb))
            xmlTxt.SetAttribute(LCase("ClassiTossicologiche"), CStr(classiTossicologiche))
            xmlTxt.SetAttribute(LCase("DoseEtichetta_Value"), CStr(doseEtichettaValue))
            xmlTxt.SetAttribute(LCase("Qualifica_Cod"), CInt(qualificaCod))
            xmlTxt.SetAttribute(LCase("Tariffa_Cod"), CInt(tariffaCod))
            xmlTxt.SetAttribute(LCase("qta_extra_totale"), CDec(qtaExtraTotale))
            xmlTxt.SetAttribute(LCase("mezzo_det"), CInt(mezzoDet))
            xmlTxt.SetAttribute(LCase("buffer"), CStr(buffer))
            xmlTxt.SetAttribute(LCase("Extra_Str"), CStr(extraStr))
            xmlTxt.SetAttribute(LCase("polverulento"), CInt(polverulento))

            xmlTxt.SetAttribute(LCase("PrincipiAttiviPesi"), CStr(principiAttiviPesi))

            'Imposto XmlTxt come figlio del documento principale
            xmlDoc.AppendChild(xmlTxt)

            'Restituisco in uscita la stringa creata
            Return xmlDoc.InnerXml

        End Function


        Private Function XML_Agenda_MovimentoDettaglioTecnico_2(
                           ByVal tipoOperazioneDb As enum_TipoOperazioneDB,
                           Optional ByVal piva As String = "00000000000",
                           Optional ByVal saCod As Integer = 0,
                           Optional ByVal idAgenda As Integer = 0,
                           Optional ByVal idMov As Integer = 0,
                           Optional ByVal idMovDet As Integer = 0,
                           Optional ByVal idRegDettaglio As Integer = 0,
                           Optional ByVal qtaRil As Decimal = 0,
                           Optional ByVal dataRil As String = "0",
                           Optional ByVal dittaCod As Integer = 0,
                           Optional ByVal dettCod As Integer = 0,
                           Optional ByVal idInsetto As Integer = 0,
                           Optional ByVal ffClasse As Integer = 0,
                           Optional ByVal dose As Decimal = 0,
                           Optional ByVal Mg As Decimal = 0,
                           Optional ByVal N As Decimal = 0,
                           Optional ByVal K As Decimal = 0,
                           Optional ByVal P As Decimal = 0,
                           Optional ByVal Cu As Decimal = 0,
                           Optional ByVal parziale As Integer = 0,
                           Optional ByVal nitrati As Integer = 0,
                           Optional ByVal freatimetro As Decimal = 0,
                           Optional ByVal piezo1 As Decimal = 0,
                           Optional ByVal piezo2 As Decimal = 0,
                           Optional ByVal piezo3 As Decimal = 0,
                           Optional ByVal piezo4 As Decimal = 0,
                           Optional ByVal siglaAv As String = "0",
                           Optional ByVal trapNum As Integer = 0,
                           Optional ByVal inn1Data As String = "0",
                           Optional ByVal inn2Data As String = "0",
                           Optional ByVal inn3Data As String = "0",
                           Optional ByVal inn4Data As String = "0",
                           Optional ByVal avCod As Integer = 0,
                           Optional ByVal avGru As Integer = 0,
                           Optional ByVal sogliaCod As Integer = 0,
                           Optional ByVal sogliaDes As String = "",
                           Optional ByVal sogliaQuantita As Integer = 0,
                           Optional ByVal efficienza As Decimal = 0,
                           Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                           Optional ByVal validitaFine As Date = AGRODATAFINE,
                           Optional ByVal baseCode As Integer = 0,
                           Optional ByVal topCode As Integer = 200000000,
                           Optional ByVal Ditta_Cod As String = "0"
                           ) As String



            Dim xmlDoc As New XmlDocument
            Dim xmlTxt As XmlElement

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_2")

            'Imposto gli attributi
            xmlTxt.SetAttribute("TipoOperazioneDB", CStr(tipoOperazioneDb))
            xmlTxt.SetAttribute(LCase("piva"), piva)
            xmlTxt.SetAttribute(LCase("sa_cod"), CStr(saCod))
            xmlTxt.SetAttribute(LCase("id_agenda"), CStr(idAgenda))
            xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(idMov))
            xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(idMovDet))
            xmlTxt.SetAttribute(LCase("ID_Reg_Dettaglio"), CStr(idRegDettaglio))
            xmlTxt.SetAttribute(LCase("Qta_Ril"), CStr(qtaRil))
            xmlTxt.SetAttribute(LCase("Data_Ril"), dataRil)
            xmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(dittaCod))
            xmlTxt.SetAttribute(LCase("Dett_Cod"), CStr(dettCod))
            xmlTxt.SetAttribute(LCase("ID_Insetto"), CStr(idInsetto))
            xmlTxt.SetAttribute(LCase("FF_Classe"), CStr(ffClasse))
            xmlTxt.SetAttribute(LCase("Dose"), CStr(dose))
            xmlTxt.SetAttribute(LCase("Mg"), CStr(Mg))
            xmlTxt.SetAttribute(LCase("N"), CStr(N))
            xmlTxt.SetAttribute(LCase("K"), CStr(K))
            xmlTxt.SetAttribute(LCase("P"), CStr(P))
            xmlTxt.SetAttribute(LCase("cu"), CStr(Cu))
            xmlTxt.SetAttribute(LCase("Parziale"), CStr(parziale))
            xmlTxt.SetAttribute(LCase("Nitrati"), CStr(nitrati))
            xmlTxt.SetAttribute(LCase("Freatimetro"), CStr(freatimetro))
            xmlTxt.SetAttribute(LCase("Piezo1"), CStr(piezo1))
            xmlTxt.SetAttribute(LCase("Piezo2"), CStr(piezo2))
            xmlTxt.SetAttribute(LCase("Piezo3"), CStr(piezo3))
            xmlTxt.SetAttribute(LCase("Piezo4"), CStr(piezo4))
            xmlTxt.SetAttribute(LCase("Ditta_Cod"), Ditta_Cod)
            xmlTxt.SetAttribute(LCase("Sigla_AV"), siglaAv)
            xmlTxt.SetAttribute(LCase("Trap_Num"), CStr(trapNum))
            xmlTxt.SetAttribute(LCase("Inn1_Data"), inn1Data)
            xmlTxt.SetAttribute(LCase("Inn2_Data"), inn2Data)
            xmlTxt.SetAttribute(LCase("Inn3_Data"), inn3Data)
            xmlTxt.SetAttribute(LCase("Inn4_Data"), inn4Data)
            xmlTxt.SetAttribute(LCase("Av_Cod"), CStr(avCod))
            xmlTxt.SetAttribute(LCase("Av_Gru"), CStr(avGru))
            xmlTxt.SetAttribute(LCase("Soglia_Cod"), CStr(sogliaCod))
            xmlTxt.SetAttribute(LCase("Soglia_Des"), CStr(sogliaDes))
            xmlTxt.SetAttribute(LCase("Soglia_Quantita"), CStr(sogliaQuantita))
            xmlTxt.SetAttribute(LCase("Efficienza"), CStr(efficienza))
            xmlTxt.SetAttribute(LCase("validita_inizio"), Format(validitaInizio, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("validita_fine"), Format(validitaFine, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("basecode"), CStr(baseCode))
            xmlTxt.SetAttribute(LCase("topcode"), CStr(topCode))

            'Imposto XmlTxt come figlio del documento principale
            xmlDoc.AppendChild(xmlTxt)

            'Restituisco in uscita la stringa creata
            Return xmlDoc.InnerXml

        End Function

        Private Function XML_Agenda_MovimentoDestinazione(
                            ByVal tipoOperazioneDb As enum_TipoOperazioneDB,
                            Optional ByVal piva As String = "00000000000",
                            Optional ByVal saCod As Integer = 0,
                            Optional ByVal idAgenda As Integer = 0,
                            Optional ByVal idMov As Integer = 0,
                            Optional ByVal idMovDet As Integer = 0,
                            Optional ByVal appezza As Integer = 0,
                            Optional ByVal idDestinazione As Integer = 0,
                            Optional ByVal tipoDestinazione As Integer = 0,
                            Optional ByVal qta As Decimal = 0,
                            Optional ByVal qta2 As Decimal = 0,
                            Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                            Optional ByVal validitaFine As Date = AGRODATAFINE,
                            Optional ByVal baseCode As Integer = 0,
                            Optional ByVal topCode As Integer = 200000000,
                            Optional ByVal programmazioneEntitaCod As Integer = 0,
                            Optional ByVal quotaDistribuzione As Decimal = 0D,
                            Optional ByVal supRiduzioneBufferZone As Decimal = 0D,
                            Optional ByVal percRiduzioneDeriva As Decimal = 0D,
                            Optional ByVal magazzinoEsterno_Cod As String = "",
                            Optional ByVal magazzinoEsterno_Des As String = "",
                            Optional ByVal magazzinoEsterno_Dettagli As String = ""
                            ) As String

            Dim xmlDoc As New XmlDocument
            Dim xmlTxt As XmlElement

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            xmlTxt = xmlDoc.CreateElement("Movimento_Destinazione")

            'Imposto gli attributi
            xmlTxt.SetAttribute("TipoOperazioneDB", CStr(tipoOperazioneDb))
            xmlTxt.SetAttribute(LCase("piva"), piva)
            xmlTxt.SetAttribute(LCase("sa_cod"), CStr(saCod))
            xmlTxt.SetAttribute(LCase("id_agenda"), CStr(idAgenda))
            xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(idMov))
            xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(idMovDet))
            xmlTxt.SetAttribute(LCase("Appezza"), CStr(appezza))
            xmlTxt.SetAttribute(LCase("ID_Destinazione"), CStr(idDestinazione))
            xmlTxt.SetAttribute(LCase("Tipo_Destinazione"), CStr(tipoDestinazione))
            xmlTxt.SetAttribute(LCase("Qta"), CStr(qta))
            xmlTxt.SetAttribute(LCase("Qta2"), CStr(qta2))
            xmlTxt.SetAttribute(LCase("validita_inizio"), Format(validitaInizio, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("validita_fine"), Format(validitaFine, "dd/MM/yyyy"))
            xmlTxt.SetAttribute(LCase("basecode"), CStr(baseCode))
            xmlTxt.SetAttribute(LCase("topcode"), CStr(topCode))

            xmlTxt.SetAttribute(LCase("programmazione_entita_cod"), CStr(programmazioneEntitaCod))

            xmlTxt.SetAttribute(LCase("QuotaDistribuzione"), CStr(quotaDistribuzione))

            xmlTxt.SetAttribute(LCase("Sup_Riduzione_BufferZone"), CStr(supRiduzioneBufferZone))
            xmlTxt.SetAttribute(LCase("Perc_Riduzione_Deriva"), CStr(percRiduzioneDeriva))

            xmlTxt.SetAttribute(LCase("magazzinoesterno_cod"), CStr(magazzinoEsterno_Cod))
            xmlTxt.SetAttribute(LCase("magazzinoesterno_des"), CStr(magazzinoEsterno_Des))
            xmlTxt.SetAttribute(LCase("magazzinoesterno_dettagli"), CStr(magazzinoEsterno_Dettagli))

            'Imposto XmlTxt come figlio del documento principale
            xmlDoc.AppendChild(xmlTxt)

            'Restituisco in uscita la stringa creata
            Return xmlDoc.InnerXml

        End Function



        Public Function GeneraXML_CAU_TRATTAMENTO(ByRef Agenda As Operazione_Agenda,
                                                  ByRef dpiCod As Integer,
                                                  ByRef dpiPubblicoPrivato As Integer
                                                  ) As String

            '------------------------------------------------
            '----- Definizione delle Variabili
            '------------------------------------------------

            Dim xmlDoc As New XmlDocument
            Dim xmlDoc2 As New XmlDocument

            Dim xmlDatiAgenda As XmlElement
            Dim xmlAgenda As XmlElement
            Dim XmlDatiNote As XmlElement
            Dim XmlNota As XmlElement
            Dim xmlDatiMovimenti As XmlElement
            Dim xmlMovimento As XmlElement

            Dim xmlDatiMovDettagliTecnici As XmlElement
            Dim xmlDatiMovimentiDettagli As XmlElement
            Dim xmlMovimentoDettaglio As XmlElement

            Dim strDatiMovimentiDettagli As String = ""
            'DPI
            Dim strMovimentoDettaglio As String = ""

            Dim i, j As Integer



            '------------------------------------------------
            '----- Genero la struttura XML
            '------------------------------------------------
            '----- DatiAgenda
            xmlDatiAgenda = xmlDoc.CreateElement("DatiAgenda")

            Dim errore As String = ""
            xmlDatiAgenda.InnerXml = XML_Agenda_Agenda(errore, Agenda)
            If errore <> "" Then
                Throw New Exception(errore)
            End If
            xmlAgenda = xmlDatiAgenda.SelectSingleNode("Agenda")


            '--- NOTE

            If Agenda.Note.Count > 0 Then

                XmlDatiNote = xmlDoc.CreateElement("DatiNote")

                'Effettuo un ciclo sui Dettagli Tecnici del Movimento
                For n = 0 To Agenda.Note.Count - 1

                    '----- < NOTA > -----
                    XmlNota = xmlDoc.CreateElement("Nota")

                    With XmlNota

                        .SetAttribute("TipoOperazioneDB", Agenda.Tipo_Operazione)
                        .SetAttribute("id_agenda", Agenda.Note(n).Id_Agenda)
                        .SetAttribute("nota_cod", Agenda.Note(n).Nota_Cod)
                        .SetAttribute("validita_inizio", AGRODATAINIZIO)
                        .SetAttribute("validita_fine", AGRODATAFINE)

                        .SetAttribute("data_creazione", Agenda.Note(n).Data_Creazione)
                        .SetAttribute("data_modifica", Agenda.Note(n).Data_Modifica)
                        .SetAttribute("username_creazione", Agenda.Note(n).Username_Creazione)
                        .SetAttribute("username_modifica", Agenda.Note(n).Username_Modifica)


                    End With

                    XmlDatiNote.AppendChild(XmlNota)
                    '----- < / NOTA > -----

                Next

                xmlAgenda.AppendChild(XmlDatiNote)

            End If


            '----- DatiMovimenti

            xmlDatiMovimenti = xmlDoc.CreateElement("DatiMovimenti")
            xmlAgenda.AppendChild(xmlDatiMovimenti)


            '----- Movimento
            For i = 0 To Agenda.Movimenti.Count - 1

                strDatiMovimentiDettagli = ""

                Select Case Agenda.Movimenti(i).Cau_Mov

                    Case CAU_TRATTAMENTO

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, Agenda.Movimenti(i))

                        dpiCod = Agenda.Movimenti(i).Num_Protocollo
                        dpiPubblicoPrivato = Agenda.Movimenti(i).Disciplinare_PubblicoPrivato

                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)


                        '----- DatiMov_Dettagli_Tecnici
                        xmlDatiMovDettagliTecnici = xmlDoc.CreateElement("DatiMov_Dettagli_Tecnici")
                        xmlMovimento.AppendChild(xmlDatiMovDettagliTecnici)

                        Dim stringaXml As String

                        If Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count > 0 Then

                            'trovo qtaril 
                            Dim jj As Integer

                            Dim Qta_Ril As Decimal = 0

                            Dim BaseCode As Integer = 0

                            Dim TopCode As Integer = 200000000

                            For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                                If Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril <> 0 Then
                                    Qta_Ril = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(jj).Qta_Ril
                                    BaseCode = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(jj).BaseCode
                                    TopCode = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(jj).TopCode
                                    Exit For
                                End If
                            Next

                            'appendo Movimento_Dettaglio_Tecnico

                            stringaXml = XML_Agenda_MovimentoDettaglioTecnico(
                                    Agenda.Tipo_Operazione,
                                    piva:=Agenda.Piva,
                                    saCod:=Agenda.Sa_Cod,
                                    qtaRil:=Qta_Ril,
                                    avCod:=0,
                                    avGru:=0,
                                    validitaInizio:=AGRODATAINIZIO,
                                    validitaFine:=AGRODATAFINE,
                                    baseCode:=BaseCode,
                                    topCode:=TopCode)

                            xmlDatiMovDettagliTecnici.InnerXml = stringaXml

                        End If

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                ,
                                                , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta,
                                                , , , , , , , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Data,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).BaseCode,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TopCode,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Cau_Mov,
                                                , , , Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto, ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod_Extra,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TempoCarenza,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).DoseEtichetta,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).PrincipiAttivi,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).CLassiTossicologiche,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).DoseEtichetta_Value,
                                                ,,,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra_Totale,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mezzo_Det,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Buffer,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Str,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).PrincipiAttiviPercAbb,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).PrincipiAttiviPesi,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Polverulento
                                                )


                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            'nel caso Nessun Filtro e caso vecchio senza DPI ho 1 nodo Movimento_Dettaglio x ogni formulato 
                            'con tanti nodi figli Movimento_Destinazione quanti sono gli impianti su cui intervengo

                            'nel caso filtro Coltura e Coltura / Avversità ho 1 nodo Movimento_Dettaglio x ogni formulato 
                            'con un nodo figlio Movimento_Dettaglio_Tecnico_2 x ogni avversità
                            '+ tanti nodi figli Movimento_Destinazione quanti sono gli impianti su cui intervengo
                            'Select Case frm_CodDisciplinare
                            '    Case 0
                            '        XML_MovimentoDettaglio.InnerXml = XML_GeneraBlocco_MovimentoDestinazione(i)
                            '    Case Else
                            stringaXml = ""

                            If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count > 0 Then
                                stringaXml = XML_Agenda_MovimentoDettaglioTecnico_2(
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    piva:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Piva,
                                                    saCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sa_Cod,
                                                    Ditta_Cod:=If(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Ditta_cod <> 0, Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Ditta_cod, "0"),
                                                    siglaAv:=If(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sigla_av <> "", Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sigla_av, "0"),
                                                    avCod:=CInt(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Av_Cod),
                                                    avGru:=CInt(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Av_Gru),
                                                    sogliaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Soglia_Cod,
                                                    sogliaDes:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Soglia_Des,
                                                    sogliaQuantita:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Soglia_Quantita,
                                                    validitaInizio:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Data,
                                                    validitaFine:=AGRODATAFINE,
                                                    baseCode:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).BaseCode,
                                                    topCode:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).TopCode,
                                                    dose:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Dose)

                            End If


                            For jj = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                'Genero la stringa XML del nodo
                                stringaXml += XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Piva,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Appezza,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Id_Destinazione,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Tipo,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Qta,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Qta2,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Data,
                                                 AGRODATAFINE,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).BaseCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).TopCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Programmazione_Entita_Cod,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).QuotaDistribuzione,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Sup_Riduzione_BufferZone,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Perc_Riduzione_Deriva)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                    Case CAU_SCARICO

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, Agenda.Movimenti(i))
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                            'AAAAAAAAA
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                ,
                                                , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod,
                                                , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta,
                                                , , , , , , , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Data,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).BaseCode,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TopCode,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Cau_Mov,
                                                , , , Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto, ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod_Extra,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TempoCarenza,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).DoseEtichetta,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).PrincipiAttivi,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).CLassiTossicologiche,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).DoseEtichetta_Value)


                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            Dim stringaXml As String = ""

                            For jj = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                'Genero la stringa XML del nodo
                                stringaXml += XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                  Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Piva,
                                                  Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Appezza,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Id_Destinazione,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Tipo,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Qta,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Qta2,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Data,
                                                 AGRODATAFINE,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).BaseCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).TopCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).Programmazione_Entita_Cod,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).QuotaDistribuzione,
                                                 magazzinoEsterno_Cod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).MagazzinoEsterno_Cod,
                                                 magazzinoEsterno_Des:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).MagazzinoEsterno_Des,
                                                 magazzinoEsterno_Dettagli:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(jj).MagazzinoEsterno_Dettagli)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli


                    Case Else

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, Agenda.Movimenti(i))
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)


                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        '  Vanni, 21/09/2016 18:00:47: Aggiunti dati salvataggio sbtf costi
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                ,
                                                 , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta,
                                                , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Prezzo_Unitario _
                                                , , , , , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Data,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).BaseCode,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TopCode,
                                                , , ,
                                                lotto:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto,
                                                qualificaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Qualifica_Cod,
                                                tariffaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Tariffa_Cod,
                                                idAttivita:=Agenda.Movimenti(i).Movimenti_Dettagli(j).ID_Attivita)


                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next

                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                End Select

            Next

            xmlDoc.AppendChild(xmlDatiAgenda)

            Return xmlDoc.OuterXml

        End Function


        Public Function GeneraXML_CAU_LAVORAZIONI(ByRef Agenda As Operazione_Agenda) As String

            '------------------------------------------------
            '----- Definizione delle Variabili
            '------------------------------------------------

            Dim xmlDoc As New XmlDocument
            Dim xmlDoc2 As New XmlDocument

            Dim xmlDatiAgenda As XmlElement
            Dim xmlAgenda As XmlElement
            Dim XmlDatiNote As XmlElement
            Dim XmlNota As XmlElement
            Dim xmlDatiMovimenti As XmlElement
            Dim xmlMovimento As XmlElement

            Dim xmlDatiMovimentiDettagli As XmlElement
            Dim xmlMovimentoDettaglio As XmlElement

            Dim strDatiMovimentiDettagli As String = ""
            Dim strMovimentoDettaglio As String = ""

            Dim errore As String = ""

            '------------------------------------------------
            '----- Genero la struttura XML
            '------------------------------------------------
            '----- DatiAgenda
            xmlDatiAgenda = xmlDoc.CreateElement("DatiAgenda")

            xmlDatiAgenda.InnerXml = XML_Agenda_Agenda(errore, Agenda)
            If errore <> "" Then
                Throw New Exception(errore)
            End If
            xmlAgenda = xmlDatiAgenda.SelectSingleNode("Agenda")

            '--- NOTE

            If Agenda.Note.Count > 0 Then

                XmlDatiNote = xmlDoc.CreateElement("DatiNote")

                'Effettuo un ciclo sui Dettagli Tecnici del Movimento
                For n = 0 To Agenda.Note.Count - 1

                    '----- < NOTA > -----
                    XmlNota = xmlDoc.CreateElement("Nota")

                    With XmlNota

                        .SetAttribute("TipoOperazioneDB", Agenda.Tipo_Operazione)
                        .SetAttribute("id_agenda", Agenda.Note(n).Id_Agenda)
                        .SetAttribute("nota_cod", Agenda.Note(n).Nota_Cod)
                        .SetAttribute("validita_inizio", AGRODATAINIZIO)
                        .SetAttribute("validita_fine", AGRODATAFINE)

                        .SetAttribute("data_creazione", Agenda.Note(n).Data_Creazione)
                        .SetAttribute("data_modifica", Agenda.Note(n).Data_Modifica)
                        .SetAttribute("username_creazione", Agenda.Note(n).Username_Creazione)
                        .SetAttribute("username_modifica", Agenda.Note(n).Username_Modifica)


                    End With

                    XmlDatiNote.AppendChild(XmlNota)
                    '----- < / NOTA > -----

                Next

                xmlAgenda.AppendChild(XmlDatiNote)

            End If


            xmlDatiMovimenti = xmlDoc.CreateElement("DatiMovimenti")
            xmlAgenda.AppendChild(xmlDatiMovimenti)

            For Each mov As Movimento In Agenda.Movimenti

                strDatiMovimentiDettagli = ""

                Select Case mov.Cau_Mov

                    Case CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, mov)
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)

                        Dim stringaXml As String

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For Each det As Movimento_Dettaglio In mov.Movimenti_Dettagli

                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                det.Piva,
                                                det.Sa_Cod,
                                                ,
                                                , ,
                                                det.Elem_Cod,
                                                det.Pro_Cod,
                                                det.Mat_Cod,
                                                ,
                                                det.Udm_Cod,
                                                det.Extra_Int,
                                                det.Qta,
                                                , , , , , , , ,
                                                det.Data,
                                                ,
                                                det.BaseCode,
                                                det.TopCode,
                                                , , ,,,
                                                det.Lotto,
                                                ,
                                                det.Udm_Cod_Extra,
                                                det.Qta_Extra,
                                                ,,,,,,,,
                                                det.Qta_Extra_Totale,
                                                det.Mezzo_Det)

                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            stringaXml = ""

                            If det.Movimenti_Dettagli_Tecnici.Count > 0 Then
                                Dim dittaCod As String = "0"
                                If Agenda.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                    dittaCod = CStr(det.Movimenti_Dettagli_Tecnici(0).Ditta_cod)
                                End If

                                stringaXml = XML_Agenda_MovimentoDettaglioTecnico_2(
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    det.Movimenti_Dettagli_Tecnici(0).Piva,
                                                    det.Movimenti_Dettagli_Tecnici(0).Sa_Cod,
                                                    , , , ,
                                                    det.Movimenti_Dettagli_Tecnici(0).Qta_Ril,
                                                    , ,
                                                    det.Movimenti_Dettagli_Tecnici(0).dett_cod,
                                                    , ,
                                                    det.Movimenti_Dettagli_Tecnici(0).Dose,
                                                    det.Movimenti_Dettagli_Tecnici(0).M,
                                                    det.Movimenti_Dettagli_Tecnici(0).N,
                                                    det.Movimenti_Dettagli_Tecnici(0).K,
                                                    det.Movimenti_Dettagli_Tecnici(0).P,
                                                    det.Movimenti_Dettagli_Tecnici(0).Cu,
                                                    det.Movimenti_Dettagli_Tecnici(0).Parziale,
                                                    det.Movimenti_Dettagli_Tecnici(0).Nitrati,
                                                    det.Movimenti_Dettagli_Tecnici(0).Freatimetro,
                                                    , , , , , ,
                                                    det.Movimenti_Dettagli_Tecnici(0).Inn1_data,
                                                    det.Movimenti_Dettagli_Tecnici(0).Inn2_data,
                                                    , , , , , , ,
                                                    det.Movimenti_Dettagli_Tecnici(0).Efficienza,
                                                    det.Movimenti_Dettagli_Tecnici(0).Data,
                                                    AGRODATAFINE,
                                                    det.Movimenti_Dettagli_Tecnici(0).BaseCode,
                                                    det.Movimenti_Dettagli_Tecnici(0).TopCode,
                                                    dittaCod)
                            End If

                            For Each dest As Movimento_Destinazione In det.Movimenti_Destinazioni

                                'Genero la stringa XML del nodo
                                stringaXml &= XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                 dest.Piva,
                                                 dest.Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 dest.Appezza,
                                                 dest.Id_Destinazione,
                                                 dest.Tipo,
                                                 dest.Qta,
                                                 dest.Qta2,
                                                 dest.Data,
                                                 AGRODATAFINE,
                                                 dest.BaseCode,
                                                 dest.TopCode,
                                                 dest.Programmazione_Entita_Cod,
                                                 dest.QuotaDistribuzione)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml
                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                    Case CAU_SCARICO

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, mov)
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For Each det As Movimento_Dettaglio In mov.Movimenti_Dettagli
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                det.Piva,
                                                det.Sa_Cod,
                                                ,
                                                , ,
                                                det.Elem_Cod,
                                                det.Pro_Cod,
                                                det.Mat_Cod,
                                                ,
                                                det.Udm_Cod,
                                                det.Extra_Int,
                                                det.Qta,
                                                , , , , , , , ,
                                                det.Data,
                                                ,
                                                det.BaseCode,
                                                det.TopCode,
                                                ,
                                                det.Cau_Mov,
                                                , , ,
                                                lotto:=det.Lotto,
                                                qualificaCod:=det.Qualifica_Cod,
                                                tariffaCod:=det.Tariffa_Cod,
                                                idAttivita:=det.ID_Attivita,
                                                qtaExtra:=det.Qta_Extra,
                                                udmCodExtra:=det.Udm_Cod_Extra,
                                                qtaExtraTotale:=det.Qta_Extra_Totale,
                                                mezzoDet:=det.Mezzo_Det)


                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            Dim stringaXml As String = ""

                            For Each dest As Movimento_Destinazione In det.Movimenti_Destinazioni

                                'Genero la stringa XML del nodo
                                stringaXml &= XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                 dest.Piva,
                                                 dest.Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 dest.Appezza,
                                                 dest.Id_Destinazione,
                                                 dest.Tipo,
                                                 dest.Qta,
                                                 dest.Qta2,
                                                 dest.Data,
                                                 AGRODATAFINE,
                                                 dest.BaseCode,
                                                 dest.TopCode,
                                                 dest.Programmazione_Entita_Cod,
                                                 dest.QuotaDistribuzione,
                                                 magazzinoEsterno_Cod:=dest.MagazzinoEsterno_Cod,
                                                 magazzinoEsterno_Des:=dest.MagazzinoEsterno_Des,
                                                 magazzinoEsterno_Dettagli:=dest.MagazzinoEsterno_Dettagli)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml
                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                    Case CAU_CARICO

                        If Agenda.Lav_Cod <> LAVCOD_RACCOLTA Then
                            Exit Select
                        End If

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, mov)
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For Each det As Movimento_Dettaglio In mov.Movimenti_Dettagli
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                det.Piva,
                                                det.Sa_Cod,
                                                ,
                                                , ,
                                                det.Elem_Cod,
                                                det.Pro_Cod,
                                                det.Mat_Cod,
                                                ,
                                                det.Udm_Cod,
                                                det.Extra_Int,
                                                det.Qta,
                                                , , , , , , , ,
                                                det.Data,
                                                ,
                                                det.BaseCode,
                                                det.TopCode,
                                                ,
                                                det.Cau_Mov,
                                                , , ,
                                                lotto:=det.Lotto,
                                                qualificaCod:=det.Qualifica_Cod,
                                                tariffaCod:=det.Tariffa_Cod,
                                                idAttivita:=det.ID_Attivita,
                                                qtaExtra:=det.Qta_Extra,
                                                udmCodExtra:=det.Udm_Cod_Extra,
                                                qtaExtraTotale:=det.Qta_Extra_Totale,
                                                mezzoDet:=det.Mezzo_Det)


                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            Dim stringaXml As String = ""

                            For Each dest As Movimento_Destinazione In det.Movimenti_Destinazioni

                                'Genero la stringa XML del nodo
                                stringaXml &= XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                 dest.Piva,
                                                 dest.Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 dest.Appezza,
                                                 dest.Id_Destinazione,
                                                 dest.Tipo,
                                                 dest.Qta,
                                                 dest.Qta2,
                                                 dest.Data,
                                                 AGRODATAFINE,
                                                 dest.BaseCode,
                                                 dest.TopCode,
                                                 dest.Programmazione_Entita_Cod,
                                                 dest.QuotaDistribuzione)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml
                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                    Case Else

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, mov)
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)


                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For Each det As Movimento_Dettaglio In mov.Movimenti_Dettagli
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                det.Piva,
                                                det.Sa_Cod,
                                                ,
                                                 , ,
                                                det.Elem_Cod,
                                                det.Pro_Cod,
                                                det.Mat_Cod,
                                                ,
                                                det.Udm_Cod,
                                                det.Extra_Int,
                                                det.Qta,
                                                , ,
                                                det.Prezzo_Unitario _
                                                , , , , , ,
                                                det.Data,
                                                ,
                                                det.BaseCode,
                                                det.TopCode,
                                                , , ,
                                                lotto:=det.Lotto,
                                                qualificaCod:=det.Qualifica_Cod,
                                                tariffaCod:=det.Tariffa_Cod,
                                                idAttivita:=det.ID_Attivita)

                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next

                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                End Select

            Next

            xmlDoc.AppendChild(xmlDatiAgenda)

            Return xmlDoc.OuterXml

        End Function

        Public Function GeneraXML_CAU_LAVORAZIONI(ByRef Agenda As Operazione_Agenda,
                                                  ByRef dpiCod As Integer,
                                                  ByRef dpiPubblicoPrivato As Integer
                                                  ) As String

            '------------------------------------------------
            '----- Definizione delle Variabili
            '------------------------------------------------

            Dim xmlDoc As New XmlDocument
            Dim xmlDoc2 As New XmlDocument

            Dim xmlDatiAgenda As XmlElement
            Dim xmlAgenda As XmlElement
            Dim XmlDatiNote As XmlElement
            Dim XmlNota As XmlElement
            Dim xmlDatiMovimenti As XmlElement
            Dim xmlMovimento As XmlElement

            Dim xmlDatiMovimentiDettagli As XmlElement
            Dim xmlMovimentoDettaglio As XmlElement
            Dim xmlDatiMovDettagliTecnici As XmlElement

            Dim strDatiMovimentiDettagli As String = ""
            Dim strMovimentoDettaglio As String = ""

            Dim i, j As Integer


            '------------------------------------------------
            '----- Genero la struttura XML
            '------------------------------------------------
            '----- DatiAgenda
            xmlDatiAgenda = xmlDoc.CreateElement("DatiAgenda")

            Dim errore As String = ""
            xmlDatiAgenda.InnerXml = XML_Agenda_Agenda(errore, Agenda)
            If errore <> "" Then
                Throw New Exception(errore)
            End If
            xmlAgenda = xmlDatiAgenda.SelectSingleNode("Agenda")



            '--- NOTE

            If Agenda.Note.Count > 0 Then

                XmlDatiNote = xmlDoc.CreateElement("DatiNote")

                'Effettuo un ciclo sui Dettagli Tecnici del Movimento
                For n = 0 To Agenda.Note.Count - 1

                    '----- < NOTA > -----
                    XmlNota = xmlDoc.CreateElement("Nota")

                    With XmlNota

                        .SetAttribute("TipoOperazioneDB", Agenda.Tipo_Operazione)
                        .SetAttribute("id_agenda", Agenda.Note(n).Id_Agenda)
                        .SetAttribute("nota_cod", Agenda.Note(n).Nota_Cod)
                        .SetAttribute("validita_inizio", AGRODATAINIZIO)
                        .SetAttribute("validita_fine", AGRODATAFINE)

                        .SetAttribute("data_creazione", Agenda.Note(n).Data_Creazione)
                        .SetAttribute("data_modifica", Agenda.Note(n).Data_Modifica)
                        .SetAttribute("username_creazione", Agenda.Note(n).Username_Creazione)
                        .SetAttribute("username_modifica", Agenda.Note(n).Username_Modifica)

                    End With

                    XmlDatiNote.AppendChild(XmlNota)
                    '----- < / NOTA > -----

                Next

                xmlAgenda.AppendChild(XmlDatiNote)

            End If

            xmlDatiMovimenti = xmlDoc.CreateElement("DatiMovimenti")
            xmlAgenda.AppendChild(xmlDatiMovimenti)


            For i = 0 To Agenda.Movimenti.Count - 1

                strDatiMovimentiDettagli = ""

                Select Case Agenda.Movimenti(i).Cau_Mov

                    Case CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, Agenda.Movimenti(i))

                        dpiCod = Agenda.Movimenti(i).Num_Protocollo
                        dpiPubblicoPrivato = Agenda.Movimenti(i).Disciplinare_PubblicoPrivato

                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)

                        '----- DatiMov_Dettagli_Tecnici
                        xmlDatiMovDettagliTecnici = xmlDoc.CreateElement("DatiMov_Dettagli_Tecnici")
                        xmlMovimento.AppendChild(xmlDatiMovDettagliTecnici)

                        Dim stringaXml As String

                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) AndAlso Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count > 0 Then

                            stringaXml = XML_Agenda_MovimentoDettaglioTecnico(
                                Agenda.Tipo_Operazione,
                                Agenda.Piva,
                                Agenda.Sa_Cod,
                                , , , ,
                                Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).Qta_Ril, , , , , , , , , , , , , , , , , , , , , , , ,
                                0,
                                0,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).BaseCode,
                                Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).TopCode)

                            xmlDatiMovDettagliTecnici.InnerXml = stringaXml

                        End If

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                ,
                                                ,,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta,
                                                ,,,,,,,,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Data,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).BaseCode,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TopCode,
                                                ,,,,,,,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod_Extra,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra,
                                                ,,,,,,,,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra_Totale,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mezzo_Det)

                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            stringaXml = ""

                            If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count > 0 Then

                                If Agenda.Lav_Cod = LAVCOD_FERTIRRIGAZIONE Then 'gestito separatamente per poter salvare i nuovi dati relativi alla fertirrigazione
                                    stringaXml = XML_Agenda_MovimentoDettaglioTecnico_2(
                                    enum_TipoOperazioneDB.Scrittura,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Piva,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sa_Cod,
                                                                    , , , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril,
                                                                    , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).dett_cod,
                                                                    , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Dose,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).M,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).N,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).K,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).P,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Cu,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Parziale,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Nitrati,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Freatimetro,
                                                                    , , , , , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Inn1_data,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Inn2_data,
                                                                    , , , , , , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Efficienza,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Data,
                                    AGRODATAFINE,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).BaseCode,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).TopCode,
                                    CStr(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Ditta_cod))
                                Else
                                    stringaXml = XML_Agenda_MovimentoDettaglioTecnico_2(
                                    enum_TipoOperazioneDB.Scrittura,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Piva,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sa_Cod,
                                                                    , , , , , , , , , , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).M,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).N,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).K,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).P,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Cu,
                                                                    , , , , , , , ,
                                                                    ,
                                                                    ,
                                                                    , , , , , , , ,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Efficienza,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Data,
                                    AGRODATAFINE,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).BaseCode,
                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).TopCode)
                                End If

                            End If


                            For JJ = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                'Genero la stringa XML del nodo
                                stringaXml += XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                  Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Piva,
                                                  Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Appezza,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Id_Destinazione,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Tipo,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Qta,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Qta2,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Data,
                                                 AGRODATAFINE,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).BaseCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).TopCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Programmazione_Entita_Cod,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).QuotaDistribuzione)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                    Case CAU_SCARICO

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, Agenda.Movimenti(i))
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)

                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                ,
                                                , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta,
                                                , , , , , , , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Data,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).BaseCode,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TopCode,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Cau_Mov,
                                                , , ,
                                                lotto:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto,
                                                qualificaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Qualifica_Cod,
                                                tariffaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Tariffa_Cod,
                                                idAttivita:=Agenda.Movimenti(i).Movimenti_Dettagli(j).ID_Attivita,
                                                qtaExtra:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra,
                                                udmCodExtra:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod_Extra,
                                                qtaExtraTotale:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta_Extra_Totale,
                                                mezzoDet:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Mezzo_Det)


                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            Dim stringaXml As String = ""

                            For JJ = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                'Genero la stringa XML del nodo
                                stringaXml += XML_Agenda_MovimentoDestinazione(
                                                 enum_TipoOperazioneDB.Scrittura,
                                                  Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Piva,
                                                  Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Sa_Cod,
                                                 0,
                                                 ,
                                                 ,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Appezza,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Id_Destinazione,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Tipo,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Qta,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Qta2,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Data,
                                                 AGRODATAFINE,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).BaseCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).TopCode,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).Programmazione_Entita_Cod,
                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).QuotaDistribuzione,
                                                 magazzinoEsterno_Cod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).MagazzinoEsterno_Cod,
                                                 magazzinoEsterno_Des:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).MagazzinoEsterno_Des,
                                                 magazzinoEsterno_Dettagli:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(JJ).MagazzinoEsterno_Dettagli)
                            Next


                            xmlMovimentoDettaglio.InnerXml = stringaXml

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next
                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli


                    Case Else

                        xmlMovimento = XML_2_Agenda_Movimento(errore, xmlDoc, Agenda, Agenda.Movimenti(i))
                        If errore <> "" Then
                            Throw New Exception(errore)
                        End If
                        xmlDatiMovimenti.AppendChild(xmlMovimento)


                        '----- DatiMovimenti_Dettagli
                        xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")
                        xmlMovimento.AppendChild(xmlDatiMovimentiDettagli)

                        'Movimento dettaglio
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                            strMovimentoDettaglio = XML_Agenda_MovimentoDettaglio(
                                                enum_TipoOperazioneDB.Scrittura,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                ,
                                                 , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta,
                                                , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Prezzo_Unitario _
                                                , , , , , ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Data,
                                                ,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).BaseCode,
                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).TopCode,
                                                , , ,
                                                lotto:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto,
                                                qualificaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Qualifica_Cod,
                                                tariffaCod:=Agenda.Movimenti(i).Movimenti_Dettagli(j).Tariffa_Cod,
                                                idAttivita:=Agenda.Movimenti(i).Movimenti_Dettagli(j).ID_Attivita)

                            xmlDoc2.LoadXml(strMovimentoDettaglio)
                            xmlMovimentoDettaglio = xmlDoc2.SelectSingleNode("Movimento_Dettaglio")

                            strDatiMovimentiDettagli &= xmlDoc2.OuterXml

                        Next

                        xmlDatiMovimentiDettagli.InnerXml = strDatiMovimentiDettagli

                End Select

            Next

            xmlDoc.AppendChild(xmlDatiAgenda)

            Return xmlDoc.OuterXml

        End Function

    End Class

    Public Class Opzioni_Lettura_Agenda

        Public Property LeggiGps As Boolean

        Public Property LeggiNote As Boolean

        Public Property LeggiMovimenti As Boolean

        Public Property LeggiRiferimenti As Boolean

        Public Property OpzioniLetturaMovimenti As Opzioni_Lettura_Movimenti

        Sub New()
            LeggiGps = True
            LeggiNote = True
            LeggiMovimenti = True
            LeggiRiferimenti = True
            OpzioniLetturaMovimenti = New Opzioni_Lettura_Movimenti
        End Sub

    End Class

End Namespace
