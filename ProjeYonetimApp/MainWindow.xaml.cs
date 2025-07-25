using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjeYonetimApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SyncTable();
        }
        const string connString = @"Server=.\SQLEXPRESS;Database=projeYonetimi;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30;";
        SqlConnection connection;
        SqlCommand komut;
        SqlDataAdapter adapter;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

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

        private void SyncColors()
        {
            foreach (var item in AsilListe.Items)
            {
                // Satır container'ını al
                var dgRow = AsilListe
                    .ItemContainerGenerator
                    .ContainerFromItem(item) as DataGridRow;

                // Eğer null geldiyse görünürlüğü tetikleyip tekrar dene
                if (dgRow == null)
                {
                    AsilListe.ScrollIntoView(item);
                    dgRow = AsilListe
                        .ItemContainerGenerator
                        .ContainerFromItem(item) as DataGridRow;
                }

                if (dgRow != null && item is DataRowView drv)
                {
                    // SorunDurumu kolonunu oku
                    var durum = drv["sorunDurumu"]?.ToString();

                    // Renkleri ayarla
                    if (durum == "Yüklendi.")
                        dgRow.Background = Brushes.LimeGreen;
                    else if (durum == "Yapıldı.")
                        dgRow.Background = Brushes.Yellow;
                    else
                        dgRow.ClearValue(DataGridRow.BackgroundProperty);
                }
                AsilListe.UpdateLayout();
            }
        }

        private void SyncTable()
        {
            try
            {
                connection = new SqlConnection(connString);
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Couldn't connect to Database" + ex.ToString());
                }
                adapter = new SqlDataAdapter("SELECT * FROM GorevTablosu ORDER BY id DESC", connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                AsilListe.ItemsSource = dataTable.DefaultView;
                connection.Close();
                SyncColors();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database'e erişilemedi. \nHata: " + ex.ToString());
                return;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SyncTable();
        }

        private void YeniGorevButonu_Click(object sender, RoutedEventArgs e)
        {
            GorevGirisiPenceresi yeniSayfa = new GorevGirisiPenceresi();
            bool? result = yeniSayfa.ShowDialog();
            if (result == true) {
                SyncTable();    
            } else if (result == false)
                    {
                return;
            }
        }

        private void GorevSilButonu_Click(object sender, RoutedEventArgs e)
        {
            if (!(AsilListe.SelectedItem is DataRowView row))
            {
                MessageBox.Show("Lütfen önce bir satır seçin.");
                return;
            }

            // 2) ID bilgisini çek
            int id = (int)row["id"];

            string query = "DELETE FROM GorevTablosu WHERE id = @id";
            komut = new SqlCommand(query, connection);
            komut.Parameters.AddWithValue("@id", id);
            connection.Open();
            komut.ExecuteNonQuery();
            connection.Close();
            SyncTable();
        }

        private void YapildiButonu_Click(object sender, RoutedEventArgs e)
        {
            if (!(AsilListe.SelectedItem is DataRowView row))
            {
                MessageBox.Show("Lütfen önce bir satır seçin.");
                return;
            }

            // 2) ID bilgisini çek
            int id = (int)row["id"];

            string query = @"
    UPDATE GorevTablosu
    SET 
      cozumTarih   = @cozumTarih,
      sorunDurumu  = @sorunDurumu
    WHERE id = @id";
            try
            {
                connection = new SqlConnection(connString);
                komut = new SqlCommand(query, connection);
                connection.Open();
                komut.Parameters.AddWithValue("@cozumTarih", DateTime.Today);
                komut.Parameters.AddWithValue("@sorunDurumu", "Yapıldı.");
                komut.Parameters.AddWithValue("@id", id);
                komut.ExecuteNonQuery();
                connection.Close();
                
                SyncTable();
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.Message);
            }
        }

        private void YuklendiButonu_Click(object sender, RoutedEventArgs e)
        {
            if (!(AsilListe.SelectedItem is DataRowView row))
            {
                MessageBox.Show("Lütfen önce bir satır seçin.");
                return;
            }

            // 2) ID bilgisini çek
            int id = (int)row["id"];

            string query = @"
    UPDATE GorevTablosu
    SET 
      yuklemeTarih   = @yuklemeTarih,
      sorunDurumu  = @sorunDurumu
    WHERE id = @id";
            try
            {
                connection = new SqlConnection(connString);
                komut = new SqlCommand(query, connection);
                komut.Parameters.AddWithValue("@yuklemeTarih", DateTime.Today);
                komut.Parameters.AddWithValue("@sorunDurumu", "Yüklendi.");
                komut.Parameters.AddWithValue("@id", id);

                connection.Open();
                komut.ExecuteNonQuery();
                connection.Close();
                
                SyncTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }

        private void FiltreButonu_Click(object sender, RoutedEventArgs e)
        {
            FiltreExpander.IsExpanded = !FiltreExpander.IsExpanded;
            if (FiltreExpander.IsExpanded)
            {
                FiltreExpander.Visibility = Visibility.Visible;
            } else
            {
                FiltreExpander.Visibility = Visibility.Hidden;
            }
                
        }

        private void FiltreyiUygulaButonu_Click(object sender, RoutedEventArgs e)
        {
            FiltreExpander.IsExpanded = !FiltreExpander.IsExpanded;
            FiltreExpander.Visibility = Visibility.Hidden;
            try
            {
                connection = new SqlConnection(connString);
                connection.Open();
                
                if (BekleyenlerCheckbox.IsChecked == true && YapilanlarCheckbox.IsChecked == true && YuklenenlerCheckbox.IsChecked == true)
                {
                    //hepsi
                    SyncTable();
                } else if (BekleyenlerCheckbox.IsChecked == true && YapilanlarCheckbox.IsChecked != true && YuklenenlerCheckbox.IsChecked != true)
                {
                    //sadece bekleyenler
                    string query = "SELECT * FROM GorevTablosu WHERE sorunDurumu = @sorunDurumu";
                    adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu", "Beklemede.");
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AsilListe.ItemsSource = dt.DefaultView;
                    connection.Close();
                    SyncColors();
                } else if(BekleyenlerCheckbox.IsChecked == true && YapilanlarCheckbox.IsChecked == true && YuklenenlerCheckbox.IsChecked != true)
                {
                    //bekleyenler ve yapılanlar
                    string query = "SELECT * FROM GorevTablosu WHERE sorunDurumu = @sorunDurumu1 OR sorunDurumu = @sorunDurumu2";
                    adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu1", "Beklemede.");
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu2", "Yapıldı.");
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AsilListe.ItemsSource = dt.DefaultView;
                    connection.Close();
                    SyncColors();

                }
                else if(BekleyenlerCheckbox.IsChecked == true && YapilanlarCheckbox.IsChecked != true && YuklenenlerCheckbox.IsChecked == true){
                    //bekleyenler ve yüklenenler
                    string query = "SELECT * FROM GorevTablosu WHERE sorunDurumu = @sorunDurumu1 OR sorunDurumu = @sorunDurumu2";
                    adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu1", "Beklemede.");
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu2", "Yüklendi.");
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AsilListe.ItemsSource = dt.DefaultView;
                    connection.Close();
                    SyncColors();

                }
                else if(BekleyenlerCheckbox.IsChecked != true && YapilanlarCheckbox.IsChecked == true && YuklenenlerCheckbox.IsChecked != true){
                    //sadece yapılanlar
                    string query = "SELECT * FROM GorevTablosu WHERE sorunDurumu = @sorunDurumu";
                    adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu", "Yapıldı.");
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AsilListe.ItemsSource = dt.DefaultView;
                    connection.Close();
                    SyncColors();

                }
                else if(BekleyenlerCheckbox.IsChecked != true && YapilanlarCheckbox.IsChecked == true && YuklenenlerCheckbox.IsChecked == true)
                {
                    //yapılanlar ve yüklenenler
                    string query = "SELECT * FROM GorevTablosu WHERE sorunDurumu = @sorunDurumu1 OR sorunDurumu = @sorunDurumu2";
                    adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu1", "Yapıldı.");
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu2", "Yüklendi.");
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AsilListe.ItemsSource = dt.DefaultView;
                    connection.Close();
                    SyncColors();

                }
                else if (BekleyenlerCheckbox.IsChecked != true && YapilanlarCheckbox.IsChecked != true && YuklenenlerCheckbox.IsChecked == true)
                {
                    //sadece yüklenenler
                    string query = "SELECT * FROM GorevTablosu WHERE sorunDurumu = @sorunDurumu";
                    adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@sorunDurumu", "Yüklendi.");
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AsilListe.ItemsSource = dt.DefaultView;
                    connection.Close();
                    SyncColors();

                }
                else
                {
                    return;
                }
                
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void CozumAciklamaCell_DoubleClick(object sender, EventArgs e) {

            DataRowView row = (DataRowView)AsilListe.SelectedItem;
            string text = string.Empty;
            if (row != null)
            {
                TextBlock txtBlock = CozumAciklamaSutunu.GetCellContent(row) as TextBlock;
                if (txtBlock != null)
                {
                    text = txtBlock.Text;
                    
                }
            }
            if (string.IsNullOrEmpty(text)) { return; }
            DetayPenceresi detayGoster = new DetayPenceresi(text);
            detayGoster.ShowDialog();
        }
        private void SorunAciklamaCell_DoubleClick(Object sender, EventArgs e) {
            DataRowView row = (DataRowView)AsilListe.SelectedItem;
            string text = string.Empty;
            if (row != null)
            {
                TextBlock txtBlock = SorunAciklamaSutunu.GetCellContent(row) as TextBlock;
                if (txtBlock != null)
                {
                    text = txtBlock.Text;

                }
            }
            if (string.IsNullOrEmpty(text)) { return; }
            DetayPenceresi detayGoster = new DetayPenceresi(text);
            detayGoster.ShowDialog();
        }

        private void ResimGostermeCell_DoubleClick(Object sender, EventArgs e)
        {
            // 1) Seçili satırı al
            var row = AsilListe.SelectedItem as DataRowView;
            if (row == null)
                return;

            // 2) attachmentPath sütununu oku
            var pathObj = row["attachment"];
            if (pathObj == DBNull.Value)
            {
                MessageBox.Show("Veritabanında resim yolu yok.");
                return;
            }

            string path = pathObj.ToString();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                MessageBox.Show("Resim dosyası bulunamadı:\n" + path);
                return;
            }

            // 3) Dosyadan BitmapImage oluştur
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();

            // 4) Küçük bir pencere açıp resmi göster
            var imgControl = new Image
            {
                Source = bmp,
                Stretch = System.Windows.Media.Stretch.Uniform
            };

            var previewWin = new Window
            {
                Title = "Resim Önizleme",
                Content = imgControl,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            previewWin.ShowDialog();
        }
        }
    }


