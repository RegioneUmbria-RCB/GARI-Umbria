Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class ConfigurazioneStampe

    Public Property PivaSuperUser As String
    Public Property Piva As String
    Public Property enum_CodificaStampe As enum_CodificaStampe
    Public Property NomeStampa As String
    Public Property PathFileAspx As String
    Public Property NomeFileAspx As String
    Public Property PathFileRpt As String
    Public Property NomeFileRpt As String
    Public Property ModuloGenerazione As enum_Omni_Modulo_Generazione
    Public Property CodRisUm_PrimoCessionario As String
    Public Property PosizioneLogo As String
    Public Property SalvataggioTemporaneo As Integer
    Public Property SezioniDaNascondereString As String
    Public Property SezioniDaNascondereArray As String()
    Public Property Flag_ConfezioniDesc As Boolean
    Public Property Flag_ContenitoriDesc As Boolean
    Public Property Flag_ImballaggiDesc As Boolean
    Public Property Flag_CertificazioniDesc As Boolean
    Public Property Flag_RiepilogoConfezioni As Boolean
    Public Property Flag_RiepilogoContenitori As Boolean
    Public Property Flag_RiepilogoImballi As Boolean
    Public Property Flag_TaraConfezioneDesc As Boolean
    Public Property Flag_TaraContenitoreDesc As Boolean
    Public Property Flag_TaraImballaggioDesc As Boolean
    Public Property Parametri_Extra As String
    Public Property Parametri_Extra_HT As Hashtable


    Public Sub New()
        'Se uso questo vuol dire che è un cliente che non ha ancora la gestione con tabella ==> inizializzo alcune proprietà con valori default
        'valorizzo esclusivamente le proprietà che mi serviranno sicuramente per il corpo del documento

        'Valori Default
        Flag_ConfezioniDesc = False
        Flag_ContenitoriDesc = True
        Flag_ImballaggiDesc = True

        Flag_CertificazioniDesc = False

        Flag_RiepilogoConfezioni = False
        Flag_RiepilogoContenitori = True
        Flag_RiepilogoImballi = True

        Flag_TaraConfezioneDesc = True
        Flag_TaraContenitoreDesc = True
        Flag_TaraImballaggioDesc = True
    End Sub

    Public Sub New(ByVal dr As DataRow)
        PivaSuperUser = CStr(dr.Item("PivaSuperUser"))
        Piva = CStr(dr.Item("Piva"))
        enum_CodificaStampe = CInt(dr.Item("enum_CodificaStampe"))
        ModuloGenerazione = CInt(dr.Item("Modulo_Generazione"))
        CodRisUm_PrimoCessionario = CStr(dr.Item("CodRisUm_PrimoCessionario"))
        PosizioneLogo = CStr(dr.Item("PosizioneLogo"))
        SalvataggioTemporaneo = CInt(dr.Item("SalvataggioTemporaneo"))
        SezioniDaNascondereString = CStr(dr.Item("SezioniDaNascondere"))
        SezioniDaNascondereArray = Nothing
        Flag_ConfezioniDesc = If(CInt(dr.Item("ConfezioniInDescrizione")) = 1, True, False)
        Flag_ContenitoriDesc = If(CInt(dr.Item("ContenitoriInDescrizione")) = 1, True, False)
        Flag_ImballaggiDesc = If(CInt(dr.Item("ImballaggiInDescrizione")) = 1, True, False)
        Flag_CertificazioniDesc = If(CInt(dr.Item("CertificazioneInDescrizione")) = 1, True, False)
        Flag_RiepilogoConfezioni = If(CInt(dr.Item("RiepilogoConfezioni")) = 1, True, False)
        Flag_RiepilogoContenitori = If(CInt(dr.Item("RiepilogoContenitori")) = 1, True, False)
        Flag_RiepilogoImballi = If(CInt(dr.Item("RiepilogoImballi")) = 1, True, False)
        Flag_TaraConfezioneDesc = If(CInt(dr.Item("TaraConfezioneInDescrizione")) = 1, True, False)
        Flag_TaraContenitoreDesc = If(CInt(dr.Item("TaraContenitoreInDescrizione")) = 1, True, False)
        Flag_TaraImballaggioDesc = If(CInt(dr.Item("TaraImballoInDescrizione")) = 1, True, False)
        NomeStampa = CStr(dr.Item("NomeStampa"))
        PathFileAspx = CStr(dr.Item("PathFileAspx"))
        NomeFileAspx = CStr(dr.Item("NomeFileAspx"))
        PathFileRpt = CStr(dr.Item("PathFileRpt"))
        NomeFileRpt = CStr(dr.Item("NomeFileRpt"))

        Parametri_Extra = CStr(dr.Item("Parametri_Extra"))

        RicavaParametriExtraHT()

        If Not String.IsNullOrEmpty(SezioniDaNascondereString) Then
            Dim arraySezioni As String() = SezioniDaNascondereString.Split(New Char() {ChrW(124)}, StringSplitOptions.RemoveEmptyEntries) '124 = "|"
            If arraySezioni.Length > 0 Then
                SezioniDaNascondereArray = arraySezioni
            End If
        End If
    End Sub

    Private Sub RicavaParametriExtraHT()

        If Parametri_Extra <> "" Then

            If InStr(Parametri_Extra, "|") > 0 Then

                If InStr(Parametri_Extra, "=") > 0 Then

                    Dim Parametri_Extra_Vet As String() = Parametri_Extra.Split("|")
                    Parametri_Extra_HT = New Hashtable

                    For Each parametro As String In Parametri_Extra_Vet
                        If parametro.Split("=").Count <> 2 Then
                            Throw New Exception("Errore nella lettura dei parametri extra: parametro.Split(=).Count <> 2: " & parametro)
                        End If
                        Dim key As String = parametro.Split("=")(0).Trim
                        Dim valore As String = parametro.Split("=")(1).Trim
                        Parametri_Extra_HT.Add(key, valore)
                    Next

                End If '=


                '--------------------
            Else
                'non c'è "|"

                If InStr(Parametri_Extra, "=") > 0 Then

                    Parametri_Extra_HT = New Hashtable

                    If Parametri_Extra.Split("=").Count <> 2 Then
                        Throw New Exception("Errore nella lettura dei parametri extra: parametro.Split(=).Count <> 2: " & Parametri_Extra)
                    End If
                    Dim key As String = Parametri_Extra.Split("=")(0).Trim
                    Dim valore As String = Parametri_Extra.Split("=")(1).Trim
                    Parametri_Extra_HT.Add(key, valore)

                End If '=

            End If ' |

        End If ' parametri non valorizzati

    End Sub

