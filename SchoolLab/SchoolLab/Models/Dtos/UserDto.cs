namespace SchoolLab.Models;

public enum UserRole
{
    SchoolUser = 0,
    SchoolAdmin = 1,
}

public class UserDto
{
    public string Id { get; }
    public string Username { get; }
    public string Name { get; }
    public string Surname { get; }
    public string Token { get; }
    public UserRole UserRole { get; }
    public int HoursOfUse { get; }
    public int MaxHoursBookingAllowed { get; }

    public UserDto(string id, string username, string name, string surname, string token, UserRole userRole = UserRole.SchoolUser, int hoursOfUse = 0, int maxHoursBookingAllowed = 2)
    {
        Id = id;
        Username = username;
        Name = name;
        Surname = surname;
        Token = token;
        UserRole = userRole;
        HoursOfUse = hoursOfUse;
        MaxHoursBookingAllowed = maxHoursBookingAllowed;
    }    
}