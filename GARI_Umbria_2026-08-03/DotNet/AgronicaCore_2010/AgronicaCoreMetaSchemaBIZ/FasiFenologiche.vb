Public Class FasiFenologiche
    Public Sub New()

    End Sub

    Public Function FasiFenologiche(ByVal Input As FasiFenologiche_input,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                   As FasiFenologiche_output

        Dim Output As New FasiFenologiche_output
        Dim Elemento As FaseFenologica

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.SpecieVegetaliXStadiCrescita_R

        If Not IsNothing(Input.Lingua_Cod) AndAlso Input.Lingua_Cod > 0 Then
            objParametri.Lingua_Cod = Input.Lingua_Cod
        End If

        Dim SoloRipresaVegetativa As Boolean = False
        If Not IsNothing(Input.SoloRipresaVegetativa) Then
            SoloRipresaVegetativa = Input.SoloRipresaVegetativa
        End If

        Dt = objCore.Leggi(Input.Veg_Cod,
                                   0,
                                   Input.FF_Cod,
                                   Input.SoloVisibili,
                                   Input.SoloFioritura,
                                   SoloRipresaVegetativa,
                                   Input.strFiltro,
                                   Input.strOrdinamento,
                                   objParametri,
                                   Input.Personalizzate,
                                   Input.Piva_Superuser,
                                   Input.EstraiPersonalizzatePerAPP)

        'se non esiste personalizzazione della specie selezionata le restituisco tutte
        If Input.Personalizzate = True AndAlso Dt.Rows.Count = 0 Then
            Dt = objCore.Leggi(Input.Veg_Cod,
                                       0,
                                       Input.FF_Cod,
                                       Input.SoloVisibili,
                                       Input.SoloFioritura,
                                       SoloRipresaVegetativa,
                                       Input.strFiltro,
                                       Input.strOrdinamento,
                                       objParametri,
                                       False,
                                       Input.Piva_Superuser)
        End If


        For i = 0 To Dt.Rows.Count - 1

            Elemento = New FaseFenologica

            Elemento.Cod_SS = Dt.Rows(i).Item("Cod_SS")

            If Not IsDBNull(Dt.Rows(i).Item("descrizione")) Then
                Elemento.Descrizione = Dt.Rows(i).Item("descrizione")
            End If

            Elemento.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")

            If Not IsDBNull(Dt.Rows(i).Item("ID_BBCH")) Then
                Elemento.ID_BBCH = Dt.Rows(i).Item("ID_BBCH")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("ff_cod")) Then
                Elemento.FF_Cod = Dt.Rows(i).Item("ff_cod")
            End If

            Elemento.Visibile = Dt.Rows(i).Item("flag_visibile")
            Elemento.Fioritura = Dt.Rows(i).Item("flag_fioritura")

            If Not IsDBNull(Dt.Rows(i).Item("tipo_scala")) Then
                Select Case Dt.Rows(i).Item("tipo_scala")
                    Case 2
                        Elemento.Stadio = Dt.Rows(i).Item("stadio_principale") & Dt.Rows(i).Item("seconda_cifra")
                    Case 3
                        Elemento.Stadio = Dt.Rows(i).Item("stadio_principale") & Dt.Rows(i).Item("seconda_cifra") & Dt.Rows(i).Item("terza_cifra")
                    Case Else
                        Elemento.Stadio = Dt.Rows(i).Item("stadio_principale")
                End Select
            End If

            If Not IsDBNull(Dt.Rows(i).Item("Flag_RipresaVegetativa")) Then
                Elemento.RipresaVegetativa = Dt.Rows(i).Item("Flag_RipresaVegetativa")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("RipresaVegetativa_GG")) Then
                Elemento.RipresaVegetativa_GG = Dt.Rows(i).Item("RipresaVegetativa_GG")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("RipresaVegetativa_MM")) Then
                Elemento.RipresaVegetativa_MM = Dt.Rows(i).Item("RipresaVegetativa_MM")
            End If

            If Input.EstraiPersonalizzatePerAPP = True Then
                'Stiamo gestendo in APP la casistica dove non esistono personalizzate per la pivasuperuser, e quindi dobbiamo estrarre le fasi fenologiche NON personalizzate
                If Input.Personalizzate = False Then
                    Elemento.PivaSuperUser = ""
                Else
                    If Not IsDBNull(Dt.Rows(i).Item("Piva_superUser")) Then
                        Elemento.PivaSuperUser = Dt.Rows(i).Item("Piva_superUser")
                    End If
                End If
            End If
            Output.ListaFasiFenologiche.Add(Elemento)

        Next

        Return Output

    End Function

    Public Function FasiFenologiche_OLD(ByVal Input As FasiFenologiche_input,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                   As FasiFenologiche_output

        Dim Output As New FasiFenologiche_output
        Dim Elemento As FaseFenologica

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R

        Dt = objCore.Leggi_WS(Input.FF_Cod,
                           Input.Veg_Cod,
                           Input.strFiltro,
                           Input.strOrdinamento,
                            objParametri,
                              Input.Personalizzate,
                              Input.Piva_Superuser)

        'Evidenzio la fase fenologica associata alla fioritura
        Dim objFioritura As New AgronicaCoreMetaSchemaDAL.FasiFenologichexFioriture_R

        Dim FF_Cod_fioritura As Integer = 0
        FF_Cod_fioritura = objFioritura.Fioritura_from_VegCod(Input.Veg_Cod,
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                    "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New FaseFenologica

            Elemento.Cod_SS = 0
            Elemento.Descrizione = Dt.Rows(i).Item("FF_Des")

            Elemento.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")
            Elemento.ID_BBCH = 0
            Elemento.FF_Cod = Dt.Rows(i).Item("ff_cod")
            Elemento.Visibile = 1
            Elemento.Fioritura = 0
            If Dt.Rows(i).Item("ff_cod") = FF_Cod_fioritura Then
                Elemento.Fioritura = 1
            End If

            Output.ListaFasiFenologiche.Add(Elemento)

        Next

        Return Output

    End Function

End Class


Public Class FasiFenologiche_input

    Public Veg_Cod As Integer
    Public FF_Cod As Integer
    Public strFiltro As String
    Public strOrdinamento As String

    Public SoloFioritura As Boolean
    Public SoloVisibili As Boolean
    Public SoloRipresaVegetativa As Boolean

    Public Personalizzate As Boolean
    Public Piva_Superuser As String

    Public Lingua_Cod As Integer

    Public Url As String

    Public EstraiPersonalizzatePerAPP As Boolean

    Sub New()

        Veg_Cod = 0
        FF_Cod = 0
        strFiltro = ""
        strOrdinamento = ""
        SoloVisibili = True
        SoloFioritura = False
        SoloRipresaVegetativa = False
        Personalizzate = False
        Piva_Superuser = ""
        Lingua_Cod = 0
        Url = ""
        EstraiPersonalizzatePerAPP = False
    End Sub

End Class


Public Class APP_IndiciMaturita
    Public Property IND_MAT_COD As Integer
    Public Property IND_MAT_DES As String
    Public Property DATA_AGG As DateTime?
    Public Property LAV_COD As Integer
End Class

Public Class APP_IndiciMaturitaxSpecieVegetali
    Public Property IND_MAT_COD As Integer
    Public Property VEG_COD As Integer
    Public Property REG_COD As Integer
    Public Property CLASSE As String
    Public Property DATA_AGG As DateTime?
    Public Property Flag_Raccolta As Integer?
    Public Property NoSpecieVegetale As Boolean?
End Class

Public Class APP_MisuraXAvversita_Anagrafiche
    Public Property MxAV_Cod As Integer
    Public Property Anag_des As String
    Public Property Anag_valore As Integer
End Class

Public Class APP_MisuraxAvversita
    Public Property COD As Integer
    Public Property UDM_COD As Integer
    Public Property AV_COD As Integer
    Public Property VEG_COD As Integer
    Public Property Fondamentale As Integer?
    Public Property DATA_AGG As DateTime?
    Public Property ff_Cod As Integer?
    Public Property ordine As Integer?
    Public Property AV_GRU As Integer?
End Class

Public Class APP_MisuraXIndiciMaturita_Anagrafiche
    Public Property MxIn_Cod As Integer
    Public Property UDM_COD As Integer
    Public Property Anag_des As String
    Public Property Anag_valore As Integer
End Class

Public Class APP_MisuraXIndiciMaturita
    Public Property IND_MAT_COD As Integer
    Public Property UDM_COD As Integer
    Public Property DATA_AGG As DateTime?
End Class

Public Class APP_SpecieVegetaliXStadiCrescita
    Public Property Cod_SS As Integer
    Public Property Veg_Cod As Integer
    Public Property ID_BBCH As Integer
    Public Property Cod_MS As Integer
    Public Property Progressivo As Integer
    Public Property Descrizione As String
    Public Property FF_Cod As Integer
    Public Property Flag_Fioritura As Integer
    Public Property Flag_Visibile As Integer
End Class

Public Class APP_MisuraXDanniRaccolta
    Public Property Veg_Cod As Integer
    Public Property Dr_Cod As Integer
    Public Property Udm_Cod As Integer
    Public Property Dr_Des As String
    Public Property Udm_Des As String
    Public Property Udm_Sim As String
    Public Property Flag_Visibile As Integer
    Public Property NoSpecieVegetale As Boolean
End Class

Public Class FasiFenologiche_output

    Public ListaFasiFenologiche As List(Of FaseFenologica)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFasiFenologiche = New List(Of FaseFenologica)
        MessaggioErrore = ""

    End Sub

End Class

Public Class FasiFenologiche_output_APP

    Public ListaFasiFenologiche As List(Of APP_SpecieVegetaliXStadiCrescita)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFasiFenologiche = New List(Of APP_SpecieVegetaliXStadiCrescita)
        MessaggioErrore = ""

    End Sub

End Class
Public Class MisuraxAvversita_output_APP

    Public ListaAPP_MisuraxAvversita As List(Of APP_MisuraxAvversita)
    Public ListaAPP_MisuraXAvversita_Anagrafiche As List(Of APP_MisuraXAvversita_Anagrafiche)

    Public MessaggioErrore As String

    Public Sub New()

        ListaAPP_MisuraxAvversita = New List(Of APP_MisuraxAvversita)
        ListaAPP_MisuraXAvversita_Anagrafiche = New List(Of APP_MisuraXAvversita_Anagrafiche)
        MessaggioErrore = ""

    End Sub

End Class
Public Class MisuraXDanniRaccolta_output_APP
    Public ListaAPP_MisuraXDanniRaccolta As List(Of APP_MisuraXDanniRaccolta)
    Public MessaggioErrore As String

    Public Sub New()
        ListaAPP_MisuraXDanniRaccolta = New List(Of APP_MisuraXDanniRaccolta)
        MessaggioErrore = ""
    End Sub
End Class
Public Class IndiciMaturita_output_APP

    Public ListaAPP_IndiciMaturita As List(Of APP_IndiciMaturita)
    Public ListaAPP_MisuraXIndiciMaturita As List(Of APP_MisuraXIndiciMaturita)
    Public ListaAPP_IndiciMaturitaxSpecieVegetali As List(Of APP_IndiciMaturitaxSpecieVegetali)
    Public ListaAPP_MisuraXIndiciMaturita_Anagrafiche As List(Of APP_MisuraXIndiciMaturita_Anagrafiche)

    Public MessaggioErrore As String

    Public Sub New()

        ListaAPP_IndiciMaturita = New List(Of APP_IndiciMaturita)
        ListaAPP_MisuraXIndiciMaturita = New List(Of APP_MisuraXIndiciMaturita)
        ListaAPP_IndiciMaturitaxSpecieVegetali = New List(Of APP_IndiciMaturitaxSpecieVegetali)
        ListaAPP_MisuraXIndiciMaturita_Anagrafiche = New List(Of APP_MisuraXIndiciMaturita_Anagrafiche)
        MessaggioErrore = ""

    End Sub

End Class

Public Class FaseFenologica

    Public Cod_SS As Integer
    Public Veg_Cod As Integer
    Public ID_BBCH As Integer
    Public Descrizione As String
    Public FF_Cod As Integer
    Public Visibile As Integer
    Public Fioritura As Integer
    Public Stadio As String

    Public RipresaVegetativa As Integer
    Public RipresaVegetativa_GG As Integer
    Public RipresaVegetativa_MM As Integer

    Public PivaSuperUser As String

    Sub New()

        Descrizione = ""
        Cod_SS = 0
        Veg_Cod = 0
        ID_BBCH = 0
        FF_Cod = 0
        Visibile = 0
        Fioritura = 0
        Stadio = ""
        RipresaVegetativa = 0
        RipresaVegetativa_GG = 0
        RipresaVegetativa_MM = 0

        PivaSuperUser = ""

    End Sub

End Class

