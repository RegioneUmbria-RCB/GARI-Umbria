Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis

Public Class Messaggi_Esecuzione_R
    Public Function LeggiMessaggiEsecuzioneNonLetti(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of MessaggioEsecuzione)
        Dim xRead As New AgronicaCoreMessaggisticaDAL.Messaggi_Esecuzione_R

        Dim DT As DataTable

        Dim resp As New List(Of MessaggioEsecuzione)

        DT = xRead.LeggiMessaggiEsecuzioneNonLetti(objParametri_Server)

        If DT Is Nothing Then
            Return resp
        End If

        For Each row In DT.Rows
            Dim messaggio As New MessaggioEsecuzione With {
                .ID_Messaggio = CInt(row("ID_Messaggio")),
                .Destinatario_UserName = row("Destinatario_UserName").ToString,
                .Testo_Messaggio = row("Testo_Messaggio").ToString
            }

            resp.Add(messaggio)
        Next

        Return resp
    End Function

    Public Function LeggiMessaggioEsecuzione(ByVal ID_Messaggio As Int32,
                                             ByRef objParametri_Server As AgronicaCoreParametri) As MessaggioEsecuzione

        Dim xRead As New AgronicaCoreMessaggisticaDAL.Messaggi_Esecuzione_R

        Dim DT As DataTable

        DT = xRead.LeggiMessaggioByID(ID_Messaggio, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Throw New Exception("Messaggio non trovato.")
        End If

        Dim messaggio As New MessaggioEsecuzione With {
            .ID_Messaggio = CInt(DT.Rows(0)("ID_Messaggio")),
            .Destinatario_UserName = DT.Rows(0)("Destinatario_UserName").ToString,
            .Testo_Messaggio = DT.Rows(0)("Testo_Messaggio").ToString,
            .letto = CBool(DT.Rows(0)("Letto")),
            .annullato = CBool(DT.Rows(0)("Annullata_Lettura"))
        }

        Return messaggio

    End Function
End Class
Public Class Messaggi_Esecuzione_W
    Public Function AccodaMessaggioEsecuzione(ByVal destinatario_UserName As String,
                                              ByVal testo_Messaggio As String,
                                              ByRef objParametri_server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean
        Dim xWrite As New AgronicaCoreMessaggisticaDAL.Messaggi_Esecuzione_W
        Dim sequenza As New Agro_Sequenze

        Dim newIDMessaggio = sequenza.NuovoId_Tabella("Messaggistica_Programmazione", 0, Int32.MaxValue, objParametri_server, True)


        resp = xWrite.AccodaMessaggioEsecuzione(destinatario_UserName, testo_Messaggio, newIDMessaggio, objParametri_server)

        If Not resp Then
            Throw New Exception("Errore nell'accodamento del messaggio.")
        End If

        Return resp
    End Function

    Public Function ModificaMessaggioEsecuzione(ByVal ID_Messaggio As Int32,
                                                ByVal letto As Boolean,
                                                ByVal annullato As Boolean,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean
        Dim xWrite As New AgronicaCoreMessaggisticaDAL.Messaggi_Esecuzione_W
        Dim xRead As New AgronicaCoreMessaggisticaDAL.Messaggi_Esecuzione_R
        Dim DT As DataTable

        If letto And annullato Then
            Throw New Exception("Il messaggio non può essere letto ed annullato contemporaneamente.")
        End If

        DT = xRead.LeggiMessaggioByID(ID_Messaggio, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Messaggio non trovato.")
        End If

        resp = xWrite.ModificaMessaggioEsecuzione(ID_Messaggio, letto, annullato, objParametri_Server)

        If Not resp Then
            Throw New Exception("Errore nella modifica del messaggio.")
        End If

        Return resp
    End Function
End Class
