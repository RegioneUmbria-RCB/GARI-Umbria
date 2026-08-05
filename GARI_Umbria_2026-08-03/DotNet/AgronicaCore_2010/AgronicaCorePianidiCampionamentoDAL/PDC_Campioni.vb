Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class PDC_Campioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Prefisso_Terremerse(ByVal Id_PDC_Testata As Int32,
                               ByVal ID_Pdc_Dettagli As Int32,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As String
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Prefisso_Terremerse()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim prefisso As String

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            'StrSQL.AppendLine(" SELECT isnull( cast(i.val_cod as nvarchar(25)) , '') ")
            'StrSQL.AppendLine(" AS codice ")
            'StrSQL.AppendLine(" FROM pdc_dettagli p ")
            'StrSQL.AppendLine(" LEFT JOIN Imprese_Codici i ON id_cod = 1091 AND i.piva = p.piva ")
            'StrSQL.AppendLine(" WHERE Id_PDC_Testata = " & Id_PDC_Testata)
            'StrSQL.AppendLine(" AND ID_Pdc_Dettagli = " & ID_Pdc_Dettagli)

            ''--------------------------------------------------------------------------
            'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            ''--------------------------------------------------------------------------

            'prefisso = DT.Rows(0).Item("codice")

            '  Marco Grilli, 11/08/2016 12:41:31: se non ho trovato il codice fornitore 2, metto la ragione sociale
            'If prefisso = "" Then
            ' Grilli 31/05/2017 Saragoni vuole solo la ragione sociale e non più il codice fornitore
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT i.rag_soc ")
            StrSQL.AppendLine(" FROM pdc_dettagli p ")
            StrSQL.AppendLine(" INNER JOIN imprese i ON i.piva = p.piva ")
            StrSQL.AppendLine(" WHERE p.Id_PDC_Testata = " & Id_PDC_Testata)
            StrSQL.AppendLine(" AND p.ID_Pdc_Dettagli = " & ID_Pdc_Dettagli)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            prefisso = DT.Rows(0).Item("rag_soc")

            'End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return prefisso

    End Function

    Public Function Prefisso_Terremerse_Acquisti(ByVal Id_PDC_Testata As Int32,
                           ByVal ID_Pdc_Dettagli As Int32,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As String
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Prefisso_Terremerse_Acquisti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim prefisso As String

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT c.rag_soc ")
            StrSQL.AppendLine(" FROM pdc_dettagli p ")
            StrSQL.AppendLine(" INNER JOIN contatti c ON p.piva = c.Cod_Contatto  ")
            StrSQL.AppendLine(" WHERE p.Id_PDC_Testata = " & Id_PDC_Testata)
            StrSQL.AppendLine(" AND p.ID_Pdc_Dettagli = " & ID_Pdc_Dettagli)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            prefisso = DT.Rows(0).Item("rag_soc")

            'End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return prefisso

    End Function

    'Public Function Progressivo_Terremerse(ByVal Id_PDC_Testata As Int32, _
    '                           ByVal ID_Pdc_Dettagli As Int32, _
    '                           ByVal Anno As Integer, _
    '                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                          ) As String
    '    Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Progressivo_Terremerse()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        StrSQL.Length = 0
    '        '---------------------------------------------

    '        'StrSQL.AppendLine(" select isnull((a.val_cod + a.Cultivar_Coltiva),'???') codice ")
    '        'StrSQL.AppendLine(" from ( ")
    '        'StrSQL.AppendLine(" select i.val_cod , (select top 1 c.Cultivar_Coltiva from CAC_Codifica_Cultivar c where p.Veg_Cod = p.Veg_Cod and p.Cul_Cod = c.Cultivar_Gias and p.Regolamento_Cod = c.reg_cod) as Cultivar_Coltiva ")
    '        'StrSQL.AppendLine(" from pdc_dettagli p ")
    '        'StrSQL.AppendLine(" Left join Imprese_Codici i on id_cod = 1091 and i.piva = p.piva ")
    '        'StrSQL.AppendLine(" where 1=1")
    '        'StrSQL.AppendLine("  AND Id_PDC_Testata = " & Id_PDC_Testata & "")
    '        'StrSQL.AppendLine("  AND ID_Pdc_Dettagli = " & ID_Pdc_Dettagli & "")
    '        'StrSQL.AppendLine(" ) a ")



    '        StrSQL.AppendLine("select isnull( cast(i.val_cod as nvarchar(25)) +'_' , '') +   ")
    '        StrSQL.AppendLine("    (select cast((isnull(max(Codice_Progressivo_Inizio_Anno),0) + 1 ) as nvarchar(25)) + '_' + ")
    '        StrSQL.AppendLine(" '" & Anno & "' from PDC_Campioni pc  where pc.Codice_Anno = " & Anno & ") ")
    '        StrSQL.AppendLine("  as codice ")
    '        StrSQL.AppendLine(" from pdc_dettagli p ")
    '        StrSQL.AppendLine(" Left join Imprese_Codici i on id_cod = 1091 and i.piva = p.piva ")
    '        StrSQL.AppendLine(" where(1 = 1)")
    '        StrSQL.AppendLine("  AND Id_PDC_Testata = " & Id_PDC_Testata & "")
    '        StrSQL.AppendLine("  AND ID_Pdc_Dettagli = " & ID_Pdc_Dettagli & "")




    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT.Rows(0).Item("codice")

    'End Function



    Public Function Progressivo_Inizio_Anno(ByVal Anno As Int32,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Integer
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim pro As Integer
        Try
            StrSQL.Length = 0
            '---------------------------------------------

            Dim oo As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim Codice = oo.NuovoId_Tabella("PDC_Campioni_" & Anno, 0, 2000000000, objParametri)
            pro = Codice

            If (Codice = 1) Then

                StrSQL.AppendLine(" select isnull(MAX(codice_progressivo_inizio_anno),0)+1 as Progressivo from pdc_campioni ")
                StrSQL.AppendLine(" WHERE PDC_Campioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine("  AND (dbo.pdc_campioni.codice_anno = " & Anno & ")")

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                pro = DT.Rows(0).Item("Progressivo")

                StrSQL.Clear()
                StrSQL.Append(" UPDATE Sequenza_Tabelle ")
                StrSQL.Append(" SET Ultimo_Valore = " & Agro_SQL_SaveNum(pro) & " ")
                StrSQL.Append(" WHERE  Nome_Tabella = '" & Agro_SQL_SaveText(LCase("PDC_Campioni_" & Anno)) & "' ")

                '--------------------------------------------------------------------------
                EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)


            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return pro

    End Function


    Public Function Leggi(ByVal ID_PDC_Testata As Int32,
                          ByVal ID_PDC_Dettagli As Int32,
                          ByVal ID_PDC_Campione As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_Campioni.*, ISNULL((PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo),'') ID_PuntoDiPrelievo, ISNULL((PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo),'') Descrizione_PuntoDiPrelievo ")
            StrSQL.AppendLine("        , ISNULL((PDC_MotivoCampionamento.ID_MotivoCampionamento),'') ID_Motivo_Campione, ISNULL((PDC_MotivoCampionamento.Descrizione_MotivoCampionamento),'') Descrizione_Motivo_Campione ")

            StrSQL.AppendLine(" FROM PDC_Campioni ")
            StrSQL.AppendLine(" LEFT JOIN PDC_PuntoDiPrelievo ON PDC_Campioni.puntoprelievo = PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo ")
            StrSQL.AppendLine(" LEFT JOIN PDC_MotivoCampionamento ON PDC_Campioni.Motivo_Campione = PDC_MotivoCampionamento.ID_MotivoCampionamento ")

            StrSQL.AppendLine(" WHERE PDC_Campioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If
            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggixAnalisiZoo(ByVal Matricola As String,
                                     ByVal Data_Campionamento As Date,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.LeggixAnalisiZoo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_Campioni.ID_PDC_Testata, PDC_Campioni.ID_PDC_Dettagli, PDC_Campioni.ID_PDC_Campione, Analisi_Testata_Cod ")

            StrSQL.AppendLine(" FROM PDC_Campioni ")

            StrSQL.AppendLine("INNER JOIN PDC_Dettagli ON PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli")
            StrSQL.AppendLine("INNER JOIN PDC_Analisi ON PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione")

            StrSQL.AppendLine(" WHERE PDC_Campioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND Matricola = '" & Agro_SQL_SaveText(Matricola) & "'")
            StrSQL.AppendLine(" AND Data_Campionamento = " & Agro_SQL_SaveDate(Data_Campionamento))

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

    Public Function LeggixAnalisiZooDaAnalizzare(ByVal Matricola As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.LeggixAnalisiZoo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_Campioni.ID_PDC_Testata, PDC_Campioni.ID_PDC_Dettagli, PDC_Campioni.ID_PDC_Campione, Analisi_Testata_Cod ")

            StrSQL.AppendLine(" FROM PDC_Campioni ")

            StrSQL.AppendLine("INNER JOIN PDC_Dettagli ON PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli")
            StrSQL.AppendLine("INNER JOIN PDC_Analisi ON PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione")

            StrSQL.AppendLine(" WHERE PDC_Campioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND Matricola = '" & Agro_SQL_SaveText(Matricola) & "'")
            StrSQL.AppendLine(" AND PDC_Analisi.PDC_Stato_Analisi = 1 ")

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


    Public Function LeggiCampionixRichiestaAnalisi(ByVal ID_PDC_Testata As Int32,
                                                   ByVal ID_PDC_Dettagli As Int32,
                                                   ByVal ID_PDC_Campione As Int32,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.LeggiCampionixRichiestaAnalisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_Campioni.*, ISNULL((PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo),'') ID_PuntoDiPrelievo, ISNULL((PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo),'') Descrizione_PuntoDiPrelievo ")
            StrSQL.AppendLine("        , ISNULL((PDC_MotivoCampionamento.ID_MotivoCampionamento),'') ID_Motivo_Campione, ISNULL((PDC_MotivoCampionamento.Descrizione_MotivoCampionamento),'') Descrizione_Motivo_Campione ")
            StrSQL.AppendLine("        , ISNULL(PDC_Analisi.PDC_Stato_Analisi, 0) ID_Stato_Analisi ")

            StrSQL.AppendLine(" FROM PDC_Campioni ")
            StrSQL.AppendLine(" LEFT JOIN PDC_PuntoDiPrelievo ON PDC_Campioni.puntoprelievo = PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo ")
            StrSQL.AppendLine(" LEFT JOIN PDC_MotivoCampionamento ON PDC_Campioni.Motivo_Campione = PDC_MotivoCampionamento.ID_MotivoCampionamento ")
            StrSQL.AppendLine(" LEFT JOIN PDC_Analisi ON PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata ")
            StrSQL.AppendLine("     AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli ")
            StrSQL.AppendLine("     AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione ")

            StrSQL.AppendLine(" WHERE PDC_Campioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If
            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    'Public Function LeggiCampioni_Ancora_Non_Analizzati(
    '                              ByVal xFiltroAggiuntivo As String, _
    '                              ByVal xOrderBy As String, _
    '                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                              ) As DataTable
    '    Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Campioni_R.Leggi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        StrSQL.Length = 0
    '        '---------------------------------------------

    '        StrSQL.AppendLine(" SELECT * ")
    '        StrSQL.AppendLine(" FROM PDC_Campioni ")
    '        StrSQL.AppendLine(" WHERE PDC_Campioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

    '        If ID_PDC_Testata <> 0 Then
    '            StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
    '        End If
    '        If ID_PDC_Dettagli <> 0 Then
    '            StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
    '        End If
    '        If ID_PDC_Campione <> 0 Then
    '            StrSQL.AppendLine(" AND ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
    '        End If


    '        '--------------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_Campioni_W
    Inherits AgronicaCoreDataProvider.DataProvider



#Region "Scrivi"

    Public Function Scrivi(
                           ByVal ID_PDC_Testata As Int32,
                           ByVal ID_PDC_Dettagli As Int32,
                           ByVal ID_PDC_Campione As Int32,
                           ByVal PDC_Campione_Des As String,
                           ByVal ID_PDC_Stato_Campione As Int32,
                           ByVal Data_Campionamento As Date,
                           ByVal Codice_Campione As String,
                           ByVal PuntoPrelievo As String,
                           ByVal Note_Campione As String,
                           ByVal Tecnico_Campione As String,
                           ByVal Codice_Progressivo_Inizio_Anno As Integer,
                           ByVal Codice_Anno As Integer,
                           ByVal Tipo_Campione As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Motivo_Campione As Integer = 0,
                           Optional ByVal Num_Prodotti As Integer = 0,
                           Optional ByVal Lat_Campione As String = "",
                           Optional ByVal Long_Campione As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.PDC_W.Scrivi_Campione()"

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
            StrSQL.AppendLine("INSERT INTO PDC_Campioni(PivaSuperUser , ID_PDC_Testata, ID_PDC_Dettagli, ID_PDC_Campione, ")
            StrSQL.AppendLine("         PDC_Campione_Des, ID_PDC_Stato_Campione, Data_Campionamento, ")
            StrSQL.AppendLine("         Codice_Campione , Note_Campione , Tecnico_Campione, PuntoPrelievo, Tipo_Campione, Codice_Progressivo_Inizio_Anno, Codice_Anno,")
            StrSQL.AppendLine("         Motivo_Campione, X, Y, Num_Prodotti, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Campione))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PDC_Campione_Des) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Stato_Campione))
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(Data_Campionamento) & "")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Codice_Campione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Note_Campione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Tecnico_Campione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(If(PuntoPrelievo = "undefined", "", PuntoPrelievo)) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Campione) & "")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Codice_Progressivo_Inizio_Anno) & "")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Codice_Anno) & "")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Motivo_Campione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Lat_Campione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Long_Campione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Num_Prodotti))
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.AppendLine(")")

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


    Public Shared Function GeneraCodiceCampione(ByVal ID_PDC_Testata As Int32,
                           ByVal ID_PDC_Dettagli As Int32,
                           ByVal ID_PDC_Campione As Int32,
                           ByVal Data_Campionamento As Date) As String
        Dim risp As String = ""
        'risp = Data_Campionamento.Year.ToString()
        'risp += Data_Campionamento.Month.ToString()
        'risp += Data_Campionamento.Day.ToString()
        risp += ID_PDC_Testata.ToString()
        risp += ID_PDC_Dettagli.ToString()
        risp += ID_PDC_Campione.ToString()
        Return risp
    End Function
#End Region

#Region "Modifica"
    Public Function Modifica(
                           ByVal ID_PDC_Testata As Int32,
                           ByVal ID_PDC_Dettagli As Int32,
                           ByVal ID_PDC_Campione As Int32,
                           ByVal Data_Campionamento As Date,
                           ByVal Codice_Campione As String,
                           ByVal Note_Campione As String,
                           ByVal Codice_Progressivo_Inizio_Anno As Integer,
                           ByVal Codice_Anno As Integer,
                           ByVal PuntoPrelievo As String,
                           ByVal Tecnico_Campione As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Lat_Campione As String = "",
                           Optional ByVal Long_Campione As String = ""
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.PDC_W.Modifica()"

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
            StrSQL.AppendLine("update PDC_Campioni set ")
            StrSQL.AppendLine("         Data_Campionamento = " & Agro_SQL_SaveDate(Data_Campionamento) & "")
            StrSQL.AppendLine("         ,Codice_Campione = '" & Agro_SQL_SaveText(Codice_Campione) & "' ")
            StrSQL.AppendLine("         ,Note_Campione = '" & Agro_SQL_SaveText(Note_Campione) & "' ")
            StrSQL.AppendLine("         ,Codice_Progressivo_Inizio_Anno = " & Agro_SQL_SaveNum(Codice_Progressivo_Inizio_Anno) & " ")
            StrSQL.AppendLine("         ,Codice_Anno = " & Agro_SQL_SaveNum(Codice_Anno) & " ")

            If PuntoPrelievo <> "" AndAlso PuntoPrelievo <> "undefined" Then
                StrSQL.AppendLine("         ,PuntoPrelievo = " & Agro_SQL_SaveNum(PuntoPrelievo) & " ")
            End If
            StrSQL.AppendLine("         ,Tecnico_Campione = '" & Agro_SQL_SaveText(Tecnico_Campione) & "' ")
            StrSQL.AppendLine("         ,X = " & Agro_SQL_SaveNum(Lat_Campione))
            StrSQL.AppendLine("         ,Y = " & Agro_SQL_SaveNum(Long_Campione))


            StrSQL.AppendLine("         where ")


            StrSQL.AppendLine("        PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("        and ID_PDC_Testata=  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("        and ID_PDC_Dettagli=  " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            StrSQL.AppendLine("        and ID_PDC_Campione=  " & Agro_SQL_SaveNum(ID_PDC_Campione))

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

#End Region

#Region "Cancellazione"

    Public Function Cancella(
                           ByVal ID_PDC_Testata As Int32,
                           ByVal ID_PDC_Dettagli As Int32,
                           ByVal ID_PDC_Campione As Int32,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnalisiDal.PDC_Campioni_W.Cancella()"

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
            StrSQL.AppendLine("DELETE FROM PDC_Campioni ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine("     AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If
            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine("     AND ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
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



#End Region



End Class




'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
