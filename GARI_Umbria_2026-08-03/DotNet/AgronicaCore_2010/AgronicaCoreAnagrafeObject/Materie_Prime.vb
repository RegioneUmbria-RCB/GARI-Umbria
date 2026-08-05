Imports System.Runtime.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Xml
Imports System.Data.Common

<DataContract()>
Public Class Materie_Prime

    <DataMember()> Public Property Piva() As String
    <DataMember()> Public Property Sa_Cod() As Integer
    <DataMember()> Public Property Elem_Cod() As Integer
    <DataMember()> Public Property Mat_Cod() As Integer
    <DataMember()> Public Property Cod_Articolo() As String
    <DataMember()> Public Property Mat_Des() As String
    <DataMember()> Public Property Sem_Cod() As Integer
    <DataMember()> Public Property Cul_Cod() As Integer
    <DataMember()> Public Property Veg_Cod() As Integer
    <DataMember()> Public Property Trap_Dur() As Integer
    <DataMember()> Public Property Uso() As Integer
    <DataMember()> Public Property ClToss_Cod() As String
    <DataMember()> Public Property NewClToss_Cod() As String
    <DataMember()> Public Property Ditta_Cod() As Integer
    <DataMember()> Public Property N() As Decimal
    <DataMember()> Public Property P2O5() As Decimal
    <DataMember()> Public Property K2O() As Decimal
    <DataMember()> Public Property MgO() As Decimal
    <DataMember()> Public Property Note() As String
    <DataMember()> Public Property inviato() As Int16
    <DataMember()> Public Property datainvio() As Date?
    <DataMember()> Public Property data_creazione() As DateTime
    <DataMember()> Public Property data_modifica() As DateTime
    <DataMember()> Public Property username_creazione() As String
    <DataMember()> Public Property username_modifica() As String
    <DataMember()> Public Property validita_inizio() As DateTime
    <DataMember()> Public Property validita_fine() As DateTime
    <DataMember()> Public Property Regolamento() As Integer
    <DataMember()> Public Property Cal_Cod() As Integer
    <DataMember()> Public Property Prezzo_Unitario() As Decimal
    <DataMember()> Public Property Extra_Str() As String
    <DataMember()> Public Property Extra_Int() As Integer
    <DataMember()> Public Property Extra_Date() As DateTime
    <DataMember()> Public Property GRVA_COD_VEG() As Integer
    <DataMember()> Public Property GEN_COD() As Integer
    <DataMember()> Public Property SPE_COD() As Integer
    <DataMember()> Public Property RAZ_COD() As Integer
    <DataMember()> Public Property IPRO_COD() As Integer
    <DataMember()> Public Property CAT_COD() As Integer
    <DataMember()> Public Property Flag_Biologico() As Int16
    <DataMember()> Public Property Flag_Convenzionale() As Int16
    <DataMember()> Public Property Flag_NonAgricolo() As Int16
    <DataMember()> Public Property Flag_AusiliareFabbricazione() As Int16
    <DataMember()> Public Property Udm_Cod_Extra() As Integer
    <DataMember()> Public Property Flag_Extra() As Int16
    <DataMember()> Public Property ChkImballaggio() As Int16
    <DataMember()> Public Property Qta_Extra() As Decimal
    <DataMember()> Public Property Taglio() As Int16
    <DataMember()> Public Property Mat_Cod_Origine() As Integer
    <DataMember()> Public Property Piva_SuperUser_Origine() As String
    <DataMember()> Public Property ChkListino() As Int16
    <DataMember()> Public Property Grfi_Cod() As Integer
    <DataMember()> Public Property Codice_Prodotto() As Integer
    <DataMember()> Public Property Colore() As Integer
    <DataMember()> Public Property Codice_NC() As String
    <DataMember()> Public Property Manipolazioni() As Integer
    <DataMember()> Public Property Titolo_Alcol() As Decimal
    <DataMember()> Public Property ChkContenitore() As Int16
    <DataMember()> Public Property Qta_Contenitore() As Decimal
    <DataMember()> Public Property Tipo_Peso() As Int16
    <DataMember()> Public Property Tara() As Decimal
    <DataMember()> Public Property Udm_Cod() As Integer
    <DataMember()> Public Property Peso_Set() As Int16
    <DataMember()> Public Property ChkEscludi_Magazzino() As Int16
    <DataMember()> Public Property ChkEscludi_Preparazione() As Int16
    <DataMember()> Public Property Id_Accisa_Cod() As Integer
    <DataMember()> Public Property Confezione_Cod() As String
    <DataMember()> Public Property Categoria_Vino_Cod() As Int16
    <DataMember()> Public Property Tipo_Reg_Alcoli() As String
    <DataMember()> Public Property ChkAlias() As Int16
    <DataMember()> Public Property Linea_Cod() As Integer
    <DataMember()> Public Property Tipo_Default() As Integer
    <DataMember()> Public Property ChkStampa_Dettagli() As Int16
    '<DataMember()> Public Property ChkReferenza() As Int16
    '<DataMember()> Public Property Mat_Cod_Referenza() As Integer

    <DataMember()> Public Property Codice_Esterno() As String
    <DataMember()> Public Property Flag_Importato() As Integer

    <DataMember()> Public Property ID_DisciplinareAcquisti() As Integer

    'oggetti innestati
    '<DataMember()> Public Property ListaMaterie_PrimexReport() As List(Of Materie_PrimexReport)
    '<DataMember()> Public Property ListaMaterie_PrimexLotto_Configurazione() As List(Of Materie_PrimexLotto_Configurazione)
    '<DataMember()> Public Property ListaMaterie_PrimexLotto_Proprieta() As List(Of Materie_PrimexLotto_Proprieta)
    '<DataMember()> Public Property ListaMaterie_Prime_Alias() As List(Of Materie_Prime_Alias)
    <DataMember()> Public Property Materie_Prime_Dettagli() As Materie_Prime_Dettagli
    <DataMember()> Public Property ListaMaterie_Prime_ParametriQualitativi() As List(Of Materie_Prime_ParametriQualitativi)
    <DataMember()> Public Property ListaProdottiCosti() As List(Of AgronicaCoreContabObject.Prodotti_Costi)

    'campi non presenti in realtà nella tabella ma necesari per compatibilità con XML
    <DataMember()> Public Property tipo_operazione() As Integer
    <DataMember()> Public Property basecode() As Integer
    <DataMember()> Public Property topcode() As Integer

    Public Sub New()
        Piva = ""
        Sa_Cod = PRIVATO
        Elem_Cod = 0
        Mat_Cod = 0
        Cod_Articolo = ""
        Mat_Des = ""
        Sem_Cod = 0
        Cul_Cod = 0
        Veg_Cod = 0
        Trap_Dur = 0
        Uso = 0
        ClToss_Cod = ""
        NewClToss_Cod = ""
        Ditta_Cod = 0
        N = 0
        P2O5 = 0
        K2O = 0
        MgO = 0
        Note = ""
        inviato = 0
        datainvio = Nothing
        data_creazione = DateTime.Now()
        data_modifica = DateTime.Now()
        username_creazione = ""
        username_modifica = ""
        validita_inizio = AGRODATAINIZIO
        validita_fine = AGRODATAFINE
        Regolamento = 0
        Cal_Cod = 0
        Prezzo_Unitario = 0
        Extra_Str = ""
        Extra_Int = 0
        Extra_Date = AGRODATAINIZIO
        GRVA_COD_VEG = 0
        GEN_COD = 0
        SPE_COD = 0
        RAZ_COD = 0
        IPRO_COD = 0
        CAT_COD = 0
        Flag_Biologico = 0
        Flag_Convenzionale = 0
        Flag_NonAgricolo = 0
        Flag_AusiliareFabbricazione = 0
        Udm_Cod_Extra = 0
        Flag_Extra = 0
        ChkImballaggio = 0
        Qta_Extra = 0
        Taglio = 0
        Mat_Cod_Origine = 0
        Piva_SuperUser_Origine = ""
        ChkListino = 0
        Grfi_Cod = 0
        Codice_Prodotto = 0
        Colore = 0
        Codice_NC = ""
        Manipolazioni = 0
        Titolo_Alcol = 0
        ChkContenitore = 0
        Qta_Contenitore = 0
        Tipo_Peso = 0
        Tara = 0
        Udm_Cod = 0
        Peso_Set = 0
        ChkEscludi_Magazzino = 0
        ChkEscludi_Preparazione = 0
        Id_Accisa_Cod = 0
        Confezione_Cod = ""
        Categoria_Vino_Cod = 0
        Tipo_Reg_Alcoli = ""
        ChkAlias = 0
        Linea_Cod = 0
        Tipo_Default = 0
        ChkStampa_Dettagli = 0
        'ChkReferenza = 0
        'Mat_Cod_Referenza = 0
        Codice_Esterno = ""
        Flag_Importato = 0
        ID_DisciplinareAcquisti = 0

        'ListaMaterie_PrimexReport = New List(Of Materie_PrimexReport)
        'ListaMaterie_PrimexLotto_Configurazione = New List(Of Materie_PrimexLotto_Configurazione)
        'ListaMaterie_PrimexLotto_Proprieta = New List(Of Materie_PrimexLotto_Proprieta)
        'ListaMaterie_Prime_Alias = New List(Of Materie_Prime_Alias)
        Materie_Prime_Dettagli = New Materie_Prime_Dettagli
        ListaMaterie_Prime_ParametriQualitativi = New List(Of Materie_Prime_ParametriQualitativi)

        tipo_operazione = 0
        basecode = 0
        topcode = 0

    End Sub

