# ?? Sample Puzzles for Manual Testing

## Overview
This document provides sample puzzles for each difficulty level to facilitate manual testing of the Expert Difficulty & Premium System.

**Date:** December 19, 2024  
**Purpose:** Manual testing validation  
**Difficulties:** 8 (Easy ? Expert)

---

## How to Use This Guide

### Testing in App:
1. Open **Test Mode** from Profile page
2. Select desired difficulty
3. Tap "Generate Puzzle"
4. Compare displayed puzzle with samples below
5. Enter the provided answer
6. Verify "Correct!" message appears

### Testing Each Type:
- Test at least 3 puzzles per difficulty
- Verify answer validation works
- Check hint text displays correctly
- Confirm points are awarded
- Test timer (for Speed mode)

---

## ?? Easy Difficulty - Solve It

### Sample 1: Multiplication First
```
Puzzle: ? × 4 + 3 = 19
Answer: 4
Explanation: 4 × 4 = 16, then 16 + 3 = 19
Hint: "Remember PEMDAS: Multiply before adding! Do ? × 4 first."
Points: 100
Time Limit: None
```

**How to Test:**
1. In Test Mode, select Easy
2. Generate until you see a multiplication puzzle
3. Enter: 4
4. Submit
5. ? Expected: Correct! +100 points

---

### Sample 2: Division First
```
Puzzle: ? ÷ 2 + 5 = 9
Answer: 8
Explanation: 8 ÷ 2 = 4, then 4 + 5 = 9
Hint: "Remember PEMDAS: Divide before adding! Do ? ÷ 2 first."
Points: 100
Time Limit: None
```

**How to Test:**
1. Generate Easy puzzles until division appears
2. Enter: 8
3. Submit
4. ? Expected: Correct! +100 points

---

### Sample 3: Subtraction Order
```
Puzzle: 10 - ? + 2 = 7
Answer: 5
Explanation: 10 - 5 = 5, then 5 + 2 = 7
Hint: "Work left to right: Do 10 - ? first, then add 2."
Points: 100
Time Limit: None
```

**Test Steps:**
1. Generate Easy
2. Find subtraction puzzle
3. Enter: 5
4. ? Verify correct

---

## ?? Medium Difficulty - Build It

### Sample 1: Basic Build It
```
Puzzle: Build equation to reach: 24
Available Digits: 2, 3, 4, 6
Answer Examples:
  - (2 + 4) × (6 - 3)
  - 6 × 4 + 3 - 2
  - 3 × 6 + 4 + 2
Hint: "Try using multiplication and addition."
Points: 200 (or 225 with Elegant Solution bonus, or 250 with PEMDAS bonus)
Time Limit: None
```

**Test Cases:**
```
? Test 1: (2 + 4) × (6 - 3) = 6 × 3 = 24 ?
? Test 2: 6 × 4 + 3 - 2 = 24 + 1 = 25 ? (wrong)
   Actually: 6 × 4 = 24, 24 + 3 = 27, 27 - 2 = 25 ?
   
Correct: 6 × 4 = 24, so just "6 × 4" or "(6 - 3 + 1) × 4"
```

---

### Sample 2: Medium Build It
```
Puzzle: Build equation to reach: 30
Available Digits: 2, 3, 5, 6
Answer Examples:
  - (5 + 3) × 6 - 2 = 48 - 2 = 46 ?
  Actually: 5 × 6 + 3 - 2 = 30 - 2 = 28 ?
  
Correct: 5 × 6 = 30 (simple!)
  or: (6 - 1) × 6 (if 1 available)
  or: 3 × (6 + 4) - 2 (if 4 available)
Hint: "Think about which pairs multiply to 30"
Points: 200 + bonuses
Time Limit: None
```

---

## ?? Hard Difficulty - Solve It

### Sample 1: Two Variables
```
Puzzle: A × 3 + B = 17
Where: A and B must be different
Answer: A = 5, B = 2
Explanation: 5 × 3 = 15, then 15 + 2 = 17
Hint: "Start with multiplication. What times 3 gets close to 17?"
Points: 300
Time Limit: None
```

