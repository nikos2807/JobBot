CREATE TABLE vacancies
(
    id           BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    platform_id  BIGINT NOT NULL,
    external_id  VARCHAR(255) NOT NULL,
    title        VARCHAR(500) NOT NULL,
    company_name VARCHAR(500),
    description  TEXT,
    url          TEXT,
    salary_from  NUMERIC(12, 2),
    salary_to    NUMERIC(12, 2),
    currency     VARCHAR(10),
    location     VARCHAR(255),
    remote       BOOLEAN NOT NULL DEFAULT FALSE,
    published_at TIMESTAMPTZ NOT NULL,
    created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at   TIMESTAMPTZ,

    CONSTRAINT fk_vacancies_platform
        FOREIGN KEY (platform_id)
            REFERENCES platforms (id),

    CONSTRAINT vacancies_salary_valid
        CHECK (
            salary_from IS NULL
                OR salary_to IS NULL
                OR salary_from <= salary_to
            )
);

CREATE UNIQUE INDEX ux_vacancies_platform_external_id
    ON vacancies (platform_id, external_id);

CREATE INDEX ix_vacancies_platform_id
    ON vacancies (platform_id);

CREATE INDEX ix_vacancies_published_at
    ON vacancies (published_at);

CREATE INDEX ix_vacancies_location
    ON vacancies (location);