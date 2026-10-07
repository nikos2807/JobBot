CREATE TABLE search_filters
(
    id              BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id         BIGINT NOT NULL,
    keywords        TEXT,
    salary_from     NUMERIC(12, 2),
    salary_to       NUMERIC(12, 2),
    location        VARCHAR(255),
    remote_only     BOOLEAN NOT NULL DEFAULT FALSE,
    employment_type VARCHAR(100),
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ,

    CONSTRAINT fk_search_filters_user
        FOREIGN KEY (user_id)
            REFERENCES users (id),

    CONSTRAINT search_filters_salary_valid
        CHECK (
            salary_from IS NULL
                OR salary_to IS NULL
                OR salary_from <= salary_to
            )
);

CREATE INDEX ix_search_filters_user_id
    ON search_filters (user_id);

CREATE INDEX ix_search_filters_active
    ON search_filters (user_id, is_active);