using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PalkaShop1
{
    public partial class Producties : UserControl
    {

        public Producties()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.Padding = new Padding(10);
            this.Cursor = Cursors.Hand;
            this.Height = 180;
            this.MouseEnter += (s, e) =>
            {
                this.BackColor = Color.FromArgb(240, 240, 255);
            };

            this.MouseLeave += (s, e) =>
            {
                this.BackColor = Color.White;
            };


        }
        public void FillData(string title, string desc, string prod, string prov,
                      decimal price, int discount, string unit,
                      int stock, string photo)
        {
            lblTitle.Text = title;
            lblDesc.Text = desc;
            lblProducer.Text = "Производитель: " + prod;
            lblProvider.Text = "Поставщик: " + prov;
            lblUnit.Text = "Ед. измерения: " + unit;
            lblStock.Text = "Кол-во на складе: " + stock;

            // СКИДКА
            if (discount > 0)
            {
                decimal newPrice = price * (1 - (decimal)discount / 100);

                lblPrice.Text = $"Цена: {newPrice:N2} руб.";
                lblDiscount.Text = $"Действующая скидка: {discount}%";

                lblOldPrice.Text = $"{price:N2} руб.";
                lblOldPrice.Font = new Font(lblOldPrice.Font, FontStyle.Strikeout);
                lblOldPrice.ForeColor = Color.Red;
                lblOldPrice.Visible = true;
            }
            else
            {
                lblPrice.Text = $"Цена: {price:N2} руб.";
                lblDiscount.Text = "";
                lblOldPrice.Visible = false;
            }

            // ЗАГРУЗКА ФОТО
            try
            {
                if (!string.IsNullOrEmpty(photo))
                {
                    string photoPath = Path.Combine(Application.StartupPath, photo.Trim());

                    if (File.Exists(photoPath))
                    {
                        pbPhoto.Image = Image.FromFile(photoPath);
                    }
                    else
                    {
                        string stub = Path.Combine(Application.StartupPath, "Res", "picture.png");
                        pbPhoto.Image = File.Exists(stub) ? Image.FromFile(stub) : null;
                    }
                }
                else
                {
                    pbPhoto.Image = null;
                }
            }
            catch
            {
                pbPhoto.Image = null;
            }

            pbPhoto.SizeMode = PictureBoxSizeMode.Zoom;

            // ЦВЕТ ФОНА ПО ТЗ
            if (stock == 0)
                this.BackColor = Color.LightBlue;
            else if (discount > 15)
                this.BackColor = ColorTranslator.FromHtml("#2E8B57");
            else
                this.BackColor = Color.White;
        }

        private void Producties_Load(object sender, EventArgs e)
        {

        }
    }
}
