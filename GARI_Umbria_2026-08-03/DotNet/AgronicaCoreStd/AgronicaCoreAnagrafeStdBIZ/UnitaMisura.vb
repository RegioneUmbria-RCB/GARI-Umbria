
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD

Public Class UnitaMisura

    Private ReadOnly dbContext As GiasDbContext


    Public Sub New(dbcontext As GiasDbContext)
        Me.dbContext = dbcontext
    End Sub


    Public Function LeggiUnitaMisuraPerOperazione(udmCodBase As AgronicaCoreModelloSTD.UnitaMisura, Lav_Cod As Integer) As List(Of AgronicaCoreModelloSTD.UnitaMisura)

        Dim ListaElemCod As New List(Of Integer)


        ListaElemCod =
            CategorieMagazzino.LeggiCategorieMagazzinoDatoLavCod(Lav_Cod)


        Dim Rval As New List(Of AgronicaCoreModelloSTD.UnitaMisura)

        For Each lElem As enum_CategorieMagazzino In ListaElemCod

            Select Case lElem
                Case enum_CategorieMagazzino.FORMULATI,
                     enum_CategorieMagazzino.FERTILIZZANTI

                    If udmCodBase.udm_cod = enum_Udm_Base.kg OrElse udmCodBase.udm_cod = enum_Udm_Base.NonSpecificata Then
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 3, .udm_des = "Grammi", .udm_sim = "g"}, Rval)
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 2, .udm_des = "Chilogrammi", .udm_sim = "kg"}, Rval)
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 4, .udm_des = "Quintali", .udm_sim = "q"}, Rval)
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 304, .udm_des = "Tonnellate", .udm_sim = "t"}, Rval)
                    End If

                    If udmCodBase.udm_cod = enum_Udm_Base.litri OrElse udmCodBase.udm_cod = enum_Udm_Base.NonSpecificata Then
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 101, .udm_des = "Millilitri", .udm_sim = "ml"}, Rval)
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 104, .udm_des = "Centimetri Cubi", .udm_sim = "cm3"}, Rval)
                        AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 29, .udm_des = "Litri", .udm_sim = "l"}, Rval)
                    End If

                Case enum_CategorieMagazzino.SEMENTI

                    AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 2, .udm_des = "Chilogrammi", .udm_sim = "kg"}, Rval)
                    AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 92, .udm_des = "N. di piante", .udm_sim = "n. piante"}, Rval)
                    AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 93, .udm_des = "Unità di seme", .udm_sim = "u. seme"}, Rval)
                    AggiungiUDM_ConVerifica(New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 1003, .udm_des = "Confezioni", .udm_sim = "Cfz"}, Rval)

            End Select

        Next


        Return Rval

    End Function


    Private Sub AggiungiUDM_ConVerifica(udm As AgronicaCoreModelloSTD.UnitaMisura, ByRef elencoUDM As List(Of AgronicaCoreModelloSTD.UnitaMisura))

        If (From u In elencoUDM Where u.udm_cod = udm.udm_cod).Count = 0 Then
            elencoUDM.Add(udm)
        End If

    End Sub
End Class
