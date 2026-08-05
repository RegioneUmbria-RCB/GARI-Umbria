Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class IndiciMaturita_W
    Inherits AgronicaCoreDataProvider.LogProvider


#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property

    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

End Class

Public Class IndiciMaturita_R

    Public Sub New()
    End Sub

    Public Function IndiciMaturita(ByVal Input As IndiciMaturita_input,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As IndiciMaturita_output

        Dim Output As New IndiciMaturita_output
        Dim Elemento As IndiciMaturita

        Dim objCore As New AgronicaCoreMetaSchemaDAL.IndiciMaturita_R

        If IsNothing(Input.lav_cod) Then
            Input.lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA
        End If

        Dim veg_cod As Integer = Input.veg_cod

        Dim NoSpecie As Boolean = False

        If veg_cod < 0 Then
            NoSpecie = True
        End If

        Dim Dt As DataTable = objCore.Leggi_WS(veg_cod,
                                               Input.ind_mat_cod,
                                               Input.udm_cod,
                                               Input.tipoTestata,
                                               Input.strFiltro,
                                               Input.strOrdinamento,
                                               objParametri,
                                               Input.Personalizzate,
                                               Input.Piva_Superuser,
                                               Input.lav_cod,
                                               NoSpecie,
                                               Input.FiltraSpecie,
                                               Input.joinPersonalizzate,
                                               Input.estraiPersonalizzatePerAPP)

        Dim HashInseriti As New Hashtable

        For Each dr As DataRow In Dt.Rows

            If Not HashInseriti.ContainsKey(dr.Item("ind_mat_cod") & "-" & dr.Item("veg_cod") & "-" & dr.Item("udm_cod")) OrElse
                Input.EstraiPersonalizzatePerAPP = True Then

                Elemento = New IndiciMaturita

                Elemento.ind_mat_cod = dr.Item("ind_mat_cod")
                Elemento.ind_mat_des = dr.Item("ind_mat_des")
                Elemento.veg_cod = dr.Item("veg_cod")
                Elemento.veg_des = dr.Item("veg_des")

                Elemento.gru_cod = dr.Item("gru_cod")
                Elemento.grsp_cod = dr.Item("grsp_cod")
                Elemento.udm_cod = dr.Item("udm_cod")

                Elemento.udm_des = dr.Item("Udm_Des")
                Elemento.udm_sim = dr.Item("Udm_Sim")

                Elemento.reg_cod = dr.Item("reg_cod")
                Elemento.classe = dr.Item("classe")
                Elemento.flag_raccolta = dr.Item("flag_raccolta")

                Elemento.lav_cod = dr.Item("lav_cod")

                If Input.EstraiPersonalizzatePerAPP Then
                    If Not IsDBNull(dr.Item("Piva_superUser")) Then
                        Elemento.pivasuperuser = dr.Item("Piva_superUser")
                    End If
                End If

                Output.ListaIndiciMaturita.Add(Elemento)

                If Not HashInseriti.ContainsKey(dr.Item("ind_mat_cod") & "-" & dr.Item("veg_cod") & "-" & dr.Item("udm_cod")) Then
                    HashInseriti.Add(dr.Item("ind_mat_cod") & "-" & dr.Item("veg_cod") & "-" & dr.Item("udm_cod"), "")
                End If
            End If

        Next

        Return Output

    End Function

End Class

Public Class IndiciMaturita_input

    Public ind_mat_cod As Integer
    Public udm_cod As Integer
    Public veg_cod As Integer
    Public tipoTestata As Integer ' INDICI_MATURITA = 0 - INDICI_RESE_RACCOLTA = 1

    Public lav_cod As Integer

    Public strFiltro As String
    Public strOrdinamento As String

    Public Personalizzate As Boolean
    Public Piva_Superuser As String

    Public Url As String

    Public Lingua_Cod As Integer

    Public FiltraSpecie As Boolean
    Public joinPersonalizzate As Boolean

    Public EstraiPersonalizzatePerAPP As Boolean

    Sub New()
        veg_cod = 0
        ind_mat_cod = 0
        tipoTestata = 0
        lav_cod = 0
        strFiltro = ""
        strOrdinamento = ""
        Personalizzate = False
        Piva_Superuser = ""
        Url = ""
        Lingua_Cod = AgronicaCoreDataProvider.TipiEnumerativi.enum_AgroLingue.Italiano_it
        FiltraSpecie = True
        joinPersonalizzate = True
        EstraiPersonalizzatePerAPP = False
    End Sub

End Class

Public Class IndiciMaturita_output

    Public ListaIndiciMaturita As List(Of IndiciMaturita)
    Public MessaggioErrore As String

    Public Sub New()
        ListaIndiciMaturita = New List(Of IndiciMaturita)
        MessaggioErrore = ""
    End Sub

End Class

Public Class IndiciMaturita

    Public ind_mat_cod As Integer
    Public ind_mat_des As String
    Public veg_cod As Integer
    Public veg_des As String
    Public gru_cod As Integer
    Public grsp_cod As Integer
    Public udm_cod As Integer
    Public udm_des As String
    Public udm_sim As String
    Public reg_cod As Integer
    Public classe As String
    Public flag_raccolta As Integer
    Public lav_cod As Integer
    Public pivasuperuser As String

    Sub New()

        ind_mat_cod = 0
        ind_mat_des = ""
        veg_cod = 0
        veg_des = ""
        gru_cod = 0
        grsp_cod = 0
        udm_cod = 0
        udm_des = ""
        udm_sim = ""
        reg_cod = 0
        classe = ""
        flag_raccolta = 0
        lav_cod = 0
        pivasuperuser = ""
    End Sub

End Class




