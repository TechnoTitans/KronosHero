using System;
using System.Collections;

namespace KronosHero.net {
    public static class Collections {
        public static bool Contains(ref Array collection, object item) {
            return Array.IndexOf(collection, item, 0, collection.Length) >= 0;
        }

        public static bool Disjoint(ICollection c1, ICollection c2) {
            int c1Size = c1.Count;
            int c2Size = c2.Count;
            if (c1Size == 0 || c2Size == 0) {
                return true;
            }

            ICollection contains = c2;
            ICollection iterate = c1;
            if (c1Size > c2Size) {
                iterate = c2;
                contains = c1;
            }

            Array containsArray = new object[contains.Count];
            contains.CopyTo(containsArray, 0);

            foreach (object o in iterate) {
                if (Contains(ref containsArray, o)) {
                    return false;
                }
            }

            return true;
        }
    }
}