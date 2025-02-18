using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MIVLGUPRO.Model
{
    public class Tasks
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int PracticeId { get; set; }

        [ForeignKey("PracticeId")]
        public Practice Practice { get; set; }

        [Required]
        public int Variant { get; set; }

        public Tasks() { }
    }
}
