Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.UI.WebControls

Public Class NC_Stati_R

    Public Function leggi_NC_Stati_ToListOfListItem( _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As List(Of ListItem)

        Dim objNC As List(Of NC_Stati) = leggi_NC_Stati(objParametri, Nothing, Nothing)
        Dim listNC As New List(Of ListItem)

        For Each elem As NC_Stati In objNC
            listNC.Add(New ListItem(elem.Nome, elem.ID_Stato))
        Next

        Return listNC

    End Function

    Public Function leggi_NC_Stati( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of NC_Stati)

        Return leggi_NC_Stati(objParametri, Nothing, Nothing)
    End Function

    Public Function leggi_NC_Stati( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_Stato As Integer?, _
                                ByVal Nome As String _
                              ) As List(Of NC_Stati)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Stati_R
        Dim listaObjPnlCtrl As New List(Of NC_Stati)

        Dim dt As DataTable = r.Leggi(ID_Stato, Nome, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New NC_Stati( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Stato")), _
                                          UtilityProvider.DBNullToNothing(dRow("Nome")) _
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

    Public Function testNomeGiaEsistente(ByRef objParametri As AgronicaCoreParametri, nome As String, EccettoID_Stato As Integer?) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Stati_R
        Dim filtro As String = If(IsNothing(EccettoID_Stato), "", " ID_Stato <> " & EccettoID_Stato & " ")

        Dim dt As DataTable = r.Leggi(Nothing, nome, filtro, "", objParametri)
        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

    Public Function testElemGiaInUso(ByRef objParametri As AgronicaCoreParametri, ID_Stato As Integer) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Dettagli_R
        Dim dt As DataTable = r.Leggi(Nothing, Nothing, Nothing, Nothing, ID_Stato, Nothing, "", "", objParametri)

        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

End Class

Public Class NC_Stati_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal nc As NC_Stati
                            ) As String

        Dim res As String = aggiungi(objParametri, nc.ID_Stato, nc.Nome)
        Return res

    End Function

    Private Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_Stato As Integer, _
                                ByVal Nome As String _
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_Stati_R
        If r.testNomeGiaEsistente(objParametri, Nome, Nothing) Then
            Return "Errore: Esiste già uno stato con questo nome"
        End If

        Try
            'Salvo il nuovo elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Stati_W
            Dim res As Boolean = w.Scrivi(objParametri, ID_Stato, Nome)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal Nome As String _
                          ) As String

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Creo la nuova Chiave
            Dim seq As New Agro_Sequenze()
            Dim ID_Stato As Integer = seq.NuovoId_Tabella("NC_ID_Stato", 0, 2000000000, objParametri)

            'Salvo il nuovo elemento
            Dim esito As String = aggiungi(objParametri, ID_Stato, Nome)

            If esito <> "" Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = esito
                Exit Try
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            res = ex.Message
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
        End Try

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal Old_ID_Stato As Integer, _
                                ByVal New_Nome As String _
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_Stati_R
        If r.testNomeGiaEsistente(objParametri, New_Nome, Old_ID_Stato) Then
            Return "Errore: Esiste già uno stato con questo nome"
        End If

        'Verifico che Lo stato non sia già stato utilizzato
        If r.testElemGiaInUso(objParametri, Old_ID_Stato) Then
            Return "Errore: Lo stato è in uso per qualche NC"
        End If

        Try
            'Modifico l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Stati_W
            Dim res As Boolean = w.Modifica(objParametri, Old_ID_Stato, New_Nome)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal e As NC_Stati _
                              ) As String

        Dim res As String = modifica(objParametri, e.ID_Stato, e.Nome)
        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreParametri, _
                                            ByVal ID_Stato As Integer _
                                           ) As String

        'Verifico che Lo stato non sia già stato utilizzato
        Dim r As New NC_Stati_R
        If r.testElemGiaInUso(objParametri, ID_Stato) Then
            Return "Errore: Esiste già uno stato con questo nome"
        End If

        Try
            'cancello l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Stati_W
            Dim res As Boolean = w.Cancella("", ID_Stato, objParametri)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

End Class
<Serializable()>
Public Class NC_Stati
    Public Property ID_Stato As Integer
    Public Property Nome As String

    Public Sub New(ID_Stato As Integer, _
                    Nome As String)
        _ID_Stato = ID_Stato
        _Nome = Nome
    End Sub

    Public Sub New()
        _ID_Stato = 0
        _Nome = ""
    End Sub

End Class