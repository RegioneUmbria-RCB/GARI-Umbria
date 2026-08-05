
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Namespace DSSIrrigazione


    Public Class IrrigazioneShared

        Public Sub New()

        End Sub

        Public Function ImpiantiIrrigabili(objParametri_Server As AgronicaCoreParametri, fertiDetails As Boolean, Optional ByVal sa_cod As Integer = 0, Optional ByVal appezza As Integer = 0, Optional ByVal id_reg As Integer = 0) As List(Of Centro)

            Dim objParametriAgenda As New ParametriAgenda

            Dim objRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DT As DataTable = objRead.Leggi_x_DSS_Irrigazione_V2(objParametriAgenda.Piva, sa_cod, appezza, id_reg, fertiDetails, objParametri_Server)

            For Each column As DataColumn In DT.Columns
                If column.ColumnName.StartsWith("GruppoVegetale_XLingue") Then
                    column.ColumnName = ("GruppoVegetale")
                End If
            Next

            Dim Json = JsonConvert.SerializeObject(DT)

            Dim ImpiantiList As List(Of Irriframe.ImpiantiIrrigabili) =
                JsonConvert.DeserializeObject(Of List(Of Irriframe.ImpiantiIrrigabili))(Json, New IsoDateTimeConverter() With {.DateTimeFormat = "yyyy-MM-dd"})

            Dim groupByCampi As Boolean = False

            Dim objLetturaImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim dtImpostazione = objLetturaImpreseImpostazioni.Leggi(
                    objParametriAgenda.Piva,
                    SACOD_NOFILTRO,
                    enum_Impostazioni_Utenti.DSS_IRRIGAZIONE_RAGGRUPPAMENTO_X_CAMPI,
                    "",
                    "",
                    objParametri_Server)

            If dtImpostazione IsNot Nothing AndAlso dtImpostazione.Rows.Count > 0 Then
                Try
                    Dim iVal As Integer

                    If Not Integer.TryParse(dtImpostazione.Rows(0).Field(Of String)("Impostazione_Valore"), iVal) Then
                        iVal = 0
                    End If

                    groupByCampi = iVal = 1

                Catch ex As Exception

                End Try
            End If

            Dim centriList = GestisciImpiantiIrrigabili(ImpiantiList, groupByCampi)

            Return centriList
        End Function

        Private Function GestisciImpiantiIrrigabili(impIrriList As List(Of Irriframe.ImpiantiIrrigabili), groupByCampi As Boolean) As List(Of Centro)

            Dim centriList As New List(Of Centro)

            For Each ii In impIrriList

                Dim idxCentro = centriList.FindIndex(Function(elem) elem.IdCentro = ii.Id_Centro)

                If idxCentro < 0 Then
                    centriList.Add(New Centro With {
                               .IdCentro = ii.Id_Centro,
                               .DesCentro = ii.Centro,
                               .Campi = New List(Of Campo),
                               .Indicatori = New List(Of Irriframe.ImpiantiIrrigabili)
                               })
                    idxCentro = centriList.Count - 1
                End If

                If groupByCampi Then
                    Dim idxCampo As Integer = -1

                    If ii.Id_Campo > 0 Then

                        idxCampo = centriList(idxCentro).Campi.FindIndex(Function(elem) elem.IdCampo = ii.Id_Campo AndAlso elem.IdCrop = ii.Irri_Veg_Cod)
                    End If

                    If idxCampo < 0 Then

                        centriList(idxCentro).Campi.Add(New Campo With {
                                                .IdCampo = ii.Id_Campo,
                                                .DesCampo = ii.Campo,
                                                .IdCrop = ii.Irri_Veg_Cod,
                                                .DesCrop = ii.Coltura,
                                                .Indicatori = New List(Of Irriframe.ImpiantiIrrigabili) From {ii}
                                                })
                    Else

                        centriList(idxCentro).Campi(idxCampo).Indicatori.Add(ii)
                    End If
                Else

                    centriList(idxCentro).Indicatori.Add(ii)
                End If

            Next

            centriList.Sort(New CentriComparer)

            For Each c In centriList
                c.Campi.Sort(New CampoComparer)
                c.Indicatori.Sort(New Irriframe.ImpiantiIrrigabiliComparer)

                For Each p In c.Campi
                    p.Indicatori.Sort(New Irriframe.ImpiantiXCampiIrrigabiliComparer)
                Next
            Next

            Return centriList
        End Function

    End Class




    Public Class Centro
        Public IdCentro As Integer
        Public DesCentro As String
        Public Campi As List(Of Campo)
        Public Indicatori As List(Of Irriframe.ImpiantiIrrigabili)
    End Class

    Public Class CentriComparer
        Implements IComparer(Of Centro)

        Public Function Compare(x As Centro, y As Centro) As Integer Implements IComparer(Of Centro).Compare
            Return x.DesCentro.ToUpper().CompareTo(y.DesCentro.ToUpper())
        End Function
    End Class




    Public Class Campo
        Public IdCampo As Integer
        Public DesCampo As String
        Public IdCrop As Integer
        Public DesCrop As String
        Public Indicatori As List(Of Irriframe.ImpiantiIrrigabili)
    End Class

    Public Class CampoComparer
        Implements IComparer(Of Campo)

        Public Function Compare(x As Campo, y As Campo) As Integer Implements IComparer(Of Campo).Compare

            Dim res As Integer = x.DesCampo.ToUpper().CompareTo(y.DesCampo.ToUpper())

            If res = 0 Then

                res = x.DesCrop.ToUpper().CompareTo(y.DesCrop.ToUpper())
            End If

            Return res
        End Function
    End Class


End Namespace