End Class

Public Class Configurazione_Stampe_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Legge Tabella Configurazione_Stampe
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="enum_CodificaStampe">Opzionale = 0</param>
    ''' <param name="Modulo_Generazione">Modulo Omni di generazione (-1 per non filtrare, 0 significativo si applica a tutti i moduli)</param>
    ''' <param name="Cod_RisUm_PrimoCessionario">Opzionale = 0</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>Modulo_Generazione = -1 => non filtrare, Modulo_Generazione = 0 => vale per tutti i moduli</remarks>
    Public Function Leggi(ByVal piva As String,
                          ByVal enum_CodificaStampe As Integer,
                          ByVal Modulo_Generazione As Integer,
                          ByVal Cod_RisUm_PrimoCessionario As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Configurazione_Stampe_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   enum_CodificaStampe = 0
        '   Modulo_Generazione = -1
        '   Cod_RisUm_PrimoCessionario = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        If piva = "" Then
            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
        End If

        Try

            strSql.Length = 0

            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  Configurazione_Stampe ")
            strSql.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'If Piva <> "" Then
            strSql.Append(" AND Piva ='" & Agro_SQL_SaveText(piva) & "' ")
            'End If

            If enum_CodificaStampe <> 0 Then
                strSql.Append(" AND enum_CodificaStampe = " & Agro_SQL_SaveNum(enum_CodificaStampe) & " ")
            End If

            If Modulo_Generazione <> -1 Then
                strSql.Append(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            'Documento generico, valido per tutti i codRisUm
            strSql.Append(" AND ( CodRisUm_PrimoCessionario = '' ")
            'aggiungo anche i documenti per questo specifico codrisum (poi tramite l'ordinamento darò la priorità a questo)
            If Cod_RisUm_PrimoCessionario <> 0 Then
                strSql.Append(" OR CodRisUm_PrimoCessionario LIKE '" & Agro_SQL_SaveText("%|" & Cod_RisUm_PrimoCessionario & "|%") & "' ) ")
            Else
                strSql.Append(" ) ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY enum_CodificaStampe ASC, Modulo_Generazione DESC, CodRisUm_PrimoCessionario DESC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###############################################################################
    Public Function URLrpt_from_enum_CodificaStampe(ByVal piva As String,
                                                    ByVal enum_CodificaStampe As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As String

        Dim url As String = ""
        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Configurazione_Stampe_R.URLrpt_from_enum_CodificaStampe()"

        Try

            dt = Leggi(piva,
                       enum_CodificaStampe,
                       -1, 0,
                       "", "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                url = dt.Rows(0).Item("PathFileRpt")

                If url.EndsWith("\") Then
                    url = Replace(url, "\", "/")
                End If

                If Not url.EndsWith("/") Then
                    url &= "/"
                End If

                url &= dt.Rows(0).Item("NomeFileRpt")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return url

    End Function

    '###############################################################################
    Public Function URLaspx_from_enum_CodificaStampe(ByVal piva As String,
                                                     ByVal enum_CodificaStampe As Integer,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As String

        Dim url As String = ""
        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Configurazione_Stampe_R.URLaspx_from_enum_CodificaStampe()"

        Try

            dt = Leggi(piva,
                       enum_CodificaStampe,
                       -1, 0,
                       "", "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                url = dt.Rows(0).Item("PathFileAspx")

                If url.EndsWith("\") Then
                    url = Replace(url, "\", "/")
                End If

                If Not url.EndsWith("/") Then
                    url &= "/"
                End If

                url &= dt.Rows(0).Item("NomeFileAspx")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return url

    End Function

End Class
