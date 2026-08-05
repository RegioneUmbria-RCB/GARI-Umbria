
Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate

Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Ricette_Dettagli_Prodotti


    Public Shared Sub LeggiDettagli(dbContext As GiasDbContext, piva As String, DettaglioDaLeggere As AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti, App_Dettagli As APP_Ricette_Dettagli, APP_DettaglioMagazzino As APP_Ricette_Dettagli, App_Destinazioni As List(Of APP_Ricette_Destinazioni))


        'dati
        Dim ProdottoCod As Integer = 0

        If App_Dettagli.Mat_Cod = 0 Then
            ProdottoCod = App_Dettagli.Pro_Cod
        Else
            ProdottoCod = -App_Dettagli.Mat_Cod
        End If

        'lettura con descrizione
        If DettaglioDaLeggere.Prodotto Is Nothing Then

            Dim letturaProdotto As New Prodotti_R
            DettaglioDaLeggere.Prodotto =
                letturaProdotto.Leggi(dbContext, piva, App_Dettagli.Elem_Cod, ProdottoCod)

        End If

        DettaglioDaLeggere.Dose_Ha_Reale = App_Dettagli.Qta
        DettaglioDaLeggere.Dose_Hl_Reale = App_Dettagli.Qta_Extra
        DettaglioDaLeggere.Dose_Totale_Reale = App_Dettagli.Qta_Extra_Totale

        DettaglioDaLeggere.Ha_Hl = App_Dettagli.Mezzo_Det
        DettaglioDaLeggere.Dose_QtaTotale = App_Dettagli.Udm_Cod_Extra


        'lettura unità di misura
        Dim letturaUdm As New AgronicaCoreAnagrafeStdDAL.UnitaMisura_R

        Dim udmDes1 As AgronicaCoreModelloSTD.UnitaMisura = letturaUdm.Leggi(dbContext, App_Dettagli.Udm_Cod)
        Dim udmDes2 As AgronicaCoreModelloSTD.UnitaMisura = letturaUdm.Leggi(dbContext, App_Dettagli.Extra_Int)

        If udmDes1 Is Nothing Then
            DettaglioDaLeggere.Udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = App_Dettagli.Udm_Cod}
        Else
            DettaglioDaLeggere.Udm = udmDes1
        End If


        If udmDes2 Is Nothing Then
            DettaglioDaLeggere.Udm_Indicata = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = App_Dettagli.Extra_Int}
        Else
            DettaglioDaLeggere.Udm_Indicata = udmDes2
        End If

        'parte di carico/scarico di magazzino
        If App_Destinazioni.Count > 0 AndAlso Not APP_DettaglioMagazzino Is Nothing Then

            Dim xDestinazione = App_Destinazioni.FirstOrDefault

            Dim leggiXMovimenti As New Prodotti_Giacenze_R
            'leggiXMovimenti.Leggi(dbContext, piva)

            Dim giacenzaPerDescrizioni As APP_Prodotti_Giacenze =
                leggiXMovimenti.Leggi(
                    dbContext,
                    xDestinazione.Piva,
                    xDestinazione.Sa_Cod,
                    20,
                    xDestinazione.Id_Reg,
                    APP_DettaglioMagazzino.Elem_Cod,
                    DettaglioDaLeggere.Prodotto.Prodotto_Cod,
                    APP_DettaglioMagazzino.Lotto,
                    0,
                    0,
                    DettaglioDaLeggere.Udm.udm_cod
                )

            If giacenzaPerDescrizioni IsNot Nothing Then

                DettaglioDaLeggere.MagazziniMovimentazioni = New AgronicaCoreModelloSTD.RilevamentoDiMagazzino With {
                .Lotto = APP_DettaglioMagazzino.Lotto,
                .Cal_Cod = giacenzaPerDescrizioni.Cal_Cod,
                .Cod_Progetto = giacenzaPerDescrizioni.Cod_Progetto,
                .Magazzino = New AgronicaCoreModelloSTD.Fabbricato With {
                        .Piva = giacenzaPerDescrizioni.Piva,
                        .Sa_Cod = giacenzaPerDescrizioni.Sa_Cod,
                        .Tipo_Destinazione = giacenzaPerDescrizioni.Tipo_Destinazione,
                        .Fabbricato_Cod = giacenzaPerDescrizioni.Fabbricato_Cod,
                        .Fabbricato_Des = giacenzaPerDescrizioni.Fabbricato_Des
                    },
                .udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = DettaglioDaLeggere.Udm.udm_cod, .udm_des = DettaglioDaLeggere.Udm.udm_des},
                .Qta = APP_DettaglioMagazzino.Qta,
                .Prodotto = New AgronicaCoreModelloSTD.Prodotto With {
                    .Elem_Cod = APP_DettaglioMagazzino.Elem_Cod,
                    .Prodotto_Cod = DettaglioDaLeggere.Prodotto.Prodotto_Cod,
                    .Prodotto_Des = DettaglioDaLeggere.Prodotto.Prodotto_Des,
                    .Udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = DettaglioDaLeggere.Udm.udm_cod, .udm_des = DettaglioDaLeggere.Udm.udm_des}
                    },
                .TipoRilevamento = AgronicaCoreDataProviderSTD.TipiEnumerativi.enum_RilevamentoMagazzinoTipo.giacenza,
                .Descrizione = DettaglioDaLeggere.Prodotto.Prodotto_Des & "[" & giacenzaPerDescrizioni.Fabbricato_Des & " (" & giacenzaPerDescrizioni.Giacenza & " " & DettaglioDaLeggere.Udm.udm_des & ")]"
                }

            Else

                Dim leggiXMagazzini As New Magazzini_R
                Dim magazzino = leggiXMagazzini.LeggiMagazzino(dbContext, xDestinazione.Piva, xDestinazione.Sa_Cod, 20, xDestinazione.Id_Reg)

                If magazzino IsNot Nothing Then

                    DettaglioDaLeggere.MagazziniMovimentazioni = New AgronicaCoreModelloSTD.RilevamentoDiMagazzino With {
                        .Lotto = APP_DettaglioMagazzino.Lotto,
                        .Magazzino = New AgronicaCoreModelloSTD.Fabbricato With {
                                .Piva = xDestinazione.Piva,
                                .Sa_Cod = xDestinazione.Sa_Cod,
                                .Tipo_Destinazione = xDestinazione.Tipo_Destinazione,
                                .Fabbricato_Cod = xDestinazione.Id_Reg,
                                .Fabbricato_Des = magazzino.Ubic_Des
                            },
                        .udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = DettaglioDaLeggere.Udm.udm_cod, .udm_des = DettaglioDaLeggere.Udm.udm_des},
                        .Qta = APP_DettaglioMagazzino.Qta,
                        .Prodotto = New AgronicaCoreModelloSTD.Prodotto With {
                            .Elem_Cod = APP_DettaglioMagazzino.Elem_Cod,
                            .Prodotto_Cod = DettaglioDaLeggere.Prodotto.Prodotto_Cod,
                            .Prodotto_Des = DettaglioDaLeggere.Prodotto.Prodotto_Des,
                            .Udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = DettaglioDaLeggere.Udm.udm_cod, .udm_des = DettaglioDaLeggere.Udm.udm_des}
                            },
                        .TipoRilevamento = AgronicaCoreDataProviderSTD.TipiEnumerativi.enum_RilevamentoMagazzinoTipo.giacenza,
                        .Descrizione = DettaglioDaLeggere.Prodotto.Prodotto_Des & "[" & magazzino.Ubic_Des & "]"
                        }

                End If

            End If

        End If

    End Sub


    Public Shared Sub ScriviDettagli(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, DettagliDaScrivere As AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti, App_Dettagli As APP_Ricette_Dettagli)

        Ricette_Dettagli.ScriviDettagli(Ricetta_Cod, Ricetta_Operazione_Cod, DettagliDaScrivere, App_Dettagli)

        Dim xProCod As Integer = 0
        Dim xMatCod As Integer = 0

        If DettagliDaScrivere.Prodotto.Prodotto_Cod > 0 Then
            xProCod = DettagliDaScrivere.Prodotto.Prodotto_Cod
        Else
            xMatCod = -DettagliDaScrivere.Prodotto.Prodotto_Cod
        End If

        App_Dettagli.Elem_Cod = DettagliDaScrivere.Prodotto.Elem_Cod
        App_Dettagli.Pro_Cod = xProCod
        App_Dettagli.Mat_Cod = xMatCod

        App_Dettagli.Lotto = ""
        If DettagliDaScrivere.MagazziniMovimentazioni IsNot Nothing Then
            App_Dettagli.Lotto = DettagliDaScrivere.MagazziniMovimentazioni.Lotto
        End If

        App_Dettagli.Qta = DettagliDaScrivere.Dose_Ha_Reale
        App_Dettagli.Qta_Extra = DettagliDaScrivere.Dose_Hl_Reale
        App_Dettagli.Qta_Extra_Totale = DettagliDaScrivere.Dose_Totale_Reale
        App_Dettagli.Mezzo_Det = DettagliDaScrivere.Ha_Hl
        App_Dettagli.Udm_Cod_Extra = DettagliDaScrivere.Dose_QtaTotale

        App_Dettagli.Udm_Cod = DettagliDaScrivere.Udm.udm_cod
        App_Dettagli.Extra_Int = DettagliDaScrivere.Udm_Indicata.udm_cod


    End Sub

End Class
