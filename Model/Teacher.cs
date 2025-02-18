using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIVLGUPRO.Model
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("Id")]
        public User User { get; set; } // Связь с таблицей Users

        public ICollection<Group> Groups { get; set; } = new List<Group>(); // Учитель управляет группами
        public ICollection<Practice> Practices { get; set; } = new List<Practice>(); // Учитель создаёт практики

        public Teacher() { }
    }
}
