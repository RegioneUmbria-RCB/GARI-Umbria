Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider




'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################


' QUESTO CORE NON E' DA USARE!!!!
' L'INFORMAZIONE SUI FITO BIO è SOLO SULLA MATRICE E NON PIU' IN QUESTA TABELLA

' PER SAPERE SE UN FITO E' BIO OCCORRE CHIAMARE IL WEBSERVICE!





'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################





























Public Class RegolamentixFormulati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="REG_COD"></param>
    ''' <param name="FR_COD"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	03/05/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal REG_COD As Long, _
                      ByVal FR_COD As Long, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.RegolamentixFertilizza_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  RegolamentixFormulati , Formulati , Regolamenti ")
            StrSQL.Append(" WHERE RegolamentixFormulati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   RegolamentixFormulati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Formulati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Formulati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Regolamenti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Regolamenti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   RegolamentixFormulati.REG_COD = Regolamenti.REG_COD  ")
            StrSQL.Append(" AND   RegolamentixFormulati.FR_COD = Formulati.FR_COD ")

            If REG_COD <> 0 Then
                StrSQL.Append(" AND RegolamentixFormulati.REG_COD =  " & Agro_SQL_SaveNum(REG_COD) & "  ")
            End If

            If FR_COD <> 0 Then
                StrSQL.Append(" AND RegolamentixFormulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   RegolamentixFormulati.Inviato >=0 ")
                    StrSQL.Append(" AND   Formulati.Inviato >=0 ")
                    StrSQL.Append(" AND   Regolamenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   RegolamentixFormulati.Inviato =-1 ")
                    StrSQL.Append(" AND   Formulati.Inviato =-1 ")
                    StrSQL.Append(" AND   Regolamenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_DES ASC, Regolamenti.REG_DES ASC ")
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

Public Class RegolamentixFormulati_W
    Inherits AgronicaCoreDataProvider.DataProvider

End Class
