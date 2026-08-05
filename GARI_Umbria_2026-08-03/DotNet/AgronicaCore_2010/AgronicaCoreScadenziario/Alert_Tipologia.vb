Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Alert_Tipologia_R
    Inherits AgronicaCoreDataProvider.DataProvider

    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################


    Public Function Leggi(ByVal ID_Area As Integer,
                          ByVal ID_Tipologia As Integer,
                          ByVal Piva As String,
                          ByVal soloPrivate As Boolean,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal controllaSeUtenteAutorizzato As Boolean? = True,
                          Optional ByVal tipoPermessoDaControllare As Integer = 2,
                          Optional ByVal listaIndici As String = "",
                          Optional ByVal xMultiSelect As Boolean = False
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaSuperUser = objParametri.PivaSuperUser


        Dim bPermessixUtente As Boolean = False 'Booleano per la presenza di record in tabella obj_CategTipologiaDocumentiXUtenti
        'In caso non esistano record viene bypassato il filtro

        Dim iLivelloGerarchia As Integer = 0 'Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

        Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

        Try

            If controllaSeUtenteAutorizzato Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Se_Vengono_Gestiti_Permessi(iLivelloGerarchia, bPermessixUtente, Piva, objParametri)

            End If


            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Alert_Tipologia.*")

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_SELECT(StrSQL, iLivelloGerarchia)

            End If


            StrSQL.AppendLine(" FROM  Alert_Tipologia ")

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_JOIN(StrSQL, iLivelloGerarchia,
                                                                            Piva, False, objParametri)
            End If


            StrSQL.AppendLine(" WHERE Alert_Tipologia.PivaSuperUser = '" & PivaSuperUser & "' ")


            If ID_Area <> 0 Then
                StrSQL.AppendLine(" AND Alert_Tipologia.ID_Area = " & Agro_SQL_SaveNum_NULL(ID_Area))
            End If

            If ID_Tipologia <> 0 And xMultiSelect = False Then
                StrSQL.AppendLine(" AND Alert_Tipologia.ID_Tipologia = " & Agro_SQL_SaveNum_NULL(ID_Tipologia))
            End If

            'Anna 29/04/22: aggiunti campi al filtro di ricerca
            If listaIndici <> "" And xMultiSelect = True Then
                Dim count As Integer = (listaIndici.Split(",")).Count
                StrSQL.AppendLine(" AND  Alert_Tipologia.ID_Tipologia IN (  ")
                StrSQL.AppendLine("                       SELECT Alert_IndicexTipologia.ID_Tipologia ")
                StrSQL.AppendLine("                       FROM Alert_IndicexTipologia  ")
                StrSQL.AppendLine("                       WHERE ID_Tipologia IN (")
                StrSQL.AppendLine("                             SELECT Alert_IndicexTipologia.ID_Tipologia")
                StrSQL.AppendLine("                             FROM Alert_IndicexTipologia ")
                StrSQL.AppendLine("                             WHERE ID_Indice  IN (" & Agro_SQL_Save_Clausola_IN(listaIndici) & ")")
                StrSQL.AppendLine("                             AND Alert_IndicexTipologia.ID_Tipologia <> " & Agro_SQL_SaveNum(ID_Tipologia))
                StrSQL.AppendLine("                             AND Alert_IndicexTipologia.ID_Tipologia IN (")
                StrSQL.AppendLine("                                                        SELECT ID_Tipologia FROM Alert_IndicexTipologia	")
                StrSQL.AppendLine("                                                        WHERE ID_Tipologia <> " & (ID_Tipologia))
                If count > 1 Then
                    StrSQL.AppendLine("                                                        AND ID_Indice IN (" & Agro_SQL_Save_Clausola_IN(listaIndici) & ")")
                End If
                StrSQL.AppendLine("                                                        AND ID_Area = " & Agro_SQL_SaveNum(ID_Area))
                StrSQL.AppendLine("                                                        GROUP BY ID_Tipologia")
                StrSQL.AppendLine("                                                        HAVING COUNT(ID_Indice) = " & Agro_SQL_SaveNum(count) & " ")
                StrSQL.AppendLine("                                                        )")
                StrSQL.AppendLine("                       ) ")
                StrSQL.AppendLine(" ) ")

            ElseIf listaIndici = "" And xMultiSelect = True Then
                StrSQL.AppendLine(" AND  Alert_Tipologia.ID_Tipologia NOT IN (  ")
                StrSQL.AppendLine("                                 SELECT ID_Tipologia FROM Alert_IndicexTipologia	")
                StrSQL.AppendLine("                                 WHERE ID_Tipologia <> " & (ID_Tipologia))
                StrSQL.AppendLine("                                 AND ID_Area = " & Agro_SQL_SaveNum(ID_Area))
                StrSQL.AppendLine("                                 GROUP BY ID_Tipologia")
                StrSQL.AppendLine("                                 HAVING COUNT(ID_Indice) > 0 ")
                StrSQL.AppendLine("                                 )")
                StrSQL.AppendLine("AND Alert_Tipologia.ID_Tipologia <> " & (ID_Tipologia))
            End If

            'If Piva <> "" Then
            '    StrSQL.AppendLine(" AND Alert_Tipologia.Piva In ('', " & Agro_SQL_SaveText_NULL(Piva) & ") ")
            'End If

            'If soloPrivate Then
            '    StrSQL.AppendLine(" AND Alert_Tipologia.ID_Tipologia >=0 OR Alert_Tipologia.Piva IS NULL")
            'End If

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_WHERE(StrSQL, iLivelloGerarchia, tipoPermessoDaControllare, False)

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_ORDER_BY(StrSQL, False)

            End If

            'Anna 29/04/22: aggiunti campi al filtro di ricerca
            If bPermessixUtente = False Then
                StrSQL.AppendLine(" ORDER BY Alert_Tipologia.Nome ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_DataTable(DT, tipoPermessoDaControllare, False)

            End If


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function LeggiIdDaNomeTipologia(ByVal ID_Area As Integer,
                                           ByVal NomeTipologia As String,
                                           ByVal Piva As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Integer?

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_R.LeggiIdDaNomeTipologia()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim id As Integer? = Nothing

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *  ")
            StrSQL.AppendLine(" FROM  Alert_Tipologia ")
            StrSQL.AppendLine(" WHERE ID_Area = " & Agro_SQL_SaveNum(ID_Area))
            StrSQL.AppendLine(" AND Nome = '" & Agro_SQL_SaveText(NomeTipologia) & "' ")
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                id = DT.Rows(0)("ID_Tipologia")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return id

    End Function

    Public Function Max(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Max()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT isnull( max(id_tipologia),0) + 1 as 'max'  ")
            StrSQL.Append(" FROM  alert_tipologia ")
            StrSQL.Append(" where   id_tipologia > 0 ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT(0).Item("Max")

    End Function

    Public Function TestSeTipologiaInUso(ByVal id_tipologia As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.TestSeTipologiaInUso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim res As Boolean = True

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM alert_elenco ")
            StrSQL.Append(" WHERE id_tipologia = " & Agro_SQL_SaveNum(id_tipologia) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            res = If(DT.Rows.Count > 0, True, False)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return res

    End Function


    Public Function Leggi_non_ancora_gestite(ByVal ID_Area As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Leggi_non_ancora_gestite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Tipologia ")
            StrSQL.Append(" WHERE 1= 1 ")
            StrSQL.Append(" AND id_tipologia not in (select id_tipologia from alert_tipologia_x_utente ")
            StrSQL.Append("         where Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "') ")
            If ID_Area <> 0 Then
                StrSQL.Append(" AND ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
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


    Public Function Leggi_CatCod(ByVal ID_Tipologia As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Leggi_CatCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT       Alert_Area.TipoEntita_Cod, Alert_Tipologia.Nome, Alert_Tipologia.Colore, Alert_Tipologia.Preavviso,  Alert_Tipologia.Cat_Cod, CategorieDocumenti.Sottocartella , CategorieDocumenti.Cat_Des , CategorieDocumenti.Cat_Des_long     ")
            StrSQL.Append(" FROM   Alert_Tipologia  ")
            StrSQL.Append("     INNER JOIN		Alert_Area on Alert_Area.Id_Area = Alert_Tipologia.ID_Area ")
            StrSQL.Append("     INNER JOIN   CategorieDocumenti ON Alert_Tipologia.Cat_Cod = CategorieDocumenti.Cat_Cod  ")

            StrSQL.Append(" WHERE 1= 1 ")

            If ID_Tipologia <> 0 Then
                StrSQL.Append(" AND Alert_Tipologia.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
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




    Public Function Leggi_AreaETipologia(ByVal piva As String,
                                         ByVal ID_Area As Integer,
                                         ByVal ID_Tipologia As Integer,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByVal Utilizzo_GiasAPP As Integer? = Nothing
                                         ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_R.Leggi_AreaETipologia()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT   Alert_Area.ID_Area, Alert_Tipologia.ID_Tipologia, Alert_Area.Nome As Nome_Area, Alert_Tipologia.Nome As Nome_Tipologia ")
            StrSQL.AppendLine(" FROM   Alert_Tipologia  ")
            StrSQL.AppendLine(" INNER JOIN	 Alert_Area on Alert_Area.Id_Area = Alert_Tipologia.ID_Area ")
            StrSQL.AppendLine(" WHERE Alert_Tipologia.PivaSuperUser = '" & objParametri.PivaSuperUser & "'")

            ' Tipologie con indice negatovo sono validi per tutti, diversamente considero solo Piva Superuser
            ' per ora Piva non viene utilizzata
            'StrSQL.AppendLine(" (")
            'StrSQL.AppendLine("(ID_Tipologia > 0 And Alert_Tipologia.Piva = " & objParametri.PivaSuperUser & ")")
            'StrSQL.AppendLine(" Or ")
            'StrSQL.AppendLine("(ID_Tipologia < 0)")
            'StrSQL.AppendLine(" ) ")

            'Gestione Esclusioni Tabella CategTipologiaDocumentiXUtenti
            'OLD
            'StrSQL.AppendLine(" And (exists (select * From CategTipologiaDocumentiXUtenti permessi where permessi.ID_Categoria = Alert_Tipologia.ID_area And permessi.ID_Tipologia = Alert_Tipologia.ID_Tipologia And permessi.Autorizzato = 1 And permessi.Username = '" & objParametri.UtenteUsername & "') ")
            'StrSQL.AppendLine(" Or ( Not exists (select * From CategTipologiaDocumentiXUtenti permessi  where permessi.ID_Categoria = Alert_Tipologia.ID_area And permessi.ID_Tipologia = Alert_Tipologia.ID_Tipologia And permessi.Autorizzato = 0 And permessi.Username = '" & objParametri.UtenteUsername & "') ")
            'StrSQL.AppendLine(" And  Not exists (select * From CategTipologiaDocumentiXUtenti permessi  where permessi.ID_Categoria = Alert_Tipologia.ID_area And permessi.ID_Tipologia = 0 And permessi.Autorizzato = 0 And permessi.Username = '" & objParametri.UtenteUsername & "' ) ) ) ")

            'Ora se sono stati inseriti dei record nella tabella CategTipologiaDocumentiXUtenti,
            'l'utente deve per forza essere autorizzato ad utilizzare quella Categoria oppure quella specifica Tipologia. (Tranne se l'utente superuser che vede tutto)
            If objParametri.UtenteUsername.ToLower() <> objParametri.SuperUserUsername.ToLower() Then
                StrSQL.AppendLine(" And (exists (select * From CategTipologiaDocumentiXUtenti permessi where permessi.ID_Categoria = Alert_Tipologia.ID_area And ( permessi.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi.ID_Tipologia = 0 ) And permessi.Autorizzato = 1 And permessi.Username = '" & objParametri.UtenteUsername & "') ")
                StrSQL.AppendLine(" Or ( Not exists (select * From CategTipologiaDocumentiXUtenti permessi  ))) ")
            End If

            If ID_Area <> 0 Then
                StrSQL.AppendLine(" And Alert_Area.ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
            End If

            If ID_Tipologia <> 0 Then
                StrSQL.AppendLine(" And Alert_Tipologia.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            End If

            If Not IsNothing(Utilizzo_GiasAPP) Then
                StrSQL.AppendLine(" And Alert_Tipologia.Utilizzo_GiasAPP = " & Agro_SQL_SaveNum(Utilizzo_GiasAPP) & " ")
                StrSQL.AppendLine(" And (Alert_Area.TipoEntita_Cod = 0 OR Alert_Area.ID_Area IN (6,10,11,13)) ")
            End If

            StrSQL.AppendLine(" Order By Alert_Area.Nome, Alert_Tipologia.Nome ")

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

    Public Function Leggi_Tipologia_UNION_Categoria(ByVal ID_Area As Integer,
                                                    ByVal ID_Tipologia As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_R.Leggi_Tipologia_UNION_Categoria()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *, NEWID() AS rowId  FROM ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     SELECT c.ID_Area, c.nome AS Nome_Area ,ID_Tipologia ,t.nome AS Nome_Tipologia ")
            StrSQL.AppendLine("     FROM  Alert_Tipologia t ")
            StrSQL.AppendLine("     JOIN  Alert_Area c ON c.ID_Area = t.id_area ")

            StrSQL.AppendLine("     UNION      ")

            StrSQL.AppendLine("     SELECT ID_Area, Nome AS Nome_Area, 0 AS ID_Tipologia, '' AS Nome_Tipologia")
            StrSQL.AppendLine("     FROM Alert_Area ")

            If ID_Area <> 0 Then
                StrSQL.AppendLine(" AND Alert_Area.ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
            End If

            If ID_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND Alert_Tipologia.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            End If

            StrSQL.AppendLine(" ) AS queryPrincipale")

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


'##############################################################################################


Public Class Alert_Tipologia_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
                                ByVal ID_Area As Integer,
                                ByVal ID_Tipologia As Integer,
                                ByVal Piva As String,
                                ByVal Nome As String,
                                ByVal Colore As String,
                                ByVal Preavviso As Int32,
                                ByVal Cat_Cod As Integer,
                                ByVal Codice_Tipologia As String,
                                ByVal DataDefault As String,
                                ByVal FlagDataScadenzaObbligatoria As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Utilizzo_GiasAPP As Integer? = 0
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Alert_Tipologia ")
            StrSQL.Append("                   ( PivaSuperUser, ID_Area, ID_Tipologia, Piva ,Nome  ,Colore ,Preavviso, Cat_Cod, Codice_Tipologia, Utilizzo_GiasAPP,  ")
            'Anna 29/04/22: aggiunti campi al filtro di ricerca
            StrSQL.Append("                     FlagDataScadenzaObbligatoria,   ")
            If DataDefault IsNot Nothing Then
                StrSQL.Append("                 DataDefault,  ")
            End If

            StrSQL.Append(" username_creazione, data_creazione) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_vb_SaveNum(ID_Area) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Tipologia) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Colore) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Preavviso) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Codice_Tipologia) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Utilizzo_GiasAPP) & " ")

            'Anna 29/04/22: aggiunti campi al filtro di ricerca
            StrSQL.Append("         , " & Agro_SQL_SaveNum(FlagDataScadenzaObbligatoria) & " ")
            If DataDefault IsNot Nothing Then
                StrSQL.Append("     , " & Agro_SQL_SaveDate(DataDefault) & " ")
            End If

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")

            StrSQL.Append(" )")
            '---------------------------------------------


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


    Public Function Modifica(ByVal ID_Tipologia As Integer,
                             ByVal Piva As String,
                             ByVal Nome As String,
                             ByVal Colore As String,
                             ByVal Preavviso As Integer,
                             ByVal Cat_Cod As Integer,
                             ByVal DataDefault As String,
                             ByVal FlagDataScadenzaObbligatoria As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Utilizzo_GiasAPP As Integer? = Nothing
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_W.Modifica()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Tipologia SET ")
            StrSQL.Append("   Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            If Nome <> "" Then
                StrSQL.Append("    ,Nome           = '" & Agro_SQL_SaveText(Nome) & "' ")
            End If
            If Colore <> "" Then
                StrSQL.Append("    ,Colore           = '" & Agro_SQL_SaveText(Colore) & "' ")
            End If
            If Preavviso <> 0 Then
                StrSQL.Append("    ,Preavviso           = " & Agro_SQL_SaveNum(Preavviso) & " ")
            End If
            If Cat_Cod <> 0 Then
                StrSQL.Append("    ,Cat_Cod           = " & Agro_SQL_SaveNum(Cat_Cod) & " ")
            End If
            If Not IsNothing(Utilizzo_GiasAPP) Then
                StrSQL.Append("   ,Utilizzo_GiasAPP = " & Agro_SQL_SaveNum(Utilizzo_GiasAPP) & " ")
            End If

            'Anna 29/04/22: aggiunti campi al filtro di ricerca
            If Not IsNothing(DataDefault) AndAlso DataDefault <> "" Then
                StrSQL.Append("   , DataDefault = " & Agro_SQL_SaveDate(DataDefault) & " ")
            Else
                StrSQL.Append("   , DataDefault = NULL ")
            End If
            If Not IsNothing(FlagDataScadenzaObbligatoria) Then
                StrSQL.Append("   ,FlagDataScadenzaObbligatoria = " & Agro_SQL_SaveNum(FlagDataScadenzaObbligatoria) & " ")
            End If


            StrSQL.Append(" WHERE   ID_Tipologia        =" & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            'StrSQL.Append(" AND   Piva        ='" & Agro_SQL_SaveText(Piva) & "' ")

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





    Public Function Cancella(
                               ByVal ID_Tipologia As Integer,
                               ByVal Piva As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Tipologia ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Tipologia ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            'StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


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
