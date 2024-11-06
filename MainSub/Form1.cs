using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainSub
{
    public partial class Form1 : Form
    {
        QLSVms ds = new QLSVms();
        QLSVmsTableAdapters.KETQUATableAdapter adpKetQua = new QLSVmsTableAdapters.KETQUATableAdapter();
        QLSVmsTableAdapters.MONHOCTableAdapter adpMonHoc = new QLSVmsTableAdapters.MONHOCTableAdapter();
        QLSVmsTableAdapters.SINHVIENTableAdapter adpSinhVien = new QLSVmsTableAdapters.SINHVIENTableAdapter();
        QLSVmsTableAdapters.KHOATableAdapter adpKhoa = new QLSVmsTableAdapters.KHOATableAdapter();
        BindingSource bsSV = new BindingSource(); // Làm nguồn dữ liệu cho Main Form
        BindingSource bsKQ = new BindingSource(); // Làm nguồn dữ liệu cho lưới
        int stt = -1;
        public Form1()
        {
            InitializeComponent();
            bsSV.CurrentChanged += BsSV_CurrentChanged;
        }

        private void BsSV_CurrentChanged(object sender, EventArgs e)
        {
            bdnSinhVien.BindingSource = bsSV;
            lblSTT.Text = (bsSV.Position + 1) + "/" + bsSV.Count;
            txtTongDiem.Text = TongDiem(txtMaSV.Text).ToString();
        }

        private object TongDiem(string MSV)
        {
            double kq =0;
            Object td = ds.Tables["KETQUA"].Compute("sum(Diem)", "MaSV='" + MSV + "'");
            //Lưu ý: trường hợp SV không có điểmt hì phương thức Compute trả về giá trị DBNull
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Doc_Du_Lieu();
            Lien_Ket_Dieu_Khien();
            txtTongDiem.Text = TongDiem(txtMaSV.Text).ToString();
        }

        private void Lien_Ket_Dieu_Khien()
        {
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtTongDiem")
                    ctl.DataBindings.Add("text", bsSV, ctl.Name.Substring(3), true);
                else if (ctl is ComboBox)
                    ctl.DataBindings.Add("Selectedvalue", bsSV, ctl.Name.Substring(3), true);
                else if (ctl is DateTimePicker)
                    ctl.DataBindings.Add("value", bsSV, ctl.Name.Substring(3), true);
                else if (ctl is CheckBox)
                    ctl.DataBindings.Add("checked", bsSV, ctl.Name.Substring(3), true);
        }

        private void Doc_Du_Lieu()
        {
            //1. nạp dữ liệu cho các DataTable
            adpKhoa.Fill(ds.KHOA);
            adpSinhVien.Fill(ds.SINHVIEN);
            adpMonHoc.Fill(ds.MONHOC);
            adpKetQua.Fill(ds.KETQUA);

            //2. nạp nguồn cho ComboxKhoa
            cboMaKH.DisplayMember = "TenKH";
            cboMaKH.ValueMember = "MaMH";
            cboMaKH.DataSource = ds.KHOA;

            //3. Nạp nguồn cho BindingSource bsSV
            bsSV.DataSource = ds.SINHVIEN;

            //4. Nạp nguồn cho BindingSource bsKQ
            bsKQ.DataSource = bsSV;
            bsKQ.DataMember = "SINHVIENKETQUA";

            //5. gán nguồn cho lưới
            DgvKetqua.DataSource = bsKQ;

            //6. Không hiển thị cột MaSV trong lưới
            DgvKetqua.Columns["MaSV"].Visible = false;
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bsSV.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bsSV.MoveNext();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaSV.ReadOnly = false;
            stt= bsSV.Position;
            bsSV.AddNew();
            txtMaSV.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            bsSV.CancelEdit();
            bsSV.Position = stt;
            txtMaSV.ReadOnly = true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //Kiểm tra có tồn tại các mẫu tin có liên quan trong KETQUA hay không ?
            QLSVms.SINHVIENRow rSV = (bsSV.Current as DataRowView).Row as QLSVms.SINHVIENRow;
            if (rSV.GetKETQUARows().Length>0)
            {
                MessageBox.Show("Sinh Viên: " + "\r\n" +
                    "   + MaSV: " + txtMaSV.Text + "\r\n" +
                    "   + Họ và tên: " + txtHoSV.Text + " " + txtTenSV.Text + "\r\n" +
                    "này đã thi, không huỷ được !", "Thông báo không huy Sinh viên",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //Được xoá SV
            DialogResult tl;
            tl = MessageBox.Show("Bạn có muốn Xoá sinh viên: " + "\r\n" +
                "   + MaSV: " + txtMaSV.Text + "\r\n" +
                "   + Họ và tên: " + txtHoSV.Text + ' ' + txtTenSV.Text + "\r\n" +
                "không (Y/N) ?", "Cẩn thận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(tl == DialogResult.Yes)
            {
                //Xoá trong DataTable
                bsSV.RemoveCurrent();
                //Xoá trong CSDL
                int n = adpSinhVien.Update(ds.SINHVIEN);
                if (n > 0)
                    MessageBox.Show("Xoá Sinh Viên:" + "\r\n" +
                        "   + MaSV: " + txtMaSV.Text + "\r\n" +
                        "   + Họ và tên: " + txtHoSV.Text + " " + txtTenSV.Text + "\r\n" +
                        "thành công", "Thông báo Xoá Sinh Viên",MessageBoxButtons.YesNo);
            }    
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if ( txtMaSV.ReadOnly == false) // Thêm mới
            {
                QLSVms.SINHVIENRow rSV = ds.SINHVIEN.FindByMaSV(txtMaSV.Text);
                if (rSV != null) 
                {
                    MessageBox.Show("Mã Sinh Viên: " + txtMaSV.Text + " vừa nhập bị trùng, mời nhập lại", "Thông báo lỗi trùng MaSV",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaSV.Clear();
                    txtMaSV.Focus();
                    return;
                } 
            }
            txtMaSV.ReadOnly = true;
            // Cập nhật lại việc thêm mới hay sửa trong Data Table
            bsSV.EndEdit();
            // Cập nhật lại trong CSDL
            int n = adpSinhVien.Update(ds.SINHVIEN);
            if (n > 0)
                MessageBox.Show("Cập nhật (THÊM/SỬA) cho sinh viên " + "\r\n" +
                    "   + MaSV: " + txtMaSV.Text + "\r\n" +
                    "   + Họ và tên: " + txtHoSV.Text + " " + txtTenSV.Text + "\r\n" +
                    "thành công", "Cập nhật sinh viên thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DgvKetqua_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Xử lý khi cập nhật trên DataGridView
            //1. khi là dòng trống, thì không làm gì hết nếu Click chọn dòng khác
            if (DgvKetqua.CurrentRow.IsNewRow == true) return;
            //2. Dòng có chỉnh sửa: Thêm mới hay dòng đang chỉnh sửa
            if(DgvKetqua.IsCurrentRowDirty==true)
            {
                if((DgvKetqua.CurrentRow.DataBoundItem as DataRowView).IsNew == true)
                {
                    //Kiểm tra khoá chính có bị trùng hay không
                    if(ds.KETQUA.FindByMaSVMaMH(DgvKetqua.CurrentRow.Cells["MaSV"].Value.ToString(),
                        DgvKetqua.CurrentRow.Cells["colMaMon"].Value.ToString()) != null)
                    {
                        MessageBox.Show("Môn học này, sinh viên đã thi,mời nhập lại","Thông báo lỗi bị trùng Mã môn học", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        //Cho ô Mã MH là hiện hành
                        DgvKetqua.CurrentCell = DgvKetqua.CurrentRow.Cells["colMaMon"];
                        return;
                    }    
                }
            //Kết quả quá trình chỉnh sửa
            (DgvKetqua.CurrentRow.DataBoundItem as DataRowView).EndEdit();
                // Cập nhật về CSDL
                int n = adpKetQua.Update(ds.KETQUA);
                if (n > 0)
                    MessageBox.Show("Cập nhật điểm thi cho sinh viên thành công", "Cập nhật kết quả thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void DgvKetqua_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            // Cập nhật về CSDL
            int n = adpKetQua.Update(ds.KETQUA);
            if (n > 0)
                MessageBox.Show("Cập nhật điểm thi cho sinh viên thành công", "Cập nhật kết quả thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
