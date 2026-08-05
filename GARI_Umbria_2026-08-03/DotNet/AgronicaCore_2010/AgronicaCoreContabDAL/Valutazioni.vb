Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.valutazioni


Public Class Valutazione_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################
    'Lettura semplice generica
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Testata As Integer,
                          ByVal Valutazione_Piano_Cod As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Distinct Valutazione_Testata.*, Valutazione_Piano_Conti.Valutazione_Piano_Des, Imprese.Rag_Soc, ")
            StrSQL.AppendLine("        CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Valutazione_Testata.Piva ELSE Imprese.partitaIvaReale END AS partitaIvaReale ")
            StrSQL.AppendLine(" FROM  Valutazione_Testata ")
            StrSQL.AppendLine(" INNER JOIN Valutazione_Piano_Conti ON Valutazione_Testata.Valutazione_Piano_Cod = Valutazione_Piano_Conti.Valutazione_Piano_Cod ")
            StrSQL.AppendLine(" INNER JOIN Imprese ON Valutazione_Testata.Piva = Imprese.Piva ")

            StrSQL.AppendLine(" WHERE Valutazione_Testata.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Valutazione_Testata.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Testata <> 0 Then
                StrSQL.AppendLine(" AND Valutazione_Testata.ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If

            If Valutazione_Piano_Cod <> 0 Then
                StrSQL.AppendLine(" AND Valutazione_Testata.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Valutazione_Testata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Valutazione_Testata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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



Public Class Valutazione_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Valutazione_Testata As Valutazione_Testata,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Testata_W.Scrivi()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Valutazione_Testata ")
            StrSQL.Append("                   ( Piva_SuperUser, Piva, Id_Testata, Valutazione_Piano_Cod, Data_Redazione, Note")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valutazione_Testata.primaryKey.Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Testata.primaryKey.Id_Testata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Testata.Valutazione_Piano_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Valutazione_Testata.Data_Redazione) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valutazione_Testata.Note) & "' ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            StrSQL.Append(" )")
            '---------------------------------------------

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



    Public Function Modifica(ByVal Valutazione_Testata As Valutazione_Testata,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Testata_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_Testata ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Data_Redazione = " & Agro_SQL_SaveDateTime(Valutazione_Testata.Data_Redazione) & " ")
            StrSQL.AppendLine("     ,Note = " & Agro_SQL_SaveText_NULL(Valutazione_Testata.Note))
            StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione) & " ")
            StrSQL.AppendLine("     ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva =    '" & Agro_SQL_SaveText(Valutazione_Testata.primaryKey.Piva) & "' ")
            StrSQL.AppendLine(" AND Id_Testata = " & Agro_SQL_SaveNum(Valutazione_Testata.primaryKey.Id_Testata) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            '---------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal objValutazione_Testata As Valutazione_Testata,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.objValutazione_Testata.Cancella()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Testata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Testata ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(objValutazione_Testata.primaryKey.Piva) & "' ")
            StrSQL.Append(" AND Id_Testata = " & Agro_SQL_SaveNum(objValutazione_Testata.primaryKey.Id_Testata) & " ")

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

Public Class Valutazione_TestataxAnno_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################

    'Ritorna l'elenco delle testata con indicazione del piano conti per l'anno selezionato con indicazione delle tipologia anno
    Public Function LeggixAnno(ByVal Piva As String,
                               ByVal Id_Testata As Integer,
                               ByVal Valutazione_Piano_Cod As Integer,
                               ByVal Anno As Integer,
                               ByRef objParametri As AgronicaCoreParametri,
                               Optional ByVal xFiltroAggiuntivo As String = "",
                               Optional ByVal xOrderBy As String = ""
                               ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_TestataxAnno_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Testata.*, Valutazione_Piano_Conti.Valutazione_Piano_Des, ")
            StrSQL.Append(" Valutazione_TestataxAnno.Anno, Valutazione_TestataxAnno.Anno_Tipo, ")

            StrSQL.AppendLine(" CASE")
            StrSQL.AppendLine("       WHEN Valutazione_TestataxAnno.Anno_Tipo = 2 Then 'Aperto'")
            StrSQL.AppendLine("       WHEN Valutazione_TestataxAnno.Anno_Tipo = 3 Then 'Previsionale'")
            StrSQL.AppendLine("       ELSE 'Chiuso'")
            StrSQL.AppendLine("   END AS Anno_Tipo_Des")
            StrSQL.Append(" FROM  Valutazione_Testata ")

            StrSQL.Append(" INNER JOIN Valutazione_Piano_Conti On (Valutazione_Testata.Valutazione_Piano_Cod = Valutazione_Piano_Conti.Valutazione_Piano_Cod ) ")
            StrSQL.Append(" INNER JOIN Valutazione_TestataxAnno On (Valutazione_Testata.Piva = Valutazione_TestataxAnno.Piva And Valutazione_Testata.Id_Testata = Valutazione_TestataxAnno.Id_Testata ) ")

            StrSQL.Append(" WHERE Valutazione_Testata.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.Append(" AND Valutazione_Testata.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Testata <> 0 Then
                StrSQL.Append(" AND Valutazione_TestataxAnno.ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If

            If Valutazione_Piano_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_TestataxAnno.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND Valutazione_TestataxAnno.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Valutazione_TestataxAnno.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Valutazione_TestataxAnno.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Anno ")
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


    'Ritorna l'elenco delle testate esistenti in periodo determinato
    Public Function LeggixRangeAnno(ByVal Piva As String,
                                    ByVal Id_Testata As Integer,
                                    ByVal Valutazione_Piano_Cod As Integer,
                                    ByVal Anno_Da As Integer,
                                    ByVal Anno_A As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal xFiltroAggiuntivo As String = "",
                                    Optional ByVal xOrderBy As String = ""
                                    ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_TestataxAnno_R.LeggixRangeAnno()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable


        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Testata.*, Valutazione_Piano_Conti.Valutazione_Piano_Des ")

            StrSQL.Append(" FROM  Valutazione_Testata ")

            StrSQL.Append(" INNER JOIN Valutazione_Piano_Conti On (Valutazione_Testata.Valutazione_Piano_Cod = Valutazione_Piano_Conti.Valutazione_Piano_Cod ) ")

            StrSQL.Append(" WHERE Valutazione_Testata.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Valutazione_Testata.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Valutazione_Testata.ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Valutazione_Piano_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Testata.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")
            End If


            For iAnno As Integer = Anno_Da To Anno_A

                StrSQL.Append(" And Exists (Select * From Valutazione_TestataxAnno On (Valutazione_Testata.Piva = Valutazione_TestataxAnno.Piva ")
                StrSQL.Append("                                                    And Valutazione_Testata.Id_Testata = Valutazione_TestataxAnno.Id_Testata ) ")
                StrSQL.Append("                                                    And Valutazione_TestataxAnno.Anno = " & iAnno & ") ")


            Next


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" And   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" And   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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








Public Class Valutazione_TestataxAnno_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Valutazione_TestataxAnno As Valutazione_TestataxAnno,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_TestataxAnno_W.Scrivi()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Valutazione_TestataxAnno ")
            StrSQL.Append("                   ( Piva_SuperUser, Piva, Id_Testata , Anno,  Anno_Tipo ")

            StrSQL.Append("                   ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valutazione_TestataxAnno.primaryKey.valutazioneTestataPK.Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_TestataxAnno.primaryKey.valutazioneTestataPK.Id_Testata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_TestataxAnno.primaryKey.Anno) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_TestataxAnno.Anno_Tipo) & "  ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            StrSQL.Append(" )")
            '---------------------------------------------

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




    Public Function Modifica(ByVal Valutazione_TestataxAnno As Valutazione_TestataxAnno,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_TestataxAnno_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_TestataxAnno ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Anno_Tipo = " & Agro_SQL_SaveNum(Valutazione_TestataxAnno.Anno_Tipo) & " ")

            StrSQL.Append(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Valutazione_TestataxAnno.primaryKey.valutazioneTestataPK.Piva) & "' ")
            StrSQL.Append(" AND Id_Testata = " & Agro_SQL_SaveNum(Valutazione_TestataxAnno.primaryKey.valutazioneTestataPK.Id_Testata) & " ")
            StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(Valutazione_TestataxAnno.primaryKey.Anno) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            '---------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal objValutazione_TestataxAnno As Valutazione_TestataxAnno,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.objValutazione_TestataxAnno.Cancella()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_TestataxAnno ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_TestataxAnno ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(objValutazione_TestataxAnno.primaryKey.valutazioneTestataPK.Piva) & "' ")
            StrSQL.Append(" AND Id_Testata = " & Agro_SQL_SaveNum(objValutazione_TestataxAnno.primaryKey.valutazioneTestataPK.Id_Testata) & " ")

            If objValutazione_TestataxAnno.primaryKey.Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(objValutazione_TestataxAnno.primaryKey.Anno) & " ")
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




    Public Function Cancella2(ByVal Piva As String,
                              ByVal Id_Testata As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.objValutazione_TestataxAnno.Cancella2()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_TestataxAnno ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_TestataxAnno ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

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


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Valutazione_Dettaglio_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Testata As Integer,
                          ByVal Valutazione_Conto_Cod As Integer,
                          ByVal Anno As Integer,
                          ByVal Dettaglio_Key As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Dettaglio.*, Valutazione_Conto.Valutazione_Conto_Des, Valutazione_Conto.Ordine_Default,   ")
            StrSQL.Append(" Valutazione_Sezione.Valutazione_Sezione_Cod, Valutazione_Sezione.Valutazione_Sezione_Des, Valutazione_Sezione.Pat_Eco, ")
            StrSQL.Append(" Valutazione_Sezione.Attivo_Passivo, Valutazione_Sezione.Ordine as Sezione_Ordine, Valutazione_Sezione.Valutazione_Gruppo_Cod, Valutazione_Sezione.ChkInvisibile as Sezione_Invisibile, ")
            StrSQL.Append(" Isnull(Valutazione_Gruppo.Valutazione_Gruppo_Des, '') as Valutazione_Gruppo_Des,  ")
            StrSQL.Append(" Isnull(Valutazione_Gruppo.Valutazione_Gruppo_Padre, 0) as Valutazione_Gruppo_Padre,  ")
            StrSQL.Append(" Isnull(VG2.Valutazione_Gruppo_Des, '') as Valutazione_Gruppo_Padre_Des,  ")
            StrSQL.Append(" Isnull(Valutazione_Conto.Valutazione_Conto_Padre, 0) as Valutazione_Conto_Padre,  ")
            StrSQL.Append(" Isnull(Valutazione_Conto.ChkBypassValore, 0) as ChkBypassValore,  ")
            StrSQL.Append(" Valutazione_TestataxAnno.Anno_Tipo, Valutazione_Piano_ContixConti.Ordine_PC, ")

            StrSQL.AppendLine(" CASE")
            StrSQL.AppendLine("       WHEN Valutazione_TestataxAnno.Anno_Tipo = 2 Then 'Aperto'")
            StrSQL.AppendLine("       WHEN Valutazione_TestataxAnno.Anno_Tipo = 3 Then 'Previsionale'")
            StrSQL.AppendLine("       ELSE 'Chiuso'")
            StrSQL.AppendLine("   END AS Anno_Tipo_Des")

            StrSQL.Append(" FROM  Valutazione_Dettaglio ")


            StrSQL.Append(" INNER JOIN Valutazione_Conto On (Valutazione_Dettaglio.Valutazione_Conto_Cod = Valutazione_Conto.Valutazione_Conto_Cod)  ")
            StrSQL.Append(" INNER JOIN Valutazione_Sezione On (Valutazione_Conto.Valutazione_Sezione_Cod = Valutazione_Sezione.Valutazione_Sezione_Cod And Valutazione_Dettaglio.Valutazione_Conto_Cod = Valutazione_Conto.Valutazione_Conto_Cod)  ")
            StrSQL.Append(" INNER JOIN Valutazione_TestataxAnno On (Valutazione_Dettaglio.Piva = Valutazione_TestataxAnno.Piva And Valutazione_Dettaglio.Id_Testata = Valutazione_TestataxAnno.Id_Testata And Valutazione_Dettaglio.Anno = Valutazione_TestataxAnno.Anno)  ")
            StrSQL.Append(" INNER JOIN Valutazione_Testata On (Valutazione_Dettaglio.Piva = Valutazione_Testata.Piva And Valutazione_Dettaglio.Id_Testata = Valutazione_Testata.Id_Testata)  ")

            StrSQL.Append(" INNER JOIN Valutazione_Piano_ContixConti On ( Valutazione_Dettaglio.Valutazione_Conto_Cod = Valutazione_Piano_ContixConti.Valutazione_Conto_Cod  ")
            StrSQL.Append("                                           And Valutazione_Testata.Valutazione_Piano_Cod = Valutazione_Piano_ContixConti.Valutazione_Piano_Cod ) ")


            StrSQL.Append(" LEFT OUTER JOIN Valutazione_Gruppo On (Valutazione_Dettaglio.Valutazione_Conto_Cod = Valutazione_Conto.Valutazione_Conto_Cod And Valutazione_Sezione.Valutazione_Gruppo_Cod = Valutazione_Gruppo.Valutazione_Gruppo_Cod)  ")
            StrSQL.Append(" LEFT OUTER JOIN Valutazione_Gruppo VG2 On (Valutazione_Gruppo.Valutazione_Gruppo_Padre = VG2.Valutazione_Gruppo_Cod)  ")

            If Trim(Dettaglio_Key) <> "" Then
                StrSQL.Append(" INNER JOIN Valutazione_Dettaglio_Specifico On (Valutazione_Dettaglio.Piva = Valutazione_Dettaglio_Specifico.Piva  ")
                StrSQL.Append("                                            And Valutazione_Dettaglio.Id_Testata = Valutazione_Dettaglio_Specifico.Id_Testata  ")
                StrSQL.Append("                                            And Valutazione_Dettaglio.Valutazione_Conto_Cod = Valutazione_Dettaglio_Specifico.Valutazione_Conto_Cod  ")
                StrSQL.Append("                                            And Valutazione_Dettaglio.Anno = Valutazione_Dettaglio_Specifico.Anno  ")
                StrSQL.Append("                                            And Valutazione_Dettaglio_Specifico.Dettaglio_Key = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "') ")
            End If

            StrSQL.Append(" WHERE Valutazione_Dettaglio.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Valutazione_Dettaglio.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Valutazione_Dettaglio.ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" And Valutazione_Dettaglio.Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If

            If Anno <> 0 Then
                StrSQL.Append(" And Valutazione_Dettaglio.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" And   Valutazione_Dettaglio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" And   Valutazione_Dettaglio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Piva, Id_Testata, Pat_Eco, Attivo_Passivo, Sezione_Ordine, Ordine_PC ")
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



    Public Function Leggi_Da_Id_Attivita(ByVal Piva As String,
                                         ByVal Valutazione_Conto_Cod As Integer,
                                         ByVal Colonna_Attivita As String,
                                         ByVal Validita_Inizio As DateTime,
                                         ByVal Validita_Fine As DateTime,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal xFiltroAggiuntivo As String = "") As Decimal

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_R.Leggi_Da_Id_Attivita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim Valore As Decimal = 0
        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Isnull(SUM(CDG_Testata.Valore_Totale),0) as Totale_CDG ")

            StrSQL.Append(" FROM  CDG_Testata ")

            StrSQL.Append(" INNER JOIN Attivita On (CDG_Testata.Piva = Attivita.Piva ")
            StrSQL.Append("                     And CDG_Testata.Piva_SuperUser = Attivita.Piva_SuperUser ")
            StrSQL.Append("                     And CDG_Testata.Id_Attivita = Attivita.Id_Attivita   ")
            StrSQL.Append("                     And Attivita." & Colonna_Attivita & " = " & Valutazione_Conto_Cod & ")  ")

            StrSQL.Append(" WHERE CDG_Testata.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND CDG_Testata.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   CDG_Testata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   CDG_Testata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Valore = dt(0)("Totale_CDG")

            Return Valore

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Valore = -1
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return Valore

    End Function



    '##############################################################################################
    Public Function Leggi_Da_Piano_Cod(ByVal Piva As String,
                                       ByVal Valutazione_Piano_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal xFiltroAggiuntivo As String = "",
                                       Optional ByVal xOrderBy As String = ""
                                       ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_R.Leggi_Da_Piano_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Dettaglio.* ")

            StrSQL.Append(" FROM  Valutazione_Dettaglio ")
            StrSQL.Append(" INNER JOIN Valutazione_Testata On (Valutazione_Dettaglio.Piva = Valutazione_Testata.Piva And Valutazione_Dettaglio.Id_Testata = Valutazione_Testata.Id_Testata)  ")

            StrSQL.Append(" WHERE Valutazione_Dettaglio.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Valutazione_Dettaglio.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Valutazione_Testata.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Valutazione_Dettaglio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Valutazione_Dettaglio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


Public Class Valutazione_Dettaglio_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Valutazione_Dettaglio As Valutazione_Dettaglio,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_W.Scrivi()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Valutazione_Dettaglio ")
            StrSQL.Append("                   ( Piva_SuperUser, Piva, Id_Testata, Valutazione_Conto_Cod , Anno,  Valore")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valutazione_Dettaglio.Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Id_Testata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Valutazione_Conto_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Anno) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Valore) & "  ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            StrSQL.Append(" )")
            '---------------------------------------------


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


    Public Function Modifica(ByVal Valutazione_Dettaglio As Valutazione_Dettaglio,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_Dettaglio ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Valore = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Valore) & " ")
            StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione) & " ")
            StrSQL.AppendLine("     ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva =    '" & Agro_SQL_SaveText(Valutazione_Dettaglio.Piva) & "' ")
            StrSQL.AppendLine(" AND Id_Testata = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Id_Testata) & " ")
            StrSQL.AppendLine(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Valutazione_Conto_Cod) & " ")
            StrSQL.AppendLine(" AND Anno = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Anno) & " ")

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


    Public Function Modifica_Valore(ByVal Piva As String,
                                    ByVal Id_Testata As Integer,
                                    ByVal Valutazione_Conto_Cod As Integer,
                                    ByVal Anno As Integer,
                                    ByVal Valore As Decimal,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_W.Modifica_Valore()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_Dettaglio ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Valore = " & Agro_SQL_SaveNum(Valore) & " ")
            StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione) & " ")
            StrSQL.AppendLine("     ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            StrSQL.AppendLine(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            StrSQL.AppendLine(" AND Anno = " & Agro_SQL_SaveNum(Anno) & " ")

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


    Public Function Cancella(ByVal Valutazione_Dettaglio As Valutazione_Dettaglio,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio.Cancella()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'Tabella Valutazione_Dettaglio_Specifico
            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Valutazione_Dettaglio.Piva) & "' ")
            StrSQL.Append(" AND ID_Testata = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Id_Testata) & " ")

            If Valutazione_Dettaglio.Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Valutazione_Conto_Cod) & " ")
            End If

            If Valutazione_Dettaglio.Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Anno) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------



            'Tabella Valutazione_Dettaglio
            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Dettaglio ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Dettaglio ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Valutazione_Dettaglio.Piva) & "' ")
            StrSQL.Append(" AND ID_Testata = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Id_Testata) & " ")

            If Valutazione_Dettaglio.Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Valutazione_Conto_Cod) & " ")
            End If
            If Valutazione_Dettaglio.Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(Valutazione_Dettaglio.Anno) & " ")
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



    Public Function Cancella2(ByVal Piva As String,
                              ByVal Id_Testata As Integer,
                              ByVal Valutazione_Conto_Cod As Integer,
                              ByVal Anno As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Dettaglio.Cancella2()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'Tabella Valutazione_Dettaglio_Specifico
            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            'Tabella Valutazione_Dettaglio
            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Dettaglio ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Dettaglio ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If
            If Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(Anno) & " ")
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


