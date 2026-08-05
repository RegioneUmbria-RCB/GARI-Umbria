Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDomandaIrriguaDAL
Public Class LettureContatoriAziendali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiContatoreAziendale(ByVal piva As String,
                                            ByVal StartDate As Date,
                                            ByVal EndDate As Date,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim ret As New DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R.LeggiContatoreAziendale()"
        Dim messaggioErrore As String = ""
        If piva = "" Then
            Throw New Exception("Specificare la partita iva")
        End If

        Try
            Dim lca_dal As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R

            ret = lca_dal.LeggiAnagraficheContatori(piva, 0, StartDate, EndDate, "", "", objParametri)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try

        Return ret
    End Function

    Public Function LeggiLettureContatore(ByVal piva As String,
                                          ByVal id_contatore As Integer,
                                          ByVal StartDate As Date,
                                          ByVal EndDate As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale
        Dim ret As New AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R.LeggiLettureContatore()"

        Dim messaggioErrore As String = ""

        If (piva = "" And id_contatore = 0) Then
            Throw New Exception("Specificare partita iva oppure id_contatore")
        End If

        Try
            Dim lca_dal As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R
            Dim aux As New AgronicaCoreDomandaIrriguaBIZ.DatiAggiuntiviDocumenti_R
            Dim dtlca As New DataTable

            ret.piva = piva

            ret.elencoContatori = GetDatiAnagraficaContatori(piva, StartDate, EndDate, objParametri)

            ret.datiAzienda = aux.GetDatiAzienda(piva, objParametri)

            ret.elencoLetture = New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.LettureContatore)

            Dim filtroContatori As String = ""
            If ret.elencoContatori.Count > 0 Then
                filtroContatori = "id_contatore in ( " + String.Join(",", ret.elencoContatori.Select(Function(x) x.id_contatore)) + ")"
            End If

            dtlca = lca_dal.Leggi(0, piva, 0, StartDate, EndDate, filtroContatori, "", objParametri)
            For Each row In dtlca.Rows
                ret.elencoLetture.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.LettureContatore() With {
                                        .id = row("Id"),
                                        .id_contatore = row("id_contatore"),
                                        .datalettura = row("DataLettura"),
                                        .valore = row("Valore")
                                      })
            Next

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try

        Return ret

    End Function

    Public Function LeggiSingolaLetturaContatore(ByVal IdLettura As Integer,
                                                 ByVal piva As String,
                                                 ByVal anno As Integer,
                                                 ByVal TipoLettura As Integer,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale
        Dim ret As New AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R.LeggiLettureContatore()"

        Dim messaggioErrore As String = ""

        If IdLettura = 0 And (piva = "" Or anno = 0) Then
            Throw New Exception("Specificare id della lettura oppure partita iva ed anno")
        End If

        Try
            Dim lca_dal As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R
            Dim aux As New AgronicaCoreDomandaIrriguaBIZ.DatiAggiuntiviDocumenti_R
            Dim dtlca As New DataTable

            If IdLettura > 0 Then

                dtlca = lca_dal.Leggi(IdLettura, "", 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)
                For Each row In dtlca.Rows
                    ret.piva = row("piva")
                    ret.anno = Convert.ToDateTime(row("DataLettura")).Year
                    ret.elencoLetture = New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.LettureContatore)
                    ret.elencoLetture.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.LettureContatore() With {
                                            .id = row("Id"),
                                            .id_contatore = row("Id_contatore"),
                                            .datalettura = row("DataLettura"),
                                            .valore = row("Valore")
                                          })
                Next
            Else
                Dim xOrderBy As String = ""
                Select Case TipoLettura
                    Case 1

                        xOrderBy = " DataLettura Desc"
                    Case 2

                        xOrderBy = " DataLettura Asc"
                End Select
                dtlca = lca_dal.Leggi(0, piva, 0, New DateTime(anno, 1, 1, 0, 0, 0), New DateTime(anno, 12, 31, 23, 59, 59), "", xOrderBy, objParametri)
                For Each row In dtlca.Rows
                    ret.piva = row("piva")
                    ret.anno = Convert.ToDateTime(row("DataLettura")).Year
                    ret.elencoLetture = New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.LettureContatore)
                    ret.elencoLetture.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.LettureContatore() With {
                                            .id = row("Id"),
                                            .id_contatore = row("Id_contatore"),
                                            .datalettura = row("DataLettura"),
                                            .valore = row("Valore")
                                          })
                    Exit For
                Next
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try

        Return ret

    End Function

    Private Function GetDatiAnagraficaContatori(ByVal piva As String,
                                               ByVal StartDate As DateTime,
                                               ByVal EndDate As DateTime,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiAnagraficaContatori)
        Dim ret As New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiAnagraficaContatori)
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R.GetDatianagrficaContatori()"

        Dim messaggioErrore As String = ""

        If (piva = "") Then
            Throw New Exception("Specificare partita iva oppure id_contatore")
        End If

        Try
            Dim lca_dal As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R

            Dim dtAnag = lca_dal.LeggiAnagraficheContatori(piva, 0, StartDate, EndDate, "", "", objParametri)

            For Each row In dtAnag.Rows
                ret.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiAnagraficaContatori() With {
                            .id_contatore = row("Mac_Cod"),
                            .matricola = row("N_Immatricolazione"),
                            .descrizione = row("Mac_Des"),
                            .ValidoDal = row("validita_inizio"),
                            .ValidoAl = row("validita_fine")
                        })
            Next

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret
    End Function

