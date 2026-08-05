Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json


Public Class Configurazione_Imballaggi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal idConfig As Integer,
                          ByVal piva As String,
                          ByVal tabellaCod As Integer,
                          ByVal matCod As Integer,
                          ByVal vegCod As Integer,
                          ByVal culCod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        'ByVal tipoConfig As Integer,
        'ByVal moduloGenerazione As Integer,
        'ByVal tabellaParCod As Integer,

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Configurazione_Imballaggi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Configurazione_Imballaggi ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If idConfig <> 0 Then
                strSql.AppendLine(" AND Id_Config = " & Agro_SQL_SaveNum(idConfig) & " ")
            End If

            If piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If tabellaCod <> 0 Then
                strSql.AppendLine(" AND Tabella_Cod = " & Agro_SQL_SaveNum(tabellaCod) & " ")
            End If

            'If tipoConfig <> 0 Then
            '    strSql.AppendLine(" AND Tipo_Config = " & Agro_SQL_SaveNum(tipoConfig) & " ")
            'End If

            If matCod <> 0 Then
                strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(matCod) & " ")
            End If

            'If moduloGenerazione <> 0 Then
            '    strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(moduloGenerazione) & " ")
            'End If

            'If tabellaParCod <> 0 Then
            '    strSql.AppendLine(" AND Tabella_Par_Cod = " & Agro_SQL_SaveNum(tabellaParCod) & " ")
            'End If

            If vegCod <> 0 Then
                strSql.AppendLine(" AND Veg_Cod = " & Agro_SQL_SaveNum(vegCod) & " ")
            End If

            If culCod <> 0 Then
                strSql.AppendLine(" AND Cul_Cod = " & Agro_SQL_SaveNum(culCod) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_Configurazione_Imballaggi(ByVal piva As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Configurazione_Imballaggi_R.Leggi_Configurazione_Imballaggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim strElem_Cod As String = "205, 305"

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim TestataElem =
               From CI In GiasContext.Configurazione_Imballaggi
               Join Materie_Prime In GiasContext.Materie_Prime
                 On Materie_Prime.Mat_Cod Equals CI.Mat_Cod
               Join oTabelle In GiasContext.OTabelle
                 On oTabelle.Tabella_Cod Equals CI.Tabella_Cod
               Group Join Specie In GiasContext.SpecieVegetali
                 On Specie.Veg_Cod Equals CI.Veg_Cod Into Specie_Group = Group
               From _Specie_Group In Specie_Group.DefaultIfEmpty()
               Group Join Cultivar In GiasContext.Cultivar
                 On Cultivar.Cul_Cod Equals CI.Cul_Cod Into Cultivar_Group = Group
               From _Cultivar_Group In Cultivar_Group.DefaultIfEmpty()
               Where CI.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                     CI.Piva.Equals(piva) AndAlso
                     strElem_Cod.Contains(Materie_Prime.Elem_Cod)
               Order By CI.Id_Config
               Select New With {
                  .Id_Config = CI.Id_Config,
                  .Tabella_Cod = CI.Tabella_Cod,
                  .Tipo_Imballo_Des = oTabelle.Tabella_Des,
                  .Mat_Cod = CI.Mat_Cod,
                  .Imballo_Des = Materie_Prime.Mat_Des,
                  .Veg_Cod = If(_Specie_Group Is Nothing, 0, _Specie_Group.Veg_Cod),
                  .Veg_Des = If(_Specie_Group Is Nothing, "", _Specie_Group.Veg_Des),
                  .Cul_Cod = If(_Cultivar_Group Is Nothing, 0, _Cultivar_Group.Cul_Cod),
                  .Cul_Des = If(_Cultivar_Group Is Nothing, "", _Cultivar_Group.Cul_Des),
                  .ValoreStr = CI.Valore,
                  .Valore = 0.0
              }

            Dim myList = TestataElem.ToList()
            For Each obj In myList
                obj.Valore = CDec(obj.ValoreStr)
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function


    '##############################################################################################
    Public Function Leggi_Elem_Configurazione_Imballaggi(ByVal piva As String,
                                                         ByVal ID As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As Configurazione_Imballaggi
        Dim Elem As Configurazione_Imballaggi = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Const nomeRoutine = "AgronicaCoreContabDAL.Configurazione_Imballaggi_R.Leggi_Elem_Configurazione_Imballaggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Elem =
           (From conf_imb In GiasContext.Configurazione_Imballaggi
            Where conf_imb.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                  conf_imb.Piva.Equals(piva) AndAlso
                  conf_imb.Id_Config = ID
            Select conf_imb).FirstOrDefault()

        End Using

        Return Elem

    End Function

End Class

'
'#################################################################
'#################################################################
'#################################################################

Public Class Configurazione_Imballaggi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function Aggiorna_Configurazione_Imballaggi(ByVal piva As String,
                                                       ByVal EFArrayToInsert As ArrayList,
                                                       ByVal EFArrayToUpdate As ArrayList,
                                                       ByVal EFArrayToDelete As ArrayList,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Dim ObjSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Configurazione_Imballaggi_W.Aggiorna_Configurazione_Imballaggi()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each elem As Configurazione_Imballaggi In EFArrayToInsert
                        success = False
                        For i As Integer = 0 To retries - 1
                            Try
                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext, "Configurazione_Imballaggi",
                                                                       0, 2000000000, objParametri)
                                elem.Id_Config = idSeq
                                GiasContext.Configurazione_Imballaggi.Add(elem)

                                GiasContext.SaveChanges()
                                success = True
                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each elem As Configurazione_Imballaggi In EFArrayToUpdate
                            GiasContext.Configurazione_Imballaggi.Attach(elem)
                            GiasContext.Entry(elem).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each elem As Configurazione_Imballaggi In EFArrayToDelete

                            GiasContext.Configurazione_Imballaggi.Attach(elem)
                            GiasContext.Configurazione_Imballaggi.Remove(elem)

                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

End Class
