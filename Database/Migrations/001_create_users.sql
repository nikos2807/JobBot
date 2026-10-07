CREATE TABLE users
(
    id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    telegram_id BIGINT NOT NULL,
    username    VARCHAR(255),
    first_name  VARCHAR(255),
    last_name   VARCHAR(255),
    status      INTEGER NOT NULL,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ,

    CONSTRAINT users_telegram_id_unique UNIQUE (telegram_id)
);

CREATE INDEX ix_users_status
    ON users (status);