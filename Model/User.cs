using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIVLGUPRO.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(10)]
        public string Role { get; set; } // "Teacher" or "Student"

        public int? GroupID { get; set; }

        [ForeignKey("GroupID")]
        public Group Group { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } // Имя

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } // Фамилия

        [MaxLength(50)]
        public string MiddleName { get; set; } // Отчество 

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim(); // Для отображения ФИО

        public User() { }
    }
}
