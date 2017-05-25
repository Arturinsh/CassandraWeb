using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CassandraWebTest.Models
{
    public class Company
    {
        public string Name { get; set; }
        public string AddressLine { get; set; }
        public string Login { get; set; }
        public float Budget { get; set; }
        public string Password { get; set; }
        public List<Employee> Employees { get; set; }    
    }
    public class Employee
    {
        public string Email { get; set; }
        public int Id { get; set; }
    }

    public class EmployeeReAdd
    {
        public int EmployeeId { get; set; }
        public List<Employee> Employees { get; set; }
    }
}