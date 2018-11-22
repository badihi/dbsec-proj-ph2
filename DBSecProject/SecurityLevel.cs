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

            if (Sets.Count == 0)
                Sets = new List<List<string>> { new List<string> { "" } };
        }

        public bool Contains(SecurityCategory otherCategory)
        {
            bool isSubset = false;
            bool isSuperset = false;

            foreach (var set in Sets)
            {
                foreach (var otherSet in otherCategory.Sets)
                {
                    if (set.All(member => otherSet.Contains(member)))
                        isSubset = true;
                    if (otherSet.All(member => set.Contains(member)))
                        isSuperset = true;
                }
            }

            return isSubset && !isSuperset;
        }
    }
}
