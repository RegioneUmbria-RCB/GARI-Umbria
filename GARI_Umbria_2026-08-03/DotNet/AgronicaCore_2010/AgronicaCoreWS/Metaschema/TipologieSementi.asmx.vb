Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class TipologieSementi
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboTipologieSementi_NG(ByVal InData As CoreWS_Generic(Of CaricaComboTipologieSementi)) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)


            Dim objCom As New AgronicaCoreMetaSchemaDAL.TipologieSementi_R
            Dim Dt As DataTable = objCom.Leggi(0, "",
                                               InData.InData.FiltroAggiuntivo,
                                                InData.InData.Ordinamento,
                                                objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("sem_cod", dr.Item("sem_COD")), New JProperty("sem_des", dr.Item("sem_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboTipologieSementi(FiltroAggiuntivo As String,
                                                         Ordinamento As String,
                                                         objP_server As String) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)


            Dim objCom As New AgronicaCoreMetaSchemaDAL.TipologieSementi_R
            Dim Dt As DataTable = objCom.Leggi(0, "",
                                               FiltroAggiuntivo,
                                                Ordinamento,
                                                objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("sem_cod", dr.Item("sem_COD")), New JProperty("sem_des", dr.Item("sem_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    'DA TESTARE
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboTipologieSementiByVeg_Cod_NG(ByVal InData As CoreWS_Generic(Of CaricaComboTipologieSementiByVeg_Cod)) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementiByVeg_Cod()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)


            Dim objCom As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
            Dim Dt As DataTable = objCom.Leggi(InData.InData.Veg_Cod,
                                               0,
                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                InData.InData.FiltroAggiuntivo,
                                                InData.InData.Ordinamento,
                                                objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("sem_cod", dr.Item("sem_COD")), New JProperty("sem_des", dr.Item("sem_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboTipologieSementiByVeg_Cod(Veg_Cod As Integer,
                                                           FiltroAggiuntivo As String,
                                                         Ordinamento As String,
                                                         objP_server As String) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementiByVeg_Cod()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)


            Dim objCom As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
            Dim Dt As DataTable = objCom.Leggi(Veg_Cod,
                                               0,
                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                FiltroAggiuntivo,
                                                Ordinamento,
                                                objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("sem_cod", dr.Item("sem_COD")), New JProperty("sem_des", dr.Item("sem_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    '###############################################################################
    'Questa CaricaCombo è simile a CaricaCombo_SpecieVegetale_Semina e a CaricaCheckBoxList_SpecieVegetale_Optimize
    'se si modifica il meccanismo in una, verificare anche le altre
    '------------------------------------------------------------------------------
    'Se Flag_FiltroUtente = true, Filtra le Specie a seconda  
    'del FILTRO impostato sull'utente in Utenti_Impostazioni.
    '
    'Il Veg_Cod_daModificare è il veg_cod della semente
    'che bisogna caricare nella combo, anche se non è incluso nel filtro specie vegetali associato all'utente
    '(altrimenti si genera un errore)
    '###############################################################################
    Public Sub CaricaCombo_SpecieVegetale_Semente(ByVal Sem_Cod As Integer,
                                                  ByVal objP_server As String,
                                                 ByVal objP_utenti As String,
                                                Optional ByVal Veg_Cod As Integer = 0,
                                                Optional ByVal Flag_PrimaRiga As Boolean = True,
                                                Optional ByVal Testo_PrimaRiga As String = "",
                                                Optional ByVal Cod_PrimaRiga As String = "",
                                                Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
                                                Optional ByVal FinestraTemp_Fine As String = "31/12/2100",
                                                Optional ByVal FiltroAggiuntivo As String = "",
                                                Optional ByVal Ordinamento As String = "",
                                                Optional ByVal Flag_FiltroUtente As Boolean = False,
                                                Optional ByVal Veg_Cod_daModificare As Integer = 0,
                                                Optional ByVal SemCod_Rif_VegCod_daModificare As Integer = 0)



        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Impostazione_Valore_1 As String = ""
        'Dim Flag_FiltroUtenteImpostato As Boolean = False
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False
        Dim objTSxSp As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
        Dim objSP As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        Dim JArrayLista As New JArray()

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)


        If Flag_FiltroUtente = True Then

            '''VECCHIA GESTIONE
            '''Attenzione! Poichè il filtro sulla specie è un filtro utente,
            '''va passata come username, la username dell'utente, non quella del superuser
            ''Impostazione_Valore_1 = ImpostazioneValore1_from_ImpostazioneCod(objServer, objSession, objPage, _
            ''                                                                CStr(objSession("ASG_SuperUser_CodFiscale")), _
            ''                                                                CStr(objSession("ASG_Utente_Username")), _
            ''                                                                COD_FILTRO_SPECIE_VEGETALI)

            ''If Impostazione_Valore_1 <> "" Then
            ''    Flag_FiltroUtenteImpostato = True
            ''    FiltroAggiuntivo += " AND ( TipologieSementixSpecieVegetali.Veg_Cod IN (" & Impostazione_Valore_1 & ") ) "
            ''End If

            'Dt = NewCom_TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi(objServer, objSession, objPage,
            '                                                                        Sem_Cod,
            '                                                                        Veg_Cod,
            '                                                                        Ordinamento,
            '                                                                        FiltroAggiuntivo,
            '                                                                        FinestraTemp_Inizio,
            '                                                                        FinestraTemp_Fine)

            Dt = objTSxSp.TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi(Sem_Cod,
                                                                                    Veg_Cod,
                                                                                    FiltroAggiuntivo,
                                                                                     Ordinamento,
                                                                                     objParametri_Server,
                                                                                     objParametri_Utenti,
                                                                                    FinestraTemp_Inizio,
                                                                                    FinestraTemp_Fine)


        Else

            'Dt = NewCom_TipologieSementixSpecieVegetali_Leggi(objServer, objSession, objPage,
            '                                        Sem_Cod,
            '                                        Veg_Cod,
            '                                        Ordinamento,
            '                                        FiltroAggiuntivo,
            '                                        FinestraTemp_Inizio,
            '                                        FinestraTemp_Fine)

            Dt = objTSxSp.Leggi2(Veg_Cod,
                                Sem_Cod,
                                FiltroAggiuntivo,
                                Ordinamento,
                                objParametri_Server)

        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            'uso il dataview per ordinare
            Dim Dv As New DataView

            Dt.TableName = "specie"
            Dv.Table = Dt
            Dv.Sort = "Veg_Des ASC"

            If Not IsNothing(Dv) Then

                For i = 0 To NumTotale - 1

                    If Dv.Item(i).Item("Gru_COD") <> -1 Then

                        'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                        'faccio il controllo
                        If Veg_Cod_daModificare <> 0 Then

                            ''se l'utente ha il filtro specie impostato
                            'If Flag_FiltroUtenteImpostato = True Then

                            'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                            If Dv.Item(i).Item("Veg_Cod") = Veg_Cod_daModificare Then
                                Flag_VegCodTrovatoNelFiltroUtente = True
                            End If
                            'Else
                            '    'non c'è un filtro impostato
                            '    'quindi imposto il flag, come se avessi trovato la specie
                            '    'così non devo aggiungere niente
                            '    Flag_VegCodTrovatoNelFiltroUtente = True
                            'End If
                        Else
                            'non è stato passato il veg_cod
                            'quindi imposto il flag, come se avessi trovato la specie
                            'così non devo aggiungere niente
                            Flag_VegCodTrovatoNelFiltroUtente = True
                        End If


                        x_Cod = Dv.Item(i).Item("Veg_COD")
                        x_Des = CStr(Dv.Item(i).Item("Veg_DES"))
                        'Cmb.Items.Add(New ListItem(x_Des, x_Cod))

                        JArrayLista.Add(New JObject(New JProperty("veg_cod", x_Cod), New JProperty("veg_des", x_Des)))


                    End If

                Next

            End If

            Dv = Nothing

        Else
            NumTotale = 0
        End If

        Dt = Nothing



        'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
        If Veg_Cod_daModificare <> 0 Then

            ''se l'utente ha il filtro specie impostato
            'If Flag_FiltroUtenteImpostato = True Then

            'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
            If Flag_VegCodTrovatoNelFiltroUtente = False Then

                x_Des = ""

                'se il Veg_Cod_daModificare è valorizzato, devo sapere anche il sem_cod
                '(se non l'ho passato me lo ricavo )
                'però solo nel caso di sem_cod <> 0
                If Sem_Cod <> 0 And SemCod_Rif_VegCod_daModificare = 0 Then
                    SemCod_Rif_VegCod_daModificare = objTSxSp.SemCod_from_VegCod(Veg_Cod_daModificare, "", "", objParametri_Server)
                End If

                'se il sem_cod delal specie da modificare è lo stesso di quello selezionato,
                'allora inserisco la specie vegetale da modificare nel menù
                If Sem_Cod = SemCod_Rif_VegCod_daModificare Then

                    'se la descrizione non l'ho già ricavata sopra
                    If x_Des = "" Then
                        x_Des = objSP.VegDes_from_VegCod(Veg_Cod_daModificare, objParametri_Server)
                    End If

                    'Cmb.Items.Add(New ListItem(x_Des, Veg_Cod_daModificare))
                    JArrayLista.Add(New JObject(New JProperty("veg_cod", Veg_Cod_daModificare), New JProperty("veg_des", x_Des)))

                End If

            End If

            'End If

        End If


    End Sub

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaCombo_SpecieVegetale_Semente_New_NG(ByVal InData As CoreWS_Generic(Of CaricaCombo_SpecieVegetale_Semente_New)) As RispostaStandard

        Dim r As New RispostaStandard
        Try

            Dim Dt As DataTable
            Dim i As Integer
            Dim x_Cod As Integer
            Dim x_Des As String
            Dim NumTotale As Integer
            Dim Impostazione_Valore_1 As String = ""
            'Dim Flag_FiltroUtenteImpostato As Boolean = False
            Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False
            Dim objTSxSp As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
            Dim objSP As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

            Dim JArrayLista As New JArray()

            If InData.InData.Flag_PrimaRiga = True Then
                JArrayLista.Add(New JObject(New JProperty("veg_cod", InData.InData.Cod_PrimaRiga), New JProperty("veg_des", InData.InData.Testo_PrimaRiga)))
            End If

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)


            If InData.InData.Flag_FiltroUtente = True Then

                Dt = objTSxSp.TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi(InData.InData.Sem_Cod,
                                                                                        InData.InData.Veg_Cod,
                                                                                        InData.InData.FiltroAggiuntivo,
                                                                                         InData.InData.Ordinamento,
                                                                                         objParametri_Server,
                                                                                         objParametri_Utenti,
                                                                                        InData.InData.FinestraTemp_Inizio,
                                                                                        InData.InData.FinestraTemp_Fine)


            Else

                Dt = objTSxSp.Leggi2(InData.InData.Veg_Cod,
                                    InData.InData.Sem_Cod,
                                    InData.InData.FiltroAggiuntivo,
                                    InData.InData.Ordinamento,
                                    objParametri_Server)

            End If


            If Not IsNothing(Dt) Then

                NumTotale = Dt.Rows.Count

                'uso il dataview per ordinare
                Dim Dv As New DataView

                Dt.TableName = "specie"
                Dv.Table = Dt
                Dv.Sort = "Veg_Des ASC"

                If Not IsNothing(Dv) Then

                    For i = 0 To NumTotale - 1

                        If Dv.Item(i).Item("Gru_COD") <> -1 Then

                            'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                            'faccio il controllo
                            If InData.InData.Veg_Cod_daModificare <> 0 Then

                                ''se l'utente ha il filtro specie impostato
                                'If Flag_FiltroUtenteImpostato = True Then

                                'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                                If Dv.Item(i).Item("Veg_Cod") = InData.InData.Veg_Cod_daModificare Then
                                    Flag_VegCodTrovatoNelFiltroUtente = True
                                End If
                            Else
                                'non è stato passato il veg_cod
                                'quindi imposto il flag, come se avessi trovato la specie
                                'così non devo aggiungere niente
                                Flag_VegCodTrovatoNelFiltroUtente = True
                            End If


                            x_Cod = Dv.Item(i).Item("Veg_COD")
                            x_Des = CStr(Dv.Item(i).Item("Veg_DES"))
                            'Cmb.Items.Add(New ListItem(x_Des, x_Cod))

                            JArrayLista.Add(New JObject(New JProperty("veg_cod", x_Cod), New JProperty("veg_des", x_Des)))


                        End If

                    Next

                End If

                Dv = Nothing

            Else
                NumTotale = 0
            End If

            Dt = Nothing



            'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
            If InData.InData.Veg_Cod_daModificare <> 0 Then

                ''se l'utente ha il filtro specie impostato
                'If Flag_FiltroUtenteImpostato = True Then

                'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
                If Flag_VegCodTrovatoNelFiltroUtente = False Then

                    x_Des = ""

                    'se il Veg_Cod_daModificare è valorizzato, devo sapere anche il sem_cod
                    '(se non l'ho passato me lo ricavo )
                    'però solo nel caso di sem_cod <> 0
                    If InData.InData.Sem_Cod <> 0 And InData.InData.SemCod_Rif_VegCod_daModificare = 0 Then
                        InData.InData.SemCod_Rif_VegCod_daModificare = objTSxSp.SemCod_from_VegCod(InData.InData.Veg_Cod_daModificare, "", "", objParametri_Server)
                    End If

                    'se il sem_cod delal specie da modificare è lo stesso di quello selezionato,
                    'allora inserisco la specie vegetale da modificare nel menù
                    If InData.InData.Sem_Cod = InData.InData.SemCod_Rif_VegCod_daModificare Then

                        'se la descrizione non l'ho già ricavata sopra
                        If x_Des = "" Then
                            x_Des = objSP.VegDes_from_VegCod(InData.InData.Veg_Cod_daModificare, objParametri_Server)
                        End If

                        'Cmb.Items.Add(New ListItem(x_Des, Veg_Cod_daModificare))
                        JArrayLista.Add(New JObject(New JProperty("veg_cod", InData.InData.Veg_Cod_daModificare), New JProperty("veg_des", x_Des)))

                    End If

                End If

                'End If

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaCombo_SpecieVegetale_Semente_New(ByVal Sem_Cod As Integer,
                                                              ByVal objP_server As String,
                                                             ByVal objP_utenti As String,
                                                             ByVal Veg_Cod As Integer,
                                                             ByVal Flag_PrimaRiga As Boolean,
                                                             ByVal Testo_PrimaRiga As String,
                                                             ByVal Cod_PrimaRiga As String,
                                                             ByVal FinestraTemp_Inizio As String,
                                                             ByVal FinestraTemp_Fine As String,
                                                             ByVal FiltroAggiuntivo As String,
                                                             ByVal Ordinamento As String,
                                                             ByVal Flag_FiltroUtente As Boolean,
                                                             ByVal Veg_Cod_daModificare As Integer,
                                                             ByVal SemCod_Rif_VegCod_daModificare As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Try

            Dim Dt As DataTable
            Dim i As Integer
            Dim x_Cod As Integer
            Dim x_Des As String
            Dim NumTotale As Integer
            Dim Impostazione_Valore_1 As String = ""
            'Dim Flag_FiltroUtenteImpostato As Boolean = False
            Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False
            Dim objTSxSp As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
            Dim objSP As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

            Dim JArrayLista As New JArray()

            If Flag_PrimaRiga = True Then
                JArrayLista.Add(New JObject(New JProperty("veg_cod", Cod_PrimaRiga), New JProperty("veg_des", Testo_PrimaRiga)))
            End If

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)


            If Flag_FiltroUtente = True Then

                Dt = objTSxSp.TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi(Sem_Cod,
                                                                                        Veg_Cod,
                                                                                        FiltroAggiuntivo,
                                                                                         Ordinamento,
                                                                                         objParametri_Server,
                                                                                         objParametri_Utenti,
                                                                                        FinestraTemp_Inizio,
                                                                                        FinestraTemp_Fine)


            Else

                Dt = objTSxSp.Leggi2(Veg_Cod,
                                    Sem_Cod,
                                    FiltroAggiuntivo,
                                    Ordinamento,
                                    objParametri_Server)

            End If


            If Not IsNothing(Dt) Then

                NumTotale = Dt.Rows.Count

                'uso il dataview per ordinare
                Dim Dv As New DataView

                Dt.TableName = "specie"
                Dv.Table = Dt
                Dv.Sort = "Veg_Des ASC"

                If Not IsNothing(Dv) Then

                    For i = 0 To NumTotale - 1

                        If Dv.Item(i).Item("Gru_COD") <> -1 Then

                            'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                            'faccio il controllo
                            If Veg_Cod_daModificare <> 0 Then

                                ''se l'utente ha il filtro specie impostato
                                'If Flag_FiltroUtenteImpostato = True Then

                                'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                                If Dv.Item(i).Item("Veg_Cod") = Veg_Cod_daModificare Then
                                    Flag_VegCodTrovatoNelFiltroUtente = True
                                End If
                            Else
                                'non è stato passato il veg_cod
                                'quindi imposto il flag, come se avessi trovato la specie
                                'così non devo aggiungere niente
                                Flag_VegCodTrovatoNelFiltroUtente = True
                            End If


                            x_Cod = Dv.Item(i).Item("Veg_COD")
                            x_Des = CStr(Dv.Item(i).Item("Veg_DES"))
                            'Cmb.Items.Add(New ListItem(x_Des, x_Cod))

                            JArrayLista.Add(New JObject(New JProperty("veg_cod", x_Cod), New JProperty("veg_des", x_Des)))


                        End If

                    Next

                End If

                Dv = Nothing

            Else
                NumTotale = 0
            End If

            Dt = Nothing



            'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
            If Veg_Cod_daModificare <> 0 Then

                ''se l'utente ha il filtro specie impostato
                'If Flag_FiltroUtenteImpostato = True Then

                'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
                If Flag_VegCodTrovatoNelFiltroUtente = False Then

                    x_Des = ""

                    'se il Veg_Cod_daModificare è valorizzato, devo sapere anche il sem_cod
                    '(se non l'ho passato me lo ricavo )
                    'però solo nel caso di sem_cod <> 0
                    If Sem_Cod <> 0 And SemCod_Rif_VegCod_daModificare = 0 Then
                        SemCod_Rif_VegCod_daModificare = objTSxSp.SemCod_from_VegCod(Veg_Cod_daModificare, "", "", objParametri_Server)
                    End If

                    'se il sem_cod delal specie da modificare è lo stesso di quello selezionato,
                    'allora inserisco la specie vegetale da modificare nel menù
                    If Sem_Cod = SemCod_Rif_VegCod_daModificare Then

                        'se la descrizione non l'ho già ricavata sopra
                        If x_Des = "" Then
                            x_Des = objSP.VegDes_from_VegCod(Veg_Cod_daModificare, objParametri_Server)
                        End If

                        'Cmb.Items.Add(New ListItem(x_Des, Veg_Cod_daModificare))
                        JArrayLista.Add(New JObject(New JProperty("veg_cod", Veg_Cod_daModificare), New JProperty("veg_des", x_Des)))

                    End If

                End If

                'End If

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class