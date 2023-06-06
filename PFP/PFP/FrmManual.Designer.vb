<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmManual
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmManual))
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.LabWelcome = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.SplitContainer15 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.LabFirstSteps = New System.Windows.Forms.Label()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.TBManualPIN = New System.Windows.Forms.TextBox()
        Me.ChBxManualEncryptPP = New System.Windows.Forms.CheckBox()
        Me.TBManualAddress = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TBManualPassPhrase = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btManualSetPassPhrase = New System.Windows.Forms.Button()
        Me.BtClose = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.SplitContainer15, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer15.Panel1.SuspendLayout()
        Me.SplitContainer15.Panel2.SuspendLayout()
        Me.SplitContainer15.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(784, 361)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.TabPage1.Controls.Add(Me.LabWelcome)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(776, 335)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Welcome"
        '
        'LabWelcome
        '
        Me.LabWelcome.BackColor = System.Drawing.Color.Transparent
        Me.LabWelcome.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LabWelcome.Location = New System.Drawing.Point(3, 3)
        Me.LabWelcome.Name = "LabWelcome"
        Me.LabWelcome.Size = New System.Drawing.Size(770, 329)
        Me.LabWelcome.TabIndex = 0
        Me.LabWelcome.Text = "Loading..."
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.TabPage2.Controls.Add(Me.SplitContainer15)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(776, 335)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "First Steps"
        '
        'SplitContainer15
        '
        Me.SplitContainer15.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SplitContainer15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer15.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer15.Name = "SplitContainer15"
        Me.SplitContainer15.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer15.Panel1
        '
        Me.SplitContainer15.Panel1.Controls.Add(Me.SplitContainer1)
        '
        'SplitContainer15.Panel2
        '
        Me.SplitContainer15.Panel2.Controls.Add(Me.BtClose)
        Me.SplitContainer15.Size = New System.Drawing.Size(770, 329)
        Me.SplitContainer15.SplitterDistance = 183
        Me.SplitContainer15.TabIndex = 2
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.LabFirstSteps)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(770, 183)
        Me.SplitContainer1.SplitterDistance = 117
        Me.SplitContainer1.TabIndex = 1
        '
        'LabFirstSteps
        '
        Me.LabFirstSteps.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LabFirstSteps.Location = New System.Drawing.Point(0, 0)
        Me.LabFirstSteps.Name = "LabFirstSteps"
        Me.LabFirstSteps.Size = New System.Drawing.Size(770, 117)
        Me.LabFirstSteps.TabIndex = 0
        Me.LabFirstSteps.Text = "Loading..."
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBManualPIN)
        Me.SplitContainer2.Panel1.Controls.Add(Me.ChBxManualEncryptPP)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBManualAddress)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TBManualPassPhrase)
        Me.SplitContainer2.Panel1.Controls.Add(Me.Label1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.btManualSetPassPhrase)
        Me.SplitContainer2.Size = New System.Drawing.Size(770, 62)
        Me.SplitContainer2.SplitterDistance = 625
        Me.SplitContainer2.TabIndex = 0
        '
        'TBManualPIN
        '
        Me.TBManualPIN.Location = New System.Drawing.Point(253, 28)
        Me.TBManualPIN.Name = "TBManualPIN"
        Me.TBManualPIN.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TBManualPIN.Size = New System.Drawing.Size(163, 20)
        Me.TBManualPIN.TabIndex = 5
        Me.TBManualPIN.UseSystemPasswordChar = True
        '
        'ChBxManualEncryptPP
        '
        Me.ChBxManualEncryptPP.AutoSize = True
        Me.ChBxManualEncryptPP.Location = New System.Drawing.Point(77, 30)
        Me.ChBxManualEncryptPP.Name = "ChBxManualEncryptPP"
        Me.ChBxManualEncryptPP.Size = New System.Drawing.Size(170, 17)
        Me.ChBxManualEncryptPP.TabIndex = 4
        Me.ChBxManualEncryptPP.Text = "Encrypt PassPhrase With PIN:"
        Me.ChBxManualEncryptPP.UseVisualStyleBackColor = True
        '
        'TBManualAddress
        '
        Me.TBManualAddress.Location = New System.Drawing.Point(511, 4)
        Me.TBManualAddress.Name = "TBManualAddress"
        Me.TBManualAddress.Size = New System.Drawing.Size(158, 20)
        Me.TBManualAddress.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(422, 7)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "and/or Address:"
        '
        'TBManualPassPhrase
        '
        Me.TBManualPassPhrase.Location = New System.Drawing.Point(77, 4)
        Me.TBManualPassPhrase.Name = "TBManualPassPhrase"
        Me.TBManualPassPhrase.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TBManualPassPhrase.Size = New System.Drawing.Size(339, 20)
        Me.TBManualPassPhrase.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "PassPhrase:"
        '
        'btManualSetPassPhrase
        '
        Me.btManualSetPassPhrase.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btManualSetPassPhrase.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btManualSetPassPhrase.Location = New System.Drawing.Point(0, 0)
        Me.btManualSetPassPhrase.Name = "btManualSetPassPhrase"
        Me.btManualSetPassPhrase.Size = New System.Drawing.Size(141, 62)
        Me.btManualSetPassPhrase.TabIndex = 0
        Me.btManualSetPassPhrase.Text = "Check/Set Account"
        Me.btManualSetPassPhrase.UseVisualStyleBackColor = False
        '
        'BtClose
        '
        Me.BtClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BtClose.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtClose.Location = New System.Drawing.Point(0, 0)
        Me.BtClose.Name = "BtClose"
        Me.BtClose.Size = New System.Drawing.Size(770, 142)
        Me.BtClose.TabIndex = 1
        Me.BtClose.Text = "Lets Trade..."
        Me.BtClose.UseVisualStyleBackColor = False
        Me.BtClose.Visible = False
        '
        'FrmManual
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(153, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.BackgroundImage = Global.PFP.My.Resources.Resources.signum_back3
        Me.ClientSize = New System.Drawing.Size(784, 361)
        Me.Controls.Add(Me.TabControl1)
        Me.ForeColor = System.Drawing.Color.White
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmManual"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Intro (Manual)"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.SplitContainer15.Panel1.ResumeLayout(False)
        Me.SplitContainer15.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer15, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer15.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents LabWelcome As Label
    Friend WithEvents SplitContainer15 As SplitContainer
    Friend WithEvents BtClose As Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents LabFirstSteps As Label
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents TBManualPIN As TextBox
    Friend WithEvents ChBxManualEncryptPP As CheckBox
    Friend WithEvents TBManualAddress As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TBManualPassPhrase As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btManualSetPassPhrase As Button
End Class
