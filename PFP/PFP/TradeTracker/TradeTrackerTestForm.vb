
'Imports TradeTracker.TradeTrackerTimeRail
'Imports TradeTracker.Graph


Public Class TradeTrackerTestForm

    Dim SplitPanel As SplitContainer = New SplitContainer
    Dim PanelForSplitPanel As Panel = New Panel


    Dim TLS As SplitContainer = New SplitContainer
    Dim WTTL As TradeTrackerTimeLine = New TradeTrackerTimeLine

    Dim WTSList As List(Of TradeTrackerSlot) = New List(Of TradeTrackerSlot)


    Private Sub WorkTrackerTestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SplitPanel.Dock = DockStyle.Fill
        SplitPanel.Orientation = Orientation.Horizontal

        SplitPanel.SplitterDistance = 7


        PanelForSplitPanel.AutoScroll = True
        PanelForSplitPanel.Dock = DockStyle.Fill


        SplitPanel.BorderStyle = BorderStyle.FixedSingle

        TLS.Dock = DockStyle.Top
        TLS.IsSplitterFixed = True
        TLS.BorderStyle = BorderStyle.FixedSingle
        'TLS.SplitterDistance = 32
        TLS.Size = New Size(0, 70)
        'WTTL.Dock = DockStyle.Fill
        WTTL.Height = TLS.Height


        AddHandler WTTL.TimerTick, AddressOf WorkTrackerTimeLine1_TimerTick

        TLS.Panel2.Controls.Add(WTTL)

        For i As Integer = 1 To 3
            Dim WTS As TradeTrackerSlot = New TradeTrackerSlot

            WTS.Location = New Point(0, WTS.Height * (i - 1))
            WTS.LabPair.Text = "Pair" + i.ToString


            WTSList.Add(WTS)
        Next

        SplitPanel.Panel1.Controls.Add(TLS)

        PanelForSplitPanel.Controls.AddRange(WTSList.ToArray)

        SplitPanel.Panel2.Controls.Add(PanelForSplitPanel)

        Me.Controls.Add(SplitPanel)


    End Sub



    Dim ticks As Integer = 0
    Private Sub WorkTrackerTimeLine1_TimerTick(sender As Object)

        If ticks > 100 Then

            For Each wts As TradeTrackerSlot In PanelForSplitPanel.Controls

                Dim GraphList As List(Of Graph) = New List(Of Graph)
                Dim Graph As Graph = New Graph

                Dim XList As List(Of Object) = New List(Of Object)

                Dim BigFont As Font = New Font("Arial", 8, FontStyle.Bold)

                Select Case wts.LabPair.Text.Trim
                    Case "Pair1"

                        'Graph.StartGraph = Now.AddMinutes(-35)
                        'Graph.EndGraph = Now
                        'Graph.GraphArt = Graph.E_GraphArt.EMA3

                        'Graph.MaxValueGraph = 110
                        'Graph.MinValueGraph = 80

                        'XList.Add(New List(Of Object)({Now.AddMinutes(-35), 100}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-30), 90}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-25), 110}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-20), 80}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-15), 100}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-10), 100}))

                        'Graph.GraphValuesList = Graph.DateIntToCandle(XList)



                    Case "Pair2"

                        'Graph.StartGraph = Now.AddMinutes(-35)
                        'Graph.EndGraph = Now
                        'Graph.GraphArt = Graph.E_GraphArt.EMA6

                        'Graph.MaxValueGraph = 50
                        'Graph.MinValueGraph = 10


                        'XList.Add(New List(Of Object)({Now.AddMinutes(-35), 10}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-30), 12}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-25), 18}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-20), 21}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-15), 27}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-10), 50}))



                        'Graph.GraphValuesList = Graph.DateIntToCandle(XList)



                    Case "Pair3"

                        'Graph.StartGraph = Now.AddMinutes(-35)
                        'Graph.EndGraph = Now
                        'Graph.GraphArt = Graph.E_GraphArt.EMA3

                        'Graph.MaxValueGraph = 1000
                        'Graph.MinValueGraph = 200



                        'XList.Add(New List(Of Object)({Now.AddMinutes(-35), 1000}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-30), 988}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-25), 850}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-20), 723}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-15), 536}))
                        'XList.Add(New List(Of Object)({Now.AddMinutes(-10), 200}))



                        'Graph.GraphValuesList = Graph.DateIntToCandle(XList)


                End Select

                'GraphList.Add(Graph)

                'wts.Chart_EMA_Graph = GraphList

            Next

            ticks = 0
        End If
        ticks += 1

        For Each wts As TradeTrackerSlot In PanelForSplitPanel.Controls

            Dim TempSC As SplitContainer = CType(SplitPanel.Panel1.Controls(0), SplitContainer)
            Dim TempTimeLine As TradeTrackerTimeLine = CType(TempSC.Panel2.Controls(0), TradeTrackerTimeLine)

            TLS.SplitterDistance = wts.SplitterDistance
            wts.Width = PanelForSplitPanel.Width - 20

            WTTL.Width = TLS.Panel2.Width - 20


            wts.Chart_EMA_StartDate = TempTimeLine.SkalaStartDate
            wts.Chart_EMA_EndDate = TempTimeLine.SkalaEndDate

        Next

    End Sub


End Class