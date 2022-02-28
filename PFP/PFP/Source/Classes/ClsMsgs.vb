Option Strict On
Option Explicit On

Public Class ClsMsgs

    Shared Buttonsl As List(Of Button) = New List(Of Button)

    Shared frmText As String
    Shared msgText As String
    Shared t_Time As Integer
    Shared t_TimeType As Timer_Type
    Shared c_Color As Color

    Shared Property frmMSG As New Form
    Shared Property DefBut As Button

    Shared ButtonTimer As Timer
    Shared CustomResult As CustomDialogResult

    Public Shared Function MBox(Optional ByVal msgTxt As String = "", Optional ByVal titleTxt As String = "", Optional ByVal buttons As List(Of Button) = Nothing, Optional ByVal c_txtColor As Color = Nothing, Optional ByVal status As ClsMsgs.Status = ClsMsgs.Status.Standard, Optional ByVal t_time As Integer = -1, Optional ByVal timer_typ As ClsMsgs.Timer_Type = ClsMsgs.Timer_Type.ButtonEnable) As ClsMsgs.CustomDialogResult

        Dim buts As List(Of Button) = New List(Of Button)
        Dim but As Button = New Button

        With but
            .Text = "OK"
            .Name = "DefBut"
            .Tag = ClsMsgs.CustomDialogResult.OK
        End With

        If buttons Is Nothing Then
            DefBut = but
            buts.Add(but)
        Else

            For Each Buton As Button In buttons

                If Buton.Name = "DefBut" Then
                    DefBut = Buton
                    Exit For
                End If

            Next

            buts = buttons
        End If

        Dim ms As ClsMsgs = New ClsMsgs(titleTxt, msgTxt, buts, c_txtColor, status, t_time, timer_typ)
        Dim res As ClsMsgs.CustomDialogResult = ClsMsgs.msbox

        Return res
    End Function

    Public Shared Function DefaultButtonMaker(ByVal standard As DBList, Optional ByVal BtName As String = "OK") As List(Of Button)

        'Dim ButList As List(Of String) = New List(Of String)({Split(BtName, BtName, , "List")})

        Select Case standard
            Case DBList.OneOnly
                Return New List(Of Button)({New Button With {.Text = BtName, .Name = "DefBut", .Tag = CustomDialogResult.OK}})
            Case DBList._YesNo
                Return New List(Of Button)({New Button With {.Text = "Yes", .Name = "DefBut", .Tag = CustomDialogResult.Yes}, New Button With {.Text = "No", .Tag = CustomDialogResult.No}})
            Case DBList.Yes_No
                Return New List(Of Button)({New Button With {.Text = "Yes", .Tag = CustomDialogResult.Yes}, New Button With {.Text = "No", .Name = "DefBut", .Tag = CustomDialogResult.No}})
            Case DBList._YesNoCancel
                Return New List(Of Button)({New Button With {.Text = "Yes", .Name = "DefBut", .Tag = CustomDialogResult.Yes}, New Button With {.Text = "No", .Tag = CustomDialogResult.No}, New Button With {.Text = "Cancel", .Tag = CustomDialogResult.Close}})
            Case DBList.Yes_NoCancel
                Return New List(Of Button)({New Button With {.Text = "Yes", .Tag = CustomDialogResult.Yes}, New Button With {.Text = "No", .Name = "DefBut", .Tag = CustomDialogResult.No}, New Button With {.Text = "Cancel", .Tag = CustomDialogResult.Close}})
            Case DBList.YesNo_Cancel
                Return New List(Of Button)({New Button With {.Text = "Yes", .Tag = CustomDialogResult.Yes}, New Button With {.Text = "No", .Tag = CustomDialogResult.No}, New Button With {.Text = "Cancel", .Name = "DefBut", .Tag = CustomDialogResult.Close}})
            Case Else
                Return New List(Of Button)({New Button With {.Text = "OK", .Name = "DefBut", .Tag = CustomDialogResult.OK}})

        End Select

    End Function

    Sub New(Optional ByVal title As String = "", Optional ByVal msg As String = "", Optional ByVal Buttons As List(Of Button) = Nothing, Optional ByVal colorr As Color = Nothing, Optional ByVal status As Status = Status.Standard, Optional ByVal t_timer As Integer = -1, Optional ByVal timer_type As Timer_Type = Timer_Type.ButtonEnable)


        Buttonsl.Clear()
        CustomResult = CustomDialogResult.Close


        If title <> "" Then
            frmText = title
        Else
            frmText = PFPForm.Text
        End If

        If msg <> "" Then
            msgText = msg
        Else
            msgText = "No Informations."
        End If


        If Not IsNothing(colorr) Then
            c_Color = colorr
        Else
            c_Color = Color.Black
        End If

        If Not Buttons Is Nothing Then
            Buttonsl.AddRange(Buttons)
        Else
            Dim button As Button = New Button
            button.Text = "OK"
            button.Tag = CustomDialogResult.OK
            Buttonsl.Add(button)
        End If


        If t_timer > -1 Then
            t_Time = t_timer
            DefBut.Text += " (" + t_Time.ToString + ")"
        Else
            t_Time = -1
        End If

        t_TimeType = timer_type

        frmMSG.Controls.Clear()

        With frmMSG
            .Name = "name"
            .Height = 300
            'If t_Time > -1 Then
            '    .Text = frmText + " (" + t_Time.ToString + ")"
            'Else
            .Text = frmText
            'End If

            .StartPosition = FormStartPosition.CenterScreen
            .FormBorderStyle = FormBorderStyle.FixedSingle
            .MaximizeBox = False
            .MinimizeBox = False
            Dim fbr As Integer = Convert.ToInt32(PFPForm.Width / 3)

            If fbr < 300 Then
                .Width = 300
            Else
                .Width = fbr
            End If


            .Font = New Font("Arial", 8, FontStyle.Bold)
            '.BackColor = Color.LightPink
            .ShowIcon = False
        End With






        Dim con As New SplitContainer
        With con
            .Name = "splitcon"
            .Dock = DockStyle.Fill
            .Orientation = Orientation.Horizontal
            .IsSplitterFixed = True
            .FixedPanel = FixedPanel.Panel2
            .Panel1.BackColor = Color.White
        End With



        Dim icobmp As PictureBox = New PictureBox

        icobmp.SizeMode = PictureBoxSizeMode.CenterImage

        If status = ClsMsgs.Status.Question Then
            icobmp.Image = SystemIcons.Question.ToBitmap
            con.BackColor = Color.LightSkyBlue
        ElseIf status = ClsMsgs.Status.Information Then
            icobmp.Image = SystemIcons.Information.ToBitmap
            con.BackColor = Color.FromArgb(0, 102, 255)
        ElseIf status = ClsMsgs.Status.Warning Then
            icobmp.Image = SystemIcons.Warning.ToBitmap
            con.BackColor = Color.Yellow
        ElseIf status = ClsMsgs.Status.Attention Then
            icobmp.Image = SystemIcons.Exclamation.ToBitmap
            con.BackColor = Color.LightPink
        ElseIf status = ClsMsgs.Status.Erro Then
            icobmp.Image = SystemIcons.Error.ToBitmap
            con.BackColor = Color.Crimson
        Else
            icobmp.Image = SystemIcons.Information.ToBitmap
            con.BackColor = Color.PaleGreen

        End If



        Dim icowidth As Integer = icobmp.Image.Width
        icobmp.Dock = DockStyle.Fill










        Dim con1 As New SplitContainer
        With con1
            .Name = "splitcon"
            .Dock = DockStyle.Fill
            .Orientation = Orientation.Vertical
            .IsSplitterFixed = True
            .FixedPanel = FixedPanel.Panel2
            .Panel2.AutoScroll = True
        End With


        Dim con2 As New SplitContainer
        With con2
            .Name = "splitcon"
            .Dock = DockStyle.Fill
            .Orientation = Orientation.Vertical
            .IsSplitterFixed = True
            .FixedPanel = FixedPanel.Panel2
        End With

        Dim lab As New Label
        With lab
            .Text = msgText
            .ForeColor = c_Color
            .Name = "message"
            .Location = New Point(7, 10)

        End With


        For butidx As Integer = 0 To Buttonsl.Count - 1

            Dim but As Button = Buttonsl.Item(butidx)


            With but
                AddHandler .Click, AddressOf But_click

                If butidx = 0 Then
                    .Location = New Point(0, 10)
                Else
                    Dim xl As Integer = Buttonsl.Item(butidx - 1).Location.X
                    'Dim yl As Integer = Buttons.Item(i - 1).Location.Y
                    xl += Buttonsl.Item(butidx - 1).Width '+ 10
                    .Location = New Point(xl, 10)
                End If

                .AutoSize = True

                If t_Time > -1 Then
                    If timer_type = Timer_Type.ButtonEnable Then
                        .Enabled = False
                        .BackColor = Color.LightGray
                    Else
                        .Enabled = True
                        .BackColor = Control.DefaultBackColor
                    End If

                Else
                    .Enabled = True
                    .BackColor = Control.DefaultBackColor
                End If

            End With

            Buttonsl.Item(butidx) = but
        Next


        If t_Time > -1 Then

            ButtonTimer = New Timer
            ButtonTimer.Interval = 1000

            AddHandler ButtonTimer.Tick, AddressOf CountDownTimerTick

            ButtonTimer.Enabled = True
            ButtonTimer.Start()

        End If



        'Dim y As Integer = 0
        ' Dim sf As SizeF = TextMessung(lab.Text, lab.Font)
        Dim hm As SizeF = TextMessung("gÜ", lab.Font)
        Dim wm As SizeF = TextMessung("i", lab.Font)



        If Not msgText.Contains(vbCrLf) Then

            Dim max As Integer = frmMSG.Width

            If Not status = ClsMsgs.Status.Standard Then
                max = frmMSG.Width - icobmp.Width
            End If

            Dim tex As String = lab.Text
            Dim charN As Integer = Convert.ToInt32(max / wm.Width)

            Dim cnt As Integer = 1
            For i As Integer = 0 To tex.Length - 1

                If i Mod charN = 0 And i <> 0 Then
                    tex = tex.Insert(i, vbCrLf)
                    cnt += 1
                End If

            Next

            lab.Text = tex

            If cnt < 14 Then
                cnt = 14
            End If

            lab.Height = Convert.ToInt32(cnt * (Math.Round(hm.Height, 0, MidpointRounding.AwayFromZero) + 0.7))

        Else
            Dim cnt As Integer = 0
            For Each zeichen As Char In msgText
                Dim ts As String = zeichen

                If ts = vbCr Then
                    cnt += 1
                End If
            Next

            If cnt < 14 Then
                cnt = 14
            End If
            cnt += 1
            lab.Height = Convert.ToInt32(cnt * Math.Round(hm.Height, 0, MidpointRounding.AwayFromZero))

        End If

        If Not status = ClsMsgs.Status.Standard Then
            lab.Width = frmMSG.Width - icobmp.Width
        Else
            lab.Width = frmMSG.Width - 30
        End If


        'lab.BackColor = Color.Yellow

        'If y = 1 Then
        '    frmMSG.Height = lab.Height * 2
        '    If frmMSG.Height > 100 Then
        '        frmMSG.Height = 200
        '    End If
        'End If


        con1.Panel1.Controls.Add(icobmp)

        If status = ClsMsgs.Status.Standard Then
            con1.Panel1Collapsed = True
        End If

        Dim pan As Panel = New Panel

        pan.Height = lab.Height
        pan.Width = lab.Width

        pan.Controls.Add(lab)

        con1.Panel2.Controls.Add(pan)

        'con1.Panel2.Controls.Add(lab)

        con1.Panel2.AutoScroll = True

        con.Panel1.Controls.Add(con1)
        For Each but As Button In Buttonsl
            con2.Panel2.Controls.Add(but)
        Next
        con.Panel2.Controls.Add(con2)


        frmMSG.Controls.Add(con)

        Dim sd As Integer = con2.Width - 10 - Buttonsl.Item(Buttonsl.Count - 1).Width - Buttonsl.Item(Buttonsl.Count - 1).Location.X


        If sd < 0 Then
            con2.SplitterDistance = 0
        Else
            con2.SplitterDistance = sd
        End If

        con1.SplitterDistance = icowidth * 2

        If t_Time = -1 Then
            For Each but As Button In Buttonsl
                If but.Name = "DefBut" Then
                    but.Focus()
                    but.Select()
                    Exit For
                End If
            Next

            'Buttonsl.Item(0).Focus()
            'Buttonsl.Item(0).Select()
        Else
            lab.Focus()
            lab.Select()
        End If


    End Sub



    Shared Sub frmMSG_closing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs)

        If t_Time > -1 Then
            e.Cancel = True
        Else
            e.Cancel = False
        End If

    End Sub

    Public Shared Function msbox() As CustomDialogResult

        AddHandler frmMSG.FormClosing, AddressOf frmMSG_closing

        frmMSG.ShowDialog()
        Return CustomResult

    End Function


    Private Shared Sub But_click(ByVal sender As Object, ByVal e As EventArgs)

        Dim T_Control As Control = DirectCast(sender, Control)

        CustomResult = DirectCast(T_Control.Tag, CustomDialogResult)

        If Not ButtonTimer Is Nothing Then
            ButtonTimer.Stop()
            ButtonTimer.Enabled = False
            ButtonTimer.Dispose()

            ButtonTimer = Nothing
        End If


        t_Time = -1
        frmMSG.Close()
    End Sub



    Shared Sub CountDownTimerTick(ByVal sender As Object, ByVal e As EventArgs)

        Dim txt As String = DefBut.Text

        Try
            txt = txt.Remove(txt.IndexOf("(")).Trim
        Catch ex As Exception

        End Try

        t_Time -= 1

        If t_Time < 1 Then
            ButtonTimer.Stop()
            ButtonTimer.Enabled = False

            For Each but As Button In Buttonsl
                but.Enabled = True
                but.BackColor = Control.DefaultBackColor
            Next

            For Each but As Button In Buttonsl
                If but.Name = "DefBut" Then
                    but.Focus()
                    but.Select()
                    Exit For
                End If
            Next

            'Buttonsl.Item(0).Focus()
            'Buttonsl.Item(0).Select()

            If t_TimeType = Timer_Type.AutoOK Then

                For Each DefaultButton As Button In Buttonsl

                    If DefaultButton.Name = "DefBut" Then
                        DefaultButton.PerformClick()
                        Exit For
                    End If

                Next

            End If


            Try
                txt = txt.Remove(txt.IndexOf("(")).Trim
            Catch ex As Exception

            End Try

            DefBut.Text = txt
            ButtonTimer = Nothing
        Else
            DefBut.Text = txt + " (" + t_Time.ToString + ")"

        End If


    End Sub

    Public Shared Function TextMessung(ByVal input As String, ByVal font As Font) As SizeF

        Dim BMP As New Drawing.Bitmap(1, 1)
        Dim GFX As Graphics = Graphics.FromImage(BMP)

        GFX.DrawString(input, font, Brushes.Black, New Point(1, 1))
        Dim sf As SizeF = GFX.MeasureString(input, font)

        Return sf

    End Function

    Public Enum CustomDialogResult
        Understood
        OK
        Yes
        No
        Perhaps
        ForAll
        Confirm
        Close
    End Enum

    Public Enum DBList
        OneOnly
        TwoButs
        _YesNo
        Yes_No
        _YesNoCancel
        Yes_NoCancel
        YesNo_Cancel
    End Enum

    Public Enum Status
        Standard
        Information
        Question
        Warning
        Attention
        Erro
    End Enum

    Public Enum Timer_Type
        ButtonEnable
        AutoOK
    End Enum

End Class