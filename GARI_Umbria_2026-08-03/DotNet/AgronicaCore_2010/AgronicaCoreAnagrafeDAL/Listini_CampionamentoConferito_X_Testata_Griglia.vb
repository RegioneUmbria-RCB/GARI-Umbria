Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Listini_CampionamentoConferito_X_Testata_Griglia_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggiPuntuale(ByVal piva As String,
                          ByVal listino_Cod As Integer,
                          ByVal id_TestataGriglia As Integer,
                          ByVal validita_Inizio As DateTime,
                          ByVal validita_Fine As DateTime,
                          ByRef objParametri As AgronicaCoreParametri,
                          ByRef Gias_Context As Gias_DeveloperServer_Entities) As Listini_CampionamentoConferito_X_Testata_Griglia


        Const nomeRoutine = "Listini_CampionamentoConferito_X_Testata_Griglia_R.LeggiPuntuale()"
        Dim risposta As Listini_CampionamentoConferito_X_Testata_Griglia = Nothing
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim messaggioErrore As String = String.Empty
        Dim Connessione As Gias_DeveloperServer_Entities = Nothing

        Try

            If Gias_Context IsNot Nothing Then
                Connessione = Gias_Context
            Else
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
                Connessione = New Gias_DeveloperServer_Entities(EFConnString)
            End If

            Using Connessione

                risposta = (From lctg In Connessione.Listini_CampionamentoConferito_X_Testata_Griglia
                            Where lctg.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                  lctg.PIVA.Equals(piva) AndAlso
                                  lctg.Listino_Cod.Equals(listino_Cod) AndAlso
                                  lctg.Id_TestataGriglia.Equals(id_TestataGriglia) AndAlso
                                  lctg.Validita_Inizio.Equals(validita_Inizio) AndAlso
                                  lctg.Validita_Fine.Equals(validita_Fine)).FirstOrDefault
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta


    End Function

    Public Function Leggi(ByVal piva As String,
                          ByVal listino_Cod As Integer?,
                          ByVal id_TestataGriglia As Integer?,
                          ByVal validita_Inizio As DateTime?,
                          ByVal validita_Fine As DateTime?,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal DataRif As String = ""
                        ) As String

        Const nomeRoutine = "Listini_CampionamentoConferito_X_Testata_Griglia_R.Leggi()"
        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim messaggioErrore As String = String.Empty

        Dim DataRifDateTime As Nullable(Of DateTime)
        DataRifDateTime = Nothing
        If Not String.IsNullOrEmpty(DataRif) Then
            DataRifDateTime = Convert.ToDateTime(DataRif)
        End If

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim listiniXGriglie = (From lctg In GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia
                                       Group Join lp In GiasContext.Listini_Prezzi
                                    On lctg.PIVA Equals lp.Piva And lctg.Listino_Cod Equals lp.Listino_Cod
                                    Into lp_group = Group
                                       From _lp_group In lp_group.DefaultIfEmpty()
                                       Group Join tg In GiasContext.CampionamentoConferito_TestataGriglia
                                    On lctg.PIVA Equals tg.PIVA And lctg.Id_TestataGriglia Equals tg.Id_TestataGriglia
                                    Into tg_group = Group
                                       From _tg_group In tg_group.DefaultIfEmpty()
                                       Where
                                           (lctg.PIVA.Equals(piva)) And
                                           (DataRifDateTime Is Nothing Or ((_lp_group.Validita_Inizio <= DataRifDateTime And _lp_group.Validita_Fine >= DataRifDateTime) And
                                                                            (_tg_group.Validita_Inizio <= DataRifDateTime And _tg_group.Validita_Fine >= DataRifDateTime)))
                                       Select New With
                                        {
                                            .Key = "",
                                            .Piva = lctg.PIVA,
                                            .Listino_Cod = lctg.Listino_Cod,
                                            .Listino_Des = _lp_group.Listino_Des,
                                            .Id_TestataGriglia = lctg.Id_TestataGriglia,
                                            .des_TestataGriglia = _tg_group.des_TestataGriglia,
                                            .Validita_Inizio = lctg.Validita_Inizio,
                                            .Validita_Fine = lctg.Validita_Fine,
                                            .Validita_Inizio_Listini = _lp_group.Validita_Inizio,
                                            .Validita_Fine_Listini = _lp_group.Validita_Fine,
                                            .Validita_Inizio_TestataGriglie = _tg_group.Validita_Inizio,
                                            .Validita_Fine_TestataGriglie = _tg_group.Validita_Fine
                                        }).ToList

                listiniXGriglie.ForEach(Function(f)
                                            Dim chiave = String.Format("{0}_{1}_{2}_{3}", Piva_SuperUser, f.Piva, f.Listino_Cod.ToString(), f.Id_TestataGriglia.ToString())
                                            f.Key = chiave
                                        End Function)


                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(listiniXGriglie.ToList(), Formatting.None, serializerSettings)

            End Using
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = String.Empty
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

    Public Function LeggiListiniAssociatiCampionamento(ByVal piva As String, ByVal id_TestataGriglia As Integer, ByRef objParametri As AgronicaCoreParametri) As List(Of Listini_CampionamentoConferito_X_Testata_Griglia)
        Const nomeRoutine = "Listini_CampionamentoConferito_X_Testata_Griglia_R.LeggiListiniAssociatiCampionamento()"
        Dim risposta As List(Of Listini_CampionamentoConferito_X_Testata_Griglia) = Nothing
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim messaggioErrore As String = String.Empty
        Dim Connessione As Gias_DeveloperServer_Entities = Nothing

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Connessione = New Gias_DeveloperServer_Entities(EFConnString)

            Using Connessione

                risposta = (From lctg In Connessione.Listini_CampionamentoConferito_X_Testata_Griglia
                            Where lctg.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                                  lctg.PIVA.Equals(piva) AndAlso
                                  lctg.Id_TestataGriglia.Equals(id_TestataGriglia)).ToList()
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta
    End Function

End Class


Public Class Listini_CampionamentoConferito_X_Testata_Griglia_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Inserisci(ByVal piva As String,
                                ByVal elemento As Listini_CampionamentoConferito_X_Testata_Griglia,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As String

        Const nomeRoutine = "Listini_CampionamentoConferito_X_Testata_Griglia_W.Inserisci()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    elemento.Piva_SuperUser = objParametri.PivaSuperUser
                    elemento.PIVA = piva
                    elemento.Data_Creazione = Date.Now
                    elemento.Username_Creazione = objParametri.UsernameOperazione
                    elemento.Data_Modifica = Date.Now
                    elemento.Username_Modifica = objParametri.UsernameOperazione
                    elemento.Validita_Inizio = AGRODATAINIZIO
                    elemento.Validita_Fine = AGRODATAFINE

                    GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Add(elemento)

                    GiasContext.SaveChanges()

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function Elimina(ByVal piva As String,
                        ByVal elemento As Listini_CampionamentoConferito_X_Testata_Griglia,
                        ByRef objParametri As AgronicaCoreParametri
                        ) As String


        Const nomeRoutine = "Listini_CampionamentoConferito_X_Testata_Griglia_W.Elimina()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            If elemento IsNot Nothing Then
                GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Attach(elemento)
                GiasContext.Listini_CampionamentoConferito_X_Testata_Griglia.Remove(elemento)
                GiasContext.SaveChanges()
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

End Class
