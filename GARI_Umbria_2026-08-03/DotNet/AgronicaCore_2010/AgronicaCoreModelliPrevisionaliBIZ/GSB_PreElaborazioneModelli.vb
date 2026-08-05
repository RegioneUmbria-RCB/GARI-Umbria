
Imports System.Text
Imports System.Threading
Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoBiz
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class GSB_PreElaborazioneModelli

    Private ReadOnly mObjParametri_Server As AgronicaCoreParametri
    Private ReadOnly mObjParametri_Utenti As AgronicaCoreParametri
    Private ReadOnly mObjParametri_Super_Server As AgronicaCoreParametri
    Private ReadOnly mMessaggio As StringBuilder
    Private ReadOnly mBlockLimit As Integer
    Private ReadOnly mPopolaCentri As Boolean
    Private ReadOnly mPopolaModelli As Boolean



    Public ReadOnly Property Messaggio As String
        Get
            Return mMessaggio.ToString
        End Get
    End Property



    Public Sub New(Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ObjParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        mObjParametri_Server = ObjParametri_Server
        mObjParametri_Utenti = ObjParametri_Utenti
        mObjParametri_Super_Server = ObjParametri_Super_Server

        mMessaggio = New StringBuilder

        Dim objParam As JObject = JsonConvert.DeserializeObject(Configurazione_Servizio.Parametri_Extra)

        Try
            mBlockLimit = objParam("LimiteElaborazioneBlocchi").Value(Of Integer)
        Catch ex As Exception
            mBlockLimit = 100
        End Try

        Try
            mPopolaCentri = objParam("PopolaTabellaAssociazioneCentri").Value(Of Integer) <> 0
        Catch ex As Exception
            mPopolaCentri = True
        End Try

        Try
            mPopolaModelli = objParam("PopolaTabellaAssociazioneModelli").Value(Of Integer) <> 0
        Catch ex As Exception
            mPopolaModelli = True
        End Try

    End Sub



    Private Class BloccoElab
        Public arrModelli As JArray
        Public risModelli As RisultatoElaborazioneIndicatori
    End Class



    Public Function Avvia() As Boolean

        Dim cfg As New ModelloParametriGSB With {
            .ParametriChiamataWsMeteo = New ParametriChiamataWsMeteo,
            .GiasOnline_WS_Meteo_Meteo = "",
            .ListaSpecieFiltro = New List(Of Integer),
            .Validita_Inizio_impianti = Date.Now()
        }

        cfg.ParametriChiamataWsMeteo.ASG_Utente_Username = mObjParametri_Server.SuperUserUsername

        Dim isa_tbl = LeggiIndicatoriXSpecieXAvversita()

        If isa_tbl Is Nothing OrElse isa_tbl.Rows.Count = 0 Then

            mMessaggio.AppendLine("Elenco modelli autorizzati vuoto...")

            Return True
        End If

        For Each dr In isa_tbl.Rows

            Dim veg_cod As Integer = CInt(dr("Veg_Cod"))

            If Not cfg.ListaSpecieFiltro.Contains(veg_cod) Then

                cfg.ListaSpecieFiltro.Add(veg_cod)
            End If
        Next



        mMessaggio.AppendLine("Fase 1: Scrittura impianti senza origine associata...")



        Dim impianti_stazioni As New DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo(cfg)
        Dim risp = impianti_stazioni.AccodaImpiantiConDatoGisSenzaOrigineAssociata(mObjParametri_Server, mObjParametri_Utenti, mObjParametri_Super_Server)

        If Not risp.RispostaOK Then

            mMessaggio.AppendLine("ERRORE: " & risp.Errore)
        End If


        If mPopolaCentri Then

            mMessaggio.AppendLine()
            mMessaggio.AppendLine("Fase 1.1: Scrittura tabella associazione origine per centri aziendali da tabella associazione impianti...")

            Dim centri_stazioni As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_W

            Dim aggiunti As Integer = 0

            Try

                aggiunti = centri_stazioni.PopolaDaImpianti(mObjParametri_Server)

            Catch ex As Exception

                mMessaggio.AppendLine(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

            End Try

        End If



        mMessaggio.AppendLine()
        mMessaggio.AppendLine("Fase 2: Scrittura tabella elenco indicatori da elaborare...")



        Dim imp_staz As New AgronicaCoreMeteoDAL.DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_R
        Dim staz_veg_tbl = imp_staz.LeggiElencoPerStazioneSpecieDataSemina(mObjParametri_Server)

        If staz_veg_tbl Is Nothing OrElse staz_veg_tbl.Rows.Count = 0 Then

            mMessaggio.AppendLine("Elenco impianti per calcolo vuoto...")

            Return True
        End If


        Dim CurrentBlock As New BloccoElab With {.arrModelli = New JArray, .risModelli = Nothing}

        Dim Blocks As New List(Of BloccoElab) From {CurrentBlock}

        Dim Modelli As New List(Of AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_W.ObjModello)


        Try

            Dim join = From t1 In staz_veg_tbl.AsEnumerable()
                       Join t2 In isa_tbl.AsEnumerable() On t1.Field(Of Integer)("Veg_Cod") Equals t2.Field(Of Integer)("Veg_Cod")
                       Select New With {
                           .Tipo_Sorgente = t1.Field(Of Integer)("tipo_sorgente"),
                           .Stazione_Cod = t1.Field(Of Integer)("Stazione_Cod"),
                           .Veg_Cod = t1.Field(Of Integer)("Veg_Cod"),
                           .Mod_Cod = t2.Field(Of Integer)("Mod_Cod"),
                           .Avv_Cod = t2.Field(Of Integer)("Avv_Cod"),
                           .DataSeminaPrevista = t1.Field(Of Date?)("DataSeminaPrevista")}

            If join Is Nothing OrElse Not join.Any Then

                mMessaggio.AppendLine("Elenco indicatori richiesti vuoto...")

                Return True
            End If

            Dim joinList = join.ToList

            For Each item In joinList

                If CurrentBlock.arrModelli.Count >= mBlockLimit Then

                    CurrentBlock = New BloccoElab With {.arrModelli = New JArray, .risModelli = Nothing}

                    Blocks.Add(CurrentBlock)
                End If

                Dim parElab As New JObject

                If item.Mod_Cod = enum_ModelliPrevisionali.UniCatt_Mais_AFLA OrElse item.Mod_Cod = enum_ModelliPrevisionali.UniCatt_Mais_FER Then

                    If item.DataSeminaPrevista.HasValue Then

                        If item.DataSeminaPrevista.Value > CostantiPersonalizzate.AGRODATAINIZIO Then

                            parElab("DataEmergenza") = item.DataSeminaPrevista.Value.ToString("yyyy-MM-dd")
                            'parElab("DataEmergenza_gg") = item.DataSeminaPrevista.Value.DayOfYear
                        End If
                    End If
                End If

                Dim objModello As New JObject

                objModello("Tipo_Sorgente") = item.Tipo_Sorgente
                objModello("Stazione_Cod") = item.Stazione_Cod
                objModello("Mod_Cod") = item.Mod_Cod
                objModello("Veg_Cod") = item.Veg_Cod
                objModello("Avv_Cod") = item.Avv_Cod
                objModello("Alg_Cod") = 0
                objModello("ParametriElaborazione") = parElab
                objModello("InizioPeriodo_gg") = 1
                objModello("FinePeriodo_gg") = 365
                objModello("Validita_minuti") = 60 * 12 'Tenere basso se si vuole forzare l'esecuzione del modello

                CurrentBlock.arrModelli.Add(objModello)

                Modelli.Add(New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_W.ObjModello With {
                            .Tipo_Sorgente = item.Tipo_Sorgente,
                            .Stazione_Cod = item.Stazione_Cod,
                            .Mod_Cod = item.Mod_Cod,
                            .Algoritmo_Cod = 0,
                            .ParametriElaborazione = parElab.ToString(Formatting.None),
                            .GG_InizioValidita = 1,
                            .GG_FineValidita = 360
                            })

            Next

        Catch ex As Exception

            mMessaggio.AppendLine("ERRORE: " & ex.Message)

            Return False
        End Try


        If mPopolaModelli Then

            mMessaggio.AppendLine()
            mMessaggio.AppendLine("Fase 2.1: Scrittura tabella associazione modelli previsionali per stazioni meteo...")

            Dim stazioni_modelli As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_W

            Dim aggiunti As Integer = 0

            Try

                aggiunti = stazioni_modelli.PopolaDaImpianti(Modelli, mObjParametri_Server)

            Catch ex As Exception

                mMessaggio.AppendLine(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

            End Try

        End If


        For Each blocco In Blocks

            While blocco.risModelli Is Nothing OrElse blocco.risModelli.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.InAttesa

                blocco.risModelli = ScriviRichiestaGSB(blocco.arrModelli)

                If blocco.risModelli.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.InAttesa Then

                    Thread.Sleep(2000)
                End If
            End While
        Next


        Dim writer As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_W

        For Each blocco In Blocks

            If blocco.risModelli.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.Pronti Then

                For Each indic In blocco.risModelli.Indicatori

                    If indic.Risultato.Status = RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Stato_Elaborazione._Valid OrElse
                        indic.Risultato.Status = RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Stato_Elaborazione._Warning Then

                        Dim parElab As New JObject
                        parElab("Veg_Cod") = indic.Parametri.Veg_Cod

                        Dim Tipo_Sorgente As Integer = indic.Parametri.Tipo_Sorgente
                        Dim Stazione_Cod As Integer = indic.Parametri.Stazione_Cod
                        Dim Mod_Cod As Integer = indic.Parametri.Mod_Cod
                        Dim Data_Elaborazione As DateTime = Date.Now()
                        Dim risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione = indic.Risultato
                        Dim Qta_Ril As Decimal = 0
                        'Qta_Ril assume il valore 0, 5, 10, ... a seconda della fascia di appartenenza dell'indicatore
                        Dim b As Integer = 0
                        While b < risElab.Bands.Count
                            If risElab.Value <= risElab.Bands(b).Value Then
                                Qta_Ril = b * 5
                                b = risElab.Bands.Count
                            End If
                            b += 1
                        End While
                        Dim udm_cod As Integer = 5001049
                        Dim Data_Riferimento As DateTime = Data_Elaborazione
                        Dim av_cod As Integer = indic.Parametri.Avv_Cod
                        Dim RisultatoElaborazione As String = JsonConvert.SerializeObject(risElab)

                        Try

                            writer.Scrivi(Tipo_Sorgente, Stazione_Cod, Mod_Cod, 0, parElab.ToString, 0, Data_Elaborazione, RisultatoElaborazione,
                                  Qta_Ril, udm_cod, Data_Riferimento, av_cod, mObjParametri_Server)

                        Catch ex As Exception

                            'Violazione chiave primaria...
                            Dim errr = 0
                        End Try

                    End If
                Next
            End If
        Next

        Return True
    End Function



    Private Function LeggiIndicatoriXSpecieXAvversita() As DataTable

        Dim objParams As New JObject(New JProperty("Doorkey", "Y4h8u3B5w2"),
                                     New JProperty("PIVA_Superuser", mObjParametri_Server.PivaSuperUser))

        Dim objMeteo As New AgronicaCoreWebService.MeteoNT

        Dim ss As String = objMeteo.LeggiTuttiModelliAutorizzati(objParams, mObjParametri_Server)

        Dim jss = New JavaScriptSerializer With {
            .MaxJsonLength = Integer.MaxValue
        }

        Dim r = jss.Deserialize(Of RispostaStandard)(ss)

        Dim dt As New DataTable
        dt.Columns.Add("Veg_Cod", GetType(Integer))
        dt.Columns.Add("Mod_Cod", GetType(Integer))
        dt.Columns.Add("Avv_Cod", GetType(Integer))

        If r.RispostaOK Then

            Dim modelliXindicatori As Integer() = {
                enum_ModelliPrevisionali.Agronomica30_Peronospora,
                enum_ModelliPrevisionali.Agronomica30_OidioVite,
                enum_ModelliPrevisionali.Agronomica30_BotriteVite,
                enum_ModelliPrevisionali.Agronomica30_BatteriosiKiwi_PSA,
                enum_ModelliPrevisionali.Agronomica30_TicchiolaturaMelo,
                enum_ModelliPrevisionali.Agronomica30_MaculaturaPero,
                enum_ModelliPrevisionali.Racca_PeroPom,
                enum_ModelliPrevisionali.Racca_AlterPom,
                enum_ModelliPrevisionali.Racca_OidioPom,
                enum_ModelliPrevisionali.Racca_BotriPom,
                enum_ModelliPrevisionali.Racca_PeroBiet,
                enum_ModelliPrevisionali.Racca_OidioBiet,
                enum_ModelliPrevisionali.Racca_CercoBiet,
                enum_ModelliPrevisionali.Racca_PeroPat,
                enum_ModelliPrevisionali.Racca_AlterPat,
                enum_ModelliPrevisionali.Racca_ScleroSoia,
                enum_ModelliPrevisionali.Racca_BrusoneRiso,
                enum_ModelliPrevisionali.UniCatt_Mais_AFLA,
                enum_ModelliPrevisionali.UniCatt_Mais_FER
            }

            Dim msa_arr = JArray.Parse(r.RispostaStringa)

            For Each msa In msa_arr

                Dim mod_cod As Integer = msa("Mod_Cod")

                If modelliXindicatori.Contains(mod_cod) Then

                    Dim dr = dt.NewRow()

                    dr("Veg_Cod") = CInt(msa("Veg_Cod"))
                    dr("Mod_Cod") = mod_cod
                    dr("Avv_Cod") = CInt(msa("Av_Cod"))

                    dt.Rows.Add(dr)

                End If
            Next
        End If

        Return dt
    End Function



    Private Function ScriviRichiestaGSB(arrModelli As JArray) As RisultatoElaborazioneIndicatori

        Dim DataCalcolo As DateTime = Date.Now().Date

        Dim objRequest = New JObject(New JProperty("ElencoModelli", arrModelli))
        'If Not String.IsNullOrEmpty(parExtra) Then
        '    objRequest.Add("ParametriExtra", JObject.Parse(parExtra))
        'End If

        Dim objParams As New JObject(New JProperty("Doorkey", "Y4h8u3B5w2"),
                                         New JProperty("PivaSuperuser", mObjParametri_Server.PivaSuperUser),
                                         New JProperty("Piva", mObjParametri_Server.PivaSuperUser),
                                         New JProperty("Request", objRequest))


        Dim objMeteo As New AgronicaCoreWebService.MeteoNT

        Dim ss As String = objMeteo.ModelliPrevisionaliElaboraIndicatori(objParams, mObjParametri_Server)

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }

        Dim r = jss.Deserialize(Of rispostaStandard(Of RisultatoElaborazioneIndicatori))(ss)

        Return r.RispostaStringa
    End Function

End Class
