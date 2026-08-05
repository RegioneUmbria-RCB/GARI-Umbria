Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Namespace OperazioneAgenda_Temp

    Public Class Movimento_Dettaglio_Tecnico_Extra
        Sub New()
            Piva = ""

            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Id_Reg_Dettaglio = 0

            Regione = ""
            ASL = ""
            Serie = ""
            Numero = ""
            Mac_Cod = 0
            Cod_RisUm = 0
            Trasportatore = ""
            Mezzo_Trasporto = ""
            Targa = ""
            N_Immatricolazione = ""
            N_Immatricolazione_Rimorchio = ""
            N_Autorizzazione_Trasporto = ""
            Data_Rilascio_Autorizzazione = New Date
            Peso = 0
            Codice_Prodotto = 0
            Colore = 0
            Zona_Viticola = ""
            Manipolazioni = 0
            Precisazioni = ""
            Annotazioni = ""
            Num_Contenitori = 0
            Marche_Contenitori = ""
            Des_Contenitori = ""
            Tipo_Documento = ""
            Id_Cod_Autorita = 0
            Luogo_Partenza = ""
            Luogo_Consegna = ""
            Data_Spedizione = New Date
            Indicazioni_Complementari = ""
            Titolo_Alcol = 0
            Codice_NC = ""
            Num_Riferimento = ""
            Data_Dichiarazione = New Date
            Garanzia = ""
            Certificati = ""
            Durata_Viaggio = ""
            Peso_Lordo = 0
            Num_Colli = 0
            Contenitore_Cod = 0
            Imballaggio_Cod = 0
            Agente_Cod = 0
            Provvigione = 0
            Tipo_Trasporto = 0
            Unita_Trasporto = 0
            Codice_Alternativo = ""
            Id_Gestione_Vettore = 0
            Ritenuta_Acconto_Cod = 0
            Ritenuta_Acconto = 0
            Enasarco_Cod = 0
            Enasarco = 0
            ACCDAA_Cod_Risum_Destinatario = 0
            ACCDAA_Cod_Risum_Destinazione = 0
            ACCDAA_Cod_IndirizzoRisum_Destinatario = 0
            ACCDAA_Cod_IndirizzoRisum_Destinazione = 0

            CapoArea_Cod = 0
            Provvigione_CapoArea = 0
            Provvigione_Pagata_Agente = 0
            Provvigione_Pagata_CapoArea = 0
            N_Doc_Cliente = ""
            Data_Doc_Cliente = AGRODATAINIZIO
            N_Doc_Ente = ""
            Anno_Doc_Ente = Year(AGRODATAFINE)
            Num_Conf_Riscontrate = -1
            Num_Colli_Riscontrati = -1
            Num_Imballi_Riscontrati = -1
            Peso_Netto_Riscontrato = 0
            Peso_Lordo_Riscontrato = 0

            Tara_Unit_Conf_Riscontrata = -1
            Tara_Unit_Collo_Riscontrata = -1
            Tara_Unit_Imballo_Riscontrata = -1
            N_Nota_Fattura = ""
            Data_Nota_Fattura = AGRODATAINIZIO
            N_Nota_DDT = ""
            N_Nota_Riga_DDT = ""
            Data_Nota_DDT = AGRODATAINIZIO
            Causale_Fattura = 0

            DistanzaTrasportoUdm = 0
            DistanzaTrasporto = 0

            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Sub New(ByVal pivaInput As String)
            Piva = pivaInput

            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Id_Reg_Dettaglio = 0

            Regione = ""
            ASL = ""
            Serie = ""
            Numero = ""
            Mac_Cod = 0
            Cod_RisUm = 0
            Trasportatore = ""
            Mezzo_Trasporto = ""
            Targa = ""
            N_Immatricolazione = ""
            N_Immatricolazione_Rimorchio = ""
            N_Autorizzazione_Trasporto = ""
            Data_Rilascio_Autorizzazione = New Date
            Peso = 0
            Codice_Prodotto = 0
            Colore = 0
            Zona_Viticola = ""
            Manipolazioni = 0
            Precisazioni = ""
            Annotazioni = ""
            Num_Contenitori = 0
            Marche_Contenitori = ""
            Des_Contenitori = ""
            Tipo_Documento = ""
            Id_Cod_Autorita = 0
            Luogo_Partenza = ""
            Luogo_Consegna = ""
            Data_Spedizione = New Date
            Indicazioni_Complementari = ""
            Titolo_Alcol = 0
            Codice_NC = ""
            Num_Riferimento = ""
            Data_Dichiarazione = New Date
            Garanzia = ""
            Certificati = ""
            Durata_Viaggio = ""
            Peso_Lordo = 0
            Num_Colli = 0
            Contenitore_Cod = 0
            Imballaggio_Cod = 0
            Agente_Cod = 0
            Provvigione = 0
            Tipo_Trasporto = 0
            Unita_Trasporto = 0
            Codice_Alternativo = ""
            Id_Gestione_Vettore = 0
            Ritenuta_Acconto_Cod = 0
            Ritenuta_Acconto = 0
            Enasarco_Cod = 0
            Enasarco = 0
            ACCDAA_Cod_Risum_Destinatario = 0
            ACCDAA_Cod_Risum_Destinazione = 0
            ACCDAA_Cod_IndirizzoRisum_Destinatario = 0
            ACCDAA_Cod_IndirizzoRisum_Destinazione = 0

            CapoArea_Cod = 0
            Provvigione_CapoArea = 0
            Provvigione_Pagata_Agente = 0
            Provvigione_Pagata_CapoArea = 0
            N_Doc_Cliente = ""
            Data_Doc_Cliente = AGRODATAINIZIO
            N_Doc_Ente = ""
            Anno_Doc_Ente = Year(AGRODATAFINE)
            Num_Conf_Riscontrate = -1
            Num_Colli_Riscontrati = -1
            Num_Imballi_Riscontrati = -1
            Peso_Netto_Riscontrato = 0
            Peso_Lordo_Riscontrato = 0

            Tara_Unit_Conf_Riscontrata = -1
            Tara_Unit_Collo_Riscontrata = -1
            Tara_Unit_Imballo_Riscontrata = -1
            N_Nota_Fattura = ""
            Data_Nota_Fattura = AGRODATAINIZIO
            N_Nota_DDT = ""
            N_Nota_Riga_DDT = ""
            Data_Nota_DDT = AGRODATAINIZIO
            Causale_Fattura = 0
            
            DistanzaTrasportoUdm = 0
            DistanzaTrasporto = 0

            Validita_Inizio = AGRODATAINIZIO
            validita_fine = AGRODATAFINE

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property Piva As String

        Public Property Sa_Cod As Integer

        Public Property Id_Agenda As Integer

        Public Property Id_Mov As Integer

        Public Property Id_Mov_Det As Integer

        Public Property Id_Reg_Dettaglio As Integer

        Public Property Regione As String

        Public Property ASL As String

        Public Property Serie As String

        Public Property Numero As String

        Public Property Mac_Cod As Integer

        Public Property Cod_RisUm As Integer

        Public Property Trasportatore As String

        Public Property Mezzo_Trasporto As String

        Public Property Targa As String

        Public Property N_Immatricolazione As String

        Public Property N_Immatricolazione_Rimorchio As String

        Public Property N_Autorizzazione_Trasporto As String

        Public Property Data_Rilascio_Autorizzazione As DateTime

        Public Property Peso As Decimal

        Public Property Codice_Prodotto As Integer

        Public Property Colore As Integer

        Public Property Zona_Viticola As String

        Public Property Manipolazioni As Integer

        Public Property Precisazioni As String

        Public Property Annotazioni As String

        Public Property Num_Contenitori As Integer

        Public Property Marche_Contenitori As String

        Public Property Des_Contenitori As String

        Public Property Tipo_Documento As String

        Public Property Id_Cod_Autorita As Integer

        Public Property Luogo_Partenza As String

        Public Property Luogo_Consegna As String

        Public Property Data_Spedizione As DateTime

        Public Property Indicazioni_Complementari As String

        Public Property Titolo_Alcol As Decimal

        Public Property Codice_NC As String

        Public Property Num_Riferimento As String

        Public Property Data_Dichiarazione As DateTime

        Public Property Garanzia As String

        Public Property Certificati As String

        Public Property Durata_Viaggio As String

        Public Property Peso_Lordo As Decimal

        Public Property Num_Colli As Integer

        Public Property Contenitore_Cod As Integer

        Public Property Imballaggio_Cod As Integer

        Public Property Agente_Cod As Integer

        Public Property Provvigione As Decimal

        Public Property Tipo_Trasporto As Integer

        Public Property Unita_Trasporto As Integer

        Public Property Codice_Alternativo As String

        Public Property Id_Gestione_Vettore As Integer

        Public Property Ritenuta_Acconto_Cod As Integer

        Public Property Ritenuta_Acconto As Decimal

        Public Property Enasarco_Cod As Integer

        Public Property Enasarco As Decimal

        Public Property ACCDAA_Cod_Risum_Destinatario As Integer

        Public Property ACCDAA_Cod_Risum_Destinazione As Integer

        Public Property ACCDAA_Cod_IndirizzoRisum_Destinatario As Integer

        Public Property ACCDAA_Cod_IndirizzoRisum_Destinazione As Integer

        Public Property CapoArea_Cod As Integer

        Public Property Provvigione_CapoArea As Decimal

        Public Property Provvigione_Pagata_Agente As Decimal

        Public Property Provvigione_Pagata_CapoArea As Decimal

        Public Property N_Doc_Cliente As String

        Public Property Data_Doc_Cliente As Date

        Public Property N_Doc_Ente As String

        Public Property Anno_Doc_Ente As Integer

        Public Property Num_Conf_Riscontrate As Integer

        Public Property Num_Colli_Riscontrati As Integer

        Public Property Num_Imballi_Riscontrati As Integer

        Public Property Peso_Netto_Riscontrato As Decimal

        Public Property Peso_Lordo_Riscontrato As Decimal

        Public Property Tara_Unit_Conf_Riscontrata As Decimal

        Public Property Tara_Unit_Collo_Riscontrata As Decimal

        Public Property Tara_Unit_Imballo_Riscontrata As Decimal

        Public Property N_Nota_Fattura As String

        Public Property Data_Nota_Fattura As Date

        Public Property N_Nota_DDT As String

        Public Property N_Nota_Riga_DDT As String

        Public Property Data_Nota_DDT As Date

        Public Property Causale_Fattura As Integer
        Public Property DistanzaTrasportoUdm As Integer
        Public Property DistanzaTrasporto As Decimal
        Public Property Validita_Inizio As DateTime

        Public Property Validita_Fine As DateTime

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

    End Class

    Public Class Agenda_Movimenti_Dettagli_Tecnici_extra_Helper

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal idMov As Integer,
                              ByVal idMovDet As Integer,
                              ByVal idRegDettaglio As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Movimento_Dettaglio_Tecnico_Extra)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDettagliTecniciExtra As New List(Of Movimento_Dettaglio_Tecnico_Extra)
            Dim objMovDetTecnicoExtra As Movimento_Dettaglio_Tecnico_Extra

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DETTAGLI_TECNICI EXTRA --------------
                '--------------------------------------------------------
                Dim objMovDetTecExtraR As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R
                Dim dtMovDetTecExtra As DataTable

                dtMovDetTecExtra = objMovDetTecExtraR.Leggi(CStr(piva),
                                                            CInt(saCod),
                                                            CInt(idAgenda),
                                                            idMov,
                                                            idMovDet,
                                                            idRegDettaglio,
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "",
                                                            "",
                                                            objParametri)

                objMovDetTecExtraR = Nothing

                If dtMovDetTecExtra.Rows.Count > 0 Then

                    For Each tecnicoExtra As DataRow In dtMovDetTecExtra.Rows

                        objMovDetTecnicoExtra = New Movimento_Dettaglio_Tecnico_Extra With {
                            .Piva = tecnicoExtra.Item("Piva"),
                            .Sa_Cod = tecnicoExtra.Item("Sa_Cod"),
                            .Id_Agenda = tecnicoExtra.Item("Id_Agenda"),
                            .Id_Mov = tecnicoExtra.Item("Id_Mov"),
                            .Id_Mov_Det = tecnicoExtra.Item("Id_Mov_Det"),
                            .Id_Reg_Dettaglio = tecnicoExtra.Item("Id_Reg_Dettaglio"),
                            .Regione = tecnicoExtra.Item("Regione"),
                            .ASL = tecnicoExtra.Item("ASL"),
                            .Serie = tecnicoExtra.Item("Serie"),
                            .Numero = tecnicoExtra.Item("Numero"),
                            .Mac_Cod = tecnicoExtra.Item("Mac_Cod"),
                            .Cod_RisUm = tecnicoExtra.Item("Cod_RisUm"),
                            .Trasportatore = tecnicoExtra.Item("Trasportatore"),
                            .Mezzo_Trasporto = tecnicoExtra.Item("Mezzo_Trasporto"),
                            .Targa = tecnicoExtra.Item("Targa"),
                            .N_Immatricolazione = tecnicoExtra.Item("N_Immatricolazione"),
                            .N_Immatricolazione_Rimorchio = tecnicoExtra.Item("N_Immatricolazione_Rimorchio"),
                            .N_Autorizzazione_Trasporto = tecnicoExtra.Item("N_Autorizzazione_Trasporto"),
                            .Data_Creazione = CDate(tecnicoExtra.Item("Data_Creazione")),
                            .Data_Modifica = CDate(tecnicoExtra.Item("Data_Modifica")),
                            .Username_Creazione = tecnicoExtra.Item("Username_Creazione"),
                            .Username_Modifica = tecnicoExtra.Item("Username_Modifica")
                        }

                        If Not IsNothing(tecnicoExtra.Item("Data_Rilascio_Autorizzazione")) AndAlso
                            IsDate(tecnicoExtra.Item("Data_Rilascio_Autorizzazione")) Then
                            objMovDetTecnicoExtra.Data_Rilascio_Autorizzazione = CDate(tecnicoExtra.Item("Data_Rilascio_Autorizzazione"))
                        End If

                        objMovDetTecnicoExtra.Peso = tecnicoExtra.Item("Peso")
                        objMovDetTecnicoExtra.Codice_Prodotto = tecnicoExtra.Item("Codice_Prodotto")
                        objMovDetTecnicoExtra.Colore = tecnicoExtra.Item("Colore")
                        objMovDetTecnicoExtra.Zona_Viticola = tecnicoExtra.Item("Zona_Viticola")
                        objMovDetTecnicoExtra.Manipolazioni = tecnicoExtra.Item("Manipolazioni")
                        objMovDetTecnicoExtra.Precisazioni = tecnicoExtra.Item("Precisazioni")
                        objMovDetTecnicoExtra.Annotazioni = tecnicoExtra.Item("Annotazioni")
                        objMovDetTecnicoExtra.Num_Contenitori = tecnicoExtra.Item("Num_Contenitori")
                        objMovDetTecnicoExtra.Marche_Contenitori = tecnicoExtra.Item("Marche_Contenitori")
                        objMovDetTecnicoExtra.Des_Contenitori = tecnicoExtra.Item("Tipo_Documento")
                        objMovDetTecnicoExtra.Tipo_Documento = tecnicoExtra.Item("Id_Reg_Dettaglio")
                        objMovDetTecnicoExtra.Id_Cod_Autorita = tecnicoExtra.Item("Id_Cod_Autorita")
                        objMovDetTecnicoExtra.Luogo_Partenza = tecnicoExtra.Item("Luogo_Partenza")
                        objMovDetTecnicoExtra.Luogo_Consegna = tecnicoExtra.Item("Luogo_Consegna")

                        If Not IsNothing(tecnicoExtra.Item("Data_Spedizione")) AndAlso
                            IsDate(tecnicoExtra.Item("Data_Spedizione")) Then
                            objMovDetTecnicoExtra.Data_Spedizione = CDate(tecnicoExtra.Item("Data_Spedizione"))
                        End If

                        objMovDetTecnicoExtra.Indicazioni_Complementari = tecnicoExtra.Item("Indicazioni_Complementari")
                        objMovDetTecnicoExtra.Titolo_Alcol = tecnicoExtra.Item("Titolo_Alcol")
                        objMovDetTecnicoExtra.Codice_NC = tecnicoExtra.Item("Codice_NC")
                        objMovDetTecnicoExtra.Num_Riferimento = tecnicoExtra.Item("Num_Riferimento")

                        If Not IsNothing(tecnicoExtra.Item("Data_Dichiarazione")) AndAlso
                            IsDate(tecnicoExtra.Item("Data_Dichiarazione")) Then
                            objMovDetTecnicoExtra.Data_Dichiarazione = CDate(tecnicoExtra.Item("Data_Dichiarazione"))
                        End If

                        objMovDetTecnicoExtra.Garanzia = tecnicoExtra.Item("Garanzia")
                        objMovDetTecnicoExtra.Certificati = tecnicoExtra.Item("Certificati")
                        objMovDetTecnicoExtra.Durata_Viaggio = tecnicoExtra.Item("Durata_Viaggio")
                        objMovDetTecnicoExtra.Peso_Lordo = tecnicoExtra.Item("Peso_Lordo")
                        objMovDetTecnicoExtra.Num_Colli = tecnicoExtra.Item("Num_Colli")
                        objMovDetTecnicoExtra.Contenitore_Cod = tecnicoExtra.Item("Contenitore_Cod")
                        objMovDetTecnicoExtra.Imballaggio_Cod = tecnicoExtra.Item("Imballaggio_Cod")
                        objMovDetTecnicoExtra.Agente_Cod = tecnicoExtra.Item("Agente_Cod")
                        objMovDetTecnicoExtra.Provvigione = tecnicoExtra.Item("Provvigione")

                        If Not IsNothing(tecnicoExtra.Item("Tipo_Trasporto")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Tipo_Trasporto")) Then
                            objMovDetTecnicoExtra.Tipo_Trasporto = tecnicoExtra.Item("Tipo_Trasporto")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Unita_Trasporto")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Unita_Trasporto")) Then
                            objMovDetTecnicoExtra.Unita_Trasporto = tecnicoExtra.Item("Unita_Trasporto")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Codice_Alternativo")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Codice_Alternativo")) Then
                            objMovDetTecnicoExtra.Codice_Alternativo = tecnicoExtra.Item("Codice_Alternativo")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Id_Gestione_Vettore")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Id_Gestione_Vettore")) Then
                            objMovDetTecnicoExtra.Id_Gestione_Vettore = tecnicoExtra.Item("Id_Gestione_Vettore")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Ritenuta_Acconto_Cod")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Ritenuta_Acconto_Cod")) Then
                            objMovDetTecnicoExtra.Ritenuta_Acconto_Cod = tecnicoExtra.Item("Ritenuta_Acconto_Cod")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Ritenuta_Acconto")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Ritenuta_Acconto")) Then
                            objMovDetTecnicoExtra.Ritenuta_Acconto = tecnicoExtra.Item("Ritenuta_Acconto")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Enasarco_Cod")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Enasarco_Cod")) Then
                            objMovDetTecnicoExtra.Enasarco_Cod = tecnicoExtra.Item("Enasarco_Cod")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Enasarco")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("Enasarco")) Then
                            objMovDetTecnicoExtra.Enasarco = tecnicoExtra.Item("Enasarco")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("ACCDAA_Cod_Risum_Destinatario")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("ACCDAA_Cod_Risum_Destinatario")) Then
                            objMovDetTecnicoExtra.ACCDAA_Cod_Risum_Destinatario = tecnicoExtra.Item("ACCDAA_Cod_Risum_Destinatario")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("ACCDAA_Cod_Risum_Destinazione")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("ACCDAA_Cod_Risum_Destinazione")) Then
                            objMovDetTecnicoExtra.ACCDAA_Cod_Risum_Destinatario = tecnicoExtra.Item("ACCDAA_Cod_Risum_Destinazione")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("ACCDAA_Cod_IndirizzoRisum_Destinatario")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("ACCDAA_Cod_IndirizzoRisum_Destinatario")) Then
                            objMovDetTecnicoExtra.ACCDAA_Cod_Risum_Destinatario = tecnicoExtra.Item("ACCDAA_Cod_IndirizzoRisum_Destinatario")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("ACCDAA_Cod_IndirizzoRisum_Destinazione")) AndAlso
                            Not IsDBNull(tecnicoExtra.Item("ACCDAA_Cod_IndirizzoRisum_Destinazione")) Then
                            objMovDetTecnicoExtra.ACCDAA_Cod_Risum_Destinatario = tecnicoExtra.Item("ACCDAA_Cod_IndirizzoRisum_Destinazione")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("CapoArea_Cod")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("CapoArea_Cod")) Then
                            objMovDetTecnicoExtra.CapoArea_Cod = tecnicoExtra.Item("CapoArea_Cod")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Provvigione_CapoArea")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Provvigione_CapoArea")) Then
                            objMovDetTecnicoExtra.Provvigione_CapoArea = tecnicoExtra.Item("Provvigione_CapoArea")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Provvigione_Pagata_Agente")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Provvigione_Pagata_Agente")) Then
                            objMovDetTecnicoExtra.Provvigione_Pagata_Agente = tecnicoExtra.Item("Provvigione_Pagata_Agente")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Provvigione_Pagata_CapoArea")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Provvigione_Pagata_CapoArea")) Then
                            objMovDetTecnicoExtra.Provvigione_Pagata_CapoArea = tecnicoExtra.Item("Provvigione_Pagata_CapoArea")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("N_Doc_Cliente")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("N_Doc_Cliente")) Then
                            objMovDetTecnicoExtra.N_Doc_Cliente = tecnicoExtra.Item("N_Doc_Cliente")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Data_Doc_Cliente")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Data_Doc_Cliente")) AndAlso 
                           IsDate(tecnicoExtra.Item("Data_Doc_Cliente")) Then
                            objMovDetTecnicoExtra.Data_Doc_Cliente = CDate(tecnicoExtra.Item("Data_Doc_Cliente"))
                        End If

                        If Not IsNothing(tecnicoExtra.Item("N_Doc_Ente")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("N_Doc_Ente")) Then
                            objMovDetTecnicoExtra.N_Doc_Ente = tecnicoExtra.Item("N_Doc_Ente")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Anno_Doc_Ente")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Anno_Doc_Ente")) Then
                            objMovDetTecnicoExtra.Anno_Doc_Ente = tecnicoExtra.Item("Anno_Doc_Ente")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Num_Conf_Riscontrate")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Num_Conf_Riscontrate")) Then
                            objMovDetTecnicoExtra.Num_Conf_Riscontrate = tecnicoExtra.Item("Num_Conf_Riscontrate")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Num_Colli_Riscontrati")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Num_Colli_Riscontrati")) Then
                            objMovDetTecnicoExtra.Num_Colli_Riscontrati = tecnicoExtra.Item("Num_Colli_Riscontrati")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Num_Imballi_Riscontrati")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Num_Imballi_Riscontrati")) Then
                            objMovDetTecnicoExtra.Num_Imballi_Riscontrati = tecnicoExtra.Item("Num_Imballi_Riscontrati")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Peso_Netto_Riscontrato")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Peso_Netto_Riscontrato")) Then
                            objMovDetTecnicoExtra.Peso_Netto_Riscontrato = tecnicoExtra.Item("Peso_Netto_Riscontrato")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Peso_Lordo_Riscontrato")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Peso_Lordo_Riscontrato")) Then
                            objMovDetTecnicoExtra.Peso_Lordo_Riscontrato = tecnicoExtra.Item("Peso_Lordo_Riscontrato")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Tara_Unit_Conf_Riscontrata")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Tara_Unit_Conf_Riscontrata")) Then
                            objMovDetTecnicoExtra.Tara_Unit_Conf_Riscontrata = tecnicoExtra.Item("Tara_Unit_Conf_Riscontrata")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Tara_Unit_Collo_Riscontrata")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Tara_Unit_Collo_Riscontrata")) Then
                            objMovDetTecnicoExtra.Tara_Unit_Collo_Riscontrata = tecnicoExtra.Item("Tara_Unit_Collo_Riscontrata")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Tara_Unit_Imballo_Riscontrata")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Tara_Unit_Imballo_Riscontrata")) Then
                            objMovDetTecnicoExtra.Tara_Unit_Imballo_Riscontrata = tecnicoExtra.Item("Tara_Unit_Imballo_Riscontrata")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("N_Nota_Fattura")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("N_Nota_Fattura")) Then
                            objMovDetTecnicoExtra.N_Nota_Fattura = tecnicoExtra.Item("N_Nota_Fattura")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Data_Nota_Fattura")) AndAlso
                           IsDate(tecnicoExtra.Item("Data_Nota_Fattura")) Then
                            objMovDetTecnicoExtra.Data_Nota_Fattura = CDate(tecnicoExtra.Item("Data_Nota_Fattura"))
                        End If

                        If Not IsNothing(tecnicoExtra.Item("N_Nota_DDT")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("N_Nota_DDT")) Then
                            objMovDetTecnicoExtra.N_Nota_DDT = tecnicoExtra.Item("N_Nota_DDT")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("N_Nota_Riga_DDT")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("N_Nota_Riga_DDT")) Then
                            objMovDetTecnicoExtra.N_Nota_Riga_DDT = tecnicoExtra.Item("N_Nota_Riga_DDT")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Data_Nota_DDT")) AndAlso
                           IsDate(tecnicoExtra.Item("Data_Nota_DDT")) Then
                            objMovDetTecnicoExtra.Data_Nota_DDT = CDate(tecnicoExtra.Item("Data_Nota_DDT"))
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Causale_Fattura")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Causale_Fattura")) Then
                            objMovDetTecnicoExtra.Causale_Fattura = tecnicoExtra.Item("Causale_Fattura")
                        End If

                        If Not IsNothing(tecnicoExtra.Item("Distanza_Trasporto_Udm")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Distanza_Trasporto_Udm")) Then
                            objMovDetTecnicoExtra.DistanzaTrasportoUdm = tecnicoExtra.Item("Distanza_Trasporto_Udm")
                        End If
                        
                        If Not IsNothing(tecnicoExtra.Item("Distanza_Trasporto")) AndAlso
                           Not IsDBNull(tecnicoExtra.Item("Distanza_Trasporto")) Then
                            objMovDetTecnicoExtra.DistanzaTrasporto = tecnicoExtra.Item("Distanza_Trasporto")
                        End If


                        If Not IsNothing(tecnicoExtra.Item("validita_inizio")) AndAlso
                            IsDate(tecnicoExtra.Item("validita_inizio")) Then
                            objMovDetTecnicoExtra.Validita_Inizio = CDate(tecnicoExtra.Item("validita_inizio"))
                        End If

                        If Not IsNothing(tecnicoExtra.Item("validita_fine")) AndAlso
                            IsDate(tecnicoExtra.Item("validita_fine")) Then
                            objMovDetTecnicoExtra.Validita_fine = CDate(tecnicoExtra.Item("validita_fine"))
                        End If

                        listaMovimentiDettagliTecniciExtra.Add(objMovDetTecnicoExtra)

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Movimenti_Dettagli_Tecnici_Helper.Leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDettagliTecniciExtra

        End Function

        Public Function Scrivi(ByVal objMovDetTecnicoExtra As Movimento_Dettaglio_Tecnico_Extra,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim idRegDettaglio As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Dettaglio_Tecnico
                '---------------------------

                idRegDettaglio = objMovDetTecnicoExtra.Id_Reg_Dettaglio

                If idRegDettaglio <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    idRegDettaglio = objSequenze.NuovoId_Tabella("movimenti_dettagli_tecnici_extra",
                                                                 objMovDetTecnicoExtra.BaseCode,
                                                                 objMovDetTecnicoExtra.TopCode,
                                                                 objParametri)

                    objSequenze = Nothing

                End If

                Dim objMovimentiDettagliTecnici As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W

                objMovimentiDettagliTecnici.ScriviFull(
                        Piva:=objMovDetTecnicoExtra.Piva,
                        Sa_Cod:=objMovDetTecnicoExtra.Sa_Cod, 
                        Id_Agenda:=objMovDetTecnicoExtra.Id_Agenda, 
                        Id_Mov:=objMovDetTecnicoExtra.Id_Mov, 
                        Id_Mov_Det:=objMovDetTecnicoExtra.Id_Mov_Det, 
                        Id_Reg_Dettaglio:=idRegDettaglio, 
                        Regione:=objMovDetTecnicoExtra.Regione, 
                        ASL:=objMovDetTecnicoExtra.ASL, 
                        Serie:=objMovDetTecnicoExtra.Serie, 
                        Numero:=objMovDetTecnicoExtra.Numero, 
                        Mac_Cod:=objMovDetTecnicoExtra.Mac_Cod, 
                        Cod_RisUm:=objMovDetTecnicoExtra.Cod_RisUm, 
                        Trasportatore:=objMovDetTecnicoExtra.Trasportatore, 
                        Mezzo_Trasporto:=objMovDetTecnicoExtra.Mezzo_Trasporto, 
                        Targa:=objMovDetTecnicoExtra.Targa, 
                        N_Immatricolazione:=objMovDetTecnicoExtra.N_Immatricolazione, 
                        N_Immatricolazione_Rimorchio:=objMovDetTecnicoExtra.N_Immatricolazione_Rimorchio, 
                        N_Autorizzazione_Trasporto:=objMovDetTecnicoExtra.N_Autorizzazione_Trasporto, 
                        Data_Rilascio_Autorizzazione:=objMovDetTecnicoExtra.Data_Rilascio_Autorizzazione, 
                        Peso:=objMovDetTecnicoExtra.Peso, 
                        Codice_Prodotto:=objMovDetTecnicoExtra.Codice_Prodotto, 
                        Colore:=objMovDetTecnicoExtra.Colore, 
                        Zona_Viticola:=objMovDetTecnicoExtra.Zona_Viticola, 
                        Manipolazioni:=objMovDetTecnicoExtra.Manipolazioni, 
                        Precisazioni:=objMovDetTecnicoExtra.Precisazioni, 
                        Annotazioni:=objMovDetTecnicoExtra.Annotazioni, 
                        Num_Contenitori:=objMovDetTecnicoExtra.Num_Contenitori, 
                        Marche_Contenitori:=objMovDetTecnicoExtra.Marche_Contenitori, 
                        Des_Contenitori:=objMovDetTecnicoExtra.Des_Contenitori, 
                        Tipo_Documento:=objMovDetTecnicoExtra.Tipo_Documento, 
                        Id_Cod_Autorita:=objMovDetTecnicoExtra.Id_Cod_Autorita, 
                        Luogo_Partenza:=objMovDetTecnicoExtra.Luogo_Partenza, 
                        Luogo_Consegna:=objMovDetTecnicoExtra.Luogo_Consegna, 
                        Data_Spedizione:=objMovDetTecnicoExtra.Data_Spedizione, 
                        Indicazioni_Complementari:=objMovDetTecnicoExtra.Indicazioni_Complementari, 
                        Titolo_Alcol:=objMovDetTecnicoExtra.Titolo_Alcol, 
                        Codice_NC:=objMovDetTecnicoExtra.Codice_NC, 
                        Num_Riferimento:=objMovDetTecnicoExtra.Num_Riferimento, 
                        Data_Dichiarazione:=objMovDetTecnicoExtra.Data_Dichiarazione, 
                        Garanzia:=objMovDetTecnicoExtra.Garanzia, 
                        Certificati:=objMovDetTecnicoExtra.Certificati, 
                        Durata_Viaggio:=objMovDetTecnicoExtra.Durata_Viaggio, 
                        Peso_Lordo:=objMovDetTecnicoExtra.Peso_Lordo, 
                        Num_Colli:=objMovDetTecnicoExtra.Num_Colli, 
                        Contenitore_Cod:=objMovDetTecnicoExtra.Contenitore_Cod, 
                        Imballaggio_Cod:=objMovDetTecnicoExtra.Imballaggio_Cod, 
                        Agente_Cod:=objMovDetTecnicoExtra.Agente_Cod, 
                        Provvigione:=objMovDetTecnicoExtra.Provvigione, 
                        Tipo_Trasporto:=objMovDetTecnicoExtra.Tipo_Trasporto, 
                        Unita_Trasporto:=objMovDetTecnicoExtra.Unita_Trasporto, 
                        Codice_Alternativo:=objMovDetTecnicoExtra.Codice_Alternativo, 
                        Id_Gestione_Vettore:=objMovDetTecnicoExtra.Id_Gestione_Vettore, 
                        Ritenuta_Acconto_Cod:=objMovDetTecnicoExtra.Ritenuta_Acconto_Cod, 
                        Ritenuta_Acconto:=objMovDetTecnicoExtra.Ritenuta_Acconto, 
                        Enasarco_Cod:=objMovDetTecnicoExtra.Enasarco_Cod, 
                        Enasarco:=objMovDetTecnicoExtra.Enasarco, 
                        ACCDAA_Cod_Risum_Destinatario:=objMovDetTecnicoExtra.ACCDAA_Cod_Risum_Destinatario, 
                        ACCDAA_Cod_Risum_Destinazione:=objMovDetTecnicoExtra.ACCDAA_Cod_Risum_Destinazione, 
                        ACCDAA_Cod_IndirizzoRisum_Destinatario:=objMovDetTecnicoExtra.ACCDAA_Cod_IndirizzoRisum_Destinatario, 
                        ACCDAA_Cod_IndirizzoRisum_Destinazione:=objMovDetTecnicoExtra.ACCDAA_Cod_IndirizzoRisum_Destinazione,
                        CapoArea_Cod:=objMovDetTecnicoExtra.CapoArea_Cod,
                        Provvigione_CapoArea:=objMovDetTecnicoExtra.Provvigione_CapoArea,
                        Provvigione_Pagata_Agente:=objMovDetTecnicoExtra.Provvigione_Pagata_Agente,
                        Provvigione_Pagata_CapoArea:=objMovDetTecnicoExtra.Provvigione_Pagata_CapoArea,
                        N_Doc_Cliente:=objMovDetTecnicoExtra.N_Doc_Cliente,
                        Data_Doc_Cliente:=objMovDetTecnicoExtra.Data_Doc_Cliente,
                        N_Doc_Ente:=objMovDetTecnicoExtra.N_Doc_Ente,
                        Anno_Doc_Ente:=objMovDetTecnicoExtra.Anno_Doc_Ente,
                        Num_Conf_Riscontrate:=objMovDetTecnicoExtra.Num_Conf_Riscontrate,
                        Num_Colli_Riscontrati:=objMovDetTecnicoExtra.Num_Colli_Riscontrati,
                        Num_Imballi_Riscontrati:=objMovDetTecnicoExtra.Num_Imballi_Riscontrati,
                        Peso_Netto_Riscontrato:=objMovDetTecnicoExtra.Peso_Netto_Riscontrato,
                        Peso_Lordo_Riscontrato:=objMovDetTecnicoExtra.Peso_Lordo_Riscontrato,
                        Validita_Inizio:=objMovDetTecnicoExtra.Validita_Inizio,
                        Validita_Fine:=objMovDetTecnicoExtra.Validita_Fine,
                        objParametri:=objParametri,
                        Data_creazione:=objMovDetTecnicoExtra.Data_Creazione,
                        username_creazione:=objMovDetTecnicoExtra.Username_Creazione,
                        Tara_Unit_Conf_Riscontrata:=objMovDetTecnicoExtra.Tara_Unit_Conf_Riscontrata,
                        Tara_Unit_Collo_Riscontrata:=objMovDetTecnicoExtra.Tara_Unit_Collo_Riscontrata,
                        Tara_Unit_Imballo_Riscontrata:=objMovDetTecnicoExtra.Tara_Unit_Imballo_Riscontrata,
                        N_Nota_Fattura:=objMovDetTecnicoExtra.N_Nota_Fattura,
                        Data_Nota_Fattura:=objMovDetTecnicoExtra.Data_Nota_Fattura,
                        N_Nota_DDT:= objMovDetTecnicoExtra.N_Nota_DDT,
                        N_Nota_Riga_DDT:=objMovDetTecnicoExtra.N_Nota_Riga_DDT,
                        Data_Nota_DDT:=objMovDetTecnicoExtra.Data_Nota_DDT,
                        Causale_Fattura:=objMovDetTecnicoExtra.Causale_Fattura,
                        DistanzaTrasportoUdm := objMovDetTecnicoExtra.DistanzaTrasportoUdm,
                        DistanzaTrasporto := objMovDetTecnicoExtra.DistanzaTrasporto
                )

                objMovimentiDettagliTecnici = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Tecnici_extra_Helper.Scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal idMov As Integer,
                                 ByVal idMovDet As Integer,
                                 ByVal idRegDettaglio As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Dettaglio_Tecnico
                '---------------------------

                Dim objMovimentiDettagliTecnici As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W

                objMovimentiDettagliTecnici.Cancella(piva,
                                                     saCod,
                                                     idAgenda,
                                                     idMov,
                                                     idMovDet,
                                                     idRegDettaglio,
                                                     "",
                                                     objParametri)

                objMovimentiDettagliTecnici = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Tecnici_Helper.Cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

    End Class

End Namespace
