namespace TurboGroupMaker2K
{
    public class RandomGroupShuffler
    {
        private const int GroupSize = 3;
        private readonly IList<string> _names;
        private readonly Random _random;
        private readonly ISet<int> _knownGroups = new HashSet<int>();

        public RandomGroupShuffler(string[] names)
        {
            _random = new Random();
            _names = names.ToList();
        }

        public IEnumerable<string> GetGroups()
        {
            var shift = 0;
            while (shift + GroupSize <= _names.Count)
            {
                var index = 0;

                // We find the most recent employee of the batch
                string first = _names.Reverse().Skip(shift).First();

                // We create random groups of two of the remaining employees
                List<List<string>> groups = _names.Reverse()
                    .Skip(shift + 1)
                    .OrderBy(_ => _random.Next())
                    .GroupBy(_ => index++ / (GroupSize - 1))
                    .Select(group => group.ToList())
                    .ToList();

                foreach (List<string> group in groups)
                {
                    // We add the recent employee to the group
                    group.Insert(0, first);

                    // We "finish" the group and make sure we are not returning a duplicate
                    if (TryFillGroupAndReturnIfNeeded(group))
                    {
                        yield return string.Join(", ", group);
                    }
                }

                shift += 1;
            }
        }

        private bool TryFillGroupAndReturnIfNeeded(List<string> group)
        {
            int hashCode = CalculateHashCode(group);

            // If the group is 3 people and has not been generated before
            if (group.Count == GroupSize && _knownGroups.Add(hashCode))
            {
                // We allow returning it
                return true;
            }

            // if the group is incomplete
            if (group.Count == GroupSize - 1)
            {
                foreach (string name in _names.OrderBy(_ => _random.Next()))
                {
                    // Copy it with an empty third member
                    var copy = new List<string>(group)
                    {
                        name
                    };

                    // If the newly build group is satisfactory
                    if (TryFillGroupAndReturnIfNeeded(copy))
                    {
                        // We replace the group passed as a parameter
                        group.Clear();
                        group.AddRange(copy);

                        // This is now a valid group
                        return true;
                    }
                }
            }

            // The group already exists
            // And we cannot create a new group from it
            return false;
        }

        /// <summary>
        /// Combine the group member names and calculate a unique enough hash
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
