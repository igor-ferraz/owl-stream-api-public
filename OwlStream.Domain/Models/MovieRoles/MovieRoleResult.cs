namespace OwlStream.Domain.Models.MovieRoles;

public class MovieRoleResult : MovieRoleUpdate
{
    public bool Active { get; set; }
    public DateTime CreationDate { get; set; }
}