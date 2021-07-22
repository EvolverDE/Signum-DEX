Public Class FrmManual


    Shared Property frmManual As New FrmManual

    Public Enum CustomDialogResult
        OK
        Yes
        No
        Close
    End Enum

    Shared Property CustomResult As CustomDialogResult


    Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        CustomResult = CustomDialogResult.Close

    End Sub

    Public Shared Function MBox(Optional ByVal msgTxt As String = "", Optional ByVal titleTxt As String = "", Optional ByVal buttons As List(Of Button) = Nothing, Optional ByVal c_Color As Color = Nothing, Optional ByVal status As ClsMsgs.Status = ClsMsgs.Status.Standard, Optional ByVal t_time As Integer = -1, Optional ByVal timer_typ As ClsMsgs.Timer_Type = ClsMsgs.Timer_Type.ButtonEnable)

        ' Dim ms As FrmManual = New FrmManual
        Dim Result As CustomDialogResult = ShowManuDialog()

        Return Result
    End Function


    Shared Sub frmMSG_closing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs)
        e.Cancel = False
    End Sub

    Public Shared Function ShowManuDialog()

        AddHandler frmManual.FormClosing, AddressOf frmMSG_closing

        frmManual.ShowDialog()
        Return CustomResult

    End Function




    Private Sub FrmManual_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim WelcomeStr As String = "Welcome to a decentrailze exchange for SIGNA." + vbCrLf
        WelcomeStr += "" + vbCrLf
        WelcomeStr += "###This is just a testapplication! You use it at your own risk!###" + vbCrLf
        WelcomeStr += "" + vbCrLf
        WelcomeStr += "Why Should use that instead of centralized ones?" + vbCrLf
        WelcomeStr += "Because you don’t have to verify your real identity to a company, just to that one you deal with." + vbCrLf
        WelcomeStr += "" + vbCrLf
        WelcomeStr += "Why do not exist a Token for this exchange?" + vbCrLf
        WelcomeStr += "Because you dont need an extra token for this kind of exchanging. It follows the ""Be your own bank""-Schema" + vbCrLf
        WelcomeStr += "You chose your Risk yourself by creating a Order with the amount of the Collateral" + vbCrLf
        WelcomeStr += "" + vbCrLf
        WelcomeStr += "" + vbCrLf

        LabWelcome.Text = WelcomeStr


        Dim FirstStepsStr As String = "It seems you start this program for the first time (or settings are gone) and it has to configure some settings." + vbCrLf
        FirstStepsStr += "" + vbCrLf
        FirstStepsStr += "1. The first thing to do is to set the passphrase for an account." + vbCrLf
        FirstStepsStr += "" + vbCrLf
        FirstStepsStr += "2. Close the program and click ""Yes"" on the Setting changed prompt. (if it take no effect, restart program as Administrator)" + vbCrLf
        FirstStepsStr += "" + vbCrLf
        FirstStepsStr += "3. Restart the Program and let load/check the Ats of the Blockchain (give them some time)" + vbCrLf
        FirstStepsStr += "" + vbCrLf
        FirstStepsStr += "4. the program is ready to use (optonally set some extra settings in the Settings-Tab)" + vbCrLf
        FirstStepsStr += "" + vbCrLf

        LabFirstSteps.Text = FirstStepsStr


        Dim Example1 As String = "once the program started correctly and you want to Sell some SIGNA, you chose ""Sell"" at point ""1""." + vbCrLf
        Example1 += "" + vbCrLf
        Example1 += "At ""2"" you set the amount you want to sell." + vbCrLf
        Example1 += "" + vbCrLf
        Example1 += "to prevent scamming chose the collateral amount at ""3""." + vbCrLf
        Example1 += "" + vbCrLf
        Example1 += "now you chose the amount of the Item you want to get (""4"")." + vbCrLf
        Example1 += "" + vbCrLf
        Example1 += "optionally chose an Transaction Fee amount." + vbCrLf
        Example1 += "" + vbCrLf
        Example1 += "click Set Order to place it to the market (""5"")." + vbCrLf

        LabExample1.Text = Example1


        Dim Example2 As String = "After a while your Order will also be showed in ""MyOrders"" tab in the ""MyOpenOrders"" List (at ""1"")." + vbCrLf
        Example2 += "" + vbCrLf

        LabExample2.Text = Example2


        Dim Example3 As String = "For the time the Order is open, everyone can also see the Order in the ""SellOrders""-Section on the ""Marketdetails""-Tab." + vbCrLf
        Example3 += "" + vbCrLf
        Example3 += "To Buy the Order just select it and click ""Buy"". In case you are the Seller AND Buyer, the Order will canceled." + vbCrLf
        Example3 += "" + vbCrLf
        Example3 += "The same procedure applies to Sell orders. With the exception that a encrypted message is also sent to the buyer with payment infos." + vbCrLf

        LabExample3.Text = Example3


        Dim Example4 As String = "If you are ok with the conditions of the Order, just confirm it with a click on ""Yes"" in the Buy-Prompt" + vbCrLf
        Example4 += "" + vbCrLf
        Example4 += "A Transaction will be created to inform the AT." + vbCrLf

        LabExample4.Text = Example4


        Dim Example5 As String = "On the Buyer side the Accepted Order will also be showed in the ""MyOpenOrder""-List." + vbCrLf

        LabExample5.Text = Example5


        Dim Example6 As String = "In case that your settings are configured to send the Buyer Payment infos (""2"") automatically (""1""), the program send it as soon as there is a buyer." + vbCrLf
        Example6 += "" + vbCrLf
        Example6 += "alternatively to (""2"") you can send stored PayPal information like PayPal-E-Mail oder PayPal Account ID." + vbCrLf
        Example6 += "" + vbCrLf
        Example6 += "For the best automatic experience you can create PayPal-Orders (""A"") that make purchases on the Buyer side as easy as possible." + vbCrLf
        Example6 += "For this option you need a PayPal-Business-Account (""B"") and a PayPal-App with its API-User and API-Secret." + vbCrLf

        LabExample6.Text = Example6



    End Sub

    Private Sub btManualSetPassPhrase_Click(sender As Object, e As EventArgs) Handles btManualSetPassPhrase.Click

        Dim BCR As ClsSignumAPI = New ClsSignumAPI With {.C_PassPhrase = TBManualPassPhrase.Text}

        Dim x As List(Of String) = BCR.GetAccountFromPassPhrase()

        Dim Address As String = BetweenFromList(x, "<address>", "</address>")
        Dim Balance As String = BetweenFromList(x, "<available>", "</available>")
        Dim AccountID As String = BetweenFromList(x, "<account>", "</account>")


        If Address.Trim = "False" Then
            ClsMsgs.MBox("Unknown Address", "Check Address", ,, ClsMsgs.Status.Erro, 5, ClsMsgs.Timer_Type.AutoOK)
        Else
            PFPForm.T_PassPhrase = TBManualPassPhrase.Text
            SetINISetting(E_Setting.PassPhrase, TBManualPassPhrase.Text)
            ClsMsgs.MBox("Address OK", "Check Address", ,, ClsMsgs.Status.Information, 5, ClsMsgs.Timer_Type.AutoOK)
        End If

    End Sub

    Private Sub FrmManual_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Me.Hide()
    End Sub

End Class