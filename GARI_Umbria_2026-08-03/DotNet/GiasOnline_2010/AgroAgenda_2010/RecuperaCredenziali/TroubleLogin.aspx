<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TroubleLogin.aspx.vb" Inherits="AgroAgenda_2010.TroubleLogin" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RecuperaCredenziali %>" runat="server"></asp:Localize></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />

    <link rel="stylesheet" href="<%= ResolveUrl("~/Styles/font-awesome-4.7.0/css/font-awesome.min.css?" & Application("GiasVersioneCorrente").ToString) %>" media="screen" />
    
    <!--bootstrap css-->
    <asp:PlaceHolder ID="bootstrapPlaceHeader" runat="server"></asp:PlaceHolder>
    
    <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/bootstrap_AGRONICA.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />

    <asp:PlaceHolder ID="siteCssPlaceHolder" runat="server"></asp:PlaceHolder>
    <cc2:jquery ID="jquery" runat="server" />

    <link rel="stylesheet" href="<%= ResolveUrl("RecuperaCredenziali.css?" & Application("GiasVersioneCorrente").ToString) %>" media="screen" />

    <!-- Versione Grafica Pagina Login -->
    <% If Login_Versione = "2022" Then %>
        <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/styleXonneLogin.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />
    <% End If %>

    <style type="text/css">
        
        #boxFormTroubleLogin {
            margin-top: 25px;
        }

        #boxReimpostaPassword > div, #boxRecuperaUsername > div {
            margin-bottom: 10px;
            margin-top: 10px;
        }

    </style>
