<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Recover.aspx.cs" Inherits="AppSecAssignment.Recover" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Image ID="Image1" runat="server" Height="55px" Width="186px" ImageUrl="~/Captcha.aspx" />
            <br />
            <asp:Label ID="Label4" runat="server" Text="Enter Verification Code"></asp:Label>
            <asp:TextBox ID="tbVC" runat="server"></asp:TextBox>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
            <br />
            <asp:Label ID="lbCaptchaMessage" runat="server" Text=""></asp:Label>
        </div>
    </form>
</body>
</html>
