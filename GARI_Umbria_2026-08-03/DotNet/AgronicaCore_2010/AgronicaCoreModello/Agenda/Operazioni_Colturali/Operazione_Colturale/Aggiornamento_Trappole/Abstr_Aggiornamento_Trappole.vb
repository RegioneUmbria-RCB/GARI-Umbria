Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole

    Public MustInherit Class Abstr_Aggiornamento_Trappole
        Inherits Agenda.Operazioni_Colturali.Operazione_Colturale.Operazione_Colturale

        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
        End Sub

    End Class

End Namespace

