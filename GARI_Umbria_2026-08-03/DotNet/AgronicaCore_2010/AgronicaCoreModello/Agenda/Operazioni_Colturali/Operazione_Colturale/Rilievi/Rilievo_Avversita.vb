Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Rilievi

    Public Class Rilievo_Avversita
        Implements I_RilievoInCampo


        Private _Impianto_Colturale As Anagrafe.Impianto_Colturale
        Private _Avversita As Integer
        Private _QuantitaRilevata As Integer
        Private _DatoAccessorio As Object



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
                Return Avversita
            End Get
            Set(value As Object)
                Avversita = value
            End Set
        End Property

        Public Property Avversita() As Integer
            Get
                Return _Avversita
            End Get
            Set(value As Integer)
                _Avversita = Value
            End Set
        End Property

        Public Property DatoRilevato As Object Implements I_RilievoInCampo.DatoRilevato
            Get
                Return QuantitaRilevata
            End Get
            Set(value As Object)
                QuantitaRilevata = value
            End Set
        End Property

        Public Property QuantitaRilevata() As Integer
            Get
                Return _QuantitaRilevata
            End Get
            Set(value As Integer)
                _QuantitaRilevata = Value
            End Set
        End Property

        Public Property DatoAccessorio As Object Implements I_RilievoInCampo.DatoAccessorio
            Get
                Return _DatoAccessorio
            End Get
            Set(value As Object)
                _DatoAccessorio = value
            End Set
        End Property

    End Class

End Namespace


