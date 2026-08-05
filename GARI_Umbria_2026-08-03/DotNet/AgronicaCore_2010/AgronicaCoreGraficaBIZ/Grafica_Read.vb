Imports System.Xml
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider

Public Class Grafica_Read
    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function Grafica_Leggi(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal sezione As String,
                                    ByVal Id As String,
                                    ByVal ForDelete As Boolean,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByVal TipoOperazioneDB As Integer = 0,
                                    Optional ByVal XFiltroAggiuntivo As String = ""
                                 ) As String
        '============================================================================

        Dim NomeRoutine As String = "GraficaBIZ.Grafica_Read.Grafica_Leggi()"

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim RisultatoFunzione As String = String.Empty

        Dim dt As DataTable

        Dim ObjLeggi As New AgronicaCoreGraficaDAL.Grafica_Read

        Dim XmlDoc As XmlDocument

        Dim XmlDati As XmlElement
        Dim XmlDato As XmlElement

        Dim i As Integer

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            If objParametri.objConnessione.State = ConnectionState.Closed Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                'objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            '------------------------------

            If TipoOperazioneDB > 0 Then

                dt = ObjLeggi.LeggiPerSincronizzazioneGIS2012(Piva, _
                                           Sa_Cod, _
                                           sezione, _
                                           Id, _
                                           AccodaPrefissoAccessorio(XFiltroAggiuntivo), _
                                           TipoOperazioneDB, _
                                           enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                           "", "", objParametri)
            Else
                dt = ObjLeggi.Leggi(Piva, _
                                           Sa_Cod, _
                                           sezione, _
                                           Id, _
                                           enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                           XFiltroAggiuntivo, "", objParametri)
            End If



            'Se ottengo almeno un risultato, creo la struttura XML
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                XmlDoc = New XmlDocument

                XmlDati = XmlDoc.CreateElement("DatiEntita")


                'Effettuo un ciclo sui layers dell'utente
                For i = 0 To dt.Rows.Count - 1

                    '----- <Entita> -----
                    XmlDato = XmlDoc.CreateElement("Entita")

                    If TipoOperazioneDB And Not ForDelete Then
                        XmlDato.SetAttribute("TipoOperazioneDB", Agro_SQL_Load(dt.Rows(i).Item("TipoOperazioneDB")))
                        XmlDato.SetAttribute("entita_cod", Agro_SQL_Load(dt.Rows(i).Item("entita_cod")))
                        XmlDato.SetAttribute("elementografico_cod", Agro_SQL_Load(dt.Rows(i).Item("elementografico_cod")))
                    Else
                        XmlDato.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                    End If

                    XmlDato.SetAttribute("recno", "")
                    XmlDato.SetAttribute("deleted", "")
                    XmlDato.SetAttribute("section", Agro_SQL_Load(dt.Rows(i).Item("section")))
                    XmlDato.SetAttribute("id", Agro_SQL_Load(dt.Rows(i).Item("id")))
                    XmlDato.SetAttribute("prefisso", GraphicKeyGetPrefix(Agro_SQL_Load(dt.Rows(i).Item("id"))))
                    XmlDato.SetAttribute("codice", GraphicKeyGetCode(Agro_SQL_Load(dt.Rows(i).Item("id"))))
                    XmlDato.SetAttribute("descr", Agro_SQL_Load(dt.Rows(i).Item("descr")))
                    XmlDato.SetAttribute("ecolor", Agro_SQL_Load(dt.Rows(i).Item("ecolor")))
                    XmlDato.SetAttribute("layer", Agro_SQL_Load(dt.Rows(i).Item("layer")))
                    XmlDato.SetAttribute("eline", Agro_SQL_Load(dt.Rows(i).Item("eline")))
                    XmlDato.SetAttribute("vx1", Agro_SQL_Load(dt.Rows(i).Item("vx1")))
                    XmlDato.SetAttribute("vy1", Agro_SQL_Load(dt.Rows(i).Item("vy1")))
                    XmlDato.SetAttribute("vx2", Agro_SQL_Load(dt.Rows(i).Item("vx2")))
                    XmlDato.SetAttribute("vy2", Agro_SQL_Load(dt.Rows(i).Item("vy2")))
                    XmlDato.SetAttribute("rad", Agro_SQL_Load(dt.Rows(i).Item("rad")))
                    XmlDato.SetAttribute("text", Agro_SQL_Load(dt.Rows(i).Item("text")))
                    XmlDato.SetAttribute("gps", Agro_SQL_Load(dt.Rows(i).Item("gps")))
                    XmlDato.SetAttribute("lat", Agro_SQL_Load(dt.Rows(i).Item("Lat")))
                    XmlDato.SetAttribute("lon", Agro_SQL_Load(dt.Rows(i).Item("Lon")))
                    XmlDato.SetAttribute("pdop", Agro_SQL_Load(dt.Rows(i).Item("Pdop")))


                    If dt.Columns.Contains("prov") Then
                        XmlDato.SetAttribute("prov", Agro_SQL_Load(dt.Rows(i).Item("prov")))
                    Else
                        XmlDato.SetAttribute("prov", "")
                    End If

                    If dt.Columns.Contains("com") Then
                        XmlDato.SetAttribute("com", Agro_SQL_Load(dt.Rows(i).Item("com")))
                    Else
                        XmlDato.SetAttribute("com", "")
                    End If

                    If dt.Columns.Contains("sezione") Then
                        XmlDato.SetAttribute("sezione", Agro_SQL_Load(dt.Rows(i).Item("sezione")))
                    Else
                        XmlDato.SetAttribute("sezione", "")
                    End If

                    If dt.Columns.Contains("foglio") Then
                        XmlDato.SetAttribute("foglio", Agro_SQL_Load(dt.Rows(i).Item("foglio")))
                    Else
                        XmlDato.SetAttribute("foglio", 0)
                    End If

                    If dt.Columns.Contains("numero") Then
                        XmlDato.SetAttribute("numero", Agro_SQL_Load(dt.Rows(i).Item("numero")))
                    Else
                        XmlDato.SetAttribute("numero", 0)
                    End If

                    If dt.Columns.Contains("subalterno") Then
                        XmlDato.SetAttribute("subalterno", Agro_SQL_Load(dt.Rows(i).Item("subalterno")))
                    Else
                        XmlDato.SetAttribute("subalterno", "")
                    End If

                    If dt.Columns.Contains("partita_catastale") Then
                        XmlDato.SetAttribute("partita_catastale", Agro_SQL_Load(dt.Rows(i).Item("partita_catastale")))
                    Else
                        XmlDato.SetAttribute("partita_catastale", "")
                    End If


                    If dt.Columns.Contains("analisi_campione_cod") Then
                        XmlDato.SetAttribute("analisi_campione_cod", Agro_SQL_Load(dt.Rows(i).Item("analisi_campione_cod")))
                    Else
                        XmlDato.SetAttribute("analisi_campione_cod", 0)
                    End If

                    If dt.Columns.Contains("id_agenda") Then
                        XmlDato.SetAttribute("id_agenda", Agro_SQL_Load(dt.Rows(i).Item("id_agenda")))
                    Else
                        XmlDato.SetAttribute("id_agenda", 0)
                    End If

                    If dt.Columns.Contains("ricetta_operazione_cod") Then
                        XmlDato.SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(dt.Rows(i).Item("ricetta_operazione_cod")))
                    Else
                        XmlDato.SetAttribute("ricetta_operazione_cod", 0)
                    End If


                    'XmlDato.SetAttribute("prov", Agro_SQL_Load(dt.Rows(i).Item("prov")))
                    'XmlDato.SetAttribute("com", Agro_SQL_Load(dt.Rows(i).Item("com")))
                    'XmlDato.SetAttribute("sezione", Agro_SQL_Load(dt.Rows(i).Item("sezione")))
                    'XmlDato.SetAttribute("foglio", Agro_SQL_Load(dt.Rows(i).Item("foglio")))
                    'XmlDato.SetAttribute("numero", Agro_SQL_Load(dt.Rows(i).Item("numero")))
                    'XmlDato.SetAttribute("subalterno", Agro_SQL_Load(dt.Rows(i).Item("subalterno")))
                    'XmlDato.SetAttribute("partita_catastale", Agro_SQL_Load(dt.Rows(i).Item("partita_catastale")))

                    'XmlDato.SetAttribute("analisi_campione_cod", Agro_SQL_Load(dt.Rows(i).Item("analisi_campione_cod")))
                    'XmlDato.SetAttribute("id_agenda", Agro_SQL_Load(dt.Rows(i).Item("id_agenda")))
                    'XmlDato.SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(dt.Rows(i).Item("ricetta_operazione_cod")))

                    XmlDato.SetAttribute("validita_inizio", Agro_SQL_Load(dt.Rows(i).Item("Validita_Inizio")))
                    XmlDato.SetAttribute("validita_fine", Agro_SQL_Load(dt.Rows(i).Item("Validita_Fine")))

                    XmlDato.SetAttribute("data_creazione", Agro_SQL_Load(dt.Rows(i).Item("data_creazione")))
                    XmlDato.SetAttribute("data_modifica", Agro_SQL_Load(dt.Rows(i).Item("data_modifica")))
                    XmlDato.SetAttribute("username_creazione", Agro_SQL_Load(dt.Rows(i).Item("username_creazione")))
                    XmlDato.SetAttribute("username_modifica", Agro_SQL_Load(dt.Rows(i).Item("username_modifica")))

                    XmlDati.AppendChild(XmlDato)
                    '----- </ GRAFICA > -----

                    XmlDato = Nothing

                Next


                XmlDoc.AppendChild(XmlDati)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----

                XmlDati = Nothing
                XmlDoc = Nothing

            Else

                'nessun risultato
                RisultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            dt = Nothing
            ObjLeggi = Nothing



        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale = True Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()

                    objParametri.objConnessione = Nothing
                    objParametri.objTransazione = Nothing
                End If
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function


    '==========================================================================
    '  © AGRONICA Srl - Stefano Flamigni - 2002 Ottobre
    '==========================================================================
    '  DESCRIZIONE :
    '  Estrae il prefisso dalla chiave composta
    '
    '
    Private Function GraphicKeyGetPrefix(ByVal GraphicKey As String) As String

        If GraphicKey <> "" Then
            If Len(GraphicKey) = 9 Then
                GraphicKeyGetPrefix = Left(GraphicKey, 1)
            Else
                GraphicKeyGetPrefix = ""
            End If
        Else
            GraphicKeyGetPrefix = ""
        End If

    End Function

    '==========================================================================
    '  © AGRONICA Srl - Stefano Flamigni - 2002 Ottobre
    '==========================================================================
    '  DESCRIZIONE :
    '  Estrae il codice dalla chiave composta
    '
    '
    Private Function GraphicKeyGetCode(ByVal GraphicKey As String) As Long

        If GraphicKey <> "" Then
            If Len(GraphicKey) = 9 Then
                GraphicKeyGetCode = CLng("&H" & Mid(GraphicKey, 2))
            Else
                GraphicKeyGetCode = -1
            End If
        Else
            GraphicKeyGetCode = -1
        End If

    End Function

    ''' <summary>
    ''' Dati i layer in formato "I,A" restituisce "F,E,I,A"
    ''' </summary>
    ''' <param name="prefisso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function AccodaPrefissoAccessorio(ByVal prefisso As String) As String
        Dim tmp As String() = prefisso.Split(",")
        Dim apici As String = ""
        If prefisso.StartsWith("'") Then
            apici = "'"
        End If
        Dim rval As String = ""
        For Each s As String In tmp
            rval &= apici & lDammiPrefissoAccessorio(s.Replace("'", "")) & apici & ","
        Next

        Return rval & prefisso

    End Function
    Private Shared Function lDammiPrefissoAccessorio(ByVal prefisso As String) As String
        Select Case prefisso
            Case "A"
                Return "E"
            Case "D"
                Return "Q"
            Case "L"
                Return "B"
            Case "T"
                Return "T"
            Case "O"
                Return "P"
            Case "W"
                Return "K"
            Case "X"
                Return "Y"
            Case "M"
                Return "N"
            Case "I"
                Return "F"
            Case "G"
                Return "H"
            Case Else
                Return "Z"

                'A(E)
                'D(Q)
                'T(T)
                'L(B)
                'O(P)
                'W(K)
                'X(Y)                
                'M(N)
                'I(F)
        End Select

    End Function

End Class
