using api_demo_test.Data;
using api_demo_test.Domain;
using Microsoft.EntityFrameworkCore;

namespace api_demo_test.Application;

public interface ISurveyService
{
    Task<IEnumerable<SectionDto>> GetQuestionsGroupedAsync();
    Task<(bool Success, string? ErrorMessage, int? CreatedId)> SubmitSurveyAsync(SurveySubmissionDto submission);
    Task<IEnumerable<SurveyResponseDto>> GetAllSurveysAsync();
}


public class SurveyService : ISurveyService
{
    private readonly SurveyDbContext _db;

    public SurveyService(SurveyDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<SectionDto>> GetQuestionsGroupedAsync()
    {
        var questions = await _db.Questions.AsNoTracking().ToListAsync();

        return questions
            .GroupBy(q => q.SectionName)
            .Select(g => new SectionDto(
                Section: g.Key ?? "General",
                Questions: g.OrderBy(q => q.Id)
                            .Select(q => new QuestionDto(q.Id, q.QuestionText, q.ScaleMin, q.ScaleMax))
                            .ToList()
            ));
    }


    public async Task<IEnumerable<SurveyResponseDto>> GetAllSurveysAsync()
    {
        // 1. Obtener todos los encuestados
        var respondents = await _db.Respondents.AsNoTracking().ToListAsync();

        // 2. Obtener todas las respuestas
        // Nota: En un sistema real con millones de registros, esto debería paginarse.
        var answers = await _db.Answers.AsNoTracking().ToListAsync();

        // 3. Agrupar respuestas por RespondentId en memoria para acceso rápido
        var answersLookup = answers.ToLookup(a => a.RespondentId);

        // 4. Mapear a DTOs
        var result = respondents.Select(r => new SurveyResponseDto(
            Id: r.Id,
            Respondent: new RespondentDto(
                r.Email, r.AgeRange, r.GenderIdentity, r.FullName,
                r.TeamRole
            ),
            Answers: answersLookup[r.Id] // Obtenemos las respuestas de este usuario del lookup
                .Select(a => new AnswerDto(a.QuestionId, a.ScoreValue, a.TextValue))
                .ToList()
        ));

        return result;
    }


    public async Task<(bool Success, string? ErrorMessage, int? CreatedId)> SubmitSurveyAsync(SurveySubmissionDto input)
    {
        // 1. Validar unicidad del email
        bool emailExists = await _db.Respondents.AnyAsync(r => r.Email == input.Respondent.Email);
        if (emailExists)
        {
            return (false, "El correo electrónico ya ha registrado una respuesta.", null);
        }

        // 2. Transacción atómica
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var respondent = new Respondent
            {
                Email = input.Respondent.Email,
                AgeRange = input.Respondent.AgeRange,
                GenderIdentity = input.Respondent.GenderIdentity,
                FullName = input.Respondent.FullName,
                TeamRole = input.Respondent.TeamRole
            };

            _db.Respondents.Add(respondent);
            await _db.SaveChangesAsync();

            if (input.Answers.Any())
            {
                var answersEntities = input.Answers.Select(a => new Answer
                {
                    RespondentId = respondent.Id,
                    QuestionId = a.QuestionId,
                    ScoreValue = a.ScoreValue,
                    TextValue = a.TextValue
                });

                _db.Answers.AddRange(answersEntities);
                await _db.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return (true, null, respondent.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            // Aquí podrías agregar un ILogger para registrar el error real
            return (false, "Error interno al procesar la encuesta.", null);
        }
    }
}