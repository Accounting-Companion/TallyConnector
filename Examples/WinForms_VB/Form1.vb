Imports TallyConnector.Models

Public Class Form1
    Dim CTally As New TallyConnector.Tally

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        CTally.Setup("http://localhost", 9000, "Comname")
        If Await CTally.Check() Then
            Dim Voucher As Voucher = Await CTally.GetVoucherByVoucherNumber("Vchnum", "Vchdate")
        End If
    End Sub
End Class
