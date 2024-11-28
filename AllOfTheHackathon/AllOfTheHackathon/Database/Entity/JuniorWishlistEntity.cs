using System.ComponentModel.DataAnnotations.Schema;

namespace AllOfTheHackathon.Database.Entity;

public class JuniorWishlistEntity
{
    public int Id { get; set; }
    [ForeignKey("Juniors")]
    public int EmployeeId { get; set; }
    [ForeignKey("TeamLeads")]
    public int[] DesiredEmployee { get; set; }
}
