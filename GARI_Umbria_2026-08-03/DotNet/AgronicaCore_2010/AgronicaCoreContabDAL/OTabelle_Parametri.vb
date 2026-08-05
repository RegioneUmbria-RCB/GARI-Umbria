Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.anagrafiche
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity.Core.Metadata.Edm
Imports System.Data.Entity
Imports AgronicaCoreModelsSTD.attivita

Public Class OTabelle_Parametri_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiValoriParametriQualitativi(
                ByVal piva As String,
                ByVal Tabella_ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim TabellaID_Imballaggi As Integer?() = {4, 8, 5}

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiValoriParametriQualitativi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ValoriParamQual =
           From tabelleParametri In GiasContext.OTabelle_Parametri
           Join config_dettagli In GiasContext.OModuli_Referenze_Config_Dettagli On
                        CStr(tabelleParametri.Tabella_Cod) Equals config_dettagli.Tabella_ID
           Group Join mat_prime In GiasContext.Materie_Prime
           On tabelleParametri.Mat_Cod_Generazione_Link Equals mat_prime.Mat_Cod
                                            Into mat_prime_group = Group
           From _mat_prime_group In mat_prime_group.DefaultIfEmpty()
           Group Join ogen_anag_log In GiasContext.OGenerazioni_Anagrafe_Log.Where(Function(x) x.ChkScollegamento = 0)
           On tabelleParametri.Codice_Generazione_Link Equals ogen_anag_log.Codice_Generazione
                                            Into ogen_anag_log_group = Group
           From _ogen_anag_log_group In ogen_anag_log_group.DefaultIfEmpty()
           Where
                (tabelleParametri.Piva.Equals(piva) Or
                    (tabelleParametri.Piva.Equals("AAAAAAAAAAA") And config_dettagli.ChkOmni_Invisibili = 0)) And
                config_dettagli.Tipo = 1 And
                (tabelleParametri.Modulo_Generazione = 2) And
                (tabelleParametri.Tabella_Cod = Tabella_ID) And
                (tabelleParametri.ChkInvisibile = 0) And
                (Not TabellaID_Imballaggi.Contains(Tabella_ID) OrElse ((tabelleParametri.Mat_Cod_Generazione_Link <> 0) OrElse (Not _ogen_anag_log_group Is Nothing AndAlso _ogen_anag_log_group.Mat_Cod <> 0)))
           Order By
                tabelleParametri.Descrizione
           Select New With {
               .val_tabella_cod = tabelleParametri.Tabella_Cod,
               .val_cod = tabelleParametri.Tabella_Par_Cod,
               .val_sigla = tabelleParametri.Sigla,
               .val_des = tabelleParametri.Descrizione & If(_mat_prime_group.Cod_Articolo Is Nothing, "", " (" & _mat_prime_group.Cod_Articolo & ")"),
               .tara = If(_mat_prime_group Is Nothing, 0, _mat_prime_group.Tara),
               .elem_cod = If(_mat_prime_group Is Nothing, 0, _mat_prime_group.Elem_Cod),
               .mat_cod = If(tabelleParametri.Mat_Cod_Generazione_Link = 0, If(_ogen_anag_log_group Is Nothing OrElse _ogen_anag_log_group.Mat_Cod = 0, 0, _ogen_anag_log_group.Mat_Cod), tabelleParametri.Mat_Cod_Generazione_Link),
               .imballo_des = If(_mat_prime_group Is Nothing, "", _mat_prime_group.Mat_Des & If(_mat_prime_group.Cod_Articolo Is Nothing, "", " (" & _mat_prime_group.Cod_Articolo & ")")),
               .qta_extra = If(_mat_prime_group Is Nothing, 0, _mat_prime_group.Qta_Extra),
               .ofiltro_veg_cod = tabelleParametri.OFiltro_Veg_Cod,
               .ofiltro_cul_cod = tabelleParametri.OFiltro_Cul_Cod,
               .valore_min = tabelleParametri.Valore_Min,
               .valore_max = tabelleParametri.Valore_Max
           }

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            risposta = JsonConvert.SerializeObject(ValoriParamQual.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function LeggiParCodStatiAppezzamenti(appezzamenti As RifAppezzamento(), xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OTabelle_Parametri_R.LeggiParCodStatiAppezzamenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT otp.tabella_par_cod ")
            StrSQL.Append(" FROM AppezzamentixIndirizzi AxI ")
            StrSQL.Append(" JOIN Indirizzi I ")
            StrSQL.Append(" ON I.cod_indirizzo = AxI.cod_indirizzo ")
            StrSQL.Append(" JOIN otabelle_parametri otp ")
            StrSQL.Append(" ON otp.Codice_Origine = I.Stato ")
            StrSQL.Append($" WHERE AxI.Validita_inizio <= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)} ")
            StrSQL.Append($" AND AxI.Validita_Fine >= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)} ")

            If (appezzamenti.Length <> 0) Then
                StrSQL.Append(" AND ( ")
                StrSQL.Append(
                    Join(
                        appezzamenti.Select(
                            Function(appezza)
                                Return $" (AxI.PIVA = {appezza.partitaIva} AND AxI.sa_cod = {appezza.saCod} AND AxI.appezza = {appezza.appezza}) "
                            End Function
                        ).ToArray(),
                        " OR "
                    )
                )
                StrSQL.Append(" ) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append($" AND {xFiltroAggiuntivo}")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return DT
    End Function

    Public Function VerificaParametroQualitativoUtilizzato(tabellaCod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_Parametri_R.VerificaParametroQualitativoUtilizzato()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine("SELECT TOP(1) 1 ")
            strSql.AppendLine("FROM OTabelle t")
            strSql.AppendLine("JOIN Materie_Prime_Campionature m")
            strSql.AppendLine("ON t.Tabella_Cod_Des = m.Tipo")
            strSql.AppendLine($"WHERE t.Tabella_Cod = {tabellaCod}")
            strSql.AppendLine($"AND (")
            strSql.AppendLine($"    NOT m.Tipo_Cod = 0")
            strSql.AppendLine($"    OR")
            strSql.AppendLine($"    NOT m.Val_Cod = ''")
            strSql.AppendLine($")")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiJoinParametri(ByRef objParametri As AgronicaCoreParametri, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.OTabelle_Parametri_R.LeggiJoinParametri()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder

        Try
            strSql.AppendLine("SELECT * ")
            strSql.AppendLine("FROM OTabelle_Parametri ")
            strSql.AppendLine("LEFT JOIN OTabelle ")
            strSql.AppendLine("ON OTabelle_Parametri.Tabella_Cod = OTabelle.Tabella_Cod ")
            strSql.AppendLine("AND OTabelle_Parametri.Modulo_Generazione = OTabelle.Modulo_Generazione ")
            strSql.AppendLine("WHERE 1 = 1 ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class OTabelle_Parametri_W
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

    Public Function AggiornaRecordParametriModificati(ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByVal tutteleRighe As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim esitoAggioramento As String = String.Empty
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OTabelle_W.AggiornaRecordParametriModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of OTabelle_Parametri)
            Dim EFArrayToUpdate As New List(Of OTabelle_Parametri)
            Dim EFArrayToDelete As New List(Of OTabelle_Parametri)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(piva, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = Scrivi(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
            Else
                Throw New Exception(MessaggioErrore)
            End If


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        Return MessaggioErrore
    End Function

    Private Function ImpostaRigheParametriInserire(righeArray As JArray,
                                          EFArray As List(Of OTabelle_Parametri),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim parametro As New OTabelle_Parametri
                ImpostaTabellaParametroEF(obj, parametro, objParametri)
                parametro.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                parametro.Username_Creazione = objParametri.UsernameOperazione
                parametro.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                parametro.Username_Modifica = objParametri.UsernameOperazione
                parametro.inviato = 0
                EFArray.Add(parametro)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OTabelle_Parametri),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim parametro As New OTabelle_Parametri
                ImpostaTabellaParametroEF(obj, parametro, objParametri)
                parametro.Piva = piva
                parametro.Tabella_Par_Cod = obj("Tabella_Par_Cod")
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    parametro.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                parametro.Username_Creazione = obj("Username_Creazione").ToString
                parametro.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                parametro.Username_Modifica = objParametri.UsernameOperazione
                parametro.inviato = obj("inviato")
                EFArray.Add(parametro)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OTabelle_Parametri),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            Dim parametro As New OTabelle_Parametri With {
                .piva = piva,
                .Modulo_Generazione = obj("Modulo_Cod"),
                .Tabella_Cod = obj("Tabella_Cod"),
                .Tabella_Par_Cod = obj("Tabella_Par_Cod")
            }
            messaggioErrore = VerificaRigaParametroCancellazioneValida(parametro, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                EFArray.Add(parametro)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaParametroEF(obj As JObject, ByRef parametro As OTabelle_Parametri, objParametri As AgronicaCoreParametri)
        Dim inizio As DateTime = DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
        Dim fine As DateTime = DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing)
        Dim valoreMinimo As Integer = -2000000000
        Dim valoreMassimo As Integer = 2000000000

        Dim OFiltro_Veg_Cod As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Veg_Cod")))
        Dim OFiltro_Cul_Cod As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Cul_Cod")))
        Dim OFiltro_Colore As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Colore")))
        Dim OFiltro_Categoria As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Categoria")))
        Dim OFiltro_Classificazione As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Classificazione")))
        Dim OFiltro_Dicitura As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Dicitura")))
        Dim OFiltro_Caratteristica As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Caratteristica")))
        Dim OFiltro_Deno As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Deno")))
        Dim OFiltro_Finalita As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Finalita")))
        Dim OFiltro_Regolamento As String = ImpostaFiltroCod(ConvertIntegerArray(obj("OFiltro_Regolamento")))

        If IsNumeric(obj("Valore_Min")) Then
            valoreMinimo = obj("Valore_Min")
        End If

        If IsNumeric(obj("Valore_Max")) Then
            valoreMassimo = obj("Valore_Max")
        End If

        parametro.Piva = obj("Piva")
        parametro.Tabella_Cod = obj("Tabella_Cod")
        parametro.Tabella_Par_Cod = 0
        parametro.Modulo_Generazione = obj("Modulo_Cod")
        parametro.Descrizione = obj("Descrizione")
        parametro.Sigla = obj("Sigla")
        parametro.Codice_Origine = obj("Codice_Origine")
        parametro.ChkInvisibile = obj("ChkInvisibile")
        parametro.Valore_Min = valoreMinimo
        parametro.Valore_Max = valoreMassimo
        parametro.OFiltro_Veg_Cod = OFiltro_Veg_Cod
        parametro.OFiltro_Cul_Cod = OFiltro_Cul_Cod
        parametro.OFiltro_Colore = OFiltro_Colore
        parametro.OFiltro_Categoria = OFiltro_Categoria
        parametro.OFiltro_Classificazione = OFiltro_Classificazione
        parametro.OFiltro_Dicitura = OFiltro_Dicitura
        parametro.OFiltro_Caratteristica = OFiltro_Caratteristica
        parametro.OFiltro_Deno = OFiltro_Deno
        parametro.OFiltro_Finalita = OFiltro_Finalita
        parametro.OFiltro_Regolamento = OFiltro_Regolamento
        parametro.Codice_Generazione_Link = obj("Codice_Generazione_Link")
        parametro.Mat_Cod_Generazione_Link = obj("Mat_Cod_Generazione_Link")

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            parametro.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            parametro.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If
    End Sub

    Private Function VerificaRigaParametroValida(obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty

        IsRangeMaxMinValido(3, obj("Valore_Min"), obj("Valore_Max"), 0, messaggio)
        IsFiltroSpecieVarietaValido(obj("OFiltro_Veg_Cod"), obj("OFiltro_Cul_Cod"), messaggio)

        If IsNumeric(obj("Mat_Cod_Generazione_Link")) AndAlso CInt(obj("Mat_Cod_Generazione_Link")) > 0 Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "Il parametro è referenziato nelle 'Materie Prime' non è possibile cancellare"
        End If

        If IsNumeric(obj("Codice_Generazione_Link")) AndAlso CInt(obj("Codice_Generazione_Link")) > 0 Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "Il parametro è referenziato nelle 'Generazioni Anagrafiche' non è possibile cancellare"
        End If


        If Not String.IsNullOrEmpty(messaggio) Then
            messaggio = $"Per il parametro '{obj("Descrizione")}' son state riscontrate le seguenti segnalazioni:<br />{messaggio}"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaParametroCancellazioneValida(parametro As OTabelle_Parametri, ByRef objParametri As AgronicaCoreParametri) As String
        Dim result As String = String.Empty

        If parametro.Mat_Cod_Generazione_Link > 0 Then
            result += $"Il parametro '{parametro.Descrizione}' è referenziato nelle 'Materie Prime' non è possibile cancellare"
        End If

        If parametro.Codice_Generazione_Link.HasValue AndAlso parametro.Codice_Generazione_Link.Value > 0 Then
            If Not String.IsNullOrEmpty(result) Then
                result += "<br />"
            End If
            result += $"Il parametro '{parametro.Descrizione}' è referenziato nelle 'Generazioni Anagrafiche' non è possibile cancellare"
        End If

        Dim lettura As New OTabelle_Parametri_R
        Dim conReferenza = lettura.VerificaParametroQualitativoUtilizzato(parametro.Tabella_Cod, objParametri)

        If Not IsNothing(conReferenza) AndAlso conReferenza.Rows.Count > 0 Then
            If Not String.IsNullOrEmpty(result) Then
                result += "<br />"
            End If
            result += $"Il parametro '{parametro.Descrizione}' non può essere cancellato perchè già utilizzata dal campionamento delle materie prime"
        End If

        Return result
    End Function

    Private Function IsFiltroSpecieVarietaValido(OFiltro_Veg_Cod As JToken, OFiltro_Cul_Cod As JToken, ByRef messaggio As String) As Boolean
        Dim result As Boolean = False
        Dim speci As Integer() = ConvertIntegerArray(OFiltro_Veg_Cod)
        Dim varieta As Integer() = ConvertIntegerArray(OFiltro_Cul_Cod)

        Dim countSpeci As Integer = speci.Count()
        Dim countVarieta As Integer = varieta.Count()

        If countSpeci = 0 AndAlso Not countVarieta = 0 Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "Non è possibile impostare una varietà senza aver definito una specie"
        ElseIf countSpeci > 1 AndAlso Not countVarieta = 0 Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "Non è possibile impostare delle varietà per più speci"
        Else ' countSpeci = 1, nessun controllo su countVarieta perchè sempre valido
            result = True
        End If

        Return result
    End Function

    Private Function IsRangeMaxMinValido(tipoCod As Short, valoreMinimo As String, valoreMaximo As String,
                                         numDecimaliMaximo As Short?, ByRef messaggio As String) As Boolean
        Dim result As Boolean = True
        Select Case tipoCod
            Case 3 'tipo dato numerico => verifica valòri min/max
                If Not String.IsNullOrEmpty(valoreMinimo) AndAlso String.IsNullOrEmpty(valoreMaximo) Then
                    If Not IsNumeric(valoreMinimo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore mininmo deve essere un valore numerico"
                        result = False
                    End If

                ElseIf String.IsNullOrEmpty(valoreMinimo) AndAlso Not String.IsNullOrEmpty(valoreMaximo) Then

                    If Not IsNumeric(valoreMaximo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore massimo deve essere un valore numerico"
                        result = False
                    End If

                ElseIf Not String.IsNullOrEmpty(valoreMinimo) AndAlso Not String.IsNullOrEmpty(valoreMaximo) Then
                    Dim minimo As Double = 0
                    Dim massimo As Double = 0

                    If Not IsNumeric(valoreMinimo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore mininmo deve essere un valore numerico"
                        result = False
                    Else
                        minimo = CDbl(valoreMinimo)
                    End If

                    If Not IsNumeric(valoreMaximo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore massimo deve essere un valore numerico"
                        result = False
                    Else
                        massimo = CDbl(valoreMaximo)
                    End If

                    If result AndAlso (minimo > massimo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore minimo deve essere un valore numerico inferiore, oppure uguale, al valore massimo"
                        result = False
                    End If

                End If

                If numDecimaliMaximo.HasValue AndAlso numDecimaliMaximo.Value < 0 Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Il numero massimo di decimali deve essere un valore numerico maggiore, oppure uguale a 0(Zero)"
                    result = False
                End If

                'Case 4 'tipo dato caratteri => verifica valòri min/max per ora non prevista
            Case Else
                If Not String.IsNullOrEmpty(valoreMinimo) Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Per la tipologia di dato il valore mininmo deve essere vuoto"
                    result = False
                End If

                If Not String.IsNullOrEmpty(valoreMaximo) Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Per la tipologia di dato il valore massimo deve essere vuoto"
                    result = False
                End If

                If Not IsNumeric(numDecimaliMaximo) OrElse
                    (Not numDecimaliMaximo = 0) Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Per la tipologia di dato il numero massimo di decimali deve essere vuoto"
                    result = False
                End If
        End Select
        Return result
    End Function

    Private Function Scrivi(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of OTabelle_Parametri),
                           ByVal EFArrayToUpdate As List(Of OTabelle_Parametri),
                           ByVal EFArrayToDelete As List(Of OTabelle_Parametri),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_W.ScriviParametri()"

        Dim messaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze

        Try
            'Prova di scrittura

            'Scrittura in Entity Framework 
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)
                Dim transaction As DbContextTransaction = Nothing
                ' Contiene anche i dettagli
                Try
                    transaction = GiasContext.Database.BeginTransaction()
                    ' Contiene anche i dettagli
                    For Each listProdotti As OTabelle_Parametri In EFArrayToInsert
                        Dim tabellaParCod As Integer = 0
                        Do
                            tabellaParCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "omni_tabella_parametri",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                        Loop While (tabellaParCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.OTabelle_Parametri.Any(Function(x) x.Tabella_Par_Cod = tabellaParCod))
                        listProdotti.Tabella_Par_Cod = tabellaParCod
                        GiasContext.OTabelle_Parametri.Add(listProdotti)
                    Next

                    For Each listProdotti As OTabelle_Parametri In EFArrayToUpdate
                        GiasContext.OTabelle_Parametri.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As OTabelle_Parametri In EFArrayToDelete
                        GiasContext.OTabelle_Parametri.Attach(listProdotti)
                        GiasContext.OTabelle_Parametri.Remove(listProdotti)
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

    Private Function ImpostaFiltroCod(array As Integer()) As String
        Dim result As String = String.Empty
        If Not IsNothing(array) AndAlso array.Any() Then
            result = $"|{String.Join("|", array)}|"
        End If
        Return result
    End Function

    Private Function ConvertIntegerArray(obj As JToken) As Integer()
        Dim result As Integer() = {}
        If Not IsNothing(obj) AndAlso Not String.IsNullOrEmpty(obj.ToString()) Then
            result = JArray.Parse(obj.ToString()).ToObject(Of List(Of Integer)).ToArray()
        End If
        Return result
    End Function

End Class