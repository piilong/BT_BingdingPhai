using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BT06_MonHoc_Dataset_CoDinhKieu
{
    public partial class Form1 : Form
    {

        // Khai báo dataset có định kiểu ds
        DsQLSV ds = new DsQLSV();
        // Khai báo các đối tượng DataAdapter
        DsQLSVTableAdapters.MONHOCTableAdapter adpMonHoc = new DsQLSVTableAdapters.MONHOCTableAdapter();
        DsQLSVTableAdapters.KETQUATableAdapter adpKetQua = new DsQLSVTableAdapters.KETQUATableAdapter();
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
            bs.CurrentChanged += Bs_CurrentChanged;
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            lblStt.Text = bs.Position + 1 + "/" + bs.Count;
            txtMax.Text = DiemMax(txtMaMH.Text).ToString();
            txtTSSV.Text = TongSV(txtMaMH.Text).ToString();
        }

        private object TongSV(string MMH)
        {
            double kq = 0;
            Object td = ds.Tables["KETQUA"].Compute("count(diem)", "MaMH='" + MMH + "'");
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private object DiemMax(string MMH)
        {
            double kq = 0;
            Object td = ds.Tables["KETQUA"].Compute("max(diem)", "MaMH='" + MMH + "'");
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Doc_du_lieu();
            Khoi_tao_BindingSource();
            Lien_ket_Dieu_khien();
        }

        private void Lien_ket_Dieu_khien()
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is TextBox && ctl.Name != "txtTSSV" && ctl.Name != "txtMax" && ctl.Name != "txtLoaiMH")
                    ctl.DataBindings.Add("text", bs, ctl.Name.Substring(3), true);

            }
            Binding bdLoaiMH = new Binding("text", bs, "LoaiMH", true);
            bdLoaiMH.Format += BdLoaiMH_Format;
            bdLoaiMH.Parse += BdLoaiMH_Parse;
            txtLoaiMH.DataBindings.Add(bdLoaiMH);
        }

        private void BdLoaiMH_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value == null) return;
            e.Value = e.Value.ToString().ToUpper() == "Bắt buộc" ? true : false;
        }

        private void BdLoaiMH_Format(object sender, ConvertEventArgs e)
        {
            if (e.Value == DBNull.Value || e.Value == null) return;
            e.Value = (Boolean)e.Value ? "Bắt buộc" : "Tùy chọn";
        }

        private void Khoi_tao_BindingSource()
        {
            bs.DataSource = ds;
            //bs.DataMember = ds.MONHOC.TableName;
            bs.DataMember = "MONHOC";
            bdnMonHoc.BindingSource = bs;
        }

        private void Doc_du_lieu()
        {
            adpMonHoc.Fill(ds.MONHOC);
            adpKetQua.Fill(ds.KETQUA);
        }

        private void btnDau_Click(object sender, EventArgs e)
        {
            bs.MoveFirst();
        }

        private void btnCuoi_Click(object sender, EventArgs e)
        {
            bs.MoveLast();
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {

        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            
        }
    }
}
