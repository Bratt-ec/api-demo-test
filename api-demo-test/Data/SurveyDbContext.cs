using api_demo_test.Domain;
using Microsoft.EntityFrameworkCore;

namespace api_demo_test.Data;

public class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }

    public DbSet<Respondent> Respondents => Set<Respondent>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
}