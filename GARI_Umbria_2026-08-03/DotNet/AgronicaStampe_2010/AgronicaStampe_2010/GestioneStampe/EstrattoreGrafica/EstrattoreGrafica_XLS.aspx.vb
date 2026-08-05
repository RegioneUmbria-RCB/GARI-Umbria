Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports System.Windows.Forms
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaConversioneCartografiaGias
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaGIS2012.Commons
Imports System.Drawing


Partial Class EstrattoreGrafica_XLS
    Inherits System.Web.UI.Page

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Giorno As String
    Dim Qs_Validita_Inizio As String
    Dim Qs_Validita_Fine As String


    Dim Dt As DataTable
    Dim Dv As DataView

    Dim Dt_Risultato As DataTable
    Dim Dv_Risultato As DataView

    Dim Matrice_Variabili(0, 0) As String

    Dim str_Des_Layers As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = estrattoregrafica.xls")

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim strXmlVariabilistampe As String
        Dim strErr As String
        Dim i, j As Integer

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        Dim strDummy As String      'controllo accesso negato.....

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente( _
                            Session("ASG_Utente_Username"), _
                            Session("ASG_IdServizio"), _
                            enum_Security_Attivita.Gest_Stampe, _
                            enum_Security_Operazione.Modifica, _
                            Date.Now, _
                            "", _
                            objParametri_Utenti)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.


        '----- !!!!!!!!!!! -------------

        'Attivazione forzata provvisoria

        UtenteAbilitato = True

        '----- !!!!!!!!!!! -------------


        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If

        '##############################################################
        '#####  Recupero Piva e Sa_Cod  
        '##############################################################

        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            'Ricavo i parametri che servono
            Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            ReDim Matrice_Variabili(XMLs_VariabiliStampe.Count - 1, 2)

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                Matrice_Variabili(i, 0) = XML_VariabiliStampe.GetAttribute("piva")
                Matrice_Variabili(i, 1) = XML_VariabiliStampe.GetAttribute("sa_cod")

            Next

            Qs_Piva = Matrice_Variabili(0, 0)
            Qs_Sa_Cod = Matrice_Variabili(0, 1)


            Qs_Giorno = Stringa_Decodifica(Request.QueryString("dG").ToString, _
                                                    AgroKey_EncoderDecoder, _
                                                    Server)

            Qs_Validita_Inizio = Stringa_Decodifica(Request.QueryString("dI").ToString, _
                                                    AgroKey_EncoderDecoder, _
                                                    Server)

            Qs_Validita_Fine = Stringa_Decodifica(Request.QueryString("dF").ToString, _
                                                    AgroKey_EncoderDecoder, _
                                                    Server)

            Try

                CreaDtRisultati1()

            Catch ex As Exception

                strErr = ex.Message
                Me.TableDati.Rows(0).Cells(0).InnerHtml = strErr
                Exit Sub

            End Try


            '##############################################################
            '#####  Costruisco la tabella   ###############################
            '##############################################################

            Dim Riga As HtmlTableRow

            str_Des_Layers = Session("Des_Layers")
            Session("Des_Layers") = Nothing

            Me.TableDati.Rows(0).Cells(0).InnerHtml = "Impresa : <font color='red'>" & RagSoc_from_Piva(Qs_Piva) & _
                                                      "</font><br> Centro Aziendale : <font color='blue'>" & objCentriAz.SaNome_from_SaCod(Qs_Piva, CInt(Qs_Sa_Cod), objParametri_Server) & _
                                                      "</font><br> Layers selezionati : " & str_Des_Layers

            Me.TableDati.Rows(0).Cells(0).ColSpan = 7


            'Creo la prima riga con l'intestazione
            Riga = New HtmlTableRow

            For j = 0 To 6
                Riga.Cells.Add(New HtmlTableCell)
                ElaboraCellaHTML(Riga.Cells(j), 2, "", "", "Gainsboro", "center", "top")
            Next

            Riga.Cells(0).InnerHtml = "Layer"
            Riga.Cells(1).InnerHtml = "Descrizione"
            Riga.Cells(2).InnerHtml = "Date Validita'"
            Riga.Cells(3).InnerHtml = "X [m]"
            Riga.Cells(4).InnerHtml = "Y [m]"
            Riga.Cells(5).InnerHtml = "Perimetro [m]"
            Riga.Cells(6).InnerHtml = "Area [m2]"

            TableDati.Rows.Add(Riga)

            For i = 0 To Dt_Risultato.Rows.Count - 1

                Riga = New HtmlTableRow

                For j = 0 To 6

                    'aggiungo la cella
                    Riga.Cells.Add(New HtmlTableCell)

                    Select Case j

                        Case 0
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("layer")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("layer")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                        Case 1
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("descrizione")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("descrizione")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                        Case 2
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("date")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("date")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                        Case 3
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("x")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("x")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                        Case 4
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("y")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("y")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                        Case 5
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("perimetro")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("perimetro")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                        Case 6
                            If Not IsDBNull(Dt_Risultato.Rows(i).Item("area")) Then
                                Riga.Cells(j).InnerHtml = Dt_Risultato.Rows(i).Item("area")
                            Else
                                Riga.Cells(j).InnerHtml = ""
                            End If

                    End Select

                Next

                'Aggiungo la Riga alla Tabella 
                Me.TableDati.Rows.Add(Riga)

            Next


        End If



    End Sub


    '========================================================================

    Private Function RagSoc_from_Piva(ByVal piva As String) As String
        Dim anag As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim dt1 As DataTable = _
            anag.Leggi( _
                piva, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)


        Dim rval As String
        If dt1.Rows.Count > 0 Then
            rval = dt1(0)("Rag_Soc")
        End If
        Return rval


    End Function



    Private Sub CreaDtRisultati1()



        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi( _
            4, _
            enumSelezioneVariabile.Selezione_TabellaCompleta, _
            "", _
            "", _
            objParametri_Server _
        )


        Dim ParametriCartografici As New ParametriCoordinateConverter With { _
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"), _
            .CStoText = dtLeggiTrasformazione(0)("CSTo"), _
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"), _
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"), _
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"), _
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare") _
        }


        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml

        Dim cconverter As New AgronicaConversioneCartografiaGias.Agronica.CoordinateConverter

        Dim objSql As New AgronicaCoreDataProvider.DataProvider
        Dim Appezzamento_Read As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dt_Risultato = New DataTable
        Dim Dr_Risultato As DataRow

        Dim RsL As DataTable

        '----- Definisco la struttura del DataTable

        Dt_Risultato.Columns.Add(New DataColumn("layer", GetType(String)))
        Dt_Risultato.Columns.Add(New DataColumn("descrizione", GetType(String)))
        Dt_Risultato.Columns.Add(New DataColumn("date", GetType(String)))
        Dt_Risultato.Columns.Add(New DataColumn("x", GetType(String)))
        Dt_Risultato.Columns.Add(New DataColumn("y", GetType(String)))
        Dt_Risultato.Columns.Add(New DataColumn("perimetro", GetType(String)))
        Dt_Risultato.Columns.Add(New DataColumn("area", GetType(String)))


        Dim leggiDatiCartografici As New AgronicaCoreGisDAL.GIS_EstrattoreGrafica_R

        RsL = leggiDatiCartografici.Leggi( _
            Qs_Piva, _
            Qs_Sa_Cod, _
            Qs_Validita_Inizio, _
            Qs_Validita_Fine, _
            Session("Cod_Layers"), _
            "", _
            "", _
            objParametri_Server _
        )



        Dim addok As Boolean = True

        For Each drow In RsL.Rows

            addok = True

            'Creo una nuova riga x l'INTESTAZIONE ENTITA'
            Dr_Risultato = Dt_Risultato.NewRow

            Dim strDate As String = ""

            If drow.Item("Validita_Inizio") <> "01/01/1900" Then
                strDate = drow.Item("Validita_Inizio") & " - "
            Else
                strDate = "... - "
            End If


            If drow.Item("Validita_Fine") <> "31/12/2100" Then
                strDate = strDate & drow.Item("Validita_Fine")
            Else
                strDate = strDate & "..."
            End If

            Dr_Risultato.Item("date") = strDate


            Select Case drow("layerElementiGrafici_cod")
                Case 1

                    Dim App_Nome As String = _
                        Appezzamento_Read.AppezzamentoNome_from_Appezza(Qs_Piva, CInt(Qs_Sa_Cod), drow("Appezza"), objParametri_Server)

                    Dr_Risultato.Item("descrizione") = App_Nome
                    Dr_Risultato.Item("x") = ""
                    Dr_Risultato.Item("y") = ""

                    Dr_Risultato.Item("perimetro") = drow.Item("Perimetro")
                    Dr_Risultato.Item("area") = drow.Item("Area")

                    Dr_Risultato.Item("layer") = drow("LayerElementiGrafici_Des")


                Case 13  'CAMPIONI ANALISI NEW ---> CAMPIONAMENTI

                    Dr_Risultato.Item("descrizione") = DescrCampione_from_CodCampione(drow("Analisi_campione_cod"))
                    Dim strResult As String = _
                        cconverter.WKTPolygonWGS84_from_WKTPolygonED50(
                            drow("Poligono_GeoEntityWKT"), _
                            True, _
                            ParametriCartografici _
                        )


                    Dim lCoord As xyz
                    lCoord = wktHelp.CreaCoordinateDaPoligono(strResult).FirstOrDefault


                    Dr_Risultato.Item("x") = lCoord.X
                    Dr_Risultato.Item("y") = lCoord.Y

                    Dr_Risultato.Item("perimetro") = ""
                    Dr_Risultato.Item("area") = ""

                    Dr_Risultato.Item("layer") = drow("LayerElementiGrafici_Des")

                Case 12, 8  'ETTARI EQUIVALENTI, AREE OMOGENEE

                    Dr_Risultato.Item("descrizione") = drow("ElementoGrafico_Des")

                    Dr_Risultato.Item("x") = ""
                    Dr_Risultato.Item("y") = ""


                    Dr_Risultato.Item("perimetro") = drow.Item("Perimetro")
                    Dr_Risultato.Item("area") = drow.Item("Area")

                    Dr_Risultato.Item("layer") = drow("LayerElementiGrafici_Des")

                Case 4 'TESTO
                    Dr_Risultato.Item("layer") = drow("LayerElementiGrafici_Des")
                    Dr_Risultato.Item("descrizione") = drow("ElementoGrafico_Des")
                    Dr_Risultato.Item("x") = ""
                    Dr_Risultato.Item("y") = ""
                    Dr_Risultato.Item("perimetro") = ""
                    Dr_Risultato.Item("area") = ""

                Case Else
                    'non previsto.
                    addok = False

            End Select

            Dt_Risultato.Rows.Add(Dr_Risultato)

        Next


    End Sub


    


    '############################################################################
    '############################################################################
    '########## Funzioni per il calcolo Id_reg e Appezza senza BaseCode #########
    '############################################################################
    '############################################################################
    'Queste funzioni calcolano Id_reg e Appezza senza il BaseCode e viceversa
    'Servono per la tabella Grafica dove si è scelto di utilizzare ID per contenere
    'sia Appezza che Id_Reg
    Public Function AppezzaIdReg_Comprimi(ByVal Prefisso As String, ByVal Appezza As Long, ByVal Id_Reg As Long, ByVal BaseCode As Long) As String
        'Dato: Appezza e Id_Reg --> calcola --> ID

        Dim AppezzaCompresso As Long
        Dim Id_RegCompresso As Long

        Dim Segno As Char
        Dim lung As Short

        Dim ID As String

        'Output di default
        ID = "-1"

        Try

            If Appezza >= 0 Then
                lung = 8
                Segno = ""

                AppezzaCompresso = IIf(Appezza > BaseCode, Appezza - BaseCode, Appezza)
                ID = Prefisso & Left("0000", 4 - Len(CStr(Hex(AppezzaCompresso)))) & CStr(Hex(AppezzaCompresso))

                If Id_Reg >= 0 Then

                    Id_RegCompresso = IIf(Id_Reg > BaseCode, Id_Reg - BaseCode, Id_Reg)
                    ID = ID & Left("0000", 4 - Len(CStr(Hex(Id_RegCompresso)))) & CStr(Hex(Id_RegCompresso))

                Else
                    lung = 3
                    Segno = "-"
                    'ID = ID & Prefisso & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), lung)
                    ID = ID & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), lung)

                End If
            Else
                lung = 3
                Segno = "-"
                ID = Prefisso & Segno & Right("0000" & Hex(Math.Abs(Appezza)), lung)
                ID = ID & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), lung)

            End If

            'Return Prefisso & Segno & Right("00000000" & Hex(Math.Abs(Codice)), Lung)


            '------------------------------------------

            ''Elimino da Appezza e Id_Reg il BaseCode
            'AppezzaCompresso = Appezza - BaseCode
            'Id_RegCompresso = Id_Reg - BaseCode

            ''Creo l'ID da restituire con prefisso iniziale
            'ID = Prefisso & Left("0000", 4 - Len(CStr(Hex(AppezzaCompresso)))) & CStr(Hex(AppezzaCompresso))
            'ID = ID & Left("0000", 4 - Len(CStr(Hex(Id_RegCompresso)))) & CStr(Hex(Id_RegCompresso))

            '------------------------------------------

        Catch ex As Exception

            AppezzaCompresso = 0
            Id_RegCompresso = 0
            ID = "-1"

            'Attivo una condizione di errore
            AgroMsgBox("Errore nella costruzione della chiave grafica per l'impianto. " + ex.Message, Page)

        End Try

        Return ID

    End Function


    '***************************************************************************
    'Dato: ID --> calcola --> Appezza e Id_Reg
    Public Sub AppezzaIdReg_Decomprimi(ByVal ID As String, ByRef Appezza As Long, ByRef Id_Reg As Long, ByVal BaseCode As Long)


        Dim AppezzaCompresso As Long
        Dim Id_RegCompresso As Long

        Const MetodoNome = "AppezzaIdReg_Decomprimi"

        Try

            '------------------------------------------

            'Output di default
            Appezza = 0
            Id_Reg = 0

            If ID.Substring(1, 1) = "-" Then
                'Calcolo l'Appezza
                AppezzaCompresso = CLng("&H" & CStr(Left(Right(ID, 7), 3))) * (-1)


                'Calcolo l'Id_Reg
                Id_RegCompresso = CLng("&H" & CStr(Right(ID, 3))) * (-1)

            Else

                'Calcolo l'Appezza
                AppezzaCompresso = CLng("&H" & CStr(Left(Right(ID, 8), 4)))

                If ID.Substring(5, 1) = "-" Then 'Se l'impianto è stato creato da palmare(Id_Reg < 0)

                    'Calcolo l'Id_Reg
                    Id_RegCompresso = CLng("&H" & CStr(Right(ID, 3))) * (-1)

                Else 'Altrimenti Id_Reg > 0

                    'Calcolo l'Id_Reg
                    Id_RegCompresso = CLng("&H" & CStr(Right(ID, 4)))

                End If


            End If


            'Ora devo uniformare entrambi con il BaseCode

            If AppezzaCompresso > 0 Then

                Appezza = AppezzaCompresso + BaseCode

                If Id_RegCompresso > 0 Then

                    Id_Reg = Id_RegCompresso + BaseCode

                Else

                    Id_Reg = Id_RegCompresso

                End If

            Else

                Appezza = AppezzaCompresso
                Id_Reg = Id_RegCompresso

            End If


            '------------------------------------------

        Catch ex As Exception
            AppezzaCompresso = 0
            Id_RegCompresso = 0
            'Attivo una condizione di errore
            AgroMsgBox("Errore nella decodifica della chiave grafica dell'impianto. " + ex.Message, Page)
        End Try
    End Sub



    Private Function DescrCampione_from_CodCampione(ByVal Analisi_Campione_cod As Integer) As String

        Dim objSql As New AgronicaCoreDataProvider.DataProvider

        Dim Rs As DataTable
        Dim StrSql As String


        StrSql = ""
        'StrSql = "  SELECT Analisi_Campione_Des "
        'StrSql += " FROM Analisi_Campioni "
        'StrSql += " WHERE Analisi_Campione_Cod =" + SQL_SaveNum(Cod_Campione) + " "

        StrSql += " SELECT  Analisi_Testata.Analisi_SuperUser, Analisi_Testata.Analisi_Testata_Cod, Analisi_Testata.Analisi_Testata_Des, Analisi_Dettagli.Analisi_Dettaglio_Cod,  "
        StrSql += "         Analisi_Campioni.Analisi_Campione_Cod, Analisi_Campioni.Analisi_Campione_Des, Analisi_Testata.Analisi_Testata_Riferimento_1, "
        StrSql += "         Analisi_Campioni.Analisi_Campione_Riferimento_1 "
        StrSql += " FROM    Analisi_Campioni INNER JOIN "
        StrSql += "         Analisi_Dettagli INNER JOIN "
        StrSql += "         Analisi_CampionixDettagli ON Analisi_Dettagli.Analisi_SuperUser = Analisi_CampionixDettagli.Analisi_SuperUser AND  "
        StrSql += "         Analisi_Dettagli.Analisi_Testata_Cod = Analisi_CampionixDettagli.Analisi_Testata_Cod AND  "
        StrSql += "         Analisi_Dettagli.Analisi_Dettaglio_Cod = Analisi_CampionixDettagli.Analisi_Dettaglio_Cod INNER JOIN "
        StrSql += "         Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  "
        StrSql += "         Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ON  "
        StrSql += "         Analisi_Campioni.Analisi_Campione_Cod = Analisi_CampionixDettagli.Analisi_Campione_Cod "
        StrSql += " WHERE   (Analisi_Campioni.Analisi_Campione_Cod = " + Agro_SQL_SaveNum(Analisi_Campione_cod) + ") "


        Rs = objSql.EseguiQuery_Lettura(objParametri_Server, StrSql, "")

        Dim Descrizione As String = ""

        If (Not IsNothing(Rs)) AndAlso (Rs.Rows.Count > 0) Then

            If Not IsDBNull(Rs(0)("Analisi_Testata_Des")) Then
                Descrizione = Rs(0)("Analisi_Testata_Des")
            Else
                Descrizione = " "
            End If

            If Not IsDBNull(Rs(0)("Analisi_Testata_Riferimento_1")) Then
                Descrizione += " - " + Rs(0)("Analisi_Testata_Riferimento_1")
            Else
                Descrizione += " - "
            End If

            If Not IsDBNull(Rs(0)("Analisi_Campione_Riferimento_1")) Then
                Descrizione += " - " + Rs(0)("Analisi_Campione_Riferimento_1")
            Else
                Descrizione += " - "
            End If

        End If


        Return Descrizione



    End Function


End Class
