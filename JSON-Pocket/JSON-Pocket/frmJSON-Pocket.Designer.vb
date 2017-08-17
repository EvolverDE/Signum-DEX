<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmJSONPocket
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmJSONPocket))
        Me.HomeStrip = New System.Windows.Forms.MenuStrip()
        Me.FileTSMI = New System.Windows.Forms.ToolStripMenuItem()
        Me.refreshpeers = New System.Windows.Forms.ToolStripMenuItem()
        Me.unlock = New System.Windows.Forms.ToolStripMenuItem()
        Me.TBPassPhrase = New System.Windows.Forms.ToolStripTextBox()
        Me.lock = New System.Windows.Forms.ToolStripMenuItem()
        Me.exitprog = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsTSMI = New System.Windows.Forms.ToolStripMenuItem()
        Me.PeerTSMI = New System.Windows.Forms.ToolStripMenuItem()
        Me.refreshpeer = New System.Windows.Forms.ToolStripMenuItem()
        Me.autoconnect = New System.Windows.Forms.ToolStripMenuItem()
        Me.NodeURLs = New System.Windows.Forms.ToolStripComboBox()
        Me.AddressTSMI = New System.Windows.Forms.ToolStripMenuItem()
        Me.refreshaddressinfo = New System.Windows.Forms.ToolStripMenuItem()
        Me.TBaddress = New System.Windows.Forms.ToolStripTextBox()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.WalletVersion = New System.Windows.Forms.ToolStripStatusLabel()
        Me.WalletPeers = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TSProgressBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.TSSLabOn = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TSSLabSec = New System.Windows.Forms.ToolStripStatusLabel()
        Me.WLock = New System.Windows.Forms.ToolStripStatusLabel()
        Me.WalletPanel = New System.Windows.Forms.Panel()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabOverview = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.GrpBxAccInfo = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtAccVerify = New System.Windows.Forms.Button()
        Me.LabNewAcc = New System.Windows.Forms.Label()
        Me.BtSetName = New System.Windows.Forms.Button()
        Me.LabName = New System.Windows.Forms.Label()
        Me.TBName = New System.Windows.Forms.TextBox()
        Me.LabNumAcc = New System.Windows.Forms.Label()
        Me.TBNumAdd = New System.Windows.Forms.TextBox()
        Me.TBPubKey = New System.Windows.Forms.TextBox()
        Me.LabPubKey = New System.Windows.Forms.Label()
        Me.GrpBxMiningInfo = New System.Windows.Forms.GroupBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.CoBxRewAss = New System.Windows.Forms.ComboBox()
        Me.BtSetRewAss = New System.Windows.Forms.Button()
        Me.LabGenBlocks = New System.Windows.Forms.Label()
        Me.LabRewAss = New System.Windows.Forms.Label()
        Me.TBMinedBlocks = New System.Windows.Forms.TextBox()
        Me.TBAssistedTo = New System.Windows.Forms.TextBox()
        Me.GrpBxBalancesInfo = New System.Windows.Forms.GroupBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.LGenBal = New System.Windows.Forms.TextBox()
        Me.LUnBal = New System.Windows.Forms.TextBox()
        Me.LSumBal = New System.Windows.Forms.TextBox()
        Me.LConBal = New System.Windows.Forms.TextBox()
        Me.TabSend = New System.Windows.Forms.TabPage()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.GrpBxSendTo = New System.Windows.Forms.GroupBox()
        Me.BtSend = New System.Windows.Forms.Button()
        Me.TBAmount = New System.Windows.Forms.TextBox()
        Me.LabAmount = New System.Windows.Forms.Label()
        Me.TBRecipient = New System.Windows.Forms.TextBox()
        Me.LabRecipient = New System.Windows.Forms.Label()
        Me.GrpBxUnConfTrans = New System.Windows.Forms.GroupBox()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LabFee = New System.Windows.Forms.Label()
        Me.TBFee = New System.Windows.Forms.TextBox()
        Me.BtGetFeeInfo = New System.Windows.Forms.Button()
        Me.LUnConTransBytes = New System.Windows.Forms.TextBox()
        Me.LSumFee = New System.Windows.Forms.TextBox()
        Me.LBigFee = New System.Windows.Forms.TextBox()
        Me.LAvgFee = New System.Windows.Forms.TextBox()
        Me.LUnConfTrans = New System.Windows.Forms.TextBox()
        Me.TabTransacts = New System.Windows.Forms.TabPage()
        Me.LVTrans = New System.Windows.Forms.ListView()
        Me.BlockID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TransID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Confirms = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Sender = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Recipient = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Type = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Amount = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Fee = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.HomeStrip.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        Me.WalletPanel.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabOverview.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.GrpBxAccInfo.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.GrpBxMiningInfo.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.GrpBxBalancesInfo.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.TabSend.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.GrpBxSendTo.SuspendLayout()
        Me.GrpBxUnConfTrans.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        Me.TabTransacts.SuspendLayout()
        Me.SuspendLayout()
        '
        'HomeStrip
        '
        Me.HomeStrip.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.HomeStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileTSMI, Me.SettingsTSMI, Me.PeerTSMI, Me.NodeURLs, Me.AddressTSMI, Me.TBaddress})
        Me.HomeStrip.Location = New System.Drawing.Point(0, 0)
        Me.HomeStrip.Name = "HomeStrip"
        Me.HomeStrip.Size = New System.Drawing.Size(685, 27)
        Me.HomeStrip.TabIndex = 2
        Me.HomeStrip.Text = "MenuStrip"
        '
        'FileTSMI
        '
        Me.FileTSMI.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.refreshpeers, Me.unlock, Me.lock, Me.exitprog})
        Me.FileTSMI.Name = "FileTSMI"
        Me.FileTSMI.Size = New System.Drawing.Size(64, 23)
        Me.FileTSMI.Text = "FileTSMI"
        '
        'refreshpeers
        '
        Me.refreshpeers.Name = "refreshpeers"
        Me.refreshpeers.Size = New System.Drawing.Size(138, 22)
        Me.refreshpeers.Text = "refreshpeers"
        '
        'unlock
        '
        Me.unlock.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TBPassPhrase})
        Me.unlock.Name = "unlock"
        Me.unlock.Size = New System.Drawing.Size(138, 22)
        Me.unlock.Text = "unlock"
        '
        'TBPassPhrase
        '
        Me.TBPassPhrase.Name = "TBPassPhrase"
        Me.TBPassPhrase.Size = New System.Drawing.Size(100, 23)
        Me.TBPassPhrase.Text = "PassPhrase"
        '
        'lock
        '
        Me.lock.Name = "lock"
        Me.lock.Size = New System.Drawing.Size(138, 22)
        Me.lock.Text = "lock"
        Me.lock.Visible = False
        '
        'exitprog
        '
        Me.exitprog.Name = "exitprog"
        Me.exitprog.Size = New System.Drawing.Size(138, 22)
        Me.exitprog.Text = "exitprog"
        '
        'SettingsTSMI
        '
        Me.SettingsTSMI.Name = "SettingsTSMI"
        Me.SettingsTSMI.Size = New System.Drawing.Size(88, 23)
        Me.SettingsTSMI.Text = "SettingsTSMI"
        '
        'PeerTSMI
        '
        Me.PeerTSMI.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.refreshpeer, Me.autoconnect})
        Me.PeerTSMI.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PeerTSMI.ForeColor = System.Drawing.SystemColors.ControlText
        Me.PeerTSMI.Name = "PeerTSMI"
        Me.PeerTSMI.Size = New System.Drawing.Size(69, 23)
        Me.PeerTSMI.Text = "PeerTSMI"
        '
        'refreshpeer
        '
        Me.refreshpeer.Name = "refreshpeer"
        Me.refreshpeer.Size = New System.Drawing.Size(141, 22)
        Me.refreshpeer.Text = "refreshpeer"
        '
        'autoconnect
        '
        Me.autoconnect.Name = "autoconnect"
        Me.autoconnect.Size = New System.Drawing.Size(141, 22)
        Me.autoconnect.Text = "autoconnect"
        '
        'NodeURLs
        '
        Me.NodeURLs.DropDownWidth = 300
        Me.NodeURLs.Items.AddRange(New Object() {"http://Localhost:8125"})
        Me.NodeURLs.Name = "NodeURLs"
        Me.NodeURLs.Size = New System.Drawing.Size(168, 23)
        '
        'AddressTSMI
        '
        Me.AddressTSMI.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.refreshaddressinfo})
        Me.AddressTSMI.Name = "AddressTSMI"
        Me.AddressTSMI.Size = New System.Drawing.Size(88, 23)
        Me.AddressTSMI.Text = "AddressTSMI"
        '
        'refreshaddressinfo
        '
        Me.refreshaddressinfo.Name = "refreshaddressinfo"
        Me.refreshaddressinfo.Size = New System.Drawing.Size(171, 22)
        Me.refreshaddressinfo.Text = "refreshaddressinfo"
        '
        'TBaddress
        '
        Me.TBaddress.Name = "TBaddress"
        Me.TBaddress.Size = New System.Drawing.Size(201, 23)
        '
        'StatusStrip
        '
        Me.StatusStrip.BackColor = System.Drawing.SystemColors.Control
        Me.StatusStrip.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.WalletVersion, Me.WalletPeers, Me.ToolStripStatusLabel2, Me.TSProgressBar, Me.TSSLabOn, Me.TSSLabSec, Me.WLock})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 375)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(685, 22)
        Me.StatusStrip.TabIndex = 3
        Me.StatusStrip.Text = "StatusStrip"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'WalletVersion
        '
        Me.WalletVersion.Name = "WalletVersion"
        Me.WalletVersion.Size = New System.Drawing.Size(78, 17)
        Me.WalletVersion.Text = "WalletVersion"
        '
        'WalletPeers
        '
        Me.WalletPeers.Name = "WalletPeers"
        Me.WalletPeers.Size = New System.Drawing.Size(68, 17)
        Me.WalletPeers.Text = "WalletPeers"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(355, 17)
        Me.ToolStripStatusLabel2.Spring = True
        '
        'TSProgressBar
        '
        Me.TSProgressBar.Name = "TSProgressBar"
        Me.TSProgressBar.Size = New System.Drawing.Size(100, 16)
        Me.TSProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.TSProgressBar.Visible = False
        '
        'TSSLabOn
        '
        Me.TSSLabOn.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TSSLabOn.Name = "TSSLabOn"
        Me.TSSLabOn.Size = New System.Drawing.Size(63, 17)
        Me.TSSLabOn.Text = "TSSLabOn"
        '
        'TSSLabSec
        '
        Me.TSSLabSec.Name = "TSSLabSec"
        Me.TSSLabSec.Size = New System.Drawing.Size(63, 17)
        Me.TSSLabSec.Text = "TSSLabSec"
        '
        'WLock
        '
        Me.WLock.Name = "WLock"
        Me.WLock.Size = New System.Drawing.Size(43, 17)
        Me.WLock.Text = "WLock"
        '
        'WalletPanel
        '
        Me.WalletPanel.BackColor = System.Drawing.SystemColors.Control
        Me.WalletPanel.Controls.Add(Me.TabControl1)
        Me.WalletPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WalletPanel.Location = New System.Drawing.Point(0, 27)
        Me.WalletPanel.Name = "WalletPanel"
        Me.WalletPanel.Size = New System.Drawing.Size(685, 348)
        Me.WalletPanel.TabIndex = 4
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabOverview)
        Me.TabControl1.Controls.Add(Me.TabSend)
        Me.TabControl1.Controls.Add(Me.TabTransacts)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(685, 348)
        Me.TabControl1.TabIndex = 8
        '
        'TabOverview
        '
        Me.TabOverview.BackColor = System.Drawing.SystemColors.Control
        Me.TabOverview.Controls.Add(Me.SplitContainer1)
        Me.TabOverview.Location = New System.Drawing.Point(4, 22)
        Me.TabOverview.Name = "TabOverview"
        Me.TabOverview.Padding = New System.Windows.Forms.Padding(3)
        Me.TabOverview.Size = New System.Drawing.Size(677, 322)
        Me.TabOverview.TabIndex = 0
        Me.TabOverview.Text = "TabOverview"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
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
        Me.SplitContainer1.Panel2.Controls.Add(Me.GrpBxBalancesInfo)
        Me.SplitContainer1.Size = New System.Drawing.Size(671, 316)
        Me.SplitContainer1.SplitterDistance = 255
        Me.SplitContainer1.TabIndex = 9
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.GrpBxAccInfo)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.GrpBxMiningInfo)
        Me.SplitContainer2.Size = New System.Drawing.Size(671, 255)
        Me.SplitContainer2.SplitterDistance = 460
        Me.SplitContainer2.TabIndex = 0
        '
        'GrpBxAccInfo
        '
        Me.GrpBxAccInfo.Controls.Add(Me.Panel1)
        Me.GrpBxAccInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GrpBxAccInfo.Location = New System.Drawing.Point(0, 0)
        Me.GrpBxAccInfo.Margin = New System.Windows.Forms.Padding(2)
        Me.GrpBxAccInfo.Name = "GrpBxAccInfo"
        Me.GrpBxAccInfo.Padding = New System.Windows.Forms.Padding(2)
        Me.GrpBxAccInfo.Size = New System.Drawing.Size(456, 251)
        Me.GrpBxAccInfo.TabIndex = 7
        Me.GrpBxAccInfo.TabStop = False
        Me.GrpBxAccInfo.Text = "GrpBxAccInfo"
        '
        'Panel1
        '
        Me.Panel1.AutoScroll = True
        Me.Panel1.BackColor = System.Drawing.Color.Transparent
        Me.Panel1.Controls.Add(Me.BtAccVerify)
        Me.Panel1.Controls.Add(Me.LabNewAcc)
        Me.Panel1.Controls.Add(Me.BtSetName)
        Me.Panel1.Controls.Add(Me.LabName)
        Me.Panel1.Controls.Add(Me.TBName)
        Me.Panel1.Controls.Add(Me.LabNumAcc)
        Me.Panel1.Controls.Add(Me.TBNumAdd)
        Me.Panel1.Controls.Add(Me.TBPubKey)
        Me.Panel1.Controls.Add(Me.LabPubKey)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(2, 15)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(452, 234)
        Me.Panel1.TabIndex = 12
        '
        'BtAccVerify
        '
        Me.BtAccVerify.Location = New System.Drawing.Point(201, -1)
        Me.BtAccVerify.Name = "BtAccVerify"
        Me.BtAccVerify.Size = New System.Drawing.Size(88, 22)
        Me.BtAccVerify.TabIndex = 14
        Me.BtAccVerify.Text = "BtAccVerify"
        Me.BtAccVerify.UseVisualStyleBackColor = True
        Me.BtAccVerify.Visible = False
        '
        'LabNewAcc
        '
        Me.LabNewAcc.AutoSize = True
        Me.LabNewAcc.ForeColor = System.Drawing.Color.Green
        Me.LabNewAcc.Location = New System.Drawing.Point(2, 3)
        Me.LabNewAcc.Name = "LabNewAcc"
        Me.LabNewAcc.Size = New System.Drawing.Size(75, 13)
        Me.LabNewAcc.TabIndex = 13
        Me.LabNewAcc.Text = "LabNewAcc"
        '
        'BtSetName
        '
        Me.BtSetName.Location = New System.Drawing.Point(201, 35)
        Me.BtSetName.Name = "BtSetName"
        Me.BtSetName.Size = New System.Drawing.Size(88, 22)
        Me.BtSetName.TabIndex = 12
        Me.BtSetName.Text = "BtSetName"
        Me.BtSetName.UseVisualStyleBackColor = True
        '
        'LabName
        '
        Me.LabName.AutoSize = True
        Me.LabName.Location = New System.Drawing.Point(2, 18)
        Me.LabName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabName.Name = "LabName"
        Me.LabName.Size = New System.Drawing.Size(60, 13)
        Me.LabName.TabIndex = 10
        Me.LabName.Text = "LabName"
        '
        'TBName
        '
        Me.TBName.BackColor = System.Drawing.SystemColors.Window
        Me.TBName.Location = New System.Drawing.Point(2, 36)
        Me.TBName.Margin = New System.Windows.Forms.Padding(2)
        Me.TBName.Name = "TBName"
        Me.TBName.Size = New System.Drawing.Size(194, 20)
        Me.TBName.TabIndex = 11
        '
        'LabNumAcc
        '
        Me.LabNumAcc.AutoSize = True
        Me.LabNumAcc.Location = New System.Drawing.Point(3, 58)
        Me.LabNumAcc.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabNumAcc.Name = "LabNumAcc"
        Me.LabNumAcc.Size = New System.Drawing.Size(75, 13)
        Me.LabNumAcc.TabIndex = 1
        Me.LabNumAcc.Text = "LabNumAcc"
        '
        'TBNumAdd
        '
        Me.TBNumAdd.Location = New System.Drawing.Point(3, 76)
        Me.TBNumAdd.Margin = New System.Windows.Forms.Padding(2)
        Me.TBNumAdd.Name = "TBNumAdd"
        Me.TBNumAdd.ReadOnly = True
        Me.TBNumAdd.Size = New System.Drawing.Size(193, 20)
        Me.TBNumAdd.TabIndex = 5
        '
        'TBPubKey
        '
        Me.TBPubKey.Location = New System.Drawing.Point(3, 116)
        Me.TBPubKey.Margin = New System.Windows.Forms.Padding(2)
        Me.TBPubKey.Name = "TBPubKey"
        Me.TBPubKey.ReadOnly = True
        Me.TBPubKey.Size = New System.Drawing.Size(286, 20)
        Me.TBPubKey.TabIndex = 9
        '
        'LabPubKey
        '
        Me.LabPubKey.AutoSize = True
        Me.LabPubKey.Location = New System.Drawing.Point(3, 98)
        Me.LabPubKey.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabPubKey.Name = "LabPubKey"
        Me.LabPubKey.Size = New System.Drawing.Size(71, 13)
        Me.LabPubKey.TabIndex = 8
        Me.LabPubKey.Text = "LabPubKey"
        '
        'GrpBxMiningInfo
        '
        Me.GrpBxMiningInfo.Controls.Add(Me.Panel2)
        Me.GrpBxMiningInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GrpBxMiningInfo.Location = New System.Drawing.Point(0, 0)
        Me.GrpBxMiningInfo.Margin = New System.Windows.Forms.Padding(2)
        Me.GrpBxMiningInfo.Name = "GrpBxMiningInfo"
        Me.GrpBxMiningInfo.Padding = New System.Windows.Forms.Padding(2)
        Me.GrpBxMiningInfo.Size = New System.Drawing.Size(203, 251)
        Me.GrpBxMiningInfo.TabIndex = 8
        Me.GrpBxMiningInfo.TabStop = False
        Me.GrpBxMiningInfo.Text = "GrpBxMiningInfo"
        '
        'Panel2
        '
        Me.Panel2.AutoScroll = True
        Me.Panel2.BackColor = System.Drawing.Color.Transparent
        Me.Panel2.Controls.Add(Me.CoBxRewAss)
        Me.Panel2.Controls.Add(Me.BtSetRewAss)
        Me.Panel2.Controls.Add(Me.LabGenBlocks)
        Me.Panel2.Controls.Add(Me.LabRewAss)
        Me.Panel2.Controls.Add(Me.TBMinedBlocks)
        Me.Panel2.Controls.Add(Me.TBAssistedTo)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(2, 15)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(199, 234)
        Me.Panel2.TabIndex = 8
        '
        'CoBxRewAss
        '
        Me.CoBxRewAss.BackColor = System.Drawing.SystemColors.Window
        Me.CoBxRewAss.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxRewAss.FormattingEnabled = True
        Me.CoBxRewAss.Location = New System.Drawing.Point(4, 63)
        Me.CoBxRewAss.Name = "CoBxRewAss"
        Me.CoBxRewAss.Size = New System.Drawing.Size(315, 21)
        Me.CoBxRewAss.TabIndex = 9
        '
        'BtSetRewAss
        '
        Me.BtSetRewAss.Location = New System.Drawing.Point(233, 37)
        Me.BtSetRewAss.Name = "BtSetRewAss"
        Me.BtSetRewAss.Size = New System.Drawing.Size(86, 22)
        Me.BtSetRewAss.TabIndex = 8
        Me.BtSetRewAss.Text = "BtSetRewAss"
        Me.BtSetRewAss.UseVisualStyleBackColor = True
        '
        'LabGenBlocks
        '
        Me.LabGenBlocks.AutoSize = True
        Me.LabGenBlocks.Location = New System.Drawing.Point(4, 5)
        Me.LabGenBlocks.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabGenBlocks.Name = "LabGenBlocks"
        Me.LabGenBlocks.Size = New System.Drawing.Size(89, 13)
        Me.LabGenBlocks.TabIndex = 2
        Me.LabGenBlocks.Text = "LabGenBlocks"
        '
        'LabRewAss
        '
        Me.LabRewAss.AutoSize = True
        Me.LabRewAss.Location = New System.Drawing.Point(4, 20)
        Me.LabRewAss.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabRewAss.Name = "LabRewAss"
        Me.LabRewAss.Size = New System.Drawing.Size(73, 13)
        Me.LabRewAss.TabIndex = 3
        Me.LabRewAss.Text = "LabRewAss"
        '
        'TBMinedBlocks
        '
        Me.TBMinedBlocks.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TBMinedBlocks.Location = New System.Drawing.Point(117, 5)
        Me.TBMinedBlocks.Margin = New System.Windows.Forms.Padding(2)
        Me.TBMinedBlocks.Name = "TBMinedBlocks"
        Me.TBMinedBlocks.ReadOnly = True
        Me.TBMinedBlocks.Size = New System.Drawing.Size(111, 13)
        Me.TBMinedBlocks.TabIndex = 6
        '
        'TBAssistedTo
        '
        Me.TBAssistedTo.BackColor = System.Drawing.SystemColors.Window
        Me.TBAssistedTo.Location = New System.Drawing.Point(4, 38)
        Me.TBAssistedTo.Margin = New System.Windows.Forms.Padding(2)
        Me.TBAssistedTo.Name = "TBAssistedTo"
        Me.TBAssistedTo.Size = New System.Drawing.Size(224, 20)
        Me.TBAssistedTo.TabIndex = 7
        '
        'GrpBxBalancesInfo
        '
        Me.GrpBxBalancesInfo.Controls.Add(Me.Panel3)
        Me.GrpBxBalancesInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GrpBxBalancesInfo.Location = New System.Drawing.Point(0, 0)
        Me.GrpBxBalancesInfo.Margin = New System.Windows.Forms.Padding(2)
        Me.GrpBxBalancesInfo.Name = "GrpBxBalancesInfo"
        Me.GrpBxBalancesInfo.Padding = New System.Windows.Forms.Padding(2)
        Me.GrpBxBalancesInfo.Size = New System.Drawing.Size(667, 53)
        Me.GrpBxBalancesInfo.TabIndex = 6
        Me.GrpBxBalancesInfo.TabStop = False
        Me.GrpBxBalancesInfo.Text = "GrpBxBalancesInfo"
        '
        'Panel3
        '
        Me.Panel3.AutoScroll = True
        Me.Panel3.BackColor = System.Drawing.Color.Transparent
        Me.Panel3.Controls.Add(Me.LGenBal)
        Me.Panel3.Controls.Add(Me.LUnBal)
        Me.Panel3.Controls.Add(Me.LSumBal)
        Me.Panel3.Controls.Add(Me.LConBal)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(2, 15)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(663, 36)
        Me.Panel3.TabIndex = 6
        '
        'LGenBal
        '
        Me.LGenBal.BackColor = System.Drawing.SystemColors.Control
        Me.LGenBal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LGenBal.Dock = System.Windows.Forms.DockStyle.Top
        Me.LGenBal.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LGenBal.Location = New System.Drawing.Point(0, 26)
        Me.LGenBal.Name = "LGenBal"
        Me.LGenBal.ReadOnly = True
        Me.LGenBal.Size = New System.Drawing.Size(646, 13)
        Me.LGenBal.TabIndex = 9
        Me.LGenBal.Text = "LGenBal"
        '
        'LUnBal
        '
        Me.LUnBal.BackColor = System.Drawing.SystemColors.Control
        Me.LUnBal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LUnBal.Dock = System.Windows.Forms.DockStyle.Top
        Me.LUnBal.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LUnBal.Location = New System.Drawing.Point(0, 13)
        Me.LUnBal.Name = "LUnBal"
        Me.LUnBal.ReadOnly = True
        Me.LUnBal.Size = New System.Drawing.Size(646, 13)
        Me.LUnBal.TabIndex = 8
        Me.LUnBal.Text = "LUnBal"
        '
        'LSumBal
        '
        Me.LSumBal.BackColor = System.Drawing.SystemColors.Control
        Me.LSumBal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LSumBal.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.LSumBal.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LSumBal.Location = New System.Drawing.Point(0, 39)
        Me.LSumBal.Name = "LSumBal"
        Me.LSumBal.ReadOnly = True
        Me.LSumBal.Size = New System.Drawing.Size(646, 19)
        Me.LSumBal.TabIndex = 7
        Me.LSumBal.Text = "LSumBal"
        '
        'LConBal
        '
        Me.LConBal.BackColor = System.Drawing.SystemColors.Control
        Me.LConBal.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LConBal.Dock = System.Windows.Forms.DockStyle.Top
        Me.LConBal.Location = New System.Drawing.Point(0, 0)
        Me.LConBal.Name = "LConBal"
        Me.LConBal.ReadOnly = True
        Me.LConBal.Size = New System.Drawing.Size(646, 13)
        Me.LConBal.TabIndex = 6
        Me.LConBal.Text = "LConBal"
        '
        'TabSend
        '
        Me.TabSend.BackColor = System.Drawing.SystemColors.Control
        Me.TabSend.Controls.Add(Me.SplitContainer3)
        Me.TabSend.Location = New System.Drawing.Point(4, 22)
        Me.TabSend.Name = "TabSend"
        Me.TabSend.Padding = New System.Windows.Forms.Padding(3)
        Me.TabSend.Size = New System.Drawing.Size(677, 322)
        Me.TabSend.TabIndex = 1
        Me.TabSend.Text = "TabSend"
        '
        'SplitContainer3
        '
        Me.SplitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer3.Margin = New System.Windows.Forms.Padding(2)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.GrpBxSendTo)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.GrpBxUnConfTrans)
        Me.SplitContainer3.Size = New System.Drawing.Size(671, 316)
        Me.SplitContainer3.SplitterDistance = 187
        Me.SplitContainer3.SplitterWidth = 3
        Me.SplitContainer3.TabIndex = 0
        '
        'GrpBxSendTo
        '
        Me.GrpBxSendTo.Controls.Add(Me.BtSend)
        Me.GrpBxSendTo.Controls.Add(Me.TBAmount)
        Me.GrpBxSendTo.Controls.Add(Me.LabAmount)
        Me.GrpBxSendTo.Controls.Add(Me.TBRecipient)
        Me.GrpBxSendTo.Controls.Add(Me.LabRecipient)
        Me.GrpBxSendTo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GrpBxSendTo.Location = New System.Drawing.Point(0, 0)
        Me.GrpBxSendTo.Margin = New System.Windows.Forms.Padding(2)
        Me.GrpBxSendTo.Name = "GrpBxSendTo"
        Me.GrpBxSendTo.Padding = New System.Windows.Forms.Padding(2)
        Me.GrpBxSendTo.Size = New System.Drawing.Size(667, 183)
        Me.GrpBxSendTo.TabIndex = 0
        Me.GrpBxSendTo.TabStop = False
        Me.GrpBxSendTo.Text = "GrpBxSendTo"
        '
        'BtSend
        '
        Me.BtSend.Location = New System.Drawing.Point(173, 69)
        Me.BtSend.Margin = New System.Windows.Forms.Padding(2)
        Me.BtSend.Name = "BtSend"
        Me.BtSend.Size = New System.Drawing.Size(100, 22)
        Me.BtSend.TabIndex = 4
        Me.BtSend.Text = "BtSend"
        Me.BtSend.UseVisualStyleBackColor = True
        '
        'TBAmount
        '
        Me.TBAmount.Location = New System.Drawing.Point(7, 70)
        Me.TBAmount.Margin = New System.Windows.Forms.Padding(2)
        Me.TBAmount.Name = "TBAmount"
        Me.TBAmount.Size = New System.Drawing.Size(164, 20)
        Me.TBAmount.TabIndex = 3
        '
        'LabAmount
        '
        Me.LabAmount.AutoSize = True
        Me.LabAmount.Location = New System.Drawing.Point(4, 55)
        Me.LabAmount.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabAmount.Name = "LabAmount"
        Me.LabAmount.Size = New System.Drawing.Size(70, 13)
        Me.LabAmount.TabIndex = 2
        Me.LabAmount.Text = "LabAmount"
        '
        'TBRecipient
        '
        Me.TBRecipient.Location = New System.Drawing.Point(7, 29)
        Me.TBRecipient.Margin = New System.Windows.Forms.Padding(2)
        Me.TBRecipient.MaxLength = 26
        Me.TBRecipient.Name = "TBRecipient"
        Me.TBRecipient.Size = New System.Drawing.Size(268, 20)
        Me.TBRecipient.TabIndex = 1
        '
        'LabRecipient
        '
        Me.LabRecipient.AutoSize = True
        Me.LabRecipient.Location = New System.Drawing.Point(4, 14)
        Me.LabRecipient.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabRecipient.Name = "LabRecipient"
        Me.LabRecipient.Size = New System.Drawing.Size(82, 13)
        Me.LabRecipient.TabIndex = 0
        Me.LabRecipient.Text = "LabRecipient"
        '
        'GrpBxUnConfTrans
        '
        Me.GrpBxUnConfTrans.Controls.Add(Me.SplitContainer4)
        Me.GrpBxUnConfTrans.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GrpBxUnConfTrans.Location = New System.Drawing.Point(0, 0)
        Me.GrpBxUnConfTrans.Margin = New System.Windows.Forms.Padding(2)
        Me.GrpBxUnConfTrans.Name = "GrpBxUnConfTrans"
        Me.GrpBxUnConfTrans.Padding = New System.Windows.Forms.Padding(2)
        Me.GrpBxUnConfTrans.Size = New System.Drawing.Size(667, 122)
        Me.GrpBxUnConfTrans.TabIndex = 0
        Me.GrpBxUnConfTrans.TabStop = False
        Me.GrpBxUnConfTrans.Text = "GrpBxUnConfTrans"
        '
        'SplitContainer4
        '
        Me.SplitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.Location = New System.Drawing.Point(2, 15)
        Me.SplitContainer4.Name = "SplitContainer4"
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer4.Panel1.Controls.Add(Me.LabFee)
        Me.SplitContainer4.Panel1.Controls.Add(Me.TBFee)
        Me.SplitContainer4.Panel1.Controls.Add(Me.BtGetFeeInfo)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.LUnConTransBytes)
        Me.SplitContainer4.Panel2.Controls.Add(Me.LSumFee)
        Me.SplitContainer4.Panel2.Controls.Add(Me.LBigFee)
        Me.SplitContainer4.Panel2.Controls.Add(Me.LAvgFee)
        Me.SplitContainer4.Panel2.Controls.Add(Me.LUnConfTrans)
        Me.SplitContainer4.Size = New System.Drawing.Size(663, 105)
        Me.SplitContainer4.SplitterDistance = 198
        Me.SplitContainer4.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(99, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(69, 20)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "BURST"
        '
        'LabFee
        '
        Me.LabFee.AutoSize = True
        Me.LabFee.Location = New System.Drawing.Point(2, 3)
        Me.LabFee.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabFee.Name = "LabFee"
        Me.LabFee.Size = New System.Drawing.Size(49, 13)
        Me.LabFee.TabIndex = 0
        Me.LabFee.Text = "LabFee"
        '
        'TBFee
        '
        Me.TBFee.Location = New System.Drawing.Point(5, 18)
        Me.TBFee.Margin = New System.Windows.Forms.Padding(2)
        Me.TBFee.Name = "TBFee"
        Me.TBFee.Size = New System.Drawing.Size(89, 20)
        Me.TBFee.TabIndex = 1
        '
        'BtGetFeeInfo
        '
        Me.BtGetFeeInfo.Location = New System.Drawing.Point(5, 42)
        Me.BtGetFeeInfo.Margin = New System.Windows.Forms.Padding(2)
        Me.BtGetFeeInfo.Name = "BtGetFeeInfo"
        Me.BtGetFeeInfo.Size = New System.Drawing.Size(163, 22)
        Me.BtGetFeeInfo.TabIndex = 2
        Me.BtGetFeeInfo.Text = "BtGetFeeInfo"
        Me.BtGetFeeInfo.UseVisualStyleBackColor = True
        '
        'LUnConTransBytes
        '
        Me.LUnConTransBytes.BackColor = System.Drawing.SystemColors.Control
        Me.LUnConTransBytes.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LUnConTransBytes.Dock = System.Windows.Forms.DockStyle.Top
        Me.LUnConTransBytes.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LUnConTransBytes.Location = New System.Drawing.Point(0, 15)
        Me.LUnConTransBytes.Name = "LUnConTransBytes"
        Me.LUnConTransBytes.ReadOnly = True
        Me.LUnConTransBytes.Size = New System.Drawing.Size(459, 15)
        Me.LUnConTransBytes.TabIndex = 8
        Me.LUnConTransBytes.Text = "LUnConTransBytes"
        Me.LUnConTransBytes.Visible = False
        '
        'LSumFee
        '
        Me.LSumFee.BackColor = System.Drawing.SystemColors.Control
        Me.LSumFee.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LSumFee.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.LSumFee.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LSumFee.Location = New System.Drawing.Point(0, 58)
        Me.LSumFee.Name = "LSumFee"
        Me.LSumFee.ReadOnly = True
        Me.LSumFee.Size = New System.Drawing.Size(459, 15)
        Me.LSumFee.TabIndex = 9
        Me.LSumFee.Text = "LSumFee"
        '
        'LBigFee
        '
        Me.LBigFee.BackColor = System.Drawing.SystemColors.Control
        Me.LBigFee.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LBigFee.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.LBigFee.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBigFee.Location = New System.Drawing.Point(0, 73)
        Me.LBigFee.Name = "LBigFee"
        Me.LBigFee.ReadOnly = True
        Me.LBigFee.Size = New System.Drawing.Size(459, 15)
        Me.LBigFee.TabIndex = 11
        Me.LBigFee.Text = "LBigFee"
        '
        'LAvgFee
        '
        Me.LAvgFee.BackColor = System.Drawing.SystemColors.Control
        Me.LAvgFee.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LAvgFee.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.LAvgFee.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LAvgFee.Location = New System.Drawing.Point(0, 88)
        Me.LAvgFee.Name = "LAvgFee"
        Me.LAvgFee.ReadOnly = True
        Me.LAvgFee.Size = New System.Drawing.Size(459, 15)
        Me.LAvgFee.TabIndex = 10
        Me.LAvgFee.Text = "LAvgFee"
        '
        'LUnConfTrans
        '
        Me.LUnConfTrans.BackColor = System.Drawing.SystemColors.Control
        Me.LUnConfTrans.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LUnConfTrans.Dock = System.Windows.Forms.DockStyle.Top
        Me.LUnConfTrans.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LUnConfTrans.Location = New System.Drawing.Point(0, 0)
        Me.LUnConfTrans.Name = "LUnConfTrans"
        Me.LUnConfTrans.ReadOnly = True
        Me.LUnConfTrans.Size = New System.Drawing.Size(459, 15)
        Me.LUnConfTrans.TabIndex = 7
        Me.LUnConfTrans.Text = "LUnConfTrans"
        '
        'TabTransacts
        '
        Me.TabTransacts.BackColor = System.Drawing.SystemColors.Control
        Me.TabTransacts.Controls.Add(Me.LVTrans)
        Me.TabTransacts.Location = New System.Drawing.Point(4, 22)
        Me.TabTransacts.Name = "TabTransacts"
        Me.TabTransacts.Size = New System.Drawing.Size(677, 322)
        Me.TabTransacts.TabIndex = 2
        Me.TabTransacts.Text = "TabTransacts"
        '
        'LVTrans
        '
        Me.LVTrans.BackColor = System.Drawing.SystemColors.Window
        Me.LVTrans.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.BlockID, Me.TransID, Me.Confirms, Me.Sender, Me.Recipient, Me.Type, Me.Amount, Me.Fee})
        Me.LVTrans.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVTrans.FullRowSelect = True
        Me.LVTrans.GridLines = True
        Me.LVTrans.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.LVTrans.Location = New System.Drawing.Point(0, 0)
        Me.LVTrans.Margin = New System.Windows.Forms.Padding(2)
        Me.LVTrans.MultiSelect = False
        Me.LVTrans.Name = "LVTrans"
        Me.LVTrans.Size = New System.Drawing.Size(677, 322)
        Me.LVTrans.TabIndex = 0
        Me.LVTrans.UseCompatibleStateImageBehavior = False
        Me.LVTrans.View = System.Windows.Forms.View.Details
        '
        'BlockID
        '
        Me.BlockID.Text = "BlockID"
        Me.BlockID.Width = 101
        '
        'TransID
        '
        Me.TransID.Text = "TransID"
        Me.TransID.Width = 52
        '
        'Confirms
        '
        Me.Confirms.Text = "Confirms"
        Me.Confirms.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Confirms.Width = 96
        '
        'Sender
        '
        Me.Sender.Text = "Sender"
        Me.Sender.Width = 71
        '
        'Recipient
        '
        Me.Recipient.Text = "Recipient"
        Me.Recipient.Width = 92
        '
        'Type
        '
        Me.Type.Text = "Type"
        Me.Type.Width = 47
        '
        'Amount
        '
        Me.Amount.Text = "Amount"
        Me.Amount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Amount.Width = 79
        '
        'Fee
        '
        Me.Fee.Text = "Fee"
        Me.Fee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Fee.Width = 94
        '
        'frmJSONPocket
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(685, 397)
        Me.Controls.Add(Me.WalletPanel)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.HomeStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.HomeStrip
        Me.MinimumSize = New System.Drawing.Size(672, 436)
        Me.Name = "frmJSONPocket"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Burstcoin JSON-Pocket for Windows"
        Me.HomeStrip.ResumeLayout(False)
        Me.HomeStrip.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.WalletPanel.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabOverview.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.GrpBxAccInfo.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.GrpBxMiningInfo.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.GrpBxBalancesInfo.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.TabSend.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.GrpBxSendTo.ResumeLayout(False)
        Me.GrpBxSendTo.PerformLayout()
        Me.GrpBxUnConfTrans.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel1.PerformLayout()
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        Me.SplitContainer4.Panel2.PerformLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.TabTransacts.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents HomeStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents FileTSMI As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents unlock As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsTSMI As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NodeURLs As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents PeerTSMI As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSSLabOn As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents WalletPanel As System.Windows.Forms.Panel
    Friend WithEvents TSSLabSec As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents exitprog As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WLock As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents refreshpeers As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddressTSMI As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabOverview As System.Windows.Forms.TabPage
    Friend WithEvents TabSend As System.Windows.Forms.TabPage
    Friend WithEvents TabTransacts As System.Windows.Forms.TabPage
    Friend WithEvents refreshaddressinfo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents refreshpeer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WalletPeers As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents WalletVersion As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents autoconnect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TBPassPhrase As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents lock As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TBaddress As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents GrpBxBalancesInfo As System.Windows.Forms.GroupBox
    Friend WithEvents GrpBxAccInfo As System.Windows.Forms.GroupBox
    Friend WithEvents TBPubKey As System.Windows.Forms.TextBox
    Friend WithEvents LabPubKey As System.Windows.Forms.Label
    Friend WithEvents TBAssistedTo As System.Windows.Forms.TextBox
    Friend WithEvents TBMinedBlocks As System.Windows.Forms.TextBox
    Friend WithEvents TBNumAdd As System.Windows.Forms.TextBox
    Friend WithEvents LabRewAss As System.Windows.Forms.Label
    Friend WithEvents LabGenBlocks As System.Windows.Forms.Label
    Friend WithEvents LabNumAcc As System.Windows.Forms.Label
    Friend WithEvents GrpBxMiningInfo As System.Windows.Forms.GroupBox
    Friend WithEvents TBName As System.Windows.Forms.TextBox
    Friend WithEvents LabName As System.Windows.Forms.Label
    Friend WithEvents LVTrans As System.Windows.Forms.ListView
    Friend WithEvents BlockID As System.Windows.Forms.ColumnHeader
    Friend WithEvents TransID As System.Windows.Forms.ColumnHeader
    Friend WithEvents Sender As System.Windows.Forms.ColumnHeader
    Friend WithEvents Recipient As System.Windows.Forms.ColumnHeader
    Friend WithEvents Confirms As System.Windows.Forms.ColumnHeader
    Friend WithEvents Type As System.Windows.Forms.ColumnHeader
    Friend WithEvents Amount As System.Windows.Forms.ColumnHeader
    Friend WithEvents Fee As System.Windows.Forms.ColumnHeader
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents BtSetName As System.Windows.Forms.Button
    Friend WithEvents BtSetRewAss As System.Windows.Forms.Button
    Friend WithEvents LabNewAcc As System.Windows.Forms.Label
    Friend WithEvents BtAccVerify As System.Windows.Forms.Button
    Friend WithEvents CoBxRewAss As System.Windows.Forms.ComboBox
    Friend WithEvents TSProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents LConBal As System.Windows.Forms.TextBox
    Friend WithEvents LSumBal As System.Windows.Forms.TextBox
    Friend WithEvents LUnBal As System.Windows.Forms.TextBox
    Friend WithEvents LGenBal As System.Windows.Forms.TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents GrpBxSendTo As GroupBox
    Friend WithEvents GrpBxUnConfTrans As GroupBox
    Friend WithEvents BtSend As Button
    Friend WithEvents TBAmount As TextBox
    Friend WithEvents LabAmount As Label
    Friend WithEvents TBRecipient As TextBox
    Friend WithEvents LabRecipient As Label
    Friend WithEvents BtGetFeeInfo As Button
    Friend WithEvents TBFee As TextBox
    Friend WithEvents LabFee As Label
    Friend WithEvents SplitContainer4 As System.Windows.Forms.SplitContainer
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LUnConfTrans As System.Windows.Forms.TextBox
    Friend WithEvents LAvgFee As System.Windows.Forms.TextBox
    Friend WithEvents LSumFee As System.Windows.Forms.TextBox
    Friend WithEvents LUnConTransBytes As System.Windows.Forms.TextBox
    Friend WithEvents LBigFee As System.Windows.Forms.TextBox
End Class
