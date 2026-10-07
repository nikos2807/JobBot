CREATE TABLE subscription_plans
(
    id                       BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                     VARCHAR(255) NOT NULL,
    price                    NUMERIC(12, 2) NOT NULL,
    duration_days            INTEGER NOT NULL,
    max_applications_per_day INTEGER NOT NULL,
    is_active                BOOLEAN NOT NULL DEFAULT TRUE,
    created_at               TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at               TIMESTAMPTZ,

    CONSTRAINT subscription_plans_price_positive
        CHECK (price >= 0),

    CONSTRAINT subscription_plans_duration_positive
        CHECK (duration_days > 0),

    CONSTRAINT subscription_plans_applications_positive
        CHECK (max_applications_per_day > 0)
);

CREATE INDEX ix_subscription_plans_is_active
    ON subscription_plans (is_active);