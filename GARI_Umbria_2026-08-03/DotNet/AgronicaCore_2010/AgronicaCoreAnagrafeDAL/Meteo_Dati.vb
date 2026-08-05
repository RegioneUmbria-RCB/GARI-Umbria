Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Meteo_Dati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'per non filtrare Tempo, passare AGRODATAFINE
    Public Function Leggi(ByVal ID_Origine As Integer, _
                          ByVal Tempo As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Meteo_Dati " + vbCrLf)
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If ID_Origine <> 0 Then
                StrSQL.Append(" AND Meteo_Dati.ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) + vbCrLf)
            End If

            If Tempo <> AGRODATAFINE Then
                StrSQL.Append(" AND Meteo_Dati.Tempo = " & Agro_SQL_SaveDate(Tempo) + "" + vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Meteo_Dati.Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Meteo_Dati.Inviato =-1 " + vbCrLf)
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
    Public Function LeggiJoinMeteoOrigine(ByVal ID_Origine As Integer, _
                                          ByVal Tempo As Date, _
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

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_R.LeggiJoinMeteoOrigine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Meteo_Dati " + vbCrLf)
            StrSQL.Append(" INNER JOIN Meteo_Origine ON Meteo_Origine.Piva_SuperUser = Meteo_Dati.Piva_SuperUser AND Meteo_Origine.ID_Origine = Meteo_Dati.ID_Origine " + vbCrLf)
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



    '#############################################################################################
    Public Function Leggi_dato_anno_e_zona(ByVal ID_Zona As Integer, _
                                          ByVal Anno As Integer, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_R.Leggi_dato_anno_e_zona()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Meteo_Origine.ID_Origine, Meteo_Dati.Unita_Calore, Meteo_Origine.Importato, Meteo_Origine.ID_Zona, Meteo_Dati.Tempo, Meteo_Dati.Temp_Min, Meteo_Dati.Temp_Media, Meteo_Dati.Temp_Max, ")
            StrSQL.Append("         Meteo_Dati.Precipitazione ")
            StrSQL.Append(" FROM         Meteo_Origine INNER JOIN ")
            StrSQL.Append("           Meteo_Dati ON Meteo_Origine.Piva_SuperUser = Meteo_Dati.Piva_SuperUser AND Meteo_Origine.ID_Origine = Meteo_Dati.ID_Origine ")
 
            StrSQL.Append(" WHERE Meteo_Dati.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND Meteo_Origine.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Anno <> 0 Then
                StrSQL.Append(" AND YEAR(Meteo_Dati.Tempo) = " & Agro_SQL_SaveNum(Anno) + vbCrLf)
            End If
               
            If ID_Zona <> 0 Then
                StrSQL.Append(" AND Meteo_Origine.ID_Zona = " & Agro_SQL_SaveNum(ID_Zona) + vbCrLf)
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
    Public Function Elenco_Anni(ByVal ID_Quadrante As Integer, _
                                ByVal ID_Stazione As Integer, _
                                ByVal ID_Zona As Integer, _
                                ByVal Coord_X As Single, _
                                ByVal Coord_Y As Single, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_R.Elenco_Anni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT YEAR(Tempo)as Anno ")
            StrSQL.Append(" FROM  Meteo_Dati " + vbCrLf)
            StrSQL.Append(" INNER JOIN Meteo_Origine ON Meteo_Origine.Piva_SuperUser = Meteo_Dati.Piva_SuperUser AND Meteo_Origine.ID_Origine = Meteo_Dati.ID_Origine " + vbCrLf)
            StrSQL.Append(" WHERE Meteo_Dati.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND Meteo_Origine.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")


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
                    StrSQL.Append(" AND  Meteo_Dati.Inviato =-1 " + vbCrLf)
                    StrSQL.Append(" AND  Meteo_Origine.Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY anno DESC ")

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



Public Class Meteo_Dati_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '###########################################################
    Public Function Scrivi(ByVal ID_Origine As Integer, _
                          ByVal Tempo As Date, _
                          ByVal Flag_G_H As Byte, _
                          ByVal Temp_Min As Single, _
                          ByVal Temp_Media As Single, _
                           ByVal Temp_Max As Single, _
                           ByVal Unita_Calore As Single, _
                           ByVal Precipitazione As Single, _
                           ByVal Bagnatura As Byte, _
                           ByVal Umidita_Min As Byte, _
                           ByVal Umidita_Media As Byte, _
                           ByVal Umidita_Max As Byte, _
                           ByVal Vento_Intensita As Single, _
                           ByVal Vento_Direzione As Integer, _
                           ByVal EvapoTraspirazione As Single, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Meteo_Dati ")
            StrSQL.Append("             (Piva_SuperUser,    ID_Origine,   ")
            StrSQL.Append("               Tempo, Flag_G_H, Temp_Min, Temp_Media, Temp_Max, ")
            StrSQL.Append("               Unita_Calore,   ")
            StrSQL.Append("               Precipitazione, Bagnatura,  ")
            StrSQL.Append("               Umidita_Min, Umidita_Media,  ")
            If Not IsNothing(Unita_Calore) Then
                StrSQL.Append("               Umidita_Max, ")
            End If
            StrSQL.Append("               Vento_Intensita, Vento_Direzione, EvapoTraspirazione, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Origine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Tempo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_G_H) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Temp_Min) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Temp_Media) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Temp_Max) & "  ")
            If Not IsNothing(Unita_Calore) Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Unita_Calore) & "  ")
            End If
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Precipitazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Bagnatura) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Umidita_Min) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Umidita_Media) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Umidita_Max) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Vento_Intensita) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Vento_Direzione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(EvapoTraspirazione) & "  ")

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

    '###########################################################
    Public Function Scrivi_Temp_Precipitazioni( _
                        ByVal ID_Origine As Integer, _
                          ByVal Tempo As Date, _
                          ByVal Flag_G_H As Byte, _
                          ByVal Temp_Min As Single, _
                          ByVal Temp_Media As Single, _
                           ByVal Temp_Max As Single, _
                           ByVal Unita_Calore As Single, _
                           ByVal Precipitazione As Single, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Meteo_Dati ")
            StrSQL.Append("             (Piva_SuperUser,    ID_Origine,   ")
            StrSQL.Append("               Tempo, Flag_G_H, ")
            StrSQL.Append("               Temp_Min, Temp_Media, Temp_Max, ")
            StrSQL.Append("               Precipitazione, ")
            If Not IsNothing(Unita_Calore) Then
                StrSQL.Append("               Unita_Calore,  ")
            End If

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Origine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Tempo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_G_H) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Temp_Min) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Temp_Media) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Temp_Max) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Precipitazione) & "  ")
            If Not IsNothing(Unita_Calore) Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Unita_Calore) & "  ")
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
                              ByVal Tempo As Date, _
                              ByVal Flag_G_H As Byte, _
                              ByVal Temp_Min As Single, _
                              ByVal Temp_Media As Single, _
                               ByVal Temp_Max As Single, _
                               ByVal Unita_Calore As Single, _
                               ByVal Precipitazione As Single, _
                               ByVal Bagnatura As Byte, _
                               ByVal Umidita_Min As Byte, _
                               ByVal Umidita_Media As Byte, _
                               ByVal Umidita_Max As Byte, _
                               ByVal Vento_Intensita As Single, _
                               ByVal Vento_Direzione As Integer, _
                               ByVal EvapoTraspirazione As Single, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.Modifica_Parametrizzata()"

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

            'If Tempo =  Then
            '    Throw New Exception("Parametro non corretto nella query (Tempo obbligatorio)")
            'End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Meteo_Dati SET ")
            StrSQL.Append("    Flag_G_H        = " & Agro_SQL_SaveNum(Flag_G_H) & "")
            StrSQL.Append("    ,Temp_Min        = " & Agro_SQL_SaveNum(Temp_Min) & "")
            StrSQL.Append("    ,Temp_Media      = " & Agro_SQL_SaveNum(Temp_Media) & "")
            StrSQL.Append("    ,Temp_Max        = " & Agro_SQL_SaveNum(Temp_Max) & "")
            If Not IsNothing(Unita_Calore) Then
                StrSQL.Append("    ,Unita_Calore    = " & Agro_SQL_SaveNum(Unita_Calore) & "")
            End If

            StrSQL.Append("    ,Precipitazione  = " & Agro_SQL_SaveNum(Precipitazione) & "")
            StrSQL.Append("    ,Bagnatura       = " & Agro_SQL_SaveNum(Bagnatura) & "")
            StrSQL.Append("    ,Umidita_Min       = " & Agro_SQL_SaveNum(Umidita_Min) & "")
            StrSQL.Append("    ,Umidita_Media       = " & Agro_SQL_SaveNum(Umidita_Media) & "")
            StrSQL.Append("    ,Umidita_Max         = " & Agro_SQL_SaveNum(Umidita_Max) & "")
            StrSQL.Append("    ,Vento_Intensita     = " & Agro_SQL_SaveNum(Vento_Intensita) & "")
            StrSQL.Append("    ,Vento_Direzione     = " & Agro_SQL_SaveNum(Vento_Direzione) & "")
            StrSQL.Append("    ,EvapoTraspirazione  = " & Agro_SQL_SaveNum(EvapoTraspirazione) & "")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")
            StrSQL.Append(" AND   Tempo = " & Agro_SQL_SaveDate(Tempo) & " ")

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
                                            ByVal Tempo As Date, _
                                          ByVal Campo As String, _
                                          ByVal Valore As Object, _
                                               ByVal xFiltroAggiuntivo As String, _
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.Modifica_Parametrizzata()"

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
            StrSQL.Append("UPDATE Meteo_Dati SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")
            StrSQL.Append(" AND   Tempo = " & Agro_SQL_SaveDate(Tempo) & " ")

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
                             ByVal Tempo As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.Cancella()"

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
                StrSQL.Append(" UPDATE Meteo_Dati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")
                StrSQL.Append(" AND     Tempo = " & Agro_SQL_SaveDate(Tempo) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Meteo_Dati ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     ID_Origine = " & Agro_SQL_SaveNum(ID_Origine) & " ")
                StrSQL.Append(" AND     Tempo = " & Agro_SQL_SaveDate(Tempo) & " ")

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
    '###################################################################
    Public Function Cancella(ByVal ID_Origine As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.Cancella()"

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
                StrSQL.Append(" UPDATE Meteo_Dati ")
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
                StrSQL.Append(" FROM    Meteo_Dati ")
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

    '###################################################################
    Public Function CancellaAll(ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Meteo_Dati_W.CancellaAll()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
 

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Meteo_Dati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Meteo_Dati ")
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
