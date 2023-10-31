
Option Strict On
Option Explicit On

Imports System.ComponentModel

Public Class TradeTrackerSlot

    Inherits System.Windows.Forms.UserControl

    Public Property TTTL As TradeTrackerTimeLine

    Public Property Pair As String

    Property Chart_EMA_TR As TradeTrackerTimeRail = New TradeTrackerTimeRail
    Property MACD_RSI_TR As TradeTrackerTimeRail = New TradeTrackerTimeRail

    <Description("Wird bei einem click auf einer CheckBox ausgeführt")>
    Public Event ChBxXChange(sender As Object)

    <Description("SplitterDistanz laden oder ändern")>
    Public Property SplitterDistance() As Integer
        Get
            Return SlotSplitter.SplitterDistance
        End Get
        Set(ByVal Value As Integer)
            If SlotSplitter.SplitterDistance <> Value Then
                SlotSplitter.SplitterDistance = Value
                Me.Invalidate()
            End If
        End Set
    End Property

    <Description("Startzeit des Anfangs(x-achse) der TimeLine")>
    Public Property Chart_EMA_StartDate() As Date
        Get
            Return Chart_EMA_TR.StartDate
        End Get
        Set(ByVal Value As Date)
            If Chart_EMA_TR.StartDate <> Value Then
                Chart_EMA_TR.StartDate = Value
            End If
        End Set
    End Property

    <Description("Endzeit der Länge(x-achse) der TimeLine")>
    Public Property Chart_EMA_EndDate() As Date
        Get
            Return Chart_EMA_TR.EndDate
        End Get
        Set(ByVal Value As Date)
            If Chart_EMA_TR.EndDate <> Value Then
                Chart_EMA_TR.EndDate = Value
            End If
        End Set
    End Property

    <Description("Die Liste der Chart/EMA-Graphen in der TimeRail")>
    Public Property Chart_EMA_Graph As Graph
        Get
            Return Chart_EMA_TR.Graph
        End Get
        Set(ByVal SetGraphlist As Graph)
            Chart_EMA_TR.Graph = SetGraphlist
        End Set
    End Property

    <Description("Startzeit des Anfangs(x-achse) der TimeLine")>
    Public Property MACD_RSI_TR_StartDate() As Date
        Get
            Return MACD_RSI_TR.StartDate
        End Get
        Set(ByVal Value As Date)
            If MACD_RSI_TR.StartDate <> Value Then
                MACD_RSI_TR.StartDate = Value
            End If
        End Set
    End Property

    <Description("Endzeit der Länge(x-achse) der TimeLine")>
    Public Property MACD_RSI_TR_EndDate() As Date
        Get
            Return MACD_RSI_TR.EndDate
        End Get
        Set(ByVal Value As Date)
            If MACD_RSI_TR.EndDate <> Value Then
                MACD_RSI_TR.EndDate = Value
            End If
        End Set
    End Property

    <Description("Die Liste der MACD/RSI-Graphen in der TimeRail")>
    Public Property MACD_RSI_TR_GraphList As Graph
        Get
            Return MACD_RSI_TR.Graph
        End Get
        Set(ByVal SetGraphlist As Graph)
            MACD_RSI_TR.Graph = SetGraphlist
        End Set
    End Property


    Private Sub TradeTrackerSlot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Chart_EMA_TR.Dock = DockStyle.Fill
        MACD_RSI_TR.Dock = DockStyle.Fill

        Chart_EMA_TR.TTTL = TTTL
        MACD_RSI_TR.TTTL = TTTL

        Dim SC1 As SplitContainer = DirectCast(SlotSplitter.Panel2.Controls(0), SplitContainer)
        'SC1.SplitterDistance = Integer.Parse(SC1.Height / 2) + Integer.Parse(SC1.Height / 8)
        SC1.Panel1.Controls.Add(Chart_EMA_TR)
        SC1.Panel2.Controls.Add(MACD_RSI_TR)

        Me.Invalidate()
    End Sub


    Protected Overridable Sub ChBxX_CheckedChanged(sender As Object, e As EventArgs) Handles ChBxCandles.CheckedChanged, ChBxEMA1.CheckedChanged, ChBxEMA2.CheckedChanged, ChBxLine.CheckedChanged, ChBxMACD.CheckedChanged, ChBxMACDSig.CheckedChanged, ChBxRSI.CheckedChanged, ChBxMACDHis.CheckedChanged
        RaiseEvent ChBxXChange(Me.GetType)
    End Sub

    Public Sub Refresher()
        Chart_EMA_TR.Pair = Pair
        MACD_RSI_TR.Pair = Pair
    End Sub


End Class
