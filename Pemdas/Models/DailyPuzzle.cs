using SQLite;

namespace Pemdas.Models
{
    [SQLite.Table("DailyPuzzles")]
    public class DailyPuzzle
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public DateTime PuzzleDate { get; set; }
        
        // Unique puzzle identifier for sharing (e.g., "P20240115-E" for Easy puzzle on Jan 15, 2024)
        [Indexed]
        [SQLite.MaxLength(50)]
        public string PuzzleIdentifier { get; set; } = string.Empty;
        
        // Slot for difficulty (0-7 for 8 difficulty levels per day)
        // This allows multiple puzzles per day, one for each difficulty
        public int DifficultySlot { get; set; }

        public PuzzleMode Mode { get; set; }
        public DifficultyLevel Difficulty { get; set; }

        [SQLite.MaxLength(2000)]
        public string PuzzleData { get; set; } = string.Empty;

        [SQLite.MaxLength(500)]
        public string Solution { get; set; } = string.Empty;

        [SQLite.MaxLength(500)]
        public string? Hint { get; set; }

        public int BasePoints { get; set; }
        public int TimeLimit { get; set; }
    }

    public enum PuzzleMode
    {
        SolveIt,
        BuildIt
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        Creative,
        Tricky,
        Speed,
        Boss,
        Expert  // Advanced: exponentials, logs, basic calculus
    }
}