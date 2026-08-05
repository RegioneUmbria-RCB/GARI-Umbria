Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate

Public Class Ricette_Operazioni


    Public Function EstraiListaOperazioni(dbContext As GiasDbContext, tipoRicetta As enum_TipoRicetta_APP, stato As enum_WWorflow_WAnagraficaStati, piva As String, ricerca As String) As List(Of AgronicaCoreModelloSTD.Ricette_Operazioni)

        Dim leggiOperazioni As New AgronicaCoreContabStdDAL.Ricette_Operazioni_R
        Dim ricetteOperazioni As List(Of APP_Ricette_Operazioni) =
            leggiOperazioni.LeggiPerLista(dbContext, tipoRicetta, stato, piva)

        If ricetteOperazioni.Count = 0 Then
            Return New List(Of AgronicaCoreModelloSTD.Ricette_Operazioni)
        End If

        Dim rval As List(Of AgronicaCoreModelloSTD.Ricette_Operazioni) = (
            From o In ricetteOperazioni
            Select New AgronicaCoreModelloSTD.Ricette_Operazioni With {
                .Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod,
                    .Ricetta_Operazione_Des = o.Ricetta_Operazione_Des,
                    .Bozza = o.Bozza,
                    .Note = o.Note,
                    .DataRiferimento = o.Validita_Inizio,
                    .Operazione = New AgronicaCoreModelloSTD.Operazione With {.lav_cod = o.Lav_Cod}
                    }).ToList()

        Dim letturaImpianti As New AgronicaCoreAnagrafeStdDAL.Reg_Impianti_R
        Dim letturaOperazione As New AgronicaCoreAnagrafeStdBIZ.Operazioni(dbContext)

        For Each iOp As AgronicaCoreModelloSTD.Ricette_Operazioni In rval

            ' descrizione operazione
            iOp.Operazione.lav_des = letturaOperazione.LeggiOperazione(iOp.Operazione.lav_cod).lav_des

            ' descrizione da lista impianti
            Dim listaImpianti As List(Of APP_Reg_Impianti) = letturaImpianti.LeggiDatoRicettaOperazioneCod(dbContext, iOp.Ricetta_Operazione_Cod)
            'Dim listaIpiantiCheck As New List(Of APP_Reg_Impianti)
            'For Each cImp In listaImpianti
            '    If listaIpiantiCheck.Where(Function(i) i.piva = cImp.piva And i.sa_cod = cImp.sa_cod And i.appezza = cImp.appezza And i.id_reg = cImp.id_reg).Count = 0 Then
            '        listaIpiantiCheck.Add(cImp)
            '    End If
            'Next

            iOp.DescrizioneImpianti = String.Join(" | ", (From iii In listaImpianti Select iii.app_nome).ToArray)

            ' centro e specie
            If listaImpianti.Count > 0 Then
                Dim iFirst As APP_Reg_Impianti = listaImpianti.FirstOrDefault
                iOp.CentroAziendale = New AgronicaCoreModelloSTD.Centri_Aziendali With {.Piva = iFirst.piva, .Sa_Cod = iFirst.sa_cod, .Rag_Soc = iFirst.rag_soc, .Sa_Nome = iFirst.sa_nome}
                If iFirst.veg_cod = 0 Then
                    iOp.Specie = New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni With {.cod = "0/" & iFirst.id_cod, .veg_des = iFirst.codici_anagrafe_des}
                Else
                    iOp.Specie = New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni With {.cod = CStr(iFirst.veg_cod), .veg_des = iFirst.veg_des}
                End If
            End If

        Next

        ' filtro ricerca libero
        If Not String.IsNullOrEmpty(ricerca) Then
            rval = rval.Where(Function(i) String.IsNullOrEmpty(ricerca) OrElse i.TestoRicerca.ToLower.Contains(ricerca.ToLower)).ToList()
        End If

        Return rval

    End Function


    Public Shared Sub LeggiOperazione(OperazioneDaLeggere As AgronicaCoreModelloSTD.Ricette_Operazioni, App_Operazione As APP_Ricette_Operazioni)

        'chiavi..
        OperazioneDaLeggere.Ricetta_Operazione_Cod = App_Operazione.Ricetta_Operazione_Cod

        'dati
        OperazioneDaLeggere.Operazione = New AgronicaCoreModelloSTD.Operazione With {.lav_cod = App_Operazione.Lav_Cod}
        OperazioneDaLeggere.Ricetta_Operazione_Des = App_Operazione.Ricetta_Operazione_Des
        OperazioneDaLeggere.W_Anagrafica_Stati_Cod = App_Operazione.W_Anagrafica_Stati_Cod
        OperazioneDaLeggere.Bozza = App_Operazione.Bozza
        OperazioneDaLeggere.Note = App_Operazione.Note

        OperazioneDaLeggere.Epoca = App_Operazione.Extra_Int

        OperazioneDaLeggere.DataRiferimento = App_Operazione.Validita_Inizio

        If App_Operazione.Ricetta_Operazione_Cod_RIF <> 0 Then
            OperazioneDaLeggere.Ricetta_Operazione_Rif = New AgronicaCoreModelloSTD.Ricette_Operazioni With {.Ricetta_Operazione_Cod = App_Operazione.Ricetta_Operazione_Cod_RIF}
        End If

    End Sub


    Public Shared Sub ScriviOperazione(Ricetta_cod As Integer, OperazioneDaScrivere As AgronicaCoreModelloSTD.Ricette_Operazioni, App_Operazione As APP_Ricette_Operazioni, OperazioneConfig As AgronicaCoreModelloSTD.FormOperazioneConfig)

        App_Operazione.Ricetta_Cod = Ricetta_cod
        App_Operazione.Ricetta_Operazione_Des = OperazioneDaScrivere.Ricetta_Operazione_Des
        App_Operazione.Ricetta_Operazione_Cod = OperazioneDaScrivere.Ricetta_Operazione_Cod
        App_Operazione.Lav_Cod = OperazioneDaScrivere.Operazione.lav_cod


        If OperazioneDaScrivere.Ricetta_Operazione_Rif IsNot Nothing Then

            App_Operazione.Ricetta_Operazione_Cod_RIF =
                OperazioneDaScrivere.Ricetta_Operazione_Rif.Ricetta_Operazione_Cod

        End If


        If OperazioneDaScrivere.W_Anagrafica_Stati_Cod = 0 Then
            OperazioneDaScrivere.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
        End If

        App_Operazione.W_Anagrafica_Stati_Cod = OperazioneDaScrivere.W_Anagrafica_Stati_Cod
        App_Operazione.Bozza = OperazioneDaScrivere.Bozza
        App_Operazione.Note = OperazioneDaScrivere.Note
        App_Operazione.Extra_Int = OperazioneDaScrivere.Epoca

        ' forzo a 1 il campo Mezzo su concimazioni trattamenti/diserbi e semine (Mail Grillo del 26/03/2019)
        If TypeOf OperazioneConfig.OggettoDettaglioPerTipo IsNot AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni Then
            App_Operazione.Mezzo = enum_TipoMezzo.Ettaro
        End If

        ' forzo a 10 il campo Extra_Int per la raccolta (Mail Grillo del 07/05/2019)
        If OperazioneDaScrivere.Operazione.lav_cod = LAVCOD_RACCOLTA Then
            App_Operazione.Extra_Int = 10
        End If

        App_Operazione.Validita_Inizio = OperazioneDaScrivere.DataRiferimento

    End Sub



End Class
