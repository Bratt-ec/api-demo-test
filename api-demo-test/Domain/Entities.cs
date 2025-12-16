using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_demo_test.Domain;

[Table("respondents")]
public class Respondent
{
    [Key, Column("respondent_id")] public int Id { get; set; }
    [Column("email")] public required string Email { get; set; } 
    [Column("age_range")] public string? AgeRange { get; set; }
    [Column("full_name")] public string? FullName { get; set; }
    [Column("gender_identity")] public string? GenderIdentity { get; set; }
    [Column("team_role")] public string? TeamRole { get; set; }
}

[Table("questions")]
public class Question
{
    [Key, Column("question_id")] public int Id { get; set; }
    [Column("section_name")] public string? SectionName { get; set; }
    [Column("category")] public string? Category { get; set; }
    [Column("question_text")] public required string QuestionText { get; set; }
    [Column("scale_min")] public int ScaleMin { get; set; }
    [Column("scale_max")] public int ScaleMax { get; set; }
}


[Table("answers")]
public class Answer
{
    [Key, Column("answer_id")] public int Id { get; set; }
    [Column("respondent_id")] public int RespondentId { get; set; }
    [Column("question_id")] public int QuestionId { get; set; }
    [Column("score_value")] public int? ScoreValue { get; set; }
    [Column("text_value")] public string? TextValue { get; set; }
}