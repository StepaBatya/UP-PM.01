using PalkaShop1;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PalkaShop1
{
    public partial class Form4 : Form
    {
        string podkl = @"Data Source=.\SQLEXPRESS;Initial Catalog=PalkaShop;Integrated Security=True";
        int _role;

        public Form4(int role)
        {
            InitializeComponent();
            _role = role;
            LoadOrders();
        }

        public void LoadOrders()
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(podkl))
            {
                try
                {
                    conn.Open();

                    string sql = @"
SELECT o.id,
       os.name AS StatusName,
       RTRIM(p.city) + ', ' + RTRIM(p.street) AS FullAddr,
       o.creationdate,
       o.deliverydate
FROM [Order] o
JOIN OrderStatus os ON o.statusid = os.Id
JOIN PickUpPoint p ON o.pickuppoint = p.Id";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        UserControl1 card = new UserControl1();

                        card.Fill(
                            Convert.ToInt32(r["id"]),
                            r["StatusName"].ToString(),
                            r["FullAddr"].ToString(),
                            Convert.ToDateTime(r["creationdate"]),
                            Convert.ToDateTime(r["deliverydate"])
                        );

                        card.Tag = Convert.ToInt32(r["id"]);

                        // подключаем клики ко ВСЕМ элементам карточки
                        AddClick(card);

                        flowLayoutPanel1.Controls.Add(card);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка БД: " + ex.Message);
                }
            }
        }

        // универсальный обработчик кликов карточки
        void AddClick(Control parent)
        {
            parent.Click += Card_Click;

            foreach (Control c in parent.Controls)
                AddClick(c);
        }

        void Card_Click(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;

            while (ctrl != null && !(ctrl is UserControl1))
                ctrl = ctrl.Parent;

            if (ctrl is UserControl1 card && card.Tag != null)
                OpenEdit(Convert.ToInt32(card.Tag));
        }

        void OpenEdit(int? orderId)
        {
            Form5 f5 = new Form5(orderId);
            if (f5.ShowDialog() == DialogResult.OK)
                LoadOrders();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenEdit(null);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
