Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Lotto_AssegnaxRisumSpeVarQualCert_BIZ
    Inherits AgronicaCoreDataProvider.LogProvider

#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    '##############################################################################################

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property

    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    '##############################################################################################

    Public Function Aggiorna_LottoAssegna(
            ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByVal tutteleRighe As String,
            ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim keys As String() = Nothing
        Dim chiaveDescrittiva = Nothing
        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AnagrafeBIZ.Lotto_AssegnaxRisumSpeVarQualCert_BIZ.Aggiorna_LottoAssegna()"

        Try
            Dim lotto_ass_R As New Lotto_AssegnaxRisumSpeVarQualCert_DAL_R
            Dim lottoAssegna As New Lotto_AssegnaxRisumSpeVarQualCert
            Dim lottoAssegnaInsert As New Lotto_AssegnaxRisumSpeVarQualCert

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray
                lottoAssegnaInsert = New Lotto_AssegnaxRisumSpeVarQualCert
                lottoAssegnaInsert.Piva_SuperUser = Piva_SuperUser
                lottoAssegnaInsert.Piva = piva
                lottoAssegnaInsert.Cod_RisUm = obj("Cod_RisUm")
                lottoAssegnaInsert.Veg_Cod = obj("Veg_Cod")
                lottoAssegnaInsert.Cul_Cod = obj("Cul_Cod")
                lottoAssegnaInsert.Qual_Cod = obj("qualita_cod")
                lottoAssegnaInsert.Cert_Cod = obj("certif_cod")
                lottoAssegnaInsert.Lotto = obj("lotto")
                lottoAssegnaInsert.Data_Creazione = Date.Now
                lottoAssegnaInsert.Username_Creazione = objParametri.UsernameOperazione
                lottoAssegnaInsert.Data_Modifica = Date.Now
                lottoAssegnaInsert.Username_Modifica = objParametri.UsernameOperazione
                lottoAssegnaInsert.inviato = 0
                lottoAssegnaInsert.Validita_Inizio = Date.ParseExact(obj("validita_inizio"), Format, Provider)
                lottoAssegnaInsert.Validita_Fine = Date.ParseExact(obj("validita_fine"), Format, Provider)
                lottoAssegnaInsert.Preparazione_Cod = obj("Preparazione_Cod")
                lottoAssegnaInsert.Reg_Cod = obj("Reg_Cod")
                lottoAssegnaInsert.Tipo_Lotto = obj("Tipo_Lotto")
                lottoAssegnaInsert.Algoritmo_Config = obj("Algoritmo_Config_String")
                lottoAssegnaInsert.Separatore_Config = obj("Separatore_Config_Des")

                If obj("Tipo_Lotto") = "E" Then

                    ' Controllo che non ci siano periodi sovrapposti già registrati
                    MessaggioErrore += CheckLottoAssegna(lottoAssegnaInsert, Nothing, CStr(obj("Rag_Soc")), CStr(obj("Veg_Des")), CStr(obj("Cul_Des")), CStr(obj("qualita_des")), CStr(obj("certif_des")), objParametri)

                ElseIf obj("Tipo_Lotto") = "L" Then

                    'Controllo che non ci siano altre righe con la stessa Tipologia di Lavorazione, Specie,Varietà e Regolamento
                    Dim Righe_Trovate As Integer = 0

                    For Each obj_r As JObject In tutteleRigheArray

                        If CInt(obj_r("Preparazione_Cod")) = lottoAssegnaInsert.Preparazione_Cod AndAlso
                           CInt(obj_r("Veg_Cod")) = lottoAssegnaInsert.Veg_Cod AndAlso
                           CInt(obj_r("Cul_Cod")) = lottoAssegnaInsert.Cul_Cod AndAlso
                           CInt(obj_r("Reg_Cod")) = lottoAssegnaInsert.Reg_Cod Then

                            Righe_Trovate += 1
                        End If

                    Next

                    If Righe_Trovate > 1 Then
                        MessaggioErrore += "Esistono altre righe con  " & obj("Preparazione_Des").ToString() & " " & obj("Veg_Des").ToString() & " " &
                            obj("Cul_Des").ToString() & " " & obj("Reg_Des").ToString()
                    End If

                End If

                EFArrayToInsert.Add(lottoAssegnaInsert)
            Next

            For Each obj As JObject In righeModificateArray
                keys = CStr(obj("key_lotto_assegna")).Split("_")
                Dim dataInizio = keys(5).Substring(0, 2) & "/" & keys(5).Substring(2, 2) & "/" & keys(5).Substring(4, 4)
                Dim dataFine = keys(6).Substring(0, 2) & "/" & keys(6).Substring(2, 2) & "/" & keys(6).Substring(4, 4)
                lottoAssegna = lotto_ass_R.Leggi_Elem_Lotto_Assegna(piva, keys(0), keys(1), keys(2), keys(3), keys(4), CDate(dataInizio), CDate(dataFine), keys(7), keys(8), CStr(obj("Tipo_Lotto")), objParametri)
                If lottoAssegna Is Nothing Then
                    chiaveDescrittiva = CStr(obj("Rag_Soc")) & " " &
                                        CStr(obj("Veg_Des")) & " " &
                                        CStr(obj("Cul_Des")) & " " &
                                        CStr(obj("qualita_des")) & " " &
                                        CStr(obj("certif_des")) & " " &
                                        CDate(obj("validita_inizio")).ToShortDateString & " " &
                                        CDate(obj("validita_fine")).ToShortDateString & " " &
                                        CStr(obj("Preparazione_Des")) & " " &
                                        CStr(obj("Reg_Des"))
                    MessaggioErrore += "Riga da aggiornare non trovata: " & chiaveDescrittiva
                Else
                    ' Visto che lasciamo modificare anche i campi chiave faccio delete + insert
                    EFArrayToDelete.Add(lottoAssegna)

                    lottoAssegnaInsert = New Lotto_AssegnaxRisumSpeVarQualCert
                    lottoAssegnaInsert.Piva_SuperUser = Piva_SuperUser
                    lottoAssegnaInsert.Piva = piva
                    lottoAssegnaInsert.Cod_RisUm = obj("Cod_RisUm")
                    lottoAssegnaInsert.Veg_Cod = obj("Veg_Cod")
                    lottoAssegnaInsert.Cul_Cod = obj("Cul_Cod")
                    lottoAssegnaInsert.Qual_Cod = obj("qualita_cod")
                    lottoAssegnaInsert.Cert_Cod = obj("certif_cod")
                    lottoAssegnaInsert.Lotto = obj("lotto")
                    lottoAssegnaInsert.Data_Creazione = lottoAssegna.Data_Creazione
                    lottoAssegnaInsert.Username_Creazione = lottoAssegna.Username_Creazione
                    lottoAssegnaInsert.Data_Modifica = Date.Now
                    lottoAssegnaInsert.Username_Modifica = objParametri.UsernameOperazione
                    lottoAssegnaInsert.inviato = 0
                    lottoAssegnaInsert.Validita_Inizio = Date.ParseExact(obj("validita_inizio"), Format, Provider)
                    lottoAssegnaInsert.Validita_Fine = Date.ParseExact(obj("validita_fine"), Format, Provider)
                    lottoAssegnaInsert.Preparazione_Cod = obj("Preparazione_Cod")
                    lottoAssegnaInsert.Reg_Cod = obj("Reg_Cod")
                    lottoAssegnaInsert.Tipo_Lotto = obj("Tipo_Lotto")
                    lottoAssegnaInsert.Algoritmo_Config = obj("Algoritmo_Config_String")
                    lottoAssegnaInsert.Separatore_Config = obj("Separatore_Config_Des")

                    If obj("Tipo_Lotto") = "E" Then

                        ' Controllo che non ci siano periodi sovrapposti già registrati - solo se è cambiata la chiave
                        If lottoAssegnaInsert.Piva_SuperUser <> lottoAssegna.Piva_SuperUser OrElse
                            lottoAssegnaInsert.Piva <> lottoAssegna.Piva OrElse
                            lottoAssegnaInsert.Cod_RisUm <> lottoAssegna.Cod_RisUm OrElse
                            lottoAssegnaInsert.Veg_Cod <> lottoAssegna.Veg_Cod OrElse
                            lottoAssegnaInsert.Cul_Cod <> lottoAssegna.Cul_Cod OrElse
                            lottoAssegnaInsert.Qual_Cod <> lottoAssegna.Qual_Cod OrElse
                            lottoAssegnaInsert.Cert_Cod <> lottoAssegna.Cert_Cod OrElse
                            lottoAssegnaInsert.Validita_Inizio <> lottoAssegna.Validita_Inizio OrElse
                            lottoAssegnaInsert.Validita_Fine <> lottoAssegna.Validita_Fine Then
                            MessaggioErrore += CheckLottoAssegna(lottoAssegnaInsert, Nothing, CStr(obj("Rag_Soc")), CStr(obj("Veg_Des")), CStr(obj("Cul_Des")), CStr(obj("qualita_des")), CStr(obj("certif_des")), objParametri)
                        End If

                    ElseIf obj("Tipo_Lotto") = "L" Then

                        'Controllo che non ci siano altre righe con la stessa Tipologia di Lavorazione, Specie,Varietà e Regolamento
                        Dim Righe_Trovate As Integer = 0

                        For Each obj_r As JObject In tutteleRigheArray

                            If CInt(obj_r("Preparazione_Cod")) = lottoAssegnaInsert.Preparazione_Cod AndAlso
                               CInt(obj_r("Veg_Cod")) = lottoAssegnaInsert.Veg_Cod AndAlso
                               CInt(obj_r("Cul_Cod")) = lottoAssegnaInsert.Cul_Cod AndAlso
                               CInt(obj_r("Reg_Cod")) = lottoAssegnaInsert.Reg_Cod Then

                                Righe_Trovate += 1
                            End If

                        Next

                        If Righe_Trovate > 1 Then
                            MessaggioErrore += "Esistono altre righe con  " & obj("Preparazione_Des").ToString() & " " & obj("Veg_Des").ToString() & " " &
                                obj("Cul_Des").ToString() & " " & obj("Reg_Des").ToString()
                        End If

                    End If

                    EFArrayToInsert.Add(lottoAssegnaInsert)
                End If
            Next

            For Each obj As JObject In righeCancellateArray
                keys = CStr(obj("key_lotto_assegna")).Split("_")
                Dim dataInizio = keys(5).Substring(0, 2) & "/" & keys(5).Substring(2, 2) & "/" & keys(5).Substring(4, 4)
                Dim dataFine = keys(6).Substring(0, 2) & "/" & keys(6).Substring(2, 2) & "/" & keys(6).Substring(4, 4)
                lottoAssegna = lotto_ass_R.Leggi_Elem_Lotto_Assegna(piva, keys(0), keys(1), keys(2), keys(3), keys(4), CDate(dataInizio), CDate(dataFine), keys(7), keys(8), CStr(obj("Tipo_Lotto")), objParametri)
                If lottoAssegna Is Nothing Then
                    chiaveDescrittiva = CStr(obj("Rag_Soc")) & " " &
                                        CStr(obj("Veg_Des")) & " " &
                                        CStr(obj("Cul_Des")) & " " &
                                        CStr(obj("qualita_des")) & " " &
                                        CStr(obj("certif_des")) & " " &
                                        CDate(obj("validita_inizio")).ToShortDateString & " " &
                                        CDate(obj("validita_fine")).ToShortDateString & " " &
                                        CStr(obj("Preparazione_Des")) & " " &
                                        CStr(obj("Reg_Des"))
                    MessaggioErrore += "Riga da cancellare non trovata: " & chiaveDescrittiva
                Else
                    EFArrayToDelete.Add(lottoAssegna)
                End If
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim lotto_assegna_W As New Lotto_AssegnaxRisumSpeVarQualCert_DAL_W

                MessaggioErrore = lotto_assegna_W.Aggiorna_Lotto_Assegna(piva, EFArrayToInsert,
                    EFArrayToDelete, objParametri)

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Private Function CheckLottoAssegna(
            ByVal lottoAssegnaInsert As Lotto_AssegnaxRisumSpeVarQualCert,
            ByVal lottoAssegnaOld As Lotto_AssegnaxRisumSpeVarQualCert,
             ByVal Rag_Soc As String,
            ByVal Veg_Des As String,
            ByVal Cul_Des As String,
            ByVal qualita_des As String,
            ByVal certif_des As String,
            ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim MessaggioErrore As String = String.Empty
        Dim lotto_assegna_R As New Lotto_AssegnaxRisumSpeVarQualCert_DAL_R

        Dim chiaveDescrittiva = Rag_Soc & " " &
                                        Veg_Des & " " &
                                        Cul_Des & " " &
                                        qualita_des & " " &
                                        certif_des & " " &
                                        lottoAssegnaInsert.Validita_Inizio.ToShortDateString & " " &
                                        lottoAssegnaInsert.Validita_Fine.ToShortDateString

        Dim NomeRoutine As String = "AnagrafeBIZ.Lotto_AssegnaxRisumSpeVarQualCert_BIZ.CheckLottoAssegna()"

        Try

            'Controlli congruenza sulle date da <= a
            If lottoAssegnaInsert.Validita_Inizio > lottoAssegnaInsert.Validita_Fine Then

                MessaggioErrore = chiaveDescrittiva & ": Valido Da deve essere minore di Valido fino a <br/>"

            End If

            'Controllo che non vi siano altre testate con periodi sovrapposti
            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim myQueryCount As Integer =
                    lotto_assegna_R.Controlla_Date_LottoAssegna(lottoAssegnaInsert, lottoAssegnaOld, objParametri)
                If myQueryCount > 0 Then
                    MessaggioErrore &= "Esistono altre righe con periodi sovrapposti alla riga " & chiaveDescrittiva
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        End Try

        'If Not String.IsNullOrEmpty(MessaggioErrore) Then
        'End If

        Return MessaggioErrore
    End Function

    Public Function Impostazione_LottoLavorazioni(ByVal piva As String,
                                                   ByVal modulo_generazione As Integer,
                                                   ByVal mat_cod As Integer,
                                                   ByVal cod_risum As Integer,
                                                   ByVal data_ingresso As DateTime,
                                                   ByVal qualita_cod As Integer,
                                                   ByVal calibro_cod As Integer,
                                                   ByVal destinazioni_cod As String,
                                                   ByVal contatore_univoco_parametri As String,
                                                   ByVal preparazione_cod As Integer,
                                                   ByVal lotto_entrata As String,
                                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                                   ByRef ErrCode As Integer,
                                                   Optional ByVal lotto_testata As String = Nothing,
                                                   Optional ByRef configurazioneLotto As String = Nothing
                                                   ) As String

        Const nomeRoutine = "AnagrafeBIZ.Lotto_AssegnaxRisumSpeVarQualCert_BIZ.Impostazione_LottoLavorazioni()"
        Dim messaggioErrore As String = String.Empty
        Dim prefissoErrLotto = "Lotto non assegnabile: "

        Dim DT_Prodotto_Log As DataTable
        Dim DT_Materie_Prime As DataTable
        Dim DT_Linea_Log As DataTable
        Dim DT_Contatto As DataTable
        Dim DT_Assegna As DataTable
        Dim DT_OTabelle As DataTable = Nothing


        'Dim leggi_core As Object
        'Dim leggi_core2 As Object


        Dim Lotto As String = ""
        Dim Lotto_Configurazione As String = ""
        Dim Lotto_Separatore As String = ""
        Dim contatore_univoco_carico As Integer
        Dim Arrayp As String()
        'Dim Arrays As String()
        Dim i As Integer
        'Dim j As Integer

        Dim bOk As Boolean

        'Dim Sigla As String
        Dim Cod_Articolo As String = ""
        Dim Linea_Cod As Long = 0
        Dim Linea_Cod_Des As String = ""
        Dim Giorno_Giuliano As Integer
        Dim Numero_Settimana As Integer
        Dim Veg_Cod_Prodotto As Long = 0
        Dim Cul_Cod_Prodotto As Long = 0
        Dim Reg_Cod_Prodotto As Long = 0
        Dim Filtro_Aggiuntivo As String = ""
        Dim Errore As String = ""
        Dim Risultato As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Try

            Dim leggi_Omni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
            Dim leggi_LottoxRisum As New AgronicaCoreAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_R
            Dim leggi_Materie_LC As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
            Dim leggi_Materie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim leggi_Otabelle As New AgronicaCoreAnagrafeDAL.OTabelle_R

            'Lettura Prodotto x Vedere se è referenza
            DT_Materie_Prime = leggi_Materie_Prime.Leggi2(piva, 0, mat_cod, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            If DT_Materie_Prime.Rows.Count > 0 Then

                Cod_Articolo = DT_Materie_Prime.Rows(0).Item("Cod_Articolo")

                Veg_Cod_Prodotto = DT_Materie_Prime.Rows(0).Item("Veg_Cod")
                Cul_Cod_Prodotto = DT_Materie_Prime.Rows(0).Item("Cul_Cod")
                Reg_Cod_Prodotto = DT_Materie_Prime.Rows(0).Item("Regolamento")

                If DT_Materie_Prime.Rows(0).Item("Mat_Cod_Referenza") <> 0 Then

                    'Impostazione della referenza sul log omni
                    mat_cod = DT_Materie_Prime.Rows(0).Item("Mat_Cod_Referenza")

                End If

            End If

            'Lettura del modulo_generazione se non è già valorizzato
            If modulo_generazione = 0 Then

                Dim xFiltroAggiuntivo = "Mat_Cod = " & mat_cod

                Dim xOrderBy = "Id_Generazione desc"

                Dim dtOgenerazioni_Anagrafe_Log = leggi_Omni.Leggi(piva, 0, 0, 0, 0, 0, AGRODATAINIZIO,
                                                                                     AGRODATAFINE, xFiltroAggiuntivo,
                                                                                     xOrderBy, objParametri_Server)

                If dtOgenerazioni_Anagrafe_Log.Rows.Count > 0 Then
                    modulo_generazione = CInt(dtOgenerazioni_Anagrafe_Log.Rows(0)("Modulo_Generazione").ToString())
                End If

            End If

            'Lettura Log Prodotto
            DT_Prodotto_Log = leggi_Omni.LeggiOmniLog_Prodotto(piva, modulo_generazione, 0, 0, 0, 0, mat_cod, "", objParametri_Server)

            'Lettura Log Prodotto x Linea
            DT_Linea_Log = leggi_Omni.Leggi_Linee_Produzione_Log(piva, modulo_generazione, 0, 0, 0, 0, mat_cod, "", objParametri_Server)


            If DT_Linea_Log.Rows.Count > 0 AndAlso DT_Prodotto_Log.Rows.Count > 0 Then

                Linea_Cod = DT_Linea_Log.Rows(0).Item("Linea_Cod")
                Linea_Cod_Des = DT_Linea_Log.Rows(0).Item("Linea_Cod_Des")


                'Cod_Articolo = DT_Prodotto_Log.Rows(0).Item("Cod_Articolo")

            End If


            'Cerco la Configurazione del Lotto e il Separatore
            Dim xFiltro_Aggiuntivo = "Tipo_Lotto = 'L' " &
                                      "AND Preparazione_Cod = " & preparazione_cod

            DT_Assegna = leggi_LottoxRisum.Leggi_Lotto_AssegnaxRisumSpeVarQualCert(piva, xFiltro_Aggiuntivo, objParametri_Server)

            If DT_Assegna.Rows.Count > 0 Then

                Dim TrovatoVeg_Cod = DT_Assegna.Select("Veg_Cod = " & Veg_Cod_Prodotto)

                If TrovatoVeg_Cod.Length > 0 Then

                    Dim DT_Assegna_FiltratoXVeg_Cod = TrovatoVeg_Cod.CopyToDataTable()

                    Dim TrovatoCul_Cod = DT_Assegna_FiltratoXVeg_Cod.Select("Cul_Cod = " & Cul_Cod_Prodotto)

                    If TrovatoCul_Cod.Length > 0 Then

                        Dim DT_Assegna_FiltratoXCul_Cod = TrovatoCul_Cod.CopyToDataTable()

                        Dim TrovatoReg_Cod = DT_Assegna_FiltratoXCul_Cod.Select("Reg_Cod = " & Reg_Cod_Prodotto)

                        If TrovatoReg_Cod.Length = 1 Then

                            Lotto_Configurazione = TrovatoReg_Cod(0)("Algoritmo_Config")
                            Lotto_Separatore = TrovatoReg_Cod(0)("Separatore_Config")

                        Else

                            Trova_Reg_Cod_Generale_LottoLavorazioni(DT_Assegna, Veg_Cod_Prodotto, Cul_Cod_Prodotto, Lotto_Configurazione, Lotto_Separatore)

                            If Lotto_Configurazione = "" Then

                                Trova_Cul_Cod_Generale_LottoLavorazioni(DT_Assegna, Veg_Cod_Prodotto, Lotto_Configurazione, Lotto_Separatore)

                                If Lotto_Configurazione = "" Then
                                    Trova_Veg_Cod_Generale_LottoLavorazioni(DT_Assegna, Lotto_Configurazione, Lotto_Separatore)
                                End If

                            End If

                        End If
                    Else

                        Trova_Cul_Cod_Generale_LottoLavorazioni(DT_Assegna, Veg_Cod_Prodotto, Lotto_Configurazione, Lotto_Separatore)

                        If Lotto_Configurazione = "" Then
                            Trova_Veg_Cod_Generale_LottoLavorazioni(DT_Assegna, Lotto_Configurazione, Lotto_Separatore)
                        End If

                    End If

                Else

                    Trova_Veg_Cod_Generale_LottoLavorazioni(DT_Assegna, Lotto_Configurazione, Lotto_Separatore)

                End If

            End If

            configurazioneLotto = Lotto_Configurazione

            If Trim(Lotto_Configurazione) <> "" Then

                '==============================================================================================
                'Controllo Configurazione
                '----------------------------------------------------------------------------------------------
                Arrayp = Split(Lotto_Configurazione & "|", "|")

                If Array.FindIndex(Arrayp, Function(s) s <> "" AndAlso s <> "NULL" AndAlso (
                                               CInt(s) = enLotto_Config_Lavorazioni_FF.lcffProd_CALIBRO OrElse
                                               CInt(s) = enLotto_Config_Lavorazioni_FF.lcffProd_QUALITA
                                           )) <> -1 Then
                    'Lettura OTabelle
                    DT_OTabelle = leggi_Otabelle.LeggiParametri(piva, 0, modulo_generazione.ToString(), "", objParametri_Server)

                End If

                For i = 0 To UBound(Arrayp, 1) - 1

                    If IsNumeric(Arrayp(i)) Then

                        Select Case CInt(Arrayp(i))

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_FORNITORE

                                bOk = False

                                If cod_risum <> 0 Then

                                    'Lettura Contatto
                                    Dim leggi_contatto As New AgronicaCoreAnagrafeDAL.Contatti_R

                                    DT_Contatto = leggi_contatto.Leggi(piva, "", cod_risum, 0, True, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametri_Server)


                                    If DT_Contatto.Rows.Count > 0 Then

                                        If Trim(Lotto) = "" Then
                                            Lotto = DT_Contatto.Rows(0).Item("Settore_Des")
                                        Else
                                            Lotto = Lotto & Lotto_Separatore & DT_Contatto.Rows(0).Item("Settore_Des")
                                        End If

                                        bOk = True

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = prefissoErrLotto & " Fornitore non impostato correttamente."

                                End If


                            Case enLotto_Config_Lavorazioni_FF.lcffProd_DATA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = Strings.Format(CDate(data_ingresso), "yMMdd")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Strings.Format(CDate(data_ingresso), "yMMdd")
                                    End If

                                Else

                                    Errore = prefissoErrLotto & "Data ingresso non impostata correttamente."

                                End If




                            Case enLotto_Config_Lavorazioni_FF.lcffProd_ANNO_AA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = Strings.Format(CDate(data_ingresso), "yy")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Strings.Format(CDate(data_ingresso), "yy")
                                    End If

                                Else

                                    Errore = prefissoErrLotto & "Data ingresso non impostata correttamente."

                                End If



                            Case enLotto_Config_Lavorazioni_FF.lcffProd_NUMERO_SETTIMANA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    ' Numero_Settimana = Weekday(CDate(data_ingresso))

                                    Dim dfi = Globalization.DateTimeFormatInfo.CurrentInfo
                                    Numero_Settimana = Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, DayOfWeek.Monday)

                                    If Trim(Lotto) = "" Then
                                        Lotto = Strings.Format(Numero_Settimana, "00")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Strings.Format(Numero_Settimana, "00")
                                    End If

                                Else

                                    Errore = prefissoErrLotto & "Data ingresso non impostata correttamente."

                                End If



                            Case enLotto_Config_Lavorazioni_FF.lcffProd_GIORNO_GIULIANO

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    Giorno_Giuliano = DateDiff("d", "01/01/" & Year(CDate(data_ingresso)), CDate(data_ingresso)) + 1

                                    If Trim(Lotto) = "" Then
                                        Lotto = Strings.Format(Giorno_Giuliano, "000")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Strings.Format(Giorno_Giuliano, "000")
                                    End If

                                Else

                                    Errore = prefissoErrLotto & "Data ingresso non impostata correttamente."

                                End If


                            'Per ora Commentato perché non c'è un identificativo univoco per le Celle
                            'Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_DESTINAZIONE

                            '    Arrays = Split(destinazioni_cod, "|")

                            '    For j = 0 To UBound(Arrays) - 1

                            '        If Trim(Lotto) = "" Then
                            '            Lotto = Arrays(j)
                            '        Else
                            '            Lotto = Lotto & Lotto_Separatore & Arrays(j)
                            '        End If

                            '    Next


                            'Per ora Commentato
                            'Case enLotto_Config_Lavorazioni_FF.lcffProd_SIGLA_SPECIE_VARIETA

                            '    bOk = False

                            '    If DT_Prodotto_Log.Rows.Count <> 0 Then

                            '        Veg_Cod_Prodotto = DT_Prodotto_Log.Rows(0).Item("Veg_Cod")
                            '        Cul_Cod_Prodotto = DT_Prodotto_Log.Rows(0).Item("Cul_Cod")
                            '        Reg_Cod_Prodotto = DT_Prodotto_Log.Rows(0).Item("Regolamento")

                            '    End If

                            '    leggi_core = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                            '    leggi_core2 = New AgronicaCoreMetaSchemaDAL.Cultivar_R

                            '    Sigla = UCase(Trim(Left(leggi_core.VegDes_from_VegCod(Veg_Cod_Prodotto, objParametri_Server), 1))) &
                            '                UCase(Trim(Left(leggi_core2.CulDes_from_CulCod(Cul_Cod_Prodotto, objParametri_Server), 1)))

                            '    If Trim(Lotto) = "" Then
                            '        Lotto = Sigla
                            '    Else
                            '        Lotto = Lotto & Lotto_Separatore & Sigla
                            '    End If

                            '    bOk = True


                            '    'If Not bOk Then

                            '    '        Errore = prefissoErrLotto & "Specie vegetale/varietà colturale non impostata correttamente."

                            '    '    End If


                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CONTATORE_UNIVOCO

                                'Creazione Contatore Univoco
                                Dim sequenza = New Sequenza_Progressivi_R
                                contatore_univoco_carico = sequenza.Nuovo_Progressivo_UpdateImmediato(piva, Year(data_ingresso), 19, "", "", 0, objParametri_Server)

                                If Trim(Lotto) = "" Then
                                    Lotto = Strings.Format(contatore_univoco_carico, "000000")
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Strings.Format(contatore_univoco_carico, "000000")
                                End If


                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_LINEA


                                If Trim(Lotto) = "" Then
                                    Lotto = Linea_Cod_Des
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Linea_Cod_Des
                                End If



                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_ARTICOLO


                                If Trim(Lotto) = "" Then
                                    Lotto = Cod_Articolo
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Cod_Articolo
                                End If

                            'Per ora commentato
                            'Case enLotto_Config_Lavorazioni_FF.lcffProd_CONTATORE_UNIVOCO_PARAMETRI


                            '    If Trim(Lotto) = "" Then
                            '        Lotto = contatore_univoco_parametri
                            '    Else
                            '        Lotto = Lotto & Lotto_Separatore & contatore_univoco_parametri
                            '    End If

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_ENTRATA

                                bOk = False

                                If lotto_entrata <> "" Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = lotto_entrata
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & lotto_entrata
                                    End If

                                    bOk = True

                                End If

                                If Not bOk Then

                                    Errore = prefissoErrLotto & "Non sono presenti lotti in entrata oppure sono presenti più lotti diversi."

                                End If

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CALIBRO

                                bOk = False

                                If calibro_cod <> 0 AndAlso
                                   DT_OTabelle IsNot Nothing AndAlso DT_OTabelle.Rows.Count > 0 Then

                                    Dim Sigla_Trovata = DT_OTabelle.Select("Tabella_Par_Cod = " & calibro_cod & "And Tabella_Cod = " & enum_OTabelle.Calibro)

                                    If Sigla_Trovata.Length = 1 Then

                                        If Trim(Lotto) = "" Then
                                            Lotto = Sigla_Trovata(0)("Sigla")
                                        Else
                                            Lotto = Lotto & Lotto_Separatore & Sigla_Trovata(0)("Sigla")
                                        End If

                                        bOk = True

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = prefissoErrLotto & "Calibro non impostato correttamente."

                                End If

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_QUALITA

                                bOk = False

                                If qualita_cod <> 0 AndAlso DT_OTabelle.Rows.Count > 0 Then

                                    Dim Sigla_Trovata = DT_OTabelle.Select("Tabella_Par_Cod = " & qualita_cod & "And Tabella_Cod = " & enum_OTabelle.Qualita)

                                    If Sigla_Trovata.Length = 1 Then

                                        If Trim(Lotto) = "" Then
                                            Lotto = Sigla_Trovata(0)("Sigla")
                                        Else
                                            Lotto = Lotto & Lotto_Separatore & Sigla_Trovata(0)("Sigla")
                                        End If

                                        bOk = True

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = prefissoErrLotto & "Qualità non impostata correttamente."

                                End If

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_TESTATA

                                bOk = False

                                If lotto_testata <> "" Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = lotto_testata
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & lotto_testata
                                    End If

                                    bOk = True

                                End If

                                If Not bOk Then

                                    Errore = prefissoErrLotto & "Non sono presenti lotti di testata oppure sono presenti più lotti diversi."

                                End If

                        End Select

                    End If

                Next i



                '============================================================================================================
                'Controllo impostazione nulla e creazione di un lotto univoco (data e ora)
                '------------------------------------------------------------------------------------------------------------
                If Trim(Errore) <> "" Then

                    Risultato = Errore
                    ErrCode = 1

                Else

                    If Trim(Lotto) = "" Then

                        'Lotto non Impostato --> ritorno data
                        Lotto = Strings.Format(Now, "yyMMdd")

                    End If


                    Risultato = Lotto
                    ErrCode = 0


                End If



            Else

                Risultato = "Lotto Configurazione Non Impostato Correttamente o Prodotto Mancante."
                ErrCode = 2

            End If


            Return Risultato

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function

    Private Sub Trova_Veg_Cod_Generale_LottoLavorazioni(ByVal DT As DataTable, ByRef Lotto_Configurazione As String, ByRef Lotto_Separatore As String)

        Dim Trovato_Veg_Cod_Generale = DT.Select("Veg_Cod = -1")

        If Trovato_Veg_Cod_Generale IsNot Nothing AndAlso Trovato_Veg_Cod_Generale.Length = 1 Then
            Lotto_Configurazione = Trovato_Veg_Cod_Generale(0)("Algoritmo_Config")
            Lotto_Separatore = Trovato_Veg_Cod_Generale(0)("Separatore_Config")
        End If

    End Sub

    Private Sub Trova_Cul_Cod_Generale_LottoLavorazioni(ByVal DT As DataTable, ByVal Veg_Cod As Integer, ByRef Lotto_Configurazione As String, ByRef Lotto_Separatore As String)

        Dim Trovato_Cul_Cod_Generale = DT.Select("Veg_Cod = " & Veg_Cod & " And Cul_Cod = 0")

        If Trovato_Cul_Cod_Generale IsNot Nothing AndAlso Trovato_Cul_Cod_Generale.Length = 1 Then
            Lotto_Configurazione = Trovato_Cul_Cod_Generale(0)("Algoritmo_Config")
            Lotto_Separatore = Trovato_Cul_Cod_Generale(0)("Separatore_Config")
        End If

    End Sub

    Private Sub Trova_Reg_Cod_Generale_LottoLavorazioni(ByVal DT As DataTable, ByVal Veg_Cod As Integer, ByVal Cul_Cod As Integer, ByRef Lotto_Configurazione As String, ByRef Lotto_Separatore As String)

        Dim Trovato_Reg_Cod_Generale = DT.Select("Veg_Cod = " & Veg_Cod & " And Cul_Cod = " & Cul_Cod & " And Reg_Cod = 0")

        If Trovato_Reg_Cod_Generale IsNot Nothing AndAlso Trovato_Reg_Cod_Generale.Length = 1 Then
            Lotto_Configurazione = Trovato_Reg_Cod_Generale(0)("Algoritmo_Config")
            Lotto_Separatore = Trovato_Reg_Cod_Generale(0)("Separatore_Config")
        End If

    End Sub

    Public Function SeSostituisciLottoOrigine(ByVal lottoOrigine As String,
                                              ByVal lottoTestata As String,
                                              ByVal configurazioneLotto As String,
                                              ByRef lottoFinale As String,
                                              ByRef codiceErrore As Integer
                                              ) As Boolean

        'Se lotto origine valorizzato, lo sostituisco solo se è uguale a quello di testata e
        'nella configurazione lotto è previsto l'utilizzo del lotto di testata
        Dim sostituisciLottoOrigine As Boolean = False
        If Not String.IsNullOrEmpty(lottoOrigine) AndAlso lottoOrigine = lottoTestata Then
            If Not String.IsNullOrEmpty(configurazioneLotto) Then
                Dim elencoCfgLotto As String() = configurazioneLotto.Split("|")
                Dim paramLottoTestata As String = enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_TESTATA
                Dim elencoLottoTestata As String() = Filter(elencoCfgLotto, paramLottoTestata)
                If elencoLottoTestata.Length > 0 Then
                    sostituisciLottoOrigine = True
                End If
            End If
        End If

        'Se lotto origine valorizzato ma non lo devo sostituire, lo ripasso in uscita ignorando eventuali errori
        If Not sostituisciLottoOrigine AndAlso Not String.IsNullOrEmpty(lottoOrigine) Then
            lottoFinale = lottoOrigine
            codiceErrore = 0
        End If

        Return True
    End Function

End Class
