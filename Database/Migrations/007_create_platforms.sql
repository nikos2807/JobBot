CREATE TABLE platforms
(
    id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type       INTEGER NOT NULL,
    name       VARCHAR(255) NOT NULL,
    is_active  BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

CREATE UNIQUE INDEX ux_platforms_type
    ON platforms (type);

CREATE INDEX ix_platforms_is_active
    ON platforms (is_active);