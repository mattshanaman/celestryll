namespace Pemdas.Models
{
    public class SolveItPuzzle
    {
        public string Equation { get; set; } = string.Empty;
        public List<int> BlankPositions { get; set; } = new();
        public List<int> Solutions { get; set; } = new();
        public bool AllowsExponents { get; set; }
    }
}