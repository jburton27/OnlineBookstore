namespace OnlineBookstore
{
    public class Book
    {
        public int ID { get; set; }

        public string ISBN { get; set; }

        public int AuthorID { get; set; }

        public int StoreID { get; set; }

        public int GenreID { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public int PublicationYear { get; set; }

        public string Condition { get; set; }

        public string CoverType { get; set; }

        public string? ImagePath { get; set; }
    }
}
