using System;

namespace spike.Models;

public class Pet
{
    public int Pet_id { get; set; }
    public string Name { get; set; }
    public string Breed { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public string Gender { get; set; }
    public string Health { get; set; }
    public string Spayed_Neutered { get; set; }
    public string Vet_Name { get; set; }
    public string Vet_Phone { get; set; }
    public string Vaccines { get; set; }
}