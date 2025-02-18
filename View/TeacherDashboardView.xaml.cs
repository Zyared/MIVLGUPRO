using MIVLGUPRO.ViewModel;
using System.Windows;

namespace MIVLGUPRO.View
{
    /// <summary>
    /// Логика взаимодействия для TeacherDashboardView.xaml
    /// </summary>
    public partial class TeacherDashboardView : Window
    {
        public TeacherDashboardView()
        {
            InitializeComponent();
            DataContext = new TeacherDashboardViewModel();
        }
    }
}