End Class


Public Class Valutazione_Dettaglio_Specifico_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Testata As Integer,
                          ByVal Valutazione_Conto_Cod As Integer,
                          ByVal Anno As Integer,
                          ByVal Dettaglio_Key As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = "",
                          Optional ByVal Data As DateTime = AGRODATAINIZIO
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_Specifico_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Dettaglio_Specifico.*, Valutazione_Conto.Valutazione_Conto_Des ")
            StrSQL.Append(" FROM  Valutazione_Dettaglio_Specifico ")

            StrSQL.Append(" INNER JOIN Valutazione_Conto On (Valutazione_Dettaglio_Specifico.Valutazione_Conto_Cod = Valutazione_Conto.Valutazione_Conto_Cod AND Valutazione_Conto.Validita_Fine > " & Agro_SQL_SaveDate(Data) & ") ")

            StrSQL.Append(" WHERE Valutazione_Dettaglio_Specifico.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Valutazione_Dettaglio_Specifico.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Valutazione_Dettaglio_Specifico.ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Dettaglio_Specifico.Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND Valutazione_Dettaglio_Specifico.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            If Trim(Dettaglio_Key) <> "" Then
                StrSQL.Append(" AND Valutazione_Dettaglio_Specifico.Dettaglio_Key = '" & Agro_SQL_SaveText(Dettaglio_Key) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Valutazione_Dettaglio_Specifico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Valutazione_Dettaglio_Specifico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function LeggiImpianti(ByVal Piva As String,
                                  ByVal Id_Testata As Integer,
                                  ByVal Anno As Integer,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_Specifico_R.LeggiImpianti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" Select Impresa, Id_Testata, Specie, Varieta, Regolamento, Anno ")
            StrSQL.AppendLine(" ,SUM(Sup_Imp) AS Sup_Specie, COUNT(Progetto_Cod) AS Num_Esercizi, ROUND(AVG(Produzione_Prevista),2) AS Media_Produzione, Veg_Cod, Cul_Cod  ")
            StrSQL.AppendLine(" From ( ")

            StrSQL.AppendLine(" Select ri.Piva As Impresa, vta1.Id_Testata ")
            StrSQL.AppendLine(" , ISNULL(Veg_Des, 'Terreno Nudo') AS Specie, ISNULL(Cul_Des, '') AS Varieta ")
            StrSQL.AppendLine(" , ri.Validita_Inizio, ri.Validita_Fine, sup_imp, i.Produzione_Prevista, vta1.Anno ")
            StrSQL.AppendLine(" , ISNULL(c.Veg_Cod, 0) AS Veg_Cod, ri.cul_cod, ri.REGOLAMENTO, i.Progetto_Cod --, * ")
            StrSQL.AppendLine(" from Reg_Impianti ri ")
            StrSQL.AppendLine(" inner join Imprese_Progetti i ON ri.Piva = i.Piva and ri.SA_COD = i.Sa_Cod and ri.APPEZZA = i.Appezza and ri.ID_REG = i.Id_Reg ")
            StrSQL.AppendLine(" left join Valutazione_TestataxAnno vta1 ON ri.PIVA = vta1.Piva ")
            StrSQL.AppendLine(" left join Cultivar c on ri.cul_cod = c.Cul_Cod ")
            StrSQL.AppendLine(" left join SpecieVegetali s on c.veg_cod = s.Veg_Cod ")
            StrSQL.AppendLine(" Where (YEAR(ri.Validita_Inizio) <= vta1.Anno AND YEAR(ri.Validita_Fine) >= vta1.Anno)")
            StrSQL.AppendLine(" And ri.Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" And vta1.ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Anno <> 0 Then
                StrSQL.AppendLine(" And vta1.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            StrSQL.AppendLine(" ) y ")

            StrSQL.AppendLine(" Where 1 = 1 ")
            StrSQL.AppendLine(" Group by y.Impresa, y.Id_Testata, y.Specie, y.Varieta, y.Regolamento, y.Anno, y.Veg_Cod, y.CUL_COD")

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


Public Class Valutazione_Dettaglio_Specifico_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Valutazione_Dettaglio_Specifico As Valutazione_Dettaglio_Specifico,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_Specifico_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Valutazione_Dettaglio_Specifico ")
            StrSQL.Append("                   ( Piva_SuperUser, Piva, Id_Testata,  Valutazione_Conto_Cod , Anno,  Dettaglio_Key, ")
            StrSQL.Append("                     Valore_Unitario, Valore_Totale, Valore_Ha, Valore_Peso ")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valutazione_Dettaglio_Specifico.Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Id_Testata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valutazione_Conto_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Anno) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valutazione_Dettaglio_Specifico.Dettaglio_Key) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Unitario) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Totale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Ha) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Peso) & "  ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
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


    Public Function Modifica(ByVal Valutazione_Dettaglio_Specifico As Valutazione_Dettaglio_Specifico,
                             ByVal bAggiornamento As Boolean,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Dettaglio_Specifico_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_Dettaglio_Specifico ")
            StrSQL.AppendLine(" SET ")


            Select Case bAggiornamento

                Case False

                    StrSQL.AppendLine("      Valore_Unitario = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Unitario) & " ")
                    StrSQL.AppendLine("     ,Valore_Totale = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Totale) & " ")

                Case True

                    StrSQL.AppendLine("      Valore_Ha = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Ha) & " ")
                    StrSQL.AppendLine("     ,Valore_Peso = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valore_Peso) & " ")

            End Select


            StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione))
            StrSQL.AppendLine("     ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva =    '" & Agro_SQL_SaveText(Valutazione_Dettaglio_Specifico.Piva) & "' ")
            StrSQL.AppendLine(" AND Id_Testata = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Id_Testata) & " ")
            StrSQL.AppendLine(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Valutazione_Conto_Cod) & " ")
            StrSQL.AppendLine(" AND Anno = " & Agro_SQL_SaveNum(Valutazione_Dettaglio_Specifico.Anno) & " ")
            StrSQL.AppendLine(" AND Dettaglio_Key =    '" & Agro_SQL_SaveText(Valutazione_Dettaglio_Specifico.Dettaglio_Key) & "' ")

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

    Public Function Cancella(ByVal ObjValutazione_Dettaglio_Specifico As Valutazione_Dettaglio_Specifico,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Dettaglio.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(ObjValutazione_Dettaglio_Specifico.Piva) & "' ")
            StrSQL.Append(" AND ID_Testata = " & Agro_SQL_SaveNum(ObjValutazione_Dettaglio_Specifico.Id_Testata) & " ")

            If ObjValutazione_Dettaglio_Specifico.Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(ObjValutazione_Dettaglio_Specifico.Valutazione_Conto_Cod) & " ")
            End If

            If ObjValutazione_Dettaglio_Specifico.Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(ObjValutazione_Dettaglio_Specifico.Anno) & " ")
            End If

            If Trim(ObjValutazione_Dettaglio_Specifico.Dettaglio_Key) <> "" Then
                StrSQL.Append(" AND Dettaglio_Key =    '" & Agro_SQL_SaveText(ObjValutazione_Dettaglio_Specifico.Dettaglio_Key) & "' ")
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



    Public Function Cancella2(ByVal Piva As String,
                              ByVal Id_Testata As Integer,
                              ByVal Valutazione_Conto_Cod As Integer,
                              ByVal Anno As Integer,
                              ByVal Dettaglio_Key As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Dettaglio.Cancella2()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Dettaglio_Specifico ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND ID_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            If Trim(Dettaglio_Key) <> "" Then
                StrSQL.Append(" AND Dettaglio_Key =    '" & Agro_SQL_SaveText(Dettaglio_Key) & "' ")
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


End Class



Public Class Valutazione_Piano_Conti_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################

    Public Function Leggi(ByVal Piva As String,
                          ByVal Valutazione_Piano_Cod As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Piano_Conti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * From Valutazione_Piano_Conti ")

            StrSQL.Append(" WHERE Valutazione_Piano_Conti.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.Append(" AND (Valutazione_Piano_Conti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Valutazione_Piano_Conti.Piva = 'AAAAAAAAAAA'  ) ")

            Else

                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If Not DtImpreseVisibili Is Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND Valutazione_Piano_Conti.piva IN ('AAAAAAAAAAA', " & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                    End If

                    StrSQL.AppendLine(UtenteProfiloImpreseSql)

                End If

            End If


            If Valutazione_Piano_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_Conti.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function

    '##############################################################################################

    Public Function Leggi_x_Griglia(ByVal Piva As String,
                                    ByVal Valutazione_Piano_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal xFiltroAggiuntivo As String = "",
                                    Optional ByVal xOrderBy As String = ""
                                    ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Piano_Conti_R.Leggi_x_Griglia()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT c.Piva_SuperUser, c.Piva, CASE WHEN c.Piva = 'AAAAAAAAAAA' THEN 'Tutte le imprese' ELSE ISNULL(i.rag_soc,'') END AS Rag_Soc ")
            StrSQL.AppendLine("      , c.Valutazione_Piano_Cod, c.Valutazione_Piano_Des, COUNT(t.Id_Testata) AS Num_Valutazioni_Associate ")
            StrSQL.AppendLine("      , c.Data_Creazione, c.Data_Modifica, c.Username_Creazione, c.Username_Modifica, ")
            StrSQL.AppendLine("      CASE WHEN ISNULL(i.partitaIvaReale, '') = '' THEN c.Piva ELSE i.partitaIvaReale END AS PartitaIvaReale  ")

            StrSQL.AppendLine(" FROM Valutazione_Piano_Conti c ")
            StrSQL.AppendLine(" LEFT JOIN Imprese i ON c.Piva = i.Piva ")
            StrSQL.AppendLine(" LEFT JOIN Valutazione_Testata t ON c.Valutazione_Piano_Cod = t.Valutazione_Piano_Cod ")

            StrSQL.AppendLine(" WHERE c.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (c.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR c.Piva = 'AAAAAAAAAAA'  ) ")
            Else

                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If Not DtImpreseVisibili Is Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND c.piva IN ('AAAAAAAAAAA', " & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                    End If

                    StrSQL.AppendLine(UtenteProfiloImpreseSql)

                End If

            End If



            If Valutazione_Piano_Cod <> 0 Then
                StrSQL.AppendLine(" AND c.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   c.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   c.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            StrSQL.AppendLine(" GROUP BY c.Piva_SuperUser, c.Piva, i.rag_soc, c.Valutazione_Piano_Cod, c.Valutazione_Piano_Des, c.Data_Creazione, c.Data_Modifica, c.Username_Creazione, c.Username_Modifica ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function


End Class


Public Class Valutazione_Piano_Conti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal objValutazione_Piano_Conti As Valutazione_Piano_Conti,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Piano_Conti_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Valutazione_Piano_Conti ")
            StrSQL.Append("                   ( Piva_SuperUser, Piva, Valutazione_Piano_Cod , Valutazione_Piano_Des ")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objValutazione_Piano_Conti.primaryKey.Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objValutazione_Piano_Conti.Valutazione_Piano_Des) & "' ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
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

    Public Function Modifica(ByVal objValutazione_Piano_Conti As Valutazione_Piano_Conti,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Piano_Conti_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_Piano_Conti ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Valutazione_Piano_Des = " & Agro_SQL_SaveText_NULL(objValutazione_Piano_Conti.Valutazione_Piano_Des) & " ")
            StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione) & " ")
            StrSQL.AppendLine("     ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva =    '" & Agro_SQL_SaveText(objValutazione_Piano_Conti.primaryKey.Piva) & "' ")
            StrSQL.AppendLine(" AND Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            '---------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal objValutazione_Piano_Conti As Valutazione_Piano_Conti,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Piano_Conti_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Piano_Conti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Piano_Conti ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(objValutazione_Piano_Conti.primaryKey.Piva) & "' ")
            StrSQL.Append(" AND Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod) & " ")

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




Public Class Valutazione_Conto_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################

    Public Function Leggi(ByVal Valutazione_Conto_Cod As Integer,
                          ByVal Valutazione_Sezione_Cod As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = "",
                          Optional ByVal Data As DateTime = AGRODATAINIZIO
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Conto_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * From Valutazione_Conto ")

            StrSQL.Append(" WHERE 1 = 1 ")

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto.Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If

            If Valutazione_Sezione_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Conto.Valutazione_Sezione_Cod = " & Agro_SQL_SaveNum(Valutazione_Sezione_Cod) & " ")
            End If

            'Vengono esclusi i conti con validita_fine = 01/01/1900 (questo perchè i record nel metaschema non possono essere cancellati da aggiornabancadati)
            StrSQL.Append(" AND Valutazione_Conto.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function




    Public Function Leggi2(ByVal Attivo_Passivo As Integer,
                          ByVal Pat_Eco As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Conto_R.Leggi2()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Conto.* From Valutazione_Conto ")

            StrSQL.Append(" Inner Join Valutazione_Sezione On Valutazione_Conto.Valutazione_Sezione_Cod = Valutazione_Sezione.Valutazione_Sezione_Cod ")

            StrSQL.Append(" WHERE 1 = 1 ")

            If Attivo_Passivo <> 0 Then
                StrSQL.Append(" AND Valutazione_Sezione.Attivo_Passivo = " & Agro_SQL_SaveNum(Attivo_Passivo) & " ")
            End If

            If Pat_Eco <> 0 Then
                StrSQL.Append(" AND Valutazione_Sezione.Pat_Eco = " & Agro_SQL_SaveNum(Pat_Eco) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Valutazione_Conto.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Valutazione_Conto.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function


End Class




Public Class Valutazione_Sezione_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################

    Public Function Leggi(ByVal Valutazione_Sezione_Cod As Integer,
                          ByVal Valutazione_Gruppo_Cod As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Conto_R.Valutazione_Sezione_R()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * From Valutazione_Sezione ")

            StrSQL.Append(" WHERE 1 = 1 ")

            If Valutazione_Sezione_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Sezione.Valutazione_Sezione_Cod = " & Agro_SQL_SaveNum(Valutazione_Sezione_Cod) & " ")
            End If

            If Valutazione_Gruppo_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Sezione.Valutazione_Gruppo_Cod = " & Agro_SQL_SaveNum(Valutazione_Gruppo_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function


End Class




Public Class Valutazione_Gruppo_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################

    Public Function Leggi(ByVal Valutazione_Gruppo_Cod As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Conto_R.Valutazione_Gruppo_R()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Distinct Valutazione_Gruppo.Valutazione_Gruppo_Cod, Valutazione_Gruppo.Valutazione_Gruppo_Des, Valutazione_Sezione.Pat_Eco,  Valutazione_Sezione.Attivo_Passivo ")
            StrSQL.Append(" From Valutazione_Gruppo, Valutazione_Sezione ")
            StrSQL.Append(" WHERE Valutazione_Gruppo.Valutazione_Gruppo_Cod = Valutazione_Sezione.Valutazione_Gruppo_Cod ")

            If Valutazione_Gruppo_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Gruppo.Valutazione_Gruppo_Cod = " & Agro_SQL_SaveNum(Valutazione_Gruppo_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Valutazione_Gruppo.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Valutazione_Gruppo.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function


End Class




Public Class Valutazione_Piano_ContixConti_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################

    Public Function Leggi(ByVal Piva As String,
                          ByVal Valutazione_Piano_Cod As Integer,
                          ByVal Valutazione_Conto_Cod As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = "",
                          Optional ByVal Data As DateTime = AGRODATAINIZIO
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Piano_ContixConti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Valutazione_Piano_Conti.Valutazione_Piano_Des, Valutazione_Piano_ContixConti.* ")
            StrSQL.Append(" FROM Valutazione_Piano_ContixConti ")

            StrSQL.Append(" Inner Join Valutazione_Piano_Conti On (Valutazione_Piano_Conti.Piva_SuperUser = Valutazione_Piano_ContixConti.Piva_SuperUser And Valutazione_Piano_Conti.Piva = Valutazione_Piano_ContixConti.Piva And Valutazione_Piano_Conti.Valutazione_Piano_Cod = Valutazione_Piano_ContixConti.Valutazione_Piano_Cod) ")

            StrSQL.Append(" WHERE Valutazione_Piano_ContixConti.Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'Vengono esclusi i conti con validita_fine = 01/01/1900 (questo perchè i record nel metaschema non possono essere cancellati da aggiornabancadati)
            StrSQL.Append(" AND Valutazione_Piano_ContixConti.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")


            If Piva <> "" Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Piva In ('AAAAAAAAAAA', '" & Agro_SQL_SaveText(Piva) & "') ")

            Else

                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If Not DtImpreseVisibili Is Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND Valutazione_Piano_ContixConti.piva IN ('AAAAAAAAAAA', " & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                    End If

                    StrSQL.AppendLine(UtenteProfiloImpreseSql)

                End If

            End If

            If Valutazione_Piano_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(Valutazione_Piano_Cod) & " ")
            End If

            If Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(Valutazione_Conto_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Valutazione_Piano_ContixConti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Valutazione_Piano_ContixConti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function


End Class


Public Class Valutazione_Piano_ContixConti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal objValutazione_Piano_ContixConti As Valutazione_Piano_ContixConti,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Piano_ContixConti_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Valutazione_Piano_ContixConti ")
            StrSQL.Append("                   ( Piva_SuperUser, Piva, Valutazione_Piano_Cod , Valutazione_Conto_Cod, Ordine_PC ")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objValutazione_Piano_ContixConti.primaryKey.Piva) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.primaryKey.Valutazione_Piano_Cod) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.primaryKey.Valutazione_Conto_Cod) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.ordine_pc) & "  ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
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



    Public Function Modifica(ByVal objValutazione_Piano_ContixConti As Valutazione_Piano_ContixConti,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Valutazione_Piano_ContixConti_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Valutazione_Piano_ContixConti ")
            StrSQL.AppendLine(" SET ")
            StrSQL.AppendLine("      Ordine_PC = " & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.ordine_pc) & " ")
            StrSQL.AppendLine("     ,Username_Modifica = " & Agro_SQL_SaveText_NULL(objParametri.UsernameOperazione) & " ")
            StrSQL.AppendLine("     ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" Where Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva =    '" & Agro_SQL_SaveText(objValutazione_Piano_ContixConti.primaryKey.Piva) & "' ")

            If objValutazione_Piano_ContixConti.primaryKey.Valutazione_Piano_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.primaryKey.Valutazione_Piano_Cod) & " ")
            End If

            If objValutazione_Piano_ContixConti.primaryKey.Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.primaryKey.Valutazione_Conto_Cod) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            '---------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal objValutazione_Piano_ContixConti As Valutazione_Piano_ContixConti,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Valutazione_Piano_ContixConti_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Valutazione_Piano_ContixConti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Valutazione_Piano_ContixConti ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Piva =    '" & Agro_SQL_SaveText(objValutazione_Piano_ContixConti.primaryKey.Piva) & "' ")

            If objValutazione_Piano_ContixConti.primaryKey.Valutazione_Piano_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Valutazione_Piano_Cod = " & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.primaryKey.Valutazione_Piano_Cod) & " ")
            End If

            If objValutazione_Piano_ContixConti.primaryKey.Valutazione_Conto_Cod <> 0 Then
                StrSQL.Append(" AND Valutazione_Piano_ContixConti.Valutazione_Conto_Cod = " & Agro_SQL_SaveNum(objValutazione_Piano_ContixConti.primaryKey.Valutazione_Conto_Cod) & " ")
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


End Class

