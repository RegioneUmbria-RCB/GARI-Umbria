Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider

Public Class Mappe_Read
    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function Mappa_Leggi(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal ForDelete As Boolean,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As String
        '============================================================================

        Dim NomeRoutine As String = "GraficaBIZ.Mappe_Read.Mappa_Leggi()"

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim RisultatoFunzione As String = String.Empty

        Dim dt As DataTable

        Dim ObjLeggiAziInfoRER As New AgronicaCoreGraficaDAL.AziInfoRER_Read

        Dim XmlDoc As XmlDocument

        Dim XmlDati As XmlElement
        Dim XmlDato As XmlElement
        Dim XmlSfondo As XmlElement

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
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            '------------------------------



            dt = ObjLeggiAziInfoRER.Leggi(Piva,
                                       Sa_Cod,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "",
                                       "",
                                       objParametri)



            Dim ObjLeggiCentriXSfondi As New AgronicaCoreGraficaDAL.CentriXSfondi_Read
            Dim dtSfondi = ObjLeggiCentriXSfondi.Leggi( _
                                                      Piva, _
                                                      Sa_Cod, _
                                                      0, _
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                      "", "", objParametri)




            'Se ottengo almeno un risultato, creo la struttura XML
            If Not dtSfondi Is Nothing AndAlso dtSfondi.Rows.Count > 0 Then

                XmlDoc = New XmlDocument

                XmlDati = XmlDoc.CreateElement("DatiMappa")


                'Effettuo un ciclo sui layers dell'utente
                For i = 0 To dtSfondi.Rows.Count - 1

                    '----- <AziInfoRER> -----
                    XmlDato = XmlDoc.CreateElement("AziInfoRER")


                    'XmlDato.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                    'XmlDato.SetAttribute("piva", Agro_SQL_Load(dt.Rows(i).Item("piva")))
                    'XmlDato.SetAttribute("sa_cod", Agro_SQL_Load(dt.Rows(i).Item("sa_cod")))
                    'XmlDato.SetAttribute("quadrante", Agro_SQL_Load(dt.Rows(i).Item("quadrante")))
                    'XmlDato.SetAttribute("meteoattivo", Agro_SQL_Load(dt.Rows(i).Item("meteoattivo")))
                    'XmlDato.SetAttribute("tipometeo", Agro_SQL_Load(dt.Rows(i).Item("tipometeo")))
                    'XmlDato.SetAttribute("sfondobackground", Agro_SQL_Load(dt.Rows(i).Item("sfondobackground")))
                    'XmlDato.SetAttribute("sfondocod", Agro_SQL_Load(dt.Rows(i).Item("sfondocod")))
                    'XmlDato.SetAttribute("validita_inizio", Agro_SQL_Load(dt.Rows(i).Item("validita_inizio")))
                    'XmlDato.SetAttribute("validita_fine", Agro_SQL_Load(dt.Rows(i).Item("validita_fine")))


                    XmlDato.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                    XmlDato.SetAttribute("piva", Agro_SQL_Load(dtSfondi.Rows(i).Item("piva")))
                    XmlDato.SetAttribute("sa_cod", Agro_SQL_Load(dtSfondi.Rows(i).Item("sa_cod")))
                    XmlDato.SetAttribute("quadrante", "0")
                    XmlDato.SetAttribute("meteoattivo", "0")
                    XmlDato.SetAttribute("tipometeo", "0")
                    XmlDato.SetAttribute("sfondobackground", "0")
                    XmlDato.SetAttribute("sfondocod", "1")
                    XmlDato.SetAttribute("validita_inizio", Agro_SQL_Load(dtSfondi.Rows(i).Item("validita_inizio")))
                    XmlDato.SetAttribute("validita_fine", Agro_SQL_Load(dtSfondi.Rows(i).Item("validita_fine")))



                    'Dim ObjLeggiCentriXSfondi As New AgronicaCoreGraficaDAL.CentriXSfondi_Read
                    'Dim dtSfondi = ObjLeggiCentriXSfondi.Leggi( _
                    '                                          Piva, _
                    '                                          Sa_Cod, _
                    '                                          0, _
                    '                                          enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    '                                          "", "", objParametri)



                    Dim j As Integer = 0
                    'For j = 0 To dtSfondi.Rows.Count - 1

                    XmlSfondo = XmlDoc.CreateElement("CentriXSfondi")

                    '<CentriXSfondi> 

                    XmlSfondo.SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                    XmlSfondo.SetAttribute("piva", Agro_SQL_Load(dtSfondi.Rows(j).Item("piva")))
                    XmlSfondo.SetAttribute("sa_cod", Agro_SQL_Load(dtSfondi.Rows(j).Item("sa_cod")))
                    XmlSfondo.SetAttribute("sfondocod", Agro_SQL_Load(dtSfondi.Rows(j).Item("sfondocod")))
                    XmlSfondo.SetAttribute("ixno", Agro_SQL_Load(dtSfondi.Rows(j).Item("ixno")))
                    XmlSfondo.SetAttribute("iyno", Agro_SQL_Load(dtSfondi.Rows(j).Item("iyno")))
                    XmlSfondo.SetAttribute("ixse", Agro_SQL_Load(dtSfondi.Rows(j).Item("ixse")))
                    XmlSfondo.SetAttribute("iyse", Agro_SQL_Load(dtSfondi.Rows(j).Item("iyse")))
                    XmlSfondo.SetAttribute("oxno", Agro_SQL_Load(dtSfondi.Rows(j).Item("oxno")))
                    XmlSfondo.SetAttribute("oyno", Agro_SQL_Load(dtSfondi.Rows(j).Item("oyno")))
                    XmlSfondo.SetAttribute("oxse", Agro_SQL_Load(dtSfondi.Rows(j).Item("oxse")))
                    XmlSfondo.SetAttribute("oyse", Agro_SQL_Load(dtSfondi.Rows(j).Item("oyse")))

                    XmlSfondo.SetAttribute("LatMin", Agro_SQL_Load(dtSfondi.Rows(j).Item("LatMin")))
                    XmlSfondo.SetAttribute("LngMin", Agro_SQL_Load(dtSfondi.Rows(j).Item("LngMin")))
                    XmlSfondo.SetAttribute("LatMax", Agro_SQL_Load(dtSfondi.Rows(j).Item("LatMax")))
                    XmlSfondo.SetAttribute("LngMax", Agro_SQL_Load(dtSfondi.Rows(j).Item("LngMax")))

                    XmlSfondo.SetAttribute("filebitmap", Agro_SQL_Load(dtSfondi.Rows(j).Item("filebitmap")))
                    XmlSfondo.SetAttribute("pathbitmap", Agro_SQL_Load(dtSfondi.Rows(j).Item("pathbitmap")))
                    XmlSfondo.SetAttribute("validita_inizio", Agro_SQL_Load(dtSfondi.Rows(j).Item("validita_inizio")))
                    XmlSfondo.SetAttribute("validita_fine", Agro_SQL_Load(dtSfondi.Rows(j).Item("validita_fine")))

                    '</CentriXSfondi> 

                    XmlDato.AppendChild(XmlSfondo)

                    'Next

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
