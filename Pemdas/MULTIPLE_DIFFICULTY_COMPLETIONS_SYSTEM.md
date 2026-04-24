# ?? Multiple Difficulty Completions Per Day - Implementation Guide

## Overview

Comprehensive system allowing users to complete multiple difficulty levels per day, with ad requirements, time/points tracking, and shareable puzzle IDs.

---

## ? PHASE 1: Database & Models - COMPLETE

### New Models Added:

#### `DailyCompletion` Table
```csharp
- CompletionDate: Which day completed
- DifficultySlot: Which difficulty (0-7)
- AdWatched: Whether ad was required
- CompletionTime: Seconds to complete
- PointsEarned: Points awarded
- PuzzleIdentifier: Shareable ID (e.g., "P20240115-E")
- DateSlotKey: Index for fast lookup
```

#### `DailyPuzzle` Enhanced
```csharp
- PuzzleIdentifier: Unique ID for sharing (e.g., "P20240115-E")
  Format: P + YYYYMMDD + Difficulty Initial
  Examples:
    - P20240115-E = Easy puzzle on Jan 15, 2024
    - P20240115-H = Hard puzzle on Jan 15, 2024
    - P20240115-X = Expert puzzle on Jan 15, 2024
```

### Database Methods Added:

```csharp
// Check if difficulty completed today
await _databaseService.HasCompletedDifficultyToday(difficultySlot);

// Get list of completed difficulties today
var completed = await _databaseService.GetTodaysCompletedDifficulties();

// Record a completion
await _databaseService.RecordDailyCompletion(
    difficultySlot, 
    puzzleId, 
    completionTimeSeconds, 
    pointsEarned, 
    adWatched
);

// Get completion stats
var stats = await _databaseService.GetCompletionStats(difficultySlot, date);

// Get total points across all completions
var totalPoints = await _databaseService.GetTotalPointsEarned();

// Get recent completions for stats/leaderboard
var recent = await _databaseService.GetRecentCompletions(days: 7);
```

---

## ?? PHASE 2: GameViewModel Changes (TODO)

### Add Properties:

```csharp
// Time tracking
private DateTime _puzzleStartTime;
private int _completionTimeSeconds;

public string CompletionTimeDisplay => 
    _completionTimeSeconds > 0 
        ? $"{_completionTimeSeconds / 60}:{_completionTimeSeconds % 60:D2}"
        : "0:00";

// Puzzle identification
public string CurrentPuzzleId { get; set; }

// Completion tracking
public ObservableCollection<int> CompletedDifficulties { get; set; }
public bool HasCompletedToday { get; set; }
public int TotalPointsEarned { get; set; }

// Ad requirements
public bool RequiresAd { get; set; }
```

### Initialize On Load:

```csharp
private async Task InitializeGame()
{
    // Start timer
    _puzzleStartTime = DateTime.UtcNow;
    
    // Set puzzle ID
    CurrentPuzzleId = _currentPuzzle.PuzzleIdentifier;
    
    // Check completed difficulties
    CompletedDifficulties = new ObservableCollection<int>(
        await _databaseService.GetTodaysCompletedDifficulties()
    );
    
    // Get total points
    TotalPointsEarned = await _databaseService.GetTotalPointsEarned();
    
    // Check if already completed this difficulty
    HasCompletedToday = CompletedDifficulties.Contains(_currentPuzzle.DifficultySlot);
    
    // Check if ad required (if not first completion and not subscribed)
    var isSubscribed = await _subscriptionService.CheckSubscriptionStatus();
    RequiresAd = CompletedDifficulties.Count > 0 && !isSubscribed;
}
```

### On Submission Success:

```csharp
private async Task HandleCorrectAnswer(int pointsEarned)
{
    // Calculate completion time
    var elapsed = DateTime.UtcNow - _puzzleStartTime;
    _completionTimeSeconds = (int)elapsed.TotalSeconds;
    
    // Check if ad was watched (if required)
    bool adWatched = RequiresAd; // Set by ad viewing logic
    
    // Record completion
    await _databaseService.RecordDailyCompletion(
        _currentPuzzle.DifficultySlot,
        _currentPuzzle.PuzzleIdentifier,
        _completionTimeSeconds,
        pointsEarned,
        adWatched
    );
    
    // Update completed list
    CompletedDifficulties.Add(_currentPuzzle.DifficultySlot);
    
    // Update total points
    TotalPointsEarned = await _databaseService.GetTotalPointsEarned();
    
    // Show success message with stats
    FeedbackMessage = $"Correct! +{pointsEarned} points! Time: {CompletionTimeDisplay}";
}
```

