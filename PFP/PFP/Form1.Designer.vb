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
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Default Settings")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MyOrders Settings")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Develope")
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PFPForm))
        Me.BlockTimer = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.StatusBlockLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusFeeLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TSSStatusImage = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TSSCryptStatus = New System.Windows.Forms.ToolStripStatusLabel()
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
        Me.ColumnHeader34 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label3 = New System.Windows.Forms.Label()
        Me.BtSNOSetOrder = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.LabColPercentage = New System.Windows.Forms.Label()
        Me.TBarCollateralPercent = New System.Windows.Forms.TrackBar()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.LabXItem = New System.Windows.Forms.Label()
        Me.LabDealAmount = New System.Windows.Forms.Label()
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
        Me.TBSNOAddress = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.CoBxMarket = New System.Windows.Forms.ComboBox()
        Me.BtCreateNewAT = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer5 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer7 = New System.Windows.Forms.SplitContainer()
        Me.CoBxSellFilterMaxOrders = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtShowSellFilter = New System.Windows.Forms.Button()
        Me.SplitContainer8 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainerSellFilter = New System.Windows.Forms.SplitContainer()
        Me.ChBxSellFilterShowPayable = New System.Windows.Forms.CheckBox()
        Me.ChLBSellFilterMethods = New System.Windows.Forms.CheckedListBox()
        Me.ChBxSellFilterShowAutofinish = New System.Windows.Forms.CheckBox()
        Me.ChBxSellFilterShowAutoinfo = New System.Windows.Forms.CheckBox()
        Me.SplitContainer9 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer10 = New System.Windows.Forms.SplitContainer()
        Me.CoBxBuyFilterMaxOrders = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.BtShowBuyFilter = New System.Windows.Forms.Button()
        Me.SplitContainer11 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainerBuyFilter = New System.Windows.Forms.SplitContainer()
        Me.ChBxBuyFilterShowPayable = New System.Windows.Forms.CheckBox()
        Me.ChBxBuyFilterShowAutofinish = New System.Windows.Forms.CheckBox()
        Me.ChLBBuyFilterMethods = New System.Windows.Forms.CheckedListBox()
        Me.ChBxBuyFilterShowAutoinfo = New System.Windows.Forms.CheckBox()
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
        Me.SCSettings = New System.Windows.Forms.SplitContainer()
        Me.TVSettings = New System.Windows.Forms.TreeView()
        Me.SplitContainer12 = New System.Windows.Forms.SplitContainer()
        Me.BtSetPIN = New System.Windows.Forms.Button()
        Me.TBSNOBalance = New System.Windows.Forms.TextBox()
        Me.BtChartGFXOnOff = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.StatusStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.TBarCollateralPercent, System.ComponentModel.ISupportInitialize).BeginInit()
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
        CType(Me.SplitContainerSellFilter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerSellFilter.Panel1.SuspendLayout()
        Me.SplitContainerSellFilter.Panel2.SuspendLayout()
        Me.SplitContainerSellFilter.SuspendLayout()
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
        CType(Me.SplitContainerBuyFilter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerBuyFilter.Panel1.SuspendLayout()
        Me.SplitContainerBuyFilter.Panel2.SuspendLayout()
        Me.SplitContainerBuyFilter.SuspendLayout()
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
        CType(Me.SCSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SCSettings.Panel1.SuspendLayout()
        Me.SCSettings.SuspendLayout()
        CType(Me.SplitContainer12, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer12.Panel1.SuspendLayout()
        Me.SplitContainer12.Panel2.SuspendLayout()
        Me.SplitContainer12.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BlockTimer
        '
        Me.BlockTimer.Enabled = True
        Me.BlockTimer.Interval = 1000
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel, Me.StatusBar, Me.StatusBlockLabel, Me.StatusFeeLabel, Me.TSSStatusImage, Me.TSSCryptStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 734)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.ShowItemToolTips = True
        Me.StatusStrip1.Size = New System.Drawing.Size(1554, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(1349, 17)
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
        'TSSStatusImage
        '
        Me.TSSStatusImage.Image = Global.PFP.My.Resources.Resources.status_offline
        Me.TSSStatusImage.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.TSSStatusImage.Name = "TSSStatusImage"
        Me.TSSStatusImage.Size = New System.Drawing.Size(57, 17)
        Me.TSSStatusImage.Text = "offline"
        '
        'TSSCryptStatus
        '
        Me.TSSCryptStatus.AutoToolTip = True
        Me.TSSCryptStatus.Image = Global.PFP.My.Resources.Resources.status_decrypted
        Me.TSSCryptStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.TSSCryptStatus.Name = "TSSCryptStatus"
        Me.TSSCryptStatus.Size = New System.Drawing.Size(72, 17)
        Me.TSSCryptStatus.Text = "Click Me!"
        Me.TSSCryptStatus.ToolTipText = "test" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
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
        Me.LVSellorders.Size = New System.Drawing.Size(762, 127)
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
        Me.LVBuyorders.Size = New System.Drawing.Size(764, 127)
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
        Me.LVOpenChannels.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader9, Me.ColumnHeader34})
        Me.LVOpenChannels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVOpenChannels.ForeColor = System.Drawing.Color.White
        Me.LVOpenChannels.FullRowSelect = True
        Me.LVOpenChannels.GridLines = True
        Me.LVOpenChannels.HideSelection = False
        Me.LVOpenChannels.Location = New System.Drawing.Point(0, 0)
        Me.LVOpenChannels.MultiSelect = False
        Me.LVOpenChannels.Name = "LVOpenChannels"
        Me.LVOpenChannels.Size = New System.Drawing.Size(371, 63)
        Me.LVOpenChannels.TabIndex = 9
        Me.LVOpenChannels.UseCompatibleStateImageBehavior = False
        Me.LVOpenChannels.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "AT"
        Me.ColumnHeader9.Width = 114
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
        Me.BtSNOSetOrder.Location = New System.Drawing.Point(124, 225)
        Me.BtSNOSetOrder.Name = "BtSNOSetOrder"
        Me.BtSNOSetOrder.Size = New System.Drawing.Size(82, 23)
        Me.BtSNOSetOrder.TabIndex = 11
        Me.BtSNOSetOrder.Text = "Set Order"
        Me.BtSNOSetOrder.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.LabColPercentage)
        Me.GroupBox1.Controls.Add(Me.TBarCollateralPercent)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.LabXItem)
        Me.GroupBox1.Controls.Add(Me.LabDealAmount)
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
        Me.GroupBox1.MinimumSize = New System.Drawing.Size(371, 253)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(371, 263)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Set New Order"
        '
        'LabColPercentage
        '
        Me.LabColPercentage.AutoSize = True
        Me.LabColPercentage.Location = New System.Drawing.Point(8, 149)
        Me.LabColPercentage.Name = "LabColPercentage"
        Me.LabColPercentage.Size = New System.Drawing.Size(34, 13)
        Me.LabColPercentage.TabIndex = 33
        Me.LabColPercentage.Text = "30 %"
        '
        'TBarCollateralPercent
        '
        Me.TBarCollateralPercent.Cursor = System.Windows.Forms.Cursors.Default
        Me.TBarCollateralPercent.Location = New System.Drawing.Point(93, 92)
        Me.TBarCollateralPercent.Maximum = 11
        Me.TBarCollateralPercent.Name = "TBarCollateralPercent"
        Me.TBarCollateralPercent.Size = New System.Drawing.Size(266, 45)
        Me.TBarCollateralPercent.TabIndex = 32
        Me.TBarCollateralPercent.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.TBarCollateralPercent.Value = 1
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(212, 204)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(109, 13)
        Me.Label18.TabIndex = 31
        Me.Label18.Text = "Signa (0.0 = auto)"
        '
        'LabXItem
        '
        Me.LabXItem.AutoSize = True
        Me.LabXItem.Location = New System.Drawing.Point(212, 176)
        Me.LabXItem.Name = "LabXItem"
        Me.LabXItem.Size = New System.Drawing.Size(39, 13)
        Me.LabXItem.TabIndex = 30
        Me.LabXItem.Text = "XItem"
        '
        'LabDealAmount
        '
        Me.LabDealAmount.AutoSize = True
        Me.LabDealAmount.Location = New System.Drawing.Point(212, 70)
        Me.LabDealAmount.Name = "LabDealAmount"
        Me.LabDealAmount.Size = New System.Drawing.Size(141, 13)
        Me.LabDealAmount.TabIndex = 29
        Me.LabDealAmount.Text = "Signa (Max 100 on 0 %)"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(212, 149)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(64, 13)
        Me.Label16.TabIndex = 28
        Me.Label16.Text = "Loading..."
        '
        'NUDSNOTXFee
        '
        Me.NUDSNOTXFee.DecimalPlaces = 8
        Me.NUDSNOTXFee.Location = New System.Drawing.Point(93, 200)
        Me.NUDSNOTXFee.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.NUDSNOTXFee.Name = "NUDSNOTXFee"
        Me.NUDSNOTXFee.Size = New System.Drawing.Size(113, 20)
        Me.NUDSNOTXFee.TabIndex = 27
        '
        'NUDSNOItemAmount
        '
        Me.NUDSNOItemAmount.DecimalPlaces = 8
        Me.NUDSNOItemAmount.Location = New System.Drawing.Point(93, 172)
        Me.NUDSNOItemAmount.Maximum = New Decimal(New Integer() {-1, -1, -1, 0})
        Me.NUDSNOItemAmount.Name = "NUDSNOItemAmount"
        Me.NUDSNOItemAmount.Size = New System.Drawing.Size(113, 20)
        Me.NUDSNOItemAmount.TabIndex = 26
        '
        'NUDSNOCollateral
        '
        Me.NUDSNOCollateral.DecimalPlaces = 8
        Me.NUDSNOCollateral.Increment = New Decimal(New Integer() {5, 0, 0, 131072})
        Me.NUDSNOCollateral.Location = New System.Drawing.Point(93, 146)
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
        Me.BtSNOSetCurFee.Location = New System.Drawing.Point(331, 197)
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
        Me.Label7.Location = New System.Drawing.Point(6, 202)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(52, 13)
        Me.Label7.TabIndex = 16
        Me.Label7.Text = "TX-Fee:"
        '
        'LabXitemAmount
        '
        Me.LabXitemAmount.AutoSize = True
        Me.LabXitemAmount.Location = New System.Drawing.Point(6, 174)
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
        Me.BtCheckAddress.Location = New System.Drawing.Point(788, 2)
        Me.BtCheckAddress.Name = "BtCheckAddress"
        Me.BtCheckAddress.Size = New System.Drawing.Size(110, 22)
        Me.BtCheckAddress.TabIndex = 31
        Me.BtCheckAddress.Text = "Check Address"
        Me.BtCheckAddress.UseVisualStyleBackColor = True
        Me.BtCheckAddress.Visible = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(499, 6)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(49, 13)
        Me.Label11.TabIndex = 30
        Me.Label11.Text = "Balance:"
        '
        'TBSNOAddress
        '
        Me.TBSNOAddress.Location = New System.Drawing.Point(335, 3)
        Me.TBSNOAddress.Name = "TBSNOAddress"
        Me.TBSNOAddress.ReadOnly = True
        Me.TBSNOAddress.Size = New System.Drawing.Size(158, 20)
        Me.TBSNOAddress.TabIndex = 28
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(281, 6)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(48, 13)
        Me.Label10.TabIndex = 27
        Me.Label10.Text = "Address:"
        '
        'CoBxMarket
        '
        Me.CoBxMarket.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxMarket.FormattingEnabled = True
        Me.CoBxMarket.Items.AddRange(New Object() {"AUD", "BRL", "CAD", "CNY", "CZK", "DKK", "EUR", "HKD", "HUF", "INR", "ILS", "JPY", "MYR", "MXN", "TWD", "NZD", "NOK", "PHP", "PLN", "GBP", "RUB", "SGD", "SEK", "CHF", "THB", "USD"})
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
        Me.SplitContainer1.Size = New System.Drawing.Size(1540, 673)
        Me.SplitContainer1.SplitterDistance = 396
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
        Me.SplitContainer2.Size = New System.Drawing.Size(1540, 396)
        Me.SplitContainer2.SplitterDistance = 1163
        Me.SplitContainer2.TabIndex = 14
        '
        'SplitContainer3
        '
        Me.SplitContainer3.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.SplitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
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
        Me.SplitContainer3.Panel2.AutoScroll = True
        Me.SplitContainer3.Panel2.AutoScrollMinSize = New System.Drawing.Size(371, 254)
        Me.SplitContainer3.Panel2.Controls.Add(Me.GroupBox1)
        Me.SplitContainer3.Size = New System.Drawing.Size(373, 396)
        Me.SplitContainer3.SplitterDistance = 127
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
        Me.SplitContainer4.Size = New System.Drawing.Size(371, 125)
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
        Me.SplitContainer5.Size = New System.Drawing.Size(371, 96)
        Me.SplitContainer5.SplitterDistance = 63
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
        Me.SplitContainer6.Size = New System.Drawing.Size(1538, 271)
        Me.SplitContainer6.SplitterDistance = 766
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
        Me.SplitContainer7.Panel1.Controls.Add(Me.CoBxSellFilterMaxOrders)
        Me.SplitContainer7.Panel1.Controls.Add(Me.Label6)
        Me.SplitContainer7.Panel1.Controls.Add(Me.BtShowSellFilter)
        Me.SplitContainer7.Panel1.Controls.Add(Me.Label1)
        '
        'SplitContainer7.Panel2
        '
        Me.SplitContainer7.Panel2.Controls.Add(Me.SplitContainer8)
        Me.SplitContainer7.Size = New System.Drawing.Size(762, 267)
        Me.SplitContainer7.SplitterDistance = 25
        Me.SplitContainer7.TabIndex = 0
        '
        'CoBxSellFilterMaxOrders
        '
        Me.CoBxSellFilterMaxOrders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxSellFilterMaxOrders.FormattingEnabled = True
        Me.CoBxSellFilterMaxOrders.Items.AddRange(New Object() {"10", "20", "50", "100"})
        Me.CoBxSellFilterMaxOrders.Location = New System.Drawing.Point(218, 3)
        Me.CoBxSellFilterMaxOrders.Name = "CoBxSellFilterMaxOrders"
        Me.CoBxSellFilterMaxOrders.Size = New System.Drawing.Size(47, 21)
        Me.CoBxSellFilterMaxOrders.TabIndex = 9
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(148, 7)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(64, 13)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Max Orders:"
        '
        'BtShowSellFilter
        '
        Me.BtShowSellFilter.BackColor = System.Drawing.Color.Crimson
        Me.BtShowSellFilter.Location = New System.Drawing.Point(68, 2)
        Me.BtShowSellFilter.Name = "BtShowSellFilter"
        Me.BtShowSellFilter.Size = New System.Drawing.Size(75, 23)
        Me.BtShowSellFilter.TabIndex = 8
        Me.BtShowSellFilter.Text = "show Filter"
        Me.BtShowSellFilter.UseVisualStyleBackColor = False
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
        Me.SplitContainer8.Panel1.Controls.Add(Me.SplitContainerSellFilter)
        '
        'SplitContainer8.Panel2
        '
        Me.SplitContainer8.Panel2.Controls.Add(Me.SplitContainer9)
        Me.SplitContainer8.Size = New System.Drawing.Size(762, 238)
        Me.SplitContainer8.SplitterDistance = 209
        Me.SplitContainer8.TabIndex = 0
        '
        'SplitContainerSellFilter
        '
        Me.SplitContainerSellFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerSellFilter.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerSellFilter.Name = "SplitContainerSellFilter"
        Me.SplitContainerSellFilter.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerSellFilter.Panel1
        '
        Me.SplitContainerSellFilter.Panel1.AutoScroll = True
        Me.SplitContainerSellFilter.Panel1.Controls.Add(Me.ChBxSellFilterShowPayable)
        Me.SplitContainerSellFilter.Panel1.Controls.Add(Me.ChLBSellFilterMethods)
        Me.SplitContainerSellFilter.Panel1.Controls.Add(Me.ChBxSellFilterShowAutofinish)
        Me.SplitContainerSellFilter.Panel1.Controls.Add(Me.ChBxSellFilterShowAutoinfo)
        '
        'SplitContainerSellFilter.Panel2
        '
        Me.SplitContainerSellFilter.Panel2.Controls.Add(Me.LVSellorders)
        Me.SplitContainerSellFilter.Size = New System.Drawing.Size(762, 209)
        Me.SplitContainerSellFilter.SplitterDistance = 78
        Me.SplitContainerSellFilter.TabIndex = 3
        '
        'ChBxSellFilterShowPayable
        '
        Me.ChBxSellFilterShowPayable.AutoSize = True
        Me.ChBxSellFilterShowPayable.Location = New System.Drawing.Point(167, 49)
        Me.ChBxSellFilterShowPayable.Name = "ChBxSellFilterShowPayable"
        Me.ChBxSellFilterShowPayable.Size = New System.Drawing.Size(145, 17)
        Me.ChBxSellFilterShowPayable.TabIndex = 9
        Me.ChBxSellFilterShowPayable.Text = "show only what i can pay"
        Me.ChBxSellFilterShowPayable.UseVisualStyleBackColor = True
        '
        'ChLBSellFilterMethods
        '
        Me.ChLBSellFilterMethods.CheckOnClick = True
        Me.ChLBSellFilterMethods.FormattingEnabled = True
        Me.ChLBSellFilterMethods.Items.AddRange(New Object() {"Loading..."})
        Me.ChLBSellFilterMethods.Location = New System.Drawing.Point(3, 3)
        Me.ChLBSellFilterMethods.Name = "ChLBSellFilterMethods"
        Me.ChLBSellFilterMethods.Size = New System.Drawing.Size(158, 94)
        Me.ChLBSellFilterMethods.TabIndex = 8
        '
        'ChBxSellFilterShowAutofinish
        '
        Me.ChBxSellFilterShowAutofinish.AutoSize = True
        Me.ChBxSellFilterShowAutofinish.Location = New System.Drawing.Point(167, 26)
        Me.ChBxSellFilterShowAutofinish.Name = "ChBxSellFilterShowAutofinish"
        Me.ChBxSellFilterShowAutofinish.Size = New System.Drawing.Size(145, 17)
        Me.ChBxSellFilterShowAutofinish.TabIndex = 6
        Me.ChBxSellFilterShowAutofinish.Text = "show only autofinish=true"
        Me.ChBxSellFilterShowAutofinish.UseVisualStyleBackColor = True
        '
        'ChBxSellFilterShowAutoinfo
        '
        Me.ChBxSellFilterShowAutoinfo.AutoSize = True
        Me.ChBxSellFilterShowAutoinfo.Location = New System.Drawing.Point(167, 3)
        Me.ChBxSellFilterShowAutoinfo.Name = "ChBxSellFilterShowAutoinfo"
        Me.ChBxSellFilterShowAutoinfo.Size = New System.Drawing.Size(138, 17)
        Me.ChBxSellFilterShowAutoinfo.TabIndex = 5
        Me.ChBxSellFilterShowAutoinfo.Text = "show only autoinfo=true"
        Me.ChBxSellFilterShowAutoinfo.UseVisualStyleBackColor = True
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
        Me.SplitContainer9.Size = New System.Drawing.Size(762, 25)
        Me.SplitContainer9.SplitterDistance = 673
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
        Me.SplitContainer10.Panel1.Controls.Add(Me.CoBxBuyFilterMaxOrders)
        Me.SplitContainer10.Panel1.Controls.Add(Me.Label13)
        Me.SplitContainer10.Panel1.Controls.Add(Me.BtShowBuyFilter)
        Me.SplitContainer10.Panel1.Controls.Add(Me.Label2)
        '
        'SplitContainer10.Panel2
        '
        Me.SplitContainer10.Panel2.Controls.Add(Me.SplitContainer11)
        Me.SplitContainer10.Size = New System.Drawing.Size(764, 267)
        Me.SplitContainer10.SplitterDistance = 25
        Me.SplitContainer10.TabIndex = 0
        '
        'CoBxBuyFilterMaxOrders
        '
        Me.CoBxBuyFilterMaxOrders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxBuyFilterMaxOrders.FormattingEnabled = True
        Me.CoBxBuyFilterMaxOrders.Items.AddRange(New Object() {"10", "20", "50", "100"})
        Me.CoBxBuyFilterMaxOrders.Location = New System.Drawing.Point(217, 2)
        Me.CoBxBuyFilterMaxOrders.Name = "CoBxBuyFilterMaxOrders"
        Me.CoBxBuyFilterMaxOrders.Size = New System.Drawing.Size(47, 21)
        Me.CoBxBuyFilterMaxOrders.TabIndex = 10
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(147, 6)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(64, 13)
        Me.Label13.TabIndex = 11
        Me.Label13.Text = "Max Orders:"
        '
        'BtShowBuyFilter
        '
        Me.BtShowBuyFilter.BackColor = System.Drawing.Color.PaleGreen
        Me.BtShowBuyFilter.Location = New System.Drawing.Point(66, 1)
        Me.BtShowBuyFilter.Name = "BtShowBuyFilter"
        Me.BtShowBuyFilter.Size = New System.Drawing.Size(75, 23)
        Me.BtShowBuyFilter.TabIndex = 9
        Me.BtShowBuyFilter.Text = "show Filter"
        Me.BtShowBuyFilter.UseVisualStyleBackColor = False
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
        Me.SplitContainer11.Panel1.Controls.Add(Me.SplitContainerBuyFilter)
        '
        'SplitContainer11.Panel2
        '
        Me.SplitContainer11.Panel2.Controls.Add(Me.BtSell)
        Me.SplitContainer11.Size = New System.Drawing.Size(764, 238)
        Me.SplitContainer11.SplitterDistance = 209
        Me.SplitContainer11.TabIndex = 0
        '
        'SplitContainerBuyFilter
        '
        Me.SplitContainerBuyFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerBuyFilter.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerBuyFilter.Name = "SplitContainerBuyFilter"
        Me.SplitContainerBuyFilter.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerBuyFilter.Panel1
        '
        Me.SplitContainerBuyFilter.Panel1.AutoScroll = True
        Me.SplitContainerBuyFilter.Panel1.Controls.Add(Me.ChBxBuyFilterShowPayable)
        Me.SplitContainerBuyFilter.Panel1.Controls.Add(Me.ChBxBuyFilterShowAutofinish)
        Me.SplitContainerBuyFilter.Panel1.Controls.Add(Me.ChLBBuyFilterMethods)
        Me.SplitContainerBuyFilter.Panel1.Controls.Add(Me.ChBxBuyFilterShowAutoinfo)
        '
        'SplitContainerBuyFilter.Panel2
        '
        Me.SplitContainerBuyFilter.Panel2.Controls.Add(Me.LVBuyorders)
        Me.SplitContainerBuyFilter.Size = New System.Drawing.Size(764, 209)
        Me.SplitContainerBuyFilter.SplitterDistance = 78
        Me.SplitContainerBuyFilter.TabIndex = 6
        '
        'ChBxBuyFilterShowPayable
        '
        Me.ChBxBuyFilterShowPayable.AutoSize = True
        Me.ChBxBuyFilterShowPayable.Location = New System.Drawing.Point(167, 49)
        Me.ChBxBuyFilterShowPayable.Name = "ChBxBuyFilterShowPayable"
        Me.ChBxBuyFilterShowPayable.Size = New System.Drawing.Size(145, 17)
        Me.ChBxBuyFilterShowPayable.TabIndex = 10
        Me.ChBxBuyFilterShowPayable.Text = "show only what i can pay"
        Me.ChBxBuyFilterShowPayable.UseVisualStyleBackColor = True
        '
        'ChBxBuyFilterShowAutofinish
        '
        Me.ChBxBuyFilterShowAutofinish.AutoSize = True
        Me.ChBxBuyFilterShowAutofinish.Location = New System.Drawing.Point(167, 26)
        Me.ChBxBuyFilterShowAutofinish.Name = "ChBxBuyFilterShowAutofinish"
        Me.ChBxBuyFilterShowAutofinish.Size = New System.Drawing.Size(145, 17)
        Me.ChBxBuyFilterShowAutofinish.TabIndex = 9
        Me.ChBxBuyFilterShowAutofinish.Text = "show only autofinish=true"
        Me.ChBxBuyFilterShowAutofinish.UseVisualStyleBackColor = True
        '
        'ChLBBuyFilterMethods
        '
        Me.ChLBBuyFilterMethods.CheckOnClick = True
        Me.ChLBBuyFilterMethods.FormattingEnabled = True
        Me.ChLBBuyFilterMethods.Items.AddRange(New Object() {"Loading..."})
        Me.ChLBBuyFilterMethods.Location = New System.Drawing.Point(3, 3)
        Me.ChLBBuyFilterMethods.Name = "ChLBBuyFilterMethods"
        Me.ChLBBuyFilterMethods.Size = New System.Drawing.Size(158, 94)
        Me.ChLBBuyFilterMethods.TabIndex = 9
        '
        'ChBxBuyFilterShowAutoinfo
        '
        Me.ChBxBuyFilterShowAutoinfo.AutoSize = True
        Me.ChBxBuyFilterShowAutoinfo.Location = New System.Drawing.Point(167, 3)
        Me.ChBxBuyFilterShowAutoinfo.Name = "ChBxBuyFilterShowAutoinfo"
        Me.ChBxBuyFilterShowAutoinfo.Size = New System.Drawing.Size(138, 17)
        Me.ChBxBuyFilterShowAutoinfo.TabIndex = 8
        Me.ChBxBuyFilterShowAutoinfo.Text = "show only autoinfo=true"
        Me.ChBxBuyFilterShowAutoinfo.UseVisualStyleBackColor = True
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
        Me.TabControl1.Size = New System.Drawing.Size(1554, 705)
        Me.TabControl1.TabIndex = 15
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1546, 679)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Marketdetails"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.SplitContainer13)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1546, 679)
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
        Me.SplitContainer13.Size = New System.Drawing.Size(1540, 673)
        Me.SplitContainer13.SplitterDistance = 388
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
        Me.SplitContainer14.Size = New System.Drawing.Size(1538, 386)
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
        Me.SplitContainer16.Size = New System.Drawing.Size(1538, 357)
        Me.SplitContainer16.SplitterDistance = 274
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
        Me.LVMyOpenOrders.Size = New System.Drawing.Size(1538, 274)
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
        Me.SplitContainer15.Size = New System.Drawing.Size(1538, 279)
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
        Me.SplitContainer17.Size = New System.Drawing.Size(1538, 250)
        Me.SplitContainer17.SplitterDistance = 219
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
        Me.LVMyClosedOrders.Size = New System.Drawing.Size(1538, 219)
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
        Me.TabPage3.Controls.Add(Me.SCSettings)
        Me.TabPage3.ForeColor = System.Drawing.Color.White
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(1546, 679)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Settings"
        '
        'SCSettings
        '
        Me.SCSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SCSettings.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SCSettings.Location = New System.Drawing.Point(0, 0)
        Me.SCSettings.Name = "SCSettings"
        '
        'SCSettings.Panel1
        '
        Me.SCSettings.Panel1.Controls.Add(Me.TVSettings)
        Me.SCSettings.Size = New System.Drawing.Size(1546, 679)
        Me.SCSettings.SplitterDistance = 226
        Me.SCSettings.TabIndex = 10
        '
        'TVSettings
        '
        Me.TVSettings.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(87, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.TVSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TVSettings.ForeColor = System.Drawing.Color.White
        Me.TVSettings.Location = New System.Drawing.Point(0, 0)
        Me.TVSettings.Name = "TVSettings"
        TreeNode1.Name = "Defaults"
        TreeNode1.Text = "Default Settings"
        TreeNode2.Name = "MyOrderssettings"
        TreeNode2.Text = "MyOrders Settings"
        TreeNode3.Name = "Developements"
        TreeNode3.Text = "Develope"
        Me.TVSettings.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3})
        Me.TVSettings.Size = New System.Drawing.Size(226, 679)
        Me.TVSettings.TabIndex = 0
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
        Me.SplitContainer12.Panel1.AutoScroll = True
        Me.SplitContainer12.Panel1.Controls.Add(Me.BtSetPIN)
        Me.SplitContainer12.Panel1.Controls.Add(Me.TBSNOBalance)
        Me.SplitContainer12.Panel1.Controls.Add(Me.BtChartGFXOnOff)
        Me.SplitContainer12.Panel1.Controls.Add(Me.BtCheckAddress)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label11)
        Me.SplitContainer12.Panel1.Controls.Add(Me.CoBxMarket)
        Me.SplitContainer12.Panel1.Controls.Add(Me.TBSNOAddress)
        Me.SplitContainer12.Panel1.Controls.Add(Me.Label10)
        '
        'SplitContainer12.Panel2
        '
        Me.SplitContainer12.Panel2.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer12.Panel2.Controls.Add(Me.TabControl1)
        Me.SplitContainer12.Size = New System.Drawing.Size(1554, 734)
        Me.SplitContainer12.SplitterDistance = 25
        Me.SplitContainer12.TabIndex = 15
        '
        'BtSetPIN
        '
        Me.BtSetPIN.Location = New System.Drawing.Point(158, 1)
        Me.BtSetPIN.Name = "BtSetPIN"
        Me.BtSetPIN.Size = New System.Drawing.Size(117, 23)
        Me.BtSetPIN.TabIndex = 34
        Me.BtSetPIN.Text = "(un)set/change PIN"
        Me.BtSetPIN.UseVisualStyleBackColor = True
        '
        'TBSNOBalance
        '
        Me.TBSNOBalance.Location = New System.Drawing.Point(554, 3)
        Me.TBSNOBalance.Name = "TBSNOBalance"
        Me.TBSNOBalance.ReadOnly = True
        Me.TBSNOBalance.Size = New System.Drawing.Size(123, 20)
        Me.TBSNOBalance.TabIndex = 33
        Me.TBSNOBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BtChartGFXOnOff
        '
        Me.BtChartGFXOnOff.Location = New System.Drawing.Point(683, 2)
        Me.BtChartGFXOnOff.Name = "BtChartGFXOnOff"
        Me.BtChartGFXOnOff.Size = New System.Drawing.Size(99, 22)
        Me.BtChartGFXOnOff.TabIndex = 32
        Me.BtChartGFXOnOff.Text = "Chart GFX on/off"
        Me.BtChartGFXOnOff.UseVisualStyleBackColor = True
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
        'Panel1
        '
        Me.Panel1.AutoScroll = True
        Me.Panel1.Controls.Add(Me.SplitContainer12)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1554, 734)
        Me.Panel1.TabIndex = 16
        '
        'PFPForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1554, 756)
        Me.Controls.Add(Me.Panel1)
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
        CType(Me.TBarCollateralPercent, System.ComponentModel.ISupportInitialize).EndInit()
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
        Me.SplitContainerSellFilter.Panel1.ResumeLayout(False)
        Me.SplitContainerSellFilter.Panel1.PerformLayout()
        Me.SplitContainerSellFilter.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerSellFilter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerSellFilter.ResumeLayout(False)
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
        Me.SplitContainerBuyFilter.Panel1.ResumeLayout(False)
        Me.SplitContainerBuyFilter.Panel1.PerformLayout()
        Me.SplitContainerBuyFilter.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerBuyFilter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerBuyFilter.ResumeLayout(False)
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
        Me.SCSettings.Panel1.ResumeLayout(False)
        CType(Me.SCSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SCSettings.ResumeLayout(False)
        Me.SplitContainer12.Panel1.ResumeLayout(False)
        Me.SplitContainer12.Panel1.PerformLayout()
        Me.SplitContainer12.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer12, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer12.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
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
    Friend WithEvents Label3 As Label
    Friend WithEvents BtSNOSetOrder As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents CoBxMarket As ComboBox
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
    Friend WithEvents ColumnHeader41 As ColumnHeader
    Friend WithEvents BtPayOrder As Button
    Friend WithEvents TBManuMsg As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents BtReCreatePayPalOrder As Button
    Friend WithEvents BtSendMsg As Button
    Friend WithEvents ChBxEncMsg As CheckBox
    Friend WithEvents NUDSNOAmount As NumericUpDown
    Friend WithEvents NUDSNOCollateral As NumericUpDown
    Friend WithEvents NUDSNOItemAmount As NumericUpDown
    Friend WithEvents NUDSNOTXFee As NumericUpDown
    Friend WithEvents ColumnHeader16 As ColumnHeader
    Friend WithEvents ColumnHeader18 As ColumnHeader
    Friend WithEvents Label16 As Label
    Friend WithEvents LabXItem As Label
    Friend WithEvents LabDealAmount As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents TBarCollateralPercent As TrackBar
    Friend WithEvents LabColPercentage As Label
    Friend WithEvents SCSettings As SplitContainer
    Friend WithEvents TVSettings As TreeView
    Friend WithEvents BtChartGFXOnOff As Button
    Friend WithEvents ChBxSellFilterShowAutofinish As CheckBox
    Friend WithEvents ChBxSellFilterShowAutoinfo As CheckBox
    Friend WithEvents ChBxBuyFilterShowAutofinish As CheckBox
    Friend WithEvents ChBxBuyFilterShowAutoinfo As CheckBox
    Friend WithEvents CoBxSellFilterMaxOrders As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents BtShowSellFilter As Button
    Friend WithEvents SplitContainerSellFilter As SplitContainer
    Friend WithEvents ChLBSellFilterMethods As CheckedListBox
    Friend WithEvents CoBxBuyFilterMaxOrders As ComboBox
    Friend WithEvents Label13 As Label
    Friend WithEvents BtShowBuyFilter As Button
    Friend WithEvents SplitContainerBuyFilter As SplitContainer
    Friend WithEvents ChLBBuyFilterMethods As CheckedListBox
    Friend WithEvents ChBxSellFilterShowPayable As CheckBox
    Friend WithEvents ChBxBuyFilterShowPayable As CheckBox
    Friend WithEvents TSSStatusImage As ToolStripStatusLabel
    Friend WithEvents TBSNOBalance As TextBox
    Friend WithEvents BtSetPIN As Button
    Friend WithEvents TSSCryptStatus As ToolStripStatusLabel
    Friend WithEvents Panel1 As Panel
End Class
