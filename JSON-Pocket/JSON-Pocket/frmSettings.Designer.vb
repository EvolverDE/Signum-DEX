<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Me.LabTimeout = New System.Windows.Forms.Label()
        Me.TBTimeout = New System.Windows.Forms.TextBox()
        Me.BtTimeoutInfo = New System.Windows.Forms.Button()
        Me.BtNodeURLInfo = New System.Windows.Forms.Button()
        Me.TBNodeListURL = New System.Windows.Forms.TextBox()
        Me.LabURLPeers = New System.Windows.Forms.Label()
        Me.BtTypeInfo = New System.Windows.Forms.Button()
        Me.LabType = New System.Windows.Forms.Label()
        Me.GrpBxConn = New System.Windows.Forms.GroupBox()
        Me.ChBxOKOnly = New System.Windows.Forms.CheckBox()
        Me.LabURLPools = New System.Windows.Forms.Label()
        Me.BtPoolURLInfo = New System.Windows.Forms.Button()
        Me.TBPoolListURL = New System.Windows.Forms.TextBox()
        Me.CoBxTyp = New System.Windows.Forms.ComboBox()
        Me.BtOK = New System.Windows.Forms.Button()
        Me.GrpBxConn.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabTimeout
        '
        Me.LabTimeout.AutoSize = True
        Me.LabTimeout.Location = New System.Drawing.Point(6, 133)
        Me.LabTimeout.Name = "LabTimeout"
        Me.LabTimeout.Size = New System.Drawing.Size(73, 13)
        Me.LabTimeout.TabIndex = 0
        Me.LabTimeout.Text = "LabTimeout"
        '
        'TBTimeout
        '
        Me.TBTimeout.Location = New System.Drawing.Point(9, 149)
        Me.TBTimeout.MaxLength = 2
        Me.TBTimeout.Name = "TBTimeout"
        Me.TBTimeout.Size = New System.Drawing.Size(273, 20)
        Me.TBTimeout.TabIndex = 1
        '
        'BtTimeoutInfo
        '
        Me.BtTimeoutInfo.Location = New System.Drawing.Point(288, 147)
        Me.BtTimeoutInfo.Name = "BtTimeoutInfo"
        Me.BtTimeoutInfo.Size = New System.Drawing.Size(41, 23)
        Me.BtTimeoutInfo.TabIndex = 2
        Me.BtTimeoutInfo.Text = "Info"
        Me.BtTimeoutInfo.UseVisualStyleBackColor = True
        '
        'BtNodeURLInfo
        '
        Me.BtNodeURLInfo.Location = New System.Drawing.Point(288, 30)
        Me.BtNodeURLInfo.Name = "BtNodeURLInfo"
        Me.BtNodeURLInfo.Size = New System.Drawing.Size(41, 23)
        Me.BtNodeURLInfo.TabIndex = 5
        Me.BtNodeURLInfo.Text = "Info"
        Me.BtNodeURLInfo.UseVisualStyleBackColor = True
        '
        'TBNodeListURL
        '
        Me.TBNodeListURL.Location = New System.Drawing.Point(9, 32)
        Me.TBNodeListURL.Name = "TBNodeListURL"
        Me.TBNodeListURL.Size = New System.Drawing.Size(273, 20)
        Me.TBNodeListURL.TabIndex = 4
        '
        'LabURLPeers
        '
        Me.LabURLPeers.AutoSize = True
        Me.LabURLPeers.Location = New System.Drawing.Point(6, 16)
        Me.LabURLPeers.Name = "LabURLPeers"
        Me.LabURLPeers.Size = New System.Drawing.Size(85, 13)
        Me.LabURLPeers.TabIndex = 3
        Me.LabURLPeers.Text = "LabURLPeers"
        '
        'BtTypeInfo
        '
        Me.BtTypeInfo.Location = New System.Drawing.Point(288, 108)
        Me.BtTypeInfo.Name = "BtTypeInfo"
        Me.BtTypeInfo.Size = New System.Drawing.Size(41, 23)
        Me.BtTypeInfo.TabIndex = 8
        Me.BtTypeInfo.Text = "Info"
        Me.BtTypeInfo.UseVisualStyleBackColor = True
        '
        'LabType
        '
        Me.LabType.AutoSize = True
        Me.LabType.Location = New System.Drawing.Point(6, 94)
        Me.LabType.Name = "LabType"
        Me.LabType.Size = New System.Drawing.Size(56, 13)
        Me.LabType.TabIndex = 6
        Me.LabType.Text = "LabType"
        '
        'GrpBxConn
        '
        Me.GrpBxConn.Controls.Add(Me.ChBxOKOnly)
        Me.GrpBxConn.Controls.Add(Me.LabURLPools)
        Me.GrpBxConn.Controls.Add(Me.BtPoolURLInfo)
        Me.GrpBxConn.Controls.Add(Me.TBPoolListURL)
        Me.GrpBxConn.Controls.Add(Me.CoBxTyp)
        Me.GrpBxConn.Controls.Add(Me.LabURLPeers)
        Me.GrpBxConn.Controls.Add(Me.BtTypeInfo)
        Me.GrpBxConn.Controls.Add(Me.LabTimeout)
        Me.GrpBxConn.Controls.Add(Me.TBTimeout)
        Me.GrpBxConn.Controls.Add(Me.LabType)
        Me.GrpBxConn.Controls.Add(Me.BtTimeoutInfo)
        Me.GrpBxConn.Controls.Add(Me.BtNodeURLInfo)
        Me.GrpBxConn.Controls.Add(Me.TBNodeListURL)
        Me.GrpBxConn.Location = New System.Drawing.Point(12, 12)
        Me.GrpBxConn.Name = "GrpBxConn"
        Me.GrpBxConn.Size = New System.Drawing.Size(335, 181)
        Me.GrpBxConn.TabIndex = 9
        Me.GrpBxConn.TabStop = False
        Me.GrpBxConn.Text = "GrpBxConn"
        '
        'ChBxOKOnly
        '
        Me.ChBxOKOnly.AutoSize = True
        Me.ChBxOKOnly.Location = New System.Drawing.Point(100, 112)
        Me.ChBxOKOnly.Name = "ChBxOKOnly"
        Me.ChBxOKOnly.Size = New System.Drawing.Size(97, 17)
        Me.ChBxOKOnly.TabIndex = 13
        Me.ChBxOKOnly.Text = "ChBxOKOnly"
        Me.ChBxOKOnly.UseVisualStyleBackColor = True
        '
        'LabURLPools
        '
        Me.LabURLPools.AutoSize = True
        Me.LabURLPools.Location = New System.Drawing.Point(6, 55)
        Me.LabURLPools.Name = "LabURLPools"
        Me.LabURLPools.Size = New System.Drawing.Size(84, 13)
        Me.LabURLPools.TabIndex = 10
        Me.LabURLPools.Text = "LabURLPools"
        '
        'BtPoolURLInfo
        '
        Me.BtPoolURLInfo.Location = New System.Drawing.Point(288, 69)
        Me.BtPoolURLInfo.Name = "BtPoolURLInfo"
        Me.BtPoolURLInfo.Size = New System.Drawing.Size(41, 23)
        Me.BtPoolURLInfo.TabIndex = 12
        Me.BtPoolURLInfo.Text = "Info"
        Me.BtPoolURLInfo.UseVisualStyleBackColor = True
        '
        'TBPoolListURL
        '
        Me.TBPoolListURL.Location = New System.Drawing.Point(9, 71)
        Me.TBPoolListURL.Name = "TBPoolListURL"
        Me.TBPoolListURL.Size = New System.Drawing.Size(273, 20)
        Me.TBPoolListURL.TabIndex = 11
        '
        'CoBxTyp
        '
        Me.CoBxTyp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CoBxTyp.FormattingEnabled = True
        Me.CoBxTyp.Items.AddRange(New Object() {"Wallet", "Pool", "Faucet", "ALL"})
        Me.CoBxTyp.Location = New System.Drawing.Point(9, 110)
        Me.CoBxTyp.Name = "CoBxTyp"
        Me.CoBxTyp.Size = New System.Drawing.Size(85, 21)
        Me.CoBxTyp.TabIndex = 9
        '
        'BtOK
        '
        Me.BtOK.Location = New System.Drawing.Point(272, 199)
        Me.BtOK.Name = "BtOK"
        Me.BtOK.Size = New System.Drawing.Size(75, 23)
        Me.BtOK.TabIndex = 10
        Me.BtOK.Text = "OK"
        Me.BtOK.UseVisualStyleBackColor = True
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(359, 236)
        Me.Controls.Add(Me.BtOK)
        Me.Controls.Add(Me.GrpBxConn)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmSettings"
        Me.GrpBxConn.ResumeLayout(False)
        Me.GrpBxConn.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LabTimeout As System.Windows.Forms.Label
    Friend WithEvents TBTimeout As System.Windows.Forms.TextBox
    Friend WithEvents BtTimeoutInfo As System.Windows.Forms.Button
    Friend WithEvents BtNodeURLInfo As System.Windows.Forms.Button
    Friend WithEvents TBNodeListURL As System.Windows.Forms.TextBox
    Friend WithEvents LabURLPeers As System.Windows.Forms.Label
    Friend WithEvents BtTypeInfo As System.Windows.Forms.Button
    Friend WithEvents LabType As System.Windows.Forms.Label
    Friend WithEvents GrpBxConn As System.Windows.Forms.GroupBox
    Friend WithEvents CoBxTyp As System.Windows.Forms.ComboBox
    Friend WithEvents LabURLPools As System.Windows.Forms.Label
    Friend WithEvents BtPoolURLInfo As System.Windows.Forms.Button
    Friend WithEvents TBPoolListURL As System.Windows.Forms.TextBox
    Friend WithEvents ChBxOKOnly As System.Windows.Forms.CheckBox
    Friend WithEvents BtOK As System.Windows.Forms.Button
End Class
