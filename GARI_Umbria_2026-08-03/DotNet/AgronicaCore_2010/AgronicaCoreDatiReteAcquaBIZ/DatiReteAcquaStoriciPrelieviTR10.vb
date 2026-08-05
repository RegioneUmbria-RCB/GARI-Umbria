Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class DatiReteAcquaStoriciPrelieviTR10_R
    Public Function LeggiDatiStorici(ByVal PivaSuperUser As String,
                                     ByVal veg_cod As Integer,
                                     ByVal settimana As Integer,
                                     ByVal anno As Integer,
                                     ByVal gruppoconsegna As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Risposta_DatiReteAcquaPrelieviStorici

        Dim resp As New Risposta_DatiReteAcquaPrelieviStorici

        Dim iotdal As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_R

        Dim dt = iotdal.Leggi(PivaSuperUser, veg_cod, settimana, anno, gruppoconsegna, "", "a.settimana,a.veg_cod", objParametri)
        Dim coltura As Integer = -1
        Dim week As Integer = 0

        '1- elenco colonne e model
        Dim col_list As New List(Of DatiReteAcqua_ColumnDefinition)

        For Each row In dt.Rows
            If week = 0 Then
                week = row("settimana")
                col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = "week", .desc = "Settimana"})
                col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = "gruppo", .desc = "Gruppo Consegna"})
                col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = "anno", .desc = "Anno"})
            End If
            If week <> row("settimana") Then
                Exit For
            End If

            col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = "CF_" + row("veg_cod").ToString(), .desc = "Storico_Full_" + row("veg_des") + " m3"})
            col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = "CD_" + row("veg_cod").ToString(), .desc = "Storico_Deficit_" + row("veg_des") + " m3"})

        Next
        Dim columns As New JArray
        Dim model As New JObject
        For Each col In col_list
            Dim jj = New JObject(New JProperty("field", col.id), New JProperty("title", col.desc), New JProperty("filterable", False))
            If col.id <> "week" AndAlso col.id <> "gruppo" AndAlso col.id <> "anno" Then
                jj.Add(New JProperty("format", "{0:0.000}"))
            End If
            columns.Add(jj)

            ', New JProperty("width", 20)
            model.Add(New JProperty(col.id, New JObject(New JProperty("editable", IIf({"week", "anno", "gruppo"}.Contains(col.id), False, True)), New JProperty("type", IIf(col.id = "week", "string", "number")))))
        Next

        resp.columns = columns.ToString()
        resp.model = model.ToString()

        '2- dati
        Dim data As New JArray
        For Each row In dt.Rows
            Dim row_data As New JObject
            row_data.Add(New JProperty("week", row("settimana")))
            row_data.Add(New JProperty("gruppo", row("GruppoConsegna")))
            row_data.Add(New JProperty("anno", row("Anno")))
            row_data.Add(New JProperty("CF_" + row("veg_cod").ToString(), row("FullReturn")))
            row_data.Add(New JProperty("CD_" + row("veg_cod").ToString(), row("DeficitReturn")))
            data.Add(row_data)
        Next

        resp.data = data.ToString()


        Return resp

    End Function

    Public Function LeggiDatiStoriciXSpecieDT(ByVal PivaSuperUser As String,
                                              ByVal veg_cod As Integer,
                                              ByVal settimana As Integer,
                                              ByVal anno As Integer,
                                              ByVal gruppoconsegna As Integer,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim iotdal As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_R
        Return iotdal.Leggi(PivaSuperUser, veg_cod, settimana, anno, gruppoconsegna, "", "", objParametri)
    End Function

End Class

Public Class DatiReteAcquaStoriciPrelieviTR10_W
    Public Function AggiornaDatiStorici(ByVal PivaSuperUser As String,
                                        ByVal veg_cod As Integer,
                                        ByVal anno As Integer,
                                        ByVal gruppoconsegna As Integer,
                                        ByVal data As Dictionary(Of Integer, Tuple(Of Decimal, Decimal)),
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim iotdal As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim ret As Boolean = False
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)
        Try

            If iotdal.Cancella(objParametri.PivaSuperUser, veg_cod, 0, anno, gruppoconsegna, objParametri) = False Then
                Throw New Exception("Errore in cancellazione dati - PivaSuperUser: " + objParametri.PivaSuperUser + "/ Specie: " + veg_cod.ToString())
            End If
            For Each row In data
                iotdal.Scrivi(objParametri.PivaSuperUser, veg_cod, row.Key, anno, gruppoconsegna, row.Value.Item1, row.Value.Item2, AGRODATAINIZIO, AGRODATAFINE, objParametri)
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            ret = True
        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If
            ret = False
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try


        Return ret
    End Function

    Public Function AggiungiSpecieDatiStorici(ByVal PivaSuperUser As String,
                                              ByVal veg_cod As Integer,
                                              ByVal anno As Integer,
                                              ByVal gruppoconsegna As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim iotdal_r As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_R
        Dim iotdal_w As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_W


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim ret As Boolean = False

        Try

            Dim chkDT = iotdal_r.Leggi(PivaSuperUser, veg_cod, 0, anno, gruppoconsegna, "", "", objParametri)
            If chkDT.Rows.Count <= 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

                Dim i = 19
                For i = 19 To 42
                    iotdal_w.Scrivi(objParametri.PivaSuperUser, veg_cod, i, anno, gruppoconsegna, 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                Next
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            Else
                Throw New Exception("Specie Vegetale già presente nello storico")
            End If


            ret = True
        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If
            ret = False
        Finally
            If FlagConnessioneLocale Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
            End If
        End Try


        Return ret
    End Function

End Class
