Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Rilievi

    Public Class Rilievo_IndiciMaturita
        Implements I_RilievoInCampo


        Private _Impianto_Colturale As Anagrafe.Impianto_Colturale
        Private _IndiceMaturita As Integer
        Private _Valore As Integer
        Private _UnitaDiMisura As Integer

        Public Property Impianto_Colturale As Anagrafe.Impianto_Colturale Implements I_RilievoInCampo.Impianto_Colturale
            Get
                Return _Impianto_Colturale
            End Get
            Set(value As Anagrafe.Impianto_Colturale)
                _Impianto_Colturale = value
            End Set
        End Property

        Public Property DatoDaRilevare As Object Implements I_RilievoInCampo.DatoDaRilevare
            Get
                Return IndiceMaturita
            End Get
            Set(value As Object)
                IndiceMaturita = value
            End Set
        End Property

        Public Property IndiceMaturita() As Integer
            Get
                Return _IndiceMaturita
            End Get
            Set(value As Integer)
                _IndiceMaturita = value
            End Set
        End Property

        Public Property DatoRilevato As Object Implements I_RilievoInCampo.DatoRilevato
            Get
                Return Valore
            End Get
            Set(value As Object)
                Valore = value
            End Set
        End Property

        Public Property Valore() As Integer
            Get
                Return _Valore
            End Get
            Set(value As Integer)
                _Valore = value
            End Set
        End Property

        Public Property UnitaDiMisura() As Integer
            Get
                Return _UnitaDiMisura
            End Get
            Set(value As Integer)
                _UnitaDiMisura = Value
            End Set
        End Property

        Public Property DatoAccessorio As Object Implements I_RilievoInCampo.DatoAccessorio
            Get
                Return UnitaDiMisura
            End Get
            Set(value As Object)
                UnitaDiMisura = value
            End Set
        End Property


    End Class

End Namespace


