Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Stringa_PrincipiAttivi_R

    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function Stringa_PA_Validi(ByVal Id_RcDpi As Int32,
                                      ByVal Disciplinare_Cod As Int32,
                                      ByVal TipoTestata As Int32,
                                      ByVal Av_Gru As Int32,
                                      ByVal Av_Cod As Int32,
                                      ByVal strAvversita As String,
                                      ByVal Modulo As Int32,
                                      ByVal Ep_Cod As Int32,
                                      ByVal Data As Date,
                                      ByVal strListaComuni As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal Stato_Impianto As Integer = 0
                                      ) As String

        Const nomeRoutine = "DpiBIZ.Stringa_PrincipiAttivi_R.Stringa_PA_Validi()"

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim i As Int32
        Dim j As Int32

        Dim risultatoFunzione As String = String.Empty

        Dim objDpi As AgronicaCoreDpiDAL.Dpi_R

        Dim dt As DataTable

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------


            'Lettura dei principi attivi validi per il disciplinare

            objDpi = New AgronicaCoreDpiDAL.Dpi_R

            dt = objDpi.Leggi_PrincipiAttivi_Da_Infestanti_Deroghe(Id_RcDpi,
                                                        Disciplinare_Cod,
                                                        TipoTestata,
                                                        0,
                                                        Av_Gru,
                                                        Av_Cod,
                                                        strAvversita,
                                                        Ep_Cod,
                                                        Modulo,
                                                        Data,
                                                        strListaComuni,
                                                        "", "",
                                                        objParametri,
                                                        Stato_Impianto)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For i = 0 To dt.Rows.Count - 1

                    If Not IsDBNull(dt.Rows(i).Item("Gru_PA_Cod")) AndAlso dt.Rows(i).Item("Gru_PA_Cod") <> 0 Then

                        'Considero il Gpa_Cod
                        Dim DtGPA As DataTable

                        DtGPA = objDpi.Leggi_GruppiPrincipiAttivixPrincipiAttivi(Agro_SQL_SaveNum(dt.Rows(i).Item("Gru_PA_Cod")),
                                                                                 0,
                                                                                 "", "",
                                                                                 objParametri)

                        If DtGPA IsNot Nothing Then
                            'Inserimento dei Singoli PA all'interno della stringa dei pa_validi
                            For j = 0 To DtGPA.Rows.Count - 1
                                Stringa_PA_Validi = Stringa_PA_Validi & IIf(Trim(Stringa_PA_Validi) = "", "", ", ") & Agro_SQL_SaveNum(DtGPA.Rows(j).Item("Pa_Cod"))
                            Next
                        End If

                    End If

                    If Not IsDBNull(dt.Rows(i).Item("Pa_Cod")) AndAlso dt.Rows(i).Item("Pa_Cod") <> 0 Then

                        'Considero il Pa_Cod
                        Stringa_PA_Validi = Stringa_PA_Validi & IIf(Trim(Stringa_PA_Validi) = "", "", ", ") & Agro_SQL_SaveNum(dt.Rows(i).Item("Pa_Cod"))

                    End If

                Next

                If Stringa_PA_Validi <> "" Then
                    Stringa_PA_Validi = "( " & Stringa_PA_Validi & ")"
                Else
                    Stringa_PA_Validi = "( -1000 )" 'Dummy
                End If

            Else
                Stringa_PA_Validi = "( -1000 )" 'Dummy
            End If


        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Stringa_PA_Validi = "( -1000 )"

        Finally

            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

    End Function


    'a differenza della precedente chiama Leggi_PrincipiAttivi_Da_Infestanti_2
    '- Id_RcDpi è una stringa di valori
    '- La testata non è obbligatoria (TipoTestata=-1)
    Public Function Stringa_PA_Validi_2(ByVal Id_RcDpi As String,
                                        ByVal Disciplinare_Cod As Int32,
                                        ByVal TipoTestata As Int32,
                                        ByVal Av_Gru As Int32,
                                        ByVal Av_Cod As Int32,
                                        ByVal strAvversita As String,
                                        ByVal Modulo As Int32,
                                        ByVal Ep_Cod As Int32,
                                        ByVal Data As Date,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As String

        Const nomeRoutine = "DpiBIZ.Stringa_PrincipiAttivi_R.Stringa_PA_Validi_2()"

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim i As Int32
        Dim j As Int32

        Dim risultatoFunzione As String = String.Empty

        Dim objDpi As AgronicaCoreDpiDAL.Dpi_R

        Dim dt As DataTable

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance().CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------


            'Lettura dei principi attivi validi per il disciplinare

            objDpi = New AgronicaCoreDpiDAL.Dpi_R

            dt = objDpi.Leggi_PrincipiAttivi_Da_Infestanti_2(Id_RcDpi,
                                                        Disciplinare_Cod,
                                                        TipoTestata,
                                                        0,
                                                        Av_Gru,
                                                        Av_Cod,
                                                        strAvversita,
                                                        Ep_Cod,
                                                        Modulo,
                                                         "", "",
                                                         objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For i = 0 To dt.Rows.Count - 1

                    If Not IsDBNull(dt.Rows(i).Item("Gru_PA_Cod")) AndAlso dt.Rows(i).Item("Gru_PA_Cod") <> 0 Then

                        'Considero il Gpa_Cod
                        Dim DtGPA As DataTable

                        DtGPA = objDpi.Leggi_GruppiPrincipiAttivixPrincipiAttivi(Agro_SQL_SaveNum(dt.Rows(i).Item("Gru_PA_Cod")),
                                                                                 0,
                                                                                 "", "",
                                                                                 objParametri)

                        If DtGPA IsNot Nothing Then
                            'Inserimento dei Singoli PA all'interno della stringa dei pa_validi
                            For j = 0 To DtGPA.Rows.Count - 1
                                Stringa_PA_Validi_2 = Stringa_PA_Validi_2 & IIf(Trim(Stringa_PA_Validi_2) = "", "", ", ") & Agro_SQL_SaveNum(DtGPA.Rows(j).Item("Pa_Cod"))
                            Next
                        End If

                    End If

                    If Not IsDBNull(dt.Rows(i).Item("Pa_Cod")) AndAlso dt.Rows(i).Item("Pa_Cod") <> 0 Then

                        'Considero il Pa_Cod
                        Stringa_PA_Validi_2 = Stringa_PA_Validi_2 & IIf(Trim(Stringa_PA_Validi_2) = "", "", ", ") & Agro_SQL_SaveNum(dt.Rows(i).Item("Pa_Cod"))

                    End If

                Next

                If Stringa_PA_Validi_2 <> "" Then
                    Stringa_PA_Validi_2 = "( " & Stringa_PA_Validi_2 & ")"
                Else
                    Stringa_PA_Validi_2 = "( -1000 )" 'Dummy
                End If

            Else
                Stringa_PA_Validi_2 = "( -1000 )" 'Dummy
            End If


        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Stringa_PA_Validi_2 = "( -1000 )"

        Finally

            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

    End Function

End Class
