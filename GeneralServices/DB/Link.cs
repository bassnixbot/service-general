using System.ComponentModel.DataAnnotations;
using GeneralServices.Models;

namespace GeneralServices.DB;

public class Link : ChatterInformation
{
    [Key]
    public Guid recid {get; set;}
    public string message { get; set; }
    public DateTime savedateutc { get; set; }
}