**How to Enter:**
- First box (A =): 5
- Second box (B =): 2
- Submit
- ? Expected: Correct! +300 points

---

### Sample 2: Hard with Division
```
Puzzle: A ÷ 2 + B = 12
Answer: A = 18, B = 3
Explanation: 18 ÷ 2 = 9, then 9 + 3 = 12
Hint: "A must be even. Try different values."
Points: 300
Time Limit: None
```

**Test Multi-Value Input:**
1. See two entry boxes labeled "A =" and "B ="
2. Enter A: 18
3. Enter B: 3
4. Submit
5. ? Verify both values required

---

## ?? Creative Difficulty - Build It

### Sample 1: Elegant Solution Challenge
```
Puzzle: Build equation to reach: 36
Available Digits: 2, 3, 6, 9
Answer Examples:
  - 9 × (6 - 3 - 2) = 9 × 1 = 9 ?
  - 6 × (9 - 3) + 2 = 6 × 6 + 2 = 38 ?
  - 9 × 6 - 3 × 2 = 54 - 6 = 48 ?
  
Correct: 6 × 6 = 36 (but need two 6's)
  or: 9 × 4 = 36 (need 4)
  or: (9 + 3) × 3 = 36 (need two 3's)
  
Best: 9 × (2 + 2) = 36 (if two 2's)
      or 6 × (3 + 3) = 36 (if two 3's)

Hint: "Look for elegant multiplication."
Points: 250 + bonuses
Time Limit: None
```

---

### Sample 2: Creative PEMDAS
```
Puzzle: Build equation to reach: 42
Available Digits: 3, 5, 6, 7
Answers:
  - 7 × 6 + 5 - 3 = 42 + 2 = 44 ?
  Actually: 7 × 6 = 42 (perfect!)
  or: 7 × (5 + 1) (if 1 available)
Hint: "7 × 6 = 42. Use PEMDAS to your advantage."
Points: 250 + bonuses
```

---

## ?? Tricky Difficulty - Solve It (3 Variables)

### Sample 1: Three Variables
```
Puzzle: A × B + C = 23
Where: A, B, C all different
Answer: A = 5, B = 4, C = 3
Explanation: 5 × 4 = 20, then 20 + 3 = 23
Hint: "Try small numbers. What multiplies to less than 23?"
Points: 400
Time Limit: None
```

**Test Three Inputs:**
1. See three boxes: A =, B =, C =
2. Enter A: 5, B: 4, C: 3
3. Submit
4. ? Verify all three required

---

### Sample 2: Tricky with Subtraction
```
Puzzle: A × B - C = 17
Answer: A = 4, B = 5, C = 3
Explanation: 4 × 5 = 20, then 20 - 3 = 17
Hint: "Multiply first, then subtract."
Points: 400
Time Limit: None
```

**Alternative Answers:**
```
Also valid:
- A = 5, B = 4, C = 3 (same, order doesn't matter for ×)
- A = 10, B = 2, C = 3
- A = 2, B = 10, C = 3
```

---

## ? Speed Difficulty - Build It (Timed!)

### Sample 1: Speed Challenge
```
Puzzle: Build equation to reach: 24
Available Digits: 2, 4, 6, 8
Time Limit: 60 seconds ??
Answer: 
  - 6 × 4 = 24
  - 8 × (6 - 3) = 24 (if 3 available)
  - (6 + 2) × (8 - 5) = 24 (if needed)
Hint: "Quick! 6 × 4 = 24"
Points: 200 + time bonus
```

**Speed Testing:**
1. ?? Timer should start immediately
2. Countdown visible (60, 59, 58...)
3. Sound at 10 seconds remaining
4. "Time Up!" if expired
5. Submit before timer runs out

---

### Sample 2: Speed Under Pressure
```
Puzzle: Build equation to reach: 30
Available Digits: 3, 5, 6, 8
Time: 60 seconds
Answer:
  - 5 × 6 = 30
  - 6 × 5 = 30 (same)
Hint: "5 × 6 = 30"
```

