using P133Allup.Models;

namespace P133Allup.ViewModels
{
    public class PageNatedList<T> : List<T>
    {
        public PageNatedList(List<T> queries, int pageindex, int totalPage)
        {
            PageIndex= pageindex;
            TotalPage= totalPage;

            if (totalPage >= 5)
            {
                int start = PageIndex - 2;
                int end = PageIndex + 2;

                if (start <= 0)
                {
                    end = end - (start - 1);
                    start = 1;
                }

                if (end > TotalPage)
                {
                    end = TotalPage;
                    start = totalPage - 4;
                }

                StartPage = start;
                EndPage = end;
            }
            else
            {
                StartPage = 1; 
                EndPage = totalPage;
            }

            this.AddRange(queries);
        }

        public int PageIndex { get; }
        public int TotalPage { get; }
        public int StartPage { get; }
        public int EndPage { get; }
        public bool HasPrev => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPage;

        public static PageNatedList<T> Create(IQueryable<T> query, int pageIndex, int itemCount)
        {
            int totalPage = (int)Math.Ceiling((decimal)query.Count() / itemCount);
            query = query.Skip((pageIndex - 1) * itemCount).Take(itemCount);

            return new PageNatedList<T>(query.ToList(), pageIndex, totalPage);   
        }
    }
}
