using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Data;
using MIVLGUPRO.Model;

namespace MIVLGUPRO.ViewModel
{
    public class PracticeExecutionViewModel : BaseViewModel
    {
        public ObservableCollection<TaskViewModel> Tasks { get; set; }
        private string _resultsSummary;
        public string ResultsSummary
        {
            get => _resultsSummary;
            set
            {
                _resultsSummary = value;
                OnPropertyChanged(nameof(ResultsSummary));
            }
        }

        public ICommand CheckAnswersCommand { get; }

        private readonly int _practiceId;

        public PracticeExecutionViewModel(int practiceId)
        {
            _practiceId = practiceId;
            LoadTasks();
            CheckAnswersCommand = new RelayCommand(CheckAnswers);
        }

        private void LoadTasks()
        {
            using var context = new ApplicationDbContext();
            Tasks = new ObservableCollection<TaskViewModel>(
                context.tasks
                       .Where(t => t.PracticeId == _practiceId && t.Variant == CurrentUser.Variant) // Фильтрация по варианту
                       .Select(t => new TaskViewModel
                       {
                           TaskId = t.Id,
                           Description = t.Description
                       })
                       .ToList()
            );
        }

        private void CheckAnswers()
        {
            using var context = new ApplicationDbContext();

            int correctCount = 0;

            foreach (var task in Tasks)
            {
                var correctAnswer = context.correctanswers.FirstOrDefault(a => a.TaskId == task.TaskId)?.Answer;
                if (correctAnswer != null)
                {
                    task.IsCorrect = task.StudentAnswer == correctAnswer;
                    if (task.IsCorrect == true) correctCount++;
                }
                else
                {
                    task.IsCorrect = false;
                }
            }

            ResultsSummary = $"Правильных ответов: {correctCount} из {Tasks.Count}";
            OnPropertyChanged(nameof(Tasks));
        }

    }

    public class TaskViewModel : BaseViewModel
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public string StudentAnswer { get; set; }
        public bool? IsCorrect { get; set; } // null: не проверено, true: верно, false: неверно
    }

}
