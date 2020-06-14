<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login_page.aspx.cs" Inherits="assignment.login_page" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 122px;
        }
        .label-des {
        font-family:’Lato’, sans-serif;
    color: gray;
    }   
    </style>
</head>
<body style="text-align: center">
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Welcome to ASP.NET"></asp:Label>
        <div>
        <asp:Label ID="Label2" runat="server" Text="Name:  &nbsp;" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="username" runat="server" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;"></asp:TextBox>
    </div>
        <br />
        <div>
            <asp:Label ID="Label3" runat="server" Text="Password:" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="password" runat="server" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;" TextMode="Password"></asp:TextBox>
        </div>
    </div>
        <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_login" runat="server" Text="Log_in" OnClick="btn_login_Click" style="background-color:#4CAF50; width: 10%; padding: 9px 5px; margin:5px 0; cursor:pointer; border:none; color:#ffffff; margin-left: 80px;" />
            <asp:Button ID="btn_register" runat="server" Text="Register" style="background-color:#4CAF50; width: 10%; padding: 9px 5px; margin:5px 0; cursor:pointer; border:none; color:#ffffff; margin-left: 80px;" OnClick="btn_register_Click"/>
        </p>
    </form>
    </body>
</html>
