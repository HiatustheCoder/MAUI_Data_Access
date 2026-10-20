using System.Collections.ObjectModel;
using MAUI_Data_Access.DataAccess;
using MAUI_Data_Access.Models;

namespace MAUI_Data_Access
{
    public partial class MainPage : ContentPage
    {
        private readonly PersonData personData;

        public ObservableCollection<Person> People { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();

            personData = new PersonData();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            btnSave.IsEnabled = false;

            try
            {
                await UpdatePeopleList();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                btnSave.IsEnabled = true;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                await DisplayAlert(
                    "Error",
                    "First name cannot be empty",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                await DisplayAlert(
                    "Error",
                    "Last name cannot be empty",
                    "OK");

                return;
            }

            DateTime? selectedDate = dpDateOfBirth.Date;

            if (!selectedDate.HasValue)
            {
                await DisplayAlert(
                    "Error",
                    "Please select a date of birth",
                    "OK");

                return;
            }

            if (selectedDate.Value.Date > DateTime.Today)
            {
                await DisplayAlert(
                    "Error",
                    "Date of Birth cannot be in the future",
                    "OK");

                return;
            }

            btnSave.IsEnabled = false;

            try
            {
                var person = new Person
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    DoB = selectedDate.Value.Date
                };

                await personData.SavePersonAsync(person);

                await UpdatePeopleList();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                btnSave.IsEnabled = true;
            }
        }

        private async Task UpdatePeopleList()
        {
            var people = await personData.GetPeopleAsync();

            People.Clear();

            foreach (var person in people)
            {
                People.Add(person);
            }
        }
    }
}