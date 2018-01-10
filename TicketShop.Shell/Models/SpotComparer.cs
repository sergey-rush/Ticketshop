using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TicketShop.Shell.Models
{
    public class SpotComparer : IEqualityComparer<Spot>
    {
        private Func<Spot, object> KeySelector { get; set; }

        public SpotComparer(Func<Spot, object> expr)
        {
            KeySelector = expr;
        }

        public bool Equals(Spot x, Spot y)
        {
            return KeySelector(x).Equals(KeySelector(y));
        }

        public int GetHashCode(Spot obj)
        {
            return KeySelector(obj).GetHashCode();
        }
    }

    public class SpotIdComparer : IEqualityComparer<Spot>
    {
        public bool Equals(Spot x, Spot y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Spot obj)
        {
            return obj.GetHashCode();
        }
    }
}