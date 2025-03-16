using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Helpers;
using System.Linq;

namespace Extensions
{
    public static class Extension
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)       
        => self.Select((item, index) => (item, index));
    }
}
