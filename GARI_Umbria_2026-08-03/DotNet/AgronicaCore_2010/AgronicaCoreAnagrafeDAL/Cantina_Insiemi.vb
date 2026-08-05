Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity.Core.Metadata.Edm
Imports System.Data.Entity
Imports System.Reflection
Imports AgronicaCoreModelsSTD.exceptions

Public Class Cantina_Insiemi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Insieme_Cod As Integer,
                            ByVal Piano_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Insiemi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Cantina_Insiemi ")
            StrSQL.AppendLine(" WHERE 1=1")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND (Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                    End If
                End If
            End If


            If Insieme_Cod <> 0 Then
                StrSQL.AppendLine(" AND Insieme_Cod = " & Agro_SQL_SaveNum(Insieme_Cod) & " ")
            End If

            If Piano_Cod <> 0 Then
                StrSQL.AppendLine(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
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
            Else
                StrSQL.AppendLine(" ORDER BY Insieme_Des ")
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

    Friend Function VerificaCellaUtilizzata(piva As String, saCod As Integer, stiveDaCancellare As Integer(),
                                            objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.VerificaCancellazioneReparti()"

        Dim result As Boolean = False
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT 1")
            StrSQL.AppendLine("FROM Mov_Destinazioni")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND sa_cod = " & Agro_SQL_SaveNum(saCod))
            StrSQL.AppendLine("AND Id_Destinazione IN (" & String.Join(", ", stiveDaCancellare) & ")")
            StrSQL.AppendLine("AND Tipo_Destinazione = 16")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return result
    End Function

    Friend Function Leggi_Cella(piva As String, saCod As Integer, moduloGenerazione As enum_Omni_Modulo_Generazione,
                                pianoCod As Integer, insiemeCod As Integer, objParametri_Server As AgronicaCoreParametri) As Cantina_Insiemi
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.Leggi_Cella()"

        Dim result As Cantina_Insiemi = Nothing
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Cantina_Insiemi")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND sa_cod = " & Agro_SQL_SaveNum(saCod))
            StrSQL.AppendLine("AND Modulo_Generazione = " & Agro_SQL_SaveNum(moduloGenerazione))
            StrSQL.AppendLine("AND Piano_Cod = " & Agro_SQL_SaveNum(pianoCod))
            StrSQL.AppendLine("AND Insieme_Cod = " & Agro_SQL_SaveNum(insiemeCod))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New Cantina_Insiemi
                For Each row As DataRow In DT.Rows
                    For Each prop As PropertyInfo In result.GetType().GetProperties()
                        If DT.Columns.Contains(prop.Name) Then
                            Dim value As Object = Nothing
                            If Not IsDBNull(row(prop.Name)) Then
                                value = row(prop.Name)
                            End If
                            prop.SetValue(result, value)
                        End If
                    Next
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return result
    End Function

    Friend Function Leggi_Stiva(piva As String, saCod As Integer, moduloGenerazione As enum_Omni_Modulo_Generazione,
                                pianoCod As Integer, insiemeCod As Integer, vasCod As Integer, objParametri_Server As AgronicaCoreParametri) As Cantina_Vasche
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.Leggi_Stiva()"

        Dim result As Cantina_Vasche = Nothing
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Cantina_Vasche")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND sa_cod = " & Agro_SQL_SaveNum(saCod))
            StrSQL.AppendLine("AND Modulo_Generazione = " & Agro_SQL_SaveNum(moduloGenerazione))
            StrSQL.AppendLine("AND Piano_Cod = " & Agro_SQL_SaveNum(pianoCod))
            StrSQL.AppendLine("AND Insieme_Cod = " & Agro_SQL_SaveNum(insiemeCod))
            StrSQL.AppendLine("AND Vas_Cod = " & Agro_SQL_SaveNum(vasCod))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New Cantina_Vasche
                For Each row As DataRow In DT.Rows
                    For Each prop As PropertyInfo In result.GetType().GetProperties()
                        If DT.Columns.Contains(prop.Name) Then
                            Dim value As Object = Nothing
                            If Not IsDBNull(row(prop.Name)) Then
                                value = row(prop.Name)
                            End If
                            prop.SetValue(result, value)
                        End If
                    Next
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return result
    End Function

    Friend Function VerificaDescrizioniCelle(piva As String, saCod As Integer, pianoCod As Integer, insiemeCod As Integer,
                                             descrizione As String, objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.VerificaDescrizioniCelle()"

        Dim result As Boolean = False
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT 1")
            StrSQL.AppendLine("FROM Cantina_Insiemi")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND sa_cod = " & Agro_SQL_SaveNum(saCod))
            StrSQL.AppendLine("AND Piano_Cod = " & Agro_SQL_SaveNum(pianoCod))
            StrSQL.AppendLine("AND NOT Insieme_Cod = " & Agro_SQL_SaveNum(insiemeCod))
            StrSQL.AppendLine("AND Insieme_Des = '" & Agro_SQL_SaveText(Trim(descrizione)) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return result
    End Function

    Friend Function VerificaDescrizioniStive(piva As String, vasCod As Integer, descrizione As String, objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.VerificaDescrizioniCelle()"

        Dim result As Boolean = False
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT 1")
            StrSQL.AppendLine("FROM Cantina_Vasche")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND NOT Vas_Cod = " & Agro_SQL_SaveNum(vasCod))
            StrSQL.AppendLine("AND Identificativo = '" & Agro_SQL_SaveText(Trim(descrizione)) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return result
    End Function
End Class

Public Class Cantina_Insiemi_W
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

    '##############################################################################################
    Public Function Scrivi(
                    ByVal Piva As String,
                    ByVal Sa_Cod As Integer,
                    ByVal Piano_Cod As Integer,
                    ByVal Insieme_Cod As Integer,
                    ByVal Insieme_Des As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    Optional ByVal Modulo_Generazione As Integer = enum_Omni_Modulo_Generazione.FreshFood,
                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                    Optional ByVal DimX As Integer = 0,
                    Optional ByVal DimY As Integer = 0,
                    Optional ByVal DimH As Integer = 0,
                    Optional ByVal NumX As Integer = 0,
                    Optional ByVal NumY As Integer = 0,
                    Optional ByVal NumH As Integer = 0,
                    Optional ByVal PosX As Integer = 0,
                    Optional ByVal PosY As Integer = 0,
                    Optional ByVal Rotazione As Decimal = 0,
                    Optional ByVal Tipo_Posizionamento As Integer = 0,
                    Optional ByVal Spessore As Integer = 0,
                    Optional ByVal Colore_Esterno As Integer = 0,
                    Optional ByVal Data_Creazione As Date = #2/1/1900#,
                    Optional ByVal Data_Modifica As Date = #2/1/1900#,
                    Optional ByVal Username_Creazione As String = "",
                    Optional ByVal Username_Modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Insiemi_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_Creazione = #2/1/1900# Then
                Data_Creazione = Date.Now
            End If

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Creazione = "" Then
                Username_Creazione = objParametri.UsernameOperazione
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.AppendLine(" INSERT Cantina_Insiemi ( ")
            StrSQL.AppendLine("   [Piva] ")
            StrSQL.AppendLine("  ,[Sa_Cod] ")
            StrSQL.AppendLine("  ,[Piano_Cod] ")
            StrSQL.AppendLine("  ,[Insieme_Cod] ")
            StrSQL.AppendLine("  ,[Insieme_Des] ")
            StrSQL.AppendLine("  ,[Modulo_Generazione] ")

            StrSQL.AppendLine("  ,[DimX] ")
            StrSQL.AppendLine("  ,[DimY] ")
            StrSQL.AppendLine("  ,[DimH] ")
            StrSQL.AppendLine("  ,[NumX] ")
            StrSQL.AppendLine("  ,[NumY] ")
            StrSQL.AppendLine("  ,[NumH] ")
            StrSQL.AppendLine("  ,[PosX] ")
            StrSQL.AppendLine("  ,[PosY] ")
            StrSQL.AppendLine("  ,[Rotazione] ")
            StrSQL.AppendLine("  ,[Tipo_Posizionamento] ")
            StrSQL.AppendLine("  ,[Spessore] ")
            StrSQL.AppendLine("  ,[Colore_Esterno] ")
            StrSQL.AppendLine("  ,[Data_Creazione] ")
            StrSQL.AppendLine("  ,[Data_Modifica] ")
            StrSQL.AppendLine("  ,[Username_Creazione] ")
            StrSQL.AppendLine("  ,[Username_Modifica] ")
            StrSQL.AppendLine("  ,[Validita_Inizio] ")
            StrSQL.AppendLine("  ,[Validita_Fine] ")

            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES ( ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" ," & Agro_SQL_SaveNum(Piano_Cod) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Insieme_Cod) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Insieme_Des) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")

            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimX) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimY) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimH) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(NumX) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(NumY) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(NumH) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(PosX) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(PosY) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Rotazione) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Tipo_Posizionamento) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Spessore) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Colore_Esterno) & " ")

            StrSQL.AppendLine("	, " & Agro_SQL_SaveDateTime(Data_Creazione) & " ")
            StrSQL.AppendLine("	, " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            StrSQL.AppendLine("	,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.AppendLine("	,'" & Agro_SQL_SaveText(Username_Modifica) & "' ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    '#################################################################
    Public Sub Cancella(ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Piano_Cod As Integer,
                        ByVal Insieme_Cod As Integer,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Insiemi_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM Cantina_Caratteristiche ")
            StrSQL.AppendLine(" WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " ")

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Insieme_Cod = " & Agro_SQL_SaveNum(Insieme_Cod) & "  ")
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

#Region "Cantina Insiemi EF"
    Public Function DeleteCantinaInsiemiEF(
                                          ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal insiemeCod As Integer,
                                          ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                          Optional SistemaOrigine As Integer = -1
                                          ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Insiemi_W.DeleteCantinaInsiemiEF()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            Using scope As New TransactionScope(scopeOption, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim insieme = (From v In GiasContext.Cantina_Insiemi Where v.Piva = Piva And
                                                               v.Sa_Cod = Sa_Cod And
                                                               v.Insieme_Cod = insiemeCod).FirstOrDefault

                    GiasContext.Cantina_Insiemi.Remove(insieme)
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
            Return False
        End Try

        Return True

    End Function

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
            Dim EFArrayToInsert As New List(Of RelazioneCantineInterniVasche)
            Dim EFArrayToUpdate As New List(Of RelazioneCantineInterniVasche)
            Dim EFArrayToDelete As New List(Of RelazioneCantineInterniVasche)

            Dim isValide As Boolean = ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(piva, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = ScriviCelle(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
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
                                          EFArray As List(Of RelazioneCantineInterniVasche),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        Dim celleRaggruppate = righeArray _
            .GroupBy(Function(key) key("Insieme_Cod"), Function(gr) gr,
                     Function(k, g) New KeyValuePair(Of JToken, JToken())(g.First(), g)) _
            .ToList()

        For Each obj As KeyValuePair(Of JToken, JToken()) In celleRaggruppate
            Dim descrizioniStive = obj.Value _
                .Select(Function(s) _
                        New KeyValuePair(Of Integer, String)(
                            s("Vas_Cod").Value(Of Integer), s("Identificativo").Value(Of String))) _
                .ToList()

            messaggioErrore = VerificaRigaParametroValida(obj.Key, descrizioniStive, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim cella As New RelazioneCantineInterniVasche
                ImpostaTabellaCantinaInsiemiEF(obj.Key, cella, objParametri)

                If Not cella.CellaPresente Then
                    cella.Cantine_Interni.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                    cella.Cantine_Interni.Username_Creazione = objParametri.UsernameOperazione
                    cella.Cantine_Interni.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                    cella.Cantine_Interni.Username_Modifica = objParametri.UsernameOperazione
                    cella.Cantine_Interni.inviato = 0
                End If


                For Each vasca In obj.Value
                    ImpostaTabellaCantinaVascheEF(vasca, cella, objParametri)
                    Dim stiva As Cantina_Vasche = cella.CantinaVasche.LastOrDefault()
                    stiva.Data_Creazione = cella.Cantine_Interni.Data_Creazione
                    stiva.Username_Creazione = cella.Cantine_Interni.Username_Creazione
                    stiva.Data_Modifica = cella.Cantine_Interni.Data_Modifica
                    stiva.Username_Modifica = cella.Cantine_Interni.Username_Modifica
                    stiva.inviato = cella.Cantine_Interni.inviato
                Next

                EFArray.Add(cella)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of RelazioneCantineInterniVasche),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        Dim celleRaggruppate = righeArray _
            .GroupBy(Function(key) key("Insieme_Cod"), Function(gr) gr,
                     Function(k, g) New KeyValuePair(Of JToken, JToken())(g.First(), g)) _
            .ToList()

        For Each obj As KeyValuePair(Of JToken, JToken()) In celleRaggruppate
            Dim descrizioniStive = obj.Value _
                .Select(Function(s) _
                        New KeyValuePair(Of Integer, String)(
                            s("Vas_Cod").Value(Of Integer), s("Identificativo").Value(Of String))) _
                .ToList()

            messaggioErrore = VerificaRigaParametroValida(obj.Key, descrizioniStive, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim cella As New RelazioneCantineInterniVasche
                ImpostaTabellaCantinaInsiemiEF(obj.Key, cella, objParametri)
                If cella.CellaPresente Then
                    cella.Cantine_Interni.Insieme_Des = obj.Key("Insieme_Des")
                    If Not String.IsNullOrEmpty(obj.Key("Data_Creazione")) Then
                        cella.Cantine_Interni.Data_Creazione = Date.ParseExact(obj.Key("Data_Creazione").ToString, Format, Provider)
                    End If
                    cella.Cantine_Interni.Username_Creazione = obj.Key("Username_Creazione").ToString
                    cella.Cantine_Interni.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                    cella.Cantine_Interni.Username_Modifica = objParametri.UsernameOperazione
                    cella.Cantine_Interni.inviato = obj.Key("inviato")

                    For Each stiva In obj.Value
                        ImpostaTabellaCantinaVascheEF(stiva, cella, objParametri)
                    Next

                    EFArray.Add(cella)
                End If
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of RelazioneCantineInterniVasche),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        Dim celleRaggruppate = righeArray _
            .GroupBy(Function(key) key("Insieme_Cod"), Function(gr) gr,
                     Function(k, g) New KeyValuePair(Of JToken, JToken())(g.First(), g)) _
            .ToList()

        For Each obj As KeyValuePair(Of JToken, JToken()) In celleRaggruppate
            Dim cella As New RelazioneCantineInterniVasche With {
                .Cantine_Interni = New Cantina_Insiemi With {
                .Piva = piva,
                .Sa_Cod = obj.Key("Sa_Cod"),
                .Insieme_Cod = obj.Key("Insieme_Cod")
                },
                .CantinaVasche = New List(Of Cantina_Vasche),
                .CancellazioneTotale = obj.Key("deleteAll").Value(Of Boolean)
            }

            For Each stiva In obj.Value
                cella.CantinaVasche.Add(New Cantina_Vasche With {
                    .Piva = cella.Cantine_Interni.Piva,
                    .Sa_Cod = cella.Cantine_Interni.Sa_Cod,
                    .Vas_Cod = stiva("Vas_Cod")
                })
            Next

            messaggioErrore = VerificaRigaParametroCancellazioneValida(cella, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                EFArray.Add(cella)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ScriviCelle(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of RelazioneCantineInterniVasche),
                           ByVal EFArrayToUpdate As List(Of RelazioneCantineInterniVasche),
                           ByVal EFArrayToDelete As List(Of RelazioneCantineInterniVasche),
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
                    For Each listProdotti As RelazioneCantineInterniVasche In EFArrayToInsert
                        Dim insiemeCod As Integer = 0
                        If Not listProdotti.CellaPresente Then
                            Do
                                insiemeCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "cantina_insiemi",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                            Loop While (insiemeCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Cantina_Insiemi.Any(Function(x) x.Insieme_Cod = insiemeCod))
                            listProdotti.Cantine_Interni.Insieme_Cod = insiemeCod
                            GiasContext.Cantina_Insiemi.Add(listProdotti.Cantine_Interni)
                        Else
                            insiemeCod = listProdotti.Cantine_Interni.Insieme_Cod
                        End If

                        For Each stiva In listProdotti.CantinaVasche
                            Dim vasCod As Integer = 0
                            Do
                                vasCod = sequenza_tabelle.NuovoId_SeqMagazzino(listProdotti.Cantine_Interni.Piva,
                                                                               listProdotti.Cantine_Interni.Sa_Cod,
                                                                               0,
                                                                               AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                               objParametri)
                            Loop While (vasCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                                (GiasContext.Cantina_Vasche.Any(Function(x) x.Vas_Cod = vasCod))

                            stiva.Insieme_Cod = listProdotti.Cantine_Interni.Insieme_Cod
                            stiva.Vas_Cod = vasCod
                            GiasContext.Cantina_Vasche.Add(stiva)
                        Next

                    Next

                    For Each listProdotti As RelazioneCantineInterniVasche In EFArrayToUpdate
                        For Each stiva In listProdotti.CantinaVasche
                            GiasContext.Cantina_Vasche.Attach(stiva)
                            GiasContext.Entry(stiva).State = EntityState.Modified
                        Next

                        GiasContext.Cantina_Insiemi.Attach(listProdotti.Cantine_Interni)
                        GiasContext.Entry(listProdotti.Cantine_Interni).State = EntityState.Modified
                    Next

                    For Each listProdotti As RelazioneCantineInterniVasche In EFArrayToDelete
                        For Each stiva In listProdotti.CantinaVasche
                            GiasContext.Cantina_Vasche.Attach(stiva)
                            GiasContext.Cantina_Vasche.Remove(stiva)
                        Next

                        If listProdotti.CancellazioneTotale Then
                            GiasContext.Cantina_Insiemi.Attach(listProdotti.Cantine_Interni)
                            GiasContext.Cantina_Insiemi.Remove(listProdotti.Cantine_Interni)
                        End If
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

    Private Sub ImpostaTabellaCantinaInsiemiEF(obj As JObject, ByRef cella As RelazioneCantineInterniVasche, objParametri As AgronicaCoreParametri)
        cella.Cantine_Interni = (New Cantina_Insiemi_R).Leggi_Cella(obj("Piva"), obj("Sa_Cod"),
                                                                    enum_Omni_Modulo_Generazione.FreshFood,
                                                                    obj("Piano_Cod"), obj("Insieme_Cod"), objParametri)
        cella.CantinaVasche = New List(Of Cantina_Vasche)
        If Not IsNothing(cella.Cantine_Interni) Then
            cella.CellaPresente = True
        Else
            cella.Cantine_Interni = New Cantina_Insiemi()
            cella.Cantine_Interni.Piva = obj("Piva")
            cella.Cantine_Interni.Sa_Cod = obj("Sa_Cod")
            cella.Cantine_Interni.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood
            cella.Cantine_Interni.Insieme_Cod = obj("Insieme_Cod")
            cella.Cantine_Interni.Piano_Cod = obj("Piano_Cod")
            cella.Cantine_Interni.Insieme_Des = obj("Insieme_Des")
            cella.Cantine_Interni.Dimx = 0
            cella.Cantine_Interni.Dimy = 0
            cella.Cantine_Interni.Dimh = 0
            cella.Cantine_Interni.Numx = 0
            cella.Cantine_Interni.Numy = 0
            cella.Cantine_Interni.Numh = 0
            cella.Cantine_Interni.Posx = 0
            cella.Cantine_Interni.Posy = 0
            cella.Cantine_Interni.Rotazione = 0
            cella.Cantine_Interni.Tipo_Posizionamento = 0
            cella.Cantine_Interni.Spessore = 0
            cella.Cantine_Interni.Colore_Esterno = 0

            If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                cella.Cantine_Interni.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
            End If
            If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                cella.Cantine_Interni.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
            End If
        End If

    End Sub

    Private Sub ImpostaTabellaCantinaVascheEF(obj As JObject, ByRef cella As RelazioneCantineInterniVasche, objParametri As AgronicaCoreParametri)
        Dim stiva As Cantina_Vasche = (New Cantina_Insiemi_R).Leggi_Stiva(obj("Piva"), obj("Sa_Cod"),
                                                                    enum_Omni_Modulo_Generazione.FreshFood,
                                                                    obj("Piano_Cod"), obj("Insieme_Cod"),
                                                                    obj("Vas_Cod"), objParametri)
        If IsNothing(stiva) Then
            stiva = New Cantina_Vasche
            stiva.Piva = cella.Cantine_Interni.Piva
            stiva.Sa_Cod = cella.Cantine_Interni.Sa_Cod
            stiva.Vas_Cod = 0
            stiva.Piano_Cod = cella.Cantine_Interni.Piano_Cod
            stiva.Insieme_Cod = cella.Cantine_Interni.Insieme_Cod
            stiva.Modulo_Generazione = cella.Cantine_Interni.Modulo_Generazione
            stiva.Numero_Serie = 0
            stiva.Modello = 0
            stiva.Materiale_Cod = 0
            stiva.Appoggio_Cod = 0
            stiva.Inclinato = 0
            stiva.Refrigerata = 0
            stiva.Tipo_Tasca = 0
            stiva.Coibentata = 0
            stiva.Udm_Cod_Capacita = 0
            stiva.Capacita_Nominale = 0
            stiva.Capacita_Effettiva = 0
            stiva.Udm_Cod_Altezza = 0
            stiva.Altezza_Cilindro = 0
            stiva.Altezza_Totale = 0
            stiva.Udm_Cod_Peso = 0
            stiva.Peso = 0
            stiva.DimX = 0
            stiva.DimY = 0
            stiva.Dimh = 0
            stiva.Rotazione = 0
            stiva.Costo_Acquisto = 0
            stiva.Ammortamento = 0
            stiva.Ultima_Revisione = AGRODATAINIZIO
            stiva.Note = String.Empty
            stiva.Tipo = "R"
            stiva.Spessore = 1
            stiva.Colore_Esterno = 0
            stiva.PosX = 0
            stiva.PosY = 0
            stiva.Pos_Relativa = "00010001"
            stiva.Tipo_Destinazione = 16
            stiva.Tipo_Serbatoio = 0
            stiva.Validita_Inizio = cella.Cantine_Interni.Validita_Inizio
            stiva.Validita_Fine = cella.Cantine_Interni.Validita_Fine
        End If
        stiva.Identificativo = obj("Identificativo")

        cella.CantinaVasche.Add(stiva)

    End Sub

    Private Function VerificaRigaParametroCancellazioneValida(cella As RelazioneCantineInterniVasche, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty

        Dim objCelle As New Cantina_Insiemi_R
        Dim utilizzato As Boolean = objCelle.VerificaCellaUtilizzata(cella.Cantine_Interni.Piva, cella.Cantine_Interni.Sa_Cod,
                                                                       cella.CantinaVasche.Select(Function(s) s.Vas_Cod).ToArray(),
                                                                       objParametri)

        If utilizzato Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "La cella '" + cella.Cantine_Interni.Insieme_Des + "' non può essere cancellata perchè in uso"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaParametroValida(obj As JToken, stive As List(Of KeyValuePair(Of Integer, String)),
                                                 ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty

        Dim objCelle As New Cantina_Insiemi_R
        Dim celleDoppie As Boolean = objCelle.VerificaDescrizioniCelle(obj("Piva"), obj("Sa_Cod"), obj("Piano_Cod"),
                                                                       obj("Insieme_Cod"), obj("Insieme_Des"), objParametri)

        If celleDoppie Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio = "La descrizione della cella '" + obj("Insieme_Des").Value(Of String) + "' risulta utilizzata su altre celle"
        End If

        For Each stiva In stive
            Dim stiveDoppie As Boolean = objCelle.VerificaDescrizioniStive(obj("Piva"), stiva.Key, stiva.Value, objParametri)

            If stiveDoppie Then
                If Not String.IsNullOrEmpty(messaggio) Then
                    messaggio += "<br />"
                End If
                messaggio = "La descrizione della stiva '" + stiva.Value + "' risulta utilizzata più volte"
            End If
        Next

        Return messaggio
    End Function


#End Region

End Class
