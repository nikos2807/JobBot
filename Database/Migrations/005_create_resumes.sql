CREATE TABLE resumes
(
    id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id     BIGINT NOT NULL,
    file_name   VARCHAR(500) NOT NULL,
    file_url    TEXT,
    text        TEXT,
    parsed_data JSONB,
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ,

    CONSTRAINT fk_resumes_user
        FOREIGN KEY (user_id)
            REFERENCES users (id)
);

CREATE INDEX ix_resumes_user_id
    ON resumes (user_id);

CREATE INDEX ix_resumes_active
    ON resumes (user_id, is_active);