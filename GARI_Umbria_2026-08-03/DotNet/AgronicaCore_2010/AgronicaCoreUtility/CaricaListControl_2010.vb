Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports System.Web.UI.WebControls
Imports AgronicaCoreGestioneRichieste
Imports System.Web

Public Class CaricaListControl_2010
    Inherits AgronicaCoreDataProvider.DataProvider

    'è stato messo in AgronicaCoreVarieBIZ





    '###############################################################################
    'OBSOLETA:
    'USARE ProdottiAnagrafica oppure ProdottiMagazzino
    'default:
    'Flag_VisualizzaProCod As Boolean = True
    'Public Shared Sub Prodotti_Old(ByRef Controllo As ListControl, _
    '                            ByVal PrimaRiga_Flag As Boolean, _
    '                            ByVal PrimaRiga_Text As String, _
    '                            ByVal PrimaRiga_Value As String, _
    '                            ByVal Cau_Mov As String, _
    '                            ByVal Piva As String, _
    '                            ByVal Sa_Cod As Integer, _
    '                            ByVal Id_Destinazione As Integer, _
    '                            ByVal Elem_Cod As Integer, _
    '                            ByVal Flag_Negativo As Boolean, _
    '                            ByVal RicercaTesto As String, _
    '                            ByVal Flag_VisualizzaProCod As Boolean, _
    '                            ByVal Pro_Cod As Integer, _
    '                            ByVal Udm_Cod As Integer, _
    '                            ByVal DataFiltroFormulati As Date, _
    '                            ByVal PUA_RegolamentoCod As Integer, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByVal xOrderBy As String, _
    '                            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                            ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            )


    '    Dim Dt_Categorie As DataTable
    '    Dim Dt_Giacenze As DataTable
    '    Dim Dt_Prodotti, Dt_FertAzi As DataTable
    '    Dim i, j As Integer
    '    Dim x_Pro_Cod As Integer
    '    Dim x_Pro_Des As String
    '    Dim NomeTabella As String
    '    Dim NomeCodice As String
    '    Dim NomeDescrizione As String


    '    'Pulisco il controllo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag = True Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
    '    Dim objCat As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

    '    Select Case Cau_Mov

    '        Case CAU_CARICO

    '            '#############################################
    '            '##############     CARICO    ################
    '            '#############################################

    '            'Leggo l'anagrafica prodotti

    '            'legge le categorie di magazzino
    '            Dt_Categorie = objCat.Leggi(Elem_Cod, _
    '                                         CAU_MAGAZZINO, _
    '                                          False, _
    '                                          "", "", _
    '                                          objParametri_server)

    '            Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R


    '            'la categoria di magazzino è 1
    '            For i = 0 To Dt_Categorie.Rows.Count - 1

    '                NomeTabella = Dt_Categorie.Rows(i).Item("Tabella")
    '                NomeCodice = Dt_Categorie.Rows(i).Item("Tabella_Cod")
    '                NomeDescrizione = Dt_Categorie.Rows(i).Item("Tabella_Des")

    '                Select Case Elem_Cod

    '                    Case FERTILIZZANTI

    '                        'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
    '                        'perchè oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
    '                        '(per i fertilizzanti aziendali)

    '                        'Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
    '                        '                                                   Piva, _
    '                        '                                                   0, _
    '                        '                                                   True, _
    '                        '                                                   RicercaTesto)

    '                        Dim TipoRichiesto As Integer = 0

    '                        Select Case PUA_RegolamentoCod
    '                            Case 0
    '                                TipoRichiesto = 0
    '                            Case 1
    '                                TipoRichiesto = 6
    '                            Case Else
    '                                TipoRichiesto = 7
    '                        End Select

    '                        Dt_Prodotti = objFert.Leggi_Completa(0, _
    '                                                            RicercaTesto, _
    '                                                            TipoRichiesto, _
    '                                                            True, _
    '                                                            Piva, _
    '                                                            0, _
    '                                                            objParametri_server.FinestraTemporaleInizio, _
    '                                                            objParametri_server.FinestraTemporaleFine, _
    '                                                            "", "  " & NomeDescrizione & " ", _
    '                                                            objParametri_server, _
    '                                                            PUA_RegolamentoCod)

    '                    Case Else

    '                        Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella, _
    '                                                                             NomeCodice, _
    '                                                                             NomeDescrizione, _
    '                                                                             RicercaTesto, _
    '                                                                             0, "", " " & NomeDescrizione & " ", _
    '                                                                             objParametri_server)





    '                        '''''''''''''''''''''''''''''modifica webservice
    '                        Dim XML_Credenziali As System.Xml.XmlElement
    '                        Dim StrCredenziali As String = ""
    '                        Dim StrParametri As String = ""
    '                        Dim Parametri As String = ""
    '                        Dim strErr As String = ""
    '                        Dim LastFr_Des As String = ""


    '                        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
    '                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

    '                        Dim objAgroWebConfig As New AgroWebConfig
    '                        Dim XmlDoc As New System.Xml.XmlDocument


    '                        objWs.NewWS(ObjDownloadWs, _
    '                                        objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
    '                                        objParametri_utenti)

    '                        Try



    '                            XmlDoc = New System.Xml.XmlDocument

    '                            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

    '                            Dim str_fr_cod As String = ""

    '                            Dim jj As Integer
    '                            For jj = 0 To Dt_Prodotti.Rows.Count - 1
    '                                If str_fr_cod.Length = 0 Then
    '                                    str_fr_cod = Dt_Prodotti.Rows(jj).Item("fr_cod")
    '                                Else
    '                                    str_fr_cod = str_fr_cod & "," & Dt_Prodotti.Rows(jj).Item("fr_cod")
    '                                End If
    '                            Next
    '                            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                            StrCredenziali, _
    '                                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo, _
    '                                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo), _
    '                                                            HttpContext.Current.Session("ASG_ProgressivoGIAS"), _
    '                                                            HttpContext.Current.Session("ASG_SuperUser_Username").ToString, _
    '                                                            HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

    '                            XmlDoc.LoadXml(StrCredenziali)

    '                            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '                            'Dim grfi_Cod As Integer = 0
    '                            objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                                   StrParametri, _
    '                                                                   str_fr_cod, _
    '                                                                   strErr)

    '                            XML_Credenziali.InnerXml = StrParametri

    '                            Parametri = XmlDoc.OuterXml

    '                            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)
    '                            Dim dt As DataTable
    '                            dt = ObjDownloadWs.Leggi_Formulati_Info_DT(Parametri, strErr)
    '                            For jj = 0 To Dt_Prodotti.Rows.Count - 1

    '                                Dim dr As DataRow() = dt.Select("fr_cod = " & Dt_Prodotti.Rows(jj).Item("fr_cod"))

    '                                Dt_Prodotti.Rows(jj).Item("data_reg") = dr(0).Item("data_reg")
    '                                Dt_Prodotti.Rows(jj).Item("Data_fine_comm") = dr(0).Item("Data_fine_comm")
    '                                Dt_Prodotti.Rows(jj).Item("Data_fine_usoscorte") = dr(0).Item("Data_fine_usoscorte")
    '                                Dt_Prodotti.Rows(jj).Item("revocato") = dr(0).Item("revocato")
    '                            Next
    '                            ''verifico se è tutto ok

    '                            ObjDownloadWs.Dispose()

    '                        Catch ex As Exception

    '                        End Try


    '                        '''''''''''''''''''''' fine modifica webservice




    '                End Select


    '                If Not IsNothing(Dt_Prodotti) Then

    '                    Select Case Elem_Cod

    '                        Case FORMULATI

    '                            'nel caso dei formulati devo filtrare per evitare di visualizzare prodotti revocati, ecc
    '                            'Dt_Prodotti = objFito.Filtra_Formulati(Dt_Prodotti, DataFiltroFormulati, objParametri)
    '                            Dt_Prodotti = objFito.Filtra_Formulati_2(Dt_Prodotti, DataFiltroFormulati, objParametri_server)

    '                    End Select

    '                    'NumTotale = Dt_Prodotti.Rows.Count

    '                    For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                        Select Case Elem_Cod

    '                            Case FERTILIZZANTI
    '                                'value salvato nella modalità gestita dalla formprodotto
    '                                If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
    '                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
    '                                ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
    '                                    x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
    '                                Else
    '                                    'errore, qui non dovrebbe mai entrare
    '                                    x_Pro_Cod = 0
    '                                End If

    '                            Case Else
    '                                x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                        End Select

    '                        If Not IsDBNull(x_Pro_Cod) Then

    '                            If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then

    '                                If Flag_VisualizzaProCod = True Then
    '                                    x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                                 " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                                Else
    '                                    x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                                End If

    '                                Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

    '                            End If
    '                        End If


    '                    Next
    '                End If

    '            Next

    '            'Dt_Categorie.Dispose()

    '            'If Not IsNothing(Dt_Prodotti) Then
    '            '    Dt_Prodotti.Dispose()
    '            'End If

    '            Dt_Categorie = Nothing
    '            Dt_Prodotti = Nothing


    '        Case CAU_SCARICO

    '            '#############################################
    '            '##############     SCARICO    ###############
    '            '#############################################

    '            Dim Dt_Temp As New DataTable
    '            Dim DrTemp As DataRow

    '            Dt_Temp.Columns.Add(New DataColumn("Des", GetType(String)))
    '            Dt_Temp.Columns.Add(New DataColumn("Cod", GetType(Integer)))


    '            'leggo i prodotti in magazzino

    '            Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R


    '            'legge le giacenze con le categorie di magazzino
    '            'Dt_Giacenze = objGiacenze.Giacenze_Leggi(Piva, _
    '            '                                        Sa_Cod, _
    '            '                                        Id_Destinazione, _
    '            '                                        0, 0, _
    '            '                                        Elem_Cod, _
    '            '                                        0, 0, 0, 0, 0, 0, "", _
    '            '                                        0, 0, 0, _
    '            '                                         AGRODATAFINE, _
    '            '                                        "", "", _
    '            '                                        objParametri)

    '            Dt_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltroFormulati, _
    '                                                              Piva, _
    '                                                              Sa_Cod, _
    '                                                              Id_Destinazione, _
    '                                                              Elem_Cod, _
    '                                                              0, 0, 0, 0, 0, 0, _
    '                                                              LOTTO_NONDEFINITO, _
    '                                                              False, _
    '                                                              "", "", "", "", "", "", "", "", "", "", "", "", objParametri_server)


    '            For i = 0 To Dt_Giacenze.Rows.Count - 1

    '                NomeTabella = Dt_Giacenze.Rows(i).Item("Tabella")
    '                NomeCodice = Dt_Giacenze.Rows(i).Item("Tabella_Cod")
    '                NomeDescrizione = Dt_Giacenze.Rows(i).Item("Tabella_Des")

    '                Select Case Elem_Cod

    '                    Case FERTILIZZANTI

    '                        'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
    '                        'perchè oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
    '                        '(per i fertilizzanti aziendali)

    '                        'Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
    '                        '                                                   Piva, _
    '                        '                                                   0, _
    '                        '                                                   True, _
    '                        '                                                   RicercaTesto, _
    '                        '                                                   Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                        '                                                   Dt_Giacenze.Rows(i).Item("Mat_Cod"))


    '                        Dt_Prodotti = objFert.Leggi_Completa(Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                                                    RicercaTesto, _
    '                                                    0, _
    '                                                    True, _
    '                                                    Piva, _
    '                                                    Dt_Giacenze.Rows(i).Item("Mat_Cod"), _
    '                                                    objParametri_server.FinestraTemporaleInizio, _
    '                                                    objParametri_server.FinestraTemporaleFine, _
    '                                                    "", " " & NomeDescrizione & " ", _
    '                                                    objParametri_server, _
    '                                                    0)

    '                    Case Else

    '                        'Dt_Prodotti = NewCom_LeggiTabella_da_CategorieMagazzino(objServer, objSession, objPage, _
    '                        '                                                        NomeTabella, _
    '                        '                                                        NomeCodice, _
    '                        '                                                        NomeDescrizione, _
    '                        '                                                        RicercaTesto, _
    '                        '                                                        Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                        '                                                        "")

    '                        Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella, _
    '                                                     NomeCodice, _
    '                                                     NomeDescrizione, _
    '                                                     RicercaTesto, _
    '                                                     Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                                                     "", " " & NomeDescrizione & " ", _
    '                                                     objParametri_server)

    '                End Select


    '                If Not IsNothing(Dt_Prodotti) Then

    '                    For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                        Select Case Elem_Cod

    '                            Case FERTILIZZANTI
    '                                'value salvato nella modalità gestita dalla formprodotto
    '                                If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
    '                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
    '                                ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
    '                                    x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
    '                                Else
    '                                    'errore, qui non dovrebbe mai entrare
    '                                    x_Pro_Cod = 0
    '                                End If

    '                            Case Else
    '                                x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                        End Select

    '                        If Not IsDBNull(x_Pro_Cod) Then
    '                            'If IsNothing(Cmb.Items.FindByValue(x_Pro_Cod)) Then

    '                            If Flag_VisualizzaProCod = True Then
    '                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                            " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                            Else
    '                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                            End If

    '                            'salvo in un dt di appoggio (da utilizzare per il dataview)
    '                            'e dopo l'ordinamento del dataview carico la combo
    '                            DrTemp = Dt_Temp.NewRow
    '                            DrTemp.Item("Des") = x_Pro_Des
    '                            DrTemp.Item("Cod") = x_Pro_Cod
    '                            Dt_Temp.Rows.Add(DrTemp)

    '                            'Cmb.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

    '                            'NumTotale += 1

    '                            'End If
    '                        End If

    '                    Next

    '                End If

    '            Next 'giacenze

    '            If Dt_Temp.Rows.Count <> 0 Then

    '                'uso il dataview per ordinare
    '                Dim Dv As New DataView

    '                Dt_Temp.TableName = "prodotti"
    '                Dv.Table = Dt_Temp
    '                Dv.Sort = "Des ASC"

    '                For i = 0 To Dt_Temp.Rows.Count - 1

    '                    x_Pro_Cod = Dv.Item(i).Item("Cod")
    '                    x_Pro_Des = CStr(Dv.Item(i).Item("Des"))

    '                    If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
    '                        Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
    '                    End If

    '                Next

    '            End If

    '            Dt_Giacenze = Nothing
    '            Dt_Prodotti = Nothing


    '    End Select


    'End Sub



    'Public Shared Sub Prodotti(ByRef Controllo As ListControl, _
    '                        ByVal PrimaRiga_Flag As Boolean, _
    '                        ByVal PrimaRiga_Text As String, _
    '                        ByVal PrimaRiga_Value As String, _
    '                        ByVal Cau_Mov As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Integer, _
    '                        ByVal Id_Destinazione As Integer, _
    '                        ByVal Elem_Cod As Integer, _
    '                        ByVal Flag_Negativo As Boolean, _
    '                        ByVal RicercaTesto As String, _
    '                        ByVal Flag_VisualizzaProCod As Boolean, _
    '                        ByVal Flag_CaricaUdmCod As Boolean, _
    '                        ByVal Pro_Cod As Integer, _
    '                        ByVal Udm_Cod As Integer, _
    '                        ByVal DataFiltroFormulati As Date, _
    '                        ByVal PUA_RegolamentoCod As Integer, _
    '                        ByVal TipoRichiesto As Integer, _
    '                        ByVal Flag_LeggiGiacenze As Boolean, _
    '                        ByVal xFiltroAggiuntivo As String, _
    '                        ByVal xOrderBy As String, _
    '                        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                        )


    '    Dim Dt_Categorie As DataTable
    '    Dim Dt_Giacenze As DataTable
    '    Dim Dt_Prodotti As New DataTable
    '    Dim i, j As Integer
    '    Dim x_Pro_Cod As Integer
    '    Dim x_Pro_Des As String
    '    Dim NomeTabella As String
    '    Dim NomeCodice As String
    '    Dim NomeDescrizione As String
    '    Dim Value As String


    '    'Pulisco il controllo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag = True Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Dim objCat As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

    '    Select Case Cau_Mov

    '        Case CAU_CARICO

    '            'legge le categorie di magazzino
    '            Dt_Categorie = objCat.Leggi(Elem_Cod, _
    '                                         CAU_MAGAZZINO, _
    '                                          False, _
    '                                          "", "", _
    '                                          objParametri_server)

    '            'la categoria di magazzino è 1
    '            For i = 0 To Dt_Categorie.Rows.Count - 1
    '                NomeTabella = Dt_Categorie.Rows(i).Item("Tabella")
    '                NomeCodice = Dt_Categorie.Rows(i).Item("Tabella_Cod")
    '                NomeDescrizione = Dt_Categorie.Rows(i).Item("Tabella_Des")
    '            Next

    '            Dt_Categorie = Nothing

    '            Select Case Elem_Cod

    '                Case FERTILIZZANTI

    '                    Select Case PUA_RegolamentoCod
    '                        Case 0
    '                            TipoRichiesto = 0
    '                        Case 1
    '                            TipoRichiesto = 6
    '                        Case Else
    '                            TipoRichiesto = 7
    '                    End Select

    '                    Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

    '                    Dt_Prodotti = objFert.Leggi_Completa(0, _
    '                                                        RicercaTesto, _
    '                                                        TipoRichiesto, _
    '                                                        True, _
    '                                                        Piva, _
    '                                                        0, _
    '                                                        objParametri_server.FinestraTemporaleInizio, _
    '                                                        objParametri_server.FinestraTemporaleFine, _
    '                                                        "", "  " & NomeDescrizione & " ", _
    '                                                        objParametri_server, _
    '                                                        PUA_RegolamentoCod)

    '                Case FORMULATI


    '                    Try

    '                        '''''''''''''''''''''''''''''modifica webservice
    '                        Dim XML_Credenziali As System.Xml.XmlElement
    '                        Dim StrCredenziali As String = ""
    '                        Dim StrParametri As String = ""
    '                        Dim Parametri As String = ""
    '                        Dim strErr As String = ""
    '                        Dim LastFr_Des As String = ""

    '                        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
    '                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

    '                        Dim objAgroWebConfig As New AgroWebConfig
    '                        Dim XmlDoc As New System.Xml.XmlDocument

    '                        objWs.NewWS(ObjDownloadWs, _
    '                                        objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
    '                                        objParametri_utenti)
    '                        XmlDoc = New System.Xml.XmlDocument

    '                        Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

    '                        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                        StrCredenziali, _
    '                                                        AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo, _
    '                                                        objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo), _
    '                                                        HttpContext.Current.Session("ASG_ProgressivoGIAS"), _
    '                                                        HttpContext.Current.Session("ASG_SuperUser_Username").ToString, _
    '                                                        HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

    '                        XmlDoc.LoadXml(StrCredenziali)

    '                        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '                        objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                               StrParametri, _
    '                                                               "", _
    '                                                               strErr, _
    '                                                               DataFiltroFormulati, _
    '                                                               TipoRichiesto, _
    '                                                               RicercaTesto)

    '                        XML_Credenziali.InnerXml = StrParametri

    '                        Parametri = XmlDoc.OuterXml

    '                        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

    '                        Dim Ds As DataSet

    '                        Ds = ObjDownloadWs.Leggi_Formulato_Info_con_UdM_DS(Parametri, strErr)

    '                        Dt_Prodotti = Ds.Tables("formulati")

    '                        ObjDownloadWs.Dispose()


    '                    Catch ex As Exception

    '                        'se ci sono problemi al web service o non si è in linea
    '                        'carico i prodotti del metaschema locale (senza controlli date poichè non sono più in locale)
    '                        Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
    '                        Dt_Prodotti = objFito.LeggixDescrizione(RicercaTesto, _
    '                                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                               "", "", objParametri_server)

    '                        Flag_CaricaUdmCod = False


    '                    End Try


    '                    '''''''''''''''''''''' fine modifica webservice



    '                Case Else

    '                    Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella, _
    '                                                 NomeCodice, _
    '                                                 NomeDescrizione, _
    '                                                 RicercaTesto, _
    '                                                 0, "", " " & NomeDescrizione & " ", _
    '                                                 objParametri_server)
    '            End Select

    '            For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                Select Case Elem_Cod

    '                    Case FERTILIZZANTI
    '                        'value salvato nella modalità gestita dalla formprodotto
    '                        If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
    '                            x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
    '                        ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
    '                            x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
    '                        Else
    '                            'errore, qui non dovrebbe mai entrare
    '                            x_Pro_Cod = 0
    '                        End If

    '                        If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
    '                            If Flag_VisualizzaProCod = True Then
    '                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                             " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                            Else
    '                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                            End If
    '                            Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
    '                        End If

    '                    Case FORMULATI

    '                        x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                        If Flag_VisualizzaProCod = True Then
    '                            x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                         " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                        Else
    '                            x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                        End If

    '                        Value = x_Pro_Cod
    '                        If Flag_CaricaUdmCod = True Then
    '                            If Dt_Prodotti.Rows(j).Item("Udm_Cod_A") <> 0 Then
    '                                Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_A")
    '                            ElseIf Dt_Prodotti.Rows(j).Item("Udm_Cod_I") <> 0 Then
    '                                Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_I")
    '                            End If
    '                        End If

    '                        If IsNothing(Controllo.Items.FindByValue(Value)) Then
    '                            If Flag_VisualizzaProCod = True Then
    '                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                             " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                            Else
    '                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                            End If
    '                            Controllo.Items.Add(New ListItem(x_Pro_Des, Value))
    '                        End If

    '                    Case Else

    '                        x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                        If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
    '                            If Flag_VisualizzaProCod = True Then
    '                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                             " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                            Else
    '                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                            End If
    '                            Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
    '                        End If

    '                End Select



    '            Next

    '            Dt_Prodotti = Nothing


    '        Case CAU_SCARICO

    '            '#############################################
    '            '##############     SCARICO    ###############
    '            '#############################################

    '            Dim Dt_Temp As New DataTable
    '            Dim DrTemp As DataRow

    '            Dt_Temp.Columns.Add(New DataColumn("Des", GetType(String)))
    '            Dt_Temp.Columns.Add(New DataColumn("Cod", GetType(String)))


    '            'leggo i prodotti in magazzino

    '            Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
    '            Dim strFiltroFrCod As String = ""

    '            If Flag_LeggiGiacenze = True Then
    '                Dt_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltroFormulati, _
    '                                                                      Piva, _
    '                                                                      Sa_Cod, _
    '                                                                      Id_Destinazione, _
    '                                                                      Elem_Cod, _
    '                                                                      0, 0, 0, 0, 0, 0, _
    '                                                                      LOTTO_NONDEFINITO, _
    '                                                                      False, _
    '                                                                      "", "", "", "", "", "", "", "", "", "", "", "", objParametri_server)
    '                If Not Dt_Giacenze Is Nothing Then
    '                    For i = 0 To Dt_Giacenze.Rows.Count - 1
    '                        strFiltroFrCod &= Dt_Giacenze.Rows(i).Item("pro_cod") & ","
    '                    Next
    '                    If strFiltroFrCod <> "" Then
    '                        strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 1)
    '                    End If
    '                End If
    '            Else
    '                If xFiltroAggiuntivo <> "" Then
    '                    strFiltroFrCod = xFiltroAggiuntivo
    '                End If
    '            End If


    '            Select Case Elem_Cod

    '                Case FORMULATI




    '                    Try

    '                        '''''''''''''''''''''''''''''modifica webservice
    '                        Dim XML_Credenziali As System.Xml.XmlElement
    '                        Dim StrCredenziali As String = ""
    '                        Dim StrParametri As String = ""
    '                        Dim Parametri As String = ""
    '                        Dim strErr As String = ""
    '                        Dim LastFr_Des As String = ""

    '                        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
    '                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

    '                        Dim objAgroWebConfig As New AgroWebConfig
    '                        Dim XmlDoc As New System.Xml.XmlDocument

    '                        objWs.NewWS(ObjDownloadWs, _
    '                                        objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci, _
    '                                        objParametri_utenti)
    '                        XmlDoc = New System.Xml.XmlDocument

    '                        Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

    '                        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                        StrCredenziali, _
    '                                                        AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo, _
    '                                                        objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo), _
    '                                                        HttpContext.Current.Session("ASG_ProgressivoGIAS"), _
    '                                                        HttpContext.Current.Session("ASG_SuperUser_Username").ToString, _
    '                                                        HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

    '                        XmlDoc.LoadXml(StrCredenziali)

    '                        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '                        objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                               StrParametri, _
    '                                                               strFiltroFrCod, _
    '                                                               strErr, _
    '                                                               DataFiltroFormulati, _
    '                                                               TipoRichiesto, _
    '                                                               RicercaTesto)

    '                        XML_Credenziali.InnerXml = StrParametri

    '                        Parametri = XmlDoc.OuterXml

    '                        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

    '                        Dim Ds As DataSet

    '                        Ds = ObjDownloadWs.Leggi_Formulati_Info_DS(Parametri, strErr)

    '                        Dt_Prodotti = Ds.Tables("formulati")

    '                        ObjDownloadWs.Dispose()


    '                    Catch ex As Exception

    '                        'se ci sono problemi al web service o non si è in linea
    '                        'carico i prodotti del metaschema locale (senza controlli date poichè non sono più in locale)
    '                        Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
    '                        Dt_Prodotti = objFito.LeggixDescrizione(RicercaTesto, _
    '                                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                               "", "", objParametri_server)

    '                        Flag_CaricaUdmCod = False


    '                    End Try


    '                    '''''''''''''''''''''' fine modifica webservice

    '                    If Not IsNothing(Dt_Prodotti) Then

    '                        For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                            x_Pro_Cod = Dt_Prodotti.Rows(j).Item("fr_cod")

    '                            If Flag_VisualizzaProCod = True Then
    '                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item("fr_des")) & _
    '                                             " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                            Else
    '                                x_Pro_Des = Dt_Prodotti.Rows(j).Item("fr_des")
    '                            End If

    '                            Value = x_Pro_Cod
    '                            If Flag_CaricaUdmCod = True Then
    '                                If Dt_Prodotti.Rows(j).Item("Udm_Cod_A") <> 0 Then
    '                                    Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_A")
    '                                ElseIf Dt_Prodotti.Rows(j).Item("Udm_Cod_I") <> 0 Then
    '                                    Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_I")
    '                                End If
    '                            End If

    '                            'salvo in un dt di appoggio (da utilizzare per il dataview)
    '                            'e dopo l'ordinamento del dataview carico la combo
    '                            DrTemp = Dt_Temp.NewRow
    '                            DrTemp.Item("Des") = x_Pro_Des
    '                            DrTemp.Item("Cod") = Value
    '                            Dt_Temp.Rows.Add(DrTemp)

    '                        Next

    '                    End If

    '                Case Else

    '                    For i = 0 To Dt_Giacenze.Rows.Count - 1

    '                        NomeTabella = Dt_Giacenze.Rows(i).Item("Tabella")
    '                        NomeCodice = Dt_Giacenze.Rows(i).Item("Tabella_Cod")
    '                        NomeDescrizione = Dt_Giacenze.Rows(i).Item("Tabella_Des")

    '                        Select Case Elem_Cod

    '                            Case FERTILIZZANTI

    '                                Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

    '                                Dt_Prodotti = objFert.Leggi_Completa(Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                                                            RicercaTesto, _
    '                                                            0, _
    '                                                            True, _
    '                                                            Piva, _
    '                                                            Dt_Giacenze.Rows(i).Item("Mat_Cod"), _
    '                                                            objParametri_server.FinestraTemporaleInizio, _
    '                                                            objParametri_server.FinestraTemporaleFine, _
    '                                                            "", " " & NomeDescrizione & " ", _
    '                                                            objParametri_server, _
    '                                                            0)

    '                            Case Else

    '                                Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella, _
    '                                                             NomeCodice, _
    '                                                             NomeDescrizione, _
    '                                                             RicercaTesto, _
    '                                                             Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
    '                                                             "", " " & NomeDescrizione & " ", _
    '                                                             objParametri_server)

    '                        End Select


    '                        If Not IsNothing(Dt_Prodotti) Then

    '                            For j = 0 To Dt_Prodotti.Rows.Count - 1

    '                                Select Case Elem_Cod

    '                                    Case FERTILIZZANTI
    '                                        'value salvato nella modalità gestita dalla formprodotto
    '                                        If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
    '                                            x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
    '                                        ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
    '                                            x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
    '                                        Else
    '                                            'errore, qui non dovrebbe mai entrare
    '                                            x_Pro_Cod = 0
    '                                        End If

    '                                        If Flag_VisualizzaProCod = True Then
    '                                            x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                                        " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                                        Else
    '                                            x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                                        End If

    '                                        'salvo in un dt di appoggio (da utilizzare per il dataview)
    '                                        'e dopo l'ordinamento del dataview carico la combo
    '                                        DrTemp = Dt_Temp.NewRow
    '                                        DrTemp.Item("Des") = x_Pro_Des
    '                                        DrTemp.Item("Cod") = x_Pro_Cod
    '                                        Dt_Temp.Rows.Add(DrTemp)


    '                                    Case Else

    '                                        x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

    '                                        If Flag_VisualizzaProCod = True Then
    '                                            x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
    '                                                        " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
    '                                        Else
    '                                            x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
    '                                        End If

    '                                        'salvo in un dt di appoggio (da utilizzare per il dataview)
    '                                        'e dopo l'ordinamento del dataview carico la combo
    '                                        DrTemp = Dt_Temp.NewRow
    '                                        DrTemp.Item("Des") = x_Pro_Des
    '                                        DrTemp.Item("Cod") = x_Pro_Cod
    '                                        Dt_Temp.Rows.Add(DrTemp)

    '                                End Select

    '                            Next

    '                        End If

    '                    Next 'giacenze

    '            End Select

    '            If Dt_Temp.Rows.Count <> 0 Then

    '                'uso il dataview per ordinare
    '                Dim Dv As New DataView

    '                Dt_Temp.TableName = "prodotti"
    '                Dv.Table = Dt_Temp
    '                Dv.Sort = "Des ASC"

    '                For i = 0 To Dt_Temp.Rows.Count - 1

    '                    x_Pro_Cod = Dv.Item(i).Item("Cod")
    '                    x_Pro_Des = CStr(Dv.Item(i).Item("Des"))

    '                    If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
    '                        Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
    '                    End If

    '                Next

    '            End If

    '            Dt_Giacenze = Nothing
    '            Dt_Prodotti = Nothing


    '    End Select


    'End Sub



    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="Controllo"></param>
    '''' <param name="PrimaRiga_Flag"></param>
    '''' <param name="PrimaRiga_Text"></param>
    '''' <param name="PrimaRiga_Value"></param>
    '''' <param name="xFiltroAggiuntivo"></param>
    '''' <param name="xOrderBy"></param>
    '''' <param name="objParametri"></param>
    '''' <remarks>
    '''' </remarks>
    '''' <history>
    ''''' 	[magnani]	22/04/2011	Created, [vanni], 27/11/2017, spostato su AgronicaControlliGIS
    ''''' </history>
    ''''' -----------------------------------------------------------------------------
    'Public Shared Sub Gis_LayerElementiGrafici(ByRef Controllo As ListControl, _
    '                         ByVal PrimaRiga_Flag As Boolean, _
    '                         ByVal PrimaRiga_Text As String, _
    '                         ByVal PrimaRiga_Value As String, _
    '                         ByVal xFiltroAggiuntivo As String, _
    '                         ByVal xOrderBy As String, _
    '                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                         )

    '    Dim Dt As DataTable
    '    Dim i As Integer
    '    Dim obj As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

    '    'Pulisco il controllo
    '    Controllo.Items.Clear()

    '    If PrimaRiga_Flag = True Then
    '        Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
    '    End If

    '    Dt = obj.Leggi(objParametri.PivaSuperUser, objParametri.SuperUserUsername, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri)

    '    If Not IsNothing(Dt) Then

    '        If Dt.Rows.Count <> 0 Then

    '            For i = 0 To Dt.Rows.Count - 1

    '                Controllo.Items.Add(New ListItem(Dt.Rows(i).Item("LayerElementiGrafici_Des"), _
    '                                                 Dt.Rows(i).Item("LayerElementiGrafici_Cod")))


    '            Next

    '        End If

    '    End If

    'End Sub

End Class
