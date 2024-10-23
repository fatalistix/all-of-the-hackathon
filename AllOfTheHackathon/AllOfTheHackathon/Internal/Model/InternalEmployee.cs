namespace AllOfTheHackathon.Internal.Model;

internal readonly record struct InternalEmployee(int Id, string name)
{
    public bool Equals(InternalEmployee other)
    {
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}