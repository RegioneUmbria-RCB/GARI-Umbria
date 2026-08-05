Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


''' -----------------------------------------------------------------------------
''' Project	 : AgronicaCoreAnagrafeDAL
''' Class	 : ImpresexPersone_Codici_R
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[magnani]	05/05/2011	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class ImpresexPersone_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="cod_fis"></param>
    ''' <param name="id_cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	05/05/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi( _
                          ByVal PIVA As String, _
                          ByVal cod_fis As String, _
                          ByVal id_cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_Codici_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            'Select Case xSelezioneVariabile

            'Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM  ImpresexPersone_Codici ")

            StrSQL.Append(" INNER JOIN Codici_Persone ON ImpresexPersone_Codici.Id_Cod = Codici_Persone.Codice ")

            StrSQL.Append(" WHERE ImpresexPersone_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            StrSQL.Append(" AND   ImpresexPersone_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
            StrSQL.Append(" AND   Codici_Persone.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            StrSQL.Append(" AND   Codici_Persone.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


            If PIVA <> "" Then
                StrSQL.Append(" AND ImpresexPersone_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If cod_fis <> "" Then
                StrSQL.Append(" AND ImpresexPersone_Codici.Cod_Fis = " & Agro_SQL_SaveNum(Trim(cod_fis)))
            End If

            If id_cod <> 0 Then
                StrSQL.Append(" AND ImpresexPersone_Codici.Id_Cod =  " & Agro_SQL_SaveText(Trim(id_cod)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ImpresexPersone_Codici.Inviato >=0 ")
                    StrSQL.Append(" AND   Codici_Persone.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ImpresexPersone_Codici.Inviato =-1 ")
                    StrSQL.Append(" AND   Codici_Persone.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Codici_Persone.Descrizione ASC ")
            End If
            '          ''

            'End Select



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

