

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class GestoreCommesseLottiDAL_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function LeggiListaAssegnazioniLotto(
            ByVal CodCommessa As String,
            ByVal CodLotto As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select lu.* ")
            Stb.AppendLine(" From __V_LottiXUtenti_DDocumenti lu ")
            Stb.AppendLine(" Where CodCommessa = '" & Agro_SQL_SaveText(CodCommessa) & "' ")
            Stb.AppendLine(" And CodSottoCommessa = '" & Agro_SQL_SaveText(CodLotto) & "'")

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    '##############################################################################################
    Public Function LeggiListaLotti(
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select ")
            Stb.AppendLine("       c.Descrizione as DescrizioneCommessa ")
            Stb.AppendLine("     , l.* ")
            Stb.AppendLine(" From v_Commesse c ")
            Stb.AppendLine("     inner Join v_lotti l     ")
            Stb.AppendLine("         On c.codCommessa = l.CodCommessa ")

            'TODO: sostituire con apposito filtro
            Stb.AppendLine(" where c.Descrizione <> '" & Agro_SQL_SaveText("Man & Ass") & "' ")
            Stb.AppendLine(" and c.Descrizione NOT LIKE '%" & Agro_SQL_SaveText("Man&Ass") & "%' ")
            Stb.AppendLine(" and L.DataChiusura >= " & Agro_SQL_SaveDate(Now.Date) & " ")
            'Stb.AppendLine(" and L.StatoLotto = '" & Agro_SQL_SaveText("Aperto") & "' ")


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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

Public Class GestoreCommesseLottiDAL_W
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' <summary>
    ''' Aggiorna il campo "Valore" nella corrispondente tabella secondo i parametri passati
    ''' </summary>
    ''' <param name="IDDocumento"></param>
    ''' <param name="IDParametroModelloDocumento"></param>
    ''' <param name="IDRiga"></param>
    ''' <param name="Progressivo"></param>
    ''' <param name="ValoreSerializzatoSuStringa"></param>
    ''' <param name="TipoDatoValore">valori ammessi da tipo Enum, Dal momento che si tratta di un SQL Variant, occorre ben specificare il cast altrimenti memorizzi stringhe per numeri</param>
    ''' <param name="TabellaDD">es: ddparametridocumentidefault</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function AggiornaParametroDocumentoPerforma(
          ByVal IDDocumento As Integer,
          ByVal IDParametroModelloDocumento As Integer,
          ByVal IDRiga As Integer,
          ByVal Progressivo As Integer,
          ByVal ValoreSerializzatoSuStringa As String,
          ByVal TipoDatoValore As enum_Manager_TipiDatiSQLVariant,
          ByVal TabellaDD As String,
          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" UPDATE o SET ")

            Dim sValUpdate As String = " valore = "
            Select Case TipoDatoValore
                Case enum_Manager_TipiDatiSQLVariant.system_integer, enum_Manager_TipiDatiSQLVariant.system_Real
                    sValUpdate &= Agro_SQL_SaveNum(ValoreSerializzatoSuStringa)
                Case enum_Manager_TipiDatiSQLVariant.system_string
                    sValUpdate &= "'" & Agro_SQL_SaveText(ValoreSerializzatoSuStringa) & "'"
                Case enum_Manager_TipiDatiSQLVariant.system_dateTime
                    sValUpdate &= Agro_SQL_SaveDateTime(ValoreSerializzatoSuStringa)
            End Select

            Stb.AppendLine(sValUpdate)

            Stb.AppendLine(" From " & TabellaDD & " o ")
            Stb.AppendLine(" Where IDDocumento =  " & Agro_SQL_SaveNum(IDDocumento))
            Stb.AppendLine(" And [IDParametroModelloDocumento]  =  " & Agro_SQL_SaveNum(IDParametroModelloDocumento))
            Stb.AppendLine(" And [IDRiga] =  " & Agro_SQL_SaveNum(IDRiga))
            Stb.AppendLine(" And [Progressivo] = " & Agro_SQL_SaveNum(Progressivo))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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
