Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreVarieBIZ

Public Class GIS_Bookmark_R
    Public Function leggi(ByVal bookmark_Cod As Int32,
                          ByRef objParametri_Server As AgronicaCoreParametri) As List(Of Bookmark)

        Dim resp As New List(Of Bookmark)

        Dim xRead As New AgronicaCoreGisDAL.GIS_Bookmark_R
        Dim DT As DataTable

        DT = xRead.Leggi(bookmark_Cod, objParametri_Server)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura dei bookmark.")
        End If

        For Each row In DT.Rows

            Dim bkm As New Bookmark With {
                .Bookmark_Cod = CInt(row("Bookmark_Cod")),
                .Bookmark_Des = row("Bookmark_Des").ToString,
                .Center_Lat = CDbl(row("Center_Lat")),
                .Center_Lng = CDbl(row("Center_Lng")),
                .Posizioni_Speciali = CInt(row("Posizioni_Speciali")),
                .Zoom = CInt(row("Zoom"))
            }

            resp.Add(bkm)
        Next

        Return resp

    End Function
End Class
Public Class GIS_Bookmark_W
    Public Function SalvaNuovoBookmark(ByVal bookmark As Bookmark,
                                       ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisDAL.GIS_Bookmark_W

        Try

            Dim sequenza As New Agro_Sequenze

            Dim newIDBookmark = sequenza.NuovoId_Tabella("GIS_Bookmark", 0, Int32.MaxValue, objParametri_Server, True)

            resp.RispostaOK = xWrite.SalvaNuovoBookmark(newIDBookmark, bookmark, objParametri_Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nel salvataggio del bookmark.")
            End If

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp
    End Function

    Public Function AggiornaBookmark(ByVal bookmark As Bookmark,
                                     ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisDAL.GIS_Bookmark_W
        Dim xRead As New AgronicaCoreGisDAL.GIS_Bookmark_R

        Try

            Dim DT As DataTable

            DT = xRead.Leggi(bookmark.Bookmark_Cod, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Impossibile trovare il bookmark per l'utente corrente.")
            End If

            resp.RispostaOK = xWrite.AggiornaBookmark(bookmark, objParametri_Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'aggiornamento del bookmark.")
            End If

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp

    End Function

    Public Function EliminaBookmark(ByVal bookmark_Cod As Integer,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisDAL.GIS_Bookmark_W
        Dim xRead As New AgronicaCoreGisDAL.GIS_Bookmark_R

        Try

            Dim DT As DataTable

            DT = xRead.Leggi(bookmark_Cod, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Impossibile trovare il bookmark per l'utente corrente.")
            End If

            resp.RispostaOK = xWrite.EliminaBookmark(bookmark_Cod, objParametri_Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nel'eliminazione del bookmark.")
            End If

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp

    End Function
End Class
