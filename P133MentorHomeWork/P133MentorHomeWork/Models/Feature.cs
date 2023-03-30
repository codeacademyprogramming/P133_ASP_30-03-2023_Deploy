using System.ComponentModel.DataAnnotations;

namespace P133MentorHomeWork.Models
{
    public class Feature
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string Icon { get; set; }
        [StringLength(255)]
        public string Text { get; set; }
    }
}
