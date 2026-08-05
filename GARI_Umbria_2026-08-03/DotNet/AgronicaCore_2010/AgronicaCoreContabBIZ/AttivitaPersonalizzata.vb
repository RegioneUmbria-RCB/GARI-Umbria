Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.attivita

Public Class AttivitaPersonalizzata

    Public Function LeggiAttivitaPersonalizzata(ByVal id_attivita As Integer, ByVal lav_cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri, Optional verbose As Boolean = False) As List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata)

        Dim attivitaPersonalizzataList As New List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata)

        Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R

        Dim axo_R As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
        Dim Dt As DataTable = axo_R.Leggi(id_attivita, lav_cod, "", "", objParametri_Server)

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            For Each dr As DataRow In Dt.Rows

                Dim attivitaPers As New AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata
                attivitaPers.codice = dr.Item("Id_Attivita")
                attivitaPers.descrizione = dr.Item("Desc")

                If lav_cod = CostantiPersonalizzate.LAVCOD_VISITA Then

                    Dim att_R As New AgronicaCoreContabDAL.Attivita_R
                    Dim DtLavCod As DataTable = att_R.Leggi(dr.Item("Id_Attivita"), "", "", objParametri_Server, loadLavCodAssociatiVisite:=True)

                    If DtLavCod IsNot Nothing AndAlso DtLavCod.Rows.Count > 0 Then
                        For Each drLavCod As DataRow In DtLavCod.Rows

                            If drLavCod("LAV_COD").ToString() <> "0" Then
                                Dim operazione As New AgronicaCoreModelsSTD.attivita.Lavorazione With {
                                    .tipo = TipiJob.LAVORAZIONE,
                                    .primaryKey = New Job.PK("Lavorazione", drLavCod("LAV_COD").ToString())
                                }

                                If verbose Then

                                    Dim rOperazioni = New AgronicaCoreAnagrafeDAL.Operazioni_R
                                    operazione.descrizione = rOperazioni.Lav_Des_From_Lav_Cod(drLavCod("LAV_COD").ToString(), objParametri_Server)

                                End If

                                attivitaPers.operazioni.Add(operazione)
                            End If

                        Next
                    End If

                End If

                attivitaPersonalizzataList.Add(attivitaPers)

            Next
        End If

        Return attivitaPersonalizzataList

    End Function

End Class
