<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PFPForm
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PFPForm))
        Me.BlockTimer = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.StatusBlockLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusFeeLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LVSellorders = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LVBuyorders = New System.Windows.Forms.ListView()
        Me.ColumnHeader18 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label2 = New System.Windows.Forms.Label()
        Me.BtBuy = New System.Windows.Forms.Button()
        Me.BtSell = New System.Windows.Forms.Button()
        Me.LVOpenChannels = New System.Windows.Forms.ListView()
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader34 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label3 = New System.Windows.Forms.Label()
        Me.BtSNOSetOrder = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.LabXItem = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.NUDSNOTXFee = New System.Windows.Forms.NumericUpDown()
        Me.NUDSNOItemAmount = New System.Windows.Forms.NumericUpDown()
        Me.NUDSNOCollateral = New System.Windows.Forms.NumericUpDown()
        Me.NUDSNOAmount = New System.Windows.Forms.NumericUpDown()
        Me.BtSNOSetCurFee = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.LabXitemAmount = New System.Windows.Forms.Label()
        Me.LabWTX = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.RBSNOBuy = New System.Windows.Forms.RadioButton()
        Me.RBSNOSell = New System.Windows.Forms.RadioButton()
        Me.BtCheckAddress = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TBSNOBalance = New System.Windows.Forms.TextBox()
        Me.TBSNOAddress = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TBSNOPassPhrase = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.CoBxMarket = New System.Windows.Forms.ComboBox()
        Me.BtCreateNewAT = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer5 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer7 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer8 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer9 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer10 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer11 = New System.Windows.Forms.SplitContainer()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.SplitContainer13 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer14 = New System.Windows.Forms.SplitContainer()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SplitContainer16 = New System.Windows.Forms.SplitContainer()
        Me.LVMyOpenOrders = New System.Windows.Forms.ListView()
        Me.ColumnHeader41 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.BtSendMsg = New System.Windows.Forms.Button()
        Me.ChBxEncMsg = New System.Windows.Forms.CheckBox()
        Me.TBManuMsg = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.BtReCreatePayPalOrder = New System.Windows.Forms.Button()
        Me.BtPayOrder = New System.Windows.Forms.Button()
        Me.BtExecuteOrder = New System.Windows.Forms.Button()
        Me.SplitContainer15 = New System.Windows.Forms.SplitContainer()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.SplitContainer17 = New System.Windows.Forms.SplitContainer()
        Me.LVMyClosedOrders = New System.Windows.Forms.ListView()
        Me.ColumnHeader16 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.TBTestDelTXINI = New System.Windows.Forms.TextBox()
        Me.TBTestGetTXINI = New System.Windows.Forms.TextBox()
        Me.TBTestSetTXINI = New System.Windows.Forms.TextBox()
        Me.BtTestDelTXINI = New System.Windows.Forms.Button()
        Me.BtTestGetTXINI = New System.Windows.Forms.Button()
        Me.BtTestSetTXINI = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.BtTestPPAPI = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.BtTestConvert2 = New System.Windows.Forms.Button()
        Me.BtTestConvert = New System.Windows.Forms.Button()
        Me.TBTestConvert = New System.Windows.Forms.TextBox()
        Me.BtTestFinish = New System.Windows.Forms.Button()
        Me.BtTestAccept = New System.Windows.Forms.Button()
        Me.BtTestCreate = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.RBUseXItemSettings = New System.Windows.Forms.RadioButton()
        Me.RBUsePaymentInfo = New System.Windows.Forms.RadioButton()
        Me.ChBxCheckXItemTX = New System.Windows.Forms.CheckBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.CoBxNode = New System.Windows.Forms.ComboBox()
        Me.CoBxRefresh = New System.Windows.Forms.ComboBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtSaveSettings = New System.Windows.Forms.Button()
        Me.ChBxAutoSendPaymentInfo = New System.Windows.Forms.CheckBox()
        Me.TBPaymentInfo = New System.Windows.Forms.TextBox()
        Me.GrpBxSeller = New System.Windows.Forms.GroupBox()
        Me.TabControl2 = New System.Windows.Forms.TabControl()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.RBPayPalOrder = New System.Windows.Forms.RadioButton()
        Me.RBPayPalEMail = New System.Windows.Forms.RadioButton()
        Me.TBPayPalEMail = New System.Windows.Forms.TextBox()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.BtCheckPayPalBiz = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TBPayPalAPISecret = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TBPayPalAPIUser = New System.Windows.Forms.TextBox()
        Me.SplitContainer12 = New System.Windows.Forms.SplitContainer()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.StatusStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.NUDSNOTXFee, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUDSNOItemAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUDSNOCollateral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUDSNOAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer5.Panel1.SuspendLayout()
        Me.SplitContainer5.Panel2.SuspendLayout()
        Me.SplitContainer5.SuspendLayout()
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer6.Panel1.SuspendLayout()
        Me.SplitContainer6.Panel2.SuspendLayout()
        Me.SplitContainer6.SuspendLayout()
        CType(Me.SplitContainer7, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer7.Panel1.SuspendLayout()
        Me.SplitContainer7.Panel2.SuspendLayout()
        Me.SplitContainer7.SuspendLayout()
        CType(Me.SplitContainer8, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer8.Panel1.SuspendLayout()
        Me.SplitContainer8.Panel2.SuspendLayout()
        Me.SplitContainer8.SuspendLayout()
        CType(Me.SplitContainer9, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer9.Panel2.SuspendLayout()
        Me.SplitContainer9.SuspendLayout()
        CType(Me.SplitContainer10, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer10.Panel1.SuspendLayout()
        Me.SplitContainer10.Panel2.SuspendLayout()
        Me.SplitContainer10.SuspendLayout()
        CType(Me.SplitContainer11, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer11.Panel1.SuspendLayout()
        Me.SplitContainer11.Panel2.SuspendLayout()
        Me.SplitContainer11.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.SplitContainer13, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer13.Panel1.SuspendLayout()
        Me.SplitContainer13.Panel2.SuspendLayout()
        Me.SplitContainer13.SuspendLayout()
        CType(Me.SplitContainer14, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer14.Panel1.SuspendLayout()
        Me.SplitContainer14.Panel2.SuspendLayout()
        Me.SplitContainer14.SuspendLayout()
        CType(Me.SplitContainer16, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer16.Panel1.SuspendLayout()
        Me.SplitContainer16.Panel2.SuspendLayout()
        Me.SplitContainer16.SuspendLayout()
        CType(Me.SplitContainer15, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer15.Panel1.SuspendLayout()
        Me.SplitContainer15.Panel2.SuspendLayout()
        Me.SplitContainer15.SuspendLayout()
        CType(Me.SplitContainer17, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer17.Panel1.SuspendLayout()
        Me.SplitContainer17.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GrpBxSeller.SuspendLayout()
        Me.TabControl2.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        CType(Me.SplitContainer12, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer12.Panel1.SuspendLayout()
        Me.SplitContainer12.Panel2.SuspendLayout()
        Me.SplitContainer12.SuspendLayout()
        Me.SuspendLayout()
        '
        'BlockTimer
        '
        Me.BlockTimer.Enabled = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel, Me.StatusBar, Me.StatusBlockLabel, Me.StatusFeeLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 739)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1604, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(1528, 17)
        Me.StatusLabel.Spring = True
        Me.StatusLabel.Text = " "
        Me.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusBar
        '
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.Size = New System.Drawing.Size(117, 16)
        Me.StatusBar.Visible = False
        '
        'StatusBlockLabel
        '
        Me.StatusBlockLabel.Name = "StatusBlockLabel"
        Me.StatusBlockLabel.Size = New System.Drawing.Size(36, 17)
        Me.StatusBlockLabel.Text = "Block"
        '
        'StatusFeeLabel
        '
        Me.StatusFeeLabel.Name = "StatusFeeLabel"
        Me.StatusFeeLabel.Size = New System.Drawing.Size(25, 17)
        Me.StatusFeeLabel.Text = "Fee"
        '
        'LVSellorders
        '
        Me.LVSellorders.BackColor = System.Drawing.Color.Crimson
        Me.LVSellorders.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.LVSellorders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVSellorders.FullRowSelect = True
        Me.LVSellorders.GridLines = True
        Me.LVSellorders.HideSelection = False
        Me.LVSellorders.Location = New System.Drawing.Point(0, 0)
        Me.LVSellorders.MultiSelect = False
        Me.LVSellorders.Name = "LVSellorders"
        Me.LVSellorders.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.LVSellorders.RightToLeftLayout = True
        Me.LVSellorders.Size = New System.Drawing.Size(792, 202)
        Me.LVSellorders.TabIndex = 2
        Me.LVSellorders.UseCompatibleStateImageBehavior = False
        Me.LVSellorders.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Loading..."
        Me.ColumnHeader1.Width = 99
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(56, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Sellorders:"
        '
        'LVBuyorders
        '
        Me.LVBuyorders.BackColor = System.Drawing.Color.PaleGreen
        Me.LVBuyorders.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader18})
        Me.LVBuyorders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVBuyorders.FullRowSelect = True
        Me.LVBuyorders.GridLines = True
        Me.LVBuyorders.HideSelection = False
        Me.LVBuyorders.Location = New System.Drawing.Point(0, 0)
        Me.LVBuyorders.MultiSelect = False
        Me.LVBuyorders.Name = "LVBuyorders"
        Me.LVBuyorders.Size = New System.Drawing.Size(784, 202)
        Me.LVBuyorders.TabIndex = 5
        Me.LVBuyorders.UseCompatibleStateImageBehavior = False
        Me.LVBuyorders.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader18
        '
        Me.ColumnHeader18.Text = "Loading..."
        Me.ColumnHeader18.Width = 63
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 6)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Buyorders:"
        '
        'BtBuy
        '
        Me.BtBuy.BackColor = System.Drawing.Color.Crimson
        Me.BtBuy.Location = New System.Drawing.Point(1, -1)
        Me.BtBuy.Name = "BtBuy"
        Me.BtBuy.Size = New System.Drawing.Size(75, 23)
        Me.BtBuy.TabIndex = 7
        Me.BtBuy.Text = "Buy"
        Me.BtBuy.UseVisualStyleBackColor = False
        '
        'BtSell
        '
        Me.BtSell.BackColor = System.Drawing.Color.PaleGreen
        Me.BtSell.Location = New System.Drawing.Point(6, -1)
        Me.BtSell.Name = "BtSell"
        Me.BtSell.Size = New System.Drawing.Size(75, 23)
        Me.BtSell.TabIndex = 8
        Me.BtSell.Text = "Sell"
        Me.BtSell.UseVisualStyleBackColor = False
        '
        'LVOpenChannels
        '
        Me.LVOpenChannels.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.LVOpenChannels.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader9, Me.ColumnHeader10, Me.ColumnHeader11, Me.ColumnHeader34})
        Me.LVOpenChannels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVOpenChannels.ForeColor = System.Drawing.Color.White
        Me.LVOpenChannels.FullRowSelect = True
        Me.LVOpenChannels.GridLines = True
        Me.LVOpenChannels.HideSelection = False
        Me.LVOpenChannels.Location = New System.Drawing.Point(0, 0)
        Me.LVOpenChannels.MultiSelect = False
        Me.LVOpenChannels.Name = "LVOpenChannels"
        Me.LVOpenChannels.Size = New System.Drawing.Size(365, 146)
        Me.LVOpenChannels.TabIndex = 9
        Me.LVOpenChannels.UseCompatibleStateImageBehavior = False
        Me.LVOpenChannels.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "AT"
        Me.ColumnHeader9.Width = 114
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Name"
        Me.ColumnHeader10.Width = 67
        '
        'ColumnHeader11
        '
        Me.ColumnHeader11.Text = "Description"
        Me.ColumnHeader11.Width = 71
        '
        'ColumnHeader34
        '
        Me.ColumnHeader34.Text = "Status"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(148, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Open Payment channels:"
        '
        'BtSNOSetOrder
        '
        Me.BtSNOSetOrder.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtSNOSetOrder.Location = New System.Drawing.Point(124, 171)
        Me.BtSNOSetOrder.Name = "BtSNOSetOrder"
        Me.BtSNOSetOrder.Size = New System.Drawing.Size(82, 23)
        Me.BtSNOSetOrder.TabIndex = 11
        Me.BtSNOSetOrder.Text = "Set Order"
        Me.BtSNOSetOrder.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.LabXItem)
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Controls.Add(Me.NUDSNOTXFee)
        Me.GroupBox1.Controls.Add(Me.NUDSNOItemAmount)
        Me.GroupBox1.Controls.Add(Me.NUDSNOCollateral)
        Me.GroupBox1.Controls.Add(Me.NUDSNOAmount)
        Me.GroupBox1.Controls.Add(Me.BtSNOSetCurFee)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.LabXitemAmount)
        Me.GroupBox1.Controls.Add(Me.LabWTX)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Controls.Add(Me.BtSNOSetOrder)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(365, 196)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Set New Order"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(212, 150)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(111, 13)
        Me.Label18.TabIndex = 31
        Me.Label18.Text = "BURST (0.0=auto)"
        '
        'LabXItem
        '
        Me.LabXItem.AutoSize = True
        Me.LabXItem.Location = New System.Drawing.Point(212, 122)
        Me.LabXItem.Name = "LabXItem"
        Me.LabXItem.Size = New System.Drawing.Size(39, 13)
        Me.LabXItem.TabIndex = 30
        Me.LabXItem.Text = "XItem"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(212, 70)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(49, 13)
        Me.Label17.TabIndex = 29
        Me.Label17.Text = "BURST"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(212, 96)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(151, 13)
        Me.Label16.TabIndex = 28
        Me.Label16.Text = "+ 1 BURST handling fees"
        '
        'NUDSNOTXFee
        '
        Me.NUDSNOTXFee.DecimalPlaces = 8
        Me.NUDSNOTXFee.Location = New System.Drawing.Point(93, 146)
        Me.NUDSNOTXFee.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.NUDSNOTXFee.Name = "NUDSNOTXFee"
        Me.NUDSNOTXFee.Size = New System.Drawing.Size(113, 20)
        Me.NUDSNOTXFee.TabIndex = 27
        '
        'NUDSNOItemAmount
        '
        Me.NUDSNOItemAmount.DecimalPlaces = 8
        Me.NUDSNOItemAmount.Location = New System.Drawing.Point(93, 118)
        Me.NUDSNOItemAmount.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.NUDSNOItemAmount.Name = "NUDSNOItemAmount"
        Me.NUDSNOItemAmount.Size = New System.Drawing.Size(113, 20)
        Me.NUDSNOItemAmount.TabIndex = 26
        '
        'NUDSNOCollateral
        '
        Me.NUDSNOCollateral.DecimalPlaces = 8
        Me.NUDSNOCollateral.Location = New System.Drawing.Point(93, 92)
        Me.NUDSNOCollateral.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.NUDSNOCollateral.Name = "NUDSNOCollateral"
        Me.NUDSNOCollateral.Size = New System.Drawing.Size(113, 20)
        Me.NUDSNOCollateral.TabIndex = 25
        '
        'NUDSNOAmount
        '
        Me.NUDSNOAmount.DecimalPlaces = 8
        Me.NUDSNOAmount.Location = New System.Drawing.Point(93, 66)
        Me.NUDSNOAmount.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.NUDSNOAmount.Name = "NUDSNOAmount"
        Me.NUDSNOAmount.Size = New System.Drawing.Size(113, 20)
        Me.NUDSNOAmount.TabIndex = 24
        '
        'BtSNOSetCurFee
        '
        Me.BtSNOSetCurFee.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtSNOSetCurFee.Location = New System.Drawing.Point(331, 143)
        Me.BtSNOSetCurFee.Name = "BtSNOSetCurFee"
        Me.BtSNOSetCurFee.Size = New System.Drawing.Size(32, 23)
        Me.BtSNOSetCurFee.TabIndex = 23
        Me.BtSNOSetCurFee.Text = "set Current Slotfee"
        Me.BtSNOSetCurFee.UseVisualStyleBackColor = False
        Me.BtSNOSetCurFee.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(6, 94)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(64, 13)
        Me.Label8.TabIndex = 17
        Me.Label8.Text = "Collateral:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 148)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(52, 13)
        Me.Label7.TabIndex = 16
        Me.Label7.Text = "TX-Fee:"
        '
        'LabXitemAmount
        '
        Me.LabXitemAmount.AutoSize = True
        Me.LabXitemAmount.Location = New System.Drawing.Point(6, 120)
        Me.LabXitemAmount.Name = "LabXitemAmount"
        Me.LabXitemAmount.Size = New System.Drawing.Size(81, 13)
        Me.LabXitemAmount.TabIndex = 15
        Me.LabXitemAmount.Text = "Item Amount:"
        '
        'LabWTX
        '
        Me.LabWTX.AutoSize = True
        Me.LabWTX.Location = New System.Drawing.Point(6, 68)
        Me.LabWTX.Name = "LabWTX"
        Me.LabWTX.Size = New System.Drawing.Size(77, 13)
        Me.LabWTX.TabIndex = 13
        Me.LabWTX.Text = "WantToSell:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.RBSNOBuy)
        Me.GroupBox2.Controls.Add(Me.RBSNOSell)
        Me.GroupBox2.ForeColor = System.Drawing.Color.White
        Me.GroupBox2.Location = New System.Drawing.Point(11, 19)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(108, 40)
        Me.GroupBox2.TabIndex = 12
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Direction"
        '
        'RBSNOBuy
        '
        Me.RBSNOBuy.AutoSize = True
        Me.RBSNOBuy.Location = New System.Drawing.Point(54, 17)
        Me.RBSNOBuy.Name = "RBSNOBuy"
        Me.RBSNOBuy.Size = New System.Drawing.Size(46, 17)
        Me.RBSNOBuy.TabIndex = 1
        Me.RBSNOBuy.Text = "Buy"
        Me.RBSNOBuy.UseVisualStyleBackColor = True
        '
        'RBSNOSell
        '
        Me.RBSNOSell.AutoSize = True
        Me.RBSNOSell.Checked = True
        Me.RBSNOSell.Location = New System.Drawing.Point(6, 17)
        Me.RBSNOSell.Name = "RBSNOSell"
        Me.RBSNOSell.Size = New System.Drawing.Size(46, 17)
        Me.RBSNOSell.TabIndex = 0
        Me.RBSNOSell.TabStop = True
        Me.RBSNOSell.Text = "Sell"
        Me.RBSNOSell.UseVisualStyleBackColor = True
        '
        'BtCheckAddress
        '
        Me.BtCheckAddress.Location = New System.Drawing.Point(977, 1)
        Me.BtCheckAddress.Name = "BtCheckAddress"
        Me.BtCheckAddress.Size = New System.Drawing.Size(110, 23)
        Me.BtCheckAddress.TabIndex = 31
        Me.BtCheckAddress.Text = "Check Address"
        Me.BtCheckAddress.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(835, 6)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(49, 13)
        Me.Label11.TabIndex = 30
        Me.Label11.Text = "Balance:"
        '
        'TBSNOBalance
        '
        Me.TBSNOBalance.Location = New System.Drawing.Point(890, 3)
        Me.TBSNOBalance.Name = "TBSNOBalance"
        Me.TBSNOBalance.ReadOnly = True
        Me.TBSNOBalance.Size = New System.Drawing.Size(81, 20)
        Me.TBSNOBalance.TabIndex = 29
        '
        'TBSNOAddress
        '
        Me.TBSNOAddress.Location = New System.Drawing.Point(629, 3)
        Me.TBSNOAddress.Name = "TBSNOAddress"
        Me.TBSNOAddress.ReadOnly = True
        Me.TBSNOAddress.Size = New System.Drawing.Size(200, 20)
        Me.TBSNOAddress.TabIndex = 28
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(575, 6)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(48, 13)
        Me.Label10.TabIndex = 27
        Me.Label10.Text = "Address:"
        '
        'TBSNOPassPhrase
        '
        Me.TBSNOPassPhrase.Location = New System.Drawing.Point(277, 3)
        Me.TBSNOPassPhrase.Name = "TBSNOPassPhrase"
        Me.TBSNOPassPhrase.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TBSNOPassPhrase.Size = New System.Drawing.Size(292, 20)
        Me.TBSNOPassPhrase.TabIndex = 25
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(158, 6)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(113, 13)
        Me.Label9.TabIndex = 24
        Me.Label9.Text = "PassPhrase (TestNet):"
        '
        'CoBxMarket
        '
        Me.CoBxMarket.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxMarket.FormattingEnabled = True
        Me.CoBxMarket.Items.AddRange(New Object() {"AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD", "BTC"})
        Me.CoBxMarket.Location = New System.Drawing.Point(55, 3)
        Me.CoBxMarket.Name = "CoBxMarket"
        Me.CoBxMarket.Size = New System.Drawing.Size(97, 21)
        Me.CoBxMarket.TabIndex = 26
        '
        'BtCreateNewAT
        '
        Me.BtCreateNewAT.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtCreateNewAT.Location = New System.Drawing.Point(1, 1)
        Me.BtCreateNewAT.Name = "BtCreateNewAT"
        Me.BtCreateNewAT.Size = New System.Drawing.Size(118, 23)
        Me.BtCreateNewAT.TabIndex = 13
        Me.BtCreateNewAT.Text = "Create New AT"
        Me.BtCreateNewAT.UseVisualStyleBackColor = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer6)
        Me.SplitContainer1.Size = New System.Drawing.Size(1590, 678)
        Me.SplitContainer1.SplitterDistance = 408
        Me.SplitContainer1.TabIndex = 14
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.SplitContainer2.Panel1.ForeColor = System.Drawing.Color.White
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(1590, 408)
        Me.SplitContainer2.SplitterDistance = 1219
        Me.SplitContainer2.TabIndex = 14
        '
        'SplitContainer3
        '
        Me.SplitContainer3.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.SplitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer3.ForeColor = System.Drawing.Color.White
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.SplitContainer4)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.GroupBox1)
        Me.SplitContainer3.Size = New System.Drawing.Size(367, 408)
        Me.SplitContainer3.SplitterDistance = 206
        Me.SplitContainer3.TabIndex = 13
        '
        'SplitContainer4
        '
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer4.IsSplitterFixed = True
        Me.SplitContainer4.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer4.Name = "SplitContainer4"
        Me.SplitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.Label3)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.SplitContainer5)
        Me.SplitContainer4.Size = New System.Drawing.Size(365, 204)
        Me.SplitContainer4.SplitterDistance = 25
        Me.SplitContainer4.TabIndex = 0
        '
        'SplitContainer5
        '
        Me.SplitContainer5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer5.IsSplitterFixed = True
        Me.SplitContainer5.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer5.Name = "SplitContainer5"
        Me.SplitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer5.Panel1
        '
        Me.SplitContainer5.Panel1.Controls.Add(Me.LVOpenChannels)
        '
        'SplitContainer5.Panel2
        '
        Me.SplitContainer5.Panel2.Controls.Add(Me.BtCreateNewAT)
        Me.SplitContainer5.Size = New System.Drawing.Size(365, 175)
        Me.SplitContainer5.SplitterDistance = 146
        Me.SplitContainer5.TabIndex = 0
        '
        'SplitContainer6
        '
        Me.SplitContainer6.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.SplitContainer6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer6.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer6.Name = "SplitContainer6"
        '
        'SplitContainer6.Panel1
        '
        Me.SplitContainer6.Panel1.Controls.Add(Me.SplitContainer7)
        '
        'SplitContainer6.Panel2
        '
        Me.SplitContainer6.Panel2.Controls.Add(Me.SplitContainer10)
        Me.SplitContainer6.Size = New System.Drawing.Size(1588, 264)
        Me.SplitContainer6.SplitterDistance = 796
        Me.SplitContainer6.TabIndex = 9
        '
        'SplitContainer7
        '
        Me.SplitContainer7.BackColor = System.Drawing.Color.Crimson
        Me.SplitContainer7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer7.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer7.IsSplitterFixed = True
        Me.SplitContainer7.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer7.Name = "SplitContainer7"
        Me.SplitContainer7.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer7.Panel1
        '
        Me.SplitContainer7.Panel1.Controls.Add(Me.Label1)
        '
        'SplitContainer7.Panel2
        '
        Me.SplitContainer7.Panel2.Controls.Add(Me.SplitContainer8)
        Me.SplitContainer7.Size = New System.Drawing.Size(792, 260)
        Me.SplitContainer7.SplitterDistance = 25
        Me.SplitContainer7.TabIndex = 0
        '
        'SplitContainer8
        '
        Me.SplitContainer8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer8.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer8.IsSplitterFixed = True
        Me.SplitContainer8.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer8.Name = "SplitContainer8"
        Me.SplitContainer8.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer8.Panel1
        '
        Me.SplitContainer8.Panel1.Controls.Add(Me.LVSellorders)
        '
        'SplitContainer8.Panel2
        '
        Me.SplitContainer8.Panel2.Controls.Add(Me.SplitContainer9)
        Me.SplitContainer8.Size = New System.Drawing.Size(792, 231)
        Me.SplitContainer8.SplitterDistance = 202
        Me.SplitContainer8.TabIndex = 0
        '
        'SplitContainer9
        '
        Me.SplitContainer9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer9.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer9.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer9.Name = "SplitContainer9"
        '
        'SplitContainer9.Panel2
        '
        Me.SplitContainer9.Panel2.Controls.Add(Me.BtBuy)
        Me.SplitContainer9.Size = New System.Drawing.Size(792, 25)
        Me.SplitContainer9.SplitterDistance = 706
        Me.SplitContainer9.TabIndex = 0
        '
        'SplitContainer10
        '
        Me.SplitContainer10.BackColor = System.Drawing.Color.PaleGreen
        Me.SplitContainer10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer10.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer10.IsSplitterFixed = True
        Me.SplitContainer10.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer10.Name = "SplitContainer10"
        Me.SplitContainer10.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer10.Panel1
        '
        Me.SplitContainer10.Panel1.Controls.Add(Me.Label2)
        '
        'SplitContainer10.Panel2
        '
        Me.SplitContainer10.Panel2.Controls.Add(Me.SplitContainer11)
        Me.SplitContainer10.Size = New System.Drawing.Size(784, 260)
        Me.SplitContainer10.SplitterDistance = 25
        Me.SplitContainer10.TabIndex = 0
        '
        'SplitContainer11
        '
        Me.SplitContainer11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer11.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer11.IsSplitterFixed = True
        Me.SplitContainer11.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer11.Name = "SplitContainer11"
        Me.SplitContainer11.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer11.Panel1
        '
        Me.SplitContainer11.Panel1.Controls.Add(Me.LVBuyorders)
        '
        'SplitContainer11.Panel2
        '
        Me.SplitContainer11.Panel2.Controls.Add(Me.BtSell)
        Me.SplitContainer11.Size = New System.Drawing.Size(784, 231)
        Me.SplitContainer11.SplitterDistance = 202
        Me.SplitContainer11.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1604, 710)
        Me.TabControl1.TabIndex = 15
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1596, 684)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Marketdetails"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.SplitContainer13)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1596, 684)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "MyOrders"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'SplitContainer13
        '
        Me.SplitContainer13.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.SplitContainer13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer13.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer13.ForeColor = System.Drawing.Color.White
        Me.SplitContainer13.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer13.Name = "SplitContainer13"
        Me.SplitContainer13.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer13.Panel1
        '
        Me.SplitContainer13.Panel1.Controls.Add(Me.SplitContainer14)
        '
        'SplitContainer13.Panel2
        '
        Me.SplitContainer13.Panel2.Controls.Add(Me.SplitContainer15)
        Me.SplitContainer13.Size = New System.Drawing.Size(1590, 678)
        Me.SplitContainer13.SplitterDistance = 305
        Me.SplitContainer13.TabIndex = 5
        '
        'SplitContainer14
        '
        Me.SplitContainer14.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer14.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer14.IsSplitterFixed = True
        Me.SplitContainer14.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer14.Name = "SplitContainer14"
        Me.SplitContainer14.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer14.Panel1
        '
        Me.SplitContainer14.Panel1.Controls.Add(Me.Label5)
        '
        'SplitContainer14.Panel2
        '
        Me.SplitContainer14.Panel2.Controls.Add(Me.SplitContainer16)
        Me.SplitContainer14.Size = New System.Drawing.Size(1588, 303)
        Me.SplitContainer14.SplitterDistance = 25
        Me.SplitContainer14.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 6)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(81, 13)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "MyOpenOrders:"
        '
        'SplitContainer16
        '
        Me.SplitContainer16.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer16.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer16.IsSplitterFixed = True
        Me.SplitContainer16.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer16.Name = "SplitContainer16"
        Me.SplitContainer16.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer16.Panel1
        '
        Me.SplitContainer16.Panel1.Controls.Add(Me.LVMyOpenOrders)
        '
        'SplitContainer16.Panel2
        '
        Me.SplitContainer16.Panel2.Controls.Add(Me.BtSendMsg)
        Me.SplitContainer16.Panel2.Controls.Add(Me.ChBxEncMsg)
        Me.SplitContainer16.Panel2.Controls.Add(Me.TBManuMsg)
        Me.SplitContainer16.Panel2.Controls.Add(Me.Label15)
        Me.SplitContainer16.Panel2.Controls.Add(Me.BtReCreatePayPalOrder)
        Me.SplitContainer16.Panel2.Controls.Add(Me.BtPayOrder)
        Me.SplitContainer16.Panel2.Controls.Add(Me.BtExecuteOrder)
        Me.SplitContainer16.Size = New System.Drawing.Size(1588, 274)
        Me.SplitContainer16.SplitterDistance = 191
        Me.SplitContainer16.TabIndex = 0
        '
        'LVMyOpenOrders
        '
        Me.LVMyOpenOrders.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.LVMyOpenOrders.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader41})
        Me.LVMyOpenOrders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVMyOpenOrders.ForeColor = System.Drawing.Color.White
        Me.LVMyOpenOrders.FullRowSelect = True
        Me.LVMyOpenOrders.GridLines = True
        Me.LVMyOpenOrders.HideSelection = False
        Me.LVMyOpenOrders.Location = New System.Drawing.Point(0, 0)
        Me.LVMyOpenOrders.MultiSelect = False
        Me.LVMyOpenOrders.Name = "LVMyOpenOrders"
        Me.LVMyOpenOrders.Size = New System.Drawing.Size(1588, 191)
        Me.LVMyOpenOrders.TabIndex = 0
        Me.LVMyOpenOrders.UseCompatibleStateImageBehavior = False
        Me.LVMyOpenOrders.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader41
        '
        Me.ColumnHeader41.Text = "Loading..."
        Me.ColumnHeader41.Width = 90
        '
        'BtSendMsg
        '
        Me.BtSendMsg.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtSendMsg.Location = New System.Drawing.Point(206, 53)
        Me.BtSendMsg.Name = "BtSendMsg"
        Me.BtSendMsg.Size = New System.Drawing.Size(94, 23)
        Me.BtSendMsg.TabIndex = 7
        Me.BtSendMsg.Text = "send Message"
        Me.BtSendMsg.UseVisualStyleBackColor = False
        Me.BtSendMsg.Visible = False
        '
        'ChBxEncMsg
        '
        Me.ChBxEncMsg.AutoSize = True
        Me.ChBxEncMsg.Checked = True
        Me.ChBxEncMsg.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBxEncMsg.Location = New System.Drawing.Point(8, 59)
        Me.ChBxEncMsg.Name = "ChBxEncMsg"
        Me.ChBxEncMsg.Size = New System.Drawing.Size(108, 17)
        Me.ChBxEncMsg.TabIndex = 6
        Me.ChBxEncMsg.Text = "Encrypt Message"
        Me.ChBxEncMsg.UseVisualStyleBackColor = True
        Me.ChBxEncMsg.Visible = False
        '
        'TBManuMsg
        '
        Me.TBManuMsg.Location = New System.Drawing.Point(64, 29)
        Me.TBManuMsg.Name = "TBManuMsg"
        Me.TBManuMsg.Size = New System.Drawing.Size(236, 20)
        Me.TBManuMsg.TabIndex = 5
        Me.TBManuMsg.Visible = False
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(5, 32)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(53, 13)
        Me.Label15.TabIndex = 4
        Me.Label15.Text = "Message:"
        Me.Label15.Visible = False
        '
        'BtReCreatePayPalOrder
        '
        Me.BtReCreatePayPalOrder.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtReCreatePayPalOrder.Location = New System.Drawing.Point(206, 3)
        Me.BtReCreatePayPalOrder.Name = "BtReCreatePayPalOrder"
        Me.BtReCreatePayPalOrder.Size = New System.Drawing.Size(94, 23)
        Me.BtReCreatePayPalOrder.TabIndex = 3
        Me.BtReCreatePayPalOrder.Text = "send new PP-ID"
        Me.BtReCreatePayPalOrder.UseVisualStyleBackColor = False
        Me.BtReCreatePayPalOrder.Visible = False
        '
        'BtPayOrder
        '
        Me.BtPayOrder.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtPayOrder.Location = New System.Drawing.Point(106, 3)
        Me.BtPayOrder.Name = "BtPayOrder"
        Me.BtPayOrder.Size = New System.Drawing.Size(94, 23)
        Me.BtPayOrder.TabIndex = 2
        Me.BtPayOrder.Text = "pay Order"
        Me.BtPayOrder.UseVisualStyleBackColor = False
        Me.BtPayOrder.Visible = False
        '
        'BtExecuteOrder
        '
        Me.BtExecuteOrder.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtExecuteOrder.Location = New System.Drawing.Point(6, 3)
        Me.BtExecuteOrder.Name = "BtExecuteOrder"
        Me.BtExecuteOrder.Size = New System.Drawing.Size(94, 23)
        Me.BtExecuteOrder.TabIndex = 1
        Me.BtExecuteOrder.Text = "execute/cancel"
        Me.BtExecuteOrder.UseVisualStyleBackColor = False
        Me.BtExecuteOrder.Visible = False
        '
        'SplitContainer15
        '
        Me.SplitContainer15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer15.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer15.IsSplitterFixed = True
        Me.SplitContainer15.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer15.Name = "SplitContainer15"
        Me.SplitContainer15.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer15.Panel1
        '
        Me.SplitContainer15.Panel1.Controls.Add(Me.Label12)
        '
        'SplitContainer15.Panel2
        '
        Me.SplitContainer15.Panel2.Controls.Add(Me.SplitContainer17)
        Me.SplitContainer15.Size = New System.Drawing.Size(1588, 367)
        Me.SplitContainer15.SplitterDistance = 25
        Me.SplitContainer15.TabIndex = 0
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(3, 6)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(87, 13)
        Me.Label12.TabIndex = 4
        Me.Label12.Text = "MyClosedOrders:"
        '
        'SplitContainer17
        '
        Me.SplitContainer17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer17.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer17.IsSplitterFixed = True
        Me.SplitContainer17.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer17.Name = "SplitContainer17"
        Me.SplitContainer17.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer17.Panel1
        '
        Me.SplitContainer17.Panel1.Controls.Add(Me.LVMyClosedOrders)
        Me.SplitContainer17.Size = New System.Drawing.Size(1588, 338)
        Me.SplitContainer17.SplitterDistance = 307
        Me.SplitContainer17.TabIndex = 0
        '
        'LVMyClosedOrders
        '
        Me.LVMyClosedOrders.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.LVMyClosedOrders.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader16})
        Me.LVMyClosedOrders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVMyClosedOrders.ForeColor = System.Drawing.Color.White
        Me.LVMyClosedOrders.FullRowSelect = True
        Me.LVMyClosedOrders.GridLines = True
        Me.LVMyClosedOrders.HideSelection = False
        Me.LVMyClosedOrders.Location = New System.Drawing.Point(0, 0)
        Me.LVMyClosedOrders.MultiSelect = False
        Me.LVMyClosedOrders.Name = "LVMyClosedOrders"
        Me.LVMyClosedOrders.Size = New System.Drawing.Size(1588, 307)
        Me.LVMyClosedOrders.TabIndex = 3
        Me.LVMyClosedOrders.UseCompatibleStateImageBehavior = False
        Me.LVMyClosedOrders.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader16
        '
        Me.ColumnHeader16.Text = "Loading..."
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.TabPage3.Controls.Add(Me.GroupBox4)
        Me.TabPage3.Controls.Add(Me.GroupBox3)
        Me.TabPage3.Controls.Add(Me.GrpBxSeller)
        Me.TabPage3.ForeColor = System.Drawing.Color.White
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(1596, 684)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Settings"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.TBTestDelTXINI)
        Me.GroupBox4.Controls.Add(Me.TBTestGetTXINI)
        Me.GroupBox4.Controls.Add(Me.TBTestSetTXINI)
        Me.GroupBox4.Controls.Add(Me.BtTestDelTXINI)
        Me.GroupBox4.Controls.Add(Me.BtTestGetTXINI)
        Me.GroupBox4.Controls.Add(Me.BtTestSetTXINI)
        Me.GroupBox4.Controls.Add(Me.Button3)
        Me.GroupBox4.Controls.Add(Me.BtTestPPAPI)
        Me.GroupBox4.Controls.Add(Me.ListBox1)
        Me.GroupBox4.Controls.Add(Me.Button2)
        Me.GroupBox4.Controls.Add(Me.Button1)
        Me.GroupBox4.Controls.Add(Me.BtTestConvert2)
        Me.GroupBox4.Controls.Add(Me.BtTestConvert)
        Me.GroupBox4.Controls.Add(Me.TBTestConvert)
        Me.GroupBox4.Controls.Add(Me.BtTestFinish)
        Me.GroupBox4.Controls.Add(Me.BtTestAccept)
        Me.GroupBox4.Controls.Add(Me.BtTestCreate)
        Me.GroupBox4.Location = New System.Drawing.Point(8, 276)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(1585, 389)
        Me.GroupBox4.TabIndex = 9
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Testing"
        '
        'TBTestDelTXINI
        '
        Me.TBTestDelTXINI.Location = New System.Drawing.Point(864, 47)
        Me.TBTestDelTXINI.Name = "TBTestDelTXINI"
        Me.TBTestDelTXINI.Size = New System.Drawing.Size(97, 20)
        Me.TBTestDelTXINI.TabIndex = 16
        '
        'TBTestGetTXINI
        '
        Me.TBTestGetTXINI.Location = New System.Drawing.Point(761, 47)
        Me.TBTestGetTXINI.Name = "TBTestGetTXINI"
        Me.TBTestGetTXINI.Size = New System.Drawing.Size(97, 20)
        Me.TBTestGetTXINI.TabIndex = 15
        '
        'TBTestSetTXINI
        '
        Me.TBTestSetTXINI.Location = New System.Drawing.Point(658, 47)
        Me.TBTestSetTXINI.Name = "TBTestSetTXINI"
        Me.TBTestSetTXINI.Size = New System.Drawing.Size(97, 20)
        Me.TBTestSetTXINI.TabIndex = 14
        '
        'BtTestDelTXINI
        '
        Me.BtTestDelTXINI.ForeColor = System.Drawing.Color.Black
        Me.BtTestDelTXINI.Location = New System.Drawing.Point(864, 74)
        Me.BtTestDelTXINI.Name = "BtTestDelTXINI"
        Me.BtTestDelTXINI.Size = New System.Drawing.Size(97, 23)
        Me.BtTestDelTXINI.TabIndex = 13
        Me.BtTestDelTXINI.Text = "Del TX from INI"
        Me.BtTestDelTXINI.UseVisualStyleBackColor = True
        '
        'BtTestGetTXINI
        '
        Me.BtTestGetTXINI.ForeColor = System.Drawing.Color.Black
        Me.BtTestGetTXINI.Location = New System.Drawing.Point(761, 74)
        Me.BtTestGetTXINI.Name = "BtTestGetTXINI"
        Me.BtTestGetTXINI.Size = New System.Drawing.Size(97, 23)
        Me.BtTestGetTXINI.TabIndex = 12
        Me.BtTestGetTXINI.Text = "Get TX from INI"
        Me.BtTestGetTXINI.UseVisualStyleBackColor = True
        '
        'BtTestSetTXINI
        '
        Me.BtTestSetTXINI.ForeColor = System.Drawing.Color.Black
        Me.BtTestSetTXINI.Location = New System.Drawing.Point(658, 74)
        Me.BtTestSetTXINI.Name = "BtTestSetTXINI"
        Me.BtTestSetTXINI.Size = New System.Drawing.Size(97, 23)
        Me.BtTestSetTXINI.TabIndex = 11
        Me.BtTestSetTXINI.Text = "Set TX in INI"
        Me.BtTestSetTXINI.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.ForeColor = System.Drawing.Color.Black
        Me.Button3.Location = New System.Drawing.Point(459, 48)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(118, 23)
        Me.Button3.TabIndex = 10
        Me.Button3.Text = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'BtTestPPAPI
        '
        Me.BtTestPPAPI.ForeColor = System.Drawing.Color.Black
        Me.BtTestPPAPI.Location = New System.Drawing.Point(459, 19)
        Me.BtTestPPAPI.Name = "BtTestPPAPI"
        Me.BtTestPPAPI.Size = New System.Drawing.Size(118, 23)
        Me.BtTestPPAPI.TabIndex = 9
        Me.BtTestPPAPI.Text = "PPAPIGetPayments"
        Me.BtTestPPAPI.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(7, 103)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(1572, 264)
        Me.ListBox1.TabIndex = 8
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(1000, 74)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 7
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.ForeColor = System.Drawing.Color.Black
        Me.Button1.Location = New System.Drawing.Point(6, 45)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(104, 23)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "DataStr2ULngList"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'BtTestConvert2
        '
        Me.BtTestConvert2.ForeColor = System.Drawing.Color.Black
        Me.BtTestConvert2.Location = New System.Drawing.Point(208, 45)
        Me.BtTestConvert2.Name = "BtTestConvert2"
        Me.BtTestConvert2.Size = New System.Drawing.Size(86, 23)
        Me.BtTestConvert2.TabIndex = 5
        Me.BtTestConvert2.Text = "ULong2String"
        Me.BtTestConvert2.UseVisualStyleBackColor = True
        '
        'BtTestConvert
        '
        Me.BtTestConvert.ForeColor = System.Drawing.Color.Black
        Me.BtTestConvert.Location = New System.Drawing.Point(116, 45)
        Me.BtTestConvert.Name = "BtTestConvert"
        Me.BtTestConvert.Size = New System.Drawing.Size(86, 23)
        Me.BtTestConvert.TabIndex = 4
        Me.BtTestConvert.Text = "String2ULong"
        Me.BtTestConvert.UseVisualStyleBackColor = True
        '
        'TBTestConvert
        '
        Me.TBTestConvert.Location = New System.Drawing.Point(6, 19)
        Me.TBTestConvert.Name = "TBTestConvert"
        Me.TBTestConvert.Size = New System.Drawing.Size(408, 20)
        Me.TBTestConvert.TabIndex = 3
        '
        'BtTestFinish
        '
        Me.BtTestFinish.ForeColor = System.Drawing.Color.Black
        Me.BtTestFinish.Location = New System.Drawing.Point(168, 74)
        Me.BtTestFinish.Name = "BtTestFinish"
        Me.BtTestFinish.Size = New System.Drawing.Size(75, 23)
        Me.BtTestFinish.TabIndex = 2
        Me.BtTestFinish.Text = "finish"
        Me.BtTestFinish.UseVisualStyleBackColor = True
        '
        'BtTestAccept
        '
        Me.BtTestAccept.ForeColor = System.Drawing.Color.Black
        Me.BtTestAccept.Location = New System.Drawing.Point(87, 74)
        Me.BtTestAccept.Name = "BtTestAccept"
        Me.BtTestAccept.Size = New System.Drawing.Size(75, 23)
        Me.BtTestAccept.TabIndex = 1
        Me.BtTestAccept.Text = "accept"
        Me.BtTestAccept.UseVisualStyleBackColor = True
        '
        'BtTestCreate
        '
        Me.BtTestCreate.ForeColor = System.Drawing.Color.Black
        Me.BtTestCreate.Location = New System.Drawing.Point(6, 74)
        Me.BtTestCreate.Name = "BtTestCreate"
        Me.BtTestCreate.Size = New System.Drawing.Size(75, 23)
        Me.BtTestCreate.TabIndex = 0
        Me.BtTestCreate.Text = "create"
        Me.BtTestCreate.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.RBUseXItemSettings)
        Me.GroupBox3.Controls.Add(Me.RBUsePaymentInfo)
        Me.GroupBox3.Controls.Add(Me.ChBxCheckXItemTX)
        Me.GroupBox3.Controls.Add(Me.Label21)
        Me.GroupBox3.Controls.Add(Me.Label20)
        Me.GroupBox3.Controls.Add(Me.CoBxNode)
        Me.GroupBox3.Controls.Add(Me.CoBxRefresh)
        Me.GroupBox3.Controls.Add(Me.Label19)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.BtSaveSettings)
        Me.GroupBox3.Controls.Add(Me.ChBxAutoSendPaymentInfo)
        Me.GroupBox3.Controls.Add(Me.TBPaymentInfo)
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Location = New System.Drawing.Point(8, 13)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(341, 257)
        Me.GroupBox3.TabIndex = 8
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "General Settings"
        '
        'RBUseXItemSettings
        '
        Me.RBUseXItemSettings.AutoSize = True
        Me.RBUseXItemSettings.Location = New System.Drawing.Point(28, 65)
        Me.RBUseXItemSettings.Name = "RBUseXItemSettings"
        Me.RBUseXItemSettings.Size = New System.Drawing.Size(115, 17)
        Me.RBUseXItemSettings.TabIndex = 18
        Me.RBUseXItemSettings.TabStop = True
        Me.RBUseXItemSettings.Text = "Use XItem Settings"
        Me.RBUseXItemSettings.UseVisualStyleBackColor = True
        '
        'RBUsePaymentInfo
        '
        Me.RBUsePaymentInfo.AutoSize = True
        Me.RBUsePaymentInfo.Location = New System.Drawing.Point(28, 42)
        Me.RBUsePaymentInfo.Name = "RBUsePaymentInfo"
        Me.RBUsePaymentInfo.Size = New System.Drawing.Size(112, 17)
        Me.RBUsePaymentInfo.TabIndex = 17
        Me.RBUsePaymentInfo.TabStop = True
        Me.RBUsePaymentInfo.Text = "Use Payment Info:"
        Me.RBUsePaymentInfo.UseVisualStyleBackColor = True
        '
        'ChBxCheckXItemTX
        '
        Me.ChBxCheckXItemTX.AutoSize = True
        Me.ChBxCheckXItemTX.Location = New System.Drawing.Point(6, 137)
        Me.ChBxCheckXItemTX.Name = "ChBxCheckXItemTX"
        Me.ChBxCheckXItemTX.Size = New System.Drawing.Size(270, 17)
        Me.ChBxCheckXItemTX.TabIndex = 16
        Me.ChBxCheckXItemTX.Text = "automatically check XItem transaction and finish AT"
        Me.ChBxCheckXItemTX.UseVisualStyleBackColor = True
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.ForeColor = System.Drawing.Color.Yellow
        Me.Label21.Location = New System.Drawing.Point(65, 211)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(260, 13)
        Me.Label21.TabIndex = 15
        Me.Label21.Text = "It is strongly recommended to use localhost !"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(202, 163)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(44, 13)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "Minutes"
        '
        'CoBxNode
        '
        Me.CoBxNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxNode.FormattingEnabled = True
        Me.CoBxNode.Items.AddRange(New Object() {"http://nivbox.co.uk:6876/burst", "https://testnet.burstcoin.network:6876/burst", "https://testnet-2.burst-alliance.org:6876/burst", "https://wallet.testnet.burstscan.net/burst", "https://wallet.dev.burst-test.net/burst", "http://localhost:6876/burst"})
        Me.CoBxNode.Location = New System.Drawing.Point(133, 187)
        Me.CoBxNode.Name = "CoBxNode"
        Me.CoBxNode.Size = New System.Drawing.Size(192, 21)
        Me.CoBxNode.TabIndex = 13
        '
        'CoBxRefresh
        '
        Me.CoBxRefresh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxRefresh.FormattingEnabled = True
        Me.CoBxRefresh.Items.AddRange(New Object() {"1", "4", "10"})
        Me.CoBxRefresh.Location = New System.Drawing.Point(133, 160)
        Me.CoBxRefresh.Name = "CoBxRefresh"
        Me.CoBxRefresh.Size = New System.Drawing.Size(63, 21)
        Me.CoBxRefresh.TabIndex = 12
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(6, 190)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(121, 13)
        Me.Label19.TabIndex = 11
        Me.Label19.Text = "API-Node (POST/GET):"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 163)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 13)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Refreshrate:"
        '
        'BtSaveSettings
        '
        Me.BtSaveSettings.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtSaveSettings.Location = New System.Drawing.Point(6, 227)
        Me.BtSaveSettings.Name = "BtSaveSettings"
        Me.BtSaveSettings.Size = New System.Drawing.Size(134, 23)
        Me.BtSaveSettings.TabIndex = 9
        Me.BtSaveSettings.Text = "save settings"
        Me.BtSaveSettings.UseVisualStyleBackColor = False
        '
        'ChBxAutoSendPaymentInfo
        '
        Me.ChBxAutoSendPaymentInfo.AutoSize = True
        Me.ChBxAutoSendPaymentInfo.Location = New System.Drawing.Point(6, 19)
        Me.ChBxAutoSendPaymentInfo.Name = "ChBxAutoSendPaymentInfo"
        Me.ChBxAutoSendPaymentInfo.Size = New System.Drawing.Size(319, 17)
        Me.ChBxAutoSendPaymentInfo.TabIndex = 4
        Me.ChBxAutoSendPaymentInfo.Text = "automatically send encrypted payment information to the buyer"
        Me.ChBxAutoSendPaymentInfo.UseVisualStyleBackColor = True
        '
        'TBPaymentInfo
        '
        Me.TBPaymentInfo.Enabled = False
        Me.TBPaymentInfo.Location = New System.Drawing.Point(146, 41)
        Me.TBPaymentInfo.Name = "TBPaymentInfo"
        Me.TBPaymentInfo.Size = New System.Drawing.Size(179, 20)
        Me.TBPaymentInfo.TabIndex = 7
        '
        'GrpBxSeller
        '
        Me.GrpBxSeller.Controls.Add(Me.TabControl2)
        Me.GrpBxSeller.ForeColor = System.Drawing.Color.White
        Me.GrpBxSeller.Location = New System.Drawing.Point(355, 13)
        Me.GrpBxSeller.Name = "GrpBxSeller"
        Me.GrpBxSeller.Size = New System.Drawing.Size(405, 257)
        Me.GrpBxSeller.TabIndex = 0
        Me.GrpBxSeller.TabStop = False
        Me.GrpBxSeller.Text = "PayPal Settings (Sandbox)"
        '
        'TabControl2
        '
        Me.TabControl2.Controls.Add(Me.TabPage4)
        Me.TabControl2.Controls.Add(Me.TabPage5)
        Me.TabControl2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl2.Location = New System.Drawing.Point(3, 16)
        Me.TabControl2.Name = "TabControl2"
        Me.TabControl2.SelectedIndex = 0
        Me.TabControl2.Size = New System.Drawing.Size(399, 238)
        Me.TabControl2.TabIndex = 6
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.TabPage4.Controls.Add(Me.RBPayPalOrder)
        Me.TabPage4.Controls.Add(Me.RBPayPalEMail)
        Me.TabPage4.Controls.Add(Me.TBPayPalEMail)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(391, 212)
        Me.TabPage4.TabIndex = 0
        Me.TabPage4.Text = "Payment informations"
        '
        'RBPayPalOrder
        '
        Me.RBPayPalOrder.AutoSize = True
        Me.RBPayPalOrder.Location = New System.Drawing.Point(5, 32)
        Me.RBPayPalOrder.Name = "RBPayPalOrder"
        Me.RBPayPalOrder.Size = New System.Drawing.Size(243, 17)
        Me.RBPayPalOrder.TabIndex = 12
        Me.RBPayPalOrder.Text = "create a PayPal Order (need PayPal Business)"
        Me.RBPayPalOrder.UseVisualStyleBackColor = True
        '
        'RBPayPalEMail
        '
        Me.RBPayPalEMail.AutoSize = True
        Me.RBPayPalEMail.Checked = True
        Me.RBPayPalEMail.Location = New System.Drawing.Point(5, 7)
        Me.RBPayPalEMail.Name = "RBPayPalEMail"
        Me.RBPayPalEMail.Size = New System.Drawing.Size(93, 17)
        Me.RBPayPalEMail.TabIndex = 10
        Me.RBPayPalEMail.TabStop = True
        Me.RBPayPalEMail.Text = "PayPal E-Mail:"
        Me.RBPayPalEMail.UseVisualStyleBackColor = True
        '
        'TBPayPalEMail
        '
        Me.TBPayPalEMail.Location = New System.Drawing.Point(104, 6)
        Me.TBPayPalEMail.Name = "TBPayPalEMail"
        Me.TBPayPalEMail.Size = New System.Drawing.Size(262, 20)
        Me.TBPayPalEMail.TabIndex = 7
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.TabPage5.Controls.Add(Me.BtCheckPayPalBiz)
        Me.TabPage5.Controls.Add(Me.Label13)
        Me.TabPage5.Controls.Add(Me.TBPayPalAPISecret)
        Me.TabPage5.Controls.Add(Me.Label14)
        Me.TabPage5.Controls.Add(Me.TBPayPalAPIUser)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(391, 212)
        Me.TabPage5.TabIndex = 1
        Me.TabPage5.Text = "Business API"
        '
        'BtCheckPayPalBiz
        '
        Me.BtCheckPayPalBiz.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.BtCheckPayPalBiz.Location = New System.Drawing.Point(76, 58)
        Me.BtCheckPayPalBiz.Name = "BtCheckPayPalBiz"
        Me.BtCheckPayPalBiz.Size = New System.Drawing.Size(75, 23)
        Me.BtCheckPayPalBiz.TabIndex = 4
        Me.BtCheckPayPalBiz.Text = "check"
        Me.BtCheckPayPalBiz.UseVisualStyleBackColor = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(9, 9)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(52, 13)
        Me.Label13.TabIndex = 0
        Me.Label13.Text = "API-User:"
        '
        'TBPayPalAPISecret
        '
        Me.TBPayPalAPISecret.Location = New System.Drawing.Point(76, 32)
        Me.TBPayPalAPISecret.Name = "TBPayPalAPISecret"
        Me.TBPayPalAPISecret.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TBPayPalAPISecret.Size = New System.Drawing.Size(296, 20)
        Me.TBPayPalAPISecret.TabIndex = 3
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(9, 35)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(61, 13)
        Me.Label14.TabIndex = 1
        Me.Label14.Text = "API-Secret:"
        '
        'TBPayPalAPIUser
        '
        Me.TBPayPalAPIUser.Location = New System.Drawing.Point(76, 6)
        Me.TBPayPalAPIUser.Name = "TBPayPalAPIUser"
        Me.TBPayPalAPIUser.Size = New System.Drawing.Size(296, 20)
        Me.TBPayPalAPIUser.TabIndex = 2
        '
        'SplitContainer12
        '
        Me.SplitContainer12.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer12.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer12.IsSplitterFixed = True
        Me.SplitContainer12.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer12.Name = "SplitContainer12"
        Me.SplitContainer12.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer12.Panel1
        '
        Me.SplitContainer12.Panel1.Controls.Add(Me.BtCheckAddress)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label11)
        Me.SplitContainer12.Panel1.Controls.Add(Me.TBSNOBalance)
        Me.SplitContainer12.Panel1.Controls.Add(Me.CoBxMarket)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label9)
        Me.SplitContainer12.Panel1.Controls.Add(Me.TBSNOAddress)
        Me.SplitContainer12.Panel1.Controls.Add(Me.TBSNOPassPhrase)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label10)
        '
        'SplitContainer12.Panel2
        '
        Me.SplitContainer12.Panel2.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer12.Panel2.Controls.Add(Me.TabControl1)
        Me.SplitContainer12.Size = New System.Drawing.Size(1604, 739)
        Me.SplitContainer12.SplitterDistance = 25
        Me.SplitContainer12.TabIndex = 15
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 6)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Market:"
        '
        'PFPForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1604, 761)
        Me.Controls.Add(Me.SplitContainer12)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "PFPForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Codename: Perls for Pigs (TestNet)"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.NUDSNOTXFee, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUDSNOItemAmount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUDSNOCollateral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUDSNOAmount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel1.PerformLayout()
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.SplitContainer5.Panel1.ResumeLayout(False)
        Me.SplitContainer5.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer5.ResumeLayout(False)
        Me.SplitContainer6.Panel1.ResumeLayout(False)
        Me.SplitContainer6.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer6.ResumeLayout(False)
        Me.SplitContainer7.Panel1.ResumeLayout(False)
        Me.SplitContainer7.Panel1.PerformLayout()
        Me.SplitContainer7.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer7, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer7.ResumeLayout(False)
        Me.SplitContainer8.Panel1.ResumeLayout(False)
        Me.SplitContainer8.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer8, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer8.ResumeLayout(False)
        Me.SplitContainer9.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer9, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer9.ResumeLayout(False)
        Me.SplitContainer10.Panel1.ResumeLayout(False)
        Me.SplitContainer10.Panel1.PerformLayout()
        Me.SplitContainer10.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer10, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer10.ResumeLayout(False)
        Me.SplitContainer11.Panel1.ResumeLayout(False)
        Me.SplitContainer11.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer11, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer11.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.SplitContainer13.Panel1.ResumeLayout(False)
        Me.SplitContainer13.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer13, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer13.ResumeLayout(False)
        Me.SplitContainer14.Panel1.ResumeLayout(False)
        Me.SplitContainer14.Panel1.PerformLayout()
        Me.SplitContainer14.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer14, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer14.ResumeLayout(False)
        Me.SplitContainer16.Panel1.ResumeLayout(False)
        Me.SplitContainer16.Panel2.ResumeLayout(False)
        Me.SplitContainer16.Panel2.PerformLayout()
        CType(Me.SplitContainer16, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer16.ResumeLayout(False)
        Me.SplitContainer15.Panel1.ResumeLayout(False)
        Me.SplitContainer15.Panel1.PerformLayout()
        Me.SplitContainer15.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer15, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer15.ResumeLayout(False)
        Me.SplitContainer17.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer17, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer17.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GrpBxSeller.ResumeLayout(False)
        Me.TabControl2.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        Me.SplitContainer12.Panel1.ResumeLayout(False)
        Me.SplitContainer12.Panel1.PerformLayout()
        Me.SplitContainer12.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer12, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer12.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BlockTimer As Timer
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents StatusLabel As ToolStripStatusLabel
    Friend WithEvents StatusBlockLabel As ToolStripStatusLabel
    Friend WithEvents StatusFeeLabel As ToolStripStatusLabel
    Friend WithEvents StatusBar As ToolStripProgressBar
    Friend WithEvents LVSellorders As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents Label1 As Label
    Friend WithEvents LVBuyorders As ListView
    Friend WithEvents Label2 As Label
    Friend WithEvents BtBuy As Button
    Friend WithEvents BtSell As Button
    Friend WithEvents LVOpenChannels As ListView
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents ColumnHeader10 As ColumnHeader
    Friend WithEvents ColumnHeader11 As ColumnHeader
    Friend WithEvents Label3 As Label
    Friend WithEvents BtSNOSetOrder As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents CoBxMarket As ComboBox
    Friend WithEvents TBSNOPassPhrase As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents BtSNOSetCurFee As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents LabXitemAmount As Label
    Friend WithEvents LabWTX As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents RBSNOBuy As RadioButton
    Friend WithEvents RBSNOSell As RadioButton
    Friend WithEvents BtCreateNewAT As Button
    Friend WithEvents BtCheckAddress As Button
    Friend WithEvents Label11 As Label
    Friend WithEvents TBSNOBalance As TextBox
    Friend WithEvents TBSNOAddress As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents SplitContainer4 As SplitContainer
    Friend WithEvents SplitContainer5 As SplitContainer
    Friend WithEvents SplitContainer6 As SplitContainer
    Friend WithEvents SplitContainer7 As SplitContainer
    Friend WithEvents SplitContainer8 As SplitContainer
    Friend WithEvents SplitContainer9 As SplitContainer
    Friend WithEvents SplitContainer10 As SplitContainer
    Friend WithEvents SplitContainer11 As SplitContainer
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents SplitContainer12 As SplitContainer
    Friend WithEvents Label4 As Label
    Friend WithEvents LVMyOpenOrders As ListView
    Friend WithEvents BtExecuteOrder As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents LVMyClosedOrders As ListView
    Friend WithEvents SplitContainer13 As SplitContainer
    Friend WithEvents SplitContainer14 As SplitContainer
    Friend WithEvents SplitContainer16 As SplitContainer
    Friend WithEvents SplitContainer15 As SplitContainer
    Friend WithEvents SplitContainer17 As SplitContainer
    Friend WithEvents ColumnHeader34 As ColumnHeader
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents GrpBxSeller As GroupBox
    Friend WithEvents ChBxAutoSendPaymentInfo As CheckBox
    Friend WithEvents TBPayPalAPISecret As TextBox
    Friend WithEvents TBPayPalAPIUser As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents TBPaymentInfo As TextBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents TabControl2 As TabControl
    Friend WithEvents TabPage4 As TabPage
    Friend WithEvents RBPayPalEMail As RadioButton
    Friend WithEvents TBPayPalEMail As TextBox
    Friend WithEvents TabPage5 As TabPage
    Friend WithEvents ColumnHeader41 As ColumnHeader
    Friend WithEvents RBPayPalOrder As RadioButton
    Friend WithEvents BtPayOrder As Button
    Friend WithEvents TBManuMsg As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents BtReCreatePayPalOrder As Button
    Friend WithEvents BtSendMsg As Button
    Friend WithEvents ChBxEncMsg As CheckBox
    Friend WithEvents BtSaveSettings As Button
    Friend WithEvents NUDSNOAmount As NumericUpDown
    Friend WithEvents NUDSNOCollateral As NumericUpDown
    Friend WithEvents NUDSNOItemAmount As NumericUpDown
    Friend WithEvents NUDSNOTXFee As NumericUpDown
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents BtTestCreate As Button
    Friend WithEvents BtTestFinish As Button
    Friend WithEvents BtTestAccept As Button
    Friend WithEvents ColumnHeader16 As ColumnHeader
    Friend WithEvents ColumnHeader18 As ColumnHeader
    Friend WithEvents Label16 As Label
    Friend WithEvents LabXItem As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents BtTestConvert As Button
    Friend WithEvents TBTestConvert As TextBox
    Friend WithEvents BtTestConvert2 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents BtCheckPayPalBiz As Button
    Friend WithEvents CoBxNode As ComboBox
    Friend WithEvents CoBxRefresh As ComboBox
    Friend WithEvents Label19 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label20 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents BtTestPPAPI As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents ChBxCheckXItemTX As CheckBox
    Friend WithEvents BtTestSetTXINI As Button
    Friend WithEvents BtTestDelTXINI As Button
    Friend WithEvents BtTestGetTXINI As Button
    Friend WithEvents TBTestDelTXINI As TextBox
    Friend WithEvents TBTestGetTXINI As TextBox
    Friend WithEvents TBTestSetTXINI As TextBox
    Friend WithEvents RBUseXItemSettings As RadioButton
    Friend WithEvents RBUsePaymentInfo As RadioButton
End Class
