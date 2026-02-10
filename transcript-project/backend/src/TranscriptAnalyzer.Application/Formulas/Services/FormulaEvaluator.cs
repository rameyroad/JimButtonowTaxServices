using System.Globalization;
using System.Text.RegularExpressions;

namespace TranscriptAnalyzer.Application.Formulas.Services;

/// <summary>
/// Safe expression evaluator supporting basic math, functions, and conditionals.
/// Supports: +, -, *, /, parentheses, min, max, round, abs, if/then/else
/// Variables are resolved from the inputs dictionary.
/// </summary>
public static partial class FormulaEvaluator
{
    public static decimal Evaluate(string expression, IReadOnlyDictionary<string, decimal> inputs)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);

        // Substitute variable names with values
        var resolved = VariablePattern().Replace(expression, match =>
        {
            var varName = match.Value;
            if (inputs.TryGetValue(varName, out var value))
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            throw new InvalidOperationException($"Unknown variable: {varName}");
        });

        var tokens = Tokenize(resolved);
        var position = 0;
        var result = ParseExpression(tokens, ref position);

        if (position < tokens.Count)
        {
            throw new InvalidOperationException($"Unexpected token: {tokens[position]}");
        }

        return result;
    }

    [GeneratedRegex(@"[a-zA-Z_][a-zA-Z0-9_]*", RegexOptions.Compiled)]
    private static partial Regex VariablePattern();

    private static List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var i = 0;
        while (i < expression.Length)
        {
            if (char.IsWhiteSpace(expression[i]))
            {
                i++;
                continue;
            }

            // Numbers (including decimals and negative numbers at start or after operator)
            if (char.IsDigit(expression[i]) || (expression[i] == '.' && i + 1 < expression.Length && char.IsDigit(expression[i + 1])))
            {
                var start = i;
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    i++;
                tokens.Add(expression[start..i]);
                continue;
            }

            // Function names and keywords (min, max, round, abs, if, then, else)
            if (char.IsLetter(expression[i]))
            {
                var start = i;
                while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                    i++;
                tokens.Add(expression[start..i]);
                continue;
            }

            // Operators and punctuation
            if ("+-*/(),".Contains(expression[i], StringComparison.Ordinal))
            {
                tokens.Add(expression[i].ToString());
                i++;
                continue;
            }

            // Comparison operators
            if (expression[i] == '<' || expression[i] == '>')
            {
                if (i + 1 < expression.Length && expression[i + 1] == '=')
                {
                    tokens.Add(expression.Substring(i, 2));
                    i += 2;
                }
                else
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                continue;
            }

            if (expression[i] == '=' && i + 1 < expression.Length && expression[i + 1] == '=')
            {
                tokens.Add("==");
                i += 2;
                continue;
            }

            throw new InvalidOperationException($"Unexpected character: {expression[i]}");
        }

        return tokens;
    }

    // Expression = Term (('+' | '-') Term)*
    private static decimal ParseExpression(List<string> tokens, ref int pos)
    {
        var left = ParseTerm(tokens, ref pos);

        while (pos < tokens.Count && (tokens[pos] == "+" || tokens[pos] == "-"))
        {
            var op = tokens[pos++];
            var right = ParseTerm(tokens, ref pos);
            left = op == "+" ? left + right : left - right;
        }

        return left;
    }

    // Term = Factor (('*' | '/') Factor)*
    private static decimal ParseTerm(List<string> tokens, ref int pos)
    {
        var left = ParseUnary(tokens, ref pos);

        while (pos < tokens.Count && (tokens[pos] == "*" || tokens[pos] == "/"))
        {
            var op = tokens[pos++];
            var right = ParseUnary(tokens, ref pos);
            if (op == "/")
            {
                if (right == 0)
                    throw new InvalidOperationException("Division by zero");
                left /= right;
            }
            else
            {
                left *= right;
            }
        }

        return left;
    }

    // Unary = ('-' | '+') Unary | Primary
    private static decimal ParseUnary(List<string> tokens, ref int pos)
    {
        if (pos < tokens.Count && tokens[pos] == "-")
        {
            pos++;
            return -ParseUnary(tokens, ref pos);
        }

        if (pos < tokens.Count && tokens[pos] == "+")
        {
            pos++;
            return ParseUnary(tokens, ref pos);
        }

        return ParsePrimary(tokens, ref pos);
    }

    // Primary = Number | '(' Expression ')' | Function '(' args ')' | if '(' cond ')' 'then' '(' expr ')' 'else' '(' expr ')'
    private static decimal ParsePrimary(List<string> tokens, ref int pos)
    {
        if (pos >= tokens.Count)
            throw new InvalidOperationException("Unexpected end of expression");

        var token = tokens[pos];

        // Number literal
        if (decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
        {
            pos++;
            return number;
        }

        // Parenthesized expression
        if (token == "(")
        {
            pos++;
            var result = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, ")");
            return result;
        }

        // If/then/else: if(cond > val) then(expr) else(expr)
        if (string.Equals(token, "if", StringComparison.OrdinalIgnoreCase))
        {
            pos++;
            Expect(tokens, ref pos, "(");
            var condLeft = ParseExpression(tokens, ref pos);
            var compOp = tokens[pos++]; // comparison operator
            var condRight = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, ")");

            var condResult = EvaluateComparison(condLeft, compOp, condRight);

            Expect(tokens, ref pos, "then");
            Expect(tokens, ref pos, "(");
            var thenExpr = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, ")");

            Expect(tokens, ref pos, "else");
            Expect(tokens, ref pos, "(");
            var elseExpr = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, ")");

            return condResult ? thenExpr : elseExpr;
        }

        // Functions: min, max, round, abs
        if (string.Equals(token, "min", StringComparison.OrdinalIgnoreCase))
        {
            pos++;
            var args = ParseArguments(tokens, ref pos);
            if (args.Count < 2)
                throw new InvalidOperationException("min() requires at least 2 arguments");
            return args.Min();
        }

        if (string.Equals(token, "max", StringComparison.OrdinalIgnoreCase))
        {
            pos++;
            var args = ParseArguments(tokens, ref pos);
            if (args.Count < 2)
                throw new InvalidOperationException("max() requires at least 2 arguments");
            return args.Max();
        }

        if (string.Equals(token, "round", StringComparison.OrdinalIgnoreCase))
        {
            pos++;
            var args = ParseArguments(tokens, ref pos);
            return args.Count switch
            {
                1 => Math.Round(args[0], MidpointRounding.AwayFromZero),
                2 => Math.Round(args[0], (int)args[1], MidpointRounding.AwayFromZero),
                _ => throw new InvalidOperationException("round() requires 1 or 2 arguments")
            };
        }

        if (string.Equals(token, "abs", StringComparison.OrdinalIgnoreCase))
        {
            pos++;
            var args = ParseArguments(tokens, ref pos);
            if (args.Count != 1)
                throw new InvalidOperationException("abs() requires exactly 1 argument");
            return Math.Abs(args[0]);
        }

        throw new InvalidOperationException($"Unexpected token: {token}");
    }

    private static List<decimal> ParseArguments(List<string> tokens, ref int pos)
    {
        Expect(tokens, ref pos, "(");
        var args = new List<decimal> { ParseExpression(tokens, ref pos) };

        while (pos < tokens.Count && tokens[pos] == ",")
        {
            pos++;
            args.Add(ParseExpression(tokens, ref pos));
        }

        Expect(tokens, ref pos, ")");
        return args;
    }

    private static bool EvaluateComparison(decimal left, string op, decimal right)
    {
        return op switch
        {
            ">" => left > right,
            "<" => left < right,
            ">=" => left >= right,
            "<=" => left <= right,
            "==" => left == right,
            _ => throw new InvalidOperationException($"Unknown comparison operator: {op}")
        };
    }

    private static void Expect(List<string> tokens, ref int pos, string expected)
    {
        if (pos >= tokens.Count || !string.Equals(tokens[pos], expected, StringComparison.OrdinalIgnoreCase))
        {
            var actual = pos < tokens.Count ? tokens[pos] : "end of expression";
            throw new InvalidOperationException($"Expected '{expected}', got '{actual}'");
        }
        pos++;
    }
}
