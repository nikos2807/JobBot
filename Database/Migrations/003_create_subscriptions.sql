CREATE TABLE subscriptions
(
    id                   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id              BIGINT NOT NULL,
    subscription_plan_id BIGINT NOT NULL,
    status               INTEGER NOT NULL,
    started_at           TIMESTAMPTZ NOT NULL,
    expires_at           TIMESTAMPTZ NOT NULL,
    created_at           TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at           TIMESTAMPTZ,

    CONSTRAINT fk_subscriptions_user
        FOREIGN KEY (user_id)
            REFERENCES users (id),

    CONSTRAINT fk_subscriptions_plan
        FOREIGN KEY (subscription_plan_id)
            REFERENCES subscription_plans (id),

    CONSTRAINT subscriptions_dates_valid
        CHECK (expires_at > started_at)
);

CREATE INDEX ix_subscriptions_user_id
    ON subscriptions (user_id);

CREATE INDEX ix_subscriptions_status
    ON subscriptions (status);

CREATE INDEX ix_subscriptions_expires_at
    ON subscriptions (expires_at);