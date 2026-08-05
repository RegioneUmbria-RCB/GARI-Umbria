Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreAnagrafeStdBIZ
Imports AgronicaCoreContabStdDAL
Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreDataProviderSTD
Imports Newtonsoft.Json

Public Class Visite

    ''' <summary>
    ''' Lettura lista visite con filtro ricerca
    ''' </summary>
    ''' <param name="Ricerca"></param>
    Public Function EstraiListaVisite(dbContext As GiasDbContext, Ricerca As String) As List(Of AgronicaCoreModelloSTD.Visite)

        Dim leggiVisite As New Visite_R
        Dim leggiVisiteDettagli As New Visite_Dettagli_R
        Dim leggiVisiteDestinazioni As New Visite_Destinazioni_R

        Dim letturaCentri As New CentriAziendali_R
        Dim letturaImpianti As New Reg_Impianti_R
        Dim letturaAttivita As New Attivita_R
        Dim letturaSpecie As New SpecieVegetali_R
        Dim leggiDocumenti As New Documenti(dbContext)

        Dim listaVisite As New List(Of AgronicaCoreModelloSTD.Visite)
        Dim listaVisiteApp = leggiVisite.Leggi(dbContext, 0)
        Dim listaCentri = letturaCentri.EstraiListaCentriAziendali(dbContext, "")
        Dim listaAttivita = letturaAttivita.EstraiListaAttivitaFiltrataPerOperazione(dbContext, "", LAVCOD_VISITA)

        For Each visitaAPP In listaVisiteApp

            Dim dettaglio = leggiVisiteDettagli.Leggi(dbContext, visitaAPP.Visita_Cod).FirstOrDefault
            Dim impianti = letturaImpianti.LeggiDatoVisitaCod(dbContext, visitaAPP.Visita_Cod)

            Dim centro = listaCentri.Where(Function(i) i.Piva = visitaAPP.Piva AndAlso i.Sa_Cod = visitaAPP.Sa_Cod).FirstOrDefault
            Dim attivita = listaAttivita.Where(Function(i) i.Cod = dettaglio.Id_Attivita).FirstOrDefault
            Dim documento = leggiDocumenti.EstraiListaDocumenti("", visitaAPP.Visita_Cod).FirstOrDefault

            Dim visita As New AgronicaCoreModelloSTD.Visite With {
                .Visita_Cod = visitaAPP.Visita_Cod,
                .TipoOperazioneDB = enum_TipoOperazioneDB.Lettura,
                .Tipo_Visita = enum_TipoVisita_DB.Standard,
                .Data_Visita = visitaAPP.Data_Visita,
                .Operatore = visitaAPP.Operatore,
                .Centro_Aziendale = centro,
                .Documento = documento 'If(documento Is Nothing, documento, New AgronicaCoreModelloSTD.Documento)
            }

            If Not String.IsNullOrEmpty(visitaAPP.Posizione) Then
                visita.Posizione = visitaAPP.Posizione
                visita.DescrizionePosizione = "Lat/Lon: " & visitaAPP.Posizione.Replace("|", ", ")
            End If

            visita.Visita_Dettaglio_Cod = dettaglio.Visita_Dettaglio_Cod
            visita.Attivita = attivita
            visita.Descrizione = dettaglio.Descrizione

            If impianti.Count > 0 Then
                Dim impianto = impianti.FirstOrDefault
                If impianto.veg_cod = 0 Then
                    visita.Specie = New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni With {.cod = "0/" & impianto.id_cod, .veg_des = impianto.codici_anagrafe_des}
                Else
                    visita.Specie = New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni With {.cod = CStr(impianto.veg_cod), .veg_des = impianto.veg_des}
                End If
                visita.DescrizioneImpianti = visita.Specie.veg_des & ": " & String.Join(" | ", (From iii In impianti Select iii.app_nome).ToArray)
                visita.Impianti = impianti
            Else
                visita.Impianti = New List(Of APP_Reg_Impianti)
            End If

            listaVisite.Add(visita)

        Next

        ' filtro ricerca libero
        If Not String.IsNullOrEmpty(Ricerca) Then
            listaVisite = listaVisite.Where(Function(i) String.IsNullOrEmpty(Ricerca) OrElse i.TestoRicerca.ToLower.Contains(Ricerca.ToLower)).ToList()
        End If

        Return listaVisite

    End Function

    Public Function LeggiVisiteDaInviare(dbContext As GiasDbContext, piva As String) As AgronicaCoreModelloSTD.VisitePerScarico

        Dim VisitaLetta As New AgronicaCoreModelloSTD.VisitePerScarico

        Dim visiteR As New Visite_R
        Dim visiteDettagliR As New Visite_Dettagli_R
        Dim visiteDestinazioniR As New Visite_Destinazioni_R

        VisitaLetta.Visite = visiteR.LeggiPerRicaricoDati(dbContext, piva)
        VisitaLetta.VisiteDettagli = visiteDettagliR.LeggiPerRicaricoDati(dbContext, piva)
        VisitaLetta.VisiteDestinazioni = visiteDestinazioniR.LeggiPerRicaricoDati(dbContext, piva)

        Dim documentiBIZ = New Documenti(dbContext)
        VisitaLetta.VisiteDocumenti = documentiBIZ.LeggiDocumentiDaInviare(piva, True)

        Return VisitaLetta

    End Function

    Public Sub CancellaVisiteInviate(dbContext As GiasDbContext, visiteDaCancellare As List(Of APP_Visite))
        For Each visita In visiteDaCancellare
            CancellaVisita(dbContext, visita.Visita_Cod)
        Next
    End Sub

    ''' <summary>
    ''' Scrittura di una nuova visita
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Visita"></param>
    Public Sub Scrivi(dbContext As GiasDbContext, Visita As AgronicaCoreModelloSTD.Visite, username As String, utente As String)

        Dim agroSequenze As New AgronicaCoreDataProviderSTD.Agro_Sequenze

        Dim VisitaCodPrecedente As Integer = Visita.Visita_Cod

        If Visita.Visita_Cod = 0 Then
            Visita.Visita_Cod = agroSequenze.NuovoId_Tabella_EF(dbContext, "visite", AgroSequenzeBase0, AgroSequenzeEndUpperBound)
            Visita.Visita_Dettaglio_Cod = agroSequenze.NuovoId_Tabella_EF(dbContext, "visite_dettagli", AgroSequenzeBase0, AgroSequenzeEndUpperBound)
            dbContext.SaveChanges()
        End If

        Dim APP_Visita As New APP_Visite With {
            .Visita_Cod = Visita.Visita_Cod,
            .Tipo_Visita = Visita.Tipo_Visita,
            .Piva = Visita.Centro_Aziendale.Piva,
            .Sa_Cod = Visita.Centro_Aziendale.Sa_Cod,
            .Data_Visita = Visita.Data_Visita,
            .Operatore = Visita.Operatore,
            .Posizione = Visita.Posizione,
            .Lav_Cod = LAVCOD_VISITA
        }

        Dim APP_Visita_Dettaglio As New APP_Visite_Dettagli With {
            .Visita_Cod = Visita.Visita_Cod,
            .Visita_Dettaglio_Cod = Visita.Visita_Dettaglio_Cod,
            .Id_Attivita = Visita.Attivita.Cod,
            .Descrizione = Visita.Descrizione
        }

        Dim listaDettagli As New List(Of APP_Visite_Dettagli)
        listaDettagli.Add(APP_Visita_Dettaglio)

        Dim listaDestinazioni As New List(Of APP_Visite_Destinazioni)
        For Each impianto In Visita.Impianti

            ' verificare se mantenere numerazione in modifica e segno
            Dim Visita_Destinazione_Cod = agroSequenze.NuovoId_Tabella_EF(dbContext, "visite_destinazioni", AgroSequenzeBase0, AgroSequenzeEndUpperBound)
            dbContext.SaveChanges()

            Dim APP_Visita_Destinazione As New APP_Visite_Destinazioni With {
                .Visita_Cod = Visita.Visita_Cod,
                .Visita_Destinazione_Cod = Visita_Destinazione_Cod,
                .Piva = impianto.piva,
                .Sa_Cod = impianto.sa_cod,
                .Appezza = impianto.appezza,
                .Id_Reg = impianto.id_reg,
                .Qta2 = impianto.sup_imp
            }
            listaDestinazioni.Add(APP_Visita_Destinazione)

        Next

        Dim APP_Documento As APP_Documenti = Nothing
        If Visita.Documento IsNot Nothing AndAlso Visita.Documento.Allegati IsNot Nothing Then
            If Visita.Documento.Allegati.Count > 0 OrElse Visita.Documento.Documento_Cod <> 0 Then
                APP_Documento = New APP_Documenti With {
                    .Piva_Superuser = Visita.Documento.Piva_Superuser,
                    .Piva = Visita.Documento.Piva,
                    .Documento_Cod = Visita.Documento.Documento_Cod,
                    .ID_Tipologia = Visita.Documento.ID_Tipologia,
                    .Data_Creazione = Visita.Data_Visita,
                    .Data_Scadenza = Visita.Documento.Data_Scadenza,
                    .Descrizione = Visita.Documento.Descrizione,
                    .Note = Visita.Documento.Note,
                    .Visita_Cod = Visita.Visita_Cod
                }
                If Visita.Documento.Allegati.Count > 0 Then
                    APP_Documento.Allegati = JsonConvert.SerializeObject(Visita.Documento.Allegati)
                End If
            End If
        End If

        Using transaction = dbContext.Database.BeginTransaction()

            Try

                If Visita.TipoOperazioneDB = AgronicaCoreDataProviderSTD.TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then
                    CancellaVisita(dbContext, VisitaCodPrecedente)
                End If

                ScritturaDatiComuni(APP_Visita, username)
                dbContext.APP_Visite.Add(APP_Visita)
                dbContext.SaveChanges()

                For Each dettaglioDaScrivere In listaDettagli
                    ScritturaDatiComuni(dettaglioDaScrivere, username)
                    dbContext.Add(dettaglioDaScrivere)
                Next
                dbContext.SaveChanges()

                For Each destinazioneDaScrivere In listaDestinazioni
                    ScritturaDatiComuni(destinazioneDaScrivere, username)
                    dbContext.Add(destinazioneDaScrivere)
                Next
                dbContext.SaveChanges()

                If APP_Documento IsNot Nothing Then
                    Dim scriviDocumenti As New Documenti(dbContext)
                    scriviDocumenti.ScriviDocumento(APP_Documento, username)
                End If

                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try
        End Using

    End Sub

    Public Sub ScritturaDatiComuni(oggettoDoveScrivere As APP_Agronica_Entity, username As String)


        Dim dataora As Date = System.DateTime.Now()

        oggettoDoveScrivere.Data_Creazione = dataora
        oggettoDoveScrivere.Data_Modifica = dataora
        oggettoDoveScrivere.Username_Creazione = username
        oggettoDoveScrivere.Username_Modifica = username

        'valori non memorizzati danno luogo a data "01/01/0001", quindi allineo su agro-data.inizio/fine

        If oggettoDoveScrivere.Validita_Inizio < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Inizio = AGRODATAINIZIO
        End If

        If oggettoDoveScrivere.Validita_Fine < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Fine = AGRODATAFINE
        End If

    End Sub

    Public Sub CancellaVisitaConTransazione(dbContext As GiasDbContext, VisitaCod As Integer)

        Using transaction = dbContext.Database.BeginTransaction()

            Try

                CancellaVisita(dbContext, VisitaCod)
                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try

        End Using

    End Sub

    Public Sub CancellaVisita(dbContext As GiasDbContext, VisitaCod As Integer)

        Try

            Dim visiteW As New Visite_W
            Dim visiteDettagliW As New Visite_Dettagli_W
            Dim visiteDestinazioniW As New Visite_Destinazioni_W

            visiteDestinazioniW.CancellaDaVisitaCod(dbContext, VisitaCod)
            visiteDettagliW.CancellaDaVisitaCod(dbContext, VisitaCod)
            visiteW.CancellaDocumentoVisita(dbContext, VisitaCod)
            visiteW.CancellaDaVisitaCod(dbContext, VisitaCod)

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

End Class
