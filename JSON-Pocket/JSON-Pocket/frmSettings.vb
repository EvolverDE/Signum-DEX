Public Class frmSettings

    Private Sub BtURLInfo_Click(sender As System.Object, e As System.EventArgs) Handles BtNodeURLInfo.Click
        MsgBox(getLang(iniSettingsMsgBox.URLInfo))
    End Sub

    Private Sub BtTypeInfo_Click(sender As System.Object, e As System.EventArgs) Handles BtTypeInfo.Click
        MsgBox(getLang(iniSettingsMsgBox.TypeInfo))
    End Sub

    Private Sub BtTimeoutInfo_Click(sender As System.Object, e As System.EventArgs) Handles BtTimeoutInfo.Click
        MsgBox(getLang(iniSettingsMsgBox.TimeoutInfo))
    End Sub

    Private Sub BtPoolURLInfo_Click(sender As System.Object, e As System.EventArgs) Handles BtPoolURLInfo.Click
        MsgBox(getLang(iniSettingsMsgBox.PoolURLInfo))
    End Sub


    Private Sub frmSettings_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.Text = getLang(iniSettings.Title) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.Title.ToString)
        GrpBxConn.Text = getLang(iniSettings.GrpBxConn) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.GrpBxConn.ToString)
        LabURLPeers.Text = getLang(iniSettings.LabURLPeers) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabURLPeers.ToString)
        LabURLPools.Text = getLang(iniSettings.LabURLPools) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabURLPools.ToString)
        LabType.Text = getLang(iniSettings.LabType) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabType.ToString)
        LabTimeout.Text = getLang(iniSettings.LabTimeout) 'INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.LabTimeout.ToString)
        ChBxOKOnly.Text = getLang(iniSettings.ChBxOKOnly) ' INIGetValue(startuppath + "lang\" + INIGetValue("", "GUI", "Language") + ".ini", iniSections.Settings.ToString, iniSettings.ChBxOKOnly.ToString)

        Try

            TBNodeListURL.Text = merge(INIGetValue("", "Connection", "NodeListURL"))
            TBPoolListURL.Text = merge(INIGetValue("", "Connection", "PoolListURL"))

            For Each x In CoBxTyp.Items
                If x.ToString.Trim.ToLower = INIGetValue("", "Connection", "Type").ToLower Then
                    CoBxTyp.SelectedItem = x
                End If
            Next

            TBTimeout.Text = INIGetValue("", "Connection", "Timeout")
            ChBxOKOnly.Checked = CBool(INIGetValue("", "Connection", "OKOnly"))

        Catch ex As Exception

        End Try

    End Sub

    Private Sub frmSettings_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        If TBNodeListURL.Text.Trim <> merge(INIGetValue("", "Connection", "NodeListURL")) Or TBPoolListURL.Text.Trim <> merge(INIGetValue("", "Connection", "PoolListURL")) Or CoBxTyp.SelectedItem.ToString.ToLower <> INIGetValue("", "Connection", "Type").ToLower Or TBTimeout.Text.Trim <> INIGetValue("", "Connection", "Timeout") Then

            Dim res As MsgBoxResult = MsgBox(getLang(iniSettingsMsgBox.SaveQuestion), MsgBoxStyle.YesNo Or MsgBoxStyle.Question Or MsgBoxStyle.DefaultButton2)

            If res = MsgBoxResult.Yes Then
                INISetValue("", "Connection", "NodeListURL", TBNodeListURL.Text.Trim)
                INISetValue("", "Connection", "PoolListURL", TBPoolListURL.Text.Trim)
                INISetValue("", "Connection", "Type", CoBxTyp.SelectedItem.ToString.Trim)
                INISetValue("", "Connection", "Timeout", TBTimeout.Text.Trim)
                INISetValue("", "Connection", "OKOnly", ChBxOKOnly.Checked.ToString.Trim)
            End If

        End If

        Dim ji As String = ""
        If System.IO.File.Exists(startuppath + "network.txt") Then
            ji = frmJSONPocket.readFile("network.txt")
            frmJSONPocket.LoadNodes(ji)
        End If

    End Sub

    Private Sub TBTimeout_KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TBTimeout.KeyPress

        Select Case Asc(e.KeyChar)
            Case 48 To 57, 8
                ' Zahlen und Backspace zulassen
            Case Else
                ' alle anderen Eingaben unterdrücken
                e.Handled = True
        End Select

    End Sub

  
    Private Sub BtOK_Click(sender As System.Object, e As System.EventArgs) Handles BtOK.Click

        INISetValue("", "Connection", "NodeListURL", TBNodeListURL.Text.Trim)
        INISetValue("", "Connection", "PoolListURL", TBPoolListURL.Text.Trim)
        INISetValue("", "Connection", "Type", CoBxTyp.SelectedItem.ToString.Trim)
        INISetValue("", "Connection", "Timeout", TBTimeout.Text.Trim)
        INISetValue("", "Connection", "OKOnly", ChBxOKOnly.Checked.ToString.Trim)

        Me.Close()

    End Sub

End Class