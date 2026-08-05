Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PDC_CapitolatiCliente_Attivi_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_CapitolatoPrivato As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_CapitolatiCliente_Attivi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT *  ")
            StrSQL.AppendLine(" FROM PDC_CapitolatiCliente_Attivi ")
            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_CapitolatoPrivato <> 0 Then
                StrSQL.AppendLine(" AND ID_CapitolatoPrivato = " & Agro_SQL_SaveNum(ID_CapitolatoPrivato))
            End If

            'data distinta
            StrSQL.AppendLine(" AND Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  Des_Capitolato_Privato")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_DistinctUtilizzati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_CapitolatiCliente_Attivi_R.Leggi_DistinctUtilizzati()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT id_capitolatoprivato, Des_Capitolato_Privato ")
            StrSQL.AppendLine(" FROM PDC_CapitolatiCliente_Attivi  ")
            StrSQL.AppendLine(" ORDER BY  Des_Capitolato_Privato")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_CapitolatiCliente_Attivi_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal ID_CapitolatoPrivato As Integer,
                           ByVal Des_Capitolato_Privato As String,
                           ByVal Sigla_Capitolato_Privato As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_CapitolatiCliente_Attivi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_CapitolatiCliente_Attivi(PivaSuperUser , ID_PDC_Testata, ID_CapitolatoPrivato, ")
            StrSQL.AppendLine("         Des_Capitolato_Privato, Sigla_Capitolato_Privato, ")

            StrSQL.AppendLine("            Inviato, DataInvio, ")
            StrSQL.AppendLine("            Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("            Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_CapitolatoPrivato))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Des_Capitolato_Privato) & "'")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Sigla_Capitolato_Privato) & "'")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function
    
    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_CapitolatoPrivato As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_CapitolatiCliente_Attivi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_CapitolatiCliente_Attivi ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine("     AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_CapitolatoPrivato <> 0 Then
                StrSQL.AppendLine("     AND ID_CapitolatoPrivato = " & Agro_SQL_SaveNum(ID_CapitolatoPrivato))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
