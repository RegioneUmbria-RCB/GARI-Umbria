Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreDTOStd.InData.Shared.GridDto
Imports System.Web

Public Class VisteGrigliaDALService
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CaricaViste(nomeUtente As String, gridId As String, utentiDBContext As AgronicaCoreParametri, Optional vistaChiave As ChiaveVista = Nothing)
        Dim NomeRoutine As String = Reflection.MethodBase.GetCurrentMethod().Name

        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        If nomeUtente Is Nothing Then
            nomeUtente = "null"
        End If
        StrSQL.Length = 0
        StrSQL.AppendLine("SELECT * ")
        StrSQL.AppendLine(" From Utenti_Viste")
        StrSQL.AppendLine(" WHERE 1 = 1 ")

        If Not String.IsNullOrEmpty(nomeUtente) Then
            StrSQL.AppendLine(" And NomeUtente ='" & Agro_SQL_SaveText(nomeUtente) & "'")
        End If

        If Not String.IsNullOrEmpty(gridId) Then
            StrSQL.AppendLine(" And GridId = '" & Agro_SQL_SaveText(gridId) & "'")
        End If

        If vistaChiave IsNot Nothing Then
            'Prima cerco le nuove viste salvate con il nuovo GridId (nomeServizioGriglia|PartefinaleURL)
            'Se non le trovo cerco per il vecchio GridId (nomeServizioGriglia)
            If vistaChiave.GridId.Split("|").Length = 2 Then
                StrSQL.AppendLine(" And (")
                StrSQL.AppendLine("     (NomeUtente = '" & Agro_SQL_SaveText(vistaChiave.NomeUtente) & "' And (GridId = '" & Agro_SQL_SaveText(vistaChiave.GridId) & "' Or GridId = '" & Agro_SQL_SaveText(vistaChiave.GridId.Split("|")(0)) & "'))")
                StrSQL.AppendLine("     Or ((GridId = '" & Agro_SQL_SaveText(vistaChiave.GridId) & "' Or GridId = '" & Agro_SQL_SaveText(vistaChiave.GridId.Split("|")(0)) & "') And FlagPubblica = 1)")
                StrSQL.AppendLine(" )")
            Else
                StrSQL.AppendLine(" And NomeUtente = '" & Agro_SQL_SaveText(vistaChiave.NomeUtente) & "'")
                StrSQL.AppendLine(" And GridId = '" & Agro_SQL_SaveText(vistaChiave.GridId.Split("|")(0)) & "'")
            End If
        End If

        DT = EseguiQuery_Lettura(utentiDBContext, StrSQL.ToString, NomeRoutine)

        Return DT
    End Function

    Public Sub ScriviViste(viste As SalvaVisteWrapper, utentiDBContext As AgronicaCoreParametri)
        Dim NomeRoutine As String = Reflection.MethodBase.GetCurrentMethod().Name

        Dim StrSQL As New System.Text.StringBuilder

        If viste.VisteNuove.Count > 0 Then

            StrSQL.Length = 0
            For Each vista In viste.VisteNuove
                StrSQL.AppendLine(" Insert into Utenti_Viste (IdVista, NomeUtente, NomeVista, StatoJson, ColonneJson, GridId, Predefinita, Data_Creazione, Data_Modifica, FiltroJSON, FlagPubblica) values ")
                StrSQL.AppendLine("('" & Agro_SQL_SaveText(vista.IdVista) &
                          "','" & Agro_SQL_SaveText(vista.NomeUtente) & "',")
                StrSQL.AppendLine("'" & Agro_SQL_SaveText(vista.NomeVista) &
                          "','" & Agro_SQL_SaveText(vista.Stato.Replace("'", "''")) &
                          "','" & Agro_SQL_SaveText(vista.Colonne.Replace("'", "''")) &
                          "','" & Agro_SQL_SaveText(vista.GridId) &
                          "','" & Agro_SQL_SaveText(vista.Predefinita) &
                          "', " & Agro_SQL_SaveDateTime(Date.Now) &
                          ", " & Agro_SQL_SaveDateTime(Date.Now) &
                          ",'" & Agro_SQL_SaveText(vista.FiltroJSON.Replace("'", "''")) &
                          "','" & Agro_SQL_SaveText(vista.FlagPubblica) & "')")
            Next

            EseguiQuery_Scrittura(utentiDBContext, StrSQL.ToString, NomeRoutine)

        End If

        If viste.VisteModificate.Count > 0 Then

            StrSQL.Length = 0
            For Each vista In viste.VisteModificate
                StrSQL.AppendLine(" UPDATE Utenti_Viste")
                StrSQL.AppendLine(" Set NomeVista = '" & Agro_SQL_SaveText(vista.NomeVista) &
                          "', StatoJson = '" & Agro_SQL_SaveText(vista.Stato.Replace("'", "''")) &
                          "', ColonneJson = '" & Agro_SQL_SaveText(vista.Colonne.Replace("'", "''")) &
                          "',Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now) &
                          ", FiltroJSON = '" & Agro_SQL_SaveText(vista.FiltroJSON.Replace("'", "''")) &
                          "', FlagPubblica = '" & Agro_SQL_SaveText(vista.FlagPubblica) & "' ")
                StrSQL.AppendLine(" WHERE IdVista = '" & Agro_SQL_SaveText(vista.IdVista) &
                        "' AND NomeUtente = '" & Agro_SQL_SaveText(vista.NomeUtente) &
                        "' AND GridId = '" & Agro_SQL_SaveText(vista.GridId) & "';")
            Next

            EseguiQuery_Scrittura(utentiDBContext, StrSQL.ToString, NomeRoutine)

        End If

    End Sub

    Public Sub CancellaVista(vista As ChiaveVista, utentiDBContext As AgronicaCoreParametri)
        Dim NomeRoutine As String = Reflection.MethodBase.GetCurrentMethod().Name

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0
        StrSQL.AppendLine("DELETE from Utenti_Viste")
        StrSQL.AppendLine(" WHERE IdVista = '" & Agro_SQL_SaveText(vista.IdVista) & "'")
        StrSQL.AppendLine(" AND NomeUtente = '" & Agro_SQL_SaveText(vista.NomeUtente) & "'")
        StrSQL.AppendLine(" AND GridId = '" & Agro_SQL_SaveText(vista.GridId) & "'")

        EseguiQuery_Scrittura(utentiDBContext, StrSQL.ToString, NomeRoutine)
    End Sub

End Class