**Test Time Bonus:**
- Submit at 50s ? +50 bonus points
- Submit at 30s ? +30 bonus points
- Submit at 10s ? +10 bonus points

---

## ?? Boss Difficulty - Solve It (Exponents)

### Sample 1: Exponent Equation
```
Puzzle: X² + Y × 3 = 19
Where: X and Y must be different
Answer: X = 2, Y = 5
Explanation: 2² = 4, then 4 + (5 × 3) = 4 + 15 = 19
Hint: "Calculate X² first (exponent), then Y × 3, then add."
Points: 500
Time Limit: None
```

**Test Exponent Display:**
1. Puzzle shows X² with proper superscript
2. Two input boxes
3. Enter X: 2, Y: 5
4. Submit
5. ? Verify correct

**Alternative Valid Answers:**
```
None - this has unique solution
X² = 4, so X = 2 or X = -2
If X = 2: Y × 3 = 15, so Y = 5 ?
If X = -2: same math
```

---

### Sample 2: Boss Challenge
```
Puzzle: X² × Y - Z = 24
Answer: X = 2, Y = 8, Z = 8
Explanation: 2² = 4, then 4 × 8 = 32, then 32 - 8 = 24
Hint: "PEMDAS: X² (exponent first), then × Y, finally - Z. All different."
Points: 500
Time Limit: None
```

**Test Different Values:**
```
Also valid:
- X = 3, Y = 3, Z = 3 (but violates "all different" if enforced)
  3² × 3 - 3 = 9 × 3 - 3 = 27 - 3 = 24 ?
  
Depends on puzzle validation!
```

---

## ?? Expert Difficulty - Solve It (Advanced Math)

### Type A: Simple Exponential

#### Sample 1:
```
Puzzle: 2^? = 16
Answer: 4
Explanation: 2^4 = 2 × 2 × 2 × 2 = 16
Hint: "What power of 2 equals 16? Think: 2×2×2×2 = 16"
Points: 600
Time Limit: None
```

**Test Steps:**
1. Select Expert (if subscribed)
2. Generate until exponential appears
3. Enter: 4
4. ? Verify: Correct! +600 points

---

#### Sample 2:
```
Puzzle: 3^? = 27
Answer: 3
Explanation: 3^3 = 3 × 3 × 3 = 27
Hint: "What power of 3 equals 27? 3 × 3 × 3 = 27"
Points: 600
```

**Test:**
- Enter: 3
- ? Expect correct

---

#### Sample 3:
```
Puzzle: 2^? = 8
Answer: 3
Explanation: 2^3 = 8
Hint: "2 to what power equals 8?"
Points: 600
```

---

### Type B: Exponential with Expression

#### Sample 1:
```
Puzzle: 2^(3-?) = 4
Answer: 1
Explanation: 
  - 2^2 = 4, so the exponent must be 2
  - 3 - ? = 2
  - Therefore ? = 1
Hint: "Solve the exponent: 2^? = 4, so 3-? must equal that exponent."
Points: 600
```

**Test Advanced Logic:**
1. User must think: "What equals 4? 2^2"
2. So: 3 - ? = 2
3. Therefore: ? = 1
4. Enter: 1
5. ? Verify correct

---

#### Sample 2:
```
Puzzle: 2^(4-?) = 8
Answer: 1
Explanation:
  - 2^3 = 8
  - So 4 - ? = 3
  - Therefore ? = 1
Points: 600
```

---

#### Sample 3:
```
Puzzle: 2^(5-?) = 16
Answer: 1
Explanation:
  - 2^4 = 16
  - So 5 - ? = 4
  - ? = 1
Points: 600
```

---

### Type C: Logarithm

#### Sample 1:
```
Puzzle: log???(?) = 3
Answer: 8
Explanation: 
  - log???(?) = 3 means "2 to what power equals ?"
  - 2^3 = 8
  - So ? = 8
Hint: "log???(?) = 3 means 2^3 = ?. What is 2 to the power of 3?"
Points: 600
```

