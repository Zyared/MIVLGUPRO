using MIVLGUPRO.Data;
using MIVLGUPRO.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MIVLGUPRO.ViewModel
{
    public class CreateTaskViewModel : INotifyPropertyChanged
    {
        private string _taskName;
        private string _taskDescription;
        private string _taskVariant;
        private string _correctAnswer;

        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        public string TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
            }
        }

        public string TaskVariant
        {
            get => _taskVariant;
            set
            {
                _taskVariant = value;
                OnPropertyChanged(nameof(TaskVariant));
            }
        }

        public string CorrectAnswer
        {
            get => _correctAnswer;
            set
            {
                _correctAnswer = value;
                OnPropertyChanged(nameof(CorrectAnswer));
            }
        }

        public ICommand SaveTaskCommand { get; }

        public CreateTaskViewModel(int practiceId)
        {
            SaveTaskCommand = new RelayCommand(() => SaveTask(practiceId));
        }

        private void SaveTask(int practiceId)
        {
            using var context = new ApplicationDbContext();

            if (string.IsNullOrWhiteSpace(TaskName) || string.IsNullOrWhiteSpace(TaskDescription) || string.IsNullOrWhiteSpace(CorrectAnswer))
            {
                MessageBox.Show("Не удалость создать");
                return;
            }

            if (!int.TryParse(TaskVariant, out var variant))
            {
                MessageBox.Show("Неверный номер варианта");
                return;
            }

            var task = new Tasks
            {
                Name = TaskName,
                Description = TaskDescription,
                PracticeId = practiceId,
                Variant = variant
            };

            context.tasks.Add(task);
            context.SaveChanges();

            var correctAnswer = new CorrectAnswer
            {
                TaskId = task.Id,
                Answer = CorrectAnswer
            };

            context.correctanswers.Add(correctAnswer);
            context.SaveChanges();
            MessageBox.Show("задание созданно");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
