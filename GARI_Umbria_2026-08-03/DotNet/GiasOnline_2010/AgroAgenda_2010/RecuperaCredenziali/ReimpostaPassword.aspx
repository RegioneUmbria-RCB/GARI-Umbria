<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReimpostaPassword.aspx.vb" Inherits="AgroAgenda_2010.ReimpostaPassword" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ReimpostaPassword %>" runat="server"></asp:Localize></title>
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
        
        #boxFormReimpostaPassword {
            margin-top: 25px;
        }

        #boxFormReimpostaPassword > div {
            margin-bottom: 5px;
            margin-top: 5px;
        }

        
        #txtUsername, #txtConfermaPassword, #btnReimpostaPassword, #btnTornaAllaLogin {
            width: 100%;
        }

    </style>
</head>
<body>

    <script type="text/javascript">
        const GiasVersioneLogin = '<%=Login_Versione %>';
    </script>

    <div id="wrap_master">
        <cc2:AgroMasterPage ID="AgroMasterPage" runat="server" />

        <form id="form1" runat="server">
            <div class="container">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-sx-12 align-self-center">
                        <asp:PlaceHolder ID="headerPlaceHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
                <p class="text-center">
                    <span id="descDominioScelto" runat="server"></span>
                </p>
                <div class="boxEsitoContainer">
                    <div id="boxEsito" class="">
                        <i id="iconEsito" class="fa fa-check-circle"></i><span id="mexEsito"></span>
                    </div>
                </div>
                <div id="boxFormReimpostaPassword">
                    <div class="row justify-content-center">
                        <div class="col-xs-6 col-6 col-xs-offset-3">
                            <div class="input-group">
                                <label class="input-group-addon control-label" id="lblUsername">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Username %>" runat="server"></asp:Localize>
                                </label>
                                <input type="text" runat="server" id="txtUsername" name="txtUsername" class="form-control" autocomplete="off" readonly />
                            </div>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div class="col-xs-6 col-6 col-xs-offset-3">
                            <% If Login_Versione = "2022" %>
                            <label class="input-group-addon control-label" id="lblPassword" for="txtPassword">
                                <!-- Label portata fuori dall'input group per questioni di stile -->
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NuovaPassword %>" runat="server"></asp:Localize>
                            </label>
                            <div class="input-group">
                            <% Else %>
                            <div class="input-group">
                                <label class="input-group-addon control-label" id="lblPassword" for="txtPassword">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NuovaPassword %>" runat="server"></asp:Localize>
                                </label>
                            <%End If %>
                                <input type="password" class="form-control xo-login-password" id="txtPassword" name="txtPassword" aria-describedby="lblPassword" autocomplete="new-password" required />
                                <%--<asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" autocomplete="new-password" required></asp:TextBox>--%>
                                <div class="input-group-addon xo-login-eye" style="max-width: 30px !important; width: 30px; min-width: 30px;">
                                    <button type="button" name="btnMostraNascondiPassword" class="btnAsLink">
                                        <i class="fa fa-eye-slash" aria-hidden="true"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div id="boxInfo" class="col-xs-6 col-6 col-xs-offset-3">
                            <i id="iconInfo" class="fa fa-info-circle"></i><span id="mexInfo" runat="server"></span>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div class="col-xs-6 col-6 col-xs-offset-3">
                            <div class="input-group">
                                <label class="input-group-addon control-label" id="lblConfermaPassword" for="txtConfermaPassword">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ConfermaPassword %>" runat="server"></asp:Localize>
                                </label>
                                <input type="password" class="form-control" id="txtConfermaPassword" name="txtConfermaPassword" aria-describedby="lblUsernameRP" autocomplete="new-password" required />
                                <%--<asp:TextBox runat="server" ID="txtConfermaPassword" TextMode="Password" CssClass="form-control" autocomplete="off" required></asp:TextBox>--%>
                            </div>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div class="col-xs-6 col-6 col-xs-offset-3">
                            <%--<asp:Button runat="server" ID="btnReimpostaPassword" CssClass="btn btn-primary btn-block" Text="Procedi" />--%>
                            <button type="button" name="btnReimpostaPassword" id="btnReimpostaPassword" class="btn btn-primary btn-block">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedi %>" runat="server"></asp:Localize>
                            </button>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div id="boxTornaAllaLogin" class="col-xs-6 col-6 col-xs-offset-3">
                            <%--<button type="button" name="btnTornaAllaLogin" class="btn btn-primary btn-block">Torna alla login</button>--%>
                            <asp:Button runat="server" ID="btnTornaAllaLogin" CssClass="btn btn-primary btn-block" Text="<%$ Resources: AgronicaAgenda_2010, TornaAllaLogin %>" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Hidden field -->
            <input type="hidden" runat="server" id="hfValSuperServer" name="hfValSuperServer" />
            <input type="hidden" runat="server" id="hfUsername" name="hfUsername" />
            <input type="hidden" runat="server" id="hfToken" name="hfToken" />
            <input type="hidden" runat="server" id="hfIsValidSession" name="hfIsValidSession" />
            <input type="hidden" name="hfMexConfermaPassword" value="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, LePasswordNonCorrispondono %>' runat='server'></asp:Localize>" />

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
                    $("#boxEsito").hide();
                    $("#boxTornaAllaLogin").hide();

                    checkResetPasswordSessionExpired();

                    $("[name='btnMostraNascondiPassword']").on("click", function () {
                        var inputPwd = $("[name='txtPassword']");
                        var iconBtn = $(this).children("i");

                        switch (inputPwd.prop("type")) {
                            case "password":
                                inputPwd.prop("type", "text");
                                iconBtn.removeClass("fa-eye-slash");
                                iconBtn.addClass("fa-eye");
                                break;
                            case "text":
                                inputPwd.prop("type", "password");
                                iconBtn.removeClass("fa-eye");
                                iconBtn.addClass("fa-eye-slash");
                                break;
                        }
                    });

                    $("[name='txtPassword']").on("change", validaPassword);
                    $("[name='txtConfermaPassword']").on("input", validaPassword);

                    $("[name='btnReimpostaPassword']").on("click", function () {
                        $("#boxEsito").hide();
                        //ByVal username As String, ByVal pwdNuova As String, ByVal pwdConferma As String, ByVal idSuperServer As Integer
                        var strValSuperServer = $("[name='hfValSuperServer']").val();
                        var strUsername = $("[name='hfUsername']").val();
                        var strToken = $("[name='hfToken']").val();
                        var strPassword = $("[name='txtPassword']").val();
                        var strConfermaPassword = $("[name='txtConfermaPassword']").val();
                        wsReimpostaPassword(strToken, strUsername, strPassword, strConfermaPassword, strValSuperServer, successReimpostaPassword, errorReimpostaPassword);
                    });
                });

                function validaPassword() {
                    var inputPwd = $("[name='txtPassword']")[0];
                    var inputConfPwd = $("[name='txtConfermaPassword']")[0];
                    var boxConfPwd = $(inputConfPwd).parent();

                    if (inputConfPwd.value === "") {
                        inputConfPwd.setCustomValidity("");
                        boxConfPwd.removeClass("has-success has-error");
                    }
                    else {
                        if (inputPwd.value != inputConfPwd.value) {
                            inputConfPwd.setCustomValidity($("[name='hfMexConfermaPassword']").val());
                            boxConfPwd.removeClass("has-success");
                            boxConfPwd.addClass("has-error");
                        }
                        else {
                            inputConfPwd.setCustomValidity("");
                            boxConfPwd.removeClass("has-error");
                            boxConfPwd.addClass("has-success");
                        }
                    }
                    inputConfPwd.reportValidity();
                }

                function wsReimpostaPassword(strToken, strUsername, strPassword, strConfermaPassword, strValSuperServer, callbackSuccess, callbackError) {
                    var params = {
                        token: strToken,
                        username: strUsername,
                        pwdNuova: strPassword,
                        pwdConferma: strConfermaPassword,
                        valSuperServer: strValSuperServer
                    };
                    ajaxAgronicaSync(
                        "./ReimpostaPassword.aspx/wsReimpostaPassword",
                        JSON.stringify(params),
                        false,
                        callbackSuccess,
                        callbackError
                    );
                }

                function successReimpostaPassword(rispServer) {
                    $("#boxFormReimpostaPassword").children("div").hide();
                    risp = rispServer.RispostaStringa;
                    $("#mexEsito").text(risp);
                    $("#iconEsito").removeClass("fa-times-circle");
                    $("#iconEsito").addClass("fa-check-circle");
                    var boxEsito = $("#boxEsito");
                    boxEsito.removeClass("esitoErrore");
                    boxEsito.addClass("esitoSuccesso");
                    boxEsito.show();
                    $("#boxTornaAllaLogin").show();
                }

                function errorReimpostaPassword(rispServer) {
                    risp = rispServer.Errore;
                    $("#mexEsito").text(risp);
                    $("#iconEsito").removeClass("fa-check-circle");
                    $("#iconEsito").addClass("fa-times-circle");
                    var boxEsito = $("#boxEsito");
                    boxEsito.removeClass("esitoSuccesso");
                    boxEsito.addClass("esitoErrore");
                    boxEsito.show();

                    if (rispServer.Tipo != '') {
                        $("#boxTornaAllaLogin").show();
                        $("[name='btnReimpostaPassword']").hide();
                        $("#txtPassword").prop("disabled", true)
                        $("#txtConfermaPassword").prop("disabled", true)
                    }
                }

                function checkResetPasswordSessionExpired() {
                    risp = $("#hfIsValidSession").val();
                    var isValid = (risp == '');

                    $("#txtPassword").prop("disabled", !isValid)
                    $("#txtConfermaPassword").prop("disabled", !isValid)

                    if (!isValid) {
                        $("#boxTornaAllaLogin").show();
                        $("[name='btnReimpostaPassword']").hide();
                        $("#mexEsito").text(risp);
                        $("#iconEsito").removeClass("fa-check-circle");
                        $("#iconEsito").addClass("fa-times-circle");
                        var boxEsito = $("#boxEsito");
                        boxEsito.removeClass("esitoSuccesso");
                        boxEsito.addClass("esitoErrore");
                        boxEsito.show();
                    }
                }
            </script>
        </form>
    </div>
    <asp:PlaceHolder ID="footerPlaceHolder" runat="server"></asp:PlaceHolder>
</body>
</html>
