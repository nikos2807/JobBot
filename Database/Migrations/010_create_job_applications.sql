CREATE TABLE job_applications
(
    id                      BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id                 BIGINT NOT NULL,
    vacancy_id              BIGINT NOT NULL,
    status                  INTEGER NOT NULL,
    external_application_id VARCHAR(255),
    error_message           TEXT,
    sent_at                 TIMESTAMPTZ,
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMPTZ,

    CONSTRAINT fk_job_applications_user
        FOREIGN KEY (user_id)
            REFERENCES users (id),

    CONSTRAINT fk_job_applications_vacancy
        FOREIGN KEY (vacancy_id)
            REFERENCES vacancies (id)
);

CREATE UNIQUE INDEX ux_job_applications_user_vacancy
    ON job_applications (user_id, vacancy_id);

CREATE INDEX ix_job_applications_user_id
    ON job_applications (user_id);

CREATE INDEX ix_job_applications_vacancy_id
    ON job_applications (vacancy_id);

CREATE INDEX ix_job_applications_status
    ON job_applications (status);

CREATE INDEX ix_job_applications_created_at
    ON job_applications (created_at);