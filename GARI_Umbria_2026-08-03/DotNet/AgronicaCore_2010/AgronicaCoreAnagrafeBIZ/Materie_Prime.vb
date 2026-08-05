Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Materie_Prime_R
    Inherits AgronicaCoreDataProvider.LogProvider

    '##################################################################################
    'bEscludiRegistri: default = false
    Public Function Materia_Prima_Leggi(ByVal piva As String,
                                        ByVal Mat_Cod As Integer,
                                        ByVal ForDelete As Boolean,
                                        ByVal bEscludiRegistri As Boolean,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal TipoG2G As Integer = 0,
                                        Optional ByVal Cod_Articolo As String = "",
                                        Optional ByVal pivaDestinazione As String = ""
                                        ) As String

        Const nomeRoutine = "AnagrafeBIZ.Materie_Prime_R.Materia_Prima_Leggi()"

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim risultatoFunzione As String

        Dim xmlDoc As XmlDocument

        Dim XmlDatiMaterie_Prime As XmlElement
        Dim XmlMateria_Prima As XmlElement
        Dim XmlDatiMateria_PrimaxReport As XmlElement
        Dim XmlMateria_PrimaxReport As XmlElement
        Dim XmlDatiMateria_PrimaxLC As XmlElement
        Dim XmlMateria_PrimaxLC As XmlElement
        Dim XmlDatiMateria_PrimaxLP As XmlElement
        Dim XmlMateria_PrimaxLP As XmlElement
        Dim XmlDatiMateria_Prima_Dettagli As XmlElement
        Dim XmlMateria_Prima_Dettagli As XmlElement
        Dim XmlDatiMateria_Prima_PQ As XmlElement
        Dim XmlMateria_Prima_PQ As XmlElement
        Dim XmlDatiMateria_Prima_Alias As XmlElement
        Dim XmlMateria_Prima_Alias As XmlElement

        Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim ObjMateria_PrimaxReport As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_R
        Dim ObjMateria_PrimaxLC As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
        Dim ObjMateria_PrimaxLP As New AgronicaCoreAnagrafeDAL.Materie_PrimexLP_R
        Dim ObjMateria_Prima_Alias As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R
        Dim ObjMateria_Prima_Dettagli As New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_R
        Dim ObjMateria_Prima_PQ As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R


        Dim DtMaterie_Prime As DataTable
        Dim DtMateria_PrimaxReport As DataTable
        Dim DtMateria_PrimaxLC As DataTable
        Dim DtMateria_PrimaxLP As DataTable
        Dim DtMateria_Prima_Alias As DataTable
        Dim DtMateria_Prima_Dettagli As DataTable
        Dim DtMateria_Prima_PQ As DataTable

        Dim i, j As Integer

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------

            '------------------------------

            'Mi procuro un elenco delle Materie_Prime
            'all'interno della finestra temporale selezionata

            'Mi procuro il recordset richiesto
            DtMaterie_Prime = objMaterie_Prime.Leggi(CStr(piva),
                                                     0, 0,
                                                     CInt(Mat_Cod),
                                                     Cod_Articolo, 0, 0, 0, 0, 0, 0, 0, "",
                                                     0, "", True, False, "",
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri, TipoG2G, pivaDestinazione)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtMaterie_Prime IsNot Nothing AndAlso DtMaterie_Prime.Rows.Count > 0 Then

                '----- < Documento XML > -----
                xmlDoc = New XmlDataDocument

                XmlDatiMaterie_Prime = xmlDoc.CreateElement("DatiMaterie_Prime")


                'Effettuo un ciclo sulle Materie_Prime
                For i = 0 To DtMaterie_Prime.Rows.Count - 1

                    '----- < Materia_Prima > -----
                    XmlMateria_Prima = xmlDoc.CreateElement("Materia_Prima")

                    With XmlMateria_Prima
                        .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                        .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                        .SetAttribute("piva", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("sa_cod")))
                        .SetAttribute("elem_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("elem_cod")))
                        .SetAttribute("mat_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("mat_cod")))
                        .SetAttribute("cod_articolo", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("cod_articolo")))
                        .SetAttribute("mat_des", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("mat_des")))
                        .SetAttribute("sem_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("sem_cod")))
                        .SetAttribute("cul_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("cul_cod")))
                        .SetAttribute("veg_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("veg_cod")))
                        .SetAttribute("trap_dur", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("trap_dur")))
                        .SetAttribute("uso", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("uso")))
                        .SetAttribute("cltoss_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("cltoss_cod")))
                        .SetAttribute("newcltoss_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("newcltoss_cod")))
                        .SetAttribute("ditta_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("ditta_cod")))
                        .SetAttribute("n", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("n")))
                        .SetAttribute("p2o5", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("p2o5")))
                        .SetAttribute("k2o", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("k2o")))
                        .SetAttribute("mgo", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("mgo")))
                        .SetAttribute("note", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("note")))
                        .SetAttribute("regolamento", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("regolamento")))
                        .SetAttribute("cal_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("cal_cod")))
                        .SetAttribute("prezzo_unitario", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("prezzo_unitario")))
                        .SetAttribute("extra_str", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("extra_str")))
                        .SetAttribute("extra_int", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("extra_int")))
                        .SetAttribute("extra_date", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("extra_date")))
                        .SetAttribute("grva_cod_veg", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("grva_cod_veg")))
                        .SetAttribute("gen_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("gen_cod")))
                        .SetAttribute("spe_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("spe_cod")))
                        .SetAttribute("raz_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("raz_cod")))
                        .SetAttribute("ipro_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("ipro_cod")))
                        .SetAttribute("cat_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("cat_cod")))
                        .SetAttribute("flag_biologico", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("flag_biologico")))
                        .SetAttribute("flag_convenzionale", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("flag_convenzionale")))
                        .SetAttribute("flag_nonagricolo", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("flag_nonagricolo")))
                        .SetAttribute("flag_ausiliarefabbricazione", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("flag_ausiliarefabbricazione")))
                        .SetAttribute("udm_cod_extra", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("udm_cod_extra")))
                        .SetAttribute("flag_extra", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("flag_extra")))
                        .SetAttribute("chkimballaggio", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("chkimballaggio")))
                        .SetAttribute("qta_extra", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("qta_extra")))
                        .SetAttribute("taglio", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("taglio")))
                        .SetAttribute("mat_cod_origine", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("mat_cod_origine")))
                        .SetAttribute("chklistino", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("chklistino")))
                        .SetAttribute("grfi_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("grfi_cod")))
                        .SetAttribute("codice_prodotto", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("codice_prodotto")))
                        .SetAttribute("colore", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("colore")))
                        .SetAttribute("codice_nc", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("codice_nc")))
                        .SetAttribute("manipolazioni", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("manipolazioni")))
                        .SetAttribute("titolo_alcol", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("titolo_alcol")))
                        .SetAttribute("chkcontenitore", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("chkcontenitore")))
                        .SetAttribute("qta_contenitore", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("qta_contenitore")))
                        .SetAttribute("tipo_peso", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("tipo_peso")))
                        .SetAttribute("tara", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("tara")))
                        .SetAttribute("udm_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("udm_cod")))
                        .SetAttribute("peso_set", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("peso_set")))
                        .SetAttribute("chkescludi_magazzino", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("chkescludi_magazzino")))
                        .SetAttribute("chkescludi_preparazione", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("chkescludi_preparazione")))
                        .SetAttribute("id_accisa_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("id_accisa_cod")))
                        .SetAttribute("confezione_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("confezione_cod")))
                        .SetAttribute("categoria_vino_cod", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("categoria_vino_cod")))
                        .SetAttribute("tipo_reg_alcoli", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("tipo_reg_alcoli")))
                        .SetAttribute("chkalias", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("chkalias")))

                        .SetAttribute("username_creazione", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("username_creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("username_modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtMaterie_Prime.Rows(i).Item("Validita_Fine")))

                        .SetAttribute("basecode", 0)
                        .SetAttribute("topcode", 2000000000)

                    End With


                    '########################################################
                    '###############  Materia_Prima X Report  ###############
                    '########################################################

                    XmlDatiMateria_PrimaxReport = xmlDoc.CreateElement("DatiMaterie_PrimexReport")

                    'Mi procuro il recordset richiesto
                    ObjMateria_PrimaxReport = New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_R
                    DtMateria_PrimaxReport = ObjMateria_PrimaxReport.Leggi(DtMaterie_Prime.Rows(i).Item("Piva"),
                                                                           0,
                                                                           DtMaterie_Prime.Rows(i).Item("Mat_Cod"),
                                                                           0,
                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                           "", "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMateria_PrimaxReport IsNot Nothing AndAlso DtMateria_PrimaxReport.Rows.Count > 0 Then

                        'Effettuo un ciclo sulle Preparazioni
                        For j = 0 To DtMateria_PrimaxReport.Rows.Count - 1

                            '----- < Materia_PrimaxReport > -----
                            XmlMateria_PrimaxReport = xmlDoc.CreateElement("Materia_PrimaxReport")

                            With XmlMateria_PrimaxReport
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("piva", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("piva")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("pro_cod")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("mat_cod")))
                                .SetAttribute("id_report", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("id_report")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("username_modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMateria_PrimaxReport.Rows(j).Item("Validita_Fine")))

                            End With

                            XmlDatiMateria_PrimaxReport.AppendChild(XmlMateria_PrimaxReport)

                            XmlMateria_PrimaxReport = Nothing
                            '----- < / Materia_PrimaxReport > -----

                        Next

                    End If

                    XmlMateria_PrimaxReport = Nothing
                    DtMateria_PrimaxReport = Nothing
                    ObjMateria_PrimaxReport = Nothing

                    '-----------------------------------------------------------

                    XmlMateria_Prima.AppendChild(XmlDatiMateria_PrimaxReport)



                    '########################################################
                    '#####  Materia_Prima X Lotto Configurazione  ###########
                    '########################################################

                    XmlDatiMateria_PrimaxLC = xmlDoc.CreateElement("DatiMaterie_PrimexLC")

                    'Mi procuro il recordset richiesto
                    ObjMateria_PrimaxLC = New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
                    DtMateria_PrimaxLC = ObjMateria_PrimaxLC.Leggi_xAltoLivello(DtMaterie_Prime.Rows(i).Item("Piva"), _
                                                                                0, 0, 0, _
                                                                                CInt(DtMaterie_Prime.Rows(i).Item("Mat_Cod")), _
                                                                                0, -1, -1, -1, _
                                                                               "", "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMateria_PrimaxLC IsNot Nothing Then

                        'Effettuo un ciclo sulle Preparazioni
                        For j = 0 To DtMateria_PrimaxLC.Rows.Count - 1

                            '----- < Materia_PrimaxLC > -----
                            XmlMateria_PrimaxLC = xmlDoc.CreateElement("Materia_PrimaxLC")

                            With XmlMateria_PrimaxLC
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("piva", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("sa_cod")))
                                .SetAttribute("elem_cod", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("elem_cod")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("pro_cod")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("mat_cod")))
                                .SetAttribute("lotto_cod", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("lotto_cod")))
                                .SetAttribute("chklistini", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("chklistini")))
                                .SetAttribute("chkreport", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("chkreport")))
                                .SetAttribute("chkproprieta", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("chkproprieta")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("username_modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMateria_PrimaxLC.Rows(j).Item("Validita_Fine")))

                            End With

                            XmlDatiMateria_PrimaxLC.AppendChild(XmlMateria_PrimaxLC)

                            XmlMateria_PrimaxLC = Nothing
                            '----- < / Materia_PrimaxLC > -----

                        Next

                    End If

                    XmlMateria_PrimaxLC = Nothing
                    DtMateria_PrimaxLC = Nothing
                    ObjMateria_PrimaxLC = Nothing

                    '-----------------------------------------------------------

                    XmlMateria_Prima.AppendChild(XmlDatiMateria_PrimaxLC)



                    '########################################################
                    '#####  Materia_Prima X Lotto Proprietà  ################
                    '########################################################

                    XmlDatiMateria_PrimaxLP = xmlDoc.CreateElement("DatiMaterie_PrimexLP")

                    'Mi procuro il recordset richiesto
                    ObjMateria_PrimaxLP = New AgronicaCoreAnagrafeDAL.Materie_PrimexLP_R
                    DtMateria_PrimaxLP = ObjMateria_PrimaxLP.Leggi(DtMaterie_Prime.Rows(i).Item("Piva"),
                                                                   0, 0, 0, 0,
                                                                   CInt(DtMaterie_Prime.Rows(i).Item("Mat_Cod")),
                                                                   0, "", 0, "", 0, "", 0,
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMateria_PrimaxLP IsNot Nothing AndAlso DtMateria_PrimaxLP.Rows.Count > 0 Then

                        'Effettuo un ciclo sulle Preparazioni
                        For j = 0 To DtMateria_PrimaxLP.Rows.Count - 1

                            '----- < Materia_PrimaxLP > -----
                            XmlMateria_PrimaxLP = xmlDoc.CreateElement("Materia_PrimaxLP")

                            With XmlMateria_PrimaxLP
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("piva", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("piva")))
                                .SetAttribute("id", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("id")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("sa_cod")))
                                .SetAttribute("elem_cod", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("elem_cod")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("pro_cod")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("mat_cod")))
                                .SetAttribute("lotto_cod1", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("lotto_cod1")))
                                .SetAttribute("lotto_val1", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("lotto_val1")))
                                .SetAttribute("lotto_cod2", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("lotto_cod2")))
                                .SetAttribute("lotto_val2", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("lotto_val2")))
                                .SetAttribute("lotto_cod3", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("lotto_cod3")))
                                .SetAttribute("lotto_val3", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("lotto_val3")))
                                .SetAttribute("id_proprieta", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("id_proprieta")))
                                .SetAttribute("proprieta_val", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("proprieta_val")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("username_modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMateria_PrimaxLP.Rows(j).Item("Validita_Fine")))

                            End With

                            XmlDatiMateria_PrimaxLP.AppendChild(XmlMateria_PrimaxLP)

                            XmlMateria_PrimaxLP = Nothing
                            '----- < / Materia_PrimaxLP > -----

                        Next

                    End If

                    XmlMateria_PrimaxLP = Nothing
                    DtMateria_PrimaxLP = Nothing
                    ObjMateria_PrimaxLP = Nothing


                    '-----------------------------------------------------------

                    XmlMateria_Prima.AppendChild(XmlDatiMateria_PrimaxLP)



                    '########################################################
                    '#################  Materia_Prima Alias  ################
                    '########################################################

                    XmlDatiMateria_Prima_Alias = xmlDoc.CreateElement("DatiMaterie_Prime_Alias")

                    'Mi procuro il recordset richiesto
                    ObjMateria_Prima_Alias = New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R
                    DtMateria_Prima_Alias = ObjMateria_Prima_Alias.LeggiAlias(DtMaterie_Prime.Rows(i).Item("Piva"),
                                                                              CInt(DtMaterie_Prime.Rows(i).Item("Mat_Cod")),
                                                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                              "", "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMateria_Prima_Alias IsNot Nothing AndAlso DtMateria_Prima_Alias.Rows.Count > 0 Then

                        'Effettuo un ciclo sugli alias
                        For j = 0 To DtMateria_Prima_Alias.Rows.Count - 1

                            '----- < Materia_Prima_Alias > -----
                            XmlMateria_Prima_Alias = xmlDoc.CreateElement("Materia_Prima_Alias")

                            With XmlMateria_Prima_Alias
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("piva", Agro_SQL_Load(DtMateria_Prima_Alias.Rows(j).Item("piva")))
                                .SetAttribute("mat_cod", DtMaterie_Prime.Rows(i).Item("Mat_Cod"))
                                .SetAttribute("mat_cod_alias", Agro_SQL_Load(DtMateria_Prima_Alias.Rows(j).Item("mat_cod")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtMateria_Prima_Alias.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtMateria_Prima_Alias.Rows(j).Item("username_modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMateria_Prima_Alias.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMateria_Prima_Alias.Rows(j).Item("Validita_Fine")))

                            End With

                            XmlDatiMateria_Prima_Alias.AppendChild(XmlMateria_Prima_Alias)

                            XmlMateria_Prima_Alias = Nothing
                            '----- < / Materia_Prima_Alias > -----

                        Next

                    End If

                    XmlMateria_Prima_Alias = Nothing
                    DtMateria_Prima_Alias = Nothing
                    ObjMateria_Prima_Alias = Nothing

                    '-----------------------------------------------------------

                    XmlMateria_Prima.AppendChild(XmlDatiMateria_Prima_Alias)



                    '########################################################
                    '##############  Materia_Prima Dettagli  ################
                    '########################################################

                    XmlDatiMateria_Prima_Dettagli = xmlDoc.CreateElement("DatiMaterie_Prime_Dettagli")

                    'Mi procuro il recordset richiesto
                    ObjMateria_Prima_Dettagli = New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_R
                    DtMateria_Prima_Dettagli = ObjMateria_Prima_Dettagli.Leggi(DtMaterie_Prime.Rows(i).Item("Piva"),
                                                                               CInt(DtMaterie_Prime.Rows(i).Item("Mat_Cod")),
                                                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                               "", "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMateria_Prima_Dettagli IsNot Nothing AndAlso DtMateria_Prima_Dettagli.Rows.Count > 0 Then

                        'Effettuo un ciclo sui Dettagli
                        For j = 0 To DtMateria_Prima_Dettagli.Rows.Count - 1

                            '----- < Materia_Prima_Dettagli > -----
                            XmlMateria_Prima_Dettagli = xmlDoc.CreateElement("Materia_Prima_Dettagli")

                            With XmlMateria_Prima_Dettagli
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("piva", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("piva")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("mat_cod")))
                                .SetAttribute("extra_smallint1", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_smallint1")))
                                .SetAttribute("extra_smallint2", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_smallint2")))
                                .SetAttribute("extra_smallint3", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_smallint3")))
                                .SetAttribute("extra_smallint4", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_smallint4")))
                                .SetAttribute("extra_smallint5", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_smallint5")))
                                .SetAttribute("extra_smallint6", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_smallint6")))
                                .SetAttribute("extra_int1", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_int1")))
                                .SetAttribute("extra_int2", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_int2")))
                                .SetAttribute("extra_int3", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_int3")))
                                .SetAttribute("extra_int4", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_int4")))
                                .SetAttribute("extra_int5", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_int5")))
                                .SetAttribute("extra_int6", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_int6")))
                                .SetAttribute("extra_dbl1", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_dbl1")))
                                .SetAttribute("extra_dbl2", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_dbl2")))
                                .SetAttribute("extra_dbl3", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_dbl3")))
                                .SetAttribute("extra_dbl4", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_dbl4")))
                                .SetAttribute("extra_dbl5", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_dbl5")))
                                .SetAttribute("extra_dbl6", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_dbl6")))
                                .SetAttribute("extra_str1", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_str1")))
                                .SetAttribute("extra_str2", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_str2")))
                                .SetAttribute("extra_str3", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_str3")))
                                .SetAttribute("extra_str4", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_str4")))
                                .SetAttribute("extra_str5", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_str5")))
                                .SetAttribute("extra_str6", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_str6")))
                                .SetAttribute("extra_date1", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_date1")))
                                .SetAttribute("extra_date2", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_date2")))
                                .SetAttribute("extra_date3", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_date3")))
                                .SetAttribute("extra_date4", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_date4")))
                                .SetAttribute("extra_date5", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_date5")))
                                .SetAttribute("extra_date6", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("extra_date6")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("username_modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMateria_Prima_Dettagli.Rows(j).Item("Validita_Fine")))

                            End With

                            XmlDatiMateria_Prima_Dettagli.AppendChild(XmlMateria_Prima_Dettagli)

                            XmlMateria_Prima_Dettagli = Nothing
                            '----- < / Materia_Prima_Dettagli > -----

                        Next

                    End If

                    XmlMateria_Prima_Dettagli = Nothing
                    DtMateria_Prima_Dettagli = Nothing
                    ObjMateria_Prima_Dettagli = Nothing

                    '-----------------------------------------------------------

                    XmlMateria_Prima.AppendChild(XmlDatiMateria_Prima_Dettagli)





                    '########################################################
                    '#####  Materia_Prima Parametri Qualitativi  ############
                    '########################################################

                    Dim stringaFiltro_PQ As String

                    If bEscludiRegistri Then
                        'Nota: Al fine di duplicare una materia prima occorre reimpostare i riepiloghi nei registri
                        stringaFiltro_PQ = "ChkRegistri = 0 And ChkRegistri_Vinificazione = 0"
                    Else
                        stringaFiltro_PQ = ""
                    End If

                    XmlDatiMateria_Prima_PQ = xmlDoc.CreateElement("DatiParametri_Qualitativi")

                    'Mi procuro il recordset richiesto
                    ObjMateria_Prima_PQ = New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R
                    DtMateria_Prima_PQ = ObjMateria_Prima_PQ.Leggi(DtMaterie_Prime.Rows(i).Item("Piva"),
                                                                   0,
                                                                   CInt(DtMaterie_Prime.Rows(i).Item("Mat_Cod")),
                                                                   "", 0, 0,
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   stringaFiltro_PQ, "", objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMateria_Prima_PQ IsNot Nothing AndAlso DtMateria_Prima_PQ.Rows.Count > 0 Then

                        'Effettuo un ciclo sui Parametri Qualitativi
                        For j = 0 To DtMateria_Prima_PQ.Rows.Count - 1

                            '----- < Materia_Prima_PQ > -----
                            XmlMateria_Prima_PQ = xmlDoc.CreateElement("Parametro_Qualitativo")

                            With XmlMateria_Prima_PQ
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", objParametri.PivaSuperUser)
                                .SetAttribute("piva", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("sa_cod")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("mat_cod")))
                                .SetAttribute("tipo", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("tipo")))
                                .SetAttribute("tipo_cod", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("tipo_cod")))
                                .SetAttribute("udm_cod", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("udm_cod")))
                                .SetAttribute("valore_des", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("valore_des")))
                                .SetAttribute("valore_min", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("valore_min")))
                                .SetAttribute("valore_max", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("valore_max")))
                                .SetAttribute("chkregistri", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("chkregistri")))
                                .SetAttribute("chkcalibri", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("chkcalibri")))
                                .SetAttribute("chkregistri_vinificazione", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("chkregistri_vinificazione")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("username_modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMateria_Prima_PQ.Rows(j).Item("Validita_Fine")))

                            End With

                            XmlDatiMateria_Prima_PQ.AppendChild(XmlMateria_Prima_PQ)

                            XmlMateria_Prima_PQ = Nothing
                            '----- < / Materia_Prima_PQ > -----

                        Next

                    End If

                    XmlMateria_Prima_PQ = Nothing
                    DtMateria_Prima_PQ = Nothing
                    ObjMateria_Prima_PQ = Nothing

                    '-----------------------------------------------------------

                    XmlMateria_Prima.AppendChild(XmlDatiMateria_Prima_PQ)



                    '-----------------------------------------------------------

                    XmlDatiMaterie_Prime.AppendChild(XmlMateria_Prima)

                    XmlMateria_Prima = Nothing

                Next

                xmlDoc.AppendChild(XmlDatiMaterie_Prime)

                risultatoFunzione = xmlDoc.OuterXml
                '----- < / Documento XML > -----

                XmlDatiMaterie_Prime = Nothing
                XmlMateria_Prima = Nothing
                xmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun Materia_Prima ...
                risultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtMaterie_Prime = Nothing
            objMaterie_Prime = Nothing

        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

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

Public Class Materie_Prime_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Materia_Prima_Scrivi(ByVal DatiMateriePrime As String,
                                         ByRef OUTPUT_Mat_Cod As Integer,
                                         ByVal FinestraTemp_Inizio As Date,
                                         ByVal FinestraTemp_Fine As Date,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Materie_Prime_W.Materia_Prima_Scrivi()"

        Dim dummy As Boolean
        Dim xmlDoc As XmlDocument

        Dim DummyProdotti_Costi As Boolean


        Dim objSequenze As Agro_Sequenze
        Dim objMateriePrime As AgronicaCoreAnagrafeDAL.Materie_Prime_W
        Dim objProdottiCosti As AgronicaCoreContabBIZ.Prodotti_Costi_W
        Dim ObjMaterie_Prime_PQ As AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W
        Dim ObjMaterie_PrimexReport As AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W
        Dim ObjMaterie_PrimexLC As AgronicaCoreAnagrafeDAL.Materie_PrimexLC_W
        Dim ObjMaterie_Prime_Dettagli As AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_W

        Dim codMateriaPrima As Int32
        Dim codArticoloNew As String
        Dim codArticolo As String
        Dim lottoCod As Int32

        Dim xDatiMaterie_Prime As XmlNodeList               ' IXMLDOMNodeList
        Dim xDatiMateria_Prima As XmlElement                ' IXMLDOMElement
        Dim xMaterie_Prime As XmlNodeList                   ' IXMLDOMNodeList
        Dim xMateria_Prima As XmlElement                    ' IXMLDOMElement
        Dim xProdotti_Costi As XmlNodeList                  ' IXMLDOMNodeList
        Dim xProdotto_Costi As XmlElement                   ' IXMLDOMElement
        Dim xDatiParametri_Qualitativi As XmlNodeList       ' IXMLDOMNodeList
        Dim xDatiParametro_Qualitativo As XmlElement        ' IXMLDOMElement
        Dim xParametri_Qualitativi As XmlNodeList           ' IXMLDOMNodeList
        Dim xParametro_Qualitativo As XmlElement            ' IXMLDOMElement
        Dim xDatiMaterie_PrimexReport As XmlNodeList        ' IXMLDOMNodeList
        Dim xDatiMateria_PrimaxReport As XmlElement         ' IXMLDOMElement
        Dim xMaterie_PrimexReport As XmlNodeList            ' IXMLDOMNodeList
        Dim xMateria_PrimaxReport As XmlElement             ' IXMLDOMElement
        Dim xDatiMaterie_LottoConfig As XmlNodeList         ' IXMLDOMNodeList
        Dim xDatiMateria_LottoConfig As XmlElement          ' IXMLDOMElement
        Dim xMaterie_LottoConfig As XmlNodeList             ' IXMLDOMNodeList
        Dim xMateria_LottoConfig As XmlElement              ' IXMLDOMElement
        Dim xDatiMaterie_Dettagli As XmlNodeList            ' IXMLDOMNodeList
        Dim xDatiMateria_Dettagli As XmlElement             ' IXMLDOMElement
        Dim xMaterie_Dettagli As XmlNodeList                ' IXMLDOMNodeList
        Dim xMateria_Dettagli As XmlElement                 ' IXMLDOMElement


        Dim i_DatiMaterie_Prime As Integer
        Dim i_Materia_Prima As Integer
        Dim i_Parametro_Qualitativo As Integer
        Dim i_DatiMateria_PrimaxReport As Integer
        Dim i_Materia_PrimaxReport As Integer
        Dim i_DatiMaterie_LottoConfig As Integer
        Dim i_Materia_LottoConfig As Integer
        Dim i_DatiMaterie_Dettagli As Integer
        Dim i_Materia_Dettagli As Integer


        Dim OpeDB_Materia_Prima As String
        Dim OpeDB_Materie_PrimexReport As String
        Dim OpeDB_Parametro_Qualitativo As String
        Dim OpeDB_Materia_LottoConfig As String
        Dim OpeDB_Materia_Dettagli As String
        Dim DatiProdotti_Costi As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False 'true
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)
            'Verifico se e' stata impostata una connessione
            'If IsNothing(objParametri.objConnessione) Then
            '    'Flag
            '    FlagConnessioneLocale = True
            '    'Creo la connessione localmente
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            'End If
            'If objParametri.objConnessione.State = ConnectionState.Closed Then
            '    'Flag
            '    FlagConnessioneLocale = True
            '    'Creo la connessione localmente
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            'End If
            '------------------------------
            '------------------------------

            xmlDoc = New XmlDocument
            'XmlDoc.async = False
            xmlDoc.LoadXml(DatiMateriePrime)

            '------------------------------
            '------------------------------
            '------------------------------

            xDatiMaterie_Prime = xmlDoc.GetElementsByTagName("DatiMaterie_Prime")

            i_DatiMaterie_Prime = 0

            Do While i_DatiMaterie_Prime < xDatiMaterie_Prime.Count

                'Prelevo l'i-esimo blocco di DatiMaterie_Prime (in realtà ne esiste uno solo)
                xDatiMateria_Prima = xDatiMaterie_Prime.Item(i_DatiMaterie_Prime)


                '------------------------------

                xMaterie_Prime = xDatiMateria_Prima.GetElementsByTagName("Materia_Prima")

                i_Materia_Prima = 0

                Do While i_Materia_Prima < xMaterie_Prime.Count

                    'Prelevo l' i-esima Codifica Materia_Prima
                    xMateria_Prima = xMaterie_Prime.Item(i_Materia_Prima)

                    'Prelevo gli attributi della materia prima selezionata
                    OpeDB_Materia_Prima = xMateria_Prima.GetAttribute("TipoOperazioneDB")

                    objMateriePrime = New AgronicaCoreAnagrafeDAL.Materie_Prime_W

                    'Inizializzo Preventivamente il Cod_Materia_Prima
                    If CStr(xMateria_Prima.GetAttribute("mat_cod")) = "" OrElse
                       Not IsNumeric(xMateria_Prima.GetAttribute("mat_cod")) Then
                        codMateriaPrima = 0
                    Else
                        codMateriaPrima = CInt(xMateria_Prima.GetAttribute("mat_cod"))
                    End If

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Materia_Prima

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Mat_Cod = codMateriaPrima

                        Case "1"    'SALVA -------------------------------------------------------

                            If codMateriaPrima <= 0 Then

                                objSequenze = New Agro_Sequenze

                                'Richiedo un nuovo codice materia prima
                                codMateriaPrima = objSequenze.NuovoId_Tabella( _
                                                "Materie_Prime", _
                                                CInt(xMateria_Prima.GetAttribute("basecode")), _
                                                CInt(xMateria_Prima.GetAttribute("topcode")), _
                                                objParametri)

                                objSequenze = Nothing

                            Else

                                'Codice Già Definito

                            End If

                            OUTPUT_Mat_Cod = codMateriaPrima


                            dummy = objMateriePrime.Scrivi(
                                                CStr(xMateria_Prima.GetAttribute("piva")),
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")),
                                                CInt(xMateria_Prima.GetAttribute("elem_cod")),
                                                CInt(codMateriaPrima),
                                                CStr(xMateria_Prima.GetAttribute("mat_des")),
                                                CInt(xMateria_Prima.GetAttribute("sem_cod")),
                                                CInt(xMateria_Prima.GetAttribute("veg_cod")),
                                                CInt(xMateria_Prima.GetAttribute("cul_cod")),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("gen_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("spe_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("ipro_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("raz_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("cat_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_biologico"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_convenzionale"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_nonagricolo"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_ausiliarefabbricazione"), False),
                                                CInt(xMateria_Prima.GetAttribute("trap_dur")),
                                                CInt(xMateria_Prima.GetAttribute("uso")),
                                                CStr(xMateria_Prima.GetAttribute("cltoss_cod")),
                                                CStr(xMateria_Prima.GetAttribute("newcltoss_cod")),
                                                CInt(xMateria_Prima.GetAttribute("ditta_cod")),
                                                CDbl(xMateria_Prima.GetAttribute("n")),
                                                CDbl(xMateria_Prima.GetAttribute("p2o5")),
                                                CDbl(xMateria_Prima.GetAttribute("k2o")),
                                                CDbl(xMateria_Prima.GetAttribute("mgo")),
                                                CStr(xMateria_Prima.GetAttribute("note")),
                                                CInt(xMateria_Prima.GetAttribute("regolamento")),
                                                CInt(xMateria_Prima.GetAttribute("cal_cod")),
                                                CStr(xMateria_Prima.GetAttribute("cod_articolo")),
                                                CDbl(xMateria_Prima.GetAttribute("prezzo_unitario")),
                                                CInt(xMateria_Prima.GetAttribute("extra_int")),
                                                CStr(xMateria_Prima.GetAttribute("extra_str")),
                                                CDate(xMateria_Prima.GetAttribute("extra_date")),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("grva_cod_veg"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_extra"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("chkimballaggio"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("udm_cod_extra"), False),
                                                If(IsNumeric(xMateria_Prima.GetAttribute("qta_extra")), xMateria_Prima.GetAttribute("qta_extra"), 0),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("taglio"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("grfi_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("chklistino"), False),
                                                Agro_XML_GetInteger(xMateria_Prima, "mat_cod_origine", 0),
                                                Agro_XML_GetString(xMateria_Prima, "piva_superuser_origine", ""),
                                                CDate(xMateria_Prima.GetAttribute("validita_inizio")),
                                                CDate(xMateria_Prima.GetAttribute("validita_fine")),
                                                Agro_XML_GetString(xMateria_Prima, "Codice_Esterno".ToLower, ""),
                                                Agro_XML_GetInteger(xMateria_Prima, "Flag_Importato".ToLower, 0),
                                                objParametri,
                                                Agro_XML_GetInteger(xMateria_Prima, "ID_DisciplinareAcquisti".ToLower, 0))


                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Mat_Cod = codMateriaPrima

                            codArticoloNew = CStr(xMateria_Prima.GetAttribute("cod_articolo"))

                            'If IsNull(xMateria_Prima.GetAttribute("cod_articolo_old")) Then
                            '    Cod_Articolo = CStr(xMateria_Prima.GetAttribute("cod_articolo"))
                            'Else
                            '    Cod_Articolo = CStr(xMateria_Prima.GetAttribute("cod_articolo_old"))
                            'End If

                            If Not xMateria_Prima.HasAttribute("cod_articolo_old") Then
                                codArticolo = CStr(xMateria_Prima.GetAttribute("cod_articolo"))
                            Else
                                codArticolo = CStr(xMateria_Prima.GetAttribute("cod_articolo_old"))
                            End If


                            objMateriePrime.Modifica2(
                                                CStr(xMateria_Prima.GetAttribute("piva")),
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")),
                                                codArticoloNew,
                                                CInt(xMateria_Prima.GetAttribute("elem_cod")),
                                                CInt(codMateriaPrima),
                                                CStr(xMateria_Prima.GetAttribute("mat_des")),
                                                CInt(xMateria_Prima.GetAttribute("sem_cod")),
                                                CInt(xMateria_Prima.GetAttribute("veg_cod")),
                                                CInt(xMateria_Prima.GetAttribute("cul_cod")),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("gen_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("spe_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("ipro_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("raz_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("cat_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_biologico"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_convenzionale"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_nonagricolo"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_ausiliarefabbricazione"), False),
                                                CInt(xMateria_Prima.GetAttribute("trap_dur")),
                                                CInt(xMateria_Prima.GetAttribute("uso")),
                                                CStr(xMateria_Prima.GetAttribute("cltoss_cod")),
                                                CStr(xMateria_Prima.GetAttribute("newcltoss_cod")),
                                                CInt(xMateria_Prima.GetAttribute("ditta_cod")),
                                                CDbl(xMateria_Prima.GetAttribute("n")),
                                                CDbl(xMateria_Prima.GetAttribute("p2o5")),
                                                CDbl(xMateria_Prima.GetAttribute("k2o")),
                                                CDbl(xMateria_Prima.GetAttribute("mgo")),
                                                CStr(xMateria_Prima.GetAttribute("note")),
                                                CInt(xMateria_Prima.GetAttribute("regolamento")),
                                                CInt(xMateria_Prima.GetAttribute("cal_cod")),
                                                codArticolo,
                                                CDbl(xMateria_Prima.GetAttribute("prezzo_unitario")),
                                                CInt(xMateria_Prima.GetAttribute("extra_int")),
                                                CStr(xMateria_Prima.GetAttribute("extra_str")),
                                                CDate(xMateria_Prima.GetAttribute("extra_date")),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("flag_extra"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("chkimballaggio"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("udm_cod_extra"), False),
                                                If(IsNumeric(xMateria_Prima.GetAttribute("qta_extra")), xMateria_Prima.GetAttribute("qta_extra"), 0),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("grva_cod_veg"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("taglio"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("grfi_cod"), False),
                                                Agro_SQL_SaveNum(xMateria_Prima.GetAttribute("chklistino"), False),
                                                CDate(xMateria_Prima.GetAttribute("validita_inizio")),
                                                CDate(xMateria_Prima.GetAttribute("validita_fine")),
                                                Agro_XML_GetString(xMateria_Prima, "Codice_Esterno".ToLower, ""),
                                                Agro_XML_GetInteger(xMateria_Prima, "Flag_Importato".ToLower, 0),
                                                "",
                                                objParametri,
                                                Agro_XML_GetInteger(xMateria_Prima, "ID_DisciplinareAcquisti".ToLower, 0))



                        Case "3" 'CANCELLAZIONE  ------------------------------------------------------

                            OUTPUT_Mat_Cod = codMateriaPrima

                            objMateriePrime.Cancella2( _
                                                CInt(xMateria_Prima.GetAttribute("elem_cod")), _
                                                CInt(codMateriaPrima), _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CStr(xMateria_Prima.GetAttribute("cod_articolo")), _
                                                "", _
                                                objParametri)


                    End Select



                    '#############################################
                    '##########  MATERIE PRIME X REPORT   ########
                    '#############################################

                    xDatiMaterie_PrimexReport = xMateria_Prima.GetElementsByTagName("DatiMaterie_PrimexReport")

                    i_DatiMateria_PrimaxReport = 0

                    Do While i_DatiMateria_PrimaxReport < xDatiMaterie_PrimexReport.Count

                        'Prelevo l'i-esimo blocco di xDatiMaterie_PrimexReport (in realtà ne esiste uno solo)
                        xDatiMateria_PrimaxReport = xDatiMaterie_PrimexReport.Item(i_DatiMateria_PrimaxReport)

                        ObjMaterie_PrimexReport = New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

                        '------------------------------

                        'Prelevo l'elenco degli Dettagli
                        xMaterie_PrimexReport = xDatiMateria_PrimaxReport.GetElementsByTagName("Materia_PrimaxReport")

                        i_Materia_PrimaxReport = 0

                        Do While i_Materia_PrimaxReport < xMaterie_PrimexReport.Count

                            'Prelevo l'i-esimo Dettaglio
                            xMateria_PrimaxReport = xMaterie_PrimexReport.Item(i_Materia_PrimaxReport)

                            If i_Materia_PrimaxReport = 0 Then

                                'Cancellazione Preventiva

                                ObjMaterie_PrimexReport.Cancella( _
                                                CStr(xMateria_PrimaxReport.GetAttribute("piva")), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                0, _
                                                 "", _
                                                objParametri)

                            End If


                            'Prelevo gli attributi del Dettaglio selezionato
                            OpeDB_Materie_PrimexReport = xMateria_PrimaxReport.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Materie_PrimexReport

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Salvo il Dettaglio
                                    dummy = ObjMaterie_PrimexReport.Scrivi( _
                                                CStr(xMateria_PrimaxReport.GetAttribute("piva")), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("id_report")), _
                                                CDate(xMateria_PrimaxReport.GetAttribute("validita_inizio")), _
                                                CDate(xMateria_PrimaxReport.GetAttribute("validita_fine")), _
                                                objParametri)


                                Case "2"    'MODIFICA -------------------------------------------------------

                                    ObjMaterie_PrimexReport.Modifica( _
                                                CStr(xMateria_PrimaxReport.GetAttribute("piva")), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("id_report")), _
                                                CDate(xMateria_PrimaxReport.GetAttribute("validita_inizio")), _
                                                CDate(xMateria_PrimaxReport.GetAttribute("validita_fine")), _
                                                 "", _
                                                objParametri)


                                Case "3"    'ELIMINA -------------------------------------------------------


                                    ObjMaterie_PrimexReport.Cancella( _
                                                CStr(xMateria_PrimaxReport.GetAttribute("piva")), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                CInt(xMateria_PrimaxReport.GetAttribute("id_report")), _
                                                 "", _
                                                objParametri)

                            End Select

                            'Incremento l'indice
                            i_Materia_PrimaxReport += 1

                        Loop

                        'Incremento l'indice
                        i_DatiMateria_PrimaxReport += 1

                    Loop

                    'Elimino l'oggetto
                    ObjMaterie_PrimexReport = Nothing



                    '#########################################################
                    '##########  MATERIA PRIMA X LOTTO CONFIGURAZIONI ########
                    '#########################################################

                    xDatiMaterie_LottoConfig = xMateria_Prima.GetElementsByTagName("DatiMaterie_PrimexLC")

                    i_DatiMaterie_LottoConfig = 0

                    Do While i_DatiMaterie_LottoConfig < xDatiMaterie_LottoConfig.Count

                        'Prelevo l'i-esimo blocco di i_DatiMaterie_LottoConfig (in realtà ne esiste uno solo)
                        xDatiMateria_LottoConfig = xDatiMaterie_LottoConfig.Item(i_DatiMaterie_LottoConfig)

                        ObjMaterie_PrimexLC = New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_W

                        '------------------------------

                        'Prelevo l'elenco degli Dettagli
                        xMaterie_LottoConfig = xDatiMateria_LottoConfig.GetElementsByTagName("Materia_PrimaxLC")

                        i_Materia_LottoConfig = 0

                        Do While i_Materia_LottoConfig < xMaterie_LottoConfig.Count

                            'Prelevo l'i-esimo Dettaglio
                            xMateria_LottoConfig = xMaterie_LottoConfig.Item(i_Materia_LottoConfig)

                            'Impostazione Lotto_Cod
                            lottoCod = CInt(xMateria_LottoConfig.GetAttribute("lotto_cod"))

                            'Prelevo gli attributi del Dettaglio selezionato
                            OpeDB_Materia_LottoConfig = xMateria_LottoConfig.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Materia_LottoConfig

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Salvo la Configurazione del Lotto
                                    dummy = ObjMaterie_PrimexLC.Scrivi( _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("elem_cod")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                CInt(lottoCod), _
                                                CInt(xMateria_LottoConfig.GetAttribute("chklistini")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("chkreport")), _
                                                CDate(xMateria_LottoConfig.GetAttribute("validita_inizio")), _
                                                CDate(xMateria_LottoConfig.GetAttribute("validita_fine")), _
                                                objParametri)


                                Case "2"    'MODIFICA -------------------------------------------------------

                                    ObjMaterie_PrimexLC.Modifica( _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("elem_cod")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                CInt(lottoCod), _
                                                CInt(xMateria_LottoConfig.GetAttribute("chklistini")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("chkreport")), _
                                                CDate(xMateria_LottoConfig.GetAttribute("validita_inizio")), _
                                                CDate(xMateria_LottoConfig.GetAttribute("validita_fine")), _
                                                 "", _
                                                objParametri)




                                Case "3"    'ELIMINA -------------------------------------------------------


                                    ObjMaterie_PrimexLC.Cancella( _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("elem_cod")), _
                                                CInt(xMateria_LottoConfig.GetAttribute("pro_cod")), _
                                                CInt(codMateriaPrima), _
                                                CInt(lottoCod), _
                                                 "", _
                                                objParametri)



                            End Select

                            'Incremento l'indice
                            i_Materia_LottoConfig += 1

                        Loop

                        'Incremento l'indice
                        i_DatiMaterie_LottoConfig += 1

                    Loop

                    'Elimino l'oggetto
                    ObjMaterie_PrimexLC = Nothing


                    '#########################################################
                    '###################  MATERIA PRIMA DETTAGLI #############
                    '#########################################################

                    xDatiMaterie_Dettagli = xMateria_Prima.GetElementsByTagName("DatiMaterie_Prime_Dettagli")

                    i_DatiMaterie_Dettagli = 0

                    Do While i_DatiMaterie_Dettagli < xDatiMaterie_Dettagli.Count

                        'Prelevo l'i-esimo blocco di i_DatiMaterie_Dettagli (in realtà ne esiste uno solo)
                        xDatiMateria_Dettagli = xDatiMaterie_Dettagli.Item(i_DatiMaterie_Dettagli)

                        ObjMaterie_Prime_Dettagli = New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_W

                        '------------------------------

                        'Prelevo l'elenco degli Dettagli
                        xMaterie_Dettagli = xDatiMateria_Dettagli.GetElementsByTagName("Materia_Prima_Dettagli")

                        i_Materia_Dettagli = 0

                        Do While i_Materia_Dettagli < xMaterie_Dettagli.Count

                            'Prelevo l'i-esimo Dettaglio
                            xMateria_Dettagli = xMaterie_Dettagli.Item(i_Materia_Dettagli)

                            'Prelevo gli attributi del Dettaglio selezionato
                            OpeDB_Materia_Dettagli = xMateria_Dettagli.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Materia_Dettagli

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Salvo i Dettagli della Materia Prima
                                    dummy = ObjMaterie_Prime_Dettagli.Scrivi( _
                                               CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(codMateriaPrima), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint1")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint2")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint3")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint4")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint5")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint6")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int1")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int2")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int3")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int4")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int5")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int6")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl1")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl2")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl3")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl4")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl5")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl6")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str1")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str2")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str3")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str4")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str5")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str6")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str7")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str8")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str9")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date1")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date2")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date3")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date4")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date5")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date6")), _
                                                CDate(xMateria_Dettagli.GetAttribute("validita_inizio")), _
                                                CDate(xMateria_Dettagli.GetAttribute("validita_fine")), _
                                                objParametri)


                                Case "2"    'MODIFICA -------------------------------------------------------

                                    ObjMaterie_Prime_Dettagli.Modifica( _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(codMateriaPrima), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint1")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint2")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint3")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint4")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint5")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_smallint6")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int1")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int2")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int3")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int4")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int5")), _
                                                CInt(xMateria_Dettagli.GetAttribute("extra_int6")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl1")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl2")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl3")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl4")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl5")), _
                                                CDbl(xMateria_Dettagli.GetAttribute("extra_dbl6")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str1")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str2")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str3")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str4")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str5")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str6")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str7")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str8")), _
                                                CStr(xMateria_Dettagli.GetAttribute("extra_str9")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date1")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date2")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date3")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date4")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date5")), _
                                                CDate(xMateria_Dettagli.GetAttribute("extra_date6")), _
                                                CDate(xMateria_Dettagli.GetAttribute("validita_inizio")), _
                                                CDate(xMateria_Dettagli.GetAttribute("validita_fine")), _
                                                "", _
                                                objParametri)





                                Case "3"    'ELIMINA -------------------------------------------------------


                                    ObjMaterie_Prime_Dettagli.Cancella( _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(codMateriaPrima), _
                                                "", _
                                                objParametri)


                            End Select

                            'Incremento l'indice
                            i_Materia_Dettagli += 1

                        Loop

                        'Incremento l'indice
                        i_DatiMaterie_Dettagli += 1

                    Loop

                    'Elimino l'oggetto
                    ObjMaterie_Prime_Dettagli = Nothing



                    '#####################################
                    '##########  PRODOTTI COSTI   ########
                    '#####################################


                    'Prelevo l'elenco dei movimenti
                    xProdotti_Costi = xMateria_Prima.GetElementsByTagName("DatiProdotti_Costi")

                    If xProdotti_Costi.Count > 0 Then

                        xProdotto_Costi = xProdotti_Costi.Item(0)

                        DatiProdotti_Costi = xProdotto_Costi.OuterXml

                        'Creo l'oggetto COM

                        'ObjProdotti_Costi = CreateObject("Agro_Contab.Prodotti_Costi_W")
                        objProdottiCosti = New AgronicaCoreContabBIZ.Prodotti_Costi_W


                        DummyProdotti_Costi = objProdottiCosti.Prodotti_Costi_Scrivi( _
                                                CStr(DatiProdotti_Costi), _
                                                codMateriaPrima, _
                                               objParametri)


                        objProdottiCosti = Nothing

                    End If





                    '############################################
                    '##########  PARAMETRI QUALITATIVI   ########
                    '############################################

                    'Prelevo l'elenco dei parametri
                    xDatiParametri_Qualitativi = xMateria_Prima.GetElementsByTagName("DatiParametri_Qualitativi")

                    If xDatiParametri_Qualitativi.Count > 0 Then

                        'Creo gli oggetti COM

                        'ObjMaterie_Prime_PQ = CreateObject("Agro_Contab_AD.Materie_Prime_PQ_W")
                        ObjMaterie_Prime_PQ = New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W

                        xDatiParametro_Qualitativo = xDatiParametri_Qualitativi.Item(0)

                        'Prelevo l'elenco dei parametri
                        xParametri_Qualitativi = xDatiParametro_Qualitativo.GetElementsByTagName("Parametro_Qualitativo")

                        i_Parametro_Qualitativo = 0

                        Do While i_Parametro_Qualitativo < xParametri_Qualitativi.Count

                            xParametro_Qualitativo = xParametri_Qualitativi.Item(i_Parametro_Qualitativo)

                            'Prelevo gli attributi del parametro selezionata
                            OpeDB_Parametro_Qualitativo = xParametro_Qualitativo.GetAttribute("TipoOperazioneDB")

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Parametro_Qualitativo

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    dummy = ObjMaterie_Prime_PQ.Scrivi(
                                                CStr(xMateria_Prima.GetAttribute("piva")),
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")),
                                                codMateriaPrima,
                                                CStr(xParametro_Qualitativo.GetAttribute("tipo")),
                                                CInt(xParametro_Qualitativo.GetAttribute("tipo_cod")),
                                                CInt(xParametro_Qualitativo.GetAttribute("udm_cod")),
                                                Agro_SQL_SaveText(xParametro_Qualitativo.GetAttribute("valore_des"), False),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("valore_min")), xParametro_Qualitativo.GetAttribute("valore_min"), 0),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("valore_max")), xParametro_Qualitativo.GetAttribute("valore_max"), 0),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("chkregistri")), xParametro_Qualitativo.GetAttribute("chkregistri"), 0),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("chkcalibri")), xParametro_Qualitativo.GetAttribute("chkcalibri"), 0),
                                                CDate(xMateria_Prima.GetAttribute("validita_inizio")),
                                                CDate(xMateria_Prima.GetAttribute("validita_fine")),
                                                objParametri)


                                Case "2"    'MODIFICA -------------------------------------------------------


                                    ObjMaterie_Prime_PQ.Modifica(
                                                CStr(xMateria_Prima.GetAttribute("piva")),
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")),
                                                codMateriaPrima,
                                                CStr(xParametro_Qualitativo.GetAttribute("tipo")),
                                                CInt(xParametro_Qualitativo.GetAttribute("tipo_cod")),
                                                CInt(xParametro_Qualitativo.GetAttribute("udm_cod")),
                                                Agro_SQL_SaveText(xParametro_Qualitativo.GetAttribute("valore_des"), False),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("valore_min")), xParametro_Qualitativo.GetAttribute("valore_min"), 0),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("valore_max")), xParametro_Qualitativo.GetAttribute("valore_max"), 0),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("chkregistri")), xParametro_Qualitativo.GetAttribute("chkregistri"), 0),
                                                If(IsNumeric(xParametro_Qualitativo.GetAttribute("chkcalibri")), xParametro_Qualitativo.GetAttribute("chkcalibri"), 0),
                                                CDate(xMateria_Prima.GetAttribute("validita_inizio")),
                                                CDate(xMateria_Prima.GetAttribute("validita_fine")),
                                                 "",
                                                objParametri)



                                Case "3"    'CANCELLA -------------------------------------------------------

                                    ObjMaterie_Prime_PQ.Cancella( _
                                                CStr(xMateria_Prima.GetAttribute("piva")), _
                                                CInt(xMateria_Prima.GetAttribute("sa_cod")), _
                                                codMateriaPrima, _
                                                CStr(xParametro_Qualitativo.GetAttribute("tipo")), _
                                                CInt(xParametro_Qualitativo.GetAttribute("tipo_cod")), _
                                                CInt(xParametro_Qualitativo.GetAttribute("udm_cod")), _
                                                 "", _
                                                objParametri)



                            End Select

                            i_Parametro_Qualitativo += 1

                        Loop

                        'Elimino l'oggetto
                        ObjMaterie_Prime_PQ = Nothing

                    End If

                    'Elimino l'oggetto
                    objMateriePrime = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Materia_Prima += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiMaterie_Prime += 1

            Loop


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiMaterie_Prime = Nothing
            xDatiMateria_Prima = Nothing
            xMaterie_Prime = Nothing
            xMateria_Prima = Nothing

            objSequenze = Nothing

            xmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            'If FlagTransazioneLocale = True Then
            '    objParametri.objTransazione.Commit()
            'End If

            'If FlagTransazioneLocale = True Then
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
            'End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            'If objParametri.objTransazione IsNot Nothing Then
            '    objParametri.objTransazione.Rollback()
            '    objParametri.objTransazione = Nothing
            'End If

            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Mat_Cod=" & CStr(OUTPUT_Mat_Cod) & ")" & " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

            'If (FlagConnessioneLocale = True) AndAlso (objParametri.objConnessione IsNot Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If

        End Try

        Return xRisp

    End Function

End Class
