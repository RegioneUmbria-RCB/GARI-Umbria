Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Nota

        Sub New()

            Id_Agenda = 0
            Nota_Cod = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

        End Sub

        Public Property Id_Agenda As Integer

        Public Property Nota_Cod As Integer

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

    End Class

    Public Class Agenda_Note_Helper

        Public Function Scrivi(ByVal Nota As Nota,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'NOTE
                '---------------------------

                Dim objNota As New AgronicaCoreContabDAL.AgendaxNote_W

                objNota.Scrivi(Nota.Id_Agenda,
                               Nota.Nota_Cod,
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                               objParametri,
                               Data_creazione:=Nota.Data_Creazione,
                               username_creazione:=Nota.Username_Creazione)

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Note_Helper.Scrivi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return True

        End Function

        Public Function Cancella(ByVal idAgenda As Integer,
                                 ByVal notaCod As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'NOTE
                '---------------------------

                Dim objNota As New AgronicaCoreContabDAL.AgendaxNote_W

                objNota.Cancella(idAgenda,
                                 notaCod,
                                 "",
                                 objParametri)

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Note_Helper.Cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return True

        End Function

        Public Function Leggi(ByVal idAgenda As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Nota)

            Dim flagConnessione As Boolean = False

            Dim listaNote As New List(Of Nota)

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '---------------------------
                'NOTE
                '---------------------------

                Dim objNota As New AgronicaCoreContabDAL.AgendaxNote_R
                Dim dt As DataTable

                dt = objNota.Leggi(idAgenda,
                                   0,
                                   AGRODATAINIZIO,
                                   AGRODATAFINE,
                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                   "", "",
                                   objParametri)

                objNota = Nothing

                Dim Nota As Nota

                For j = 0 To dt.Rows.Count - 1
                    Nota = New Nota With {
                        .Id_Agenda = dt.Rows(j).Item("Id_Agenda"),
                        .Nota_Cod = dt.Rows(j).Item("Nota_Cod"),
                        .Data_Creazione = CDate(dt.Rows(j).Item("Data_Creazione")),
                        .Data_Modifica = CDate(dt.Rows(j).Item("Data_Modifica")),
                        .Username_Creazione = dt.Rows(j).Item("Username_Creazione"),
                        .Username_Modifica = dt.Rows(j).Item("Username_Modifica")
                    }
                    listaNote.Add(Nota)
                Next


            Catch ex As Exception

                Throw New Exception("[ Agenda_Note_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaNote

        End Function

    End Class

End Namespace
