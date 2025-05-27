using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvselAtikTakip
{
    public partial class Form1 : Form
    {
        private static string connectionString = "Server=ŞEYMA\\SQLEXPRESS;Database=atiktakip_;Integrated Security=True;";
        SqlConnection conn = new SqlConnection("Server=ŞEYMA\\SQLEXPRESS;Database=atiktakip_;Integrated Security=True;");
        int aktifKullaniciID = 0;
        public Form1()
        {
            InitializeComponent();
        }

       

        private void btnGiris_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT KullaniciID FROM Kullanicilar WHERE Eposta = @eposta AND Sifre = @sifre", conn);
            cmd.Parameters.AddWithValue("@eposta", txtGirisEposta.Text);
            cmd.Parameters.AddWithValue("@sifre", txtGirisSifre.Text);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                aktifKullaniciID = reader.GetInt32(0);
                MessageBox.Show("Giriş Başarılı");
                tabControl1.SelectedTab = tabPage2;
            }
            else
            {
                MessageBox.Show("Eposta veya şifre hatalı");
            }
            conn.Close();
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Kullanicilar (Ad, Soyad, Eposta, Sifre) VALUES (@ad, @soyad, @eposta, @sifre)", conn);
            cmd.Parameters.AddWithValue("@ad", txtKayitAd.Text);
            cmd.Parameters.AddWithValue("@soyad", txtKayitSoyad.Text);
            cmd.Parameters.AddWithValue("@eposta", txtKayitEposta.Text);
            cmd.Parameters.AddWithValue("@sifre", txtKayitSifre.Text);

            cmd.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Kayit basarili, şimdi giriş yapabilirsiniz");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateAtikTuru();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT AtikTuruID, TurAdi FROM AtikTurleri", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comboBoxAtikTuru.Items.Add(new ComboboxItem
                {
                    Text = reader["TurAdi"].ToString(),
                    Value = reader["AtikTuruID"]
                });
            }
            conn.Close();
            comboBoxBirim.Items.AddRange(new string[] { "kg", "lt", "adet" });
            comboBoxBirim.SelectedIndex = 0;
        }
        private void PopulateAtikTuru()
        {
            comboBoxAtikTuru.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT AtikTuruID, TurAdi FROM AtikTurleri ORDER BY TurAdi ASC";
                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    comboBoxAtikTuru.Items.Add(reader["TurAdi"].ToString());
                                }
                            }
                            else
                            {
                                MessageBox.Show("Veritabanında AtikTurleri tablosunda hiç veri bulunmadı");
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantı hatası veya sorgu hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }
        private void comboBoxAtikTuru_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxAtikTuru.SelectedItem != null)
            {
                string secilenAtikTuru = comboBoxAtikTuru.SelectedItem.ToString();

                MessageBox.Show("Seçilen atık türü: " + secilenAtikTuru);
            }
        }
        private void btnAtikKaydet_Click(object sender, EventArgs e)
        {
            if (aktifKullaniciID == 0)
            {
                MessageBox.Show("Önce giriş yapmalısınız!");
                return;
            }

            ComboboxItem secilen = (ComboboxItem)comboBoxAtikTuru.SelectedItem;
            int atikTuruId = (int)secilen.Value;

            conn.Open();
            SqlCommand cmd = new SqlCommand(@"INSERT INTO AtikKayitlari 
            (KullaniciID, AtikTuruID, Miktar, Birim, AtilmaTarihi) 
            VALUES (@kulid, @turid, @miktar, @birim, @tarih)", conn);

            cmd.Parameters.AddWithValue("@kulid", aktifKullaniciID);
            cmd.Parameters.AddWithValue("@turid", atikTuruId);
            cmd.Parameters.AddWithValue("@miktar", Convert.ToDecimal(txtMiktar.Text));
            cmd.Parameters.AddWithValue("@birim", comboBoxBirim.Text);
            cmd.Parameters.AddWithValue("@tarih", dateTimePicker1.Value);
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Atık başarıyla kaydedildi.");
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT K.KayitID, A.TurAdi, K.Miktar, K.Birim, K.AtilmaTarihi, K.GeriDonusumeGitti " +
                "FROM AtikKayitlari K JOIN AtikTurleri A ON K.AtikTuruID = A.AtikTuruID " +
                "WHERE K.KullaniciID = @kulid", conn);
            da.SelectCommand.Parameters.AddWithValue("@kulid", aktifKullaniciID);

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        
    }
    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return Text;
        }

    }
}
