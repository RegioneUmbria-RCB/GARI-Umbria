Imports AgronicaCoreDataProvider

Public Class Statistiche_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function getStatistiche_PrevisioniAI(piva As String,
                                                dataStats As Date,
                                                objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim objStatPrevisioniDAL As New AgronicaCoreContabDAL.Statistiche_R
        Dim dt As DataTable

        Dim Inizio_Anno = "01/01/" & dataStats.Year
        Dim Fine_Anno = "31/12/" & dataStats.Year

        Dim xFiltroAggiuntivo As String = ""
        xFiltroAggiuntivo = xFiltroAggiuntivo + " Reg_Impianti.Cul_Cod != 0 "
        xFiltroAggiuntivo = xFiltroAggiuntivo + " AND ( ((Imprese_Progetti.Validita_inizio <= " + UtilityProvider.Agro_SQL_SaveDate(dataStats)
        xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(dataStats) + " <= Imprese_Progetti.Validita_Fine)) OR  "
        xFiltroAggiuntivo = xFiltroAggiuntivo + "   ((Imprese_Progetti.Validita_Fine >= " + UtilityProvider.Agro_SQL_SaveDate(Inizio_Anno)
        xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(Fine_Anno) + " >= Imprese_Progetti.Validita_Fine) ) ) "

        dt = objStatPrevisioniDAL.Leggi_Statistiche_PrevisioniAI(piva, xFiltroAggiuntivo, objParametri_Server)

        Return dt

    End Function

    ''' <summary>
    ''' Calcola le stime di produzione per gli impianti
    ''' </summary>
    Private Sub CalcolaStimeProduzione(filtroProgetti As List(Of Integer),
                                      ByRef dt As DataTable,
                                      dataReport As Date,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim statistiche As New AgronicaCoreContabDAL.Statistiche_R
        dt.Columns.Add("Resa_Ultimo_Rilievo", GetType(Double))
        dt.Columns.Add("Data_Ultimo_Rilievo", GetType(Date))
        dt.Columns.Add("Utente_Ultimo_Rilievo", GetType(String))
        dt.Columns.Add("Stime_Produzione_Rilievo", GetType(Double))
        dt.Columns.Add("Resa_Impianto_Tot", GetType(Double))

        ' stima produzione rilievo
        Dim dtRilievi = statistiche.Leggi_Rilievi_Stime_Produzione_NEW(filtroProgetti, objParametri_Server, objParametri_Utenti, dataReport)

        For Each row In dt.Rows

            Dim supTotale As Double = row.Item("Sup_Imp")
            Dim supAbbattuta As Double = row.Item("Sup_Abbattuta")
            Dim rilievoDanni As Double? = If(IsDBNull(row.Item("Perc_Piante_Morte")), Nothing, row.Item("Perc_Piante_Morte"))
            Dim produzionePrevista As Double = row.Item("Produzione_Prevista")

            Dim supPostAbbattimento As Double = supTotale
            Dim produzionePostRilievoDanni As Double = produzionePrevista
            Dim StimaProduzione As Double = 0
            Dim resaImpiantoTot As Double = 0

            If supAbbattuta > supTotale Then
                'Se sono state registrate N operazioni di abbattimento e la somma della superfici abbattute > sup originale impianto
                'Imposto supAbbattura = supTotale
                supAbbattuta = supTotale
                row.Item("Sup_Abbattuta") = supAbbattuta
            End If

            If rilievoDanni > 100 Then
                'Sono state registrate n operazioni di Rilievo Danni e la somma > 100%
                'La imposto a 100%
                rilievoDanni = 100
                row.Item("Perc_Piante_Morte") = rilievoDanni
            End If

            If supAbbattuta > 0 Then
                'Ricalcolo la superficie sottraendo la superficie abbattuta
                supPostAbbattimento -= supAbbattuta
            End If

            If rilievoDanni > 0 Then
                'Ricalcolo la stima di produzione rimuovendo la percentuale di danno
                produzionePostRilievoDanni = produzionePrevista * ((100 - rilievoDanni) / 100)
            End If

            StimaProduzione = supPostAbbattimento * produzionePostRilievoDanni
            resaImpiantoTot = supPostAbbattimento * produzionePrevista

            row.Item("Stime_Produzione") = StimaProduzione
            row.Item("Resa_Impianto_Tot") = resaImpiantoTot

            If dtRilievi.Rows.Count > 0 Then
                Dim r As DataRow = dtRilievi.Select("Progetto_Cod = " & row.Item("Progetto_Cod")).FirstOrDefault()

                If r IsNot Nothing Then
                    Dim resaRilievo As Double = r.Item("Resa_Rilievo") * 100
                    row.Item("Resa_Ultimo_Rilievo") = resaRilievo
                    row.Item("Data_Ultimo_Rilievo") = r.Item("Data_Rilievo")
                    row.Item("Utente_Ultimo_Rilievo") = r.Item("Utente_Rilievo")
                    row.Item("Stime_Produzione_Rilievo") = supPostAbbattimento * resaRilievo
                End If
            End If
        Next

    End Sub

    Public Function CalcolaStimeProduzione_Report(chiavi As List(Of Integer),
                                                dataReport As Date,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Dim statistiche As New AgronicaCoreContabDAL.Statistiche_R
        Dim dt = statistiche.Leggi_Report_Stime_Produzione_NEW(chiavi, "", objParametri_Server, dataReport)
        CalcolaStimeProduzione(chiavi, dt, dataReport, objParametri_Server, objParametri_Utenti)

        Return dt

    End Function

End Class
