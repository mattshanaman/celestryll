using NCalc;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Pemdas.Services;

public class ExpressionEvaluator
{
    // Performance: Cache compiled expressions for faster repeat evaluations
    private readonly ConcurrentDictionary<string, Expression> _expressionCache = new();
    private const int MaxCacheSize = 100;

    // Performance: Pre-compiled regex for whitespace normalization
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public (bool isValid, double result) Evaluate(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            System.Diagnostics.Debug.WriteLine("Cannot evaluate null or empty expression");
            return (false, 0);
        }

        try
        {
            // Performance: Normalize expression once
            expression = NormalizeExpression(expression);

            // Performance: Try to get from cache first
            if (!_expressionCache.TryGetValue(expression, out var eval))
            {
                eval = new Expression(expression);
                
                // Performance: Cache the expression if cache isn't too large
                if (_expressionCache.Count < MaxCacheSize)
                {
                    _expressionCache.TryAdd(expression, eval);
                }
            }
            
            if (eval.HasErrors())
            {
                System.Diagnostics.Debug.WriteLine($"Expression has errors: {eval.Error}");
                return (false, 0);
            }

            var result = eval.Evaluate();
            
            if (result == null)
            {
                System.Diagnostics.Debug.WriteLine("Expression evaluation returned null");
                return (false, 0);
            }

            var doubleResult = Convert.ToDouble(result);
            
            if (double.IsNaN(doubleResult) || double.IsInfinity(doubleResult))
            {
                System.Diagnostics.Debug.WriteLine($"Expression evaluation resulted in invalid number: {doubleResult}");
                return (false, 0);
            }

            return (true, doubleResult);
        }
        catch (FormatException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Format error evaluating expression: {ex.Message}");
            return (false, 0);
        }
        catch (DivideByZeroException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Division by zero in expression: {ex.Message}");
            return (false, 0);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error evaluating expression: {ex.Message}");
            return (false, 0);
        }
    }

    private static string NormalizeExpression(string expression)
    {
        // Replace mathematical symbols with operators NCalc understands
        expression = expression.Replace("×", "*", StringComparison.Ordinal)
                              .Replace("÷", "/", StringComparison.Ordinal)
                              .Replace("²", "^2", StringComparison.Ordinal)
                              .Replace("√", "Sqrt", StringComparison.Ordinal);

        // Remove extra whitespace using pre-compiled regex
        return WhitespaceRegex.Replace(expression, " ").Trim();
    }

    public bool ValidateDigitsUsed(string? expression, List<int>? allowedDigits)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            System.Diagnostics.Debug.WriteLine("Cannot validate null or empty expression");
            return false;
        }

        if (allowedDigits == null || allowedDigits.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine("No allowed digits provided");
            return false;
        }

        try
        {
            // Performance: Use Span<char> for better performance
            var span = expression.AsSpan();
            var digitsInExpression = new List<int>(allowedDigits.Count);

            foreach (var c in span)
            {
                if (char.IsDigit(c))
                {
                    digitsInExpression.Add(c - '0');
                }
            }

            if (digitsInExpression.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No digits found in expression");
                return false;
            }

            // Performance: Use HashSet for O(1) lookups
            var allowedSet = new HashSet<int>(allowedDigits);
            var usedSet = new HashSet<int>();

            foreach (var digit in digitsInExpression)
            {
                if (!allowedSet.Contains(digit) || usedSet.Contains(digit))
                {
                    System.Diagnostics.Debug.WriteLine($"Digit {digit} not in allowed list or used multiple times");
                    return false;
                }
                usedSet.Add(digit);
                allowedSet.Remove(digit);
            }

            if (allowedSet.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Not all digits used. Remaining: {string.Join(", ", allowedSet)}");
            }

            return allowedSet.Count == 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error validating digits: {ex.Message}");
            return false;
        }
    }

    public int CountParentheses(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return 0;

        try
        {
            // Performance: Use Span for better performance
            var span = expression.AsSpan();
            int openCount = 0;
            int closeCount = 0;

            foreach (var c in span)
            {
                if (c == '(') openCount++;
                else if (c == ')') closeCount++;
            }

            if (openCount != closeCount)
            {
                System.Diagnostics.Debug.WriteLine($"Mismatched parentheses: {openCount} open, {closeCount} close");
            }

            return openCount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error counting parentheses: {ex.Message}");
            return 0;
        }
    }

    // Performance: Clear cache periodically
    public void ClearCache()
    {
        _expressionCache.Clear();
    }
}