Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq

Public Class Algorithm_Extension
    Public Sub AlgorithmRaster_Extension(ByVal LayerAnalysisConfig_Cod As Integer,
                                         ByVal Esecuzione_cod As Integer,
                                         ByVal Esecuzione_GUID As String,
                                         ByVal ParametriEsecuzione As String,
                                         ByVal responseObj As JObject,
                                         ByRef objParametri_Server As AgronicaCoreParametri)
        Dim xCheckL_R As New AgronicaCoreGisBIZ.CheckList_Extension_R
        Dim DTExt As DataTable

        Dim checklistext_factory As New AgronicaAlgoritmiProiezione.CheckListExtenstion_Factory

        DTExt = xCheckL_R.LeggiElencoCheckListAttivePerCodiceAlgoritmo(LayerAnalysisConfig_Cod,
                                                                              DateTime.Now,
                                                                              "",
                                                                              objParametri_Server)
        If DTExt IsNot Nothing Then
            For Each rowExt In DTExt.Rows
                If rowExt("Applica_Risultato") = True Then
                    Dim extention As AgronicaAlgoritmiProiezione.ICheckListExtension
                    Dim ret As Boolean = False
                    extention = checklistext_factory.GetExtensionDaCodice(rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod"),
                                                                      "",
                                                                      objParametri_Server)


                    If responseObj("1.0") IsNot Nothing AndAlso CDbl(responseObj("1.0")) > 0 Then
                        ret = extention.Esegui(Esecuzione_cod,
                                               Esecuzione_GUID,
                                               ParametriEsecuzione,
                                               If(responseObj("1.0") IsNot Nothing AndAlso CDbl(responseObj("1.0")) > 0, responseObj("1.0"), ""),
                                               objParametri_Server)
                        If Not ret Then
                            Throw New Exception(String.Format("Errore in elaborazione CheckList Extension per il tipo {0}", DirectCast(System.Enum.Parse(GetType(enum_GIS_CheckList_Type), rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod")), enum_GIS_CheckList_Type)))
                        End If
                    End If
                    If responseObj("RisultatoOperazione") IsNot Nothing Then
                        ret = extention.Esegui(Esecuzione_cod,
                                                   Esecuzione_GUID,
                                                   ParametriEsecuzione,
                                                   If(JArray.Parse(responseObj("RisultatoOperazione").ToString()).Count = 0, "", responseObj("RisultatoOperazione").ToString()),
                                                   objParametri_Server)
                        If Not ret Then
                            Throw New Exception(String.Format("Errore in elaborazione CheckList Extension per il tipo {0}", DirectCast(System.Enum.Parse(GetType(enum_GIS_CheckList_Type), rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod")), enum_GIS_CheckList_Type)))
                        End If
                    End If
                End If

            Next
        End If
    End Sub

    Public Sub AlgorithmVector_Extension(ByVal LayerAnalysisConfig_Cod As Integer,
                                         ByVal Esecuzione_cod As Integer,
                                         ByVal Esecuzione_GUID As String,
                                         ByVal ParametriEsecuzione As String,
                                         ByVal intersezione As String,
                                         ByRef messaggio As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri)
        Dim xCheckL_R As New AgronicaCoreGisBIZ.CheckList_Extension_R
        Dim DTExt As DataTable

        Dim checklistext_factory As New AgronicaAlgoritmiProiezione.CheckListExtenstion_Factory

        DTExt = xCheckL_R.LeggiElencoCheckListAttivePerCodiceAlgoritmo(LayerAnalysisConfig_Cod,
                                                                           DateTime.Now,
                                                                           "",
                                                                           objParametri_Server)
        If DTExt IsNot Nothing Then
            Try
                For Each rowExt In DTExt.Rows
                    If rowExt("Applica_Risultato") = True Then
                        'Dim extention As AgronicaAlgoritmiProiezione.ICheckListExtension

                        Dim extention = checklistext_factory.GetExtensionDaCodice(rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod"),
                                                                          "",
                                                                          objParametri_Server)

                        Dim ret = extention.Esegui(Esecuzione_cod,
                                                   Esecuzione_GUID,
                                                   ParametriEsecuzione,
                                                   intersezione,
                                                   objParametri_Server)
                        If Not ret Then
                            messaggio &= vbCrLf & String.Format("Errore in elaborazione CheckList Extension per il tipo {0}", DirectCast(System.Enum.Parse(GetType(enum_GIS_CheckList_Type), rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod")), enum_GIS_CheckList_Type))
                        End If
                    End If
                Next
            Catch ex As Exception
                Throw ex
            End Try
        End If


        ''Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione

        'DTExt = xCheckL_R.LeggiElencoCheckListAttivePerCodiceAlgoritmo(LayerAnalysisConfig_Cod,
        '                                                           DateTime.Now,
        '                                                           "",
        '                                                           objParametri_Server)
        'If DTExt IsNot Nothing Then
        '    Try
        '        For Each rowExt In DTExt.Rows
        '            If rowExt("Applica_Risultato") = True Then
        '                'Dim extention As AgronicaAlgoritmiProiezione.ICheckListExtension

        '                Dim extention = checklistext_factory.GetExtensionDaCodice(rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod"),
        '                                                                  "",
        '                                                                  objParametri_Server)

        '                Dim ret = extention.Esegui(Esecuzione_cod,
        '                                           Esecuzione_GUID,
        '                                           ParametriEsecuzione,
        '                                           If(DT.Rows.Count <= 0, "", row("Intersezione").ToString()),
        '                                           objParametri_Server)
        '                If Not ret Then
        '                    messaggio &= vbCrLf & String.Format("Errore in elaborazione CheckList Extension per il tipo {0}", DirectCast(System.Enum.Parse(GetType(enum_GIS_CheckList_Type), rowExt("GIS_LayerAnalysisConfig_CheckList_Type_Cod")), enum_GIS_CheckList_Type))
        '                End If
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End If



    End Sub

End Class
