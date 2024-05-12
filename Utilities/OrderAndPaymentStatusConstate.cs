using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Utilities
{
	public class OrderAndPaymentStatusConstate
	{
		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";

		public const string StatusInProcess = "Processing";
		public const string StatusDelivered = "Delivered";

		public const string StatusCancelled = "Cancelled";
		
		public const string StatusRefunded = "Refunded";

		// public const string StatusDelivering = "Delivering";


		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";

	}
}
