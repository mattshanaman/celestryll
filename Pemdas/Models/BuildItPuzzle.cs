namespace Pemdas.Models
{
    public class BuildItPuzzle
    {
        public List<int> AvailableDigits { get; set; } = new();
        public int TargetNumber { get; set; }
        public List<string> AcceptedSolutions { get; set; } = new();
        public int MaxParentheses { get; set; }
        public bool AllowsAdvancedOperators { get; set; }
    }
}