-- 1. Tabla de Encuestados (Respondents)
-- Almacena la Sección 1 (Demografía) ya que son atributos 1:1 con la persona.
-- Esto facilita mucho los queries analíticos posteriores (ej: agrupar por rol).

CREATE TABLE respondents (
    respondent_id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Sección 1: Información Demográfica
    age_range VARCHAR(20), -- Ej: '18-25', '26-35'
    gender_identity VARCHAR(50), -- Se guarda el texto final (Si es 'Otro', se guarda lo que escribió)
	full_name VARCHAR(50), -- Nombre del entrevistado
    email VARCHAR(100)
    team_role VARCHAR(50), -- 'Analista', 'Developer', etc.
    country VARCHAR(100)
);

-- 2. Tabla de Catálogo de Preguntas (Questions)
-- Define la estructura de las secciones 2, 3 y 4.
-- Esto normaliza el texto de las preguntas y evita repetición.

CREATE TABLE questions (
    question_id SERIAL PRIMARY KEY,
    section_name VARCHAR(100), -- Ej: 'Dinámicas del Equipo', 'Comunicación'
    category VARCHAR(100), -- Ej: 'Seguridad Psicológica', 'Voz', 'Silencio'
    question_text TEXT NOT NULL,
    scale_min INT DEFAULT 1,
    scale_max INT DEFAULT 5 -- Puede ser 5 o 7 según la sección
);

-- 3. Tabla de Respuestas (Answers)
-- Tabla transaccional que vincula al usuario con la pregunta y su respuesta.

CREATE TABLE answers (
    answer_id SERIAL PRIMARY KEY,
    respondent_id INT REFERENCES respondents(respondent_id) ON DELETE CASCADE,
    question_id INT REFERENCES questions(question_id),
    
    score_value INT, -- Para las escalas Likert (1-5 o 1-7)
    text_value TEXT, -- Para la pregunta final abierta (33) o notas adicionales
    
    -- Constraint: Un usuario solo responde una vez a cada pregunta
    UNIQUE (respondent_id, question_id) 
);

-- INDICES para mejorar performance en reportes
CREATE INDEX idx_answers_respondent ON answers(respondent_id);
CREATE INDEX idx_answers_question ON answers(question_id);
CREATE UNIQUE INDEX idx_respondents_email ON respondents(email);

-- =============================================
-- SEED DATA (Datos Semilla)
-- Poblamos las preguntas basándonos en tu encuesta
-- =============================================

-- Sección 2: Dinámicas del Equipo (Escala 1-5 y 1-7)
INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Dinámicas del Equipo', 'Impacto Percibido', 5, 'Mi impacto en lo que sucede en mi equipo es grande.'),
('Dinámicas del Equipo', 'Impacto Percibido', 5, 'Tengo un gran control sobre lo que sucede en mi equipo.'),
('Dinámicas del Equipo', 'Impacto Percibido', 5, 'Tengo una influencia significativa sobre lo que sucede en mi equipo.');

INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Si cometo un error en este equipo, a menudo se usa en mi contra.'),
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Los miembros de mi equipo son capaces de plantear problemas y asuntos difíciles.'),
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Las personas en mi equipo a veces rechazan a otros por ser diferentes.'),
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Es seguro tomar riesgos aquí.'),
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Es difícil pedir ayuda a otros miembros de mi equipo.'),
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Nadie en mi equipo actuaría deliberadamente de una manera que socave mis esfuerzos.'),
('Dinámicas del Equipo', 'Seguridad Psicológica', 7, 'Al trabajar con los miembros de mi equipo, mis habilidades y talentos únicos son valorados y utilizados.');

-- Sección 3: Comunicación (Escala 1-7)
INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Comunicación', 'Voz', 7, 'Di sugerencias proactivamente para problemas que pueden influir en mi equipo.'),
('Comunicación', 'Voz', 7, 'Expresé proactivamente sugerencias constructivas beneficiosas para mi equipo.'),
('Comunicación', 'Voz', 7, 'Hice sugerencias sobre cómo mejorar los procedimientos de trabajo de mi equipo.'),
('Comunicación', 'Voz', 7, 'Aconsejé contra comportamientos indeseables que obstaculizarían el desempeño de mi equipo.'),
('Comunicación', 'Voz', 7, 'Hablé honestamente sobre problemas que podrían causar pérdidas graves al equipo, incluso si existían opiniones disidentes.'),
('Comunicación', 'Voz', 7, 'Señalé problemas cuando aparecieron en mi equipo, incluso si eso obstaculizara las relaciones con otros colegas.');

INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Comunicación', 'Silencio', 7, 'Me quedé callado y no hice recomendaciones sobre cómo solucionar problemas relacionados con el trabajo.'),
('Comunicación', 'Silencio', 7, 'Me guardé para mí ideas sobre cómo mejorar las prácticas de trabajo.'),
('Comunicación', 'Silencio', 7, 'Elegí no hablar sobre ideas para prácticas de trabajo nuevas o más efectivas.');

-- Sección 4: Bienestar y Desempeño
INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Pienso en ausentarme.'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Charlo con compañeros de trabajo sobre temas no laborales (en exceso).'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Dejo mi puesto/área de trabajo por razones innecesarias.'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Sueño despierto (daydreaming).'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Paso tiempo de trabajo en asuntos personales.'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Pongo menos esfuerzo en mi trabajo del que debería.'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Pienso en dejar mi trabajo actual.'),
('Bienestar y Desempeño', 'Retraimiento Psicológico', 7, 'Dejo que otros hagan mi trabajo.');

INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Bienestar y Desempeño', 'Desempeño Laboral', 5, 'Logré planificar mi trabajo para terminarlo a tiempo.'),
('Bienestar y Desempeño', 'Desempeño Laboral', 5, 'Mantuve en mente el resultado del trabajo que necesitaba lograr.'),
('Bienestar y Desempeño', 'Desempeño Laboral', 5, 'Fui capaz de establecer prioridades.'),
('Bienestar y Desempeño', 'Desempeño Laboral', 5, 'Fui capaz de realizar mi trabajo eficientemente.'),
('Bienestar y Desempeño', 'Desempeño Laboral', 5, 'Gestioné bien mi tiempo.');

-- Pregunta Final (Abierta)
INSERT INTO questions (section_name, category, scale_max, question_text) VALUES
('Final', 'Comentarios', 0, '¿Hay algo más que quiera decirnos sobre cómo los comportamientos de los miembros del equipo afectan a su equipo?');