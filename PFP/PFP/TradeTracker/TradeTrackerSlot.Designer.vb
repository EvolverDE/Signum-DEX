<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TradeTrackerSlot
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SlotSplitter = New System.Windows.Forms.SplitContainer()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.TBRSICandles = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TBRSIULine = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TBRSIOLine = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TBMACDSig = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TBMACDULine = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TBMACDOLine = New System.Windows.Forms.TextBox()
        Me.ChBxMACDZero = New System.Windows.Forms.CheckBox()
        Me.TBMACDEMA1 = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TBMACDEMA2 = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.ChBxMACDHis = New System.Windows.Forms.CheckBox()
        Me.ChBxLine = New System.Windows.Forms.CheckBox()
        Me.ChBxCandles = New System.Windows.Forms.CheckBox()
        Me.ChBxMACDSig = New System.Windows.Forms.CheckBox()
        Me.ChBxEMA2 = New System.Windows.Forms.CheckBox()
        Me.ChBxEMA1 = New System.Windows.Forms.CheckBox()
        Me.ChBxRSI = New System.Windows.Forms.CheckBox()
        Me.ChBxMACD = New System.Windows.Forms.CheckBox()
        Me.LabMIN = New System.Windows.Forms.Label()
        Me.LabLast = New System.Windows.Forms.Label()
        Me.LabMAX = New System.Windows.Forms.Label()
        Me.LabPair = New System.Windows.Forms.Label()
        Me.LabExch = New System.Windows.Forms.Label()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        CType(Me.SlotSplitter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SlotSplitter.Panel1.SuspendLayout()
        Me.SlotSplitter.Panel2.SuspendLayout()
        Me.SlotSplitter.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SlotSplitter
        '
        Me.SlotSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SlotSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SlotSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SlotSplitter.IsSplitterFixed = True
        Me.SlotSplitter.Location = New System.Drawing.Point(0, 0)
        Me.SlotSplitter.Name = "SlotSplitter"
        '
        'SlotSplitter.Panel1
        '
        Me.SlotSplitter.Panel1.Controls.Add(Me.GroupBox3)
        Me.SlotSplitter.Panel1.Controls.Add(Me.GroupBox2)
        Me.SlotSplitter.Panel1.Controls.Add(Me.GroupBox1)
        Me.SlotSplitter.Panel1.Controls.Add(Me.LabMIN)
        Me.SlotSplitter.Panel1.Controls.Add(Me.LabLast)
        Me.SlotSplitter.Panel1.Controls.Add(Me.LabMAX)
        Me.SlotSplitter.Panel1.Controls.Add(Me.LabPair)
        Me.SlotSplitter.Panel1.Controls.Add(Me.LabExch)
        '
        'SlotSplitter.Panel2
        '
        Me.SlotSplitter.Panel2.Controls.Add(Me.SplitContainer1)
        Me.SlotSplitter.Size = New System.Drawing.Size(1000, 371)
        Me.SlotSplitter.SplitterDistance = 230
        Me.SlotSplitter.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label15)
        Me.GroupBox3.Controls.Add(Me.Label16)
        Me.GroupBox3.Controls.Add(Me.TBRSICandles)
        Me.GroupBox3.Controls.Add(Me.Label13)
        Me.GroupBox3.Controls.Add(Me.TBRSIULine)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(Me.TBRSIOLine)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox3.Enabled = False
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Location = New System.Drawing.Point(0, 285)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(228, 81)
        Me.GroupBox3.TabIndex = 6
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "RSI config"
        Me.GroupBox3.Visible = False
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(116, 60)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(16, 13)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "%"
        Me.Label15.Visible = False
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(116, 39)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(16, 13)
        Me.Label16.TabIndex = 14
        Me.Label16.Text = "%"
        Me.Label16.Visible = False
        '
        'TBRSICandles
        '
        Me.TBRSICandles.Location = New System.Drawing.Point(57, 16)
        Me.TBRSICandles.Margin = New System.Windows.Forms.Padding(2)
        Me.TBRSICandles.Name = "TBRSICandles"
        Me.TBRSICandles.Size = New System.Drawing.Size(54, 20)
        Me.TBRSICandles.TabIndex = 6
        Me.TBRSICandles.Text = "14"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(2, 18)
        Me.Label13.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(56, 13)
        Me.Label13.TabIndex = 5
        Me.Label13.Text = "Candles:"
        '
        'TBRSIULine
        '
        Me.TBRSIULine.Location = New System.Drawing.Point(57, 37)
        Me.TBRSIULine.Margin = New System.Windows.Forms.Padding(2)
        Me.TBRSIULine.Name = "TBRSIULine"
        Me.TBRSIULine.Size = New System.Drawing.Size(54, 20)
        Me.TBRSIULine.TabIndex = 4
        Me.TBRSIULine.Text = "30"
        Me.TBRSIULine.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(2, 60)
        Me.Label9.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(56, 13)
        Me.Label9.TabIndex = 3
        Me.Label9.Text = "O.thres.:"
        Me.Label9.Visible = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(2, 39)
        Me.Label10.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(56, 13)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "U.thres.:"
        Me.Label10.Visible = False
        '
        'TBRSIOLine
        '
        Me.TBRSIOLine.Location = New System.Drawing.Point(57, 58)
        Me.TBRSIOLine.Margin = New System.Windows.Forms.Padding(2)
        Me.TBRSIOLine.Name = "TBRSIOLine"
        Me.TBRSIOLine.Size = New System.Drawing.Size(54, 20)
        Me.TBRSIOLine.TabIndex = 2
        Me.TBRSIOLine.Text = "70"
        Me.TBRSIOLine.Visible = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label14)
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.TBMACDSig)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.TBMACDULine)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.TBMACDOLine)
        Me.GroupBox2.Controls.Add(Me.ChBxMACDZero)
        Me.GroupBox2.Controls.Add(Me.TBMACDEMA1)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.TBMACDEMA2)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox2.Enabled = False
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Location = New System.Drawing.Point(0, 167)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Size = New System.Drawing.Size(228, 118)
        Me.GroupBox2.TabIndex = 5
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "MACD config (in Days)"
        Me.GroupBox2.Visible = False
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(116, 96)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(16, 13)
        Me.Label14.TabIndex = 13
        Me.Label14.Text = "%"
        Me.Label14.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(116, 75)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(16, 13)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "%"
        Me.Label8.Visible = False
        '
        'TBMACDSig
        '
        Me.TBMACDSig.Location = New System.Drawing.Point(118, 36)
        Me.TBMACDSig.Margin = New System.Windows.Forms.Padding(2)
        Me.TBMACDSig.Name = "TBMACDSig"
        Me.TBMACDSig.Size = New System.Drawing.Size(56, 20)
        Me.TBMACDSig.TabIndex = 11
        Me.TBMACDSig.Text = "9"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(115, 18)
        Me.Label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(46, 13)
        Me.Label7.TabIndex = 10
        Me.Label7.Text = "Signal:"
        '
        'TBMACDULine
        '
        Me.TBMACDULine.Location = New System.Drawing.Point(57, 73)
        Me.TBMACDULine.Margin = New System.Windows.Forms.Padding(2)
        Me.TBMACDULine.Name = "TBMACDULine"
        Me.TBMACDULine.Size = New System.Drawing.Size(54, 20)
        Me.TBMACDULine.TabIndex = 9
        Me.TBMACDULine.Text = "30"
        Me.TBMACDULine.Visible = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(2, 96)
        Me.Label11.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(56, 13)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "O.thres.:"
        Me.Label11.Visible = False
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(2, 75)
        Me.Label12.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(56, 13)
        Me.Label12.TabIndex = 6
        Me.Label12.Text = "U.thres.:"
        Me.Label12.Visible = False
        '
        'TBMACDOLine
        '
        Me.TBMACDOLine.Location = New System.Drawing.Point(57, 94)
        Me.TBMACDOLine.Margin = New System.Windows.Forms.Padding(2)
        Me.TBMACDOLine.Name = "TBMACDOLine"
        Me.TBMACDOLine.Size = New System.Drawing.Size(54, 20)
        Me.TBMACDOLine.TabIndex = 7
        Me.TBMACDOLine.Text = "70"
        Me.TBMACDOLine.Visible = False
        '
        'ChBxMACDZero
        '
        Me.ChBxMACDZero.AutoSize = True
        Me.ChBxMACDZero.Location = New System.Drawing.Point(4, 58)
        Me.ChBxMACDZero.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxMACDZero.Name = "ChBxMACDZero"
        Me.ChBxMACDZero.Size = New System.Drawing.Size(140, 17)
        Me.ChBxMACDZero.TabIndex = 5
        Me.ChBxMACDZero.Text = "Thresholds / 0 Linie"
        Me.ChBxMACDZero.UseVisualStyleBackColor = True
        Me.ChBxMACDZero.Visible = False
        '
        'TBMACDEMA1
        '
        Me.TBMACDEMA1.Location = New System.Drawing.Point(57, 14)
        Me.TBMACDEMA1.Margin = New System.Windows.Forms.Padding(2)
        Me.TBMACDEMA1.Name = "TBMACDEMA1"
        Me.TBMACDEMA1.Size = New System.Drawing.Size(54, 20)
        Me.TBMACDEMA1.TabIndex = 4
        Me.TBMACDEMA1.Text = "12"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(2, 39)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(44, 13)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "EMA2:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(2, 18)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(44, 13)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "EMA1:"
        '
        'TBMACDEMA2
        '
        Me.TBMACDEMA2.Location = New System.Drawing.Point(57, 37)
        Me.TBMACDEMA2.Margin = New System.Windows.Forms.Padding(2)
        Me.TBMACDEMA2.Name = "TBMACDEMA2"
        Me.TBMACDEMA2.Size = New System.Drawing.Size(54, 20)
        Me.TBMACDEMA2.TabIndex = 2
        Me.TBMACDEMA2.Text = "26"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ChBxMACDHis)
        Me.GroupBox1.Controls.Add(Me.ChBxLine)
        Me.GroupBox1.Controls.Add(Me.ChBxCandles)
        Me.GroupBox1.Controls.Add(Me.ChBxMACDSig)
        Me.GroupBox1.Controls.Add(Me.ChBxEMA2)
        Me.GroupBox1.Controls.Add(Me.ChBxEMA1)
        Me.GroupBox1.Controls.Add(Me.ChBxRSI)
        Me.GroupBox1.Controls.Add(Me.ChBxMACD)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(0, 76)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(228, 91)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Basic Settings"
        '
        'ChBxMACDHis
        '
        Me.ChBxMACDHis.AutoSize = True
        Me.ChBxMACDHis.Checked = True
        Me.ChBxMACDHis.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBxMACDHis.Location = New System.Drawing.Point(135, 61)
        Me.ChBxMACDHis.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxMACDHis.Name = "ChBxMACDHis"
        Me.ChBxMACDHis.Size = New System.Drawing.Size(66, 17)
        Me.ChBxMACDHis.TabIndex = 10
        Me.ChBxMACDHis.Text = "Histog."
        Me.ChBxMACDHis.UseVisualStyleBackColor = True
        '
        'ChBxLine
        '
        Me.ChBxLine.AutoSize = True
        Me.ChBxLine.Location = New System.Drawing.Point(70, 17)
        Me.ChBxLine.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxLine.Name = "ChBxLine"
        Me.ChBxLine.Size = New System.Drawing.Size(50, 17)
        Me.ChBxLine.TabIndex = 9
        Me.ChBxLine.Text = "Line"
        Me.ChBxLine.UseVisualStyleBackColor = True
        '
        'ChBxCandles
        '
        Me.ChBxCandles.AutoSize = True
        Me.ChBxCandles.Checked = True
        Me.ChBxCandles.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBxCandles.Location = New System.Drawing.Point(5, 17)
        Me.ChBxCandles.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxCandles.Name = "ChBxCandles"
        Me.ChBxCandles.Size = New System.Drawing.Size(71, 17)
        Me.ChBxCandles.TabIndex = 8
        Me.ChBxCandles.Text = "Candles"
        Me.ChBxCandles.UseVisualStyleBackColor = True
        '
        'ChBxMACDSig
        '
        Me.ChBxMACDSig.AutoSize = True
        Me.ChBxMACDSig.Location = New System.Drawing.Point(70, 61)
        Me.ChBxMACDSig.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxMACDSig.Name = "ChBxMACDSig"
        Me.ChBxMACDSig.Size = New System.Drawing.Size(61, 17)
        Me.ChBxMACDSig.TabIndex = 7
        Me.ChBxMACDSig.Text = "Signal"
        Me.ChBxMACDSig.UseVisualStyleBackColor = True
        '
        'ChBxEMA2
        '
        Me.ChBxEMA2.AutoSize = True
        Me.ChBxEMA2.Location = New System.Drawing.Point(70, 39)
        Me.ChBxEMA2.Name = "ChBxEMA2"
        Me.ChBxEMA2.Size = New System.Drawing.Size(59, 17)
        Me.ChBxEMA2.TabIndex = 6
        Me.ChBxEMA2.Text = "EMA2"
        Me.ChBxEMA2.UseVisualStyleBackColor = True
        '
        'ChBxEMA1
        '
        Me.ChBxEMA1.AutoSize = True
        Me.ChBxEMA1.Location = New System.Drawing.Point(5, 39)
        Me.ChBxEMA1.Name = "ChBxEMA1"
        Me.ChBxEMA1.Size = New System.Drawing.Size(59, 17)
        Me.ChBxEMA1.TabIndex = 5
        Me.ChBxEMA1.Text = "EMA1"
        Me.ChBxEMA1.UseVisualStyleBackColor = True
        '
        'ChBxRSI
        '
        Me.ChBxRSI.AutoSize = True
        Me.ChBxRSI.Location = New System.Drawing.Point(127, 17)
        Me.ChBxRSI.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxRSI.Name = "ChBxRSI"
        Me.ChBxRSI.Size = New System.Drawing.Size(47, 17)
        Me.ChBxRSI.TabIndex = 4
        Me.ChBxRSI.Text = "RSI"
        Me.ChBxRSI.UseVisualStyleBackColor = True
        '
        'ChBxMACD
        '
        Me.ChBxMACD.AutoSize = True
        Me.ChBxMACD.Location = New System.Drawing.Point(5, 61)
        Me.ChBxMACD.Margin = New System.Windows.Forms.Padding(2)
        Me.ChBxMACD.Name = "ChBxMACD"
        Me.ChBxMACD.Size = New System.Drawing.Size(61, 17)
        Me.ChBxMACD.TabIndex = 3
        Me.ChBxMACD.Text = "MACD"
        Me.ChBxMACD.UseVisualStyleBackColor = True
        '
        'LabMIN
        '
        Me.LabMIN.AutoSize = True
        Me.LabMIN.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabMIN.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabMIN.Location = New System.Drawing.Point(0, 63)
        Me.LabMIN.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabMIN.Name = "LabMIN"
        Me.LabMIN.Size = New System.Drawing.Size(29, 13)
        Me.LabMIN.TabIndex = 3
        Me.LabMIN.Text = "Pair"
        '
        'LabLast
        '
        Me.LabLast.AutoSize = True
        Me.LabLast.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabLast.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabLast.Location = New System.Drawing.Point(0, 50)
        Me.LabLast.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabLast.Name = "LabLast"
        Me.LabLast.Size = New System.Drawing.Size(29, 13)
        Me.LabLast.TabIndex = 2
        Me.LabLast.Text = "Pair"
        '
        'LabMAX
        '
        Me.LabMAX.AutoSize = True
        Me.LabMAX.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabMAX.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabMAX.Location = New System.Drawing.Point(0, 37)
        Me.LabMAX.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabMAX.Name = "LabMAX"
        Me.LabMAX.Size = New System.Drawing.Size(29, 13)
        Me.LabMAX.TabIndex = 1
        Me.LabMAX.Text = "Pair"
        '
        'LabPair
        '
        Me.LabPair.AutoSize = True
        Me.LabPair.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabPair.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabPair.Location = New System.Drawing.Point(0, 13)
        Me.LabPair.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabPair.Name = "LabPair"
        Me.LabPair.Size = New System.Drawing.Size(46, 24)
        Me.LabPair.TabIndex = 0
        Me.LabPair.Text = "Pair"
        '
        'LabExch
        '
        Me.LabExch.AutoSize = True
        Me.LabExch.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabExch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabExch.Location = New System.Drawing.Point(0, 0)
        Me.LabExch.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabExch.Name = "LabExch"
        Me.LabExch.Size = New System.Drawing.Size(29, 13)
        Me.LabExch.TabIndex = 1
        Me.LabExch.Text = "Pair"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(2)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.SplitContainer1.Size = New System.Drawing.Size(766, 371)
        Me.SplitContainer1.SplitterDistance = 230
        Me.SplitContainer1.SplitterWidth = 3
        Me.SplitContainer1.TabIndex = 0
        '
        'TradeTrackerSlot
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SlotSplitter)
        Me.Name = "TradeTrackerSlot"
        Me.Size = New System.Drawing.Size(1000, 371)
        Me.SlotSplitter.Panel1.ResumeLayout(False)
        Me.SlotSplitter.Panel1.PerformLayout()
        Me.SlotSplitter.Panel2.ResumeLayout(False)
        CType(Me.SlotSplitter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SlotSplitter.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SlotSplitter As SplitContainer
    Friend WithEvents LabPair As Label
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents LabMIN As Label
    Friend WithEvents LabLast As Label
    Friend WithEvents LabMAX As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TBMACDEMA2 As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents TBRSICandles As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents TBRSIULine As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents TBRSIOLine As TextBox
    Friend WithEvents TBMACDEMA1 As TextBox
    Friend WithEvents ChBxRSI As CheckBox
    Friend WithEvents ChBxMACD As CheckBox
    Friend WithEvents TBMACDSig As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents TBMACDULine As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents TBMACDOLine As TextBox
    Friend WithEvents ChBxMACDZero As CheckBox
    Friend WithEvents ChBxMACDSig As CheckBox
    Friend WithEvents ChBxEMA2 As CheckBox
    Friend WithEvents ChBxEMA1 As CheckBox
    Friend WithEvents Label14 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents ChBxLine As CheckBox
    Friend WithEvents ChBxCandles As CheckBox
    Friend WithEvents ChBxMACDHis As CheckBox
    Friend WithEvents LabExch As Label
End Class
