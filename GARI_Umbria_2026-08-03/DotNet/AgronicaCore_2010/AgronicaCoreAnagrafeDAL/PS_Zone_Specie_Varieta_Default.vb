Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class PS_Zone_Specie_Varieta_Default_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi( _
                                ByVal ID_Zona As Integer, _
                                ByVal Veg_Cod As Integer, _
                                ByVal Cul_Cod As Integer, _
                                ByVal Codice As Integer, _
                                ByVal Valore As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default_W.Scrivi()"

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

            StrSQL.Append("INSERT INTO PS_Zone_Specie_Varieta_Default( ")
            StrSQL.Append("            PivaSuperUser,        ID_Zona,            ")
            StrSQL.Append("            Veg_Cod,             Cul_Cod,        ")
            StrSQL.Append("            Codice,             Valore,        ")

            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Zona) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Codice) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Valore) & "'  ")

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
                                ByVal ID_Zona As Integer, _
                                ByVal Veg_Cod As Integer, _
                                ByVal Cul_Cod As Integer, _
                                ByVal Codice As Integer, _
                                ByVal Valore As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default_W.Modifica()"

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


            StrSQL.Append("UPDATE PS_Zone_Specie_Varieta_Default SET ")
            StrSQL.Append("    Codice           = " & Agro_SQL_SaveText(Codice))
            StrSQL.Append("   ,Valore       = '" & Agro_SQL_SaveText(Valore) & "' ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_Zona = " & ID_Zona & " ")
            StrSQL.Append(" AND   Veg_Cod = " & Veg_Cod & " ")
            StrSQL.Append(" AND   Cul_cod = " & Cul_Cod & " ")
             
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
                            ByVal ID_Zona As Integer, _
                            ByVal Veg_cod As Integer, _
                            ByVal Cul_cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default_W.Cancella()"

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

                StrSQL.Append(" UPDATE  PS_Zone_Specie_Varieta_Default ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     PS_Zone_Specie_Varieta_Default ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato = 0 ")

            End If
            
            If ID_Zona <> 0 Then
                StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.ID_Zona = " & ID_Zona & " ")
            End If
            If Veg_cod <> 0 Then
                StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.Veg_cod = " & Veg_cod & " ")
            End If
            If Cul_cod <> 0 Then
                StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.Cul_cod = " & Cul_cod & " ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
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
Public Class PS_Zone_Specie_Varieta_Default

    Public Shared Function CalcolaUnitaCalore(ByVal t_max As String, ByVal t_min As String, ByVal SogliaGerminazione As String)
        t_max = t_max.Replace(".", ",")
        t_min = t_min.Replace(".", ",")
        SogliaGerminazione = SogliaGerminazione.Replace(".", ",")

        Dim app As Decimal
        app = ((CInt(t_max) + CInt(t_min)) / 2) - CDbl(SogliaGerminazione)
        'app = ((CDbl(t_max) + CDbl(t_min)) / 2) - CDbl(SogliaGerminazione)
        If app > 0 Then
            Return Math.Round(app, 1)
        Else
            Return 0
        End If
    End Function

End Class

Public Class PS_Zone_Specie_Varieta_Default_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID_Zona As Integer,
                          ByVal Veg_cod As Integer,
                          ByVal Cul_cod As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM   PS_Zone_Specie_Varieta_Default ")
                    StrSQL.Append(" WHERE  PS_Zone_Specie_Varieta_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If ID_Zona <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.ID_Zona = " & ID_Zona & " ")
                    End If

                    If Veg_cod <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.Veg_cod = " & Veg_cod)
                    End If

                    If Cul_cod <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.Cul_cod = " & Cul_cod)
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
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PS_Zone_Specie_Varieta_Default.Veg_Cod, PS_Zone_Specie_Varieta_Default.Cul_Cod, PS_Zone_Specie_Varieta_Default.Codice,  ")
                    StrSQL.Append("       PS_Zone_Specie_Varieta_Default.Valore, PS_Zone_Specie_Varieta_Default.ID_Zona,SpecieVegetali.Veg_Des,  Cultivar.Cul_Des ")
                    StrSQL.Append(" FROM         PS_Zone_Specie_Varieta_Default LEFT OUTER JOIN ")
                    StrSQL.Append("         SpecieVegetali ON PS_Zone_Specie_Varieta_Default.Veg_Cod = SpecieVegetali.Veg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("         Cultivar ON PS_Zone_Specie_Varieta_Default.Veg_Cod = Cultivar.Veg_Cod AND PS_Zone_Specie_Varieta_Default.Cul_Cod = Cultivar.Cul_Cod ")
                    StrSQL.Append(" WHERE  PS_Zone_Specie_Varieta_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If ID_Zona <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.ID_Zona = " & ID_Zona & " ")
                    End If

                    If Veg_cod <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.Veg_cod = " & Veg_cod)
                    End If

                    If Cul_cod <> 0 Then
                        StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.Cul_cod = " & Cul_cod)
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
                        StrSQL.Append(" order by veg_des ")
                    End If
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
    Public Function Leggi_Distinct_Veg_Des(ByVal ID_Zona As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PS_Zone_Specie_Varieta_Default.Veg_Cod, SpecieVegetali.Veg_Des ")
            StrSQL.Append(" FROM         PS_Zone_Specie_Varieta_Default INNER JOIN ")
            StrSQL.Append("       SpecieVegetali ON PS_Zone_Specie_Varieta_Default.Veg_Cod = SpecieVegetali.Veg_Cod ")

            StrSQL.Append(" WHERE  PS_Zone_Specie_Varieta_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_Zona <> 0 Then
                StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.ID_Zona = " & ID_Zona & " ")
            End If
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
    Public Function Leggi_Distinct_Veg_Des_Utilizzati_in_Meteo(ByVal ID_Zona As Integer, _
                                ByVal Anno As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PS_Zone_Specie_Varieta_Default.Veg_Cod, SpecieVegetali.Veg_Des ")
            StrSQL.Append(" FROM         PS_Zone_Specie_Varieta_Default INNER JOIN ")
            StrSQL.Append("       SpecieVegetali ON PS_Zone_Specie_Varieta_Default.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.Append("       Meteo_Origine ON PS_Zone_Specie_Varieta_Default.PivaSuperUser = Meteo_Origine.Piva_SuperUser AND  ")
            StrSQL.Append("       PS_Zone_Specie_Varieta_Default.ID_Zona = Meteo_Origine.ID_Zona AND PS_Zone_Specie_Varieta_Default.Veg_Cod = Meteo_Origine.Veg_Cod INNER JOIN ")
            StrSQL.Append("       Meteo_Dati ON Meteo_Origine.Piva_SuperUser = Meteo_Dati.Piva_SuperUser AND Meteo_Origine.ID_Origine = Meteo_Dati.ID_Origine ")

            StrSQL.Append(" WHERE  PS_Zone_Specie_Varieta_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Zone_Specie_Varieta_Default.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_Zona <> 0 Then
                StrSQL.Append(" AND PS_Zone_Specie_Varieta_Default.ID_Zona = " & ID_Zona & " ")
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND YEAR(Meteo_Dati.Tempo) = " & Agro_SQL_SaveNum(Anno) + vbCrLf)
            End If


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