End Class

Public Class LettureContatoriAziendali_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviLettureContatori(ByRef data As AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_W.ScriviLettureContatori()"

        Dim messaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        If data.piva = "" Then
            Throw New Exception("Specificare partita iva ")
        End If

        If data.elencoLetture Is Nothing OrElse data.elencoLetture.Count <= 0 Then
            Throw New Exception("Nessuna lettura per il contatore indicato. Operazione interrotta")
        End If

        Try


            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri)
            Dim lca_dal_W As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_W
            Dim lca_dal_R As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R

            For Each lettura In data.elencoLetture
                If lettura.id = -1 Then
                    Dim retL = lca_dal_W.Scrivi(data.piva, lettura.id_contatore, lettura.datalettura, lettura.valore, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                    If retL = False Then
                        Throw New Exception(String.Format("Errore in scrittura lettura contatore (piva: {0} \ id: {1} \ data: {2} \ valore {3}. operazione interrotta", data.piva, lettura.id_contatore, lettura.datalettura, lettura.valore))
                    End If
                Else
                    Dim retL = lca_dal_W.Modifica(lettura.id, data.piva, lettura.id_contatore, lettura.datalettura, lettura.valore, "", objParametri)
                    If retL = False Then
                        Throw New Exception(String.Format("Errore in modifica lettura contatore (piva: {0} \ id: {1} \ data: {2} \ valore {3}. operazione interrotta", data.piva, lettura.id_contatore, lettura.datalettura, lettura.valore))
                    End If
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            ret = True
        Catch ex As Exception
            ret = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret
    End Function

    Public Function EliminaLettureContatori(ByVal id_lettura As Integer,
                                            ByVal piva As String,
                                            ByVal id_contatore As Integer,
                                            ByVal StartDate As Date,
                                            ByVal EndDate As Date,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_W.EliminaLettureContatori()"

        Dim messaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        If id_lettura = 0 And (piva = "" Or id_contatore = 0) Then
            Throw New Exception("Specificare id lettura oppure partita iva ed id_contatore")
        End If

        Try


            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri)
            Dim lca_dal As New AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_W
            Dim RetD As Boolean = False
            If id_lettura > 0 Then
                RetD = lca_dal.Cancella(id_lettura, "", 0, StartDate, EndDate, "", objParametri)
            Else
                RetD = lca_dal.Cancella(0, piva, id_contatore, StartDate, EndDate, "", objParametri)
            End If
            If RetD = False Then
                Throw New Exception(String.Format("Errore in cancellazione letture. operazione interrotta"))
            End If
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            ret = True
        Catch ex As Exception
            ret = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret
    End Function

End Class