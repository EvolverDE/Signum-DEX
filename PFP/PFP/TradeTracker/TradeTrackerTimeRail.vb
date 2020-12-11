
'Option Strict On
Imports System.ComponentModel
Imports PFP.Graph

Imports System.Runtime.Serialization

Public Class TradeTrackerTimeRail

    Inherits System.Windows.Forms.UserControl

    Dim TR_StartDate As Date = New Date
    Dim TR_EndDate As Date = New Date



    <Description("Startzeit des Anfangs(x-achse) der TimeLine")>
    Public Property StartDate() As Date
        Get
            Return TR_StartDate
        End Get
        Set(ByVal Value As Date)
            If TR_StartDate <> Value Then
                TR_StartDate = Value
                Invalidater()
            End If
        End Set
    End Property

    <Description("Endzeit der Länge(x-achse) der TimeLine")>
    Public Property EndDate() As Date
        Get
            Return TR_EndDate
        End Get
        Set(ByVal Value As Date)
            If TR_EndDate <> Value Then
                TR_EndDate = Value
                Invalidater()
            End If
        End Set
    End Property


    Private Graphi As Graph = New Graph

    <Description("Die Liste der Graphen")>
    Public Property Graph As Graph
        Get
            Return Graphi
        End Get
        Set(ByVal SetGraphList As Graph)
            Graphi = SetGraphList
            Invalidater()
        End Set
    End Property


    'Sub New()

    '    ' Dieser Aufruf ist für den Designer erforderlich.
    '    InitializeComponent()

    '    ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    '    TR_StartDate = Now.AddDays(0)
    '    TR_EndDate = Now.AddHours(2)


    '    Dim testshift As Shift = New Shift
    '    testshift.StartShift = Now.AddMinutes(10)
    '    testshift.EndShift = testshift.StartShift.AddMinutes(495)

    '    ShiftList.Add(testshift)

    '    Dim testarticle As Article = New Article
    '    testarticle.ArtText = "Lasern: 123-1234-12-01"
    '    testarticle.ArtToolTipText = "ComNr: 123450" + vbCrLf + "Artikel: 123-1234-12-01" + vbCrLf + "Arbgang: Lasern"
    '    testarticle.StartArt = testshift.StartShift.AddMinutes(5)
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(10)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Entgraten: 123-1234-12-02"
    '    testarticle.ArtToolTipText = "ComNr: 123451" + vbCrLf + "Artikel: 123-1234-12-02" + vbCrLf + "Arbgang: Entgraten"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(30)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Kanten: 123-1234-12-03"
    '    testarticle.ArtToolTipText = "ComNr: 123452" + vbCrLf + "Artikel: 123-1234-12-03" + vbCrLf + "Arbgang: Kanten"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(10)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Bohren: 123-1234-12-04"
    '    testarticle.ArtToolTipText = "ComNr: 123453" + vbCrLf + "Artikel: 123-1234-12-04" + vbCrLf + "Arbgang: Bohren"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(5)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Schleifen: 123-1234-12-05"
    '    testarticle.ArtToolTipText = "ComNr: 123454" + vbCrLf + "Artikel: 123-1234-12-05" + vbCrLf + "Arbgang: Schleifen"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(3)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Einpressen: 123-1234-12-06"
    '    testarticle.ArtToolTipText = "ComNr: 123455" + vbCrLf + "Artikel: 123-1234-12-06" + vbCrLf + "Arbgang: Einpressen"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(1)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Einnieten: 123-1234-12-07"
    '    testarticle.ArtToolTipText = "ComNr: 123456" + vbCrLf + "Artikel: 123-1234-12-07" + vbCrLf + "Arbgang: Einnieten"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(1)

    '    ArtList.Add(testarticle)

    '    testarticle.ArtText = "Trowale: 123-1234-12-08"
    '    testarticle.ArtToolTipText = "ComNr: 123457" + vbCrLf + "Artikel: 123-1234-12-08" + vbCrLf + "Arbgang: Trowale"
    '    testarticle.StartArt = testarticle.EndArt
    '    testarticle.EndArt = testarticle.StartArt.AddMinutes(1)

    '    ArtList.Add(testarticle)

    '    Invalidater()

    'End Sub


    Private Sub Invalidater()
        Me.Invalidate()
    End Sub


    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)


        Try

            Dim GFX_Graphics As System.Drawing.Graphics

            GFX_Graphics = e.Graphics

            GFX_Graphics.Clear(Color.White)

            'For Each TempGraph As Graph In Graphi
            'Dim InputList As List(Of Object) = New List(Of Object)({0, 0, Me.Width, Me.Height, Graphi})


            'Dim IPut As List(Of Object) = DirectCast(Input(), List(Of Object))
            'Dim TempGraph As Graph = DirectCast(IPut(IPut.Count - 1), Graph)
            'Dim BMPI As Bitmap = New Bitmap(CInt(IPut(2)), CInt(IPut(3)))
            'Dim GFX_Graphics As Graphics = Graphics.FromImage(BMPI)

            'Dim StartGraph As Date = TempGraph.StartGraph
            'Dim EndGraph As Date = TempGraph.EndGraph


            Dim GraphList As List(Of Graph.S_Graph) = Graphi.GraphValuesList

            If IsNothing(GraphList) Then
                Exit Sub
            End If

            Dim YMiddlePoint As Double = (CInt(Me.Height) / 2)
            Dim YQuaddrPoint As Double = YMiddlePoint / 2


            GFX_Graphics.DrawLine(New Pen(Color.LightGray), 0, 1, Me.Width, 1)
            GFX_Graphics.DrawLine(New Pen(Color.LightGray), 0, CInt(YMiddlePoint), Me.Width, CInt(YMiddlePoint))
            GFX_Graphics.DrawLine(New Pen(Color.LightGray), 0, Me.Height - 2, Me.Width, CInt(Me.Height - 2))

            Dim MinStr As String = "Min-Values: "
            Dim MaxStr As String = "Max-Values: "

            For Each GF As S_Graph In GraphList

                Dim Art As E_GraphArt = GF.GraphArt
                Dim PieceGraph As List(Of S_PieceGraph) = GF.PieceGraphList

                Dim MinValue As Double = GF.MinValue
                Dim MaxValue As Double = GF.MaxValue

                Dim MiddleValue As Double = (MinValue + MaxValue) / 2


                Try
                    Dim Extra As List(Of S_Extra) = GF.Extra

                    If Not IsNothing(Extra) Then
                        For Each Ex As S_Extra In Extra
                            Dim EY As Integer = GraphValue2Pixel(0, CInt(Me.Height) - 3, MinValue, MaxValue, Ex.Ex_Val) + 1
                            GFX_Graphics.DrawLine(New Pen(Ex.Ex_Color, Ex.Ex_Width), 0, EY, Me.Width, EY)

                        Next


                        For Each Ex As S_Extra In Extra

                            Dim EY As Integer = GraphValue2Pixel(0, CInt(Me.Height) - 3, MinValue, MaxValue, Ex.Ex_Val) + 1
                            Dim XX As Integer = 0

                            If Ex.Ex_Color <> Color.Gray Then
                                Dim mess As SizeF = GFX_Graphics.MeasureString(Ex.Ex_Text, New Font("Arial", 8))
                                XX = Me.Width - mess.Width
                            End If

                            GFX_Graphics.DrawString(Ex.Ex_Text, New Font("Arial", 8), New SolidBrush(Ex.Ex_Color), XX, EY)

                        Next

                    End If


                Catch ex As Exception
                    'TBot.StatusLabel3.Text = "TradeTrackerTimeRail/Extra: " + ex.Message
                End Try



                For i As Integer = 0 To PieceGraph.Count - 1

                    Dim Piece As S_PieceGraph = PieceGraph(i)


                    If Double.IsNaN(Piece.CloseValue) Or Double.IsNaN(Piece.OpenValue) Then
                        Continue For
                    End If

                    Dim X As Integer = Time2Pixel(0, CInt(Me.Width), TR_StartDate, TR_EndDate, Piece.OpenDate)
                    Dim X2 As Integer = Time2Pixel(0, CInt(Me.Width), TR_StartDate, TR_EndDate, Piece.CloseDate)

                    Dim Y As Integer = GraphValue2Pixel(0, CInt(Me.Height) - 3, MinValue, MaxValue, Piece.OpenValue) + 1
                    Dim Y2 As Integer = GraphValue2Pixel(0, CInt(Me.Height) - 3, MinValue, MaxValue, Piece.CloseValue) + 1

                    Dim YM As Integer = GraphValue2Pixel(0, CInt(Me.Height) - 3, MinValue, MaxValue, MiddleValue) + 1
                    Dim YZ As Integer = GraphValue2Pixel(0, CInt(Me.Height) - 3, MinValue, MaxValue, 0.0) + 1



                    Select Case Art
                        Case E_GraphArt.Candle

                            If Y2 > Y Then
                                GFX_Graphics.FillRectangle(Brushes.Crimson, X + 1, Y, X2 - X - 1, Y2 - Y + 1)

                                If i = PieceGraph.Count - 1 Then
                                    GFX_Graphics.DrawString(String.Format("{0:#0.00000000}", Piece.CloseValue), New Font("Arial", 8), Brushes.Crimson, X2 + 5, Y)
                                End If

                            Else
                                GFX_Graphics.FillRectangle(Brushes.Green, X + 1, Y2, X2 - X - 1, Y - Y2 + 1)

                                If i = PieceGraph.Count - 1 Then
                                    GFX_Graphics.DrawString(String.Format("{0:#0.00000000}", Piece.CloseValue), New Font("Arial", 8), Brushes.Green, X2 + 5, Y)
                                End If

                            End If

                            If i = 0 Then
                                MaxStr += "Candles: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "Candles: " + String.Format("{0:#0.00000000}", MinValue) + " "
                            End If


                        Case E_GraphArt.Line
                            GFX_Graphics.DrawLine(New Pen(Color.Black), X, Y, X2, Y2)

                            If i = PieceGraph.Count - 1 Then
                                GFX_Graphics.DrawString(String.Format("{0:#0.00000000}", Piece.CloseValue), New Font("Arial", 8), Brushes.Black, X2 + 5, Y)
                            End If

                            If i = 0 Then
                                MaxStr += "Line: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "Line: " + String.Format("{0:#0.00000000}", MinValue) + " "

                            End If


                        Case E_GraphArt.EMAFast
                            GFX_Graphics.DrawLine(New Pen(Color.LimeGreen), X, Y, X2, Y2)

                            If i = 0 Then
                                MaxStr += "EMA1: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "EMA1: " + String.Format("{0:#0.00000000}", MinValue) + " "

                            End If


                        Case E_GraphArt.EMASlow
                            GFX_Graphics.DrawLine(New Pen(Color.Blue), X, Y, X2, Y2)

                            If i = 0 Then
                                MaxStr += "EMA2: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "EMA2: " + String.Format("{0:#0.00000000}", MinValue) + " "

                            End If


                        Case E_GraphArt.MACD
                            'GFX_Graphics.DrawLine(New Pen(Color.SkyBlue), 0, CInt(YMiddlePoint) - CInt(YQuaddrPoint), Me.Width, CInt(YMiddlePoint) - CInt(YQuaddrPoint))
                            'GFX_Graphics.DrawLine(New Pen(Color.SkyBlue), 0, CInt(YMiddlePoint) + CInt(YQuaddrPoint), Me.Width, CInt(YMiddlePoint) + CInt(YQuaddrPoint))
                            GFX_Graphics.DrawLine(New Pen(Color.DarkSlateBlue), X, Y, X2, Y2)

                            If i = 0 Then
                                MaxStr += "MACD: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "MACD: " + String.Format("{0:#0.00000000}", MinValue) + " "

                            End If


                        Case E_GraphArt.RSI
                            'GFX_Graphics.DrawLine(New Pen(Color.SkyBlue), 0, CInt(YMiddlePoint) - CInt(YQuaddrPoint), Me.Width, CInt(YMiddlePoint) - CInt(YQuaddrPoint))
                            'GFX_Graphics.DrawLine(New Pen(Color.SkyBlue), 0, CInt(YMiddlePoint) + CInt(YQuaddrPoint), Me.Width, CInt(YMiddlePoint) + CInt(YQuaddrPoint))
                            GFX_Graphics.DrawLine(New Pen(Color.Black), X, Y, X2, Y2)

                            If i = 0 Then
                                MaxStr += "RSI: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "RSI: " + String.Format("{0:#0.00000000}", MinValue) + " "

                            End If

                        Case E_GraphArt.Signal
                            GFX_Graphics.DrawLine(New Pen(Color.Red), X, Y, X2, Y2)

                            If i = 0 Then
                                MaxStr += "Signal: " + String.Format("{0:#0.00000000}", MaxValue) + " "
                                MinStr += "Signal: " + String.Format("{0:#0.00000000}", MinValue) + " "

                            End If

                        Case E_GraphArt.Sticks

                            Dim Middle As Integer = (Y + Y2) / 2

                            If Middle > YZ Then
                                GFX_Graphics.FillRectangle(Brushes.Crimson, X + 1, YZ, X2 - X, Middle - YZ) 'CInt(CInt(Me.Height) / 2)
                            Else
                                GFX_Graphics.FillRectangle(Brushes.Green, X + 1, Middle, X2 - X, YZ - Middle) '(CInt(Me.Height) / 2)
                            End If

                            If i = 0 Then
                                MaxStr += "Histogram: " + String.Format("{0:#0.0000000000}", MaxValue) + " "
                                MinStr += "Histogram: " + String.Format("{0:#0.0000000000}", MinValue) + " "

                            End If


                        Case Else

                    End Select

                Next

                GFX_Graphics.DrawString(MinStr, New Font("Arial", 8), Brushes.Magenta, 0, Me.Height - 14)
                GFX_Graphics.DrawString(MaxStr, New Font("Arial", 8), Brushes.Magenta, 0, 0)

            Next


            'GFX_Graphics.Dispose()


            'Dim Threader As Threading.Thread = New Threading.Thread(AddressOf GraphDraw)
            'ThreadPool.Add(Threader)
            'ThreadPool.Item(ThreadPool.Count - 1).Start(InputList)

            ''GraphDraw(GFX_Graphics, TempGraph)
            ''Next

            ''auf letzten thread warten...
            'Dim thexit = True
            'While thexit
            '    thexit = False
            '    For Each threa In ThreadPool
            '        If threa.IsAlive Then
            '            thexit = True
            '        End If
            '    Next
            'End While


            ''...bevor gezeichnet wird
            'For s As Integer = 0 To BMPList.Count - 1
            '    Dim BMPItem As Bitmap = BMPList(s)
            '    GFX_Graphics.DrawImage(BMPItem, 0, 0)
            'Next

            DoubleBuffered = True

        Catch ex As Exception
            'TBot.StatusLabel3.Text = "TradeTrackerTimeRail: " + ex.Message

        End Try

    End Sub

    Private Sub GraphDraw(input)

        Dim IPut As List(Of Object) = DirectCast(input, List(Of Object))
        Dim TempGraph As Graph = DirectCast(IPut(IPut.Count - 1), Graph)
        Dim BMPI As Bitmap = New Bitmap(CInt(IPut(2)), CInt(IPut(3)))
        Dim GFX_Graphics As Graphics = Graphics.FromImage(BMPI)

        'Dim StartGraph As Date = TempGraph.StartGraph
        'Dim EndGraph As Date = TempGraph.EndGraph


        Dim GraphList As List(Of Graph.S_Graph) = TempGraph.GraphValuesList

        Dim YMiddlePoint As Double = (CInt(IPut(3)) / 2)

        GFX_Graphics.DrawLine(New Pen(Color.Gray), 0, 1, CInt(IPut(2)), 1)
        GFX_Graphics.DrawLine(New Pen(Color.Gray), 0, CInt(YMiddlePoint), CInt(IPut(2)), CInt(YMiddlePoint))
        GFX_Graphics.DrawLine(New Pen(Color.Gray), 0, CInt(IPut(3)) - 2, CInt(IPut(2)), CInt(IPut(3) - 2))


        For Each GF As S_Graph In GraphList

            Dim Art As E_GraphArt = GF.GraphArt
            Dim PieceGraph As List(Of S_PieceGraph) = GF.PieceGraphList

            Dim MinValue As Double = GF.MinValue
            Dim MaxValue As Double = GF.MaxValue

            For Each Piece In PieceGraph

                Dim X As Integer = Time2Pixel(0, CInt(IPut(2)), TR_StartDate, TR_EndDate, Piece.OpenDate)
                Dim X2 As Integer = Time2Pixel(0, CInt(IPut(2)), TR_StartDate, TR_EndDate, Piece.CloseDate)

                Dim Y As Integer = GraphValue2Pixel(0, CInt(IPut(3)) - 3, MinValue, MaxValue, Piece.OpenValue) + 3
                Dim Y2 As Integer = GraphValue2Pixel(0, CInt(IPut(3)) - 3, MinValue, MaxValue, Piece.CloseValue) + 3

                Select Case Art
                    Case E_GraphArt.Candle
                        If Y2 > Y Then
                            GFX_Graphics.FillRectangle(Brushes.Red, X + 1, Y, X2 - X - 1, Y2 - Y + 1)
                        Else
                            GFX_Graphics.FillRectangle(Brushes.Green, X + 1, Y2, X2 - X - 1, Y - Y2 + 1)
                        End If
                    Case E_GraphArt.EMAFast
                        GFX_Graphics.DrawLine(New Pen(Color.Magenta), X, Y, X2, Y2)
                    Case E_GraphArt.EMASlow
                        GFX_Graphics.DrawLine(New Pen(Color.Blue), X, Y, X2, Y2)
                    Case E_GraphArt.MACD
                        GFX_Graphics.DrawLine(New Pen(Color.Green), X, Y, X2, Y2)
                    Case E_GraphArt.RSI
                        GFX_Graphics.DrawLine(New Pen(Color.Gray), X, Y, X2, Y2)
                    Case Else

                End Select

            Next

        Next


        'For Each Vali As S_PieceGraph In Valuelist

        '    If Not MaxValue / MinValue > 0.5 Then
        '        Continue For
        '    End If

        '    Dim X As Integer = Time2Pixel(0, CInt(IPut(2)), TR_StartDate, TR_EndDate, Vali.OpenDate)
        '    Dim X2 As Integer = Time2Pixel(0, CInt(IPut(2)), TR_StartDate, TR_EndDate, Vali.CloseDate)

        '    Dim Y As Integer = GraphValue2Pixel(0, CInt(IPut(3)) - 3, MinValue, MaxValue, Vali.OpenValue) + 3
        '    Dim Y2 As Integer = GraphValue2Pixel(0, CInt(IPut(3)) - 3, MinValue, MaxValue, Vali.CloseValue) + 3

        '    If Y2 > Y Then
        '        GFX_Graphics.FillRectangle(Brushes.Red, X + 1, Y, X2 - X - 1, Y2 - Y + 1)
        '    Else
        '        GFX_Graphics.FillRectangle(Brushes.Green, X + 1, Y2, X2 - X - 1, Y - Y2 + 1)
        '    End If

        'Next


        'For Each EMAItem As S_Graph In GraphList

        '    Dim EMAArt As E_GraphArt = EMAItem.GraphArt
        '    Dim EMAL As List(Of S_PieceGraph) = EMAItem.PieceGraphList

        '    Dim EMAPen As Pen = New Pen(Color.Blue)

        '    Select Case EMAArt
        '        Case E_GraphArt.Candle

        '        Case E_GraphArt.EMA12
        '            EMAPen = New Pen(Color.Magenta)
        '        Case E_GraphArt.EMA26
        '            EMAPen = New Pen(Color.Green)
        '        Case E_GraphArt.MACD
        '            EMAPen = New Pen(Color.Red)
        '        Case Else
        '    End Select


        '    For Each EMAx As S_PieceGraph In EMAL

        '        If Not MaxValue / MinValue > 0.5 Then
        '            Continue For
        '        End If

        '        Dim X As Integer = Time2Pixel(0, CInt(IPut(2)), TR_StartDate, TR_EndDate, EMAx.OpenDate)
        '        Dim X2 As Integer = Time2Pixel(0, CInt(IPut(2)), TR_StartDate, TR_EndDate, EMAx.CloseDate)

        '        Dim Y As Integer = GraphValue2Pixel(0, CInt(IPut(3)) - 3, MinValue, MaxValue, EMAx.OpenValue) + 3
        '        Dim Y2 As Integer = GraphValue2Pixel(0, CInt(IPut(3)) - 3, MinValue, MaxValue, EMAx.CloseValue) + 3

        '        GFX_Graphics.DrawLine(EMAPen, X, Y, X2, Y2)

        '    Next

        'Next


        'Dim TotalMin As TimeSpan = EndGraph - StartGraph

        'BMPList.Add(BMPI)

    End Sub

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


    Public Shared Function GraphValue2Pixel(ByVal StartY As Integer, ByVal EndHeight As Integer, ByVal GraphMin As Double, ByVal GraphMax As Double, ByVal SetGraph As Double) As Integer

        If GraphMax = 0 And GraphMin = 0 Then
            Return 0
        End If

        Dim SatScala As Double = GraphMax - GraphMin

        If SatScala = 0 Then
            Return 0
        End If

        Dim PixelScala As Integer = EndHeight - StartY

        Dim OneSat As Double = PixelScala / SatScala

        Dim x As Double = GraphMax - SetGraph

        x *= OneSat

        Try
            Return CInt(x)
        Catch ex As Exception
            Return 0
        End Try

    End Function



End Class
