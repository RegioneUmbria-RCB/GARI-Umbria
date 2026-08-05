Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.attivita.dettagli

Public Class STD_Farmaci
    Public Function LeggiFarmaci(piva As String,
                                 saCod As Integer,
                                 codiceBDN As String,
                                 codFiscaleProprietario As String,
                                 codiceAIC() As String,
                                 validitaFine As DateTime,
                                 tipoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita,
                                 statoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Stati,
                                 escludiGiacenzeZero As Boolean,
                                 objParametri_Super_Server As AgronicaCoreParametri,
                                 objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni)


        Dim farmaciList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni)

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'DT:     bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA 
        Dim gestioneMagazzino = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, CostantiPersonalizzate.FARMACI, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
        Dim gestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.FARMACI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
        Dim gestioneLotto = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, CostantiPersonalizzate.FARMACI, enum_Gestione_Lotti.Nessuna, objParametri_Utenti, objParametri_Server))
        Dim bloccaGiacenze_Utente = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        Dim usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero As Boolean
        STD_Utility.GetAssettoMagazzino(tipoAttivita, statoAttivita, gestioneLotto, gestioneMagazzino, gestioneGiacenze, bloccaGiacenze_Utente, escludiGiacenzeZero, usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero)

        'Se utilizzo il magazzino, leggo prima i prodotti in giacenza
        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing

        Dim strFiltroFarmCod As String = ""

        If usaMagazzino Then

            Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ") " & " AND Fabbricati.CodiceBDN = '" & Agro_SQL_SaveText(codiceBDN) & "' AND Fabbricati.ChkMagazzinoFarmaci = 1 AND Fabbricati.ProprietarioCapi = '" & Agro_SQL_SaveText(codFiscaleProprietario) & "' "

            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                       piva, saCod, 0,
                                                       FARMACI,
                                                       0,
                                                       0, 0, 0, 0, 0,
                                                       LOTTO_NONDEFINITO,
                                                       Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                       "",
                                                       "",
                                                       "", "",
                                                       "", "",
                                                       "", "",
                                                       "", "",
                                                       "", "",
                                                       "",
                                                       objParametri_Server, objParametri_Utenti,
                                                       xFiltroAggiuntivo_16:=xFiltroAggiuntivo_MagazzinoAttivoAllaData,
                                                       Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

            Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                           piva, saCod, 0,
                                                           FARMACI,
                                                           0,
                                                           0, 0, 0, 0, 0,
                                                           LOTTO_NONDEFINITO,
                                                           Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                           "",
                                                           "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "",
                                                           objParametri_Server, objParametri_Utenti,
                                                           xFiltroAggiuntivo_16:=xFiltroAggiuntivo_MagazzinoAttivoAllaData,
                                                           Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

            For i = 0 To Dt_Giacenze.Rows.Count - 1
                strFiltroFarmCod &= " Farmaci.Farm_Cod = " & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
            Next

            If strFiltroFarmCod <> "" Then
                strFiltroFarmCod = Left(strFiltroFarmCod, strFiltroFarmCod.Length - 3)
                strFiltroFarmCod = " AND (" & strFiltroFarmCod & ")"
            Else
                'se non ho alcun farmaco in magazzino e non si vogliono prodotti presenti solo in anagrafica, si esce
                If Not usaAnagrafica Then
                    Return farmaciList
                End If
            End If
        End If

        Dim objFarmaci As New AgronicaCoreMetaSchemaDAL.Farmaci
        Dim DtRisultati As New DataTable

        DtRisultati = objFarmaci.leggi(objParametri_Server, 0, codiceAIC)

        Dim DtFarmaciOrdinati = New DataTable
        DtFarmaciOrdinati.Columns.Add(New DataColumn("FarmDes", GetType(String)))
        DtFarmaciOrdinati.Columns.Add(New DataColumn("DettaglioSomministrazione", GetType(AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni)))
        DtFarmaciOrdinati.Columns.Add(New DataColumn("ConGiacenza", GetType(Integer)))


        Dim Dr As DataRow
        Dim DrGiacenze() As DataRow = Nothing
        Dim DrGiacenze_Tot() As DataRow = Nothing


        Dim objDTU As New AgronicaCoreDataProvider.DatatableUtility

        For Each farmaco In DtRisultati.Rows

            Dim Farm_Cod = farmaco.Item("Farm_Cod")
            Dim Farm_Des = farmaco.Item("Denominazione") + " - " + farmaco.Item("Confezione")
            Dim codice_AIC = farmaco.Item("AIC")

            Dim dettaglioSomministrazione = New AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni()
            dettaglioSomministrazione.codiceAIC = codice_AIC
            dettaglioSomministrazione.MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)
            dettaglioSomministrazione.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(Farm_Cod, FARMACI)
            dettaglioSomministrazione.prodotto.descrizione = Farm_Des

            dettaglioSomministrazione.dataPrescrizione = AGRODATAINIZIO

            Dim trovataGiacenza As Boolean = False
            If usaMagazzino Then

                If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                    DrGiacenze = Dt_Giacenze.Select("Pro_Cod = " & Farm_Cod)
                End If

                If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                    For g = 0 To DrGiacenze.Length - 1

                        Dim rilevamentoMagazzino As New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino()
                        rilevamentoMagazzino.Qta = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                        rilevamentoMagazzino.udm = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura()
                        rilevamentoMagazzino.udm.codice = DrGiacenze(g).Item("Udm_Cod")
                        rilevamentoMagazzino.udm.simbolo = DrGiacenze(g).Item("Udm_Sim")
                        rilevamentoMagazzino.Lotto = DrGiacenze(g).Item("lotto")
                        rilevamentoMagazzino.Magazzino = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato()
                        rilevamentoMagazzino.Magazzino.primaryKey = New anagrafiche.Fabbricato.PK()
                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK = New anagrafiche.CentroAziendale.PK()
                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva = DrGiacenze(g).Item("Piva")
                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice = DrGiacenze(g).Item("Sa_Cod")
                        rilevamentoMagazzino.Magazzino.primaryKey.codice = DrGiacenze(g).Item("Id_Destinazione")
                        rilevamentoMagazzino.Magazzino.descrizione = CStr(DrGiacenze(g).Item("Fabbricato_Des")) & " (" & CStr(DrGiacenze(g).Item("Sa_Nome")) & ")"
                        rilevamentoMagazzino.Magazzino.tipo = DrGiacenze(g).Item("Tipo_Destinazione")

                        dettaglioSomministrazione.unitaDiMisura = rilevamentoMagazzino.udm

                        If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                            DrGiacenze_Tot = Dt_Giacenze_Tot.Select(" Piva = '" & DrGiacenze(g).Item("Piva") & "' and Sa_Cod =" & DrGiacenze(g).Item("Sa_Cod") & " and Id_Destinazione=" & DrGiacenze(g).Item("Id_Destinazione") & " and lotto='" & Agro_SQL_SaveText(DrGiacenze(g).Item("lotto")) & "'")
                            If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                rilevamentoMagazzino.QtaTot = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4)
                            End If
                        End If

                        Dim dettaglioTrattamentoGiacenza = dettaglioSomministrazione.Clona
                        dettaglioTrattamentoGiacenza.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                        Dr = DtFarmaciOrdinati.NewRow
                        Dr.Item("FarmDes") = Farm_Des
                        Dr.Item("DettaglioSomministrazione") = dettaglioTrattamentoGiacenza
                        Dr.Item("ConGiacenza") = 1
                        DtFarmaciOrdinati.Rows.Add(Dr)

                        trovataGiacenza = True
                    Next

                End If

            End If

            'DT: il prodotto va restituito anche se presente solo in anagrafica se "usaAnagrafica=true"
            'DT: se si sta usando anche il magazzino, si restituisce il prodotto (senza indicazioni di giacenza) solo se non è stato trovato in magazzino (trovataGiacenza=False)
            If usaAnagrafica AndAlso Not trovataGiacenza Then
                Dr = DtFarmaciOrdinati.NewRow
                Dr.Item("FarmDes") = Farm_Des
                Dr.Item("DettaglioSomministrazione") = dettaglioSomministrazione
                Dr.Item("ConGiacenza") = 0
                DtFarmaciOrdinati.Rows.Add(Dr)
            End If

        Next

        If DtFarmaciOrdinati IsNot Nothing Then

            Dim Dv As New DataView()
            DtFarmaciOrdinati.TableName = "Farmaci"
            Dv.Table = DtFarmaciOrdinati
            Dv.Sort = "ConGiacenza DESC, FarmDes ASC"

            For i = 0 To Dv.Count - 1
                farmaciList.Add(Dv(i).Item("DettaglioSomministrazione"))
            Next

        End If

        Return farmaciList

    End Function

End Class