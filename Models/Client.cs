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
    
}