Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Vincoli_WS

    Public Shared Function LeggiVincoli(ByVal filtro As AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli,
                                        ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)
        Dim list As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)

        Dim objConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim leggiPrivati As String = objConfigurazioneSiti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)

        If leggiPrivati.ToLower() = "true" Then
            list = LeggiVincoli_Privati(filtro, False, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        Else
            list = LeggiVincoli_Pubblici(filtro, False, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        Return list
    End Function

    Public Shared Function LeggiVincoliOrdered(ByVal filtro As AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli,
                                        ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)
        Dim list As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)

        Dim objConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim leggiPrivati As String = objConfigurazioneSiti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)

        If leggiPrivati.ToLower() = "true" Then
            list = LeggiVincoli_Privati(filtro, True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        Else
            list = LeggiVincoli_Pubblici(filtro, True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        Return list
    End Function

    Private Shared Function LeggiVincoli_Privati(ByVal filtro As AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli,
                                                 ByVal orderByYear As Boolean,
                                                 ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)
        Dim leggiRegolamenti As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim dtDPI = Disciplinari_Elenco_TuttigliElemInChiave(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, 0, 0, 0, 0, False, False, True)
        Dim dtDPIr = dtDPI.Select(" Validita_Inizio <= '" & CStr(filtro.validita.fine) & "' AND Validita_Fine >= '" & CStr(filtro.validita.inizio) & "' ")
        Dim listDPI = New List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)
        Dim dtRegolamenti = leggiRegolamenti.Leggi(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " Reg_Cod IN (1,4) ", "", objParametri_Server)
        If dtDPIr.Length > 0 Then
            dtDPI = dtDPIr.CopyToDataTable
            Dim rows = If(orderByYear, (From row In dtDPI.Rows Order By row("Validita_Fine") Descending Select row).ToList, (From row In dtDPI.Rows).ToList)
            listDPI = (From row In rows
                       Select New AgronicaCoreModelsSTD.metaschema.Vincolo(
                                                     row("PubblicoPrivato") & "_" & row("codRegolamento")) With {
                                                      .descrizione = row("nomeEsteso"),
                                                      .disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare() With {
                                                                .codice = row("codRegolamento") & "/" & row("PubblicoPrivato") & "/" & row("PUA_Regolamento_Cod") & "/" & row("id_tr"),
                                                                .descrizione = row("nomeEsteso"),
                                                                .idTr = row("id_tr"),
                                                                .disciplinarePubblicoPrivato = row("PubblicoPrivato"),
                                                                .regolamentoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione() With {
                                                                    .codice = row("PUA_Regolamento_Cod"),
                                                                    .descrizione = row("nomeEsteso") & " - Piano di Concimazione Dose Standard"
                                                                }
                                                            },
                                                      .regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti() With {
                                                                .codice = 1,
                                                                .descrizione = "Nessuno"
                                                            },
                                                       .IdEnte = row("IDEnte")
                                                        }).ToList
        Else

        End If

        Dim list = (From row In dtRegolamenti.Rows Select New AgronicaCoreModelsSTD.metaschema.Vincolo(
                                                 row("Reg_COD")) With {
                                                        .descrizione = row("Reg_DES"),
                                                        .disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare() With {
                                                            .codice = "0",
                                                            .descrizione = "",
                                                            .idTr = 3,
                                                            .disciplinarePubblicoPrivato = 1,
                                                            .regolamentoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione() With {
                                                                .codice = 0,
                                                                .descrizione = "Nessuno",
                                                                .tipo = 0
                                                            }
                                                        },
                                                        .regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti() With {
                                                            .codice = row("Reg_COD"),
                                                            .descrizione = row("Reg_DES")
                                                        },
                                                        .IdEnte = 0
                                                    }).ToList

        list(1).descrizione = "Bio"
        list(1).regolamento.descrizione = "Bio"

        list.AddRange(listDPI)
        Return list
    End Function


    Private Shared Function LeggiVincoli_Pubblici(ByVal filtro As AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli,
                                                  ByVal orderByYear As Boolean,
                                                  ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo)

        Dim leggiDPI As New AgronicaCoreDpiDAL.Dpi_R
        Dim leggiRegolamenti As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim dtDPI = leggiDPI.Leggi_DPI_Regolamenti(1, 0, 0, filtro.validita.inizio, filtro.validita.fine, "", "", objParametri_Server)
        Dim dtRegolamenti = leggiRegolamenti.Leggi(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " Reg_Cod IN (1,4) ", "", objParametri_Server)
        Dim rows = If(orderByYear, (From row In dtDPI.Rows Order By row("ValidoAl") Descending Select row).ToList, (From row In dtDPI.Rows).ToList)
        Dim listDPI = (From row In rows
                       Select New AgronicaCoreModelsSTD.metaschema.Vincolo(
                                                 row("Flag_Privato_Pubblico") & "_" & row("cod_Regolamento")) With {
                                                  .descrizione = row("nomeEsteso"),
                                                  .disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare() With {
                                                            .codice = row("cod_Regolamento") & "/" & row("Flag_Privato_Pubblico") & "/" & row("PUA_Regolamento_Cod") & "/" & row("id_tr"),
                                                            .descrizione = row("nomeEsteso"),
                                                            .idTr = row("id_tr"),
                                                            .disciplinarePubblicoPrivato = row("Flag_Privato_Pubblico"),
                                                            .regolamentoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione() With {
                                                                .codice = row("PUA_Regolamento_Cod"),
                                                                .descrizione = row("nomeEsteso") & " - Piano di Concimazione Dose Standard"
                                                            }
                                                        },
                                                  .regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti() With {
                                                            .codice = 1,
                                                            .descrizione = "Nessuno"
                                                        },
                                                  .IdEnte = row("IDEnte")
                                                    }).ToList

        Dim list = (From row In dtRegolamenti.Rows Select New AgronicaCoreModelsSTD.metaschema.Vincolo(
                                                 row("Reg_COD")) With {
                                                        .descrizione = row("Reg_DES"),
                                                        .disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare() With {
                                                            .codice = "0",
                                                            .descrizione = "",
                                                            .idTr = 3,
                                                            .disciplinarePubblicoPrivato = 1,
                                                            .regolamentoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione() With {
                                                                .codice = 0,
                                                                .descrizione = "Nessuno",
                                                                .tipo = 0
                                                            }
                                                        },
                                                        .regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti() With {
                                                            .codice = row("Reg_COD"),
                                                            .descrizione = row("Reg_DES")
                                                        },
                                                        .IdEnte = 0
                                                    }).ToList

        list(1).descrizione = "Bio"
        list(1).regolamento.descrizione = "Bio"

        list.AddRange(listDPI)
        Return list

    End Function

    Private Shared Function Disciplinari_Elenco_TuttigliElemInChiave(
                                                        ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Reg_Cod As Integer,
                                                        ByVal VEG_COD As Integer,
                                                        ByVal Id_RcDpi As Integer,
                                                        ByVal Flag_Privato_Pubblico As Integer,
                                                        ByVal Includi_Nessuno As Boolean,
                                                        ByVal Includi_Biologico As Boolean,
                                                        ByVal Flag_DisciplinareAttivo As Boolean) As DataTable


        Dim strErr As String = ""

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("IDEnte", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))

        Dim chiaveCache = "Disciplinari_Elenco_TuttigliElemInChiave_" & objParametri_Server.PivaSuperUser & "_" & Reg_Cod & "_" & VEG_COD & "_" & Flag_Privato_Pubblico

        If Flag_DisciplinareAttivo Then

            Try

                If HttpContext.Current.Cache(chiaveCache) IsNot Nothing Then
                    Return HttpContext.Current.Cache(chiaveCache)
                End If

                Dim agroWs As String
                Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                If agroWs = "" Then
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)
                End If

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                agroWs,
                                objParametri_Utenti)

                Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
                Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
                Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)


                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        objParametri_Server.PivaSuperUser,
                                                        ASG_Utente_Username_Crypt,
                                                        ASG_Utente_Password_Crypt,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")
                                Dr.Item("IDEnte") = XmlElemento.GetAttribute("idente")

                                If XmlElemento.HasAttribute("validodal") AndAlso IsDate(XmlElemento.GetAttribute("validodal")) Then
                                    Dr.Item("Validita_Inizio") = CDate(XmlElemento.GetAttribute("validodal"))
                                Else
                                    Dr.Item("Validita_Inizio") = AGRODATAINIZIO
                                End If


                                If XmlElemento.HasAttribute("validoal") AndAlso IsDate(XmlElemento.GetAttribute("validoal")) Then
                                    Dr.Item("Validita_Fine") = CDate(XmlElemento.GetAttribute("validoal"))
                                Else
                                    Dr.Item("Validita_Fine") = AGRODATAFINE
                                End If

                                Dt.Rows.Add(Dr)
                            Next
                        End If
                    End If
                End If

                HttpContext.Current.Cache(chiaveCache) = Dt

                If Not Dt Is Nothing Then
                    Return Dt
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

                Return Dt

            End Try

        End If
        Return Dt
    End Function

End Class
