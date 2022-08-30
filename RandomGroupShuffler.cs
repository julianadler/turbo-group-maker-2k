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
                string first = _names.Reverse().Skip(shift).First();
                List<List<string>> groups = _names.Reverse()
                    .Skip(shift + 1)
                    .OrderBy(_ => _random.Next())
                    .GroupBy(_ => index++ / (GroupSize - 1))
                    .Select(group => group.ToList())
                    .ToList();

                foreach (List<string> group in groups)
                {
                    group.Insert(0, first);
                    if (TryFillGroupAndReturnIfNeeded(group))
                    {
                        yield return string.Join(", ", group);
                    }
                }

                shift += 1;
            }
        }

        private bool TryFillGroupAndReturnIfNeeded(List<string> group,
            bool tryFill = true)
        {
            int hashCode = CalculateHashCode(group);
            if (group.Count == GroupSize && _knownGroups.Add(hashCode))
            {
                return true;
            }

            if (tryFill && group.Count != GroupSize)
            {
                List<string> copy = group.ToList();
                copy.Add(string.Empty);
                foreach (string name in _names.OrderBy(_ => _random.Next()))
                {
                    copy[GroupSize - 1] = name;
                    if (TryFillGroupAndReturnIfNeeded(copy, false))
                    {
                        group.Clear();
                        group.AddRange(copy);
                        return true;
                    }
                }
            }

            return false;
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
