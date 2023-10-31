Option Strict On
Option Explicit On

Public Class Graph

    'Implements IDisposable

    Private StartDate As Date
    Private EndDate As Date
    'Private MinValue As Double
    'Private MaxValue As Double

    Private GraphList As List(Of S_Graph)

    Enum E_GraphArt
        Candle = 1
        EMAFast = 2
        EMASlow = 3
        MACD = 4
        RSI = 5
        Signal = 6
        Line = 7
        Sticks = 8
        Volume = 9
    End Enum

    Public Structure S_PieceGraph
        Dim OpenDate As Date
        Dim CloseDate As Date

        Dim OpenValue As Double
        Dim CloseValue As Double

        Dim MinValue As Double
        Dim MaxValue As Double

        Dim Volume As Double
    End Structure

    Public Structure S_Graph
        Dim GraphArt As E_GraphArt
        Dim MinValue As Double
        Dim MaxValue As Double

        Dim PieceGraphList As List(Of S_PieceGraph)
        Dim Extra As List(Of S_Extra)
    End Structure


    Public Structure S_Extra
        Dim Ex_Text As String
        Dim Ex_Val As Double
        Dim Ex_Width As Integer
        Dim Ex_Color As Color
    End Structure


    '<Description("Startzeit des Graphen")>
    Public Property StartGraph As Date
        Get
            Return StartDate
        End Get
        Set(ByVal Dat As Date)
            StartDate = Dat
        End Set
    End Property


    '<Description("Endzeit des Graphen")>
    Public Property EndGraph As Date
        Get
            Return EndDate
        End Get
        Set(ByVal Dat As Date)
            EndDate = Dat
        End Set
    End Property


    ''<Description("Minimalwert des Graphen")>
    'Public Property MinValueGraph As Double
    '    Get
    '        Return MinValue
    '    End Get
    '    Set(ByVal SetMinValue As Double)
    '        MinValue = SetMinValue
    '    End Set
    'End Property


    ''<Description("Maximalwert des Graphen")>
    'Public Property MaxValueGraph As Double
    '    Get
    '        Return MaxValue
    '    End Get
    '    Set(ByVal SetMaxValue As Double)
    '        MaxValue = SetMaxValue
    '    End Set
    'End Property


    '<Description("WerteListe des Graphen")>
    Public Property GraphValuesList As List(Of S_Graph)
        Get
            Return GraphList
        End Get
        Set(ByVal SetValueList As List(Of S_Graph))
            GraphList = SetValueList
        End Set
    End Property

    '#Region "IDisposable Support"
    '    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    '    ' IDisposable
    '    Protected Overridable Sub Dispose(disposing As Boolean)
    '        If Not disposedValue Then
    '            If disposing Then
    '                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.

    '                StartDate = Nothing
    '                EndDate = Nothing
    '                GraphList = Nothing

    '            End If

    '            StartDate = Nothing
    '            EndDate = Nothing
    '            GraphList = Nothing

    '            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
    '            ' TODO: große Felder auf Null setzen.
    '        End If
    '        disposedValue = True
    '    End Sub

    '    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    '    'Protected Overrides Sub Finalize()
    '    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    '    Dispose(False)
    '    '    MyBase.Finalize()
    '    'End Sub

    '    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    '    Public Sub Dispose() Implements IDisposable.Dispose
    '        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '        Dispose(True)
    '        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
    '        ' GC.SuppressFinalize(Me)
    '    End Sub
    '#End Region

End Class
