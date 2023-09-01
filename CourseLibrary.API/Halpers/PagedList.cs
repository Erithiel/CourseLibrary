namespace CourseLibrary.API.Halpers
{
	public class PagedList<T> : List<T>
	{
		public int CurrentPage { get; private set; }

		public int TotalPages { get; private set; }

		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }

		public bool HasPrevious => (CurrentPage > 1);
		public bool HasNext => (CurrentPage < TotalPages);


		public PagedList(List<T> item, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			CurrentPage = pageNumber;
			PageSize = pageSize;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			AddRange(item);
		}

		public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
		{
			var count = source.Count();
			var item = source.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
			return new PagedList<T>(item, count, pageNumber, pageSize);
		}



	}
}
