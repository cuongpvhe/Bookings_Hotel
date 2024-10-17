namespace Bookings_Hotel.Common
{
    public class Pagination
    {
        public static List<T> GetCurrentPageData<T>(List<T> items, int currentPage, int itemsPerPage)
        {
            int start = (currentPage - 1) * itemsPerPage;
            int end = Math.Min(start + itemsPerPage, items.Count);

            if (start >= items.Count)
            {
                return new List<T>();
            }

            return items.GetRange(start, end - start);
        }
    }
}
