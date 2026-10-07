CREATE TABLE payments
(
    id                   BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id              BIGINT NOT NULL,
    subscription_plan_id BIGINT NOT NULL,
    amount               NUMERIC(12, 2) NOT NULL,
    currency             VARCHAR(10) NOT NULL DEFAULT 'RUB',
    status               INTEGER NOT NULL,
    external_payment_id  VARCHAR(255),
    created_at            TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    paid_at              TIMESTAMPTZ,

    CONSTRAINT fk_payments_user
        FOREIGN KEY (user_id)
            REFERENCES users (id),

    CONSTRAINT fk_payments_plan
        FOREIGN KEY (subscription_plan_id)
            REFERENCES subscription_plans (id),

    CONSTRAINT payments_amount_positive
        CHECK (amount >= 0)
);

CREATE UNIQUE INDEX ux_payments_external_payment_id
    ON payments (external_payment_id)
    WHERE external_payment_id IS NOT NULL;

CREATE INDEX ix_payments_user_id
    ON payments (user_id);

CREATE INDEX ix_payments_status
    ON payments (status);

CREATE INDEX ix_payments_created_at
    ON payments (created_at);