Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class BrogliaccioMovimentiTabella
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Shared Function CaricaKendoBrogliaccioMovimenti(ByVal qs_piva As String, ByVal dataDa As String, ByVal dataA As String, ByVal mese As Integer, ByVal anno As Integer) As RispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try
            Dim piva = Stringa_Decodifica(qs_piva, AgroKey_EncoderDecoder)
            Dim objCTerzi As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim Esiste_GestioneContoTerzi As Boolean

            Esiste_GestioneContoTerzi = objCTerzi.Esiste_GestioneContoTerzi_RegCantina( _
                                                    piva, _
                                                    0, _
                                                    objParametri_Server)
            Dim conto_Terzi As TipiEnumerativi.enum_RegistroContoTerzi
            If Esiste_GestioneContoTerzi = False Then
                conto_Terzi = TipiEnumerativi.enum_RegistroContoTerzi.Nessuno
            Else
                conto_Terzi = TipiEnumerativi.enum_RegistroContoTerzi.RegistroUnicoDiversificato
            End If


            Dim regCantina As New AgronicaCoreStampeDAL.RegistriCantina
            Dim ValiditaInizio As Date
            Dim ValiditaFine As Date
            If dataDa = "" And dataA = "" Then
                ValiditaInizio = New Date(anno, mese, 1)
                Dim dataFine As Date
                If mese <> 12 Then
                    dataFine = New Date(anno, mese + 1, 1)
                Else
                    dataFine = New Date(anno + 1, 1, 1)
                End If
                ValiditaFine = dataFine.AddDays(-1)
            Else
                ValiditaInizio = CDate(dataDa)
                ValiditaFine = CDate(dataA)
            End If
            Dim dt = regCantina.BrogliaccioMovimenti(piva, ValiditaInizio, ValiditaFine, "", "", 0, 0, 0, 0, 0, "", conto_Terzi, "", "", "", objParametri_Server)

            Dim res = CaricaGriglia_BrogliaccioMovimenti_xJSON(dt)

            r.RispostaOK = True
            r.RispostaStringa = res

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Stringa_Decodifica(ByVal Testo As String, _
                                       ByVal Chiave As String, _
                                       Optional ByRef objServer As Object = Nothing) _
                                       As String

        If Testo = "" Then

            Return ""
            Exit Function

        End If
        Testo = Agronica_Url_Decode(Testo)

        Dim Cript As Boolean = False

        Dim strEncrypted As String
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next


        strEncrypted = Replace(strEncrypted, "@", "\")

        Return strEncrypted


    End Function

    Private Shared Function Agronica_Url_Decode( _
                            ByVal StrInput As String, _
                            Optional ByVal Separatore As String = "G") _
                            As String

        Dim i As Integer
        Dim Carattere As String
        Dim Cod_Ascii_16 As Integer
        Dim Cod_Hex As String
        Dim StrOutput As String
        Dim Vettore() As String

        'Inizializzo
        StrOutput = ""

        'Recupero gli elementi
        Vettore = Split(StrInput, Separatore)

        For i = LBound(Vettore) To UBound(Vettore)

            Cod_Hex = Vettore(i)

            Cod_Ascii_16 = Val("&H" & Cod_Hex)

            Carattere = ChrW(Cod_Ascii_16)

            StrOutput = StrOutput & Carattere

        Next i

        Return StrOutput

    End Function

    Public Shared Function CaricaGriglia_BrogliaccioMovimenti_xJSON(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome



        'c = New ColonneNome("chiave_composita", "kendoKey", "string")
        ''c._hidden = True
        'l.Add(c)

        c = New ColonneNome("Data_Movimento", "Data Movimento", "date")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Ora", "Ora", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("DataOra", "DataOra", "date")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("NDoc", "N. Doc", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Agenda", "Cod. Op.", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("des_lib", "Descrizione Movimento", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("lav_cod", "Cod. Tipo Op.", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("lav_des", "Tipo Operazione", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Preparazione_Des", "Tipo Operazione Cantina", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Linea_Des", "Linea di Produzione", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Lotto_Linea", "Lotto Linea", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Azienda", "Azienda-C/Lavoro", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("StrCampo_Registri", "Dettaglio", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("desc_agg", "Dettaglio2", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cau_Mov", "Cau_Mov", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Elem_Cod", "Elem_Cod", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Mat_Des", "Prodotto", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Lotto", "Lotto", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cod_Articolo", "Cod. Articolo", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Cod", "Udm_Cod", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Sim", "Udm", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("qta", "qta", "numeric")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Peso_Set", "Peso_Set", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("capacita", "Tipo Capacità", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("udm_cod_extra", "udm_cod_extra", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Sim_Extra", "udm Capacità", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("QTA_EXTRA", "Capacità", "numeric")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Destinazione", "Tipo_Destinazione", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("sa_nome", "Centro Az.", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("SaCod_Dest", "SaCod_Dest", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Dest", "Id_Dest", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Integrazione", "Tipo_Integrazione", "numeric")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Num_Vasca", "Vasca", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Magazzino", "Magazzino", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("CaricoKg", "Carico Kg", "numeric")
        'c._hidden = True
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("ScaricoKg", "Scarico Kg", "numeric")
        'c._hidden = True
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("CaricoLt", "Carico Lt", "numeric")
        'c._hidden = True
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("ScaricoLt", "Scarico Lt", "numeric")
        'c._hidden = True
        c._Filtrabile = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, True, True, TipoFiltroKendo_colonne.CasellaTesto) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp
    End Function

End Class