Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class STD_Sementi

    Public Function LeggiSementi(tipoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita,
                                 statoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Stati,
                                 lavorazione As attivita.Lavorazione,
                                 impianti As Impianto(),
                                 filtroPerDescrizione As String,
                                 validitaFine As DateTime,
                                 escludiGiacenzeZero As Boolean,
                                 magazziniAgenzie As Boolean,
                                 magazziniEsterni As Boolean,
                                 objParametri_Super_Server As AgronicaCoreParametri,
                                 objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri) As List(Of attivita.dettagli.DettaglioSemina)

        Dim sementiList As New List(Of attivita.dettagli.DettaglioSemina)

        'Dim sa_cod_list = STD_Utility.getSaCodDaImpianti(impianti)
        Dim piva = STD_Utility.getPivaDaImpianti(impianti)

        Dim chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK) = Nothing

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA 
        Dim gestioneMagazzino = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, CostantiPersonalizzate.SEMENTI, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
        Dim gestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.SEMENTI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
        'DT: per compatibilità con il pregresso, se non c'è l'impostazione si assume che debba essere gestito
        Dim gestioneLotto = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, CostantiPersonalizzate.SEMENTI, enum_Gestione_Lotti.Obbligatoria, objParametri_Utenti, objParametri_Server))
        Dim bloccaGiacenze_Utente = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        'DT: in caso di magazzino esterno partiamo prendendo tutti i prodotti, senza considerare la giacenza e filtriamo successivamente, in base alle impostazioni di ogni singolo magazzino esterno
        If magazziniEsterni Then
            gestioneGiacenze = enum_Gestione_Giacenze.TuttiProdotti
        End If

        Dim usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero As Boolean
        STD_Utility.GetAssettoMagazzino(tipoAttivita, statoAttivita, gestioneLotto, gestioneMagazzino, gestioneGiacenze, bloccaGiacenze_Utente, escludiGiacenzeZero, usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero)

        Dim DtSementiOrdinate = New DataTable
        DtSementiOrdinate.Columns.Add(New DataColumn("MatDes", GetType(String)))
        DtSementiOrdinate.Columns.Add(New DataColumn("DettaglioSemina", GetType(AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina)))
        DtSementiOrdinate.Columns.Add(New DataColumn("ConGiacenza", GetType(Integer)))

        'DT: la semina è sempre multi specie

        Dim filtroRicerca As String = ""
        If filtroPerDescrizione <> "" Then
            filtroRicerca = " AND Materie_Prime.Mat_Des Like '%" & filtroPerDescrizione & "%' "
        End If

        Dim filtroTipologiaSementi As String = ""
        Select Case lavorazione.primaryKey.codice
            Case LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                filtroTipologiaSementi = " AND Materie_Prime.Sem_Cod IN (1,3,7) "
            Case LAVCOD_TRAPIANTO
                filtroTipologiaSementi = " AND Materie_Prime.Sem_Cod IN (2,3,4,5,6,7,8,10) "
        End Select

        'Caricamento magazzini agenzie, se presenti
        Dim filtroMagazziniEsterni = STD_Utility.getfiltroMagazziniEsterni(piva, tipoAttivita, statoAttivita, magazziniAgenzie, magazziniEsterni, chiaviMagazziniUso_da_terzi,
                                                                            objParametri_Server, objParametri_Utenti)

        Dim strFiltroMatCod As String = ""

        Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = "AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ")"

        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing

        If usaMagazzino Then

            Dim objG As New AgronicaCoreContabDAL.Giacenze_R

            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                       piva, 0, 0,
                                                       SEMENTI,
                                                       0, Mat_Cod:=0,
                                                       0, 0, 0, 0,
                                                       LOTTO_NONDEFINITO,
                                                       Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                       xFiltroAggiuntivo_MagazzinoAttivoAllaData, "",
                                                       "", "",
                                                       "", "",
                                                       "", filtroRicerca & filtroTipologiaSementi,
                                                       "", "",
                                                       "", "",
                                                       objParametri_Server, objParametri_Utenti,
                                                       Flag_QtaMaggioreZero:=flagQtaMaggioreZero,
                                                       filtroMagazziniEsterni:=filtroMagazziniEsterni)

            Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                           piva, 0, 0,
                                                           SEMENTI,
                                                           0, Mat_Cod:=0,
                                                           0, 0, 0, 0,
                                                           LOTTO_NONDEFINITO,
                                                           Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                           xFiltroAggiuntivo_MagazzinoAttivoAllaData, "",
                                                           "", "",
                                                           "", "",
                                                           "", filtroRicerca & filtroTipologiaSementi,
                                                           "", "",
                                                           "", "",
                                                           objParametri_Server, objParametri_Utenti,
                                                           Flag_QtaMaggioreZero:=flagQtaMaggioreZero,
                                                           filtroMagazziniEsterni:=filtroMagazziniEsterni)


            STD_Utility.FiltraGiacenze_X_MagazziniUso_da_terzi(chiaviMagazziniUso_da_terzi, Dt_Giacenze, Dt_Giacenze_Tot)

            For i = 0 To Dt_Giacenze.Rows.Count - 1
                Dim prodToFiter = " Materie_Prime.Mat_Cod=" & Dt_Giacenze.Rows(i).Item("Mat_Cod").ToString & " OR "
                If Not strFiltroMatCod.Contains(prodToFiter) Then
                    If flagQtaMaggioreZero = True Then
                        If Math.Abs(Dt_Giacenze.Rows(i).Item("Giacenza")) > QTA_GiancenzeVisualizzate Then
                            strFiltroMatCod &= prodToFiter
                        End If
                    Else
                        strFiltroMatCod &= prodToFiter
                    End If
                End If
            Next

            If strFiltroMatCod <> "" Then
                strFiltroMatCod = Left(strFiltroMatCod, strFiltroMatCod.Length - 3)
                strFiltroMatCod = " (" & strFiltroMatCod & ")"
            Else
                'se non ho sementi in magazzino e non si vogliono sementi presenti solo in anagrafica, si esce
                If Not usaAnagrafica Then
                    Return sementiList
                End If
            End If
        End If

        Dim strFiltroAggiuntivo = ""
        If usaMagazzino AndAlso Not usaAnagrafica Then
            strFiltroAggiuntivo &= strFiltroMatCod
        End If

        'DT: sa_cod ininfluente, le materie prime non sono usate per centro aziendale
        Dim objSementi As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim DtRisultati = objSementi.Leggi(piva, Sa_Cod:=0,
                                       SEMENTI,
                                       0, "", 0, 0,
                                       0, 0, 0, 0, 0,
                                       filtroPerDescrizione, 0, "",
                                       False, Flag_MateriePrimeSoloPrivate:=False,
                                       filtroTipologiaSementi,
                                       enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                       xFiltroAggiuntivo:=strFiltroAggiuntivo,
                                       "",
                                       objParametri_Server)

        If IsNothing(DtRisultati) OrElse DtRisultati.Rows.Count = 0 Then
            Return sementiList
        End If

        For i = 0 To DtRisultati.Rows.Count - 1

            Dim Mat_Cod = DtRisultati.Rows(i).Item("mat_cod")
            Dim Mat_Des = DtRisultati.Rows(i).Item("mat_des")

            Dim dettaglioSemina = New AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina
            dettaglioSemina.MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)
            dettaglioSemina.prodotto = New attivita.risorse.Prodotto(Mat_Cod, SEMENTI)
            dettaglioSemina.prodotto.descrizione = Mat_Des
            dettaglioSemina.varieta = New utilizzi.Varieta(DtRisultati.Rows(i).Item("Cul_cod"))
            dettaglioSemina.varieta.specie = New utilizzi.Specie(DtRisultati.Rows(i).Item("Veg_cod"))
            dettaglioSemina.varieta.specie.descrizione = DtRisultati.Rows(i).Item("Veg_Des")
            dettaglioSemina.codArticolo = DtRisultati.Rows(i).Item("Cod_Articolo")
            dettaglioSemina.regolamento = DtRisultati.Rows(i).Item("Regolamento")

            Dim trovataGiacenza As Boolean = False
            If usaMagazzino Then

                Dim DrGiacenze() As DataRow = Nothing
                If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                    DrGiacenze = Dt_Giacenze.Select("mat_cod=" & Mat_Cod)
                End If

                If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                    For g = 0 To DrGiacenze.Length - 1

                        If magazziniEsterni Then
                            Dim gestioneGiacenzeMagazzinoEsterno = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(DrGiacenze(g).Item("Piva"), Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.SEMENTI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
                            Dim qtaMagazzinoEsterno = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                            If gestioneGiacenzeMagazzinoEsterno = enum_Gestione_Giacenze.SoloPresenti Then
                                If qtaMagazzinoEsterno <= 0 Then
                                    Continue For
                                End If
                            End If
                        End If

                        Dim dettaglioSeminaGiacenza = dettaglioSemina.Clona()

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

                        If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                            Dim DrGiacenze_Tot = Dt_Giacenze_Tot.Select("mat_cod=" & Mat_Cod & " and Piva='" & DrGiacenze(g).Item("Piva") & "' and Sa_Cod=" & DrGiacenze(g).Item("Sa_Cod") & " and Id_Destinazione=" & DrGiacenze(g).Item("Id_Destinazione") & " and lotto='" & Agro_SQL_SaveText(DrGiacenze(g).Item("lotto")) & "'")
                            If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                rilevamentoMagazzino.QtaTot = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4)
                            End If
                        End If

                        If Not IsNothing(chiaviMagazziniUso_da_terzi) AndAlso chiaviMagazziniUso_da_terzi.FindIndex(Function(u) u.centroAziendalePK.partitaIva = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                u.centroAziendalePK.codice = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                u.codice = rilevamentoMagazzino.Magazzino.primaryKey.codice) > -1 Then
                            rilevamentoMagazzino.Magazzino.usoDaTerzi = True

                            'Se il magazzino ha il fabbricato codice Uso da Terzi aggiungo alla descrizione del Magazzino l'Azienda
                            rilevamentoMagazzino.Magazzino.descrizione &= " (" & AgronicaCoreDataProvider.My.Resources.Gias.Azienda & ": " & CStr(DrGiacenze(g).Item("Impresa")) & ")"
                        Else
                            rilevamentoMagazzino.Magazzino.usoDaTerzi = False
                        End If

                        dettaglioSeminaGiacenza.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                        Dim Dr = DtSementiOrdinate.NewRow
                        Dr.Item("MatDes") = Mat_Des
                        Dr.Item("DettaglioSemina") = dettaglioSeminaGiacenza
                        Dr.Item("ConGiacenza") = 1

                        DtSementiOrdinate.Rows.Add(Dr)

                        trovataGiacenza = True

                    Next

                End If

            End If

            'DT: la semente va restituita anche se presente solo in anagrafica se "usaAnagrafica=true"
            'DT: se si sta usando anche il magazzino, si restituisce la semente (senza indicazioni di giacenza) solo se non è stato trovato in magazzino (trovataGiacenza=False)
            If usaAnagrafica AndAlso Not trovataGiacenza Then

                Dim Dr = DtSementiOrdinate.NewRow
                Dr.Item("MatDes") = Mat_Des
                Dr.Item("DettaglioSemina") = dettaglioSemina
                Dr.Item("ConGiacenza") = 0

                DtSementiOrdinate.Rows.Add(Dr)
            End If

        Next

        If DtSementiOrdinate IsNot Nothing Then

            Dim Dv As New DataView()
            DtSementiOrdinate.TableName = "Sementi"
            Dv.Table = DtSementiOrdinate
            Dv.Sort = "ConGiacenza DESC, MatDes ASC"

            For i = 0 To Dv.Count - 1
                sementiList.Add(Dv(i).Item("DettaglioSemina"))
            Next

        End If

        Return sementiList

    End Function
End Class
