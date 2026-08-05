Imports System.Web
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports System.Diagnostics.Contracts

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Rilievi

    Public MustInherit Class Abstr_Rilievi
        Inherits Operazione_Colturale

        Private _Rilievi As List(Of I_RilievoInCampo)

        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
        End Sub

        Protected Sub ImpostaParametriOperazione()
            Rilievi = New List(Of I_RilievoInCampo)
            ImpostaParametriOperazione_2()
        End Sub

        Protected MustOverride Sub ImpostaParametriOperazione_2()

        Public Property Rilievi() As List(Of I_RilievoInCampo)
            Get
                Return _Rilievi
            End Get
            Set(value As List(Of I_RilievoInCampo))
                _Rilievi = Value
            End Set
        End Property

        'Metodi che devono essere implementati dalle classi che implememntano l'operazione specifica, e che sono in  grado
        'di ricavare gli appezzamenti, i centri e le aziende coinvolte nella operazione
#Region "Metodi Overrides"

        Public Function Impianti_CoinvoltiNellaOperazione() As System.Collections.Generic.List(Of Anagrafe.Impianto_Colturale)
            Dim listRidondanteImpianti As New System.Collections.Generic.List(Of Anagrafe.Impianto_Colturale)
            For Each rilievo As I_RilievoInCampo In Rilievi
                listRidondanteImpianti.Add(rilievo.Impianto_Colturale)
            Next
            Return TogliImpiantiRidondanti(listRidondanteImpianti)
        End Function

        Protected Function TogliImpiantiRidondanti(ByRef ListRidondante As System.Collections.Generic.List(Of Anagrafe.Impianto_Colturale)) As System.Collections.Generic.List(Of Anagrafe.Impianto_Colturale)
            Dim list As New System.Collections.Generic.List(Of Anagrafe.Impianto_Colturale)
            Dim presente = False
            For Each Impianto As Anagrafe.Impianto_Colturale In ListRidondante
                presente = False
                For Each obj As Anagrafe.Impianto_Colturale In list
                    If Impianto.UgualeA(obj) Then
                        presente = True
                    End If
                    If Not presente Then
                        list.Add(Impianto)
                    End If
                Next
            Next
            Return list
        End Function

#End Region



    End Class

End Namespace
