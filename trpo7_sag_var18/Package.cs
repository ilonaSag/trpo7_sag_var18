using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trpo7_sag_var18
{
    public class Package
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = null!;

        public string Recipient { get; set; } = null!;

        public decimal Weight { get; set; }

        public string Status { get; set; } = null!;

        public bool IsDelivered { get; set; }
    }
}
