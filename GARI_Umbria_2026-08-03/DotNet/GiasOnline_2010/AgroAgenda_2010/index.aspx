<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="index.aspx.vb" Inherits="AgroAgenda_2010.index" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>
<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <title>
        <% If CustomLoghi IsNot Nothing AndAlso Not String.IsNullOrEmpty(CustomLoghi.Title) Then %>
            <%=CustomLoghi.Title %>
        <% Else %>
            Agronica
        <% End If %>
    </title>
    <%-- For other devs: Do not remove below line. --%>
    <%="" %>
    <!-- css -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/Styles/font-awesome-4.7.0/css/font-awesome.min.css") %>" media="screen" />
    
    <!--bootstrap css-->
    <asp:PlaceHolder ID="bootstrapPlaceHeader" runat="server"></asp:PlaceHolder>
    
    <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/bootstrap_AGRONICA.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />

    <% If CustomLoghi IsNot Nothing AndAlso Not String.IsNullOrEmpty(CustomLoghi.Logo_Favicon_PATH) Then %>
        <link rel="icon" href="<%=PATH_GIASBASE %><%= CustomLoghi.Logo_Favicon_PATH %>?<% =Application("GiasVersioneCorrente")%>" type="image/svg+xml" />
    <% Else %>
        <link rel="icon" href="<%=PATH_GIASBASE %>agronica/AB_Immagini/Logo/favicon.ico?<% =Application("GiasVersioneCorrente")%>" type="image/svg+xml" />
    <% End If %>

    <asp:PlaceHolder ID="siteCssPlaceHolder" runat="server"></asp:PlaceHolder>

    <asp:PlaceHolder ID="kendoPlaceHeader" runat="server"></asp:PlaceHolder>

    <!-- Versione Grafica Pagina Login -->
    <% If Login_Versione = "2022" Then %>
        <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/styleXonneLogin.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />
    <% End If %>

    <style type="text/css">
        /* Sticky footer styles
      -------------------------------------------------- */

        html, body {
            height: 100%; /* The html and body elements cannot have any padding or margin. */
        }

        .modal-dialog {
            z-index: 2000;
        }

        /* Wrapper for page content to push down footer */
        #wrap_master {
            min-height: 100%;
            height: auto !important;
            height: 100%; /* Negative indent footer by it's height */
            margin: 0 auto -85px;
        }

        /* Set the fixed height of the footer here */
        #push, #footer {
            height: 85px;
        }

        #footer {
            background-color: #052747;
            color: #fff;
            font-size: 10px;
            height: auto !important;
            padding: 8px 0 !important;
            margin-top: 23px;
        }

            #footer h5 {
                font-size: 14px;
                margin-bottom: 3px;
            }

        /* Lastly, apply responsive CSS fixes as necessary */
        @media (max-width: 991px) {
            #footer {
                padding-left: 20px;
                padding-right: 20px;
                padding-bottom: 20px;
                height: auto;
            }

            .footer_logo {
                float: none;
                margin-bottom: 5px;
            }

            #footer .nopadding {
                text-align: center;
                margin-bottom: 15px;
            }
        }

        .footer_logo {
            margin-bottom: 5px;
            float: left;
            margin-right: 10px;
            background-color: #fff;
            padding: 8px 3px;
            border-radius: 5px;
        }

        .row-center {
            text-align: center;
        }

        .col-center {
            display: inline-block;
            float: none;
        }

        .input-group-addon-pwd {
            text-align: center !important;
            text-transform: uppercase;
        }

        .spidCss1 {
            border-color: #000000;
            width: 200px;
            height: 140px;
            margin: 4px
        }

        .btnAsLink {
            border: none;
            background: none;
            color: #428bca;
            padding: 0;
        }
        .btnAsLink:hover {
            text-decoration: underline;
        }
        .btnAsLink:focus {
            outline: none;
        }

        #boxMessaggiAgronica {
            background-color: #19b925;
            border: 2px solid white;
            border-radius: 10px;
            color: white;
            display: none;
            font-weight: bold;
            margin-top: 10px;
            padding: 10px 0 10px 12px;
            text-align: center;
        }

        #Txt_username, #button1, #Cmb_Server, .k-picker.k-dropdownlist {
            width: 100%;
        }

        .modal-header button.btn-close {
            /*Sovrascrivo default di bootstrap*/
            margin-left:0;
        }

    </style>
