Imports System.Data.Common
Imports System.Linq
Imports System.Text
Imports System.Transactions
Imports System.Web.UI.WebControls
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreModelsSTD.exceptions

Public Class CentroAziendale_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggiCentriAziendaliPKPerPIVA(ByVal PIVA As String,
                                                ByRef ObjParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK)

        Dim ret As New List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.CentroAziendale_R.LeggiCentriAziendaliPKPerPIVA()"
        Dim MessaggioErrore As String = ""
        Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim xCentriAziendaliR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        If PIVA = "" Then
            Throw New Exception("Specificare la partita iva ")
        End If

        Try
            Dim dt As DataTable = xCentriAziendaliR.Leggi_CentriXImprese(PIVA, False, ObjParametri_Server)
            For Each row In dt.Rows
                ret.Add(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(row("sa_cod"), PIVA))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return ret
    End Function

    Public Function LeggiCentriAziendaliPKPerCUAA(ByVal CUAA As String,
                                                ByRef ObjParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK)

        Dim ret As New List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.CentroAziendale_R.LeggiCentriAziendaliPKPerCUAA()"
        Dim MessaggioErrore As String = ""
        Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim xCentriAziendaliR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim piva As String = ""
        Try
            piva = xImpCodR.Piva_from_CUAA(CUAA, ObjParametri_Server)
            If piva = "" Then
                Throw New Exception("Nessuna partita iva trovata per il CUAA " & CUAA)
            End If

            Dim dt As DataTable = xCentriAziendaliR.Leggi_CentriXImprese(piva, False, ObjParametri_Server)
            For Each row In dt.Rows
                ret.Add(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(row("sa_cod"), piva))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return ret
    End Function

    Public Function CaricaComuni(provincia As String, objPServer As Object) As rispostaStandard(Of ListItemCollection)
        Dim risposta As New rispostaStandard(Of ListItemCollection)

        If provincia <> "" Then
            Dim cmb_comuni As New DropDownList
            Call AgronicaCoreUtility.CaricaListControl.Comuni(cmb_comuni, True, "", "", provincia, True, 1, "", "", objPServer)

            Dim items = cmb_comuni.Items.Cast(Of ListItem).ToArray().Select(Of ListItem)(Function(s) New ListItem(s.Text, s.Value))
            risposta.RispostaStringa = cmb_comuni.Items
            risposta.RispostaOK = True
        End If

        Return risposta
    End Function

    '============================================================================

    Public Function CentroAziendale_Leggi(ByVal PIVA As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal ForDelete As Boolean,
                                          ByVal AllAttributes As Boolean,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional ByVal LeggiXmlDaSQLServer As Boolean = False,
                                          Optional ByVal TipoG2G As Integer = 0
                                          ) As String

        Dim NomeRoutine As String = "DpiBIZ.CentroAziendale_R.CentroAziendale_Leggi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim RisultatoFunzione As String = String.Empty

        '======

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            '------------------------------
            'Mi procuro un elenco dei Centri Aziendali associati all'Impresa
            'all'interno della finestra temporale selezionata

            Dim objUtentixStrutture As New AgronicaCoreAnagrafeDAL.UtentixStrutture_Read
            Dim DtCentri As DataTable


            'Mi procuro il recordset richiesto
            DtCentri = objUtentixStrutture.Leggi(CStr(PIVA),
                                                 CLng(Sa_Cod),
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "",
                                                 "",
                                                 objParametri,
                                                 TipoG2G)


            'Se ottengo almeno un risultato, creo la struttura XML
            If DtCentri.Rows.Count <> 0 Then

                '----- < Documento XML > -----
                'XmlDoc = CreateObject("Msxml2.DOMDocument.4.0")
                Dim XmlDoc As New XmlDocument
                Dim XmlDatiCentriAziendali As XmlElement
                Dim XmlCentroAziendale As XmlElement
                XmlDatiCentriAziendali = XmlDoc.CreateElement("DatiCentriAziendali")

                'Effettuo un ciclo sui centri aziendali
                Dim iCen As Integer
                For iCen = 0 To DtCentri.Rows.Count - 1

                    '----- < CENTROAZIENDALE > -----
                    XmlCentroAziendale = XmlDoc.CreateElement("CentroAziendale")

                    With XmlCentroAziendale
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtCentri.Rows(iCen).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtCentri.Rows(iCen).Item("sa_cod")))
                        .SetAttribute("sa_nome", Agro_SQL_Load(DtCentri.Rows(iCen).Item("sa_nome")))
                        .SetAttribute("x", Agro_SQL_Load(DtCentri.Rows(iCen).Item("X")))
                        .SetAttribute("y", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Y")))
                        .SetAttribute("zslm", Agro_SQL_Load(DtCentri.Rows(iCen).Item("ZSLM")))
                        .SetAttribute("long", Agro_SQL_Load(DtCentri.Rows(iCen).Item("long")))
                        .SetAttribute("lat", Agro_SQL_Load(DtCentri.Rows(iCen).Item("lat")))
                        .SetAttribute("area", Agro_SQL_Load(DtCentri.Rows(iCen).Item("area")))
                        .SetAttribute("titolopossesso", Agro_SQL_Load(DtCentri.Rows(iCen).Item("TitoloPossesso")))
                        .SetAttribute("sup_bosco", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_Bosco")))
                        .SetAttribute("sup_prati", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_Prati")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Validita_Fine")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Username_Modifica")))

                    End With

                    'Se voglio tutti gli attributi della tabella
                    If AllAttributes Then

                        With XmlCentroAziendale
                            .SetAttribute("ca_sipi", Agro_SQL_Load(DtCentri.Rows(iCen).Item("ca_sipi")))
                            .SetAttribute("at_prevalente", Agro_SQL_Load(DtCentri.Rows(iCen).Item("AT_Prevalente")))
                            .SetAttribute("forma_possesso", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Forma_Possesso")))
                            .SetAttribute("sup_sau_convenzionale", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_SAU_Convenzionale")))
                            .SetAttribute("sup_sau_conversione", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_SAU_Conversione")))
                            .SetAttribute("sup_sau_biologico", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_SAU_Biologico")))
                            .SetAttribute("sup_totale", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_Totale")))
                            .SetAttribute("sup_tare", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_Tare")))
                            .SetAttribute("sup_sau", Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sup_SAU")))
                        End With

                    End If

                    '#################################
                    '##########    TIPO     ##########
                    '#################################

                    'Mi procuro il tipo dell'impresa
                    Dim objtipo As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                    Dim dtTipo As DataTable

                    'Mi procuro il recordset richiesto
                    dtTipo = objtipo.Leggi(CStr(Agro_SQL_Load(DtCentri.Rows(iCen).Item("PIVA"))),
                                           CLng(Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sa_Cod"))),
                                           0,
                                           "TIPO_CA",
                                           2,
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           "",
                                           objParametri)


                    'Se ottengo almeno un risultato, creo la struttura XML
                    If dtTipo.Rows.Count <> 0 Then
                        XmlCentroAziendale.SetAttribute("tipo", dtTipo.Rows(0).Item("Id_Cod"))
                        XmlCentroAziendale.SetAttribute("descrizione", dtTipo.Rows(0).Item("Descrizione"))
                    Else
                        XmlCentroAziendale.SetAttribute("tipo", 101) 'Default
                        XmlCentroAziendale.SetAttribute("descrizione", "Sede Legale")
                    End If

                    dtTipo = Nothing

                    '#################################
                    '##########  INDIRIZZI  ##########
                    '#################################

                    'Mi procuro un elenco degli indirizzi dell'impresa
                    Dim objCentrixIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                    Dim DtIndirizzi As DataTable
                    Dim XmlIndirizzo As XmlElement

                    'Mi procuro il recordset richiesto
                    DtIndirizzi = objCentrixIndirizzi.Leggi(CStr(Agro_SQL_Load(DtCentri.Rows(iCen).Item("PIVA"))),
                                                            CLng(Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sa_Cod"))),
                                                            0,
                                                            0,
                                                            enumSelezioneVariabile.Selezione_JoinCompleta,
                                                            "",
                                                            "",
                                                            objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtIndirizzi.Rows.Count <> 0 Then

                        'Effettuo un ciclo sugli indirizzi
                        For iInd As Integer = 0 To DtIndirizzi.Rows.Count - 1

                            '----- < INDIRIZZO > -----

                            XmlIndirizzo = XmlDoc.CreateElement("Indirizzo")

                            With XmlIndirizzo
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("tipo_indirizzo", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Tipo_Indirizzo")))
                                .SetAttribute("cod_indirizzo", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("cod_indirizzo")))
                                .SetAttribute("ind_des", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("ind_des")))
                                .SetAttribute("frz_des", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("frz_des")))
                                .SetAttribute("cap", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("CAP")))
                                .SetAttribute("com_des", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("com_des")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("pro_cod")))
                                .SetAttribute("pro_des", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("pro_des")))
                                .SetAttribute("stato", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("stato")))
                                .SetAttribute("note", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("note")))
                                .SetAttribute("pro_cod_istat", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("pro_cod_istat")))
                                .SetAttribute("com_cod_istat", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("com_cod_istat")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Username_Modifica")))

                                .SetAttribute("validazione", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Indirizzi_Validazione")))
                                .SetAttribute("data_validazione", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Indirizzi_Data_Validazione")))
                                .SetAttribute("username_validazione", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Indirizzi_UserName_Validazione")))
                                .SetAttribute("codice_lingua", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Codice_Lingua")))
                                .SetAttribute("codice_alternativo", Agro_SQL_Load(DtIndirizzi.Rows(iInd).Item("Codice_Alternativo")))
                            End With

                            XmlCentroAziendale.AppendChild(XmlIndirizzo)
                            '----- < / INDIRIZZO > -----

                        Next


                    End If

                    XmlIndirizzo = Nothing
                    DtIndirizzi = Nothing
                    objCentrixIndirizzi = Nothing


                    '#################################
                    '##########  RUBRICA  ############
                    '#################################

                    'Mi procuro un elenco delle voci di rubrica del centro aziendale
                    Dim objCentrixRubrica As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
                    Dim DtRubrica As New DataTable
                    Dim XmlRubrica As XmlElement

                    'Mi procuro il recordset richiesto
                    DtRubrica = objCentrixRubrica.Leggi(CStr(Agro_SQL_Load(DtCentri.Rows(iCen).Item("PIVA"))),
                                                        CStr(Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sa_Cod"))),
                                                        0,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRubrica.Rows.Count <> 0 Then

                        'Effettuo un ciclo sulla rubrica
                        For iRub As Integer = 0 To DtRubrica.Rows.Count - 1

                            '----- < RUBRICA > -----
                            XmlRubrica = XmlDoc.CreateElement("Rubrica")

                            With XmlRubrica
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("cod_rubrica", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("cod_rubrica")))
                                .SetAttribute("numero", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("numero")))
                                .SetAttribute("descr", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("descr")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRubrica.Rows(iRub).Item("Username_Modifica")))
                            End With

                            XmlCentroAziendale.AppendChild(XmlRubrica)
                            '----- < / PERSONA > -----

                        Next

                    End If

                    XmlRubrica = Nothing
                    DtRubrica = Nothing
                    objCentrixRubrica = Nothing

                    '#################################
                    '##########  CODICI  #############
                    '#################################

                    'Mi procuro un elenco dei codici del centro aziendale
                    Dim objCentrixCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                    Dim DtCodici As DataTable
                    Dim XmlCodice As XmlElement

                    'Mi procuro il recordset richiesto
                    DtCodici = objCentrixCodici.Leggi(CStr(Agro_SQL_Load(DtCentri.Rows(iCen).Item("PIVA"))),
                                                      CLng(Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sa_Cod"))),
                                                      0, "",
                                                      "",
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "",
                                                      "",
                                                      objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtCodici.Rows.Count <> 0 Then

                        'Effettuo un ciclo sugli indirizzi
                        'Do While Not RsCodici.EOF
                        For iCod As Integer = 0 To DtCodici.Rows.Count - 1

                            '----- < CODICE > -----
                            XmlCodice = XmlDoc.CreateElement("Codice")

                            With XmlCodice
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_cod", Agro_SQL_Load(DtCodici.Rows(iCod).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(DtCodici.Rows(iCod).Item("val_cod")))
                                .SetAttribute("descrizione", Agro_SQL_Load(DtCodici.Rows(iCod).Item("descrizione")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtCodici.Rows(iCod).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtCodici.Rows(iCod).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtCodici.Rows(iCod).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtCodici.Rows(iCod).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtCodici.Rows(iCod).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtCodici.Rows(iCod).Item("Username_Modifica")))
                            End With

                            XmlCentroAziendale.AppendChild(XmlCodice)
                            '----- < / CODICE > -----

                        Next

                    End If

                    XmlCodice = Nothing
                    DtCodici = Nothing
                    objCentrixCodici = Nothing



                    '#################################################
                    '##########  CARATTERISTICHE CANTINA  ############
                    '#################################################

                    'Mi procuro un eventuale elenco delle caratteristiche della cantina

                    Dim ObjCantina_Caratteristiche As New AgronicaCoreAnagrafeDAL.Cantina_Caratter_R
                    Dim DtCantina_Caratteristiche As DataTable
                    Dim XmlCaratteristica As XmlElement

                    Dim ObjCantina_Pareti As New AgronicaCoreAnagrafeDAL.Cantina_Pareti_R
                    Dim DtCantina_Pareti As DataTable
                    Dim XmlParete As XmlElement

                    'Mi procuro il recordset richiesto
                    DtCantina_Caratteristiche = ObjCantina_Caratteristiche.Leggi(
                                                       CStr(Agro_SQL_Load(DtCentri.Rows(iCen).Item("PIVA"))),
                                                       CLng(Agro_SQL_Load(DtCentri.Rows(iCen).Item("Sa_Cod"))),
                                                        0,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri)


                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtCantina_Caratteristiche.Rows.Count <> 0 Then

                        'Effettuo un ciclo sulle caratteristiche
                        For iCan As Integer = 0 To DtCantina_Caratteristiche.Rows.Count - 1

                            '----- < cantina_Caratteristica > -----
                            XmlCaratteristica = XmlDoc.CreateElement("Cantina_Caratteristica")

                            With XmlCaratteristica
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("sa_cod")))
                                .SetAttribute("piano_cod", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("piano_cod")))
                                .SetAttribute("piano_des", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("piano_des")))
                                .SetAttribute("dimx", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("dimx")))
                                .SetAttribute("dimy", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("dimy")))
                                .SetAttribute("colore_interno", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("colore_interno")))
                                .SetAttribute("colore_esterno", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("colore_esterno")))
                                .SetAttribute("spessore", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("spessore")))
                                .SetAttribute("riempimento", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("riempimento")))
                                .SetAttribute("zoom", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("zoom")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("validita_fine")))
                            End With


                            '#################################################
                            '##############  PARETI CANTINA  #################
                            '#################################################

                            'Mi procuro un eventuale elenco delle pareti del piano della cantina


                            'Mi procuro il recordset richiesto
                            DtCantina_Pareti = ObjCantina_Pareti.Leggi(
                                                               CStr(Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("PIVA"))),
                                                               CLng(Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("Sa_Cod"))),
                                                               CLng(Agro_SQL_Load(DtCantina_Caratteristiche.Rows(iCan).Item("Piano_Cod"))),
                                                               0,
                                                               AGRODATAINIZIO,
                                                                AGRODATAFINE,
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "",
                                                                "",
                                                                objParametri)

                            'Se ottengo almeno un risultato, creo la struttura XML
                            If DtCantina_Pareti.Rows.Count <> 0 Then

                                'Effettuo un ciclo sulle pareti
                                For iCanC As Integer = 0 To DtCantina_Pareti.Rows.Count - 1

                                    '----- < Cantina_Parete > -----
                                    XmlParete = XmlDoc.CreateElement("Cantina_Parete")

                                    With XmlParete
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("piva", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("piva")))
                                        .SetAttribute("sa_cod", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("sa_cod")))
                                        .SetAttribute("piano_cod", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("piano_cod")))
                                        .SetAttribute("parete_cod", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("parete_cod")))
                                        .SetAttribute("dimx", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("dimx")))
                                        .SetAttribute("dimy", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("dimy")))
                                        .SetAttribute("colore_interno", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("colore_interno")))
                                        .SetAttribute("colore_esterno", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("colore_esterno")))
                                        .SetAttribute("posx", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("posx")))
                                        .SetAttribute("posy", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("posy")))
                                        .SetAttribute("spessore", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("spessore")))
                                        .SetAttribute("riempimento", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("riempimento")))
                                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("validita_inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(DtCantina_Pareti.Rows(iCanC).Item("validita_fine")))
                                    End With


                                    XmlCaratteristica.AppendChild(XmlParete)
                                    '----- < / Cantina_Parete > -----

                                Next

                            End If


                            '#################################
                            '#################################
                            '#################################


                            XmlCentroAziendale.AppendChild(XmlCaratteristica)
                            '----- < / Cantina_Caratteristica > -----

                        Next

                    End If

                    XmlCaratteristica = Nothing
                    DtCantina_Caratteristiche = Nothing
                    ObjCantina_Caratteristiche = Nothing
                    XmlParete = Nothing
                    DtCantina_Pareti = Nothing
                    ObjCantina_Pareti = Nothing

                    If LeggiXmlDaSQLServer Then
                        '#################################
                        '###     SFONDI GRAFICI ##########
                        '#################################
                        Dim ObjCentriXSfondi As AgronicaCoreGraficaDAL.CentriXSfondi_Read
                        ObjCentriXSfondi = New AgronicaCoreGraficaDAL.CentriXSfondi_Read

                        Dim sXmlCentriXSfondi As String = ObjCentriXSfondi.LeggiXML(PIVA,
                                                                                    Sa_Cod,
                                                                                    0,
                                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                    "",
                                                                                    "",
                                                                                    objParametri
                                                                                    )

                        If sXmlCentriXSfondi <> "" Then

                            Dim xListaSfondi As New XmlDocument
                            xListaSfondi.LoadXml(sXmlCentriXSfondi)

                            XmlCentroAziendale.AppendChild(XmlDoc.ImportNode(xListaSfondi.FirstChild, True))

                        End If

                    End If

                    '#################################
                    '#################################
                    '#################################




                    XmlDatiCentriAziendali.AppendChild(XmlCentroAziendale)
                    '----- < / CENTROAZIENDALE > -----


                Next iCen


                XmlDoc.AppendChild(XmlDatiCentriAziendali)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlCentroAziendale = Nothing
                XmlDatiCentriAziendali = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun centro aziendale ...
                RisultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtCentri = Nothing
            objUtentixStrutture = Nothing

            '------------------------------

            'Assegno il risultato
            Return RisultatoFunzione

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            'Restituisco un valore Dummy
            Return ""

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try


    End Function

    Public Function Centro_Leggi_Anagrafica(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Leggi_Impresa As Boolean,
                                            ByVal Leggi_Indirizzo As Boolean,
                                            ByVal Leggi_Codici As Boolean,
                                            ByVal Leggi_Rubrica As Boolean,
                                            ByVal Leggi_Catasto As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale

        If Piva = "" Then
            Throw New Exception("Piva Obbligatoria")
        End If

        If Sa_Cod = 0 Then
            Throw New Exception("Sa_Cod Obbligatorio")
        End If

        Dim centro As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva))
        Dim DT_Centro As DataTable

        Dim objCentriR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        DT_Centro = objCentriR.Leggi(Piva, Sa_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If DT_Centro.Rows.Count > 0 Then

            If Leggi_Impresa Then
                Dim Impresa_R As New Impresa_R
                'centro.Impresa = Impresa_R.Impresa_Leggi_Anagrafica(Piva, True, False, False, False, objParametri_Server)
            End If

            Dim DR_Centro = DT_Centro.Rows(0)

            centro.nome = DR_Centro("Sa_Nome")
            centro.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CDate(DR_Centro("Validita_Inizio")), CDate(DR_Centro("Validita_Fine")))

            If Leggi_Indirizzo Then
                Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
                centro.indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Centro(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)
            End If

            If Leggi_Codici Then
                Dim objCodici As New AgronicaCoreAnagrafeBIZ.Codici_R
                Dim codici = objCodici.Leggi_Codici_Centro(centro.primaryKey.partitaIva,
                                                              centro.primaryKey.codice,
                                                              objParametri_Server,
                                                              New List(Of enum_CodiciAnagrafe)({0,
                                                                    enum_CodiciAnagrafe.Centro_Sede_Legale,
                                                                    enum_CodiciAnagrafe.Centro_Sede_Aziendale,
                                                                    enum_CodiciAnagrafe.Centro_Stabilimento,
                                                                    enum_CodiciAnagrafe.TitoloPossesso,
                                                                    enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO,
                                                                    enum_CodiciAnagrafe.TipoAttivita,
                                                                    enum_CodiciAnagrafe.OTE,
                                                                    enum_CodiciAnagrafe.Centro_Aziendale_Esterno_Collegato}),
                                                              True)

                codici = (From c In codici Where c.codiceAnagrafe.codice < 2000).ToList

                centro.codici = codici

            End If

            centro.lng = DR_Centro("long")

            If Leggi_Rubrica Then

                Dim objRubrica As New AgronicaCoreAnagrafeBIZ.Rubrica_R
                centro.rubricaVoci = objRubrica.Leggi_Rubrica_Centro(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)

            End If

            'If Leggi_Catasto Then

            'End If


            centro.lat = DR_Centro("lat")
            centro.lng = DR_Centro("long")


            centro.tipologia = imposta_TipologiaSede(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)
            centro.titolo_Di_Possesso = imposta_TitoloPossesso(centro.primaryKey.partitaIva, centro.primaryKey.codice, DT_Centro.Rows(0)("TitoloPossesso"), objParametri_Server)

            centro.bioOrganismoDiControllo = imposta_bioOrganismoDiControllo(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)
            centro.centroAziendaleEsternoCollegato = imposta_centroAziendaleEsternoCollegato(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)
            centro.bioTipoAttivita = imposta_bioTipoAttivita(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)
            centro.orientamentoTecnicoEconomico = imposta_orientamentoTecnicoEconomico(centro.primaryKey.partitaIva, centro.primaryKey.codice, objParametri_Server)

        End If

        Return centro
    End Function

    Public Function imposta_TipologiaSede(Piva As String, sa_cod As Integer, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.TipologiaSede
        Dim objCodici_R As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim dt_codici = objCodici_R.Leggi(Piva,
                          sa_cod,
                          0, "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          " Centri_Aziendali_Codici.id_Cod IN (101, 102, 103) ", "", objParametri_Server)

        If dt_codici.Rows.Count = 0 Then
            Return New AgronicaCoreModelsSTD.metaschema.TipologiaSede(0)
        End If

        Dim tipologia = New AgronicaCoreModelsSTD.metaschema.TipologiaSede(dt_codici.Rows("0")("id_cod"))
        tipologia.descrizione = dt_codici.Rows("0")("descrizione")
        Return tipologia
    End Function

    Public Function imposta_TitoloPossesso(Piva As String, sa_cod As Integer, titoloDiPossesso_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso

        Dim objCodici_R As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read

        Dim dt_codici = objCodici_R.Leggi(Piva, sa_cod, 1016, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim titoloDiPossesso As Integer
        If dt_codici.Rows.Count = 0 Then
            titoloDiPossesso = titoloDiPossesso_Cod
        End If

        If dt_codici.Rows.Count > 0 AndAlso Not IsNumeric(dt_codici.Rows("0")("val_cod")) Then
            titoloDiPossesso = titoloDiPossesso_Cod
        ElseIf dt_codici.Rows.Count > 0 AndAlso IsNumeric(dt_codici.Rows("0")("val_cod")) Then
            titoloDiPossesso = dt_codici.Rows("0")("val_cod")
        End If

        Dim titolo = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(titoloDiPossesso)

        Select Case titoloDiPossesso
            Case enum_TitoloPossesso.Altro
                titolo.descrizione = "Altro"
            Case enum_TitoloPossesso.Proprieta
                titolo.descrizione = "Proprietà"
            Case enum_TitoloPossesso.Comodato
                titolo.descrizione = "Comodato d'uso"
            Case enum_TitoloPossesso.AffittoContratto
                titolo.descrizione = "Affitto con contratto"
            Case enum_TitoloPossesso.AffittoSenzaContratto
                titolo.descrizione = "Affitto senza contratto"
            Case enum_TitoloPossesso.InContoTerzi
                titolo.descrizione = "In conto terzi"
            Case enum_TitoloPossesso.InConvenzione
                titolo.descrizione = "In convenzione"
            Case enum_TitoloPossesso.InCompartecipazione
                titolo.descrizione = "In compartecipazione"
            Case Else
                titolo.descrizione = ""
        End Select

        Return titolo
    End Function

    Public Function imposta_bioOrganismoDiControllo(Piva As String, sa_cod As Integer, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo
        Dim objCodici_R As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim dt_codici = objCodici_R.Leggi(Piva,
                          sa_cod,
                          enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO, "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "", "", objParametri_Server)

        If dt_codici.Rows.Count = 0 Then
            Return New AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo(0)
        End If

        Dim objOrganismoControllo As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R
        Dim dtMeta = objOrganismoControllo.Leggi(dt_codici.Rows("0")("val_cod"), "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        If dtMeta.Rows.Count = 0 Then
            Return New AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo(0)
        End If

        Dim tipologia = New AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo(dt_codici.Rows("0")("val_cod"))
        tipologia.descrizione = dtMeta.Rows("0")("Organismo_Des")
        Return tipologia

    End Function

    Public Function imposta_centroAziendaleEsternoCollegato(Piva As String, sa_cod As Integer,
                                                            objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.CentroAziendaleEsternoCollegato
        Dim objCodici_R As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim dt_codici = objCodici_R.Leggi(Piva,
                          sa_cod,
                          enum_CodiciAnagrafe.Centro_Aziendale_Esterno_Collegato, "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "", "", objParametri_Server)

        If dt_codici.Rows.Count = 0 Then
            Return New AgronicaCoreModelsSTD.metaschema.CentroAziendaleEsternoCollegato("")
        End If

        Dim centroAziendaleEsternoCollegato As New AgronicaCoreModelsSTD.metaschema.CentroAziendaleEsternoCollegato("")

        centroAziendaleEsternoCollegato.codice = dt_codici.Rows(0)("val_cod")
        Return centroAziendaleEsternoCollegato

    End Function

    Public Function imposta_bioTipoAttivita(Piva As String, sa_cod As Integer, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.BioTipoAttivita
        Dim objCodici_R As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim dt_codici = objCodici_R.Leggi(Piva,
                          sa_cod,
                          enum_CodiciAnagrafe.TipoAttivita, "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "", "", objParametri_Server)

        If dt_codici.Rows.Count = 0 Then
            Return New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita(0)
        End If

        Dim descrizione = ""
        Select Case CStr(dt_codici.Rows("0")("val_cod"))
            Case "PV"
                descrizione = "Produzione vegetale"
            Case "PZ"
                descrizione = "Produzione zootecnica"
            Case "PVZ"
                descrizione = "Produzione vegetale e zootecnica"
            Case "TPV"
                descrizione = "Preparazione vegetale"
            Case "TPZ"
                descrizione = "Preparazione zootecnica"
            Case "TPVZ"
                descrizione = "Preparazione vegetale e zootecnica"
            Case "I"
                descrizione = "Importazione"
            Case "RS"
                descrizione = "Raccolta spontanea"
            Case "P/TP"
                descrizione = "Produzione / Preparazione"
            Case "TP/I"
                descrizione = "Preparazione / Importazione"
            Case "@"
                descrizione = "Altro"
        End Select

        Dim tipologia = New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita(dt_codici.Rows("0")("val_cod"))
        tipologia.descrizione = descrizione
        Return tipologia
    End Function

    Public Function imposta_orientamentoTecnicoEconomico(Piva As String, sa_cod As Integer, objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico)
        Dim list_OTE As New List(Of AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico)
        Dim objCodici_R As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim dt_codici = objCodici_R.Leggi(Piva,
                          sa_cod,
                          enum_CodiciAnagrafe.OTE, "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "", "", objParametri_Server)

        If dt_codici.Rows.Count = 0 Then
            Return list_OTE
        End If

        Dim objOTE_R As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
        Dim OTE_str As String = dt_codici.Rows(0)("val_cod")

        Dim OTE_Arr = OTE_str.Split("|")
        For Each el In OTE_Arr
            el = el.Trim
            If el <> "" Then
                Dim ote_el As New AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico(el)

                Dim dtOte = objOTE_R.Leggi(el, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtOte.Rows.Count > 0 Then
                    ote_el.descrizione = dtOte.Rows(0)("OTE_des")
                End If

                list_OTE.Add(ote_el)
            End If
        Next

        Return list_OTE
    End Function

    Public Function Carica_centriXImprese(piveList As List(Of String),
                                          obj_Server As AgronicaCoreParametri,
                                          obj_Utenti As AgronicaCoreParametri) As IEnumerable(Of Object)
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim DT_CentriImprese As DataTable = objCentri.Leggi_CentriXImprese("", False, obj_Server)

        Dim CentriImprese = (From row In DT_CentriImprese.Rows
                             Select (New With {
                                 .piva = row(0),
                                 .rag_soc = row(1),
                                 .sa_cod = row(2),
                                 .sa_nome = row(3)
                         })).ToList()

        Dim listItems = CentriImprese.Where(Function(c)
                                                Return piveList.Contains(c.piva)
                                            End Function)

        Return listItems
    End Function

    Public Function Leggi_Centri_APP(piva As String, data As Date, ByRef objParametri_Server As AgronicaCoreParametri, Optional ByVal leggiSoloAttivi As Boolean = True) As List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

        Dim objCodici As New Codici_R
        Dim objIndirizzi As New Indirizzi_R
        Dim objFabbricati As New Fabbricato_R
        Dim objCentri As New CentriAziendali_Read
        Dim codiciEsclusi = New List(Of enum_CodiciAnagrafe) ' codici anagrafe da escludere

        Dim xFiltroAggiuntivo As New StringBuilder
        If leggiSoloAttivi Then
            xFiltroAggiuntivo.AppendLine(" ( Validita_inizio <= " & Agro_SQL_SaveDate(data) & " ")
            xFiltroAggiuntivo.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(data) & " ) ")
        Else
            xFiltroAggiuntivo.AppendLine(" Validita_Fine >= " & Agro_SQL_SaveDate(data) & " ")
        End If
        Dim filtro = xFiltroAggiuntivo.ToString
        Dim ordine As String = " Sa_Nome "

        Dim centriAziendali As New List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)
        Dim dtCentriAziendali = objCentri.Leggi(piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, ordine, objParametri_Server)

        For Each row In dtCentriAziendali.Rows

            Dim centro = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale() With {
                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK() With {
                    .partitaIva = row("piva"), .codice = row("sa_cod")
                },
                .nome = row("sa_nome"),
                .lat = row("lat"),
                .lng = row("long")
            }

            centro.indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Centro(row("piva"), row("sa_cod"), objParametri_Server)
            centro.codici = objCodici.Leggi_Codici_Centro(row("piva"), row("sa_cod"), objParametri_Server, codiciEsclusi, True)
            centro.fabbricati = objFabbricati.Leggi_Fabbricati_APP(row("piva"), row("sa_cod"), objParametri_Server)

            centriAziendali.Add(centro)

        Next

        Return centriAziendali

    End Function

    Public Function readCentreAddresses(
                                       ByVal piva As String,
                                       ByVal sa_cod As Integer,
                                       ByVal objParametri As AgronicaCoreParametri
                                       ) As List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)

        If piva = "" Then
            Throw New Exception("Piva Obbligatoria")
        End If

        If sa_cod = 0 Then
            Throw New Exception("Sa_Cod Obbligatorio")
        End If

        Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
        Dim indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Centro(piva, sa_cod, objParametri)

        Return indirizzi

    End Function

    Public Function ReadLatLng(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional piva As String = "",
                               Optional saCod As Int32 = 0) As List(Of KeyValuePair(Of KeyValuePair(Of String, Int32), LatLng))

        Dim biz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim cxc As New List(Of KeyValuePair(Of KeyValuePair(Of String, Int32), LatLng))
        Dim dt = biz.ReadLatLng(objParametri, piva, saCod)

        For Each row In dt.Rows
            Dim cc As New KeyValuePair(Of KeyValuePair(Of String, Int32), LatLng)(
                New KeyValuePair(Of String, Integer)(row("PIVA"), row("sa_cod")),
                New LatLng(row("lat"), row("long"))
                )
            cxc.Add(cc)
        Next

        Return cxc
    End Function
End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CentroAziendale_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CentroAziendale_Scrivi(ByVal DatiCentroAziendale As String,
                                           ByRef OUTPUT_Piva As String,
                                           ByRef OUTPUT_Sa_Cod As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByVal TipoG2G As Integer = 0,
                                           Optional NoteLog As String = ""
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Centro_Aziendale_W.CentroAziendale_Scrivi()"

        Dim XmlDoc As XmlDocument

        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objUtentixStrutture As AgronicaCoreAnagrafeDAL.UtentixStrutture_Write
        Dim objCentri As AgronicaCoreAnagrafeDAL.CentriAziendali_Write
        Dim objImpresexParticelle As AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
        Dim objCentrixIndirizzi As AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Write
        Dim objIndirizzi As AgronicaCoreAnagrafeDAL.Indirizzi_Write
        Dim objCentrixRubrica As AgronicaCoreAnagrafeDAL.CentrixRubrica_Write
        Dim objRubrica As AgronicaCoreAnagrafeDAL.Rubrica_Write
        Dim objCentrixCodici As AgronicaCoreAnagrafeDAL.Centri_Codici_Write
        'Dim ObjCampi As AgronicaCoreAnagrafeDAL.Campi_W
        'Dim objUtentixCampi As AgronicaCoreAnagrafeDAL.UtentixCampi_Write
        'Dim objCampixParticelle As AgronicaCoreAnagrafeDAL.CampixParticelle_W
        'Dim objAppezzamenti As AgronicaCoreAnagrafeDAL.Appezzamento_Write
        'Dim objUtentixAppezzamenti As AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
        'Dim objAppezzaxParticelle As AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
        Dim ObjLeggiParticelle As AgronicaCoreAnagrafeBIZ.Particella_R
        Dim ObjCancellaParticelle As AgronicaCoreAnagrafeBIZ.Particella_W
        'Dim objReg_Impianti As AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
        Dim ObjCantina_Caratteristiche As Object
        Dim ObjCantina_Pareti As Object

        'Figlio Maggiore = Campo

        Dim objCampiLeggi As AgronicaCoreAnagrafeBIZ.Campo_R
        Dim objCampiScrivi As AgronicaCoreAnagrafeBIZ.Campo_W
        Dim objAppezzamentiLeggi As AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim objAppezzamentiScrivi As AgronicaCoreAnagrafeBIZ.Appezzamento_W
        Dim objAgendaLeggi As AgronicaCoreContabBIZ.Agenda_R
        Dim objAgendaScrivi As AgronicaCoreContabBIZ.Agenda_W
        Dim objFabbricatiLeggi As AgronicaCoreAnagrafeBIZ.Fabbricato_R
        Dim objFabbricatiScrivi As AgronicaCoreAnagrafeBIZ.Fabbricato_W
        'Dim objFabbricati_AD As AgronicaCoreAnagrafeDAL.Fabbricati_W
        Dim objGrafica As AgronicaCoreGraficaDAL.Grafica_Write
        Dim objAZIInfo As AgronicaCoreGraficaDAL.AziInfoRER_Write
        Dim ObjCentriXSfondi As AgronicaCoreGraficaDAL.CentriXSfondi_Write


        Dim XmlParticelle As String
        Dim XmlCampi As String
        Dim XmlAppezzamenti As String
        Dim XmlAgenda As String
        Dim XmlFabbricati As String

        Dim Dummy As Long
        Dim Cod_Indirizzo As Long
        Dim Cod_Rubrica As Long
        Dim Sa_Cod As Long

        Dim xDatiCentriAziendali As XmlNodeList
        Dim xDatiCentroAziendale As XmlElement
        Dim xCentriAziendali As XmlNodeList
        Dim xCentroAziendale As XmlElement
        Dim xIndirizzi As XmlNodeList
        Dim xIndirizzo As XmlElement
        Dim xRubriche As XmlNodeList
        Dim xRubrica As XmlElement
        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement

        Dim i_DatiCentroAziendale As Integer
        Dim i_CentroAziendale As Integer
        Dim i_Indirizzo As Integer
        Dim i_Rubrica As Integer
        Dim i_Codice As Integer

        Dim OpeDB_CentroAziendale As String
        Dim OpeDB_Indirizzo As String
        Dim OpeDB_Rubrica As String
        Dim OpeDB_Codice As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        Dim xPiva As String = ""
        '------------------------------

        Dim objAgronicaLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W


        'Gestione degli errori
        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            '------------------------------

            XmlDoc = New XmlDocument
            XmlDoc.LoadXml(DatiCentroAziendale)

            '------------------------------

            xDatiCentriAziendali = XmlDoc.GetElementsByTagName("DatiCentriAziendali")

            i_DatiCentroAziendale = 0

            Do While i_DatiCentroAziendale < xDatiCentriAziendali.Count

                'Prelevo l'i-esimo blocco di DatiCentriAziendali (in realta' ne esiste uno solo)
                xDatiCentroAziendale = xDatiCentriAziendali.Item(i_DatiCentroAziendale)

                '------------------------------

                xCentriAziendali = xDatiCentroAziendale.GetElementsByTagName("CentroAziendale")

                i_CentroAziendale = 0

                Do While i_CentroAziendale < xCentriAziendali.Count

                    'Prelevo l' i-esimo Centro Aziendale
                    xCentroAziendale = xCentriAziendali.Item(i_CentroAziendale)

                    'Prelevo gli attributi dell'impresa selezionata
                    OpeDB_CentroAziendale = xCentroAziendale.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    objCentri = New AgronicaCoreAnagrafeDAL.CentriAziendali_Write
                    objUtentixStrutture = New AgronicaCoreAnagrafeDAL.UtentixStrutture_Write
                    objImpresexParticelle = New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
                    'TODO
                    'ObjCantina_Caratteristiche = CreateObject("Agro_Anagrafe_AD.Cantina_Caratter_W")
                    'ObjCantina_Pareti = CreateObject("Agro_Anagrafe_AD.Cantina_Pareti_W")


                    'Inizializzo Preventivamente il sa_Cod
                    Sa_Cod = CInt(xCentroAziendale.GetAttribute("sa_cod"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_CentroAziendale

                        Case "0"    'LEGGI -------------------------------------------------------

                            'Preparo il valore da restituire in uscita
                            OUTPUT_Piva = CStr(xCentroAziendale.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xCentroAziendale.GetAttribute("sa_cod"))


                        Case "1"    'SALVA -------------------------------------------------------

                            If Sa_Cod <= 0 Then

                                Sa_Cod = objSequenze.NuovoId_CentriAziendali(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                             CInt(xCentroAziendale.GetAttribute("basecode")),
                                                                             CInt(xCentroAziendale.GetAttribute("topcode")),
                                                                             objParametri)
                            Else
                                'Esportazione del Centro Aziendale in Locale

                            End If

                            Dummy = objCentri.Scrivi(
                                     CStr(xCentroAziendale.GetAttribute("piva")),
                                     Sa_Cod,
                                     CStr(xCentroAziendale.GetAttribute("sa_nome")),
                                     CDbl(xCentroAziendale.GetAttribute("x")),
                                     CDbl(xCentroAziendale.GetAttribute("y")),
                                     CDbl(xCentroAziendale.GetAttribute("zslm")),
                                     CDbl(xCentroAziendale.GetAttribute("long")),
                                     CDbl(xCentroAziendale.GetAttribute("lat")),
                                     CDbl(xCentroAziendale.GetAttribute("area")),
                                     Agro_XML_GetString(xCentroAziendale, "ca_sipi", ""),
                                     Agro_XML_GetString(xCentroAziendale, "at_prevalente", ""),
                                     Agro_XML_GetString(xCentroAziendale, "forma_possesso", ""),
                                     CInt(xCentroAziendale.GetAttribute("titolopossesso")),
                                     Agro_XML_GetDecimal(xCentroAziendale, "sup_sau_convenzionale", 0),
                                     Agro_XML_GetDecimal(xCentroAziendale, "sup_sau_conversione", 0),
                                     Agro_XML_GetDecimal(xCentroAziendale, "sup_sau_biologico", 0),
                                     Agro_XML_GetDecimal(xCentroAziendale, "sup_totale", 0),
                                     CDbl(xCentroAziendale.GetAttribute("sup_bosco")),
                                     Agro_XML_GetDecimal(xCentroAziendale, "sup_tare", 0),
                                     Agro_XML_GetDecimal(xCentroAziendale, "sup_sau", 0),
                                     CDbl(xCentroAziendale.GetAttribute("sup_prati")),
                                     CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                                     CDate(xCentroAziendale.GetAttribute("validita_fine")),
                                     objParametri,
                                     objParametri_Utenti,
                                     Data_creazione:=Agro_XML_GetDate(xCentroAziendale, "data_creazione", #2/1/1900#),
                                     Data_modifica:=Agro_XML_GetDate(xCentroAziendale, "data_modifica", #2/1/1900#),
                                     username_creazione:=Agro_XML_GetString(xCentroAziendale, "username_creazione", ""),
                                     username_modifica:=Agro_XML_GetString(xCentroAziendale, "username_modifica", "")
                                     )

                            Dummy = objUtentixStrutture.Scrivi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                               Sa_Cod,
                                                               CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                                                               CDate(xCentroAziendale.GetAttribute("validita_fine")),
                                                               objParametri,
                                                               Data_creazione:=Agro_XML_GetDate(xCentroAziendale, "data_creazione", #2/1/1900#),
                                                               Data_modifica:=Agro_XML_GetDate(xCentroAziendale, "data_modifica", #2/1/1900#),
                                                               username_creazione:=Agro_XML_GetString(xCentroAziendale, "username_creazione", ""),
                                                               username_modifica:=Agro_XML_GetString(xCentroAziendale, "username_modifica", "")
                                                               )

                            'Preparo il valore da restituire in uscita
                            OUTPUT_Piva = CStr(xCentroAziendale.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = Sa_Cod


                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************
                            objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_CentroAziendale),
                                                           enum_TipoEntita_Des.CentriAziendali,
                                                           CStr(xCentroAziendale.GetAttribute("piva")),
                                                           CStr(xCentroAziendale.GetAttribute("sa_cod")),
                                                           Nothing, Nothing, Nothing, Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri)
                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                        Case "2"    'MODIFICA -------------------------------------------------------

                            xPiva = CStr(xCentroAziendale.GetAttribute("piva"))

                            objCentri.Modifica(CStr(xCentroAziendale.GetAttribute("piva")),
                                               CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                               CStr(xCentroAziendale.GetAttribute("sa_nome")),
                                               CDbl(xCentroAziendale.GetAttribute("x")),
                                               CDbl(xCentroAziendale.GetAttribute("y")),
                                               CDbl(xCentroAziendale.GetAttribute("zslm")),
                                               CDbl(xCentroAziendale.GetAttribute("long")),
                                               CDbl(xCentroAziendale.GetAttribute("lat")),
                                               CDbl(xCentroAziendale.GetAttribute("area")),
                                               Agro_XML_GetString(xCentroAziendale, "ca_sipi", ""),
                                               Agro_XML_GetString(xCentroAziendale, "at_prevalente", ""),
                                               Agro_XML_GetString(xCentroAziendale, "forma_possesso", ""),
                                               CInt(xCentroAziendale.GetAttribute("titolopossesso")),
                                               Agro_XML_GetDecimal(xCentroAziendale, "sup_sau_convenzionale", 0),
                                               Agro_XML_GetDecimal(xCentroAziendale, "sup_sau_conversione", 0),
                                               Agro_XML_GetDecimal(xCentroAziendale, "sup_sau_biologico", 0),
                                               Agro_XML_GetDecimal(xCentroAziendale, "sup_totale", 0),
                                               CDbl(xCentroAziendale.GetAttribute("sup_bosco")),
                                               Agro_XML_GetDecimal(xCentroAziendale, "sup_tare", 0),
                                               Agro_XML_GetDecimal(xCentroAziendale, "sup_sau", 0),
                                               CDbl(xCentroAziendale.GetAttribute("sup_prati")),
                                               CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                                               CDate(xCentroAziendale.GetAttribute("validita_fine")),
                                               "",
                                               objParametri,
                                               Data_modifica:=Agro_XML_GetDate(xCentroAziendale, "data_modifica", #2/1/1900#),
                                               username_modifica:=Agro_XML_GetString(xCentroAziendale, "username_modifica", "")
                                               )

#Region "Commentato"
                            'Aggiorno le Validità inizio e fine

                            'ALLINEO le validita delle relazioni ImpresexParticelle
                            ' ATTENZIONE
                            ' 09/12/2009 BACO 
                            'objImpresexParticelle.AggiornaValidita( _
                            '            CStr(xCentroAziendale.GetAttribute("piva")), _
                            '            CInt(xCentroAziendale.GetAttribute("sa_cod")), _
                            '            CStr(Username_Operazione), _
                            '            CDate(xCentroAziendale.GetAttribute("validita_inizio")), _
                            '            CDate(xCentroAziendale.GetAttribute("validita_fine")), _
                            '            objConnessione, _
                            '            objTransazione, _
                            '            StringaConnessione, _
                            '            DirectoryLOG, _
                            '            FileLOG, _
                            '            IdentificatoreUtente)


                            'objUtentixStrutture.AggiornaValiditaInizio( _
                            '           CStr(PivaSuperUser), _
                            '           CStr(xCentroAziendale.GetAttribute("piva")), _
                            '           CInt(xCentroAziendale.GetAttribute("sa_cod")), _
                            '           CStr(Username_Operazione), _
                            '           CDate(xCentroAziendale.GetAttribute("validita_inizio")), _
                            '           objConnessione, _
                            '           objTransazione, _
                            '           StringaConnessione, _
                            '           DirectoryLOG, _
                            '           FileLOG, _
                            '           IdentificatoreUtente)


                            'objUtentixStrutture.AggiornaValiditaFine( _
                            '            CStr(PivaSuperUser), _
                            '            CStr(xCentroAziendale.GetAttribute("piva")), _
                            '            CInt(xCentroAziendale.GetAttribute("sa_cod")), _
                            '            CStr(Username_Operazione), _
                            '            CDate(xCentroAziendale.GetAttribute("validita_fine")), _
                            '            objConnessione, _
                            '            objTransazione, _
                            '            StringaConnessione, _
                            '            DirectoryLOG, _
                            '            FileLOG, _
                            '            IdentificatoreUtente)

#End Region

                            'Preparo il valore da restituire in uscita
                            OUTPUT_Piva = CStr(xCentroAziendale.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xCentroAziendale.GetAttribute("sa_cod"))


                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************
                            objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_CentroAziendale),
                                                           enum_TipoEntita_Des.CentriAziendali,
                                                           CStr(xCentroAziendale.GetAttribute("piva")),
                                                           CStr(xCentroAziendale.GetAttribute("sa_cod")),
                                                           Nothing, Nothing, Nothing, Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri)
                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                    End Select


                    '-------------------------------------------------------------
                    ' MAPPA
                    '-------------------------------------------------------------

                    ' TODO
                    'xMappa = xCentroAziendale.GetElementsByTagName("DatiMappa")
                    'If xMappa.length = 1 Then
                    '    objMappa = CreateObject("Agro_Grafica.Mappa_Write")
                    '    objMappa.Mappa_Scrivi(SuperAdmin, xMappa.Item(0).xml, CStr(xCentroAziendale.GetAttribute("piva")), cint(Sa_Cod), objCnManager, , ConnessioneAlternativa)
                    'End If

                    '-------------------------------------------------------------
                    ' TIPO
                    '-------------------------------------------------------------
                    'Creo l'oggetto COM
                    objCentrixCodici = New AgronicaCoreAnagrafeDAL.Centri_Codici_Write

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_CentroAziendale

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------

                            'Salvo il tipo
                            Dummy = objCentrixCodici.Scrivi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                            Sa_Cod,
                                                            CInt(xCentroAziendale.GetAttribute("tipo")),
                                                            CInt(xCentroAziendale.GetAttribute("tipo")),
                                                            CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                                                            CDate(xCentroAziendale.GetAttribute("validita_fine")),
                                                            objParametri)

                        Case "2"    'MODIFICA -------------------------------------------------------

                            'I campi Id_Cod e Val_Cod dentro la tabella "Centri-Aziendali_Codici" devono
                            'essere = , ma Id_Cod è chiave -> cancello e reinserisco il valore
                            objCentrixCodici.CancellaId_Cod101_102_103(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                       CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                       "",
                                                                       objParametri)


                            Dummy = objCentrixCodici.Scrivi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                            CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                            CInt(xCentroAziendale.GetAttribute("tipo")),
                                                            CInt(xCentroAziendale.GetAttribute("tipo")),
                                                            CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                                                            CDate(xCentroAziendale.GetAttribute("validita_fine")),
                                                            objParametri)

                        Case "3"    'ELIMINA -------------------------------------------------------

                            objCentrixCodici.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                      CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                      CInt(xCentroAziendale.GetAttribute("tipo")),
                                                      "",
                                                      objParametri)

                    End Select

                    ''Elimino l'oggetto
                    'objCentrixCodici = Nothing

                    '-------------------------------------------------------------
                    ' INDIRIZZI
                    '-------------------------------------------------------------

                    ''Prelevo l'elenco degli indirizzi
                    'xIndirizzi = xCentroAziendale.GetElementsByTagName("Indirizzo")

                    'Non va bene, perché vengono presi anche il nodo Indirizzo del Fabbricato
                    'Devo prendere invece solo l'indirizzo del centro

                    'è lo stesso problema che si è verificato nell'impianto:
                    'venivano presi anche i codici della distinta!!!! 
                    'AgronicaCoreAnagrafeBIZ.Reg_Impianto_W.Reg_Impianto_Scrivi()(riga 848)

                    'prende il nodo Indirizzo figlio del nodo corrente (CentroAziendale)
                    xIndirizzi = xCentroAziendale.SelectNodes("child::Indirizzo")

                    i_Indirizzo = 0

                    'commentato il ciclo sugli indirizzi: 
                    'sul centro ce n'è per forza uno solo!!!!!

                    '    Do While i_Indirizzo < xIndirizzi.Count

                    'Prelevo l'i-esimo indirizzo
                    xIndirizzo = xIndirizzi.Item(i_Indirizzo)

                    'Prelevo gli attributi dell'indirizzo selezionato
                    OpeDB_Indirizzo = xIndirizzo.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    objCentrixIndirizzi = New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Write
                    objIndirizzi = New AgronicaCoreAnagrafeDAL.Indirizzi_Write

                    Cod_Indirizzo = CInt(xIndirizzo.GetAttribute("cod_indirizzo"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Indirizzo


                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------

                            If Cod_Indirizzo <= 0 Then

                                'Richiedo un nuovo codice indirizzo
                                Cod_Indirizzo = objSequenze.NuovoId_Tabella("Indirizzi",
                                                                            CInt(xIndirizzo.GetAttribute("basecode")),
                                                                            CInt(xIndirizzo.GetAttribute("topcode")),
                                                                            objParametri)

                            Else

                                'Esportazione dell'Indirizzo del Centro Aziendale in Locale

                            End If

                            'Salvo l'indirizzo
                            Dummy = objIndirizzi.Scrivi(Cod_Indirizzo,
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
                                                        Data_creazione:=Agro_XML_GetDate(xIndirizzo, "data_creazione", #2/1/1900#),
                                                        Data_modifica:=Agro_XML_GetDate(xIndirizzo, "data_modifica", #2/1/1900#),
                                                        username_creazione:=Agro_XML_GetString(xIndirizzo, "username_creazione", ""),
                                                        username_modifica:=Agro_XML_GetString(xIndirizzo, "username_modifica", ""),
                                                        Validazione:=Agro_XML_GetInteger(xIndirizzo, "validazione", 0),
                                                        Data_Validazione:=Agro_XML_GetDate(xIndirizzo, "data_validazione", Now),
                                                        UserName_Validazione:=Agro_XML_GetString(xIndirizzo, "username_validazione", ""),
                                                        Codice_Lingua:=Agro_XML_GetString(xIndirizzo, "codice_lingua", ""),
                                                        Codice_Alternativo:=Agro_XML_GetString(xIndirizzo, "codice_alternativo", "")
                                                        )

                            'Salvo la relazione Impresa x Indirizzo
                            Dummy = objCentrixIndirizzi.Scrivi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                               Sa_Cod,
                                                               Cod_Indirizzo,
                                                               CInt(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                                               CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                               CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                               objParametri,
                                                               Data_creazione:=Agro_XML_GetDate(xIndirizzo, "data_creazione", #2/1/1900#),
                                                               Data_modifica:=Agro_XML_GetDate(xIndirizzo, "data_modifica", #2/1/1900#),
                                                               username_creazione:=Agro_XML_GetString(xIndirizzo, "username_creazione", ""),
                                                               username_modifica:=Agro_XML_GetString(xIndirizzo, "username_modifica", "")
                                                               )

                        Case "2"    'MODIFICA -------------------------------------------------------

                            objIndirizzi.Modifica(CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
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
                                                  "",
                                                  objParametri,
                                                  Data_modifica:=Agro_XML_GetDate(xIndirizzo, "data_modifica", #2/1/1900#),
                                                  username_modifica:=Agro_XML_GetString(xIndirizzo, "username_modifica", ""),
                                                  Validazione:=If(Not xIndirizzo.HasAttribute("validazione"), Nothing, CInt(xIndirizzo.GetAttribute("validazione"))),
                                                  Data_Validazione:=If(Not xIndirizzo.HasAttribute("data_validazione"), AGRODATAINIZIO, CDate(xIndirizzo.GetAttribute("data_validazione"))),
                                                  UserName_Validazione:=If(Not xIndirizzo.HasAttribute("username_validazione"), Nothing, CStr(xIndirizzo.GetAttribute("username_validazione"))),
                                                  Codice_Lingua:=If(Not xIndirizzo.HasAttribute("codice_lingua"), Nothing, CStr(xIndirizzo.GetAttribute("codice_lingua"))),
                                                  Codice_Alternativo:=If(Not xIndirizzo.HasAttribute("codice_alternativo"), Nothing, CStr(xIndirizzo.GetAttribute("codice_alternativo")))
                                                  )

                            'Aggiorno le Validità inizio e fine
                            objCentrixIndirizzi.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                       CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                       CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                                       CInt(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                                                       CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                                       "",
                                                                       objParametri)

                            objCentrixIndirizzi.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                     CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                     CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                                     CInt(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                                                     CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                                     "",
                                                                     objParametri)


                        Case "3"    'ELIMINA -------------------------------------------------------

                            objCentrixIndirizzi.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                         CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                         CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                         CInt(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                                         "",
                                                         objParametri)

                            objIndirizzi.Cancella(CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                  "",
                                                  objParametri)
                    End Select


                    'Elimino l'oggetto
                    objCentrixIndirizzi = Nothing
                    objIndirizzi = Nothing

                    'Incremento l'indice
                    i_Indirizzo += 1

                    '    Loop 'indirizzi
                    'commentato il ciclo sugli indirizzi: 
                    'sul centro ce n'è per forza uno solo!!!!!


                    '-------------------------------------------------------------
                    ' RUBRICA
                    '-------------------------------------------------------------

                    'Prelevo l'elenco delle rubriche
                    xRubriche = xCentroAziendale.GetElementsByTagName("Rubrica")

                    i_Rubrica = 0

                    Do While i_Rubrica < xRubriche.Count

                        'Prelevo l'i-esima rubrica
                        xRubrica = xRubriche.Item(i_Rubrica)

                        'Prelevo gli attributi del codice selezionato
                        OpeDB_Rubrica = xRubrica.GetAttribute("TipoOperazioneDB")

                        'Creo l'oggetto COM
                        objRubrica = New AgronicaCoreAnagrafeDAL.Rubrica_Write
                        objCentrixRubrica = New AgronicaCoreAnagrafeDAL.CentrixRubrica_Write

                        Cod_Rubrica = CInt(xRubrica.GetAttribute("cod_rubrica"))

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Rubrica

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                If Cod_Rubrica <= 0 Then

                                    'Richiedo un nuovo codice rubrica
                                    Cod_Rubrica = objSequenze.NuovoId_Tabella("Rubrica",
                                                                              CInt(xRubrica.GetAttribute("basecode")),
                                                                              CInt(xRubrica.GetAttribute("topcode")),
                                                                              objParametri)

                                Else

                                    'Esportazione della Rubrica in Locale

                                End If

                                'Salvataggio
                                Dummy = objRubrica.Scrivi(Cod_Rubrica,
                                                          CStr(xRubrica.GetAttribute("numero")),
                                                          CStr(xRubrica.GetAttribute("descr")),
                                                          CDate(xRubrica.GetAttribute("validita_inizio")),
                                                          CDate(xRubrica.GetAttribute("validita_fine")),
                                                          objParametri)


                                Dummy = objCentrixRubrica.Scrivi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                 Sa_Cod,
                                                                 Cod_Rubrica,
                                                                 CDate(xRubrica.GetAttribute("validita_inizio")),
                                                                 CDate(xRubrica.GetAttribute("validita_fine")),
                                                                 objParametri)


                            Case "2"    'MODIFICA -------------------------------------------------------

                                ' se il valore è 0 non richiamo il metodo
                                'altrimenti mi genera una eccezione
                                If IsNumeric(xRubrica.GetAttribute("cod_rubrica")) AndAlso
                                    (CInt(xRubrica.GetAttribute("cod_rubrica")) <> 0) Then

                                    objRubrica.Modifica(CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                        CStr(xRubrica.GetAttribute("numero")),
                                                        CStr(xRubrica.GetAttribute("descr")),
                                                        CDate(xRubrica.GetAttribute("validita_inizio")),
                                                        CDate(xRubrica.GetAttribute("validita_fine")),
                                                        "",
                                                        objParametri)

                                End If


                            Case "3"    'ELIMINA -------------------------------------------------------

                                objCentrixRubrica.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                           CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                           CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                           "",
                                                           objParametri)

                                objRubrica.Cancella(CInt(xRubrica.GetAttribute("cod_rubrica")),
                                                    "",
                                                    objParametri)


                        End Select

                        'Elimino gli oggetti
                        objRubrica = Nothing
                        objCentrixRubrica = Nothing

                        'Incremento l'indice
                        i_Rubrica += 1

                    Loop


                    '-------------------------------------------------------------
                    ' SFONDI
                    '-------------------------------------------------------------
                    For Each xSfondo As XmlElement In xCentroAziendale.GetElementsByTagName("CentriXSfondi")
                        Dim opeDb_sfondo = xSfondo.GetAttribute("TipoOperazioneDB")

                        Select Case opeDb_sfondo

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1" 'inserimento

                                Dim Sfondo_data_creazione As Date = #2/1/1900#
                                Dim Sfondo_data_Modifica As Date = #2/1/1900#
                                Dim Sfondo_username_creazione As String = ""
                                Dim Sfondo_username_modifica As String = ""

                                If Not IsNothing(xSfondo.GetAttribute("data_creazione")) AndAlso
                                 xSfondo.GetAttribute("data_creazione") <> "" Then
                                    Sfondo_data_creazione = CDate(xSfondo.GetAttribute("data_creazione"))
                                End If

                                If Not IsNothing(xSfondo.GetAttribute("data_modifica")) AndAlso
                                 xSfondo.GetAttribute("data_modifica") <> "" Then
                                    Sfondo_data_Modifica = CDate(xSfondo.GetAttribute("data_modifica"))
                                End If

                                If Not IsNothing(xSfondo.GetAttribute("username_creazione")) Then
                                    Sfondo_username_creazione = CStr(xSfondo.GetAttribute("username_creazione"))
                                End If

                                If Not IsNothing(xSfondo.GetAttribute("username_modifica")) Then
                                    Sfondo_username_modifica = CStr(xSfondo.GetAttribute("username_modifica"))
                                End If


                                Dim objScrivi As New AgronicaCoreGraficaDAL.CentriXSfondi_Write
                                objScrivi.Scrivi(
                                    xSfondo.GetAttribute("piva"),
                                    CInt(xSfondo.GetAttribute("sa_cod")),
                                    CInt(xSfondo.GetAttribute("sfondocod")),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("ixno"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("iyno"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("ixse"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("iyse"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("oxno"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("oyno"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("oxse"))),
                                    CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("oyse"))),
                                        CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("LatMin"))),
                                        CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("LngMin"))),
                                        CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("LatMax"))),
                                        CDbl(Agro_vb_SaveNum(xSfondo.GetAttribute("LngMax"))),
                                    xSfondo.GetAttribute("filebitmap"),
                                    xSfondo.GetAttribute("pathbitmap"),
                                    xSfondo.GetAttribute("validita_inizio"),
                                    xSfondo.GetAttribute("validita_fine"),
                                    objParametri,
                                    Sfondo_data_creazione,
                                    Sfondo_data_Modifica,
                                    Sfondo_username_creazione,
                                    Sfondo_username_modifica
                            )


                            Case "2" 'modifica

                            Case "3" 'cancellazione

                        End Select

                    Next


                    '-------------------------------------------------------------
                    ' CODICI
                    '-------------------------------------------------------------

                    ' se G2G cancello eventuali codici presenti
                    If TipoG2G <> 0 AndAlso OpeDB_CentroAziendale = 2 Then
                        objCentrixCodici.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                  CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                  0, "Id_Cod NOT IN (101,102,103)", objParametri)
                    End If

                    'Prelevo l'elenco dei codici
                    xCodici = xCentroAziendale.GetElementsByTagName("Codice")

                    i_Codice = 0

                    Do While i_Codice < xCodici.Count
                        'Prelevo l'i-esimo codice
                        xCodice = xCodici.Item(i_Codice)

                        'lavez - 15/04/2021 - il tag "Codice" è utilizzato per gli attributi aggiuntivi di qualsiasi entità anagrafica
                        '                     onde evitare che vengano ribaltati erroneamente attributi non appartenenti alla entità in gestione
                        '                     bisogna verificare che il nodo padre dell'elemento corrente sia del tipo voluto (check su nome tag)

                        If xCodice.ParentNode.Name = "CentroAziendale" Then

                            'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                            OpeDB_Codice = If(TipoG2G = 0, xCodice.GetAttribute("TipoOperazioneDB"), "1")

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Codice

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Se entro in modifica e inserisco un nuovo codice ->
                                    'il sa_cod si trova nella stringa Xml
                                    If OpeDB_CentroAziendale = 2 Then
                                        Sa_Cod = CInt(xCentroAziendale.GetAttribute("sa_cod"))
                                    End If

                                    If CInt(xCodice.GetAttribute("id_cod")) <> 101 AndAlso
                                       CInt(xCodice.GetAttribute("id_cod")) <> 102 AndAlso
                                       CInt(xCodice.GetAttribute("id_cod")) <> 103 Then

                                        Dummy = objCentrixCodici.Scrivi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                        Sa_Cod,
                                                                        CInt(xCodice.GetAttribute("id_cod")),
                                                                        CStr(xCodice.GetAttribute("val_cod")),
                                                                        CDate(xCodice.GetAttribute("validita_inizio")),
                                                                        CDate(xCodice.GetAttribute("validita_fine")),
                                                                        objParametri,
                                                                        Data_creazione:=Agro_XML_GetDate(xCodice, "data_creazione", #2/1/1900#),
                                                                        Data_modifica:=Agro_XML_GetDate(xCodice, "data_modifica", #2/1/1900#),
                                                                        username_creazione:=Agro_XML_GetString(xCodice, "username_creazione", ""),
                                                                        username_modifica:=Agro_XML_GetString(xCodice, "username_modifica", "")
                                                                        )

                                    End If

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objCentrixCodici.Modifica(CStr(xCentroAziendale.GetAttribute("piva")),
                                                              CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                              CInt(xCodice.GetAttribute("id_cod")),
                                                              CStr(xCodice.GetAttribute("val_cod")),
                                                              CDate(xCodice.GetAttribute("validita_inizio")),
                                                              CDate(xCodice.GetAttribute("validita_fine")),
                                                              "",
                                                              objParametri,
                                                              Data_modifica:=Agro_XML_GetDate(xCodice, "data_modifica", #2/1/1900#),
                                                              username_modifica:=Agro_XML_GetString(xCodice, "username_modifica", "")
                                                              )

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objCentrixCodici.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                              CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                              CInt(xCodice.GetAttribute("id_cod")),
                                                              "",
                                                              objParametri)

                            End Select
                        End If
                        ''Elimino l'oggetto
                        'objCentrixCodici = Nothing

                        'Incremento l'indice
                        i_Codice += 1

                    Loop

#Region "Commentato"


                    '-------------------------------------------------------------
                    ' CARATTERISTICHE CANTINA
                    '-------------------------------------------------------------
                    ' TODO
                    '''''Prelevo l'elenco delle eventuali caratteristiche della cantina
                    ''''xCantinaCaratteristiche = xCentroAziendale.GetElementsByTagName("Cantina_Caratteristica")

                    ''''i_CantinaCaratteristica = 0

                    ''''Do While i_CantinaCaratteristica < xCantinaCaratteristiche.length

                    ''''    'Prelevo l'i-esima caratteristisca (ne esista una sola)
                    ''''    xCantinaCaratteristica = xCantinaCaratteristiche.Item(i_CantinaCaratteristica)

                    ''''    'Prelevo gli attributi del codice selezionato
                    ''''    OpeDB_CantinaCaratteristica = xCantinaCaratteristica.GetAttribute("TipoOperazioneDB")

                    ''''    Piano_Cod = cint(xCantinaCaratteristica.GetAttribute("piano_cod"))


                    ''''    'Verifico l'operazione richiesta
                    ''''    Select Case OpeDB_CantinaCaratteristica
                    ''''        '
                    ''''    Case "0"    'LEGGI -------------------------------------------------------
                    ''''            '
                    ''''        Case "1"    'SALVA -------------------------------------------------------


                    ''''            If Piano_Cod <= 0 Then
                    ''''                '
                    ''''                'Richiedo un nuovo codice piano
                    ''''                objSequenze = CreateObject("Agro_Anagrafe_AD.Agro_Sequenze")

                    ''''                Piano_Cod = objSequenze.NuovoId_Tabella( _
                    ''''                               "Cantina_Caratteristica", _
                    ''''                               CStr(SuperAdmin), _
                    ''''                               objCnManager, _
                    ''''                               cint(xCantinaCaratteristica.GetAttribute("basecode")), _
                    ''''                               cint(xCantinaCaratteristica.GetAttribute("topcode")), _
                    ''''                               ConnessioneAlternativa)

                    ''''                objSequenze = Nothing

                    ''''            Else

                    ''''                'Esportazione della Rubrica in Locale

                    ''''            End If
                    ''''            '

                    ''''            Dummy = ObjCantina_Caratteristiche.Scrivi( _
                    ''''                        CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                        Sa_Cod, _
                    ''''                        Piano_Cod, _
                    ''''                        CStr(xCantinaCaratteristica.GetAttribute("piano_des")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("dimx")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("dimy")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("colore_interno")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("colore_esterno")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("spessore")), _
                    ''''                        CInt(xCantinaCaratteristica.GetAttribute("riempimento")), _
                    ''''                        CDbl(xCantinaCaratteristica.GetAttribute("zoom")), _
                    ''''                        CStr(Username_Operazione), _
                    ''''                        CDate(xCantinaCaratteristica.GetAttribute("validita_inizio")), _
                    ''''                        CDate(xCantinaCaratteristica.GetAttribute("validita_fine")), _
                    ''''                        objCnManager, _
                    ''''                        ConnessioneAlternativa)
                    ''''            '
                    ''''        Case "2"    'MODIFICA -------------------------------------------------------
                    ''''            '
                    ''''            ObjCantina_Caratteristiche.Modifica( _
                    ''''                        CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                        Sa_Cod, _
                    ''''                        Piano_Cod, _
                    ''''                        CStr(xCantinaCaratteristica.GetAttribute("piano_des")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("dimx")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("dimy")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("colore_interno")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("colore_esterno")), _
                    ''''                        cint(xCantinaCaratteristica.GetAttribute("spessore")), _
                    ''''                        CInt(xCantinaCaratteristica.GetAttribute("riempimento")), _
                    ''''                        CDbl(xCantinaCaratteristica.GetAttribute("zoom")), _
                    ''''                        CStr(Username_Operazione), _
                    ''''                        CDate(xCantinaCaratteristica.GetAttribute("validita_inizio")), _
                    ''''                        CDate(xCantinaCaratteristica.GetAttribute("validita_fine")), _
                    ''''                        objCnManager, _
                    ''''                        ConnessioneAlternativa)

                    ''''            '
                    ''''        Case "3"    'ELIMINA -------------------------------------------------------

                    ''''            ObjCantina_Caratteristiche.Cancella( _
                    ''''                        CStr(Username_Operazione), _
                    ''''                        CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                        Sa_Cod, _
                    ''''                        Piano_Cod, _
                    ''''                        , _
                    ''''                        objCnManager, _
                    ''''                        ConnessioneAlternativa)

                    ''''            'Cancellazione Pareti della cantina
                    ''''            ObjCantina_Pareti.Cancella( _
                    ''''                        CStr(Username_Operazione), _
                    ''''                        CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                        Sa_Cod, _
                    ''''                        Piano_Cod, _
                    ''''                        0, _
                    ''''                        , _
                    ''''                        objCnManager, _
                    ''''                        ConnessioneAlternativa)

                    ''''            '
                    ''''    End Select



                    ''''    '-------------------------------------------------------------
                    ''''    ' PARETI CANTINA
                    ''''    '-------------------------------------------------------------

                    ''''    'Prelevo l'elenco delle eventuali pareti della cantina
                    ''''    xCantinaPareti = xCantinaCaratteristica.GetElementsByTagName("Cantina_Parete")

                    ''''    i_CantinaParete = 0

                    ''''    Do While i_CantinaParete < xCantinaPareti.length

                    ''''        'Prelevo l'i-esima parete
                    ''''        xCantinaParete = xCantinaPareti.Item(i_CantinaParete)

                    ''''        'Prelevo gli attributi del codice selezionato
                    ''''        OpeDB_CantinaParete = xCantinaParete.GetAttribute("TipoOperazioneDB")

                    ''''        Parete_Cod = cint(xCantinaParete.GetAttribute("parete_cod"))

                    ''''        'Verifico l'operazione richiesta
                    ''''        Select Case OpeDB_CantinaParete
                    ''''            '
                    ''''        Case "0"    'LEGGI -------------------------------------------------------
                    ''''                '
                    ''''            Case "1"    'SALVA -------------------------------------------------------

                    ''''                If Parete_Cod <= 0 Then

                    ''''                    '
                    ''''                    'Richiedo un nuovo codice parete
                    ''''                    objSequenze = CreateObject("Agro_Anagrafe_AD.Agro_Sequenze")

                    ''''                    Parete_Cod = objSequenze.NuovoId_Tabella( _
                    ''''                             "Cantina_Parete", _
                    ''''                             CStr(SuperAdmin), _
                    ''''                             objCnManager, _
                    ''''                             cint(xCantinaParete.GetAttribute("basecode")), _
                    ''''                             cint(xCantinaParete.GetAttribute("topcode")), _
                    ''''                             ConnessioneAlternativa)

                    ''''                    objSequenze = Nothing

                    ''''                Else

                    ''''                    'Esportazione della Rubrica in Locale

                    ''''                End If
                    ''''                '

                    ''''                Dummy = ObjCantina_Pareti.Scrivi( _
                    ''''                            CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                            Sa_Cod, _
                    ''''                            Piano_Cod, _
                    ''''                            Parete_Cod, _
                    ''''                            cint(xCantinaParete.GetAttribute("dimx")), _
                    ''''                            cint(xCantinaParete.GetAttribute("dimy")), _
                    ''''                            cint(xCantinaParete.GetAttribute("colore_interno")), _
                    ''''                            cint(xCantinaParete.GetAttribute("colore_esterno")), _
                    ''''                            cint(xCantinaParete.GetAttribute("posx")), _
                    ''''                            cint(xCantinaParete.GetAttribute("posy")), _
                    ''''                            cint(xCantinaParete.GetAttribute("spessore")), _
                    ''''                            CInt(xCantinaParete.GetAttribute("riempimento")), _
                    ''''                            CStr(Username_Operazione), _
                    ''''                            CDate(xCantinaParete.GetAttribute("validita_inizio")), _
                    ''''                            CDate(xCantinaParete.GetAttribute("validita_fine")), _
                    ''''                            objCnManager, _
                    ''''                            ConnessioneAlternativa)
                    ''''                '
                    ''''            Case "2"    'MODIFICA -------------------------------------------------------
                    ''''                '
                    ''''                ObjCantina_Pareti.Modifica( _
                    ''''                            CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                            Sa_Cod, _
                    ''''                            Piano_Cod, _
                    ''''                            Parete_Cod, _
                    ''''                            cint(xCantinaParete.GetAttribute("dimx")), _
                    ''''                            cint(xCantinaParete.GetAttribute("dimy")), _
                    ''''                            cint(xCantinaParete.GetAttribute("colore_interno")), _
                    ''''                            cint(xCantinaParete.GetAttribute("colore_esterno")), _
                    ''''                            cint(xCantinaParete.GetAttribute("posx")), _
                    ''''                            cint(xCantinaParete.GetAttribute("posy")), _
                    ''''                            cint(xCantinaParete.GetAttribute("spessore")), _
                    ''''                            CInt(xCantinaParete.GetAttribute("riempimento")), _
                    ''''                            CStr(Username_Operazione), _
                    ''''                            CDate(xCantinaParete.GetAttribute("validita_inizio")), _
                    ''''                            CDate(xCantinaParete.GetAttribute("validita_fine")), _
                    ''''                            objCnManager, _
                    ''''                            ConnessioneAlternativa)

                    ''''                '
                    ''''            Case "3"    'ELIMINA -------------------------------------------------------

                    ''''                ObjCantina_Pareti.Cancella( _
                    ''''                            CStr(Username_Operazione), _
                    ''''                            CStr(xCentroAziendale.GetAttribute("piva")), _
                    ''''                            Sa_Cod, _
                    ''''                            Piano_Cod, _
                    ''''                            Parete_Cod, _
                    ''''                            , _
                    ''''                            objCnManager, _
                    ''''                            ConnessioneAlternativa)
                    ''''                '
                    ''''        End Select


                    ''''        'Incremento l'indice
                    ''''        i_CantinaParete = i_CantinaParete + 1

                    ''''    Loop







                    ''''    'Incremento l'indice
                    ''''    i_CantinaCaratteristica = i_CantinaCaratteristica + 1

                    ''''Loop

#End Region


                    Select Case OpeDB_CentroAziendale

                        Case "2" '-

                            'Nel caso in cui si modificano solo le validità della struttura
                            'L'aggiornamento delle validità delle tabelle correlate deve essere
                            'Comunque eseguita

                            objCentrixRubrica = New AgronicaCoreAnagrafeDAL.CentrixRubrica_Write
                            objCentrixRubrica.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                     CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                     0,
                                                                     CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                                                                     "",
                                                                     objParametri)
                            objCentrixRubrica.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                   CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                   0,
                                                                   CDate(xCentroAziendale.GetAttribute("validita_fine")),
                                                                   "",
                                                                   objParametri)
                            objCentrixRubrica = Nothing

#Region "Commentato"

                            '                     Set objCentrixCodici = CreateObject("Agro_Anagrafe_ad.Centri_Codici_Write")
                            '                     objCentrixCodici.AggiornaValiditaInizio CStr(xCentroAziendale.getAttribute("piva")), cint(xCentroAziendale.getAttribute("sa_cod")), 0, cstr(Username_Operazione), CDate(xCentroAziendale.getAttribute("validita_inizio")), objCnManager, ConnessioneAlternativa
                            '                     objCentrixCodici.AggiornaValiditaFine CStr(xCentroAziendale.getAttribute("piva")), cint(xCentroAziendale.getAttribute("sa_cod")), 0, cstr(Username_Operazione), CDate(xCentroAziendale.getAttribute("validita_fine")), objCnManager, ConnessioneAlternativa
                            '                     Set objCentrixCodici = Nothing

                            '-------------------------------------------------
                            'Modifica delle finestre temporali dei figli

                            ' Giulia: 11/7/2019: Commentato perché potrebbe generarmi cmq dei casini 
                            ' perché ci potrebbero essere delle operazioni oltre quella data, e cmq non sono considerate le distinte

                            'objFabbricati_AD = New AgronicaCoreAnagrafeDAL.Fabbricati_W
                            'objFabbricati_AD.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                        CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                        0,
                            '                                        CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                        "",
                            '                                        objParametri)
                            'objFabbricati_AD.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                      CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                      0,
                            '                                      CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                      "",
                            '                                      objParametri)
                            'objFabbricati_AD = Nothing

                            'ObjCampi = New AgronicaCoreAnagrafeDAL.Campi_W
                            'ObjCampi.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                0,
                            '                                CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                "",
                            '                                objParametri)
                            'ObjCampi.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                              CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                              0,
                            '                              CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                              "",
                            '                              objParametri)
                            'ObjCampi = Nothing

                            'objUtentixCampi = New AgronicaCoreAnagrafeDAL.UtentixCampi_Write
                            'objUtentixCampi.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                       CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                       0,
                            '                                       CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                       "",
                            '                                       objParametri)
                            'objUtentixCampi.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                     CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                     0,
                            '                                     CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                     "",
                            '                                     objParametri)
                            'objUtentixCampi = Nothing

                            'objCampixParticelle = New AgronicaCoreAnagrafeDAL.CampixParticelle_W
                            'objCampixParticelle.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                           CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                           0,
                            '                                           "",
                            '                                           "",
                            '                                           "",
                            '                                           0,
                            '                                           0,
                            '                                           "",
                            '                                           CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                           "",
                            '                                           objParametri)
                            'objCampixParticelle.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                         CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                         0,
                            '                                         "",
                            '                                         "",
                            '                                         "",
                            '                                         0,
                            '                                         0,
                            '                                         "",
                            '                                         CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                         "",
                            '                                         objParametri)
                            'objCampixParticelle = Nothing

                            ''Set objCampi_Dettagli = CreateObject("Agro_Anagrafe_ad.Campi_Dettagli_Write")
                            ''objCampi_Dettagli.AggiornaValiditaInizio CStr(xCentroAziendale.getAttribute("piva")), cint(xCentroAziendale.getAttribute("sa_cod")), 0, 0, cstr(Username_Operazione), CDate(xCentroAziendale.getAttribute("validita_inizio")), objCnManager,ConnessioneAlternativa
                            ''objCampi_Dettagli.AggiornaValiditaFine CStr(xCentroAziendale.getAttribute("piva")), cint(xCentroAziendale.getAttribute("sa_cod")), 0, 0, cstr(Username_Operazione), CDate(xCentroAziendale.getAttribute("validita_fine")), objCnManager,ConnessioneAlternativa
                            ''Set objCampi_Dettagli = Nothing

                            'objAppezzamenti = New AgronicaCoreAnagrafeDAL.Appezzamento_Write
                            'objAppezzamenti.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                       CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                       0,
                            '                                       0,
                            '                                       CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                       "",
                            '                                       objParametri)
                            'objAppezzamenti.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                     CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                     0,
                            '                                     0,
                            '                                     CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                     "",
                            '                                     objParametri)
                            'objAppezzamenti = Nothing

                            'objUtentixAppezzamenti = New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
                            'objUtentixAppezzamenti.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                              CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                              0,
                            '                                              0,
                            '                                              CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                              "",
                            '                                              objParametri)
                            'objUtentixAppezzamenti.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                            CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                            0,
                            '                                            0,
                            '                                            CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                            "",
                            '                                            objParametri)
                            'objUtentixAppezzamenti = Nothing

                            'objAppezzaxParticelle = New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
                            'objAppezzaxParticelle.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                             CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                             0,
                            '                                             0,
                            '                                             "",
                            '                                             "",
                            '                                             "",
                            '                                             0,
                            '                                             0,
                            '                                             "",
                            '                                             CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                                             "",
                            '                                             objParametri)
                            'objAppezzaxParticelle.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                           CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                           0,
                            '                                           0,
                            '                                           "",
                            '                                           "",
                            '                                           "",
                            '                                           0,
                            '                                           0,
                            '                                           "",
                            '                                           CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                           "",
                            '                                           objParametri)
                            'objAppezzaxParticelle = Nothing

                            'objReg_Impianti = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
                            'objReg_Impianti.AggiornaValiditaInizio(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                    CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                    0,
                            '                    0,
                            '                    0,
                            '                        CDate(xCentroAziendale.GetAttribute("validita_inizio")),
                            '                            objParametri)
                            'objReg_Impianti.AggiornaValiditaFine(CStr(xCentroAziendale.GetAttribute("piva")),
                            '                                     CInt(xCentroAziendale.GetAttribute("sa_cod")),
                            '                                     0,
                            '                                     0,
                            '                                     0,
                            '                                     CDate(xCentroAziendale.GetAttribute("validita_fine")),
                            '                                     objParametri)
                            'objReg_Impianti = Nothing

#End Region


                        Case "3" 'CANCELLAZIONE CENTRO AZIENDALE

                            'Cancello i figli Maggiori del Centro Aziendale

                            'Cancello tutti gli appezzamenti!!
                            objAppezzamentiLeggi = New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                            XmlAppezzamenti = objAppezzamentiLeggi.Appezzamento_Leggi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                                      CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                                      0, 0,
                                                                                      True, True, False, False,
                                                                                      objParametri)
                            objAppezzamentiLeggi = Nothing
                            If XmlAppezzamenti <> "" Then
                                objAppezzamentiScrivi = New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                                objAppezzamentiScrivi.Appezzamento_Scrivi(XmlAppezzamenti,
                                                                          Nothing,
                                                                          Nothing,
                                                                          Nothing,
                                                                          objParametri,
                                                                          objParametri_Utenti)

                                objAppezzamentiScrivi = Nothing
                            End If

                            'Cancello TUTTI i Campi... ma solo i campi (non cancello gli appezzamenti e gli impianti)
                            objCampiLeggi = New AgronicaCoreAnagrafeBIZ.Campo_R
                            XmlCampi = objCampiLeggi.Campo_Leggi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                 CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                 0,
                                                                 AGRODATAINIZIO,
                                                                 AGRODATAFINE,
                                                                 True, True,
                                                                 objParametri)
                            objCampiLeggi = Nothing

                            If XmlCampi <> "" Then
                                objCampiScrivi = New AgronicaCoreAnagrafeBIZ.Campo_W
                                objCampiScrivi.Campo_Scrivi(XmlCampi,
                                                            Nothing,
                                                            Nothing,
                                                            Nothing,
                                                            True,
                                                            objParametri,
                                                            objParametri_Utenti)

                                objCampiScrivi = Nothing
                            End If

                            'Cancellazione delle particelle catastali associate al centro aziendale
                            ObjLeggiParticelle = New AgronicaCoreAnagrafeBIZ.Particella_R
                            XmlParticelle = ObjLeggiParticelle.ImpresexParticelle_Leggi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                                        CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                                        0, "", "", "", 0, 0, "", True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                         objParametri)

                            ObjLeggiParticelle = Nothing

                            If XmlParticelle <> "" Then
                                ObjCancellaParticelle = New AgronicaCoreAnagrafeBIZ.Particella_W
                                ObjCancellaParticelle.Particella_Scrivi(XmlParticelle, objParametri)
                                ObjCancellaParticelle = Nothing
                            End If
                            '--------------------------------------------------------------------------------------------------

                            'Cancellazione di TUTTE le OPERAZIONI DI AGENDA

                            objAgendaLeggi = New AgronicaCoreContabBIZ.Agenda_R
                            XmlAgenda = objAgendaLeggi.Agenda_Leggi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                    CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                    0, 0, True,
                                                                    objParametri)

                            objAgendaLeggi = Nothing

                            If XmlAgenda <> "" Then
                                objAgendaScrivi = New AgronicaCoreContabBIZ.Agenda_W
                                objAgendaScrivi.Agenda_Scrivi(XmlAgenda,
                                                              Nothing,
                                                              0, 5,
                                                              0, "",
                                                              objParametri)
                                objAgendaScrivi = Nothing
                            End If
                            '--------------------------------------------------------------------------------------------------

                            'Cancellazione di TUTTI i FABBRICATI

                            objFabbricatiLeggi = New AgronicaCoreAnagrafeBIZ.Fabbricato_R
                            XmlFabbricati = objFabbricatiLeggi.Fabbricato_Leggi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                                CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                                0,
                                                                                True,
                                                                                objParametri)
                            objFabbricatiLeggi = Nothing

                            If XmlFabbricati <> "" Then
                                objFabbricatiScrivi = New AgronicaCoreAnagrafeBIZ.Fabbricato_W
                                objFabbricatiScrivi.Fabbricato_Scrivi(XmlFabbricati,
                                                                        Nothing,
                                                                        Nothing,
                                                                        Nothing,
                                                                        objParametri, NoteLog:="Registrato da Eliminazione Centro Aziendale (bootstrap)")
                                objFabbricatiScrivi = Nothing
                            End If

                            '--------------------------------------------------------------------------------------------------
                            'Cancellazione Sequenze del Centro Aziendale

                            xRisp = objSequenze.CancellaSeqMagazzino(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                     CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                     objParametri)

                            xRisp = objSequenze.CancellaSeqCampi(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                 CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                 objParametri)

                            xRisp = objSequenze.CancellaSeqAppezzamento(CStr(xCentroAziendale.GetAttribute("piva")),
                                                                        CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                                        objParametri)

                            '--------------------------------------------------------------------------------------------------

                            'ma ha senso questa cancellazione???????
                            ''Cancellazione delle MATERIE PRIME AZIENDALI

                            'ObjMaterie_Prime = New AgronicaCoreAnagrafeDAL.Materie_Prime_W
                            'ObjMaterie_Prime.Cancella(0, 0, CStr(Username_Operazione), CStr(xCentroAziendale.GetAttribute("piva")), objCnManager, ConnessioneAlternativa)
                            'ObjMaterie_Prime = Nothing
                            ''--------------------------------------------------------------------------------------------------


                            'Cancellazione della CARTOGRAFIA AZIENDALE

                            objAZIInfo = New AgronicaCoreGraficaDAL.AziInfoRER_Write
                            objAZIInfo.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                "", objParametri)
                            objAZIInfo = Nothing

                            ObjCentriXSfondi = New AgronicaCoreGraficaDAL.CentriXSfondi_Write
                            ObjCentriXSfondi.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                      CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                      0,
                                                      "", objParametri)
                            ObjCentriXSfondi = Nothing

                            objGrafica = New AgronicaCoreGraficaDAL.Grafica_Write
                            objGrafica.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                                CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                "", "", "",
                                                "", objParametri)
                            objGrafica = Nothing


                            '-------------------------------------------------------------------

                            'Cancello le Strutture

                            objUtentixStrutture.Cancella(objParametri.PivaSuperUser,
                                                         CStr(xCentroAziendale.GetAttribute("piva")),
                                                         CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                                         "", objParametri)

                            objCentri.Cancella(CStr(xCentroAziendale.GetAttribute("piva")),
                                               CInt(xCentroAziendale.GetAttribute("sa_cod")),
                                               "", objParametri, objParametri_Utenti)


                            'Preparo il valore da restituire in uscita
                            OUTPUT_Sa_Cod = CInt(xCentroAziendale.GetAttribute("sa_cod"))

                            xPiva = CStr(xCentroAziendale.GetAttribute("piva"))


                    End Select

                    'Elimino l'oggetto
                    objCentri = Nothing
                    objImpresexParticelle = Nothing
                    objUtentixStrutture = Nothing
                    ObjCantina_Caratteristiche = Nothing
                    ObjCantina_Pareti = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_CentroAziendale += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiCentroAziendale += 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati


            xCodice = Nothing
            xCodici = Nothing
            xRubrica = Nothing
            xRubriche = Nothing
            xIndirizzo = Nothing
            xIndirizzi = Nothing
            xCentroAziendale = Nothing
            xCentriAziendali = Nothing
            xDatiCentroAziendale = Nothing
            xDatiCentriAziendali = Nothing
            XmlDoc = Nothing

            objSequenze = Nothing

            xRisp = True


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Sa_Cod=" & CStr(OUTPUT_Sa_Cod) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

    Public Function Scrivi_Centro_Anagrafica(ByRef objCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal creaMagazzino As Boolean = True,
                                             Optional ByVal nomeMagazzino As String = Nothing,
                                             Optional ByVal NoteLog As String = NOTELOG_ANAGRAFE_NG) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.CentroAziendale_W.Scrivi_Centro_Anagrafica()"
        Dim messaggioErrore As String = ""


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)


        Dim Sa_Cod As Integer

        Try
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            Using scope As New TransactionScope(scopeOption, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    If Not objCentro.flag_cancellazione Then

                        Sa_Cod = ScriviModifica_Centro(objCentro, GiasContext, objParametri_Server, objParametri_Utenti, creaMagazzino:=creaMagazzino, nomeMagazzino:=nomeMagazzino, NoteLog:=NoteLog)

                    Else

                        Sa_Cod = EliminaCentro(objCentro, GiasContext, objParametri_Server, objParametri_Utenti, NoteLog:=NoteLog)

                    End If

                    GiasContext.SaveChanges()
                    scope.Complete()
                    scope.Dispose()
                End Using
            End Using
        Catch ex As GiasException
            messaggioErrore = ex.Message
            Throw ex

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return 0
        End Try

        Return Sa_Cod
    End Function

    Private Function ScriviModifica_Centro(objCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                                           ByRef GiasContext As Gias_DeveloperServer_Entities,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                           Optional ByVal creaMagazzino As Boolean = True,
                                           Optional ByVal nomeMagazzino As String = Nothing,
                                           Optional NoteLog As String = NOTELOG_ANAGRAFE_NG)
        Dim Piva As String = ""
        Dim Sa_Cod = 0
        Piva = objCentro.primaryKey.partitaIva
        Dim Tipo_Operazione = "0" ' 0: Non fare niente - 1: Creazione nuovo Centro - 2: Modifica Centro

        If objCentro.primaryKey.codice = 0 Then
            Tipo_Operazione = "1"
        Else
            Tipo_Operazione = "2"
            Sa_Cod = objCentro.primaryKey.codice
        End If

        Dim Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali = Nothing
        Dim UtentixStrutture As AgronicaCoreEntityFramework_POCO.UtentiXStrutture

        If Tipo_Operazione = "2" Then
            Verifica_ValiditaInizioFine(objCentro, GiasContext, objParametri_Server, objParametri_Server, objParametri_Utenti)
        End If

        If Tipo_Operazione = "1" Then
            Centro = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliEF(GiasContext, objParametri_Server, Piva, objCentro.nome, objParametri_Utenti.UsernameOperazione, objParametri_Utenti, NoteLog:=NoteLog)
            Piva = Centro.PIVA
            Sa_Cod = Centro.sa_cod
            objCentro.primaryKey.partitaIva = Centro.PIVA
            objCentro.primaryKey.codice = Centro.sa_cod

            ' ---------- UTENTIxSTRUTTURE --------------- '
            UtentixStrutture = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateUtentixStrutture(GiasContext, objParametri_Server, Centro, objParametri_Utenti.UsernameOperazione)

        Else
            Centro = (
                From cent In GiasContext.Centri_Aziendali Where
                                                              cent.PIVA = Piva AndAlso
                                                              cent.sa_cod = Sa_Cod).FirstOrDefault()
        End If

        If Centro Is Nothing Then
            Throw New GiasException("Centro non trovato")
        End If


        Centro.sa_nome = objCentro.nome

        If Centro.TitoloPossesso Is Nothing Then
            Centro.TitoloPossesso = 0
        End If

        If objCentro.titolo_Di_Possesso IsNot Nothing Then
            Centro.TitoloPossesso = objCentro.titolo_Di_Possesso.codice
        End If

        Centro.lat = objCentro.lat
        Centro.long = objCentro.lng

        If objCentro.validita Is Nothing Then
            objCentro.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale() With {.inizio = AGRODATAINIZIO, .fine = AGRODATAFINE}
        Else
            If objCentro.validita.inizio < AGRODATAINIZIO OrElse objCentro.validita.inizio > AGRODATAFINE Then
                objCentro.validita.inizio = AGRODATAINIZIO
            End If
            If objCentro.validita.fine < AGRODATAINIZIO OrElse objCentro.validita.fine > AGRODATAFINE Then
                objCentro.validita.fine = AGRODATAFINE
            End If
        End If

        Centro.Validita_Inizio = objCentro.validita.inizio
        Centro.Validita_Fine = objCentro.validita.fine

        'Colonne non usate e settate a ZERO/STRINGA VUOTA
        Centro.area = 0

        'QUESTO DATO NON VA BRASATO, alcuni clienti lo settano dal LAN!
        'Centro.AT_Prevalente = ""

        Centro.ca_sipi = ""
        Centro.Forma_Possesso = ""
        Centro.Sup_Bosco = 0
        Centro.Sup_Prati = 0
        Centro.Sup_SAU = 0
        Centro.Sup_SAU_Biologico = 0
        Centro.Sup_SAU_Convenzionale = 0
        Centro.Sup_SAU_Conversione = 0
        Centro.Sup_Tare = 0
        Centro.Sup_Totale = 0
        Centro.ZSLM = 0
        Centro.Username_Modifica = objParametri_Server.UsernameOperazione
        Centro.Data_Modifica = DateTime.Now


        ' ---------- INDIRIZZO -------------- '
        Dim indirizzoBIZ As New Indirizzi_W
        If (objCentro.indirizzi IsNot Nothing) Then
            For Each add As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato In objCentro.indirizzi
                If (add.indirizzo.istatComune.com IsNot Nothing) AndAlso (add.indirizzo.istatComune.prov IsNot Nothing) Then

                    indirizzoBIZ.Scrivi_Indirizzi_Associati_Centro(
                        add,
                        GiasContext,
                        Centro,
                        objParametri_Server,
                        objParametri_Utenti
                        )
                End If
            Next
        End If


        ' ---------- CODICI --------------- '
        Dim codiciBIZ As New Codici_W
        If (objCentro.codici IsNot Nothing) Then

            If objCentro.codici IsNot Nothing Then

                For Each codice In objCentro.codici
                    If codice.validita Is Nothing Then
                        codice.validita = New IntervalloTemporale()
                    Else
                        If codice.validita.inizio < AGRODATAINIZIO Then
                            codice.validita.inizio = AGRODATAINIZIO
                        End If

                        If codice.validita.fine < AGRODATAINIZIO Then
                            codice.validita.fine = AGRODATAFINE
                        End If

                    End If
                Next

                Dim listIdCod As New List(Of Integer)
                'listIdCod.Add(enum_CodiciAnagrafe.Centro_Sede_Legale)
                'listIdCod.Add(enum_CodiciAnagrafe.Centro_Sede_Aziendale)
                'listIdCod.Add(enum_CodiciAnagrafe.Centro_Stabilimento)
                listIdCod.Add(enum_CodiciAnagrafe.TitoloPossesso)
                listIdCod.Add(0)
                listIdCod.Add(enum_CodiciAnagrafe.TipoAttivita)
                listIdCod.Add(enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO)
                listIdCod.Add(enum_CodiciAnagrafe.OTE)

                Dim imprese_codici_del = (From cc In GiasContext.Centri_Aziendali_Codici
                                          Where cc.PIVA = Piva AndAlso
                                                cc.sa_cod = Sa_Cod AndAlso
                                                Not listIdCod.Contains(cc.id_cod) AndAlso (cc.id_cod < 2000 OrElse cc.id_cod > 3000)).ToList

                If imprese_codici_del.Count > 0 Then
                    GiasContext.Centri_Aziendali_Codici.RemoveRange(imprese_codici_del)
                    GiasContext.SaveChanges()
                End If
            End If


            For Each codici As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori In objCentro.codici
                If (codici.valore IsNot Nothing) Then
                    codiciBIZ.Scrivi_Codici_Centro_NoEF(
                        codici,
                        GiasContext,
                        Centro,
                        objParametri_Server,
                        objParametri_Utenti
                        )
                End If
            Next

        End If

        If (objCentro.tipologia IsNot Nothing) Then
            Dim enumerativo As New enum_CodiciAnagrafe

            If objCentro.tipologia.codice = 101 Then
                enumerativo = enum_CodiciAnagrafe.Centro_Sede_Legale
            ElseIf objCentro.tipologia.codice = 102 Then
                enumerativo = enum_CodiciAnagrafe.Centro_Sede_Aziendale
            ElseIf objCentro.tipologia.codice = 103 Then
                enumerativo = enum_CodiciAnagrafe.Centro_Stabilimento
            End If

            Scrivi_Codice_Centro(enum_CodiciAnagrafe.Centro_Sede_Legale, "", GiasContext, Centro, objParametri_Server, objParametri_Utenti)
            Scrivi_Codice_Centro(enum_CodiciAnagrafe.Centro_Sede_Aziendale, "", GiasContext, Centro, objParametri_Server, objParametri_Utenti)
            Scrivi_Codice_Centro(enum_CodiciAnagrafe.Centro_Stabilimento, "", GiasContext, Centro, objParametri_Server, objParametri_Utenti)

            Scrivi_Codice_Centro(enumerativo, objCentro.tipologia.codice, GiasContext, Centro, objParametri_Server, objParametri_Utenti)
        End If

        If (objCentro.titolo_Di_Possesso IsNot Nothing) Then
            Scrivi_Codice_Centro(enum_CodiciAnagrafe.TitoloPossesso, objCentro.titolo_Di_Possesso.codice, GiasContext, Centro, objParametri_Server, objParametri_Utenti)
        End If

        If (objCentro.bioTipoAttivita IsNot Nothing) Then
            Scrivi_Codice_Centro(enum_CodiciAnagrafe.TipoAttivita, objCentro.bioTipoAttivita.codice, GiasContext, Centro, objParametri_Server, objParametri_Utenti)
        End If

        If (objCentro.bioOrganismoDiControllo IsNot Nothing) Then
            Scrivi_Codice_Centro(enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO, objCentro.bioOrganismoDiControllo.codice, GiasContext, Centro, objParametri_Server, objParametri_Utenti)
        End If

        If (objCentro.centroAziendaleEsternoCollegato IsNot Nothing) Then
            Scrivi_Codice_Centro(enum_CodiciAnagrafe.Centro_Aziendale_Esterno_Collegato, objCentro.centroAziendaleEsternoCollegato.codice, GiasContext, Centro, objParametri_Server, objParametri_Utenti)
        End If

        'CONTROLLARE QUESTI
        ' Scrivi_Codice_Centro(enum_CodiciAnagrafe.CodiceCentro_Attuale, objCentro.Cod_Operatore, GiasContext, Centro, objParametri_Server, objParametri_Utenti)
        'Scrivi_Codice_Centro(enum_CodiciAnagrafe.TipoAttivita, objCentro.Attivita, GiasContext, Centro, objParametri_Server, objParametri_Utenti)

        Dim strOTE As String = ""
        If objCentro.orientamentoTecnicoEconomico IsNot Nothing AndAlso objCentro.orientamentoTecnicoEconomico.Count > 0 Then
            For Each OTE In objCentro.orientamentoTecnicoEconomico
                strOTE &= OTE.codice & "|"
            Next
        End If
        If strOTE <> "" Then
            strOTE = strOTE.Remove(strOTE.LastIndexOf("|"))
        End If
        Scrivi_Codice_Centro(enum_CodiciAnagrafe.OTE, strOTE, GiasContext, Centro, objParametri_Server, objParametri_Utenti)

        ' ---------- RUBRICA ---------------- ' TO DO.....................
        If objCentro.rubricaVoci IsNot Nothing Then

            Rimuova_Rubrica_Centro(objCentro.rubricaVoci, Centro, GiasContext, objParametri_Server)

            For Each rubrica In objCentro.rubricaVoci
                Scrivi_Rubrica_Centro(rubrica.ToRubrica(), GiasContext, Centro, objParametri_Server, objParametri_Utenti)
            Next

        End If

        If Tipo_Operazione = "1" Then
            Try
                Dim objUtentiVisibilitaR As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilitaR.Leggi(enum_TipoEntita.Centro, "", "", objParametri_Server)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                    objUtentiVisibilita.Scrivi(enum_TipoEntita.Centro, objCentro.primaryKey.partitaIva, objCentro.primaryKey.codice, 0, 0, objParametri_Server)
                End If
            Catch ex As Exception

            End Try

            If creaMagazzino Then
                Crea_Magazzino(objCentro, objParametri_Server, objParametri_Utenti, GiasContext, nomeMagazzino:=nomeMagazzino, NoteLog:="Registrato da creazione Centro Aziendale (NG)")
            End If
        End If



        Dim DatiCentroStr = ""
        If objCentro IsNot Nothing Then
            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiCentroStr = JsonConvert.SerializeObject(objCentro, a)
        End If

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
            enum_TipoEntita_Des.CentriAziendali,
            CStr(Piva), CStr(Sa_Cod),
            Nothing, Nothing,
            Nothing, Nothing,
            CInt(Tipo_Operazione),
            objParametri_Server, enum_Id_Servizio.GiasOnline,
            NoteLog, DatiCentroStr
            )

        GiasContext.Agronica_Log_Anagrafe.Add(log)
        GiasContext.SaveChanges()

        Return Sa_Cod
    End Function

    Private Sub Rimuova_Rubrica_Centro(rubricaVoci As List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci), centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali, context As Gias_DeveloperServer_Entities, objParametri_Server As AgronicaCoreParametri)
        Dim rubricheSalvate As List(Of Integer) = (From c In rubricaVoci Select c.rubrica.codice).ToList()

        Dim rubrichexDaCancellare = (From rub In context.CentrixRubrica Where rub.PIVA = centro.PIVA AndAlso rub.sa_cod = centro.sa_cod AndAlso Not rubricheSalvate.Contains(rub.cod_rubrica)).ToList()
        If rubrichexDaCancellare.Count > 0 Then
            context.CentrixRubrica.RemoveRange(rubrichexDaCancellare)
            context.SaveChanges()
        End If

        Dim rubxIds = rubrichexDaCancellare.Select(Function(s) s.cod_rubrica)
        Dim rubricheDaCancellare = (From rub In context.Rubrica Where rubxIds.Contains(rub.cod_rubrica)).ToList()
        If rubricheDaCancellare.Count > 0 Then
            context.Rubrica.RemoveRange(rubricheDaCancellare)
            context.SaveChanges()
        End If
    End Sub


    Private Function Scrivi_Rubrica_Centro(rubrica As AnagrafeNG.Rubrica,
                                           ByRef GiasContext As Gias_DeveloperServer_Entities,
                                           ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri)
        Dim Cod_Rubrica As Integer
        Dim RubricaEF As AgronicaCoreEntityFramework_POCO.Rubrica

        If rubrica.Rubrica_Cod = 0 Then

            RubricaEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateRubricaCentro(GiasContext, objParametri_Server, Centro, rubrica.Valore, rubrica.Tipologia, objParametri_Utenti.UsernameOperazione)
            Cod_Rubrica = RubricaEF.cod_rubrica

        Else

            Cod_Rubrica = rubrica.Rubrica_Cod
            RubricaEF = (From rub In GiasContext.Rubrica
                         Where rub.cod_rubrica = Cod_Rubrica).FirstOrDefault()
        End If

        If RubricaEF Is Nothing Then
            Throw New GiasException("Rubrica non trovata")
        End If

        RubricaEF.numero = rubrica.Valore
        RubricaEF.descr = rubrica.Tipologia

        Return Cod_Rubrica
    End Function

    'Private Function Scrivi_Indirizzo_Centro(indirizzo As AnagrafeNG.Indirizzi,
    '                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
    '                                         ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
    '                                         ByRef objParametri_Server As AgronicaCoreParametri,
    '                                         ByRef objParametri_Utenti As AgronicaCoreParametri)

    '    Dim Cod_Indirizzo As Integer
    '    Dim indirizzoEF As AgronicaCoreEntityFramework_POCO.Indirizzi

    '    If indirizzo.Cod_Indirizzo = 0 Then

    '        'indirizzoEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateIndirizzoCentro(GiasContext, objParametri_Server, Centro, 1, objParametri_Utenti.UsernameOperazione)
    '        'Cod_Indirizzo = indirizzoEF.cod_indirizzo

    '    Else

    '        Cod_Indirizzo = indirizzo.Cod_Indirizzo
    '        indirizzoEF = (From ind In GiasContext.Indirizzi Where
    '                                                         ind.cod_indirizzo = Cod_Indirizzo).FirstOrDefault()
    '    End If

    '    indirizzoEF.ind_des = indirizzo.Via
    '    indirizzoEF.frz_des = indirizzo.Frazione
    '    indirizzoEF.pro_cod_istat = indirizzo.Prov
    '    indirizzoEF.com_cod_istat = indirizzo.Com
    '    indirizzoEF.CAP = indirizzo.CAP
    '    indirizzoEF.stato = indirizzo.Stato
    '    indirizzoEF.note = indirizzo.Note

    '    Return Cod_Indirizzo

    'End Function

    Private Function Scrivi_Codici_Centro(codici As AnagrafeNG.Codici,
                                           ByRef GiasContext As Gias_DeveloperServer_Entities,
                                           ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Scrivi_Codice_Centro(codici.Id_Cod, codici.Val_Cod, GiasContext, Centro, objParametri_Server, objParametri_Utenti)

        Return codici.Id_Cod
    End Function

    Private Function Scrivi_Codice_Centro(id_cod As Integer,
                                          val_cod As String,
                                           ByRef GiasContext As Gias_DeveloperServer_Entities,
                                           ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim CodiciEF As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici

        Dim piva = Centro.PIVA
        Dim sa_cod = Centro.sa_cod

        CodiciEF = (From cod In GiasContext.Centri_Aziendali_Codici Where
                                                         cod.id_cod = id_cod AndAlso
                                                         piva = cod.PIVA AndAlso
                                                         sa_cod = cod.sa_cod).FirstOrDefault()

        If val_cod <> "" AndAlso val_cod <> "0" Then

            If CodiciEF Is Nothing Then

                CodiciEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliCodici(GiasContext, objParametri_Server, Centro, id_cod,
                                                                                                   val_cod, objParametri_Utenti.UsernameOperazione)


            End If

            CodiciEF.val_cod = val_cod
            CodiciEF.Username_Modifica = objParametri_Server.UsernameOperazione
            CodiciEF.Data_Modifica = DateTime.Now

        Else
            If CodiciEF IsNot Nothing Then

                ' funzione per cancellare il record
                GiasContext.Centri_Aziendali_Codici.Attach(CodiciEF)
                GiasContext.Centri_Aziendali_Codici.Remove(CodiciEF)

            End If
        End If
        GiasContext.SaveChanges()

        Return id_cod
    End Function

    Private Function Verifica_ValiditaInizioFine(ByRef centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                                                 ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                 ByRef objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""

        Dim Validita_Fine = centro.validita.fine
        Dim Validita_Inizio = centro.validita.inizio
        Dim piva = centro.primaryKey.partitaIva
        Dim sa_Cod = centro.primaryKey.codice

        Dim objCentroR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim cen = objCentroR.Leggi(piva, sa_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objCampoR As New AgronicaCoreAnagrafeDAL.Campi_R
        Dim cam = objCampoR.Leggi(piva, sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objAppR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim app = objAppR.Leggi(piva, sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, " Campo_Cod = 0 ", "", objParametri_Server)

        Try

            If cen.Rows.Count > 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, il Campo/Appezzamento
                If Validita_Inizio <> cen.Rows(0)("Validita_Inizio") Or Validita_Fine <> cen.Rows(0)("Validita_Fine") Then

                    If cam.Rows.Count > 0 Then
                        For Each c In cam.Rows
                            Dim campo_cod = c("campo_cod")

                            'Ripristino le date dopo ogni iterazione
                            Dim Validita_Inizio_CampoNew = Validita_Inizio
                            Dim Validita_Fine_CampoNew = Validita_Fine

                            'Controllo se le date del Campo rientrano nell'intervallo temporale del Centro, in questo caso rimangono invariate
                            If c("Validita_Inizio") > Validita_Inizio_CampoNew Then
                                Validita_Inizio_CampoNew = c("Validita_Inizio")
                            End If
                            If Validita_Fine_CampoNew > c("Validita_Fine") Then
                                Validita_Fine_CampoNew = c("Validita_Fine")
                            End If

                            If Validita_Fine_CampoNew <> c("Validita_Fine") Or c("Validita_Inizio") <> Validita_Inizio_CampoNew Then
                                Dim objCampR As New AgronicaCoreAnagrafeBIZ.Campo_R
                                Dim campR = objCampR.Leggi_Campo(Piva:=piva,
                                                             Sa_Cod:=sa_Cod,
                                                             Campo_Cod:=campo_cod,
                                                             objParametri_Server
                                                             )

                                campR.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_CampoNew, Validita_Fine_CampoNew)

                                Dim objCampW As New AgronicaCoreAnagrafeBIZ.Campo_W
                                Dim campW = objCampW.Scrivi_Campo_Anagrafica(campR,
                                                                         enum_TipoOperazioneDB.Modifica,
                                                                         objParametri_Server,
                                                                         objParametri_Utenti
                                                                         )
                            End If
                        Next
                    End If

                    'Appezzamenti senza campo associato
                    If app.Rows.Count > 0 Then
                        For Each a In app.Rows
                            Dim appezza = a("appezza")

                            'Ripristino le date dopo ogni iterazione
                            Dim Validita_Inizio_AppNew = Validita_Inizio
                            Dim Validita_Fine_AppNew = Validita_Fine

                            'Controllo se le date dell'Appezzamento rientrano nell'intervallo temporale del Centro, in questo caso rimangono invariate
                            If a("Validita_Inizio") > Validita_Inizio_AppNew Then
                                Validita_Inizio_AppNew = a("Validita_Inizio")
                            End If
                            If Validita_Fine_AppNew > a("Validita_Fine") Then
                                Validita_Fine_AppNew = a("Validita_Fine")
                            End If

                            If a("Validita_Inizio") <> Validita_Inizio_AppNew Or Validita_Fine_AppNew <> a("Validita_Fine") Then
                                Dim objAppsR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                                Dim appR = objAppsR.Leggi_Appezzamento_Anagrafica(Piva:=piva,
                                                                              Sa_Cod:=sa_Cod,
                                                                              Appezza:=appezza,
                                                                              0,
                                                                              Leggi_Impianti:=True,
                                                                              Leggi_Indirizzi:=True,
                                                                              Leggi_Catasto:=True,
                                                                              data:=AGRODATAINIZIO,
                                                                              filtroData:=False,
                                                                              Leggi_Distinte:=True,
                                                                              Leggi_Cartografia:=False,
                                                                              objParametri_SuperServer,
                                                                              objParametri_Server,
                                                                              objParametri_Utenti
                                                                              )
                                appR.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_AppNew, Validita_Fine_AppNew)

                                '------------------------
                                'CONTROLLO DATE IMPIANTI
                                '------------------------
                                Dim impiantiDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)
                                Dim eserciziDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

                                For Each impianto In appR.impianti

                                    'Ripristino le date dopo ogni iterazione
                                    Dim Validita_Inizio_Impianto = Validita_Inizio_AppNew
                                    Dim Validita_Fine_Impianto = Validita_Fine_AppNew

                                    'se la data di inizio dell'impianto è SUCCESSIVA alla FINE dell'Appezzamento, elimino l'Impianto
                                    'se la data di fine dell'impianto è PRECEDENTE all'INIZIO dell'Appezzamento, elimino l'Impianto
                                    If impianto.validita.inizio > Validita_Fine_AppNew OrElse impianto.validita.fine < Validita_Inizio_AppNew Then
                                        impiantiDaEliminare.Add(impianto)

                                        'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamento, in questo caso rimangono invariate
                                    ElseIf impianto.validita.inizio >= Validita_Inizio_AppNew AndAlso impianto.validita.fine <= Validita_Fine_AppNew Then
                                        If impianto.validita.inizio > Validita_Inizio_AppNew Then
                                            Validita_Inizio_Impianto = impianto.validita.inizio
                                        End If
                                        If impianto.validita.fine < Validita_Fine_AppNew Then
                                            Validita_Fine_Impianto = impianto.validita.fine
                                        End If
                                    End If

                                    'aggiorno le validità solo se l'Impianto non è stato cancellato
                                    If Not impiantiDaEliminare.Contains(impianto) Then
                                        impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Impianto, Validita_Fine_Impianto)
                                    End If


                                    '------------------------
                                    'CONTROLLO DATE ESERCIZI
                                    '------------------------
                                    For Each esercizio In impianto.esercizi

                                        'Ripristino le date dopo ogni iterazione
                                        Dim Validita_Inizio_Esercizio = Validita_Inizio_Impianto
                                        Dim Validita_Fine_Esercizio = Validita_Fine_Impianto

                                        'se la data di inizio dell'Esercizio è SUCCESSIVA alla FINE dell'Impianto, elimino l'Esercizio
                                        'se la data di fine dell'Esercizio è PRECEDENTE all'INIZIO dell'Impianto, elimino l'Esercizio
                                        If esercizio.validita.inizio > Validita_Fine_Impianto OrElse esercizio.validita.fine < Validita_Inizio_Impianto Then
                                            eserciziDaEliminare.Add(esercizio)


                                            'Controllo se le date dell'Esercizio rientrano nell'intervallo temporale dell'Impianto, in questo caso rimangono invariate
                                        ElseIf esercizio.validita.inizio >= Validita_Inizio_Impianto AndAlso esercizio.validita.fine <= Validita_Fine_Impianto Then
                                            If esercizio.validita.inizio > Validita_Inizio_Impianto Then
                                                Validita_Inizio_Esercizio = esercizio.validita.inizio
                                            End If
                                            If esercizio.validita.fine < Validita_Fine_Impianto Then
                                                Validita_Fine_Esercizio = esercizio.validita.fine
                                            End If
                                        End If

                                        'aggiorno le validità solo se l'Esercizio non è stato cancellato
                                        If Not eserciziDaEliminare.Contains(esercizio) Then
                                            esercizio.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Esercizio, Validita_Fine_Esercizio)
                                        End If
                                    Next

                                    For Each esercizio In eserciziDaEliminare
                                        impianto.esercizi.Remove(esercizio)
                                    Next
                                Next

                                For Each impianto In impiantiDaEliminare
                                    appR.impianti.Remove(impianto)
                                Next

                                Dim objAppsW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                                Dim appW = objAppsW.Appezzamento_ScriviModifica(appR, objParametri_Server, objParametri_Utenti)
                            End If
                        Next
                    End If
                End If
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore &= ("La fine del Centro non può precedere la sua data di inizio.")
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Private Sub Crea_Magazzino(ByRef objCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                               Optional ByVal nomeMagazzino As String = Nothing,
                               Optional NoteLog As String = "")

        Dim objFabbricato As New AgronicaCoreModelsSTD.anagrafiche.Fabbricato()
        objFabbricato.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK()
        objFabbricato.primaryKey.codice = 0
        objFabbricato.primaryKey.centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(objCentro.primaryKey.codice, objCentro.primaryKey.partitaIva)
        objFabbricato.descrizione = If(nomeMagazzino, "Magazzino n.01")

        If objCentro.indirizzi IsNot Nothing AndAlso objCentro.indirizzi.Count > 0 Then
            Dim indirizzo As New AgronicaCoreModelsSTD.anagrafiche.Indirizzo
            indirizzo.codice = 0
            indirizzo.cap = objCentro.indirizzi(0).indirizzo.cap
            indirizzo.frazione = objCentro.indirizzi(0).indirizzo.frazione
            indirizzo.note = objCentro.indirizzi(0).indirizzo.note
            indirizzo.stato = objCentro.indirizzi(0).indirizzo.stato
            indirizzo.via = objCentro.indirizzi(0).indirizzo.via
            indirizzo.istatComune = objCentro.indirizzi(0).indirizzo.istatComune

            objFabbricato.indirizzo = indirizzo
        End If

        objFabbricato.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(objCentro.validita.inizio, objCentro.validita.fine)

        Dim objFabbricati_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        objFabbricato.primaryKey.codice = objFabbricati_W.Scrivi_Fabbricato_Anagrafica(objFabbricato, objParametri_Server, objParametri_Utenti, NoteLog:=NoteLog)
        If objCentro.fabbricati Is Nothing Then
            objCentro.fabbricati = New List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)({objFabbricato})
        End If

    End Sub

    Private Function EliminaCentro(objCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   Optional NoteLog As String = NOTELOG_ANAGRAFE_NG)


        Dim Piva = objCentro.primaryKey.partitaIva
        Dim Sa_Cod = objCentro.primaryKey.codice
        Dim PivaSuperUser = objParametri_Server.PivaSuperUser

        'Prima di poter eliminare controllo CdG e Movimenti
        Dim objControlloAgenda As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
        Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim controlloCdG = objControlloCdG.controllo_CdGxEliminazione(Nothing, Piva, Sa_Cod, 0, 0, 0, objParametri_Server)
        If objControlloAgenda.controllo_MovimentiRicettexEliminazione(Nothing, Piva, Sa_Cod, 0, 0, objParametri_Server) Then

            Dim MessaggioErroreAgenda As String = objCentro.nome & " " & My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileEliminarCentroRegistrazioniAssociate
            Throw New GiasException(MessaggioErroreAgenda)

        ElseIf controlloCdG.errore Then

            Dim MessaggioErroreCdG As String = objCentro.nome & " " & My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileEliminarCentroCdGAssociati
            Throw New GiasException(MessaggioErroreCdG)

        End If

        Dim fabbricati = (From f In GiasContext.Fabbricati Where f.PIVA = Piva AndAlso f.SA_COD = Sa_Cod).ToList
        Dim Fabbricati_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        For Each fabbricato In fabbricati
            Dim fabbricatoObj As New AgronicaCoreModelsSTD.anagrafiche.Fabbricato
            fabbricatoObj.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK
            fabbricatoObj.primaryKey.codice = fabbricato.Fabbricato_Cod
            fabbricatoObj.primaryKey.centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK
            fabbricatoObj.primaryKey.centroAziendalePK.codice = Sa_Cod
            fabbricatoObj.primaryKey.centroAziendalePK.partitaIva = Piva
            fabbricatoObj.flag_cancellazione = True
            fabbricatoObj.tipo = fabbricato.Tipo_Fabbricato_Cod

            Fabbricati_W.Scrivi_Fabbricato_Anagrafica(fabbricatoObj, objParametri_Server, objParametri_Utenti, NoteLog:="Registrato da cancellazione Centro Aziendale (NG)")
        Next

        'Gestisco l'eliminazione delle celle (Cantina_Vasche) e delle cose a loro collegate
        HandleDeleteCelle(Piva, Sa_Cod, objParametri_Server, objParametri_Utenti, GiasContext)

        Dim campi = (From c In GiasContext.Campi Where c.Piva = Piva AndAlso c.Sa_Cod = Sa_Cod).ToList
        Dim Campi_W As New AgronicaCoreAnagrafeBIZ.Campo_W
        For Each campo In campi
            Dim campoObj As New AgronicaCoreModelsSTD.anagrafiche.Campo(New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(
            campo.Campo_Cod, New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)))
            campoObj.flag_cancellazione = True
            Campi_W.Scrivi_Campo_Anagrafica(campoObj, enum_TipoOperazioneDB.Cancellazione, objParametri_Server, objParametri_Utenti)
        Next

        Dim objAnagrafeAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        'Dim appezzamenti = (From c In GiasContext.Appezzamento Where c.PIVA = Piva And c.SA_COD = Sa_Cod And c.Campo_Cod = 0).ToList
        Dim appezzamenti = objAnagrafeAppezza.Leggi(Piva, Sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Appezzamento.Campo_Cod = 0 ", "", objParametri_Server)
        Dim Appezzamenti_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        For Each appezzamento In appezzamenti.Rows
            Dim appezzamentoObj As New AgronicaCoreModelsSTD.anagrafiche.Appezzamento(New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(
                                                                                      appezzamento("APPEZZA"),
                                                                                      New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(appezzamento("Sa_Cod"), appezzamento("Piva"))))
            appezzamentoObj.flag_cancellazione = True
            Appezzamenti_W.Appezzamento_ScriviModifica(appezzamentoObj, objParametri_Server, objParametri_Utenti)
        Next

        Dim particelle = (From p In GiasContext.ImpreseXParticelle Where p.PIVA = Piva AndAlso p.sa_cod = Sa_Cod).ToList
        Dim particelle_W As New AgronicaCoreAnagrafeBIZ.Particella_W
        Dim particelle_R As New AgronicaCoreAnagrafeBIZ.Particella_R
        For Each particella In particelle
            Dim p = particelle_R.Leggi_Particella_Anagrafica(Piva, Sa_Cod, particella.PROV, particella.COM, particella.SEZIONE, particella.FOGLIO, particella.NUMERO, particella.SUBALTERNO, True, True, True, True, objParametri_Server)
            p.flag_cancellazione = True
            particelle_W.ParticellaCatasto_Scrivi(p, Nothing, objParametri_Server, objParametri_Utenti, True)
        Next

        GiasContext.SaveChanges()


        Dim centrixRubrica = (From c In GiasContext.CentrixRubrica Where c.PIVA = Piva AndAlso c.sa_cod = Sa_Cod).ToList
        Dim rubrica_cod = (From c In centrixRubrica Select c.cod_rubrica).ToList
        Dim rubrica = (From i In GiasContext.Rubrica Where rubrica_cod.Contains(i.cod_rubrica)).ToList
        GiasContext.CentrixRubrica.RemoveRange(centrixRubrica)
        GiasContext.Rubrica.RemoveRange(rubrica)
        GiasContext.SaveChanges()

        Dim centrixIndirizzi = (From c In GiasContext.CentrixIndirizzi Where c.PIVA = Piva AndAlso c.sa_cod = Sa_Cod).ToList

        Dim indirizzi_Cod = (From c In centrixIndirizzi Select c.cod_indirizzo).ToList

        Dim indirizzi = (From i In GiasContext.Indirizzi Where indirizzi_Cod.Contains(i.cod_indirizzo)).ToList
        GiasContext.Indirizzi.RemoveRange(indirizzi)
        GiasContext.CentrixIndirizzi.RemoveRange(centrixIndirizzi)
        GiasContext.SaveChanges()

        Dim centri_Aziendali_Codici = (From c In GiasContext.Centri_Aziendali_Codici Where c.PIVA = Piva AndAlso c.sa_cod = Sa_Cod).ToList
        GiasContext.Centri_Aziendali_Codici.RemoveRange(centri_Aziendali_Codici)
        GiasContext.SaveChanges()

        Dim utentixStrutture = (From c In GiasContext.UtentiXStrutture Where c.PIVA = Piva AndAlso c.SA_COD = Sa_Cod).ToList
        GiasContext.UtentiXStrutture.RemoveRange(utentixStrutture)
        GiasContext.SaveChanges()

        Dim centro_aziendale = (From c In GiasContext.Centri_Aziendali Where c.PIVA = Piva AndAlso c.sa_cod = Sa_Cod).FirstOrDefault
        GiasContext.Centri_Aziendali.Remove(centro_aziendale)
        GiasContext.SaveChanges()


        Dim DatiCentroStr = ""
        If objCentro IsNot Nothing Then
            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiCentroStr = JsonConvert.SerializeObject(objCentro, a)
        End If

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.CentriAziendali,
                                                                             CStr(Piva), CStr(Sa_Cod),
                                                                             "", "",
                                                                             Nothing, Nothing,
                                                                             enum_TipoOperazioneDB.Cancellazione,
                                                                             objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                             NoteLog, DatiCentroStr)
        GiasContext.Agronica_Log_Anagrafe.Add(log)
        GiasContext.SaveChanges()

        Return objCentro.primaryKey.codice

    End Function

    Private Sub HandleDeleteCelle(
                                 ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal objParametriServer As AgronicaCoreParametri,
                                 ByVal objParametriUtenti As AgronicaCoreParametri,
                                 ByRef GiasContext As Gias_DeveloperServer_Entities,
                                 Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                 Optional SistemaOrigine As Integer = -1
                                 )
        Dim objFabbricatoW As New AgronicaCoreAnagrafeBIZ.Fabbricato_W

        Dim vasche = (From v In GiasContext.Cantina_Vasche Where v.Piva = piva AndAlso v.Sa_Cod = saCod).ToList
        For Each vasca In vasche
            objFabbricatoW.DeleteCella(vasca.Piva, vasca.Sa_Cod, vasca.Vas_Cod, objParametriServer, objParametriUtenti, NoteLog, SistemaOrigine)
        Next

        Dim insiemi = (From i In GiasContext.Cantina_Insiemi Where i.Piva = piva AndAlso i.Sa_Cod = saCod).ToList
        For Each insieme In insiemi
            objFabbricatoW.DeleteInsieme(insieme.Piva, insieme.Sa_Cod, insieme.Insieme_Cod, objParametriServer, objParametriUtenti, NoteLog, SistemaOrigine)
        Next

        Dim caratteristiche = (From c In GiasContext.Cantina_Caratteristiche Where c.PIVA = piva AndAlso c.sa_cod = saCod).ToList
        For Each caratteristica In caratteristiche
            objFabbricatoW.DeletePiano(caratteristica.PIVA, caratteristica.sa_cod, caratteristica.Piano_Cod, objParametriServer, objParametriUtenti, NoteLog, SistemaOrigine)
        Next

        Dim pareti = (From c In GiasContext.Cantina_Pareti Where c.PIVA = piva AndAlso c.sa_cod = saCod).ToList
        For Each parete In pareti
            objFabbricatoW.DeletePareti(parete.PIVA, parete.sa_cod, parete.Piano_Cod, objParametriServer, objParametriUtenti, NoteLog, SistemaOrigine)
        Next

    End Sub

    Public Sub Scrivi_Centro_APP(ByRef centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                                 ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal creaMagazzinoConferimento As Boolean = False,
                                 Optional NoteLog As String = NOTELOG_ANAGRAFE_APP)

        If centro.tipologia Is Nothing Then
            Dim tipologia = AgronicaCoreModelsSTD.metaschema.TipologiaSede.TIPO.SedeAziendale
            centro.tipologia = New AgronicaCoreModelsSTD.metaschema.TipologiaSede(tipologia)
        End If

        If centro.validita Is Nothing Then
            centro.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE)
        End If

        ' creo indirizzo fittizio se non passato
        If centro.indirizzi Is Nothing AndAlso tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
            Dim indirizzo As New AgronicaCoreModelsSTD.anagrafiche.Indirizzo() With {
                .via = "", .frazione = "", .cap = "00000", .note = "",
                .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat With {.com = "000", .prov = "000"},
                .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166("IT")
            }
            centro.indirizzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato) From {
                New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato() With {
                    .indirizzo = indirizzo,
                    .tipo_Indirizzo = enum_IndirizzoTipo.SedeOperativa
                }
            }
        End If

        ' modifico indirizzo centro aziendale
        If centro.indirizzi IsNot Nothing AndAlso tipoOperazione = enum_TipoOperazioneDB.Modifica Then
            Dim centrixIndirizzi As New CentrixIndirizzi_Read
            Dim indirizzi = centrixIndirizzi.Leggi(centro.primaryKey.partitaIva, centro.primaryKey.codice, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If centro.indirizzi.Count > 0 AndAlso indirizzi.Rows.Count > 0 Then
                centro.indirizzi(0).indirizzo.codice = indirizzi.Rows(0)("cod_indirizzo")
            End If
        End If

        ' ricavo telefono centro e volume magazzino
        Dim volume As Double = 0
        Dim telefono As String = ""
        If centro.codici IsNot Nothing AndAlso centro.codici.Count > 0 Then
            Dim dati = (From c In centro.codici Where c.codiceAnagrafe.codice = enum_CodiciAnagrafe.GiasAPP_Dati_Centro Select c.valore).FirstOrDefault
            If Not String.IsNullOrEmpty(dati) Then
                Dim datiCentro As JObject = JsonConvert.DeserializeObject(dati)
                If datiCentro IsNot Nothing Then
                    If datiCentro.ContainsKey("dimension") Then
                        volume = CDbl(datiCentro.GetValue("dimension"))
                    End If
                    If datiCentro.ContainsKey("telephone") Then
                        telefono = datiCentro.GetValue("telephone")
                    End If
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(telefono) AndAlso centro.rubricaVoci Is Nothing AndAlso tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
            centro.rubricaVoci = New List(Of AgronicaCoreModelsSTD.anagrafiche.RubricaVoci) From {
                New AgronicaCoreModelsSTD.anagrafiche.RubricaVoci() With {
                    .rubrica = New AgronicaCoreModelsSTD.anagrafiche.Rubrica(0) With {.tipologia = "Telefono"},
                    .valore = telefono
                }
            }
        End If

        Dim codice = Scrivi_Centro_Anagrafica(centro, objParametri_Server, objParametri_Utenti, False, NoteLog:=NoteLog)

        If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

            centro.primaryKey.codice = codice

            ' crea magazzino app
            Dim objFabbricati As New Fabbricato_W
            Dim magazzino = Crea_Magazzino_APP(centro, volume)
            objFabbricati.Scrivi_Fabbricato_APP(magazzino, tipoOperazione, objParametri_Server, objParametri_Utenti)
            centro.fabbricati = New List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato) From {magazzino}

            ' crea cella per conferimento
            If creaMagazzinoConferimento Then
                objFabbricati.Scrivi_Cella_APP(centro, tipoOperazione, objParametri_Server, objParametri_Utenti)
            End If

        End If

    End Sub

    Public Function Crea_Magazzino_APP(ByRef centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale, Optional ByVal volume As Double = 0) As AgronicaCoreModelsSTD.anagrafiche.Fabbricato

        Dim magazzino As New AgronicaCoreModelsSTD.anagrafiche.Fabbricato With {
                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK With {
                    .codice = 0,
                    .centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(centro.primaryKey.codice, centro.primaryKey.partitaIva)
                },
                .descrizione = "Magazzino " & centro.nome,
                .volumeConvenzionale = volume,
                .indirizzo = New AgronicaCoreModelsSTD.anagrafiche.Indirizzo With {
                    .codice = 0,
                    .cap = centro.indirizzi(0).indirizzo.cap,
                    .frazione = centro.indirizzi(0).indirizzo.frazione,
                    .note = centro.indirizzi(0).indirizzo.note,
                    .stato = centro.indirizzi(0).indirizzo.stato,
                    .via = centro.indirizzi(0).indirizzo.via,
                    .istatComune = centro.indirizzi(0).indirizzo.istatComune
                },
                .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(centro.validita.inizio, centro.validita.fine)
            }

        Return magazzino

    End Function

End Class
