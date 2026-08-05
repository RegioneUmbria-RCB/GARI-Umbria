Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.UI.WebControls

Public Class NC_CauseChiusura_R

    Public Function leggi_NC_CauseChiusura_ToListOfListItem( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of ListItem)

        Dim objNC As List(Of NC_CauseChiusura) = leggi_NC_CauseChiusura(objParametri, Nothing, Nothing)
        Dim listNC As New List(Of ListItem)

        For Each elem As NC_CauseChiusura In objNC
            listNC.Add(New ListItem(elem.Nome, elem.ID_CausaChiusura))
        Next

        Return listNC

    End Function

    Public Function leggi_NC_CauseChiusura( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of NC_CauseChiusura)

        Return leggi_NC_CauseChiusura(objParametri, Nothing, Nothing)
    End Function

    Public Function leggi_NC_CauseChiusura( _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ID_CausaChiusura As Integer _
                          ) As List(Of NC_CauseChiusura)

        Return leggi_NC_CauseChiusura(objParametri, ID_CausaChiusura, Nothing)
    End Function

    Public Function leggi_NC_CauseChiusura( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_CausaChiusura As Integer?, _
                                ByVal Nome As String _
                              ) As List(Of NC_CauseChiusura)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_CauseChiusura_R
        Dim listaObjPnlCtrl As New List(Of NC_CauseChiusura)

        Dim dt As DataTable = r.Leggi(ID_CausaChiusura, Nome, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New NC_CauseChiusura( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_CausaChiusura")), _
                                          UtilityProvider.DBNullToNothing(dRow("Nome")) _
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

    Public Function testNomeGiaEsistente(ByRef objParametri As AgronicaCoreParametri, nome As String, EccettoID_CausaChiusura As Integer?) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_CauseChiusura_R
        Dim filtro As String = If(IsNothing(EccettoID_CausaChiusura), "", " ID_CausaChiusura <> " & EccettoID_CausaChiusura & " ")

        Dim dt As DataTable = r.Leggi(Nothing, nome, filtro, "", objParametri)
        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

    Public Function testElemGiaInUso(ByRef objParametri As AgronicaCoreParametri, ID_CausaChiusura As Integer) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Testata_R
        Dim dt As DataTable = r.Leggi(Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, ID_CausaChiusura, Nothing, Nothing, "", "", objParametri)

        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

End Class

Public Class NC_CauseChiusura_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal nc As NC_CauseChiusura
                            ) As String

        Dim res As String = aggiungi(objParametri, nc.ID_CausaChiusura, nc.Nome)
        Return res

    End Function

    Private Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_CausaChiusura As Integer, _
                                ByVal Nome As String _
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_CauseChiusura_R
        If r.testNomeGiaEsistente(objParametri, Nome, Nothing) Then
            Return "Errore: Esiste già una causa di chiusura con questo nome"
        End If

        Try
            'Salvo il nuovo elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_CauseChiusura_W
            Dim res As Boolean = w.Scrivi(objParametri, ID_CausaChiusura, Nome)
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
            Dim ID_CausaChiusura As Integer = seq.NuovoId_Tabella("NC_ID_CausaChiusura", 0, 2000000000, objParametri)

            'Salvo il nuovo elemento
            Dim esito As String = aggiungi(objParametri, ID_CausaChiusura, Nome)

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
                                ByVal Old_ID_CausaChiusura As Integer, _
                                ByVal New_Nome As String _
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_CauseChiusura_R
        If r.testNomeGiaEsistente(objParametri, New_Nome, Old_ID_CausaChiusura) Then
            Return "Errore: Esiste già una causa di chiusura con questo nome"
        End If

        'Verifico che l'elemento non sia già stato utilizzato
        If r.testElemGiaInUso(objParametri, Old_ID_CausaChiusura) Then
            Return "Errore: La causa di chiusura è in uso per qualche NC"
        End If

        Try
            'Modifico l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_CauseChiusura_W
            Dim res As Boolean = w.Modifica(objParametri, Old_ID_CausaChiusura, New_Nome)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal e As NC_CauseChiusura _
                              ) As String

        Dim res As String = modifica(objParametri, e.ID_CausaChiusura, e.Nome)
        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreParametri, _
                                        ByVal ID_CausaChiusura As Integer _
                                       ) As String

        'Verifico che l'elemento non sia già stato utilizzato
        Dim r As New NC_CauseChiusura_R
        If r.testElemGiaInUso(objParametri, ID_CausaChiusura) Then
            Return "Errore: La causa di chiusura è in uso per qualche NC"
        End If

        Try
            'Cancello l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_CauseChiusura_W
            Dim res As Boolean = w.Cancella("", ID_CausaChiusura, objParametri)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

End Class
<Serializable()>
Public Class NC_CauseChiusura
    Public Property ID_CausaChiusura As Integer
    Public Property Nome As String

    Public Sub New(ID_CausaChiusura As Integer, _
                    Nome As String)
        _ID_CausaChiusura = ID_CausaChiusura
        _Nome = Nome
    End Sub

    Public Sub New()
        _ID_CausaChiusura = 0
        _Nome = ""
    End Sub

End Class