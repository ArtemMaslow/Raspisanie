﻿namespace Raspisanie.Models
{
    public class Department
    {
        public int CodeOfDepartment { get; set; }
        public string NameOfDepartment { get; set; }
        public Faculty Faculty { get; set; }
    }
}
