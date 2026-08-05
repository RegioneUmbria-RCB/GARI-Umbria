
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreSementieriDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class Interferenze

#Region "Funzioni Shared da progetto AgronicaSementi_5"

    '################################################################################
    Public Shared Sub Analizza_Interferenze(
                                ByRef DT_Impianti As DataTable,
                                ByRef DT_Distanze As DataTable,
                                ByRef DT_DistanzeBietola As DataTable,
                                ByRef MatriceDistanze(,) As Double,
                                ByRef DT_Interferenze As DataTable,
                                ByRef NumeroInterferenze As Integer,
                                ByVal MoltiplicatoreDistanze As Double,
                                ByVal Flag_Interferenze_AA As Boolean,
                                ByVal Flag_Interferenze_AX As Boolean,
                                ByVal Flag_Interferenze_XX As Boolean,
                                ByVal Flag_Interferenze_XY As Boolean,
                                ByVal Flag_VisualizzaNonInterferenze As Boolean,
                                ByVal UtenteAmministratore As Boolean,
                                ByRef objSession As System.Web.SessionState.HttpSessionState,
                                ByRef objParametri_Server As AgronicaCoreParametri)

        Dim iA As Integer
        Dim iB As Integer

        'Dim DR_Impianto_A As DataRow
        'Dim DR_Impianto_B As DataRow
        'Dim DR_Distanza As DataRow

        Dim Data_Ia As Date
        Dim Data_Fa As Date
        Dim Data_Ib As Date
        Dim Data_Fb As Date

        Dim Specie_A As Integer
        Dim Specie_B As Integer

        Dim PivaReferente_A As String
        Dim PivaReferente_B As String

        Dim CoppiaEsaminata As Boolean
        Dim MaxValueDistanzaMinima As Double
        Dim MaxValueDistanzaMinimaX As Double
        Dim DistanzaEffettiva As Double

        Dim TecnicoPiva As String

        Dim TipoInterferenza As Integer
        Dim Motivazione_Cod As Integer = 0
        Dim Motivazione_Des As String = ""
        Dim Flag_Bietola As Boolean
        Dim DistanzaMinima As Double
        Dim DistanzaMinima_BietolaBase As Double
        Dim DistanzaMinima_BietolaCert As Double

        Dim Flag_InformazioniInsufficienti_A As Boolean
        Dim Flag_InformazioniInsufficienti_B As Boolean
        Dim FlagIntero As Integer



        '--------------------------------------------

        'Inizializzo
        NumeroInterferenze = 0

        'Genero la struttura del datatable delle interferenze
        Call Datatable_Interferenze_Prepara(DT_Interferenze)

        '--------------------------------------------

        'Recupero la piu' grande delle distanze minime di legge
        Recupera_MaxValue_DistanzaMinima(MaxValueDistanzaMinima, objParametri_Server)

        'La moltiplico per il MOLTIPLICATORE
        MaxValueDistanzaMinimaX = MaxValueDistanzaMinima * MoltiplicatoreDistanze

        '--------------------------------------------

        'Recupero la Piva dell'utente attuale ...
        TecnicoPiva = objSession("ASG_Utente_CodFiscale")

        '--------------------------------------------





        '--------------------------------------------

        Dim ModalitaDEBUG As Boolean

        ModalitaDEBUG = System.Configuration.ConfigurationManager.AppSettings("Debug_Mode")

        '--------------------------------------------


        'Ciclo doppio
        For iA = 1 To DT_Impianti.Rows.Count - 1
            For iB = 0 To (iA - 1)


                CoppiaEsaminata = False


                '--------------------------------------------------------------------------
                '--- Verifico se l'utente desidera controllare l'interferenza
                '--------------------------------------------------------------------------

                PivaReferente_A = DT_Impianti.Rows(iA).Item("Referente_Piva")
                PivaReferente_B = DT_Impianti.Rows(iB).Item("Referente_Piva")

                '-----

                If (CoppiaEsaminata = False) Then

                    'Non mi interessa il caso AA 
                    If (Flag_Interferenze_AA = False) And
                       (PivaReferente_A = PivaReferente_B) And (PivaReferente_A = TecnicoPiva) Then

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            If ModalitaDEBUG = True Then

                                Datatable_Interferenze_Inserisci(
                                                DT_Interferenze,
                                                DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                                DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                                False,
                                                False, False,
                                                0, -11, "Caso AA non richiesto",
                                                0, 0, 0, 0)

                            End If
                        End If

                        CoppiaEsaminata = True

                    End If
                End If

                '-----

                If (CoppiaEsaminata = False) Then

                    'Non mi interessa il caso AX 
                    If (Flag_Interferenze_AX = False) And
                       (PivaReferente_A <> PivaReferente_B) And
                       ((PivaReferente_A = TecnicoPiva) Or (PivaReferente_B = TecnicoPiva)) Then

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            If ModalitaDEBUG = True Then

                                Datatable_Interferenze_Inserisci(
                                                DT_Interferenze,
                                                DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                                DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                                False,
                                                False, False,
                                                0, -12, "Caso AX non richiesto",
                                                0, 0, 0, 0)
                            End If
                        End If

                        CoppiaEsaminata = True

                    End If
                End If

                '-----

                If (CoppiaEsaminata = False) Then

                    'Non mi interessa il caso XX 
                    If (Flag_Interferenze_XX = False) And
                       (PivaReferente_A = PivaReferente_B) And (PivaReferente_A <> TecnicoPiva) Then

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            If ModalitaDEBUG = True Then

                                Datatable_Interferenze_Inserisci(
                                                DT_Interferenze,
                                                DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                                DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                                False,
                                                False, False,
                                                0, -13, "Caso XX non richiesto",
                                                0, 0, 0, 0)

                            End If
                        End If

                        CoppiaEsaminata = True

                    End If
                End If

                '-----

                If (CoppiaEsaminata = False) Then

                    'Non mi interessa il caso XY 
                    If (Flag_Interferenze_XY = False) And
                       (PivaReferente_A <> PivaReferente_B) And
                       ((PivaReferente_A <> TecnicoPiva) And (PivaReferente_B <> TecnicoPiva)) Then

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            If ModalitaDEBUG = True Then

                                Datatable_Interferenze_Inserisci(
                                                DT_Interferenze,
                                                DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                                DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                                False,
                                                False, False,
                                                0, -14, "Caso XY non richiesto",
                                                0, 0, 0, 0)

                            End If
                        End If

                        CoppiaEsaminata = True

                    End If
                End If


                '--------------------------------------------------------------------------
                '--- Verifico se gli intervalli di VALIDITA' sono disgiunti
                '--------------------------------------------------------------------------

                If (CoppiaEsaminata = False) Then

                    Data_Ia = DT_Impianti.Rows(iA).Item("Data_Inizio")
                    Data_Fa = DT_Impianti.Rows(iA).Item("Data_Fine")
                    Data_Ib = DT_Impianti.Rows(iB).Item("Data_Inizio")
                    Data_Fb = DT_Impianti.Rows(iB).Item("Data_Fine")

                    If (Data_Ia > Data_Fb) Or (Data_Fa < Data_Ib) Then

                        'Intervalli disgiunti ==> nessuna interferenza

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            Datatable_Interferenze_Inserisci(
                                            DT_Interferenze,
                                            DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                            DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                            False,
                                            False, False,
                                            0, -1, "Impianti non contemporanei",
                                            0, 0, 0, 0)

                            CoppiaEsaminata = True

                        End If

                    End If

                End If

                '--------------------------------------------------------------------------
                '--- Verifico se la DISTANZA e' superiore alla massima distanza possibile
                '--------------------------------------------------------------------------

                If (CoppiaEsaminata = False) Then

                    '--------------------------------------------------------------------------------------
                    ' MATRICE DELLE DISTANZE
                    '
                    '   Creo una matrice (x,y) 
                    '   con la convenzione che 
                    '           nelle celle con x=y non c'e' nulla (zero)
                    '           nelle celle con x>y ci sono le distanze tra i poligoni x-esimo e y-esimo
                    '           nelle celle con x<y ci sono le distanze minime consentite (in funzione delle coltura)
                    '
                    '---------------------------------------------------------------------------------------

                    'NOTA Ricordo che vale sempre iA>iB
                    DistanzaEffettiva = MatriceDistanze(iA, iB)

                    'Verifico se la distanza effettiva e' maggiore della massima distanza di legge 
                    'moltiplicata (in precedenza) per il moltiplicatore ...
                    If (DistanzaEffettiva > MaxValueDistanzaMinimaX) Then

                        'Distanza di sicurezza ==> nessuna interferenza

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            Datatable_Interferenze_Inserisci(
                                            DT_Interferenze,
                                            DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                            DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                            False,
                                            False, False,
                                            0, -2, "Distanza non critica",
                                            MaxValueDistanzaMinimaX, DistanzaEffettiva, MaxValueDistanzaMinima, 0)

                            CoppiaEsaminata = True

                        End If

                    End If

                End If

                '--------------------------------------------------------------------------
                '--- Verifico se la SPECIE [sementieri] e' diversa
                '--------------------------------------------------------------------------

                If (CoppiaEsaminata = False) Then

                    Specie_A = DT_Impianti.Rows(iA).Item("ID_Specie")
                    Specie_B = DT_Impianti.Rows(iB).Item("ID_Specie")

                    If (Specie_A <> Specie_B) Then

                        'Specie Diverse ==> nessuna interferenza

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            Datatable_Interferenze_Inserisci(
                                            DT_Interferenze,
                                            DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                            DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                            False,
                                            False, False,
                                            0, -3, "Specie vegetali diverse",
                                            0, 0, 0, 0)

                            CoppiaEsaminata = True

                        End If

                    End If

                End If



                '--------------------------------------------------------------------------
                '--- 
                '--------------------------------------------------------------------------

                '
                '
                '   varie ...
                '
                '

                '--------------------------------------------------------------------------
                '--- Verifico l'interferenza dovuta alla coltura ...
                '--------------------------------------------------------------------------

                Motivazione_Cod = 0
                Motivazione_Des = ""
                Flag_Bietola = False
                DistanzaMinima = -1
                DistanzaMinima_BietolaBase = -1
                DistanzaMinima_BietolaCert = -1


                If (CoppiaEsaminata = False) Then

                    'NOTA Ricordo che vale sempre iA>iB
                    DistanzaEffettiva = MatriceDistanze(iA, iB)



                    FlagIntero = DT_Impianti.Rows(iA).Item("Flag_DatiSpecie_Completi")
                    Select Case FlagIntero
                        Case 0
                            Flag_InformazioniInsufficienti_A = True
                        Case 1
                            Flag_InformazioniInsufficienti_A = False
                    End Select

                    FlagIntero = DT_Impianti.Rows(iB).Item("Flag_DatiSpecie_Completi")
                    Select Case FlagIntero
                        Case 0
                            Flag_InformazioniInsufficienti_B = True
                        Case 1
                            Flag_InformazioniInsufficienti_B = False
                    End Select


                    'Recupero la distanza minima di legge fra i due impianti ...
                    Confrontatore(
                                DT_Distanze, DT_DistanzeBietola,
                                DT_Impianti.Rows(iA).Item("ID_Specie"),
                                DT_Impianti.Rows(iA).Item("ID_SottoSpecie"),
                                DT_Impianti.Rows(iA).Item("ID_Gruppo"),
                                DT_Impianti.Rows(iA).Item("ID_GenoTipo"),
                                DT_Impianti.Rows(iB).Item("ID_Specie"),
                                DT_Impianti.Rows(iB).Item("ID_SottoSpecie"),
                                DT_Impianti.Rows(iB).Item("ID_Gruppo"),
                                DT_Impianti.Rows(iB).Item("ID_GenoTipo"),
                                Flag_InformazioniInsufficienti_A,
                                Flag_InformazioniInsufficienti_B,
                                TipoInterferenza,
                                Motivazione_Cod, Motivazione_Des,
                                Flag_Bietola,
                                DistanzaMinima,
                                DistanzaMinima_BietolaBase,
                                DistanzaMinima_BietolaCert)


                    'Verifico se la distanza minima e' rispettata ...
                    If (DistanzaEffettiva < DistanzaMinima * MoltiplicatoreDistanze) Then


                        'INTERFERENZA

                        Datatable_Interferenze_Inserisci(
                                        DT_Interferenze,
                                        DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                        DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                        Flag_Bietola,
                                        Flag_InformazioniInsufficienti_A,
                                        Flag_InformazioniInsufficienti_B,
                                        TipoInterferenza,
                                        Motivazione_Cod, Motivazione_Des,
                                        DistanzaMinima * MoltiplicatoreDistanze,
                                        DistanzaEffettiva,
                                        DistanzaMinima,
                                        DistanzaMinima_BietolaBase)

                        CoppiaEsaminata = True

                    Else


                        'NON INTERFERENZA

                        'Se voglio vedere anche anche le NON-Interferenze ...
                        If (Flag_VisualizzaNonInterferenze = True) Then

                            Datatable_Interferenze_Inserisci(
                                            DT_Interferenze,
                                            DT_Impianti.Rows(iA).Item("UNID_Impianto"),
                                            DT_Impianti.Rows(iB).Item("UNID_Impianto"),
                                            Flag_Bietola,
                                            Flag_InformazioniInsufficienti_A,
                                            Flag_InformazioniInsufficienti_B,
                                            0,
                                            -20, "Distanza Minima Rispettata",
                                            DistanzaMinima * MoltiplicatoreDistanze,
                                            DistanzaEffettiva,
                                            DistanzaMinima,
                                            DistanzaMinima_BietolaBase)

                            CoppiaEsaminata = True

                        End If

                    End If

                End If

                '--------------------------------------------------------------------------
                '--------------------------------------------------------------------------
                '--------------------------------------------------------------------------

            Next iB
        Next iA


    End Sub




    '################################################################################
    Private Shared Sub Datatable_Interferenze_Prepara(ByRef DT As DataTable)

        '----- Genero la struttura del Datatable

        DT.Columns.Add(New DataColumn("UNID_Impianto_A", GetType(Integer)))
        DT.Columns.Add(New DataColumn("UNID_Impianto_B", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Flag_Bietola", GetType(Boolean)))
        DT.Columns.Add(New DataColumn("Flag_Informazioni_Insufficienti_A", GetType(Boolean)))
        DT.Columns.Add(New DataColumn("Flag_Informazioni_Insufficienti_B", GetType(Boolean)))

        'TipoInterferenza   {0=Nessuna  1=Possibile  2=Sicura}
        DT.Columns.Add(New DataColumn("TipoInterferenza", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Motivazione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Motivazione_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Distanza_Minima", GetType(String)))
        DT.Columns.Add(New DataColumn("Distanza_Effettiva", GetType(String)))
        DT.Columns.Add(New DataColumn("Distanza_Legge", GetType(String)))
        DT.Columns.Add(New DataColumn("Distanza_Legge_Bietola", GetType(String)))

    End Sub



    '################################################################################
    Private Shared Sub Datatable_Interferenze_Inserisci(
                                    ByRef DT_Interferenze As DataTable,
                                    ByVal UNID_Impianto_A As Integer,
                                    ByVal UNID_Impianto_B As Integer,
                                    ByVal Flag_Bietola As Boolean,
                                    ByVal Flag_Informazioni_Insufficienti_A As Boolean,
                                    ByVal Flag_Informazioni_Insufficienti_B As Boolean,
                                    ByVal TipoInterferenza As Integer,
                                    ByVal Motivazione_Cod As Integer,
                                    ByVal Motivazione_Des As String,
                                    ByVal Distanza_Minima As Double,
                                    ByVal Distanza_Effettiva As Double,
                                    ByVal Distanza_Legge As Double,
                                    ByVal Distanza_Legge_Bietola As Double)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT_Interferenze.NewRow

        '----- Valorizzo le colonne

        Dr.Item("UNID_Impianto_A") = UNID_Impianto_A
        Dr.Item("UNID_Impianto_B") = UNID_Impianto_B
        Dr.Item("Flag_Bietola") = Flag_Bietola
        Dr.Item("Flag_Informazioni_Insufficienti_A") = Flag_Informazioni_Insufficienti_A
        Dr.Item("Flag_Informazioni_Insufficienti_B") = Flag_Informazioni_Insufficienti_B
        Dr.Item("TipoInterferenza") = TipoInterferenza
        Dr.Item("Motivazione_Cod") = Motivazione_Cod
        Dr.Item("Motivazione_Des") = Motivazione_Des
        Dr.Item("Distanza_Minima") = Distanza_Minima
        Dr.Item("Distanza_Effettiva") = Distanza_Effettiva
        Dr.Item("Distanza_Legge") = Distanza_Legge
        Dr.Item("Distanza_Legge_Bietola") = Distanza_Legge_Bietola

        '----- Aggiungo la nuova riga al DataTable

        DT_Interferenze.Rows.Add(Dr)


    End Sub






    '########################################################################################
    Private Shared Sub Recupera_MaxValue_DistanzaMinima(
                                    ByRef MaxValueDistanzaMinima As Double,
                                    objParametri_server As AgronicaCoreParametri)

        Dim DT_Varie As DataTable
        Dim DT_Bietola As DataTable
        Dim i As Integer
        Dim MaxDistanza_V As Double
        Dim MaxDistanza_B As Double

        Dim leggi_SpecieDistanze As New AgronicaCoreSementieriDAL.Mappatura_Specie_Distanze_R
        DT_Varie = leggi_SpecieDistanze.Leggi("", objParametri_server)
        'DT_Varie = AD_SpecieDistanze_Leggi(objServer, objSession, objPage)

        Dim leggi_SpecieDistanzeBietola As New AgronicaCoreSementieriDAL.Mappatura_Specie_Distanze_Bietola_R
        DT_Bietola = leggi_SpecieDistanzeBietola.Leggi("", objParametri_server)
        'DT_Bietola = AD_SpecieDistanzeBietola_Leggi(objServer, objSession, objPage)

        MaxDistanza_V = 0
        MaxDistanza_B = 0

        For i = 0 To DT_Varie.Rows.Count - 1
            If (CDbl(DT_Varie.Rows(i).Item("Distanza_MaxValue")) > MaxDistanza_V) Then
                MaxDistanza_V = CDbl(DT_Varie.Rows(i).Item("Distanza_MaxValue"))
            End If
        Next

        For i = 0 To DT_Bietola.Rows.Count - 1
            If (CDbl(DT_Bietola.Rows(i).Item("Div_Varieta_in_Gruppo")) > MaxDistanza_B) Then
                MaxDistanza_B = CDbl(DT_Bietola.Rows(i).Item("Div_Varieta_in_Gruppo"))
            End If
        Next

        'Risultato
        If (MaxDistanza_V > MaxDistanza_B) Then

            MaxValueDistanzaMinima = MaxDistanza_V
        Else
            MaxValueDistanzaMinima = MaxDistanza_B
        End If

    End Sub








    '########################################################################################
    Private Shared Sub Confrontatore(
                                ByRef DT_Distanze As DataTable,
                                ByRef DT_Distanze_AUX As DataTable,
                                ByVal ID_Specie_A As Integer,
                                ByVal ID_SottoSpecie_A As Integer,
                                ByVal ID_Gruppo_A As Integer,
                                ByVal ID_Genotipo_A As Integer,
                                ByVal ID_Specie_B As Integer,
                                ByVal ID_SottoSpecie_B As Integer,
                                ByVal ID_Gruppo_B As Integer,
                                ByVal ID_Genotipo_B As Integer,
                                ByVal Flag_InformazioniInsufficienti_A As Boolean,
                                ByVal Flag_InformazioniInsufficienti_B As Boolean,
                                ByRef TipoInterferenza As Integer,
                                ByRef Motivazione_Cod As Integer,
                                ByRef Motivazione_Des As String,
                                ByRef Flag_Bietola As Boolean,
                                ByRef DistanzaMinima As Double,
                                ByRef DistanzaMinima_BietolaBase As Double,
                                ByRef DistanzaMinima_BietolaCert As Double)

        Dim Distanza_MaxValue As Integer
        Dim Dist_Genotipi As Integer
        Dim Dist_Varieta_OP As Integer
        Dim Dist_Varieta_HY As Integer
        Dim Dist_Gruppi_OP As Integer
        Dim Dist_Gruppi_HY As Integer
        Dim Dist_SottoSpecie_OP As Integer
        Dim Dist_SottoSpecie_HY As Integer
        Dim Flag_BietolaPlurigerme As Boolean


        '-------------------------------
        '--- Analisi ....
        '-------------------------------

        'Verifico se ho la stessa SPECIE ... 
        '(non puo' essere diversamente perche' ho gia' verificato in precedenza il caso contrario)

        If ID_Specie_A = ID_Specie_B Then

            'Verifico se sono nel caso particolare della bietola ...
            If (ID_Specie_A = 1) Then

                '###############################################################
                '###############################################################
                '#####  BIETOLA  ###############################################
                '###############################################################
                '###############################################################

                Flag_Bietola = True

                '-------------------------------
                '--- Tipo di Interferenza
                '-------------------------------

                'NOTA
                'Poiche' non ho informazioni sul tipo di semente
                'che voglio generare : Certificata, Base o PreBase
                'Ho comunque sempre delle interferenze che richiedono
                'ulteriore analisi .....

                TipoInterferenza = 1        '   Possibile INTERFERENZA 




                '////////////////////////////////////////////
                '///  Semente di Categoria BASE e PREBASE  //
                '////////////////////////////////////////////


                '-------------------------------
                '--- Distanza
                '-------------------------------

                'Mi procuro le distanze per questa Specie ...
                Informazioni_from_DT_Distanze(
                                             DT_Distanze,
                                             ID_Specie_A,
                                             Distanza_MaxValue,
                                             Dist_Genotipi,
                                             Dist_Varieta_OP,
                                             Dist_Varieta_HY,
                                             Dist_Gruppi_OP,
                                             Dist_Gruppi_HY,
                                             Dist_SottoSpecie_OP,
                                             Dist_SottoSpecie_HY)

                'Se le informazioni anche per uno solo degli impianti sono incomplete ...

                If (Flag_InformazioniInsufficienti_A = True) Or
                   (Flag_InformazioniInsufficienti_B = True) Then

                    'NOTA
                    'Se non ho informazioni sul GRUPPO devo considerare il caso peggiore !!!

                    'Mi procuro la distanza maggiore
                    Informazioni_from_DT_Distanze_AUX_MaxDistanza(
                                                            DT_Distanze_AUX,
                                                            DistanzaMinima_BietolaBase)

                    DistanzaMinima_BietolaCert = Distanza_MaxValue

                    If (DistanzaMinima_BietolaCert < DistanzaMinima_BietolaBase) Then
                        DistanzaMinima = DistanzaMinima_BietolaBase
                    Else
                        DistanzaMinima = DistanzaMinima_BietolaCert
                    End If

                    'Esito ...

                    TipoInterferenza = 1    'Possibile INTERFERENZA 
                    Motivazione_Cod = 1     'Bietola - Informazioni Incomplete
                    Motivazione_Des = "Poiche' le informazioni fornite sono incomplete, " & "<br>" &
                                      "si e' preso in considerazione il caso peggiore ... " & "<br>" &
                                      "E' necessario analizzare piu' approfonditamente " & "<br>" &
                                      "questa possibile interferenza !!!"

                    Exit Sub

                Else

                    'Se ho informazioni complete ...

                    If (ID_Gruppo_A = ID_Gruppo_B) Then

                        'Elementi ausiliari ...
                        Informazioni_from_DT_Distanze_AUX(
                                                    DT_Distanze_AUX,
                                                    ID_Specie_A,
                                                    ID_Gruppo_A,
                                                    DistanzaMinima_BietolaBase)

                        Flag_BietolaPlurigerme = True

                        TipoInterferenza = 1    'Possibile INTERFERENZA 
                        Motivazione_Cod = 2     'Bietola - Informazioni Complete - Stesso GRUPPO - Plurigerme
                        'Motivazione_Des = "Cat. BASE e PRE-BASE : <br>Le due colture appartengono allo stesso gruppo PLURIGERME"
                        Motivazione_Des = "<b>Cat. BASE e PRE-BASE :</b> <br>Stesso GRUPPO - PLURIGERME"


                        'Se ottengo 999999 allora non e' PLURIGERME ...
                        If (DistanzaMinima_BietolaBase = 999999) Then

                            'Provo con -1
                            Informazioni_from_DT_Distanze_AUX(
                                                        DT_Distanze_AUX,
                                                        ID_Specie_A,
                                                        -1,
                                                        DistanzaMinima_BietolaBase)

                            Flag_BietolaPlurigerme = False

                            TipoInterferenza = 1    'Possibile INTERFERENZA 
                            Motivazione_Cod = 3     'Bietola - Informazioni Complete - Stesso GRUPPO - Non Plurigerme
                            'Motivazione_Des = "Cat. BASE e PRE-BASE : <br>Le due colture appartengono allo stesso gruppo NON-PLURIGERME"
                            Motivazione_Des = "<b>Cat. BASE e PRE-BASE :</b> <br>Stesso GRUPPO - NON-PLURIGERME"

                        End If

                    Else

                        'Se il gruppo e' diverso allora ...
                        Informazioni_from_DT_Distanze_AUX(
                                                     DT_Distanze_AUX,
                                                     ID_Specie_A,
                                                     -1,
                                                     DistanzaMinima_BietolaBase)

                        TipoInterferenza = 1    'Possibile INTERFERENZA 
                        Motivazione_Cod = 4     'Bietola - Informazioni Complete - GRUPPO Diverso 
                        'Motivazione_Des = "Cat. BASE e PRE-BASE : <br>Le due colture NON appartengono allo stesso gruppo"
                        Motivazione_Des = "<b>Cat. BASE e PRE-BASE :</b> <br>GRUPPI diversi"

                    End If  '(ID_Gruppo_A = ID_Gruppo_B)

                End If


                '////////////////////////////////////////////
                '///  Semente di Categoria CERTIFICATA  /////
                '////////////////////////////////////////////


                If ID_Gruppo_A = ID_Gruppo_B Then

                    DistanzaMinima_BietolaCert = Dist_Varieta_OP
                    'Motivazione_Des += "<br><br>Cat. CERTIFICATA : <br>Varieta' dello stesso GRUPPO" & vbCrLf
                    Motivazione_Des += "<br><br><b>Cat. CERTIFICATA :</b> <br>Stesso GRUPPO" & vbCrLf

                Else

                    If ID_SottoSpecie_A = ID_SottoSpecie_B Then

                        DistanzaMinima_BietolaCert = Dist_Gruppi_OP
                        'Motivazione_Des += "<br><br>Cat. CERTIFICATA : <br>Varieta' di GRUPPI diversi della stessa SOTTOSPECIE" & vbCrLf
                        Motivazione_Des += "<br><br><b>Cat. CERTIFICATA :</b> <br>GRUPPI diversi - stessa SOTTOSPECIE" & vbCrLf

                    Else

                        DistanzaMinima_BietolaCert = Dist_SottoSpecie_OP
                        'Motivazione_Des += "<br><br>Cat. CERTIFICATA : <br>Varieta' di SOTTOSPECIE diverse" & vbCrLf
                        Motivazione_Des += "<br><br><b>Cat. CERTIFICATA :</b> <br>SOTTOSPECIE diverse" & vbCrLf

                    End If

                End If



                If (DistanzaMinima_BietolaCert < DistanzaMinima_BietolaBase) Then
                    DistanzaMinima = DistanzaMinima_BietolaBase
                Else
                    DistanzaMinima = DistanzaMinima_BietolaCert
                End If



            Else

                '###############################################################
                '###############################################################
                '#####  ALTRE SPECIE  ##########################################
                '###############################################################
                '###############################################################

                Flag_Bietola = False


                DistanzaMinima_BietolaBase = -1
                DistanzaMinima_BietolaCert = -1

                '-------------------------------
                '--- Tipo di Avviso
                '-------------------------------

                If (Flag_InformazioniInsufficienti_A = True) Or
                   (Flag_InformazioniInsufficienti_B = True) Then

                    TipoInterferenza = 1    'Possibile INTERFERENZA 
                Else
                    TipoInterferenza = 2    'INTERFERENZA 
                End If

                '-------------------------------
                '--- Distanza
                '-------------------------------

                'Mi procuro le distanze per questa Specie ...
                Informazioni_from_DT_Distanze(
                                             DT_Distanze,
                                             ID_Specie_A,
                                             Distanza_MaxValue,
                                             Dist_Genotipi,
                                             Dist_Varieta_OP,
                                             Dist_Varieta_HY,
                                             Dist_Gruppi_OP,
                                             Dist_Gruppi_HY,
                                             Dist_SottoSpecie_OP,
                                             Dist_SottoSpecie_HY)

                'Se le informazioni sono insufficienti, devo considerare il caso peggiore ...
                'Recupero la massima distanza per la specie in oggetto ...
                If (Flag_InformazioniInsufficienti_A = True) Or
                   (Flag_InformazioniInsufficienti_B = True) Then

                    DistanzaMinima = Distanza_MaxValue
                    Motivazione_Cod = 4     'Non Bietola - Informazioni Incomplete
                    Motivazione_Des = "Poiche' le informazioni fornite sono incomplete, " & "<br>" &
                                      "si e' preso in considerazione il caso peggiore ... " & "<br>" &
                                      "E' necessario analizzare piu' approfonditamente " & "<br>" &
                                      "questa possibile interferenza !!!"

                Else

                    If ID_Genotipo_A <> ID_Genotipo_B Then

                        DistanzaMinima = Dist_Genotipi
                        Motivazione_Cod = 5     'Non Bietola - Informazioni Complete - OP/HY diversi
                        Motivazione_Des = "Varieta' con valori OP/HY diversi"

                    Else

                        If ID_Gruppo_A = ID_Gruppo_B Then

                            If (ID_Genotipo_A = ID_Genotipo_B) And (ID_Genotipo_A = 1) Then

                                DistanzaMinima = Dist_Varieta_HY
                                Motivazione_Cod = 6     'Non Bietola - Informazioni Complete - Varieta' HY - Stesso GRUPPO
                                Motivazione_Des = "Varieta' HY - stesso GRUPPO"

                            End If

                            If (ID_Genotipo_A = ID_Genotipo_B) And (ID_Genotipo_A = 0) Then

                                DistanzaMinima = Dist_Varieta_OP
                                Motivazione_Cod = 7     'Non Bietola - Informazioni Complete - Varieta' OP - Stesso GRUPPO
                                Motivazione_Des = "Varieta' OP - stesso GRUPPO"

                            End If


                        Else

                            If ID_SottoSpecie_A = ID_SottoSpecie_B Then


                                If (ID_Genotipo_A = ID_Genotipo_B) And (ID_Genotipo_A = 1) Then

                                    DistanzaMinima = Dist_Gruppi_HY
                                    Motivazione_Cod = 8     'Non Bietola - Informazioni Complete - Varieta' HY - GRUPPI Diversi - Stessa SOTTOSPECIE
                                    Motivazione_Des = "Varieta' HY - GRUPPO diverso - stessa SOTTOSPECIE"

                                End If

                                If (ID_Genotipo_A = ID_Genotipo_B) And (ID_Genotipo_A = 0) Then

                                    DistanzaMinima = Dist_Gruppi_OP
                                    Motivazione_Cod = 9     'Non Bietola - Informazioni Complete - Varieta' OP - GRUPPI Diversi - Stessa SOTTOSPECIE
                                    Motivazione_Des = "Varieta' OP - GRUPPO diverso - stessa SOTTOSPECIE"

                                End If

                            Else

                                If (ID_Genotipo_A = ID_Genotipo_B) And (ID_Genotipo_A = 1) Then

                                    DistanzaMinima = Dist_SottoSpecie_HY
                                    Motivazione_Cod = 10     'Non Bietola - Informazioni Complete - Varieta' HY - SOTTOSPECIE Diversa
                                    Motivazione_Des = "Varieta' HY - SOTTOSPECIE diverse"

                                End If

                                If (ID_Genotipo_A = ID_Genotipo_B) And (ID_Genotipo_A = 0) Then

                                    DistanzaMinima = Dist_SottoSpecie_OP
                                    Motivazione_Cod = 11     'Non Bietola - Informazioni Complete - Varieta' OP - SOTTOSPECIE Diversa
                                    Motivazione_Des = "Varieta' OP - SOTTOSPECIE diverse"

                                End If


                            End If
                        End If
                    End If

                    '
                    '
                    '

                End If  '(Flag_InformazioniInsufficienti_A = True) Or (Flag_InformazioniInsufficienti_B = True)

                '###############################################################
                '###############################################################
                '###############################################################

            End If  'Specie=Bietola

        End If




    End Sub






    '#############################################################################################
    Private Shared Sub Informazioni_from_DT_Distanze(
                                        ByRef DT_Distanze As DataTable,
                                        ByVal ID_Specie As Integer,
                                        ByRef Distanza_MaxValue As Integer,
                                        ByRef Genotipi As Integer,
                                        ByRef Varieta_OP As Integer,
                                        ByRef Varieta_HY As Integer,
                                        ByRef Gruppi_OP As Integer,
                                        ByRef Gruppi_HY As Integer,
                                        ByRef SottoSpecie_OP As Integer,
                                        ByRef SottoSpecie_HY As Integer)

        Dim Risultato As DataRow()
        Dim StrSelect As String

        StrSelect = " ID_Specie = " & ID_Specie
        Risultato = DT_Distanze.Select(StrSelect)

        If Risultato.GetUpperBound(0) >= 0 Then

            Distanza_MaxValue = Risultato(0)("Distanza_MaxValue")
            Genotipi = Risultato(0)("Diversi_Genotipi")
            Varieta_OP = Risultato(0)("Div_Varieta_in_Gruppo_OP")
            Varieta_HY = Risultato(0)("Div_Varieta_in_Gruppo_HY")
            Gruppi_OP = Risultato(0)("Div_Gruppi_in_SottoSpecie_OP")
            Gruppi_HY = Risultato(0)("Div_Gruppi_in_SottoSpecie_HY")
            SottoSpecie_OP = Risultato(0)("Div_SottoSpecie_in_Specie_OP")
            SottoSpecie_HY = Risultato(0)("Div_SottoSpecie_in_Specie_HY")

        Else

            Distanza_MaxValue = 999999
            Genotipi = 999999
            Varieta_OP = 999999
            Varieta_HY = 999999
            Gruppi_OP = 999999
            Gruppi_HY = 999999
            SottoSpecie_OP = 999999
            SottoSpecie_HY = 999999

        End If

    End Sub





    '#############################################################################################
    Private Shared Sub Informazioni_from_DT_Distanze_AUX(
                                        ByRef DT_Distanze_AUX As DataTable,
                                        ByVal ID_Specie As Integer,
                                        ByVal ID_Gruppo As Integer,
                                        ByRef Distanza As Integer)

        Dim Risultato As DataRow()
        Dim StrSelect As String

        StrSelect = " ID_Specie = " & ID_Specie & " AND ID_Gruppo = " & ID_Gruppo
        Risultato = DT_Distanze_AUX.Select(StrSelect)

        If Risultato.GetUpperBound(0) >= 0 Then
            Distanza = Risultato(0)("Div_Varieta_in_Gruppo")
        Else
            Distanza = 999999
        End If

    End Sub






    '#############################################################################################
    Private Shared Sub Informazioni_from_DT_Distanze_AUX_MaxDistanza(
                                        ByRef DT_Distanze_AUX As DataTable,
                                        ByRef MaxDistanza As Integer)

        Dim i As Integer
        Dim Distanza As Integer

        MaxDistanza = 0

        For i = 0 To DT_Distanze_AUX.Rows.Count - 1

            Distanza = DT_Distanze_AUX.Rows(i).Item("Div_Varieta_in_Gruppo")

            If (Distanza > MaxDistanza) Then
                MaxDistanza = Distanza
            End If

        Next

    End Sub




#End Region

    Public Function MostraInterferenzeGlobaliIterativo(
        ByVal Mittente As String,
        ByVal DataRiferimento As Date,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
    ) As RispostaStandard

        Dim rs As New RispostaStandard

        Try

            rs.RispostaOK = True
            rs.RispostaStringa = ""


            Dim leggiSementi As New AgronicaCoreSementieriDAL.Sportello_R

            Dim dt As DataTable = leggiSementi.Leggi(
                0,
                0,
                DataRiferimento,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                objParametri_Server,
                objParametri_utenti:=objParametri_Utenti,
                ApplicaFiltroUtente:=False
            )

            Dim xConteggio As Integer = 0

            For Each dr In dt.Rows


                'accodo la mail su tutti gli utenti
                If dr("eMail") <> "" Then

                    xConteggio += 1

                    MostraInterferenzeGlobali(dr("UserName"),
                                              dr("utenteCodFiscale"),
                                              dr("id_Specie"),
                                              dr("CodiceFiscaleTecnico"),
                                              Mittente,
                                              dr("eMail"),
                                              objParametri_Server,
                                              objParametri_Utenti)
                End If


            Next

            rs.RispostaStringa = "Accodamento completato su " & xConteggio & " destinazioni."

        Catch ex As Exception

            rs.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            rs.RispostaOK = False


        End Try

        Return rs

    End Function


    Private Function AccodaxInvioEmail(
        ByVal interferenze_Cod As Integer,
        ByVal TestoDescrittivo As String,
        ByVal Mittente As String,
        ByVal DestinatarioEmail_A As String,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
        ) As RispostaStandard


        Dim rval As New RispostaStandard

        Dim xLeggiAccoda As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_Notifiche_R

        Dim dtLeggi As DataTable =
            xLeggiAccoda.leggi(interferenze_Cod, "", "", objParametri_Server)


        If dtLeggi.Rows.Count = 0 Then


            Dim messaggioErrore As String = ""

            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False


            'TODO: Gestire la tranzazione
            Try

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Apro la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                FlagTransazioneLocale,
                                                                                objParametri_Server)


                Dim xAccodaEmail As New AgronicaCoreMailBIZ.Mail_Programmazione_W
                xAccodaEmail.ScriviNuovaMail(
                    objParametri_Server,
                    TipiEnumerativi.enum_MailTipo.NotificheSuInterferenze,
                    interferenze_Cod & "_" & DestinatarioEmail_A,
                    Mittente,
                    DestinatarioEmail_A,
                    "",
                    "",
                    "Notifica di impianti in interferenza",
                    TestoDescrittivo,
                    True,
                    "",
                    Now
                )


                Dim xSementieri_Sportello_InterferenzePerConferma_NotificheScrivi As New Sementieri_Sportello_InterferenzePerConferma_Notifiche_W
                xSementieri_Sportello_InterferenzePerConferma_NotificheScrivi.Scrivi(interferenze_Cod, objParametri_Server)


                ' VAnni: 17/5/2018: non occorre impostare il flag statoGestione, poichè se viene fatto allora la lettura per utenti ancora da accodare non funziona
                'Dim xSementieri_Sportello_InterferenzePerConfermaScrivi As New Sementieri_Sportello_InterferenzePerConferma_W
                'xSementieri_Sportello_InterferenzePerConfermaScrivi.ModificaStatoGestione(interferenze_Cod, 1, objParametri_Server)

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Chiudo la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                rval.RispostaOK = True

            Catch ex As Exception

                'Faccio il rollback della transazione
                If Not objParametri_Server.objTransazione Is Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

                End If

                messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


                Dim Messaggio As String = ""
                If messaggioErrore <> "" Then


                    Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                    Messaggio += "" & vbCrLf
                    Messaggio += messaggioErrore
                    Messaggio += "" & vbCrLf
                    Messaggio += "Ritentare il salvataggio dopo la correzione ..."

                End If


                rval.RispostaOK = False
                rval.Errore &= messaggioErrore


            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try

        Else
            rval.RispostaOK = True

        End If

        Return rval

    End Function

    ''' <summary>
    ''' Funzione di calcolo e visualizzazione delle interferenze
    ''' </summary>
    ''' <param name="utente"></param>
    ''' <param name="utenteCodFiscale"></param>
    ''' <param name="id_specie"></param>
    ''' <param name="CodiceFiscaleTecnico"></param>
    ''' <param name="AccodaMail"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Function MostraInterferenzeGlobali(ByVal utente As String,
                                              ByVal utenteCodFiscale As String,
                                              ByVal id_specie As Integer,
                                              ByVal CodiceFiscaleTecnico As String,
                                              ByVal Mittente As String,
                                              ByVal MailDestinatario As String,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal objParametri_Utenti As AgronicaCoreParametri,
                                              Optional ByVal DataInizioSportello As DateTime? = Nothing,
                                              Optional ByVal DataFineSportello As DateTime? = Nothing) As String

        If utenteCodFiscale <> "" Then
            objParametri_Server.UtenteUsername = utente
            objParametri_Server.UtenteCodFiscale = utenteCodFiscale
            objParametri_Utenti.UtenteUsername = utente
            objParametri_Utenti.UtenteCodFiscale = utenteCodFiscale
        End If

        Dim AccodaMail As Boolean = MailDestinatario <> ""
        Dim isSuperUser As Boolean = (objParametri_Server.PivaSuperUser = objParametri_Server.UtenteCodFiscale)

        'TODO
        'Se sono nel caso SuperUser come Codice_Fiscale_Tecnico, viene inviato "" ... compaiono tutte le interferenze ... OK
        'Nel caso normale invece viene inviato il corretto valore di Codice_Fiscale_Tecnico ... ma non compare nessuna interferenza !!!!

        'Bisogna verificare cosa fa esattamente la query ...

        'Se AccodaMail prendo solo le interferenze che coinvolgono il CodiceFiscaleTecnico come proprietario, altrimenti anche come interferente
        Dim SoloProprietario As Boolean = AccodaMail

        SoloProprietario = True

        Dim objR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_R

        Dim dt As DataTable = objR.Leggi(0, Not isSuperUser, CodiceFiscaleTecnico, SoloProprietario, id_specie, AccodaMail, objParametri_Server, objParametri_Utenti)

        Dim objUten As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dt_utenti As DataTable = objUten.Leggi("", 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Utenti)

        Dim StrBldr As New StringBuilder


        StrBldr.Append("<div id='tabs'><ul><li class='k-state-active k-active'>In attesa</li><li>Confermati</li><li>Rifiutati</li></ul>")

        Dim Stati As Integer() = {1, 2, 3} 'IN ATTESA / CONFERMATI / NON CONFERMATI

        Dim StrBldrElemento As New StringBuilder
        Dim riga As DataRow
        Dim riga1 As DataRow
        Dim dr_indirizzo As DataRow = Nothing
        Dim distanza As String
        Dim data As String
        Dim key_conflitto As String = ""
        Dim separatore As Boolean
        Dim rigaDaCache As Boolean
        Dim strEntitaInterferenti As String = ""

        For Each stato In Stati

            separatore = False

            StrBldr.Append("<div id='tabs-" & stato & "'>")

            'In "dt" sono presenti le interferenze che coinvolgono impianti dell'utente corrente
            'Estraggo solo gli elementi di "dt" che hanno un valore preciso di "StatoCod" ( 1=InAttesa  2=Confermati  3=NonConfermati ) 
            'Creo un elenco DISTINCT degli "Entita_cod_Interferente" che interferiscono con uno qualunque degli impianti di "dt"

            Dim distinct_interferenti = (From elem In dt.AsEnumerable()
                                         Where elem.Field(Of Integer)("Stato_Cod") = stato
                                         Select elem.Field(Of Integer)("Entita_Cod_Interferente")
                                         Distinct).ToList()

            For Each entita_cod In distinct_interferenti
                '--------------------
                'INTERFERENTE
                '--------------------
                'Recupero tutti gli impianti dell'utente che vanno in conflitto con l'interferente i-esimo
                Dim lista_interferenti = (From elem In dt.AsEnumerable()
                                          Where elem.Field(Of Integer)("Stato_Cod") = stato _
                                          AndAlso elem.Field(Of Integer)("Entita_Cod_Interferente") = entita_cod
                                          Select elem).ToList()

                Dim tmpList As New List(Of String)
                For Each dr In lista_interferenti
                    tmpList.Add(dr("Entita_Cod_Propietario"))
                Next
                strEntitaInterferenti = entita_cod & "|" & String.Join("|", tmpList)

                riga = Riga_O_Cache(objR, lista_interferenti(0), rigaDaCache)

                dr_indirizzo = LeggiIndirizzo(True, riga, objParametri_Server)

                StrBldrElemento.Clear()
                StrBldrElemento.Append("<div style='margin-bottom: 10px;" & If(rigaDaCache, " text-decoration: line-through;", "") & "'>")

                StrBldrElemento.Append("<div style='display:flex; align-items:stretch; justify-content:space-between;'>")

                'Preparo le informazioni del entita' (impianto o planning) interferente
                StrBldrElemento.Append("<div><span style='font-style: italic;'>Richiedente: </span>")
                StrBldrElemento.Append("<span style='font-size: 1.5em; font-weight: bold;'>")
                StrBldrElemento.Append(infoUtente(True, riga, dt_utenti))
                StrBldrElemento.Append("</span></div>")

                If True OrElse Not rigaDaCache Then

                    'Pulsante Vai a GIS con questa coordinate
                    StrBldrElemento.Append("<div style='border:1px solid #ccc; border-radius:4px; margin-left:7px; background-color:#eee; cursor:pointer;' onclick='vai_a_gis_da_interferenze(""" & strEntitaInterferenti & """)'>")
                    StrBldrElemento.Append("<div style='font-weight:bolder; font-size:large; padding-left:10px; padding-right:10px;'>")
                    StrBldrElemento.Append("<span>&#x21AA;</span>")
                    StrBldrElemento.Append("</div>")
                    StrBldrElemento.Append("</div>")

                End If

                StrBldrElemento.Append("</div>")

                StrBldrElemento.Append("<div style='margin-top: 5px;'><span style='font-style: italic;'>Campo in via: </span>")
                StrBldrElemento.Append(infoIndirizzo(True, riga, dr_indirizzo))
                StrBldrElemento.Append("</div>")

                StrBldrElemento.Append("<div style='margin-top:5px; display:flex; align-items:center'>")
                StrBldrElemento.Append(infoCoord(True, riga, dr_indirizzo))
                StrBldrElemento.Append("</div>")

                StrBldrElemento.Append(infoSpecie(True, riga, "margin-top:5px;"))
                StrBldrElemento.Append(infoValidita(True, riga, "margin-top:5px;"))
                StrBldrElemento.Append("<div style='font-style: italic; margin-top: 5px;'>Conflitti:</div>")

                '--------------------
                'PROPRIETARIO
                '--------------------
                StrBldrElemento.Append("<div style='padding-left: 3em;'><ol>")

                key_conflitto = ""

                For Each dr In lista_interferenti

                    riga1 = Riga_O_Cache(objR, dr, rigaDaCache)

                    dr_indirizzo = LeggiIndirizzo(False, riga1, objParametri_Server)

                    StrBldrElemento.Append("<li style='margin-bottom: 5px;'>")

                    If isSuperUser Then
                        StrBldrElemento.Append("<div style='font-weight: bold;'>" & infoUtente(False, riga1, dt_utenti) & "</div>")
                    End If
                    StrBldrElemento.Append("<div>" & infoIndirizzo(False, riga1, dr_indirizzo) & "</div>")
                    StrBldrElemento.Append(infoCoord(False, riga1, dr_indirizzo))
                    StrBldrElemento.Append(infoSpecie(False, riga1, ""))
                    StrBldrElemento.Append(infoValidita(False, riga1, ""))

                    StrBldrElemento.Append("<div>")
                    'esiste data creazione
                    If riga1.Table.Columns.Contains("DataCreazione") Then
                        If Not IsDBNull(riga1("DataCreazione")) Then
                            data = CDate(riga1("DataCreazione")).ToString("d")
                            StrBldrElemento.Append("<span style='font-style: italic;'>Data: </span><span>" & data & " - </span>")
                        End If
                    End If

                    distanza = "Non disponibile"
                    If Not IsDBNull(riga1("DistanzaEffettiva")) AndAlso (IsNumeric(riga1("DistanzaEffettiva"))) Then
                        distanza = Agro_Math.ArrotondaVal_0(riga1("DistanzaEffettiva"))
                    End If
                    StrBldrElemento.Append("<span style='font-style: italic;'>Distanza: </span><span style='font-weight: bold;'>" & distanza & "</span><span> mt.</span>")
                    StrBldrElemento.Append("</div>")

                    StrBldrElemento.Append("</li>")

                    If Not String.IsNullOrEmpty(key_conflitto) Then
                        key_conflitto &= "@"
                    End If
                    key_conflitto &= riga1("Interferenze_cod")

                Next

                StrBldrElemento.Append("</ol></div>")

                'Se e' collegato l'utente (non il SuperUser) e l'interferenza e' nello "StatoCod"=1  ( 1=InAttesa  2=Confermati  3=NonConfermati )
                'allora visualizzo l'option button per accettare/rifiutare l'interferenza :
                If isSuperUser = False And stato = 1 And Not AccodaMail Then

                    'GABRIELE 20 03 2019

                    'StrBldrElemento.Append("<div style='text-align: right; margin-bottom: 5px;'>")
                    'StrBldrElemento.Append("<input name='option_" & key_conflitto & "' type='radio' id='accetta_" & key_conflitto & "' value='si' class='k-radio opzioni_selezione' name='accetta_" & key_conflitto & "'>")
                    'StrBldrElemento.Append("<label class='k-radio-label' for='accetta_" & key_conflitto & "'>Accetta</label>")
                    'StrBldrElemento.Append("<span style='margin-left:10px; margin-right:10px;'></span>")
                    'StrBldrElemento.Append("<input name='option_" & key_conflitto & "' type='radio' id='rifiuta_" & key_conflitto & "' value='no' class='k-radio opzioni_selezione' name='rifiuta_" & key_conflitto & "'>")
                    'StrBldrElemento.Append("<label class='k-radio-label' for='rifiuta_" & key_conflitto & "'>Rifiuta</label>")
                    'StrBldrElemento.Append("</div>")

                    StrBldrElemento.Append("<div style='display:flex; justify-content:flex-end;'>")
                    StrBldrElemento.Append("<div style='display:flex; align-self:center;'>")
                    StrBldrElemento.Append("<input class='k-radio' name='opzione_" & key_conflitto & "' type='radio' id='accetta_" & key_conflitto & "' value='si'>")
                    StrBldrElemento.Append("<label class='k-radio-label' for='accetta_" & key_conflitto & "' style='margin:0px 7px 0px 0px;'>Accetta</label>")
                    StrBldrElemento.Append("<input class='k-radio' name='opzione_" & key_conflitto & "' type='radio' id='rifiuta_" & key_conflitto & "' value='no'>")
                    StrBldrElemento.Append("<label class='k-radio-label' for='rifiuta_" & key_conflitto & "' style='margin:0px 0px 0px 7px;'>Rifiuta</label>")
                    StrBldrElemento.Append("</div>")
                    StrBldrElemento.Append("<div class='k-button' style='margin-left:25px; padding: 3px 20px;' onclick='modificaStatoInterferenza(""" & key_conflitto & """)'>Salva</div>")
                    StrBldrElemento.Append("</div>")

                End If

                StrBldrElemento.Append("</div>")

                If separatore Then
                    StrBldr.Append("<div style='margin-top: 15px; margin-bottom: 15px; border-top: 1px solid #ccc;'></div>")
                End If
                separatore = True

                StrBldr.Append(StrBldrElemento)

                ' VAnni: 11/5/2018: memorizza i dati per l'invio dell'info di interferenza via mail all'utente interessato            
                If AccodaMail Then
                    Dim rvalAccoda As RispostaStandard = AccodaxInvioEmail(entita_cod, StrBldrElemento.ToString(), Mittente, MailDestinatario, objParametri_Server, objParametri_Utenti)
                End If

            Next

            'Chiusura del div tab
            StrBldr.Append("</div>")

            'solo quelli IN ATTESA possono avere questo flag True
            AccodaMail = False
        Next

        StrBldr.Append("</div>")

        Return StrBldr.ToString()
    End Function


    Private Function Riga_O_Cache(ByVal ObjR As AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_R, ByVal riga As DataRow, ByRef flagDaCache As Boolean) As DataRow

        If Not IsDBNull(riga("Flag_Attivo")) AndAlso riga("Flag_Attivo") = False Then
            flagDaCache = True
            'Uno degli impianti coinvolti nell'interferenza e' stato cancellato
            'Recupero le informazioni dalla colonna "descrizione_Casella_Conflitto" che funge da cache
            Return ObjR.LeggiDaCache(riga("Descrizione_Casella_Conflitto"))
        End If
        flagDaCache = False
        'Gli impianti esistono ancora... Le informazioni sono direttamente disponibili
        Return riga
    End Function




    Private Class Interferenza

        Private Class _impianto

            Public ReadOnly Entita_Cod As Integer
            Protected ReadOnly Veg_Des As String
            Protected ReadOnly Grva_Des As String
            Protected ReadOnly Lat As Decimal
            Protected ReadOnly Lng As Decimal
            Protected ReadOnly Indirizzo As String
            Protected ReadOnly Layer As String
            Protected ReadOnly Ind_des As String
            Protected ReadOnly Frz_des As String
            Protected ReadOnly Com_des As String
            Protected ReadOnly Pr_des As String

            Public Sub New(r As DataRow)
                Me.New(r, False)
            End Sub

            Protected Sub New(r As DataRow, inList As Boolean)
                Dim prefix As String = "interferente"
                If inList Then
                    prefix = "proprietario"
                End If

                Entita_Cod = r(prefix & ".Entita_Cod")
                Veg_Des = r(prefix & ".veg_des")
                Grva_Des = r(prefix & ".grva_des")
                Lat = r(prefix & ".coord.lat")
                Lng = r(prefix & ".coord.lng")
                Indirizzo = r(prefix & ".indirizzo")
                Layer = r(prefix & ".layer")
                Ind_des = r(prefix & ".ind_des")
                Frz_des = r(prefix & ".frz_des")
                Com_des = r(prefix & ".com_des")
                Pr_des = r(prefix & ".pr_des")
            End Sub

            Public Function Compare(other As _impianto) As Integer
                Return String.Compare(Layer, other.Layer, True)
            End Function

            Public Function Readable() As Boolean
                If String.IsNullOrEmpty(Layer) Then

                    Return False
                End If

                If (String.IsNullOrEmpty(Veg_Des) AndAlso String.IsNullOrEmpty(Grva_Des)) OrElse
                    (String.IsNullOrEmpty(Indirizzo) AndAlso String.IsNullOrEmpty(Ind_des)) Then

                    Return False
                End If
                Return True
            End Function

            'Protected Function htmlInfo(label As String, info As String, Optional info_css As String = "") As String

            '    Dim spanLabel As String = ""

            '    If Not String.IsNullOrEmpty(label) Then

            '        spanLabel = "<span class='etichetta'>" & label & "</span>"
            '    End If

            '    Dim spanInfo As String = "<span class='info'"

            '    If Not String.IsNullOrEmpty(info_css) Then

            '        spanInfo &= " style='" & info_css & "'"
            '    End If

            '    spanInfo &= ">" & info & "</span>"

            '    Return spanLabel & spanInfo
            'End Function

            'Protected Function htmlRow(row As String, Optional css As String = "") As String

            '    Dim divRow As String = "<div class='interferenze-row'"

            '    If Not String.IsNullOrEmpty(css) Then

            '        divRow &= " style='" & css & "'"
            '    End If

            '    divRow &= ">" & row & "</div>"

            '    Return divRow
            'End Function

            'Protected Function htmlIndirizzo() As String

            '    If Not String.IsNullOrEmpty(Indirizzo) Then

            '        Return "<span class='info'>" & Indirizzo & "</span>"
            '    End If

            '    Dim html As String = ""

            '    If Not String.IsNullOrEmpty(Ind_des) Then

            '        html = "<span class='info'>" & Ind_des & "</span>"
            '    End If

            '    Dim span As String = Pr_des

            '    If Not String.IsNullOrEmpty(Com_des) Then

            '        If Not String.IsNullOrEmpty(span) Then

            '            span = " - " & span
            '        End If

            '        span = Com_des & span
            '    End If

            '    If Not String.IsNullOrEmpty(span) Then

            '        span = "(" & span & ")"
            '    End If

            '    If Not String.IsNullOrEmpty(Frz_des) Then

            '        If Not String.IsNullOrEmpty(span) Then

            '            span = " " & span
            '        End If

            '        span = Frz_des & span
            '    End If

            '    If Not String.IsNullOrEmpty(span) Then

            '        Dim style As String = ""

            '        If Not String.IsNullOrEmpty(html) Then

            '            style = " style='margin-left: 0.5em;'"
            '        End If

            '        html &= "<span" & style & ">" & span & "</span>"
            '    End If

            '    Return html
            'End Function

            'Protected Function htmlCoord() As String
            '    If Lat > 0 AndAlso Lng > 0 Then

            '        Dim strLat = Agro_Math.ArrotondaVal_6(Lat).ToString("N6")
            '        Dim strLng = Agro_Math.ArrotondaVal_6(Lng).ToString("N6")

            '        Return htmlInfo("Lat:", strLat) & htmlInfo("Lng:", strLng)
            '    End If
            '    Return ""
            'End Function

            'Protected Function htmlSpecie() As String

            '    Dim html As String = ""

            '    If Not String.IsNullOrEmpty(Veg_Des) Then

            '        html &= htmlInfo("Specie:", Veg_Des)
            '    End If

            '    If Not String.IsNullOrEmpty(Grva_Des) Then

            '        html &= htmlInfo("Tipologia:", Grva_Des)
            '    End If

            '    Return html
            'End Function

            'Public Overridable Function html(needLayer As Boolean, gis_onclick As String) As String

            '    Dim _htmlIndirizzo As String = "<span class='etichetta'>Campo in via:</span>" & htmlIndirizzo()

            '    Dim strHtml As New StringBuilder

            '    strHtml.AppendLine("<div class='interferenze-row' style='display:flex; align-items:center; justify-content:space-between;'>")

            '    strHtml.AppendLine("<div>")

            '    If needLayer Then

            '        strHtml.Append(htmlInfo("Richiedente:", Layer, "font-size: 1.5em;"))
            '    Else

            '        strHtml.Append(_htmlIndirizzo)
            '    End If

            '    strHtml.AppendLine("</div>")

            '    'Pulsante Vai a GIS con questa coordinate
            '    strHtml.Append("<div class='k-button' " & gis_onclick & ">")
            '    strHtml.Append("<span class='fa fa-globe fa-lg'></span>")
            '    strHtml.AppendLine("</div>")

            '    strHtml.AppendLine("</div>")

            '    If needLayer Then

            '        strHtml.AppendLine(htmlRow(_htmlIndirizzo))
            '    End If

            '    strHtml.AppendLine(htmlRow(htmlCoord()))
            '    strHtml.AppendLine(htmlRow(htmlSpecie()))

            '    Return strHtml.ToString
            'End Function

            Public Overridable Function ToObject() As JObject

                Dim jobj As New JObject(New JProperty("entita_cod", Entita_Cod),
                                        New JProperty("layer", Layer),
                                        New JProperty("indirizzo", Indirizzo),
                                        New JProperty("indirizzo_centro", New JObject(New JProperty("via", Ind_des), New JProperty("fraz", Frz_des), New JProperty("com", Com_des), New JProperty("pr", Pr_des))),
                                        New JProperty("coord", New JObject(New JProperty("lat", Lat), New JProperty("lng", Lng))),
                                        New JProperty("specie", Veg_Des),
                                        New JProperty("tipologia", Grva_Des))

                Return jobj
            End Function
        End Class

        Private Class _interferenza
            Inherits _impianto

            Private ReadOnly InterferenzaCod As Integer
            Private ReadOnly Stato As Integer '1 -> In attesa, 2 -> Accettata , 3 -> Rifiutata
            Private ReadOnly StatoDescr As String
            Private ReadOnly DataCreazione As Date
            Private ReadOnly DataConferma As Date
            Private ReadOnly DistanzaEffettiva As Decimal
            Private ReadOnly IsActive As Boolean

            Public Sub New(r As DataRow, isActive As Boolean)
                MyBase.New(r, True)

                InterferenzaCod = r("Interferenze_cod")
                Stato = r("Stato_cod")
                StatoDescr = r("Stato")
                If IsDBNull(r("Data_Creazione")) Then

                    DataCreazione = Date.MinValue
                Else

                    DataCreazione = CDate(r("Data_Creazione"))
                End If
                If IsDBNull(r("Data_Conferma")) Then

                    DataConferma = Date.MinValue
                Else

                    DataConferma = CDate(r("Data_Conferma"))
                End If
                If IsDBNull(r("DistanzaEffettiva")) Then

                    DistanzaEffettiva = -1
                Else

                    DistanzaEffettiva = CDec(r("DistanzaEffettiva"))
                End If

                Me.IsActive = isActive
            End Sub

            Public Shadows Function Compare(other As _interferenza) As String

                Dim result As Integer = String.Compare(Layer, other.Layer, True)

                If result = 0 Then

                    If DataCreazione > Date.MinValue AndAlso other.DataCreazione > Date.MinValue Then

                        If DataCreazione < other.DataCreazione Then

                            result = -1
                        Else

                            If DataCreazione > other.DataCreazione Then

                                result = 1
                            End If
                        End If
                    End If
                End If

                If result = 0 Then

                    If DistanzaEffettiva < other.DistanzaEffettiva Then

                        result = -1
                    Else

                        If DistanzaEffettiva > other.DistanzaEffettiva Then

                            result = 1
                        End If
                    End If
                End If

                Return result
            End Function

            'Public Overrides Function html(needLayer As Boolean, gis_onclick As String) As String

            '    Dim strHtml As New StringBuilder

            '    strHtml.AppendLine("<div class='interferente-content status-" & Stato.ToString & "'>")

            '    If needLayer Then

            '        strHtml.AppendLine(htmlRow(htmlInfo("Notificato a:", Layer, "font-size: 1.3em;")))
            '    End If

            '    strHtml.AppendLine(htmlRow(htmlIndirizzo()))
            '    strHtml.AppendLine(htmlRow(htmlCoord()))
            '    strHtml.AppendLine(htmlRow(htmlSpecie()))

            '    Dim htmlTmp As String = ""
            '    If DataCreazione > Date.MinValue Then

            '        htmlTmp = htmlInfo("Data:", DataCreazione.ToString("d"))
            '    End If

            '    Dim distanza As String = "Non disponibile"
            '    If DistanzaEffettiva > 0 Then
            '        distanza = Agro_Math.ArrotondaVal_0(DistanzaEffettiva)
            '    End If

            '    htmlTmp &= htmlInfo("Distanza:", distanza & " mt.")

            '    strHtml.AppendLine(htmlRow(htmlTmp))

            '    htmlTmp = htmlInfo("Stato:", StatoDescr)
            '    Dim cssTmp As String = ""

            '    If Stato = 1 AndAlso Not needLayer Then

            '        cssTmp = "display:flex; align-items:center;"

            '        htmlTmp = "<div>" & htmlTmp & "</div>"

            '        Dim divAccettazione As String = "<div class='btns-attesa'>"
            '        divAccettazione &= "<div><div class='k-button' onclick='attivaAzione(this)'><span class='fa fa-fw fa-unlock'></span></div></div>"
            '        divAccettazione &= "<div class='btns-accetta-rifiuta'>"
            '        divAccettazione &= "<div class='k-button k-state-disabled' onclick='azioneInterferenza(true, " & InterferenzaCod.ToString & ")'><span class='accetta fa fa-fw fa-thumbs-up'></span><span>Accetta</span></div>"
            '        divAccettazione &= "<div class='k-button k-state-disabled' onclick='azioneInterferenza(false, " & InterferenzaCod.ToString & ")'><span class='rifiuta fa fa-fw fa-thumbs-down'></span><span>Rifiuta</span></div>"
            '        divAccettazione &= "</div>"
            '        divAccettazione &= "</div>"

            '        htmlTmp &= divAccettazione
            '    Else

            '        If Stato <> 1 AndAlso DataConferma > Date.MinValue Then

            '            htmlTmp &= htmlInfo("in data", DataConferma.ToString("d"))
            '        End If
            '    End If

            '    strHtml.AppendLine(htmlRow(htmlTmp, cssTmp))

            '    Return strHtml.ToString
            'End Function

            Public Overrides Function ToObject() As JObject
                Dim jobj As JObject = MyBase.ToObject

                jobj.Add(New JProperty("attiva", IsActive))
                jobj.Add(New JProperty("interferenza_cod", InterferenzaCod))
                jobj.Add(New JProperty("stato", New JObject(New JProperty("cod", Stato), New JProperty("descr", StatoDescr))))
                jobj.Add(New JProperty("distanza", DistanzaEffettiva))

                If DataCreazione > Date.MinValue Then

                    jobj.Add(New JProperty("data_creazione", DataCreazione.ToString("u")))
                End If

                If DataConferma > Date.MinValue Then

                    jobj.Add(New JProperty("data_conferma", DataConferma.ToString("u")))
                End If

                Return jobj
            End Function
        End Class

        Private ReadOnly Impianto As _impianto
        Private ReadOnly ListaInterferenti As List(Of _interferenza)

        Public Sub New(r As DataRow, isActive As Boolean)
            Impianto = New _impianto(r)
            ListaInterferenti = New List(Of _interferenza) From {New _interferenza(r, isActive)}
        End Sub

        Public Sub AddInterferenza(r As DataRow, isActive As Boolean)
            ListaInterferenti.Add(New _interferenza(r, isActive))
        End Sub

        Public Function Compare(other As Interferenza) As Integer
            Return Impianto.Compare(other.Impianto)
        End Function

        Public Function Readable() As Boolean
            If Not Impianto.Readable() Then

                Return False
            End If

            Dim idx As Integer = 0
            While idx < ListaInterferenti.Count

                If Not ListaInterferenti(idx).Readable() Then

                    ListaInterferenti.RemoveAt(idx)
                Else

                    idx += 1
                End If
            End While

            If ListaInterferenti.Count = 0 Then

                Return False
            End If
            Return True
        End Function

        'Public Function buildHtml(proprietario As Boolean, isSuperuser As Boolean) As String

        '    ListaInterferenti.Sort(Function(el0 As _interferenza, el1 As _interferenza)
        '                               Return el0.Compare(el1)
        '                           End Function)

        '    Dim strHtml As New StringBuilder

        '    Dim gis_onclick As String = "onclick='vai_a_gis_da_interferenze("""

        '    gis_onclick &= Impianto.Entita_Cod.ToString

        '    For Each interf In ListaInterferenti

        '        gis_onclick &= "|" & interf.Entita_Cod.ToString
        '    Next

        '    gis_onclick &= """)'"

        '    strHtml.Append(Impianto.html(proprietario Or isSuperuser, gis_onclick))

        '    strHtml.AppendLine("<div class='interferenti-lista'>")

        '    strHtml.AppendLine("<ol style='margin-bottom: 0px;'>")

        '    For Each interf In ListaInterferenti

        '        strHtml.AppendLine("<li class='interferente'>")

        '        strHtml.Append(interf.html(Not proprietario Or isSuperuser, ""))

        '        strHtml.AppendLine("</li>")
        '    Next

        '    strHtml.AppendLine("</ol>")

        '    strHtml.AppendLine("</div>")

        '    If Not IsActive Then

        '        strHtml.Insert(0, "<div class='interferenze-non-attive'>" & vbNewLine & "<div class='interferenze-non-attive-overlay'></div>" & vbNewLine)
        '        strHtml.AppendLine("</div>")
        '    End If

        '    Return strHtml.ToString
        'End Function

        Public Function ToObject() As JObject

            ListaInterferenti.Sort(Function(el0 As _interferenza, el1 As _interferenza)
                                       Return el0.Compare(el1)
                                   End Function)

            Dim jobj As New JObject(New JProperty("richiedente", Impianto.ToObject()))

            Dim jarr As New JArray
            For Each interf In ListaInterferenti

                jarr.Add(interf.ToObject())
            Next

            jobj.Add(New JProperty("interferenti", jarr))

            Return jobj
        End Function
    End Class


    Public Function MostraInterferenzeGlobali_New(id_specie As Integer, codice_fiscale_tecnico As String, AccodaMail As Boolean, DataInizioSportello As Date, DataFineSportello As Date, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As String

        Dim objR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_R

        Dim dt As DataTable = objR.LeggiAttiviPerCodiceFiscaleTecnico(id_specie, codice_fiscale_tecnico, DataInizioSportello, DataFineSportello, AccodaMail, objParametri_Server, objParametri_Utenti)

        Dim dt_na As DataTable = Nothing

        Dim isSuperuser As Boolean = String.IsNullOrEmpty(codice_fiscale_tecnico)

        If isSuperuser Then

            dt_na = objR.LeggiNonAttiviAttiviPerSportello(id_specie, DataInizioSportello, DataFineSportello, objParametri_Server, objParametri_Utenti)
        End If

        If dt Is Nothing AndAlso dt_na Is Nothing Then

            Return ""
        End If

        Dim listaProprietario As New Dictionary(Of Integer, Interferenza)
        Dim listaInterferente As New Dictionary(Of Integer, Interferenza)

        For Each r In dt.Rows

            Dim l As Dictionary(Of Integer, Interferenza)

            If CInt(r("Proprietario")) = 1 Then

                l = listaProprietario
            Else

                l = listaInterferente
            End If

            Dim entita_cod As Integer = r("interferente.entita_cod")

            If l.ContainsKey(entita_cod) Then

                l(entita_cod).AddInterferenza(r, True)
            Else

                l.Add(entita_cod, New Interferenza(r, True))
            End If
        Next

        Dim jsonNotifiche As New JObject

        If Not isSuperuser Then

            Dim arrProprietario As New JArray
            Dim arrInterferente As New JArray

            For Each interf In listaProprietario
                arrProprietario.Add(interf.Value.ToObject())
            Next

            For Each interf In listaInterferente
                arrInterferente.Add(interf.Value.ToObject)
            Next

            jsonNotifiche.Add(New JProperty("proprietario", arrProprietario))
            jsonNotifiche.Add(New JProperty("interferente", arrInterferente))
        Else

            If dt_na IsNot Nothing AndAlso dt_na.Rows.Count > 0 Then

                For Each row In dt_na.Rows

                    If LeggiDaCache(row) Then

                        If IsNumeric(row("interferente.entita_cod")) Then

                            Dim entita_cod As Integer = row("interferente.entita_cod")

                            If listaInterferente.ContainsKey(entita_cod) Then

                                listaInterferente(entita_cod).AddInterferenza(row, False)
                            Else

                                listaInterferente.Add(entita_cod, New Interferenza(row, False))
                            End If

                        End If

                    End If
                Next

            End If

            'Per una migliore visione dell'output ordino la lista per Richiedente
            Dim sortedList As New List(Of Interferenza)

            For Each interf In listaInterferente

                sortedList.Add(interf.Value)
            Next

            sortedList.Sort(Function(el0 As Interferenza, el1 As Interferenza)
                                Return el0.Compare(el1)
                            End Function)

            Dim arrGlobali As New JArray

            For Each interf In sortedList
                arrGlobali.Add(interf.ToObject())
            Next

            jsonNotifiche.Add(New JProperty("globali", arrGlobali))
        End If

        Return jsonNotifiche.ToString

#If False Then

        Dim strHtml As New StringBuilder

        If Not isSuperuser Then

            strHtml.Append("<div id='tabs'><ul><li class='k-state-active k-active'>Proprietario</li><li>Interferente</li></ul>")

            strHtml.AppendLine("<div id='tabs-proprietario' class='interferenze-group-container'>")

            For Each interf In listaProprietario
                strHtml.AppendLine("<div class='interferenze-group'>")
                strHtml.Append(interf.Value.buildHtml(True, False))
                strHtml.AppendLine("</div>")
            Next

            strHtml.AppendLine("</div>")

            strHtml.AppendLine("<div id='tabs-interferente' class='interferenze-group-container'>")

            For Each interf In listaInterferente
                strHtml.AppendLine("<div class='interferenze-group'>")
                strHtml.Append(interf.Value.buildHtml(False, False))
                strHtml.AppendLine("</div>")
            Next

            strHtml.AppendLine("</div>")

            strHtml.AppendLine("</div>")

        Else

            'Per una migliore visione dell'output ordino la lista per Richiedente
            Dim sortedList As New List(Of Interferenza)

            For Each interf In listaInterferente

                sortedList.Add(interf.Value)
            Next

            If dt_na IsNot Nothing AndAlso dt_na.Rows.Count > 0 Then

                Dim lista_na As New Dictionary(Of Integer, Interferenza)

                For Each row In dt_na.Rows

                    If LeggiDaCache(row) Then

                        If IsNumeric(row("interferente.entita_cod")) Then

                            Dim entita_cod As Integer = row("interferente.entita_cod")

                            If lista_na.ContainsKey(entita_cod) Then

                                lista_na(entita_cod).AddInterferenza(row)
                            Else

                                lista_na.Add(entita_cod, New Interferenza(row, False))
                            End If

                        End If

                    End If
                Next

                For Each interf In lista_na

                    If interf.Value.Readable() Then

                        sortedList.Add(interf.Value)
                    End If
                Next

            End If

            sortedList.Sort(Function(el0 As Interferenza, el1 As Interferenza)
                                Return el0.Compare(el1)
                            End Function)

            strHtml.Append("<div id='tabs'><ul><li class='k-state-active k-active'>Interferenze</li></ul>")

            strHtml.AppendLine("<div id='tabs-interferenze' class='interferenze-group-container'>")

            For Each interf In sortedList
                strHtml.AppendLine("<div class='interferenze-group'>")
                strHtml.Append(interf.buildHtml(False, True))
                strHtml.AppendLine("</div>")
            Next

            strHtml.AppendLine("</div>")

            strHtml.AppendLine("</div>")
        End If

        Return strHtml.ToString

#End If
    End Function


    Private Function LeggiDaCache(row As DataRow) As Boolean

        Dim data_ser As String = row("Descrizione_Casella_Conflitto")

        If String.IsNullOrEmpty(data_ser) Then

            Return False
        End If

        Dim row_arr As String() = data_ser.Split("§")

        If row_arr.Length < 2 Then

            Return False
        End If

        Dim cols_arr As String() = row_arr(0).Split("|")
        'Per un baco il record potrebbe contenere due volte l'intestazione...
        Dim data_arr As String() = row_arr.Last().Split("|")

        If cols_arr.Length <> data_arr.Length Then

            Return False
        End If

        Dim jobj As New JObject

        Dim c As Integer = 0

        While c < cols_arr.Length

            jobj.Add(New JProperty(cols_arr(c), data_arr(c)))

            c += 1
        End While

        Dim prefix As String() = {"proprietario", "interferente"}

        Dim map As String(,) = {
            {"entita_cod", "Entita_Cod_Propietario", "Entita_Cod_Interferente"},
            {"veg_des", "P_Veg_Des", "I_Veg_Des"},
            {"grva_des", "P_Grva_Des", "I_Grva_Des"},
            {"coord.lat", "P_Lat", "I_Lat"},
            {"coord.lng", "P_Lng", "I_Lng"},
            {"indirizzo", "P_Via_Stringa", "I_Via_Stringa"},
            {"layer", "PropietarioLayer", "InterferenteLayer"}
        }

        Dim idx As Integer = 0
        While idx < prefix.Length

            Dim outer As Integer = 0
            While outer < map.GetLength(0)

                Dim row_col As String = prefix(idx) & "." & map(outer, 0)
                Dim obj_pro As String = map(outer, idx + 1)

                If IsDBNull(row(row_col)) Then

                    If jobj.ContainsKey(obj_pro) Then

                        If row.Table.Columns(row_col).DataType = GetType(Double) Then

                            row(row_col) = Convert.ToDecimal(jobj(obj_pro).ToString.Replace(",", "."), Globalization.CultureInfo.InvariantCulture)
                        Else

                            row(row_col) = jobj(obj_pro)
                        End If
                    Else

                        If row.Table.Columns(row_col).DataType = GetType(String) Then

                            row(row_col) = ""
                        Else

                            row(row_col) = 0
                        End If
                    End If
                End If

                outer += 1
            End While

            idx += 1
        End While

        Return True
    End Function



    Private Function LeggiIndirizzo(ByVal flagInterferente As Boolean, ByVal riga As DataRow, ByVal objParametri_Server As AgronicaCoreParametri) As DataRow

        Dim campo_Piva As String = "P_Reg_Impianti_Piva"
        Dim campo_Sa_cod As String = "P_Reg_Impianti_Sa_Cod"
        If flagInterferente Then
            campo_Piva = "I_Reg_Impianti_Piva"
            campo_Sa_cod = "I_Reg_Impianti_Sa_Cod"
        End If
        campo_Piva = If(IsDBNull(riga(campo_Piva)), "", riga(campo_Piva))
        campo_Sa_cod = If(IsDBNull(riga(campo_Sa_cod)), "0", riga(campo_Sa_cod))
        If Not IsNumeric(campo_Sa_cod) Then
            campo_Sa_cod = "0"
        End If

        Try

            Dim filtro As String = "CentrixIndirizzi.Piva = '" & campo_Piva & "' AND CentrixIndirizzi.Sa_Cod = " & campo_Sa_cod
        Dim objIndir As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            Dim dt_Indirizzi As DataTable = objIndir.Leggi("", 0, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, filtro, "", objParametri_Server)

            If dt_Indirizzi.Rows.Count > 0 Then
                Return dt_Indirizzi.Rows(0)
            End If

        Catch ex As Exception

            Return Nothing
        End Try

        Return Nothing
    End Function

    Private Function infoUtente(ByVal flagInterferente As Boolean, ByVal riga As DataRow, ByVal dt_utenti As DataTable) As String
        Dim Username As String = ""
        Dim layerColName As String = "PropietarioLayer"
        Dim impiantoColName As String = "Propietario_Reg_Impianti_CODICE_FISCALE_TECNICO"
        If flagInterferente Then
            layerColName = "InterferenteLayer"
            impiantoColName = "Interferente_Reg_Impianti_CODICE_FISCALE_TECNICO"
        End If

        If riga.Table.Columns.Contains(layerColName) AndAlso Not IsDBNull(riga(layerColName)) Then
            Username = riga(layerColName)
        End If

        If String.IsNullOrEmpty(Username) Then
            Dim dr_utenti As DataRow() = dt_utenti.Select("CodFisc ='" & riga(impiantoColName) & "'")
            If (dr_utenti.Length > 0) AndAlso (Not IsNothing(dr_utenti(0)("username"))) Then
                Username = dr_utenti(0)("username")
            End If
        End If

        If Not String.IsNullOrEmpty(Username) Then
            Return Username
        End If

        Return "{undef.username}"
    End Function

    Private Function infoIndirizzo(ByVal flagInterferente As Boolean, ByVal riga As DataRow, ByVal dr_indirizzo As DataRow) As String
        Dim indir As String = ""
        Dim viaStringa As String = If(flagInterferente, "I_Via_Stringa", "P_Via_Stringa")

        If riga.Table.Columns.Contains(viaStringa) AndAlso Not IsDBNull(riga(viaStringa)) Then
            If Not String.IsNullOrEmpty(riga(viaStringa).ToString) Then
                indir &= "<span style='font-weight: bolder;'>" & riga(viaStringa).ToString & "</span> "
            End If
        End If
        If String.IsNullOrEmpty(indir) AndAlso dr_indirizzo IsNot Nothing Then
            indir &= "<span>"
            indir &= "<b>" & dr_indirizzo("Ind_Des") & "</b> " & dr_indirizzo("frz_des") & " (" & dr_indirizzo("com_des") & " - " & dr_indirizzo("pro_des") & ")"
            indir &= "</span> "
        End If

        Return indir
    End Function

    Private Function infoCoord(ByVal flagInterferente As Boolean, ByVal riga As DataRow, ByVal dr_indirizzo As DataRow) As String
        Dim coord As String = ""
        Dim colLat As String = "P_Lat"
        Dim colLng As String = "P_Lng"
        If flagInterferente Then
            colLat = "I_Lat"
            colLng = "I_Lng"
        End If

        Dim Lat As Double = -1
        Dim Lng As Double = -1
        If riga.Table.Columns.Contains(colLat) AndAlso Not IsDBNull(riga(colLat)) AndAlso IsNumeric(riga(colLat)) Then
            Lat = riga(colLat)
        End If
        If riga.Table.Columns.Contains(colLng) AndAlso Not IsDBNull(riga(colLng)) AndAlso IsNumeric(riga(colLng)) Then
            Lng = riga(colLng)
        End If

        If (Lat <= 0 Or Lng <= 0) AndAlso dr_indirizzo IsNot Nothing Then
            If Not IsDBNull(dr_indirizzo("lat")) AndAlso (IsNumeric(dr_indirizzo("lat"))) Then
                Lat = dr_indirizzo("lat")
            End If
            If Lng <= 0 AndAlso Not IsDBNull(dr_indirizzo("long")) AndAlso (IsNumeric(dr_indirizzo("long"))) Then
                Lng = dr_indirizzo("long")
            End If
        End If

        If Lat >= 0 AndAlso Lng >= 0 Then

            Lat = Agro_Math.ArrotondaVal_6(Lat)
            Lng = Agro_Math.ArrotondaVal_6(Lng)

            coord = "<div><i>Latitudine: </i><b>" & Lat.ToString("N6") & "</b> - <i>Longitudine: </i><b>" & Lng.ToString("N6") & "</b></div>"

        End If

        Return coord
    End Function

    Private Function infoSpecie(ByVal flagInterferente As Boolean, ByVal riga As DataRow, ByVal divStyle As String) As String

        Dim colVegDes As String = "P_Veg_Des"
        Dim colGrvaDes As String = "P_Grva_Des"
        If flagInterferente Then
            colVegDes = "I_Veg_Des"
            colGrvaDes = "I_Grva_Des"
        End If

        Dim result As String = ""
        If riga.Table.Columns.Contains(colVegDes) Then
            If Not String.IsNullOrEmpty(riga(colVegDes).ToString) Then
                result = "<i>Specie:</i> " & riga(colVegDes)
            End If
        End If
        If riga.Table.Columns.Contains(colGrvaDes) Then
            If Not String.IsNullOrEmpty(riga(colGrvaDes).ToString) Then
                If Not String.IsNullOrEmpty(result) Then
                    result &= " - "
                End If
                result &= "<i>Tipologia:</i> " & riga(colGrvaDes)
            End If
        End If
        If Not String.IsNullOrEmpty(result) Then
            If Not String.IsNullOrEmpty(divStyle) Then
                divStyle = " style='" & divStyle & "'"
            End If
            result = "<div" & divStyle & ">" & result & "</div>"
        End If
        Return result
    End Function

    Private Function infoValidita(ByVal flagInterferente As Boolean, ByVal riga As DataRow, ByVal divStyle As String) As String

        Dim colValInizio As String = "P_ValiditaInizio"
        Dim colValFine As String = "P_ValiditaFine"
        If flagInterferente Then
            colValInizio = "I_ValiditaInizio"
            colValFine = "I_ValiditaFine"
        End If

        Dim result As String = ""
        If riga.Table.Columns.Contains(colValInizio) Then
            If Not String.IsNullOrEmpty(riga(colValInizio).ToString) Then
                result = riga(colValInizio)
                If riga.Table.Columns.Contains(colValFine) Then
                    If Not String.IsNullOrEmpty(riga(colValFine).ToString) Then
                        result &= " - " & riga(colValFine)
                    End If
                End If
            End If
        End If
        If Not String.IsNullOrEmpty(result) Then
            If Not String.IsNullOrEmpty(divStyle) Then
                divStyle = " style='" & divStyle & "'"
            End If
            result = "<div" & divStyle & "><i>Validità: </i>" & result & "</div>"
        End If
        Return result

    End Function
    '####################################################################################
    'Private Shared Function GetDr_Indirizzi(ByVal dr As DataRow, ByRef _stringa As StringBuilder, ByVal dt_Indirizzi As DataTable, ByVal PrefissoCampoPiva As String, ByVal PrefissoCampoSa_cod As String, ByRef key_conflitto As String) As DataRow()
    '    Dim dr_Indirizzi As DataRow()


    '    Try
    '        If dr.Item("p_tipoEntita_cod") = 53 Then
    '            Try
    '                dr_Indirizzi = dt_Indirizzi.Select("Piva= '" & dr.Item(PrefissoCampoPiva & "_programmazione_Piva") & "' and sa_cod =" & dr.Item(PrefissoCampoSa_cod & "_programmazione_sa_cod"))
    '            Catch ex As Exception
    '                dr_Indirizzi = dt_Indirizzi.Select("Piva= '" & dr.Item(PrefissoCampoPiva & "_Reg_Impianti_Piva") & "' and sa_cod =" & dr.Item(PrefissoCampoSa_cod & "_Reg_Impianti_Sa_Cod"))
    '            End Try
    '        Else
    '            Try
    '                ' impianto
    '                dr_Indirizzi = dt_Indirizzi.Select("Piva= '" & dr.Item(PrefissoCampoPiva & "_Reg_Impianti_Piva") & "' and sa_cod =" & dr.Item(PrefissoCampoSa_cod & "_Reg_Impianti_Sa_Cod"))
    '            Catch ex As Exception
    '                dr_Indirizzi = dt_Indirizzi.Select("Piva= '" & dr.Item(PrefissoCampoPiva & "_programmazione_Piva") & "' and sa_cod =" & dr.Item(PrefissoCampoSa_cod & "_programmazione_sa_cod"))
    '            End Try

    '        End If

    '    Catch ex As Exception
    '        dr_Indirizzi = dt_Indirizzi.Select("Piva='' ")
    '    End Try


    '    If key_conflitto <> "-1" Then
    '        If key_conflitto = "" Then
    '            key_conflitto = dr.Item("Interferenze_cod")
    '        Else
    '            key_conflitto = key_conflitto & "@" & dr.Item("Interferenze_cod")
    '        End If
    '    End If

    '    _stringa.Append("<br />")
    '    Return dr_Indirizzi
    'End Function


    'Private Shared Sub FormattaRichiedente(ByVal dr_utenti As DataRow(), ByVal _stringa As StringBuilder, ByVal fontSize As String, ByVal boldTag As String, ByVal aggiungiLI As Boolean)
    '    Dim tagFont As String = ""
    '    Dim tagFontClose As String = ""

    '    If fontSize <> "1" Then
    '        tagFont = "<font style=""font-size: " & fontSize & "em""> "
    '        tagFontClose = "</font>"
    '    Else
    '        tagFont = " .. "
    '    End If

    '    If (dr_utenti.Length > 0) AndAlso (Not IsNothing(dr_utenti(0).Item("username"))) Then
    '        _stringa.Append(tagFont & boldTag & dr_utenti(0).Item("username") & boldTag.Replace("<", "</") & tagFontClose)
    '    Else
    '        _stringa.Append(tagFont & boldTag & "{undef.username}" & boldTag.Replace("<", "</") & tagFontClose)
    '    End If

    '    If aggiungiLI Then
    '        _stringa.Append("</li>")
    '    End If

    'End Sub



    ''###########################################################################################################
    'Private Shared Sub FormattaDescrizioneCampo(ByRef _stringa As StringBuilder, ByVal dr_Indirizzi As DataRow, ByVal AggiungiLI As Boolean)
    '    If AggiungiLI Then
    '        _stringa.Append("<li>")
    '    End If

    '    Dim lLat As Double = 0
    '    If Not IsDBNull(dr_Indirizzi.Item("lat")) AndAlso (IsNumeric(dr_Indirizzi.Item("lat"))) Then
    '        lLat = Agro_Math.RoundNumber_3Decimali(dr_Indirizzi.Item("lat"))
    '    Else
    '        lLat = 0
    '    End If

    '    Dim lLong As Double = 0
    '    If Not IsDBNull(dr_Indirizzi.Item("long")) AndAlso (IsNumeric(dr_Indirizzi.Item("long"))) Then
    '        lLong = Agro_Math.RoundNumber_3Decimali(dr_Indirizzi.Item("long"))
    '    Else
    '        lLong = 0
    '    End If

    '    _stringa.Append("<b>" & dr_Indirizzi.Item("Ind_Des") & "</b>  " & dr_Indirizzi.Item("frz_des") & " (" & dr_Indirizzi.Item("com_des") & " - " & dr_Indirizzi.Item("pro_des") & ") <b>&gt;</b> Lat: <b>" & lLat & " </b>.. Long: <b>" & lLong & "</b>")

    'End Sub

End Class
