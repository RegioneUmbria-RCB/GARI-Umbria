Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PS_PianoSeminaTestata_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal Veg_Cod As Integer, _
                                ByVal ID_Zona As Integer, _
                                ByVal Anno As Integer, _
                                ByVal PianoSeminaTestata_Descrizione As String, _
                                ByVal PianoSeminaTestata_Note As String, _
                                ByVal pianoSemina As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoSeminaTestata_W.Scrivi()"

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

            StrSQL.Append("INSERT INTO PS_PianoSeminaTestata( ")
            StrSQL.Append("            PivaSuperUser,        ID_PianoSeminaTestata,            ")
            StrSQL.Append("            Veg_Cod,             ID_Zona,        ")
            StrSQL.Append("            Anno,        ")
            StrSQL.Append("            PianoSeminaTestata_Descrizione, PianoSeminaTestata_Note, ")

            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine , PS_PianoSeminaTestata.PS_assoc_Reg_Impianti")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_PianoSeminaTestata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Zona) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Anno) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(PianoSeminaTestata_Descrizione) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(PianoSeminaTestata_Note) & "'  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(pianoSemina) & "'  ")

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


    '#########################################################################
    Public Function Modifica( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal Veg_Cod As Integer, _
                                ByVal ID_Zona As Integer, _
                                ByVal Anno As Integer, _
                                ByVal PianoSeminaTestata_Descrizione As String, _
                                ByVal PianoSeminaTestata_Note As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    Optional ByVal PS_assoc_Reg_Impianti As String = "" _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoSeminaTestata_W.Modifica()"

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


            StrSQL.Append("UPDATE PS_PianoSeminaTestata SET ")
            StrSQL.Append("    Veg_Cod            = " & Agro_SQL_SaveNum(Veg_Cod))
            StrSQL.Append("   ,ID_Zona       = " & Agro_SQL_SaveNum(ID_Zona) & " ")
            StrSQL.Append("   ,Anno       = " & Agro_SQL_SaveNum(Anno) & " ")
            StrSQL.Append("   ,PianoSeminaTestata_Descrizione       = '" & Agro_SQL_SaveText(PianoSeminaTestata_Descrizione) & "' ")
            StrSQL.Append("   ,PianoSeminaTestata_Note       = '" & Agro_SQL_SaveText(PianoSeminaTestata_Note) & "' ")

            If PS_assoc_Reg_Impianti <> "" Then
                StrSQL.Append("   , PS_PianoSeminaTestata.PS_assoc_Reg_Impianti       = '" & Agro_SQL_SaveText(PS_assoc_Reg_Impianti) & "' ")
            End If


            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")

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


    '#########################################################################
    Public Function Cancella( _
                            ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoSeminaTestata_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

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

                StrSQL.Append(" UPDATE  PS_PianoSeminaTestata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     PS_PianoSeminaTestata ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato = 0 ")

            End If

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_PianoSeminaTestata.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
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

'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################

Public Class PS_PianoSeminaTestata_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoSeminaTestata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  PS_PianoSeminaTestata.ID_PianoSeminaTestata, PS_PianoSeminaTestata.Veg_Cod, PS_PianoSeminaTestata.ID_Zona, ")
                    StrSQL.Append("         PS_PianoSeminaTestata.PianoSeminaTestata_Descrizione, PS_PianoSeminaTestata.Anno, PS_PianoSeminaTestata.PianoSeminaTestata_Note, PS_Zone.Zona_Codice, ")
                    StrSQL.Append("         SpecieVegetali.Veg_Des , PS_Zone.Zona_Descrizione , PS_PianoSeminaTestata.Validita_Inizio, PS_PianoSeminaTestata.Validita_Fine, PS_PianoSeminaTestata.PS_assoc_Reg_Impianti")
                    StrSQL.Append(" FROM         PS_PianoSeminaTestata INNER JOIN ")
                    StrSQL.Append("       PS_Zone ON PS_PianoSeminaTestata.PivaSuperUser = PS_Zone.PivaSuperUser AND PS_PianoSeminaTestata.ID_Zona = PS_Zone.ID_Zona INNER JOIN ")
                    StrSQL.Append("       SpecieVegetali ON PS_PianoSeminaTestata.Veg_Cod = SpecieVegetali.Veg_Cod ")


                    StrSQL.Append(" WHERE  PS_PianoSeminaTestata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    PS_PianoSeminaTestata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    PS_PianoSeminaTestata.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If ID_PianoSeminaTestata <> 0 Then
                        StrSQL.Append(" AND PS_PianoSeminaTestata.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PS_PianoSeminaTestata.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PS_PianoSeminaTestata.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY PS_PianoSeminaTestata.Validita_Inizio,Veg_Des, Zona_Codice ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

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