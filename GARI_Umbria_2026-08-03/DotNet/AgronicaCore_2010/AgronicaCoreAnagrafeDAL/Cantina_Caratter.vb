Imports AgronicaCoreDataProvider
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDTOStd.Identity
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity
Imports System.Data.Entity.Core.Metadata.Edm
Imports AgronicaCoreModelsSTD.exceptions

Public Class Cantina_Caratter_R

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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################


    Public Function Leggi_Cantine(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.Leggi_Cantine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Centri_Aziendali.* " &
                                " FROM   Centri_Aziendali, Cantina_Caratteristiche " &
                                " WHERE  Centri_Aziendali.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                " AND    Centri_Aziendali.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                " AND    Centri_Aziendali.Piva = Cantina_Caratteristiche.Piva " &
                                " AND    Centri_Aziendali.Sa_Cod = Cantina_Caratteristiche.Sa_Cod ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Cantina_Caratteristiche.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Cantina_Caratteristiche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome ASC ")
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

    Public Function LeggiReparti(piva As String, objParametri_Server As AgronicaCoreParametri,
                                 Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                 Optional ByVal Validita_Fine As Date = AGRODATAFINE) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.Leggi_Cantine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT DISTINCT Centri_Aziendali.PIVA,")
            StrSQL.AppendLine(" Centri_Aziendali.sa_cod,")
            StrSQL.AppendLine(" Centri_Aziendali.sa_nome,")
            StrSQL.AppendLine(" Cantina_Caratteristiche.Piano_Cod,")
            StrSQL.AppendLine(" Cantina_Caratteristiche.Piano_Des")
            StrSQL.AppendLine("FROM Centri_Aziendali")
            StrSQL.AppendLine("JOIN Cantina_Caratteristiche")
            StrSQL.AppendLine("ON Centri_Aziendali.Piva = Cantina_Caratteristiche.Piva")
            StrSQL.AppendLine("AND Centri_Aziendali.Sa_Cod = Cantina_Caratteristiche.Sa_Cod")
            StrSQL.AppendLine("WHERE Cantina_Caratteristiche.PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND Centri_Aziendali.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Centri_Aziendali.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function VerificaDescrizioneReparti(piva As String, saCod As Integer, pianoCod As Integer, pianoDes As String,
                                               objParametri_Server As AgronicaCoreParametri,
                                               Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                               Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.VerificaDescrizioneReparti()"

        Dim result As Boolean = False
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("SELECT 1")
            StrSQL.AppendLine("FROM Cantina_Caratteristiche")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND NOT sa_cod = " & Agro_SQL_SaveNum(saCod))
            StrSQL.AppendLine("AND NOT Piano_Cod = " & Agro_SQL_SaveNum(pianoCod))
            StrSQL.AppendLine("AND Piano_Des = '" & Agro_SQL_SaveText(Trim(pianoDes)) & "'")
            StrSQL.AppendLine("AND Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

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

    Public Function VerificaCancellazioneReparti(piva As String, saCod As Integer, pianoCod As Integer,
                                               objParametri_Server As AgronicaCoreParametri,
                                               Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                               Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.VerificaCancellazioneReparti()"

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
            StrSQL.AppendLine("AND Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("AND Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

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


Public Class Cantina_Caratter_W

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
    Public Function ScriviPiano(
                    ByVal Piva As String,
                    ByVal Sa_Cod As Integer,
                    ByVal Piano_Cod As Integer,
                    ByVal Piano_Des As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                    Optional ByVal DimX As Integer = 0,
                    Optional ByVal DimY As Integer = 0,
                    Optional ByVal Colore_Interno As Integer = 0,
                    Optional ByVal Colore_Esterno As Integer = 0,
                    Optional ByVal Spessore As Integer = 0,
                    Optional ByVal Riempimento As Integer = 0,
                    Optional ByVal Zoom As Decimal = 0,
                    Optional ByVal Data_Creazione As Date = #2/1/1900#,
                    Optional ByVal Data_Modifica As Date = #2/1/1900#,
                    Optional ByVal Username_Creazione As String = "",
                    Optional ByVal Username_Modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.ScriviPiano()"

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
            StrSQL.AppendLine(" INSERT Cantina_Caratteristiche ( ")
            StrSQL.AppendLine("   [Piva] ")
            StrSQL.AppendLine("  ,[Sa_Cod] ")
            StrSQL.AppendLine("  ,[Piano_Cod] ")
            StrSQL.AppendLine("  ,[Piano_Des] ")

            StrSQL.AppendLine("  ,[DimX] ")
            StrSQL.AppendLine("  ,[DimY] ")
            StrSQL.AppendLine("  ,[Colore_Interno] ")
            StrSQL.AppendLine("  ,[Colore_Esterno] ")
            StrSQL.AppendLine("  ,[Spessore] ")
            StrSQL.AppendLine("  ,[Riempimento] ")
            StrSQL.AppendLine("  ,[Zoom] ")
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
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Piano_Des) & "'")

            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimX) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimY) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Colore_Interno) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Colore_Esterno) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Spessore) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Riempimento) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Zoom) & " ")

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

    Public Function Scrivi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Piano_Cod As Int32,
                          ByVal Piano_Des As String,
                          ByVal DimX As Int32,
                          ByVal DimY As Int32,
                          ByVal Colore_Interno As Int32,
                          ByVal Colore_Esterno As Int32,
                          ByVal Spessore As Int32,
                          ByVal Riempimento As Int32,
                          ByVal Zoom As Decimal,
                          ByVal UserName_Creazione As String,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.Scrivi()"

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

            If UserName_Creazione = "" Then
                UserName_Creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Cantina_Caratteristiche(PIVA, Sa_Cod, Piano_Cod, Piano_Des, " &
                        "                    DimX, DimY, Colore_Interno, Colore_Esterno, Spessore, Riempimento, Zoom,  " &
                        "                    Inviato, DataInvio, " &
                        "                    Data_Creazione,     Data_Modifica, " &
                        "                    UserName_Creazione, UserName_Modifica, " &
                        "                    Validita_Inizio,    Validita_Fine " &
                        "                    ) " &
                        "VALUES (" &
                        "          '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        "         , " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                        "         , " & Agro_SQL_SaveNum(Piano_Cod) & "  " &
                        "         ,'" & Agro_SQL_SaveText(Piano_Des) & "' " &
                        "         , " & Agro_SQL_SaveNum(DimX) & " " &
                        "         , " & Agro_SQL_SaveNum(DimY) & "  " &
                        "         , " & Agro_SQL_SaveNum(Colore_Interno) & "  " &
                        "         , " & Agro_SQL_SaveNum(Colore_Esterno) & "  " &
                        "         , " & Agro_SQL_SaveNum(Spessore) & "  " &
                        "         , " & Agro_SQL_SaveNum(Riempimento) & "  " &
                        "         , " & Agro_SQL_SaveNum(Zoom) & "  " &
                        "         , 0  " &
                        "         , Null  " &
                        "         , " & Agro_SQL_SaveDate(Data_creazione) & "  " &
                        "         , " & Agro_SQL_SaveDate(Data_modifica) & "  " &
                        "         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' " &
                        "         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' " &
                        "         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " &
                        "         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " &
                        ")")



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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################


    Public Sub Modifica(ByVal Piva As String,
                    ByVal Sa_Cod As Int32,
                    ByVal Piano_Cod As Int32,
                    ByVal Piano_Des As String,
                    ByVal DimX As Int32,
                    ByVal DimY As Int32,
                    ByVal Colore_Interno As Int32,
                    ByVal Colore_Esterno As Int32,
                    ByVal Spessore As Int32,
                    ByVal Riempimento As Int32,
                    ByVal Zoom As Decimal,
                    ByVal UserName_Modifica As String,
                    ByVal Validita_Inizio As Date,
                    ByVal Validita_Fine As Date,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Cantina_Caratteristiche SET " &
                        "    Piano_Des         = '" & Agro_SQL_SaveText(Piano_Des) & "' " &
                        "   ,DimX              = " & Agro_SQL_SaveNum(DimX) & "  " &
                        "   ,DimY              = " & Agro_SQL_SaveNum(DimY) & "  " &
                        "   ,Colore_Interno    = " & Agro_SQL_SaveNum(Colore_Interno) & "  " &
                        "   ,Colore_Esterno    = " & Agro_SQL_SaveNum(Colore_Esterno) & "  " &
                        "   ,Spessore          = " & Agro_SQL_SaveNum(Spessore) & "  " &
                        "   ,Riempimento       = " & Agro_SQL_SaveNum(Riempimento) & "  " &
                        "   ,Zoom              = " & Agro_SQL_SaveNum(Zoom) & "  " &
                        "   ,Inviato           =  0 " &
                        "   ,DataInvio         =  Null " &
                        "   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & " " &
                        "   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' " &
                        "   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                        "   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                        " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        " AND   Sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
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


    End Sub

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################


    Public Sub Cancella(ByVal UserName_Modifica As String,
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Piano_Cod As Int32,
                        ByVal Validita_Fine As Date,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            If (Validita_Fine <> AGRODATAINIZIO) Then

                '############################
                '### Cancellazione Logica ###
                '############################

                StrSQL.Append(" UPDATE   Cantina_Caratteristiche " &
                                " SET " &
                                "          Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                "         ,Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' " &
                                " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                If Piano_Cod <> 0 Then
                    StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                End If

            Else

                '############################
                '### Cancellazione Fisica ###
                '############################

                'Verifico se il dato e' gia' stato inviato al server

                StrSQL.Append(" SELECT   Inviato " &
                       " FROM     Cantina_Caratteristiche " &
                       " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                       " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                If Piano_Cod <> 0 Then
                    StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                End If


                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                'If Not rs.EOF Then
                If DT.Rows.Count > 0 Then

                    If DT.Rows(0).Item("Inviato") = 1 Then

                        '######################################
                        '### Cancellazione Fisica Rinviata  ###
                        '######################################
                        '
                        StrSQL.Append(" UPDATE   Cantina_Caratteristiche " &
                               " SET " &
                               "          Validita_Fine = " & Agro_SQL_SaveDate(Date.Now) & " " &
                               "         ,Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' " &
                               "         ,Inviato = -1 " &
                               " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                               " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                        If Piano_Cod <> 0 Then
                            StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                        End If


                        '
                        '######################################

                    Else

                        '######################################
                        '### Cancellazione Fisica Immediata ###
                        '######################################
                        '
                        StrSQL.Append(" DELETE " &
                               " FROM     Cantina_Caratteristiche " &
                               " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                               " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")


                        If Piano_Cod <> 0 Then
                            StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                        End If

                        '
                        '######################################

                    End If

                End If

            End If


            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    Public Sub AggiornaValiditaInizio(ByVal Piva As String,
                                      ByVal Sa_Cod As Int32,
                                      ByVal Piano_Cod As Int32,
                                      ByVal UserName_Modifica As String,
                                      ByVal Validita_Inizio As Date,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.AggiornaValiditaInizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xRisp As Boolean = False

        Try
            StrSQL.Append("UPDATE Cantina_caratteristiche SET " &
                        "   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'" &
                        "   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) &
                        " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        " AND   Sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                        " AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            End If
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    Public Sub AggiornaValiditaFine(ByVal Piva As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal Piano_Cod As Int32,
                                          ByVal UserName_Modifica As String,
                                          ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.AggiornaValiditaFine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Append("UPDATE Cantina_caratteristiche SET " &
                        "   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'" &
                        "   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Fine) &
                        " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        " AND   Sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                        " AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Fine))

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

#Region "Cantina Caratteristiche EF"

    Public Function DeleteCantinaCaratteristicheEF(
                                                  ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal pianoCod As Integer,
                                                  ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                                  Optional SistemaOrigine As Integer = -1
                                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_W.DeleteCantinaCaratteristicheEF()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            Using scope As New TransactionScope(scopeOption, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim piano = (From v In GiasContext.Cantina_Caratteristiche Where v.PIVA = Piva And
                                                               v.sa_cod = Sa_Cod And
                                                               v.Piano_Cod = pianoCod).FirstOrDefault

                    GiasContext.Cantina_Caratteristiche.Remove(piano)
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

#End Region

End Class