### Enhanced Sharing:

```csharp
private async Task ShareResult()
{
    var stats = await _databaseService.GetCompletionStats(
        _currentPuzzle.DifficultySlot, 
        DateTime.UtcNow.Date
    );
    
    var shareText = $@"I solved PEMDAS puzzle {stats.PuzzleIdentifier}!
Difficulty: {_currentPuzzle.Difficulty}
Time: {stats.CompletionTime / 60}:{stats.CompletionTime % 60:D2}
Points: {stats.PointsEarned}
Total Points: {TotalPointsEarned}

Can you beat my time? Download PEMDAS now!";

    await Share.RequestAsync(new ShareTextRequest
    {
        Title = "Share PEMDAS Result",
        Text = shareText
    });
}
```

---

## ?? PHASE 3: UI Changes (TODO)

### Update "Already Completed" Message:

**Current (GamePage.xaml):**
```xaml
<Frame IsVisible="{Binding HasCompletedToday}">
    <Label Text="You've already completed today's puzzle!" />
</Frame>
```

**New:**
```xaml
<Frame IsVisible="{Binding HasCompletedToday}" 
       BackgroundColor="{StaticResource Info}">
    <VerticalStackLayout Spacing="8" Padding="12">
        <Label Text="? You've completed this difficulty today!" 
               FontSize="16" 
               FontAttributes="Bold"
               HorizontalTextAlignment="Center"/>
        
        <Label Text="{Binding CompletionMessage}" 
               FontSize="14"
               HorizontalTextAlignment="Center"
               LineBreakMode="WordWrap"/>
        
        <!-- Show which difficulties are completed -->
        <HorizontalStackLayout HorizontalOptions="Center" Spacing="4">
            <Label Text="Completed today:" FontSize="12"/>
            <Label Text="{Binding CompletedDifficultiesDisplay}" 
                   FontSize="12" 
                   FontAttributes="Bold"/>
        </HorizontalStackLayout>
        
        <!-- Encourage trying other difficulties -->
        <Label Text="Try another difficulty level below!" 
               FontSize="14"
               FontAttributes="Italic"
               HorizontalTextAlignment="Center"/>
        
        <!-- Show total points -->
        <HorizontalStackLayout HorizontalOptions="Center" Spacing="6">
            <Label Text="?? Total Points:" FontSize="14"/>
            <Label Text="{Binding TotalPointsEarned}" 
                   FontSize="16" 
                   FontAttributes="Bold"
                   TextColor="{StaticResource Primary}"/>
        </HorizontalStackLayout>
    </VerticalStackLayout>
</Frame>
```

### Add Puzzle ID Display:

```xaml
<!-- Add below puzzle display -->
<HorizontalStackLayout HorizontalOptions="Center" Spacing="6">
    <Label Text="Puzzle ID:" 
           FontSize="11"
           TextColor="{StaticResource Gray600}"/>
    <Label Text="{Binding CurrentPuzzleId}" 
           FontSize="11"
           FontAttributes="Bold"
           TextColor="{StaticResource Primary}"/>
</HorizontalStackLayout>
```

### Add Time Display:

```xaml
<!-- In header stats grid, add fourth column -->
<Frame Grid.Column="3" 
       BackgroundColor="{StaticResource Info}" 
       Padding="8" 
       CornerRadius="8">
    <VerticalStackLayout Spacing="2">
        <Label Text="??" FontSize="18" HorizontalOptions="Center"/>
        <Label Text="{Binding ElapsedTime}" 
               FontSize="12" 
               FontAttributes="Bold" 
               HorizontalOptions="Center" 
               TextColor="White"/>
        <Label Text="Time" 
               FontSize="10" 
               HorizontalOptions="Center" 
               TextColor="White"/>
    </VerticalStackLayout>
</Frame>
```

