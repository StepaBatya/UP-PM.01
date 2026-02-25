using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace PalkaShop1
{
    public partial class Form3 : Form
    {
        string cn = @"Data Source=.\SQLEXPRESS;Initial Catalog=PalkaShop;Integrated Security=True";

        int? _id;

        string _img = "";


        public Form3(int? id)
        {
            InitializeComponent();

            _id = id;

            LoadComboBoxes();

            if (_id != null)
                LoadProductData();

            this.Text = _id == null ? "Добавление" : "Редактирование";
        }


        private void LoadComboBoxes()
        {
            using (SqlConnection c = new SqlConnection(cn))
            {
                c.Open();

                SqlDataAdapter da1 =
                    new SqlDataAdapter("SELECT id,name FROM Producer", c);

                var dt1 = new System.Data.DataTable();

                da1.Fill(dt1);

                cmbProducer.DataSource = dt1;
                cmbProducer.DisplayMember = "name";
                cmbProducer.ValueMember = "id";


                SqlDataAdapter da2 =
                    new SqlDataAdapter("SELECT id,name FROM Provider", c);

                var dt2 = new System.Data.DataTable();

                da2.Fill(dt2);

                cmbProvider.DataSource = dt2;
                cmbProvider.DisplayMember = "name";
                cmbProvider.ValueMember = "id";
            }
        }


        private void LoadProductData()
        {
            using (SqlConnection c = new SqlConnection(cn))
            {
                c.Open();

                SqlCommand cmd =
                    new SqlCommand("SELECT * FROM Product WHERE id=@id", c);

                cmd.Parameters.AddWithValue("@id", _id);

                SqlDataReader r = cmd.ExecuteReader();

                if (r.Read())
                {
                    txtTitle.Text = r["name"].ToString();

                    txtPrice.Text = r["price"].ToString();

            

                    txtDescription.Text = r["description"].ToString();

                    txtDiscount.Text = r["discount"].ToString();

                    _img = r["photo"]?.ToString();

                    if (!string.IsNullOrEmpty(_img) && File.Exists(_img))
                        pbPhoto.Image = Image.FromFile(_img);

                    cmbProducer.SelectedValue = r["producerid"];

                    cmbProvider.SelectedValue = r["providerid"];
                }
            }
        }


        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Image|*.png;*.jpg;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _img = ofd.FileName;

                pbPhoto.Image = Image.FromFile(_img);
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection c = new SqlConnection(cn))
                {
                    c.Open();

                    string sql;

                    if (_id == null)
                        sql = @"
INSERT INTO Product
(article,name,unitid,price,providerid,producerid,categoryid,discount,amountinstock,description,photo)
VALUES
(@article,@name,1,@price,@provider,@producer,1,@discount,@stock,@description,@photo)";
                    else
                        sql = @"
UPDATE Product SET
article=@article,
name=@name,
unitid=1,
price=@price,
providerid=@provider,
producerid=@producer,
categoryid=1,
discount=@discount,
amountinstock=@stock,
description=@description,
photo=@photo
WHERE id=@id";

                    SqlCommand cmd = new SqlCommand(sql, c);

                    cmd.Parameters.AddWithValue("@article", Guid.NewGuid().ToString().Substring(0, 6).ToUpper());
                    cmd.Parameters.AddWithValue("@name", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                    cmd.Parameters.AddWithValue("@stock", int.Parse(txtStock.Text));
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@discount",
                        string.IsNullOrEmpty(txtDiscount.Text) ? 0 : int.Parse(txtDiscount.Text));
                    cmd.Parameters.AddWithValue("@photo", _img ?? "");

                    // теперь берем выбранные значения из ComboBox
                    cmd.Parameters.AddWithValue("@provider", cmbProvider.SelectedValue);
                    cmd.Parameters.AddWithValue("@producer", cmbProducer.SelectedValue);

                    if (_id != null)
                        cmd.Parameters.AddWithValue("@id", _id);

                    cmd.ExecuteNonQuery();

                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
