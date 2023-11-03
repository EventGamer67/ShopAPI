using System.ComponentModel.DataAnnotations;

namespace ShopAPI.Models
{
    public class Item
    {
        public Item(string itemID, string item_categoryID, string item_name, double item_price)
        {
            this.itemID = itemID;
            this.item_categoryID = item_categoryID;
            this.item_name = item_name;
            this.item_price = item_price;
        }

        public string itemID { get; set; }
        public string item_categoryID { get; set; }
        public string item_name { get; set; }
        public double item_price { get; set; }
    }

    public class WPFItem
    {
        public string itemID { get; set; }
        public string item_categoryID { get; set; }
        public string item_name { get; set; }
        public double item_price { get; set; }
        public List<string> item_image { get; set; }
    }

    public class ItemServiceInfo
    {
        [Key]
        public int itemServiceID { get; set; }
        public string itemID { get; set; }
        public string item_image { get; set; }

        public ItemServiceInfo(int itemServiceID, string itemID, string item_image)
        {
            this.itemServiceID = itemServiceID;
            this.itemID = itemID;
            this.item_image = item_image;
        }
    }
}
