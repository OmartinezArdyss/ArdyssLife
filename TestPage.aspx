<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="ArdyssLife.TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   
        <asp:Button ID="Button1" runat="server" Text="Button" OnClientClick="AddCart(); return false"  />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    </form>

    <script src="https://code.jquery.com/jquery-3.0.0.js"></script>
    <script src="https://bodymagic-world.myshopify.com/cart/add.js"></script>
    <script>

        function AddCart()
        {
            try
            {
                alert("Empieza Peticion POST");

                jQuery.post('https://bodymagic-world.myshopify.com/cart/add.js=quantity=1&id=7647266183', {
                    quantity: 1,
                    id: 7647266183
                });

                alert("Peticion Existosa");

            } catch (e)
            {
                alert("Exception:"+e.message)
            }
        }

    </script>


</body>
</html>
