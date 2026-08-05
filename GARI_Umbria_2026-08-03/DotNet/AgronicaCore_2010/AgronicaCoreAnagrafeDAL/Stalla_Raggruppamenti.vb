Imports System.Data.Entity
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.My.Resources

Public Class Stalla_Raggruppamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################
    Public Function Leggi_x_anagrafica(ByVal pivaSuperUser As String,
                                       ByVal piva As String,
                                       ByVal sa_cod As Integer,
                                       ByVal STA_NUM As Integer,
                                       ByVal Raggruppamento_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R.Leggi_x_anagrafica()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0

            Stb.AppendLine("SELECT Imprese.piva,  ")
            Stb.AppendLine(" Stalla_Raggruppamenti.piva + '_' + CAST(Stalla_Raggruppamenti.sa_cod as varchar) + '_' + CAST(Stalla_Raggruppamenti.STA_NUM as varchar) + '_' + CAST(Stalla_Raggruppamenti.Raggruppamento_Cod as varchar) as chiave,   ")
            Stb.AppendLine(" Imprese.rag_soc, ")
            Stb.AppendLine(" Centri_Aziendali.sa_cod, ")
            Stb.AppendLine(" Centri_Aziendali.sa_nome, ")
            Stb.AppendLine(" STALLA.STA_NUM, ")
            Stb.AppendLine(" STALLA.sta_des, ")
            Stb.AppendLine(" Stalla_Raggruppamenti.raggruppamento_cod, ")
            Stb.AppendLine(" Stalla_Raggruppamenti.raggruppamento_Des, ")
            Stb.AppendLine(" Stalla_Raggruppamenti.raggruppamento_Tipo, ")
            Stb.AppendLine(" CASE WHEN Stalla_Raggruppamenti.Flag_BDN = 1 THEN '" & Gias.Si & "' ELSE '" & Gias.No & "' END AS Flag_BDN, ")
            Stb.AppendLine(" Lista_Tipi_Raggruppamento_Stalla.Raggruppamento_Des as Raggruppamento_Tipo_Des, ")

            Stb.Append("Stalla_Raggruppamenti.Data_Creazione, ")
            Stb.Append("Stalla_Raggruppamenti.Data_Modifica, ")
            Stb.Append("ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Stalla_Raggruppamenti.Username_Creazione), Stalla_Raggruppamenti.Username_Creazione) AS Utente_Creazione, ")
            Stb.Append("ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Stalla_Raggruppamenti.Username_Modifica), Stalla_Raggruppamenti.Username_Modifica) AS Utente_Modifica ")

            Stb.AppendLine("FROM Stalla_Raggruppamenti ")
            Stb.AppendLine("LEFT JOIN Stalla ON Stalla_Raggruppamenti.piva = stalla.piva And Stalla_Raggruppamenti.sa_cod = stalla.sa_cod And Stalla_Raggruppamenti.Sta_Num = Stalla.STA_NUM ")
            Stb.AppendLine("LEFT JOIN Centri_Aziendali ON Stalla_Raggruppamenti.piva = Centri_Aziendali.piva And Stalla_Raggruppamenti.sa_cod = Centri_Aziendali.sa_cod ")
            Stb.AppendLine("LEFT JOIN Imprese ON Stalla_Raggruppamenti.piva = imprese.piva")
            Stb.AppendLine("LEFT JOIN Lista_Tipi_Raggruppamento_Stalla ON Lista_Tipi_Raggruppamento_Stalla.Raggruppamento_Cod = Stalla_Raggruppamenti.Raggruppamento_Tipo ")

            Stb.AppendLine("WHERE 1=1 ")

            If pivaSuperUser <> "" Then
                Stb.AppendLine(" AND Stalla_Raggruppamenti.pivaSuperUser = " & Agro_SQL_SaveText_NULL(pivaSuperUser) & " ")
            End If

            If piva <> "" Then
                Stb.AppendLine(" AND Stalla_Raggruppamenti.piva = " & Agro_SQL_SaveText_NULL(piva) & " ")
            End If

            If sa_cod <> 0 Then
                Stb.AppendLine(" AND Stalla_Raggruppamenti.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        Stb.AppendLine(" AND Stalla_Raggruppamenti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If STA_NUM <> 0 Then
                Stb.AppendLine(" AND Stalla_Raggruppamenti.STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & " ")
            End If

            If Raggruppamento_Cod <> 0 Then
                Stb.AppendLine(" AND Stalla_Raggruppamenti.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Stalla_Raggruppamenti.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Stalla_Raggruppamenti.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

Public Class Stalla_Raggruppamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(piva As String,
                      sa_cod As Integer,
                      Sta_num As Integer,
                      obj_raggruppamento As JObject,
                      GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim Raggruppamento_Cod = agroDP.NuovoId_Tabella_EF(GiasContext, "Stalla_Raggruppamenti", 0, 200000000, objParametri_Server)

        Dim Raggruppamento_Stalla As New AgronicaCoreEntityFramework_POCO.Stalla_Raggruppamenti

        ValidaRaggruppamento(Raggruppamento_Stalla, obj_raggruppamento, Raggruppamento_Cod, objParametri_Server)

        Raggruppamento_Stalla.Data_Creazione = DateTime.Now
        Raggruppamento_Stalla.Data_Modifica = DateTime.Now

        'se deve impostare il flag_BDN, entra nella gestione di quest'ultimo
        If Raggruppamento_Stalla.Flag_BDN = 1 Then
            GestioneRaggruppamento_FlagBDN(Raggruppamento_Stalla, GiasContext, objParametri_Server)
        End If

        GiasContext.Stalla_Raggruppamenti.Add(Raggruppamento_Stalla)
        GiasContext.SaveChanges()

    End Sub

    Public Sub Modifica(piva As String,
                        sa_cod As Integer,
                        Sta_num As Integer,
                        Raggruppamento_Cod As Integer,
                        obj_raggruppamento As JObject,
                        GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser

        Dim rsl = From rs In GiasContext.Stalla_Raggruppamenti
                  Where rs.PivaSuperUser = pivaSuperUser AndAlso
                        rs.PIVA = piva AndAlso
                        rs.sa_cod = sa_cod AndAlso
                        rs.STA_NUM = Sta_num AndAlso
                        rs.Raggruppamento_Cod = Raggruppamento_Cod
                  Select rs

        Dim Raggruppamento_Stalla = rsl.FirstOrDefault

        ValidaRaggruppamento(Raggruppamento_Stalla, obj_raggruppamento, Raggruppamento_Cod, objParametri_Server)

        'se deve impostare il flag_BDN, entra nella gestione di quest'ultimo
        If Raggruppamento_Stalla.Flag_BDN = 1 Then
            GestioneRaggruppamento_FlagBDN(Raggruppamento_Stalla, GiasContext, objParametri_Server)
        End If

        Raggruppamento_Stalla.Data_Modifica = DateTime.Now

        GiasContext.Entry(Raggruppamento_Stalla).State = EntityState.Modified
        GiasContext.SaveChanges()

    End Sub


    Private Sub ValidaRaggruppamento(ByRef Raggruppamento_Stalla As AgronicaCoreEntityFramework_POCO.Stalla_Raggruppamenti,
                                     ByRef obj_raggruppamento As JObject,
                                     ByRef Raggruppamento_Cod As Integer,
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Raggruppamento_Stalla.PivaSuperUser = objParametri_Server.PivaSuperUser
        Raggruppamento_Stalla.PIVA = obj_raggruppamento("PIVA")
        Raggruppamento_Stalla.sa_cod = obj_raggruppamento("sa_cod")

        Raggruppamento_Stalla.STA_NUM = obj_raggruppamento("STA_NUM")
        Raggruppamento_Stalla.Raggruppamento_Cod = Raggruppamento_Cod
        Raggruppamento_Stalla.Raggruppamento_Des = obj_raggruppamento("Raggruppamento_Des")
        Raggruppamento_Stalla.Raggruppamento_Tipo = obj_raggruppamento("Raggruppamento_Tipo")


        Raggruppamento_Stalla.Username_Creazione = objParametri_Server.UsernameOperazione
        Raggruppamento_Stalla.Username_Modifica = objParametri_Server.UsernameOperazione

        Raggruppamento_Stalla.Validita_Inizio = CDate(obj_raggruppamento("Validita_Inizio"))
        Raggruppamento_Stalla.Validita_Fine = CDate(obj_raggruppamento("Validita_Fine"))

        Raggruppamento_Stalla.Codice = obj_raggruppamento("Codice")
        Raggruppamento_Stalla.SPE_COD = obj_raggruppamento("SPE_COD")

        Raggruppamento_Stalla.Flag_BDN = obj_raggruppamento("Flag_BDN")

        Raggruppamento_Stalla.RAZ_COD = 0
        If Not IsNothing(obj_raggruppamento("RAZ_COD").ToString) AndAlso IsNumeric(obj_raggruppamento("RAZ_COD").ToString) Then
            Raggruppamento_Stalla.RAZ_COD = obj_raggruppamento("RAZ_COD").ToString
        End If

        Raggruppamento_Stalla.STATO_COD = 0
        If Not IsNothing(obj_raggruppamento("STATO_COD").ToString) AndAlso IsNumeric(obj_raggruppamento("STATO_COD").ToString) Then
            Raggruppamento_Stalla.STATO_COD = obj_raggruppamento("STATO_COD")
        End If

        Raggruppamento_Stalla.Mq = 0
        If Not IsNothing(obj_raggruppamento("Mq").ToString) AndAlso IsNumeric(obj_raggruppamento("Mq").ToString) Then
            Raggruppamento_Stalla.Mq = obj_raggruppamento("Mq")
        End If

    End Sub

    ''' <summary>
    ''' Gestione del Flag_BDN: nel caso siano già presenti uno o più raggruppamenti col flag impostato,
    ''' verranno impostati a 0 poichè per una stalla solo un raggruppamento può averlo valorizzato
    ''' </summary>
    ''' <param name="Raggruppamento_Stalla"></param>
    ''' <param name="GiasContext"></param>
    ''' <param name="objParametri_Server"></param>
    Private Sub GestioneRaggruppamento_FlagBDN(ByRef Raggruppamento_Stalla As AgronicaCoreEntityFramework_POCO.Stalla_Raggruppamenti,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim pivaSuperUser As String = Raggruppamento_Stalla.PivaSuperUser
        Dim piva As String = Raggruppamento_Stalla.PIVA
        Dim sa_cod As Integer = Raggruppamento_Stalla.sa_cod
        Dim Sta_num As Integer = Raggruppamento_Stalla.STA_NUM

        'tutti i raggruppamenti di quella stalla col flag impsotato
        Dim raggrStalla_FlagImpostato = (From rs In GiasContext.Stalla_Raggruppamenti
                                         Where rs.PivaSuperUser = pivaSuperUser AndAlso
                                               rs.PIVA = piva AndAlso
                                               rs.sa_cod = sa_cod AndAlso
                                               rs.STA_NUM = Sta_num AndAlso
                                               rs.Flag_BDN = 1
                                         Select rs).ToList

        'se non ne esistono col flag impostato in quella stalla, esce
        If IsNothing(raggrStalla_FlagImpostato) OrElse raggrStalla_FlagImpostato.Count = 0 Then
            Exit Sub

        End If

        'se è l'unico col campo impostato in quella stalla, esce
        If raggrStalla_FlagImpostato.Count = 1 AndAlso raggrStalla_FlagImpostato.First.Raggruppamento_Cod = Raggruppamento_Stalla.Raggruppamento_Cod Then
            Exit Sub

        End If

        'per ognuno dei raggruppamenti trovati col flag impostato, lo imposta a 0
        For Each rs In raggrStalla_FlagImpostato
            rs.Flag_BDN = 0
            rs.Data_Modifica = DateTime.Now

            GiasContext.Entry(rs).State = EntityState.Modified

        Next
        GiasContext.SaveChanges()

    End Sub

    Public Sub Cancella(piva As String,
                      sa_cod As Integer,
                      Sta_num As Integer,
                      Raggruppamento_Cod As Integer,
                      GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim raggruppamento = (From g In GiasContext.Stalla_Raggruppamenti Where g.PIVA = piva And
                                                                              g.sa_cod = sa_cod And
                                                                              g.STA_NUM = Sta_num And
                                                                              g.Raggruppamento_Cod = Raggruppamento_Cod).FirstOrDefault()

        GiasContext.Stalla_Raggruppamenti.Remove(raggruppamento)
        GiasContext.SaveChanges()

    End Sub

End Class