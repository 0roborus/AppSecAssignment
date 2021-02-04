<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePass.aspx.cs" Inherits="AppSecAssignment.ChangePass" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lbl_message" runat="server" Text=""></asp:Label>
            <br />
            <asp:Label ID="Label1" runat="server" Text="Current Password: "></asp:Label>
            <asp:TextBox ID="tbCPass" runat="server" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Label ID="Label2" runat="server" Text="New Password: "></asp:Label>
            <asp:TextBox ID="tbNPass" runat="server" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Label ID="Label3" runat="server" Text="Confirm New Password: "></asp:Label>
            <asp:TextBox ID="tbCNPass" runat="server" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" />
            <br />
            <asp:Label ID="lbl_error" runat="server" Text=""></asp:Label>
        </div>
    </form>
</body>
</html>
