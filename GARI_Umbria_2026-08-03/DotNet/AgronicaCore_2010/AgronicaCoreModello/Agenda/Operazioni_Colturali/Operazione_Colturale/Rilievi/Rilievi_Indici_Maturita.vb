Imports AgronicaCoreDataProvider

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Rilievi

    Public Class Rilievi_Indici_Maturita
        Inherits Abstr_Rilievi

        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri, ByVal lavcod As Integer)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
        End Sub

        Protected Overrides Sub ImpostaParametriOperazione_2()
            'ImpostaLavorazione(CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA)
            ImpostaCauMov(TipiEnumerativi.enum_Agenda_Causali.RILIEVO_RACCOLTA)
        End Sub

        'Public Property Rilievi_Spec() As List(Of Rilievo_IndiciMaturita)
        '    Get
        '        Return Rilievi
        '    End Get
        '    Set(value As List(Of Rilievo_IndiciMaturita))
        '        _Rilievi = value
        '    End Set
        'End Property

    End Class

End Namespace

