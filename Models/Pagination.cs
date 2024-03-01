namespace ASP_RazorWeb.Models {
    public class Pagination {
        public int currentPage{get; set; }
        public int countPage{get; set; }

        public Func<int?,string> generateUrl{get; set; }

    }

}