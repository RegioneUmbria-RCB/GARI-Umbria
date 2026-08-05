Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DescrizioneBeniServizi_R

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri, ByVal objParametriUtente As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
    End Sub

    Public Function Leggi(ByVal movimento As Movimenti_dettagli, ByRef flag_extra As Boolean, ByVal modulo_generazione As enum_Omni_Modulo_Generazione) As String

        Dim tipo As String = String.Empty
        Dim descrizione As String = String.Empty
        Dim Flag_ProdottoAgroAlimentare As Boolean = True

        Dim Udm_Cod_Extra As Integer = 0
        Dim descrizioneBreve = String.Empty
        Dim qtaExtra As Decimal = 0
        Dim nomeCategoria As String = String.Empty
        Dim codArt As String = String.Empty
        Dim nomeCalibro As String = String.Empty
        Dim pesoSet As Integer = 0
        Dim oTabella_Cod_Base As Integer = 0

        Select Case movimento.Elem_Cod

            Case CostantiPersonalizzate.SERVIZI

                Dim objServizi As New AgronicaCoreMetaSchemaDAL.Categorie_R
                Dim tipoServizio As String
                tipoServizio = objServizi.Descr_From_Padre_Cod("S000029", "", movimento.Pro_Cod, _objParametriServer)
                descrizione = tipoServizio & " - " & movimento.Mov_Det_Des

            Case ALTRI_BENI, CostantiPersonalizzate.MACCHINE

                tipo = CAU_MAGAZZINO 'anche se in realtà non movimenta il magazzino

                descrizione = movimento.Mov_Det_Des
                descrizione = Replace(descrizione, "§", " ")
                descrizione = Replace(descrizione, "?", "€")

            Case ZOO_CONSISTENZA 'Consistenze Animali

                Flag_ProdottoAgroAlimentare = True
                tipo = CAU_ANIMALE

                If movimento.Cod_Progetto <> 0 Then

                    Dim objContab As New AgronicaCoreContabDAL.Contabilita_R
                    descrizione = objContab.LeggiProdottoStampeContab(_objParametriServer,
                                                                        _objParametriUtente,
                                                                        nomeCategoria,
                                                                        Nothing,
                                                                        Nothing,
                                                                        movimento.PIVA,
                                                                        movimento.Elem_Cod,
                                                                        movimento.Mat_Cod,
                                                                        movimento.Cod_Progetto,
                                                                        0,
                                                                        "",
                                                                        0,
                                                                        False,
                                                                        , ,
                                                                        ,
                                                                        , ,
                                                                        codArt,
                                                                        descrizioneBreve,
                                                                        flag_extra,
                                                                        Udm_Cod_Extra,
                                                                        qtaExtra)

                Else
                    descrizione = "Consistenze Zootecniche: " & movimento.Mov_Det_Des
                End If

            Case Else

                Select Case movimento.Elem_Cod
                    Case SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, TRASFORMATI_VEGETALI,
                            SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, TRASFORMATI_ANIMALI
                        Flag_ProdottoAgroAlimentare = True
                End Select

                tipo = CAU_MAGAZZINO

                Dim objContab As New AgronicaCoreContabDAL.Contabilita_R
                descrizione = objContab.LeggiProdottoStampeContab(_objParametriServer,
                                                                    _objParametriUtente,
                                                                    nomeCategoria,
                                                                    nomeCalibro,
                                                                    pesoSet,
                                                                    movimento.PIVA,
                                                                    movimento.Elem_Cod,
                                                                    movimento.Pro_Cod,
                                                                    If(movimento.Mat_Cod_Alias <> 0, movimento.Mat_Cod_Alias, movimento.Mat_Cod),
                                                                    movimento.Cod_Progetto,
                                                                    movimento.Fase_Cod,
                                                                    movimento.Lotto,
                                                                    movimento.Cal_Cod,
                                                                    False,
                                                                     , ,
                                                                    False,
                                                                    , ,
                                                                    codArt,
                                                                    descrizioneBreve,
                                                                    flag_extra,
                                                                    oTabella_Cod_Base,
                                                                    Udm_Cod_Extra,
                                                                    qtaExtra,
                                                                    False,
                                                                    movimento.Mat_Cod)
        End Select


        If modulo_generazione = enum_Omni_Modulo_Generazione.FreshFood Then

            Dim objMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim veg_Cod As Integer = 0
            Dim cul_Cod As Integer = 0

            'prima devo recuperare il veg_cod
            '(uso la stessa funzione che usa dopo nel caso del raggruppamento)
            objMatPrime.VegCod_CulCod_from_MatCod(movimento.PIVA, movimento.Elem_Cod, movimento.Mat_Cod, veg_Cod, cul_Cod, _objParametriServer)
            '----------------------------------
            'devo recuperare i parametri qualitativi solo se per il prodotto non è stato selezionato l'alias
            If movimento.Mat_Cod_Alias = 0 Then
                Dim objMPCamp As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
                Dim Dettagli_Prodotto As String = ""
                'Recupera_DettagliEConfezionamento_Prodotto
                Dettagli_Prodotto = objMPCamp.Recupera_Dettagli_Prodotto_XStampa(
                                                            movimento.PIVA, veg_Cod, movimento.Cal_Cod,
                                                            " AND OModuli_Referenze_Config_Dettagli.ChkEtichetta = 1 ",
                                                            _objParametriServer)

                If Dettagli_Prodotto <> "" Then
                    descrizione &= Dettagli_Prodotto
                End If
            End If 'ChkAlias

        End If

        If descrizione Is Nothing Then
            descrizione = String.Empty
        End If

        Return descrizione

    End Function

End Class


