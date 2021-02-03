    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AppSecAssignment.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function check() {
            var str = document.getElementById('<%=tbPassword.ClientID %>').value;
            var str1 = document.getElementById('<%=tbConfirmPassword.ClientID %>').value;

            if (str != str1) {
                document.getElementById('lbl_pwdchecker2').innerHTML = "Passwords do not match!";
                document.getElementById('lbl_pwdchecker2').style.color = "red";
                return ('0')
            }
            else {
                document.getElementById('lbl_pwdchecker2').innerHTML = "Good to go!";
                document.getElementById('lbl_pwdchecker2').style.color = "green";
                return ('1')
            }
        };
        function validate() {
            var str = document.getElementById('<%=tbPassword.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password Length Must be at Least 8 Charcters!";
                document.getElementById('lbl_pwdchecker').style.color = "red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 number!";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_number");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 lowercase letter!";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_lower");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 Uppercase letter!";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_upper");
            }
            else if (str.search(/[^\w]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 special letter!";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_special");
            }

            document.getElementById('lbl_pwdchecker').innerHTML = "Excellent!";
            document.getElementById('lbl_pwdchecker').style.color = "blue";

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="First Name: "></asp:Label>
            <asp:TextBox ID="tbFirstName" runat="server" ></asp:TextBox>
            <br />
            <asp:Label ID="Label2" runat="server" Text="Last Name: "></asp:Label>
            <asp:TextBox ID="tbLastName" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label7" runat="server" Text="Date of Birth"></asp:Label>
            <asp:TextBox ID="tbDOB" runat="server" TextMode="Date"></asp:TextBox>
            <br />
            <asp:Label ID="Label3" runat="server" Text="Credit Card: "></asp:Label>
            <asp:TextBox ID="tbCC" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label4" runat="server" Text="Email: "></asp:Label>
            <asp:TextBox ID="tbEmail" runat="server" TextMode="Email"></asp:TextBox>
            <br />
            <asp:Label ID="Label5" runat="server" Text="Password: "></asp:Label>
            <asp:TextBox ID="tbPassword" runat="server" OnTextChanged="tbPassword_TextChanged" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
            <asp:Label ID="lbl_pwdchecker" runat="server" Text=""></asp:Label>
            <asp:Button ID="btCheckPass" runat="server" Text="Check Password" OnClick="btCheckPass_Click" Width="150px" />
            <asp:Label ID="lbl_pwdchecker1" runat="server" Text=""></asp:Label>
            <br />
            <asp:Label ID="Label6" runat="server" Text="Confirm Password: "></asp:Label>
            <asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" onkeyup="javascript:check()"></asp:TextBox>
            <asp:Label ID="lbl_pwdchecker2" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnCreate" runat="server" Text="Register" Width="200px" OnClick="btnCreate_Click" />
            <asp:Label ID="lbl_error" runat="server" Text=""></asp:Label>
        </div>
    </form>
</body>
</html>
