using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Data;
using MIVLGUPRO.Model;
using MIVLGUPRO.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MIVLGUPRO.ViewModel
{
    public class TeacherDashboardViewModel
    {
        public ObservableCollection<Group> Groups { get; set; } = new();
        public ObservableCollection<Practice> Practices { get; set; } = new();
        public ObservableCollection<Tasks> Tasks { get; set; } = new();

        private ObservableCollection<User> _selectedGroupStudents;
        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
                LoadStudentsForGroup(); // Загрузка студентов при изменении выбранной группы
                (RemoveStudentCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Обновляем состояние кнопки
            }
        }
        public ObservableCollection<User> SelectedGroupStudents
        {
            get => _selectedGroupStudents;
            set
            {
                _selectedGroupStudents = value;
                OnPropertyChanged(nameof(SelectedGroupStudents));
            }
        }
        public User SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
                (RemoveStudentCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Обновляем команду
            }
        }

        private Group _selectedGroup;

        private User _selectedStudent;


        public ICommand CreateGroupCommand { get; }
        public ICommand CreatePracticeCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand SelectStudentCommand { get; }
        public ICommand RemoveStudentCommand { get; }




        public TeacherDashboardViewModel()
        {
            CreateGroupCommand = new RelayCommand(CreateGroup);
            CreatePracticeCommand = new RelayCommand(CreatePractice);
            AddTaskCommand = new RelayCommand(AddTask);
            SelectStudentCommand = new RelayCommand(SelectStudentForGroup);
            RemoveStudentCommand = new RelayCommand(RemoveStudent, CanRemoveStudent);


            LoadData();
        }


        private void LoadData()
        {
            using var context = new ApplicationDbContext();
            Groups = new ObservableCollection<Group>(context.groups.Include(g => g.Students));
            Practices = new ObservableCollection<Practice>(context.practices.Include(p => p.Group));
            Tasks = new ObservableCollection<Tasks>(context.tasks.Include(t => t.Practice)    .OrderBy(t => t.Name)
            );
        }
        private void LoadStudentsForGroup()
        {
            if (SelectedGroup == null)
            {
                SelectedGroupStudents = new ObservableCollection<User>(); // Очищаем список, если группа не выбрана
                return;
            }

            using var context = new ApplicationDbContext();
            SelectedGroupStudents = new ObservableCollection<User>(
                context.users
                       .Where(u => u.GroupID == SelectedGroup.Id) // Фильтруем студентов по ID группы
                       .OrderBy(u => u.LastName) // Сортировка по фамилии
            );
        }


        private void CreateGroup()
        {
            // Диалог для ввода имени группы
            var groupName = PromptForInput("Введите название группы");
            if (string.IsNullOrWhiteSpace(groupName))
            {
                MessageBox.Show("Группа НЕ созданна");
                return;
            }

            using var context = new ApplicationDbContext();
            var group = new Group
            {
                Name = groupName,
                TeacherId = GetCurrentTeacherId() // Метод для получения текущего ID учителя
            };

            context.groups.Add(group);
            context.SaveChanges();

            Groups.Add(group); // Обновляем локальную коллекцию
        }

        private string PromptForInput(string message)
        {
            // Создаём диалоговое окно
            var dialog = new Window
            {
                Title = "Введите данные",
                Height = 150,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            var textBlock = new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 10) };
            var textBox = new TextBox { Margin = new Thickness(0, 0, 0, 10) };

            var button = new Button
            {
                Content = "OK",
                IsDefault = true,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(button);

            dialog.Content = stackPanel;

            string result = null;

            button.Click += (sender, args) =>
            {
                result = textBox.Text;
                dialog.Close();
            };

            dialog.ShowDialog();

            return result;
        }

        private int GetCurrentTeacherId()
        {
            if (CurrentUser.Role != "Teacher")
            {
                throw new InvalidOperationException("Текущий пользователь не является учителем.");
            }
            return CurrentUser.Id;
        }

        private void CreatePractice()
        {
            if (Groups.Count == 0)
            {
                MessageBox.Show("Группа НЕ созданна");
                return;
            }

            // Выбор группы и ввод названия практики
            var group = PromptForSelection("Выберите группу", Groups);
            var practiceName = PromptForInput("Введите название практики");

            if (group == null || string.IsNullOrWhiteSpace(practiceName))
            {
                return;
            }

            using var context = new ApplicationDbContext();
            var practice = new Practice
            {
                Name = practiceName,
                GroupId = group.Id,
                TeacherId = GetCurrentTeacherId()
            };

            context.practices.Add(practice);
            context.SaveChanges();

            Practices.Add(practice);
        }

        private T PromptForSelection<T>(string message, ObservableCollection<T> items) where T : class
        {
            if (items == null || items.Count == 0)
            {
                // Сообщение об ошибке, если коллекция пуста
                MessageBox.Show("Список пуст.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Создаём диалоговое окно для выбора
            var dialog = new Window
            {
                Title = message,
                Height = 300,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            var listBox = new ListBox
            {
                ItemsSource = items,
                DisplayMemberPath = "Name", // Отображаемое свойство, нужно для корректного отображения
                Margin = new Thickness(0, 0, 0, 10)
            };

            var button = new Button
            {
                Content = "Выбрать",
                IsDefault = true,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(button);

            dialog.Content = stackPanel;

            T selectedItem = null;

            button.Click += (sender, args) =>
            {
                selectedItem = listBox.SelectedItem as T;
                dialog.Close();
            };

            dialog.ShowDialog();

            return selectedItem;
        }

        private void SelectStudentForGroup()
        {
            var selectedGroup = PromptForSelection("Выберите группу", Groups);
            if (selectedGroup == null) return;

            var selectStudentWindow = new SelectStudentView
            {
                DataContext = new SelectStudentViewModel(selectedGroup.Id)
            };

            selectStudentWindow.ShowDialog();

            // Обновляем данные о группах
            LoadData();
        }

        private void AddTask()
        {
            if (Practices.Count == 0)
            {
                MessageBox.Show("Создайте практику");
                return;
            }
            var practice = PromptForSelection("Выберите практику", Practices);
            if (practice == null) return;

            var createTaskWindow = new CreateTaskView
            {
                DataContext = new CreateTaskViewModel(practice.Id)
            };

            createTaskWindow.ShowDialog();

            // Обновляем список заданий после добавления
            LoadData();
        }

        private bool CanRemoveStudent()
        {
            return SelectedStudent != null; // Кнопка активна, если выбран студент
        }

        private void RemoveStudent()
        {
            if (SelectedStudent == null) return;

            using var context = new ApplicationDbContext();
            var student = context.users.FirstOrDefault(u => u.Id == SelectedStudent.Id);
            if (student == null) return;

            student.GroupID = null; // Удаляем связь с группой
            context.SaveChanges();

            SelectedGroupStudents.Remove(SelectedStudent); // Удаляем из списка
            SelectedStudent = null; // Сбрасываем выбор
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}