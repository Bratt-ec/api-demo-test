using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api_demo_test.Migrations
{
    public partial class InitialSurveySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Respondents Table
            migrationBuilder.CreateTable(
                name: "Respondents",
                columns: table => new
                {
                    respondent_id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), // Or .Annotation("Npgsql:ValueGenerationStrategy", ...) for Postgres
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),

                    // New requested field
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),

                    // Preserved demographic fields
                    age_range = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    gender_identity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    team_role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respondents", x => x.respondent_id);
                });

            // 2. Create Questions Table
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    section_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    question_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    scale_min = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    scale_max = table.Column<int>(type: "int", nullable: true, defaultValue: 5)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.question_id);
                });

            // 3. Create Answers Table
            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    answer_id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    respondent_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    score_value = table.Column<int>(type: "int", nullable: true),
                    text_value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.answer_id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "question_id",
                        onDelete: ReferentialAction.Cascade); // Cascade delete as per SQL
                    table.ForeignKey(
                        name: "FK_Answers_Respondents_respondent_id",
                        column: x => x.respondent_id,
                        principalTable: "Respondents",
                        principalColumn: "respondent_id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Indexes
            migrationBuilder.CreateIndex(
                name: "IX_Answers_question_id",
                table: "Answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_respondent_id",
                table: "Answers",
                column: "respondent_id");

            // Unique constraint for (respondent_id, question_id)
            migrationBuilder.CreateIndex(
                name: "IX_Answers_respondent_id_question_id",
                table: "Answers",
                columns: new[] { "respondent_id", "question_id" },
                unique: true);

            // =============================================
            // SEED DATA
            // =============================================

            // Section 2: Team Dynamics
            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "section_name", "category", "scale_max", "question_text" },
                values: new object[,]
                {
                    { "Dinámicas del Equipo", "Impacto Percibido", 5, "Mi impacto en lo que sucede en mi equipo es grande." },
                    { "Dinámicas del Equipo", "Impacto Percibido", 5, "Tengo un gran control sobre lo que sucede en mi equipo." },
                    { "Dinámicas del Equipo", "Impacto Percibido", 5, "Tengo una influencia significativa sobre lo que sucede en mi equipo." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Si cometo un error en este equipo, a menudo se usa en mi contra." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Los miembros de mi equipo son capaces de plantear problemas y asuntos difíciles." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Las personas en mi equipo a veces rechazan a otros por ser diferentes." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Es seguro tomar riesgos aquí." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Es difícil pedir ayuda a otros miembros de mi equipo." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Nadie en mi equipo actuaría deliberadamente de una manera que socave mis esfuerzos." },
                    { "Dinámicas del Equipo", "Seguridad Psicológica", 7, "Al trabajar con los miembros de mi equipo, mis habilidades y talentos únicos son valorados y utilizados." }
                });

            // Section 3: Communication
            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "section_name", "category", "scale_max", "question_text" },
                values: new object[,]
                {
                    { "Comunicación", "Voz", 7, "Di sugerencias proactivamente para problemas que pueden influir en mi equipo." },
                    { "Comunicación", "Voz", 7, "Expresé proactivamente sugerencias constructivas beneficiosas para mi equipo." },
                    { "Comunicación", "Voz", 7, "Hice sugerencias sobre cómo mejorar los procedimientos de trabajo de mi equipo." },
                    { "Comunicación", "Voz", 7, "Aconsejé contra comportamientos indeseables que obstaculizarían el desempeño de mi equipo." },
                    { "Comunicación", "Voz", 7, "Hablé honestamente sobre problemas que podrían causar pérdidas graves al equipo, incluso si existían opiniones disidentes." },
                    { "Comunicación", "Voz", 7, "Señalé problemas cuando aparecieron en mi equipo, incluso si eso obstaculizara las relaciones con otros colegas." },
                    { "Comunicación", "Silencio", 7, "Me quedé callado y no hice recomendaciones sobre cómo solucionar problemas relacionados con el trabajo." },
                    { "Comunicación", "Silencio", 7, "Me guardé para mí ideas sobre cómo mejorar las prácticas de trabajo." },
                    { "Comunicación", "Silencio", 7, "Elegí no hablar sobre ideas para prácticas de trabajo nuevas o más efectivas." }
                });

            // Section 4: Well-being and Performance
            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "section_name", "category", "scale_max", "question_text" },
                values: new object[,]
                {
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Pienso en ausentarme." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Charlo con compañeros de trabajo sobre temas no laborales (en exceso)." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Dejo mi puesto/área de trabajo por razones innecesarias." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Sueño despierto (daydreaming)." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Paso tiempo de trabajo en asuntos personales." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Pongo menos esfuerzo en mi trabajo del que debería." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Pienso en dejar mi trabajo actual." },
                    { "Bienestar y Desempeño", "Retraimiento Psicológico", 7, "Dejo que otros hagan mi trabajo." },
                    { "Bienestar y Desempeño", "Desempeño Laboral", 5, "Logré planificar mi trabajo para terminarlo a tiempo." },
                    { "Bienestar y Desempeño", "Desempeño Laboral", 5, "Mantuve en mente el resultado del trabajo que necesitaba lograr." },
                    { "Bienestar y Desempeño", "Desempeño Laboral", 5, "Fui capaz de establecer prioridades." },
                    { "Bienestar y Desempeño", "Desempeño Laboral", 5, "Fui capaz de realizar mi trabajo eficientemente." },
                    { "Bienestar y Desempeño", "Desempeño Laboral", 5, "Gestioné bien mi tiempo." }
                });

            // Final Question
            migrationBuilder.InsertData(
               table: "Questions",
               columns: new[] { "section_name", "category", "scale_max", "question_text" },
               values: new object[,]
               {
                    { "Final", "Comentarios", 0, "¿Hay algo más que quiera decirnos sobre cómo los comportamientos de los miembros del equipo afectan a su equipo?" }
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Answers");
            migrationBuilder.DropTable(name: "Questions");
            migrationBuilder.DropTable(name: "Respondents");
        }
    }
}
