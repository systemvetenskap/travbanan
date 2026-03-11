using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using travkollen.Models;
using travkollen.repositories;

namespace travkollen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbRepository _dbRepo = new DbRepository(App.DataSource);

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnRandomTrack_Click(object sender, RoutedEventArgs e)
        {
            var track = await _dbRepo.GetTrackById(3);

            MessageBox.Show($"Vi fick tillbaka bana: {track?.Name}");
        }

        private async void btnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNameOfPerson.Text;
            DateOnly date = DateOnly.Parse(txtDateOfBirth.Text);

            Person person = new Person
            {
                Name = name,
                DateOfBirth = date
            };

            bool svar = await _dbRepo.CreateNewPerson(person);

            MessageBox.Show(svar.ToString());
        }
    }
}