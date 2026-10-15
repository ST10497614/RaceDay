using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public partial class Organiser
{
    public int OrganiserID { get; set; }
    public int organiserId { get; set; }
    [Column("fullName")]
    public string Name { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("createdAT")]
    public DateTime? CreatedAT { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
