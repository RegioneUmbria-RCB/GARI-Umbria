Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

<CachedDataProviderAttribute("UnitaMisura_R")>
Public Class UnitaMisura_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function Converti_A_Grammi_Millilitri(ByVal Udm_Cod_1 As Integer, ByVal Quantita_da_Convertire As Decimal, ByRef udm_Finale As Integer) As Decimal

        Dim Quantita_da_Convertire_Trasformata As Decimal

        'passo a litri o kg
        Select Case Udm_Cod_1
            Case enum_UnitaMisura.Grammi
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire / 1000
                udm_Finale = enum_UnitaMisura.Grammi
            Case enum_UnitaMisura.Milligrammi
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire / 1000000
                udm_Finale = enum_UnitaMisura.Grammi
            Case enum_UnitaMisura.Quintali
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire * 100
                udm_Finale = enum_UnitaMisura.Grammi
            Case enum_UnitaMisura.Tonnellate
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire * 1000
                udm_Finale = enum_UnitaMisura.Grammi

            Case enum_UnitaMisura.Millilitri
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire / 1000
                udm_Finale = enum_UnitaMisura.Millilitri
            Case enum_UnitaMisura.CentimetriCubi
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire / 1000
                udm_Finale = enum_UnitaMisura.Millilitri
            Case enum_UnitaMisura.Metri_Cubi
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire * 1000
                udm_Finale = enum_UnitaMisura.Millilitri

            Case enum_UnitaMisura.KG
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire
                udm_Finale = enum_UnitaMisura.Grammi
            Case enum_UnitaMisura.Litri
                Quantita_da_Convertire_Trasformata = Quantita_da_Convertire
                udm_Finale = enum_UnitaMisura.Millilitri

        End Select

        'passo a millilitri o grammi
        Return Quantita_da_Convertire_Trasformata * 1000

    End Function

    Public Shared Function Converti(ByVal Udm_Cod_1 As Integer, ByVal Quantita_da_Convertire As Decimal,
                                    ByVal Udm_Cod_2 As Integer) As Decimal

        Select Case Udm_Cod_1

            'partendo dal Tonnellate
            Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.Grammi__HL, enum_UnitaMisura.Grammi__Litro
                        Return Quantita_da_Convertire * 1000000

                    Case enum_UnitaMisura.Quintali, enum_UnitaMisura.Ettolitro
                        Return Quantita_da_Convertire * 10

                    Case enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Litri
                        Return Quantita_da_Convertire * 1000

                    Case enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Milligrammi__HL
                        Return Quantita_da_Convertire * 1000000000
                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Quintali 
            Case enum_UnitaMisura.Quintali, enum_UnitaMisura.Ettolitro
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Litri
                        Return Quantita_da_Convertire * 100

                    Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.Grammi__HL, enum_UnitaMisura.Grammi__Litro

                        Return Quantita_da_Convertire * 100000

                    Case enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Milligrammi__HL
                        Return Quantita_da_Convertire * 100000000

                    Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 10
                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal KG, KG__HA
            Case enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Chilogrammi__HL
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.Grammi__HL, enum_UnitaMisura.Grammi__Litro
                        Return Quantita_da_Convertire * 1000

                    Case enum_UnitaMisura.Quintali, enum_UnitaMisura.Ettolitro
                        Return Quantita_da_Convertire / 100

                    Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 1000

                    Case enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Milligrammi__HL
                        Return Quantita_da_Convertire * 1000000

                    Case enum_UnitaMisura.Litri
                        Return Quantita_da_Convertire

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Grammi , Grammi__HA
            Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.Grammi__HL
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Litri
                        Return Quantita_da_Convertire / 1000

                    Case enum_UnitaMisura.Quintali, enum_UnitaMisura.Ettolitro
                        Return Quantita_da_Convertire / 100000

                    Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 1000000

                    Case enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Milligrammi__HL
                        Return Quantita_da_Convertire * 1000

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                 'partendo dal Grammi__Litro 
            Case enum_UnitaMisura.Grammi__Litro
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA,
                         enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA,
                        enum_UnitaMisura.Quintali, enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.Metri_Cubi, enum_UnitaMisura.Ettolitro
                        Return Converti(enum_UnitaMisura.Grammi__HL, Udm_Cod_2, Quantita_da_Convertire)

                    Case enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Milligrammi__HL, enum_UnitaMisura.Grammi__HL
                        Return Converti(enum_UnitaMisura.Grammi__HL, Udm_Cod_2, Quantita_da_Convertire / 100)

                    Case enum_UnitaMisura.Milligrammi__Litro
                        Return Quantita_da_Convertire * 1000

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Milligrammi__Litro 
            Case enum_UnitaMisura.Milligrammi__Litro
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA,
                        enum_UnitaMisura.Quintali, enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.Metri_Cubi, enum_UnitaMisura.Ettolitro
                        Return Converti(enum_UnitaMisura.Milligrammi, Udm_Cod_2, Quantita_da_Convertire)

                    Case enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Grammi__Litro, enum_UnitaMisura.Grammi__HL
                        Return Converti(enum_UnitaMisura.Milligrammi__HL, Udm_Cod_2, Quantita_da_Convertire / 100)

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Milligrammi 
            Case enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Milligrammi__HL
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Litri
                        Return Quantita_da_Convertire / 100000

                    Case enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.Grammi__HL
                        Return Quantita_da_Convertire / 1000

                    Case enum_UnitaMisura.Quintali, enum_UnitaMisura.Ettolitro
                        Return Quantita_da_Convertire / 100000000

                    Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 1000000000

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select


                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                'partendo dal Millilitri enum_UnitaMisura.CentimetriCubi
            Case enum_UnitaMisura.Millilitri, enum_UnitaMisura.CentimetriCubi, enum_UnitaMisura.Millilitri__Ha, enum_UnitaMisura.CC__HL, enum_UnitaMisura.Millilitri__HL
                Select Case Udm_Cod_2

                    Case enum_UnitaMisura.Litri, enum_UnitaMisura.Litro__HA, enum_UnitaMisura.Litri__HL, enum_UnitaMisura.KG
                        Return Quantita_da_Convertire / 1000

                    Case enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 1000000

                    Case enum_UnitaMisura.Ettolitro
                        Return Quantita_da_Convertire / 100000

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Litri 
            Case enum_UnitaMisura.Litri, enum_UnitaMisura.Litro__HA, enum_UnitaMisura.Litri__HL
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Millilitri, enum_UnitaMisura.CentimetriCubi, enum_UnitaMisura.Millilitri__Ha, enum_UnitaMisura.CC__HL, enum_UnitaMisura.Millilitri__HL
                        Return Quantita_da_Convertire * 1000

                    Case enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 1000

                    Case enum_UnitaMisura.Ettolitro
                        Return Quantita_da_Convertire / 100

                    Case enum_UnitaMisura.KG
                        Return Quantita_da_Convertire

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Metri_Cubi 
            Case enum_UnitaMisura.Metri_Cubi
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Milligrammi, enum_UnitaMisura.Milligrammi__HL
                        Return Quantita_da_Convertire * 1000000000

                    Case enum_UnitaMisura.Millilitri, enum_UnitaMisura.CentimetriCubi, enum_UnitaMisura.Millilitri__Ha, enum_UnitaMisura.CC__HL, enum_UnitaMisura.Millilitri__HL,
                         enum_UnitaMisura.Grammi, enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.Grammi__HL, enum_UnitaMisura.Grammi__Litro
                        Return Quantita_da_Convertire * 1000000

                    Case enum_UnitaMisura.Litri, enum_UnitaMisura.Litro__HA, enum_UnitaMisura.Litri__HL,
                         enum_UnitaMisura.KG, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Chilogrammi__HL
                        Return Quantita_da_Convertire * 1000

                    Case enum_UnitaMisura.Ettolitro,
                         enum_UnitaMisura.Quintali
                        Return Quantita_da_Convertire * 10

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                'partendo dal Metri_Cubi 
            Case enum_UnitaMisura.Ettolitro
                Select Case Udm_Cod_2
                    Case enum_UnitaMisura.Millilitri, enum_UnitaMisura.CentimetriCubi, enum_UnitaMisura.Millilitri__Ha, enum_UnitaMisura.CC__HL, enum_UnitaMisura.Millilitri__HL
                        Return Quantita_da_Convertire * 100000

                    Case enum_UnitaMisura.Litri, enum_UnitaMisura.Litro__HA, enum_UnitaMisura.Litri__HL, enum_UnitaMisura.KG
                        Return Quantita_da_Convertire * 100

                    Case enum_UnitaMisura.Metri_Cubi
                        Return Quantita_da_Convertire / 10

                    Case Else
                        Throw New Exception("Conversione non Gestita")
                End Select

                '''''''''''''''''''''''''''''''''''''''''''''''''''''

        End Select

    End Function

    ''' <summary>
    ''' identifica se la dose è una dose a ettaro o a ettolitro
    ''' </summary>
    ''' <param name="UDM"></param>
    ''' <returns>ritrona enum_HL_HA.HL o enum_HL_HA.HA </returns>
    ''' <remarks></remarks>
    Public Shared Function Identifica_doseHL_doseHA(ByRef UDM As Integer)
        Select Case UDM
            Case enum_UnitaMisura.CC__HL, enum_UnitaMisura.Chilogrammi__HL, enum_UnitaMisura.Grammi__HL, enum_UnitaMisura.Litri__HL, enum_UnitaMisura.Milligrammi__HL,
                enum_UnitaMisura.Millilitri__HL
                Return enum_HL_HA.HL
            Case enum_UnitaMisura.Grammi__HA, enum_UnitaMisura.METRI3__HA, enum_UnitaMisura.UNITA__HA, enum_UnitaMisura.KG__HA, enum_UnitaMisura.Litro__HA,
                enum_UnitaMisura.Millilitri__Ha, enum_UnitaMisura.Tonnellate__HA, enum_UnitaMisura.NumUnita__HA, enum_UnitaMisura.QUINTALI__HA
                Return enum_HL_HA.HA
            Case Else
                Throw New Exception("Non è possibile identificare se si tratta di una dose a ettaro o a ettolitro")
        End Select
    End Function

    Public Sub ScomponiUdm(ByRef UDM_radice As Integer,
                          ByRef perHa_hl As Integer,
                          ByRef UnitaMisura As Integer,
                           Optional ByRef MoltiplicatoreDose As Decimal = 1)

        Select Case UnitaMisura

            Case enum_UnitaMisura.Millilitri__Litro '2016 ml/l
                UDM_radice = 101
                perHa_hl = 2121
                MoltiplicatoreDose = 100

            Case enum_UnitaMisura.Grammi__Litro '2003 g/l
                UDM_radice = 3
                perHa_hl = 2121
                MoltiplicatoreDose = 100
            '---------------------
            ' a HL
            Case 21  'cc/hl
                UDM_radice = 104
                perHa_hl = 2121
            Case 23 'g/hl
                UDM_radice = 3
                perHa_hl = 2121
            Case 126 'mg/hl
                UDM_radice = 2032
                perHa_hl = 2121
            Case 164  'ml/hl
                UDM_radice = 101
                perHa_hl = 2121
            Case 173 'l/hl
                UDM_radice = 29
                perHa_hl = 2121
            Case 175  'kg/hl
                UDM_radice = 2
                perHa_hl = 2121

                '---------------------
                'a HA
            Case 20  'g/ha
                UDM_radice = 3
                perHa_hl = 2123
            Case 22  'l/ha
                UDM_radice = 29
                perHa_hl = 2123
            Case 88  'kg/ha
                UDM_radice = 2
                perHa_hl = 2123

            Case enum_UnitaMisura.UNITA__HA  'unita/ha
                UDM_radice = enum_UnitaMisura.UNITA
                perHa_hl = 2123

            Case 90  'm3/ha
                UDM_radice = -1
                perHa_hl = 2123

            Case 163  'ml/ha
                UDM_radice = 101
                perHa_hl = 2123
            Case 176  'n° u/ha
                perHa_hl = 2123

            Case 2098  't/ha
                UDM_radice = 304
                perHa_hl = 2123

            Case 2112 't/ha spighe
                UDM_radice = 304
                perHa_hl = 2123

            Case 2120  'q/ha
                UDM_radice = 4
                perHa_hl = 2123

            Case enum_UnitaMisura.Numero_Diffusori_HA 'n. diffusori/ha
                UDM_radice = enum_UnitaMisura.Numero_Diffusori
                perHa_hl = 2123

                '--------------------
            Case 2 'kg
                UDM_radice = 2
                perHa_hl = 0
            Case 4 'q
                UDM_radice = 4
                perHa_hl = 0
            Case 29 'l
                UDM_radice = 29
                perHa_hl = 0
            Case 101 'ml
                UDM_radice = 101
                perHa_hl = 0
            Case 3   'g
                UDM_radice = 3
                perHa_hl = 0
            Case 104 'cc
                UDM_radice = 104
                perHa_hl = 0
            Case 304 't
                UDM_radice = 304
                perHa_hl = 0
            Case 2032 'mg
                UDM_radice = 2032
                perHa_hl = 0
            Case enum_UnitaMisura.Numero_Trappole 'numero trappole
                UDM_radice = enum_UnitaMisura.Numero_Trappole
                perHa_hl = 0

                '--------------------
                'concianti
            Case 2004   'l/100 kg di seme
                UDM_radice = 29
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2005   'ml/100 kg di seme	
                UDM_radice = 101
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2017   'ml/100 kg di semi	
                UDM_radice = 101
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2021   'ml/kg di semente	
                UDM_radice = 101
                perHa_hl = enum_UnitaMisura.Quintali
            Case 5001003    'ml/unità di seme	
                UDM_radice = 101
                perHa_hl = 0
            Case 2006   'g/unita' di seme	
                UDM_radice = 3
                perHa_hl = 0
            Case 2007   'g/100 kg di semente	
                UDM_radice = 3
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2010   'kg/100 kg di seme
                UDM_radice = 2
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2026  'kg/1 tonnellata di semente
                UDM_radice = 2
                perHa_hl = enum_UnitaMisura.Tonnellate

            Case 2030 'litri/1.000 piante
                UDM_radice = 29
                perHa_hl = 0
            Case 2018 'grammi/pianta
                UDM_radice = 3
                perHa_hl = 0
            Case 170 'ml/pianta
                UDM_radice = 101
                perHa_hl = 0


                 '--------------------
                 'post raccolta
            Case enum_UnitaMisura.Millilitri__Quintale
                UDM_radice = enum_UnitaMisura.Millilitri
                perHa_hl = enum_UnitaMisura.Quintali

            Case enum_UnitaMisura.KG__Quintale
                UDM_radice = enum_UnitaMisura.KG
                perHa_hl = enum_UnitaMisura.Quintali

            Case enum_UnitaMisura.Grammi__Quintale
                UDM_radice = enum_UnitaMisura.Grammi
                perHa_hl = enum_UnitaMisura.Quintali

            Case enum_UnitaMisura.Litri__Quintale
                UDM_radice = enum_UnitaMisura.Litri
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2015  'ml / 100 kg di bulbilli
                UDM_radice = enum_UnitaMisura.Millilitri
                perHa_hl = enum_UnitaMisura.Quintali
            Case 2009  'ml/1 t di prodotto
                UDM_radice = enum_UnitaMisura.Millilitri
                perHa_hl = enum_UnitaMisura.Tonnellate
        End Select

    End Sub



    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi(ByVal Udm_Cod As Integer,
                            ByVal Udm_Cod_Aux As Integer,
                            ByVal Cerca_UdmDes As String,
                            ByVal Cerca_UdmSim As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.UnitaMisura_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.Append(" SELECT udm_cod, udm_sim, udm_des, tipocontrollo_cod ")
                    StrSQL.Append(" FROM    UnitaMisura ")
                    StrSQL.Append(" WHERE   1 = 1 ")

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod))
                    End If

                    If Udm_Cod_Aux <> 0 Then
                        StrSQL.Append(" AND Udm_Cod_Aux = " & Agro_SQL_SaveNum(Udm_Cod_Aux))
                    End If

                    If Cerca_UdmDes <> "" Then
                        StrSQL.Append(" AND UDM_DES LIKE '%" & Agro_SQL_SaveText(Cerca_UdmDes) & "%' ")
                    End If

                    If Cerca_UdmSim <> "" Then
                        StrSQL.Append(" AND UDM_SIM LIKE '%" & Agro_SQL_SaveText(Cerca_UdmSim) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY UDM_DES ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.AppendLine(" SELECT UnitaMisura.*, ")
                    StrSQL.AppendLine(" ISNULL(Tipo_Controlli.TipoControllo_Des, '') as TipoControllo_Des ")
                    StrSQL.AppendLine(" FROM      UnitaMisura ")
                    StrSQL.AppendLine(" LEFT JOIN (SELECT TipoControllo_Cod, TipoControllo_Des ")
                    StrSQL.AppendLine("            FROM Tipo_Controlli) as Tipo_Controlli ")
                    StrSQL.AppendLine("        ON UnitaMisura.TipoControllo_Cod = Tipo_Controlli.TipoControllo_Cod ")
                    StrSQL.AppendLine(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.AppendLine(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod))
                    End If

                    If Udm_Cod_Aux <> 0 Then
                        StrSQL.Append(" AND Udm_Cod_Aux = " & Agro_SQL_SaveNum(Udm_Cod_Aux))
                    End If

                    If Cerca_UdmDes <> "" Then
                        StrSQL.Append(" AND UDM_DES LIKE '%" & Agro_SQL_SaveText(Cerca_UdmDes) & "%' ")
                    End If

                    If Cerca_UdmSim <> "" Then
                        StrSQL.Append(" AND UDM_SIM LIKE '%" & Agro_SQL_SaveText(Cerca_UdmSim) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY UDM_DES ")
                    End If


            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDaElencoCod(ByRef objParametri As AgronicaCoreParametri, ByVal listaUdmCod As List(Of Integer),
                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                     ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.UnitaMisura_R.LeggiDaElencoSim()"

        Dim dt As New DataTable
        Dim strSql As New Text.StringBuilder

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.Append(" SELECT udm_cod, udm_sim, udm_des ")
                    strSql.Append(" FROM    UnitaMisura ")
                    strSql.Append(" WHERE   1 = 1 ")

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.Append(" SELECT * ")
                    strSql.Append(" FROM    UnitaMisura ")
                    strSql.Append(" WHERE   1 = 1 ")

            End Select

            If listaUdmCod IsNot Nothing AndAlso listaUdmCod.Count > 0 Then
                strSql.Append(" AND UDM_COD IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", listaUdmCod)) & ") ")
            End If

            strSql.Append(" AND   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            strSql.Append(" AND   Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND     Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND     Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY UDM_DES ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function UdmDes_from_UdmCod(ByVal Udm_Cod As Integer,
                                        ByRef Udm_Sim As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As String

        'Creo gli oggetti COM+
        Dim DT As DataTable

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

        'Recupero le informazioni		
        DT = objCOM.Leggi(Udm_Cod, 0, "", "",
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri)

        'Elimino gli oggetti COM
        objCOM = Nothing

        'Se il recordset non è chiuso allora ...	
        If DT.Rows.Count > 0 Then
            Udm_Sim = DT.Rows(0).Item("Udm_Sim")
            Return DT.Rows(0).Item("Udm_Des")
        Else
            Udm_Sim = ""
            Return "Non Definita"
        End If
    End Function

    Public Shared Sub ConvertiToKG_L(ByVal UDM As Integer, ByRef Udm_Cod_Trasformato As Integer, ByRef Moltiplicatore As Decimal)
        Select Case UDM
            Case enum_UnitaMisura.Grammi
                Moltiplicatore = 0.001
                Udm_Cod_Trasformato = 2
            Case enum_UnitaMisura.Milligrammi
                Moltiplicatore = 0.000001
                Udm_Cod_Trasformato = 2
            Case enum_UnitaMisura.Quintali
                Moltiplicatore = 100
                Udm_Cod_Trasformato = 2
            Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Metri_Cubi
                Moltiplicatore = 1000
                Udm_Cod_Trasformato = 2
            Case enum_UnitaMisura.Millilitri, enum_UnitaMisura.CentimetriCubi
                Moltiplicatore = 0.001
                Udm_Cod_Trasformato = 29
            Case enum_UnitaMisura.KG
                Moltiplicatore = 1
                Udm_Cod_Trasformato = 2
            Case enum_UnitaMisura.Litri
                Moltiplicatore = 1
                Udm_Cod_Trasformato = 29
            Case enum_UnitaMisura.Numero_Diffusori_HA, enum_UnitaMisura.Numero_Diffusori
                Moltiplicatore = 1
                Udm_Cod_Trasformato = enum_UnitaMisura.Numero_Diffusori
            Case enum_UnitaMisura.UNITA__HA, enum_UnitaMisura.UNITA
                Moltiplicatore = 1
                Udm_Cod_Trasformato = enum_UnitaMisura.UNITA

        End Select

    End Sub


    '##############################################################################################
    Public Function Converti_Kg_L_from_UdmCod(ByVal Udm_Cod As Integer,
                                              ByRef Udm_Sim As String,
                                              ByRef Udm_Des As String) As Integer

        Dim Udm_Cod_Trasformato As Integer = 0

        Select Case Udm_Cod

            'kg
            Case 2, 3, 20, 23, 88, 169, 171, 174, 175, 300, 301, 2003, 2006, 2007, 2010, 2011, 2013, 2014, 2018, 2026, 2032, 2033, 5001000

                Udm_Cod_Trasformato = 2
                Udm_Sim = "kg"
                Udm_Des = "chilogrammi"

                'l
            Case 29, 21, 22, 101, 104, 163, 164, 165, 170, 172, 173, 302, 303, 2004, 2005, 2009, 2012, 2015, 2016, 2017, 2020, 2021, 2022, 2028, 2030, 2031, 2043, 5001003

                Udm_Cod_Trasformato = 29
                Udm_Sim = "l"
                Udm_Des = "litri"

            Case enum_UnitaMisura.Numero_Diffusori, enum_UnitaMisura.Numero_Diffusori_HA

                Udm_Cod_Trasformato = enum_UnitaMisura.Numero_Diffusori
                Udm_Sim = "n. diffusori"
                Udm_Des = "n. diffusori"

            Case enum_UnitaMisura.Numero

                Udm_Cod_Trasformato = enum_UnitaMisura.Numero
                Udm_Sim = "n."
                Udm_Des = "n."

        End Select

        Return Udm_Cod_Trasformato

    End Function

    '########################################################################################################
    Public Function MezzoDes_from_Mezzo(ByVal Mezzo As Integer) As String

        Select Case Mezzo

            Case -1
                Return "Indefinito"
            Case 0
                Return "Ettolitri"
            Case 1
                Return "Ettari"
            Case 2
                Return "Ore"
            Case 3
                Return "Mensile"
            Case 4
                Return "Complessivo"
            Case Else
                Return "Indefinito"
        End Select


    End Function

    ''' <summary>
    ''' Conversione quantità e prezzi per cambio unità di misura (Udm)
    ''' </summary>
    ''' <param name="UdmIng">Udm Ingresso</param>
    ''' <param name="UdmUsc">Udm Uscita</param>
    ''' <param name="QtaIng">Qtà Ingresso</param>
    ''' <param name="QtaUsc">Qtà Uscita</param>
    ''' <param name="PrezzoIng">Prezzo Ingresso</param>
    ''' <param name="PrezzoUsc">Prezzo Uscita</param>
    ''' <param name="QtaIngAgg">Elenco Qtà Ingresso</param>
    ''' <param name="QtaUscAgg">Elenco Qtà Uscita</param>
    ''' <param name="PrezzoIngAgg">Elenco Prezzi Ingresso</param>
    ''' <param name="PrezzoUscAgg">Elenco Prezzi Uscita</param>
    Public Shared Sub ConvertiQtaPrezzi(ByVal UdmIng As Integer,
                                        ByVal UdmUsc As Integer,
                                        Optional ByVal QtaIng As Decimal? = Nothing,
                                        Optional ByRef QtaUsc As Decimal? = Nothing,
                                        Optional ByVal PrezzoIng As Decimal? = Nothing,
                                        Optional ByRef PrezzoUsc As Decimal? = Nothing,
                                        Optional ByVal QtaIngAgg As Decimal() = Nothing,
                                        Optional ByRef QtaUscAgg As Decimal() = Nothing,
                                        Optional ByVal PrezzoIngAgg As Decimal() = Nothing,
                                        Optional ByRef PrezzoUscAgg As Decimal() = Nothing
                                        )

        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.UnitaMisura_R.ConvertiQtaPrezzi()"

        If IsNothing(QtaIng) AndAlso
           IsNothing(PrezzoIng) AndAlso
           IsNothing(QtaIngAgg) AndAlso
           IsNothing(PrezzoIngAgg) Then

            Dim messaggioErrore = "Nessuna quantità/prezzo da convertire: {0} -> {1}"
            Dim desUdmIng = [Enum].GetName(GetType(enum_UnitaMisura), UdmIng)
            Dim desUdmUsc = [Enum].GetName(GetType(enum_UnitaMisura), UdmUsc)
            messaggioErrore = String.Format(messaggioErrore, desUdmIng, desUdmUsc)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End If

        If IsNothing(QtaIng) Then
            QtaIng = 0
        End If

        If IsNothing(PrezzoIng) Then
            PrezzoIng = 0
        End If

        Dim CoeffConvQta = DeterminaCoeffConversioneQta(UdmIng, UdmUsc)

        Try

            'Conversione quantità

            ConvQtaPrezzo(enum_QtaPrz.Quantita, CoeffConvQta, QtaIng, QtaUsc)

            'Conversione prezzo

            ConvQtaPrezzo(enum_QtaPrz.Prezzo, CoeffConvQta, PrezzoIng, PrezzoUsc)

            'Conversione quantità aggiuntive

            ConvQtaPrezzoAgg(enum_QtaPrz.Quantita, CoeffConvQta, QtaIngAgg, QtaUscAgg)

            'Conversione prezzi aggiuntivi

            ConvQtaPrezzoAgg(enum_QtaPrz.Prezzo, CoeffConvQta, PrezzoIngAgg, PrezzoUscAgg)

        Catch ex As Exception

            Dim messaggioErrore = "Errore inaspettato in conversione unità di misura: {0} -> {1}"
            Dim desUdmIng = [Enum].GetName(GetType(enum_UnitaMisura), UdmIng)
            Dim desUdmUsc = [Enum].GetName(GetType(enum_UnitaMisura), UdmUsc)
            messaggioErrore = String.Format(messaggioErrore, desUdmIng, desUdmUsc)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

    End Sub

    Private Shared Sub ConvQtaPrezzo(ByVal SeQtaPrz As Integer,
                                     ByVal CoeffConvQta As Decimal,
                                     ByVal QtaPrzIng As Decimal,
                                     ByRef QtaPrzUsc As Decimal?)

        QtaPrzUsc = 0

        If QtaPrzIng <> 0 Then

            Select Case SeQtaPrz

                Case enum_QtaPrz.Quantita
                    QtaPrzUsc = ArrotondaVal_6(QtaPrzIng * CoeffConvQta)

                Case enum_QtaPrz.Prezzo
                    QtaPrzUsc = ArrotondaVal_6(QtaPrzIng / CoeffConvQta)

            End Select

        End If

    End Sub

    Private Shared Sub ConvQtaPrezzoAgg(ByVal SeQtaPrz As Integer,
                                        ByVal CoeffConvQta As Decimal,
                                        ByVal QtaPrzIngAgg As Decimal(),
                                        ByRef QtaPrzUscAgg As Decimal())

        If Not IsNothing(QtaPrzIngAgg) AndAlso QtaPrzIngAgg.Length > 0 Then

            ReDim QtaPrzUscAgg(QtaPrzIngAgg.Length - 1)

            For i As Integer = 0 To QtaPrzIngAgg.Length - 1

                ConvQtaPrezzo(SeQtaPrz, CoeffConvQta, QtaPrzIngAgg(i), QtaPrzUscAgg(i))

            Next

        End If

    End Sub

    Private Enum enum_QtaPrz As Integer
        Quantita = 1
        Prezzo = 2
    End Enum




    ''' <summary>
    ''' Determina il coefficiente di conversione fra due unità di misura (Udm)
    ''' </summary>
    ''' <param name="UdmIng">Udm Ingresso</param>
    ''' <param name="UdmUsc">Udm Uscita</param>
    ''' <returns>Coefficiente conversione (moltiplicatore)</returns>
    Public Shared Function DeterminaCoeffConversioneQta(UdmIng As Integer,
                                                        UdmUsc As Integer) As Decimal

        'Determina il coefficente di conversione quantità da Udm Ingresso a Udm Uscita

        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.UnitaMisura_R.DeterminaCoeffConversioneQta()"

        'Coefficienti conversione Udm in KG
        Dim elencoCoeffConvPeso As New Dictionary(Of enum_UnitaMisura, Decimal)
        AggiungiUdmPeso(elencoCoeffConvPeso)

        'Coefficienti conversione Udm in Litri
        Dim elencoCoeffConvVolume As New Dictionary(Of enum_UnitaMisura, Decimal)
        AggiungiUdmVolume(elencoCoeffConvVolume)

        'Coefficienti conversione Udm in Metri
        Dim elencoCoeffConvLunghezza As New Dictionary(Of enum_UnitaMisura, Decimal)
        AggiungiUdmLunghezza(elencoCoeffConvLunghezza)

        Dim coeffConversioneQta As Decimal

        Select Case True

            Case elencoCoeffConvPeso.ContainsKey(UdmIng) AndAlso
                 elencoCoeffConvPeso.ContainsKey(UdmUsc)

                coeffConversioneQta = CalcolaCoeffConvQta(UdmIng, UdmUsc, elencoCoeffConvPeso, nomeRoutine)

            Case elencoCoeffConvVolume.ContainsKey(UdmIng) AndAlso
                 elencoCoeffConvVolume.ContainsKey(UdmUsc)

                coeffConversioneQta = CalcolaCoeffConvQta(UdmIng, UdmUsc, elencoCoeffConvVolume, nomeRoutine)

            Case elencoCoeffConvLunghezza.ContainsKey(UdmIng) AndAlso
                 elencoCoeffConvLunghezza.ContainsKey(UdmUsc)

                coeffConversioneQta = CalcolaCoeffConvQta(UdmIng, UdmUsc, elencoCoeffConvLunghezza, nomeRoutine)

            Case Else

                Dim messaggioErrore = "Conversione fra unità di misura non gestita: {0} -> {1}"
                Dim desUdmIng = [Enum].GetName(GetType(enum_UnitaMisura), UdmIng)
                Dim desUdmUsc = [Enum].GetName(GetType(enum_UnitaMisura), UdmUsc)
                messaggioErrore = String.Format(messaggioErrore, desUdmIng, desUdmUsc)
                Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Select

        Return coeffConversioneQta

    End Function

    Private Shared Sub AggiungiUdmPeso(ByRef elencoCoeffConv As Dictionary(Of enum_UnitaMisura, Decimal))

        elencoCoeffConv.Add(enum_UnitaMisura.Milligrammi, 0.000001)
        elencoCoeffConv.Add(enum_UnitaMisura.Grammi, 0.001)
        elencoCoeffConv.Add(enum_UnitaMisura.KG, 1)
        elencoCoeffConv.Add(enum_UnitaMisura.Quintali, 100)
        elencoCoeffConv.Add(enum_UnitaMisura.Tonnellate, 1000)

    End Sub

    Private Shared Sub AggiungiUdmVolume(ByRef elencoCoeffConv As Dictionary(Of enum_UnitaMisura, Decimal))

        elencoCoeffConv.Add(enum_UnitaMisura.Millilitri, 0.001)
        elencoCoeffConv.Add(enum_UnitaMisura.CentimetriCubi, 0.001)
        elencoCoeffConv.Add(enum_UnitaMisura.Litri, 1)
        elencoCoeffConv.Add(enum_UnitaMisura.Ettolitro, 100)
        elencoCoeffConv.Add(enum_UnitaMisura.Metri_Cubi, 1000)

    End Sub

    Private Shared Sub AggiungiUdmLunghezza(ByRef elencoCoeffConv As Dictionary(Of enum_UnitaMisura, Decimal))

        elencoCoeffConv.Add(enum_UnitaMisura.Millimetri, 0.001)
        elencoCoeffConv.Add(enum_UnitaMisura.Metri, 1)

    End Sub

    Private Shared Function CalcolaCoeffConvQta(UdmIng As Integer,
                                                UdmUsc As Integer,
                                                elencoCoeffConv As Dictionary(Of enum_UnitaMisura, Decimal),
                                                nomeRoutine As String
                                                ) As Decimal

        Dim coeffConvQta As Decimal
        Dim CoeffConvUdmIng As Decimal
        Dim CoeffConvUdmUsc As Decimal

        If Not elencoCoeffConv.TryGetValue(UdmIng, CoeffConvUdmIng) Then
            eccezioneCoeffConvNonTrovato(nomeRoutine, UdmIng)

        End If
        If Not elencoCoeffConv.TryGetValue(UdmUsc, CoeffConvUdmUsc) Then
            eccezioneCoeffConvNonTrovato(nomeRoutine, UdmUsc)
        End If

        coeffConvQta = CoeffConvUdmIng / CoeffConvUdmUsc

        Return coeffConvQta

    End Function

    Private Shared Sub eccezioneCoeffConvNonTrovato(nomeRoutine As String, Udm As Integer)
        Dim messaggioErrore = "Coefficiente conversione non trovato per unità di misura: {0}"
        Dim desUdm = [Enum].GetName(GetType(enum_UnitaMisura), Udm)
        messaggioErrore = String.Format(messaggioErrore, desUdm)
        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
    End Sub

    Public Function LeggiUdmConversione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim elencoUdmConversione As New Dictionary(Of enum_UnitaMisura, Decimal)
        AggiungiUdmPeso(elencoUdmConversione)
        AggiungiUdmVolume(elencoUdmConversione)
        AggiungiUdmLunghezza(elencoUdmConversione)

        Dim StrFiltroAgg As New Text.StringBuilder
        StrFiltroAgg.Length = 0
        For Each Udm In elencoUdmConversione
            If StrFiltroAgg.Length = 0 Then
                StrFiltroAgg.Append("Udm_Cod in (")
            Else
                StrFiltroAgg.Append(",")
            End If
            StrFiltroAgg.Append(CInt(Udm.Key))
        Next
        StrFiltroAgg.Append(")")

        Dim dtUnitaMisura = Leggi(0,
                                  0,
                                  "",
                                  "",
                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  StrFiltroAgg.ToString,
                                  "",
                                  objParametri)

        Return dtUnitaMisura

    End Function

End Class