End Class


Public Class Materie_Prime_R
    Inherits AgronicaCoreDataProvider.LogProvider
    '##################################################################################
    'bEscludiRegistri: default = false
    Public Function Materia_Prima_Leggi(ByVal piva As String, _
                                        ByVal Mat_Cod As Integer, _
                                        ByVal ForDelete As Boolean, _
                                        ByVal bEscludiRegistri As Boolean, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        Optional ByVal TipoG2G As Integer = 0 _
                                        ) As List(Of Materie_Prime)


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_R.Materia_Prima_Leggi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False

        'Dim MPxReport_R As New AgronicaCoreAnagrafeObject.Materie_PrimexReport_R
        'Dim MPxLC As New AgronicaCoreAnagrafeObject.Materie_PrimexLotto_Configurazione_R
        'Dim MPxLP As New AgronicaCoreAnagrafeObject.Materie_PrimexLotto_Proprieta_R
        'Dim MP_Alias As New AgronicaCoreAnagrafeObject.Materie_Prime_Alias_R
        Dim MP_Dettagli As New AgronicaCoreAnagrafeObject.Materie_Prime_Dettagli_R
        Dim MP_PQ As New AgronicaCoreAnagrafeObject.Materie_Prime_ParametriQualitativi_R

        Dim listaMateriePrime As New List(Of Materie_Prime)

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------

            '------------------------------

            'Mi procuro un elenco delle Materie_Prime all'interno della finestra temporale selezionata
            Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim DtMaterie_Prime As DataTable = objMaterie_Prime.Leggi(CStr(piva), _
                                                        0, 0, _
                                                        CInt(Mat_Cod), _
                                                        "", 0, 0, 0, 0, 0, 0, 0, "", _
                                                        0, "", True, False, "", _
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                        "", "", objParametri, TipoG2G)


            'Se il DT è vuoto, ritorno Nothing altrimenti popolo l'elemento
            If Not IsNothing(DtMaterie_Prime) Then

                'Effettuo un ciclo sulle Materie_Prime
                For Each dRowMaterie_Prime As DataRow In DtMaterie_Prime.Rows

                    Dim mp As New Materie_Prime()
                    mp.Piva = dRowMaterie_Prime("PIVA")
                    mp.Sa_Cod = dRowMaterie_Prime("sa_cod")
                    mp.Elem_Cod = dRowMaterie_Prime("elem_cod")
                    mp.Mat_Cod = dRowMaterie_Prime("mat_cod")
                    mp.Cod_Articolo = dRowMaterie_Prime("cod_articolo")
                    mp.Mat_Des = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("mat_des"), mp.Mat_Des.GetType)
                    mp.Sem_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("sem_cod"), mp.Sem_Cod.GetType)
                    mp.Cul_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("cul_cod"), mp.Cul_Cod.GetType)
                    mp.Veg_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("veg_cod"), mp.Veg_Cod.GetType)
                    mp.Trap_Dur = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("trap_dur"), mp.Trap_Dur.GetType)
                    mp.Uso = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("uso"), mp.Uso.GetType)
                    mp.ClToss_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("cltoss_cod"), mp.ClToss_Cod.GetType)
                    mp.NewClToss_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("newcltoss_cod"), mp.NewClToss_Cod.GetType)
                    mp.Ditta_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("ditta_cod"), mp.Ditta_Cod.GetType)
                    mp.N = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("n"), mp.N.GetType)
                    mp.P2O5 = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("p2o5"), mp.P2O5.GetType)
                    mp.K2O = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("k2o"), mp.K2O.GetType)
                    mp.MgO = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("mgo"), mp.MgO.GetType)
                    mp.Note = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("note"), mp.Note.GetType)

                    mp.inviato = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("inviato"), mp.inviato.GetType)
                    mp.datainvio = DBNullToNothing(dRowMaterie_Prime("datainvio"))
                    mp.data_creazione = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("data_creazione"), mp.data_creazione.GetType)
                    mp.data_modifica = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("data_modifica"), mp.data_modifica.GetType)
                    mp.username_creazione = dRowMaterie_Prime("username_creazione")
                    mp.username_modifica = dRowMaterie_Prime("username_modifica")
                    mp.validita_inizio = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("Validita_Inizio"), mp.validita_inizio.GetType)
                    mp.validita_fine = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("Validita_Fine"), mp.validita_fine.GetType)

                    mp.Regolamento = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("regolamento"), mp.Regolamento.GetType)
                    mp.Cal_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("cal_cod"), mp.Cal_Cod.GetType)
                    mp.Prezzo_Unitario = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("prezzo_unitario"), mp.Prezzo_Unitario.GetType)
                    mp.Extra_Str = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("extra_str"), mp.Extra_Str.GetType)
                    mp.Extra_Int = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("extra_int"), mp.Extra_Int.GetType)
                    mp.Extra_Date = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("extra_date"), mp.Extra_Date.GetType)
                    mp.GRVA_COD_VEG = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("grva_cod_veg"), mp.GRVA_COD_VEG.GetType)
                    mp.GEN_COD = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("gen_cod"), mp.GEN_COD.GetType)
                    mp.SPE_COD = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("spe_cod"), mp.SPE_COD.GetType)
                    mp.RAZ_COD = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("raz_cod"), mp.RAZ_COD.GetType)
                    mp.IPRO_COD = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("ipro_cod"), mp.IPRO_COD.GetType)
                    mp.CAT_COD = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("cat_cod"), mp.CAT_COD.GetType)
                    mp.Flag_Biologico = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("flag_biologico"), mp.Flag_Biologico.GetType)
                    mp.Flag_Convenzionale = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("flag_convenzionale"), mp.Flag_Convenzionale.GetType)
                    mp.Flag_NonAgricolo = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("flag_nonagricolo"), mp.Flag_NonAgricolo.GetType)
                    mp.Flag_AusiliareFabbricazione = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("flag_ausiliarefabbricazione"), mp.Flag_AusiliareFabbricazione.GetType)
                    mp.Udm_Cod_Extra = dRowMaterie_Prime("udm_cod_extra")
                    mp.Flag_Extra = dRowMaterie_Prime("flag_extra")
                    mp.ChkImballaggio = dRowMaterie_Prime("chkimballaggio")
                    mp.Qta_Extra = dRowMaterie_Prime("qta_extra")
                    mp.Taglio = dRowMaterie_Prime("taglio")
                    mp.Mat_Cod_Origine = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("mat_cod_origine"), mp.Mat_Cod_Origine.GetType)
                    mp.Piva_SuperUser_Origine = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("Piva_SuperUser_Origine"), mp.Piva_SuperUser_Origine.GetType)
                    mp.ChkListino = dRowMaterie_Prime("chklistino")
                    mp.Grfi_Cod = dRowMaterie_Prime("grfi_cod")
                    mp.Codice_Prodotto = dRowMaterie_Prime("codice_prodotto")
                    mp.Colore = dRowMaterie_Prime("colore")
                    mp.Codice_NC = dRowMaterie_Prime("codice_nc")
                    mp.Manipolazioni = dRowMaterie_Prime("manipolazioni")
                    mp.Titolo_Alcol = dRowMaterie_Prime("titolo_alcol")
                    mp.ChkContenitore = dRowMaterie_Prime("chkcontenitore")
                    mp.Qta_Contenitore = dRowMaterie_Prime("qta_contenitore")
                    mp.Tipo_Peso = dRowMaterie_Prime("tipo_peso")
                    mp.Tara = dRowMaterie_Prime("tara")
                    mp.Udm_Cod = dRowMaterie_Prime("udm_cod")
                    mp.Peso_Set = dRowMaterie_Prime("peso_set")
                    mp.ChkEscludi_Magazzino = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("chkescludi_magazzino"), mp.ChkEscludi_Magazzino.GetType)
                    mp.ChkEscludi_Preparazione = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("chkescludi_preparazione"), mp.ChkEscludi_Preparazione.GetType)
                    mp.Id_Accisa_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("id_accisa_cod"), mp.Id_Accisa_Cod.GetType)
                    mp.Confezione_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("confezione_cod"), mp.Confezione_Cod.GetType)
                    mp.Categoria_Vino_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("categoria_vino_cod"), mp.Categoria_Vino_Cod.GetType)
                    mp.Tipo_Reg_Alcoli = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("tipo_reg_alcoli"), mp.Tipo_Reg_Alcoli.GetType)
                    mp.ChkAlias = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("chkalias"), mp.ChkAlias.GetType)

                    mp.Linea_Cod = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("Linea_Cod"), mp.Linea_Cod.GetType)
                    mp.Tipo_Default = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("Tipo_Default"), mp.Tipo_Default.GetType)
                    mp.ChkStampa_Dettagli = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("ChkStampa_Dettagli"), mp.ChkStampa_Dettagli.GetType)
                    'mp.ChkReferenza = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("ChkReferenza"))
                    'mp.Mat_Cod_Referenza = Agro_SQL_Load_ConDefault(dRowMaterie_Prime("Mat_Cod_Referenza"))
                    mp.basecode = 0
                    mp.topcode = 2000000000

                    '########################################################
                    '###############  Materia_Prima X Report  ###############
                    '########################################################
                    'mp.ListaMaterie_PrimexReport = MPxReport_R.Leggi(dRowMaterie_Prime.Item("Piva"), _
                    '                                                       0, _
                    '                                                       dRowMaterie_Prime.Item("Mat_Cod"), _
                    '                                                       0, _
                    '                                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    '                                                       "", "", objParametri)


                    '########################################################
                    '#####  Materia_Prima X Lotto Configurazione  ###########
                    '########################################################
                    'mp.ListaMaterie_PrimexLotto_Configurazione = MPxLC.Leggi_xAltoLivello(dRowMaterie_Prime.Item("Piva"), _
                    '                                                0, 0, 0, _
                    '                                                CInt(dRowMaterie_Prime.Item("Mat_Cod")), _
                    '                                                0, -1, -1, -1, _
                    '                                                "", "", objParametri)


                    '########################################################
                    '#####  Materia_Prima X Lotto Proprietà  ################
                    '########################################################
                    'mp.ListaMaterie_PrimexLotto_Proprieta = MPxLP.Leggi(dRowMaterie_Prime.Item("Piva"), _
                    '                                            0, 0, 0, 0, _
                    '                                            CInt(dRowMaterie_Prime.Item("Mat_Cod")), _
                    '                                            0, "", 0, "", 0, "", 0, _
                    '                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    '                                            "", "", objParametri)



                    '########################################################
                    '#################  Materia_Prima Alias  ################
                    '########################################################
                    'mp.ListaMaterie_Prime_Alias = MP_Alias.Leggi(dRowMaterie_Prime.Item("Piva"), _
                    '                                                    CInt(dRowMaterie_Prime.Item("Mat_Cod")), _
                    '                                                    "", _
                    '                                                    "", "", objParametri)


                    '########################################################
                    '##############  Materia_Prima Dettagli  ################
                    '########################################################
                    mp.Materie_Prime_Dettagli = MP_Dettagli.Leggi(dRowMaterie_Prime.Item("Piva"), _
                                                                        CInt(dRowMaterie_Prime.Item("Mat_Cod")), _
                                                                        ForDelete, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                        "", "", objParametri).First()


                    '########################################################
                    '#####  Materia_Prima Parametri Qualitativi  ############
                    '########################################################

                    'Nota: Al fine di duplicare una materia prima occorre reimpostare i riepiloghi nei registri
                    Dim stringaFiltro_PQ As String = IIf(bEscludiRegistri, "ChkRegistri = 0 And ChkRegistri_Vinificazione = 0", "")

                    mp.ListaMaterie_Prime_ParametriQualitativi = MP_PQ.Leggi(dRowMaterie_Prime.Item("Piva"), _
                                                                   0, _
                                                                   CInt(dRowMaterie_Prime.Item("Mat_Cod")), _
                                                                   "", 0, 0, _
                                                                   ForDelete, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                   stringaFiltro_PQ, "", objParametri)

                    'Aggiungo l'elemento alla lista
                    listaMateriePrime.Add(mp)
                Next

            Else
                'se non ho nessuna materia prima allora ritorno nothing
                listaMateriePrime = Nothing
            End If

        Catch ex As Exception
            listaMateriePrime = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            If FlagConnessioneLocale = True Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If
        End Try

        'Restituisco il risultato
        Return listaMateriePrime

    End Function



