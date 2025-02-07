namespace spike.Models;

public class Employee
{
   // private string employeeId;
    public string Name { get; set; }
    //private string email;
   // private string phone;
   // private string address;

   public Employee(string name)
   {
       this.Name = name;
   }
}