<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmEnterPIN
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmEnterPIN))
        Me.ChBxPIN = New System.Windows.Forms.CheckBox()
        Me.TBPIN = New System.Windows.Forms.TextBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TBOldPIN = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TBSignedBytes = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TBUnsignedBytes = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.BtOK = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.SuspendLayout()
        '
        'ChBxPIN
        '
        Me.ChBxPIN.AutoSize = True
        Me.ChBxPIN.Location = New System.Drawing.Point(6, 5)
        Me.ChBxPIN.Name = "ChBxPIN"
        Me.ChBxPIN.Size = New System.Drawing.Size(47, 17)
        Me.ChBxPIN.TabIndex = 0
        Me.ChBxPIN.Text = "PIN:"
        Me.ChBxPIN.UseVisualStyleBackColor = True
        '
        'TBPIN
        '
        Me.TBPIN.Location = New System.Drawing.Point(56, 3)
        Me.TBPIN.Name = "TBPIN"
        Me.TBPIN.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TBPIN.Size = New System.Drawing.Size(178, 20)
        Me.TBPIN.TabIndex = 1
        Me.TBPIN.UseSystemPasswordChar = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TBOldPIN)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ChBxPIN)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TBPIN)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TBSignedBytes)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TBUnsignedBytes)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label1)
        Me.SplitContainer1.Size = New System.Drawing.Size(246, 113)
        Me.SplitContainer1.SplitterDistance = 26
        Me.SplitContainer1.TabIndex = 2
        '
        'TBOldPIN
        '
        Me.TBOldPIN.Location = New System.Drawing.Point(56, 29)
        Me.TBOldPIN.Name = "TBOldPIN"
        Me.TBOldPIN.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TBOldPIN.Size = New System.Drawing.Size(178, 20)
        Me.TBOldPIN.TabIndex = 3
        Me.TBOldPIN.UseSystemPasswordChar = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 32)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Old PIN:"
        '
        'TBSignedBytes
        '
        Me.TBSignedBytes.Location = New System.Drawing.Point(3, 58)
        Me.TBSignedBytes.Name = "TBSignedBytes"
        Me.TBSignedBytes.Size = New System.Drawing.Size(240, 20)
        Me.TBSignedBytes.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 42)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(131, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Signed Transaction Bytes:"
        '
        'TBUnsignedBytes
        '
        Me.TBUnsignedBytes.Location = New System.Drawing.Point(3, 19)
        Me.TBUnsignedBytes.Name = "TBUnsignedBytes"
        Me.TBUnsignedBytes.ReadOnly = True
        Me.TBUnsignedBytes.Size = New System.Drawing.Size(240, 20)
        Me.TBUnsignedBytes.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(143, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Unsigned Transaction Bytes:"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BackColor = System.Drawing.Color.Transparent
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.SplitContainer1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(246, 156)
        Me.SplitContainer2.SplitterDistance = 113
        Me.SplitContainer2.TabIndex = 3
        '
        'SplitContainer3
        '
        Me.SplitContainer3.BackColor = System.Drawing.Color.Transparent
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.BtOK)
        Me.SplitContainer3.Size = New System.Drawing.Size(246, 39)
        Me.SplitContainer3.SplitterDistance = 155
        Me.SplitContainer3.TabIndex = 0
        '
        'BtOK
        '
        Me.BtOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtOK.ForeColor = System.Drawing.Color.White
        Me.BtOK.Location = New System.Drawing.Point(3, 8)
        Me.BtOK.Name = "BtOK"
        Me.BtOK.Size = New System.Drawing.Size(75, 23)
        Me.BtOK.TabIndex = 0
        Me.BtOK.Text = "OK"
        Me.BtOK.UseVisualStyleBackColor = False
        '
        'FrmEnterPIN
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.SnipSwap.My.Resources.Resources.signum_back3
        Me.ClientSize = New System.Drawing.Size(246, 156)
        Me.Controls.Add(Me.SplitContainer2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmEnterPIN"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Enter/Change PIN"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ChBxPIN As CheckBox
    Friend WithEvents TBPIN As TextBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TBSignedBytes As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TBUnsignedBytes As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents BtOK As Button
    Friend WithEvents TBOldPIN As TextBox
    Friend WithEvents Label3 As Label
End Class
