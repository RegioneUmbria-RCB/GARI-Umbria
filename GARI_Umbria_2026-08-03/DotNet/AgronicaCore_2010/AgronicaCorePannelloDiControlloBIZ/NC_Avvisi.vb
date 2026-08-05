Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCorePannelloDiControlloDAL
Imports System.Web.UI.WebControls

Public Class NC_Avvisi_R

    Public Function leggi_NC_Avvisi( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of NC_Avvisi)

        Return leggi_NC_Avvisi(objParametri, Nothing, Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_NC_Avvisi( _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ID_Avviso As Integer
                          ) As List(Of NC_Avvisi)

        Return leggi_NC_Avvisi(objParametri, ID_Avviso, Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_NC_Avvisi( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_Avviso As Integer?, _
                                ByVal Area As String, _
                                ByVal ID_Evento As Integer?, _
                                ByVal GGAttesa As Integer? _
                              ) As List(Of NC_Avvisi)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Avvisi_R
        Dim listaObjPnlCtrl As New List(Of NC_Avvisi)

        Dim dt As DataTable = r.Leggi(ID_Avviso, Area, ID_Evento, GGAttesa, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New NC_Avvisi(
                                          UtilityProvider.DBNullToNothing(dRow("ID_Avviso")),
                                          UtilityProvider.DBNullToNothing(dRow("Area")),
                                          0,
                                          0,
                                          UtilityProvider.DBNullToNothing(dRow("ID_Evento")),
                                          UtilityProvider.DBNullToNothing(dRow("GGAttesa")),
                                          UtilityProvider.DBNullToNothing(dRow("MailMittente")),
                                          UtilityProvider.DBNullToNothing(dRow("MailA")),
                                          UtilityProvider.DBNullToNothing(dRow("MailCC")),
                                          UtilityProvider.DBNullToNothing(dRow("MailA_IncludiResponsabile")),
                                          UtilityProvider.DBNullToNothing(dRow("MailCC_IncludiResponsabile"))
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

    Public Function testElemGiaEsistente(ByRef objParametri As AgronicaCoreParametri, Area As String, ID_Evento As Integer, GGAttesa As Integer, EccettoID_Avviso As Integer?) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Avvisi_R
        Dim filtro As String = If(IsNothing(EccettoID_Avviso), "", " ID_Avviso <> " & EccettoID_Avviso & " ")

        Dim dt As DataTable = r.Leggi(Nothing, Area, ID_Evento, GGAttesa, filtro, "", objParametri)
        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

End Class

Public Class NC_Avvisi_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal nc As NC_Avvisi
                            ) As String

        Dim res As String = aggiungi(objParametri, nc.ID_Avviso, nc.Area, nc.ID_Evento, _
                                       nc.GGAttesa, nc.MailMittente, nc.MailA, nc.MailCC, _
                                       nc.MailA_IncludiResponsabile, nc.MailCC_IncludiResponsabile)
        Return res

    End Function

    Private Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_Avviso As Integer, _
                                ByVal Area As String, _
                                ByVal ID_Evento As Integer, _
                                ByVal GGAttesa As Integer, _
                                ByVal MailMittente As String, _
                                ByVal MailA As String, _
                                ByVal MailCC As String, _
                                ByVal MailA_IncludiResponsabile As Boolean, _
                                ByVal MailCC_IncludiResponsabile As Boolean _
                              ) As String

        'Verifico che non esista già un elemento con quell'area, evento e ggAttesa
        Dim r As New NC_Avvisi_R
        If r.testElemGiaEsistente(objParametri, Area, ID_Evento, GGAttesa, Nothing) Then
            Return "Errore: Esiste già un avviso per questa area, evento e GG di Attesa"
        End If

        Try
            'Salvo il nuovo elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Avvisi_W
            Dim res As Boolean = w.Scrivi(objParametri, ID_Avviso, Area, ID_Evento, _
                             GGAttesa, MailMittente, MailA, MailCC, _
                             MailA_IncludiResponsabile, MailCC_IncludiResponsabile)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal Area As String, _
                            ByVal ID_Evento As Integer, _
                            ByVal GGAttesa As Integer, _
                            ByVal MailMittente As String, _
                            ByVal MailA As String, _
                            ByVal MailCC As String, _
                            ByVal MailA_IncludiResponsabile As Boolean, _
                            ByVal MailCC_IncludiResponsabile As Boolean _
                          ) As String

        'Verifico che non esista già un elemento con quell'area, evento e ggAttesa
        Dim r As New NC_Avvisi_R
        If r.testElemGiaEsistente(objParametri, Area, ID_Evento, GGAttesa, Nothing) Then
            Return "Errore: Esiste già un avviso per questa area, evento e GG di Attesa"
        End If

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Creo la nuova Chiave
            Dim seq As New Agro_Sequenze()
            Dim ID_Avviso As Integer = seq.NuovoId_Tabella("NC_ID_Avviso", 0, 2000000000, objParametri)

            'Salvo il nuovo elemento
            Dim esito As String = aggiungi(objParametri, ID_Avviso, Area, ID_Evento, _
                                            GGAttesa, MailMittente, MailA, MailCC, _
                                            MailA_IncludiResponsabile, MailCC_IncludiResponsabile)

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
                                ByVal Old_ID_Avviso As Integer, _
                                ByVal New_Area As String, _
                                ByVal New_ID_Evento As Integer, _
                                ByVal New_GGAttesa As Integer, _
                                ByVal New_MailMittente As String, _
                                ByVal New_MailA As String, _
                                ByVal New_MailCC As String, _
                                ByVal New_MailA_IncludiResponsabile As Boolean, _
                                ByVal New_MailCC_IncludiResponsabile As Boolean _
                              ) As String

        'Verifico che non esista già un elemento con quell'area, evento e ggAttesa
        Dim r As New NC_Avvisi_R
        If r.testElemGiaEsistente(objParametri, New_Area, New_ID_Evento, New_GGAttesa, Old_ID_Avviso) Then
            Return "Errore: Esiste già un avviso per questa area, evento e GG di Attesa"
        End If

        Try
            'Modifico l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Avvisi_W
            Dim res As Boolean = w.Modifica(objParametri, Old_ID_Avviso, New_Area, New_ID_Evento, _
                                New_GGAttesa, New_MailMittente, New_MailA, New_MailCC, _
                                New_MailA_IncludiResponsabile, New_MailCC_IncludiResponsabile)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal e As NC_Avvisi _
                              ) As String

        Dim res As String = modifica(objParametri, e.ID_Avviso, e.Area, e.ID_Evento, _
                                       e.GGAttesa, e.MailMittente, e.MailA, e.MailCC, _
                                       e.MailA_IncludiResponsabile, e.MailCC_IncludiResponsabile)
        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreParametri, _
                                            ByVal ID_Avviso As Integer _
                                           ) As String

        Try
            'Cancello l'elemento
            Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Avvisi_W
            Dim res As Boolean = w.Cancella("", ID_Avviso, objParametri)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

