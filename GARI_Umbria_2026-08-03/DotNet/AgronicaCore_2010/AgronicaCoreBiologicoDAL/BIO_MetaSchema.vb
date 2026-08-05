Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class BIO_MetaSchema_R


    '###################################################################################
    Public Function MetodoProduzioneDes_from_MetodoProduzioneCod(ByVal MetodoProduzione_Cod As Integer, _
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_MetaSchema_R.MetodoProduzioneDes_from_MetodoProduzioneCod()"

        Dim MetodoProduzioneDes As String = ""
        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_MetodoProduzione_R

        Try

            MetodoProduzioneDes = objMSDAL.MetodoProduzioneDes_from_MetodoProduzioneCod(MetodoProduzione_Cod, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return MetodoProduzioneDes

    End Function


    '###################################################################################
    Public Function OrganismoDes_from_OrganismoCod(ByVal Organismo_Cod As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_MetaSchema_R.OrganismoDes_from_OrganismoCod()"

        Dim OrganismoDes As String = ""
        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R

        Try

            OrganismoDes = objMSDAL.OrganismoDes_from_OrganismoCod(Organismo_Cod, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return OrganismoDes

    End Function


    '###################################################################################
    Public Function OrganismoDes_from_OrganismoSigla(ByVal Organismo_Sigla As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_MetaSchema_R.OrganismoDes_from_OrganismoSigla()"

        Dim OrganismoDes As String = ""
        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R

        Try

            OrganismoDes = objMSDAL.OrganismoDes_from_OrganismoSigla(Organismo_Sigla, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return OrganismoDes

    End Function

    '###################################################################################
    Public Function OrganismoDes_from_Codice(ByVal Codice As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_MetaSchema_R.OrganismoDes_from_Codice()"

        Dim OrganismoDes As String = ""
        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R

        Try

            OrganismoDes = objMSDAL.OrganismoDes_from_Codice(Codice, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return OrganismoDes

    End Function

    '###################################################################################
    Public Function OrientProduttivoDes_from_OrientProduttivoCod(ByVal Orientamento_Cod As String, _
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_MetaSchema_R.OrientProduttivoDes_from_OrientProduttivoCod()"

        Dim Orientamento_Des As String = ""
        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R

        Try

            Orientamento_Des = objMSDAL.OrientProduttivoDes_from_OrientProduttivoCod(Orientamento_Cod, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return Orientamento_Des

    End Function


    '###################################################################################
    Public Function TipologiaColturaDes_from_TipologiaColturaCod(ByVal TipologiaColtura_Cod As Integer, _
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_MetaSchema_R.TipologiaColturaDes_from_TipologiaColturaCod()"

        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_TipologiaColtura_R
        Dim TipologiaColtura_Des As String = ""

        Try

            TipologiaColtura_Des = objMSDAL.TipologiaColturaDes_from_TipologiaColturaCod(TipologiaColtura_Cod, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return TipologiaColtura_Des

    End Function


    '###################################################################################
    Public Function TitoloPossessoDes_from_TitoloPossessoCod(ByVal TitoloPossesso_Cod As Integer, _
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_MetaSchema_R.TitoloPossessoDes_from_TitoloPossessoCod()"

        Dim objMSDAL As New AgronicaCoreMetaSchemaDAL.BIO_Dati_TitoloPossesso_R
        Dim TitoloPossesso_Des As String = ""

        Try

            TitoloPossesso_Des = objMSDAL.TitoloPossessoDes_from_TitoloPossessoCod(TitoloPossesso_Cod, objParametri)

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return TitoloPossesso_Des

    End Function


    '#######################################################################################################
    Public Function Carica_HT_MetProd(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Hashtable

        Dim i As Integer
        Dim DT_MetProd As DataTable
        Dim HT_MetProd As New Hashtable
        Dim objMetProd As New AgronicaCoreMetaSchemaDAL.BIO_Dati_MetodoProduzione_R

        DT_MetProd = objMetProd.Leggi(0, _
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                       "", "", _
                                      objParametri_Server)

        If Not IsNothing(DT_MetProd) AndAlso DT_MetProd.Rows.Count > 0 Then
            For i = 0 To DT_MetProd.Rows.Count - 1
                If Not HT_MetProd.Contains(DT_MetProd.Rows(i).Item("MetodoProduzione_Cod")) Then
                    HT_MetProd.Add(DT_MetProd.Rows(i).Item("MetodoProduzione_Cod"), DT_MetProd.Rows(i).Item("MetodoProduzione_Des"))
                End If
            Next
        End If

        Return HT_MetProd

    End Function

    '#######################################################################################################
    Public Function Carica_HT_TipColt(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Hashtable

        Dim i As Integer
        Dim DT_TipColt As DataTable
        Dim objTipColt As New AgronicaCoreMetaSchemaDAL.BIO_Dati_TipologiaColtura_R
        Dim HT_TipColt As New Hashtable

        DT_TipColt = objTipColt.Leggi(0, _
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                        "", "", _
                                        objParametri_Server)

        If Not IsNothing(DT_TipColt) AndAlso DT_TipColt.Rows.Count > 0 Then
            For i = 0 To DT_TipColt.Rows.Count - 1
                If Not HT_TipColt.Contains(DT_TipColt.Rows(i).Item("TipologiaColtura_Cod")) Then
                    HT_TipColt.Add(DT_TipColt.Rows(i).Item("TipologiaColtura_Cod"), DT_TipColt.Rows(i).Item("TipologiaColtura_Des"))
                End If
            Next
        End If

        Return HT_TipColt

    End Function

    '#######################################################################################################
    Public Function Carica_HT_OrProd(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Hashtable

        Dim i As Integer
        Dim DT_OrProd As DataTable
        Dim HT_OrProd As New Hashtable
        Dim objOrProd As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R

        DT_OrProd = objOrProd.Leggi(0, _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                            "", "", _
                            objParametri_Server)

        If Not IsNothing(DT_OrProd) AndAlso DT_OrProd.Rows.Count > 0 Then
            For i = 0 To DT_OrProd.Rows.Count - 1
                If Not HT_OrProd.Contains(DT_OrProd.Rows(i).Item("Orientamento_Cod")) Then
                    HT_OrProd.Add(DT_OrProd.Rows(i).Item("Orientamento_Cod"), DT_OrProd.Rows(i).Item("Orientamento_Des"))
                End If
            Next
        End If

        Return HT_OrProd

    End Function


    '###################################################################################
    '###################################################################################
    '###################################################################################
    '###################################################################################
    '###################################################################################
    '

    '################################################################################
    Private Sub TipologiaColturaBIO_from_GruCod_OLD(ByVal Gru_Cod As Integer, _
                                               ByRef TipologiaColtura_Cod As Integer, _
                                               ByRef TipologiaColtura_Des As String)

        Select Case Gru_Cod

            Case 1 'Arboree

                TipologiaColtura_Cod = 1
                TipologiaColtura_Des = "Arborea : pura"

            Case 2, 3 'Erbacee e Orticole (Precedentemento non codificate, ora codificate come le erbacee)

                TipologiaColtura_Cod = 3
                TipologiaColtura_Des = "Erbacea : pura"

            Case Else

                TipologiaColtura_Cod = 0
                TipologiaColtura_Des = ""

        End Select


    End Sub

    '#####################################################
    Private Function MetodoProduzioneDes_from_MetodoProduzioneCod_OLD(ByVal MetodoProduzione_Cod As Integer)

        Select Case MetodoProduzione_Cod

            Case 1
                Return "Convenzionale"
            Case 2
                Return "In Conversione"
            Case 3
                Return "Biologico"
            Case Else
                Return "N.D."

        End Select

    End Function


    '################################################################################
    Private Function OrganismoDes_from_OrganismoCod_OLD(ByVal OrganismoCod As String) As String

        Dim Des As String

        Select Case OrganismoCod

            Case "CDX"
                Des = "Codex Srl"

            Case "ASS"
                Des = "Associazione Suolo e Salute"

            Case "ICA"
                Des = "Istituto per la Certificazione Etica e Ambientale"

            Case "IMC"
                Des = "Istituto Mediterraneo di Certificazione"

            Case "BAC"
                Des = "BioAgriCert"

            Case "CPB"
                Des = "Consorzio per il controllo dei Prodotti Biologici"

            Case "QCI"
                Des = "Q C & I International Service"

            Case "ECO"
                Des = "Ecocert Italia"

            Case "BSI"
                Des = "BIOS"

            Case "ECS"
                Des = "Eco System International Certificazioni Srl"

            Case "BZO"
                Des = "BioZoo Srl"

            Case Else
                Des = "Non definito"

        End Select

        'Restituisco il risultato
        Return Des

    End Function


    '################################################################################
    Private Function OrientProduttivoDes_from_OrientProduttivoCod_OLD(ByVal strOrientProduttivoCod As String) As String

        Dim Des As String

        Select Case strOrientProduttivoCod

            Case "10"
                Des = "Cerealicolo"
            Case "20"
                Des = "Orticolo"
            Case "30"
                Des = "Frutticolo"
            Case "40"
                Des = "Viticolo"
            Case "50"
                Des = "Olivicolo"
            Case "60"
                Des = "Floricolo - Vivaistico"
            Case "70"
                Des = "Colture industriali"
            Case "80"
                Des = "Foraggero"
            Case "90"
                Des = "Zootecnico"
            Case "99"
                Des = "Altro"

        End Select

        'Restituisco il risultato
        Return Des

    End Function



End Class
