Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity

Public Class Pagamenti_Causali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                            ByVal Cau_Pagamento As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Pagamenti_Causali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    Pagamenti_Causali ")

            StrSQL.Append(" WHERE ( Piva_SuperUser = 'AAAAAAAAAAA' OR Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ) ")

            If Cau_Pagamento <> 0 Then
                StrSQL.Append(" AND Cau_Pagamento = " & Agro_SQL_SaveNum(Cau_Pagamento) & "   " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '------------------------------------------------------------------

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

    '###################################################################################
    Public Sub CauPagamentoSiglaDes_from_CauPagamento(ByVal Piva As String,
                                                        ByVal Cau_Pagamento As Int32,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef Cau_Pagamento_Sigla As String,
                                                        ByRef Cau_Pagamento_Des As String)

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Pagamenti_Causali_R.CauPagamentoSiglaDes_from_CauPagamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            DT = Leggi(Piva,
                        Cau_Pagamento,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Cau_Pagamento_Sigla = DT.Rows(0).Item("Cau_Pagamento_Sigla")
                Cau_Pagamento_Des = DT.Rows(0).Item("Cau_Pagamento_Des")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Public Function VerificaDescrizioneDoppiaModalitaPagamento(ByVal piva As String, ByVal pagamentoCau As Integer, ByVal descrizione As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Pagamenti_Causali_R.VerificaDescrizioneModalitaPagamento()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT 1")
            StrSQL.AppendLine(" FROM Pagamenti_Causali")
            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND NOT Cau_Pagamento = " & Agro_SQL_SaveNum(pagamentoCau))
            StrSQL.AppendLine(" AND Cau_Pagamento_Des = '" & Agro_SQL_SaveText(descrizione) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    Public Function VerificaSiglaDoppiaModalitaPagamento(ByVal piva As String, ByVal pagamentoCau As Integer, ByVal sigla As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Pagamenti_Causali_R.VerificaSiglaModalitaPagamento()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT 1")
            StrSQL.AppendLine(" FROM Pagamenti_Causali")
            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND NOT Cau_Pagamento = " & Agro_SQL_SaveNum(pagamentoCau))
            StrSQL.AppendLine(" AND Cau_Pagamento_Sigla = '" & Agro_SQL_SaveText(sigla) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    Public Function VerificaUtilizzoModalitaPagamento(ByVal piva As String, ByVal pagamentoCau As Integer,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Pagamenti_Causali_R.VerificaUtilizzoModalitaPagamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT 'Agenda' TipoBlocco, a.des_lib Riferimenti, 0 NumeroRiferimentiAggiuntivi")
            StrSQL.AppendLine(" FROM Pagamenti p")
            StrSQL.AppendLine(" JOIN Agenda a")
            StrSQL.AppendLine(" ON p.Id_Agenda = a.Id_Agenda ")
            StrSQL.AppendLine(" WHERE p.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND p.Cau_Pagamento = " & Agro_SQL_SaveNum(pagamentoCau))
            StrSQL.AppendLine(" UNION")
            StrSQL.AppendLine(" SELECT 'Contatti' TipoBlocco, LEFT(elenco.Rag_Soc, LEN(elenco.Rag_Soc) - 1) Riferimenti, elenco.altri NumeroRiferimentiAggiuntivi")
            StrSQL.AppendLine(" FROM (")
            StrSQL.AppendLine("   SELECT cc1.PIVA, cc1.Id_Cod, cc1.Val_Cod,")
            StrSQL.AppendLine("     (")
            StrSQL.AppendLine("       SELECT TOP 10 LTRIM(RTRIM(c.Rag_Soc)) + ', ' as [text()]")
            StrSQL.AppendLine("       FROM Contatti_Codici cc2 ")
            StrSQL.AppendLine("       JOIN Contatti c")
            StrSQL.AppendLine("       ON cc2.Cod_Contatto = c.Cod_Contatto")
            StrSQL.AppendLine("       AND cc1.PIVA = cc2.PIVA")
            StrSQL.AppendLine("       AND cc1.Id_Cod = cc2.Id_Cod")
            StrSQL.AppendLine("       AND cc1.Val_Cod = cc2.Val_Cod")
            StrSQL.AppendLine("       FOR XML PATH (''), TYPE")
            StrSQL.AppendLine("     ).value('text()[1]','nvarchar(max)') [Rag_Soc],")
            StrSQL.AppendLine("     (")
            StrSQL.AppendLine("       SELECT CASE WHEN COUNT(*) > 10 THEN COUNT(*) - 10 ELSE 0 END ")
            StrSQL.AppendLine("       FROM Contatti_Codici cc2 ")
            StrSQL.AppendLine("       JOIN Contatti c")
            StrSQL.AppendLine("       ON cc2.Cod_Contatto = c.Cod_Contatto")
            StrSQL.AppendLine("       AND cc1.PIVA = cc2.PIVA")
            StrSQL.AppendLine("       AND cc1.Id_Cod = cc2.Id_Cod")
            StrSQL.AppendLine("       AND cc1.Val_Cod = cc2.Val_Cod")
            StrSQL.AppendLine("     ) [altri] ")
            StrSQL.AppendLine("   FROM Contatti_Codici cc1")
            StrSQL.AppendLine("   WHERE cc1.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("   AND cc1.Id_Cod = '4004'")
            StrSQL.AppendLine("   AND cc1.Val_Cod = " & Agro_SQL_SaveNum(pagamentoCau))
            StrSQL.AppendLine("   GROUP BY cc1.PIVA, cc1.Id_Cod, cc1.Val_Cod")
            StrSQL.AppendLine(" ) elenco")

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

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Pagamenti_Causali_W
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
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_W.AggiornaRecordParametriModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of Pagamenti_Causali)
            Dim EFArrayToUpdate As New List(Of Pagamenti_Causali)
            Dim EFArrayToDelete As New List(Of Pagamenti_Causali)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = ScriviPagamentiCausali(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
            Else
                Throw New Exception(MessaggioErrore)
            End If


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        Return MessaggioErrore
    End Function

    Private Function ScriviPagamentiCausali(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of Pagamenti_Causali),
                           ByVal EFArrayToUpdate As List(Of Pagamenti_Causali),
                           ByVal EFArrayToDelete As List(Of Pagamenti_Causali),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.ScriviReparti()"

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
                    For Each listProdotti As Pagamenti_Causali In EFArrayToInsert
                        Dim cauPagamento As Integer = 0
                        Do
                            cauPagamento = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "pagamenti_causali",
                                                                1000,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                        Loop While (cauPagamento < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Pagamenti_Causali.Any(Function(x) Math.Abs(x.Cau_Pagamento) = cauPagamento))
                        listProdotti.Cau_Pagamento = cauPagamento
                        GiasContext.Pagamenti_Causali.Add(listProdotti)
                    Next

                    For Each listProdotti As Pagamenti_Causali In EFArrayToUpdate
                        GiasContext.Pagamenti_Causali.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As Pagamenti_Causali In EFArrayToDelete
                        GiasContext.Pagamenti_Causali.Attach(listProdotti)
                        GiasContext.Pagamenti_Causali.Remove(listProdotti)
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


    Private Function ImpostaRigheParametriInserire(righeArray As JArray,
                                          EFArray As List(Of Pagamenti_Causali),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim pagamentoCausale As New Pagamenti_Causali
                ImpostaTabellaPagamentiCausaliEF(obj, pagamentoCausale, objParametri)
                pagamentoCausale.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                pagamentoCausale.Username_Creazione = objParametri.UsernameOperazione
                pagamentoCausale.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                pagamentoCausale.Username_Modifica = objParametri.UsernameOperazione
                pagamentoCausale.inviato = 0
                EFArray.Add(pagamentoCausale)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of Pagamenti_Causali),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim pagamentoCausale As New Pagamenti_Causali
                ImpostaTabellaPagamentiCausaliEF(obj, pagamentoCausale, objParametri)
                pagamentoCausale.Piva_SuperUser = obj("Piva")
                pagamentoCausale.Cau_Pagamento = obj("Cau_Pagamento_Cod")
                pagamentoCausale.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                pagamentoCausale.Username_Creazione = objParametri.UsernameOperazione
                pagamentoCausale.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                pagamentoCausale.Username_Modifica = objParametri.UsernameOperazione
                pagamentoCausale.inviato = 0
                EFArray.Add(pagamentoCausale)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(righeArray As JArray,
                                                     EFArray As List(Of Pagamenti_Causali),
                                                     ByRef messaggioErrore As String,
                                                     objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray

            messaggioErrore = VerificaRigaParametroCancellazioneValida(obj, objParametri)
            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim pagamentoCausale As New Pagamenti_Causali With {
                    .Piva_SuperUser = obj("Piva"),
                    .Cau_Pagamento = obj("Cau_Pagamento_Cod")
                }

                EFArray.Add(pagamentoCausale)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaPagamentiCausaliEF(obj As JObject, pagamentoCausale As Pagamenti_Causali, objParametri As AgronicaCoreParametri)
        pagamentoCausale.Piva_SuperUser = objParametri.PivaSuperUser
        pagamentoCausale.Cau_Pagamento = 0
        pagamentoCausale.Cau_Pagamento_Sigla = obj("Cau_Pagamento_Sigla")
        pagamentoCausale.Cau_Pagamento_Des = obj("Cau_Pagamento_Des")
        pagamentoCausale.Giorni_Scadenza = obj("Giorni_Scadenza")
        pagamentoCausale.Opzione = obj("Opzione_Cod")
        pagamentoCausale.Cau_Risorsa = obj("Risorsa_Cod")
        pagamentoCausale.Tipo = obj("Tipologia_Causale_Cod")
        pagamentoCausale.Riferimento = obj("Riferimento")

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            pagamentoCausale.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            pagamentoCausale.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If

    End Sub

    Private Function VerificaRigaParametroValida(obj As JObject, objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim sigla As String = obj("Cau_Pagamento_Sigla")
        Dim descrizione As String = obj("Cau_Pagamento_Des")
        Dim objPagamenti As New Pagamenti_Causali_R
        Dim doppio As Boolean = objPagamenti.VerificaDescrizioneDoppiaModalitaPagamento(obj("Piva"), obj("Cau_Pagamento_Cod"), descrizione, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La descrizione '" + descrizione + "' risulta già in uso per un'altra modalità di pagamento"
        End If

        doppio = objPagamenti.VerificaSiglaDoppiaModalitaPagamento(obj("Piva"), obj("Cau_Pagamento_Cod"), sigla, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La sigla '" + sigla + "' risulta già in uso per un'altra modalità di pagamento"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaParametroCancellazioneValida(obj As JObject, objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Cau_Pagamento_Des")
        Dim objPagamenti As New Pagamenti_Causali_R
        Dim blocchi As DataTable = objPagamenti.VerificaUtilizzoModalitaPagamento(obj("Piva"), obj("Cau_Pagamento_Cod"), objParametri)

        If Not IsNothing(blocchi) AndAlso blocchi.Rows.Count > 0 Then
            For Each row In blocchi.Rows
                If Not String.IsNullOrEmpty(messaggio) Then
                    messaggio += "<br />"
                End If

                Select Case row("TipoBlocco")
                    Case "Agenda" : messaggio += "La modalità di pagamento '" + descrizione + "' risulta già in uso dall'agenda '" + row("Riferimenti") + "'"
                    Case "Contatti" : messaggio += "La modalità di pagamento '" + descrizione _
                            + "' risulta già in utilizzata dai seguenti contatti:<br />" _
                            + Replace(row("Riferimenti"), ", ", ",<br/>")
                    Case Else : messaggio += "La modalità di pagamento '" + descrizione + "' risulta già in uso ['" + row("TipoBlocco") + "' - " + row("Riferimenti") + "'"
                End Select
            Next
        End If

        Return messaggio
    End Function


    '##############################################################################################
    Public Function Scrivi( _
                          ByVal Piva_SuperUser As String _
                        , ByVal Cau_Pagamento As Integer _
                        , ByVal Cau_Pagamento_Sigla As String _
                        , ByVal Cau_Pagamento_Des As String _
                        , ByVal Giorni_Scadenza As Integer _
                        , ByVal Opzione As Integer _
                        , ByVal Cau_Risorsa As String _
                        , ByVal Tipo As Integer _
                        , ByVal validita_inizio As Date _
                        , ByVal validita_fine As Date _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
            ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT Pagamenti_Causali  " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            StrSQL.Append("  ,[Cau_Pagamento] " & vbCrLf)
            StrSQL.Append("  ,[Cau_Pagamento_Sigla] " & vbCrLf)
            StrSQL.Append("  ,[Cau_Pagamento_Des] " & vbCrLf)
            StrSQL.Append("  ,[Giorni_Scadenza] " & vbCrLf)
            StrSQL.Append("  ,[Opzione] " & vbCrLf)
            StrSQL.Append("  ,[Cau_Risorsa] " & vbCrLf)
            StrSQL.Append("  ,[Tipo], " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(" '" & Agro_SQL_SaveText(Piva_SuperUser) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Cau_Pagamento) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cau_Pagamento_Sigla) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cau_Pagamento_Des) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Giorni_Scadenza) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Opzione) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cau_Risorsa) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo) & " " & vbCrLf)


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_fine) & "  ")


            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class


