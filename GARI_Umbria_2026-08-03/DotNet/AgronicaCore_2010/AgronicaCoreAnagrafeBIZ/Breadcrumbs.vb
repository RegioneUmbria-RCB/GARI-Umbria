Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider

Public Class Breadcrumbs

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Sub New(objParamServer As AgronicaCoreParametri, objParamUtenti As AgronicaCoreParametri)
        _objParametriServer = objParamServer
        _objParametriUtenti = objParamUtenti
    End Sub

    Function GetBreadcrumbs(tipoPagina As BreadcrumbLevel,
                            objParametriAgenda As BreadcrumbParams
                            ) As List(Of Breadcrumb)

        Dim crumbs As New List(Of Breadcrumb)

        Select Case tipoPagina

            Case BreadcrumbLevel.Centro

                Dim breadcrumb = CreateCentroBreadcrumb(objParametriAgenda)
                If breadcrumb IsNot Nothing Then
                    crumbs.Add(breadcrumb)
                End If

            Case BreadcrumbLevel.Campi

                Dim centro = CreateCentroBreadcrumb(objParametriAgenda)
                If centro IsNot Nothing Then
                    crumbs.Add(centro)
                End If

                Dim campo = CreateCampoBreadcrumb(objParametriAgenda)
                If campo IsNot Nothing Then
                    crumbs.Add(campo)
                End If

        End Select

        Return crumbs

    End Function

    Private Function CreateCentroBreadcrumb(objParametriAgenda As BreadcrumbParams) As Breadcrumb

        Dim treeElemId = "3§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod &
                         "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
        Dim rowId = objParametriAgenda.Piva & "_" & objParametriAgenda.Sa_Cod

        Dim objCentri As New CentriAziendali_Read
        If objParametriAgenda.Sa_Cod <> 0 Then
            Dim dt = objCentri.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                                  "", "", _objParametriServer)
            If dt.Rows.Count > 0 Then
                Return New Breadcrumb With {
                    .item = NewCentroBreadcrumbItem(dt),
                    .gridRowId = rowId,
                    .treeElemPendingSelection = treeElemId,
                    .level = BreadcrumbLevel.Centro
                }
            End If
        End If

        Return Nothing

    End Function

    Private Function CreateCampoBreadcrumb(objParametriAgenda As BreadcrumbParams) As Breadcrumb

        Dim treeElemId = "4§" & objParametriAgenda.Piva & "§" & objParametriAgenda.Sa_Cod & "§" &
                         objParametriAgenda.Campo_Cod & "§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
        Dim rowId = objParametriAgenda.Piva & "_" & objParametriAgenda.Sa_Cod & "_" & objParametriAgenda.Campo_Cod

        Dim objCampi As New Campi_R
        If objParametriAgenda.Campo_Cod <> 0 Then
            Dim dt = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                                 objParametriAgenda.Campo_Cod,
                                                 "", "", _objParametriServer)
            If dt.Rows.Count > 0 Then
                Return New Breadcrumb With {
                    .item = NewCampoBreadcrumbItem(dt),
                    .gridRowId = rowId,
                    .treeElemPendingSelection = treeElemId,
                    .level = BreadcrumbLevel.Campi
                }
            End If
        End If

        Return Nothing

    End Function

    Private Function NewCentroBreadcrumbItem(dt As DataTable) As BreadCrumbItem

        Dim nome = dt.Rows(0).Item("sa_nome")

        Return New BreadCrumbItem With {
            .text = nome,
            .title = nome
        }

    End Function

    Private Function NewCampoBreadcrumbItem(dt As DataTable) As BreadCrumbItem

        Dim nome = dt.Rows(0).Item("Campo")

        Return New BreadCrumbItem With {
            .text = nome,
            .title = nome
        }

    End Function

End Class