### Enhanced Success Message:

```xaml
<Frame IsVisible="{Binding PuzzleCompleted}" 
       BackgroundColor="{StaticResource Success}">
    <VerticalStackLayout Spacing="8">
        <Label Text="{Binding FeedbackMessage}" 
               FontSize="16"
               FontAttributes="Bold"
               HorizontalTextAlignment="Center"/>
        
        <!-- Show completion stats -->
        <Grid ColumnDefinitions="*,*" ColumnSpacing="12">
            <VerticalStackLayout Grid.Column="0">
                <Label Text="Time" FontSize="11" HorizontalTextAlignment="Center"/>
                <Label Text="{Binding CompletionTimeDisplay}" 
                       FontSize="18" 
                       FontAttributes="Bold"
                       HorizontalTextAlignment="Center"/>
            </VerticalStackLayout>
            
            <VerticalStackLayout Grid.Column="1">
                <Label Text="Puzzle ID" FontSize="11" HorizontalTextAlignment="Center"/>
                <Label Text="{Binding CurrentPuzzleId}" 
                       FontSize="14" 
                       FontAttributes="Bold"
                       HorizontalTextAlignment="Center"/>
            </VerticalStackLayout>
        </Grid>
        
        <!-- Share button with enhanced stats -->
        <Button Text="?? Share My Time" 
                Command="{Binding ShareResultCommand}"
                BackgroundColor="White"
                TextColor="{StaticResource Success}"/>
    </VerticalStackLayout>
</Frame>
```

---

## ?? PHASE 4: Ad Integration (TODO)

### When Selecting Additional Difficulty:

```csharp
[RelayCommand]
private async Task SelectDifficulty(int difficultySlot)
{
    // Check if already completed
    var completedToday = await _databaseService.GetTodaysCompletedDifficulties();
    
    // If not first completion, require ad (unless subscribed)
    if (completedToday.Count > 0)
    {
        var isSubscribed = await _subscriptionService.CheckSubscriptionStatus();
        
        if (!isSubscribed)
        {
            // Show ad first
            var adResult = await _adService.ShowInterstitialAdAsync();
            
            if (!adResult)
            {
                // Ad failed or user closed
                ErrorMessage = "Please watch the ad to play additional puzzles today.";
                HasError = true;
                return;
            }
            
            RequiresAd = true;
        }
    }
    
    // Load the selected difficulty
    await LoadPuzzleForDifficulty(difficultySlot);
}
```

### IAdService Enhancement:

```csharp
public interface IAdService
{
    // Existing
    void ShowInterstitialAd();
    
    // NEW: Async version that returns success/failure
    Task<bool> ShowInterstitialAdAsync();
}
```

**Implementation:**
```csharp
public async Task<bool> ShowInterstitialAdAsync()
{
    var tcs = new TaskCompletionSource<bool>();
    
    // Show ad and wait for result
    try
    {
        ShowInterstitialAd();
        
        // Wait a moment for ad to complete
        await Task.Delay(2000);
        
        // Assume success if no exception
        tcs.SetResult(true);
    }
    catch
    {
        tcs.SetResult(false);
    }
    
    return await tcs.Task;
}
```

---

## ?? PHASE 5: Stats & Leaderboard (TODO)

### Add to ProfilePage.xaml:

