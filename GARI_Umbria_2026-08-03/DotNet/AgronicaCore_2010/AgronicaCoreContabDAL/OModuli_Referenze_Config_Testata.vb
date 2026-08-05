Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider
Imports System.Text
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity.Core.Metadata.Edm
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports AgronicaCoreDTOStd.Identity

Public Class OModuli_Referenze_Config_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    '28/04/2016
    'N.B.: questa funzione non viene più utilizzata da St. Fatture/Bolle : 
    'Al posto del campo OModuli_Referenze_Config_Testata.Tabella_Cod_Base viene utilizzato Materie_Prime.Tabella_Cod_Base
    Public Function Recupera_Tabella_Cod_Base(ByVal Modulo_Generazione As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R.Recupera_Tabella_Cod_Base()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Tabella_Cod_Base As Integer

        Try

            DT = Leggi("", 0, Modulo_Generazione,
                      xFiltroAggiuntivo,
                      "",
                      objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Tabella_Cod_Base = DT.Rows(0).Item("Tabella_Cod_Base")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Tabella_Cod_Base

    End Function

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Modulo_Generazione As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  OModuli_Referenze_Config_Testata ")

            StrSQL.Append(" WHERE   1 = 1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Modulo_Generazione <> 0 Then
                StrSQL.Append(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi(ByVal Piva As String,
                          ByVal Modulo_Generazione As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  OModuli_Referenze_Config_Testata ")
            StrSQL.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Modulo_Generazione <> 0 Then
                StrSQL.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagliTestata(ByVal Piva As String,
                                         ByVal Id_Testata As Integer,
                                         ByVal Tabella_ID As String,
                                         ByVal Modulo_Generazione As Int32,
                                         ByVal Tipo As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R.LeggiDettagliTestata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  OModuli_Referenze_Config_Testata, OModuli_Referenze_Config_Dettagli ")
            StrSQL.AppendLine(" WHERE OModuli_Referenze_Config_Testata.Validita_Inizio <= " & AGRODATAFINE)
            StrSQL.AppendLine(" AND   OModuli_Referenze_Config_Testata.Validita_Fine >= " & AGRODATAINIZIO)

            StrSQL.AppendLine(" AND OModuli_Referenze_Config_Testata.Piva = OModuli_Referenze_Config_Dettagli.Piva ")
            StrSQL.AppendLine(" AND   OModuli_Referenze_Config_Testata.Id_Testata = OModuli_Referenze_Config_Dettagli.Id_Testata ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND OModuli_Referenze_Config_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Testata <> 0 Then
                StrSQL.AppendLine(" AND OModuli_Referenze_Config_Dettagli.Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If

            If Tabella_ID <> "" Then
                StrSQL.AppendLine(" AND OModuli_Referenze_Config_Dettagli.Tabella_ID = '" & Agro_SQL_SaveText(Tabella_ID) & "' ")
            End If

            If Modulo_Generazione <> 0 Then
                StrSQL.AppendLine(" AND OModuli_Referenze_Config_Testata.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            If Tipo <> 0 Then
                StrSQL.AppendLine(" AND OModuli_Referenze_Config_Dettagli.Tipo = " & Agro_SQL_SaveNum(Tipo) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY OModuli_Referenze_Config_Dettagli.Piva, Modulo_Generazione, OModuli_Referenze_Config_Dettagli.Id_Testata, Ordine ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiModuliConferimento(ByVal Piva As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R.LeggiModuliConferimento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim filtro As String = $"(Modulo_Generazione = {CInt(enum_Omni_Modulo_Generazione.FreshFood)} "
            filtro += $"OR Modulo_Generazione = {CInt(enum_Omni_Modulo_Generazione.Zoo)})"

            Dim orderBy = "Piva, Modulo_Generazione, Descrizione ASC"

            '--------------------------------------------------------------------------
            DT = Leggi(Piva, 0, AGRODATAINIZIO, AGRODATAFINE, filtro, orderBy, objParametri)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class OModuli_Referenze_Config_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Costruttori"

    Public Sub New()
        Provider = System.Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As System.Globalization.CultureInfo
    Public Shadows Property Provider() As System.Globalization.CultureInfo
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

    Public Function AggiornaRecordModificati(ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByVal tutteleRighe As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim esitoAggioramento As String = String.Empty
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_W.AggiornaRecordModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of OModuli_Referenze_Config_Testata)
            Dim EFArrayToUpdate As New List(Of OModuli_Referenze_Config_Testata)
            Dim EFArrayToDelete As New List(Of OModuli_Referenze_Config_Testata)

            Dim isValide As Boolean = VerificaTestateDoppie(tutteleRigheArray, MessaggioErrore)
            isValide = isValide AndAlso ImpostaRigheInserire(piva, righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheCancellate(piva, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = Scrivi(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
            Else
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esitoAggioramento = ex.Message
        Finally

        End Try

        Return esitoAggioramento
    End Function

    Private Function ImpostaRigheInserire(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OModuli_Referenze_Config_Testata),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaValida(obj)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim testata As New OModuli_Referenze_Config_Testata
                ImpostaTestataEF(piva, obj, testata, objParametri)
                testata.Id_Testata = 0
                testata.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                testata.Username_Creazione = objParametri.UsernameOperazione
                testata.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                testata.Username_Modifica = objParametri.UsernameOperazione
                testata.inviato = 0
                EFArray.Add(testata)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OModuli_Referenze_Config_Testata),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaValida(obj)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim testata As New OModuli_Referenze_Config_Testata
                ImpostaTestataEF(piva, obj, testata, objParametri)
                testata.Id_Testata = obj("Id_Testata")
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    testata.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                testata.Username_Creazione = obj("Username_Creazione").ToString
                testata.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                testata.Username_Modifica = objParametri.UsernameOperazione
                testata.inviato = obj("inviato")
                EFArray.Add(testata)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OModuli_Referenze_Config_Testata),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            Dim testata As New OModuli_Referenze_Config_Testata With {
                    .Piva = piva,
                    .Modulo_Generazione = obj("Modulo_Cod"),
                    .Id_Testata = obj("Id_Testata"),
                    .Descrizione = obj("Descrizione")
                }
            messaggioErrore = VerificaRigaCancellazioneValida(testata, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                EFArray.Add(testata)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function VerificaTestateDoppie(tutteleRigheArray As JArray, ByRef messaggioErrore As String) As Boolean
        Dim doppione = tutteleRigheArray _
            .GroupBy(Function(key) $"Modulo Generazione: {key("Modulo_Des")} - Descrizione: {key("Descrizione")}",
                    Function(gr) gr,
                    Function(k, g) New With
                    {
                        .chiave = k,
                        .TotaleIstanze = g.Count()
                    }) _
            .Where(Function(x) x.TotaleIstanze > 1) _
            .ToList()

        If doppione.Any() Then
            messaggioErrore = $"I seguenti gruppi di referenze risultano doppi:{String.Join("<br />", doppione.Select(Function(x) x.chiave))}"
        End If

        Return (Not doppione.Any())
    End Function

    Private Function VerificaRigaValida(obj As JObject) As String
        Return IsFiltroSpecieVarietaValido(obj("OFiltro_Veg_Cod"), obj("OFiltro_Cul_Cod"))
    End Function

    Private Function VerificaRigaCancellazioneValida(testata As OModuli_Referenze_Config_Testata, ByRef objParametri As AgronicaCoreParametri) As String
        Return IsConDettaglio(testata, objParametri)
    End Function

    Private Sub ImpostaTestataEF(piva As String, obj As JObject, ByRef testata As OModuli_Referenze_Config_Testata, objParametri As AgronicaCoreParametri)
        Dim inizio As DateTime = DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
        Dim fine As DateTime = DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing)

        Dim OFiltro_Veg_Cod As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Veg_Cod")))
        Dim OFiltro_Cul_Cod As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Cul_Cod")))

        testata.Piva = piva
        testata.Modulo_Generazione = obj("Modulo_Cod")
        testata.Descrizione = obj("Descrizione")
        testata.OFiltro_Veg_Cod = OFiltro_Veg_Cod
        testata.OFiltro_Cul_Cod = OFiltro_Cul_Cod
        testata.Tabella_Cod_Base = obj("Tabella_Cod_Base")

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            testata.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            testata.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If

    End Sub

    Private Function IsFiltroSpecieVarietaValido(OFiltro_Veg_Cod As JToken, OFiltro_Cul_Cod As JToken) As String
        Dim result As String = String.Empty
        Dim speci As Integer() = ConvertIntegerArray(OFiltro_Veg_Cod)
        Dim varieta As Integer() = ConvertIntegerArray(OFiltro_Cul_Cod)

        Dim countSpeci As Integer = speci.Count()
        Dim countVarieta As Integer = varieta.Count()

        If countSpeci = 0 AndAlso Not countVarieta = 0 Then
            result = "Non è possibile impostare una varietà senza aver definito una specie"
        ElseIf countSpeci > 1 AndAlso Not countVarieta = 0 Then
            result = "Non è possibile impostare delle varietà per più speci"
        Else ' countSpeci = 1, nessun controllo su countVarieta perchè sempre valido
            result = String.Empty
        End If

        Return result
    End Function

    Private Function IsConDettaglio(testata As OModuli_Referenze_Config_Testata, ByRef objParametri As AgronicaCoreParametri) As String
        Dim result As String = String.Empty
        Dim leggiTestata As New OModuli_Referenze_Config_Testata_R
        Dim filtroTabellaID = " NOT OModuli_Referenze_Config_Dettagli.Tabella_ID IN ('C', 'F')"
        Dim dettagli As DataTable = leggiTestata.LeggiDettagliTestata(testata.Piva, testata.Id_Testata, "",
                                                                      testata.Modulo_Generazione, "1",
                                                                      filtroTabellaID, "", objParametri)
        If Not IsNothing(dettagli) AndAlso dettagli.Rows.Count > 1 Then
            result = $"Non è possibile cancellare il gruppo di referenze '{testata.Descrizione}' perchè referenziato da parametri qualitativi"
        End If
        Return result
    End Function

    '#########################################################################
    Private Function Scrivi(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of OModuli_Referenze_Config_Testata),
                           ByVal EFArrayToUpdate As List(Of OModuli_Referenze_Config_Testata),
                           ByVal EFArrayToDelete As List(Of OModuli_Referenze_Config_Testata),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim dettagli As New OModuli_Referenze_Config_Dettagli_W

        Try
            'Prova di scrittura

            'Scrittura in Entity Framework 
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)
                Dim transaction As DbContextTransaction = Nothing
                ' Contiene anche i dettagli
                Try
                    transaction = GiasContext.Database.BeginTransaction()
                    For Each listProdotti As OModuli_Referenze_Config_Testata In EFArrayToInsert
                        Dim idTestata As Integer = 0

                        Do
                            idTestata = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                                       "OModuli_Referenze_Config_Testata",
                                                                                       0,
                                                                                       AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                                       objParametri)
                        Loop While (idTestata < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.OModuli_Referenze_Config_Testata.Any(Function(x) x.Id_Testata = idTestata))
                        listProdotti.Id_Testata = idTestata
                        GiasContext.OModuli_Referenze_Config_Testata.Add(listProdotti)
                        GiasContext.OModuli_Referenze_Config_Dettagli.Add(ParametroFornitore(listProdotti))
                        GiasContext.OModuli_Referenze_Config_Dettagli.Add(ParametroNote(listProdotti))
                    Next

                    For Each listProdotti As OModuli_Referenze_Config_Testata In EFArrayToUpdate
                        GiasContext.OModuli_Referenze_Config_Testata.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As OModuli_Referenze_Config_Testata In EFArrayToDelete
                        Dim parametri As OModuli_Referenze_Config_Dettagli() = GiasContext.OModuli_Referenze_Config_Dettagli _
                            .Where(Function(x) x.Piva = listProdotti.Piva AndAlso
                                x.Id_Testata = listProdotti.Id_Testata) _
                            .ToArray()
                        GiasContext.OModuli_Referenze_Config_Dettagli.RemoveRange(parametri)
                        GiasContext.OModuli_Referenze_Config_Testata.Attach(listProdotti)
                        GiasContext.OModuli_Referenze_Config_Testata.Remove(listProdotti)
                    Next

                    GiasContext.SaveChanges()
                    'Gias Context.SaveChanges() è come se fosse una transazione se c'è un errore,
                    'nelle righe inserite,cancellate o modificate viene annullata tutta la scrittura
                    transaction.Commit()
                Catch ex As Exception
                    If IsNothing(transaction) Then
                        transaction.Rollback()
                    End If
                End Try
            End Using

            '---------------------------------------------

            '--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Private Function ParametroFornitore(testata As OModuli_Referenze_Config_Testata) As OModuli_Referenze_Config_Dettagli
        Dim result As OModuli_Referenze_Config_Dettagli = ParametroDettaglio(testata)
        result.Tipo = 1
        result.Tabella_ID = "F"
        result.Tabella_Key = "fornitore"
        result.Ordine = 101
        Return result
    End Function

    Private Function ParametroNote(testata As OModuli_Referenze_Config_Testata) As OModuli_Referenze_Config_Dettagli
        Dim result As OModuli_Referenze_Config_Dettagli = ParametroDettaglio(testata)
        result.Tipo = 2
        result.Tabella_ID = "onote"
        result.Tabella_Key = "note"
        result.Ordine = 102
        Return result
    End Function

    Private Function ParametroDettaglio(testata As OModuli_Referenze_Config_Testata) As OModuli_Referenze_Config_Dettagli
        Dim result As New OModuli_Referenze_Config_Dettagli
        result.Piva = testata.Piva
        result.Id_Testata = testata.Id_Testata
        result.Configurazione_Des = String.Empty
        result.ChkReferenza = 0
        result.ChkOmni_Invisibili = 0
        result.ChkEtichetta = 0
        result.ChkEdit = 0
        result.ChkObbligatorio = 0
        result.Tabella_Key_Rif = String.Empty
        result.Validita_Inizio = testata.Validita_Inizio
        result.Validita_Fine = testata.Validita_Fine
        result.Data_Creazione = testata.Data_Creazione
        result.Username_Creazione = testata.Username_Creazione
        result.Data_Modifica = testata.Data_Modifica
        result.Username_Modifica = testata.Username_Modifica
        result.inviato = testata.inviato
        Return result
    End Function

    Private Function ConvertIntegerArray(obj As JToken) As Integer()
        Dim result As Integer() = {}
        If Not IsNothing(obj) AndAlso Not String.IsNullOrEmpty(obj.ToString()) Then
            result = JArray.Parse(obj.ToString()).ToObject(Of List(Of Integer)).ToArray()
        End If
        Return result
    End Function

    Private Function ImpostaFiltroCod(array As Integer()) As String
        Dim result As String = String.Empty
        If Not IsNothing(array) AndAlso array.Any() Then
            result = $"|{String.Join("|", array)}|"
        End If
        Return result
    End Function

End Class