# 🎮 BadlyDefined - Complete Implementation Guide

## Executive Summary

**BadlyDefined** is a humorous word-guessing game where players decode intentionally misleading definitions. This document outlines the complete 4-phase implementation following the same architecture as Pemdas.

---

## 🎯 Game Overview

### Core Concept:
Players receive a deliberately bad or misleading definition and must guess the correct word/phrase. With each wrong guess:
- Points decrease
- Progressive hints are revealed (category → letters → more letters)
- Attempts tracked

### Example:
```
Definition: "A loud rectangle that makes people stare at walls"
Letters: 2 (__ __)
Category Hint: Thing
First Hint: T_ (letters 0)
Second Hint: TV
Answer: TV
```

---

## 📊 Architecture Overview

### Technology Stack:
- **.NET 10 MAUI** (iOS, Android, Windows, Mac)
- **CommunityToolkit.Mvvm** for MVVM pattern
- **SQLite** for local database
- **Wordle-style monetization** (ads + subscriptions)

### Folder Structure:
```
BadlyDefined/
├── Models/
│   ├── BadDefinition.cs (puzzle model)
│   ├── UserProgress.cs (user stats)
│   ├── DailyCompletion.cs (completion tracking)
│   └── PuzzleState.cs (current game state)
├── Services/
│   ├── DatabaseService.cs (SQLite operations)
│   ├── GameService.cs (game logic)
│   ├── IAdService.cs (ad interface)
│   ├── ISubscriptionService.cs (subscription interface)
│   └── IFeedbackService.cs (haptics/sounds)
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── GameViewModel.cs (main game)
│   ├── ProfileViewModel.cs (stats/settings)
│   └── TestModeViewModel.cs (testing)
├── Pages/
│   ├── GamePage.xaml (main gameplay)
│   ├── ProfilePage.xaml (stats)
│   └── TestModePage.xaml (testing)
├── Resources/
│   ├── Styles/ (Colors.xaml, Styles.xaml)
│   └── Localization/ (AppResources.resx)
├── Platforms/ (Android, iOS, Mac, Windows)
├── App.xaml (application entry)
├── AppShell.xaml (navigation)
└── MauiProgram.cs (DI setup)
```

---

## 📋 Phase 1: Project Setup & Core Models ✅

### Files Created:
1. ✅ **BadlyDefined.csproj** - Project file
2. ✅ **Models/BadDefinition.cs** - Puzzle model
3. ✅ **Models/UserProgress.cs** - User stats
4. ✅ **Models/DailyCompletion.cs** - Completion tracking
5. ✅ **Models/PuzzleState.cs** - Game state

### Next Steps for Phase 1:
- [ ] Create DatabaseService.cs
- [ ] Create GameService.cs
- [ ] Create service interfaces
- [ ] Create sample puzzle data

---

## 📋 Phase 2: Services & Game Logic

### DatabaseService Methods:
```csharp
// Initialization
Task InitializeAsync()
Task<bool> VerifyDatabaseIntegrity()
Task SeedPuzzlesAsync()

// Puzzle Management
Task<BadDefinition?> GetTodaysPuzzleAsync(int difficultySlot)
Task<List<BadDefinition>> GetPuzzlesByDateAsync(DateTime date)

// User Progress
Task<UserProgress> GetUserProgressAsync()
Task UpdateUserProgressAsync(UserProgress progress)
Task UpdateStreakAsync()

// Completion Tracking
Task<bool> RecordCompletionAsync(DailyCompletion completion)
Task<List<int>> GetTodaysCompletedDifficultiesAsync()
Task<bool> HasCompletedDifficultyTodayAsync(int slot)
Task<int> GetTotalPointsEarnedAsync()

// Statistics
Task<CompletionStats?> GetCompletionStatsAsync(int slot, DateTime date)
```

### GameService Methods:
```csharp
// Puzzle Management
Task<(BadDefinition? puzzle, bool alreadyPlayed)> GetTodaysPuzzleAsync(int difficultySlot)
void StartPuzzle()
PuzzleState GetCurrentState()

// Guess Handling
Task<GuessResult> SubmitGuessAsync(string guess)
void RevealHint()
string GetHintText(int hintLevel)
string GetRevealedWord(HashSet<int> revealedIndices)

// Scoring
int CalculatePoints(int attempts, int basePoints)
Task<bool> CompletePuzzleAsync()

// Sharing
string GenerateShareableResult(BadDefinition puzzle, int attempts, int points)
```

