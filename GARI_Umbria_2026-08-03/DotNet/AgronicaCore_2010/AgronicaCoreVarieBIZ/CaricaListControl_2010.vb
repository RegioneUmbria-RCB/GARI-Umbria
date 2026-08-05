Imports System.Data.OleDb
Imports System.Web
Imports System.Web.UI.WebControls
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreGestioneRichieste

Public Class CaricaListControl_2010
    Inherits AgronicaCoreDataProvider.DataProvider
    '###############################################################################
    'OBSOLETA:
    'USARE ProdottiAnagrafica oppure ProdottiMagazzino
    'default:
    'Flag_VisualizzaProCod As Boolean = True
    Public Shared Sub Prodotti_Old(ByRef Controllo As ListControl,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal Cau_Mov As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Id_Destinazione As Integer,
                                ByVal Elem_Cod As Integer,
                                ByVal Flag_Negativo As Boolean,
                                ByVal RicercaTesto As String,
                                ByVal Flag_VisualizzaProCod As Boolean,
                                ByVal Pro_Cod As Integer,
                                ByVal Udm_Cod As Integer,
                                ByVal DataFiltroFormulati As Date,
                                ByVal PUA_RegolamentoCod As Integer,
                                   ByVal Stato_Cod As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )


        Dim Dt_Categorie As DataTable
        Dim Dt_Giacenze As DataTable
        Dim Dt_Prodotti, Dt_FertAzi As DataTable
        Dim i, j As Integer
        Dim x_Pro_Cod As Integer
        Dim x_Pro_Des As String
        Dim NomeTabella As String
        Dim NomeCodice As String
        Dim NomeDescrizione As String


        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
        Dim objCat As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

        Select Case Cau_Mov

            Case CAU_CARICO

                '#############################################
                '##############     CARICO    ################
                '#############################################

                'Leggo l'anagrafica prodotti

                'legge le categorie di magazzino
                Dt_Categorie = objCat.Leggi(Elem_Cod,
                                             CAU_MAGAZZINO,
                                              False,
                                              "", "",
                                              objParametri_server)

                Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R


                'la categoria di magazzino è 1
                For i = 0 To Dt_Categorie.Rows.Count - 1

                    NomeTabella = Dt_Categorie.Rows(i).Item("Tabella")
                    NomeCodice = Dt_Categorie.Rows(i).Item("Tabella_Cod")
                    NomeDescrizione = Dt_Categorie.Rows(i).Item("Tabella_Des")

                    Select Case Elem_Cod

                        Case FERTILIZZANTI

                            'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
                            'perchè oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
                            '(per i fertilizzanti aziendali)

                            'Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
                            '                                                   Piva, _
                            '                                                   0, _
                            '                                                   True, _
                            '                                                   RicercaTesto)

                            Dim TipoRichiesto As Integer = 0

                            Select Case PUA_RegolamentoCod
                                Case 0
                                    TipoRichiesto = 0
                                Case 1
                                    TipoRichiesto = 6
                                Case Else
                                    TipoRichiesto = 7
                            End Select

                            Dt_Prodotti = objFert.Leggi_Completa(0,
                                                                RicercaTesto,
                                                                TipoRichiesto,
                                                                True,
                                                                Piva,
                                                                0,
                                                                objParametri_server.FinestraTemporaleInizio,
                                                                objParametri_server.FinestraTemporaleFine,
                                                                "", "  " & NomeDescrizione & " ",
                                                                objParametri_server,
                                                                PUA_RegolamentoCod)

                        Case Else

                            Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella,
                                                                                 NomeCodice,
                                                                                 NomeDescrizione,
                                                                                 RicercaTesto,
                                                                                 0, "", " " & NomeDescrizione & " ",
                                                                                 objParametri_server)





                            '''''''''''''''''''''''''''''modifica webservice
                            Dim XML_Credenziali As System.Xml.XmlElement
                            Dim StrCredenziali As String = ""
                            Dim StrParametri As String = ""
                            Dim Parametri As String = ""
                            Dim strErr As String = ""
                            Dim LastFr_Des As String = ""


                            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                            Dim objAgroWebConfig As New AgroWebConfig
                            Dim XmlDoc As New System.Xml.XmlDocument


                            objWs.NewWS(ObjDownloadWs,
                                            objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                            objParametri_utenti)

                            Try



                                XmlDoc = New System.Xml.XmlDocument

                                Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                                Dim str_fr_cod As String = ""

                                Dim jj As Integer
                                For jj = 0 To Dt_Prodotti.Rows.Count - 1
                                    If str_fr_cod.Length = 0 Then
                                        str_fr_cod = Dt_Prodotti.Rows(jj).Item("fr_cod")
                                    Else
                                        str_fr_cod = str_fr_cod & "," & Dt_Prodotti.Rows(jj).Item("fr_cod")
                                    End If
                                Next
                                objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                StrCredenziali,
                                                                AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                                                objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                                                HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                                                HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                                                HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

                                XmlDoc.LoadXml(StrCredenziali)

                                XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                                'Dim grfi_Cod As Integer = 0
                                objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                       StrParametri,
                                                                       str_fr_cod,
                                                                       strErr,
                                                                        Stato_Cod:=Stato_Cod)

                                XML_Credenziali.InnerXml = StrParametri

                                Parametri = XmlDoc.OuterXml

                                Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)
                                Dim dt As DataTable
                                dt = ObjDownloadWs.Leggi_Formulati_Info_DT(Parametri, strErr)
                                For jj = 0 To Dt_Prodotti.Rows.Count - 1

                                    Dim dr As DataRow() = dt.Select("fr_cod = " & Dt_Prodotti.Rows(jj).Item("fr_cod"))

                                    Dt_Prodotti.Rows(jj).Item("data_reg") = dr(0).Item("data_reg")
                                    Dt_Prodotti.Rows(jj).Item("Data_fine_comm") = dr(0).Item("Data_fine_comm")
                                    Dt_Prodotti.Rows(jj).Item("Data_fine_usoscorte") = dr(0).Item("Data_fine_usoscorte")
                                    Dt_Prodotti.Rows(jj).Item("revocato") = dr(0).Item("revocato")
                                Next
                                ''verifico se è tutto ok

                                ObjDownloadWs.Dispose()

                            Catch ex As Exception

                            End Try


                            '''''''''''''''''''''' fine modifica webservice




                    End Select


                    If Not IsNothing(Dt_Prodotti) Then

                        Select Case Elem_Cod

                            Case FORMULATI

                                'nel caso dei formulati devo filtrare per evitare di visualizzare prodotti revocati, ecc
                                'Dt_Prodotti = objFito.Filtra_Formulati(Dt_Prodotti, DataFiltroFormulati, objParametri)
                                Dt_Prodotti = objFito.Filtra_Formulati_2(Dt_Prodotti, DataFiltroFormulati, objParametri_server)

                        End Select

                        'NumTotale = Dt_Prodotti.Rows.Count

                        For j = 0 To Dt_Prodotti.Rows.Count - 1

                            Select Case Elem_Cod

                                Case FERTILIZZANTI
                                    'value salvato nella modalità gestita dalla formprodotto
                                    If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
                                        x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
                                    ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
                                        x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
                                    Else
                                        'errore, qui non dovrebbe mai entrare
                                        x_Pro_Cod = 0
                                    End If

                                Case Else
                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

                            End Select

                            If Not IsDBNull(x_Pro_Cod) Then

                                If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then

                                    If Flag_VisualizzaProCod = True Then
                                        x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                                                     " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                                    Else
                                        x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                                    End If

                                    Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

                                End If
                            End If


                        Next
                    End If

                Next

                'Dt_Categorie.Dispose()

                'If Not IsNothing(Dt_Prodotti) Then
                '    Dt_Prodotti.Dispose()
                'End If

                Dt_Categorie = Nothing
                Dt_Prodotti = Nothing


            Case CAU_SCARICO

                '#############################################
                '##############     SCARICO    ###############
                '#############################################

                Dim Dt_Temp As New DataTable
                Dim DrTemp As DataRow

                Dt_Temp.Columns.Add(New DataColumn("Des", GetType(String)))
                Dt_Temp.Columns.Add(New DataColumn("Cod", GetType(Integer)))


                'leggo i prodotti in magazzino

                Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R


                'legge le giacenze con le categorie di magazzino
                'Dt_Giacenze = objGiacenze.Giacenze_Leggi(Piva, _
                '                                        Sa_Cod, _
                '                                        Id_Destinazione, _
                '                                        0, 0, _
                '                                        Elem_Cod, _
                '                                        0, 0, 0, 0, 0, 0, "", _
                '                                        0, 0, 0, _
                '                                         AGRODATAFINE, _
                '                                        "", "", _
                '                                        objParametri)

                Dt_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltroFormulati,
                                                                  Piva,
                                                                  Sa_Cod,
                                                                  Id_Destinazione,
                                                                  Elem_Cod,
                                                                  0, 0, 0, 0, 0, 0,
                                                                  LOTTO_NONDEFINITO,
                                                                  False,
                                                                  "", "", "", "", "", "", "", "", "", "", "", "",
                                                                  objParametri_server, objParametri_utenti)


                For i = 0 To Dt_Giacenze.Rows.Count - 1

                    NomeTabella = Dt_Giacenze.Rows(i).Item("Tabella")
                    NomeCodice = Dt_Giacenze.Rows(i).Item("Tabella_Cod")
                    NomeDescrizione = Dt_Giacenze.Rows(i).Item("Tabella_Des")

                    Select Case Elem_Cod

                        Case FERTILIZZANTI

                            'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
                            'perchè oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
                            '(per i fertilizzanti aziendali)

                            'Dt_Prodotti = NewCom_Fertilizzanti_Completa_Leggi(objServer, objSession, objPage, _
                            '                                                   Piva, _
                            '                                                   0, _
                            '                                                   True, _
                            '                                                   RicercaTesto, _
                            '                                                   Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
                            '                                                   Dt_Giacenze.Rows(i).Item("Mat_Cod"))


                            Dt_Prodotti = objFert.Leggi_Completa(Dt_Giacenze.Rows(i).Item("Pro_Cod"),
                                                        RicercaTesto,
                                                        0,
                                                        True,
                                                        Piva,
                                                        Dt_Giacenze.Rows(i).Item("Mat_Cod"),
                                                        objParametri_server.FinestraTemporaleInizio,
                                                        objParametri_server.FinestraTemporaleFine,
                                                        "", " " & NomeDescrizione & " ",
                                                        objParametri_server,
                                                        0)

                        Case Else

                            'Dt_Prodotti = NewCom_LeggiTabella_da_CategorieMagazzino(objServer, objSession, objPage, _
                            '                                                        NomeTabella, _
                            '                                                        NomeCodice, _
                            '                                                        NomeDescrizione, _
                            '                                                        RicercaTesto, _
                            '                                                        Dt_Giacenze.Rows(i).Item("Pro_Cod"), _
                            '                                                        "")

                            Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella,
                                                         NomeCodice,
                                                         NomeDescrizione,
                                                         RicercaTesto,
                                                         Dt_Giacenze.Rows(i).Item("Pro_Cod"),
                                                         "", " " & NomeDescrizione & " ",
                                                         objParametri_server)

                    End Select


                    If Not IsNothing(Dt_Prodotti) Then

                        For j = 0 To Dt_Prodotti.Rows.Count - 1

                            Select Case Elem_Cod

                                Case FERTILIZZANTI
                                    'value salvato nella modalità gestita dalla formprodotto
                                    If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
                                        x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
                                    ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
                                        x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
                                    Else
                                        'errore, qui non dovrebbe mai entrare
                                        x_Pro_Cod = 0
                                    End If

                                Case Else
                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

                            End Select

                            If Not IsDBNull(x_Pro_Cod) Then
                                'If IsNothing(Cmb.Items.FindByValue(x_Pro_Cod)) Then

                                If Flag_VisualizzaProCod = True Then
                                    x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                                                " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                                Else
                                    x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                                End If

                                'salvo in un dt di appoggio (da utilizzare per il dataview)
                                'e dopo l'ordinamento del dataview carico la combo
                                DrTemp = Dt_Temp.NewRow
                                DrTemp.Item("Des") = x_Pro_Des
                                DrTemp.Item("Cod") = x_Pro_Cod
                                Dt_Temp.Rows.Add(DrTemp)

                                'Cmb.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

                                'NumTotale += 1

                                'End If
                            End If

                        Next

                    End If

                Next 'giacenze

                If Dt_Temp.Rows.Count <> 0 Then

                    'uso il dataview per ordinare
                    Dim Dv As New DataView

                    Dt_Temp.TableName = "prodotti"
                    Dv.Table = Dt_Temp
                    Dv.Sort = "Des ASC"

                    For i = 0 To Dt_Temp.Rows.Count - 1

                        x_Pro_Cod = Dv.Item(i).Item("Cod")
                        x_Pro_Des = CStr(Dv.Item(i).Item("Des"))

                        If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
                            Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
                        End If

                    Next

                End If

                Dt_Giacenze = Nothing
                Dt_Prodotti = Nothing


        End Select


    End Sub

    '##################################################################
    'xOrderBy non è usato
    'xFiltroAggiuntivo viene usata nella strFiltroFrCod 
    Public Sub Prodotti(ByRef Controllo As ListControl,
                        ByVal PrimaRiga_Flag As Boolean,
                        ByVal PrimaRiga_Text As String,
                        ByVal PrimaRiga_Value As String,
                        ByVal Cau_Mov As String,
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Id_Destinazione As Integer,
                        ByVal Elem_Cod As Integer,
                        ByVal Flag_Negativo As Boolean,
                        ByVal RicercaTesto As String,
                        ByVal Flag_VisualizzaProCod As Boolean,
                        ByVal Flag_CaricaUdmCod As Boolean,
                        ByVal Pro_Cod As Integer,
                        ByVal Udm_Cod As Integer,
                        ByVal DataFiltroFormulati As Date,
                        ByVal PUA_RegolamentoCod As Integer,
                        ByVal TipoRichiesto As Integer,
                        ByVal Flag_LeggiGiacenze As Boolean,
                        ByVal Flag_FiltraRevocati As Boolean,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        Optional ByRef DT_Risultato As DataTable = Nothing,
                        Optional ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                        Optional ByVal RegolamentoCod_Operazioni As Integer = 0,
                        Optional ByVal Tipo_PuaRegolamento As Integer = 0,
                        Optional ByVal Flag_IncludiNPK_Desc As Boolean = False,
                        Optional ByVal Flag_IncludiClassificazione As Boolean = False,
                        Optional ByVal Flag_QtaNoZero As Boolean = False,
                        Optional ByVal xFiltroAggiuntivoGiasAPP As String = "",
                        Optional ByVal Stato_Cod As String = "IT",
                        Optional ByVal Flag_QtaMaggioreZero As Boolean = False,
                        Optional ByVal leggiGiacenzeConAgroDataFine As Boolean = True,
                        Optional ByVal Trap_Cod As Integer = 0,
                        Optional ByRef NomeCodice As String = "",
                        Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing,
                        Optional ByVal inibisciVisibilitaGruppiMerce As Boolean = False,
                        Optional ByVal Flag_Filtra_MateriePrime_Per_Piva As Boolean? = False,
                        Optional ByVal Flag_Filtra_MateriePrime_Pubblici As Boolean? = False,
                        Optional ByVal Flag_CodArticolo_In_Descrizione As Boolean? = False,
                        Optional ByVal Veg_Cod As Integer = 0,
                        Optional ByVal TipiRichiestiFormulati As String = "")

        ' INIZIO Modifica necessaria per richiamo da CoreWS senza Session
        Dim ASG_ProgressivoGIAS As Integer = 0
        Dim ASG_SuperUser_Username As String = ""
        Dim ASG_SuperUser_Password As String = ""

        If Not HttpContext.Current.Session Is Nothing Then
            ASG_ProgressivoGIAS = HttpContext.Current.Session("ASG_ProgressivoGIAS")
            ASG_SuperUser_Username = HttpContext.Current.Session("ASG_SuperUser_Username").ToString
            ASG_SuperUser_Password = HttpContext.Current.Session("ASG_SuperUser_Password").ToString
        Else
            Dim DT As DataTable
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            DT = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_server.SuperUserUsername, "",
                                                    Date.Now,
                                                    CType(Now.Hour, Short),
                                                    5, objParametri_utenti)
            If DT.Rows.Count > 0 Then
                ASG_ProgressivoGIAS = DT.Rows(0).Item("ProgressivoGIAS")
                ASG_SuperUser_Username = objParametri_server.SuperUserUsername
                ASG_SuperUser_Password = DT.Rows(0).Item("Password")
            End If
        End If
        ' FINE Modifica necessaria per richiamo da CoreWS senza Session

        Dim Dt_Categorie As DataTable
        Dim Dt_Giacenze As DataTable
        Dim Dt_Giacenze_Tot As DataTable
        Dim DrGiacenze() As DataRow
        Dim DrGiacenze_Tot() As DataRow
        Dim strGiacenza As String
        Dim Dt_Prodotti As New DataTable
        Dim i, j As Integer
        Dim x_Pro_Cod As Integer
        Dim x_Pro_Cod_Stringa As String
        Dim x_Pro_Des As String

        Dim NomeTabella As String = ""
        'NomeCodice è stato spostato come parametro
        Dim NomeDescrizione As String = ""

        Dim Value As String

        'Pulisco il controllo
        Controllo.Items.Clear()

        If PrimaRiga_Flag = True Then
            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        End If

        If Elem_Cod = FORMULATI AndAlso String.IsNullOrWhiteSpace(TipiRichiestiFormulati) Then
            TipiRichiestiFormulati = CStr(TipoRichiesto)
        End If

        Dim objCat As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

        Select Case True

            Case Cau_Mov = CAU_CARICO Or Elem_Cod = SERVIZI

                If Elem_Cod = SERVIZI Then

                    NomeTabella = "Categorie"
                    NomeCodice = "COD"
                    NomeDescrizione = "DESCR"

                Else

                    'legge le categorie di magazzino
                    Dt_Categorie = objCat.Leggi(Elem_Cod,
                                                CAU_MAGAZZINO,
                                                False,
                                                "",
                                                "",
                                                objParametri_server)

                    'la categoria di magazzino è 1
                    For i = 0 To Dt_Categorie.Rows.Count - 1
                        NomeTabella = Dt_Categorie.Rows(i).Item("Tabella")
                        NomeCodice = Dt_Categorie.Rows(i).Item("Tabella_Cod")
                        NomeDescrizione = Dt_Categorie.Rows(i).Item("Tabella_Des")
                    Next

                End If

                Dt_Categorie = Nothing

                Select Case Elem_Cod

                    Case FERTILIZZANTI
                        '28/06/2018: sostituita chiamata al MS con chiamata al WS

                        'questo è quello che faceva prima:
                        'Select Case PUA_RegolamentoCod
                        '    Case 0
                        '        TipoRichiesto = 0
                        '    Case 1
                        '        TipoRichiesto = 6
                        '    Case Else
                        '        TipoRichiesto = 7
                        'End Select

                        'questo è quello che aveva suggerito di fare la fede:
                        'Select Case PUA_RegolamentoCod
                        '    Case 0, -2
                        '        TipoRichiesto = 0
                        '    Case Else
                        '        TipoRichiesto = 2
                        'End Select

                        'questo è quello che fa la pagina della concimazione
                        If Tipo_PuaRegolamento = 2 Then
                            TipoRichiesto = PUA_RegolamentoCod
                        End If

                        Dt_Prodotti = New DataTable
                        'Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
                        Dt_Prodotti.Columns.Add(New DataColumn("Fer_Cod", GetType(Integer)))
                        Dt_Prodotti.Columns.Add(New DataColumn("Fer_Des", GetType(String)))
                        Dt_Prodotti.Columns.Add(New DataColumn("N", GetType(Decimal)))
                        Dt_Prodotti.Columns.Add(New DataColumn("P2O5", GetType(Decimal)))
                        Dt_Prodotti.Columns.Add(New DataColumn("K2O", GetType(Decimal)))
                        Dt_Prodotti.Columns.Add(New DataColumn("MgO", GetType(Decimal)))
                        Dt_Prodotti.Columns.Add(New DataColumn("Cu", GetType(Decimal)))
                        Dt_Prodotti.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))

                        Dim dr As DataRow

                        'Dt_Prodotti = objFert.Leggi_Completa(0,
                        '                                    RicercaTesto,
                        '                                    TipoRichiesto,
                        '                                    True,
                        '                                    Piva,
                        '                                    0,
                        '                                    objParametri_server.FinestraTemporaleInizio,
                        '                                    objParametri_server.FinestraTemporaleFine,
                        '                                    "", "  " & NomeDescrizione & " ",
                        '                                    objParametri_server,
                        '                                    PUA_RegolamentoCod)

                        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
                        objParametriIngresso.Codice = Pro_Cod
                        objParametriIngresso.Descrizione = RicercaTesto
                        objParametriIngresso.DataInizio = AGRODATAINIZIO
                        objParametriIngresso.DataFine = AGRODATAFINE
                        objParametriIngresso.Tipo = TipoRichiesto
                        objParametriIngresso.IncludiApporti = True
                        objParametriIngresso.IncludiTipologia = True
                        objParametriIngresso.Regolamento = PUA_RegolamentoCod
                        objParametriIngresso.strFiltro = ""
                        objParametriIngresso.Stato_Cod = Stato_Cod

                        '(27/09/2018 fede)
                        Dim objAgroWebConfig As AgroWebConfig
                        If Not HttpContext.Current.Session Is Nothing Then
                            objAgroWebConfig = New AgroWebConfig
                        Else
                            objAgroWebConfig = New AgroWebConfig(objParametri_Super_Server, objParametri_server, True)
                        End If

                        objParametriIngresso.Url = objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti & "/Fertilizzanti"

                        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output
                        Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
                        objParametriUscita = objFert_WS.Fertilizzanti(objParametriIngresso)

                        Dim strTipologie As String = ""

                        For i = 0 To objParametriUscita.ListaFertilizzanti.Count - 1

                            If TipoRichiesto >= 6 Then
                                x_Pro_Des = CStr(objParametriUscita.ListaFertilizzanti(i).TipoFertilizzanteDescrizione) & " - " & objParametriUscita.ListaFertilizzanti(i).Descrizione
                            Else
                                x_Pro_Des = objParametriUscita.ListaFertilizzanti(i).Descrizione
                            End If

                            x_Pro_Cod = objParametriUscita.ListaFertilizzanti(i).Codice

                            If Flag_IncludiNPK_Desc = True Then
                                '(16/06/2017 fede) sostituito Mg con Cu
                                x_Pro_Des += " (" + Format(objParametriUscita.ListaFertilizzanti(i).N, "0.##") + "-" +
                                                                     Format(objParametriUscita.ListaFertilizzanti(i).P2O5, "0.##") + "-" +
                                                                     Format(objParametriUscita.ListaFertilizzanti(i).K2O, "0.##") + "-" +
                                                                     Format(objParametriUscita.ListaFertilizzanti(i).Cu, "0.##") + ")"

                                'Format(objParametriUscita.ListaFertilizzanti(i).MgO, "0.##") + "-" +

                                'x_Pro_Cod = x_Pro_Cod & "/" +
                                '              CStr(objParametriUscita.ListaFertilizzanti(i).N) + "-" +
                                '              CStr(objParametriUscita.ListaFertilizzanti(i).P2O5) + "-" +
                                '              CStr(objParametriUscita.ListaFertilizzanti(i).K2O) + "-" +
                                '              CStr(objParametriUscita.ListaFertilizzanti(i).MgO) + "-" +
                                '              CStr(objParametriUscita.ListaFertilizzanti(i).Cu)

                            End If

                            strTipologie = ""

                            If Flag_IncludiClassificazione = True Then
                                If TipoRichiesto < 6 Then
                                    For j = 0 To objParametriUscita.ListaFertilizzanti(i).ListaTipologie.Count - 1
                                        strTipologie &= objParametriUscita.ListaFertilizzanti(i).ListaTipologie(j).Descrizione & ","
                                    Next
                                    If strTipologie <> "" Then
                                        x_Pro_Des += " --- [" + Left(strTipologie, strTipologie.Length - 1) + "]"
                                    End If
                                End If
                            End If

                            If Flag_VisualizzaProCod = True Then
                                x_Pro_Des = x_Pro_Des & " (Cod." & CStr(Math.Abs(x_Pro_Cod)) & ")"
                            End If

                            Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))

                            'Creo una nuova riga
                            dr = Dt_Prodotti.NewRow
                            dr.Item("Fer_Cod") = x_Pro_Cod
                            dr.Item("Fer_Des") = x_Pro_Des
                            dr.Item("N") = objParametriUscita.ListaFertilizzanti(i).N
                            dr.Item("P2O5") = objParametriUscita.ListaFertilizzanti(i).P2O5
                            dr.Item("K2O") = objParametriUscita.ListaFertilizzanti(i).K2O
                            dr.Item("MgO") = objParametriUscita.ListaFertilizzanti(i).MgO
                            dr.Item("Cu") = objParametriUscita.ListaFertilizzanti(i).Cu
                            dr.Item("Udm_Cod") = objParametriUscita.ListaFertilizzanti(i).Udm_Cod
                            Dt_Prodotti.Rows.Add(dr)

                        Next

                        '-------------------------------------------------------------------------------------------

                    Case FORMULATI

                        Try

                            '''''''''''''''''''''''''''''modifica webservice
                            Dim XML_Credenziali As System.Xml.XmlElement
                            Dim StrCredenziali As String = ""
                            Dim StrParametri As String = ""
                            Dim Parametri As String = ""
                            Dim strErr As String = ""
                            Dim LastFr_Des As String = ""

                            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                            Dim objAgroWebConfig As AgroWebConfig
                            If Not HttpContext.Current.Session Is Nothing Then
                                objAgroWebConfig = New AgroWebConfig
                            Else
                                objAgroWebConfig = New AgroWebConfig(objParametri_Super_Server, objParametri_server, True)
                            End If

                            Dim XmlDoc As New System.Xml.XmlDocument

                            objWs.NewWS(ObjDownloadWs,
                                            objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                            objParametri_utenti)
                            XmlDoc = New System.Xml.XmlDocument

                            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs
                            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                            StrCredenziali,
                                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                                            ASG_ProgressivoGIAS,
                                                            ASG_SuperUser_Username,
                                                            ASG_SuperUser_Password)

                            XmlDoc.LoadXml(StrCredenziali)

                            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                            Dim strFr_Cod As String = ""
                            If Pro_Cod <> 0 Then
                                strFr_Cod = Pro_Cod.ToString
                            End If

                            objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                   StrParametri,
                                                                   strFr_Cod,
                                                                   strErr,
                                                                   DataFiltroFormulati,
                                                                   TipiRichiestiFormulati,
                                                                   RicercaTesto, Stato_Cod, Veg_Cod)

                            XML_Credenziali.InnerXml = StrParametri

                            Parametri = XmlDoc.OuterXml

                            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                            Dim Ds As DataSet

                            Ds = ObjDownloadWs.Leggi_Formulato_Info_con_UdM_DS(Parametri, strErr)

                            Dt_Prodotti = Ds.Tables("formulati")

                            ObjDownloadWs.Dispose()

                        Catch ex As Exception

                            'se ci sono problemi al web service o non si è in linea
                            'carico i prodotti del metaschema locale (senza controlli date poichè non sono più in locale)
                            Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
                            Dt_Prodotti = objFito.LeggixDescrizione(RicercaTesto.ToOrigin(objParametri_server),
                                                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                   "", "", objParametri_server)

                            Flag_CaricaUdmCod = False

                        End Try

                        '''''''''''''''''''''' fine modifica webservice

                    Case SERVIZI

                        Dim objLC As New AgronicaCoreUtility.CaricaListControl

                        Dim filtroAggiuntivo = ""
                        If Not String.IsNullOrEmpty(RicercaTesto) Then
                            filtroAggiuntivo = String.Format(" DESCR LIKE '%{0}%' ", Agro_SQL_SaveText(RicercaTesto, False))
                        End If
                        Dt_Prodotti = objLC.LeggiServizi("",
                                                         filtroAggiuntivo,
                                                         "",
                                                         objParametri_server)

                    Case Else

                        '28/02/2019 patch nel caso dei carburanti:
                        'essendo stati spostati sulla tabella materie_prime,
                        'veniva caricata tutta la tabella indipendentemente dall'elem_cod
                        'venivano quindi caricate materie prime che non c'entravano niente
                        'oltre a esplodere il caricamento su alcuni archivi

                        Dim filtroMP As String = ""
                        If NomeTabella.ToLower = "materie_prime" Then
                            filtroMP = " Elem_cod = " & CStr(Elem_Cod)
                        End If

                        If Elem_Cod = INNESCHI AndAlso Trap_Cod <> 0 Then
                            Dim objLC As New AgronicaCoreUtility.CaricaListControl
                            Dt_Prodotti = objLC.Leggi_AvversitaxTrappole(0,
                                                                         Trap_Cod,
                                                                         0,
                                                                         "",
                                                                         "",
                                                                         objParametri_server)
                        Else

                            Dim xFiltroMateriePrime As String = ""

                            If NomeTabella = "Materie_Prime" Then

                                If Not IsNothing(Flag_Filtra_MateriePrime_Per_Piva) AndAlso Flag_Filtra_MateriePrime_Per_Piva Then

                                    xFiltroMateriePrime = " (Piva = '" & Piva & "' "
                                    If Not IsNothing(Flag_Filtra_MateriePrime_Pubblici) AndAlso Flag_Filtra_MateriePrime_Pubblici Then
                                        xFiltroMateriePrime &= " OR Sa_Cod = -1 "
                                    End If
                                    xFiltroMateriePrime &= ") "
                                End If

                            End If

                            If Not String.IsNullOrEmpty(xFiltroMateriePrime) Then
                                If String.IsNullOrEmpty(filtroMP) Then
                                    filtroMP = xFiltroMateriePrime
                                Else
                                    filtroMP += " AND " + xFiltroMateriePrime
                                End If
                            End If

                            Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella,
                                                                                    NomeCodice,
                                                                                    NomeDescrizione,
                                                                                    RicercaTesto,
                                                                                    Pro_Cod,
                                                                                    filtroMP,
                                                                                    " " & NomeDescrizione & " ",
                                                                                    objParametri_server)
                        End If

                End Select

                For j = 0 To Dt_Prodotti.Rows.Count - 1

                    Select Case Elem_Cod

                        Case FERTILIZZANTI

                            '28/06/2018: sostituita chiamata al MS con chiamata al WS
                            'la combo è già stata caricata sopra

                            ''value salvato nella modalità gestita dalla formprodotto
                            'If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
                            '    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
                            'ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
                            '    x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
                            'Else
                            '    'errore, qui non dovrebbe mai entrare
                            '    x_Pro_Cod = 0
                            'End If

                            'If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
                            '    If Flag_VisualizzaProCod = True Then
                            '        x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                            '                     " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                            '    Else
                            '        x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                            '    End If
                            '    Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
                            'End If

                        Case FORMULATI

                            x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

                            If Flag_VisualizzaProCod = True Then
                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                                             " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                            Else
                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                            End If

                            Value = x_Pro_Cod
                            If Flag_CaricaUdmCod = True Then
                                If Dt_Prodotti.Rows(j).Item("Udm_Cod_A") <> 0 Then
                                    Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_A")
                                ElseIf Dt_Prodotti.Rows(j).Item("Udm_Cod_I") <> 0 Then
                                    Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_I")
                                End If
                            End If

                            If Flag_VisualizzaProCod = True Then
                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                                                 " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"

                                Dt_Prodotti.Rows(j).Item(NomeDescrizione) = x_Pro_Des
                            Else
                                x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                            End If

                            If IsNothing(Controllo.Items.FindByValue(Value)) Then
                                Controllo.Items.Add(New ListItem(x_Pro_Des, Value))
                            End If

                        Case FARMACI
                            x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)
                            Dim x_Aic As String = Dt_Prodotti.Rows(j).Item("AIC")
                            Dim x_Conf As String = Dt_Prodotti.Rows(j).Item("Confezione")

                            If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
                                x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & " (" & x_Aic & ") - " & x_Conf
                                Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
                            End If

                        Case Else

                            If Elem_Cod = SERVIZI Then
                                x_Pro_Cod_Stringa = Dt_Prodotti.Rows(j).Item(NomeCodice)
                                'elimino il primo carattere e trasformo in numerico
                                x_Pro_Cod = CInt(Right(x_Pro_Cod_Stringa, Len(x_Pro_Cod_Stringa) - 1))
                            Else
                                x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)
                            End If


                            If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
                                If Flag_VisualizzaProCod = True Then
                                    x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                                Else
                                    x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                                End If
                                If Not IsNothing(Flag_CodArticolo_In_Descrizione) AndAlso Flag_CodArticolo_In_Descrizione Then
                                    If Dt_Prodotti.Columns.Contains("Cod_Articolo") AndAlso Not IsDBNull(Dt_Prodotti.Rows(j).Item("Cod_Articolo")) Then
                                        Dim cod_articolo = Dt_Prodotti.Rows(j).Item("Cod_Articolo").ToString.Trim
                                        If Not String.IsNullOrEmpty(cod_articolo) AndAlso Not String.IsNullOrWhiteSpace(cod_articolo) Then
                                            x_Pro_Des = x_Pro_Des & " (" + cod_articolo & ") "
                                        End If
                                    End If
                                End If
                                Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
                            End If

                    End Select

                Next

                If DT_Risultato Is Nothing Then
                    DT_Risultato = Dt_Prodotti.Copy()
                ElseIf DT_Risultato.Rows.Count = 0 Then
                    DT_Risultato = Dt_Prodotti.Copy()
                Else
                    For Each r In Dt_Prodotti.Rows
                        DT_Risultato.Rows.Add(r)
                    Next
                End If

                Dt_Prodotti = Nothing

            Case Cau_Mov = CAU_SCARICO

                '#############################################
                '##############     SCARICO    ###############
                '#############################################

                Dim Dt_Temp As New DataTable
                Dim DrTemp As DataRow

                Dt_Temp.Columns.Add(New DataColumn("Des", GetType(String)))
                Dt_Temp.Columns.Add(New DataColumn("Cod", GetType(String)))

                'leggo i prodotti in magazzino

                Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
                Dim strFiltroFrCod As String = ""

                If Flag_LeggiGiacenze = True Then

                    Dim xFiltroAggiuntivo_1 As String = ""
                    Dim xFiltroAggiuntivo_3 As String = ""
                    Dim xFiltroAggiuntivo_4 As String = ""
                    Dim xFiltroAggiuntivo_5 As String = ""
                    Dim xFiltroAggiuntivo_6 As String = ""
                    Dim xFiltroAggiuntivo_8 As String = ""
                    Dim xFiltroAggiuntivo_16 As String = ""

                    '11/07/2018 ottimizzazione
                    If RicercaTesto <> "" Then
                        Select Case Elem_Cod
                            Case COADIUVANTI
                                xFiltroAggiuntivo_1 = " AND Coad_Des like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                            Case FERTILIZZANTI
                                xFiltroAggiuntivo_3 = " AND Fer_Des like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                            Case FORMULATI
                                xFiltroAggiuntivo_4 = " AND fr_des like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                            Case INNESCHI
                                xFiltroAggiuntivo_5 = " AND Av_des_Vol like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                            Case INSETTI
                                xFiltroAggiuntivo_6 = " AND Ins_Des like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                            Case TRAPPOLE
                                xFiltroAggiuntivo_8 = " AND Trap_Des like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                            Case FARMACI
                                xFiltroAggiuntivo_16 = " AND Denominazione like '%" & Agro_SQL_SaveText(RicercaTesto, False) & "%'"
                        End Select
                    End If

                    Dt_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltroFormulati,
                                                                      Piva,
                                                                      Sa_Cod,
                                                                      Id_Destinazione,
                                                                      Elem_Cod,
                                                                      0, 0, 0, 0, 0, 0,
                                                                      LOTTO_NONDEFINITO,
                                                                      Flag_QtaNoZero,
                                                                      xFiltroAggiuntivoGiasAPP, xFiltroAggiuntivo_1, "", xFiltroAggiuntivo_3, xFiltroAggiuntivo_4, xFiltroAggiuntivo_5, xFiltroAggiuntivo_6, "", xFiltroAggiuntivo_8, "", "", "",
                                                                      objParametri_server, objParametri_utenti, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                                                                      gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria, inibisciVisibilitaGruppiMerce:=inibisciVisibilitaGruppiMerce,
                                                                      xFiltroAggiuntivo_16:=xFiltroAggiuntivo_16)

                    If leggiGiacenzeConAgroDataFine Then
                        Dt_Giacenze_Tot = objGiacenze.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                                              Piva,
                                                                              Sa_Cod,
                                                                              Id_Destinazione,
                                                                              Elem_Cod,
                                                                              0, 0, 0, 0, 0, 0,
                                                                              LOTTO_NONDEFINITO,
                                                                              Flag_QtaNoZero,
                                                                              xFiltroAggiuntivoGiasAPP, xFiltroAggiuntivo_1, "", xFiltroAggiuntivo_3, xFiltroAggiuntivo_4, xFiltroAggiuntivo_5, xFiltroAggiuntivo_6, "", xFiltroAggiuntivo_8, "", "", "",
                                                                              objParametri_server, objParametri_utenti, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                                                                              gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria, inibisciVisibilitaGruppiMerce:=inibisciVisibilitaGruppiMerce,
                                                                              xFiltroAggiuntivo_16:=xFiltroAggiuntivo_16)
                    End If

                    If Not Dt_Giacenze Is Nothing Then

                        If Dt_Giacenze.Rows.Count > 0 Then
                            NomeTabella = Dt_Giacenze.Rows(0).Item("Tabella")
                            NomeCodice = Dt_Giacenze.Rows(0).Item("Tabella_Cod")
                            NomeDescrizione = Dt_Giacenze.Rows(0).Item("Tabella_Des")
                        End If

                        For i = 0 To Dt_Giacenze.Rows.Count - 1
                            strFiltroFrCod &= Dt_Giacenze.Rows(i).Item("pro_cod") & ","
                        Next
                        If strFiltroFrCod <> "" Then
                            strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 1)
                        End If
                    End If

                Else

                    If xFiltroAggiuntivo <> "" Then
                        strFiltroFrCod = xFiltroAggiuntivo
                    End If

                End If

                ' Senza questa vengono passati tutti i prodotti anche se non hanno giacenza
                If Not String.IsNullOrWhiteSpace(strFiltroFrCod) Then

                    Select Case Elem_Cod

                        Case FORMULATI

                            Try

                                '''''''''''''''''''''''''''''modifica webservice
                                Dim XML_Credenziali As System.Xml.XmlElement
                                Dim StrCredenziali As String = ""
                                Dim StrParametri As String = ""
                                Dim Parametri As String = ""
                                Dim strErr As String = ""
                                Dim LastFr_Des As String = ""

                                Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                                Dim objAgroWebConfig As AgroWebConfig
                                If Not HttpContext.Current.Session Is Nothing Then
                                    objAgroWebConfig = New AgroWebConfig
                                Else
                                    objAgroWebConfig = New AgroWebConfig(objParametri_Super_Server, objParametri_server, True)
                                End If
                                Dim XmlDoc As New System.Xml.XmlDocument

                                objWs.NewWS(ObjDownloadWs,
                                            objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                            objParametri_utenti)
                                XmlDoc = New System.Xml.XmlDocument

                                Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                                objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                            StrCredenziali,
                                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                                            ASG_ProgressivoGIAS,
                                                            ASG_SuperUser_Username,
                                                            ASG_SuperUser_Password)

                                XmlDoc.LoadXml(StrCredenziali)

                                XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")


                                If Flag_FiltraRevocati = True Then
                                    objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                       StrParametri,
                                       strFiltroFrCod,
                                       strErr,
                                       DataFiltroFormulati,
                                       TipiRichiestiFormulati,
                                       RicercaTesto, Stato_Cod)
                                Else
                                    objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                       StrParametri,
                                       strFiltroFrCod,
                                       strErr,
                                       "",
                                       TipiRichiestiFormulati,
                                       RicercaTesto, Stato_Cod)
                                End If

                                XML_Credenziali.InnerXml = StrParametri

                                Parametri = XmlDoc.OuterXml

                                Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                                Dim Ds As DataSet

                                Ds = ObjDownloadWs.Leggi_Formulati_Info_DS(Parametri, strErr)

                                Dt_Prodotti = Ds.Tables("formulati")

                                ObjDownloadWs.Dispose()

                            Catch ex As Exception

                                Dim filtroAggiuntivo = ""
                                ' fix nel caso va in errore chiamata da GiasAPP
                                If Not String.IsNullOrEmpty(xFiltroAggiuntivoGiasAPP) Then
                                    If Not String.IsNullOrEmpty(strFiltroFrCod) Then
                                        filtroAggiuntivo = " Fr_Cod IN (" & strFiltroFrCod & ") "
                                    End If
                                End If

                                'se ci sono problemi al web service o non si è in linea
                                'carico i prodotti del metaschema locale (senza controlli date poichè non sono più in locale)
                                Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
                                Dt_Prodotti = objFito.LeggixDescrizione(RicercaTesto,
                                                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                   filtroAggiuntivo, "", objParametri_server)

                                Flag_CaricaUdmCod = False

                            End Try

                            '''''''''''''''''''''' fine modifica webservice


                            If Not IsNothing(Dt_Prodotti) Then

                                'Gruppi Merce
                                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                                    Dt_Prodotti.Columns.Add(New DataColumn("Id_Gruppo_Merce", GetType(Integer)))
                                    Dt_Prodotti.Columns.Add(New DataColumn("Des_Gruppo_Merce", GetType(String)))
                                End If

                                For j = 0 To Dt_Prodotti.Rows.Count - 1

                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("fr_cod")

                                    If Flag_VisualizzaProCod = True Then
                                        x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item("fr_des")) &
                                                 " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                                    Else
                                        x_Pro_Des = Dt_Prodotti.Rows(j).Item("fr_des")
                                    End If

                                    Dt_Prodotti.Rows(j).Item("fr_des") = x_Pro_Des

                                    Value = x_Pro_Cod
                                    If Flag_CaricaUdmCod = True Then
                                        If Dt_Prodotti.Rows(j).Item("Udm_Cod_A") <> 0 Then
                                            Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_A")
                                        ElseIf Dt_Prodotti.Rows(j).Item("Udm_Cod_I") <> 0 Then
                                            Value = x_Pro_Cod & "|" & Dt_Prodotti.Rows(j).Item("Udm_Cod_I")
                                        End If
                                    End If

                                    'salvo in un dt di appoggio (da utilizzare per il dataview)
                                    'e dopo l'ordinamento del dataview carico la combo

                                    strGiacenza = ""
                                    If Not Dt_Giacenze Is Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                        DrGiacenze = Dt_Giacenze.Select("pro_cod=" & x_Pro_Cod)
                                        If Not DrGiacenze Is Nothing AndAlso DrGiacenze.Length > 0 Then
                                            strGiacenza = " --- GIACENZA: alla data " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")

                                            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                                                Dt_Prodotti.Rows(j).Item("Id_Gruppo_Merce") = DrGiacenze(0).Item("Id_Gruppo_Merce")
                                                Dt_Prodotti.Rows(j).Item("Des_Gruppo_Merce") = DrGiacenze(0).Item("Des_Gruppo_Merce")
                                            End If

                                        End If
                                    End If
                                    If Not Dt_Giacenze_Tot Is Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                        DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & x_Pro_Cod)
                                        If Not DrGiacenze_Tot Is Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                            strGiacenza &= " totale " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                        End If
                                    End If

                                    DrTemp = Dt_Temp.NewRow
                                    DrTemp.Item("Des") = x_Pro_Des + strGiacenza
                                    DrTemp.Item("Cod") = Value
                                    Dt_Temp.Rows.Add(DrTemp)

                                Next

                            End If

                            If DT_Risultato Is Nothing Then
                                DT_Risultato = Dt_Prodotti.Copy()
                            ElseIf DT_Risultato.Rows.Count = 0 Then
                                DT_Risultato = Dt_Prodotti.Copy()
                            Else
                                For Each r In Dt_Prodotti.Rows
                                    DT_Risultato.Rows.Add(r)
                                Next
                            End If

                        Case Else

                            For i = 0 To Dt_Giacenze.Rows.Count - 1

                                Select Case Elem_Cod

                                    Case FERTILIZZANTI

                                        Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

                                        Dt_Prodotti = objFert.Leggi_Completa(Dt_Giacenze.Rows(i).Item("Pro_Cod"),
                                                                RicercaTesto,
                                                                0,
                                                                True,
                                                                Piva,
                                                                Dt_Giacenze.Rows(i).Item("Mat_Cod"),
                                                                objParametri_server.FinestraTemporaleInizio,
                                                                objParametri_server.FinestraTemporaleFine,
                                                                "", " " & NomeDescrizione & " ",
                                                                objParametri_server,
                                                                0)

                                    Case Else

                                        Dt_Prodotti = objCat.LeggiTabella_da_CategorieMagazzino(NomeTabella,
                                                                 NomeCodice,
                                                                 NomeDescrizione,
                                                                 RicercaTesto,
                                                                 Dt_Giacenze.Rows(i).Item("Pro_Cod"),
                                                                 "", " " & NomeDescrizione & " ",
                                                                 objParametri_server)

                                End Select

                                If Not IsNothing(Dt_Prodotti) Then

                                    'Gruppi Merce
                                    If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                                        Dt_Prodotti.Columns.Add(New DataColumn("Id_Gruppo_Merce", GetType(Integer)))
                                        Dt_Prodotti.Columns.Add(New DataColumn("Des_Gruppo_Merce", GetType(String)))
                                    End If

                                    For j = 0 To Dt_Prodotti.Rows.Count - 1

                                        Select Case Elem_Cod

                                            Case FERTILIZZANTI
                                                'value salvato nella modalità gestita dalla formprodotto
                                                If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
                                                    x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
                                                ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
                                                    x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
                                                Else
                                                    'errore, qui non dovrebbe mai entrare
                                                    x_Pro_Cod = 0
                                                End If

                                                If Flag_VisualizzaProCod = True Then
                                                    x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                                                            " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                                                Else
                                                    x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                                                End If

                                                'salvo in un dt di appoggio (da utilizzare per il dataview)
                                                'e dopo l'ordinamento del dataview carico la combo
                                                DrTemp = Dt_Temp.NewRow
                                                DrTemp.Item("Des") = x_Pro_Des
                                                DrTemp.Item("Cod") = x_Pro_Cod
                                                Dt_Temp.Rows.Add(DrTemp)

                                            Case Else

                                                x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

                                                If Flag_VisualizzaProCod = True Then
                                                    x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) &
                                                            " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                                                Else
                                                    x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                                                End If
                                                If Not IsNothing(Flag_CodArticolo_In_Descrizione) AndAlso Flag_CodArticolo_In_Descrizione Then
                                                    If Dt_Prodotti.Columns.Contains("Cod_Articolo") AndAlso Not IsDBNull(Dt_Prodotti.Rows(j).Item("Cod_Articolo")) Then
                                                        Dim cod_articolo = Dt_Prodotti.Rows(j).Item("Cod_Articolo").ToString.Trim
                                                        If Not String.IsNullOrEmpty(cod_articolo) AndAlso Not String.IsNullOrWhiteSpace(cod_articolo) Then
                                                            x_Pro_Des = x_Pro_Des & " (" + cod_articolo & ") "
                                                        End If
                                                    End If
                                                End If

                                                'salvo in un dt di appoggio (da utilizzare per il dataview)
                                                'e dopo l'ordinamento del dataview carico la combo
                                                DrTemp = Dt_Temp.NewRow
                                                DrTemp.Item("Des") = x_Pro_Des
                                                DrTemp.Item("Cod") = x_Pro_Cod
                                                Dt_Temp.Rows.Add(DrTemp)

                                        End Select

                                        If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                                            Dt_Prodotti.Rows(j).Item("Id_Gruppo_Merce") = Dt_Giacenze.Rows(i).Item("Id_Gruppo_Merce")
                                            Dt_Prodotti.Rows(j).Item("Des_Gruppo_Merce") = Dt_Giacenze.Rows(i).Item("Des_Gruppo_Merce")
                                        End If

                                    Next

                                    If DT_Risultato Is Nothing Then
                                        DT_Risultato = Dt_Prodotti.Copy()
                                    ElseIf DT_Risultato.Rows.Count = 0 Then
                                        DT_Risultato = Dt_Prodotti.Copy()
                                    Else
                                        For Each r In Dt_Prodotti.Rows
                                            DT_Risultato.ImportRow(r)
                                            ' DT_Risultato.Rows.Add(r.ItemArray)
                                        Next
                                    End If

                                End If

                            Next 'giacenze

                    End Select

                End If

                If Dt_Temp.Rows.Count <> 0 Then

                    'uso il dataview per ordinare
                    Dim Dv As New DataView

                    Dt_Temp.TableName = "prodotti"
                    Dv.Table = Dt_Temp
                    Dv.Sort = "Des ASC"

                    For i = 0 To Dt_Temp.Rows.Count - 1

                        x_Pro_Cod = Dv.Item(i).Item("Cod")
                        x_Pro_Des = CStr(Dv.Item(i).Item("Des"))

                        If IsNothing(Controllo.Items.FindByValue(x_Pro_Cod)) Then
                            Controllo.Items.Add(New ListItem(x_Pro_Des, x_Pro_Cod))
                        End If

                    Next

                End If

                Dt_Giacenze = Nothing
                Dt_Prodotti = Nothing

        End Select


    End Sub

    Public Sub Lotti_Zootecnia(ByRef Controllo As ListControl,
                                ByVal DataInizio As Date,
                                ByVal DataFine As Date,
                                ByVal piva As String,
                                ByVal Centro As String,
                                ByVal Stalla As String,
                                ByVal Dettaglio As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        Dim Dt_Lotti As DataTable
        Dim Dt_Prodotti As New DataTable
        Dim x_Lot_Cod As String
        Dim x_Lot_Des As String

        Dim zooDal As New AgronicaCoreZooDAL.Report_Partite_R
        Dt_Lotti = zooDal.LeggiSintesiDet(DataInizio, DataFine, "", piva, Centro, Stalla, "", "", "", "", xFiltroAggiuntivo, xOrderBy, objParametri_server, True, Dettaglio)

        If Dt_Lotti.Rows.Count > 0 Then
            For Each dr As DataRow In Dt_Lotti.Rows
                x_Lot_Cod = dr.Item("Lotto")
                x_Lot_Des = dr.Item("Lotto")
                If IsNothing(Controllo.Items.FindByValue(x_Lot_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Lot_Des, x_Lot_Cod))
                End If
            Next
        End If

    End Sub

    Public Sub Fornitore_Zootecnia(ByRef Controllo As ListControl,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        Dim Dt_Lotti As DataTable
        Dim Dt_Prodotti As New DataTable
        Dim x_Forn_Cod As String
        Dim x_Forn_Des As String

        Dim contDal As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dt_Lotti = contDal.Leggi_Generico(xFiltroAggiuntivo, xOrderBy, objParametri_server)

        If Dt_Lotti.Rows.Count > 0 Then
            For Each dr As DataRow In Dt_Lotti.Rows
                x_Forn_Cod = dr.Item("Cod_Contatto")
                x_Forn_Des = If(dr.Item("Rag_Soc") <> "", dr.Item("Rag_Soc"), dr.Item("Cognome") & " " & dr.Item("Nome"))
                If IsNothing(Controllo.Items.FindByValue(x_Forn_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Forn_Des, x_Forn_Cod))
                End If
            Next
        End If

    End Sub

    Public Sub Lista_Razze_Bovini_Zootecnia(ByRef Controllo As ListControl,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                               )

        Dim Dt_Bovini As DataTable
        Dim Dt_Prodotti As New DataTable
        Dim x_Raz_Cod As String
        Dim x_Raz_Des As String

        Dim zooDal As New AgronicaCoreMetaSchemaDAL.Lista_Razze_Animali_R
        '1 per bovini e il secondo 1 per le mucche
        Dt_Bovini = zooDal.Leggi(1, 1, -1, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_server)

        If Dt_Bovini.Rows.Count > 0 Then
            For Each dr As DataRow In Dt_Bovini.Rows
                x_Raz_Cod = dr.Item("GEN_Cod") & "-" & dr.Item("SPE_Cod") & "-" & dr.Item("RAZ_Cod")
                x_Raz_Des = dr.Item("RAZ_DES")
                If IsNothing(Controllo.Items.FindByValue(x_Raz_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Raz_Des, x_Raz_Cod))
                End If
            Next
        End If

    End Sub

    Public Sub Stalle_Per_AziendaCentro(ByRef Controllo As ListControl,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Dim Dt_Stalle As DataTable
        Dim Dt_Prodotti As New DataTable
        Dim x_Sta_Cod As String
        Dim x_Sta_Des As String

        Dim zooDal As New AgronicaCoreZooDAL.Report_Partite_R
        Dt_Stalle = zooDal.TrovaStallePerAziendaCentro(Piva, Sa_Cod, xFiltroAggiuntivo, xOrderBy, objParametri_server)

        If Dt_Stalle.Rows.Count > 0 Then
            For Each dr As DataRow In Dt_Stalle.Rows
                x_Sta_Cod = dr.Item("STA_NUM").ToString
                x_Sta_Des = dr.Item("STA_DES").ToString
                If IsNothing(Controllo.Items.FindByValue(x_Sta_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Sta_Des, x_Sta_Cod))
                End If
            Next
        End If

    End Sub

    Public Sub Centri_Zoo_Per_Azienda(ByRef Controllo As ListControl,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Dim Dt_Centri As DataTable
        Dim x_Cen_Cod As String
        Dim x_Cen_Des As String

        Dim zooDal As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dt_Centri = zooDal.Leggi_x_anagraficaZoo(Piva, 0, "", "", objParametri_server)

        If Dt_Centri.Rows.Count > 0 Then
            For Each dr As DataRow In Dt_Centri.Rows
                x_Cen_Cod = dr.Item("sa_cod").ToString
                x_Cen_Des = dr.Item("Sa_Nome").ToString
                If IsNothing(Controllo.Items.FindByValue(x_Cen_Cod)) Then
                    Controllo.Items.Add(New ListItem(x_Cen_Des, x_Cen_Cod))
                End If
            Next
        End If

    End Sub


    Private Class CaricaListControl_2010_AddON
        Private Sub Chiama_WSConcimi_xCaricaCombo(ByVal PUA_RegolamentoCod As Integer)






        End Sub

    End Class

End Class

