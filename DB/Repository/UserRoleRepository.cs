using EcommerceWebAppProject.DB.Data;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Repository
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRoleRepository(AppDbContext db) : base(db)
        {
            // Get db context from dependency injection to work with db
            _dbContext = db;
        }
    }
}
