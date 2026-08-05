Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreScadenziario
Imports System.Web.UI.WebControls

Public Class Alert_Avvisi_R

    Public Function leggi_Alert_Avvisi(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As List(Of Alert_Avvisi)

        Return leggi_Alert_Avvisi(objParametri, Nothing, Nothing, Nothing, "", Nothing, Nothing)
    End Function

    Public Function leggi_Alert_Avvisi(
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal ID_Avviso As Integer
                          ) As List(Of Alert_Avvisi)

        Return leggi_Alert_Avvisi(objParametri, ID_Avviso, Nothing, Nothing, "", Nothing, Nothing)
    End Function

    Public Function leggi_Alert_Avvisi(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal ID_Avviso As Integer?,
                                ByVal ID_Area As Integer?,
                                ByVal ID_Tipologia As Integer?,
                                ByVal Filtro_RapCon As String,
                                ByVal ID_Evento As Integer?,
                                ByVal GGAttesa As Integer?
                              ) As List(Of Alert_Avvisi)

        Dim r As New AgronicaCoreScadenziario.Alert_Avvisi_R
        Dim listaObjPnlCtrl As New List(Of Alert_Avvisi)

        Dim dt As DataTable = r.Leggi(ID_Avviso, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento, GGAttesa, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New Alert_Avvisi(
                                          UtilityProvider.DBNullToNothing(dRow("ID_Avviso")),
                                          UtilityProvider.DBNullToNothing(dRow("ID_Area")),
                                          UtilityProvider.DBNullToNothing(dRow("ID_Tipologia")),
                                          UtilityProvider.DBNullToNothing(dRow("Filtro_RapCon")),
                                          UtilityProvider.DBNullToNothing(dRow("ID_Evento")),
                                          UtilityProvider.DBNullToNothing(dRow("GGAttesa")),
                                          UtilityProvider.DBNullToNothing(dRow("MailMittente")),
                                          UtilityProvider.DBNullToNothing(dRow("MailA")),
                                          UtilityProvider.DBNullToNothing(dRow("MailCC"))
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

    Public Function testElemGiaEsistente(ByRef objParametri As AgronicaCoreParametri, ID_Area As Integer, ID_Tipologia As Integer, Filtro_RapCon As String, ID_Evento As Integer, GGAttesa As Integer, EccettoID_Avviso As Integer?) As Boolean

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCoreScadenziario.Alert_Avvisi_R
        Dim filtro As String = If(IsNothing(EccettoID_Avviso), "", " ID_Avviso <> " & EccettoID_Avviso & " ")

        Dim dt As DataTable = r.Leggi(Nothing, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento, GGAttesa, filtro, "", objParametri)
        Dim res As Boolean = dt.Rows.Count > 0

        Return res

    End Function

End Class

Public Class Alert_Avvisi_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                            ByVal nc As Alert_Avvisi
                            ) As String

        Dim res As String = aggiungi(objParametri, nc.ID_Avviso, nc.ID_Area, nc.ID_Tipologia, nc.ID_Evento,
                                       nc.GGAttesa, nc.MailMittente, nc.MailA, nc.MailCC)
        Return res

    End Function

    Private Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                                ByVal ID_Avviso As Integer,
                                ByVal ID_Area As Integer,
                                ByVal ID_Tipologia As Integer,
                                ByVal Filtro_RapCon As String,
                                ByVal ID_Evento As Integer,
                                ByVal GGAttesa As Integer,
                                ByVal MailMittente As String,
                                ByVal MailA As String,
                                ByVal MailCC As String
                              ) As String

        'Verifico che non esista già un elemento con quell'area, evento e ggAttesa
        Dim r As New Alert_Avvisi_R
        If r.testElemGiaEsistente(objParametri, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento, GGAttesa, Nothing) Then
            Return "Errore: Esiste già un avviso per questa area, tipologia, rapporto contabile, evento e GG di Attesa"
        End If

        Try
            'Salvo il nuovo elemento
            Dim w As New AgronicaCoreScadenziario.Alert_Avvisi_W
            Dim res As Boolean = w.Scrivi(objParametri, ID_Avviso, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento,
                                        GGAttesa, MailMittente, MailA, MailCC)
        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                            ByVal ID_Area As Integer,
                            ByVal ID_Tipologia As Integer,
                            ByVal Filtro_RapCon As String,
                            ByVal ID_Evento As Integer,
                            ByVal GGAttesa As Integer,
                            ByVal MailMittente As String,
                            ByVal MailA As String,
                            ByVal MailCC As String
                          ) As String

        'Verifico che non esista già un elemento con quell'area, evento e ggAttesa
        Dim r As New Alert_Avvisi_R
        If r.testElemGiaEsistente(objParametri, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento, GGAttesa, Nothing) Then
            Return "Errore: Esiste già un avviso per questa area, tipologia, rapporto contabile, evento e GG di Attesa"
        End If

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Creo la nuova Chiave
            Dim seq As New Agro_Sequenze()
            Dim ID_Avviso As Integer = seq.NuovoId_Tabella("Alert_ID_Avviso", 0, 2000000000, objParametri)

            'Salvo il nuovo elemento
            Dim esito As String = aggiungi(objParametri, ID_Avviso, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento,
                                            GGAttesa, MailMittente, MailA, MailCC)

            If esito = "" Then
                esito = aggiornadatainvio(objParametri, ID_Area, ID_Tipologia, Filtro_RapCon, 0)
            End If

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

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByVal Old_ID_Avviso As Integer,
                                ByVal New_ID_Area As Integer,
                                ByVal New_ID_Tipologia As Integer,
                                ByVal NewFiltro_RapCon As String,
                                ByVal New_ID_Evento As Integer,
                                ByVal New_GGAttesa As Integer,
                                ByVal New_MailMittente As String,
                                ByVal New_MailA As String,
                                ByVal New_MailCC As String
                              ) As String

        Dim res2 As String

        'Verifico che non esista già un elemento con quell'area, evento e ggAttesa
        Dim r As New Alert_Avvisi_R
        If r.testElemGiaEsistente(objParametri, New_ID_Area, New_ID_Tipologia, NewFiltro_RapCon, New_ID_Evento, New_GGAttesa, Old_ID_Avviso) Then
            Return "Errore: Esiste già un avviso per questa area, tipologia, rapporto contabile, evento e GG di Attesa"
        End If

        Try
            'Modifico l'elemento
            Dim w As New AgronicaCoreScadenziario.Alert_Avvisi_W
            Dim res As Boolean = w.Modifica(objParametri, Old_ID_Avviso, New_ID_Area, New_ID_Tipologia, NewFiltro_RapCon, New_ID_Evento,
                                New_GGAttesa, New_MailMittente, New_MailA, New_MailCC)


            res2 = aggiornadatainvio(objParametri, New_ID_Area, New_ID_Tipologia, NewFiltro_RapCon, 0)


        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function


    Public Function aggiornadatainvio(ByRef objParametri As AgronicaCoreParametri,
                                      ByVal ID_Area As Integer,
                                      ByVal ID_Tipologia As Integer,
                                      ByVal Filtro_RapCon As String,
                                      ByVal ID_Avviso As Integer,
                                      Optional ByVal bDelete As Boolean = False
                                      ) As String

        Dim DataInvio As DateTime
        Dim bOk As Boolean



        Try

            Dim DT As DataTable
            Dim mr As New AgronicaCoreMailBIZ.Mail_Programmazione_R()
            Dim mw As New AgronicaCoreMailBIZ.Mail_Programmazione_W()

            DT = mr.LeggixUpdateAvviso(objParametri, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Avviso)


            If DT.Rows.Count > 0 Then

                For Each dr As DataRow In DT.Rows

                    Select Case bDelete

                        Case False

                            DataInvio = DateAdd("d", CInt(dr("ggattesa")), CDate(dr("data_scadenza")))
                        Case True

                            DataInvio = AGRODATAFINE 'Dummy per annullamento

                    End Select


                    bOk = mw.AggiornaDataInvio(objParametri, CStr(dr("TipoMail_Chiave")), "", DataInvio)

                Next

            End If



        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function


    Public Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByVal e As Alert_Avvisi
                              ) As String

        Dim res As String = modifica(objParametri, e.ID_Avviso, e.ID_Area, e.ID_Tipologia, e.Filtro_RapCon, e.ID_Evento,
                                       e.GGAttesa, e.MailMittente, e.MailA, e.MailCC)
        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreParametri,
                                            ByVal ID_Avviso As Integer
                                           ) As String

        Try

            Dim res2 As String = aggiornadatainvio(objParametri, 0, 0, "", ID_Avviso, True)

            'Cancello l'elemento
            Dim w As New AgronicaCoreScadenziario.Alert_Avvisi_W
            Dim res As Boolean = w.Cancella("", ID_Avviso, objParametri)



        Catch ex As Exception
            Return ex.Message
        End Try

        Return ""

    End Function

