Imports AgronicaCoreDataProvider


Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Irrigazione

    Public Class Irrigazioni
        Inherits Operazione_Colturale

        Private _IrrigazioniImpianti As List(Of IrrigazioneImpianto)


        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
            ImpostaParametriOperazione(objParametri_Server)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
            ImpostaParametriOperazione(objParametri_Server)
        End Sub

        Private Sub ImpostaParametriOperazione(objParametri_Server As AgronicaCoreParametri)
            _Lav_Cod = CostantiPersonalizzate.LAVCOD_IRRIGAZIONE
            _Cau_Mov = TipiEnumerativi.enum_Agenda_Causali.LAVORAZIONE
            _Cau_Des = "Lavorazione"
            If IsNothing(objParametri_Server) Then
                _Lav_Des = ""
            Else
                Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
                _Lav_Des = objOperazioni.Lav_Des_From_Lav_Cod(_Lav_Cod, objParametri_Server)
            End If
            _OperazioneMulticentro = False
        End Sub


        Public Property IrrigazioniImpianti() As List(Of IrrigazioneImpianto)
            Get
                Return _IrrigazioniImpianti
            End Get
            Set(value As List(Of IrrigazioneImpianto))
                _IrrigazioniImpianti = value
            End Set
        End Property

    End Class


End Namespace




