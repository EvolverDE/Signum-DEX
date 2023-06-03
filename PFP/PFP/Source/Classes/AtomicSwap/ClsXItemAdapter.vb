Public Class ClsXItemAdapter

    Public Shared Function NewXItem(ByVal XItem As String) As AbsClsXItem

        Select Case XItem
            Case "BTC"
                Return New ClsBitcoin()
        End Select

        Return Nothing

    End Function

    Public Shared Function GetXItemAddress(ByVal XItem As String) As String

        Select Case XItem
            Case "BTC"
                Return GetBitcoinMainAddress()
        End Select

        Return ""

    End Function

End Class
