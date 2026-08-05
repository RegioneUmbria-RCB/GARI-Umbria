Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Ricetta

        Sub New()

            Id_Agenda = 0
            Ricetta_Cod = 0
            Ricetta_Operazione_Cod = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

        End Sub

        Public Property Id_Agenda As Integer

        Public Property Ricetta_Cod As Integer

        Public Property Ricetta_Operazione_Cod As Integer

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

    End Class

    Public Class Agenda_Ricette_Helper

        Public Function Scrivi(ByVal Ricetta As Ricetta,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W

                objRicetta.Scrivi(Ricetta.Ricetta_Cod,
                                  Ricetta.Ricetta_Operazione_Cod,
                                  Ricetta.Id_Agenda,
                                  AGRODATAINIZIO,
                                  AGRODATAFINE,
                                  objParametri,
                                  Data_creazione:=Ricetta.Data_Creazione,
                                  username_creazione:=Ricetta.Username_Creazione)

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Ricette_Helper.Scrivi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return True

        End Function

        Public Function Cancella(ByVal idAgenda As Integer,
                                 ByVal ricettaCod As Integer,
                                 ByVal ricettaOperazioneCod As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


                Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W

                objRicetta.Cancella(ricettaCod,
                                    ricettaOperazioneCod,
                                    idAgenda,
                                    "",
                                    objParametri)

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Ricette_Helper.Cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return True

        End Function

        Public Function Leggi(ByVal idAgenda As Integer,
                              ByVal ricettaCod As Integer,
                              ByVal ricettaOperazioneCod As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Ricetta)

            Dim flagConnessione As Boolean = False

            Dim listaRicette As New List(Of Ricetta)

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)


                Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_R
                Dim dt As DataTable

                dt = objRicetta.Leggi(ricettaCod,
                                      ricettaOperazioneCod,
                                      idAgenda,
                                      AGRODATAINIZIO,
                                      AGRODATAFINE,
                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                      "", "",
                                      objParametri)

                objRicetta = Nothing

                Dim Ricetta As Ricetta

                For j = 0 To dt.Rows.Count - 1
                    Ricetta = New Ricetta With {
                        .Id_Agenda = dt.Rows(j).Item("Id_Agenda"),
                        .Ricetta_Cod = dt.Rows(j).Item("Ricetta_Cod"),
                        .Ricetta_Operazione_Cod = dt.Rows(j).Item("Ricetta_Operazione_Cod"),
                        .Data_Creazione = CDate(dt.Rows(j).Item("Data_Creazione")),
                        .Data_Modifica = CDate(dt.Rows(j).Item("Data_Modifica")),
                        .Username_Creazione = dt.Rows(j).Item("Username_Creazione"),
                        .Username_Modifica = dt.Rows(j).Item("Username_Modifica")
                    }
                    listaRicette.Add(Ricetta)
                Next


            Catch ex As Exception

                Throw New Exception("[ Agenda_Ricette_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaRicette

        End Function

    End Class

End Namespace
