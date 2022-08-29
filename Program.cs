using TurboGroupMaker2K;

var groupShuffler = new RandomGroupShuffler(args);

for (var i = 1; i <= 52; i++)
{
    Console.WriteLine($"Week {i:00} - {groupShuffler.GetNext()}");
}
