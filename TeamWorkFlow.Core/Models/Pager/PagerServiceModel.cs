namespace TeamWorkFlow.Core.Models.Pager
{
    public class PagerServiceModel
    {
        public int PageSize { get; set; }
        public int TotalProjects { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public PagerServiceModel() { }

        public PagerServiceModel(int totalProjects, int page, int pageSize = 5)
        {
            int totalPages = (int)Math.Ceiling(totalProjects / (decimal)pageSize);
            int currentPage = page;

            int startPage = currentPage - 2;
            int endPage = currentPage + 2;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;

                if (endPage > 5)
                {
                    startPage = endPage - 4;
                }
            }

            PageSize = pageSize;
            TotalProjects = totalProjects;
            TotalPages = totalPages;
            CurrentPage = currentPage;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
