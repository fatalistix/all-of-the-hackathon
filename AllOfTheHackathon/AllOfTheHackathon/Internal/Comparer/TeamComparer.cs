using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Internal.Comparer;

public sealed class TeamLeadJuniorEqualityComparer : IEqualityComparer<Team>
    {
        public bool Equals(Team? x, Team? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.TeamLead.Id == y.TeamLead.Id && 
                   x.Junior.Id == y.Junior.Id;
        }

        public int GetHashCode(Team obj)
        {
            return HashCode.Combine(obj.TeamLead.Id, obj.Junior.Id);
        }
    }