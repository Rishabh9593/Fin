<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="voting.aspx.cs" Inherits="voting.voting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="text-align:center">
    <form id="form1" runat="server">
    <div style="text-align:center; background-color:lightgoldenrodyellow;">
        <br />
        <br />
    <asp:Label ID="label_question" runat="server" Text="Will AI takeover Humans?" style="font-family:’Lato’, sans-serif; color: gray;"></asp:Label>
        <asp:RadioButtonList ID="answer_list" runat="server" style="align: center" CellPadding="1" CellSpacing="1" RepeatLayout="UnorderedList" ForeColor="White" Height="60px">
            <asp:ListItem style="font-family:’Lato’, sans-serif; color: green;">Yes</asp:ListItem>
            <asp:ListItem style="font-family:’Lato’, sans-serif; color: red;">No</asp:ListItem>
            <asp:ListItem style="font-family:’Lato’, sans-serif; color: blue;">Can&#39;t Say</asp:ListItem>
        </asp:RadioButtonList>
        <asp:Button ID="btn_submit" runat="server" Text="Submit" OnClick="btn_submit_Click" style="background-color:#4CAF50; width: 10%; padding: 9px 5px; margin:5px 0; cursor:pointer; border:none; color:#ffffff; margin-left: 80px;" />

        </div>
    </form>
</body>
</html>
