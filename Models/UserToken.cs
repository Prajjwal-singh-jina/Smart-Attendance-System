using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace bolonotoproxy.Models

{

    public class UserToken

    {

        [Key]

        public int Id { get; set; }



        [Required]

        public string TokenValue { get; set; } // The random key card code



        public DateTime ExpiryDate { get; set; } // When the key card stops working



        // This links the token back to a specific user

        public int UserId { get; set; }

        [ForeignKey("UserId")]

        public Sign_up User { get; set; }

    }

}
