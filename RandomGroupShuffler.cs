namespace TurboGroupMaker2K
{
    public class RandomGroupShuffler
    {
        private const int GroupSize = 3;
        private const int MaxAttempts = 10000;
        private int _attempts;
        private readonly IList<string> _names;
        private readonly Random _random;
        private readonly ISet<int> _knownGroups = new HashSet<int>();
        private readonly string _lastJoined;

        public RandomGroupShuffler(string[] names)
        {
            _random = new Random();
            _names = names.ToList();
            _lastJoined = names.Last();
        }

        public string GetNext()
        {
            // While we haven't tried MaxAttempts times
            while (_attempts < MaxAttempts)
            {
                _attempts += 1;
                var index = 0;

                // Generate groups randomly
                List<List<string>> randomizedGroups = _names
                    .OrderBy(_ => _random.Next())
                    .GroupBy(_ => index++ / GroupSize)
                    .Select(x => x.ToList())
                    .ToList();

                // Get the hash codes of the groups of size larger than one
                List<int> hashCodes = randomizedGroups
                    .Where(x => x.Count != 1)
                    .Select(CalculateHashCode)
                    .ToList();

                // Find the only 1 person group member if they exist
                string loner = randomizedGroups.FirstOrDefault(x => x.Count == 1)?.FirstOrDefault();

                // If the group has never been formed before
                // And if the loner is not the last added employee
                if ((loner == null || loner != _lastJoined) && !IsKnownHashCode(hashCodes))
                {
                    // Remember all the calculated hash codes
                    hashCodes.ForEach(hc => _knownGroups.Add(hc));

                    // Join the group members as strings
                    List<string> groups = randomizedGroups
                        .Select(g => string.Join(", ", g))
                        .ToList();

                    // Join the group and return
                    return string.Join(" / ", groups);
                }
            }

            // Happens if the list of names is too short and we cannot generate anymore valid
            return "No luck :)";
        }

        /// <summary>
        /// Check if we have ever encountered the given group hash code
        /// </summary>
        private bool IsKnownHashCode(List<int> hashCodes)
        {
            var containsAny = false;

            foreach (int hashCode in hashCodes)
            {
                containsAny |= _knownGroups.Contains(hashCode);
            }

            return containsAny;
        }

        /// <summary>
        /// Combine the team member names and calculate a unique enough hash
        /// </summary>
        private static int CalculateHashCode(List<string> grouping)
        {
            var hc = new HashCode();

            foreach (string value in grouping.OrderBy(x => x))
            {
                hc.Add(value);
            }

            return hc.ToHashCode();
        }
    }
}
