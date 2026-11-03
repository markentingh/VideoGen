using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGen.Data.Entities.Auth
{
    [Table("AppRoles")]
    public class AppRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
