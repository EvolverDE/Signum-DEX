
Option Strict On
Imports System.ComponentModel

Public Class TradeTrackerTimeLine

    Inherits System.Windows.Forms.UserControl

    'Dim TL_BorderWidth As Single = 1
    Dim TL_Zoom As Int64 = 200000

    Public Shared TL_StartDate As Date = Now
    Public Shared TL_EndDate As Date = Now
    Public Shared TL_Multiplikator As Single = 32768

    Dim Dater As Date = CDate("01.01.2000")
    Dim DaterPlus As Date = Dater

    Dim TimerInterval As Integer

    <Description("Wird bei dem angegebenen Intervall entsprechend ausgeführt")>
    Public Event TimerTick(sender As Object)


    <DefaultValue(100.0!), Description("Bereich in Sekunden der in der TimeLine angezeigt werden soll.")>
    Public Property WorkTrackTimerInterval() As Single
        Get
            Return WorkTrackTimer.Interval
        End Get
        Set(ByVal Value As Single)
            If WorkTrackTimer.Interval <> Value Then
                If Value < 10 Then Value = 10
                WorkTrackTimer.Interval = CInt(Value)
                Me.Invalidate()
            End If
        End Set
    End Property


    <DefaultValue(False), Description("Legt fest, ob der timer aktiv sein soll")>
    Public Property WorkTrackTimerEnable() As Boolean
        Get
            Return WorkTrackTimer.Enabled
        End Get
        Set(ByVal Value As Boolean)
            If WorkTrackTimer.Enabled <> Value Then
                WorkTrackTimer.Enabled = Value
                Me.Invalidate()
            End If
        End Set
    End Property


    <DefaultValue(90000), Description("Zoom in n-Fach für den sichtbaren Bereich in der TimeLine")>
    Public Property Zoom() As Int64
        Get
            Return TL_Zoom
        End Get
        Set(ByVal Value As Int64)
            If TL_Zoom <> Value Then
                TL_Zoom = Value
                Me.Invalidate()
            End If
        End Set
    End Property

    <Description("Enddatum der aktuellen Ansicht der TimeLine")>
    Public ReadOnly Property SkalaStartDate() As Date
        Get
            Return TL_StartDate
        End Get
    End Property


    <Description("Enddatum der aktuellen Ansicht der TimeLine")>
    Public ReadOnly Property SkalaEndDate() As Date
        Get
            Return TL_EndDate
        End Get
    End Property


    Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(pe)
        Try

            Dim GFX_Graphics As System.Drawing.Graphics
            GFX_Graphics = pe.Graphics

            GFX_Graphics.Clear(Color.White)

            'Dim BorderDrawingPen As New System.Drawing.Pen(System.Drawing.Color.Black, TL_BorderWidth)
            'Dim GFX_Font As Font = New Font("Arial", 10, FontStyle.Bold)

            Dim rectBorder As System.Drawing.Rectangle
            rectBorder.X = 0
            rectBorder.Y = 0
            rectBorder.Height = Me.Height - 1
            rectBorder.Width = Me.Width - 1
            Dim GFX_TimeLineY As Single = CSng(Me.Size.Height / 4)


            GFX_Graphics.DrawLine(New Pen(Color.Black, 1), 0, GFX_TimeLineY, Me.Width, GFX_TimeLineY)


            Dim startdat As String = TL_StartDate.ToShortDateString + " " + TL_StartDate.ToLongTimeString
            Dim enddat As String = TL_EndDate.ToShortDateString + " " + TL_EndDate.ToLongTimeString

            Dim daterstr As String = Dater.ToShortDateString + " " + Dater.ToLongTimeString
            Dim daterPstr As String = DaterPlus.ToShortDateString + " " + DaterPlus.ToLongTimeString

            Dim TS As TimeSpan = New TimeSpan

            TS = TL_EndDate - TL_StartDate

            Select Case TS.TotalMilliseconds / 1000
                Case Is < 1
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalMilliseconds, 2)) + " Milliseconds", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
                Case 1 To 60
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalSeconds, 2)) + " Seconds", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
                Case 60 To 60 * 60
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalMinutes, 2)) + " Minutes", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
                Case 60 * 60 To 60 * 60 * 24
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalHours, 2)) + " Hours", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
                Case 60 * 60 * 24 * 7 To 60 * 60 * 24 * 7 * 4
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalDays, 2)) + " Days", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
            'Case 60 * 60 * 24 * 7 * 4 To 60 * 60 * 24 * 7 * 4 * 12
            '    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalDays / 7, 2)) + " Weeks", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))

                Case Is > 60 * 60 * 24 * 7 * 4
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalDays / 7, 2)) + " Weeks", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
                Case Else
                    GFX_Graphics.DrawString("View Range: " + CStr(Math.Round(TS.TotalDays)) + " Days", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))

            End Select

            'GFX_Graphics.DrawString("DBM: " + DoubleBuffered.ToString, GFX_Font, Brushes.Black, 0, 0 + 15)
            'GFX_Graphics.DrawString("EndDate : " + enddat, GFX_Font, Brushes.Black,0, 0 + 30)
            'GFX_Graphics.DrawString("Dater : " + daterstr + " DaterPlus: " + daterPstr, GFX_Font, Brushes.Black, 0, 0 + 45)

            'SuspendLayout()

            NuDraw(GFX_Graphics)

            'ResumeLayout()

            'DrawTimeLine(TL_Modus, GFX_Graphics, TL_BereichSekunden, TL_Offset)
            GFX_Graphics.DrawRectangle(Pens.Black, rectBorder)
            DoubleBuffered = True
            'Select Case ZehnSeks * 10
            '    Case Is < 60
            '        GFX_Graphics.DrawString("Skala: " + CStr(ZehnSeks * 10).ToString + " Sek. Bereich: " + BereichSekunden.ToString + " Sek. Aktualisierung: " + Math.Round(WorkTrackTimer.Interval / 1000, 2).ToString + " Sek.", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
            '    Case 60 To 3599
            '        GFX_Graphics.DrawString("Skala: " + CStr(ZehnSeks * 10 / 60).ToString + " Min. Bereich: " + CStr(Math.Round(BereichSekunden / 60, 2)) + " Min. Aktualisierung: " + Math.Round(WorkTrackTimer.Interval / 1000, 2).ToString + " Sek.", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
            '    Case 3600 To 74700
            '        GFX_Graphics.DrawString("Skala: " + CStr(ZehnSeks * 10 / 60 / 60).ToString + " Std. Bereich: " + CStr(Math.Round(BereichSekunden / 60 / 60, 2)) + " Std. Aktualisierung: " + Math.Round(WorkTrackTimer.Interval / 1000, 2).ToString + " Sek.", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
            '    Case Else
            '        GFX_Graphics.DrawString("Skala: " + CStr(ZehnSeks * 10 / 60 / 60 / 24).ToString + " Tag(e) Bereich: " + CStr(Math.Round(BereichSekunden / 60 / 60 / 24, 2)) + " Tag(e) Aktualisierung: " + Math.Round(WorkTrackTimer.Interval / 1000, 2).ToString + " Sek.", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, New Point(0, 0))
            'End Select

            'BorderDrawingPen.Dispose()
            'GFX_Font.Dispose()


        Catch ex As Exception
            'TBot.StatusLabel3.Text = "TradeTrackerTimeLine: " + ex.Message
        End Try
    End Sub

    Public Shared Function DateUSToGer(datum As String, Optional ByVal PlusTage As Integer = 0) As String

        If PlusTage = 0 Then
            'von YY.MM.DD zu DD.MM.YY (und DD.MM.YY zu YY.MM.DD)
            If datum.Length = 8 Then 'richtung egal da alles 2-stellig ist
                'oMsgBox(datum.Trim.Substring(datum.Length - 2) + "." + datum.Trim.Substring(3, 2) + "." + datum.Trim.Remove(2))
                Return datum.Trim.Substring(datum.Length - 2) + "." + datum.Trim.Substring(3, 2) + "." + datum.Trim.Remove(2)
            End If

            'Umconvert (nur DD.MM.YYYY zu YY.MM.DD)
            If datum.IndexOf(".") = 2 And datum.LastIndexOf(".") = 5 Then
                If datum.Length = 10 Then
                    'oMsgBox(datum.Trim.Substring(datum.Length - 2) + "." + datum.Trim.Substring(3, 2) + "." + datum.Trim.Remove(2))
                    Return datum.Trim.Substring(datum.Length - 2) + "." + datum.Trim.Substring(3, 2) + "." + datum.Trim.Remove(2)
                End If
            End If
        End If

        If PlusTage <> 0 Then
            Dim dat As Date = CDate(datum)
            dat = dat.AddDays(PlusTage)
            datum = DateUSToGer(DateUSToGer(dat.ToShortDateString, 0), 0)
        End If

        Return datum 'wenn die länge nicht passt, das zurückgeben, was die funktion bekommen hat

    End Function

    Public Shared Function Time2Pixel(ByVal StartX As Integer, ByVal EndWidth As Integer, ByVal StartDate As Date, ByVal EndDate As Date, ByVal SetDate As Date) As Integer

        Dim StartEndTimeSpan As TimeSpan = EndDate - StartDate
        Dim StartSetTimeSpan As TimeSpan = SetDate - StartDate

        Dim EndX As Integer = StartX + EndWidth

        Dim XFactor As Double = EndX / StartEndTimeSpan.TotalMilliseconds
        XFactor *= StartSetTimeSpan.TotalMilliseconds
        Try
            Return CInt(XFactor)
        Catch ex As Exception
            Return 0
        End Try


    End Function

    Function TimeSpan(ByVal StartDate As Date, ByVal EndDate As Date) As TimeSpan
        Return EndDate - StartDate
    End Function

    Dim TL_ScaleStartDate As Date = CDate("01.01.2000")
    Dim TL_ScaleEndDate As Date = CDate("01.01.2000")

    Private Sub NuDraw(ByRef GFX_Graphics As Graphics)

        If Dater.ToShortDateString = "01.01.2000" Then
            Dater = Now
        End If

        Dim XFactor As Integer = 1000 'WorkTrackTimer.Interval
        'Select Case XFactor.ToString.Length
        '    Case 2
        '        XFactor *= 100
        '    Case 3
        '        XFactor *= 10
        '    Case Else
        'End Select

        Dim TimeMarkPen As System.Drawing.Pen
        TimeMarkPen = New Pen(Color.Black)

        Dim TimeMarkMiddlePen As System.Drawing.Pen
        TimeMarkMiddlePen = New Pen(Color.Red)

        Dim DatMulti As Single = CSng(CSng(XFactor) * TL_Zoom)

        'Dim a = 0
        'Dim b = Me.Width

        'TL_ScaleStartDate = Now.AddMilliseconds(-DatMulti)
        'TL_ScaleEndDate = Now.AddMilliseconds(DatMulti)

        'DaterPlus = Dater.AddMilliseconds(DatMulti)
        'If DaterPlus < TL_ScaleStartDate Then
        '    Dater = Now
        'End If

        Dim ts = TimeSpan(TL_ScaleStartDate, TL_ScaleEndDate)

        TL_StartDate = TL_ScaleStartDate
        TL_EndDate = TL_ScaleEndDate

        Dim YMiddlePoint As Double = 0 + (Me.Height / 3)

        'YMiddlePoint -= 10

        Dim e = CInt(YMiddlePoint * 0.5)
        Dim f = CInt(YMiddlePoint * 1.5)

        Dim fo = New Font("Arial", 10, FontStyle.Bold)

        Dim LastMarkDate As Date

        For i As Integer = -50 To 50

            Dim Multi As Double = i * XFactor

            Dim z = Dater.AddMilliseconds(Multi * TL_Multiplikator)
            Dim SkalaMarkX As Integer = Time2Pixel(0, Me.Width, TL_ScaleStartDate, TL_ScaleEndDate, z)

            GFX_Graphics.DrawLine(TimeMarkPen, SkalaMarkX, CInt(e * 2.5), SkalaMarkX, CInt(f / 2))
            GFX_Graphics.DrawString(DateUSToGer(DateUSToGer(z.ToShortDateString)), fo, Brushes.Black, SkalaMarkX - 25, f)

            Select Case ts.TotalSeconds
                Case Is < 60 * 10
                    GFX_Graphics.DrawString(z.ToLongTimeString, fo, Brushes.Black, SkalaMarkX - 25, f + 15)
                Case 60 * 10 To 60 * 60 * 24 * 7
                    GFX_Graphics.DrawString(z.ToShortTimeString, fo, Brushes.Black, SkalaMarkX - 15, f + 15)
                Case Else
                    GFX_Graphics.DrawString(z.ToShortTimeString, fo, Brushes.Black, SkalaMarkX - 15, f + 15)
            End Select

            If i = 20 Then
                LastMarkDate = z
            End If

        Next


        If LastMarkDate < Now.AddMilliseconds((TL_ScaleEndDate - Now).TotalMilliseconds) Then
            Dater = Now
        End If


        Dim SkalaMiddleMarkX As Integer = Time2Pixel(0, Me.Width, TL_ScaleStartDate, TL_ScaleEndDate, Now)
        GFX_Graphics.DrawLine(TimeMarkMiddlePen, SkalaMiddleMarkX, e, SkalaMiddleMarkX, f)

        If MouseDownFlag Then

            Dim TimeMarkMiddlePen2 As System.Drawing.Pen
            TimeMarkMiddlePen2 = New Pen(Color.Blue, 3)

            'Dim ts2 As TimeSpan = TL_ScaleEndDate - TL_ScaleStartDate

            Dim SkalaMiddleScrollMarkX As Integer = Time2Pixel(0, Me.Width, TL_ScaleStartDate, TL_ScaleEndDate, DragDropTime)
            GFX_Graphics.DrawLine(TimeMarkMiddlePen2, SkalaMiddleScrollMarkX, e, SkalaMiddleScrollMarkX, f)

        End If


    End Sub

    Dim DragDropTime As Date


    Private Sub WorkTrackerTimeLine_MouseHover(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel

        Dim Multiplikator As Single = 0

        Select Case TL_Zoom
            Case 8 To 60 - 1
                Multiplikator = 1

            Case 60 To 500 - 1
                Multiplikator = 2

            Case 500 To 4000 - 1
                Multiplikator = 3

            Case 4000 To 30000 - 1
                Multiplikator = 4

            Case 30000 To 250000 - 1
                Multiplikator = 5

            Case 250000 To 2000000 - 1
                Multiplikator = 6

            Case 2000000 To 17000000 - 1
                Multiplikator = 7

            Case 17000000 To 130000000 - 1
                Multiplikator = 8

            Case 130000000 To 1000000000
                Multiplikator = 9

            Case > 1000000000
                Multiplikator = 10

        End Select

        If Multiplikator = 0 Then
            Multiplikator = 1
            TL_Multiplikator = 1
        Else
            TL_Multiplikator = CSng(8 ^ Multiplikator)
        End If

        Select Case e.Delta
            Case 120

                TL_Zoom -= CInt((10 ^ Multiplikator) / 10)
                If TL_Zoom < 1 Then
                    TL_Zoom = 1
                End If



                'TL_ScaleStartDate = TL_ScaleStartDate.AddMilliseconds(-(XOffset * TL_Zoom))
                'TL_ScaleEndDate = TL_ScaleEndDate.AddMilliseconds((XOffset * TL_Zoom))

            Case -120

                If Multiplikator = 10 Then

                Else
                    TL_Zoom += CInt((10 ^ Multiplikator) / 10)


                End If

                'Case 240
                '    TL_Zoom -= CSng(10 ^ Multiplikator)
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -240
                '    TL_Zoom += CSng(10 ^ Multiplikator)
                'Case Else

                'Case 360
                '    TL_Zoom -= 100
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -360
                '    TL_Zoom += 100

                'Case 480
                '    TL_Zoom -= 1000
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -480
                '    TL_Zoom += 1000

                'Case 600
                '    TL_Zoom -= 10000
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -600
                '    TL_Zoom += 10000

                'Case 720
                '    TL_Zoom -= 100000
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -720
                '    TL_Zoom += 100000

                'Case 840
                '    TL_Zoom -= 7
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -840
                '    TL_Zoom += 7

                'Case 960
                '    TL_Zoom -= 8
                '    If TL_Zoom < 1 Then TL_Zoom = 1
                'Case -960
                '    TL_Zoom += 8

                'Case 1080
                '    Multiplikator -= 9
                'Case -1080
                '    Multiplikator += 9

        End Select





        Dater = Now
        Me.Invalidate()

    End Sub

    Dim MouseDownFlag As Boolean = False
    Dim Rast1 As Date
    Dim Rast2 As Date

    Dim mouseXrast As Integer = 0
    Dim mouseYrast As Integer = 0


    Private Sub WorkTrackerTimeLine_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown

        If e.Button = MouseButtons.Left Then

            old_Timerinterval = WorkTrackTimer.Interval
            WorkTrackTimer.Interval = 10

            DragDropTime = Now

            If MouseDownFlag Then

            Else

                MouseDownFlag = True
                Rast1 = Now
                Rast2 = Now

                mouseXrast = e.X
                mouseYrast = e.Y

            End If

        End If

    End Sub

    Private Sub WorkTrackerTimeLine_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp

        If e.Button = MouseButtons.Left Then
            MouseDownFlag = False
            Try
                WorkTrackTimer.Interval = old_Timerinterval
            Catch ex As Exception

            End Try

        End If

    End Sub

    Dim XOffset As Integer = 0

    Dim old_Timerinterval As Integer
    Private Sub WorkTrackerTimeLine_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove

        If MouseDownFlag Then

            Dim XFactor As Integer = 1000 'CInt(WorkTrackTimer.Interval)
            'Select Case XFactor.ToString.Length
            '    Case 2
            '        XFactor *= 100
            '    Case 3
            '        XFactor *= 10
            '    Case Else

            'End Select

            If e.X > mouseXrast Then
                XOffset += mouseXrast - e.X
            Else
                XOffset -= e.X - mouseXrast
            End If

            mouseXrast = e.X
            'TL_ScaleStartDate = Rast1.AddMilliseconds(-(XFactor * TL_Zoom) + XDiff)
            'TL_ScaleEndDate = Rast2.AddMilliseconds((XFactor * TL_Zoom) + XDiff)
        Else
            WorkTrackTimer.Interval = CInt(WorkTrackTimerInterval)
        End If

    End Sub


    Protected Overridable Sub WorkTrackTimer_Tick(sender As Object, e As EventArgs) Handles WorkTrackTimer.Tick

        RaiseEvent TimerTick(Me.GetType)

        Dim XFactor As Integer = 1000 'WorkTrackTimer.Interval
        'Select Case XFactor.ToString.Length
        '    Case 2
        '        XFactor *= 100
        '    Case 3
        '        XFactor *= 10
        '    Case Else

        'End Select

        If MouseDownFlag Then

            TL_ScaleStartDate = Rast1.AddMilliseconds(-(XFactor * TL_Zoom))
            TL_ScaleEndDate = Rast2.AddMilliseconds((XFactor * TL_Zoom))

            'TL_ScaleStartDate = TL_ScaleStartDate.AddMilliseconds((XOffset * TL_Zoom))
            'TL_ScaleEndDate = TL_ScaleEndDate.AddMilliseconds((XOffset * TL_Zoom))

        Else

            If TL_ScaleStartDate.ToShortDateString = "01.01.2000" Then TL_ScaleStartDate = Now.AddMilliseconds(-(XFactor * TL_Zoom))
            If TL_ScaleEndDate.ToShortDateString = "01.01.2000" Then TL_ScaleEndDate = Now.AddMilliseconds((XFactor * TL_Zoom))

            TL_ScaleStartDate = Now.AddMilliseconds(-(XFactor * TL_Zoom))
            TL_ScaleEndDate = Now.AddMilliseconds((XFactor * TL_Zoom))

            'TL_ScaleStartDate = TL_ScaleStartDate.AddMilliseconds((XOffset * TL_Zoom))
            'TL_ScaleEndDate = TL_ScaleEndDate.AddMilliseconds((XOffset * TL_Zoom))

        End If

        TL_ScaleStartDate = TL_ScaleStartDate.AddMilliseconds((XOffset * TL_Zoom))
        TL_ScaleEndDate = TL_ScaleEndDate.AddMilliseconds((XOffset * TL_Zoom))

        Me.Invalidate()
    End Sub

End Class
