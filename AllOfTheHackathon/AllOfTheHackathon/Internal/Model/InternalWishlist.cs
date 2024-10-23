namespace AllOfTheHackathon.Internal.Model;

internal readonly record struct InternalWishlist(int EmployeeId, int[] DesiredEmployees)
{
    public bool Equals(InternalWishlist other)
    {
        return EmployeeId == other.EmployeeId;
    }

    public override int GetHashCode()
    {
        return EmployeeId;
    }
}