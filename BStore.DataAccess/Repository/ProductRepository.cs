using BStore.DataAccess.Data;
using BStore.DataAccess.Repository.IRepository;
using BStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
           
        }
        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
