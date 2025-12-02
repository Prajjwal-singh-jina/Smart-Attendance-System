using System.ComponentModel.DataAnnotations;

namespace bolonotoproxy.Models
{
    public class Sign_up
    {
        // The unique ID for each user, automatically managed by the database.
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // This will store the SECURELY HASHED password, not the plain text one.
        [Required]
        public string Password { get; set; }
        public List<Student> Students { get; set; }
        public List<UserToken> UserTokens { get; set; }
    }
}
