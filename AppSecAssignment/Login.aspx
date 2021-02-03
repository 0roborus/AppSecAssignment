<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSecAssignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="https://www.google.com/recaptcha/api.js?render=6LcZa0YaAAAAAHJA0LFUF5jQedNIfuJbdtDGeNvS"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Email: "></asp:Label>
            <asp:TextBox ID="tbEmail" runat="server" TextMode="Email"></asp:TextBox>
            <br />
            <asp:Label ID="Label2" runat="server" Text="Password: "></asp:Label>
            <asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <br />
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
            <br />
            <br />
            <asp:Label ID="lbl_Error" runat="server" Text=""></asp:Label>
            <asp:Label ID="lbl_Test" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnRecover" runat="server" Text="Recover Account" OnClick="btnRecover_Click" Visible="false"/>
        </div>
    </form>
    <script>
            grecaptcha.ready(function () {
                grecaptcha.execute('6LcZa0YaAAAAAHJA0LFUF5jQedNIfuJbdtDGeNvS', { action: 'Login' }).then(function (token) {
                    document.getElementById("g-recaptcha-response").value = token;
                });
            });
    </script>
</body>
</html>
