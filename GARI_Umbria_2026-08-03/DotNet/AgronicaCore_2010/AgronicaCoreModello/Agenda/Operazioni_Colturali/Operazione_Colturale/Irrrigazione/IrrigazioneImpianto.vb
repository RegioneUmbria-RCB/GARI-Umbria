Imports AgronicaCoreDataProvider

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Irrigazione



    Public Class IrrigazioneImpianto



        Private _ImpiantoIrrigato As Anagrafe.Impianto_Colturale
        Private _Dose As Decimal
        Private _UDM_Dose As Integer
        Private _Ore As Decimal
        Private _Portata As Decimal
        Private _Data_Inizio As Date
        Private _Data_Fine As Date
        Private _Frequenza As Integer
        Private _Qta_Totale As Decimal
        Private _TipoIrrigazioneUtilizzata As Integer
        Private _Qta2 As Decimal

        Sub New(ByVal ImpiantoIrrigato_In As Anagrafe.Impianto_Colturale, ByVal Dose_In As Decimal, ByVal UDM_Dose_In As Integer, ByVal Ore_In As Decimal, ByVal Portata_In As Decimal, ByVal Data_Inizio_In As Date, ByVal Data_Fine_In As Date, ByVal Frequenza_In As Integer, ByVal Qta_Totale_In As Decimal, ByVal TipoIrrigazioneUtilizzata_In As Integer, ByVal Qta2_In As Decimal)
            ImpiantoIrrigato = ImpiantoIrrigato_In
            Dose = Dose_In
            UDM_Dose = UDM_Dose_In
            Ore = Ore_In
            Portata = Portata_In
            Data_Inizio = Data_Inizio_In
            Data_Fine = Data_Fine_In
            Frequenza = Frequenza_In
            Qta_Totale = Qta_Totale_In
            TipoIrrigazioneUtilizzata = TipoIrrigazioneUtilizzata_In
            Qta2 = Qta2_In
        End Sub

        Sub New(ByVal ImpiantoIrrigato_In As Anagrafe.Impianto_Colturale)
            ImpiantoIrrigato = ImpiantoIrrigato_In
            Dose = 0
            UDM_Dose = 0
            Ore = 0
            Portata = 0
            Data_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
            Data_Fine = CostantiPersonalizzate.AGRODATAFINE
            Frequenza = 0
            Qta_Totale = 0
            TipoIrrigazioneUtilizzata = 0
            Qta2 = 0
        End Sub

        Sub New()
            ImpiantoIrrigato = New Anagrafe.Impianto_Colturale("", 0, 0, 0, 0, #1/1/1900#, #12/31/2100#)
            Dose = 0
            UDM_Dose = 0
            Ore = 0
            Portata = 0
            Data_Inizio = CostantiPersonalizzate.AGRODATAINIZIO
            Data_Fine = CostantiPersonalizzate.AGRODATAFINE
            Frequenza = 0
            Qta_Totale = 0
            TipoIrrigazioneUtilizzata = 0
            Qta2 = 0
        End Sub








        Public Property Data_Fine() As Date
            Get
                Return _Data_Fine
            End Get
            Set(value As Date)
                _Data_Fine = Value
            End Set
        End Property
        Public Property Data_Inizio() As Date
            Get
                Return _Data_Inizio
            End Get
            Set(value As Date)
                _Data_Inizio = Value
            End Set
        End Property
        Public Property Dose() As Decimal
            Get
                Return _Dose
            End Get
            Set(value As Decimal)
                _Dose = Value
            End Set
        End Property
        Public Property Frequenza() As Integer
            Get
                Return _Frequenza
            End Get
            Set(value As Integer)
                _Frequenza = Value
            End Set
        End Property
        Public Property ImpiantoIrrigato() As Anagrafe.Impianto_Colturale
            Get
                Return _ImpiantoIrrigato
            End Get
            Set(value As Anagrafe.Impianto_Colturale)
                _ImpiantoIrrigato = Value
            End Set
        End Property
        Public Property Ore() As Decimal
            Get
                Return _Ore
            End Get
            Set(value As Decimal)
                _Ore = Value
            End Set
        End Property
        Public Property Portata() As Decimal
            Get
                Return _Portata
            End Get
            Set(value As Decimal)
                _Portata = Value
            End Set
        End Property
        Public Property Qta_Totale() As Decimal
            Get
                Return _Qta_Totale
            End Get
            Set(value As Decimal)
                _Qta_Totale = Value
            End Set
        End Property
        Public Property TipoIrrigazioneUtilizzata() As Integer
            Get
                Return _TipoIrrigazioneUtilizzata
            End Get
            Set(value As Integer)
                _TipoIrrigazioneUtilizzata = Value
            End Set
        End Property
        Public Property UDM_Dose() As Integer
            Get
                Return _UDM_Dose
            End Get
            Set(value As Integer)
                _UDM_Dose = Value
            End Set
        End Property
        Public Property Qta2() As Decimal
            Get
                Return _Qta2
            End Get
            Set(value As Decimal)
                _Qta2 = value
            End Set
        End Property






    End Class

End Namespace






