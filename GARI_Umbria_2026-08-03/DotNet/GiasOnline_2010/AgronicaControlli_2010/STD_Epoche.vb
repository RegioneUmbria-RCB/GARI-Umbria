Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD

Public Class STD_Epoche

    Public Function LeggiEpocheDPI(lavorazione As attivita.Lavorazione,
                                   disciplinare As metaschema.Disciplinare,
                                   modulo As Integer,
                                   objParametri_Super_Server As AgronicaCoreParametri,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri) As List(Of metaschema.Epoca)

        Dim epocheDPIList As New List(Of metaschema.Epoca)

        Dim strErr As String = ""
        Dim DtRisultati As DataTable

        If lavorazione IsNot Nothing AndAlso lavorazione.primaryKey IsNot Nothing Then

            'DT: per indicazione delle banche dati (21/12/2022) non vengono caricate le epoche per disseccamento e trattamento fitoregolatore
            If lavorazione.primaryKey.codice <> LAVCOD_DISSECCAMENTO AndAlso lavorazione.primaryKey.codice <> LAVCOD_TRATTAMENTO_FITOREGOLATORE Then

                Dim tipoTestata As Integer = STD_Utility.getTipoTestata(lavorazione.primaryKey.codice)

                If tipoTestata = enum_Disciplinare_Tipo_Testata.Difesa OrElse tipoTestata = enum_Disciplinare_Tipo_Testata.Diserbo Then

                    Dim Dpi_Cod = 0
                    Dim Id_Rcdpi As Integer = 0
                    If disciplinare IsNot Nothing Then
                        Dpi_Cod = STD_Utility.getDpiCod(disciplinare)
                        If disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                            Id_Rcdpi = disciplinare.raggruppamentiColturaliDPI.codice
                        End If
                    End If

                    'Richiamo caricamento epocheDPI da banche dati
                    DtRisultati = AgronicaCoreWebService.Epoche_WS.EpocheDPI_Elenco(Dpi_Cod:=Dpi_Cod,
                                                                                       Id_Rcdpi:=Id_Rcdpi,
                                                                                       Tipo_Testata:=tipoTestata,
                                                                                       Modulo:=modulo,
                                                                                       objParametri_Super_Server,
                                                                                       objParametri_Server,
                                                                                       objParametri_Utenti,
                                                                                       strErr)

                    If strErr = "" Then
                        If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then

                            For i = 0 To DtRisultati.Rows.Count - 1
                                Dim epocaDPI = New AgronicaCoreModelsSTD.metaschema.Epoca With {
                                    .codice = DtRisultati.Rows(i).Item("codice"),
                                    .descrizione = DtRisultati.Rows(i).Item("descrizione")
                                }
                                epocheDPIList.Add(epocaDPI)
                            Next

                        End If
                    End If
                End If
            End If
        End If

        Return epocheDPIList

    End Function

    Public Function LeggiEpocheFertilizzazione(specie As metaschema.utilizzi.Specie,
                                               disciplinare As metaschema.Disciplinare,
                                               epocaCodice As Integer,
                                               objParametri_Super_Server As AgronicaCoreParametri,
                                               objParametri_Server As AgronicaCoreParametri
                                                ) As List(Of metaschema.Epoca)

        Dim epocheFertilizzazioneList As New List(Of metaschema.Epoca)

        Dim Veg_Cod As String = "0" 'Destinazione d'uso
        If specie IsNot Nothing AndAlso specie.codice > 0 Then
            Veg_Cod = specie.codice
        End If

        'DT: per regolamenti <= 0, dobbiamo ignorarli perchè non hanno epoche associate
        Dim Regolamento_Cod = 0
        If disciplinare IsNot Nothing AndAlso disciplinare.regolamentoConcimazione IsNot Nothing AndAlso disciplinare.regolamentoConcimazione.codice > 0 Then
            Regolamento_Cod = disciplinare.regolamentoConcimazione.codice
        End If

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)

        If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
        End If

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_input
        objParametriIngresso.Epoca_Cod = epocaCodice
        objParametriIngresso.Specie_Cod = Veg_Cod
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output

        'Richiamo caricamento epocheFertilizzazione da banche dati
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.EpocheModalitaxSpecie(objParametriIngresso)

        For i = 0 To objParametriUscita.ListaEpocheModalitaxSpecie.Count - 1
            Dim epocaFertilizzazione = New AgronicaCoreModelsSTD.metaschema.Epoca With {
                        .codice = objParametriUscita.ListaEpocheModalitaxSpecie(i).Epoca_Cod,
                        .descrizione = objParametriUscita.ListaEpocheModalitaxSpecie(i).Epoca_Des
                    }
            epocheFertilizzazioneList.Add(epocaFertilizzazione)

        Next

        Return epocheFertilizzazioneList

    End Function

End Class
