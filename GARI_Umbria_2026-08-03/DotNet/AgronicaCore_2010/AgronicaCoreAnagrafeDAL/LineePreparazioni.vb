Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Reflection

Public Class LineePreparazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Piano_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT [PIVA],[sa_cod] ,[Piano_Cod],[Piano_Des] " + vbCrLf)
                    StrSQL.Append(",[DimX] ,[DimY],[Colore_Interno] ,[Colore_Esterno]" + vbCrLf)
                    StrSQL.Append(",[Spessore],[Riempimento],[Zoom] " + vbCrLf)
                    StrSQL.Append(" FROM  Cantina_Caratteristiche " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Piano_Cod <> 0 Then
                        StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY  Piano_Des ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " &
                                    " FROM  Cantina_Caratteristiche " &
                                    " WHERE Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Piano_Cod <> 0 Then
                        StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva, Sa_Cod, Piano_Des ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



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

    Public Function Leggi_ElencoLineePreparazioni(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                          Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineePreparazioni_R.Leggi_ElencoLineePreparazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT Piva, Preparazione_Cod, Preparazione_Des, Tipo_Produzione, ")
            StrSQL.AppendLine("   Lav_Cod, Preparazione_Sigla, Tipo_Preparazione, Modulo_Generazione")
            StrSQL.AppendLine("FROM  Linee_Preparazioni ")
            StrSQL.AppendLine("WHERE Piva = 'AAAAAAAAAAA'")
            StrSQL.AppendLine("AND Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

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

    Public Function Leggi_LineaPreparazione(piva As String,
                                            preparazioneCod As Integer,
                                            objParametri As AgronicaCoreParametri,
                                            Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                            Optional ByVal Validita_Fine As DateTime = AGRODATAFINE) As Linee_Preparazioni
        Dim result As Linee_Preparazioni
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineePreparazioni_R.Leggi_LineaPreparazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM  Linee_Preparazioni ")
            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND Preparazione_Cod = " & Agro_SQL_SaveNum(preparazioneCod))
            StrSQL.AppendLine("AND Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New Linee_Preparazioni
                Dim row As DataRow = DT.AsEnumerable().FirstOrDefault()
                For Each prop As PropertyInfo In result.GetType().GetProperties()
                    If DT.Columns.Contains(prop.Name) Then
                        Dim value As Object = Nothing
                        If Not IsDBNull(row(prop.Name)) Then
                            value = row(prop.Name)
                        End If
                        prop.SetValue(result, value)
                    End If
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function

    Public Function ImpostaLineePreparazioni(piva As String, lineePreparazioni As Integer(),
                                             objParametri As AgronicaCoreParametri,
                                             Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                             Optional ByVal Validita_Fine As DateTime = AGRODATAFINE) As List(Of Linee_Preparazioni)
        Dim result As List(Of Linee_Preparazioni) = Nothing
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineePreparazioni_R.ImpostaLineePreparazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM Linee_Preparazioni ")
            StrSQL.AppendLine("WHERE Piva = 'AAAAAAAAAAA'")
            StrSQL.AppendLine("AND Preparazione_Cod IN (" & String.Join(", ", lineePreparazioni) & ")")
            StrSQL.AppendLine("AND Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New List(Of Linee_Preparazioni)
                For Each row As DataRow In DT.Rows
                    Dim item As New Linee_Preparazioni
                    For Each prop As PropertyInfo In item.GetType().GetProperties()
                        If DT.Columns.Contains(prop.Name) Then
                            Dim value As Object = Nothing
                            If Not IsDBNull(row(prop.Name)) Then
                                value = row(prop.Name)
                            End If
                            prop.SetValue(item, value)
                        End If
                    Next
                    item.Piva = piva
                    result.Add(item)
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function

    Public Function LeggiLineeProduzioneXPreparazioni(piva As String, lineaCod As Integer, codiciPreparazione As Integer(),
                                                      objParametri As AgronicaCoreParametri,
                                                      Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                      Optional ByVal Validita_Fine As DateTime = AGRODATAFINE) As List(Of Linee_ProduzionixPreparazioni)
        Dim result As List(Of Linee_ProduzionixPreparazioni)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineePreparazioni_R.LeggiRelazioneLineeProduzioneXPreparazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM Linee_ProduzionixPreparazioni ")
            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND Linea_Cod = " & Agro_SQL_SaveNum(lineaCod))

            If (codiciPreparazione.Any(Function(x) x > 0)) Then
                StrSQL.AppendLine("AND Preparazione_Cod IN (" & String.Join(", ", codiciPreparazione) & ")")
            End If

            StrSQL.AppendLine("AND Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New List(Of Linee_ProduzionixPreparazioni)
                For Each row As DataRow In DT.Rows
                    Dim item As New Linee_ProduzionixPreparazioni
                    For Each prop As PropertyInfo In item.GetType().GetProperties()
                        If DT.Columns.Contains(prop.Name) Then
                            Dim value As Object = Nothing
                            If Not IsDBNull(row(prop.Name)) Then
                                value = row(prop.Name)
                            End If
                            prop.SetValue(item, value)
                        End If
                    Next
                    result.Add(item)
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function

End Class

Public Class LineePreparazioni_W
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
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.AggiornaRecordParametriModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of Cantina_Caratteristiche)
            Dim EFArrayToUpdate As New List(Of Cantina_Caratteristiche)
            Dim EFArrayToDelete As New List(Of Cantina_Caratteristiche)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(piva, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = ScriviReparti(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
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

    Private Function ScriviReparti(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of Cantina_Caratteristiche),
                           ByVal EFArrayToUpdate As List(Of Cantina_Caratteristiche),
                           ByVal EFArrayToDelete As List(Of Cantina_Caratteristiche),
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
                    For Each listProdotti As Cantina_Caratteristiche In EFArrayToInsert
                        Dim pianoCod As Integer = 0
                        Do
                            pianoCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "cantina_caratteristica",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                        Loop While (pianoCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Cantina_Caratteristiche.Any(Function(x) x.Piano_Cod = pianoCod))
                        listProdotti.Piano_Cod = pianoCod
                        GiasContext.Cantina_Caratteristiche.Add(listProdotti)
                    Next

                    For Each listProdotti As Cantina_Caratteristiche In EFArrayToUpdate
                        GiasContext.Cantina_Caratteristiche.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As Cantina_Caratteristiche In EFArrayToDelete
                        GiasContext.Cantina_Caratteristiche.Attach(listProdotti)
                        GiasContext.Cantina_Caratteristiche.Remove(listProdotti)
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
                                          EFArray As List(Of Cantina_Caratteristiche),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim reparto As New Cantina_Caratteristiche
                ImpostaTabellaCantinaCaratteristicheEF(obj, reparto, objParametri)
                reparto.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                reparto.Username_Creazione = objParametri.UsernameOperazione
                reparto.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                reparto.Username_Modifica = objParametri.UsernameOperazione
                reparto.inviato = 0
                EFArray.Add(reparto)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of Cantina_Caratteristiche),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim reparto As New Cantina_Caratteristiche
                ImpostaTabellaCantinaCaratteristicheEF(obj, reparto, objParametri)
                reparto.PIVA = piva
                reparto.Piano_Cod = obj("Piano_Cod")
                reparto.Piano_Des = obj("Piano_Des")
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    reparto.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                reparto.Username_Creazione = obj("Username_Creazione").ToString
                reparto.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                reparto.Username_Modifica = objParametri.UsernameOperazione
                reparto.inviato = obj("inviato")
                EFArray.Add(reparto)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of Cantina_Caratteristiche),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            Dim reparto As New Cantina_Caratteristiche With {
                .PIVA = piva,
                .sa_cod = obj("Sa_Cod"),
                .Piano_Cod = obj("Piano_Cod")
            }
            messaggioErrore = VerificaRigaParametroCancellazioneValida(reparto, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                EFArray.Add(reparto)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaCantinaCaratteristicheEF(obj As JObject, ByRef reparto As Cantina_Caratteristiche, objParametri As AgronicaCoreParametri)
        reparto.PIVA = obj("Piva")
        reparto.sa_cod = obj("Sa_Cod")
        reparto.Piano_Cod = 0
        reparto.Piano_Des = obj("Piano_Des")
        reparto.DimX = 8000
        reparto.DimY = 8000
        reparto.Colore_Interno = 16715792
        reparto.Colore_Esterno = 16715792
        reparto.Spessore = 0
        reparto.Riempimento = 0
        reparto.Zoom = 1

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            reparto.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            reparto.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If
    End Sub

    Private Function VerificaRigaParametroCancellazioneValida(reparto As Cantina_Caratteristiche, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty

        Dim objReparto As New Cantina_Caratter_R
        Dim utilizzato As Boolean = objReparto.VerificaCancellazioneReparti(reparto.PIVA, reparto.sa_cod, reparto.Piano_Cod, objParametri)

        If utilizzato Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "Il reparto '" + reparto.Piano_Des + "' non può essere cancellato perchè utilizzato da una cella"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaParametroValida(obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Piano_Des")

        Dim objReparto As New Cantina_Caratter_R
        Dim doppio As Boolean = objReparto.VerificaDescrizioneReparti(obj("Piva"), obj("Sa_Cod"), obj("Piano_Cod"), descrizione, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "La descrizione del reparto '" + descrizione + "' risulta doppia"
        End If

        Return messaggio
    End Function

End Class
