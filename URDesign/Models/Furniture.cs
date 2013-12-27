using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using PagedList;
using PagedList.Mvc;

namespace URDesign.Models
{
    public class Furniture
    {
        [Key]
        public int IdFurniture { get; set; }
        public string Name { get; set; }
        public string Vendor { get; set; }
        public string ImagePath { get; set; }
        public string VendorId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        
    }



    public class FurnitureDBContext : DbContext
    {
        public DbSet<Furniture> Furniture { get; set; }

        public IQueryable<Furniture> FindAllProducts()
        {
            return Furniture.AsQueryable().OrderBy(furniture => furniture.IdFurniture);
        }

    }
}