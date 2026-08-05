Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json

Public Class FF_CampionamentoConferito_TestataGriglia_R : Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal id_TestataGriglia As Integer?,
                          ByVal des_TestataGriglia As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal DataRif As String = ""
                           ) As String

        Const nomeRoutine = "FF_CampionamentoConferito_TestataGriglia_R.Leggi()"
        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim messaggioErrore = String.Empty

        Dim DataRifDateTime As Nullable(Of DateTime)
        DataRifDateTime = Nothing
        If Not String.IsNullOrEmpty(DataRif) Then
            DataRifDateTime = Convert.ToDateTime(DataRif)
        End If

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim testate = From tg In GiasContext.CampionamentoConferito_TestataGriglia
                              Where
                                  (tg.PIVA.Equals(piva)) And
                              (DataRifDateTime Is Nothing Or (tg.Validita_Inizio <= DataRifDateTime And tg.Validita_Fine >= DataRifDateTime))
                              Order By tg.des_TestataGriglia

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(testate.ToList(), Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = String.Empty
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

End Class

Public Class FF_CampionamentoConferito_TestataGriglia_W : Inherits AgronicaCoreDataProvider.DataProvider



End Class
