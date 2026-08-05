Imports System.Data.Common
Imports System.Data.SqlClient
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Documento_Default_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal sa_cod As Integer?,
                          ByVal tipoFattura As String,
                          ByVal numTipo_Cod As Integer?,
                          ByVal sezionale_Cod As Integer?,
                          ByVal causale_Cod As Integer?,
                          ByVal vincolante As Boolean?,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "Documento_Default_DAL.Documento_Default_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.AppendLine(" SELECT ")
                    StrSQL.AppendLine(" dd.piva, dd.Sa_Cod, ")
                    StrSQL.AppendLine(" CASE dd.Sa_Cod WHEN -1 THEN 'Tutti' ELSE ca.sa_nome END AS Sa_Descr, ")
                    StrSQL.AppendLine(" dd.Lav_Cod, ")
                    StrSQL.AppendLine(" CASE dd.Lav_Cod ")
                    StrSQL.AppendLine("   WHEN 0 THEN 'Tutti' ")
                    StrSQL.AppendLine("   WHEN -10000 THEN ' - ACQUISTI - ' ")
                    StrSQL.AppendLine("   WHEN -10001 THEN ' - vendite - ' ")
                    StrSQL.AppendLine("   WHEN -10002 THEN ' - TRASFERIMENTI - ' ")
                    StrSQL.AppendLine("   ELSE ope.LAV_DES ")
                    StrSQL.AppendLine(" END AS Lav_Descr, ")
                    StrSQL.AppendLine(" dd.TipoFattura as TipoFattura_Cod, ")
                    StrSQL.AppendLine(" CASE dd.TipoFattura ")
                    StrSQL.AppendLine("   WHEN 'A' THEN 'Accompagnatoria' ")
                    StrSQL.AppendLine("   WHEN 'D' THEN 'Differita' ")
                    StrSQL.AppendLine("   ELSE ' ' ")
                    StrSQL.AppendLine(" END AS TipoFattura_Descr, ")
                    StrSQL.AppendLine(" dd.Vincolante, ")
                    StrSQL.AppendLine(" COALESCE(dd.Sezionale_Cod, -99) As Sezionale_Cod, ")
                    StrSQL.AppendLine(" COALESCE(sez.Sezionale_Des,'') as Sezionale_Descr, ")
                    StrSQL.AppendLine(" COALESCE(dd.Causale_Cod, -99) As Causale_Cod, ")
                    StrSQL.AppendLine(" COALESCE(ct.Causale_Trasporto_Des,'') as CausaleDoc_Descr, ")
                    StrSQL.AppendLine(" COALESCE(dd.NumTipo_Cod, -99) As NumTipo_Cod, ")
                    StrSQL.AppendLine(" COALESCE(nt.Descrizione,'') as NumTipo_Descr ")

                    StrSQL.AppendLine(" FROM documento_Default dd ")
                    StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca ON dd.Piva = ca.PIVA AND dd.Sa_Cod = ca.sa_cod ")
                    StrSQL.AppendLine(" LEFT JOIN Operazioni ope ON dd.Lav_Cod = ope.LAV_COD ")
                    StrSQL.AppendLine(" LEFT JOIN Imprese_Sezionali sez ON dd.Piva = sez.Piva AND dd.Sezionale_Cod = sez.Sezionale_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN Causali_Trasporto ct ON dd.Causale_Cod = ct.Causale_Trasporto_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN Numeratore_Tipo nt ON dd.Piva = nt.Piva AND dd.NumTipo_Cod = nt.Tipo ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.AppendLine(" SELECT dd.* ")
                    StrSQL.AppendLine(" FROM documento_Default dd ")
            End Select

            StrSQL.Append(" WHERE dd.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND dd.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND dd.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If sa_cod.HasValue Then
                StrSQL.AppendLine(" AND dd.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod.Value) & " ")
            End If

            If Not String.IsNullOrEmpty(tipoFattura) Then
                StrSQL.AppendLine(" AND dd.TipoFattura = '" & Agro_SQL_SaveText(tipoFattura) & "' ")
            End If

            If numTipo_Cod.HasValue Then
                StrSQL.AppendLine(" AND dd.NumTipo_Cod = " & Agro_SQL_SaveNum(numTipo_Cod.Value) & " ")
            End If

            If sezionale_Cod.HasValue Then
                StrSQL.AppendLine(" AND dd.Sezionale_Cod = " & Agro_SQL_SaveNum(sezionale_Cod.Value) & " ")
            End If

            If causale_Cod.HasValue Then
                StrSQL.AppendLine(" AND dd.Causale_Cod = " & Agro_SQL_SaveNum(causale_Cod.Value) & " ")
            End If

            If vincolante.HasValue Then
                StrSQL.AppendLine(" AND dd.Vincolante = " & Agro_SQL_SaveBoolStrToInt(vincolante.Value.ToString()) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function RicercaNumeratori(ByVal dataDocumento As DateTime?,
                                    ByVal piva As String,
                                    ByVal sa_cod As Integer?,
                                    ByVal lav_cod As Integer?,
                                    ByVal lav_cod_gruppo As Integer?,
                                    ByVal fattura_accompagnatoria As Boolean,
                                    ByVal numTipo_Cod As Integer?,
                                    ByVal sezionale_Cod As Integer?,
                                    ByVal causale_Cod As Integer?,
                                    ByVal vincolante As Boolean?,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Documento_Default_DAL.Documento_Default_R.RicercaNumeratori()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" ps.Piva, ps.Numeratore_Tipo, ps.Doc_Numero_Sin, ps.Doc_Numero_Des, ")
            StrSQL.AppendLine(" ps.validita_inizio, ps.validita_fine, ")
            StrSQL.AppendLine(" COALESCE(ps.Descrizione, '') As PrefissoSuffisso_Des, ")
            StrSQL.AppendLine(" COALESCE(ps.Lunghezza_Centro, 0) as Lunghezza_Centro, ")
            StrSQL.AppendLine(" COALESCE(ps.CarattereFormattazione, '') AS CarattereFormattazione, ")
            StrSQL.AppendLine(" COALESCE(nt.Sigla, '') as Sigla, ")
            StrSQL.AppendLine(" COALESCE(nt.Descrizione, '') As NumeratoreTipo_Des, ")
            StrSQL.AppendLine(" dd.Sa_Cod, dd.Lav_Cod, dd.TipoFattura, ")
            StrSQL.AppendLine(" dd.NumTipo_Cod, dd.Vincolante ")
            StrSQL.AppendLine(" From Numeratore_PrefissoSuffisso ps ")
            StrSQL.AppendLine(" inner Join Numeratore_Tipo nt ")
            StrSQL.AppendLine(" On ps.Piva = nt.Piva And ps.Numeratore_Tipo = nt.Tipo ")
            StrSQL.AppendLine(" Left Join Documento_Default dd ")
            StrSQL.AppendLine(" On ps.Piva = dd.Piva And ps.Numeratore_Tipo = dd.NumTipo_Cod ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" ps.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If Not dataDocumento Is Nothing AndAlso dataDocumento.HasValue Then
                StrSQL.AppendLine(" And ps.Validita_Inizio <= " & Agro_SQL_SaveDate(dataDocumento) & " ")
                StrSQL.AppendLine(" And ps.Validita_Fine >= " & Agro_SQL_SaveDate(dataDocumento) & " ")
            End If
            If Not sa_cod Is Nothing AndAlso sa_cod.HasValue Then
                StrSQL.AppendLine(" And (dd.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " Or dd.Sa_Cod = -1 Or dd.Sa_Cod Is null) ")
            End If
            If Not lav_cod Is Nothing AndAlso lav_cod.HasValue Then
                StrSQL.AppendLine(" And (dd.Lav_Cod = " & Agro_SQL_SaveNum(lav_cod) & " Or dd.Lav_Cod = 0 Or dd.Lav_Cod Is null ")
                If Not lav_cod_gruppo Is Nothing AndAlso lav_cod_gruppo.HasValue Then
                    StrSQL.AppendLine(" Or dd.Lav_cod = " & Agro_SQL_SaveNum(lav_cod_gruppo) & " ")
                End If
                StrSQL.AppendLine(" )")
            End If

            If fattura_accompagnatoria Then
                StrSQL.AppendLine(" And (dd.TipoFattura= 'A' or dd.TipoFattura = ' ' or dd.TipoFattura is null ) ")
            Else
                StrSQL.AppendLine(" And (dd.TipoFattura= 'D' or dd.TipoFattura = ' ' or dd.TipoFattura is null ) ")
            End If

            If Not numTipo_Cod Is Nothing AndAlso numTipo_Cod.HasValue Then
                StrSQL.AppendLine(" And (dd.numTipo_Cod = " & Agro_SQL_SaveNum(numTipo_Cod) & " Or dd.numTipo_Cod = 0 Or dd.numTipo_Cod Is null) ")
            End If
            If Not sezionale_Cod Is Nothing AndAlso sezionale_Cod.HasValue Then
                StrSQL.AppendLine(" And (dd.sezionale_Cod = " & Agro_SQL_SaveNum(sezionale_Cod) & " Or dd.sezionale_Cod = 0 Or dd.sezionale_Cod Is null) ")
            End If
            If Not causale_Cod Is Nothing AndAlso causale_Cod.HasValue Then
                StrSQL.AppendLine(" And (dd.causale_Cod = " & Agro_SQL_SaveNum(causale_Cod) & " Or dd.causale_Cod = 0 Or dd.causale_Cod Is null) ")
            End If
            If Not vincolante Is Nothing AndAlso vincolante.HasValue Then
                StrSQL.AppendLine(" And (dd.vincolante = " & Agro_SQL_SaveBoolStrToInt(vincolante) & " Or dd.vincolante = 0 Or dd.vincolante Is null) ")
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

    Public Function RicercaDefaults(ByVal piva As String,
                                    ByVal sa_cod As Integer?,
                                    ByVal lav_cod As Integer?,
                                    ByVal lav_cod_gruppo As Integer?,
                                    ByVal fattura_accompagnatoria As Boolean,
                                    ByVal numTipo_Cod As Integer?,
                                    ByVal sezionale_Cod As Integer?,
                                    ByVal causale_Cod As Integer?,
                                    ByVal vincolante As Boolean?,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Documento_Default_DAL.Documento_Default_R.RicercaDefaults()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" dd.Sa_Cod, dd.Lav_Cod, dd.TipoFattura, ")
            StrSQL.AppendLine(" dd.NumTipo_Cod, dd.Sezionale_Cod, dd.Causale_Cod ")
            StrSQL.AppendLine(" From Documento_Default dd ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" dd.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If Not sa_cod Is Nothing AndAlso sa_cod.HasValue Then
                StrSQL.AppendLine(" And (dd.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " Or dd.Sa_Cod = -1 Or dd.Sa_Cod Is null) ")
            End If
            If Not lav_cod Is Nothing AndAlso lav_cod.HasValue Then
                StrSQL.AppendLine(" And (dd.Lav_Cod = " & Agro_SQL_SaveNum(lav_cod) & " Or dd.Lav_Cod = 0 Or dd.Lav_Cod Is null ")
                If Not lav_cod_gruppo Is Nothing AndAlso lav_cod_gruppo.HasValue Then
                    StrSQL.AppendLine(" Or dd.Lav_cod = " & Agro_SQL_SaveNum(lav_cod_gruppo) & " ")
                End If
                StrSQL.AppendLine(" )")
            End If

            If fattura_accompagnatoria Then
                StrSQL.AppendLine(" And (dd.TipoFattura= 'A' or dd.TipoFattura = ' ' or dd.TipoFattura is null ) ")
            Else
                StrSQL.AppendLine(" And (dd.TipoFattura= 'D' or dd.TipoFattura = ' ' or dd.TipoFattura is null ) ")
            End If

            If Not numTipo_Cod Is Nothing AndAlso numTipo_Cod.HasValue Then
                StrSQL.AppendLine(" And (dd.numTipo_Cod = " & Agro_SQL_SaveNum(numTipo_Cod) & " Or dd.numTipo_Cod = 0 Or dd.numTipo_Cod Is null) ")
            End If
            If Not sezionale_Cod Is Nothing AndAlso sezionale_Cod.HasValue Then
                StrSQL.AppendLine(" And (dd.sezionale_Cod = " & Agro_SQL_SaveNum(sezionale_Cod) & " Or dd.sezionale_Cod = 0 Or dd.sezionale_Cod Is null) ")
            End If
            If Not causale_Cod Is Nothing AndAlso causale_Cod.HasValue Then
                StrSQL.AppendLine(" And (dd.causale_Cod = " & Agro_SQL_SaveNum(causale_Cod) & " Or dd.causale_Cod = 0 Or dd.causale_Cod Is null) ")
            End If
            If Not vincolante Is Nothing AndAlso vincolante.HasValue Then
                StrSQL.AppendLine(" And (dd.vincolante = " & Agro_SQL_SaveBoolStrToInt(vincolante) & " Or dd.vincolante = 0 Or dd.vincolante Is null) ")
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

End Class



Public Class Documento_Default_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub New()

    End Sub

    Public Function Scrivi(ByVal piva As String,
                           ByVal sa_Cod As Integer,
                           ByVal lav_Cod As Integer,
                           ByVal tipoFattura As String,
                           ByVal numTipo_Cod As Integer?,
                           ByVal sezionale_Cod As Integer?,
                           ByVal causale_Cod As Integer?,
                           ByVal vincolante As Boolean,
                           ByVal data_Inizio As DateTime?,
                           ByVal data_Fine As DateTime?,
                           ByVal username_creazione As String,
                           ByVal username_modifica As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Documento_Default_DAL.Documento_Default_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Documento_Default " + vbCrLf)
            StrSQL.Append(" ( Piva, Sa_Cod, Lav_Cod, TipoFattura, " + vbCrLf)
            StrSQL.Append(" NumTipo_Cod, Sezionale_Cod, " + vbCrLf)
            StrSQL.Append(" Causale_Cod, Vincolante, " + vbCrLf)
            StrSQL.Append(" Inviato, datainvio, " + vbCrLf)
            StrSQL.Append(" Data_Creazione, Data_Modifica, " + vbCrLf)
            StrSQL.Append(" Username_Creazione, Username_Modifica, " + vbCrLf)
            StrSQL.Append(" Validita_Inizio, Validita_Fine) " + vbCrLf)

            StrSQL.Append(" VALUES " + vbCrLf)
            StrSQL.Append(" ( '" & Agro_SQL_SaveText(piva) & "', " + vbCrLf)
            StrSQL.Append(" " & Agro_SQL_SaveNum(sa_Cod) & ", " + vbCrLf)
            StrSQL.Append(" " & Agro_SQL_SaveNum(lav_Cod) & ", " + vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(tipoFattura) & "', " + vbCrLf)

            If Not numTipo_Cod Is Nothing AndAlso numTipo_Cod.HasValue Then
                StrSQL.Append(" " & Agro_SQL_SaveNum(numTipo_Cod) & ", " + vbCrLf)
            Else
                StrSQL.Append(" NULL, " + vbCrLf)
            End If

            If Not sezionale_Cod Is Nothing AndAlso sezionale_Cod.HasValue Then
                StrSQL.Append(" " & Agro_SQL_SaveNum(sezionale_Cod) & ", " + vbCrLf)
            Else
                StrSQL.Append(" NULL, " + vbCrLf)
            End If

            If Not causale_Cod Is Nothing AndAlso causale_Cod.HasValue Then
                StrSQL.Append(" " & Agro_SQL_SaveNum(causale_Cod) & ", " + vbCrLf)
            Else
                StrSQL.Append(" NULL, " + vbCrLf)
            End If

            StrSQL.Append(" " & Agro_SQL_SaveBoolStrToInt(vincolante) & ", " + vbCrLf)
            StrSQL.Append(" 0, null, " + vbCrLf)
            StrSQL.Append(Agro_SQL_SaveDateTime(DateTime.Now) & ", " & Agro_SQL_SaveDateTime(DateTime.Now) & ", " + vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(username_creazione) & "', '" & Agro_SQL_SaveText(username_modifica) & "', " + vbCrLf)
            StrSQL.Append(Agro_SQL_SaveDateTime(data_Inizio) & ", " & Agro_SQL_SaveDateTime(data_Fine) & vbCrLf)
            StrSQL.Append(" )")

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

    Public Function Aggiorna(ByVal piva As String,
                          ByVal sa_Cod As Integer,
                           ByVal lav_Cod As Integer,
                           ByVal tipoFattura As String,
                           ByVal numTipo_Cod As Integer?,
                           ByVal sezionale_Cod As Integer?,
                           ByVal causale_Cod As Integer?,
                           ByVal vincolante As Boolean,
                           ByVal data_Inizio As DateTime?,
                           ByVal data_Fine As DateTime?,
                           ByVal username_modifica As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Documento_Default_DAL.Documento_Default_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Documento_Default " + vbCrLf)
            StrSQL.Append(" Set tipoFattura = '" & Agro_SQL_SaveText(tipoFattura) & "', " + vbCrLf)

            If Not numTipo_Cod Is Nothing AndAlso numTipo_Cod.HasValue Then
                StrSQL.Append(" NumTipo_Cod = " & Agro_SQL_SaveNum(numTipo_Cod) & ", " + vbCrLf)
            Else
                StrSQL.Append(" NumTipo_Cod = NULL, " + vbCrLf)
            End If

            If Not sezionale_Cod Is Nothing AndAlso sezionale_Cod.HasValue Then
                StrSQL.Append(" Sezionale_Cod = " & Agro_SQL_SaveNum(sezionale_Cod) & ", " + vbCrLf)
            Else
                StrSQL.Append(" Sezionale_Cod = NULL, " + vbCrLf)
            End If

            If Not causale_Cod Is Nothing AndAlso causale_Cod.HasValue Then
                StrSQL.Append(" Causale_Cod = " & Agro_SQL_SaveNum(causale_Cod) & ", " + vbCrLf)
            Else
                StrSQL.Append(" Causale_Cod = NULL, " + vbCrLf)
            End If

            StrSQL.Append(" Vincolante = " & Agro_SQL_SaveBoolStrToInt(vincolante) & ", " + vbCrLf)
            StrSQL.Append(" Validita_Inizio = " & Agro_SQL_SaveDateTime(data_Inizio) & ", " & vbCrLf)
            StrSQL.Append(" Validita_Fine = " & Agro_SQL_SaveDateTime(data_Fine) & ", " & vbCrLf)
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " + vbCrLf)
            StrSQL.Append(" WHERE " + vbCrLf)
            StrSQL.Append(" Piva = '" & Agro_SQL_SaveText(piva) & "' " + vbCrLf)
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " " + vbCrLf)
            StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(lav_Cod) & " " + vbCrLf)
            StrSQL.Append(" And TipoFattura = '" & Agro_SQL_SaveText(tipoFattura) & "' " & vbCrLf)

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
    Public Function Cancella(ByVal piva As String,
                            ByVal sa_Cod As Integer,
                            ByVal lav_Cod As Integer,
                            ByVal tipoFattura As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "Documento_Default_DAL.Documento_Default_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" DELETE FROM documento_default " + vbCrLf)
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
            StrSQL.Append(" And Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " " & vbCrLf)
            StrSQL.Append(" And Lav_Cod = " & Agro_SQL_SaveNum(lav_Cod) & " " & vbCrLf)
            StrSQL.Append(" And TipoFattura = '" & Agro_SQL_SaveText(tipoFattura) & "' " & vbCrLf)

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.Append(" And '" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & "' " & vbCrLf)
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