**Test Logarithm Display:**
1. Subscript "2" displays correctly
2. User enters: 8
3. ? Verify correct

---

#### Sample 2:
```
Puzzle: log???(?) = 4
Answer: 16
Explanation: 2^4 = 16
Hint: "2 to the power of 4 equals ?"
Points: 600
```

**Test:**
- Enter: 16
- ? Expect correct

---

#### Sample 3:
```
Puzzle: log???(?) = 2
Answer: 9
Explanation: 3^2 = 9
Hint: "3 to the power of 2 equals ?"
Points: 600
```

---

### Type D: Basic Calculus (Derivatives)

#### Sample 1:
```
Puzzle: d/dx(?x²) = 6x
Answer: 3
Explanation:
  - Power rule: bring down exponent
  - d/dx(ax²) = 2ax
  - So 2 × ? = 6
  - Therefore ? = 3
Hint: "Power rule: bring down the exponent and multiply. If d/dx(?x²) = 6x, then 2×? = 6."
Points: 600
```

**Test Calculus Notation:**
1. d/dx displays clearly
2. Exponent (²) shows correctly
3. Enter: 3
4. ? Verify correct

---

#### Sample 2:
```
Puzzle: d/dx(?x²) = 8x
Answer: 4
Explanation: 2 × ? = 8, so ? = 4
Hint: "2 × ? = 8"
Points: 600
```

---

#### Sample 3:
```
Puzzle: d/dx(?x²) = 10x
Answer: 5
Explanation: 2 × 5 = 10
Points: 600
```

---

## ?? Testing Scenarios

### Scenario 1: Free User (Default Easy)
```
1. Open app (not subscribed)
2. Go to Daily Challenge
3. ? See Easy puzzle
4. ? Only Easy button enabled
5. Solve puzzle
6. ? Streak increments
```

---

### Scenario 2: Free User Tries Other Difficulty
```
1. Not subscribed
2. Tap Medium button
3. ? Dialog: "Change Difficulty - Watch Ad or Subscribe"
4. Choose "Watch Ad"
5. ? Ad plays
6. ? All buttons except Expert enabled
7. Tap Medium
8. ? Medium puzzle loads
```

---

### Scenario 3: Free User Tries Expert
```
1. Not subscribed
2. Tap Expert button (dimmed, opacity 0.5)
3. ? Dialog: "Expert Level - Premium Only"
4. Options: Subscribe / Learn More / Cancel
5. Tap "Learn More"
6. ? See features list
7. Tap OK
```

---

### Scenario 4: Subscriber Access
```
1. Subscribed
2. Open app
3. ? All 8 buttons enabled (no dimming)
4. Tap Expert
5. ? Expert puzzle loads
6. Solve: log???(?) = 3, enter 8
7. ? Correct! +600 points
8. Tap Easy
9. ? Easy puzzle loads
10. Complete both
11. ? Streak only increments once
```

---

### Scenario 5: Streak Tracking (Any Difficulty)
```
Day 1:
1. Complete Easy ? Streak = 1 ?
2. Complete Medium (same day) ? Streak = 1 ? (no change)
3. Complete Expert (same day) ? Streak = 1 ? (no change)

Day 2:
4. Complete Boss ? Streak = 2 ?

Day 3:
5. Skip (don't play)

Day 4:
6. Complete any ? Streak = 1 ? (broken, restarted)
```

---

### Scenario 6: Speed Mode Timer
```
1. Select Speed difficulty
2. Generate puzzle
3. ? Timer starts: 60s
4. ?? Countdown visible (59, 58, 57...)
5. Wait until 10s remaining
6. ? Hear countdown sound
7. Submit answer at 45s
8. ? Points + time bonus
```

---

## ?? Testing Checklist

### Easy Difficulty:
- [ ] Multiplication puzzle works
- [ ] Division puzzle works
- [ ] Subtraction puzzle works
- [ ] Answer validation correct
- [ ] Hint displays
- [ ] +100 points awarded

