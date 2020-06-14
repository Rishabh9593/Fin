<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reg_page.aspx.cs" Inherits="assignment.reg_page" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="text-align:center;">
    <h2>
        Register Here
    </h2>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Name:  &nbsp;" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="name" runat="server" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;"></asp:TextBox>
    </div>
        <br />
        <div>
            <asp:Label ID="Label2" runat="server" Text="College:  &nbsp;" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="college" runat="server" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;"></asp:TextBox>
        </div>
        <br />
        <div>
            <asp:Label ID="Label3" runat="server" Text="Address:  &nbsp;" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="address" runat="server" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;"></asp:TextBox>
        </div>
        <br />
        <div>
            <asp:Label ID="Label4" runat="server" Text="Username:  &nbsp;" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="username" runat="server" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;"></asp:TextBox>
        </div>
        <br />
        <div>
            <asp:Label ID="Label5" runat="server" Text="Password:  &nbsp;" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:TextBox ID="password" runat="server" TextMode="Password" style="width: 20%; margin: 8px 0; padding: 7px 10px; display: inline-block; border:1px solid #ccc; box-sizing: border-box;"></asp:TextBox>
        </div>
        <br />
        <div>
            <asp:Button ID="register" runat="server" Text="Register" OnClick="register_Click" style="background-color:#4CAF50; width: 10%; padding: 9px 5px; margin:5px 0; cursor:pointer; border:none; color:#ffffff; margin-left: 80px;" />
        </div>
    </form>

</body>
</html>
