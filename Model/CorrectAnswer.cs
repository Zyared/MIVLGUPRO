using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIVLGUPRO.Model
{
    public class CorrectAnswer
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Tasks Task { get; set; }

        [Required]
        [MaxLength(255)]
        public string Answer { get; set; }

        public CorrectAnswer() { }
    }
}
