<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorManaged.aspx.cs" Inherits="ArdyssLife.ErrorManaged" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ardyss Life ErrorManaged</title>
    <meta charset="utf-8" />
    <!-- mobile settings -->
    <meta name="viewport" content="width=device-width, maximum-scale=1, initial-scale=1, user-scalable=0" />
    <!--[if IE]><meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'><![endif]-->

    <!-- WEB FONTS : use %7C instead of | (pipe) -->
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400%7CRaleway:300,400,500,600,700%7CLato:300,400,400italic,600,700" rel="stylesheet" type="text/css" />

    <!-- CORE CSS -->
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />

    <%--Se Modifico--%>
    <!-- THEME CSS -->
    <link href="assets/css/essentials.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/layout.css" rel="stylesheet" type="text/css" />
    <!-- PAGE LEVEL SCRIPTS -->
    <link href="assets/css/header-1.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/layout-shop.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/color_scheme/green.css" rel="stylesheet" type="text/css" id="color_scheme" />
    <link rel="Stylesheet" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.10/themes/redmond/jquery-ui.css" />
    <link href="Styles/ArdyssStyles.css" rel="stylesheet" />
    <link rel="shortcut icon" href="assets/img/favicon.ico" />



</head>
<body>
    <form id="form1" runat="server">
        <div id="header" class="sticky clearfix">
            <!-- TOP NAV -->
            <header id="topNav">
                <div class="container">
                    <!-- Mobile Menu Button -->
                    <button class="btn btn-mobile" data-toggle="collapse" data-target=".nav-main-collapse"><i class="fa fa-bars"></i></button>
                    <!-- Logo -->
                    <a class="logo pull-left" href="https://ardysslife.com">
                        <img src="assets/images/OwnImages/logo_dark2.png" alt="" /></a>

                    <div class="navbar-collapse pull-right nav-main-collapse collapse">
                        <nav class="nav-main">
                            <ul id="topMain" class="nav nav-pills nav-main">
                                <li class="active">
                                    <!-- HOME -->
                                    <a href="https://ardysslife.com">
                                        <asp:Label ID="Label1" runat="server" Text="RETURN TO ARDYSS LIFE"></asp:Label></a></li>
                            </ul>
                        </nav>
                    </div>
                </div>
            </header>
            <!-- /Top Nav -->
        </div>

        <section class="page-header page-header-lg parallax parallax-3" style="background-image: url('../assets/images/OwnImages/1200x800/fondo_hojas.jpg')">

            <div class="overlay dark-3">
                <!-- dark overlay [1 to 9 opacity] -->
            </div>

        </section>
        <%--aqui va la seccion --%>
        <section class="padding-xlg">
            <div class="container">

                <div class="row">
                    <div style="width: 30%; margin-left: auto; margin-right: auto">
                        <img src="assets/images/OwnImages/perfil_picture_big.png" width="100%" />
                    </div>
                    <div style="width: 30%; margin-left: auto; margin-right: auto">
                        <h3><b>
                            <asp:Label ID="Label7" runat="server" Text="WELCOME TO ARDYSS"></asp:Label></b></h3>
                        <p>
                            <asp:Label ID="Label8" runat="server" Text="In order get access to this personal website, you need to the URL the username of the person who invited you to Ardyss."></asp:Label>
                        </p>
                        <div class="browser-img">
                            <img src="assets/images/OwnImages/browser_sample.png" width="100%">
                        </div>
                        <p>
                            <asp:Label ID="Label9" runat="server" Text="If you don't know any Ardyss independient distribuitor, you can contact us ">
                            </asp:Label>
                            <asp:HyperLink ID="HyperLink1" class="size-20 font-lato" NavigateUrl="https://ardysslife.com" runat="server">here.</asp:HyperLink>
                        </p>


                    </div>
                </div>

            </div>
        </section>

        <!-- PRELOADER -->
        <div id="preloader">
            <div class="inner">
                <span class="loader"></span>
            </div>
        </div>
        <!-- /PRELOADER -->

    </form>
    <!-- FOOTER -->
    <footer id="footer">
        <div class="copyright">
            <div class="container">
                <ul class="pull-right nomargin list-inline mobile-block redes">
                    <li><a href="#" class="facebook"></a></li>
                    <li><a href="#" class="twitter"></a></li>
                    <li><a href="#" class="pinterest"></a></li>
                    <li><a href="#" class="youtube"></a></li>
                </ul>
                <a href="#">
                    <asp:Label ID="Label2" runat="server" Text="Ardyss Life"></asp:Label></a>
                <asp:Label ID="Label3" runat="server" Text="© 2016 All rights reserved"></asp:Label>
                <a href="#">
                    <asp:Label ID="Label4" runat="server" Text="Terms of use"></asp:Label></a>
                <asp:Label ID="Label5" runat="server" Text="and"></asp:Label>
                <a href="#">
                    <asp:Label ID="Label6" runat="server" Text="privacy policy"></asp:Label></a>
            </div>
        </div>
    </footer>
    <!-- /FOOTER -->

    <script type="text/javascript">var plugin_path = '../assets/plugins/';</script>
    <script type="text/javascript" src="../assets/plugins/jquery/jquery-2.1.4.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <script type="text/javascript" src="../assets/js/scripts.js"></script>
    <script type="text/javascript" src="../assets/js/view/demo.shop.js"></script>
</body>
</html>
