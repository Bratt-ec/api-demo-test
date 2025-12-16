using System.ComponentModel.DataAnnotations;

namespace api_demo_test.Application;


// --- DTOs de Entrada (Requests) ---
public record SurveySubmissionDto(RespondentDto Respondent, List<AnswerDto> Answers);

public record RespondentDto(
    [property: EmailAddress] string Email,
    string? AgeRange,
    string? FullName,
    string? GenderIdentity,
    string? TeamRole
);

public record AnswerDto(int QuestionId, int? ScoreValue, string? TextValue);

// --- DTOs de Salida (Responses) ---
public record SectionDto(string Section, List<QuestionDto> Questions);
public record QuestionDto(int Id, string Text, int Min, int Max);

public record SurveyResponseDto(int Id, RespondentDto Respondent, List<AnswerDto> Answers);
