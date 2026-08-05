Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


''' <summary>
''' HELPER
''' </summary>
''' <remarks></remarks>
Public Class Meteo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '############################################################################
    Public Function Delete_Origine_Dati(ByVal Id_Zona As Integer,
                                        ByVal Anno As Integer,
                                        ByVal Veg_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim DT As DataTable
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Meteo_Helper.Delete_Origine_Dati()"

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)


            Dim objM_D As New AgronicaCoreAnagrafeDAL.Meteo_Dati_R
            Dim str As String = ""
            If Veg_Cod <> 0 Then
                str = " Veg_Cod = " & Veg_Cod
            End If
            DT = objM_D.Leggi_dato_anno_e_zona(Id_Zona, Anno, str, "", objParametri)

            Dim i As Integer
            Dim Str_ID As New StringBuilder
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    Str_ID.Append(DT.Rows(i).Item("ID_Origine"))
                    If i <> DT.Rows.Count - 1 Then
                        Str_ID.Append(",")
                    End If
                Next


                'cancello origine
                Dim objO As New AgronicaCoreAnagrafeDAL.Meteo_Origine_W
                objO.CancellaAll(" ID_Origine in (" & Str_ID.ToString & ")", objParametri)


                Dim objD As New AgronicaCoreAnagrafeDAL.Meteo_Dati_W
                objD.CancellaAll(" ID_Origine in (" & Str_ID.ToString & ")", objParametri)

            End If

            xRisp = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = " " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function


    '############################################################################
    Public Function Delete_Origine_Dati(ByVal Id_Zona As Integer,
                                        ByVal Data As Date,
                                        ByVal Veg_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim DT As DataTable
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Meteo_Helper.Delete_Origine_Dati()"

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)


            Dim objM_D As New AgronicaCoreAnagrafeDAL.Meteo_Dati_R
            DT = objM_D.Leggi_dato_anno_e_zona(Id_Zona, 0, "Meteo_Dati.Tempo = " & Agro_SQL_SaveDate(Data, False) & " AND Meteo_Origine.Veg_Cod=" & Veg_Cod, "", objParametri)

            Dim i As Integer
            Dim Str_ID As New StringBuilder
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    Str_ID.Append(DT.Rows(i).Item("ID_Origine"))
                    If i <> DT.Rows.Count - 1 Then
                        Str_ID.Append(",")
                    End If
                Next


                'cancello origine
                Dim objO As New AgronicaCoreAnagrafeDAL.Meteo_Origine_W
                objO.CancellaAll(" ID_Origine in (" & Str_ID.ToString & ")", objParametri)


                Dim objD As New AgronicaCoreAnagrafeDAL.Meteo_Dati_W
                objD.CancellaAll(" ID_Origine in (" & Str_ID.ToString & ")", objParametri)

            End If

            xRisp = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = " " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function


End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Meteo_R
    Inherits AgronicaCoreDataProvider.LogProvider



End Class
