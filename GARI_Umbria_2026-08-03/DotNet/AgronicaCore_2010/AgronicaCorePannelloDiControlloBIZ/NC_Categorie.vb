Imports System.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCorePannelloDiControlloDAL
Imports System.Web.UI.WebControls

Public Class NC_Categorie_R

    Public Function leggi_NC_CategorieAree_ToListOfListItem( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of ListItem)

        Dim objNC As List(Of NC_Categorie) = leggi_NC_Categorie(objParametri, Nothing, Nothing, Nothing)
        Dim listNC As New List(Of ListItem)

        For Each elem As NC_Categorie In objNC
            listNC.Add(New ListItem(elem.Area, elem.Area))
        Next

        listNC = listNC.Distinct().ToList() 'Tolgo i doppioni

        Return listNC

    End Function

    Public Function leggi_NC_CategorieTipologie_ToListOfListItem( _
                                ByVal Area As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of ListItem)

        Dim objNC As List(Of NC_Categorie) = leggi_NC_Categorie(objParametri, Nothing, Area, Nothing)
        Dim listNC As New List(Of ListItem)

        For Each elem As NC_Categorie In objNC
            listNC.Add(New ListItem(elem.Tipologia, elem.ID_Categoria))
        Next

        Return listNC

    End Function

    Public Function leggi_NC_Categorie( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of NC_Categorie)

        Dim listaObjPnlCtrl As List(Of NC_Categorie) = leggi_NC_Categorie(objParametri, Nothing, Nothing, Nothing)

        Return listaObjPnlCtrl

    End Function

    Public Function leggi_NC_Categorie( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_Categoria As Integer? _
                              ) As List(Of NC_Categorie)

        Dim listaObjPnlCtrl As List(Of NC_Categorie) = leggi_NC_Categorie(objParametri, ID_Categoria, Nothing, Nothing)

        Return listaObjPnlCtrl

    End Function

    Public Function leggi_NC_Categorie( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_Categoria As Integer?, _
                                ByVal Area As String, _
                                ByVal Tipologia As String _
                              ) As List(Of NC_Categorie)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Categorie_R
        Dim listaObjPnlCtrl As New List(Of NC_Categorie)

        Dim dt As DataTable = r.Leggi(ID_Categoria, Area, Tipologia, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New NC_Categorie( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Categoria")), _
                                          UtilityProvider.DBNullToNothing(dRow("Area")), _
                                          UtilityProvider.DBNullToNothing(dRow("Tipologia"))
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

    Public Function testNomeAreaTipologiaGiaEsistente(ByRef objParametri As AgronicaCoreParametri, Area As String, Tipologia As String, EccettoID_Categoria As Integer?) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Categorie_R
        Dim filtro As String = If(IsNothing(EccettoID_Categoria), "", " ID_Categoria <> " & EccettoID_Categoria & " ")

        Dim dt As DataTable = r.Leggi(Nothing, Area, Tipologia, filtro, "", objParametri)
        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

    Public Function testElemGiaInUso(ByRef objParametri As AgronicaCoreParametri, ID_Categoria As Integer) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Testata_R
        Dim dt As DataTable = r.Leggi(Nothing, ID_Categoria, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "", "", objParametri)

        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

End Class

Public Class NC_Categorie_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                        ByVal nc As NC_Categorie
                        ) As String

        Dim res As String = aggiungi(objParametri, nc.ID_Categoria, nc.Area, nc.Tipologia)
        Return res

    End Function

    Private Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_Categoria As Integer, _
                                ByVal Area As String, _
                                ByVal Tipologia As String _
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_Categorie_R
        If r.testNomeAreaTipologiaGiaEsistente(objParametri, Area, Tipologia, Nothing) Then
            Return "Errore: Esiste già una categoria con questa area e tipologia"
        End If

        Try
            'Salvo il nuovo elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Categorie_W
            Dim res As Boolean = w.Scrivi(objParametri, ID_Categoria, Area, Tipologia)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal Area As String, _
                            ByVal Tipologia As String _
                          ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_Categorie_R
        If r.testNomeAreaTipologiaGiaEsistente(objParametri, Area, Tipologia, Nothing) Then
            Return "Errore: Esiste già una categoria con questa area e tipologia"
        End If

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Creo la nuova Chiave
            Dim seq As New Agro_Sequenze()
            Dim ID_Categoria As Integer = seq.NuovoId_Tabella("NC_ID_Categoria", 0, 2000000000, objParametri)

            'Salvo il nuovo elemento
            Dim esito As String = aggiungi(objParametri, ID_Categoria, Area, Tipologia)

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
                                ByVal Old_ID_Categoria As Integer, _
                                ByVal New_Area As String, _
                                ByVal New_Tipologia As String _
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New NC_Categorie_R
        If r.testNomeAreaTipologiaGiaEsistente(objParametri, New_Area, New_Tipologia, Old_ID_Categoria) Then
            Return "Errore: Esiste già una categoria con questa area e tipologia"
        End If

        'Verifico che l'elemento non sia già stato utilizzato
        If r.testElemGiaInUso(objParametri, Old_ID_Categoria) Then
            Return "Errore: La categoria è in uso per qualche NC"
        End If

        Try
            'Modifico l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Categorie_W
            Dim res As Boolean = w.Modifica(objParametri, Old_ID_Categoria, New_Area, New_Tipologia)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal e As NC_Categorie _
                              ) As String

        Dim res As String = modifica(objParametri, e.ID_Categoria, e.Area, e.Tipologia)
        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreParametri, _
                                            ByVal ID_Categoria As Integer _
                                           ) As String

        'Verifico che l'elemento non sia già stato utilizzato
        Dim r As New NC_Categorie_R
        If r.testElemGiaInUso(objParametri, ID_Categoria) Then
            Return "Errore: La categoria è in uso per qualche NC"
        End If

        Try
            'cancello l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Categorie_W
            Dim res As Boolean = w.Cancella("", ID_Categoria, objParametri)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

End Class
<Serializable()>
Public Class NC_Categorie
    Public Property ID_Categoria As Integer
    Public Property Area As String
    Public Property Tipologia As String

    Public Sub New(ID_Categoria As Integer, _
                    Area As String, _
                    Tipologia As String)

        _ID_Categoria = ID_Categoria
        _Area = Area
        _Tipologia = Tipologia
    End Sub

End Class