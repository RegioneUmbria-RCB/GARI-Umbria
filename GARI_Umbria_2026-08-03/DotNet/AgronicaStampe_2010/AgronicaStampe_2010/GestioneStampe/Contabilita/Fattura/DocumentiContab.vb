Imports AgronicaCoreContabHLP
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility.FileSystemHelper
Imports AgronicaCoreStampeDAL

Public Module DocumentiContab

    '#####################################################################################################
    Public Sub Leggi_Intestazione_Impresa(ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByVal Progressivo_GIAS As Integer,
                                          ByVal Flag_DOCO As Boolean,
                                          ByVal Flag_DAA As Boolean,
                                          ByVal Piva As String,
                                          ByRef Log_Errori As String,
                                          ByRef x_RagSoc_Impresa As String,
                                          ByRef x_CodContatto_Impresa As String,
                                          ByRef x_CodiceFiscale_Impresa As String,
                                          ByRef x_IndDes_Impresa As String,
                                          ByRef x_FrzDes_Impresa As String,
                                          ByRef x_Cap_Impresa As String,
                                          ByRef x_Comune_Impresa As String,
                                          ByRef x_Provincia_Impresa As String,
                                          ByRef x_RegImprese As String,
                                          ByRef x_Provincia_RegImprese As String,
                                          ByRef x_REA As String,
                                          ByRef x_ISO As String,
                                          ByRef x_AlboCoop As String,
                                          ByRef x_CapitaleSociale As String,
                                          ByRef x_Telefono As String,
                                          ByRef x_Fax As String,
                                          ByRef x_Cell As String,
                                          ByRef x_Email As String,
                                          ByRef x_SitoWeb As String,
                                          ByRef x_Fabbricato_Des() As String,
                                          ByRef x_IndDes_Fabbricato() As String,
                                          ByRef x_FrzDes_Fabbricato() As String,
                                          ByRef x_Cap_Fabbricato() As String,
                                          ByRef x_Comune_Fabbricato() As String,
                                          ByRef x_Provincia_Fabbricato() As String,
                                          ByRef x_IndDes_Ministero As String,
                                          ByRef x_FrzDes_Ministero As String,
                                          ByRef x_Cap_Ministero As String,
                                          ByRef x_Comune_Ministero As String,
                                          ByRef x_Provincia_Ministero As String,
                                          ByRef x_RagSoc_Autorita As String,
                                          ByRef x_IndDes_Autorita As String,
                                          ByRef x_FrzDes_Autorita As String,
                                          ByRef x_Cap_Autorita As String,
                                          ByRef x_Comune_Autorita As String,
                                          ByRef x_Provincia_Autorita As String,
                                          ByRef x_Codice_Accisa_Impresa As String,
                                          ByRef x_SocialNetwork As String,
                                          ByRef x_Stato_Impresa As String)


        Dim objDistinct As New DatatableUtility

        Dim DT As DataTable
        Dim Key As Object
        Dim i As Integer

        Dim dtCodici As DataTable
        Dim hsCodici As Hashtable
        Dim idCodImpresa As Integer
        Dim valCodImpresa As String
        Dim chiaveCodiciImpresa() As String = {"ID_COD_IMPRESA"}
        Dim valoreCodiceImpresa() As String = {"VAL_COD_IMPRESA"}

        Dim dtRubrica As DataTable
        Dim hsRubrica As Hashtable
        Dim numeroImpresa As String
        Dim descrizioneImpresa As String
        Dim chiaveRubricaImpresa() As String = {"COD_RUBRICA"}

        Dim dtFabbricati As DataTable
        Dim hsFabbricati As Hashtable
        Dim chiaveFabbricato() As String = {"SA_COD_FABBRICATO", "FABBRICATO_COD"}

        Dim objStampe As New AgronicaCoreStampeDAL.DocContab


        Select Case Progressivo_GIAS
            Case enum_CodiceGIAS_Clienti.Fruttagel
                'DT = NewCom_Intestazione_Impresa_ConFabbricati_Leggi(objServer, objSession, objPage, Piva)
                DT = objStampe.Intestazione_Documento(Piva, True, Flag_DOCO, Flag_DAA, "", "", objParametri_Server)
            Case Else
                'DT = NewCom_Intestazione_Impresa_NoFabbricati_Leggi(objServer, objSession, objPage, Flag_DOCO, Flag_DAA, Piva)
                DT = objStampe.Intestazione_Documento(Piva, False, Flag_DOCO, Flag_DAA, "", "", objParametri_Server)
        End Select


        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            'devo copiare i dt perché la funzione del distinct mi modifica il contenuto
            dtCodici = DT.Copy
            dtRubrica = DT.Copy

            x_RagSoc_Impresa = DT.Rows(0).Item("rag_soc")

            x_IndDes_Impresa = DT.Rows(0).Item("ind_des_impresa")
            x_FrzDes_Impresa = DT.Rows(0).Item("frz_des_impresa")
            x_Cap_Impresa = DT.Rows(0).Item("cap_impresa")
            x_Comune_Impresa = DT.Rows(0).Item("localita_impresa")
            x_Provincia_Impresa = DT.Rows(0).Item("comuni_prov_impresa")
            Sistema_Comune_Provincia(x_Comune_Impresa, x_Provincia_Impresa)

            x_Provincia_RegImprese = DT.Rows(0).Item("provincia_impresa")
            Sistema_Provincia_RegImprese(x_Provincia_RegImprese)
            x_Stato_Impresa = DT.Rows(0).Item("stato_impresa")

            x_CodContatto_Impresa = DT.Rows(0).Item("cod_contatto")
            x_CodiceFiscale_Impresa = DT.Rows(0).Item("codice_fiscale")

            If Flag_DOCO = True Then
                x_IndDes_Ministero = DT.Rows(0).Item("ind_des_ministero")
                x_FrzDes_Ministero = DT.Rows(0).Item("frz_des_ministero")
                x_Cap_Ministero = DT.Rows(0).Item("cap_ministero")
                x_Comune_Ministero = DT.Rows(0).Item("localita_ministero")
                x_Provincia_Ministero = DT.Rows(0).Item("comuni_prov_ministero")
            End If

            If Flag_DAA = True Then
                x_RagSoc_Autorita = DT.Rows(0).Item("RagSoc_Autorita")
                x_IndDes_Autorita = DT.Rows(0).Item("ind_des_Autorita")
                x_FrzDes_Autorita = DT.Rows(0).Item("frz_des_Autorita")
                x_Cap_Autorita = DT.Rows(0).Item("cap_Autorita")
                x_Comune_Autorita = DT.Rows(0).Item("localita_Autorita")
                x_Provincia_Autorita = DT.Rows(0).Item("comuni_prov_Autorita")

                x_Codice_Accisa_Impresa = DT.Rows(0).Item("Codice_Accisa_Speditore")
            End If


            'DISTINCT PER RICAVARE I CODICI IMPRESA

            Try

                hsCodici = objDistinct.SelectDistinctWithHash_Base2(dtCodici, chiaveCodiciImpresa, valoreCodiceImpresa)

                For Each Key In hsCodici.Keys

                    idCodImpresa = CInt(Key)
                    valCodImpresa = CStr(hsCodici.Item(Key))

                    Select Case idCodImpresa

                        Case enum_CodiciAnagrafe.CodiceREA
                            x_REA = valCodImpresa

                        Case enum_CodiciAnagrafe.CodiceISO
                            x_ISO = valCodImpresa

                        Case enum_CodiciAnagrafe.NumIscrAlboSocCoop
                            x_AlboCoop = valCodImpresa

                        Case enum_CodiciAnagrafe.NumRegImprese
                            x_RegImprese = valCodImpresa

                        Case enum_CodiciAnagrafe.CapitaleSociale
                            x_CapitaleSociale = valCodImpresa
                            x_CapitaleSociale = x_CapitaleSociale.Replace("?", "€")

                    End Select

                Next

            Catch ex As Exception
                Log_Errori &= "- Intestazione: errore durante la lettura dei codici dell'impresa. " & vbCrLf
            End Try


            'DISTINCT PER LA RUBRICA

            Try

                hsRubrica = objDistinct.SelectDistinctWithHash_Base2(dtRubrica, chiaveRubricaImpresa)

                If Not IsNothing(dtRubrica) AndAlso dtRubrica.Rows.Count <> 0 Then

                    For i = 0 To dtRubrica.Rows.Count - 1

                        numeroImpresa = dtRubrica.Rows(i).Item("numero")
                        descrizioneImpresa = dtRubrica.Rows(i).Item("descr")

                        If InStr(1, descrizioneImpresa.ToLower, "tel", CompareMethod.Text) > 0 Then
                            x_Telefono = numeroImpresa
                        End If

                        If InStr(1, descrizioneImpresa.ToLower, "cel", CompareMethod.Text) > 0 Then
                            x_Cell = numeroImpresa
                        End If

                        If InStr(1, descrizioneImpresa.ToLower, "fax", CompareMethod.Text) > 0 Then
                            x_Fax = numeroImpresa
                        End If

                        If InStr(1, descrizioneImpresa.ToLower, "mail", CompareMethod.Text) > 0 Then
                            x_Email = numeroImpresa
                        End If

                        If InStr(1, descrizioneImpresa.ToLower, "sito", CompareMethod.Text) > 0 Then
                            x_SitoWeb = numeroImpresa
                        End If

                        If InStr(1, descrizioneImpresa.ToLower, "social", CompareMethod.Text) > 0 Then
                            x_SocialNetwork = numeroImpresa
                        End If

                    Next

                End If

            Catch ex As Exception
                Log_Errori &= "- Intestazione: errore durante la lettura della rubrica dell'impresa. " & vbCrLf
            End Try


            Select Case Progressivo_GIAS

                'elencare i clienti che vogliono lo stabilimento
                Case enum_CodiceGIAS_Clienti.Fruttagel

                    dtFabbricati = DT.Copy

                    'DISTINCT PER I FABBRICATI

                    Try

                        hsFabbricati = objDistinct.SelectDistinctWithHash_Base2(dtFabbricati, chiaveFabbricato, )

                        If Not IsNothing(dtFabbricati) AndAlso dtFabbricati.Rows.Count <> 0 Then

                            ReDim x_Fabbricato_Des(dtFabbricati.Rows.Count - 1)
                            ReDim x_IndDes_Fabbricato(dtFabbricati.Rows.Count - 1)
                            ReDim x_FrzDes_Fabbricato(dtFabbricati.Rows.Count - 1)
                            ReDim x_Cap_Fabbricato(dtFabbricati.Rows.Count - 1)
                            ReDim x_Comune_Fabbricato(dtFabbricati.Rows.Count - 1)
                            ReDim x_Provincia_Fabbricato(dtFabbricati.Rows.Count - 1)


                            For i = 0 To dtFabbricati.Rows.Count - 1

                                x_Fabbricato_Des(i) = dtFabbricati.Rows(i).Item("Fabbricato_Des")
                                x_IndDes_Fabbricato(i) = dtFabbricati.Rows(i).Item("ind_des_fabbricato")
                                x_FrzDes_Fabbricato(i) = dtFabbricati.Rows(i).Item("frz_des_fabbricato")
                                x_Cap_Fabbricato(i) = dtFabbricati.Rows(i).Item("cap_fabbricato")
                                x_Comune_Fabbricato(i) = dtFabbricati.Rows(i).Item("localita_fabbricato")
                                x_Provincia_Fabbricato(i) = dtFabbricati.Rows(i).Item("comuni_prov_fabbricato")

                            Next

                        End If

                    Catch ex As Exception
                        Log_Errori &= "- Intestazione: errore durante la lettura dei fabbricati dell'impresa. " & vbCrLf
                    End Try

            End Select


        Else
            Log_Errori &= "- Non sono stati trovati dati per l'intestazione. " & vbCrLf
        End If

    End Sub


    '#####################################################################################################
    Public Sub Leggi_Intestazione_Impresa_2(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByVal Progressivo_GIAS As Integer,
                                            ByVal Flag_DOCO As Boolean,
                                            ByVal Flag_DAA As Boolean,
                                            ByVal x_Id_Cf_Cliente As Integer,
                                            ByRef Log_Errori As String,
                                            ByRef x_Piva As String,
                                            ByRef x_CodiceFiscale_Impresa As String,
                                            ByRef x_RagSoc_Impresa As String,
                                            ByRef x_IndDes_Impresa As String,
                                            ByRef x_FrzDes_Impresa As String,
                                            ByRef x_Cap_Impresa As String,
                                            ByRef x_Comune_Impresa As String,
                                            ByRef x_Provincia_Impresa As String,
                                            ByRef x_Stato_Impresa As String,
                                            ByRef Intestazione_Riga3 As String,
                                            ByRef Intestazione_Riga4 As String,
                                            ByRef Intestazione_Riga5 As String,
                                            ByRef Intestazione_Riga6 As String,
                                            ByRef Intestazione_Riga7 As String,
                                            ByRef Intestazione_Riga8 As String,
                                            ByRef Intestazione_Riga9 As String,
                                            ByRef Intestazione_Riga10 As String,
                                            ByRef Intestazione_Riga11 As String,
                                            ByRef Intestazione_Riga12 As String,
                                            ByRef Intestazione_Riga13 As String,
                                            ByRef Intestazione_Riga14 As String,
                                            ByRef Intestazione_Riga15 As String,
                                            ByVal Flag_GestMaterialeVivaistico As Boolean,
                                            ByVal traduttore As Traduzione_Stampa_Base,
                                            ByRef Num_Global_Gap As String,
                                            ByVal Flag_CentroAziendalePartenza As Boolean)

        'ByVal CentroAziendale_Cod As Integer, _
        'ByVal CentroAziendale_Desc As String, _

        Dim objDistinct As New DatatableUtility

        Dim DT As DataTable
        Dim Key As Object
        'Dim i As Integer

        'Dim x_IndDes_Impresa As String
        'Dim x_FrzDes_Impresa As String
        'Dim x_Cap_Impresa As String
        'Dim x_Comune_Impresa As String
        'Dim x_Provincia_Impresa As String
        Dim x_RegImprese As String = ""
        Dim x_Provincia_RegImprese As String
        Dim x_REA As String = ""
        Dim x_ISO As String = ""
        Dim x_BNDOO As String = ""
        Dim x_CodOperatoreBio As String
        'Dim x_AlboCoop As String
        'Dim x_CapitaleSociale As String
        Dim x_Telefono_SedeLegale As String
        Dim x_Fax_SedeLegale As String
        Dim x_Cell_SedeLegale As String
        Dim x_Telefono_SedeOperativa As String
        Dim x_Fax_SedeOperativa As String
        Dim x_Cell_SedeOperativa As String
        Dim x_Email As String
        Dim x_PEC As String
        Dim x_SitoWeb As String
        'Dim x_Fabbricato_Des() As String
        'Dim x_IndDes_Fabbricato() As String
        'Dim x_FrzDes_Fabbricato() As String
        'Dim x_Cap_Fabbricato() As String
        'Dim x_Comune_Fabbricato() As String
        'Dim x_Provincia_Fabbricato() As String
        Dim x_IndDes_Ministero As String
        Dim x_FrzDes_Ministero As String
        Dim x_Cap_Ministero As String
        Dim x_Comune_Ministero As String
        Dim x_Provincia_Ministero As String
        Dim x_RagSoc_Autorita As String
        Dim x_IndDes_Autorita As String
        Dim x_FrzDes_Autorita As String
        Dim x_Cap_Autorita As String
        Dim x_Comune_Autorita As String
        Dim x_Provincia_Autorita As String
        Dim x_Codice_Accisa_Impresa As String
        Dim x_SocialNetwork As String

        Dim x_Indirizzo1_Sede_Operativa, x_Indirizzo2_Sede_Operativa, x_Jolly1, x_Jolly2 As String

        Dim DT_Codici As DataTable
        Dim HS_Codici As Hashtable
        Dim Id_Cod_Impresa As Integer
        Dim Val_Cod_Impresa As String
        Dim chiave_codici_impresa() As String = {"ID_COD_IMPRESA"}
        Dim valore_codice_impresa() As String = {"VAL_COD_IMPRESA"}

        'Dim DT_Rubrica As DataTable
        'Dim HS_Rubrica As Hashtable
        'Dim Numero_Impresa As String
        'Dim Descrizione_Impresa As String
        'Dim chiave_rubrica_impresa() As String = {"COD_RUBRICA"}

        'Dim DT_Fabbricati As DataTable
        'Dim HS_Fabbricati As Hashtable
        'Dim chiave_fabbricato() As String = {"SA_COD_FABBRICATO", "FABBRICATO_COD"}

        Dim objStampe As New AgronicaCoreStampeDAL.DocContab

        Dim flagFabbricati = If(Progressivo_GIAS = enum_CodiceGIAS_Clienti.Fruttagel, True, False)

        DT = objStampe.Intestazione_Documento_2(x_Piva, objParametri_Server.PivaSuperUser, flagFabbricati, Flag_DOCO, Flag_DAA, "", "", objParametri_Server)

        If DT.Rows.Count = 0 Then

            Dim handleGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

            Dim piveAntenati = handleGerarchiaImprese.Ricava_Stringa_PivePadre_Ricorsivo(x_Piva, "", objParametri_Server)
            Dim arrPiveAntenati = piveAntenati.Split(",")

            Dim i As Integer = 0

            While DT.Rows.Count = 0 AndAlso i < arrPiveAntenati.Length

                Dim pivaPadre = arrPiveAntenati(i).Replace("'", "")

                DT = objStampe.Intestazione_Documento_2(x_Piva, pivaPadre, flagFabbricati, Flag_DOCO, Flag_DAA, "", "", objParametri_Server)

                i += 1
            End While

            If DT.Rows.Count = 0 Then
                DT = objStampe.Intestazione_Documento_2(x_Piva, "", flagFabbricati, Flag_DOCO, Flag_DAA, "", "", objParametri_Server)
            End If

        End If

        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            'devo copiare i dt perchè la funzione del distinct mi modifica il contenuto
            DT_Codici = DT.Copy
            'DT_Rubrica = DT.Copy

            x_RagSoc_Impresa = DT.Rows(0).Item("rag_soc")
            'x_CodContatto_Impresa = DT.Rows(0).Item("cod_contatto")
            x_CodiceFiscale_Impresa = DT.Rows(0).Item("codice_fiscale")

            'modifica del 20/09/2010
            'introduco la chiamata a questa funzione
            'per la gestione del taroccamento piva x multi-attività
            Ricava_Piva_Codicefiscale(PERSONA_GIURIDICA, x_Piva, x_CodiceFiscale_Impresa, 0, x_Piva, x_CodiceFiscale_Impresa, Nothing)


            x_IndDes_Impresa = DT.Rows(0).Item("ind_des_impresa")
            x_FrzDes_Impresa = DT.Rows(0).Item("frz_des_impresa")
            x_Cap_Impresa = DT.Rows(0).Item("cap_impresa")
            x_Comune_Impresa = DT.Rows(0).Item("localita_impresa")
            x_Provincia_Impresa = DT.Rows(0).Item("comuni_prov_impresa")
            Sistema_Comune_Provincia(x_Comune_Impresa, x_Provincia_Impresa)
            x_Stato_Impresa = DT.Rows(0).Item("stato_impresa")

            x_Provincia_RegImprese = DT.Rows(0).Item("provincia_impresa")
            Sistema_Provincia_RegImprese(x_Provincia_RegImprese)

            x_CodOperatoreBio = DT.Rows(0).Item("CodiceOperatoreBio")

            x_Telefono_SedeLegale = DT.Rows(0).Item("Tel_Sede_Legale")
            x_Fax_SedeLegale = DT.Rows(0).Item("Fax_Sede_Legale")
            x_Cell_SedeLegale = DT.Rows(0).Item("Cell_Sede_Legale")
            x_Telefono_SedeOperativa = DT.Rows(0).Item("Tel_Sede_Operativa")
            x_Fax_SedeOperativa = DT.Rows(0).Item("Fax_Sede_Operativa")
            x_Cell_SedeOperativa = DT.Rows(0).Item("Cell_Sede_Operativa")
            x_Indirizzo1_Sede_Operativa = DT.Rows(0).Item("IndirizzoRiga1_Sede_Operativa")
            x_Indirizzo2_Sede_Operativa = DT.Rows(0).Item("IndirizzoRiga2_Sede_Operativa")
            x_Email = DT.Rows(0).Item("Mail")
            x_PEC = DT.Rows(0).Item("PEC")
            x_SitoWeb = DT.Rows(0).Item("SitoWeb")
            x_SocialNetwork = DT.Rows(0).Item("Social")
            x_Jolly1 = DT.Rows(0).Item("Jolly1")
            x_Jolly2 = DT.Rows(0).Item("Jolly2")

            If Flag_DOCO = True Then
                x_IndDes_Ministero = DT.Rows(0).Item("ind_des_ministero")
                x_FrzDes_Ministero = DT.Rows(0).Item("frz_des_ministero")
                x_Cap_Ministero = DT.Rows(0).Item("cap_ministero")
                x_Comune_Ministero = DT.Rows(0).Item("localita_ministero")
                x_Provincia_Ministero = DT.Rows(0).Item("comuni_prov_ministero")
            End If

            If Flag_DAA = True Then
                x_RagSoc_Autorita = DT.Rows(0).Item("RagSoc_Autorita")
                x_IndDes_Autorita = DT.Rows(0).Item("ind_des_Autorita")
                x_FrzDes_Autorita = DT.Rows(0).Item("frz_des_Autorita")
                x_Cap_Autorita = DT.Rows(0).Item("cap_Autorita")
                x_Comune_Autorita = DT.Rows(0).Item("localita_Autorita")
                x_Provincia_Autorita = DT.Rows(0).Item("comuni_prov_Autorita")

                x_Codice_Accisa_Impresa = DT.Rows(0).Item("Codice_Accisa_Speditore")
            End If


            'DISTINCT PER RICAVARE I CODICI IMPRESA

            Try

                HS_Codici = objDistinct.SelectDistinctWithHash_Base2(DT_Codici, chiave_codici_impresa, valore_codice_impresa)

                For Each Key In HS_Codici.Keys

                    Id_Cod_Impresa = CInt(Key)
                    Val_Cod_Impresa = CStr(HS_Codici.Item(Key))

                    Select Case Id_Cod_Impresa

                        Case enum_CodiciAnagrafe.CodiceREA
                            x_REA = Val_Cod_Impresa

                        Case enum_CodiciAnagrafe.BNDOO_BancaDatiOperatoriOrtofrutticoli
                            x_BNDOO = Val_Cod_Impresa

                        Case enum_CodiciAnagrafe.CodiceISO
                            x_ISO = Val_Cod_Impresa

                        Case enum_CodiciAnagrafe.NumRegImprese
                            x_RegImprese = Val_Cod_Impresa

                            'Case enum_CodiciAnagrafe.NumIscrAlboSocCoop
                            '    x_AlboCoop = Val_Cod_Impresa
                            'Case enum_CodiciAnagrafe.CapitaleSociale
                            '    x_CapitaleSociale = Val_Cod_Impresa
                            '    x_CapitaleSociale = x_CapitaleSociale.Replace("?", "€")

                        Case enum_CodiciAnagrafe.Codice_GlobalGap
                            Num_Global_Gap = Val_Cod_Impresa
                    End Select

                Next

            Catch ex As Exception
                Log_Errori &= "- Intestazione: errore durante la lettura dei codici dell'impresa. " & vbCrLf
            End Try


            ''DISTINCT PER LA RUBRICA

            'Try

            '    HS_Rubrica = objDistinct.SelectDistinctWithHash_Base2(DT_Rubrica, chiave_rubrica_impresa)

            '    If Not IsNothing(DT_Rubrica) AndAlso DT_Rubrica.Rows.Count <> 0 Then

            '        For i = 0 To DT_Rubrica.Rows.Count - 1

            '            Numero_Impresa = DT_Rubrica.Rows(i).Item("numero")
            '            Descrizione_Impresa = DT_Rubrica.Rows(i).Item("descr")

            '            If InStr(1, Descrizione_Impresa.ToLower, "tel", CompareMethod.Text) > 0 Then
            '                x_Telefono = Numero_Impresa
            '            End If

            '            If InStr(1, Descrizione_Impresa.ToLower, "cel", CompareMethod.Text) > 0 Then
            '                x_Cell = Numero_Impresa
            '            End If

            '            If InStr(1, Descrizione_Impresa.ToLower, "fax", CompareMethod.Text) > 0 Then
            '                x_Fax = Numero_Impresa
            '            End If

            '            If InStr(1, Descrizione_Impresa.ToLower, "mail", CompareMethod.Text) > 0 Then
            '                x_Email = Numero_Impresa
            '            End If

            '            If InStr(1, Descrizione_Impresa.ToLower, "sito", CompareMethod.Text) > 0 Then
            '                x_SitoWeb = Numero_Impresa
            '            End If

            '            If InStr(1, Descrizione_Impresa.ToLower, "social", CompareMethod.Text) > 0 Then
            '                x_SocialNetwork = Numero_Impresa
            '            End If

            '        Next

            '    End If

            'Catch ex As Exception
            '    Log_Errori &= "- Intestazione: errore durante la lettura della rubrica dell'impresa. " & vbCrLf
            'End Try


            'Select Case Progressivo_GIAS

            '    'elencare i clienti che vogliono lo stabilimento
            '    Case enum_CodiceGIAS_Clienti.Fruttagel

            '        DT_Fabbricati = DT.Copy

            '        'DISTINCT PER I FABBRICATI

            '        Try

            '            HS_Fabbricati = objDistinct.SelectDistinctWithHash_Base2(DT_Fabbricati, chiave_fabbricato, )

            '            If Not IsNothing(DT_Fabbricati) AndAlso DT_Fabbricati.Rows.Count <> 0 Then

            '                ReDim x_Fabbricato_Des(DT_Fabbricati.Rows.Count - 1)
            '                ReDim x_IndDes_Fabbricato(DT_Fabbricati.Rows.Count - 1)
            '                ReDim x_FrzDes_Fabbricato(DT_Fabbricati.Rows.Count - 1)
            '                ReDim x_Cap_Fabbricato(DT_Fabbricati.Rows.Count - 1)
            '                ReDim x_Comune_Fabbricato(DT_Fabbricati.Rows.Count - 1)
            '                ReDim x_Provincia_Fabbricato(DT_Fabbricati.Rows.Count - 1)


            '                For i = 0 To DT_Fabbricati.Rows.Count - 1

            '                    x_Fabbricato_Des(i) = DT_Fabbricati.Rows(i).Item("Fabbricato_Des")
            '                    x_IndDes_Fabbricato(i) = DT_Fabbricati.Rows(i).Item("ind_des_fabbricato")
            '                    x_FrzDes_Fabbricato(i) = DT_Fabbricati.Rows(i).Item("frz_des_fabbricato")
            '                    x_Cap_Fabbricato(i) = DT_Fabbricati.Rows(i).Item("cap_fabbricato")
            '                    x_Comune_Fabbricato(i) = DT_Fabbricati.Rows(i).Item("localita_fabbricato")
            '                    x_Provincia_Fabbricato(i) = DT_Fabbricati.Rows(i).Item("comuni_prov_fabbricato")

            '                Next

            '            End If

            '        Catch ex As Exception
            '            Log_Errori &= "- Intestazione: errore durante la lettura dei fabbricati dell'impresa. " & vbCrLf
            '        End Try


            '    Case Else


            'End Select

            Try


                If x_REA <> "" Then
                    Intestazione_Riga3 = ValoreDizionarioTraduzioneComuni(traduttore, "R.E.A.: ") & x_REA
                End If
                If x_BNDOO <> "" Then
                    Intestazione_Riga3 &= ValoreDizionarioTraduzioneComuni(traduttore, "   Codice BNDOO: ") & x_BNDOO
                End If
                If x_CodOperatoreBio <> "" Then
                    Intestazione_Riga3 &= ValoreDizionarioTraduzioneComuni(traduttore, "   Cod. operatore Bio: ") & x_CodOperatoreBio
                End If
                If x_ISO <> "" Then
                    Intestazione_Riga3 &= ValoreDizionarioTraduzioneComuni(traduttore, "   Codice ISO: ") & x_ISO
                End If

                Intestazione_Riga4 = ValoreDizionarioTraduzioneComuni(traduttore, "Iscr. al n. ") & x_RegImprese & ValoreDizionarioTraduzioneComuni(traduttore, " del Reg. Imprese di ") & x_Provincia_RegImprese

                Intestazione_Riga5 = ValoreDizionarioTraduzioneComuni(traduttore, "Sede legale: ") & x_IndDes_Impresa & " " & x_FrzDes_Impresa
                Intestazione_Riga6 = x_Cap_Impresa & " " & x_Comune_Impresa & " " & "(" & x_Provincia_Impresa & ")"
                If x_Id_Cf_Cliente = 2 Then
                    Intestazione_Riga6 &= " " & x_Stato_Impresa
                End If

                If x_Telefono_SedeLegale <> "" Then
                    Intestazione_Riga7 &= ValoreDizionarioTraduzioneComuni(traduttore, "Tel: ") & x_Telefono_SedeLegale
                End If
                If x_Fax_SedeLegale <> "" Then
                    Intestazione_Riga7 &= ValoreDizionarioTraduzioneComuni(traduttore, " Fax: ") & x_Fax_SedeLegale
                End If
                If x_Cell_SedeLegale <> "" Then
                    Intestazione_Riga7 &= ValoreDizionarioTraduzioneComuni(traduttore, " Cell: ") & x_Cell_SedeLegale
                End If
                'If Intestazione_Riga7 <> "" Then
                '    Intestazione_Riga7 = "Sede legale: " & Intestazione_Riga7
                'End If

                If x_Email <> "" Then
                    Intestazione_Riga8 = x_Email
                End If
                If x_SitoWeb <> "" Then
                    If Intestazione_Riga8 <> "" Then
                        Intestazione_Riga8 &= " "
                    End If
                    Intestazione_Riga8 &= x_SitoWeb
                End If

                If x_PEC <> "" Then
                    Intestazione_Riga9 = ValoreDizionarioTraduzioneComuni(traduttore, "PEC: ") & x_PEC
                End If

                '---------------------------------------------
                '06/04/2020: spostata dalla riga 13
                'e di conseguenza shiftato tutte le altre
                Intestazione_Riga10 = Trim(x_SocialNetwork)

                '06/04/2020: gestita intestazione modificata x zespri
                If Flag_GestMaterialeVivaistico = False Then

                    x_Telefono_SedeOperativa = DT.Rows(0).Item("Tel_Sede_Operativa")
                    x_Fax_SedeOperativa = DT.Rows(0).Item("Fax_Sede_Operativa")
                    x_Cell_SedeOperativa = DT.Rows(0).Item("Cell_Sede_Operativa")

                    If Trim(x_Indirizzo1_Sede_Operativa) <> "" Then
                        Intestazione_Riga11 = ValoreDizionarioTraduzioneComuni(traduttore, "Sede operativa: ") & x_Indirizzo1_Sede_Operativa
                        Intestazione_Riga12 = x_Indirizzo2_Sede_Operativa
                    End If

                    If x_Telefono_SedeOperativa <> "" Then
                        Intestazione_Riga13 &= ValoreDizionarioTraduzioneComuni(traduttore, "Tel: ") & x_Telefono_SedeOperativa
                    End If
                    If x_Fax_SedeOperativa <> "" Then
                        Intestazione_Riga13 &= ValoreDizionarioTraduzioneComuni(traduttore, " Fax: ") & x_Fax_SedeOperativa
                    End If
                    If x_Cell_SedeOperativa <> "" Then
                        Intestazione_Riga13 &= ValoreDizionarioTraduzioneComuni(traduttore, " Cell: ") & x_Cell_SedeOperativa
                    End If
                    'If Intestazione_Riga13 <> "" Then
                    '    Intestazione_Riga13 = "Sede operativa: " & Intestazione_Riga7
                    'End If

                    'Intestazione_Riga13 = Trim(x_SocialNetwork)
                    'fine shift dati
                    '---------------------------------------------

                    Intestazione_Riga14 = Trim(x_Jolly1)
                    Intestazione_Riga15 = Trim(x_Jolly2)

                Else
                    'ZESPRIBUD MODE

                    Intestazione_Riga11 = ""
                    'la riga dalla 12 alla 15 le valorizzo dopo la gestione dei dettagli
                    'perchè ricavo da lì il sa_cod da utilizzare

                End If

                If Flag_CentroAziendalePartenza = False Then

                    x_Telefono_SedeOperativa = DT.Rows(0).Item("Tel_Sede_Operativa")
                    x_Fax_SedeOperativa = DT.Rows(0).Item("Fax_Sede_Operativa")
                    x_Cell_SedeOperativa = DT.Rows(0).Item("Cell_Sede_Operativa")

                    If Trim(x_Indirizzo1_Sede_Operativa) <> "" Then
                        Intestazione_Riga11 = ValoreDizionarioTraduzioneComuni(traduttore, "Sede operativa: ") & x_Indirizzo1_Sede_Operativa
                        Intestazione_Riga12 = x_Indirizzo2_Sede_Operativa
                    End If

                    If x_Telefono_SedeOperativa <> "" Then
                        Intestazione_Riga13 &= ValoreDizionarioTraduzioneComuni(traduttore, "Tel: ") & x_Telefono_SedeOperativa
                    End If
                    If x_Fax_SedeOperativa <> "" Then
                        Intestazione_Riga13 &= ValoreDizionarioTraduzioneComuni(traduttore, " Fax: ") & x_Fax_SedeOperativa
                    End If
                    If x_Cell_SedeOperativa <> "" Then
                        Intestazione_Riga13 &= ValoreDizionarioTraduzioneComuni(traduttore, " Cell: ") & x_Cell_SedeOperativa
                    End If
                    'If Intestazione_Riga13 <> "" Then
                    '    Intestazione_Riga13 = "Sede operativa: " & Intestazione_Riga7
                    'End If

                    'Intestazione_Riga13 = Trim(x_SocialNetwork)
                    'fine shift dati
                    '---------------------------------------------

                    Intestazione_Riga14 = Trim(x_Jolly1)
                    Intestazione_Riga15 = Trim(x_Jolly2)

                Else
                    'ZESPRIBUD MODE

                    Intestazione_Riga11 = ""
                    'la riga dalla 12 alla 15 le valorizzo dopo la gestione dei dettagli
                    'perchè ricavo da lì il sa_cod da utilizzare

                End If

            Catch ex As Exception
                Log_Errori &= "- Intestazione: preparazione parametri. " & vbCrLf
            End Try

        Else
            Log_Errori &= "- Non sono stati trovati dati per l'intestazione. Verificare che l'azienda sia presente nei CONTATTI. " & vbCrLf
        End If


    End Sub

    Private Function ValoreDizionarioTraduzioneComuni(ByVal traduttore As Traduzione_Stampa_Base, ByVal chiave As String) As String
        If Not traduttore Is Nothing Then
            Return traduttore.ValoreDizionarioTraduzioneComuni(chiave)
        End If
        Return chiave
    End Function

    '#####################################################################################################
    Public Sub Leggi_CentroAziendale_Partenza(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef Log_Errori As String,
                                            ByVal Piva As String,
                                            ByVal Sa_cod As String,
                                            ByRef Rag_Soc As String,
                                            ByRef Sa_Nome As String,
                                            ByRef x_Indirizzo1_Sede_Operativa As String,
                                            ByRef x_Indirizzo2_Sede_Operativa As String,
                                            ByRef x_eMail As String)

        Rag_Soc = ""
        Sa_Nome = ""
        x_Indirizzo1_Sede_Operativa = ""
        x_Indirizzo2_Sede_Operativa = ""

        Try

            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            Dim dtCentro As DataTable
            dtCentro = objCentri.Leggi(Piva, Sa_cod, 0, 1,
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                        "", "", objParametri_Server)

            If Not IsNothing(dtCentro) AndAlso dtCentro.Rows.Count > 0 Then
                Rag_Soc = dtCentro.Rows(0).Item("Rag_Soc")
                Sa_Nome = dtCentro.Rows(0).Item("Sa_Nome")
                x_Indirizzo1_Sede_Operativa = dtCentro.Rows(0).Item("ind_des") & " " & dtCentro.Rows(0).Item("frz_des")
                x_Indirizzo2_Sede_Operativa = dtCentro.Rows(0).Item("cap") & " " & dtCentro.Rows(0).Item("com_des") & " " & " (" + dtCentro.Rows(0).Item("pro_cod") & ")"
            End If

            Dim objCentriRubrica As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
            Dim dtCentroRubrica As DataTable
            dtCentroRubrica = objCentriRubrica.Leggi(Piva, Sa_cod, 0,
                                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                     " Rubrica.descr like '%mail%' ", "", objParametri_Server)

            If Not IsNothing(dtCentroRubrica) AndAlso dtCentroRubrica.Rows.Count > 0 Then
                x_eMail = dtCentroRubrica.Rows(0).Item("numero")
            Else
                x_eMail = ""
            End If

        Catch ex As Exception
            Log_Errori = "Leggi_CentroAziendale_Partenza: " & ex.Message
        End Try

    End Sub

    Public Function Leggi_RubricaTel_CentroAziendale(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef Log_Errori As String,
                                            ByVal Piva As String,
                                            ByVal Sa_cod As String,
                                            Optional ByVal traduttore As Traduzione_Stampa_Base = Nothing) As String

        Dim RubricaTel As String = ""

        Try

            Dim handleRubricaCentri As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
            Dim dtRubricaCentro As DataTable = handleRubricaCentri.LeggiCentroSpecifico(Piva, Sa_cod, 0,
                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "",
                            objParametri_Server)

            Dim centroAzTel, centroAzFax, centroAzCell As String

            If dtRubricaCentro.Rows.Count > 0 Then

                Dim drTel = dtRubricaCentro.AsEnumerable().Where(Function(row) row.Field(Of String)("descr").ToLower().Contains("tel"))
                If drTel.Count > 0 Then
                    centroAzTel = drTel.First().Field(Of String)("numero")
                End If

                Dim drFax = dtRubricaCentro.AsEnumerable().Where(Function(row) row.Field(Of String)("descr").ToLower().Contains("fax"))
                If drFax.Count > 0 Then
                    centroAzFax = drFax.First().Field(Of String)("numero")
                End If

                Dim drCell = dtRubricaCentro.AsEnumerable().Where(Function(row) row.Field(Of String)("descr").ToLower().Contains("cell"))
                If drCell.Count > 0 Then
                    centroAzCell = drCell.First().Field(Of String)("numero")
                End If

            End If

            If Not String.IsNullOrEmpty(centroAzTel) Then
                RubricaTel &= ValoreDizionarioTraduzioneComuni(traduttore, "Tel: ") & centroAzTel
            End If
            If Not String.IsNullOrEmpty(centroAzFax) Then
                RubricaTel &= ValoreDizionarioTraduzioneComuni(traduttore, " Fax: ") & centroAzFax
            End If
            If Not String.IsNullOrEmpty(centroAzCell) Then
                RubricaTel &= ValoreDizionarioTraduzioneComuni(traduttore, " Cell: ") & centroAzCell
            End If

        Catch ex As Exception
            Log_Errori = "Leggi_RubricaTel_CentroAziendale: " & ex.Message
        End Try


        Return RubricaTel

    End Function



    '#####################################################################################################
    'Parametri per personalizzazioni
    'Qs_TipoView : N/""-Normale, P-Per prodotto serve per restituire una visualizzazione della fattura con dettagli raggruppati per prodotto (richiesta di Maiorano)

    Public Sub Leggi_Movimenti_DocContabile(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef Log_Errori As String,
                                            ByVal Piva As String,
                                            ByVal Id_Agenda As Integer,
                                            ByVal Cod_Report As enum_CodificaStampe,
                                            ByRef DT As DataTable,
                                            ByRef x_Id_Mov_Contabile As Integer,
                                            ByRef x_RagSoc_Impresa As String,
                                            ByRef x_CodiceFiscale_Impresa As String,
                                            ByRef x_Mov_Desc_Contabile As String,
                                            ByRef x_Data_Movimento As Date,
                                            ByRef x_Ora As String,
                                            ByRef x_Scadenza As Date,
                                            ByRef x_Scadenza_Extra As Date,
                                            ByRef x_Doc_Numero_Sin As String,
                                            ByRef x_Doc_Numero As Integer,
                                            ByRef x_Doc_Numero_Des As String,
                                            ByRef x_Progr_Protocollo As Integer,
                                            ByRef x_Progr_Registrazione As Integer,
                                            ByRef x_Data_Registrazione As Date,
                                            ByRef x_Num_Protocollo As Decimal,
                                            ByRef x_Colli As Integer,
                                            ByRef x_Peso As Decimal,
                                            ByRef x_Aspetto As String,
                                            ByRef x_Causale_Trasporto As String,
                                            ByRef x_Edit_Importo As enum_EditImporto,
                                            ByRef x_Cod_RisUm As Integer,
                                            ByRef x_Cod_Contatto As String,
                                            ByRef x_Rag_Soc As String,
                                            ByRef x_Codice_Fiscale As String,
                                            ByRef x_ChkFittizio As Integer,
                                            ByRef x_Cod_IndirizzoRisUm As Integer,
                                            ByRef x_Id_Cf_Cliente As Integer,
                                            ByRef x_Cod_Destinazione As Integer,
                                            ByRef x_CodContatto_Destinazione As String,
                                            ByRef x_RagSoc_Destinazione As String,
                                            ByRef x_CodiceFiscale_Destinazione As String,
                                            ByRef x_ChkFittizio_Destinazione As Integer,
                                            ByRef x_Cod_IndirizzoDestinazione As Integer,
                                            ByRef x_Id_Cf_Destinazione As Integer,
                                            ByRef x_Mezzo As Integer,
                                            ByRef x_Cod_Vettore As Integer,
                                            ByRef x_Cod_IndirizzoVettore As Integer,
                                            ByRef x_Id_Cf_Vettore As Integer,
                                            ByRef x_CodContatto_Vettore As String,
                                            ByRef x_RagSoc_Vettore As String,
                                            ByRef x_CodiceFiscale_Vettore As String,
                                             ByRef x_ChkFittizio_Vettore As Integer,
                                            ByRef x_NumReg_Vettore As String,
                                            ByRef x_TargaMezzo_Vettore As String,
                                            ByRef x_N_Autorizzazione_Trasporto As String,
                                            ByRef x_Agente_Cod As Integer,
                                            ByRef x_Natura_Beni As String,
                                            ByRef x_Modalita As Integer,
                                            ByRef x_Tara_Veicolo As Decimal,
                                            ByRef x_Tara_Imballi As Decimal,
                                            ByRef x_Tipo_Peso As Integer,
                                            ByRef x_Username_Note As String,
                                            ByRef x_Extra_Str As String,
                                            ByRef x_Extra_Int As Integer,
                                            ByRef x_Extra_Date As Date,
                                            ByRef x_ChkLayout_Bypass_Fatturato As Integer,
                                            ByRef x_ChkLayout_Join_Prodotti As Integer,
                                            ByRef x_ChkLayOut_Peso As Integer,
                                            ByRef x_ChkLayOut_Prezzo As Integer,
                                            ByRef x_Gestione_Vettore As String,
                                            ByRef Peso_Lordo As Decimal,
                                            ByRef x_ChkLayOut_Litri As Integer,
                                            ByRef x_Cod_RisUm_Aggiuntivo As Integer,
                                            ByRef x_Cod_Indirizzo_Aggiuntivo As Integer,
                                            ByRef x_Id_Cf_Aggiuntivo As Integer,
                                            ByRef x_CodContatto_Aggiuntivo As String,
                                            ByRef x_RagSoc_Aggiuntivo As String,
                                            ByRef x_CodiceFiscale_Aggiuntivo As String,
                                            ByRef x_ChkFittizio_Aggiuntivo As Integer,
                                            ByRef x_Flag_UveDiraspate As Boolean,
                                            ByRef x_ChkLayOut_Riscontrato As Integer,
                                            ByRef N_Doc_Cliente As String,
                                              ByRef Data_Doc_Cliente As Date,
                                              ByRef N_Nota_Fattura As String,
                                               ByRef Data_Nota_Fattura As Date,
                                               ByRef N_Nota_DDT As String,
                                              ByRef N_Nota_Riga_DDT As String,
                                              ByRef Data_Nota_DDT As Date, _
                                              ByRef EsigibilitaIva As Integer)


        Dim Dt_Contatti As DataTable
        Dim Dt_ContattiCodici As DataTable


        Try

            Dim objDoc As New AgronicaCoreStampeDAL.DocContab

            'L'impresa Gias corrente ha uno o più contatti ad essa associati, dalla query occorre cercare di ricavare
            'quello creato in automatico alla creazione dell'impresa e non altri che possono essere stati creati da altre imprese su gias,
            'di conseguenza prima eseguo la query utilizzando la pivasuperuser come creatrice del contatto,
            'se non vengono estratti dati allora ricorro alla gerarchia cercando per gli eventuali padri e nonni dell'impresa
            'infine non effettuo filtro sull'impresa creatrice nel caso questa sia slegata dall'impresa Gias corrente
            DT = objDoc.DocumentiContabili(Piva, objParametri_Server.PivaSuperUser, 0, Id_Agenda,
                                           "", "",
                                           Cod_Report,
                                           objParametri_Server)


            If DT.Rows.Count = 0 Then

                Dim handleGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

                Dim piveAntenati = handleGerarchiaImprese.Ricava_Stringa_PivePadre_Ricorsivo(Piva, "", objParametri_Server)
                Dim arrPiveAntenati = piveAntenati.Split(",")

                Dim i As Integer = 0

                While DT.Rows.Count = 0 AndAlso i < arrPiveAntenati.Length

                    Dim pivaPadre = arrPiveAntenati(i).Replace("'", "")

                    DT = objDoc.DocumentiContabili(Piva, pivaPadre, 0, Id_Agenda,
                                           "", "",
                                           Cod_Report,
                                           objParametri_Server)

                    i += 1
                End While

                If DT.Rows.Count = 0 Then
                    DT = objDoc.DocumentiContabili(Piva, "", 0, Id_Agenda,
                                           "", "",
                                           Cod_Report,
                                           objParametri_Server)
                End If

            End If


        Catch ex As Exception
            Log_Errori &= "- Errore del Core di lettura: " & ex.Message & vbCrLf & vbCrLf
        End Try



        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim objHLP As New AgronicaCoreContabHLP.Contabilita
            Dim objAnagrafeConDAL As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R

            Dim Sezionale_Cod As Integer

            With DT.Rows(0)

                x_Id_Mov_Contabile = .Item("Id_Mov_Contabile")

                x_RagSoc_Impresa = .Item("Rag_Soc_Impresa")
                x_CodiceFiscale_Impresa = .Item("Codice_Fiscale_Impresa")

                x_Mov_Desc_Contabile = .Item("mov_desc") 'descrizione

                x_Data_Movimento = .Item("data_movimento")
                x_Ora = .Item("ora")

                x_Scadenza = .Item("scadenza")
                x_Scadenza_Extra = .Item("scadenza_extra")

                x_Doc_Numero_Sin = .Item("doc_numero_sin")
                x_Doc_Numero = .Item("doc_numero")
                x_Doc_Numero_Des = .Item("doc_numero_des")

                x_Progr_Protocollo = .Item("progr_protocollo")
                x_Progr_Registrazione = .Item("progr_registrazione")
                x_Data_Registrazione = .Item("data_registrazione")

                x_Num_Protocollo = .Item("num_protocollo")

                x_Colli = .Item("colli")
                x_Aspetto = .Item("aspetto")
                x_Causale_Trasporto = .Item("causale_trasporto")

                x_Edit_Importo = .Item("tipo_sconto")

                x_Cod_RisUm = .Item("cod_risum")
                x_Cod_IndirizzoRisUm = .Item("cod_indirizzorisum")
                x_Rag_Soc = .Item("Rag_Soc_Cliente")
                If x_Rag_Soc = "" Then
                    x_Rag_Soc = .Item("Cognome_Cliente") & " " & .Item("Nome_Cliente")
                End If
                x_Cod_Contatto = .Item("Cod_Contatto_Cliente")
                x_Codice_Fiscale = .Item("Codice_Fiscale_Cliente")
                x_ChkFittizio = .Item("ChkFittizio")
                x_Id_Cf_Cliente = .Item("Id_Cf_Cliente")

                x_Cod_Destinazione = .Item("cod_destinazione")
                x_Cod_IndirizzoDestinazione = .Item("cod_indirizzodestinazione")

                If x_Cod_Destinazione <> 0 Then

                    Dt_Contatti = objContatti.Contatti_Contatto_Leggi("", "",
                                                x_Cod_Destinazione,
                                                0, False,
                                                False, 0, 0,
                                                False, 0,
                                                ID_CF_NOFILTRO,
                                                0, "", True,
                                                0, 0, 0, 0, 0,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "",
                                                objParametri_Server)

                    If Not IsNothing(Dt_Contatti) Then
                        If Dt_Contatti.Rows.Count <> 0 Then
                            If Dt_Contatti.Rows(0).Item("Rag_Soc") <> "" Then
                                x_RagSoc_Destinazione = Dt_Contatti.Rows(0).Item("Rag_Soc")
                            Else
                                x_RagSoc_Destinazione = Dt_Contatti.Rows(0).Item("Cognome") & " " & Dt_Contatti.Rows(0).Item("Nome")
                            End If
                            x_CodContatto_Destinazione = Dt_Contatti.Rows(0).Item("Cod_Contatto")
                            x_CodiceFiscale_Destinazione = Dt_Contatti.Rows(0).Item("Codice_Fiscale")
                            x_Id_Cf_Destinazione = Dt_Contatti.Rows(0).Item("Id_Cf")
                            x_ChkFittizio_Destinazione = Dt_Contatti.Rows(0).Item("ChkFittizio")
                        End If
                    End If

                End If

                x_Mezzo = .Item("mezzo")
                x_Cod_Vettore = .Item("cod_vettore")
                x_Cod_IndirizzoVettore = .Item("cod_indirizzovettore")

                If IsDBNull(.Item("TargaMezzoVettore")) Then
                    x_TargaMezzo_Vettore = ""
                Else
                    x_TargaMezzo_Vettore = .Item("TargaMezzoVettore")
                End If

                x_N_Autorizzazione_Trasporto = .Item("N_Autorizzazione_Trasporto")

                If IsDBNull(.Item("Agente_Cod")) Then
                    x_Agente_Cod = 0
                Else
                    x_Agente_Cod = .Item("Agente_Cod")
                End If


                If x_Cod_Vettore <> 0 Then

                    Dt_Contatti = objContatti.Contatti_Contatto_Leggi("", "",
                                     x_Cod_Vettore,
                                     0, False,
                                     False, 0, 0,
                                     False, 0,
                                     ID_CF_NOFILTRO,
                                     0, "", True,
                                     0, 0, 0, 0, 0,
                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     "", "",
                                     objParametri_Server)

                    If Not IsNothing(Dt_Contatti) Then
                        If Dt_Contatti.Rows.Count <> 0 Then
                            If Dt_Contatti.Rows(0).Item("Rag_Soc") <> "" Then
                                x_RagSoc_Vettore = Dt_Contatti.Rows(0).Item("Rag_Soc")
                            Else
                                x_RagSoc_Vettore = Dt_Contatti.Rows(0).Item("Cognome") & " " & Dt_Contatti.Rows(0).Item("Nome")
                            End If
                            x_CodContatto_Vettore = Dt_Contatti.Rows(0).Item("Cod_Contatto")
                            x_CodiceFiscale_Vettore = Dt_Contatti.Rows(0).Item("Codice_Fiscale")
                            x_Id_Cf_Vettore = Dt_Contatti.Rows(0).Item("Id_Cf")
                            x_ChkFittizio_Vettore = Dt_Contatti.Rows(0).Item("ChkFittizio")

                            Dt_ContattiCodici = objAnagrafeConDAL.Leggi(Piva, x_CodContatto_Vettore, 4017, "",
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "",
                                         objParametri_Server)

                            If Dt_ContattiCodici.Rows.Count <> 0 Then
                                If Not IsDBNull(Dt_ContattiCodici.Rows(0).Item("Val_cod")) Then
                                    If (Trim(Dt_ContattiCodici.Rows(0).Item("Val_cod")) <> "" And Len(Dt_ContattiCodici.Rows(0).Item("Val_cod")) > 1) Then
                                        x_NumReg_Vettore = Dt_ContattiCodici.Rows(0).Item("Val_cod")
                                    End If
                                End If
                            End If
                        End If
                    End If



                End If


                x_Natura_Beni = .Item("natura_beni")
                x_Modalita = .Item("modalita")

                x_Tara_Veicolo = .Item("tara_veicolo")
                x_Tara_Imballi = .Item("tara_imballi")
                x_Tipo_Peso = .Item("tipo_peso")
                x_Peso = .Item("peso")

                Peso_Lordo = Calcola_PesoLordo(x_Tipo_Peso, x_Peso, x_Tara_Imballi)

                x_Username_Note = .Item("username_note")

                x_Extra_Str = .Item("extra_str") 'note
                x_Extra_Int = .Item("extra_int")
                x_Extra_Date = .Item("extra_date") 'insolvenza

                x_ChkLayout_Bypass_Fatturato = .Item("chklayout_bypass_fatturato")
                x_ChkLayout_Join_Prodotti = .Item("chklayout_join_prodotti")
                x_ChkLayOut_Peso = .Item("ChkLayOut_Peso".ToLower)
                x_ChkLayOut_Prezzo = .Item("ChkLayOut_Prezzo".ToLower)
                x_ChkLayOut_Riscontrato = .Item("ChkLayOut_Riscontrato".ToLower)
                x_ChkLayOut_Litri = .Item("ChkLayOut_Litri".ToLower)

                If .Item("Flag_UveDiraspate") = 1 Then
                    x_Flag_UveDiraspate = True
                Else
                    x_Flag_UveDiraspate = False
                End If

                x_Cod_RisUm_Aggiuntivo = .Item("Cod_RisUm_Aggiuntivo")
                x_Cod_Indirizzo_Aggiuntivo = .Item("Cod_Indirizzo_Aggiuntivo")

                If x_Cod_RisUm_Aggiuntivo <> 0 Then

                    Dt_Contatti = objContatti.Contatti_Contatto_Leggi("", "",
                                                x_Cod_RisUm_Aggiuntivo,
                                                0, False,
                                                False, 0, 0,
                                                False, 0,
                                                ID_CF_NOFILTRO,
                                                0, "", True,
                                                0, 0, 0, 0, 0,
                                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "",
                                                objParametri_Server)

                    If Not IsNothing(Dt_Contatti) Then
                        If Dt_Contatti.Rows.Count <> 0 Then
                            If Dt_Contatti.Rows(0).Item("Rag_Soc") <> "" Then
                                x_RagSoc_Aggiuntivo = Dt_Contatti.Rows(0).Item("Rag_Soc")
                            Else
                                x_RagSoc_Aggiuntivo = Dt_Contatti.Rows(0).Item("Cognome") & " " & Dt_Contatti.Rows(0).Item("Nome")
                            End If
                            x_CodContatto_Aggiuntivo = Dt_Contatti.Rows(0).Item("Cod_Contatto")
                            x_CodiceFiscale_Aggiuntivo = Dt_Contatti.Rows(0).Item("Codice_Fiscale")
                            x_Id_Cf_Aggiuntivo = Dt_Contatti.Rows(0).Item("Id_Cf")
                            x_ChkFittizio_Aggiuntivo = Dt_Contatti.Rows(0).Item("ChkFittizio")
                        End If
                    End If

                End If

                x_Gestione_Vettore = objHLP.GestioneVettore_from_Id_Gestione_Vettore(.Item("Id_Gestione_Vettore"))

                N_Doc_Cliente = .Item("N_Doc_Cliente")
                Data_Doc_Cliente = .Item("Data_Doc_Cliente")
                N_Nota_Fattura = .Item("N_Nota_Fattura")
                Data_Nota_Fattura = .Item("Data_Nota_Fattura")
                N_Nota_DDT = .Item("N_Nota_DDT")
                N_Nota_Riga_DDT = .Item("N_Nota_Riga_DDT")
                Data_Nota_DDT = .Item("Data_Nota_DDT")

                Sezionale_Cod = .Item("Sezionale_Cod")

                Dim objSez As New AgronicaCoreContabDAL.Imprese_Sezionali_R
                EsigibilitaIva = objSez.EsigibilitaIva_from_SezionaleCod(Piva, Sezionale_Cod, objParametri_Server)

            End With

        Else
            Log_Errori &= "- Il Core di lettura non ha restituito dati." & vbCrLf & vbCrLf
        End If


    End Sub

    '#####################################################################################################
    Public Sub Leggi_Pagamenti(ByRef objParametri_Server As AgronicaCoreParametri,
                               ByVal Piva As String,
                               ByVal Lav_Cod As Integer,
                               ByVal Id_Agenda As Integer,
                               ByVal Id_Mov As Integer,
                               ByVal x_Num_Protocollo As Decimal,
                               ByRef x_RisorsaFinanziaria_Dare As String,
                               ByRef x_RisorsaFinanziaria_Avere As String,
                               ByRef Pagamento_Desc As String)

        'ByRef Importo_Pagato As Decimal, _

        Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_R
        Dim objPagamentiCausali As New AgronicaCoreContabDAL.Pagamenti_Causali_R
        Dim objHLP As New AgronicaCoreContabHLP.Contabilita
        Dim DT_Pag As DataTable
        Dim DT_PagCausali As DataTable
        Dim i As Integer

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        'MODIFICA DEL 29/10/2012:
        'viene letta la modalità di pagamento, ovvero i pagamenti previsti, che hanno Previsto_Avvenuto = 0
        'nuovo campo aggiunto con migra 287
        DT_Pag = objPagamenti.Leggi(Piva,
                                    0,
                                    Id_Agenda,
                                    Id_Mov,
                                    0,
                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    " Previsto_Avvenuto = 0 ", "",
                                    objParametri_Server)

        objParametri_Server.ResettaFinestra()

        If Not IsNothing(DT_Pag) AndAlso DT_Pag.Rows.Count > 0 Then

            DT_PagCausali = objPagamentiCausali.Leggi(Piva, 0, "", "", objParametri_Server)

            Dim x_Cod_Liquidita_Dare As Integer
            Dim x_Cod_Liquidita_Avere As Integer
            Dim x_Note_Pagamento As String
            Dim x_Percentuale_Pagamento As Decimal
            Dim x_Importo_Pagamento As Decimal
            Dim x_Importo_DaPagare As Decimal
            Dim x_Data_Pagamento As Date
            Dim x_Data_Scadenza As Date
            Dim x_Cau_Pagamento As Integer
            Dim x_Cau_Pagamento_Sigla As String
            ' Dim x_Cau_Pagamento_Des As String
            Dim objIban As New AgronicaCoreContabDAL.Liquidita_R
            Dim Numero_Tranche As Integer

            Numero_Tranche = DT_Pag.Rows.Count

            For i = 0 To Numero_Tranche - 1

                If i <> 0 Then
                    Pagamento_Desc &= "; "
                End If

                With DT_Pag.Rows(i)

                    x_Cod_Liquidita_Dare = .Item("cod_liquidita_dare")
                    If x_Cod_Liquidita_Dare > 0 Then
                        ' x_RisorsaFinanziaria_Dare = Leggi_RisorsaFinanziaria(objServer, objSession, objPage, Piva, x_Cod_Liquidita_Dare)
                        x_RisorsaFinanziaria_Dare = objIban.IBAN_from_CodLiquidita(Piva, x_Cod_Liquidita_Dare, objParametri_Server)
                    End If

                    x_Cod_Liquidita_Avere = .Item("cod_liquidita_avere")
                    If x_Cod_Liquidita_Avere > 0 Then
                        ' x_RisorsaFinanziaria_Avere = Leggi_RisorsaFinanziaria(objServer, objSession, objPage, Piva, x_Cod_Liquidita_Avere)
                        x_RisorsaFinanziaria_Avere = objIban.IBAN_from_CodLiquidita(Piva, x_Cod_Liquidita_Avere, objParametri_Server)
                    End If

                    x_Note_Pagamento = .Item("note")

                    x_Percentuale_Pagamento = .Item("percentuale")

                    x_Cau_Pagamento = .Item("cau_pagamento")

                    x_Cau_Pagamento_Sigla = objHLP.CauPagamentoSigla_from_CauPagamento(DT_PagCausali, x_Cau_Pagamento)

                    x_Importo_Pagamento = Format(.Item("importo"), "#,###,##0.00##")

                    x_Data_Pagamento = .Item("data_pagamento")

                    'If .Item("ChkDataScadenza_Manuale") = 1 Then
                    '    x_Data_Scadenza = .Item("DataScadenza_Manuale")
                    'Else
                    '    'scadenza calcolata sulla modalità di pagamento impostata, ma il giaslan al momento non la salva
                    '    'quindi non la stampo
                    '    x_Data_Scadenza = AGRODATAFINE
                    'End If
                    x_Data_Scadenza = .Item("DataScadenza_Manuale")

                    If x_Importo_Pagamento = 0 Then

                        Select Case x_Percentuale_Pagamento
                            Case 0
                                'eccezione
                                Pagamento_Desc &= x_Cau_Pagamento_Sigla
                            Case 100
                                'unica tranche per importo totale
                                x_Importo_DaPagare = x_Num_Protocollo
                                'Pagamento_Desc &= x_Cau_Pagamento_Sigla & " per il 100%"
                                'c'è un'unica tranche, non specifico la %
                                Pagamento_Desc &= x_Cau_Pagamento_Sigla
                            Case Else
                                'gestione con percentuali
                                x_Importo_DaPagare = ArrotondaVal_2((x_Percentuale_Pagamento * x_Num_Protocollo) / 100)
                                Pagamento_Desc &= x_Cau_Pagamento_Sigla & " per il " & CStr(x_Percentuale_Pagamento) & "%"
                        End Select

                    Else
                        If Numero_Tranche > 1 Then
                            'se ci sono più tranche scrivo anche l'importo
                            Pagamento_Desc &= x_Cau_Pagamento_Sigla & " per " & Format(x_Importo_Pagamento, "#,###,##0.00##") & " €"
                            ' Pagamento_Desc &= x_Cau_Pagamento_Sigla & " per " & CStr(x_Importo_Pagamento)     & " €"
                        Else
                            Pagamento_Desc &= x_Cau_Pagamento_Sigla
                        End If
                    End If

                    '25/02/2016: per le bolle che vengono stampate con layout fattura (check stampa dettagli economici)
                    'non deve stampare la data di scadenza perchè verrà calcolata dalla data della fattura (che ancora non è stata emessa)
                    If x_Data_Scadenza <> AGRODATAFINE And Lav_Cod <> LAVCOD_BOLLA_EMESSA Then
                        Pagamento_Desc &= " scad." & x_Data_Scadenza.ToShortDateString
                    End If

                    If x_Note_Pagamento <> "" Then
                        Pagamento_Desc &= " " & x_Note_Pagamento
                    End If

                End With

            Next

        End If


    End Sub


    '#####################################################################################################
    Public Sub Leggi_MovimentoDettaglio_DocContabile(ByRef objParametriServer As AgronicaCoreParametri,
                                                     ByRef objParametriUtenti As AgronicaCoreParametri,
                                                     ByVal moduliCliente As List(Of Integer),
                                                     ByRef logErrori As String,
                                                     ByVal dr As DataRow,
                                                     ByVal Piva As String,
                                                     ByVal Lav_Cod As Integer,
                                                     ByVal Flag_Raggruppa As Boolean,
                                                     ByVal ChkLayOut_Peso As Integer,
                                                     ByRef x_Mov_Det_Des As String,
                                                     ByRef x_Elem_Cod As Integer,
                                                     ByRef x_Pro_Cod As Integer,
                                                     ByRef x_Mat_Cod As Integer,
                                                     ByRef x_Cod_Progetto As Integer,
                                                     ByRef x_Fase_Cod As Integer,
                                                     ByRef x_Lotto As String,
                                                     ByRef x_Cal_Cod As Integer,
                                                     ByRef x_Udm_Cod As Integer,
                                                     ByRef x_Udm_Sim As String,
                                                     ByRef x_Udm_Des As String,
                                                     ByRef x_Udm_Cod_Extra As Integer,
                                                     ByRef x_Udm_Sim_Extra As String,
                                                     ByRef x_Udm_Des_Extra As String,
                                                     ByRef x_Qta As Decimal,
                                                     ByRef x_Qta_Extra As Decimal,
                                                     ByRef x_Qta_Extra_Totale As Decimal,
                                                     ByRef x_Descrizione As String,
                                                     ByRef x_Prezzo_Unitario As Decimal,
                                                     ByRef x_Prezzo_Unitario_Netto As Decimal,
                                                     ByRef x_Imponibile As Decimal,
                                                     ByRef x_Imponibile_Netto As Decimal,
                                                     ByRef x_Cod_IVA As Integer,
                                                     ByRef x_Aliquota_Des As String,
                                                     ByRef x_IVA As Decimal,
                                                     ByRef x_ChkIVA_Manuale As Integer,
                                                     ByRef x_Cod_IVAIndetraibile As Integer,
                                                     ByRef x_Sconto_Perc As Decimal,
                                                     ByRef x_Sconto_Perc_2 As Decimal,
                                                     ByRef x_Sconto_Testo As String,
                                                     ByRef x_Sconto As Decimal,
                                                     ByRef x_Sconto_Modalita As Integer,
                                                     ByRef x_Prezzo_Effettivo As Decimal,
                                                     ByRef x_Anno As Integer,
                                                     ByRef x_Ric_Cod As Integer,
                                                     ByRef x_Cod_Conto As Integer,
                                                     ByRef x_Conto As String,
                                                     ByRef x_Contabilizzato As Integer,
                                                     ByRef x_Pendente As Integer,
                                                     ByRef x_ChkLayOut_Hide As Integer,
                                                     ByRef x_Tara As Decimal,
                                                     ByRef x_Extra_Str_Dettagli As String,
                                                     ByRef x_Extra_Int_Dettagli As Integer,
                                                     ByRef x_Extra_Date_Dettagli As Date,
                                                     ByRef Veg_Cod As Integer,
                                                     ByRef Cul_Cod As Integer,
                                                     ByRef Nome_Calibro As String,
                                                     ByRef Riferimento_DocAllegato As String,
                                                     ByRef Dt_Dettagli_Round As DataTable,
                                                     ByRef Flag_ProdottoAgroAlimentare As Boolean,
                                                     ByVal Flag_StampaNumeroVasca As Boolean,
                                                     ByRef Flag_DDTallegati As Boolean,
                                                     ByRef Flag_ORDallegati As Boolean,
                                                     ByRef numcolli_contenitori As Integer,
                                                     ByRef contenitore_cod As Integer,
                                                     ByRef descr_contenitore As String,
                                                     ByRef numcontenitori_imballi As Integer,
                                                     ByRef imballaggio_cod As Integer,
                                                     ByRef descr_imballo As String,
                                                     ByRef ChkConfezione As Integer,
                                                     ByRef ChkContenitore As Integer,
                                                     ByRef ChkImballaggio As Integer,
                                                     ByRef x_ret_Cod_Articolo As String,
                                                     ByRef x_ret_Descr_breve As String,
                                                     ByRef x_Flag_Extra As Integer,
                                                     ByRef x_OTabella_Cod_Base As Integer,
                                                     ByRef x_Prezzo_Livello As Integer,
                                                     ByVal objConfigStampe As AgronicaCoreStampeDAL.ConfigurazioneStampe,
                                                     ByRef Dettaglio_Evasione As String,
                                                     ByRef x_N_Conf_Riscontrate As Integer,
                                                     ByRef x_N_Colli_Riscontrati As Integer,
                                                     ByRef x_N_Imballi_Riscontrati As Integer,
                                                     ByRef x_Peso_Netto_Riscontrato As Decimal,
                                                     ByRef x_Peso_Lordo_Riscontrato As Decimal,
                                                     ByRef x_Tara_Unit_Collo_Riscontrata As Decimal,
                                                     ByRef x_Tara_Unit_Imballo_Riscontrata As Decimal,
                                                     Optional ByVal Qs_TipoView As String = "",
                                                     Optional ByRef x_Aliquota_Val As Integer = 0,
                                                     Optional ByVal Flag_UveDiraspate As Boolean = False,
                                                     Optional ByVal flagTareRiscontrateInDesc As Boolean = True,
                                                     Optional ByRef Rif_Ordine As String = "",
                                                     Optional ByVal Flag_GradoAlcolico As Boolean = False,
                                                     Optional ByVal Flag_GestMaterialeVivaistico As Boolean = False)


        '====================================================================================
        '---------------- Dt dettagli per round finale e riepiloghi ----------------------------
        Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
        If IsNothing(Dt_Dettagli_Round) Then

            Try
                Dt_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

            Catch ex As Exception
                logErrori &= "- Dt dettagli per round finale e riepiloghi: " & vbCrLf & ex.Message & vbCrLf
            End Try

        End If

        objParametriServer.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim objContab As New AgronicaCoreContabDAL.Contabilita_R
        Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
        Dim objMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim tipo As Integer
        Dim nomeCategoria As String = ""
        Dim Peso_Set As Integer
        Dim Mat_Cod_Alias As Integer = 0
        Dim Id_Mov_Det As Integer
        'Dim MP_Flag_Extra As Integer = 0
        Dim MP_Udm_Cod_Extra As Integer = 0
        Dim MP_Qta_Extra As Decimal = 0
        Dim titoloAlcol As Decimal = 0

        Dim moduloCantine As Boolean = moduliCliente.Contains(enum_Omni_Modulo_Generazione.Cantine)
        Dim moduloFreshFood As Boolean = moduliCliente.Contains(enum_Omni_Modulo_Generazione.FreshFood)
        Dim moduloTabacco As Boolean = moduliCliente.Contains(enum_Omni_Modulo_Generazione.Tabacco)
        Dim moduloZoo As Boolean = moduliCliente.Contains(enum_Omni_Modulo_Generazione.Zoo)

        'azzero ad ogni giro
        numcolli_contenitori = 0
        contenitore_cod = 0
        descr_contenitore = ""
        numcontenitori_imballi = 0
        imballaggio_cod = 0
        descr_imballo = ""
        Flag_DDTallegati = False
        ChkConfezione = 0
        ChkContenitore = 0
        ChkImballaggio = 0
        x_Flag_Extra = 0
        x_OTabella_Cod_Base = 0
        x_Prezzo_Livello = 0
        Dettaglio_Evasione = ""
        x_Qta = 0
        x_Qta_Extra = 0
        x_Qta_Extra_Totale = 0
        x_Udm_Cod_Extra = 0
        x_Udm_Des_Extra = ""
        x_Tara = 0
        x_Prezzo_Unitario = 0
        x_Prezzo_Unitario_Netto = 0
        x_Imponibile = 0
        x_Imponibile_Netto = 0
        x_Sconto_Perc = 0
        x_Sconto_Perc_2 = 0
        x_Sconto_Testo = ""
        x_Sconto = 0
        x_Sconto_Modalita = 0
        x_Cod_IVA = 0
        x_Aliquota_Des = ""
        x_IVA = 0
        x_ChkIVA_Manuale = 0
        x_Cod_IVAIndetraibile = 0
        x_ChkLayOut_Hide = 0
        x_Prezzo_Effettivo = 0
        x_Anno = 0
        x_Ric_Cod = 0
        x_Cod_Conto = 0
        x_Contabilizzato = 0
        x_Pendente = 0
        Veg_Cod = 0
        Cul_Cod = 0
        ChkContenitore = 0
        ChkImballaggio = 0
        x_Udm_Des = ""
        x_Udm_Sim = ""
        x_Prezzo_Livello = 0
        Riferimento_DocAllegato = ""

        x_N_Conf_Riscontrate = -1
        x_N_Colli_Riscontrati = -1
        x_N_Imballi_Riscontrati = -1
        x_Peso_Netto_Riscontrato = 0
        x_Peso_Lordo_Riscontrato = 0
        x_Tara_Unit_Collo_Riscontrata = -1
        x_Tara_Unit_Imballo_Riscontrata = -1

        With dr

            Id_Mov_Det = .Item("id_mov_det")

            x_Mov_Det_Des = .Item("mov_det_des")

            'chiave del prodotto
            x_Elem_Cod = .Item("elem_cod")
            x_Pro_Cod = .Item("pro_cod")
            x_Mat_Cod = .Item("mat_cod")
            x_Cod_Progetto = .Item("cod_progetto")
            x_Fase_Cod = .Item("fase_cod")
            x_Lotto = .Item("lotto")
            x_Cal_Cod = .Item("cal_cod")
            x_Udm_Cod = .Item("udm_cod")

            If Flag_GestMaterialeVivaistico = True And x_Elem_Cod = TRASFORMATI_VEGETALI And moduloFreshFood = True Then
                'impostazione superuser 848 se è 1 devi mostrare numero invece di Kg
                ' stata fatta per utilizzare le piantine come trasformati vegetali
                x_Udm_Cod = enum_UnitaMisura.Numero
            End If

            If x_Udm_Cod = 0 Then
                x_Udm_Sim = ""
                x_Udm_Des = ""
            Else
                x_Udm_Des = objUdm.UdmDes_from_UdmCod(x_Udm_Cod, x_Udm_Sim, objParametriServer)
            End If

            Mat_Cod_Alias = .Item("mat_cod_alias")

            Dim mat_cod_xdesc As Integer
            If Mat_Cod_Alias <> 0 Then
                mat_cod_xdesc = Mat_Cod_Alias
            Else
                mat_cod_xdesc = x_Mat_Cod
            End If


            If Flag_GradoAlcolico = True Then
                titoloAlcol = CDec(.Item("Titolo_Alcol"))
            End If

            Select Case x_Elem_Cod

                Case SERVIZI

                    Dim objServizi As New AgronicaCoreMetaSchemaDAL.Categorie_R
                    Dim tipoServizio As String
                    tipoServizio = objServizi.Descr_From_Padre_Cod("S000029", "", x_Pro_Cod, objParametriServer)
                    x_Descrizione = tipoServizio & " - " & x_Mov_Det_Des
                    '-------------------------------------
                    x_ret_Descr_breve = x_Descrizione
                    x_ret_Cod_Articolo = ""

                Case ALTRI_BENI, MACCHINE

                    tipo = CAU_MAGAZZINO 'anche se in realtà non movimenta il magazzino

                    x_Descrizione = x_Mov_Det_Des
                    x_Descrizione = Replace(x_Descrizione, "§", "<br>")
                    x_Descrizione = Replace(x_Descrizione, "?", "€")
                    '-------------------------------------
                    x_ret_Descr_breve = x_Descrizione
                    x_ret_Cod_Articolo = ""

                Case RIGA_DESCRIZIONE_LIBERA

                    x_Descrizione = x_Mov_Det_Des
                    x_Descrizione = Replace(x_Descrizione, "§", "<br>")
                    x_Descrizione = Replace(x_Descrizione, "?", "€")

                    ' Giulia: 2/11/2017:fixbug x Trombin che usa descrizione breve e in caso di riga descrizione libera non vedeva nulla
                    x_ret_Descr_breve = x_Descrizione
                    x_ret_Cod_Articolo = ""
                    'è una riga di testo libero, non devo fare altro

                    '  Giulia, 30/05/2017 11:11:43: devo cmq leggere il riferimento al doc allegato, perché sennò
                    '   non viene stampato bene il riferimento in mezzo ai dettagli
                    Select Case Lav_Cod

                        Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                                LAVCOD_RICEVUTA_EMESSA,
                                LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                            Try

                                '---------------------------------------------------------------
                                '------- BOLLA /DOCO / DDT / ORDINE COLLEGATI ------------------
                                '---------------------------------------------------------------

                                'al momento è usato solo dalla funzione omonima del fresh&food

                                Rif_Ordine = ""
                                If .Item("N_Doc_Cliente") <> "" Then
                                    Rif_Ordine = "Rif. Ordine n." & .Item("N_Doc_Cliente") & " del " & CDate(.Item("Data_Doc_Cliente")).ToShortDateString
                                End If

                                '04/02/2019: aggiunto if
                                If Rif_Ordine <> "" Then
                                    'se l'utente ha specificato un numero ordine del suo cliente
                                    'questo prende la priorità
                                    Riferimento_DocAllegato = Rif_Ordine
                                Else

                                    Riferimento_DocAllegato = Leggi_RiferimentoDocAllegato(objParametriServer,
                                                                                           Piva,
                                                                                           .Item("id_agenda"),
                                                                                           .Item("id_mov_magazzino"),
                                                                                           .Item("id_mov_det"),
                                                                                           Flag_DDTallegati,
                                                                                           Flag_ORDallegati,
                                                                                           Dettaglio_Evasione)


                                End If

                            Catch ex As Exception
                                logErrori &= "- Lettura del riferimento documento allegato: " & vbCrLf & ex.Message & vbCrLf
                            End Try

                    End Select



                    Exit Sub
                    '-------------------------------------

                Case ZOO_CONSISTENZA 'Consistenze Animali

                    Flag_ProdottoAgroAlimentare = True

                    tipo = CAU_ANIMALE


                    If x_Cod_Progetto <> 0 Then

                        x_Descrizione = objContab.LeggiProdottoStampeContab(objParametriServer,
                                                                            objParametriUtenti,
                                                                            nomeCategoria,
                                                                            Nothing,
                                                                            Nothing,
                                                                            Piva,
                                                                            x_Elem_Cod,
                                                                            x_Mat_Cod,
                                                                            x_Cod_Progetto,
                                                                            0,
                                                                            "",
                                                                            0,
                                                                            False,
                                                                            , ,
                                                                            ,
                                                                            , ,
                                                                            x_ret_Cod_Articolo,
                                                                            x_ret_Descr_breve,
                                                                            x_Flag_Extra,
                                                                            MP_Udm_Cod_Extra,
                                                                            MP_Qta_Extra,
                                                                            0,
                                                                            0,
                                                                            False,
                                                                            0,
                                                                            0,
                                                                            moduliCliente)

                    Else
                        x_Descrizione = "Consistenze Zootecniche: " & x_Mov_Det_Des
                    End If

                    '-------------------------------------

                Case Else

                    Select Case x_Elem_Cod
                        Case SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, TRASFORMATI_VEGETALI,
                                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, TRASFORMATI_ANIMALI
                            Flag_ProdottoAgroAlimentare = True
                    End Select

                    tipo = CAU_MAGAZZINO


                    If Qs_TipoView = "P" Then
                        x_Lotto = ""
                    End If

                    ''-----------------------------------
                    '''GESTIONE CERTIFICAZIONI
                    ''-----------------------------------
                    'If objConfigStampe.Flag_CertificazioniDesc = True Then

                    '    Dim objDocContab As New AgronicaCoreStampeDAL.DocContab
                    '    Dim descr_cert As String = objDocContab.Descrizione_Certificazione_FF(Id_Mov_Det, objParametri_Server)

                    '    If descr_cert <> "" Then
                    '        x_Descrizione &= " - " & descr_cert
                    '    End If
                    'End If


                    x_Descrizione = objContab.LeggiProdottoStampeContab(objParametriServer,
                                                                        objParametriUtenti,
                                                                        nomeCategoria,
                                                                        Nome_Calibro,
                                                                        Peso_Set,
                                                                        Piva,
                                                                        x_Elem_Cod,
                                                                        x_Pro_Cod,
                                                                        mat_cod_xdesc,
                                                                        x_Cod_Progetto,
                                                                        x_Fase_Cod,
                                                                        x_Lotto,
                                                                        x_Cal_Cod,
                                                                        False,
                                                                         , ,
                                                                        Flag_Raggruppa,
                                                                        , ,
                                                                        x_ret_Cod_Articolo,
                                                                        x_ret_Descr_breve,
                                                                        x_Flag_Extra,
                                                                        x_OTabella_Cod_Base,
                                                                        MP_Udm_Cod_Extra,
                                                                        MP_Qta_Extra,
                                                                        objConfigStampe.Flag_CertificazioniDesc,
                                                                        x_Mat_Cod,
                                                                        titoloAlcol,
                                                                        moduliCliente)
            End Select


            'nel F&F devo concatenare i parametri qualitativi
            '01/06/2017 x Cofruta: qualità, calibro, certificazione, rugginosità - solo sigla in quest'ordine
            'uso l'ordine salvato in tabella e il chkEtichetta
            If moduloFreshFood = True Then
                'prima devo recuperare il veg_cod
                '(uso la stessa funzione che usa dopo nel caso del raggruppamento)
                objMatPrime.VegCod_CulCod_from_MatCod(Piva, x_Elem_Cod, x_Mat_Cod, Veg_Cod, Cul_Cod, objParametriServer)
                '----------------------------------
                'devo recuperare i parametri qualitativi solo se per il prodotto non è stato selezionato l'alias
                If Mat_Cod_Alias = 0 Then
                    Dim objMPCamp As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
                    Dim dettagliProdotto As String = ""
                    'Recupera_DettagliEConfezionamento_Prodotto
                    dettagliProdotto = objMPCamp.Recupera_Dettagli_Prodotto_XStampa(Piva, Veg_Cod, x_Cal_Cod,
                                                                                    " AND OModuli_Referenze_Config_Dettagli.ChkEtichetta = 1 ",
                                                                                    objParametriServer)

                    If dettagliProdotto <> "" Then
                        x_Descrizione &= dettagliProdotto
                    End If
                End If 'ChkAlias

            End If

            x_Qta = CDec(.Item("qta"))
            x_Qta_Extra = CDec(.Item("qta_extra"))
            x_Qta_Extra_Totale = CDec(.Item("qta_extra_totale")) 'peso netto totale

            x_Udm_Cod_Extra = .Item("udm_cod_extra")

            If x_Udm_Cod_Extra = 0 Then
                x_Udm_Sim_Extra = ""
                x_Udm_Des_Extra = ""
            Else
                x_Udm_Des_Extra = objUdm.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametriServer)
            End If

            'spostato sotto
            'If Peso_Set > 0 Then
            '    x_Descrizione &= " -- " & CStr(x_Qta_Extra) & " " & x_Udm_Sim_Extra
            'End If

            x_Tara = CDec(.Item("tara"))

            x_Prezzo_Unitario = CDec(.Item("prezzo_unitario"))
            x_Prezzo_Unitario_Netto = CDec(.Item("prezzo_unitario_netto"))

            x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDec(.Item("imponibile")))
            x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDec(.Item("imponibile_netto")))
            'arrotondamento spostato dopo

            'modifica del 17/09/2012: nella funzione lo sconto viene aggiunto, quindi va lasciato con segno -
            'x_Sconto_Perc = Math.Abs(.Item("sconto"))
            'x_Sconto_Perc_2 = Math.Abs(.Item("sconto_listino"))
            x_Sconto_Perc = CDec(.Item("sconto"))
            x_Sconto_Perc_2 = CDec(.Item("sconto_listino"))

            x_Sconto_Testo = .Item("sconto_testo")
            'x_Sconto = ((x_Prezzo_Unitario * x_Sconto_Perc) / 100) * x_Qta
            x_Sconto = x_Imponibile_Netto - x_Imponibile

            x_Sconto_Modalita = .Item("sconto_modalita")

            x_Cod_IVA = .Item("cod_iva")
            x_Aliquota_Des = .Item("Sigla_Iva")
            x_Aliquota_Val = .Item("Aliquota")

            'viene usato dalla stampa del ddt contabilizzato
            x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa(Lav_Cod, CDec(.Item("iva")))

            x_ChkIVA_Manuale = .Item("chkiva_manuale")
            x_Cod_IVAIndetraibile = .Item("cod_ivaindetraibile")

            x_ChkLayOut_Hide = .Item("chklayout_hide")

            objLanRound.InserisciRiga_DtDettagli(Dt_Dettagli_Round,
                                                Id_Mov_Det,
                                                x_ChkLayOut_Hide,
                                                x_Sconto_Modalita,
                                                x_Sconto_Perc,
                                                x_Sconto_Perc_2,
                                                x_Qta,
                                                x_Prezzo_Unitario,
                                                x_Prezzo_Unitario_Netto,
                                                x_Imponibile,
                                                x_Imponibile_Netto,
                                                x_Cod_IVA,
                                                x_IVA,
                                                x_Aliquota_Val,
                                                x_Aliquota_Des,
                                                0, 0, 0, 0)


            x_Imponibile = ArrotondaVal_2(x_Imponibile)
            x_Imponibile_Netto = ArrotondaVal_2(x_Imponibile_Netto)

            'DIC 2018 - Tolto arrotondamento, verrà eseguito più tardi sul castelletto
            'x_IVA = ArrotondaVal_2(x_IVA)

            x_Prezzo_Effettivo = CDec(.Item("prezzo_effettivo"))

            x_Anno = .Item("anno")
            x_Ric_Cod = .Item("ric_cod")

            x_Cod_Conto = .Item("cod_conto")
            If x_Cod_Conto = 0 Then
                x_Conto = ""
                'Else
                'in fase di stampa non mi serve il conto, quindi mi risparmio la lettura
                'x_Conto = ""
                'If Not IsNothing(.Item("conto")) AndAlso .Item("conto") <> "" Then
                '    'da filtro movimenti
                '    x_Conto = CStr(.Item("conto"))
                'Else
                '    'da form prodotto
                '    x_Conto = ContoDescr_from_CodConto(objServer, objSession, objPage, Piva, x_Ric_Cod, x_Anno, x_Cod_Conto, True)
                'End If
            End If

            x_Contabilizzato = .Item("contabilizzato")
            x_Pendente = .Item("pendente")

            '------------------------------------------------------------------
            '------------ VALORI RISCONTRATI USATI PER FATTURAZIONE (IN CASO DI DDT) ----------------
            '------------------------------------------------------------------
            x_N_Conf_Riscontrate = CInt(.Item("Num_Conf_Riscontrate"))
            x_N_Colli_Riscontrati = CInt(.Item("Num_Colli_Riscontrati"))
            x_N_Imballi_Riscontrati = CInt(.Item("Num_Imballi_Riscontrati"))
            x_Peso_Netto_Riscontrato = CDec(.Item("Peso_Netto_Riscontrato"))
            x_Peso_Lordo_Riscontrato = CDec(.Item("Peso_Lordo_Riscontrato"))
            x_Tara_Unit_Collo_Riscontrata = CDec(.Item("Tara_Unit_Collo_Riscontrata"))
            x_Tara_Unit_Imballo_Riscontrata = CDec(.Item("Tara_Unit_Imballo_Riscontrata"))

            Veg_Cod = 0
            Cul_Cod = 0

            If Flag_Raggruppa = True Then
                'nel caso del raggruppamento dei dettagli, ho bisogno di conoscere specie e varietà
                'x_mat_cod
                If mat_cod_xdesc <> 0 Then
                    objMatPrime.VegCod_CulCod_from_MatCod(Piva, x_Elem_Cod, mat_cod_xdesc, Veg_Cod, Cul_Cod, objParametriServer)
                End If
            End If


            'GESTIONE CAPACITA' / PESO NETTO
            'GESTIONE IMBALLAGGI E CONTENITORI
            'leggo gli imballaggi solo nel caso i prodotti non siano raggruppati
            If Flag_Raggruppa = False Or Qs_TipoView = "P" Then

                Try

                    Select Case x_Elem_Cod

                        Case BENI_CONFEZ_VEGETALE, BENI_CONFEZ_ANIMALE

                            'imballaggi movimentati dal magazzino
                            'verifico se si tratta di contenitori o di imballaggi

                            objMatPrime.Recupera_Chk_TipoConfezionamento(Piva,
                                                                         x_Elem_Cod,
                                                                         x_Mat_Cod,
                                                                         "",
                                                                         ChkConfezione,
                                                                         ChkContenitore,
                                                                         ChkImballaggio,
                                                                         objParametriServer)


                        Case Else
                            'lettura del confezionamento del prodotto

                            Dim objExtra As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R
                            Dim dtExtra As DataTable


                            dtExtra = objExtra.Leggi(Piva,
                                                     0,
                                                     .Item("id_agenda"),
                                                     .Item("Id_Mov_Magazzino"),
                                                     .Item("Id_Mov_Det"),
                                                     0,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "",
                                                     objParametriServer)

                            If Not IsNothing(dtExtra) AndAlso dtExtra.Rows.Count > 0 Then
                                Dim j As Integer
                                Dim flag_confez As Boolean = False
                                For j = 0 To dtExtra.Rows.Count - 1

                                    '-----------------------------------
                                    'GESTIONE CAPACITA' / PESO NETTO
                                    '-----------------------------------

                                    '06/08/2015
                                    'nel caso del fresh&food stampo anche il peso netto totale del dettaglio
                                    If moduloFreshFood = True Then

                                        If Flag_GestMaterialeVivaistico = True Then
                                            'modalità ZESPRIBUD
                                            'non bisogna aggiungere i pesi
                                            Dim Debug As Boolean = True
                                        Else
                                            'tutto il resto F&F
                                            If ChkLayOut_Peso = 0 Then
                                                If Peso_Set > 0 Then
                                                    Dim qtaDettaglio1 As Integer = .Item("Qta_Dettaglio1")
                                                    Dim qtaDettaglio2 As Integer = .Item("Qta_Dettaglio2")

                                                    '  Giulia, 23/01/2017 17:40:28: se udm = 2 (KG) vuol dire che non ho confezioni, quindi mostro il peso del confezionamento più interno
                                                    If x_Udm_Cod = 2 AndAlso qtaDettaglio1 <> 0 Then
                                                        x_Descrizione &= " - netto unit: " & CStr(ArrotondaVal_2(x_Qta_Extra_Totale / qtaDettaglio1)) & " " & x_Udm_Sim_Extra
                                                    ElseIf x_Udm_Cod = 2 AndAlso qtaDettaglio2 <> 0 Then
                                                        x_Descrizione &= " - netto unit: " & CStr(ArrotondaVal_2(x_Qta_Extra_Totale / qtaDettaglio2)) & " " & x_Udm_Sim_Extra
                                                    Else
                                                        x_Descrizione &= " - netto unit: " & CStr(ArrotondaVal_2(x_Qta_Extra)) & " " & x_Udm_Sim_Extra
                                                    End If
                                                    x_Descrizione &= ", netto tot: " & CStr(x_Qta_Extra_Totale) & " " & x_Udm_Sim_Extra

                                                    If x_Tara <> CDec(0) Then
                                                        x_Descrizione &= ", lordo tot: " & CStr(x_Qta_Extra_Totale + x_Tara) & " " & x_Udm_Sim_Extra
                                                    End If
                                                End If
                                            Else
                                                'è attiva la stampa con layout dettagli con tutti i pesi,
                                                'quindi non serve metterlo in riga
                                                Dim Debug As Boolean = True
                                            End If

                                        End If
                                    Else
                                        'caso cantine (il GiasLan ancora non salva 1 che è il modulo gias cantine)
                                        If Peso_Set > 0 Then
                                            x_Descrizione &= " - capacità: " & CStr(x_Qta_Extra) & " " & x_Udm_Sim_Extra
                                            x_Descrizione &= ", tot " & x_Udm_Sim_Extra & ": " & CStr(x_Qta_Extra_Totale)
                                        End If
                                    End If


                                    '-----------------------------------
                                    ''GESTIONE IMBALLAGGI E CONTENITORI
                                    '-----------------------------------

                                    'mail Modifiche stampe del 11/01/2011 11.11
                                    'per quanto riguarda la gestione dei colli i campi nella tabella mov_dettaglio_tecnico_extra sono
                                    '            num_colli
                                    '            contenitore_cod
                                    'per quanto riguarda la gestione degli imballaggi (non per conferimento)  i campi nella tabella mov_dettaglio_tecnico_extra sono
                                    '            num_contenitori
                                    '            imballaggio_cod

                                    numcolli_contenitori = dtExtra.Rows(j).Item("num_colli")
                                    contenitore_cod = dtExtra.Rows(j).Item("contenitore_cod")

                                    If contenitore_cod <> 0 Then
                                        Dim matDesContenitore As String
                                        matDesContenitore = objMatPrime.MatDes_from_MatCod(Piva,
                                                                                           BENI_CONFEZ_VEGETALE,
                                                                                           contenitore_cod,
                                                                                           "",
                                                                                           Nothing,
                                                                                           "",
                                                                                           objParametriServer)
                                        If matDesContenitore <> "" Then
                                            If flag_confez = True Then
                                                descr_contenitore &= " - "
                                            End If
                                            descr_contenitore &= "n." & CStr(numcolli_contenitori) & " " & matDesContenitore
                                            flag_confez = True
                                        End If
                                    End If

                                    numcontenitori_imballi = dtExtra.Rows(j).Item("num_contenitori")
                                    imballaggio_cod = dtExtra.Rows(j).Item("imballaggio_cod")

                                    If imballaggio_cod <> 0 Then
                                        Dim matDesImballo As String
                                        matDesImballo = objMatPrime.MatDes_from_MatCod(Piva,
                                                                                       BENI_CONFEZ_VEGETALE,
                                                                                       imballaggio_cod,
                                                                                       "",
                                                                                       Nothing,
                                                                                       "",
                                                                                       objParametriServer)
                                        If matDesImballo <> "" Then
                                            If flag_confez = True Then
                                                descr_imballo &= " - "
                                            End If
                                            descr_imballo &= "n." & CStr(numcontenitori_imballi) & " " & matDesImballo
                                        End If
                                    End If

                                    '  Giulia, 20/01/2017 17:42:31: Aggiunta possibilità di scegliere quali livelli di confezionamento aggiungere alla descrizione
                                    If objConfigStampe.Flag_ConfezioniDesc = True OrElse
                                       objConfigStampe.Flag_ContenitoriDesc = True OrElse
                                       objConfigStampe.Flag_ImballaggiDesc = True Then
                                        'solo se ne devo stampare almeno uno
                                        Dim confezionamentoDesc As String = ""
                                        Dim objDocContab As New AgronicaCoreStampeDAL.DocContab

                                        If objConfigStampe.Flag_ConfezioniDesc = True Then
                                            'aggiungi confezioni

                                            Dim descrConfezione As String = objDocContab.Descrizione_Confezione_FF(Id_Mov_Det, objParametriServer)

                                            If descrConfezione <> "" Then
                                                confezionamentoDesc = "<br>" & "("
                                                descrConfezione = "n. " & CStr(x_Qta) & " " & descrConfezione
                                                confezionamentoDesc &= descrConfezione
                                            End If
                                        End If

                                        If objConfigStampe.Flag_ContenitoriDesc = True AndAlso descr_contenitore <> "" Then
                                            'aggiungi contenitori
                                            If confezionamentoDesc <> "" Then
                                                confezionamentoDesc &= " - "
                                            Else
                                                confezionamentoDesc = "<br>" & "("
                                            End If

                                            confezionamentoDesc &= descr_contenitore

                                            If moduloFreshFood = True AndAlso objConfigStampe.Flag_TaraContenitoreDesc = True Then
                                                Dim taraContenitore As String = ""
                                                If x_Tara_Unit_Collo_Riscontrata <> -1 AndAlso flagTareRiscontrateInDesc = True Then
                                                    taraContenitore = CStr(x_Tara_Unit_Collo_Riscontrata)
                                                Else
                                                    taraContenitore = objDocContab.Tare_Confez_Contenitore_Imballo_FF(Id_Mov_Det, "ocontenitore", objParametriServer)
                                                End If
                                                confezionamentoDesc &= " tara kg " & taraContenitore
                                            End If

                                            'nella descrizione breve includo solo il livello dei contenitori
                                            x_ret_Descr_breve &= "<br>" & "(" & descr_contenitore & ")"
                                        End If

                                        If objConfigStampe.Flag_ImballaggiDesc = True AndAlso descr_imballo <> "" Then
                                            'aggiungi imballaggi
                                            If confezionamentoDesc <> "" Then
                                                confezionamentoDesc &= " "
                                            Else
                                                confezionamentoDesc = "<br>" & "("
                                            End If

                                            confezionamentoDesc &= descr_imballo

                                            If moduloFreshFood = True AndAlso objConfigStampe.Flag_TaraImballaggioDesc = True Then
                                                Dim taraImballo As String = ""
                                                If x_Tara_Unit_Imballo_Riscontrata <> -1 AndAlso flagTareRiscontrateInDesc = True Then
                                                    taraImballo = CStr(x_Tara_Unit_Imballo_Riscontrata)
                                                Else
                                                    taraImballo = objDocContab.Tare_Confez_Contenitore_Imballo_FF(Id_Mov_Det, "oimballaggio", objParametriServer)
                                                End If
                                                confezionamentoDesc &= " tara kg " & taraImballo
                                            End If

                                        End If

                                        If confezionamentoDesc <> "" Then
                                            confezionamentoDesc &= ")"
                                        End If

                                        x_Descrizione &= confezionamentoDesc
                                    End If

                                    'If descr_imballo <> "" Or descr_contenitore <> "" Then
                                    '    'x_Descrizione &= " (" & descr_contenitore & " " & descr_imballo & ")"
                                    '    x_Descrizione &= "<br>" & "(" & descr_contenitore & " " & descr_imballo & ")"

                                    '    If descr_contenitore <> "" Then
                                    '        x_ret_Descr_breve &= "<br>" & "(" & descr_contenitore & ")"
                                    '    End If
                                    'End If
                                Next

                            Else
                                '06/08/2015
                                'nel caso non ci siano salvati extra sul dettaglio
                                If Peso_Set > 0 Then
                                    If x_Qta_Extra_Totale = 0 Then
                                        x_Qta_Extra_Totale = x_Qta * x_Qta_Extra
                                    End If
                                    Select Case x_Udm_Cod_Extra
                                        Case enum_UnitaMisura.Litri
                                            x_Descrizione &= " - capacità: " & CStr(x_Qta_Extra) & " " & x_Udm_Sim_Extra
                                            x_Descrizione &= ", tot " & x_Udm_Sim_Extra & ": " & CStr(x_Qta_Extra_Totale)
                                        Case enum_UnitaMisura.KG
                                            x_Descrizione &= " - peso unit. " & CStr(x_Qta_Extra) & " " & x_Udm_Sim_Extra
                                            If Qs_TipoView <> "P" Then
                                                x_Descrizione &= ", peso netto tot. " & CStr(x_Qta_Extra_Totale) & " " & x_Udm_Sim_Extra
                                            End If
                                        Case Else
                                            x_Descrizione &= " - " & CStr(x_Qta_Extra) & " " & x_Udm_Sim_Extra
                                    End Select
                                End If

                            End If 'extra

                    End Select

                Catch ex As Exception
                    logErrori &= "- Lettura degli imballaggi e contenitori: " & vbCrLf & ex.Message & vbCrLf
                End Try

            End If 'Flag_Raggruppa

            '-------------------------------------------------
            '------------ NOTE AGGIUNTIVE ----------------
            '-------------------------------------------------
            x_Extra_Str_Dettagli = .Item("extra_str_dett")
            x_Extra_Int_Dettagli = .Item("extra_int_dett")
            x_Extra_Date_Dettagli = .Item("extra_date_dett")

            '  Giulia, 05/10/2017 17:37:31: toppa per coprire il caso di nota di credito di un bene strumentale, dove i due campi potrebbero essere uguali
            If x_Extra_Str_Dettagli <> "" AndAlso x_Extra_Str_Dettagli <> x_Mov_Det_Des Then
                x_Descrizione &= " " & x_Extra_Str_Dettagli
            End If

            '-------------------------------------------------
            '--------------- % UVE DIRASPATE -----------------
            '-------------------------------------------------

            If Flag_UveDiraspate = True Then
                '01/08/2017: il GiasLan salva il dato solo sul movimento_dettaglio collegato al movimento con cau_mov= 7920,
                'ma la stampa legge i dettagli del movimento con cau_mov 4070
                'x_Descrizione &= "<br>Ingresso uve fresche diraspate; Raspi " & CStr(.Item("Perc_UveDiraspate")) & "%"
                x_Descrizione &= "<br>Ingresso uve fresche diraspate; Raspi 2%"
            End If

            '--------------------------------------------------------
            '------------ FORMATTAZIONE TESTO A CAPO ----------------
            '--------------------------------------------------------
            'modifica del 30/11/2015 x Andreola e altri: poter scegliere quando mandare a capo il testo nel prodotto
            'stabilito carattere § per mandare a capo
            'usare il <br> e non il <br/> altrimenti non funziona
            'x_Descrizione = Replace(x_Descrizione, "§", "<br/>")
            x_Descrizione = Replace(x_Descrizione, "§", "<br>")


            '--------------------------------------------------------
            '------------ GESTIONE UDM ASPETTO DEFAULT ----------------
            '--------------------------------------------------------
            'sviluppo fatto il 29/02/2016 per Maiorano
            'udm principale = numero
            'ma in certi casi vogliono che venga stampato kg
            'si usa quindi la configurazione sulla materia prima
            'che è il check flag_extra (udm aspetto default)
            If x_Flag_Extra = 1 Then

                'sostituisco l'udm principale
                If Peso_Set > 0 Then
                    'caso di peso effettivo -> prende i dati da movimenti dettagli
                    'x_Udm_Cod_Extra -> ok è quello di movimenti_dettagli
                    'x_Qta_Extra -> ok è quello di movimenti_dettagli
                Else
                    'caso di peso nominale -> prende i dati della materia prima
                    x_Udm_Cod_Extra = MP_Udm_Cod_Extra
                    x_Qta_Extra = MP_Qta_Extra

                    Select Case x_Udm_Cod
                        Case enum_UnitaMisura.KG
                            x_Udm_Des_Extra = "kg"
                            x_Udm_Sim_Extra = "kg"
                        Case enum_UnitaMisura.Litri
                            x_Udm_Des_Extra = "l"
                            x_Udm_Sim_Extra = "l"
                        Case Else
                            x_Udm_Des_Extra = objUdm.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametriServer)
                    End Select

                End If

                'Select Case x_Udm_Cod
                '    Case enum_UnitaMisura.KG
                '        x_Udm_Des = "kg"
                '        x_Udm_Sim = "kg"
                '    Case enum_UnitaMisura.Litri
                '        x_Udm_Des = "l"
                '        x_Udm_Sim = "l"
                '    Case Else
                '        x_Udm_Des = objUdm.UdmDes_from_UdmCod(x_Udm_Cod, x_Udm_Sim, objParametri_Server)
                'End Select
                ''sostituisco anche il prezzo perché non è più quello unitario, ma quello riferito all'udm aspetto
                'x_Prezzo_Unitario = x_Prezzo_Effettivo
            End If

            '------------------------------------------------------------------
            '------------ LIVELLO DEL PREZZO UNITARIO IMPUTATO ----------------
            '------------------------------------------------------------------
            '  Giulia, 18/01/2017 09:54:50: livello a cui si riferisce il prezzo imputato:
            '       il prezzo unitario salvato sul db si riferisce sempre al livello più interno,
            '       ma usando questa impostazione posso capire se l'utente ha prezzato a livello di contenitori (per esempio) 
            '       e quindi in stampa mostrare il prezzo come l'ha imputato lui, ricalcolandolo sui contenitori
            '       e mostrare il numero dei contenitori (e non delle confezioni) come quantità.
            '       Se è zero non dovrò fare nessun ricalcolo
            x_Prezzo_Livello = CInt(.Item("Prezzo_Livello"))


            '--------------------------------------------------------------
            '------------ SERBATOIO/VASCA DI PROVENIENZA  ----------------
            '------------------------------------------------------------

            'abilito per tutti i documenti contabili
            'Select Case Lav_Cod

            '    Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

            Try

                'se lo scarico avviene da una vasca e ho attiva l'opzione di stampare il numero vasca
                If .Item("Tipo_Destinazione") = 13 And Flag_StampaNumeroVasca = True Then
                    Dim objVasca As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R
                    Dim identificativo As String
                    identificativo = objVasca.Identificativo_from_VasCod(Piva,
                                                                        .Item("Sa_Cod_Dest"),
                                                                        .Item("Id_Destinazione"),
                                                                        objParametriServer)
                    If identificativo <> "" Then
                        x_Descrizione &= " (n. vasca: " & identificativo & ")"
                    End If
                End If

            Catch ex As Exception
                logErrori &= "- Lettura del SERBATOIO/VASCA del vino sfuso: " & vbCrLf & ex.Message & vbCrLf
            End Try

            ' End Select


            '-------------------------------------------------
            '------------ DOCUMENTI COLLEGATI ----------------
            '-------------------------------------------------

            Select Case Lav_Cod

                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                    LAVCOD_RICEVUTA_EMESSA,
                    LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                    Try

                        '---------------------------------------------------------------
                        '------- BOLLA /DOCO / DDT / ORDINE COLLEGATI ------------------
                        '---------------------------------------------------------------

                        'al momento è usato solo dalla funzione omonima del fresh&food
                        ' Dim Flag_DDTallegati As Boolean

                        Rif_Ordine = ""
                        If .Item("N_Doc_Cliente") <> "" Then
                            Rif_Ordine = "Rif. Ordine n." & .Item("N_Doc_Cliente") & " del " & CDate(.Item("Data_Doc_Cliente")).ToShortDateString
                        End If

                        '04/02/2019: aggiunto if
                        If Rif_Ordine <> "" Then
                            'se l'utente ha specificato un numero ordine del suo cliente
                            'questo prende la priorità
                            Riferimento_DocAllegato = Rif_Ordine
                        Else

                            Riferimento_DocAllegato = Leggi_RiferimentoDocAllegato(objParametriServer,
                                                                                   Piva,
                                                                                   .Item("id_agenda"),
                                                                                   .Item("id_mov_magazzino"),
                                                                                   .Item("id_mov_det"),
                                                                                   Flag_DDTallegati,
                                                                                   Flag_ORDallegati,
                                                                                   Dettaglio_Evasione)

                        End If

                    Catch ex As Exception
                        logErrori &= "- Lettura del riferimento documento allegato: " & vbCrLf & ex.Message & vbCrLf
                    End Try

            End Select

        End With

        objParametriServer.ResettaFinestra()


    End Sub


    '#####################################################################################################
    'al momento viene chiamata dalla stampa personalizzata ddt/fatture di Gandini
    <Obsolete("Al momento non viene più usata. ormai disallineata su nuovi sviluppi")>
    Public Sub Leggi_MovimentoDettaglio_DocContabile_GANDINI(ByRef objParametri_Server As AgronicaCoreParametri,
                                                             ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                             ByRef Log_Errori As String,
                                                             ByVal Dr As DataRow,
                                                             ByVal Piva As String,
                                                             ByVal Lav_Cod As Integer,
                                                             ByVal Flag_Raggruppa As Boolean,
                                                             ByRef x_Mov_Det_Des As String,
                                                             ByRef x_Elem_Cod As Integer,
                                                             ByRef x_Pro_Cod As Integer,
                                                             ByRef x_Mat_Cod As Integer,
                                                             ByRef x_Cod_Progetto As Integer,
                                                             ByRef x_Fase_Cod As Integer,
                                                             ByRef x_Lotto As String,
                                                             ByRef x_Cal_Cod As Integer,
                                                             ByRef x_Udm_Cod As Integer,
                                                             ByRef x_Udm_Sim As String,
                                                             ByRef x_Udm_Des As String,
                                                             ByRef x_Udm_Cod_Extra As Integer,
                                                             ByRef x_Udm_Sim_Extra As String,
                                                             ByRef x_Udm_Des_Extra As String,
                                                             ByRef x_Qta As Decimal,
                                                             ByRef x_Qta_Extra As Decimal,
                                                             ByRef x_Qta_Extra_Totale As Decimal,
                                                             ByRef x_Descrizione As String,
                                                             ByRef x_Prezzo_Unitario As Decimal,
                                                             ByRef x_Prezzo_Unitario_Netto As Decimal,
                                                             ByRef x_Imponibile As Decimal,
                                                             ByRef x_Imponibile_Netto As Decimal,
                                                             ByRef x_Cod_IVA As Integer,
                                                             ByRef x_Aliquota As String,
                                                             ByRef x_IVA As Decimal,
                                                             ByRef x_ChkIVA_Manuale As Integer,
                                                             ByRef x_Cod_IVAIndetraibile As Integer,
                                                             ByRef x_Sconto_Perc As Decimal,
                                                             ByRef x_Sconto_Perc_2 As Decimal,
                                                             ByRef x_Sconto_Testo As String,
                                                             ByRef x_Sconto As Decimal,
                                                             ByRef x_Sconto_Modalita As Integer,
                                                             ByRef x_Prezzo_Effettivo As Decimal,
                                                             ByRef x_Anno As Integer,
                                                             ByRef x_Ric_Cod As Integer,
                                                             ByRef x_Cod_Conto As Integer,
                                                             ByRef x_Conto As String,
                                                             ByRef x_Contabilizzato As Integer,
                                                             ByRef x_Pendente As Integer,
                                                             ByRef x_ChkLayOut_Hide As Integer,
                                                             ByRef x_Tara As Decimal,
                                                             ByRef x_Extra_Str_Dettagli As String,
                                                             ByRef x_Extra_Int_Dettagli As Integer,
                                                             ByRef x_Extra_Date_Dettagli As Date,
                                                             ByRef Veg_Cod As Integer,
                                                             ByRef Cul_Cod As Integer,
                                                             ByRef Nome_Calibro As String,
                                                             ByRef Riferimento_DocAllegato As String,
                                                             ByRef numcolli_contenitori As Integer,
                                                             ByRef contenitore_cod As Integer,
                                                             ByRef descr_contenitore As String,
                                                             ByRef numcontenitori_imballi As Integer,
                                                             ByRef imballaggio_cod As Integer,
                                                             ByRef descr_imballo As String,
                                                             ByRef Flag_DDTallegati As Boolean,
                                                             ByRef Flag_ORDAllegati As Boolean,
                                                             ByRef ChkConfezione As Integer,
                                                             ByRef ChkContenitore As Integer,
                                                             ByRef ChkImballaggio As Integer,
                                                             ByRef Dettaglio_Evasione As String,
                                                             Optional ByRef Dt_Dettagli_Round As DataTable = Nothing,
                                                             Optional ByRef Flag_ProdottoAgroAlimentare As Boolean = False)


        '====================================================================================
        '---------------- Dt dettagli per round finale e riepiloghi ----------------------------
        Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
        If IsNothing(Dt_Dettagli_Round) Then

            Try
                Dt_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

            Catch ex As Exception
                Log_Errori &= "- Dt dettagli per round finale e riepiloghi: " & vbCrLf & ex.Message & vbCrLf
            End Try

        End If

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        'ByRef mat_des_imballo_contenitore As String, _
        'ByRef num_imballi_contenitori As Integer)

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim objContab As New AgronicaCoreContabDAL.Contabilita_R
        Dim Tipo As Integer
        Dim Nome_Categoria As String = ""
        Dim Peso_Set As Integer
        Dim Mat_Cod_Alias As Integer = 0
        Dim Id_Mov_Det As Integer

        With Dr

            Id_Mov_Det = .Item("id_mov_det")

            x_Mov_Det_Des = .Item("mov_det_des")

            'chiave del prodotto
            x_Elem_Cod = .Item("elem_cod")
            x_Pro_Cod = .Item("pro_cod")
            x_Mat_Cod = .Item("mat_cod")
            x_Cod_Progetto = .Item("cod_progetto")
            x_Fase_Cod = .Item("fase_cod")
            x_Lotto = .Item("lotto")
            x_Cal_Cod = .Item("cal_cod")
            x_Udm_Cod = .Item("udm_cod")

            If x_Udm_Cod = 0 Then
                x_Udm_Sim = ""
                x_Udm_Des = ""
            Else
                x_Udm_Des = objUdm.UdmDes_from_UdmCod(x_Udm_Cod, x_Udm_Sim, objParametri_Server)
            End If

            Mat_Cod_Alias = .Item("mat_cod_alias")

            Dim mat_cod_xdesc As Integer
            If Mat_Cod_Alias <> 0 Then
                mat_cod_xdesc = Mat_Cod_Alias
            Else
                mat_cod_xdesc = x_Mat_Cod
            End If

            Select Case x_Elem_Cod

                Case SERVIZI
                    'non può essere!
                    x_Descrizione = x_Mov_Det_Des
                    '-------------------------------------

                Case ALTRI_BENI, MACCHINE

                    Tipo = CAU_MAGAZZINO 'anche se in realtà non movimenta il magazzino

                    x_Descrizione = x_Mov_Det_Des
                    '-------------------------------------

                Case ZOO_CONSISTENZA 'Consistenze Animali

                    Flag_ProdottoAgroAlimentare = True

                    Tipo = CAU_ANIMALE

                    If x_Cod_Progetto <> 0 Then

                        x_Descrizione = objContab.LeggiProdottoStampeContab(objParametri_Server, _
                                                        objParametri_Utenti, _
                                                        Nome_Categoria, _
                                                        Nothing, _
                                                        Nothing, _
                                                        Piva, _
                                                        x_Elem_Cod, _
                                                        x_Pro_Cod, _
                                                        x_Mat_Cod, _
                                                        x_Cod_Progetto, _
                                                        0, _
                                                        "", _
                                                        0, _
                                                        False, _
                                                        , , _
                                                        )

                    Else
                        x_Descrizione = "Consistenze Zootecniche: " & x_Mov_Det_Des
                    End If

                    '-------------------------------------

                Case Else

                    Select Case x_Elem_Cod
                        Case SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, TRASFORMATI_VEGETALI, _
                                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, TRASFORMATI_ANIMALI
                            Flag_ProdottoAgroAlimentare = True
                    End Select

                    Tipo = CAU_MAGAZZINO

                    x_Descrizione = objContab.LeggiProdottoStampeContab(objParametri_Server, _
                                                                        objParametri_Utenti, _
                                                                        Nome_Categoria, _
                                                                        Nome_Calibro, _
                                                                        Peso_Set, _
                                                                        Piva, _
                                                                        x_Elem_Cod, _
                                                                        x_Pro_Cod, _
                                                                        mat_cod_xdesc, _
                                                                        x_Cod_Progetto, _
                                                                        x_Fase_Cod, _
                                                                        x_Lotto, _
                                                                        x_Cal_Cod, _
                                                                        False, _
                                                                         , , _
                                                                        Flag_Raggruppa, _
                                                                        , , , , _
                                                                        , _
                                                                        , _
                                                                        )

            End Select

            x_Qta = .Item("qta")
            x_Qta_Extra = .Item("qta_extra")
            x_Qta_Extra_Totale = .Item("qta_extra_totale") 'peso netto totale

            x_Udm_Cod_Extra = .Item("udm_cod_extra")

            If x_Udm_Cod_Extra = 0 Then
                x_Udm_Sim_Extra = ""
                x_Udm_Des_Extra = ""
            Else
                x_Udm_Des_Extra = objUdm.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametri_Server)
            End If

            'nel fresh&food non serve 
            'If Peso_Set > 0 Then
            '    x_Descrizione &= " - " & CStr(x_Qta_Extra) & " " & x_Udm_Sim_Extra
            'End If

            x_Tara = .Item("tara")

            x_Prezzo_Unitario = CDec(.Item("prezzo_unitario"))
            x_Prezzo_Unitario_Netto = CDec(.Item("prezzo_unitario_netto"))

            x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDec(.Item("imponibile")))
            x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDec(.Item("imponibile_netto")))
            'arrotondamento spostato dopo

            'modifica del 17/09/2012: nella funzione lo sconto viene aggiunto, quindi va lasciato con segno -
            x_Sconto_Perc = .Item("sconto")
            x_Sconto_Perc_2 = .Item("sconto_listino")

            x_Sconto_Testo = .Item("sconto_testo")
            'x_Sconto = ((x_Prezzo_Unitario * x_Sconto_Perc) / 100) * x_Qta
            x_Sconto = x_Imponibile_Netto - x_Imponibile

            x_Sconto_Modalita = .Item("sconto_modalita")

            x_Cod_IVA = .Item("cod_iva")
            x_Aliquota = .Item("Sigla_Iva")
            Dim Aliquota_Cod As Integer = .Item("Aliquota")

            'viene usato dalla stampa del ddt contabilizzato
            x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa(Lav_Cod, CDec(.Item("iva")))

            x_ChkIVA_Manuale = .Item("chkiva_manuale")
            x_Cod_IVAIndetraibile = .Item("cod_ivaindetraibile")

            x_ChkLayOut_Hide = .Item("chklayout_hide")

            objLanRound.InserisciRiga_DtDettagli(Dt_Dettagli_Round,
                                                Id_Mov_Det,
                                                x_ChkLayOut_Hide,
                                                x_Sconto_Modalita,
                                                x_Sconto_Perc,
                                                x_Sconto_Perc_2,
                                                x_Qta,
                                                x_Prezzo_Unitario,
                                                x_Prezzo_Unitario_Netto,
                                                x_Imponibile,
                                                x_Imponibile_Netto,
                                                x_Cod_IVA,
                                                x_IVA,
                                                Aliquota_Cod,
                                                x_Aliquota,
                                                0, 0, 0, 0)

            x_Imponibile = RoundNumber_2Decimali(x_Imponibile)
            x_Imponibile_Netto = RoundNumber_2Decimali(x_Imponibile_Netto)
            x_IVA = RoundNumber_2Decimali(x_IVA)

            x_Prezzo_Effettivo = .Item("prezzo_effettivo")

            x_Anno = .Item("anno")
            x_Ric_Cod = .Item("ric_cod")

            x_Cod_Conto = .Item("cod_conto")
            If x_Cod_Conto = 0 Then
                x_Conto = ""
            End If

            x_Contabilizzato = .Item("contabilizzato")
            x_Pendente = .Item("pendente")

            Veg_Cod = 0
            Cul_Cod = 0

            If Flag_Raggruppa = True Then
                'nel caso del raggruppamento dei dettagli, ho bisogno di conoscere specie e varietà
                'x_mat_cod
                If mat_cod_xdesc <> 0 Then
                    Dim objMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    objMatPrime.VegCod_CulCod_from_MatCod(Piva, x_Elem_Cod, mat_cod_xdesc, Veg_Cod, Cul_Cod, objParametri_Server)
                End If
            End If


            'GESTIONE IMBALLAGGI E CONTENITORI
            ''leggo gli imballaggi solo nel caso i prodotti non siano raggruppati
            If Flag_Raggruppa = False Then

                Select Case x_Elem_Cod

                    Case BENI_CONFEZ_VEGETALE, BENI_CONFEZ_ANIMALE

                        'imballaggi movimentati dal magazzino
                        'verifico se si tratta di contenitori o di imballaggi

                        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                        objMP.Recupera_Chk_TipoConfezionamento(Piva,
                                                               x_Elem_Cod,
                                                               x_Mat_Cod,
                                                               "",
                                                               ChkConfezione,
                                                               ChkContenitore,
                                                               ChkImballaggio,
                                                               objParametri_Server)


                    Case Else

                        'lettura del confezionamento del prodotto

                        Try

                            Dim objExtra As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R
                            Dim Dt_Extra As DataTable

                            Dt_Extra = objExtra.Leggi(Piva,
                                                      0,
                                                      .Item("id_agenda"),
                                                      .Item("Id_Mov_Magazzino"),
                                                      .Item("Id_Mov_Det"),
                                                      0,
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "", "",
                                                      objParametri_Server)

                            If Not IsNothing(Dt_Extra) AndAlso Dt_Extra.Rows.Count > 0 Then
                                Dim j As Integer
                                'Dim flag_confez As Boolean = False
                                For j = 0 To Dt_Extra.Rows.Count - 1

                                    'mail Modifiche stampe del 11/01/2011 11.11
                                    'per quanto riguarda la gestione dei colli i campi nella tabella mov_dettaglio_tecnico_extra sono
                                    '            num_colli
                                    '            contenitore_cod
                                    'per quanto riguarda la gestione deigli imballaggi (non per conferimento)  i campi nella tabella mov_dettaglio_tecnico_extra sono
                                    '            num_contenitori
                                    '            imballaggio_cod

                                    'Dim num_colli, contenitore_cod, num_contenitori, imballaggio_cod As Integer
                                    'Dim mat_des_imballo_contenitore, descr_imballo, descr_contenitore As String

                                    numcolli_contenitori = Dt_Extra.Rows(j).Item("num_colli")
                                    contenitore_cod = Dt_Extra.Rows(j).Item("contenitore_cod")

                                    If contenitore_cod <> 0 Then
                                        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                        descr_contenitore = objMP.MatDes_from_MatCod(Piva, _
                                                                                    BENI_CONFEZ_VEGETALE, _
                                                                                    contenitore_cod, _
                                                                                    "", _
                                                                                    Nothing, _
                                                                                    "", _
                                                                                    objParametri_Server)

                                    End If

                                    numcontenitori_imballi = Dt_Extra.Rows(j).Item("num_contenitori")
                                    imballaggio_cod = Dt_Extra.Rows(j).Item("imballaggio_cod")

                                    If imballaggio_cod <> 0 Then
                                        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                        descr_imballo = objMP.MatDes_from_MatCod(Piva, _
                                                                                BENI_CONFEZ_VEGETALE, _
                                                                                imballaggio_cod, _
                                                                                "", _
                                                                                Nothing, _
                                                                                "", _
                                                                                objParametri_Server)
                                    End If

                                Next

                            End If 'extra


                        Catch ex As Exception
                            Log_Errori &= "- Lettura degli imballaggi e contenitori: " & vbCrLf & ex.Message & vbCrLf
                        End Try

                End Select

            End If 'Flag_Raggruppa

            '-------------------------------------------------
            '------------ NOTE AGGIUNTIVE ----------------
            '-------------------------------------------------
            x_Extra_Str_Dettagli = .Item("extra_str_dett")
            x_Extra_Int_Dettagli = .Item("extra_int_dett")
            x_Extra_Date_Dettagli = .Item("extra_date_dett")

            If x_Extra_Str_Dettagli <> "" Then
                x_Descrizione &= " " & x_Extra_Str_Dettagli
            End If

            '-------------------------------------------------
            '------------ DOCUMENTI COLLEGATI ----------------
            '-------------------------------------------------

            Select Case Lav_Cod

                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, _
                        LAVCOD_RICEVUTA_EMESSA, _
                        LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                    Try

                        '---------------------------------------------------------------
                        '------ BOLLA /DOCO / DDT / ORDINE COLLEGATI ------------------
                        '---------------------------------------------------------------

                        Riferimento_DocAllegato = Leggi_RiferimentoDocAllegato(objParametri_Server, _
                                                                               Piva, _
                                                                               .Item("id_agenda"), _
                                                                               .Item("id_mov_magazzino"), _
                                                                               .Item("id_mov_det"), _
                                                                               Flag_DDTallegati, _
                                                                               Flag_ORDAllegati, _
                                                                               Dettaglio_Evasione)

                    Catch ex As Exception
                        Log_Errori &= "- Lettura del riferimento documento allegato: " & vbCrLf & ex.Message & vbCrLf
                    End Try

            End Select


        End With

        objParametri_Server.ResettaFinestra()


    End Sub


    '########################################################################################
    'Bolla/doco/ordine agganciata a fattura/bolla: il riferimento è legato al movimento dettaglio
    Public Function Leggi_RiferimentoDocAllegato(ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByVal Piva As String,
                                                 ByVal Id_Agenda As Integer,
                                                 ByVal Id_Mov As Integer,
                                                 ByVal Id_Mov_Det As Integer,
                                                 ByRef Flag_DDTallegati As Boolean,
                                                 ByRef Flag_ORDAllegati As Boolean,
                                                 ByRef Dettaglio_Evasione As String
                                                 ) As String


        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)


        Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim Dt_rif As DataTable
        Dim Filtro As String
        Dim Lav_Cod_Bolla As Integer
        Dim Piva_Bolla As String
        Dim Sa_Cod_Bolla As Integer
        Dim Id_Agenda_Bolla As Integer
        Dim Id_Mov_Bolla As Integer
        Dim Id_Mov_Det_Bolla As Integer
        Dim Num_Bolla_Sin As String
        Dim Num_Bolla As String
        Dim Num_Bolla_Des As String
        Dim Data_Bolla As String
        Dim Riferimento_DocAllegato As String = ""

        'Filtro = "  ( lav_cod_rif = " & CStr(LAVCOD_BOLLA_EMESSA) & _
        '            " OR " & _
        '            "lav_cod_rif= " & CStr(LAVCOD_BOLLA_RICEVUTA) & _
        '            " OR " & _
        '            "lav_cod_rif= " & CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & _
        '            " OR " & _
        '            "lav_cod_rif= " & CStr(LAVCOD_DOCO_EMESSO) & _
        '            " OR " & _
        '            "lav_cod_rif= " & CStr(LAVCOD_CONFERIMENTO) & _
        '            " OR " & _
        '            "lav_cod_rif= " & CStr(LAVCOD_CONFERIMENTO_DIVERSI) & _
        '            ") "

        Filtro = "  lav_cod_rif IN ( " & CStr(LAVCOD_BOLLA_EMESSA) & ", " &
                                        CStr(LAVCOD_BOLLA_RICEVUTA) & ", " &
                                        CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                        CStr(LAVCOD_ORDINE_VENDITA) & ", " &
                                        CStr(LAVCOD_ORDINE_ACQUISTO) & ", " &
                                        CStr(LAVCOD_DOCO_RICEVUTO) & "," &
                                        CStr(LAVCOD_DOCO_EMESSO) & "," &
                                        CStr(LAVCOD_DAA_EMESSO) & "," &
                                        CStr(LAVCOD_MVV_EMESSO) & "," &
                                        CStr(LAVCOD_MVV_RICEVUTO) & "," &
                                        CStr(LAVCOD_CONFERIMENTO) & ", " &
                                        CStr(LAVCOD_CONFERIMENTO_DIVERSI) &
                                        "  ) "

        Dt_rif = objRif.Leggi_DDT_Nota_aggancio_Fattura(Piva,
                                                        0,
                                                        Id_Agenda,
                                                        Id_Mov,
                                                        Id_Mov_Det,
                                                        0,
                                                        "",
                                                        Piva,
                                                        0, 0, 0, 0, 0, "",
                                                        Filtro, "",
                                                        objParametri_Server,
                                                        True)

        If Not IsNothing(Dt_rif) AndAlso Dt_rif.Rows.Count > 0 Then
            Lav_Cod_Bolla = Dt_rif.Rows(0).Item("lav_cod_rif")
            Piva_Bolla = Dt_rif.Rows(0).Item("piva_rif")
            Sa_Cod_Bolla = Dt_rif.Rows(0).Item("sa_cod_rif")
            Id_Agenda_Bolla = Dt_rif.Rows(0).Item("id_agenda_rif")
            Id_Mov_Bolla = Dt_rif.Rows(0).Item("id_mov_rif")
            Id_Mov_Det_Bolla = Dt_rif.Rows(0).Item("id_mov_det_rif")

            Num_Bolla_Sin = Dt_rif.Rows(0).Item("Doc_Numero_Sin")
            Num_Bolla = Dt_rif.Rows(0).Item("Doc_Numero")
            Num_Bolla_Des = Dt_rif.Rows(0).Item("Doc_Numero_Des")
            Data_Bolla = Dt_rif.Rows(0).Item("Data_movimento")

            Select Case Lav_Cod_Bolla
                Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_DDT_CONTABILIZZATO_EMESSO
                    Riferimento_DocAllegato = "- Rif. DDT n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
                    Flag_DDTallegati = True
                Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI
                    Riferimento_DocAllegato = "- Rif. Bolla n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
                    Flag_DDTallegati = True
                Case LAVCOD_DOCO_EMESSO, LAVCOD_DOCO_RICEVUTO
                    Riferimento_DocAllegato = "- Rif. DOCO n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
                Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO
                    Riferimento_DocAllegato = "- Rif. Ordine n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
                    Flag_ORDAllegati = True
                Case LAVCOD_MVV_EMESSO, LAVCOD_MVV_RICEVUTO
                    Riferimento_DocAllegato = "- Rif. MVV n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
                Case LAVCOD_DAA_EMESSO
                    Riferimento_DocAllegato = "- Rif. DAA n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
                Case Else
                    Riferimento_DocAllegato = "- Rif. doc n." & Num_Bolla_Sin & Num_Bolla & Num_Bolla_Des & " del " & Data_Bolla & ""
            End Select


            '04/02/2019: Scatto e Valerio ahnno deliberato di rimuovere questa parte (era stata fatta per SBTF)
            'If Flag_ORDAllegati = True Then

            '    '  Giulia, 02/11/2016 18.17.51: devo considerare solo i movimenti intercorsi tra l'ordine e questo DDT
            '    Dim Data_Movimento_FATT As Date = CDate(Dt_rif.Rows(0).Item("Data_Movimento_FATT"))

            '    Filtro = Filtro & vbCrLf &
            '        " AND Mov_Cont_FATT.Data_movimento >= " & Agro_SQL_SaveDate(Data_Bolla) & vbCrLf &
            '        " AND Mov_Cont_FATT.Data_movimento <= " & Agro_SQL_SaveDate(Data_Movimento_FATT) & vbCrLf


            '    Dim DT_Evasi As DataTable = objRif.Leggi_DDT_Nota_aggancio_Fattura(Piva,
            '                                                                       0,
            '                                                                       0,
            '                                                                       0,
            '                                                                       0,
            '                                                                       0,
            '                                                                       "",
            '                                                                       Piva,
            '                                                                       0,
            '                                                                       Id_Agenda_Bolla,
            '                                                                       Id_Mov_Bolla,
            '                                                                       Id_Mov_Det_Bolla,
            '                                                                       0, "",
            '                                                                       Filtro,
            '                                                                       "",
            '                                                                       objParametri_Server,
            '                                                                       True)

            '    If Not IsNothing(DT_Evasi) AndAlso DT_Evasi.Rows.Count > 0 Then

            '        Dim tot_evaso As Decimal = CDec(DT_Evasi.Rows(0).Item("Qta_Tot_Doc_Rif"))
            '        Dim parz_evaso As Decimal = 0

            '        For Each row As DataRow In DT_Evasi.Rows
            '            parz_evaso += CDec(row.Item("Qta"))
            '        Next

            '        Dettaglio_Evasione = "[Evasi " & parz_evaso & " su totale di " & tot_evaso & "]"

            '    End If

            'End If


        End If


        ''modifica del 04/10/2010
        ''il riferimento lo aggiungo dopo
        ''in base a se accorpo dettagli o meno
        'x_Descrizione &= Riferimento_DocAllegato

        Return Riferimento_DocAllegato

        ''Mi procuro il recordset richiesto
        'RsMov_Det_Riferimenti = objMov_Det_Riferimenti.Leggi( _
        '                        , _
        '                        , _
        '                        CLng(Agro_SQL_Load(RsMovimenti_Dettagli("Id_Agenda"))), _
        '                        CLng(Agro_SQL_Load(RsMovimenti_Dettagli("Id_Mov"))), _
        '                        CLng(Agro_SQL_Load(RsMovimenti_Dettagli("Id_Mov_Det"))), _
        '                        , _
        '                        , _

        'Dim xMov_Det_Riferimenti As XmlNodeList
        'Dim xMov_Det_Riferimento As XmlElement
        'Dim i_Dati_Riferimenti As Integer

        'If Not xMovimento_Dettaglio Is Nothing Then

        '    'Controllo che la fattura sia allegata a bolla
        '    If xMovimento_Dettaglio.GetAttribute("pendente") = enum_Pendenza.DocBolla Then

        '        xMov_Det_Riferimenti = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Riferimento2")

        '        i_Dati_Riferimenti = 0

        '        Do While i_Dati_Riferimenti < xMov_Det_Riferimenti.Count

        '            xMov_Det_Riferimento = xMov_Det_Riferimenti.Item(i_Dati_Riferimenti)

        '            'La fattura potrebbe essere allegata o refernziata a + movimenti --> filtro i Lav_Cod_Rif
        '            Select Case CInt(xMov_Det_Riferimento.GetAttribute("lav_cod_rif"))

        '                Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA,
        '                    LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI

        '                    Lav_Cod_Bolla = xMov_Det_Riferimento.GetAttribute("lav_cod_rif")
        '                    Piva_Bolla = xMov_Det_Riferimento.GetAttribute("piva_rif")
        '                    Sa_Cod_Bolla = xMov_Det_Riferimento.GetAttribute("sa_cod_rif")
        '                    Id_Agenda_Bolla = xMov_Det_Riferimento.GetAttribute("id_agenda_rif")
        '                    Id_Mov_Bolla = xMov_Det_Riferimento.GetAttribute("id_mov_rif")
        '                    Id_Mov_Det_Bolla = xMov_Det_Riferimento.GetAttribute("id_mov_det_rif")

        '                    Info_DocAllegato(objServer, objSession, objPage,
        '                                        Num_Bolla_Sin,
        '                                        Num_Bolla,
        '                                        Num_Bolla_Des,
        '                                        Data_Bolla,
        '                                        CStr(xMov_Det_Riferimento.GetAttribute("piva_rif")), _
        '                                        CInt(xMov_Det_Riferimento.GetAttribute("id_agenda_rif")))


        '                Case Else

        '                    'Il riferimento non è una bolla

        '                    'Do Nothing

        '            End Select

        '            i_Dati_Riferimenti = i_Dati_Riferimenti + 1


        '        Loop


        '    End If

        'end if


        objParametri_Server.ResettaFinestra()


    End Function


    '########################################################################################
    'Nota di accredito riferita a fattura: il riferimento è legato al movimento contabile
    Public Function Leggi_RiferimentoFattura_NEW(ByRef objParametri_Server As AgronicaCoreParametri, ByVal id_Agenda_NotaAccredito As Integer) As String

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim Dt_rif As DataTable
        Dim Filtro As String
        Dim Riferimento_DocAllegato As String = ""
        Dim Lav_Cod_Fattura As Integer = 0
        'Dim Piva_Fattura As String
        'Dim Sa_Cod_Fattura As Integer
        'Dim Id_Agenda_Fattura As Integer
        Dim Num_Fattura_Sin As String = ""
        Dim Num_Fattura As String = ""
        Dim Num_Fattura_Des As String = ""
        Dim Data_Fattura As String = ""

        Filtro = "  ( lav_cod_rif = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR lav_cod_rif= " & CStr(LAVCOD_FATTURA_RICEVUTA) & ") "

        'Dt_rif = objRif.Leggi_Con_Movimenti("", 0, _
        '                                    id_Agenda_NotaAccredito, _
        '                                    -1, _
        '                                    -1, _
        '                                    0, _
        '                                    CAU_REGISTRAZIONI, _
        '                                    Filtro, "", _
        '                                    objParametri_Server)

        Dt_rif = objRif.Leggi_DDT_Nota_aggancio_Fattura( _
                                    "", 0, _
                                    id_Agenda_NotaAccredito, _
                                    -1, _
                                    -1, _
                                    LAVCOD_NOTA_ACCREDITO_EMESSA, _
                                    CAU_REGISTRAZIONI, _
                                    "", _
                                    0, _
                                    0, _
                                    -1, -1, _
                                    0, _
                                    CAU_REGISTRAZIONI, _
                                    Filtro, "", _
                                    objParametri_Server)

        If Not IsNothing(Dt_rif) AndAlso Dt_rif.Rows.Count > 0 Then
            Lav_Cod_Fattura = Dt_rif.Rows(0).Item("lav_cod_rif")
            Num_Fattura_Sin = Dt_rif.Rows(0).Item("Doc_Numero_Sin")
            Num_Fattura = Dt_rif.Rows(0).Item("Doc_Numero")
            Num_Fattura_Des = Dt_rif.Rows(0).Item("Doc_Numero_Des")
            Data_Fattura = Dt_rif.Rows(0).Item("Data_movimento")
        End If

        If Lav_Cod_Fattura <> 0 Then
            Riferimento_DocAllegato = " (Rif. Fattura n." & Num_Fattura_Sin & Num_Fattura & Num_Fattura_Des & " del " & Data_Fattura & ")"
        End If

        Return Riferimento_DocAllegato


        'Dim XML_MovimentoRiferimento As XmlElement

        'If Not IsNothing(XML_Movimento) Then

        'XML_MovimentoRiferimento = XML_Movimento.SelectSingleNode("Movimento_Riferimento")

        'If Not IsNothing(XML_MovimentoRiferimento) Then

        'Mi procuro un elenco dei riferimenti del Movimento

        'Select Case Cau_Mov

        '    Case CAU_CARICO, CAU_SCARICO, _
        '            CAU_CONFERIMENTO, CAU_CONFERIMENTO_DIVERSI, _
        '            CAU_ACCETTAZIONE_BENI, CAU_ACCETTAZIONE_BENI_DA_DIVERSI, _
        '            CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TERZISTI, _
        '            CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI, CAU_IMPUTAZIONE_PARCOMACCHINE

        '        'Escludo i movimenti collaterali a quello base
        '        '(carico, scarico, conferimenti, accettazioni, imputazione costi accessori)

        '    Case Else

        'RsMovimenti_Riferimenti = ObjMovimenti_Riferimenti.Leggi( _
        '        , _
        '        , _
        '        CLng(Agro_SQL_Load(RsMovimenti("Id_Agenda"))), _
        '        -1, _
        '        -1, _
        '        , _
        '        , _
        '        2, _
        '        objCnManager, _
        '        FinestraTemp_Inizio, _
        '        FinestraTemp_Fine, _
        '        ConnessioneAlternativa)

        'Leggi(Optional ByVal PIVA As String, _
        '      Optional ByVal Sa_Cod As Long, _
        '      Optional ByVal Id_Agenda As Long, _
        '      Optional ByVal Id_Mov As Long, _
        '      Optional ByVal Id_Mov_Det As Long, _
        '      Optional ByVal Lav_Cod As Long, _
        '      Optional ByVal Cau_Mov As String, _
        '      Optional ByVal TipoOutput As Long = 2, _
        '      Optional ByRef objCnManager As Object, _
        '      Optional ByVal FinestraTemp_Inizio As Date = CDate("01/01/1900"), _
        '      Optional ByVal FinestraTemp_Fine As Date = CDate("31/12/2100"), _
        '      Optional ByVal ConnessioneAlternativa As String = "" _
        '      ) As Variant


        'If CInt(XML_MovimentoRiferimento.GetAttribute("lav_cod_rif")) = LAVCOD_FATTURA_EMESSA Or _
        '    CInt(XML_MovimentoRiferimento.GetAttribute("lav_cod_rif")) = LAVCOD_FATTURA_RICEVUTA Then

        '    Lav_Cod_Fattura = XML_MovimentoRiferimento.GetAttribute("lav_cod_rif")
        '    Piva_Fattura = XML_MovimentoRiferimento.GetAttribute("piva_rif")
        '    Sa_Cod_Fattura = XML_MovimentoRiferimento.GetAttribute("sa_cod_rif")
        '    Id_Agenda_Fattura = XML_MovimentoRiferimento.GetAttribute("id_agenda_rif")

        '    Info_DocAllegato(objServer, objSession, objPage, _
        '                        Num_Fattura_Sin, _
        '                        Num_Fattura, _
        '                        Num_Fattura_Des, _
        '                        Data_Fattura, _
        '                        CStr(XML_MovimentoRiferimento.GetAttribute("piva_rif")), _
        '                        CInt(XML_MovimentoRiferimento.GetAttribute("id_agenda_rif")))



        'End If

        'End If

        'End If

        ' End Select


        objParametri_Server.ResettaFinestra()


    End Function



    ''#####################################################################################################
    'Public Function Leggi_Rubrica_OLD(ByRef objServer As System.Web.HttpServerUtility, _
    '                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                ByRef objPage As System.Web.UI.Page, _
    '                                ByVal Piva_Contatto As String, _
    '                                ByVal Cod_Contatto As String, _
    '                                ByVal Cod_RisUm As Integer, _
    '                                ByVal Sa_Cod As Integer, _
    '                                ByVal Cod_Rubrica As Integer) As DataTable

    '    Dim DT As DataTable
    '    Dim Str_FiltroDescr As String = ""

    '    If Cod_Rubrica = 0 Then
    '        Str_FiltroDescr = "TEL"
    '    End If

    '    Select Case VerificaEsistenza_PivaGIAS(objServer, objSession, objPage, Cod_Contatto)

    '        Case True

    '            'Cod_Contatto è la piva dell'impresa
    '            DT = NewCom_CentriAziendali_Rubrica_Leggi(objServer, objSession, objPage, _
    '                                                     Cod_Contatto, _
    '                                                     Sa_Cod, _
    '                                                     Cod_Rubrica, _
    '                                                     Str_FiltroDescr)

    '        Case False

    '            DT = NewCom_Contatti_Rubrica_Leggi(objServer, objSession, objPage, _
    '                                              Piva_Contatto, _
    '                                              Cod_Contatto, _
    '                                              Cod_RisUm, _
    '                                              Cod_Rubrica, _
    '                                              Str_FiltroDescr)


    '    End Select

    '    Return DT


    'End Function


    ''#####################################################################################################
    'Public Sub _Leggi_Rubrica(ByRef objServer As System.Web.HttpServerUtility, _
    '                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                    ByRef objPage As System.Web.UI.Page, _
    '                                    ByVal Cod_Contatto As String, _
    '                                    ByVal Cod_RisUm As Integer, _
    '                                    ByVal Sa_Cod As Integer, _
    '                                    ByVal Cod_Rubrica As Integer, _
    '                                    ByRef x_Numero As String)

    '    Dim DT_Rubrica As DataTable
    '    Dim Str_FiltroDescr As String = ""

    '    If Cod_Rubrica = 0 Then
    '        Str_FiltroDescr = "TEL"
    '    End If

    '    Select Case VerificaEsistenza_PivaGIAS(objServer, objSession, objPage, Cod_Contatto)

    '        Case True

    '            'Cod_Contatto è la piva dell'impresa
    '            DT_Rubrica = NewCom_CentriAziendali_Rubrica_Leggi(objServer, objSession, objPage, _
    '                                                            Cod_Contatto, _
    '                                                            Sa_Cod, _
    '                                                            Cod_Rubrica, _
    '                                                            Str_FiltroDescr)

    '        Case False

    '            DT_Rubrica = NewCom_Contatti_Rubrica_Leggi(objServer, objSession, objPage, _
    '                                                        , _
    '                                                        , _
    '                                                        Cod_RisUm, _
    '                                                        Cod_Rubrica, _
    '                                                        Str_FiltroDescr)


    '    End Select

    '    If Not IsNothing(DT_Rubrica) Then

    '        If DT_Rubrica.Rows.Count <> 0 Then

    '            x_Numero = DT_Rubrica.Rows(0).Item("Numero")

    '        End If

    '    End If

    'End Sub


    ''#####################################################################################################
    'Public Sub Leggi_Indirizzi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                            ByRef objServer As System.Web.HttpServerUtility, _
    '                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                            ByRef objPage As System.Web.UI.Page, _
    '                            ByVal Cod_Contatto As String, _
    '                            ByVal Cod_RisUm As Integer, _
    '                            ByVal Cod_Indirizzo As Integer, _
    '                            ByRef x_Ind_Des As String, _
    '                            ByRef x_Frz_Des As String, _
    '                            ByRef x_Cap As String, _
    '                            ByRef x_Comune As String, _
    '                            ByRef x_Provincia As String, _
    '                            ByRef x_Stato As String, _
    '                            ByRef Flag_StatoMembro As Boolean)

    '    Leggi_Indirizzi(objParametri_Server, _
    '                    Cod_Contatto, _
    '                    Cod_RisUm, _
    '                    Cod_Indirizzo, _
    '                    x_Ind_Des, _
    '                    x_Frz_Des, _
    '                    x_Cap, _
    '                    x_Comune, _
    '                    x_Provincia, _
    '                    x_Stato, _
    '                    Flag_StatoMembro)

    'End Sub


    '#####################################################################################################
    Public Sub Leggi_Indirizzi(ByRef objParametri_Server As AgronicaCoreParametri,
                               ByVal Cod_Contatto As String,
                               ByVal Cod_RisUm As Integer,
                               ByVal Cod_Indirizzo As Integer,
                               ByRef x_Ind_Des As String,
                               ByRef x_Frz_Des As String,
                               ByRef x_Cap As String,
                               ByRef x_Comune As String,
                               ByRef x_Provincia As String,
                               ByRef x_Stato As String,
                               ByRef Flag_StampaCodiceVAT As Boolean, _
                               ByRef x_IndirizzoTipoDesc As String, _
                               ByVal Cod_Contatto_Cliente As String)

        Dim DT_Indirizzi As DataTable = Nothing
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim IsPivaGias As Boolean

        'VerificaEsistenza_PivaGIAS(objServer, objSession, objPage, Cod_Contatto)
        IsPivaGias = objImprese.VerificaEsistenza_PivaGIAS(Cod_Contatto, objParametri_Server)

        Select Case IsPivaGias

            Case True

                Dim objInd As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                DT_Indirizzi = objInd.Leggi2(Cod_Contatto,
                                            True,
                                            Cod_Indirizzo,
                                            0,
                                            "", "",
                                            objParametri_Server)

                '  Giulia, 25/10/2016 12.53.27: Se non non ho trovato l'indirizzo in ImpresexIndirizzi
                '           vuol dire che può essere un'idirizzo alternativo (quello di un centro)
                If Not IsNothing(DT_Indirizzi) AndAlso DT_Indirizzi.Rows.Count = 0 Then
                    Dim objCentriInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

                    DT_Indirizzi = objCentriInd.Leggi(Cod_Contatto, 0, Cod_Indirizzo, 0,
                                                      enumSelezioneVariabile.Selezione_JoinCompleta,
                                                      "", "",
                                                      objParametri_Server)

                End If

                'DT_Indirizzi = NewCom_Imprese_Leggi(objServer, objSession, objPage, _
                '                                    Cod_Contatto, _
                '                                    "", _
                '                                    True, _
                '                                    , _
                '                                    Cod_Indirizzo)
            Case False

                Dim objInd As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
                DT_Indirizzi = objInd.Leggi2("",
                                                "",
                                                Cod_Indirizzo,
                                                0,
                                                Cod_RisUm,
                                                "", "",
                                                objParametri_Server)

                'DT_Indirizzi = NewCom_Contatti_Indirizzi_Leggi(objServer, objSession, objPage, _
                '                                                , _
                '                                                , _
                '                                                Cod_RisUm, _
                '                                                , _
                '                                                Cod_Indirizzo)
        End Select


        If Not IsNothing(DT_Indirizzi) Then

            If DT_Indirizzi.Rows.Count <> 0 Then

                x_Ind_Des = DT_Indirizzi.Rows(0).Item("Ind_Des")
                x_Frz_Des = DT_Indirizzi.Rows(0).Item("Frz_Des")
                x_Cap = DT_Indirizzi.Rows(0).Item("Cap")
                x_Comune = DT_Indirizzi.Rows(0).Item("Com_Des")
                'x_Provincia = "(" & DT_Indirizzi.Rows(0).Item("Pro_Cod") & ")"
                x_Provincia = DT_Indirizzi.Rows(0).Item("Pro_Cod")
                x_Stato = DT_Indirizzi.Rows(0).Item("Stato")

                '22/01/2019: questo campo viene letto solo dalla query dei contatti
                If DT_Indirizzi.Columns.Contains("IndirizzoTipoDesc") = True Then
                    '17/10/19: aggiunto controllo <> ""
                    If DT_Indirizzi.Rows(0).Item("IndirizzoTipoDesc") <> "" Then
                        If Cod_Contatto_Cliente <> Cod_Contatto Then
                            'è il caso di contatto diverso (dal contatto cliente) usato nella destinazione
                            'oltre al tipo indirizzo devo stampare anche rag_soc del contatto
                            '(ci entra anche nei casi in cui viene passato cod_contatto_cliente = "", ma sono casi in cui poi la pagina non usa x_IndirizzoTipoDesc)
                            '17/10/19: aggiunto anche nome e cognome
                            x_IndirizzoTipoDesc = Trim(DT_Indirizzi.Rows(0).Item("Rag_Soc_Contatto") & " " & DT_Indirizzi.Rows(0).Item("Cognome") & " " & DT_Indirizzi.Rows(0).Item("Nome")) & _
                                                " - " & DT_Indirizzi.Rows(0).Item("IndirizzoTipoDesc")
                        Else
                            x_IndirizzoTipoDesc = DT_Indirizzi.Rows(0).Item("IndirizzoTipoDesc")
                        End If
                    Else
                        x_IndirizzoTipoDesc = ""
                    End If
                Else
                    x_IndirizzoTipoDesc = ""
                End If

            End If

        End If

        Sistema_Comune_Provincia(x_Comune, x_Provincia)

        '05/11/2019: ora la stampa del codice vat va in base al ChkFittizio
        'Select Case x_Stato.ToUpper

        '    Case "SM", "SAN MARINO"
        '        Flag_StampaCodiceVAT = True

        '    Case Else
        '        Dim objDAA As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T004_TabellaCodiciStatiMembri_R
        '        Flag_StampaCodiceVAT = objDAA.Stato_In_StatiMembri(x_Stato, objParametri_Server)

        'End Select

    End Sub

    '#####################################################################################################
    Public Sub Leggi_Rubrica(ByRef objParametri_Server As AgronicaCoreParametri,
                             ByVal Cod_Contatto As String,
                             ByVal Cod_RisUm As Integer,
                             ByVal Cod_Rubrica As Integer,
                             ByRef x_Tel As String,
                             ByRef x_Cell As String)

        Dim DT As DataTable
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim IsPivaGias As Boolean

        'VerificaEsistenza_PivaGIAS(objServer, objSession, objPage, Cod_Contatto)
        IsPivaGias = objImprese.VerificaEsistenza_PivaGIAS(Cod_Contatto, objParametri_Server)

        Select Case IsPivaGias

            Case True

                Dim objInd As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
                x_Tel = objInd.Recupera_Telefono_Centro(Cod_Contatto, _
                                                     0, _
                                                        objParametri_Server)
                x_Cell = ""

            Case False

                Dim objInd As New AgronicaCoreAnagrafeDAL.ContattixRubrica_R
                DT = objInd.LeggiByCod_Risum(Cod_RisUm, _
                                             "", "", _
                                                " ( Rubrica.Descr LIKE '%tel%' OR Rubrica.Descr LIKE '%cel%' ) ", _
                                                " Rubrica.Descr DESC ", _
                                                objParametri_Server)

                If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then
                    Dim i As Integer
                    For i = 0 To DT.Rows.Count - 1
                        If InStr(CStr(DT.Rows(i).Item("Descr")).ToLower, "tel") > 0 Then
                            x_Tel = DT.Rows(i).Item("Numero")
                        End If
                        If InStr(CStr(DT.Rows(i).Item("Descr")).ToLower, "cel") > 0 Then
                            x_Cell = DT.Rows(i).Item("Numero")
                        End If
                    Next
                End If


        End Select




    End Sub


    '#####################################################################################################
    Public Sub Leggi_CodiciIntestazioneImpresa(ByVal Piva As String,
                                               ByRef RegImprese As String,
                                               ByRef REA As String,
                                               ByRef ISO As String,
                                               ByRef AlboCoop As String,
                                               ByRef objParametri As AgronicaCoreParametri)


        Dim DT_Codici As DataTable
        Dim i As Integer
        Dim Id_Cod As Integer

        Dim objImprCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        DT_Codici = objImprCodici.Imprese_CodiciIntestazioneFattura_Leggi(Piva, "", "", objParametri)

        ' DT_Codici = NewCom_Imprese_CodiciIntestazioneFattura_Leggi(objServer, objSession, objPage, _
        '                                                                Piva, _
        '                                                                 )


        RegImprese = ""
        REA = ""
        ISO = ""
        AlboCoop = ""

        If Not IsNothing(DT_Codici) Then

            For i = 0 To DT_Codici.Rows.Count - 1

                Id_Cod = DT_Codici.Rows(i).Item("Id_Cod")

                Select Case Id_Cod

                    Case 1111 'Codice ISO
                        ISO = DT_Codici.Rows(i).Item("val_Cod")

                    Case 1112 'Codice R.E.A.
                        REA = DT_Codici.Rows(i).Item("val_Cod")

                    Case 1113 'N. Iscrizione all'Albo delle Soc. Coop. a Mutualità Prevalente
                        AlboCoop = DT_Codici.Rows(i).Item("val_Cod")

                    Case 1119 'N. Registro Imprese
                        RegImprese = DT_Codici.Rows(i).Item("val_Cod")

                End Select

            Next


        End If


    End Sub

    '#####################################################################################################
    Public Sub Leggi_Personalizzazioni_Doc_Contabili(ByRef CodRisUm_Pers1 As String(), _
                                                     ByRef objParametri As AgronicaCoreParametri)

        Dim Pers1 As String
        Dim Separator() As String = {"|"}

        '  Giulia, 10/01/2017 12.27.01: la personalizzazione ora si trova in Configurazione_Stampe, non più in Configurazione_Siti
        Dim objConfSiti As New Configurazione_Siti_R
        Pers1 = objConfSiti.Recupera_Valore_ByChiave(6, "CodRisUm_Personalizzazione_Doc_Contabili_1", objParametri)


        If Not String.IsNullOrEmpty(Pers1) Then
            CodRisUm_Pers1 = Pers1.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
        End If

    End Sub

    '#####################################################################################################
    Public Sub Ricava_Piva_Codicefiscale(ByVal ID_CF As Integer, _
                                         ByVal Cod_Contatto As String,
                                         ByVal Codice_Fiscale As String,
                                         ByVal chk_fittizio As Integer,
                                         ByRef Piva As String,
                                         ByRef CodiceFiscale As String,
                                         ByRef Flag_PersonaPrivato As Boolean)

        Dim Flag_Verifica As Boolean = False
        Flag_PersonaPrivato = False

        If chk_fittizio = 1 Then
            'se è stato salvato appositamente fittizio -> non stampo i codici
            Piva = ""
            CodiceFiscale = ""
        Else
            'CASO CONTATTO GENERICO DEL GIASLAN
            'controllare che in caso di cod_contatto numerico e minore di 0 
            '(2 istruzioni distinte!!) 
            'non venga inserito nella stampa il cod_contatto
            'e neanche il campo codice fiscale (stringa vuota in questo caso)
            If IsNumeric(Cod_Contatto) = True Then
                If CDbl(Cod_Contatto) < 0 Then
                    Flag_PersonaPrivato = True
                    Piva = ""
                    CodiceFiscale = ""
                Else
                    Flag_Verifica = True
                End If
            Else
                If Cod_Contatto.Length = 11 AndAlso Cod_Contatto.StartsWith("F") AndAlso (IsNumeric(Mid(Cod_Contatto, 2, Cod_Contatto.Length - 1))) Then
                    'CONTATTO FITTIZIO SALVATO DAL GIASONLINE es: F1999998632
                    Piva = ""
                    CodiceFiscale = ""
                Else
                    Flag_Verifica = True
                End If
            End If

            If Flag_Verifica = True Then

                'se e' una partita iva tarocca (prima cifra una lettera e le altre 10 numeri)
                'sono nel caso di un'impresa multi-attività
                If Cod_Contatto.Length = 11 AndAlso ((Not IsNumeric(Left(Cod_Contatto, 1))) AndAlso (IsNumeric(Right(Cod_Contatto, 10)))) Then
                    'era stato fatto per la Fattoria il Monte (Donatella Cena) per evitare di sviluppare cose per lei
                    Piva = "0" & Right(Cod_Contatto, 10)
                    CodiceFiscale = Codice_Fiscale
                Else

                    '14/03/2019: Revisione per poter stampare su Consorzio per la Bonifica Capitanata (cliente Agronica) 00345000715 come codice fiscale e non come piva

                    Select Case ID_CF

                        Case PERSONA_FISICA
                            Piva = ""
                            CodiceFiscale = Cod_Contatto
                            Flag_PersonaPrivato = True

                        Case PERSONA_GIURIDICA

                            If Cod_Contatto.StartsWith("8") Or Cod_Contatto.StartsWith("9") Then
                                'devo impostare   Flag_PersonaPrivato = True per le associazioni?
                                Piva = ""
                                CodiceFiscale = Cod_Contatto
                            Else
                                Piva = Cod_Contatto
                                CodiceFiscale = Codice_Fiscale
                            End If

                        Case CONTATTO_ESTERO
                            'per gli esteri tratto come le p.g.
                            'la stampa di fattura/ordine/ddt/ndc fa un filtro prima sugli esteri e qui non ci entra
                            Piva = Cod_Contatto
                            CodiceFiscale = Codice_Fiscale
                    End Select

                    ''sono in un caso normale
                    'If VerificaEspressioneRegolare(Cod_Contatto, "", enum_EspressioniRegolari.RegExp_PartitaIVA) = True Then
                    '    Piva = Cod_Contatto
                    '    ' Giulia: 11/10/2017: si tratta di un'associazione che potrebbe essere stata memorizzata come persona fisica, 
                    '    '       quindi deve comparire nel codice fiscale
                    '    If Cod_Contatto.StartsWith("8") Or Cod_Contatto.StartsWith("9") Then
                    '        CodiceFiscale = Cod_Contatto
                    '    Else
                    '        CodiceFiscale = Codice_Fiscale
                    '    End If
                    'Else
                    '    Flag_PersonaPrivato = True
                    '    Piva = ""
                    '    CodiceFiscale = Cod_Contatto
                    'End If

                End If

            End If


        End If

    End Sub

    '#####################################################################################################
    'usata da:
    '- scheda materie prime bio
    '- scheda vendite bio
    '- bolla accettazione che non è in uso (Fruttagel usa la versione delle stampe 2003)
    '- bolla FF
    Public Sub Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(ByVal Cod_Contatto As String,
                                                         ByVal Codice_Fiscale As String,
                                                         ByRef Piva As String,
                                                         ByRef CodiceFiscale As String,
                                                         ByRef Flag_PersonaPrivato As Boolean)

        Dim Flag_Verifica As Boolean = False
        Flag_PersonaPrivato = False

        'controllare che in caso di cod_contatto numerico e minore di 0 
        '(2 istruzioni distinte!!) 
        'non venga inserito nella stampa il cod_contatto
        'e neanche il campo codice fiscale (stringa vuota in questo caso)
        If IsNumeric(Cod_Contatto) = True Then
            If CDbl(Cod_Contatto) < 0 Then
                Flag_PersonaPrivato = True
                Piva = ""
                CodiceFiscale = ""
            Else
                Flag_Verifica = True
            End If
        Else
            Flag_Verifica = True
        End If

        If Flag_Verifica = True Then

            'se e' una partita iva tarocca (prima cifra una lettera e le altre 10 numeri)
            'sono nel caso di un'impresa multi-attività
            If Cod_Contatto.Length = 11 AndAlso ((Not IsNumeric(Left(Cod_Contatto, 1))) And (IsNumeric(Right(Cod_Contatto, 10)))) Then
                Piva = "0" & Right(Cod_Contatto, 10)
                CodiceFiscale = Codice_Fiscale
            Else
                'sono in un caso normale
                If VerificaEspressioneRegolare(Cod_Contatto, "", enum_EspressioniRegolari.RegExp_PartitaIVA) = True Then
                    Piva = Cod_Contatto

                    ' Giulia: 11/10/2017: si tratta di un'associazione che potrebbe essere stata memorizzata come persona fisica, 
                    '       quindi deve comparire nel codice fiscale
                    If Cod_Contatto.StartsWith("8") Or Cod_Contatto.StartsWith("9") Then
                        CodiceFiscale = Cod_Contatto
                    Else
                        CodiceFiscale = Codice_Fiscale
                    End If
                Else
                    Flag_PersonaPrivato = True
                    Piva = ""
                    CodiceFiscale = Cod_Contatto
                End If
            End If

        End If

    End Sub


    '#####################################################################################################
    Public Sub Sistema_Comune_Provincia(ByRef Localita As String, _
                                        ByRef Comuni_Prov As String)

        If Localita.ToLower = "non definita" OrElse Localita.ToLower = "not defined" Then
            Localita = ""
        End If

        If Comuni_Prov.ToLower = "00" OrElse Comuni_Prov.ToLower = "0" Then
            Comuni_Prov = ""
        End If

    End Sub

    '#####################################################################################################
    Public Sub Sistema_Provincia_RegImprese(ByRef Provincia As String)

        If Provincia.ToLower = "non definita" OrElse Provincia.ToLower = "not defined" Then
            Provincia = ""
        End If

    End Sub


    '#####################################################################################################
    'spostato sul core AgronicaCoreContabHLP.Contabilita
    Public Function Ricava_NumeroDocumento_Con_Sequenza(ByVal Doc_Numero_Sin As String, _
                                                        ByVal Doc_Numero As Decimal, _
                                                        ByVal Doc_Numero_Des As String, _
                                                        ByVal Lunghezza_Sin As Integer, _
                                                        ByVal Lunghezza_Centro As Integer, _
                                                        ByVal Lunghezza_Des As Integer, _
                                                        ByVal CarattereFormattazione As String) As String

        Dim j As Integer
        Dim Stringa_Vuota_Sin As String = ""
        Dim Stringa_Vuota_Centro As String = ""
        Dim Stringa_Vuota_Des As String = ""
        Dim Numero_Completo As String
        Dim Format_Numero_Sin As String
        Dim Format_Numero_Centro As String
        Dim Format_Numero_Des As String

        For j = 0 To CInt(Lunghezza_Sin) - 1
            Stringa_Vuota_Sin &= CStr(CarattereFormattazione)
        Next

        For j = 0 To CInt(Lunghezza_Centro) - 1
            Stringa_Vuota_Centro &= CStr(CarattereFormattazione)
        Next

        For j = 0 To CInt(Lunghezza_Des) - 1
            Stringa_Vuota_Des &= CStr(CarattereFormattazione)
        Next

        Format_Numero_Sin = Right(Stringa_Vuota_Sin & Doc_Numero_Sin, Lunghezza_Sin)
        Format_Numero_Centro = Right(Stringa_Vuota_Centro & CStr(Doc_Numero), Lunghezza_Centro)
        Format_Numero_Des = Right(Stringa_Vuota_Des & Doc_Numero_Des, Lunghezza_Des)

        Numero_Completo = CStr(Format_Numero_Sin & Format_Numero_Centro & Format_Numero_Des)

        Return Numero_Completo

    End Function

    '#####################################################################################################
    'spostato sul core AgronicaCoreContabHLP.Contabilita
    Public Function Ricava_NumeroDocumento_Senza_Sequenza(ByVal Doc_Numero_Sin As String,
                                                          ByVal Doc_Numero As Decimal,
                                                          ByVal Doc_Numero_Des As String) As String


        Dim Numero_Completo As String

        Numero_Completo = Doc_Numero_Sin & CStr(Doc_Numero) & Doc_Numero_Des

        Return Numero_Completo


    End Function

    '#####################################################################################################
    '  'spostato sul core AgronicaCoreContabHLP.Contabilita
    Public Function Calcola_PesoLordo(ByVal Tipo_Peso As Integer, ByVal Peso As Decimal, ByVal Tara_Imballi As Decimal) As Decimal

        Dim Peso_Lordo As Decimal

        Select Case Tipo_Peso
            Case 0 'lordo
                Peso_Lordo = Peso
            Case 1 'netto
                Peso_Lordo = Peso + Tara_Imballi
        End Select

        Return Peso_Lordo

    End Function

    '#####################################################################################################
    'spostato sul core AgronicaCoreContabHLP.Contabilita
    Public Function Calcola_PesoNetto(ByVal Tipo_Peso As Integer, ByVal Peso As Decimal, ByVal Tara_Imballi As Decimal) As Decimal

        Dim Peso_Netto As Decimal

        Select Case Tipo_Peso
            Case 0 'lordo
                Peso_Netto = Peso - Tara_Imballi
            Case 1 'netto
                Peso_Netto = Peso
        End Select

        Return Peso_Netto

    End Function

    '#####################################################################################################
    Public Function Composizione_Stringa_Sconti(ByVal x_Sconto_Perc As Decimal,
                                                ByVal x_Sconto_Perc_2 As Decimal,
                                                ByVal x_Sconto_Testo As String
                                                ) As String

        Dim Riga_Sconto As String = ""

        'modifica del 17/09/2012: lo sconto ha già il segno -

        'primo sconto
        If x_Sconto_Perc <> 0 Then
            'Riga_Sconto = "-" & CStr(x_Sconto_Perc)
            Riga_Sconto = CStr(x_Sconto_Perc)
        Else
            Riga_Sconto = "" 'vanni, 02/09/2011, baco su riga fattura con sconto = 0
        End If

        If Trim(x_Sconto_Testo) = "" Then
            'sconto del listino
            If x_Sconto_Perc_2 <> 0 Then
                'Riga_Sconto &= " -" & CStr(x_Sconto_Perc_2)
                Riga_Sconto &= CStr(x_Sconto_Perc_2)
            End If
        Else
            'sconti aggiuntivi
            Riga_Sconto &= " -" & CStr(x_Sconto_Testo)
        End If

        If Riga_Sconto <> "" Then
            Riga_Sconto &= "%"
        End If

        Return Riga_Sconto

    End Function


    '############################################################################
    Public Sub Carica_LogoDocumento(ByRef Log_Errori As String,
                                    ByRef Dr_Documento As DataSetFattura.IntestazioneFatturaRow,
                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                    ByVal Codice_Report As enum_CodificaStampe,
                                    ByVal Piva As String,
                                    ByRef Flag_LogoInAlto As Boolean)

        Try

            'di default inserisco un'imamgine bianca come logo
            Dim bmpImg As Drawing.Bitmap = New Drawing.Bitmap(1, 1)
            bmpImg.SetPixel(0, 0, Drawing.Color.White)

            Dim cx As New Drawing.ImageConverter
            Dim logoBianco() As Byte
            logoBianco = cx.ConvertTo(bmpImg, GetType(Byte()))

            Dr_Documento.Item("Logo") = logoBianco
            Dr_Documento.Item("LogoInAlto") = logoBianco

            '====================================================

            Dim NomeLogo, path As String
            Dim NomeLogoInAlto As String = ""

            Select Case Codice_Report
                Case enum_CodificaStampe.DDT_Contabilizzato_Emesso, _
                        enum_CodificaStampe.Fatture, _
                            enum_CodificaStampe.Nota_Accredito, _
                                enum_CodificaStampe.Ordine, _
                                enum_CodificaStampe.Preventivo_Vendita
                    NomeLogo = Piva & "_LogoFatturaNotaOrdine.jpg"
                    NomeLogoInAlto = Piva & "_LogoFatturaNotaOrdine_InAlto.jpg"
                    '----------------------
                Case enum_CodificaStampe.Bolle, _
                        enum_CodificaStampe.Bolle_Conferimento_Soci, _
                           enum_CodificaStampe.Bolle_Conferimento_Diversi
                    NomeLogo = Piva & "_LogoBolle.jpg"
                    NomeLogoInAlto = Piva & "_LogoBolle_InAlto.jpg"
                    '-----------------------
                Case enum_CodificaStampe.Buono_Accettazione_Diversi
                    NomeLogo = Piva & "_LogoBollaAccettazione.jpg"
                    '-----------------------
                Case Else
                    NomeLogo = ""
            End Select

            If NomeLogo <> "" Then

                Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

                If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                    Try

                        If objWebConfig.Path_Directory_Loghi_Cliente.EndsWith("\") = False Then
                            objWebConfig.Path_Directory_Loghi_Cliente &= "\"
                        End If

                        path = objWebConfig.Path_Directory_Loghi_Cliente & NomeLogo

                        If IO.File.Exists(path) = True Then
                            Flag_LogoInAlto = False
                            Dim bmpF As Drawing.Bitmap = New Drawing.Bitmap(path)
                            Dim logo() As Byte
                            Dim c As New Drawing.ImageConverter
                            logo = c.ConvertTo(bmpF, GetType(Byte()))
                            Dr_Documento.Item("Logo") = logo
                        Else

                            path = objWebConfig.Path_Directory_Loghi_Cliente & NomeLogoInAlto
                            If IO.File.Exists(path) = True Then
                                Flag_LogoInAlto = True
                                Dim bmpF As Drawing.Bitmap = New Drawing.Bitmap(path)
                                Dim logo() As Byte
                                Dim c As New Drawing.ImageConverter
                                logo = c.ConvertTo(bmpF, GetType(Byte()))
                                Dr_Documento.Item("LogoInAlto") = logo
                            End If

                        End If

                    Catch ex As Exception
                        Log_Errori &= "Carica_Logo. Caricamento logo cliente errore: " & ex.Message
                    End Try
                Else
                    Log_Errori &= "Carica_Logo.Percorso del logo non impostato."
                End If 'path
            Else
                Log_Errori &= "Carica_Logo. Logo non gestito per questo tipo di stampa."
            End If 'nome logo

        Catch ex As Exception
            Log_Errori &= "Carica_Logo. Errore: " & ex.Message
        End Try

    End Sub

    '#####################################################################################################
    Public Sub ImpostazioniUtente_StampaDoc(ByRef objParametriUtenti As AgronicaCoreParametri,
                                            ByRef logErrori As String,
                                            ByVal lavCod As Integer,
                                            ByRef strNoteIntegrative1 As String,
                                            ByRef strNoteIntegrative2 As String,
                                            ByRef strNoteIntegrative3 As String,
                                            ByRef strArticolo62 As String,
                                            ByRef flagContributoConai As Boolean,
                                            ByRef flagStampaRifOrdine As Boolean,
                                            ByRef flagGradoAlcolico As Boolean,
                                            ByRef FlagGestMaterialeVivaistico As Boolean,
                                            ByRef Flag_CentroAziendalePartenza As Boolean)

        Dim dt As DataTable
        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim valore As String

        Try

            strNoteIntegrative1 = ""
            strNoteIntegrative2 = ""
            strNoteIntegrative3 = ""
            strArticolo62 = "Assolve agli obblighi dell'Art. 3 del D.Lgs 198/2021 e s.m.i. Il contratto ha durata per la presente consegna."
            flagContributoConai = False
            flagStampaRifOrdine = False
            flagGradoAlcolico = False


            Dim impLista As New List(Of String) From {
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1DDT),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2DDT),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3DDT),
                CStr(enum_Impostazioni_Utenti.SuperUser_Articolo62DDT),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1DDTCorrispettivi),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2DDTCorrispettivi),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3DDTCorrispettivi),
                CStr(enum_Impostazioni_Utenti.SuperUser_Articolo62DDTCorrispettivi),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1Ordini),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2Ordini),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3Ordini),
                CStr(enum_Impostazioni_Utenti.SuperUser_Articolo62Ordini),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1Fatture),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2Fatture),
                CStr(enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3Fatture),
                CStr(enum_Impostazioni_Utenti.SuperUser_Articolo62Fatture),
                CStr(enum_Impostazioni_Utenti.SuperUser_ContributoConai),
                CStr(enum_Impostazioni_Utenti.SuperUser_StRifOrdine),
                CStr(enum_Impostazioni_Utenti.SUPERUSER_StampaAlcolTotale),
                CStr(enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO),
                CStr(enum_Impostazioni_Utenti.SUPERUSER_INTESTAZIONE_STAMPA_CENTROAZIENDALEPARTENZA)
            }


            Dim impString As String = String.Join(", ", impLista)

            Dim filtro As String = " ( Impostazione_Cod IN ( " & impString & " ) )"

            dt = objImpost.Leggi(0, 2,
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 filtro, "",
                                 objParametriUtenti)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows

                    valore = dr.Item("Impostazione_Valore_1")

                    Select Case lavCod

                        Case LAVCOD_BOLLA_EMESSA, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_CONFERIMENTO

                            Select Case dr.Item("Impostazione_Cod")
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1DDT
                                    strNoteIntegrative1 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2DDT
                                    strNoteIntegrative2 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3DDT
                                    strNoteIntegrative3 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_Articolo62DDT
                                    strArticolo62 = valore
                            End Select
                            '=================================================

                        Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                            Select Case dr.Item("Impostazione_Cod")
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1DDTCorrispettivi
                                    strNoteIntegrative1 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2DDTCorrispettivi
                                    strNoteIntegrative2 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3DDTCorrispettivi
                                    strNoteIntegrative3 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_Articolo62DDTCorrispettivi
                                    strArticolo62 = valore
                            End Select
                            '=================================================

                        Case LAVCOD_ORDINE_VENDITA
                            Select Case dr.Item("Impostazione_Cod")
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1Ordini
                                    strNoteIntegrative1 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2Ordini
                                    strNoteIntegrative2 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3Ordini
                                    strNoteIntegrative3 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_Articolo62Ordini
                                    strArticolo62 = valore
                            End Select
                            '=================================================


                        Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                            LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
                            Select Case dr.Item("Impostazione_Cod")
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative1Fatture
                                    strNoteIntegrative1 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative2Fatture
                                    strNoteIntegrative2 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_NoteIntegrative3Fatture
                                    strNoteIntegrative3 = valore
                                Case enum_Impostazioni_Utenti.SuperUser_Articolo62Fatture
                                    strArticolo62 = valore
                            End Select
                            '=================================================

                    End Select

                    Select Case dr.Item("Impostazione_Cod")

                        Case enum_Impostazioni_Utenti.SuperUser_ContributoConai
                            If valore = "1" Then
                                flagContributoConai = True
                            End If

                        Case enum_Impostazioni_Utenti.SuperUser_StRifOrdine
                            If valore = "1" Then
                                flagStampaRifOrdine = True
                            End If

                        Case enum_Impostazioni_Utenti.SUPERUSER_StampaAlcolTotale
                            If valore = "1" Then
                                flagGradoAlcolico = True
                            End If
                        Case enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO
                            If valore = "1" Then
                                FlagGestMaterialeVivaistico = True
                            End If
                        Case enum_Impostazioni_Utenti.SUPERUSER_INTESTAZIONE_STAMPA_CENTROAZIENDALEPARTENZA
                            If valore = "1" Then
                                Flag_CentroAziendalePartenza = True
                            End If
                    End Select

                Next

            End If

        Catch ex As Exception
            logErrori &= "ImpostazioniUtente_StampaDoc. Errore: " & ex.Message
        End Try
    End Sub

    Public Sub ImpostazioniImpresaUtente_StampaDoc(ByRef objParametriServer As AgronicaCoreParametri,
                                                        ByRef objParametriUtenti As AgronicaCoreParametri,
                                                        ByRef logErrori As String,
                                                        ByVal piva As String,
                                                        ByVal saCod As Integer,
                                                        ByRef stampaSDI As Boolean)

        Dim handleImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Try
            Dim listaCentriAz As New List(Of Integer)() From {saCod}
            Dim strStampaSDI As String = handleImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(piva, listaCentriAz, enum_Impostazioni_Utenti.StampaDocAcquisto_CodiceSDI, 0, objParametriUtenti, objParametriServer)

            stampaSDI = Not strStampaSDI = "0"

        Catch ex As Exception
            logErrori &= "ImpostazioniImpresaUtente_StampaDoc. Errore: " & ex.Message
        End Try

    End Sub

    ''' <summary>
    ''' L'impostazione intende se mostrare la dicitura di conformità al dlgs. 198, 8/11/2021 (ex Articolo 62 dl. 1 del 24/01/2012)
    ''' se il DDT contiene almeno un prodotto di categoria ALTRE_MATERIE (200). Il nome dell'impostazione è generico sull'elem_cod
    ''' per eventuali estensioni future e si fa riferimento all'articolo abrogato per coerenza con i sorgenti già esistenti
    ''' </summary>
    ''' <param name="objParametriServer"></param>
    ''' <param name="objParametriUtenti"></param>
    ''' <param name="logErrori"></param>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="stampaArticolo62ElemCod"></param>
    Public Sub ImpostazioniImpresaUtente_Articolo62ElemCod(ByRef objParametriServer As AgronicaCoreParametri,
                                                        ByRef objParametriUtenti As AgronicaCoreParametri,
                                                        ByRef logErrori As String,
                                                        ByVal piva As String,
                                                        ByVal saCod As Integer,
                                                        ByRef stampaArticolo62ElemCod As Boolean)

        Dim handleImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Try
            Dim listaCentriAz As New List(Of Integer)() From {saCod}
            Dim strStampaArticolo62ElemCod As String = handleImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(piva, listaCentriAz, enum_Impostazioni_Utenti.StampaArticolo62ElemCod, 0, objParametriUtenti, objParametriServer)

            stampaArticolo62ElemCod = Not strStampaArticolo62ElemCod = "0"

        Catch ex As Exception
            logErrori &= "ImpostazioniImpresaUtente_Articolo62ElemCod. Errore: " & ex.Message
        End Try

    End Sub

    '############################################################################
    Public Function ImpostaAgente(ByRef logErrori As String, ByVal codRisUmAgente As Integer, ByRef objParametriServer As AgronicaCoreParametri) As String

        Dim agenteInfo As String = ""
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        Try

            Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi("", "",
                                                                              codRisUmAgente,
                                                                              0, False,
                                                                              False, 0, 0,
                                                                              False, 0,
                                                                              ID_CF_NOFILTRO,
                                                                              0, "", True,
                                                                              0, 0, 0, 0, 0,
                                                                              enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                              "", "",
                                                                              objParametriServer)

            If Not IsNothing(dtContatti) Then
                If dtContatti.Rows.Count <> 0 Then
                    If dtContatti.Rows(0).Item("Rag_Soc") <> "" Then
                        agenteInfo = dtContatti.Rows(0).Item("Rag_Soc")
                    Else
                        agenteInfo = dtContatti.Rows(0).Item("Cognome") & " " & dtContatti.Rows(0).Item("Nome")
                    End If
                End If
            End If

        Catch ex As Exception
            logErrori &= "- impostazione agente: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Return agenteInfo

    End Function

    '############################################################################
    Public Sub NascondiLoghiVuoti(ByRef logErrori As String,
                                  ByRef drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow,
                                  ByRef reportDocument As CrystalDecisions.CrystalReports.Engine.ReportDocument,
                                  ByRef objParametriServer As AgronicaCoreParametri)

        Dim objStampeUtility As New AgronicaCoreStampeDAL.Utility

        Try
            Dim listLoghi As New List(Of String)

            If drIntestazioneNew.IsLogoInAltoNull Then
                listLoghi.Add(STAMPE_CONTAB_LOGO_IN_ALTO)
            End If

            If drIntestazioneNew.IsLogoNull Then
                listLoghi.Add(STAMPE_CONTAB_LOGO_IN_BASSO)
            End If

            If drIntestazioneNew.IsLogoNull Then
                listLoghi.Add(STAMPE_CONTAB_LOGO_IN_BASSO)
            End If

            If drIntestazioneNew.IsLogoHeaderNull Then
                listLoghi.Add(STAMPE_CONTAB_LOGO_HEADER)
            End If

            If drIntestazioneNew.IsLogoFooterNull Then
                listLoghi.Add(STAMPE_CONTAB_LOGO_FOOTER)
            End If

            If listLoghi.Count > 0 Then
                objStampeUtility.HideShowObjCrystal(reportDocument, "", CrystalDecisions.Shared.ReportObjectKind.BlobFieldObject, listLoghi, False, objParametriServer)
            End If

        Catch ex As Exception
            logErrori &= "- Nascondo i loghi vuoti: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '############################################################################
    Public Sub InserisciRigaVuotaDescrizioneFattura(ByRef ds As DataSetFattura, ByVal contatore As Integer)
        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow

        drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
        drDescrizioneNew.Contatore = contatore
        drDescrizioneNew.Descrizione = ""
        drDescrizioneNew.SuperPiva = ""
        drDescrizioneNew.Udm_Des = ""
        drDescrizioneNew.Qta = ""
        drDescrizioneNew.Prezzo = ""
        drDescrizioneNew.Sconto = ""
        drDescrizioneNew.Iva = ""
        drDescrizioneNew.IvaImposta = ""
        drDescrizioneNew.Importo = ""
        drDescrizioneNew.Extra_Str_1 = ""
        drDescrizioneNew.Extra_Str_2 = ""
        drDescrizioneNew.Extra_Str_3 = ""
        drDescrizioneNew.Extra_Str_4 = ""
        drDescrizioneNew.Extra_Str_5 = ""

        ds.Descrizione.Rows.Add(drDescrizioneNew)
    End Sub

    '############################################################################
    Public Sub CaricaLogoInCampoBlobFattura(ByRef logErrori As String,
                                            ByRef drIntestazione As DataSetFattura.IntestazioneFatturaRow,
                                            ByVal piva As String,
                                            ByVal nomeCampoBlob As String)

        Dim nomeLogo As String = piva & "_LogoFattura.jpg"
        Dim nomeLogoAlternativo As String = "LogoFattura.jpg" 'Usato internamente o per prova
        Dim nomeLogoHeader As String = piva & "_LogoHeaderFattura.jpg"
        Dim nomeLogoFooter As String = piva & "_LogoFooterFattura.jpg"

        Dim path As String
        Dim pathAlternativo As String

        Try

            'di default inserisco un'immagine bianca come logo
            Dim bmpImg As New Drawing.Bitmap(1, 1)
            bmpImg.SetPixel(0, 0, Drawing.Color.White)

            Dim cx As New Drawing.ImageConverter
            Dim logoBianco() As Byte
            logoBianco = cx.ConvertTo(bmpImg, GetType(Byte()))

            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                Try

                    objWebConfig.Path_Directory_Loghi_Cliente = AggiungiSlashSeNonEsiste(objWebConfig.Path_Directory_Loghi_Cliente)

                    Select Case nomeCampoBlob
                        Case STAMPE_CONTAB_LOGO_HEADER
                            path = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogoHeader
                            If IO.File.Exists(path) = True Then
                                drIntestazione.LogoHeader = My.Computer.FileSystem.ReadAllBytes(path)
                            Else
                                drIntestazione.LogoHeader = logoBianco
                            End If
                        Case STAMPE_CONTAB_LOGO_FOOTER
                            path = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogoFooter
                            If IO.File.Exists(path) = True Then
                                drIntestazione.LogoFooter = My.Computer.FileSystem.ReadAllBytes(path)
                            Else
                                drIntestazione.LogoFooter = logoBianco
                            End If
                        Case STAMPE_CONTAB_LOGO_IN_ALTO
                            path = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogo
                            pathAlternativo = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogoAlternativo
                            If IO.File.Exists(path) = True Then
                                drIntestazione.LogoInAlto = My.Computer.FileSystem.ReadAllBytes(path)
                            Else
                                If IO.File.Exists(pathAlternativo) = True Then
                                    drIntestazione.LogoInAlto = My.Computer.FileSystem.ReadAllBytes(pathAlternativo)
                                Else
                                    drIntestazione.LogoInAlto = logoBianco
                                End If
                            End If
                        Case STAMPE_CONTAB_LOGO_IN_BASSO
                            path = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogo
                            pathAlternativo = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogoAlternativo

                            If IO.File.Exists(path) = True Then
                                drIntestazione.Logo = My.Computer.FileSystem.ReadAllBytes(path)
                            Else
                                If IO.File.Exists(pathAlternativo) = True Then
                                    drIntestazione.Logo = My.Computer.FileSystem.ReadAllBytes(pathAlternativo)
                                Else
                                    drIntestazione.Logo = logoBianco
                                End If
                            End If
                    End Select

                Catch ex As Exception
                    logErrori &= "CaricaLogoFattura. Caricamento logo cliente: " & ex.Message
                End Try

            End If 'path

        Catch ex As Exception
            logErrori &= "CaricaLogoFattura: " & ex.Message
        End Try

    End Sub

    '############################################################################
    Public Sub SalvaLogErrori_Agenda(ByVal logErrori As String,
                                     ByVal nomeDocumento As String,
                                     ByVal identificazioneDocumento As String,
                                     ByVal nomeRoutine As String,
                                     ByVal sottoCartella As String,
                                     ByVal idAgenda As Integer,
                                     ByRef objParametriServer As AgronicaCoreParametri)

        If logErrori <> "" Then

            logErrori = nomeDocumento & ", id_agenda = " & CStr(idAgenda) & vbCrLf & vbCrLf & logErrori

            Dim nomeFile As String = "LogErrori_" & identificazioneDocumento & "_" & objParametriServer.UtenteUsername & "_id" & CStr(idAgenda) & ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametriServer,
                                             sottoCartella,
                                             nomeFile,
                                             objParametriServer.UtenteUsername,
                                             nomeRoutine,
                                             logErrori)
        End If

    End Sub

    '############################################################################
    Public Sub SalvaLogErrori_Generico(ByVal logErrori As String,
                                       ByVal nomeDocumento As String,
                                       ByVal identificazioneDocumento As String,
                                       ByVal nomeRoutine As String,
                                       ByVal sottoCartella As String,
                                       ByVal infoInLog As String,
                                       ByRef objParametriServer As AgronicaCoreParametri)

        If logErrori <> "" Then

            logErrori = nomeDocumento & ", " & infoInLog & vbCrLf & vbCrLf & logErrori

            Dim nomeFile As String = "LogErrori_" & identificazioneDocumento & "_" & objParametriServer.UtenteUsername & ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametriServer,
                                             sottoCartella,
                                             nomeFile,
                                             objParametriServer.UtenteUsername,
                                             nomeRoutine,
                                             logErrori)
        End If

    End Sub

    '############################################################################
    Public Sub ValorizzaRisUmAggiuntivo(ByRef logErrori As String,
                                        ByVal x_Cod_RisUm_Aggiuntivo As Integer,
                                        ByVal x_Cod_Indirizzo_Aggiuntivo As Integer,
                                        ByVal x_Id_Cf_Aggiuntivo As Integer,
                                        ByVal x_CodContatto_Aggiuntivo As String,
                                        ByVal x_RagSoc_Aggiuntivo As String,
                                        ByVal x_CodiceFiscale_Aggiuntivo As String,
                                        ByVal x_ChkFittizio_Aggiuntivo As Integer,
                                        ByRef Parametro_Rag_Soc_Aggiuntivo As String,
                                        ByRef Parametro_Indirizzo_Aggiuntivo As String,
                                        ByRef Parametro_CAP_Aggiuntivo As String,
                                        ByRef Parametro_Frazione_Aggiuntivo As String,
                                        ByRef Parametro_Comune_Aggiuntivo As String,
                                        ByRef Parametro_Prov_Aggiuntivo As String,
                                        ByRef Parametro_TxtPivaAgg As String,
                                        ByRef Parametro_TxtCFAgg As String,
                                        ByRef objParametriServer As AgronicaCoreParametri)

        Dim pivaAggiuntivo As String = ""
        Dim codiceFiscaleAggiuntivo As String = ""
        Dim xIndirizzoAggiuntivo As String = ""
        Dim xFrazioneAggiuntivo As String = ""
        Dim xCapAggiuntivo As String = ""
        Dim xComuneAggiuntivo As String = ""
        Dim xProvinciaAggiuntivo As String = ""
        Dim xStatoAggiuntivo As String = ""

        Try

            Leggi_Indirizzi(objParametriServer,
                            x_CodContatto_Aggiuntivo,
                            x_Cod_RisUm_Aggiuntivo,
                            x_Cod_Indirizzo_Aggiuntivo,
                            xIndirizzoAggiuntivo,
                            xFrazioneAggiuntivo,
                            xCapAggiuntivo,
                            xComuneAggiuntivo,
                            xProvinciaAggiuntivo,
                            xStatoAggiuntivo,
                            Nothing,
                            Nothing, _
                            "")

            Parametro_Indirizzo_Aggiuntivo = xIndirizzoAggiuntivo
            Parametro_Frazione_Aggiuntivo = xFrazioneAggiuntivo
            Parametro_CAP_Aggiuntivo = xCapAggiuntivo

            Select Case x_Id_Cf_Aggiuntivo

                Case enum_Contatti_IdCf.ContattoEstero
                    Parametro_Comune_Aggiuntivo = xStatoAggiuntivo
                    Parametro_Prov_Aggiuntivo = ""

                    'pivaAggiuntivo = x_CodiceFiscale_Aggiuntivo
                    'codiceFiscaleAggiuntivo = ""

                    Ricava_Piva_Codicefiscale(x_Id_Cf_Aggiuntivo, _
                                           x_CodContatto_Aggiuntivo,
                                           x_CodiceFiscale_Aggiuntivo,
                                           x_ChkFittizio_Aggiuntivo,
                                           pivaAggiuntivo,
                                           codiceFiscaleAggiuntivo,
                                           Nothing)

                Case Else

                    Ricava_Piva_Codicefiscale(x_Id_Cf_Aggiuntivo, _
                                              x_CodContatto_Aggiuntivo,
                                              x_CodiceFiscale_Aggiuntivo,
                                              x_ChkFittizio_Aggiuntivo,
                                              pivaAggiuntivo,
                                              codiceFiscaleAggiuntivo,
                                              Nothing)

                    Parametro_Comune_Aggiuntivo = xComuneAggiuntivo
                    Parametro_Prov_Aggiuntivo = "(" & xProvinciaAggiuntivo & ")"

                    If x_Id_Cf_Aggiuntivo = enum_Contatti_IdCf.PersonaFisica Then
                        pivaAggiuntivo = ""
                    End If

            End Select

            Parametro_Rag_Soc_Aggiuntivo = x_RagSoc_Aggiuntivo
            Parametro_TxtPivaAgg = pivaAggiuntivo
            Parametro_TxtCFAgg = codiceFiscaleAggiuntivo

        Catch ex As Exception
            logErrori &= "- Lettura dell'indirizzo del cessionario aggiuntivo: " & vbCrLf & ex.Message & vbCrLf
            'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
        End Try

    End Sub

    '############################################################################
    Public Sub AggiungiNoteAggiuntiveOmaggi(ByRef logErrori As String,
                                            ByRef ds As DataSetFattura,
                                            ByRef contatore As Integer,
                                            ByVal strNoteIntegrative1 As String,
                                            ByVal strNoteIntegrative2 As String,
                                            ByVal strNoteIntegrative3 As String,
                                            ByVal flagCampioniOmaggioSenzaRivalsa As Boolean,
                                            ByVal flagCampioniOmaggioRivalsaIva As Boolean,
                                            ByVal flagCampioniGratuiti As Boolean,
                                            ByVal flagScontoMerce As Boolean)

        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow

        'NOTE AGGIUNTIVE NEI DETTAGLI
        Try

            If strNoteIntegrative1 <> "" Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.Descrizione = strNoteIntegrative1
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

            If strNoteIntegrative2 <> "" Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.Descrizione = strNoteIntegrative2
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

            If strNoteIntegrative3 <> "" Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.Descrizione = strNoteIntegrative3
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

        Catch ex As Exception
            logErrori &= "- Visualizzazione note aggiuntive nei dettagli: " & vbCrLf & ex.Message & vbCrLf
        End Try
        '-------------------------------------

        Try

            '   CASO CAMPIONI OMAGGIO
            If flagCampioniOmaggioSenzaRivalsa = True Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.Descrizione = vbCrLf & vbCrLf & "Cessione gratuita ai sensi dell'art.2, comma 2 del D.P.R. n.633/1972 " &
                                               "senza esercizio di rivalsa dell'IVA, ai sensi dell'art.18, comma 3, del D.P.R. n.633/1972"
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

            '   CASO CAMPIONI OMAGGIO CON RIVALSA IVA
            If flagCampioniOmaggioRivalsaIva = True Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.Descrizione = vbCrLf & vbCrLf & "Cessione gratuita ai sensi dell'art. 2, comma 2, n. 4, del D.P.R. n. 633/1972, con rivalsa dell'Iva."
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

            'aggiunto in data 06/11/2012:
            '   CASO CAMPIONI GRATUITI 
            If flagCampioniGratuiti = True Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.Descrizione = vbCrLf & vbCrLf & "Cessione gratuita ai sensi dell'art.2, n.4 del D.P.R. n.633/1972 " &
                                               "senza esercizio della rivalsa dell'IVA, ai sensi dell'art.18, comma 3, del D.P.R. n.633/1972"
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

            '   CASO SCONTO MERCE
            If flagScontoMerce = True Then
                contatore += 1
                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow
                drDescrizioneNew.Contatore = contatore
                '23/05/2019: aggiornata dicitura su richiesta di BORGOLUCE
                'drDescrizioneNew.Descrizione = vbCrLf & vbCrLf & "Sconto merce senza esercizio di rivalsa dell'IVA ai sensi dell'art.15 del D.P.R. n.633/1972"
                drDescrizioneNew.Descrizione = vbCrLf & vbCrLf & "Sconto merce escluso dalla base imponibile ai sensi dell’art. 15/2 DPR 633/72"
                drDescrizioneNew.SuperPiva = ""
                drDescrizioneNew.Udm_Des = ""
                drDescrizioneNew.Qta = ""
                drDescrizioneNew.Prezzo = ""
                drDescrizioneNew.Sconto = ""
                drDescrizioneNew.Iva = ""
                drDescrizioneNew.IvaImposta = ""
                drDescrizioneNew.Importo = ""
                ds.Descrizione.Rows.Add(drDescrizioneNew)
            End If

        Catch ex As Exception
            logErrori &= "- Visualizzazione della nota sugli omaggi: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '############################################################################
    Public Sub AggiungiRiferimentoDocAllegato(ByRef logErrori As String,
                                               ByRef DS As DataSetFattura,
                                               ByVal Flag_Raggruppa As Boolean,
                                               ByRef Riferimento_DocAllegato As String,
                                               ByVal i As Integer,
                                               ByRef Ultimo_DocRiferimento As String,
                                               ByRef numContatoreBasePerRif As Integer,
                                               ByVal piva As String,
                                               ByVal objConfigStampe As ConfigurazioneStampe,
                                               ByVal moduloFreshFood As Boolean,
                                               ByVal flagStampaRiepilogoImballi As Boolean,
                                               ByVal tipologiaRiga As enum_TipoRigaFattura)

        Try
            '  Giulia, 14/02/2017 11:57:44: se sto raggruppando i prodotti non devo stampare le righe di intestazione
            If Riferimento_DocAllegato <> "" AndAlso Flag_Raggruppa = False Then
                '  Giulia, 19/01/2017 14:50:22: se ho un riferimento lo devo riportare solo la prima volta che compare,
                '       come una riga aggiuntiva in testa ai prodotti; se è allegato più di un documento,
                '       aggiungo la riga del riferimento solo la prima volta che compare e quando è diversa dalla prcedente
                '       (==> i prodotti seguenti sono collegati ad un altro documento)
                Riferimento_DocAllegato = "----" & Riferimento_DocAllegato & " -----"
                If Riferimento_DocAllegato <> Ultimo_DocRiferimento Then

                    'aggiungo una riga vuota per separarmi dal resto (solo se non è il primo dettaglio della fattura)
                    If i <> 0 Then
                        InserisciRigaVuotaDescrizioneFattura(DS, numContatoreBasePerRif)
                        numContatoreBasePerRif += 1
                    End If

                    Ultimo_DocRiferimento = Riferimento_DocAllegato

                    'rif cambiato => devo aggiungere la riga dei riferimenti
                    Dim drDescrizioneRifNew As DataSetFattura.DescrizioneRow = DS.Descrizione.NewDescrizioneRow

                    drDescrizioneRifNew.Descrizione = Ultimo_DocRiferimento

                    drDescrizioneRifNew.Udm_Des = ""
                    drDescrizioneRifNew.Qta = ""
                    drDescrizioneRifNew.Prezzo = ""
                    drDescrizioneRifNew.Sconto = ""
                    drDescrizioneRifNew.Iva = ""
                    drDescrizioneRifNew.IvaImposta = ""
                    drDescrizioneRifNew.Importo = ""
                    drDescrizioneRifNew.Extra_Str_1 = ""
                    drDescrizioneRifNew.SuperPiva = piva

                    'il contatore è obbligatorio: uso il numero totale delle righe + 10 (e a seguire) per essere sicuri che sia univoco
                    drDescrizioneRifNew.Contatore = numContatoreBasePerRif
                    numContatoreBasePerRif += 1

                    If moduloFreshFood = True Then

                        Select Case tipologiaRiga

                            Case enum_TipoRigaFattura.RiepilogoConfezioni
                                'Se è voce di riepilogo confezioni ed ho scelto di visualizzarli
                                If objConfigStampe.Flag_RiepilogoConfezioni = True AndAlso flagStampaRiepilogoImballi = True Then
                                    DS.Descrizione.Rows.Add(drDescrizioneRifNew)
                                End If

                            Case enum_TipoRigaFattura.RiepilogoContenitori
                                'Se è voce di riepilogo contenitori ed ho scelto di visualizzarli
                                If objConfigStampe.Flag_RiepilogoContenitori = True AndAlso flagStampaRiepilogoImballi = True Then
                                    DS.Descrizione.Rows.Add(drDescrizioneRifNew)
                                End If

                            Case enum_TipoRigaFattura.RiepilogoImballaggi
                                'Se è voce di riepilogo imballaggi ed ho scelto di visualizzarli
                                If objConfigStampe.Flag_RiepilogoImballi = True AndAlso flagStampaRiepilogoImballi = True Then
                                    DS.Descrizione.Rows.Add(drDescrizioneRifNew)
                                End If

                            Case Else
                                'stampo sempre il dettaglio
                                DS.Descrizione.Rows.Add(drDescrizioneRifNew)
                        End Select

                    Else
                        DS.Descrizione.Rows.Add(drDescrizioneRifNew)
                    End If

                End If
            ElseIf Riferimento_DocAllegato = "" AndAlso Ultimo_DocRiferimento <> "" AndAlso Flag_Raggruppa = False Then
                'ho degli oggetti non allegati a documento, ma quello prima era allegato, quindi lascio una riga vuota 
                ' per separare i dettagli che non sono allegati al documento di prima
                ' non c'è bisogno di considerare il caso della prima riga perchè tanto se fosse la prima riga Ultimo_DocRiferimento
                ' sarebbe comunque zero
                InserisciRigaVuotaDescrizioneFattura(DS, numContatoreBasePerRif)
                numContatoreBasePerRif += 1
                Ultimo_DocRiferimento = ""
            End If

        Catch ex As Exception
            logErrori &= "- Aggiungi riferimento doc allegato: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '############################################################################
    Public Function ConteggioLitriDettaglio(ByRef x_Descrizione As String,
                                            ByVal x_Udm_Cod As Integer,
                                            ByVal x_Udm_Cod_Extra As Integer,
                                            ByVal x_Qta As Decimal,
                                            ByVal x_Qta_Extra_Totale As Decimal
                                            ) As Decimal

        Dim litriDettaglio As Decimal
        Dim udmLitro As Boolean = False

        If x_Udm_Cod = enum_UnitaMisura.Litri Then
            udmLitro = True
            litriDettaglio = x_Qta
        ElseIf x_Udm_Cod_Extra = enum_UnitaMisura.Litri Then
            udmLitro = True
            litriDettaglio = x_Qta_Extra_Totale
        End If

        If udmLitro = True Then
            x_Descrizione &= " [Litri: " & Format(litriDettaglio, "#,###,##0.####") & "]"
        End If

        Return litriDettaglio
    End Function

    '############################################################################
    Public Sub CalcolaPrezziFFLivello(ByVal x_Prezzo_Livello As Integer,
                                      ByRef DrDescrizioneNew As DataSetFattura.DescrizioneRow,
                                      ByRef Prezzo_Unitario_REALE As Decimal,
                                      ByRef Prezzo_Unitario_Netto_REALE As Decimal,
                                      ByVal x_Qta As Decimal,
                                      ByVal x_Qta_Extra As Decimal,
                                      ByVal x_Prezzo_Unitario As Decimal,
                                      ByVal x_Prezzo_Unitario_Netto As Decimal,
                                      ByVal x_Prezzo_Effettivo As Decimal,
                                      ByVal x_Imponibile As Decimal,
                                      ByVal x_Imponibile_Netto As Decimal,
                                      ByVal x_Udm_Sim As String,
                                      ByVal x_Udm_Sim_Extra As String,
                                      ByVal x_Flag_Extra As Integer,
                                      ByVal numcolli_contenitori As Integer,
                                      ByVal numcontenitori_imballi As Integer)

        Select Case x_Prezzo_Livello

            Case enum_OTabelle.Confezione

                'è il livello più interno, quindi di fatto è come se avessi 0, non devo ricalcolare nulla
                DrDescrizioneNew.Udm_Des = "n"
                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des

            Case enum_OTabelle.Contenitore

                DrDescrizioneNew.Udm_Des = "n"
                DrDescrizioneNew.Qta = Format(numcolli_contenitori, "#,###,##0.####")
                Prezzo_Unitario_REALE = x_Imponibile / numcolli_contenitori
                Prezzo_Unitario_Netto_REALE = x_Imponibile_Netto / numcolli_contenitori
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des

            Case enum_OTabelle.Imballaggio

                DrDescrizioneNew.Udm_Des = "n"
                DrDescrizioneNew.Qta = Format(numcontenitori_imballi, "#,###,##0.####")
                Prezzo_Unitario_REALE = x_Imponibile / numcontenitori_imballi
                Prezzo_Unitario_Netto_REALE = x_Imponibile_Netto / numcontenitori_imballi
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des

            Case enum_OTabelle.Nessuno

                'non devo fare nessuna operazione particolare => valori secchi
                DrDescrizioneNew.Udm_Des = x_Udm_Sim
                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto

                '  Giulia, 23/01/2017 16:39:34: solo in questo caso devo verificare se ho scelto di esprimerlo in kg anziché numero,
                '       perché vince sempre la scelta a livello di documento, quindi se sono in uno degli altri casi
                '       non c'è bisogno che verifico cosa dice il Flag_Extra

                If x_Flag_Extra = 1 Then
                    'anagrafica con impostazione udm aspetto default
                    DrDescrizioneNew.Udm_Des = IIf(x_Udm_Sim_Extra = "", x_Udm_Sim, x_Udm_Sim_Extra)
                    DrDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.####")
                    DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Effettivo, "#,###,##0.00##") & "/Kg"
                Else
                    'DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des
                    DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des
                End If

            Case -1
                'ho impostato il prezzo al KG
                DrDescrizioneNew.Udm_Des = x_Udm_Sim
                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                Prezzo_Unitario_REALE = x_Prezzo_Effettivo
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Effettivo
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_REALE, "#,###,##0.00##") & "/Kg"

        End Select

        If DrDescrizioneNew.Qta = 0 Then
            'movimentato a kg
            DrDescrizioneNew.Udm_Des = x_Udm_Sim
            DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
        End If

    End Sub

    '############################################################################
    ''' <summary>
    '''  Usata per nuova versione arrotondamenti: in prezzo_unitario viene già salvato il valore del prezzo per il livello scelto,
    '''  quindi in fase di stampa non è necessario fare alcun ricalcolo, ma prendere il valore così com'è sul db
    ''' </summary>
    Public Sub CalcolaPrezziFFLivello_NEW(ByVal x_Prezzo_Livello As Integer,
                                          ByRef DrDescrizioneNew As DataSetFattura.DescrizioneRow,
                                          ByRef Prezzo_Unitario_REALE As Decimal,
                                          ByRef Prezzo_Unitario_Netto_REALE As Decimal,
                                          ByVal x_Qta As Decimal,
                                          ByVal x_Qta_Extra As Decimal,
                                          ByVal x_Prezzo_Unitario As Decimal,
                                          ByVal x_Prezzo_Unitario_Netto As Decimal,
                                          ByVal x_Prezzo_Effettivo As Decimal,
                                          ByVal x_Imponibile As Decimal,
                                          ByVal x_Imponibile_Netto As Decimal,
                                          ByVal x_Udm_Sim As String,
                                          ByVal x_Udm_Sim_Extra As String,
                                          ByVal x_Flag_Extra As Integer,
                                          ByVal numcolli_contenitori As Integer,
                                          ByVal numcontenitori_imballi As Integer,
                                          ByVal tipoArrotondamentoFF As enum_TipoArrotondamentoFF,
                                          ByVal flagPrezzoAlKgUdmKg As Boolean)

        Select Case x_Prezzo_Livello

            Case enum_OTabelle.Confezione

                'è il livello più interno, quindi di fatto è come se avessi 0, non devo ricalcolare nulla
                DrDescrizioneNew.Udm_Des = "n"
                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.###")
                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des

            Case enum_OTabelle.Contenitore

                DrDescrizioneNew.Udm_Des = "n"
                DrDescrizioneNew.Qta = Format(numcolli_contenitori, "#,###,##0.###")
                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des

            Case enum_OTabelle.Imballaggio

                DrDescrizioneNew.Udm_Des = "n"
                DrDescrizioneNew.Qta = Format(numcontenitori_imballi, "#,###,##0.###")
                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des

            Case enum_OTabelle.Nessuno

                'non devo fare nessuna operazione particolare => valori secchi
                DrDescrizioneNew.Udm_Des = x_Udm_Sim
                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.###")
                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto

                '  Giulia, 23/01/2017 16:39:34: solo in questo caso devo verificare se ho scelto di esprimerlo in kg anziché numero,
                '       perché vince sempre la scelta a livello di documento, quindi se sono in uno degli altri casi
                '       non c'è bisogno che verifico cosa dice il Flag_Extra

                If x_Flag_Extra = 1 Then
                    'anagrafica con impostazione udm aspetto default
                    DrDescrizioneNew.Udm_Des = IIf(x_Udm_Sim_Extra = "", x_Udm_Sim, x_Udm_Sim_Extra)
                    DrDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.###")
                    DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Effettivo, "#,###,##0.00##") & "/Kg"
                Else
                    'DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des
                    DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des
                End If

            Case -1
                'ho impostato il prezzo al KG

                If flagPrezzoAlKgUdmKg = True Then
                    'in questo caso l'udm è sempre kg e la qta è kg netti, a prescindere che su db ci sia quella (cont, imballi) oppure no (confezioni)
                    DrDescrizioneNew.Udm_Des = x_Udm_Sim_Extra  'dovrebbe essere sempre kg nel FF
                    'DrDescrizioneNew.Qta = Format(x_Qta * x_Qta_Extra, "#,###,##0.####")
                    DrDescrizioneNew.Qta = FormattaArrotondaFF(tipoArrotondamentoFF, x_Qta * x_Qta_Extra, "#,###,##0.###")
                Else
                    DrDescrizioneNew.Udm_Des = x_Udm_Sim
                    'DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                    DrDescrizioneNew.Qta = FormattaArrotondaFF(tipoArrotondamentoFF, x_Qta, "#,###,##0.###")
                End If
                Prezzo_Unitario_REALE = x_Prezzo_Effettivo
                Prezzo_Unitario_Netto_REALE = x_Prezzo_Effettivo
                DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_REALE, "#,###,##0.00##") & "/Kg"

        End Select

        If DrDescrizioneNew.Qta = 0 Then
            'movimentato a kg
            DrDescrizioneNew.Udm_Des = x_Udm_Sim
            DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
        End If

    End Sub

    '############################################################################
    Public Function FormattaArrotondaFF(ByVal tipoArrotondamentoFF As enum_TipoArrotondamentoFF,
                                        ByVal valore As Decimal,
                                        ByVal formatAlternativoNessuno As String
                                        ) As String

        Select Case tipoArrotondamentoFF
            Case enum_TipoArrotondamentoFF.Nessuno
                If formatAlternativoNessuno <> "" Then
                    Return Format(valore, formatAlternativoNessuno)
                Else
                    Return Format(valore, "#,###,##0.##")
                End If
            Case enum_TipoArrotondamentoFF.Kg
                Return Format(valore, "#,###,##0")
            Case Else
                'per il momento lascio lo stesso di nessuno
                Return Format(valore, "#,###,##0.##")
        End Select

    End Function

    '############################################################################
    Public Function GetTipologiaRiga(ByVal elemCod As Integer,
                                     ByVal movDetDes As String,
                                     ByVal flagRaggruppa As Boolean,
                                     ByVal chkConfezione As Integer,
                                     ByVal chkContenitore As Integer,
                                     ByVal chkImballaggio As Integer,
                                     ByVal ordineDet As Integer,
                                     ByRef logErrori As String
                                     ) As enum_TipoRigaFattura

        Dim tipologiaRiga As enum_TipoRigaFattura = enum_TipoRigaFattura.NonSpecificato

        Try

            '  Giulia, 02/03/2017 12:35:50: se sto usando i beni confezionamento come prodotto principale di vendita, 
            '       li devo trattare come tutti gli altri, anche se hanno ChkContenitore o ChkImballaggio
            '       se ordineDet = '1000' allora non sono i prodotti principali
            '
            '

            'Devo capire se sono in una delle righe di riepilogo
            If (elemCod = BENI_CONFEZ_ANIMALE OrElse elemCod = BENI_CONFEZ_VEGETALE) AndAlso ordineDet = 1000 Then
                'sono una voce di scarico generico dei confezionamenti

                'Devo capire se sono confezione/contenitore/imballaggio
                If chkImballaggio = 1 Then
                    'Sono imballaggio
                    tipologiaRiga = enum_TipoRigaFattura.RiepilogoImballaggi

                ElseIf chkContenitore = 1 Then
                    'Sono contenitore
                    tipologiaRiga = enum_TipoRigaFattura.RiepilogoContenitori

                ElseIf chkConfezione = 1 Then
                    'Sono confezione
                    tipologiaRiga = enum_TipoRigaFattura.RiepilogoConfezioni

                End If

            ElseIf elemCod = RIGA_DESCRIZIONE_LIBERA Then

                tipologiaRiga = enum_TipoRigaFattura.DescrizioneLibera

            ElseIf flagRaggruppa = True Then

                tipologiaRiga = enum_TipoRigaFattura.DettaglioRaggruppato

            Else
                tipologiaRiga = enum_TipoRigaFattura.DettaglioNormale
            End If

        Catch ex As Exception
            logErrori &= "- Tipologia Riga: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Return tipologiaRiga
    End Function


    '############################################################################
    Public Sub ValorizzazioneRegistroRiba(ByVal piva As String, ByVal dt As DataTable, ByRef dsRiba As DS_RegistroRiBa, ByRef logErrori As String)

        Dim i As Integer
        Dim drR As DS_RegistroRiBa.DS_RegistroRiBaRow

        Dim IBAN_Impresa As String
        Dim IBAN_Cliente As String
        Dim importoPagamento As Decimal = 0
        Dim importoTotalePagamento As Decimal = 0
        Dim rifDocumento, indirizzoCliente As String

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            For i = 0 To dt.Rows.Count - 1

                Try

                    IBAN_Impresa = ""
                    IBAN_Cliente = ""

                    With dt.Rows(i)

                        drR = dsRiba.DS_RegistroRiBa.NewRow

                        drR.Piva = piva
                        drR.Impresa = .Item("Impresa")

                        If .Item("Filiale_DARE") <> 0 Then
                            drR.Filiale_Impresa = " - Filiale n." & CStr(.Item("Filiale_DARE"))
                        Else
                            drR.Filiale_Impresa = ""
                        End If

                        drR.Banca_Impresa = .Item("Istituto_Des_DARE") & drR.Filiale_Impresa

                        IBAN_Impresa &= Contabilita.Costruisci_IBAN(.Item("Nazione_DARE"), .Item("Cifre_Controllo_DARE"), .Item("Cin_DARE"),
                                                                    .Item("Abi_DARE"), .Item("Cab_DARE"), .Item("Numero_DARE"), True) & " "

                        If .Item("Bic_DARE") <> "" Then
                            IBAN_Impresa &= " Bic/Swift: " & .Item("Bic_DARE")
                        End If

                        drR.IBAN_Impresa = IBAN_Impresa

                        drR.Num = CStr(i + 1)
                        drR.NumTot = drR.Num
                        drR.Scadenza = .Item("Scadenza")

                        'TODO: ARROTONDAMENTO - la nuova versione lo scrive, la vecchia no, quindi sulla vecchia il calcolo è inevitabile ==> DONE
                        If CDec(.Item("Importo")) <> 0 Then
                            importoPagamento = ArrotondaVal_2(CDec(.Item("Importo")))
                        Else
                            importoPagamento = ArrotondaVal_2(CDec(.Item("Percentuale")) * CDec(.Item("Num_Protocollo")) / 100)
                        End If

                        drR.Importo = Format(importoPagamento, "#,###,##0.00")

                        importoTotalePagamento += importoPagamento

                        drR.ImportoTotale = Format(importoTotalePagamento, "#,###,##0.00")


                        IBAN_Cliente &= Contabilita.Costruisci_IBAN(.Item("Nazione_AVERE"), .Item("Cifre_Controllo_AVERE"), .Item("Cin_AVERE"),
                                                                    .Item("Abi_AVERE"), .Item("Cab_AVERE"), .Item("Numero_AVERE"), True) & " "

                        If .Item("Bic_AVERE") <> "" Then
                            IBAN_Cliente &= " Bic/Swift: " & .Item("Bic_AVERE")
                        End If

                        drR.IBAN_Cliente = IBAN_Cliente

                        If .Item("Filiale_AVERE") <> 0 Then
                            drR.Filiale_Cliente = ", Filiale n." & CStr(.Item("Filiale_AVERE"))
                        Else
                            drR.Filiale_Cliente = ""
                        End If

                        drR.Banca_Cliente = drR.IBAN_Cliente & " " & .Item("Istituto_Des_AVERE") & drR.Filiale_Cliente


                        drR.Codcontatto_Cliente = .Item("Cod_Contatto")

                        If .Item("Rag_Soc_Contatto") <> "" Then
                            drR.RagSoc_Cliente = .Item("Rag_Soc_Contatto")
                        Else
                            drR.RagSoc_Cliente = .Item("cognome") & " " & .Item("Nome")
                        End If

                        indirizzoCliente = .Item("ind_des") & " " & .Item("frz_des") & " " & .Item("CAP") & " " & .Item("com_des") & " (" & .Item("comuni_prov") & ")"
                        drR.Indirizzo_Cliente = indirizzoCliente

                        drR.Dati_Cliente = drR.Codcontatto_Cliente & ", " & drR.RagSoc_Cliente & ", " & indirizzoCliente

                        Select Case .Item("Lav_Cod")

                            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                                LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA
                                rifDocumento = "Fattura n." & .Item("Doc_Numero_Sin") & CStr(.Item("Doc_Numero")) & .Item("Doc_Numero_Des") &
                                               " del " & .Item("Data_Movimento")
                            Case LAVCOD_RICEVUTA_EMESSA
                                rifDocumento = "Ricevuta Fiscale n." & .Item("Doc_Numero_Sin") & CStr(.Item("Doc_Numero")) & .Item("Doc_Numero_Des") &
                                               " del " & .Item("Data_Movimento")
                        End Select

                        drR.Rif_Documento = rifDocumento

                        dsRiba.DS_RegistroRiBa.Rows.Add(drR)

                    End With

                Catch ex As Exception
                    logErrori &= "Dettaglio " & CStr(i + 1) & ": " & vbCrLf & ex.Message & vbCrLf
                End Try

            Next

        End If

    End Sub


    '############################################################################
    Public Function UsaNuoviArrotondamenti(ByRef objParametriServer As AgronicaCoreParametri) As Boolean
        Dim objConfigSitiR As New Configurazione_Siti_R
        Dim val As String = objConfigSitiR.Recupera_Valore_ByChiave(Enum_SiteRedirector.Sito_AgronicaStampe_2010, "Flag_Stampe_Nuovi_Arrotondamenti", objParametriServer)

        If val = "true" Then
            Return True
        Else
            Return False
        End If

    End Function



    '############################################################################
    'questa funzione è stata messa qui, invece che nel core AgronicaCoreStampeDAL.Traduzioni_Stampe
    'perchè, facendo una chiamata a AgronicaCoreContabDAL, si sarebbe generato riferimento circolare
    Public Function OttieniLingua_ReportContabilita(ByRef objParametriServer As AgronicaCoreParametri,
                                                     ByRef objParametriUtenti As AgronicaCoreParametri,
                                                     ByVal lav_Cod As Integer,
                                                     ByVal piva As String,
                                                     ByVal idAgenda As Integer) As String

        '------ GESTIONE TRADUZIONE LAYOUT -------------

        '1) leggere l'impostazione che dice da dove ricavare la lingua
        '(enum_Impostazioni_Utenti.Utente_Lingua_Stampa = 200):
        '“” = al momento usa l'impostazione 1 come default
        '0 = nessuna impostazione (il documento verrà in IT)
        '1= usa meccanismo del cessionario diverso / cessionario
        '“Codice ISO della lingua” = stampa tutti i documenti in questa lingua

        Dim objUtentiImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim ModalitaLinguaStampe As String = objUtentiImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.Utente_Lingua_Stampa, objParametriUtenti)
        Dim flag_UsaCessionario As Boolean = True
        Dim codiceLingua As String

        Select Case ModalitaLinguaStampe
            Case ""
                flag_UsaCessionario = True
            Case "0"
                flag_UsaCessionario = False
            Case "1"
                flag_UsaCessionario = True
            Case Else
                flag_UsaCessionario = False
        End Select

        If flag_UsaCessionario = True Then

            Select Case lav_Cod
            'LAVCOD_NOTA_ACCREDITO_EMESSA
                Case LAVCOD_BOLLA_EMESSA, LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                    LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_FATTURA_PROFORMA, LAVCOD_FATTURA_EMESSA

                    Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
                    Dim cod_Indirizzo As Integer
                    Dim cod_risUm As Integer

                    cod_risUm = objMovimenti.Leggi_CodRisUm_CodIndirizzoRisUm_Cessionario_From_idAgenda(piva, idAgenda, "",
                                                                                                cod_Indirizzo,
                                                                                                objParametriServer)

                    Dim cod_Indirizzo_Destinazione As Integer
                    Dim cod_risUm_Destinazione As Integer = objMovimenti.Leggi_CodRisUm_CodIndirizzoRisUm_CessionarioDiverso_From_idAgenda(piva, idAgenda, "",
                                                                                                                                           cod_Indirizzo_Destinazione,
                                                                                                                                           objParametriServer)

                    Dim objContIndir As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R

                    If cod_risUm_Destinazione <> 0 Then
                        'sulla fattura è impostata una destinazione diversa

                        'la lingua  si prende dal cessionario diverso
                        codiceLingua = objContIndir.CodiceLingua_from_CodRisUm_o_CodIndirizzoRisUm(cod_risUm_Destinazione, cod_Indirizzo_Destinazione, "", "", objParametriServer)
                        If codiceLingua = "" Then
                            codiceLingua = "IT"
                        End If
                    Else
                        'sulla fattura non c'è destinazione diversa

                        'la lingua si prende dal cessionario
                        codiceLingua = objContIndir.CodiceLingua_from_CodRisUm_o_CodIndirizzoRisUm(cod_risUm, cod_Indirizzo, "", "", objParametriServer)
                        If codiceLingua = "" Then
                            codiceLingua = "IT"
                        End If

                    End If

                Case Else
                    codiceLingua = "IT"
            End Select

        ElseIf ModalitaLinguaStampe <> "0" Then
            'MODALITA' NON CESSIONARIO -> USA LA LINGUA SELEZIONATA PER LA STAMPA DI TUTTI I DOCUMENTI
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            'verifico che esista la lingua (e che non sia quindi impostato un codice ISO che non è presente in tabella)
            Dim flag_EsisteLingua As Boolean = objUtenti.Esiste_Lingua(ModalitaLinguaStampe, objParametriUtenti)
            If flag_EsisteLingua = True Then
                codiceLingua = ModalitaLinguaStampe
            Else
                codiceLingua = "IT"
            End If
        Else
            codiceLingua = "IT"
        End If

        Return codiceLingua

    End Function







End Module

