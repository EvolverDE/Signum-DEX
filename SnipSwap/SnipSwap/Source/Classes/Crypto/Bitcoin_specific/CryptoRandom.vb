Option Strict On
Option Explicit On
Imports System.Security.Cryptography

Namespace Secp256k1Vb.Randoms
    Friend NotInheritable Class CryptoRandom
        '<ThreadStatic>
        '<Browsable(False)>
        '<EditorBrowsable(EditorBrowsableState.Never)>
        '<DebuggerBrowsable(DebuggerBrowsableState.Never)>
        '<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Private Shared ReadOnly Rand As RandomNumberGenerator = RandomNumberGenerator.Create()

        Friend Shared Sub NextBytes(bytes As Byte())
            Rand.GetNonZeroBytes(bytes)
        End Sub

        Private Shared Sub WarmUp()
            Dim bytes = New Byte(3) {}
            Dim number = BitConverter.ToUInt32(bytes, 0)
            Dim count As Int32 = CInt(number Mod 1000 + 1234)
            bytes = New Byte(count - 1) {}
            Rand.GetNonZeroBytes(bytes)
        End Sub

        Shared Sub New()
            WarmUp()
        End Sub
    End Class
End Namespace