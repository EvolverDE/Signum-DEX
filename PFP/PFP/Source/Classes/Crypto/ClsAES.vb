Option Strict On
Option Explicit On

Imports System.Security.Cryptography

Public Class ClsAES

    Public Function AES_Encrypt(ByVal Value As Byte(), ByVal Key As Byte()) As Byte()
        Dim AES As New RijndaelManaged
        Dim SHA256 As New SHA256Cng
        Dim Output() As Byte

        AES.GenerateIV()
        Dim IV() As Byte = AES.IV
        AES.Key = SHA256.ComputeHash(Key)

        AES.Mode = CipherMode.CBC
        Dim AESEncrypter As ICryptoTransform = AES.CreateEncryptor
        Dim Buffer As Byte() = Value
        Output = AESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length)

        'Copy the IV as the first 16 bytes of the output then copy encrypted bytes
        Dim IVAndOutput(Output.Length - 1 + 16) As Byte
        Array.Copy(IV, IVAndOutput, 16)
        Array.Copy(Output, 0, IVAndOutput, 16, Output.Length)

        Return IVAndOutput

    End Function

    Public Function AES_Decrypt(ByVal EncryptedValue As Byte(), ByVal Key As Byte(), Optional ByVal IVs As Byte() = Nothing) As Byte()

        Try

            Dim IV(15) As Byte
            Dim Buffer(EncryptedValue.Length - 1 - 16) As Byte

            If IsNothing(IVs) Then

                'Extract first 16 bytes of input stream as IV.  Copy remaining bytes into encrypted buffer
                Array.Copy(EncryptedValue, IV, 16)
                Array.Copy(EncryptedValue, 16, Buffer, 0, EncryptedValue.Length - 16)

            Else
                IV = IVs.ToArray
                Buffer = EncryptedValue
            End If

            Dim AES As New RijndaelManaged
            Dim SHA256 As New SHA256Cng
            AES.Key = SHA256.ComputeHash(Key)
            AES.IV = IV
            AES.Mode = CipherMode.CBC
            Dim AESDecrypter As ICryptoTransform = AES.CreateDecryptor()

            Dim DecryptedValue As Byte() = AESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length)

            Return DecryptedValue

        Catch ex As Exception
            Dim ErrorValue As Byte() = Nothing
            Return ErrorValue
        End Try

    End Function

End Class