```xaml
<VerticalStackLayout Spacing="12">
    <!-- Total Points -->
    <Frame BackgroundColor="{StaticResource Primary}">
        <HorizontalStackLayout Spacing="12">
            <Label Text="??" FontSize="32"/>
            <VerticalStackLayout>
                <Label Text="Total Points" FontSize="14"/>
                <Label Text="{Binding TotalPoints}" 
                       FontSize="28" 
                       FontAttributes="Bold"/>
            </VerticalStackLayout>
        </HorizontalStackLayout>
    </Frame>
    
    <!-- Today's Completions -->
    <Frame>
        <VerticalStackLayout Spacing="8">
            <Label Text="Today's Completions" 
                   FontSize="16" 
                   FontAttributes="Bold"/>
            <CollectionView ItemsSource="{Binding TodaysCompletions}">
                <CollectionView.ItemTemplate>
                    <DataTemplate>
                        <HorizontalStackLayout Spacing="12">
                            <Label Text="{Binding DifficultyIcon}" FontSize="20"/>
                            <Label Text="{Binding DifficultyName}" FontSize="14"/>
                            <Label Text="{Binding CompletionTimeDisplay}" 
                                   FontSize="14" 
                                   FontAttributes="Bold"/>
                            <Label Text="{Binding PointsDisplay}" 
                                   FontSize="14"
                                   TextColor="{StaticResource Primary}"/>
                        </HorizontalStackLayout>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </VerticalStackLayout>
    </Frame>
    
    <!-- Recent Best Times -->
    <Frame>
        <VerticalStackLayout Spacing="8">
            <Label Text="?? Recent Best Times" 
                   FontSize="16" 
                   FontAttributes="Bold"/>
            <CollectionView ItemsSource="{Binding RecentBestTimes}">
                <CollectionView.ItemTemplate>
                    <DataTemplate>
                        <Grid ColumnDefinitions="Auto,*,Auto">
                            <Label Grid.Column="0" 
                                   Text="{Binding PuzzleId}" 
                                   FontSize="12"/>
                            <Label Grid.Column="1" 
                                   Text="{Binding DifficultyName}" 
                                   FontSize="12"/>
                            <Label Grid.Column="2" 
                                   Text="{Binding TimeDisplay}" 
                                   FontSize="12" 
                                   FontAttributes="Bold"/>
                        </Grid>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </VerticalStackLayout>
    </Frame>
</VerticalStackLayout>
```

---

## ?? PHASE 6: Implementation Summary

### What's Been Completed:
? Database models updated (DailyCompletion table)
? Puzzle identifier generation (P20240115-E format)
? Database methods for tracking completions
? Methods to get completion stats
? Methods to calculate total points
? Recent completions retrieval

### What's Next (In Order):

1. **GameViewModel Updates** (1-2 hours)
   - Add time tracking
   - Add puzzle ID display
   - Update completion logic
   - Add ad requirement checking

2. **UI Updates** (1-2 hours)
   - Update "already completed" message
   - Add puzzle ID display
   - Add elapsed time display
   - Enhanced success screen

3. **Ad Integration** (30 min)
   - Add async ad method
   - Integrate with difficulty selection
   - Handle ad failures

4. **Testing** (1 hour)
   - Test multiple completions per day
   - Test ad requirements
   - Test time tracking
   - Test sharing with puzzle IDs

5. **Stats Page** (Optional, 1-2 hours)
   - Add stats display to ProfilePage
   - Show recent completions
   - Show best times

---

## ?? Key Features Summary

### For Users:
- ? Complete all 8 difficulties each day
- ? Track completion time for each puzzle
- ? Earn and track total points
- ? Share specific puzzle IDs (e.g., "P20240115-E")
- ? See which difficulties completed today
- ? Ad-free subscribers can play unlimited

### For Non-Subscribers:
- ? First puzzle of the day: FREE
- ? Additional puzzles: Watch ad first
- ? Tracked separately per difficulty

### Sharing Example:
```
I solved PEMDAS puzzle P20240115-H!
Difficulty: Hard
Time: 1:23
Points: 300
Total Points: 15,250

Can you beat my time? Download PEMDAS now!
```

---

## ?? User Flow Example

**Morning:**
1. User opens app
2. Sees Easy (?) highlighted (default)
3. Solves Easy puzzle ? Gets 100 points
4. Timer stops at 0:45 ? Recorded
5. Message: "Completed! Try another difficulty!"

**Later:**
6. User selects Hard (???)
7. If NOT subscribed: Ad appears
8. After ad: Hard puzzle loads
9. Solves Hard puzzle ? Gets 300 points
10. Total points now: 400

**Evening:**
11. Selects Boss (??)
12. Another ad appears (if not subscribed)
13. Solves Boss ? Gets 500 points
14. Total: 900 points today

**Sharing:**
15. Clicks "Share My Time"
16. Posts: "I solved P20240115-B in 2:15! 500 points!"

---

## ?? Ready to Implement?

All database infrastructure is in place. Ready to proceed with:
1. GameViewModel updates
2. UI changes
3. Ad integration

Estimated total time: 4-6 hours for full implementation.
