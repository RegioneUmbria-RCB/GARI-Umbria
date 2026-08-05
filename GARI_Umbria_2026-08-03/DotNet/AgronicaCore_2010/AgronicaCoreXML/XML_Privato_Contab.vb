Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class XML_Contab


    '=========================================================================
    '=========================================================================
    '
    '
    '
    'IN QUESTO MODULO VANNO GESTITI SOLO I MOVIMENTI DI AGENDA
    '
    '
    'FABBRICATI, CONTATTI E MATERIE PRIME VANNO GESTITI IN XML_ANAGRAFE
    '
    '
    '
    '
    '

    '=========================================================================
    '=========================================================================



    Const C_AgroDataInizio = #1/1/1900#
    Const C_AgroDataFine = #12/31/2100#
    Const C_BaseCode_Default = 0
    Const C_TopCode_Default = 2000000000
    Const C_Piva_Default = "00000000000"



    '########################################################################################
    '########################################################################################
    '##### TIPI ENUMERATIVI
    '########################################################################################
    '########################################################################################


    Public Enum CoreEnum_Pendenza
        Pendenza_DocBolla = 0                'Movimento Allegato a Bolla di accompagnamento
        Pendenza_DocFattura = 1              'Movimento Allegato a Fattura
        Pendenza_MovPendente = 2             'Movimento Pendente
        Pendenza_MovEsente = 3               'Movimento Esente
        Pendenza_MovGiustificato = 4         'Movimento Giustificato
        Pendenza_MovForzato = 5              'Movimento Non Giustificato e Forzato dall'Utente
        Pendenza_GiacenzeIniziali = 6        'Giacenze Iniziali
        Pendenza_Conferimento = 7            'Materia Prima/Lavorato in Conferimento
        Pendenza_AutoProduzione = 8          'Materia Prima/Lavorato Autoprodotto
        Pendenza_AutoConsumo = 9             'Materia Prima/Lavorato Autoconsumato
        Pendenza_Smaltimento = 10            'Materia Prima/Lavorato Smaltimento
        Pendenza_ZooConsistenzeIniziali = 11 'Consistenze Iniziali Zootecniche
        Pendenza_Trasferimento = 12          'Trasferimento Merci
        Pendenza_DocRicevuta = 13            'Movimento Allegato a Ricevuta Fiscale
        Pendenza_Resi_Acquisti = 14          'Scarico Giustificato da Resi su Acquisti
        Pendenza_Resi_Vendite = 15           'Carico Giustificato da Resi su Vendite
    End Enum

    '---------------------------------------------------------------------------------------------

    'Parametri del Campo 'Contabilizzato' nella tabella 'Movimenti_Dettagli'
    'CONTABILIZZATO POSITIVO -> IL COM+ GESTISCE LE GIACENZE

    Public Enum CoreEnum_Contabilizzato
        Contabilizzato_NonContabile = 1
        Contabilizzato_Contabile = 2
    End Enum

    '---------------------------------------------------------------------------------------------

    'Impostazione del campo Jolly:Int = Movimentazione di Magazzino

    'CASO DI DOCUMENTO ASSOCIATO A UN ALTRO (ES. BOLLE - FATTURE)
    'IL COMPONENTE NON DEVE GESTIRE LE GIACENZE
    'Dettaglio che non comporta movimentazione di magazzino. 
    'Si verifica cioè una delle seguenti ipotesi:
    'a.Movimento che riguarda Servizi o Parco Macchine
    'b.Movimento di Fattura Allegata a Bolla di Accompagnamento già movimentata precedentemente
    'jolly_int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
    'MagazzinoNONMovimentato As Integer = 1

    'CASO BUONO DI CARICO E SCARICO
    'IL COMPONENTE GESTISCE LE GIACENZE
    'jolly_int = enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato
    'MagazzinoMovimentato As Integer = 0

    Public Enum CoreEnum_MovimentazioneMagazzino
        Movimentazione_Movimentato = 0
        Movimentazione_NonMovimentato = 1
    End Enum



    Public Enum enum_TipoOperazioneDB
        Lettura = 0
        Scrittura = 1
        Modifica = 2
        Cancellazione = 3
        Trasferimento = 4
        Copia = 10
    End Enum







    '########################################################################################
    '########################################################################################
    '########################################################################################
    '########################################################################################
    '########################################################################################

    '########################################################################################
    '########################################################################################
    '########################################################################################
    '########################################################################################
    '########################################################################################











    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '/////  PARCO MACCHINE  ///////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////



    '########################################################################################
    Public Function DtForXml_Genera_ParcoMacchine() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Sa_Cod", GetType(String)))
            .Add(New DataColumn("Mac_Cod", GetType(String)))
            .Add(New DataColumn("Class_Code", GetType(String)))
            .Add(New DataColumn("Mac_Des", GetType(String)))
            .Add(New DataColumn("Costo_Acquisto", GetType(String)))
            .Add(New DataColumn("Targa", GetType(String)))
            .Add(New DataColumn("Telaio", GetType(String)))
            .Add(New DataColumn("Ditta_Cod", GetType(String)))
            .Add(New DataColumn("Modello", GetType(String)))
            .Add(New DataColumn("Potenza", GetType(String)))
            .Add(New DataColumn("Ammortamento", GetType(String)))
            .Add(New DataColumn("Ammortizzato", GetType(String)))
            .Add(New DataColumn("Data_Immatricolazione", GetType(String)))
            .Add(New DataColumn("Ultima_Manutenzione", GetType(String)))
            .Add(New DataColumn("Data_Revisione", GetType(String)))
            .Add(New DataColumn("Stato_Utilizzo", GetType(String)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))
            .Add(New DataColumn("BaseCode", GetType(String)))
            .Add(New DataColumn("TopCode", GetType(String)))
            .Add(New DataColumn("Note", GetType(String)))
            .Add(New DataColumn("Tipo", GetType(String)))
            .Add(New DataColumn("N_Immatricolazione", GetType(String)))
            .Add(New DataColumn("N_Immatricolazione_Rimorchio", GetType(String)))
            .Add(New DataColumn("N_Autorizzazione_Trasporto", GetType(String)))
            .Add(New DataColumn("Data_Rilascio_Autorizzazione", GetType(String)))
            .Add(New DataColumn("Peso", GetType(String)))
            .Add(New DataColumn("Mac_Cod_Origine", GetType(String)))
            .Add(New DataColumn("Piva_SuperUser_Origine", GetType(String)))
            .Add(New DataColumn("ChkDefault", GetType(String)))
            .Add(New DataColumn("Portata_Max", GetType(String)))
            .Add(New DataColumn("Cod_Contatto", GetType(String)))
            .Add(New DataColumn("Alimentazione_Cod", GetType(String)))
            .Add(New DataColumn("Potenza_Udm_Cod", GetType(String)))

            .Add(New DataColumn("CUAA_Proprietario", GetType(String)))
            .Add(New DataColumn("Denominazione_Proprietario", GetType(String)))
            .Add(New DataColumn("Tipo_Targa_Cod", GetType(String)))
            .Add(New DataColumn("Tipo_Trazione_Cod", GetType(String)))
            .Add(New DataColumn("N_Omologazione", GetType(String)))
            .Add(New DataColumn("Ditta_Cod_Motore", GetType(String)))
            .Add(New DataColumn("Tipo_Motore", GetType(String)))
            .Add(New DataColumn("Matricola_Motore", GetType(String)))
            .Add(New DataColumn("Data_Reimmatricolazione", GetType(String)))
            .Add(New DataColumn("Data_Carico", GetType(String)))
            .Add(New DataColumn("Data_Scarico", GetType(String)))
            .Add(New DataColumn("TitoloPossesso", GetType(String)))
            .Add(New DataColumn("Flag_Attrezzatura_Macchina", GetType(String)))
            .Add(New DataColumn("Taratura_Ugello", GetType(Decimal)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function




    '########################################################################################
    Public Sub DtForXml_InserisciRiga_ParcoMacchine( _
                                ByRef DT As DataTable, _
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    Optional ByVal Piva As String = C_Piva_Default, _
                                    Optional ByVal Sa_Cod As Integer = 0, _
                                    Optional ByVal Mac_Cod As Integer = 0, _
                                    Optional ByVal Class_Code As String = "", _
                                    Optional ByVal Mac_Des As String = "", _
                                    Optional ByVal Costo_Acquisto As Decimal = 0, _
                                    Optional ByVal Targa As String = "", _
                                    Optional ByVal Telaio As String = "", _
                                    Optional ByVal Ditta_Cod As Integer = 0, _
                                    Optional ByVal Modello As String = "", _
                                    Optional ByVal Potenza As String = "", _
                                    Optional ByVal Ammortamento As Decimal = 0, _
                                    Optional ByVal Ammortizzato As Decimal = 0, _
                                    Optional ByVal Data_Immatricolazione As Date = C_AgroDataInizio, _
                                    Optional ByVal Ultima_Manutenzione As Date = C_AgroDataInizio, _
                                    Optional ByVal Data_Revisione As Date = C_AgroDataInizio, _
                                    Optional ByVal Stato_Utilizzo As String = "", _
                                    Optional ByVal Validita_Inizio As Date = C_AgroDataInizio, _
                                    Optional ByVal Validita_Fine As Date = C_AgroDataFine, _
                                    Optional ByVal BaseCode As Integer = C_BaseCode_Default, _
                                    Optional ByVal TopCode As Integer = C_TopCode_Default, _
                                    Optional ByVal Note As String = "", _
                                    Optional ByVal Tipo As Integer = 0, _
                                    Optional ByVal N_Immatricolazione As String = "", _
                                    Optional ByVal N_Immatricolazione_Rimorchio As String = "", _
                                    Optional ByVal N_Autorizzazione_Trasporto As String = "", _
                                    Optional ByVal Data_Rilascio_Autorizzazione As Date = C_AgroDataInizio, _
                                    Optional ByVal Peso As Decimal = 0, _
                                    Optional ByVal Mac_Cod_Origine As Integer = 0, _
                                    Optional ByVal Piva_SuperUser_Origine As String = "", _
                                    Optional ByVal ChkDefault As Integer = 0, _
                                    Optional ByVal Portata_Max As Decimal = 0, _
                                    Optional ByVal Cod_Contatto As String = "", _
                                    Optional ByVal Alimentazione_Cod As Integer = 0, _
                                    Optional ByVal Potenza_Udm_Cod As Integer = 0, _
                                    Optional ByVal CUAA_Proprietario As String = "", _
                                    Optional ByVal Denominazione_Proprietario As String = "", _
                                    Optional ByVal Tipo_Targa_Cod As Integer = 0, _
                                    Optional ByVal Tipo_Trazione_Cod As Integer = 0, _
                                    Optional ByVal N_Omologazione As String = "", _
                                    Optional ByVal Ditta_Cod_Motore As Integer = 0, _
                                    Optional ByVal Tipo_Motore As String = "", _
                                    Optional ByVal Matricola_Motore As String = "", _
                                    Optional ByVal Data_Reimmatricolazione As Date = C_AgroDataInizio, _
                                    Optional ByVal Data_Carico As Date = C_AgroDataInizio, _
                                    Optional ByVal Data_Scarico As Date = C_AgroDataFine, _
                                    Optional ByVal TitoloPossesso As Integer = 0, _
                                    Optional ByVal Flag_Attrezzatura_Macchina As String = "M", _
                                    Optional ByVal Taratura_Ugello As Decimal = 0)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("Mac_Cod") = Mac_Cod
            .Item("Class_Code") = Class_Code
            .Item("Mac_Des") = Mac_Des
            .Item("Costo_Acquisto") = Costo_Acquisto
            .Item("Targa") = Targa
            .Item("Telaio") = Telaio
            .Item("Ditta_Cod") = Ditta_Cod
            .Item("Modello") = Modello
            .Item("Potenza") = Potenza
            .Item("Ammortamento") = Ammortamento
            .Item("Ammortizzato") = Ammortizzato
            .Item("Data_Immatricolazione") = Data_Immatricolazione
            .Item("Ultima_Manutenzione") = Ultima_Manutenzione
            .Item("Data_Revisione") = Data_Revisione
            .Item("Stato_Utilizzo") = Stato_Utilizzo
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode
            .Item("Note") = Note
            .Item("Tipo") = Tipo
            .Item("N_Immatricolazione") = N_Immatricolazione
            .Item("N_Immatricolazione_Rimorchio") = N_Immatricolazione_Rimorchio
            .Item("N_Autorizzazione_Trasporto") = N_Autorizzazione_Trasporto
            .Item("Data_Rilascio_Autorizzazione") = Data_Rilascio_Autorizzazione
            .Item("Peso") = Peso
            .Item("Mac_Cod_Origine") = Mac_Cod_Origine
            .Item("Piva_SuperUser_Origine") = Piva_SuperUser_Origine
            .Item("ChkDefault") = ChkDefault
            .Item("Portata_Max") = Portata_Max
            .Item("Cod_Contatto") = Cod_Contatto
            .Item("Alimentazione_Cod") = Alimentazione_Cod
            .Item("Potenza_Udm_Cod") = Potenza_Udm_Cod

            .Item("CUAA_Proprietario") = CUAA_Proprietario
            .Item("Denominazione_Proprietario") = Denominazione_Proprietario
            .Item("Tipo_Targa_Cod") = Tipo_Targa_Cod
            .Item("Tipo_Trazione_Cod") = Tipo_Trazione_Cod
            .Item("N_Omologazione") = N_Omologazione
            .Item("Ditta_Cod_Motore") = Ditta_Cod_Motore
            .Item("Tipo_Motore") = Tipo_Motore
            .Item("Matricola_Motore") = Matricola_Motore
            .Item("Data_Reimmatricolazione") = Data_Reimmatricolazione
            .Item("Data_Carico") = Data_Carico
            .Item("Data_Scarico") = Data_Scarico
            .Item("TitoloPossesso") = TitoloPossesso
            .Item("Flag_Attrezzatura_Macchina") = Flag_Attrezzatura_Macchina
            .Item("Taratura_Ugello") = Taratura_Ugello

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub




    '##########################################################################################
    Public Function XML_ParcoMacchine__DATI( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DT_ParcoMacchine As DataTable) _
                                        As XmlElement

        Dim RootXml As XmlElement = Nothing
        Dim DataXml As XmlElement
        Dim i As Integer

        Try '-----------------------------------------------------------------------------

            RootXml = XmlDoc.CreateElement("DatiParcoMacchine")

            For i = 0 To DT_ParcoMacchine.Rows.Count - 1

                DataXml = XML_ParcoMacchine_ParcoMacchine( _
                                                Log_Errori, _
                                                XmlDoc, _
                                                DT_ParcoMacchine.Rows(i))

                RootXml.AppendChild(DataXml)

            Next

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_ParcoMacchine__DATI) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return RootXml


    End Function






    '##########################################################################################
    Public Function XML_ParcoMacchine_ParcoMacchine( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_ParcoMacchine As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("ParcoMacchina")

            With DR_ParcoMacchine

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))
                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("Mac_Cod"), CStr(.Item("Mac_Cod")))
                DataXml.SetAttribute(LCase("Class_Code"), CStr(.Item("Class_Code")))
                DataXml.SetAttribute(LCase("Mac_Des"), CStr(.Item("Mac_Des")))
                DataXml.SetAttribute(LCase("Costo_Acquisto"), CStr(.Item("Costo_Acquisto")))
                DataXml.SetAttribute(LCase("Targa"), CStr(.Item("Targa")))
                DataXml.SetAttribute(LCase("Telaio"), CStr(.Item("Telaio")))
                DataXml.SetAttribute(LCase("Ditta_Cod"), CStr(.Item("Ditta_Cod")))
                DataXml.SetAttribute(LCase("Modello"), CStr(.Item("Modello")))
                DataXml.SetAttribute(LCase("Potenza"), CStr(.Item("Potenza")))
                DataXml.SetAttribute(LCase("Ammortamento"), CStr(.Item("Ammortamento")))
                DataXml.SetAttribute(LCase("Ammortizzato"), CStr(.Item("Ammortizzato")))
                DataXml.SetAttribute(LCase("Data_Immatricolazione"), Format(CDate(.Item("Data_Immatricolazione")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Ultima_Manutenzione"), Format(CDate(.Item("Ultima_Manutenzione")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Ultima_Revisione"), Format(CDate(.Item("Data_Revisione")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Stato_Utilizzo"), CStr(.Item("Stato_Utilizzo")))
                DataXml.SetAttribute(LCase("Validita_Inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))
                DataXml.SetAttribute(LCase("Note"), CStr(.Item("Note")))
                DataXml.SetAttribute(LCase("Tipo"), CStr(.Item("Tipo")))
                DataXml.SetAttribute(LCase("N_Immatricolazione"), CStr(.Item("N_Immatricolazione")))
                DataXml.SetAttribute(LCase("N_Immatricolazione_Rimorchio"), CStr(.Item("N_Immatricolazione_Rimorchio")))
                DataXml.SetAttribute(LCase("N_Autorizzazione_Trasporto"), CStr(.Item("N_Autorizzazione_Trasporto")))
                DataXml.SetAttribute(LCase("Data_Rilascio_Autorizzazione"), Format(CDate(.Item("Data_Rilascio_Autorizzazione")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Peso"), CStr(.Item("Peso")))
                DataXml.SetAttribute(LCase("Mac_Cod_Origine"), CStr(.Item("Mac_Cod_Origine")))
                DataXml.SetAttribute(LCase("Piva_SuperUser_Origine"), CStr(.Item("Piva_SuperUser_Origine")))
                DataXml.SetAttribute(LCase("ChkDefault"), CStr(.Item("ChkDefault")))
                DataXml.SetAttribute(LCase("Portata_Max"), CStr(.Item("Portata_Max")))
                DataXml.SetAttribute(LCase("Cod_Contatto"), CStr(.Item("Cod_Contatto")))
                DataXml.SetAttribute(LCase("Alimentazione_Cod"), CStr(.Item("Alimentazione_Cod")))
                DataXml.SetAttribute(LCase("Potenza_Udm_Cod"), CStr(.Item("Potenza_Udm_Cod")))

                DataXml.SetAttribute(LCase("CUAA_Proprietario"), CStr(.Item("CUAA_Proprietario")))
                DataXml.SetAttribute(LCase("Denominazione_Proprietario"), CStr(.Item("Denominazione_Proprietario")))
                DataXml.SetAttribute(LCase("Tipo_Targa_Cod"), CStr(.Item("Tipo_Targa_Cod")))
                DataXml.SetAttribute(LCase("Tipo_Trazione_Cod"), CStr(.Item("Tipo_Trazione_Cod")))
                DataXml.SetAttribute(LCase("N_Omologazione"), CStr(.Item("N_Omologazione")))
                DataXml.SetAttribute(LCase("Ditta_Cod_Motore"), CStr(.Item("Ditta_Cod_Motore")))
                DataXml.SetAttribute(LCase("Tipo_Motore"), CStr(.Item("Tipo_Motore")))
                DataXml.SetAttribute(LCase("Matricola_Motore"), CStr(.Item("Matricola_Motore")))
                DataXml.SetAttribute(LCase("Data_Reimmatricolazione"), CStr(.Item("Data_Reimmatricolazione")))
                DataXml.SetAttribute(LCase("Data_Carico"), CStr(.Item("Data_Carico")))
                DataXml.SetAttribute(LCase("Data_Scarico"), CStr(.Item("Data_Scarico")))
                DataXml.SetAttribute(LCase("TitoloPossesso"), CStr(.Item("TitoloPossesso")))
                DataXml.SetAttribute(LCase("Flag_Attrezzatura_Macchina"), CStr(.Item("Flag_Attrezzatura_Macchina")))
                DataXml.SetAttribute(LCase("Taratura_Ugello"), CStr(.Item("Taratura_Ugello")))

            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_ParcoMacchine_ParcoMacchine) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function







    '########################################################################################
    Public Function DtForXml_Genera_ParcoMacchineCodici() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
            .Add(New DataColumn("Id_Cod", GetType(String)))
            .Add(New DataColumn("Val_Cod", GetType(String)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function




    '########################################################################################
    Public Sub DtForXml_InserisciRiga_ParcoMacchineCodici( _
                                ByRef DT As DataTable, _
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    ByVal Id_Cod As Integer, _
                                    Optional ByVal Val_Cod As String = "", _
                                    Optional ByVal Validita_Inizio As Date = C_AgroDataInizio, _
                                    Optional ByVal Validita_Fine As Date = C_AgroDataFine)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Id_Cod") = Id_Cod
            .Item("Val_Cod") = Val_Cod
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub




    '##########################################################################################
    Public Function XML_ParcoMacchine_ParcoMacchineCodici( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_ParcoMacchineCodici As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("CodiceParcoMacchina")

            With DR_ParcoMacchineCodici

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))
                DataXml.SetAttribute(LCase("Id_Cod"), .Item("Id_Cod"))
                DataXml.SetAttribute(LCase("Val_Cod"), .Item("Val_Cod"))
                DataXml.SetAttribute(LCase("Validita_Inizio"), .Item("Validita_Inizio"))
                DataXml.SetAttribute(LCase("Validita_Fine"), .Item("Validita_Fine"))

            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_ParcoMacchine_ParcoMacchineCodici) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function



    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '/////  AGENDA  /  MOV DESTINAZIONI  ////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////



    '########################################################################################
    Public Function DtForXml_Genera_MovDestinazioni() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Sa_Cod", GetType(String)))
            .Add(New DataColumn("ID_Agenda", GetType(String)))
            .Add(New DataColumn("ID_Mov", GetType(String)))
            .Add(New DataColumn("ID_Mov_Det", GetType(String)))
            .Add(New DataColumn("Appezza", GetType(String)))
            .Add(New DataColumn("ID_Destinazione", GetType(String)))
            .Add(New DataColumn("Tipo_Destinazione", GetType(String)))
            .Add(New DataColumn("Qta", GetType(String)))
            .Add(New DataColumn("Qta2", GetType(String)))
            .Add(New DataColumn("Tipo_Scorta", GetType(String)))
            .Add(New DataColumn("Scorta_Min", GetType(String)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))
            .Add(New DataColumn("BaseCode", GetType(String)))
            .Add(New DataColumn("TopCode", GetType(String)))

            .Add(New DataColumn("QuotaDestribuzione", GetType(String)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function


    '########################################################################################
    Public Sub DtForXml_InserisciRiga_MovDestinazioni( _
                                                        ByRef DT As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        Optional ByVal Piva As String = "00000000000", _
                                                        Optional ByVal Sa_Cod As Integer = 0, _
                                                        Optional ByVal ID_Agenda As Integer = 0, _
                                                        Optional ByVal ID_Mov As Integer = 0, _
                                                        Optional ByVal ID_Mov_Det As Integer = 0, _
                                                        Optional ByVal Appezza As Integer = 0, _
                                                        Optional ByVal ID_Destinazione As Integer = 0, _
                                                        Optional ByVal Tipo_Destinazione As Integer = 0, _
                                                        Optional ByVal Qta As Decimal = 0, _
                                                        Optional ByVal Qta2 As Decimal = 0, _
                                                        Optional ByVal Tipo_Scorta As Integer = 0, _
                                                        Optional ByVal Scorta_Min As Decimal = 0, _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                                        Optional ByVal BaseCode As Integer = 0, _
                                                        Optional ByVal TopCode As Integer = 200000000, _
                                                        Optional ByVal QuotaDistribuzione As Decimal = 0)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("ID_Agenda") = ID_Agenda
            .Item("ID_Mov") = ID_Mov
            .Item("ID_Mov_Det") = ID_Mov_Det
            .Item("Appezza") = Appezza
            .Item("Id_Destinazione") = ID_Destinazione
            .Item("Tipo_Destinazione") = Tipo_Destinazione
            .Item("Qta") = Qta
            .Item("Qta2") = Qta2
            .Item("Tipo_Scorta") = Tipo_Scorta
            .Item("Scorta_Min") = Scorta_Min
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode

            .Item("QuotaDestribuzione") = QuotaDistribuzione

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub


    '##########################################################################################
    Public Function XML_Agenda_MovDestinazioni( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_MovDestinazioni As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("Movimento_Destinazione")

            With DR_MovDestinazioni

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("id_agenda"), CStr(.Item("ID_Agenda")))
                DataXml.SetAttribute(LCase("ID_Mov"), CStr(.Item("ID_Mov")))
                DataXml.SetAttribute(LCase("ID_Mov_Det"), CStr(.Item("ID_Mov_Det")))
                DataXml.SetAttribute(LCase("Appezza"), CStr(.Item("Appezza")))
                DataXml.SetAttribute(LCase("id_destinazione"), CStr(.Item("Id_Destinazione")))
                DataXml.SetAttribute(LCase("Tipo_Destinazione"), CStr(.Item("Tipo_Destinazione")))
                DataXml.SetAttribute(LCase("Qta"), CStr(.Item("Qta")))
                DataXml.SetAttribute(LCase("Qta2"), CStr(.Item("Qta2")))
                DataXml.SetAttribute(LCase("Tipo_Scorta"), CStr(.Item("Tipo_Scorta")))
                DataXml.SetAttribute(LCase("Scorta_Min"), CStr(.Item("Scorta_Min")))
                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))

                DataXml.SetAttribute(LCase("QuotaDestribuzione"), CStr(.Item("QuotaDestribuzione")))

            End With




        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_MovDestinazioni) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml



    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '/////  AGENDA  /  MOV DESTINAZIONI  ////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////



    '########################################################################################
    Public Function DtForXml_Genera_MovDettagliRiferimenti() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Sa_Cod", GetType(String)))
            .Add(New DataColumn("ID_Agenda", GetType(String)))
            .Add(New DataColumn("ID_Mov", GetType(String)))
            .Add(New DataColumn("ID_Mov_Det", GetType(String)))
            .Add(New DataColumn("lav_cod", GetType(String)))
            .Add(New DataColumn("cau_mov", GetType(String)))

            .Add(New DataColumn("Piva_Rif", GetType(String)))
            .Add(New DataColumn("Sa_Cod_Rif", GetType(String)))
            .Add(New DataColumn("ID_Agenda_Rif", GetType(String)))
            .Add(New DataColumn("ID_Mov_Rif", GetType(String)))
            .Add(New DataColumn("ID_Mov_Det_Rif", GetType(String)))
            .Add(New DataColumn("lav_cod_rif", GetType(String)))
            .Add(New DataColumn("cau_mov_rif", GetType(String)))

            .Add(New DataColumn("Qta", GetType(String)))

            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))
            .Add(New DataColumn("BaseCode", GetType(String)))
            .Add(New DataColumn("TopCode", GetType(String)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function

    '########################################################################################
    Public Sub DtForXml_InserisciRiga_MovDettagliRiferimenti( _
                                                        ByRef DT As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        Optional ByVal Piva As String = "00000000000", _
                                                        Optional ByVal Sa_Cod As Integer = 0, _
                                                        Optional ByVal ID_Agenda As Integer = 0, _
                                                        Optional ByVal ID_Mov As Integer = 0, _
                                                        Optional ByVal ID_Mov_Det As Integer = 0, _
                                                        Optional ByVal Lav_Cod As Integer = 0, _
                                                        Optional ByVal Cau_Mov As String = "", _
                                                        Optional ByVal Piva_Rif As String = "00000000000", _
                                                        Optional ByVal Sa_Cod_Rif As Integer = 0, _
                                                        Optional ByVal ID_Agenda_Rif As Integer = 0, _
                                                        Optional ByVal ID_Mov_Rif As Integer = 0, _
                                                        Optional ByVal ID_Mov_Det_Rif As Integer = 0, _
                                                        Optional ByVal Lav_Cod_Rif As Integer = 0, _
                                                        Optional ByVal Cau_Mov_Rif As String = "", _
                                                        Optional ByVal Qta As Decimal = 0, _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                                        Optional ByVal BaseCode As Integer = 0, _
                                                        Optional ByVal TopCode As Integer = 200000000)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("ID_Agenda") = ID_Agenda
            .Item("ID_Mov") = ID_Mov
            .Item("ID_Mov_Det") = ID_Mov_Det
            .Item("Lav_Cod") = Lav_Cod
            .Item("Cau_Mov") = Cau_Mov

            .Item("Piva_Rif") = Piva_Rif
            .Item("Sa_Cod_Rif") = Sa_Cod_Rif
            .Item("ID_Agenda_Rif") = ID_Agenda_Rif
            .Item("ID_Mov_Rif") = ID_Mov_Rif
            .Item("ID_Mov_Det_Rif") = ID_Mov_Det_Rif
            .Item("Lav_Cod_Rif") = Lav_Cod_Rif
            .Item("Cau_Mov_Rif") = Cau_Mov_Rif

            .Item("Qta") = Qta
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub


    '##########################################################################################
    Public Function XML_Agenda_MovDettagliRiferimenti( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_MovDettRif As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("Movimento_Riferimento2")

            With DR_MovDettRif

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("id_agenda"), CStr(.Item("ID_Agenda")))
                DataXml.SetAttribute(LCase("id_mov"), CStr(.Item("ID_Mov")))
                DataXml.SetAttribute(LCase("id_mov_det"), CStr(.Item("ID_Mov_Det")))
                DataXml.SetAttribute(LCase("lav_cod"), CStr(.Item("lav_cod")))
                DataXml.SetAttribute(LCase("cau_mov"), CStr(.Item("cau_mov")))

                DataXml.SetAttribute(LCase("piva_rif"), .Item("Piva_rif"))
                DataXml.SetAttribute(LCase("sa_cod_rif"), CStr(.Item("Sa_Cod_rif")))
                DataXml.SetAttribute(LCase("id_agenda_rif"), CStr(.Item("ID_Agenda_rif")))
                DataXml.SetAttribute(LCase("id_mov_rif"), CStr(.Item("ID_Mov_rif")))
                DataXml.SetAttribute(LCase("id_mov_det_rif"), CStr(.Item("ID_Mov_Det_rif")))
                DataXml.SetAttribute(LCase("lav_cod_rif"), CStr(.Item("lav_cod_rif")))
                DataXml.SetAttribute(LCase("cau_mov_rif"), CStr(.Item("cau_mov_rif")))

                DataXml.SetAttribute(LCase("Qta"), CStr(.Item("Qta")))
                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))

            End With

        Catch ex As Exception '---------------------------------------------------------------
            Log_Errori += " (XML_Agenda_MovDettagliRiferimenti) : " & ex.Message
        End Try '-----------------------------------------------------------------------------

        Return DataXml



    End Function


    '##########################################################################################
    Public Function XML_Agenda_MovDestinazioni__DATI( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DT_MovDestinazioni As DataTable) _
                                        As XmlElement

        Dim RootXml As XmlElement = Nothing
        Dim DataXml As XmlElement
        Dim i As Integer

        Try '-----------------------------------------------------------------------------

            RootXml = XmlDoc.CreateElement("DatiMov_Destinazioni")

            For i = 0 To DT_MovDestinazioni.Rows.Count - 1

                DataXml = XML_Agenda_MovDestinazioni( _
                                                Log_Errori, _
                                                XmlDoc, _
                                                DT_MovDestinazioni.Rows(i))

                RootXml.AppendChild(DataXml)

            Next

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_MovDestinazioni__DATI) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return RootXml

    End Function




    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '/////  AGENDA  /  MOVIMENTI DETTAGLI  ////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////



    '########################################################################################
    Public Function DtForXml_Genera_MovimentiDettagli() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Sa_Cod", GetType(String)))
            .Add(New DataColumn("ID_Agenda", GetType(String)))
            .Add(New DataColumn("ID_Mov", GetType(String)))
            .Add(New DataColumn("ID_Mov_Det", GetType(String)))
            .Add(New DataColumn("Mov_Det_Des", GetType(String)))
            .Add(New DataColumn("Elem_Cod", GetType(String)))
            .Add(New DataColumn("Pro_Cod", GetType(String)))
            .Add(New DataColumn("Mat_Cod", GetType(String)))
            .Add(New DataColumn("Cod_Progetto", GetType(String)))
            .Add(New DataColumn("Fase_Cod", GetType(String)))
            .Add(New DataColumn("Lotto", GetType(String)))
            .Add(New DataColumn("Cal_Cod", GetType(String)))
            .Add(New DataColumn("Udm_Cod", GetType(String)))
            .Add(New DataColumn("Udm_Cod_Extra", GetType(String)))
            .Add(New DataColumn("Qta", GetType(String)))
            .Add(New DataColumn("Qta_Extra", GetType(String)))
            .Add(New DataColumn("Prezzo_Unitario", GetType(String)))
            .Add(New DataColumn("Prezzo_Unitario_Netto", GetType(String)))
            .Add(New DataColumn("Imponibile", GetType(String)))
            .Add(New DataColumn("Imponibile_Netto", GetType(String)))
            .Add(New DataColumn("Cod_IVA", GetType(String)))
            .Add(New DataColumn("IVA", GetType(String)))
            .Add(New DataColumn("Sconto", GetType(String)))
            .Add(New DataColumn("Prezzo_Effettivo", GetType(String)))
            .Add(New DataColumn("Anno", GetType(String)))
            .Add(New DataColumn("Ric_Cod", GetType(String)))
            .Add(New DataColumn("Cod_Conto", GetType(String)))
            .Add(New DataColumn("Jolly_Int", GetType(String)))
            .Add(New DataColumn("Contabilizzato", GetType(String)))
            .Add(New DataColumn("Pendente", GetType(String)))
            .Add(New DataColumn("Extra_Str", GetType(String)))
            .Add(New DataColumn("Extra_Int", GetType(String)))
            .Add(New DataColumn("Extra_Date", GetType(String)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))
            .Add(New DataColumn("ChkIva_Manuale", GetType(String)))
            .Add(New DataColumn("Cod_IvaIndetraibile", GetType(String)))
            .Add(New DataColumn("Qta_Extra_Totale", GetType(String)))
            .Add(New DataColumn("Tara", GetType(String)))
            .Add(New DataColumn("ChkLayOut_Hide", GetType(String)))
            .Add(New DataColumn("Variazione", GetType(String)))
            .Add(New DataColumn("Listino_Cod", GetType(String)))
            .Add(New DataColumn("BaseCode", GetType(String)))
            .Add(New DataColumn("TopCode", GetType(String)))



            .Add(New DataColumn("TempoCarenza", GetType(String)))
            .Add(New DataColumn("doseetichetta", GetType(String)))
            .Add(New DataColumn("principiattivi", GetType(String)))
            .Add(New DataColumn("classitossicologiche", GetType(String)))
            .Add(New DataColumn("doseetichetta_value", GetType(String)))
            .Add(New DataColumn("turno_cod", GetType(String)))
            .Add(New DataColumn("id_attivita", GetType(String)))
            .Add(New DataColumn("qualifica_cod", GetType(String)))
            .Add(New DataColumn("tariffa_cod", GetType(String)))


            .Add(New DataColumn("Sconto_Listino", GetType(String)))
            .Add(New DataColumn("Sconto_Modalita", GetType(String)))
            .Add(New DataColumn("Mat_Cod_Alias", GetType(String)))
            .Add(New DataColumn("Mezzo_Det", GetType(String)))
            .Add(New DataColumn("Sconto_Testo", GetType(String)))
            .Add(New DataColumn("Ric_Cod_Pat", GetType(String)))
            .Add(New DataColumn("Cod_Conto_Pat", GetType(String)))
            .Add(New DataColumn("Dettaglio_VegCod", GetType(String)))
            .Add(New DataColumn("Iva_Indetraibile", GetType(String)))
            .Add(New DataColumn("Iva_Indetraibile_Perc", GetType(String)))
            .Add(New DataColumn("Iva_Deto_Cod", GetType(String)))
            .Add(New DataColumn("Qta_Dettaglio1", GetType(String)))
            .Add(New DataColumn("Qta_Dettaglio2", GetType(String)))
            .Add(New DataColumn("Dettagli_Blocco_Flag", GetType(String)))
            .Add(New DataColumn("Dettagli_Blocco_Username", GetType(String)))
            .Add(New DataColumn("Dettagli_Blocco_Data", GetType(String)))
            .Add(New DataColumn("Ordine_Det", GetType(String)))
            .Add(New DataColumn("Deroga_Cod", GetType(String)))
            .Add(New DataColumn("Prezzo_Livello", GetType(String)))



            'Inserimento nella stringa Xml dei campi non presenti in Db 
            'ma utili (necessari) per la gestione delle giacenze

            .Add(New DataColumn("Id_Destinazione", GetType(String)))
            .Add(New DataColumn("Lav_Cod_Allegato", GetType(String)))
            .Add(New DataColumn("Cau_Mov", GetType(String)))

            .Add(New DataColumn("Dt_MovDettRif", GetType(DataTable)))


        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function




    '########################################################################################
    Public Sub DtForXml_InserisciRiga_MovimentiDettagli( _
                                    ByRef DT As DataTable, _
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    Optional ByVal Piva As String = C_Piva_Default, _
                                    Optional ByVal Sa_Cod As Integer = 0, _
                                    Optional ByVal ID_Agenda As Integer = 0, _
                                    Optional ByVal ID_Mov As Integer = 0, _
                                    Optional ByVal ID_Mov_Det As Integer = 0, _
                                    Optional ByVal Mov_Det_Des As String = "", _
                                    Optional ByVal Elem_Cod As Integer = 0, _
                                    Optional ByVal Pro_Cod As Integer = 0, _
                                    Optional ByVal Mat_Cod As Integer = 0, _
                                    Optional ByVal Cod_Progetto As Integer = 0, _
                                    Optional ByVal Fase_Cod As Integer = 0, _
                                    Optional ByVal Lotto As String = "", _
                                    Optional ByVal Cal_Cod As Integer = 0, _
                                    Optional ByVal Udm_Cod As Integer = 0, _
                                    Optional ByVal Udm_Cod_Extra As Integer = 0, _
                                    Optional ByVal Qta As Decimal = 0.0, _
                                    Optional ByVal Qta_Extra As Decimal = 0.0, _
                                    Optional ByVal Prezzo_Unitario As Decimal = 0.0, _
                                    Optional ByVal Prezzo_Unitario_Netto As Decimal = 0.0, _
                                    Optional ByVal Imponibile As Decimal = 0.0, _
                                    Optional ByVal Imponibile_Netto As Decimal = 0.0, _
                                    Optional ByVal Cod_IVA As Integer = 0, _
                                    Optional ByVal IVA As Decimal = 0.0, _
                                    Optional ByVal Sconto As Decimal = 0, _
                                    Optional ByVal Prezzo_Effettivo As Decimal = 0, _
                                    Optional ByVal Anno As Integer = 1900, _
                                    Optional ByVal Ric_Cod As Integer = 0, _
                                    Optional ByVal Cod_Conto As Integer = 0, _
                                    Optional ByVal Jolly_Int As Integer = CoreEnum_MovimentazioneMagazzino.Movimentazione_Movimentato, _
                                    Optional ByVal Contabilizzato As Integer = CoreEnum_Contabilizzato.Contabilizzato_NonContabile, _
                                    Optional ByVal Pendente As Integer = CoreEnum_Pendenza.Pendenza_MovPendente, _
                                    Optional ByVal Extra_Str As String = "", _
                                    Optional ByVal Extra_Int As Integer = 0, _
                                    Optional ByVal Extra_Date As Date = C_AgroDataInizio, _
                                    Optional ByVal Validita_Inizio As Date = C_AgroDataInizio, _
                                    Optional ByVal Validita_Fine As Date = C_AgroDataFine, _
                                    Optional ByVal ChkIva_Manuale As Integer = 0, _
                                    Optional ByVal Cod_IvaIndetraibile As Integer = 0, _
                                    Optional ByVal Qta_Extra_Totale As Decimal = 0, _
                                    Optional ByVal Tara As Decimal = 0, _
                                    Optional ByVal ChkLayOut_Hide As Integer = 0, _
                                    Optional ByVal Variazione As Decimal = 0, _
                                    Optional ByVal Listino_Cod As Integer = 0, _
                                    Optional ByVal BaseCode As Integer = C_BaseCode_Default, _
                                    Optional ByVal TopCode As Integer = C_TopCode_Default, _
                                    Optional ByVal Id_Destinazione As Integer = 0, _
                                    Optional ByVal Lav_Cod_Allegato As Integer = 0, _
                                    Optional ByVal Cau_Mov As String = "", _
                                    Optional ByVal Dt_MovDettRif As DataTable = Nothing, _
                                    Optional ByVal TempoCarenza As Integer = 0, _
                                    Optional ByVal DoseEtichetta As String = "", _
                                    Optional ByVal PrincipiAttivi As String = "", _
                                    Optional ByVal ClassiTossicologiche As String = "", _
                                    Optional ByVal DoseEtichetta_Value As String = "", _
                                    Optional ByVal Turno_Cod As Integer = 0, _
                                    Optional ByVal Id_Attivita As Integer = 0, _
                                    Optional ByVal Qualifica_Cod As Integer = 0, _
                                    Optional ByVal Tariffa_Cod As Integer = 0, _
                                    Optional ByVal Sconto_Listino As Decimal = 0, _
                                    Optional ByVal Sconto_Modalita As Integer = 0, _
                                    Optional ByVal Mat_Cod_Alias As Integer = 0, _
                                    Optional ByVal Mezzo_Det As Integer = -1, _
                                    Optional ByVal Sconto_Testo As String = "", _
                                    Optional ByVal Ric_Cod_Pat As Integer = 0, _
                                    Optional ByVal Cod_Conto_Pat As Integer = 0, _
                                    Optional ByVal Dettaglio_VegCod As Integer = 0, _
                                    Optional ByVal Iva_Indetraibile As Decimal = 0, _
                                    Optional ByVal Iva_Indetraibile_Perc As Decimal = 0, _
                                    Optional ByVal Iva_Deto_Cod As Integer = 0, _
                                    Optional ByVal Qta_Dettaglio1 As Decimal = 0, _
                                    Optional ByVal Qta_Dettaglio2 As Decimal = 0, _
                                    Optional ByVal Dettagli_Blocco_Flag As Integer = 0, _
                                    Optional ByVal Dettagli_Blocco_Username As String = "0", _
                                    Optional ByVal Dettagli_Blocco_Data As DateTime = AGRODATAINIZIO, _
                                    Optional ByVal Ordine_Det As Integer = 0, _
                                    Optional ByVal Deroga_Cod As Integer = 0, _
                                    Optional ByVal Prezzo_Livello As Integer = 0 _
                                   )




        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("ID_Agenda") = ID_Agenda
            .Item("ID_Mov") = ID_Mov
            .Item("ID_Mov_Det") = ID_Mov_Det
            .Item("Mov_Det_Des") = Mov_Det_Des
            .Item("Elem_Cod") = Elem_Cod
            .Item("Pro_Cod") = Pro_Cod
            .Item("Mat_Cod") = Mat_Cod
            .Item("Cod_Progetto") = Cod_Progetto
            .Item("Fase_Cod") = Fase_Cod
            .Item("Lotto") = Lotto
            .Item("Cal_Cod") = Cal_Cod
            .Item("Udm_Cod") = Udm_Cod
            .Item("Udm_Cod_Extra") = Udm_Cod_Extra
            .Item("Qta") = Qta
            .Item("Qta_Extra") = Qta_Extra
            .Item("Prezzo_Unitario") = Prezzo_Unitario
            .Item("Prezzo_Unitario_Netto") = Prezzo_Unitario_Netto
            .Item("Imponibile") = Imponibile
            .Item("Imponibile_Netto") = Imponibile_Netto
            .Item("Cod_IVA") = Cod_IVA
            .Item("IVA") = IVA
            .Item("Sconto") = Sconto
            .Item("Prezzo_Effettivo") = Prezzo_Effettivo
            .Item("Anno") = Anno
            .Item("Ric_Cod") = Ric_Cod
            .Item("Cod_Conto") = Cod_Conto
            .Item("Jolly_Int") = Jolly_Int
            .Item("Contabilizzato") = Contabilizzato
            .Item("Pendente") = Pendente
            .Item("Extra_Str") = Extra_Str
            .Item("Extra_Int") = Extra_Int
            .Item("Extra_Date") = Extra_Date
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("ChkIva_Manuale") = ChkIva_Manuale
            .Item("Cod_IvaIndetraibile") = Cod_IvaIndetraibile
            .Item("Qta_Extra_Totale") = Qta_Extra_Totale
            .Item("Tara") = Tara
            .Item("ChkLayOut_Hide") = ChkLayOut_Hide
            .Item("Variazione") = Variazione
            .Item("Listino_Cod") = Listino_Cod
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode
            .Item("Id_Destinazione") = Id_Destinazione
            .Item("Lav_Cod_Allegato") = Lav_Cod_Allegato
            .Item("Cau_Mov") = Cau_Mov
            If Not IsNothing(Dt_MovDettRif) Then
                .Item("Dt_MovDettRif") = Dt_MovDettRif
            End If

            .Item("TempoCarenza") = TempoCarenza
            .Item("doseetichetta") = DoseEtichetta
            .Item("principiattivi") = PrincipiAttivi
            .Item("classitossicologiche") = ClassiTossicologiche
            .Item("doseetichetta_value") = DoseEtichetta_Value
            .Item("turno_cod") = Turno_Cod
            .Item("id_attivita") = Id_Attivita
            .Item("qualifica_cod") = Qualifica_Cod
            .Item("tariffa_cod") = Tariffa_Cod


            .Item("Sconto_Listino") = Sconto_Listino
            .Item("Sconto_Modalita") = Sconto_Modalita
            .Item("Mat_Cod_Alias") = Mat_Cod_Alias
            .Item("Mezzo_Det") = Mezzo_Det
            .Item("Sconto_Testo") = Sconto_Testo
            .Item("Ric_Cod_Pat") = Ric_Cod_Pat
            .Item("Cod_Conto_Pat") = Cod_Conto_Pat
            .Item("Dettaglio_VegCod") = Dettaglio_VegCod
            .Item("Iva_Indetraibile") = Iva_Indetraibile
            .Item("Iva_Indetraibile_Perc") = Iva_Indetraibile_Perc
            .Item("Iva_Deto_Cod") = Iva_Deto_Cod
            .Item("Qta_Dettaglio1") = Qta_Dettaglio1
            .Item("Qta_Dettaglio2") = Qta_Dettaglio2
            .Item("Dettagli_Blocco_Flag") = Dettagli_Blocco_Flag
            .Item("Dettagli_Blocco_Username") = Dettagli_Blocco_Username
            .Item("Dettagli_Blocco_Data") = Dettagli_Blocco_Data
            .Item("Ordine_Det") = Ordine_Det
            .Item("Deroga_Cod") = Deroga_Cod
            .Item("Prezzo_Livello") = Prezzo_Livello


        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub



    '##########################################################################################
    Public Function XML_Agenda_MovimentiDettagli( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_MovimentiDettagli As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("Movimento_Dettaglio")

            With DR_MovimentiDettagli

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("id_agenda"), CStr(.Item("ID_Agenda")))
                DataXml.SetAttribute(LCase("ID_Mov"), CStr(.Item("ID_Mov")))
                DataXml.SetAttribute(LCase("ID_Mov_Det"), CStr(.Item("ID_Mov_Det")))
                DataXml.SetAttribute(LCase("Mov_Det_Des"), .Item("Mov_Det_Des"))
                DataXml.SetAttribute(LCase("Elem_Cod"), CStr(.Item("Elem_Cod")))
                DataXml.SetAttribute(LCase("Pro_Cod"), CStr(.Item("Pro_Cod")))
                DataXml.SetAttribute(LCase("Mat_Cod"), CStr(.Item("Mat_Cod")))
                DataXml.SetAttribute(LCase("Cod_Progetto"), CStr(.Item("Cod_Progetto")))
                DataXml.SetAttribute(LCase("Fase_Cod"), CStr(.Item("Fase_Cod")))
                DataXml.SetAttribute(LCase("Lotto"), .Item("Lotto"))
                DataXml.SetAttribute(LCase("Cal_Cod"), CStr(.Item("Cal_Cod")))
                DataXml.SetAttribute(LCase("Udm_Cod"), CStr(.Item("Udm_Cod")))
                DataXml.SetAttribute(LCase("udm_cod_extra"), CStr(.Item("Udm_Cod_Extra")))
                DataXml.SetAttribute(LCase("Qta"), CStr(.Item("Qta")))
                DataXml.SetAttribute(LCase("qta_extra"), CStr(.Item("Qta_Extra")))
                DataXml.SetAttribute(LCase("Prezzo_Unitario"), CStr(.Item("Prezzo_Unitario")))
                DataXml.SetAttribute(LCase("Prezzo_Unitario_Netto"), CStr(.Item("Prezzo_Unitario_Netto")))
                DataXml.SetAttribute(LCase("Imponibile"), CStr(.Item("Imponibile")))
                DataXml.SetAttribute(LCase("Imponibile_Netto"), CStr(.Item("Imponibile_Netto")))
                DataXml.SetAttribute(LCase("Cod_IVA"), CStr(.Item("Cod_IVA")))
                DataXml.SetAttribute(LCase("Iva"), CStr(.Item("IVA")))
                DataXml.SetAttribute(LCase("Sconto"), CStr(.Item("Sconto")))
                DataXml.SetAttribute(LCase("Prezzo_Effettivo"), CStr(.Item("Prezzo_Effettivo")))
                DataXml.SetAttribute(LCase("Anno"), CStr(.Item("Anno")))
                DataXml.SetAttribute(LCase("Ric_Cod"), CStr(.Item("Ric_Cod")))
                DataXml.SetAttribute(LCase("Cod_Conto"), CStr(.Item("Cod_Conto")))
                DataXml.SetAttribute(LCase("jolly_Int"), CStr(.Item("Jolly_Int")))
                DataXml.SetAttribute(LCase("Contabilizzato"), CStr(.Item("Contabilizzato")))
                DataXml.SetAttribute(LCase("Pendente"), CStr(.Item("Pendente")))
                DataXml.SetAttribute(LCase("Extra_Str"), .Item("Extra_Str"))
                DataXml.SetAttribute(LCase("Extra_Int"), CStr(.Item("Extra_Int")))
                DataXml.SetAttribute(LCase("Extra_Date"), CStr(.Item("Extra_Date")))
                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("ChkIva_Manuale"), CStr(.Item("ChkIva_Manuale")))
                DataXml.SetAttribute(LCase("Cod_IvaIndetraibile"), CStr(.Item("Cod_IvaIndetraibile")))
                DataXml.SetAttribute(LCase("Qta_Extra_Totale"), CStr(.Item("Qta_Extra_Totale")))
                DataXml.SetAttribute(LCase("Tara"), CStr(.Item("Tara")))
                DataXml.SetAttribute(LCase("ChkLayOut_Hide"), CStr(.Item("ChkLayOut_Hide")))
                DataXml.SetAttribute(LCase("Variazione"), CStr(.Item("Variazione")))
                DataXml.SetAttribute(LCase("Listino_Cod"), CStr(.Item("Listino_Cod")))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))

                DataXml.SetAttribute(LCase("TempoCarenza"), CStr(.Item("TempoCarenza")))
                DataXml.SetAttribute(LCase("doseetichetta"), CStr(.Item("doseetichetta")))
                DataXml.SetAttribute(LCase("principiattivi"), CStr(.Item("principiattivi")))
                DataXml.SetAttribute(LCase("classitossicologiche"), CStr(.Item("classitossicologiche")))
                DataXml.SetAttribute(LCase("doseetichetta_value"), CStr(.Item("doseetichetta_value")))
                DataXml.SetAttribute(LCase("id_attivita"), CStr(.Item("id_attivita")))
                DataXml.SetAttribute(LCase("qualifica_cod"), CStr(.Item("qualifica_cod")))
                DataXml.SetAttribute(LCase("tariffa_cod"), CStr(.Item("tariffa_cod")))


                DataXml.SetAttribute(LCase("Sconto_Listino"), CStr(.Item("Sconto_Listino")))
                DataXml.SetAttribute(LCase("Sconto_Modalita"), CStr(.Item("Sconto_Modalita")))
                DataXml.SetAttribute(LCase("Mat_Cod_Alias"), CStr(.Item("Mat_Cod_Alias")))
                DataXml.SetAttribute(LCase("Mezzo_Det"), CStr(.Item("Mezzo_Det")))
                DataXml.SetAttribute(LCase("Sconto_Testo"), CStr(.Item("Sconto_Testo")))
                DataXml.SetAttribute(LCase("Ric_Cod_Pat"), CStr(.Item("Ric_Cod_Pat")))
                DataXml.SetAttribute(LCase("Cod_Conto_Pat"), CStr(.Item("Cod_Conto_Pat")))
                DataXml.SetAttribute(LCase("Dettaglio_VegCod"), CStr(.Item("Dettaglio_VegCod")))
                DataXml.SetAttribute(LCase("Iva_Indetraibile"), CStr(.Item("Iva_Indetraibile")))
                DataXml.SetAttribute(LCase("Iva_Indetraibile_Perc"), CStr(.Item("Iva_Indetraibile_Perc")))
                DataXml.SetAttribute(LCase("Iva_Deto_Cod"), CStr(.Item("Iva_Deto_Cod")))
                DataXml.SetAttribute(LCase("Qta_Dettaglio1"), CStr(.Item("Qta_Dettaglio1")))
                DataXml.SetAttribute(LCase("Qta_Dettaglio2"), CStr(.Item("Qta_Dettaglio2")))
                DataXml.SetAttribute(LCase("Dettagli_Blocco_Flag"), CStr(.Item("Dettagli_Blocco_Flag")))
                DataXml.SetAttribute(LCase("Dettagli_Blocco_Username"), CStr(.Item("Dettagli_Blocco_Username")))
                DataXml.SetAttribute(LCase("Dettagli_Blocco_Data"), Format(CDate(.Item("Dettagli_Blocco_Data")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Ordine_Det"), CStr(.Item("Ordine_Det")))
                DataXml.SetAttribute(LCase("Deroga_Cod"), CStr(.Item("Deroga_Cod")))
                DataXml.SetAttribute(LCase("Prezzo_Livello"), CStr(.Item("Prezzo_Livello")))




                'Inserimento nella stringa Xml dei campi non presenti in Db 
                'ma utili (necessari) per la gestione delle giacenze

                DataXml.SetAttribute(LCase("id_destinazione"), CStr(.Item("Id_Destinazione")))
                DataXml.SetAttribute(LCase("lav_cod"), CStr(.Item("Lav_Cod_Allegato"))) 'Nota: Utile in FormFattura
                DataXml.SetAttribute(LCase("cau_mov"), .Item("Cau_Mov")) 'IMPORTANTE!!!!!



            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_MovimentiDettagli) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function




    '##########################################################################################
    'DT_MovimentiDettagli e DT_MovDestinazioni devono avere lo stesso numero di righe!
    Public Function XML_Agenda_MovimentiDettagli__DATI( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DT_MovimentiDettagli As DataTable, _
                                    Optional ByVal Flag_MovDestinazione As Boolean = False, _
                                    Optional ByRef DT_MovDestinazioni As DataTable = Nothing) _
                                        As XmlElement

        Dim RootXml As XmlElement = Nothing
        Dim DataXml As XmlElement
        Dim Data1Xml As XmlElement
        Dim Data2Xml As XmlElement
        Dim i As Integer
        Dim DT_MovDettRif As DataTable

        Try '-----------------------------------------------------------------------------

            RootXml = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

            For i = 0 To DT_MovimentiDettagli.Rows.Count - 1

                DataXml = XML_Agenda_MovimentiDettagli( _
                                                Log_Errori, _
                                                XmlDoc, _
                                                DT_MovimentiDettagli.Rows(i))

                RootXml.AppendChild(DataXml)



                If Not IsNothing(DT_MovimentiDettagli.Rows(i).Item("Dt_MovDettRif")) _
                    AndAlso Not IsDBNull(DT_MovimentiDettagli.Rows(i).Item("Dt_MovDettRif")) Then

                    DT_MovDettRif = DT_MovimentiDettagli.Rows(i).Item("Dt_MovDettRif")

                    If DT_MovDettRif.Rows.Count > 0 Then

                        Data1Xml = XML_Agenda_MovDettagliRiferimenti( _
                                                  Log_Errori, _
                                                  XmlDoc, _
                                                  DT_MovDettRif.Rows(0))

                        DataXml.AppendChild(Data1Xml)

                    End If

                End If

                If Flag_MovDestinazione = True Then

                    Data2Xml = XML_Agenda_MovDestinazioni( _
                                                           Log_Errori, _
                                                           XmlDoc, _
                                                           DT_MovDestinazioni.Rows(i))

                    DataXml.AppendChild(Data2Xml)

                End If

            Next

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_MovimentiDettagli__DATI) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return RootXml

    End Function





    '########################################################################################
    Public Function DtForXml_Genera_MovimentiDettagliTecnici() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Sa_Cod", GetType(String)))
            .Add(New DataColumn("ID_Agenda", GetType(String)))
            .Add(New DataColumn("ID_Mov", GetType(String)))
            .Add(New DataColumn("ID_Mov_Det", GetType(String)))
            .Add(New DataColumn("id_reg_dettaglio", GetType(String)))
            .Add(New DataColumn("qta_ril", GetType(String)))
            .Add(New DataColumn("Data_Ril", GetType(String)))
            .Add(New DataColumn("Ditta_Cod", GetType(String)))
            .Add(New DataColumn("Dett_Cod", GetType(String)))
            .Add(New DataColumn("Id_Insetto", GetType(String)))
            .Add(New DataColumn("FF_Classe", GetType(String)))
            .Add(New DataColumn("Dose", GetType(String)))
            .Add(New DataColumn("Mg", GetType(String)))
            .Add(New DataColumn("n", GetType(String)))
            .Add(New DataColumn("p", GetType(String)))
            .Add(New DataColumn("k", GetType(String)))
            .Add(New DataColumn("Parziale", GetType(String)))
            .Add(New DataColumn("Nitrati", GetType(String)))
            .Add(New DataColumn("Freatimetro", GetType(String)))
            .Add(New DataColumn("piezo1", GetType(String)))
            .Add(New DataColumn("piezo2", GetType(String)))
            .Add(New DataColumn("piezo3", GetType(String)))
            .Add(New DataColumn("piezo4", GetType(String)))
            .Add(New DataColumn("Sigla_AV", GetType(String)))
            .Add(New DataColumn("Trap_Num", GetType(String)))
            .Add(New DataColumn("Inn1_Data", GetType(String)))
            .Add(New DataColumn("Inn2_Data", GetType(String)))
            .Add(New DataColumn("Inn3_Data", GetType(String)))
            .Add(New DataColumn("Inn4_Data", GetType(String)))
            .Add(New DataColumn("Av_Cod", GetType(String)))
            .Add(New DataColumn("Av_Gru", GetType(String)))
            .Add(New DataColumn("Lotto", GetType(String)))
            .Add(New DataColumn("Extra_Str", GetType(String)))
            .Add(New DataColumn("Extra_Int", GetType(String)))
            .Add(New DataColumn("Extra_Date", GetType(String)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))
            .Add(New DataColumn("BaseCode", GetType(String)))
            .Add(New DataColumn("TopCode", GetType(String)))
            .Add(New DataColumn("Efficienza", GetType(String)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function


    '########################################################################################
    Public Sub DtForXml_InserisciRiga_MovimentiDettagliTecnici( _
                                ByRef DT As DataTable, _
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    Optional ByVal Piva As String = C_Piva_Default, _
                                    Optional ByVal Sa_Cod As Integer = 0, _
                                    Optional ByVal ID_Agenda As Integer = 0, _
                                    Optional ByVal ID_Mov As Integer = 0, _
                                    Optional ByVal ID_Mov_Det As Integer = 0, _
                                    Optional ByVal ID_Reg_Dettaglio As Integer = 0, _
                                    Optional ByVal qta_ril As Decimal = 0, _
                                    Optional ByVal data_ril As Date = C_AgroDataInizio, _
                                    Optional ByVal Ditta_Cod As Integer = 0, _
                                    Optional ByVal Dett_Cod As Integer = 0, _
                                    Optional ByVal Id_Insetto As Integer = 0, _
                                    Optional ByVal FF_Classe As Integer = 0, _
                                    Optional ByVal Dose As Decimal = 0, _
                                    Optional ByVal Mg As Decimal = 0, _
                                    Optional ByVal N As Decimal = 0, _
                                    Optional ByVal P As Decimal = 0, _
                                    Optional ByVal K As Decimal = 0, _
                                    Optional ByVal Parziale As Integer = 0, _
                                    Optional ByVal Nitrati As Integer = 0, _
                                    Optional ByVal Freatimetro As Decimal = 0, _
                                    Optional ByVal Piezo1 As Decimal = 0, _
                                    Optional ByVal Piezo2 As Decimal = 0, _
                                    Optional ByVal Piezo3 As Decimal = 0, _
                                    Optional ByVal Piezo4 As Decimal = 0, _
                                    Optional ByVal Sigla_AV As String = "", _
                                    Optional ByVal Trap_Num As Integer = 0, _
                                    Optional ByVal Inn1_Data As Date = C_AgroDataInizio, _
                                    Optional ByVal Inn2_Data As Date = C_AgroDataInizio, _
                                    Optional ByVal Inn3_Data As Date = C_AgroDataInizio, _
                                    Optional ByVal Inn4_Data As Date = C_AgroDataInizio, _
                                    Optional ByVal Av_Cod As Integer = 0, _
                                    Optional ByVal Av_Gru As Integer = 0, _
                                    Optional ByVal Lotto As String = "", _
                                    Optional ByVal Extra_Str As String = "", _
                                    Optional ByVal Extra_Int As Integer = 0, _
                                    Optional ByVal Extra_Date As Date = C_AgroDataInizio, _
                                    Optional ByVal Validita_Inizio As Date = C_AgroDataInizio, _
                                    Optional ByVal Validita_Fine As Date = C_AgroDataFine, _
                                    Optional ByVal BaseCode As Integer = C_BaseCode_Default, _
                                    Optional ByVal TopCode As Integer = C_TopCode_Default, _
                                    Optional ByVal Efficienza As Decimal = 0)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("ID_Agenda") = ID_Agenda
            .Item("ID_Mov") = ID_Mov
            .Item("ID_Mov_Det") = ID_Mov_Det
            .Item("id_reg_dettaglio") = ID_Reg_Dettaglio

            .Item("qta_ril") = qta_ril
            .Item("Data_Ril") = data_ril
            .Item("Ditta_Cod") = Ditta_Cod
            .Item("Dett_Cod") = Dett_Cod
            .Item("Id_Insetto") = Id_Insetto
            .Item("FF_Classe") = FF_Classe
            .Item("Dose") = Dose
            .Item("Mg") = Mg
            .Item("n") = N
            .Item("p") = P
            .Item("k") = K
            .Item("Parziale") = Parziale
            .Item("Nitrati") = Nitrati
            .Item("Freatimetro") = Freatimetro
            .Item("piezo1") = Piezo1
            .Item("piezo2") = Piezo2
            .Item("piezo3") = Piezo3
            .Item("piezo4") = Piezo4
            .Item("Sigla_AV") = Sigla_AV
            .Item("Trap_Num") = Trap_Num
            .Item("Inn1_Data") = Inn1_Data
            .Item("Inn2_Data") = Inn2_Data
            .Item("Inn3_Data") = Inn3_Data
            .Item("Inn4_Data") = Inn4_Data
            .Item("Av_Cod") = Av_Cod
            .Item("Av_Gru") = Av_Gru
            .Item("Lotto") = Lotto

            .Item("Extra_Str") = Extra_Str
            .Item("Extra_Int") = Extra_Int
            .Item("Extra_Date") = Extra_Date
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode

            .Item("Efficienza") = Efficienza

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub



    '##########################################################################################
    Public Function XML_Agenda_MovimentiDettagliTecnici( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_MovimentiDettagliTecnici As DataRow, _
                                    Optional ByVal DettaglioTecnico2 As Boolean = False) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            If DettaglioTecnico2 Then
                DataXml = XmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_2")
            Else
                DataXml = XmlDoc.CreateElement("Movimento_Dettaglio_Tecnico")
            End If

            With DR_MovimentiDettagliTecnici

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("id_agenda"), CStr(.Item("ID_Agenda")))
                DataXml.SetAttribute(LCase("ID_Mov"), CStr(.Item("ID_Mov")))
                DataXml.SetAttribute(LCase("ID_Mov_Det"), CStr(.Item("ID_Mov_Det")))
                DataXml.SetAttribute(LCase("id_reg_dettaglio"), CStr(.Item("id_reg_dettaglio")))

                DataXml.SetAttribute(LCase("qta_ril"), CStr(.Item("qta_ril")))
                DataXml.SetAttribute(LCase("Data_Ril"), CStr(.Item("Data_Ril")))
                DataXml.SetAttribute(LCase("Ditta_Cod"), CStr(.Item("Ditta_Cod")))
                DataXml.SetAttribute(LCase("Dett_Cod"), CStr(.Item("Dett_Cod")))
                DataXml.SetAttribute(LCase("Id_Insetto"), CStr(.Item("Id_Insetto")))
                DataXml.SetAttribute(LCase("FF_Classe"), CStr(.Item("FF_Classe")))
                DataXml.SetAttribute(LCase("Dose"), CStr(.Item("Dose")))
                DataXml.SetAttribute(LCase("Mg"), CStr(.Item("Mg")))
                DataXml.SetAttribute(LCase("n"), CStr(.Item("n")))
                DataXml.SetAttribute(LCase("p"), CStr(.Item("p")))
                DataXml.SetAttribute(LCase("k"), CStr(.Item("k")))
                DataXml.SetAttribute(LCase("Parziale"), CStr(.Item("Parziale")))
                DataXml.SetAttribute(LCase("Nitrati"), CStr(.Item("Nitrati")))
                DataXml.SetAttribute(LCase("Freatimetro"), CStr(.Item("Freatimetro")))
                DataXml.SetAttribute(LCase("piezo1"), CStr(.Item("piezo1")))
                DataXml.SetAttribute(LCase("piezo2"), CStr(.Item("piezo2")))
                DataXml.SetAttribute(LCase("piezo3"), CStr(.Item("piezo3")))
                DataXml.SetAttribute(LCase("piezo4"), CStr(.Item("piezo4")))
                DataXml.SetAttribute(LCase("Sigla_AV"), CStr(.Item("Sigla_AV")))
                DataXml.SetAttribute(LCase("Trap_Num"), CStr(.Item("Trap_Num")))
                DataXml.SetAttribute(LCase("Inn1_Data"), CStr(.Item("Inn1_Data")))
                DataXml.SetAttribute(LCase("Inn2_Data"), CStr(.Item("Inn2_Data")))
                DataXml.SetAttribute(LCase("Inn3_Data"), CStr(.Item("Inn3_Data")))
                DataXml.SetAttribute(LCase("Inn4_Data"), CStr(.Item("Inn4_Data")))
                DataXml.SetAttribute(LCase("Av_Cod"), CStr(.Item("Av_Cod")))
                DataXml.SetAttribute(LCase("Av_Gru"), CStr(.Item("Av_Gru")))
                DataXml.SetAttribute(LCase("Lotto"), CStr(.Item("Lotto")))
                DataXml.SetAttribute(LCase("Extra_Str"), CStr(.Item("Extra_Str")))
                DataXml.SetAttribute(LCase("Extra_Int"), CStr(.Item("Extra_Int")))
                DataXml.SetAttribute(LCase("Extra_Date"), CStr(.Item("Extra_Date")))

                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))

                DataXml.SetAttribute(LCase("efficienza"), CStr(.Item("Efficienza")))


            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_MovimentiDettagliTecnici) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function


    '##########################################################################################
    Public Function XML_Agenda_MovimentiDettagliTecnici__DATI( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DT_MovimentiDettagliTecnici As DataTable) _
                                        As XmlElement

        Dim RootXml As XmlElement = Nothing
        Dim DataXml As XmlElement
        Dim i As Integer

        Try '-----------------------------------------------------------------------------

            RootXml = XmlDoc.CreateElement("DatiMov_Dettagli_Tecnici")

            For i = 0 To DT_MovimentiDettagliTecnici.Rows.Count - 1

                DataXml = XML_Agenda_MovimentiDettagliTecnici(Log_Errori, _
                                                                XmlDoc, _
                                                                DT_MovimentiDettagliTecnici.Rows(i))

                RootXml.AppendChild(DataXml)

            Next

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_MovimentiDettagliTenici__DATI) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return RootXml

    End Function


    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '/////  AGENDA  /  MOVIMENTI //////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////



    '########################################################################################
    Public Function DtForXml_Genera_Movimenti() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))

            .Add(New DataColumn("piva", GetType(String)))
            .Add(New DataColumn("sa_cod", GetType(String)))
            .Add(New DataColumn("id_agenda", GetType(String)))
            .Add(New DataColumn("ID_Mov", GetType(String)))
            .Add(New DataColumn("Cau_Mov", GetType(String)))
            .Add(New DataColumn("Mov_Desc", GetType(String)))
            .Add(New DataColumn("Data_Movimento", GetType(String)))
            .Add(New DataColumn("ora", GetType(String)))
            .Add(New DataColumn("Scadenza", GetType(String)))
            .Add(New DataColumn("Scadenza_Extra", GetType(String)))
            .Add(New DataColumn("Doc_Numero_Sin", GetType(String)))
            .Add(New DataColumn("Doc_Numero", GetType(String)))
            .Add(New DataColumn("Doc_Numero_Des", GetType(String)))
            .Add(New DataColumn("Num_Protocollo", GetType(String)))
            .Add(New DataColumn("colli", GetType(String)))
            .Add(New DataColumn("peso", GetType(String)))
            .Add(New DataColumn("Aspetto", GetType(String)))
            .Add(New DataColumn("Causale_Trasporto", GetType(String)))
            .Add(New DataColumn("Tipo_Sconto", GetType(String)))
            .Add(New DataColumn("Cod_RisUm", GetType(String)))
            .Add(New DataColumn("Cod_IndirizzoRisUm", GetType(String)))
            .Add(New DataColumn("Cod_Destinazione", GetType(String)))
            .Add(New DataColumn("Cod_IndirizzoDestinazione", GetType(String)))
            .Add(New DataColumn("Mezzo", GetType(String)))
            .Add(New DataColumn("Cod_Vettore", GetType(String)))
            .Add(New DataColumn("Cod_IndirizzoVettore", GetType(String)))
            .Add(New DataColumn("Natura_Beni", GetType(String)))
            .Add(New DataColumn("Modalita", GetType(String)))
            .Add(New DataColumn("Tara_Veicolo", GetType(String)))
            .Add(New DataColumn("Tara_Imballi", GetType(String)))
            .Add(New DataColumn("Tipo_Peso", GetType(String)))
            .Add(New DataColumn("Username_Note", GetType(String)))
            .Add(New DataColumn("Extra_Str", GetType(String)))
            .Add(New DataColumn("Extra_Int", GetType(String)))
            .Add(New DataColumn("Extra_Date", GetType(String)))
            .Add(New DataColumn("validita_inizio", GetType(String)))
            .Add(New DataColumn("validita_fine", GetType(String)))
            .Add(New DataColumn("basecode", GetType(String)))
            .Add(New DataColumn("topcode", GetType(String)))
            .Add(New DataColumn("Progr_Protocollo", GetType(String)))
            .Add(New DataColumn("Progr_Registrazione", GetType(String)))
            .Add(New DataColumn("Data_Registrazione", GetType(String)))
            .Add(New DataColumn("ChkLayOut_Bypass_Fatturato", GetType(String)))
            .Add(New DataColumn("ChkLayOut_Join_Prodotti", GetType(String)))
            .Add(New DataColumn("Cod_RisUm_Altro", GetType(String)))

            .Add(New DataColumn("Disciplinare_PubblicoPrivato", GetType(String)))

            .Add(New DataColumn("ChkLayOut_Peso", GetType(String)))
            .Add(New DataColumn("ChkLayOut_Prezzo", GetType(String)))
            .Add(New DataColumn("ChkFiltro_Varietale", GetType(String)))
            .Add(New DataColumn("Sezionale_Cod", GetType(String)))
            .Add(New DataColumn("Causale_Trasporto_Cod", GetType(String)))
            .Add(New DataColumn("ChkLayOut_Litri", GetType(String)))
            .Add(New DataColumn("Cod_RisUm_Aggiuntivo", GetType(String)))
            .Add(New DataColumn("Cod_Indirizzo_Aggiuntivo", GetType(String)))
        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function




    '########################################################################################
    Public Sub DtForXml_InserisciRiga_Movimenti(
                                ByRef DT As DataTable,
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                    Optional ByVal Piva As String = C_Piva_Default,
                                    Optional ByVal Sa_Cod As Integer = 0,
                                    Optional ByVal ID_Agenda As Integer = 0,
                                    Optional ByVal ID_Mov As Integer = 0,
                                    Optional ByVal Cau_Mov As String = "",
                                    Optional ByVal Mov_Desc As String = "",
                                    Optional ByVal Data_Movimento As Date = C_AgroDataInizio,
                                    Optional ByVal Ora As Date = C_AgroDataInizio,
                                    Optional ByVal Scadenza As Date = C_AgroDataFine,
                                    Optional ByVal Scadenza_Extra As Date = C_AgroDataInizio,
                                    Optional ByVal Doc_Numero_Sin As String = "",
                                    Optional ByVal Doc_Numero As Decimal = 0,
                                    Optional ByVal Doc_Numero_Des As String = "",
                                    Optional ByVal Num_Protocollo As Decimal = 0.0,
                                    Optional ByVal Colli As Integer = 0,
                                    Optional ByVal Peso As Decimal = 0.0,
                                    Optional ByVal Aspetto As String = "",
                                    Optional ByVal Causale_Trasporto As String = "",
                                    Optional ByVal Tipo_Sconto As Integer = 0,
                                    Optional ByVal Cod_RisUm As Integer = 0,
                                    Optional ByVal Cod_IndirizzoRisUm As Integer = 0,
                                    Optional ByVal Cod_Destinazione As Integer = 0,
                                    Optional ByVal Cod_IndirizzoDestinazione As Integer = 0,
                                    Optional ByVal Mezzo As Integer = 0,
                                    Optional ByVal Cod_Vettore As Integer = 0,
                                    Optional ByVal Cod_IndirizzoVettore As Integer = 0,
                                    Optional ByVal Natura_Beni As String = "",
                                    Optional ByVal Modalita As Integer = 0,
                                    Optional ByVal Tara_Veicolo As Decimal = 0,
                                    Optional ByVal Tara_Imballi As Decimal = 0,
                                    Optional ByVal Tipo_Peso As Integer = 0,
                                    Optional ByVal Username_Note As String = "",
                                    Optional ByVal Extra_Str As String = "",
                                    Optional ByVal Extra_Int As Integer = 0,
                                    Optional ByVal Extra_Date As Date = C_AgroDataInizio,
                                    Optional ByVal Validita_Inizio As Date = C_AgroDataInizio,
                                    Optional ByVal Validita_Fine As Date = C_AgroDataFine,
                                    Optional ByVal BaseCode As Integer = C_BaseCode_Default,
                                    Optional ByVal TopCode As Integer = C_TopCode_Default,
                                    Optional ByVal Progr_Protocollo As Integer = 0,
                                    Optional ByVal Progr_Registrazione As Integer = 0,
                                    Optional ByVal Data_Registrazione As Date = C_AgroDataInizio,
                                    Optional ByVal ChkLayOut_Bypass_Fatturato As Integer = 0,
                                    Optional ByVal ChkLayOut_Join_Prodotti As Integer = 0,
                                    Optional ByVal Cod_RisUm_Altro As Integer = 0,
                                    Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                    Optional ByVal ChkLayOut_Peso As Integer = 0,
                                    Optional ByVal ChkLayOut_Prezzo As Integer = 0,
                                    Optional ByVal ChkFiltro_Varietale As Integer = 0,
                                    Optional ByVal Sezionale_Cod As Integer = 0,
                                    Optional ByVal Causale_Trasporto_Cod As Integer = 0,
                                    Optional ByVal ChkLayOut_Litri As Integer = 0,
                                    Optional ByVal Cod_RisUm_Aggiuntivo As Integer = 0,
                                    Optional ByVal Cod_Indirizzo_Aggiuntivo As Integer = 0
                                    )


        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("ID_Agenda") = ID_Agenda
            .Item("ID_Mov") = ID_Mov
            .Item("Cau_Mov") = Cau_Mov
            .Item("Mov_Desc") = Mov_Desc
            .Item("Data_Movimento") = Data_Movimento
            .Item("Ora") = Ora
            .Item("Scadenza") = Scadenza
            .Item("Scadenza_Extra") = Scadenza_Extra
            .Item("Doc_Numero_Sin") = Doc_Numero_Sin
            .Item("Doc_Numero") = Doc_Numero
            .Item("Doc_Numero_Des") = Doc_Numero_Des
            .Item("Num_Protocollo") = Num_Protocollo
            .Item("Colli") = Colli
            .Item("Peso") = Peso
            .Item("Aspetto") = Aspetto
            .Item("Causale_Trasporto") = Causale_Trasporto
            .Item("Tipo_Sconto") = Tipo_Sconto
            .Item("Cod_RisUm") = Cod_RisUm
            .Item("Cod_IndirizzoRisUm") = Cod_IndirizzoRisUm
            .Item("Cod_Destinazione") = Cod_Destinazione
            .Item("Cod_IndirizzoDestinazione") = Cod_IndirizzoDestinazione
            .Item("Mezzo") = Mezzo
            .Item("Cod_Vettore") = Cod_Vettore
            .Item("Cod_IndirizzoVettore") = Cod_IndirizzoVettore
            .Item("Natura_Beni") = Natura_Beni
            .Item("Modalita") = Modalita
            .Item("Tara_Veicolo") = Tara_Veicolo
            .Item("Tara_Imballi") = Tara_Imballi
            .Item("Tipo_Peso") = Tipo_Peso
            .Item("Username_Note") = Username_Note
            .Item("Extra_Str") = Extra_Str
            .Item("Extra_Int") = Extra_Int
            .Item("Extra_Date") = Extra_Date
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode
            .Item("Progr_Protocollo") = Progr_Protocollo
            .Item("Progr_Registrazione") = Progr_Registrazione
            .Item("Data_Registrazione") = Data_Registrazione
            .Item("ChkLayOut_Bypass_Fatturato") = ChkLayOut_Bypass_Fatturato
            .Item("ChkLayOut_Join_Prodotti") = ChkLayOut_Join_Prodotti
            .Item("Cod_RisUm_Altro") = Cod_RisUm_Altro
            .Item("Disciplinare_PubblicoPrivato") = Disciplinare_PubblicoPrivato
            .Item("ChkLayOut_Peso") = ChkLayOut_Peso
            .Item("ChkLayOut_Prezzo") = ChkLayOut_Prezzo
            .Item("ChkFiltro_Varietale") = ChkFiltro_Varietale
            .Item("Sezionale_Cod") = Sezionale_Cod
            .Item("Causale_Trasporto_Cod") = Causale_Trasporto_Cod
            .Item("ChkLayOut_Litri") = ChkLayOut_Litri
            .Item("Cod_RisUm_Aggiuntivo") = Cod_RisUm_Aggiuntivo
            .Item("Cod_Indirizzo_Aggiuntivo") = Cod_Indirizzo_Aggiuntivo
        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub



    '##########################################################################################
    Public Function XML_Agenda_Movimenti( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_Movimenti As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("Movimento")

            With DR_Movimenti

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("id_agenda"), CStr(.Item("ID_Agenda")))
                DataXml.SetAttribute(LCase("ID_Mov"), CStr(.Item("ID_Mov")))
                DataXml.SetAttribute(LCase("Cau_Mov"), .Item("Cau_Mov"))
                DataXml.SetAttribute(LCase("Mov_Desc"), .Item("Mov_Desc"))
                DataXml.SetAttribute(LCase("Data_Movimento"), Format(CDate(.Item("Data_Movimento")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("ora"), .Item("Ora"))
                DataXml.SetAttribute(LCase("Scadenza"), Format(CDate(.Item("Scadenza")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Scadenza_Extra"), Format(CDate(.Item("Scadenza_Extra")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("Doc_Numero_Sin"), .Item("Doc_Numero_Sin"))
                DataXml.SetAttribute(LCase("Doc_Numero"), CStr(.Item("Doc_Numero")))
                DataXml.SetAttribute(LCase("Doc_Numero_Des"), .Item("Doc_Numero_Des"))
                DataXml.SetAttribute(LCase("Num_Protocollo"), CStr(.Item("Num_Protocollo")))
                DataXml.SetAttribute(LCase("colli"), CStr(.Item("Colli")))
                DataXml.SetAttribute(LCase("peso"), CStr(.Item("Peso")))
                DataXml.SetAttribute(LCase("Aspetto"), .Item("Aspetto"))
                DataXml.SetAttribute(LCase("Causale_Trasporto"), .Item("Causale_Trasporto"))
                DataXml.SetAttribute(LCase("Tipo_Sconto"), CStr(.Item("Tipo_Sconto")))
                DataXml.SetAttribute(LCase("Cod_RisUm"), CStr(.Item("Cod_RisUm")))
                DataXml.SetAttribute(LCase("Cod_IndirizzoRisUm"), CStr(.Item("Cod_IndirizzoRisUm")))
                DataXml.SetAttribute(LCase("Cod_Destinazione"), CStr(.Item("Cod_Destinazione")))
                DataXml.SetAttribute(LCase("Cod_IndirizzoDestinazione"), CStr(.Item("Cod_IndirizzoDestinazione")))
                DataXml.SetAttribute(LCase("Mezzo"), CStr(.Item("Mezzo")))
                DataXml.SetAttribute(LCase("Cod_Vettore"), CStr(.Item("Cod_Vettore")))
                DataXml.SetAttribute(LCase("Cod_IndirizzoVettore"), CStr(.Item("Cod_IndirizzoVettore")))
                DataXml.SetAttribute(LCase("Natura_Beni"), .Item("Natura_Beni"))
                DataXml.SetAttribute(LCase("Modalita"), CStr(.Item("Modalita")))
                DataXml.SetAttribute(LCase("Tara_Veicolo"), CStr(.Item("Tara_Veicolo")))
                DataXml.SetAttribute(LCase("Tara_Imballi"), CStr(.Item("Tara_Imballi")))
                DataXml.SetAttribute(LCase("Tipo_Peso"), CStr(.Item("Tipo_Peso")))
                DataXml.SetAttribute(LCase("Username_Note"), .Item("Username_Note"))
                DataXml.SetAttribute(LCase("Extra_Str"), .Item("Extra_Str"))
                DataXml.SetAttribute(LCase("Extra_Int"), CStr(.Item("Extra_Int")))
                DataXml.SetAttribute(LCase("Extra_Date"), CStr(.Item("Extra_Date")))

                DataXml.SetAttribute(LCase("Progr_Protocollo"), CStr(.Item("Progr_Protocollo")))
                DataXml.SetAttribute(LCase("Progr_Registrazione"), CStr(.Item("Progr_Registrazione")))
                DataXml.SetAttribute(LCase("Data_Registrazione"), Format(CDate(.Item("Data_Registrazione")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("ChkLayOut_Bypass_Fatturato"), CStr(.Item("ChkLayOut_Bypass_Fatturato")))
                DataXml.SetAttribute(LCase("ChkLayOut_Join_Prodotti"), CStr(.Item("ChkLayOut_Join_Prodotti")))

                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))

                DataXml.SetAttribute(LCase("Disciplinare_PubblicoPrivato"), CStr(.Item("Disciplinare_PubblicoPrivato")))

            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_Movimenti) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function






    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '/////  AGENDA  ///////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////



    '########################################################################################
    Public Function DtForXml_Genera_Agenda() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))

            .Add(New DataColumn("piva", GetType(String)))
            .Add(New DataColumn("sa_cod", GetType(String)))
            .Add(New DataColumn("id_agenda", GetType(String)))
            .Add(New DataColumn("lav_cod", GetType(String)))
            .Add(New DataColumn("des_lib", GetType(String)))
            .Add(New DataColumn("tipo_accettazione", GetType(String)))
            .Add(New DataColumn("linea_cod", GetType(String)))
            .Add(New DataColumn("preparazione_cod", GetType(String)))
            .Add(New DataColumn("id_trasformazione", GetType(String)))
            .Add(New DataColumn("validita_inizio", GetType(String)))
            .Add(New DataColumn("validita_fine", GetType(String)))
            .Add(New DataColumn("basecode", GetType(String)))
            .Add(New DataColumn("topcode", GetType(String)))
            .Add(New DataColumn("Blocco_Flag", GetType(String)))
            .Add(New DataColumn("Blocco_Data", GetType(String)))
            .Add(New DataColumn("Blocco_Username", GetType(String)))
            .Add(New DataColumn("stato_export", GetType(String)))
            .Add(New DataColumn("Stato_Export_2", GetType(String)))
            .Add(New DataColumn("Tipo_Visibilita", GetType(String)))
            .Add(New DataColumn("ChkCoge_Manuale", GetType(String)))
            .Add(New DataColumn("Id_Attivita", GetType(String)))
            .Add(New DataColumn("Modulo", GetType(String)))
            .Add(New DataColumn("Audit_Cod", GetType(String)))
        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function


    Public Function DtForXml_Genera_Prodotti_Costi() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable


        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))

            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Riferimento", GetType(String)))
            .Add(New DataColumn("Elem_Cod", GetType(Integer)))
            .Add(New DataColumn("Pro_Cod", GetType(Integer)))
            .Add(New DataColumn("Mat_Cod", GetType(Integer)))
            .Add(New DataColumn("Udm_Cod", GetType(Integer)))
            .Add(New DataColumn("Mezzo", GetType(Integer)))
            .Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
            .Add(New DataColumn("Veg_Cod", GetType(Integer)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))
            .Add(New DataColumn("Cul_Cod", GetType(Integer)))

        End With



        Return Dt

    End Function


    Public Sub DtForXml_InserisciRiga_Prodotti_Costi(ByRef dt As DataTable, _
                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                        ByVal Piva As String, _
                        ByVal Riferimento As String, _
                        ByVal Elem_Cod As Integer, _
                         ByVal Pro_Cod As Integer, _
                         ByVal Mat_Cod As Integer, _
                         ByVal Udm_Cod As Integer, _
                         ByVal Mezzo As Integer, _
                         ByVal Prezzo_Unitario As Decimal, _
                         ByVal Veg_Cod As Integer, _
                         ByVal Cul_Cod As Integer, _
                         ByVal Validita_Inizio As Date, _
                         ByVal Validita_Fine As Date)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = dt.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Riferimento") = Riferimento
            .Item("Elem_Cod") = Elem_Cod
            .Item("Pro_Cod") = Pro_Cod
            .Item("Mat_Cod") = Mat_Cod
            .Item("Udm_Cod") = Udm_Cod
            .Item("Mezzo") = Mezzo
            .Item("Prezzo_Unitario") = Prezzo_Unitario
            .Item("Veg_Cod") = Veg_Cod
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("Cul_Cod") = Cul_Cod
            .Item("Id_Budget") = 0

        End With

        '----- Associo al datatable la nuova riga creata

        dt.Rows.Add(Dr)



    End Sub


    '########################################################################################
    Public Sub DtForXml_InserisciRiga_Agenda(
                                ByRef DT As DataTable,
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                    Optional ByVal Piva As String = C_Piva_Default,
                                    Optional ByVal Sa_Cod As Integer = 0,
                                    Optional ByVal ID_Agenda As Integer = 0,
                                    Optional ByVal Lav_Cod As Integer = 0,
                                    Optional ByVal Des_Lib As String = "",
                                    Optional ByVal Tipo_Accettazione As Integer = 0,
                                    Optional ByVal Linea_Cod As Integer = 0,
                                    Optional ByVal Preparazione_Cod As Integer = 0,
                                    Optional ByVal Id_Trasformazione As Integer = 0,
                                    Optional ByVal Validita_Inizio As Date = C_AgroDataInizio,
                                    Optional ByVal Validita_Fine As Date = C_AgroDataFine,
                                    Optional ByVal BaseCode As Integer = C_BaseCode_Default,
                                    Optional ByVal TopCode As Integer = C_TopCode_Default,
                                    Optional ByVal Blocco_Flag As Integer = 0,
                                    Optional ByVal Blocco_Data As Date = C_AgroDataInizio,
                                    Optional ByVal Blocco_Username As String = "",
                                    Optional ByVal Stato_Export As Integer = 0,
                                    Optional ByVal Stato_Export_2 As Integer = 0,
                                    Optional ByVal Tipo_Visibilita As Integer = 0,
                                    Optional ByVal ChkCoge_Manuale As Integer = 0,
                                    Optional ByVal Id_Attivita As Integer = 0,
                                    Optional ByVal Modulo As Integer = 0,
                                    Optional ByVal Audit_Cod As Integer = 0
                                    )

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("TipoOperazioneDB") = TipoOperazioneDB
            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("ID_Agenda") = ID_Agenda
            .Item("Lav_Cod") = Lav_Cod
            .Item("Des_Lib") = Des_Lib
            .Item("Tipo_Accettazione") = Tipo_Accettazione
            .Item("Linea_Cod") = Linea_Cod
            .Item("Preparazione_Cod") = Preparazione_Cod
            .Item("Id_Trasformazione") = Id_Trasformazione
            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine
            .Item("BaseCode") = BaseCode
            .Item("TopCode") = TopCode
            .Item("Blocco_Flag") = Blocco_Flag
            .Item("Blocco_Data") = Blocco_Data
            .Item("Blocco_Username") = Blocco_Username
            .Item("Stato_Export") = Stato_Export
            .Item("Stato_Export_2") = Stato_Export_2
            .Item("Tipo_Visibilita") = Tipo_Visibilita
            .Item("ChkCoge_Manuale") = ChkCoge_Manuale
            .Item("Id_Attivita") = Id_Attivita
            .Item("Modulo") = Modulo
            .Item("Audit_Cod") = Audit_Cod

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub




    '##########################################################################################
    Public Function XML_Agenda_Agenda(ByVal TipoOperazioneDB As Integer,
                                      Optional ByVal Piva As String = "00000000000",
                                      Optional ByVal Sa_Cod As Integer = 0,
                                      Optional ByVal ID_Agenda As Integer = 0,
                                      Optional ByVal Lav_Cod As Integer = 0,
                                      Optional ByVal Des_Lib As String = "",
                                      Optional ByVal Tipo_Accettazione As Integer = 0,
                                      Optional ByVal Linea_Cod As Integer = 0,
                                      Optional ByVal Preparazione_Cod As Integer = 0,
                                      Optional ByVal Id_Trasformazione As Integer = 0,
                                      Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                      Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                      Optional ByVal BaseCode As Integer = 0,
                                      Optional ByVal TopCode As Integer = 200000000,
                                      Optional ByRef XmlDoc As XmlDocument = Nothing) _
                                      As XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Agenda")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        XmlTxt.SetAttribute(LCase("piva"), Piva)
        XmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        XmlTxt.SetAttribute(LCase("lav_cod"), CStr(Lav_Cod))
        XmlTxt.SetAttribute(LCase("des_lib"), Des_Lib)
        XmlTxt.SetAttribute(LCase("tipo_accettazione"), CStr(Tipo_Accettazione))
        XmlTxt.SetAttribute(LCase("linea_cod"), CStr(Linea_Cod))
        XmlTxt.SetAttribute(LCase("preparazione_cod"), CStr(Preparazione_Cod))
        XmlTxt.SetAttribute(LCase("id_trasformazione"), CStr(Id_Trasformazione))
        XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return XmlTxt

        'Distruggo gli oggetti
        XmlTxt = Nothing

    End Function

    '##########################################################################################
    Public Function XML_Agenda_Movimento(ByVal TipoOperazioneDB As Integer,
                                        Optional ByVal Piva As String = "00000000000",
                                        Optional ByVal Sa_Cod As Integer = 0,
                                        Optional ByVal ID_Agenda As Integer = 0,
                                        Optional ByVal ID_Mov As Integer = 0,
                                        Optional ByVal Cau_Mov As String = "",
                                        Optional ByVal Mov_Desc As String = "",
                                        Optional ByVal Data_Movimento As Date = #1/1/1900#,
                                        Optional ByVal Ora As String = "00.00",
                                        Optional ByVal Scadenza As Date = #12/31/2100#,
                                        Optional ByVal Scadenza_Extra As Date = #1/1/1900#,
                                        Optional ByVal Doc_Numero_Sin As String = "",
                                        Optional ByVal Doc_Numero As Integer = 0,
                                        Optional ByVal Doc_Numero_Des As String = "",
                                        Optional ByVal Num_Protocollo As Double = 0.0,
                                        Optional ByVal Colli As Integer = 0,
                                        Optional ByVal Peso As Double = 0.0,
                                        Optional ByVal Aspetto As String = "",
                                        Optional ByVal Causale_Trasporto As String = "",
                                        Optional ByVal Tipo_Sconto As Integer = 0,
                                        Optional ByVal Cod_RisUm As Integer = 0,
                                        Optional ByVal Cod_IndirizzoRisUm As Integer = 0,
                                        Optional ByVal Cod_Destinazione As Integer = 0,
                                        Optional ByVal Cod_IndirizzoDestinazione As Integer = 0,
                                        Optional ByVal Mezzo As Integer = 0,
                                        Optional ByVal Cod_Vettore As Integer = 0,
                                        Optional ByVal Cod_IndirizzoVettore As Integer = 0,
                                        Optional ByVal Natura_Beni As String = "",
                                        Optional ByVal Modalita As Integer = 0,
                                        Optional ByVal Tara_Veicolo As Double = 0,
                                        Optional ByVal Tara_Imballi As Double = 0,
                                        Optional ByVal Tipo_Peso As Integer = 0,
                                        Optional ByVal Username_Note As String = "",
                                        Optional ByVal Extra_Str As String = "",
                                        Optional ByVal Extra_Int As Integer = 0,
                                        Optional ByVal Extra_Date As Date = #1/1/1900#,
                                        Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                        Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                        Optional ByVal BaseCode As Integer = 0,
                                        Optional ByVal TopCode As Integer = 200000000,
                                        Optional ByRef XmlDoc As XmlDocument = Nothing) _
                                        As XmlElement

        Dim XmlTxt As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Movimento")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        XmlTxt.SetAttribute(LCase("piva"), Piva)
        XmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        XmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        XmlTxt.SetAttribute(LCase("Cau_Mov"), Cau_Mov)
        XmlTxt.SetAttribute(LCase("Mov_Desc"), Mov_Desc)
        XmlTxt.SetAttribute(LCase("Data_Movimento"), Format(Data_Movimento, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("ora"), Ora)
        XmlTxt.SetAttribute(LCase("Scadenza"), Format(Scadenza, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Scadenza_Extra"), Format(Scadenza_Extra, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Doc_Numero_Sin"), Doc_Numero_Sin)
        XmlTxt.SetAttribute(LCase("Doc_Numero"), CStr(Doc_Numero))
        XmlTxt.SetAttribute(LCase("Doc_Numero_Des"), Doc_Numero_Des)
        XmlTxt.SetAttribute(LCase("Num_Protocollo"), CStr(Num_Protocollo))
        XmlTxt.SetAttribute(LCase("colli"), CStr(Colli))
        XmlTxt.SetAttribute(LCase("peso"), CStr(Peso))
        XmlTxt.SetAttribute(LCase("Aspetto"), Aspetto)
        XmlTxt.SetAttribute(LCase("Causale_Trasporto"), Causale_Trasporto)
        XmlTxt.SetAttribute(LCase("Tipo_Sconto"), CStr(Tipo_Sconto))
        XmlTxt.SetAttribute(LCase("Cod_RisUm"), CStr(Cod_RisUm))
        XmlTxt.SetAttribute(LCase("Cod_IndirizzoRisUm"), CStr(Cod_IndirizzoRisUm))
        XmlTxt.SetAttribute(LCase("Cod_Destinazione"), CStr(Cod_Destinazione))
        XmlTxt.SetAttribute(LCase("Cod_IndirizzoDestinazione"), CStr(Cod_IndirizzoDestinazione))
        XmlTxt.SetAttribute(LCase("Mezzo"), CStr(Mezzo))
        XmlTxt.SetAttribute(LCase("Cod_Vettore"), CStr(Cod_Vettore))
        XmlTxt.SetAttribute(LCase("Cod_IndirizzoVettore"), CStr(Cod_IndirizzoVettore))
        XmlTxt.SetAttribute(LCase("Natura_Beni"), Natura_Beni)
        XmlTxt.SetAttribute(LCase("Modalita"), CStr(Modalita))
        XmlTxt.SetAttribute(LCase("Tara_Veicolo"), CStr(Tara_Veicolo))
        XmlTxt.SetAttribute(LCase("Tara_Imballi"), CStr(Tara_Imballi))
        XmlTxt.SetAttribute(LCase("Tipo_Peso"), CStr(Tipo_Peso))
        XmlTxt.SetAttribute(LCase("Username_Note"), Username_Note)
        XmlTxt.SetAttribute(LCase("Extra_Str"), Extra_Str)
        XmlTxt.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        XmlTxt.SetAttribute(LCase("Extra_Date"), CStr(Extra_Date))
        XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return XmlTxt

        'Distruggo gli oggetti
        XmlTxt = Nothing

    End Function



    '##########################################################################################
    Public Function XML_Agenda_MovimentoDettaglio(
                        ByVal TipoOperazioneDB As Integer,
                        Optional ByVal Piva As String = "00000000000",
                        Optional ByVal Sa_Cod As Integer = 0,
                        Optional ByVal ID_Agenda As Integer = 0,
                        Optional ByVal ID_Mov As Integer = 0,
                        Optional ByVal ID_Mov_Det As Integer = 0,
                        Optional ByVal Mov_Det_Des As String = "",
                        Optional ByVal Elem_Cod As Integer = 0,
                        Optional ByVal Pro_Cod As Integer = 0,
                        Optional ByVal Mat_Cod As Integer = 0,
                        Optional ByVal Cod_Progetto As Integer = 0,
                        Optional ByVal Fase_Cod As Integer = 0,
                        Optional ByVal Lotto As String = "",
                        Optional ByVal Cal_Cod As Integer = 0,
                        Optional ByVal Udm_Cod As Integer = 0,
                        Optional ByVal Udm_Cod_Extra As Integer = 0,
                        Optional ByVal Qta As Double = 0.0,
                        Optional ByVal Qta_Extra As Double = 0.0,
                        Optional ByVal Prezzo_Unitario As Double = 0.0,
                        Optional ByVal Prezzo_Unitario_Netto As Double = 0.0,
                        Optional ByVal Imponibile As Double = 0.0,
                        Optional ByVal Imponibile_Netto As Double = 0.0,
                        Optional ByVal Cod_IVA As Integer = 0,
                        Optional ByVal IVA As Double = 0.0,
                        Optional ByVal Sconto As Double = 0,
                        Optional ByVal Prezzo_Effettivo As Double = 0,
                        Optional ByVal Anno As Integer = 1900,
                        Optional ByVal Ric_Cod As Integer = 0,
                        Optional ByVal Cod_Conto As Integer = 0,
                        Optional ByVal Jolly_Int As Integer = 0,
                        Optional ByVal Contabilizzato As Integer = 1,
                        Optional ByVal Pendente As Integer = 2,
                        Optional ByVal Extra_Str As String = "",
                        Optional ByVal Extra_Int As Integer = 0,
                        Optional ByVal Extra_Date As Date = #1/1/1900#,
                        Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                        Optional ByVal Validita_Fine As Date = #12/31/2100#,
                        Optional ByVal BaseCode As Integer = 0,
                        Optional ByVal TopCode As Integer = 200000000,
                        Optional ByRef XmlDoc As XmlDocument = Nothing,
                        Optional ByVal Id_Destinazione As Integer = 0,
                        Optional ByVal Lav_Cod_Allegato As Integer = 0,
                        Optional ByVal Cau_Mov As String = "") As XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Movimento_Dettaglio")

        'Imposto gli attributi

        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        XmlTxt.SetAttribute(LCase("piva"), Piva)
        XmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        XmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        XmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        XmlTxt.SetAttribute(LCase("Mov_Det_Des"), Mov_Det_Des)
        XmlTxt.SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
        XmlTxt.SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
        XmlTxt.SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
        XmlTxt.SetAttribute(LCase("Cod_Progetto"), CStr(Cod_Progetto))
        XmlTxt.SetAttribute(LCase("Fase_Cod"), CStr(Fase_Cod))
        XmlTxt.SetAttribute(LCase("Lotto"), Lotto)
        XmlTxt.SetAttribute(LCase("Cal_Cod"), CStr(Cal_Cod))
        XmlTxt.SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))
        XmlTxt.SetAttribute(LCase("udm_cod_extra"), CStr(Udm_Cod_Extra))
        XmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))
        XmlTxt.SetAttribute(LCase("qta_extra"), CStr(Qta_Extra))
        XmlTxt.SetAttribute(LCase("Prezzo_Unitario"), CStr(Prezzo_Unitario))
        XmlTxt.SetAttribute(LCase("Prezzo_Unitario_Netto"), CStr(Prezzo_Unitario_Netto))
        XmlTxt.SetAttribute(LCase("Imponibile"), CStr(Imponibile))
        XmlTxt.SetAttribute(LCase("Imponibile_Netto"), CStr(Imponibile_Netto))
        XmlTxt.SetAttribute(LCase("Cod_IVA"), CStr(Cod_IVA))
        XmlTxt.SetAttribute(LCase("Iva"), CStr(IVA))
        XmlTxt.SetAttribute(LCase("Sconto"), CStr(Sconto))
        XmlTxt.SetAttribute(LCase("Prezzo_Effettivo"), CStr(Prezzo_Effettivo))
        XmlTxt.SetAttribute(LCase("Anno"), CStr(Anno))
        XmlTxt.SetAttribute(LCase("Ric_Cod"), CStr(Ric_Cod))
        XmlTxt.SetAttribute(LCase("Cod_Conto"), CStr(Cod_Conto))
        XmlTxt.SetAttribute(LCase("jolly_Int"), CStr(Jolly_Int))
        XmlTxt.SetAttribute(LCase("Contabilizzato"), CStr(Contabilizzato))
        XmlTxt.SetAttribute(LCase("Pendente"), CStr(Pendente))
        XmlTxt.SetAttribute(LCase("Extra_Str"), Extra_Str)
        XmlTxt.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        XmlTxt.SetAttribute(LCase("Extra_Date"), CStr(Extra_Date))
        XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        '==============================================================================================================
        'Inserimento nella stringa Xml dei campi non presenti in Db ma utili (necessari) per la gestione delle giacenze
        '--------------------------------------------------------------------------------------------------------------
        XmlTxt.SetAttribute("id_destinazione", CStr(Id_Destinazione))
        XmlTxt.SetAttribute("lav_cod", CStr(Lav_Cod_Allegato)) 'Nota: Utile in FormFattura
        XmlTxt.SetAttribute("cau_mov", Cau_Mov) 'IMPORTANTE!!!!!


        'Restituisco in uscita 
        Return XmlTxt

        'Distruggo gli oggetti
        XmlTxt = Nothing

    End Function

    '##########################################################################################
    Public Function XML_Agenda_MovimentoDestinazione(ByVal TipoOperazioneDB As Integer,
                                                    Optional ByVal Piva As String = "00000000000",
                                                    Optional ByVal Sa_Cod As Integer = 0,
                                                    Optional ByVal ID_Agenda As Integer = 0,
                                                    Optional ByVal ID_Mov As Integer = 0,
                                                    Optional ByVal ID_Mov_Det As Integer = 0,
                                                    Optional ByVal Appezza As Integer = 0,
                                                    Optional ByVal ID_Destinazione As Integer = 0,
                                                    Optional ByVal Tipo_Destinazione As Integer = 0,
                                                    Optional ByVal Qta As Double = 0,
                                                    Optional ByVal Qta2 As Double = 0,
                                                    Optional ByVal Tipo_Scorta As Integer = 0,
                                                    Optional ByVal Scorta_Min As Double = 0,
                                                    Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                    Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                    Optional ByVal BaseCode As Integer = 0,
                                                    Optional ByVal TopCode As Integer = 200000000,
                                                    Optional ByRef XmlDoc As XmlDocument = Nothing) _
                                                    As XmlElement

        Dim XmlTxt As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Movimento_Destinazione")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        XmlTxt.SetAttribute(LCase("piva"), Piva)
        XmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        XmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        XmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        XmlTxt.SetAttribute(LCase("Appezza"), CStr(Appezza))
        XmlTxt.SetAttribute(LCase("ID_Destinazione"), CStr(ID_Destinazione))
        XmlTxt.SetAttribute(LCase("Tipo_Destinazione"), CStr(Tipo_Destinazione))
        XmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))
        XmlTxt.SetAttribute(LCase("Qta2"), CStr(Qta2))
        XmlTxt.SetAttribute(LCase("Tipo_Scorta"), CStr(Tipo_Scorta))
        XmlTxt.SetAttribute(LCase("Scorta_Min"), CStr(Scorta_Min))
        XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return XmlTxt

        'Distruggo gli oggetti
        XmlTxt = Nothing

    End Function



    '##########################################################################################
    Public Function XML_Agenda_Agenda( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DR_Agenda As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("Agenda")

            With DR_Agenda

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("sa_cod"), CStr(.Item("Sa_Cod")))
                DataXml.SetAttribute(LCase("id_agenda"), CStr(.Item("ID_Agenda")))
                DataXml.SetAttribute(LCase("lav_cod"), CStr(.Item("Lav_Cod")))
                DataXml.SetAttribute(LCase("des_lib"), .Item("Des_Lib"))
                DataXml.SetAttribute(LCase("tipo_accettazione"), CStr(.Item("Tipo_Accettazione")))
                DataXml.SetAttribute(LCase("linea_cod"), CStr(.Item("Linea_Cod")))
                DataXml.SetAttribute(LCase("preparazione_cod"), CStr(.Item("Preparazione_Cod")))
                DataXml.SetAttribute(LCase("id_trasformazione"), CStr(.Item("Id_Trasformazione")))
                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(DR_Agenda.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(DR_Agenda.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("basecode"), CStr(.Item("BaseCode")))
                DataXml.SetAttribute(LCase("topcode"), CStr(.Item("TopCode")))
                DataXml.SetAttribute(LCase("Blocco_Flag"), CStr(.Item("Blocco_Flag")))
                DataXml.SetAttribute(LCase("Blocco_Data"), CStr(.Item("Blocco_Data")))
                DataXml.SetAttribute(LCase("Blocco_Username"), .Item("Blocco_Username"))

            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_Agenda) :     " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function

    Public Function XML_Agenda_Prodotti_Costi(ByRef Log_Errori As String, ByVal XmlDoc As XmlDocument, ByVal dr As DataRow) _
                                        As XmlElement

        Dim DataXml As XmlElement = Nothing

        Try '-----------------------------------------------------------------------------

            DataXml = XmlDoc.CreateElement("Prodotto_Costo")

            With dr

                DataXml.SetAttribute("TipoOperazioneDB", CStr(.Item("TipoOperazioneDB")))

                DataXml.SetAttribute(LCase("piva"), .Item("Piva"))
                DataXml.SetAttribute(LCase("riferimento"), .Item("Riferimento"))
                DataXml.SetAttribute(LCase("elem_cod"), CStr(.Item("Elem_Cod")))
                DataXml.SetAttribute(LCase("pro_cod"), CStr(.Item("Pro_Cod")))
                DataXml.SetAttribute(LCase("mat_cod"), CStr(.Item("Mat_Cod")))
                DataXml.SetAttribute(LCase("udm_cod"), CStr(.Item("Udm_Cod")))
                DataXml.SetAttribute(LCase("mezzo"), CStr(.Item("Mezzo")))
                DataXml.SetAttribute(LCase("prezzo_unitario"), CStr(.Item("Prezzo_Unitario")))
                DataXml.SetAttribute(LCase("veg_cod"), CStr(.Item("Veg_Cod")))
                DataXml.SetAttribute(LCase("validita_inizio"), Format(CDate(.Item("Validita_Inizio")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("validita_fine"), Format(CDate(.Item("Validita_Fine")), "dd/MM/yyyy"))
                DataXml.SetAttribute(LCase("cul_cod"), CStr(.Item("Cul_Cod")))
                DataXml.SetAttribute(LCase("Id_Budget"), CStr(0))


            End With

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_Prodotti_Costi) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return DataXml

    End Function




    '##########################################################################################
    Public Function XML_Agenda_Movimenti__DATI( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DT_Movimenti As DataTable) _
                                        As XmlElement

        Dim RootXml As XmlElement = Nothing
        Dim DataXml As XmlElement
        Dim i As Integer

        Try '-----------------------------------------------------------------------------

            RootXml = XmlDoc.CreateElement("DatiMovimenti")

            For i = 0 To DT_Movimenti.Rows.Count - 1

                DataXml = XML_Agenda_Movimenti( _
                                                Log_Errori, _
                                                XmlDoc, _
                                                DT_Movimenti.Rows(i))

                RootXml.AppendChild(DataXml)

            Next

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_Movimenti__DATI) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return RootXml

    End Function




    '##########################################################################################
    Public Function XML_Agenda_Agenda__DATI( _
                                    ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByRef DT_Agenda As DataTable) _
                                        As XmlElement

        Dim RootXml As XmlElement = Nothing
        Dim DataXml As XmlElement
        Dim i As Integer

        Try '-----------------------------------------------------------------------------

            RootXml = XmlDoc.CreateElement("DatiAgenda")

            For i = 0 To DT_Agenda.Rows.Count - 1

                DataXml = XML_Agenda_Agenda( _
                                                Log_Errori, _
                                                XmlDoc, _
                                                DT_Agenda.Rows(i))

                RootXml.AppendChild(DataXml)

            Next

        Catch ex As Exception '---------------------------------------------------------------

            Log_Errori += " (XML_Agenda_Agenda__DATI) : " & ex.Message

        End Try '-----------------------------------------------------------------------------

        Return RootXml

    End Function



    '##########################################################################################
    'i DT devono essere creati con le funzioni dell'AgronicaCore (DtForXml_Genera_* e DtForXml_InserisciRiga_*)
    Public Function MacroXML_Contabilita_DDT(ByRef Log_Errori As String, _
                                                ByRef XmlDoc As XmlDocument, _
                                                ByVal Piva_SuperUser As String, _
                                                ByVal Piva As String, _
                                                ByVal Lav_Cod As Integer, _
                                                ByVal Des_Lib As String, _
                                                ByVal Doc_Numero_Sin As String, _
                                                ByVal Doc_Numero As Decimal, _
                                                ByVal Doc_Numero_Des As String, _
                                                ByVal Data_Movimento As Date, _
                                                ByVal Num_Colli As Integer, _
                                                ByVal Causale_Trasporto As String, _
                                                ByVal Aspetto_Beni As String, _
                                                ByVal Natura_Beni As String, _
                                                ByVal Peso As Decimal, _
                                                ByVal Data_Consegna As Date, _
                                                ByVal Ora_Consegna As String, _
                                                ByVal Num_Protocollo As Decimal, _
                                                ByVal Note As String, _
                                                ByVal Cod_RisUm As Integer, _
                                                ByVal Cod_IndirizzoRisUm As Integer, _
                                                ByVal Cau_Mov_Magazzino As String, _
                                                ByVal Mov_Desc_Magazzino As String, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                ByVal DT_Mov_Dettaglio_Tecnico_Extra As DataTable, _
                                                ByVal DT_Mov_Dettagli_Riferimenti As DataTable, _
                                                ByVal DT_MovimentiDettagli As DataTable, _
                                                ByVal DT_MovDestinazioni As DataTable, _
                                                Optional ByVal Blocco_Flag As Integer = 0, _
                                                Optional ByVal Blocco_Data As Date = AGRODATAINIZIO, _
                                                Optional ByVal Blocco_Username As String = "", _
                                                Optional ByVal Stato_Export As Integer = 0, _
                                                Optional ByVal Cod_Destinazione As Integer = 0, _
                                                Optional ByVal Cod_IndirizzoDestinazione As Integer = 0, _
                                                Optional ByVal Mezzo As Integer = 0, _
                                                Optional ByVal Cod_Vettore As Integer = 0, _
                                                Optional ByVal Cod_IndirizzoVettore As Integer = 0, _
                                                Optional ByVal Modalita As Integer = 0, _
                                                Optional ByVal Tara_Veicolo As Decimal = 0, _
                                                Optional ByVal Tara_Imballi As Decimal = 0, _
                                                Optional ByVal Tipo_Peso As Integer = 0, _
                                                Optional ByVal Username_Note As String = "", _
                                                Optional ByVal Extra_Str As String = "", _
                                                Optional ByVal Extra_Int As Integer = 0, _
                                                Optional ByVal Extra_Date As Date = C_AgroDataInizio, _
                                                Optional ByVal Progr_Protocollo As Integer = 0, _
                                                Optional ByVal Progr_Registrazione As Integer = 0, _
                                                Optional ByVal Data_Registrazione As Date = C_AgroDataInizio, _
                                                Optional ByVal ChkLayOut_Bypass_Fatturato As Integer = 0, _
                                                Optional ByVal ChkLayOut_Join_Prodotti As Integer = 0, _
                                                Optional ByVal Cod_RisUm_Altro As Integer = 0, _
                                                Optional ByVal Id_Agenda As Integer = 0 _
                                                ) As String


        'Dim i As Integer

        Dim XmlDatiAgenda As System.Xml.XmlElement
        Dim XmlAgenda As System.Xml.XmlElement
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim XmlMovimento As System.Xml.XmlElement
        Dim XmlMovimentoContabile As System.Xml.XmlElement
        Dim XmlMovimentoMagazzino As System.Xml.XmlElement
        Dim XmlDatiMovDettagli As System.Xml.XmlElement

        Dim Dt_Agenda As DataTable
        Dim Dt_Movimenti As DataTable
        '-----------------------------------------

        '#######################################################

        'CREAZIONE DOCUMENTO
        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If


        '#######################################################
        '##################   CREAZIONE DT    ##################
        '#######################################################

        Dt_Agenda = DtForXml_Genera_Agenda()

        Dt_Movimenti = DtForXml_Genera_Movimenti()


        '#######################################################
        '############   INSERIMENTO DATI NEI DT    #############
        '#######################################################

        '----------------------------------
        '-------------- AGENDA ------------
        '----------------------------------
        DtForXml_InserisciRiga_Agenda(Dt_Agenda, _
                                        enum_TipoOperazioneDB.Scrittura, _
                                         Piva, _
                                         0, _
                                         Id_Agenda, _
                                         Lav_Cod, _
                                         Des_Lib, _
                                         , , , , _
                                         Data_Movimento, _
                                         AGRODATAFINE, _
                                         BaseCode, _
                                         TopCode, _
                                         Blocco_Flag, _
                                         Blocco_Data, _
                                         Blocco_Username, _
                                         Stato_Export)

        '----------------------------------
        '-------- MOVIMENTO CONTABILE -----
        '----------------------------------

        DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                                enum_TipoOperazioneDB.Scrittura, _
                                                Piva, _
                                                0, _
                                                Id_Agenda, 0, _
                                                CAU_REGISTRAZIONI, _
                                                Note, _
                                                Data_Movimento, _
                                                Ora_Consegna, _
                                                Data_Consegna, _
                                                 , _
                                                Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des, _
                                                Num_Protocollo, _
                                                Num_Colli, _
                                                Peso, _
                                                Aspetto_Beni, _
                                                Causale_Trasporto, _
                                                0, _
                                                Cod_RisUm, _
                                                Cod_IndirizzoRisUm, _
                                                Cod_Destinazione, _
                                                Cod_IndirizzoDestinazione, _
                                                Mezzo, _
                                                Cod_Vettore, _
                                                Cod_IndirizzoVettore, _
                                                Natura_Beni, _
                                                Modalita, _
                                                Tara_Veicolo, _
                                                Tara_Imballi, _
                                                Tipo_Peso, _
                                                Username_Note, _
                                                Extra_Str, _
                                                Extra_Int, _
                                                Extra_Date, _
                                                Data_Movimento, _
                                                AGRODATAFINE, _
                                                BaseCode, _
                                                TopCode, _
                                                Progr_Protocollo, _
                                                Progr_Registrazione, _
                                                Data_Registrazione, _
                                                ChkLayOut_Bypass_Fatturato, _
                                                ChkLayOut_Join_Prodotti, _
                                                Cod_RisUm_Altro)


        '----------------------------------
        '-------- MOVIMENTO MAGAZZINO -----
        '----------------------------------

        DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            Piva, _
                                            0, _
                                            Id_Agenda, 0, _
                                            Cau_Mov_Magazzino, _
                                            Mov_Desc_Magazzino, _
                                            Data_Movimento, _
                                            Ora_Consegna, _
                                            AGRODATAFINE, _
                                            , _
                                            , , , _
                                            , _
                                            , , , , _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            , , , , , , _
                                            , _
                                            , _
                                            , _
                                            Data_Movimento, _
                                            AGRODATAFINE, _
                                            BaseCode, _
                                            TopCode, _
                                            , , , , , )




        '#######################################################
        '##################   DATI AGENDA    ###################
        '#######################################################

        XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

        XmlDoc.AppendChild(XmlDatiAgenda)


        '#######################################################
        '#####################   AGENDA    #####################
        '#######################################################

        '1 riga solo nel DT
        XmlAgenda = XML_Agenda_Agenda( _
                                   Log_Errori, _
                                   XmlDoc, _
                                   Dt_Agenda.Rows(0))

        XmlDatiAgenda.AppendChild(XmlAgenda)



        '#######################################################
        '################   DATI MOVIMENTI    ##################
        '#######################################################

        XmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

        XmlAgenda.AppendChild(XmlDatiMovimenti)


        '#######################################################
        '##############   MOVIMENTO CONTABILE   ################
        '#######################################################

        XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                            XmlDoc, _
                                            Dt_Movimenti.Rows(0))

        XmlDatiMovimenti.AppendChild(XmlMovimento)


        '##########################################################
        '################  MOVIMENTO TECNICO EXTRA   ##############
        '##########################################################


        '##########################################################
        '#########  MOVIMENTO DI AGENDA DI RIFERIMENTO   ##########
        '##########################################################


        '#######################################################
        '##############   MOVIMENTO MAGAZZINO   ################
        '#######################################################

        XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                            XmlDoc, _
                                            Dt_Movimenti.Rows(1))

        XmlDatiMovimenti.AppendChild(XmlMovimento)


        XmlMovimentoContabile = XmlDatiMovimenti.SelectSingleNode("//Movimento[@cau_mov='" + CAU_REGISTRAZIONI + "']")

        XmlMovimentoMagazzino = XmlDatiMovimenti.SelectSingleNode("//Movimento[@cau_mov='" + Cau_Mov_Magazzino + "']")


        '#######################################################
        '###############   MOVIMENTI DETTAGLI    ###############
        '#######################################################

        XmlDatiMovDettagli = XML_Agenda_MovimentiDettagli__DATI(Log_Errori, _
                                            XmlDoc, _
                                            DT_MovimentiDettagli, _
                                            True, _
                                            DT_MovDestinazioni)

        XmlMovimentoMagazzino.AppendChild(XmlDatiMovDettagli)



        Return XmlDoc.OuterXml



    End Function

    '##########################################################################################
    'i DT devono essere creati con le funzioni dell'AgronicaCore (DtForXml_Genera_* e DtForXml_InserisciRiga_*)
    Public Function MacroXML_Rilievo_Giacenze(ByRef Log_Errori As String, _
                                                ByRef XmlDoc As XmlDocument, _
                                                ByVal Piva_SuperUser As String, _
                                                ByVal Piva As String, _
                                                ByVal Sa_Cod As Integer, _
                                                ByVal Lav_Cod As Integer, _
                                                ByVal Des_Lib As String, _
                                                ByVal Doc_Numero_Sin As String, _
                                                ByVal Doc_Numero As Decimal, _
                                                ByVal Doc_Numero_Des As String, _
                                                ByVal Data_Movimento As Date, _
                                                ByVal Num_Colli As Integer, _
                                                ByVal Causale_Trasporto As String, _
                                                ByVal Aspetto_Beni As String, _
                                                ByVal Peso As Decimal, _
                                                ByVal Data_Consegna As Date, _
                                                ByVal Ora_Consegna As String, _
                                                ByVal Num_Protocollo As Decimal, _
                                                ByVal Note As String, _
                                                ByVal Cod_RisUm As Integer, _
                                                ByVal Cod_IndirizzoRisUm As Integer, _
                                                ByVal Cau_Mov_Magazzino As String, _
                                                ByVal Mov_Desc_Magazzino As String, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                ByVal DT_Mov_Dettaglio_Tecnico_Extra As DataTable, _
                                                ByVal DT_Mov_Dettagli_Riferimenti As DataTable, _
                                                ByVal DT_MovimentiDettagli As DataTable, _
                                                ByVal DT_MovDestinazioni As DataTable, _
                                                Optional ByVal Blocco_Flag As Integer = 0, _
                                                Optional ByVal Blocco_Data As Date = AGRODATAINIZIO, _
                                                Optional ByVal Blocco_Username As String = "", _
                                                Optional ByVal Stato_Export As Integer = 0, _
                                                Optional ByVal Cod_Destinazione As Integer = 0, _
                                                Optional ByVal Cod_IndirizzoDestinazione As Integer = 0, _
                                                Optional ByVal Modalita As Integer = 0, _
                                                Optional ByVal Tara_Veicolo As Decimal = 0, _
                                                Optional ByVal Tara_Imballi As Decimal = 0, _
                                                Optional ByVal Tipo_Peso As Integer = 0, _
                                                Optional ByVal Username_Note As String = "", _
                                                Optional ByVal Extra_Str As String = "", _
                                                Optional ByVal Extra_Int As Integer = 0, _
                                                Optional ByVal Extra_Date As Date = C_AgroDataInizio, _
                                                Optional ByVal Progr_Protocollo As Integer = 0, _
                                                Optional ByVal Progr_Registrazione As Integer = 0, _
                                                Optional ByVal Data_Registrazione As Date = C_AgroDataInizio, _
                                                Optional ByVal Id_Agenda As Integer = 0 _
                                                ) As String


        'Dim i As Integer

        Dim XmlDatiAgenda As System.Xml.XmlElement
        Dim XmlAgenda As System.Xml.XmlElement
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim XmlMovimento As System.Xml.XmlElement
        Dim XmlMovimentoMagazzino As System.Xml.XmlElement
        Dim XmlDatiMovDettagli As System.Xml.XmlElement

        Dim Dt_Agenda As DataTable
        Dim Dt_Movimenti As DataTable
        '-----------------------------------------

        '#######################################################

        'CREAZIONE DOCUMENTO
        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If


        '#######################################################
        '##################   CREAZIONE DT    ##################
        '#######################################################

        Dt_Agenda = DtForXml_Genera_Agenda()

        Dt_Movimenti = DtForXml_Genera_Movimenti()


        '#######################################################
        '############   INSERIMENTO DATI NEI DT    #############
        '#######################################################

        '----------------------------------
        '-------------- AGENDA ------------
        '----------------------------------
        DtForXml_InserisciRiga_Agenda(Dt_Agenda, _
                                        enum_TipoOperazioneDB.Scrittura, _
                                         Piva, _
                                         Sa_Cod, _
                                         Id_Agenda, _
                                         Lav_Cod, _
                                         Des_Lib, _
                                         , , , , _
                                         Data_Movimento, _
                                         AGRODATAFINE, _
                                         BaseCode, _
                                         TopCode, _
                                         Blocco_Flag, _
                                         Blocco_Data, _
                                         Blocco_Username, _
                                         Stato_Export)

        '----------------------------------
        '-------- MOVIMENTO CONTABILE -----
        '----------------------------------

        'DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
        '                                        enum_TipoOperazioneDB.Scrittura, _
        '                                        Piva, _
        '                                        0, _
        '                                        Id_Agenda, 0, _
        '                                        CAU_REGISTRAZIONI, _
        '                                        Note, _
        '                                        Data_Movimento, _
        '                                        Ora_Consegna, _
        '                                        Data_Consegna, _
        '                                         , _
        '                                        Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des, _
        '                                        Num_Protocollo, _
        '                                        Num_Colli, _
        '                                        Peso, _
        '                                        Aspetto_Beni, _
        '                                        Causale_Trasporto, _
        '                                        0, _
        '                                        Cod_RisUm, _
        '                                        Cod_IndirizzoRisUm, _
        '                                        Cod_Destinazione, _
        '                                        Cod_IndirizzoDestinazione, _
        '                                        Modalita, _
        '                                        Tara_Veicolo, _
        '                                        Tara_Imballi, _
        '                                        Tipo_Peso, _
        '                                        Username_Note, _
        '                                        Extra_Str, _
        '                                        Extra_Int, _
        '                                        Extra_Date, _
        '                                        Data_Movimento, _
        '                                        AGRODATAFINE, _
        '                                        BaseCode, _
        '                                        TopCode, _
        '                                        Progr_Protocollo, _
        '                                        Progr_Registrazione, _
        '                                        Data_Registrazione)


        '----------------------------------
        '-------- MOVIMENTO MAGAZZINO -----
        '----------------------------------

        DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            Piva, _
                                            Sa_Cod, _
                                            Id_Agenda, 0, _
                                            Cau_Mov_Magazzino, _
                                            Mov_Desc_Magazzino, _
                                            Data_Movimento, _
                                            Data_Movimento, _
                                            AGRODATAFINE, _
                                            , _
                                            , , , _
                                            , _
                                            , , , , _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            , , , , , , _
                                            , _
                                            , _
                                            , _
                                            Data_Movimento, _
                                            AGRODATAFINE, _
                                            BaseCode, _
                                            TopCode, _
                                            0, _
                                            Progr_Registrazione, _
                                            Data_Registrazione, , , )




        '#######################################################
        '##################   DATI AGENDA    ###################
        '#######################################################

        XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

        XmlDoc.AppendChild(XmlDatiAgenda)


        '#######################################################
        '#####################   AGENDA    #####################
        '#######################################################

        '1 riga solo nel DT
        XmlAgenda = XML_Agenda_Agenda( _
                                   Log_Errori, _
                                   XmlDoc, _
                                   Dt_Agenda.Rows(0))

        XmlDatiAgenda.AppendChild(XmlAgenda)



        '#######################################################
        '################   DATI MOVIMENTI    ##################
        '#######################################################

        XmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

        XmlAgenda.AppendChild(XmlDatiMovimenti)


        '#######################################################
        '##############   MOVIMENTO CONTABILE   ################
        '#######################################################

        'XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
        '                                    XmlDoc, _
        '                                    Dt_Movimenti.Rows(0))

        'XmlDatiMovimenti.AppendChild(XmlMovimento)


        '#######################################################
        '##############   MOVIMENTO MAGAZZINO   ################
        '#######################################################

        XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                            XmlDoc, _
                                            Dt_Movimenti.Rows(0))

        XmlDatiMovimenti.AppendChild(XmlMovimento)


        'XmlMovimentoContabile = XmlDatiMovimenti.SelectSingleNode("//Movimento[@cau_mov='" + CAU_REGISTRAZIONI + "']")

        XmlMovimentoMagazzino = XmlDatiMovimenti.SelectSingleNode("//Movimento[@cau_mov='" + Cau_Mov_Magazzino + "']")


        '#######################################################
        '###############   MOVIMENTI DETTAGLI    ###############
        '#######################################################

        XmlDatiMovDettagli = XML_Agenda_MovimentiDettagli__DATI(Log_Errori, _
                                            XmlDoc, _
                                            DT_MovimentiDettagli, _
                                            True, _
                                            DT_MovDestinazioni)

        XmlMovimentoMagazzino.AppendChild(XmlDatiMovDettagli)



        Return XmlDoc.OuterXml



    End Function


    '##########################################################################################
    'Elenco possibili valori del campo Pendente
    'MovForzato = 5              'Movimento Non Giustificato e Forzato dall'Utente
    'GiacenzeIniziali = 6        'Giacenze Iniziali
    'AutoProduzione = 8          'Materia Prima/Lavorato Autoprodotto
    'AutoConsumo = 9             'Materia Prima/Lavorato Autoconsumato
    'Smaltimento = 10            'Materia Prima/Lavorato Smaltimento
    Public Function MacroXML_CaricoScaricoMagazzino(ByRef Log_Errori As String, _
                                                ByRef XmlDoc As XmlDocument, _
                                                ByVal Flag_Carico1_Scarico2 As Integer, _
                                                ByVal Piva As String, _
                                                ByVal Sa_Cod As Integer, _
                                                ByVal Id_Destinazione As Integer, _
                                                ByVal Des_Lib As String, _
                                                ByVal Data_Movimento As Date, _
                                                ByVal Ora As String, _
                                                ByVal Mov_Desc As String, _
                                                ByVal Mov_Det_Des As String, _
                                                ByVal Elem_Cod As Integer, _
                                                ByVal Pro_Cod As Integer, _
                                                ByVal Mat_Cod As Integer, _
                                                ByVal Cod_Progetto As Integer, _
                                                ByVal Fase_Cod As Integer, _
                                                ByVal Lotto As String, _
                                                ByVal Cal_Cod As Integer, _
                                                ByVal Udm_Cod As Integer, _
                                                ByVal Qta As Decimal, _
                                                ByVal Udm_Cod_Extra As Integer, _
                                                ByVal Qta_Extra As Decimal, _
                                                ByVal Prezzo_Unitario As Decimal, _
                                                ByVal Prezzo_Unitario_Netto As Decimal, _
                                                ByVal Imponibile As Decimal, _
                                                ByVal Imponibile_Netto As Decimal, _
                                                ByVal Pendente As enum_Pendenza, _
                                                ByVal Qta_2 As Decimal, _
                                                ByVal Tipo_Scorta As Integer, _
                                                ByVal Scorta_Min As Decimal, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                Optional ByVal Blocco_Flag As Integer = 0, _
                                                Optional ByVal Blocco_Data As Date = AGRODATAINIZIO, _
                                                Optional ByVal Blocco_Username As String = "", _
                                                Optional ByVal Id_Agenda As Integer = 0 _
                                                ) As String

        Dim XmlDatiAgenda As System.Xml.XmlElement
        Dim XmlAgenda As System.Xml.XmlElement
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim XmlMovimento As System.Xml.XmlElement
        Dim XmlDatiMovDettagli As System.Xml.XmlElement

        Dim Dt_Agenda As DataTable
        Dim Dt_Movimenti As DataTable
        Dim Dt_Dettagli As DataTable
        Dim Dt_Destinazioni As DataTable
        Dim Lav_Cod As Integer
        Dim Cau_Mov As String
        '-----------------------------------------

        '#######################################################

        'CREAZIONE DOCUMENTO
        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Try

            '#######################################################
            '##################   CREAZIONE DT    ##################
            '#######################################################

            Dt_Agenda = DtForXml_Genera_Agenda()

            Dt_Movimenti = DtForXml_Genera_Movimenti()

            Dt_Dettagli = DtForXml_Genera_MovimentiDettagli()

            Dt_Destinazioni = DtForXml_Genera_MovDestinazioni()


            '#######################################################
            '############   INSERIMENTO DATI NEI DT    #############
            '#######################################################

            If Flag_Carico1_Scarico2 = 1 Then
                Lav_Cod = LAVCOD_CARICO
                Cau_Mov = CAU_CARICO
            Else
                Lav_Cod = LAVCOD_SCARICO
                Cau_Mov = CAU_SCARICO
            End If

            '----------------------------------
            '-------------- AGENDA ------------
            '----------------------------------
            DtForXml_InserisciRiga_Agenda(Dt_Agenda, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                             Piva, _
                                             Sa_Cod, _
                                             Id_Agenda, _
                                             Lav_Cod, _
                                             Des_Lib, _
                                               , , , , _
                                             Data_Movimento, _
                                             AGRODATAFINE, _
                                             BaseCode, _
                                             TopCode, _
                                             Blocco_Flag, _
                                             Blocco_Data, _
                                             Blocco_Username, _
                                             0)


            '----------------------------------
            '-------- MOVIMENTO MAGAZZINO -----
            '----------------------------------

            DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                                enum_TipoOperazioneDB.Scrittura, _
                                                Piva, _
                                                Sa_Cod, _
                                                Id_Agenda, _
                                                0, _
                                                Cau_Mov, _
                                                Mov_Desc, _
                                                Data_Movimento, _
                                                Ora, _
                                                AGRODATAFINE, _
                                                , _
                                                , , , _
                                                , _
                                                , , , , _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                , , , , , , _
                                                , _
                                                , _
                                                , _
                                                Data_Movimento, _
                                                AGRODATAFINE, _
                                                BaseCode, _
                                                TopCode, _
                                                , , , , , )

            '----------------------------------
            '-------- MOVIMENTI DETTAGLI -----
            '----------------------------------

            DtForXml_InserisciRiga_MovimentiDettagli(Dt_Dettagli, _
                                        enum_TipoOperazioneDB.Scrittura, _
                                        Piva, _
                                        Sa_Cod, _
                                        Id_Agenda, _
                                        0, 0, _
                                        Mov_Det_Des, _
                                        Elem_Cod, _
                                        Pro_Cod, _
                                        Mat_Cod, _
                                        Cod_Progetto, _
                                        Fase_Cod, _
                                        Lotto, _
                                        Cal_Cod, _
                                        Udm_Cod, _
                                        Udm_Cod_Extra, _
                                        Qta, _
                                        Qta_Extra, _
                                        Prezzo_Unitario, _
                                        Prezzo_Unitario_Netto, _
                                        Imponibile, _
                                        Imponibile_Netto, _
                                        0, _
                                        0.0, _
                                        0, _
                                        0, _
                                        1900, _
                                        0, _
                                        0, _
                                        enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato, _
                                        NONCONTABILE, _
                                        Pendente, _
                                        "", _
                                        0, _
                                        AGRODATAINIZIO, _
                                        Data_Movimento, _
                                        AGRODATAFINE, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                         0, _
                                        0, _
                                        BaseCode, _
                                        TopCode, _
                                        Id_Destinazione, _
                                        Lav_Cod, _
                                        Cau_Mov)

            '----------------------------------
            '-------- MOV DESTINAZIONI --------
            '----------------------------------

            DtForXml_InserisciRiga_MovDestinazioni(Dt_Destinazioni, _
                                                    enum_TipoOperazioneDB.Scrittura, _
                                                    Piva, _
                                                    Sa_Cod, _
                                                    Id_Agenda, _
                                                    0, _
                                                    0, _
                                                    0, _
                                                    Id_Destinazione, _
                                                    TIPO_DESTINAZIONE_MAGAZZINO, _
                                                    Qta, _
                                                    Qta_2, _
                                                    Tipo_Scorta, _
                                                    Scorta_Min, _
                                                    Data_Movimento, _
                                                    AGRODATAFINE, _
                                                    BaseCode, _
                                                    TopCode)


            '#######################################################
            '##################   DATI AGENDA    ###################
            '#######################################################

            XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

            XmlDoc.AppendChild(XmlDatiAgenda)


            '#######################################################
            '#####################   AGENDA    #####################
            '#######################################################

            '1 riga solo nel DT
            XmlAgenda = XML_Agenda_Agenda( _
                                       Log_Errori, _
                                       XmlDoc, _
                                       Dt_Agenda.Rows(0))

            XmlDatiAgenda.AppendChild(XmlAgenda)



            '#######################################################
            '################   DATI MOVIMENTI    ##################
            '#######################################################

            XmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

            XmlAgenda.AppendChild(XmlDatiMovimenti)


            '#######################################################
            '##############   MOVIMENTO MAGAZZINO   ################
            '#######################################################

            XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                                XmlDoc, _
                                                Dt_Movimenti.Rows(0))

            XmlDatiMovimenti.AppendChild(XmlMovimento)

            '#######################################################
            '###############   MOVIMENTI DETTAGLI    ###############
            '#######################################################

            XmlDatiMovDettagli = XML_Agenda_MovimentiDettagli__DATI(Log_Errori, _
                                                XmlDoc, _
                                                Dt_Dettagli, _
                                                True, _
                                                Dt_Destinazioni)

            XmlMovimento.AppendChild(XmlDatiMovDettagli)



        Catch ex As Exception
            Log_Errori = "MacroXML_CaricoScaricoMagazzino: " + ex.Message
        End Try


        Return XmlDoc.OuterXml



    End Function


    '##########################################################################################
    'i DT devono essere creati con le funzioni dell'AgronicaCore (DtForXml_Genera_* e DtForXml_InserisciRiga_*)
    Public Function MacroXML_QDC_Semina(ByRef Log_Errori As String, _
                                                ByRef XmlDoc As XmlDocument, _
                                                ByVal Piva_SuperUser As String, _
                                                ByVal Piva As String, _
                                                ByVal Sa_Cod_Semina As Integer, _
                                                ByVal Sa_Cod_Fabbricato As Integer, _
                                                ByVal Lav_Cod As Integer, _
                                                ByVal Des_Lib As String, _
                                                ByVal Note As String, _
                                                ByVal Data_Movimento As Date, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                ByVal DT_Movimenti_Dettagli_Semina As DataTable, _
                                                ByVal DT_Movimenti_Dettagli_Magazzino As DataTable, _
                                                ByVal DT_MovDestinazioni_Semina As DataTable, _
                                                ByVal DT_MovDestinazioni_Magazzino As DataTable, _
                                                Optional ByVal Blocco_Flag As Integer = 0, _
                                                Optional ByVal Blocco_Data As Date = AGRODATAINIZIO, _
                                                Optional ByVal Blocco_Username As String = "", _
                                                Optional ByVal Stato_Export As Integer = 0) As String



        Dim XmlDatiAgenda As System.Xml.XmlElement
        Dim XmlAgenda As System.Xml.XmlElement
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim XmlMovimento As System.Xml.XmlElement
        Dim XmlMovimentoSemina As System.Xml.XmlElement
        Dim XmlMovimentoMagazzino As System.Xml.XmlElement
        Dim XmlDatiMovDettagli As System.Xml.XmlElement

        Dim Dt_Agenda As DataTable
        Dim Dt_Movimenti As DataTable
        'Dim Dt_MovimentiDettagli As DataTable
        'Dim Dt_MovDestinazioni As DataTable

        '-----------------------------------------

        '#######################################################

        Try


            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '#######################################################
            '##################   CREAZIONE DT    ##################
            '#######################################################

            Dt_Agenda = DtForXml_Genera_Agenda()

            Dt_Movimenti = DtForXml_Genera_Movimenti()


            '#######################################################
            '############   INSERIMENTO DATI NEI DT    #############
            '#######################################################

            '----------------------------------
            '-------------- AGENDA ------------
            '----------------------------------
            DtForXml_InserisciRiga_Agenda(Dt_Agenda, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                             Piva, _
                                             Sa_Cod_Semina, _
                                             0, _
                                             Lav_Cod, _
                                             Des_Lib, _
                                             , , , , _
                                             Data_Movimento, _
                                             AGRODATAFINE, _
                                             BaseCode, _
                                             TopCode, _
                                             Blocco_Flag, _
                                             Blocco_Data, _
                                             Blocco_Username, _
                                             Stato_Export)

            '----------------------------------
            '-------- MOVIMENTO SEMINA --------
            '----------------------------------

            DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                                    enum_TipoOperazioneDB.Scrittura, _
                                                    Piva, _
                                                    Sa_Cod_Semina, _
                                                    0, 0, _
                                                    CAU_LAVORAZIONE, _
                                                    Note, _
                                                    Data_Movimento, _
                                                    , , , , , , , , , , , , , , , , , , , , , , , , , , , , _
                                                    Data_Movimento, _
                                                    AGRODATAFINE, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    , , , , , )


            '----------------------------------
            '-------- MOVIMENTO MAGAZZINO -----
            '----------------------------------

            DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                                enum_TipoOperazioneDB.Scrittura, _
                                                Piva, _
                                                Sa_Cod_Fabbricato, _
                                                0, 0, _
                                                CAU_SCARICO, _
                                                Note, _
                                                Data_Movimento, _
                                                , , , , , , , , , , , , , , , , , , , , , , , , , , , , _
                                                Data_Movimento, _
                                                AGRODATAFINE, _
                                                BaseCode, _
                                                TopCode, _
                                                , , , , , )


            '#######################################################
            '##################   DATI AGENDA    ###################
            '#######################################################

            XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

            XmlDoc.AppendChild(XmlDatiAgenda)


            '#######################################################
            '#####################   AGENDA    #####################
            '#######################################################

            '1 riga solo nel DT
            XmlAgenda = XML_Agenda_Agenda( _
                                       Log_Errori, _
                                       XmlDoc, _
                                       Dt_Agenda.Rows(0))

            XmlDatiAgenda.AppendChild(XmlAgenda)


            '#######################################################
            '################   DATI MOVIMENTI    ##################
            '#######################################################

            XmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

            XmlAgenda.AppendChild(XmlDatiMovimenti)


            '#######################################################
            '##############   MOVIMENTO SEMINA   ###################
            '#######################################################

            XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                                XmlDoc, _
                                                Dt_Movimenti.Rows(0))

            XmlDatiMovimenti.AppendChild(XmlMovimento)


            XmlMovimentoSemina = XmlDatiMovimenti.SelectSingleNode("//Movimento[@cau_mov='" + CAU_LAVORAZIONE + "']")


            '#######################################################
            '############   MOVIMENTI DETTAGLI SEMINA  #############
            '#######################################################

            XmlDatiMovDettagli = XML_Agenda_MovimentiDettagli__DATI(Log_Errori, _
                                                                    XmlDoc, _
                                                                    DT_Movimenti_Dettagli_Semina, _
                                                                    True, _
                                                                    DT_MovDestinazioni_Semina)

            XmlMovimentoSemina.AppendChild(XmlDatiMovDettagli)


            '#######################################################
            '##############   MOVIMENTO MAGAZZINO   ################
            '#######################################################

            XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                                XmlDoc, _
                                                Dt_Movimenti.Rows(1))

            XmlDatiMovimenti.AppendChild(XmlMovimento)


            XmlMovimentoMagazzino = XmlDatiMovimenti.SelectSingleNode("//Movimento[@cau_mov='" + CAU_SCARICO + "']")


            '#######################################################
            '###########   MOVIMENTI DETTAGLI MAGAZZINO  ###########
            '#######################################################


            XmlDatiMovDettagli = XML_Agenda_MovimentiDettagli__DATI(Log_Errori, _
                                                                    XmlDoc, _
                                                                    DT_Movimenti_Dettagli_Magazzino, _
                                                                    True, _
                                                                    DT_MovDestinazioni_Magazzino)

            XmlMovimentoMagazzino.AppendChild(XmlDatiMovDettagli)




        Catch ex As Exception
            Log_Errori += "MacroXML_QDC_Semina. Errore durante la creazione dell'XML della Semina: " + ex.Message
            Return Nothing
        End Try

        Return XmlDoc.OuterXml


    End Function


    '##########################################################################################
    Public Function MacroXML_QDC_RilievoPiogge(ByRef Log_Errori As String, _
                                                ByRef XmlDoc As XmlDocument, _
                                                ByVal Piva_SuperUser As String, _
                                                ByVal Piva As String, _
                                                ByVal Sa_Cod As Integer, _
                                                ByVal Des_Lib As String, _
                                                ByVal Note As String, _
                                                ByVal Data_Movimento As Date, _
                                                ByVal Ora As Date, _
                                                ByVal mm_Pioggia As Decimal, _
                                                ByVal Temp_Minima As Decimal, _
                                                ByVal Temp_Massima As Decimal, _
                                                ByVal Umidita As Decimal, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                Optional ByVal Blocco_Flag As Integer = 0, _
                                                Optional ByVal Blocco_Data As Date = AGRODATAINIZIO, _
                                                Optional ByVal Blocco_Username As String = "") As String


        Dim NomeRoutine As String = "AgronicaCoreXML.XML_Contab.MacroXML_QDC_RilievoPiogge()"

        Dim XmlDatiAgenda As System.Xml.XmlElement
        Dim XmlAgenda As System.Xml.XmlElement
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim XmlMovimento As System.Xml.XmlElement
        Dim XmlDatiMovDettagliTecnici As System.Xml.XmlElement
        'Dim XmlMovDettaglioTecnico As System.Xml.XmlElement

        Dim Dt_Agenda As DataTable
        Dim Dt_Movimenti As DataTable
        Dim DT_Movimenti_Dettagli_Tecnici As DataTable

        '-----------------------------------------

        '#######################################################

        Try


            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '#######################################################
            '##################   CREAZIONE DT    ##################
            '#######################################################

            Dt_Agenda = DtForXml_Genera_Agenda()

            Dt_Movimenti = DtForXml_Genera_Movimenti()

            DT_Movimenti_Dettagli_Tecnici = DtForXml_Genera_MovimentiDettagliTecnici()


            '#######################################################
            '############   INSERIMENTO DATI NEI DT    #############
            '#######################################################

            '----------------------------------
            '-------------- AGENDA ------------
            '----------------------------------
            DtForXml_InserisciRiga_Agenda(Dt_Agenda, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                             Piva, _
                                             Sa_Cod, _
                                             0, _
                                             LAVCOD_RILIEVO_PIOGGE, _
                                             Des_Lib, _
                                             , , , , _
                                             Data_Movimento, _
                                             AGRODATAFINE, _
                                             BaseCode, _
                                             TopCode, _
                                             Blocco_Flag, _
                                             Blocco_Data, _
                                             Blocco_Username, _
                                             0)

            '----------------------------------
            '---------- MOVIMENTO  ------------
            '----------------------------------

            DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, _
                                                    enum_TipoOperazioneDB.Scrittura, _
                                                    Piva, _
                                                    Sa_Cod, _
                                                    0, 0, _
                                                    CAU_RILIEVO_CAMPO, _
                                                    Note, _
                                                    Data_Movimento, _
                                                    Ora, _
                                                    , , , , , , , , , , , , , , , , , , , , , , , , , , , _
                                                    Data_Movimento, _
                                                    AGRODATAFINE, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    , , , , , )

            '----------------------------------------------
            '----- MOVIMENTI DETTAGLI TECNICI  ------------
            '----------------------------------------------

            DtForXml_InserisciRiga_MovimentiDettagliTecnici(DT_Movimenti_Dettagli_Tecnici, _
                                                            enum_TipoOperazioneDB.Scrittura, _
                                                            Piva, _
                                                            Sa_Cod, _
                                                            0, 0, 0, 0, _
                                                            mm_Pioggia, _
                                                            , , , , , , , , , , , , , _
                                                            Temp_Minima, _
                                                            Temp_Massima, _
                                                            Umidita, _
                                                            , , , , , , , , , , , , , _
                                                            Data_Movimento, _
                                                            AGRODATAFINE, _
                                                            BaseCode, _
                                                            TopCode)


            '#######################################################
            '##################   DATI AGENDA    ###################
            '#######################################################

            XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

            XmlDoc.AppendChild(XmlDatiAgenda)


            '#######################################################
            '#####################   AGENDA    #####################
            '#######################################################

            '1 riga solo nel DT
            XmlAgenda = XML_Agenda_Agenda( _
                                       Log_Errori, _
                                       XmlDoc, _
                                       Dt_Agenda.Rows(0))

            XmlDatiAgenda.AppendChild(XmlAgenda)


            '#######################################################
            '################   DATI MOVIMENTI    ##################
            '#######################################################

            XmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

            XmlAgenda.AppendChild(XmlDatiMovimenti)


            '#######################################################
            '#################   MOVIMENTO   #######################
            '#######################################################

            XmlMovimento = XML_Agenda_Movimenti(Log_Errori, _
                                                XmlDoc, _
                                                Dt_Movimenti.Rows(0))

            XmlDatiMovimenti.AppendChild(XmlMovimento)

            '#######################################################
            '###########  DATI MOVIMENTI DETTAGLI TECNICI  #########
            '#######################################################


            '#######################################################
            '###########   MOVIMENTI DETTAGLI TECNICI  #############
            '#######################################################

            XmlDatiMovDettagliTecnici = XML_Agenda_MovimentiDettagliTecnici__DATI(Log_Errori, _
                                                                                XmlDoc, _
                                                                                DT_Movimenti_Dettagli_Tecnici)

            XmlMovimento.AppendChild(XmlDatiMovDettagliTecnici)


            Return XmlDoc.OuterXml


        Catch ex As Exception
            Log_Errori += NomeRoutine + ": Errore durante la creazione dell'XML del Rilievo Piogge: " + ex.Message
        End Try



    End Function

    Public Function MicroXML_Prodotti_Costi(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                        ByVal XmlDoc As XmlDocument, _
                        ByVal Piva As String, _
                        ByVal Riferimento As String, _
                        ByVal Elem_Cod As Integer, _
                         ByVal Pro_Cod As Integer, _
                         ByVal Mat_Cod As Integer, _
                         ByVal Udm_Cod As Integer, _
                         ByVal Mezzo As Integer, _
                         ByVal Prezzo_Unitario As Decimal, _
                         ByVal Veg_Cod As Integer, _
                         ByVal Cul_Cod As Integer, _
                         ByRef Validita_Inizio As Date, _
                         ByRef Validita_Fine As Date, ByRef Log_errori As String) As XmlElement

        Dim dtProd_cost As DataTable = Nothing

        Try
            dtProd_cost = DtForXml_Genera_Prodotti_Costi()
            DtForXml_InserisciRiga_Prodotti_Costi(dtProd_cost, TipoOperazioneDB, Piva, Riferimento, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, Mezzo, Prezzo_Unitario,
                                Veg_Cod, Cul_Cod, Validita_Inizio, Validita_Fine)

            If IsNothing(dtProd_cost) Or dtProd_cost.Rows.Count = 0 Then
                Throw New ApplicationException("DtForXml_InserisciRiga_Prodotti_Costi non ha restituito nessuna riga")
            End If

        Catch ex As Exception
            Log_errori = "MacroXML_Prodotti_Costi: Errore durante la creazione dell'XML: " + ex.Message
            Return Nothing
        End Try

        Return XML_Agenda_Prodotti_Costi(Log_errori, XmlDoc, dtProd_cost.Rows(0))

    End Function

    'creo il macro xml.... non ho ben capito il giro, ma il cut & paste ha una risposta a tutto (Marco docet)
    Public Function MicroXML_Agenda(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                        ByVal XmlDoc As XmlDocument, _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Integer, _
                        ByVal ID_Agenda As Integer, _
                        ByVal Des_Lib As String, _
                        ByVal Lav_Cod As Integer, _
                        ByVal Validita_Inizio As Date, _
                        ByVal Validita_Fine As Date, _
                        ByVal BaseCode As Integer, _
                        ByVal TopCode As Integer, ByRef Log_Errori As String) As XmlElement

        Try

            Dim Dt_Agenda As DataTable = Me.DtForXml_Genera_Agenda()
            'valorizzo tutti i parametri anche quelli opzionali, se no il codice è illegibile!
            'MALEDETTO VB.NET
            Me.DtForXml_InserisciRiga_Agenda(Dt_Agenda, TipoOperazioneDB, _
                                             Piva, Sa_Cod, ID_Agenda, Lav_Cod, Des_Lib, 0, 0, 0, 0, Validita_Inizio, _
                                             Validita_Fine, BaseCode, TopCode, 0, AGRODATAINIZIO, "", 0)


            If (Dt_Agenda.Rows.Count > 0) Then
                '1 riga solo nel DT
                Return XML_Agenda_Agenda(Log_Errori, XmlDoc, Dt_Agenda.Rows(0))
            Else
                Throw New ApplicationException("DtForXml_InserisciRiga_Agenda non ha restituito nessuna riga")
            End If

        Catch ex As Exception
            Log_Errori = "MacroXML_Agenda: Errore durante la creazione dell'XML: " + ex.Message
            Return Nothing
        End Try



    End Function

    Public Function MicroXML_Agenda_Movimento(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                         ByVal xmlDoc As XmlDocument, _
                         ByVal Piva As String, _
                         ByVal Sa_Cod As Integer, _
                         ByVal ID_Agenda As Integer, _
                         ByVal ID_Mov As Integer, _
                         ByVal Cod_RisUm As Integer, _
                         ByVal Cau_Mov As String, _
                         ByVal Mov_Desc As String, _
                         ByVal Data_Movimento As Date, _
                         ByVal Scadenza As Date, _
                         ByVal Doc_Numero As Integer, _
                         ByVal Num_Protocollo As Decimal, _
                         ByVal Mezzo As Integer, _
                         ByVal Validita_Inizio As Date, _
                         ByVal Validita_Fine As Date, _
                         ByVal BaseCode As Integer, _
                         ByVal TopCode As Integer, _
                         ByVal Ora As DateTime, _
                         ByVal Extra_Int As Integer, ByRef Log_Errori As String) As XmlElement


        Try
            Dim Dt_Movimenti As DataTable = Me.DtForXml_Genera_Movimenti()
            Me.DtForXml_InserisciRiga_Movimenti(Dt_Movimenti, TipoOperazioneDB, Piva, Sa_Cod, ID_Agenda, ID_Mov, Cau_Mov, Mov_Desc, Data_Movimento, _
                                        Ora, Scadenza, Date.Parse("01/01/1900"), "", Doc_Numero, "", Num_Protocollo, 0, 0, "", "", 0, _
                                         Cod_RisUm, 0, 0, 0, Mezzo, 0, 0, "", 0, 0, 0, 0, "", "", Extra_Int, Date.Parse("01/01/1900"), Validita_Inizio, _
                                        Validita_Fine, BaseCode, TopCode, 0, 0, Date.Parse("01/01/1900"), 0, 0, 0)


            If (Dt_Movimenti.Rows.Count > 0) Then
                '1 riga solo nel DT
                Return XML_Agenda_Movimenti(Log_Errori, xmlDoc, Dt_Movimenti.Rows(0))
            Else
                Throw New ApplicationException("DtForXml_InserisciRiga_Movimenti non ha restituito nessuna riga")
            End If
        Catch ex As Exception
            Log_Errori = "MacroXML_Agenda_Movimento: Errore durante la creazione dell'XML: " + ex.Message
            Return Nothing
        End Try
    End Function


    Public Function MicroXML_Agenda_Movimento_Dettaglio(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                        ByVal xmlDoc As XmlDocument, _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Integer, _
                        ByVal ID_Agenda As Integer, _
                         ByVal ID_Mov As Integer, _
                         ByVal ID_Mov_Det As Integer, _
                         ByVal Elem_Cod As Integer, _
                         ByVal Pro_Cod As Integer, _
                         ByVal Mat_Cod As Integer, _
                         ByVal Mov_Det_Des As String, _
                         ByVal Udm_Cod As Integer, _
                         ByVal Extra_Int As Integer, _
                         ByVal Qta As Decimal, _
                         ByVal Cod_IVA As Integer, _
                         ByVal Sconto As Decimal, _
                         ByVal Prezzo_Unitario As Decimal, _
                         ByVal Cod_Conto As Integer, _
                         ByVal Cod_Progetto As Integer, _
                         ByVal Fase_Cod As Integer, _
                         ByVal Contabilizzato As Integer, _
                         ByVal Pendente As Integer, _
                         ByVal Validita_Inizio As Date, _
                         ByVal Validita_Fine As Date, _
                         ByVal BaseCode As Integer, _
                         ByVal TopCode As Integer, _
                         ByVal ID_Destinazione As Integer, _
                         ByVal Cau_Mov As String, _
                         ByVal Cal_Cod As Integer, _
                         ByVal Lotto As String, _
                         ByVal Anno As Integer, _
                         ByVal Udm_Cod_Extra As Integer, _
                         ByVal Qta_Extra As Integer, ByRef Log_Errori As String) As XmlElement

        Try

            Dim dt_mov_det As DataTable = Me.DtForXml_Genera_MovimentiDettagli()
            Me.DtForXml_InserisciRiga_MovimentiDettagli(dt_mov_det, TipoOperazioneDB, Piva, Sa_Cod, ID_Agenda, ID_Mov, ID_Mov_Det, Mov_Det_Des, _
                                                     Elem_Cod, Pro_Cod, Mat_Cod, Cod_Progetto, Fase_Cod, Lotto, Cal_Cod, Udm_Cod, Udm_Cod_Extra, _
                                                     Qta, Qta_Extra, Prezzo_Unitario, 0, 0, 0, Cod_IVA, 0, Sconto, 0, Anno, 0, Cod_Conto, 0, _
                                                     Contabilizzato, Pendente, "", Extra_Int, Date.Parse("01/01/1900"), Validita_Inizio, _
                                                     Validita_Fine, 0, 0, Qta_Extra, 0, 0, 0, 0, BaseCode, TopCode, ID_Destinazione, 0, Cau_Mov)



            If (dt_mov_det.Rows.Count > 0) Then
                '1 riga solo nel DT
                Return XML_Agenda_MovimentiDettagli(Log_Errori, xmlDoc, dt_mov_det.Rows(0))
            Else
                Throw New ApplicationException("DtForXml_InserisciRiga_MovimentiDettagli non ha restituito nessuna riga")
            End If
        Catch ex As Exception
            Log_Errori = "MacroXML_Agenda_Movimento_Dettaglio: Errore durante la creazione dell'XML: " + ex.Message
            Return Nothing
        End Try


    End Function


    Public Function MicroXML_ParcoMacchine(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                        ByVal xmlDoc As XmlDocument, _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Integer, _
                        ByVal Mac_Cod As Integer, _
                        ByVal Class_Code As String, _
                         ByVal Mac_Des As String, _
                         ByVal Costo_Acquisto As Decimal, _
                         ByVal Targa As String, _
                         ByVal Telaio As String, _
                         ByVal Ditta_Cod As Integer, _
                         ByVal Modello As String, _
                         ByVal Potenza As String, _
                         ByVal Ammortamento As Decimal, _
                         ByVal Ammortizzato As Decimal, _
                         ByVal Data_Immatricolazione As Date, _
                         ByVal Ultima_Manutenzione As Date, _
                         ByVal Data_Revisione As Date, _
                         ByVal Stato_Utilizzo As String, _
                         ByVal Validita_Inizio As Date, _
                         ByVal Validita_Fine As Date, _
                         ByVal BaseCode As Integer, _
                         ByVal TopCode As Integer, _
                         ByVal Note As String, _
                         ByVal Tipo As Integer, _
                         ByVal N_Immatricolazione As String, _
                         ByVal N_Immatricolazione_Rimorchio As String, _
                         ByVal N_Autorizzazione_Trasporto As String, _
                         ByVal Data_Rilascio_Autorizzazione As Date, _
                         ByVal Peso As Decimal, ByVal pivaSuperUser As String, _
                        ByVal Tipo_Targa_Cod As Integer, ByVal Alimentazione_Cod As Integer, _
                        ByVal Potenza_Udm_Cod As Integer, ByVal Taratura_Ugello As Decimal, _
                        ByVal TitoloPossesso As Integer, ByVal CUAA_proprietario As String, _
                        ByVal denominazione_proprietario As String, ByVal data_carico As Date, _
                        ByVal data_scarico As Date, _
                        ByVal Mac_Cod_Origine As Integer, _
                        ByVal PivasuperUser_Origine As String, _
                        ByRef Log_Errori As String) As XmlElement


        Try
            Dim dt_parco_mac As DataTable = Me.DtForXml_Genera_ParcoMacchine()
            Me.DtForXml_InserisciRiga_ParcoMacchine(dt_parco_mac, TipoOperazioneDB, Piva, Sa_Cod, Mac_Cod, Class_Code, Mac_Des, Costo_Acquisto, _
                                                 Targa, Telaio, Ditta_Cod, Modello, Potenza, Ammortamento, Ammortizzato, Data_Immatricolazione, _
                                                 Ultima_Manutenzione, Data_Revisione, Stato_Utilizzo, Validita_Inizio, Validita_Fine, BaseCode, _
                                                  TopCode, Note, Tipo, N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, _
                                                 Data_Rilascio_Autorizzazione, Peso, Mac_Cod_Origine, PivasuperUser_Origine, 0, 0, "", Alimentazione_Cod, _
                                                Potenza_Udm_Cod, CUAA_proprietario, denominazione_proprietario, _
                                                Tipo_Targa_Cod, 0, "", 0, "", "", Date.Parse("01/01/1900"), _
                                                data_carico, data_scarico, TitoloPossesso, "M", Taratura_Ugello)

            If (dt_parco_mac.Rows.Count > 0) Then
                '1 riga solo nel DT
                Return XML_ParcoMacchine_ParcoMacchine(Log_Errori, xmlDoc, dt_parco_mac.Rows(0))
            Else
                Throw New ApplicationException("DtForXml_InserisciRiga_ParcoMacchine non ha restituito nessuna riga")
            End If
        Catch ex As Exception
            Log_Errori = "MacroXML_ParcoMacchine: Errore durante la creazione dell'XML: " + ex.Message
            Return Nothing
        End Try




    End Function




    '##########################################################################################
    'Xml Per il salvataggio dei Corpi Estranei
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' Mappatura campi:
    ''' TABELLA AGENDA:
    ''' 5003 =Lav_Cod (codice creato nella tabella relativa)
    ''' 
    ''' TABELLA MOVIMENTI
    ''' Data_Arrivo  --> Data_Movimento
    ''' Numero_Bolla_Sin --> Doc_Numero_Sin
    ''' Numero_Bolla --> Doc_Numero
    ''' Numero_Bolla_Des --> Doc_Numero_Des
    ''' Anno --> Extra_Int
    ''' Carico_Numero --> Colli 
    ''' Note --> Mov_Des
    ''' Data_Inizio_Cottura --> Extra_Data
    ''' Ora_Inizio_Cottura --> Ora
    ''' Livello_Qualitativo --> Extra_Str
    ''' ConfezioneMarchio--> Natura_Beni
    ''' 
    ''' TABELLA MOVIMENTI_DETTAGLI
    ''' -50 --> Elem_Cod
    ''' Codice del CE (tabella CorpiEstranei) -->Prod_Cod
    ''' Numero_CE --> QTA
    ''' Tipo enumerativo --> Mat_Cod (per sapere se è stato rilevato in un aereoseparatore , cernita ottica , cernita manuale)
    ''' 38 --> Udm_Cod
    ''' -----------------------------------------------------------------------------
    '''
    ''Documentazione su:
    '''\\Diamante\bk_documentazione\GIAS --- Clienti --- Fruttagel\Monitoraggio fornitura di materia prima per valutazione corpi estranei\Documentazione Interna
    Public Function MacroXML_Corpi_Estranei(ByRef Log_Errori As String,
                                            ByRef XmlDoc As XmlDocument,
                                            ByVal Piva As String,
                                            ByVal Numero_Bolla_Sin As String,
                                            ByVal Numero_Bolla As Integer,
                                            ByVal Numero_Bolla_Des As String,
                                            ByVal Anno As Integer,
                                            ByVal Data_Arrivo As Date,
                                            ByVal Carico_Numero As Integer,
                                            ByVal Data_Inizio_Cottura As Date,
                                            ByVal Ora_Cottura As Date,
                                            ByVal Livello_Qualitativo As String,
                                            ByVal Confezione_Marchio_1 As String,
                                            ByVal Confezione_Marchio_2 As String,
                                            ByVal Pesticidi As Integer,
                                            ByVal Note As String,
                                            ByVal DT_MovimentiDettagli As DataTable,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String


        Dim XmlDatiAgenda As System.Xml.XmlElement
        Dim XmlAgenda As System.Xml.XmlElement
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim XmlMovimento As System.Xml.XmlElement
        Dim XmlDatiMovDettagli As System.Xml.XmlElement
        Dim Dt_Agenda As DataTable
        Dim Dt_Movimenti As DataTable
        Dim Des_Lib As String
        Dim Str_Numero_Bolla As String
        '-----------------------------------------

        '#######################################################

        'CREAZIONE DOCUMENTO
        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If


        '#######################################################
        '##################   CREAZIONE DT    ##################
        '#######################################################

        Dt_Agenda = DtForXml_Genera_Agenda()

        Dt_Movimenti = DtForXml_Genera_Movimenti()


        '#######################################################
        '############   INSERIMENTO DATI NEI DT    #############
        '#######################################################

        '----------------------------------
        '-------------- AGENDA ------------
        '----------------------------------
        Str_Numero_Bolla = Numero_Bolla_Sin + Right(("00000" + CStr(Numero_Bolla)), 5) + Numero_Bolla_Des
        Des_Lib = "Monitoraggio CE - Modulo di Carico n." + CStr(Carico_Numero) + "  (Rif. Bolla n." + Str_Numero_Bolla + ")"

        DtForXml_InserisciRiga_Agenda(Dt_Agenda,
                                        enum_TipoOperazioneDB.Scrittura,
                                         Piva,
                                         0,
                                         0,
                                         LAVCOD_MONITORAGGIO_CE,
                                         Des_Lib,
                                         , , , ,
                                         Data_Arrivo,
                                         AGRODATAFINE,
                                         BaseCode,
                                         TopCode,
                                         , , , )


        '----------------------------------
        '-------- MOVIMENTO  -----
        '----------------------------------
        DtForXml_InserisciRiga_Movimenti(Dt_Movimenti,
                                            enum_TipoOperazioneDB.Scrittura,
                                            Piva,
                                            0, 0, 0,
                                            CAU_CORPI_ESTRANEI,
                                            Note,
                                            Data_Arrivo,
                                            Ora_Cottura,
                                             , ,
                                            Numero_Bolla_Sin, Numero_Bolla, Numero_Bolla_Des,
                                            ,
                                            Carico_Numero,
                                            , Confezione_Marchio_2, , Pesticidi, , , , , , , ,
                                            Confezione_Marchio_1,
                                            , , , , ,
                                            Livello_Qualitativo,
                                            Anno,
                                            Data_Inizio_Cottura,
                                            Data_Arrivo,
                                            AGRODATAFINE,
                                            BaseCode,
                                            TopCode,
                                            , , , , , )




        '#######################################################
        '##################   DATI AGENDA    ###################
        '#######################################################

        XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

        XmlDoc.AppendChild(XmlDatiAgenda)


        '#######################################################
        '#####################   AGENDA    #####################
        '#######################################################

        '1 riga solo nel DT
        XmlAgenda = XML_Agenda_Agenda(
                                   Log_Errori,
                                   XmlDoc,
                                   Dt_Agenda.Rows(0))

        XmlDatiAgenda.AppendChild(XmlAgenda)



        '#######################################################
        '################   DATI MOVIMENTI    ##################
        '#######################################################

        XmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

        XmlAgenda.AppendChild(XmlDatiMovimenti)


        '#######################################################
        '##############   MOVIMENTO  ################
        '#######################################################

        XmlMovimento = XML_Agenda_Movimenti(Log_Errori,
                                            XmlDoc,
                                            Dt_Movimenti.Rows(0))



        '#######################################################
        '###############   MOVIMENTI DETTAGLI    ###############
        '#######################################################

        XmlDatiMovDettagli = XML_Agenda_MovimentiDettagli__DATI(Log_Errori,
                                            XmlDoc,
                                            DT_MovimentiDettagli, False,
                                            )

        XmlMovimento.AppendChild(XmlDatiMovDettagli)


        XmlDatiMovimenti.AppendChild(XmlMovimento)


        Return XmlDoc.OuterXml



    End Function

    '##########################################################################################
    Public Function XML_Ricetta(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal Ricetta_SuperUser As String,
                                ByVal BaseCode As Integer,
                                ByVal TopCode As Integer,
                                Optional ByVal Ricetta_Cod As Integer = 0,
                                Optional ByVal Ricetta_Des As String = "",
                                Optional ByVal Ricetta_Des_Long As String = "",
                                Optional ByVal Veg_Cod As Integer = 0,
                                Optional ByVal Note As String = "",
                                Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                Optional ByVal Piva As String = "",
                                Optional ByVal Sa_Cod As Integer = 0,
                                Optional ByVal Tipo_Ricetta As Integer = 0,
                                Optional ByVal Ricetta_Numero As String = "",
                                Optional ByVal Programmazione_Cod As Integer = 0,
                                Optional ByVal Origine As String = ""
                                ) As String


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Ricetta")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Numero"), CStr(Ricetta_Numero))
        XmlTxt.SetAttribute(LCase("Ricetta_Des"), CStr(Ricetta_Des))
        XmlTxt.SetAttribute(LCase("Ricetta_Des_Long"), CStr(Ricetta_Des_Long))
        XmlTxt.SetAttribute(LCase("Veg_Cod"), CStr(Veg_Cod))
        XmlTxt.SetAttribute(LCase("Note"), CStr(Note))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Piva"), CStr(Piva))
        XmlTxt.SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("Tipo_Ricetta"), CStr(Tipo_Ricetta))
        XmlTxt.SetAttribute(LCase("Programmazione_Cod"), CStr(Programmazione_Cod))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))
        XmlTxt.SetAttribute(LCase("origine"), CStr(Origine))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '##########################################################################################
    Public Function XML_RicettaxCultivar(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                         ByVal Ricetta_SuperUser As String, _
                                         Optional ByVal Ricetta_Cod As Integer = 0, _
                                         Optional ByVal Veg_Cod As Integer = 0, _
                                         Optional ByVal Cul_Cod As Integer = 0, _
                                         Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                         Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                                         As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("RicettaxCultivar")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Veg_Cod"), CStr(Veg_Cod))
        XmlTxt.SetAttribute(LCase("Cul_Cod"), CStr(Cul_Cod))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_RicettaxAgenda(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                         ByVal Ricetta_SuperUser As String, _
                                         Optional ByVal Ricetta_Cod As Integer = 0, _
                                         Optional ByVal Ricetta_Operazione_Cod As Integer = 0, _
                                         Optional ByVal Id_Agenda As Integer = 0, _
                                         Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                         Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                                         As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("RicettaxAgenda")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Id_Agenda"), CStr(Id_Agenda))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '##########################################################################################
    Public Function XML_RicettaxAgenda_2(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                         ByVal Ricetta_SuperUser As String, _
                                         Optional ByVal Ricetta_Cod As Integer = 0, _
                                         Optional ByVal Ricetta_Operazione_Cod As Integer = 0, _
                                         Optional ByVal Id_Agenda As Integer = 0, _
                                         Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                         Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                                         As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("RicettaxAgenda_2")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Id_Agenda"), CStr(Id_Agenda))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '##########################################################################################
    Public Function XML_RicettaxNote(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Ricetta_SuperUser As String, _
                                        Optional ByVal Ricetta_Cod As Integer = 0, _
                                        Optional ByVal Ricetta_Operazione_Cod As Integer = 0, _
                                        Optional ByVal Nota_Cod As Integer = 0, _
                                        Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("RicettaxNote")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Nota_Cod"), CStr(Nota_Cod))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '##########################################################################################
    Public Function XML_RicettaxNote_2(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByVal Ricetta_SuperUser As String,
                                        Optional ByVal Ricetta_Cod As Integer = 0,
                                        Optional ByVal Ricetta_Operazione_Cod As Integer = 0,
                                        Optional ByVal Nota_Cod As Integer = 0,
                                        Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                        Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                        Optional ByVal guidRicetta As String = "") As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("RicettaxNote_2")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Nota_Cod"), CStr(Nota_Cod))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("app_ricetta_operazione_id"), guidRicetta)

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '##########################################################################################
    Public Function XML_Ricetta_Operazione(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Ricetta_SuperUser As String,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            Optional ByVal Ricetta_Cod As Integer = 0,
                                            Optional ByVal Ricetta_Operazione_Cod As Integer = 0,
                                            Optional ByVal Lav_Cod As Integer = 0,
                                            Optional ByVal Ricetta_Operazione_Des As String = "",
                                            Optional ByVal Note As String = "",
                                            Optional ByVal Num_Protocollo As Decimal = 0.0,
                                            Optional ByVal Id_Rcdpi As Integer = 0,
                                            Optional ByVal Extra_Int As Integer = 0,
                                            Optional ByVal Mezzo As Integer = 0,
                                            Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                            Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                            Optional ByVal Gru_Op As Integer = 0,
                                            Optional ByVal Costo As Decimal = 0,
                                            Optional ByVal Noleggio_Passivo As Integer = 0,
                                            Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                            Optional ByVal W_Anagrafica_Stati_Cod As Integer = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire,
                                            Optional ByVal Ricetta_Operazione_Cod_RIF As Integer = 0,
                                            Optional ByVal APP_Ricetta_Operazione_ID As String = "",
                                            Optional ByVal Raccoglitore_Cod As Integer = 0,
                                            Optional ByVal Invia_App As Integer = 0,
                                            Optional ByVal Invia_HubIoT As Integer = 0,
                                            Optional Data_Creazione As Date = AGRODATAINIZIO,
                                            Optional Username_Creazione As String = "",
                                            Optional guidRicetta As String = "",
                                            Optional ByVal Ora As Date? = Nothing
                                            ) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Ricetta_Operazione")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Lav_Cod"), CStr(Lav_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Des"), CStr(Ricetta_Operazione_Des))
        XmlTxt.SetAttribute(LCase("Note"), CStr(Note))
        XmlTxt.SetAttribute(LCase("Num_Protocollo"), CStr(Num_Protocollo))
        XmlTxt.SetAttribute(LCase("Id_Rcdpi"), CStr(Id_Rcdpi))
        XmlTxt.SetAttribute(LCase("Disciplinare_PubblicoPrivato"), CStr(Disciplinare_PubblicoPrivato))
        XmlTxt.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        XmlTxt.SetAttribute(LCase("Mezzo"), CStr(Mezzo))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        If Ora IsNot Nothing Then
            XmlTxt.SetAttribute(LCase("Ora"), Format(Ora, "dd/MM/yyyy HH:mm:ss.fff"))
        Else
            XmlTxt.SetAttribute(LCase("Ora"), Format(Validita_Inizio, "dd/MM/yyyy HH:mm:ss.fff"))
        End If
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Gru_Op"), CStr(Gru_Op))
        XmlTxt.SetAttribute(LCase("Costo"), CStr(Costo))
        XmlTxt.SetAttribute(LCase("Noleggio_Passivo"), CStr(Noleggio_Passivo))
        XmlTxt.SetAttribute(LCase("BaseCode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("TopCode"), CStr(TopCode))
        XmlTxt.SetAttribute(LCase("Noleggio_Passivo"), CStr(Noleggio_Passivo))
        XmlTxt.SetAttribute(LCase("W_Anagrafica_Stati_Cod"), CStr(W_Anagrafica_Stati_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod_RIF"), CStr(Ricetta_Operazione_Cod_RIF))
        XmlTxt.SetAttribute(LCase("APP_Ricetta_Operazione_ID"), CStr(APP_Ricetta_Operazione_ID))
        XmlTxt.SetAttribute(LCase("Raccoglitore_Cod"), CStr(Raccoglitore_Cod))
        XmlTxt.SetAttribute(LCase("Invia_App"), CStr(Invia_App))
        XmlTxt.SetAttribute(LCase("Invia_HubIoT"), CStr(Invia_HubIoT))
        XmlTxt.SetAttribute(LCase("app_ricetta_operazione_id"), guidRicetta)

        If Data_Creazione <> AGRODATAINIZIO Then
            XmlTxt.SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
        End If
        If Username_Creazione <> "" Then
            XmlTxt.SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
        End If

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function




    '##########################################################################################
    Public Function XML_Ricetta_Dettaglio(
                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                        ByVal Ricetta_SuperUser As String,
                        Optional ByVal Ricetta_Cod As Integer = 0,
                        Optional ByVal Ricetta_Operazione_Cod As Integer = 0,
                        Optional ByVal Ricetta_Dettaglio_Cod As Integer = 0,
                        Optional ByVal Miscela_Cod As Integer = 0,
                        Optional ByVal Elem_Cod As Integer = 0,
                        Optional ByVal Pro_Cod As Integer = 0,
                        Optional ByVal Mat_Cod As Integer = 0,
                        Optional ByVal Udm_Cod As Integer = 0,
                        Optional ByVal Extra_Int As Integer = 0,
                        Optional ByVal Qta As Decimal = 0,
                        Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                        Optional ByVal Validita_Fine As Date = #12/31/2100#,
                        Optional ByVal Prezzo_Unitario As Decimal = 0,
                        Optional ByVal Cau_Mov As String = "",
                        Optional ByVal TempoCarenza As Integer = 0,
                        Optional ByVal DoseEtichetta As String = "",
                        Optional ByVal PrincipiAttivi As String = "",
                        Optional ByVal ClassiTossicologiche As String = "",
                        Optional ByVal DoseEtichetta_Value As String = "",
                        Optional ByVal Qualifica_cod As Integer = 0,
                        Optional ByVal Tariffa_cod As Integer = 0,
                        Optional ByVal id_attivita As Integer = 0,
                        Optional ByVal lotto As String = "",
                        Optional ByVal Qta_Extra As Decimal = 0,
                        Optional ByVal Qta_Extra_Totale As Decimal = 0,
                        Optional ByVal Udm_Cod_Extra As Integer = 0,
                        Optional ByVal Mezzo_Det As Integer = 0,
                        Optional ByVal Extra_Str As String = "",
                        Optional ByVal PrincipiAttiviPercAbb As String = "",
                        Optional ByVal PrincipiAttiviPesi As String = "",
                        Optional ByVal Polverulento As Integer = 0,
                        Optional ByVal Buffer As String = ""
                        ) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Ricetta_Dettaglio")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Dettaglio_Cod"), CStr(Ricetta_Dettaglio_Cod))
        XmlTxt.SetAttribute(LCase("Miscela_Cod"), CStr(Miscela_Cod))
        XmlTxt.SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
        XmlTxt.SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
        XmlTxt.SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
        XmlTxt.SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))
        XmlTxt.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        XmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Prezzo_Unitario"), CStr(Prezzo_Unitario))
        XmlTxt.SetAttribute(LCase("Cau_Mov"), CStr(Cau_Mov))
        XmlTxt.SetAttribute(LCase("TempoCarenza"), CStr(TempoCarenza))
        XmlTxt.SetAttribute(LCase("DoseEtichetta"), CStr(DoseEtichetta))
        XmlTxt.SetAttribute(LCase("PrincipiAttivi"), CStr(PrincipiAttivi))
        XmlTxt.SetAttribute(LCase("PrincipiAttiviPercAbb"), CStr(PrincipiAttiviPercAbb))
        XmlTxt.SetAttribute(LCase("PrincipiAttiviPesi"), CStr(PrincipiAttiviPesi))
        XmlTxt.SetAttribute(LCase("Polverulento"), CStr(Polverulento))
        XmlTxt.SetAttribute(LCase("Buffer"), CStr(Buffer))

        XmlTxt.SetAttribute(LCase("ClassiTossicologiche"), CStr(ClassiTossicologiche))
        XmlTxt.SetAttribute(LCase("DoseEtichetta_Value"), CStr(DoseEtichetta_Value))

        XmlTxt.SetAttribute(LCase("Qualifica_cod"), CStr(Qualifica_cod))
        XmlTxt.SetAttribute(LCase("Tariffa_cod"), CStr(Tariffa_cod))
        XmlTxt.SetAttribute(LCase("id_attivita"), CStr(id_attivita))
        XmlTxt.SetAttribute(LCase("lotto"), CStr(lotto))

        XmlTxt.SetAttribute(LCase("Qta_Extra"), CStr(Qta_Extra))
        XmlTxt.SetAttribute(LCase("Qta_Extra_Totale"), CStr(Qta_Extra_Totale))
        XmlTxt.SetAttribute(LCase("Udm_Cod_Extra"), CStr(Udm_Cod_Extra))
        XmlTxt.SetAttribute(LCase("Mezzo_Det"), CStr(Mezzo_Det))

        XmlTxt.SetAttribute(LCase("Extra_Str"), CStr(Extra_Str))


        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_Ricetta_DettaglioTecnico( _
                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                        ByVal Ricetta_SuperUser As String, _
                        Optional ByVal Ricetta_Cod As Integer = 0, _
                        Optional ByVal Ricetta_Operazione_Cod As Integer = 0, _
                        Optional ByVal Ricetta_Dettaglio_Cod As Integer = 0, _
                        Optional ByVal Ricetta_Tecnico_Cod As Integer = 0, _
                        Optional ByVal Miscela_Cod As Integer = 0, _
                        Optional ByVal Qta_Ril As Decimal = 0, _
                        Optional ByVal Dett_Cod As Integer = 0, _
                        Optional ByVal Av_Cod As Integer = 0, _
                        Optional ByVal Av_Gru As Integer = 0, _
                        Optional ByVal Dose As Decimal = 0, _
                        Optional ByVal Parziale As Integer = 0, _
                        Optional ByVal Nitrati As Integer = 0, _
                        Optional ByVal Freatimetro As Decimal = 0, _
                        Optional ByVal Inn1_Data As Date = #1/1/1900#, _
                        Optional ByVal Inn2_Data As Date = #1/1/1900#, _
                        Optional ByVal Inn3_Data As Date = #1/1/1900#, _
                        Optional ByVal Inn4_Data As Date = #1/1/1900#, _
                        Optional ByVal Ditta_Cod As Integer = 0, _
                        Optional ByVal Sigla_AV As String = "0", _
                        Optional ByVal Trap_Num As Integer = 0, _
                        Optional ByVal ID_Insetto As Integer = 0, _
                        Optional ByVal FF_Classe As Integer = 0, _
                        Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                        Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Ricetta_Dettaglio_Tecnico")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Dettaglio_Cod"), CStr(Ricetta_Dettaglio_Cod))
        XmlTxt.SetAttribute(LCase("Miscela_Cod"), CStr(Miscela_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Tecnico_Cod"), CStr(Ricetta_Tecnico_Cod))
        XmlTxt.SetAttribute(LCase("Qta_Ril"), CStr(Qta_Ril))
        XmlTxt.SetAttribute(LCase("Dett_Cod"), CStr(Dett_Cod))
        XmlTxt.SetAttribute(LCase("Av_Cod"), CStr(Av_Cod))
        XmlTxt.SetAttribute(LCase("Av_Gru"), CStr(Av_Gru))
        XmlTxt.SetAttribute(LCase("Dose"), CStr(Dose))
        XmlTxt.SetAttribute(LCase("Parziale"), CStr(Parziale))
        XmlTxt.SetAttribute(LCase("Nitrati"), CStr(Nitrati))
        XmlTxt.SetAttribute(LCase("Freatimetro"), CStr(Freatimetro))
        XmlTxt.SetAttribute(LCase("Inn1_Data"), CStr(Inn1_Data))
        XmlTxt.SetAttribute(LCase("Inn2_Data"), CStr(Inn2_Data))
        XmlTxt.SetAttribute(LCase("Inn3_Data"), CStr(Inn3_Data))
        XmlTxt.SetAttribute(LCase("Inn4_Data"), CStr(Inn4_Data))
        XmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(Ditta_Cod))
        XmlTxt.SetAttribute(LCase("Sigla_Av"), Sigla_AV)
        XmlTxt.SetAttribute(LCase("Trap_Num"), CStr(Trap_Num))
        XmlTxt.SetAttribute(LCase("Id_Insetto"), CStr(ID_Insetto))
        XmlTxt.SetAttribute(LCase("FF_Classe"), CStr(FF_Classe))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_Ricetta_DettaglioTecnico_2(
                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                    ByVal Ricetta_SuperUser As String,
                    Optional ByVal Ricetta_Cod As Integer = 0,
                    Optional ByVal Ricetta_Operazione_Cod As Integer = 0,
                    Optional ByVal Ricetta_Dettaglio_Cod As Integer = 0,
                    Optional ByVal Ricetta_Tecnico_Cod As Integer = 0,
                    Optional ByVal Miscela_Cod As Integer = 0,
                    Optional ByVal Qta_Ril As Decimal = 0,
                    Optional ByVal Dett_Cod As Integer = 0,
                    Optional ByVal Av_Cod As Integer = 0,
                    Optional ByVal Av_Gru As Integer = 0,
                    Optional ByVal Dose As Decimal = 0,
                    Optional ByVal Parziale As Integer = 0,
                    Optional ByVal Nitrati As Integer = 0,
                    Optional ByVal Freatimetro As Decimal = 0,
                    Optional ByVal Inn1_Data As Date = #1/1/1900#,
                    Optional ByVal Inn2_Data As Date = #1/1/1900#,
                    Optional ByVal Inn3_Data As Date = #1/1/1900#,
                    Optional ByVal Inn4_Data As Date = #1/1/1900#,
                    Optional ByVal Ditta_Cod As Integer = 0,
                    Optional ByVal Sigla_AV As String = "0",
                    Optional ByVal Trap_Num As Integer = 0,
                    Optional ByVal ID_Insetto As Integer = 0,
                    Optional ByVal FF_Classe As Integer = 0,
                    Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                    Optional ByVal Validita_Fine As Date = #12/31/2100#,
                    Optional ByVal Mg As Decimal = 0,
                    Optional ByVal N As Decimal = 0,
                    Optional ByVal P As Decimal = 0,
                    Optional ByVal K As Decimal = 0,
                    Optional ByVal Soglia_Cod As Integer = 0,
                    Optional ByVal Soglia_Des As String = "",
                    Optional ByVal Soglia_Qta As Integer = 0,
                    Optional ByVal Efficienza As Decimal = 0,
                    Optional ByVal Ricette_Dettaglio_Tecnico_graphickey As String = "",
                    Optional ByVal piezo2 As Integer = 0,
                    Optional ByVal piezo3 As Integer = 0,
                    Optional ByVal piezo4 As Integer = 0,
                    Optional ByVal Piezo1 As Integer = 0,
                    Optional ByVal Cu As Decimal = 0
            ) _
        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Ricetta_Dettaglio_Tecnico_2")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Dettaglio_Cod"), CStr(Ricetta_Dettaglio_Cod))
        XmlTxt.SetAttribute(LCase("Miscela_Cod"), CStr(Miscela_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Tecnico_Cod"), CStr(Ricetta_Tecnico_Cod))
        XmlTxt.SetAttribute(LCase("Qta_Ril"), CStr(Qta_Ril))
        XmlTxt.SetAttribute(LCase("Dett_Cod"), CStr(Dett_Cod))
        XmlTxt.SetAttribute(LCase("Av_Cod"), CStr(Av_Cod))
        XmlTxt.SetAttribute(LCase("Av_Gru"), CStr(Av_Gru))
        XmlTxt.SetAttribute(LCase("Dose"), CStr(Dose))
        XmlTxt.SetAttribute(LCase("Parziale"), CStr(Parziale))
        XmlTxt.SetAttribute(LCase("Nitrati"), CStr(Nitrati))
        XmlTxt.SetAttribute(LCase("Freatimetro"), CStr(Freatimetro))
        XmlTxt.SetAttribute(LCase("Inn1_Data"), CStr(Inn1_Data))
        XmlTxt.SetAttribute(LCase("Inn2_Data"), CStr(Inn2_Data))
        XmlTxt.SetAttribute(LCase("Inn3_Data"), CStr(Inn3_Data))
        XmlTxt.SetAttribute(LCase("Inn4_Data"), CStr(Inn4_Data))
        XmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(Ditta_Cod))
        XmlTxt.SetAttribute(LCase("Sigla_Av"), Sigla_AV)
        XmlTxt.SetAttribute(LCase("Trap_Num"), CStr(Trap_Num))
        XmlTxt.SetAttribute(LCase("Id_Insetto"), CStr(ID_Insetto))
        XmlTxt.SetAttribute(LCase("FF_Classe"), CStr(FF_Classe))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("mg"), CStr(Mg))
        XmlTxt.SetAttribute(LCase("n"), CStr(N))
        XmlTxt.SetAttribute(LCase("p"), CStr(P))
        XmlTxt.SetAttribute(LCase("k"), CStr(K))
        XmlTxt.SetAttribute(LCase("soglia_cod"), CStr(Soglia_Cod))
        XmlTxt.SetAttribute(LCase("soglia_des"), CStr(Soglia_Des))
        XmlTxt.SetAttribute(LCase("soglia_quantita"), CStr(Soglia_Qta))
        XmlTxt.SetAttribute(LCase("efficienza"), CStr(Efficienza))

        XmlTxt.SetAttribute(LCase("ricette_dettaglio_tecnico_graphickey"), CStr(Ricette_Dettaglio_Tecnico_graphickey))
        XmlTxt.SetAttribute(LCase("piezo1"), CStr(Piezo1))
        XmlTxt.SetAttribute(LCase("piezo2"), CStr(piezo2))
        XmlTxt.SetAttribute(LCase("piezo3"), CStr(piezo3))
        XmlTxt.SetAttribute(LCase("piezo4"), CStr(piezo4))

        XmlTxt.SetAttribute(LCase("cu"), CStr(Cu))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function

    '##########################################################################################
    Public Function XML_Ricetta_Destinazione(
                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                        ByVal Ricetta_SuperUser As String,
                        Optional ByVal Ricetta_Cod As Integer = 0,
                        Optional ByVal Ricetta_Operazione_Cod As Integer = 0,
                        Optional ByVal Ricetta_Dettaglio_Cod As Integer = 0,
                        Optional ByVal Ricetta_Destinazione_Cod As Integer = 0,
                        Optional ByVal Piva As String = "",
                        Optional ByVal Sa_Cod As Integer = 0,
                        Optional ByVal Appezza As Integer = 0,
                        Optional ByVal Id_Reg As Integer = 0,
                        Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                        Optional ByVal Qta As Decimal = 0,
                        Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                        Optional ByVal Validita_Fine As Date = #12/31/2100#,
                        Optional ByVal QuotaDistribuzione As Decimal = 0D,
                        Optional ByVal Qta2 As Decimal = 0D,
                        Optional ByVal Tipo_Destinazione As Integer = 0,
                        Optional ByVal magazzinoEsterno_Cod As String = "",
                        Optional ByVal magazzinoEsterno_Des As String = "",
                        Optional ByVal magazzinoEsterno_Dettagli As String = "",
                        Optional ByVal Sup_Riduzione_BufferZone As Decimal = 0,
                        Optional ByVal Perc_Riduzione_Deriva As Decimal = 0
                    ) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Ricetta_Destinazione")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        XmlTxt.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Dettaglio_Cod"), CStr(Ricetta_Dettaglio_Cod))
        XmlTxt.SetAttribute(LCase("Ricetta_Destinazione_Cod"), CStr(Ricetta_Destinazione_Cod))
        XmlTxt.SetAttribute(LCase("Piva"), CStr(Piva))
        XmlTxt.SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("appezza"), CStr(Appezza))
        XmlTxt.SetAttribute(LCase("id_reg"), CStr(Id_Reg))
        XmlTxt.SetAttribute(LCase("Programmazione_Entita_Cod"), CStr(Programmazione_Entita_Cod))
        XmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))
        XmlTxt.SetAttribute(LCase("Qta2"), CStr(Qta2))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("QuotaDistribuzione"), CStr(QuotaDistribuzione))
        XmlTxt.SetAttribute(LCase("Tipo_Destinazione"), CStr(Tipo_Destinazione))
        XmlTxt.SetAttribute(LCase("magazzinoEsterno_Cod"), CStr(magazzinoEsterno_Cod))
        XmlTxt.SetAttribute(LCase("magazzinoEsterno_Des"), CStr(magazzinoEsterno_Des))
        XmlTxt.SetAttribute(LCase("magazzinoEsterno_Dettagli"), CStr(magazzinoEsterno_Dettagli))
        XmlTxt.SetAttribute(LCase("Sup_Riduzione_BufferZone"), CStr(Sup_Riduzione_BufferZone))
        XmlTxt.SetAttribute(LCase("Perc_Riduzione_Deriva"), CStr(Perc_Riduzione_Deriva))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '###############################################################################################
    Public Function Estrai_XML_CostiAccessori(ByVal strXml As String, ByVal Elem_Cod As Integer) As String


        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XmlDocCA As New System.Xml.XmlDocument

        Dim XML_Movimento As System.Xml.XmlElement
        Dim XMLs_Movimento As System.Xml.XmlNodeList
        Dim XML_DatiMovimentiDettagli As System.Xml.XmlElement
        Dim XML_MovimentoDettaglio As System.Xml.XmlElement
        Dim XMLs_MovimentoDettaglio As System.Xml.XmlNodeList
        Dim xmlDatiMovimentiCA As System.Xml.XmlElement
        Dim xmlMovimentoCA As System.Xml.XmlElement
        Dim xmlErr As System.Xml.XmlElement
        Dim xmlMovimentoDettaglioCA As System.Xml.XmlElement
        Dim xmlDatiMovimentiDettagliCA As System.Xml.XmlElement

        Dim AppendiMovimento As Boolean

        Dim i As Integer

        Try

            AppendiMovimento = False

            strXml = strXml.Replace("TipoOperazioneDB=" + Chr(34) + "0" + Chr(34), "TipoOperazioneDB=" + Chr(34) + "1" + Chr(34))

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXml)

            'Creo un nuovo documento per i costi accessori
            XmlDocCA = New System.Xml.XmlDocument
            xmlDatiMovimentiCA = XmlDocCA.CreateElement("DatiMovimenti")

            'Recupero l'insieme dei nodi Movimento
            XMLs_Movimento = XmlDoc.GetElementsByTagName("Movimento")

            For Each XML_Movimento In XMLs_Movimento

                xmlMovimentoCA = XmlDocCA.ImportNode(XML_Movimento, True)
                For i = 0 To xmlMovimentoCA.ChildNodes.Count - 1

                    xmlMovimentoCA.RemoveChild(xmlMovimentoCA.FirstChild)

                Next

                xmlDatiMovimentiDettagliCA = XmlDocCA.CreateElement("DatiMovimenti_Dettagli")

                If XML_Movimento.GetAttribute("cau_mov") = CAU_SCARICO Then

                    '----- Tag DatiMovimenti_Dettagli

                    If XML_Movimento.HasChildNodes Then

                        XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                        '----- Tag Movimento_Dettaglio  (multiplo)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")


                        'Ciclo su tutti i nodi
                        For i = 0 To XMLs_MovimentoDettaglio.Count - 1

                            'Prendo l'i-esimo nodo della collezione
                            XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(i)

                            Select Case Elem_Cod

                                Case TRAPPOLE  'TRAPPOLE

                                    If XML_MovimentoDettaglio.GetAttribute("elem_cod") <> Elem_Cod And _
                                        XML_MovimentoDettaglio.GetAttribute("elem_cod") <> INNESCHI Then

                                        'Ho un movimento di scarico da magazzino relativo ai costi accessori
                                        AppendiMovimento = True

                                        'Attacco il nodo MovimentoDettaglio al nodo Movimento                              
                                        xmlMovimentoDettaglioCA = XmlDocCA.ImportNode(XML_MovimentoDettaglio, True)
                                        xmlDatiMovimentiDettagliCA.AppendChild(xmlMovimentoDettaglioCA)

                                    End If

                                Case SEMILAVORATI_ANIMALI 'CONSISTENZE



                                Case Else

                                    If XML_MovimentoDettaglio.GetAttribute("elem_cod") <> Elem_Cod Then

                                        'Ho un movimento di scarico da magazzino relativo ai costi accessori
                                        AppendiMovimento = True

                                        'Attacco il nodo MovimentoDettaglio al nodo Movimento                              
                                        xmlMovimentoDettaglioCA = XmlDocCA.ImportNode(XML_MovimentoDettaglio, True)
                                        xmlDatiMovimentiDettagliCA.AppendChild(xmlMovimentoDettaglioCA)

                                    End If

                            End Select

                        Next

                        If AppendiMovimento = True Then

                            xmlMovimentoCA.AppendChild(xmlDatiMovimentiDettagliCA)
                            xmlDatiMovimentiCA.AppendChild(xmlMovimentoCA)

                        End If

                        AppendiMovimento = False

                    End If

                    'Se ho un movimento di imputazione costi: manodopera, parco macchine, contoterzisti lo aggancio al nodo dei costi accessori
                ElseIf XML_Movimento.GetAttribute("cau_mov") = CAU_IMPUTAZIONE_MANODOPERA Or _
                       XML_Movimento.GetAttribute("cau_mov") = CAU_IMPUTAZIONE_PARCOMACCHINE Or _
                       XML_Movimento.GetAttribute("cau_mov") = CAU_IMPUTAZIONE_TERZISTI Then

                    'Attacco il nodo Movimento ai costi Accessori                    
                    xmlMovimentoCA = XmlDocCA.ImportNode(XML_Movimento, True)
                    xmlDatiMovimentiCA.AppendChild(xmlMovimentoCA)

                End If

            Next  'ciclo dei movimenti

            XmlDocCA.AppendChild(xmlDatiMovimentiCA)

        Catch ex As Exception
            XmlDocCA = New System.Xml.XmlDocument
            xmlErr = XmlDocCA.CreateElement("NodoErrore")
            xmlErr.SetAttribute("Message", ex.Message)
            XmlDocCA.AppendChild(xmlErr)
        End Try

        If XmlDocCA.OuterXml = "<DatiMovimenti />" Then
            XmlDocCA = New System.Xml.XmlDocument
            xmlErr = XmlDocCA.CreateElement("NodoErrore")
            xmlErr.SetAttribute("Message", "nessun costo accessorio")
            XmlDocCA.AppendChild(xmlErr)
        End If

        Return XmlDocCA.OuterXml

    End Function


    '##########################################################################################
    Public Function XML_Agenda_Ricetta(ByRef XmlDoc As XmlDocument, _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                         ByVal Ricetta_SuperUser As String, _
                                         Optional ByVal Ricetta_Cod As Integer = 0, _
                                         Optional ByVal Ricetta_Operazione_Cod As Integer = 0, _
                                         Optional ByVal Id_Agenda As Integer = 0, _
                                         Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                         Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                                        As XmlElement

        Dim DataXml As XmlElement

        DataXml = XmlDoc.CreateElement("Ricetta")

        DataXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        DataXml.SetAttribute(LCase("Ricetta_SuperUser"), Ricetta_SuperUser)
        DataXml.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        DataXml.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        DataXml.SetAttribute(LCase("Id_Agenda"), CStr(Id_Agenda))
        DataXml.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        DataXml.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))


        Return DataXml

    End Function


End Class
