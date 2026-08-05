Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreGisBIZ
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Budget_Reg_Impianto_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi_Impianti_Anagrafica(ByVal Id_Budget As Integer,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal IdReg As Integer,
                                              ByVal Leggi_Distinte As Boolean,
                                              ByVal data As Date,
                                              ByVal filtroData As Boolean,
                                              ByVal Leggi_Cartografia As Boolean,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

        Dim impianti As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

        If Id_Budget <> 0 AndAlso Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 Then
            Dim objImpianti As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
            Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
            Dim appFine As Date = objParametri_Server.FinestraTemporaleFine

            If filtroData Then
                objParametri_Server.FinestraTemporaleInizio = data
                objParametri_Server.FinestraTemporaleFine = data
            Else
                objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
                objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
            End If


            Dim DT_Impianti = objImpianti.Leggi(Id_Budget, Piva, Sa_Cod, Appezza, IdReg,
                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                "", " Budget_Reg_Impianti.Validita_Inizio ASC ",
                                                objParametri_Server)

            objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
            objParametri_Server.FinestraTemporaleFine = CDate(appFine)
            Dim leggiStaticMap As Boolean = False
            Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)

            If jSonStaticMapCFG <> "" Then
                Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
                If StaticMapCFG.StaticMapAttive Then
                    leggiStaticMap = True
                End If
            End If


            For Each row In DT_Impianti.Rows
                Dim imp = Leggi_Impianto_Anagrafica(row("Id_Budget"),
                                                    row("piva"),
                                                    row("sa_cod"),
                                                    row("appezza"),
                                                    row("Id_Reg"),
                                                    Leggi_Distinte,
                                                    leggiStaticMap,
                                                    data,
                                                    filtroData,
                                                    Leggi_Cartografia,
                                                    objParametri_Super_Server,
                                                    objParametri_Server,
                                                    objParametri_Utenti)
                impianti.Add(imp)
            Next


        End If

        Return impianti
    End Function

    Public Function Leggi_Impianto_Anagrafica(ByVal Id_Budget As Integer,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal Id_Reg As Integer,
                                              ByVal Leggi_Distinte As Boolean,
                                              ByVal LeggiStaticMap As Boolean,
                                              ByVal data As Date,
                                              ByVal filtroData As Boolean,
                                              ByVal Leggi_Cartografia As Boolean,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Impianto

        Dim impianto As New AgronicaCoreModelsSTD.anagrafiche.Impianto(
            New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(Id_Reg,
                                                              New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(Appezza,
                                                                                                                    New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                                                                                                                    )
                                                              )
            )

        If Id_Budget <> 0 AndAlso Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 Then
            Dim obj_imp_cod As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_R

            Dim dtcodice = obj_imp_cod.Leggi(Id_Budget, Piva, Sa_Cod, Appezza, Id_Reg, "", enum_CodiciAnagrafe.Codice_Impianto, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dtcodice.Rows.Count > 0 Then
                impianto.codiceImpianto = dtcodice.Rows(0).Item("val_cod")
            End If

            Dim objRegImpianti As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
            Dim DtRegImpianti As DataTable

            Dim objCodici As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_R

            DtRegImpianti = objRegImpianti.Leggi(Id_Budget, Piva, Sa_Cod, Appezza, Id_Reg, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server, LeggiStaticMap:=LeggiStaticMap)

            If DtRegImpianti IsNot Nothing AndAlso DtRegImpianti.Rows.Count > 0 Then

                ''' TODO CONSOCIAZIONE
                'impianto.Id_Consociazione = CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione"))
                If CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione")) > 0 Then
                    impianto.consociazionePK = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione")),
                                                                           New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK() With {
                                                                                .codice = 0,
                                                                                .centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK() With {
                                                                                        .codice = 0,
                                                                                        .partitaIva = ""
                                                                                    }
                                                                                }
                                                                           )
                    impianto.impiantoConsociato = True
                Else
                    impianto.impiantoConsociato = False
                End If



                impianto.superficie = DtRegImpianti.Rows(0).Item("sup_imp")

                impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DtRegImpianti.Rows(0).Item("Validita_Inizio"), DtRegImpianti.Rows(0).Item("Validita_Fine"))

                impianto.data_Inizio_Produzione = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")) Then
                    impianto.data_Inizio_Produzione = DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")
                End If

                impianto.data_Innesto_Varieta = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")) Then
                    impianto.data_Innesto_Varieta = DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")
                End If

                impianto.data_Inizio_Portinnesto = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("data_Inizio_Portinnesto")) Then
                    impianto.data_Inizio_Portinnesto = DtRegImpianti.Rows(0).Item("data_Inizio_Portinnesto")
                End If

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Piante_Maschi_InSesto")) Then
                    impianto.maschi_in_Sesto = DtRegImpianti.Rows(0).Item("Piante_Maschi_InSesto")
                End If

                impianto.data_Inizio_Impianto = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Impianto")) Then
                    impianto.data_Inizio_Impianto = DtRegImpianti.Rows(0).Item("Data_Inizio_Impianto")
                End If

                If LeggiStaticMap Then
                    impianto.immagineBase64 = DtRegImpianti.Rows(0)("StaticMapBase64String")
                End If

                If Leggi_Cartografia AndAlso DtRegImpianti.Columns.Contains("cartografia") Then
                    impianto.cartografia = DtRegImpianti.Rows(0)("cartografia")
                End If


                If DtRegImpianti.Rows(0).Item("Cul_cod") <> 0 Then
                    impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(DtRegImpianti.Rows(0).Item("Cul_cod")) With {
                        .descrizione = DtRegImpianti.Rows(0).Item("Cul_Des"),
                        .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(DtRegImpianti.Rows(0).Item("Veg_Cod")) With {
                            .descrizione = DtRegImpianti.Rows(0).Item("Veg_Des")
                        }
                    }
                End If
                If DtRegImpianti.Rows(0).Item("cover") = 0 Then
                    impianto.cover_Crops = False
                Else
                    impianto.cover_Crops = True
                End If

                If DtRegImpianti.Rows(0).Item("monitorato") = 0 Then
                    impianto.monitorato = False
                Else
                    impianto.monitorato = True
                End If

                impianto.copertura = New AgronicaCoreModelsSTD.metaschema.Copertura(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_cod")) Then
                    impianto.copertura = New AgronicaCoreModelsSTD.metaschema.Copertura(DtRegImpianti.Rows(0).Item("cop_cod")) With {.descrizione = DtRegImpianti.Rows(0).Item("cop_des")}
                End If

                impianto.cop_Data_Inizio = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_di")) Then
                    impianto.cop_Data_Inizio = DtRegImpianti.Rows(0).Item("cop_di")
                End If

                impianto.cop_Data_Fine = AGRODATAFINE
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_df")) Then
                    impianto.cop_Data_Fine = DtRegImpianti.Rows(0).Item("cop_df")
                End If

                impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("-1") With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("setup_cod")) Then
                    Select Case CStr(DtRegImpianti.Rows(0).Item("setup_cod"))
                        Case "Trapiantato"
                            impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("Trapiantato") With {.descrizione = "Trapiantato"}
                        Case "Seminato"
                            impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("Seminato") With {.descrizione = "Seminato"}
                        Case Else
                            impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("") With {.descrizione = ""}
                    End Select
                End If

                impianto.tecnicaConduzioneTraFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("tecn_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("tecn_cod")) > 0 Then
                    Dim objConduzioneTraFila As New AgronicaCoreMetaSchemaDAL.ConduzioneTraFila_R
                    Dim dt = objConduzioneTraFila.Leggi(DtRegImpianti.Rows(0).Item("tecn_cod"), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    impianto.tecnicaConduzioneTraFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila(dt.Rows(0).Item("tecn_cod")) With {.descrizione = dt.Rows(0).Item("tecn_des")}
                End If

                impianto.tecnicaConduzioneSuFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("su_cod")) Then
                    Dim objConduzioneSuFila As New AgronicaCoreMetaSchemaDAL.ConduzioneSuFila_R
                    Dim dt = objConduzioneSuFila.Leggi(DtRegImpianti.Rows(0).Item("su_cod"), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    impianto.tecnicaConduzioneSuFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila(dt.Rows(0).Item("tecn_cod")) With {.descrizione = dt.Rows(0).Item("tecn_des")}
                End If

                impianto.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Grfi_cod")) Then
                    impianto.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(DtRegImpianti.Rows(0).Item("Grfi_cod")) With {.descrizione = DtRegImpianti.Rows(0).Item("Grfi_des")}
                    If DtRegImpianti.Rows(0).Item("Cul_cod") <> 0 Then
                        impianto.gruppoFinalita.specieCod = CType(impianto.utilizzoTerreno, Varieta).specie.codice
                    End If
                End If

                'Imp_Des
                impianto.irrigazione = New AgronicaCoreModelsSTD.metaschema.Irrigazione(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Imp_Cod")) Then
                    impianto.irrigazione = New AgronicaCoreModelsSTD.metaschema.Irrigazione(DtRegImpianti.Rows(0).Item("Imp_Cod")) With {.descrizione = DtRegImpianti.Rows(0).Item("Imp_Des")}
                End If

                ' macchina irrigazione impianto
                impianto.macchineIrrigazione = New List(Of ParcoMacchine)
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("flagImpiantoIsMacchina")) AndAlso CInt(DtRegImpianti.Rows(0).Item("flagImpiantoIsMacchina")) = 1 Then
                    Dim objImpiantiMacchine As New AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R
                    Dim dtImpiantiMacchine = objImpiantiMacchine.ReadJoinDescriptions_Budget(objParametri_Server, Id_Budget, Piva, Sa_Cod, Appezza, Id_Reg)
                    If dtImpiantiMacchine IsNot Nothing AndAlso dtImpiantiMacchine.Rows.Count > 0 Then
                        impianto.macchineIrrigazione.Add(
                            New ParcoMacchine With {
                                .codice = dtImpiantiMacchine.Rows(0).Item("Mac_Cod"),
                                .descrizione = dtImpiantiMacchine.Rows(0).Item("Mac_Des")
                            }
                        )
                    End If
                End If

                impianto.gruppoVarietale = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale(DtRegImpianti.Rows(0).Item("grva_cod_veg"), DtRegImpianti.Rows(0).Item("Grva_Des"))

                impianto.portinnesto = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("port_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("port_cod")) > 0 Then
                    Dim objPortinnesti As New AgronicaCoreMetaSchemaDAL.Portinnesti
                    Dim dt = objPortinnesti.Leggi(DtRegImpianti.Rows(0).Item("port_cod"), "", "", objParametri_Server)
                    impianto.portinnesto = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(DtRegImpianti.Rows(0).Item("port_cod")) With {.descrizione = dt.Rows(0).Item("port_des")}
                ElseIf Not IsDBNull(DtRegImpianti.Rows(0).Item("port_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("port_cod")) < 1 Then
                    impianto.portinnesto = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(DtRegImpianti.Rows(0).Item("port_cod")) With {.descrizione = ""}
                End If

                impianto.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("foral_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("foral_cod")) > 0 Then
                    Dim objFormaAllevamento As New AgronicaCoreMetaSchemaDAL.FormeAllevamento
                    Dim dt = objFormaAllevamento.Leggi(DtRegImpianti.Rows(0).Item("foral_cod"), "", "", objParametri_Server)
                    impianto.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(dt.Rows(0).Item("foral_cod")) With {.descrizione = dt.Rows(0).Item("foral_des")}
                End If

                Dim provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(0) With {.descrizione = ""}
                Select Case DtRegImpianti.Rows(0).Item("ProvenienzaSeme")
                    Case 0
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = ""
                        }
                    Case 1
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = "Biologica"
                        }
                    Case 2
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = "Integrato"
                        }
                    Case 3
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = "Deroga"
                        }
                End Select

                impianto.unita_Vitata = 0
                If Not IsNothing(DtRegImpianti.Rows(0).Item("Unita_Vitata")) Then
                    impianto.unita_Vitata = DtRegImpianti.Rows(0).Item("Unita_Vitata")
                End If

                Dim unitaMisuraAlternativa = 0
                Dim unitaMisuraAlternativa_Des = ""
                Dim tasso_conv As Double = 0
                impianto.superficieAlternativa = IIf(IsDBNull(DtRegImpianti.Rows(0).Item("SUP_ALT")), 0, DtRegImpianti.Rows(0).Item("SUP_ALT"))

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("UDM_COD_ALT")) Then
                    unitaMisuraAlternativa = DtRegImpianti.Rows(0).Item("UDM_COD_ALT")
                    unitaMisuraAlternativa_Des = DtRegImpianti.Rows(0).Item("UDM_DES_ALT")
                    tasso_conv = DtRegImpianti.Rows(0).Item("tasso_conv")
                End If
                impianto.unitaMisuraAlternativa = New UnitaDiMisura_Alternativa(unitaMisuraAlternativa) With {
                    .descrizione = unitaMisuraAlternativa_Des,
                    .tassoConversione = tasso_conv
                }


            End If

            Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            Dim StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
            'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
            'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
            'perchè già presenti
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso) & " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati) & " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente) & " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " Or ", "")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase) & " Or ", "")
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) & " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto) & " Or ", "")


            Dim FiltroLetturaCodici As String = ""

            FiltroLetturaCodici &= " ( " &
                " Budget_Reg_Impianti_Codici.Id_Cod NOT IN ( " &
                CStr(enum_CodiciAnagrafe.Codice_Specie_Agea) & ", " &
                CStr(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_1) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_2) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_3) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_4) & " " &
                "   ) " &
                " )"

            FiltroLetturaCodici &= " AND Budget_Reg_Impianti_Codici.progetto_cod = 0  " &
                                " AND (Budget_Reg_Impianti_Codici.id_cod < 2000 OR Budget_Reg_Impianti_Codici.id_cod >= 3000 ) "


            Dim DtCodici = objCodici.Leggi(Id_Budget,
                                    Piva,
                                    Sa_Cod,
                                    Appezza,
                                    Id_Reg,
                                    "",
                                    0,
                                    "",
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    FiltroLetturaCodici,
                                    "",
                                    objParametri_Server)

            Dim StrCodici2 As String = ""

            Dim Codici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

            impianto.impianto_Ibrido = False
            impianto.codBMBDBT_M = ""
            impianto.codBMBDBT_F = ""
            impianto.genetica_M = ""
            impianto.genetica_F = ""
            impianto.offType_M = ""
            impianto.offType_F = ""
            impianto.tra_Fila_M = 0
            impianto.distanzaTraFila_F = 0
            impianto.su_Fila_M = 0
            impianto.distanzaSuFila_F = 0
            impianto.interbina = 0
            impianto.germinabilita = 0
            impianto.codiceZona = New baseClass.BaseCodeDescrStr("", "")
            impianto.tagliatoIntero = New AgronicaCoreModelsSTD.metaschema.TagliatoIntero("") With {.descrizione = ""}
            impianto.partiTuberi = 0
            impianto.dettaglio_varieta_personalizzato = New AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato(0) With {.descrizione = ""}

            If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then

                For i = 0 To DtCodici.Rows.Count - 1

                    'If (Not IsDBNull(Dr(i).Item("val_cod"))) Then
                    If (Not IsDBNull(DtCodici.Rows(i).Item("val_cod"))) Then

                        Dim Id_Cod = DtCodici.Rows(i).Item("id_cod")
                        Dim Val_Cod = DtCodici.Rows(i).Item("val_cod")
                        Dim CodiceAnagrafeDes = DtCodici.Rows(i).Item("descrizione")

                        Select Case Id_Cod

                            Case enum_CodiciAnagrafe.Impianto_Ibrido
                                impianto.impianto_Ibrido = False
                                If DtRegImpianti IsNot Nothing AndAlso DtRegImpianti.Rows.Count > 0 AndAlso
                                   DtRegImpianti.Rows(0).Item("grva_cod_veg") < 0 Then
                                    impianto.impianto_Ibrido = True
                                End If
                            Case enum_CodiciAnagrafe.Impianto_CodiceB_Maschio
                                impianto.codBMBDBT_M = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_CodiceB_Femmina
                                impianto.codBMBDBT_F = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_Genetica_Maschio
                                impianto.genetica_M = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_Genetica_Femmina
                                impianto.genetica_F = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_OffType_Maschio
                                impianto.offType_M = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_OffType_Femmina
                                impianto.offType_F = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                                If Val_Cod <> "" Then
                                    impianto.tra_Fila_M = Val_Cod
                                End If

                            Case enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                                If Val_Cod <> "" Then
                                    impianto.distanzaTraFila_F = Val_Cod
                                End If

                            Case enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                                If Val_Cod <> "" Then
                                    impianto.su_Fila_M = Val_Cod
                                End If

                            Case enum_CodiciAnagrafe.Impianto_SuFila_Femmina
                                If Val_Cod <> "" Then
                                    impianto.distanzaSuFila_F = Val_Cod
                                End If

                            Case enum_CodiciAnagrafe.Impianto_Interbina
                                If Val_Cod <> "0" And Val_Cod <> "" Then
                                    impianto.interbina = Val_Cod
                                End If

                            Case enum_CodiciAnagrafe.Impianto_Germinabilita
                                impianto.germinabilita = Val_Cod

                            Case enum_CodiciAnagrafe.CodiceZona
                                If IsNumeric(Val_Cod) Then

                                    Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                    Dim dtProdotto = objMateriePrime.Leggi(Piva, 0, 0, Val_Cod, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                                    If dtProdotto.Rows.Count > 0 Then
                                        impianto.codiceZona = New baseClass.BaseCodeDescrStr(Val_Cod, dtProdotto(0)("Mat_Des"))
                                    End If

                                End If

                            Case 3000 To 3999

                                If impianto.utilizzoTerreno IsNot Nothing Then
                                    Throw New Exception("Impianto con Utilizzo Terreno " & CStr(Id_Cod) & " - " & CodiceAnagrafeDes &
                                                        " e Cultivar " & CStr(impianto.utilizzoTerreno.descrizione))
                                End If

                                impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(Id_Cod) With {
                                    .descrizione = CodiceAnagrafeDes
                                }
                                impianto.utilizzoTerreno.classType = ClassType.DestinazioneUso

                            Case enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate

                                impianto.tagliatoIntero.codice = Val_Cod

                            Case enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate

                                impianto.partiTuberi = Val_Cod

                            Case enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato

                                'If IsNumeric(Val_Cod) AndAlso Val_Cod <> 0 Then
                                If Val_Cod IsNot Nothing AndAlso Val_Cod <> "" Then
                                    Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

                                    Dim Dt = objCAC.Leggi(2, Val_Cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                                    If Dt.Rows.Count > 0 Then
                                        impianto.dettaglio_varieta_personalizzato = New AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato(Val_Cod) With {
                                            .descrizione = Dt.Rows(0)("InfoAgg_Des")
                                        }
                                    End If
                                End If

                            Case Else

                                'se nella stringa contenente i codici restituita dal componente è presente..
                                If InStr(StrCodiciImpianto, Id_Cod.ToString) <> 0 Then

                                    Dim Codice As New AnagrafeNG.Codici

                                    Codice.Id_Cod = Id_Cod
                                    Codice.Descrizione = CodiceAnagrafeDes
                                    Codice.Val_Cod = Val_Cod
                                    Codice.Validita_Inizio = DtCodici.Rows(i).Item("Validita_Inizio")
                                    Codice.Validita_Fine = DtCodici.Rows(i).Item("Validita_Fine")

                                    Codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                                    .valore = Val_Cod,
                                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(
                                                                DtCodici.Rows(i).Item("Validita_Inizio"),
                                                                DtCodici.Rows(i).Item("Validita_Fine")),
                                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(Id_Cod) With {
                                                        .descrizione = CodiceAnagrafeDes
                                                    }
                                                })
                                End If
                        End Select
                    End If
                Next

                impianto.codici = Codici

            End If

            If impianto.utilizzoTerreno Is Nothing Then
                impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(-1) With {
                                    .descrizione = Str_TerrenoNudo
                                }
            End If

            If Leggi_Distinte Then
                Dim objProgetti_R As New Budget_Progetto_R
                Dim specie = New Specie(0)
                If (impianto.utilizzoTerreno IsNot Nothing) Then
                    If (impianto.utilizzoTerreno.classType = ClassType.Varieta) Then
                        specie = CType(impianto.utilizzoTerreno, Varieta).specie
                    End If
                End If
                impianto.esercizi = objProgetti_R.Leggi_Esercizi_Anagrafica(Id_Budget,
                                                                            Piva,
                                                                            Sa_Cod,
                                                                            Appezza,
                                                                            Id_Reg,
                                                                            specie,
                                                                            data,
                                                                            filtroData,
                                                                            objParametri_Super_Server,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti)


            End If
        End If

        Return impianto
    End Function

    Public Function Leggi_Esercizi_Anagrafica(ByVal Id_Budget As Integer,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod As Long,
                                              ByVal Appezza As Long,
                                              ByVal Id_Reg As Long,
                                              ByVal Campo_Cod As Long,
                                              ByVal Data_Filtro As Date,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri)
        Dim objImprese_Progetti As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R
        Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
        Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

        If Data_Filtro <> AGRODATAINIZIO Then
            objParametri_Server.FinestraTemporaleInizio = Data_Filtro
            objParametri_Server.FinestraTemporaleFine = Data_Filtro
        End If

        Dim leggiStaticMap As Boolean = False
        Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)

        If jSonStaticMapCFG <> "" Then
            Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
            If StaticMapCFG.StaticMapAttive Then
                leggiStaticMap = True
            End If
        End If

        Dim budgetPianoSemina As Boolean = False

        Dim objBudget_Testata As New AgronicaCoreBudgetDAL.Budget_Testata_R
        Dim dtBudget = objBudget_Testata.leggiElencoBdgTestata("", "", objParametri_Server, "", True, 1, Id_Budget)
        If (dtBudget.Rows.Count > 0) Then
            budgetPianoSemina = True
        End If

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim leggiDatiRibaltamento As Boolean = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                     enum_Id_Servizio.GiasOnline, enum_Security_Attivita.Budget_Ribaltamento_Su_Reale, enum_Security_Operazione.Lettura,
                                                                                     Date.Now, "", objParametri_Utenti)

        Dim dt = objImprese_Progetti.Leggi_x_anagraficaNG(Id_Budget, Piva, Sa_Cod, Appezza, Id_Reg, Campo_Cod, "", "", objParametri_Server, Date.Now, leggiStaticMap, leggiDatiRibaltamento, budgetPianoSemina)

        If Data_Filtro <> AGRODATAINIZIO Then
            objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
            objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
        End If

        Return dt
    End Function

    Public Function controllo_MovimentiProgrammazionexEliminazione(id_budget As Integer,
                                                                   piva As String,
                                                                   sa_cod As Integer,
                                                                   appezza As Integer,
                                                                   id_reg As Integer,
                                                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Reg_Impianto.controllo_MovimentiProgrammazione()"
        Dim dtProgrammazione As New DataTable
        Dim objProgrammazione As New Programmazione_Entita_R

        Try
            dtProgrammazione = objProgrammazione.LeggiPrenotazionePianteBudget(id_budget, piva, sa_cod, appezza, id_reg,
                                                                               "", "",
                                                                               objParametri)

            If dtProgrammazione.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function
End Class
Public Class Budget_Reg_Impianto_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function controllo_MovimentiRicettexEliminazione(DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                               piva As String,
                                                               sa_cod As Integer,
                                                               appezza As Integer,
                                                               id_reg As Integer,
                                                               ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "controllo_MovimentiRicettexEliminazione"
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Try
            dtAgenda = objDestR.LeggiCronologiaMovimenti(piva, sa_cod, appezza, id_reg,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                     "",
                                                     objParametri)

            dtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                        piva, sa_cod, appezza, id_reg,
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        objParametri)

            If dtAgenda.Rows.Count > 0 OrElse dtRicette.Rows.Count > 0 Then
                'Dim appezza_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                'Dim dtAppezza = appezza_R.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                Dim descrizioneImpianto = ""
                If DatiImpianto.utilizzoTerreno IsNot Nothing Then
                    If DatiImpianto.utilizzoTerreno.classType = ClassType.DestinazioneUso Then
                        descrizioneImpianto = DatiImpianto.utilizzoTerreno.descrizione
                    Else
                        Dim varieta = CType(DatiImpianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                        descrizioneImpianto = varieta.specie.descrizione & " - " & varieta.descrizione
                    End If

                    descrizioneImpianto &= " [" & DatiImpianto.superficie & "ha] "
                End If
                Return True
            End If

            Return False

            dtAgenda.Dispose()
            dtAgenda = Nothing

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Function controllo_CancellazioneVincoliLetamazioniPUAxEliminazione(DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                               piva As String,
                                                               sa_cod As Integer,
                                                               appezza As Integer,
                                                               id_reg As Integer,
                                                               ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "controllo_CancellazioneVincoliLetamazioniPUAxEliminazione"

        Try

            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

            'DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
            Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
            Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0, piva, sa_cod, appezza, id_reg,
                                                                           0, 0, 0, 0, "", "", objParametri)

            For Each vincolo In dtAnagrafe_Vincoli.Rows
                If vincolo("Progetto_Cod") = 0 Then
                    objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
                End If
            Next


            Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
            Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

            Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0, 0,
                                                                         piva, sa_cod, appezza, id_reg,
                                                                         0, "", "", objParametri)

            For Each pualet In dtPUA_Letamazioni.Rows
                If pualet("Progetto_Cod") = 0 Then
                    objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
                End If
            Next

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Sub Verifica_Utilizzo(ByRef Id_Budget As Integer,
                                      ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "Verifica_Utilizzo"
        Dim Validita_Fine = impianto.validita.fine
        Dim Validita_Inizio = impianto.validita.inizio

        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim id_Reg = impianto.primaryKey.codice
        Dim Veg_Cod As Integer = 0
        Select Case impianto.utilizzoTerreno.classType
            Case costanti.ClassType.Varieta
                Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                Veg_Cod = objCultivar.VegCod_from_CulCod(impianto.utilizzoTerreno.codice, objParametri_Server)
        End Select


        Dim objImpiantoR As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
        Dim imp = objImpiantoR.Leggi(Id_Budget, piva, sa_cod, appezza, id_Reg, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

        Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva,
                                                        sa_cod,
                                                        appezza,
                                                        id_Reg,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)

        Try
            If imp.Rows.Count > 0 Then
                If Veg_Cod <> imp.Rows(0)("Veg_Cod") Then
                    If DTAgenda.Rows.Count > 0 Then
                        'Se esistono delle Agende collegate al mio impianto, controllo che non sia coinvolto con altri impianti
                        Dim piuApp As Boolean = False
                        Dim objMovDes As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                        For ag As Integer = 0 To DTAgenda.Rows.Count - 1
                            Dim id_ag As Integer = DTAgenda.Rows(ag).Item("Id_Agenda")
                            Dim DT_dest As DataTable = objMovDes.Leggi_Dettagli_Impianti(piva, id_ag, "", "", objParametri_Server)
                            For Each dest As DataRow In DT_dest.Rows
                                'Se trovo appezzamenti diversi, do errore
                                If CInt(dest.Item("appezza")) <> appezza Then
                                    piuApp = True
                                    Exit For
                                End If
                            Next
                            If piuApp Then
                                Exit For
                            End If
                        Next
                        If Not piuApp Then
                            MessaggioErrore += Gias.AttenzioneImpossibileVariareSpeciePercheAssociatiMovimentiAgendaCoinvolgentiAltriAppezzamenti
                            Throw New GiasException(MessaggioErrore)
                        End If
                    End If
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Public Sub Verifica_Finalita(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "Verifica_Finalita"
        Try
            'Superficie nulla ...
            If impianto.gruppoFinalita.codice = 0 Then
                MessaggioErrore = ("Il campo FINALITA' è da selezionare.")
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Public Function Verifica_ValiditaInizioFine(ByRef Id_Budget As Integer,
                                                ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "Verifica_ValiditaInizioFine"
        Dim Validita_Fine = impianto.validita.fine
        Dim Validita_Inizio = impianto.validita.inizio

        Dim Idbudget = Id_Budget
        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim id_Reg = impianto.primaryKey.codice
        Dim sup_imp = impianto.superficie

        Dim nome_imp As String
        If impianto.descrizione <> "" Then
            nome_imp = " '" & impianto.descrizione & "'"
        Else
            nome_imp = ""
        End If


        Dim objAppezzamentoR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
        Dim appezzamento = objAppezzamentoR.Leggi(Idbudget, piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objImpiantoR As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
        Dim imp = objImpiantoR.Leggi(Idbudget, piva, sa_cod, appezza, id_Reg, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objEsercizioR As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R
        Dim ese = objEsercizioR.LeggiDistinta3(Idbudget, piva, sa_cod, appezza, id_Reg, "", "", objParametri_Server)


        Try

            If imp.Rows.Count > 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, l'Esercizio
                If Validita_Inizio <> imp.Rows(0)("Validita_Inizio") Or Validita_Fine <> imp.Rows(0)("Validita_Fine") Then

                    'controllo_MovimentiRicettePua(impianto, piva, sa_cod, appezza, id_Reg, Validita_Inizio, Validita_Fine, objParametri_Server)

                    If Validita_Inizio < CDate(appezzamento.Rows(0)("Validita_Inizio")) Then
                        MessaggioErrore += ("La data di inizio dell'impianto non può essere inferiore alla data di inizio dell'appezzamento") & " (" & appezzamento.Rows(0)("Validita_Inizio") & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    If Validita_Fine > CDate(appezzamento.Rows(0)("Validita_Fine")) Then
                        MessaggioErrore &= ("La data di fine dell'impianto non può essere superiore alla data di fine dell'appezzamento") & " (" & appezzamento.Rows(0)("Validita_Fine") & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controllo = objControllo.controllo_CdG(Nothing, piva, sa_cod, appezza, id_Reg, 0, Validita_Inizio, Validita_Fine, objParametri_Server, Id_Budget:=Id_Budget)
                    If controllo.errore Then
                        Dim MessaggioErroreCdG As String = ""
                        If nome_imp = "" Then
                            nome_imp = "[dal " & imp.Rows(0)("Validita_Inizio") & " al " & imp.Rows(0)("Validita_Fine") & "]"
                        End If

                        If Not controllo.messaggioSpecifico Then
                            MessaggioErroreCdG &= ("Non è possibile modificare la " & controllo.inizio_fine & " dell'impianto " & nome_imp & ", perché sono stati associati Costi di Gestione ad un esercizio in data successiva a quella selezionata")
                        Else
                            MessaggioErroreCdG &= ("Non è possibile modificare la " & controllo.inizio_fine & " dell'impianto " & nome_imp & ", perché sono stati associati Costi di Gestione ad un esercizio in data " & controllo.dataCdG)
                        End If

                        Throw New GiasException(MessaggioErroreCdG)
                    End If

                End If
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore += ("La fine dell'impianto non può precedere la data di inizio.")
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function
    Public Sub Verifica_Superficie(ByRef Id_budget As Integer,
                                   ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   ByRef NewTransaction As Boolean)

        Dim MessaggioErrore As String = ""

        Dim Validita_Fine = impianto.validita.fine
        Dim Validita_Inizio = impianto.validita.inizio

        Dim idbudget = Id_budget
        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim id_Reg = impianto.primaryKey.codice
        Dim sup_imp = impianto.superficie


        Dim objImpiantoR As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
        Dim imp = objImpiantoR.Leggi(idbudget, piva, sa_cod, appezza, id_Reg, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva,
                                                        sa_cod,
                                                        appezza,
                                                        id_Reg,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)


        Try
            If imp.Rows.Count > 0 Then
                If sup_imp <> imp.Rows(0).Item("sup_imp") And DTAgenda.Rows.Count > 0 Then

                    Dim id_agendas = (From a In GiasContext.Mov_Destinazioni
                                      Where a.Piva = piva AndAlso
                                            a.Sa_Cod = sa_cod AndAlso
                                            a.Appezza = appezza AndAlso
                                            a.Id_Destinazione = id_Reg AndAlso
                                            a.Tipo_Destinazione = 0
                                      Select a.Id_Agenda).Distinct.ToList

                    For Each id_agenda In id_agendas
                        Dim agenda = (From a In GiasContext.Agenda Where a.Id_Agenda = id_agenda).FirstOrDefault

                        If agenda IsNot Nothing Then
                            agenda.Blocco_Flag = 2
                            agenda.Blocco_Username = objParametri_Server.UtenteUsername
                            agenda.Blocco_Data = DateTime.Now
                            GiasContext.Entry(agenda).State = EntityState.Modified
                        End If

                    Next
                    GiasContext.SaveChanges()

                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message.ToString()
            Throw New Exception(MessaggioErrore)
        End Try
    End Sub
    Public Function controllo_MovimentiRicettePua(DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                  piva As String,
                                                  sa_cod As Integer,
                                                  appezza As Integer,
                                                  id_reg As Integer,
                                                  Validita_Inizio As Date,
                                                  Validita_Fine As Date,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  )

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "controllo_MovimentiRicettePua"
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Try
            dtAgenda = objDestR.LeggiCronologiaMovimenti(piva, sa_cod, appezza, id_reg,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     " ( Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                                     "",
                                                     objParametri)

            dtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                        piva, sa_cod, appezza, id_reg,
                                        Validita_Inizio, Validita_Fine,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        objParametri)

            If dtAgenda.Rows.Count > 0 OrElse dtRicette.Rows.Count > 0 Then

                Dim descrizioneImpianto = ""
                If DatiImpianto.utilizzoTerreno.classType = ClassType.DestinazioneUso Then
                    descrizioneImpianto = DatiImpianto.utilizzoTerreno.descrizione
                Else
                    Dim varieta = CType(DatiImpianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                    descrizioneImpianto = varieta.specie.descrizione & " - " & varieta.descrizione
                End If

                descrizioneImpianto &= " [" & DatiImpianto.superficie & "ha] "

                MessaggioErrore += (" Non è possibile modificare/eliminare l'Impianto " & descrizioneImpianto & " perché ci sono registrazioni associate. ")

                Throw New GiasException(MessaggioErrore)
            End If



            dtAgenda.Dispose()
            dtAgenda = Nothing

            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

            ''DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
            'Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
            'Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
            'Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0, piva, sa_cod, appezza, id_reg,
            '                                                               0, 0, 0, 0, "", "", objParametri)

            'For Each vincolo In dtAnagrafe_Vincoli.Rows
            '    If vincolo("Progetto_Cod") = 0 Then
            '        objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
            '        objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
            '    End If
            'Next


            'Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
            'Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

            'Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0, 0,
            '                                                             piva, sa_cod, appezza, id_reg,
            '                                                             0, "", "", objParametri)

            'For Each pualet In dtPUA_Letamazioni.Rows
            '    If pualet("Progetto_Cod") = 0 Then
            '        objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
            '        objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
            '    End If
            'Next

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

End Class