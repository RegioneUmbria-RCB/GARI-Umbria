Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Movimento_Dettaglio

        Sub New(ByVal pivaInput As String, ByVal lavCod As Integer, ByVal dataOperazione As Date)
            Piva = pivaInput
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Lav_Cod = lavCod
            Cau_Mov = ""
            Data = dataOperazione
            Elem_Cod = 0
            Pro_Cod = 0
            Mat_Cod = 0
            Udm_Cod = 0
            Qta = 0
            Extra_Int = 0
            Extra_Str = ""
            Jolly_Int = 0
            Contabilizzato = 0
            Pendente = 0

            Mov_det_des = ""

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = AgroDataInizializzata
            Data_Modifica = AgroDataInizializzata
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            Anno = 0
            Extra_Date = AGRODATAINIZIO

            Lotto = ""

            TempoCarenza = 0
            DoseEtichetta = ""
            DoseEtichetta_Value = ""

            PrincipiAttivi = ""
            PrincipiAttiviPesi = ""
            CLassiTossicologiche = ""
            Buffer = ""

            Cod_Progetto = 0
            Cal_Cod = 0

            Turno_Cod = 0
            ID_Attivita = 0
            Veg_Cod = 0
            Prezzo_Unitario = 0
            Prezzo_Unitario_Netto = 0

            Fase_Cod = 0
            Udm_Cod_Extra = 0
            Qta_Extra = 0
            Prezzo_Effettivo = 0
            Qta_Extra_Totale = 0
            Tara = 0
            Sconto = 0
            Imponibile = 0
            Imponibile_Netto = 0
            Cod_Iva = 0
            Iva = 0
            Ric_Cod = 0
            Cod_Conto = 0
            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE

            Qta_Dettaglio1 = 0
            Qta_Dettaglio2 = 0

            Sconto_Listino = 0
            Sconto_Modalita = 0
            Sconto_Testo = ""

            ChkIva_Manuale = 0
            Cod_IvaIndetraibile = 0
            ChkLayOut_Hide = 0
            Variazione = 0
            Listino_Cod = 0
            Mezzo_Det = - 1
            Ric_Cod_Pat = 0
            Cod_Conto_Pat = 0
            Iva_Indetraibile = 0
            Iva_Indetraibile_Perc = 0
            Iva_Deto_Cod = 0
            Dettagli_Blocco_Flag = 0
            Dettagli_Blocco_Username = "0"
            Dettagli_Blocco_Data = AGRODATAINIZIO
            Ordine_Det = 0
            Deroga_Cod = 0
            Prezzo_Livello = 0

            Qualifica_Cod = 0
            Tariffa_Cod = 0

            Rif_Esterno = ""
            Rif_Esterno_2 = ""

            PrincipiAttiviPercAbb = ""

            Polverulento = 0
            Id_Cod = 0
            Gen_Cod = 0
            Spe_Cod = 0
            IPro_Cod = 0


            Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
            Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra)
            Movimenti_Dettagli_Conferimento = New List(Of Movimento_Dettaglio_Conferimento)
            Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
            Movimenti_Dettagli_Riferiti = New List(Of Movimento_Dettaglio)
            Movimenti_Dettagli_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)
            Materie_Prime_Campionature = New List(Of Materia_Prima_Campionatura)

        End Sub

        Sub New()

            Piva = ""
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Lav_Cod = 0
            Cau_Mov = ""
            Data = AGRODATAINIZIO
            Elem_Cod = 0
            Pro_Cod = 0
            Mat_Cod = 0
            Udm_Cod = 0
            Qta = 0
            Extra_Int = 0
            Extra_Str = ""
            Jolly_Int = 0
            Contabilizzato = 0
            Pendente = 0

            Mov_det_des = ""

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = AgroDataInizializzata
            Data_Modifica = AgroDataInizializzata
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            Anno = 0
            Extra_Date = AGRODATAINIZIO

            Lotto = ""

            TempoCarenza = 0
            DoseEtichetta = ""
            DoseEtichetta_Value = ""

            PrincipiAttivi = ""
            PrincipiAttiviPesi = ""
            CLassiTossicologiche = ""
            Buffer = ""

            Cod_Progetto = 0
            Cal_Cod = 0

            Turno_Cod = 0
            ID_Attivita = 0
            Veg_Cod = 0
            Prezzo_Unitario = 0
            Prezzo_Unitario_Netto = 0

            Fase_Cod = 0
            Udm_Cod_Extra = 0
            Qta_Extra = 0
            Prezzo_Effettivo = 0
            Qta_Extra_Totale = 0
            Tara = 0
            Sconto = 0
            Imponibile = 0
            Imponibile_Netto = 0
            Cod_Iva = 0
            Iva = 0
            Ric_Cod = 0
            Cod_Conto = 0
            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE

            Qta_Dettaglio1 = 0
            Qta_Dettaglio2 = 0

            Sconto_Listino = 0
            Sconto_Modalita = 0
            Sconto_Testo = ""

            ChkIva_Manuale = 0
            Cod_IvaIndetraibile = 0
            ChkLayOut_Hide = 0
            Variazione = 0
            Listino_Cod = 0
            Mezzo_Det = -1
            Ric_Cod_Pat = 0
            Cod_Conto_Pat = 0
            Iva_Indetraibile = 0
            Iva_Indetraibile_Perc = 0
            Iva_Deto_Cod = 0
            Dettagli_Blocco_Flag = 0
            Dettagli_Blocco_Username = "0"
            Dettagli_Blocco_Data = AGRODATAINIZIO
            Ordine_Det = 0
            Deroga_Cod = 0
            Prezzo_Livello = 0
            Qualifica_Cod = 0
            Tariffa_Cod = 0

            Rif_Esterno = ""
            Rif_Esterno_2 = ""

            PrincipiAttiviPercAbb = ""

            Polverulento = 0

            Id_Cod = 0
            Gen_Cod = 0
            Spe_Cod = 0
            IPro_Cod = 0

            Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
            Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra)
            Movimenti_Dettagli_Conferimento = New List(Of Movimento_Dettaglio_Conferimento)
            Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
            Movimenti_Dettagli_Riferiti = New List(Of Movimento_Dettaglio)
            Movimenti_Dettagli_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)
            Materie_Prime_Campionature = New List(Of Materia_Prima_Campionatura)

        End Sub


        Public Property Lav_Cod As Integer
        Public Property Cau_Mov As String

        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Id_Agenda As Integer
        Public Property Id_Mov As Integer
        Public Property Id_Mov_Det As Integer

        Public Property Data As Date

        Public Property Elem_Cod As Integer

        Public Property Pro_Cod As Integer

        Public Property Mat_Cod As Integer

        Public Property Mov_Det_Des As String

        Public Property Udm_Cod As Integer

        Public Property Qta As Decimal

        Public Property Cod_Iva As Integer

        Public Property Sconto As Decimal

        Public Property Prezzo_Unitario As Decimal

        Public Property Cod_Conto As Integer

        Public Property Cod_Progetto As Integer

        Public Property Fase_Cod As Integer

        Public Property Contabilizzato As Integer

        Public Property Pendente As Integer

        Public Property Validita_Inizio As Date

        Public Property Validita_Fine As Date

        Public Property Cal_Cod As Integer

        Public Property Extra_Str As String

        Public Property Extra_Int As Integer

        Public Property Extra_Date As Date

        Public Property Anno As Integer

        Public Property Ric_Cod As Integer

        Public Property Imponibile As Decimal

        Public Property Iva As Decimal

        Public Property Lotto As String

        Public Property Jolly_Int As Integer

        Public Property Imponibile_Netto As Decimal

        Public Property Prezzo_Unitario_Netto As Decimal

        Public Property Udm_Cod_Extra As Integer

        Public Property Qta_Extra As Decimal

        Public Property Prezzo_Effettivo As Decimal

        Public Property ChkIva_Manuale As Integer

        Public Property Cod_IvaIndetraibile As Integer

        Public Property Qta_Extra_Totale As Decimal

        Public Property Tara As Decimal

        Public Property ChkLayOut_Hide As Integer

        Public Property Variazione As Decimal

        Public Property Listino_Cod As Integer

        Public Property Sconto_Listino As Decimal

        Public Property Sconto_Modalita As Integer

        Public Property Mat_Cod_Alias as Integer

        Public Property Mezzo_Det As Integer

        Public Property Sconto_Testo As String

        Public Property Ric_Cod_Pat As Integer

        Public Property Cod_Conto_Pat As Integer

        Public Property TempoCarenza As Integer

        Public Property DoseEtichetta As String

        Public Property Turno_Cod As Integer

        Public Property ID_Attivita As Integer

        Public Property Veg_Cod As Integer

        Public Property Iva_Indetraibile As Decimal

        Public Property Iva_Indetraibile_Perc As Decimal

        Public Property PrincipiAttivi As String
        Public Property PrincipiAttiviPesi As String
        Public Property Buffer As String

        Public Property CLassiTossicologiche As String

        Public Property DoseEtichetta_Value As String

        Public Property Iva_Deto_Cod As Integer

        Public Property Qta_Dettaglio1 As Decimal

        Public Property Qta_Dettaglio2 As Decimal

        Public Property Dettagli_Blocco_Flag As Integer

        Public Property Dettagli_Blocco_Username As String

        Public Property Dettagli_Blocco_Data() As DateTime

        Public Property Qualifica_Cod As Integer

        Public Property Tariffa_Cod As Integer

        Public Property Ordine_Det As Integer

        Public Property Deroga_Cod As Integer

        Public Property Prezzo_Livello As Integer

        Public Property Rif_Esterno As String
        Public Property Rif_Esterno_2 As String

        Public Property Importo As Decimal

        Public Property Data_Creazione As DateTime

        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property PrincipiAttiviPercAbb As String

        Public Property Movimenti_Dettagli_Tecnici As List(Of Movimento_Dettaglio_Tecnico)

        Public Property Movimenti_Dettagli_Tecnici_Extra As List(Of Movimento_Dettaglio_Tecnico_Extra)

        Public Property Movimenti_Dettagli_Conferimento As List(Of Movimento_Dettaglio_Conferimento)

        Public Property Movimenti_Destinazioni As List(Of Movimento_Destinazione)

        Public Property Movimenti_Dettagli_Riferiti As List(Of Movimento_Dettaglio)

        Public Property Movimenti_Dettagli_Riferimenti As List(Of Movimento_Dettaglio_Riferimento)

        Public Property Materie_Prime_Campionature As List(Of Materia_Prima_Campionatura)

        Public Property Polverulento As Integer

        Public Property Id_Cod As Integer
        Public Property Gen_Cod As Integer
        Public Property Spe_Cod As Integer
        Public Property IPro_Cod As Integer

        Public ReadOnly Property IsDettaglioIrrigazione As Boolean
            Get
                Return Elem_Cod = CostantiPersonalizzate.ALTRE_MATERIE AndAlso Mat_Cod = CostantiPersonalizzate.MAT_COD_ACQUA_IRRIGAZIONE
            End Get
        End Property

        'Property XML_Raccolto_Campionatura As String
        '    Get
        '        Return _XML_Raccolto_Campionatura
        '    End Get
        '    Set(value As String)
        '        _XML_Raccolto_Campionatura = value
        '    End Set
        'End Property
    End Class

    Public Class Agenda_Movimenti_Dettagli_Helper

        Public Function Scrivi(ByVal Movimento_Dettaglio As Movimento_Dettaglio,
                               ByVal objParametri As AgronicaCoreParametri,
                               Optional ByVal flagScriviSempreLotto As Boolean = False,
                               Optional ByVal documentoPrevisionale As Boolean = False,
                               Optional ByRef Progressivo As Integer = 0,
                               Optional ByVal flagUtilizzaDataModifica As Boolean = False
                               ) As Integer

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim idMovDet As Integer
            Dim i As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'MATERIE_PRIME_CAMPIONATURE 

                If Not IsNothing(Movimento_Dettaglio.Materie_Prime_Campionature) AndAlso Movimento_Dettaglio.Materie_Prime_Campionature.Count > 0 Then

                    AggiornaMateriePrimeCampionature(Progressivo, Movimento_Dettaglio, objParametri)

                End If

                '---------------------------
                'Movimenti_Dettagli
                '---------------------------

                idMovDet = Movimento_Dettaglio.Id_Mov_Det

                If idMovDet <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    idMovDet = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                           Movimento_Dettaglio.BaseCode,
                                                           Movimento_Dettaglio.TopCode,
                                                           objParametri)

                    objSequenze = Nothing

                End If

                Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                '(21/10/2022) Nuovo allineamento a vecchia agenda e LAN, queste non utilizzano più il lotto "indefinito",
                '             quindi scrivo il campo così come già valorizzato su Movimento_Dettaglio
                '(09/02/2016) allineamento a vecchia agenda e LAN
                '=================================================================================================
                'GESTIONE CATEGORIE SENSIBILI
                '-------------------------------------------------------------------------------------------------
                'Select Case CInt(Movimento_Dettaglio.Elem_Cod)

                '    Case ALTRE_MATERIE, SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                '         SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                '         MANGIMI, SEMENTI,
                '         CAT_MAG_SERVIZI_PROFESSIONALI
                '        'Altre Materie Prime Aziendali, Semilavorati, Materie Prime, Beni Confezionamento, Mangimi, Trasformati, Sementi e Materiali Vivaisti

                '        If Movimento_Dettaglio.Lotto = "" Then
                '            Movimento_Dettaglio.Lotto = "Indefinito" '-->Impostazione Forzata Lotto
                '        End If

                '    Case FORMULATI

                '        'Considero anche i formulati come categoria sensibile, quindi se ho valorizzato un lotto, lo scrivo, altrimenti Indefinito
                '        If flagScriviSempreLotto = True Then

                '            If Movimento_Dettaglio.Lotto = "" Then
                '                Movimento_Dettaglio.Lotto = "Indefinito" '-->Impostazione Forzata Lotto
                '            End If

                '        Else
                '            Movimento_Dettaglio.Lotto = ""  '--> Lotto Non Valorizzato
                '        End If

                '    Case Else 'Categoria Non Sensibile

                '        Movimento_Dettaglio.Lotto = ""  '--> Lotto Non Valorizzato

                'End Select

                'Per evitare incompatibilità tra Lan e online, quando i documenti sono creati dall'interfaccia del Doc Contabile
                'Viene bypassata la scrittura di Indefinito (se da UI era stata lasciato vuoto, è stata cambiato con "DocContabile")
                'Lo devo fare fuori dal case, perché da UI viene messo "DocContabile" a prescindere dalla categoria,
                'quindi lo devo segar via a prescindere dalla categoria  
                'If Movimento_Dettaglio.Lotto = "DocContabile" Then
                '    Movimento_Dettaglio.Lotto = ""
                'End If
                '=================================================================================================

                Dim contabilizzato As Integer = Math.Abs(CInt(Movimento_Dettaglio.Contabilizzato))
                If CDate(Movimento_Dettaglio.Data) > CDate(Now) AndAlso documentoPrevisionale = False Then
                    contabilizzato = -Math.Abs(CInt(Movimento_Dettaglio.Contabilizzato))
                End If

                'Utilizzo data modifica solo se impostato apposito flag
                Dim dataModifica As DateTime
                If flagUtilizzaDataModifica Then
                    dataModifica = Movimento_Dettaglio.Data_Modifica
                Else
                    dataModifica = AgroDataInizializzata
                End If

                objMovimentiDettagli.Scrivi(Movimento_Dettaglio.Piva,
                                            Movimento_Dettaglio.Sa_Cod,
                                            Movimento_Dettaglio.Id_Agenda,
                                            Movimento_Dettaglio.Id_Mov,
                                            idMovDet,
                                            Movimento_Dettaglio.Elem_Cod,
                                            Movimento_Dettaglio.Pro_Cod,
                                            Movimento_Dettaglio.Mat_Cod,
                                            Movimento_Dettaglio.Mov_Det_Des,
                                            Movimento_Dettaglio.Qta,
                                            Movimento_Dettaglio.Udm_Cod,
                                            Movimento_Dettaglio.Cod_Iva,
                                            Movimento_Dettaglio.Jolly_Int,
                                            Movimento_Dettaglio.Sconto,
                                            Movimento_Dettaglio.Prezzo_Unitario,
                                            Movimento_Dettaglio.Prezzo_Unitario_Netto,
                                            Movimento_Dettaglio.Cod_Conto,
                                            Movimento_Dettaglio.Cal_Cod,
                                            Movimento_Dettaglio.Cod_Progetto,
                                            Movimento_Dettaglio.Fase_Cod,
                                            Movimento_Dettaglio.Extra_Str,
                                            Movimento_Dettaglio.Extra_Int,
                                            Movimento_Dettaglio.Extra_Date,
                                            Movimento_Dettaglio.Ric_Cod,
                                            Movimento_Dettaglio.Anno,
                                            Movimento_Dettaglio.Imponibile,
                                            Movimento_Dettaglio.Imponibile_Netto,
                                            Movimento_Dettaglio.Iva,
                                            Movimento_Dettaglio.Listino_Cod,
                                            contabilizzato,
                                            Movimento_Dettaglio.Pendente,
                                            Movimento_Dettaglio.Lotto,
                                            Movimento_Dettaglio.Udm_Cod_Extra,
                                            Movimento_Dettaglio.Qta_Extra,
                                            Movimento_Dettaglio.Qta_Extra_Totale,
                                            Movimento_Dettaglio.Prezzo_Effettivo,
                                            Movimento_Dettaglio.Variazione,
                                            Movimento_Dettaglio.Tara,
                                            Movimento_Dettaglio.ChkLayOut_Hide,
                                            Movimento_Dettaglio.ChkIva_Manuale,
                                            Movimento_Dettaglio.Cod_IvaIndetraibile,
                                            Movimento_Dettaglio.TempoCarenza,
                                            Movimento_Dettaglio.DoseEtichetta,
                                            Movimento_Dettaglio.Turno_Cod,
                                            Movimento_Dettaglio.ID_Attivita,
                                            Movimento_Dettaglio.Veg_Cod,
                                            Movimento_Dettaglio.PrincipiAttivi,
                                            Movimento_Dettaglio.CLassiTossicologiche,
                                            Movimento_Dettaglio.DoseEtichetta_Value,
                                            Movimento_Dettaglio.Validita_Inizio,
                                            Movimento_Dettaglio.Validita_Fine,
                                            objParametri,
                                            Mat_Cod_alias:=Movimento_Dettaglio.Mat_Cod_Alias,
                                            Qta_Dettaglio1:=Movimento_Dettaglio.Qta_Dettaglio1,
                                            Qta_Dettaglio2:=Movimento_Dettaglio.Qta_Dettaglio2,
                                            Sconto_Listino:=Movimento_Dettaglio.Sconto_Listino,
                                            Sconto_Modalita:=Movimento_Dettaglio.Sconto_Modalita,
                                            Sconto_Testo:=Movimento_Dettaglio.Sconto_Testo,
                                            Qualifica_Cod:=Movimento_Dettaglio.Qualifica_Cod,
                                            Tariffa_Cod:=Movimento_Dettaglio.Tariffa_Cod,
                                            Mezzo_Det:=Movimento_Dettaglio.Mezzo_Det,
                                            Ric_cod_Pat:=Movimento_Dettaglio.Ric_Cod_Pat,
                                            Cod_Conto_Pat:=Movimento_Dettaglio.Cod_Conto_Pat,
                                            Iva_Indetraibile:=Movimento_Dettaglio.Iva_Indetraibile,
                                            Iva_Indetraibile_Perc:=Movimento_Dettaglio.Iva_Indetraibile_Perc,
                                            Iva_Deto_Cod:=Movimento_Dettaglio.Iva_Deto_Cod,
                                            Dettagli_Blocco_Flag:=Movimento_Dettaglio.Dettagli_Blocco_Flag,
                                            Dettagli_Blocco_Username:=Movimento_Dettaglio.Dettagli_Blocco_Username,
                                            Dettagli_Blocco_Data:=Movimento_Dettaglio.Dettagli_Blocco_Data,
                                            Ordine_Det:=Movimento_Dettaglio.Ordine_Det,
                                            Deroga_Cod:=Movimento_Dettaglio.Deroga_Cod,
                                            Prezzo_Livello:=Movimento_Dettaglio.Prezzo_Livello,
                                            Data_creazione:=Movimento_Dettaglio.Data_Creazione,
                                            username_creazione:=Movimento_Dettaglio.Username_Creazione,
                                            PrincipiAttiviPesi:=Movimento_Dettaglio.PrincipiAttiviPesi,
                                            Buffer:=Movimento_Dettaglio.Buffer,
                                            Rif_Esterno:=Movimento_Dettaglio.Rif_Esterno,
                                            Rif_Esterno_2:=Movimento_Dettaglio.Rif_Esterno_2,
                                            Data_modifica:=dataModifica,
                                            PrincipiAttiviPercAbb:=Movimento_Dettaglio.PrincipiAttiviPercAbb,
                                            Polverulento:=Movimento_Dettaglio.Polverulento,
                                            Dettaglio_IdCod:=Movimento_Dettaglio.Id_Cod,
                                            Dettaglio_GenCod:=Movimento_Dettaglio.Gen_Cod,
                                            Dettaglio_SpeCod:=Movimento_Dettaglio.Spe_Cod,
                                            Dettaglio_IProCod:=Movimento_Dettaglio.IPro_Cod
                                            )

                objMovimentiDettagli = Nothing

                '---------------------------
                'MOVIMENTI_DETTAGLI per i RIFERIMENTI della tabella [Mov_Dettagli_Riferimenti]

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Riferiti) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Riferiti.Count > 0 Then

                    For i = 0 To Movimento_Dettaglio.Movimenti_Dettagli_Riferiti.Count - 1

                        Dim objAg As New AgronicaCoreContabDAL.Agenda_R
                        Dim objMv As New AgronicaCoreContabDAL.Movimenti_R

                        Dim lavCodOp As Integer = objAg.Lav_Cod_From_Piva_Sa_Cod_Id_Agenda(Movimento_Dettaglio.Piva,
                                                                                           Movimento_Dettaglio.Sa_Cod,
                                                                                           Movimento_Dettaglio.Id_Agenda,
                                                                                           objParametri)

                        Dim lavCodOpRif As Integer = objAg.Lav_Cod_From_Piva_Sa_Cod_Id_Agenda(Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Piva,
                                                                                              Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Sa_Cod,
                                                                                              Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Id_Agenda,
                                                                                              objParametri)

                        Dim cauMovOp As String = objMv.Cau_Mov_From_Piva_Sa_Cod_Id_Agenda_Id_Mov(Movimento_Dettaglio.Piva,
                                                                                                 Movimento_Dettaglio.Sa_Cod,
                                                                                                 Movimento_Dettaglio.Id_Agenda,
                                                                                                 Movimento_Dettaglio.Id_Mov,
                                                                                                 objParametri)

                        Dim cauMovOpRif As String = objMv.Cau_Mov_From_Piva_Sa_Cod_Id_Agenda_Id_Mov(Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Piva,
                                                                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Sa_Cod,
                                                                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Id_Agenda,
                                                                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Id_Mov,
                                                                                                    objParametri)

                        'attenzione, per la semina quando allego ddt il caumovrif non è quello del movimento del movdettagliorif da
                        'collegare, che è 7300, ma quello del movimento 4000
                        If (lavCodOpRif = LAVCOD_BOLLA_RICEVUTA Or lavCodOpRif = LAVCOD_FATTURA_RICEVUTA) Then
                            cauMovOpRif = CAU_REGISTRAZIONI
                        End If

                        Dim flagInsert As Boolean = False
                        Dim objRifW As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                        ' il sa_cod è 0 di solito per le bolle
                        flagInsert = objRifW.Scrivi(Movimento_Dettaglio.Piva,
                                                    Movimento_Dettaglio.Sa_Cod,
                                                    Movimento_Dettaglio.Id_Agenda,
                                                    Movimento_Dettaglio.Id_Mov,
                                                    idMovDet,
                                                    lavCodOp,
                                                    cauMovOp,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Piva,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Sa_Cod,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Id_Agenda,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Id_Mov,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Id_Mov_Det,
                                                    lavCodOpRif,
                                                    cauMovOpRif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferiti(i).Qta,
                                                    Movimento_Dettaglio.Data,
                                                    AGRODATAFINE,
                                                    objParametri)


                        If flagInsert = False Then
                            Throw New Exception("Errore nella creazione del movimento riferito in [Mov_Dettagli_Riferimenti]")
                        End If
                    Next

                End If


                '---------------------------
                'Movimenti_Dettagli_Riferimenti per i RIFERIMENTI della tabella [Mov_Dettagli_Riferimenti]

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Count > 0 Then

                    For i = 0 To Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Count - 1

                        Dim flagInsert As Boolean = False
                        Dim objRifW As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                        ' il sa_cod è 0 di solito per le bolle
                        flagInsert = objRifW.Scrivi(Movimento_Dettaglio.Piva,
                                                    Movimento_Dettaglio.Sa_Cod,
                                                    Movimento_Dettaglio.Id_Agenda,
                                                    Movimento_Dettaglio.Id_Mov,
                                                    idMovDet,
                                                    Movimento_Dettaglio.Lav_Cod,
                                                    Movimento_Dettaglio.Cau_Mov,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Piva_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Sa_Cod_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Id_Agenda_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Id_Mov_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Id_Mov_Det_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Lav_Cod_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Cau_Mov_Rif,
                                                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Qta,
                                                    Movimento_Dettaglio.Data,
                                                    AGRODATAFINE,
                                                    objParametri,
                                                    Preserva_Legame:=Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Preserva_Legame,
                                                    Tipo_Associazione:=Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(i).Tipo_Associazione)


                        If flagInsert = False Then
                            Throw New Exception("Errore nella creazione del movimento riferito in [Mov_Dettagli_Riferimenti]")
                        End If
                    Next

                End If



                '---------------------------
                'MOVIMENTI_DETTAGLI_TECNICI 

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Tecnici) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then

                    Dim objMovDettTecn As New Agenda_Movimenti_Dettagli_Tecnici_Helper
                    For i = 0 To Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Count - 1

                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(i).Id_Agenda = Movimento_Dettaglio.Id_Agenda
                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(i).Id_Mov = Movimento_Dettaglio.Id_Mov
                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(i).Id_Mov_Det = idMovDet

                        objMovDettTecn.Scrivi(Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(i), objParametri)
                    Next
                    objMovDettTecn = Nothing

                End If


                '---------------------------
                'MOVIMENTI_DETTAGLI_tecnici_extra 

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra.Count > 0 Then

                    Dim objMovDettTecn As New Agenda_Movimenti_Dettagli_Tecnici_extra_Helper
                    For i = 0 To Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra.Count - 1

                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra(i).Id_Agenda = Movimento_Dettaglio.Id_Agenda
                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra(i).Id_Mov = Movimento_Dettaglio.Id_Mov
                        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra(i).Id_Mov_Det = idMovDet

                        objMovDettTecn.Scrivi(Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra(i), objParametri)
                    Next
                    objMovDettTecn = Nothing

                End If


                '---------------------------
                'MOVIMENTI_DETTAGLI_CONFERIMENTO 

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Conferimento) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Conferimento.Count > 0 Then

                    Dim objMovDettConf As New Agenda_Movimenti_Dettagli_Conferimento_Helper
                    For i = 0 To Movimento_Dettaglio.Movimenti_Dettagli_Conferimento.Count - 1

                        Movimento_Dettaglio.Movimenti_Dettagli_Conferimento(i).Id_Agenda = Movimento_Dettaglio.Id_Agenda
                        Movimento_Dettaglio.Movimenti_Dettagli_Conferimento(i).Id_Mov = Movimento_Dettaglio.Id_Mov
                        Movimento_Dettaglio.Movimenti_Dettagli_Conferimento(i).Id_Mov_Det = idMovDet

                        objMovDettConf.Scrivi(Movimento_Dettaglio.Movimenti_Dettagli_Conferimento(i), objParametri)
                    Next
                    objMovDettConf = Nothing

                End If

                '---------------------------
                'MOVIMENTI_DESTINAZIONI
                'Dim tipoDestinazione As Integer
                'Dim destinazionePiva As String = ""
                'Dim destinazioneSaCod As Integer
                'Dim destinazioneAppezza As Integer
                'Dim destinazioneIdDestinazione As Integer

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Destinazioni) AndAlso Movimento_Dettaglio.Movimenti_Destinazioni.Count > 0 Then

                    'Se sono dentro un movimento di scarico
                    'prendo i dati dell'unica destinazione ( = magazzino)
                    'mi servono poi sotto per aggiornare la giacenza
                    'Select Case Movimento_Dettaglio.Cau_Mov
                    '    'Carico, Scarico, Conferimento, Conferimento a diversi, Accettazione, Accettazione da Diversi
                    '    Case CAU_CARICO, CAU_SCARICO,
                    '        CAU_CONFERIMENTO, CAU_CONFERIMENTO_DIVERSI,
                    '        CAU_ACCETTAZIONE_BENI, CAU_ACCETTAZIONE_BENI_DA_DIVERSI

                    '        tipoDestinazione = Movimento_Dettaglio.Movimenti_Destinazioni(0).Tipo
                    '        destinazionePiva = Movimento_Dettaglio.Movimenti_Destinazioni(0).Piva
                    '        destinazioneSaCod = Movimento_Dettaglio.Movimenti_Destinazioni(0).Sa_Cod
                    '        destinazioneAppezza = Movimento_Dettaglio.Movimenti_Destinazioni(0).Appezza
                    '        destinazioneIdDestinazione = Movimento_Dettaglio.Movimenti_Destinazioni(0).Id_Destinazione

                    'End Select

                    Dim objMovDest As New Agenda_Movimenti_Destinazioni_Helper
                    For i = 0 To Movimento_Dettaglio.Movimenti_Destinazioni.Count - 1

                        Movimento_Dettaglio.Movimenti_Destinazioni(i).Id_Agenda = Movimento_Dettaglio.Id_Agenda
                        Movimento_Dettaglio.Movimenti_Destinazioni(i).Id_Mov = Movimento_Dettaglio.Id_Mov
                        Movimento_Dettaglio.Movimenti_Destinazioni(i).Id_Mov_Det = idMovDet

                        objMovDest.Scrivi(Movimento_Dettaglio.Movimenti_Destinazioni(i), objParametri)
                    Next
                    objMovDest = Nothing

                End If

                ' Giulia: 14/8/2019: Il record statico ci giacenza non viene più usato!!!

                '##############################################################
                '#####  GIACENZE DI MAGAZZINO (QUANTITATIVA-QUALITATIVA)  #####
                '##############################################################

                'Se la Destinazione è 20 -> l'Operazione coinvolge le giacenze di magazzino

                'La gestione delle giacenze non deve essere movimentata se la connessione è locale
                'poiché in caso di importazione dati verrebbe movimentata 2 volte!

                'Le giacenze devono essere aggiornate se l'operazione non è pianificata

                'Il campo Jolly_Int se = 1 --> il movimento non riguarda il magazzino

                'Se CInt(xMovimento_Dettaglio.getAttribute("contabilizzato")) < 0 --> si sta cancellando una pianificazione

                '-----

                'If tipoDestinazione = TIPO_DESTINAZIONE_MAGAZZINO And
                '   Movimento_Dettaglio.Data <= CDate(Now) And
                '   Movimento_Dettaglio.Contabilizzato >= 0 Then

                '    'Verifico che la causale sia gestita dal modulo "Giacenze"
                '    Select Case Movimento_Dettaglio.Cau_Mov

                '        'Carico, Scarico, Conferimento, Conferimento a diversi, Accettazione, Accettazione da Diversi
                '        Case CAU_CARICO, CAU_SCARICO,
                '            CAU_CONFERIMENTO, CAU_CONFERIMENTO_DIVERSI,
                '            CAU_ACCETTAZIONE_BENI, CAU_ACCETTAZIONE_BENI_DA_DIVERSI

                '            'Causale Gestita
                '            Dim objGiacenze As New AgronicaCoreContabBIZ.Giacenze_W

                '            '===================================================================================
                '            'Setting dei Parametri quantitativi del movimento dettaglio
                '            '-----------------------------------------------------------------------------------
                '            'mBasecode = 0
                '            'mTopcode = 2000000000

                '            objGiacenze.Giacenza_Scrivi(destinazionePiva,
                '                                        destinazioneSaCod,
                '                                        Movimento_Dettaglio.Elem_Cod,
                '                                        Movimento_Dettaglio.Pro_Cod,
                '                                        Movimento_Dettaglio.Mat_Cod,
                '                                        Movimento_Dettaglio.Udm_Cod,
                '                                        destinazioneIdDestinazione,
                '                                        enum_TipoOperazioneDB.Scrittura.ToString,
                '                                        Movimento_Dettaglio.Cau_Mov,
                '                                        0,
                '                                        0,
                '                                        0,
                '                                        "",
                '                                        Movimento_Dettaglio.Qta,
                '                                        0,
                '                                        0,
                '                                        0,
                '                                        0,
                '                                        0,
                '                                        Movimento_Dettaglio.Lav_Cod,
                '                                        Movimento_Dettaglio.Jolly_Int,
                '                                        0,
                '                                        2000000000,
                '                                        Movimento_Dettaglio.Data,
                '                                        Movimento_Dettaglio.Data,
                '                                        objParametri)

                '            objGiacenze = Nothing

                '        Case Else 'Causale Non Gestita

                '            Throw New Exception(" Causale Non Gestita ")

                '    End Select

                'End If

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Movimenti_Dettagli_Helper.Scrivi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return idMovDet

        End Function

        Private Sub AggiornaMateriePrimeCampionature(ByRef Progressivo As Integer,
                                                     ByRef Movimento_Dettaglio As Movimento_Dettaglio,
                                                     ByRef objParametri As AgronicaCoreParametri)

            'devo creare qui un unico progressivo per tutti questi e poi andare anche a metterlo nel cal_cod
            Progressivo = Movimento_Dettaglio.Materie_Prime_Campionature(0).Progressivo

            If Progressivo = 0 Then

                Dim objSequenze As New Agro_Sequenze

                Progressivo = objSequenze.NuovoId_Tabella("Materie_Prime_Campionature",
                                                          Movimento_Dettaglio.Materie_Prime_Campionature(0).BaseCode,
                                                          Movimento_Dettaglio.Materie_Prime_Campionature(0).TopCode,
                                                          objParametri)

                Progressivo = -Math.Abs(Progressivo) 'valore negativo

                'questo progressivo lo fisso anche nel Cal_Cod
                If Movimento_Dettaglio.Cal_Cod = 0 Then
                    Movimento_Dettaglio.Cal_Cod = Progressivo
                End If

                objSequenze = Nothing

            End If

            For Each matPrimaCamp As Materia_Prima_Campionatura In Movimento_Dettaglio.Materie_Prime_Campionature

                matPrimaCamp.Progressivo = Progressivo

                Dim objMateriePrimeCamp As New Agenda_Materie_Prime_Campionature_Helper

                'Se mi è già arrivato un progressivo, non è detto che debba per forza scrivere il record,
                'potrebbe già esistere ed allora devo andare in update

                Dim listMat As New List(Of Materia_Prima_Campionatura)

                listMat = objMateriePrimeCamp.Leggi(matPrimaCamp.Progressivo,
                                                    matPrimaCamp.Tipo,
                                                    0,
                                                    matPrimaCamp.Udm_Cod,
                                                    objParametri)

                Dim elementoModificato As Boolean = False

                If Not listMat Is Nothing Then

                    For Each elemMat In listMat

                        If elemMat.Tipo_Cod = matPrimaCamp.Tipo_Cod Then

                            'Il record esiste già, quindi update
                            elementoModificato = True
                            objMateriePrimeCamp.Modifica(matPrimaCamp.Progressivo,
                                                         matPrimaCamp.Tipo,
                                                         matPrimaCamp.Tipo_Cod,
                                                         matPrimaCamp.Udm_Cod,
                                                         objParametri,
                                                         matPrimaCamp.Val_Cod,
                                                         matPrimaCamp.Descrizione,
                                                         matPrimaCamp.Validita_Inizio,
                                                         matPrimaCamp.Validita_Fine,
                                                         matPrimaCamp.Progressivo_Origine,
                                                         matPrimaCamp.Piva_SuperUser_Origine,
                                                         matPrimaCamp.Peso_Campione,
                                                         matPrimaCamp.ChkStima,
                                                         matPrimaCamp.ChkTara_Campionatura,
                                                         matPrimaCamp.Tara_Campionatura,
                                                         matPrimaCamp.Data_Modifica,
                                                         matPrimaCamp.Username_Modifica)
                        Else

                            'Il record non esiste più, quindi delete
                            objMateriePrimeCamp.Cancella(elemMat.Progressivo,
                                                         elemMat.Tipo,
                                                         elemMat.Tipo_Cod,
                                                         0,
                                                         False,
                                                         objParametri)

                        End If

                    Next

                End If

                If Not elementoModificato Then

                    'Il record non esiste, quindi scrivo
                    objMateriePrimeCamp.Scrivi(matPrimaCamp, objParametri)

                End If

                objMateriePrimeCamp = Nothing

            Next

        End Sub

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal idMov As Integer,
                                 ByVal idMovDet As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'MOVIMENTI_DETTAGLI_TECNICI 

                Dim objMovDettTecn As New Agenda_Movimenti_Dettagli_Tecnici_Helper
                objMovDettTecn.Cancella(piva, saCod, idAgenda, idMov, idMovDet, 0, objParametri)
                
                Dim objMovDettTecnExtra As New Agenda_Movimenti_Dettagli_Tecnici_extra_Helper
                objMovDettTecnExtra.Cancella(piva, 0, idAgenda, idMov, idMovDet, 0, objParametri)


                '---------------------------
                'MOVIMENTI_DETTAGLI_CONFERIMENTO 

                Dim objMovDettConf As New Agenda_Movimenti_Dettagli_Conferimento_Helper
                objMovDettConf.Cancella(piva, saCod, idAgenda, idMov, idMovDet, 0, objParametri)
                

                '---------------------------
                'MOVIMENTI_DESTINAZIONI

                Dim objMovDest As New Agenda_Movimenti_Destinazioni_Helper
                objMovDest.Cancella(piva, saCod, idAgenda, idMov, idMovDet, 0, 0, objParametri)


                If True Then

                    '---------------------------
                    'MOVIMENTI_DETTAGLI
                    '---------------------------
                    'prima di cancellare devo salvarmi il cal_cod
                    Dim objMovimentiDettagliR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                    Dim calCodTemp As Integer = objMovimentiDettagliR.Leggi_Cal_Cod(piva, saCod, idAgenda, idMov, idMovDet, "", objParametri)


                    Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                    objMovimentiDettagli.Cancella(piva,
                                                  saCod,
                                                  idAgenda,
                                                  idMov,
                                                  idMovDet,
                                                  "",
                                                  objParametri)

                    objMovimentiDettagli = Nothing


                    '---------------------------
                    'Materie_Prime_Campionature
                    '---------------------------
                     'Devo anche eliminare i record collegati in Materie_Prime_Campionature
                    '(solo se non le ho già usati in altri punti)
                    If calCodTemp <> 0 Then
                        Dim objMateriePrimeCamp As New Agenda_Materie_Prime_Campionature_Helper
                        objMateriePrimeCamp.Cancella(calCodTemp, "", 0, 0, True, objParametri)
                        objMateriePrimeCamp = Nothing
                    End If

                End If

                'per semina, cancello ddt collegati
                If True Then

                    '---------------------------
                    'Movimenti_Dettagli_riferimenti
                    '---------------------------

                    Dim objMovimentiDettagliRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                    objMovimentiDettagliRif.Cancella(piva,
                                                     saCod,
                                                     idAgenda,
                                                     idMov,
                                                     idMovDet,
                                                     "",
                                                     objParametri)

                End If


                If True Then

                    '---------------------------
                    'GHG_Registrazioni
                    '---------------------------

                    Dim objGHG_Registrazioni As New AgronicaCoreContabDAL.GHG_Registrazioni_W
                    objGHG_Registrazioni.Cancella(piva,
                                                  0,
                                                  idAgenda,
                                                  "",
                                                  objParametri)

                End If



                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception
                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Helper.cancella() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal idMov As Integer,
                                         ByVal idMovDet As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         Optional ByVal elemCod As Integer? = Nothing,
                                         Optional ByVal proCod As Integer? = Nothing,
                                         Optional ByVal matCod As Integer? = Nothing,
                                         Optional ByVal movDetDes As String = Nothing,
                                         Optional ByVal udmCod As Integer? = Nothing,
                                         Optional ByVal qta As Decimal? = Nothing,
                                         Optional ByVal codIva As Integer? = Nothing,
                                         Optional ByVal sconto As Decimal? = Nothing,
                                         Optional ByVal prezzoUnitario As Decimal? = Nothing,
                                         Optional ByVal codConto As Integer? = Nothing,
                                         Optional ByVal codProgetto As Integer? = Nothing,
                                         Optional ByVal faseCod As Integer? = Nothing,
                                         Optional ByVal contabilizzato As Integer? = Nothing,
                                         Optional ByVal pendente As Integer? = Nothing,
                                         Optional ByVal data As Date? = Nothing,
                                         Optional ByVal calCod As Integer? = Nothing,
                                         Optional ByVal extraStr As String = Nothing,
                                         Optional ByVal extraInt As Integer? = Nothing,
                                         Optional ByVal extraDate As DateTime? = Nothing,
                                         Optional ByVal anno As Integer? = Nothing,
                                         Optional ByVal ricCod As Integer? = Nothing,
                                         Optional ByVal imponibile As Decimal? = Nothing,
                                         Optional ByVal iva As Decimal? = Nothing,
                                         Optional ByVal lotto As String = Nothing,
                                         Optional ByVal jollyInt As Integer? = Nothing,
                                         Optional ByVal imponibileNetto As Decimal? = Nothing,
                                         Optional ByVal prezzoUnitarioNetto As Decimal? = Nothing,
                                         Optional ByVal udmCodExtra As Integer? = Nothing,
                                         Optional ByVal qtaExtra As Decimal? = Nothing,
                                         Optional ByVal prezzoEffettivo As Decimal? = Nothing,
                                         Optional ByVal chkIvaManuale As Integer? = Nothing,
                                         Optional ByVal codIvaIndetraibile As Integer? = Nothing,
                                         Optional ByVal qtaExtraTotale As Decimal? = Nothing,
                                         Optional ByVal tara As Decimal? = Nothing,
                                         Optional ByVal chkLayOutHide As Integer? = Nothing,
                                         Optional ByVal variazione As Decimal? = Nothing,
                                         Optional ByVal listinoCod As Integer? = Nothing,
                                         Optional ByVal scontoListino As Decimal? = Nothing,
                                         Optional ByVal scontoModalita As Integer? = Nothing,
                                         Optional ByVal matCodAlias As Integer? = Nothing,
                                         Optional ByVal mezzoDet As Integer? = Nothing,
                                         Optional ByVal scontoTesto As String = Nothing,
                                         Optional ByVal ricCodPat As Integer? = Nothing,
                                         Optional ByVal codContoPat As Integer? = Nothing,
                                         Optional ByVal tempoCarenza As Integer? = Nothing,
                                         Optional ByVal doseEtichetta As String = Nothing,
                                         Optional ByVal turnoCod As Integer? = Nothing,
                                         Optional ByVal idAttivita As Integer? = Nothing,
                                         Optional ByVal dettaglioVegCod As Integer? = Nothing,
                                         Optional ByVal ivaIndetraibile As Decimal? = Nothing,
                                         Optional ByVal ivaIndetraibilePerc As Decimal? = Nothing,
                                         Optional ByVal principiAttivi As String = Nothing,
                                         Optional ByVal classiTossicologiche As String = Nothing,
                                         Optional ByVal doseEtichettaValue As String = Nothing,
                                         Optional ByVal ivaDetoCod As Integer? = Nothing,
                                         Optional ByVal qtaDettaglio1 As Decimal? = Nothing,
                                         Optional ByVal qtaDettaglio2 As Decimal? = Nothing,
                                         Optional ByVal dettagliBloccoFlag As Integer? = Nothing,
                                         Optional ByVal dettagliBloccoUsername As String = Nothing,
                                         Optional ByVal dettagliBloccoData As DateTime? = Nothing,
                                         Optional ByVal qualificaCod As Integer? = Nothing,
                                         Optional ByVal tariffaCod As Integer? = Nothing,
                                         Optional ByVal ordineDet As Integer? = Nothing,
                                         Optional ByVal derogaCod As Integer? = Nothing,
                                         Optional ByVal prezzoLivello As Integer? = Nothing,
                                         Optional ByVal dataModifica As DateTime = AgroDataInizializzata,
                                         Optional ByVal usernameModifica As String = "",
                                         Optional ByVal principiAttiviPesi As String = Nothing,
                                         Optional ByVal buffer As String = Nothing,
                                         Optional ByVal rifEsterno As String = Nothing,
                                         Optional ByVal rifEsterno2 As String = Nothing,
                                         Optional ByVal principiAttiviPercAbb As String = Nothing,
                                         Optional ByVal polverulento As Integer? = Nothing,
                                         Optional ByVal dettaglioIdCod As Integer? = Nothing,
                                         Optional ByVal dettaglioGenCod As Integer? = Nothing,
                                         Optional ByVal dettaglioSpeCod As Integer? = Nothing,
                                         Optional ByVal dettaglioIProCod As Integer? = Nothing
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objMovDettW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                xRisp = objMovDettW.ModificaPuntuale(piva, saCod, idAgenda, idMov, idMovDet, objParametri,
                                                     Elem_Cod:=elemCod,
                                                     Pro_Cod:=proCod,
                                                     Mat_Cod:=matCod,
                                                     Mov_Det_Des:=movDetDes,
                                                     Udm_Cod:=udmCod,
                                                     Qta:=qta,
                                                     Cod_Iva:=codIva,
                                                     Sconto:=sconto,
                                                     Prezzo_Unitario:=prezzoUnitario,
                                                     Cod_Conto:=codConto,
                                                     Cod_Progetto:=codProgetto,
                                                     Fase_Cod:=faseCod,
                                                     Contabilizzato:=contabilizzato,
                                                     Pendente:=pendente,
                                                     Validita_Inizio:=data,
                                                     Cal_Cod:=calCod,
                                                     Extra_Str:=extraStr,
                                                     Extra_Int:=extraInt,
                                                     Extra_Date:=extraDate,
                                                     Anno:=anno,
                                                     Ric_Cod:=ricCod,
                                                     Imponibile:=imponibile,
                                                     Iva:=iva,
                                                     Lotto:=lotto,
                                                     Jolly_Int:=jollyInt,
                                                     Imponibile_Netto:=imponibileNetto,
                                                     Prezzo_Unitario_Netto:=prezzoUnitarioNetto,
                                                     Udm_Cod_Extra:=udmCodExtra,
                                                     Qta_Extra:=qtaExtra,
                                                     Prezzo_Effettivo:=prezzoEffettivo,
                                                     ChkIva_Manuale:=chkIvaManuale,
                                                     Cod_IvaIndetraibile:=codIvaIndetraibile,
                                                     Qta_Extra_Totale:=qtaExtraTotale,
                                                     Tara:=tara,
                                                     ChkLayOut_Hide:=chkLayOutHide,
                                                     Variazione:=variazione,
                                                     Listino_Cod:=listinoCod,
                                                     Sconto_Listino:=scontoListino,
                                                     Sconto_Modalita:=scontoModalita,
                                                     Mat_Cod_Alias:=matCodAlias,
                                                     Mezzo_Det:=mezzoDet,
                                                     Sconto_Testo:=scontoTesto,
                                                     Ric_Cod_Pat:=ricCodPat,
                                                     Cod_Conto_Pat:=codContoPat,
                                                     TempoCarenza:=tempoCarenza,
                                                     DoseEtichetta:=doseEtichetta,
                                                     Turno_Cod:=turnoCod,
                                                     Id_Attivita:=idAttivita,
                                                     Dettaglio_VegCod:=dettaglioVegCod,
                                                     Iva_Indetraibile:=ivaIndetraibile,
                                                     Iva_Indetraibile_Perc:=ivaIndetraibilePerc,
                                                     PrincipiAttivi:=principiAttivi,
                                                     ClassiTossicologiche:=classiTossicologiche,
                                                     DoseEtichetta_Value:=doseEtichettaValue,
                                                     Iva_Deto_Cod:=ivaDetoCod,
                                                     Qta_Dettaglio1:=qtaDettaglio1,
                                                     Qta_Dettaglio2:=qtaDettaglio2,
                                                     Dettagli_Blocco_Flag:=dettagliBloccoFlag,
                                                     Dettagli_Blocco_Username:=dettagliBloccoUsername,
                                                     Dettagli_Blocco_Data:=dettagliBloccoData,
                                                     Qualifica_Cod:=qualificaCod,
                                                     Tariffa_Cod:=tariffaCod,
                                                     Ordine_Det:=ordineDet,
                                                     Deroga_Cod:=derogaCod,
                                                     Prezzo_Livello:=prezzoLivello,
                                                     PrincipiAttiviPesi:=principiAttiviPesi,
                                                     Buffer:=buffer,
                                                     Rif_Esterno:=rifEsterno,
                                                     Rif_Esterno_2:=rifEsterno2,
                                                     PrincipiAttiviPercAbb:=principiAttiviPercAbb,
                                                     Data_Modifica:=dataModifica,
                                                     Username_Modifica:=usernameModifica,
                                                     Polverulento:=polverulento,
                                                     Dettaglio_IdCod:=dettaglioIdCod,
                                                     Dettaglio_GenCod:=dettaglioGenCod,
                                                     Dettaglio_SpeCod:=dettaglioSpeCod,
                                                     Dettaglio_IProCod:=dettaglioIProCod)

                objMovDettW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal idMov As Integer,
                                         ByVal idMovDet As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         ByVal Movimento_Dettaglio As Movimento_Dettaglio
                                         ) As Boolean
            
            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objMovDettW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                xRisp = objMovDettW.ModificaPuntuale(piva, saCod, idAgenda, idMov, idMovDet, objParametri,
                                                     Elem_Cod:=Movimento_Dettaglio.Elem_Cod,
                                                     Pro_Cod:=Movimento_Dettaglio.Pro_Cod,
                                                     Mat_Cod:=Movimento_Dettaglio.Mat_Cod,
                                                     Mov_Det_Des:=Movimento_Dettaglio.Mov_Det_Des,
                                                     Udm_Cod:=Movimento_Dettaglio.Udm_Cod,
                                                     Qta:=Movimento_Dettaglio.Qta,
                                                     Cod_Iva:=Movimento_Dettaglio.Cod_Iva,
                                                     Sconto:=Movimento_Dettaglio.Sconto,
                                                     Prezzo_Unitario:=Movimento_Dettaglio.Prezzo_Unitario,
                                                     Cod_Conto:=Movimento_Dettaglio.Cod_Conto,
                                                     Cod_Progetto:=Movimento_Dettaglio.Cod_Progetto,
                                                     Fase_Cod:=Movimento_Dettaglio.Fase_Cod,
                                                     Contabilizzato:=Movimento_Dettaglio.Contabilizzato,
                                                     Pendente:=Movimento_Dettaglio.Pendente,
                                                     Validita_Inizio:=Movimento_Dettaglio.Data,
                                                     Cal_Cod:=Movimento_Dettaglio.Cal_Cod,
                                                     Extra_Str:=Movimento_Dettaglio.Extra_Str,
                                                     Extra_Int:=Movimento_Dettaglio.Extra_Int,
                                                     Extra_Date:=Movimento_Dettaglio.Extra_Date,
                                                     Anno:=Movimento_Dettaglio.Anno,
                                                     Ric_Cod:=Movimento_Dettaglio.Ric_Cod,
                                                     Imponibile:=Movimento_Dettaglio.Imponibile,
                                                     Iva:=Movimento_Dettaglio.Iva,
                                                     Lotto:=Movimento_Dettaglio.Lotto,
                                                     Jolly_Int:=Movimento_Dettaglio.Jolly_Int,
                                                     Imponibile_Netto:=Movimento_Dettaglio.Imponibile_Netto,
                                                     Prezzo_Unitario_Netto:=Movimento_Dettaglio.Prezzo_Unitario_Netto,
                                                     Udm_Cod_Extra:=Movimento_Dettaglio.Udm_Cod_Extra,
                                                     Qta_Extra:=Movimento_Dettaglio.Qta_Extra,
                                                     Prezzo_Effettivo:=Movimento_Dettaglio.Prezzo_Effettivo,
                                                     ChkIva_Manuale:=Movimento_Dettaglio.ChkIva_Manuale,
                                                     Cod_IvaIndetraibile:=Movimento_Dettaglio.Cod_IvaIndetraibile,
                                                     Qta_Extra_Totale:=Movimento_Dettaglio.Qta_Extra_Totale,
                                                     Tara:=Movimento_Dettaglio.Tara,
                                                     ChkLayOut_Hide:=Movimento_Dettaglio.ChkLayOut_Hide,
                                                     Variazione:=Movimento_Dettaglio.Variazione,
                                                     Listino_Cod:=Movimento_Dettaglio.Listino_Cod,
                                                     Sconto_Listino:=Movimento_Dettaglio.Sconto_Listino,
                                                     Sconto_Modalita:=Movimento_Dettaglio.Sconto_Modalita,
                                                     Mat_Cod_Alias:=Movimento_Dettaglio.Mat_Cod_Alias,
                                                     Mezzo_Det:=Movimento_Dettaglio.Mezzo_Det,
                                                     Sconto_Testo:=Movimento_Dettaglio.Sconto_Testo,
                                                     Ric_Cod_Pat:=Movimento_Dettaglio.Ric_Cod_Pat,
                                                     Cod_Conto_Pat:=Movimento_Dettaglio.Cod_Conto_Pat,
                                                     TempoCarenza:=Movimento_Dettaglio.TempoCarenza,
                                                     DoseEtichetta:=Movimento_Dettaglio.DoseEtichetta,
                                                     Turno_Cod:=Movimento_Dettaglio.Turno_Cod,
                                                     Id_Attivita:=Movimento_Dettaglio.ID_Attivita,
                                                     Dettaglio_VegCod:=Movimento_Dettaglio.Veg_Cod,
                                                     Iva_Indetraibile:=Movimento_Dettaglio.Iva_Indetraibile,
                                                     Iva_Indetraibile_Perc:=Movimento_Dettaglio.Iva_Indetraibile_Perc,
                                                     PrincipiAttivi:=Movimento_Dettaglio.PrincipiAttivi,
                                                     ClassiTossicologiche:=Movimento_Dettaglio.CLassiTossicologiche,
                                                     DoseEtichetta_Value:=Movimento_Dettaglio.DoseEtichetta_Value,
                                                     Iva_Deto_Cod:=Movimento_Dettaglio.Iva_Deto_Cod,
                                                     Qta_Dettaglio1:=Movimento_Dettaglio.Qta_Dettaglio1,
                                                     Qta_Dettaglio2:=Movimento_Dettaglio.Qta_Dettaglio2,
                                                     Dettagli_Blocco_Flag:=Movimento_Dettaglio.Dettagli_Blocco_Flag,
                                                     Dettagli_Blocco_Username:=Movimento_Dettaglio.Dettagli_Blocco_Username,
                                                     Dettagli_Blocco_Data:=Movimento_Dettaglio.Dettagli_Blocco_Data,
                                                     Qualifica_Cod:=Movimento_Dettaglio.Qualifica_Cod,
                                                     Tariffa_Cod:=Movimento_Dettaglio.Tariffa_Cod,
                                                     Ordine_Det:=Movimento_Dettaglio.Ordine_Det,
                                                     Deroga_Cod:=Movimento_Dettaglio.Deroga_Cod,
                                                     Prezzo_Livello:=Movimento_Dettaglio.Prezzo_Livello,
                                                     PrincipiAttiviPesi:=Movimento_Dettaglio.PrincipiAttiviPesi,
                                                     Buffer:=Movimento_Dettaglio.Buffer,
                                                     Rif_Esterno:=Movimento_Dettaglio.Rif_Esterno,
                                                     Rif_Esterno_2:=Movimento_Dettaglio.Rif_Esterno_2,
                                                     PrincipiAttiviPercAbb:=Movimento_Dettaglio.PrincipiAttiviPercAbb,
                                                     Data_Modifica:=Movimento_Dettaglio.Data_Modifica,
                                                     Username_Modifica:=Movimento_Dettaglio.Username_Modifica,
                                                     Polverulento:=Movimento_Dettaglio.Polverulento,
                                                     Dettaglio_IdCod:=Movimento_Dettaglio.Id_Cod,
                                                     Dettaglio_GenCod:=Movimento_Dettaglio.Gen_Cod,
                                                     Dettaglio_SpeCod:=Movimento_Dettaglio.Spe_Cod,
                                                     Dettaglio_IProCod:=Movimento_Dettaglio.IPro_Cod)

                objMovDettW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal idMov As Integer,
                              ByVal idMovDet As Integer,
                              ByVal objParametri As AgronicaCoreParametri,
                              Optional ByVal leggiEsclusivamenteDettaglio As Boolean = False,
                              Optional ByVal filtroMovimentiDettagli As Integer = enum_FiltroMovimentiDettagli.Nessuno,
                              Optional ByVal filtroAggiuntivoRiferimenti As String = ""
                              ) As List(Of Movimento_Dettaglio)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDettagli As New List(Of Movimento_Dettaglio)
            Dim Movimento_Dettaglio As Movimento_Dettaglio

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DETTAGLI ----------------------------
                '--------------------------------------------------------
                Dim objMovDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim dtMovDettagli As DataTable
                Dim filtroAggiuntivo As String = ""

                Select Case filtroMovimentiDettagli
                    Case enum_FiltroMovimentiDettagli.DatiTestataLavorazione
                        filtroAggiuntivo = "MOVIMENTI_DETTAGLI.EXTRA_STR <> '' AND MOVIMENTI_DETTAGLI.EXTRA_INT > 0"
                End Select

                dtMovDettagli = objMovDettagli.Leggi(CStr(piva),
                                                     CInt(saCod),
                                                     CInt(idAgenda),
                                                     idMov,
                                                     idMovDet,
                                                     0,
                                                     0,
                                                     0,
                                                     "",
                                                     0, 0, 0, 0, 0, 0,
                                                     enumSelezioneVariabile.Selezione_JoinCompleta,
                                                     filtroAggiuntivo,
                                                     "",
                                                     objParametri)

                objMovDettagli = Nothing

                If dtMovDettagli.Rows.Count > 0 Then

                    For Each drMovDet In dtMovDettagli.Rows

                        Movimento_Dettaglio = New Movimento_Dettaglio With {
                            .Piva = drMovDet.Item("Piva"),
                            .Sa_Cod = drMovDet.Item("Sa_Cod"),
                            .Id_Agenda = drMovDet.Item("Id_Agenda"),
                            .Id_Mov = drMovDet.Item("Id_Mov"),
                            .Id_Mov_Det = drMovDet.Item("Id_Mov_Det"),
                            .Data = drMovDet.Item("Validita_Inizio"),
                            .Lav_Cod = drMovDet.Item("Lav_Cod"),
                            .Cau_Mov = drMovDet.Item("Cau_Mov"),
                            .Elem_Cod = drMovDet.Item("Elem_Cod"),
                            .Pro_Cod = drMovDet.Item("Pro_Cod"),
                            .Mat_Cod = drMovDet.Item("Mat_Cod"),
                            .Contabilizzato = drMovDet.Item("Contabilizzato"),
                            .Jolly_Int = drMovDet.Item("Jolly_Int"),
                            .Pendente = drMovDet.Item("Pendente"),
                            .Qta = drMovDet.Item("Qta"),
                            .Udm_Cod = drMovDet.Item("Udm_Cod"),
                            .Extra_Int = drMovDet.Item("Extra_Int"),
                            .Extra_Str = drMovDet.Item("Extra_Str"),
                            .Lotto = drMovDet.Item("Lotto"),
                            .Cod_Progetto = drMovDet.Item("Cod_Progetto"),
                            .Cal_Cod = drMovDet.Item("Cal_Cod"),
                            .Fase_Cod = drMovDet.Item("Fase_Cod"),
                            .Udm_Cod_Extra = drMovDet.Item("Udm_Cod_Extra"),
                            .Qta_Extra = drMovDet.Item("Qta_Extra"),
                            .Prezzo_Effettivo = drMovDet.Item("Prezzo_Effettivo"),
                            .Sconto = drMovDet.Item("Sconto"),
                            .Imponibile = drMovDet.Item("Imponibile"),
                            .Imponibile_Netto = drMovDet.Item("Imponibile_Netto"),
                            .Cod_Iva = drMovDet.Item("Cod_Iva"),
                            .Iva = drMovDet.Item("Iva"),
                            .Ric_Cod = drMovDet.Item("Ric_Cod"),
                            .Cod_Conto = drMovDet.Item("Cod_Conto"),
                            .Validita_Inizio = drMovDet.Item("Validita_Inizio"),
                            .Validita_Fine = drMovDet.Item("Validita_Fine"),
                            .Sconto_Listino = drMovDet.Item("Sconto_Listino"),
                            .Sconto_Modalita = drMovDet.Item("Sconto_Modalita"),
                            .Sconto_Testo = drMovDet.Item("Sconto_Testo"),
                            .Data_Creazione = CDate(drMovDet.Item("Data_Creazione")),
                            .Data_Modifica = CDate(drMovDet.Item("Data_Modifica")),
                            .Username_Creazione = drMovDet.Item("Username_Creazione"),
                            .Username_Modifica = drMovDet.Item("Username_Modifica")
                        }

                        If Not IsDBNull(drMovDet.Item("Extra_Date")) Then
                            Movimento_Dettaglio.Extra_Date = drMovDet.Item("Extra_Date")
                        End If

                        If Not IsDBNull(drMovDet.Item("TempoCarenza")) Then
                            Movimento_Dettaglio.TempoCarenza = drMovDet.Item("TempoCarenza")
                        End If
                        If Not IsDBNull(drMovDet.Item("DoseEtichetta")) Then
                            Movimento_Dettaglio.DoseEtichetta = drMovDet.Item("DoseEtichetta")
                        End If
                        If Not IsDBNull(drMovDet.Item("DoseEtichetta_Value")) Then
                            Movimento_Dettaglio.DoseEtichetta_Value = drMovDet.Item("DoseEtichetta_Value")
                        End If

                        If Not IsDBNull(drMovDet.Item("PrincipiAttivi")) Then
                            Movimento_Dettaglio.PrincipiAttivi = drMovDet.Item("PrincipiAttivi")
                        End If
                        If Not IsDBNull(drMovDet.Item("PrincipiAttiviPesi")) Then
                            Movimento_Dettaglio.PrincipiAttiviPesi = drMovDet.Item("PrincipiAttiviPesi")
                        End If
                        If Not IsDBNull(drMovDet.Item("Buffer")) Then
                            Movimento_Dettaglio.Buffer = drMovDet.Item("Buffer")
                        End If

                        If Not IsDBNull(drMovDet.Item("ClassiTossicologiche")) Then
                            Movimento_Dettaglio.CLassiTossicologiche = drMovDet.Item("ClassiTossicologiche")
                        End If
                        If Not IsDBNull(drMovDet.Item("Id_Attivita")) Then
                            Movimento_Dettaglio.ID_Attivita = drMovDet.Item("Id_Attivita")
                        Else
                            Movimento_Dettaglio.ID_Attivita = 0
                        End If
                        If Not IsDBNull(drMovDet.Item("Turno_Cod")) Then
                            Movimento_Dettaglio.Turno_Cod = drMovDet.Item("Turno_Cod")
                        Else
                            Movimento_Dettaglio.Turno_Cod = 0
                        End If
                        If Not IsDBNull(drMovDet.Item("Prezzo_Unitario")) Then
                            Movimento_Dettaglio.Prezzo_Unitario = drMovDet.Item("Prezzo_Unitario")
                        End If

                        If Not IsDBNull(drMovDet.Item("Qualifica_Cod")) Then
                            Movimento_Dettaglio.Qualifica_Cod = drMovDet.Item("Qualifica_Cod")
                        End If
                        If Not IsDBNull(drMovDet.Item("Tariffa_Cod")) Then
                            Movimento_Dettaglio.Tariffa_Cod = drMovDet.Item("Tariffa_Cod")
                        End If
                        If Not IsDBNull(drMovDet.Item("Prezzo_Unitario_Netto")) Then
                            Movimento_Dettaglio.Prezzo_Unitario_Netto = drMovDet.Item("Prezzo_Unitario_Netto")
                        End If
                        If Not IsDBNull(drMovDet.Item("Mov_Det_Des")) Then
                            Movimento_Dettaglio.Mov_Det_Des = drMovDet.Item("Mov_Det_Des")
                        End If
                        If Not IsDBNull(drMovDet.Item("Anno")) Then
                            Movimento_Dettaglio.Anno = drMovDet.Item("Anno")
                        End If

                        If Not IsDBNull(drMovDet.Item("Qta_Extra_Totale")) Then
                            Movimento_Dettaglio.Qta_Extra_Totale = drMovDet.Item("Qta_Extra_Totale")
                        Else
                            Movimento_Dettaglio.Qta_Extra_Totale = 0
                        End If
                        If Not IsDBNull(drMovDet.Item("Tara")) Then
                            Movimento_Dettaglio.Tara = drMovDet.Item("Tara")
                        Else
                            Movimento_Dettaglio.Tara = 0
                        End If


                        If Not IsNothing(drMovDet.Item("ChkIva_Manuale")) AndAlso
                            Not IsDBNull(drMovDet.Item("ChkIva_Manuale")) Then
                            Movimento_Dettaglio.ChkIva_Manuale = drMovDet.Item("ChkIva_Manuale")
                        End If

                        If Not IsNothing(drMovDet.Item("Cod_IvaIndetraibile")) AndAlso
                            Not IsDBNull(drMovDet.Item("Cod_IvaIndetraibile")) Then
                            Movimento_Dettaglio.Cod_IvaIndetraibile = drMovDet.Item("Cod_IvaIndetraibile")
                        End If

                        If Not IsNothing(drMovDet.Item("ChkLayOut_Hide")) AndAlso
                            Not IsDBNull(drMovDet.Item("ChkLayOut_Hide")) Then
                            Movimento_Dettaglio.ChkLayOut_Hide = drMovDet.Item("ChkLayOut_Hide")
                        End If

                        If Not IsNothing(drMovDet.Item("Variazione")) AndAlso
                            Not IsDBNull(drMovDet.Item("Variazione")) Then
                            Movimento_Dettaglio.Variazione = drMovDet.Item("Variazione")
                        End If

                        If Not IsNothing(drMovDet.Item("Listino_Cod")) AndAlso
                            Not IsDBNull(drMovDet.Item("Listino_Cod")) Then
                            Movimento_Dettaglio.Listino_Cod = drMovDet.Item("Listino_Cod")
                        End If

                        If Not IsNothing(drMovDet.Item("Mezzo_Det")) AndAlso
                            Not IsDBNull(drMovDet.Item("Mezzo_Det")) Then
                            Movimento_Dettaglio.Mezzo_Det = drMovDet.Item("Mezzo_Det")
                        End If

                        If Not IsNothing(drMovDet.Item("Ric_Cod_Pat")) AndAlso
                            Not IsDBNull(drMovDet.Item("Ric_Cod_Pat")) Then
                            Movimento_Dettaglio.Ric_Cod_Pat = drMovDet.Item("Ric_Cod_Pat")
                        End If

                        If Not IsNothing(drMovDet.Item("Cod_Conto_Pat")) AndAlso
                            Not IsDBNull(drMovDet.Item("Cod_Conto_Pat")) Then
                            Movimento_Dettaglio.Cod_Conto_Pat = drMovDet.Item("Cod_Conto_Pat")
                        End If

                        If Not IsNothing(drMovDet.Item("Dettaglio_VegCod")) AndAlso
                            Not IsDBNull(drMovDet.Item("Dettaglio_VegCod")) Then
                            Movimento_Dettaglio.Veg_Cod = drMovDet.Item("Dettaglio_VegCod")
                        End If

                        If Not IsNothing(drMovDet.Item("Iva_Indetraibile")) AndAlso
                            Not IsDBNull(drMovDet.Item("Iva_Indetraibile")) Then
                            Movimento_Dettaglio.Iva_Indetraibile = drMovDet.Item("Iva_Indetraibile")
                        End If

                        If Not IsNothing(drMovDet.Item("Iva_Indetraibile_Perc")) AndAlso
                            Not IsDBNull(drMovDet.Item("Iva_Indetraibile_Perc")) Then
                            Movimento_Dettaglio.Iva_Indetraibile_Perc = drMovDet.Item("Iva_Indetraibile_Perc")
                        End If

                        If Not IsNothing(drMovDet.Item("Iva_Deto_Cod")) AndAlso
                            Not IsDBNull(drMovDet.Item("Iva_Deto_Cod")) Then
                            Movimento_Dettaglio.Iva_Deto_Cod = drMovDet.Item("Iva_Deto_Cod")
                        End If

                        If Not IsNothing(drMovDet.Item("Qta_Dettaglio1")) AndAlso
                           Not IsDBNull(drMovDet.Item("Qta_Dettaglio1")) Then
                            Movimento_Dettaglio.Qta_Dettaglio1 = drMovDet.Item("Qta_Dettaglio1")
                        End If

                        If Not IsNothing(drMovDet.Item("Qta_Dettaglio2")) AndAlso
                           Not IsDBNull(drMovDet.Item("Qta_Dettaglio2")) Then
                            Movimento_Dettaglio.Qta_Dettaglio2 = drMovDet.Item("Qta_Dettaglio2")
                        End If

                        If Not IsNothing(drMovDet.Item("Dettagli_Blocco_Flag")) AndAlso
                            Not IsDBNull(drMovDet.Item("Dettagli_Blocco_Flag")) Then
                            Movimento_Dettaglio.Dettagli_Blocco_Flag = drMovDet.Item("Dettagli_Blocco_Flag")
                        End If

                        If Not IsNothing(drMovDet.Item("Dettagli_Blocco_Username")) AndAlso
                            Not IsDBNull(drMovDet.Item("Dettagli_Blocco_Username")) Then
                            Movimento_Dettaglio.Dettagli_Blocco_Username = drMovDet.Item("Dettagli_Blocco_Username")
                        End If

                        If Not IsNothing(drMovDet.Item("Dettagli_Blocco_Data")) AndAlso
                            Not IsDBNull(drMovDet.Item("Dettagli_Blocco_Data")) Then
                            Movimento_Dettaglio.Dettagli_Blocco_Data = drMovDet.Item("Dettagli_Blocco_Data")
                        End If

                        If Not IsNothing(drMovDet.Item("Ordine_Det")) AndAlso
                            Not IsDBNull(drMovDet.Item("Ordine_Det")) Then
                            Movimento_Dettaglio.Ordine_Det = drMovDet.Item("Ordine_Det")
                        End If

                        If Not IsNothing(drMovDet.Item("Deroga_Cod")) AndAlso
                            Not IsDBNull(drMovDet.Item("Deroga_Cod")) Then
                            Movimento_Dettaglio.Deroga_Cod = drMovDet.Item("Deroga_Cod")
                        End If

                        If Not IsNothing(drMovDet.Item("Prezzo_Livello")) AndAlso
                            Not IsDBNull(drMovDet.Item("Prezzo_Livello")) Then
                            Movimento_Dettaglio.Prezzo_Livello = drMovDet.Item("Prezzo_Livello")
                        End If

                        If Not IsNothing(drMovDet.Item("Rif_Esterno")) AndAlso
                            Not IsDBNull(drMovDet.Item("Rif_Esterno")) Then
                            Movimento_Dettaglio.Rif_Esterno = drMovDet.Item("Rif_Esterno")
                        End If

                        If Not IsNothing(drMovDet.Item("Rif_Esterno_2")) AndAlso
                            Not IsDBNull(drMovDet.Item("Rif_Esterno_2")) Then
                            Movimento_Dettaglio.Rif_Esterno_2 = drMovDet.Item("Rif_Esterno_2")
                        End If

                        If Not IsDBNull(drMovDet.Item("PrincipiAttiviPercAbb")) Then
                            Movimento_Dettaglio.PrincipiAttiviPercAbb = drMovDet.Item("PrincipiAttiviPercAbb")
                        End If

                        If Not IsDBNull(drMovDet.Item("Polverulento")) Then
                            Movimento_Dettaglio.Polverulento = drMovDet.Item("Polverulento")
                        End If

                        If Not IsDBNull(drMovDet.Item("Dettaglio_IdCod")) Then
                            Movimento_Dettaglio.Id_Cod = drMovDet.Item("Dettaglio_IdCod")
                        End If

                        If Not IsDBNull(drMovDet.Item("Dettaglio_GenCod")) Then
                            Movimento_Dettaglio.Gen_Cod = drMovDet.Item("Dettaglio_GenCod")
                        End If

                        If Not IsDBNull(drMovDet.Item("Dettaglio_SpeCod")) Then
                            Movimento_Dettaglio.Spe_Cod = drMovDet.Item("Dettaglio_SpeCod")
                        End If

                        If Not IsDBNull(drMovDet.Item("Dettaglio_IProCod")) Then
                            Movimento_Dettaglio.IPro_Cod = drMovDet.Item("Dettaglio_IProCod")
                        End If

                        listaMovimentiDettagli.Add(Movimento_Dettaglio)

                        If leggiEsclusivamenteDettaglio = False Then


                            '--------------------------------------------------------
                            '-------- MOVIMENTI_DETTAGLI_TECNICI --------------------
                            '--------------------------------------------------------
                            Dim objMovimentiDettagliTecnici As New Agenda_Movimenti_Dettagli_Tecnici_Helper
                            Dim listaMovimentiDettagliTecnici As List(Of Movimento_Dettaglio_Tecnico)

                            listaMovimentiDettagliTecnici = objMovimentiDettagliTecnici.Leggi(piva,
                                                                                              saCod,
                                                                                              idAgenda,
                                                                                              drMovDet.Item("Id_Mov"),
                                                                                              drMovDet.Item("Id_Mov_Det"),
                                                                                              objParametri)
                            objMovimentiDettagliTecnici = Nothing

                            If Not IsNothing(listaMovimentiDettagliTecnici) Then
                                Movimento_Dettaglio.Movimenti_Dettagli_Tecnici = listaMovimentiDettagliTecnici
                            End If

                            '--------------------------------------------------------
                            '-------- MOVIMENTI_DETTAGLI_TECNICI_EXTRA --------------
                            '--------------------------------------------------------
                            Dim objMovimentiDettagliTecniciExtra As New Agenda_Movimenti_Dettagli_Tecnici_extra_Helper
                            Dim listaMovimentiDettagliTecniciExtra As List(Of Movimento_Dettaglio_Tecnico_Extra)

                            listaMovimentiDettagliTecniciExtra = objMovimentiDettagliTecniciExtra.Leggi(piva,
                                                                                                        saCod,
                                                                                                        idAgenda,
                                                                                                        drMovDet.Item("Id_Mov"),
                                                                                                        drMovDet.Item("Id_Mov_Det"),
                                                                                                        0,
                                                                                                        objParametri)
                            objMovimentiDettagliTecniciExtra = Nothing

                            If Not IsNothing(listaMovimentiDettagliTecniciExtra) Then
                                Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra = listaMovimentiDettagliTecniciExtra
                            End If

                            '--------------------------------------------------------
                            '-------- MOVIMENTI_DETTAGLI_CONFERIMENTO ---------------
                            '--------------------------------------------------------
                            Dim objMovimentiDettagliConferimento As New Agenda_Movimenti_Dettagli_Conferimento_Helper
                            Dim listaMovimentiDettagliConferimento As List(Of Movimento_Dettaglio_Conferimento)

                            listaMovimentiDettagliConferimento = objMovimentiDettagliConferimento.Leggi(piva,
                                                                                                        saCod,
                                                                                                        idAgenda,
                                                                                                        drMovDet.Item("Id_Mov"),
                                                                                                        drMovDet.Item("Id_Mov_Det"),
                                                                                                        0,
                                                                                                        objParametri)
                            objMovimentiDettagliConferimento = Nothing

                            If Not IsNothing(listaMovimentiDettagliConferimento) Then
                                Movimento_Dettaglio.Movimenti_Dettagli_Conferimento = listaMovimentiDettagliConferimento
                            End If

                            '--------------------------------------------------------
                            '-------- MOVIMENTI_DESTINAZIONI ------------------------
                            '--------------------------------------------------------
                            Dim objMovimentiDestinazioni As New Agenda_Movimenti_Destinazioni_Helper
                            Dim listaMovimentiDestinazioni As List(Of Movimento_Destinazione)

                            listaMovimentiDestinazioni = objMovimentiDestinazioni.Leggi(piva,
                                                                                        0,
                                                                                        idAgenda,
                                                                                        drMovDet.Item("Id_Mov"),
                                                                                        drMovDet.Item("Id_Mov_Det"),
                                                                                        objParametri)

                            objMovimentiDestinazioni = Nothing

                            If Not IsNothing(listaMovimentiDestinazioni) Then
                                Movimento_Dettaglio.Movimenti_Destinazioni = listaMovimentiDestinazioni
                            End If

                            '--------------------------------------------------------
                            '-------- MOVIMENTI_RIFERIMENTI ------------------------
                            '--------------------------------------------------------
                            Dim objMovimentiRiferimenti As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                            Dim listaMovimentiDettagliRiferimenti As List(Of Movimento_Dettaglio_Riferimento)

                            listaMovimentiDettagliRiferimenti = objMovimentiRiferimenti.Leggi(piva,
                                                                                              0,
                                                                                              idAgenda,
                                                                                              drMovDet.Item("Id_Mov"),
                                                                                              drMovDet.Item("Id_Mov_Det"),
                                                                                              0, "",
                                                                                              objParametri,
                                                                                              filtroAggiuntivoRiferimenti)

                            objMovimentiRiferimenti = Nothing

                            If Not IsNothing(listaMovimentiDettagliRiferimenti) Then
                                Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti = listaMovimentiDettagliRiferimenti
                            End If

                            '--------------------------------------------------------
                            '-------- MATERIE_PRIME_CAMPIONATURE --------------------
                            '--------------------------------------------------------
                            'Le devo leggere solo se il Cal_Cod è negativo, perché sennò tanto non ci sarebbe
                            If CInt(drMovDet.Item("Cal_Cod")) < 0 Then

                                Dim objMateriePrimeCampionature As New Agenda_Materie_Prime_Campionature_Helper
                                Dim listaMateriePrimeCampionature As List(Of Materia_Prima_Campionatura)

                                listaMateriePrimeCampionature = objMateriePrimeCampionature.Leggi(CInt(drMovDet.Item("Cal_Cod")),
                                                                                                  "",
                                                                                                  0,
                                                                                                  0,
                                                                                                  objParametri)

                                objMateriePrimeCampionature = Nothing

                                If Not IsNothing(listaMateriePrimeCampionature) Then
                                    Movimento_Dettaglio.Materie_Prime_Campionature = listaMateriePrimeCampionature
                                End If

                            End If

                        End If

                    Next

                End If

            Catch ex As Exception
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Helper.Leggi() ] : " & ex.Message)
            Finally
                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDettagli

        End Function

    End Class


End Namespace
