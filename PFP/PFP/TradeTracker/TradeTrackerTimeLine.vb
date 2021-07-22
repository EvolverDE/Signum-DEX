
Option Strict On
Imports System.ComponentModel

Public Class TradeTrackerTimeLine

    Inherits System.Windows.Forms.UserControl


    Private Property TL_Zoom As Int64 = -150000000

    Private Property TL_StartDate As Date = Now
    Private Property TL_EndDate As Date = Now
    Private Property TL_TimeStickList As List(Of Date) = New List(Of Date)


    <Description("Wird bei dem angegebenen Intervall entsprechend ausgeführt")>
    Public Event TimerTick(sender As Object)


    <DefaultValue(100.0!), Description("Bereich in Sekunden der in der TimeLine angezeigt werden soll.")>
    Public Property TradeTrackTimerInterval() As Single
        Get
            Return TradeTrackTimer.Interval
        End Get
        Set(ByVal Value As Single)
            If TradeTrackTimer.Interval <> Value Then
                If Value < 10 Then Value = 10
                TradeTrackTimer.Interval = CInt(Value)
                Me.Invalidate()
            End If
        End Set
    End Property

    <DefaultValue(False), Description("Legt fest, ob der timer aktiv sein soll")>
    Public Property TradeTrackTimerEnable() As Boolean
        Get
            Return TradeTrackTimer.Enabled
        End Get
        Set(ByVal Value As Boolean)
            If TradeTrackTimer.Enabled <> Value Then
                TradeTrackTimer.Enabled = Value
                Me.Invalidate()
            End If
        End Set
    End Property

    <DefaultValue(200000), Description("Zoom in n-Fach für den sichtbaren Bereich in der TimeLine")>
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

    <Description("Startdatum der aktuellen Ansicht der TimeLine")>
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

    <Description("Einzelne Zeitpfosten der aktuellen Ansicht der TimeLine")>
    Public ReadOnly Property TimeSticks() As List(Of Date)
        Get
            Return TL_TimeStickList
        End Get
    End Property


    Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(pe)

        Dim GFX_Graphics As System.Drawing.Graphics
        GFX_Graphics = pe.Graphics

        GFX_Graphics.Clear(Color.White)
        Dim GFX_TimeLineY As Single = CSng(Me.Size.Height / 4)
        GFX_Graphics.DrawLine(New Pen(Color.Black, 1), 0, GFX_TimeLineY, Me.Width, GFX_TimeLineY)

        Dim TS As TimeSpan = New TimeSpan
        TS = TL_EndDate - TL_StartDate

        GFX_Graphics.DrawImage(GetViewRangeBMP(TS), New Point(0, 0))

        NuDraw(GFX_Graphics, TS)


        Dim rectBorder As System.Drawing.Rectangle
        rectBorder.X = 0
        rectBorder.Y = 0
        rectBorder.Height = Me.Height - 1
        rectBorder.Width = Me.Width - 1

        GFX_Graphics.DrawRectangle(Pens.Black, rectBorder)

        DoubleBuffered = True

    End Sub



    Private Property Tigger As Ticks = Ticks.Zero


    Function GetViewRangeBMP(ByVal TS As TimeSpan) As Bitmap
        Dim BMP As Bitmap = New Bitmap(100, 100)
        Dim GFX As Graphics = Graphics.FromImage(BMP)
        Dim DFont As Font = New Font("Arial", 10, FontStyle.Bold)

        Select Case TS.TotalMilliseconds / 1000
            Case Is < 1

                Tigger = Ticks.Zero


                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalMilliseconds, 2)) + " Milliseconds"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))

            Case 1 To 60 '1 sec to 1 min

                Select Case TS.TotalMilliseconds / 1000
                    Case 1 To 2
                        Tigger = Ticks.OneSec
                    Case 2 To 5
                        Tigger = Ticks.OneSec
                    Case 5 To 10
                        Tigger = Ticks.OneSec
                    Case 10 To 15
                        Tigger = Ticks.TwoSec
                    Case 15 To 30
                        Tigger = Ticks.FiveSec
                    Case 30 To 60
                        Tigger = Ticks.TenSec
                End Select

                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalSeconds, 2)) + " Seconds"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))

            Case 60 To 60 * 60 '1 min to 1 hour

                Select Case TS.TotalMilliseconds / 1000
                    Case 60 To 60 * 2
                        Tigger = Ticks.FifteenSec
                    Case 60 * 2 To 60 * 5
                        Tigger = Ticks.ThirtySec
                    Case 60 * 5 To 60 * 10
                        Tigger = Ticks.OneMin
                    Case 60 * 10 To 60 * 15
                        Tigger = Ticks.TwoMin
                    Case 60 * 15 To 60 * 30
                        Tigger = Ticks.FiveMin
                    Case 60 * 30 To 60 * 60
                        Tigger = Ticks.TenMin
                End Select



                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalMinutes, 2)) + " Minutes"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))

            Case 60 * 60 To 60 * 60 * 24 '1 hour to 1 day

                Select Case TS.TotalMilliseconds / 1000
                    Case 60 * 60 To 60 * 60 * 2 '1 hour to 2 hour
                        Tigger = Ticks.FifteenMin
                    Case 60 * 60 * 2 To 60 * 60 * 6 '2 hour to 6 hour
                        Tigger = Ticks.ThirtyMin
                    Case 60 * 60 * 6 To 60 * 60 * 12 '6 hour to 12 hour
                        Tigger = Ticks.OneHour
                    Case 60 * 60 * 12 To 60 * 60 * 24 '12 hour to 24 hour
                        Tigger = Ticks.TwoHour
                End Select

                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalHours, 2)) + " Hours"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))


            Case 60 * 60 * 24 To 60 * 60 * 24 * 7 '1 day to 1 week

                Select Case TS.TotalMilliseconds / 1000
                    Case 60 * 60 * 24 To 60 * 60 * 24 * 3 '1 day to 3 day
                        Tigger = Ticks.SixHour
                    Case 60 * 60 * 24 * 3 To 60 * 60 * 24 * 4 '3 day to 4 day
                        Tigger = Ticks.TwelveHour
                    Case 60 * 60 * 24 * 4 To 60 * 60 * 24 * 7  '4 day to 1 week
                        Tigger = Ticks.TwelveHour
                End Select

                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalDays, 2)) + " Days"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))


            Case 60 * 60 * 24 * 7 To 60 * 60 * 24 * 7 * 4 '1 week to 1 month

                Tigger = Ticks.FiveDay

                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalDays, 2)) + " Days"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))

            Case Is > 60 * 60 * 24 * 7 * 4 'over 1 month

                Select Case TS.TotalDays
                    Case 30 To 45
                        Tigger = Ticks.FiveDay
                    Case 45 To 30 * 5
                        Tigger = Ticks.TwoWeek
                    Case 30 * 5 To 30 * 12
                        Tigger = Ticks.OneMonth
                    Case 30 * 12 To 30 * 12 * 6
                        Tigger = Ticks.SixMonth
                    Case 30 * 12 * 6 To 30 * 12 * 10
                        Tigger = Ticks.OneYear
                    Case 30 * 12 * 10 To 30 * 12 * 20
                        Tigger = Ticks.TwoYear
                    Case 30 * 12 * 20 To 30 * 12 * 50
                        Tigger = Ticks.FiveYear
                    Case > 30 * 12 * 50
                        Tigger = Ticks.TenYear

                End Select

                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalDays / 7, 2)) + " Weeks"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))

            Case Else

                Tigger = Ticks.TwoDay

                Dim DString As String = "View Range: " + CStr(Math.Round(TS.TotalDays)) + " error"
                Dim Size As SizeF = GFX.MeasureString(DString, DFont)

                BMP = New Bitmap(CInt(Size.Width), CInt(Size.Height))
                GFX = Graphics.FromImage(BMP)

                GFX.Clear(Color.White)
                GFX.DrawString(DString, DFont, Brushes.Black, New Point(0, 0))

        End Select

        Return BMP

    End Function



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

        If StartEndTimeSpan.TotalMilliseconds = 0.0 Then
            Return 0
        End If

        Dim StartSetTimeSpan As TimeSpan = SetDate - StartDate

        If StartSetTimeSpan.TotalMilliseconds = 0.0 Then
            Return 0
        End If

        Dim EndX As Integer = StartX + EndWidth

        Dim XFactor As Double = EndX / StartEndTimeSpan.TotalMilliseconds
        XFactor *= StartSetTimeSpan.TotalMilliseconds

        Return CInt(XFactor)

    End Function

    Function TimeSpan(ByVal StartDate As Date, ByVal EndDate As Date) As TimeSpan
        Return EndDate - StartDate
    End Function

    Private Sub NuDraw(ByRef GFX_Graphics As Graphics, ByVal TS As TimeSpan)


        Dim TimeMarkPen As System.Drawing.Pen
        TimeMarkPen = New Pen(Color.Black)

        Dim TimeMarkMiddlePen As System.Drawing.Pen
        TimeMarkMiddlePen = New Pen(Color.Red)


        If TL_Zoom < 0 Then
            TL_Zoom *= -1
            TL_StartDate = TL_StartDate.AddMilliseconds(-TL_Zoom)
            TL_EndDate = Now
            TL_Zoom = 3000000
        End If



        Dim YMiddlePoint As Double = 0 + (Me.Height / 3)

        Dim e = CInt(YMiddlePoint * 0.5)
        Dim f = CInt(YMiddlePoint * 1.5)

        Dim fo = New Font("Arial", 10, FontStyle.Bold)


        'GFX_Graphics.DrawString(TL_StartDate.ToLongTimeString, fo, Brushes.Black, 0, 20)
        'GFX_Graphics.DrawString(TL_Zoom.ToString + "/" + TL_EndDate.ToLongTimeString, fo, Brushes.Black, Me.Width - 100, 20)


        Dim Wait As Boolean = GetTimeSticks(Tigger)

        For Each TimeStick As Date In TimeSticks

            Dim SkalaMarkX As Integer = Time2Pixel(0, Me.Width, TL_StartDate, TL_EndDate, TimeStick)

            GFX_Graphics.DrawLine(TimeMarkPen, SkalaMarkX, CInt(e * 2.5), SkalaMarkX, CInt(f / 2))
            GFX_Graphics.DrawString(DateUSToGer(DateUSToGer(TimeStick.ToShortDateString)), fo, Brushes.Black, SkalaMarkX - 25, f)
            GFX_Graphics.DrawString(TimeStick.ToLongTimeString, fo, Brushes.Black, SkalaMarkX - 25, f + 15)

        Next

        Dim SkalaMiddleMarkX As Integer = Time2Pixel(0, Me.Width, TL_StartDate, TL_EndDate, Now)
        GFX_Graphics.DrawLine(TimeMarkMiddlePen, SkalaMiddleMarkX, e, SkalaMiddleMarkX, f)


    End Sub

    Enum Ticks
        Zero = 0

        OneSec = 1
        TwoSec = 2
        FiveSec = 5
        TenSec = 10
        FifteenSec = 15
        ThirtySec = 30

        OneMin = 60
        TwoMin = 120
        FiveMin = 300
        TenMin = 600
        FifteenMin = 900
        ThirtyMin = 1800

        OneHour = 3600
        TwoHour = 7200
        ThreeHour = 10800
        FiveHour = 18000
        SixHour = 21600
        TwelveHour = 43200

        OneDay = 86400
        TwoDay = 172800
        FiveDay = 432000

        OneWeek = 604800
        TwoWeek = 1209600

        OneMonth = 2419200
        ThreeMonth = 7257600
        SixMonth = 14515200

        OneYear = 29030400
        TwoYear = 58060800
        FiveYear = 145152000
        TenYear = 290304000

    End Enum

    Function GetTimeSticks(Optional ByVal RasterTicks As Ticks = Ticks.Zero) As Boolean

        Dim T_TimeWidth_Count As List(Of Double) = GetTimeWidth()
        Dim T_TimeWidth As Double = T_TimeWidth_Count(0)
        Dim T_Cnt As Integer = CInt(T_TimeWidth_Count(1))

        Dim T_Time As Date = SkalaEndDate

        If Not RasterTicks = Ticks.Zero Then
            T_Time = GetCorrectTime(SkalaEndDate, RasterTicks)
        End If



        TimeSticks.Clear()


        If Not T_TimeWidth = 0 Then

            If TimeSticks.Count = 0 Then
                For i As Integer = 0 To 20
                    Select Case RasterTicks
                        Case Ticks.Zero
                            TimeSticks.Add(T_Time.AddMilliseconds(-(i * T_TimeWidth)))

                        Case Ticks.OneSec
                            TimeSticks.Add(T_Time.AddSeconds(-i))
                        Case Ticks.TwoSec
                            TimeSticks.Add(T_Time.AddSeconds(-(i * 2)))
                        Case Ticks.FiveSec
                            TimeSticks.Add(T_Time.AddSeconds(-(i * 5)))
                        Case Ticks.TenSec
                            TimeSticks.Add(T_Time.AddSeconds(-(i * 10)))
                        Case Ticks.FifteenSec
                            TimeSticks.Add(T_Time.AddSeconds(-(i * 15)))
                        Case Ticks.ThirtySec
                            TimeSticks.Add(T_Time.AddSeconds(-(i * 30)))

                        Case Ticks.OneMin
                            TimeSticks.Add(T_Time.AddMinutes(-i))
                        Case Ticks.TwoMin
                            TimeSticks.Add(T_Time.AddMinutes(-(i * 2)))
                        Case Ticks.FiveMin
                            TimeSticks.Add(T_Time.AddMinutes(-(i * 5)))
                        Case Ticks.TenMin
                            TimeSticks.Add(T_Time.AddMinutes(-(i * 10)))
                        Case Ticks.FifteenMin
                            TimeSticks.Add(T_Time.AddMinutes(-(i * 15)))
                        Case Ticks.ThirtyMin
                            TimeSticks.Add(T_Time.AddMinutes(-(i * 30)))

                        Case Ticks.OneHour
                            TimeSticks.Add(T_Time.AddHours(-(i)))
                        Case Ticks.TwoHour
                            TimeSticks.Add(T_Time.AddHours(-(i * 2)))
                        Case Ticks.ThreeHour
                            TimeSticks.Add(T_Time.AddHours(-(i * 3)))
                        Case Ticks.SixHour
                            TimeSticks.Add(T_Time.AddHours(-(i * 6)))
                        Case Ticks.TwelveHour
                            TimeSticks.Add(T_Time.AddHours(-(i * 12)))

                        Case Ticks.OneDay
                            TimeSticks.Add(T_Time.AddDays(-(i)))
                        Case Ticks.TwoDay
                            TimeSticks.Add(T_Time.AddDays(-(i * 2)))
                        Case Ticks.FiveDay
                            TimeSticks.Add(T_Time.AddDays(-(i * 5)))

                        Case Ticks.OneWeek
                            TimeSticks.Add(T_Time.AddDays(-(i * 7)))
                        Case Ticks.TwoWeek
                            TimeSticks.Add(T_Time.AddDays(-(i * 14)))

                        Case Ticks.OneMonth
                            TimeSticks.Add(T_Time.AddMonths(-(i)))
                        Case Ticks.ThreeMonth
                            TimeSticks.Add(T_Time.AddMonths(-(i * 3)))
                        Case Ticks.SixMonth
                            TimeSticks.Add(T_Time.AddMonths(-(i * 6)))

                        Case Ticks.OneYear
                            TimeSticks.Add(T_Time.AddYears(-(i)))
                        Case Ticks.TwoYear
                            TimeSticks.Add(T_Time.AddYears(-(i * 2)))
                        Case Ticks.FiveYear
                            TimeSticks.Add(T_Time.AddYears(-(i * 5)))
                        Case Ticks.TenYear
                            TimeSticks.Add(T_Time.AddYears(-(i * 10)))

                        Case Else
                            TimeSticks.Add(T_Time.AddMilliseconds(-(i * T_TimeWidth)))
                    End Select

                Next

                'Else

                '    ClearTimeSticks()

                '    If Not TimeSticks.Count = T_Cnt Then

                '        Dim FirstTimeStick As Date = TimeSticks(0)
                '        Dim LastTimeStick As Date = TimeSticks(TimeSticks.Count - 1)

                '        Dim NuFirstTimeStick As Date = FirstTimeStick.AddMilliseconds(-T_TimeWidth)
                '        Dim NuLastTimeStick As Date = LastTimeStick.AddMilliseconds(T_TimeWidth)


                '        If NuFirstTimeStick > SkalaStartDate Then
                '            TimeSticks.Insert(0, NuFirstTimeStick)
                '        End If

                '        If NuLastTimeStick < SkalaEndDate Then
                '            TimeSticks.Add(NuLastTimeStick)
                '        End If


                '        TimeSticks.Sort()

                '    End If

            End If

        End If

        Return True

    End Function


    Function GetCorrectTime(ByVal Time As Date, ByVal Tick As Ticks) As Date

        Select Case Tick
            Case Ticks.Zero
                Return Time

            Case Ticks.OneSec
                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":" + Time.Second.ToString)

            Case Ticks.TwoSec

                Dim Second As Integer = Time.Second

                While Second Mod 2 <> 0
                    Second -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":" + Second.ToString)


            Case Ticks.FiveSec

                Dim Second As Integer = Time.Second

                While Second Mod 5 <> 0
                    Second -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":" + Second.ToString)

            Case Ticks.TenSec

                Dim Second As Integer = Time.Second

                While Second Mod 10 <> 0
                    Second -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":" + Second.ToString)

            Case Ticks.FifteenSec

                Dim Second As Integer = Time.Second

                While Second Mod 15 <> 0
                    Second -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":" + Second.ToString)

            Case Ticks.ThirtySec

                Dim Second As Integer = Time.Second

                While Second Mod 30 <> 0
                    Second -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":" + Second.ToString)


            Case Ticks.OneMin
                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Time.Minute.ToString + ":00")
            Case Ticks.TwoMin

                Dim Minute As Integer = Time.Minute

                While Minute Mod 2 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case Ticks.FiveMin

                Dim Minute As Integer = Time.Minute

                While Minute Mod 5 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case Ticks.TenMin

                Dim Minute As Integer = Time.Minute

                While Minute Mod 10 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case Ticks.FifteenMin

                Dim Minute As Integer = Time.Minute

                While Minute Mod 15 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case Ticks.ThirtyMin

                Dim Minute As Integer = Time.Minute

                While Minute Mod 30 <> 0
                    Minute -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":" + Minute.ToString + ":00")

            Case Ticks.OneHour

                Return CDate(Time.ToShortDateString + " " + Time.Hour.ToString + ":00:00")

            Case Ticks.TwoHour

                Dim Hour As Integer = Time.Hour

                While Hour Mod 2 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case Ticks.ThreeHour

                Dim Hour As Integer = Time.Hour

                While Hour Mod 3 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case Ticks.SixHour

                Dim Hour As Integer = Time.Hour

                While Hour Mod 6 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case Ticks.TwelveHour

                Dim Hour As Integer = Time.Hour

                While Hour Mod 12 <> 0
                    Hour -= 1
                End While

                Return CDate(Time.ToShortDateString + " " + Hour.ToString + ":00:00")

            Case Ticks.OneDay
                Return CDate(Time.ToShortDateString + " 00:00:00")

            Case Ticks.TwoDay

                Dim Day As Integer = Time.Day

                While Day Mod 2 <> 0
                    Day -= 1
                End While

                If Day = 0 Then Day = 1

                Return CDate(Day.ToString + "." + Time.Month.ToString + "." + Time.Year.ToString + " 00:00:00")

            Case Ticks.FiveDay

                Dim Day As Integer = Time.Day

                While Day Mod 5 <> 0
                    Day -= 1
                End While

                If Day = 0 Then Day = 1

                Return CDate(Day.ToString + "." + Time.Month.ToString + "." + Time.Year.ToString + " 00:00:00")

            Case Ticks.OneWeek

                Dim Day As Integer = Time.Day

                While Day Mod 7 <> 0
                    Day -= 1
                End While

                If Day = 0 Then Day = 1

                Return CDate(Day.ToString + "." + Time.Month.ToString + "." + Time.Year.ToString + " 00:00:00")

            Case Ticks.TwoWeek

                Dim Day As Integer = Time.Day

                While Day Mod 14 <> 0
                    Day -= 1
                End While

                If Day = 0 Then Day = 1

                Return CDate(Day.ToString + "." + Time.Month.ToString + "." + Time.Year.ToString + " 00:00:00")

            Case Ticks.OneMonth
                Return CDate("01." + Time.Month.ToString + "." + Time.Year.ToString + " 00:00:00")
            Case Ticks.ThreeMonth

                Dim Month As Integer = Time.Month

                While Month Mod 3 <> 0
                    Month -= 1
                End While

                If Month = 0 Then Month = 1

                Return CDate("01." + Month.ToString + "." + Time.Year.ToString + " 00:00:00")

            Case Ticks.SixMonth

                Dim Month As Integer = Time.Month

                While Month Mod 6 <> 0
                    Month -= 1
                End While

                If Month = 0 Then Month = 1

                Return CDate("01." + Month.ToString + "." + Time.Year.ToString + " 00:00:00")

            Case Ticks.OneYear
                Return CDate("01.01." + Time.Year.ToString + " 00:00:00")

            Case Ticks.TwoYear

                Dim Year As Integer = Time.Year

                While Year Mod 2 <> 0
                    Year -= 1
                End While

                If Year = 0 Then Year = 1

                Return CDate("01.01." + Year.ToString + " 00:00:00")

            Case Ticks.FiveYear

                Dim Year As Integer = Time.Year

                While Year Mod 5 <> 0
                    Year -= 1
                End While

                If Year = 0 Then Year = 1

                Return CDate("01.01." + Year.ToString + " 00:00:00")

            Case Ticks.TenYear

                Dim Year As Integer = Time.Year

                While Year Mod 10 <> 0
                    Year -= 1
                End While

                If Year = 0 Then Year = 1

                Return CDate("01.01." + Year.ToString + " 00:00:00")

        End Select



    End Function



    Sub ClearTimeSticks()

        Dim DelIdx As Integer = -1
        For i As Integer = 0 To TimeSticks.Count - 1
            Dim TimeStick As Date = TimeSticks(i)

            If TimeStick < SkalaStartDate Or TimeStick > SkalaEndDate Then
                DelIdx = i
                Exit For
            End If

        Next

        If DelIdx <> -1 Then
            TimeSticks.RemoveAt(DelIdx)
        End If

    End Sub

    Function GetTimeWidth() As List(Of Double)

        Dim TS As TimeSpan = New TimeSpan
        TS = TL_EndDate - TL_StartDate

        Select Case TS.TotalMilliseconds / 1000
            Case Is < 1
                Return {TS.TotalMilliseconds / 2, 2}.ToList
            Case 1 To 60
                Return {TS.TotalMilliseconds / 6, 6}.ToList
            Case 60 To 60 * 60
                Return {TS.TotalMilliseconds / 4, 4}.ToList
            Case 60 * 60 To 60 * 60 * 24
                Return {TS.TotalMilliseconds / 12, 12}.ToList
            Case 60 * 60 * 24 * 7 To 60 * 60 * 24 * 7 * 4
                Return {TS.TotalMilliseconds / 7, 7}.ToList
            Case Is > 60 * 60 * 24 * 7 * 4
                Return {TS.TotalMilliseconds / 4, 4}.ToList
            Case Else
                Return {TS.TotalMilliseconds / 12, 12}.ToList
        End Select

    End Function


    Function GetTimeWidth(ByVal TS As TimeSpan) As List(Of Double)

        Dim MS As Double = TS.TotalMilliseconds

        Select Case MS / 1000
            Case Is < 1

                Dim T_Span As Double = MS / 2

                Return New List(Of Double)({T_Span, T_Span * 2})
            Case 1 To 60

                Dim T_Span As Double = MS / 6
                Dim T_List As List(Of Double) = New List(Of Double)

                For i As Integer = 1 To 6
                    T_List.Add(i * T_Span)
                Next

                Return T_List

            Case 60 To 60 * 60

                Dim T_Span As Double = MS / 4
                Dim T_List As List(Of Double) = New List(Of Double)

                For i As Integer = 1 To 4
                    T_List.Add(i * T_Span)
                Next

                Return T_List

            Case 60 * 60 To 60 * 60 * 24

                Dim T_Span As Double = MS / 12
                Dim T_List As List(Of Double) = New List(Of Double)

                For i As Integer = 1 To 12
                    T_List.Add(i * T_Span)
                Next

                Return T_List

            Case 60 * 60 * 24 * 7 To 60 * 60 * 24 * 7 * 4

                Dim T_Span As Double = MS / 7
                Dim T_List As List(Of Double) = New List(Of Double)

                For i As Integer = 1 To 7
                    T_List.Add(i * T_Span)
                Next

                Return T_List

            Case Is > 60 * 60 * 24 * 7 * 4

                Dim T_Span As Double = MS / 4
                Dim T_List As List(Of Double) = New List(Of Double)

                For i As Integer = 1 To 4
                    T_List.Add(i * T_Span)
                Next

                Return T_List

            Case Else

                Dim T_Span As Double = MS / 12
                Dim T_List As List(Of Double) = New List(Of Double)

                For i As Integer = 1 To 12
                    T_List.Add(i * T_Span)
                Next

                Return T_List

        End Select

    End Function



    Private Sub TradeTrackerTimeLine_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel

        Dim TS As TimeSpan = New TimeSpan
        TS = TL_EndDate - TL_StartDate

        If TS.TotalMilliseconds < 1 Then
            TL_Zoom = 0
        Else
            TL_Zoom = CLng(TS.TotalMilliseconds * 0.02)
        End If

        Select Case e.Delta
            Case 120 'reinzoomen

                TL_StartDate = TL_StartDate.AddMilliseconds(TL_Zoom)
                TL_EndDate = TL_EndDate.AddMilliseconds(-TL_Zoom)


            Case -120 'rauszoomen

                If TL_Zoom < 1 Then
                    TL_Zoom = 50
                End If

                If TL_Zoom > 2000000000000 Then
                    TL_Zoom = 0
                End If

                TL_StartDate = TL_StartDate.AddMilliseconds(-TL_Zoom)
                TL_EndDate = TL_EndDate.AddMilliseconds(TL_Zoom)

        End Select

        Me.Invalidate()

    End Sub

    Private Property MouseDownFlag As Boolean = False

    Dim mouseXrast As Integer = 0


    Private Sub TradeTrackerTimeLine_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown

        If e.Button = MouseButtons.Left Then

            old_Timerinterval = TradeTrackTimer.Interval
            TradeTrackTimer.Interval = 10

            If Not MouseDownFlag Then
                MouseDownFlag = True
                mouseXrast = e.X
            End If

        End If

    End Sub

    Private Sub TradeTrackerTimeLine_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp

        If e.Button = MouseButtons.Left Then
            MouseDownFlag = False
            Try
                TradeTrackTimer.Interval = old_Timerinterval
            Catch ex As Exception

            End Try

        End If

    End Sub

    Private Property XOffset As Integer = 0

    Dim old_Timerinterval As Integer
    Private Sub TradeTrackerTimeLine_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove

        If MouseDownFlag Then

            Dim TX As Double = CDbl(TL_Zoom * 0.15)

            If e.X > mouseXrast Then
                TL_StartDate = TL_StartDate.AddMilliseconds(-(TX))
                TL_EndDate = TL_EndDate.AddMilliseconds(-(TX))
            Else
                TL_StartDate = TL_StartDate.AddMilliseconds((TX * 0.9))
                TL_EndDate = TL_EndDate.AddMilliseconds((TX * 0.9))
            End If

            mouseXrast = e.X

        Else
            TradeTrackTimer.Interval = CInt(TradeTrackTimerInterval)
        End If

    End Sub


    Protected Overridable Sub TradeTrackTimer_Tick(sender As Object, e As EventArgs) Handles TradeTrackTimer.Tick

        RaiseEvent TimerTick(Me.GetType)

        If MouseDownFlag Then

        Else

            TL_StartDate = TL_StartDate.AddMilliseconds(110)
            TL_EndDate = TL_EndDate.AddMilliseconds(110)
        End If

        Me.Invalidate()

    End Sub

End Class
