<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSecAssignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Email: "></asp:Label>
            <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label2" runat="server" Text="Password: "></asp:Label>
            <asp:TextBox ID="tbPassword" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
            <br />
            <br />
            <asp:Label ID="lbl_Error" runat="server" Text=""></asp:Label>
            <br />
            <div class="g-recaptcha" data-sitekey="6LcFXuUZAAAAAIEsKRd2pOSMNndfJLL6L5OHTt1b"></div>
        </div>
    </form>
</body>
</html>
