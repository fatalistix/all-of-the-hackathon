using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Internal.Comparer;

public sealed class EmployeeIdEqualityComparer : IEqualityComparer<Wishlist>
{
    public bool Equals(Wishlist? x, Wishlist? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.EmployeeId == y.EmployeeId;
    }

    public int GetHashCode(Wishlist obj)
    {
        return HashCode.Combine(obj.EmployeeId);
    }
}
