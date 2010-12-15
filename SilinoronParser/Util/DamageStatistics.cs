using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.Util
{
    public static class DamageStatistics
    {
        public static float MaxDamage(List<float> damageTaken)
        {
            float largest = -1;
            foreach (float f in damageTaken)
                if (f > largest)
                    largest = f;
            return largest + (largest / damageTaken.Count) - 1;
        }

        public static float MinDamage(List<float> damageTaken)
        {
            float smallest = float.MaxValue;
            foreach (float f in damageTaken)
                if (f < smallest)
                    smallest = f;
            return smallest - (smallest / damageTaken.Count) + 1;
        }
    }
}