End Class

Public Class Alert_Avvisi
    Public Property ID_Avviso As Integer
    Public Property ID_Area As Integer
    Public Property ID_Tipologia As Integer
    Public Property Filtro_RapCon As String
    Public Property ID_Evento As Integer
    Public Property MailMittente As String
    Public Property MailA As String
    Public Property MailCC As String
    Public Property GGAttesa As Integer

    Public Sub New(ID_Avviso As Integer,
                   ID_Area As Integer,
                   ID_Tipologia As Integer,
                   Filtro_RapCon As String,
                   ID_Evento As Integer,
                   GGAttesa As Integer,
                   MailMittente As String,
                   MailA As String,
                   MailCC As String)

        _ID_Avviso = ID_Avviso
        _ID_Area = ID_Area
        _ID_Tipologia = ID_Tipologia
        _Filtro_RapCon = Filtro_RapCon
        _ID_Evento = ID_Evento
        _GGAttesa = GGAttesa
        _MailMittente = MailMittente
        _MailA = MailA
        _MailCC = MailCC
    End Sub

    Public Sub New()
        _ID_Avviso = 0
        _ID_Area = 0
        _ID_Evento = 0
        _GGAttesa = 0
        _MailMittente = ""
        _MailA = ""
        _MailCC = ""
    End Sub

End Class