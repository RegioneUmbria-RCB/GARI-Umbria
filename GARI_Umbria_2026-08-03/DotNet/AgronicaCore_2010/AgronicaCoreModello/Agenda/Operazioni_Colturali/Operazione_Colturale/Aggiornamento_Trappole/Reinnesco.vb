Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole

    Public Class Reinnesco

        Private _Trappola As Installazione_Trappole.Con_Inneschi.Trappola
        Private _NumeroReinneschi As Integer

        Public Sub New(ByRef Trappola_Interessata As Installazione_Trappole.Con_Inneschi.Trappola, ByVal Numero_Reinneschi As Integer)
            Trappola = Trappola_Interessata
            NumeroReinneschi = Numero_Reinneschi
        End Sub


        Public Property Trappola As Installazione_Trappole.Con_Inneschi.Trappola
            Get
                Return _Trappola
            End Get
            Set(value As Installazione_Trappole.Con_Inneschi.Trappola)
                _Trappola = value
            End Set
        End Property

        Public Property NumeroReinneschi As Integer
            Get
                Return _NumeroReinneschi
            End Get
            Set(value As Integer)
                _NumeroReinneschi = value
            End Set
        End Property

    End Class

End Namespace

