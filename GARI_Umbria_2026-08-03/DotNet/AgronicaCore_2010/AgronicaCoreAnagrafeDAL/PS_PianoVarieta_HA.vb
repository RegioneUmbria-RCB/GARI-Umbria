Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class PS_PianoVarieta_HA_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal Cul_Cod As Integer, _
                                ByVal HA_Disponibili As Decimal, _
                                ByVal HA_Semina As Decimal, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_W.Scrivi()"

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

            StrSQL.Append("INSERT INTO PS_PianoVarieta_HA( ")
            StrSQL.Append("            PivaSuperUser,        ID_PianoSeminaTestata,            ")
            StrSQL.Append("            Cul_Cod,  ")
            StrSQL.Append("            HA_Disponibili,             HA_Semina,        ")

            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_PianoSeminaTestata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(HA_Disponibili) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(HA_Semina) & " ")


            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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
                                ByVal Cul_Cod As Integer, _
                                ByVal HA_Disponibili As Integer, _
                                ByVal HA_Semina As Integer, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_W.Modifica()"

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


            StrSQL.Append("UPDATE PS_PianoVarieta_HA SET ")
            StrSQL.Append("    HA_Disponibili      = " & Agro_SQL_SaveText(HA_Disponibili))
            StrSQL.Append("   ,HA_Semina           = " & Agro_SQL_SaveNum(HA_Semina))

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            StrSQL.Append(" AND   Cul_Cod = " & Cul_Cod & " ")

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
    Public Function Modifica_HA_Seminati( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal Cul_Cod As Integer, _
                                ByVal HA_Semina As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_W.Modifica()"

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


            StrSQL.Append("UPDATE PS_PianoVarieta_HA SET ")
            StrSQL.Append("   HA_Semina           = " & Agro_SQL_SaveNum(HA_Semina))

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            StrSQL.Append(" AND   Cul_Cod = " & Cul_Cod & " ")

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
                            ByVal Cul_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_W.Cancella()"

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

                StrSQL.Append(" UPDATE  PS_PianoVarieta_HA ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     PS_PianoVarieta_HA ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato = 0 ")

            End If

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_PianoVarieta_HA.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If
            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND PS_PianoVarieta_HA.Cul_Cod = " & Cul_Cod & " ")
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

Public Class PS_PianoVarieta_HA_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID_PianoSeminaTestata As Integer, _
                          ByVal Cul_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM   PS_PianoVarieta_HA ")
                    StrSQL.Append(" WHERE  PS_PianoVarieta_HA.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    PS_PianoVarieta_HA.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    PS_PianoVarieta_HA.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If ID_PianoSeminaTestata <> 0 Then
                        StrSQL.Append(" AND PS_PianoVarieta_HA.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND PS_PianoVarieta_HA.Cul_Cod = " & Cul_Cod)
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ID_PianoSeminaTestata.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ID_PianoSeminaTestata.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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



    '##############################################################################################
    Public Function Leggi_con_UnitaCalore_e_Cul_Des(ByVal ID_PianoSeminaTestata As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_R.Leggi_con_UnitaCalore_e_Cul_Des()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT     PS_PianoVarieta_HA.ID_PianoSeminaTestata, PS_PianoVarieta_HA.Cul_Cod, PS_PianoVarieta_HA.HA_Disponibili, PS_PianoVarieta_HA.HA_Semina, ")
            StrSQL.Append("   CONVERT(real,(SELECT     Valore FROM          PS_Zone_Specie_Varieta_Default AS p  WHERE      (ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (Cul_Cod = PS_PianoVarieta_HA.Cul_Cod) AND  (Codice = 5))) AS Unita_Calore, ")
            StrSQL.Append("   (SELECT     Cul_Des FROM          Cultivar AS c WHERE      (PS_PianoSeminaTestata.Veg_Cod = Veg_Cod) AND (PS_PianoVarieta_HA.Cul_Cod = Cul_Cod)) AS Cul_Des, ")
            StrSQL.Append("   (SELECT     Valore FROM          PS_Zone_Specie_Varieta_Default AS ps WHERE      (ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (Cul_Cod = PS_PianoVarieta_HA.Cul_Cod) AND (Codice = 4)) AS Resa ")
            StrSQL.Append(" , PS_PianoSeminaTestata.Veg_Cod , PS_PianoSeminaTestata.ID_Zona ")

            StrSQL.Append("   FROM         PS_PianoVarieta_HA INNER JOIN ")
            StrSQL.Append("         PS_PianoSeminaTestata ON PS_PianoVarieta_HA.PivaSuperUser = PS_PianoSeminaTestata.PivaSuperUser AND ")
            StrSQL.Append("         PS_PianoVarieta_HA.ID_PianoSeminaTestata = PS_PianoSeminaTestata.ID_PianoSeminaTestata ")

            StrSQL.Append(" WHERE  PS_PianoVarieta_HA.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_PianoVarieta_HA.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_PianoVarieta_HA.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")



            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_PianoVarieta_HA.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PS_PianoVarieta_HA.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PS_PianoVarieta_HA.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Unita_Calore asc   ")
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





    '##############################################################################################
    Public Function Leggi_x_Grid(ByVal ID_PianoSeminaTestata As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    Stb.Length = 0


                    Stb.AppendLine(" Select ")
                    Stb.AppendLine("   PS_PianoVarieta_HA.Cul_Cod ")
                    Stb.AppendLine(" , PS_PianoVarieta_HA.HA_Disponibili ")
                    Stb.AppendLine(" , PS_PianoVarieta_HA.HA_Semina ")
                    Stb.AppendLine(" , PS_PianoVarieta_HA.ID_PianoSeminaTestata ")
                    Stb.AppendLine(" , PS_PianoSeminaTestata.Veg_Cod ")
                    Stb.AppendLine(" , PS_Zone_Specie_Varieta_Default.Codice ")
                    Stb.AppendLine(" , PS_Zone_Specie_Varieta_Default.Valore ")
                    Stb.AppendLine(" , Cultivar.Cul_Des   ")
                    Stb.AppendLine(" From PS_PianoVarieta_HA  ")
                    Stb.AppendLine("         INNER Join PS_PianoSeminaTestata  ")
                    Stb.AppendLine("             On PS_PianoVarieta_HA.PivaSuperUser = PS_PianoSeminaTestata.PivaSuperUser  ")
                    Stb.AppendLine("             And PS_PianoVarieta_HA.ID_PianoSeminaTestata = PS_PianoSeminaTestata.ID_PianoSeminaTestata  ")
                    Stb.AppendLine("         INNER Join PS_Zone_Specie_Varieta_Default  ")
                    Stb.AppendLine("             On PS_PianoVarieta_HA.PivaSuperUser = PS_Zone_Specie_Varieta_Default.PivaSuperUser  ")
                    Stb.AppendLine("         And PS_PianoSeminaTestata.ID_Zona = PS_Zone_Specie_Varieta_Default.ID_Zona  ")
                    Stb.AppendLine("         And PS_PianoSeminaTestata.Veg_Cod = PS_Zone_Specie_Varieta_Default.Veg_Cod  ")
                    Stb.AppendLine("         And PS_PianoVarieta_HA.Cul_Cod = PS_Zone_Specie_Varieta_Default.Cul_Cod  ")
                    Stb.AppendLine("         INNER Join Cultivar  ")
                    Stb.AppendLine("             On PS_Zone_Specie_Varieta_Default.Veg_Cod = Cultivar.Veg_Cod  ")
                    Stb.AppendLine("             And PS_Zone_Specie_Varieta_Default.Cul_Cod = Cultivar.Cul_Cod ")
                    Stb.AppendLine(" ")


                    Stb.Append(" WHERE  PS_PianoVarieta_HA.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    Stb.Append(" AND    PS_PianoVarieta_HA.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    Stb.Append(" AND    PS_PianoVarieta_HA.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
                    Stb.Append(" AND    PS_Zone_Specie_Varieta_Default.Codice = 5")

                    If ID_PianoSeminaTestata <> 0 Then
                        Stb.Append(" AND PS_PianoVarieta_HA.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
                    End If




                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            Stb.Append(" AND   PS_PianoVarieta_HA.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            Stb.Append(" AND   PS_PianoVarieta_HA.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        Stb.Append("ORDER BY CONVERT(real,Valore) asc")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

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






    '##############################################################################################
    Public Function Leggi_Veg_Cod_ancoranonutilizzati(ByVal ID_PianoSeminaTestata As Integer,
                          ByVal ID_Zona As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT     PS_Zone_Specie_Varieta_Default.ID_Zona, PS_Zone_Specie_Varieta_Default.Veg_Cod, PS_Zone_Specie_Varieta_Default.Cul_Cod, Cultivar.Cul_Des, PS_Zone_Specie_Varieta_Default.Valore ")
                    StrSQL.Append(" FROM      PS_Zone_Specie_Varieta_Default INNER JOIN ")
                    StrSQL.Append("           PS_PianoSeminaTestata AS PST ON PS_Zone_Specie_Varieta_Default.PivaSuperUser = PST.PivaSuperUser AND ")
                    StrSQL.Append("           PS_Zone_Specie_Varieta_Default.ID_Zona = PST.ID_Zona AND PS_Zone_Specie_Varieta_Default.Veg_Cod = PST.Veg_Cod INNER JOIN ")
                    StrSQL.Append("           Cultivar ON PS_Zone_Specie_Varieta_Default.Cul_Cod = Cultivar.Cul_Cod AND PS_Zone_Specie_Varieta_Default.Veg_Cod = Cultivar.Veg_Cod ")

                    StrSQL.Append(" WHERE  PS_Zone_Specie_Varieta_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    StrSQL.Append(" and PS_Zone_Specie_Varieta_Default.cul_cod not in ")
                    StrSQL.Append(" (select p.Cul_Cod from PS_PianoVarieta_HA as  p ")
                    StrSQL.Append("  where p.ID_PianoSeminaTestata = pst.ID_PianoSeminaTestata)")

                    StrSQL.Append(" AND (PS_Zone_Specie_Varieta_Default.Cul_Cod <> 0) ")
                    StrSQL.Append(" AND (PS_Zone_Specie_Varieta_Default.Codice = 5) ")



                    If ID_PianoSeminaTestata <> 0 Then
                        StrSQL.Append(" AND pst.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
                    End If

                    If ID_Zona <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.ID_Zona = " & ID_Zona)
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PS_Zone_Specie_Varieta_Default.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PS_Zone_Specie_Varieta_Default.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY CONVERT(real,Valore) asc ")
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