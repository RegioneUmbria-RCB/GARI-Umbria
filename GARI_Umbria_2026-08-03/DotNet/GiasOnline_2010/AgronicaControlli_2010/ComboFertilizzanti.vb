Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel
Imports AgronicaCoreDataProvider


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboFertilizzanti")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("ComboFertilizzanti_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboFertilizzanti runat=server></{0}:ComboFertilizzanti>")> Public Class ComboFertilizzanti
    Inherits System.Web.UI.WebControls.WebControl


    Public ddl_Fertilizzanti As DropDownList
    Private _Bootstrap As Boolean
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _TipoRichiesto As Integer
    Private _TestoRicerca As String
    Private _FerCod As Integer
    Private _MatCod As Integer
    Private _Flag_CodNegativo As Boolean
    Private _Flag_IncludiNPK As Boolean
    Private _Flag_IncludiNPK_Desc As Boolean
    Private _IncludiAziendali As Boolean
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Fabbricato_Cod As String
    Private _Cau_Mov As String
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date
    Private _FiltroAggiuntivo As String
    Private _OrderBy As String
    Private _Regolamento_Cod As Integer
    Private _Pua_Cod As Integer
    Private _Stato_Cod As String

    Private _GestioneLotto As enum_Gestione_Lotti
    Private _GestioneGiacenze As enum_Gestione_Giacenze
    Private _EscludiGiacenzeZero As Boolean

    Private _Ricerca As Boolean = True
    Private _IncludiFertilizzantiDirettive As Boolean = False
    Private _Flag_Classificazione As Boolean


    Public Sub New()

        _Bootstrap = False
        _PrimaRiga_Flag = True
        _PrimaRiga_Text = ""
        _PrimaRiga_Value = ""
        _TipoRichiesto = 0
        _TestoRicerca = ""
        _FerCod = 0
        _MatCod = 0
        _Flag_CodNegativo = False
        _Flag_IncludiNPK = True
        _Flag_IncludiNPK_Desc = False
        _IncludiAziendali = False
        _Piva = ""
        _Sa_Cod = 0
        _Fabbricato_Cod = "0"
        _Cau_Mov = ""
        _Validita_Inizio = AGRODATAINIZIO
        _Validita_Fine = AGRODATAFINE
        _FiltroAggiuntivo = ""
        _OrderBy = ""
        _Regolamento_Cod = 0
        _Pua_Cod = 0
        _IncludiFertilizzantiDirettive = False
        _Flag_Classificazione = False
        _GestioneLotto = enum_Gestione_Lotti.Nessuna
        _GestioneGiacenze = enum_Gestione_Giacenze.SoloMovimentati
        _EscludiGiacenzeZero = True
        _Stato_Cod = ""

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Fertilizzanti = New DropDownList
        ddl_Fertilizzanti.ID = Me.ClientID & "ComboFertilizzanti"

        If _Bootstrap = False Then
            ddl_Fertilizzanti.CssClass = "ComboFertilizzanti txtUI"
            ddl_Fertilizzanti.Attributes.Add("style", " min-width: 525px;width: 90%; ")
            ddl_Fertilizzanti.Attributes.Add("onchange", " CambiaFertilizzanti(); ")
        Else
            ddl_Fertilizzanti.CssClass = "selectpicker ComboFertilizzanti"
            If _Ricerca = True Then
                ddl_Fertilizzanti.Attributes.Add("data-live-search", "true")
            End If

            ddl_Fertilizzanti.Attributes.Add("data-container", "body")
            ddl_Fertilizzanti.Attributes.Add("onchange", " CambiaFertilizzanti(); ")
        End If
        

        Me.Controls.Add(ddl_Fertilizzanti)
        MyBase.OnInit(e)

    End Sub
     

