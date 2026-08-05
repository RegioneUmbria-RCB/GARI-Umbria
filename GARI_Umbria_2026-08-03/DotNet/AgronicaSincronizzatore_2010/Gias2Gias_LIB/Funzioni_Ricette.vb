Imports System.Xml
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports AgronicaCoreModello
Imports AgronicaCoreUtility
Imports AgronicaCoreG2GLocalDal

Partial Public Class Funzioni

#Region "OLD"
    Public Sub Elabora_Ricette_XML_Salva(
                                    ByVal objOpzioni As clsOpzioni,
                                    ByRef Log_Import As StringBuilder,
                                    ByRef Log_Errori As StringBuilder,
                                    ByRef Log_Riepilogo As StringBuilder,
                                    ByVal Piva_Origine As String,
                                    ByVal Piva_Destinazione As String,
                                    ByVal Sa_cod_Origine As Integer,
                                    ByVal sa_cod_destinazione As Integer,
                                    ByVal FiltriXlettura_vuoti As Boolean
                            )

        Const nomeFunzione As String = "Elabora_Ricette_Salva"

        Try

            Dim xmlRicetta As String
            Dim leggiRicette As New AgronicaCoreContabBIZ.Ricette_R

            xmlRicetta = leggiRicette.Ricetta_Leggi(
                0,
                Piva_Origine,
                Sa_cod_Origine,
                0,
                0,
                False,
                objOpzioni.objParametri_Server_GIAS_ORIGINE,
                FiltriXlettura_vuoti
            )

            Dim docRicette As New XmlDocument

            If xmlRicetta <> "" Then
                docRicette.LoadXml(xmlRicetta)

                Dim scriviRicetta As New AgronicaCoreContabBIZ.Ricette_W
                For Each xRicetta As XmlNode In docRicette.SelectNodes("//DatiRicetta/Ricetta")
                    Dim ricetta_cod As Integer
                    scriviRicetta.Ricetta_Scrivi(
                        "<DatiRicetta>" & Elabora_Ricette_XML_elabora(
                            objOpzioni,
                            Log_Import,
                            Log_Errori,
                            Log_Riepilogo,
                            xRicetta.OuterXml,
                            Piva_Origine,
                            Piva_Destinazione,
                            Sa_cod_Origine,
                            sa_cod_destinazione
                        ) & "</DatiRicetta>",
                        ricetta_cod,
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                    )

                Next

            End If

        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try


    End Sub



    Public Function Elabora_Ricette_XML_elabora(
                ByVal objOpzioni As clsOpzioni,
                ByRef Log_Import As StringBuilder,
                ByRef Log_Errori As StringBuilder,
                ByRef Log_Riepilogo As StringBuilder,
                ByRef Str_XML_Agenda As String,
                ByVal Piva_Origine As String,
                ByVal Piva_Destinazione As String,
                ByVal sa_cod_origine As Integer,
                ByVal sa_cod_destinazione As Integer
            ) As String

        Const NomeFunzione As String = "Elabora_XML_Agenda_SistemaXml"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Agenda)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement



        Try

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ricetta_dettaglio_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("ricetta_dettaglio_cod", 0)
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ricetta_destinazione_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("ricetta_destinazione_cod", 0)
            Next


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ricetta_tecnico_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("ricetta_tecnico_cod", 0)
            Next


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ricetta_operazione_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("ricetta_operazione_cod", 0)
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@piva]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
            Next


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@sa_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("sa_cod", sa_cod_destinazione)
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ricetta_superuser]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("ricetta_superuser", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ricetta_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("ricetta_cod", 0)
            Next


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@programmazione_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("programmazione_cod",
                        recode_dammi_programmazione_cod(
                            objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            XML_Nodo.Attributes("programmazione_cod").Value
                            )
                        )
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@programmazione_entita_cod]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("programmazione_entita_cod",
                        recode_dammi_programmazione_entita_cod(
                            objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            XML_Nodo.Attributes("programmazione_entita_cod").Value
                            )
                        )
            Next


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_reg]")
            For Each XML_Nodo In XMLs_Nodi

                Dim _To_Appezza As Integer
                Dim _To_Id_reg As Integer


                Dim impiantonew = Nothing

                If NuovaLogicaRecodePC Then
                    impiantonew = (From m In _efG2G.G2G_Recode_Impianti
                                   Where m.From_Piva = Piva_Origine _
                                    And m.From_Sa_Cod = sa_cod_origine _
                                    And m.From_Appezza = CInt(XML_Nodo.Attributes("appezza").Value) _
                                    And m.From_Id_Reg = CInt(XML_Nodo.Attributes("id_reg").Value)
                                   Select m.To_Appezza, m.To_Id_Reg).FirstOrDefault
                Else
                    impiantonew = (From m In _ImpiantiMappati
                                   Where m.From_Piva = Piva_Origine _
                                    And m.From_Sa_Cod = sa_cod_origine _
                                    And m.From_Appezza = CInt(XML_Nodo.Attributes("appezza").Value) _
                                    And m.From_Id_Reg = CInt(XML_Nodo.Attributes("id_reg").Value)
                                   Select m.To_Appezza, m.To_Id_Reg).FirstOrDefault
                End If


                If CInt(XML_Nodo.Attributes("appezza").Value) = 0 Then
                    _To_Appezza = 0
                Else
                    _To_Appezza = impiantonew.To_Appezza
                End If

                If CInt(XML_Nodo.Attributes("id_reg").Value) = 0 Then
                    _To_Id_reg = 0
                Else
                    _To_Id_reg = impiantonew.To_Id_Reg
                End If

                XML_Nodo.SetAttribute("appezza", _To_Appezza)
                XML_Nodo.SetAttribute("id_destinazione", _To_Id_reg)

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_agenda]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("id_agenda",
                        recode_dammi_id_agenda(
                            Piva_Origine,
                            XML_Nodo.Attributes("id_agenda").Value
                            )
                        )

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_note]")
            For Each XML_Nodo In XMLs_Nodi
                XML_Nodo.SetAttribute("id_note",
                        FunzioniGLOBAL.recode_return_NotaCod(
                            objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            XML_Nodo.Attributes("id_note").Value
                            )
                        )

            Next


            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function


    Private Function recode_dammi_programmazione_cod(ByVal old_pivaSuerUser As String, ByVal old_programmazione_cod As Integer) As String
        Return (
            From pl In _funzioniGLOBAL.Planning
            Where pl.From_PivaSuperUser = old_pivaSuerUser _
            And pl.From_Programmazione_Cod = old_programmazione_cod _
            And pl.From_Programmazione_Entita_cod = -1
            Select pl.To_Programmazione_Cod
            ).FirstOrDefault

    End Function

    Private Function recode_dammi_programmazione_entita_cod(ByVal old_pivaSuerUser As String, ByVal old_programmazione_entita_cod As Integer) As String
        Return (
            From pl In _funzioniGLOBAL.Planning
            Where pl.From_PivaSuperUser = old_pivaSuerUser _
            And pl.From_Programmazione_Entita_cod = old_programmazione_entita_cod
            Select pl.To_Programmazione_Entita_cod
            ).FirstOrDefault

    End Function


    Private Function recode_dammi_id_agenda(ByVal old_piva As String, ByVal old_id_agenda As Integer) As String
        If NuovaLogicaRecodeAG Then
            Return (
                From pl In _efG2G.G2G_Recode_Agenda
                Where pl.FromPiva = old_piva _
                And pl.FromId_Agenda = old_id_agenda
                Select pl.ToId_Agenda
                ).FirstOrDefault
        Else
            Return (
                From pl In _Agenda
                Where pl.FromPiva = old_piva _
                And pl.FromId_Agenda = old_id_agenda
                Select pl.ToId_Agenda
                ).FirstOrDefault
        End If
    End Function

