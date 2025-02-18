using MIVLGUPRO.ViewModel;
using System.Windows;


namespace MIVLGUPRO.View
{
    /// <summary>
    /// Логика взаимодействия для StudentDashboardView.xaml
    /// </summary>
    public partial class StudentDashboardView : Window
    {
        public StudentDashboardView()
        {
            InitializeComponent();
            DataContext = new StudentDashboardViewModel();
        }
    }
}
