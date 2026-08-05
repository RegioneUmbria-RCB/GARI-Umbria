Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports System.Collections


''' <summary>
''' DATI
''' </summary>
''' <remarks></remarks>
Public Class SpecieVegetali_Default_Dettagli
    Private _PIVA As String
    Private _Gru_Cod As Integer
    Private _Veg_Cod As Integer
    Private _Veg_Des As String
    Private _Cul_Cod As Integer
    Private _Cul_Des As String

    Private _ListaDati As List(Of SpecieVegetali_Default_Dettagli_Dati)
    
    Public Property Cul_Cod() As Integer
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As Integer)
            _Cul_Cod = value
        End Set
    End Property
    Public Property Cul_Des() As String
        Get
            Return _Cul_Des
        End Get
        Set(ByVal value As String)
            _Cul_Des = value
        End Set
    End Property
    Public Property Gru_Cod() As Integer
        Get
            Return _Gru_Cod
        End Get
        Set(ByVal value As Integer)
            _Gru_Cod = Value
        End Set
    End Property
    Public Property ListaDati() As List(Of SpecieVegetali_Default_Dettagli_Dati)
        Get
            Return _ListaDati
        End Get
        Set(value As List(Of SpecieVegetali_Default_Dettagli_Dati))
            _ListaDati = value
        End Set
    End Property
    Public Property PIVA() As String
        Get
            Return _PIVA
        End Get
        Set(ByVal value As String)
            _PIVA = value
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property
    Public Property Veg_Des() As String
        Get
            Return _Veg_Des
        End Get
        Set(ByVal value As String)
            _Veg_Des = value
        End Set
    End Property


End Class


Public Class SpecieVegetali_Default
    Private _Dettagli As List(Of SpecieVegetali_Default_Dettagli)
    Public Property Dettagli() As List(Of SpecieVegetali_Default_Dettagli)
        Get
            Return _Dettagli
        End Get
        Set(ByVal value As List(Of SpecieVegetali_Default_Dettagli))
            _Dettagli = value
        End Set
    End Property
End Class



Public Class SpecieVegetali_Default_Dettagli_Dati
    Private _N_Ciclo As Integer
    Private _Data_Semina As String
    Private _Data_Fioritura As String
    Private _Data_Raccolta As String
    Private _Resa As Decimal
    Private _Dosi_ha As Decimal

    Public Property Data_Fioritura() As String
        Get
            Return _Data_Fioritura
        End Get
        Set(value As String)
            _Data_Fioritura = Value
        End Set
    End Property
    Public Property Data_Raccolta() As String
        Get
            Return _Data_Raccolta
        End Get
        Set(value As String)
            _Data_Raccolta = Value
        End Set
    End Property
    Public Property Data_Semina() As String
        Get
            Return _Data_Semina
        End Get
        Set(value As String)
            _Data_Semina = Value
        End Set
    End Property
    Public Property N_Ciclo() As Integer
        Get
            Return _N_Ciclo
        End Get
        Set(value As Integer)
            _N_Ciclo = Value
        End Set
    End Property
    'Public Property Resa() As Decimal
    '    Get
    '        Return _Resa
    '    End Get
    '    Set(value As Decimal)
    '        _Resa = Value
    '    End Set
    'End Property



End Class







''' <summary>
''' HELPER
''' </summary>
''' <remarks></remarks>

