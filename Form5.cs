using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PalkaShop1
{
    public partial class Form5 : Form
    {
        string podkl = @"Data Source=.\SQLEXPRESS;Initial Catalog=PalkaShop;Integrated Security=True";
        int? _id;

        public Form5(int? id)
        {
            InitializeComponent();
            _id = id;

            LoadLists();

            if (_id != null)
            {
                Text = "Редактирование заказа";
                LoadOrderData();
                btnDelete.Visible = true;
            }
            else
            {
                Text = "Добавление заказа";
                btnDelete.Visible = false;
            }
        }

        void LoadLists()
        {
            using (SqlConnection conn = new SqlConnection(podkl))
            {
                conn.Open();

                // статусы
                SqlDataAdapter da1 = new SqlDataAdapter("SELECT Id, Name FROM OrderStatus", conn);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                cmbStatus.DataSource = dt1;
                cmbStatus.DisplayMember = "Name";
                cmbStatus.ValueMember = "Id";

                // пункты выдачи
                SqlDataAdapter da2 = new SqlDataAdapter(
                    "SELECT Id, RTRIM(City) + ', ' + RTRIM(Street) AS Addr FROM pickuppoint", conn);

                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                cmbPoint.DataSource = dt2;
                cmbPoint.DisplayMember = "Addr";
                cmbPoint.ValueMember = "Id";
            }
        }

        void LoadOrderData()
        {
            using (SqlConnection conn = new SqlConnection(podkl))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM [Order] WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", _id);

                SqlDataReader r = cmd.ExecuteReader();

                if (r.Read())
                {
                    cmbStatus.SelectedValue = r["StatusId"];
                    cmbPoint.SelectedValue = r["pickuppoint"];
                    dtpOrder.Value = Convert.ToDateTime(r["CreationDate"]);
                    dtpDelivery.Value = Convert.ToDateTime(r["DeliveryDate"]);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dtpDelivery.Value < dtpOrder.Value)
            {
                MessageBox.Show("Дата доставки не может быть раньше даты заказа");
                return;
            }

            using (SqlConnection conn = new SqlConnection(podkl))
            {
                conn.Open();

                string sql = (_id == null)
                ? @"INSERT INTO [Order]
                  (CreationDate,DeliveryDate,pickUppoint,StatusId,UserId,ReceiptCode)
                  VALUES(@d1,@d2,@p,@s,@u,@r)"
                : @"UPDATE [Order]
                   SET CreationDate=@d1,
                       DeliveryDate=@d2,
                       pickuppoint=@p,
                       StatusId=@s,
                       UserId=@u,
                       ReceiptCode=@r
                   WHERE Id=@id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (_id != null)
                    cmd.Parameters.AddWithValue("@id", _id);

                cmd.Parameters.AddWithValue("@d1", dtpOrder.Value);
                cmd.Parameters.AddWithValue("@d2", dtpDelivery.Value);
                cmd.Parameters.AddWithValue("@p", cmbPoint.SelectedValue);
                cmd.Parameters.AddWithValue("@s", cmbStatus.SelectedValue);

                // пока фиксированные значения
                cmd.Parameters.AddWithValue("@u", 1);
                cmd.Parameters.AddWithValue("@r", new Random().Next(100, 999));

                cmd.ExecuteNonQuery();

                DialogResult = DialogResult.OK;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_id == null) return;

            if (MessageBox.Show("Удалить заказ?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(podkl))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM [Order] WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _id);
                    cmd.ExecuteNonQuery();

                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
