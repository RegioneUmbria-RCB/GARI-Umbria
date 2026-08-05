Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.Zoo
Imports Newtonsoft.Json.Linq

Public Class Causali_Trasporto_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Causale_Trasporto_Cod As Integer,
                          ByVal Causale_Trasporto_Sigla As String,
                          ByVal Causale_Trasporto_Des As String,
                          ByVal ChkDefault As Integer,
                          ByVal Tipo As enum_Tipo_CausaliTrasporto,
                          ByVal ChkPrefissoSuffisso As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Causali_Trasporto_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Causali_Trasporto ")
            stb.AppendLine(" WHERE (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Piva_SuperUser = 'AAAAAAAAAAA') ")

            If Causale_Trasporto_Cod <> 0 Then
                stb.AppendLine(" AND Causale_Trasporto_Cod = " & Agro_SQL_SaveNum(Causale_Trasporto_Cod) & " ")
            End If

            If Causale_Trasporto_Sigla <> "" Then
                stb.AppendLine(" AND Causale_Trasporto_Sigla = '" & Agro_SQL_SaveText(Causale_Trasporto_Sigla) & "' ")
            End If

            If Causale_Trasporto_Des <> "" Then
                stb.AppendLine(" AND Causale_Trasporto_Des = '" & Agro_SQL_SaveText(Causale_Trasporto_Des) & "' ")
            End If

            If ChkDefault <> -1 Then
                stb.AppendLine(" AND ChkDefault = " & Agro_SQL_SaveNum(ChkDefault) & " ")
            End If

            If Tipo <> enum_Tipo_CausaliTrasporto.Non_Impostato Then
                stb.AppendLine(" AND Tipo = " & Agro_SQL_SaveNum(CInt(Tipo)) & " ")
            End If

            If ChkPrefissoSuffisso <> -1 Then
                stb.AppendLine(" AND ChkPrefissoSuffisso = " & Agro_SQL_SaveNum(ChkPrefissoSuffisso) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function CausaleTrasportoCodFromDes(ByVal causaleTrasportoDes As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Causali_Trasporto_R.CausaleTrasportoCodFromDes()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim causaleTrasportoCod As Integer = 0

        Try

            dt = Leggi(0, "", causaleTrasportoDes, -1, enum_Tipo_CausaliTrasporto.Non_Impostato, -1, "", "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                causaleTrasportoCod = dt.Rows(0).Item("Causale_Trasporto_Cod")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return causaleTrasportoCod

    End Function

    Friend Function VerificaDescrizioneDoppiaCausaliTrasporto(piva As String, causaleTrasportoCod As Integer, descrizione As String, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Causali_Trasporto_R.VerificaDescrizioneDoppiaCausaliTrasporto()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT TOP 1 1")
            StrSQL.AppendLine(" FROM Causali_Trasporto")
            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND NOT Causale_Trasporto_Cod = " & Agro_SQL_SaveNum(causaleTrasportoCod))
            StrSQL.AppendLine(" AND Causale_Trasporto_Des = '" & Agro_SQL_SaveText(descrizione) & "'")

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

    Friend Function VerificaSiglaDoppiaCausaliTrasporto(piva As String, causaleTrasportoCod As Integer, sigla As String, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Causali_Trasporto_R.VerificaSiglaDoppiaCausaliTrasporto()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT TOP 1 1")
            StrSQL.AppendLine(" FROM Causali_Trasporto")
            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND NOT Causale_Trasporto_Cod = " & Agro_SQL_SaveNum(causaleTrasportoCod))
            StrSQL.AppendLine(" AND Causale_Trasporto_Sigla = '" & Agro_SQL_SaveText(sigla) & "'")

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

    Friend Function VerificaUtilizzoCausaliTrasporto(piva As String, causaleTrasportoCod As Integer, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Causali_Trasporto_R.VerificaUtilizzoCausaliTrasporto()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT TOP 1 1")
            StrSQL.AppendLine(" FROM Movimenti")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND causale_trasporto_cod = " & Agro_SQL_SaveNum(causaleTrasportoCod))

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
End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Causali_Trasporto_W
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
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Causali_Trasporto_W.AggiornaRecordParametriModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of Causali_Trasporto)
            Dim EFArrayToUpdate As New List(Of Causali_Trasporto)
            Dim EFArrayToDelete As New List(Of Causali_Trasporto)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = ScriviCausaliTrasporto(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
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

    Private Function ScriviCausaliTrasporto(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of Causali_Trasporto),
                           ByVal EFArrayToUpdate As List(Of Causali_Trasporto),
                           ByVal EFArrayToDelete As List(Of Causali_Trasporto),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Causali_Trasporto_W.ScriviCausaliTrasporto()"

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
                    For Each listProdotti As Causali_Trasporto In EFArrayToInsert
                        Dim causaleTrasporto As Integer = 0
                        Do
                            causaleTrasporto = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "causaletrasporto",
                                                                1000,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                        Loop While (causaleTrasporto < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Causali_Trasporto.Any(Function(x) Math.Abs(x.Causale_Trasporto_Cod) = causaleTrasporto))
                        listProdotti.Causale_Trasporto_Cod = causaleTrasporto
                        GiasContext.Causali_Trasporto.Add(listProdotti)
                    Next

                    For Each listProdotti As Causali_Trasporto In EFArrayToUpdate
                        GiasContext.Causali_Trasporto.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As Causali_Trasporto In EFArrayToDelete
                        GiasContext.Causali_Trasporto.Attach(listProdotti)
                        GiasContext.Causali_Trasporto.Remove(listProdotti)
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
                                          EFArray As List(Of Causali_Trasporto),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim trasportoCausale As New Causali_Trasporto
                ImpostaTabellaCausaliTrasportoEF(obj, trasportoCausale, objParametri)
                trasportoCausale.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                trasportoCausale.Username_Creazione = objParametri.UsernameOperazione
                trasportoCausale.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                trasportoCausale.Username_Modifica = objParametri.UsernameOperazione
                trasportoCausale.inviato = 0
                EFArray.Add(trasportoCausale)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of Causali_Trasporto),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim trasportoCausale As New Causali_Trasporto
                ImpostaTabellaCausaliTrasportoEF(obj, trasportoCausale, objParametri)
                trasportoCausale.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                trasportoCausale.Username_Creazione = objParametri.UsernameOperazione
                trasportoCausale.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                trasportoCausale.Username_Modifica = objParametri.UsernameOperazione
                trasportoCausale.inviato = 0
                EFArray.Add(trasportoCausale)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(righeArray As JArray,
                                                     EFArray As List(Of Causali_Trasporto),
                                                     ByRef messaggioErrore As String,
                                                     objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray

            messaggioErrore = VerificaRigaParametroCancellazioneValida(obj, objParametri)
            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim pagamentoCausale As New Causali_Trasporto With {
                    .Piva_SuperUser = obj("Piva"),
                    .Causale_Trasporto_Cod = obj("Causale_Trasporto_Cod")
                }

                EFArray.Add(pagamentoCausale)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaCausaliTrasportoEF(obj As JObject, trasportoCausale As Causali_Trasporto, objParametri As AgronicaCoreParametri)

        trasportoCausale.Piva_SuperUser = objParametri.PivaSuperUser
        trasportoCausale.Causale_Trasporto_Cod = obj("Causale_Trasporto_Cod")
        trasportoCausale.Causale_Trasporto_Sigla = obj("Causale_Trasporto_Sigla")
        trasportoCausale.Causale_Trasporto_Des = obj("Causale_Trasporto_Des")
        trasportoCausale.ChkDefault = 0
        trasportoCausale.Tipo = obj("Tipo_Causale_Trasporto_Cod")
        trasportoCausale.ChkPrefissoSuffisso = 0
        trasportoCausale.Prefisso = String.Empty
        trasportoCausale.Suffisso = String.Empty

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            trasportoCausale.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            trasportoCausale.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If

    End Sub

    Private Function VerificaRigaParametroValida(obj As JObject, objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim sigla As String = obj("Causale_Trasporto_Sigla")
        Dim descrizione As String = obj("Causale_Trasporto_Des")
        Dim objPagamenti As New Causali_Trasporto_R
        Dim doppio As Boolean = objPagamenti.VerificaDescrizioneDoppiaCausaliTrasporto(obj("Piva"), obj("Causale_Trasporto_Cod"), descrizione, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La descrizione '" + descrizione + "' risulta già in uso per un'altra causale trasporto"
        End If

        doppio = objPagamenti.VerificaSiglaDoppiaCausaliTrasporto(obj("Piva"), obj("Causale_Trasporto_Cod"), sigla, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La sigla '" + sigla + "' risulta già in uso per un'altra causale trasporto"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaParametroCancellazioneValida(obj As JObject, objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Cau_Pagamento_Des")
        Dim objPagamenti As New Causali_Trasporto_R
        Dim utilizzato As Boolean = objPagamenti.VerificaUtilizzoCausaliTrasporto(obj("Piva"), obj("Causale_Trasporto_Cod"), objParametri)

        If utilizzato Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += "La causale di trasporto '" + descrizione + "' risulta già utilizzata nelle movimentazioni"
        End If

        Return messaggio
    End Function

End Class