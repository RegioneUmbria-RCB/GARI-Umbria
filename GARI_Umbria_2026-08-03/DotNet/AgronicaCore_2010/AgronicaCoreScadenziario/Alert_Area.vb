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


Public Class Alert_Area_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal Piva As String,
                          ByVal ID_Area As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              Optional ByVal controllaSeUtenteAutorizzato As Boolean? = True,
                              Optional ByVal tipoPermessoDaControllare As Integer = 2,
                              Optional ByVal xFiltroAggiuntivo As String = ""
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Leggi()"

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
            StrSQL.AppendLine(" SELECT Alert_Area.*  ")

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_SELECT(StrSQL, iLivelloGerarchia)

            End If



            StrSQL.AppendLine(" FROM  Alert_Area ")

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_JOIN(StrSQL, iLivelloGerarchia,
                                                                            Piva, True, objParametri)
            End If


            StrSQL.AppendLine(" WHERE Alert_Area.PivaSuperUser = '" & PivaSuperUser & "' ")


            'If Piva <> "" Then
            '    StrSQL.AppendLine(" AND Alert_Area.Piva In ('', " & Agro_SQL_SaveText_NULL(Piva) & ") ")
            'End If


            If ID_Area <> 0 Then
                StrSQL.AppendLine(" AND Alert_Area.ID_Area = " & Agro_SQL_SaveNum(ID_Area) & " ")
            End If

            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_WHERE(StrSQL, iLivelloGerarchia, tipoPermessoDaControllare, True)

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_ORDER_BY(StrSQL, True)

            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'Anna 29/04/22: aggiunti campi al filtro di ricerca
            If bPermessixUtente = False Then
                StrSQL.AppendLine("ORDER BY Alert_Area.Nome")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_DataTable(DT, tipoPermessoDaControllare, True)

            End If


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiIdDaNomeArea(ByVal Nome_Area As String,
                                      ByVal Piva As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Integer?


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim id As Integer? = Nothing

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Area ")
            StrSQL.Append(" WHERE Nome = '" & Agro_SQL_SaveText(Nome_Area) & "' ")
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                id = DT.Rows(0)("ID_Area")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return id

    End Function


    Public Function LeggiPerPiva(ByVal piva As String,
                                 ByVal pivePadre As String,
                                 ByVal soloPrivate As Boolean,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal controllaSeUtenteAutorizzato As Boolean? = True,
                                 Optional ByVal tipoPermessoDaControllare As Integer = 2,
                                 Optional ByVal xFiltroAggiuntivo As String = ""
                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.LeggiPerPiva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim bPermessixUtente As Boolean = False 'Booleano per la presenza di record in tabella obj_CategTipologiaDocumentiXUtenti
        'In caso non esistano record viene bypassato il filtro

        Dim iLivelloGerarchia As Integer = 0 'Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

        Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

        Try

            If controllaSeUtenteAutorizzato Then
                obj_CategTipologiaDocumentiXUtenti.Controllo_Se_Vengono_Gestiti_Permessi(iLivelloGerarchia, bPermessixUtente, piva, objParametri)
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT Alert_Area.* ")

            If bPermessixUtente Then
                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_SELECT(StrSQL, iLivelloGerarchia)
            End If


            StrSQL.AppendLine(" FROM  Alert_Area  ")
            StrSQL.AppendLine(" LEFT JOIN  Alert_Tipologia t ON Alert_Area.id_area=t.id_area ")

            If bPermessixUtente Then
                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_JOIN(StrSQL, iLivelloGerarchia,
                                                                            piva, True, objParametri)
            End If



            StrSQL.AppendLine(" WHERE ( t.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR t.PivaSuperUser IS NULL")


            'If pivePadre <> "" Then
            '    StrSQL.AppendLine(" OR t.Piva IN " & pivePadre & " ")
            'End If

            StrSQL.AppendLine(" ) ")

            'If soloPrivate Then
            '    StrSQL.AppendLine(" AND t.id_tipologia >=0 OR t.Piva IS NULL")
            'End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If bPermessixUtente Then

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_WHERE(StrSQL, iLivelloGerarchia, tipoPermessoDaControllare, True)

                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_ORDER_BY(StrSQL, True)

            Else

                StrSQL.AppendLine(" ORDER BY Alert_Area.nome ")

            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If bPermessixUtente Then
                obj_CategTipologiaDocumentiXUtenti.Controllo_Permessi_Filtro_DataTable(DT, tipoPermessoDaControllare, True)
            End If


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_non_ancora_gestite(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Leggi_non_ancora_gestite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select * from alert_area  ")
            StrSQL.Append(" where id_area  in (  ")

            StrSQL.Append("     select ID_Area from alert_tipologia  ")
            StrSQL.Append("         where id_tipologia not in (select id_tipologia from alert_tipologia_x_utente  ")
            StrSQL.Append("     where Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "')  ")
            StrSQL.Append(" ) ")


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

    Public Function TestSeAreaInUso(ByVal id_area As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim res As Boolean = True

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM alert_tipologia ")
            StrSQL.Append(" WHERE id_area = " & Agro_SQL_SaveNum(id_area) & " ")

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


End Class

Public Class Alert_Area_W
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Scrivi(
                            ByVal ID_Area As Integer,
                            ByVal Nome As String,
                            ByVal TipoEntita_Cod As Integer,
                            ByVal Piva As String,
                            ByVal Codice_Area As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Alert_Area ")
            StrSQL.Append("                   ( PivaSuperUser, ID_Area, Nome, TipoEntita_Cod, Piva, Codice_Area  ")

            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_vb_SaveNum(ID_Area) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(TipoEntita_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Codice_Area) & "' ")

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

    Public Function Modifica(
                                ByVal ID_area As Integer,
                                ByVal Nome As String,
                                ByVal TipoEntita_Cod As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_W.Modifica()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Area SET ")
            StrSQL.Append("    Nome           = '" & Agro_SQL_SaveText(Nome) & "' ")

            If TipoEntita_Cod <> 0 Then
                StrSQL.Append("    ,TipoEntita_Cod           = " & Agro_vb_SaveNum(TipoEntita_Cod) & " ")
            End If

            StrSQL.Append(" WHERE   ID_Area        =" & Agro_SQL_SaveNum(ID_area) & " ")

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
                               ByVal ID_area As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Area_W.Cancella()"

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

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Area ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Area = " & Agro_SQL_SaveNum(ID_area) & " ")


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