---

## 📋 Phase 3: ViewModels & UI

### GameViewModel Properties:
```csharp
// Display
string PuzzleDefinition
string CategoryHint
string RevealedWord
int LetterCount

// Input
string UserGuess
ObservableCollection<GuessAttempt> PreviousGuesses

// State
int AttemptsCount
int MaxAttempts
int CurrentPoints
int CurrentHintLevel
bool PuzzleCompleted

// Stats
int CurrentStreak
int HintTokens
string ElapsedTime
string CurrentPuzzleId

// Difficulty
int SelectedDifficultySlot
bool EasyEnabled
bool MediumEnabled
bool HardEnabled

// Subscription
bool IsSubscribed
```

### GameViewModel Commands:
```csharp
RelayCommand SubmitGuessCommand
RelayCommand UseHintCommand
RelayCommand SelectDifficultyCommand
RelayCommand ShareResultCommand
```

### GamePage UI Sections:
1. **Header Stats** (Streak, Hints, Elapsed Time)
2. **Difficulty Selector** (Easy/Medium/Hard buttons)
3. **Puzzle Display** (Bad definition, letter count)
4. **Revealed Word** (Shows letters with _ _ _ format)
5. **Category Hint** (Person/Place/Thing etc.)
6. **Guess Input** (Text entry field)
7. **Previous Guesses** (List of attempts)
8. **Action Buttons** (Submit, Hint, Share)
9. **Feedback** (Correct/Wrong messages)

---

## 📋 Phase 4: Resources & Polish

### Color Scheme:
```xaml
<!-- Similar to Pemdas -->
<Color x:Key="Primary">#512BD4</Color>
<Color x:Key="Secondary">#DFD8F7</Color>
<Color x:Key="Tertiary">#2B0B98</Color>
<Color x:Key="Success">#28a745</Color>
<Color x:Key="Error">#dc3545</Color>
<Color x:Key="Warning">#ffc107</Color>

<!-- Difficulty Colors -->
<Color x:Key="DifficultyEasy">#4CAF50</Color>
<Color x:Key="DifficultyMedium">#FF9800</Color>
<Color x:Key="DifficultyHard">#F44336</Color>
```

### Localization Strings:
```
AppName = BadlyDefined
TabGame = Play
TabProfile = Profile

Easy = Easy
Medium = Medium
Hard = Hard

BadDefinition = Bad Definition
LetterCount = Letters: {0}
CategoryHint = Category: {0}
AttemptsRemaining = Attempts: {0}/{1}
PointsRemaining = Points: {0}

SubmitGuess = Submit Guess
UseHint = Use Hint
ShareResult = Share Result

Correct = Correct! 🎉
Wrong = Not quite right, try again!
PuzzleCompleted = Puzzle Completed!
```

---

## 🎮 Game Mechanics

### Hint System:

**Level 0 (Default):**
- Bad definition displayed
- Letter count shown
- No other hints

**Level 1 (After 1st wrong guess):**
- Category revealed (Person, Place, Thing, Animal, Plant, Job, etc.)
- Points reduced by 20%

**Level 2 (After 2nd wrong guess):**
- 1-2 letters revealed in word
- Points reduced by another 20%

**Level 3 (After 3rd wrong guess):**
- 2-3 more letters revealed
- Points reduced by another 20%

**Level 4+ (After 4th+ wrong guesses):**
- Additional letters revealed
- Minimum points retained

### Point System:

| Difficulty | Base Points | Penalty per Wrong Guess |
|-----------|-------------|------------------------|
| Easy | 100 | -20 per guess |
| Medium | 200 | -40 per guess |
| Hard | 300 | -60 per guess |

**Minimum Points:** 10 (never goes below)

**Example:**
```
Medium puzzle (200 points):
- Correct on 1st try: 200 points
- Correct on 2nd try: 160 points (200 - 40)
- Correct on 3rd try: 120 points (200 - 80)
- Correct on 4th try: 80 points (200 - 120)
- Correct on 5th try: 40 points (200 - 160)
- Correct on 6th try: 10 points (minimum)
```

---

## 📱 Monetization (Wordle-Style)

### Free Users:
- Ad before EVERY puzzle
- Can play all 3 difficulties per day
- First puzzle free, subsequent require watching ad
- Limited hint tokens (earn 1 per day)

