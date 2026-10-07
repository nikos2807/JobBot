CREATE TABLE platform_accounts
(
    id                BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id           BIGINT NOT NULL,
    platform_id       BIGINT NOT NULL,
    external_user_id  VARCHAR(255),
    access_token      TEXT,
    refresh_token     TEXT,
    token_expires_at  TIMESTAMPTZ,
    is_active         BOOLEAN NOT NULL DEFAULT TRUE,
    created_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at        TIMESTAMPTZ,

    CONSTRAINT fk_platform_accounts_user
        FOREIGN KEY (user_id)
            REFERENCES users (id),

    CONSTRAINT fk_platform_accounts_platform
        FOREIGN KEY (platform_id)
            REFERENCES platforms (id)
);

CREATE UNIQUE INDEX ux_platform_accounts_user_platform
    ON platform_accounts (user_id, platform_id);

CREATE INDEX ix_platform_accounts_platform_id
    ON platform_accounts (platform_id);