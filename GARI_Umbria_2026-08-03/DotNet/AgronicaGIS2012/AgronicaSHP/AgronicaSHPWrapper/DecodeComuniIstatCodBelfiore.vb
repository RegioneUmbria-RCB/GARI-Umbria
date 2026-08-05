Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class DecodeComuniIstatCodBelfiore
    Private _PROV As String
    Private _COM As String
    Private _Belfiore As String
    Public Property Belfiore() As String
        Get
            Return _Belfiore
        End Get
        Set(value As String)
            _Belfiore = value
        End Set
    End Property
    Public Property COM() As String
        Get
            Return _COM
        End Get
        Set(value As String)
            _COM = value
        End Set
    End Property
    Public Property PROV() As String
        Get
            Return _PROV
        End Get
        Set(value As String)
            _PROV = value
        End Set
    End Property


End Class

Public Class DecodeComuniIstatCodBelfiore_controller

    Private _objParametri_Server As AgronicaCoreParametri

    Private _CacheListaComuni As List(Of DecodeComuniIstatCodBelfiore)

    Public Sub New(objParametri_Server As AgronicaCoreParametri)
        _objParametri_Server = objParametri_Server
        _CacheListaComuni = New List(Of DecodeComuniIstatCodBelfiore)
    End Sub


    Public Sub LeggiDecodeBelfioreDaDB(ByVal codBelfiore As String, ByRef Prov As String, ByRef com As String)


        Dim l1 As DecodeComuniIstatCodBelfiore = (
            From i In _CacheListaComuni
            Where i.Belfiore = codBelfiore
        ).FirstOrDefault

        If l1 Is Nothing Then

            Dim leggi As New AgronicaCoreMetaSchemaDAL.ISTAT_Comuni_R
            Dim dt As DataTable =
            leggi.Leggi(
                "",
                "",
                "",
                "",
                "",
                codBelfiore,
                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                "",
                "",
                _objParametri_Server
                )

            If dt.Rows.Count > 0 Then
                _CacheListaComuni.Add(
                New DecodeComuniIstatCodBelfiore With {
                    .Belfiore = codBelfiore,
                    .PROV = dt.Rows(0)("Pro_Cod_Istat"),
                    .COM = dt.Rows(0)("COM_Cod_Istat")
            })
                Prov = dt.Rows(0)("Pro_Cod_Istat")
                com = dt.Rows(0)("COM_Cod_Istat")
            End If

        Else

            Prov = l1.PROV
            com = l1.COM

        End If

    End Sub
End Class
