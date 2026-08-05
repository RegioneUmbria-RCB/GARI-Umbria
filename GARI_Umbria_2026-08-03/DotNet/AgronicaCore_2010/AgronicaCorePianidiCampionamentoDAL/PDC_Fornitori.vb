'Imports System.Data.OleDb
'Imports AgronicaCoreDataProvider.UtilityProvider
'Imports AgronicaCoreDataProvider.TipiEnumerativi

''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'Public Class PDC_Fornitori_R
'    Inherits AgronicaCoreDataProvider.DataProvider


'    Public Function Leggi(ByVal Piva As String, _
'                          ByVal CodiceFornitore As String, _
'                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                          ) As DataTable
'        Dim NomeRoutine As String = "PDC_Fornitori_R.LFO_R.Leggi()"


'        Dim MessaggioErrore As String = ""
'        Dim StrSQL As New System.Text.StringBuilder
'        Dim DT As DataTable

'        Try
'            StrSQL.Length = 0
'            '---------------------------------------------
'            StrSQL.AppendLine(" SELECT * ")
'            StrSQL.AppendLine(" FROM  PDC_Fornitori")
'            StrSQL.AppendLine(" WHERE  PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

'            If Piva <> "" Then
'                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
'            End If

'            If CodiceFornitore <> "" Then
'                StrSQL.AppendLine(" AND CodiceFornitore = '" & Agro_SQL_SaveText(CodiceFornitore) & "'")
'            End If

'            '--------------------------------------------------------------------------
'            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
'            '--------------------------------------------------------------------------

'        Catch ex As Exception

'            MessaggioErrore = ex.Message
'            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
'            DT = Nothing
'            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

'        End Try

'        Return DT

'    End Function

'End Class



''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

'Public Class PDC_Fornitori_W
'    Inherits AgronicaCoreDataProvider.DataProvider


'    Public Function Scrivi( _
'                            ByVal PIVA As String, _
'                            ByVal CodiceFornitore As String, _
'                            ByVal Rag_Soc As String, _
'                            ByVal Citta As String, _
'                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                                ) As Boolean

'        Dim NomeRoutine As String = "PDC_Fornitori_W.LFO_W.Scrivi()"

'        '====================================================================================
'        'Parametri opzionali :
'        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
'        '   DirectoryLOG = ""           =>  viene usato il valore di default
'        '   FileLOG = ""                =>  viene usato il valore di default
'        '====================================================================================

'        Dim MessaggioErrore As String = ""
'        Dim StrSQL As New System.Text.StringBuilder
'        Dim xRisp As Boolean = False

'        Try
'            '---------------------------------------------
'            StrSQL.Length = 0
'            StrSQL.AppendLine("INSERT INTO PDC_Fornitori (PivaSuperUser , PIVA,  CodiceFornitore , Rag_Soc, Citta )")

'            StrSQL.AppendLine("   VALUES (")
'            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
'            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PIVA) & "'")
'            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(CodiceFornitore) & "'")
'            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Rag_Soc) & "'")
'            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Citta) & "'")
'            StrSQL.AppendLine(")")

'            '--------------------------------------------------------------------------
'            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
'            '--------------------------------------------------------------------------

'        Catch ex As Exception

'            MessaggioErrore = ex.Message
'            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
'            xRisp = False
'            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

'        End Try

'        Return xRisp

'    End Function



'    Public Function Modifica(ByVal PIVA As String, _
'                            ByVal CodiceFornitore As String, _
'                            ByVal Rag_Soc As String, _
'                            ByVal Citta As String, _
'                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                                ) As Boolean

'        Dim NomeRoutine As String = "AgronicaAnalisiDAL.LFO_W.Modifica()"

'        '====================================================================================
'        'Parametri opzionali :
'        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
'        '   DirectoryLOG = ""           =>  viene usato il valore di default
'        '   FileLOG = ""                =>  viene usato il valore di default
'        '====================================================================================

'        Dim MessaggioErrore As String = ""
'        Dim StrSQL As New System.Text.StringBuilder
'        Dim xRisp As Boolean = False

'        '------------------------------

'        Try


'            '---------------------------------------------
'            StrSQL.Length = 0
'            StrSQL.AppendLine(" UPDATE PDC_Fornitori SET ")
'            StrSQL.AppendLine("   CodiceFornitore         =  '" & Agro_SQL_SaveText(CodiceFornitore) & "'   ")
'            StrSQL.AppendLine("   ,Rag_Soc         =  '" & Agro_SQL_SaveText(Rag_Soc) & "'   ")
'            StrSQL.AppendLine("   ,Citta         =  '" & Agro_SQL_SaveText(Citta) & "'   ")


'            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
'            StrSQL.AppendLine(" AND   PIVA    =  '" & Agro_SQL_SaveText(PIVA) & "'  ")


'            '--------------------------------------------------------------------------
'            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
'            '--------------------------------------------------------------------------

'        Catch ex As Exception
'            MessaggioErrore = ex.Message
'            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
'            xRisp = False
'            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
'        End Try

'        Return xRisp
'    End Function




'    Public Function Cancella( _
'                           ByVal PIVA As String, _
'                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                               ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.LFO_W.Cancella()"

'        '====================================================================================
'        'Parametri opzionali :
'        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
'        '   DirectoryLOG = ""           =>  viene usato il valore di default
'        '   FileLOG = ""                =>  viene usato il valore di default
'        '====================================================================================

'        Dim MessaggioErrore As String = ""
'        Dim StrSQL As New System.Text.StringBuilder
'        Dim xRisp As Boolean = False

'        Try
'            '---------------------------------------------
'            StrSQL.Length = 0
'            StrSQL.AppendLine("DELETE FROM PDC_Fornitori ")

'            StrSQL.AppendLine("  WHERE ")
'            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
'            StrSQL.AppendLine("         AND PIVA =  '" & Agro_SQL_SaveText(PIVA) & "'")

'            '--------------------------------------------------------------------------
'            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
'            '--------------------------------------------------------------------------

'        Catch ex As Exception

'            MessaggioErrore = ex.Message
'            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
'            xRisp = False
'            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

'        End Try

'        Return xRisp

'    End Function



'End Class
