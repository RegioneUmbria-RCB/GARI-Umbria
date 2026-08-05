Imports System.Data
Imports System.Linq
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Analisi_Testata_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Analisi_Testata_LeggiConEF(ByVal Analisi_Testata_Cod As Long,
                                               ByVal piva As String,
                                               ByVal ForDelete As Boolean,
                                               ByVal Analisi_Id_Agenda As Long,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As String

        Dim nomeRoutine As String = "AnagrafeBIZ.Analisi_Testata_R.Analisi_Testata_LeggiConEF()"

        Dim rVal As String

        Dim gEfUtils As New Gias_EF_Utility
        Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim giasContext As New Gias_DeveloperServer_Entities(efConnString)

        'Dim o As List(Of Analisi_Testata) =
        'GiasContext.Analisi_Testata.Include("Analisi_EntitaxTestata").Select(Function(a) a.Analisi_EntitaxTestata.Where(Function(b) b.Piva = piva)).ToList()

        'Dim o As List(Of Analisi_Testata) = (
        '    From t In GiasContext.Analisi_Testata.Include("").Where(Function (a) a.piva = piva))

        giasContext.Configuration.LazyLoadingEnabled = False

        Dim o As List(Of Analisi_Testata) = (
            From t In giasContext.Analisi_Testata.Include("Analisi_EntitaxTestata").Where(Function(a) a.Analisi_EntitaxTestata.Any(Function(b) b.Piva = piva))
                ).ToList


        rVal = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of List(Of Analisi_Testata))(o, "")

        Dim o2 As List(Of Analisi_Testata) = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of List(Of Analisi_Testata))(rVal, "")

        Return rVal

    End Function


    Public Function Analisi_Testata_Leggi(ByVal Analisi_Testata_Cod As Long,
                                          ByVal ForDelete As Boolean,
                                          ByVal Analisi_Id_Agenda As Long,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As String

        Const nomeRoutine = "AnagrafeBIZ.Analisi_Testata_R.Analisi_Testata_Leggi()"

        Dim messaggioErrore As String = ""

        Dim risultatoFunzione As String = String.Empty

        Dim xmlDoc As XmlDocument

        Dim xmlCertificato As XmlElement
        Dim xmlDatiEntita As XmlElement
        Dim xmlEntita As XmlElement
        Dim xmlDatiTestate As XmlElement
        Dim xmlTestata As XmlElement
        Dim xmlDatiDettagli As XmlElement
        Dim xmlDettaglio As XmlElement
        Dim xmlDatiCampioni As XmlElement
        Dim xmlCampione As XmlElement

        Dim certificatoCod As Integer

        Dim FlagConnessioneLocale As Boolean

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------

            '#################################
            '##########  ANALISI  ############
            '#################################

            Dim objTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
            Dim dtTestata As DataTable
            dtTestata = objTestata.Leggi(CLng(Analisi_Testata_Cod),
                                         CLng(Analisi_Id_Agenda),
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "",
                                         "",
                                         objParametri)


            'Se ottengo almeno un risultato, creo la struttura XML
            If dtTestata.Rows.Count > 0 Then

                '----- < Documento XML > -----
                xmlDoc = New XmlDocument

                xmlDatiTestate = xmlDoc.CreateElement("DatiTestate")

                Dim i As Integer

                'Effettuo un ciclo sulle testate 
                For i = 0 To dtTestata.Rows.Count - 1

                    xmlTestata = xmlDoc.CreateElement("Testata")

                    xmlTestata.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                    xmlTestata.SetAttribute("analisi_superuser", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_SuperUser")))
                    xmlTestata.SetAttribute("analisi_certificato_cod", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Certificato_Cod")))
                    xmlTestata.SetAttribute("analisi_testata_cod", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Cod")))
                    xmlTestata.SetAttribute("analisi_testata_des", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Des")))
                    xmlTestata.SetAttribute("analisi_testata_data_inizio", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Data_Inizio")))
                    xmlTestata.SetAttribute("analisi_testata_data_fine", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Data_Fine")))
                    xmlTestata.SetAttribute("analisi_testata_coord_x", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Coord_X")))
                    xmlTestata.SetAttribute("analisi_testata_coord_y", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Coord_Y")))
                    xmlTestata.SetAttribute("analisi_testata_riferimento_1", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Riferimento_1")))
                    xmlTestata.SetAttribute("analisi_testata_riferimento_2", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Riferimento_2")))
                    xmlTestata.SetAttribute("analisi_testata_riferimento_3", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Riferimento_3")))
                    xmlTestata.SetAttribute("analisi_testata_riferimento_4", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Riferimento_4")))
                    xmlTestata.SetAttribute("analisi_testata_riferimento_5", Agro_SQL_Load(dtTestata.Rows(i).Item("Analisi_Testata_Riferimento_5")))

                    xmlTestata.SetAttribute("analisi_testata_note1", Agro_SQL_Load(dtTestata.Rows(i).Item("analisi_testata_note1")))
                    xmlTestata.SetAttribute("analisi_testata_note2", Agro_SQL_Load(dtTestata.Rows(i).Item("analisi_testata_note2")))
                    xmlTestata.SetAttribute("analisi_testata_note3", Agro_SQL_Load(dtTestata.Rows(i).Item("analisi_testata_note3")))
                    xmlTestata.SetAttribute("analisi_testata_note4", Agro_SQL_Load(dtTestata.Rows(i).Item("analisi_testata_note4")))
                    xmlTestata.SetAttribute("analisi_testata_tipo", Agro_SQL_Load(dtTestata.Rows(i).Item("analisi_testata_tipo")))
                    xmlTestata.SetAttribute("analisi_id_agenda", Agro_SQL_Load(dtTestata.Rows(i).Item("analisi_id_agenda")))

                    xmlTestata.SetAttribute("id_classetessitura", Agro_SQL_Load(dtTestata.Rows(i).Item("Id_ClasseTessitura")))


                    certificatoCod = dtTestata.Rows(i).Item("Analisi_Certificato_Cod")

                    '#################################
                    '########  CERTIFICATO  ##########
                    '#################################

                    If certificatoCod <> 0 Then

                        Dim dtCertificato As DataTable
                        Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_Read

                        dtCertificato = objCertificato.Leggi(CLng(certificatoCod),
                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                             "",
                                                             "",
                                                             objParametri)

                        For j As Integer = 0 To dtCertificato.Rows.Count - 1

                            '----- < Certificato > -----
                            xmlCertificato = xmlDoc.CreateElement("Certificato")

                            xmlCertificato.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            xmlCertificato.SetAttribute("analisi_superuser", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_SuperUser")))
                            xmlCertificato.SetAttribute("analisi_certificato_cod", Agro_SQL_Load(certificatoCod))

                            xmlCertificato.SetAttribute("analisi_certificato_des", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_des")))
                            xmlCertificato.SetAttribute("analisi_certificato_data_inizio", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_data_inizio")))
                            xmlCertificato.SetAttribute("analisi_certificato_data_fine", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_data_fine")))
                            xmlCertificato.SetAttribute("analisi_certificato_laboratorio", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_laboratorio")))
                            xmlCertificato.SetAttribute("analisi_certificato_tipologiacod", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_tipologiacod")))
                            xmlCertificato.SetAttribute("analisi_certificato_tipocampione", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_tipocampione")))
                            xmlCertificato.SetAttribute("analisi_certificato_provenienza", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_provenienza")))
                            xmlCertificato.SetAttribute("analisi_certificato_verbale", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_verbale")))
                            xmlCertificato.SetAttribute("analisi_certificato_richiedente", Agro_SQL_Load(dtCertificato.Rows(j).Item("analisi_certificato_richiedente")))
                            xmlCertificato.SetAttribute("analisi_certificato_prelevatoda", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_PrelevatoDa")))
                            xmlCertificato.SetAttribute("analisi_certificato_comune", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Comune")))
                            xmlCertificato.SetAttribute("analisi_certificato_protocollo", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Protocollo")))
                            xmlCertificato.SetAttribute("analisi_certificato_numregistro", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_NumRegistro")))
                            xmlCertificato.SetAttribute("analisi_certificato_sezione", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Sezione")))
                            xmlCertificato.SetAttribute("analisi_certificato_datafirma", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_DataFirma")))
                            xmlCertificato.SetAttribute("analisi_certificato_responsabile", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Responsabile")))
                            xmlCertificato.SetAttribute("analisi_certificato_analista", Agro_SQL_Load(dtCertificato.Rows(j).Item("Analisi_Certificato_Analista")))
                            xmlCertificato.SetAttribute("validita_inizio", Agro_SQL_Load(dtCertificato.Rows(j).Item("validita_inizio")))
                            xmlCertificato.SetAttribute("validita_fine", Agro_SQL_Load(dtCertificato.Rows(j).Item("validita_fine")))

                            xmlDatiTestate.AppendChild(xmlCertificato)
                            '----- < / Certificato > -----

                        Next

                    End If




                    '#################################
                    '##########  ENTITA'  ############
                    '#################################

                    'Mi procuro un elenco dei dettagli della testata

                    Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
                    Dim dtEntitaTest As DataTable

                    dtEntitaTest = objEntitaxTestata.Leggi(CInt(dtTestata.Rows(i).Item("Analisi_Testata_Cod")),
                                                           0,
                                                           "",
                                                           0,
                                                           0,
                                                           0,
                                                           0,
                                                           0,
                                                           "",
                                                           "",
                                                           "",
                                                           0,
                                                           0,
                                                           "",
                                                           "",
                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           "",
                                                           "",
                                                           objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If dtEntitaTest.Rows.Count > 0 Then

                        xmlDatiEntita = xmlDoc.CreateElement("DatiAnalisi_EntitaxTestata")

                        For j As Integer = 0 To dtEntitaTest.Rows.Count - 1

                            '----- < Analisi_EntitaxTestata > -----
                            xmlEntita = xmlDoc.CreateElement("Analisi_EntitaxTestata")

                            xmlEntita.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            xmlEntita.SetAttribute("analisi_superuser", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("Analisi_SuperUser")))

                            xmlEntita.SetAttribute("analisi_testata_cod", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("Analisi_Testata_Cod")))
                            xmlEntita.SetAttribute("analisi_entita_cod", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("Analisi_Entita_Cod")))

                            xmlEntita.SetAttribute("piva", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("piva")))
                            xmlEntita.SetAttribute("sa_cod", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("sa_cod")))
                            xmlEntita.SetAttribute("campo_cod", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("campo_cod")))
                            xmlEntita.SetAttribute("appezza", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("appezza")))
                            xmlEntita.SetAttribute("id_imp", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("id_imp")))
                            xmlEntita.SetAttribute("fabbricato_cod", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("fabbricato_cod")))
                            xmlEntita.SetAttribute("prov", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("prov")))
                            xmlEntita.SetAttribute("com", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("com")))
                            xmlEntita.SetAttribute("sezione", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("sezione")))
                            xmlEntita.SetAttribute("foglio", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("foglio")))
                            xmlEntita.SetAttribute("numero", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("numero")))
                            xmlEntita.SetAttribute("subalterno", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("subalterno")))
                            xmlEntita.SetAttribute("id_oggetto_grafico", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("id_oggetto_grafico")))
                            xmlEntita.SetAttribute("vas_cod", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("vas_cod")))

                            xmlEntita.SetAttribute("validita_inizio", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("validita_inizio")))
                            xmlEntita.SetAttribute("validita_fine", Agro_SQL_Load(dtEntitaTest.Rows(j).Item("validita_fine")))

                            xmlDatiEntita.AppendChild(xmlEntita)
                            '----- < / Analisi_EntitaxTestata > -----

                        Next

                        xmlTestata.AppendChild(xmlDatiEntita)

                    End If


                    '#################################
                    '##########  DETTAGLI  ###########
                    '#################################

                    'Mi procuro un elenco dei dettagli della testata

                    Dim objDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
                    Dim dtDettagli As DataTable
                    dtDettagli = objDettagli.Leggi(dtTestata.Rows(i).Item("Analisi_Testata_Cod"),
                                                   0, 0,
                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "", "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If dtDettagli.Rows.Count > 0 Then

                        xmlDatiDettagli = xmlDoc.CreateElement("DatiDettagli")

                        'Effettuo un ciclo sugli indirizzi
                        For j As Integer = 0 To dtDettagli.Rows.Count - 1

                            '----- < Dettaglio > -----
                            xmlDettaglio = xmlDoc.CreateElement("Dettaglio")

                            xmlDettaglio.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            xmlDettaglio.SetAttribute("analisi_superuser", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_SuperUser")))

                            xmlDettaglio.SetAttribute("analisi_testata_cod", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_Testata_Cod")))
                            xmlDettaglio.SetAttribute("analisi_dettaglio_cod", Agro_SQL_Load(dtDettagli.Rows(j).Item("analisi_dettaglio_cod")))
                            xmlDettaglio.SetAttribute("analisi_parametro_cod", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_Parametro_Cod")))
                            xmlDettaglio.SetAttribute("analisi_dettaglio_valore_1", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_Dettaglio_Valore_1")))
                            xmlDettaglio.SetAttribute("analisi_dettaglio_margineerrore_1", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_Dettaglio_MargineErrore_1")))
                            xmlDettaglio.SetAttribute("analisi_dettaglio_valore_2", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_Dettaglio_Valore_2")))
                            xmlDettaglio.SetAttribute("analisi_dettaglio_margineerrore_2", Agro_SQL_Load(dtDettagli.Rows(j).Item("Analisi_Dettaglio_MargineErrore_2")))

                            xmlDettaglio.SetAttribute("validita_inizio", Agro_SQL_Load(dtDettagli.Rows(j).Item("validita_inizio")))
                            xmlDettaglio.SetAttribute("validita_fine", Agro_SQL_Load(dtDettagli.Rows(j).Item("validita_fine")))

                            '#################################
                            '##########  CAMPIONIXDETTAGLI  ##
                            '#################################

                            Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R

                            Dim dtCampionixDettagli As DataTable
                            dtCampionixDettagli = objCampionixDettagli.Leggi(CInt(dtTestata.Rows(i).Item("Analisi_Testata_Cod")),
                                                                             CInt(dtDettagli.Rows(j).Item("analisi_dettaglio_cod")),
                                                                             0,
                                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                             "",
                                                                             "",
                                                                             objParametri)

                            'Se ottengo almeno un risultato, creo la struttura XML
                            If dtCampionixDettagli.Rows.Count > 0 Then

                                xmlDatiCampioni = xmlDoc.CreateElement("DatiCampioni")

                                'Effettuo un ciclo
                                Dim objCampioni As New AgronicaCoreAnagrafeDAL.Analisi_Campione_Read

                                Dim dtCampione As DataTable

                                For jj As Integer = 0 To dtCampionixDettagli.Rows.Count - 1

                                    dtCampione = objCampioni.Leggi(CInt(dtCampionixDettagli.Rows(jj).Item("Analisi_Campione_Cod")),
                                                                   enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                   "", "",
                                                                   objParametri)

                                    If dtCampione.Rows.Count > 0 Then

                                        '----- < CAMPIONE > -----
                                        xmlCampione = xmlDoc.CreateElement("Campione")

                                        xmlCampione.SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                        xmlCampione.SetAttribute("analisi_superuser", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_SuperUser")))
                                        xmlCampione.SetAttribute("analisi_campione_cod", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Cod")))
                                        xmlCampione.SetAttribute("analisi_campione_des", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Des")))
                                        xmlCampione.SetAttribute("analisi_campione_coord_x", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Coord_X")))
                                        xmlCampione.SetAttribute("analisi_campione_coord_y", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Coord_Y")))

                                        xmlCampione.SetAttribute("analisi_campione_quantita", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Quantita")))
                                        xmlCampione.SetAttribute("analisi_campione_udm", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_UdM")))
                                        xmlCampione.SetAttribute("analisi_campione_udm_des", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Udm_Des")))
                                        xmlCampione.SetAttribute("analisi_campione_udm_sim", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Udm_Sim")))

                                        xmlCampione.SetAttribute("analisi_campione_profondita", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Profondita")))
                                        xmlCampione.SetAttribute("analisi_campione_profondita_min", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Profondita_Min")))
                                        xmlCampione.SetAttribute("analisi_campione_profondita_max", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Profondita_Max")))

                                        xmlCampione.SetAttribute("analisi_campione_riferimento_1", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Riferimento_1")))
                                        xmlCampione.SetAttribute("analisi_campione_riferimento_2", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Riferimento_2")))
                                        xmlCampione.SetAttribute("analisi_campione_riferimento_3", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Riferimento_3")))
                                        xmlCampione.SetAttribute("analisi_campione_riferimento_4", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Riferimento_4")))
                                        xmlCampione.SetAttribute("analisi_campione_riferimento_5", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Riferimento_5")))
                                        xmlCampione.SetAttribute("analisi_campione_note", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Note")))
                                        xmlCampione.SetAttribute("analisi_campione_key_piva", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Key_Piva")))
                                        xmlCampione.SetAttribute("analisi_campione_key_sacod", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Key_SaCod")))
                                        xmlCampione.SetAttribute("analisi_campione_key_idgrafica", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Key_IDGrafica")))
                                        xmlCampione.SetAttribute("analisi_campione_prov", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_Prov")))
                                        xmlCampione.SetAttribute("analisi_campione_com", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_com")))
                                        xmlCampione.SetAttribute("analisi_campione_sezione", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_sezione")))
                                        xmlCampione.SetAttribute("analisi_campione_foglio", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_foglio")))
                                        xmlCampione.SetAttribute("analisi_campione_numero", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_numero")))
                                        xmlCampione.SetAttribute("analisi_campione_subalterno", Agro_SQL_Load(dtCampione.Rows(0).Item("Analisi_Campione_subalterno")))

                                    End If

                                    xmlDatiCampioni.AppendChild(xmlCampione)
                                    '----- < / CAMPIONE > -----

                                Next

                                xmlDettaglio.AppendChild(xmlDatiCampioni)

                            End If

                            '#################################
                            '#################################
                            '#################################


                            xmlDatiDettagli.AppendChild(xmlDettaglio)
                            '----- < / Dettaglio > -----

                        Next

                        xmlTestata.AppendChild(xmlDatiDettagli)

                    End If

                    '#################################  
                    '################################# 
                    '################################# 

                    xmlDatiTestate.AppendChild(xmlTestata)

                Next

                xmlDoc.AppendChild(xmlDatiTestate)

                risultatoFunzione = xmlDoc.InnerXml
                '----- < / Documento XML > -----

            Else

                risultatoFunzione = ""

            End If

        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            '------------------------------
            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return risultatoFunzione

    End Function

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Analisi_Testata_W
    Inherits AgronicaCoreDataProvider.LogProvider


    Public Function Analisi_Testata_Scrivi(ByVal DatiAnalisi As String,
                                           ByRef OUTPUT_TestatCod As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional NoteLog As String = "",
                                           Optional Piva As String = ""
                                           ) As Boolean

        Const nomeRoutine = "AnagrafeBIZ.Analisi_Testata_W.Analisi_Testata_Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim objAgronicaLogAnalisiW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_W

        Dim xmlDoc As XmlDocument

        Dim xDatiAnalisi_EntitaxTestate As XmlNodeList
        Dim xDatiAnalisi_EntitaxTestata As XmlElement
        Dim xAnalisi_EntitaxTestate As XmlNodeList
        Dim xAnalisi_EntitaxTestata As XmlElement

        Dim xmlDatiTestate As XmlNodeList
        Dim xmlDatiTestata As XmlElement
        Dim xTestate As XmlNodeList
        Dim xTestata As XmlElement

        Dim xCertificati As XmlNodeList
        Dim xCertificato As XmlElement

        Dim xmlDatiDettagli As XmlNodeList
        Dim xmlDatiDettaglio As XmlElement
        Dim xDettagli As XmlNodeList
        Dim xDettaglio As XmlElement
        Dim xmlDatiCampioni As XmlNodeList
        Dim xmlDatiCampione As XmlElement

        Dim resLog As Boolean = False

        Dim i_DatiAnalisi_EntitaxTestate As Integer
        Dim i_Analisi_EntitaxTestate As Integer
        Dim i_Testata As Integer
        Dim i_Dettaglio As Integer

        Dim OpeDB_Analisi_EntitaxTestata As String
        Dim OpeDB_Certificato As String
        Dim OpeDB_Testata As String
        Dim OpeDB_Dettaglio As String

        Dim certificatoCod As Long
        Dim testataCod As Long
        Dim dettaglioCod As Long

        Dim dummy As Integer
        Dim objSequenze As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)
            'Dim ConnessioneAperta As Boolean = False
            'If Not objParametri.objConnessione Is Nothing AndAlso objParametri.objConnessione.State = ConnectionState.Open Then
            '    ConnessioneAperta = True
            'End If

            ''Se la connessione è chiusa la apro
            'If ConnessioneAperta = False Then
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            '    FlagConnessioneLocale = True
            'End If

            'If objParametri.objTransazione Is Nothing Then
            '    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
            '    FlagTransazioneLocale = True
            'End If
            '------------------------------

            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(DatiAnalisi)

            '------------------------------


            '-------------------------------------------------------------
            ' DATI TESTATE
            '-------------------------------------------------------------

            'Prelevo l'elenco dei movimenti
            xmlDatiTestate = xmlDoc.GetElementsByTagName("DatiTestate")


            If xmlDatiTestate.Count > 0 Then

                xmlDatiTestata = xmlDatiTestate.Item(0)

                '-------------------------------------------------------------
                ' CERTIFICATO
                '-------------------------------------------------------------

                'Se sto inserendo o modificando una testata guardo
                'se ha anche un certificato di analisi associato
                'If OpeDB_Testata = "1" Or OpeDB_Testata = "2" Then

                certificatoCod = 0

                xCertificati = xmlDatiTestata.GetElementsByTagName("Certificato")

                If xCertificati IsNot Nothing Then

                    xCertificato = xCertificati.Item(0)

                    If xCertificato IsNot Nothing Then

                        'Prelevo gli attributi dell'impresa selezionata
                        OpeDB_Certificato = xCertificato.GetAttribute("TipoOperazioneDB")
                        Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_W

                        certificatoCod = CLng(xCertificato.GetAttribute("analisi_certificato_cod"))


                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Certificato

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                If certificatoCod <= 0 Then

                                    certificatoCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_CERTIFICATO"),
                                                                                 CLng(xCertificato.GetAttribute("basecode")),
                                                                                 CLng(xCertificato.GetAttribute("topcode")),
                                                                                 objParametri)

                                Else

                                    'Do nothing

                                End If

                                dummy = objCertificato.Scrivi(certificatoCod,
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_des")),
                                                              CDate(xCertificato.GetAttribute("analisi_certificato_data_inizio")),
                                                              CDate(xCertificato.GetAttribute("analisi_certificato_data_fine")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_laboratorio")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_tipologiacod")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_tipocampione")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_provenienza")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_verbale")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_richiedente")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_prelevatoda")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_comune")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_protocollo")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_numregistro")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_sezione")),
                                                              CDate(xCertificato.GetAttribute("analisi_certificato_datafirma")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_responsabile")),
                                                              CStr(xCertificato.GetAttribute("analisi_certificato_analista")),
                                                              AGRODATAINIZIO,
                                                              0,
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              objParametri)

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objCertificato.Modifica(CLng(xCertificato.GetAttribute("analisi_certificato_cod")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_des")),
                                                        CDate(xCertificato.GetAttribute("analisi_certificato_data_inizio")),
                                                        CDate(xCertificato.GetAttribute("analisi_certificato_data_fine")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_laboratorio")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_tipologiacod")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_tipocampione")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_provenienza")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_verbale")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_richiedente")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_prelevatoda")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_comune")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_protocollo")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_numregistro")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_sezione")),
                                                        CDate(xCertificato.GetAttribute("analisi_certificato_datafirma")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_responsabile")),
                                                        CStr(xCertificato.GetAttribute("analisi_certificato_analista")),
                                                        0,
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        objParametri)

                            Case "3" 'CANCELLAZIONE

                                objCertificato.Cancella(CLng(xCertificato.GetAttribute("analisi_certificato_cod")),
                                                        "",
                                                        objParametri)

                        End Select

                    End If

                End If

                xCertificato = Nothing
                xCertificati = Nothing

                'End If

                '-------------------------------------------------------------
                ' TESTATA
                '-------------------------------------------------------------

                xTestate = xmlDatiTestata.GetElementsByTagName("Testata")

                i_Testata = 0

                Do While i_Testata < xTestate.Count

                    'Prelevo l'i-esima testata
                    xTestata = xTestate.Item(i_Testata)

                    'Prelevo gli attributi del codice selezionato
                    OpeDB_Testata = xTestata.GetAttribute("TipoOperazioneDB")

                    Dim objTestate As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W

                    Dim objPC_Dettagli_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_R
                    Dim objParticelleCatastali_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_R
                    Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R

                    testataCod = 0
                    testataCod = CLng(xTestata.GetAttribute("analisi_testata_cod"))
                    OUTPUT_TestatCod = testataCod

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Testata

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------

                            If testataCod <= 0 Then

                                testataCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_TESTATA"),
                                                                         CLng(xTestata.GetAttribute("basecode")),
                                                                         CLng(xTestata.GetAttribute("topcode")),
                                                                         objParametri)

                                OUTPUT_TestatCod = testataCod

                            Else

                                'Do Nothing

                            End If

                            dummy = objTestate.Scrivi(testataCod,
                                                      CLng(certificatoCod),
                                                      CStr(xTestata.GetAttribute("analisi_testata_des")),
                                                      CDate(xTestata.GetAttribute("analisi_testata_data_inizio")),
                                                      CDate(xTestata.GetAttribute("analisi_testata_data_fine")),
                                                      CDbl(xTestata.GetAttribute("analisi_testata_coord_x")),
                                                      CDbl(xTestata.GetAttribute("analisi_testata_coord_y")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_riferimento_1")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_riferimento_2")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_riferimento_3")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_riferimento_4")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_riferimento_5")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_note1")), CStr(xTestata.GetAttribute("analisi_testata_note2")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_note3")), CStr(xTestata.GetAttribute("analisi_testata_note4")),
                                                      CStr(xTestata.GetAttribute("analisi_testata_tipo")),
                                                      CStr(xTestata.GetAttribute("analisi_id_agenda")),
                                                      0,
                                                      CDate(Now),
                                                      CDate(xTestata.GetAttribute("validita_inizio")),
                                                      CDate(xTestata.GetAttribute("validita_fine")),
                                                      objParametri,
                                                      If(xTestata.HasAttribute("id_classetessitura") = False, 0, CStr(xTestata.GetAttribute("id_classetessitura"))))


                        Case "2"    'MODIFICA -------------------------------------------------------

                            objTestate.Modifica(testataCod,
                                                CLng(certificatoCod),
                                                CStr(xTestata.GetAttribute("analisi_testata_des")),
                                                CDate(xTestata.GetAttribute("analisi_testata_data_inizio")),
                                                CDate(xTestata.GetAttribute("analisi_testata_data_fine")),
                                                CDbl(xTestata.GetAttribute("analisi_testata_coord_x")),
                                                CDbl(xTestata.GetAttribute("analisi_testata_coord_y")),
                                                CStr(xTestata.GetAttribute("analisi_testata_riferimento_1")),
                                                CStr(xTestata.GetAttribute("analisi_testata_riferimento_2")),
                                                CStr(xTestata.GetAttribute("analisi_testata_riferimento_3")),
                                                CStr(xTestata.GetAttribute("analisi_testata_riferimento_4")),
                                                CStr(xTestata.GetAttribute("analisi_testata_riferimento_5")),
                                                CStr(xTestata.GetAttribute("analisi_testata_note1")), CStr(xTestata.GetAttribute("analisi_testata_note2")),
                                                CStr(xTestata.GetAttribute("analisi_testata_note3")), CStr(xTestata.GetAttribute("analisi_testata_note4")),
                                                CStr(xTestata.GetAttribute("analisi_testata_tipo")),
                                                CStr(xTestata.GetAttribute("analisi_id_agenda")),
                                                0,
                                                CDate(xTestata.GetAttribute("validita_inizio")),
                                                CDate(xTestata.GetAttribute("validita_fine")),
                                                objParametri,
                                                If(xTestata.HasAttribute("id_classetessitura") = False, 0, CStr(xTestata.GetAttribute("id_classetessitura"))))

                        Case "3"    'ELIMINA -------------------------------------------------------

                            If objPC_Dettagli_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, " PC_Dettagli_Analisi_Testata_Cod =" & testataCod & " ", "", objParametri).Rows.Count > 0 Then
                                Throw New Exception("Non è possibile eliminare l'analisi " & testataCod & " in quanto è utilizzata nei piani ci concimazione.")
                            End If

                            If objParticelleCatastali_VincoliAgronomici_R.Leggi(0, "", "", "", 0, 0, "", "", "", "", "", "", "", " Analisi_Testata_Cod=" & testataCod & " ", "", objParametri).Rows.Count > 0 OrElse
                               objAnagrafe_VincoliAgronomici_R.Leggi(0, "", 0, 0, 0, 0, 0, 0, testataCod, "", "", objParametri).Rows.Count > 0 Then
                                Throw New Exception("Non è possibile eliminare l'analisi " & testataCod & " in quanto è utilizzata nel PUA.")
                            End If

                            objTestate.Cancella(testataCod, "", objParametri)

                    End Select

                    '************************************************
                    '*********** INIZIO LOGGING *******************
                    '************************************************

                    'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                    'è necessario avere Migra >= 748

                    resLog = objAgronicaLogAnalisiW.Scrivi(CInt(OpeDB_Testata),
                                                           CInt(xTestata.GetAttribute("analisi_testata_tipo")),
                                                           testataCod,
                                                           CStr(xTestata.GetAttribute("analisi_testata_des")),
                                                           CDate(xTestata.GetAttribute("analisi_testata_data_inizio")),
                                                           NoteLog, enum_Id_Servizio.GiasOnline,
                                                           objParametri,
                                                           object_data:=DatiAnalisi,
                                                           Piva:=Piva)

                    '************************************************
                    '*********** FINE LOGGING *********************
                    '************************************************

                    '-------------------------------------------------------------
                    ' ANALISI_ENTITAXTESTATA
                    '-------------------------------------------------------------

                    xDatiAnalisi_EntitaxTestate = xTestata.GetElementsByTagName("DatiAnalisi_EntitaxTestata")

                    i_DatiAnalisi_EntitaxTestate = 0

                    Do While i_DatiAnalisi_EntitaxTestate < xDatiAnalisi_EntitaxTestate.Count

                        'Prelevo l'i-esimo blocco di xDatiAnalisi_EntitaxTestate (in realtà ne esiste uno solo)
                        xDatiAnalisi_EntitaxTestata = xDatiAnalisi_EntitaxTestate.Item(i_DatiAnalisi_EntitaxTestate)

                        '------------------------------

                        xAnalisi_EntitaxTestate = xDatiAnalisi_EntitaxTestata.GetElementsByTagName("Analisi_EntitaxTestata")

                        i_Analisi_EntitaxTestate = 0

                        Do While i_Analisi_EntitaxTestate < xAnalisi_EntitaxTestate.Count

                            xAnalisi_EntitaxTestata = xAnalisi_EntitaxTestate.Item(i_Analisi_EntitaxTestate)

                            'Prelevo gli attributi dell'analisi
                            OpeDB_Analisi_EntitaxTestata = xAnalisi_EntitaxTestata.GetAttribute("TipoOperazioneDB")

                            Dim objAnalisi_EntitaxTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Analisi_EntitaxTestata

                                Case "0"    'LEGGI -----------------------

                                Case "1"    'SALVA  ----------------------

                                    dummy = objAnalisi_EntitaxTestata.Scrivi2(
                                            testataCod,
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("analisi_entita_cod")),
                                            CStr(xAnalisi_EntitaxTestata.GetAttribute("piva")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("sa_cod")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("campo_cod")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("appezza")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("id_imp")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("fabbricato_cod")),
                                            CStr(xAnalisi_EntitaxTestata.GetAttribute("prov")),
                                            CStr(xAnalisi_EntitaxTestata.GetAttribute("com")),
                                            CStr(xAnalisi_EntitaxTestata.GetAttribute("sezione")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("foglio")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("numero")),
                                            CStr(xAnalisi_EntitaxTestata.GetAttribute("subalterno")),
                                            CStr(xAnalisi_EntitaxTestata.GetAttribute("id_oggetto_grafico")),
                                            CLng(xAnalisi_EntitaxTestata.GetAttribute("vas_cod")),
                                            0,
                                            CDate(xAnalisi_EntitaxTestata.GetAttribute("validita_inizio")),
                                            CDate(xAnalisi_EntitaxTestata.GetAttribute("validita_fine")),
                                            objParametri)

                                Case "2"   'MODIFICA ----------------------

                                    objAnalisi_EntitaxTestata.Modifica2(
                                           testataCod,
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("analisi_entita_cod")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("piva")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("sa_cod")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("campo_cod")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("appezza")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("id_imp")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("fabbricato_cod")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("prov")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("com")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("sezione")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("foglio")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("numero")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("subalterno")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("id_oggetto_grafico")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("vas_cod")),
                                           0,
                                           CDate(xAnalisi_EntitaxTestata.GetAttribute("validita_inizio")),
                                           CDate(xAnalisi_EntitaxTestata.GetAttribute("validita_fine")),
                                           objParametri)

                                Case "3"   'CANCELLA --------------------------

                                    objAnalisi_EntitaxTestata.Cancella2(
                                           testataCod,
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("analisi_entita_cod")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("piva")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("sa_cod")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("campo_cod")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("appezza")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("id_imp")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("fabbricato_cod")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("prov")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("com")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("sezione")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("foglio")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("numero")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("subalterno")),
                                           CStr(xAnalisi_EntitaxTestata.GetAttribute("id_oggetto_grafico")),
                                           CLng(xAnalisi_EntitaxTestata.GetAttribute("vas_cod")),
                                           "", objParametri)

                            End Select

                            i_Analisi_EntitaxTestate += 1

                        Loop

                        i_DatiAnalisi_EntitaxTestate += 1

                    Loop

                    '</ANALISI_ENTITAXTESTATA>
                    '------------------------------------------------------------------------------------------

                    '-------------------------------------------------------------
                    ' DETTAGLI
                    '-------------------------------------------------------------

                    xmlDatiDettagli = xTestata.GetElementsByTagName("DatiDettagli")

                    If xmlDatiDettagli.Count > 0 Then

                        xmlDatiDettaglio = xmlDatiDettagli.Item(0)

                        xDettagli = xmlDatiDettaglio.GetElementsByTagName("Dettaglio")

                        i_Dettaglio = 0

                        Do While i_Dettaglio < xDettagli.Count

                            'Prelevo l'i-esimo dettaglio
                            xDettaglio = xDettagli.Item(i_Dettaglio)

                            'Prelevo gli attributi del codice selezionato
                            OpeDB_Dettaglio = xDettaglio.GetAttribute("TipoOperazioneDB")

                            Dim objDettaglio As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Dettaglio

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'controllo se dettaglio_cod="" allora genero il record con dettaglio_cod=0
                                    If Not IsNumeric(xDettaglio.GetAttribute("analisi_dettaglio_cod")) Then
                                        dettaglioCod = 0
                                    Else

                                        If CLng(xDettaglio.GetAttribute("analisi_dettaglio_cod")) < 0 Then

                                            dettaglioCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_DETTAGLI"),
                                                                                       CLng(xTestata.GetAttribute("basecode")),
                                                                                       CLng(xTestata.GetAttribute("topcode")),
                                                                                       objParametri)

                                        Else

                                            dettaglioCod = CLng(xDettaglio.GetAttribute("analisi_dettaglio_cod"))

                                        End If

                                    End If


                                    dummy = objDettaglio.Scrivi(testataCod,
                                                                dettaglioCod,
                                                                CLng(xDettaglio.GetAttribute("analisi_parametro_cod")),
                                                                CDbl(xDettaglio.GetAttribute("analisi_dettaglio_valore_1")),
                                                                CDbl(xDettaglio.GetAttribute("analisi_dettaglio_margineerrore_1")),
                                                                CDbl(xDettaglio.GetAttribute("analisi_dettaglio_valore_2")),
                                                                CDbl(xDettaglio.GetAttribute("analisi_dettaglio_margineerrore_2")),
                                                                0,
                                                                CDate(xDettaglio.GetAttribute("validita_inizio")),
                                                                CDate(xDettaglio.GetAttribute("validita_fine")),
                                                                objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    dettaglioCod = CLng(xDettaglio.GetAttribute("analisi_dettaglio_cod"))

                                    objDettaglio.Modifica(testataCod,
                                                          CLng(xDettaglio.GetAttribute("analisi_dettaglio_cod")),
                                                          CLng(xDettaglio.GetAttribute("analisi_parametro_cod")),
                                                          CDbl(xDettaglio.GetAttribute("analisi_dettaglio_valore_1")),
                                                          CDbl(xDettaglio.GetAttribute("analisi_dettaglio_margineerrore_1")),
                                                          CDbl(xDettaglio.GetAttribute("analisi_dettaglio_valore_2")),
                                                          CDbl(xDettaglio.GetAttribute("analisi_dettaglio_margineerrore_2")),
                                                          0,
                                                          AGRODATAINIZIO,
                                                          AGRODATAFINE,
                                                          objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objDettaglio.Cancella(testataCod,
                                                CLng(xDettaglio.GetAttribute("analisi_dettaglio_cod")),
                                                CLng(xDettaglio.GetAttribute("analisi_parametro_cod")),
                                                "",
                                                objParametri)

                            End Select

                            '-------------------------------------------------------------
                            ' CAMPIONI
                            '-------------------------------------------------------------

                            xmlDatiCampioni = xDettaglio.GetElementsByTagName("DatiCampioni")

                            If xmlDatiCampioni IsNot Nothing AndAlso
                               xmlDatiCampioni.Count > 0 Then

                                xmlDatiCampione = xmlDatiCampioni.Item(0)
                                Dim objCampione As New AgronicaCoreAnagrafeBIZ.Analisi_Campione_W

                                objCampione.Campione_Scrivi(CStr(xmlDatiCampione.OuterXml),
                                                            CLng(testataCod),
                                                            CLng(dettaglioCod),
                                                            objParametri)

                            End If

                            '-------------------------------------------------------------------------------

                            i_Dettaglio += 1

                        Loop

                    End If

                    '-------------------------------------------------------------------------------

                    'Incremento l'indice
                    i_Testata += 1

                Loop

            End If


            'If FlagTransazioneLocale = True Then
            '    objParametri.objTransazione.Commit()
            'End If
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            xRisp = True

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
