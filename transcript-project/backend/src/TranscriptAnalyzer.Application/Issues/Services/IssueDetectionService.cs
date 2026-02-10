using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Issues.Services;

public static class IssueDetectionService
{
    // IRS Transaction Codes for balance due assessments
    private static readonly HashSet<string> BalanceDueCodes = ["150", "290", "300", "298"];

    // IRS Transaction Codes for penalties
    private static readonly HashSet<string> PenaltyCodes = ["160", "170", "270", "280", "166", "276"];

    // IRS Transaction Codes for payment plans / installment agreements
    private static readonly HashSet<string> PaymentPlanCodes = ["971", "672"];

    // IRS Transaction Codes for liens
    private static readonly HashSet<string> LienCodes = ["582", "583"];

    // IRS Transaction Codes for levies
    private static readonly HashSet<string> LevyCodes = ["668", "671"];

    // Collection Statute Expiration Date is 10 years from assessment
    private static readonly TimeSpan CsedPeriod = TimeSpan.FromDays(365.25 * 10);

    public static IReadOnlyList<Issue> DetectIssues(
        Guid organizationId,
        Guid clientId,
        IReadOnlyList<TranscriptEntry> entries,
        Guid? caseWorkflowId = null)
    {
        var issues = new List<Issue>();

        issues.AddRange(DetectBalanceDueIssues(organizationId, clientId, entries, caseWorkflowId));
        issues.AddRange(DetectPenaltyIssues(organizationId, clientId, entries, caseWorkflowId));
        issues.AddRange(DetectUnfiledReturnIssues(organizationId, clientId, entries, caseWorkflowId));
        issues.AddRange(DetectStatuteExpirationIssues(organizationId, clientId, entries, caseWorkflowId));
        issues.AddRange(DetectPaymentPlanIssues(organizationId, clientId, entries, caseWorkflowId));
        issues.AddRange(DetectLienIssues(organizationId, clientId, entries, caseWorkflowId));
        issues.AddRange(DetectLevyIssues(organizationId, clientId, entries, caseWorkflowId));

        return issues;
    }

    private static List<Issue> DetectBalanceDueIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        foreach (var entry in entries.Where(e => BalanceDueCodes.Contains(e.TransactionCode)))
        {
            var severity = entry.Amount switch
            {
                >= 50000 => IssueSeverity.Critical,
                >= 10000 => IssueSeverity.High,
                >= 1000 => IssueSeverity.Medium,
                _ => IssueSeverity.Low
            };

            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.BalanceDue,
                severity,
                entry.Date.Year,
                $"Balance due assessment (TC {entry.TransactionCode}): {entry.Description}",
                entry.Amount,
                entry.TransactionCode,
                caseWorkflowId));
        }

        return issues;
    }

    private static List<Issue> DetectPenaltyIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        foreach (var entry in entries.Where(e => PenaltyCodes.Contains(e.TransactionCode)))
        {
            var penaltyType = entry.TransactionCode switch
            {
                "160" or "166" => "Failure to file penalty",
                "170" => "Estimated tax penalty",
                "270" or "276" => "Failure to pay penalty",
                "280" => "Bad check penalty",
                _ => "Penalty"
            };

            var severity = entry.Amount switch
            {
                >= 10000 => IssueSeverity.High,
                >= 1000 => IssueSeverity.Medium,
                _ => IssueSeverity.Low
            };

            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.Penalty,
                severity,
                entry.Date.Year,
                $"{penaltyType} (TC {entry.TransactionCode}): {entry.Description}",
                entry.Amount,
                entry.TransactionCode,
                caseWorkflowId));
        }

        return issues;
    }

    private static List<Issue> DetectUnfiledReturnIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        // TC 599 indicates "No return filed" for substitute for return (SFR)
        foreach (var entry in entries.Where(e => e.TransactionCode == "599"))
        {
            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.UnfiledReturn,
                IssueSeverity.High,
                entry.Date.Year,
                $"Unfiled return indicator (TC {entry.TransactionCode}): {entry.Description}",
                transactionCode: entry.TransactionCode,
                caseWorkflowId: caseWorkflowId));
        }

        return issues;
    }

    private static List<Issue> DetectStatuteExpirationIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        // Look for assessment dates (TC 150 = return filed/assessed)
        // CSED is 10 years from assessment date
        foreach (var entry in entries.Where(e => e.TransactionCode == "150"))
        {
            var csedDate = entry.Date.Add(CsedPeriod);
            var daysUntilExpiry = (csedDate - DateTime.UtcNow).TotalDays;

            var severity = daysUntilExpiry switch
            {
                <= 365 => IssueSeverity.Critical,
                <= 730 => IssueSeverity.High,
                <= 1825 => IssueSeverity.Medium,
                _ => IssueSeverity.Low
            };

            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.StatuteExpiration,
                severity,
                entry.Date.Year,
                $"CSED expires {csedDate:yyyy-MM-dd} ({daysUntilExpiry:F0} days remaining). Assessment date: {entry.Date:yyyy-MM-dd}",
                entry.Amount,
                entry.TransactionCode,
                caseWorkflowId));
        }

        return issues;
    }

    private static List<Issue> DetectPaymentPlanIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        // TC 971 can indicate various notices including installment agreement
        // TC 672 indicates installment agreement payment
        foreach (var entry in entries.Where(e => PaymentPlanCodes.Contains(e.TransactionCode)))
        {
            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.PaymentPlan,
                IssueSeverity.Medium,
                entry.Date.Year,
                $"Payment plan activity (TC {entry.TransactionCode}): {entry.Description}",
                entry.Amount,
                entry.TransactionCode,
                caseWorkflowId));
        }

        return issues;
    }

    private static List<Issue> DetectLienIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        foreach (var entry in entries.Where(e => LienCodes.Contains(e.TransactionCode)))
        {
            var description = entry.TransactionCode switch
            {
                "582" => "Notice of Federal Tax Lien filed",
                "583" => "Notice of Federal Tax Lien released",
                _ => "Lien activity"
            };

            var severity = entry.TransactionCode == "582"
                ? IssueSeverity.Critical
                : IssueSeverity.Medium;

            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.Lien,
                severity,
                entry.Date.Year,
                $"{description} (TC {entry.TransactionCode}): {entry.Description}",
                entry.Amount,
                entry.TransactionCode,
                caseWorkflowId));
        }

        return issues;
    }

    private static List<Issue> DetectLevyIssues(
        Guid organizationId, Guid clientId,
        IReadOnlyList<TranscriptEntry> entries, Guid? caseWorkflowId)
    {
        var issues = new List<Issue>();

        foreach (var entry in entries.Where(e => LevyCodes.Contains(e.TransactionCode)))
        {
            var description = entry.TransactionCode switch
            {
                "668" => "Levy issued",
                "671" => "Levy released",
                _ => "Levy activity"
            };

            var severity = entry.TransactionCode == "668"
                ? IssueSeverity.Critical
                : IssueSeverity.Medium;

            issues.Add(new Issue(
                organizationId,
                clientId,
                IssueType.Levy,
                severity,
                entry.Date.Year,
                $"{description} (TC {entry.TransactionCode}): {entry.Description}",
                entry.Amount,
                entry.TransactionCode,
                caseWorkflowId));
        }

        return issues;
    }
}
