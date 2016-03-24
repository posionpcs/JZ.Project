using System.Collections.Generic;

namespace FrameWork
{
    public class PageList<T>
    {
        private List<T> items;
        private int pageIndex;
        private int pageSize;
        private int total;
        private int totalPage;

        public PageList(int total, int pageSize, int pageIndex, List<T> items)
        {
            this.total = total;
            this.pageSize = pageSize;
            this.pageIndex = pageIndex;
            this.items = items;
            this.totalPage = ((this.total % this.pageSize) == 0) ? (this.total / this.pageSize) : ((this.total / this.pageSize) + 1);
        }

        public bool HasNext
        {
            get
            {
                return (this.pageIndex < this.totalPage);
            }
        }

        public bool HasPrev
        {
            get
            {
                return (this.pageIndex > 1);
            }
        }

        public List<T> Items
        {
            get
            {
                return this.items;
            }
        }

        public int PageIndex
        {
            get
            {
                return this.pageIndex;
            }
        }

        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
        }

        public int Total
        {
            get
            {
                return this.total;
            }
        }

        public int TotalPage
        {
            get
            {
                return this.totalPage;
            }
        }
    }
}

