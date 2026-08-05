Namespace AnagrafeNG

    Public Class AppezzamentoGlobal
        Public Property Impresa As Imprese
        Public Property Centro As Centri
        Public Property Campo As Campi

        Public Property Appezzamento As Appezzamento
        Public Property Impianti As New List(Of Impianto)
    End Class


    Public Class Appezzamento
        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Appezza As Integer
        Public Property Campo_Cod As Integer

        'Dati Generali
        Public Property Superficie As Double
        Public Property Denominazione As String
        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date
        Public Property Rif_Appezzamento As String
        Public Property Isola As String
        Public Property Metodo_Produzione As Integer

        'Rotazioni Colturali
        Public Property Coltura_Precedente_1_Anno As String
        Public Property Coltura_Precedente_2_Anno As String
        Public Property Coltura_Precedente_3_Anno As String
        Public Property Coltura_Precedente_4_Anno As String

        'Posizione Appezzamento
        Public Property Pendenza As Double
        Public Property Esposizione As String
        Public Property Ubicazione As String

        'Coordinate Baricentro
        Public Property Lat As Double
        Public Property Lng As Double
        Public Property Altitudine As Double

        'Buffer Zone
        Public Property DistBZ_CorpiIdrici As Double
        Public Property DistBZ_AreeResPub As Double
        Public Property DistBZ_Allevamenti As Double
        Public Property DistBZ_VegNatNonColt As Double
        Public Property SupBZ_Riduzione As Double

        'Biologico
        Public Property N_App_Bio As String
        Public Property Utilizzo_Terreno As Integer
        Public Property Confini_A_Rischio As String
        Public Property Fine_Impiego_Prod_Non_Conformi As Date

        Public Property Codici As New List(Of Codici)
        Public Property Indirizzi As New List(Of Indirizzi)
        Public Property Catasto As New List(Of Catasto_Appezzamento)
    End Class

    Public Class Impianto
        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Appezza As Integer
        Public Property Id_Reg As Integer

        Public Property Veg_Des As String
        Public Property Destinazione_Uso As String
        Public Property Cul_Des As String

        Public Property Veg_Cod As Integer
        Public Property Gru_Cod As Integer
        Public Property Id_Cod As Integer
        Public Property Cul_Cod As Integer
        Public Property Grfi_Cod As Integer
        Public Property Grva_Cod As Integer
        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date
        Public Property Superficie As Double
        Public Property Data_Innesto_Varieta As Date
        Public Property Data_Inizio_Produzione As Date
        Public Property Esercizi As New List(Of Esercizio)

        Public Property Codici As New List(Of Codici)

        '
        'Dati Accessori
        '
        Public Property codice_impianto As String
        'Sez Densita Impianto
        Public Property Cover_Crops As Boolean
        Public Property Monitorato As Boolean
        Public Property Imp_Cod As Integer
        Public Property Foral_Cod As Integer
        Public Property Port_Cod As Integer
        Public Property SeminaTrapianto As String
        Public Property ProvenienzaSeme As Integer
        Public Property Tecn_Cod_Tra As Integer
        Public Property Tecn_Cod_Su As Integer
        Public Property InfoAgg_Cod As Integer
        Public Property Id_Consociazione As Integer
        Public Property Maschi_in_Sesto As Integer

        Public Property Tra_Fila_M As Double
        Public Property Su_Fila_M As Double

        Public Property Cop_Cod As Integer
        Public Property Cop_Data_Inizio As Date
        Public Property Cop_Data_Fine As Date

        Public Property Unita_Vitata As String


        Public Property Impianto_Ibrido As Boolean

        'Linea Maschio
        Public Property CodBMBDBT_M As String
        Public Property CodBMBDBT_F As String
        Public Property Genetica_M As String
        Public Property Genetica_F As String
        Public Property OffType_M As String
        Public Property OffType_F As String
        Public Property DistanzaSuFila_F As Double
        Public Property DistanzaTraFila_F As Double

        'Pannello Tuberi
        Public Property PartiTuberi As Double
        Public Property TagliatoIntero As String

        Public Property Interbina As Integer
        Public Property Germinabilita As Integer
        Public Property dettaglio_varieta_personalizzato As String

    End Class

    Public Class Esercizio
        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Appezza As Integer
        Public Property Id_Reg As Integer
        Public Property Progetto_Cod As Integer

        Public Property Lotto As String
        Public Property Descrizione As String
        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date

        Public Property Piante_Ha As Double
        Public Property Piante_Impianto As Double
        Public Property Codice As Integer
        Public Property Piante_Ha_Femmine As Integer
        Public Property Piante_Ha_Impianto_Femmine As Integer
        Public Property Piante_Ha_Maschi As Integer
        Public Property Piante_Ha_Impianto_Maschi As Integer

        Public Property Data_Semina_Trapianto As Date
        Public Property Data_Raccolta As Date
        Public Property Data_Fioritura As Date
        Public Property Resa As Double

        Public Property Regolamento As String
        Public Property Disciplinare As String
        Public Property DisciplinarePubblicoPrivato As Integer
        Public Property Regolamento_Concimazione_Cod As String
        Public Property Id_tr As Integer
        Public Property IAF As New List(Of Integer)

        Public Property Capitolato_Privato As String
        Public Property Organismo_Referente As String
        Public Property Licenza_Coltivazione As String
        Public Property Riferimento_Trasferimento_Dati As String
        Public Property Magazzino_Conferimento As String
        Public Property Piano_Semina As String
        Public Property Esercizio_Chiuso As Boolean

        'Apporti massimi di Macroelementi
        Public Property Reg_Fert As String
        Public Property FinalitaConc As String
        Public Property Stato As String
        Public Property N As Double
        Public Property P2O5 As Double
        Public Property K2O As Double
        Public Property MGO As Double

        Public Property Codici As New List(Of Codici)
        Public Property Particelle As New List(Of Progetti_Particelle)
    End Class


    Public Class Catasto_Appezzamento
        Public Property Prov As String
        Public Property Com As String
        Public Property Sezione As String
        Public Property Foglio As Integer
        Public Property Numero As Integer
        Public Property Subalterno As String
        Public Property Area As Double
    End Class

    Public Class Progetti_Particelle
        Public Property Prov As String
        Public Property Com As String
        Public Property Sezione As String
        Public Property Foglio As Integer
        Public Property Numero As Integer
        Public Property Subalterno As String
        Public Property Prov_Des As String
        Public Property Com_Des As String
        Public Property Id_Cod As Integer
        Public Property Descrizione As String
        Public Property Val_Cod As String
    End Class

End Namespace
