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
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboSementi")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("ComboSementi_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboSementi runat=server></{0}:ComboSementi>")> Public Class ComboSementi
    Inherits System.Web.UI.WebControls.WebControl


    Public ddl_Sementi As DropDownList
    Private _Bootstrap As Boolean
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _TipoRichiesto As Integer
    Private _TestoRicerca As String
    Private _MatCod As Integer
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

    Private _Ricerca As Boolean = True

    Private _GestioneLotto As enum_Gestione_Lotti
    Private _GestioneGiacenze As enum_Gestione_Giacenze
    Private _EscludiGiacenzeZero As Boolean

    Public Sub New()

        _Bootstrap = False
        _PrimaRiga_Flag = True
        _PrimaRiga_Text = ""
        _PrimaRiga_Value = ""
        _TipoRichiesto = 0
        _TestoRicerca = ""
        _MatCod = 0
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
        _GestioneLotto = enum_Gestione_Lotti.Nessuna
        _GestioneGiacenze = enum_Gestione_Giacenze.SoloMovimentati
        _EscludiGiacenzeZero = True

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Sementi = New DropDownList
        ddl_Sementi.ID = Me.ClientID & "ComboSementi"

        If _Bootstrap = False Then
            ddl_Sementi.CssClass = "ComboSementi txtUI"
            ddl_Sementi.Attributes.Add("style", " min-width: 525px;width: 90%; ")
            ddl_Sementi.Attributes.Add("onchange", " CambiaSementi(); ")
        Else
            ddl_Sementi.CssClass = "selectpicker ComboSementi"
            If _Ricerca = True Then
                ddl_Sementi.Attributes.Add("data-live-search", "true")
            End If

            ddl_Sementi.Attributes.Add("data-container", "body")
            ddl_Sementi.Attributes.Add("onchange", " CambiaSementi(); ")
        End If


        Me.Controls.Add(ddl_Sementi)
        MyBase.OnInit(e)

    End Sub


