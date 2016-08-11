<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageTest.aspx.cs" Inherits="ArdyssLife.PageTest" %>

<html>

<head>
    <title>Test Ajax ShopiFy</title>

    <script src="https://code.jquery.com/jquery-2.2.4.js"></script>
    <script type="text/javascript">

        function AddCart()
        {
            try
            {

            } catch (e)
            {
                alert(e.message);
            }
        }

    </script>


</head>
<body>
    <form runat="server">

        <input id="Button1" type="button" value="button" onclick="AddCart(); return false" />

    </form>
</body>
</html>
