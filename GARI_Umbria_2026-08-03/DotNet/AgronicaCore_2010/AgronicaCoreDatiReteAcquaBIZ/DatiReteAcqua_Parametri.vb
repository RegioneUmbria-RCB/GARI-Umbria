Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class DatiReteAcqua_Parametri_R
    Public Function LeggiParametriGenerali(ByVal PivasuperUser As String,
                                           ByVal Settimana As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Risposta_DatiReteAcquaPars

        Dim resp As New Risposta_DatiReteAcquaPars

        Dim iotparametri As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R

        Dim dt_par_gen = iotparametri.Leggi_ParametriGenerali(PivasuperUser, Settimana, "", "", objParametri)


        '1- elenco colonne e model
        Dim col_list As New List(Of DatiReteAcqua_ColumnDefinition)

        For Each col As DataColumn In dt_par_gen.Columns
            Select Case col.ColumnName
                Case "week"
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = "Settimana"})
                Case "RadiazioneExtraterrestre"
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = "Radiazione extraterrestre"})
                Case Else
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = col.ColumnName})
            End Select
        Next

        Dim chk = {"week", "year"}

        Dim columns As New JArray
        Dim model As New JObject
        For Each col In col_list
            Dim jj = New JObject(New JProperty("field", col.id), New JProperty("title", col.desc), New JProperty("filterable", False))
            If chk.Contains(col.id) = False Then
                jj.Add(New JProperty("format", "{0:0.0}"))
            End If
            columns.Add(jj)

            ', New JProperty("width", 20)
            model.Add(New JProperty(col.id, New JObject(New JProperty("editable", IIf(chk.Contains(col.id), False, True)), New JProperty("type", IIf(chk.Contains(col.id), "string", "number")))))
        Next

        resp.columns = columns.ToString()
        resp.model = model.ToString()

        '2- dati
        Dim data As New JArray
        For Each row In dt_par_gen.Rows
            Dim row_data As New JObject
            For Each col As DataColumn In dt_par_gen.Columns
                row_data.Add(New JProperty(col.ColumnName, row(col.ColumnName)))
            Next
            data.Add(row_data)
        Next

        resp.data = data.ToString()

        Return resp

    End Function

    Public Function LeggiCoefficientiXSpecie(ByVal PivasuperUser As String,
                                           ByVal Veg_cod As Integer,
                                           ByVal Settimana As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Risposta_DatiReteAcquaPars

        Dim resp As New Risposta_DatiReteAcquaPars

        Dim iotparametri As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R

        Dim dt_parXspecie = iotparametri.Leggi_CoeffXSpecie(PivasuperUser, Veg_cod, Settimana, "", "", objParametri)

        '1- elenco colonne e model
        Dim col_list As New List(Of DatiReteAcqua_ColumnDefinition)

        For Each col As DataColumn In dt_parXspecie.Columns
            Select Case col.ColumnName
                Case "week"
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = "Settimana"})
                Case "Kc"
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = "Coefficiente Colturale (Kc)"})
                Case "DFc"
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = "Deficit (DFc)"})
                Case Else
                    col_list.Add(New DatiReteAcqua_ColumnDefinition() With {.id = col.ColumnName, .desc = col.ColumnName})
            End Select
        Next

        Dim columns As New JArray
        Dim model As New JObject
        For Each col In col_list
            Dim jj = New JObject(New JProperty("field", col.id), New JProperty("title", col.desc), New JProperty("filterable", False), New JProperty("hidden", IIf({"Veg_Cod", "Veg_Des"}.Contains(col.id), True, False)))
            If {"Veg_Cod", "Veg_Des", "week"}.Contains(col.id) = False Then
                jj.Add(New JProperty("format", "{0:n3}"))
                jj.Add(New JProperty("editor", "customNumberEditor"))
            End If
            columns.Add(jj)

            ', New JProperty("width", 20)
            model.Add(New JProperty(col.id, New JObject(New JProperty("editable", IIf({"week", "Veg_Des"}.Contains(col.id), False, True)), New JProperty("type", IIf({"week", "Veg_Des"}.Contains(col.id), "string", "number")))))
        Next

        resp.columns = columns.ToString()
        resp.model = model.ToString()

        '2- dati
        Dim data As New JArray
        For Each row In dt_parXspecie.Rows
            Dim row_data As New JObject
            For Each col As DataColumn In dt_parXspecie.Columns
                row_data.Add(New JProperty(col.ColumnName, row(col.ColumnName)))
            Next
            data.Add(row_data)
        Next

        resp.data = data.ToString()

        Return resp

    End Function

    Public Function LeggiParametriGeneraliDT(ByVal PivasuperUser As String,
                                       ByVal Settimana As Integer,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim iotparametri As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R

        Return iotparametri.Leggi_ParametriGenerali(PivasuperUser, Settimana, "", "", objParametri)

    End Function

    Public Function LeggiCoefficientiXSpecieDT(ByVal PivasuperUser As String,
                                           ByVal Veg_cod As Integer,
                                           ByVal Settimana As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim iotparametri As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R

        Return iotparametri.Leggi_CoeffXSpecie(PivasuperUser, Veg_cod, Settimana, "", "", objParametri)

    End Function
End Class

Public Class DatiReteAcqua_Parametri_W

    Public Function InizializzaParametriGenerali(ByVal PivaSuperUser As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim iotdal_r As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R
        Dim iotdal_w As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim ret As Boolean = False

        Try

            Dim chkDT = iotdal_r.Leggi_ParametriGenerali(PivaSuperUser, 0, "", "", objParametri)
            If chkDT.Rows.Count <= 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

                Dim i = 19
                For i = 19 To 42
                    iotdal_w.Scrivi_ParametriGenerali(objParametri.PivaSuperUser, i, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                Next
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            Else
                Throw New Exception("Parametri già inizializzati. Operazione non permessa")
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

    Public Function ModificaParametriGenerali(ByVal PivaSuperUser As String,
                                              ByVal data As List(Of DatiReteAcqua_ParametriGenerali),
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim iotdal As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim ret As Boolean = False
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)
        Try
            If iotdal.Cancella_ParametriGenerali(objParametri.PivaSuperUser, 0, objParametri) = False Then
                Throw New Exception("Errore in cancellazione parmetri generali - PivaSuperUser: " + objParametri.PivaSuperUser)
            End If
            For Each row In data
                iotdal.Scrivi_ParametriGenerali(objParametri.PivaSuperUser, row.Settimana, row.RadiazioneExtraterrestre, AGRODATAINIZIO, AGRODATAFINE, objParametri)
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

    Public Function AggiungiCoefficientiXSpecie(ByVal PivaSuperUser As String,
                                        ByVal veg_cod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim iotdal_r As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R
        Dim iotdal_w As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim ret As Boolean = False

        Try

            Dim chkDT = iotdal_r.Leggi_CoeffXSpecie(PivaSuperUser, veg_cod, 0, "", "", objParametri)
            If chkDT.Rows.Count <= 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

                Dim i = 19
                For i = 19 To 42
                    iotdal_w.Scrivi_CoeffXSpecie(objParametri.PivaSuperUser, veg_cod, i, 1, 0.5, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                Next
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            Else
                Throw New Exception("Coefficienti per specie vegetale già presenti")
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

    Public Function ModificaCoefficientiXSpecie(ByVal PivaSuperUser As String,
                                                ByVal veg_cod As Integer,
                                                ByVal data As List(Of CoefficientiXSpecieVegetale),
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim iotdal As New AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim ret As Boolean = False
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)
        Try

            If iotdal.Cancella_CoeffXSpecie(objParametri.PivaSuperUser, 0, veg_cod, objParametri) = False Then
                Throw New Exception("Errore in cancellazione dati - PivaSuperUser: " + objParametri.PivaSuperUser + "/ Specie: " + veg_cod.ToString())
            End If
            For Each row In data
                iotdal.Scrivi_CoeffXSpecie(objParametri.PivaSuperUser, veg_cod, row.Settimana, row.Kc, row.DFc, AGRODATAINIZIO, AGRODATAFINE, objParametri)
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
End Class
