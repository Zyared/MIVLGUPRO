using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Data;
using MIVLGUPRO.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MIVLGUPRO.ViewModel
{
    public class SelectStudentViewModel : INotifyPropertyChanged
    {
        private User _selectedStudent;
        private readonly int _groupId;

        public ObservableCollection<User> Students { get; set; }
        public User SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
                (AddToGroupCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Обновляем состояние кнопки
            }
        }

        public ICommand AddToGroupCommand { get; }

        public SelectStudentViewModel(int groupId)
        {
            _groupId = groupId;
            LoadStudents();

            AddToGroupCommand = new RelayCommand(AddToGroup, CanAddToGroup);
        }
        private bool CanAddToGroup()
        {
            return SelectedStudent != null;
        }
        private void LoadStudents()
        {
            using var context = new ApplicationDbContext();
            Students = new ObservableCollection<User>(
                context.users
                       .Include(u => u.Group) // Загружаем данные о группе
                       .Where(u => u.Role == "Student")
                       .OrderBy(u => u.LastName)
            );
        }


        private void AddToGroup()
        {
            if (SelectedStudent == null) return;

            using var context = new ApplicationDbContext();
            var student = context.users.FirstOrDefault(u => u.Id == SelectedStudent.Id);
            if (student == null) return;

            student.GroupID = _groupId;
            context.SaveChanges();

            Students.Remove(SelectedStudent);
            SelectedStudent = null;

            // Обновляем команду
            (AddToGroupCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