</head>
<body>
    
    <cc2:jquery ID="jquery" runat="server" />

    <script type="text/javascript">

        const GiasVersioneLogin = '<%=Login_Versione %>';

    </script>

    <div id="wrap_master">

        <cc2:AgroMasterPage ID="AgroMasterPage" runat="server" />

        <div id="DIV_Messaggi"></div>
        <form id="Form1" runat="server" class="form-horizontal">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <!-- jquery doc ready -->
            <script type="text/javascript">
                //DOCUMENT READY
                jQuery(function () {

                    kendo.culture("it-IT");

                    //VERIFICA MESSAGGI DA AGRONICA
                    var hasMessaggiDaMostrare = $("#messaggiDaAgronica").text().length !== 0;
                    if (hasMessaggiDaMostrare === true) {
                        if (GiasVersioneLogin === '2022') {
                            $("#boxMessaggiAgronica").css("display", "flex");
                        } else {
                            $("#boxMessaggiAgronica").show();
                        }
                    } else {
                        if (GiasVersioneLogin === '2022') {
                            $("#boxMessaggiAgronica").css("display", "none");
                        } else {
                            $("#boxMessaggiAgronica").hide();
                        }
                    }

                    var hasSessionExpired = $("#lblSessioneScaduta").text().length !== 0;
                    if (hasSessionExpired === true) {
                        if (GiasVersioneLogin === '2022') {
                            $("#expiredSession").css("display", "flex");
                        } else {
                            $("#expiredSession").show();
                        }
                    } else {
                        if (GiasVersioneLogin === '2022') {
                            $("#expiredSession").css("display", "none");
                        } else {
                            $("#expiredSession").hide();
                        }
                    }

                    $("select[name='Cmb_Server']").kendoDropDownList({filter: "contains"});

                    $("#Passw a").on('click', function (event) {
                        MostraNascondiPwd();
                    });


                    //if ($("#loginSPIDCodFiscContoMultiplo").val() != null)
                    //    mostraPopupLoginAccountPerSPID();

                    //$("[name='btnTroubleLogin']").on("click", function (event) {
                    //    var valDominio = encodeURIComponent($("[name='Cmb_Server']").val());
                    //    var descDominio = encodeURIComponent($("[name='Cmb_Server']").children("option:selected").text().trim());
                    //    //window.open("TroubleLogin.aspx?val_dominio=" + valDominio + "&desc_dominio=" + descDominio, "_self");
                    //    ajaxAgronica("TroubleLogin.aspx?val_dominio=" + valDominio + "&desc_dominio=" + descDominio, "", function (risp) {
                    //        var divTemp = document.createElement("div");
                    //        divTemp.innerHTML = risp;
                    //        $(divTemp).kendoWindow();
                    //    });
                    //});
                });

                function MostraNascondiPwd() {
                    event.preventDefault();
                    if ($('#Passw input').attr("type") == "text") {
                        $('#Passw input').attr('type', 'password');
                        $('#Passw i').addClass("fa-eye-slash");
                        $('#Passw i').removeClass("fa-eye");
                    } else if ($('#Passw input').attr("type") == "password") {
                        $('#Passw input').attr('type', 'text');
                        $('#Passw i').removeClass("fa-eye-slash");
                        $('#Passw i').addClass("fa-eye");
                    }
                }

            </script>
            <div class="container">
                <div class="row">
                    <!-- Box dei messaggi spostato qui -->
                    <div class="col-sm-12 col-md-12 col-sx-12 align-self-center">
                        <div id="boxMessaggiAgronica" style="display: none;">
                            <i id="iconEsito" class="fa fa-info-circle"></i><span runat="server" id="messaggiDaAgronica"></span>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-12 col-sx-12 align-self-center">
                        <asp:PlaceHolder ID="headerPlaceHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
                <div class="row row-center justify-content-center">
                    <!--<div class="col-sm-6 col-md-4 col-md-offset-4">-->

                    <%-- DIV CAMPI LOGIN --%>
                    <% if Show_Login_Fields = True %>
                    <div class="col-sm-5 col-md-5 col-center">
                        <div style="text-align: left">
                            <div class="row" id="expiredSession" style="display: none;">
                                <asp:Label ID="lblSessioneScaduta" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <!-- col-sm-6 col-md-6 ---- col-md-offset-6 -->
                                <!--  -->
                                <asp:Panel ID="pnlLogin" runat="server">
                                    <div class="form-inline account-wall">

                                        <div class="input-group" id="pan_superserver" visible="false" runat="server">
                                            <asp:Label class="input-group-addon control-label " ID="lbl_dominio" for="ddl_dominio" runat="server">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ambiente %>" runat="server"></asp:Localize>
                                            </asp:Label>

                                            <asp:Label ID="lbl_cmb_server" runat="server" style="font-size: 18px;"></asp:Label>
                                            <asp:DropDownList ID="Cmb_Server" runat="server" name="Cmb_Server" CssClass="form-control"
                                                aria-describedby="lbl_dominio" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                        <!--  -->
                                        <div class="input-group" id="username">
                                            <label class="input-group-addon control-label " id="lbl_username" for="Txt_username">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Username %>" runat="server"></asp:Localize>
                                            </label>

                                            <asp:TextBox class="form-control" ID="Txt_username" aria-describedby="lbl_username" runat="server" autocomplete="username"/>
                                        </div>
                                        <!--  -->
                                        <% If Login_Versione = "2022" %>
                                        <label class="input-group-addon control-label " id="lbl_Password" for="Txt_Password">
                                            <!-- Label portata fuori dall'input group per questioni di stile -->
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Password %>" runat="server"></asp:Localize>
                                        </label>
                                        <div class="input-group" id="Passw">
                                        <% Else %>
                                        <div class="input-group" id="Passw">
                                            <label class="input-group-addon control-label " id="lbl_Password" for="Txt_Password">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Password %>" runat="server"></asp:Localize>
                                            </label>
                                        <% End If %>
                                            <asp:TextBox class="form-control xo-login-password" ID="Txt_Password"
                                                aria-describedby="lbl_Passw" runat="server" TextMode="Password" autocomplete="current-password" />
                                            <div class="input-group-addon xo-login-eye" style="max-width: 30px !important; width: 30px; min-width: 30px;">
                                                <a href="">
                                                    <i class="fa fa-eye-slash" aria-hidden="true"></i>
                                                </a>
                                            </div>
                                        </div>

                                         <!-- Link per recupero credenziali e richiesta iscrizione spostati qui -->
                                        <div class="bottom-links">
                                            <div class="row">
                                                <asp:Button runat="server" ID="btnCambiaPassword" CssClass="btnAsLink" Text="<%$ Resources: AgronicaAgenda_2010, CambiaPassword %>" />
                                            </div>
                                            <div class="row">
                                                <%--<button type="button" name="btnTroubleLogin" class="btnAsLink">Recupera Credenziali</button>--%>
                                                <asp:Button runat="server" ID="btnTroubleLogin" CssClass="btnAsLink" Text="<%$ Resources: AgronicaAgenda_2010, RecuperaCredenziali %>" /> 
                                            </div>
                                        </div>

                                        <asp:Button ID="button1" OnClientClick=" storageClear(); " runat="server" CssClass="btn btn-lg btn-primary btn-block AgroBtnLogin" Text="<%$ Resources: AgronicaAgenda_2010, Accedi %>" />
                                            
                                    </div>
                                </asp:Panel>
                            </div> 
                            <div class="row">
                                <asp:Label ID="Label1" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <asp:Label ID="Label2" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <asp:Label ID="Label3" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <asp:Label ID="Label4" runat="server"></asp:Label>
                            </div>
                            <div class="row">
                                <asp:Label ID="lbl_Messaggio" runat="server"></asp:Label>
                            </div>
                            <!-- Link per recupero credenziali e richiesta iscrizione spostati piu su -->
                            <div class="row">
                                <asp:Button runat="server" ID="btnRichiestaIscrizione" CssClass="btnAsLink" Text="<%$ Resources: AgronicaAgenda_2010, CompilaRichiestaIscrizione %>" /> 
                            </div>
                        </div>
                    </div>
                    <% End if %>
                    <!-- DIV LOGIN SPID -->
                    <div id="divSPID" class="col-sm-5 col-md-5 col-center" runat="server" visible="false">
                        <asp:Image id ="divSPIDImage" CssClass="spidCss1" ImageUrl="" AlternateText="" runat ="server" />
                        <%--<img class="" style="border-color: #000000" src="AB_Immagini/Logo/REGIONEUMBRIA.jpg" width="200px" height="140px" alt="Entra con FedUmbria" />--%>
                        <br />
                        <asp:Panel ID="Panel1" runat="server">
                            <div class="account-wall">
                                <asp:ImageButton Visible="false" ID="btnSPIDImage" runat="server" CssClass="" ImageUrl="AB_Immagini/Logo/REGIONEUMBRIA.jpg" />
                                <asp:Button ID="btnSPID" runat="server" CssClass="btn btn-lg btn-primary" Text="Entra con Spid/FedUmbria" />
                                <asp:Button ID="btnSPID_Logout" runat="server" Visible="false" CssClass="btn btn-lg " Text="Logout" />
                                <!--
                                    <div id="btnSPIDUmbria" class="btn btn-info" onclick="btnSpidClick()">
                                    <i class="fa fa-list" aria-hidden="true"></i>
                                    <span id="lbl_CercaGiacenze">Entra con Spid/FedUmbria</span>
                                </div>-->
                            </div>
                        </asp:Panel>
                        <br />
                    </div>
                    <!--</div>-->
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <asp:Panel runat="server" ID="pnlBassoPersonalizzato" Style="margin-top: 10px; text-align: center;"></asp:Panel>
                    </div>
                </div>

            </div>

            <!-- disclaimer per utilizzo cookie tecnici -->
            <div style="margin-top: 20px; padding: 20px; text-align: center;">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DisclaimerCookieTecnici %>" runat="server"></asp:Localize>
            </div>

            <!--Dialog errore-->
            <div id="dialog_errore" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close btn-close" data-dismiss="modal" data-bs-dismiss="modal" id="btnErrore">
                                <span class="sr-only"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Chiudi %>" runat="server"></asp:Localize></span></button>
                            <h4 class="modal-title">
                                <i class="fa fa-exclamation-triangle fa-3x"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SiEVerificatoUnErrore %>" runat="server"></asp:Localize>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <p id="messaggioErrore">
                            </p>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>

            <!--Dialog ok-->
            <div id="dialog_ok" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close btn-close" data-dismiss="modal" data-bs-dismiss="modal">
                                <span class="sr-only"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Chiudi %>" runat="server"></asp:Localize></span></button>
                        </div>
                        <div class="modal-body">
                            <p id="messaggioOk">
                            </p>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>

            <!-- Dialog seleziona utente (SPID login) -->
            <div id="dialog_selezionaUtente" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                             <button type="button" class="close btn-close" data-dismiss="modal" data-bs-dismiss="modal">
                             <span class="sr-only"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Chiudi %>" runat="server"></asp:Localize></span></button>
                            <h4 class="modal-title">
                                <i lass="fa fa-user fa-3x" aria-hidden="true"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SelezionaAccount %>" runat="server"></asp:Localize>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="input-group" id="div_ddl_account" visible="false" runat="server">
                                <asp:Label class="input-group-addon control-label " ID="label_ddl_account" runat="server">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Account %>" runat="server"></asp:Localize>
                                </asp:Label>

                                <asp:DropDownList ID="ddl_account" runat="server" onchange="return false;" CssClass="form-control"
                                    data-live-search="true" aria-describedby="label_ddl_account" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <asp:Button ID="btn_accountSelezionato" OnClientClick=" storageClear(); " runat="server" CssClass="btn btn-lg btn-primary btn-block AgroBtnLogin" Text="<%$ Resources: AgronicaAgenda_2010, Accedere %>" />

                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>

            <!--Dialog si/no-->
            <div id="dialog_sino" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close btn-close" data-dismiss="modal" data-bs-dismiss="modal">
                                <span class="sr-only"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Chiudi %>" runat="server"></asp:Localize></span></button>
                            <p>
                            </p>
                        </div>
                        <div class="modal-body">
                            <div id='variabileSiNo' style='display: none;'>
                            </div>
                            <p id="messaggioSiNo">
                            </p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, No %>" runat="server"></asp:Localize>
                            </button>
                            <button type="button" class="btn btn-primary" onclick="DoPostBack_ControlliSiNo($('#variabileSiNo').html()); $('#dialog_sino').modal('hide');">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Si %>" runat="server"></asp:Localize>
                            </button>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>

            <!--Dialog sessione scaduta-->
            <div id="dialogSessioneScaduta" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close btn-close" data-dismiss="modal" data-bs-dismiss="modal">
                                <span class="sr-only">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Chiudi %>" runat="server"></asp:Localize>
                                </span></button>
                            <h4 class="modal-title">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SessioneScadutaRipetereLogin %>" runat="server"></asp:Localize>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <p>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SeNonAppareFinestraLoginCliccaLink %>" runat="server"></asp:Localize><a href="../GST_Autenticazione/Autenticazione.aspx"></a>
                            </p>
                            <div id="lblAutoLogin">
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-default" data-dismiss="modal">
                                    Close</button>
                            </div>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div style="display: none">
                <div id="loginSPIDContoMultiplo">
                    <div class="row">
                        <h2>Login SPID Conto Multiplo</h2>
                    </div>
                </div>
            </div>
            <input type="hidden" id="loginSPIDCodFiscContoMultiplo" runat="server" />

            <asp:HiddenField runat="server" ID="hdf_ShowModal" />

            <cc2:bootstrap ID="bootstrap" runat="server" />
            <cc2:AgronicaBase ID="AgronicaBase" runat="server" />
            <cc2:bootstrap_WaitFrame ID="bW" runat="server" />
            <cc2:jAgroHelper ID="jAgroHelper1" runat="server" />
            <cc2:jquery_cookie ID="jCookie" runat="server" />
            <cc2:Agrokendo id="agroKendo" runat="server" />

            <!-- <script src="Scripts/jquery.linq.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script> -->
            <script src="Scripts/jquery.tmpl.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
            <script src="Scripts/jquery.validate.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
            <div style="clear: both;"></div>

            <script type="text/javascript">

                function posizionaMessggi(jQuerySelector) {
                    try {

                        $(jQuerySelector).css("z-index", "100000");
                        $(jQuerySelector).css("position", "absolute");
                        $(jQuerySelector).css("top", window.pageYOffset.toString() + "px");
                        $(jQuerySelector).css("width", $(window).width().toString() + "px");

                    } catch (e) {
                        console.log("posizionaMessggi non riuscita...");
                    }

                }


                //MESSAGGI SU SCHERMO
                function MessaggioErrore_Bootstrap(str, id_div) {
                    var stringa_html = '<div class="alert alert-danger" role="alert">' + str +
                        '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
                    $('#' + id_div).html(stringa_html);
                    posizionaMessggi('#' + id_div);
                }
                function MessaggioErroreTooltip_Bootstrap(str, id_div, tooltip) {
                    MessaggioErrore_Bootstrap(str, id_div);
                    $('#' + id_div).attr('title', tooltip);
                    posizionaMessggi('#' + id_div);
                }

                function MessaggioTuttoOK_Bootstrap(str, id_div) {
                    var stringa_html = '<div class="alert alert-success" role="alert">' + str +
                        '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
                    $('#' + id_div).html(stringa_html);
                    posizionaMessggi('#' + id_div);
                    setTimeout(function () { $('.chiudi_alert').click(); }, 3000);
                }

                function MessaggioAttenzione_Bootstrap(str, id_div) {
                    var stringa_html = '<div class="alert alert-warning" role="alert">' + str +
                        '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
                    $('#' + id_div).html(stringa_html);
                    posizionaMessggi('#' + id_div);
                    setTimeout(function () { $('.chiudi_alert').click(); }, 3000);
                }

                function MessaggioErrore(str) {
                    $('#dialog_errore').modal('show');
                    $('#messaggioErrore').html(str);
                }

                function ScritturaOK(str) {
                    $('#dialog_ok').modal('show');
                    $('#messaggioOk').html(str);
                }
               


                function ConfermaControlliSiNo(messaggio, str) {
                    $('#messaggioSiNo').html(messaggio);
                    $('#variabileSiNo').html(str);

                    $('#dialog_sino').modal('show');
                }

                function btnSpidClick() {
                    $.ajax({
                        type: 'POST',
                        url: './index.aspx/BtnSpidClick',
                        data: "{}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            var objRisposta = JSON.parse(r.d.RispostaStringa);
                            var url = objRisposta.url;
                            var data = objRisposta.data;
                            alert(url);
                            $.ajax({
                                type: 'POST',
                                url: url,
                                data: "{ data:'" + data + "', RelayState:'' }",
                                contentType: 'application/json; charset=utf-8',
                                cache: false,
                                dataType: 'json', async: true,
                                success: function (r) {
                                    var objRisposta = JSON.parse(r.d.RispostaStringa);
                                    var url = objRisposta.url;
                                    var data = objRisposta.data;
                                    alert(url);
                                }
                            });
                        }
                    });
                }

            </script>

            <asp:UpdatePanel runat="server" ID="updateScript"></asp:UpdatePanel>

        </form>
    </div>
    <!-- /#wrap -->
    <!-- Bootstrap Footer fixed bottom -->

    <asp:PlaceHolder ID="footerPlaceHoler" runat="server"></asp:PlaceHolder>


</body>
</html>
