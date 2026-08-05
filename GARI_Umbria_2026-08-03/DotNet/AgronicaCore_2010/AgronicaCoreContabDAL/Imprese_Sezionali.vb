Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq

Public Class Imprese_Sezionali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal Sezionale_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Imprese_Sezionali ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Sezionale_Cod <> 0 Then
                strSql.AppendLine(" AND Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function Leggi_ImpreseSezionali(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.Leggi_ImpreseSezionali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT ISNULL(s.Piva_SuperUser, NULL) Piva_SuperUser,")
            strSql.AppendLine("  ISNULL(s.Piva, i.Piva) Piva,")
            strSql.AppendLine("  ISNULL(s.Sezionale_Cod, NULL) Sezionale_Cod,")
            strSql.AppendLine("  ISNULL(s.Sezionale_Des, i.rag_soc) Sezionale_Des,")
            strSql.AppendLine("  ISNULL(s.ChkDefault, 0) ChkDefault,")
            strSql.AppendLine("  s.inviato,")
            strSql.AppendLine("  ISNULL(s.Data_Creazione, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & ") Data_Creazione,")
            strSql.AppendLine("  s.Data_Modifica,")
            strSql.AppendLine("  s.Username_Creazione,")
            strSql.AppendLine("  s.Username_Modifica,")
            strSql.AppendLine("  ISNULL(s.Validita_Inizio, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & ") Validita_Inizio,")
            strSql.AppendLine("  ISNULL(s.Validita_Fine, " & Agro_SQL_SaveDate(AGRODATAFINE) & ") Validita_Fine,")
            strSql.AppendLine("  s.RegimeIva,")
            strSql.AppendLine("  s.LiquidazioneIva,")
            strSql.AppendLine("  s.InteresseDebitoIva_Perc,")
            strSql.AppendLine("  s.ChkPrefissoSuffisso, ")
            strSql.AppendLine("  s.Prefisso,")
            strSql.AppendLine("  s.Suffisso,")
            strSql.AppendLine("  s.RegimeFiscale_Cod, ")
            strSql.AppendLine("  s.EsigibilitaIva,")
            strSql.AppendLine("  s.Fatturazione")
            strSql.AppendLine("FROM Imprese i")
            strSql.AppendLine("LEFT JOIN Imprese_Sezionali s ")
            strSql.AppendLine("ON i.PIVA = s.Piva")
            strSql.AppendLine("WHERE i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine("ORDER BY i.Piva, s.Sezionale_Cod")

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

    Public Function Leggi_ImpreseSezionali_Base(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.Leggi_ImpreseSezionali_Base()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT ISNULL(s.Piva_SuperUser, NULL) Piva_SuperUser, ")
            strSql.AppendLine(" i.Piva,")
            strSql.AppendLine(" ISNULL(s.Sezionale_Cod, NULL) Sezionale_Cod,")
            strSql.AppendLine(" ISNULL(s.Sezionale_Des, i.rag_soc) Sezionale_Des")
            strSql.AppendLine("FROM Imprese i ")
            strSql.AppendLine("LEFT JOIN Imprese_Sezionali s ")
            strSql.AppendLine("ON i.PIVA = s.Piva ")
            strSql.AppendLine("AND s.Sezionale_Cod = 0")
            strSql.AppendLine("WHERE i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

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


    '##############################################################################################
    Public Function LeggiDistinctInteresseIva(ByVal piva As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.LeggiDistinctInteresseIva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT InteresseDebitoIva_Perc ")
            strSql.AppendLine(" FROM  Imprese_Sezionali ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    '##############################################################################################
    Public Function LeggiDistinctLiquidazioneIva(ByVal piva As String,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.LeggiDistinctLiquidazioneIva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT LiquidazioneIva ")
            strSql.AppendLine(" FROM  Imprese_Sezionali ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 " & vbCrLf)
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 " & vbCrLf)
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    '##############################################################################################
    Public Function LeggiDistinctRegimeIva(ByVal piva As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.LeggiDistinctRegimeIva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT RegimeIva ")
            strSql.AppendLine(" FROM  Imprese_Sezionali ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    '###################################################################################
    Public Function EsistonoPiuInteressiIva(ByVal piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef interessiIvaTuttiSezionali As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.EsistonoPiuInteressiIva()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        interessiIvaTuttiSezionali = 0

        Try

            dt = LeggiDistinctInteresseIva(piva, xFiltroAggiuntivo, "", objParametri)

            'se c'è più di un interesse iva diverso
            If dt IsNot Nothing AndAlso dt.Rows.Count > 1 Then
                flagEsiste = True
            ElseIf dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                interessiIvaTuttiSezionali = dt.Rows(0).Item("InteresseDebitoIva_Perc")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '###################################################################################
    Public Function EsistonoPiuLiquidazioniIva(ByVal piva As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef liquidazioneIvaTuttiSezionali As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.EsistonoPiuLiquidazioniIva()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        liquidazioneIvaTuttiSezionali = enum_LiquidazioneIva.NonImpostato

        Try

            dt = LeggiDistinctLiquidazioneIva(piva, xFiltroAggiuntivo, "", objParametri)

            'se c'è più di una liquidazione iva
            If dt IsNot Nothing AndAlso dt.Rows.Count > 1 Then
                flagEsiste = True
            ElseIf dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                liquidazioneIvaTuttiSezionali = dt.Rows(0).Item("LiquidazioneIva")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '###################################################################################
    Public Function EsistonoPiuRegimiIva(ByVal piva As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef regimeIvaTuttiSezionali As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.EsistonoPiuRegimiIva()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        regimeIvaTuttiSezionali = enum_RegimeIva.NonImpostato

        Try

            dt = LeggiDistinctRegimeIva(piva, xFiltroAggiuntivo, "", objParametri)

            'se c'è più di un regime iva
            If dt IsNot Nothing AndAlso dt.Rows.Count > 1 Then
                flagEsiste = True
            ElseIf dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                regimeIvaTuttiSezionali = dt.Rows(0).Item("RegimeIva")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function


    '###################################################################################
    Public Function Azienda_InForfettario(ByVal piva As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.Azienda_InForfettario()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim inForfettario As Boolean = False

        Dim filtro As String = " RegimeIva = " & CStr(enum_RegimeIva.Speciale)

        Try

            dt = LeggiDistinctRegimeIva(piva, filtro, "", objParametri)

            'se c'è più di un regime iva
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                inForfettario = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return inForfettario

    End Function

    '###################################################################################
    Public Function EsigibilitaIva_from_SezionaleCod(ByVal piva As String,
                                                     ByVal Sezionale_Cod As Integer,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Sezionali_R.EsigibilitaIva_from_SezionaleCod()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim esigibilitaIva As Integer = 0

        Try

            dt = Leggi(piva, Sezionale_Cod, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                esigibilitaIva = dt.Rows(0).Item("EsigibilitaIva")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return esigibilitaIva

    End Function

    Friend Function VerificaDescrizioneDoppiaImpresaSezionale(ByVal piva As String, ByVal sezionaleCod As Integer, descrizione As String, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Sezionali_R.VerificaDescrizioneDoppiaImpresaSezionale()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT TOP 1 1")
            StrSQL.AppendLine(" FROM ")
            StrSQL.AppendLine(" (")
            StrSQL.AppendLine("   SELECT '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' AS Piva_SuperUser,")
            StrSQL.AppendLine("     Piva,")
            StrSQL.AppendLine("     0 AS Sezionale_Cod,")
            StrSQL.AppendLine("     rag_soc AS Sezionale_Des")
            StrSQL.AppendLine("   FROM Imprese")
            StrSQL.AppendLine("   WHERE Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("   UNION")
            StrSQL.AppendLine("   SELECT Piva_SuperUser,")
            StrSQL.AppendLine("   	Piva,")
            StrSQL.AppendLine("   	Sezionale_Cod,")
            StrSQL.AppendLine("   	Sezionale_Des")
            StrSQL.AppendLine("   FROM Imprese_Sezionali")
            StrSQL.AppendLine("   WHERE Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" ) tot")
            StrSQL.AppendLine(" WHERE tot.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND NOT tot.Sezionale_Cod = " & Agro_SQL_SaveNum(sezionaleCod))
            StrSQL.AppendLine(" AND tot.Sezionale_Des = '" & Agro_SQL_SaveText(descrizione) & "'")

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

    Friend Function VerificaUtilizzoImpreseSezionale(ByVal piva As String, ByVal sezionaleCod As Integer, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Sezionali_R.VerificaUtilizzoImpreseSezionale()"
        Dim result As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT TOP 1 1")
            StrSQL.AppendLine(" FROM Movimenti")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND sezionale_cod = " & Agro_SQL_SaveNum(sezionaleCod))

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

Public Class Imprese_Sezionali_W
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
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Sezionali_W.AggiornaRecordParametriModificati()"

        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim esitoAggioramento As String = String.Empty
        Dim MessaggioErrore As String = String.Empty

        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of Imprese_Sezionali)
            Dim EFArrayToUpdate As New List(Of Imprese_Sezionali)
            Dim EFArrayToDelete As New List(Of Imprese_Sezionali)

            Dim isValide As Boolean = ImpostaRigheParametriImpreseSezionali(piva, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriModificate(piva, righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheParametriCancellate(righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = ScriviImpreseSezionali(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
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

    Private Function ScriviImpreseSezionali(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of Imprese_Sezionali),
                           ByVal EFArrayToUpdate As List(Of Imprese_Sezionali),
                           ByVal EFArrayToDelete As List(Of Imprese_Sezionali),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Sezionali_W.ScriviImpreseSezionali()"

        Dim messaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim sequenza_tabelle As New Agro_Sequenze

        Try
            'Prova di scrittura

            'Scrittura in Entity Framework 
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)
                Dim transaction As DbContextTransaction = Nothing
                ' Contiene anche i dettagli
                Try
                    transaction = GiasContext.Database.BeginTransaction()
                    ' Contiene anche i dettagli
                    For Each listProdotti As Imprese_Sezionali In EFArrayToInsert
                        If listProdotti.Sezionale_Cod = 0 Then
                            GiasContext.Imprese_Sezionali.Add(listProdotti)
                        Else
                            Dim sezionali As Integer = 0
                            Do
                                sezionali = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                    "imprese_sezionali",
                                                                    1000,
                                                                    UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                    objParametri)
                            Loop While (sezionali < UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                                (GiasContext.Imprese_Sezionali.Any(Function(x) Math.Abs(x.Sezionale_Cod) = sezionali))
                            listProdotti.Sezionale_Cod = sezionali
                            GiasContext.Imprese_Sezionali.Add(listProdotti)
                        End If
                    Next

                    For Each listProdotti As Imprese_Sezionali In EFArrayToUpdate
                        GiasContext.Imprese_Sezionali.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As Imprese_Sezionali In EFArrayToDelete
                        GiasContext.Imprese_Sezionali.Attach(listProdotti)
                        GiasContext.Imprese_Sezionali.Remove(listProdotti)
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

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Private Function ImpostaRigheParametriImpreseSezionali(piva As String,
                                          EFArray As List(Of Imprese_Sezionali),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = True

        Dim sezioneBase As Imprese_Sezionali = CreaImpresaSezioneBase(piva, objParametri)
        If Not IsNothing(sezioneBase) Then
            EFArray.Add(sezioneBase)
        End If

        Return result

    End Function

    Private Function ImpostaRigheParametriInserire(righeArray As JArray,
                                          EFArray As List(Of Imprese_Sezionali),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim selezione As New Imprese_Sezionali
                ImpostaTabellaImpreseSezionaliEF(obj, selezione, objParametri)
                selezione.Data_Creazione = DateTime.Now
                selezione.Username_Creazione = objParametri.UsernameOperazione
                selezione.Data_Modifica = DateTime.Now
                selezione.Username_Modifica = objParametri.UsernameOperazione
                selezione.inviato = 0
                EFArray.Add(selezione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriModificate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of Imprese_Sezionali),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim selezione As New Imprese_Sezionali
                ImpostaTabellaImpreseSezionaliEF(obj, selezione, objParametri)
                selezione.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                selezione.Username_Creazione = obj("Username_Creazione")
                selezione.Data_Modifica = DateTime.Now
                selezione.Username_Modifica = objParametri.UsernameOperazione
                selezione.inviato = 0
                EFArray.Add(selezione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheParametriCancellate(righeArray As JArray,
                                                     EFArray As List(Of Imprese_Sezionali),
                                                     ByRef messaggioErrore As String,
                                                     objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaParametroCancellazioneValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim selezione As New Imprese_Sezionali With {
                    .Piva_SuperUser = objParametri.PivaSuperUser,
                    .Piva = obj("Piva"),
                    .Sezionale_Cod = obj("Sezionale_Cod")
                }

                EFArray.Add(selezione)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaImpreseSezionaliEF(obj As JObject, sezionale As Imprese_Sezionali, objParametri As AgronicaCoreParametri)

        sezionale.Piva_SuperUser = objParametri.PivaSuperUser
        sezionale.Piva = obj("Piva")
        sezionale.Sezionale_Cod = obj("Sezionale_Cod")
        sezionale.Sezionale_Des = obj("Sezionale_Des")
        sezionale.ChkDefault = 0
        sezionale.RegimeIva = obj("RegimeIva")
        sezionale.LiquidazioneIva = obj("LiquidazioneIva")
        sezionale.InteresseDebitoIva_Perc = obj("InteresseDebitoIva_Perc")
        sezionale.ChkPrefissoSuffisso = obj("ChkPrefissoSuffisso")
        sezionale.Prefisso = obj("Prefisso")
        sezionale.Suffisso = obj("Suffisso")
        sezionale.RegimeFiscale_Cod = obj("Regime_Fiscale_Cod")
        sezionale.EsigibilitaIva = obj("Esigibilita_Iva_Cod")
        sezionale.Fatturazione = obj("Fatturazione_Elettronica_Cod")

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            sezionale.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            sezionale.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If

    End Sub

    Private Function CreaImpresaSezioneBase(piva As String, objParametri As AgronicaCoreParametri) As Imprese_Sezionali
        Dim result As Imprese_Sezionali = Nothing
        Dim objImpresaSezionale As New Imprese_Sezionali_R
        Dim DT As DataTable = objImpresaSezionale.Leggi_ImpreseSezionali_Base(piva, objParametri)
        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            Dim row As DataRow = DT.Rows(0)
            If IsDBNull(row("Sezionale_Cod")) Then
                result = New Imprese_Sezionali
                result.Piva_SuperUser = objParametri.PivaSuperUser
                result.Piva = piva
                result.Sezionale_Cod = 0
                result.Sezionale_Des = row("Sezionale_Des")
                result.ChkDefault = 0
                result.RegimeIva = 0
                result.LiquidazioneIva = 0
                result.InteresseDebitoIva_Perc = 0
                result.ChkPrefissoSuffisso = 0
                result.Prefisso = String.Empty
                result.Suffisso = String.Empty
                result.RegimeFiscale_Cod = 0
                result.EsigibilitaIva = 0
                result.Fatturazione = 1
                result.Validita_Inizio = AGRODATAINIZIO
                result.Validita_Fine = AGRODATAFINE
                result.Data_Creazione = DateTime.Now
                result.Username_Creazione = objParametri.UsernameOperazione
                result.Data_Modifica = DateTime.Now
                result.Username_Modifica = objParametri.UsernameOperazione
                result.inviato = 0
            End If
        End If

        Return result
    End Function

    Private Function VerificaRigaParametroValida(obj As JObject, objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Sezionale_Des")
        Dim objImpresaSezionale As New Imprese_Sezionali_R
        Dim doppio As Boolean = objImpresaSezionale.VerificaDescrizioneDoppiaImpresaSezionale(obj("Piva"), obj("Sezionale_Cod"), descrizione, objParametri)

        If doppio Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio &= "<br />"
            End If
            messaggio &= "La descrizione '" & descrizione & "' risulta già in uso per un'altra impresa sezionale"
        End If

        Dim dataInizio As Date = _validitaInizio
        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            dataInizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If

        Dim dataFine As Date = _validitaFine
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            dataFine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If

        If dataInizio > dataFine Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio &= "<br />"
            End If
            messaggio &= "La 'Validita_Inizio' non può essere maggiore della 'Validita_Fine'"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaParametroCancellazioneValida(obj As JObject, objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        Dim descrizione As String = obj("Sezionale_Des")
        Dim objPagamenti As New Imprese_Sezionali_R
        Dim utilizzato As Boolean = objPagamenti.VerificaUtilizzoImpreseSezionale(obj("Piva"), obj("Sezionale_Cod"), objParametri)

        If utilizzato Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio &= "<br />"
            End If
            messaggio &= "L'impresa sezionale '" & descrizione & "' risulta già utilizzata nelle movimentazioni"
        End If

        Return messaggio
    End Function
End Class