USE issuing;

CREATE TABLE IF NOT EXISTS authorization_events(message_id CHAR(36) NOT NULL,
                                                event_type VARCHAR(20) NOT NULL,
                                                card_id VARCHAR(64) NOT NULL,
                                                amount DECIMAL(18,2) NOT NULL,
                                                currency VARCHAR(8) NOT NULL,
                                                authorization_code VARCHAR(32) NULL,
                                                reason_code VARCHAR(64) NULL,
                                                created_on DATETIME(6) NOT NULL,
                                                processed_on DATETIME(6) NOT NULL,
                                                PRIMARY KEY (message_id),
                                                INDEX idx_authorization_events_card_id (card_id),
                                                INDEX idx_authorization_events_created_on (created_on));