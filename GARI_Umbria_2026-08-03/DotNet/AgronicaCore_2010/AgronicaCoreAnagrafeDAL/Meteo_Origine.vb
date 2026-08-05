Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Meteo_Origine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal ID_Origine As Integer, _
                          ByVal ID_Fonte As Integer, _
                          ByVal ID_Quadrante As Integer, _
                          ByVal ID_Stazione As Integer, _
                          ByVal ID_Zona As Integer, _
                          ByVal Coord_X As Single, _
                          ByVal Coord_Y As Single, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Meteo_Origine " + vbCrLf)
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If ID_Origine <> 0 Then
                StrSQL.Append(" AND ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) + vbCrLf)
            End If

            If ID_Fonte <> 0 Then
                StrSQL.Append(" AND ID_Fonte = " & Agro_SQL_SaveNum(ID_Fonte) + vbCrLf)
            End If

            If ID_Quadrante <> 0 Then
                StrSQL.Append(" AND ID_Quadrante = " & Agro_SQL_SaveNum(ID_Quadrante) + vbCrLf)
            End If

            If ID_Stazione <> 0 Then
                StrSQL.Append(" AND ID_Stazione = " & Agro_SQL_SaveNum(ID_Stazione) + vbCrLf)
            End If

            If ID_Zona <> 0 Then
                StrSQL.Append(" AND ID_Zona = " & Agro_SQL_SaveNum(ID_Zona) + vbCrLf)
            End If

            If Coord_X <> 0 Then
                StrSQL.Append(" AND Coord_X = " & Agro_SQL_SaveNum(Coord_X) + vbCrLf)
            End If

            If Coord_Y <> 0 Then
                StrSQL.Append(" AND Coord_Y = " & Agro_SQL_SaveNum(Coord_Y) + vbCrLf)
            End If

            'If Organismo_Sigla <> "" Then
            '    StrSQL.Append(" AND Organismo_Sigla = '" & Agro_SQL_SaveText(Organismo_Sigla) + "'" + vbCrLf)
            'End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
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


    '##############################################################################################
    Public Function Leggi_da_Zona_Anno(ByVal ID_Zona As Integer, _
                                         ByVal Anno As Integer, _
                                         ByVal Veg_Cod As Integer, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_R.Leggi_da_Zona_Anno()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Meteo_Origine.ID_Origine, Meteo_Dati.Unita_Calore, Meteo_Origine.ID_Zona, Meteo_Dati.Tempo, Meteo_Dati.Temp_Min, Meteo_Dati.Temp_Media, Meteo_Dati.Temp_Max, ")
            StrSQL.Append("             Meteo_Dati.Precipitazione, Meteo_Origine.Importato,  ")
            StrSQL.Append("             CONVERT(nvarchar(25),DAY(Tempo)) + '/' + CONVERT(nvarchar(25),MONTH(Tempo)) as 'gg/mm' ")
            StrSQL.Append(" FROM         Meteo_Origine INNER JOIN ")
            StrSQL.Append("           Meteo_Dati ON Meteo_Origine.Piva_SuperUser = Meteo_Dati.Piva_SuperUser AND Meteo_Origine.ID_Origine = Meteo_Dati.ID_Origine ")


            StrSQL.Append(" WHERE Meteo_Origine.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND  Meteo_Dati.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If ID_Zona <> 0 Then
                StrSQL.Append(" AND Meteo_Origine.ID_Zona = " & Agro_SQL_SaveNum(ID_Zona) + vbCrLf)
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Meteo_Origine.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) + vbCrLf)
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND YEAR(Meteo_Dati.Tempo) = " & Agro_SQL_SaveNum(Anno) + vbCrLf)
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Meteo_Dati.Inviato >=0 " + vbCrLf)
                    StrSQL.Append(" AND   Meteo_Origine.Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Meteo_Dati.Inviato =-1 " + vbCrLf)
                    StrSQL.Append(" AND   Meteo_Origine.Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
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



    '##############################################################################################
    Public Function Leggi_Con_SogliaGerminazione(ByVal ID_PianoSeminaTestata As Integer, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_R.Leggi_Con_SogliaGerminazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Meteo_Dati.Unita_Calore, Meteo_Dati.Tempo, Meteo_Dati.Temp_Min, Meteo_Dati.Temp_Max , '' as Somma, ")
            StrSQL.Append("            (SELECT     Valore FROM          PS_Zone_Specie_Varieta_Default AS ps WHERE      (ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (Cul_Cod = 0) AND (Codice = 14)) AS Soglia_Germinazione  ")

            StrSQL.Append(" FROM         Meteo_Origine INNER JOIN ")
            StrSQL.Append("           Meteo_Dati ON Meteo_Origine.Piva_SuperUser = Meteo_Dati.Piva_SuperUser AND Meteo_Origine.ID_Origine = Meteo_Dati.ID_Origine INNER JOIN ")
            StrSQL.Append("           PS_PianoSeminaTestata ON Meteo_Origine.ID_Zona = PS_PianoSeminaTestata.ID_Zona AND Meteo_Origine.Veg_Cod = PS_PianoSeminaTestata.Veg_Cod AND  ")
            StrSQL.Append("           Meteo_Origine.Piva_SuperUser = PS_PianoSeminaTestata.PivaSuperUser And Year(Meteo_Dati.Tempo) = PS_PianoSeminaTestata.Anno ")

            StrSQL.Append(" WHERE Meteo_Origine.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND  Meteo_Dati.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_PianoSeminaTestata.ID_PianoSeminaTestata = " & Agro_SQL_SaveNum(ID_PianoSeminaTestata) + vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Meteo_Dati.Inviato >=0 " + vbCrLf)
                    StrSQL.Append(" AND   Meteo_Origine.Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Meteo_Dati.Inviato =-1 " + vbCrLf)
                    StrSQL.Append(" AND   Meteo_Origine.Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Tempo ")
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



End Class



Public Class Meteo_Origine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ID_Origine As Integer, _
                           ByVal ID_Fonte As Integer, _
                           ByVal ID_Quadrante As Integer, _
                           ByVal ID_Stazione As Integer, _
                           ByVal ID_Zona As Integer, _
                            ByVal Coord_X As Decimal, _
                            ByVal Coord_Y As Decimal, _
                            ByVal Importato As Boolean, _
                            ByVal Veg_Cod As Integer, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Meteo_Origine ")
            StrSQL.Append("             (Piva_SuperUser,    ID_Origine,   ")
            StrSQL.Append("               ID_Fonte, ID_Quadrante, ID_Stazione, ")
            StrSQL.Append("               ID_Zona, Coord_X, Coord_Y, ")
            If Not IsNothing(Importato) Then
                StrSQL.Append("               Importato, ")
            End If
            If Not IsNothing(Veg_Cod) Then
                StrSQL.Append("               Veg_Cod, ")
            End If



            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Origine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Fonte) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Quadrante) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Stazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Zona) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Coord_X) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Coord_Y) & "  ")
            If Not IsNothing(Importato) Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(CInt(Importato)) & "  ")
            End If
            If Not IsNothing(Veg_Cod) Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(") ")

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


    '########################################################
    Public Function Modifica(ByVal ID_Origine As Integer, _
                           ByVal ID_Fonte As Integer, _
                           ByVal ID_Quadrante As Integer, _
                           ByVal ID_Stazione As Integer, _
                           ByVal ID_Zona As Integer, _
                            ByVal Coord_X As Decimal, _
                            ByVal Coord_Y As Decimal, _
                            ByVal Importato As Boolean, _
                            ByVal Veg_Cod As Integer, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_W.Modifica_Parametrizzata()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID_Origine = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Origine obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Meteo_Origine SET ")
            StrSQL.Append("    ID_Fonte         = " & Agro_SQL_SaveNum(ID_Fonte) & "")
            StrSQL.Append("    ,ID_Quadrante    = " & Agro_SQL_SaveNum(ID_Quadrante) & "")
            StrSQL.Append("    ,ID_Stazione    = " & Agro_SQL_SaveNum(ID_Stazione) & "")
            StrSQL.Append("    ,ID_Zona    = " & Agro_SQL_SaveNum(ID_Zona) & "")
            StrSQL.Append("    ,Coord_X    = " & Agro_SQL_SaveNum(Coord_X) & "")
            StrSQL.Append("    ,Coord_Y    = " & Agro_SQL_SaveNum(Coord_Y) & "")
            If Not IsNothing(Importato) Then
                StrSQL.Append("    ,Importato    = " & Agro_SQL_SaveNum(CInt(Importato)) & "")
            End If
            If Not IsNothing(Veg_Cod) Then
                StrSQL.Append("    ,Veg_Cod    = " & Agro_SQL_SaveNum(Veg_Cod) & "")
            End If

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    '##############################################################################################
    Public Function Modifica_Parametrizzata(ByVal ID_Origine As Integer, _
                                          ByVal Campo As String, _
                                          ByVal Valore As Object, _
                                               ByVal xFiltroAggiuntivo As String, _
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_W.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try

            If ID_Origine = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Origine obbligatorio)")
            End If

            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then

                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            ElseIf TypeVal.Equals(Data) Then

                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "

            Else

                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "

            End If


            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Meteo_Origine SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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



    '###################################################################
    Public Function Cancella(ByVal ID_Origine As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID_Origine = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Origine obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Meteo_Origine ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Meteo_Origine ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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



    Public Function CancellaAll(ByVal xFiltroAggiuntivo As String, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Origine_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
             

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Meteo_Origine ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Meteo_Origine ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
