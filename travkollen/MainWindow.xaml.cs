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
        int? _currentHorseId;

        public MainWindow()
        {
            InitializeComponent();
            FillComboBoxes();
            //hejsan
        }

        private async void FillComboBoxes()
        {
            List<TrainerViewModel> trainers = await _dbRepo.GetAllTrainerViewModels();

            List<HorseShortViewModel> horses = await _dbRepo.GetShortHorseViewModels();

            HorseShortViewModel dummyHorse = new HorseShortViewModel
            {
                Id = null,
                Name = "** Ingen häst vald **"
            };

            horses.Add(dummyHorse);

            FillCombobox<HorseShortViewModel>(cbHorseSelector, horses);
            FillCombobox<HorseShortViewModel>(cbSire, horses);
            FillCombobox<HorseShortViewModel>(cbDam, horses);
            FillCombobox<TrainerViewModel>(cbTrainer, trainers);

        }

        private async void btnRandomTrack_Click(object sender, RoutedEventArgs e)
        {
            var track = await _dbRepo.GetTrackById(3);

            MessageBox.Show($"Vi fick tillbaka bana: {track?.Name}");
        }

        private async void btnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
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
            try
            {
                HorseDetailsViewModel? horseVM = await _dbRepo.GetHorseDetailsViewModel(8);
                if (horseVM != null)
                {
                    RefreshCurrentHorseInfo(horseVM);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshCurrentHorseInfo(HorseDetailsViewModel horseVM)
        {
            _currentHorseId = horseVM.Id;
            txtHorseName.Text = horseVM.Name;
            txtHorseAge.Text = horseVM.Age.ToString();
            txtTrainerName.Text = horseVM.TrainerName;
            txtTrack.Text = horseVM.TrackName;
            cbSire.SelectedValue = horseVM.SireId;
            cbDam.SelectedValue = horseVM.DamId;
            cbTrainer.SelectedValue = horseVM.TrainerId;

            if (horseVM.ImageUrl == null)
            { 
                imgHorse.Source = new BitmapImage(new Uri("https://img.freepik.com/premium-vector/cute-vector-illustration-horse-drawing-toddlers-colouring-page_925324-6417.jpg")); 
            }
            else
            {
                imgHorse.Source = new BitmapImage(new Uri(horseVM.ImageUrl));
            }

        }

        private async void FillCombobox<T>(ComboBox cb, List<T> list)
        {
            cb.ItemsSource = list;
            cb.DisplayMemberPath = "Name";
            cb.SelectedValuePath = "Id";
        }

        private async void cbHorseSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;

            if (combo.SelectedValue == null)
                return;

            int horseId = (int)combo.SelectedValue;

            HorseDetailsViewModel? horseDetails = await _dbRepo.GetHorseDetailsViewModel(horseId);

            if(horseDetails != null)
            {
                RefreshCurrentHorseInfo(horseDetails);
            }
            else
            {
                MessageBox.Show($"Hästen med id:{horseId} du försöker hämta finns inte längre i databasen. " +
                    $"Uppdaterar gränssnittet med ny data från databasen.");
                FillComboBoxes();
            }
        }

        private async void btnDeleteHorse_Click(object sender, RoutedEventArgs e)
        {
            if (_currentHorseId.HasValue)
            {
                try
                {
                    bool wasDeleted = await _dbRepo.DeleteHorse((int)_currentHorseId);
                    if(wasDeleted)
                    {
                        MessageBox.Show("Hästen är nu borttagen ur databasen.");
                        FillComboBoxes();
                    }
                    else
                    {
                        MessageBox.Show("Hästen kunde INTE tas bort ur databasen.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void btnSaveHorse_Click(object sender, RoutedEventArgs e)
        {
            if (_currentHorseId.HasValue)
            {
                try
                {
                    Horse horse = new Horse
                    {
                        Id = (int)_currentHorseId,
                        Name = txtHorseName.Text,
                        SireId = (int?)cbSire.SelectedValue,
                        DamId = (int?)cbDam.SelectedValue
                    };

                    bool wasUpdated = await _dbRepo.UpdateHorse(horse);
                    if (wasUpdated)
                    {
                        MessageBox.Show("Hästen är nu uppdaterad i databasen.");                        
                    }
                    else
                    {
                        MessageBox.Show("Hästen kunde INTE uppdateras i databasen.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    Horse horse = new Horse
                    {
                        Name = txtHorseName.Text,
                        DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
                        SireId = (int?)cbSire.SelectedValue,
                        DamId = (int?)cbDam.SelectedValue,
                        TrainerId = (int)cbTrainer.SelectedValue
                    };
                    bool wasAdded = await _dbRepo.CreateNewHorse(horse);
                    if (wasAdded)
                    {
                        MessageBox.Show("Hästen är nu tillagd i databasen.");
                    }
                    else
                    {
                        MessageBox.Show("Hästen kunde INTE läggas in i databasen.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}