### Subscribers ($2.99/month):
- No ads
- Unlimited hint tokens
- Access to all difficulties instantly
- Priority support
- Badge/icon in profile

### Ad Flow:
```
1. User opens app → Ad plays (15-30 sec)
2. Puzzle loads
3. User switches difficulty → Ad plays
4. New puzzle loads
```

---

## 🗃️ Sample Puzzle Database

### Easy Examples:
```json
{
  "solution": "TV",
  "definition": "A loud rectangle that makes people stare at walls",
  "category": "Thing",
  "letterCount": 2,
  "basePoints": 100
}

{
  "solution": "DOG",
  "definition": "Fluffy alarm system with teeth",
  "category": "Animal",
  "letterCount": 3,
  "basePoints": 100
}

{
  "solution": "PHONE",
  "definition": "Pocket-sized panic machine",
  "category": "Thing",
  "letterCount": 5,
  "basePoints": 100
}
```

### Medium Examples:
```json
{
  "solution": "THERAPIST",
  "definition": "Talks to people who don't want to talk",
  "category": "Person",
  "letterCount": 9,
  "basePoints": 200
}

{
  "solution": "OFFICE",
  "definition": "Where you go to pretend you're productive",
  "category": "Place",
  "letterCount": 6,
  "basePoints": 200
}

{
  "solution": "CACTUS",
  "definition": "Green thing that refuses to die",
  "category": "Plant",
  "letterCount": 6,
  "basePoints": 200
}
```

### Hard Examples:
```json
{
  "solution": "TRAFFIC LIGHT",
  "definition": "A silent negotiator between chaos and order",
  "category": "Thing",
  "letterCount": 13,
  "basePoints": 300
}

{
  "solution": "CALENDAR",
  "definition": "A rectangle that holds emotional hostages",
  "category": "Thing",
  "letterCount": 8,
  "basePoints": 300
}

{
  "solution": "ENGINE",
  "definition": "A metal whisperer that turns heat into movement",
  "category": "Thing",
  "letterCount": 6,
  "basePoints": 300
}
```

---

## 🧪 Testing Scenarios

### Test 1: First Puzzle (Easy)
```
1. Open app → Ad plays
2. Easy puzzle loads
3. User guesses wrong → Category hint shown
4. User guesses wrong → Letters revealed
5. User guesses correct → Points awarded
6. Success screen shows stats
```

### Test 2: Multiple Difficulties
```
1. Complete Easy puzzle
2. Switch to Medium → Ad plays
3. Complete Medium puzzle
4. Switch to Hard → Ad plays
5. Complete Hard puzzle
6. All 3 shown as completed today
```

### Test 3: Hint System
```
1. Start puzzle (Level 0 - no hints)
2. Wrong guess → Category hint (Level 1)
3. Wrong guess → Letters revealed (Level 2)
4. Wrong guess → More letters (Level 3)
5. Continue until solved or all letters revealed
```

---

## 📊 Implementation Status

### Phase 1: Project Setup ✅ (90% Complete)
- [x] Project file created
- [x] Core models created
- [ ] DatabaseService (to create)
- [ ] GameService (to create)
- [ ] Service interfaces (to create)

### Phase 2: Services & Logic ⏳ (Next)
- [ ] Complete DatabaseService
- [ ] Complete GameService
- [ ] Hint progression logic
- [ ] Points calculation
- [ ] Puzzle seeding

### Phase 3: ViewModels & UI ⏳ (After Phase 2)
- [ ] GameViewModel
- [ ] ProfileViewModel
- [ ] GamePage.xaml
- [ ] ProfilePage.xaml
- [ ] Navigation setup

### Phase 4: Polish & Testing ⏳ (Final)
- [ ] Resources (colors, styles, strings)
- [ ] Sample puzzle database
- [ ] Testing guide
- [ ] Documentation

---

## 🚀 Next Steps

I'll continue with **Phase 1** completion by creating:
1. DatabaseService.cs
2. GameService.cs  
3. Service interfaces (IAdService, ISubscriptionService, IFeedbackService)
4. Sample puzzle data generator

Then move to **Phase 2, 3, and 4** systematically.

**Estimated total files to create:** ~60 files
**Current progress:** 5/60 files (8%)

Would you like me to continue with the remaining Phase 1 files now?