#Region "Proprietà"

    Public Property Ricerca() As Boolean
        Get
            Return _Ricerca
        End Get
        Set(ByVal value As Boolean)
            _Ricerca = value
        End Set
    End Property

    Public Property Bootstrap() As Boolean
        Get
            Return _Bootstrap
        End Get
        Set(ByVal value As Boolean)
            _Bootstrap = value
        End Set
    End Property

    Public Property PrimaRiga_Flag() As Boolean
        Get
            Return _PrimaRiga_Flag
        End Get
        Set(ByVal value As Boolean)
            _PrimaRiga_Flag = value
        End Set
    End Property

    Public Property PrimaRiga_Text() As String
        Get
            Return _PrimaRiga_Text
        End Get
        Set(ByVal value As String)
            _PrimaRiga_Text = value
        End Set
    End Property

    Public Property PrimaRiga_Value() As String
        Get
            Return _PrimaRiga_Value
        End Get
        Set(ByVal value As String)
            _PrimaRiga_Value = value
        End Set
    End Property

    Public Property TipoRichiesto() As Integer
        Get
            Return _TipoRichiesto
        End Get
        Set(ByVal value As Integer)
            _TipoRichiesto = value
        End Set
    End Property

    Public Property TestoRicerca() As String
        Get
            Return _TestoRicerca
        End Get
        Set(ByVal value As String)
            _TestoRicerca = value
        End Set
    End Property

    Public Property FerCod() As Integer
        Get
            Return _FerCod
        End Get
        Set(ByVal value As Integer)
            _FerCod = value
        End Set
    End Property

    Public Property MatCod() As Integer
        Get
            Return _MatCod
        End Get
        Set(ByVal value As Integer)
            _MatCod = value
        End Set
    End Property

    Public Property Flag_CodNegativo() As Boolean
        Get
            Return _Flag_CodNegativo
        End Get
        Set(ByVal value As Boolean)
            _Flag_CodNegativo = value
        End Set
    End Property

    Public Property Flag_IncludiNPK() As Boolean
        Get
            Return _Flag_IncludiNPK
        End Get
        Set(ByVal value As Boolean)
            _Flag_IncludiNPK = value
        End Set
    End Property

    Public Property Flag_IncludiNPK_Desc() As Boolean
        Get
            Return _Flag_IncludiNPK_Desc
        End Get
        Set(ByVal value As Boolean)
            _Flag_IncludiNPK_Desc = value
        End Set
    End Property

    Public Property Flag_Classificazione() As Boolean
        Get
            Return _Flag_Classificazione
        End Get
        Set(ByVal value As Boolean)
            _Flag_Classificazione = value
        End Set
    End Property

    Public Property IncludiAziendali() As Boolean
        Get
            Return _IncludiAziendali
        End Get
        Set(ByVal value As Boolean)
            _IncludiAziendali = value
        End Set
    End Property
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property

    Public Property Fabbricato_Cod() As String
        Get
            Return _Fabbricato_Cod
        End Get
        Set(ByVal value As String)
            _Fabbricato_Cod = value
        End Set
    End Property

    Public Property Cau_Mov() As String
        Get
            Return _Cau_Mov
        End Get
        Set(ByVal value As String)
            _Cau_Mov = value
        End Set
    End Property
    Public Property Validita_Inizio() As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property

    Public Property Validita_Fine() As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property

    Public Property FiltroAggiuntivo() As String
        Get
            Return _FiltroAggiuntivo
        End Get
        Set(ByVal value As String)
            _FiltroAggiuntivo = value
        End Set
    End Property

    Public Property OrderBy() As String
        Get
            Return _OrderBy
        End Get
        Set(ByVal value As String)
            _OrderBy = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_Fertilizzanti.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Fertilizzanti.Items.FindByValue(value)) Then
                    ddl_Fertilizzanti.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            If Not IsNothing(ddl_Fertilizzanti.SelectedItem) Then
                Return ddl_Fertilizzanti.SelectedItem.Text
            Else
                Return Nothing
            End If

        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Fertilizzanti.Items.FindByText(value)) Then
                    ddl_Fertilizzanti.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property N_Fertilizzanti() As Integer
        Get
            Return ddl_Fertilizzanti.Items.Count - 1
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property

    Public Property Regolamento_Cod() As Integer
        Get
            Return _Regolamento_Cod
        End Get
        Set(ByVal value As Integer)
            _Regolamento_Cod = value
        End Set
    End Property

    Public Property Pua_Cod() As Integer
        Get
            Return _Pua_Cod
        End Get
        Set(ByVal value As Integer)
            _Pua_Cod = value
        End Set
    End Property

    Public Property Stato_Cod() As String
        Get
            Return _Stato_Cod
        End Get
        Set(ByVal value As String)
            _Stato_Cod = value
        End Set
    End Property

    Public Property IncludiFertilizzantiDirettive() As Boolean
        Get
            Return _IncludiFertilizzantiDirettive
        End Get
        Set(ByVal value As Boolean)
            _IncludiFertilizzantiDirettive = value
        End Set
    End Property

    Public Property GestioneLotto() As enum_Gestione_Lotti
        Get
            Return _GestioneLotto
        End Get
        Set(ByVal value As enum_Gestione_Lotti)
            _GestioneLotto = value
        End Set
    End Property

    Public Property GestioneGiacenze() As enum_Gestione_Giacenze
        Get
            Return _GestioneGiacenze
        End Get
        Set(ByVal value As enum_Gestione_Giacenze)
            _GestioneGiacenze = value
        End Set
    End Property

    Public Property EscludiGiacenzeZero() As Boolean
        Get
            Return _EscludiGiacenzeZero
        End Get
        Set(ByVal value As Boolean)
            _EscludiGiacenzeZero = value
        End Set
    End Property

