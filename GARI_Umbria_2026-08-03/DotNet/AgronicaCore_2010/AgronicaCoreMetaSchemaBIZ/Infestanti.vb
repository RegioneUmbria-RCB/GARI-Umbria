Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Infestanti_W
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

Public Class Infestanti_R

    Public Sub New()
    End Sub

    Public Function ErbeInfestanti(ByVal Input As Infestanti_input,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Infestanti_output

        Dim Output As New Infestanti_output
        Dim Elemento_InfestantiAttive As InfestantiAttive
        Dim Elemento_GruppoInfestantiAttive As GruppoInfestantiAttive

        Dim objGruppoAvversitaAttive As New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R
        Dim objInfestantiAttive As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R


        Dim DtInfestantiAttive As New DataTable
        Dim DtGruppoAvversitaAttive As New DataTable

        DtGruppoAvversitaAttive = objGruppoAvversitaAttive.Leggi(Input.AV_GRU,
                                                                 0,
                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                 Input.strFiltro,
                                                                 Input.strOrdinamento,
                                                                 objParametri)

        DtInfestantiAttive = objInfestantiAttive.Leggi(Input.AV_COD,
                                                       Input.AV_GRU,
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       Input.strFiltro,
                                                       Input.strOrdinamento,
                                                       objParametri)

        Dim HashInseriti As New Hashtable

        For Each dr As DataRow In DtInfestantiAttive.Rows
            If Not HashInseriti.ContainsKey(dr.Item("Av_Gru") & "-" & dr.Item("Av_Cod")) Then
                Elemento_InfestantiAttive = New InfestantiAttive With {
                    .AV_GRU = dr.Item("Av_Gru"),
                    .AV_COD = dr.Item("Av_Cod"),
                    .Av_Des_Vol = dr.Item("Av_Des_Vol")
                }

                Output.InfestantiAttive.Add(Elemento_InfestantiAttive)

                HashInseriti.Add(dr.Item("Av_Gru") & "-" & dr.Item("Av_Cod"), "")
            End If
        Next

        For Each dr As DataRow In DtGruppoAvversitaAttive.Rows
            If Not HashInseriti.ContainsKey(dr.Item("Av_Gru")) Then
                Elemento_GruppoInfestantiAttive = New GruppoInfestantiAttive With {
                    .AV_GRU = dr.Item("Av_Gru"),
                    .Av_Gru_Des = dr.Item("Av_Gru_Des")
                }

                Output.GruppoInfestantiAttive.Add(Elemento_GruppoInfestantiAttive)

                HashInseriti.Add(dr.Item("Av_Gru"), "")
            End If
        Next

        Return Output

    End Function

End Class

Public Class Infestanti_input

    Public AV_COD As Integer
    Public AV_GRU As Integer

    Public strFiltro As String
    Public strOrdinamento As String

    Public Url As String

    Public Lingua_Cod As Integer

    Sub New()
        AV_GRU = 0
        AV_COD = 0
        strFiltro = ""
        strOrdinamento = ""
        Url = ""
        Lingua_Cod = AgronicaCoreDataProvider.TipiEnumerativi.enum_AgroLingue.Italiano_it
    End Sub

End Class

Public Class Infestanti_output

    Public InfestantiAttive As List(Of InfestantiAttive)
    Public GruppoInfestantiAttive As List(Of GruppoInfestantiAttive)
    Public MessaggioErrore As String

    Public Sub New()
        InfestantiAttive = New List(Of InfestantiAttive)
        GruppoInfestantiAttive = New List(Of GruppoInfestantiAttive)
        MessaggioErrore = ""
    End Sub

End Class

Public Class GruppoInfestantiAttive

    Public AV_GRU As Integer
    Public Av_Gru_Des As String

    Sub New()
        AV_GRU = 0
        Av_Gru_Des = ""
    End Sub

End Class

Public Class InfestantiAttive

    Public AV_COD As Integer
    Public AV_GRU As Integer
    Public Av_Des_Vol As String
    Public Av_Gru_Des As String

    Sub New()
        AV_COD = 0
        AV_GRU = 0
        Av_Des_Vol = ""
        Av_Gru_Des = ""
    End Sub

End Class




