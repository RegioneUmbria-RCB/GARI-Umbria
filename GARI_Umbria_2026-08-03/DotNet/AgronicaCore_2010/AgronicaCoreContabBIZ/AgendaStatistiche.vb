Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Threading.Tasks
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreUtility
Imports InData.Agenda

Public Class AgendaStatistiche
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function CaricaOperazioniStatistiche(ByVal InData As LeggiAgendaStatistiche,
                                                ByRef objP_Server As AgronicaCoreParametri,
                                                ByRef objP_Utenti As AgronicaCoreParametri) As List(Of AttivitaStatistiche)

        Dim r As New List(Of AttivitaStatistiche)
        Dim res As New ConcurrentQueue(Of AttivitaStatistiche)
        Dim po = New ParallelOptions With {.MaxDegreeOfParallelism = Environment.ProcessorCount / 2}
        Dim filtroOperazioni As String = String.Empty
        Dim filtroLavorazioni As String = String.Empty

        Dim statDAL As New AgronicaCoreContabDAL.AgendaStatistiche_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objUtentiImpostazioniMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R

        Dim dt = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA,
                                               1,
                                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", objP_Utenti)

        If dt.Rows.Count > 0 Then

            If Not IsDBNull(dt.Rows(0).Item("Impostazione_Valore_1")) AndAlso dt.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim tipi As String = dt.Rows(0).Item("Impostazione_Valore_1")
                Dim tipis() As String = tipi.Split("|")
                Dim Filtro_Tipo_GruppoOperazioni As String = " ( "
                For i = 0 To tipis.Count - 1
                    If i > 0 Then
                        Filtro_Tipo_GruppoOperazioni = Filtro_Tipo_GruppoOperazioni & " OR "
                    End If
                    Select Case (tipis(i))
                        Case "C", "E", "Z", "P", "V"
                            Filtro_Tipo_GruppoOperazioni &= " GruppoOperazioni.TIPO = '" & tipis(i) & "' "

                        Case "E6"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 ) "
                        Case "E10"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 ) "

                    End Select

                Next
                filtroOperazioni = String.Concat(Filtro_Tipo_GruppoOperazioni, " ) ")
            End If

        End If

        dt = objUtentiImpostazioniMono.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI,
                                                           0,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objP_Utenti)
        Dim filtroTipo As String = ""
        If dt.Rows.Count > 0 Then
            If filtroOperazioni.Length > 0 Then
                filtroOperazioni = String.Concat(filtroOperazioni, " AND ")
            End If
            filtroTipo = " GruppoOperazioni.GRU_COD in ("
            For i = 0 To dt.Rows.Count - 1

                If i <> 0 Then
                    filtroTipo &= " ,"
                End If
                filtroTipo &= dt.Rows(i).Item("ID_0")
            Next
            filtroTipo &= " ) "
            If filtroOperazioni.Length > 0 Then
                filtroOperazioni = String.Concat(filtroOperazioni, filtroTipo)
            Else
                filtroOperazioni = filtroTipo
            End If
        End If

        If InData.Tipo_operazioni.Length > 0 Then

            If filtroOperazioni.Length > 0 Then
                filtroOperazioni = String.Concat(filtroOperazioni, " AND ")
            End If

            filtroOperazioni = String.Concat(filtroOperazioni, " GruppoOperazioni.GRU_COD ", QueryBuilderUtility.GeneraClausolaINDaList(InData.Tipo_operazioni.ToList()))

        End If

        dt = objUtentiImpostazioniMono.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           0,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objP_Utenti)
        Dim Filtro_Utente_Lavorazioni = ""
        If dt.Rows.Count > 0 Then
            Filtro_Utente_Lavorazioni = " agenda.Lav_Cod in ("
            For i = 0 To dt.Rows.Count - 1

                If i <> 0 Then
                    Filtro_Utente_Lavorazioni &= " ,"
                End If
                Filtro_Utente_Lavorazioni &= dt.Rows(i).Item("ID_0")
            Next
            Filtro_Utente_Lavorazioni &= " )  "

        End If
        filtroLavorazioni = Filtro_Utente_Lavorazioni

        If InData.Operazioni.Length > 0 Then

            If filtroLavorazioni.Length > 0 Then
                filtroLavorazioni = String.Concat(filtroLavorazioni, " AND ")
            End If

            filtroLavorazioni = String.Concat(filtroLavorazioni, " Operazioni.LAV_COD ", QueryBuilderUtility.GeneraClausolaINDaList(InData.Operazioni.ToList()))

        End If

        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(objP_Utenti.UtenteUsername, objP_Utenti)

        dt = objUtentiImpostazioniMono.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI,
                                                           0,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objP_Utenti)
        Dim filtroGruppiVeg As String = ""
        If dt.Rows.Count > 0 Then
            filtroGruppiVeg = " SpecieVegetali.gru_cod in ("
            For i = 0 To dt.Rows.Count - 1

                If i <> 0 Then
                    filtroGruppiVeg &= " ,"
                End If
                filtroGruppiVeg &= dt.Rows(i).Item("ID_0")
            Next
            filtroGruppiVeg &= " ) "

        End If

        dt = objUtentiImpostazioniMono.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI,
                                                           0,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objP_Utenti)
        Dim filtroSpecieVeg As String = ""
        If dt.Rows.Count > 0 Then
            If filtroSpecieVeg.Length > 0 Then
                filtroSpecieVeg = String.Concat(filtroSpecieVeg, " AND ")
            End If
            filtroSpecieVeg = " SpecieVegetali.Veg_Cod in ("
            For i = 0 To dt.Rows.Count - 1

                If i <> 0 Then
                    filtroSpecieVeg &= " ,"
                End If
                filtroSpecieVeg &= dt.Rows(i).Item("ID_0")
            Next
            filtroSpecieVeg &= " ) "

        End If

        Dim dte As New DataTable
        Dim filtroSpecieUtente As String = ""
        Dim filtroAziende As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Piva_aziende.ToList())
        Dim filtroSpecie As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Specie_vegetale.Select(Function(x) x.codice).ToList())
        Dim filtroReferenti As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Aziende_referenti.ToList())
        Dim filtroNazioni As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Nazioni.ToList())
        Dim filtroRegioni As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Regioni.ToList())
        Dim filtroProvince As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Province.ToList())
        Dim filtroComuni As String = QueryBuilderUtility.GeneraClausolaINDaList(InData.Comuni.ToList())

        If filtroSpecieVeg.Length > 0 And filtroGruppiVeg.Length > 0 Then
            filtroSpecieUtente = String.Concat(filtroGruppiVeg, " AND ", filtroSpecieVeg)
        ElseIf filtroSpecieVeg.Length > 0 Then
            filtroSpecieUtente = filtroSpecieVeg
        Else
            filtroSpecieUtente = filtroGruppiVeg
        End If

        If InData.OperazioniColt Then

            dte = statDAL.Estrazione_OpColturali(InData, objP_Server, objP_Utenti, filtroAziende,
                                                 filtroSpecie, filtroOperazioni, filtroLavorazioni, filtroReferenti,
                                                 filtroNazioni, filtroRegioni, filtroProvince, filtroComuni,
                                                 Filtro_Visibilita_Utente, filtroSpecieUtente)

            If InData.Estrazione <> 1 Then
                Dim dteP = statDAL.Estrazione_PostRaccolta(InData, objP_Server, objP_Utenti, filtroAziende,
                                                       filtroSpecie, filtroOperazioni, filtroLavorazioni, filtroReferenti,
                                                       filtroNazioni, filtroRegioni, filtroProvince, filtroComuni,
                                                       Filtro_Visibilita_Utente, filtroSpecieUtente)

                Dim dteV = statDAL.Estrazione_Visite(InData, objP_Server, objP_Utenti, filtroAziende,
                                                 filtroSpecie, filtroOperazioni, filtroLavorazioni, filtroReferenti,
                                                 filtroNazioni, filtroRegioni, filtroProvince, filtroComuni,
                                                 Filtro_Visibilita_Utente)

                dte.Merge(dteP)
                dte.Merge(dteV)
            End If

        Else

                dte = statDAL.Estrazione_Impianti(InData, objP_Server, objP_Utenti, filtroAziende,
                                              filtroSpecie, filtroOperazioni, filtroLavorazioni, filtroReferenti,
                                              filtroNazioni, filtroRegioni, filtroProvince, filtroComuni,
                                              Filtro_Visibilita_Utente)

        End If

        Parallel.ForEach(dte.AsEnumerable, po,
                         Sub(row)

                             Dim x As New AttivitaStatistiche With {
                                .Piva = If(IsDBNull(row.Item("Piva")), "", row.Item("Piva")),
                                .Sa_Cod = If(IsDBNull(row.Item("Sa_Cod")), 0, row.Item("Sa_Cod")),
                                .Id_Agenda = If(IsDBNull(row.Item("Id_Agenda")), 0, row.Item("Id_Agenda")),
                                .Id_Mov = If(IsDBNull(row.Item("Id_Mov")), 0, row.Item("Id_Mov")),
                                .Id_Mov_Det = If(IsDBNull(row.Item("Id_Mov_Det")), 0, row.Item("Id_Mov_Det")),
                                .Lav_Cod = If(IsDBNull(row.Item("Lav_Cod")), 0, row.Item("Lav_Cod")),
                                .Des_Lib = If(IsDBNull(row.Item("Des_Lib")), "", row.Item("Des_Lib")),
                                .Data_Movimento = If(IsDBNull(row.Item("Data_Movimento")), CDate("01/01/1900"), FormatDateTime(row.Item("Data_Movimento"), DateFormat.ShortDate)),
                                .Ora = If(IsDBNull(row.Item("Ora")), CDate("01/01/1900"), row.Item("Ora")),
                                .Mov_Desc = If(IsDBNull(row.Item("Mov_Desc")), "", row.Item("Mov_Desc")),
                                .Username_Creazione = If(IsDBNull(row.Item("Username_Creazione")), "", row.Item("Username_Creazione")),
                                .Data_creazione = If(IsDBNull(row.Item("Data_Creazione")), CDate("01/01/1900"), FormatDateTime(row.Item("Data_Creazione"), DateFormat.ShortDate)),
                                .Cau_Mov = If(IsDBNull(row.Item("Cau_Mov")), "", row.Item("Cau_Mov")),
                                .Cul_Cod = If(IsDBNull(row.Item("Cul_Cod")), 0, row.Item("Cul_Cod")),
                                .Veg_Cod = If(IsDBNull(row.Item("Veg_Cod")), 0, row.Item("Veg_Cod")),
                                .Veg_Des = If(IsDBNull(row.Item("Veg_Des")), "", row.Item("Veg_Des")),
                                .Raccoglitore_Cod = If(IsDBNull(row.Item("Raccoglitore_Cod")), 0, row.Item("Raccoglitore_Cod")),
                                .Tecnico = If(IsDBNull(row.Item("Tecnico")), "", row.Item("Tecnico")),
                                .Tipo_Destinazione = If(IsDBNull(row.Item("Tipo_Destinazione")), 0, row.Item("Tipo_Destinazione")),
                                .Appezza = If(IsDBNull(row.Item("Appezza")), 0, row.Item("Appezza")),
                                .App_Nome = If(IsDBNull(row.Item("App_Nome")), "", row.Item("App_Nome")),
                                .Elem_Cod = If(IsDBNull(row.Item("Elem_Cod")), 0, row.Item("Elem_Cod")),
                                .Mat_Cod = If(IsDBNull(row.Item("Mat_Cod")), 0, row.Item("Mat_Cod")),
                                .Pro_Cod = If(IsDBNull(row.Item("Pro_Cod")), 0, row.Item("Pro_Cod")),
                                .TipoProdotto = If(IsDBNull(row.Item("TipoProdotto")), "", row.Item("TipoProdotto")),
                                .NomeProdotto = If(IsDBNull(row.Item("NomeProdotto")), "", row.Item("NomeProdotto")),
                                .Cod_Articolo = If(IsDBNull(row.Item("Cod_Articolo")), "", row.Item("Cod_Articolo")),
                                .sa_nome = If(IsDBNull(row.Item("sa_nome")), "", row.Item("sa_nome")),
                                .rag_soc = If(IsDBNull(row.Item("rag_soc")), "", row.Item("rag_soc")),
                                .lav_des = If(IsDBNull(row.Item("lav_des")), "", row.Item("lav_des")),
                                .gru_des = If(IsDBNull(row.Item("gru_des")), "", row.Item("gru_des")),
                                .tipo = If(IsDBNull(row.Item("tipo")), "", row.Item("tipo")),
                                .cul_des = If(IsDBNull(row.Item("cul_des")), "", row.Item("cul_des")),
                                .campo_des = If(IsDBNull(row.Item("campo_des")), "", row.Item("campo_des")),
                                .IdImpianto = If(IsDBNull(row.Item("IdImpianto")), 0, row.Item("IdImpianto")),
                                .LottoImpianto = If(IsDBNull(row.Item("LottoImpianto")), "", row.Item("LottoImpianto")),
                                .SupApp = If(IsDBNull(row.Item("SupApp")), 0, row.Item("SupApp")),
                                .QtaImp = If(IsDBNull(row.Item("QtaImp")), 0, row.Item("QtaImp")),
                                .SupTrattata = If(IsDBNull(row.Item("SupTrattata")), 0, row.Item("SupTrattata")),
                                .DestinazioneTerreniNudi_Cod = If(IsDBNull(row.Item("DestinazioneTerreniNudi_Cod")), 0, row.Item("DestinazioneTerreniNudi_Cod")),
                                .DestinazioneTerreniNudi_Des = If(IsDBNull(row.Item("DestinazioneTerreniNudi_Des")), "", row.Item("DestinazioneTerreniNudi_Des")),
                                .Data_Ultima_Modifica_Intervento = If(IsDBNull(row.Item("Data_Ultima_Modifica_Intervento")), CostantiPersonalizzate.AGRODATAINIZIO, FormatDateTime(row.Item("Data_Ultima_Modifica_Intervento"), DateFormat.ShortDate)),
                                .validita_inizio_destinazione = If(IsDBNull(row.Item("validita_inizio_destinazione")), CostantiPersonalizzate.AGRODATAINIZIO, FormatDateTime(row.Item("validita_inizio_destinazione"), DateFormat.ShortDate)),
                                .Qta_Extra_Totale = If(IsDBNull(row.Item("Qta_Extra_Totale")), "",
                                                    If(row.Item("Qta_Extra_Totale") = 0 AndAlso Not IsDBNull(row.Item("QtaProd")),
                                                    row.Item("QtaProd"), row.Item("Qta_Extra_Totale"))),
                                .QtaProd = If(IsDBNull(row.Item("QtaProd")), 0, row.Item("QtaProd")),
                                .QTA_EXTRA = If(IsDBNull(row.Item("QTA_EXTRA")), 0, row.Item("QTA_EXTRA")),
                                .UdmProd = If(IsDBNull(row.Item("UdmProd")), 0, row.Item("UdmProd")),
                                .UdmProdSim = If(IsDBNull(row.Item("UdmProdSim")), "", row.Item("UdmProdSim")),
                                .UdmImp = If(IsDBNull(row.Item("UdmProd")), 0, row.Item("UdmProd")),
                                .UdmImpSim = If(IsDBNull(row.Item("UdmProdSim")), "", row.Item("UdmProdSim")),
                                .UdmExtra = If(IsDBNull(row.Item("UdmExtra")), 0,
                                            If(row.Item("Qta_Extra_Totale") = 0 AndAlso Not IsDBNull(row.Item("QtaProd")),
                                            row.Item("UdmProd"), row.Item("UdmExtra"))),
                                .UdmExtraSim = If(IsDBNull(row.Item("UdmExtraSim")), "",
                                               If(row.Item("Qta_Extra_Totale") = 0 AndAlso Not IsDBNull(row.Item("QtaProd")),
                                               row.Item("UdmProdSim"), row.Item("UdmExtraSim"))),
                                .Azienda_Padre = If(IsDBNull(row.Item("Azienda_Padre")), "", row.Item("Azienda_Padre")),
                                .Piva_Padre = If(IsDBNull(row.Item("Piva_Padre")), "", row.Item("Piva_Padre")),
                                .Stato = If(IsDBNull(row.Item("Stato")), "", row.Item("Stato")),
                                .Regione = If(IsDBNull(row.Item("Regione")), "", row.Item("Regione")),
                                .Provincia = If(IsDBNull(row.Item("Provincia")), "", row.Item("Provincia")),
                                .Localita = If(IsDBNull(row.Item("Localita")), "", row.Item("Localita")),
                                .Anno_movimento = If(IsDBNull(row.Item("Anno_movimento")), 1900, row.Item("Anno_movimento")),
                                .Mese_movimento = If(IsDBNull(row.Item("Mese_movimento")), 1, row.Item("Mese_movimento")),
                                .num_impianti = row.Item("num_imp"),
                                .num_operazioni = row.Item("num_operazioni"),
                                .num_poligoni = row.Item("num_pol"),
                                .num_poligoni_mancanti = row.Item("num_pol_manc"),
                                .Anno_Creazione_Azienda = If(IsDBNull(row.Item("Anno_Creazione_Azienda")), 1900, row.Item("Anno_Creazione_Azienda")),
                                .Anno_Creazione_Imp = If(IsDBNull(row.Item("Anno_Creazione_Imp")), 1900, row.Item("Anno_Creazione_Imp")),
                                .Anno_Creazione_Op = If(IsDBNull(row.Item("Anno_Creazione_Op")), 1900, row.Item("Anno_Creazione_Op")),
                                .Data_Creazione_Azienda = If(IsDBNull(row.Item("Data_Creazione_Azienda")), CDate("01/01/1900"), FormatDateTime(row.Item("Data_Creazione_Azienda"), DateFormat.ShortDate)),
                                .Data_Creazione_Imp = If(IsDBNull(row.Item("Data_Creazione_Imp")), CDate("01/01/1900"), FormatDateTime(row.Item("Data_Creazione_Imp"), DateFormat.ShortDate)),
                                .Data_Creazione_Op = If(IsDBNull(row.Item("Data_Creazione_Op")), CDate("01/01/1900"), FormatDateTime(row.Item("Data_Creazione_Op"), DateFormat.ShortDate)),
                                .Mese_Creazione_Azienda = If(IsDBNull(row.Item("Mese_Creazione_Azienda")), 1, row.Item("Mese_Creazione_Azienda")),
                                .Mese_Creazione_Imp = If(IsDBNull(row.Item("Mese_Creazione_Imp")), 1, row.Item("Mese_Creazione_Imp")),
                                .Mese_Creazione_Op = If(IsDBNull(row.Item("Mese_Creazione_Op")), 1, row.Item("Mese_Creazione_Op")),
                                .Utente_Creazione_Azienda = If(IsDBNull(row.Item("Utente_Creazione_Azienda")), "", row.Item("Utente_Creazione_Azienda")),
                                .Utente_Creazione_Imp = If(IsDBNull(row.Item("Utente_Creazione_Imp")), "", row.Item("Utente_Creazione_Imp")),
                                .Utente_Creazione_Op = If(IsDBNull(row.Item("Utente_Creazione_Op")), "", row.Item("Utente_Creazione_Op"))
                             }

                             res.Enqueue(x)
                         End Sub)

        Return res.ToList

    End Function

    Public Function CaricaOperazioniStatistichePivot(ByVal piva_Aziende As List(Of String),
                                                     ByVal centri_Aziendali As List(Of String),
                                                     ByVal specie As List(Of Integer),
                                                     ByVal dataInizioOp As String, ByVal dataFineOp As String,
                                                     ByVal periodoDataOperazioni As Integer,
                                                     ByVal dataInizioImp As String, ByVal dataFineImp As String,
                                                     ByVal periodoData As Integer,
                                                     ByVal tipoOperazioni As List(Of Integer),
                                                     ByVal operazioni As List(Of Integer),
                                                     ByVal nazioni As List(Of String), ByVal regioni As List(Of String),
                                                     ByVal province As List(Of String), ByVal comuni As List(Of String), ByVal impianti As Boolean,
                                                     ByVal prodotti As Boolean, ByVal operazioniColt As Boolean,
                                                     ByVal estrazione As Integer, ByVal aziende_referenti As List(Of String), ByVal datiTecnici As Boolean,
                                                     ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                     ByRef ObjParametri_Utenti As AgronicaCoreParametri) As List(Of AttivitaStatistiche)

        Dim centriAziendali As CentroAziendale.PK() = {}
        Dim specieobj As Specie() = {}

        For Each centro In centri_Aziendali
            Dim pknew As New CentroAziendale.PK
            pknew.partitaIva = centro.Split("|").First
            pknew.codice = centro.Split("|").Last

            Array.Resize(centriAziendali, centriAziendali.Length + 1)
            centriAziendali(centriAziendali.Length - 1) = pknew
        Next

        For Each spec In specie
            Dim specnew As New Specie With {
                .codice = spec,
                .descrizione = "Banana"
            }

            Array.Resize(specieobj, specieobj.Length + 1)
            specieobj(specieobj.Length - 1) = specnew
        Next

        Dim inData As New LeggiAgendaStatistiche With {
            .Piva_aziende = piva_Aziende.ToArray,
            .Centri_aziendali = centriAziendali,
            .Specie_vegetale = specieobj,
            .Data_inizio_operazione = If(dataInizioOp.Length > 0, CDate(dataInizioOp), CostantiPersonalizzate.AGRODATAINIZIO),
            .Data_fine_operazione = If(dataFineOp.Length > 0, CDate(dataFineOp), CostantiPersonalizzate.AGRODATAFINE),
            .Periodo_da_data_operazione = periodoDataOperazioni,
            .Data_inizio_impianto = If(dataInizioImp.Length > 0, CDate(dataInizioImp), CostantiPersonalizzate.AGRODATAINIZIO),
            .Data_fine_impianto = If(dataFineImp.Length > 0, CDate(dataFineImp), CostantiPersonalizzate.AGRODATAFINE),
            .Periodo_da_data_Impianto = periodoData,
            .Tipo_operazioni = tipoOperazioni.ToArray,
            .Operazioni = operazioni.ToArray,
            .Nazioni = nazioni.ToArray,
            .Regioni = regioni.ToArray,
            .Province = province.ToArray,
            .Comuni = comuni.ToArray,
            .Impianti = impianti,
            .Prodotti = prodotti,
            .OperazioniColt = operazioniColt,
            .Estrazione = estrazione,
            .Aziende_referenti = aziende_referenti.ToArray,
            .Dati_tecnici = datiTecnici
            }

        Return CaricaOperazioniStatistiche(inData, ObjParametri_Server, ObjParametri_Utenti)
    End Function

End Class
