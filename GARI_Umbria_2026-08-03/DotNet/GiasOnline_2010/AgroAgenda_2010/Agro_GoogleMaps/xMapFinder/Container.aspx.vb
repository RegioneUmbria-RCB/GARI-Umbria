Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste

'Option Strict On

Partial Class Map_Container
    Inherits System.Web.UI.Page


    Private QS_Id As String = ""
    Private QS_Id2 As String = ""
    Private QS_Username As String = ""
    Private QS_Password As String = ""
    Private QS_Chiudi As Boolean

    'Chiave di codifica e decodifica
    Private AgroKey_EncoderDecoder As String = "cobaltoleccioplutone"

    Private Connessione_Aternativa As String
    Private Fuso As String
    Private deltaE As Double
    Private deltaN As Double
    Private ipAddressHost As String
    Dim parX As String
    Dim parY As String
    Dim tipoCoord As Integer ' 1 = UTM , 2 = Lat , Long

    Dim b As String

    Public srv_gm As String
    Public srv_lat As String
    Public srv_lng As String
    Public srv_adr As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        If Not Request.QueryString("p") Is Nothing Then
            QS_Id = Stringa_Decodifica_LANCompatibile(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)
        End If

        If Not Request.QueryString("s") Is Nothing Then
            QS_Id2 = Stringa_Decodifica_LANCompatibile(Request.QueryString("s").ToString, AgroKey_EncoderDecoder)
        End If

        If Not Request.QueryString("f") Is Nothing Then
            Fuso = Stringa_Decodifica_LANCompatibile(Request.QueryString("f").ToString, AgroKey_EncoderDecoder)
        End If

        If Not Request.QueryString("a") Is Nothing Then
            srv_adr = Stringa_Decodifica_LANCompatibile(Request.QueryString("a").ToString, AgroKey_EncoderDecoder)
        End If

        '----------------------------------------------------------------------
        'Estraggo username e password
        If Not Request.QueryString("n") Is Nothing Then
            QS_Username = Stringa_Decodifica_LANCompatibile(Request.QueryString("n").ToString, AgroKey_EncoderDecoder)
        End If

        If Not Request.QueryString("w") Is Nothing Then
            QS_Password = Stringa_Decodifica_LANCompatibile(Request.QueryString("w").ToString, AgroKey_EncoderDecoder)
        End If
        '-----------------------------------------------------------------------

        If Not Request.QueryString("l") Is Nothing Then
            QS_Chiudi = IIf(Stringa_Decodifica_LANCompatibile(Request.QueryString("l").ToString, AgroKey_EncoderDecoder).ToLower = "1", True, False)
        Else
            QS_Chiudi = True
        End If

        If Not Request.QueryString("x") Is Nothing Then
            parX = Stringa_Decodifica_LANCompatibile(Request.QueryString("x").ToString, AgroKey_EncoderDecoder)
        Else
            parX = ""
        End If

        If Not Request.QueryString("y") Is Nothing Then
            parY = Stringa_Decodifica_LANCompatibile(Request.QueryString("y").ToString, AgroKey_EncoderDecoder)
        Else
            parY = ""
        End If

        If Not Request.QueryString("t") Is Nothing Then
            tipoCoord = Stringa_Decodifica_LANCompatibile(Request.QueryString("t").ToString, AgroKey_EncoderDecoder)
        Else
            tipoCoord = 1
        End If

        '*** GABRIELE *** 
        ' Ho DOVUTO (mio malgrado) continuare a gestire la finestra in questo modo
        ' ed aggiungere la possibilità di eseguire un click alla chiusura della finestra
        ' per poter reagire alla modifica delle coordinate
        If Not Request.QueryString("b") Is Nothing Then
            b = Stringa_Decodifica_LANCompatibile(Request.QueryString("b").ToString, AgroKey_EncoderDecoder)
        Else
            b = ""
        End If

        'Recupero da webconfig la connessione alternativa da usare
        Dim AgroWebC As New AgroWebConfig()
        Connessione_Aternativa = AgroWebC.ConnessioneMaps_Server 'ConfigurationSettings.AppSettings("Connessione_Server").ToString

        srv_gm = "https://maps.googleapis.com/maps/api/js?key="

        If AgroWebC.GoogleMaps <> "" Then
            srv_gm = AgroWebC.GoogleMaps
            srv_gm = srv_gm.Replace("&sensor=false&libraries=drawing,geometry", "")
        End If

        If Debugger.IsAttached Then
            srv_gm = "https://maps.googleapis.com/maps/api/js?v=3.exp&client=gme-addictive&sensor=false&libraries=drawing,geometry"
        End If

        '  Galassi, 23/06/2016 13:14:13: Inserito il replace perchè altrimenti non prendeva la virgola
        '  Recupero da webconfig l'offset da sommare alle coordinate piane UTM ED50
        deltaE = CDbl(Replace(AgroWebC.deltaE.ToString, ".", ","))  'ConfigurationSettings.AppSettings("deltaE").ToString)
        deltaN = CDbl(Replace(AgroWebC.deltaN.ToString, ".", ","))  'ConfigurationSettings.AppSettings("deltaN").ToString

        'Salvo l'indirizzo IP dell'host che ha chiamato la pagina
        ipAddressHost = Request.UserHostAddress

        Me.form1.DataBind()

        If Not Page.IsPostBack Then
            'output.Write("Page has just been loaded")
        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If




    End Sub

    Protected Sub btnConfermaNet_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfermaNet.Click


        Dim Xno As Double
        Dim Yno As Double
        Dim Xse As Double
        Dim Yse As Double

        Dim XoLAT As Double
        Dim YoLNG As Double

        Dim latNO As Double
        Dim lonNO As Double
        Dim latSE As Double
        Dim lonSE As Double

        'Dim objDati As AccessoDati
        Dim objCoordinate As Coordinate
        Dim vCoord() As String
        Dim strClose As String

        Dim strException As String = ""
        Dim decimalSeparator As String = CulturaHelper.SeparatoreDecimaleVB()
        If QS_Id <> "" Then

            'Scrivi tutto

            objCoordinate = New Coordinate(Fuso, deltaE, deltaN)
            'Dim objDati = New AccessoDati()
            'objDati.Connessione = Connessione_Aternativa

            'Se l'utente non è autenticato non gli faccio salvare un piffero
            'If objDati.AutenticaUtente(QS_Username, QS_Password) Then

            If IsNumeric(TxtRitorno.Value <> "") Then


                vCoord = Split(TxtRitorno.Value, "~")

                If (vCoord.Count > 3) Then
                    If (Not IsNothing(vCoord(3))) AndAlso IsNumeric(vCoord(3).Replace("."c, decimalSeparator)) Then
                        latNO = CDbl(vCoord(3).Replace("."c, decimalSeparator))
                    Else
                        strException &= "Err su coord LatNO"
                    End If

                    If (Not IsNothing(vCoord(4))) AndAlso vCoord(4).Replace("."c, decimalSeparator) Then
                        lonNO = CDbl(vCoord(4).Replace("."c, decimalSeparator))
                    Else
                        strException &= "Err su coord LonNO"
                    End If

                    If (Not IsNothing(vCoord(5))) AndAlso vCoord(5).Replace("."c, decimalSeparator) Then
                        latSE = CDbl(vCoord(5).Replace("."c, decimalSeparator))
                    Else
                        strException &= "Err su coord LatSE"
                    End If

                    If (Not IsNothing(vCoord(6))) AndAlso vCoord(6).Replace("."c, decimalSeparator) Then
                        lonSE = CDbl(vCoord(6).Replace("."c, decimalSeparator))
                    Else
                        strException &= "Err su coord LonSE"
                    End If
                Else

                    strException = "Inserire un indirizzo corretto! Se si è su Internet Explorer" &
                        "verificare di aver tolto la modalità compatibilità per la corretta visualizzazione" &
                        "di questa pagina"



                End If

                If Not String.IsNullOrEmpty(strException) Then

                    strException = "<script language='javascript'>" &
                        "window.alert('" & strException & "');  </script>"

                    ScriptManager.RegisterStartupScript(updt1, updt1.GetType(),
                                       String.Format("jQuery_{0}", updt1.ClientID), strException, False)
                    Exit Sub
                End If


                Select Case tipoCoord
                        Case 1 ' Coordinate UTM

                        Try
                            objCoordinate.FromGradiToUTM(latNO, lonNO, Xno, Yno)
                            objCoordinate.FromGradiToUTM(latSE, lonSE, Xse, Yse)
                            XoLAT = Int((CDbl(Xno) + CDbl(Xse)) / 2)
                            YoLNG = Int((CDbl(Yno) + CDbl(Yse)) / 2)
                        Catch ex As Exception
                            strException = " Errore su conversione coordinate UTM "
                        End Try


                    Case 2 ' Coordinate LAT LONGI
                            XoLAT = ((CDbl(latNO) + CDbl(latSE)) / 2)
                            YoLNG = ((CDbl(lonNO) + CDbl(lonSE)) / 2)
                    End Select

                    'objDati.ScriviCoordinate(QS_Id, QS_Id2, Xno, Yno, Xse, Yse, ipAddressHost, Date.Today)

                End If

                'End If


            End If

        '--------------------------

        'Dim strClose As String = "<script language=" + Chr(34) + "vbscript" + Chr(34) + "> " & vbNewLine & _
        '"window.returnValue = document.all(" + Chr(34) + "TxtRitorno" + Chr(34) + ").value " & vbNewLine & _
        '"window.close()" & vbNewLine & _
        '"</script>"

        'ctl00_ctl00_ContentAgendaContenuti_ContentOperazioniContenuti_GridViewCentriMeteo_ctl02_Txt_CoordX().value = Xtot
        'ctl00_ctl00_ContentAgendaContenuti_ContentOperazioniContenuti_GridViewCentriMeteo_ctl02_Txt_CoordY().value = Ytot

        'lanciaWS();
        '  Galassi, 28/06/2016 10:53:56: Questa tecnica qui non funziona con cross-platform per una qwuestione di permessi..

        If Not String.IsNullOrEmpty(strException) Then

            strException = "<script language='javascript'>" &
                        "window.alert('" & strException & "');  </script>"

            ScriptManager.RegisterStartupScript(updt1, updt1.GetType(),
                                       String.Format("jQuery_{0}", updt1.ClientID), strException, False)
            Exit Sub
        End If

        strClose = "<script language='javascript'> " & vbNewLine &
            "try{ " & vbNewLine &
            "window.opener.document.getElementById('" & parX & "').value = '" & XoLAT & "';" & vbNewLine &
            "window.opener.document.getElementById('" & parY & "').value = '" & YoLNG & "';" & vbNewLine

        If Not String.IsNullOrEmpty(b) Then
            strClose &= "window.opener.document.getElementById('" & b & "').click();" & vbNewLine
        End If

        strClose &= "} catch(e){window.alert('Errore durante il salvataggio delle coordinate! Rif. RilievoPiogge. Se il problema persiste contattare l\'assistenza');}" &
            vbNewLine &
            "window.returnValue = document.all('TxtRitorno').value; "

        If QS_Chiudi = True Then
            strClose += "window.close()" & vbNewLine
        Else
            strClose += "window.alert('Coordinate GIAS salvate! Chiudere la finestra per continuare')"
        End If

        strClose += "</script>"

        ScriptManager.RegisterStartupScript(updt1, updt1.GetType(),
                                       String.Format("jQuery_{0}", updt1.ClientID), strClose, False)

        'Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub

    '##############################################################
    Function Agronica_Url_Decode(
                            ByVal StrInput As String,
                            Optional ByVal Separatore As String = "G") _
                            As String

        Dim i As Integer
        Dim Carattere As String
        Dim Cod_Ascii_16 As Integer
        Dim Cod_Hex As String
        Dim StrOutput As String
        Dim Vettore() As String

        'Inizializzo
        StrOutput = ""

        'Recupero gli elementi
        Vettore = Split(StrInput, Separatore)

        For i = LBound(Vettore) To UBound(Vettore)

            Cod_Hex = Vettore(i)

            Cod_Ascii_16 = Val("&H" & Cod_Hex)

            Carattere = ChrW(Cod_Ascii_16)

            StrOutput = StrOutput & Carattere

        Next i

        Return StrOutput

    End Function

    '#####################################################################
    Public Function Stringa_Decodifica_LANCompatibile( _
                                       ByVal Testo As String, _
                                       ByVal Chiave As String) _
                                       As String

        Dim Cript As Boolean = False

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        If Testo <> "" Then

            Testo = Agronica_Url_Decode(Testo)



            KeyPos = 1

            For i = 1 To Len(Testo)
                A1 = Asc(Mid(Testo, i, 1))
                A2 = Asc(Mid(Chiave, KeyPos, 1))
                If Cript Then
                    strEncrypted = strEncrypted & Chr(A2 + A1)
                Else
                    strEncrypted = strEncrypted & Chr(A1 - A2)
                End If
                KeyPos = KeyPos + 1
                If KeyPos > Len(Chiave) Then KeyPos = 1
            Next


            'strEncrypted = Replace(strEncrypted, "@", "\")

        End If

        Return strEncrypted


    End Function

    '#####################################################################
    Public Function Stringa_Codifica_Nuova(ByVal Testo As String, _
                                     ByVal Chiave As String) _
                                     As String

        Testo = Replace(Testo, "\", "@")

        Dim Cript As Boolean
        Dim strEncrypted As String = ""
        Dim i As Long
        'Dim A1 As Long
        'Dim A2 As Long
        Dim KeyPos As Byte

        Cript = True

        KeyPos = 1

        '        For i = 1 To Len(Testo)
        '            A1 = Asc(Mid(Testo, i, 1))
        '            A2 = Asc(Mid(Chiave, KeyPos, 1))
        '            If Cript Then
        '                strEncrypted = strEncrypted & Chr(A2 + A1)
        '            Else
        '                strEncrypted = strEncrypted & Chr(A1 - A2)
        '            End If
        '            KeyPos = KeyPos + 1
        '            If KeyPos > Len(Chiave) Then KeyPos = 1
        '        Next
        '
        '        Stringa_Codifica = URLEncode(strEncrypted)

        For i = 1 To Len(Testo)

            strEncrypted = strEncrypted & Right("00" & Hex(Asc(Mid(Testo, i, 1))), 2)

        Next

        Stringa_Codifica_Nuova = strEncrypted


    End Function



    '#####################################################################
    Public Function Stringa_Decodifica_Nuova(ByVal Testo As String, _
                                       ByVal Chiave As String, _
                                       ByRef objServer As Object) _
                                       As String


        'Testo = Simple_UrlDecode(Testo)

        'Dim Cript As Boolean = False

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer


        If Testo <> "" Then
            Testo = Replace(Testo, "@", "\")

            Dim strProva As String

            For i = 1 To Len(Testo) Step 2

                strProva = Mid(Testo, i, 2)

                A1 = Val("&H" & strProva)

                strProva = Chr(A1)
                strEncrypted += strProva


            Next

            strEncrypted = Replace(strEncrypted, "@", "\")

        End If

        Return strEncrypted


    End Function
End Class