#Region "Proprietà"

    Public Property Lav_Cod As Integer = 0
    Public Property Veg_Cod As Integer = 0

    Public Property SeminaMultiSpecie As Boolean = False
    Public Property SoloGiacenzePositive As Boolean = True

    Public Property Lotto As String = LOTTO_NONDEFINITO

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

    Public Property MatCod() As Integer
        Get
            Return _MatCod
        End Get
        Set(ByVal value As Integer)
            _MatCod = value
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
            Return ddl_Sementi.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Sementi.Items.FindByValue(value)) Then
                    ddl_Sementi.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            If Not IsNothing(ddl_Sementi.SelectedItem) Then
                Return ddl_Sementi.SelectedItem.Text
            Else
                Return Nothing
            End If

        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Sementi.Items.FindByText(value)) Then
                    ddl_Sementi.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property N_Sementi() As Integer
        Get
            Return ddl_Sementi.Items.Count - 1
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






    Public Sub CaricaComboSementi()

        Dim strGiacenza As String = ""
        Dim Dt_Giacenze As New DataTable
        Dim Dt_Giacenze_Tot As New DataTable
        Dim DrGiacenze() As DataRow
        Dim DrGiacenze_Tot() As DataRow

        Dim flagQtaMaggioreZero As Boolean = False
        Dim utilizzaLotto = False

        Dim objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        ddl_Sementi.Items.Clear()

        If _PrimaRiga_Flag = True Then
            ddl_Sementi.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
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

        Dim filtroTipologiaSementi As String = ""
        Select Case Lav_Cod
            Case LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                filtroTipologiaSementi = " AND Materie_Prime.Sem_Cod IN (1,3,7) "
            Case LAVCOD_TRAPIANTO
                filtroTipologiaSementi = " AND Materie_Prime.Sem_Cod IN (2,3,4,5,6,7,8,10) "
        End Select

        Dim filtroSpecie As String = ""
        If SeminaMultiSpecie = False Then
            filtroSpecie = " AND Materie_Prime.Veg_Cod = " & Veg_Cod & " "
        End If

        Dim filtroRicerca As String = ""
        If TestoRicerca <> "" Then
            filtroRicerca = " AND Materie_Prime.Mat_Des Like '%" & TestoRicerca & "%' "
        End If

        Select Case _Fabbricato_Cod

            Case "0"

                'Senza Magazzino
                Dim Dt_TS As DataTable = New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R().Leggi(
                                            Veg_Cod, 0,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                            filtroTipologiaSementi.Replace("AND Materie_Prime", "TipologieSementi"),
                                            "", HttpContext.Current.Session("ASG_objParametri_Server"))

                If IsNothing(Dt_TS) OrElse (Dt_TS.Rows.Count = 0 AndAlso Lav_Cod <> LAVCOD_SOVESCIO) Then
                    'Throw New Exception("Nessuna tipologia di semente/piantina trovata per questa specie")
                    Exit Sub
                End If

                For Each dr As DataRow In Dt_TS.Rows
                    Dim x_Cod As String = CStr(dr.Item("Sem_Cod")) & "§" & CStr(dr.Item("Veg_Des")) & " (" & CStr(dr.Item("Sem_Des")) & ")" & "§" & CStr(dr.Item("Veg_Cod"))
                    Dim x_Des As String = CStr(dr.Item("Veg_Des") & " (" & CStr(dr.Item("Sem_Des")) & ")")
                    ddl_Sementi.Items.Add(New ListItem(x_Des, x_Cod))
                Next

            Case Else

                'Con Magazzino

                Dim objG As New AgronicaCoreContabDAL.Giacenze_R

                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Validita_Fine,
                                                            Split(_Fabbricato_Cod, "|")(2),
                                                            Split(_Fabbricato_Cod, "|")(1),
                                                            Split(_Fabbricato_Cod, "|")(0),
                                                            SEMENTI,
                                                            0,
                                                            MatCod,
                                                            0, 0, 0, 0, Lotto,
                                                            _EscludiGiacenzeZero,
                                                            "", "", "", "", "", "", "", filtroTipologiaSementi & filtroSpecie & filtroRicerca, "", "", "",
                                                            "",
                                                            objParametriServer, objParametriUtenti,
                                                            Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

                Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                            Split(_Fabbricato_Cod, "|")(2),
                                                            Split(_Fabbricato_Cod, "|")(1),
                                                            Split(_Fabbricato_Cod, "|")(0),
                                                            SEMENTI,
                                                            0,
                                                            MatCod,
                                                            0, 0, 0, 0, Lotto,
                                                            _EscludiGiacenzeZero,
                                                            "", "", "", "", "", "", "", filtroTipologiaSementi & filtroSpecie & filtroRicerca, "", "", "",
                                                            "",
                                                            objParametriServer, objParametriUtenti,
                                                            Flag_QtaMaggioreZero:=flagQtaMaggioreZero)
                objG = Nothing

                'filtro Materie prime per dettagli
                Dim strFiltroMatCod As String = ""
                For Each dr As DataRow In Dt_Giacenze.Rows

                    'Anna 10/05/2022: nascoste migrogiacenze SE
                    '                                   impostazione utente = Solo Presenti (> 0 e QTA_GiancenzeVisualizzate)
                    '                                        O SE
                    '                                   se visualizza giacenze zero = non checkato e impostazione utente = tutti i movimenti
                    '[rif chiamata 17160]
                    Dim filtroGiacenze = (dr.Item("Giacenza") > QTA_GiancenzeVisualizzate Or dr.Item("Giacenza") < -QTA_GiancenzeVisualizzate)
                    If _EscludiGiacenzeZero = False Then
                        If (flagQtaMaggioreZero) = False Then
                            strFiltroMatCod &= " Materie_Prime.Mat_Cod=" & dr.Item("Mat_Cod").ToString & " OR "
                        ElseIf flagQtaMaggioreZero = True And filtroGiacenze = True Then
                            strFiltroMatCod &= " Materie_Prime.Mat_Cod=" & dr.Item("Mat_Cod").ToString & " OR "
                        End If
                    Else
                        If (filtroGiacenze) Then
                            strFiltroMatCod &= " Materie_Prime.Mat_Cod=" & dr.Item("Mat_Cod").ToString & " OR "
                        End If
                    End If
                    'strFiltroMatCod &= " Materie_Prime.Mat_Cod=" & dr.Item("Mat_Cod").ToString & " OR "
                Next

                If strFiltroMatCod.Length > 0 Then
                    strFiltroMatCod = "(" & Left(strFiltroMatCod, strFiltroMatCod.Length - 3) & ")"
                End If



                Dim Dt_Prodotti As New DataTable


                If strFiltroMatCod <> "" Then
                    Dim objSementi As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dt_Prodotti = objSementi.Leggi(Split(_Fabbricato_Cod, "|")(2), 0, SEMENTI, 0, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", False, False, "",
                                                   enumSelezioneVariabile.Selezione_JoinDescrizioni, strFiltroMatCod, "",
                                                   HttpContext.Current.Session("ASG_objParametri_Server"))

                    objSementi = Nothing
                End If

                '------------------- 
                Dim Dt As DataTable = Dt_Prodotti.Copy()

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                    'uso il dataview per ordinare 
                    Dt.TableName = "Prodotti"
                    Dim Dv As New DataView(Dt)
                    Dv.Sort = "Mat_Des ASC"

                    'Dim isnothingMatCod As Boolean = IsNothing(Dv.ToTable.Columns.Item("Mat_Cod"))

                    For i = 0 To Dv.Count - 1

                        'If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then
                        DrGiacenze = Dt_Giacenze.Select("mat_cod=" & Dv(i).Item("mat_cod"))
                        If Not IsNothing(DrGiacenze) Then
                            For Each dr As DataRow In DrGiacenze

                                Dim x_Cod As String = ""
                                Dim x_Des As String = CStr(Dv(i).Item("Mat_Des"))

                                If Not IsDBNull(Dv(i).Item("Cod_Articolo")) AndAlso Not String.IsNullOrEmpty(Dv(i).Item("Cod_Articolo")) Then
                                    x_Des &= " (" & My.Resources.AgronicaControlli_2010.CodArticolo & ": " & Dv(i).Item("Cod_Articolo") & ")"
                                End If

                                If dr.Item("Lotto") <> "" Then
                                    x_Des &= "(Lotto: " & dr.Item("Lotto") & ")"
                                    x_Des &= "(" & My.Resources.AgronicaControlli_2010.Lotto & ": " & dr.Item("Lotto") & ")"
                                End If

                                x_Des &= " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(dr.Item("Giacenza"), 4).ToString & " " & dr.Item("Udm_Sim")

                                DrGiacenze_Tot = Dt_Giacenze_Tot.Select("mat_cod=" & Dv(i).Item("mat_cod") & "And lotto='" & UtilityProvider.Agro_SQL_SaveText(dr.Item("Lotto")) & "'")

                                If Not IsNothing(DrGiacenze_Tot) AndAlso DrGiacenze_Tot.Length > 0 Then
                                    x_Cod = CStr(Dv(i).Item("Mat_Cod")) & "§" & CStr(Dv(i).Item("Mat_Des")) & "§" & CStr(Dv(i).Item("Cod_Articolo")) & "§" & CStr(dr.Item("Lotto")) & "§" &
                                            CStr(Dv(i).Item("Regolamento")) & "§" & CStr(Dv(i).Item("Veg_cod")) & "§" & CStr(Dv(i).Item("Cul_cod")) & "§" & Math.Round(dr.Item("Giacenza"), 4).ToString & "§" &
                                            Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & "§" & dr.Item("Udm_Cod").ToString() & "§" & dr.Item("Udm_Sim").ToString()
                                    x_Des &= ", " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                    ddl_Sementi.Items.Add(New ListItem(x_Des, x_Cod))
                                End If

                            Next
                        End If

                    Next
                End If
                Dt = Nothing

        End Select

        N_Sementi = ddl_Sementi.Items.Count - 1

    End Sub

#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If (Bootstrap = False) Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Sementi.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        Else
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Sementi.ClientID & "').parent().find('button').click();")
            StrSelect.AppendLine("});")
        End If

        Return StrSelect.ToString
    End Function
#End Region
End Class
