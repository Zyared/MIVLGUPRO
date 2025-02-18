using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIVLGUPRO.Model
{
    public class UserAnswers
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Tasks Task { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Answer { get; set; }

        public bool IsCorrect { get; set; }

        public UserAnswers() { }
    }

}
