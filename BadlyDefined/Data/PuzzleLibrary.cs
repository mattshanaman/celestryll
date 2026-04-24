namespace BadlyDefined.Data;

/// <summary>
/// Comprehensive puzzle data for BadlyDefined game
/// Contains 60+ puzzles across all difficulty levels
/// </summary>
public static class PuzzleLibrary
{
    public static class Easy
    {
        public static readonly List<(string solution, string definition, string category)> Puzzles = new()
        {
            // Original
            ("TV", "A loud rectangle that makes people stare at walls", "Thing"),
            ("DOG", "Fluffy alarm system with teeth", "Animal"),
            
            // Animals
            ("CAT", "Furry overlord who allows you to live in their house", "Animal"),
            ("BIRD", "Feathered alarm clock that never snoozes", "Animal"),
            ("FISH", "Swimming potato with fins", "Animal"),
            ("COW", "Grass-eating milk factory", "Animal"),
            ("PIG", "Mud enthusiast with a curly tail", "Animal"),
            
            // Things - Technology
            ("PHONE", "Pocket-sized panic machine", "Thing"),
            ("LAPTOP", "Portable procrastination device", "Thing"),
            ("MOUSE", "Hand-sized desk slider", "Thing"),
            ("WIFI", "Invisible house blessing", "Thing"),
            ("EMAIL", "Digital mail that never stops coming", "Thing"),
            
            // Things - Household
            ("BED", "Horizontal life support system", "Thing"),
            ("CHAIR", "Sit stick with legs", "Thing"),
            ("DOOR", "Wall with a handle", "Thing"),
            ("CLOCK", "Time anxiety generator", "Thing"),
            ("LAMP", "Tiny indoor sun", "Thing"),
            ("MIRROR", "Truth-telling wall rectangle", "Thing"),
            
            // Food
            ("PIZZA", "Round cheese and bread relationship", "Thing"),
            ("COFFEE", "Hot bean water that makes you go", "Thing"),
            ("BREAD", "Sliced carb block", "Thing"),
            ("EGG", "Chicken's daily offering", "Thing"),
            
            // People
            ("BABY", "Tiny loud human that leaks", "Person"),
            ("BOSS", "Work opinion enforcer", "Person"),
            ("FRIEND", "Chosen family member", "Person"),
            
            // Places
            ("HOME", "Safe sleeping cave", "Place"),
            ("STORE", "Money exchange building", "Place"),
            ("PARK", "Grass storage area with benches", "Place")
        };
    }

    public static class Medium
    {
        public static readonly List<(string solution, string definition, string category)> Puzzles = new()
        {
            // Original
            ("OFFICE", "Where you go to pretend you're productive", "Place"),
            ("THERAPIST", "Talks to people who don't want to talk", "Person"),
            ("CACTUS", "Green thing that refuses to die", "Plant"),
            
            // Jobs/People
            ("TEACHER", "Professional question answerer who asks more questions", "Person"),
            ("LAWYER", "Professional argument winner", "Person"),
            ("DOCTOR", "Licensed body mechanic", "Person"),
            ("PLUMBER", "Water tube wizard", "Person"),
            ("DENTIST", "Tooth janitor", "Person"),
            ("BARBER", "Hair removal specialist", "Person"),
            ("CHEF", "Food temperature manager", "Person"),
            ("ARTIST", "Professional opinion divider", "Person"),
            
            // Places
            ("AIRPORT", "Expensive waiting room with planes", "Place"),
            ("HOSPITAL", "Sickness hotel", "Place"),
            ("LIBRARY", "Quiet book prison", "Place"),
            ("MUSEUM", "Old stuff storage facility", "Place"),
            ("MALL", "Indoor street with cash registers", "Place"),
            ("SCHOOL", "Knowledge distribution center", "Place"),
            ("SUBWAY", "Underground people tube", "Place"),
            
            // Things - Abstract
            ("MONDAY", "Weekly hope destroyer", "Thing"),
            ("WEEKEND", "Temporary freedom permit", "Thing"),
            ("ALARM", "Morning violence device", "Thing"),
            ("PASSWORD", "Forgotten security word", "Thing"),
            ("MEETING", "Group time waste session", "Thing"),
            
            // Technology
            ("COMPUTER", "Thinking box that heats your desk", "Thing"),
            ("PRINTER", "Paper jam creator", "Thing"),
            ("KEYBOARD", "Fancy letter pusher", "Thing"),
            ("INTERNET", "Global distraction network", "Thing"),
            
            // Plants/Nature
            ("FLOWER", "Temporary pretty plant part", "Plant"),
            ("GRASS", "Carpet that needs trimming", "Plant"),
            ("TREE", "Slow-motion rebellion against gravity", "Plant")
        };
    }

