Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Rilievi

    Public Class Rilievo_FasiFenologiche
        Implements I_RilievoInCampo


        Private _Impianto_Colturale As Anagrafe.Impianto_Colturale
        Private _FaseFenologica As Integer
        Private _DataRilevata As Date
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
                Return FaseFenologica
            End Get
            Set(value As Object)
                FaseFenologica = value
            End Set
        End Property

        Public Property FaseFenologica() As Integer
            Get
                Return _FaseFenologica
            End Get
            Set(value As Integer)
                _FaseFenologica = value
            End Set
        End Property

        Public Property DatoRilevato As Object Implements I_RilievoInCampo.DatoRilevato
            Get
                Return DataRilevata
            End Get
            Set(value As Object)
                DataRilevata = value
            End Set
        End Property

        Public Property DataRilevata() As Date
            Get
                Return _DataRilevata
            End Get
            Set(value As Date)
                _DataRilevata = value
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


