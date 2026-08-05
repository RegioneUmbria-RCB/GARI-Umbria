Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class XML_Utility



    '################################################################################################
    Public Overloads Function XML_ReadLong(ByVal Valore As String) As Long
        Dim Risultato As Long

        If Valore <> "" Then
            Risultato = CLng(Valore)
        Else
            Risultato = 0
        End If

        Return Risultato

    End Function




    '################################################################################################
    Public Overloads Function XML_ReadInt(ByVal Valore As String) As Integer
        Dim Risultato As Long

        If Valore <> "" Then
            Risultato = CInt(Valore)
        Else
            Risultato = 0
        End If

        Return Risultato

    End Function

    '################################################################################################
    Public Overloads Function XML_ReadDecimal(ByVal Valore As String) As Decimal
        Dim Risultato As Decimal

        If Valore <> "" Then
            Valore = Replace(Valore, ".", ",")
            Risultato = CDbl(Valore)
        Else
            Risultato = 0
        End If

        Return Risultato

    End Function

    '###############################################################################################
    Public Function XML_ReadDate(ByVal DataItaliana As String) As Date

        Dim DataSenzaOra As String

        If DataItaliana <> "" Then
            DataSenzaOra = Left(DataItaliana, 10)
        Else : DataSenzaOra = ""
        End If

        If DataSenzaOra <> "" And DataSenzaOra <> "0.00.00" And DataSenzaOra <> New Date Then
            Return New DateTime(CInt(Right(DataSenzaOra, 4)), CInt(Mid(DataSenzaOra, 4, 2)), CInt(Left(DataSenzaOra, 2)))
        End If

        'Return CDate("#" & Mid(DataItaliana, 4, 2) & "/" & Left(DataItaliana, 2) & "/" & Right(DataItaliana, 4) & "#")


    End Function

    '###############################################################################################
    Public Function XML_ReadInteger(ByVal Valore As String) As Integer
        Dim Risultato As Integer

        If Valore <> "" Then

            Risultato = CInt(Valore)

        End If

        Return Risultato

    End Function

    '###############################################################################################
    Public Function XML_ReadShort(ByVal Valore As String) As Short
        Dim Risultato As Integer

        If Valore <> "" Then

            Risultato = CShort(Valore)

        End If

        Return Risultato

    End Function






    '########################################################################################
    'ATTENZIONE!!!!!!!!!!!!!! QUESTA FUNZIONE E' OBSOLETA!!!!!!!!!!
    'VA SOSTITUITA CON QUELLA DELL'IMPRESA O DEL CENTRO
    'PERCHE' QUESTA FUNZIONA SOLO IN INSERIMENTO
    Public Function CaricaGriglia_Codici_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function


    '########################################################################################
    'ATTENZIONE!!!!!!!!!!!!!! QUESTA FUNZIONE E' OBSOLETA!!!!!!!!!!
    'VA SOSTITUITA CON QUELLA DELL'IMPRESA O DEL CENTRO
    'PERCHE' QUESTA FUNZIONA SOLO IN INSERIMENTO
    Public Sub Inserisci_Riga_Dt_Codici_for_XML(ByRef Dt_Codici As DataTable, _
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                    ByVal Id_Cod As Integer, _
                                                    Optional ByVal Val_Cod As String = "", _
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub



    '########################################################################################
    Public Function CaricaGriglia_RisUm_for_XML() As DataTable
        'Private Sub CaricaGriglia_Dettagli()

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Risum", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Settore_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Occasionale", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ore_Settimanali", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Giorni_Ferie", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ferie_Godute", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Giorni_Malattia", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Patentino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Rilascio_Patentino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Scadenza_Patentino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ente_di_rilascio", GetType(String)))
        Dt.Columns.Add(New DataColumn("ChkSpesometro", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Saldo_Iniziale_Crediti", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Saldo_Iniziale_Debiti", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Cod_RisUm_Origine", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva_SuperUser_Origine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Dt.Columns.Add(New DataColumn("DT_ProdottiCosti", GetType(DataTable)))

        Dt.Columns.Add(New DataColumn("Qualifica_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Mansione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Classificazione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Info_Famiglia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Iva_Contatto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Conto_Econ", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Conto_Pat", GetType(Integer)))

        '------------------------------------------------------

        Return Dt

    End Function


    '########################################################################################
    'prodotti costi legati al contatto
    Public Function CaricaGriglia_ProdottiCosti_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("id", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("riferimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("elem_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("pro_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("mat_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("mezzo", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("prezzo_unitario", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("veg_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("cul_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function



    '########################################################################################
    Public Sub Inserisci_Riga_Dt_RisUm_for_XML(ByRef Dt_RisUm As DataTable,
                                                ByVal TipoOperazioneDB As Integer,
                                                ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                Optional ByVal Sa_Cod As Integer = 0,
                                                Optional ByVal Cod_Risum As Integer = 0,
                                                Optional ByVal Cod_Rapporto As Integer = 0,
                                                Optional ByVal Settore_Des As String = "",
                                                Optional ByVal Attivita_Des As String = "",
                                                Optional ByVal Occasionale As Integer = 0,
                                                Optional ByVal Ore_Settimanali As Decimal = 0,
                                                Optional ByVal Giorni_Ferie As Integer = 0,
                                                Optional ByVal Ferie_Godute As Integer = 0,
                                                Optional ByVal Giorni_Malattia As Integer = 0,
                                                Optional ByVal Patentino As String = "",
                                                Optional ByVal Data_Rilascio_Patentino As Date = AGRODATAINIZIO,
                                                Optional ByVal Data_Scadenza_Patentino As Date = AGRODATAFINE,
                                                Optional ByVal Cod_RisUm_Origine As Integer = 0,
                                                Optional ByVal Piva_SuperUser_Origine As String = "",
                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                Optional ByVal DT_ProdottiCosti As DataTable = Nothing,
                                                Optional ByVal Ente_di_rilascio As String = "",
                                                Optional ByVal ChkSpesometro As Integer = 0,
                                                Optional ByVal Saldo_Iniziale_Crediti As Decimal = 0,
                                                Optional ByVal Saldo_Iniziale_Debiti As Decimal = 0,
                                                Optional ByVal Qualifica_Cod As Integer = 0,
                                                Optional ByVal Mansione_Cod As Integer = 0,
                                                Optional ByVal Classificazione_Cod As Integer = 0,
                                                Optional ByVal Info_Famiglia As String = "",
                                                Optional ByVal Cod_Iva_Contatto As Integer = -1,
                                                Optional ByVal Cod_Conto_Econ As Integer = 0,
                                                Optional ByVal Cod_Conto_Pat As Integer = 0)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_RisUm.NewRow

        'Definisco i valori
        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Cod_Contatto") = Cod_Contatto
        Dr.Item("Cod_Risum") = Cod_Risum
        Dr.Item("Cod_Rapporto") = Cod_Rapporto
        Dr.Item("Settore_Des") = Settore_Des
        Dr.Item("Attivita_Des") = Attivita_Des
        Dr.Item("Occasionale") = Occasionale
        Dr.Item("Ore_Settimanali") = Ore_Settimanali
        Dr.Item("Giorni_Ferie") = Giorni_Ferie
        Dr.Item("Ferie_Godute") = Ferie_Godute
        Dr.Item("Giorni_Malattia") = Giorni_Malattia
        Dr.Item("Patentino") = Patentino
        Dr.Item("Data_Rilascio_Patentino") = Data_Rilascio_Patentino
        Dr.Item("Data_Scadenza_Patentino") = Data_Scadenza_Patentino
        Dr.Item("Ente_di_rilascio") = Ente_di_rilascio
        Dr.Item("ChkSpesometro") = ChkSpesometro
        Dr.Item("Saldo_Iniziale_Crediti") = Saldo_Iniziale_Crediti
        Dr.Item("Saldo_Iniziale_Debiti") = Saldo_Iniziale_Debiti

        Dr.Item("Cod_RisUm_Origine") = Cod_RisUm_Origine
        Dr.Item("Piva_SuperUser_Origine") = Piva_SuperUser_Origine
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine
        If IsNothing(DT_ProdottiCosti) Then
            Dr.Item("DT_ProdottiCosti") = System.Convert.DBNull
        Else
            Dr.Item("DT_ProdottiCosti") = DT_ProdottiCosti
        End If

        Dr.Item("Qualifica_Cod") = Qualifica_Cod
        Dr.Item("Mansione_Cod") = Mansione_Cod
        Dr.Item("Classificazione_Cod") = Classificazione_Cod
        Dr.Item("Info_Famiglia") = Info_Famiglia
        Dr.Item("Cod_Iva_Contatto") = Cod_Iva_Contatto
        Dr.Item("Cod_Conto_Econ") = Cod_Conto_Econ
        Dr.Item("Cod_Conto_Pat") = Cod_Conto_Pat

        'Associo alla tabella la nuova riga creata
        Dt_RisUm.Rows.Add(Dr)


    End Sub

    '########################################################################################
    Public Sub Inserisci_Riga_Dt_ProdottiCosti_for_XML(ByRef Dt_Costi As DataTable, _
                                                        ByVal TipoOperazioneDB As Integer, _
                                                        ByVal Id As Integer, _
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
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Costi.NewRow

        'Definisco i valori
        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("id") = Id
        Dr.Item("Piva") = Piva
        Dr.Item("riferimento") = Riferimento
        Dr.Item("elem_cod") = Elem_Cod
        Dr.Item("pro_cod") = Pro_Cod 'per i contatti  -> 0
        Dr.Item("mat_cod") = Mat_Cod 'per i contatti  -> cod_risum
        Dr.Item("udm_cod") = Udm_Cod
        Dr.Item("mezzo") = Mezzo
        Dr.Item("prezzo_unitario") = Prezzo_Unitario
        Dr.Item("veg_cod") = Veg_Cod
        Dr.Item("cul_cod") = Cul_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Costi.Rows.Add(Dr)


    End Sub

    Public Sub Inserisci_Riga_Dt_Liquidita_for_XML(ByRef Dt_Liquidita As DataTable,
                                                ByVal TipoOperazioneDB As Integer,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Cod_Contatto As String,
                                                ByVal Riferimento As String,
                                                ByVal Cau_Risorsa As String,
                                                ByVal Interbancario As String,
                                                ByVal Offset As String,
                                                ByVal Cod_Liquidita As Integer,
                                                ByVal Cod_Istituto As Integer,
                                                ByVal Nazione As String,
                                                ByVal Cifre_Controllo As String,
                                                ByVal Cin As String,
                                                ByVal Abi As String,
                                                ByVal Cab As String,
                                                ByVal Numero As String,
                                                ByVal Bic As String,
                                                ByVal ChkAbilitazione As Integer,
                                                ByVal ChkDefault As Integer,
                                                ByVal Validita_Inizio As DateTime,
                                                ByVal Validita_Fine As DateTime,
                                                ByVal Note As String)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Liquidita.NewRow

        'Definisco i valori
        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Cod_Contatto") = Cod_Contatto
        Dr.Item("Riferimento") = Riferimento
        Dr.Item("Cau_Risorsa") = Cau_Risorsa
        Dr.Item("Interbancario") = Interbancario
        Dr.Item("Offset") = Offset
        Dr.Item("Cod_Liquidita") = Cod_Liquidita
        Dr.Item("Cod_Istituto") = Cod_Istituto
        Dr.Item("Nazione") = Nazione
        Dr.Item("Cifre_Controllo") = Cifre_Controllo
        Dr.Item("Cin") = Cin
        Dr.Item("Abi") = Abi
        Dr.Item("Cab") = Cab
        Dr.Item("Numero") = Numero
        Dr.Item("Bic") = Bic
        Dr.Item("ChkAbilitazione") = ChkAbilitazione
        Dr.Item("ChkDefault") = ChkDefault
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine
        Dr.Item("Note") = Note

        'Associo alla tabella la nuova riga creata
        Dt_Liquidita.Rows.Add(Dr)


    End Sub

    Public Sub Inserisci_Riga_Dt_Conti_for_XML(ByRef Dt_Conti As DataTable,
                                                ByVal TipoOperazioneDB As Integer,
                                                ByVal Piva As String,
                                                ByVal Cod_Conto As Integer,
                                                ByVal Cod_Contatto As String,
                                                ByVal Validita_Inizio As DateTime,
                                                ByVal Validita_Fine As DateTime)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Conti.NewRow

        'Definisco i valori
        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Cod_Conto") = Cod_Conto
        Dr.Item("Cod_Contatto") = Cod_Contatto
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Conti.Rows.Add(Dr)


    End Sub

    '########################################################################################
    'crea la griglia degli indirizzi per il contatto
    Public Function CaricaGriglia_Indirizzi_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Cod_Indirizzo", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_indirizzo", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ind_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("frz_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("CAP", GetType(String)))
        Dt.Columns.Add(New DataColumn("stato", GetType(String)))
        Dt.Columns.Add(New DataColumn("note", GetType(String)))
        Dt.Columns.Add(New DataColumn("pro_cod_istat", GetType(String)))
        Dt.Columns.Add(New DataColumn("com_cod_istat", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sigla_Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Codice_Lingua", GetType(String)))
        Dt.Columns.Add(New DataColumn("Citta_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Comune_des", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function

    '########################################################################################
    'inserisce gli indirizzi per il contatto
    Public Sub Inserisci_Riga_Dt_Indirizzi_for_XML(ByRef Dt_Indirizzi As DataTable,
                                                    ByVal TipoOperazioneDB As Integer,
                                                    ByVal Piva As String,
                                                    ByVal Cod_Contatto As String,
                                                    ByVal Tipo_Indirizzo As Integer,
                                                    ByVal Pro_Cod_Istat As String,
                                                    ByVal Com_Cod_Istat As String,
                                                    Optional ByVal Cod_Indirizzo As Integer = 0,
                                                    Optional ByVal Ind_Des As String = "",
                                                    Optional ByVal Frz_Des As String = "",
                                                    Optional ByVal CAP As String = "",
                                                    Optional ByVal Stato As String = "IT",
                                                    Optional ByVal Note As String = "",
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                    Optional ByVal Sigla_Provincia As String = "00",
                                                    Optional ByVal Codice_Lingua As String = "000",
                                                    Optional ByVal Citta_Des As String = "",
                                                    Optional ByVal Comune_Des As String = "")

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Indirizzi.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Cod_Contatto") = Cod_Contatto
        Dr.Item("Tipo_Indirizzo") = Tipo_Indirizzo
        Dr.Item("Pro_Cod_Istat") = Pro_Cod_Istat
        Dr.Item("Com_Cod_Istat") = Com_Cod_Istat
        Dr.Item("Cod_Indirizzo") = Cod_Indirizzo
        Dr.Item("Ind_Des") = Ind_Des
        Dr.Item("Frz_Des") = Frz_Des
        Dr.Item("CAP") = CAP
        Dr.Item("Stato") = If(Tipo_Indirizzo = enum_TipiIndirizzi.StabileOrganizzazione, "IT", Stato)
        Dr.Item("Note") = Note
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine
        Dr.Item("Sigla_Prov") = Sigla_Provincia
        Dr.Item("Codice_Lingua") = Codice_Lingua
        Dr.Item("Citta_Des") = Citta_Des
        Dr.Item("Comune_Des") = Comune_Des

        If (Stato <> "IT" AndAlso Citta_Des <> "") Then
            Dr.Item("Frz_Des") = Citta_Des
        End If

        'Associo alla tabella la nuova riga creata
        Dt_Indirizzi.Rows.Add(Dr)

    End Sub


    '########################################################################################
    Public Function CaricaGriglia_Rubrica_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Rubrica", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function


    '########################################################################################
    Public Sub Inserisci_Riga_Dt_Rubrica_for_XML(ByRef Dt_Rubrica As DataTable, _
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                    ByVal Cod_Rubrica As Long, _
                                                    ByVal Numero As String, _
                                                    ByVal Descrizione As String, _
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Rubrica.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Cod_Rubrica") = Cod_Rubrica
        Dr.Item("Numero") = Numero
        Dr.Item("Descrizione") = Descrizione
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Rubrica.Rows.Add(Dr)


    End Sub

    '########################################################################################
    'codici legati al contatto
    Public Function CaricaGriglia_CodiciContatto_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    Public Function CaricaGriglia_Conti_for_XML() As DataTable

        Dim Dt As New DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("PIVA_SuperUser", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Conto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(DateTime)))

        Return Dt

    End Function

    Public Function CaricaGriglia_Liquidita_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))

        Dt.Columns.Add(New DataColumn("Riferimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cau_Risorsa", GetType(String)))
        Dt.Columns.Add(New DataColumn("Interbancario", GetType(String)))
        Dt.Columns.Add(New DataColumn("Offset", GetType(String)))

        Dt.Columns.Add(New DataColumn("Cod_Liquidita", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Istituto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Nazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cifre_Controllo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cin", GetType(String)))
        Dt.Columns.Add(New DataColumn("Abi", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cab", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Bic", GetType(String)))
        Dt.Columns.Add(New DataColumn("ChkAbilitazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ChkDefault", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))

        Return Dt

    End Function

    '########################################################################################
    Public Sub Inserisci_Riga_Dt_CodiciContatto_for_XML(ByRef Dt_ContattoCodici As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        ByVal Piva As String, _
                                                        ByVal Cod_Contatto As String, _
                                                        ByVal Id_Cod As Integer, _
                                                        Optional ByVal Sa_Cod As Integer = 0, _
                                                        Optional ByVal Val_Cod As String = "", _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_ContattoCodici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Cod_Contatto") = Cod_Contatto
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_ContattoCodici.Rows.Add(Dr)


    End Sub


    '########################################################################################
    'codici legati al fabbricato
    Public Function CaricaGriglia_CodiciFabbricato_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fabbricato_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    '########################################################################################
    'codici legati al centro
    Public Function CaricaGriglia_CodiciCentro_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    '########################################################################################
    'codici legati all'impresa
    Public Function CaricaGriglia_CodiciImpresa_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    '########################################################################################
    'codici legati all'appezzamento
    Public Function CaricaGriglia_CodiciAppezzamento_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    '########################################################################################
    'codici legati all'impianto
    Public Function CaricaGriglia_CodiciImpianto_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Reg", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    '########################################################################################
    'codici legati alla distinta
    Public Function CaricaGriglia_CodiciProgetto_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Reg", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function

    '########################################################################################
    'codici legati alla distinta
    Public Function CaricaGriglia_CodiciCampo_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function



    '########################################################################################
    'Fabbricato_Cod va valorizzato nel caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciFabbricato_for_XML(ByRef Dt_FabbricatoCodici As DataTable, _
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                            ByVal Piva As String, _
                                                            ByVal Sa_Cod As Integer, _
                                                            ByVal Fabbricato_Cod As Integer, _
                                                            ByVal Id_Cod As Integer, _
                                                            Optional ByVal Val_Cod As String = "", _
                                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_FabbricatoCodici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Fabbricato_Cod") = Fabbricato_Cod
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_FabbricatoCodici.Rows.Add(Dr)


    End Sub


    '########################################################################################
    'Sa_Cod va valorizzato nel caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciCentro_for_XML(ByRef Dt_Codici As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        ByVal Piva As String, _
                                                        ByVal Sa_Cod As Integer, _
                                                        ByVal Id_Cod As Integer, _
                                                        Optional ByVal Val_Cod As String = "", _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub

    '########################################################################################
    'Piva va valorizzata nel caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciImpresa_for_XML(ByRef Dt_Codici As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        ByVal Piva As String, _
                                                        ByVal Id_Cod As Integer, _
                                                        Optional ByVal Val_Cod As String = "", _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub


    '########################################################################################
    'Sa_Cod e appezza vanno valorizzati in caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(ByRef Dt_Codici As DataTable, _
                                                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                                ByVal Piva As String, _
                                                                ByVal Sa_Cod As Integer, _
                                                                ByVal Appezza As Integer, _
                                                                ByVal Id_Cod As Integer, _
                                                                Optional ByVal Val_Cod As String = "", _
                                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                                Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Appezza") = Appezza
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub

    '########################################################################################
    'Sa_Cod e appezza vanno valorizzati in caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciCampo_for_XML(ByRef Dt_Codici As DataTable, _
                                                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                                ByVal Piva As String, _
                                                                ByVal Sa_Cod As Integer, _
                                                                ByVal Campo_Cod As Integer, _
                                                                ByVal Id_Cod As Integer, _
                                                                ByVal Val_Cod As String, _
                                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                                Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Campo_Cod") = Campo_Cod
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub


    '########################################################################################
    'Sa_Cod, appezza e id_reg vanno valorizzati in caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciImpianto_for_XML(ByRef Dt_Codici As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        ByVal Piva As String, _
                                                        ByVal Sa_Cod As Integer, _
                                                        ByVal Appezza As Integer, _
                                                        ByVal Id_Reg As Integer, _
                                                        ByVal Id_Cod As Integer, _
                                                        Optional ByVal Val_Cod As String = "", _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Appezza") = Appezza
        Dr.Item("Id_Reg") = Id_Reg
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub


    '########################################################################################
    'Sa_Cod, appezza, id_reg e progetto_cod vanno valorizzati in caso di modifica
    'in caso di inserimento impostare = 0
    Public Sub Inserisci_Riga_Dt_CodiciProgetto_for_XML(ByRef Dt_Codici As DataTable, _
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                        ByVal Piva As String, _
                                                        ByVal Sa_Cod As Integer, _
                                                        ByVal Appezza As Integer, _
                                                        ByVal Id_Reg As Integer, _
                                                        ByVal Progetto_Cod As Integer, _
                                                        ByVal Id_Cod As Integer, _
                                                        Optional ByVal Val_Cod As String = "", _
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_Codici.NewRow

        'Definisco i valori

        Dr.Item("TipoOperazioneDB") = TipoOperazioneDB
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Appezza") = Appezza
        Dr.Item("Id_Reg") = Id_Reg
        Dr.Item("Progetto_Cod") = Progetto_Cod
        Dr.Item("Id_Cod") = Id_Cod
        Dr.Item("Val_Cod") = Val_Cod
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_Codici.Rows.Add(Dr)


    End Sub


    '########################################################################################
    'Stalla legata al fabbricato
    Public Function CaricaGriglia_Stalla_for_XML() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable


        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))

            .Add(New DataColumn("Piva", GetType(String)))
            .Add(New DataColumn("Sa_Cod", GetType(Integer)))
            .Add(New DataColumn("STA_NUM", GetType(Integer)))
            .Add(New DataColumn("Sta_Des", GetType(String)))
            .Add(New DataColumn("Ausl_Cod", GetType(String)))
            .Add(New DataColumn("Dat_Costr", GetType(Date)))
            .Add(New DataColumn("Dat_Chiu", GetType(Date)))
            .Add(New DataColumn("Cod_Fabb", GetType(String)))
            .Add(New DataColumn("Gen_Cod", GetType(Integer)))
            .Add(New DataColumn("Spe_Cod", GetType(Integer)))
            .Add(New DataColumn("Ipro_Cod", GetType(Integer)))
            .Add(New DataColumn("X", GetType(String)))
            .Add(New DataColumn("Y", GetType(String)))
            .Add(New DataColumn("Dat_Ult_Agg", GetType(Date)))
            .Add(New DataColumn("Latitudine", GetType(Integer)))
            .Add(New DataColumn("Longitudine", GetType(Integer)))
            .Add(New DataColumn("CUAA_Proprietario", GetType(String)))
            .Add(New DataColumn("Denominazione_Proprietario", GetType(String)))
            .Add(New DataColumn("CUAA_Detentore", GetType(String)))
            .Add(New DataColumn("Denominazione_Detentore", GetType(String)))
            .Add(New DataColumn("Validita_Inizio", GetType(String)))
            .Add(New DataColumn("Validita_Fine", GetType(String)))

        End With

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt


    End Function




    '########################################################################################
    Public Sub Inserisci_Riga_Dt_Stalla_for_XML( _
                                    ByRef DT_Stalla As DataTable, _
                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    ByVal Piva As String, _
                                    ByVal Sa_Cod As Int32, _
                                    ByVal STA_NUM As Int32, _
                                    ByVal Sta_Des As String, _
                                    ByVal Ausl_Cod As String, _
                                    ByVal Dat_Costr As Date, _
                                    ByVal Dat_Chiu As Date, _
                                    ByVal Cod_Fabb As String, _
                                    ByVal Gen_Cod As Int32, _
                                    ByVal Spe_Cod As Int32, _
                                    ByVal Ipro_Cod As Int32, _
                                    ByVal X As String, _
                                    ByVal Y As String, _
                                    ByVal Dat_Ult_Agg As Date, _
                                    ByVal Latitudine As Int32, _
                                    ByVal Longitudine As Int32, _
                                    ByVal CUAA_Proprietario As String, _
                                    ByVal Denominazione_Proprietario As String, _
                                    ByVal CUAA_Detentore As String, _
                                    ByVal Denominazione_Detentore As String, _
                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim DR As DataRow

        'Creo una nuova riga
        DR = DT_Stalla.NewRow

        'Definisco i valori

        With DR

            .Item("TipoOperazioneDB") = TipoOperazioneDB

            .Item("Piva") = Piva
            .Item("Sa_Cod") = Sa_Cod
            .Item("STA_NUM") = STA_NUM
            .Item("Sta_Des") = Sta_Des
            .Item("Ausl_Cod") = Ausl_Cod
            .Item("Dat_Costr") = Dat_Costr
            .Item("Dat_Chiu") = Dat_Chiu
            .Item("Cod_Fabb") = Cod_Fabb
            .Item("Gen_Cod") = Gen_Cod
            .Item("Spe_Cod") = Spe_Cod
            .Item("Ipro_Cod") = Ipro_Cod
            .Item("X") = X
            .Item("Y") = Y
            .Item("Dat_Ult_Agg") = Dat_Ult_Agg
            .Item("Latitudine") = Latitudine
            .Item("Longitudine") = Longitudine
            .Item("CUAA_Proprietario") = CUAA_Proprietario
            .Item("Denominazione_Proprietario") = Denominazione_Proprietario
            .Item("CUAA_Detentore") = CUAA_Detentore
            .Item("Denominazione_Detentore") = Denominazione_Detentore

            .Item("Validita_Inizio") = Validita_Inizio
            .Item("Validita_Fine") = Validita_Fine

        End With

        'Associo alla tabella la nuova riga creata
        DT_Stalla.Rows.Add(DR)

    End Sub


    '########################################################################################
    Public Function CaricaGriglia_ParamQual_for_XML() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("valore_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("valore_min", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Valore_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("chkregistri", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ChkCalibri", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '------------------------------------------------------

        'Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("Cod_Risum")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function


    '########################################################################################
    Public Sub Inserisci_Riga_Dt_ParamQual_for_XML(ByRef Dt_ParamQual As DataTable, _
                                                    ByVal Piva As String, _
                                                    ByVal Mat_Cod As Integer, _
                                                    Optional ByVal Sa_Cod As Integer = 0, _
                                                    Optional ByVal Tipo As String = "", _
                                                    Optional ByVal Tipo_Cod As Integer = 0, _
                                                    Optional ByVal Udm_Cod As Integer = 0, _
                                                    Optional ByVal Valore_Des As String = "", _
                                                    Optional ByVal Valore_Min As Decimal = 0, _
                                                    Optional ByVal Valore_Max As Decimal = 0, _
                                                    Optional ByVal ChkRegistri As Integer = 0, _
                                                    Optional ByVal ChkCalibri As Integer = 0, _
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = Dt_ParamQual.NewRow

        'Definisco i valori
        Dr.Item("Piva") = Piva
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Mat_Cod") = Mat_Cod
        Dr.Item("tipo") = Tipo
        Dr.Item("tipo_cod") = Tipo_Cod
        Dr.Item("udm_cod") = Udm_Cod
        Dr.Item("valore_des") = Valore_Des
        Dr.Item("valore_min") = Valore_Min
        Dr.Item("Valore_Max") = Valore_Max
        Dr.Item("chkregistri") = ChkRegistri
        Dr.Item("ChkCalibri") = ChkCalibri
        Dr.Item("Validita_Inizio") = Validita_Inizio
        Dr.Item("Validita_Fine") = Validita_Fine

        'Associo alla tabella la nuova riga creata
        Dt_ParamQual.Rows.Add(Dr)


    End Sub


    '########################################################################################
    Public Function CaricaGriglia_DT_DocumentiAcquisto_WS() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("piva_fornitore_cliente", GetType(String)))
        Dt.Columns.Add(New DataColumn("ragsoc_fornitore_cliente", GetType(String)))
        Dt.Columns.Add(New DataColumn("piva_impresa", GetType(String)))
        Dt.Columns.Add(New DataColumn("ragsoc_impresa", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_documento", GetType(String)))
        Dt.Columns.Add(New DataColumn("documento_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("prefisso_numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("suffisso_numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("data", GetType(String)))
        Dt.Columns.Add(New DataColumn("dettagli_prodotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("cod_magazzino", GetType(String)))
        Dt.Columns.Add(New DataColumn("magazzino", GetType(String)))

        'Dt.Columns.Add(New DataColumn("categoria_magazzino", GetType(String)))
        'Dt.Columns.Add(New DataColumn("descrizione_prodotto", GetType(String)))
        'Dt.Columns.Add(New DataColumn("numero_registrazione", GetType(String)))
        'Dt.Columns.Add(New DataColumn("unita_misura", GetType(String)))
        'Dt.Columns.Add(New DataColumn("qta", GetType(String)))

        Return Dt

    End Function


    ''########################################################################################
    'Public Function CaricaGriglia_DT_DocumentiVendita_WS() As DataTable

    '    '----- Definizione delle variabili
    '    Dim Dt As New DataTable

    '    '----- Definisco la struttura del DataTable
    '    Dt.Columns.Add(New DataColumn("piva_cliente", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("ragsoc_cliente", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("piva_impresa", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("ragsoc_impresa", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("tipo_documento", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("documento_des", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("prefisso_numero", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("numero", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("suffisso_numero", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("data", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("dettagli_prodotto", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("cod_magazzino", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("magazzino", GetType(String)))

    '    'Dt.Columns.Add(New DataColumn("categoria_magazzino", GetType(String)))
    '    'Dt.Columns.Add(New DataColumn("descrizione_prodotto", GetType(String)))
    '    'Dt.Columns.Add(New DataColumn("numero_registrazione", GetType(String)))
    '    'Dt.Columns.Add(New DataColumn("unita_misura", GetType(String)))
    '    'Dt.Columns.Add(New DataColumn("qta", GetType(String)))

    '    Return Dt

    'End Function




End Class
