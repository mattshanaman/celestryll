# GOOGLE PLAY STORE LISTING

## 📱 App Information

**App Name:** BadlyDefined - Word Puzzle Game  
**Package Name:** com.ambient.badlydefined  
**Category:** Games > Word  
**Content Rating:** Everyone (E)  
**Price:** Free (with optional in-app purchases)

---

## 📝 Store Listing Copy

### Short Description (Max 80 chars):
```
Daily word puzzles with hilariously bad definitions. Can you guess the word?
```
(79 characters)

### Full Description (Max 4000 chars):

```
🎮 BadlyDefined - The Word Puzzle Game That Makes You Think Twice!

Ever tried to guess a word from the WORST possible definition? That's BadlyDefined!

🎯 DAILY CHALLENGES
• New puzzles every day across 3 difficulty levels
• Easy, Medium, and Hard challenges
• Track your streak and compete with yourself!

💡 SMART HINT SYSTEM
• Get hints when you're stuck
• Reveal letters strategically
• Earn hint tokens by completing puzzles

🏆 TRACK YOUR PROGRESS
• Monitor your solving streak
• View detailed statistics
• Watch your points accumulate
• Improve your average attempts

🎨 UNIQUE EXPERIENCE
• Quirky, playful design
• No ads in free version (optional premium upgrade)
• Offline gameplay - no internet required
• Encrypted local storage for your progress

📊 DIFFICULTY LEVELS
⭐ Easy - Perfect for beginners, more letters revealed
⭐⭐ Medium - A good challenge for regular players
⭐⭐⭐ Hard - For word puzzle masters only!

🎓 HOW TO PLAY
1. Read the intentionally bad definition
2. Look at the category hint
3. See the revealed letters (blanks show remaining)
4. Type your guess
5. Use hints if you're stuck
6. Complete all three difficulties each day!

🌟 FEATURES
• 2 years worth of hand-crafted puzzles (2,190 total!)
• Beautiful, playful interface
• Progress tracking and statistics
• Email stats sharing
• Test mode to browse all puzzles
• Encrypted data storage for security

🎪 WHY "BADLY DEFINED"?
Because the best way to learn words is through hilariously terrible definitions! Our puzzles use intentionally confusing, misleading, or just plain silly descriptions that will make you laugh while testing your vocabulary.

💾 OFFLINE FIRST
BadlyDefined works completely offline. Your progress is saved securely on your device with no cloud sync required. Perfect for:
• Commuting without signal
• Airplane mode gaming
• Data-conscious users
• Privacy-focused players

🎁 FREE FEATURES
• All daily puzzles free forever
• Full hint system
• Complete statistics tracking
• No forced ads
• No registration required

📈 PREMIUM UPGRADE (OPTIONAL)
• Support development
• Remove all ads
• Bonus hint tokens
• Exclusive puzzle themes (coming soon)

🎯 PERFECT FOR
• Word game enthusiasts
• Vocabulary builders
• Daily brain training
• Casual puzzle solvers
• Anyone who loves a good challenge!

🔐 YOUR PRIVACY
• No personal data collection
• Encrypted local storage only
• No analytics or tracking
• Optional email for notifications only

Download BadlyDefined now and start solving today's puzzles!

---

Made with ❤️ for word puzzle lovers everywhere.
```
(2,847 characters)

---

## 🖼️ Required Assets Checklist

### App Icon
- [x] Adaptive icon (512×512 PNG) - Generated from appicon.svg
- [x] Legacy icon included
- [x] Foreground and background layers

### Screenshots (REQUIRED)
**Phone Screenshots (Minimum 2, Maximum 8):**
- [ ] Main game screen with puzzle
- [ ] Difficulty selection view
- [ ] Profile/statistics page
- [ ] Puzzle completion celebration
- [ ] Hint system in action
- [ ] Category hint display
- [ ] Timer and streak display
- [ ] Test mode puzzle browser

**Recommended sizes:**
- 1080×2400 (9:16 ratio for modern phones)
- 1920×1080 (landscape optional)

### Feature Graphic (REQUIRED)
- [ ] 1024×500 PNG
- [ ] Shows app branding
- [ ] Includes "BadlyDefined" text
- [ ] Displays game concept visually

### Promotional Assets (OPTIONAL)
- [ ] Promo graphic: 180×120
- [ ] TV banner: 1280×720
- [ ] 360 video (optional)

---

## 📋 Google Play Console Setup Steps

### 1. App Content
- [x] Privacy Policy URL: [MUST ADD YOUR URL]
- [ ] App access (full access, all features unlocked)
- [ ] Ads declaration (No ads currently)
- [ ] Content rating (complete IARC questionnaire)
- [ ] Target audience (Everyone)
- [ ] News app (No)
- [ ] COVID-19 contact tracing/status (No)

### 2. Data Safety
**Data Collected:**
- Email address (optional)
  - Purpose: Account recovery, notifications
  - Optional: Yes
  - Can be deleted: Yes
  
- App activity
  - Purpose: Game progress tracking
  - Stored locally only: Yes
  - Encrypted: Yes

