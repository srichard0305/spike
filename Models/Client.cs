using System;
using System.Collections.ObjectModel;

namespace spike.Models;

public class Client
{
    public Int64? ClientId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Address? Address { get; set; }
    public ContactInfo? ContactInfo { get; set; }
    public ObservableCollection<Pet>? Pets { get; set; }

    // for auto complete box 
    // searches for items based on calling tostring on the object in the observable collection 
    public override string ToString()
    {
        return $"{FirstName} {LastName}" + $" - {ContactInfo.PrimaryPhone}" ;
    }
}