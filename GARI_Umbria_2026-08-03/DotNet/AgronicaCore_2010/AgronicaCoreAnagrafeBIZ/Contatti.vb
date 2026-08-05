Imports System.IO
Imports System.Linq
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Contatti_MultiHost_R
    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function Contatto_Leggi(
                            ByVal Piva As String,
                            ByVal Cod_Contatto As String,
                            ByVal Piva_SuperUser_Origine As String,
                            ByVal ForDelete As Boolean,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal TipoG2G As Integer = 0) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R.Contatto_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False 'True
        Dim i As Int32

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument
        Dim XmlDatiContatti As XmlElement
        Dim XmlContatto As XmlElement
        Dim XmlIndirizzo As XmlElement
        Dim XmlRubrica As XmlElement
        Dim XmlCodice As XmlElement
        Dim XmlRapCon As XmlElement
        Dim XmlProdotto_Costo As XmlElement
        Dim Xml_DatiProdotti_Costi As XmlElement
        Dim Xml_Liquidita As XmlElement
        Dim Xml_Conti As XmlElement

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objContattixIndirizzi As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
        Dim objContattixRubrica As New AgronicaCoreAnagrafeDAL.ContattixRubrica_R
        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim objContatti_Codici As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R

        Dim objCorrispettivi As New AgronicaCoreContabDAL.Prodotti_Costi_R
        Dim objLiquidita As New AgronicaCoreContabDAL.Liquidita_R
        Dim objConti As New AgronicaCoreContabDAL.ContixContatti_R

        Dim DTContatti As DataTable
        Dim DTIndirizzi As DataTable
        Dim DTRubrica As DataTable
        Dim DTRapCon As DataTable
        Dim DTCorrispettivi As DataTable
        Dim DtContattixCodici As DataTable
        Dim DTLiquidita As DataTable
        Dim DTConti As DataTable

        '------------------------------

        Try

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


            'Creo l'oggetto COM+
            DTContatti = objContatti.LeggiContattoSpecifico(
                                             CStr(Piva),
                                             CStr(Cod_Contatto),
                                             99,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "",
                                             objParametri,
                                             TipoG2G)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DTContatti.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

                'Effettuo un ciclo 
                For i = 0 To DTContatti.Rows.Count - 1

                    '----- < AGENDA > -----
                    XmlContatto = XmlDoc.CreateElement("Contatto")

                    With XmlContatto
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DTContatti.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("cod_contatto", Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto")))
                        .SetAttribute("referente", Agro_SQL_Load(DTContatti.Rows(i).Item("Referente")))
                        .SetAttribute("rag_soc", Agro_SQL_Load(DTContatti.Rows(i).Item("Rag_Soc")))
                        .SetAttribute("nome", Agro_SQL_Load(DTContatti.Rows(i).Item("Nome")))
                        .SetAttribute("cognome", Agro_SQL_Load(DTContatti.Rows(i).Item("Cognome")))
                        .SetAttribute("data_nascita", Agro_SQL_Load(DTContatti.Rows(i).Item("Data_Nascita")))
                        .SetAttribute("sesso", Agro_SQL_Load(DTContatti.Rows(i).Item("Sesso")))
                        .SetAttribute("id_cf", Agro_SQL_Load(DTContatti.Rows(i).Item("Id_CF")))
                        .SetAttribute("codice_fiscale", Agro_SQL_Load(DTContatti.Rows(i).Item("Codice_Fiscale")))
                        .SetAttribute("convenevoli", Agro_SQL_Load(DTContatti.Rows(i).Item("Convenevoli")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DTContatti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DTContatti.Rows(i).Item("Validita_Fine")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DTContatti.Rows(i).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DTContatti.Rows(i).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DTContatti.Rows(i).Item("username_creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DTContatti.Rows(i).Item("username_modifica")))
                        .SetAttribute("nrBadge", Agro_SQL_Load(DTContatti.Rows(i).Item("nrBadge")))

                        If EsisteColonna(DTContatti, "nome_breve") Then
                            .SetAttribute("nome_breve", Agro_SQL_Load(DTContatti.Rows(i).Item("Nome_Breve")))
                        End If

                        If EsisteColonna(DTContatti, "tipo_destinazione") Then
                            .SetAttribute("tipo_destinazione", Agro_SQL_Load(DTContatti.Rows(i).Item("tipo_destinazione")))
                        End If
                        If EsisteColonna(DTContatti, "Tipo_Speditore") Then
                            .SetAttribute("tipo_speditore", Agro_SQL_Load(DTContatti.Rows(i).Item("Tipo_Speditore")))
                        End If

                        If EsisteColonna(DTContatti, "note") Then
                            .SetAttribute("note", Agro_SQL_Load(DTContatti.Rows(i).Item("note")))
                        End If
                        If EsisteColonna(DTContatti, "note2") Then
                            .SetAttribute("note2", Agro_SQL_Load(DTContatti.Rows(i).Item("note2")))
                        End If
                        If EsisteColonna(DTContatti, "note_operazioni") Then
                            .SetAttribute("note_operazioni", Agro_SQL_Load(DTContatti.Rows(i).Item("note_operazioni")))
                        End If
                        If EsisteColonna(DTContatti, "note2_operazioni") Then
                            .SetAttribute("note2_operazioni", Agro_SQL_Load(DTContatti.Rows(i).Item("note2_operazioni")))
                        End If
                        If EsisteColonna(DTContatti, "sconto_contatto") Then
                            .SetAttribute("sconto_contatto", Agro_SQL_Load(DTContatti.Rows(i).Item("sconto_contatto")))
                        End If
                        If EsisteColonna(DTContatti, "sconto_testo") Then
                            .SetAttribute("sconto_testo", Agro_SQL_Load(DTContatti.Rows(i).Item("sconto_testo")))
                        End If
                        If EsisteColonna(DTContatti, "provvigione") Then
                            .SetAttribute("provvigione", Agro_SQL_Load(DTContatti.Rows(i).Item("provvigione")))
                        End If
                        If EsisteColonna(DTContatti, "Provvigione_CapoArea") Then
                            .SetAttribute("provvigione_capoarea", Agro_SQL_Load(DTContatti.Rows(i).Item("Provvigione_CapoArea")))
                        End If
                        If EsisteColonna(DTContatti, "agente_cod") Then
                            .SetAttribute("agente_cod", Agro_SQL_Load(DTContatti.Rows(i).Item("agente_cod")))
                        End If
                        If EsisteColonna(DTContatti, "capoarea_cod") Then
                            .SetAttribute("capoarea_cod", Agro_SQL_Load(DTContatti.Rows(i).Item("capoarea_cod")))
                        End If
                        If EsisteColonna(DTContatti, "vettore_cod") Then
                            .SetAttribute("vettore_cod", Agro_SQL_Load(DTContatti.Rows(i).Item("vettore_cod")))
                        End If
                        If EsisteColonna(DTContatti, "cod_iva_contatto") Then
                            .SetAttribute("cod_iva_contatto", Agro_SQL_Load(DTContatti.Rows(i).Item("cod_iva_contatto")))
                        End If
                        If EsisteColonna(DTContatti, "cod_conto_econ") Then
                            .SetAttribute("cod_conto_econ", Agro_SQL_Load(DTContatti.Rows(i).Item("cod_conto_econ")))
                        End If
                        If EsisteColonna(DTContatti, "cod_conto_pat") Then
                            .SetAttribute("cod_conto_pat", Agro_SQL_Load(DTContatti.Rows(i).Item("cod_conto_pat")))
                        End If
                        If EsisteColonna(DTContatti, "modalita_fatturazione") Then
                            .SetAttribute("modalita_fatturazione", Agro_SQL_Load(DTContatti.Rows(i).Item("modalita_fatturazione")))
                        End If
                        If EsisteColonna(DTContatti, "documento_fatturazione") Then
                            .SetAttribute("documento_fatturazione", Agro_SQL_Load(DTContatti.Rows(i).Item("documento_fatturazione")))
                        End If
                        If EsisteColonna(DTContatti, "cod_risum_destinazione_diversa") Then
                            .SetAttribute("cod_risum_destinazione_diversa", Agro_SQL_Load(DTContatti.Rows(i).Item("cod_risum_destinazione_diversa")))
                        End If
                        If EsisteColonna(DTContatti, "memo") Then
                            .SetAttribute("memo", Agro_SQL_Load(DTContatti.Rows(i).Item("memo")))
                        End If
                        If EsisteColonna(DTContatti, "Tipo_Indirizzo_Default") Then
                            .SetAttribute("tipo_indirizzo_default", Agro_SQL_Load(DTContatti.Rows(i).Item("Tipo_Indirizzo_Default")))
                        End If
                        If EsisteColonna(DTContatti, "Tipo_Indirizzo_Default_Destinazione_Diversa") Then
                            .SetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"), Agro_SQL_Load(DTContatti.Rows(i).Item("Tipo_Indirizzo_Default_Destinazione_Diversa")))
                        End If
                        If EsisteColonna(DTContatti, "ChkFittizio") Then
                            .SetAttribute(LCase("ChkFittizio"), Agro_SQL_Load(DTContatti.Rows(i).Item("ChkFittizio")))
                        End If
                        If EsisteColonna(DTContatti, "EUDR") Then
                            .SetAttribute(LCase("EUDR"), Agro_SQL_Load(DTContatti.Rows(i).Item("EUDR")))
                        End If
                    End With

                    '#################################
                    '##########  INDIRIZZI  ##########
                    '#################################

                    'Mi procuro un elenco degli indirizzi del contatto
                    DTIndirizzi = objContattixIndirizzi.LeggiContattoSpecifico(
                                                                  DTContatti.Rows(i).Item("PIVA"),
                                                                  CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                                   99,
                                                                     0,
                                                                    0,
                                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                "", "",
                                                                 objParametri)


                    If Not IsNothing(DTIndirizzi) AndAlso DTIndirizzi.Rows.Count > 0 Then
                        Dim j As Integer
                        For j = 0 To DTIndirizzi.Rows.Count - 1
                            '----- < INDIRIZZO > -----
                            XmlIndirizzo = XmlDoc.CreateElement("Indirizzo")

                            With XmlIndirizzo
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("tipo_indirizzo", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Tipo_Indirizzo")))
                                .SetAttribute("cod_indirizzo", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("cod_indirizzo")))
                                .SetAttribute("ind_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("ind_des")))
                                .SetAttribute("frz_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("frz_des")))
                                .SetAttribute("cap", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("CAP")))
                                .SetAttribute("com_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("com_des")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("pro_cod")))
                                .SetAttribute("pro_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("pro_des")))
                                .SetAttribute("stato", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("stato")))
                                .SetAttribute("stato_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("stato_des")))
                                .SetAttribute("note", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("note")))
                                .SetAttribute("pro_cod_istat", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("pro_cod_istat")))
                                .SetAttribute("com_cod_istat", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("com_cod_istat")))
                                .SetAttribute("codice_lingua", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("codice_lingua")))
                                .SetAttribute("lingua_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("lingua_des")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("username_modifica")))
                            End With


                            XmlContatto.AppendChild(XmlIndirizzo)
                            '----- < / INDIRIZZO > -----
                        Next
                    End If

                    '#################################
                    '##########  RUBRICA  ############
                    '#################################

                    'Mi procuro un elenco delle voci di rubrica del Contatto
                    DTRubrica = objContattixRubrica.LeggiContattoSpecifico(
                                                                            DTContatti.Rows(i).Item("PIVA"),
                                                                            CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                                            99,
                                                                            0,
                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                    "",
                                    "",
                                    objParametri)


                    If DTRubrica.Rows.Count > 0 Then
                        'Effettuo un ciclo sugli indirizzi
                        Dim j As Integer
                        For j = 0 To DTRubrica.Rows.Count - 1
                            '----- < RUBRICA > -----
                            XmlRubrica = XmlDoc.CreateElement("Rubrica")

                            With XmlRubrica
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("cod_rubrica", Agro_SQL_Load(DTRubrica.Rows(j).Item("cod_rubrica")))
                                .SetAttribute("numero", Agro_SQL_Load(DTRubrica.Rows(j).Item("numero")))
                                .SetAttribute("descr", Agro_SQL_Load(DTRubrica.Rows(j).Item("descr")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DTRubrica.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DTRubrica.Rows(j).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DTRubrica.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DTRubrica.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DTRubrica.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DTRubrica.Rows(j).Item("username_modifica")))
                            End With

                            XmlContatto.AppendChild(XmlRubrica)
                            '----- < / RUBRICA > -----
                        Next
                    End If


                    '#################################
                    '#######  CODICI ANAGRAFE ########
                    '#################################

                    'Mi procuro un elenco delle voci di rubrica del Contatto
                    DtContattixCodici = objContatti_Codici.Leggi(
                                              DTContatti.Rows(i).Item("PIVA"),
                                              CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                              0, "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "",
                                              objParametri)


                    If DtContattixCodici.Rows.Count > 0 Then
                        'Effettuo un ciclo sugli indirizzi

                        'Do While Not DtContattixCodici.EOF
                        Dim iCxC As Integer
                        For iCxC = 0 To DtContattixCodici.Rows.Count - 1
                            '----- < CODICI ANAGRAFE > -----
                            XmlCodice = XmlDoc.CreateElement("Contatto_Codice")

                            With XmlCodice
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DtContattixCodici.Rows(iCxC).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtContattixCodici.Rows(iCxC).Item("sa_cod")))
                                .SetAttribute("id_cod", Agro_SQL_Load(DtContattixCodici.Rows(iCxC).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(DtContattixCodici.Rows(iCxC).Item("val_cod")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtContattixCodici.Rows(iCxC).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtContattixCodici.Rows(iCxC).Item("validita_fine")))
                            End With


                            XmlContatto.AppendChild(XmlCodice)
                            '----- < / CODICI ANAGRAFE > -----
                        Next iCxC
                    End If

                    '#############################################
                    '###############  LIQUIDITA  #############
                    '#############################################
                    DTLiquidita = objLiquidita.Leggi_Per_Contatto(DTContatti.Rows(i).Item("PIVA"), 0, "", "", "", "", "", "", "",
                                                                enum_Liquidita_CauRisorsa.RisorsaFinanziaria, 0, 0,
                                                                CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                                "", 0, 0, "", "", "", objParametri)
                    If DTLiquidita.Rows.Count > 0 Then
                        Dim il As Integer
                        For il = 0 To DTLiquidita.Rows.Count - 1

                            Xml_Liquidita = XmlDoc.CreateElement("DatiLiquidita")

                            With Xml_Liquidita
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute(LCase("Sa_Cod"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Sa_Cod")))
                                .SetAttribute("piva", Agro_SQL_Load(DTLiquidita.Rows(il).Item("piva")))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute(LCase("Cod_Liquidita"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Cod_Liquidita")))
                                .SetAttribute(LCase("Cod_Istituto"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Cod_Istituto")))

                                .SetAttribute(LCase("Istituto_Des"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Istituto_Des")))
                                .SetAttribute(LCase("Nazione"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Nazione")))
                                .SetAttribute(LCase("Cifre_Controllo"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Cifre_Controllo")))
                                .SetAttribute(LCase("Cin"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Cin")))
                                .SetAttribute(LCase("Abi"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Abi")))
                                .SetAttribute(LCase("Cab"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Cab")))
                                .SetAttribute(LCase("Numero"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Numero")))
                                .SetAttribute(LCase("Bic"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Bic")))
                                .SetAttribute(LCase("ChkAbilitazione"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("ChkAbilitazione")))

                                .SetAttribute(LCase("Abilitazione_Des"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Abilitazione_Des")))
                                .SetAttribute(LCase("Validita_Inizio"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Validita_Inizio")))
                                .SetAttribute(LCase("Validita_Fine"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Validita_Fine")))
                                .SetAttribute(LCase("Note"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("Note")))
                                .SetAttribute(LCase("ChkDefault"), Agro_SQL_Load(DTLiquidita.Rows(il).Item("ChkDefault")))


                            End With

                            XmlContatto.AppendChild(Xml_Liquidita)

                        Next il
                    End If

                    '#############################################
                    '###############  CONTI  #############
                    '#############################################
                    DTConti = objConti.Leggi_Estesa("",
                                                    CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                    DTContatti.Rows(i).Item("PIVA"), "", "",
                                                    objParametri, 2, Now.Year, 0, "")

                    If DTConti.Rows.Count > 0 Then
                        Dim c As Integer
                        For c = 0 To DTConti.Rows.Count - 1

                            Xml_Conti = XmlDoc.CreateElement("DatiConti")

                            With Xml_Conti
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DTConti.Rows(c).Item("piva")))
                                .SetAttribute("Cod_Contatto", Agro_SQL_Load(DTConti.Rows(c).Item("Cod_Contatto")))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("Cod_Conto", Agro_SQL_Load(DTConti.Rows(c).Item("Cod_Conto")))
                                .SetAttribute("ID_Riclassificazione", Agro_SQL_Load(DTConti.Rows(c).Item("ID_Riclassificazione")))
                                .SetAttribute("Conto_Descr", Agro_SQL_Load(DTConti.Rows(c).Item("Conto_Descr")))
                                .SetAttribute("Validita_Inizio", Agro_SQL_Load(DTConti.Rows(c).Item("Validita_Inizio")))
                                .SetAttribute("Validita_Fine", Agro_SQL_Load(DTConti.Rows(c).Item("Validita_Fine")))
                            End With

                            XmlContatto.AppendChild(Xml_Conti)

                        Next c
                    End If


                    '#############################################
                    '###############  RISORSE UMANE  #############
                    '#############################################

                    objParametri.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

                    'Mi procuro il recordset richiesto
                    'DTRapCon = objRisorse_Umane.LeggiContattixSuperUser( _
                    '                        Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")), _
                    '                        0, _
                    '                        CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))), _
                    '                        0, _
                    '                        0, _
                    '                         Piva_SuperUser_Origine, _
                    '                         True, _
                    '                         enumSelezioneVariabile.Selezione_JoinCompleta, _
                    '                        "", "", _
                    '                        objParametri)

                    DTRapCon = objRisorse_Umane.LeggiSoloContatto(Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")),
                                                                  0,
                                                                  CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                                  0, 0, Piva_SuperUser_Origine, True,
                                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                  "", "", objParametri)

                    objParametri.ResettaFinestra()

                    If DTRapCon.Rows.Count > 0 Then

                        'Effettuo un ciclo sui rapporti contabili
                        Dim j As Integer
                        For j = 0 To DTRapCon.Rows.Count - 1

                            '----- < RAPPORTO CONTABILE > -----
                            XmlRapCon = XmlDoc.CreateElement("RapCon")

                            With XmlRapCon
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DTRapCon.Rows(j).Item("Piva")))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("sa_cod", Agro_SQL_Load(DTRapCon.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("cod_risum", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_RisUm")))
                                .SetAttribute("cod_rapporto", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_Rapporto")))
                                .SetAttribute("cod_contatto", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_Contatto")))
                                .SetAttribute("occasionale", Agro_SQL_Load(DTRapCon.Rows(j).Item("Occasionale")))
                                .SetAttribute("corrispettivo_mensile", Agro_SQL_Load(DTRapCon.Rows(j).Item("Corrispettivo_Mensile")))
                                .SetAttribute("corrispettivo_orario", Agro_SQL_Load(DTRapCon.Rows(j).Item("Corrispettivo_Orario")))
                                .SetAttribute("ore_settimanali", Agro_SQL_Load(DTRapCon.Rows(j).Item("Ore_Settimanali")))
                                .SetAttribute("giorni_ferie", Agro_SQL_Load(DTRapCon.Rows(j).Item("Giorni_Ferie")))
                                .SetAttribute("ferie_godute", Agro_SQL_Load(DTRapCon.Rows(j).Item("Ferie_Godute")))
                                .SetAttribute("giorni_malattia", Agro_SQL_Load(DTRapCon.Rows(j).Item("Giorni_Malattia")))
                                .SetAttribute("settore_des", Agro_SQL_Load(DTRapCon.Rows(j).Item("Settore_Des")))
                                .SetAttribute("attivita_des", Agro_SQL_Load(DTRapCon.Rows(j).Item("Attivita_Des")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DTRapCon.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DTRapCon.Rows(j).Item("Validita_Fine")))
                                .SetAttribute("patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Patentino")))
                                .SetAttribute("data_rilascio_patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Rilascio_Patentino")))
                                .SetAttribute("data_scadenza_patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Scadenza_Patentino")))
                                .SetAttribute("ente_di_rilascio", Agro_SQL_Load(DTRapCon.Rows(j).Item("ente_di_rilascio")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DTRapCon.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DTRapCon.Rows(j).Item("username_modifica")))
                            End With

                            If Piva_SuperUser_Origine <> "" Then
                                XmlRapCon.SetAttribute("piva_superuser_origine", CStr(Piva_SuperUser_Origine))
                            End If


                            '#######################################################
                            '##########  PRODOTTI COSTI RAPPORTI CONTABILI  ########
                            '#######################################################

                            '----- < RAPPORTO CONTABILE > -----
                            Xml_DatiProdotti_Costi = XmlDoc.CreateElement("DatiProdotti_Costi")


                            'Mi procuro il recordset richiesto
                            DTCorrispettivi = objCorrispettivi.Leggi(
                                                     DTContatti.Rows(i).Item("PIVA"),
                                                     0,
                                                     "",
                                                     0,
                                                     Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_RisUm")),
                                                     0,
                                                     0,
                                                     0,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "",
                                                     objParametri)


                            'Se ottengo almeno un risultato, creo la struttura XML
                            If DTCorrispettivi.Rows.Count > 0 Then

                                'Effettuo un ciclo
                                Dim jj As Integer
                                For jj = 0 To DTCorrispettivi.Rows.Count - 1
                                    '----- < RAPPORTO CONTABILE > -----
                                    XmlProdotto_Costo = XmlDoc.CreateElement("Prodotto_Costo")

                                    With XmlProdotto_Costo
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("id", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Id")))
                                        .SetAttribute("piva", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Piva")))
                                        .SetAttribute("riferimento", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Riferimento")))
                                        .SetAttribute("elem_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Elem_Cod")))
                                        .SetAttribute("pro_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Pro_Cod")))
                                        .SetAttribute("mat_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Mat_Cod")))
                                        .SetAttribute("udm_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Udm_Cod")))
                                        .SetAttribute("mezzo", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Mezzo")))
                                        .SetAttribute("prezzo_unitario", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Prezzo_Unitario")))
                                        .SetAttribute("veg_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Veg_Cod")))
                                        .SetAttribute("cul_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Cul_Cod")))
                                        .SetAttribute("validita_inizio", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Validita_Inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Validita_Fine")))
                                        .SetAttribute("data_creazione", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Data_Creazione")))
                                        .SetAttribute("data_modifica", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Data_Modifica")))
                                        .SetAttribute("username_creazione", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("username_creazione")))
                                        .SetAttribute("username_modifica", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("username_modifica")))
                                        .SetAttribute("Id_Budget", Agro_SQL_SaveNum(0))
                                    End With



                                    Xml_DatiProdotti_Costi.AppendChild(XmlProdotto_Costo)
                                    '----- < / PRODOTTO_COSTO > -----
                                Next

                                XmlRapCon.AppendChild(Xml_DatiProdotti_Costi)
                                '----- < / DATIPRODOTTI_COSTI > -----
                            End If

                            '#################################


                            XmlContatto.AppendChild(XmlRapCon)


                        Next

                    End If


                    '#################################
                    '#################################


                    XmlDatiContatti.AppendChild(XmlContatto)

                Next


                XmlDoc.AppendChild(XmlDatiContatti)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlContatto = Nothing
                XmlDatiContatti = Nothing
                XmlDoc = Nothing
                XmlProdotto_Costo = Nothing
                DTCorrispettivi = Nothing
                objCorrispettivi = Nothing
                XmlIndirizzo = Nothing
                DTIndirizzi = Nothing
                objContattixIndirizzi = Nothing
                XmlCodice = Nothing
                DtContattixCodici = Nothing
                objContatti_Codici = Nothing


            Else

                'Altrimenti, se non risulta selezionato nessun contatto ...
                RisultatoFunzione = ""

            End If

            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale AndAlso Not IsNothing(objParametri.objConnessione) Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione




    End Function

    '============================================================================
    Public Function Contatto_ControllaMovimenti(
                            ByVal Piva As String,
                            ByVal Cod_Contatto As String,
                            ByRef PossibileCancellare As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                    As DataTable



        Dim cod_risum As Integer
        Dim ObjMovNC As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DtNonContabili As DataTable

        Dim ObjMovC As New AgronicaCoreContabDAL.Movimenti_R
        Dim DtContabili As DataTable

        Dim ObjRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim DtRisUm As DataTable

        Dim Rag_Soc As String = ""
        Dim Descrizione As String = ""
        Dim isGIAS As Boolean

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim i, j As Integer


        'griglia dei movimenti
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Movimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))

        'controllo se è un'impresa GIAS
        Dim objanagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Read
        isGIAS = objanagrafe.VerificaEsistenza_PivaGIAS(Cod_Contatto, objParametri)

        If Not isGIAS Then

            '===========================================================================
            'Lettura dei movimenti associati ad una risorsa umana di un contatto
            '---------------------------------------------------------------------------

            Dim objPDC As New AgronicaCoreAnagrafeDAL.PDC_Varie_r
            Dim dtapp As DataTable
            dtapp = objPDC.LeggixCancellazionePiva(Cod_Contatto, objParametri)
            For j = 0 To dtapp.Rows.Count - 1
                'istanzio una nuova riga
                Dr = Dt.NewRow
                Dr.Item("Data") = CDate(dtapp.Rows(j).Item("Data_Fornitura")).ToShortDateString
                Dr.Item("Movimento") = dtapp.Rows(j).Item("Rag_Soc")

                Dr.Item("Descrizione") = "PDC"

                Dr.Item("rag_soc") = dtapp.Rows(j).Item("Rag_Soc")

                Dt.Rows.Add(Dr)
            Next


            DtRisUm = ObjRisorseUmane.LeggiContattixSuperUser(CStr(Piva),
                                                              0,
                                                              Cod_Contatto,
                                                              0, 0, "", True,
                                                              enumSelezioneVariabile.Selezione_JoinCompleta,
                                                              "", "", objParametri)


            If DtRisUm IsNot Nothing AndAlso DtRisUm.Rows.Count > 0 Then

                For i = 0 To DtRisUm.Rows.Count - 1

                    Rag_Soc = DtRisUm.Rows(i).Item("Rag_Soc")

                    cod_risum = DtRisUm.Rows(i).Item("Cod_RisUm")


                    '===========================================================================
                    'Lettura dei Movimenti Non Contabili (Lavorazioni su campo..)
                    '---------------------------------------------------------------------------

                    DtNonContabili = ObjMovNC.Leggi("", 0, 0, 0, 0, 0, 0, cod_risum, "", 0, 0, 0,
                                                    0, 0, 0,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    "Elem_Cod = 0", "", objParametri)

                    If DtNonContabili IsNot Nothing AndAlso DtNonContabili.Rows.Count > 0 Then

                        For j = 0 To DtNonContabili.Rows.Count - 1

                            'istanzio una nuova riga
                            Dr = Dt.NewRow

                            Dr.Item("Data") = CDate(DtNonContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                            Dr.Item("Movimento") = DtNonContabili.Rows(j).Item("Des_Lib")

                            'Impostazione Descrizione del movimento
                            Select Case DtNonContabili.Rows(j).Item("Cau_Mov")
                                Case enum_Agenda_Causali.IMPUTAZIONE_MANODOPERA
                                    Descrizione = "Manodopera come dipendente" ' per " & RsNonContabili.Fields("Qta").Value ' all'ora o all'ettaro

                                Case enum_Agenda_Causali.IMPUTAZIONE_TERZISTI
                                    Descrizione = "Manodopera come terzista" ' per " & RsNonContabili.Fields("Qta").Value  ' all'ora o all'ettaro

                            End Select


                            Dr.Item("Descrizione") = Descrizione


                            Dr.Item("rag_soc") = DtNonContabili.Rows(j).Item("rag_soc")

                            'aggiungo la riga al datatable
                            Dt.Rows.Add(Dr)

                        Next

                    End If


                    '----------------------------
                    ' MOVIMENTI CONTABILI
                    '----------------------------

                    DtContabili = ObjMovC.Leggi("", 0, 0, 0,
                                                cod_risum,
                                                CAU_REGISTRAZIONI,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "", objParametri)


                    If DtContabili IsNot Nothing AndAlso DtContabili.Rows.Count > 0 Then

                        For j = 0 To DtContabili.Rows.Count - 1

                            'istanzio una nuova riga
                            Dr = Dt.NewRow

                            Dr.Item("Data") = CDate(DtContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                            Dr.Item("Movimento") = DtContabili.Rows(j).Item("Des_Lib")

                            Dr.Item("Descrizione") = DtContabili.Rows(j).Item("Mov_Desc")

                            Dr.Item("rag_soc") = DtContabili.Rows(j).Item("rag_soc")

                            'aggiungo la riga al datatable
                            Dt.Rows.Add(Dr)

                        Next
                    End If
                Next
            End If
            If Dt.Rows.Count = 0 Then
                PossibileCancellare = "Esistono in archivio movimenti collegati al contatto " & Rag_Soc & "." & Chr(13) & Chr(13) & "Cancellare tutte le registrazioni prima di richiederne l'eliminazione."
            End If

        Else
            PossibileCancellare = "Non è Possibile eliminare il contatto dato che si tratta di un impresa gias"

            Dr = Dt.NewRow
            Dr.Item("Data") = CDate(Date.Today).ToShortDateString
            Dr.Item("Movimento") = "Impresa Gias"
            Dr.Item("Descrizione") = PossibileCancellare
            Dr.Item("rag_soc") = "Impresa Gias"
            Dt.Rows.Add(Dr)


        End If
        Return Dt
    End Function

    Public Function LeggiUtentiDaAssociare(
                                        ByRef objParametri_server As AgronicaCoreParametri,
                                        ByRef objParametri_utenti As AgronicaCoreParametri
                                        ) As DataTable

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()
        Return objContatti.LeggiUtentiDaAssociare(objParametri_server, objParametri_utenti)

    End Function

    Public Function LeggiUtenteAssociato(ByVal piva As String,
                                         ByVal cod_contatto As String,
                                         ByRef objParametri_server As AgronicaCoreParametri
                                         ) As DataTable

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()
        Return objContatti.LeggiUtenteAssociato(piva, cod_contatto, objParametri_server)

    End Function

    Public Function ContattoRiferimenti(ByVal piva As String,
                                        ByVal Cod_Contatto As String,
                                        ByVal Cod_RisUm As Integer,
                                        ByVal Sa_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()
        Return objContatti.ContattoRiferimenti(piva, Cod_Contatto, Cod_RisUm, Sa_Cod, xFiltroAggiuntivo, objParametri)

    End Function

    Public Function LeggiUserTecnicoOCapo(
        ByVal username As String,
        ByRef objParametri_Server As AgronicaCoreParametri
    ) As enum_TipoOperatoreVisita

        Dim codRapporti As String
        Dim isCapo As Integer
        Dim isTecnico As Integer
        Dim result As enum_TipoOperatoreVisita
        Dim dtContatti As DataTable

        'l'idea è di restituire 
        '0 se lo user è solo capo
        '1 se è tecnico 
        '2 se è sia tecnico che capo
        '-1 altrimenti

        isCapo = 0
        isTecnico = 0
        result = enum_TipoOperatoreVisita.Altro


        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()

        'cerco se si tratta di un capo 
        codRapporti = "-20"
        dtContatti = objContatti.LeggiUtenteTecnicoOCapo(username, objParametri_Server, codRapporti)

        If dtContatti.Rows.Count > 0 Then
            isCapo += 1
        End If

        'cerco se si tratta di un tecnico 
        codRapporti = "-6"
        dtContatti = objContatti.LeggiUtenteTecnicoOCapo(username, objParametri_Server, codRapporti)

        If dtContatti.Rows.Count > 0 Then
            isTecnico += 1
        End If

        If isCapo + isTecnico = 2 Then
            result = enum_TipoOperatoreVisita.CapoTecnico
        Else
            If isCapo = 1 Then
                result = enum_TipoOperatoreVisita.Capo
            Else
                If isTecnico = 1 Then
                    result = enum_TipoOperatoreVisita.Tecnico
                End If
            End If
        End If

        Return result

    End Function

    Public Function LeggiListaTecnici(
        ByRef objParametri_Utente As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri
    ) As DataTable

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()
        Dim usersBIZ As New AgronicaCoreUtentiBIZ.Utenti
        Dim dtListaTecnici As DataTable
        Dim dtListaTecniciVisibilita As New DataTable()

        'Dim rigaVuota As DataRow = dtListaTecniciVisibilita.NewRow()
        'dtListaTecniciVisibilita.Rows.Add(rigaVuota)

        Dim isSuperUser As Boolean = (objParametri_Utente.SuperUserUsername = objParametri_Utente.UtenteUsername)
        Dim checkIfExistsImpresa As Boolean = Check_Impresa_Associata(objParametri_Utente.UtenteUsername, objParametri_Utente, objParametri_Server)
        Dim typeOperatore As enum_TipoOperatoreVisita = LeggiUserTecnicoOCapo(objParametri_Utente.UtenteUsername, objParametri_Server)

        If isSuperUser OrElse Not checkIfExistsImpresa Then
            dtListaTecniciVisibilita = objContatti.LeggiListaTecnici(objParametri_Utente, objParametri_Server, typeOperatore, False)     'chiamo con il filtro utente visibilità a false

            For Each dr As DataRow In dtListaTecniciVisibilita.Rows
                usersBIZ.InizializzaTabellaUtentiVisibilitaAppoggio(dr.Item("username"), 5, objParametri_Server, objParametri_Utente)
                '- utente_Username utente loggato
                '- idServizio: 5
            Next

        Else
            If typeOperatore = enum_TipoOperatoreVisita.Tecnico Then         'chiamo con il filtro utente visibilità a false
                dtListaTecniciVisibilita = objContatti.LeggiListaTecnici(objParametri_Utente, objParametri_Server, typeOperatore, False)     'chiamo con il filtro utente visibilità a false
            Else
                If typeOperatore = enum_TipoOperatoreVisita.CapoTecnico OrElse typeOperatore = enum_TipoOperatoreVisita.Capo Then
                    dtListaTecnici = objContatti.LeggiListaTecnici(objParametri_Utente, objParametri_Server, typeOperatore, False)     'chiamo con il filtro utente visibilità a false

                    For Each dr As DataRow In dtListaTecnici.Rows
                        usersBIZ.InizializzaTabellaUtentiVisibilitaAppoggio(dr.Item("username"), 5, objParametri_Server, objParametri_Utente)
                        '- utente_Username utente loggato
                        '- idServizio: 5
                    Next

                    'chiamo con il filtro utente visibilità a true

                    'a seconda del tipo di operatore, prendo una determinata lista di operatori selezionabili

                    dtListaTecniciVisibilita = objContatti.LeggiListaTecnici(objParametri_Utente, objParametri_Server, typeOperatore, True)     'chiamo con il filtro utente visibilità a true

                End If
            End If

        End If

        Return dtListaTecniciVisibilita

    End Function

    Public Function LeggiListaAziende(
        ByRef objParametri_Utente As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal Username As String
    ) As DataTable

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()

        Dim dtListaDDL As DataTable

        'calcolo il tipo operatore dell'utente loggato in modo da capire se l'utente è capo Area, devo far vedere solo le aziende comuni tra capoArea e tecnici
        Dim typeUserLogged As enum_TipoOperatoreVisita = LeggiUserTecnicoOCapo(objParametri_Utente.UtenteUsername, objParametri_Server)

        Dim typeTecnico As enum_TipoOperatoreVisita = LeggiUserTecnicoOCapo(Username, objParametri_Server)

        dtListaDDL = objContatti.LeggiListaAziendeEsclusoAgenzie(objParametri_Utente, objParametri_Server, Username, typeUserLogged, typeTecnico)

        Return dtListaDDL

    End Function

    Public Function ContattiPubbliciDaPrivatizzare(ByVal piva As String,
                                                   ByVal Cod_Contatto As List(Of String),
                                                   ByVal Cod_RisUm As String,
                                                   ByVal Cod_Rapporto As List(Of Integer),
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()
        Return objContatti.ContattiPubbliciDaPrivatizzare(piva, Cod_Contatto, Cod_RisUm, Cod_Rapporto, xFiltroAggiuntivo, objParametri)

    End Function

    '============================================================================
    Public Function Contatto_ControllaMovimentiXRisorsaUmana(
                            ByVal Piva As String,
                            ByVal Cod_Contatto As String,
                            ByVal Cod_RisUm As Integer,
                            ByRef PossibileCancellare As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                    As DataTable



        Dim ObjMovNC As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DtNonContabili As DataTable

        Dim ObjMovC As New AgronicaCoreContabDAL.Movimenti_R
        Dim DtContabili As DataTable

        Dim ObjRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim DtRisUm As DataTable

        Dim Rag_Soc As String = ""
        Dim Descrizione As String = ""
        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim i, j As Integer

        'griglia dei movimenti
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Movimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))

        DtRisUm = ObjRisorseUmane.LeggiContattixSuperUser(CStr(Piva),
                                                              Cod_RisUm,
                                                              Cod_Contatto,
                                                              0, 0, "", True,
                                                              enumSelezioneVariabile.Selezione_JoinCompleta,
                                                              "", "", objParametri)


        If DtRisUm IsNot Nothing AndAlso DtRisUm.Rows.Count > 0 Then

            For i = 0 To DtRisUm.Rows.Count - 1

                Rag_Soc = DtRisUm.Rows(i).Item("Rag_Soc")

                '===========================================================================
                'Lettura dei Movimenti Non Contabili (Lavorazioni su campo..)
                '---------------------------------------------------------------------------

                DtNonContabili = ObjMovNC.Leggi("", 0, 0, 0, 0, 0, 0, Cod_RisUm, "", 0, 0, 0,
                                                        0, 0, 0,
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "Elem_Cod = 0", "", objParametri)

                If DtNonContabili IsNot Nothing AndAlso DtNonContabili.Rows.Count > 0 Then

                    For j = 0 To DtNonContabili.Rows.Count - 1

                        'istanzio una nuova riga
                        Dr = Dt.NewRow

                        Dr.Item("Data") = CDate(DtNonContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                        Dr.Item("Movimento") = DtNonContabili.Rows(j).Item("Des_Lib")

                        'Impostazione Descrizione del movimento
                        Select Case DtNonContabili.Rows(j).Item("Cau_Mov")
                            Case enum_Agenda_Causali.IMPUTAZIONE_MANODOPERA
                                Descrizione = "Manodopera come dipendente" ' per " & RsNonContabili.Fields("Qta").Value ' all'ora o all'ettaro

                            Case enum_Agenda_Causali.IMPUTAZIONE_TERZISTI
                                Descrizione = "Manodopera come terzista" ' per " & RsNonContabili.Fields("Qta").Value  ' all'ora o all'ettaro

                        End Select


                        Dr.Item("Descrizione") = Descrizione


                        Dr.Item("rag_soc") = DtNonContabili.Rows(j).Item("rag_soc")

                        'aggiungo la riga al datatable
                        Dt.Rows.Add(Dr)

                    Next

                End If


                '----------------------------
                ' MOVIMENTI CONTABILI
                '----------------------------

                DtContabili = ObjMovC.Leggi("", 0, 0, 0,
                                                Cod_RisUm,
                                                CAU_REGISTRAZIONI,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "", objParametri)


                If DtContabili IsNot Nothing AndAlso DtContabili.Rows.Count > 0 Then

                    For j = 0 To DtContabili.Rows.Count - 1

                        'istanzio una nuova riga
                        Dr = Dt.NewRow

                        Dr.Item("Data") = CDate(DtContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                        Dr.Item("Movimento") = DtContabili.Rows(j).Item("Des_Lib")

                        Dr.Item("Descrizione") = DtContabili.Rows(j).Item("Mov_Desc")

                        Dr.Item("rag_soc") = DtContabili.Rows(j).Item("rag_soc")

                        'aggiungo la riga al datatable
                        Dt.Rows.Add(Dr)

                    Next
                End If
            Next
        End If
        If Dt.Rows.Count = 0 Then
            PossibileCancellare = "Esistono in archivio movimenti collegati al contatto " & Rag_Soc & "." & Chr(13) & Chr(13) & "Cancellare tutte le registrazioni prima di richiederne l'eliminazione."
        End If

        Return Dt

    End Function


    Public Function Contatto_LeggiDatiMinimi(
                           ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal ForDelete As Boolean,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                   As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R.Contatto_LeggiDatiMinimi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim i As Int32
        'Dim j As Int32

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument

        Dim XmlDatiContatti As XmlElement
        Dim XmlContatto As XmlElement
        Dim XmlRapCon As XmlElement

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

        Dim DTContatti As DataTable
        Dim DTRapCon As DataTable

        '------------------------------

        Try

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

            'Mi procuro un elenco dei Contatti associati all'Impresa
            'all'interno della finestra temporale selezionata


            'Mi procuro il recordset richiesto
            DTContatti = objContatti.LeggiContattoSpecifico(
                                             CStr(Piva),
                                             CStr(Cod_Contatto),
                                             99,
                                              enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                              "", "",
                                             objParametri)


            'Se ottengo almeno un risultato, creo la struttura XML 
            If DTContatti.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

                'Effettuo un ciclo 
                For i = 0 To DTContatti.Rows.Count - 1

                    'Effettuo un ciclo sui contatti
                    '----- < CONTATTO > -----
                    XmlContatto = XmlDoc.CreateElement("Contatto")

                    With XmlContatto
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DTContatti.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("cod_contatto", Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto")))
                        .SetAttribute("referente", Agro_SQL_Load(DTContatti.Rows(i).Item("Referente")))
                        .SetAttribute("rag_soc", Agro_SQL_Load(DTContatti.Rows(i).Item("Rag_Soc")))
                        .SetAttribute("id_cf", Agro_SQL_Load(DTContatti.Rows(i).Item("Id_CF")))
                        .SetAttribute("codice_fiscale", Agro_SQL_Load(DTContatti.Rows(i).Item("Codice_Fiscale")))
                        .SetAttribute("convenevoli", Agro_SQL_Load(DTContatti.Rows(i).Item("Convenevoli")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DTContatti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DTContatti.Rows(i).Item("Validita_Fine")))
                    End With




                    '#############################################
                    '###############  RISORSE UMANE  #############
                    '#############################################

                    'Mi procuro un elenco delle risorse umane associate al contatto

                    'Mi procuro il recordset richiesto
                    DTRapCon = objRisorse_Umane.LeggiContattixSuperUser(
                                             Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")),
                                             0,
                                             CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                             0,
                                             0,
                                             Piva_SuperUser_Origine,
                                             True,
                                             enumSelezioneVariabile.Selezione_JoinCompleta,
                                             "", "",
                                             objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DTRapCon.Rows.Count > 0 Then
                        Dim j As Integer
                        'Effettuo un ciclo sui rapporti contabili
                        For j = 0 To DTRapCon.Rows.Count - 1


                            '----- < RAPPORTO CONTABILE > -----
                            XmlRapCon = XmlDoc.CreateElement("RapCon")

                            With XmlRapCon
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DTRapCon.Rows(j).Item("Piva")))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("sa_cod", Agro_SQL_Load(DTRapCon.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("cod_risum", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_RisUm")))
                                .SetAttribute("cod_rapporto", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_Rapporto")))
                                .SetAttribute("cod_contatto", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_Contatto")))
                                .SetAttribute("occasionale", Agro_SQL_Load(DTRapCon.Rows(j).Item("Occasionale")))
                                .SetAttribute("corrispettivo_mensile", Agro_SQL_Load(DTRapCon.Rows(j).Item("Corrispettivo_Mensile")))
                                .SetAttribute("corrispettivo_orario", Agro_SQL_Load(DTRapCon.Rows(j).Item("Corrispettivo_Orario")))
                                .SetAttribute("ore_settimanali", Agro_SQL_Load(DTRapCon.Rows(j).Item("Ore_Settimanali")))
                                .SetAttribute("giorni_ferie", Agro_SQL_Load(DTRapCon.Rows(j).Item("Giorni_Ferie")))
                                .SetAttribute("ferie_godute", Agro_SQL_Load(DTRapCon.Rows(j).Item("Ferie_Godute")))
                                .SetAttribute("giorni_malattia", Agro_SQL_Load(DTRapCon.Rows(j).Item("Giorni_Malattia")))
                                .SetAttribute("settore_des", Agro_SQL_Load(DTRapCon.Rows(j).Item("Settore_Des")))
                                .SetAttribute("attivita_des", Agro_SQL_Load(DTRapCon.Rows(j).Item("Attivita_Des")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DTRapCon.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DTRapCon.Rows(j).Item("Validita_Fine")))
                                .SetAttribute("patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Patentino")))
                                .SetAttribute("data_rilascio_patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Rilascio_Patentino")))
                                .SetAttribute("data_scadenza_patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Scadenza_Patentino")))
                            End With

                            If Piva_SuperUser_Origine <> "" Then
                                XmlRapCon.SetAttribute("piva_superuser_origine", CStr(Piva_SuperUser_Origine))
                            End If

                            '#################################


                            XmlContatto.AppendChild(XmlRapCon)

                            '----- < / RAPPORTO CONTABILE > -----

                        Next
                    End If


                    XmlRapCon = Nothing
                    DTRapCon = Nothing
                    objRisorse_Umane = Nothing




                    XmlDatiContatti.AppendChild(XmlContatto)
                    '----- < / CONTATTO > -----

                Next


                XmlDoc.AppendChild(XmlDatiContatti)

                RisultatoFunzione = XmlDoc.OuterXml

                '----- < / Documento XML > -----
            Else
                RisultatoFunzione = ""
            End If


            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale AndAlso Not IsNothing(objParametri.objConnessione) Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function


    Public Function ContattoImpresa_Leggi(
                           ByVal Cod_Contatto As String,
                           ByVal ForDelete As Boolean,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                   As String

        Dim NomeRoutine As String = "AnagrafeBIZ.Contatti_MultiHost_R.Contatto_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim i As Int32
        'Dim j As Int32

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument

        Dim XmlDatiContatti As XmlElement
        Dim XmlContatto As XmlElement
        Dim XmlRapCon As XmlElement
        Dim XmlIndirizzo As XmlElement
        Dim XmlProdotto_Costo As XmlElement
        Dim Xml_DatiProdotti_Costi As XmlElement

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R


        Dim objImpresexIndirizzi As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R


        Dim objCorrispettivi As New AgronicaCoreContabDAL.Prodotti_Costi_R

        Dim DTContatti As DataTable
        Dim DTIndirizzi As DataTable
        Dim DTRapCon As DataTable
        Dim DTCorrispettivi As DataTable



        '------------------------------

        Try

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

            'Mi procuro il recordset richiesto
            DTContatti = objContatti.LeggiContattoSpecifico(
                                                objParametri.PivaSuperUser,
                                             CStr(Cod_Contatto),
                                             99,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "",
                                             objParametri)


            'Se ottengo almeno un risultato, creo la struttura XML 
            If DTContatti.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

                'Effettuo un ciclo 
                For i = 0 To DTContatti.Rows.Count - 1

                    'Effettuo un ciclo sui contatti
                    '----- < CONTATTO > -----
                    XmlContatto = XmlDoc.CreateElement("Contatto")

                    With XmlContatto
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DTContatti.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("cod_contatto", Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto")))
                        .SetAttribute("referente", Agro_SQL_Load(DTContatti.Rows(i).Item("Referente")))
                        .SetAttribute("rag_soc", Agro_SQL_Load(DTContatti.Rows(i).Item("Rag_Soc")))
                        .SetAttribute("id_cf", Agro_SQL_Load(DTContatti.Rows(i).Item("Id_CF")))
                        .SetAttribute("codice_fiscale", Agro_SQL_Load(DTContatti.Rows(i).Item("Codice_Fiscale")))
                        .SetAttribute("convenevoli", Agro_SQL_Load(DTContatti.Rows(i).Item("Convenevoli")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DTContatti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DTContatti.Rows(i).Item("Validita_Fine")))
                    End With


                    '#################################
                    '##########  INDIRIZZI  ##########
                    '#################################

                    'Mi procuro il recordset richiesto
                    DTIndirizzi = objImpresexIndirizzi.Leggi(
                                                       CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                        0,
                                                       0,
                                                       enumSelezioneVariabile.Selezione_JoinCompleta,
                                                         "",
                                                        "",
                                                        objParametri)


                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DTIndirizzi.Rows.Count > 0 Then

                        Dim j As Integer

                        'Effettuo un ciclo sugli indirizzi
                        For j = 0 To DTIndirizzi.Rows.Count - 1

                            '----- < INDIRIZZO > -----
                            XmlIndirizzo = XmlDoc.CreateElement("Indirizzo")

                            'come sa_cod gli metto quello della tabella contatti

                            With XmlIndirizzo
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Piva")))
                                '.SetAttribute("sa_cod", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("tipo_indirizzo", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("Tipo_Indirizzo")))
                                .SetAttribute("cod_indirizzo", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("cod_indirizzo")))
                                .SetAttribute("ind_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("ind_des")))
                                .SetAttribute("frz_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("frz_des")))
                                .SetAttribute("cap", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("CAP")))
                                .SetAttribute("com_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("com_des")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("pro_cod")))
                                .SetAttribute("pro_des", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("pro_des")))
                                .SetAttribute("stato", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("stato")))
                                .SetAttribute("note", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("note")))
                                .SetAttribute("pro_cod_istat", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("pro_cod_istat")))
                                .SetAttribute("com_cod_istat", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("com_cod_istat")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DTIndirizzi.Rows(j).Item("validita_fine")))
                            End With

                            XmlContatto.AppendChild(XmlIndirizzo)
                            '----- < / INDIRIZZO > -----
                        Next

                    End If

                    XmlIndirizzo = Nothing
                    DTIndirizzi = Nothing
                    objImpresexIndirizzi = Nothing


                    '#############################################
                    '###############  RISORSE UMANE  #############
                    '#############################################

                    'Mi procuro il recordset richiesto
                    DTRapCon = objRisorse_Umane.LeggiContattixSuperUser(Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")),
                                                                        0,
                                                                        CStr(Agro_SQL_Load(DTContatti.Rows(i).Item("Cod_Contatto"))),
                                                                        0,
                                                                        0,
                                                                        "",
                                                                        True,
                                                                        enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                        "", "",
                                                                        objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DTRapCon.Rows.Count > 0 Then
                        Dim j As Integer
                        'Effettuo un ciclo sui rapporti contabili
                        For j = 0 To DTRapCon.Rows.Count - 1

                            '----- < RAPPORTO CONTABILE > -----
                            XmlRapCon = XmlDoc.CreateElement("RapCon")

                            With XmlRapCon
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DTRapCon.Rows(j).Item("Piva")))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("sa_cod", Agro_SQL_Load(DTRapCon.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("cod_risum", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_RisUm")))
                                .SetAttribute("cod_rapporto", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_Rapporto")))
                                .SetAttribute("cod_contatto", Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_Contatto")))
                                .SetAttribute("occasionale", Agro_SQL_Load(DTRapCon.Rows(j).Item("Occasionale")))
                                .SetAttribute("corrispettivo_mensile", Agro_SQL_Load(DTRapCon.Rows(j).Item("Corrispettivo_Mensile")))
                                .SetAttribute("corrispettivo_orario", Agro_SQL_Load(DTRapCon.Rows(j).Item("Corrispettivo_Orario")))
                                .SetAttribute("ore_settimanali", Agro_SQL_Load(DTRapCon.Rows(j).Item("Ore_Settimanali")))
                                .SetAttribute("giorni_ferie", Agro_SQL_Load(DTRapCon.Rows(j).Item("Giorni_Ferie")))
                                .SetAttribute("ferie_godute", Agro_SQL_Load(DTRapCon.Rows(j).Item("Ferie_Godute")))
                                .SetAttribute("giorni_malattia", Agro_SQL_Load(DTRapCon.Rows(j).Item("Giorni_Malattia")))
                                .SetAttribute("settore_des", Agro_SQL_Load(DTRapCon.Rows(j).Item("Settore_Des")))
                                .SetAttribute("attivita_des", Agro_SQL_Load(DTRapCon.Rows(j).Item("Attivita_Des")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DTRapCon.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DTRapCon.Rows(j).Item("Validita_Fine")))
                                .SetAttribute("patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Patentino")))
                                .SetAttribute("data_rilascio_patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Rilascio_Patentino")))
                                .SetAttribute("data_scadenza_patentino", Agro_SQL_Load(DTRapCon.Rows(j).Item("Data_Scadenza_Patentino")))
                            End With



                            '#######################################################
                            '##########  PRODOTTI COSTI RAPPORTI CONTABILI  ########
                            '#######################################################

                            '----- < RAPPORTO CONTABILE > -----
                            Xml_DatiProdotti_Costi = XmlDoc.CreateElement("DatiProdotti_Costi")



                            'Mi procuro il recordset richiesto
                            DTCorrispettivi = objCorrispettivi.Leggi(
                                                     Agro_SQL_Load(DTContatti.Rows(i).Item("PIVA")),
                                                     0,
                                                     "",
                                                     0,
                                                     Agro_SQL_Load(DTRapCon.Rows(j).Item("Cod_RisUm")),
                                                     0,
                                                     0,
                                                     0,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "",
                                                     objParametri)


                            'Se ottengo almeno un risultato, creo la struttura XML
                            If DTCorrispettivi.Rows.Count > 0 Then
                                Dim jj As Integer
                                'Effettuo un ciclo
                                For jj = 0 To DTCorrispettivi.Rows.Count - 1

                                    '----- < RAPPORTO CONTABILE > -----
                                    XmlProdotto_Costo = XmlDoc.CreateElement("Prodotto_Costo")

                                    With XmlProdotto_Costo
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("piva", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Piva")))
                                        .SetAttribute("riferimento", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Riferimento")))
                                        .SetAttribute("elem_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Elem_Cod")))
                                        .SetAttribute("pro_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Pro_Cod")))
                                        .SetAttribute("mat_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Mat_Cod")))
                                        .SetAttribute("udm_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Udm_Cod")))
                                        .SetAttribute("mezzo", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Mezzo")))
                                        .SetAttribute("prezzo_unitario", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Prezzo_Unitario")))
                                        .SetAttribute("veg_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Veg_Cod")))
                                        .SetAttribute("cul_cod", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Cul_Cod")))
                                        .SetAttribute("validita_inizio", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Validita_Inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(DTCorrispettivi.Rows(jj).Item("Validita_Fine")))
                                        .SetAttribute("Id_Budget", Agro_SQL_SaveNum(0))
                                    End With



                                    Xml_DatiProdotti_Costi.AppendChild(XmlProdotto_Costo)
                                    '----- < / PRODOTTO_COSTO > -----

                                Next

                                XmlRapCon.AppendChild(Xml_DatiProdotti_Costi)
                                '----- < / DATIPRODOTTI_COSTI > -----

                            End If

                            XmlProdotto_Costo = Nothing
                            DTCorrispettivi = Nothing


                            '#################################


                            XmlContatto.AppendChild(XmlRapCon)

                            '----- < / RAPPORTO CONTABILE > -----
                        Next

                    End If


                    XmlRapCon = Nothing
                    DTRapCon = Nothing
                    objRisorse_Umane = Nothing
                    objCorrispettivi = Nothing


                    '#################################
                    '#################################
                    '#################################



                    XmlDatiContatti.AppendChild(XmlContatto)
                    '----- < / CONTATTO > -----

                Next

                XmlDoc.AppendChild(XmlDatiContatti)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlContatto = Nothing
                XmlDatiContatti = Nothing
                XmlDoc = Nothing


                '----- < / Documento XML > -----
            Else
                RisultatoFunzione = ""
            End If


            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale AndAlso Not IsNothing(objParametri.objConnessione) Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function


    '###################################################################################
    'cod_contatto è la piva dell'impresa padre da cui si sta scaricando dal magazzino
    Public Sub RicavaScrive_Contatto_Fornitore(ByVal Piva_Contatto As String,
                                             ByVal Cod_Contatto As String,
                                                ByVal OUT_Piva_Contatto As String,
                                                ByRef OUT_Cod_Risum As Integer,
                                                ByRef OUT_Cod_Indirizzo As Integer,
                                                ByRef OUT_Rag_Soc As String,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Const NomeFunzione As String = "RicavaScrive_Contatto_Fornitore"


        Try

            Dim objContInd_R As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
            Dim objCont_W As New AgronicaCoreAnagrafeDAL.Contatti_W
            Dim objRisUm_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim objRisUm_W As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W
            Dim DT_RisUm As DataTable
            Dim Sa_Cod As Integer

            'legge il contatto dal cod_contatto e ricava cod_risum se fornitore
            DT_RisUm = objRisUm_R.LeggiJoinRapportiContabili(Piva_Contatto,
                                                          Cod_Contatto,
                                                         0,
                                                         0,
                                                         0,
                                                         1,
                                                         0, 0, 0, 0,
                                                         0, "",
                                                         True,
                                                         "", "",
                                                         objParametri_Server)

            If Not IsNothing(DT_RisUm) AndAlso DT_RisUm.Rows.Count > 0 Then

                With DT_RisUm.Rows(0)

                    'piva_contatto è la piva che ha creato il contatto impresa padre dell'impresa che fa la semina
                    'ricava anche sa_cod
                    OUT_Piva_Contatto = .Item("Piva")
                    Sa_Cod = .Item("Sa_Cod")
                    OUT_Cod_Risum = .Item("Cod_Risum")
                    OUT_Rag_Soc = .Item("Rag_Soc")

                    If Sa_Cod = PRIVATO Then
                        'il contatto è privato, devo renderlo pubblico
                        objCont_W.Modifica_Parametrizzata(OUT_Piva_Contatto,
                                                            Cod_Contatto,
                                                            "Sa_Cod",
                                                            PUBBLICO,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                             "",
                                                             objParametri_Server)

                        objRisUm_W.Modifica_Parametrizzata(OUT_Piva_Contatto,
                                                            Cod_Contatto,
                                                            "Sa_Cod",
                                                            PUBBLICO,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                             "",
                                                             objParametri_Server)
                    End If

                End With

            Else
                '- non c'è: aggiungo al contatto la risorsa umana fornitore

                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim Dummy As Integer

                OUT_Cod_Risum = ObjSequenze.NuovoId_Tabella("Risorse_Umane",
                                                                0,
                                                                 0,
                                                                objParametri_Server)

                'legge il contatto dal cod_contatto e ricava cod_risum se fornitore
                DT_RisUm = objRisUm_R.LeggiSoloContatto(Piva_Contatto,
                                                        0,
                                                        Cod_Contatto,
                                                        0, 0, "", True,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "",
                                                        objParametri_Server)

                If Not IsNothing(DT_RisUm) AndAlso DT_RisUm.Rows.Count > 0 Then

                    With DT_RisUm.Rows(0)

                        'piva_contatto è la piva che ha creato il contatto impresa padre dell'im presa che fa la semina
                        'ricava anche sa_cod
                        OUT_Piva_Contatto = .Item("Piva")
                        Sa_Cod = .Item("Sa_Cod")
                        OUT_Cod_Risum = .Item("Cod_Risum")
                        OUT_Rag_Soc = .Item("Rag_Soc")

                    End With

                    If Sa_Cod = PRIVATO Then
                        'il contatto è privato, devo renderlo pubblico
                        objCont_W.Modifica_Parametrizzata(OUT_Piva_Contatto,
                                                            Cod_Contatto,
                                                            "Sa_Cod",
                                                            PUBBLICO,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                             "",
                                                             objParametri_Server)

                        objRisUm_W.Modifica_Parametrizzata(OUT_Piva_Contatto,
                                                            Cod_Contatto,
                                                            "Sa_Cod",
                                                            PUBBLICO,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                             "",
                                                             objParametri_Server)
                    End If

                    Dummy = objRisUm_W.Scrivi(OUT_Piva_Contatto,
                                              PUBBLICO,
                                                OUT_Cod_Risum,
                                                CStr(Cod_Contatto),
                                                COD_FORNITORE,
                                               "",
                                                "",
                                                0,
                                                0,
                                               0,
                                               0,
                                                0,
                                                0,
                                                0,
                                                "",
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                "",
                                                0,
                                                "",
                                                0,
                                                0,
                                                0,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                objParametri_Server)

                Else
                    Throw New Exception("Contatto impresa non recuperato!")
                End If

            End If

            If OUT_Cod_Risum <> 0 Then
                OUT_Cod_Indirizzo = objContInd_R.CodIndirizzo_from_CodRisUm(OUT_Cod_Risum,
                                                                objParametri_Server)
            Else
                Throw New Exception("Codice fornitore non recuperato!")
            End If


        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Throw New Exception(msg)
        End Try


    End Sub
    Public Function Leggi_Contatti_Azienda(Cod_Contatto As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)

        If Cod_Contatto = "" Then
            Throw New Exception("Cod contatto obbligatorio")
        End If

        Dim contatti As New List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)
        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim Dt_Contatto = objContattiR.Leggi_Contatti_Azienda(Cod_Contatto, objParametri_Server)

        Dim PivaPadre = Dt_Contatto.AsEnumerable().Select(Function(row) row.Field(Of String)("PivaProprietario")).Distinct().ToList()

        For Each padre In PivaPadre
            Dim rapporti_contabili = Dt_Contatto.Select("PivaProprietario = '" & padre & "'").CopyToDataTable()
            If rapporti_contabili IsNot Nothing Then
                Dim contatto As New AgronicaCoreModelsSTD.anagrafiche.Contatto

                contatto.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(padre, Cod_Contatto)
                contatto.aziendaProprietaria = rapporti_contabili.Rows(0)("Proprietario")
                'contatto.badge = row?("NrBadge")
                'contatto.cognome = row("cognome")
                'contatto.convenevoli = row?("Convenevoli")
                'contatto.data_Nascita = row?("Data_Nascita")
                'contatto.nome = row?("Proprietario")
                'contatto.nome_Breve = IIf(IsDBNull(row?("Nome_Breve")), "", row?("Nome_Breve"))
                'contatto.ragione_Sociale = row?("Cod_Contatto")
                'contatto.sesso = row?("Sesso")
                contatto.tipo = ""
                contatto.visibilitaPubblica = rapporti_contabili.Rows(0)("Sa_Cod") = -1

                Select Case contatto.visibilitaPubblica
                    Case 0
                        contatto.fisico_Giuridico = 0
                        contatto.estero = False
                    Case 1
                        contatto.fisico_Giuridico = 1
                        contatto.estero = False
                    Case 2
                        contatto.estero = True
                End Select

                contatto.fisico_Giuridico = 0
                contatto.fittizio = False

                'If (row("ChkFittizio") = 1) Then
                '    contatto.fittizio = True
                'End If
                contatto.risorseUmane = New List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)

                For Each row As DataRow In rapporti_contabili.Rows

                    Dim risorsa_umana = New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane()

                    risorsa_umana.rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(row("Cod_Rapporto")) With {.descrizione = row("Rapporto_Des")}
                    risorsa_umana.settore = row("Settore_Des")
                    risorsa_umana.attivita = row("Attivita_Des")

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("cliente") Then
                        risorsa_umana.rapportoContabile.cliente = True
                    End If

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("fornitore") Then
                        risorsa_umana.rapportoContabile.fornitore = True
                    End If

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("dipendente") Then
                        risorsa_umana.rapportoContabile.dipendente = True
                    End If

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("terzista") Then
                        risorsa_umana.rapportoContabile.terzista = True
                    End If

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("legale") Then
                        risorsa_umana.rapportoContabile.legale = True
                    End If

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("agente") Then
                        risorsa_umana.rapportoContabile.agente = True
                    End If

                    If risorsa_umana.rapportoContabile.descrizione.ToLower().Equals("consulente") Then
                        risorsa_umana.rapportoContabile.consulente = True
                    End If


                    contatto.risorseUmane.Add(risorsa_umana)
                Next
                contatti.Add(contatto)
            End If
        Next
        Return contatti
    End Function


    Public Function Leggi_Contatto(Piva As String,
                                   Cod_Contatto As String,
                                   leggi_indirizzi As Boolean,
                                   leggi_dati_fe As Boolean,
                                   ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Contatto

        If Piva = "" OrElse Cod_Contatto = "" Then
            Throw New Exception("Leggi_Contatto da usare per leggere singolo contatto non contatti massivi")
        End If

        Dim contatto As New AgronicaCoreModelsSTD.anagrafiche.Contatto
        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim Dt_Contatto = objContattiR.Contatti_Contatto_Leggi(
                                                         Piva, Cod_Contatto, 0, 0, True, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0,
                                                         enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                        objParametri_Server, leggi_NrBadge:=True)

        Dim i = 0

        Dim risorse_umane As New List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)
        For Each row In Dt_Contatto.Rows
            If i = 0 Then

                contatto.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(Piva, Cod_Contatto)

                contatto.badge = row("NrBadge")
                contatto.cognome = row("cognome")
                contatto.convenevoli = row("Convenevoli")
                contatto.data_Nascita = row("Data_Nascita")

                contatto.nome = row("Nome")
                contatto.nome_Breve = IIf(IsDBNull(row("Nome_Breve")), "", row("Nome_Breve"))
                contatto.ragione_Sociale = row("Rag_Soc")
                'contatto.risorseUmane
                'contatto.rubricaVoci
                contatto.sesso = row("Sesso")
                contatto.tipo = ""
                contatto.visibilitaPubblica = CInt(row("Sa_Cod")) = -1

                Select Case CInt(row("id_CF"))
                    Case 0
                        contatto.fisico_Giuridico = 0
                        contatto.estero = False
                    Case 1
                        contatto.fisico_Giuridico = 1
                        contatto.estero = False
                    Case 2
                        contatto.estero = True
                End Select

                contatto.fisico_Giuridico = 0
                contatto.fittizio = False

                If (row("ChkFittizio") = 1) Then
                    contatto.fittizio = True
                End If

                If leggi_indirizzi Then
                    Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
                    contatto.indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Contatto(Piva, Cod_Contatto, objParametri_Server)
                End If

                If leggi_dati_fe Then
                    contatto.fe_Pec = ""
                    contatto.fe_Rappresentante_Fiscale = ""
                    contatto.fe_SDI = ""
                    contatto.fe_Tipologia_Contatto = ""
                End If

            End If

            Dim risorsa_umana = New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane(row("cod_RisUm"))

            risorsa_umana.rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(row("Cod_Rapporto")) With {.descrizione = row("Rapporto_Des")}
            risorsa_umana.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(row("Validita_Inizio"), row("Validita_Fine"))

            risorsa_umana.settore = row("Settore_Des")
            risorsa_umana.attivita = row("Attivita_Des")

            If row("Cliente") = 1 Then
                risorsa_umana.rapportoContabile.cliente = True
            End If

            If row("Fornitore") = 1 Then
                risorsa_umana.rapportoContabile.fornitore = True
            End If

            If row("Dipendente") = 1 Then
                risorsa_umana.rapportoContabile.dipendente = True
            End If

            If row("Terzista") = 1 Then
                risorsa_umana.rapportoContabile.terzista = True
            End If

            If row("Legale") = 1 Then
                risorsa_umana.rapportoContabile.legale = True
            End If

            If row("Agente") = 1 Then
                risorsa_umana.rapportoContabile.agente = True
            End If

            If row("Consulente") = 1 Then
                risorsa_umana.rapportoContabile.consulente = True
            End If

            risorse_umane.Add(risorsa_umana)
            i += 1
        Next
        contatto.risorseUmane = risorse_umane

        Return contatto
    End Function


    'Public Function Leggi_Contatti_Impresa(Piva As String,
    '                                       ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)
    '    Dim contatti_list As New List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)
    '    Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R

    '    'Dim Dt_ContattoxRisorsa = objContattiR.Contatti_Contatto_Leggi(
    '    '                                                 Piva, "", 0, 0, False, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0,
    '    '                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
    '    '                                                 "",
    '    '                                                 "",
    '    '                                                objParametri_Server)

    '    'Dim dt_Contatti = Dt_ContattoxRisorsa.

    '    'Dim i = 0
    '    'For Each row In Dt_Contatto.Rows
    '    '    If i = 0 Then



    '    '    End If
    '    '    i += 1
    '    'Next

    '    Return contatti_list
    'End Function

    ''' <summary>
    ''' Carica i contatto utilizzabili per l'assegnazione di manodopera nella
    ''' modifica multipla di attività nel MenuAgenda.
    ''' Ricalca quanto avviene nella funzione CaricaListaPersone nella pagina
    ''' ModificaMultipla_Operazioni.aspx.vb
    ''' </summary>
    ''' <returns>Lista contenente i contatti assegnabili alle operazioni di campagna</returns>
    Public Function CaricaListaPersone(operazioni As List(Of AgronicaCoreModelsSTD.attivita.Attivita_xModificaMultipla),
                                       Solo_Aziendali As Boolean,
                                       objParametri_Server As AgronicaCoreParametri)

        Dim objRappContabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        Dim DT As DataTable

        Dim Piva As String = ""
        Dim PivaSingola As Boolean = True
        Dim MostraPubblici As Boolean = True
        Dim xFiltroAggiuntivo As String = " ( Rapporti_Contabili.Legale = 1 " &
                                          " Or Rapporti_Contabili.Dipendente = 1 " &
                                          " Or Rapporti_Contabili.terzista = 1 ) "
        Dim xOrderBy As String = " Contatti.sa_cod desc,  Contatti.Rag_Soc, cognome, nome ASC "

        If operazioni.Select(Function(a) a.Piva).Distinct().Count() = 1 Then
            Piva = operazioni.ElementAt(0).Piva
        Else
            Piva = "----"
            PivaSingola = False
        End If

        If PivaSingola Then
            If Solo_Aziendali Then
                MostraPubblici = False
            Else
                MostraPubblici = True
            End If
        Else
            MostraPubblici = True
        End If

        DT = objRappContabili.RapportiContabilixCostiAccessori(Piva, False, False, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Server, isModificaMultipla:=True, FlagPubblico:=MostraPubblici)


        Dim Dt_Contatti As New DataTable
        ''----- Definisco la struttura del DataTable
        ''1
        Dt_Contatti.Columns.Add(New DataColumn("Piva", GetType(String)))
        ''2
        Dt_Contatti.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        ''3
        Dt_Contatti.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))
        ''4
        Dt_Contatti.Columns.Add(New DataColumn("Rag_Soc_Nome_Cognome", GetType(String)))
        ''5
        Dt_Contatti.Columns.Add(New DataColumn("Cod_Rapporto", GetType(String)))
        ''6
        Dt_Contatti.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))
        ''7
        Dt_Contatti.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        ''8
        Dt_Contatti.Columns.Add(New DataColumn("Unita_Misura", GetType(String)))
        ''9
        Dt_Contatti.Columns.Add(New DataColumn("Mezzo", GetType(String)))
        ''10
        Dt_Contatti.Columns.Add(New DataColumn("Cod_RisUm", GetType(String)))
        ''11
        Dt_Contatti.Columns.Add(New DataColumn("Impresa", GetType(String)))
        ''12
        Dt_Contatti.Columns.Add(New DataColumn("Dipendente", GetType(String)))
        ''13
        Dt_Contatti.Columns.Add(New DataColumn("Terzista", GetType(String)))

        Dt_Contatti.Columns.Add(New DataColumn("Costo_Inizio", GetType(String)))
        '12
        Dt_Contatti.Columns.Add(New DataColumn("Costo_Fine", GetType(String)))


        Dim DR As DataRow
        Dim Costo_Inizio, Costo_Fine As String

        For i = 0 To DT.Rows.Count - 1
            DR = Dt_Contatti.NewRow()
            DR.Item("Piva") = DT.Rows(i).Item("Piva")
            DR.Item("sa_cod") = DT.Rows(i).Item("sa_cod")
            DR.Item("Cod_Contatto") = DT.Rows(i).Item("Cod_Contatto")
            DR.Item("Rag_Soc_Nome_Cognome") = DT.Rows(i).Item("Rag_Soc_Nome_Cognome")
            DR.Item("Rapporto_Des") = DT.Rows(i).Item("Rapporto_Des")

            DR.Item("Cod_RisUm") = DT.Rows(i).Item("Cod_RisUm")
            DR.Item("Impresa") = DT.Rows(i).Item("Impresa")
            DR.Item("Dipendente") = DT.Rows(i).Item("Dipendente")
            DR.Item("Terzista") = DT.Rows(i).Item("Terzista")
            DR.Item("Cod_Rapporto") = DT.Rows(i).Item("Cod_Rapporto")

            If DT.Rows(i).Item("Mezzo") = 1 Then
                DR.Item("Unita_Misura") = "Ha"
            ElseIf DT.Rows(i).Item("Mezzo") = 2 Then
                DR.Item("Unita_Misura") = "Ora"
            End If
            DR.Item("Mezzo") = DT.Rows(i).Item("Mezzo")

            If DT.Rows(i).Item("Elem_Cod") = "0" Then
                DR.Item("Prezzo_Unitario") = DT.Rows(i).Item("Prezzo_Unitario")

                Costo_Inizio = DT.Rows(i).Item("Costo_Inizio")
                If Costo_Inizio.Length > 0 Then
                    Costo_Inizio = Costo_Inizio.Substring(0, 10)
                    If Costo_Inizio = "01/01/1900" Then
                        Costo_Inizio = "---"
                    End If
                Else
                    Costo_Inizio = "---"
                End If
                DR.Item("Costo_Inizio") = Costo_Inizio

                Costo_Fine = DT.Rows(i).Item("Costo_Fine")
                If Costo_Fine.Length > 0 Then
                    Costo_Fine = Costo_Fine.Substring(0, 10)
                    If Costo_Fine = "31/12/2100" Then
                        Costo_Fine = "---"
                    End If
                Else
                    Costo_Fine = "---"
                End If
                DR.Item("Costo_Fine") = Costo_Fine
            End If
            Dt_Contatti.Rows.Add(DR)
        Next

        Return Dt_Contatti
    End Function

    Public Function Check_Impresa_Associata(
                                              ByVal username As String,
                                              ByRef objParametri_Utente As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Boolean

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R()
        Dim typeOperatore As enum_TipoOperatoreVisita = LeggiUserTecnicoOCapo(username, objParametri_Server)

        Dim checkIfExists As Boolean = objContatti.Check_Impresa_Associata(username, objParametri_Utente, objParametri_Server, typeOperatore)

        Return checkIfExists

    End Function

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Contatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Property Provider As Globalization.CultureInfo
    Public Property Format As String
    Public Property ValiditaInizio As Date
    Public Property ValiditaFine As Date

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

    Public Function Contatto_Scrivi(
                                ByVal DatiContatto As String,
                                    ByRef OUTPUT_Piva As String,
                                    ByRef OUTPUT_Cod_Contatto As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal ProdottiCostiCompleta As Boolean = False,
                                        Optional ByVal NoteXLog As String = Nothing,
                                        Optional ByVal Origine As Integer = enum_SistemiEsterni.gias
                                        ) As Boolean

        '============================================================================

        Dim XmlDoc As XmlDocument


        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objContatti As AgronicaCoreAnagrafeDAL.Contatti_W
        Dim objContattixIndirizzi As AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_W
        Dim objIndirizzi As AgronicaCoreAnagrafeDAL.Indirizzi_Write
        Dim objContattixRubrica As AgronicaCoreAnagrafeDAL.ContattixRubrica_W
        Dim objRubrica As AgronicaCoreAnagrafeDAL.Rubrica_Write
        Dim objRisorse_Umane As AgronicaCoreAnagrafeDAL.Risorse_Umane_W
        Dim objRisorse_Umane_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim ObjContatti_Codici As New AgronicaCoreAnagrafeDAL.Contatti_Codici_W
        Dim ObjProdotti_Costi As New AgronicaCoreContabBIZ.Prodotti_Costi_W
        Dim ObjLiquidita As New AgronicaCoreContabDAL.Liquidita_W
        Dim ObjParco_Macchine As New AgronicaCoreContabDAL.Parco_Macchine_W
        Dim objConti As New AgronicaCoreContabDAL.ContixContatti_W
        Dim objLog As New AgronicaCoreAnagrafeDAL.Agronica_Log_Contatti_W

        Dim Dummy As Long
        Dim DummyProdotti_Costi As Boolean

        Dim Cod_Indirizzo As Int32
        Dim Cod_Rubrica As Int32
        Dim Cod_Contatto As String
        'Dim Cod_Liquidita As Int32
        Dim Cod_RisUm As Int32
        Dim DatiProdotti_Costi As String
        Dim Codice_Fiscale As String
        Dim Mac_Cod As Integer

        Dim xDatiContatti As Xml.XmlNodeList
        Dim xDatiContatto As Xml.XmlElement
        Dim xContatti As Xml.XmlNodeList
        Dim xContatto As Xml.XmlElement
        Dim xConti As Xml.XmlNodeList
        Dim xConto As Xml.XmlElement
        Dim xIndirizzi As Xml.XmlNodeList
        Dim xIndirizzo As Xml.XmlElement = Nothing
        Dim xRubriche As Xml.XmlNodeList
        Dim xRubrica As Xml.XmlElement
        Dim xRapCons As Xml.XmlNodeList
        Dim xRapCon As Xml.XmlElement
        Dim xLiquiditas As Xml.XmlNodeList
        Dim xLiquidita As Xml.XmlElement
        Dim xCodici As Xml.XmlNodeList
        Dim xCodice As Xml.XmlElement
        Dim xParco_Macchine As Xml.XmlNodeList
        Dim xParco_Macchina As Xml.XmlElement
        Dim xProdotti_Costi As Xml.XmlNodeList
        Dim xProdotto_Costi As Xml.XmlElement


        Dim i_DatiContatto As Integer
        Dim i_Contatto As Integer
        Dim i_Indirizzo As Integer
        Dim i_Rubrica As Integer
        Dim i_RapCon As Integer
        Dim i_Codici As Integer
        Dim i_Liquidita As Integer
        Dim i_Parco_Macchina As Integer
        Dim i_Conti As Integer

        Dim OpeDB_Contatto As String
        Dim OpeDB_Indirizzo As String
        Dim OpeDB_Rubrica As String
        Dim OpeDB_RapCon As String
        Dim OpeDB_Codice As String
        Dim OpeDB_Parco_Macchina As String
        Dim OPEDB_Conto As String
        Dim OpeDB_Liquidita As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False 'True
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        Dim xPiva As String = ""
        Dim xCod_Contatto As String = ""


        '------------------------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Contatti_W.Contatto_Scrivi()"


        Try


            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            'If objParametri.objConnessione Is Nothing Then
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            '    FlagConnessioneLocale = True

            'End If

            'If objParametri.objTransazione Is Nothing Then
            '    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction()
            '    FlagTransazioneLocale = True
            'End If

            '------------------------------

            XmlDoc = New Xml.XmlDocument
            XmlDoc.LoadXml(DatiContatto)

            '------------------------------

            xDatiContatti = XmlDoc.GetElementsByTagName("DatiContatti")

            i_DatiContatto = 0

            Do While i_DatiContatto < xDatiContatti.Count

                'Prelevo l'i-esimo blocco di DatiContatti (in realtà ne esiste uno solo)
                xDatiContatto = xDatiContatti.Item(i_DatiContatto)

                '------------------------------

                xContatti = xDatiContatto.GetElementsByTagName("Contatto")

                i_Contatto = 0

                Do While i_Contatto < xContatti.Count

                    'Prelevo l' i-esimo Contatto
                    xContatto = xContatti.Item(i_Contatto)

                    'Prelevo gli attributi del contatto selezionato
                    OpeDB_Contatto = xContatto.GetAttribute("TipoOperazioneDB")

                    'Istanzio l'oggetto
                    objContatti = New AgronicaCoreAnagrafeDAL.Contatti_W

                    'Inizializzo Preventivamente il Cod_Contatto
                    Cod_Contatto = CStr(xContatto.GetAttribute("cod_contatto"))

                    If Not xContatto.HasAttribute("codice_fiscale") Then
                        'se non viene passato il codice fiscale, metto come default il cod_contatto
                        Codice_Fiscale = Cod_Contatto
                    Else
                        Codice_Fiscale = xContatto.GetAttribute("codice_fiscale")
                    End If

                    Dim Piva As String = CStr(xContatto.GetAttribute("piva"))
                    Dim Sa_Cod As Integer = CInt(xContatto.GetAttribute("sa_cod"))
                    Dim Id_CF As Integer = CInt(xContatto.GetAttribute("id_cf"))
                    Dim Rag_Soc As String = CStr(xContatto.GetAttribute("rag_soc"))
                    Dim Nome As String = CStr(xContatto.GetAttribute("nome"))
                    Dim Cognome As String = CStr(xContatto.GetAttribute("cognome"))
                    Dim ContattoDes As String = If(String.IsNullOrEmpty(Rag_Soc), String.Format("{0} {1}", Cognome, Nome), Rag_Soc)

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Contatto

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Piva = CStr(xContatto.GetAttribute("piva"))
                            OUTPUT_Cod_Contatto = CStr(xContatto.GetAttribute("cod_contatto"))

                        Case "1"    'SALVA -------------------------------------------------------

                            '===============================================================================================
                            'Verifica Codice Contatto
                            '-----------------------------------------------------------------------------------------------
                            Select Case Cod_Contatto

                                Case "" 'Codice Contatto Sconosciuto --> Creazione di un Codice Progressivo Negativo

                                    '28/10/2019: modificata creazione del cod-contatto fittizio
                                    'dalla modalità giaslan si passa alla modalità giasonline
                                    '(il giasonline lo crea/creava a monte, dal data entry, ecco perché qui è stato adeguato solo ora)

                                    'ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
                                    'Cod_Contatto = CStr(ObjSequenze.NuovoId_Tabella(
                                    '                                    "Contatti",
                                    '                                    -2000000000,
                                    '                                    0,
                                    '                                        objParametri))

                                    Cod_Contatto = ObjSequenze.NuovoId_Tabella("impresa", 0, 0, objParametri).ToString.Replace("-", "F")


                                Case Else
                                    'Codice Contatto Conosciuto
                                    'Do nothing
                            End Select
                            '===============================================================================================

                            OUTPUT_Piva = CStr(xContatto.GetAttribute("piva"))
                            OUTPUT_Cod_Contatto = Cod_Contatto


                            Dim tipo_indirizzo_default As Integer = 0
                            If xContatto.HasAttribute("tipo_indirizzo_default") AndAlso Not IsNothing(xContatto.GetAttribute("tipo_indirizzo_default")) Then
                                tipo_indirizzo_default = xContatto.GetAttribute("tipo_indirizzo_default")
                            End If

                            Dim data_nascita As Date
                            If xContatto.GetAttribute("data_nascita") = "" Then
                                data_nascita = AGRODATAINIZIO
                            Else
                                data_nascita = CDate(xContatto.GetAttribute("data_nascita")).ToShortDateString()
                            End If

                            Dim contatto_data_creazione As Date = #2/1/1900#
                            Dim contatto_data_Modifica As Date = #2/1/1900#
                            Dim contatto_username_creazione As String = ""
                            Dim contatto_username_modifica As String = ""

                            If Not IsNothing(xContatto.GetAttribute("data_creazione")) AndAlso
                                xContatto.GetAttribute("data_creazione") <> "" Then
                                contatto_data_creazione = CDate(xContatto.GetAttribute("data_creazione"))
                            End If

                            If Not IsNothing(xContatto.GetAttribute("data_modifica")) AndAlso
                                xContatto.GetAttribute("data_modifica") <> "" Then
                                contatto_data_Modifica = CDate(xContatto.GetAttribute("data_modifica"))
                            End If

                            If Not IsNothing(xContatto.GetAttribute("username_creazione")) Then
                                contatto_username_creazione = CStr(xContatto.GetAttribute("username_creazione"))
                            End If

                            If Not IsNothing(xContatto.GetAttribute("username_modifica")) Then
                                contatto_username_modifica = CStr(xContatto.GetAttribute("username_modifica"))
                            End If

                            Dim Tipo_Speditore As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Tipo_Speditore"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Tipo_Speditore"))) Then
                                Tipo_Speditore = xContatto.GetAttribute(LCase("Tipo_Speditore"))
                            End If
                            Dim Tipo_Destinazione As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Tipo_Destinazione"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Tipo_Destinazione"))) Then
                                Tipo_Destinazione = xContatto.GetAttribute(LCase("Tipo_Destinazione"))
                            End If
                            Dim Agente_Cod As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Agente_Cod"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Agente_Cod"))) Then
                                Agente_Cod = xContatto.GetAttribute(LCase("Agente_Cod"))
                            End If
                            Dim Provvigione As Decimal = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Provvigione"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Provvigione"))) Then
                                Provvigione = xContatto.GetAttribute(LCase("Provvigione"))
                            End If

                            Dim Note As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note"))) Then
                                Note = xContatto.GetAttribute(LCase("Note"))
                            End If

                            Dim Id_Gestione_Note As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Id_Gestione_Note"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Id_Gestione_Note"))) Then
                                Id_Gestione_Note = xContatto.GetAttribute(LCase("Id_Gestione_Note"))
                            End If

                            Dim Note2 As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note2"))) Then
                                Note2 = xContatto.GetAttribute(LCase("Note2"))
                            End If
                            Dim Note_Operazioni As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note_Operazioni"))) Then
                                Note_Operazioni = xContatto.GetAttribute(LCase("Note_Operazioni"))
                            End If
                            Dim Note2_Operazioni As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note2_Operazioni"))) Then
                                Note2_Operazioni = xContatto.GetAttribute(LCase("Note2_Operazioni"))
                            End If

                            Dim Cod_Risum_Destinazione_Diversa As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Cod_Risum_Destinazione_Diversa"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Cod_Risum_Destinazione_Diversa"))) Then
                                Cod_Risum_Destinazione_Diversa = xContatto.GetAttribute(LCase("Cod_Risum_Destinazione_Diversa"))
                            End If
                            Dim Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"))) Then
                                Tipo_Indirizzo_Default_Destinazione_Diversa = xContatto.GetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"))
                            End If
                            Dim Fido As Decimal = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Fido"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Fido"))) Then
                                Fido = xContatto.GetAttribute(LCase("Fido"))
                            End If
                            Dim Limite_Posizioni As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Limite_Posizioni"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Limite_Posizioni"))) Then
                                Limite_Posizioni = xContatto.GetAttribute(LCase("Limite_Posizioni"))
                            End If
                            Dim Limite_Giorni_Evasione As Decimal = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Limite_Giorni_Evasione"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Limite_Giorni_Evasione"))) Then
                                Limite_Giorni_Evasione = xContatto.GetAttribute(LCase("Limite_Giorni_Evasione"))
                            End If

                            Dim Orari_Ritiro As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Orari_Ritiro"))) Then
                                Orari_Ritiro = xContatto.GetAttribute(LCase("Orari_Ritiro"))
                            End If
                            Dim Filtro_Rimborsi As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Filtro_Rimborsi"))) Then
                                Filtro_Rimborsi = xContatto.GetAttribute(LCase("Filtro_Rimborsi"))
                            End If

                            Dim Vettore_Cod As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Vettore_Cod"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Vettore_Cod"))) Then
                                Vettore_Cod = xContatto.GetAttribute(LCase("Vettore_Cod"))
                            End If
                            Dim CapoArea_Cod As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("CapoArea_Cod"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("CapoArea_Cod"))) Then
                                CapoArea_Cod = xContatto.GetAttribute(LCase("CapoArea_Cod"))
                            End If
                            Dim Provvigione_CapoArea As Decimal = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Provvigione_CapoArea"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Provvigione_CapoArea"))) Then
                                Provvigione_CapoArea = xContatto.GetAttribute(LCase("Provvigione_CapoArea"))
                            End If

                            Dim Memo As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Memo"))) Then
                                Memo = xContatto.GetAttribute(LCase("Memo"))
                            End If
                            Dim Sconto_Contatto As Decimal = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Sconto_Contatto"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Sconto_Contatto"))) Then
                                Sconto_Contatto = xContatto.GetAttribute(LCase("Sconto_Contatto"))
                            End If
                            Dim Sconto_Testo As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Sconto_Testo"))) Then
                                Sconto_Testo = xContatto.GetAttribute(LCase("Sconto_Testo"))
                            End If
                            Dim Modalita_Fatturazione As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Modalita_Fatturazione"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Modalita_Fatturazione"))) Then
                                Modalita_Fatturazione = xContatto.GetAttribute(LCase("Modalita_Fatturazione"))
                            End If
                            Dim Cod_Iva_Contatto As Integer = -1
                            If Not IsNothing(xContatto.GetAttribute(LCase("Cod_Iva_Contatto"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Cod_Iva_Contatto"))) Then
                                Cod_Iva_Contatto = xContatto.GetAttribute(LCase("Cod_Iva_Contatto"))
                            End If
                            Dim Cod_Conto_Economico_Default As Integer = -1
                            If Not IsNothing(xContatto.GetAttribute(LCase("Cod_Conto_Economico_Default"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Cod_Conto_Economico_Default"))) Then
                                Cod_Conto_Economico_Default = xContatto.GetAttribute(LCase("Cod_Conto_Economico_Default"))
                            End If
                            Dim Cod_Conto_Patrimoniale_Default As Integer = -1
                            If Not IsNothing(xContatto.GetAttribute(LCase("Cod_Conto_Patrimoniale_Default"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Cod_Conto_Patrimoniale_Default"))) Then
                                Cod_Conto_Patrimoniale_Default = xContatto.GetAttribute(LCase("Cod_Conto_Patrimoniale_Default"))
                            End If
                            Dim Documento_Fatturazione As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Documento_Fatturazione"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Documento_Fatturazione"))) Then
                                Documento_Fatturazione = xContatto.GetAttribute(LCase("Documento_Fatturazione"))
                            End If

                            Dim nrBadge As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("nrBadge"))) Then
                                nrBadge = xContatto.GetAttribute(LCase("nrBadge"))
                            End If

                            Dim chkFittizio As Integer? = Nothing
                            If xContatto.HasAttribute(LCase("ChkFittizio")) AndAlso Not IsNothing(xContatto.GetAttribute(LCase("ChkFittizio"))) Then
                                chkFittizio = Convert.ToInt32(CBool(xContatto.GetAttribute(LCase("ChkFittizio"))))
                            End If

                            Dim nomeBreve As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Nome_Breve"))) Then
                                nomeBreve = xContatto.GetAttribute(LCase("Nome_Breve"))
                            End If

                            Dim EUDR As Integer? = Nothing
                            If xContatto.HasAttribute(LCase("EUDR")) AndAlso Not IsNothing(xContatto.GetAttribute(LCase("EUDR"))) Then
                                EUDR = Convert.ToInt32(CBool(xContatto.GetAttribute(LCase("EUDR"))))
                            End If

                            Dummy = objContatti.Scrivi(Piva,
                                                       Sa_Cod,
                                                       CStr(Cod_Contatto),
                                                       Id_CF,
                                                       Rag_Soc,
                                                       CStr(Codice_Fiscale),
                                                       CStr(xContatto.GetAttribute("convenevoli")),
                                                       tipo_indirizzo_default,
                                                       Nome,
                                                       Cognome,
                                                       data_nascita,
                                                       CStr(xContatto.GetAttribute("sesso")),
                                                       CStr(xContatto.GetAttribute("cod_contatto_referente")),
                                                       CDate(xContatto.GetAttribute("validita_inizio")),
                                                       CDate(xContatto.GetAttribute("validita_fine")),
                                                       objParametri,
                                                       contatto_data_creazione,
                                                       contatto_data_Modifica,
                                                       contatto_username_creazione,
                                                       contatto_username_modifica,
                                                       Tipo_Speditore,
                                                       Tipo_Destinazione,
                                                       Agente_Cod,
                                                       Provvigione,
                                                       Note,
                                                       Id_Gestione_Note,
                                                       Note2,
                                                       Note_Operazioni,
                                                       Note2_Operazioni,
                                                       Cod_Risum_Destinazione_Diversa,
                                                       Tipo_Indirizzo_Default_Destinazione_Diversa,
                                                       Fido,
                                                       Limite_Posizioni,
                                                       Limite_Giorni_Evasione,
                                                       Orari_Ritiro,
                                                       Filtro_Rimborsi,
                                                       Vettore_Cod,
                                                       CapoArea_Cod,
                                                       Provvigione_CapoArea,
                                                       Memo,
                                                       Sconto_Contatto,
                                                       Sconto_Testo,
                                                       Modalita_Fatturazione,
                                                       Cod_Iva_Contatto,
                                                       Documento_Fatturazione,
                                                       nrBadge:=nrBadge,
                                                       ChkFittizio:=chkFittizio,
                                                       Cod_Conto_Economico_Default:=Cod_Conto_Economico_Default,
                                                       Cod_Conto_Patrimoniale_Default:=Cod_Conto_Patrimoniale_Default,
                                                       Nome_Breve:=nomeBreve,
                                                       EUDR:=EUDR
                                                       )

                            ' Scrittura in Agronica_Log_Contatti
                            objLog.Scrivi(CInt(OpeDB_Contatto), Piva, Cod_Contatto, Sa_Cod, Id_CF, ContattoDes, NoteXLog, enum_Id_Servizio.GiasOnline, objParametri, DatiContatto, Origine)


                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Piva = CStr(xContatto.GetAttribute("piva"))
                            OUTPUT_Cod_Contatto = CStr(xContatto.GetAttribute("cod_contatto"))
                            Dim dataDef As Date = AGRODATAINIZIO

                            Dim Memo As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Memo"))) Then
                                Memo = xContatto.GetAttribute(LCase("Memo"))
                            End If
                            Dim Tipo_Destinazione As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Tipo_Destinazione"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Tipo_Destinazione"))) Then
                                Tipo_Destinazione = xContatto.GetAttribute(LCase("Tipo_Destinazione"))
                            End If
                            Dim Note As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note"))) Then
                                Note = xContatto.GetAttribute(LCase("Note"))
                            End If
                            Dim TipoSpeditore As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Tipo_Speditore"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Tipo_Speditore"))) Then
                                TipoSpeditore = CInt(xContatto.GetAttribute(LCase("Tipo_Speditore")))
                            End If
                            'Dim Id_Gestione_Note As Integer = 0
                            'If Not IsNothing(xContatto.GetAttribute(LCase("Id_Gestione_Note"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Id_Gestione_Note"))) Then
                            '    Id_Gestione_Note = xContatto.GetAttribute(LCase("Id_Gestione_Note"))
                            'End If

                            Dim Note2 As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note2"))) Then
                                Note2 = xContatto.GetAttribute(LCase("Note2"))
                            End If
                            Dim Note_Operazioni As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note_Operazioni"))) Then
                                Note_Operazioni = xContatto.GetAttribute(LCase("Note_Operazioni"))
                            End If
                            Dim Note2_Operazioni As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Note2_Operazioni"))) Then
                                Note2_Operazioni = xContatto.GetAttribute(LCase("Note2_Operazioni"))
                            End If

                            Dim Sconto_Testo As String = ""
                            Dim scontoAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Sconto_Testo")), XmlAttribute)
                            If Not IsNothing(scontoAttribute) Then
                                Sconto_Testo = scontoAttribute.Value
                            End If
                            Dim Agente_Cod As Integer? = Nothing
                            Dim agenteCodAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Agente_Cod")), XmlAttribute)
                            If Not IsNothing(agenteCodAttribute) AndAlso IsNumeric(agenteCodAttribute.Value) Then
                                Agente_Cod = agenteCodAttribute.Value
                            End If

                            Dim CapoArea_Cod As Integer? = Nothing
                            Dim capoAreaCodAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("CapoArea_Cod")), XmlAttribute)
                            If Not IsNothing(capoAreaCodAttribute) AndAlso IsNumeric(capoAreaCodAttribute.Value) Then
                                CapoArea_Cod = capoAreaCodAttribute.Value
                            End If

                            Dim Vettore_Cod As Integer? = Nothing
                            Dim vettoreCodAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Vettore_Cod")), XmlAttribute)
                            If Not IsNothing(vettoreCodAttribute) AndAlso IsNumeric(vettoreCodAttribute.Value) Then
                                Vettore_Cod = vettoreCodAttribute.Value
                            End If

                            Dim Modalita_Fatturazione As Integer? = Nothing
                            Dim modFatAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Modalita_Fatturazione")), XmlAttribute)
                            If Not IsNothing(modFatAttribute) AndAlso IsNumeric(modFatAttribute.Value) Then
                                Modalita_Fatturazione = modFatAttribute.Value
                            End If

                            Dim Cod_Iva_Contatto As Integer? = Nothing
                            Dim codIvaAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Cod_Iva_Contatto")), XmlAttribute)
                            If Not IsNothing(codIvaAttribute) AndAlso IsNumeric(codIvaAttribute.Value) Then
                                Cod_Iva_Contatto = codIvaAttribute.Value
                            End If

                            Dim Cod_Conto_Economico_Default As Integer? = Nothing
                            Dim codContoEcoAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Cod_Conto_Economico_Default")), XmlAttribute)
                            If Not IsNothing(codContoEcoAttribute) AndAlso IsNumeric(codContoEcoAttribute.Value) Then
                                Cod_Conto_Economico_Default = codContoEcoAttribute.Value
                            End If

                            Dim Cod_Conto_Patrimoniale_Default As Integer? = Nothing
                            Dim codContoPatAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Cod_Conto_Patrimoniale_Default")), XmlAttribute)
                            If Not IsNothing(codContoPatAttribute) AndAlso IsNumeric(codContoPatAttribute.Value) Then
                                Cod_Conto_Patrimoniale_Default = codContoPatAttribute.Value
                            End If

                            Dim Provvigione As Decimal = 0
                            Dim provvigioneAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Provvigione")), XmlAttribute)
                            If Not IsNothing(provvigioneAttribute) AndAlso IsNumeric(provvigioneAttribute.Value) Then
                                Provvigione = provvigioneAttribute.Value
                            End If

                            Dim Provvigione_CapoArea As Decimal = 0
                            Dim provCapoAreaAttibute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("Provvigione_CapoArea")), XmlAttribute)
                            If Not IsNothing(provCapoAreaAttibute) AndAlso IsNumeric(provCapoAreaAttibute.Value) Then
                                Provvigione_CapoArea = provCapoAreaAttibute.Value
                            End If

                            Dim Cod_Risum_Destinazione_Diversa As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Cod_Risum_Destinazione_Diversa"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Cod_Risum_Destinazione_Diversa"))) Then
                                Cod_Risum_Destinazione_Diversa = xContatto.GetAttribute(LCase("Cod_Risum_Destinazione_Diversa"))
                            End If
                            Dim Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0
                            If Not IsNothing(xContatto.GetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"))) AndAlso IsNumeric(xContatto.GetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"))) Then
                                Tipo_Indirizzo_Default_Destinazione_Diversa = xContatto.GetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"))
                            End If
                            Dim chkFittizio As Integer? = Nothing
                            Dim fittizioAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("ChkFittizio")), XmlAttribute)
                            If Not IsNothing(fittizioAttribute) Then

                                chkFittizio = Convert.ToInt32(CBool(fittizioAttribute.Value))
                            End If
                            Dim nomeBreve As String = ""
                            If Not IsNothing(xContatto.GetAttribute(LCase("Nome_Breve"))) Then
                                nomeBreve = xContatto.GetAttribute(LCase("Nome_Breve"))
                            End If
                            Dim EUDR As Integer? = Nothing
                            Dim EUDRAttribute As XmlAttribute = CType(xContatto.Attributes.GetNamedItem(LCase("EUDR")), XmlAttribute)
                            If Not IsNothing(EUDRAttribute) Then
                                EUDR = Convert.ToInt32(CBool(EUDRAttribute.Value))
                            End If

                            objContatti.Modifica(
                                                CStr(xContatto.GetAttribute("piva")),
                                                CInt(xContatto.GetAttribute("sa_cod")),
                                                CStr(xContatto.GetAttribute("cod_contatto")),
                                                CInt(xContatto.GetAttribute("id_cf")),
                                                CStr(xContatto.GetAttribute("rag_soc")),
                                                CStr(Codice_Fiscale),
                                                CStr(xContatto.GetAttribute("convenevoli")),
                                                If(Not xContatto.HasAttribute("tipo_indirizzo_default"), 0, xContatto.GetAttribute("tipo_indirizzo_default")),
                                                CStr(xContatto.GetAttribute("nome")),
                                                CStr(xContatto.GetAttribute("cognome")),
                                                If(xContatto.GetAttribute("data_nascita") = "", dataDef, xContatto.GetAttribute("data_nascita")),
                                                CStr(xContatto.GetAttribute("sesso")),
                                                CStr(xContatto.GetAttribute("cod_contatto_referente")),
                                                CDate(xContatto.GetAttribute("validita_inizio")),
                                                CDate(xContatto.GetAttribute("validita_fine")),
                                                CStr(xContatto.GetAttribute("nrbadge")),
                                                "",
                                                objParametri,
                                                Memo:=Memo,
                                                Tipo_Destinazione:=Tipo_Destinazione,
                                                Note:=Note, Note2:=Note2,
                                                Note_Operazioni:=Note_Operazioni, Note2_Operazioni:=Note2_Operazioni,
                                                Tipo_Speditore:=TipoSpeditore,
                                                Sconto_Testo:=Sconto_Testo,
                                                Agente_Cod:=Agente_Cod,
                                                CapoArea_Cod:=CapoArea_Cod,
                                                Vettore_Cod:=Vettore_Cod,
                                                Modalita_Fatturazione:=Modalita_Fatturazione,
                                                Cod_Iva_Contatto:=Cod_Iva_Contatto,
                                                Cod_Conto_Economico_Default:=Cod_Conto_Economico_Default,
                                                Cod_Conto_Patrimoniale_Default:=Cod_Conto_Patrimoniale_Default,
                                                Provvigione:=Provvigione,
                                                Provvigione_CapoArea:=Provvigione_CapoArea,
                                                Cod_Risum_Destinazione_Diversa:=Cod_Risum_Destinazione_Diversa,
                                                Tipo_Indirizzo_Default_Destinazione_Diversa:=Tipo_Indirizzo_Default_Destinazione_Diversa,
                                                ChkFittizio:=chkFittizio, Nome_Breve:=nomeBreve, EUDR:=EUDR
                                            )


                            ' Scrittura in Agronica_Log_Contatti
                            objLog.Scrivi(CInt(OpeDB_Contatto), Piva, Cod_Contatto, Sa_Cod, Id_CF, ContattoDes, NoteXLog, enum_Id_Servizio.GiasOnline, objParametri, DatiContatto, Origine)

                    End Select

                    '-------------------------------------------------------------
                    ' INDIRIZZI
                    '-------------------------------------------------------------

                    'Prelevo l'elenco degli indirizzi
                    xIndirizzi = xContatto.GetElementsByTagName("Indirizzo")

                    i_Indirizzo = 0

                    Do While i_Indirizzo < xIndirizzi.Count

                        'Prelevo l'i-esimo indirizzo
                        xIndirizzo = xIndirizzi.Item(i_Indirizzo)

                        'Prelevo gli attributi dell'indirizzo selezionato
                        OpeDB_Indirizzo = xIndirizzo.GetAttribute("TipoOperazioneDB")

                        'Creo l'oggetto COM
                        objContattixIndirizzi = New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_W
                        objIndirizzi = New AgronicaCoreAnagrafeDAL.Indirizzi_Write

                        Cod_Indirizzo = CInt(xIndirizzo.GetAttribute("cod_indirizzo"))

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Indirizzo

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                'Richiedo un nuovo codice indirizzo

                                If Cod_Indirizzo <= 0 Then

                                    ' istanzio l'oggetto
                                    ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                    Cod_Indirizzo = ObjSequenze.NuovoId_Tabella(
                                                "Indirizzi",
                                                CInt(xIndirizzo.GetAttribute("basecode")),
                                                CInt(xIndirizzo.GetAttribute("topcode")),
                                                    objParametri)

                                    'ObjSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If


                                Dim indirizzo_data_creazione As Date = #2/1/1900#
                                Dim indirizzo_data_Modifica As Date = #2/1/1900#
                                Dim indirizzo_username_creazione As String = ""
                                Dim indirizzo_username_modifica As String = ""
                                Dim codice_lingua As String = Nothing

                                If Not IsNothing(xIndirizzo.GetAttribute("data_creazione")) AndAlso
                                    xIndirizzo.GetAttribute("data_creazione") <> "" Then
                                    indirizzo_data_creazione = CDate(xIndirizzo.GetAttribute("data_creazione"))
                                End If

                                If Not IsNothing(xIndirizzo.GetAttribute("data_modifica")) AndAlso
                                    xIndirizzo.GetAttribute("data_modifica") <> "" Then
                                    indirizzo_data_Modifica = CDate(xIndirizzo.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xIndirizzo.GetAttribute("username_creazione")) Then
                                    indirizzo_username_creazione = CStr(xIndirizzo.GetAttribute("username_creazione"))
                                End If

                                If Not IsNothing(xIndirizzo.GetAttribute("username_modifica")) Then
                                    indirizzo_username_modifica = CStr(xIndirizzo.GetAttribute("username_modifica"))
                                End If
                                If Not IsNothing(xIndirizzo.GetAttribute("codice_lingua")) Then
                                    codice_lingua = CStr(xIndirizzo.GetAttribute("codice_lingua"))
                                End If

                                'Salvo l'indirizzo
                                Dummy = objIndirizzi.Scrivi(
                                                CLng(Cod_Indirizzo),
                                                CStr(xIndirizzo.GetAttribute("ind_des")),
                                                CStr(xIndirizzo.GetAttribute("frz_des")),
                                                CStr(xIndirizzo.GetAttribute("cap")),
                                                CStr(xIndirizzo.GetAttribute("com_des")),
                                                CStr(xIndirizzo.GetAttribute("pro_cod")),
                                                CStr(xIndirizzo.GetAttribute("stato")),
                                                CStr(xIndirizzo.GetAttribute("note")),
                                                CStr(xIndirizzo.GetAttribute("pro_cod_istat")),
                                                CStr(xIndirizzo.GetAttribute("com_cod_istat")),
                                                    CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                    CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                        objParametri,
                                                indirizzo_data_creazione,
                                                indirizzo_data_Modifica,
                                                indirizzo_username_creazione,
                                                indirizzo_username_modifica,
                                                Codice_Lingua:=codice_lingua
                                    )


                                'Salvo la relazione Contatto x Indirizzo
                                Dummy = objContattixIndirizzi.Scrivi(
                                                CStr(xContatto.GetAttribute("piva")),
                                                CInt(xContatto.GetAttribute("sa_cod")),
                                                CStr(Cod_Contatto),
                                                Cod_Indirizzo,
                                                CInt(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                                    CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                    CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                        objParametri,
                                                indirizzo_data_creazione,
                                                indirizzo_data_Modifica,
                                                indirizzo_username_creazione,
                                                indirizzo_username_modifica
                                        )

                            Case "2"    'MODIFICA -------------------------------------------------------

                                Dim codice_lingua As String = Nothing
                                If Not IsNothing(xIndirizzo.GetAttribute("codice_lingua")) Then
                                    codice_lingua = CStr(xIndirizzo.GetAttribute("codice_lingua"))
                                End If
                                objIndirizzi.Modifica(
                                                CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                CStr(xIndirizzo.GetAttribute("ind_des")),
                                                CStr(xIndirizzo.GetAttribute("frz_des")),
                                                CStr(xIndirizzo.GetAttribute("cap")),
                                                CStr(xIndirizzo.GetAttribute("com_des")),
                                                CStr(xIndirizzo.GetAttribute("pro_cod")),
                                                CStr(xIndirizzo.GetAttribute("stato")),
                                                CStr(xIndirizzo.GetAttribute("note")),
                                                CStr(xIndirizzo.GetAttribute("pro_cod_istat")),
                                                CStr(xIndirizzo.GetAttribute("com_cod_istat")),
                                                CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                "", objParametri, Codice_Lingua:=codice_lingua)
                                'CInt(xIndirizzo.GetAttribute("tipo_indirizzo")))

                                objContattixIndirizzi.ModificaPuntuale(
                                                CStr(xContatto.GetAttribute("piva")),
                                                CStr(Cod_Contatto),
                                                CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                objParametri,
                                                CInt(xContatto.GetAttribute("sa_cod")),
                                                CInt(xIndirizzo.GetAttribute("tipo_indirizzo"))
                                                )


                                'objContattixIndirizzi.Modifica( _
                                '        CStr(Cod_Contatto), _
                                '        CStr(xContatto.GetAttribute("piva")), _
                                '        CInt(xContatto.GetAttribute("sa_cod")), _
                                '        CInt(xIndirizzo.GetAttribute("cod_indirizzo")), _
                                '        CInt(xIndirizzo.GetAttribute("tipo_indirizzo")), _
                                '        CStr(Username_Operazione), _
                                '        CDate(xIndirizzo.GetAttribute("validita_inizio")), _
                                '        CDate(xIndirizzo.GetAttribute("validita_fine")), _
                                '        objConnessione, _
                                '        objTransazione, _
                                '        StringaConnessione, _
                                '        DirectoryLOG, _
                                '        FileLOG, _
                                '        IdentificatoreUtente)


                            Case "3"    'ELIMINA -------------------------------------------------------

                                objContattixIndirizzi.Cancella(
                                                CStr(xContatto.GetAttribute("cod_contatto")),
                                                CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                CInt(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                                    "",
                                                    objParametri)

                                objIndirizzi.Cancella(
                                                CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                    "",
                                                    objParametri)

                        End Select


                        'Elimino l'oggetto
                        'objContattixIndirizzi = Nothing

                        'objIndirizzi = Nothing

                        'ObjSequenze = Nothing

                        'Incremento l'indice
                        i_Indirizzo = i_Indirizzo + 1

                    Loop


                    '-------------------------------------------------------------
                    ' RUBRICA
                    '-------------------------------------------------------------

                    'Prelevo l'elenco delle rubriche
                    xRubriche = xContatto.GetElementsByTagName("Rubrica")

                    i_Rubrica = 0

                    Do While i_Rubrica < xRubriche.Count

                        'Prelevo l'i-esima rubrica
                        xRubrica = xRubriche.Item(i_Rubrica)

                        'Prelevo gli attributi del codice selezionato
                        OpeDB_Rubrica = xRubrica.GetAttribute("TipoOperazioneDB")

                        'Creo l'oggetto COM
                        objRubrica = New AgronicaCoreAnagrafeDAL.Rubrica_Write
                        objContattixRubrica = New AgronicaCoreAnagrafeDAL.ContattixRubrica_W

                        Cod_Rubrica = CInt(xRubrica.GetAttribute("cod_rubrica"))

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Rubrica


                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                If Cod_Rubrica <= 0 Then

                                    ' istanzio l'oggetto
                                    ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                    Cod_Rubrica = ObjSequenze.NuovoId_Tabella("Rubrica",
                                                CInt(xIndirizzo.GetAttribute("basecode")),
                                                CInt(xIndirizzo.GetAttribute("topcode")),
                                                    objParametri)

                                    'ObjSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If


                                Dim rubrica_data_creazione As Date = #2/1/1900#
                                Dim rubrica_data_Modifica As Date = #2/1/1900#
                                Dim rubrica_username_creazione As String = ""
                                Dim rubrica_username_modifica As String = ""

                                If Not IsNothing(xRubrica.GetAttribute("data_creazione")) AndAlso
                                    xRubrica.GetAttribute("data_creazione") <> "" Then
                                    rubrica_data_creazione = CDate(xRubrica.GetAttribute("data_creazione"))
                                End If

                                If Not IsNothing(xRubrica.GetAttribute("data_modifica")) AndAlso
                                    xRubrica.GetAttribute("data_modifica") <> "" Then
                                    rubrica_data_Modifica = CDate(xRubrica.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xRubrica.GetAttribute("username_creazione")) Then
                                    rubrica_username_creazione = CStr(xRubrica.GetAttribute("username_creazione"))
                                End If

                                If Not IsNothing(xRubrica.GetAttribute("username_modifica")) Then
                                    rubrica_username_modifica = CStr(xRubrica.GetAttribute("username_modifica"))
                                End If

                                'Salvataggio
                                Dummy = objRubrica.Scrivi(
                                                Cod_Rubrica,
                                                CStr(xRubrica.GetAttribute("numero")),
                                                CStr(xRubrica.GetAttribute("descr")),
                                                CDate(xRubrica.GetAttribute("validita_inizio")),
                                                CDate(xRubrica.GetAttribute("validita_fine")),
                                                    objParametri,
                                                    rubrica_data_creazione,
                                                    rubrica_data_Modifica,
                                                    rubrica_username_creazione,
                                                    rubrica_username_modifica
                                                )

                                Dummy = objContattixRubrica.Scrivi(
                                                CStr(xContatto.GetAttribute("piva")),
                                                CInt(xContatto.GetAttribute("sa_cod")),
                                                CStr(Cod_Contatto),
                                                Cod_Rubrica,
                                                    CDate(xRubrica.GetAttribute("validita_inizio")),
                                                    CDate(xRubrica.GetAttribute("validita_fine")),
                                                    objParametri,
                                                    rubrica_data_creazione,
                                                    rubrica_data_Modifica,
                                                    rubrica_username_creazione,
                                                    rubrica_username_modifica
                                                )

                            Case "2"    'MODIFICA -------------------------------------------------------

                                ' se il valore è 0 non richiamo il metodo
                                'altrimenti mi genera una eccezione
                                If IsNumeric(xRubrica.GetAttribute("cod_rubrica")) AndAlso
                                    (CInt(xRubrica.GetAttribute("cod_rubrica")) <> 0) Then

                                    objRubrica.Modifica(
                                                    CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                    CStr(xRubrica.GetAttribute("numero")),
                                                    CStr(xRubrica.GetAttribute("descr")),
                                                        CDate(xRubrica.GetAttribute("validita_inizio")),
                                                        CDate(xRubrica.GetAttribute("validita_fine")),
                                                            "",
                                                            objParametri)

                                    objContattixRubrica.Modifica(
                                                    CStr(Cod_Contatto),
                                                    CStr(xContatto.GetAttribute("piva")),
                                                    CInt(xContatto.GetAttribute("sa_cod")),
                                                    CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                        CDate(xRubrica.GetAttribute("validita_inizio")),
                                                        CDate(xRubrica.GetAttribute("validita_fine")),
                                                            "",
                                                            objParametri)


                                End If

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objContattixRubrica.Cancella(
                                                CStr(Cod_Contatto),
                                                CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                    "",
                                                    objParametri)

                                objRubrica.Cancella(
                                                CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                    "",
                                                    objParametri)



                        End Select

                        'Elimino l'oggetto
                        'objRubrica = Nothing
                        'objContattixRubrica = Nothing
                        'Incremento l'indice
                        i_Rubrica = i_Rubrica + 1

                    Loop



                    '-------------------------------------------------------------
                    ' RAPPORTI CONTABILI (RISORSE UMANE)
                    '-------------------------------------------------------------

                    'Prelevo l'elenco dei rapporti contabili
                    xRapCons = xContatto.GetElementsByTagName("RapCon")

                    i_RapCon = 0

                    Dim data_rilascio_patentino As Date
                    Dim data_scadenza_patentino As Date
                    Dim cod_risum_origine As Integer
                    Dim piva_superuser_origine As String
                    Dim Ente_di_rilascio As String
                    Dim ChkSpesometro As Integer
                    Dim Saldo_Iniziale_Crediti As Decimal
                    Dim Saldo_Iniziale_Debiti As Decimal
                    Dim Qualifica_Cod As Integer
                    Dim Mansione_Cod As Integer
                    Dim Classificazione_Cod As Integer
                    Dim Info_Famiglia As String
                    Dim Occasionale As Integer
                    Dim Cod_Iva_Contatto_rc As Integer
                    Dim Cod_Conto_Econ As Integer
                    Dim Cod_Conto_Pat As Integer
                    Dim Sa_Cod_Cod_RisUm As Integer = CInt(xContatto.GetAttribute("sa_cod"))

                    Do While i_RapCon < xRapCons.Count

                        'Prelevo l'i-esimo rapporto contabile
                        xRapCon = xRapCons.Item(i_RapCon)

                        'Prelevo gli attributi del codice selezionato
                        OpeDB_RapCon = xRapCon.GetAttribute("TipoOperazioneDB")

                        'Creo l'oggetto COM
                        objRisorse_Umane = New AgronicaCoreAnagrafeDAL.Risorse_Umane_W

                        Cod_RisUm = CInt(xRapCon.GetAttribute("cod_risum"))
                        If xRapCon.HasAttribute("sa_cod") AndAlso IsNumeric(xRapCon.GetAttribute("sa_cod")) Then
                            Sa_Cod_Cod_RisUm = CInt(xRapCon.GetAttribute("sa_cod"))
                        End If

                        If xRapCon.HasAttribute("data_rilascio_patentino") AndAlso IsDate(xRapCon.GetAttribute("data_rilascio_patentino")) Then
                            data_rilascio_patentino = CDate(xRapCon.GetAttribute("data_rilascio_patentino"))
                        Else
                            data_rilascio_patentino = CDate(xRapCon.GetAttribute("validita_inizio"))
                        End If

                        If xRapCon.HasAttribute("data_scadenza_patentino") AndAlso IsDate(xRapCon.GetAttribute("data_scadenza_patentino")) Then
                            data_scadenza_patentino = CDate(xRapCon.GetAttribute("data_scadenza_patentino"))
                        Else
                            data_scadenza_patentino = CDate(xRapCon.GetAttribute("validita_fine"))
                        End If

                        If xRapCon.HasAttribute("ente_di_rilascio") Then
                            Ente_di_rilascio = CStr(xRapCon.GetAttribute("ente_di_rilascio"))
                        Else
                            Ente_di_rilascio = ""
                        End If

                        If xRapCon.HasAttribute("ChkSpesometro".ToLower) Then
                            ChkSpesometro = CStr(xRapCon.GetAttribute("ChkSpesometro".ToLower))
                        Else
                            ChkSpesometro = 0
                        End If

                        If xRapCon.HasAttribute("Saldo_Iniziale_Crediti".ToLower) Then
                            Saldo_Iniziale_Crediti = CStr(xRapCon.GetAttribute("Saldo_Iniziale_Crediti".ToLower))
                        Else
                            Saldo_Iniziale_Crediti = 0
                        End If

                        If xRapCon.HasAttribute("Saldo_Iniziale_Debiti".ToLower) Then
                            Saldo_Iniziale_Debiti = CStr(xRapCon.GetAttribute("Saldo_Iniziale_Debiti".ToLower))
                        Else
                            Saldo_Iniziale_Debiti = 0
                        End If

                        cod_risum_origine = 0
                        If xRapCon.HasAttribute("cod_risum_origine") Then
                            cod_risum_origine = xRapCon.GetAttribute("cod_risum_origine")
                        End If

                        piva_superuser_origine = ""
                        If xRapCon.HasAttribute("cod_risum_origine") Then
                            piva_superuser_origine = xRapCon.GetAttribute("piva_superuser_origine")
                        End If

                        Qualifica_Cod = 0
                        If xRapCon.HasAttribute("qualifica_cod") Then
                            Qualifica_Cod = xRapCon.GetAttribute("qualifica_cod")
                        End If

                        Mansione_Cod = 0
                        If xRapCon.HasAttribute("mansione_cod") Then
                            Mansione_Cod = xRapCon.GetAttribute("mansione_cod")
                        End If

                        Classificazione_Cod = 0
                        If xRapCon.HasAttribute("classificazione_cod") Then
                            Classificazione_Cod = xRapCon.GetAttribute("classificazione_cod")
                        End If

                        Info_Famiglia = ""
                        If xRapCon.HasAttribute("info_famiglia") Then
                            Info_Famiglia = xRapCon.GetAttribute("info_famiglia")
                        End If

                        Occasionale = 0
                        If xRapCon.HasAttribute("occasionale") Then
                            Occasionale = xRapCon.GetAttribute("occasionale")
                        End If

                        Cod_Iva_Contatto_rc = -1
                        If xRapCon.HasAttribute("cod_iva_contatto") Then
                            Cod_Iva_Contatto_rc = xRapCon.GetAttribute("cod_iva_contatto")
                        End If

                        Cod_Conto_Econ = 0
                        If xRapCon.HasAttribute("cod_conto_econ") Then
                            Cod_Conto_Econ = xRapCon.GetAttribute("cod_conto_econ")
                        End If

                        Cod_Conto_Pat = 0
                        If xRapCon.HasAttribute("cod_conto_pat") Then
                            Cod_Conto_Pat = xRapCon.GetAttribute("cod_conto_pat")
                        End If

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_RapCon

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------


                                If Cod_RisUm <= 0 Then

                                    ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                    'Richiedo un nuovo codice rapporto
                                    Cod_RisUm = ObjSequenze.NuovoId_Tabella(
                                                "Risorse_Umane",
                                                CInt(xContatto.GetAttribute("basecode")),
                                                CInt(xContatto.GetAttribute("topcode")),
                                                    objParametri)
                                    'ObjSequenze = Nothing

                                    xProdotti_Costi = xRapCon.GetElementsByTagName("DatiProdotti_Costi")
                                    If xProdotti_Costi.Count > 0 Then
                                        xProdotto_Costi = xProdotti_Costi.Item(0)
                                        xProdotto_Costi.SetAttribute("mat_cod", Cod_RisUm)
                                    End If

                                Else

                                    'Esportazione in Locale

                                End If

                                Dim rapCon_data_creazione As Date = #2/1/1900#
                                Dim rapCon_data_Modifica As Date = #2/1/1900#
                                Dim rapCon_username_creazione As String = ""
                                Dim rapCon_username_modifica As String = ""

                                If Not IsNothing(xRapCon.GetAttribute("data_creazione")) AndAlso
                                    xRapCon.GetAttribute("data_creazione") <> "" Then
                                    rapCon_data_creazione = CDate(xRapCon.GetAttribute("data_creazione"))
                                End If

                                If Not IsNothing(xRapCon.GetAttribute("data_modifica")) AndAlso
                                    xRapCon.GetAttribute("data_modifica") <> "" Then
                                    rapCon_data_Modifica = CDate(xRapCon.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xRapCon.GetAttribute("username_creazione")) Then
                                    rapCon_username_creazione = CStr(xRapCon.GetAttribute("username_creazione"))
                                End If

                                If Not IsNothing(xRapCon.GetAttribute("username_modifica")) Then
                                    rapCon_username_modifica = CStr(xRapCon.GetAttribute("username_modifica"))
                                End If

                                Dim scrivi As Boolean = True
                                If objRisorse_Umane_R.Leggi(CStr(xContatto.GetAttribute("piva")),
                                                            CStr(Cod_Contatto),
                                                            0,
                                                            CInt(xRapCon.GetAttribute("cod_rapporto")),
                                                            0,
                                                            "",
                                                            False,
                                                            False,
                                                            " Risorse_Umane.Validita_Inizio = " & Agro_SQL_SaveDate(CDate(xRapCon.GetAttribute("validita_inizio")), False) & " And Risorse_Umane.Validita_Fine = " & Agro_SQL_SaveDate(CDate(xRapCon.GetAttribute("validita_fine")), False) & " ",
                                                            "",
                                                            objParametri).Rows.Count > 0 Then
                                    scrivi = False
                                End If

                                If scrivi Then
                                    Dummy = objRisorse_Umane.Scrivi(
                                                                                    CStr(xContatto.GetAttribute("piva")),
                                                                                    CInt(xContatto.GetAttribute("sa_cod")),
                                                                                    CInt(Cod_RisUm),
                                                                                    CStr(Cod_Contatto),
                                                                                    CInt(xRapCon.GetAttribute("cod_rapporto")),
                                                                                    CStr(xRapCon.GetAttribute("settore_des")),
                                                                                    CStr(xRapCon.GetAttribute("attivita_des")),
                                                                                    CDbl(xRapCon.GetAttribute("corrispettivo_mensile")),
                                                                                    CDbl(xRapCon.GetAttribute("corrispettivo_orario")),
                                                                                    CDbl(xRapCon.GetAttribute("ore_settimanali")),
                                                                                    CInt(xRapCon.GetAttribute("giorni_ferie")),
                                                                                    CInt(xRapCon.GetAttribute("ferie_godute")),
                                                                                    CInt(xRapCon.GetAttribute("giorni_malattia")),
                                                                                    Occasionale,
                                                                                    Agro_SQL_SaveText(xRapCon.GetAttribute("patentino"), False),
                                                                                    data_rilascio_patentino,
                                                                                    data_scadenza_patentino,
                                                                                    Ente_di_rilascio,
                                                                                    cod_risum_origine,
                                                                                    piva_superuser_origine,
                                                                                    Saldo_Iniziale_Crediti,
                                                                                    Saldo_Iniziale_Debiti,
                                                                                    ChkSpesometro,
                                                                                        CDate(xRapCon.GetAttribute("validita_inizio")),
                                                                                        CDate(xRapCon.GetAttribute("validita_fine")),
                                                                                            objParametri,
                                                                                    rapCon_data_creazione,
                                                                                    rapCon_data_Modifica,
                                                                                    rapCon_username_creazione,
                                                                                    rapCon_username_modifica,
                                                                                    CInt(Qualifica_Cod),
                                                                                    CInt(Mansione_Cod),
                                                                                    CInt(Classificazione_Cod),
                                                                                    Info_Famiglia,
                                                                                    Cod_Iva_Contatto:=Cod_Iva_Contatto_rc,
                                                                                    Cod_Conto_Econ:=Cod_Conto_Econ,
                                                                                    Cod_Conto_Pat:=Cod_Conto_Pat
                                                                                      )
                                End If



                            Case "2"    'MODIFICA -------------------------------------------------------

                                'modificata il 03/04/2013: leggi commenti nel DAL

                                'CStr(Cod_Contatto), _
                                'CInt(xRapCon.GetAttribute("cod_rapporto")), _
                                'CDbl(xRapCon.GetAttribute("corrispettivo_mensile")), _
                                'CDbl(xRapCon.GetAttribute("corrispettivo_orario")), _

                                objRisorse_Umane.Modifica(CInt(Cod_RisUm),
                                                            CStr(xRapCon.GetAttribute("settore_des")),
                                                            CStr(xRapCon.GetAttribute("attivita_des")),
                                                            CDbl(xRapCon.GetAttribute("ore_settimanali")),
                                                            CInt(xRapCon.GetAttribute("giorni_ferie")),
                                                            CInt(xRapCon.GetAttribute("ferie_godute")),
                                                            CInt(xRapCon.GetAttribute("giorni_malattia")),
                                                            Occasionale,
                                                            Agro_SQL_SaveText(xRapCon.GetAttribute("patentino"), False),
                                                            data_rilascio_patentino,
                                                            data_scadenza_patentino,
                                                            Ente_di_rilascio,
                                                             Saldo_Iniziale_Crediti,
                                                            Saldo_Iniziale_Debiti,
                                                            ChkSpesometro,
                                                                CDate(xRapCon.GetAttribute("validita_inizio")),
                                                                CDate(xRapCon.GetAttribute("validita_fine")),
                                                                    "",
                                                                    objParametri,
                                                            Qualifica_Cod,
                                                            Mansione_Cod,
                                                            Classificazione_Cod,
                                                            Info_Famiglia, Cod_Rapporto:=CInt(xRapCon.GetAttribute("cod_rapporto")),
                                                            Cod_Iva_Contatto:=Cod_Iva_Contatto_rc,
                                                            Cod_Conto_Econ:=Cod_Conto_Econ,
                                                            Cod_Conto_Pat:=Cod_Conto_Pat,
                                                            Sa_Cod:=Sa_Cod_Cod_RisUm
                                                            )


                            Case "3"    'ELIMINA -------------------------------------------------------

                                objRisorse_Umane.Cancella(
                                                CInt(Cod_RisUm),
                                                CStr(Cod_Contatto),
                                                CInt(xRapCon.GetAttribute("cod_rapporto")),
                                                    "",
                                                    objParametri)

                        End Select


                        '#######################################################
                        '##################  PRODOTTI COSTI  ###################
                        '#######################################################

                        'TODO
                        'Prelevo l'elenco dei movimenti
                        xProdotti_Costi = xRapCon.GetElementsByTagName("DatiProdotti_Costi")

                        If xProdotti_Costi.Count > 0 Then

                            xProdotto_Costi = xProdotti_Costi.Item(0)

                            DatiProdotti_Costi = xProdotto_Costi.OuterXml

                            If Not ProdottiCostiCompleta Then
                                DummyProdotti_Costi = ObjProdotti_Costi.Prodotti_Costi_Scrivi(DatiProdotti_Costi,
                                                                                          Cod_RisUm,
                                                                                          objParametri)
                            Else
                                DummyProdotti_Costi = ObjProdotti_Costi.Prodotti_Costi_Completa_Scrivi(DatiProdotti_Costi,
                                                                                            Cod_RisUm,
                                                                                            objParametri)
                            End If

                        End If


                        'Elimino l'oggetto
                        objRisorse_Umane = Nothing
                        'Incremento l'indice
                        i_RapCon = i_RapCon + 1

                    Loop










                    '*********************************************************
                    '*********************************************************
                    '*********************************************************
                    '*********************************************************
                    '*********************************************************
                    '*********************************************************
                    '*********************************************************






                    '-------------------------------------------------------------
                    ' CONTATTI CODICI
                    '-------------------------------------------------------------

                    Dim saCodContatto = CInt(xContatto.GetAttribute("sa_cod"))
                    If saCodContatto = -1 Then
                        saCodContatto = 0
                    End If

                    Dim handleAssociaListiniR As New Listini_PrezzixContatti_R()
                    Dim dtAssocListiniContatto = handleAssociaListiniR.Leggi(
                        OUTPUT_Piva,
                        objParametri,
                        saCod:=saCodContatto,
                        codContatto:=Cod_Contatto,
                        usernameCreazione:="creato_automaticamente")

                    Dim handleAssociaListiniW As New Listini_PrezzixContatti_W()
                    For Each rigaAssocListiniContatto As DataRow In dtAssocListiniContatto.Rows
                        handleAssociaListiniW.Delete(
                            OUTPUT_Piva,
                            rigaAssocListiniContatto("Listino_Cod"),
                            saCodContatto,
                            0,
                            Cod_Contatto,
                            "",
                            objParametri)
                    Next

                    'Prelevo l'elenco dei dati dei codici del contatto
                    xCodici = xContatto.GetElementsByTagName("Contatto_Codice")

                    i_Codici = 0

                    Do While i_Codici < xCodici.Count

                        'Prelevo l'i-esimo codice anagrafe
                        xCodice = xCodici.Item(i_Codici)

                        'Prelevo gli attributi del codice selezionato
                        OpeDB_Codice = xCodice.GetAttribute("TipoOperazioneDB")

                        'Creo l'oggetto COM
                        ObjContatti_Codici = New AgronicaCoreAnagrafeDAL.Contatti_Codici_W

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Codice


                            Case "0"    'LEGGI -------------------------------------------------------


                            Case "1"    'SALVA -------------------------------------------------------

                                Dim codice_data_creazione As Date = #2/1/1900#
                                Dim codice_data_Modifica As Date = #2/1/1900#
                                Dim codice_username_creazione As String = ""
                                Dim codice_username_modifica As String = ""

                                If Not IsNothing(xCodice.GetAttribute("data_creazione")) AndAlso
                                    xCodice.GetAttribute("data_creazione") <> "" Then
                                    codice_data_creazione = CDate(xCodice.GetAttribute("data_creazione"))
                                End If

                                If Not IsNothing(xCodice.GetAttribute("data_modifica")) AndAlso
                                    xCodice.GetAttribute("data_modifica") <> "" Then
                                    codice_data_Modifica = CDate(xCodice.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xCodice.GetAttribute("username_creazione")) Then
                                    codice_username_creazione = CStr(xCodice.GetAttribute("username_creazione"))
                                End If

                                If Not IsNothing(xCodice.GetAttribute("username_modifica")) Then
                                    codice_username_modifica = CStr(xCodice.GetAttribute("username_modifica"))
                                End If

                                'Salvataggio
                                Dummy = ObjContatti_Codici.Scrivi(
                                                CStr(xCodice.GetAttribute("piva")),
                                                CInt(xCodice.GetAttribute("sa_cod")),
                                                Cod_Contatto,
                                                CInt(xCodice.GetAttribute("id_cod")),
                                                CStr(xCodice.GetAttribute("val_cod")),
                                                    CDate(xCodice.GetAttribute("validita_inizio")),
                                                    CDate(xCodice.GetAttribute("validita_fine")),
                                                        objParametri,
                                                codice_data_creazione,
                                                codice_data_Modifica,
                                                codice_username_creazione,
                                                codice_username_modifica
                                        )

                                If (xCodice.GetAttribute("id_cod") = enum_CodiciAnagrafe.ListinoPrezziVenditaDefault OrElse
                                    xCodice.GetAttribute("id_cod") = enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault) AndAlso
                                    Not String.IsNullOrWhiteSpace(xCodice.GetAttribute("val_cod")) AndAlso
                                    xCodice.GetAttribute("val_cod") <> "0" Then

                                    handleAssociaListiniW.Insert(
                                        OUTPUT_Piva,
                                        xCodice.GetAttribute("val_cod"),
                                        saCodContatto,
                                        0,
                                        Cod_Contatto,
                                        "",
                                        objParametri,
                                        "creato_automaticamente")
                                End If

                            Case "2"    'MODIFICA -------------------------------------------------------

                                ObjContatti_Codici.Modifica(
                                                CStr(xCodice.GetAttribute("piva")),
                                                CInt(xCodice.GetAttribute("sa_cod")),
                                                Cod_Contatto,
                                                CInt(xCodice.GetAttribute("id_cod")),
                                                CStr(xCodice.GetAttribute("val_cod")),
                                                    CDate(xCodice.GetAttribute("validita_inizio")),
                                                    CDate(xCodice.GetAttribute("validita_fine")),
                                                        "",
                                                        objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                ObjContatti_Codici.Cancella(
                                                CStr(xCodice.GetAttribute("piva")),
                                                CStr(Cod_Contatto),
                                                CInt(xCodice.GetAttribute("id_cod")),
                                                    "",
                                                    objParametri)

                        End Select

                        'Elimino l'oggetto
                        'ObjContatti_Codici = Nothing
                        'Incremento l'indice
                        i_Codici = i_Codici + 1


                    Loop

                    '-------------------------------------------------------------
                    ' LIQUIDITA
                    '-------------------------------------------------------------

                    'Prelevo l'elenco delle Liquidità e Risorse Finanziarie
                    xLiquiditas = xContatto.GetElementsByTagName("DatiLiquidita")

                    i_Liquidita = 0

                    If Not IsNothing(xLiquiditas) Then

                        Do While i_Liquidita < xLiquiditas.Count

                            xLiquidita = xLiquiditas(i_Liquidita)

                            'Prelevo gli attributi del rapporto selezionato
                            OpeDB_Liquidita = xLiquidita.GetAttribute("TipoOperazioneDB")

                            'Inizializzo Preventivamente il Cod_Liquidita
                            Dim Cod_Liquidita = CLng(xLiquidita.GetAttribute("cod_liquidita"))

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Liquidita

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Richiedo un nuovo codice liquidità
                                    Cod_Liquidita = ObjSequenze.NuovoId_Tabella("Liquidita",
                                                                    CInt(xLiquidita.GetAttribute("basecode")),
                                                                    CInt(xLiquidita.GetAttribute("topcode")),
                                                                    objParametri)

                                    Dummy = ObjLiquidita.Scrivi(CStr(xLiquidita.GetAttribute("piva")),
                                                    CInt(xLiquidita.GetAttribute("sa_cod")),
                                                    Cod_Liquidita,
                                                    CStr(xLiquidita.GetAttribute("cod_contatto")),
                                                    CStr(xLiquidita.GetAttribute("riferimento")),
                                                    CStr(xLiquidita.GetAttribute("cau_risorsa")),
                                                    CInt(xLiquidita.GetAttribute("cod_istituto")),
                                                    CStr(xLiquidita.GetAttribute("numero")),
                                                    CStr(xLiquidita.GetAttribute("abi")),
                                                    CStr(xLiquidita.GetAttribute("cab")),
                                                    CStr(xLiquidita.GetAttribute("cIn")),
                                                    CStr(xLiquidita.GetAttribute("cifre_controllo")),
                                                    CStr(xLiquidita.GetAttribute("nazione")),
                                                    CStr(xLiquidita.GetAttribute("bic")),
                                                    CStr(xLiquidita.GetAttribute("interbancario")),
                                                    CDbl(xLiquidita.GetAttribute("saldo_attuale")),
                                                    CDbl(xLiquidita.GetAttribute("saldo_iniziale")),
                                                    CInt(xLiquidita.GetAttribute("avviso")),
                                                    CDbl(xLiquidita.GetAttribute("importo_avviso")),
                                                    CStr(xLiquidita.GetAttribute("note")),
                                                    CStr(xLiquidita.GetAttribute(LCase("rilevamento"))),
                                                    CStr(xLiquidita.GetAttribute(LCase("data_rilevamento"))),
                                                    CStr(xLiquidita.GetAttribute(LCase("offset"))),
                                                    CStr(xLiquidita.GetAttribute(LCase("chkdefault"))),
                                                    CStr(xLiquidita.GetAttribute(LCase("chkabilitazione"))),
                                                    CDate(xLiquidita.GetAttribute("validita_inizio")),
                                                    CDate(xLiquidita.GetAttribute("validita_fine")),
                                                    "", objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    ObjLiquidita.Modifica(
                                        CStr(xLiquidita.GetAttribute("piva")),
                                        CInt(xLiquidita.GetAttribute("sa_cod")),
                                        Cod_Liquidita,
                                        CStr(xLiquidita.GetAttribute("cod_contatto")),
                                        CStr(xLiquidita.GetAttribute("riferimento")),
                                        CStr(xLiquidita.GetAttribute("cau_risorsa")),
                                        CInt(xLiquidita.GetAttribute("cod_istituto")),
                                        CStr(xLiquidita.GetAttribute("numero")),
                                        CStr(xLiquidita.GetAttribute("abi")),
                                        CStr(xLiquidita.GetAttribute("cab")),
                                        CStr(xLiquidita.GetAttribute("cIn")),
                                        CStr(xLiquidita.GetAttribute("cifre_controllo")),
                                        CStr(xLiquidita.GetAttribute("nazione")),
                                        CStr(xLiquidita.GetAttribute("bic")),
                                        CStr(xLiquidita.GetAttribute("interbancario")),
                                        CDbl(xLiquidita.GetAttribute("saldo_attuale")),
                                        CDbl(xLiquidita.GetAttribute("saldo_iniziale")),
                                        CInt(xLiquidita.GetAttribute("avviso")),
                                        CDbl(xLiquidita.GetAttribute("importo_avviso")),
                                        CStr(xLiquidita.GetAttribute("note")),
                                        CDate(xLiquidita.GetAttribute("validita_inizio")),
                                        CDate(xLiquidita.GetAttribute("validita_fine")),
                                        CInt(xLiquidita.GetAttribute("chkdefault")),
                                        "",
                                        objParametri
                                    )



                                Case "3" 'CANCELLAZIONE

                                    ObjLiquidita.Cancella(
                                        CStr(xLiquidita.GetAttribute("piva")),
                                        CInt(xLiquidita.GetAttribute("sa_cod")),
                                        CInt(xLiquidita.GetAttribute("cod_liquidita")),
                                        xLiquidita.GetAttribute("cod_contatto"), "", objParametri)

                            End Select

                            'Incremento l'indice
                            i_Liquidita = i_Liquidita + 1

                        Loop

                    End If

                    'Elimino l'oggetto
                    ObjLiquidita = Nothing


                    '-------------------------------------------------------------
                    ' CONTI DIRETTAMENTE IMPUTABILI AL CONTATTO
                    '-------------------------------------------------------------

                    ''Prelevo l'elenco dei conti
                    xConti = xContatto.GetElementsByTagName("DatiConti")

                    i_Conti = 0

                    If Not IsNothing(xContatti) Then
                        Do While i_Conti < xConti.Count

                            'Prelevo l'i-esimo rapporto contabile
                            xConto = xConti.Item(i_Conti)

                            'Prelevo gli attributi del rapporto selezionato
                            OPEDB_Conto = xConto.GetAttribute("TipoOperazioneDB")

                            'Verifico l'operazione richiesta
                            Select Case OPEDB_Conto

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dummy = objConti.Scrivi(
                                        CStr(xConto.GetAttribute(LCase("piva"))),
                                        CInt(xConto.GetAttribute("Cod_Conto")),
                                        CStr(xConto.GetAttribute("Cod_Contatto")),
                                        CDate(xConto.GetAttribute("Validita_Inizio")),
                                        CDate(xConto.GetAttribute("Validita_Fine")),
                                        objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                Case "3" 'CANCELLAZIONE

                                    xRisp = objConti.Cancella(
                                        CStr(xConto.GetAttribute(LCase("piva"))),
                                        CInt(xConto.GetAttribute("Cod_Conto")),
                                        CStr(xConto.GetAttribute("Cod_Contatto")),
                                        "", objParametri)

                            End Select

                            i_Conti = i_Conti + 1

                        Loop

                    End If

                    'Elimino l'oggetto
                    objConti = Nothing

                    'TODO
                    '##################################################
                    '##############  PARCO MACCHINE  ##################
                    '##################################################

                    'Prelevo l'elenco delle macchine/attrezzature
                    xParco_Macchine = xContatto.GetElementsByTagName("ParcoMacchina")

                    i_Parco_Macchina = 0

                    Do While i_Parco_Macchina < xParco_Macchine.Count

                        'Prelevo l'i-esima macchina/attrezzatura
                        xParco_Macchina = xParco_Macchine.Item(i_Parco_Macchina)

                        'Prelevo gli attributi della macchina/attrezzatura selezionata
                        OpeDB_Parco_Macchina = xParco_Macchina.GetAttribute("TipoOperazioneDB")

                        'Inizializzo Preventivamente il Codice della Macchina/Attrezzatura
                        Mac_Cod = CInt(xParco_Macchina.GetAttribute("mac_cod"))

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Parco_Macchina

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                'Richiedo un nuovo codice della macchina/attrezzatura

                                If Mac_Cod <= 0 Then

                                    Mac_Cod = ObjSequenze.NuovoId_Tabella("Parco_Macchine",
                                                                          CInt(xParco_Macchina.GetAttribute("basecode")),
                                                                          CInt(xParco_Macchina.GetAttribute("topcode")),
                                                                          objParametri)

                                    ObjSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If

                                'Salvo la macchina/attrezzatura
                                Dummy = ObjParco_Macchine.Scrivi(
                                    CStr(xParco_Macchina.GetAttribute("piva")),
                                                CInt(xParco_Macchina.GetAttribute("sa_cod")),
                                                CInt(Mac_Cod),
                                                If(Not xParco_Macchina.HasAttribute("cod_contatto"), "", xParco_Macchina.GetAttribute("cod_contatto")),
                                                CStr(xParco_Macchina.GetAttribute("class_code")),
                                                CInt(xParco_Macchina.GetAttribute("tipo")),
                                                CStr(xParco_Macchina.GetAttribute("mac_des")),
                                                CDbl(xParco_Macchina.GetAttribute("costo_acquisto")),
                                                CStr(xParco_Macchina.GetAttribute("targa")),
                                                CStr(xParco_Macchina.GetAttribute("telaio")),
                                                CInt(xParco_Macchina.GetAttribute("ditta_cod")),
                                                CStr(xParco_Macchina.GetAttribute("modello")),
                                                CStr(xParco_Macchina.GetAttribute("potenza")),
                                                CDbl(xParco_Macchina.GetAttribute("ammortamento")),
                                                CDate(xParco_Macchina.GetAttribute("data_immatricolazione")),
                                                CDate(xParco_Macchina.GetAttribute("ultima_manutenzione")),
                                                CDate(xParco_Macchina.GetAttribute("ultima_revisione")),
                                                CStr(xParco_Macchina.GetAttribute("stato_utilizzo")),
                                                If(Not xParco_Macchina.HasAttribute("note"), "", xParco_Macchina.GetAttribute("note")),
                                                CStr(xParco_Macchina.GetAttribute("n_immatricolazione")),
                                                CStr(xParco_Macchina.GetAttribute("n_immatricolazione_rimorchio")),
                                                CStr(xParco_Macchina.GetAttribute("n_autorizzazione_trasporto")),
                                                CDate(xParco_Macchina.GetAttribute("data_rilascio_autorizzazione")),
                                                CDbl(xParco_Macchina.GetAttribute("peso")),
                                                If(IsNumeric(xParco_Macchina.GetAttribute("portata_max")), xParco_Macchina.GetAttribute("portata_max"), 0),
                                                If(IsNumeric(xParco_Macchina.GetAttribute("chkdefault")), xParco_Macchina.GetAttribute("chkdefault"), 0),
                                                CInt(xParco_Macchina.GetAttribute("alimentazione_cod")),
                                                CInt(xParco_Macchina.GetAttribute("potenza_udm_cod")),
                                                CInt(xParco_Macchina.GetAttribute("mac_cod_origine")),
                                                CStr(xParco_Macchina.GetAttribute("piva_superuser_origine")),
                                                Agro_XML_GetString(xParco_Macchina, "cuaa_proprietario", ""),
                                                Agro_XML_GetString(xParco_Macchina, "denominazione_proprietario", ""),
                                                Agro_XML_GetInteger(xParco_Macchina, "tipo_targa_cod", 0),
                                                Agro_XML_GetInteger(xParco_Macchina, "tipo_trazione", 0),
                                                Agro_XML_GetString(xParco_Macchina, "n_omologazione", ""),
                                                Agro_XML_GetInteger(xParco_Macchina, "ditta_cod_motore", 0),
                                                Agro_XML_GetString(xParco_Macchina, "tipo_motore", ""),
                                                Agro_XML_GetString(xParco_Macchina, "matricola_motore", ""),
                                                Agro_XML_GetDate(xParco_Macchina, "data_reimmatricolazione", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xParco_Macchina, "data_carico", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xParco_Macchina, "data_scarico", AGRODATAFINE),
                                                Agro_XML_GetInteger(xParco_Macchina, "titolopossesso", 0),
                                                Agro_XML_GetString(xParco_Macchina, "flag_attrezzatura_macchina", ""),
                                                CDate(xParco_Macchina.GetAttribute("validita_inizio")),
                                                CDate(xParco_Macchina.GetAttribute("validita_fine")),
                                                objParametri, Agro_XML_GetDecimal(xParco_Macchina, "taratura_ugello", 0))

                            Case "2"    'MODIFICA -------------------------------------------------------

                                ObjParco_Macchine.Modifica(
                                    CStr(xParco_Macchina.GetAttribute("piva")),
                                                CInt(xParco_Macchina.GetAttribute("sa_cod")),
                                                CInt(Mac_Cod),
                                                If(Not xParco_Macchina.HasAttribute("cod_contatto"), "", xParco_Macchina.GetAttribute("cod_contatto")),
                                                CStr(xParco_Macchina.GetAttribute("class_code")),
                                                CInt(xParco_Macchina.GetAttribute("tipo")),
                                                CStr(xParco_Macchina.GetAttribute("mac_des")),
                                                CDbl(xParco_Macchina.GetAttribute("costo_acquisto")),
                                                CStr(xParco_Macchina.GetAttribute("targa")),
                                                CStr(xParco_Macchina.GetAttribute("telaio")),
                                                CInt(xParco_Macchina.GetAttribute("ditta_cod")),
                                                CStr(xParco_Macchina.GetAttribute("modello")),
                                                CStr(xParco_Macchina.GetAttribute("potenza")),
                                                CDbl(xParco_Macchina.GetAttribute("ammortamento")),
                                                CDate(xParco_Macchina.GetAttribute("data_immatricolazione")),
                                                CDate(xParco_Macchina.GetAttribute("ultima_manutenzione")),
                                                CDate(xParco_Macchina.GetAttribute("ultima_revisione")),
                                                CStr(xParco_Macchina.GetAttribute("stato_utilizzo")),
                                                If(Not xParco_Macchina.HasAttribute("note"), "", xParco_Macchina.GetAttribute("note")),
                                                CStr(xParco_Macchina.GetAttribute("n_immatricolazione")),
                                                CStr(xParco_Macchina.GetAttribute("n_immatricolazione_rimorchio")),
                                                CStr(xParco_Macchina.GetAttribute("n_autorizzazione_trasporto")),
                                                CDate(xParco_Macchina.GetAttribute("data_rilascio_autorizzazione")),
                                                CDbl(xParco_Macchina.GetAttribute("peso")),
                                                If(IsNumeric(xParco_Macchina.GetAttribute("portata_max")), xParco_Macchina.GetAttribute("portata_max"), 0),
                                                If(IsNumeric(xParco_Macchina.GetAttribute("chkdefault")), xParco_Macchina.GetAttribute("chkdefault"), 0),
                                                CInt(xParco_Macchina.GetAttribute("alimentazione_cod")),
                                                CInt(xParco_Macchina.GetAttribute("potenza_udm_cod")),
                                                Agro_XML_GetString(xParco_Macchina, "cuaa_proprietario", ""),
                                                Agro_XML_GetString(xParco_Macchina, "denominazione_proprietario", ""),
                                                Agro_XML_GetInteger(xParco_Macchina, "tipo_targa_cod", 0),
                                                Agro_XML_GetInteger(xParco_Macchina, "tipo_trazione", 0),
                                                Agro_XML_GetString(xParco_Macchina, "n_omologazione", ""),
                                                Agro_XML_GetInteger(xParco_Macchina, "ditta_cod_motore", 0),
                                                Agro_XML_GetString(xParco_Macchina, "tipo_motore", ""),
                                                Agro_XML_GetString(xParco_Macchina, "matricola_motore", ""),
                                                Agro_XML_GetDate(xParco_Macchina, "data_reimmatricolazione", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xParco_Macchina, "data_carico", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xParco_Macchina, "data_scarico", AGRODATAFINE),
                                                Agro_XML_GetInteger(xParco_Macchina, "titolopossesso", 0),
                                                Agro_XML_GetString(xParco_Macchina, "flag_attrezzatura_macchina", ""),
                                                CDate(xParco_Macchina.GetAttribute("validita_inizio")),
                                                CDate(xParco_Macchina.GetAttribute("validita_fine")),
                                                "", objParametri, Agro_XML_GetDecimal(xParco_Macchina, "taratura_ugello", 0))


                            Case "3"    'ELIMINA -------------------------------------------------------

                                ObjParco_Macchine.Cancella(CStr(xParco_Macchina.GetAttribute("piva")),
                                                            Mac_Cod,
                                                            Cod_Contatto,
                                                            "", objParametri)

                        End Select


                        i_Parco_Macchina = i_Parco_Macchina + 1

                    Loop


                    ObjParco_Macchine = Nothing



                    '########################################################################################################
                    '########################################################################################################
                    '########################################################################################################
                    '########################################################################################################



                    Select Case OpeDB_Contatto

                        Case "2" '-


                        Case "3" 'CANCELLAZIONE CONTATTO

                            xPiva = CStr(xContatto.GetAttribute("piva"))
                            xCod_Contatto = Cod_Contatto


                            'TODO
                            'Cancellazione Liquidità
                            ''''ObjLiquidita_AD = CreateObject("Agro_Contab_AD.Liquidita_W")

                            ''''ObjLiquidita_AD.Cancella( _
                            ''''    CStr(xContatto.GetAttribute("piva")), _
                            ''''    0, _
                            ''''    0, _
                            ''''    CStr(Cod_Contatto), _
                            ''''    CStr(Utente), _
                            ''''    objCnManager, _
                            ''''    ConnessioneAlternativa)

                            ''''ObjLiquidita_AD = Nothing


                            'Cancellazione Codici Associati al Contatto
                            ObjContatti_Codici = New AgronicaCoreAnagrafeDAL.Contatti_Codici_W

                            ObjContatti_Codici.Cancella(
                                                CStr(xContatto.GetAttribute("piva")),
                                                CStr(Cod_Contatto),
                                                0,
                                                    "",
                                                    objParametri)


                            'ObjContatti_Codici = Nothing

                            'CANCELLAZIONE CONTATTO
                            objContatti.Cancella(
                                                CStr(xContatto.GetAttribute("piva")),
                                                CStr(xContatto.GetAttribute("cod_contatto")),
                                                    "",
                                                    objParametri)

                            objLog.Scrivi(CInt(OpeDB_Contatto), Piva, Cod_Contatto, Sa_Cod, Id_CF, ContattoDes, NoteXLog, enum_Id_Servizio.GiasOnline, objParametri, DatiContatto, Origine)


                    End Select

                    'Elimino l'oggetto
                    objContatti = Nothing


                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Contatto = i_Contatto + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiContatto = i_DatiContatto + 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xLiquidita = Nothing
            xLiquiditas = Nothing
            xConto = Nothing
            xConti = Nothing
            xRapCon = Nothing
            xRapCons = Nothing
            xRubrica = Nothing
            xRubriche = Nothing
            xParco_Macchine = Nothing
            xParco_Macchina = Nothing
            xIndirizzo = Nothing
            xIndirizzi = Nothing
            xContatto = Nothing
            xContatti = Nothing
            xDatiContatto = Nothing
            xDatiContatti = Nothing
            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            'If Not IsNothing(Cod_Contatto) Then
            '    Contatto_Scrivi = Cod_Contatto
            'End If

            'Restituisco un valore Dummy
            xRisp = True


            ''Se ho la transazione è stata avviata in questa routine faccio il commit
            'If FlagTransazioneLocale = True Then
            '    'objParametri.objTransazione.Commit()
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
            'End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Cod_Contatto=" & CStr(OUTPUT_Cod_Contatto) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If

        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_Contatti(piva As String, ByRef righeInseriteArray As JArray, ByRef righeModificateArray As JArray, ByRef righeCancellateArray As JArray, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "Contatti_W.Aggiorna_Contatti()"

        Try

            Dim c_R As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim curContatto As New Contatti

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeInseriteArray)
            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeModificateArray)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray

                curContatto = New Contatti

                curContatto.Piva = If(Not String.IsNullOrEmpty(obj("Piva")), CStr(obj("Piva")), piva) ''''''''''''''
                curContatto.Sa_Cod = If(Not String.IsNullOrEmpty(obj("Sa_Cod")), CInt(obj("Sa_Cod")), 0)
                curContatto.Cod_Contatto = If(Not String.IsNullOrEmpty(obj("Cod_Contatto")), CStr(obj("Cod_Contatto")), "")
                curContatto.Id_CF = If(Not String.IsNullOrEmpty(obj("Id_CF")), CInt(obj("Id_CF")), 0)
                curContatto.Rag_Soc = If(Not String.IsNullOrEmpty(obj("Rag_Soc")), CStr(obj("Rag_Soc")), "")
                curContatto.Convenevoli = If(Not String.IsNullOrEmpty(obj("Convenevoli")), CStr(obj("Convenevoli")), "")
                curContatto.Codice_Fiscale = If(Not String.IsNullOrEmpty(obj("Codice_Fiscale")), CStr(obj("Codice_Fiscale")), "")
                curContatto.Tipo_Indirizzo_Default = If(Not String.IsNullOrEmpty(obj("Tipo_Indirizzo_Default")), CInt(obj("Tipo_Indirizzo_Default")), 0)
                curContatto.Nome = If(Not String.IsNullOrEmpty(obj("Nome")), CStr(obj("Nome")), "")
                curContatto.Cognome = If(Not String.IsNullOrEmpty(obj("Cognome")), CStr(obj("Cognome")), "")
                curContatto.Data_Nascita = If(Not String.IsNullOrEmpty(Trim(obj("Data_Nascita"))), AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Nascita")), AGRODATAINIZIO)
                curContatto.Sesso = If(Not String.IsNullOrEmpty(obj("Sesso")), CStr(obj("Sesso")), "")
                curContatto.Cod_Contatto_Referente = If(Not String.IsNullOrEmpty(obj("Cod_Contatto_Referente")), CStr(obj("Cod_Contatto_Referente")), "")
                curContatto.Tipo_Speditore = If(Not String.IsNullOrEmpty(obj("Tipo_Speditore")), CInt(obj("Tipo_Speditore")), 0)
                curContatto.Tipo_Destinazione = If(Not String.IsNullOrEmpty(obj("Tipo_Destinazione")), CInt(obj("Tipo_Destinazione")), 0)
                curContatto.Agente_Cod = If(Not String.IsNullOrEmpty(obj("Agente_Cod")), CInt(obj("Agente_Cod")), 0)
                curContatto.Provvigione = If(Not String.IsNullOrEmpty(obj("Provvigione")), CDbl(obj("Provvigione")), 0)
                curContatto.Note = If(Not String.IsNullOrEmpty(obj("Note")), CStr(obj("Note")), "")
                curContatto.Id_Gestione_Note = If(Not String.IsNullOrEmpty(obj("Id_Gestione_Note")), CInt(obj("Id_Gestione_Note")), 0)
                curContatto.Note2 = If(Not String.IsNullOrEmpty(obj("Note2")), CStr(obj("Note2")), "")
                curContatto.Note_Operazioni = If(Not String.IsNullOrEmpty(obj("Note_Operazioni")), CStr(obj("Note_Operazioni")), "")
                curContatto.Note2_Operazioni = If(Not String.IsNullOrEmpty(obj("Note2_Operazioni")), CStr(obj("Note2_Operazioni")), "")
                curContatto.Cod_Risum_Destinazione_Diversa = If(Not String.IsNullOrEmpty(obj("Cod_Risum_Destinazione_Diversa")), CInt(obj("Cod_Risum_Destinazione_Diversa")), 0)
                curContatto.Tipo_Indirizzo_Default_Destinazione_Diversa = If(Not String.IsNullOrEmpty(obj("Tipo_Indirizzo_Default_Destinazione_Diversa")), CInt(obj("Tipo_Indirizzo_Default_Destinazione_Diversa")), 0)
                curContatto.Fido = If(Not String.IsNullOrEmpty(obj("Fido")), CDbl(obj("Fido")), 0)
                curContatto.Limite_Posizioni = If(Not String.IsNullOrEmpty(obj("Limite_Posizioni")), CInt(obj("Limite_Posizioni")), 0)
                curContatto.Limite_Giorni_Evasione = If(Not String.IsNullOrEmpty(obj("Limite_Giorni_Evasione")), CDbl(obj("Limite_Giorni_Evasione")), 0)
                curContatto.Orari_Ritiro = If(Not String.IsNullOrEmpty(obj("Orari_Ritiro")), CStr(obj("Orari_Ritiro")), "")
                curContatto.Filtro_Rimborsi = If(Not String.IsNullOrEmpty(obj("Filtro_Rimborsi")), CStr(obj("Filtro_Rimborsi")), "")
                curContatto.Vettore_Cod = If(Not String.IsNullOrEmpty(obj("Vettore_Cod")), CInt(obj("Vettore_Cod")), 0)
                curContatto.CapoArea_Cod = If(Not String.IsNullOrEmpty(obj("CapoArea_Cod")), CInt(obj("CapoArea_Cod")), 0)
                curContatto.Provvigione_CapoArea = If(Not String.IsNullOrEmpty(obj("Provvigione_CapoArea")), CDbl(obj("Provvigione_CapoArea")), 0)
                curContatto.Memo = If(Not String.IsNullOrEmpty(obj("Memo")), CStr(obj("Memo")), "")
                curContatto.Sconto_Contatto = If(Not String.IsNullOrEmpty(obj("Sconto_Contatto")), CDbl(obj("Sconto_Contatto")), 0)
                curContatto.Sconto_Testo = If(Not String.IsNullOrEmpty(obj("Sconto_Testo")), CStr(obj("Sconto_Testo")), "")
                curContatto.Modalita_Fatturazione = If(Not String.IsNullOrEmpty(obj("Modalita_Fatturazione")), CInt(obj("Modalita_Fatturazione")), 0)
                curContatto.Cod_Iva_Contatto = If(Not String.IsNullOrEmpty(obj("Cod_Iva_Contatto")), CInt(obj("Cod_Iva_Contatto")), -1) ''''''''''''''

                curContatto.Cod_Conto_Econ = If(Not String.IsNullOrEmpty(obj("Cod_Conto_Econ")), CInt(obj("Cod_Conto_Econ")), 0) ''''''''''''''
                curContatto.Cod_Conto_Pat = If(Not String.IsNullOrEmpty(obj("Cod_Conto_Pat")), CInt(obj("Cod_Conto_Pat")), 0) ''''''''''''''

                curContatto.Documento = If(Not String.IsNullOrEmpty(obj("Documento")), CStr(obj("Documento")), curContatto.Documento) ''''''''''''''
                curContatto.dtDocumento = If(Not String.IsNullOrEmpty(obj("dtDocumento")), CDate(obj("dtDocumento")), curContatto.dtDocumento) ''''''''''''''
                curContatto.dtFonte = If(Not String.IsNullOrEmpty(obj("dtFonte")), CDate(obj("dtFonte")), curContatto.dtFonte) ''''''''''''''
                curContatto.flagReferente = If(Not String.IsNullOrEmpty(obj("flagReferente")), CStr(obj("flagReferente")), curContatto.flagReferente) ''''''''''''''
                curContatto.fonte = If(Not String.IsNullOrEmpty(obj("fonte")), CStr(obj("fonte")), curContatto.fonte) ''''''''''''''
                curContatto.fonteDescr = If(Not String.IsNullOrEmpty(obj("fonteDescr")), CStr(obj("fonteDescr")), curContatto.fonteDescr) ''''''''''''''
                curContatto.AlboProfessionale = If(Not String.IsNullOrEmpty(obj("AlboProfessionale")), CStr(obj("AlboProfessionale")), curContatto.AlboProfessionale) ''''''''''''''
                curContatto.AlboProfessionaleDescr = If(Not String.IsNullOrEmpty(obj("AlboProfessionaleDescr")), CStr(obj("AlboProfessionaleDescr")), curContatto.AlboProfessionaleDescr) ''''''''''''''
                curContatto.numeroIscrizione = If(Not String.IsNullOrEmpty(obj("numeroIscrizione")), CInt(obj("numeroIscrizione")), curContatto.numeroIscrizione) ''''''''''''''
                curContatto.Qualifica = If(Not String.IsNullOrEmpty(obj("Qualifica")), CStr(obj("Qualifica")), curContatto.Qualifica) ''''''''''''''
                curContatto.QualificaDescr = If(Not String.IsNullOrEmpty(obj("QualificaDescr")), CStr(obj("QualificaDescr")), curContatto.QualificaDescr) ''''''''''''''
                curContatto.TitoloStudio = If(Not String.IsNullOrEmpty(obj("TitoloStudio")), CStr(obj("TitoloStudio")), curContatto.TitoloStudio) ''''''''''''''
                curContatto.TitoloStudioDescr = If(Not String.IsNullOrEmpty(obj("TitoloStudioDescr")), CStr(obj("TitoloStudioDescr")), curContatto.TitoloStudioDescr) ''''''''''''''

                curContatto.Data_Creazione = DateTime.Now
                curContatto.Username_Creazione = objParametri_Server.UsernameOperazione
                curContatto.Data_Modifica = DateTime.Now
                curContatto.Username_Modifica = objParametri_Server.UsernameOperazione
                curContatto.Inviato = 0
                curContatto.Validita_Inizio = If(Not String.IsNullOrEmpty(obj("Validita_Inizio")), CDate(obj("Validita_Inizio")), AGRODATAINIZIO)
                curContatto.Validita_Fine = If(Not String.IsNullOrEmpty(obj("Validita_Fine")), CDate(obj("Validita_Fine")), AGRODATAFINE)

                EFArrayToInsert.Add(curContatto)
            Next
            For Each obj As JObject In righeModificateArray
                curContatto = c_R.Leggi_Contatti(piva, obj("Sa_Cod"), obj("Cod_Contatto"), objParametri_Server)
                If curContatto Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & piva & " - " & obj("Sa_Cod").ToString & " - " & obj("Cod_Contatto").ToString & " non trovata"
                Else

                    'curContatto.Piva = If(Not String.IsNullOrEmpty(obj("Piva")), CStr(obj("Piva")), piva) ''''''''''''''
                    'curContatto.Sa_Cod = If(Not String.IsNullOrEmpty(obj("Sa_Cod")), CInt(obj("Sa_Cod")), 0)
                    'curContatto.Cod_Contatto = If(Not String.IsNullOrEmpty(obj("Cod_Contatto")), CStr(obj("Cod_Contatto")), "")
                    curContatto.Id_CF = If(Not String.IsNullOrEmpty(obj("Id_CF")), CInt(obj("Id_CF")), curContatto.Id_CF)
                    curContatto.Rag_Soc = If(Not String.IsNullOrEmpty(obj("Rag_Soc")), CStr(obj("Rag_Soc")), curContatto.Rag_Soc)
                    curContatto.Convenevoli = If(Not String.IsNullOrEmpty(obj("Convenevoli")), CStr(obj("Convenevoli")), curContatto.Convenevoli)
                    'curContatto.Codice_Fiscale = If(Not String.IsNullOrEmpty(obj("Codice_Fiscale")), CStr(obj("Codice_Fiscale")), "")
                    curContatto.Tipo_Indirizzo_Default = If(Not String.IsNullOrEmpty(obj("Tipo_Indirizzo_Default")), CInt(obj("Tipo_Indirizzo_Default")), curContatto.Tipo_Indirizzo_Default)
                    curContatto.Nome = If(Not String.IsNullOrEmpty(obj("Nome")), CStr(obj("Nome")), curContatto.Nome)
                    curContatto.Cognome = If(Not String.IsNullOrEmpty(obj("Cognome")), CStr(obj("Cognome")), curContatto.Cognome)
                    curContatto.Data_Nascita = If(Not String.IsNullOrEmpty(Trim(obj("Data_Nascita"))), AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Nascita")), curContatto.Data_Nascita)
                    curContatto.Sesso = If(Not String.IsNullOrEmpty(obj("Sesso")), CStr(obj("Sesso")), curContatto.Sesso)
                    curContatto.Cod_Contatto_Referente = If(Not String.IsNullOrEmpty(obj("Cod_Contatto_Referente")), CStr(obj("Cod_Contatto_Referente")), curContatto.Cod_Contatto_Referente)
                    curContatto.Tipo_Speditore = If(Not String.IsNullOrEmpty(obj("Tipo_Speditore")), CInt(obj("Tipo_Speditore")), curContatto.Tipo_Speditore)
                    curContatto.Tipo_Destinazione = If(Not String.IsNullOrEmpty(obj("Tipo_Destinazione")), CInt(obj("Tipo_Destinazione")), curContatto.Tipo_Destinazione)
                    curContatto.Agente_Cod = If(Not String.IsNullOrEmpty(obj("Agente_Cod")), CInt(obj("Agente_Cod")), curContatto.Agente_Cod)
                    curContatto.Provvigione = If(Not String.IsNullOrEmpty(obj("Provvigione")), CDbl(obj("Provvigione")), curContatto.Provvigione)
                    curContatto.Note = If(Not String.IsNullOrEmpty(obj("Note")), CStr(obj("Note")), curContatto.Note)
                    curContatto.Id_Gestione_Note = If(Not String.IsNullOrEmpty(obj("Id_Gestione_Note")), CInt(obj("Id_Gestione_Note")), curContatto.Id_Gestione_Note)
                    curContatto.Note2 = If(Not String.IsNullOrEmpty(obj("Note2")), CStr(obj("Note2")), curContatto.Note2)
                    curContatto.Note_Operazioni = If(Not String.IsNullOrEmpty(obj("Note_Operazioni")), CStr(obj("Note_Operazioni")), curContatto.Note_Operazioni)
                    curContatto.Note2_Operazioni = If(Not String.IsNullOrEmpty(obj("Note2_Operazioni")), CStr(obj("Note2_Operazioni")), curContatto.Note2_Operazioni)
                    curContatto.Cod_Risum_Destinazione_Diversa = If(Not String.IsNullOrEmpty(obj("Cod_Risum_Destinazione_Diversa")), CInt(obj("Cod_Risum_Destinazione_Diversa")), curContatto.Cod_Risum_Destinazione_Diversa)
                    curContatto.Tipo_Indirizzo_Default_Destinazione_Diversa = If(Not String.IsNullOrEmpty(obj("Tipo_Indirizzo_Default_Destinazione_Diversa")), CInt(obj("Tipo_Indirizzo_Default_Destinazione_Diversa")), curContatto.Tipo_Indirizzo_Default_Destinazione_Diversa)
                    curContatto.Fido = If(Not String.IsNullOrEmpty(obj("Fido")), CDbl(obj("Fido")), curContatto.Fido)
                    curContatto.Limite_Posizioni = If(Not String.IsNullOrEmpty(obj("Limite_Posizioni")), CInt(obj("Limite_Posizioni")), curContatto.Limite_Posizioni)
                    curContatto.Limite_Giorni_Evasione = If(Not String.IsNullOrEmpty(obj("Limite_Giorni_Evasione")), CDbl(obj("Limite_Giorni_Evasione")), curContatto.Limite_Giorni_Evasione)
                    curContatto.Orari_Ritiro = If(Not String.IsNullOrEmpty(obj("Orari_Ritiro")), CStr(obj("Orari_Ritiro")), curContatto.Orari_Ritiro)
                    curContatto.Filtro_Rimborsi = If(Not String.IsNullOrEmpty(obj("Filtro_Rimborsi")), CStr(obj("Filtro_Rimborsi")), curContatto.Filtro_Rimborsi)
                    curContatto.Vettore_Cod = If(Not String.IsNullOrEmpty(obj("Vettore_Cod")), CInt(obj("Vettore_Cod")), curContatto.Vettore_Cod)
                    curContatto.CapoArea_Cod = If(Not String.IsNullOrEmpty(obj("CapoArea_Cod")), CInt(obj("CapoArea_Cod")), curContatto.CapoArea_Cod)
                    curContatto.Provvigione_CapoArea = If(Not String.IsNullOrEmpty(obj("Provvigione_CapoArea")), CDbl(obj("Provvigione_CapoArea")), curContatto.Provvigione_CapoArea)
                    curContatto.Memo = If(Not String.IsNullOrEmpty(obj("Memo")), CStr(obj("Memo")), curContatto.Memo)
                    curContatto.Sconto_Contatto = If(Not String.IsNullOrEmpty(obj("Sconto_Contatto")), CDbl(obj("Sconto_Contatto")), curContatto.Sconto_Contatto)
                    curContatto.Sconto_Testo = If(Not String.IsNullOrEmpty(obj("Sconto_Testo")), CStr(obj("Sconto_Testo")), curContatto.Sconto_Testo)
                    curContatto.Modalita_Fatturazione = If(Not String.IsNullOrEmpty(obj("Modalita_Fatturazione")), CInt(obj("Modalita_Fatturazione")), curContatto.Modalita_Fatturazione)
                    curContatto.Cod_Iva_Contatto = If(Not String.IsNullOrEmpty(obj("Cod_Iva_Contatto")), CInt(obj("Cod_Iva_Contatto")), curContatto.Cod_Iva_Contatto) ''''''''''''''

                    curContatto.Cod_Conto_Econ = If(Not String.IsNullOrEmpty(obj("Cod_Conto_Econ")), CInt(obj("Cod_Conto_Econ")), curContatto.Cod_Conto_Econ) ''''''''''''''
                    curContatto.Cod_Conto_Pat = If(Not String.IsNullOrEmpty(obj("Cod_Conto_Pat")), CInt(obj("Cod_Conto_Pat")), curContatto.Cod_Conto_Pat) ''''''''''''''

                    curContatto.Documento = If(Not String.IsNullOrEmpty(obj("Documento")), CStr(obj("Documento")), curContatto.Documento) ''''''''''''''
                    curContatto.dtDocumento = If(Not String.IsNullOrEmpty(obj("dtDocumento")), CDate(obj("dtDocumento")), curContatto.dtDocumento) ''''''''''''''
                    curContatto.dtFonte = If(Not String.IsNullOrEmpty(obj("dtFonte")), CDate(obj("dtFonte")), curContatto.dtFonte) ''''''''''''''
                    curContatto.flagReferente = If(Not String.IsNullOrEmpty(obj("flagReferente")), CStr(obj("flagReferente")), curContatto.flagReferente) ''''''''''''''
                    curContatto.fonte = If(Not String.IsNullOrEmpty(obj("fonte")), CStr(obj("fonte")), curContatto.fonte) ''''''''''''''
                    curContatto.fonteDescr = If(Not String.IsNullOrEmpty(obj("fonteDescr")), CStr(obj("fonteDescr")), curContatto.fonteDescr) ''''''''''''''
                    curContatto.AlboProfessionale = If(Not String.IsNullOrEmpty(obj("AlboProfessionale")), CStr(obj("AlboProfessionale")), curContatto.AlboProfessionale) ''''''''''''''
                    curContatto.AlboProfessionaleDescr = If(Not String.IsNullOrEmpty(obj("AlboProfessionaleDescr")), CStr(obj("AlboProfessionaleDescr")), curContatto.AlboProfessionaleDescr) ''''''''''''''
                    curContatto.numeroIscrizione = If(Not String.IsNullOrEmpty(obj("numeroIscrizione")), CInt(obj("numeroIscrizione")), curContatto.numeroIscrizione) ''''''''''''''
                    curContatto.Qualifica = If(Not String.IsNullOrEmpty(obj("Qualifica")), CStr(obj("Qualifica")), curContatto.Qualifica) ''''''''''''''
                    curContatto.QualificaDescr = If(Not String.IsNullOrEmpty(obj("QualificaDescr")), CStr(obj("QualificaDescr")), curContatto.QualificaDescr) ''''''''''''''
                    curContatto.TitoloStudio = If(Not String.IsNullOrEmpty(obj("TitoloStudio")), CStr(obj("TitoloStudio")), curContatto.TitoloStudio) ''''''''''''''
                    curContatto.TitoloStudioDescr = If(Not String.IsNullOrEmpty(obj("TitoloStudioDescr")), CStr(obj("TitoloStudioDescr")), curContatto.TitoloStudioDescr) ''''''''''''''

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curContatto.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curContatto.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If

                    curContatto.Data_Modifica = Date.Now
                    curContatto.Username_Modifica = objParametri_Server.UsernameOperazione
                    EFArrayToUpdate.Add(curContatto)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curContatto = New Contatti
                curContatto.Piva = obj("piva")
                curContatto.Sa_Cod = obj("Sa_Cod")
                curContatto.Cod_Contatto = obj("Cod_Contatto")
                EFArrayToDelete.Add(curContatto)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim c_W As New AgronicaCoreAnagrafeDAL.Contatti_W

                MessaggioErrore = c_W.Aggiorna_Contatti(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri_Server
                 )

            End If


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message & vbCrLf & "InnerException:" & CStr(ex.InnerException.Message)
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore

    End Function


    '################################################################################################################################################################################
    '################################################################################################################################################################################
    '################################################################################################################################################################################

    'Private Function Scrivi_Contatto(Contatto As AgronicaCoreModelsSTD.anagrafiche.Contatto,
    '                                 ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                 ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
    '                                 ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
    '                                 Elemento_Anagrafico As enum_EntitaAlberoImprese,
    '                                 ByRef objParametri_Server As AgronicaCoreParametri,
    '                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

    '    Dim Cod_Risum As Integer

    '    Dim RisorseUmaneEF As AgronicaCoreEntityFramework_POCO.Risorse_Umane
    '    Dim ContattiEF As AgronicaCoreEntityFramework_POCO.Contatti

    '    Dim piva = ContattiEF.Piva
    '    Dim cod_contatto = ContattiEF.Cod_Contatto
    '    Dim cod_rapporto = RisorseUmaneEF.Cod_Rapporto

    '    ContattiEF = (From cont In GiasContext.Contatti
    '                    Where cont.Piva = piva AndAlso
    '                          cont.Cod_Contatto = cod_contatto)

    '    RisorseUmaneEF = (From cont In GiasContext.Risorse_Umane Where
    '                                                    cont.Piva = piva AndAlso
    '                                                    cont.Cod_Contatto = cod_contatto AndAlso
    '                                                    cont.Cod_Rapporto = cod_rapporto)

    '    If ContattiEF Is Nothing Then

    '        If RisorseUmaneEF Is Nothing Then

    '            Select Case Elemento_Anagrafico
    '                Case enum_EntitaAlberoImprese.Impresa
    '                    ContattiEF = AgronicaCoreAnagrafeDAL.EFImprese.CreaContattoImpresa(GiasContext, objParametri_Server, Impresa, cod_contatto, 0, "id_cf", objParametri_Utenti.UsernameOperazione)
    '                    RisorseUmaneEF = AgronicaCoreAnagrafeDAL.EFImprese.CreaRisorseUmaneImpresa(GiasContext, objParametri_Server, Impresa, ContattiEF, objParametri_Utenti.UsernameOperazione)
    '                Case enum_EntitaAlberoImprese.Centro
    '                    '.....
    '                    '......

    '                Case Else
    '                    Throw New Exception("Scrittura Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
    '            End Select



    '        End If
    '    Else
    '        '----
    '    End If



    '    Return Cod_Risum

    'End Function

    'Public Function Scrivi_Contatto_Impresa(Contatto As AgronicaCoreModelsSTD.anagrafiche.Contatto,
    '                                        ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                        ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
    '                                        ByRef objParametri_Server As AgronicaCoreParametri,
    '                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

    '    Return Scrivi_Contatto(Contatto, GiasContext, Impresa, Nothing, enum_EntitaAlberoImprese.Impresa, objParametri_Server, objParametri_Utenti)

    'End Function

    'Public Function Scrivi_Contatto_Centro(Contatto As AgronicaCoreModelsSTD.anagrafiche.Contatto,
    '                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                       ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
    '                                       ByRef objParametri_Server As AgronicaCoreParametri,
    '                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

    '    Return Scrivi_Contatto(Contatto, GiasContext, Nothing, Centro, enum_EntitaAlberoImprese.Centro, objParametri_Server, objParametri_Utenti)

    'End Function

    Public Function EliminaContatto(Piva As String, Sa_Cod As Integer, Cod_Contatto As String, ByRef EseguitaOperazione As Boolean, objParametri_Server As AgronicaCoreParametri) As String

        Dim Msg As String = String.Empty

        Const NomeRoutine = "AgronicaCoreAnagrafeBIZ.Contatti_W.EliminaContatto()"

        EseguitaOperazione = False

        Try
            'controllo se è un'impresa GIAS
            Dim isGIAS As Boolean = VerificaEsistenza_PivaGIAS(objParametri_Server, Cod_Contatto)

            If Not isGIAS Then

                Msg = VerificaEliminaContatto(Piva, Sa_Cod, Cod_Contatto, objParametri_Server)

                If String.IsNullOrEmpty(Msg) Then

                    'CANCELLA IL CONTATTO
                    EliminaContattoEffettiva(Piva, Cod_Contatto, objParametri_Server)

                    EseguitaOperazione = True
                End If
            Else
                Msg = Gias.ContattoImpresaNonCancellabile
            End If

        Catch ex As GiasException
            Msg = ex.Message

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True))
        End Try

        Return Msg

    End Function

    Public Function VerificaEliminaContatto(Piva As String, Sa_Cod As Integer, Cod_Contatto As String, objParametri_Server As AgronicaCoreParametri) As String

        Dim Msg As String = String.Empty

        Const NomeRoutine = "AgronicaCoreAnagrafeBIZ.Contatti_W.VerificaEliminaContatto()"

        Try
            Dim ObjMovNC As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim DtNonContabili As DataTable

            Dim ObjMovC As New AgronicaCoreContabDAL.Movimenti_R
            Dim DtContabili As DataTable

            Dim ObjRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim DtRisUm As DataTable

            Dim objSquadre As New AgronicaCoreContabDAL.CDG_DAL_R
            Dim DtSquadre As DataTable

            Dim Nominativo As String = ""
            Dim Descrizione As String = ""
            Dim Cod_RisUm As Integer

            Dim Dt As New DataTable
            Dim Dr As DataRow

            Dim i, j As Integer

            Dim esistenzaSquadre As Boolean = False
            Dim listaSquadre As New List(Of String)

            'griglia dei movimenti
            Dt.Columns.Add(New DataColumn("Data", GetType(String)))
            Dt.Columns.Add(New DataColumn("Movimento", GetType(String)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))

            '===========================================================================
            'Lettura dei movimenti associati ad una risorsa umana di un contatto
            '---------------------------------------------------------------------------

            'non va bene!!!!!!
            'non cerca per chiave contatto,
            'cerca il cod_contatto 
            ' e piva = oppure contatto pubblico
            'quindi se c'è lo stesso cod-contatto su + imprese, viene letto + volte
            'DtRisUm = ObjRisorseUmane.LeggiContattixSuperUser(CStr(Piva_Impresa), _
            '                                                  0, _
            '                                                  Qs_CodContatto, _
            '                                                  0, 0, "", True, _
            '                                                  enumSelezioneVariabile.Selezione_JoinCompleta, _
            '                                                  "", "", objParametri_Server)

            DtRisUm = ObjRisorseUmane.LeggiSoloContatto(CStr(Piva),
                                                        0,
                                                        Cod_Contatto,
                                                        0, 0, "", True,
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", objParametri_Server)


            ' Check Preeseistenti
            If DtRisUm IsNot Nothing AndAlso DtRisUm.Rows.Count > 0 Then

                For i = 0 To DtRisUm.Rows.Count - 1

                    Nominativo = (DtRisUm.Rows(i).Field(Of String)("Rag_Soc") & DtRisUm.Rows(i).Field(Of String)("Nome") & " " & DtRisUm.Rows(i).Field(Of String)("Cognome")).Trim

                    Cod_RisUm = DtRisUm.Rows(i).Item("Cod_RisUm")


                    '===========================================================================
                    'Lettura dei Movimenti Non Contabili (Lavorazioni su campo..)
                    '---------------------------------------------------------------------------

                    DtNonContabili = ObjMovNC.Leggi("", 0, 0, 0, 0, 0, 0, Cod_RisUm, "", 0, 0, 0,
                                                    0, 0, 0,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    "Elem_Cod = 0", "", objParametri_Server)

                    If DtNonContabili IsNot Nothing AndAlso DtNonContabili.Rows.Count > 0 Then

                        For j = 0 To DtNonContabili.Rows.Count - 1

                            'istanzio una nuova riga
                            Dr = Dt.NewRow

                            Dr.Item("Data") = CDate(DtNonContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                            Dr.Item("Movimento") = DtNonContabili.Rows(j).Item("Des_Lib")

                            'i18n
                            'Impostazione Descrizione del movimento
                            Select Case DtNonContabili.Rows(j).Item("Cau_Mov")
                                Case enum_Agenda_Causali.IMPUTAZIONE_MANODOPERA
                                    Descrizione = "Manodopera come dipendente" ' per " & RsNonContabili.Fields("Qta").Value ' all'ora o all'ettaro

                                Case enum_Agenda_Causali.IMPUTAZIONE_TERZISTI
                                    Descrizione = "Manodopera come terzista" ' per " & RsNonContabili.Fields("Qta").Value  ' all'ora o all'ettaro

                            End Select


                            Dr.Item("Descrizione") = Descrizione


                            Dr.Item("rag_soc") = DtNonContabili.Rows(j).Item("rag_soc")

                            'aggiungo la riga al datatable
                            Dt.Rows.Add(Dr)

                        Next

                    End If


                    '----------------------------
                    ' MOVIMENTI CONTABILI
                    '----------------------------

                    DtContabili = ObjMovC.Leggi("", 0, 0, 0,
                                                Cod_RisUm,
                                                CAU_REGISTRAZIONI,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "", objParametri_Server)


                    If DtContabili IsNot Nothing AndAlso DtContabili.Rows.Count > 0 Then

                        For j = 0 To DtContabili.Rows.Count - 1

                            'istanzio una nuova riga
                            Dr = Dt.NewRow

                            Dr.Item("Data") = CDate(DtContabili.Rows(j).Item("Data_Movimento")).ToShortDateString

                            Dr.Item("Movimento") = DtContabili.Rows(j).Item("Des_Lib")

                            Dr.Item("Descrizione") = DtContabili.Rows(j).Item("Mov_Desc")

                            Dr.Item("rag_soc") = DtContabili.Rows(j).Item("rag_soc")

                            'aggiungo la riga al datatable
                            Dt.Rows.Add(Dr)

                        Next

                    End If

                    '----------------------------
                    ' SQUADRE CDG
                    '----------------------------
                    Dim filtroAggiuntivo As New Text.StringBuilder()
                    filtroAggiuntivo.AppendLine("(")
                    filtroAggiuntivo.AppendLine($"SquadrexAttivita.cod_risum_list LIKE '{Agro_SQL_SaveText("%" & Cod_RisUm & "%")}'")
                    filtroAggiuntivo.AppendLine("OR")
                    filtroAggiuntivo.AppendLine($"SquadrexAttivita.cod_risum_caposquadra_list LIKE '{Agro_SQL_SaveText("%" & Cod_RisUm & "%")}'")
                    filtroAggiuntivo.AppendLine(")")

                    DtSquadre = objSquadre.Leggi_SquadrexAttvita("", 0, 0, 0, 0, filtroAggiuntivo.ToString(), AGRODATAINIZIO, objParametri_Server)

                    For Each squadra In DtSquadre.Rows
                        esistenzaSquadre = True

                        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        Dim impresa_rif As String = objImprese.RagSoc_from_Piva(squadra.item("piva"), objParametri_Server)

                        listaSquadre.Add("- " & impresa_rif & " - " & squadra.item("des_squadra"))

                    Next
                Next
            End If

            If Dt.Rows.Count <> 0 OrElse esistenzaSquadre Then

                'TODO Risorse
                If esistenzaSquadre Then
                    Msg = String.Format(Gias.ImpossibileEliminareContattoXAssegnatoSquadre, Nominativo) & ":" & NEWLINE & String.Join(NEWLINE, listaSquadre)
                Else
                    Msg = String.Format(Gias.ContattoEliminaMovimentiCollegati, Nominativo, vbCrLf)
                End If
            Else

                ' TODO Check Aggiunti 14/11/2019 GIANLUCA
                Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
                Dim dtAltriRiferimenti = objContatti.ContattoRiferimenti(Piva, Cod_Contatto, 0, Sa_Cod, "", objParametri_Server)
                If dtAltriRiferimenti.Rows.Count > 0 Then

                    Dim listaImprese = (From imp In dtAltriRiferimenti.AsEnumerable
                                        Select imp.Field(Of String)("rag_soc") & " - " & Gias.Utilizzo & ": " & imp.Field(Of String)("Scopo")).ToList()

                    Msg = String.Format(Gias.ContattoEliminaUtilizziImprese,
                                        vbCrLf, String.Join(vbCrLf, listaImprese))
                End If
            End If

        Catch ex As GiasException
            Msg = ex.Message
        Catch ex As Exception
            'Se viene generata una exception generica non prevista aggiungo il nome della funzione al messaggio e la rilancio
            Throw New Exception("[" & NomeRoutine & "] : " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True))
        End Try

        'Se sono in un caso gestito da me, restituisco il messaggio di controllo se non è possibile eliminare il contatto, altrimenti restituisco stringa vuota
        Return Msg

    End Function

    Public Sub EliminaContattoEffettiva(Piva As String, Cod_Contatto As String, objParametri_Server As AgronicaCoreParametri)

        Const NomeRoutine = "AgronicaCoreAnagrafeBIZ.Contatti_W.EliminaContattoEffettiva()"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            'Verifico se c'è una connessione e nel caso la utilizzo, altrimenti ne apro una "locale" insieme ad una transazione
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

            Dim objContatti As New Contatti_MultiHost_R
            'Leggo la stringa facendomi ritornare Tipooperazione = 3 (x la cancellazione)
            Dim StringaXML = objContatti.Contatto_Leggi(Piva, Cod_Contatto, "", True, objParametri_Server)

            'Le seguenti funzioni restituiscono una eccezione in caso di errori,
            'quindi non è necessario controllare il Boolean di ritorno
            Contatto_Scrivi(CStr(StringaXML), Nothing, Nothing, objParametri_Server)
            Cancella_Allegati_Contatto(Piva, Cod_Contatto, objParametri_Server)
            AssociaUtente(Piva, Cod_Contatto, "", objParametri_Server)

            If objParametri_Server.objTransazione IsNot Nothing Then
                'Se esiste la transazione ed è locale ne faccio il commit
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception

            If objParametri_Server.objTransazione IsNot Nothing Then
                'Non controllo il FlagTransazioneLocale perché in caso di errore voglio sempre fare il rollback della transazione
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw New Exception("[" & NomeRoutine & "] : " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True))

        Finally
            'Se la connessione è locale la chiudo e imposto a Nothing gli oggetti di connessione e transazione,
            'su quest'ultimo, nel caso devono essere già stati eseguiti il commit od il rollback
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

    End Sub

    Public Function AssociaUtente(piva As String, contatto_cod As String, username As String, objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim risp As Boolean
        Dim Contatti_Scrivi As New AgronicaCoreAnagrafeDAL.Contatti_W
        risp = Contatti_Scrivi.AssociaUtente(piva, contatto_cod, username, objParametri_Server)

        Return risp

    End Function

    Private Function VerificaEsistenza_PivaGIAS(ByRef objParametri_Server As AgronicaCoreParametri, ByVal Piva As String) As Boolean

        Dim DT As DataTable
        Dim Imprese_Leggi As New AgronicaCoreAnagrafeDAL.Imprese_Read
        DT = Imprese_Leggi.Leggi(Piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Private Function Cancella_Allegati_Contatto(ByVal Piva As String, ByVal Cod_Contatto As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal PATH As String = "") As Boolean

        Dim esito As Boolean = True

        Const NomeRoutine = "AgronicaCoreAnagrafeBIZ.Contatti_W.Cancella_Allegati_Contatto()"

        Dim objDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
        Dim allegati As DataTable = objDocumenti.Leggi_Allegati_Entita(Piva, Cod_Contatto, 0, 0, 0, 0, 0, 0, "", "", objParametri)

        For Each allegato In allegati.Rows

            Dim idElenco As Integer = allegato.Item("ID_Elenco")

            If Not Cancella_Allegato(idElenco, PATH, objParametri) Then
                Throw New Exception("[" & NomeRoutine & "] : " & "Errore durante la cancellazione dell'Allegato ID_Elenco = " & idElenco & " relativo al Contatto")
            End If
        Next

        Return esito

    End Function

    Private Function Cancella_Allegato(ByVal ID_Elenco As Integer, ByVal PATH As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Cancella_Allegato()"

        Dim esito As Boolean
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim ID_Tipologia As Integer = 0
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            'parto con la cancellazione dell'allegato
            Dim objElenco_R As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim DT_Elenco As DataTable = objElenco_R.Leggi(ID_Elenco, 0, objParametri)
            ID_Tipologia = DT_Elenco(0)("ID_Tipologia")

            'leggo l Entità
            Dim objEntita_R As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim DT_Entita As DataTable = objEntita_R.Leggi(DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)

            'scrivo il log
            Dim Descrizione_scadenza As String = DT_Elenco.Rows(0).Item("Descrizione_scadenza")
            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)

            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            objLog.Scrivi(ID_Alert_Log, DT_Entita(0)("Piva"), ID_Elenco, ID_Tipologia, Descrizione_scadenza, enum_TipoOperazioneDB.Cancellazione, AGRODATAINIZIO, AGRODATAFINE, objParametri)


            'controllo se ho un allegato associato
            If Not IsDBNull(DT_Entita.Rows(0).Item("Allegati_Documenti_cod")) AndAlso DT_Entita.Rows(0).Item("Allegati_Documenti_cod") <> 0 Then

                'leggo l'allegato 
                Dim objAllegato_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
                Dim DT_Allegato As DataTable = objAllegato_R.Leggi(DT_Entita.Rows(0).Item("Allegati_Documenti_cod"),
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "", "", objParametri)
                'controllo se esiste il file 
                If Not String.IsNullOrEmpty(PATH) Then
                    If Not IsDBNull(DT_Allegato.Rows(0).Item("Sottocartella")) AndAlso Not IsDBNull(DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")) Then

                        Dim sottocartella As String = DT_Allegato.Rows(0).Item("Sottocartella")
                        Dim nomefile As String = DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")

                        If File.Exists(PATH & sottocartella & "/" & nomefile) Then
                            File.Delete(PATH & sottocartella & "/" & nomefile)
                        End If
                    End If
                End If

                'cancello il record Allegati
                Dim objAllegati_W As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                objAllegati_W.Cancella(DT_Entita.Rows(0).Item("Allegati_Documenti_cod"), "", objParametri)

            End If

            'cancello il record Elenco
            Dim objELenco_W As New AgronicaCoreScadenziario.Alert_Elenco_W
            objELenco_W.Cancella(ID_Elenco, objParametri)

            'cancello il record Entita
            Dim objEntita_W As New AgronicaCoreScadenziario.Alert_Entita_W
            objEntita_W.Cancella(DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)

            'ANNULLO LE VECCHIE PROGARMMAZIONI MAIL
            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
            mp_W.CancellaProgrammazioniFromChiave(objParametri, enum_MailTipo.Scadenze_InScadenza, ID_Elenco)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            esito = True

        Catch ex As Exception
            If objParametri.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esito = False

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return esito

    End Function

    Public Sub Valida_Contatto_APP(ByRef risorsa As AgronicaCoreModelsSTD.anagrafiche.RisorseUmane,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)


        If String.IsNullOrEmpty(risorsa.contatto.primaryKey.partitaIva) Then
            Throw New Exception("La partita IVA del contatto è obbligatoria.")
        End If

        If risorsa.codice = 0 Then

            If risorsa.flag_cancellazione Then
                Throw New Exception("Non è possibile cancellare il contatto.")
            End If

            ' eventuale fix per usare il codice fiscale come codice contatto
            'If String.IsNullOrEmpty(risorsa.contatto.primaryKey.codice) AndAlso Not String.IsNullOrEmpty(risorsa.contatto.codiceFiscale) Then
            '   risorsa.contatto.primaryKey.codice = risorsa.contatto.codiceFiscale
            'End If

            If Not String.IsNullOrEmpty(risorsa.contatto.primaryKey.codice) Then
                Dim objCont As New Contatti_R
                Dim dtContatto As DataTable = objCont.ControllaSePresente(risorsa.contatto.primaryKey.partitaIva, risorsa.contatto.primaryKey.codice, objParametri_Server)
                If dtContatto.Rows.Count > 0 Then
                    ' eventuale fix per non perdere il codice inserito
                    'If String.IsNullOrEmpty(risorsa.contatto.codiceFiscale) Then
                    '   risorsa.contatto.codiceFiscale = risorsa.contatto.primaryKey.codice
                    'End If
                    risorsa.contatto.primaryKey.codice = ""
                    'Throw New Exception("Contatto già presente.")
                End If
            End If

            risorsa.contatto.fisico_Giuridico = If(Not String.IsNullOrEmpty(Trim(risorsa.contatto.ragione_Sociale)), PERSONA_GIURIDICA, PERSONA_FISICA)

            If risorsa.contatto.fisico_Giuridico = PERSONA_FISICA Then
                If String.IsNullOrEmpty(risorsa.contatto.nome) Then
                    Throw New Exception("Il nome del contatto è obbligatorio.")
                End If
                If String.IsNullOrEmpty(risorsa.contatto.cognome) Then
                    Throw New Exception("Il cognome del contatto è obbligatorio.")
                End If
                If String.IsNullOrEmpty(risorsa.contatto.sesso) Then
                    risorsa.contatto.sesso = "M"
                End If
            End If

            If risorsa.rapportoContabile Is Nothing Then
                Throw New Exception("Il rapporto contabile è obbligatorio.")
            End If

            If risorsa.contatto.sa_cod = -1 OrElse risorsa.contatto.visibilitaPubblica Then
                Throw New Exception("Non è possibile inserire contatti pubblici.")
            End If

            If risorsa.contatto.data_Nascita = DateTime.MinValue Then
                risorsa.contatto.data_Nascita = AGRODATAINIZIO
            End If

            If IsNothing(risorsa.validita) Then
                risorsa.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale()
            End If

        ElseIf String.IsNullOrEmpty(risorsa.contatto.primaryKey.codice) Then

            Throw New Exception("Il codice del contatto è obbligatoria.")

        Else

            Dim objContattiRead As New Contatti_R
            Dim objRisorseUmaneRead As New Risorse_Umane_R

            Dim dtContatti As DataTable =
            objContattiRead.LeggiContattoSpecifico(
                risorsa.contatto.primaryKey.partitaIva,
                risorsa.contatto.primaryKey.codice,
                0,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                "", "", objParametri_Server)

            Dim dtRisorseUmane As DataTable =
            objRisorseUmaneRead.LeggiSoloContatto(
                risorsa.contatto.primaryKey.partitaIva,
                risorsa.codice,
                risorsa.contatto.primaryKey.codice,
                0,
                0,
                "",
                True,
                AGRODATAINIZIO,
                AGRODATAFINE,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                "", "", objParametri_Server)

            If dtContatti.Rows.Count > 0 AndAlso dtRisorseUmane.Rows.Count > 0 Then

                Dim drContatto As DataRow = dtContatti.Rows(0)
                Dim drRisorsa As DataRow = dtRisorseUmane.Rows(0)

                If drContatto.Item("Sa_Cod") = -1 Then
                    Throw New Exception("Non è possibile modificare contatti pubblici.")
                End If

                risorsa.codice = drRisorsa.Item("Cod_Risum")
                risorsa.settore = drRisorsa.Item("Settore_Des")
                risorsa.attivita = drRisorsa.Item("Attivita_Des")
                risorsa.contatto.primaryKey.partitaIva = drContatto.Item("Piva")
                risorsa.contatto.primaryKey.codice = drContatto.Item("Cod_Contatto")
                risorsa.contatto.sa_cod = drContatto.Item("Sa_Cod")
                risorsa.contatto.fisico_Giuridico = drContatto.Item("Id_CF")
                If risorsa.contatto.fisico_Giuridico = PERSONA_FISICA Then
                    If String.IsNullOrEmpty(risorsa.contatto.nome) Then
                        risorsa.contatto.nome = drContatto.Item("Nome")
                    End If
                    If String.IsNullOrEmpty(risorsa.contatto.cognome) Then
                        risorsa.contatto.cognome = drContatto.Item("Cognome")
                    End If
                    risorsa.contatto.ragione_Sociale = ""
                Else
                    If String.IsNullOrEmpty(risorsa.contatto.ragione_Sociale) Then
                        risorsa.contatto.ragione_Sociale = drContatto.Item("Rag_Soc")
                    End If
                    risorsa.contatto.nome = ""
                    risorsa.contatto.cognome = ""
                End If
                If String.IsNullOrEmpty(risorsa.contatto.codiceFiscale) Then
                    risorsa.contatto.codiceFiscale = drContatto.Item("Codice_Fiscale")
                End If
                risorsa.contatto.sesso = drContatto.Item("Sesso")
                risorsa.contatto.visibilitaPubblica = drContatto.Item("Sa_Cod") = -1
                risorsa.contatto.data_Nascita = If(Not IsDBNull(drContatto.Item("Data_Nascita")), CDate(drContatto.Item("Data_Nascita")), AGRODATAINIZIO)
                risorsa.contatto.badge = If(Not IsDBNull(drContatto.Item("NrBadge")), CStr(drContatto.Item("NrBadge")), "")

                If IsNothing(risorsa.rapportoContabile) Then
                    risorsa.rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(drRisorsa.Item("Cod_Rapporto"))
                ElseIf risorsa.rapportoContabile.codice <> drRisorsa.Item("Cod_Rapporto") Then
                    Throw New Exception("Non è possibile modificare il rapporto contabile.")
                End If

                If IsNothing(risorsa.validita) Then
                    risorsa.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale()
                End If

            Else
                Throw New Exception("Il contatto non esiste.")
            End If

        End If

    End Sub

    Public Sub Scrivi_Contatto_APP(ByRef risorsa As AgronicaCoreModelsSTD.anagrafiche.RisorseUmane,
                                   ByVal tipoOperazione As enum_TipoOperazioneDB,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Const NomeRoutine = "AgronicaCoreAnagrafeBIZ.Contatti_W.Scrivi_Contatto_APP()"

        Dim cod_risum As Integer = risorsa.codice
        Dim cod_rapporto As Integer = risorsa.rapportoContabile.codice
        Dim piva As String = risorsa.contatto.primaryKey.partitaIva
        Dim cod_contatto As String = risorsa.contatto.primaryKey.codice
        Dim sa_cod As Integer = risorsa.contatto.sa_cod
        Dim rag_soc As String = risorsa.contatto.ragione_Sociale
        Dim nome As String = risorsa.contatto.nome
        Dim cognome As String = risorsa.contatto.cognome
        Dim sesso As String = risorsa.contatto.sesso
        Dim data_nascita As Date = risorsa.contatto.data_Nascita
        Dim codice_fiscale As String = risorsa.contatto.codiceFiscale
        Dim id_cf As Integer = risorsa.contatto.fisico_Giuridico
        Dim badge As String = risorsa.contatto.badge
        Dim settore As String = risorsa.settore
        Dim attivita As String = risorsa.attivita
        Dim validita_inizio As Date = risorsa.validita.inizio
        Dim validita_fine As Date = risorsa.validita.fine

        If tipoOperazione = enum_TipoOperazioneDB.Cancellazione Then

            Dim esito As Boolean = False
            Dim msg = EliminaContatto(piva, sa_cod, cod_contatto, esito, objParametri_Server)

            If Not esito AndAlso Not String.IsNullOrEmpty(msg) Then
                Throw New Exception(msg)
            End If

        Else

            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False

            Try

                ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                Dim objContattiWrite As New AgronicaCoreAnagrafeDAL.Contatti_W
                Dim objRisorseUmaneWrite As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W

                If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

                    If String.IsNullOrEmpty(cod_contatto) Then
                        cod_contatto = Genera_Codice_Fittizio(piva, objParametri_Server)
                        risorsa.contatto.primaryKey.codice = cod_contatto
                    End If

                    If String.IsNullOrEmpty(codice_fiscale) Then
                        risorsa.contatto.codiceFiscale = cod_contatto
                        codice_fiscale = cod_contatto
                    End If

                    objContattiWrite.Scrivi(
                        piva, sa_cod, cod_contatto, id_cf,
                        rag_soc, codice_fiscale, "", 0,
                        nome, cognome, data_nascita, sesso, "",
                        AGRODATAINIZIO, AGRODATAFINE,
                        objParametri_Server,
                        nrBadge:=badge, Nome_Breve:="", ChkFittizio:=0,
                        Cod_Conto_Economico_Default:=-1, Cod_Conto_Patrimoniale_Default:=-1)

                    Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                    cod_risum = ObjSequenze.NuovoId_Tabella("Risorse_Umane", 0, 0, objParametri_Server)
                    risorsa.codice = cod_risum

                    objRisorseUmaneWrite.Scrivi(
                        piva, sa_cod, cod_risum, cod_contatto, cod_rapporto,
                        settore, attivita, 0, 0, 0, 0, 0, 0, 0,
                        "", AGRODATAINIZIO, AGRODATAFINE, "", 0, "", 0, 0, 0,
                        validita_inizio, validita_fine, objParametri_Server)

                ElseIf tipoOperazione = enum_TipoOperazioneDB.Modifica Then

                    objContattiWrite.Modifica(
                        piva, sa_cod, cod_contatto, id_cf,
                        rag_soc, codice_fiscale, "", 0,
                        nome, cognome, data_nascita, sesso, "",
                        AGRODATAINIZIO, AGRODATAFINE, badge, "",
                        objParametri_Server)

                    objRisorseUmaneWrite.Modifica(
                        cod_risum, settore, attivita, 0, 0, 0, 0, 0,
                        "", AGRODATAINIZIO, AGRODATAFINE, "", 0, 0, 0,
                        validita_inizio, validita_fine, "", objParametri_Server)

                End If

                If objParametri_Server.objTransazione IsNot Nothing Then
                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                End If

            Catch ex As Exception

                If objParametri_Server.objTransazione IsNot Nothing Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Throw New Exception("[" & NomeRoutine & "] : " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True))

            Finally

                ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try

        End If

        Dim Note As String = "Operazione registrata da SincroDatiApp"
        Dim ContattoDes As String = If(String.IsNullOrEmpty(rag_soc), String.Format("{0} {1}", cognome, nome), rag_soc)

        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim datiContatto = JsonConvert.SerializeObject(risorsa, tzh)

        Dim objLogContattiWrite As New AgronicaCoreAnagrafeDAL.Agronica_Log_Contatti_W
        objLogContattiWrite.Scrivi(tipoOperazione, piva, cod_contatto, 0, id_cf, ContattoDes, Note, enum_Id_Servizio.GiasOnline, objParametri_Server, datiContatto, enum_SistemiEsterni.GiasAPP)

    End Sub

    Private Function Genera_Codice_Fittizio(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim codiceFittizio As String = String.Empty
        Dim ok = False
        Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze

        While ok = False
            codiceFittizio = objAgroSe.NuovoId_Tabella("impresa", 0, 0, objParametri_Server).ToString.Replace("-", "F")
            'controllo se è già usato 
            ok = Not Esiste_Contatto(piva, codiceFittizio, objParametri_Server)
        End While

        objParametri_Server.ResettaFinestra()

        Return codiceFittizio

    End Function

    Public Function Esiste_Contatto(ByVal piva As String, ByVal cod_contatto As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim risultato As Boolean = False
        Dim objContattiRead As New AgronicaCoreAnagrafeDAL.Contatti_R

        Dim dt As DataTable = objContattiRead.LeggiContattoSpecifico(piva,
                                                                      cod_contatto,
                                                                      0,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                      "", "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return risultato = True
        End If

        Return risultato

    End Function

End Class

