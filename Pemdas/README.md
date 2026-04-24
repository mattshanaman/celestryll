# PEMDAS - Daily Math Challenge Game

A cross-platform mobile math puzzle game built with .NET MAUI that challenges users with daily equation puzzles.

## Features

### Game Modes
- **Solve It Mode**: Fill in missing numbers or operators to make equations true
- **Build It Mode**: Use given digits and operators to reach a target number

### Difficulty Levels
- Easy
- Medium
- Hard
- Creative
- Tricky
- Speed (with time limits)
- Boss (advanced operators)

### User Engagement
- **Daily Challenges**: New puzzle every day based on UTC time
- **Streak System**: Earn hint tokens every 3 days of consecutive play
- **Points & Leaderboards**: Track your progress and compete
- **Hint System**: Use earned tokens or watch ads for hints
- **Share Results**: Share your daily results with friends

### Monetization
- **Ad Integration**: Interstitial and rewarded ads (ready for AdMob)
- **Premium Subscription** ($2.99/month):
  - Access to puzzle archives
  - Unlimited practice mode
  - Advanced operators (exponents, roots)
  - Ad-free experience

## Technical Details

### Database
- **SQLite**: Self-contained database with 10 years (3,650 days) of pre-generated puzzles
- **Tables**: 
  - DailyPuzzle
  - UserProgress
  - PuzzleAttempt

### Architecture
- **MVVM Pattern**: Using CommunityToolkit.Mvvm
- **Dependency Injection**: All services registered in MauiProgram
- **Cross-Platform**: Supports Android, iOS, macOS, and Windows

### Technologies Used
- .NET 10
- .NET MAUI
- SQLite (sqlite-net-pcl)
- NCalc (expression evaluation)
- CommunityToolkit.Maui
- CommunityToolkit.Mvvm

## Project Structure

## License

This project is created as a sample/educational application. Please add your own license as needed.

## Support

For questions or issues, please create an issue in the repository.

## Getting Started

### Prerequisites
- Visual Studio 2022 (17.8 or later)
- .NET 10 SDK
- Android SDK (for Android development)
- Xcode (for iOS/macOS development)

### Build and Run
1. Clone the repository
2. Open `Pemdas.sln` in Visual Studio
3. Restore NuGet packages
4. Select your target platform (Android/iOS/Windows)
5. Press F5 to build and run

### Required NuGet Packages
The following packages are already configured in the project file:
- Microsoft.Maui.Controls
- sqlite-net-pcl (1.9.172)
- SQLitePCLRaw.bundle_green (2.1.10)
- NCalc (1.0.0)
- CommunityToolkit.Mvvm (8.3.2)
- CommunityToolkit.Maui (10.0.0)

## Future Enhancements

### Ad Integration
To enable ads, you'll need to:
1. Add AdMob packages:
   - Android: `Xamarin.Google.Android.Play.Services.Ads`
   - iOS: `Xamarin.Google.iOS.MobileAds`
2. Create AdMob account and get Ad Unit IDs
3. Implement platform-specific ad services
4. Update AndroidManifest.xml and Info.plist with AdMob App IDs

### In-App Purchases
To enable subscriptions:
1. Add `Plugin.InAppBilling` NuGet package
2. Implement subscription purchase flow
3. Set up products in Google Play Console and App Store Connect
4. Update `SubscriptionService.cs` with actual purchase logic

### Additional Features
- Leaderboards (global and friends)
- Puzzle archive page
- Practice mode with custom difficulty
- Social features (friend challenges)
- Achievements system
- Dark mode support
- Localization (multiple languages)

## Database Generation

The app automatically generates 10 years of puzzles on first launch:
- **3,650 unique puzzles** (one per day)
- **Alternating modes**: Solve It and Build It
- **Weekly difficulty progression**: Easy → Speed → Boss
- **Deterministic generation**: Same seed produces same puzzle

## Puzzle Examples

### Day 1 - Solve It (Easy)
```
Puzzle: (? + 3) × 4 = 28
Answer: ? = 4
Points: 100
```

### Day 2 - Build It (Medium)
```
Target: 10
Available Digits: 1, 2, 3, 4
Answer: (1 + 3) × 2 + 4 = 10
Points: 200
```

### Day 7 - Solve It (Boss)
```
Puzzle: (X² + 4) ÷ (Y - 1) = 4
Answer: X = 2, Y = 3
Points: 500
Note: X and Y must be different integers
```

---

## 📖 Game Rules

### **Critical Constraints:**

1. **Integer Solutions Only** 🔢
   - All answers must be whole numbers (no fractions or decimals)
   - Example: ✅ 5 (correct) | ❌ 2.5 (incorrect)

2. **Different Variable Values** ⚠️
   - For puzzles with multiple variables (A, B, C or X, Y)
   - **All variables MUST have different values**
   - Example: ✅ A=3, B=2, C=6 (all different) | ❌ A=5, B=5, C=3 (A and B same)

3. **Use All Digits** (Build It Mode) 🔨
   - Must use every available digit exactly once
   - Example: Given 1,2,3,4 → ✅ `1+2+3+4=10` | ❌ `1+2+3=6` (missing 4)

**📚 For complete rules and examples, see [GAME_RULES_AND_INSTRUCTIONS.md](GAME_RULES_AND_INSTRUCTIONS.md)**

## 🔮 Future Enhancements

- [ ] Global leaderboards (Firebase/Azure)
- [ ] Puzzle archive page UI
- [ ] Practice mode with custom difficulty
- [ ] Social features (friend challenges)
- [ ] Achievements system with badges
- [ ] Dark mode support
- [ ] Additional language translations
- [ ] Tutorial/onboarding flow
- [ ] Sound effects and haptic feedback
- [ ] Landscape orientation support
- [ ] Tablet-optimized layouts
- [ ] Apple Watch companion app

## 🐛 Known Issues / Limitations

1. **Ads require test mode** - Replace with real AdMob IDs for production
2. **Subscription is simulated** - Integrate real IAP for production
3. **No cloud sync** - User progress is local only
4. **Archive page pending** - Navigate to archive shows placeholder

## 📄 License

This project is created as a sample/educational application. 

**For Production Use:**
- Replace test AdMob IDs with your own
- Implement real in-app purchase flow
- Add proper error handling and analytics
- Test thoroughly on all target platforms

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:
1. Fork the repository
2. Create a feature branch
3. Make your changes with clear commit messages
4. Test on at least one platform
5. Submit a pull request

## 📞 Support

For questions, issues, or feature requests:
- Create an **Issue** in the repository
- Check existing issues first
- Provide detailed reproduction steps for bugs

---

## 🎮 Play Instructions

1. **Launch the app** - See today's daily puzzle
2. **Solve It Mode**: Enter the missing number(s) in the blanks
3. **Build It Mode**: Use the calculator to build an equation
4. **Submit** your answer to check if it's correct
5. **Earn points** and maintain your streak
6. **Get hints** using tokens or watching ads
7. **Share** your results with friends
8. **Come back daily** for new challenges!

---

**Happy Puzzling! 🎮📐✨**

Made with ❤️ using .NET MAUI