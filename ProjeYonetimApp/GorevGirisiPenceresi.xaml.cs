using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjeYonetimApp
{
    /// <summary>
    /// Interaction logic for GorevGirisiPenceresi.xaml
    /// </summary>
    public partial class GorevGirisiPenceresi : Window
    {
        public GorevGirisiPenceresi()
        {
            InitializeComponent();
        }
        const string connString = @"Server=.\SQLEXPRESS;Database=projeYonetimi;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30;";
        SqlConnection connection;
        SqlCommand komut;
        SqlDataAdapter adapter;
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            this.DialogResult = false;
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void TopBorderDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void KaydetButonu_Click(object sender, RoutedEventArgs e)
        {
            if (OtomasyonNoTextbox.Text == null)
            {
                MessageBox.Show("Otomasyon Numarası boş bırakılamaz.");
                return;
            } else
            {
                try
                {
                    connection = new SqlConnection(connString);
                    string query = "INSERT INTO GorevTablosu(otomasyonNo, sorunTarih, sorunSahibi, firmaAdi, sorunAciklama, cozumAciklama, telNo, sorunDurumu) VALUES (@otomasyonNo, @sorunTarih, @sorunSahibi, @firmaAdi, @sorunAciklama, @cozumAciklama, @telNo, @sorunDurumu)";
                    komut = new SqlCommand(query, connection);
                    komut.Parameters.AddWithValue("@otomasyonNo", OtomasyonNoTextbox.Text);
                    komut.Parameters.AddWithValue("@sorunTarih", DateTime.Today);
                    komut.Parameters.AddWithValue("@sorunSahibi", SorunSahibiTextbox.Text);
                    komut.Parameters.AddWithValue("@firmaAdi", FirmaAdiTextbox.Text);
                    komut.Parameters.AddWithValue("@sorunAciklama", SorunAciklamaTextbox.Text);
                    komut.Parameters.AddWithValue("@cozumAciklama", CozumAciklamaTextbox.Text);
                    komut.Parameters.AddWithValue("@telNo", TelNoTextbox.Text);
                    komut.Parameters.AddWithValue("@sorunDurumu", "Beklemede.");
                    connection.Open();
                    komut.ExecuteNonQuery();
                    connection.Close();
                    this.DialogResult = true;
                }
                catch (Exception ex){
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
