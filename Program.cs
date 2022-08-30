using TurboGroupMaker2K;

var groupShuffler = new RandomGroupShuffler(args);

var i = 1;
foreach (string group in groupShuffler.GetGroups())
{
    Console.WriteLine($"Week {i++:00} - {group}");
}
