namespace Web_PizzaShop.Helpers
{
    public class PagingModel
    {
        public int currentPage { get; set; }
        public int countPages { get; set; }
        public Func<int?, string> generalUrl { get; set; }
    }
}