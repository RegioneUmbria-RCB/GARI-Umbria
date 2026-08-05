Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreModelsSTD.exceptions

Public Class Progetto_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Impresa_Progetti_Leggi(ByVal Piva As String,
                                           ByVal Progetto_Cod As Integer,
                                           ByVal Cau_Progetto As String,
                                           ByVal Cod_Contratto As Integer,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Reg As Integer,
                                           ByVal Veg_Cod As Integer,
                                           ByVal Grfi_Cod As Integer,
                                           ByVal ForDelete As Boolean,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal isGias2Gias As Boolean = False,
                                           Optional ByVal TipoG2G As Integer = 0
                                           ) As String

        Dim nomeRoutine As String = "AnagrafeBIZ.Progetto_R.Impresa_Progetti_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutti i progetti dell'utente
        '   Progetto_Cod = 0            =>  si leggono tutti i progetti dell'impresa
        '   Sa_Cod = 0                  =>  si leggono tutti i progetti dell'impresa
        '   appezza = 0                 =>  si leggono tutti i progetti del centro
        '   id_reg = 0                  =>  si leggono tutti i progetti dell'appezzamento
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim flagConnessioneLocale As Boolean = False

        Dim i, j As Integer

        Dim xmlDoc As XmlDocument

        Dim xmlDatiImpresaProgetti As XmlElement
        Dim xmlImpresaProgetti As XmlElement
        Dim xmlDatiCodici As XmlElement
        Dim xmlCodice As XmlElement
        Dim xmlDatiParticelle As XmlElement
        Dim xmlParticella As XmlElement
        Dim xmlFase As XmlElement

        Dim objImpresaProgetti As AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim objCodici As AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim objParticelle As AgronicaCoreAnagrafeDAL.ProgettixParticelle_R
        Dim objFasi As AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_R

        Dim dtProgetti As DataTable
        Dim dtCodici As DataTable
        Dim dtParticelle As DataTable
        Dim dtFasi As DataTable
        Dim dtProgrammazione As DataTable

        Dim rowCountProgetti As Integer
        Dim rowCountCodici As Integer
        Dim rowCountParticelle As Integer
        Dim rowCountFasi As Integer

        Dim risultatoFunzione As String

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                flagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------

            objImpresaProgetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

            dtProgetti = objImpresaProgetti.Leggi(CStr(Piva),
                                                  CInt(Progetto_Cod),
                                                  CStr(Cau_Progetto),
                                                  CInt(Cod_Contratto),
                                                  CInt(Sa_Cod),
                                                  CInt(Appezza),
                                                  CInt(Id_Reg),
                                                  CInt(Veg_Cod),
                                                  CInt(Grfi_Cod),
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "",
                                                  "",
                                                  objParametri,
                                                  isGias2Gias,
                                                  TipoG2G)

            'Se ottengo almeno un risultato, creo la struttura XML
            If Not IsNothing(dtProgetti) Then

                '----- < Documento XML > -----
                xmlDoc = New XmlDocument

                xmlDatiImpresaProgetti = xmlDoc.CreateElement("DatiProgetto")

                rowCountProgetti = 0

                Do While rowCountProgetti <= dtProgetti.Rows.Count - 1

                    '----- < PROGETTO > -----
                    xmlImpresaProgetti = xmlDoc.CreateElement("Progetto")

                    i = rowCountProgetti   'Alias

                    If IsDBNull(dtProgetti.Rows(i).Item("p_ha")) Then
                        dtProgetti.Rows(i).Item("p_ha") = 0
                    End If

                    With xmlImpresaProgetti
                        .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(dtProgetti.Rows(i).Item("PIVA")))
                        .SetAttribute("progetto_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_cod")))
                        .SetAttribute("cau_progetto", Agro_SQL_Load(dtProgetti.Rows(i).Item("cau_progetto")))
                        .SetAttribute("cod_contratto", Agro_SQL_Load(dtProgetti.Rows(i).Item("cod_contratto")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("appezza", Agro_SQL_Load(dtProgetti.Rows(i).Item("Appezza")))
                        .SetAttribute("id_reg", Agro_SQL_Load(dtProgetti.Rows(i).Item("Id_Reg")))
                        .SetAttribute("progetto_nome", Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_nome")))
                        .SetAttribute("progetto_des", Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_des")))
                        .SetAttribute("cod_conto", Agro_SQL_Load(dtProgetti.Rows(i).Item("cod_conto")))
                        .SetAttribute("data_inizio_prevista", Agro_SQL_Load(dtProgetti.Rows(i).Item("data_inizio_prevista")))
                        .SetAttribute("data_fioritura_prevista", Agro_SQL_Load(dtProgetti.Rows(i).Item("data_fioritura_prevista")))
                        .SetAttribute("data_fine_prevista", Agro_SQL_Load(dtProgetti.Rows(i).Item("data_fine_prevista")))
                        .SetAttribute("giudizio", Agro_SQL_Load(dtProgetti.Rows(i).Item("giudizio")))
                        .SetAttribute("veg_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("veg_cod")))
                        .SetAttribute("grfi_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("grfi_cod")))
                        .SetAttribute("stato_impianto", Agro_SQL_Load(dtProgetti.Rows(i).Item("stato_impianto")))
                        .SetAttribute("regolamento_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("regolamento_cod")))
                        .SetAttribute("disciplinare_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("disciplinare_cod")))
                        .SetAttribute("csprogetto_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("csprogetto_cod")))
                        .SetAttribute("ricavi_previsti", Agro_SQL_Load(dtProgetti.Rows(i).Item("ricavi_previsti")))
                        .SetAttribute("produzione_prevista", Agro_SQL_Load(dtProgetti.Rows(i).Item("produzione_prevista")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(dtProgetti.Rows(i).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(dtProgetti.Rows(i).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(dtProgetti.Rows(i).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(dtProgetti.Rows(i).Item("Username_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(dtProgetti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(dtProgetti.Rows(i).Item("Validita_Fine")))
                        .SetAttribute("p_ha", Agro_SQL_Load(dtProgetti.Rows(i).Item("p_ha")))
                        .SetAttribute("disciplinare_pubblicoprivato", Agro_SQL_Load(dtProgetti.Rows(i).Item("Disciplinare_PubblicoPrivato")))
                        .SetAttribute("regolamento_concimazioni_cod", Agro_SQL_Load(dtProgetti.Rows(i).Item("Regolamento_Concimazioni_Cod")))

                        ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                        .SetAttribute("P_HA_Femmine", Agro_SQL_Load(dtProgetti.Rows(i).Item("P_HA_Femmine")))
                        .SetAttribute("P_HA_Maschi", Agro_SQL_Load(dtProgetti.Rows(i).Item("P_HA_Maschi")))
                        ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -


                        .SetAttribute("sup_prog", Agro_SQL_Load(dtProgetti.Rows(i).Item("Sup_Prog")))
                        .SetAttribute("flagsecondoraccolto", Agro_SQL_Load(dtProgetti.Rows(i).Item("FlagSecondoRaccolto")))
                    End With


                    '#################################
                    '##########  CODICI  #############
                    '#################################

                    objCodici = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                    xmlDatiCodici = xmlDoc.CreateElement("DatiCodici")

                    dtCodici = objCodici.LeggixProgetto(CStr(Agro_SQL_Load(dtProgetti.Rows(i).Item("PIVA"))),
                                                        CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Sa_Cod"))),
                                                        CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Appezza"))),
                                                        CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Id_Reg"))),
                                                        "",
                                                        CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_cod"))),
                                                        0,
                                                        "",
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri)


                    If Not IsNothing(dtCodici) Then

                        rowCountCodici = 0

                        Do While rowCountCodici <= dtCodici.Rows.Count - 1

                            '----- < CODICE > -----
                            xmlCodice = xmlDoc.CreateElement("CodiceImpianto")

                            j = rowCountCodici 'Alias

                            With xmlCodice
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_cod", Agro_SQL_Load(dtCodici.Rows(j).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(dtCodici.Rows(j).Item("val_cod")))
                                .SetAttribute("progetto_cod", Agro_SQL_Load(dtCodici.Rows(j).Item("progetto_cod")))
                                .SetAttribute("descrizione", Agro_SQL_Load(dtCodici.Rows(j).Item("descrizione")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(dtCodici.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(dtCodici.Rows(j).Item("validita_fine")))
                            End With

                            xmlDatiCodici.AppendChild(xmlCodice)
                            xmlCodice = Nothing

                            '----- < / CODICE > -----
                            rowCountCodici += 1

                        Loop

                        dtCodici.Dispose()

                    End If

                    dtCodici = Nothing
                    objCodici = Nothing


                    '#################################
                    '##########  PARTICELLE  #########
                    '#################################

                    objParticelle = New AgronicaCoreAnagrafeDAL.ProgettixParticelle_R

                    xmlDatiParticelle = xmlDoc.CreateElement("DatiParticellexProgetto")

                    dtParticelle = objParticelle.LeggixProgetto(CStr(Agro_SQL_Load(dtProgetti.Rows(i).Item("PIVA"))),
                                                                CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Sa_Cod"))),
                                                                CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Appezza"))),
                                                                CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Id_Reg"))),
                                                                CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_cod"))),
                                                                0,
                                                                "",
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "",
                                                                "",
                                                                objParametri)

                    If Not IsNothing(dtParticelle) Then

                        rowCountParticelle = 0

                        Do While rowCountParticelle <= dtParticelle.Rows.Count - 1

                            '----- < PARTICELLA > -----
                            xmlParticella = xmlDoc.CreateElement("ParticellaxProgetto")

                            j = rowCountParticelle 'Alias

                            With xmlParticella
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("part_cod", Agro_SQL_Load(dtParticelle.Rows(j).Item("part_cod")))
                                .SetAttribute("prov", Agro_SQL_Load(dtParticelle.Rows(j).Item("prov")))
                                .SetAttribute("com", Agro_SQL_Load(dtParticelle.Rows(j).Item("com")))
                                .SetAttribute("sezione", Agro_SQL_Load(dtParticelle.Rows(j).Item("sezione")))
                                .SetAttribute("foglio", Agro_SQL_Load(dtParticelle.Rows(j).Item("foglio")))
                                .SetAttribute("numero", Agro_SQL_Load(dtParticelle.Rows(j).Item("numero")))
                                .SetAttribute("subalterno", Agro_SQL_Load(dtParticelle.Rows(j).Item("subalterno")))
                                .SetAttribute("id_cod", Agro_SQL_Load(dtParticelle.Rows(j).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(dtParticelle.Rows(j).Item("val_cod")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(dtParticelle.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(dtParticelle.Rows(j).Item("validita_fine")))
                            End With

                            xmlDatiParticelle.AppendChild(xmlParticella)
                            xmlParticella = Nothing

                            '----- < / PARTICELLA > -----
                            rowCountParticelle += 1

                        Loop

                        dtParticelle.Dispose()

                    End If

                    dtParticelle = Nothing
                    objParticelle = Nothing



                    '#################################
                    '#######  PROGRAMMAZIONE  ########
                    '#################################

                    Dim xmlProgrammazione As XmlElement
                    Dim leggiProgrammazioni As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R

                    xmlProgrammazione = xmlDoc.CreateElement("Programmazione")

                    dtProgrammazione = leggiProgrammazioni.Leggi(CStr(Agro_SQL_Load(dtProgetti.Rows(i).Item("PIVA"))),
                                                                 CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Sa_Cod"))),
                                                                 CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Appezza"))),
                                                                 CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("Id_Reg"))),
                                                                 CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_cod"))),
                                                                 0,
                                                                 0,
                                                                 "",
                                                                 "",
                                                                 objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If Not IsNothing(dtProgrammazione) AndAlso dtProgrammazione.Rows.Count > 0 Then

                        With xmlProgrammazione
                            .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                            .SetAttribute("piva", Agro_SQL_Load(dtProgrammazione.Rows(0)("piva")))
                            .SetAttribute("sa_cod", Agro_SQL_Load(dtProgrammazione.Rows(0)("sa_cod")))
                            .SetAttribute("appezza", Agro_SQL_Load(dtProgrammazione.Rows(0)("appezza")))
                            .SetAttribute("id_reg", Agro_SQL_Load(dtProgrammazione.Rows(0)("id_reg")))
                            .SetAttribute("progetto_cod", Agro_SQL_Load(dtProgrammazione.Rows(0)("progetto_cod")))
                            .SetAttribute("programmazione_cod", Agro_SQL_Load(dtProgrammazione.Rows(0)("programmazione_cod")))
                            .SetAttribute("programmazione_entita_cod", Agro_SQL_Load(dtProgrammazione.Rows(0)("programmazione_entita_cod")))
                            .SetAttribute("validita_inizio", Agro_SQL_Load(dtProgrammazione.Rows(0)("validita_inizio")))
                            .SetAttribute("validita_fine", Agro_SQL_Load(dtProgrammazione.Rows(0)("validita_fine")))
                            .SetAttribute("data_creazione", Agro_SQL_Load(dtProgrammazione.Rows(0)("data_creazione")))
                            .SetAttribute("data_modifica", Agro_SQL_Load(dtProgrammazione.Rows(0)("data_modifica")))
                            .SetAttribute("username_creazione", Agro_SQL_Load(dtProgrammazione.Rows(0)("username_creazione")))
                            .SetAttribute("username_modifica", Agro_SQL_Load(dtProgrammazione.Rows(0)("username_modifica")))
                        End With

                    End If

                    xmlImpresaProgetti.AppendChild(xmlProgrammazione)


                    '#################################
                    '##########  FASI  ###############
                    '#################################


                    objFasi = New AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_R

                    dtFasi = objFasi.Leggi(CStr(Agro_SQL_Load(dtProgetti.Rows(i).Item("PIVA"))),
                                           CInt(Agro_SQL_Load(dtProgetti.Rows(i).Item("progetto_cod"))),
                                           0,
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           "",
                                           objParametri)


                    If Not IsNothing(dtFasi) Then

                        rowCountFasi = 0

                        Do While rowCountFasi <= dtFasi.Rows.Count - 1

                            '----- < FASE > -----
                            xmlFase = xmlDoc.CreateElement("Progetto_Fase")

                            j = rowCountFasi 'Alias

                            With xmlFase
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(dtFasi.Rows(j).Item("piva")))
                                .SetAttribute("progetto_cod", Agro_SQL_Load(dtFasi.Rows(j).Item("progetto_cod"))) 'Indico che il codice è associato all'impianto
                                .SetAttribute("fase_cod", Agro_SQL_Load(dtFasi.Rows(j).Item("fase_cod")))
                                .SetAttribute("fase_des", Agro_SQL_Load(dtFasi.Rows(j).Item("fase_des")))
                                .SetAttribute("giudizio", Agro_SQL_Load(dtFasi.Rows(j).Item("giudizio")))
                                .SetAttribute("budget", Agro_SQL_Load(dtFasi.Rows(j).Item("budget")))
                                .SetAttribute("lav_cod", Agro_SQL_Load(dtFasi.Rows(j).Item("lav_cod")))
                                .SetAttribute("gru_op", Agro_SQL_Load(dtFasi.Rows(j).Item("gru_op")))
                                .SetAttribute("data_inizio_prevista", Agro_SQL_Load(dtFasi.Rows(j).Item("data_inizio_prevista")))
                                .SetAttribute("data_fine_prevista", Agro_SQL_Load(dtFasi.Rows(j).Item("data_fine_prevista")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(dtFasi.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(dtFasi.Rows(j).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(dtFasi.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(dtFasi.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(dtFasi.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(dtFasi.Rows(j).Item("Username_Modifica")))

                            End With

                            xmlImpresaProgetti.AppendChild(xmlFase)
                            xmlFase = Nothing

                            '----- < / FASE > -----
                            rowCountFasi += 1

                        Loop

                        dtFasi.Dispose()

                    End If

                    dtFasi = Nothing
                    objFasi = Nothing



                    '#################################
                    '#################################
                    '#################################


                    xmlImpresaProgetti.AppendChild(xmlDatiCodici)
                    xmlImpresaProgetti.AppendChild(xmlDatiParticelle)
                    xmlDatiImpresaProgetti.AppendChild(xmlImpresaProgetti)
                    xmlDatiCodici = Nothing
                    xmlImpresaProgetti = Nothing

                    rowCountProgetti += 1

                Loop

                dtProgetti.Dispose()
                dtProgetti = Nothing
                objImpresaProgetti = Nothing

                '#################################
                '#################################
                '#################################



                '----- < / Progetto > -----

                xmlDoc.AppendChild(xmlDatiImpresaProgetti)

                risultatoFunzione = xmlDoc.OuterXml

                '----- < / Documento XML > -----
                xmlDatiImpresaProgetti = Nothing
                xmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun impianto ...
                risultatoFunzione = ""

            End If

        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            objImpresaProgetti = Nothing
            objCodici = Nothing

            If flagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return risultatoFunzione

    End Function

    Public Function Leggi_Esercizi_Anagrafica(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal Id_Reg As Integer,
                                              ByVal specie As Specie,
                                              ByVal data As Date,
                                              ByVal filtroData As Boolean,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri
                                              ) As List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

        Dim Esercizi As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

        If Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 Then
            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
            Dim appFine As Date = objParametri_Server.FinestraTemporaleFine

            If filtroData Then
                objParametri_Server.FinestraTemporaleInizio = data
                objParametri_Server.FinestraTemporaleFine = data
            Else
                objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
                objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
            End If


            Dim Dt As DataTable
            Dt = objProgetto.Leggi(
                CStr(Piva),
                0,
                CInt(9100),
                0,
                CInt(Sa_Cod),
                CInt(Appezza),
                CInt(Id_Reg),
                0,
                0,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                " Imprese_Progetti.Validita_Inizio ASC ",
                objParametri_Server
                )

            objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
            objParametri_Server.FinestraTemporaleFine = CDate(appFine)

            For Each row In Dt.Rows
                Dim Esercizio = Leggi_Esercizio_Anagrafica(specie, row("Progetto_Cod"), objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                Esercizi.Add(Esercizio)
            Next
        End If

        Return Esercizi
    End Function

    Public Function Leggi_Esercizio_Anagrafica(Specie As Specie,
                                               ByVal Progetto_Cod As Integer,
                                               ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               Optional contributeTypeFilter As ContributeType = ContributeType.Anything
                                               ) As AgronicaCoreModelsSTD.anagrafiche.Esercizio

        Dim esercizio As New AgronicaCoreModelsSTD.anagrafiche.Esercizio(Progetto_Cod, "")
        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim Dt As DataTable

        Dt = objProgetto.Leggi(
            "",
            Progetto_Cod,
            CInt(CAU_PROGETTO_PRODUZIONE),
            0, 0, 0, 0, 0, 0,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            " Imprese_Progetti.Validita_Inizio DESC ",
            objParametri_Server
            )

        esercizio.impiantoPK = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(
            Dt.Rows(0).Item("id_reg"),
            New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(
                Dt.Rows(0).Item("appezza"),
                New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(
                    Dt.Rows(0).Item("sa_cod"),
                    Dt.Rows(0).Item("piva")
                )
            )
        )

        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim sup_imp = objRegImpianti.LeggiSuperficie(
            Dt.Rows(0).Item("piva"), Dt.Rows(0).Item("sa_cod"),
            Dt.Rows(0).Item("appezza"),
            Dt.Rows(0).Item("id_reg"),
            objParametri_Server
        )

        esercizio.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Dt.Rows(0)("Validita_Inizio"), Dt.Rows(0)("Validita_Fine"))
        esercizio.lotto = Dt.Rows(0)("Progetto_Nome")
        esercizio.descrizione = Dt.Rows(0)("Progetto_Des")
        esercizio.gruppoRaccolta = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(
            If(IsDBNull(Dt.Rows(0)("GruppoRaccolta_Cod")), 0, Dt.Rows(0)("GruppoRaccolta_Cod")),
            If(IsDBNull(Dt.Rows(0)("GruppoRaccolta_Des")), "", Dt.Rows(0)("GruppoRaccolta_Des"))
        )

        Dim objRegolamento As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim Regolamento_Des = ""

        If Dt.Rows(0).Item("Regolamento_Cod") <> 0 Then
            Dim dtReg = objRegolamento.Leggi(Dt.Rows(0).Item("Regolamento_Cod"), enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If dtReg.Rows.Count > 0 Then
                Regolamento_Des = dtReg.Rows(0)("Reg_DES")
            End If
        End If

        esercizio.regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti(Dt.Rows(0).Item("Regolamento_Cod")) With {.descrizione = Regolamento_Des}
        esercizio.disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare(Dt.Rows(0).Item("Disciplinare_cod")) With {
            .disciplinarePubblicoPrivato = Dt.Rows(0).Item("disciplinare_pubblicoprivato"),
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }

        If IsDBNull(Dt.Rows(0).Item("FlagSecondoRaccolto")) OrElse Dt.Rows(0).Item("FlagSecondoRaccolto") = 0 Then
            esercizio.flagSecondoRaccolto = False
        Else
            esercizio.flagSecondoRaccolto = True
        End If

        If Specie IsNot Nothing Then
            esercizio.disciplinare = Leggi_Disciplinare_Completo(
                Specie,
                esercizio.regolamento,
                Dt.Rows(0).Item("Disciplinare_cod"),
                Dt.Rows(0).Item("disciplinare_pubblicoprivato"),
                objParametri_Super_Server,
                objParametri_Server,
                objParametri_Utenti
                )
        End If

        Dim apportoMacroElementi As New ApportoMacroelementi

        apportoMacroElementi.pianoConcimazione = New RegolamentoConcimazione(Dt.Rows(0).Item("Regolamento_Concimazioni_Cod"))
        apportoMacroElementi.tipologia = New FinalitaPianoConcimazione(Dt.Rows(0).Item("Regolamento_Concimazioni_Cod"))
        apportoMacroElementi.fase = New FaseCicloColturale(Dt.Rows(0).Item("Stato_Impianto")) With {.descrizione = Dt.Rows(0).Item("Fase_Des")}

        apportoMacroElementi.tipologia = New FinalitaPianoConcimazione(Dt.Rows(0).Item("Tipologia_Cod")) With {.descrizione = Dt.Rows(0).Item("Tipologia_Des")}

        If esercizio.regolamento.codice = 4 Then
            'Regolamento Bio --> No Disciplinare
            esercizio.vincolo = New Vincolo(esercizio.regolamento.codice) With {
                .descrizione = esercizio.regolamento.descrizione,
                .regolamento = esercizio.regolamento,
                .disciplinare = New Disciplinare("") With {
                    .descrizione = "",
                    .disciplinarePubblicoPrivato = 1,
                    .idTr = 3,
                    .regolamentoConcimazione = New RegolamentoConcimazione(0)
                }
            }
        Else
            Dim disciplinareDescrizione = "Nessuno"

            If esercizio.disciplinare.descrizione <> "" Then
                disciplinareDescrizione = esercizio.disciplinare.descrizione
            End If

            Dim disciplinareCodice As String = "1"

            If esercizio.disciplinare.codice <> "0" Then
                If esercizio.disciplinare.codice.Split("/").Length = 4 Then
                    disciplinareCodice = esercizio.disciplinare.codice.Split("/")(1) &
                        "_" & esercizio.disciplinare.codice.Split("/")(0)
                Else
                    disciplinareCodice = esercizio.disciplinare.codice
                End If
            End If

            Dim regolamento As Regolamenti = New Regolamenti(1) With {.descrizione = "Nessuno"}

            If esercizio.regolamento.codice <> 0 Then
                regolamento = esercizio.regolamento
            End If

            'Disciplinare Selezionato
            esercizio.vincolo = New Vincolo(disciplinareCodice) With {
                .descrizione = disciplinareDescrizione,
                .regolamento = regolamento,
                .disciplinare = esercizio.disciplinare
            }
        End If

        esercizio.piante_Ha = Dt.Rows(0).Item("p_ha")
        esercizio.piante_Impianto = Math.Round(Dt.Rows(0).Item("p_ha") * sup_imp)
        esercizio.data_Fioritura_Prevista = CDate(Dt.Rows(0).Item("data_fioritura_prevista"))
        esercizio.data_Raccolta_Prevista = CDate(Dt.Rows(0).Item("data_fine_prevista"))
        esercizio.data_Semina_Trapianto_Prevista = CDate(Dt.Rows(0).Item("data_inizio_prevista"))
        esercizio.id_tr = 3
        esercizio.resa_prevista = Dt.Rows(0).Item("produzione_prevista")

        If Not IsDBNull(Dt.Rows(0).Item("P_HA_Femmine")) Then
            esercizio.piante_Ha_Femmine = Dt.Rows(0).Item("P_HA_Femmine")
            esercizio.Piante_Ha_Impianto_Femmine = Math.Round(Dt.Rows(0).Item("P_HA_Femmine") * sup_imp)
        End If

        If Not IsDBNull(Dt.Rows(0).Item("P_HA_Maschi")) Then
            esercizio.Piante_Ha_Maschi = Dt.Rows(0).Item("P_HA_Maschi")
            esercizio.Piante_Ha_Impianto_Maschi = Math.Round(Dt.Rows(0).Item("P_HA_Maschi") * sup_imp)
        End If

        esercizio.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(0, TRASFORMATI_VEGETALI) With {.descrizione = ""}

        If Not IsDBNull(Dt.Rows(0)("Mat_Cod")) AndAlso CInt(Dt.Rows(0)("Mat_Cod")) > 0 Then
            Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim dtProdotto = objMateriePrime.Leggi(Dt.Rows(0)("Piva"), 0, TRASFORMATI_VEGETALI, CInt(Dt.Rows(0)("Mat_Cod")), "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If dtProdotto.Rows.Count > 0 Then
                Dim prodotto_des = dtProdotto.Rows(0)("Mat_Des")
                If Not IsDBNull(dtProdotto.Rows(0)("Cod_Articolo")) AndAlso CStr(dtProdotto.Rows(0)("Cod_Articolo")) <> "" Then
                    prodotto_des &= " - " & CStr(dtProdotto.Rows(0)("Cod_Articolo"))
                End If
                esercizio.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(Dt.Rows(0)("Mat_Cod"), TRASFORMATI_VEGETALI) With {.descrizione = prodotto_des}
            End If
        End If

        ''
        ''  CODICI PROGETTO
        ''
        Dim objCodici = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim DtCodici = objCodici.LeggixProgetto("", 0, 0, 0, "", Progetto_Cod, 0, "",
                                                enumSelezioneVariabile.Selezione_TabellaCompleta, "", "",
                                                objParametri_Server)

        Dim OrganismoReferente = ""

        Dim StrCodiciImpianto As String
        Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
        'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
        'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
        'perché già presenti
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA) & " Or ", "")

        Dim Codici As New List(Of CodiciAnagrafeValori)

        'StrCodiciImpianto = StrCodiciImpianto & ", 1287, 1288"
        If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then

            For Each rowCodice In DtCodici.Rows
                Dim id_cod As Integer = rowCodice("ID_Cod")
                Dim val_cod As String = ""
                If Not IsDBNull(rowCodice("Val_Cod")) Then
                    val_cod = rowCodice("Val_Cod")
                End If
                Dim validita_inizio As Date = rowCodice("Validita_Inizio1")
                Dim validita_fine As Date = rowCodice("Validita_Fine1")

                Select Case id_cod

                    Case enum_CodiciAnagrafe.Finalita_Concimazione_Impianto
                        'If val_cod <> "" Then
                        '    apportoMacroElementi.tipologia = New FinalitaPianoConcimazione(val_cod)
                        'End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteN
                        If val_cod <> "" Then
                            apportoMacroElementi.n = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteP
                        If val_cod <> "" Then
                            apportoMacroElementi.p2o5 = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteK
                        If val_cod <> "" Then
                            apportoMacroElementi.k2o = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteMg
                        If val_cod <> "" Then
                            apportoMacroElementi.mgo = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati
                        If val_cod <> "" Then
                            Dim objRif As New AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R
                            Dt = objRif.Leggi(True, val_cod, val_cod, "", "", objParametri_Server)

                            If Dt.Rows.Count > 0 Then
                                esercizio.riferimento_Trasferimento_Dati = New AgronicaCoreModelsSTD.anagrafiche.Contatto()
                                esercizio.riferimento_Trasferimento_Dati.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(val_cod, "")
                                esercizio.riferimento_Trasferimento_Dati.primaryKey.partitaIva = ""
                                esercizio.riferimento_Trasferimento_Dati.primaryKey.codice = val_cod
                                esercizio.riferimento_Trasferimento_Dati.ragione_Sociale = Dt.Rows(0)("ragsoc_padre")
                            End If
                        End If
                        'esercizio.riferimento_Trasferimento_Dati = val_cod

                    Case enum_CodiciAnagrafe.Organismo_Referente
                        If val_cod <> "" Then
                            Dim objOrganismoRef As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
                            Dim orgRef = objOrganismoRef.OrganismoReferente_from_Piva(val_cod, objParametri_Server)
                            If orgRef <> "" Then
                                esercizio.organismo_Referente = New AgronicaCoreModelsSTD.anagrafiche.Contatto()
                                esercizio.organismo_Referente.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK("", val_cod)
                                esercizio.organismo_Referente.primaryKey.partitaIva = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                esercizio.organismo_Referente.primaryKey.codice = val_cod
                                esercizio.organismo_Referente.ragione_Sociale = orgRef
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Magazzino_Conferimento

                        If val_cod <> "" Then
                            'Dim ValCodModificato As Boolean

                            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

                            Dim Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente("",
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametri_Server)
                            Dim mag_arr = val_cod.Split("|")

                            Dim expression As String
                            If mag_arr.Length > 2 Then
                                expression = "Fabbricato_Cod = " & mag_arr(0) & " And sa_cod = " & mag_arr(1) & " And PIVA = '" & mag_arr(2) & "' "
                            End If
                            Dim foundRows As DataRow()

                            ' Use the Select method to find all rows matching the filter.
                            foundRows = Dt_Mag.Select(expression)

                            'val_cod = Controlla_ValCod_MagazzinoConferimento(val_cod, OrganismoReferente, ValCodModificato)
                            If mag_arr.Length = 3 Then
                                Dim pivaMag As String = mag_arr(2)
                                Dim sa_codMag As Integer = mag_arr(1)
                                Dim fabbricato_CodMag As Integer = mag_arr(0)
                                Dim rag_soc_mag = ""
                                If foundRows.Length > 0 Then
                                    rag_soc_mag = foundRows(0).Item("fabbricato_des") & " (" & foundRows(0).Item("rag_soc") & ")"
                                End If

                                esercizio.magazzino_Conferimento = New Fabbricato() With {
                                    .primaryKey = New Fabbricato.PK() With {
                                        .codice = fabbricato_CodMag,
                                        .centroAziendalePK = New CentroAziendale.PK(sa_codMag, pivaMag)
                                    },
                                    .descrizione = rag_soc_mag
                                }
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Capitolato_Privato
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CapitolatoPrivato, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "InfoAgg_Cod like '%" & val_cod & "%'", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.capitolato_Privato = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Codice_Residuo
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Residuo, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.residuo = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Codice_Certificazione
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CertificazioniAziendali_R
                            Dim list As List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr) = New List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr)
                            Dim listaCodici As List(Of Integer) = New List(Of Integer)
                            For Each codice In val_cod.ToString.Split("|".ToCharArray)
                                listaCodici.Add(CInt(codice))
                            Next
                            Dt = objCAC.Leggi(listaCodici, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
                            For Each codice In Dt.Rows
                                list.Add(New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(code:=CInt(codice.Item("CA_Cod")), descr:=codice.Item("CA_Des")))
                            Next
                            esercizio.certificazioneAziendale = list
                        End If
                    Case enum_CodiciAnagrafe.Contributi
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreMetaSchemaDAL.Contributi
                            Dim list As New List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr)
                            Dim listaCodici As New List(Of Integer)
                            For Each codice In val_cod.ToString.Split("|".ToCharArray)
                                listaCodici.Add(CInt(codice))
                            Next
                            Dt = objCAC.LeggiContributi(AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, listaCodici)
                            For Each codice In Dt.Rows
                                list.Add(New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(code:=CInt(codice.Item("Contributo_Cod")), description:=codice.Item("Contributo_Des")))
                            Next
                            esercizio.contributi = list
                        End If
                    Case enum_CodiciAnagrafe.Tecnico
                        If val_cod <> "" Then
                            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
                            Dim list As New List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr)
                            Dim listaCodici As New List(Of String)
                            For Each codice In val_cod.ToString.Split("|".ToCharArray)
                                listaCodici.Add(codice)
                            Next
                            Dt = objContattiR.Contatti_Contatto_Leggi(CStr(esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva),
                                                  "",
                                                  0,
                                                  COD_TECNICO,
                                                  True,
                                                  False,
                                                  0,
                                                  0,
                                                  False,
                                                  0,
                                                  0,
                                                  0,
                                                  "", True, 0, 0, 0, 0, 0,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "", "", objParametri_Server, Lista_Cod_Contatto:=listaCodici)

                            For Each codice In Dt.Rows
                                list.Add(New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(code:=codice.Item("Cod_Contatto"), description:=codice.Item("Cognome") & " " & codice.Item("Nome")))
                            Next
                            esercizio.tecnico = list
                        End If
                    Case enum_CodiciAnagrafe.Codice_Certificazione_Prodotto
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Certificazione_Prodotto, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.certificazioneProdotto = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Zespri_Fasi_Fase
                        If val_cod <> "" Then
                            Dim objOTab As New AgronicaCoreAnagrafeDAL.OTabelle_R
                            Dt = objOTab.LeggiXDescrizione(enum_CodiciAnagrafe.Zespri_Fasi_Fase, val_cod, objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.licenza_Coltivazione = New AgronicaCoreModelsSTD.metaschema.LicenzaColtivazione(val_cod)
                                esercizio.licenza_Coltivazione.descrizione = Dt.Rows(0)("Descrizione")
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi
                        If val_cod <> "" Then
                            If Specie IsNot Nothing Then
                                esercizio.iaf = Leggi_IAF_Completi(val_cod,
                                                                   esercizio.disciplinare,
                                                                   Specie,
                                                                   objParametri_Super_Server,
                                                                   objParametri_Server,
                                                                   objParametri_Utenti)
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Impianto_PianoSemina
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Piano_Semina, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.piano_Semina = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                                esercizio.piano_Semina.descrizione = Dt.Rows(0)("InfoAgg_Cod") & " " & Dt.Rows(0)("InfoAgg_Des")
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Lavorazione
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Lavorazione, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.lavorazione = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Modalita_Liquidazione
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.ModalitaLiquidazione, val_cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.modalita_liquidazione = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Origine_Prodotto
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.OrigineProdotto, val_cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.origine_prodotto = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Specifica
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Specifica, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.specifica = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Distinta_Chiusa
                        If val_cod <> "" Then
                            esercizio.esercizio_Chiuso = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Codice_Impianto_Ribaltato

                        'rowNew("distinta_replica_codice") = val_cod
                        'rowNew("distinta_replica") = If(String.IsNullOrEmpty(val_cod), "0", "1")
                    Case Else

                        If InStr(StrCodiciImpianto, id_cod) <> 0 Then
                            'Dim codice As New AnagrafeNG.Codici

                            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                            Dim CodiceAnagrafeDes = objCodiceAnagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                                                                    CInt(id_cod), objParametri_Server)

                            Dim codice = New CodiciAnagrafeValori() With {
                                .valore = val_cod,
                                .validita = New IntervalloTemporale(validita_inizio, validita_fine),
                                .codiceAnagrafe = New CodiceAnagrafe(id_cod) With {
                                    .descrizione = CodiceAnagrafeDes
                                }
                            }

                            Codici.Add(codice)

                        End If
                End Select
            Next

        End If

        esercizio.codici = Codici

        esercizio.apportiMassimiMacroelementi = apportoMacroElementi
        esercizio.catastoEsercizio = Leggi_Particelle_Progetto(Progetto_Cod, objParametri_Server)

        Dim objProgettiXContributi As New ProgettiXContributi_R
        esercizio.acaContributes = objProgettiXContributi.Read(
            objServer:=objParametri_Server,
            objUtenti:=objParametri_Utenti,
            project:=Progetto_Cod,
            type:=ContributeType.ACA
            )

        Return esercizio
    End Function


    Public Function Leggi_Disciplinare_Completo(specie As Specie,
                                                regolamento As Regolamenti,
                                                disciplinare_cod As Integer,
                                                disciplinarePubblico_Privato As Integer,
                                                objParametri_Super_Server As AgronicaCoreParametri,
                                                objParametri_Server As AgronicaCoreParametri,
                                                objParametri_Utenti As AgronicaCoreParametri) As Disciplinare

        Dim disciplinare As New Disciplinare(CStr(disciplinare_cod)) With {
            .descrizione = "",
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }

        If disciplinare_cod = -2 Then
            Return New Disciplinare(CStr(disciplinare_cod)) With {
            .descrizione = "Nessuno",
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }
        End If

        If disciplinare_cod <> 0 Then
            Dim leggiDpi As New AgronicaCoreDpiDAL.Dpi_R
            Dim dt = leggiDpi.Leggi_DPI_Regolamenti(-1, disciplinare_cod, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
            'Dim dt = AgronicaCoreWebService.Disciplinari_WS.Disciplinari_Elenco_TuttigliElemInChiave(objParametri_Super_Server,
            '                                                            objParametri_Server,
            '                                                            objParametri_Utenti,
            '                                                            0,
            '                                                            specie.codice, 0, disciplinarePubblico_Privato, True, False, True)

            If dt.Rows.Count > 0 Then
                disciplinare.codice = CStr(disciplinare_cod) & "/" & dt.Rows(0)("Flag_Privato_Pubblico") & "/" & dt.Rows(0)("PUA_Regolamento_Cod") & "/" & dt.Rows(0)("ID_TR")
                disciplinare.descrizione = dt.Rows(0)("NomeEsteso")
                disciplinare.disciplinarePubblicoPrivato = dt.Rows(0)("Flag_Privato_Pubblico")
                disciplinare.idTr = dt.Rows(0)("ID_TR")
                disciplinare.regolamentoConcimazione = New RegolamentoConcimazione(dt.Rows(0)("PUA_Regolamento_Cod"))
            End If
        End If


        Return disciplinare
    End Function

    Public Function Leggi_IAF_Completi(listIaf As String,
                                       disciplinare As Disciplinare,
                                       specie As Specie,
                                       objParametri_Super_Server As AgronicaCoreParametri,
                                       objParametri_Server As AgronicaCoreParametri,
                                       objParametri_Utenti As AgronicaCoreParametri
                                       ) As List(Of ImpegniAggiuntiviFacoltativi)

        Dim list As New List(Of ImpegniAggiuntiviFacoltativi)
        Dim listIafInt As New List(Of Integer)
        Dim arrIafStr = listIaf.Split("|")
        For Each el In arrIafStr
            If IsNumeric(el) Then
                listIafInt.Add(CInt(el))
            End If
        Next

        If listIafInt.Count > 0 Then

            Dim dt = ChiamateWS.IAF_Elenco(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, disciplinare.codice, specie.codice, True)

            For Each iaf_cod In listIafInt

                Dim dr = dt.Select(" IAF_Cod = " & iaf_cod)

                If dr.Length > 0 Then
                    list.Add(New ImpegniAggiuntiviFacoltativi(dr(0)("IAF_Cod")) With {.descrizione = dr(0)("IAF_Descrizione")})
                End If

            Next

        End If

        Return list
    End Function

    Public Function Leggi_Particelle_Progetto(Progetto_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As List(Of CatastoEsercizio)
        Dim particelleList As New List(Of CatastoEsercizio)
        Dim objParticelle As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_R

        Dim DtParticelle = objParticelle.LeggixProgetto("", 0, 0, 0, Progetto_Cod,
                                                0, "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "", "", objParametri_Server)

        For Each partRow In DtParticelle.Rows


            Dim id_Cod = partRow("id_Cod")
            Dim val_Cod = partRow("val_cod")

            Dim prov = partRow("Prov")
            Dim provincia = partRow("Provincia")
            Dim com = partRow("Com")
            Dim comune = partRow("comune")
            Dim sezione = ""
            If partRow("sezione") <> "0" Then
                sezione = partRow("sezione")
            End If
            Dim foglio = partRow("Foglio")
            Dim numero = partRow("Numero")
            Dim subalterno = ""
            If partRow("subalterno") <> "0" Then
                subalterno = partRow("subalterno")
            End If

            Dim testo = "(" & prov & ") " & provincia & " - (" & com & ") " & comune & ":" & sezione & ":" & foglio & ":" & numero & ":" & subalterno
            Dim valore = prov & "_" & com & "_" & sezione & "_" & foglio & "_" & numero & "_" & subalterno

            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            Dim CodiceAnagrafeDes = objCodiceAnagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                                                                    CInt(id_Cod), objParametri_Server)

            Dim particella_progetto = New ParticelleCatastali(New ParticelleCatastali.PK(prov, com, sezione, foglio, numero, subalterno))
            Dim codiceAnagrafe As New CodiciAnagrafeValori()

            codiceAnagrafe.codiceAnagrafe = New CodiceAnagrafe(id_Cod) With {.descrizione = CodiceAnagrafeDes}
            codiceAnagrafe.valore = valore

            Dim catastoEsercizio = New CatastoEsercizio(particella_progetto, codiceAnagrafe)

            catastoEsercizio.particella = particella_progetto

            catastoEsercizio.codice = codiceAnagrafe

            particelleList.Add(catastoEsercizio)

        Next

        Return particelleList
    End Function

End Class


'#########################################################################################################
'#########################################################################################################
'#########################################################################################################
'#########################################################################################################


Public Class Progetto_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Impresa_Progetto_Scrivi(ByVal DatiImpresa_Progetti As String,
                                            ByRef OUTPUT_Piva As String,
                                            ByRef OUTPUT_Progetto_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                            Optional ByVal TipoG2G As Integer = 0,
                                            Optional NoteLog As String = "") As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Progetto_W.Impresa_Progetto_Scrivi()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False


        Dim xmlDoc As XmlDocument

        Dim objSequenze As AgronicaCoreDataProvider.Agro_Sequenze
        Dim objImpresaProgetti As AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
        Dim objImpresaProgettoFasi As AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_W
        Dim objPrezzoUnitario As AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objCodici As AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objParticelle As AgronicaCoreAnagrafeDAL.ProgettixParticelle_W
        Dim objRegImpProgrammazione As AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
        Dim objAgronicaLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W

        Dim dummy As Object
        Dim codProgetto As Long
        Dim codFase As Long
        Dim P_Ha As Decimal
        Dim resLog As Boolean = False

        Dim xDatiImpresa_Progetti As XmlNodeList
        Dim xDatiImpresa_Progetto As XmlElement
        Dim xImpresa_Progetti As XmlNodeList
        Dim xImpresa_Progetto As XmlElement
        Dim xImpresa_Progetto_Fasi As XmlNodeList
        Dim xProgetto_Fase As XmlElement
        Dim xPrezzi_Unitari As XmlNodeList
        Dim xPrezzo_Unitario As XmlElement
        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement
        Dim xParticelle As XmlNodeList
        Dim xParticella As XmlElement

        Dim i_DatiImpresa_Progetti As Integer
        Dim i_Impresa_Progetto As Integer
        Dim i_Progetto_Fase As Integer
        Dim i_Prezzo_Unitario As Integer
        Dim i_Codice As Integer
        Dim i_Particella As Integer

        Dim OpeDB_Impresa_Progetto As String
        Dim OpeDB_Progetto_Fase As String
        Dim OpeDB_Prezzo_Unitario As String
        Dim OpeDB_Codice As String
        Dim OpeDB_Particella As String

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale,
                                                                                    flagTransazioneLocale,
                                                                                    objParametri)


            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(DatiImpresa_Progetti)

            xDatiImpresa_Progetti = xmlDoc.GetElementsByTagName("DatiProgetto")

            i_DatiImpresa_Progetti = 0

            Do While i_DatiImpresa_Progetti < xDatiImpresa_Progetti.Count

                'Prelevo l'i-esimo blocco di DatiImpresa_Progetti (in realtà ne esiste uno solo)
                xDatiImpresa_Progetto = xDatiImpresa_Progetti.Item(i_DatiImpresa_Progetti)

                '------------------------------
                xImpresa_Progetti = xDatiImpresa_Progetto.GetElementsByTagName("Progetto")

                i_Impresa_Progetto = 0

                Do While i_Impresa_Progetto < xImpresa_Progetti.Count

                    Dim Impresa_Progetto_data_creazione As DateTime = #2/1/1900#
                    Dim Impresa_Progetto_data_Modifica As DateTime = #2/1/1900#
                    Dim Impresa_Progetto_username_creazione As String = ""
                    Dim Impresa_Progetto_username_modifica As String = ""


                    'Prelevo l' i-esima Codifica Impresa_Progetto
                    xImpresa_Progetto = xImpresa_Progetti.Item(i_Impresa_Progetto)

                    'Prelevo gli attributi del Progetto_Fase selezionato
                    OpeDB_Impresa_Progetto = xImpresa_Progetto.GetAttribute("TipoOperazioneDB")

                    objImpresaProgetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W

                    'Inizializzo Preventivamente il Cod_Progetto
                    codProgetto = CStr(xImpresa_Progetto.GetAttribute("progetto_cod"))

                    If xImpresa_Progetto.HasAttribute("p_ha") Then
                        P_Ha = CDbl(xImpresa_Progetto.GetAttribute("p_ha"))
                    Else
                        P_Ha = 0
                    End If

                    Dim appoggioOpz As Integer = -1
                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Impresa_Progetto

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Piva = CStr(xImpresa_Progetto.GetAttribute("piva"))
                            OUTPUT_Progetto_Cod = CInt(xImpresa_Progetto.GetAttribute("progetto_cod"))

                            '
                        Case "1"    'SALVA -------------------------------------------------------
                            '

                            If codProgetto <= 0 Then

                                objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                'Richiedo un nuovo codice Progetto
                                codProgetto = objSequenze.NuovoId_Tabella("Impresa_Progetto",
                                                                          CInt(xImpresa_Progetto.GetAttribute("basecode")),
                                                                          CInt(xImpresa_Progetto.GetAttribute("topcode")),
                                                                          objParametri)

                                objSequenze = Nothing

                                OUTPUT_Piva = CStr(xImpresa_Progetto.GetAttribute("piva"))
                                OUTPUT_Progetto_Cod = codProgetto

                            Else

                                'Esportazione in Locale

                                OUTPUT_Piva = CStr(xImpresa_Progetto.GetAttribute("piva"))
                                OUTPUT_Progetto_Cod = CInt(xImpresa_Progetto.GetAttribute("progetto_cod"))

                            End If

                            If Not IsNothing(xImpresa_Progetto.GetAttribute("data_creazione")) AndAlso
                               xImpresa_Progetto.GetAttribute("data_creazione") <> "" Then
                                Impresa_Progetto_data_creazione = CDate(xImpresa_Progetto.GetAttribute("data_creazione"))
                            End If

                            If Not IsNothing(xImpresa_Progetto.GetAttribute("data_modifica")) AndAlso
                               xImpresa_Progetto.GetAttribute("data_modifica") <> "" Then
                                Impresa_Progetto_data_Modifica = CDate(xImpresa_Progetto.GetAttribute("data_modifica"))
                            End If

                            If Not IsNothing(xImpresa_Progetto.GetAttribute("username_creazione")) Then
                                Impresa_Progetto_username_creazione = CStr(xImpresa_Progetto.GetAttribute("username_creazione"))
                            End If

                            If Not IsNothing(xImpresa_Progetto.GetAttribute("username_modifica")) Then
                                Impresa_Progetto_username_modifica = CStr(xImpresa_Progetto.GetAttribute("username_modifica"))
                            End If


                            dummy = objImpresaProgetti.Scrivi(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                              CInt(codProgetto),
                                                              CStr(xImpresa_Progetto.GetAttribute("progetto_nome")),
                                                              CStr(xImpresa_Progetto.GetAttribute("progetto_des")),
                                                              CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                              CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                              CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "veg_cod", 0),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "grfi_cod", 0),
                                                              CStr(xImpresa_Progetto.GetAttribute("cau_progetto")),
                                                              CInt(xImpresa_Progetto.GetAttribute("cod_conto")),
                                                              CInt(xImpresa_Progetto.GetAttribute("cod_contratto")),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "csprogetto_cod", 0),
                                                              CStr(xImpresa_Progetto.GetAttribute("giudizio")),
                                                              CDate(xImpresa_Progetto.GetAttribute("data_inizio_prevista")),
                                                              Agro_XML_GetDate(xImpresa_Progetto, "data_fioritura_prevista", AGRODATAINIZIO),
                                                              CDate(xImpresa_Progetto.GetAttribute("data_fine_prevista")),
                                                              CDbl(xImpresa_Progetto.GetAttribute("ricavi_previsti")),
                                                              CDbl(xImpresa_Progetto.GetAttribute("produzione_prevista")),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "stato_impianto", enum_Stato_Impianto.Impianto_Produzione),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "regolamento_cod", 1),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "disciplinare_cod", 0),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "disciplinare_pubblicoprivato", 0),
                                                              Agro_XML_GetInteger(xImpresa_Progetto, "regolamento_concimazioni_cod", 0),
                                                              P_Ha,
                                                              Agro_XML_GetDecimal(xImpresa_Progetto, "P_HA_Femmine", 0),' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                                                              Agro_XML_GetDecimal(xImpresa_Progetto, "P_HA_Maschi", 0),'- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                                                              CDate(xImpresa_Progetto.GetAttribute("validita_inizio")),
                                                              CDate(xImpresa_Progetto.GetAttribute("validita_fine")),
                                                              objParametri,
                                                              Impresa_Progetto_data_creazione,
                                                              Impresa_Progetto_data_Modifica,
                                                              Impresa_Progetto_username_creazione,
                                                              Impresa_Progetto_username_modifica,
                                                              Sup_Prog:=Agro_XML_GetDecimal(xImpresa_Progetto, "sup_prog", 0),
                                                              FlagSecondoRaccolto:=Agro_XML_GetInteger(xImpresa_Progetto, "flagsecondoraccolto", 0),
                                                              Mat_Cod:=Agro_XML_GetInteger(xImpresa_Progetto, "mat_cod", 0))

                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Impresa_Progetto),
                                                                    enum_TipoEntita_Des.Progetti,
                                                                    CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                    CStr(codProgetto),
                                                                    CStr(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                    CStr(xImpresa_Progetto.GetAttribute("appezza")),
                                                                    CStr(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                    Nothing,
                                                                    NoteLog, CInt(idServizio), objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Piva = CStr(xImpresa_Progetto.GetAttribute("piva"))
                            OUTPUT_Progetto_Cod = CInt(xImpresa_Progetto.GetAttribute("progetto_cod"))

                            dummy = objImpresaProgetti.Modifica(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                CInt(codProgetto),
                                                                CStr(xImpresa_Progetto.GetAttribute("progetto_nome")),
                                                                CStr(xImpresa_Progetto.GetAttribute("progetto_des")),
                                                                CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                                CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "veg_cod", 0),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "grfi_cod", 0),
                                                                CStr(xImpresa_Progetto.GetAttribute("cau_progetto")),
                                                                CInt(xImpresa_Progetto.GetAttribute("cod_conto")),
                                                                CInt(xImpresa_Progetto.GetAttribute("cod_contratto")),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "csprogetto_cod", 0),
                                                                CStr(xImpresa_Progetto.GetAttribute("giudizio")),
                                                                CDate(xImpresa_Progetto.GetAttribute("data_inizio_prevista")),
                                                                Agro_XML_GetDate(xImpresa_Progetto, "data_fioritura_prevista", AGRODATAINIZIO),
                                                                CDate(xImpresa_Progetto.GetAttribute("data_fine_prevista")),
                                                                CDbl(xImpresa_Progetto.GetAttribute("ricavi_previsti")),
                                                                CDbl(xImpresa_Progetto.GetAttribute("produzione_prevista")),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "stato_impianto", enum_Stato_Impianto.Impianto_Produzione),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "regolamento_cod", 1),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "disciplinare_cod", 0),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "disciplinare_pubblicoprivato", 0),
                                                                Agro_XML_GetInteger(xImpresa_Progetto, "regolamento_concimazioni_cod", 0),
                                                                P_Ha,
                                                                Agro_XML_GetDecimal(xImpresa_Progetto, "P_HA_Femmine", 0),' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                                                                Agro_XML_GetDecimal(xImpresa_Progetto, "P_HA_Maschi", 0),'- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                                                                CDate(xImpresa_Progetto.GetAttribute("validita_inizio")),
                                                                CDate(xImpresa_Progetto.GetAttribute("validita_fine")),
                                                                "",
                                                                objParametri,
                                                                Sup_Prog:=If(xImpresa_Progetto.HasAttribute("sup_prog") AndAlso IsNumeric(xImpresa_Progetto.GetAttribute("sup_prog")), Agro_XML_GetDecimal(xImpresa_Progetto, "sup_prog", 0), Nothing),
                                                                FlagSecondoRaccolto:=If(xImpresa_Progetto.HasAttribute("flagsecondoraccolto") AndAlso IsNumeric(xImpresa_Progetto.GetAttribute("flagsecondoraccolto")), Agro_XML_GetInteger(xImpresa_Progetto, "flagsecondoraccolto", 0), Nothing)
                                                                )

                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Impresa_Progetto),
                                                                    enum_TipoEntita_Des.Progetti,
                                                                    CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                    CStr(codProgetto),
                                                                    CStr(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                    CStr(xImpresa_Progetto.GetAttribute("appezza")),
                                                                    CStr(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                    Nothing,
                                                                    NoteLog, CInt(idServizio), objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************


                            'Cancellazione Preventiva di tutte le fasi progettuali!
                            'Questo per facilitarne la gestione all'interno del programma

                            objImpresaProgettoFasi = New AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_W

                            objImpresaProgettoFasi.Cancella(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                            CInt(codProgetto),
                                                            0,
                                                            "",
                                                            objParametri)
                            appoggioOpz = 1

                            objImpresaProgettoFasi = Nothing

                        Case "3" 'CANCELLAZIONE  ------------------------------------------------------

                            Dim rifProgetto = New RifEsercizio(
                                CStr(xImpresa_Progetto.GetAttribute("piva")),
                                CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                CInt(xImpresa_Progetto.GetAttribute("progetto_cod"))
                            )

                            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

                            'DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
                            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
                            Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
                            Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0,
                                                                                          rifProgetto.partitaIva,
                                                                                          rifProgetto.saCod,
                                                                                          rifProgetto.appezza,
                                                                                          rifProgetto.idReg,
                                                                                          rifProgetto.progettoCod,
                                                                                          0, 0, 0, "", "", objParametri)

                            For Each vincolo In dtAnagrafe_Vincoli.Rows
                                objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
                                objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
                            Next


                            Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
                            Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

                            Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0,
                                                                                         0,
                                                                                         rifProgetto.partitaIva,
                                                                                         rifProgetto.saCod,
                                                                                         rifProgetto.appezza,
                                                                                         rifProgetto.idReg,
                                                                                         rifProgetto.progettoCod,
                                                                                         "", "", objParametri)

                            For Each pualet In dtPUA_Letamazioni.Rows
                                objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
                                objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
                            Next


                            OUTPUT_Piva = rifProgetto.partitaIva
                            OUTPUT_Progetto_Cod = rifProgetto.progettoCod

                            objImpresaProgetti.Cancella(rifProgetto.partitaIva,
                                                        0,
                                                        0,
                                                        0,
                                                        CInt(codProgetto),
                                                        CInt(xImpresa_Progetto.GetAttribute("cod_contratto")),
                                                        objParametri)

                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Impresa_Progetto),
                                                                    enum_TipoEntita_Des.Progetti,
                                                                    rifProgetto.partitaIva,
                                                                    CStr(rifProgetto.progettoCod),
                                                                    CStr(rifProgetto.saCod),
                                                                    CStr(rifProgetto.appezza),
                                                                    CStr(rifProgetto.idReg),
                                                                    Nothing,
                                                                    "", CInt(idServizio), objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************


                    End Select

                    '##################################################
                    '#############  FASI PROGETTUALI  #################
                    '##################################################

                    objImpresaProgettoFasi = New AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_W

                    'Prelevo l'elenco degli Impresa_Progetto_Fasi
                    xImpresa_Progetto_Fasi = xImpresa_Progetto.GetElementsByTagName("Progetto_Fase")

                    i_Progetto_Fase = 0

                    Do While i_Progetto_Fase < xImpresa_Progetto_Fasi.Count

                        'Prelevo l'i-esimo Progetto_Fase
                        xProgetto_Fase = xImpresa_Progetto_Fasi.Item(i_Progetto_Fase)

                        'Prelevo gli attributi del Progetto_Fase selezionato
                        OpeDB_Progetto_Fase = xProgetto_Fase.GetAttribute("TipoOperazioneDB")
                        If appoggioOpz <> -1 Then
                            OpeDB_Progetto_Fase = appoggioOpz
                        End If

                        codFase = CInt(xProgetto_Fase.GetAttribute("fase_cod"))

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Progetto_Fase

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------
                                '

                                If codFase <= 0 Then

                                    objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
                                    ' CreateObject("Agro_Anagrafe_AD.Agro_Sequenze")

                                    'Richiedo un nuovo codice Progetto_Fase
                                    codFase = objSequenze.NuovoId_Tabella("Impresa_Progetto_Fasi",
                                                                          CInt(xProgetto_Fase.GetAttribute("basecode")),
                                                                          CInt(xProgetto_Fase.GetAttribute("topcode")),
                                                                          objParametri)

                                    objSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If

                                'Salvo l'Progetto_Fase
                                dummy = objImpresaProgettoFasi.Scrivi(CStr(xProgetto_Fase.GetAttribute("piva")),
                                                                      CInt(codProgetto),
                                                                      CInt(codFase),
                                                                      CStr(xProgetto_Fase.GetAttribute("fase_des")),
                                                                      CDate(xProgetto_Fase.GetAttribute("data_inizio_prevista")),
                                                                      CDate(xProgetto_Fase.GetAttribute("data_fine_prevista")),
                                                                      CStr(xProgetto_Fase.GetAttribute("giudizio")),
                                                                      CDbl(xProgetto_Fase.GetAttribute("budget")),
                                                                      CInt(xProgetto_Fase.GetAttribute("lav_cod")),
                                                                      CInt(xProgetto_Fase.GetAttribute("gru_op")),
                                                                      CDate(xProgetto_Fase.GetAttribute("validita_inizio")),
                                                                      CDate(xProgetto_Fase.GetAttribute("validita_fine")),
                                                                      objParametri,
                                                                      Impresa_Progetto_data_creazione,
                                                                      Impresa_Progetto_data_Modifica,
                                                                      Impresa_Progetto_username_creazione,
                                                                      Impresa_Progetto_username_modifica)

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objImpresaProgettoFasi.Modifica(CStr(xProgetto_Fase.GetAttribute("piva")),
                                                                CInt(codProgetto),
                                                                CInt(codFase),
                                                                CStr(xProgetto_Fase.GetAttribute("fase_des")),
                                                                CDate(xProgetto_Fase.GetAttribute("data_inizio_prevista")),
                                                                CDate(xProgetto_Fase.GetAttribute("data_fine_prevista")),
                                                                CStr(xProgetto_Fase.GetAttribute("giudizio")),
                                                                CDbl(xProgetto_Fase.GetAttribute("budget")),
                                                                CInt(xProgetto_Fase.GetAttribute("lav_cod")),
                                                                CInt(xProgetto_Fase.GetAttribute("gru_op")),
                                                                CDate(xProgetto_Fase.GetAttribute("validita_inizio")),
                                                                CDate(xProgetto_Fase.GetAttribute("validita_fine")),
                                                                "",
                                                                objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objImpresaProgettoFasi.Cancella(CStr(xProgetto_Fase.GetAttribute("piva")),
                                                                CInt(codProgetto),
                                                                CInt(xProgetto_Fase.GetAttribute("fase_cod")),
                                                                "",
                                                                objParametri)

                        End Select

                        'Incremento l'indice
                        i_Progetto_Fase += 1

                    Loop

                    objImpresaProgettoFasi = Nothing


                    '##################################################
                    '################  CODICI PROGETTO  ###############
                    '##################################################

                    objCodici = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

                    ' se G2G cancello eventuali codici presenti
                    If TipoG2G <> 0 AndAlso OpeDB_Impresa_Progetto = 2 Then
                        objCodici.CancellaxProgetto(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                    CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                    CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                    CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                    codProgetto, 0, "", objParametri)
                    End If

                    xCodici = xImpresa_Progetto.GetElementsByTagName("CodiceImpianto")

                    i_Codice = 0

                    Do While i_Codice < xCodici.Count

                        'Prelevo l' i-esimo Codice
                        xCodice = xCodici.Item(i_Codice)

                        'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                        OpeDB_Codice = If(TipoG2G = 0, xCodice.GetAttribute("TipoOperazioneDB"), "1")

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Codice

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------

                                dummy = objCodici.ScrivixProgetto(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                  CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                  CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                                  CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                  codProgetto,
                                                                  CInt(xCodice.GetAttribute("id_cod")),
                                                                  CStr(xCodice.GetAttribute("val_cod")),
                                                                  CDate(xCodice.GetAttribute("validita_inizio")),
                                                                  CDate(xCodice.GetAttribute("validita_fine")),
                                                                  objParametri,
                                                                  Impresa_Progetto_data_creazione,
                                                                  Impresa_Progetto_data_Modifica,
                                                                  Impresa_Progetto_username_creazione,
                                                                  Impresa_Progetto_username_modifica)

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objCodici.ModificaxProgetto(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                            CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                            CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                            CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                            codProgetto,
                                                            CInt(xCodice.GetAttribute("id_cod")),
                                                            CStr(xCodice.GetAttribute("val_cod")),
                                                            CDate(xCodice.GetAttribute("validita_inizio")),
                                                            CDate(xCodice.GetAttribute("validita_fine")),
                                                            "",
                                                            objParametri)


                            Case "3"    'ELIMINA -------------------------------------------------------

                                objCodici.CancellaxProgetto(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                            CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                            CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                            CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                            codProgetto,
                                                            CInt(xCodice.GetAttribute("id_cod")),
                                                            "",
                                                            objParametri)

                        End Select

                        'Incremento l'indice
                        i_Codice += 1

                    Loop

                    objCodici = Nothing

                    '##################################################
                    '################  PROGETTO PARTICELLE ############
                    '##################################################

                    objParticelle = New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W

                    xParticelle = xImpresa_Progetto.GetElementsByTagName("ParticellaxProgetto")

                    i_Particella = 0

                    Do While i_Particella < xParticelle.Count

                        'Prelevo l' i-esimo Codice
                        xParticella = xParticelle.Item(i_Particella)

                        'Prelevo gli attributi del codice selezionato
                        OpeDB_Particella = xParticella.GetAttribute("TipoOperazioneDB")


                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Particella

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------
                                '

                                dummy = objParticelle.Scrivi(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                             CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                             CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                             CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                             codProgetto,
                                                             CStr(xParticella.GetAttribute("prov")),
                                                             CStr(xParticella.GetAttribute("com")),
                                                             CStr(xParticella.GetAttribute("sezione")),
                                                             CInt(xParticella.GetAttribute("foglio")),
                                                             CInt(xParticella.GetAttribute("numero")),
                                                             CStr(xParticella.GetAttribute("subalterno")),
                                                             CInt(xParticella.GetAttribute("id_cod")),
                                                             CStr(xParticella.GetAttribute("val_cod")),
                                                             CDate(xParticella.GetAttribute("validita_inizio")),
                                                             CDate(xParticella.GetAttribute("validita_fine")),
                                                             objParametri,
                                                             Impresa_Progetto_data_creazione,
                                                             Impresa_Progetto_data_Modifica,
                                                             Impresa_Progetto_username_creazione,
                                                             Impresa_Progetto_username_modifica)

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objParticelle.Modifica(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                       CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                       CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                       CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                       codProgetto,
                                                       CStr(xParticella.GetAttribute("prov")),
                                                       CStr(xParticella.GetAttribute("com")),
                                                       CStr(xParticella.GetAttribute("sezione")),
                                                       CInt(xParticella.GetAttribute("foglio")),
                                                       CInt(xParticella.GetAttribute("numero")),
                                                       CStr(xParticella.GetAttribute("subalterno")),
                                                       CInt(xParticella.GetAttribute("id_cod")),
                                                       CStr(xParticella.GetAttribute("val_cod")),
                                                       CDate(xParticella.GetAttribute("validita_inizio")),
                                                       CDate(xParticella.GetAttribute("validita_fine")),
                                                       "",
                                                       objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objParticelle.CancellaxProgetto(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                                CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                codProgetto,
                                                                CStr(xParticella.GetAttribute("prov")),
                                                                CStr(xParticella.GetAttribute("com")),
                                                                CStr(xParticella.GetAttribute("sezione")),
                                                                CInt(xParticella.GetAttribute("foglio")),
                                                                CInt(xParticella.GetAttribute("numero")),
                                                                CStr(xParticella.GetAttribute("subalterno")),
                                                                "",
                                                                objParametri)

                        End Select

                        'Incremento l'indice
                        i_Particella += 1

                    Loop

                    objParticelle = Nothing


                    '##########################################################
                    '################  Reg_Impianti_Programmazioni ############
                    '##########################################################
                    '04/07/2019 Grilli - Nel G2G questa tabella è gestita nel Planning
                    If TipoG2G = 0 Then

                        Dim xProgrammazione As XmlElement

                        xProgrammazione = xImpresa_Progetto.SelectSingleNode("Programmazione")

                        If Not IsNothing(xProgrammazione) Then

                            Dim OpeDB_Programmazione As String

                            objRegImpProgrammazione = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W

                            OpeDB_Programmazione = xProgrammazione.GetAttribute("TipoOperazioneDB")

                            Select Case OpeDB_Programmazione
                            '
                                Case "0"    'LEGGI -------------------------------------------------------
                                '
                                Case "1"    'SALVA -------------------------------------------------------

                                    dummy = objRegImpProgrammazione.Scrivi(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                           CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                           CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                                           CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                           codProgetto,
                                                                           CInt(xProgrammazione.GetAttribute("programmazione_cod")),
                                                                           CStr(xProgrammazione.GetAttribute("programmazione_entita_cod")),
                                                                           CDate(xProgrammazione.GetAttribute("validita_inizio")),
                                                                           CDate(xProgrammazione.GetAttribute("validita_fine")),
                                                                           objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objRegImpProgrammazione.Modifica(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                     CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                     CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                                     CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                     codProgetto,
                                                                     CInt(xProgrammazione.GetAttribute("programmazione_cod")),
                                                                     CStr(xProgrammazione.GetAttribute("programmazione_entita_cod")),
                                                                     CDate(xProgrammazione.GetAttribute("validita_inizio")),
                                                                     CDate(xProgrammazione.GetAttribute("validita_fine")),
                                                                     "",
                                                                     objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objRegImpProgrammazione.Cancella(CStr(xImpresa_Progetto.GetAttribute("piva")),
                                                                     CInt(xImpresa_Progetto.GetAttribute("sa_cod")),
                                                                     CInt(xImpresa_Progetto.GetAttribute("appezza")),
                                                                     CInt(xImpresa_Progetto.GetAttribute("id_reg")),
                                                                     codProgetto,
                                                                     CInt(xProgrammazione.GetAttribute("programmazione_cod")),
                                                                     CStr(xProgrammazione.GetAttribute("programmazione_entita_cod")),
                                                                     "",
                                                                     objParametri)

                            End Select

                            objRegImpProgrammazione = Nothing

                        End If

                    End If

                    '##################################################
                    '#############  PREZZO UNITARIO LAVORATI  #########
                    '##################################################

                    objPrezzoUnitario = New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                    'Prelevo il prezzo unitario del lavorato
                    xPrezzi_Unitari = xImpresa_Progetto.GetElementsByTagName("Prezzo_Unitario")

                    i_Prezzo_Unitario = 0

                    Do While i_Prezzo_Unitario < xPrezzi_Unitari.Count

                        'Prelevo l'i-esimo Prezzo Unitario
                        xPrezzo_Unitario = xPrezzi_Unitari.Item(i_Prezzo_Unitario)

                        'Prelevo gli attributi del Lavorato selezionato
                        OpeDB_Prezzo_Unitario = xPrezzo_Unitario.GetAttribute("TipoOperazioneDB")


                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Prezzo_Unitario

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------
                                '
                            Case "2"    'MODIFICA ----------------------------------------------------
                                '
                                objPrezzoUnitario.Modifica_Prezzo_Unitario(CInt(xPrezzo_Unitario.GetAttribute("id_agenda")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("id_mov")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("id_mov_det")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("elem_cod")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("pro_cod")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("mat_cod")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("udm_cod")),
                                                                           CDbl(xPrezzo_Unitario.GetAttribute("prezzo_unitario")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("cal_cod")),
                                                                           CInt(xPrezzo_Unitario.GetAttribute("cod_progetto")),
                                                                           Agro_SQL_SaveNum(xPrezzo_Unitario.GetAttribute("fase_cod"), False),
                                                                           CStr(xPrezzo_Unitario.GetAttribute("lotto")),
                                                                           "",
                                                                           objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                'La Cancellazione Avviene al Momento della Cancellazione dell'impianto...

                        End Select

                        'Incremento l'indice
                        i_Prezzo_Unitario += 1

                    Loop

                    objPrezzoUnitario = Nothing
                    objImpresaProgetti = Nothing

                    'Incremento l'indice
                    i_Impresa_Progetto += 1

                Loop


                '------------------------------

                'Incremento l'indice
                i_DatiImpresa_Progetti += 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiImpresa_Progetti = Nothing
            xDatiImpresa_Progetto = Nothing
            xImpresa_Progetti = Nothing
            xImpresa_Progetto = Nothing
            xmlDoc = Nothing

            xRisp = True

            'Faccio il commit
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametri)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Progetto_Cod=" & CStr(OUTPUT_Progetto_Cod) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

    Public Sub Verifica_ValiditaInizioFine(ByRef esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByVal allEsercizi_ControlliMovimenti As List(Of Esercizio) = Nothing)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "Progetto.vb\Verifica_ValiditaInizioFine()"

        Dim Validita_Fine = esercizio.validita.fine
        Dim Validita_Inizio = esercizio.validita.inizio

        Dim sa_cod = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice 'PrimaryKey.esercizio.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = esercizio.impiantoPK.appezzamentoPK.codice
        Dim id_reg = esercizio.impiantoPK.codice
        Dim progetto_cod = esercizio.codice
        Dim progetto_nome = esercizio.lotto

        Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim impianto = objImpiantoR.LeggiImpianto_ControlliAnagrafica(piva, sa_cod, appezza, id_reg, objParametri_Server)

        Dim objEsercizioR As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim ese = objEsercizioR.LeggiDistinta_ControlliAnagrafica(piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)

        Dim nomeCurrent As String = ""
        If impianto.Rows.Count = 1 Then
            nomeCurrent = getCurrentDescrizione(impianto.Rows(0), progetto_nome, esercizio.validita.inizio, esercizio.validita.inizio)
        End If

        Try
            If ese.Rows.Count > 0 Then
                For Each esercizio_db In ese.Rows
                    If Validita_Inizio <> esercizio_db("Validita_Inizio") OrElse Validita_Fine <> esercizio_db("Validita_Fine") Then

                        Dim nomeCurrentEse As String = getCurrentDescrizione(esercizio_db, esercizio_db("Lotto"), CDate(esercizio_db("Validita_Inizio")), CDate(esercizio_db("Validita_Fine")))

                        If Validita_Inizio < CDate(impianto.Rows(0)("Validita_Inizio")) Then
                            MessaggioErrore &= nomeCurrentEse & " " & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.DataInizioEsercizioInferioreImpianto, impianto.Rows(0)("Validita_Inizio"))
                            Throw New GiasException(MessaggioErrore)
                        End If

                        If Validita_Fine > CDate(impianto.Rows(0)("Validita_Fine")) Then
                            MessaggioErrore &= nomeCurrentEse & " " & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.DataFineEsercizioSuperioreImpianto, impianto.Rows(0)("Validita_Fine"))
                            Throw New GiasException(MessaggioErrore)
                        End If

                        'Salto i controlli sugli esercizi nuovi (progetto_cod=0)
                        If progetto_cod <> 0 Then
                            controllo_MovimentiRicettePua(esercizio, piva, sa_cod, appezza, id_reg, nomeCurrentEse, Validita_Inizio, Validita_Fine, objParametri_Server, allEsercizi_ControlliMovimenti:=allEsercizi_ControlliMovimenti)

                            'COSTI DI GESTIONE
                            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                            Dim controllo = objControllo.controllo_CdG(esercizio, piva, sa_cod, appezza, id_reg, progetto_cod, Validita_Inizio, Validita_Fine, objParametri_Server)
                            If controllo.errore Then
                                Dim MessaggioErroreCdG As String = ""
                                If Not controllo.messaggioSpecifico Then
                                    MessaggioErroreCdG &= nomeCurrentEse & " " & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileModificareEsercizioConCdGInDataSuccessiva, controllo.inizio_fine, "")
                                Else
                                    MessaggioErroreCdG &= nomeCurrentEse & " " & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileModificareEsercizioConCdGInDataSpecifica, controllo.inizio_fine, "", controllo.dataCdG.ToShortDateString())
                                End If

                                Throw New GiasException(MessaggioErroreCdG)
                            End If
                        End If
                    End If
                Next
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore &= nomeCurrent & " " & My.Resources.AgronicaCoreAnagrafeBIZ.FineDellaDistintaAntecedenteAInizio
                Throw New GiasException(MessaggioErrore)
            End If

            If Validita_Fine = Validita_Inizio Then
                MessaggioErrore &= nomeCurrent & " " & My.Resources.AgronicaCoreAnagrafeBIZ.SovrapposizioneDateValidita
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Public Function getCurrentDescrizione(DT As DataRow, lottoEsercizio As String, validitaInizio As Date, validitaFine As Date) As String
        Dim descrizioneCurrent As String = ""

        Dim desUtilizzo As String = ""
        If DT.Item("Cul_Cod") <> 0 Then
            desUtilizzo = $"{DT.Item("Veg_Des")} - {DT.Item("Cul_Des")} "
        Else
            desUtilizzo = $"{DT.Item("destinazioneUso")}"
        End If

        Dim desProgetto As String = ""
        If lottoEsercizio <> "" Then
            desProgetto = $" ({Gias.Lotto}: {lottoEsercizio}"
        Else
            If validitaInizio <> AGRODATAINIZIO OrElse validitaFine <> AGRODATAFINE Then
                desProgetto = $" ({validitaInizio.ToShortDateString()}-{validitaFine.ToShortDateString()})"
            End If
        End If

        descrizioneCurrent = $"[{Gias.AppezzamentoAbbr} {DT.Item("App_Nome")} - {desUtilizzo}{desProgetto}]: "

        Return descrizioneCurrent
    End Function
    'ANNA
    Public Function controllo_CdG(DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                  piva As String,
                                  sa_cod As Integer,
                                  appezza As Integer,
                                  id_reg As Integer,
                                  progetto_cod As Integer,
                                  Validita_Inizio As Date,
                                  Validita_Fine As Date,
                                  ByRef objParametri As AgronicaCoreParametri,
                                    Optional Id_Budget As Integer = 0
                                  ) As messaggioErroreCdG

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "AgronicaCoreAnagrafeBIZ.Progetto_R.controllo_CdG()"

        Dim objMessaggio As New messaggioErroreCdG
        Dim objCdG As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim dtCdG As New DataTable

        'If DatiEsercizio IsNot Nothing Then
        '    If DatiEsercizio.lotto <> "" Then
        '        descrizione_distinta = DatiEsercizio.lotto
        '    Else
        '        descrizione_distinta = "con validità dal " + DatiEsercizio.validita.inizio + " al " + DatiEsercizio.validita.fine
        '    End If
        'End If

        Try
            Dim xFiltroAggiuntivo As String = " ( Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " ) " '& " OR Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) "
            If Id_Budget <> 0 Then
                xFiltroAggiuntivo += " AND Budget = " & Id_Budget
            End If
            dtCdG = objCdG.LeggiCronologia_CdG(piva, sa_cod, appezza, id_reg, progetto_cod,
                                               xFiltroAggiuntivo, "",
                                               objParametri,
                                               joinAttivita:=True)

            If dtCdG.Rows.Count > 0 Then
                objMessaggio.errore = True

                If dtCdG.Rows.Count = 1 Then
                    objMessaggio.messaggioSpecifico = True
                    objMessaggio.dataCdG = dtCdG.Rows(0).Item("Data_Movimento")
                End If

                If dtCdG.Rows(0).Item("Data_Movimento") > Validita_Fine Then
                    objMessaggio.inizio_fine = "data fine"
                End If

                If dtCdG.Rows(0).Item("Data_Movimento") < Validita_Inizio Then
                    objMessaggio.inizio_fine = "data inizio"
                End If
            End If

            dtCdG.Dispose()
            dtCdG = Nothing
        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return objMessaggio
    End Function

    Public Function controllo_CdGxEliminazione(DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                               piva As String,
                                               sa_cod As Integer,
                                               appezza As Integer,
                                               id_reg As Integer,
                                               progetto_cod As Integer,
                                               ByRef objParametri As AgronicaCoreParametri,
                                                Optional Id_Budget As Integer = 0
                                               ) As messaggioErroreCdG

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "AgronicaCoreAnagrafeBIZ.Progetto_R.controllo_CdGxEliminazione()"

        Dim objMessaggio As New messaggioErroreCdG
        Dim objCdG As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim dtCdG As New DataTable

        'If DatiEsercizio IsNot Nothing Then
        '    If DatiEsercizio.lotto <> "" Then
        '        objMessaggio.descrizioneDistinta = DatiEsercizio.lotto
        '    Else
        '        objMessaggio.descrizioneDistinta = "con validità dal " + DatiEsercizio.validita.inizio + " al " + DatiEsercizio.validita.fine
        '    End If
        'End If

        Try
            Dim xFiltroAggiuntivo As String = ""
            If Id_Budget <> 0 Then
                xFiltroAggiuntivo += " Budget = " & Id_Budget
            End If

            dtCdG = objCdG.LeggiCronologia_CdG(piva, sa_cod, appezza, id_reg, progetto_cod,
                                               xFiltroAggiuntivo, "", objParametri)

            If dtCdG.Rows.Count > 0 Then
                objMessaggio.errore = True

                If dtCdG.Rows.Count = 1 Then
                    objMessaggio.messaggioSpecifico = True
                    objMessaggio.dataCdG = dtCdG.Rows(0).Item("Data_Movimento")
                End If
            End If

            dtCdG.Dispose()
            dtCdG = Nothing
        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return objMessaggio

    End Function

    Public Function controllo_MovimentiRicettePua(DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                  piva As String,
                                                  sa_cod As Integer,
                                                  appezza As Integer,
                                                  id_reg As Integer,
                                                  progetto_nome As String,
                                                  Validita_Inizio As Date,
                                                  Validita_Fine As Date,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal allEsercizi_ControlliMovimenti As List(Of Esercizio) = Nothing
                                                  )

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.controllo_MovimentiRicettePua()"
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Try
            dtAgenda = objDestR.LeggiCronologiaMovimenti(piva, sa_cod, appezza, id_reg,
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         " ( Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                                         "",
                                                         objParametri)

            dtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                         piva, sa_cod, appezza, id_reg,
                                         AGRODATAINIZIO, AGRODATAFINE,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         " ( Ricette_Operazioni.Validita_Inizio > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Ricette_Operazioni.Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                         "",
                                         objParametri,
                                         joinRicette:=True)

            Dim almenoUno_Agenda As Boolean = False
            Dim almenoUno_Ricetta As Boolean = False

            'Verifico se esistono altri esercizi che possono coprire le operazioni
            If allEsercizi_ControlliMovimenti IsNot Nothing Then
                If dtAgenda.Rows.Count > 0 Then
                    For Each agenda In dtAgenda.Rows
                        Dim data_movimento As Date = agenda.item("Data_Movimento")
                        almenoUno_Agenda = allEsercizi_ControlliMovimenti.FindAll(Function(esercizio) esercizio.validita.inizio <= data_movimento AndAlso esercizio.validita.fine >= data_movimento).Count > 0

                        If almenoUno_Agenda Then
                            Exit For
                        End If
                    Next
                Else
                    almenoUno_Agenda = True
                End If

                If dtRicette.Rows.Count > 0 Then
                    For Each ricetta In dtRicette.Rows
                        Dim data_movimento As Date = ricetta.item("Data_Ricetta_Operazione")
                        almenoUno_Ricetta = allEsercizi_ControlliMovimenti.FindAll(Function(esercizio) esercizio.validita.inizio <= data_movimento AndAlso esercizio.validita.fine >= data_movimento).Count > 0

                        If almenoUno_Ricetta Then
                            Exit For
                        End If
                    Next
                Else
                    almenoUno_Ricetta = True
                End If

                If Not (almenoUno_Agenda AndAlso almenoUno_Ricetta) Then
                    MessaggioErrore &= (progetto_nome & " " & My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileModificareDataEsercizioRegistrazioniAssociate)

                    Throw New GiasException(MessaggioErrore)

                End If
            End If


            dtAgenda.Dispose()
            dtAgenda = Nothing

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Function controllo_CdG_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                          profonditaJoin As Enum_EntitaModificaMultiplaPianoColturale,
                                          Validita_Inizio As Date,
                                          Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As List(Of (String, Integer, Integer, Integer, Integer))


        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "AgronicaCoreAnagrafeBIZ.Progetto_R.controllo_CdG_Massivo()"

        Dim objMessaggio As New messaggioErroreCdG
        Dim objCdG As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim dtCdG As New DataTable

        Dim listaBloccati As New List(Of (String, Integer, Integer, Integer, Integer))

        Try
            Dim xFiltroAggiuntivo As String = " ( Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " ) " '& " OR Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) "

            dtCdG = objCdG.LeggiCronologia_CdG_Massivo(listChiavi,
                                                       profonditaJoin,
                                                       xFiltroAggiuntivo,
                                                       objParametri)

            If dtCdG.Rows.Count > 0 Then
                For Each row In dtCdG.Rows
                    Dim piva As String = row("piva")
                    Dim sa_cod As String = row("sa_cod")
                    Dim appezza As String = row("appezza")
                    Dim id_reg As String = row("id_destinazione")
                    Dim progetto_cod As String = row("id_destinazione")

                    listaBloccati.Add((piva, sa_cod, appezza, id_reg, progetto_cod))
                Next
            End If

            dtCdG.Dispose()
            dtCdG = Nothing
        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaBloccati
    End Function

End Class

Public Class messaggioErroreCdG

    Public Property messaggioSpecifico As Boolean = False
    Public Property dataCdG As Date
    Public Property errore As Boolean = False
    Public Property descrizioneDistinta As String
    Public Property inizio_fine As String

End Class