    public static class Hard
    {
        public static readonly List<(string solution, string definition, string category)> Puzzles = new()
        {
            // Original
            ("TRAFFIC LIGHT", "A silent negotiator between chaos and order", "Thing"),
            ("CALENDAR", "A rectangle that holds emotional hostages", "Thing"),
            ("ENGINE", "A metal whisperer that turns heat into movement", "Thing"),
            
            // Abstract Concepts
            ("SOCIAL MEDIA", "A digital confessional booth with ads", "Thing"),
            ("PHOTOGRAPH", "A time-traveling mirror that only shows the past", "Thing"),
            ("DIPLOMA", "A ceremonial stick that makes people pretend to know things", "Thing"),
            ("DEMOCRACY", "Group decision system where everyone complains", "Thing"),
            ("INSOMNIA", "Nighttime subscription to unwanted thoughts", "Thing"),
            ("NOSTALGIA", "Emotional time travel to a place that never existed", "Thing"),
            
            // Professions
            ("PROGRAMMER", "Translates caffeine into code that sometimes works", "Person"),
            ("ACCOUNTANT", "Professional number babysitter", "Person"),
            ("CONSULTANT", "Expensive advice giver who asks questions", "Person"),
            ("POLITICIAN", "Promise maker with selective memory", "Person"),
            ("PHILOSOPHER", "Professional question asker who answers with more questions", "Person"),
            
            // Places
            ("AIRPORT SECURITY", "Theatrical performance requiring shoe removal", "Place"),
            ("WAITING ROOM", "Time dilation chamber with magazines", "Place"),
            ("PARKING LOT", "Car storage maze with never enough spaces", "Place"),
            ("GYM", "Voluntary suffering facility with mirrors", "Place"),
            
            // Technology/Modern
            ("AUTOCORRECT", "Helpful text saboteur", "Thing"),
            ("ALGORITHM", "Digital decision maker that nobody understands", "Thing"),
            ("CRYPTOCURRENCY", "Digital money based on electricity and confusion", "Thing"),
            ("VIDEO CALL", "Modern meeting where everyone is muted", "Thing"),
            
            // Nature/Complex
            ("OXYGEN", "Invisible gas that prevents death", "Thing"),
            ("GRAVITY", "Invisible force that keeps everything down", "Thing"),
            ("EVOLUTION", "Nature's extremely slow update system", "Thing"),
            
            // Abstract/Philosophical
            ("PROCRASTINATION", "Art of doing tomorrow what you should do today", "Thing"),
            ("BUREAUCRACY", "System designed to make simple things complicated", "Thing"),
            ("SARCASM", "The body's natural defense against stupidity", "Thing"),
            ("MIDNIGHT SNACK", "Regret you eat in the dark", "Thing"),
            ("ESCALATOR", "Stairs that gave up on you giving them effort", "Thing")
        };
    }

    /// <summary>
    /// Gets total number of puzzles in library
    /// </summary>
    public static int TotalPuzzles => 
        Easy.Puzzles.Count + 
        Medium.Puzzles.Count + 
        Hard.Puzzles.Count;

    /// <summary>
    /// Gets puzzle by difficulty and index
    /// </summary>
    public static (string solution, string definition, string category)? GetPuzzle(
        int difficultySlot, 
        int index)
    {
        var puzzles = difficultySlot switch
        {
            0 => Easy.Puzzles,
            1 => Medium.Puzzles,
            2 => Hard.Puzzles,
            _ => null
        };

        if (puzzles == null || index < 0 || index >= puzzles.Count)
            return null;

        return puzzles[index];
    }

    /// <summary>
    /// Gets random puzzle for difficulty
    /// </summary>
    public static (string solution, string definition, string category)? GetRandomPuzzle(
        int difficultySlot,
        Random? random = null)
    {
        random ??= new Random();
        
        var puzzles = difficultySlot switch
        {
            0 => Easy.Puzzles,
            1 => Medium.Puzzles,
            2 => Hard.Puzzles,
            _ => null
        };

        if (puzzles == null || puzzles.Count == 0)
            return null;

        var index = random.Next(puzzles.Count);
        return puzzles[index];
    }

    /// <summary>
    /// Gets puzzle for specific date (deterministic based on date)
    /// </summary>
    public static (string solution, string definition, string category)? GetDailyPuzzle(
        DateTime date,
        int difficultySlot)
    {
        var puzzles = difficultySlot switch
        {
            0 => Easy.Puzzles,
            1 => Medium.Puzzles,
            2 => Hard.Puzzles,
            _ => null
        };

        if (puzzles == null || puzzles.Count == 0)
            return null;

        // Use date as seed for consistent daily puzzle
        var daysSinceEpoch = (date.Date - new DateTime(2025, 1, 1)).Days;
        var seed = daysSinceEpoch * 1000 + difficultySlot;
        var random = new Random(seed);
        
        var index = random.Next(puzzles.Count);
        return puzzles[index];
    }
}
