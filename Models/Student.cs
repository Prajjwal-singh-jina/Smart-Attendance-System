using bolonotoproxy.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace bolonotoproxy
{
    public class Student
    {
        
        [Key]
        
        public int RollNo { get; set; }
        [Required]
        [DisplayName("Student Name")]
        public string Name { get; set; }

        public string? imageAddress { get; set; }
        public int signupID { get; set; }
        [ForeignKey("signupID")] 
        public Sign_up? teacherid { get; set; }
        [NotMapped]
        public IFormFile? ImageFile {  get; set; }

    }
   
}
 