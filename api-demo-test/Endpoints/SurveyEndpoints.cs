using Microsoft.AspNetCore.Mvc;
using api_demo_test.Application;

namespace api_demo_test.Endpoints;

public static class SurveyEndpoints
{
    public static void MapSurveyRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        group.MapGet("/questions", async (ISurveyService service, ILogger<Program> logger) =>
        {
            try
            {
                var result = await service.GetQuestionsGroupedAsync();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obteniendo las preguntas de la encuesta.");
                return Results.Problem(
                    detail: "Ocurrió un error interno al cargar las preguntas.",
                    statusCode: 500
                );
            }
        })
        .WithName("GetQuestions");


        // NUEVO GET: Obtener todas las respuestas
        group.MapGet("/surveys", async (ISurveyService service, ILogger<Program> logger) =>
        {
            try
            {
                var result = await service.GetAllSurveysAsync();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obteniendo el listado de encuestas.");
                return Results.Problem(detail: "Ocurrió un error interno al obtener los datos.", statusCode: 500);
            }
        })
        .WithName("GetAllSurveys");

        group.MapPost("/surveys", async ([FromBody] SurveySubmissionDto dto, ISurveyService service, ILogger<Program> logger) =>
        {
            try
            {
                // Validación básica de entrada (Fail-fast)
                if (dto.Respondent == null || string.IsNullOrWhiteSpace(dto.Respondent.Email))
                {
                    return Results.BadRequest(new { message = "Los datos del encuestado son inválidos o el email falta." });
                }

                var result = await service.SubmitSurveyAsync(dto);

                if (!result.Success)
                {
                    // Retornamos 409 Conflict si es una regla de negocio (ej. email duplicado)
                    return Results.Conflict(new { message = result.ErrorMessage });
                }

                return Results.Created($"/api/surveys/{result.CreatedId}", new
                {
                    success = true,
                    message = "Encuesta guardada exitosamente",
                    id = result.CreatedId
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error guardando la encuesta para el email: {Email}", dto.Respondent?.Email);
                return Results.Problem(
                    detail: "Ocurrió un error interno al guardar la encuesta. Por favor intente más tarde.",
                    statusCode: 500
                );
            }
        })
        .WithName("SaveSurvey");
    }
}