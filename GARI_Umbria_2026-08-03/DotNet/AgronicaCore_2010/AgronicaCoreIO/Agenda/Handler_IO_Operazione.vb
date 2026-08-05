Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.Agenda.Operazioni_Colturali

Namespace Agenda

    Public Module Handler_IO_Operazione





        Public Function Leggi_Operazione_Da_Agenda(ByRef Operazione_Colturale_Da_Leggere As Operazione_Colturale.I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
            Dim Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale
            Try
                Dim IO_Operaz As New OperazioneColturale_ToFrom_AgendaDB(Operazione_Colturale_Da_Leggere, objParametri_Server)
                Temp_Operazione_Colturale = IO_Operaz.Leggi_Operazione()
            Catch
                Return False
            End Try
            If Not IsNothing(Temp_Operazione_Colturale) Then
                Operazione_Colturale_Da_Leggere = Temp_Operazione_Colturale
                Return True
            Else
                Return False
            End If
            Return False
        End Function


        Function Salva_Operazione_Su_Agenda(ByRef Operazione_Colturale_Da_Salvare As Operazione_Colturale.I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri, ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str, ByRef Id_Agenda_return As Integer) As Boolean
            Try
                Dim IO_Operaz As New OperazioneColturale_ToFrom_AgendaDB(Operazione_Colturale_Da_Salvare, objParametri_Server)
                Return IO_Operaz.Salva_Operazione(messaggiErrore, Id_Agenda_return)
            Catch
                Return False
            End Try
            Return False
        End Function


    End Module

End Namespace
