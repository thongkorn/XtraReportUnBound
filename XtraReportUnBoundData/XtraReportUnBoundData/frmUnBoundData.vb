#Region "About"
' / --------------------------------------------------------------------------------
' / Developer : Mr.Surapon Yodsanga (Thongkorn Tubtimkrob)
' / eMail : thongkorn@hotmail.com
' / URL: http://www.g2gnet.com (Khon Kaen - Thailand)
' / Facebook: https://www.facebook.com/g2gnet (For Thailand)
' / Facebook: https://www.facebook.com/commonindy (Worldwide)
' / More Info: http://www.g2gnet.com/webboard
' /
' / Purpose: Sample XtraReports of DevExpress. (V.17.1.6)
' / Microsoft Visual Basic .NET (2010) + MS Access
' /
' / This is open source code under @Copyleft by Thongkorn Tubtimkrob.
' / You can modify and/or distribute without to inform the developer.
' / --------------------------------------------------------------------------------
#End Region

Imports DevExpress.XtraReports.UI
Imports System.Data.OleDb
Imports System.Drawing.Printing

Public Class frmUnBoundData
    '// หากเป็นโปรเจคจริงๆ กลุ่มตัวแปรเหล่านี้ต้องนำไปวางไว้ใน Module 
    Dim Conn As OleDb.OleDbConnection
    Dim Cmd As New System.Data.OleDb.OleDbCommand
    Dim strSQL As String

    ' / Get my project path
    Function MyPath(AppPath As String) As String
        '/ MessageBox.Show(AppPath)
        MyPath = AppPath.ToLower.Replace("\bin\debug", "\").Replace("\bin\Release", "\")
    End Function

    ' / ------------------------------------------------------------------
    Public Sub ConnectDataBase()
        Dim strPath As String = MyPath(Application.StartupPath)
        Dim strConn As String = _
            " Provider = Microsoft.ACE.OLEDB.12.0; " & _
            " Data Source = " & strPath & "dbFood.accdb;"
        '//
        Conn = New OleDb.OleDbConnection(strConn)
    End Sub

    Private Sub btnShowReport_Click(sender As System.Object, e As System.EventArgs) Handles btnShowReport.Click
        Call PreviewReport()
    End Sub

    Private Sub PreviewReport()
        '// -----------------------------------------------------------------------------------------
        '// โค้ดชุดนี้นำไปใช้กับ GridControl ได้เหมือนกัน
        '// Join 2 Table between Food & Category.
        strSQL = _
            " SELECT Food.FoodPK, Food.FoodID, Food.FoodName, Food.PriceCash, Food.PictureFood, Category.CategoryName " & _
            " FROM Category INNER JOIN Food ON Category.CategoryPK = Food.CategoryFK " & _
            " ORDER BY Food.FoodID "

        Dim DT As New DataTable
        With DT
            .Columns.Add("FoodPK", GetType(Int32))
            .Columns.Add("FoodID", GetType(String))
            .Columns.Add("FoodName", GetType(String))
            .Columns.Add("PriceCash", GetType(Double))
            .Columns.Add("CategoryName", GetType(String))
            .Columns.Add("PictureFood", GetType(Image))
        End With
        Try
            If Conn.State = ConnectionState.Closed Then Conn.Open()
            Cmd.Connection = Conn
            Cmd.CommandText = strSQL
            Dim DR As OleDbDataReader = Cmd.ExecuteReader
            ' / แสดงผลรูปภาพ
            Dim imgName As Image
            Dim strPath As String = MyPath(Application.StartupPath) & "Images\"
            While DR.Read()
                If DR.HasRows Then
                    '// เช็คจากฐานข้อมูลก่อนว่ามีชื่อไฟล์ภาพหรือไม่
                    If DR.Item("PictureFood").ToString <> "" Then
                        '// ทดสอบว่ามีไฟล์ภาพอยู่หรือไม่
                        If Not System.IO.File.Exists(strPath & DR.Item("PictureFood").ToString) Then
                            ' File dosn't exist. หากไม่เจอไฟล์ภาพให้แสดงผลภาพว่างเปล่าแทน
                            imgName = Image.FromFile(strPath & "NoImage.gif")
                        Else
                            imgName = Image.FromFile(strPath & DR.Item("PictureFood").ToString)
                        End If
                    Else
                        imgName = Image.FromFile(strPath & "NoImage.gif")
                    End If
                    '//
                    DT.Rows.Add(New Object() { _
                            DR.Item("FoodPK").ToString, _
                            DR.Item("FoodID").ToString, _
                            DR.Item("FoodName").ToString, _
                            DR.Item("PriceCash"), _
                            DR.Item("CategoryName"), _
                            imgName})
                End If
            End While
            DR.Close()
            Cmd.Dispose()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        '// -----------------------------------------------------------------------------------------
        '// ------------------------------ Starting to Create Report. -----------------------
        '// Instance Name of XtraReports.
        Dim Report As New XtraReport1() With {
            .Name = "SampleXtraReports",
            .DisplayName = "Sample XtraReports",
            .PaperKind = PaperKind.A4,
            .Margins = New Margins(100, 100, 100, 100)
        }
        '/ Binding Data to XRLabel & XRPictureBox within Report.
        With Report
            .lblFoodID.DataBindings.Add("Text", DT, "FoodID")
            .lblFoodName.DataBindings.Add("Text", DT, "FoodName")
            .lblCategoryName.DataBindings.Add("Text", DT, "CategoryName")
            .lblPriceCash.DataBindings.Add("Text", DT, "PriceCash", "{0:#,##0.00}")
            .picProduct.DataBindings.Add("Image", DT, "PictureFood")
            '// Page No.
            .XrPageInfo1.Format = "Page: {0}/{1}"
            .DataSource = DT
            .CreateDocument()
        End With
        '/ แสดงรายงาน
        Dim Tool As New ReportPrintTool(Report)
        '/ แสดงผล
        Tool.ShowPreview()
        DT.Dispose()
    End Sub

    Private Sub frmUnBoundData_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Conn.State = ConnectionState.Open Then Conn.Close()
        Me.Dispose()
        GC.SuppressFinalize(Me)
        Application.Exit()
    End Sub

    Private Sub frmUnBoundData_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Call ConnectDataBase()
    End Sub
End Class