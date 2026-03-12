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
using travkollen.ViewModels;

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
            FillComboBoxes();
            
        }

        private async void FillComboBoxes()
        {
            List<HorseShortViewModel> horses = await _dbRepo.GetShortHorseViewModels();
            FillCombobox<HorseShortViewModel>(cbHorseSelector, horses);
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

        private async void btnUpdatePerson_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNameOfPerson.Text;
            DateOnly date = DateOnly.Parse(txtDateOfBirth.Text);

            Person person = new Person
            {
                Name = name,
                DateOfBirth = date
            };

            bool svar = await _dbRepo.UpdatePerson(person);

            MessageBox.Show(svar.ToString());
        }

        private async void btnGetAllHorseViewModels_Click(object sender, RoutedEventArgs e)
        {
            HorseDetailsViewModel? horseVM = await _dbRepo.GetHorseDetailsViewModel(18);
        }

        private async void FillCombobox<T>(ComboBox cb, List<T> list)
        {
            cb.ItemsSource = list;
            cb.DisplayMemberPath = "Name";
            cb.SelectedValuePath = "Id";
        }
    }
}