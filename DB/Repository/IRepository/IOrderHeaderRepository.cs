﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceWebAppProject.Models;

namespace EcommerceWebAppProject.DB.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null);
        void UpdateStripePayment(int id, string SessionId, string PaymentIntentId);
    }
}
