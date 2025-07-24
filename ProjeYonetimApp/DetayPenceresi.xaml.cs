using System;
using System.Collections.Generic;
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
    /// Interaction logic for DetayPenceresi.xaml
    /// </summary>
    public partial class DetayPenceresi : Window
    {
        public DetayPenceresi(string fullText)
        {
            InitializeComponent();
            PasteText(fullText);
        }
        private void PasteText(string text)
        {
            DetayText.Text = text;
            
        }
    }
}
