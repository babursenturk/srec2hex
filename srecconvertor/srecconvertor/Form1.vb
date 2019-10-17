Imports System.IO

Public Class Form1
    Dim reader As New System.IO.StreamReader("d:\p100_0_v0.srec")   'we are using constant. please change this you desired.
    Dim swFile As System.IO.StreamWriter
    Dim sayac1 As Integer
    Dim sayac2 As Integer
    Dim allLines As List(Of String) = New List(Of String)
    Dim h_arr(16)

    Public Function ReadLine(ByVal lineNumber As Integer, ByVal lines As List(Of String)) As String
        Return lines(lineNumber - 1)
    End Function

    Public Function get_hex(ByVal hexstr As String)

        Dim result As Integer
        Dim i As Integer

        For i = 0 To 15
            If hexstr.Substring(0, 1) = h_arr(i) Then
                result = i * 16
                Exit For
            End If

        Next
        For i = 0 To 15
            If hexstr.Substring(1, 1) = h_arr(i) Then
                result = result + i
                Exit For
            End If

        Next

        Return result
    End Function
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Do While Not reader.EndOfStream
            allLines.Add(reader.ReadLine())
        Loop
        reader.Close()

        h_arr(0) = "0" : h_arr(1) = "1" : h_arr(2) = "2" : h_arr(3) = "3" : h_arr(4) = "4"
        h_arr(5) = "5" : h_arr(6) = "6" : h_arr(7) = "7" : h_arr(8) = "8" : h_arr(9) = "9"
        h_arr(10) = "A" : h_arr(11) = "B" : h_arr(12) = "C" : h_arr(13) = "D" : h_arr(14) = "E"
        h_arr(15) = "F"

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim rdcnt
        Dim rdstr As String
        Dim idx = 0
        Dim loc As Integer
        Dim line_len As Integer
        Dim line_dat(15) As Byte
        Dim line_dat2(15) As Byte
        Dim mainfile As FileStream
        Dim csfile As FileStream


        Dim file_cs As Integer

        Try
            mainfile = New FileStream("d:\poc500.bin", FileMode.Create)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        Try
            csfile = New FileStream("d:\csum.bin", FileMode.Create)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        Dim writeBinay As New BinaryWriter(mainfile)
        Dim writeBinay2 As New BinaryWriter(csfile)
        Dim dat_idx = 0

        file_cs = 0

        For rdcnt = 0 To allLines.Count - 1
            sayac1 = sayac1 + 1


            loc = allLines(rdcnt).IndexOf("S3")
            If loc >= 0 Then
                rdstr = allLines(rdcnt).Substring(2, allLines(rdcnt).Length - 2)

                line_len = get_hex(rdstr.Substring(0, 2)) * 2
                line_len = line_len - 10

                rdstr = rdstr.Substring(10, line_len)   'currently we don't have checksum control.
                Application.DoEvents()


                dat_idx = 0
                'If (line_len < 32) Then Stop

                For hxcnt = 0 To line_len - 2 Step 2
                    line_dat(dat_idx) = get_hex(rdstr.Substring(hxcnt, 2))
                    sayac2 = sayac2 + 1

                    If (rdcnt = 0) And (hxcnt = 0) Then 'we use rdcnt 1 so, if it is 0, then it gets "S0" from srec file
                        file_cs = line_dat(0)
                    Else 'If (rdcnt > 1) Then
                        file_cs = ((file_cs + line_dat(dat_idx)) Mod 256) + 1
                    End If

                    Try
                        writeBinay.Write(line_dat(dat_idx))
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    Application.DoEvents()
                    dat_idx = dat_idx + 1
                Next
            End If

        Next
        writeBinay2.Write(file_cs)
        writeBinay2.Close()


        TextBox3.Text = file_cs
        writeBinay.Close()

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        TextBox2.Text = sayac1 & " line processed!"
        TextBox1.Text = sayac2 & " byte processed!"
    End Sub
End Class
