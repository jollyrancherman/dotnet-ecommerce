namespace Core.Entities.OrderAggregate
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
        }

        public ProductItemOrdered(int productItemId, string pRoductName, string pictureUrl)
        {
            ProductItemId = productItemId;
            PRoductName = pRoductName;
            PictureUrl = pictureUrl;
        }

        public int ProductItemId { get; set; }
        public string PRoductName { get; set; }
        public string PictureUrl { get; set; }
    }
}