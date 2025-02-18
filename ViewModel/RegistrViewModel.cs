using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Data;
using MIVLGUPRO.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MIVLGUPRO.ViewModel
{

    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _selectedRole;
        private string _firstName;
        private string _lastName;
        private string _middleName;


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
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        public string MiddleName
        {
            get => _middleName;
            set
            {
                _middleName = value;
                OnPropertyChanged(nameof(MiddleName));
            }
        }
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
            }
        }

        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string>
    {
        "Teacher", "Student"
    };

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(SelectedRole))
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            // Хэширование пароля
            string passwordHash;
            using (var sha256 = SHA256.Create())
            {
                passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(Password)));
            }

            using (var context = new ApplicationDbContext())
            {
                // Проверка уникальности логина
                if (await context.users.AnyAsync(u => u.Username == Username))
                {
                    MessageBox.Show("Логин занят");
                    return;
                }

                // Сохраняем нового пользователя
                var user = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    Username = Username,
                    PasswordHash = passwordHash,
                    Role = SelectedRole
                };

                context.users.Add(user);
                await context.SaveChangesAsync();

                if (SelectedRole == "Teacher")
                {
                    var teacher = new Teacher
                    {
                        Id = user.Id, // Используем ID, сгенерированный для пользователя
                        Name = Username // Или другое значение, например, вводимое имя
                    };

                    context.teachers.Add(teacher);
                }

                await context.SaveChangesAsync(); // Сохраняем изменения в базе

                MessageBox.Show("Вы успешно зарегистрировались");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

