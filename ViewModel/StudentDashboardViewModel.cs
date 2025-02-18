using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MIVLGUPRO.Data;
using MIVLGUPRO.Model;
using MIVLGUPRO.View;

namespace MIVLGUPRO.ViewModel
{
    public class StudentDashboardViewModel : BaseViewModel
    {
        private Practice _selectedPractice;
        public ObservableCollection<Practice> Practices { get; set; }
        public Practice SelectedPractice
        {
            get => _selectedPractice;
            set
            {
                _selectedPractice = value;
                OnPropertyChanged(nameof(SelectedPractice));
                (StartPracticeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand StartPracticeCommand { get; }

        public StudentDashboardViewModel()
        {
            LoadPractices();
            StartPracticeCommand = new RelayCommand(StartPractice, CanStartPractice);
        }

        private void LoadPractices()
        {
            using var context = new ApplicationDbContext();
            Practices = new ObservableCollection<Practice>(
                context.practices
                       .Include(p => p.Group)
                       .Where(p => p.GroupId == CurrentUser.GroupID) // Практики для группы студента
                       .ToList()
            );
        }

        private bool CanStartPractice()
        {
            return SelectedPractice != null;
        }

        private void StartPractice()
        {
            if (SelectedPractice == null) return;

            // Открываем окно выполнения заданий
            var practiceWindow = new PracticeExecutionView
            {
                DataContext = new PracticeExecutionViewModel(SelectedPractice.Id)
            };
            practiceWindow.ShowDialog();
        }
    }
}
