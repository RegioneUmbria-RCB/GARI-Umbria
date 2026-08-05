Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ

Public Class Ricette_Operazioni_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Function Ricette_Operazioni_LeggiPerAPP(
        ByRef ErrMSG As String,
        ByVal Piva As String,
        ByVal Ricetta_Cod As Integer,
        ByVal Ricetta_Operazione_Cod As Integer,
        ByVal Lav_Cod As Integer,
        ByVal Gru_Op As Integer,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByRef Dt_Ricette As DataTable,
        ByRef Dt_RicetteXNote As DataTable,
        ByRef Dt_Operazioni As DataTable,
        ByRef Dt_Dettagli As DataTable,
        ByRef Dt_DettaglioTecnico As DataTable,
        ByRef Dt_Destinazioni As DataTable,
        ByRef objParametri As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
        ) As Integer

        Dim rval As Integer = 1

        Try




            'Leggo i dati della Ricette

            Dim objRicette_R As New AgronicaCoreContabDAL.Ricette_R

            Dim filtroPerLetturaAPP As String =
                " Ricette.PIVA = '" & Agro_SQL_SaveText(Piva) & "' AND Ricette_Operazioni.W_Anagrafica_Stati_Cod =  " &
                    enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire &
                " AND Ricette_Operazioni.Ricetta_Operazione_Cod not in (select distinct Ricetta_Operazione_Cod_RIF from Ricette_Operazioni where Ricetta_Operazione_Cod_RIF is not null union " &
                " select distinct Ricetta_Operazione_Cod_RIF from APP_Ricette_Operazioni where Ricetta_Operazione_Cod_RIF is not null and (importato_data is null or importato_errore <>'')) "

            ' filtro per ricette da inviare all'app
            ' filtroPerLetturaAPP &= " AND (Ricette_Operazioni.Invia_App=1 OR Ricette_Operazioni.APP_Ricetta_Operazione_ID<>'') "
            ' filtroPerLetturaAPP &= " AND Ricette_Operazioni.Raccoglitore_Cod=0 "
            filtroPerLetturaAPP &= " AND Ricette_Operazioni.Invia_App=1 "

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                1, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", "", objParametri_Utenti)

            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                filtroPerLetturaAPP &= " AND Ricette_Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            Dt_Ricette = objRicette_R.Ricetta_Leggi_dtAPP(Piva, filtroPerLetturaAPP, objParametri)

            'Leggo i dati della Ricette x note

            Dim objRicetteXnote_R As New AgronicaCoreContabDAL.RicettexNote_R

            Dt_RicetteXNote = objRicetteXnote_R.Leggi(
                0,
                0,
                0,
                Validita_Inizio,
                Validita_Fine,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                filtroPerLetturaAPP,
                "",
                objParametri,
                True
            )


            'Leggo i dati della Ricette_Operazioni
            Dim objRicette_Operazioni_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R

            Dt_Operazioni = objRicette_Operazioni_R.Leggi(
                0,
                0,
                Lav_Cod,
                Gru_Op,
                Validita_Inizio,
                Validita_Fine,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                filtroPerLetturaAPP,
                "",
                objParametri,
                True
             )

            'Leggo i dati della Ricette_Dettagli
            Dim objRicette_Dettagli_R As New AgronicaCoreContabDAL.Ricette_Dettagli_R

            Dt_Dettagli = objRicette_Dettagli_R.Leggi(
                0,
                0,
                0,
                "",
                0,
                0,
                0,
                0,
                Validita_Inizio,
                Validita_Fine,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                filtroPerLetturaAPP,
                "",
                objParametri,
                True
             )


            'Leggo i dati della Ricette_Dettaglio_tecnico
            Dim objRicette_Dettaglio_tecnico_R As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R

            Dt_DettaglioTecnico = objRicette_Dettaglio_tecnico_R.Leggi(
                0,
                0,
                0,
                0,
                0,
                Validita_Inizio,
                Validita_Fine,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                filtroPerLetturaAPP,
                "",
                objParametri,
                True
            )

            'Leggo i dati della Ricette_Destinazioni
            Dim objRicette_Destinazioni_R As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

            Dt_Destinazioni = objRicette_Destinazioni_R.Leggi(
                0,
                0,
                0,
                0,
                0,
                "",
                0,
                0,
                0,
                Validita_Inizio,
                Validita_Fine,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                filtroPerLetturaAPP,
                "",
                objParametri,
                True)


        Catch ex As Exception

            rval = 0
            ErrMSG = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    '============================================================================
    Public Sub Ricette_Operazioni_Leggi(ByRef ErrMSG As String,
                                        ByVal Ricetta_Cod As Integer,
                                        ByVal Ricetta_Operazione_Cod As Integer,
                                        ByVal Lav_Cod As Integer,
                                        ByVal Gru_Op As Integer,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef Dt_Operazioni As DataTable,
                                        ByRef Dt_Dettagli As DataTable,
                                        ByRef Dt_Destinazioni As DataTable,
                                        ByRef objParametri As AgronicaCoreParametri)

        'Leggo i dati della Ricette_Operazioni
        Dim objRicette_Operazioni_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R

        Dt_Operazioni = objRicette_Operazioni_R.Leggi(Ricetta_Cod,
                                                        Ricetta_Operazione_Cod,
                                                        Lav_Cod,
                                                        Gru_Op,
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        xFiltroAggiuntivo,
                                                        "",
                                                        objParametri)
        objRicette_Operazioni_R = Nothing

        'Leggo i dati della Ricette_Dettagli
        Dim objRicette_Dettagli_R As New AgronicaCoreContabDAL.Ricette_Dettagli_R

        Dt_Dettagli = objRicette_Dettagli_R.Leggi(Ricetta_Cod,
                                                  Ricetta_Operazione_Cod,
                                                  0,
                                                  "",
                                                  0,
                                                  0,
                                                  0,
                                                  0,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "", "",
                                                  objParametri)

        objRicette_Dettagli_R = Nothing

        'Leggo i dati della Ricette_Destinazioni
        Dim objRicette_Destinazioni_R As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Dt_Destinazioni = objRicette_Destinazioni_R.Leggi(Ricetta_Cod,
                                                  Ricetta_Operazione_Cod,
                                                  0,
                                                  0,
                                                  0,
                                                  "",
                                                  0,
                                                  0,
                                                  0,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                  "", "",
                                                  objParametri)

        objRicette_Destinazioni_R = Nothing

    End Sub

    '============================================================================
    Public Function Ricetta_Operazioni_Leggi(ByVal Ricetta_Cod As Integer,
                                             ByVal Ricetta_Operazione_Cod As Integer,
                                             ByVal Lav_Cod As Integer,
                                             ByVal Gru_Op As Integer,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByVal ForDelete As Boolean,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal RicettaDettagliOrderBy As String = ""
                                             ) As String

        Dim NomeRoutine As String = "ContabBIZ.Ricette_Operazioni_R.Ricetta_Operazione_Leggi()"

        Dim RisultatoFunzione As String = String.Empty
        Dim MessaggioErrore As String = String.Empty
        Dim FlagConnessioneLocale As Boolean = False



        Dim XmlDoc As XmlDocument

        Dim XmlDatiRicettaOperazioni As XmlElement
        Dim XmlRicettaOperazione As XmlElement

        Dim XmlDatiRicettaDettaglioTecnico As XmlElement    'IXMLDOMElement
        Dim XmlRicettaDettaglioTecnico As XmlElement        'IXMLDOMElement

        Dim XmlDatiRicettaDettaglio As XmlElement           'IXMLDOMElement
        Dim XmlRicetta_Dett_Tecnico As XmlElement
        Dim XmlRicetta_Ricetta_Destinazione As XmlElement



        Dim DtOperazioni As DataTable


        Dim i As Integer
        Dim j As Integer = 0


        Try

            '------------------------------


            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            If objParametri.objConnessione.State = ConnectionState.Closed Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            '------------------------------

            'Leggo i dati della Ricette_Operazioni
            Dim objRicette_Operazioni_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R

            DtOperazioni = objRicette_Operazioni_R.Leggi(Ricetta_Cod,
                                                            Ricetta_Operazione_Cod,
                                                            Lav_Cod,
                                                            Gru_Op,
                                                            Validita_Inizio,
                                                            Validita_Fine,
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "",
                                                            objParametri)

            objRicette_Operazioni_R = Nothing

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtOperazioni.Rows.Count > 0 Then

                XmlDoc = New XmlDocument

                XmlDatiRicettaOperazioni = XmlDoc.CreateElement("DatiRicetta_Operazioni")

                For i = 0 To DtOperazioni.Rows.Count - 1

                    '----- < RICETTA_OPERAZIONI > -----
                    XmlRicettaOperazione = XmlDoc.CreateElement("Ricetta_Operazione")

                    With XmlRicettaOperazione
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("ricetta_superuser", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_SuperUser")))
                        .SetAttribute("ricetta_cod", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Cod")))
                        .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Operazione_Cod")))
                        .SetAttribute("lav_cod", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Lav_Cod")))
                        .SetAttribute("lav_des", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Lav_Des")))
                        .SetAttribute("num_protocollo", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Num_Protocollo")))
                        .SetAttribute("id_rcdpi", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Id_Rcdpi")))
                        .SetAttribute("extra_int", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Extra_Int")))
                        .SetAttribute("mezzo", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Mezzo")))
                        .SetAttribute("ricetta_operazione_des", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Operazione_Des")))
                        .SetAttribute("note", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Note")))
                        .SetAttribute("inviato", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Inviato")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Username_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Validita_Fine")))
                        .SetAttribute("gru_op", Agro_SQL_Load(DtOperazioni.Rows(i).Item("gru_op")))
                        .SetAttribute("costo", Agro_SQL_Load(DtOperazioni.Rows(i).Item("costo")))
                        .SetAttribute("noleggio_passivo", Agro_SQL_Load(DtOperazioni.Rows(i).Item("noleggio_passivo")))
                        .SetAttribute("id_tp_fer", Agro_SQL_Load(DtOperazioni.Rows(i).Item("id_tp_fer")))
                        .SetAttribute("em_cod", Agro_SQL_Load(DtOperazioni.Rows(i).Item("em_cod")))
                        .SetAttribute("eff_perc", Agro_SQL_Load(DtOperazioni.Rows(i).Item("eff_perc")))
                        .SetAttribute("disciplinare_pubblicoprivato", Agro_SQL_Load(DtOperazioni.Rows(i).Item("Disciplinare_PubblicoPrivato")))
                        .SetAttribute("w_anagrafica_stati_cod", Agro_SQL_Load(DtOperazioni.Rows(i).Item("w_anagrafica_stati_cod")))
                        .SetAttribute("ricetta_operazione_cod_rif", Agro_SQL_Load(DtOperazioni.Rows(i).Item("ricetta_operazione_cod_rif")))
                        .SetAttribute("app_ricetta_operazione_id", Agro_SQL_Load(DtOperazioni.Rows(i).Item("app_ricetta_operazione_id")))
                        .SetAttribute("raccoglitore_cod", Agro_SQL_Load(DtOperazioni.Rows(i).Item("raccoglitore_cod")))
                        .SetAttribute("invia_app", Agro_SQL_Load(DtOperazioni.Rows(i).Item("invia_app")))
                        .SetAttribute("invia_hubiot", Agro_SQL_Load(DtOperazioni.Rows(i).Item("invia_hubiot")))
                    End With


                    '######################################################
                    '##########  RICETTE_DETTAGLIO_TECNICO  ###############
                    '######################################################

                    Dim objRicettaDettaglioTecnico As AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
                    Dim DtRicettaDettaglioTecnico As DataTable


                    objRicettaDettaglioTecnico = New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R

                    DtRicettaDettaglioTecnico = objRicettaDettaglioTecnico.Leggi(CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Cod"))),
                                                                                 CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Operazione_Cod"))),
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 AGRODATAINIZIO,
                                                                                 AGRODATAFINE,
                                                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                " Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = 0 ",
                                                                                "",
                                                                                objParametri)

                    Dim t As Integer

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRicettaDettaglioTecnico.Rows.Count > 0 Then

                        XmlDatiRicettaDettaglioTecnico = XmlDoc.CreateElement("DatiRicetta_Dettagli_Tecnici")

                        For t = 0 To DtRicettaDettaglioTecnico.Rows.Count - 1

                            '----- < RICETTA_DETTAGLIO_TECNICO > -----
                            XmlRicettaDettaglioTecnico = XmlDoc.CreateElement("Ricetta_Dettaglio_Tecnico")

                            With XmlRicettaDettaglioTecnico
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Ricetta_Cod")))
                                .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Ricetta_Operazione_Cod")))
                                .SetAttribute("ricetta_dettaglio_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Ricetta_Dettaglio_Cod")))
                                .SetAttribute("miscela_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Miscela_Cod")))
                                .SetAttribute("ricetta_tecnico_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Ricetta_Tecnico_Cod")))
                                .SetAttribute("qta_ril", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Qta_Ril")))
                                .SetAttribute("dett_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Dett_Cod")))
                                .SetAttribute("av_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Av_Cod")))
                                .SetAttribute("av_gru", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Av_Gru")))
                                .SetAttribute("av_des_vol", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Av_Des_Vol")))
                                .SetAttribute("av_des_lat", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Av_Des_Lat")))
                                .SetAttribute("av_gru_des", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Av_Gru_Des")))
                                .SetAttribute("av_gru_des_lat", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Av_Gru_Des_Lat")))
                                .SetAttribute("dose", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Dose")))
                                .SetAttribute("parziale", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Parziale")))
                                .SetAttribute("nitrati", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Nitrati")))
                                .SetAttribute("freatimetro", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Freatimetro")))
                                .SetAttribute("inn1_data", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Inn1_Data")))
                                .SetAttribute("inn2_data", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Inn2_Data")))
                                .SetAttribute("inn3_data", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Inn3_Data")))
                                .SetAttribute("inn4_data", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Inn4_Data")))
                                .SetAttribute("ditta_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Ditta_Cod")))
                                .SetAttribute("sigla_av", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Sigla_Av")))
                                .SetAttribute("trap_num", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Trap_Num")))
                                .SetAttribute("id_insetto", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Id_Insetto")))
                                .SetAttribute("ff_classe", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("FF_Classe")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Inviato")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("validita_fine")))
                                .SetAttribute("n", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("n")))
                                .SetAttribute("p", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("p")))
                                .SetAttribute("k", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("k")))
                                .SetAttribute("mg", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("mg")))
                                .SetAttribute("apportoxha", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("ApportoxHa")))
                                .SetAttribute("nnettoxha", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("NnettoxHa")))
                                .SetAttribute("nutilexha", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("NutilexHa")))
                                .SetAttribute("soglia_cod", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("soglia_cod")))
                                .SetAttribute("soglia_des", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("soglia_des")))
                                .SetAttribute("soglia_quantita", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("soglia_quantita")))
                                .SetAttribute("efficienza", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("efficienza")))
                                .SetAttribute("Ricette_Dettaglio_Tecnico_graphickey", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("Ricette_Dettaglio_Tecnico_graphickey")))
                                .SetAttribute("piezo1", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("piezo1")))
                                .SetAttribute("piezo2", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("piezo2")))
                                .SetAttribute("piezo3", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("piezo3")))
                                .SetAttribute("piezo4", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("piezo4")))
                                .SetAttribute("cu", Agro_SQL_Load(DtRicettaDettaglioTecnico.Rows(t).Item("cu")))


                            End With

                            XmlDatiRicettaDettaglioTecnico.AppendChild(XmlRicettaDettaglioTecnico)
                            '----- < / RICETTA_DETTAGLIO_TECNICO > -----

                        Next

                        XmlRicettaOperazione.AppendChild(XmlDatiRicettaDettaglioTecnico)
                        '----- < / DATIRICETTA_DETTAGLIO_TECNICO > -----

                    End If

                    '--------------------------------------------



                    ''#############################################
                    ''##########  RICETTE_DETTAGLI  ###############
                    ''#############################################

                    Dim objRicettaDettaglio As New AgronicaCoreContabDAL.Ricette_Dettagli_R
                    Dim DtRicettaDettaglio As DataTable

                    DtRicettaDettaglio = objRicettaDettaglio.Leggi(CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Cod"))),
                                                                   CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Operazione_Cod"))),
                                                                   0, "",
                                                                   0, 0,
                                                                   0, 0,
                                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                   "", RicettaDettagliOrderBy,
                                                                   objParametri)


                    XmlDatiRicettaDettaglio = XmlDoc.CreateElement("DatiRicetta_Dettagli")

                    Dim XmlRicetta_Dettagli As XmlElement

                    If DtRicettaDettaglio.Rows.Count > 0 Then

                        For t = 0 To DtRicettaDettaglio.Rows.Count - 1

                            XmlRicetta_Dettagli = XmlDoc.CreateElement("Ricetta_Dettaglio")

                            With XmlRicetta_Dettagli
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Cod")))
                                .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Operazione_Cod")))
                                .SetAttribute("ricetta_dettaglio_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Dettaglio_Cod")))
                                .SetAttribute("miscela_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Miscela_Cod")))
                                .SetAttribute("elem_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Elem_Cod")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Pro_Cod")))
                                .SetAttribute("fer_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("fer_des")))
                                .SetAttribute("fr_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("fr_des")))
                                .SetAttribute("trap_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("trap_des")))
                                .SetAttribute("ins_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("ins_des")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Mat_Cod")))
                                .SetAttribute("mat_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Mat_des")))
                                .SetAttribute("mac_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Mac_des")))
                                .SetAttribute("rag_soc", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("rag_soc")))
                                .SetAttribute("nome", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("nome")))
                                .SetAttribute("cognome", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("cognome")))
                                .SetAttribute("udm_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Udm_Cod")))
                                .SetAttribute("udm_des", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Udm_des")))
                                .SetAttribute("udm_sim", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("UDM_SIM")))
                                .SetAttribute("qta", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Qta")))
                                .SetAttribute("extra_int", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Extra_Int")))
                                .SetAttribute("datainvio", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Datainvio")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("validita_fine")))
                                .SetAttribute("prezzo_unitario", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("prezzo_unitario")))
                                .SetAttribute("cau_mov", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("cau_mov")))
                                .SetAttribute("tempocarenza", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("tempocarenza")))
                                .SetAttribute("doseetichetta", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("doseetichetta")))
                                .SetAttribute("principiattivi", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("principiattivi")))
                                .SetAttribute("principiattivipesi", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("principiattivipesi")))
                                .SetAttribute("buffer", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("buffer")))
                                .SetAttribute("classitossicologiche", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("classitossicologiche")))
                                .SetAttribute("doseetichetta_value", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("doseetichetta_value")))
                                .SetAttribute("turno_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("turno_cod")))
                                .SetAttribute("id_attivita", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("id_attivita")))
                                .SetAttribute("lotto", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("lotto")))
                                .SetAttribute("qualifica_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("qualifica_cod")))
                                .SetAttribute("tariffa_cod", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("tariffa_cod")))
                                .SetAttribute("qta_extra", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("qta_extra")))
                                .SetAttribute("qta_extra_totale", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("qta_extra_totale")))
                                .SetAttribute("udm_cod_extra", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("udm_cod_extra")))
                                .SetAttribute("mezzo_det", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("mezzo_det")))
                                .SetAttribute("extra_str", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("extra_str")))
                                .SetAttribute("principiattivipercabb", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("PrincipiAttiviPercAbb")))
                                .SetAttribute("polverulento", Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("polverulento")))
                            End With


                            ''########################################################
                            ''##########  RICETTE_DETTAGLIO_TECNICO_2  ###############
                            ''########################################################

                            Dim objRicetta_Dett_Tecnico As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
                            Dim dtRicetta_Dett_Tecnico As DataTable

                            dtRicetta_Dett_Tecnico = objRicetta_Dett_Tecnico.Leggi(
                                                  CLng(Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Cod"))),
                                                  CLng(Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Operazione_Cod"))),
                                                  CLng(Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Dettaglio_Cod"))),
                                                  0,
                                                  0,
                                                  AGRODATAINIZIO,
                                                  AGRODATAFINE,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "",
                                                  "",
                                                  objParametri)

                            'Se ottengo almeno un risultato, creo la struttura XML
                            If dtRicetta_Dett_Tecnico.Rows.Count > 0 Then

                                'Effettuo un ciclo sui Dettagli Tecnici del Moimento Dettaglio
                                For j = 0 To dtRicetta_Dett_Tecnico.Rows.Count - 1

                                    '----- < DETTAGLIO TECNICO > -----
                                    XmlRicetta_Dett_Tecnico = XmlDoc.CreateElement("Ricetta_Dettaglio_Tecnico_2")

                                    With XmlRicetta_Dett_Tecnico
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("ricetta_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Ricetta_Cod")))
                                        .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Ricetta_Operazione_Cod")))
                                        .SetAttribute("ricetta_dettaglio_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Ricetta_Dettaglio_Cod")))
                                        .SetAttribute("miscela_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Miscela_Cod")))
                                        .SetAttribute("ricetta_tecnico_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Ricetta_Tecnico_Cod")))
                                        .SetAttribute("qta_ril", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Qta_Ril")))
                                        .SetAttribute("dett_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Dett_Cod")))
                                        .SetAttribute("av_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Av_Cod")))
                                        .SetAttribute("av_gru", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Av_Gru")))
                                        .SetAttribute("av_des_vol", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Av_Des_Vol")))
                                        .SetAttribute("av_des_lat", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Av_Des_Lat")))
                                        .SetAttribute("av_gru_des", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Av_Gru_Des")))
                                        .SetAttribute("av_gru_des_lat", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Av_Gru_Des_Lat")))
                                        .SetAttribute("dose", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Dose")))
                                        .SetAttribute("parziale", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Parziale")))
                                        .SetAttribute("nitrati", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Nitrati")))
                                        .SetAttribute("freatimetro", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Freatimetro")))
                                        .SetAttribute("inn1_data", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Inn1_Data")))
                                        .SetAttribute("inn2_data", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Inn2_Data")))
                                        .SetAttribute("inn3_data", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Inn3_Data")))
                                        .SetAttribute("inn4_data", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Inn4_Data")))
                                        .SetAttribute("ditta_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Ditta_Cod")))
                                        .SetAttribute("sigla_av", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Sigla_Av")))
                                        .SetAttribute("trap_num", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Trap_Num")))
                                        .SetAttribute("id_insetto", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Id_Insetto")))
                                        .SetAttribute("ff_classe", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("FF_Classe")))
                                        .SetAttribute("inviato", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Inviato")))
                                        .SetAttribute("data_creazione", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Data_Creazione")))
                                        .SetAttribute("data_modifica", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Data_Modifica")))
                                        .SetAttribute("username_creazione", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Username_Creazione")))
                                        .SetAttribute("username_modifica", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Username_Modifica")))
                                        .SetAttribute("validita_inizio", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("validita_inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("validita_fine")))
                                        .SetAttribute("n", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("n")))
                                        .SetAttribute("p", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("p")))
                                        .SetAttribute("k", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("k")))
                                        .SetAttribute("mg", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("mg")))
                                        .SetAttribute("apportoxha", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("ApportoxHa")))
                                        .SetAttribute("nnettoxha", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("NnettoxHa")))
                                        .SetAttribute("nutilexha", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("NutilexHa")))
                                        .SetAttribute("soglia_cod", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("soglia_cod")))
                                        .SetAttribute("soglia_des", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("soglia_des")))
                                        .SetAttribute("soglia_quantita", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("soglia_quantita")))
                                        .SetAttribute("efficienza", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("efficienza")))
                                        .SetAttribute("Ricette_Dettaglio_Tecnico_graphickey", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("Ricette_Dettaglio_Tecnico_graphickey")))
                                        .SetAttribute("piezo1", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("piezo1")))
                                        .SetAttribute("piezo2", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("piezo2")))
                                        .SetAttribute("piezo3", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("piezo3")))
                                        .SetAttribute("piezo4", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("piezo4")))
                                        .SetAttribute("cu", Agro_SQL_Load(dtRicetta_Dett_Tecnico.Rows(j).Item("cu")))

                                    End With

                                    XmlRicetta_Dettagli.AppendChild(XmlRicetta_Dett_Tecnico)
                                    '----- < / DETTAGLIO TECNICO > -----

                                Next

                            End If


                            ''########################################################
                            ''##########  RICETTE_DESTINAZIONE  ######################
                            ''########################################################

                            Dim objRicetta_Dest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                            Dim dtRicetta_Dest As DataTable

                            dtRicetta_Dest = objRicetta_Dest.Leggi(
                                                 CLng(Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Cod"))),
                                                 CLng(Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Operazione_Cod"))),
                                                 CLng(Agro_SQL_Load(DtRicettaDettaglio.Rows(t).Item("Ricetta_Dettaglio_Cod"))),
                                                 0,
                                                 0,
                                                 "",
                                                 0,
                                                 0,
                                                 0,
                                                 AGRODATAINIZIO,
                                                 AGRODATAFINE,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "",
                                                 "",
                                                 objParametri)

                            'Se ottengo almeno un risultato, creo la struttura XML
                            If dtRicetta_Dest.Rows.Count > 0 Then

                                'Effettuo un ciclo sui Dettagli Tecnici del Moimento Dettaglio
                                For j = 0 To dtRicetta_Dest.Rows.Count - 1

                                    '----- < DETTAGLIO TECNICO > -----
                                    XmlRicetta_Ricetta_Destinazione = XmlDoc.CreateElement("Ricetta_Destinazione")

                                    With XmlRicetta_Ricetta_Destinazione
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("ricetta_superuser", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Ricetta_SuperUser")))
                                        .SetAttribute("ricetta_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Ricetta_Cod")))
                                        .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Ricetta_Operazione_Cod")))
                                        .SetAttribute("ricetta_dettaglio_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Ricetta_Dettaglio_Cod")))
                                        .SetAttribute("ricetta_destinazione_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Ricetta_Destinazione_Cod")))
                                        .SetAttribute("programmazione_entita_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Programmazione_Entita_Cod")))
                                        .SetAttribute("piva", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Piva")))
                                        .SetAttribute("sa_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Sa_Cod")))
                                        .SetAttribute("appezza", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Appezza")))
                                        .SetAttribute("id_reg", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Id_Reg")))
                                        .SetAttribute("qta", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Qta")))
                                        .SetAttribute("qta2", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Qta2")))
                                        .SetAttribute("quotadistribuzione", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("QuotaDistribuzione")))
                                        .SetAttribute("inviato", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Inviato")))
                                        .SetAttribute("data_creazione", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Data_Creazione")))
                                        .SetAttribute("data_modifica", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Data_Modifica")))
                                        .SetAttribute("username_creazione", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Username_Creazione")))
                                        .SetAttribute("username_modifica", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("Username_Modifica")))
                                        .SetAttribute("validita_inizio", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("validita_inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("validita_fine")))
                                        .SetAttribute("tipo_destinazione", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("tipo_destinazione")))
                                        .SetAttribute("magazzinoesterno_cod", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("magazzinoesterno_cod")))
                                        .SetAttribute("magazzinoesterno_des", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("magazzinoesterno_des")))
                                        .SetAttribute("magazzinoesterno_dettagli", Agro_SQL_Load(dtRicetta_Dest.Rows(j).Item("magazzinoesterno_dettagli")))
                                    End With

                                    XmlRicetta_Dettagli.AppendChild(XmlRicetta_Ricetta_Destinazione)
                                    '----- < / DETTAGLIO TECNICO > -----

                                Next

                            End If

                            XmlDatiRicettaDettaglio.AppendChild(XmlRicetta_Dettagli)

                        Next

                        XmlRicettaOperazione.AppendChild(XmlDatiRicettaDettaglio)

                    End If

                    ''--------------------------------------------



                    '#####################################
                    '##########  RicettexNote  ###########
                    '#####################################

                    Dim XmlDatiRicettaxNote As XmlElement           'IXMLDOMElement
                    Dim XmlRicettaxNote As XmlElement               'IXMLDOMElement
                    Dim XmlDatiRicettaxAgenda As XmlElement           'IXMLDOMElement
                    Dim XmlRicettaxAgenda As XmlElement               'IXMLDOMElement


                    Dim objRicettexNote As New AgronicaCoreContabDAL.RicettexNote_R
                    Dim DtRicettexNote As DataTable

                    DtRicettexNote = objRicettexNote.Leggi(CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Cod"))),
                                                                   CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Operazione_Cod"))),
                                                                   0,
                                                                    AGRODATAINIZIO,
                                                                    AGRODATAFINE,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRicettexNote.Rows.Count > 0 Then

                        XmlDatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

                        For j = 0 To DtRicettexNote.Rows.Count - 1

                            '----- < RICETTAXNOTE_2 > -----
                            XmlRicettaxNote = XmlDoc.CreateElement("RicettaxNote_2")

                            With XmlRicettaxNote
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_superuser", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Ricetta_Superuser")))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Ricetta_Cod")))
                                .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Ricetta_Operazione_Cod")))
                                .SetAttribute("nota_cod", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Nota_Cod")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Inviato")))

                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("validita_fine")))
                            End With

                            XmlDatiRicettaxNote.AppendChild(XmlRicettaxNote)
                            '----- < / RICETTAXCULTIVAR > -----

                        Next
                        'Loop

                        XmlRicettaOperazione.AppendChild(XmlDatiRicettaxNote)

                    End If



                    '###########################################
                    '##########  RicettexAgenda  ###############
                    '###########################################

                    Dim objRicettexAgenda As New AgronicaCoreContabDAL.RicettexAgenda_R
                    Dim DtRicettexAgenda As DataTable

                    DtRicettexAgenda = objRicettexAgenda.Leggi(CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Cod"))),
                                                                   CInt(Agro_SQL_Load(DtOperazioni.Rows(i).Item("Ricetta_Operazione_Cod"))),
                                                                   0,
                                                                    AGRODATAINIZIO,
                                                                    AGRODATAFINE,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri)



                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRicettexAgenda.Rows.Count > 0 Then

                        XmlDatiRicettaxAgenda = XmlDoc.CreateElement("DatiRicettaxAgenda_2")

                        For j = 0 To DtRicettexAgenda.Rows.Count - 1

                            '----- < RICETTAXAGENDA_2 > -----
                            XmlRicettaxAgenda = XmlDoc.CreateElement("RicettaxAgenda_2")

                            With XmlRicettaxAgenda
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_superuser", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Ricetta_Superuser")))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Ricetta_Cod")))
                                .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Ricetta_Operazione_Cod")))
                                .SetAttribute("id_agenda", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Inviato")))

                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("validita_fine")))
                            End With

                            XmlDatiRicettaxAgenda.AppendChild(XmlRicettaxAgenda)
                            '----- < / RICETTAXCULTIVAR > -----

                        Next
                        'Loop

                        XmlRicettaOperazione.AppendChild(XmlDatiRicettaxAgenda)

                    End If


                    XmlDatiRicettaOperazioni.AppendChild(XmlRicettaOperazione)

                    '----- < / RICETTA_OPERAZIONI > -----

                Next

                RisultatoFunzione = XmlDatiRicettaOperazioni.OuterXml

            End If


        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        Finally

            'Pulizia
            'objUtentixImprese = Nothing
            'objImpresexCodici = Nothing
            'objImpresexIndirizzi = Nothing
            '
            '
            '
            '

            If FlagConnessioneLocale Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()
                End If
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function

    '============================================================================
    Public Sub Leggi_Operazioni_Da_Programmazione_Entita_Cod(ByVal Ricetta_Cod As Integer,
                                                             ByVal Ricetta_Operazione_Cod As Integer,
                                                             ByVal Ricetta_Dettaglio_Cod As Int32,
                                                             ByVal Ricetta_Destinazione_Cod As Int32,
                                                             ByVal Programmazione_Entita_Cod As Int32,
                                                             ByVal Validita_Inizio As Date,
                                                             ByVal Validita_Fine As Date,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef Dt_Operazioni As DataTable,
                                                             ByRef Dt_Dettagli As DataTable,
                                                             ByRef Dt_Destinazioni As DataTable,
                                                             ByRef objParametri As AgronicaCoreParametri)

        Dim strFiltro_Dettagli As String = ""
        Dim strFiltro_Operazioni As String = ""
        Dim i As Integer

        'Leggo i dati della Ricette_Destinazioni
        Dim objRicette_Destinazioni_R As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Dt_Destinazioni = objRicette_Destinazioni_R.Leggi(
                                                  Ricetta_Cod,
                                                  Ricetta_Operazione_Cod,
                                                  Ricetta_Dettaglio_Cod,
                                                  Ricetta_Destinazione_Cod,
                                                  Programmazione_Entita_Cod,
                                                  "",
                                                  0,
                                                  0,
                                                  0,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                  "", "",
                                                  objParametri)

        objRicette_Destinazioni_R = Nothing

        If Dt_Destinazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Destinazioni.Rows.Count - 1
                strFiltro_Dettagli &= " Ricetta_Dettaglio_Cod=" & Dt_Destinazioni.Rows(i).Item("Ricetta_Dettaglio_Cod") & " OR "
                strFiltro_Operazioni &= " Ricetta_Operazione_Cod=" & Dt_Destinazioni.Rows(i).Item("Ricetta_Operazione_Cod") & " OR "
            Next
            If strFiltro_Dettagli <> "" Then
                strFiltro_Dettagli = Left(strFiltro_Dettagli, strFiltro_Dettagli.Length - 3)
                strFiltro_Dettagli = " (" & strFiltro_Dettagli & ")"
            End If
            If strFiltro_Operazioni <> "" Then
                strFiltro_Operazioni = Left(strFiltro_Operazioni, strFiltro_Operazioni.Length - 3)
                strFiltro_Operazioni = " (" & strFiltro_Operazioni & ")"
            End If

            'Leggo i dati della Ricette_Dettagli
            Dim objRicette_Dettagli_R As New AgronicaCoreContabDAL.Ricette_Dettagli_R

            Dt_Dettagli = objRicette_Dettagli_R.Leggi(Ricetta_Cod,
                                                      Ricetta_Operazione_Cod,
                                                      0,
                                                      "",
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      Validita_Inizio,
                                                      Validita_Fine,
                                                      enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                      strFiltro_Operazioni,
                                                      "",
                                                      objParametri)

            objRicette_Dettagli_R = Nothing


            'Leggo i dati della Ricette_Operazioni
            Dim objRicette_Operazioni_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R

            Dt_Operazioni = objRicette_Operazioni_R.Leggi(Ricetta_Cod,
                                                          Ricetta_Operazione_Cod,
                                                          0,
                                                          0,
                                                            Validita_Inizio,
                                                            Validita_Fine,
                                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            strFiltro_Operazioni,
                                                            "",
                                                            objParametri)

            objRicette_Operazioni_R = Nothing

        End If

    End Sub

    Public Function XML_GeneraStringa_Ricetta_Operazione(tipo_operazione As enum_TipoOperazioneDB,
                                                         ricetta_cod As String,
                                                         ricetta_operazione_cod As String,
                                                         tipo_operazione_agenda As enum_Tipo_Operazione_Agenda,
                                                         progressivo_gias As Integer,
                                                         objParametri_Server As AgronicaCoreParametri,
                                                         strAgenda As String,
                                                            ByRef Lav_Cod As Integer,
                                                            ByRef Des_Lib As String,
                                                            ByRef Data As Date,
                                                            ByRef Note As String,
                                                         Optional Data_Creazione As Date = AGRODATAINIZIO,
                                                         Optional Username_Creazione As String = "",
                                                         Optional guidRicetta As String = ""
                                                        ) As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDocAgenda As New XmlDocument
        Dim XML_DatiAgenda As XmlElement
        Dim XML_Agenda As XmlElement
        Dim XML_DatiNote As XmlElement
        Dim XML_Nota As XmlElement
        Dim XMLs_Note As XmlNodeList
        Dim XML_DatiMovimenti As XmlElement
        Dim XML_Movimento As XmlElement
        Dim XMLs_Movimento As XmlNodeList
        Dim XML_DatiMovDettagliTecnici As XmlElement
        Dim XML_MovimentoDettaglioTecnico As XmlElement
        Dim XMLs_MovimentoDettaglioTecnico As XmlNodeList
        Dim XMLs_Movimento_Dettaglio_Tecnico_2 As XmlNodeList
        Dim XML_DatiMovimentiDettagli As XmlElement
        Dim XML_MovimentoDettaglio As XmlElement
        Dim XMLs_MovimentoDettaglio As XmlNodeList
        Dim XML_MovimentoDestinazione As XmlElement
        Dim XMLs_MovimentoDestinazione As XmlNodeList

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument
        Dim XmlDoc3 As New XmlDocument

        Dim XML_Operazione As XmlElement
        Dim XML_RicettaxNote As XmlElement
        Dim XML_DatiRicettaxAgenda As XmlElement
        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagliTecnici As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement = Nothing
        Dim XML_RicettaDettaglio As XmlElement

        Dim strOperazione As String = ""
        Dim strRicettaxAgenda As String = ""
        Dim strRicettaDettagli As String = ""
        Dim strRicettaDettaglio As String = ""
        Dim strRicettaDettagliTecnici As String = ""
        Dim strRicettaDettaglioTecnico As String = ""
        Dim strRicettaDestinazioni As String = ""
        Dim strRicettaDestinazione As String = ""
        Dim strRicettaDettagliCostiAccessori As String = ""
        Dim strRicettaDettaglioCostiAccessori As String = ""
        Dim strRicettaDestinazioniCostiAccessori As String = ""
        Dim strRicettaDestinazioneCostiAccessori As String = ""
        Dim strRicetta As String = ""

        Dim ArrayPiva(0) As String
        Dim ArraySaCod(0) As Integer
        Dim ArrayAppezza(0) As Integer
        Dim ArrayIdReg(0) As Integer
        Dim N_Array As Integer = 0
        Dim N_Cul_Cod As Integer = 0

        Dim i, j, n As Integer

        Dim Piva As String
        Dim Sa_Cod As Integer

        Dim Id_Agenda As Integer
        Dim Cau_Mov As String
        Dim CodDisciplinare As Integer
        Dim Id_Rcdpi As Integer
        Dim Extra_Int As Integer
        Dim Mezzo As Integer
        Dim DisciplinarePP As Integer = 0
        Dim Validita_Fine As Date = #12/31/2100#

        Dim idAgendaScaricoDaAzPadre As Integer
        Dim Piva_Magazzino As String
        Dim Sa_Cod_Magazzino As Integer
        Dim Id_Destinazione_Magazzino As Integer

        Dim Extra_Str As String
        Dim TempoCarenza As Integer
        Dim DoseEtichetta As String
        Dim DoseEtichetta_Value As String
        Dim PrincipiAttivi As String
        Dim PrincipiAttiviPercAbb As String = ""
        Dim PrincipiAttiviPesi As String = ""
        Dim ClassiTossicologiche As String
        Dim Polverulento As Integer
        Dim Buffer As String

        Dim objXml As New AgronicaCoreXML.XML_Contab

        Dim Dt_Impianti As New DataTable

        Dim frm_BaseCode, frm_TopCode As Integer
        Call Calcola_BaseCode_TopCode(frm_BaseCode, frm_TopCode, progressivo_gias)

        If tipo_operazione <> enum_TipoOperazioneDB.Modifica Then
            ricetta_operazione_cod = 0
        End If

        'If Not Cul_Cod Is Nothing Then
        '    N_Cul_Cod = UBound(Cul_Cod)
        'End If

        '------------------------------------------------
        '----- Recupero i dati dalla stringa
        '------------------------------------------------

        XmlDocAgenda.LoadXml(strAgenda)

        '----- Tag DatiAgenda

        XML_DatiAgenda = XmlDocAgenda.SelectSingleNode("DatiAgenda")

        '----- Tag Agenda

        XML_Agenda = XML_DatiAgenda.SelectSingleNode("Agenda")

        Piva = CStr(XML_Agenda.GetAttribute("piva"))
        Sa_Cod = CInt(XML_Agenda.GetAttribute("sa_cod"))

        Lav_Cod = CStr(XML_Agenda.GetAttribute("lav_cod"))
        Des_Lib = CStr(XML_Agenda.GetAttribute("des_lib"))
        Id_Agenda = CInt(XML_Agenda.GetAttribute("id_agenda"))

        Dim raccoglitore_cod As Integer = 0
        If XML_Agenda.HasAttribute("raccoglitore_cod") AndAlso Not String.IsNullOrEmpty(XML_Agenda.GetAttribute("raccoglitore_cod")) Then
            raccoglitore_cod = CInt(XML_Agenda.GetAttribute("raccoglitore_cod"))
        End If

        Dim invia_app As Integer = 0
        If XML_Agenda.HasAttribute("invia_app") AndAlso Not String.IsNullOrEmpty(XML_Agenda.GetAttribute("invia_app")) Then
            invia_app = CInt(XML_Agenda.GetAttribute("invia_app"))
        End If

        ' FEDE aggiungere NOTE

        XML_DatiNote = XML_Agenda.SelectSingleNode("DatiNote")

        Dim strNote = ""

        If XML_DatiNote IsNot Nothing Then
            XMLs_Note = XML_DatiNote.GetElementsByTagName("Nota")

            Dim Nota_Cod As Integer

            XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

            For i = 0 To XMLs_Note.Count - 1

                XML_Nota = XMLs_Note.Item(i)

                Nota_Cod = XML_Nota.GetAttribute("nota_cod")

                strRicetta = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                          objParametri_Server.PivaSuperUser,
                                          ricetta_cod, ricetta_operazione_cod, Nota_Cod,
                                          XML_Nota.GetAttribute("validita_inizio"),
                                          XML_Nota.GetAttribute("validita_fine"), guidRicetta)

                XmlDoc.LoadXml(strRicetta)

                XML_RicettaxNote = XmlDoc.SelectSingleNode("RicettaxNote_2")

                XML_DatiRicettaxNote.AppendChild(XML_RicettaxNote)

            Next
            strNote = XML_DatiRicettaxNote.OuterXml

        End If


        'xListaDatiRicettaxNote = xRicetta_Operazione.GetElementsByTagName("DatiRicettaxNote_2")                       '

        'xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")


        '----- Tag DatiMovimenti

        XML_DatiMovimenti = XML_Agenda.SelectSingleNode("DatiMovimenti")

        '----- Tag Movimento (multiplo)

        XMLs_Movimento = XML_DatiMovimenti.GetElementsByTagName("Movimento")

        For i = 0 To XMLs_Movimento.Count - 1

            XML_Movimento = XMLs_Movimento.Item(i)

            Cau_Mov = XML_Movimento.GetAttribute("cau_mov")

            Select Case Cau_Mov

                Case CAU_LAVORAZIONE,
                        CAU_RILIEVO_CAMPO,
                        CAU_RILIEVO_RACCOLTA,
                        CAU_TRATTAMENTO

                    Note = CStr(XML_Movimento.GetAttribute("mov_desc"))
                    CodDisciplinare = CInt(XML_Movimento.GetAttribute("num_protocollo"))
                    Id_Rcdpi = CInt(XML_Movimento.GetAttribute("doc_numero"))
                    Mezzo = CInt(XML_Movimento.GetAttribute("mezzo"))

                    If Lav_Cod = LAVCOD_ALTRE_OPERAZIONI Then
                        If XML_Agenda.HasAttribute("id_attivita") AndAlso Not String.IsNullOrEmpty(XML_Agenda.GetAttribute("id_attivita")) Then
                            Extra_Int = CInt(XML_Agenda.GetAttribute("id_attivita"))
                        End If
                    Else
                        Extra_Int = CInt(XML_Movimento.GetAttribute("extra_int"))
                    End If

                    DisciplinarePP = CInt(XML_Movimento.GetAttribute("disciplinare_pubblicoprivato"))

                    Data = CDate(XML_Movimento.GetAttribute("data_movimento")).ToShortDateString
                    Dim oraDateTime = CDate(XML_Movimento.GetAttribute("data_movimento"))

                    If XML_Movimento.HasAttribute("ora") Then
                        Dim ora = CDate(XML_Movimento.GetAttribute("ora"))
                        oraDateTime = New Date(oraDateTime.Year, oraDateTime.Month, oraDateTime.Day, ora.Hour, ora.Minute, 0)
                    End If

                    Dim W_Anagrafica_Stati_Cod As Integer = 0
                    Select Case tipo_operazione_agenda
                        Case enum_Tipo_Operazione_Agenda.Ricetta
                            W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire
                        Case enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                            W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
                    End Select

                    Dim Ricetta_Operazione_Cod_RIF As Integer = 0
                    If tipo_operazione_agenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio AndAlso ricetta_operazione_cod <> "" AndAlso tipo_operazione = enum_TipoOperazioneDB.Scrittura Then
                        Ricetta_Operazione_Cod_RIF = ricetta_operazione_cod
                    End If

                    Dim APP_Ricetta_Operazione_ID As String = ""
                    Dim Invia_HubIoT As Integer = 0
                    If tipo_operazione = enum_TipoOperazioneDB.Modifica Then
                        'Recupero l'APP_Ricetta_Operazione_ID per non perderlo
                        Dim Ric_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                        Dim dtRicetta As DataTable = Ric_R.Leggi(ricetta_cod, ricetta_operazione_cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                        APP_Ricetta_Operazione_ID = DBNullToNothing(dtRicetta.Rows(0).Item("APP_Ricetta_Operazione_ID"))
                        Invia_HubIoT = DBNullToNothing(dtRicetta.Rows(0).Item("Invia_HubIoT"))
                    End If
                    '--------------------------------
                    '----- XML Ricetta_Operazione
                    '--------------------------------

                    strOperazione = objXml.XML_Ricetta_Operazione(CInt(enum_TipoOperazioneDB.Scrittura),
                                                                  objParametri_Server.PivaSuperUser,
                                                                  frm_BaseCode,
                                                                  frm_TopCode,
                                                                  ricetta_cod,
                                                                  ricetta_operazione_cod,
                                                                  CInt(Lav_Cod),
                                                                  CStr(Des_Lib),
                                                                  CStr(Note),
                                                                  CInt(CodDisciplinare),
                                                                  Id_Rcdpi,
                                                                  Extra_Int,
                                                                  Mezzo,
                                                                  Data,
                                                                  Validita_Fine,
                                                                  , , ,
                                                                  DisciplinarePP,
                                                                  W_Anagrafica_Stati_Cod,
                                                                  Ricetta_Operazione_Cod_RIF,
                                                                  APP_Ricetta_Operazione_ID,
                                                                  raccoglitore_cod,
                                                                  invia_app,
                                                                  Invia_HubIoT,
                                                                  Data_Creazione, Username_Creazione, guidRicetta, oraDateTime)

                    XmlDoc.LoadXml(strOperazione)

                    XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

                    'se sto creando la ricetta a partire dall'operazione già registrata le lego
                    If Id_Agenda <> 0 Then

                        XML_DatiRicettaxAgenda = XmlDoc.CreateElement("DatiRicettaxAgenda_2")

                        XML_Operazione.AppendChild(XML_DatiRicettaxAgenda)

                        strRicettaxAgenda = objXml.XML_RicettaxAgenda_2(
                                                                            CInt(enum_TipoOperazioneDB.Scrittura),
                                                                            objParametri_Server.PivaSuperUser,
                                                                            ricetta_cod,
                                                                            ricetta_operazione_cod,
                                                                            CInt(Id_Agenda),
                                                                            Data,
                                                                            Validita_Fine)

                        XML_DatiRicettaxAgenda.InnerXml = strRicettaxAgenda

                    End If

                    If XML_Movimento.HasChildNodes Then

                        '-----------------------------------------
                        '-----------------------------------------
                        '----- Tag DatiMov_Dettagli_Tecnici
                        '-----------------------------------------
                        '-----------------------------------------

                        strRicettaDettagliTecnici = ""

                        XML_DatiMovDettagliTecnici = XML_Movimento.SelectSingleNode("DatiMov_Dettagli_Tecnici")

                        If XML_DatiMovDettagliTecnici IsNot Nothing Then

                            '----- Tag Movimento_Dettaglio_Tecnico  (multiplo)

                            XMLs_MovimentoDettaglioTecnico = XML_DatiMovDettagliTecnici.GetElementsByTagName("Movimento_Dettaglio_Tecnico")

                            If XMLs_MovimentoDettaglioTecnico IsNot Nothing AndAlso XMLs_MovimentoDettaglioTecnico.Count > 0 Then

                                XML_DatiRicettaDettagliTecnici = XmlDoc.CreateElement("DatiRicetta_Dettagli_Tecnici")

                                XML_Operazione.AppendChild(XML_DatiRicettaDettagliTecnici)

                                For j = 0 To XMLs_MovimentoDettaglioTecnico.Count - 1

                                    XML_MovimentoDettaglioTecnico = XMLs_MovimentoDettaglioTecnico.Item(j)

                                    strRicettaDettaglioTecnico = objXml.XML_Ricetta_DettaglioTecnico(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                        objParametri_Server.PivaSuperUser,
                                                                                                        ricetta_cod, ricetta_operazione_cod,
                                                                                                        , , ,
                                                                                                        CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("qta_ril")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("dett_cod")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_cod")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_gru")),
                                                                                                        CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("dose")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("parziale")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("nitrati")),
                                                                                                        CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("freatimetro")),
                                                                                                        ,
                                                                                                        ,
                                                                                                        ,
                                                                                                        ,
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ditta_cod")),
                                                                                                        CStr(XML_MovimentoDettaglioTecnico.GetAttribute("sigla_av")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("trap_num")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("id_insetto")),
                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ff_classe")),
                                                                                                        CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_inizio")),
                                                                                                        CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_fine")))

                                    strRicettaDettagliTecnici &= strRicettaDettaglioTecnico

                                Next

                                XML_DatiRicettaDettagliTecnici.InnerXml = strRicettaDettagliTecnici

                            End If

                        End If

                        '-----------------------------------------
                        '-----------------------------------------
                        '----- Tag DatiMovimenti_Dettagli
                        '-----------------------------------------
                        '-----------------------------------------

                        strRicettaDettagli = ""
                        strRicettaDettagliTecnici = ""
                        strRicettaDestinazioni = ""

                        XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                        If XML_DatiMovimentiDettagli IsNot Nothing Then

                            '----- Tag Movimento_Dettaglio  (multiplo)

                            'Recupero la collezione dei nodi
                            XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                            If XMLs_MovimentoDettaglio IsNot Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

                                XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

                                XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

                                For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                                    strRicettaDettagliTecnici = ""
                                    strRicettaDestinazioni = ""

                                    XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                                    Extra_Str = ""
                                    TempoCarenza = 0
                                    DoseEtichetta = ""
                                    DoseEtichetta_Value = ""
                                    PrincipiAttivi = ""
                                    ClassiTossicologiche = ""
                                    Polverulento = 0
                                    Buffer = ""

                                    If XML_MovimentoDettaglio.HasAttribute("extra_str") Then
                                        Extra_Str = XML_MovimentoDettaglio.GetAttribute("extra_str")
                                    End If

                                    If XML_MovimentoDettaglio.HasAttribute("tempocarenza") Then
                                        TempoCarenza = CInt(XML_MovimentoDettaglio.GetAttribute("tempocarenza"))
                                    End If

                                    If XML_MovimentoDettaglio.HasAttribute("doseetichetta") Then
                                        DoseEtichetta = CStr(XML_MovimentoDettaglio.GetAttribute("doseetichetta"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("doseetichetta_value") Then
                                        DoseEtichetta_Value = CStr(XML_MovimentoDettaglio.GetAttribute("doseetichetta_value"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("principiattivi") Then
                                        PrincipiAttivi = CStr(XML_MovimentoDettaglio.GetAttribute("principiattivi"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("principiattivipercabb") Then
                                        PrincipiAttiviPercAbb = CStr(XML_MovimentoDettaglio.GetAttribute("principiattivipercabb"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("principiattivipesi") Then
                                        PrincipiAttiviPesi = CStr(XML_MovimentoDettaglio.GetAttribute("principiattivipesi"))
                                    End If

                                    If XML_MovimentoDettaglio.HasAttribute("classitossicologiche") Then
                                        ClassiTossicologiche = CStr(XML_MovimentoDettaglio.GetAttribute("classitossicologiche"))
                                    End If

                                    If XML_MovimentoDettaglio.HasAttribute("polverulento") Then
                                        Polverulento = CInt(XML_MovimentoDettaglio.GetAttribute("polverulento"))
                                    End If

                                    If XML_MovimentoDettaglio.HasAttribute("buffer") Then
                                        Buffer = CStr(XML_MovimentoDettaglio.GetAttribute("buffer"))
                                    End If

                                    Dim Qta_Extra As Decimal = 0
                                    Dim Qta_Extra_Totale As Decimal = 0
                                    Dim Udm_Cod_Extra As Integer = 0
                                    Dim Mezzo_Det As Integer = 0
                                    Dim lotto As String = ""

                                    If XML_MovimentoDettaglio.HasAttribute("qta_extra") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("qta_extra")) Then
                                        Qta_Extra = CStr(XML_MovimentoDettaglio.GetAttribute("qta_extra"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("qta_extra_totale") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("qta_extra_totale")) Then
                                        Qta_Extra_Totale = CStr(XML_MovimentoDettaglio.GetAttribute("qta_extra_totale"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("udm_cod_extra") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("udm_cod_extra")) Then
                                        Udm_Cod_Extra = CStr(XML_MovimentoDettaglio.GetAttribute("udm_cod_extra"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("mezzo_det") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("mezzo_det")) Then
                                        Mezzo_Det = CStr(XML_MovimentoDettaglio.GetAttribute("mezzo_det"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("lotto") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("lotto")) Then
                                        lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                                    End If


                                    strRicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                                                    objParametri_Server.PivaSuperUser,
                                                                    ricetta_cod,
                                                                    ricetta_operazione_cod,
                                                                     0,
                                                                     1,
                                                                    CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
                                                                    CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
                                                                    CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
                                                                    CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
                                                                    CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
                                                                    CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
                                                                    CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                    CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
                                                                    0,
                                                                    Cau_Mov,
                                                                    TempoCarenza,
                                                                    DoseEtichetta,
                                                                    PrincipiAttivi,
                                                                    ClassiTossicologiche,
                                                                    DoseEtichetta_Value,
                                                                    ,,,
                                                                    lotto, Qta_Extra, Qta_Extra_Totale, Udm_Cod_Extra, Mezzo_Det,
                                                                    Extra_Str, PrincipiAttiviPercAbb, PrincipiAttiviPesi, Polverulento, Buffer)


                                    XmlDoc2.LoadXml(strRicettaDettaglio)

                                    XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

                                    '----- Tag Movimento_Dettaglio_Tecnico_2  (multiplo)

                                    XMLs_Movimento_Dettaglio_Tecnico_2 = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Dettaglio_Tecnico_2")

                                    If XMLs_Movimento_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Movimento_Dettaglio_Tecnico_2.Count > 0 Then

                                        For n = 0 To XMLs_Movimento_Dettaglio_Tecnico_2.Count - 1

                                            XML_MovimentoDettaglioTecnico = XMLs_Movimento_Dettaglio_Tecnico_2.Item(n)

                                            Dim Inn1 As Date = AGRODATAINIZIO
                                            Dim Inn2 As Date = AGRODATAFINE
                                            If IsDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn1_data")) Then
                                                Inn1 = CDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn1_data"))
                                            End If
                                            If IsDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn2_data")) Then
                                                Inn2 = CDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn2_data"))
                                            End If

                                            Dim Mg As Decimal = 0
                                            Dim Azoto As Decimal = 0
                                            Dim P As Decimal = 0
                                            Dim K As Decimal = 0
                                            Dim Efficienza As Decimal = 0
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("mg")) Then
                                                Mg = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("mg"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("n")) Then
                                                Azoto = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("n"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("p")) Then
                                                P = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("p"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("k")) Then
                                                K = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("k"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("efficienza")) Then
                                                Efficienza = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("efficienza"))
                                            End If

                                            Dim Soglia_Cod As Integer = 0
                                            Dim Soglia_Des As String = ""
                                            Dim Soglia_Qta As Decimal = 0
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_cod")) Then
                                                Soglia_Cod = CInt(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_cod"))
                                            End If
                                            Soglia_Des = XML_MovimentoDettaglioTecnico.GetAttribute("soglia_des")
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_quantita")) Then
                                                Soglia_Qta = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_quantita"))
                                            End If

                                            Dim Cu As Decimal = 0
                                            If XML_MovimentoDettaglioTecnico.HasAttribute("cu") AndAlso IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("cu")) Then
                                                Cu = CInt(XML_MovimentoDettaglioTecnico.GetAttribute("cu"))
                                            End If

                                            strRicettaDettaglioTecnico = objXml.XML_Ricetta_DettaglioTecnico_2(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                                objParametri_Server.PivaSuperUser,
                                                                                                                ricetta_cod, ricetta_operazione_cod,
                                                                                                                , ,
                                                                                                                1,
                                                                                                                CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("qta_ril")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("dett_cod")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_cod")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_gru")),
                                                                                                                CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("dose")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("parziale")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("nitrati")),
                                                                                                                CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("freatimetro")),
                                                                                                                Inn1, Inn2,
                                                                                                                ,
                                                                                                                ,
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ditta_cod")),
                                                                                                                CStr(XML_MovimentoDettaglioTecnico.GetAttribute("sigla_av")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("trap_num")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("id_insetto")),
                                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ff_classe")),
                                                                                                                CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_inizio")),
                                                                                                                CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_fine")),
                                                                                                                Mg,
                                                                                                                Azoto,
                                                                                                                P,
                                                                                                                K,
                                                                                                                Soglia_Cod,
                                                                                                                Soglia_Des,
                                                                                                                Soglia_Qta,
                                                                                                                Efficienza,,
                                                                                                                   ,,,,
                                                                                                                   Cu)

                                            strRicettaDettagliTecnici &= strRicettaDettaglioTecnico

                                        Next


                                    End If


                                    '----- Tag Movimento_Destinazione  (multiplo)

                                    XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                                    If XMLs_MovimentoDestinazione IsNot Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

                                        For n = 0 To XMLs_MovimentoDestinazione.Count - 1

                                            XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)

                                            Dim Qta2 As Decimal = 0D
                                            If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
                                                Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                                            End If

                                            Dim Tipo_Destinazione As Integer
                                            If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
                                                Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                                            End If

                                            Dim Sup_Riduzione_BufferZone As Decimal = 0D
                                            If XML_MovimentoDestinazione.HasAttribute("sup_riduzione_bufferzone") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("sup_riduzione_bufferzone")) Then
                                                Sup_Riduzione_BufferZone = XML_MovimentoDestinazione.GetAttribute("sup_riduzione_bufferzone")
                                            End If

                                            Dim Perc_Riduzione_Deriva As Decimal = 0D
                                            If XML_MovimentoDestinazione.HasAttribute("perc_riduzione_deriva") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("perc_riduzione_deriva")) Then
                                                Perc_Riduzione_Deriva = XML_MovimentoDestinazione.GetAttribute("perc_riduzione_deriva")
                                            End If

                                            strRicettaDestinazione = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                         objParametri_Server.PivaSuperUser,
                                                                                                         ricetta_cod, ricetta_operazione_cod,
                                                                                                         , ,
                                                                                                         CStr(XML_MovimentoDestinazione.GetAttribute("piva")),
                                                                                                         CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod")),
                                                                                                         CInt(XML_MovimentoDestinazione.GetAttribute("appezza")),
                                                                                                         CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione")),
                                                                                                         CInt(IIf(IsNumeric(XML_MovimentoDestinazione.GetAttribute("programmazione_entita_cod")), XML_MovimentoDestinazione.GetAttribute("programmazione_entita_cod"), 0)),
                                                                                                         CDbl(XML_MovimentoDestinazione.GetAttribute("qta")),
                                                                                                         CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                                                         CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")),
                                                                                                         CDbl(IIf(IsNumeric(XML_MovimentoDestinazione.GetAttribute("quotadistribuzione")), XML_MovimentoDestinazione.GetAttribute("quotadistribuzione"), 0)),
                                                                                                         Qta2,
                                                                                                         Tipo_Destinazione,
                                                                                                            Sup_Riduzione_BufferZone:=Sup_Riduzione_BufferZone,
                                                                                                            Perc_Riduzione_Deriva:=Perc_Riduzione_Deriva) ' errore stringa programmazione_entità_cod

                                            strRicettaDestinazioni &= strRicettaDestinazione

                                        Next

                                    End If

                                    XML_RicettaDettaglio.InnerXml = strRicettaDettagliTecnici & strRicettaDestinazioni

                                    Dim pro As String = XML_RicettaDettaglio.OuterXml

                                    XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

                                Next

                            End If

                        End If

                    End If

                Case CAU_IMPUTAZIONE_PARCOMACCHINE,
                         CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                         CAU_IMPUTAZIONE_MANODOPERA,
                         CAU_IMPUTAZIONE_TERZISTI,
                         CAU_SCARICO

                    '-----------------------------------------
                    '-----------------------------------------
                    '----- Tag DatiMovimenti_Dettagli
                    '-----------------------------------------
                    '-----------------------------------------

                    strRicettaDestinazioni = ""

                    idAgendaScaricoDaAzPadre = 0
                    Piva_Magazzino = ""
                    Sa_Cod_Magazzino = 0
                    Id_Destinazione_Magazzino = 0
                    '(14/04/2020 fede) introdotto controllo scarico da altre aziende
                    'in questo caso, sulla ricetta non creo altre operazioni (come in agenda)
                    'ma salvo i dati del magazzino da cui scarico nel movimento di scarico
                    If Cau_Mov = CAU_SCARICO Then
                        Dim dtRif As DataTable
                        Dim drRif As DataRow()
                        Dim objRifR As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                        dtRif = objRifR.Recupera_DT_Rif_Unificato(Piva,
                                                            Sa_Cod,
                                                            Id_Agenda,
                                                            0, 0, 0, "",
                                                            objParametri_Server)
                        If dtRif IsNot Nothing AndAlso dtRif.Rows.Count > 0 Then
                            drRif = dtRif.Select("Lav_Cod_Risultato=" & LAVCOD_SCARICO)
                            If drRif IsNot Nothing AndAlso drRif.Length > 0 Then
                                idAgendaScaricoDaAzPadre = drRif(0).Item("Id_Agenda_Risultato")
                                Piva_Magazzino = drRif(0).Item("Piva_Risultato")
                            End If
                        End If
                        If idAgendaScaricoDaAzPadre <> 0 Then
                            Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                            Dim DtDest As DataTable
                            DtDest = objDest.Leggi(Piva_Magazzino, 0, idAgendaScaricoDaAzPadre,
                                                                0, 0, 0, 0, CostantiPersonalizzate.MAGAZZINO, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri_Server)
                            If DtDest IsNot Nothing AndAlso DtDest.Rows.Count > 0 Then
                                Piva_Magazzino = DtDest.Rows(0).Item("piva")
                                Sa_Cod_Magazzino = DtDest.Rows(0).Item("sa_cod")
                                Id_Destinazione_Magazzino = DtDest.Rows(0).Item("id_destinazione")
                            End If
                        End If
                    End If

                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                    If XML_DatiMovimentiDettagli IsNot Nothing Then

                        '----- Tag Movimento_Dettaglio  (multiplo)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                        If XMLs_MovimentoDettaglio IsNot Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

                            For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                                strRicettaDestinazioniCostiAccessori = ""

                                XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                                Dim Qualifica_Cod As Integer = 0
                                Dim Tariffa_Cod As Integer = 0
                                Dim prezzo_unitario As Decimal = 0
                                Dim id_attivita As Integer = 0
                                Dim lotto As String = ""

                                If XML_MovimentoDettaglio.HasAttribute("qualifica_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("qualifica_cod")) Then
                                    Qualifica_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("qualifica_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("tariffa_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("tariffa_cod")) Then
                                    Tariffa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("tariffa_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("prezzo_unitario") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario")) Then
                                    prezzo_unitario = CDec(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("id_attivita") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("id_attivita")) Then
                                    id_attivita = CInt(XML_MovimentoDettaglio.GetAttribute("id_attivita"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("lotto") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("lotto")) Then
                                    lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                                End If

                                strRicettaDettaglioCostiAccessori = objXml.XML_Ricetta_Dettaglio(
                                                                                CInt(enum_TipoOperazioneDB.Scrittura),
                                                                                objParametri_Server.PivaSuperUser,
                                                                                ricetta_cod, CInt(ricetta_operazione_cod),
                                                                                0,
                                                                                0,
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
                                                                                CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
                                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
                                                                                prezzo_unitario,
                                                                                Cau_Mov,
                                                                                Qualifica_cod:=Qualifica_Cod,
                                                                                Tariffa_cod:=Tariffa_Cod,
                                                                                id_attivita:=id_attivita,
                                                                                lotto:=lotto)


                                XmlDoc3.LoadXml(strRicettaDettaglioCostiAccessori)

                                XML_RicettaDettaglio = XmlDoc3.SelectSingleNode("Ricetta_Dettaglio")

                                '----- Tag Movimento_Destinazione  (multiplo)

                                XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                                If XMLs_MovimentoDestinazione IsNot Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

                                    For n = 0 To XMLs_MovimentoDestinazione.Count - 1

                                        XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)

                                        Dim Qta2 As Decimal = 0D
                                        If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
                                            Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                                        End If

                                        Dim Tipo_Destinazione As Integer
                                        If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
                                            Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                                        End If

                                        Dim Piva_Dest As String = CStr(XML_MovimentoDestinazione.GetAttribute("piva"))
                                        Dim Sa_Cod_Dest As String = CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod"))
                                        Dim Id_Destinazione_Dest As String = CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione"))

                                        If Piva_Magazzino <> "" AndAlso Sa_Cod_Magazzino <> 0 AndAlso Id_Destinazione_Magazzino <> 0 Then
                                            Piva_Dest = Piva_Magazzino
                                            Sa_Cod_Dest = Sa_Cod_Magazzino
                                            Id_Destinazione_Dest = Id_Destinazione_Magazzino
                                        End If

                                        Dim MagazzinoEsterno_Cod As String = ""
                                        If XML_MovimentoDestinazione.HasAttribute("magazzinoesterno_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("magazzinoesterno_cod")) Then
                                            MagazzinoEsterno_Cod = XML_MovimentoDestinazione.GetAttribute("magazzinoesterno_cod")
                                        End If

                                        Dim MagazzinoEsterno_Des As String = ""
                                        If XML_MovimentoDestinazione.HasAttribute("magazzinoesterno_des") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("magazzinoesterno_des")) Then
                                            MagazzinoEsterno_Des = XML_MovimentoDestinazione.GetAttribute("magazzinoesterno_des")
                                        End If

                                        Dim MagazzinoEsterno_Dettagli As String = ""
                                        If XML_MovimentoDestinazione.HasAttribute("magazzinoesterno_dettagli") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("magazzinoesterno_dettagli")) Then
                                            MagazzinoEsterno_Dettagli = XML_MovimentoDestinazione.GetAttribute("magazzinoesterno_dettagli")
                                        End If

                                        strRicettaDestinazioneCostiAccessori = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                     objParametri_Server.PivaSuperUser,
                                                                                                     ricetta_cod, CInt(ricetta_operazione_cod),
                                                                                                     , ,
                                                                                                     Piva_Dest,
                                                                                                     Sa_Cod_Dest,
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("appezza")),
                                                                                                     Id_Destinazione_Dest,
                                                                                                     0,
                                                                                                     CDbl(XML_MovimentoDestinazione.GetAttribute("qta")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")),
                                                                                                     0,
                                                                                                     Qta2,
                                                                                                     Tipo_Destinazione,
                                                                                                     magazzinoEsterno_Cod:=MagazzinoEsterno_Cod,
                                                                                                     magazzinoEsterno_Des:=MagazzinoEsterno_Des,
                                                                                                     magazzinoEsterno_Dettagli:=MagazzinoEsterno_Dettagli)

                                        strRicettaDestinazioniCostiAccessori &= strRicettaDestinazioneCostiAccessori

                                    Next

                                End If

                                XML_RicettaDettaglio.InnerXml = strRicettaDestinazioniCostiAccessori

                                Dim pro As String = XML_RicettaDettaglio.OuterXml

                                strRicettaDettagliCostiAccessori &= XML_RicettaDettaglio.OuterXml

                                'XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

                            Next


                        End If

                    End If

                Case CAU_CARICO

                    If CInt(XML_Agenda.GetAttribute("lav_cod")) <> LAVCOD_RACCOLTA Then
                        Exit Select
                    End If

                    '-----------------------------------------
                    '-----------------------------------------
                    '----- Tag DatiMovimenti_Dettagli
                    '-----------------------------------------
                    '-----------------------------------------

                    strRicettaDestinazioni = ""

                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                    If XML_DatiMovimentiDettagli IsNot Nothing Then

                        '----- Tag Movimento_Dettaglio  (multiplo)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                        If XMLs_MovimentoDettaglio IsNot Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

                            For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                                strRicettaDestinazioniCostiAccessori = ""

                                XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                                Dim Qualifica_Cod As Integer = 0
                                Dim Tariffa_Cod As Integer = 0
                                Dim prezzo_unitario As Decimal = 0
                                Dim id_attivita As Integer = 0
                                Dim lotto As String = ""

                                If XML_MovimentoDettaglio.HasAttribute("qualifica_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("qualifica_cod")) Then
                                    Qualifica_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("qualifica_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("tariffa_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("tariffa_cod")) Then
                                    Tariffa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("tariffa_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("prezzo_unitario") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario")) Then
                                    prezzo_unitario = CDec(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("id_attivita") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("id_attivita")) Then
                                    id_attivita = CInt(XML_MovimentoDettaglio.GetAttribute("id_attivita"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("lotto") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("lotto")) Then
                                    lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                                End If

                                strRicettaDettaglioCostiAccessori = objXml.XML_Ricetta_Dettaglio(
                                                                                CInt(enum_TipoOperazioneDB.Scrittura),
                                                                                objParametri_Server.PivaSuperUser,
                                                                                ricetta_cod, CInt(ricetta_operazione_cod),
                                                                                0,
                                                                                0,
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
                                                                                CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
                                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
                                                                                prezzo_unitario,
                                                                                Cau_Mov,
                                                                                Qualifica_cod:=Qualifica_Cod,
                                                                                Tariffa_cod:=Tariffa_Cod,
                                                                                id_attivita:=id_attivita,
                                                                                lotto:=lotto)


                                XmlDoc3.LoadXml(strRicettaDettaglioCostiAccessori)

                                XML_RicettaDettaglio = XmlDoc3.SelectSingleNode("Ricetta_Dettaglio")

                                '----- Tag Movimento_Destinazione  (multiplo)

                                XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                                If XMLs_MovimentoDestinazione IsNot Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

                                    For n = 0 To XMLs_MovimentoDestinazione.Count - 1

                                        XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)

                                        Dim Qta2 As Decimal = 0D
                                        If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
                                            Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                                        End If

                                        Dim Tipo_Destinazione As Integer
                                        If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
                                            Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                                        End If

                                        strRicettaDestinazioneCostiAccessori = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                     objParametri_Server.PivaSuperUser,
                                                                                                     ricetta_cod, CInt(ricetta_operazione_cod),
                                                                                                     , ,
                                                                                                     CStr(XML_MovimentoDestinazione.GetAttribute("piva")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("appezza")),
                                                                                                      CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione")),
                                                                                                     0,
                                                                                                     CDbl(XML_MovimentoDestinazione.GetAttribute("qta")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")),
                                                                                                     0,
                                                                                                     Qta2,
                                                                                                     Tipo_Destinazione)

                                        strRicettaDestinazioniCostiAccessori &= strRicettaDestinazioneCostiAccessori

                                    Next

                                End If

                                XML_RicettaDettaglio.InnerXml = strRicettaDestinazioniCostiAccessori

                                Dim pro As String = XML_RicettaDettaglio.OuterXml

                                strRicettaDettagliCostiAccessori &= XML_RicettaDettaglio.OuterXml

                                'XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

                            Next


                        End If

                    End If

                Case CAU_CARICO

                    If CInt(XML_Agenda.GetAttribute("lav_cod")) <> LAVCOD_RACCOLTA Then
                        Exit Select
                    End If

                    '-----------------------------------------
                    '-----------------------------------------
                    '----- Tag DatiMovimenti_Dettagli
                    '-----------------------------------------
                    '-----------------------------------------

                    strRicettaDestinazioni = ""

                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                    If XML_DatiMovimentiDettagli IsNot Nothing Then

                        '----- Tag Movimento_Dettaglio  (multiplo)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                        If XMLs_MovimentoDettaglio IsNot Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

                            For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                                strRicettaDestinazioniCostiAccessori = ""

                                XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                                Dim Qualifica_Cod As Integer = 0
                                Dim Tariffa_Cod As Integer = 0
                                Dim prezzo_unitario As Decimal = 0
                                Dim id_attivita As Integer = 0
                                Dim lotto As String = ""

                                If XML_MovimentoDettaglio.HasAttribute("qualifica_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("qualifica_cod")) Then
                                    Qualifica_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("qualifica_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("tariffa_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("tariffa_cod")) Then
                                    Tariffa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("tariffa_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("prezzo_unitario") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario")) Then
                                    prezzo_unitario = CDec(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("id_attivita") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("id_attivita")) Then
                                    id_attivita = CInt(XML_MovimentoDettaglio.GetAttribute("id_attivita"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("lotto") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("lotto")) Then
                                    lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                                End If

                                strRicettaDettaglioCostiAccessori = objXml.XML_Ricetta_Dettaglio(
                                                                                CInt(enum_TipoOperazioneDB.Scrittura),
                                                                                objParametri_Server.PivaSuperUser,
                                                                                ricetta_cod, CInt(ricetta_operazione_cod),
                                                                                0,
                                                                                0,
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
                                                                                CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
                                                                                CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
                                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
                                                                                prezzo_unitario,
                                                                                Cau_Mov,
                                                                                Qualifica_cod:=Qualifica_Cod,
                                                                                Tariffa_cod:=Tariffa_Cod,
                                                                                id_attivita:=id_attivita,
                                                                                lotto:=lotto)


                                XmlDoc3.LoadXml(strRicettaDettaglioCostiAccessori)

                                XML_RicettaDettaglio = XmlDoc3.SelectSingleNode("Ricetta_Dettaglio")

                                '----- Tag Movimento_Destinazione  (multiplo)

                                XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                                If XMLs_MovimentoDestinazione IsNot Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

                                    For n = 0 To XMLs_MovimentoDestinazione.Count - 1

                                        XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)

                                        Dim Qta2 As Decimal = 0D
                                        If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
                                            Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                                        End If

                                        Dim Tipo_Destinazione As Integer
                                        If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
                                            Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                                        End If

                                        strRicettaDestinazioneCostiAccessori = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                     objParametri_Server.PivaSuperUser,
                                                                                                     ricetta_cod, CInt(ricetta_operazione_cod),
                                                                                                     , ,
                                                                                                     CStr(XML_MovimentoDestinazione.GetAttribute("piva")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("appezza")),
                                                                                                      CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione")),
                                                                                                     0,
                                                                                                     CDbl(XML_MovimentoDestinazione.GetAttribute("qta")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")),
                                                                                                     0,
                                                                                                     Qta2,
                                                                                                     Tipo_Destinazione)

                                        strRicettaDestinazioniCostiAccessori &= strRicettaDestinazioneCostiAccessori

                                    Next

                                End If

                                XML_RicettaDettaglio.InnerXml = strRicettaDestinazioniCostiAccessori

                                Dim pro As String = XML_RicettaDettaglio.OuterXml

                                strRicettaDettagliCostiAccessori &= XML_RicettaDettaglio.OuterXml

                                'XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

                            Next


                        End If

                    End If

            End Select

        Next

        XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & strRicettaDettagliCostiAccessori & strNote

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml

    End Function

    'DT: DEPRECATO!!!! non usare, lasciato solo per il Ricette_Edit

    Public Function XML_GeneraStringa_Ricetta_Operazione(ByVal Ricetta_Cod As String, ByVal Tipo_Ricetta As enum_TipoRicetta,
                                                         ByVal PivaSuperUser As String, ByVal progressivo_gias As Integer,
                                                         ByVal strAgenda As String,
                                                            ByRef Lav_Cod As Integer, ByRef Des_Lib As String, ByRef Data As Date, ByRef Note As String,
                                                         ByRef objParametri_Server As AgronicaCoreParametri) As String


        '-----------------------------------------------------------------
        'DT: DEPRECATO!!!! non usare, lasciato solo per il Ricette_Edit
        '-----------------------------------------------------------------

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDocAgenda As New XmlDocument
        Dim XML_DatiAgenda As XmlElement
        Dim XML_Agenda As XmlElement
        Dim XML_DatiMovimenti As XmlElement
        Dim XML_Movimento As XmlElement
        Dim XMLs_Movimento As XmlNodeList
        Dim XML_DatiMovDettagliTecnici As XmlElement
        Dim XML_MovimentoDettaglioTecnico As XmlElement
        Dim XMLs_MovimentoDettaglioTecnico As XmlNodeList
        Dim XMLs_Movimento_Dettaglio_Tecnico_2 As XmlNodeList
        Dim XML_DatiMovimentiDettagli As XmlElement
        Dim XML_MovimentoDettaglio As XmlElement
        Dim XMLs_MovimentoDettaglio As XmlNodeList
        Dim XML_MovimentoDestinazione As XmlElement
        Dim XMLs_MovimentoDestinazione As XmlNodeList

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument
        Dim XmlDoc3 As New XmlDocument

        Dim XML_Operazione As XmlElement
        Dim XML_DatiRicettaxAgenda As XmlElement
        Dim XML_DatiRicettaDettagliTecnici As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement = Nothing
        Dim XML_RicettaDettaglio As XmlElement

        Dim strOperazione As String = ""
        Dim strRicettaxAgenda As String = ""
        Dim strRicettaDettagli As String = ""
        Dim strRicettaDettaglio As String = ""
        Dim strRicettaDettagliTecnici As String = ""
        Dim strRicettaDettaglioTecnico As String = ""
        Dim strRicettaDestinazioni As String = ""
        Dim strRicettaDestinazione As String = ""
        Dim strRicettaDettagliCostiAccessori As String = ""
        Dim strRicettaDettaglioCostiAccessori As String = ""
        Dim strRicettaDestinazioniCostiAccessori As String = ""
        Dim strRicettaDestinazioneCostiAccessori As String = ""

        Dim i, j, n As Integer

        Dim Piva As String
        Dim Sa_Cod As Integer

        Dim Id_Agenda As Integer
        Dim Cau_Mov As String
        Dim CodDisciplinare As Integer
        Dim Id_Rcdpi As Integer = 0
        Dim Extra_Int As Integer
        Dim Mezzo As Integer
        Dim DisciplinarePP As Integer = 0
        Dim Validita_Fine As Date = AGRODATAFINE

        Dim idAgendaScaricoDaAzPadre As Integer
        Dim Piva_Magazzino As String
        Dim Sa_Cod_Magazzino As Integer
        Dim Id_Destinazione_Magazzino As Integer


        Dim TempoCarenza As Integer
        Dim DoseEtichetta As String
        Dim DoseEtichetta_Value As String
        Dim PrincipiAttivi As String
        Dim ClassiTossicologiche As String

        Dim objXml As New AgronicaCoreXML.XML_Contab

        Dim Dt_Impianti As New DataTable


        Dim frm_BaseCode, frm_TopCode As Integer
        Call Calcola_BaseCode_TopCode(frm_BaseCode, frm_TopCode, progressivo_gias)

        Dim W_Anagrafica_Stati_Cod As Integer = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire

        'Select Case Tipo_Ricetta
        '    Case enum_TipoRicetta.PianoDistribuzionePua
        '        W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
        'End Select

        '------------------------------------------------
        '----- Recupero i dati dalla stringa
        '------------------------------------------------

        XmlDocAgenda.LoadXml(strAgenda)

        '----- Tag DatiAgenda

        XML_DatiAgenda = XmlDocAgenda.SelectSingleNode("DatiAgenda")

        '----- Tag Agenda

        XML_Agenda = XML_DatiAgenda.SelectSingleNode("Agenda")

        Piva = CStr(XML_Agenda.GetAttribute("piva"))
        Sa_Cod = CInt(XML_Agenda.GetAttribute("sa_cod"))

        Lav_Cod = CStr(XML_Agenda.GetAttribute("lav_cod"))
        Des_Lib = CStr(XML_Agenda.GetAttribute("des_lib"))
        Id_Agenda = CInt(XML_Agenda.GetAttribute("id_agenda"))


        '----- Tag DatiMovimenti

        XML_DatiMovimenti = XML_Agenda.SelectSingleNode("DatiMovimenti")

        '----- Tag Movimento (multiplo)

        XMLs_Movimento = XML_DatiMovimenti.GetElementsByTagName("Movimento")

        For i = 0 To XMLs_Movimento.Count - 1

            XML_Movimento = XMLs_Movimento.Item(i)

            Cau_Mov = XML_Movimento.GetAttribute("cau_mov")

            Select Case Cau_Mov

                Case CAU_LAVORAZIONE,
                    CAU_RILIEVO_CAMPO,
                    CAU_RILIEVO_RACCOLTA,
                    CAU_TRATTAMENTO

                    Note = CStr(XML_Movimento.GetAttribute("mov_desc"))
                    CodDisciplinare = CInt(XML_Movimento.GetAttribute("num_protocollo"))
                    Id_Rcdpi = CInt(XML_Movimento.GetAttribute("doc_numero"))
                    Mezzo = CInt(XML_Movimento.GetAttribute("mezzo"))
                    Extra_Int = CInt(XML_Movimento.GetAttribute("extra_int"))
                    DisciplinarePP = CInt(XML_Movimento.GetAttribute("disciplinare_pubblicoprivato"))

                    Data = CDate(XML_Movimento.GetAttribute("data_movimento")).ToShortDateString

                    '--------------------------------
                    '----- XML Ricetta_Operazione
                    '--------------------------------

                    strOperazione = objXml.XML_Ricetta_Operazione(
                                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                                    PivaSuperUser,
                                                    frm_BaseCode,
                                                    frm_TopCode,
                                                    CInt(Ricetta_Cod),
                                                    0,
                                                    CInt(Lav_Cod),
                                                    CStr(Des_Lib),
                                                    CStr(Note),
                                                    CInt(CodDisciplinare),
                                                    Id_Rcdpi,
                                                    Extra_Int,
                                                    Mezzo,
                                                    Data,
                                                    Validita_Fine,
                                                    , , ,
                                                    DisciplinarePP,
                                                    W_Anagrafica_Stati_Cod)

                    XmlDoc.LoadXml(strOperazione)

                    XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

                    'se sto creando la ricetta a partire dall'operazione già registrata le lego
                    If Id_Agenda <> 0 Then

                        XML_DatiRicettaxAgenda = XmlDoc.CreateElement("DatiRicettaxAgenda_2")

                        XML_Operazione.AppendChild(XML_DatiRicettaxAgenda)

                        strRicettaxAgenda = objXml.XML_RicettaxAgenda_2(
                                                                        CInt(enum_TipoOperazioneDB.Scrittura),
                                                                        PivaSuperUser,
                                                                        CInt(Ricetta_Cod),
                                                                        0,
                                                                        CInt(Id_Agenda),
                                                                        Data,
                                                                        Validita_Fine)

                        XML_DatiRicettaxAgenda.InnerXml = strRicettaxAgenda

                    End If

                    If XML_Movimento.HasChildNodes Then

                        '-----------------------------------------
                        '-----------------------------------------
                        '----- Tag DatiMov_Dettagli_Tecnici
                        '-----------------------------------------
                        '-----------------------------------------

                        strRicettaDettagliTecnici = ""

                        XML_DatiMovDettagliTecnici = XML_Movimento.SelectSingleNode("DatiMov_Dettagli_Tecnici")

                        If XML_DatiMovDettagliTecnici IsNot Nothing Then

                            '----- Tag Movimento_Dettaglio_Tecnico  (multiplo)

                            XMLs_MovimentoDettaglioTecnico = XML_DatiMovDettagliTecnici.GetElementsByTagName("Movimento_Dettaglio_Tecnico")

                            If XMLs_MovimentoDettaglioTecnico IsNot Nothing AndAlso XMLs_MovimentoDettaglioTecnico.Count > 0 Then

                                XML_DatiRicettaDettagliTecnici = XmlDoc.CreateElement("DatiRicetta_Dettagli_Tecnici")

                                XML_Operazione.AppendChild(XML_DatiRicettaDettagliTecnici)

                                For j = 0 To XMLs_MovimentoDettaglioTecnico.Count - 1

                                    XML_MovimentoDettaglioTecnico = XMLs_MovimentoDettaglioTecnico.Item(j)

                                    strRicettaDettaglioTecnico = objXml.XML_Ricetta_DettaglioTecnico(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                    PivaSuperUser,
                                                                                                    CInt(Ricetta_Cod),
                                                                                                    , , , ,
                                                                                                    CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("qta_ril")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("dett_cod")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_cod")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_gru")),
                                                                                                    CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("dose")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("parziale")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("nitrati")),
                                                                                                    CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("freatimetro")),
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ditta_cod")),
                                                                                                    CStr(XML_MovimentoDettaglioTecnico.GetAttribute("sigla_av")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("trap_num")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("id_insetto")),
                                                                                                    CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ff_classe")),
                                                                                                    CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_inizio")),
                                                                                                    CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_fine")))

                                    strRicettaDettagliTecnici &= strRicettaDettaglioTecnico

                                Next

                                XML_DatiRicettaDettagliTecnici.InnerXml = strRicettaDettagliTecnici

                            End If

                        End If

                        '-----------------------------------------
                        '-----------------------------------------
                        '----- Tag DatiMovimenti_Dettagli
                        '-----------------------------------------
                        '-----------------------------------------

                        strRicettaDettagli = ""
                        strRicettaDettagliTecnici = ""
                        strRicettaDestinazioni = ""

                        XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                        If XML_DatiMovimentiDettagli IsNot Nothing Then

                            '----- Tag Movimento_Dettaglio  (multiplo)

                            'Recupero la collezione dei nodi
                            XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                            If XMLs_MovimentoDettaglio IsNot Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

                                XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

                                XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

                                For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                                    strRicettaDettagliTecnici = ""
                                    strRicettaDestinazioni = ""

                                    XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                                    TempoCarenza = 0
                                    DoseEtichetta = ""
                                    DoseEtichetta_Value = ""
                                    PrincipiAttivi = ""
                                    ClassiTossicologiche = ""
                                    If XML_MovimentoDettaglio.HasAttribute("tempocarenza") Then
                                        TempoCarenza = CInt(XML_MovimentoDettaglio.GetAttribute("tempocarenza"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("doseetichetta") Then
                                        DoseEtichetta = CStr(XML_MovimentoDettaglio.GetAttribute("doseetichetta"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("doseetichetta_value") Then
                                        DoseEtichetta_Value = CStr(XML_MovimentoDettaglio.GetAttribute("doseetichetta_value"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("principiattivi") Then
                                        PrincipiAttivi = CStr(XML_MovimentoDettaglio.GetAttribute("principiattivi"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("classitossicologiche") Then
                                        ClassiTossicologiche = CStr(XML_MovimentoDettaglio.GetAttribute("classitossicologiche"))
                                    End If

                                    Dim Qta_Extra As Decimal = 0
                                    Dim Qta_Extra_Totale As Decimal = 0
                                    Dim Udm_Cod_Extra As Integer = 0
                                    Dim Mezzo_Det As Integer = 0

                                    If XML_MovimentoDettaglio.HasAttribute("qta_extra") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("qta_extra")) Then
                                        Qta_Extra = CStr(XML_MovimentoDettaglio.GetAttribute("qta_extra"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("qta_extra_totale") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("qta_extra_totale")) Then
                                        Qta_Extra_Totale = CStr(XML_MovimentoDettaglio.GetAttribute("qta_extra_totale"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("udm_cod_extra") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("udm_cod_extra")) Then
                                        Udm_Cod_Extra = CStr(XML_MovimentoDettaglio.GetAttribute("udm_cod_extra"))
                                    End If
                                    If XML_MovimentoDettaglio.HasAttribute("mezzo_det") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("mezzo_det")) Then
                                        Mezzo_Det = CStr(XML_MovimentoDettaglio.GetAttribute("mezzo_det"))
                                    End If

                                    Dim Extra_Str As String = ""

                                    If XML_MovimentoDettaglio.HasAttribute("extra_str") AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("extra_str")) Then
                                        Extra_Str = CStr(XML_MovimentoDettaglio.GetAttribute("extra_str"))
                                    End If


                                    strRicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                                                CInt(enum_TipoOperazioneDB.Scrittura),
                                                                PivaSuperUser,
                                                                CInt(Ricetta_Cod),
                                                                 0,
                                                                 0,
                                                                 1,
                                                                CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
                                                                CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
                                                                CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
                                                                CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
                                                                CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
                                                                CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
                                                                0,
                                                                Cau_Mov,
                                                                TempoCarenza,
                                                                DoseEtichetta,
                                                                PrincipiAttivi,
                                                                ClassiTossicologiche,
                                                                DoseEtichetta_Value,
                                                                ,,,,
                                                                Qta_Extra, Qta_Extra_Totale, Udm_Cod_Extra, Mezzo_Det, Extra_Str)


                                    XmlDoc2.LoadXml(strRicettaDettaglio)

                                    XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

                                    '----- Tag Movimento_Dettaglio_Tecnico_2  (multiplo)

                                    XMLs_Movimento_Dettaglio_Tecnico_2 = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Dettaglio_Tecnico_2")

                                    If XMLs_Movimento_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Movimento_Dettaglio_Tecnico_2.Count > 0 Then

                                        For n = 0 To XMLs_Movimento_Dettaglio_Tecnico_2.Count - 1

                                            XML_MovimentoDettaglioTecnico = XMLs_Movimento_Dettaglio_Tecnico_2.Item(n)

                                            Dim Inn1 As Date = AGRODATAINIZIO
                                            Dim Inn2 As Date = AGRODATAFINE
                                            If IsDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn1_data")) Then
                                                Inn1 = CDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn1_data"))
                                            End If
                                            If IsDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn2_data")) Then
                                                Inn2 = CDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn2_data"))
                                            End If

                                            Dim Mg As Decimal = 0
                                            Dim Azoto As Decimal = 0
                                            Dim P As Decimal = 0
                                            Dim K As Decimal = 0
                                            Dim Efficienza As Decimal = 0
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("mg")) Then
                                                Mg = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("mg"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("n")) Then
                                                Azoto = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("n"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("p")) Then
                                                P = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("p"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("k")) Then
                                                K = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("k"))
                                            End If
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("efficienza")) Then
                                                Efficienza = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("efficienza"))
                                            End If

                                            Dim Soglia_Cod As Integer = 0
                                            Dim Soglia_Des As String = ""
                                            Dim Soglia_Qta As Decimal = 0
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_cod")) Then
                                                Soglia_Cod = CInt(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_cod"))
                                            End If
                                            Soglia_Des = XML_MovimentoDettaglioTecnico.GetAttribute("soglia_des")
                                            If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_quantita")) Then
                                                Soglia_Qta = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_quantita"))
                                            End If

                                            Dim Cu As Decimal = 0
                                            If XML_MovimentoDettaglioTecnico.HasAttribute("cu") AndAlso IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("cu")) Then
                                                Cu = CInt(XML_MovimentoDettaglioTecnico.GetAttribute("cu"))
                                            End If

                                            strRicettaDettaglioTecnico = objXml.XML_Ricetta_DettaglioTecnico_2(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                            PivaSuperUser,
                                                                                                            CInt(Ricetta_Cod),
                                                                                                            , , ,
                                                                                                            1,
                                                                                                            CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("qta_ril")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("dett_cod")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_cod")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_gru")),
                                                                                                            CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("dose")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("parziale")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("nitrati")),
                                                                                                            CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("freatimetro")),
                                                                                                            Inn1, Inn2,
                                                                                                            ,
                                                                                                            ,
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ditta_cod")),
                                                                                                            CStr(XML_MovimentoDettaglioTecnico.GetAttribute("sigla_av")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("trap_num")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("id_insetto")),
                                                                                                            CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ff_classe")),
                                                                                                            CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_inizio")),
                                                                                                            CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_fine")),
                                                                                                            Mg,
                                                                                                            Azoto,
                                                                                                            P,
                                                                                                            K,
                                                                                                            Soglia_Cod,
                                                                                                            Soglia_Des,
                                                                                                            Soglia_Qta,
                                                                                                            Efficienza,,
                                                                                                               ,,,,
                                                                                                               Cu)

                                            strRicettaDettagliTecnici &= strRicettaDettaglioTecnico

                                        Next


                                    End If


                                    '----- Tag Movimento_Destinazione  (multiplo)

                                    XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                                    If XMLs_MovimentoDestinazione IsNot Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

                                        For n = 0 To XMLs_MovimentoDestinazione.Count - 1

                                            XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)

                                            Dim Qta2 As Decimal = 0D
                                            If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
                                                Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                                            End If

                                            Dim Tipo_Destinazione As Integer
                                            If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
                                                Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                                            End If

                                            strRicettaDestinazione = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                     PivaSuperUser,
                                                                                                     CInt(Ricetta_Cod),
                                                                                                     , , ,
                                                                                                     CStr(XML_MovimentoDestinazione.GetAttribute("piva")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("appezza")),
                                                                                                     CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione")),
                                                                                                     CInt(IIf(IsNumeric(XML_MovimentoDestinazione.GetAttribute("programmazione_entita_cod")), XML_MovimentoDestinazione.GetAttribute("programmazione_entita_cod"), 0)),
                                                                                                     CDbl(XML_MovimentoDestinazione.GetAttribute("qta")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                                                     CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")),
                                                                                                     CDbl(IIf(IsNumeric(XML_MovimentoDestinazione.GetAttribute("quotadistribuzione")), XML_MovimentoDestinazione.GetAttribute("quotadistribuzione"), 0)),
                                                                                                     Qta2,
                                                                                                     Tipo_Destinazione) ' errore stringa programmazione_entità_cod

                                            strRicettaDestinazioni &= strRicettaDestinazione

                                        Next

                                    End If

                                    XML_RicettaDettaglio.InnerXml = strRicettaDettagliTecnici & strRicettaDestinazioni

                                    Dim pro As String = XML_RicettaDettaglio.OuterXml

                                    XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

                                Next

                            End If

                        End If

                    End If

                Case CAU_IMPUTAZIONE_PARCOMACCHINE,
                     CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                     CAU_IMPUTAZIONE_MANODOPERA,
                     CAU_IMPUTAZIONE_TERZISTI,
                     CAU_SCARICO

                    '-----------------------------------------
                    '-----------------------------------------
                    '----- Tag DatiMovimenti_Dettagli
                    '-----------------------------------------
                    '-----------------------------------------

                    strRicettaDestinazioni = ""
                    idAgendaScaricoDaAzPadre = 0
                    Piva_Magazzino = ""
                    Sa_Cod_Magazzino = 0
                    Id_Destinazione_Magazzino = 0

                    '(14/04/2020 fede) introdotto controllo scarico da altre aziende
                    'in questo caso, sulla ricetta non creo altre operazioni (come in agenda)
                    'ma salvo i dati del magazzino da cui scarico nel movimento di scarico
                    If Cau_Mov = CAU_SCARICO Then

                        Dim dtRif As DataTable
                        Dim drRif As DataRow()
                        Dim objRifR As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                        dtRif = objRifR.Recupera_DT_Rif_Unificato(Piva,
                                                            Sa_Cod,
                                                            Id_Agenda,
                                                            0, 0, 0, "",
                                                            objParametri_Server)

                        If dtRif IsNot Nothing AndAlso dtRif.Rows.Count > 0 Then
                            drRif = dtRif.Select("Lav_Cod_Risultato=" & LAVCOD_SCARICO)
                            If drRif IsNot Nothing AndAlso drRif.Length > 0 Then
                                idAgendaScaricoDaAzPadre = drRif(0).Item("Id_Agenda_Risultato")
                                Piva_Magazzino = drRif(0).Item("Piva_Risultato")
                            End If
                        End If

                        If idAgendaScaricoDaAzPadre <> 0 Then
                            Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                            Dim DtDest As DataTable
                            DtDest = objDest.Leggi(Piva_Magazzino, 0, idAgendaScaricoDaAzPadre,
                                                                0, 0, 0, 0, CostantiPersonalizzate.MAGAZZINO, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri_Server)
                            If DtDest IsNot Nothing AndAlso DtDest.Rows.Count > 0 Then
                                Piva_Magazzino = DtDest.Rows(0).Item("piva")
                                Sa_Cod_Magazzino = DtDest.Rows(0).Item("sa_cod")
                                Id_Destinazione_Magazzino = DtDest.Rows(0).Item("id_destinazione")
                            End If
                        End If

                    End If



                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                    If XML_DatiMovimentiDettagli IsNot Nothing Then

                        '----- Tag Movimento_Dettaglio  (multiplo)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                        If XMLs_MovimentoDettaglio IsNot Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

                            For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                                strRicettaDestinazioniCostiAccessori = ""

                                XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                                Dim Qualifica_Cod As Integer = 0
                                Dim Tariffa_Cod As Integer = 0
                                Dim prezzo_unitario As Decimal = 0
                                Dim id_attivita As Integer = 0
                                Dim lotto As String = ""

                                If XML_MovimentoDettaglio.HasAttribute("qualifica_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("qualifica_cod")) Then
                                    Qualifica_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("qualifica_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("tariffa_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("tariffa_cod")) Then
                                    Tariffa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("tariffa_cod"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("prezzo_unitario") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario")) Then
                                    prezzo_unitario = CDec(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("id_attivita") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("id_attivita")) Then
                                    id_attivita = CInt(XML_MovimentoDettaglio.GetAttribute("id_attivita"))
                                End If

                                If XML_MovimentoDettaglio.HasAttribute("lotto") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("lotto")) Then
                                    lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                                End If


                                strRicettaDettaglioCostiAccessori = objXml.XML_Ricetta_Dettaglio(
                                                                            CInt(enum_TipoOperazioneDB.Scrittura),
                                                                            PivaSuperUser,
                                                                            CInt(Ricetta_Cod),
                                                                            0,
                                                                            0,
                                                                            0,
                                                                            CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
                                                                            CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
                                                                            CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
                                                                            CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
                                                                            CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
                                                                            CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
                                                                            CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                            CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
                                                                            prezzo_unitario,
                                                                            Cau_Mov,
                                                                            Qualifica_cod:=Qualifica_Cod,
                                                                            Tariffa_cod:=Tariffa_Cod,
                                                                            id_attivita:=id_attivita,
                                                                            lotto:=lotto)


                                XmlDoc3.LoadXml(strRicettaDettaglioCostiAccessori)

                                XML_RicettaDettaglio = XmlDoc3.SelectSingleNode("Ricetta_Dettaglio")

                                '----- Tag Movimento_Destinazione  (multiplo)

                                XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                                If XMLs_MovimentoDestinazione IsNot Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

                                    For n = 0 To XMLs_MovimentoDestinazione.Count - 1

                                        XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)


                                        Dim Qta2 As Decimal = 0D
                                        If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
                                            Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                                        End If

                                        Dim Tipo_Destinazione As Integer
                                        If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
                                            Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                                        End If

                                        Dim Piva_Dest As String = CStr(XML_MovimentoDestinazione.GetAttribute("piva"))
                                        Dim Sa_Cod_Dest As String = CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod"))
                                        Dim Id_Destinazione_Dest As String = CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione"))

                                        If Piva_Magazzino <> "" AndAlso Sa_Cod_Magazzino <> 0 AndAlso Id_Destinazione_Magazzino <> 0 Then
                                            Piva_Dest = Piva_Magazzino
                                            Sa_Cod_Dest = Sa_Cod_Magazzino
                                            Id_Destinazione_Dest = Id_Destinazione_Magazzino
                                        End If

                                        strRicettaDestinazioneCostiAccessori = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                                                 PivaSuperUser,
                                                                                                 CInt(Ricetta_Cod),
                                                                                                 , , ,
                                                                                                 Piva_Dest,
                                                                                                 Sa_Cod_Dest,
                                                                                                 CInt(XML_MovimentoDestinazione.GetAttribute("appezza")),
                                                                                                 Id_Destinazione_Dest,
                                                                                                 0,
                                                                                                 CDbl(XML_MovimentoDestinazione.GetAttribute("qta")),
                                                                                                 CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                                                 CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")),
                                                                                                 0,
                                                                                                 Qta2,
                                                                                                 Tipo_Destinazione)

                                        strRicettaDestinazioniCostiAccessori &= strRicettaDestinazioneCostiAccessori

                                    Next

                                End If

                                XML_RicettaDettaglio.InnerXml = strRicettaDestinazioniCostiAccessori

                                Dim pro As String = XML_RicettaDettaglio.OuterXml

                                strRicettaDettagliCostiAccessori &= XML_RicettaDettaglio.OuterXml

                            Next


                        End If

                    End If

            End Select

        Next

        XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & strRicettaDettagliCostiAccessori

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml

    End Function

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Ricette_Operazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiRicetteDestinazioni(ByVal Id_Cdg_Generale As Integer, ByRef RicetteRiferimenti As List(Of APP_Riferimenti_Interventi_Cdg),
                                             ByRef RicetteDettagli As List(Of APP_Ricette_Destinazioni)) As List(Of APP_Ricette_Destinazioni)

        Dim riferimenti = RicetteRiferimenti.Where(Function(x) (x.Id_Cdg_Generale_Rif = Id_Cdg_Generale)).FirstOrDefault()
        Dim destinazioni = New List(Of APP_Ricette_Destinazioni)
        If riferimenti IsNot Nothing Then
            destinazioni = RicetteDettagli.Where(Function(x) (x.Ricetta_Cod = riferimenti.Ricetta_Cod AndAlso x.Ricetta_Operazione_Cod = riferimenti.Ricetta_Operazione_Cod)).ToList()
        End If
        Return destinazioni

    End Function

    Public Function NuovaAttivita(ByVal unid As String, ByRef attivitaMovimentiDascrivere As APP_CDG_Movimenti) As APP_CDG_Generale

        Dim nuovaAttivitaDaScrivere = New APP_CDG_Generale
        With nuovaAttivitaDaScrivere
            .ID = unid & "*|" & attivitaMovimentiDascrivere.Id_Cdg_Generale
            .Id_Cdg_Generale = attivitaMovimentiDascrivere.Id_Cdg_Generale
            .Piva_Superuser = attivitaMovimentiDascrivere.Piva_Superuser
            .Piva = attivitaMovimentiDascrivere.Piva
            .Data_Inserimento = CDate(attivitaMovimentiDascrivere.Data_Ora_Fine).Date
            .inviato = attivitaMovimentiDascrivere.inviato
            .datainvio = attivitaMovimentiDascrivere.datainvio
            .Data_Creazione = attivitaMovimentiDascrivere.Data_Creazione
            .Data_Modifica = attivitaMovimentiDascrivere.Data_Modifica
            .Username_Creazione = attivitaMovimentiDascrivere.Username_Creazione
            .Username_Modifica = attivitaMovimentiDascrivere.Username_Modifica
            .Validita_Inizio = attivitaMovimentiDascrivere.Validita_Inizio
            .Validita_Fine = attivitaMovimentiDascrivere.Validita_Fine
        End With

        Return nuovaAttivitaDaScrivere

    End Function

    Public Function NuovaAttivitaIndiretta(ByVal unid As String, ByVal Piva As String, ByRef attivitaMovimentiDascrivere As APP_CDG_Movimenti) As APP_CDG_Generale

        Dim nuovaAttivitaIndirettaDaScrivere = New APP_CDG_Generale
        With nuovaAttivitaIndirettaDaScrivere
            .ID = "*" & unid & "|" & attivitaMovimentiDascrivere.Id_Cdg_Generale
            .Id_Cdg_Generale = attivitaMovimentiDascrivere.Id_Cdg_Generale
            .Data_Inserimento = attivitaMovimentiDascrivere.Data_Ora_Inizio
            .Piva_Superuser = attivitaMovimentiDascrivere.Piva_Superuser
            .Piva = Piva
            .inviato = attivitaMovimentiDascrivere.inviato
            .datainvio = attivitaMovimentiDascrivere.datainvio
            .Data_Creazione = attivitaMovimentiDascrivere.Data_Creazione
            .Data_Modifica = attivitaMovimentiDascrivere.Data_Modifica
            .Username_Creazione = attivitaMovimentiDascrivere.Username_Creazione
            .Username_Modifica = attivitaMovimentiDascrivere.Username_Modifica
            .Validita_Inizio = attivitaMovimentiDascrivere.Validita_Inizio
            .Validita_Fine = attivitaMovimentiDascrivere.Validita_Fine
        End With

        Return nuovaAttivitaIndirettaDaScrivere

    End Function

    Public Function SpezzaAttivita(ByVal unid As String, ByRef attivitaMovimentiDascrivere As APP_CDG_Movimenti, ByRef destinazione As APP_Ricette_Destinazioni) As APP_CDG_Movimenti

        Dim nuovoMovimentoDaScrivere = New APP_CDG_Movimenti
        With nuovoMovimentoDaScrivere
            .ID = unid & "*|" & attivitaMovimentiDascrivere.Id_CDG_Movimenti & If(destinazione Is Nothing, "", "|" & destinazione.Ricetta_Destinazione_Cod)
            .Id_Cdg_Generale = attivitaMovimentiDascrivere.Id_Cdg_Generale
            .Id_CDG_Movimenti = attivitaMovimentiDascrivere.Id_CDG_Movimenti
            .Piva_Superuser = attivitaMovimentiDascrivere.Piva_Superuser
            .Piva = attivitaMovimentiDascrivere.Piva
            .Sa_Cod = attivitaMovimentiDascrivere.Sa_Cod
            .Id_Attivita = attivitaMovimentiDascrivere.Id_Attivita
            .Id_Imputazione = attivitaMovimentiDascrivere.Id_Imputazione
            .Cod_RisUm = attivitaMovimentiDascrivere.Cod_RisUm
            .NrBadge = attivitaMovimentiDascrivere.NrBadge
            .Mac_Cod = attivitaMovimentiDascrivere.Mac_Cod
            .Qta = attivitaMovimentiDascrivere.Qta
            .Data_Ora_Inizio = CDate(attivitaMovimentiDascrivere.Data_Ora_Fine).Date
            .Data_Ora_Fine = attivitaMovimentiDascrivere.Data_Ora_Fine
            .Appezza = If(destinazione Is Nothing, attivitaMovimentiDascrivere.Appezza, destinazione.Appezza)
            .Id_Reg = If(destinazione Is Nothing, attivitaMovimentiDascrivere.Id_Reg, destinazione.Id_Reg)
            .Progetto_Cod = attivitaMovimentiDascrivere.Progetto_Cod
            .inviato = attivitaMovimentiDascrivere.inviato
            .datainvio = attivitaMovimentiDascrivere.datainvio
            .Data_Creazione = attivitaMovimentiDascrivere.Data_Creazione
            .Data_Modifica = attivitaMovimentiDascrivere.Data_Modifica
            .Username_Creazione = attivitaMovimentiDascrivere.Username_Creazione
            .Username_Modifica = attivitaMovimentiDascrivere.Username_Modifica
            .Validita_Inizio = attivitaMovimentiDascrivere.Validita_Inizio
            .Validita_Fine = attivitaMovimentiDascrivere.Validita_Fine
        End With

        Return nuovoMovimentoDaScrivere

    End Function

    Public Sub ScriviAttivitaIndiretta(ByVal unid As String, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Id_Attivita As Integer, ByVal Id_Imputazione As Integer,
                                       ByRef listaChiaviMovimenti As List(Of String), ByRef attivitaMovimentiDascrivere As APP_CDG_Movimenti,
                                       ByRef EFArrayToInsert As ArrayList)

        Dim chiave As String = unid
        chiave = chiave & "|" & attivitaMovimentiDascrivere.Data_Ora_Inizio.ToString
        chiave = chiave & "|" & attivitaMovimentiDascrivere.Piva & "|" & attivitaMovimentiDascrivere.Sa_Cod
        chiave = chiave & "|" & attivitaMovimentiDascrivere.Cod_RisUm & "|" & attivitaMovimentiDascrivere.Mac_Cod

        If Not listaChiaviMovimenti.Contains(chiave) Then

            Dim attivitaMovimentiIndirettaDascrivere = New APP_CDG_Movimenti
            With attivitaMovimentiIndirettaDascrivere
                .ID = "*" & attivitaMovimentiDascrivere.ID
                .Id_Cdg_Generale = attivitaMovimentiDascrivere.Id_Cdg_Generale
                .Id_CDG_Movimenti = attivitaMovimentiDascrivere.Id_CDG_Movimenti
                .Piva_Superuser = attivitaMovimentiDascrivere.Piva_Superuser
                .Piva = Piva
                .Sa_Cod = Sa_Cod
                .Id_Attivita = Id_Attivita
                .Id_Imputazione = Id_Imputazione
                .Cod_RisUm = attivitaMovimentiDascrivere.Cod_RisUm
                .NrBadge = attivitaMovimentiDascrivere.NrBadge
                .Mac_Cod = attivitaMovimentiDascrivere.Mac_Cod
                .Qta = attivitaMovimentiDascrivere.Qta
                .Data_Ora_Fine = attivitaMovimentiDascrivere.Data_Ora_Fine
                .Data_Ora_Inizio = attivitaMovimentiDascrivere.Data_Ora_Inizio
                .Appezza = 0
                .Id_Reg = 0
                .Progetto_Cod = 0
                .inviato = attivitaMovimentiDascrivere.inviato
                .datainvio = attivitaMovimentiDascrivere.datainvio
                .Data_Creazione = attivitaMovimentiDascrivere.Data_Creazione
                .Data_Modifica = attivitaMovimentiDascrivere.Data_Modifica
                .Username_Creazione = attivitaMovimentiDascrivere.Username_Creazione
                .Username_Modifica = attivitaMovimentiDascrivere.Username_Modifica
                .Validita_Inizio = attivitaMovimentiDascrivere.Validita_Inizio
                .Validita_Fine = attivitaMovimentiDascrivere.Validita_Fine
            End With
            EFArrayToInsert.Add(attivitaMovimentiIndirettaDascrivere)
            listaChiaviMovimenti.Add(chiave)
        End If

    End Sub

    Public Sub Normalizza_Attivita(ByVal Piva As String,
                                   ByRef RicetteDettagli As List(Of APP_Ricette_Dettagli),
                                   ByRef Attivita As List(Of APP_CDG_Generale),
                                   ByRef AttivitaMovimenti As List(Of APP_CDG_Movimenti),
                                   ByRef AttivitaOperazioni As List(Of APP_Riferimenti_Interventi_Cdg),
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri)

        ' normalizza macchine/operatori in base all'impostazione su controllo gestione
        Dim leggi_CDG_R As New CDG_BIZ_R
        Dim tipoCdG = leggi_CDG_R.GetTipoCdG(Piva, objParametri_Server, objParametri_Utenti)

        If tipoCdG = enum_TipoCdG.NuovoTipo Then

            Dim causaliCostiAccessori = New List(Of String)() From {
                CAU_IMPUTAZIONE_PARCOMACCHINE,
                CAU_IMPUTAZIONE_MANODOPERA,
                CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                CAU_IMPUTAZIONE_TERZISTI
            }

            If Attivita.Count > 0 Then

                ' rimuove macchine/operatori perchè gestiti in controllo gestione
                'RicetteDettagli.RemoveAll(Function(d) causaliCostiAccessori.Contains(d.Cau_Mov))

                ' TODO: rimuovere da AttivitaMovimenti eventuali macchine non visibili in controllo gestione

            End If

        Else

            ' rimuovere dati attività cdg se non attiva la gestione costi
            'Attivita.Clear()
            'AttivitaMovimenti.Clear()
            'AttivitaOperazioni.Clear()

        End If

    End Sub

    Public Sub Normalizza_Ricette(ByVal Piva As String, ByVal Sa_Cod As Integer,
                                  ByRef RicetteDettagli As List(Of APP_Ricette_Dettagli),
                                  ByRef RicetteDestinazioni As List(Of APP_Ricette_Destinazioni),
                                  ByRef objParametri_Server As AgronicaCoreParametri)

        ' legge magazzino azienda su cui imputare
        Dim dalFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim dalFabbricatiCodici As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R

        Dim Fabbricato_Cod As Integer = 0
        Dim dtMagazziniAPP = dalFabbricatiCodici.Leggi(Piva, Sa_Cod, 0, enum_CodiciAnagrafe.Visibile_da_App, "1", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        If dtMagazziniAPP IsNot Nothing AndAlso dtMagazziniAPP.Rows.Count > 0 Then
            Fabbricato_Cod = dtMagazziniAPP.Rows(0).Item("Fabbricato_Cod")
        End If

        Dim dtMagazzini = dalFabbricati.Leggi(Piva, Sa_Cod, Fabbricato_Cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        If dtMagazzini IsNot Nothing AndAlso dtMagazzini.Rows.Count > 0 Then

            Dim magazzino = dtMagazzini.Rows(0)
            Dim chiaveMagazzino = magazzino.Item("Piva") & "-" & magazzino.Item("Sa_Cod") & "-" & magazzino.Item("Fabbricato_Cod")

            Dim mapRicetteDettagli As New Dictionary(Of Integer, APP_Ricette_Dettagli)
            For Each rd In RicetteDestinazioni
                If rd.Tipo_Destinazione = 20 Then
                    For Each dettaglio In RicetteDettagli
                        If rd.Ricetta_Dettaglio_Cod = dettaglio.Ricetta_Dettaglio_Cod AndAlso dettaglio.Cau_Mov = CAU_SCARICO Then
                            mapRicetteDettagli.Add(rd.Ricetta_Destinazione_Cod, dettaglio)
                        End If
                    Next
                End If
            Next

            For Each destinazione In RicetteDestinazioni

                If Piva <> destinazione.Piva AndAlso destinazione.Tipo_Destinazione = 20 Then

                    Dim dtMagazzinoAgenzia = dalFabbricati.Leggi(destinazione.Piva, destinazione.Sa_Cod, destinazione.Id_Reg, enumSelezioneVariabile.Selezione_LogOmni, "", "", objParametri_Server)

                    If dtMagazzinoAgenzia IsNot Nothing AndAlso dtMagazzinoAgenzia.Rows.Count > 0 Then

                        Dim prodottoAgenzia As Boolean = True
                        Dim magazzinoAgenzia = dtMagazzinoAgenzia.Rows(0)
                        Dim dettaglio = mapRicetteDettagli(destinazione.Ricetta_Destinazione_Cod)
                        Dim chiaveProdotto As String = dettaglio.Elem_Cod & "-" & dettaglio.Pro_Cod & "-" & dettaglio.Mat_Cod

                        'controlla se c'è già una riga con quel prodotto / magazzino
                        'For Each rd In RicetteDestinazioni
                        '    If Piva = rd.Piva And rd.Tipo_Destinazione = 20 Then
                        '        Dim d = mapRicetteDettagli(rd.Ricetta_Destinazione_Cod)
                        '        If chiaveMagazzino = rd.Piva & "-" & rd.Sa_Cod & "-" & rd.Id_Reg Then
                        '            If chiaveProdotto = d.Elem_Cod & "-" & d.Pro_Cod & "-" & d.Mat_Cod Then
                        '                d.Qta += dettaglio.Qta
                        '                rd.Qta += destinazione.Qta
                        '                Dim separatore As String = If(String.IsNullOrEmpty(rd.MagazzinoEsterno_Cod), "", "|")
                        '                rd.MagazzinoEsterno_Cod &= separatore & destinazione.Piva & "-" & destinazione.Sa_Cod & "-" & destinazione.Id_Reg
                        '                rd.MagazzinoEsterno_Des &= separatore & magazzinoAgenzia.Item("Fabbricato_Des_Estesa")
                        '                rd.MagazzinoEsterno_Dettagli &= separatore & dettaglio.Qta
                        '                prodottoAgenzia = False
                        '                Exit For
                        '            End If
                        '        End If
                        '    End If
                        'Next

                        If prodottoAgenzia Then
                            destinazione.MagazzinoEsterno_Cod = destinazione.Piva & "-" & destinazione.Sa_Cod & "-" & destinazione.Id_Reg
                            destinazione.MagazzinoEsterno_Des = magazzinoAgenzia.Item("Fabbricato_Des_Estesa")
                            destinazione.MagazzinoEsterno_Dettagli = dettaglio.Qta
                            destinazione.Piva = magazzino.Item("Piva")
                            destinazione.Sa_Cod = magazzino.Item("Sa_Cod")
                            destinazione.Id_Reg = magazzino.Item("Fabbricato_Cod")
                        Else
                            ' TODO gestire accorpamento righe stesso prodotto/magazzino
                            ' dettaglio.Ricetta_Cod = 0
                            ' destinazione.Ricetta_Cod = 0
                        End If

                    End If

                End If

            Next

        End If

    End Sub

    Public Sub Ricetta_Operazione_ScriviPerAPP(
        ByVal unid As String,
        Ricette As List(Of APP_Ricette),
        RicetteOperazioni As List(Of APP_Ricette_Operazioni),
        RicetteDettagli As List(Of APP_Ricette_Dettagli),
        RicetteDettaglioTecnico As List(Of APP_Ricette_Dettaglio_Tecnico),
        RicetteXNote As List(Of APP_RicettexNote),
        RicetteDestinazioni As List(Of APP_Ricette_Destinazioni),
        Attivita As List(Of APP_CDG_Generale),
        AttivitaMovimenti As List(Of APP_CDG_Movimenti),
        AttivitaOperazioni As List(Of APP_Riferimenti_Interventi_Cdg),
        objParametri_Server As AgronicaCoreParametri,
        Optional cancellazione As Boolean = False
    )

        ' per ribaltamento ore indirette
        Dim xLettura As New AgronicaCoreContabBIZ.CDG_APP
        Dim dtConfig = xLettura.LeggiConfigRibaltamentoOre(objParametri_Server)
        Dim attivitaIndiretta = dtConfig.Rows.Count > 0
        Dim splitAttivita As Boolean = True

        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList

        Try

            For Each ricettaDaScrivere In Ricette
                ricettaDaScrivere.ID = unid & "|" & ricettaDaScrivere.Ricetta_Cod
                EFArrayToInsert.Add(ricettaDaScrivere)
            Next

            For Each operazioneDaScrivere In RicetteOperazioni
                operazioneDaScrivere.ID = unid & "|" & operazioneDaScrivere.Ricetta_Operazione_Cod
                EFArrayToInsert.Add(operazioneDaScrivere)
            Next

            For Each dettaglioDaScrivere In RicetteDettagli
                If dettaglioDaScrivere.Ricetta_Cod <> 0 Then
                    dettaglioDaScrivere.ID = unid & "|" & dettaglioDaScrivere.Ricetta_Dettaglio_Cod
                    EFArrayToInsert.Add(dettaglioDaScrivere)
                End If
            Next

            For Each dettaglioTecnicoDaScrivere In RicetteDettaglioTecnico
                dettaglioTecnicoDaScrivere.ID = unid & "|" & dettaglioTecnicoDaScrivere.Ricetta_Tecnico_Cod
                EFArrayToInsert.Add(dettaglioTecnicoDaScrivere)
            Next

            For Each destinazioneDaScrivere In RicetteDestinazioni
                If destinazioneDaScrivere.Ricetta_Cod <> 0 Then
                    destinazioneDaScrivere.ID = unid & "|" & destinazioneDaScrivere.Ricetta_Destinazione_Cod
                    EFArrayToInsert.Add(destinazioneDaScrivere)
                End If
            Next

            For Each ricettaNoteDascrivere In RicetteXNote
                ricettaNoteDascrivere.ID = unid & "|" & ricettaNoteDascrivere.Ricetta_Operazione_Cod & "|" & ricettaNoteDascrivere.Nota_Cod
                EFArrayToInsert.Add(ricettaNoteDascrivere)
            Next

            For Each attivitaDascrivere In Attivita
                attivitaDascrivere.ID = unid & "|" & attivitaDascrivere.Id_Cdg_Generale
                attivitaDascrivere.Piva_Superuser = objParametri_Server.PivaSuperUser
                EFArrayToInsert.Add(attivitaDascrivere)
            Next

            Dim listaChiaviMovimenti = New List(Of String)
            Dim listaAttivitaSpezzate = New List(Of String)
            Dim listaAttivitaIndirette = New List(Of String)
            For Each attivitaMovimentiDascrivere In AttivitaMovimenti

                Dim attivitaGeneraleSpezzata As APP_CDG_Generale = Nothing
                Dim attivitaMovimentoSpezzata As APP_CDG_Movimenti = Nothing
                attivitaMovimentiDascrivere.ID = unid & "|" & attivitaMovimentiDascrivere.Id_CDG_Movimenti
                attivitaMovimentiDascrivere.Piva_Superuser = objParametri_Server.PivaSuperUser

                If attivitaMovimentiDascrivere.Data_Ora_Fine = Date.MinValue Then
                    attivitaMovimentiDascrivere.Data_Ora_Fine = AGRODATAFINE
                End If

                ' spezza l'attività a cavallo del cambio giorno (data fine > data inizio)
                If splitAttivita AndAlso attivitaMovimentiDascrivere.Data_Ora_Fine <> AGRODATAFINE Then
                    If CDate(attivitaMovimentiDascrivere.Data_Ora_Fine).Date > CDate(attivitaMovimentiDascrivere.Data_Ora_Inizio).Date Then
                        Dim idAttivitaSpezzata = unid & "*|" & attivitaMovimentiDascrivere.Id_Cdg_Generale
                        If Not listaAttivitaSpezzate.Contains(idAttivitaSpezzata) Then
                            attivitaGeneraleSpezzata = NuovaAttivita(unid, attivitaMovimentiDascrivere)
                            EFArrayToInsert.Add(attivitaGeneraleSpezzata)
                            listaAttivitaSpezzate.Add(idAttivitaSpezzata)
                        End If
                        ' se attività collegata a impianti QdC li riporto gli impianti da ricette_destinazioni a cdg_movimenti
                        Dim destinazioniMovimenti = LeggiRicetteDestinazioni(attivitaMovimentiDascrivere.Id_Cdg_Generale, AttivitaOperazioni, RicetteDestinazioni)
                        If destinazioniMovimenti.Count > 0 Then
                            For Each destinazione In destinazioniMovimenti
                                attivitaMovimentoSpezzata = SpezzaAttivita(unid, attivitaMovimentiDascrivere, destinazione)
                                EFArrayToInsert.Add(attivitaMovimentoSpezzata)
                            Next
                        Else
                            attivitaMovimentoSpezzata = SpezzaAttivita(unid, attivitaMovimentiDascrivere, Nothing)
                            EFArrayToInsert.Add(attivitaMovimentoSpezzata)
                        End If
                        ' forzo la data fine sul movimento spezzato
                        attivitaMovimentiDascrivere.Data_Ora_Fine = AGRODATAFINE
                    End If
                End If

                EFArrayToInsert.Add(attivitaMovimentiDascrivere)

                If attivitaIndiretta Then

                    ' ricavo il codice imputazione x le attività indirette
                    Dim Piva = attivitaMovimentiDascrivere.Piva
                    Dim Sa_Cod = attivitaMovimentiDascrivere.Sa_Cod
                    Dim Id_Attivita = attivitaMovimentiDascrivere.Id_Attivita
                    Dim Id_Imputazione = 0
                    xLettura.LeggiImputazioneOreIndirette(Piva, Sa_Cod, Id_Attivita, Id_Imputazione, dtConfig)

                    ' inserisce attivita indiretta compresa quella spezzata
                    If Id_Imputazione <> 0 Then

                        ' crea CDG generale per indirette e spezzate indirette
                        Dim idAttivitaIndiretta = "*" & unid & "|" & attivitaMovimentiDascrivere.Id_Cdg_Generale
                        If Not listaAttivitaIndirette.Contains(idAttivitaIndiretta) Then
                            Dim attivitaGeneraleIndiretta = NuovaAttivitaIndiretta(unid, Piva, attivitaMovimentiDascrivere)
                            EFArrayToInsert.Add(attivitaGeneraleIndiretta)
                            If attivitaMovimentoSpezzata IsNot Nothing Then
                                Dim attivitaGeneraleIndirettaSpezzata = NuovaAttivitaIndiretta(unid & "*", Piva, attivitaMovimentiDascrivere)
                                EFArrayToInsert.Add(attivitaGeneraleIndirettaSpezzata)
                            End If
                            listaAttivitaIndirette.Add(idAttivitaIndiretta)
                        End If

                        ' scrivi movimenti indirette e spezzate indirette
                        ScriviAttivitaIndiretta(unid, Piva, Sa_Cod, Id_Attivita, Id_Imputazione, listaChiaviMovimenti, attivitaMovimentiDascrivere, EFArrayToInsert)
                        If attivitaMovimentoSpezzata IsNot Nothing Then
                            ScriviAttivitaIndiretta(unid & "*", Piva, Sa_Cod, Id_Attivita, Id_Imputazione, listaChiaviMovimenti, attivitaMovimentoSpezzata, EFArrayToInsert)
                        End If

                    End If

                End If

            Next

            For Each attivitaOperazioniDascrivere In AttivitaOperazioni
                attivitaOperazioniDascrivere.ID = unid & "|" & attivitaOperazioniDascrivere.Id_Cdg_Generale_Rif & "|" & attivitaOperazioniDascrivere.Ricetta_Cod
                attivitaOperazioniDascrivere.Piva_Superuser = objParametri_Server.PivaSuperUser
                EFArrayToInsert.Add(attivitaOperazioniDascrivere)
            Next

            Dim scriviRicette As New AgronicaCoreContabDAL.Ricette_W
            Dim err As String = ""
            If cancellazione Then
                err = scriviRicette.Aggiorna_RicetteAPP(EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri_Server, unid)
            Else
                err = scriviRicette.Aggiorna_RicettePerAPP(EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri_Server)
            End If

            If err <> "" Then
                Throw New Exception(err)
            End If

        Catch ex As Exception

            Throw ex

        End Try

    End Sub

    '============================================================================
    Public Function Ricetta_Operazione_Scrivi(ByVal Piva As String, ByVal Sa_Cod As Integer,
                                              ByVal DatiRicetta_Operazione As String,
                                              ByVal Ricetta_Cod As Int32,
                                              ByVal Ricetta_Tipo As Integer,
                                              ByRef OUTPUT_Ricetta_Operazione_Cod As Int32,
                                              ByRef objParametri As AgronicaCoreParametri,
                                                Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                                Optional ByVal OpenTransaction As Boolean = True,
                                                Optional ByVal OpenConnection As Boolean = True
                                              ) As Boolean


        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Ricette_Operazioni_W.Ricetta_Operazione_Scrivi()"

        '----------------------------------------------------------------------

        Dim Dummy As Boolean
        Dim XmlDoc As XmlDocument

        Dim xDatiRicetta_Operazioni As XmlNodeList
        Dim xDatiRicetta_Operazione As XmlElement
        Dim xRicetta_Operazioni As XmlNodeList
        Dim xRicetta_Operazione As XmlElement

        ' Dim XmlDatiRicetta_Dettagli As XmlNodeList
        ' Dim XmlDatiRicetta_Dettaglio As XmlElement
        Dim xRicetta_Dettagli_Tecnici As XmlNodeList
        Dim xRicetta_Dettaglio_Tecnico As XmlElement
        Dim xRicetta_Dettagli As XmlNodeList
        Dim xRicetta_Dettaglio As XmlElement
        Dim xRicetta_Destinazioni As XmlNodeList
        Dim xRicetta_Destinazione As XmlElement


        Dim ObjSequenze As Agro_Sequenze
        Dim objRicetta_Operazioni As AgronicaCoreContabDAL.Ricette_Operazioni_W
        Dim objRicetta_Dettagli As AgronicaCoreContabDAL.Ricette_Dettagli_W
        Dim objRicetta_Dettaglio_Tecnico As AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W
        Dim objRicetta_Destinazione As AgronicaCoreContabDAL.Ricette_Destinazioni_W
        Dim ObjRicetteLog As AgronicaCoreContabDAL.AgronicaLogRicette_W

        Dim objRicetta_Dettagli_R As AgronicaCoreContabDAL.Ricette_Dettagli_R

        Dim Ricetta_Operazione_Cod As Integer
        Dim Ricetta_Tecnico_Cod As Integer

        Dim Miscela_Cod As Integer
        Dim Ricetta_Dettaglio_Cod As Integer
        Dim Ricetta_Destinazione_Cod As Integer
        Dim Ricetta_Dettaglio_Tecnico_Cod As Integer

        Dim Data_Creazione As Date = AGRODATAINIZIO
        Dim Username_Creazione As String = ""

        Dim Qta_Acqua As String

        Dim i_DatiRicetta_Operazioni As Integer
        Dim i_Ricetta_Operazioni As Integer
        Dim i_Ricetta_Dettaglio As Integer
        Dim i_Ricetta_Dettaglio_Tecnico As Integer
        Dim i_Ricetta_Destinazione As Integer

        Dim OpeDB_Ricetta_Operazione As String
        Dim OpeDB_Ricetta_Dettaglio_Tecnico As String
        Dim OpeDB_Ricetta_Dettaglio As String
        Dim OpeDB_Ricetta_Destinazione As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '------------------------------

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing AndAlso OpenConnection Then
                'Richiedo una connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri.objTransazione Is Nothing AndAlso OpenTransaction Then
                'Inizializzo la transazione
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If

            '------------------------------

            XmlDoc = New XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiRicetta_Operazione)


            xDatiRicetta_Operazioni = XmlDoc.GetElementsByTagName("DatiRicetta_Operazioni")

            i_DatiRicetta_Operazioni = 0

            Do While i_DatiRicetta_Operazioni < xDatiRicetta_Operazioni.Count

                'Prelevo l'i-esimo blocco di DatiRicetta_Operazioni (in realta' ne esiste uno solo)
                xDatiRicetta_Operazione = xDatiRicetta_Operazioni.Item(i_DatiRicetta_Operazioni)

                '------------------------------

                xRicetta_Operazioni = xDatiRicetta_Operazione.GetElementsByTagName("Ricetta_Operazione")

                i_Ricetta_Operazioni = 0

                Do While i_Ricetta_Operazioni < xRicetta_Operazioni.Count

                    'Prelevo l' i-esimo Ricetta_Operazione
                    xRicetta_Operazione = xRicetta_Operazioni.Item(i_Ricetta_Operazioni)

                    'Prelevo gli attributi dell'Operazione selezionata
                    OpeDB_Ricetta_Operazione = xRicetta_Operazione.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    objRicetta_Operazioni = New AgronicaCoreContabDAL.Ricette_Operazioni_W
                    ObjRicetteLog = New AgronicaCoreContabDAL.AgronicaLogRicette_W

                    'Inizializzo Preventivamente il Ricetta_Operazione_Cod
                    Ricetta_Operazione_Cod = CLng(xRicetta_Operazione.GetAttribute("ricetta_operazione_cod"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Ricetta_Operazione

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------

                            If Ricetta_Operazione_Cod <= 0 Then

                                'Richiedo un nuovo codice operazione

                                ObjSequenze = New Agro_Sequenze

                                Ricetta_Operazione_Cod = ObjSequenze.NuovoId_Tabella(
                                                            "RICETTE_OPERAZIONI",
                                                            CInt(0),
                                                            CInt(2000000000),
                                                            objParametri)

                                OUTPUT_Ricetta_Operazione_Cod = Ricetta_Operazione_Cod

                                ObjSequenze = Nothing

                            Else

                                'Esportazione in Locale

                            End If


                            Dim xRicetta_Operazione_data_creazione As Date = #2/1/1900#
                            Dim xRicetta_Operazione_data_Modifica As Date = #2/1/1900#
                            Dim xRicetta_Operazione_username_creazione As String = ""
                            Dim xRicetta_Operazione_username_modifica As String = ""

                            If Not IsNothing(xRicetta_Operazione.GetAttribute("data_creazione")) AndAlso
                             xRicetta_Operazione.GetAttribute("data_creazione") <> "" Then
                                xRicetta_Operazione_data_creazione = CDate(xRicetta_Operazione.GetAttribute("data_creazione"))
                                Data_Creazione = xRicetta_Operazione_data_creazione
                            End If

                            If Not IsNothing(xRicetta_Operazione.GetAttribute("data_modifica")) AndAlso
                             xRicetta_Operazione.GetAttribute("data_modifica") <> "" Then
                                xRicetta_Operazione_data_Modifica = CDate(xRicetta_Operazione.GetAttribute("data_modifica"))
                            End If

                            If Not IsNothing(xRicetta_Operazione.GetAttribute("username_creazione")) Then
                                xRicetta_Operazione_username_creazione = CStr(xRicetta_Operazione.GetAttribute("username_creazione"))
                                Username_Creazione = xRicetta_Operazione_username_creazione
                            End If

                            If Not IsNothing(xRicetta_Operazione.GetAttribute("username_modifica")) Then
                                xRicetta_Operazione_username_modifica = CStr(xRicetta_Operazione.GetAttribute("username_modifica"))
                            End If


                            ' VAnni: 2/8/2018: soluzione PROVVISORIA... perché in modifica funziona solo con le nuove ricette .. in modifica non va (occorre impostare l'attributo w_anagrafica_stati_cod)! 
                            ' 08/11/2018 fede aggiunto controllo isnumeric x copia ricette pua
                            Dim W_Anagrafica_Stati_Cod As Integer?
                            If xRicetta_Operazione.HasAttribute("w_anagrafica_stati_cod") AndAlso
                                Not IsNothing(xRicetta_Operazione.GetAttribute("w_anagrafica_stati_cod")) AndAlso
                                IsNumeric(xRicetta_Operazione.GetAttribute("w_anagrafica_stati_cod")) Then
                                W_Anagrafica_Stati_Cod = xRicetta_Operazione.GetAttribute("w_anagrafica_stati_cod")
                            Else
                                If Ricetta_Tipo = enum_TipoRicetta.Standard_Destinazioni Then
                                    W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire
                                End If

                            End If



                            Dummy = objRicetta_Operazioni.Scrivi(
                                     CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                     CInt(xRicetta_Operazione.GetAttribute("lav_cod")),
                                     CStr(xRicetta_Operazione.GetAttribute("ricetta_operazione_des")),
                                     CStr(xRicetta_Operazione.GetAttribute("note")),
                                     CDbl(xRicetta_Operazione.GetAttribute("num_protocollo")),
                                     If(Not xRicetta_Operazione.HasAttribute("id_rcdpi"), 0, xRicetta_Operazione.GetAttribute("id_rcdpi")),
                                     If(Not xRicetta_Operazione.HasAttribute("extra_int"), 0, xRicetta_Operazione.GetAttribute("extra_int")),
                                     If(Not xRicetta_Operazione.HasAttribute("mezzo"), 0, xRicetta_Operazione.GetAttribute("mezzo")),
                                     If(Not xRicetta_Operazione.HasAttribute("gru_op"), 0, If(xRicetta_Operazione.GetAttribute("gru_op") = String.Empty, "0", xRicetta_Operazione.GetAttribute("gru_op"))),
                                     If(Not xRicetta_Operazione.HasAttribute("costo"), 0, If(xRicetta_Operazione.GetAttribute("costo") = String.Empty, "0", xRicetta_Operazione.GetAttribute("costo"))),
                                     If(Not xRicetta_Operazione.HasAttribute("noleggio_passivo"), 0, If(xRicetta_Operazione.GetAttribute("noleggio_passivo") = String.Empty, "0", xRicetta_Operazione.GetAttribute("noleggio_passivo"))),
                                     Agro_XML_GetInteger(xRicetta_Operazione, "id_tp_fer", 0),
                                     Agro_XML_GetInteger(xRicetta_Operazione, "em_cod", 0),
                                     Agro_XML_GetDecimal(xRicetta_Operazione, "eff_perc", 0),
                                     Agro_XML_GetInteger(xRicetta_Operazione, "disciplinare_pubblicoprivato", 0),
                                     CDate(xRicetta_Operazione.GetAttribute("validita_inizio")),
                                     CDate(xRicetta_Operazione.GetAttribute("validita_fine")),
                                     objParametri,
                                     xRicetta_Operazione_data_creazione,
                                     xRicetta_Operazione_data_Modifica,
                                     xRicetta_Operazione_username_creazione,
                                     xRicetta_Operazione_username_modifica,
                                     W_Anagrafica_Stati_Cod:=W_Anagrafica_Stati_Cod,
                                     Ricetta_Operazione_Cod_RIF:=If(xRicetta_Operazione.HasAttribute("ricetta_operazione_cod_rif") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("ricetta_operazione_cod_rif")), xRicetta_Operazione.GetAttribute("ricetta_operazione_cod_rif"), Nothing),
                                     APP_Ricetta_Operazione_ID:=If(xRicetta_Operazione.HasAttribute("app_ricetta_operazione_id"), xRicetta_Operazione.GetAttribute("app_ricetta_operazione_id"), Nothing),
                                     Invia_App:=If(xRicetta_Operazione.HasAttribute("invia_app") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("invia_app")), CInt(xRicetta_Operazione.GetAttribute("invia_app")), 0),
                                     Raccoglitore_Cod:=If(xRicetta_Operazione.HasAttribute("raccoglitore_cod") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("raccoglitore_cod")), xRicetta_Operazione.GetAttribute("raccoglitore_cod"), 0),
                                     Invia_HubIoT:=If(xRicetta_Operazione.HasAttribute("invia_hubiot") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("invia_hubiot")), xRicetta_Operazione.GetAttribute("invia_hubiot"), 0),
                                     Ora:=If(xRicetta_Operazione.HasAttribute("ora") AndAlso IsDate(xRicetta_Operazione.GetAttribute("ora")), CDate(xRicetta_Operazione.GetAttribute("ora")), CDate(xRicetta_Operazione.GetAttribute("validita_inizio"))))


                            Dummy = ObjRicetteLog.Scrivi(OpeDB_Ricetta_Operazione,
                                                         "Ricette_Operazioni",
                                                         CInt(Ricetta_Cod),
                                                         CInt(Ricetta_Operazione_Cod),
                                                         Piva,
                                                         Sa_Cod,
                                                         CInt(xRicetta_Operazione.GetAttribute("lav_cod")),
                                                         CDate(xRicetta_Operazione.GetAttribute("validita_inizio")),
                                                         CStr(xRicetta_Operazione.GetAttribute("ricetta_operazione_des")),
                                                         CInt(idServizio),
                                                         objParametri,
                                                         DatiRicetta_Operazione,
                                                         If(xRicetta_Operazione.HasAttribute("raccoglitore_cod") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("raccoglitore_cod")), xRicetta_Operazione.GetAttribute("raccoglitore_cod"), 0),
                                                         If(xRicetta_Operazione.HasAttribute("app_ricetta_operazione_id"), xRicetta_Operazione.GetAttribute("app_ricetta_operazione_id"), "")
                                                         )

                        Case "2"    'MODIFICA -------------------------------------------------------

                            'NON CONTEMPLATO

                            objRicetta_Operazioni.Modifica(
                                     CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                     CInt(xRicetta_Operazione.GetAttribute("lav_cod")),
                                     CStr(xRicetta_Operazione.GetAttribute("ricetta_operazione_des")),
                                     CStr(xRicetta_Operazione.GetAttribute("note")),
                                     CDbl(xRicetta_Operazione.GetAttribute("num_protocollo")),
                                     If(Not xRicetta_Operazione.HasAttribute("id_rcdpi"), 0, xRicetta_Operazione.GetAttribute("id_rcdpi")),
                                     If(Not xRicetta_Operazione.HasAttribute("extra_int"), 0, xRicetta_Operazione.GetAttribute("extra_int")),
                                     If(Not xRicetta_Operazione.HasAttribute("mezzo"), 0, xRicetta_Operazione.GetAttribute("mezzo")),
                                     If(Not xRicetta_Operazione.HasAttribute("gru_op"), 0, xRicetta_Operazione.GetAttribute("gru_op")),
                                     If(Not xRicetta_Operazione.HasAttribute("costo"), 0, CDbl(xRicetta_Operazione.GetAttribute("costo"))),
                                     If(Not xRicetta_Operazione.HasAttribute("noleggio_passivo"), 0, xRicetta_Operazione.GetAttribute("noleggio_passivo")),
                                     Agro_XML_GetInteger(xRicetta_Operazione, "id_tp_fer", 0),
                                     Agro_XML_GetInteger(xRicetta_Operazione, "em_cod", 0),
                                     Agro_XML_GetDecimal(xRicetta_Operazione, "eff_perc", 0),
                                     Agro_XML_GetInteger(xRicetta_Operazione, "disciplinare_pubblicoprivato", 0),
                                     CDate(xRicetta_Operazione.GetAttribute("validita_inizio")),
                                     CDate(xRicetta_Operazione.GetAttribute("validita_fine")),
                                     String.Empty,
                                     objParametri,
                                     Ora:=If(xRicetta_Operazione.HasAttribute("ora") AndAlso IsDate(xRicetta_Operazione.GetAttribute("ora")),
                                        CDate(xRicetta_Operazione.GetAttribute("ora")), CDate(xRicetta_Operazione.GetAttribute("validita_inizio"))))

                            Dummy = ObjRicetteLog.Scrivi(OpeDB_Ricetta_Operazione,
                                                         "Ricette_Operazioni",
                                                         CInt(Ricetta_Cod),
                                                         CInt(Ricetta_Operazione_Cod),
                                                         Piva,
                                                         Sa_Cod,
                                                         CInt(xRicetta_Operazione.GetAttribute("lav_cod")),
                                                         CDate(xRicetta_Operazione.GetAttribute("validita_inizio")),
                                                         CStr(xRicetta_Operazione.GetAttribute("ricetta_operazione_des")),
                                                         CInt(idServizio),
                                                         objParametri,
                                                         DatiRicetta_Operazione,
                                                         If(xRicetta_Operazione.HasAttribute("raccoglitore_cod") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("raccoglitore_cod")), xRicetta_Operazione.GetAttribute("raccoglitore_cod"), 0),
                                                         If(xRicetta_Operazione.HasAttribute("app_ricetta_operazione_id"), xRicetta_Operazione.GetAttribute("app_ricetta_operazione_id"), "")
                                                         )

                    End Select


                    '##################################################
                    '##########  RICETTA DETTAGLI TECNICI  ############
                    '##################################################

                    'Prelevo l'elenco dei dettagli tecnici della ricetta
                    xRicetta_Dettagli_Tecnici = xRicetta_Operazione.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")

                    i_Ricetta_Dettaglio_Tecnico = 0

                    Do While i_Ricetta_Dettaglio_Tecnico < xRicetta_Dettagli_Tecnici.Count

                        'Prelevo l'i-esimo dettaglio tecnico
                        xRicetta_Dettaglio_Tecnico = xRicetta_Dettagli_Tecnici.Item(i_Ricetta_Dettaglio_Tecnico)

                        'Prelevo gli attributi del dettaglio tecnico selezionato
                        OpeDB_Ricetta_Dettaglio_Tecnico = xRicetta_Dettaglio_Tecnico.GetAttribute("TipoOperazioneDB")

                        'Inizializzo Preventivamente il Codice Dettaglio Tecnico
                        Ricetta_Tecnico_Cod = CLng(xRicetta_Dettaglio_Tecnico.GetAttribute("ricetta_tecnico_cod"))

                        'Creo l'oggetto COM
                        objRicetta_Dettaglio_Tecnico = New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W
                        'CreateObject("Agro_Contab_AD.Ricette_Dett_Tecnico_W")

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Ricetta_Dettaglio_Tecnico

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------


                                If Ricetta_Tecnico_Cod <= 0 Then

                                    'Richiedo un nuovo codice dettaglio tecnico

                                    ObjSequenze = New Agro_Sequenze

                                    Ricetta_Tecnico_Cod = ObjSequenze.NuovoId_Tabella(
                                                                "RICETTE_DETTAGLIO_TECNICO",
                                                            CInt(0),
                                                            CInt(2000000000),
                                                            objParametri)
                                    ObjSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If

                                'Salvo il dettaglio tecnico
                                Qta_Acqua = xRicetta_Dettaglio_Tecnico.GetAttribute("qta_ril")

                                If InStr(Qta_Acqua, ".") <> 0 Then
                                    Qta_Acqua = Split(Qta_Acqua, ".")(0) & "," & Split(Qta_Acqua, ".")(1)
                                End If



                                Dim xRicetta_Dettaglio_Tecnico_data_creazione As Date = #2/1/1900#
                                Dim xRicetta_Dettaglio_Tecnico_data_Modifica As Date = #2/1/1900#
                                Dim xRicetta_Dettaglio_Tecnico_username_creazione As String = ""
                                Dim xRicetta_Dettaglio_Tecnico_username_modifica As String = ""

                                If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("data_creazione")) AndAlso
                                 xRicetta_Dettaglio_Tecnico.GetAttribute("data_creazione") <> "" Then
                                    xRicetta_Dettaglio_Tecnico_data_creazione = CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("data_creazione"))
                                ElseIf Data_Creazione <> AGRODATAINIZIO Then
                                    xRicetta_Dettaglio_Tecnico_data_creazione = Data_Creazione
                                End If

                                If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("data_modifica")) AndAlso
                                 xRicetta_Dettaglio_Tecnico.GetAttribute("data_modifica") <> "" Then
                                    xRicetta_Dettaglio_Tecnico_data_Modifica = CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("username_creazione")) AndAlso
                                 xRicetta_Dettaglio_Tecnico.GetAttribute("username_creazione") <> "" Then
                                    xRicetta_Dettaglio_Tecnico_username_creazione = CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("username_creazione"))
                                ElseIf Username_Creazione <> "" Then
                                    xRicetta_Dettaglio_Tecnico_username_creazione = Username_Creazione
                                End If

                                If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("username_modifica")) Then
                                    xRicetta_Dettaglio_Tecnico_username_modifica = CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("username_modifica"))
                                End If



                                Dummy = objRicetta_Dettaglio_Tecnico.Scrivi(
                                                    CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                                    CInt(0), CInt(0),
                                                    CInt(Ricetta_Tecnico_Cod),
                                                    CDbl(Qta_Acqua),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_cod")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_gru")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("dett_cod")),
                                                    CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("dose")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("parziale")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("nitrati")),
                                                    CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("freatimetro")),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn1_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn2_data")),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn3_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn4_data")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ditta_cod")),
                                                    CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("sigla_av")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("trap_num")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("id_insetto")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ff_classe")),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "mg", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "n", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "p", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "k", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "apportoxha", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nnettoxha", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nutilexha", 0),
                                                    Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_cod", 0),
                                                    Agro_XML_GetString(xRicetta_Dettaglio_Tecnico, "soglia_des", ""),
                                                    Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_quantita", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "efficienza", 0),
                                                    Agro_XML_GetString(xRicetta_Dettaglio_Tecnico, "ricette_dettaglio_tecnico_graphickey", ""),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo1", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo2", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo3", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo4", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "cu", 0),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_inizio")),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_fine")),
                                                    objParametri,
                                                    xRicetta_Dettaglio_Tecnico_data_creazione,
                                                    xRicetta_Dettaglio_Tecnico_data_Modifica,
                                                    xRicetta_Dettaglio_Tecnico_username_creazione,
                                                    xRicetta_Dettaglio_Tecnico_username_modifica
                                            )

                            Case "2"    'MODIFICA -------------------------------------------------------

                                Qta_Acqua = xRicetta_Dettaglio_Tecnico.GetAttribute("qta_ril")
                                If InStr(Qta_Acqua, ".") <> 0 Then
                                    Qta_Acqua = Split(Qta_Acqua, ".")(0) & "," & Split(Qta_Acqua, ".")(1)
                                End If


                                objRicetta_Dettaglio_Tecnico.Modifica(
                                                    CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                                    CInt(0), CInt(0),
                                                    CInt(Ricetta_Tecnico_Cod),
                                                    CDbl(Qta_Acqua),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_cod")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_gru")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("dett_cod")),
                                                    CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("dose")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("parziale")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("nitrati")),
                                                    CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("freatimetro")),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn1_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn2_data")),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn3_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn4_data")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ditta_cod")),
                                                    CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("sigla_av")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("trap_num")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("id_insetto")),
                                                    CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ff_classe")),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "mg", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "n", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "p", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "k", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "apportoxha", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nnettoxha", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nutilexha", 0),
                                                    Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_cod", 0),
                                                    Agro_XML_GetString(xRicetta_Dettaglio_Tecnico, "soglia_des", ""),
                                                    Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_quantita", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "efficienza", 0),
                                                    Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "cu", 0),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_inizio")),
                                                    CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_fine")),
                                                    String.Empty,
                                                    objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objRicetta_Dettaglio_Tecnico.Cancella(
                                     CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                     CLng(0), CLng(0),
                                     CLng(Ricetta_Tecnico_Cod),
                                     String.Empty,
                                     objParametri)


                        End Select

                        'Elimino l'oggetto
                        objRicetta_Dettaglio_Tecnico = Nothing
                        'Incremento l'indice
                        i_Ricetta_Dettaglio_Tecnico = i_Ricetta_Dettaglio_Tecnico + 1

                    Loop



                    '#############################################
                    '##########  RICETTE DETTAGLI  ###############
                    '#############################################

                    'Prelevo l'elenco dei dettagli tecnici della ricetta
                    xRicetta_Dettagli = xRicetta_Operazione.GetElementsByTagName("Ricetta_Dettaglio")

                    i_Ricetta_Dettaglio = 0

                    Do While i_Ricetta_Dettaglio < xRicetta_Dettagli.Count

                        'Prelevo l'i-esimo dettaglio tecnico
                        xRicetta_Dettaglio = xRicetta_Dettagli.Item(i_Ricetta_Dettaglio)

                        'Prelevo gli attributi del dettaglio tecnico selezionato
                        OpeDB_Ricetta_Dettaglio = xRicetta_Dettaglio.GetAttribute("TipoOperazioneDB")

                        'Inizializzo Preventivamente il Cod_Ricetta_Dettaglio
                        Ricetta_Dettaglio_Cod = CLng(xRicetta_Dettaglio.GetAttribute("ricetta_dettaglio_cod"))

                        'Creo l'oggetto COM
                        objRicetta_Dettagli = New AgronicaCoreContabDAL.Ricette_Dettagli_W

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Ricetta_Dettaglio

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                If Ricetta_Dettaglio_Cod <= 0 Then

                                    'Richiedo un nuovo codice dettaglio tecnico
                                    ObjSequenze = New Agro_Sequenze

                                    Ricetta_Dettaglio_Cod = ObjSequenze.NuovoId_Tabella(
                                                                "RICETTE_DETTAGLI",
                                                            CInt(0),
                                                            CInt(2000000000),
                                                            objParametri)
                                    ObjSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If

                                Dim j As Integer
                                Dim lvet As Integer
                                Dim MiscelaVett As Integer()
                                Dim MiscelaVettMappati As Integer()

                                'Miscela_cod negativo, verifico se l'ho già mappato altrimenti lo mappo
                                If CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod")) < 0 Then

                                    'Verifico che il miscela_cod non sia già stato mappato
                                    Miscela_Cod = -1
                                    For j = 1 To lvet

                                        If MiscelaVett(j) = CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod")) Then

                                            'Recupero il valore mappato
                                            Miscela_Cod = MiscelaVettMappati(j)
                                            Exit For

                                        End If


                                    Next j

                                    'se non era già stato mappato il miscela_cod me ne procuro uno nuovo
                                    If Miscela_Cod < 0 Then


                                        'Aumento la dimensione dei vettori
                                        lvet = lvet + 1

                                        ReDim Preserve MiscelaVett(lvet)
                                        ReDim Preserve MiscelaVettMappati(lvet)

                                        'Assegno l'indice al nuovo elemento del vettore
                                        MiscelaVett(lvet) = CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod"))

                                        objRicetta_Dettagli_R = New AgronicaCoreContabDAL.Ricette_Dettagli_R
                                        'CreateObject("Agro_Contab_AD.Ricette_Dettagli_R")

                                        Miscela_Cod = objRicetta_Dettagli_R.MaxValoreMiscelaCod(CInt(Ricetta_Cod),
                                                                                                CInt(Ricetta_Operazione_Cod),
                                                                                                objParametri)

                                        If Miscela_Cod <> 0 Then
                                            Miscela_Cod = Miscela_Cod + 1
                                        Else
                                            Miscela_Cod = 1
                                        End If

                                        MiscelaVettMappati(lvet) = Miscela_Cod

                                        ' distruggo l'oggetto
                                        objRicetta_Dettagli_R = Nothing

                                    End If


                                    'Miscela_Cod = 0 mi procuro un nuovo valore positivo
                                ElseIf CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod")) = 0 Then

                                    objRicetta_Dettagli_R = New AgronicaCoreContabDAL.Ricette_Dettagli_R
                                    Miscela_Cod = objRicetta_Dettagli_R.MaxValoreMiscelaCod(CInt(Ricetta_Cod),
                                                                                            CInt(Ricetta_Operazione_Cod),
                                                                                            objParametri)

                                    If Miscela_Cod <> 0 Then
                                        Miscela_Cod = Miscela_Cod + 1
                                    Else
                                        Miscela_Cod = 1
                                    End If

                                    ' distruggo l'oggetto
                                    objRicetta_Dettagli_R = Nothing

                                Else 'Ho già un miscela_cod positivo

                                    Miscela_Cod = CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod"))

                                End If

                                Dim pr_Unitario As Decimal = 0.0

                                If xRicetta_Dettaglio.HasAttribute("prezzo_unitario") AndAlso
                                   xRicetta_Dettaglio.GetAttribute("prezzo_unitario") <> "" AndAlso
                                   IsNumeric(xRicetta_Dettaglio.GetAttribute("prezzo_unitario")) Then

                                    pr_Unitario = CDbl(xRicetta_Dettaglio.GetAttribute("prezzo_unitario"))
                                End If


                                Dim xRicetta_Dettaglio_data_creazione As Date = #2/1/1900#
                                Dim xRicetta_Dettaglio_data_Modifica As Date = #2/1/1900#
                                Dim xRicetta_Dettaglio_username_creazione As String = ""
                                Dim xRicetta_Dettaglio_username_modifica As String = ""

                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("data_creazione")) AndAlso
                                 xRicetta_Dettaglio.GetAttribute("data_creazione") <> "" Then
                                    xRicetta_Dettaglio_data_creazione = CDate(xRicetta_Dettaglio.GetAttribute("data_creazione"))
                                ElseIf Data_Creazione <> AGRODATAINIZIO Then
                                    xRicetta_Dettaglio_data_creazione = Data_Creazione
                                End If

                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("data_modifica")) AndAlso
                                 xRicetta_Dettaglio.GetAttribute("data_modifica") <> "" Then
                                    xRicetta_Dettaglio_data_Modifica = CDate(xRicetta_Dettaglio.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("username_creazione")) AndAlso
                                 xRicetta_Dettaglio.GetAttribute("username_creazione") <> "" Then
                                    xRicetta_Dettaglio_username_creazione = CStr(xRicetta_Dettaglio.GetAttribute("ThenThenusername_creazione"))
                                ElseIf Username_Creazione <> "" Then
                                    xRicetta_Dettaglio_username_creazione = Username_Creazione
                                End If

                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("username_modifica")) Then
                                    xRicetta_Dettaglio_username_modifica = CStr(xRicetta_Dettaglio.GetAttribute("username_modifica"))
                                End If

                                Dim TempoCarenza As Integer = 0
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("tempocarenza")) Then
                                    If IsNumeric(xRicetta_Dettaglio.GetAttribute("tempocarenza")) Then
                                        TempoCarenza = CStr(xRicetta_Dettaglio.GetAttribute("tempocarenza"))
                                    End If
                                End If

                                Dim Extra_Str As String = ""
                                If xRicetta_Dettaglio.HasAttribute("extra_str") AndAlso Not IsNothing(xRicetta_Dettaglio.GetAttribute("extra_str")) Then
                                    Extra_Str = xRicetta_Dettaglio.GetAttribute("extra_str")
                                End If

                                Dim DoseEtichetta As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("doseetichetta")) Then
                                    DoseEtichetta = CStr(xRicetta_Dettaglio.GetAttribute("doseetichetta"))
                                End If
                                Dim DoseEtichetta_Value As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("doseetichetta_value")) Then
                                    DoseEtichetta_Value = CStr(xRicetta_Dettaglio.GetAttribute("doseetichetta_value"))
                                End If
                                Dim PrincipiAttivi As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("principiattivi")) Then
                                    PrincipiAttivi = CStr(xRicetta_Dettaglio.GetAttribute("principiattivi"))
                                End If
                                Dim PrincipiAttiviPercAbb As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("principiattivipercabb")) Then
                                    PrincipiAttiviPercAbb = CStr(xRicetta_Dettaglio.GetAttribute("principiattivipercabb"))
                                End If
                                Dim PrincipiAttiviPesi As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("principiattivipesi")) Then
                                    PrincipiAttiviPesi = CStr(xRicetta_Dettaglio.GetAttribute("principiattivipesi"))
                                End If
                                Dim ClassiTossicologiche As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("classitossicologiche")) Then
                                    ClassiTossicologiche = CStr(xRicetta_Dettaglio.GetAttribute("classitossicologiche"))
                                End If

                                Dim Buffer As String = ""
                                If xRicetta_Dettaglio.HasAttribute("buffer") AndAlso Not IsNothing(xRicetta_Dettaglio.GetAttribute("buffer")) Then
                                    Buffer = xRicetta_Dettaglio.GetAttribute("buffer")
                                End If

                                Dim Polverulento As Integer = 0
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("polverulento")) Then
                                    If IsNumeric(xRicetta_Dettaglio.GetAttribute("polverulento")) Then
                                        Polverulento = CStr(xRicetta_Dettaglio.GetAttribute("polverulento"))
                                    End If
                                End If

                                '  Marco Grilli, 08/04/2016 09:53:51: Costi accessori per le attività (introdotto per il LAN)
                                Dim Turno_Cod As Integer = 0
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("turno_cod")) AndAlso IsNumeric(xRicetta_Dettaglio.GetAttribute("turno_cod")) Then
                                    Turno_Cod = CInt(xRicetta_Dettaglio.GetAttribute("turno_cod"))
                                End If

                                Dim ID_Attivita As Integer = 0
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("id_attivita")) AndAlso IsNumeric(xRicetta_Dettaglio.GetAttribute("id_attivita")) Then
                                    ID_Attivita = CInt(xRicetta_Dettaglio.GetAttribute("id_attivita"))
                                End If

                                Dim Lotto As String = ""
                                If Not IsNothing(xRicetta_Dettaglio.GetAttribute("lotto")) Then
                                    Lotto = CStr(xRicetta_Dettaglio.GetAttribute("lotto"))
                                End If


                                Dim Qualifica_cod As Integer = 0
                                Dim Tariffa_cod As Integer = 0

                                If xRicetta_Dettaglio.HasAttribute("qualifica_cod") AndAlso Not String.IsNullOrEmpty(xRicetta_Dettaglio.GetAttribute("qualifica_cod")) Then
                                    Qualifica_cod = xRicetta_Dettaglio.GetAttribute("qualifica_cod")
                                End If


                                If xRicetta_Dettaglio.HasAttribute("tariffa_cod") AndAlso Not String.IsNullOrEmpty(xRicetta_Dettaglio.GetAttribute("tariffa_cod")) Then
                                    Tariffa_cod = xRicetta_Dettaglio.GetAttribute("tariffa_cod")
                                End If


                                Dim Qta_Extra As Decimal = 0
                                Dim Qta_Extra_Totale As Decimal = 0
                                Dim Udm_Cod_Extra As Integer = 0
                                Dim Mezzo_Det As Integer = 0

                                If xRicetta_Dettaglio.HasAttribute("qta_extra") AndAlso Not String.IsNullOrEmpty(xRicetta_Dettaglio.GetAttribute("qta_extra")) Then
                                    Qta_Extra = xRicetta_Dettaglio.GetAttribute("qta_extra")
                                End If
                                If xRicetta_Dettaglio.HasAttribute("qta_extra_totale") AndAlso Not String.IsNullOrEmpty(xRicetta_Dettaglio.GetAttribute("qta_extra_totale")) Then
                                    Qta_Extra_Totale = xRicetta_Dettaglio.GetAttribute("qta_extra_totale")
                                End If
                                If xRicetta_Dettaglio.HasAttribute("udm_cod_extra") AndAlso Not String.IsNullOrEmpty(xRicetta_Dettaglio.GetAttribute("udm_cod_extra")) Then
                                    Udm_Cod_Extra = xRicetta_Dettaglio.GetAttribute("udm_cod_extra")
                                End If
                                If xRicetta_Dettaglio.HasAttribute("mezzo_det") AndAlso Not String.IsNullOrEmpty(xRicetta_Dettaglio.GetAttribute("mezzo_det")) Then
                                    Mezzo_Det = xRicetta_Dettaglio.GetAttribute("mezzo_det")
                                End If

                                Dummy = objRicetta_Dettagli.Scrivi(
                                         CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                         CInt(Ricetta_Dettaglio_Cod), Miscela_Cod,
                                         CInt(xRicetta_Dettaglio.GetAttribute("elem_cod")),
                                         CInt(xRicetta_Dettaglio.GetAttribute("pro_cod")),
                                         CInt(xRicetta_Dettaglio.GetAttribute("mat_cod")),
                                         CInt(xRicetta_Dettaglio.GetAttribute("udm_cod")),
                                         CDbl(xRicetta_Dettaglio.GetAttribute("qta")),
                                         If(Not xRicetta_Dettaglio.HasAttribute("extra_int"), 0, xRicetta_Dettaglio.GetAttribute("extra_int")),
                                         pr_Unitario,
                                         If(Not xRicetta_Dettaglio.HasAttribute("cau_mov"), 0, xRicetta_Dettaglio.GetAttribute("cau_mov")),
                                         Qualifica_cod,
                                         Tariffa_cod,
                                         CDate(xRicetta_Dettaglio.GetAttribute("validita_inizio")),
                                         CDate(xRicetta_Dettaglio.GetAttribute("validita_fine")),
                                         objParametri,
                                         xRicetta_Dettaglio_data_creazione,
                                         xRicetta_Dettaglio_data_Modifica,
                                         xRicetta_Dettaglio_username_creazione,
                                         xRicetta_Dettaglio_username_modifica,
                                         TempoCarenza, DoseEtichetta, PrincipiAttivi, ClassiTossicologiche,
                                         DoseEtichetta_Value, Turno_Cod, ID_Attivita, Lotto,
                                         Qta_Extra, Qta_Extra_Totale, Udm_Cod_Extra, Mezzo_Det, PrincipiAttiviPesi, Buffer,
                                         Extra_Str, PrincipiAttiviPercAbb, Polverulento)

                            Case "2"    'MODIFICA -------------------------------------------------------

                        End Select


                        '#############################################
                        '##########  RICETTE DESTINAZIONI  ###########
                        '#############################################

                        'Prelevo l'elenco delle destinazioni del dettaglio
                        xRicetta_Destinazioni = xRicetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                        i_Ricetta_Destinazione = 0

                        Do While i_Ricetta_Destinazione < xRicetta_Destinazioni.Count

                            'Prelevo l'i-esima destinazione
                            xRicetta_Destinazione = xRicetta_Destinazioni.Item(i_Ricetta_Destinazione)

                            'Prelevo gli attributi del dettaglio tecnico selezionato
                            OpeDB_Ricetta_Destinazione = xRicetta_Destinazione.GetAttribute("TipoOperazioneDB")

                            'Inizializzo Preventivamente il Cod_Ricetta_Dettaglio
                            Ricetta_Destinazione_Cod = CInt(xRicetta_Destinazione.GetAttribute("ricetta_destinazione_cod"))

                            'Creo l'oggetto COM
                            objRicetta_Destinazione = New AgronicaCoreContabDAL.Ricette_Destinazioni_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Ricetta_Destinazione

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    If Ricetta_Destinazione_Cod <= 0 Then

                                        'Richiedo un nuovo codice dettaglio tecnico
                                        ObjSequenze = New Agro_Sequenze

                                        Ricetta_Destinazione_Cod = ObjSequenze.NuovoId_Tabella(
                                                                    "RICETTE_DESTINAZIONI",
                                                            CInt(0),
                                                            CInt(2000000000),
                                                            objParametri)
                                        ObjSequenze = Nothing

                                    Else

                                        'Esportazione in Locale

                                    End If


                                    Dim xRicetta_Destinazione_data_creazione As Date = #2/1/1900#
                                    Dim xRicetta_Destinazione_data_Modifica As Date = #2/1/1900#
                                    Dim xRicetta_Destinazione_username_creazione As String = ""
                                    Dim xRicetta_Destinazione_username_modifica As String = ""

                                    If Not IsNothing(xRicetta_Destinazione.GetAttribute("data_creazione")) AndAlso
                                     xRicetta_Destinazione.GetAttribute("data_creazione") <> "" Then
                                        xRicetta_Destinazione_data_creazione = CDate(xRicetta_Destinazione.GetAttribute("data_creazione"))
                                    ElseIf data_creazione <> AGRODATAINIZIO Then
                                        xRicetta_Destinazione_data_creazione = Data_Creazione
                                    End If

                                    If Not IsNothing(xRicetta_Destinazione.GetAttribute("data_modifica")) AndAlso
                                     xRicetta_Destinazione.GetAttribute("data_modifica") <> "" Then
                                        xRicetta_Destinazione_data_Modifica = CDate(xRicetta_Destinazione.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicetta_Destinazione.GetAttribute("username_creazione")) AndAlso
                                     xRicetta_Destinazione.GetAttribute("username_creazione") <> "" Then
                                        xRicetta_Destinazione_username_creazione = CStr(xRicetta_Destinazione.GetAttribute("username_creazione"))
                                    ElseIf Username_Creazione <> "" Then
                                        xRicetta_Destinazione_username_creazione = Username_Creazione
                                    End If

                                    If Not IsNothing(xRicetta_Destinazione.GetAttribute("username_modifica")) Then
                                        xRicetta_Destinazione_username_modifica = CStr(xRicetta_Destinazione.GetAttribute("username_modifica"))
                                    End If

                                    Dim quotadistribuzione As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("quotadistribuzione") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("quotadistribuzione")) Then
                                        quotadistribuzione = xRicetta_Destinazione.GetAttribute("quotadistribuzione")
                                    End If

                                    Dim qta2 As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("qta2")) Then
                                        qta2 = xRicetta_Destinazione.GetAttribute("qta2")
                                    End If


                                    Dim tipo_destinazione As Integer = 0
                                    If xRicetta_Destinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("tipo_destinazione")) Then
                                        tipo_destinazione = xRicetta_Destinazione.GetAttribute("tipo_destinazione")
                                    End If

                                    Dim MagazzinoEsterno_Cod As String = ""
                                    If xRicetta_Destinazione.HasAttribute("magazzinoesterno_cod") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("magazzinoesterno_cod")) Then
                                        MagazzinoEsterno_Cod = xRicetta_Destinazione.GetAttribute("magazzinoesterno_cod")
                                    End If

                                    Dim MagazzinoEsterno_Des As String = ""
                                    If xRicetta_Destinazione.HasAttribute("magazzinoesterno_des") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("magazzinoesterno_des")) Then
                                        MagazzinoEsterno_Des = xRicetta_Destinazione.GetAttribute("magazzinoesterno_des")
                                    End If

                                    Dim MagazzinoEsterno_Dettagli As String = ""
                                    If xRicetta_Destinazione.HasAttribute("magazzinoesterno_dettagli") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("magazzinoesterno_dettagli")) Then
                                        MagazzinoEsterno_Dettagli = xRicetta_Destinazione.GetAttribute("magazzinoesterno_dettagli")
                                    End If

                                    Dim Sup_Riduzione_BufferZone As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("sup_riduzione_bufferzone") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("sup_riduzione_bufferzone")) Then
                                        Sup_Riduzione_BufferZone = xRicetta_Destinazione.GetAttribute("sup_riduzione_bufferzone")
                                    End If

                                    Dim Perc_Riduzione_Deriva As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("perc_riduzione_deriva") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("perc_riduzione_deriva")) Then
                                        Perc_Riduzione_Deriva = xRicetta_Destinazione.GetAttribute("perc_riduzione_deriva")
                                    End If

                                    Dummy = objRicetta_Destinazione.Scrivi(
                                             CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                             CInt(Ricetta_Dettaglio_Cod), CInt(Ricetta_Destinazione_Cod),
                                             CInt(xRicetta_Destinazione.GetAttribute("programmazione_entita_cod")),
                                             CStr(xRicetta_Destinazione.GetAttribute("piva")),
                                             CInt(xRicetta_Destinazione.GetAttribute("sa_cod")),
                                             CInt(xRicetta_Destinazione.GetAttribute("appezza")),
                                             CInt(xRicetta_Destinazione.GetAttribute("id_reg")),
                                             CDbl(xRicetta_Destinazione.GetAttribute("qta")),
                                             qta2,
                                             quotadistribuzione,
                                             tipo_destinazione,
                                             CDate(xRicetta_Destinazione.GetAttribute("validita_inizio")),
                                             CDate(xRicetta_Destinazione.GetAttribute("validita_fine")),
                                             objParametri,
                                             xRicetta_Destinazione_data_creazione,
                                             xRicetta_Destinazione_data_Modifica,
                                             xRicetta_Destinazione_username_creazione,
                                             xRicetta_Destinazione_username_modifica,
                                             magazzinoEsterno_Cod:=MagazzinoEsterno_Cod,
                                             magazzinoEsterno_Des:=MagazzinoEsterno_Des,
                                             magazzinoEsterno_Dettagli:=MagazzinoEsterno_Dettagli,
                                             Sup_Riduzione_BufferZone:=Sup_Riduzione_BufferZone,
                                             Perc_Riduzione_Deriva:=Perc_Riduzione_Deriva
                                        )

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    Dim quotadistribuzione As Decimal = 0D
                                    If Not IsNothing(xRicetta_Destinazione.GetAttribute("quotadistribuzione")) Then
                                        quotadistribuzione = xRicetta_Destinazione.GetAttribute("quotadistribuzione")
                                    End If

                                    Dim qta2 As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("qta2")) Then
                                        qta2 = xRicetta_Destinazione.GetAttribute("qta2")
                                    End If

                                    Dim tipo_destinazione As Integer = 0
                                    If xRicetta_Destinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("tipo_destinazione")) Then
                                        tipo_destinazione = xRicetta_Destinazione.GetAttribute("tipo_destinazione")
                                    End If

                                    Dim MagazzinoEsterno_Cod As String = ""
                                    If xRicetta_Destinazione.HasAttribute("magazzinoesterno_cod") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("magazzinoesterno_cod")) Then
                                        MagazzinoEsterno_Cod = xRicetta_Destinazione.GetAttribute("magazzinoesterno_cod")
                                    End If

                                    Dim MagazzinoEsterno_Des As String = ""
                                    If xRicetta_Destinazione.HasAttribute("magazzinoesterno_des") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("magazzinoesterno_des")) Then
                                        MagazzinoEsterno_Des = xRicetta_Destinazione.GetAttribute("magazzinoesterno_des")
                                    End If

                                    Dim MagazzinoEsterno_Dettagli As String = ""
                                    If xRicetta_Destinazione.HasAttribute("magazzinoesterno_dettagli") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("magazzinoesterno_dettagli")) Then
                                        MagazzinoEsterno_Dettagli = xRicetta_Destinazione.GetAttribute("magazzinoesterno_dettagli")
                                    End If

                                    Dim Sup_Riduzione_BufferZone As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("sup_riduzione_bufferzone") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("sup_riduzione_bufferzone")) Then
                                        Sup_Riduzione_BufferZone = xRicetta_Destinazione.GetAttribute("sup_riduzione_bufferzone")
                                    End If

                                    Dim Perc_Riduzione_Deriva As Decimal = 0D
                                    If xRicetta_Destinazione.HasAttribute("perc_riduzione_deriva") AndAlso Not String.IsNullOrEmpty(xRicetta_Destinazione.GetAttribute("perc_riduzione_deriva")) Then
                                        Perc_Riduzione_Deriva = xRicetta_Destinazione.GetAttribute("perc_riduzione_deriva")
                                    End If


                                    Dummy = objRicetta_Destinazione.Modifica(
                                            CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                            CInt(Ricetta_Dettaglio_Cod), CInt(Ricetta_Destinazione_Cod),
                                            CInt(xRicetta_Destinazione.GetAttribute("programmazione_entita_cod")),
                                            CStr(xRicetta_Destinazione.GetAttribute("piva")),
                                            CInt(xRicetta_Destinazione.GetAttribute("sa_cod")),
                                            CInt(xRicetta_Destinazione.GetAttribute("appezza")),
                                            CInt(xRicetta_Destinazione.GetAttribute("id_reg")),
                                            CDbl(xRicetta_Destinazione.GetAttribute("qta")),
                                            qta2,
                                            quotadistribuzione,
                                            tipo_destinazione,
                                            CDate(xRicetta_Destinazione.GetAttribute("validita_inizio")),
                                            CDate(xRicetta_Destinazione.GetAttribute("validita_fine")),
                                            String.Empty,
                                            objParametri,
                                            magazzinoEsterno_Cod:=MagazzinoEsterno_Cod,
                                            magazzinoEsterno_Des:=MagazzinoEsterno_Des,
                                            magazzinoEsterno_Dettagli:=MagazzinoEsterno_Dettagli,
                                            Sup_Riduzione_BufferZone:=Sup_Riduzione_BufferZone,
                                            Perc_Riduzione_Deriva:=Perc_Riduzione_Deriva)

                                Case "3"    'CANCELLA -------------------------------------------------------

                                    objRicetta_Destinazione.Cancella(
                                                            CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                                            CInt(Ricetta_Dettaglio_Cod), CInt(Ricetta_Destinazione_Cod),
                                                            String.Empty,
                                                            objParametri)
                            End Select

                            'Elimino l'oggetto
                            objRicetta_Destinazione = Nothing
                            'Incremento l'indice
                            i_Ricetta_Destinazione += 1

                        Loop

                        '##################################################
                        '##########  RIICETTA DETTAGLI TECNICI 2 ##########
                        '##################################################

                        'Prelevo l'elenco dei dettagli tecnici della ricetta
                        xRicetta_Dettagli_Tecnici = xRicetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                        i_Ricetta_Dettaglio_Tecnico = 0

                        Do While i_Ricetta_Dettaglio_Tecnico < xRicetta_Dettagli_Tecnici.Count

                            'Prelevo l'i-esimo dettaglio tecnico
                            xRicetta_Dettaglio_Tecnico = xRicetta_Dettagli_Tecnici.Item(i_Ricetta_Dettaglio_Tecnico)

                            'Prelevo gli attributi del dettaglio tecnico selezionato
                            OpeDB_Ricetta_Dettaglio_Tecnico = xRicetta_Dettaglio_Tecnico.GetAttribute("TipoOperazioneDB")

                            'Inizializzo Preventivamente il Codice Dettaglio Tecnico
                            Ricetta_Dettaglio_Tecnico_Cod = CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ricetta_tecnico_cod"))

                            'Creo l'oggetto COM
                            objRicetta_Dettaglio_Tecnico = New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Ricetta_Dettaglio_Tecnico

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    If Ricetta_Dettaglio_Tecnico_Cod <= 0 Then

                                        'Richiedo un nuovo codice dettaglio tecnico

                                        ObjSequenze = New Agro_Sequenze
                                        'CreateObject("Agro_Contab_AD.Agro_Sequenze")

                                        Ricetta_Dettaglio_Tecnico_Cod = ObjSequenze.NuovoId_Tabella(
                                                                                "RICETTE_DETTAGLIO_TECNICO",
                                                            CInt(0),
                                                            CInt(2000000000),
                                                            objParametri)

                                        ObjSequenze = Nothing

                                    Else

                                        'Esportazione in Locale

                                    End If

                                    'Salvo il dettaglio tecnico


                                    Dim xRicetta_Dettaglio_Tecnico_data_creazione As Date = #2/1/1900#
                                    Dim xRicetta_Dettaglio_Tecnico_data_Modifica As Date = #2/1/1900#
                                    Dim xRicetta_Dettaglio_Tecnico_username_creazione As String = ""
                                    Dim xRicetta_Dettaglio_Tecnico_username_modifica As String = ""

                                    If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("data_creazione")) AndAlso
                                     xRicetta_Dettaglio_Tecnico.GetAttribute("data_creazione") <> "" Then
                                        xRicetta_Dettaglio_Tecnico_data_creazione = CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("data_creazione"))
                                    ElseIf data_creazione <> AGRODATAINIZIO Then
                                        xRicetta_Dettaglio_Tecnico_data_creazione = Data_Creazione
                                    End If

                                    If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("data_modifica")) AndAlso
                                     xRicetta_Dettaglio_Tecnico.GetAttribute("data_modifica") <> "" Then
                                        xRicetta_Dettaglio_Tecnico_data_Modifica = CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("username_creazione")) AndAlso
                                     xRicetta_Dettaglio_Tecnico.GetAttribute("username_creazione") <> "" Then
                                        xRicetta_Dettaglio_Tecnico_username_creazione = CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("username_creazione"))
                                    ElseIf Username_Creazione <> "" Then
                                        xRicetta_Dettaglio_Tecnico_username_creazione = Username_Creazione
                                    End If

                                    If Not IsNothing(xRicetta_Dettaglio_Tecnico.GetAttribute("username_modifica")) Then
                                        xRicetta_Dettaglio_Tecnico_username_modifica = CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("username_modifica"))
                                    End If



                                    Dummy = objRicetta_Dettaglio_Tecnico.Scrivi(
                                             CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                             CInt(Ricetta_Dettaglio_Cod),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("miscela_cod")),
                                             CInt(Ricetta_Dettaglio_Tecnico_Cod),
                                             CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("qta_ril")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_cod")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_gru")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("dett_cod")),
                                             CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("dose")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("parziale")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("nitrati")),
                                             CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("freatimetro")),
                                             CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn1_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn2_data")),
                                             CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn3_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn4_data")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ditta_cod")),
                                             CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("sigla_av")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("trap_num")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("id_insetto")),
                                             CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ff_classe")),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "mg", 0),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "n", 0),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "p", 0),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "k", 0),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "apportoxha", 0),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nnettoxha", 0),
                                             Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nutilexha", 0),
                                            Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_cod", 0),
                                            Agro_XML_GetString(xRicetta_Dettaglio_Tecnico, "soglia_des", ""),
                                            Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_quantita", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "efficienza", 0),
                                            Agro_XML_GetString(xRicetta_Dettaglio_Tecnico, "ricette_dettaglio_tecnico_graphickey", ""),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo1", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo2", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo3", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "piezo4", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "cu", 0),
                                             CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_inizio")),
                                             CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_fine")),
                                             objParametri,
                                             xRicetta_Dettaglio_Tecnico_data_creazione,
                                             xRicetta_Dettaglio_Tecnico_data_Modifica,
                                             xRicetta_Dettaglio_Tecnico_username_creazione,
                                             xRicetta_Dettaglio_Tecnico_username_modifica
                                        )

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objRicetta_Dettaglio_Tecnico.Modifica(
                                            CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                            CInt(Ricetta_Dettaglio_Cod), CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod")),
                                            CInt(Ricetta_Dettaglio_Tecnico_Cod),
                                            CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("qta_ril")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_cod")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("av_gru")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("dett_cod")),
                                            CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("dose")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("parziale")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("nitrati")),
                                            CDbl(xRicetta_Dettaglio_Tecnico.GetAttribute("freatimetro")),
                                            CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn1_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn2_data")),
                                            CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn3_data")), CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("inn4_data")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ditta_cod")),
                                            CStr(xRicetta_Dettaglio_Tecnico.GetAttribute("sigla_av")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("trap_num")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("id_insetto")),
                                            CInt(xRicetta_Dettaglio_Tecnico.GetAttribute("ff_classe")),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "mg", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "n", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "p", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "k", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "apportoxha", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nnettoxha", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "nutilexha", 0),
                                            Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_cod", 0),
                                            Agro_XML_GetString(xRicetta_Dettaglio_Tecnico, "soglia_des", ""),
                                            Agro_XML_GetInteger(xRicetta_Dettaglio_Tecnico, "soglia_quantita", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "efficienza", 0),
                                            Agro_XML_GetDecimal(xRicetta_Dettaglio_Tecnico, "cu", 0),
                                            CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_inizio")),
                                            CDate(xRicetta_Dettaglio_Tecnico.GetAttribute("validita_fine")),
                                            String.Empty,
                                            objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objRicetta_Dettaglio_Tecnico.Cancella(
                                         CInt(Ricetta_Cod), CInt(Ricetta_Operazione_Cod),
                                         CLng(Ricetta_Dettaglio_Cod), CInt(xRicetta_Dettaglio.GetAttribute("miscela_cod")),
                                         CLng(Ricetta_Dettaglio_Tecnico_Cod),
                                         String.Empty,
                                         objParametri)

                            End Select

                            'Elimino l'oggetto
                            objRicetta_Dettaglio_Tecnico = Nothing
                            'Incremento l'indice
                            i_Ricetta_Dettaglio_Tecnico = i_Ricetta_Dettaglio_Tecnico + 1

                        Loop


                        Select Case OpeDB_Ricetta_Dettaglio

                            Case 3 'CANCELLAZIONE DETTAGLIO

                                objRicetta_Dettagli.Cancella(
                                     CInt(Ricetta_Cod),
                                     CInt(Ricetta_Operazione_Cod),
                                     CInt(Ricetta_Dettaglio_Cod),
                                     CInt(Miscela_Cod),
                                     String.Empty,
                                     objParametri)

                        End Select



                        'Elimino l'oggetto
                        objRicetta_Dettagli = Nothing
                        'Incremento l'indice
                        i_Ricetta_Dettaglio += 1

                    Loop

                    objRicetta_Dettagli = Nothing












                    '#######################################################################
                    '#######################################################################
                    '#######################################################################
                    '#######################################################################


                    '##################################################
                    '##########  NOTE              ####################
                    '##################################################

                    Dim objRicettaxNote As New AgronicaCoreContabDAL.RicettexNote_W

                    'If OpeDB_Ricetta_Operazione = enum_TipoOperazioneDB.Modifica Then

                    '    Dim objRicettaxNoteR As New AgronicaCoreContabDAL.RicettexNote_R
                    '    Dim dtRxN = objRicettaxNoteR.Leggi(Ricetta_Cod,
                    '                           Ricetta_Operazione_Cod,
                    '                           0,
                    '                           AGRODATAINIZIO,
                    '                           AGRODATAFINE,
                    '                           enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    '                           "",
                    '                           "",
                    '                           objParametri)

                    '    For Each row In dtRxN.Rows
                    '        objRicettaxNote.Cancella(row("Ricetta_Cod"), row("Ricetta_Operazione_Cod"), row("Nota_Cod"), "", objParametri)
                    '    Next

                    'End If


                    Dim xListaDatiRicettaxNote As XmlNodeList
                    Dim xDatiRicettaxNote As XmlElement
                    Dim xListaRicettaxNote As XmlNodeList
                    Dim xRicettaxNote As XmlElement
                    Dim OpeDB_RicettaxNote As String
                    Dim OpeDB_RicettaxAgenda As String

                    Dim i_DatiRicettaxNote As Integer = 0

                    xListaDatiRicettaxNote = xRicetta_Operazione.GetElementsByTagName("DatiRicettaxNote_2")

                    Do While i_DatiRicettaxNote < xListaDatiRicettaxNote.Count

                        'Prelevo l'i-esimo blocco di DatiRicettaxCultivar (in realta' ne esiste uno solo)
                        xDatiRicettaxNote = xListaDatiRicettaxNote.Item(i_DatiRicettaxNote)

                        '------------------------------

                        xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")

                        Dim i_RicettaxNote As Integer = 0
                        i_RicettaxNote = 0

                        Do While i_RicettaxNote < xListaRicettaxNote.Count

                            'Prelevo l' i-esimo RicettaxCultivar
                            xRicettaxNote = xListaRicettaxNote.Item(i_RicettaxNote)

                            'Prelevo gli attributi dell'Operazione selezionata
                            OpeDB_RicettaxNote = xRicettaxNote.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_RicettaxNote

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dim xRicettaxNote_data_creazione As Date = #2/1/1900#
                                    Dim xRicettaxNote_data_Modifica As Date = #2/1/1900#
                                    Dim xRicettaxNote_username_creazione As String = ""
                                    Dim xRicettaxNote_username_modifica As String = ""

                                    If Not IsNothing(xRicettaxNote.GetAttribute("data_creazione")) AndAlso
                                     xRicettaxNote.GetAttribute("data_creazione") <> "" Then
                                        xRicettaxNote_data_creazione = CDate(xRicettaxNote.GetAttribute("data_creazione"))
                                    ElseIf data_creazione <> AGRODATAINIZIO Then
                                        xRicettaxNote_data_creazione = Data_Creazione
                                    End If

                                    If Not IsNothing(xRicettaxNote.GetAttribute("data_modifica")) AndAlso
                                     xRicettaxNote.GetAttribute("data_modifica") <> "" Then
                                        xRicettaxNote_data_Modifica = CDate(xRicettaxNote.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicettaxNote.GetAttribute("username_creazione")) AndAlso
                                     xRicettaxNote.GetAttribute("username_creazione") <> "" Then
                                        xRicettaxNote_username_creazione = CStr(xRicettaxNote.GetAttribute("username_creazione"))
                                    ElseIf Username_Creazione <> "" Then
                                        xRicettaxNote_username_creazione = Username_Creazione
                                    End If

                                    If Not IsNothing(xRicettaxNote.GetAttribute("username_modifica")) Then
                                        xRicettaxNote_username_modifica = CStr(xRicettaxNote.GetAttribute("username_modifica"))
                                    End If



                                    Dummy = objRicettaxNote.Scrivi(
                                                      CInt(Ricetta_Cod),
                                                      CInt(Ricetta_Operazione_Cod),
                                                      CInt(xRicettaxNote.GetAttribute("nota_cod")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_fine")),
                                                      objParametri,
                                                      xRicettaxNote_data_creazione,
                                                      xRicettaxNote_data_Modifica,
                                                      xRicettaxNote_username_creazione,
                                                      xRicettaxNote_username_modifica
                                                    )

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objRicettaxNote.Modifica(
                                                      CInt(Ricetta_Cod),
                                                      CInt(Ricetta_Operazione_Cod),
                                                      CInt(xRicettaxNote.GetAttribute("nota_cod")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_fine")),
                                                      String.Empty,
                                                      objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objRicettaxNote.Cancella(
                                                CInt(Ricetta_Cod),
                                                CInt(Ricetta_Operazione_Cod),
                                                CInt(xRicettaxNote.GetAttribute("nota_cod")),
                                                String.Empty,
                                                objParametri)

                            End Select

                            i_RicettaxNote = i_RicettaxNote + 1

                        Loop

                        i_DatiRicettaxNote = i_DatiRicettaxNote + 1


                    Loop

                    objRicettaxNote = Nothing
                    xListaDatiRicettaxNote = Nothing
                    xDatiRicettaxNote = Nothing
                    xListaRicettaxNote = Nothing
                    xRicettaxNote = Nothing



                    '##################################################
                    '##########  AGENDA            ####################
                    '##################################################

                    Dim xListaDatiRicettaxAgenda As XmlNodeList
                    Dim xDatiRicettaxAgenda As XmlElement
                    Dim xListaRicettaxAgenda As XmlNodeList
                    Dim xRicettaxAgenda As XmlElement

                    Dim i_DatiRicettaxAgenda As Integer = 0

                    xListaDatiRicettaxAgenda = xRicetta_Operazione.GetElementsByTagName("DatiRicettaxAgenda_2")

                    Do While i_DatiRicettaxAgenda < xListaDatiRicettaxAgenda.Count

                        'Prelevo l'i-esimo blocco di DatiRicettaxCultivar (in realta' ne esiste uno solo)
                        xDatiRicettaxAgenda = xListaDatiRicettaxAgenda.Item(i_DatiRicettaxAgenda)

                        '------------------------------

                        xListaRicettaxAgenda = xDatiRicettaxAgenda.GetElementsByTagName("RicettaxAgenda_2")

                        Dim i_RicettaxAgenda As Integer = 0
                        i_RicettaxAgenda = 0

                        Do While i_RicettaxAgenda < xListaRicettaxAgenda.Count

                            'Prelevo l' i-esimo RicettaxCultivar
                            xRicettaxAgenda = xListaRicettaxAgenda.Item(i_RicettaxAgenda)

                            'Creo l'oggetto COM
                            Dim objRicettaxAgenda As New AgronicaCoreContabDAL.RicettexAgenda_W

                            'Prelevo gli attributi dell'Operazione selezionata
                            OpeDB_RicettaxAgenda = xRicettaxAgenda.GetAttribute("TipoOperazioneDB")

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_RicettaxAgenda

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------


                                    Dim xRicettaXAgenda_data_creazione As Date = #2/1/1900#
                                    Dim xRicettaXAgenda_data_Modifica As Date = #2/1/1900#
                                    Dim xRicettaXAgenda_username_creazione As String = ""
                                    Dim xRicettaXAgenda_username_modifica As String = ""

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("data_creazione")) AndAlso
                                     xRicettaxAgenda.GetAttribute("data_creazione") <> "" Then
                                        xRicettaXAgenda_data_creazione = CDate(xRicettaxAgenda.GetAttribute("data_creazione"))
                                    ElseIf data_creazione <> AGRODATAINIZIO Then
                                        xRicettaXAgenda_data_creazione = Data_Creazione
                                    End If

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("data_modifica")) AndAlso
                                     xRicettaxAgenda.GetAttribute("data_modifica") <> "" Then
                                        xRicettaXAgenda_data_Modifica = CDate(xRicettaxAgenda.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("username_creazione")) AndAlso
                                     xRicettaxAgenda.GetAttribute("username_creazione") <> "" Then
                                        xRicettaXAgenda_username_creazione = CStr(xRicettaxAgenda.GetAttribute("username_creazione"))
                                    ElseIf Username_Creazione <> "" Then
                                        xRicettaXAgenda_username_creazione = Username_Creazione
                                    End If

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("username_modifica")) Then
                                        xRicettaXAgenda_username_modifica = CStr(xRicettaxAgenda.GetAttribute("username_modifica"))
                                    End If



                                    Dummy = objRicettaxAgenda.Scrivi(
                                                      CInt(Ricetta_Cod),
                                                      CInt(Ricetta_Operazione_Cod),
                                                      CInt(xRicettaxAgenda.GetAttribute("id_agenda")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_fine")),
                                                      objParametri,
                                                      xRicettaXAgenda_data_creazione,
                                                      xRicettaXAgenda_data_Modifica,
                                                      xRicettaXAgenda_username_creazione,
                                                      xRicettaXAgenda_username_modifica
                                                    )

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objRicettaxAgenda.Modifica(
                                                      CInt(Ricetta_Cod),
                                                      CInt(Ricetta_Operazione_Cod),
                                                      CInt(xRicettaxAgenda.GetAttribute("id_agenda")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_fine")),
                                                      String.Empty,
                                                      objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objRicettaxAgenda.Cancella(
                                                CInt(Ricetta_Cod),
                                                CInt(Ricetta_Operazione_Cod),
                                                CInt(xRicettaxAgenda.GetAttribute("id_agenda")),
                                                String.Empty,
                                                objParametri)

                            End Select

                            i_RicettaxAgenda = i_RicettaxAgenda + 1
                            objRicettaxAgenda = Nothing

                        Loop

                        i_DatiRicettaxAgenda = i_DatiRicettaxAgenda + 1


                    Loop

                    xListaDatiRicettaxAgenda = Nothing
                    xDatiRicettaxAgenda = Nothing
                    xListaRicettaxAgenda = Nothing
                    xRicettaxAgenda = Nothing




                    '#######################################################################
                    '#######################################################################
                    '#######################################################################
                    '#######################################################################


                    Select Case OpeDB_Ricetta_Operazione


                        Case 3 'CANCELLAZIONE OPERAZIONE

                            objRicetta_Operazioni.Cancella(
                                 CInt(Ricetta_Cod),
                                 CInt(Ricetta_Operazione_Cod),
                                 String.Empty,
                                 objParametri)

                            Dummy = ObjRicetteLog.Scrivi(OpeDB_Ricetta_Operazione,
                                                         "Ricette_Operazioni",
                                                         CInt(Ricetta_Cod),
                                                         CInt(Ricetta_Operazione_Cod),
                                                         Piva,
                                                         Sa_Cod,
                                                         CInt(xRicetta_Operazione.GetAttribute("lav_cod")),
                                                         CDate(xRicetta_Operazione.GetAttribute("validita_inizio")),
                                                         CStr(xRicetta_Operazione.GetAttribute("ricetta_operazione_des")),
                                                         CInt(idServizio),
                                                         objParametri,
                                                         Raccoglitore_Cod:=If(xRicetta_Operazione.HasAttribute("raccoglitore_cod") AndAlso IsNumeric(xRicetta_Operazione.GetAttribute("raccoglitore_cod")), xRicetta_Operazione.GetAttribute("raccoglitore_cod"), 0),
                                                         guidRicetta:=If(xRicetta_Operazione.HasAttribute("app_ricetta_operazione_id"), xRicetta_Operazione.GetAttribute("app_ricetta_operazione_id"), "")
                                                         )

                    End Select




                    'Elimino l'oggetto
                    objRicetta_Operazioni = Nothing


                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Ricetta_Operazioni = i_Ricetta_Operazioni + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiRicetta_Operazioni = i_DatiRicetta_Operazioni + 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiRicetta_Operazioni = Nothing
            xDatiRicetta_Operazione = Nothing
            xRicetta_Operazioni = Nothing
            xRicetta_Operazione = Nothing
            XmlDoc = Nothing


        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing AndAlso OpenTransaction Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If FlagConnessioneLocale AndAlso objParametri.objConnessione IsNot Nothing AndAlso OpenConnection Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
                objParametri.objConnessione = Nothing
            End If

        End Try

        Return xRisp


    End Function

    Private Shared Function ScriviNuovaRicetta(ByVal piva As String, ByVal veg_cod As String,
                                                  ByVal RicettaDes As String, ByVal RicettaNumero As String,
                                                  ByVal dataInizio As String, ByVal dataFine As String,
                                                    ByVal nota_des As String,
                                                  ByVal progressivoGias As Integer,
                                                  ByRef objParametriServer As AgronicaCoreParametri,
                                                  ByRef OUTPUT_Ricetta_Cod As Integer) As Boolean

        Dim xRisp As Boolean = False

        Dim xmlStr As String = CreaXmlRicetta(piva, veg_cod, RicettaDes, RicettaNumero, dataInizio, dataFine, nota_des, progressivoGias)

        Dim objW As New AgronicaCoreContabBIZ.Ricette_W
        xRisp = objW.Ricetta_Scrivi(xmlStr, OUTPUT_Ricetta_Cod, objParametriServer)

        Return xRisp

    End Function

    Private Shared Function CreaXmlRicetta(ByVal piva As String, ByVal veg_cod As String,
                                              ByVal RicettaDes As String, ByVal RicettaNumero As String,
                                              ByVal dataInizio As String, ByVal dataFine As String,
                                                ByVal nota_des As String,
                                              ByVal progressivoGias As Integer
                                              ) As String

        Dim strRet As String

        Dim TipoOperazioneDb As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, progressivoGias)

        Dim XmlDoc As New XmlDocument

        Dim XmlDatiRicetta As XmlElement = XmlDoc.CreateElement("DatiRicetta")

        '----- < RICETTA > -----
        Dim XmlRicetta As XmlElement = XmlDoc.CreateElement("Ricetta")
        XmlDatiRicetta.AppendChild(XmlRicetta)

        Dim des As String = "Ricetta (" & RicettaDes & ")"

        With XmlRicetta
            .SetAttribute("TipoOperazioneDB", TipoOperazioneDb)
            .SetAttribute("ricetta_cod", 0)
            .SetAttribute("ricetta_numero", RicettaNumero)
            .SetAttribute("piva", piva)
            .SetAttribute("sa_cod", 0)
            .SetAttribute("tipo_ricetta", enum_TipoRicetta.Standard_Destinazioni)
            .SetAttribute("ricetta_des", des)
            .SetAttribute("ricetta_des_long", des)
            .SetAttribute("veg_cod", veg_cod)
            .SetAttribute("note", nota_des)
            .SetAttribute("basecode", BaseCode.ToString)
            .SetAttribute("topcode", TopCode.ToString)
            .SetAttribute("programmazione_cod", 0)
            .SetAttribute("validita_inizio", dataInizio)
            .SetAttribute("validita_fine", dataFine)
        End With

        XmlDoc.AppendChild(XmlDatiRicetta)

        strRet = XmlDoc.InnerXml

        Return strRet

    End Function

    Public Function CreaRicetta(ByVal data_inizio As String, ByVal data_fine As String,
                           ByVal id_agenda_checked As String, ByVal id_agenda As String,
                           ByVal piva As String, ByVal sa_cod As String, ByVal veg_cod As String,
                           ByVal ricetta_des As String, ByVal ricetta_numero As String,
                           ByVal nota_des As String, progressivoGias As Integer,
                           objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard

        Dim id_agenda_array As String() = id_agenda_checked.Split(",")

        Dim id_agenda_copiabili_array As New List(Of String)
        Dim lav_cod_copiabili_array As New List(Of String)

        For i = 0 To id_agenda_array.Length - 1
            If id_agenda_array(i) = "-1" Then
                id_agenda_copiabili_array.Add(id_agenda_array(i))
                Continue For
            End If
            id_agenda_copiabili_array.Add(id_agenda_array(i))
        Next

        If id_agenda_copiabili_array.Count = 0 OrElse (id_agenda_copiabili_array.Count = 1 AndAlso id_agenda_copiabili_array(0) = "-1") Then
            r.RispostaOK = False
            r.Errore = My.Resources.AgronicaCoreContabBIZ.NonÈPossibileCopiareQuestOperazionePlurale
            Return r
        End If

        Dim ricetta_cod As Integer = 0

        ' Dim progressivoGias As Integer = HttpContext.Current.Session("ASG_ProgressivoGIAS")

        '-----------------------------------------------------
        '----------- CONNESSIONE E TRANSAZIONE ---------------
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
        '-----------------------------------------------------

        'salvataggio ricette
        Dim CreataRicetta As Boolean = False
        CreataRicetta = ScriviNuovaRicetta(piva, veg_cod, ricetta_des, ricetta_numero, data_inizio, data_fine, nota_des, progressivoGias, objParametri_Server, ricetta_cod)

        If CreataRicetta Then

            Dim objRicettaOpW As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            Dim aggiungi_operazioni As Boolean = objRicettaOpW.Aggiungi_RicettaOperazioni_Da_OperazioniAgenda(piva, sa_cod, ricetta_cod, enum_TipoRicetta.Standard_Destinazioni, id_agenda_checked, objParametri_Server, objParametri_Utenti, "")

            If aggiungi_operazioni Then

                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

                r.RispostaOK = True
                r.RispostaStringa = My.Resources.AgronicaCoreContabBIZ.RegistrazioneEffettuataConSuccesso

            Else

                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

                r.Errore = My.Resources.AgronicaCoreContabBIZ.NonÈPossibileCreareLaRicettaAPartireDallOp
                r.RispostaOK = False

            End If

        Else

            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            r.Errore = My.Resources.AgronicaCoreContabBIZ.NonÈPossibileCreareLaRicettaAPartireDallOp
            r.RispostaOK = False

        End If
        Return r
    End Function

    Public Function Ricetta_Operazione_Cancella_ESeUnicaAncheLaRicettaPadre(ricetta_cod As Integer, ricetta_operazione_cod As Integer,
                                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                                            Optional usaTransizione As Boolean = True,
                                                                            Optional ByRef objParametri_Utenti As AgronicaCoreParametri = Nothing) As String

        Dim msgerr As String = ""

        Try
            If usaTransizione Then
                'Apro la transazione
                ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            End If

            'Se la ricetta ha una sola operazione cancello l'intera ricetta, in alternativa la singola operazione
            Dim roDal_Read As New AgronicaCoreContabDAL.Ricette_Operazioni_R
            Dim objRicette_R As New AgronicaCoreContabDAL.Ricette_R
            Dim dtRicOp As DataTable = roDal_Read.Leggi(ricetta_cod, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim dtRicetta_Operazione As DataTable = roDal_Read.Leggi(ricetta_cod, ricetta_operazione_cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Dim dtRicetta As DataTable = objRicette_R.Leggi(ricetta_cod, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            'CONTROLLO PRATICA

            If dtRicetta_Operazione.Rows.Count > 0 Then
                Dim Lav_Cod As Integer = CInt(dtRicetta_Operazione.Rows(0)("Lav_Cod"))
                Dim Data_Operazione As DateTime = CDate(dtRicetta_Operazione.Rows(0)("Validita_Inizio"))
                Dim Piva = dtRicetta.Rows(0)("Piva")

                Dim dataMin As Date = AGRODATAINIZIO
                Dim dataMax As Date = AGRODATAFINE

                Dim SportelloAperto As Boolean = True
                Dim Servizio_cod As Integer

                Select Case Lav_Cod
                    Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO
                        Servizio_cod = enum_Servizi.PUA
                    Case Else
                        Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa
                End Select



                Dim objPraticheBIZ As New AgronicaCoreProfilazioneBIZ.Pratiche_R

                'Azienda in Verifica
                Dim AziendaInVerifica As Boolean = False
                Dim dataMinxVerifica = AGRODATAINIZIO
                Dim dataMaxxVerifica = AGRODATAFINE
                objPraticheBIZ.Limitazione_Data_Per_VerificaInCorso(Piva,
                                                           Servizio_cod,
                                                           Data_Operazione,
                                                           AziendaInVerifica,
                                                           dataMinxVerifica,
                                                           dataMaxxVerifica,
                                                           objParametri_Server,
                                                           objParametri_Utenti)

                'Controllo Sportello
                objPraticheBIZ.Data_Sportello_Da_Servizio(Piva,
                                                           Servizio_cod,
                                                           Data_Operazione,
                                                           SportelloAperto,
                                                           dataMin,
                                                           dataMax,
                                                           objParametri_Server,
                                                           objParametri_Utenti)

                If Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa Then
                    objPraticheBIZ.VerificaInCorso_ChiamataSecondaria_SeNessunCambiamento(Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                         Data_Operazione,
                                                                                        False,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        AziendaInVerifica,
                                                                                        dataMinxVerifica,
                                                                                        dataMaxxVerifica,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)

                    objPraticheBIZ.Sportello_ChiamataSecondaria_SeNessunCambiamento(Piva,
                                                                            enum_Servizi.QuadernoCampagnaBio,
                                                                            Data_Operazione,
                                                                            True,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            SportelloAperto,
                                                                            dataMin,
                                                                            dataMax,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti)
                End If
                If dataMinxVerifica > dataMin Then
                    dataMin = dataMinxVerifica
                End If

                If Not SportelloAperto OrElse (Data_Operazione < dataMin OrElse Data_Operazione > dataMax) AndAlso Not AziendaInVerifica Then

                    Throw New Exception(" Non è possibile eliminare l'operazione. Sportello chiuso. ")

                ElseIf AziendaInVerifica Then

                    Throw New Exception(" Non è possibile eliminare l'operazione. Azienda in verifica. ")

                End If

            End If



            If dtRicOp.Rows.Count <= 1 Then
                'Ho una sola operazione, cancello tutta la ricetta

                'Recupero la stringa XML di cancellazione
                Dim r_Read As New AgronicaCoreContabBIZ.Ricette_R
                Dim StringaXmlCancellazione As String = r_Read.Ricetta_Leggi(ricetta_cod, "", 0, 0, 0, True, objParametri_Server)

                'Cancello i dati esistenti
                Dim r_Write As New AgronicaCoreContabBIZ.Ricette_W
                Dim strDummy As String = r_Write.Ricetta_Scrivi(StringaXmlCancellazione, ricetta_cod, objParametri_Server)
            Else
                'Ho più operazioni, cancello quindi la singola operazione

                'Recupero la stringa XML di cancellazione
                Dim ro_Read As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
                Dim StringaXmlCancellazione As String = ro_Read.Ricetta_Operazioni_Leggi(ricetta_cod, ricetta_operazione_cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, True, objParametri_Server)

                Dim Piva As String = ""
                Dim Sa_Cod As Integer = 0

                If dtRicetta IsNot Nothing AndAlso dtRicetta.Rows.Count > 0 Then
                    Piva = dtRicetta.Rows(0).Item("Piva")
                    Sa_Cod = dtRicetta.Rows(0).Item("sa_cod")
                End If

                'Cancello i dati esistenti
                Dim ro_Write As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
                Dim strDummy As String = ro_Write.Ricetta_Operazione_Scrivi(Piva, Sa_Cod, StringaXmlCancellazione, ricetta_cod, enum_TipoRicetta.Standard_Destinazioni, ricetta_operazione_cod, objParametri_Server)

            End If

            '-------------------------------------------------------------------------------------------------
            'Update Tabella Alert_Entita --> Id_Agenda = 0

            'Controllo coerenza dati
            If ricetta_operazione_cod <> 0 Then

                Dim objEntita_W As New AgronicaCoreScadenziario.Alert_Entita_W
                Dim Res As Boolean = objEntita_W.Reset_Agenda(ricetta_operazione_cod, objParametri_Server)

            End If
            '-------------------------------------------------------------------------------------------------

            If usaTransizione Then
                'chiudi connessione e commit transazione
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            End If
        Catch ex As Exception
            'inserimento fallito
            msgerr = "Errore durante la cancellazione: " & ex.Message

            If usaTransizione Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
        Finally
            If usaTransizione Then
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
        End Try

        Return msgerr

    End Function

    Public Function Aggiungi_RicettaOperazioni_Da_OperazioniAgenda(piva As String, sa_cod As Integer, ricetta_cod As Integer, ricetta_tipo As enum_TipoRicetta, strIdAgenda As String,
                                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                                                   ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                                   ByRef msgFinale As String) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Ricette_Operazioni_W.Aggiungi_RicettaOperazioni_Da_OperazioniAgenda()"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Try

            '------------------------------

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

            Dim objAgendaR As New AgronicaCoreContabBIZ.Agenda_R
            Dim objRicetteOpR As New AgronicaCoreContabBIZ.Ricette_Operazioni_R

            Dim id_agenda_array As String() = strIdAgenda.Split(",")

            Dim strIdAgendaRaccoglitore As String = ""

            'id_agenda-->raccoglitore cod
            Dim dictAgendeRaccoglitore As New Dictionary(Of Integer, Integer)

            Dim objSequenze As New Agro_Sequenze

            'DT: trovo le agende legate alle agende selezionate aventi lo stesso raccoglitore_cod e le aggiungo, se non presenti, alle agende da copiare
            Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
            For i = 0 To id_agenda_array.Length - 1
                If IsNumeric(id_agenda_array(i)) AndAlso CInt(id_agenda_array(i)) > 0 Then
                    Dim raccoglitore_cod = objAgenda.Raccoglitore_Cod_From_Piva_Id_Agenda(piva, id_agenda_array(i), objParametri_Server)

                    If raccoglitore_cod <> 0 Then

                        Dim codiceRaccoglitoreRicetteCollegate = objSequenze.NuovoId_Tabella("raccoglitore", 0, 2000000000, objParametri_Server)

                        If Not dictAgendeRaccoglitore.ContainsKey(CInt(id_agenda_array(i))) Then
                            dictAgendeRaccoglitore.Add(CInt(id_agenda_array(i)), codiceRaccoglitoreRicetteCollegate)
                        End If

                        Dim dtAgendeCollegate = objAgenda.IdAgende_From_Piva_Raccoglitore_Cod(piva, raccoglitore_cod, objParametri_Server)

                        If dtAgendeCollegate IsNot Nothing Then

                            For Each dtAgenda In dtAgendeCollegate.Rows
                                If dtAgenda("id_agenda") <> id_agenda_array(i) Then

                                    If Not dictAgendeRaccoglitore.ContainsKey(dtAgenda("id_agenda")) Then
                                        dictAgendeRaccoglitore.Add(dtAgenda("id_agenda"), codiceRaccoglitoreRicetteCollegate)
                                    End If

                                    If Not id_agenda_array.Contains(dtAgenda("id_agenda")) Then
                                        If strIdAgendaRaccoglitore.Length > 0 Then
                                            strIdAgendaRaccoglitore &= ","
                                        End If
                                        strIdAgendaRaccoglitore &= dtAgenda("id_agenda")
                                    End If
                                End If

                            Next

                        End If

                    End If

                End If

            Next

            Dim id_agenda_array_raccoglitore As String() = strIdAgendaRaccoglitore.Split(",")
            id_agenda_array = id_agenda_array.Concat(id_agenda_array_raccoglitore).ToArray

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim progressivo_gias As Integer = objUtenti.ProgressivoGias_from_Superuser(objParametri_Utenti)

            For i = 0 To id_agenda_array.Length - 1

                If IsNumeric(id_agenda_array(i)) AndAlso CInt(id_agenda_array(i)) > 0 Then

                    Dim strAgenda As String = objAgendaR.Agenda_Leggi(piva, 0, CInt(id_agenda_array(i)), 0, False, objParametri_Server)

                    'se sto importartando l'operazione di agenda nel pua 
                    'verifico di legarla al pua giusto (per data)
                    If ricetta_tipo = enum_TipoRicetta.PianoDistribuzionePua Then

                        Dim xDatiAgende As XmlNodeList
                        Dim xDatiAgenda As XmlElement
                        Dim xAgende As XmlNodeList
                        Dim xAgenda As XmlElement
                        Dim xmlDoc As New XmlDocument

                        xmlDoc.LoadXml(strAgenda)

                        xDatiAgende = xmlDoc.GetElementsByTagName("DatiAgenda")
                        xDatiAgenda = xDatiAgende.Item(0)
                        xAgende = xDatiAgenda.GetElementsByTagName("Agenda")
                        xAgenda = xAgende.Item(0)

                        Dim DataOperazione As Date
                        DataOperazione = CDate(xAgenda.GetAttribute("validita_inizio"))

                        Dim objRicetteDALR As New AgronicaCoreContabDAL.Ricette_R
                        Dim RicettaCod_Pua As Integer = objRicetteDALR.Leggi_RicettaCod_PianoDistribuzionePUA(piva, 0, DataOperazione, DataOperazione, "", "", objParametri_Server)

                        If RicettaCod_Pua > 0 Then
                            ricetta_cod = RicettaCod_Pua
                        End If

                    End If

                    If strAgenda <> "" Then

                        Dim strXMLRicetta As String = objRicetteOpR.XML_GeneraStringa_Ricetta_Operazione(enum_TipoOperazioneDB.Scrittura, ricetta_cod, 0,
                                                                                                      enum_Tipo_Operazione_Agenda.Ricetta, progressivo_gias, objParametri_Server,
                                                                                                      strAgenda, Nothing, Nothing, Nothing, Nothing)


                        If strXMLRicetta <> "" Then
                            Dim Ricetta_Operazione_Cod As Integer = 0
                            strXMLRicetta = "<DatiRicetta_Operazioni>" & strXMLRicetta & "</DatiRicetta_Operazioni>"

                            Dim xmlRicettaOperazione As New XmlDocument
                            xmlRicettaOperazione.LoadXml(strXMLRicetta)

                            If dictAgendeRaccoglitore.ContainsKey(CInt(id_agenda_array(i))) Then
                                Dim elemRaccoglitoreCod As XmlNode = xmlRicettaOperazione.SelectSingleNode("DatiRicetta_Operazioni/Ricetta_Operazione")
                                elemRaccoglitoreCod.Attributes("raccoglitore_cod").Value = dictAgendeRaccoglitore(CInt(id_agenda_array(i)))
                            End If

                            Ricetta_Operazione_Scrivi(piva, sa_cod, xmlRicettaOperazione.InnerXml, ricetta_cod, 0, Ricetta_Operazione_Cod, objParametri_Server)
                        End If

                    End If

                End If

            Next

            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return xRisp

    End Function

    Public Function Update_APP_Ricetta_Operazione_ID_And_Ricetta_Operazione_Cod_RIF(Ricetta_Cod As Integer,
                                                     APP_Ricetta_Operazione_ID As String,
                                                     Ricetta_Operazione_Cod_RIF As Integer,
                                                     objParametri_Server As AgronicaCoreParametri
                                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Ricette_Operazioni_W.Update_APP_Ricetta_Operazione_ID()"

        Dim xRisp As Boolean = False

        Dim objW As New AgronicaCoreContabDAL.Ricette_Operazioni_W
        xRisp = objW.Update_APP_Ricetta_Operazione_ID_And_Ricetta_Operazione_Cod_RIF(Ricetta_Cod, APP_Ricetta_Operazione_ID, Ricetta_Operazione_Cod_RIF, objParametri_Server)

        Return xRisp

    End Function

End Class