</head>
<body>

    <script type="text/javascript">
        const GiasVersioneLogin = '<%=Login_Versione %>';
    </script>

    <div id="wrap_master">
        <cc2:AgroMasterPage ID="AgroMasterPage" runat="server" />

        <form id="Form1" runat="server">
            <div class="container">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-sx-12 align-self-center">
                        <asp:PlaceHolder ID="headerPlaceHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
                <div class="text-center">
                    <p>
                        <span id="descDominioScelto" runat="server"></span>
                    </p>
                    <button type="button" name="btnLinkReimpostaPassword" class="btnAsLink withBorderRight">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ReimpostaPassword %>" runat="server"></asp:Localize>
                    </button>
                    <button type="button" name="btnLinkRecuperaUsername" class="btnAsLink">
                        <asp:Localize Text="<%$ Resources: RecuperaUsername %>" runat="server"></asp:Localize>
                    </button>
                    <%--<button type="button" name="btnLinkReimpostaEmail" class="btnAsLink">Recupera Email associata</button>--%>
                </div>
                <div class="boxEsitoContainer">
                    <div id="boxEsito" class="">
                        <i id="iconEsito" class="fa fa-check-circle"></i><span id="mexEsito"></span>
                    </div>
                </div>
                <div id="boxFormTroubleLogin">
                    <div id="boxReimpostaPassword">
                        <div class="row justify-content-center">
                            <div class="col-xs-6 col-6 col-xs-offset-3">
                                <div >
                                    <label class="input-group-addon control-label" id="lblUsernameRP" for="txtUsernameRP">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Username %>" runat="server"></asp:Localize>
                                    </label>
                                    <% If Login_Versione = "2022" %>
                                    <div class="troubleLoginDataRow">
                                    <% End If %>
                                    <input type="text" class="form-control" id="txtUsernameRP" name="txtUsernameRP" aria-describedby="lblUsernameRP" />
                                    <span class="input-group-btn">
                                        <button type="button" name="wsbtnReimpostaPasswordDaUtente" class="btn btn-default btn-primary">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedi %>" runat="server"></asp:Localize>
                                        </button>
                                    </span>
                                    <% If Login_Versione = "2022" %>
                                    </div>
                                    <% End If %>
                                </div>
                            </div>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-xs-6 col-6 col-xs-offset-3">
                                <div >
                                    <label class="input-group-addon control-label" id="lblEmailRP" for="txtEmailRP">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Email %>" runat="server"></asp:Localize>
                                    </label>
                                    <% If Login_Versione = "2022" %>
                                    <div class="troubleLoginDataRow">
                                    <% End If %>
                                    <input type="email" class="form-control" id="txtEmailRP" name="txtEmailRP" aria-describedby="lblUsernameRP" />
                                    <span class="input-group-btn">
                                        <button type="button" name="wsbtnReimpostaPasswordDaEmail" class="btn btn-default btn-primary">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedi %>" runat="server"></asp:Localize>
                                        </button>
                                    </span>
                                    <% If Login_Versione = "2022" %>
                                    </div>
                                    <% End If %>
                                </div>
                            </div>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-xs-6 col-6 col-xs-offset-3">
                                <span>
                                    <i class="fa fa-info-circle"></i>
                                    <asp:Localize Text="<%$ Resources: InserireUsernameOMail %>" runat="server"></asp:Localize>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div id="boxRecuperaUsername">
                        <div class="row justify-content-center">
                            <div class="col-xs-6 col-6 col-xs-offset-3">
                                <div>
                                    <label class="input-group-addon control-label" id="lblEmailRU" for="txtEmailRU">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Email %>" runat="server"></asp:Localize>
                                    </label>
                                    <% If Login_Versione = "2022" %>
                                    <div class="troubleLoginDataRow">
                                    <% End If %>
                                    <input type="email" class="form-control" id="txtEmailRU" name="txtEmailRU" aria-describedby="lblEmailRU"/>
                                    <span class="input-group-btn">
                                        <button type="button" name="wsbtnRecuperaUsername" class="btn btn-default btn-primary">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedi %>" runat="server"></asp:Localize>
                                        </button>
                                    </span>
                                    <% If Login_Versione = "2022" %>
                                    </div>
                                    <% End If %>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--<div id="boxRecuperaEmail">
                        <div class="col-xs-6 col-xs-offset-3">
                            <div class="input-group">
                                <label class="input-group-addon control-label" id="lblUsernameRE" for="txtUsernameRE">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Username %>" runat="server"></asp:Localize>
                                </label>
                                <input type="text" class="form-control" id="txtUsernameRE" name="txtUsernameRE" aria-describedby="lblUsernameRE" />
                                <span class="input-group-btn">
                                    <asp:Button runat="server" ID="btnRecuperaEmail" class="btn btn-default btn-primary" Text="Procedi" />
                                    <button type="button" name="btnRecuperaEmail" class="btn btn-default btn-primary">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedi %>" runat="server"></asp:Localize>
                                    </button>
                                </span>
                            </div>
                        </div>
                    </div>--%>
                </div>
            </div>    

            <!-- Hidden field -->
            <input type="hidden" runat="server" id="hfValSuperServer" name="hfValSuperServer" />
            <input type="hidden" runat="server" id="hfDescSuperServer" name="hfDescSuperServer" />

            <cc2:bootstrap ID="bootstrap" runat="server" />
            <cc2:AgronicaBase ID="AgronicaBase" runat="server" />
            <cc2:bootstrap_WaitFrame ID="bW" runat="server" />
            <cc2:bootstrap_select ID="bS" runat="server" />
            <cc2:jAgroHelper ID="jAgroHelper1" runat="server" />
            <cc2:jquery_cookie ID="jCookie" runat="server" />
    

            <script src="<%= ResolveUrl("~/Scripts/jquery.linq.min.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/Scripts/jquery.validate.min.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>

            <script>
                $(document).ready(function () {
                    $("#boxFormTroubleLogin").children("div").hide();
                    $("#boxEsito").hide();

                    $("[name='btnLinkReimpostaPassword']").on("click", function () {
                        $("#boxFormTroubleLogin").children("div").hide();
                        $("#boxReimpostaPassword").show();
                    });
                    $("[name='btnLinkRecuperaUsername']").on("click", function () {
                        $("#boxFormTroubleLogin").children("div").hide();
                        $("#boxRecuperaUsername").show();
                    });
                    //$("[name='btnLinkReimpostaEmail']").on("click", function () {
                    //    $("#boxFormTroubleLogin").children("div").hide();
                    //    $("#boxRecuperaEmail").show();
                    //});

                    $("[name='wsbtnReimpostaPasswordDaUtente']").on("click", function () {
                        $("#boxEsito").hide();
                        var strUsername = $("[name='txtUsernameRP']").val();
                        var strValSuperServer = $("[name='hfValSuperServer']").val();
                        var strDescSuperServer = $("[name='hfDescSuperServer']").val();
                        wsReimpostaPasswordDaUtente(strUsername, strValSuperServer, strDescSuperServer, successRecuperaCredenziali, errorRecuperaCredenziali);
                        
                    });

                    $("[name='wsbtnReimpostaPasswordDaEmail']").on("click", function () {
                        $("#boxEsito").hide();
                        var strEmail = $("[name='txtEmailRP']").val();
                        var strValSuperServer = $("[name='hfValSuperServer']").val();
                        var strDescSuperServer = $("[name='hfDescSuperServer']").val();
                        wsReimpostaPasswordDaEmail(strEmail, strValSuperServer, strDescSuperServer, successRecuperaCredenziali, errorRecuperaCredenziali)
                    });

                    $("[name='wsbtnRecuperaUsername']").on("click", function () {
                        $("#boxEsito").hide();
                        var strEmail = $("[name='txtEmailRU']").val();
                        var strValSuperServer = $("[name='hfValSuperServer']").val();
                        wsRecuperaUsername(strEmail, strValSuperServer, successRecuperaCredenziali, errorRecuperaCredenziali)
                    });

                });

                function wsReimpostaPasswordDaUtente(strUsername, strValSuperServer, strDescSuperServer, callbackSuccess, callbackError) {
                    var params = {
                        username: strUsername,
                        valSuperServer: strValSuperServer,
                        descSuperServer: strDescSuperServer
                    };
                    ajaxAgronicaSync(
                        "./TroubleLogin.aspx/wsReimpostaPasswordDaUtente",
                        JSON.stringify(params),
                        false,
                        callbackSuccess,
                        callbackError
                    );
                }

                function wsReimpostaPasswordDaEmail(strEmail, strValSuperServer, strDescSuperServer, callbackSuccess, callbackError) {
                    var params = {
                        email: strEmail,
                        valSuperServer: strValSuperServer,
                        descSuperServer: strDescSuperServer
                    };
                    ajaxAgronicaSync(
                        "./TroubleLogin.aspx/wsReimpostaPasswordDaEmail",
                        JSON.stringify(params),
                        false,
                        callbackSuccess,
                        callbackError
                    );
                }

                function wsRecuperaUsername(strEmail, strValSuperServer, callbackSuccess, callbackError) {
                    var params = {
                        email: strEmail,
                        valSuperServer: strValSuperServer
                    };
                    ajaxAgronicaSync(
                        "./TroubleLogin.aspx/wsRecuperaUsername",
                        JSON.stringify(params),
                        false,
                        callbackSuccess,
                        callbackError
                    );
                }


                function successRecuperaCredenziali(rispServer) {
                    risp = rispServer.RispostaStringa;
                    $("#mexEsito").text(risp);
                    $("#iconEsito").removeClass("fa-times-circle");
                    $("#iconEsito").addClass("fa-check-circle");
                    var boxEsito = $("#boxEsito");
                    boxEsito.removeClass("esitoErrore");
                    boxEsito.addClass("esitoSuccesso");
                    boxEsito.show();
                }

                function errorRecuperaCredenziali(rispServer) {
                    risp = rispServer.Errore;
                    $("#mexEsito").text(risp);
                    $("#iconEsito").removeClass("fa-check-circle");
                    $("#iconEsito").addClass("fa-times-circle");
                    var boxEsito = $("#boxEsito");
                    boxEsito.removeClass("esitoSuccesso");
                    boxEsito.addClass("esitoErrore");
                    boxEsito.show();
                }
            </script>
        </form>
    </div>
    <asp:PlaceHolder ID="footerPlaceHolder" runat="server"></asp:PlaceHolder>
</body>
</html>
