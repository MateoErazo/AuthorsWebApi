namespace AuthorsWebApi.DTOs
{
    public class PaginationDTO
    {
        private int pageSize = 10;
        private int page = 1;
        private readonly int minimumPageSize = 1;
        private readonly int maximumPageSize = 50;
        private readonly int minimumPage = 1;
        public int Page {
            get 
            {
                return page;
            }
            set
            {
                page = (value < minimumPage) ? minimumPage : value;
            }
        }
        public int PageSize {
            get 
            { 
                return pageSize; 
            }
            set 
            {
                pageSize = (value > maximumPageSize) ? maximumPageSize :
                           (value < minimumPageSize) ? minimumPageSize :   
                            value;
            }
        }


    }
}
