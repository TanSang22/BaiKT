using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp6.Models;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {

        private Model1 context;
        private SinhVien SelectedStudent;
        private string connectionString = "your_connection_string_here";
        public Form1()
        {
            InitializeComponent();
        }
        private void loadData()
        {
            context = new Model1();
            List<SinhVien> lstStudent = context.SinhViens.ToList();
            List<Lop> lstFaculty = context.Lops.ToList();
            fillFacultyComboBox(lstFaculty);
            BlindGrid(lstStudent);

        }

        private void BlindGrid(List<SinhVien> lstStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in lstStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.MaSV;
                dataGridView1.Rows[index].Cells[1].Value = item.HotenSV;
                dataGridView1.Rows[index].Cells[2].Value = item.NgaySinh;
                dataGridView1.Rows[index].Cells[3].Value = item.Lop.TenLop;

            }

        }
        private void fillFacultyComboBox(List<Lop> lstfaculty)
        {
            if (lstfaculty != null && lstfaculty.Count > 0)
            {
                cboKhoa.DataSource = lstfaculty; 
                cboKhoa.ValueMember = "MaLop";
                cboKhoa.DisplayMember = "TenLop"; 
            }
            else
            {
                
                cboKhoa.DataSource = null;
                cboKhoa.Items.Clear();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đóng ứng dụng không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true; // Hủy việc đóng form
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) || cboKhoa.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy dữ liệu từ form
                string maSV = txtMaSV.Text.Trim();
                string hoTen = txtHoTen.Text.Trim();
                DateTime ngaySinh = dtpNgaySinh.Value;
                string maLop = cboKhoa.SelectedValue.ToString();

                using (var context = new Model1())
                {
                    // Kiểm tra trùng mã sinh viên
                    var existingStudent = context.SinhViens.FirstOrDefault(s => s.MaSV == maSV);
                    if (existingStudent != null)
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Tạo đối tượng SinhVien mới
                    var newSinhVien = new SinhVien
                    {
                        MaSV = maSV,
                        HotenSV = hoTen,
                        NgaySinh = ngaySinh,
                        MaLop = maLop
                    };

                    // Thêm đối tượng vào cơ sở dữ liệu
                    context.SinhViens.Add(newSinhVien);
                    context.SaveChanges();

                    // Làm sạch form và tải lại dữ liệu
                    ClearForm();
                    loadData();

                    MessageBox.Show("Thêm mới sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var context = new Model1())
                {
                    var studentToDelete = context.SinhViens.Find(txtMaSV.Text);
                    if (studentToDelete != null)
                    {
                        context.SinhViens.Remove(studentToDelete);
                        context.SaveChanges();
                    }
                }
                loadData();
                ClearForm();
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để sửa!");
                return;
            }

            using (var context = new Model1())
            {
                var studentToUpdate = context.SinhViens.Find(txtMaSV.Text);
                if (studentToUpdate != null)
                {
                    // Hiển thị thông tin vào các trường nhập liệu
                    txtHoTen.Text = studentToUpdate.HotenSV;
                    cboKhoa.SelectedValue = studentToUpdate.MaLop;
                    dtpNgaySinh.Value = (DateTime)studentToUpdate.NgaySinh;

                    // Sau đó có thể thêm logic để cập nhật khi người dùng nhấn nút "Cập nhật"
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên với mã số này!");
                }
            }
        }
        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            cboKhoa.SelectedIndex = -1; 
            dtpNgaySinh.Value = DateTime.Now; 
        }

        private void BtnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1()) // Sử dụng Model1 để kết nối với cơ sở dữ liệu
                {
                    // Kiểm tra xem sinh viên đã tồn tại trong cơ sở dữ liệu chưa
                    var existingSinhVien = context.SinhViens.FirstOrDefault(s => s.MaSV == txtMaSV.Text.Trim());

                    if (existingSinhVien == null)
                    {
                        // Nếu sinh viên chưa có, tạo đối tượng SinhVien mới
                        var newSinhVien = new SinhVien
                        {
                            MaSV = txtMaSV.Text.Trim(),
                            HotenSV = txtHoTen.Text.Trim(),
                            NgaySinh = dtpNgaySinh.Value,
                            MaLop = cboKhoa.SelectedValue.ToString()
                        };

                        // Thêm vào cơ sở dữ liệu
                        context.SinhViens.Add(newSinhVien);
                        context.SaveChanges(); // Lưu vào cơ sở dữ liệu
                        MessageBox.Show("Sinh viên đã được thêm vào cơ sở dữ liệu.");
                    }
                    else
                    {
                        // Nếu sinh viên đã có, cập nhật thông tin sinh viên
                        existingSinhVien.HotenSV = txtHoTen.Text.Trim();
                        existingSinhVien.NgaySinh = dtpNgaySinh.Value;
                        existingSinhVien.MaLop = cboKhoa.SelectedValue.ToString();
                        context.SaveChanges(); // Lưu thay đổi
                        MessageBox.Show("Thông tin sinh viên đã được cập nhật.");
                    }

                    // Cập nhật lại giao diện
                    loadData();
                    ClearForm();
                    btnLuu.Enabled = false; // Disable nút Lưu sau khi lưu
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                // Debug log
                Console.WriteLine(value: $"Selected MaSV: {selectedRow.Cells["MaSV"].Value.ToString()}");

                // Cập nhật các trường nhập liệu
                txtMaSV.Text = selectedRow.Cells["MaSV"].Value?.ToString();
                txtHoTen.Text = selectedRow.Cells["HotenSV"].Value?.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(selectedRow.Cells["NgaySinh"].Value);
                cboKhoa.SelectedValue = selectedRow.Cells["MaLop"].Value?.ToString();
            }
        }
    }
}