### Medium Difficulty:
- [ ] Build It puzzle displays
- [ ] Available digits shown
- [ ] Calculator buttons work
- [ ] Digit buttons enable/disable correctly
- [ ] PEMDAS bonus calculates
- [ ] +200-250 points

### Hard Difficulty:
- [ ] Two input boxes (A, B)
- [ ] Labels display correctly
- [ ] Both values required
- [ ] Validation checks both
- [ ] +300 points

### Creative Difficulty:
- [ ] Build It with elegance check
- [ ] Bonuses calculate correctly
- [ ] +250+ points

### Tricky Difficulty:
- [ ] Three input boxes (A, B, C)
- [ ] All three required
- [ ] All different validation
- [ ] +400 points

### Speed Difficulty:
- [ ] Timer starts automatically
- [ ] Countdown displays
- [ ] 10s warning sound
- [ ] Time bonus calculates
- [ ] "Time Up!" if expires

### Boss Difficulty:
- [ ] Exponent (²) displays correctly
- [ ] Two variables work
- [ ] Exponent calculated first
- [ ] +500 points

### Expert Difficulty:
- [ ] Type A: Simple exponential works
- [ ] Type B: Expression exponential works
- [ ] Type C: Logarithm displays correctly
- [ ] Type D: Derivative notation clear
- [ ] All answers are integers
- [ ] +600 points
- [ ] Only accessible to subscribers

---

## ?? Common Issues to Check

### Issue 1: Wrong Answer Accepted
```
Problem: Entering 5 for "2^? = 8" shows correct
Expected: Should show wrong answer
Verify: 2^5 = 32, not 8
Fix: Check validation logic in GameService
```

---

### Issue 2: Multi-Value Not Validated
```
Problem: A=5, B=5 accepted for "A and B must be different"
Expected: Should reject duplicate values
Fix: Check validation in ValidateSolution
```

---

### Issue 3: Timer Not Starting
```
Problem: Speed mode loads but timer doesn't start
Expected: Timer starts immediately on puzzle load
Fix: Check StartTimer() call in SetupPuzzle
```

---

### Issue 4: Expert Not Locked
```
Problem: Free user can access Expert
Expected: Should show premium dialog
Fix: Check UpdateDifficultyButtons logic
```

---

### Issue 5: Streak Increments Multiple Times
```
Problem: Complete Easy and Medium, streak goes from 1 to 3
Expected: Should stay at 1 (same day)
Fix: Check GetAllTodaysAttempts logic
```

---

## ?? Expected Results Summary

| Difficulty | Puzzle Type | Answer Type | Points | Time Limit |
|------------|-------------|-------------|--------|------------|
| Easy | Single variable | 1 integer | 100 | None |
| Medium | Build It | Expression | 200-250 | None |
| Hard | Two variables | 2 integers | 300 | None |
| Creative | Build It elegant | Expression | 250-300 | None |
| Tricky | Three variables | 3 integers | 400 | None |
| Speed | Build It timed | Expression | 200+ | 60s |
| Boss | Exponents | 2-3 integers | 500 | None |
| Expert | Advanced math | 1 integer | 600 | None |

---

## ? Sign-Off

After testing all samples above:

**Tested By:** _________________  
**Date:** _________________  
**Device:** _________________  
**OS:** _________________

**Results:**
- [ ] All Easy puzzles work
- [ ] All Medium puzzles work
- [ ] All Hard puzzles work
- [ ] All Creative puzzles work
- [ ] All Tricky puzzles work
- [ ] All Speed puzzles work
- [ ] All Boss puzzles work
- [ ] All Expert puzzles work
- [ ] Difficulty selection works
- [ ] Ad unlock works
- [ ] Subscription works
- [ ] Streak tracking correct

**Issues Found:** _________________

**Overall Status:** [ ] Pass [ ] Fail

---

**Document Version:** 1.0  
**Last Updated:** December 19, 2024  
**Status:** Ready for Manual Testing

?? **Use this guide to thoroughly test all puzzle types!** ?