End Class

Public Class NC_Avvisi
    Public Property ID_Avviso As Integer
    Public Property Area As String
    Public Property Id_Tipologia As String
    Public Property Filtro_RapCon As String
    Public Property ID_Evento As Integer
    Public Property MailMittente As String
    Public Property MailA As String
    Public Property MailCC As String
    Public Property GGAttesa As Integer
    Public Property MailA_IncludiResponsabile As Boolean
    Public Property MailCC_IncludiResponsabile As Boolean

    Public Sub New(ID_Avviso As Integer,
                    Area As String,
                    Id_Tipologia As String,
                    Filtro_RapCon As String,
                    ID_Evento As Integer,
                    GGAttesa As Integer,
                    MailMittente As String,
                    MailA As String,
                    MailCC As String,
                    MailA_IncludiResponsabile As Boolean,
                    MailCC_IncludiResponsabile As Boolean)

        _ID_Avviso = ID_Avviso
        _Area = Area
        _Id_Tipologia = Id_Tipologia
        _Filtro_RapCon = Filtro_RapCon
        _ID_Evento = ID_Evento
        _GGAttesa = GGAttesa
        _MailMittente = MailMittente
        _MailA = MailA
        _MailCC = MailCC
        _MailA_IncludiResponsabile = MailA_IncludiResponsabile
        _MailCC_IncludiResponsabile = MailCC_IncludiResponsabile
    End Sub

    Public Sub New()
        _ID_Avviso = 0
        _Area = ""
        _ID_Evento = 0
        _GGAttesa = 0
        _MailMittente = ""
        _MailA = ""
        _MailCC = ""
        _MailA_IncludiResponsabile = False
        _MailCC_IncludiResponsabile = False
    End Sub

End Class