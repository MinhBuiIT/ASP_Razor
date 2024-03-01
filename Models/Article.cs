using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_RazorWeb.Models {
    public class Article {
        [Key]
        public int Id { get; set; }

        [StringLength(100,MinimumLength = 3,ErrorMessage = "{0} phải từ {2} đến {1}")]
        [Column(TypeName = "nvarchar")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }


        [Column(TypeName = "datetime")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DisplayName("Ngày tạo")]
        public DateTime Created { get; set; }

        [Column(TypeName = "ntext")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DisplayName("Nội dung")]
        public string Content { get; set; }
    }
}