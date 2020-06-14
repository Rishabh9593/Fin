<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="results.aspx.cs" Inherits="voting.results" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Results!</title>
</head>
<body style="text-align:center; background-color:lightgoldenrodyellow;">
    <form id="form1" runat="server">
    <div>
        <h2> VOTE PERCENTAGE</h2>
        <div style="height: 174px; margin-top: 47px"> 
            Yes: 
            <asp:Label ID="label_yes_count" runat="server" Text="0"></asp:Label>
            <br />
            No:
            <asp:Label ID="label_no_count" runat="server" Text="0"></asp:Label>
            <br />
            Can't Say:
            <asp:Label ID="label_cantsay_count" runat="server" Text="0"></asp:Label>
            <br />
            <br />
            <asp:Button ID="LogOut" runat="server" Text="Log out" OnClick="btn_logout_click" style="background-color:#4CAF50; width: 10%; padding: 9px 5px; margin:5px 0; cursor:pointer; border:none; color:#ffffff; margin-left: 80px;" />
        </div>
    
    </div>
    </form>
</body>
</html>
