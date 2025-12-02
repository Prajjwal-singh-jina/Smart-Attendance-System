using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bolonotoproxy.Models
{
    public class Attendence
    {
        [Key]
        public int Sr_no {  get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string status {  get; set; } //present or absent
        public int Studentid {  get; set; }
        [ForeignKey("Studentid")]
        public Student Student { get; set; }
    }
}
