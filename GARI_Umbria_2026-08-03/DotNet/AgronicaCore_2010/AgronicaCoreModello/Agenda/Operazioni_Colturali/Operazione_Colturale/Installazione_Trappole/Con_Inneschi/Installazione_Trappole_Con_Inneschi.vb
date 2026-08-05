Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Installazione_Trappole.Con_Inneschi

    Public Class Installazione_Trappole_Con_Inneschi
        Inherits Abstr_Installazione_Trappole_Con_Inneschi

        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
            ImpostaParametriOperazione(objParametri_Server)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
            ImpostaParametriOperazione(objParametri_Server)
        End Sub

        Private Sub ImpostaParametriOperazione(objParametri_Server As AgronicaCoreParametri)
            _Lav_Cod = CostantiPersonalizzate.LAVCOD_INSTALLAZIONE_TRAPPOLE
            _Cau_Mov = TipiEnumerativi.enum_Agenda_Causali.RILIEVO_CAMPO
            _Cau_Des = "Rilievo In Campo"
            If IsNothing(objParametri_Server) Then
                _Lav_Des = ""
            Else
                Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
                _Lav_Des = objOperazioni.Lav_Des_From_Lav_Cod(_Lav_Cod, objParametri_Server)
            End If
            _OperazioneMulticentro = False
        End Sub

    End Class


End Namespace