#End Region


#Region "NEW"

    Public Sub Elabora_XML_Ricette_Salva(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )

        Const NomeFunzione As String = "Elabora_XML_Ricette_Salva"

        Try
            Dim leggiRicette As New AgronicaCoreG2GLocalDal.G2GRicette_R
            Dim oRicette As AgronicaCoreModello.G2G_Ricette = leggiRicette.LeggiPerGias2Gias(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each ricetta In oRicette.ricette_insert

                If ricetta.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = ricetta.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = ricetta.Sa_Cod).FirstOrDefault()
                    ricetta.Sa_Cod = centro.TO_SaCod
                End If

                Dim to_programmazione_cod As Integer = 0

            Next

            For Each ricetta In oRicette.ricette_update

                If ricetta.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = ricetta.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = ricetta.Sa_Cod).FirstOrDefault()
                    ricetta.Sa_Cod = centro.TO_SaCod
                End If

                Dim to_programmazione_cod As Integer = 0

            Next

            For Each ricetta_dettaglio In oRicette.ricette_dettagli_insert
                Dim mat_cod = ricetta_dettaglio.Mat_Cod
                If mat_cod <> 0 Then
                    ricetta_dettaglio.Mat_Cod = recode_return_MatCod(mat_cod, ricetta_dettaglio.Elem_Cod, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioniImportImpresa.Flagimporta_materieprime)
                End If
                Dim id_attivita = ricetta_dettaglio.ID_Attivita
                If id_attivita <> 0 Then
                    ricetta_dettaglio.ID_Attivita = recode_return_ID_Attivita(id_attivita, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE)
                End If
            Next

            For Each ricetta_dettaglio In oRicette.ricette_dettagli_update
                Dim mat_cod = ricetta_dettaglio.Mat_Cod
                If mat_cod <> 0 Then
                    ricetta_dettaglio.Mat_Cod = recode_return_MatCod(mat_cod, ricetta_dettaglio.Elem_Cod, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioniImportImpresa.Flagimporta_materieprime)
                End If
                Dim id_attivita = ricetta_dettaglio.ID_Attivita
                If id_attivita <> 0 Then
                    ricetta_dettaglio.ID_Attivita = recode_return_ID_Attivita(id_attivita, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE)
                End If
            Next

            For Each ricetta_destinazione In oRicette.ricette_destinazioni_insert
                Dim sa_cod_origine As Integer = ricetta_destinazione.Sa_Cod
                Dim appezza_origine As Integer = ricetta_destinazione.Appezza

                If ricetta_destinazione.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = ricetta_destinazione.Sa_Cod).FirstOrDefault()
                    ricetta_destinazione.Sa_Cod = centro.TO_SaCod
                End If

                Select Case ricetta_destinazione.Tipo_Destinazione
                    Case 0
                        If ricetta_destinazione.Appezza <> 0 Then
                            Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = ricetta_destinazione.Appezza).FirstOrDefault()
                            ricetta_destinazione.Appezza = appezza.To_Appezza
                        End If

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = appezza_origine AndAlso rr.From_Id_Reg = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = impianto.To_Id_Reg
                        End If
                    Case 20

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim g2gFabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine AndAlso rr.FromSa_cod = sa_cod_origine AndAlso rr.From_FabbricatoCod = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = g2gFabbricato.To_FabbricatoCod
                        End If

                End Select

            Next

            For Each ricetta_destinazione In oRicette.ricette_destinazioni_update
                Dim sa_cod_origine As Integer = ricetta_destinazione.Sa_Cod
                Dim appezza_origine As Integer = ricetta_destinazione.Appezza

                If ricetta_destinazione.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = ricetta_destinazione.Sa_Cod).FirstOrDefault()
                    ricetta_destinazione.Sa_Cod = centro.TO_SaCod
                End If

                Select Case ricetta_destinazione.Tipo_Destinazione
                    Case 0
                        If ricetta_destinazione.Appezza <> 0 Then
                            Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = ricetta_destinazione.Appezza).FirstOrDefault()
                            ricetta_destinazione.Appezza = appezza.To_Appezza
                        End If

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = appezza_origine AndAlso rr.From_Id_Reg = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = impianto.To_Id_Reg
                        End If
                    Case 20

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim g2gFabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine AndAlso rr.FromSa_cod = sa_cod_origine AndAlso rr.From_FabbricatoCod = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = g2gFabbricato.To_FabbricatoCod
                        End If

                End Select

            Next

            For Each ricettaxagenda In oRicette.ricettexagenda_insert
                Dim id_agenda_old = ricettaxagenda.Id_Agenda
                ricettaxagenda.Id_Agenda = (From t In efG2G.G2G_Recode_Agenda
                                            Where t.From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE _
                                                   And t.To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE _
                                                   And t.FromId_Agenda = id_agenda_old _
                                                   And t.FromPiva = Piva_Origine _
                                                   And t.ToPiva = Piva_Destinazione).FirstOrDefault.ToId_Agenda
            Next

            For Each ricettaxnote In oRicette.ricettexnote_insert
                Dim id_note_old = ricettaxnote.Nota_Cod
                ricettaxnote.Nota_Cod = (From t In efG2G.G2G_Recode_NoteIntervento
                                         Where t.From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE _
                                                   And t.To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE _
                                                   And t.From_Nota_Cod = id_note_old).FirstOrDefault.To_Nota_Cod
            Next


            Dim RecordCoinvolti = oRicette.G2G_Ricette_Destinazioni_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Destinazioni_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Destinazioni_Recode_update.Count _
                                + oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_update.Count _
                                + oRicette.G2G_Ricette_Dettagli_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Dettagli_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Dettagli_Recode_update.Count _
                                + oRicette.G2G_Ricette_Operazioni_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Operazioni_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Operazioni_Recode_update.Count _
                                + oRicette.G2G_Ricette_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Recode_update.Count _
                                + oRicette.ricettexagenda_delete.Count _
                                + oRicette.ricettexagenda_insert.Count _
                                + oRicette.ricettexcultivar_delete.Count _
                                + oRicette.ricettexcultivar_insert.Count _
                                + oRicette.ricettexnote_delete.Count _
                                + oRicette.ricettexnote_insert.Count _
                                + oRicette.ricette_delete.Count _
                                + oRicette.ricette_destinazioni_delete.Count _
                                + oRicette.ricette_destinazioni_insert.Count _
                                + oRicette.ricette_destinazioni_update.Count _
                                + oRicette.ricette_dettaglio_tecnico_delete.Count _
                                + oRicette.ricette_dettaglio_tecnico_insert.Count _
                                + oRicette.ricette_dettaglio_tecnico_update.Count _
                                + oRicette.ricette_dettagli_delete.Count _
                                + oRicette.ricette_dettagli_insert.Count _
                                + oRicette.ricette_dettagli_update.Count _
                                + oRicette.ricette_insert.Count _
                                + oRicette.ricette_operazioni_delete.Count _
                                + oRicette.ricette_operazioni_insert.Count _
                                + oRicette.ricette_operazioni_update.Count _
                                + oRicette.ricette_update.Count

            If RecordCoinvolti = 0 Then
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette: Nessun dato da Inserire/Aggiornare/Eliminare ")
                Exit Sub
            End If

            Dim sXmlRicette As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Ricette)(oRicette, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlRicette, "//utente/DatiImprese/Impresa", "DatiRicette")

            Log_Import.AppendLine(CStr(Date.Now) & " " & NomeFunzione & ": Chiamo WS")
            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputRicette As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputRicette)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiRicette")
            Dim rispostaAllegati = xmlRispostaAllegati.FirstNode.ToString
            'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
            Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAllegati, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine("Trasferimento Ricette: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento Ricette ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette: " & oRicette.ricette_insert.Count & " nuovi " & oRicette.ricette_update.Count & " modificati " & oRicette.G2G_Ricette_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Operazioni: " & oRicette.ricette_operazioni_insert.Count & " nuovi " & oRicette.ricette_operazioni_update.Count & " modificati " & oRicette.G2G_Ricette_Operazioni_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Dettagli: " & oRicette.ricette_dettagli_insert.Count & " nuovi " & oRicette.ricette_dettagli_update.Count & " modificati " & oRicette.G2G_Ricette_Dettagli_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Dettagli Tecnici: " & oRicette.ricette_dettaglio_tecnico_insert.Count & " nuovi " & oRicette.ricette_dettaglio_tecnico_update.Count & " modificati " & oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Destinazioni: " & oRicette.ricette_destinazioni_insert.Count & " nuovi " & oRicette.ricette_destinazioni_update.Count & " modificati " & oRicette.G2G_Ricette_Destinazioni_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - RicetteXAgenda: " & oRicette.ricettexagenda_insert.Count & " nuovi " & oRicette.ricettexagenda_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - RicetteXCultivar: " & oRicette.ricettexcultivar_insert.Count & " nuovi " & oRicette.ricettexcultivar_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - RicetteXNote: " & oRicette.ricettexnote_insert.Count & " nuovi " & oRicette.ricettexnote_delete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            If Not ex.InnerException Is Nothing Then
                msg &= ex.InnerException.Message
            End If
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Ricette_Salva_Reverse(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )

        Const NomeFunzione As String = "Elabora_XML_Ricette_Salva_Reverse"
        Dim outputRicette As String = ""
        Try
            Dim leggiRicette As New AgronicaCoreG2GLocalDal.G2GRicette_R
            Dim oRicette As AgronicaCoreModello.G2G_Ricette_Reverse = leggiRicette.LeggiPerGias2Gias_Reverse(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each ricetta In oRicette.ricette_insert

                If ricetta.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = ricetta.Ricetta_SuperUser AndAlso
                                                                          rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso
                                                                          rr.To_Piva = Piva_Origine AndAlso
                                                                          rr.TO_SaCod = ricetta.Sa_Cod).FirstOrDefault()
                    ricetta.Sa_Cod = centro.FROM_SaCod
                End If

                Dim to_programmazione_cod As Integer = 0

            Next

            For Each ricetta In oRicette.ricette_update

                If ricetta.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = ricetta.Ricetta_SuperUser AndAlso
                                                                          rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso
                                                                          rr.To_Piva = Piva_Origine AndAlso
                                                                          rr.TO_SaCod = ricetta.Sa_Cod).FirstOrDefault()
                    ricetta.Sa_Cod = centro.FROM_SaCod
                End If

                Dim to_programmazione_cod As Integer = 0

            Next

            For Each ricetta_dettaglio In oRicette.ricette_dettagli_insert
                Dim mat_cod = ricetta_dettaglio.Mat_Cod
                If mat_cod <> 0 Then
                    ricetta_dettaglio.Mat_Cod = recode_return_MatCodReverse(mat_cod, ricetta_dettaglio.Elem_Cod, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioniImportImpresa.Flagimporta_materieprime)
                End If
                Dim id_attivita = ricetta_dettaglio.ID_Attivita
                If id_attivita <> 0 Then
                    ricetta_dettaglio.ID_Attivita = recode_return_ID_AttivitaReverse(id_attivita, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE)
                End If
            Next

            For Each ricetta_dettaglio In oRicette.ricette_dettagli_update
                Dim mat_cod = ricetta_dettaglio.Mat_Cod
                If mat_cod <> 0 Then
                    ricetta_dettaglio.Mat_Cod = recode_return_MatCod(mat_cod, ricetta_dettaglio.Elem_Cod, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioniImportImpresa.Flagimporta_materieprime)
                End If
                Dim id_attivita = ricetta_dettaglio.ID_Attivita
                If id_attivita <> 0 Then
                    ricetta_dettaglio.ID_Attivita = recode_return_ID_AttivitaReverse(id_attivita, Piva_Destinazione, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE)
                End If
            Next

            For Each ricetta_destinazione In oRicette.ricette_destinazioni_insert
                Dim sa_cod_origine As Integer = ricetta_destinazione.Sa_Cod
                Dim appezza_origine As Integer = ricetta_destinazione.Appezza

                If ricetta_destinazione.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                          AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                          AndAlso rr.To_Piva = Piva_Origine _
                                                                          AndAlso rr.TO_SaCod = ricetta_destinazione.Sa_Cod).FirstOrDefault()
                    ricetta_destinazione.Sa_Cod = centro.FROM_SaCod
                End If

                Select Case ricetta_destinazione.Tipo_Destinazione
                    Case 0
                        If ricetta_destinazione.Appezza <> 0 Then
                            Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                                        AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                                        AndAlso rr.To_Piva = Piva_Origine _
                                                                                        AndAlso rr.To_Sa_Cod = sa_cod_origine _
                                                                                        AndAlso rr.To_Appezza = ricetta_destinazione.Appezza).FirstOrDefault()
                            ricetta_destinazione.Appezza = appezza.From_Appezza
                        End If

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                                     AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                                     AndAlso rr.To_Piva = Piva_Origine _
                                                                                     AndAlso rr.To_Sa_Cod = sa_cod_origine _
                                                                                     AndAlso rr.To_Appezza = appezza_origine _
                                                                                     AndAlso rr.To_Id_Reg = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = impianto.From_Id_Reg
                        End If
                    Case 20

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim g2gFabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                                            AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                                            AndAlso rr.ToPiva = Piva_Origine _
                                                                                            AndAlso rr.ToSa_cod = sa_cod_origine _
                                                                                            AndAlso rr.To_FabbricatoCod = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = g2gFabbricato.From_FabbricatoCod
                        End If

                End Select

            Next

            For Each ricetta_destinazione In oRicette.ricette_destinazioni_update
                Dim sa_cod_origine As Integer = ricetta_destinazione.Sa_Cod
                Dim appezza_origine As Integer = ricetta_destinazione.Appezza

                If ricetta_destinazione.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                          AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                          AndAlso rr.To_Piva = Piva_Origine _
                                                                          AndAlso rr.TO_SaCod = ricetta_destinazione.Sa_Cod).FirstOrDefault()
                    ricetta_destinazione.Sa_Cod = centro.FROM_SaCod
                End If

                Select Case ricetta_destinazione.Tipo_Destinazione
                    Case 0
                        If ricetta_destinazione.Appezza <> 0 Then
                            Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                                        AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                                        AndAlso rr.To_Piva = Piva_Origine _
                                                                                        AndAlso rr.To_Sa_Cod = sa_cod_origine _
                                                                                        AndAlso rr.To_Appezza = ricetta_destinazione.Appezza).FirstOrDefault()
                            ricetta_destinazione.Appezza = appezza.From_Appezza
                        End If

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                                     AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                                     AndAlso rr.To_Piva = Piva_Origine _
                                                                                     AndAlso rr.To_Sa_Cod = sa_cod_origine _
                                                                                     AndAlso rr.To_Appezza = appezza_origine _
                                                                                     AndAlso rr.To_Id_Reg = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = impianto.From_Id_Reg
                        End If
                    Case 20

                        If ricetta_destinazione.Id_Reg <> 0 Then
                            Dim g2gFabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.To_PivaSuperUser = ricetta_destinazione.Ricetta_SuperUser _
                                                                                            AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                                                                            AndAlso rr.ToPiva = Piva_Origine _
                                                                                            AndAlso rr.ToSa_cod = sa_cod_origine _
                                                                                            AndAlso rr.To_FabbricatoCod = ricetta_destinazione.Id_Reg).FirstOrDefault()
                            ricetta_destinazione.Id_Reg = g2gFabbricato.From_FabbricatoCod
                        End If

                End Select

            Next

            For Each ricettaxagenda In oRicette.ricettexagenda_insert
                Dim id_agenda_old = ricettaxagenda.Id_Agenda
                ricettaxagenda.Id_Agenda = (From t In efG2G.G2G_Recode_Agenda
                                            Where t.To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE _
                                                   And t.From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE _
                                                   And t.ToId_Agenda = id_agenda_old _
                                                   And t.ToPiva = Piva_Origine _
                                                   And t.FromPiva = Piva_Destinazione).FirstOrDefault.FromId_Agenda
            Next

            For Each ricettaxnote In oRicette.ricettexnote_insert
                Dim id_note_old = ricettaxnote.Nota_Cod
                ricettaxnote.Nota_Cod = (From t In efG2G.G2G_Recode_NoteIntervento
                                         Where t.To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE _
                                                   And t.From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE _
                                                   And t.To_Nota_Cod = id_note_old).FirstOrDefault.From_Nota_Cod
            Next


            Dim RecordCoinvolti = oRicette.G2G_Ricette_Destinazioni_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Destinazioni_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Destinazioni_Recode_update.Count _
                                + oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_update.Count _
                                + oRicette.G2G_Ricette_Dettagli_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Dettagli_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Dettagli_Recode_update.Count _
                                + oRicette.G2G_Ricette_Operazioni_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Operazioni_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Operazioni_Recode_update.Count _
                                + oRicette.G2G_Ricette_Recode_delete.Count _
                                + oRicette.G2G_Ricette_Recode_insert.Count _
                                + oRicette.G2G_Ricette_Recode_update.Count _
                                + oRicette.ricettexagenda_delete.Count _
                                + oRicette.ricettexagenda_insert.Count _
                                + oRicette.ricettexcultivar_delete.Count _
                                + oRicette.ricettexcultivar_insert.Count _
                                + oRicette.ricettexnote_delete.Count _
                                + oRicette.ricettexnote_insert.Count _
                                + oRicette.ricette_delete.Count _
                                + oRicette.ricette_destinazioni_delete.Count _
                                + oRicette.ricette_destinazioni_insert.Count _
                                + oRicette.ricette_destinazioni_update.Count _
                                + oRicette.ricette_dettaglio_tecnico_delete.Count _
                                + oRicette.ricette_dettaglio_tecnico_insert.Count _
                                + oRicette.ricette_dettaglio_tecnico_update.Count _
                                + oRicette.ricette_dettagli_delete.Count _
                                + oRicette.ricette_dettagli_insert.Count _
                                + oRicette.ricette_dettagli_update.Count _
                                + oRicette.ricette_insert.Count _
                                + oRicette.ricette_operazioni_delete.Count _
                                + oRicette.ricette_operazioni_insert.Count _
                                + oRicette.ricette_operazioni_update.Count _
                                + oRicette.ricette_update.Count

            If RecordCoinvolti = 0 Then
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette: Nessun dato da Inserire/Aggiornare/Eliminare ")
                Exit Sub
            End If

            Dim sXmlRicette As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Ricette_Reverse)(oRicette, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlRicette, "//utente/DatiImprese/Impresa", "DatiRicette")

            Log_Import.AppendLine(CStr(Date.Now) & " " & NomeFunzione & ": Chiamo WS")
            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            outputRicette = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputRicette)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiRicette")
            Dim rispostaAllegati = xmlRispostaAllegati.FirstNode.ToString
            'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
            Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAllegati, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine("Trasferimento Ricette: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento Ricette ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette: " & oRicette.ricette_insert.Count & " nuovi " & oRicette.ricette_update.Count & " modificati " & oRicette.G2G_Ricette_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Operazioni: " & oRicette.ricette_operazioni_insert.Count & " nuovi " & oRicette.ricette_operazioni_update.Count & " modificati " & oRicette.G2G_Ricette_Operazioni_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Dettagli: " & oRicette.ricette_dettagli_insert.Count & " nuovi " & oRicette.ricette_dettagli_update.Count & " modificati " & oRicette.G2G_Ricette_Dettagli_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Dettagli Tecnici: " & oRicette.ricette_dettaglio_tecnico_insert.Count & " nuovi " & oRicette.ricette_dettaglio_tecnico_update.Count & " modificati " & oRicette.G2G_Ricette_Dettaglio_Tecnico_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - Destinazioni: " & oRicette.ricette_destinazioni_insert.Count & " nuovi " & oRicette.ricette_destinazioni_update.Count & " modificati " & oRicette.G2G_Ricette_Destinazioni_Recode_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - RicetteXAgenda: " & oRicette.ricettexagenda_insert.Count & " nuovi " & oRicette.ricettexagenda_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - RicetteXCultivar: " & oRicette.ricettexcultivar_insert.Count & " nuovi " & oRicette.ricettexcultivar_delete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Ricette - RicetteXNote: " & oRicette.ricettexnote_insert.Count & " nuovi " & oRicette.ricettexnote_delete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            If Not ex.InnerException Is Nothing Then
                msg &= ex.InnerException.Message
            End If
            msg &= vbCrLf & "outputRicette:" & outputRicette
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

#End Region

End Class
