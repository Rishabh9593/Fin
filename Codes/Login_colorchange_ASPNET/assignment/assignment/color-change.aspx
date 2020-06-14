<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="color-change.aspx.cs" Inherits="assignment.color_change" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="text-align:center;">
    <form id="form1" runat="server">
    <div>
    <h2 style="font-family:’Lato’, sans-serif; color: black;"> &nbsp;</h2>
        <h2 style="font-family:’Lato’, sans-serif; color: black;"> &nbsp;</h2>
        <h2 style="font-family:’Lato’, sans-serif; color: black;"> Select a color!</h2>
        <asp:DropDownList ID="select_color_list" runat="server" OnSelectedIndexChanged="select_color_SelectedIndexChanged" AutoPostBack="True">
            <asp:ListItem>Black</asp:ListItem>
            <asp:ListItem>Grey</asp:ListItem>
            <asp:ListItem>Yellow</asp:ListItem>
            <asp:ListItem>Green</asp:ListItem>
            <asp:ListItem>Blue</asp:ListItem>
            <asp:ListItem>White</asp:ListItem>
            <asp:ListItem>Red</asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <br />
    </div>
        <asp:Label ID="label_withcolor" runat="server" Text="Text in Black!"></asp:Label>
    </form>
</body>
</html>
