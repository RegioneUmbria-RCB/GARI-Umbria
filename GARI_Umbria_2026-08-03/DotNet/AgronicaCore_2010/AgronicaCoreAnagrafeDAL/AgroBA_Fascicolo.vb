


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class AgroBA_Fascicolo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function getFascicoloParticelle( _
            ByVal cuaa As String, _
            ByVal NumeroValidazione As String, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0



            stb.Append(" select  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("   provincia " & vbCrLf)
            stb.Append("  ,comune " & vbCrLf)
            stb.Append("  ,sezione " & vbCrLf)
            stb.Append("  ,foglio " & vbCrLf)
            stb.Append("  ,particella " & vbCrLf)
            stb.Append("  ,subalterno " & vbCrLf)
            stb.Append("  ,superficiecatastale " & vbCrLf)
            stb.Append("  ,superficiecondotta " & vbCrLf)
            stb.Append("  ,datainizioconduzione " & vbCrLf)
            stb.Append("  ,datafineconduzione " & vbCrLf)
            stb.Append("  ,mu.superficieutilizzata as Destinazione_superficieutilizzata " & vbCrLf)
            stb.Append("  ,mu.codicemacrouso as  Destinazione_codicemacrouso " & vbCrLf)
            stb.Append("  ,coalesce(u.superficieutilizzata, '0') as USO_superficieutilizzata " & vbCrLf)
            stb.Append("  ,coalesce(u.codiceprodotto, '') as USO_codiceprodotto")


            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from SIGPA_CatastoImportato_TabellaParticelle p " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join SIGPA_CatastoImportato_TabellaMacroUsi mu " & vbCrLf)
            stb.Append("        on p.id_cons  = mu.id_cons  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join SIGPA_CatastoImportato_TabellaUsi u " & vbCrLf)
            stb.Append("        on u.iden_destinaz = mu.iden_destinaz  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where p.num_validazione = '" & Agro_SQL_SaveText(NumeroValidazione) & "' " & vbCrLf)
            stb.Append(" and p.cuaa_azienda = '" & Agro_SQL_SaveText(cuaa) & "' " & vbCrLf)







            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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
    Public Function getSchedeFascicolo( _
            ByVal CUAA As String, _
            ByVal DataDa As DateTime, _
            ByVal DataA As DateTime, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" select distinct num_validazione, data_validazione  " & vbCrLf)
            stb.Append(" from SIGPA_CatastoImportato_FascicoliExcel  " & vbCrLf)
            stb.Append(" where cuaa_azienda = '" & Agro_SQL_SaveText(CUAA) & "'" & vbCrLf)
            stb.Append(" and  data_validazione <=  " & Agro_SQL_SaveDate(DataA) & "" & vbCrLf)
            stb.Append(" and  data_validazione  >= " & Agro_SQL_SaveDate(DataDa) & " " & vbCrLf)
            stb.Append(" order by data_validazione ")

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function getFascicoliProcessati( _
        ByVal cuaa As String, _
        ByVal NumeroValidazione As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_R.getFascicoliProcessati()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" select * " & vbCrLf)
            stb.Append(" from SIGPA_CatastoImportato_FascicoliProcessati " & vbCrLf)
            stb.Append(" where 1=1 ")

            If cuaa <> "" Then
                stb.Append(" and cuaa_azienda = '" & Agro_SQL_SaveText(cuaa) & "' " & vbCrLf)
            End If

            If NumeroValidazione <> "" Then
                stb.Append(" and num_validazione = '" & Agro_SQL_SaveText(NumeroValidazione) & "' " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT



    End Function

    Public Function LeggiAziende_PresentiSIGPA_NONGias(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_R.LeggiAziendePresentiSIGPANONGias()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" select distinct  i.cuaa_azienda, i.rag_soc " & vbCrLf)
            stb.Append(" from SIGPA_CatastoImportato_FascicoliExcel i  " & vbCrLf)
            stb.Append("  left join AGRONICA_NAZIONALE_SERVER_PROD.dbo.Imprese_Codici ic " & vbCrLf)
            stb.Append("  on i.cuaa_azienda = ic.val_cod  " & vbCrLf)
            stb.Append("  and ic.id_cod = 1010  " & vbCrLf)
            stb.Append(" where ic.PIVA is null ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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

Public Class AgroBA_Fascicolo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal cuaa_azienda As String, _
                            ByVal num_validazione As String, _
                            ByVal data_validazione As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO SIGPA_CatastoImportato_FascicoliProcessati( ")
            StrSQL.Append("            cuaa_azienda, num_validazione, data_validazione ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(cuaa_azienda) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(num_validazione) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(data_validazione) & "  ")
            StrSQL.Append(")")

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


