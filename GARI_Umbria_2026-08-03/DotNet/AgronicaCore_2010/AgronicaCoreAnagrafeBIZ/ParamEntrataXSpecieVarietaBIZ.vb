
Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider
Imports System.Transactions
Imports System.Web
Imports System.Linq

Public Class ParamEntrataXSpecieVarietaBIZ
    Inherits AgronicaCoreDataProvider.LogProvider


#Region "Costruttori"

    Public Sub New()
        Provider = System.Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As System.Globalization.CultureInfo
    Public Shadows Property Provider() As System.Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property


    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    '##############################################################################################

    'Public Function InserisciTestataGriglia(
    '        ByVal campConfTestataGriglia As CampionamentoConferito_TestataGriglia,
    '        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '    ) As String

    '    Dim MessaggioErrore As String = String.Empty

    '    Dim NomeRoutine As String = "ContabBIZ.FF_CampionamentoConferimento.InserisciTestataGriglia()"

    '    Try

    '        Dim campConf_R As New FF_CampionamentoConferimento_R

    '        'Controllo che non vi siano altre testate con periodi sovrapposti
    '        If String.IsNullOrEmpty(MessaggioErrore) Then
    '            Dim myQuery As IQueryable(Of CampionamentoConferito_TestataGriglia) =
    '                campConf_R.Leggi_Elem_Testata_GriglieCampionamento_Generica(campConfTestataGriglia.PIVA,
    '                objParametri)
    '            myQuery = From griglia_testata In myQuery
    '                      Where (
    '                          griglia_testata.Id_TestataGriglia <> campConfTestataGriglia.Id_TestataGriglia And
    '                          griglia_testata.des_TestataGriglia = campConfTestataGriglia.des_TestataGriglia And
    '                        ((campConfTestataGriglia.Validita_Inizio.HasValue And
    '                           griglia_testata.Validita_Inizio <= campConfTestataGriglia.Validita_Inizio And
    '                            griglia_testata.Validita_Fine >= campConfTestataGriglia.Validita_Inizio) Or
    '                            (campConfTestataGriglia.Validita_Fine.HasValue And
    '                           griglia_testata.Validita_Inizio <= campConfTestataGriglia.Validita_Fine And
    '                            griglia_testata.Validita_Fine >= campConfTestataGriglia.Validita_Fine)))
    '            If myQuery.ToList().Count > 0 Then
    '                MessaggioErrore &= "Esistono altre righe con la stessa descrizione e periodi sovrapposti"
    '            End If
    '        End If

    '        'Controlli congruenza sulle date da <= a
    '        If campConfTestataGriglia.Validita_Fine.HasValue And
    '            campConfTestataGriglia.Validita_Inizio.HasValue And
    '            campConfTestataGriglia.Validita_Inizio > campConfTestataGriglia.Validita_Fine Then

    '            MessaggioErrore &= "Valido Da deve essere minore di Valido fino a"

    '        End If

    '        If String.IsNullOrEmpty(MessaggioErrore) Then
    '            Dim campConf_W As New FF_CampionamentoConferimento_W
    '            MessaggioErrore = campConf_W.Inserisci_Elem_Testata_GriglieCampionamento(
    '                campConfTestataGriglia, objParametri)
    '        End If


    '    Catch ex As Exception
    '        MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '    Finally

    '    End Try

    '    If Not String.IsNullOrEmpty(MessaggioErrore) Then
    '    End If

    '    Return MessaggioErrore
    'End Function



    '##############################################################################################

    Public Function Aggiorna_ParamEntrataXSpecieVarieta(
            ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByVal tutteleRighe As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.ParamEntrataXSpecieVarietaBIZ.Aggiorna_ParamEntrataXSpecieVarieta()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim param_R As New ParamEntrataXSpecieVarieta_R

            Dim paramEntrataXSpecV As ParamEntrataXSpecieVarieta
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            If (String.IsNullOrEmpty(MessaggioErrore)) Then
                paramEntrataXSpecV = New ParamEntrataXSpecieVarieta
                Dim trovatoErrore = False
                Dim Inizio As Date
                Dim Fine As Date
                Dim Inizio_confronto As Date
                Dim Fine_confronto As Date
                'Righe Inserite
                For Each obj As JObject In righeInseriteArray
                    Inizio = DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                    Fine = DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing)
                    For Each obj_confronto As JObject In righeInseriteArray
                        Inizio_confronto = DateTime.ParseExact(obj_confronto("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                        Fine_confronto = DateTime.ParseExact(obj_confronto("Validita_Fine").ToString, "yyyyMMdd", Nothing)
                        If (Not obj.Equals(obj_confronto)) AndAlso
                            ((CInt(obj("Veg_Cod")) = CInt(obj_confronto("Veg_Cod"))) AndAlso (CInt(obj("Cul_Cod")) = CInt(obj_confronto("Cul_Cod"))) AndAlso
                            (CInt(obj("Regolamento_Cod")) = CInt(obj_confronto("Regolamento_Cod")))) AndAlso
                               (((Inizio <= Inizio_confronto) AndAlso (Fine >= Inizio_confronto)) OrElse
                                ((Inizio <= Fine_confronto) AndAlso (Fine >= Fine_confronto)) OrElse
                                ((Inizio <= Inizio_confronto) AndAlso (Fine >= Fine_confronto)) OrElse
                                ((Inizio >= Inizio_confronto) AndAlso (Fine <= Fine_confronto))) Then
                            trovatoErrore = True
                        End If
                    Next
                    If (trovatoErrore = False) Then
                        For Each obj_confronto As JObject In righeModificateArray
                            Inizio_confronto = DateTime.ParseExact(obj_confronto("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                            Fine_confronto = DateTime.ParseExact(obj_confronto("Validita_Fine").ToString, "yyyyMMdd", Nothing)
                            'Aggiungere id_Param e fare il controllo se una data è dentro ad un altra!
                            If (Not obj.Equals(obj_confronto)) AndAlso
                                ((CInt(obj("Veg_Cod")) = CInt(obj_confronto("Veg_Cod"))) AndAlso (CInt(obj("Cul_Cod")) = CInt(obj_confronto("Cul_Cod"))) AndAlso
                                (CInt(obj("Regolamento_Cod")) = CInt(obj_confronto("Regolamento_Cod")))) AndAlso
                               (((Inizio <= Inizio_confronto) AndAlso (Fine >= Inizio_confronto)) OrElse
                                ((Inizio <= Fine_confronto) AndAlso (Fine >= Fine_confronto)) OrElse
                                ((Inizio <= Inizio_confronto) AndAlso (Fine >= Fine_confronto)) OrElse
                                ((Inizio >= Inizio_confronto) AndAlso (Fine <= Fine_confronto))) Then

                                trovatoErrore = True
                            End If
                        Next
                    End If
                    If (trovatoErrore = False) Then
                        'Righe inserite
                        paramEntrataXSpecV = New ParamEntrataXSpecieVarieta
                        paramEntrataXSpecV.Piva = piva
                        paramEntrataXSpecV.PivaSuperUser = Piva_SuperUser
                        paramEntrataXSpecV.Veg_Cod = obj("Veg_Cod")
                        paramEntrataXSpecV.Cul_Cod = obj("Cul_Cod")
                        paramEntrataXSpecV.Id_Param = obj("Id_Param")
                        paramEntrataXSpecV.Percentuale_Degrado = obj("Percentuale_Degrado")
                        paramEntrataXSpecV.Riferimento_Prezzi = obj("Riferimento_Prezzi_Cod")
                        paramEntrataXSpecV.FormulaFissaLiquidazione = obj("FormulaFissaLiquidazione_Cod")
                        paramEntrataXSpecV.Reg_Cod = obj("Regolamento_Cod")

                        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                            paramEntrataXSpecV.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
                        End If
                        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                            paramEntrataXSpecV.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
                        End If
                        paramEntrataXSpecV.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                        paramEntrataXSpecV.Username_Creazione = objParametri.UsernameOperazione
                        paramEntrataXSpecV.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                        paramEntrataXSpecV.Username_Modifica = objParametri.UsernameOperazione
                        paramEntrataXSpecV.inviato = 0

                        MessaggioErrore &= CheckParam(paramEntrataXSpecV, objParametri, False, True, obj, righeModificateArray)
                        EFArrayToInsert.Add(paramEntrataXSpecV)
                    End If
                    If trovatoErrore Then
                        ComponiMessaggioErroreParamEntrataXSpecieVarieta(obj, True, MessaggioErrore)
                        Exit For
                    End If
                Next
                If (trovatoErrore = False) Then
                    'Righe Modificate
                    For Each obj As JObject In righeModificateArray
                        Inizio = DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                        Fine = DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing)

                        For Each obj_confronto As JObject In righeModificateArray
                            Inizio_confronto = DateTime.ParseExact(obj_confronto("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                            Fine_confronto = DateTime.ParseExact(obj_confronto("Validita_Fine").ToString, "yyyyMMdd", Nothing)


                            If (Not obj.Equals(obj_confronto)) AndAlso
                                ((CInt(obj("Veg_Cod")) = CInt(obj_confronto("Veg_Cod"))) AndAlso (CInt(obj("Cul_Cod")) = CInt(obj_confronto("Cul_Cod"))) AndAlso
                                (CInt(obj("Regolamento_Cod")) = CInt(obj_confronto("Regolamento_Cod")))) AndAlso
                               (((Inizio <= Inizio_confronto) AndAlso (Fine >= Inizio_confronto)) OrElse
                                ((Inizio <= Fine_confronto) AndAlso (Fine >= Fine_confronto)) OrElse
                                ((Inizio <= Inizio_confronto) AndAlso (Fine >= Fine_confronto)) OrElse
                                ((Inizio >= Inizio_confronto) AndAlso (Fine <= Fine_confronto))) Then

                                trovatoErrore = True
                            End If
                        Next

                        'End If
                        If trovatoErrore = False Then
                            'Righe Modificate
                            'Controllo che non ci siano righe uguali
                            paramEntrataXSpecV = New ParamEntrataXSpecieVarieta
                            paramEntrataXSpecV.Piva = piva
                            paramEntrataXSpecV.PivaSuperUser = Piva_SuperUser
                            paramEntrataXSpecV.Veg_Cod = obj("Veg_Cod")
                            paramEntrataXSpecV.Cul_Cod = obj("Cul_Cod")
                            paramEntrataXSpecV.Riferimento_Prezzi = obj("Riferimento_Prezzi_Cod")
                            paramEntrataXSpecV.Percentuale_Degrado = obj("Percentuale_Degrado")
                            paramEntrataXSpecV.Id_Param = obj("Id_Param")
                            paramEntrataXSpecV.FormulaFissaLiquidazione = obj("FormulaFissaLiquidazione_Cod")
                            paramEntrataXSpecV.Reg_Cod = obj("Regolamento_Cod")

                            If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                                paramEntrataXSpecV.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
                            End If
                            If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                                paramEntrataXSpecV.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
                            End If
                            If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                                paramEntrataXSpecV.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                            End If

                            paramEntrataXSpecV.Username_Creazione = obj("Username_Creazione").ToString
                            paramEntrataXSpecV.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                            paramEntrataXSpecV.Username_Modifica = objParametri.UsernameOperazione
                            paramEntrataXSpecV.inviato = obj("inviato")


                            MessaggioErrore &= CheckParam(paramEntrataXSpecV, objParametri, True, True, obj, righeModificateArray)
                            EFArrayToUpdate.Add(paramEntrataXSpecV)

                        End If
                        If trovatoErrore Then
                            ComponiMessaggioErroreParamEntrataXSpecieVarieta(obj, False, MessaggioErrore)
                            Exit For
                        End If
                    Next
                End If

                For Each obj As JObject In righeCancellateArray
                    'Righe cancellate
                    paramEntrataXSpecV = New ParamEntrataXSpecieVarieta With {
                        .PivaSuperUser = Piva_SuperUser,
                        .Piva = piva,
                        .Id_Param = obj("Id_Param"),
                        .Veg_Cod = obj("Veg_Cod"),
                        .Cul_Cod = obj("Cul_Cod")
                    }


                    MessaggioErrore &= CheckParam(paramEntrataXSpecV, objParametri, True, False, obj, righeModificateArray, True)
                    EFArrayToDelete.Add(paramEntrataXSpecV)
                Next

            End If

            If String.IsNullOrEmpty(MessaggioErrore) Then

                ' Controllo che non si possano fare modifiche se ci sono movimenti collegati e che non ci siano periodi sovrapposti già registrati
                MessaggioErrore &= ControllaPeriodiSovrapposti(tutteleRigheArray, righeInseriteArray, righeModificateArray)

                If String.IsNullOrEmpty(MessaggioErrore) Then
                    'Parte Di scrittura
                    Dim param_W As New ParamEntrataXSpecieVarieta_W
                    MessaggioErrore = param_W.Scrivi(piva, EFArrayToInsert, EFArrayToUpdate,
                        EFArrayToDelete, objParametri)
                End If


            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function


    Private Function ControllaPeriodiSovrapposti(ByVal tutteleRigheArray As JArray,
                                                 ByVal righeInseriteArray As JArray,
                                                 ByVal righeModificateArray As JArray)

        Dim MessaggioErrore As String = String.Empty

        If righeInseriteArray IsNot Nothing Then
            'Controllo che non ci siano righe inserite con validita sovrapposte che hanno diversi prezzi ma stesse unita di misura
            For Each ins As JObject In righeInseriteArray

                Dim ins_veg_cod = CInt(ins("Veg_Cod"))
                Dim ins_cul_cod = CInt(ins("Cul_Cod"))
                Dim ins_reg_cod = CInt(ins("Regolamento_Cod"))
                Dim ins_validitaInizio = DateTime.ParseExact(ins("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                Dim ins_validitaFine = DateTime.ParseExact(ins("Validita_Fine").ToString, "yyyyMMdd", Nothing)

                Dim esistenti = tutteleRigheArray.Where(Function(f) CInt(f("Veg_Cod")) = ins_veg_cod AndAlso
                                   CInt(f("Cul_Cod")) = ins_cul_cod AndAlso CInt(f("Regolamento_Cod")) = ins_reg_cod AndAlso
                                  ((DateTime.ParseExact(f("Validita_Fine").ToString, "yyyyMMdd", Nothing) >= ins_validitaInizio AndAlso
                                    (DateTime.ParseExact(f("Validita_Inizio").ToString, "yyyyMMdd", Nothing) <= ins_validitaFine))))

                If esistenti.Count > 1 Then
                    ComponiMessaggioErroreParamEntrataXSpecieVarieta(ins, True, MessaggioErrore)
                    Exit For
                End If
            Next
        End If

        If righeModificateArray IsNot Nothing Then
            'Controllo che non ci siano righe inserite con validita sovrapposte che hanno diversi prezzi ma stesse unita di misura
            For Each modi As JObject In righeModificateArray

                Dim modi_veg_cod = CInt(modi("Veg_Cod"))
                Dim modi_cul_cod = CInt(modi("Cul_Cod"))
                Dim modi_reg_cod = CInt(modi("Regolamento_Cod"))
                Dim modi_validitaInizio = DateTime.ParseExact(modi("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
                Dim modi_validitaFine = DateTime.ParseExact(modi("Validita_Fine").ToString, "yyyyMMdd", Nothing)

                Dim esistenti = tutteleRigheArray.Where(Function(f) CInt(f("Veg_Cod")) = modi_veg_cod AndAlso
                                   CInt(f("Cul_Cod")) = modi_cul_cod AndAlso CInt(f("Regolamento_Cod")) = modi_reg_cod AndAlso
                                  ((DateTime.ParseExact(f("Validita_Fine").ToString, "yyyyMMdd", Nothing) >= modi_validitaInizio AndAlso
                                    (DateTime.ParseExact(f("Validita_Inizio").ToString, "yyyyMMdd", Nothing) <= modi_validitaFine))))

                If esistenti.Count > 1 Then
                    ComponiMessaggioErroreParamEntrataXSpecieVarieta(modi, False, MessaggioErrore)
                    Exit For
                End If
            Next
        End If


        Return MessaggioErrore
    End Function


    Public Function CheckParam(
            ByVal paramEntrataXSpecV As ParamEntrataXSpecieVarieta,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef controllaSeGiaUtilizzato As Boolean,
            ByRef controllaSePeriodiSovrapposti As Boolean,
            ByVal obj As JObject,
            ByVal righeModificateArray As JArray,
            ByVal Optional righe_cancellate As Boolean? = False
            ) As String

        Dim messaggioErrore As String = String.Empty

        Dim param_R As New ParamEntrataXSpecieVarieta_R

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.ParamEntrataXSpecieVarietaBIZ.Aggiorna_ParamEntrataXSpecieVarieta()"

        Try

            'Controlli congruenza sulle date da <= a
            If paramEntrataXSpecV.Validita_Inizio > paramEntrataXSpecV.Validita_Fine And righe_cancellate <> True Then
                If obj("Cul_Des") Is Nothing OrElse obj("Cul_Des") = "" Then
                    messaggioErrore &= "Nella riga con Specie " & obj("Veg_Des").ToString & ": Valido Da " & DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing) & " deve essere minore di Valido fino a " & DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing) & " <br/>"
                ElseIf righe_cancellate <> True Then
                    messaggioErrore &= "Nella riga con Specie " & obj("Veg_Des").ToString & " , Varietà " & obj("Cul_Des").ToString & ": Valido Da " & DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing) & " deve essere minore di Valido fino a " & DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing) & " <br/>"
                End If

            End If

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        End Try

        Return messaggioErrore

    End Function


    Private Sub ComponiMessaggioErroreParamEntrataXSpecieVarieta(ByVal obj As JObject,
                                                                ByVal rigaInserita As Boolean,
                                                                ByRef MessaggioErrore As String)

        'Se rigaInserita = False allora devo comporre il messaggio per la riga modificata
        If rigaInserita = False Then
            If Not String.IsNullOrEmpty(obj("Cul_Des").ToString) AndAlso Not String.IsNullOrEmpty(obj("Regolamento_Des").ToString) Then
                MessaggioErrore &= "Impossibile completare la modifica. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & ", Varietà " & obj("Cul_Des").ToString & ", Regolamento " & obj("Regolamento_Des").ToString & " e periodi sovrapposti <br/>"
            Else
                If String.IsNullOrEmpty(obj("Cul_Des").ToString) AndAlso String.IsNullOrEmpty(obj("Regolamento_Des").ToString) Then
                    MessaggioErrore &= "Impossibile completare la modifica. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & " e periodi sovrapposti <br/>"
                Else
                    If Not String.IsNullOrEmpty(obj("Cul_Des").ToString) Then
                        MessaggioErrore &= "Impossibile completare la modifica. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & ", Varietà " & obj("Cul_Des").ToString & " e periodi sovrapposti <br/>"
                    ElseIf Not String.IsNullOrEmpty(obj("Regolamento_Des").ToString) Then
                        MessaggioErrore &= "Impossibile completare la modifica. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & ", Regolamento " & obj("Regolamento_Des").ToString & " e periodi sovrapposti <br/>"
                    End If
                End If
            End If
        ElseIf rigaInserita = True Then
            If Not String.IsNullOrEmpty(obj("Cul_Des").ToString) AndAlso Not String.IsNullOrEmpty(obj("Regolamento_Des").ToString) Then
                MessaggioErrore &= "Impossibile completare l'inserimento. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & ", Varietà " & obj("Cul_Des").ToString & ", Regolamento " & obj("Regolamento_Des").ToString & " e periodi sovrapposti <br/>"
            Else
                If String.IsNullOrEmpty(obj("Cul_Des").ToString) AndAlso String.IsNullOrEmpty(obj("Regolamento_Des").ToString) Then
                    MessaggioErrore &= "Impossibile completare l'inserimento. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & " e periodi sovrapposti <br/>"
                Else
                    If Not String.IsNullOrEmpty(obj("Cul_Des").ToString) Then
                        MessaggioErrore &= "Impossibile completare l'inserimento. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & ", Varietà " & obj("Cul_Des").ToString & " e periodi sovrapposti <br/>"
                    ElseIf Not String.IsNullOrEmpty(obj("Regolamento_Des").ToString) Then
                        MessaggioErrore &= "Impossibile completare l'inserimento. Perchè esistono altre righe con la stessa Specie " & obj("Veg_Des").ToString & ", Regolamento " & obj("Regolamento_Des").ToString & " e periodi sovrapposti <br/>"
                    End If
                End If
            End If
        End If


    End Sub
End Class