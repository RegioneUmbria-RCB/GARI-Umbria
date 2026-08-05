Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaBIZ
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.metaschema.avversita
Imports Newtonsoft.Json

Public Class Rilievi

    ''' <summary>
    ''' Popola il rilievo in base all'Operazione selezionata
    ''' </summary>
    ''' <param name="Lav_Cod"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="dpiCod"></param>
    ''' <param name="idRcdpi"></param>
    ''' <param name="dpiPubblicoPrivato"></param>
    ''' <param name="Personalizzata"></param>
    ''' <param name="LeggiPersonalizzataDaImpostazione"></param>
    ''' <param name="LeggiIndiciMaturitaLocali"></param>
    ''' Flag per la lettura degli indici maturità locali (tabella MisuraXIndiciMaturita_Anagrafiche)
    ''' <param name="objParametri_Super_Server"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Function Popola_Rilievo_In_base_Al_Lav_Cod(ByVal Lav_Cod As Integer, ByVal Veg_Cod As Integer, ByVal dpiCod As Integer,
                                                      ByVal idRcdpi As Integer, ByVal dpiPubblicoPrivato As Integer, ByVal eserciziCDC As List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC),
                                                      ByVal avversitaGruppo As AvversitaGruppo, ByVal Personalizzata As Boolean, ByVal LeggiPersonalizzataDaImpostazione As Boolean, ByVal LeggiIndiciMaturitaLocali As Boolean,
                                                      ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As String


        Dim Rilievi As String = String.Empty

        Select Case Lav_Cod
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                Rilievi = LetturaMisureXAvversitaStr(Veg_Cod, dpiCod,
                                                            idRcdpi, dpiPubblicoPrivato,
                                                            0, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                            objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

            Case LAVCOD_RILIEVO_INDICI_MATURITA
                Rilievi = LetturaIndiciMaturitaStr(Veg_Cod, 0, Personalizzata, LeggiPersonalizzataDaImpostazione, LeggiIndiciMaturitaLocali,
                                                          objParametri_Server, objParametri_Super_Server, objParametri_Utenti,
                                                          Lav_Cod)

            Case LAVCOD_DANNI_RACCOLTA
                Rilievi = LetturaDanniRaccoltaStr(Veg_Cod, Personalizzata, LeggiPersonalizzataDaImpostazione, objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

            Case LAVCOD_FASI_FENOLOGICHE
                Rilievi = LetturaFasiFenologicheStr(Veg_Cod, False, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                           objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                Rilievi = LetturaErbeInfestantiStr(objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                Rilievi = LetturaIndiciMaturitaStr(Veg_Cod, 1, Personalizzata, LeggiPersonalizzataDaImpostazione, LeggiIndiciMaturitaLocali,
                                                  objParametri_Server, objParametri_Super_Server, objParametri_Utenti,
                                                  Lav_Cod)

            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                Rilievi = LetturaAvversitaTrappoleStr(eserciziCDC, avversitaGruppo, objParametri_Server)

        End Select

        Return Rilievi
    End Function

    Public Function LetturaMisureXAvversitaWS(vegCod As Integer, dpiCod As Integer, idRcdpi As Integer,
                                            pubblicoPrivato As Integer, tipoTestata As Integer,
                                            Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean, ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of MisuraXAvv)

        If LeggiPersonalizzataDaImpostazione Then
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

            Dim impostazioni = objImpostazioni.LeggiImpostazioniScalare(
                objParametri_Server.UsernameOperazione, {enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA}, objParametri_Utenti
            )

            Personalizzata = impostazioni(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA) = "1"
        End If

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_input
        objParametriIngresso.Veg_Cod = vegCod
        objParametriIngresso.Dpi_Cod = dpiCod
        objParametriIngresso.Id_Rcdpi = idRcdpi
        objParametriIngresso.Dpi_Pubblico_Privato = pubblicoPrivato
        objParametriIngresso.TipoTestata = tipoTestata

        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)


        objParametriIngresso.Personalizzate = Personalizzata
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        objParametriIngresso.Url = url & "/MisureXAvversita"

        Dim objWS As New AgronicaCoreWebService.MisureXAvversita_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = objWS.MisureXAvversita(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If


        Return objParametriUscita.ListaMisureXAvversita

    End Function

    Private Function LetturaMisureXAvversitaStr(vegCod As Integer, dpiCod As Integer, idRcdpi As Integer, pubblicoPrivato As Integer,
                                                tipoTestata As Integer, Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim xLista As List(Of MisuraXAvv) = LetturaMisureXAvversita(vegCod, dpiCod, idRcdpi, pubblicoPrivato, tipoTestata, Personalizzata, LeggiPersonalizzataDaImpostazione, objParametri_Server,
                                            objParametri_Super_Server, objParametri_Utenti)

        Dim xListaStr As List(Of String) = (
            From ii In xLista
            Select "{ ""cod"": """ & ii.Av_Cod & "|" & ii.Av_Gru & "|" & ii.Udm_Cod & "|" & ii.TipoControllo_Cod & """, ""des"": """ & AgronicaCoreUtility.jSon.Escape(ii.Av_Des_Vol) & """, ""soglia"": """ & ii.Soglia & """, ""MxAV_Cod"": " & ii.Cod & "}").ToList
        'Select Case "{ ""cod"": """ & ii.Av_Cod & "|" & ii.Av_Gru & "|" & ii.Udm_Cod & """, ""des"": """ & IIf(ii.Soglia = 1, "* ", "").ToString & AgronicaCoreUtility.jSon.Escape(ii.Av_Des_Vol) & "|" & AgronicaCoreUtility.jSon.Escape(ii.Av_Gru_Des) & " (" & AgronicaCoreUtility.jSon.Escape(ii.Udm_Des) & ")""}").ToList


        Return "[" & String.Join(",", xListaStr) & "]"

    End Function

    Public Function LetturaMisureXAvversita(vegCod As Integer, dpiCod As Integer, idRcdpi As Integer, pubblicoPrivato As Integer,
                                                tipoTestata As Integer, Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of MisuraXAvv)

        Dim xLista As New List(Of MisuraXAvv)

        xLista = LetturaMisureXAvversitaWS(vegCod, dpiCod, idRcdpi, pubblicoPrivato, tipoTestata,
                                                                    Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                                    objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

        If Not IsNothing(xLista) AndAlso xLista.Count > 0 Then
            xLista.ForEach(Sub(m)
                               m.Av_Des_Vol = AgronicaCoreUtility.jSon.Escape(m.Av_Des_Vol) & "|" & AgronicaCoreUtility.jSon.Escape(m.Av_Gru_Des) & " (" & AgronicaCoreUtility.jSon.Escape(m.Udm_Des) & ")"
                           End Sub)
        End If



        Return xLista
    End Function

    Public Function LetturaIndiciMaturitaWS(vegCod As Integer, tipoTestata As Integer,
                                            Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                Optional lavCod As Integer = LAVCOD_RILIEVO_INDICI_MATURITA,
                                                Optional joinPersonalizzate As Boolean = True) As List(Of IndiciMaturita)
        Const GO_WS_SPEVEG_IAPI As String = "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali"

        If LeggiPersonalizzataDaImpostazione Then
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

            Select Case lavCod
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    Dim impostazioni = objImpostazioni.LeggiImpostazioniScalare(
                                            objParametri_Server.UsernameOperazione, {enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA}, objParametri_Utenti
                                        )

                    Personalizzata = impostazioni(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA) = "1"

                Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                    Dim impostazioni = objImpostazioni.LeggiImpostazioniScalare(
                                            objParametri_Server.UsernameOperazione, {enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA}, objParametri_Utenti
                                        )

                    Personalizzata = impostazioni(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA) = "1"

            End Select
        End If



        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.IndiciMaturita_input
        objParametriIngresso.veg_cod = vegCod
        objParametriIngresso.lav_cod = lavCod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim url As String = leggiPath.Leggi_Valore(0, GO_WS_SPEVEG_IAPI, "", "", objParametri_Server)
        If url = "" Then
            url = leggiPath.Leggi_Valore(0, GO_WS_SPEVEG_IAPI, "", "", objParametri_Super_Server)
        End If

        If url = "" Then
            Throw New Exception($"Url non recuperato (Chiave = {GO_WS_SPEVEG_IAPI}).")
        End If

        objParametriIngresso.Personalizzate = Personalizzata
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        objParametriIngresso.Url = url & "/IndiciMaturita"
        objParametriIngresso.tipoTestata = tipoTestata
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod
        objParametriIngresso.joinPersonalizzate = joinPersonalizzate

        Dim objWS As New AgronicaCoreWebService.IndiciMaturita_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = objWS.IndiciMaturita(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita.ListaIndiciMaturita

    End Function

    Public Function LetturaIndiciMaturitaStr(vegCod As Integer, tipoTestata As Integer,
                                             Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                             LeggiIndiciMaturitaLocali As Boolean,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                  Optional lavCod As Integer = LAVCOD_RILIEVO_INDICI_MATURITA) As String

        Dim xListaStr As New List(Of String)

        Dim xLista As List(Of IndiciMaturita) = LetturaIndiciMaturita(vegCod, tipoTestata, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                                      LeggiIndiciMaturitaLocali, objParametri_Server, objParametri_Super_Server, objParametri_Utenti, lavCod)

        xListaStr = (
            From ii In xLista
            Select "{ ""cod"": """ & ii.ind_mat_cod & "|" & ii.udm_cod & """, ""des"": """ & AgronicaCoreUtility.jSon.Escape(ii.ind_mat_des) & """}").ToList

        Return "[" & String.Join(",", xListaStr) & "]"

    End Function

    Public Function LetturaIndiciMaturita(vegCod As Integer, tipoTestata As Integer,
                                            Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            LeggiIndiciMaturitaLocali As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                            Optional lavCod As Integer = LAVCOD_RILIEVO_INDICI_MATURITA) As List(Of IndiciMaturita)

        Dim xLista As New List(Of IndiciMaturita)

        Dim TempXLista As List(Of IndiciMaturita) = LetturaIndiciMaturitaWS(vegCod, tipoTestata, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                              objParametri_Server, objParametri_Super_Server, objParametri_Utenti, lavCod)

        If Not IsNothing(TempXLista) AndAlso TempXLista.Count > 0 Then

            If LeggiIndiciMaturitaLocali Then

                Dim objMetaSchemaDAL As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R
                Dim DT As New DataTable
                If vegCod >= 0 Then
                    DT = objMetaSchemaDAL.Leggi(0, 0, vegCod,
                                            "", "", objParametri_Server)
                Else
                    DT = objMetaSchemaDAL.Leggi_SenzaSpecie(0, 0, objParametri_Server)
                End If

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                    For Each Im As IndiciMaturita In TempXLista
                        Dim Dr As DataRow() = DT.Select("ind_mat_cod = " & Im.ind_mat_cod & "")

                        If Not IsNothing(Dr) AndAlso Dr.Count > 0 Then
                            Im.ind_mat_des = AgronicaCoreUtility.jSon.Escape(Im.ind_mat_des)
                        Else
                            Im.ind_mat_des = AgronicaCoreUtility.jSon.Escape(Im.ind_mat_des) & " (" & AgronicaCoreUtility.jSon.Escape(Im.udm_des) & ")"
                        End If

                        xLista.Add(Im)
                    Next

                Else

                    xLista = TempXLista

                    xLista.ForEach(Sub(m)
                                       m.ind_mat_des = AgronicaCoreUtility.jSon.Escape(m.ind_mat_des) & " (" & AgronicaCoreUtility.jSon.Escape(m.udm_des) & ")"
                                   End Sub)

                End If

            Else

                xLista = TempXLista

                xLista.ForEach(Sub(m)
                                   m.ind_mat_des = AgronicaCoreUtility.jSon.Escape(m.ind_mat_des) & " (" & AgronicaCoreUtility.jSon.Escape(m.udm_des) & ")"
                               End Sub)

            End If
        End If



        Return xLista

    End Function


    Public Function LetturaDanniRaccoltaWS(vegCod As Integer,
                                         Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of MisuraXDR)
        Const GO_WS_SPEVEG_IAPI As String = "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali"

        If LeggiPersonalizzataDaImpostazione Then
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

            Dim impostazioni = objImpostazioni.LeggiImpostazioniScalare(
                    objParametri_Server.UsernameOperazione, {enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA}, objParametri_Utenti
                )

            Personalizzata = impostazioni(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA) = "1"
        End If

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_input
        objParametriIngresso.Veg_Cod = vegCod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R


        Dim url As String = leggiPath.Leggi_Valore(0, GO_WS_SPEVEG_IAPI, "", "", objParametri_Server)
        If url = "" Then
            url = leggiPath.Leggi_Valore(0, GO_WS_SPEVEG_IAPI, "", "", objParametri_Super_Server)
        End If
        If url = "" Then Throw New Exception($"Url non recuperato (Chiave = {GO_WS_SPEVEG_IAPI}).")

        objParametriIngresso.Personalizzate = Personalizzata
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        objParametriIngresso.Url = url & "/MisureXDanniRaccolta"

        Dim objWS As New AgronicaCoreWebService.MisureXDanniRaccolta_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = objWS.MisureXDanniRaccolta(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita.ListaMisureXDanniRaccolta

    End Function


    Public Function LetturaDanniRaccoltaStr(vegCod As Integer,
                                            Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim xLista As List(Of MisuraXDR) = LetturaDanniRaccolta(vegCod, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                                objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

        Dim xListaStr As List(Of String) = (
            From ii In xLista
            Select "{ ""cod"": """ & ii.Dr_Cod & "|" & ii.Udm_Cod & """, ""des"": """ & AgronicaCoreUtility.jSon.Escape(ii.Dr_Des) & """}").ToList

        Return "[" & String.Join(",", xListaStr) & "]"

    End Function

    Public Function LetturaDanniRaccolta(vegCod As Integer,
                                            Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of MisuraXDR)

        Dim xLista As New List(Of MisuraXDR)

        xLista = LetturaDanniRaccoltaWS(vegCod, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                        objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

        If Not IsNothing(xLista) AndAlso xLista.Count > 0 Then
            xLista.ForEach(Sub(m)
                               m.Dr_Des = AgronicaCoreUtility.jSon.Escape(m.Dr_Des) & " (" & AgronicaCoreUtility.jSon.Escape(m.Udm_Des) & ")"
                           End Sub)
        End If

        Return xLista

    End Function

    Public Function LetturaFasiFenologiche(vegCod As Integer,
                                          fasiOld As Boolean,
                                           Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of FaseFenologica)

        Dim xLista As New List(Of FaseFenologica)

        xLista = LetturaFasiFenologicheWS(vegCod, fasiOld, Personalizzata, LeggiPersonalizzataDaImpostazione, objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

        Return xLista
    End Function

    Public Function LetturaFasiFenologicheWS(vegCod As Integer,
                                            fasiOld As Boolean,
                                           Personalizzata As Boolean, LeggiPersonalizzataDaImpostazione As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of FaseFenologica)

        If LeggiPersonalizzataDaImpostazione Then
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

            Dim impostazioni As Dictionary(Of Integer, String) = objImpostazioni.LeggiImpostazioniScalare(
                    objParametri_Server.UsernameOperazione, {enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE}, objParametri_Utenti
                )
            If impostazioni IsNot Nothing AndAlso impostazioni.ContainsKey(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE) Then
                Personalizzata = impostazioni(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE) = "1"
            End If
        End If

        'lettura fasi fenologiche (da web service sia nuove fasi bbch sia vecchie fasi)
        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        objParametriIngresso.Veg_Cod = CInt(vegCod)

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

        objParametriIngresso.Personalizzate = Personalizzata
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser

        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        If fasiOld = False Then
            objParametriIngresso.Url = url & "/FasiFenologiche"
            objParametriUscita = objFasi_WS.FasiFenologiche(objParametriIngresso)
        Else
            objParametriIngresso.Url = url & "/FasiFenologiche_OLD"
            objParametriIngresso.strOrdinamento = "Progressivo ASC"
            objParametriUscita = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
        End If

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita.ListaFasiFenologiche

    End Function

    Public Function LetturaFasiFenologicheStr(vegCod As Integer,
                                              fasiOld As Boolean,
                                              Personalizzata As Boolean,
                                              LeggiPersonalizzataDaImpostazione As Boolean,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri
                                              ) As String

        Dim xLista As List(Of FaseFenologica) = LetturaFasiFenologicheWS(vegCod, fasiOld, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                                       objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

        Dim xListaStr As List(Of String) = (
            From ii In xLista
            Select "{ ""cod"": """ & ii.Cod_SS & "|" & ii.ID_BBCH & "|" & ii.FF_Cod & """, ""des"": """ & AgronicaCoreUtility.jSon.Escape(ii.Descrizione) & """, ""fioritura"": """ & ii.Fioritura & """, ""stadio"": """ & ii.Stadio & """}").ToList

        Return "[" & String.Join(",", xListaStr) & "]"

    End Function

    Public Function LetturaErbeInfestantiWS(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri
                                            ) As (List(Of InfestantiAttive), List(Of GruppoInfestantiAttive))
        Const GO_WS_SPEVEG_IAPI As String = "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali"

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Infestanti_input
        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim url As String = leggiPath.Leggi_Valore(0, GO_WS_SPEVEG_IAPI, "", "", objParametri_Server)
        If url = "" Then
            url = leggiPath.Leggi_Valore(0, GO_WS_SPEVEG_IAPI, "", "", objParametri_Super_Server)
        End If
        If url = "" Then Throw New Exception($"Url non recuperato (Chiave = {GO_WS_SPEVEG_IAPI}).")

        objParametriIngresso.Url = url & "/ErbeInfestanti"
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim objWS As New AgronicaCoreWebService.Infestanti_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Infestanti_output = objWS.ErbeInfestanti(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return (objParametriUscita.InfestantiAttive, objParametriUscita.GruppoInfestantiAttive)

    End Function

    Public Function LetturaErbeInfestantiStr(ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim xLista As (List(Of InfestantiAttive), List(Of GruppoInfestantiAttive)) = LetturaErbeInfestanti(objParametri_Server,
                                                                                                           objParametri_Super_Server,
                                                                                                           objParametri_Utenti)
        Dim weedsGroups As New List(Of String)
        Dim activeWeeds As New List(Of String)

        If xLista.Item2 IsNot Nothing Then
            weedsGroups = xLista.Item2.AsEnumerable().AsParallel.
                Select(Function(item) New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(
                item.AV_GRU,
                item.Av_Gru_Des)
                ).OrderBy(Function(obj) obj.descrizione).
                Select(Of String)(Function(row) "{ ""cod"": """ & row.codice &
                """, ""des"": """ & row.descrizione &
                """, ""av_cod"": ""0""}"
                ).ToList
        End If

        If xLista.Item1 IsNot Nothing Then
            activeWeeds = xLista.Item1.AsEnumerable().AsParallel.
            Select(Of String)(Function(item) "{ ""cod"": """ & item.AV_GRU &
                """, ""des"": """ & item.Av_Des_Vol &
                """, ""av_cod"": """ & item.AV_COD & """}"
            ).ToList
        End If

        weedsGroups.AddRange(activeWeeds)
        Return "[" & String.Join(",", weedsGroups) & "]"
    End Function

    Public Function LetturaErbeInfestanti(ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri) As (List(Of InfestantiAttive), List(Of GruppoInfestantiAttive))

        Dim xLista As (List(Of InfestantiAttive), List(Of GruppoInfestantiAttive)) = LetturaErbeInfestantiWS(objParametri_Server,
                                                                                                             objParametri_Super_Server,
                                                                                                             objParametri_Utenti)

        If xLista.Item2 IsNot Nothing Then
            xLista.Item2.ForEach(Sub(m)
                                     m.Av_Gru_Des = AgronicaCoreUtility.jSon.Escape(m.Av_Gru_Des).Replace("- ", "")
                                 End Sub)
        End If

        If xLista.Item1 IsNot Nothing Then
            xLista.Item1.ForEach(Sub(m)
                                     m.Av_Des_Vol = AgronicaCoreUtility.jSon.Escape(m.Av_Des_Vol)
                                 End Sub)
        End If

        Return xLista
    End Function

    Public Function LetturaAvversitaTrappoleStr(ByVal eserciziCDC As List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC),
                                                ByVal avversitaGruppo As AvversitaGruppo,
                                                ByVal objParametri_Server As AgronicaCoreParametri) As String

        Dim avversitaTrappoleList As List(Of AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole) = LetturaAvversitaTrappole(eserciziCDC, avversitaGruppo, objParametri_Server)

        Return JsonConvert.SerializeObject(avversitaTrappoleList)

    End Function


    Public Function LetturaAvversitaTrappole(ByVal eserciziCDC As List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC),
                                             ByVal avversitaGruppo As AvversitaGruppo,
                                             ByVal objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole)

        Dim avversitaTrappoleList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole)

        Dim idTestataTemp As Integer = 0

        Dim Filtro_Av_Cod As Integer = 0

        If Not IsNothing(eserciziCDC) Then

            Dim listImpianti As New List(Of AgronicaCoreVarieDAL.TmpFiltroImpiantiObject)

            For Each esercizioCDC As AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC In eserciziCDC

                listImpianti.Add(New AgronicaCoreVarieDAL.TmpFiltroImpiantiObject With {
                            .Piva = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                            .Sa_Cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString(),
                            .Appezza = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice.ToString(),
                            .IdReg = esercizioCDC.esercizio.impiantoPK.codice.ToString()
                        })
            Next

            If Not IsNothing(listImpianti) AndAlso listImpianti.Count > 0 Then

                Dim obj__tmp_filtro_impianti As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_W

                obj__tmp_filtro_impianti.Popola__tmp_FiltroImpianti_Da_Lista(listImpianti, idTestataTemp, objParametri_Server)

            End If

        End If

        If Not IsNothing(avversitaGruppo) Then
            Filtro_Av_Cod = avversitaGruppo.codice
        End If

        Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R

        Dim Dt_Trappole_Registrate As DataTable = objOperazioni.Leggi_Numero_Trappole_Registrate(idTestataTemp, Filtro_Av_Cod, "", "", objParametri_Server)

        If Not IsNothing(Dt_Trappole_Registrate) AndAlso Dt_Trappole_Registrate.Rows.Count > 0 Then
            Dim distinctAvversita As List(Of Integer) = Dt_Trappole_Registrate.AsEnumerable() _
                                                       .Select(Function(row) row.Field(Of Integer)("Av_Cod")) _
                                                       .Distinct() _
                                                       .ToList()

            If Not IsNothing(distinctAvversita) AndAlso distinctAvversita.Count > 0 Then

                Dim xFiltroAggiuntivo As String = ""

                Dim distinctUdMCod As List(Of Integer) = Dt_Trappole_Registrate.AsEnumerable() _
                                                       .Select(Function(row) row.Field(Of Integer)("Udm_Cod")) _
                                                       .Distinct() _
                                                       .ToList()

                If Not IsNothing(distinctUdMCod) Then
                    If distinctUdMCod.FindIndex(Function(x) x = enum_UnitaMisura.Numero_Adulti_Individui_Trappola) = -1 Then
                        distinctUdMCod.Add(enum_UnitaMisura.Numero_Adulti_Individui_Trappola)
                    End If

                    If distinctUdMCod.FindIndex(Function(x) x = enum_UnitaMisura.Presenza) = -1 Then
                        distinctUdMCod.Add(enum_UnitaMisura.Presenza)
                    End If

                    xFiltroAggiuntivo = "Udm_Cod IN (" & String.Join(",", distinctUdMCod) & ")"

                End If

                Dim objSTD_UnitaMisura As New STD_UnitaDiMisura

                Dim UdmList As List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura) = objSTD_UnitaMisura.LeggiUnitaDiMisuraConTipoControllo(objParametri_Server, xFiltroAggiuntivo)

                If Not IsNothing(UdmList) AndAlso UdmList.Count > 0 Then

                    For Each Av_Cod As Integer In distinctAvversita

                        Dim Dr_Temp As DataRow() = Dt_Trappole_Registrate.Select("Av_Cod = " & Av_Cod)

                        If Not IsNothing(Dr_Temp) AndAlso Dr_Temp.Count > 0 Then

                            Dim Dt_Temp As DataTable = Dr_Temp.CopyToDataTable()

                            If Not IsNothing(Dt_Temp) AndAlso Dt_Temp.Rows.Count > 0 Then

                                Dim query = From row In Dt_Temp.AsEnumerable()
                                            Group row By
                                                Piva = row.Field(Of String)("Piva"),
                                                Sa_Cod = row.Field(Of Integer)("Sa_Cod"),
                                                Appezza = row.Field(Of Integer)("Appezza"),
                                                Id_Reg = row.Field(Of Integer)("id_reg"),
                                                Fr_Cod = row.Field(Of Integer)("Fr_Cod"),
                                                Udm_Cod = row.Field(Of Integer)("Udm_Cod")
                                            Into Group
                                            Select Piva,
                                                Sa_Cod,
                                                Appezza,
                                                Id_Reg,
                                                Fr_Cod,
                                                Udm_Cod,
                                                Dose_Totale = Group.Sum(Function(r) CDec(r("Dose")))

                                If Not IsNothing(query) AndAlso query.Count > 0 Then

                                    Dim avversitaTrappoleNumero As AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole = Nothing

                                    For Each item In query

                                        Dim Dr_Descrizione As DataRow() = Dt_Trappole_Registrate.Select("Av_Cod = " & Av_Cod & " And Fr_Cod = " & item.Fr_Cod)

                                        If Not IsNothing(Dr_Descrizione) AndAlso Dr_Descrizione.Count > 0 Then

                                            Dim Fr_Des As String = Dr_Descrizione(0)("Fr_Des")

                                            Dim Av_Des_Vol As String = Dr_Descrizione(0)("Av_Des_Vol")

                                            Dim Abbreviazione As String = Dr_Descrizione(0)("Abbreviazione")

                                            If IsNothing(avversitaTrappoleNumero) Then

                                                Dim avversita = New AgronicaCoreModelsSTD.metaschema.avversita.Avversita(Av_Cod) With {
                                                        .descrizione = Av_Des_Vol,
                                                        .abbreviazione = Abbreviazione
                                                    }
                                                avversitaTrappoleNumero = New AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole(0, "") With {
                                                        .avversitaGruppo = avversita,
                                                        .utilizzi = New List(Of AgronicaCoreModelsSTD.attivita.dettagli.UtilizzoAvversitaTrappole),
                                                        .unitaDiMisura = UdmList.Find(Function(udm) udm.codice = enum_UnitaMisura.Numero_Adulti_Individui_Trappola)
                                                    }
                                            End If

                                            avversitaTrappoleNumero.utilizzi.Add(New AgronicaCoreModelsSTD.attivita.dettagli.UtilizzoAvversitaTrappole With {
                                                .impianto = New AgronicaCoreModelsSTD.anagrafiche.Impianto With {
                                                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK With {
                                                        .codice = item.Id_Reg,
                                                        .appezzamentoPK = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK With {
                                                            .codice = item.Appezza,
                                                            .centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK With {
                                                                .codice = item.Sa_Cod,
                                                                .partitaIva = item.Piva
                                                            }
                                                        }
                                                    }
                                                },
                                                .risorsaProdotto = New AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto With {
                                                    .prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto With {
                                                        .codice = item.Fr_Cod,
                                                        .descrizione = Fr_Des,
                                                        .elemCod = CostantiPersonalizzate.FORMULATI
                                                    },
                                                    .unitaDiMisuraIndicata = UdmList.Find(Function(udm) udm.codice = item.Udm_Cod),
                                                    .quantitaTotaleReale = item.Dose_Totale
                                                }
                                            })

                                            ValorizeCodiceDescrizioneAvversitaTrappole(avversitaTrappoleNumero)

                                        End If


                                    Next

                                    avversitaTrappoleList.Add(avversitaTrappoleNumero)

                                    Dim avversitaTrappolePresenza As AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole = avversitaTrappoleNumero.Clona()

                                    avversitaTrappolePresenza.unitaDiMisura = UdmList.Find(Function(udm) udm.codice = enum_UnitaMisura.Presenza)

                                    ValorizeCodiceDescrizioneAvversitaTrappole(avversitaTrappolePresenza)

                                    avversitaTrappoleList.Add(avversitaTrappolePresenza)

                                End If
                            End If
                        End If
                    Next
                End If

            End If

        End If

        Return avversitaTrappoleList
    End Function

    Private Sub ValorizeCodiceDescrizioneAvversitaTrappole(ByRef avversitaTrappole As AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole)

        If Not IsNothing(avversitaTrappole) Then

            Dim Av_Cod As Integer = 0

            Dim Av_Des_Vol As String = ""

            Dim Abbreviazione As String = ""

            Dim Udm_Cod As Integer = 0

            Dim Udm_Des As String = ""

            Dim TipoControllo_Cod As Integer = 0

            If Not IsNothing(avversitaTrappole.avversitaGruppo) Then

                If avversitaTrappole.avversitaGruppo.classType = costanti.ClassType.Avversita Then

                    Dim avversita = CType(avversitaTrappole.avversitaGruppo, AgronicaCoreModelsSTD.metaschema.avversita.Avversita)

                    Av_Cod = avversita.codice

                    Av_Des_Vol = avversita.descrizione

                    Abbreviazione = avversita.abbreviazione

                End If

            End If

            If Not IsNothing(avversitaTrappole.unitaDiMisura) Then

                Udm_Cod = avversitaTrappole.unitaDiMisura.codice

                Udm_Des = avversitaTrappole.unitaDiMisura.descrizione

                If Not IsNothing(avversitaTrappole.unitaDiMisura.tipoControllo) Then

                    TipoControllo_Cod = avversitaTrappole.unitaDiMisura.tipoControllo.codice
                End If

            End If

            avversitaTrappole.codice = Av_Cod

            avversitaTrappole.descrizione = AgronicaCoreUtility.jSon.Escape(Av_Des_Vol) & "|" & AgronicaCoreUtility.jSon.Escape(Abbreviazione) & " (" & AgronicaCoreUtility.jSon.Escape(Udm_Des) & ")"

        End If


    End Sub


End Class
