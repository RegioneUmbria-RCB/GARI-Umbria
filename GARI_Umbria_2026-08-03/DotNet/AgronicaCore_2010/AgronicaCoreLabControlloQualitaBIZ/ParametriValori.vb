Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL

Public Class ParametriValori_R

    Const UTENTE_MIRROR As String = ""

    Public Function leggi_LCQ_ParametriValori( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Documento_Cod As Integer? = Nothing, _
                              Optional ByVal PrmXMod_Cod As Integer? = Nothing, _
                              Optional ByVal MostraMirror As Boolean = False _
                              ) As List(Of LCQ_ParametriValori)

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_R
        Dim pvMirror As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_R
        Dim listaObjLCQ As New List(Of LCQ_ParametriValori)

        Dim dt As DataTable
        If objParametri.UsernameOperazione <> UTENTE_MIRROR AndAlso MostraMirror = False Then
            dt = pvBase.Leggi_ParametriXValoriTabBase(Documento_Cod, PrmXMod_Cod, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "", "", objParametri)
        Else
            dt = pvMirror.Leggi_ParametriXValoriTabMirror(Documento_Cod, PrmXMod_Cod, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "", "", objParametri)
        End If

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ParametriValori( _
                                              dRow("PivaSuperUser") _
                                            , dRow("Documento_Cod") _
                                            , dRow("PrmXMod_Cod") _
                                            , dRow("DataOraRilevazione") _
                                            , UtilityProvider.DBNullToNothing(dRow("Utente")) _
                                            , UtilityProvider.DBNullToNothing(dRow("String_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("DateTime_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("Int_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("Float_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("Provvisorio")) _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_ParametriValori( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Documento_Cod As Integer, _
                              ByVal DaValutare As Boolean, _
                              Optional ByVal MostraMirror As Boolean = False
                              ) As List(Of LCQ_ParametriValori)

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_R
        Dim pvMirror As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_R
        Dim listaObjLCQ As New List(Of LCQ_ParametriValori)

        Dim dt As DataTable
        If objParametri.UsernameOperazione <> UTENTE_MIRROR AndAlso MostraMirror = False Then
            dt = pvBase.Leggi_ParametriXValoriTabBase(Documento_Cod, DaValutare, "", "", objParametri)
        Else
            dt = pvMirror.Leggi_ParametriXValoriTabMirror(Documento_Cod, DaValutare, "", "", objParametri)
        End If

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ParametriValori( _
                                              dRow("PivaSuperUser") _
                                            , dRow("Documento_Cod") _
                                            , dRow("PrmXMod_Cod") _
                                            , dRow("DataOraRilevazione") _
                                            , UtilityProvider.DBNullToNothing(dRow("Utente")) _
                                            , UtilityProvider.DBNullToNothing(dRow("String_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("DateTime_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("Int_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("Float_Valore")) _
                                            , UtilityProvider.DBNullToNothing(dRow("Provvisorio")) _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Function test_ValEsiste( _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByVal Documento_Cod As Integer, _
                             ByVal PrmXMod_Cod As Integer, _
                             Optional ByVal DataOraRilevazione As DateTime? = Nothing _
                             ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_R
        Dim pvMirror As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_R
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then
            res = pvBase.Verifica_ValEsisteTabBase(Documento_Cod, PrmXMod_Cod, DataOraRilevazione, objParametri)
        Else
            res = pvMirror.Verifica_ValEsisteTabMirror(Documento_Cod, PrmXMod_Cod, DataOraRilevazione, objParametri)
        End If

        Return res
    End Function

End Class

Public Class ParametriValori_W

    Const UTENTE_MIRROR As String = ""

#Region "AGGIUNGI PRM-T"

    Public Function aggiungiPrmT_String(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal String_Valore As String, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                                   Documento_Cod, PrmXMod_Cod, _
                                   Nothing, Nothing, _
                                   String_Valore, Nothing, _
                                   Nothing, Nothing, _
                                   Provvisorio)
        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       Nothing, Nothing, _
                       String_Valore, Nothing, _
                       Nothing, Nothing, _
                       Provvisorio)
        End If

        Return res

    End Function

    Public Function aggiungiPrmT_DateTime(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DateTime_Valore As DateTime, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           Nothing, Nothing, _
                           Nothing, DateTime_Valore, _
                           Nothing, Nothing, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       Nothing, Nothing, _
                       Nothing, DateTime_Valore, _
                       Nothing, Nothing, _
                       Provvisorio)
        End If

        Return res

    End Function

    Public Function aggiungiPrmT_Int(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal Int_Valore As Integer, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           Nothing, Nothing, _
                           Nothing, Nothing, _
                           Int_Valore, Nothing, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       Nothing, Nothing, _
                       Nothing, Nothing, _
                       Int_Valore, Nothing, _
                       Provvisorio)
        End If

        Return res

    End Function

    Public Function aggiungiPrmT_Float(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal Double_Valore As Double, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           Nothing, Nothing, _
                           Nothing, Nothing, _
                           Nothing, Double_Valore, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       Nothing, Nothing, _
                       Nothing, Nothing, _
                       Nothing, Double_Valore, _
                       Provvisorio)
        End If

        Return res

    End Function

#End Region

#Region "AGGIUNGI PRM-A"

    Public Function aggiungiPrmA_String(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime, _
                                        ByVal Utente As String, _
                                        ByVal String_Valore As String, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           DataOraRilevazione, Utente, _
                           String_Valore, Nothing, _
                           Nothing, Nothing, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       DataOraRilevazione, Utente, _
                       String_Valore, Nothing, _
                       Nothing, Nothing, _
                       Provvisorio)
        End If

        Return res

    End Function


    Public Function aggiungiPrmA_DateTime(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime, _
                                        ByVal Utente As String, _
                                        ByVal DateTime_Valore As DateTime, _
                                        ByVal LimiteInf As DateTime?, _
                                        ByVal LimiteSup As DateTime?, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           DataOraRilevazione, Utente, _
                           Nothing, DateTime_Valore, _
                           Nothing, Nothing, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            'correggo il dato per salvarlo sul mirror
            If Not IsNothing(LimiteInf) AndAlso DateTime_Valore < LimiteInf Then
                DateTime_Valore = LimiteInf
            ElseIf Not IsNothing(LimiteSup) AndAlso DateTime_Valore > LimiteSup Then
                DateTime_Valore = LimiteSup
            End If
            res = pvm.ScriviTabMirror(objParametri, _
                      Documento_Cod, PrmXMod_Cod, _
                      DataOraRilevazione, Utente, _
                      Nothing, DateTime_Valore, _
                      Nothing, Nothing, _
                      Provvisorio)
        End If

        Return res

    End Function

    Public Function aggiungiPrmA_Int(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime, _
                                        ByVal Utente As String, _
                                        ByVal Int_Valore As Integer, _
                                        ByVal LimiteInf As Integer?, _
                                        ByVal LimiteSup As Integer?, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           DataOraRilevazione, Utente, _
                           Nothing, Nothing, _
                           Int_Valore, Nothing, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror

            'correggo il dato per salvarlo sul mirror
            Int_Valore = aggiustaValorePerMirror(Int_Valore, LimiteInf, LimiteSup)

            res = pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       DataOraRilevazione, Utente, _
                       Nothing, Nothing, _
                       Int_Valore, Nothing, _
                       Provvisorio)
        End If

        Return res

    End Function

    Public Function aggiungiPrmA_Float(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime, _
                                        ByVal Utente As String, _
                                        ByVal Double_Valore As Double, _
                                        ByVal LimiteInf As Double?, _
                                        ByVal LimiteSup As Double?, _
                                        ByVal Provvisorio As Boolean? _
                                        ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ScriviTabBase(objParametri, _
                           Documento_Cod, PrmXMod_Cod, _
                           DataOraRilevazione, Utente, _
                           Nothing, Nothing, _
                           Nothing, Double_Valore, _
                           Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror

            'correggo il dato per salvarlo sul mirror
            Double_Valore = aggiustaValorePerMirror(Double_Valore, LimiteInf, LimiteSup, 2)

            res = pvm.ScriviTabMirror(objParametri, _
                       Documento_Cod, PrmXMod_Cod, _
                       DataOraRilevazione, Utente, _
                       Nothing, Nothing, _
                       Nothing, Double_Valore, _
                       Provvisorio)
        End If

        Return res

    End Function

#End Region

#Region "MODIFICA PRM-T"

    Public Function modificaPrmT_String(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_PrmXMod_Cod As Integer, _
                                ByVal New_String_Valore As String, _
                                ByVal New_Provvisorio As Boolean? _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Nothing, Nothing, _
                              New_String_Valore, Nothing, _
                              Nothing, Nothing, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaTabMirror(objParametri, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Nothing, Nothing, _
                          New_String_Valore, Nothing, _
                          Nothing, Nothing, _
                          New_Provvisorio)
        End If

        Return res

    End Function

    Public Function modificaPrmT_DateTime(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_PrmXMod_Cod As Integer, _
                                ByVal New_DateTime_Valore As DateTime, _
                                ByVal New_Provvisorio As Boolean? _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Nothing, Nothing, _
                              Nothing, New_DateTime_Valore, _
                              Nothing, Nothing, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaTabMirror(objParametri, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Nothing, Nothing, _
                          Nothing, New_DateTime_Valore, _
                          Nothing, Nothing, _
                          New_Provvisorio)
        End If

        Return res

    End Function

    Public Function modificaPrmT_Int(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_PrmXMod_Cod As Integer, _
                                ByVal New_Int_Valore As Integer, _
                                ByVal New_Provvisorio As Boolean? _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Nothing, Nothing, _
                              Nothing, Nothing, _
                              New_Int_Valore, Nothing, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaTabMirror(objParametri, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Nothing, Nothing, _
                          Nothing, Nothing, _
                          New_Int_Valore, Nothing, _
                          New_Provvisorio)
        End If

        Return res

    End Function

    Public Function modificaPrmT_Float(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_PrmXMod_Cod As Integer, _
                                ByVal New_Float_Valore As Double, _
                                ByVal New_Provvisorio As Boolean? _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Nothing, Nothing, _
                              Nothing, Nothing, _
                              Nothing, New_Float_Valore, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaTabMirror(objParametri, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Nothing, Nothing, _
                          Nothing, Nothing, _
                          Nothing, New_Float_Valore, _
                          New_Provvisorio)
        End If

        Return res

    End Function

#End Region

#Region "MODIFICA PRM-A"

    Public Function modificaPrmA_String(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_PrmXMod_Cod As Integer, _
                                ByVal Old_DataOraRilevazione As DateTime, _
                                ByVal New_Utente As String, _
                                ByVal New_String_Valore As String, _
                                ByVal New_Provvisorio As Boolean? _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Old_DataOraRilevazione, New_Utente, _
                              New_String_Valore, Nothing, _
                              Nothing, Nothing, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaTabMirror(objParametri, _
                         Old_Documento_Cod, Old_PrmXMod_Cod, _
                         Old_DataOraRilevazione, New_Utente, _
                         New_String_Valore, Nothing, _
                         Nothing, Nothing, _
                         New_Provvisorio)
        End If

        Return res

    End Function

    Public Function modificaPrmA_DateTime(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_Documento_Cod As Integer, _
                            ByVal Old_PrmXMod_Cod As Integer, _
                            ByVal Old_DataOraRilevazione As DateTime, _
                            ByVal New_Utente As String, _
                            ByVal New_DateTime_Valore As DateTime, _
                            ByVal LimiteInf As DateTime?, _
                            ByVal LimiteSup As DateTime?, _
                            ByVal New_Provvisorio As Boolean? _
                          ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Old_DataOraRilevazione, New_Utente, _
                              Nothing, New_DateTime_Valore, _
                              Nothing, Nothing, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            'correggo il dato per salvarlo sul mirror
            If Not IsNothing(LimiteInf) AndAlso New_DateTime_Valore < LimiteInf Then
                New_DateTime_Valore = LimiteInf
            ElseIf Not IsNothing(LimiteSup) AndAlso New_DateTime_Valore > LimiteSup Then
                New_DateTime_Valore = LimiteSup
            End If

            res = pvm.ModificaTabMirror(objParametri, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Old_DataOraRilevazione, New_Utente, _
                          Nothing, New_DateTime_Valore, _
                          Nothing, Nothing, _
                          New_Provvisorio)
        End If

        Return res

    End Function

    Public Function modificaPrmA_Int(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_Documento_Cod As Integer, _
                            ByVal Old_PrmXMod_Cod As Integer, _
                            ByVal Old_DataOraRilevazione As DateTime, _
                            ByVal New_Utente As String, _
                            ByVal New_Int_Valore As Integer, _
                            ByVal LimiteInf As Integer?, _
                            ByVal LimiteSup As Integer?, _
                            ByVal New_Provvisorio As Boolean? _
                          ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Old_DataOraRilevazione, New_Utente, _
                              Nothing, Nothing, _
                              New_Int_Valore, Nothing, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            'correggo il dato per salvarlo sul mirror
            New_Int_Valore = aggiustaValorePerMirror(New_Int_Valore, LimiteInf, LimiteSup)

            res = pvm.ModificaTabMirror(objParametri, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Old_DataOraRilevazione, New_Utente, _
                          Nothing, Nothing, _
                          New_Int_Valore, Nothing, _
                          New_Provvisorio)
        End If

        Return res

    End Function

    Public Function modificaPrmA_Float(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_Documento_Cod As Integer, _
                            ByVal Old_PrmXMod_Cod As Integer, _
                            ByVal Old_DataOraRilevazione As DateTime, _
                            ByVal New_Utente As String, _
                            ByVal New_Float_Valore As Double, _
                            ByVal LimiteInf As Double?, _
                            ByVal LimiteSup As Double?, _
                            ByVal New_Provvisorio As Boolean? _
                          ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaTabBase(objParametri, _
                              Old_Documento_Cod, Old_PrmXMod_Cod, _
                              Old_DataOraRilevazione, New_Utente, _
                              Nothing, Nothing, _
                              Nothing, New_Float_Valore, _
                              New_Provvisorio)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            'correggo il dato per salvarlo sul mirror
            New_Float_Valore = aggiustaValorePerMirror(New_Float_Valore, LimiteInf, LimiteSup, 2)

            pvm.ModificaTabMirror(objParametri, _
                                      Old_Documento_Cod, Old_PrmXMod_Cod, _
                                      Old_DataOraRilevazione, New_Utente, _
                                      Nothing, Nothing, _
                                      Nothing, New_Float_Valore, _
                                      New_Provvisorio)
        End If

        Return res

    End Function

#End Region

    Public Function cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                            ByVal Documento_Cod As Integer, _
                                            ByVal PrmXMod_Cod As Integer, _
                                            ByVal DataOraRilevazione As DateTime _
                                            ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica
        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.CancellaTabBase(objParametri, Documento_Cod, PrmXMod_Cod, DataOraRilevazione, "")
        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.CancellaTabMirror(objParametri, Documento_Cod, PrmXMod_Cod, DataOraRilevazione, "")
        End If

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res
    End Function

    Public Function cancellaDocumento(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                            ByVal Documento_Cod As Integer _
                                            ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica
        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.CancellaDocumentoTabBase(objParametri, Documento_Cod, "")
        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.CancellaDocumentoTabMirror(objParametri, Documento_Cod, "")
        End If

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res
    End Function

    Public Function modificaOrario(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_DataOraRilevazione As DateTime, _
                                ByVal New_DataOraRilevazione As DateTime _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaOrarioTabBase(objParametri, _
                              Old_Documento_Cod, Old_DataOraRilevazione, New_DataOraRilevazione)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaOrarioTabMirror(objParametri, _
                              Old_Documento_Cod, Old_DataOraRilevazione, New_DataOraRilevazione)
        End If

        Return res

    End Function

    Public Function modificaUtente(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As Integer, _
                                ByVal Old_DataOraRilevazione As DateTime, _
                                ByVal New_Utente As String _
                              ) As Boolean

        Dim pvBase As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabBase_W
        Dim pvm As New AgronicaCoreLabControlloQualitaDAL.ParametriValoriTabMirror_W
        Dim res As Boolean = True

        If objParametri.UsernameOperazione <> UTENTE_MIRROR Then 'scrivo sulla tabella base solo se l'utente non è il mirror
            res = pvBase.ModificaUtenteTabBase(objParametri, _
                              Old_Documento_Cod, Old_DataOraRilevazione, New_Utente)

        End If

        If res Then 'se la modifica sulla tab base (nel caso ci sia stata) è andata a buon fine, lo faccio anche sulla mirror
            res = pvm.ModificaUtenteTabMirror(objParametri, _
                          Old_Documento_Cod, Old_DataOraRilevazione, New_Utente)
        End If

        Return res

    End Function

    Public Function aggiustaValorePerMirror(Int_Valore As Integer, LimiteInf As Integer?, LimiteSup As Integer?) As Integer

        Dim valCorretto As Integer = Int_Valore

        If Not IsNothing(LimiteInf) AndAlso Int_Valore < LimiteInf Then

            'Int_Valore = LimiteInf

            '06/06/2018 Grilli: Algoritmo di Valerio:
            'Range da min a +Inf -> Num casuale tra min e min+20%
            'Range da min a max -> Num casuale tra min e min+(max-min)/4
            Dim minRand As Integer = LimiteInf
            Dim maxRand As Integer

            If IsNothing(LimiteSup) Then
                maxRand = LimiteInf + Math.Ceiling(CDec(LimiteInf) * 20 / 100)
            Else
                maxRand = LimiteInf + Math.Ceiling(CDec((LimiteSup - LimiteInf)) / 4)
            End If

            valCorretto = New Random().Next(minRand, maxRand)

        ElseIf Not IsNothing(LimiteSup) AndAlso Int_Valore > LimiteSup Then
            'Int_Valore = LimiteSup

            '06/06/2018 Grilli: Algoritmo di Valerio:
            'Range da -Inf a max -> Num casuale tra max-20% e max
            'Range da min a max -> Num casuale tra max-(max-min)/4 e max
            Dim minRand As Integer
            Dim maxRand As Integer = LimiteSup

            If IsNothing(LimiteInf) Then
                minRand = LimiteSup - Math.Floor(CDec(LimiteSup) * 20 / 100)
            Else
                minRand = LimiteSup - Math.Floor(CDec((LimiteSup - LimiteInf)) / 4)
            End If

            valCorretto = New Random().Next(minRand, maxRand)

        End If

        Return valCorretto

    End Function

    Public Function aggiustaValorePerMirror(Double_Valore As Double, LimiteInf As Double?, LimiteSup As Double?, nDecimali As Integer) As Double

        Dim valCorretto As Double = Double_Valore

        If Not IsNothing(LimiteInf) AndAlso Double_Valore < LimiteInf Then

            'Double_Valore = LimiteInf

            '06/06/2018 Grilli: Algoritmo di Valerio:
            'Range da min a +Inf -> Num casuale tra min e min+20%
            'Range da min a max -> Num casuale tra min e min+(max-min)/4
            Dim minRand As Double = LimiteInf
            Dim maxRand As Double

            If IsNothing(LimiteSup) Then
                maxRand = LimiteInf + Math.Round(CDec(LimiteInf) * 20 / 100, nDecimali)
            Else
                maxRand = LimiteInf + Math.Round(CDec((LimiteSup - LimiteInf)) / 4, nDecimali)
            End If

            valCorretto = CDec(New Random().Next(CInt(Math.Round(minRand, nDecimali) * 10 * nDecimali), CInt(Math.Round(maxRand, nDecimali) * 10 * nDecimali))) / (10 * nDecimali)

        ElseIf Not IsNothing(LimiteSup) AndAlso Double_Valore > LimiteSup Then
            'Double_Valore = LimiteSup

            '06/06/2018 Grilli: Algoritmo di Valerio:
            'Range da -Inf a max -> Num casuale tra max-20% e max
            'Range da min a max -> Num casuale tra max-(max-min)/4 e max
            Dim minRand As Double
            Dim maxRand As Double = LimiteSup

            If IsNothing(LimiteInf) Then
                minRand = LimiteSup - Math.Round(CDec(LimiteSup) * 20 / 100, nDecimali)
            Else
                minRand = LimiteSup - Math.Round(CDec((LimiteSup - LimiteInf)) / 4, nDecimali)
            End If

            'Approssimo a due cifre decimali
            valCorretto = CDec(New Random().Next(CInt(Math.Round(minRand, nDecimali) * 10 * nDecimali), CInt(Math.Round(maxRand, nDecimali) * 10 * nDecimali))) / (10 * nDecimali)

        End If

        Return valCorretto

    End Function

End Class

Public Class LCQ_ParametriValori

    Public Sub New(PivaSuperUser As String, _
                    Documento_Cod As Integer, _
                    PrmXMod_Cod As Integer, _
                    DataOraRilevazione As DateTime, _
                    Utente As String, _
                    String_Valore As String, _
                    DateTime_Valore As DateTime?, _
                    Int_Valore As Integer?, _
                    Float_Valore As Double?, _
                    Provvisorio As Boolean?)

        _PivaSuperUser = PivaSuperUser
        _Documento_Cod = Documento_Cod
        _PrmXMod_Cod = PrmXMod_Cod
        _DataOraRilevazione = DataOraRilevazione
        _Utente = Utente
        _String_Valore = String_Valore
        _DateTime_Valore = DateTime_Valore
        _Int_Valore = Int_Valore
        _Float_Valore = Float_Valore
        _Provvisorio = Provvisorio

    End Sub

    Private _PivaSuperUser As String
    Private _Documento_Cod As Integer
    Private _PrmXMod_Cod As Integer
    Private _DataOraRilevazione As DateTime
    Private _Utente As String
    Private _String_Valore As String
    Private _DateTime_Valore As DateTime?
    Private _Int_Valore As Integer?
    Private _Float_Valore As Double?
    Private _Provvisorio As Boolean?

    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property Documento_Cod() As Integer
        Get
            Return _Documento_Cod
        End Get
        Set(ByVal value As Integer)
            _Documento_Cod = value
        End Set
    End Property


    Public Property PrmXMod_Cod() As Integer
        Get
            Return _PrmXMod_Cod
        End Get
        Set(ByVal value As Integer)
            _PrmXMod_Cod = value
        End Set
    End Property

    Public Property DataOraRilevazione() As DateTime
        Get
            Return _DataOraRilevazione
        End Get
        Set(ByVal value As DateTime)
            _DataOraRilevazione = value
        End Set
    End Property

    Public Property Utente() As String
        Get
            Return _Utente
        End Get
        Set(ByVal value As String)
            _Utente = value
        End Set
    End Property

    Public Property String_Valore() As String
        Get
            Return _String_Valore
        End Get
        Set(ByVal value As String)
            _String_Valore = value
        End Set
    End Property

    Public Property DateTime_Valore() As DateTime?
        Get
            Return _DateTime_Valore
        End Get
        Set(ByVal value As DateTime?)
            _DateTime_Valore = value
        End Set
    End Property

    Public Property Int_Valore() As Integer?
        Get
            Return _Int_Valore
        End Get
        Set(ByVal value As Integer?)
            _Int_Valore = value
        End Set
    End Property

    Public Property Float_Valore() As Double?
        Get
            Return _Float_Valore
        End Get
        Set(ByVal value As Double?)
            _Float_Valore = value
        End Set
    End Property

    Public Property Provvisorio() As Boolean?
        Get
            Return _Provvisorio
        End Get
        Set(ByVal value As Boolean?)
            _Provvisorio = value
        End Set
    End Property

End Class