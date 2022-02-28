<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMyOrderSettings
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.LVOrders = New System.Windows.Forms.ListView()
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.BtSave = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ChBxAutoCompleteAT = New System.Windows.Forms.CheckBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ChBxAutosendInfo = New System.Windows.Forms.CheckBox()
        Me.TBInfotext = New System.Windows.Forms.TextBox()
        Me.LabInfo = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CoBxPayType = New System.Windows.Forms.ComboBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BackColor = System.Drawing.Color.Transparent
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.Transparent
        Me.SplitContainer1.Panel2.Controls.Add(Me.BtSave)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label4)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ChBxAutoCompleteAT)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ChBxAutosendInfo)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TBInfotext)
        Me.SplitContainer1.Panel2.Controls.Add(Me.LabInfo)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.CoBxPayType)
        Me.SplitContainer1.Size = New System.Drawing.Size(800, 450)
        Me.SplitContainer1.SplitterDistance = 208
        Me.SplitContainer1.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.LVOrders)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(800, 208)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "MyOrders Settings"
        '
        'LVOrders
        '
        Me.LVOrders.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LVOrders.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader8, Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader7})
        Me.LVOrders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVOrders.ForeColor = System.Drawing.Color.White
        Me.LVOrders.FullRowSelect = True
        Me.LVOrders.GridLines = True
        Me.LVOrders.HideSelection = False
        Me.LVOrders.Location = New System.Drawing.Point(3, 16)
        Me.LVOrders.Name = "LVOrders"
        Me.LVOrders.Size = New System.Drawing.Size(794, 189)
        Me.LVOrders.TabIndex = 0
        Me.LVOrders.UseCompatibleStateImageBehavior = False
        Me.LVOrders.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "Smart Contract"
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Transaction"
        Me.ColumnHeader1.Width = 136
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Type"
        Me.ColumnHeader2.Width = 66
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Paytype"
        Me.ColumnHeader3.Width = 76
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Infotext"
        Me.ColumnHeader4.Width = 121
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Autosend Infotext"
        Me.ColumnHeader5.Width = 95
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Autocomplete AT"
        Me.ColumnHeader6.Width = 96
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Status"
        '
        'BtSave
        '
        Me.BtSave.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtSave.ForeColor = System.Drawing.Color.White
        Me.BtSave.Location = New System.Drawing.Point(15, 117)
        Me.BtSave.Name = "BtSave"
        Me.BtSave.Size = New System.Drawing.Size(75, 23)
        Me.BtSave.TabIndex = 8
        Me.BtSave.Text = "save"
        Me.BtSave.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(12, 92)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Autocomplete AT:"
        '
        'ChBxAutoCompleteAT
        '
        Me.ChBxAutoCompleteAT.AutoSize = True
        Me.ChBxAutoCompleteAT.ForeColor = System.Drawing.Color.White
        Me.ChBxAutoCompleteAT.Location = New System.Drawing.Point(111, 92)
        Me.ChBxAutoCompleteAT.Name = "ChBxAutoCompleteAT"
        Me.ChBxAutoCompleteAT.Size = New System.Drawing.Size(89, 17)
        Me.ChBxAutoCompleteAT.TabIndex = 6
        Me.ChBxAutoCompleteAT.Text = "(Only PayPal)"
        Me.ChBxAutoCompleteAT.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(12, 68)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Autosend Infotext:"
        '
        'ChBxAutosendInfo
        '
        Me.ChBxAutosendInfo.AutoSize = True
        Me.ChBxAutosendInfo.Location = New System.Drawing.Point(111, 68)
        Me.ChBxAutosendInfo.Name = "ChBxAutosendInfo"
        Me.ChBxAutosendInfo.Size = New System.Drawing.Size(15, 14)
        Me.ChBxAutosendInfo.TabIndex = 4
        Me.ChBxAutosendInfo.UseVisualStyleBackColor = True
        '
        'TBInfotext
        '
        Me.TBInfotext.Location = New System.Drawing.Point(111, 39)
        Me.TBInfotext.Name = "TBInfotext"
        Me.TBInfotext.Size = New System.Drawing.Size(134, 20)
        Me.TBInfotext.TabIndex = 3
        '
        'LabInfo
        '
        Me.LabInfo.AutoSize = True
        Me.LabInfo.ForeColor = System.Drawing.Color.White
        Me.LabInfo.Location = New System.Drawing.Point(12, 42)
        Me.LabInfo.Name = "LabInfo"
        Me.LabInfo.Size = New System.Drawing.Size(45, 13)
        Me.LabInfo.TabIndex = 2
        Me.LabInfo.Text = "Infotext:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Paytype:"
        '
        'CoBxPayType
        '
        Me.CoBxPayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxPayType.FormattingEnabled = True
        Me.CoBxPayType.Location = New System.Drawing.Point(111, 8)
        Me.CoBxPayType.Name = "CoBxPayType"
        Me.CoBxPayType.Size = New System.Drawing.Size(134, 21)
        Me.CoBxPayType.TabIndex = 0
        '
        'FrmMyOrderSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BackgroundImage = Global.PFP.My.Resources.Resources.signum_back3
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.SplitContainer1)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FrmMyOrderSettings"
        Me.Text = "FrmMyOrdersettings"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents LVOrders As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents TBInfotext As TextBox
    Friend WithEvents LabInfo As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents CoBxPayType As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents ChBxAutoCompleteAT As CheckBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ChBxAutosendInfo As CheckBox
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents BtSave As Button
    Friend WithEvents ColumnHeader8 As ColumnHeader
End Class
