<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReimpostazionePasswordPRO.aspx.vb" Inherits="AgronicaWebApiProfilatore.ReimpostazionePasswordPRO" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Reimposta Password Profitosan</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.css" />
    <style type="text/css">

        body {
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        .centered-card {
            max-width: 400px;
            margin: 40px auto;
            box-shadow: 04px 24px rgba(0, 0, 0, 0.25);
            border-radius: 16px;
            background: rgb(18, 64, 34);
            padding: 32px 24px;
        }

        .logo {
            display: block;
            margin: 0 auto 24px auto;
            max-width: 180px;
        }

        .form-label {
            font-weight: 500;
            color: #fff;
        }

        .input-group-text {
            background-color: rgb(18, 64, 34);
            border-width: 2px;
            color: #fff;
            border-color: #fff;
        }

        .form-control {
            border: 1px solid #ced4da;
        }

        .form-control:focus {
            border-color: rgb(18, 64, 34);
            box-shadow: 0 0.2rem rgba(13, 110, 253, 0.25);
        }

        .btn-primary {
            padding: 10px 0;
            border-radius: 8px;
            background: rgb(18, 64, 34);
            transition: background 0.2s;

            font-size: 18px !important;
            width: 100%;
            font-weight: 700;
            text-transform: uppercase;
            border-width: 2px;
            color: #fff;
            border-color: #fff;
        }

        .btn-primary:hover, .btn-primary:focus {
            background: rgb(18, 64, 34);
            border-color: #fff;
        }

        .bi {
            cursor: pointer;
        }

    </style>
</head>
<body>
    <div class="centered-card">
        <!-- Logo SVG Profitosan -->
        <div style="text-align:center;margin-bottom:24px;">
          <img class="logo" src="https://app.profitosan.it/assets/images/logo.svg" alt="Profitosan Logo" />
        </div>
        <form id="formNuovaPassword" runat="server">
            <div class="mb-4">
                <label class="form-label" for="txtNuovaPassword">
                    <asp:Localize runat="server">Nuova Password</asp:Localize>
                </label>
                <div class="input-group">
                    <input type="password" class="form-control" name="txtNuovaPassword" id="txtNuovaPassword" autocomplete="new-password" runat="server" />
                    <span class="input-group-text"><i class="bi bi-eye-slash" id="toggleNuovaPassword"></i></span>
                </div>
            </div>
            <div class="mb-4">
                <label class="form-label" for="txtRipetiNuovaPassword">
                    <asp:Localize runat="server">Ripeti Nuova Password</asp:Localize>
                </label>
                <div class="input-group">
                    <input type="password" class="form-control" name="txtRipetiNuovaPassword" id="txtRipetiNuovaPassword" autocomplete="new-password" runat="server" />
                    <span class="input-group-text"><i class="bi bi-eye-slash" id="toggleRipetiNuovaPassword"></i></span>
                </div>
            </div>
            <div class="mb-2">
                <input type="button" name="btnConferma" id="btnConferma" runat="server" value="Modifica Password" class="btn btn-primary" />
            </div>
        </form>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-C6RzsynM9kWDrMNeT87bh95OGNyZPhcTNXj1NW7RuBCsyN/o0jlpcV8Qyq46cDfL" crossorigin="anonymous"></script>
    <script>
        const toggleNuovaPassword = document.querySelector("#toggleNuovaPassword");
        const txtNuovaPassword = document.querySelector("#txtNuovaPassword");

        toggleNuovaPassword.addEventListener("click", function () {
            // toggle the type attribute
            const type = txtNuovaPassword.getAttribute("type") === "password" ? "text" : "password";
            txtNuovaPassword.setAttribute("type", type);
            // toggle the icon
            this.classList.toggle("bi-eye");
            this.classList.toggle("bi-eye-slash");
        });

        const toggleRipetiNuovaPassword = document.querySelector("#toggleRipetiNuovaPassword");
        const txtRipetiNuovaPassword = document.querySelector("#txtRipetiNuovaPassword");

        toggleRipetiNuovaPassword.addEventListener("click", function () {
            // toggle the type attribute
            const type = txtRipetiNuovaPassword.getAttribute("type") === "password" ? "text" : "password";
            txtRipetiNuovaPassword.setAttribute("type", type);
            // toggle the icon
            this.classList.toggle("bi-eye");
            this.classList.toggle("bi-eye-slash");
        });
    </script>
</body>
</html>
