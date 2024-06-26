﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
		
        [ValidateNever]
		public string AppUserId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(AppUserId))]
        public AppUser AppUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public double OrderTotal { get; set; }

        
        public DateTime? ArrivalDate { get; set; }

		[ValidateNever]
		public string? DeliveryEmpId { get; set; }
		[ValidateNever]
		[ForeignKey(nameof(DeliveryEmpId))]
		public AppUser? DeliveryEmp { get; set; }
		[ValidateNever]
		public string? deliveryEmpName { get; set; }

		[ValidateNever]
		public string? CancelledEmpId { get; set; }
		[ValidateNever]
		[ForeignKey(nameof(CancelledEmpId))]
		public AppUser? CancelEmp { get; set; }
		[ValidateNever]
		public string? cancelBy { get; set; }

		// Add arrivalDate, ship by employee, deliver status

		public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set;}

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required] public string? Name { get; set; }
        [Required] public string? PhoneNumber { get; set; }
        [Required] public string? HomeNumber { get; set; }
        [Required] public string? StreetName { get; set; }
        [Required] public string? Village { get; set; }
        [Required] public string? Commune { get; set; }
        [Required] public string? City { get; set; }
        [Required] public string? PostalNumber { get; set; }
    }
}
