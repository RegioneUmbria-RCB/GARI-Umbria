Public Class MisuraXDanniRaccolta

End Class


Public Class MisuraXDanniRaccolta_input

    Public Veg_Cod As Integer
    Public Dr_Cod As Integer

    Public SoloVisibili As Boolean
    Public strFiltro As String
    Public strOrdinamento As String

    Public Personalizzate As Boolean
    Public Piva_Superuser As String

    Public Url As String

    Public FiltraSpecie As Boolean
    Public joinPersonalizzate As Boolean

    Public EstraiPersonalizzatePerAPP As Boolean

    Sub New()
        Veg_Cod = 0
        Dr_Cod = 0

        SoloVisibili = True
        strFiltro = ""
        strOrdinamento = ""

        Personalizzate = False
        Piva_Superuser = ""

        Url = ""

        FiltraSpecie = True
        joinPersonalizzate = True

        EstraiPersonalizzatePerAPP = False
    End Sub

End Class

Public Class MisuraXDanniRaccolta_output

    Public ListaMisureXDanniRaccolta As List(Of MisuraXDR)
    Public MessaggioErrore As String

    Public Sub New()
        ListaMisureXDanniRaccolta = New List(Of MisuraXDR)
        MessaggioErrore = ""
    End Sub

End Class

Public Class MisuraXDR

    Public Veg_Cod As Integer
    Public Dr_Cod As Integer
    Public Udm_Cod As Integer

    Public Dr_Des As String

    Public Udm_Des As String
    Public Udm_Sim As String

    Public Visibile As Integer 'fondamentale

    Public PivaSuperUser As String

    Sub New()
        Veg_Cod = 0
        Dr_Cod = 0
        Udm_Cod = 0

        Dr_Des = ""

        Udm_Des = ""
        Udm_Sim = ""

        Visibile = 0

        PivaSuperUser = ""
    End Sub

End Class

Public Class MisuraXDR_R

    Public Function MisuraXDanniRaccolta(ByVal Input As MisuraXDanniRaccolta_input,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As MisuraXDanniRaccolta_output

        Dim Output As New MisuraXDanniRaccolta_output

        Dim objCore As New AgronicaCoreMetaSchemaDAL.MisuraXDanniRaccolta_R

        Dim veg_cod As Integer = Input.Veg_Cod

        Dim NoSpecie As Boolean = False

        If veg_cod < 0 Then
            NoSpecie = True
        End If

        Dim Dt As DataTable = objCore.Leggi_WS(Input.Veg_Cod,
                                               Input.Dr_Cod,
                                               Input.SoloVisibili,
                                               Input.strFiltro,
                                               Input.strOrdinamento,
                                               objParametri,
                                               Input.Personalizzate,
                                               Input.Piva_Superuser,
                                               NoSpecie,
                                               Input.FiltraSpecie,
                                               Input.joinPersonalizzate,
                                               Input.EstraiPersonalizzatePerAPP)

        Dim HashInseriti As New Hashtable

        For Each dr As DataRow In Dt.Rows

            If Not HashInseriti.ContainsKey(dr.Item("Dr_Cod") & "|" & dr.Item("Veg_Cod") & "|" & dr.Item("Udm_Cod")) OrElse
                Input.EstraiPersonalizzatePerAPP = True Then

                Dim elem As New MisuraXDR

                elem.Veg_Cod = dr.Item("Veg_Cod")
                elem.Dr_Cod = dr.Item("Dr_Cod")
                elem.Udm_Cod = dr.Item("Udm_Cod")
                elem.Dr_Des = dr.Item("Dr_Des")

                elem.Udm_Des = dr.Item("Udm_Des")
                elem.Udm_Sim = dr.Item("Udm_Sim")

                'elem.Visibile = dr.Item("Fondamentale")
                elem.Visibile = True

                If Input.EstraiPersonalizzatePerAPP = True Then
                    If Not IsDBNull(dr.Item("Piva_superUser")) Then
                        elem.PivaSuperUser = dr.Item("Piva_superUser")
                    End If
                End If
                Output.ListaMisureXDanniRaccolta.Add(elem)

                If Not HashInseriti.ContainsKey(dr.Item("Dr_Cod") & "|" & dr.Item("Veg_Cod") & "|" & dr.Item("Udm_Cod")) Then
                    HashInseriti.Add(dr.Item("Dr_Cod") & "|" & dr.Item("Veg_Cod") & "|" & dr.Item("Udm_Cod"), "")
                End If
            End If

        Next

        Return Output

    End Function


End Class
