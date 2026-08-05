Imports System.Xml
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class meteo

    Public Function Dati_Completi_R(
                                    ByVal XmlParametri As String,
                                    ByRef ErrCod As Integer,
                                    ByRef ErrMsg As String,
                                    ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                    As String



        Dim Flag_Dettagli As String = ""
        Dim Flag_Aggrega As String = ""
        Dim Flag_Spazio As String = ""
        Dim Soglia As Single = 0
        Dim OraRilievo As Integer = 23
        Dim ID_Quadrante As Integer = 0
        Dim ID_Stazione As Integer = 0
        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim Data1 As Date = #1/1/1900#
        Dim Data2 As Date = #12/31/2100#
        Dim flag_tabella_leggi As String = "H"
        Dim nomeStazione As String = ""
        Dim XmlRisultato As String = ""

        Dim xMeteoDAL As New AgronicaCoreMeteoDAL.Meteo_R
        Dim xRisultato As New xRisultato

        '-----------------------------------------------------------------------
        '--- Recupero i parametri
        '-----------------------------------------------------------------------

        LeggiParametri(XmlParametri, Flag_Dettagli, Flag_Aggrega, Flag_Spazio, Soglia, OraRilievo, ID_Quadrante, ID_Stazione, X, Y, Data1, Data2, flag_tabella_leggi, nomeStazione)

        '-----------------------------------------------------------------------
        '--- Se ho ricevuto le coordinate ==> calcolo il quadrante
        '-----------------------------------------------------------------------

        If Flag_Spazio = "C" Then

            Dim DTQuadrante As New DataTable

            DTQuadrante = xMeteoDAL.Dati_Quadranti_R(X, Y, objParametri)

            If DTQuadrante.Rows.Count = 0 Then
                ErrCod = 736254
                ErrMsg = "Nessun quadrante corrisponde alle coordinate indicate"
                Return ""
                Exit Function
            Else
                ID_Quadrante = CInt(DTQuadrante.Rows(0).Item("ID_Quadrante"))
            End If

        End If

        '-----------------------------------------------------------------------
        '--- Recupero l'identificatore corretto
        '-----------------------------------------------------------------------

        Dim ID_QuadranteStazione As Integer
        Dim Flag_Quadrante_Stazione As String

        Select Case Flag_Spazio
            Case "Q", "C"
                ID_QuadranteStazione = ID_Quadrante
                Flag_Quadrante_Stazione = "Q"
            Case "S"
                ID_QuadranteStazione = ID_Stazione
                Flag_Quadrante_Stazione = "S"
            Case Else
                ErrCod = 923773
                ErrMsg = "Parametro non corretto"
                Return ""
                Exit Function
        End Select

        '-----------------------------------------------------------------------
        '--- Recupero le informazioni
        '-----------------------------------------------------------------------

        Dim DTCompleto As New DataTable


        DTCompleto = xMeteoDAL.Dati_Completi(
            ID_QuadranteStazione,
            OraRilievo,
            Data1,
            Data2,
            Flag_Quadrante_Stazione,
            flag_tabella_leggi,
            nomeStazione,
            objParametri
        )





        '-----------------------------------------------------------------------
        '--- Genero la stringa XML di risposta
        '-----------------------------------------------------------------------

        XmlRisultato = xRisultato.Dati_Completi(
            DTCompleto,
            Flag_Dettagli,
            Flag_Aggrega,
            Flag_Spazio,
            ID_Quadrante,
            Soglia,
            flag_tabella_leggi
        )

        Return XmlRisultato

    End Function



    Public Sub LeggiParametri(XmlParametri As String, ByRef Flag_Dettagli As String, ByRef Flag_Aggrega As String, ByRef Flag_Spazio As String, ByRef Soglia As Single, ByRef OraRilievo As Integer, ByRef ID_Quadrante As Integer, ByRef ID_Stazione As Integer, ByRef X As Integer, ByRef Y As Integer, ByRef Data1 As Date, ByRef Data2 As Date, ByRef Flag_Tabella_Leggi As String, ByRef NomeStazione As String)
        Dim XmlDoc As New XmlDocument
        Dim XmlNodo As XmlElement

        XmlDoc.LoadXml(XmlParametri)

        XmlNodo = XmlDoc.SelectSingleNode("PARAMETRI")

        Flag_Dettagli = CStr(XmlNodo.GetAttribute("flag_dettagli"))
        Flag_Aggrega = CStr(XmlNodo.GetAttribute("flag_aggrega"))
        Flag_Spazio = CStr(XmlNodo.GetAttribute("flag_spazio"))

        Soglia = CSng(XmlNodo.GetAttribute("soglia"))
        OraRilievo = CInt(XmlNodo.GetAttribute("orarilievo"))

        Select Case Flag_Spazio
            Case "Q"
                ID_Quadrante = CInt(XmlNodo.GetAttribute("idq"))
                ID_Stazione = 0
                X = 0
                Y = 0
            Case "S"
                ID_Quadrante = 0
                ID_Stazione = CInt(XmlNodo.GetAttribute("ids"))
                X = 0
                Y = 0
            Case "C"
                ID_Quadrante = 0
                ID_Stazione = 0
                X = CInt(XmlNodo.GetAttribute("idx"))
                Y = CInt(XmlNodo.GetAttribute("idy"))
        End Select

        Data1 = CDate(XmlNodo.GetAttribute("data1"))
        Data2 = CDate(XmlNodo.GetAttribute("data2"))

        If XmlNodo.HasAttribute("flag_tabella_leggi") Then
            Flag_Tabella_Leggi = XmlNodo.GetAttribute("flag_tabella_leggi")
        End If

        If XmlNodo.HasAttribute("nst") Then
            NomeStazione = XmlNodo.GetAttribute("nst")
        End If


        XmlDoc = Nothing
    End Sub

    Public Function Dati_Precipitazioni_R(
                                    ByVal XmlParametri As String,
                                    ByRef ErrCod As Integer,
                                    ByRef ErrMsg As String,
                                    ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                    As String

        Dim Flag_Dettagli As String = ""
        Dim Flag_Aggrega As String = ""
        Dim Flag_Spazio As String = ""
        Dim Soglia As Single = 0
        Dim OraRilievo As Integer = 23
        Dim ID_Quadrante As Integer = 0
        Dim ID_Stazione As Integer = 0
        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim Data1 As Date = #1/1/1900#
        Dim Data2 As Date = #12/31/2100#
        Dim flag_tabella_leggi As String = "H"

        Dim nomeStazione As String = ""

        Dim XmlRisultato As String = ""

        Dim xMeteoDal As New AgronicaCoreMeteoDAL.Meteo_R

        '-----------------------------------------------------------------------
        '--- Recupero i parametri
        '-----------------------------------------------------------------------

        LeggiParametri(XmlParametri, Flag_Dettagli, Flag_Aggrega, Flag_Spazio, Soglia, OraRilievo, ID_Quadrante, ID_Stazione, X, Y, Data1, Data2, flag_tabella_leggi, nomeStazione)

        '-----------------------------------------------------------------------
        '--- Se ho ricevuto le coordinate ==> calcolo il quadrante
        '-----------------------------------------------------------------------

        If Flag_Spazio = "C" Then

            Dim DTQuadrante As New DataTable

            DTQuadrante = xMeteoDal.Dati_Quadranti_R(X, Y, objparametri)

            If DTQuadrante.Rows.Count = 0 Then
                ErrCod = 736254
                ErrMsg = "Nessun quadrante corrisponde alle coordinate indicate"
                Return ""
                Exit Function
            Else
                ID_Quadrante = CInt(DTQuadrante.Rows(0).Item("ID_Quadrante"))
            End If

        End If

        '-----------------------------------------------------------------------
        '--- Recupero l'identificatore corretto
        '-----------------------------------------------------------------------

        Dim ID_QuadranteStazione As Integer
        Dim Flag_Quadrante_Stazione As String

        Select Case Flag_Spazio
            Case "Q", "C"
                ID_QuadranteStazione = ID_Quadrante
                Flag_Quadrante_Stazione = "Q"
            Case "S"
                ID_QuadranteStazione = ID_Stazione
                Flag_Quadrante_Stazione = "S"
            Case Else
                ErrCod = 923773
                ErrMsg = "Parametro non corretto"
                Return ""
                Exit Function
        End Select

        '-----------------------------------------------------------------------
        '--- Recupero le informazioni
        '-----------------------------------------------------------------------

        Dim DTPrecipitazioni As New DataTable

        Select Case Flag_Aggrega

            Case "G", "I"
                DTPrecipitazioni = xMeteoDal.Dati_Precipitazioni_Sum24H_R(
                                                                    ID_QuadranteStazione,
                                                                    OraRilievo,
                                                                    Data1,
                                                                    Data2,
                                                                    Flag_Quadrante_Stazione,
                                                                    objparametri,
                                                                    "")

            Case "H"
                DTPrecipitazioni = xMeteoDal.Dati_Precipitazioni_Orari_R(
                                                                    ID_QuadranteStazione,
                                                                    OraRilievo,
                                                                    Data1,
                                                                    Data2,
                                                                    Soglia,
                                                                    Flag_Quadrante_Stazione,
                                                                    objparametri,
                                                                    "")

            Case Else
                ErrCod = 354955
                ErrMsg = "Parametro non corretto"
                Return ""

        End Select

        '-----------------------------------------------------------------------
        '--- Genero la stringa XML di risposta
        '-----------------------------------------------------------------------

        Dim xRisultato As New xRisultato
        XmlRisultato = xRisultato.Dati_Precipitazioni(
                                                DTPrecipitazioni,
                                                Flag_Dettagli,
                                                Flag_Aggrega,
                                                Flag_Spazio,
                                                ID_Quadrante,
                                                Soglia)

        Return XmlRisultato

    End Function



    Public Function Agronica_Dati_Precipitazioni(
                                 ByVal XmlParametri As String,
                                 ByRef ErrCod As Integer,
                                 ByRef ErrMsg As String,
                                 ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                 As String

        Dim Flag_Dettagli As String = ""
        Dim Flag_Aggrega As String = ""
        Dim Flag_Spazio As String = ""
        Dim Soglia As Single = 0
        Dim OraRilievo As Integer = 23

        Dim Data1 As Date = #1/1/1900#
        Dim Data2 As Date = #12/31/2100#

        Dim XmlRisultato As String = ""

        '-----------------------------------------------------------------------
        '--- Recupero i parametri
        '-----------------------------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlNodo As XmlElement

        XmlDoc.LoadXml(XmlParametri)

        XmlNodo = XmlDoc.SelectSingleNode("PARAMETRI")

        Flag_Dettagli = CStr(XmlNodo.GetAttribute("flag_dettagli"))
        Flag_Aggrega = CStr(XmlNodo.GetAttribute("flag_aggrega"))
        Flag_Spazio = CStr(XmlNodo.GetAttribute("flag_spazio"))

        Soglia = CSng(XmlNodo.GetAttribute("soglia"))
        OraRilievo = CInt(XmlNodo.GetAttribute("orarilievo"))

        Dim f_station_name As String
        Select Case Flag_Spazio

            Case "M"
                f_station_name = CStr(XmlNodo.GetAttribute("nst"))

            Case Else
                ErrCod = 923773
                ErrMsg = "Parametro non corretto Flag non valido <> M"
                Return ""
                Exit Function
        End Select

        Data1 = CDate(XmlNodo.GetAttribute("data1"))
        Data2 = CDate(XmlNodo.GetAttribute("data2"))

        XmlDoc = Nothing



        '-----------------------------------------------------------------------
        '--- Recupero le informazioni
        '-----------------------------------------------------------------------

        Dim DTPrecipitazioni As New DataTable
        Dim Metor_R_W As New AgronicaCoreMeteoDAL.Meteo_R

        Select Case Flag_Aggrega

            Case "G", "I"
                DTPrecipitazioni = Metor_R_W.Agronica_Dati_Rilevati_LeggiPioggeGiornaliere(
                                                                    f_station_name,
                                                                    Data1,
                                                                    Data2,
                                                                    objParametri)

            Case "H"
                ErrCod = 354955
                ErrMsg = "Parametro non corretto"
                Return ""

            Case Else
                ErrCod = 354955
                ErrMsg = "Parametro non corretto"
                Return ""

        End Select

        '-----------------------------------------------------------------------
        '--- Genero la stringa XML di risposta
        '-----------------------------------------------------------------------

        Dim xRisultato As New xRisultato
        XmlRisultato = xRisultato.Dati_Precipitazioni(
                                                DTPrecipitazioni,
                                                Flag_Dettagli,
                                                Flag_Aggrega,
                                                Flag_Spazio,
                                                -1,
                                                Soglia)

        Return XmlRisultato

    End Function


    Public Function Dati_ElencoStazioniDistanza(
                             ByVal XmlParametri As String,
                             ByVal Utente_Username_client_GIAS As String,
                             ByRef ErrCod As Integer,
                             ByRef ErrMsg As String,
                             ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim longitudine As String = ""
        Dim latitudine As String = ""


        Dim XmlRisultato As String = ""

        '-----------------------------------------------------------------------
        '--- Recupero i parametri
        '-----------------------------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlNodo As XmlElement

        XmlDoc.LoadXml(XmlParametri)

        XmlNodo = XmlDoc.SelectSingleNode("PARAMETRI")

        longitudine = CStr(XmlNodo.GetAttribute("longitudine"))
        latitudine = CStr(XmlNodo.GetAttribute("latitudine"))

        Dim TipoSorgente As enum_Meteo_Tiposorgente
        If XmlNodo.HasAttribute("tiposorgente") Then
            TipoSorgente = CStr(XmlNodo.GetAttribute("tiposorgente"))
        Else
            TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner
        End If

        Dim Piva_SuperUser As String = ""
        If XmlNodo.HasAttribute("piva_superuser") Then
            Piva_SuperUser = CStr(XmlNodo.GetAttribute("piva_superuser"))
        End If
        XmlDoc = Nothing

        Dim Piva As String = ""
        If XmlNodo.HasAttribute("piva") Then
            Piva = CStr(XmlNodo.GetAttribute("piva"))
        End If
        XmlDoc = Nothing

        '-----------------------------------------------------------------------
        '--- Recupero le informazioni
        '-----------------------------------------------------------------------

        Dim dtStazioni As New DataTable
        Dim Metor_R_W As New AgronicaCoreMeteoDAL.Meteo_R

        Dim dlat As Double = 0
        If IsNumeric(latitudine.Replace(",", AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB)) Then
            dlat = CDbl(latitudine.Replace(",", AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB))
        End If

        Dim dlng As Double = 0
        If IsNumeric(longitudine.Replace(",", AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB)) Then
            dlng = CDbl(longitudine.Replace(",", AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB))
        End If



        dtStazioni = Metor_R_W.Agronica_Stazioni_Appoggio_Leggi_Con_Distanza(
            Piva_SuperUser,
            Piva,
            Utente_Username_client_GIAS,
            "",
            dlng,
            dlat,
            TipoSorgente,
            objParametri
            )



        'If dlat > 0 AndAlso dlng > 0 Then
        '    dtStazioni = Metor_R_W.Agronica_Stazioni_Appoggio_Leggi_Con_Distanza(
        '        Piva_SuperUser,
        '        Piva,
        '        Utente_Username_client_GIAS,
        '        "",
        '        dlng,
        '        dlat,
        '        TipoSorgente,
        '        objParametri
        '    )

        'Else
        '    dtStazioni = Metor_R_W.Agronica_Stazioni_Appoggio_Leggi(Piva_SuperUser, "", objParametri)
        '    dtStazioni.Columns.Add("distanza", GetType(Double))
        '    dtStazioni.Columns.Add("f_longitude", GetType(Double))
        '    dtStazioni.Columns.Add("f_latitude", GetType(Double))

        '    ' VAnni: 11/10/2017: data comunque richiesta in rilievi piogge..
        '    'dtStazioni.Columns.Add("data_ultimo_agg", GetType(Date))
        'End If



        '-----------------------------------------------------------------------
        '--- Genero la stringa XML di risposta
        '-----------------------------------------------------------------------

        Dim xRisultato As New xRisultato

        XmlRisultato = xRisultato.Dati_ListaStazioniDistanza(
                                                dtStazioni)

        Return XmlRisultato

    End Function

End Class
