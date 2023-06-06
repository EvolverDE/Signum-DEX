<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmBitcoinAccounts
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmBitcoinAccounts))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TBBitcoinMnemonic = New System.Windows.Forms.TextBox()
        Me.BtShowHideMnemonic = New System.Windows.Forms.Button()
        Me.BtGenerateMnemonic = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TBPrivateKey = New System.Windows.Forms.TextBox()
        Me.BtShowHidePrivateKey = New System.Windows.Forms.Button()
        Me.TBPublicKey = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TBAddress = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BtAdd = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LVAddresses = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TBWallet = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtCreateWallet = New System.Windows.Forms.Button()
        Me.ChBxReScan = New System.Windows.Forms.CheckBox()
        Me.BtLoadWallet = New System.Windows.Forms.Button()
        Me.BtUnloadWallet = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.bouncer = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.ScanningTime = New System.Windows.Forms.Timer(Me.components)
        Me.BtAbortScan = New System.Windows.Forms.Button()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 42)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Mnemonic:"
        '
        'TBBitcoinMnemonic
        '
        Me.TBBitcoinMnemonic.Location = New System.Drawing.Point(84, 39)
        Me.TBBitcoinMnemonic.Name = "TBBitcoinMnemonic"
        Me.TBBitcoinMnemonic.Size = New System.Drawing.Size(400, 20)
        Me.TBBitcoinMnemonic.TabIndex = 1
        Me.TBBitcoinMnemonic.UseSystemPasswordChar = True
        '
        'BtShowHideMnemonic
        '
        Me.BtShowHideMnemonic.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtShowHideMnemonic.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtShowHideMnemonic.ForeColor = System.Drawing.Color.White
        Me.BtShowHideMnemonic.Location = New System.Drawing.Point(490, 37)
        Me.BtShowHideMnemonic.Name = "BtShowHideMnemonic"
        Me.BtShowHideMnemonic.Size = New System.Drawing.Size(45, 23)
        Me.BtShowHideMnemonic.TabIndex = 2
        Me.BtShowHideMnemonic.Text = "show"
        Me.BtShowHideMnemonic.UseVisualStyleBackColor = False
        '
        'BtGenerateMnemonic
        '
        Me.BtGenerateMnemonic.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtGenerateMnemonic.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtGenerateMnemonic.ForeColor = System.Drawing.Color.White
        Me.BtGenerateMnemonic.Location = New System.Drawing.Point(541, 37)
        Me.BtGenerateMnemonic.Name = "BtGenerateMnemonic"
        Me.BtGenerateMnemonic.Size = New System.Drawing.Size(76, 23)
        Me.BtGenerateMnemonic.TabIndex = 3
        Me.BtGenerateMnemonic.Text = "generate"
        Me.BtGenerateMnemonic.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(12, 68)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Private Key:"
        '
        'TBPrivateKey
        '
        Me.TBPrivateKey.Location = New System.Drawing.Point(84, 65)
        Me.TBPrivateKey.Name = "TBPrivateKey"
        Me.TBPrivateKey.Size = New System.Drawing.Size(400, 20)
        Me.TBPrivateKey.TabIndex = 5
        Me.TBPrivateKey.UseSystemPasswordChar = True
        '
        'BtShowHidePrivateKey
        '
        Me.BtShowHidePrivateKey.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtShowHidePrivateKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtShowHidePrivateKey.ForeColor = System.Drawing.Color.White
        Me.BtShowHidePrivateKey.Location = New System.Drawing.Point(490, 63)
        Me.BtShowHidePrivateKey.Name = "BtShowHidePrivateKey"
        Me.BtShowHidePrivateKey.Size = New System.Drawing.Size(45, 23)
        Me.BtShowHidePrivateKey.TabIndex = 6
        Me.BtShowHidePrivateKey.Text = "show"
        Me.BtShowHidePrivateKey.UseVisualStyleBackColor = False
        '
        'TBPublicKey
        '
        Me.TBPublicKey.Location = New System.Drawing.Point(84, 91)
        Me.TBPublicKey.Name = "TBPublicKey"
        Me.TBPublicKey.Size = New System.Drawing.Size(400, 20)
        Me.TBPublicKey.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(12, 94)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Public Key:"
        '
        'TBAddress
        '
        Me.TBAddress.Location = New System.Drawing.Point(84, 117)
        Me.TBAddress.Name = "TBAddress"
        Me.TBAddress.Size = New System.Drawing.Size(400, 20)
        Me.TBAddress.TabIndex = 10
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(12, 120)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Address:"
        '
        'BtAdd
        '
        Me.BtAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtAdd.ForeColor = System.Drawing.Color.White
        Me.BtAdd.Location = New System.Drawing.Point(572, 114)
        Me.BtAdd.Name = "BtAdd"
        Me.BtAdd.Size = New System.Drawing.Size(45, 23)
        Me.BtAdd.TabIndex = 11
        Me.BtAdd.Text = "Add"
        Me.BtAdd.UseVisualStyleBackColor = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(12, 146)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(92, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Wallet Addresses:"
        '
        'LVAddresses
        '
        Me.LVAddresses.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.LVAddresses.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.LVAddresses.ForeColor = System.Drawing.Color.White
        Me.LVAddresses.GridLines = True
        Me.LVAddresses.HideSelection = False
        Me.LVAddresses.Location = New System.Drawing.Point(84, 162)
        Me.LVAddresses.MultiSelect = False
        Me.LVAddresses.Name = "LVAddresses"
        Me.LVAddresses.Size = New System.Drawing.Size(400, 96)
        Me.LVAddresses.TabIndex = 13
        Me.LVAddresses.UseCompatibleStateImageBehavior = False
        Me.LVAddresses.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Address"
        Me.ColumnHeader1.Width = 391
        '
        'TBWallet
        '
        Me.TBWallet.Location = New System.Drawing.Point(84, 12)
        Me.TBWallet.Name = "TBWallet"
        Me.TBWallet.Size = New System.Drawing.Size(318, 20)
        Me.TBWallet.TabIndex = 15
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(12, 15)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(40, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Wallet:"
        '
        'BtCreateWallet
        '
        Me.BtCreateWallet.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtCreateWallet.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtCreateWallet.ForeColor = System.Drawing.Color.White
        Me.BtCreateWallet.Location = New System.Drawing.Point(408, 10)
        Me.BtCreateWallet.Name = "BtCreateWallet"
        Me.BtCreateWallet.Size = New System.Drawing.Size(76, 23)
        Me.BtCreateWallet.TabIndex = 16
        Me.BtCreateWallet.Text = "create"
        Me.BtCreateWallet.UseVisualStyleBackColor = False
        '
        'ChBxReScan
        '
        Me.ChBxReScan.AutoSize = True
        Me.ChBxReScan.Checked = True
        Me.ChBxReScan.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBxReScan.ForeColor = System.Drawing.Color.White
        Me.ChBxReScan.Location = New System.Drawing.Point(491, 119)
        Me.ChBxReScan.Name = "ChBxReScan"
        Me.ChBxReScan.Size = New System.Drawing.Size(69, 17)
        Me.ChBxReScan.TabIndex = 17
        Me.ChBxReScan.Text = "(Re)scan"
        Me.ChBxReScan.UseVisualStyleBackColor = True
        '
        'BtLoadWallet
        '
        Me.BtLoadWallet.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtLoadWallet.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtLoadWallet.ForeColor = System.Drawing.Color.White
        Me.BtLoadWallet.Location = New System.Drawing.Point(490, 10)
        Me.BtLoadWallet.Name = "BtLoadWallet"
        Me.BtLoadWallet.Size = New System.Drawing.Size(45, 23)
        Me.BtLoadWallet.TabIndex = 18
        Me.BtLoadWallet.Text = "load"
        Me.BtLoadWallet.UseVisualStyleBackColor = False
        '
        'BtUnloadWallet
        '
        Me.BtUnloadWallet.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtUnloadWallet.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtUnloadWallet.ForeColor = System.Drawing.Color.White
        Me.BtUnloadWallet.Location = New System.Drawing.Point(541, 10)
        Me.BtUnloadWallet.Name = "BtUnloadWallet"
        Me.BtUnloadWallet.Size = New System.Drawing.Size(76, 23)
        Me.BtUnloadWallet.TabIndex = 19
        Me.BtUnloadWallet.Text = "unload"
        Me.BtUnloadWallet.UseVisualStyleBackColor = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel, Me.bouncer, Me.StatusBar})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 289)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(629, 22)
        Me.StatusStrip1.TabIndex = 21
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(16, 17)
        Me.StatusLabel.Text = "   "
        '
        'bouncer
        '
        Me.bouncer.BackColor = System.Drawing.Color.Transparent
        Me.bouncer.Name = "bouncer"
        Me.bouncer.Size = New System.Drawing.Size(598, 17)
        Me.bouncer.Spring = True
        '
        'StatusBar
        '
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.Size = New System.Drawing.Size(100, 16)
        Me.StatusBar.Step = 1
        Me.StatusBar.Visible = False
        '
        'ScanningTime
        '
        Me.ScanningTime.Enabled = True
        Me.ScanningTime.Interval = 1000
        '
        'BtAbortScan
        '
        Me.BtAbortScan.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(147, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.BtAbortScan.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtAbortScan.ForeColor = System.Drawing.Color.White
        Me.BtAbortScan.Location = New System.Drawing.Point(537, 146)
        Me.BtAbortScan.Name = "BtAbortScan"
        Me.BtAbortScan.Size = New System.Drawing.Size(80, 23)
        Me.BtAbortScan.TabIndex = 22
        Me.BtAbortScan.Text = "Abort scan"
        Me.BtAbortScan.UseVisualStyleBackColor = False
        '
        'FrmBitcoinAccounts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer))
        Me.BackgroundImage = Global.PFP.My.Resources.Resources.bitcoin_logo_512
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(629, 311)
        Me.Controls.Add(Me.BtAbortScan)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.BtUnloadWallet)
        Me.Controls.Add(Me.BtLoadWallet)
        Me.Controls.Add(Me.ChBxReScan)
        Me.Controls.Add(Me.BtCreateWallet)
        Me.Controls.Add(Me.TBWallet)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.LVAddresses)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.BtAdd)
        Me.Controls.Add(Me.TBAddress)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TBPublicKey)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.BtShowHidePrivateKey)
        Me.Controls.Add(Me.TBPrivateKey)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.BtGenerateMnemonic)
        Me.Controls.Add(Me.BtShowHideMnemonic)
        Me.Controls.Add(Me.TBBitcoinMnemonic)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmBitcoinAccounts"
        Me.Text = "Bitcoin Address Settings"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TBBitcoinMnemonic As TextBox
    Friend WithEvents BtShowHideMnemonic As Button
    Friend WithEvents BtGenerateMnemonic As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents TBPrivateKey As TextBox
    Friend WithEvents BtShowHidePrivateKey As Button
    Friend WithEvents TBPublicKey As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TBAddress As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents BtAdd As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents LVAddresses As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents TBWallet As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents BtCreateWallet As Button
    Friend WithEvents ChBxReScan As CheckBox
    Friend WithEvents BtLoadWallet As Button
    Friend WithEvents BtUnloadWallet As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents StatusLabel As ToolStripStatusLabel
    Friend WithEvents StatusBar As ToolStripProgressBar
    Friend WithEvents bouncer As ToolStripStatusLabel
    Friend WithEvents ScanningTime As Timer
    Friend WithEvents BtAbortScan As Button
End Class