#End Region






    Public Sub CaricaComboFertilizzanti()

        Dim Dt As DataTable

        Dim strGiacenza As String = ""
        Dim Dt_Giacenze As DataTable
        Dim Dt_Giacenze_Tot As DataTable
        Dim DrGiacenze() As DataRow
        Dim DrGiacenze_Tot() As DataRow

        Dim x_Cod As String
        Dim x_Des As String

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        ddl_Fertilizzanti.Items.Clear()

        If _PrimaRiga_Flag = True Then
            ddl_Fertilizzanti.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
        End If

        Select Case _Fabbricato_Cod

            Case "0"

                Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

                Dt = objFertilizzanti.Leggi_Completa(_FerCod,
                                                     _TestoRicerca,
                                                     _TipoRichiesto,
                                                     _IncludiAziendali,
                                                     _Piva,
                                                     _MatCod,
                                                     AGRODATAINIZIO,
                                                     AGRODATAFINE,
                                                     _FiltroAggiuntivo,
                                                     _OrderBy,
                                                     objParametriServer,
                                                     _Regolamento_Cod)

                objFertilizzanti = Nothing

            Case Else


                Select Case _Cau_Mov

                    Case CAU_SCARICO

                        Dim objG As New AgronicaCoreStampeDAL.Magazzino
                        Dim strFiltroFerCod As String = ""
                        Dim strFiltroMatCod As String = ""

                        Dt_Giacenze = objG.SchedaGiacenzeMagazzino(_Validita_Fine,
                                             _Piva,
                                             Split(_Fabbricato_Cod, "|")(1),
                                             Split(_Fabbricato_Cod, "|")(0),
                                             FERTILIZZANTI,
                                             _FerCod,
                                             _MatCod,
                                             0, 0, 0, 0,
                                             LOTTO_NONDEFINITO,
                                             False,
                                             "", "", "", "", "", "", "", "", "", "", "",
                                             "",
                                             "",
                                             objParametriServer, objParametriUtenti)

                        Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                     _Piva,
                                     Split(_Fabbricato_Cod, "|")(1),
                                     Split(_Fabbricato_Cod, "|")(0),
                                     FERTILIZZANTI,
                                     _FerCod,
                                     _MatCod,
                                     0, 0, 0, 0,
                                     LOTTO_NONDEFINITO,
                                     False,
                                     "", "", "", "", "", "", "", "", "", "", "",
                                     "",
                                     "",
                                     objParametriServer, objParametriUtenti)
                        objG = Nothing

                        'filtro
                        If Dt_Giacenze.Rows.Count > 0 Then

                            Dim dr() As DataRow
                            dr = Dt_Giacenze.Select(" Descrizione_Prodotto Like'%" & TestoRicerca & "%'")


                            For i = 0 To dr.Count - 1

                                If Dt_Giacenze(i).Item("Giacenza") > QTA_GiancenzeVisualizzate OR Dt_Giacenze(i).Item("Giacenza") < -QTA_GiancenzeVisualizzate Then

                                    Select Case dr(i).Item("Pro_Cod")
                                        Case "0"
                                            strFiltroMatCod &= " Materie_Prime.Mat_Cod=" & dr(i).Item("Mat_Cod").ToString & " OR "
                                        Case Else
                                            strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & dr(i).Item("Pro_Cod").ToString & " OR "
                                    End Select

                                    ' ''per i fertilizzanti oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
                                    ' ''(per i fertilizzanti aziendali)

                                End If


                            Next
                        End If

                        Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
                        Dim Dt_Prodotti As DataTable


                        'leggo solo le Materie Prime
                        If strFiltroMatCod.Length > 0 Then
                            strFiltroMatCod = Left(strFiltroMatCod, strFiltroMatCod.Length - 3)
                            strFiltroMatCod = "(" & strFiltroMatCod & ")"
                        End If


                        If strFiltroFerCod.Length > 0 Then
                            strFiltroFerCod = Left(strFiltroFerCod, strFiltroFerCod.Length - 3)
                            strFiltroFerCod = "(" & strFiltroFerCod & ")"
                        End If

                        If strFiltroMatCod <> "" Or strFiltroFerCod <> "" Then
                            Dt_Prodotti = objFertilizzanti.Leggi_Completa_Magazzino(_TipoRichiesto,
                                                                           _Piva,
                                                                          _Validita_Inizio,
                                                                          _Validita_Fine,
                                                                          strFiltroFerCod,
                                                                          strFiltroMatCod,
                                                                          "",
                                                                          objParametriServer,
                                                                          _Regolamento_Cod)
                        End If
                        '------------------- 
                        If Not IsNothing(Dt_Prodotti) AndAlso Dt_Prodotti.Rows.Count > 0 Then
                            If Dt Is Nothing Then
                                Dt = Dt_Prodotti.Clone
                            End If

                            For j = 0 To Dt_Prodotti.Rows.Count - 1
                                Dt.ImportRow(Dt_Prodotti.Rows(j))
                            Next
                        End If

                        objFertilizzanti = Nothing

                    Case CAU_CARICO

                        'DA FARE se necessario

                End Select

        End Select


        If Not IsNothing(Dt) Then

            'uso il dataview per ordinare 
            Dim Dv As New DataView

            Dt.TableName = "Prodotti"
            Dv.Table = Dt
            Dv.Sort = "Fer_Des ASC"

            Dim isnothingMatCod As Boolean = IsNothing(Dv.ToTable.Columns.Item("Mat_Cod"))

            For i = 0 To Dv.Count - 1

                x_Des = CStr(Dv(i).Item("Fer_Des"))

                If _TipoRichiesto = 6 Or _TipoRichiesto = 7 Then
                    x_Des = CStr(Dv(i).Item("descrizione")) & " - " & x_Des
                End If

                If _Flag_IncludiNPK_Desc Then
                    x_Des += " (" + Format(Dv(i).Item("N"), "0.##") + "-" +
                             Format(Dv(i).Item("P2O5"), "0.##") + "-" +
                             Format(Dv(i).Item("K2O"), "0.##") + "-" +
                             Format(Dv(i).Item("MgO"), "0.##") + ")"
                End If

                strGiacenza = ""
                If Not Dt_Giacenze Is Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                    DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Dv(i).Item("Fer_Cod"))
                    If Not DrGiacenze Is Nothing AndAlso DrGiacenze.Length > 0 Then
                        strGiacenza = " --- GIACENZA: alla data " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                    End If
                End If
                If Not Dt_Giacenze_Tot Is Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                    DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Dv(i).Item("Fer_Cod"))
                    If Not DrGiacenze_Tot Is Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                        strGiacenza &= " totale " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                    End If
                End If

                If _Flag_CodNegativo = False Then

                    If Not isnothingMatCod Then
                        'value salvato nella modalità gestita dalle operazioni di campagna
                        x_Cod = CStr(Dv(i).Item("Fer_Cod")) & "/" & CStr(Dv(i).Item("Mat_Cod"))
                    Else
                        x_Cod = CStr(Dv(i).Item("Fer_Cod")) & "/0"
                    End If

                Else

                    'value salvato nella modalità gestita dalla formprodotto
                    If Dv(i).Item("Fer_Cod") <> 0 Then
                        x_Cod = CStr(Dv(i).Item("Fer_Cod"))
                    ElseIf Dv(i).Item("Mat_Cod") <> 0 Then
                        x_Cod = "-" + CStr(Dv(i).Item("Mat_Cod"))
                    Else
                        'errore, qui non dovrebbe mai entrare
                        x_Cod = "0"
                    End If
                End If

                If _Flag_IncludiNPK = True Then
                    x_Cod = x_Cod & "/" +
                            CStr(Dv(i).Item("N")) + "-" +
                            CStr(Dv(i).Item("P2O5")) + "-" +
                            CStr(Dv(i).Item("K2O")) + "-" +
                            CStr(Dv(i).Item("MgO"))
                End If

                If _TipoRichiesto = 6 Or _TipoRichiesto = 7 Then
                    x_Cod = x_Cod & "$" & CStr(Dv(i).Item("id_tp_fer"))
                End If

                ddl_Fertilizzanti.Items.Add(New ListItem(x_Des + strGiacenza, x_Cod))

            Next

            N_Fertilizzanti = ddl_Fertilizzanti.Items.Count - 1

        End If

        Dt = Nothing


    End Sub


    Public Sub CaricaComboFertilizzanti_WS()

        Dim strGiacenza As String = ""
        Dim strGiacenzaVal_AllaData As String = ""
        Dim strGiacenzaVal_Totale As String = ""
        Dim strGiacenzaUdmCod As String = ""
        Dim strGiacenzaUdmSim As String = ""

        Dim Dt_Giacenze As DataTable
        Dim Dt_Giacenze_Tot As DataTable
        Dim DrGiacenze() As DataRow
        Dim DrGiacenze_Tot() As DataRow

        Dim x_Cod As String
        Dim x_Des As String

        'Dim flagQtaNoZero As Boolean = False
        Dim flagQtaMaggioreZero As Boolean = False
        Dim utilizzaLotto = False

        Dim DtEffluenti As DataTable
        Dim objParametriUscitaEff As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output

        Dim objParametriSuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        ddl_Fertilizzanti.Items.Clear()

        If _PrimaRiga_Flag = True Then
            ddl_Fertilizzanti.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
        End If

        '(15/03/2019 fede)
        Select Case _GestioneLotto
            Case enum_Gestione_Lotti.Nessuna
                utilizzaLotto = False
            Case Else
                utilizzaLotto = True
        End Select

        Select Case _GestioneGiacenze
            Case enum_Gestione_Giacenze.SoloPresenti
                flagQtaMaggioreZero = True
            Case enum_Gestione_Giacenze.TuttiProdotti
                _Fabbricato_Cod = 0
                utilizzaLotto = False
        End Select

        'se arrivo da un pua 
        'leggo i fertilizzanti caricati nella dichiarazione effluenti
        'leggo gli effluenti del regolamento
        If _Pua_Cod > 0 Then

            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            DtEffluenti = objEff.Leggi(Regolamento_Cod, _Pua_Cod, 0, " azoto_qta > 0 ", "", objParametriServer)

            Dim objParametriIngressoEff As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
            objParametriIngressoEff.Regolamento_Cod = Regolamento_Cod
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscitaEff = objPC_WS.Effluenti(objParametriIngressoEff, objParametriServer, objParametriSuperServer)

        End If

        If _Fabbricato_Cod <> "0" Then

            Select Case _Cau_Mov

                Case CAU_SCARICO

                    Dim strFiltroFerCod As String = ""

                    If _Pua_Cod = 0 Then

                        Dim objG As New AgronicaCoreStampeDAL.Magazzino


                        Dt_Giacenze = objG.SchedaGiacenzeMagazzino(_Validita_Fine,
                                         Split(_Fabbricato_Cod, "|")(2),
                                         Split(_Fabbricato_Cod, "|")(1),
                                         Split(_Fabbricato_Cod, "|")(0),
                                         FERTILIZZANTI,
                                         _FerCod,
                                         0,
                                         0, 0, 0, 0,
                                         LOTTO_NONDEFINITO,
                                         _EscludiGiacenzeZero,
                                         "", "", "", "", "", "", "", "", "", "", "",
                                         "",
                                         "",
                                         objParametriServer, objParametriUtenti,
                                         Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

                        Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                 Split(_Fabbricato_Cod, "|")(2),
                                 Split(_Fabbricato_Cod, "|")(1),
                                 Split(_Fabbricato_Cod, "|")(0),
                                 FERTILIZZANTI,
                                 _FerCod,
                                 0,
                                 0, 0, 0, 0,
                                 LOTTO_NONDEFINITO,
                                 _EscludiGiacenzeZero,
                                 "", "", "", "", "", "", "", "", "", "", "",
                                 "",
                                 "",
                                 objParametriServer, objParametriUtenti,
                                         Flag_QtaMaggioreZero:=flagQtaMaggioreZero)
                        objG = Nothing

                        'filtro
                        If Dt_Giacenze.Rows.Count > 0 Then
                            Dim dr() As DataRow = Dt_Giacenze.Select(" Descrizione_Prodotto Like'%" & TestoRicerca & "%'")

                            For i = 0 To dr.Count - 1

                                'Anna 10/05/2022: nascoste migrogiacenze SE
                                '                                   impostazione utente = Solo Presenti (> 0 e QTA_GiancenzeVisualizzate)
                                '                                        O SE
                                '                                   se visualizza giacenze zero = non checkato e impostazione utente = tutti i movimenti
                                '[rif chiamata 17160]
                                Dim filtroGiacenze = (dr(i).Item("Giacenza") > QTA_GiancenzeVisualizzate Or dr(i).Item("Giacenza") < -QTA_GiancenzeVisualizzate)
                                If _EscludiGiacenzeZero = False Then
                                    If (flagQtaMaggioreZero) = False Then
                                        strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & dr(i).Item("Pro_Cod").ToString & " OR "
                                    ElseIf flagQtaMaggioreZero = True And filtroGiacenze = True Then
                                        strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & dr(i).Item("Pro_Cod").ToString & " OR "
                                    End If
                                Else
                                    If (filtroGiacenze) Then
                                        strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & dr(i).Item("Pro_Cod").ToString & " OR "
                                    End If
                                End If

                                'If Dt_Giacenze(i).Item("Giacenza") > QTA_GiancenzeVisualizzate Or Dt_Giacenze(i).Item("Giacenza") < -QTA_GiancenzeVisualizzate Then
                                '    strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & dr(i).Item("Pro_Cod").ToString & " OR "
                                'End If
                            Next
                        End If

                        If strFiltroFerCod.Length > 0 Then
                            strFiltroFerCod = Left(strFiltroFerCod, strFiltroFerCod.Length - 3)
                            strFiltroFerCod = "(" & strFiltroFerCod & ")"
                            If _FiltroAggiuntivo <> "" Then
                                _FiltroAggiuntivo &= " AND " & strFiltroFerCod
                            Else
                                _FiltroAggiuntivo &= " " & strFiltroFerCod
                            End If
                        End If

                        'se non esistono prodotti evito la chiamata al ws
                        If strFiltroFerCod = "" Then
                            Exit Sub
                        End If

                    Else

                        If Not IsNothing(DtEffluenti) AndAlso DtEffluenti.Rows.Count > 0 Then

                            If Not objParametriUscitaEff Is Nothing AndAlso objParametriUscitaEff.ListaEffluenti.Count > 0 Then

                                For Each drE As DataRow In DtEffluenti.Rows
                                    Dim FerCod As Integer = (From l In objParametriUscitaEff.ListaEffluenti
                                                             Where l.Eff_Cod = drE.Item("eff_cod") And l.Fer_Des.ToLower.Contains(TestoRicerca.ToLower)
                                                             Select l.Fer_Cod).FirstOrDefault()
                                    If FerCod > 0 Then
                                        strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & FerCod.ToString & " OR "
                                    End If
                                Next

                                If strFiltroFerCod.Length > 0 Then
                                    strFiltroFerCod = Left(strFiltroFerCod, strFiltroFerCod.Length - 3)
                                    strFiltroFerCod = "(" & strFiltroFerCod & ")"
                                    If _FiltroAggiuntivo <> "" Then
                                        _FiltroAggiuntivo &= " AND " & strFiltroFerCod
                                    Else
                                        _FiltroAggiuntivo &= " " & strFiltroFerCod
                                    End If
                                End If

                                'se non esistono prodotti evito la chiamata al ws
                                If strFiltroFerCod = "" Then
                                    Exit Sub
                                End If

                            Else
                                Exit Sub
                            End If

                        Else
                            Exit Sub
                        End If

                    End If

                Case CAU_CARICO

                    'DA FARE se necessario

            End Select

        End If


        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
        objParametriIngresso.Codice = _FerCod
        objParametriIngresso.Descrizione = _TestoRicerca
        objParametriIngresso.DataInizio = AGRODATAINIZIO
        objParametriIngresso.DataFine = AGRODATAFINE
        objParametriIngresso.Tipo = _TipoRichiesto
        objParametriIngresso.IncludiApporti = _Flag_IncludiNPK_Desc
        objParametriIngresso.IncludiTipologia = _Flag_Classificazione
        objParametriIngresso.Regolamento = _Regolamento_Cod
        objParametriIngresso.strFiltro = _FiltroAggiuntivo
        objParametriIngresso.Stato_Cod = _Stato_Cod
        objParametriIngresso.Lingua_Cod = objParametriServer.Lingua_Cod


        Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output = objFert_WS.Fertilizzanti(objParametriIngresso)

        Dim strTipologie As String = ""

        For i = 0 To objParametriUscita.ListaFertilizzanti.Count - 1

            If _TipoRichiesto >= 6 Then
                x_Des = CStr(objParametriUscita.ListaFertilizzanti(i).TipoFertilizzanteDescrizione) & " - " & objParametriUscita.ListaFertilizzanti(i).Descrizione
            Else
                x_Des = objParametriUscita.ListaFertilizzanti(i).Descrizione
            End If

            x_Cod = objParametriUscita.ListaFertilizzanti(i).Codice

            If _Flag_IncludiNPK_Desc Then

                '(13/06/2019 fede) in caso di direttiva nitrati ed effluenti già dichiarati prendo l'N salvato in dichiarazione
                If _Pua_Cod > 0 AndAlso Not IsNothing(DtEffluenti) AndAlso DtEffluenti.Rows.Count > 0 Then

                    Dim EffCod As Integer = (From l In objParametriUscitaEff.ListaEffluenti
                                             Where l.Fer_Cod = x_Cod
                                             Select l.Eff_Cod).FirstOrDefault()

                    Dim mediaPesataN As Decimal = 0
                    Dim divisoreMediaPesataN As Decimal = 0

                    For Each dr As DataRow In DtEffluenti.Rows

                        If dr.Item("eff_cod") = EffCod Then

                            Dim qta As Decimal = CDec(dr.Item("carico"))
                            Dim azoto_titoli As Decimal = CDec(dr.Item("azoto_titoli"))

                            If azoto_titoli > 0 Then
                                mediaPesataN += (azoto_titoli * qta)
                                divisoreMediaPesataN += qta
                            End If

                        End If

                    Next

                    If divisoreMediaPesataN <> 0 Then
                        mediaPesataN = mediaPesataN / divisoreMediaPesataN
                    End If
                    Dim strN As String = Format(IIf(mediaPesataN = 0, objParametriUscita.ListaFertilizzanti(i).N, mediaPesataN), "0.##")
                    If mediaPesataN > 0 AndAlso mediaPesataN <> objParametriUscita.ListaFertilizzanti(i).N Then
                        strN += "*"
                    End If

                    x_Des += " (" + strN + "-0-0-0)"

                    x_Cod &= "/" + CStr(IIf(mediaPesataN = 0, objParametriUscita.ListaFertilizzanti(i).N, Format(mediaPesataN, "0.##"))) + "-0-0-0-0"

                Else

                    '(19/07/2019 fede) introdotta lettura e calcolo media ponderata dei titoli dei concimi editati nei carichi
                    Dim mediaPesataN As Decimal = 0
                    Dim mediaPesataP2O5 As Decimal = 0
                    Dim mediaPesataK2O As Decimal = 0
                    Dim mediaPesataMgO As Decimal = 0
                    Dim mediaPesataCu As Decimal = 0

                    If _Fabbricato_Cod <> "0" AndAlso _TipoRichiesto > 6 Then

                        Dim UdmG As Integer = 0
                        If Not Dt_Giacenze Is Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                            DrGiacenze = Dt_Giacenze.Select("pro_cod=" & objParametriUscita.ListaFertilizzanti(i).Codice)
                            If Not DrGiacenze Is Nothing AndAlso DrGiacenze.Length > 0 Then
                                UdmG = DrGiacenze(0).Item("Udm_Cod")
                            End If
                        End If
                        Dim lavs_cod As Integer() = {LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_CARICO}
                        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R

                        '(21/04/2020 fede) aggiunto filtro magazzino
                        Dim filtroMagazzino As String = " Mov_Destinazioni.piva = '" & Split(_Fabbricato_Cod, "|")(2) & "' AND Mov_Destinazioni.sa_cod = " & Split(_Fabbricato_Cod, "|")(1) & " AND Mov_Destinazioni.Id_Destinazione = " & Split(_Fabbricato_Cod, "|")(0) & " "


                        Dim dtMovimenti As DataTable = objMovimenti.LeggiMovimentixPUA(Piva, lavs_cod, FERTILIZZANTI, x_Cod, enum_Agenda_Causali.CARICO,
                                                                                       AGRODATAINIZIO, _Validita_Fine, UdmG, filtroMagazzino, "", objParametriServer)

                        If dtMovimenti.Rows.Count > 0 Then

                            Dim divisoreMediaPesataN As Decimal = 0
                            Dim divisoreMediaPesataP2O5 As Decimal = 0
                            Dim divisoreMediaPesataK2O As Decimal = 0
                            Dim divisoreMediaPesataMgO As Decimal = 0
                            Dim divisoreMediaPesataCu As Decimal = 0

                            For Each dr As DataRow In dtMovimenti.Rows

                                Dim qta As Decimal = CDec(dr.Item("qta")) / CDec(1000)

                                Dim N As Decimal = CDec(dr.Item("N"))
                                Dim P As Decimal = CDec(dr.Item("P"))
                                Dim K As Decimal = CDec(dr.Item("K"))
                                Dim Mg As Decimal = CDec(dr.Item("Mg"))
                                Dim Cu As Decimal = CDec(dr.Item("Cu"))

                                If N > 0 Then
                                    mediaPesataN += (N * qta)
                                    divisoreMediaPesataN += qta
                                End If
                                If P > 0 Then
                                    mediaPesataP2O5 += (P * qta)
                                    divisoreMediaPesataP2O5 += qta
                                End If
                                If K > 0 Then
                                    mediaPesataK2O += (K * qta)
                                    divisoreMediaPesataK2O += qta
                                End If
                                If Mg > 0 Then
                                    mediaPesataMgO += (Mg * qta)
                                    divisoreMediaPesataMgO += qta
                                End If
                                If Cu > 0 Then
                                    mediaPesataCu += (Cu * qta)
                                    divisoreMediaPesataCu += qta
                                End If
                            Next

                            If divisoreMediaPesataN <> 0 Then
                                mediaPesataN = mediaPesataN / divisoreMediaPesataN
                            End If
                            If divisoreMediaPesataP2O5 <> 0 Then
                                mediaPesataP2O5 = mediaPesataP2O5 / divisoreMediaPesataP2O5
                            End If
                            If divisoreMediaPesataK2O <> 0 Then
                                mediaPesataK2O = mediaPesataK2O / divisoreMediaPesataK2O
                            End If
                            If divisoreMediaPesataMgO <> 0 Then
                                mediaPesataMgO = mediaPesataMgO / divisoreMediaPesataMgO
                            End If
                            If divisoreMediaPesataCu <> 0 Then
                                mediaPesataCu = mediaPesataCu / divisoreMediaPesataCu
                            End If

                        End If

                    End If

                    Dim strN As String = Format(IIf(mediaPesataN = 0, objParametriUscita.ListaFertilizzanti(i).N, mediaPesataN), "0.####")
                    If mediaPesataN > 0 AndAlso mediaPesataN <> objParametriUscita.ListaFertilizzanti(i).N Then
                        strN += "*"
                    End If
                    Dim strP2O5 As String = Format(IIf(mediaPesataP2O5 = 0, objParametriUscita.ListaFertilizzanti(i).P2O5, mediaPesataP2O5), "0.####")
                    If mediaPesataP2O5 > 0 AndAlso mediaPesataP2O5 <> objParametriUscita.ListaFertilizzanti(i).P2O5 Then
                        strP2O5 += "*"
                    End If
                    Dim strK2O As String = Format(IIf(mediaPesataK2O = 0, objParametriUscita.ListaFertilizzanti(i).K2O, mediaPesataK2O), "0.####")
                    If mediaPesataK2O > 0 AndAlso mediaPesataK2O <> objParametriUscita.ListaFertilizzanti(i).K2O Then
                        strK2O += "*"
                    End If
                    Dim strCu As String = Format(IIf(mediaPesataCu = 0, objParametriUscita.ListaFertilizzanti(i).Cu, mediaPesataCu), "0.####")
                    If mediaPesataCu > 0 AndAlso mediaPesataCu <> objParametriUscita.ListaFertilizzanti(i).Cu Then
                        strCu += "*"
                    End If
                    '(16/06/2017 fede) sostituito Mg con Cu
                    x_Des += " (" + strN + "-" +
                                    strP2O5 + "-" +
                                    strK2O + "-" +
                                    strCu + ")"

                    x_Cod &= "/" +
                               CStr(IIf(mediaPesataN = 0, objParametriUscita.ListaFertilizzanti(i).N, Format(mediaPesataN, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataP2O5 = 0, objParametriUscita.ListaFertilizzanti(i).P2O5, Format(mediaPesataP2O5, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataK2O = 0, objParametriUscita.ListaFertilizzanti(i).K2O, Format(mediaPesataK2O, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataMgO = 0, objParametriUscita.ListaFertilizzanti(i).MgO, Format(mediaPesataMgO, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataCu = 0, objParametriUscita.ListaFertilizzanti(i).Cu, Format(mediaPesataCu, "0.####")))

                End If

            End If

            strTipologie = ""
            If _Flag_Classificazione And _TipoRichiesto < 6 Then
                For j = 0 To objParametriUscita.ListaFertilizzanti(i).ListaTipologie.Count - 1
                    strTipologie &= objParametriUscita.ListaFertilizzanti(i).ListaTipologie(j).Descrizione & ","
                Next
                If strTipologie <> "" Then
                    x_Des += " --- [" + Left(strTipologie, strTipologie.Length - 1) + "]"
                End If
            End If

            'Grilli 20/05/2019 Commento perché poi ci devo aggiungere le giacenze e devo essere certo del numero di $ che ci sono nella stringa per fare lo spilt
            'Operazione effettuata con gaudio ed in stretta collaborazione con la quasi-consigliera Federica Monti
            'If _TipoRichiesto >= 6 Then
            x_Cod &= "$" & CStr(objParametriUscita.ListaFertilizzanti(i).TipoFertilizzanteCodice)
            'End If

            '(15/03/2019 fede) se utilizzo il lotto aggiungo le altre giacenze
            strGiacenza = ""
            strGiacenzaVal_AllaData = ""
            strGiacenzaVal_Totale = ""
            strGiacenzaUdmCod = ""
            strGiacenzaUdmSim = ""
            If Not Dt_Giacenze Is Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                DrGiacenze = Dt_Giacenze.Select("pro_cod=" & objParametriUscita.ListaFertilizzanti(i).Codice)
            End If
            If Not Dt_Giacenze_Tot Is Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & objParametriUscita.ListaFertilizzanti(i).Codice)
            End If

            Dim strLotto As String = ""

            If utilizzaLotto And Fabbricato_Cod <> "0" Then

                Dim x_Cod_lotto As String
                Dim x_Des_lotto As String

                If Not DrGiacenze Is Nothing AndAlso DrGiacenze.Length > 0 Then

                    For g = 0 To DrGiacenze.Length - 1

                        x_Cod_lotto = ""
                        x_Des_lotto = ""

                        strGiacenza = " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(DrGiacenze(g).Item("Giacenza"), 4).ToString & " " & DrGiacenze(g).Item("Udm_Sim")
                        strGiacenzaVal_AllaData = Math.Round(DrGiacenze(g).Item("Giacenza"), 4).ToString
                        strGiacenzaUdmCod = DrGiacenze(g).Item("Udm_Cod")
                        strGiacenzaUdmSim = DrGiacenze(g).Item("Udm_Sim")

                        If Not DrGiacenze_Tot Is Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                            For gt = 0 To DrGiacenze_Tot.Length - 1
                                If DrGiacenze_Tot(gt).Item("lotto") = DrGiacenze(g).Item("lotto") Then
                                    strGiacenza &= " " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(gt).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(gt).Item("Udm_Sim")
                                    strGiacenzaVal_Totale = Math.Round(DrGiacenze_Tot(gt).Item("Giacenza"), 4).ToString
                                    Exit For
                                End If
                            Next
                        End If

                        If strGiacenza <> "" Then
                            strGiacenza &= " --- " & My.Resources.AgronicaControlli_2010.Lotto + " " + DrGiacenze(g).Item("lotto")
                        End If

                        strLotto = DrGiacenze(g).Item("lotto")


                        x_Des_lotto = x_Des & strGiacenza
                        x_Cod_lotto = x_Cod & "$" & strGiacenzaVal_AllaData & "$" & strGiacenzaVal_Totale & "$" & strGiacenzaUdmCod & "$" & strGiacenzaUdmSim

                        x_Cod_lotto &= "$" & objParametriUscita.ListaFertilizzanti(i).Eff_Cod
                        x_Cod_lotto &= "$" & objParametriUscita.ListaFertilizzanti(i).Udm_Cod & "$" & strLotto

                        ddl_Fertilizzanti.Items.Add(New ListItem(x_Des_lotto, x_Cod_lotto))


                    Next

                End If

            Else

                If Not Dt_Giacenze Is Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                    DrGiacenze = Dt_Giacenze.Select("pro_cod=" & objParametriUscita.ListaFertilizzanti(i).Codice)
                    If Not DrGiacenze Is Nothing AndAlso DrGiacenze.Length > 0 Then
                        strGiacenza = " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                        strGiacenzaVal_AllaData = Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString
                        strGiacenzaUdmCod = DrGiacenze(0).Item("Udm_Cod")
                        strGiacenzaUdmSim = DrGiacenze(0).Item("Udm_Sim")
                    End If
                End If
                If Not Dt_Giacenze_Tot Is Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                    DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & objParametriUscita.ListaFertilizzanti(i).Codice)
                    If Not DrGiacenze_Tot Is Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                        strGiacenza &= " " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                        strGiacenzaVal_Totale = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString
                    End If
                End If

                x_Des &= strGiacenza
                x_Cod &= "$" & strGiacenzaVal_AllaData & "$" & strGiacenzaVal_Totale & "$" & strGiacenzaUdmCod & "$" & strGiacenzaUdmSim

                x_Cod &= "$" & objParametriUscita.ListaFertilizzanti(i).Eff_Cod
                x_Cod &= "$" & objParametriUscita.ListaFertilizzanti(i).Udm_Cod

                ddl_Fertilizzanti.Items.Add(New ListItem(x_Des, x_Cod))

            End If
        Next

        N_Fertilizzanti = ddl_Fertilizzanti.Items.Count - 1

    End Sub







#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If (Bootstrap = False) Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Fertilizzanti.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        Else
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Fertilizzanti.ClientID & "').parent().find('button').click();")
            StrSelect.AppendLine("});")
        End If

        Return StrSelect.ToString
    End Function
#End Region
End Class