Public Class SpecieVegetali_Default_Helper



    Private Const Semina_DT As String = "semina_"
    Private Const Fioritura_DT As String = "fioritura_"
    Private Const Raccolta_DT As String = "raccolta_"

    Public Shared Function GeneraDT_Default_Date(ByVal Piva As String, _
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT_RITORNO As DataTable

        'base 
        DT_RITORNO = GeneraDTBase()

        Dim N_Cicli As Integer

        Dim objSpecieVegDef As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R
        N_Cicli = objSpecieVegDef.CicliCulturali(Piva, _
                                                 True, 0, _
                                                 AGRODATAINIZIO, AGRODATAFINE, _
                                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                 " SpecieVegetali_Default.Codice in (" & Enum_CodiciDefault.Fioritura & "," & _
                                                 Enum_CodiciDefault.Raccolta & " ," & _
                                                 Enum_CodiciDefault.Semina & ")", "", objParametri)

        For i = 0 To N_Cicli - 1
            'aggiungo le 4 colonne per le orticole 
            Dim txt_semina As DataColumn = New DataColumn(Semina_DT & (i + 1))
            txt_semina.DataType = System.Type.GetType("System.String")
            DT_RITORNO.Columns.Add(txt_semina)

            Dim txt_fioritura As DataColumn = New DataColumn(Fioritura_DT & (i + 1))
            txt_fioritura.DataType = System.Type.GetType("System.String")
            DT_RITORNO.Columns.Add(txt_fioritura)

            Dim txt_raccolta As DataColumn = New DataColumn(Raccolta_DT & (i + 1))
            txt_raccolta.DataType = System.Type.GetType("System.String")
            DT_RITORNO.Columns.Add(txt_raccolta)
             
        Next

        'a questo punto ho creato le colonne del dt
        'aggiungo gli elementi

        Dim DT_Carica As DataTable
        DT_Carica = objSpecieVegDef.Leggi(Piva, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            AGRODATAINIZIO, _
                                            AGRODATAFINE, _
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                            " SpecieVegetali_Default.Codice in (" & Enum_CodiciDefault.Fioritura & "," & _
                                                 Enum_CodiciDefault.Raccolta & " ," & _
                                                 Enum_CodiciDefault.Semina & ")", _
                                                 "", _
                                             objParametri)







        If DT_Carica.Rows.Count > 0 Then
            Dim i, j As Integer

            'identifico il numero di righe  (Veg_Cod cul_cod differenti)
            Dim Lista_Cul_Cod As New List(Of String)
            Lista_Cul_Cod.Add(DT_Carica.Rows(0).Item("Veg_Cod") & "_" & DT_Carica.Rows(0).Item("Cul_Cod"))
            Dim Veg_Cod, Cul_Cod As Integer
            For i = 1 To DT_Carica.Rows.Count - 1
                Veg_Cod = DT_Carica.Rows(i).Item("Veg_Cod")
                Cul_Cod = DT_Carica.Rows(i).Item("Cul_Cod")

                If CStr(Veg_Cod & "_" & Cul_Cod) <> Lista_Cul_Cod(Lista_Cul_Cod.Count - 1) Then
                    Lista_Cul_Cod.Add(CStr(Veg_Cod & "_" & Cul_Cod))
                End If
            Next


            Dim dr() As DataRow
            Dim DR_NEW As DataRow
            For i = 0 To Lista_Cul_Cod.Count - 1

                dr = DT_Carica.Select(" Veg_Cod =" & Lista_Cul_Cod(i).Split("_")(0) & _
                                      " AND Cul_Cod = " & Lista_Cul_Cod(i).Split("_")(1))
                DR_NEW = DT_RITORNO.NewRow


                DR_NEW.Item("Veg_Cod") = dr(0).Item("Veg_Cod")
                DR_NEW.Item("Cul_Cod") = dr(0).Item("Cul_Cod")

                DR_NEW.Item("Veg_Des") = dr(0).Item("Veg_Des")

                If Not IsDBNull(dr(0).Item("Cul_des")) Then
                    DR_NEW.Item("Cul_des") = dr(0).Item("Cul_des")
                Else
                    DR_NEW.Item("Cul_des") = "Tutte le Varietà"
                End If



                For j = 0 To dr.Length - 1
                    Dim nomeColonna As String = ""
                    Dim N_Ciclo As Integer
                    N_Ciclo = dr(j).Item("Numero_Ciclo")

                    Select Case dr(j).Item("Codice")
                        Case Enum_CodiciDefault.Fioritura
                            nomeColonna = Fioritura_DT & N_Ciclo

                        Case Enum_CodiciDefault.Raccolta
                            nomeColonna = Raccolta_DT & N_Ciclo
                             
                        Case Enum_CodiciDefault.Semina
                            nomeColonna = Semina_DT & N_Ciclo
                    End Select
                    If nomeColonna <> "" Then
                        DR_NEW.Item(nomeColonna) = dr(j).Item("valore")
                    End If

                Next
                DT_RITORNO.Rows.Add(DR_NEW)
            Next

            'a questo punto ho il DT_New inizializzato
        End If




        Return DT_RITORNO
    End Function


    Public Shared Function GeneraDT_Default_Generale(ByVal Piva As String, _
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT_RITORNO As DataTable

        'base (veg_cod, cul_cod, cul_des veg_des)
        DT_RITORNO = GeneraDTBase()


        Dim objSpecieVegDef As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R
       
        'aggiungo le altre colonne (unita calore, gg,resa,cal.1,cal. ..6)
        Dim txt_unita_calore As DataColumn = New DataColumn("unita_calore")
        txt_unita_calore.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_unita_calore)

        Dim txt_gg As DataColumn = New DataColumn("gg")
        txt_gg.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_gg)

        Dim txt_resa As DataColumn = New DataColumn("resa")
        txt_resa.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_resa)

        Dim dosi_ha As DataColumn = New DataColumn("dosi_ha")
        dosi_ha.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(dosi_ha)

        Dim dosi_ha_udm As DataColumn = New DataColumn("dosi_ha_udm")
        dosi_ha_udm.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(dosi_ha_udm)

        Dim txt_cal_1 As DataColumn = New DataColumn("cal_1")
        txt_cal_1.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_cal_1)

        Dim txt_cal_2 As DataColumn = New DataColumn("cal_2")
        txt_cal_2.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_cal_2)

        Dim txt_cal_3 As DataColumn = New DataColumn("cal_3")
        txt_cal_3.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_cal_3)

        Dim txt_cal_4 As DataColumn = New DataColumn("cal_4")
        txt_cal_4.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_cal_4)

        Dim txt_cal_5 As DataColumn = New DataColumn("cal_5")
        txt_cal_5.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_cal_5)

        Dim txt_cal_6 As DataColumn = New DataColumn("cal_6")
        txt_cal_6.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_cal_6)

        'aggiungo gli elementi
        Dim DT_Carica As DataTable
        DT_Carica = objSpecieVegDef.Leggi(Piva, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            AGRODATAINIZIO, _
                                            AGRODATAFINE, _
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                             " SpecieVegetali_Default.Cul_Cod<>0", "", _
                                             objParametri)



        If DT_Carica.Rows.Count > 0 Then
            Dim i, j As Integer

            'identifico il numero di righe  (Veg_Cod cul_cod differenti)
            Dim Hash_distinct_Veg_Cod_Cul_Cod As New Hashtable
            Dim list_veg_cul As New List(Of String)
            Dim Veg_Cod, Cul_Cod As Integer

            For i = 0 To DT_Carica.Rows.Count - 1
                Veg_Cod = DT_Carica.Rows(i).Item("Veg_Cod")
                Cul_Cod = DT_Carica.Rows(i).Item("Cul_Cod")
                If Hash_distinct_Veg_Cod_Cul_Cod.ContainsKey(Veg_Cod & "_" & Cul_Cod) = False Then
                    Hash_distinct_Veg_Cod_Cul_Cod.Add((Veg_Cod & "_" & Cul_Cod), (Veg_Cod & "_" & Cul_Cod))
                    list_veg_cul.Add((Veg_Cod & "_" & Cul_Cod))
                End If
            Next


            Dim dr() As DataRow
            Dim DR_NEW As DataRow

            'scorro tutte le specie e varietà presenti nel db
            For i = 0 To list_veg_cul.Count - 1
                 

                'filtro 
                dr = DT_Carica.Select(" Veg_Cod =" & list_veg_cul(i).Split("_")(0) & _
                                      " AND Cul_Cod = " & list_veg_cul(i).Split("_")(1))
                DR_NEW = DT_RITORNO.NewRow

                DR_NEW.Item("Veg_Cod") = dr(0).Item("Veg_Cod")
                DR_NEW.Item("Cul_Cod") = dr(0).Item("Cul_Cod")

                DR_NEW.Item("Veg_Des") = dr(0).Item("Veg_Des")
                If Not IsDBNull(dr(0).Item("Cul_des")) Then
                    DR_NEW.Item("Cul_des") = dr(0).Item("Cul_des")
                Else
                    DR_NEW.Item("Cul_des") = "Tutte le Varietà"
                End If


                For j = 0 To dr.Length - 1
                    Dim nomeColonna As String = ""

                    Select Case dr(j).Item("Codice")
                        Case Enum_CodiciDefault.Unita_Calore
                            nomeColonna = "Unita_Calore"

                        Case Enum_CodiciDefault.GG
                            nomeColonna = "gg"

                        Case Enum_CodiciDefault.Resa
                            nomeColonna = "Resa"

                        Case Enum_CodiciDefault.Dosi_Ha
                            nomeColonna = "dosi_ha"

                        Case Enum_CodiciDefault.Dosi_Ha_Udm
                            nomeColonna = "dosi_ha_udm"

                        Case Enum_CodiciDefault.Cal_1
                            nomeColonna = "Cal_1"
                        Case Enum_CodiciDefault.Cal_2
                            nomeColonna = "Cal_2"
                        Case Enum_CodiciDefault.Cal_3
                            nomeColonna = "Cal_3"
                        Case Enum_CodiciDefault.Cal_4
                            nomeColonna = "Cal_4"
                        Case Enum_CodiciDefault.Cal_5
                            nomeColonna = "Cal_5"
                        Case Enum_CodiciDefault.Cal_6
                            nomeColonna = "Cal_6"
                    End Select
                    If nomeColonna <> "" Then
                        DR_NEW.Item(nomeColonna) = dr(j).Item("valore")
                    End If
                Next
                DT_RITORNO.Rows.Add(DR_NEW)
            Next
            'a questo punto ho il DT_New inizializzato
        End If
         

        Return DT_RITORNO
    End Function




    Public Shared Function GeneraDT_Default_Generale_Specie(ByVal Piva As String, _
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT_RITORNO As DataTable

        'base (veg_cod, cul_cod, cul_des veg_des)
        DT_RITORNO = GeneraDTBase()


        Dim objSpecieVegDef As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R

        'aggiungo le altre colonne  
        Dim txt_tipo_maturazione As DataColumn = New DataColumn("tipo_maturazione")
        txt_tipo_maturazione.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_tipo_maturazione)

        Dim Soglia_Minima As DataColumn = New DataColumn("Soglia_Minima")
        Soglia_Minima.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(Soglia_Minima)

        Dim resa_stabilimento As DataColumn = New DataColumn("resa_stabilimento")
        resa_stabilimento.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(resa_stabilimento)

        Dim peso_sgocciolato As DataColumn = New DataColumn("peso_sgocciolato")
        peso_sgocciolato.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(peso_sgocciolato)


        'aggiungo gli elementi
        Dim DT_Carica As DataTable
        DT_Carica = objSpecieVegDef.Leggi(Piva, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            AGRODATAINIZIO, _
                                            AGRODATAFINE, _
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                             " SpecieVegetali_Default.Cul_cod = 0 AND SpecieVegetali_Default.Codice in (" & _
                                             Enum_CodiciDefault.peso_sgocciolato & "," & _
                                             Enum_CodiciDefault.resa_stabilimento & "," & _
                                             Enum_CodiciDefault.Soglia_Minima & "," & _
                                             Enum_CodiciDefault.tipo_maturazione & ")", "", _
                                             objParametri)



        If DT_Carica.Rows.Count > 0 Then
            Dim i, j As Integer

            'identifico il numero di righe  (Veg_Cod cul_cod differenti)
            Dim Hash_distinct_Veg_Cod_Cul_Cod As New Hashtable
            Dim list_veg_cul As New List(Of String)
            Dim Veg_Cod As Integer

            For i = 0 To DT_Carica.Rows.Count - 1
                Veg_Cod = DT_Carica.Rows(i).Item("Veg_Cod")
                'Cul_Cod = DT_Carica.Rows(i).Item("Cul_Cod")
                If Hash_distinct_Veg_Cod_Cul_Cod.ContainsKey(Veg_Cod) = False Then
                    Hash_distinct_Veg_Cod_Cul_Cod.Add((Veg_Cod), (Veg_Cod))
                    list_veg_cul.Add((Veg_Cod))
                End If
            Next


            Dim dr() As DataRow
            Dim DR_NEW As DataRow

            'scorro tutte le specie e varietà presenti nel db
            For i = 0 To list_veg_cul.Count - 1

                'filtro 
                dr = DT_Carica.Select(" Veg_Cod =" & list_veg_cul(i).Split("_")(0))
                DR_NEW = DT_RITORNO.NewRow

                DR_NEW.Item("Veg_Cod") = dr(0).Item("Veg_Cod")
                DR_NEW.Item("Cul_Cod") = 0

                DR_NEW.Item("Veg_Des") = dr(0).Item("Veg_Des")
                If Not IsDBNull(dr(0).Item("Cul_des")) Then
                    DR_NEW.Item("Cul_des") = dr(0).Item("Cul_des")
                Else
                    DR_NEW.Item("Cul_des") = "Tutte le Varietà"
                End If


                For j = 0 To dr.Length - 1
                    Dim nomeColonna As String = ""

                    Select Case dr(j).Item("Codice")
                        Case Enum_CodiciDefault.Unita_Calore
                            nomeColonna = "Unita_Calore"

                        Case Enum_CodiciDefault.peso_sgocciolato
                            nomeColonna = "peso_sgocciolato"

                        Case Enum_CodiciDefault.resa_stabilimento
                            nomeColonna = "resa_stabilimento"

                        Case Enum_CodiciDefault.Soglia_Minima
                            nomeColonna = "Soglia_Minima"
                    End Select
                    If nomeColonna <> "" Then
                        DR_NEW.Item(nomeColonna) = dr(j).Item("valore")
                    End If
                Next
                DT_RITORNO.Rows.Add(DR_NEW)
            Next
            'a questo punto ho il DT_New inizializzato
        End If


        Return DT_RITORNO
    End Function




    Public Shared Function GeneraDTBase()
        Dim DT As New DataTable

        Dim Veg_Cod As DataColumn = New DataColumn("Veg_Cod")
        Veg_Cod.DataType = System.Type.GetType("System.Int32")
        DT.Columns.Add(Veg_Cod)

        Dim Cul_Cod As DataColumn = New DataColumn("Cul_Cod")
        Cul_Cod.DataType = System.Type.GetType("System.Int32")
        DT.Columns.Add(Cul_Cod)

        Dim Veg_Des As DataColumn = New DataColumn("Veg_Des")
        Veg_Des.DataType = System.Type.GetType("System.String")
        DT.Columns.Add(Veg_Des)

        Dim Cul_des As DataColumn = New DataColumn("Cul_des")
        Cul_des.DataType = System.Type.GetType("System.String")
        DT.Columns.Add(Cul_des)

        Return DT
    End Function



    Public Shared Function Salva(ByVal oggetto As SpecieVegetali_Default, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Risp As String = ""
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)



            'elimino i vecchi default 
            Dim Veg_Cod As Integer
            Dim Piva As String

            Veg_Cod = oggetto.Dettagli(0).Veg_Cod
            Piva = oggetto.Dettagli(0).PIVA

            Dim objSpeCancella As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_W
            objSpeCancella.Cancella(Piva, Veg_Cod, -1, "", objParametri)


            'per ogni cul_cod (ogni dettaglio)
            For i = 0 To oggetto.Dettagli.Count - 1
                'salvo il dettaglio

                Risp = SpecieVegetali_Default_Dettagli_Helper.Salva(oggetto.Dettagli(i), objParametri)
                If Risp <> "" Then
                    Throw New Exception(Risp)
                End If
            Next

            Utility.VerificaChiudiTransazione(objParametri, _
                                              Flag_Transazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function

    Public Shared Function Cancella(ByVal Piva As String, ByVal Veg_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Risp As String = ""
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)


            'ELiminazione


            Utility.VerificaChiudiTransazione(objParametri, _
                                               Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function

End Class



Public Class SpecieVegetali_Default_Dettagli_Helper


    Public Shared Function Salva(ByVal oggetto As SpecieVegetali_Default_Dettagli, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Risp As String = ""
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)



            'elimino i vecchi default 
            Dim Veg_Cod As Integer
            Dim Cul_Cod As Integer
            Dim Piva As String


            Veg_Cod = oggetto.Veg_Cod
            Piva = oggetto.PIVA
            Cul_Cod = oggetto.Cul_Cod

            'salvataggio
            Dim i, j As Integer

            'per ogni singolo ciclo culturale 
            For i = 0 To oggetto.ListaDati.Count - 1

                'salvo i singoli dati

                Risp = SpecieVegetali_Default_Dettagli_Dati_Helper.Salva(Veg_Cod, _
                                                                Cul_Cod, _
                                                                Piva, _
                                                                oggetto.ListaDati(i), _
                                                                objParametri)
                If Risp <> "" Then
                    Throw New Exception(Risp)
                End If
            Next

            Utility.VerificaChiudiTransazione(objParametri, _
                                              Flag_Transazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function

End Class






Public Class SpecieVegetali_Default_Dettagli_Dati_Helper

    Public Structure Codice_Valore
        Dim Codice As Integer
        Dim Valore As String
    End Structure


    Public Shared Function Salva(ByVal Veg_Cod As Integer, _
                                 ByVal Cul_Cod As Integer, _
                                 ByVal Piva As String, _
                                 ByVal oggetto As SpecieVegetali_Default_Dettagli_Dati, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Risp As String = ""
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)

            Dim Numero_Ciclo As Integer
            Numero_Ciclo = oggetto.N_Ciclo
            If Numero_Ciclo = 0 Then
                Numero_Ciclo = 1
            End If

            Dim lista As New List(Of Codice_Valore)
            'Semina
            If oggetto.Data_Semina <> "" Then
                Dim oggLista As New Codice_Valore
                oggLista.Codice = Enum_CodiciDefault.Semina
                oggLista.Valore = oggetto.Data_Semina
                lista.Add(oggLista)
            End If

            'Fioruitura
            If oggetto.Data_Fioritura <> "" Then
                Dim oggLista As New Codice_Valore
                oggLista.Codice = Enum_CodiciDefault.Fioritura
                oggLista.Valore = oggetto.Data_Fioritura
                lista.Add(oggLista)
            End If

            'Raccolta
            If oggetto.Data_Raccolta <> "" Then
                Dim oggLista As New Codice_Valore
                oggLista.Codice = Enum_CodiciDefault.Raccolta
                oggLista.Valore = oggetto.Data_Raccolta
                lista.Add(oggLista)
            End If
             

            Dim obj As New AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_W
            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim id As Integer
            Dim i As Integer
            For i = 0 To lista.Count - 1
                id = objSeq.NuovoId_Tabella("SpecieVegetali_Default", 0, 2000000000, objParametri)

                If obj.Scrivi(id, Piva, Veg_Cod, Cul_Cod, lista(i).Codice, lista(i).Valore, _
                             Numero_Ciclo, _
                             AGRODATAINIZIO, _
                             AGRODATAFINE, _
                             objParametri) = False Then
                    Throw New Exception("Errore in scrittura... SpecieVegetali_Default_Dettagli_Dati_Helper")
                End If

            Next

            Utility.VerificaChiudiTransazione(objParametri, _
                                              Flag_Transazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function

End Class