End Class

Public Class Materie_Prime_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'se ho un solo elemento, lo inserisco all'interno di una lista e poi richiamo la funzione base
    Public Function Materia_Prima_Scrivi( _
                                ByVal ObjMateriaPrima As Materie_Prime, _
                                ByRef OUTPUT_Mat_Cod As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                As Boolean

        Dim ListaMateriePrime As New List(Of Materie_Prime)
        ListaMateriePrime.Add(ObjMateriaPrima)

        Return Materia_Prima_Scrivi(ListaMateriePrime, OUTPUT_Mat_Cod, objParametri)

    End Function

    '============================================================================
    Public Function Materia_Prima_Scrivi( _
                                ByVal ListaMateriePrime As List(Of Materie_Prime), _
                                ByRef OUTPUT_Mat_Cod As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_W.Materia_Prima_Scrivi()"
        '----------------------------------------------------------------------

        Dim resQuery As Boolean
        Dim DummyProdotti_Costi As Boolean
        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_W
        Dim ObjProdotti_Costi As New AgronicaCoreContabObject.Prodotti_Costi_W
        Dim ObjMaterie_Prime_PQ As New AgronicaCoreAnagrafeObject.Materie_Prime_ParametriQualitativi_W
        Dim ObjMaterie_Prime_Dettagli As New AgronicaCoreAnagrafeObject.Materie_Prime_Dettagli_W

        Dim Cod_Materia_Prima As Int32

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False 'true
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Try
            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)

            For Each mp As Materie_Prime In ListaMateriePrime

                'Inizializzo Preventivamente il Cod_Materia_Prima
                Cod_Materia_Prima = mp.Mat_Cod

                'Verifico l'operazione richiesta
                Select Case mp.tipo_operazione

                    Case "0"    'LEGGI -------------------------------------------------------
                        OUTPUT_Mat_Cod = Cod_Materia_Prima

                    Case "1"    'SALVA -------------------------------------------------------

                        'se il codice non è già definito
                        If Cod_Materia_Prima <= 0 Then
                            'Richiedo un nuovo codice materia prima
                            Cod_Materia_Prima = ObjSequenze.NuovoId_Tabella( _
                                            "Materie_Prime", mp.basecode, mp.topcode, objParametri)
                        End If

                        OUTPUT_Mat_Cod = Cod_Materia_Prima

                        resQuery = objMaterie_Prime.Scrivi(
                                            mp.Piva, mp.Sa_Cod, mp.Elem_Cod,
                                            Cod_Materia_Prima,
                                            mp.Mat_Des, mp.Sem_Cod, mp.Veg_Cod, mp.Cul_Cod,
                                            UtilityProvider.Agro_SQL_SaveNum(mp.GEN_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.SPE_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.IPRO_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.RAZ_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.CAT_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_Biologico),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_Convenzionale),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_NonAgricolo),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_AusiliareFabbricazione),
                                            mp.Trap_Dur, mp.Uso,
                                            mp.ClToss_Cod, mp.NewClToss_Cod, mp.Ditta_Cod,
                                            mp.N, mp.P2O5, mp.K2O, mp.MgO,
                                            mp.Note, mp.Regolamento,
                                            mp.Cal_Cod, mp.Cod_Articolo,
                                            mp.Prezzo_Unitario,
                                            mp.Extra_Int, mp.Extra_Str, mp.Extra_Date,
                                            UtilityProvider.Agro_SQL_SaveNum(mp.GRVA_COD_VEG),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_Extra),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.ChkImballaggio),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Udm_Cod_Extra),
                                            mp.Qta_Extra,
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Taglio),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Grfi_Cod),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.ChkListino),
                                            mp.Mat_Cod_Origine, mp.Piva_SuperUser_Origine,
                                            mp.validita_inizio, mp.validita_fine,
                                            mp.Codice_Esterno, mp.Flag_Importato,
                                            objParametri, mp.ID_DisciplinareAcquisti)


                    Case "2"    'MODIFICA -------------------------------------------------------

                        OUTPUT_Mat_Cod = Cod_Materia_Prima

                        Dim Cod_Articolo_New As String = mp.Cod_Articolo

                        '  Marco Grilli, 09/02/2015 11:08:10: probabilemtne è una cosa vecchia quindi
                        'imposto il valore direttamente a mp.Cod_Articolo
                        Dim Cod_Articolo As String = mp.Cod_Articolo
                        'If (xMateria_Prima.HasAttribute("cod_articolo_old") = False) Then
                        '   Cod_Articolo = mp.Cod_Articolo
                        'Else
                        '   Cod_Articolo = CStr(xMateria_Prima.GetAttribute("cod_articolo_old"))
                        'End If

                        objMaterie_Prime.Modifica2(
                                            mp.Piva, mp.Sa_Cod,
                                            Cod_Articolo_New,
                                            mp.Elem_Cod,
                                            Cod_Materia_Prima,
                                            mp.Mat_Des, mp.Sem_Cod, mp.Veg_Cod, mp.Cul_Cod,
                                            UtilityProvider.Agro_SQL_SaveNum(mp.GEN_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.SPE_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.IPRO_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.RAZ_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.CAT_COD),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_Biologico),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_Convenzionale),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_NonAgricolo),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_AusiliareFabbricazione),
                                            mp.Trap_Dur, mp.Uso,
                                            mp.ClToss_Cod, mp.NewClToss_Cod, mp.Ditta_Cod,
                                            mp.N, mp.P2O5, mp.K2O, mp.MgO,
                                            mp.Note, mp.Regolamento,
                                            mp.Cal_Cod,
                                            Cod_Articolo,
                                            mp.Prezzo_Unitario,
                                            mp.Extra_Int, mp.Extra_Str, mp.Extra_Date,
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Flag_Extra),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.ChkImballaggio),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Udm_Cod_Extra),
                                            mp.Qta_Extra,
                                            UtilityProvider.Agro_SQL_SaveNum(mp.GRVA_COD_VEG),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Taglio),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.Grfi_Cod),
                                            UtilityProvider.Agro_SQL_SaveNum(mp.ChkListino),
                                            mp.validita_inizio, mp.validita_fine,
                                            mp.Codice_Esterno, mp.Flag_Importato,
                                            "", objParametri, mp.ID_DisciplinareAcquisti)


                    Case "3" 'CANCELLAZIONE  ------------------------------------------------------

                        OUTPUT_Mat_Cod = Cod_Materia_Prima

                        objMaterie_Prime.Cancella2( _
                                            mp.Elem_Cod, Cod_Materia_Prima, mp.Piva, mp.Cod_Articolo, _
                                            "", objParametri)

                End Select

                '#########################################################
                '###################  MATERIA PRIMA DETTAGLI #############
                '#########################################################
                Dim mp_det As Materie_Prime_Dettagli = mp.Materie_Prime_Dettagli

                'Verifico l'operazione richiesta
                Select Case mp_det.tipo_operazione

                    Case "0"    'LEGGI -------------------------------------------------------

                    Case "1"    'SALVA -------------------------------------------------------
                        'Salvo i Dettagli della Materia Prima
                        resQuery = ObjMaterie_Prime_Dettagli.Scrivi( _
                                    mp.Piva, _
                                    Cod_Materia_Prima, _
                                    mp_det.Extra_Smallint1, mp_det.Extra_Smallint2, _
                                    mp_det.Extra_Smallint3, mp_det.Extra_Smallint4, _
                                    mp_det.Extra_Smallint5, mp_det.Extra_Smallint6, _
                                    mp_det.Extra_Int1, mp_det.Extra_Int2, _
                                    mp_det.Extra_Int3, mp_det.Extra_Int4, _
                                    mp_det.Extra_Int5, mp_det.Extra_Int6, _
                                    mp_det.Extra_Dbl1, mp_det.Extra_Dbl2, _
                                    mp_det.Extra_Dbl3, mp_det.Extra_Dbl4, _
                                    mp_det.Extra_Dbl5, mp_det.Extra_Dbl6, _
                                    mp_det.Extra_Str1, mp_det.Extra_Str2, _
                                    mp_det.Extra_Str3, mp_det.Extra_Str4, _
                                    mp_det.Extra_Str5, mp_det.Extra_Str6, _
                                    mp_det.Extra_Str7, mp_det.Extra_Str8, mp_det.Extra_Str9, _
                                    mp_det.Extra_Date1, mp_det.Extra_Date2, _
                                    mp_det.Extra_Date3, mp_det.Extra_Date4, _
                                    mp_det.Extra_Date5, mp_det.Extra_Date6, _
                                    mp_det.validita_inizio, mp_det.validita_fine,
                                    objParametri)

                    Case "2"    'MODIFICA -------------------------------------------------------
                        ObjMaterie_Prime_Dettagli.Modifica( _
                                    mp.Piva, _
                                    Cod_Materia_Prima, _
                                    mp_det.Extra_Smallint1, mp_det.Extra_Smallint2, _
                                    mp_det.Extra_Smallint3, mp_det.Extra_Smallint4, _
                                    mp_det.Extra_Smallint5, mp_det.Extra_Smallint6, _
                                    mp_det.Extra_Int1, mp_det.Extra_Int2, _
                                    mp_det.Extra_Int3, mp_det.Extra_Int4, _
                                    mp_det.Extra_Int5, mp_det.Extra_Int6, _
                                    mp_det.Extra_Dbl1, mp_det.Extra_Dbl2, _
                                    mp_det.Extra_Dbl3, mp_det.Extra_Dbl4, _
                                    mp_det.Extra_Dbl5, mp_det.Extra_Dbl6, _
                                    mp_det.Extra_Str1, mp_det.Extra_Str2, _
                                    mp_det.Extra_Str3, mp_det.Extra_Str4, _
                                    mp_det.Extra_Str5, mp_det.Extra_Str6, _
                                    mp_det.Extra_Str7, mp_det.Extra_Str8, mp_det.Extra_Str9, _
                                    mp_det.Extra_Date1, mp_det.Extra_Date2, _
                                    mp_det.Extra_Date3, mp_det.Extra_Date4, _
                                    mp_det.Extra_Date5, mp_det.Extra_Date6, _
                                    mp_det.validita_inizio, mp_det.validita_fine,
                                    "", objParametri)

                    Case "3"    'ELIMINA -------------------------------------------------------
                        ObjMaterie_Prime_Dettagli.Cancella( _
                                    mp.Piva, Cod_Materia_Prima, "", objParametri)
                End Select

                '#####################################
                '##########  PRODOTTI COSTI   ########
                '#####################################
                If Not IsNothing(mp.ListaProdottiCosti) AndAlso mp.ListaProdottiCosti.Count > 0 Then
                    DummyProdotti_Costi = ObjProdotti_Costi.Prodotti_Costi_Scrivi( _
                                            mp.ListaProdottiCosti, Cod_Materia_Prima, objParametri)
                End If

                '############################################
                '##########  PARAMETRI QUALITATIVI   ########
                '############################################
                For Each mp_PQ As Materie_Prime_ParametriQualitativi In mp.ListaMaterie_Prime_ParametriQualitativi

                    'Verifico l'operazione richiesta
                    Select Case mp_PQ.tipo_operazione

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------
                            resQuery = ObjMaterie_Prime_PQ.Scrivi( _
                                        mp.Piva, mp.Sa_Cod, _
                                        Cod_Materia_Prima, _
                                        mp_PQ.Tipo, mp_PQ.Tipo_Cod, mp_PQ.Udm_Cod, _
                                        mp_PQ.Valore_Des, mp_PQ.Valore_Min, mp_PQ.Valore_Max, _
                                        mp_PQ.ChkRegistri, mp_PQ.ChkCalibri, _
                                        mp.validita_inizio, mp.validita_fine, _
                                        objParametri)

                        Case "2"    'MODIFICA -------------------------------------------------------
                            ObjMaterie_Prime_PQ.Modifica( _
                                        mp.Piva, mp.Sa_Cod, _
                                        Cod_Materia_Prima, _
                                        mp_PQ.Tipo, mp_PQ.Tipo_Cod, mp_PQ.Udm_Cod, _
                                        mp_PQ.Valore_Des, mp_PQ.Valore_Min, mp_PQ.Valore_Max, _
                                        mp_PQ.ChkRegistri, mp_PQ.ChkCalibri, _
                                        mp.validita_inizio, mp.validita_fine, _
                                        "", objParametri)

                        Case "3"    'CANCELLA -------------------------------------------------------
                            ObjMaterie_Prime_PQ.Cancella( _
                                        mp.Piva, mp.Sa_Cod, _
                                        Cod_Materia_Prima, _
                                        mp_PQ.Tipo, mp_PQ.Tipo_Cod, mp_PQ.Udm_Cod,
                                        "", objParametri)
                    End Select
                Next

            Next

            'Restituisco un valore Dummy
            xRisp = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            If Not IsNothing(objParametri.objTransazione) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Mat_Cod=" + CStr(OUTPUT_Mat_Cod) + ")" + " : " + ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function


End Class