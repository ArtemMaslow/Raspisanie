using Raspisanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class TeachersAndSubjectsViewHelper<T>
    { 
        public T Value { get; set; }
        public bool IsSelected { get; set; }
    }
}
