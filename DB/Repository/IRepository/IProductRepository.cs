﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceWebAppProject.Models;

namespace EcommerceWebAppProject.DB.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product pro);
    }
}
