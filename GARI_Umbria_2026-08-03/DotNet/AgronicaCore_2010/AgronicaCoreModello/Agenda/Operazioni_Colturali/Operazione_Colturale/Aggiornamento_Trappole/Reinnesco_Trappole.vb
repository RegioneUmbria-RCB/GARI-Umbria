Imports AgronicaCoreDataProvider


Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole

    Public Class Reinnesco_Trappole
        Inherits Abstr_Aggiornamento_Trappole

        Private _Reinneschi As List(Of Reinnesco)

        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
            'modifica temporanea, per rilievo devo creare altro modo
            ImpostaParametriOperazione(objParametri_Server, Lav_Cod)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri, ByVal lavcod As Integer)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
            'modifica temporanea, per rilievo devo creare altro modo
            ImpostaParametriOperazione(objParametri_Server, lavcod)
        End Sub

        Private Sub ImpostaParametriOperazione(objParametri_Server As AgronicaCoreParametri, ByVal lavcod As Integer)
            'modifica temporanea, per rilievo devo creare altro modo
            'modifica temporanea, per rilievo devo creare altro modo
            'modifica temporanea, per rilievo devo creare altro modo
            'modifica temporanea, per rilievo devo creare altro modo
            '_Lav_Cod = CostantiPersonalizzate.LAVCOD_REINNESCO_TRAPPOLE
            _Lav_Cod = lavcod
            'bruttissimo, devo modificare!!!! e usare ereditarietà come le altre
            'modifica temporanea, per rilievo devo creare altro modo
            'modifica temporanea, per rilievo devo creare altro modo
            'modifica temporanea, per rilievo devo creare altro modo
            'modifica temporanea, per rilievo devo creare altro modo

            _Cau_Mov = TipiEnumerativi.enum_Agenda_Causali.RILIEVO_CAMPO
            _Cau_Des = "Rilievo In Campo"
            If IsNothing(objParametri_Server) Then
                _Lav_Des = ""
            Else
                Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
                _Lav_Des = objOperazioni.Lav_Des_From_Lav_Cod(_Lav_Cod, objParametri_Server)
            End If
            _OperazioneMulticentro = False
            _Reinneschi = New List(Of Reinnesco)
        End Sub


        Public Property Reinneschi() As List(Of Reinnesco)
            Get
                Return _Reinneschi
            End Get
            Set(value As List(Of Reinnesco))
                _Reinneschi = value
            End Set
        End Property

    End Class


End Namespace




