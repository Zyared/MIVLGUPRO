using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Data;
using MIVLGUPRO.Model;
using MIVLGUPRO.View;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MIVLGUPRO.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _variant;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            private get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Variant
        {
            get => _variant;
            set
            {
                _variant = value;
                OnPropertyChanged(nameof(Variant));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }


        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async () => await LoginAsync());
            NavigateToRegisterCommand = new RelayCommand(OpenRegisterWindow);
        }

        private void OpenRegisterWindow()
        {

            // Открываем окно регистрации
            var registerView = new RegisterView();
            registerView.Show();
        }
        private async Task LoginAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                var user = await context.users.FirstOrDefaultAsync(u => u.Username == Username);

                if (user == null || !VerifyPassword(Password, user.PasswordHash))
                {
                    MessageBox.Show("Неверный логин или пароль");
                    return;
                }

                if (user.Role == "Student")
                {
                    if (!int.TryParse(Variant, out var studentVariant) || studentVariant <= 0)
                    {
                        MessageBox.Show("Неверный вариант");
                        return;
                    }
                }

                // Успешная авторизация

                if (user.Role == "Teacher")
                {
                    // Открыть панель управления учителя
                    CurrentUser.Id = user.Id;
                    CurrentUser.Role = user.Role;
                    CurrentUser.Variant = Convert.ToInt32(Variant);
                    CurrentUser.GroupID = user.GroupID;
                    var teacherDashboard = new TeacherDashboardView();
                    teacherDashboard.Show();


                }
                else if (user.Role == "Student")
                {
                    CurrentUser.Id = user.Id;
                    CurrentUser.Role = user.Role;
                    CurrentUser.Variant = Convert.ToInt32(Variant);
                    CurrentUser.GroupID = user.GroupID;
                    var studentDashboard = new StudentDashboardView();
                    studentDashboard.Show();
                }
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var inputHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword)));
            return inputHash == storedHash;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
