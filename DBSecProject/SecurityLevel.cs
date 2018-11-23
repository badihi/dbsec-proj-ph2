using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSecProject
{
    public class SecurityLevel
    {
        public int Class { get; set; }
        public SecurityCategory Category { get; set; }
        public SecurityLevel(int @class, string category)
        {
            Class = @class;
            Category = new SecurityCategory(category);
        }

        public static bool operator >=(SecurityLevel a, SecurityLevel b)
        {
            return a.Class <= b.Class && a.Category.Contains(b.Category);
        }

        public static bool operator <=(SecurityLevel a, SecurityLevel b)
        {
            return a.Class >= b.Class && b.Category.Contains(a.Category);
        }
    }

    public class SecurityCategory
    {
        public List<List<string>> Sets { get; set; } = new List<List<string>>();
        public SecurityCategory(string categorySets)
        {
            foreach (var categorySet in categorySets.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var newSet = new List<string>();
                foreach (var category in categorySet.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    newSet.Add(category);
                }
                Sets.Add(newSet);
            }
        }

        public bool Contains(SecurityCategory otherCategory)
        {
            if (Sets.Count == 0)
                return true;
            if (otherCategory.Sets.Any(set => set.Any(subset => subset == "*")))
                return true;
            return otherCategory.Sets.All(otherSet => Sets.Any(set => set.All(member => otherSet.Contains(member))));
        }
    }
}