**Data NOT Collected:**
- Location
- Personal info (beyond optional email)
- Financial info
- Photos/videos
- Audio
- Files/documents
- Device ID
- App interactions for advertising

**Data Security:**
- Data is encrypted in transit: N/A (no transmission)
- Data is encrypted at rest: Yes (SQLite encryption)
- Users can request data deletion: Yes (via app uninstall)
- Committed to Google Play Families Policy: Optional

### 3. Store Listing
- [ ] App name: BadlyDefined
- [ ] Short description: (See above)
- [ ] Full description: (See above)
- [ ] Screenshots: Upload 2-8 images
- [ ] Feature graphic: Upload 1024×500 PNG
- [ ] App category: Games > Word
- [ ] Tags: word game, puzzle, daily challenge, vocabulary
- [ ] Contact email: badlydefined@ambient-games.com
- [ ] Website: [OPTIONAL]

### 4. Pricing & Distribution
- [ ] Free app
- [ ] Available countries: All supported countries
- [ ] Contains ads: No (currently)
- [ ] In-app purchases: Yes (planned - premium subscription)
- [ ] Google Play for Education: No
- [ ] Content guidelines: Compliant

---

## 🔑 App Signing

### Development Signing
Currently using debug keystore.

### Release Signing (REQUIRED)
```powershell
# Generate upload keystore (one-time)
keytool -genkey -v -keystore badlydefined-upload.keystore -alias badlydefined -keyalg RSA -keysize 2048 -validity 10000

# Add to csproj or use signing config
```

**Google Play App Signing:**
1. Enroll in Google Play App Signing (recommended)
2. Upload your app signing key
3. Google manages and protects your key
4. You sign with upload key

---

## 🧪 Pre-Launch Checklist

### Before Submitting Internal Test:
- [x] Debug build runs successfully
- [x] All features functional
- [x] No crashes on startup
- [ ] Test on multiple Android versions (8.0+)
- [ ] Test on tablet layout
- [ ] Test rotation/orientation changes
- [ ] Test with Android accessibility features

### Before Submitting to Production:
- [ ] Privacy policy live at public URL
- [ ] All store assets uploaded
- [ ] Content rating completed
- [ ] Data safety form completed
- [ ] Release build tested thoroughly
- [ ] ProGuard rules optimized
- [ ] App size optimized (<150MB recommended)
- [ ] Battery usage tested
- [ ] Memory leaks checked

---

## 📦 Build Release AAB

```powershell
# Clean build
dotnet clean

# Build release AAB for Google Play
dotnet publish BadlyDefined.csproj -f net10.0-android -c Release

# Output location:
# BadlyDefined\bin\Release\net10.0-android\publish\com.ambient.badlydefined-Signed.aab
```

---

## 🚀 Submission Timeline

### Phase 1: Internal Testing (1-2 days)
- Create internal test track
- Upload AAB
- Add test users
- Gather feedback
- Fix critical bugs

### Phase 2: Closed Beta (1-2 weeks)
- Create closed track
- Invite beta testers (20-1000 users)
- Monitor crash reports
- Collect feedback
- Iterate on issues

### Phase 3: Open Beta (Optional, 2-4 weeks)
- Open testing to public
- Larger user base feedback
- Performance monitoring
- Final polish

### Phase 4: Production (Review: 1-7 days)
- Submit for review
- Google reviews for policy compliance
- Address any review feedback
- Go live!

---

## ⚠️ Common Rejection Reasons to Avoid

1. **Missing Privacy Policy** ✅ (Created)
2. **Incorrect target API** ✅ (API 35)
3. **Missing app icon** ✅ (Created)
4. **Incomplete Data Safety form** ⚠️ (Must complete in console)
5. **Copyright violations** ✅ (Original content)
6. **Malware/unwanted software** ✅ (Clean code)
7. **Misleading functionality** ✅ (Accurate description)

---

## 📊 Current Compliance Status

| Requirement | Status | Notes |
|-------------|--------|-------|
| Target API 34+ | ✅ PASS | Using API 35 |
| 64-bit support | ✅ PASS | MAUI default |
| App icon | ✅ FIXED | Created adaptive icon |
| Privacy policy | ⚠️ DRAFT | Must host publicly |
| Store listing | ⚠️ PENDING | Assets needed |
| Content rating | ⚠️ PENDING | IARC form |
| Data safety | ⚠️ PENDING | Console form |
| Permissions | ✅ FIXED | Minimal permissions |
| Release config | ✅ FIXED | ProGuard enabled |
| Encryption | ✅ PASS | SQLite encrypted |

**Blocking Issues:** 1 (Privacy Policy URL)  
**To Complete:** 3 (Store assets, content rating, data safety form)

---

## 🎯 READY FOR:
- ✅ Internal testing track
- ⚠️ Closed beta (needs privacy policy URL)
- ❌ Production (needs all assets + forms)

---

**Next Steps:**
1. Host privacy policy at public URL
2. Create screenshots from running app
3. Design feature graphic
4. Complete Play Console forms
5. Build release AAB
6. Submit to internal test track
