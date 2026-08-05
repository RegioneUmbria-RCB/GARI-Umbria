Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports System.Collections


 





''' <summary>
''' HELPER
''' </summary>
''' <remarks></remarks>

Public Class PS_Zone_Specie_Varieta_Default_Helper
      

    Public Shared Function GeneraDT_Default_Generale(ByVal ID_Zona As Integer, _
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT_RITORNO As DataTable
        DT_RITORNO = GeneraDTBase()

        Dim objSpecieVegDef As New AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default_R

        'aggiungo le altre colonne (unita calore, gg,resa,cal.1,cal. ..6)
        Dim txt_unita_calore As DataColumn = New DataColumn("unita_calore")
        txt_unita_calore.DataType = System.Type.GetType("System.Decimal")
        DT_RITORNO.Columns.Add(txt_unita_calore)


        Dim txt_gg As DataColumn = New DataColumn("gg")
        txt_gg.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_gg)

        Dim txt_resa As DataColumn = New DataColumn("resa")
        txt_resa.DataType = System.Type.GetType("System.String")
        DT_RITORNO.Columns.Add(txt_resa)

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
        DT_Carica = objSpecieVegDef.Leggi(ID_Zona, _
                                            0, _
                                            0, _
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                             " PS_Zone_Specie_Varieta_Default.Cul_Cod<>0", "", _
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

        'ordino per specie vegetale e unita calore

        Dim dataView As New DataView(DT_RITORNO)
        dataView.Sort = " Veg_Des , Unita_Calore"
        Return dataView.ToTable
    End Function




    Public Shared Function GeneraDT_Default_Generale_Specie(ByVal Id_Zona As Integer, _
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT_RITORNO As DataTable

        'base (veg_cod, cul_cod, cul_des veg_des)
        DT_RITORNO = GeneraDTBase()


        Dim objSpecieVegDef As New AgronicaCoreAnagrafeDAL.PS_Zone_Specie_Varieta_Default_R

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
        DT_Carica = objSpecieVegDef.Leggi(Id_Zona, _
                                            0, _
                                            0, _
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                            " PS_Zone_Specie_Varieta_Default.Cul_cod = 0 AND PS_Zone_Specie_Varieta_Default.Codice in (" & _
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




    Private Shared Function GeneraDTBase()
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

     

End Class

 