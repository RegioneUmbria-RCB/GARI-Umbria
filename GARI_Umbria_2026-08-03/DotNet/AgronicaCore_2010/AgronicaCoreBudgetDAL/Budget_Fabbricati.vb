Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Public Class Budget_Fabbricati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Magazzini_Organismoreferente(ByVal Id_Budget As Integer,
                                                       ByVal Piva As String,
                                                       ByVal Sa_Cod As Integer,
                                                       ByVal Fabbricato_Cod As Integer,
                                                       ByVal Tipo_Fabbricato_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreBudgetDAL.Budget_Fabbricati_R.Leggi_Magazzini_Organismoreferente()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0


            StrSQL.Append(" SELECT  contatti.rag_soc ,  Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Budget_Fabbricati.Fabbricato_Cod, Budget_Fabbricati.Fabbricato_Des,   " + vbCrLf)
            StrSQL.Append("  Budget_Fabbricati.Indirizzo_Cod, Budget_Fabbricati.PROV, Budget_Fabbricati.COM, Budget_Fabbricati.SEZIONE, Budget_Fabbricati.FOGLIO, Budget_Fabbricati.NUMERO, Budget_Fabbricati.SUBALTERNO,   " + vbCrLf)
            StrSQL.Append("  Budget_Fabbricati.MC_Convenzionale, Budget_Fabbricati.MC_Conversione, Budget_Fabbricati.MC_Biologico, Budget_Fabbricati.Regolamento_Cod, Budget_Fabbricati.TitoloPossesso,  " + vbCrLf)
            StrSQL.Append("   Budget_Fabbricati.Conversione_Inizio, Budget_Fabbricati.Conversione_Fine, Budget_Fabbricati.Idoneo_Costruzione, Budget_Fabbricati.Idoneo_SeparazAmbienti,  " + vbCrLf)
            StrSQL.Append("  Budget_Fabbricati.Idoneo_SeparazProdotti, Budget_Fabbricati.Idoneo_CondIgieniche, Budget_Fabbricati.Idoneo_AutorizSanitaria, Budget_Fabbricati.Idoneo_HACCP,   " + vbCrLf)
            StrSQL.Append("  Budget_Fabbricati.Idoneo_Planimetria, Budget_Fabbricati.Idoneo_Layout, Budget_Fabbricati.Idoneo_DiagrammiFlusso, Budget_Fabbricati.Idoneo_CDX_M004,   " + vbCrLf)
            StrSQL.Append("  Budget_Fabbricati.Idoneo_SupMinCoperte, Budget_Fabbricati.Idoneo_SupMinScoperte, Budget_Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des, " + vbCrLf)
            StrSQL.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " + vbCrLf)
            StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.REG, '') AS REG  " + vbCrLf)


            StrSQL.Append(" FROM    Budget_Fabbricati " + vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Budget_Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " + vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Budget_Fabbricati.PIVA = Centri_Aziendali.PIVA AND Budget_Fabbricati.SA_COD = Centri_Aziendali.sa_cod" + vbCrLf)

            StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Budget_Fabbricati.Indirizzo_Cod " + vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " + vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV " + vbCrLf)

            StrSQL.Append(" INNER JOIN Contatti ON Contatti.cod_contatto = Budget_Fabbricati.piva" & vbCrLf)
            StrSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.cod_contatto = Risorse_Umane.cod_contatto AND Contatti.piva = Risorse_Umane.piva and cod_rapporto= " & CStr(COD_ORGANISMO_REFERENTE) & vbCrLf)
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append(" " & vbCrLf)

            StrSQL.Append(" WHERE Budget_Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " + vbCrLf)
            StrSQL.Append(" AND   Budget_Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " + vbCrLf)

            If Id_Budget <> 0 Then
                StrSQL.Append(" AND     (Budget_Fabbricati.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & ")   " + vbCrLf)
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND    (Budget_Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " + vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (Budget_Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   " + vbCrLf)
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND (Budget_Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                    End If
                End If
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Budget_Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")   " + vbCrLf)
            End If

            If Tipo_Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Budget_Fabbricati.Tipo_Fabbricato_Cod = " & Agro_SQL_SaveNum(Tipo_Fabbricato_Cod) & ")   " + vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Budget_Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Budget_Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY contatti.rag_soc, Budget_Fabbricati.Fabbricato_Des ")
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

End Class

Public Class Budget_Fabbricati_W
    Inherits AgronicaCoreDataProvider.DataProvider

End Class