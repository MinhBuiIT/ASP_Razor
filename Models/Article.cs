using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_RazorWeb.Models {
    public class Article {
        [Key]
        public int Id { get; set; }

        [StringLength(100,MinimumLength = 3)]
        [Column(TypeName = "nvarchar")]
        [Required]
        public string Title { get; set; }


        [Column(TypeName = "datetime")]
        [Required]
        public DateTime Created { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Content { get; set; }
    }
}