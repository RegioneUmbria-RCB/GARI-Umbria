Imports System.Data.Entity
Imports System.Data.Entity.Core.Metadata.Edm
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.Zoo
Imports Newtonsoft.Json.Linq

Public Class Linee_Classi_Produzioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '####################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Linea_Classe_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Linee_Classi_Produzioni_R.Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT Linee_Classi_Produzioni.* ")
            StbSQL.AppendLine(" FROM Linee_Classi_Produzioni ")
            StbSQL.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Linea_Classe_Cod <> 0 Then
                StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Linea_Classe_Cod = " & Agro_SQL_SaveNum(Linea_Classe_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.AppendLine(" AND   Linee_Classi_Produzioni.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.AppendLine(" AND   Linee_Classi_Produzioni.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Linea_Classe_Des ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '####################################################################
    'la tabella Linee_Classi_Produzioni viene usata anche per classificare i prodotti:
    'nella tabella materie_prime se il campo cat_cod è valorizzato
    'il suo valore è un Linea_Classe_Cod, quindi la materia prima è raggruppata sotto questa classe
    Public Function LeggiClassiProdotto(ByVal Piva As String,
                                        ByVal Linea_Classe_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Classi_Produzioni_R.LeggiClassiProdotto"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            'questa query serve per avere l'elenco delle classi prodotto,
            'quindi serve il distinct perché una classe può essere usata per più prodotti
            StbSQL.AppendLine(" SELECT DISTINCT Linee_Classi_Produzioni.Piva, Linea_Classe_Cod, Linea_Classe_Des, Tipo_Produzione, Linea_Classe_Padre_Cod, Tipo_Classe ")

            StbSQL.AppendLine(" FROM Linee_Classi_Produzioni ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.Cat_Cod = Linee_Classi_Produzioni.Linea_Classe_Cod ")

            StbSQL.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Linea_Classe_Cod <> 0 Then
                StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Linea_Classe_Cod = " & Agro_SQL_SaveNum(Linea_Classe_Cod))
            End If

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.AppendLine(" AND   Linee_Classi_Produzioni.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.AppendLine(" AND   Linee_Classi_Produzioni.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Linea_Classe_Des ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_con_Filtro_Tipo_Classe(ByVal Piva As String,
                                                 ByVal TipoClasse As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Classi_Produzioni_R.Leggi_con_Filtro_Tipo_Classe"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT DISTINCT Linee_Classi_Produzioni.* ")
            StbSQL.AppendLine(" FROM Linee_Classi_Produzioni ")
            StbSQL.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Validita_Inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE))

            StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO))

            If TipoClasse <> -1 Then
                StbSQL.AppendLine(" AND Linee_Classi_Produzioni.Tipo_Classe = " & Agro_SQL_SaveNum(TipoClasse))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            StbSQL.AppendLine(" ORDER BY Linee_Classi_Produzioni.Piva, Linee_Classi_Produzioni.Tipo_Classe, Linee_Classi_Produzioni.Linea_Classe_Des ASC ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function VerificaCancellazioneClasseProduzione(piva As String, classeCod As Integer, objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine As String = "AgronicaCoreContabDAL.Linee_Classi_Produzioni_R.VerificaCancellazioneClasseProduzione"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim result As Boolean = False

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT TOP 1 1")
            StbSQL.AppendLine(" FROM Linee_Produzioni ")
            StbSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StbSQL.AppendLine(" AND Linea_Classe_Cod = " & Agro_SQL_SaveNum(classeCod))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return result
    End Function

    Friend Function VerificaDescrizioneClasseProduzione(piva As String, classeCod As Integer, descrizione As String,
                                                        objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine As String = "AgronicaCoreContabDAL.Linee_Classi_Produzioni_R.VerificaDescrizioneClasseProduzione"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim result As Boolean = False

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT TOP 1 1")
            StbSQL.AppendLine(" FROM Linee_Classi_Produzioni ")
            StbSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StbSQL.AppendLine(" AND NOT Linea_Classe_Cod = " & Agro_SQL_SaveNum(classeCod))
            StbSQL.AppendLine(" AND Linea_Classe_Des = '" & Agro_SQL_SaveText(descrizione) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return result
    End Function
End Class










''''''''''''''''''''''''''''''''''''''''''''''''''

Public Class Linee_Classi_Produzioni_W
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
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Linee_Classi_Produzioni_W.AggiornaRecordParametriModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of Linee_Classi_Produzioni)
            Dim EFArrayToUpdate As New List(Of Linee_Classi_Produzioni)
            Dim EFArrayToDelete As New List(Of Linee_Classi_Produzioni)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(piva, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = ScriviClassiProduzioni(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
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

    Private Function ScriviClassiProduzioni(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of Linee_Classi_Produzioni),
                           ByVal EFArrayToUpdate As List(Of Linee_Classi_Produzioni),
                           ByVal EFArrayToDelete As List(Of Linee_Classi_Produzioni),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Linee_Classi_Produzioni_W.ScriviClassiProduzioni()"

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
                    For Each listProdotti As Linee_Classi_Produzioni In EFArrayToInsert
                        Dim classeCod As Integer = 0
                        Do
                            classeCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "linea_classe_produzione",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                        Loop While (classeCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Linee_Classi_Produzioni.Any(Function(x) x.Linea_Classe_Cod = classeCod))
                        listProdotti.Linea_Classe_Cod = classeCod
                        GiasContext.Linee_Classi_Produzioni.Add(listProdotti)
                    Next

                    For Each listProdotti As Linee_Classi_Produzioni In EFArrayToUpdate
                        GiasContext.Linee_Classi_Produzioni.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As Linee_Classi_Produzioni In EFArrayToDelete
                        GiasContext.Linee_Classi_Produzioni.Attach(listProdotti)
                        GiasContext.Linee_Classi_Produzioni.Remove(listProdotti)
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
                                          EFArray As List(Of Linee_Classi_Produzioni),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim classeProduzione As New Linee_Classi_Produzioni
                ImpostaTabellaClassiProduzioniEF(obj, classeProduzione, objParametri)
                classeProduzione.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                classeProduzione.Username_Creazione = objParametri.UsernameOperazione
                classeProduzione.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                classeProduzione.Username_Modifica = objParametri.UsernameOperazione
                classeProduzione.inviato = 0

                EFArray.Add(classeProduzione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(righeArray As JArray,
                                          EFArray As List(Of Linee_Classi_Produzioni),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim classeProduzione As New Linee_Classi_Produzioni
                ImpostaTabellaClassiProduzioniEF(obj, classeProduzione, objParametri)
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    classeProduzione.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                classeProduzione.Username_Creazione = obj("Username_Creazione").ToString
                classeProduzione.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                classeProduzione.Username_Modifica = objParametri.UsernameOperazione
                classeProduzione.inviato = obj("inviato")

                EFArray.Add(classeProduzione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of Linee_Classi_Produzioni),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean

        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray

            messaggioErrore = VerificaRigaParametroCancellazioneValida(piva,
                                                                       obj("Linea_Classe_Cod").Value(Of Integer),
                                                                       obj("Linea_Classe_Des").Value(Of String),
                                                                       objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim classeProduzione As New Linee_Classi_Produzioni With {
                    .Piva = piva,
                    .Linea_Classe_Cod = obj("Linea_Classe_Cod")
                    }

                EFArray.Add(classeProduzione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)


        Return result

    End Function

    Private Sub ImpostaTabellaClassiProduzioniEF(obj As JObject, ByRef classeProduzione As Linee_Classi_Produzioni, objParametri As AgronicaCoreParametri)

        classeProduzione.Piva = obj("Piva")
        classeProduzione.Linea_Classe_Cod = obj("Linea_Classe_Cod")
        classeProduzione.Linea_Classe_Des = obj("Linea_Classe_Des")
        classeProduzione.Tipo_Produzione = 1
        classeProduzione.Linea_Classe_Padre_Cod = -1
        classeProduzione.Tipo_Classe = 0
        classeProduzione.Modulo_Generazione = 0
        classeProduzione.ChkUtility = 0
        classeProduzione.DirPicture = String.Empty
        classeProduzione.Sa_Cod = 0

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            classeProduzione.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            classeProduzione.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If

    End Sub

    Private Sub ImpostaTabellaLineeProduzioniXPreparazioniEF(ByRef lineaProduzione As LineaProduttiva, lineePreparazioni As List(Of Linee_Preparazioni))
        Dim linea = lineaProduzione.Linee_Produzioni
        lineaProduzione.RelazioneLineePreparazioni = lineePreparazioni _
            .Select(Function(lp) New RelazioneLineePreparazioni With
                {
                    .Linee_Preparazioni = lp,
                    .Linee_ProduzionixPreparazioni = New Linee_ProduzionixPreparazioni() With
                        {
                            .Piva = linea.Piva,
                            .Linea_Cod = linea.Linea_Cod,
                            .Preparazione_Cod = lp.Preparazione_Cod,
                            .Livello = 0,
                            .ChkLoop_Start = 0,
                            .ChkLoop_End = 0,
                            .Preparazione_Cod_Vincolo = 0,
                            .inviato = linea.inviato,
                            .datainvio = linea.datainvio,
                            .Data_Creazione = linea.Data_Creazione,
                            .Data_Modifica = linea.Data_Modifica,
                            .Username_Creazione = linea.Username_Creazione,
                            .Username_Modifica = linea.Username_Modifica,
                            .Validita_Inizio = linea.Validita_Inizio,
                            .Validita_Fine = linea.Validita_Fine,
                            .Opzionale = 0,
                            .Invisibile = 0,
                            .Linea_Modello_Cod = 0,
                            .Qta_Piano = 0
                        }
                }
            ) _
            .ToList()
    End Sub

    Private Function ImpostaLineeProduzioniMix(lineaProduzione As Linee_Produzioni, pivaSuperUser As String) As Linee_Produzioni_Mix
        Dim result As New Linee_Produzioni_Mix() With {
            .Piva_SuperUser = pivaSuperUser,
            .Piva = lineaProduzione.Piva,
            .Sa_Cod = 0,
            .Linea_Cod = lineaProduzione.Linea_Cod,
            .Veg_Cod = lineaProduzione.Veg_Cod,
            .Cul_Cod = lineaProduzione.Cul_Cod,
            .Gen_Cod = 0,
            .Spe_Cod = 0,
            .Raz_Cod = 0,
            .inviato = lineaProduzione.inviato,
            .datainvio = lineaProduzione.datainvio,
            .Data_Creazione = lineaProduzione.Data_Creazione,
            .Data_Modifica = lineaProduzione.Data_Modifica,
            .Username_Creazione = lineaProduzione.Username_Creazione,
            .Username_Modifica = lineaProduzione.Username_Modifica,
            .Validita_Inizio = lineaProduzione.Validita_Inizio,
            .Validita_Fine = lineaProduzione.Validita_Fine,
            .Livello = 0
        }

        Return result
    End Function

    Private Function VerificaRigaParametroCancellazioneValida(piva As String, classeCod As Integer, classeDes As String,
                                                              ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty

        Try
            Dim objclassiProduzione As New Linee_Classi_Produzioni_R
            messaggio = objclassiProduzione.VerificaCancellazioneClasseProduzione(piva, classeCod, objParametri)

        Catch ex As Exception
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La classe '" + classeDes + "' risulta già in uso per una linea di produzione"
        End Try

        Return messaggio
    End Function

    Private Function VerificaRigaParametroValida(obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Linea_Classe_Des")

        Dim objClassiProduzioni As New Linee_Classi_Produzioni_R
        Dim doppio As Boolean = objClassiProduzioni.VerificaDescrizioneClasseProduzione(obj("Piva"), obj("Linea_Classe_Cod"), descrizione, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La descrizione '" + descrizione + "' risulta già in uso per un'altra linea"
        End If

        Return messaggio
    End Function

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Linea_Classe_Cod As Integer,
                           ByVal Linea_Classe_Des As String,
                           ByVal Tipo_Produzione As Integer,
                           ByVal Linea_Classe_Padre_Cod As Integer,
                           ByVal Tipo_Classe As Integer,
                           ByVal Modulo_Generazione As Integer,
                           ByVal ChkUtility As Integer,
                           ByVal DirPicture As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Classi_Produzioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
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

            StrSQL.AppendLine("INSERT INTO [Linee_Classi_Produzioni] ")
            StrSQL.AppendLine("                    ( Piva , Linea_Classe_Cod , Linea_Classe_Des , Tipo_Produzione ,  Linea_Classe_Padre_Cod , inviato           ,")
            StrSQL.AppendLine("                    datainvio,  Username_Creazione , Username_Modifica ,  Validita_Inizio , Validita_Fine ,Tipo_Classe  , Modulo_Generazione ,ChkUtility  ,DirPicture, data_creazione, data_modifica   ")
            StrSQL.AppendLine("                    ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Linea_Classe_Cod))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Linea_Classe_Des) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Produzione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Linea_Classe_Padre_Cod))

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Classe))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Modulo_Generazione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkUtility))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(DirPicture) & "' ")


            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")

            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
