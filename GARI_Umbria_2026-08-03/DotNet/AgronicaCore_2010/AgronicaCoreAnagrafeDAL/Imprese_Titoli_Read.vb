Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Imprese_Titoli_Read
    Inherits AgronicaCoreDataProvider.DataProvider



    '################################################################################
    'Public Function Leggi_Imprese_Titoli( _
    '                                ByRef ErrMSG As String, _
    '                                ByVal Piva_SuperUser As String, _
    '                                ByVal Piva As String, _
    '                                ByVal FiltroAggiuntivo As String, _
    '                                ByVal Ordinamento As String, _
    '                                ByVal FinestraTemp_Inizio As Date, _
    '                                ByVal FinestraTemp_Fine As Date, _
    '                                ByRef objConnessione As DbConnection, _
    '                                ByRef objTransazione As DbTransaction, _
    '                                ByVal StringaConnessione As String, _
    '                                ByVal FlagVisibilita As Int32, _
    '                                ByVal DirectoryLOG As String, _
    '                                ByVal FileLOG As String, _
    '                                ByVal IdentificatoreUtente As String) _
    '                                As DataTable

    '    '----- Descrizione
    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imrese_Titoli_Read.Leggi_Imprese_Titoli()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  si leggono tutte le imprese
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable
    '    Dim objEnum As AgronicaCoreDataProvider.TipiEnumerativi

    '    Try

    '        objEnum = New AgronicaCoreDataProvider.TipiEnumerativi

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des")
    '        StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
    '        StrSQL.Append("         Imprese_Codici ON Imprese_Codici.Val_Cod = Imprese_Titoli.Cuaa INNER JOIN ")
    '        StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
    '        StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
    '        StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
    '        StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
    '        StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
    '        StrSQL.Append(" AND     Imprese_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
    '        StrSQL.Append(" AND     Imprese_Codici.Id_Cod = " & Agro_SQL_SaveNum(objEnum.enum_CodiciAnagrafe.CodiceCUAA) & "")

    '        'StrSQL.Length = 0
    '        'StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des")
    '        'StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
    '        'StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
    '        'StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")



    '        'StrSQL.Length = 0
    '        'StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des, ISNULL(Imprese_Titoli_Codici.Val_Cod,'0') AS Modifica ")
    '        'StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
    '        'StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
    '        'StrSQL.Append("         LEFT OUTER JOIN Imprese_Codici ON Imprese_Titoli.Piva = Imprese_Codici.Piva ")
    '        'StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
    '        'StrSQL.Append(" AND     Imprese_Titoli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
    '        'StrSQL.Append(" AND     Imprese_Titoli_Codici.Id_Cod = " & Agro_SQL_SaveNum(objEnum.enum_DatiAnagrafici_CodiciAnagrafe.Azienda) & "")
    '        'StrSQL.Append(" AND     Imprese_Titoli_Codici.Piva = Imprese_Titoli.Piva ")
    '        'StrSQL.Append(" AND     Imprese_Titoli_Codici.Cuaa = Imprese_Titoli.Cuaa ")
    '        'StrSQL.Append(" AND     Imprese_Titoli_Codici.Titolo_Cod = Imprese_Titoli.Titolo_Cod ")
    '        'StrSQL.Append(" AND     Imprese_Titoli_Codici.Validita_Inizio = Imprese_Titoli.Validita_Inizio ")

    '        If FiltroAggiuntivo <> "" Then
    '            StrSQL.Append(FiltroAggiuntivo)
    '        End If

    '        If Ordinamento = "" Then
    '            StrSQL.Append(" ORDER BY Titolo_Cod ")
    '        Else
    '            StrSQL.Append(Ordinamento)
    '        End If

    '        '----------------------------------------------------
    '        '--- Recupero il datatable --------------------------
    '        '----------------------------------------------------

    '        'Recupero il datatable
    '        DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function




    Public Function Leggi_Imprese_Titoli( _
                                        ByRef ErrMSG As String, _
                                        ByVal Piva_SuperUser As String, _
                                        ByVal Piva As String, _
                                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imrese_Titoli_Read.Leggi_Imprese_Titoli()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim objEnum As AgronicaCoreDataProvider.TipiEnumerativi

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'TODO
                    'modificare la query di select
                    objEnum = New AgronicaCoreDataProvider.TipiEnumerativi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des")
                    StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
                    StrSQL.Append("         Imprese_Codici ON Imprese_Codici.Val_Cod = Imprese_Titoli.Cuaa INNER JOIN ")
                    StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
                    StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
                    StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
                    StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
                    StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    StrSQL.Append(" AND     Imprese_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    StrSQL.Append(" AND     Imprese_Codici.Id_Cod = " & Agro_SQL_SaveNum(objEnum.enum_CodiciAnagrafe.CodiceCUAA) & "")

                    'StrSQL.Length = 0
                    'StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des")
                    'StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
                    'StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
                    'StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    'StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")



                    'StrSQL.Length = 0
                    'StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des, ISNULL(Imprese_Titoli_Codici.Val_Cod,'0') AS Modifica ")
                    'StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
                    'StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
                    'StrSQL.Append("         LEFT OUTER JOIN Imprese_Codici ON Imprese_Titoli.Piva = Imprese_Codici.Piva ")
                    'StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    'StrSQL.Append(" AND     Imprese_Titoli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Id_Cod = " & Agro_SQL_SaveNum(objEnum.enum_DatiAnagrafici_CodiciAnagrafe.Azienda) & "")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Piva = Imprese_Titoli.Piva ")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Cuaa = Imprese_Titoli.Cuaa ")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Titolo_Cod = Imprese_Titoli.Titolo_Cod ")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Validita_Inizio = Imprese_Titoli.Validita_Inizio ")


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese_Titoli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese_Titoli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Titolo_Cod ASC")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    objEnum = New AgronicaCoreDataProvider.TipiEnumerativi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des")
                    StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
                    StrSQL.Append("         Imprese_Codici ON Imprese_Codici.Val_Cod = Imprese_Titoli.Cuaa INNER JOIN ")
                    StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
                    StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
                    StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
                    StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
                    StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    StrSQL.Append(" AND     Imprese_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    StrSQL.Append(" AND     Imprese_Codici.Id_Cod = " & Agro_SQL_SaveNum(objEnum.enum_CodiciAnagrafe.CodiceCUAA) & "")

                    'StrSQL.Length = 0
                    'StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des")
                    'StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
                    'StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
                    'StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    'StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")



                    'StrSQL.Length = 0
                    'StrSQL.Append(" SELECT  Imprese_Titoli.*, Titoli_Tipo.Tipo_Des, Titoli_Movimento.Movimento_Des, Titoli_Stato.Stato_Des, Titoli_Origine.Origine_Des, ISNULL(Imprese_Titoli_Codici.Val_Cod,'0') AS Modifica ")
                    'StrSQL.Append(" FROM    Imprese_Titoli INNER JOIN ")
                    'StrSQL.Append("         Titoli_Movimento ON Imprese_Titoli.Movimento_cod = Titoli_Movimento.Movimento_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Stato ON Imprese_Titoli.Stato_cod = Titoli_Stato.Stato_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Origine ON Imprese_Titoli.Origine_cod = Titoli_Origine.Origine_Cod INNER JOIN ")
                    'StrSQL.Append("         Titoli_Tipo ON Imprese_Titoli.Tipo_cod = Titoli_Tipo.Tipo_Cod ")
                    'StrSQL.Append("         LEFT OUTER JOIN Imprese_Codici ON Imprese_Titoli.Piva = Imprese_Codici.Piva ")
                    'StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    'StrSQL.Append(" AND     Imprese_Titoli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Id_Cod = " & Agro_SQL_SaveNum(objEnum.enum_DatiAnagrafici_CodiciAnagrafe.Azienda) & "")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Piva = Imprese_Titoli.Piva ")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Cuaa = Imprese_Titoli.Cuaa ")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Titolo_Cod = Imprese_Titoli.Titolo_Cod ")
                    'StrSQL.Append(" AND     Imprese_Titoli_Codici.Validita_Inizio = Imprese_Titoli.Validita_Inizio ")


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese_Titoli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese_Titoli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Titolo_Cod ASC")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



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