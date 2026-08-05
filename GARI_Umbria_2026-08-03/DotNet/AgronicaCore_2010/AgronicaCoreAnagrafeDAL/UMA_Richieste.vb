Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Richieste_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal richiesta_cod As Integer,
                          ByVal gruppo_uma As String,
                          ByVal Programmazione_Cod As Integer,
                          ByVal Selezione_Variabile As enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim selectList As String
        Dim PivaSuperUser = objParametri.PivaSuperUser

        If (Selezione_Variabile = enumSelezioneVariabile.Selezione_TabellaDatiMinimi) Then
            selectList = "u.Piva, u.Programmazione_Cod, u.Gruppo_Colturale_UMA, u.Programmazione_Cod, u.Zona_Pendenza_A_UMA, u.Zona_Pendenza_B_UMA"
        Else
            selectList = "u.*, UMA_Macrousi.Macrouso_UMA_Des, Programmazione_Testata.Programmazione_Des"
        End If

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT " + selectList + " ")
            stb.AppendLine(" FROM UMA_Richieste u")
            stb.AppendLine(" LEFT JOIN Programmazione_Testata ON u.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi ON u.Gruppo_Colturale_UMA = UMA_Macrousi.Macrouso_UMA_Cod ")
            stb.AppendLine(" JOIN UMA_Richieste_Testata urt on urt.piva like '" + Agro_SQL_SaveText(piva) + "' AND urt.Richiesta_Cod = u.Richiesta_Cod ")
            stb.AppendLine(" WHERE ")
            If (gruppo_uma <> "") Then
                stb.AppendLine(" u.Gruppo_Colturale_UMA = '" + Agro_SQL_SaveText(gruppo_uma) + "' AND ")
            End If
            If Programmazione_Cod <> 0 Then
                stb.AppendLine(" u.Programmazione_Cod = " + Agro_SQL_SaveNum(Programmazione_Cod) + " AND ")
            End If
            stb.AppendLine(" u.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' AND ")
            stb.AppendLine(" u.Richiesta_Cod = " + Agro_SQL_SaveNum(richiesta_cod) + " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Richieste_Terzisti(ByVal piva As String,
                                             ByVal richiesta_cod As Integer,
                                             ByVal anno As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste.Leggi_Richieste_Terzisti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable


        Try

            stb.Length = 0

            stb.AppendLine(" SELECT DISTINCT i.rag_soc, r.gruppo_colturale_UMA, c.macrouso_UMA_Des, r.*, tt.Programmazione_Des ")
            stb.AppendLine(" FROM UMA_Richieste r")
            stb.AppendLine(" JOIN UMA_Richieste_Testata t ON ")
            If (richiesta_cod > -1) Then
                stb.AppendLine("t.richiesta_cod = " + Agro_SQL_SaveNum(richiesta_cod) + " AND ")
            End If
            stb.AppendLine(" t.richiesta_cod = r.richiesta_cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi c on c.macrouso_UMA_Cod = r.gruppo_colturale_UMA") 'Codifica_SpecieVegetali_Agea_2015_2020
            stb.AppendLine(" JOIN Imprese i ON r.piva = i.piva")
            stb.AppendLine(" LEFT JOIN Programmazione_Testata tt on tt.Programmazione_Cod = r.Programmazione_Cod ")
            stb.AppendLine(" WHERE t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            stb.AppendLine(" AND t.Tipo_Richiesta = -1 ")
            'stb.AppendLine(" AND YEAR(r.data_Creazione) = " + anno.ToString() + " ")
            stb.AppendLine(" ORDER BY i.rag_soc, r.Data_Creazione DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


End Class

Public Class UMA_Richieste_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_Richieste(ByVal toInsert As ArrayList,
                                       ByVal toUpdate As ArrayList,
                                       ByVal toDelete As ArrayList,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_W.Aggiorna_Richieste()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each richiestaIn As UMA_Richieste In toInsert

                    GiasContext.UMA_Richieste.Add(richiestaIn)

                Next

                For Each richiestaUp As UMA_Richieste In toUpdate

                    GiasContext.UMA_Richieste.Attach(richiestaUp)
                    GiasContext.Entry(richiestaUp).State = EntityState.Modified

                Next

                For Each richiestaDel As UMA_Richieste In toDelete
                    GiasContext.UMA_Richieste.Attach(richiestaDel)
                    GiasContext.UMA_Richieste.Remove(richiestaDel)

                Next


                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function

    Public Function Aggiorna_Richieste_Terzisti(ByVal toInsert As ArrayList,
                                                ByVal toUpdate As ArrayList,
                                                ByVal toDelete As ArrayList,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_W.Aggiorna_Richieste_Terzisti()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each richiestaDel As UMA_Richieste In toDelete

                    GiasContext.UMA_Richieste.Attach(richiestaDel)
                    GiasContext.Entry(richiestaDel).State = EntityState.Deleted

                Next

                For Each richiestaIn As UMA_Richieste In toInsert

                    GiasContext.UMA_Richieste.Add(richiestaIn)

                Next

                For Each richiestaUp As UMA_Richieste In toUpdate

                    GiasContext.UMA_Richieste.Attach(richiestaUp)
                    GiasContext.Entry(richiestaUp).State = EntityState.Modified

                Next


                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            If (ex.InnerException.InnerException.Message.Contains("PRIMARY KEY 'PK_UMA_Richieste'")) Then
                messaggioErrore = "Impossibile inserire una riga duplicata"
            Else
                messaggioErrore = ex.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function

End Class
