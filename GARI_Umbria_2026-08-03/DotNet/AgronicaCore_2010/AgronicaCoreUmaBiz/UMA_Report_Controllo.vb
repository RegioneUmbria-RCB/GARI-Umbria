Imports AgronicaCoreDataProvider
Imports AgronicaCoreUmaDal
Imports Newtonsoft.Json.Linq

Public Class UMA_Report_Controllo

    Public Function SegnalaSelezionati(selezionate As String, anno As Integer, data As String, ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim rec_Acc_DAL_R As New AgronicaCoreUmaDal.UMA_Richieste_Rec_Acc_R
        Dim rec_Acc_DAL_W As New AgronicaCoreUmaDal.UMA_Richieste_Rec_Acc_W

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Report_Controllo.SegnalaSelezionati()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim righe As JArray
        Dim dtTrovato As New DataTable
        Dim listaNuovi As New List(Of UMA_Rec_Acc)
        Dim listaAggiornamenti As New List(Of UMA_Rec_Acc)
        Dim dataSegnalazione As DateTime = Date.ParseExact(data, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)

        righe = JArray.Parse(selezionate)

        For Each riga In righe
            Dim newRecAcc As New UMA_Rec_Acc With {
                .Piva = riga.Item("Piva"),
                .Richiesta_Cod = riga.Item("Richiesta_Cod"),
                .Rec_Acc_Anno = anno,
                .Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO,
                .Validita_Fine = CostantiPersonalizzate.AGRODATAFINE,
                .Inviato = 0,
                .Data_Invio = Nothing,
                .Data_Creazione = DateTime.Now,
                .Data_modifica = DateTime.Now,
                .Username_creazione = objParametri.UtenteUsername,
                .Username_Modifica = objParametri.UtenteUsername,
                .Rec_acc_Gasolio = riga.Item("RecAccGas"),
                .Rec_acc_Benzina = riga.Item("RecAccBenz"),
                .Rec_acc_Gasolio_Serra = riga.Item("RecAccSerra"),
                .Rec_acc_Data = dataSegnalazione,
                .Rec_acc_Utente = objParametri.UtenteUsername,
                .Rec_acc_Causale = riga.Item("Causale"),
                .Doc_Manc = OttieniDocManc(riga)
            }

            dtTrovato = rec_Acc_DAL_R.LeggiRecAcc(newRecAcc.Piva,
                                                  newRecAcc.Richiesta_Cod,
                                                  newRecAcc.Rec_Acc_Anno,
                                                  newRecAcc.Doc_Manc,
                                                  "",
                                                  "",
                                                  objParametri)

            If dtTrovato.Rows.Count > 0 Then
                listaAggiornamenti.Add(newRecAcc)
            Else
                listaNuovi.Add(newRecAcc)
            End If

        Next

        If listaAggiornamenti.Count > 0 Then
            rec_Acc_DAL_W.AggiornaEsistenti(listaAggiornamenti, objParametri)
        End If

        If listaNuovi.Count > 0 Then
            rec_Acc_DAL_W.AggiungiNuovi(listaNuovi, objParametri)
        End If

        Return True

    End Function

    Public Function AnnullaSegnalazioneSelezionati(selezionate As String, anno As Integer, ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim rec_Acc_DAL_W As New AgronicaCoreUmaDal.UMA_Richieste_Rec_Acc_W

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Report_Controllo.SegnalaSelezionati()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim righe As JArray
        Dim dtTrovato As New DataTable
        Dim listaAnnulli As New List(Of UMA_Rec_Acc)

        righe = JArray.Parse(selezionate)

        For Each riga In righe
            Dim newRecAcc As New UMA_Rec_Acc With {
                .Piva = riga.Item("Piva"),
                .Richiesta_Cod = riga.Item("Richiesta_Cod"),
                .Rec_Acc_Anno = anno,
                .Validita_Inizio = CostantiPersonalizzate.AGRODATAINIZIO,
                .Validita_Fine = CostantiPersonalizzate.AGRODATAFINE,
                .Inviato = 0,
                .Data_Invio = Nothing,
                .Data_Creazione = DateTime.Now,
                .Data_modifica = DateTime.Now,
                .Username_creazione = objParametri.UtenteUsername,
                .Username_Modifica = objParametri.UtenteUsername,
                .Rec_acc_Gasolio = riga.Item("RecAccGas"),
                .Rec_acc_Benzina = riga.Item("RecAccBenz"),
                .Rec_acc_Gasolio_Serra = riga.Item("RecAccSerra"),
                .Rec_acc_Utente = objParametri.UtenteUsername,
                .Rec_acc_Causale = riga.Item("Causale"),
                .Doc_Manc = OttieniDocManc(riga)
            }

            listaAnnulli.Add(newRecAcc)

        Next

        If listaAnnulli.Count > 0 Then
            rec_Acc_DAL_W.AnnullaSegnalazioni(listaAnnulli, objParametri)
        End If

        Return True

    End Function

    Private Shared Function OttieniDocManc(riga As JToken) As Integer
        Return If(riga.Item("Causale") = "4" OrElse riga.Item("Causale") = "7", 1, 0)
    End Function
End Class
