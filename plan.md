The backend classes to implement

- Budgeting App Database Schema

-- Categories table for organizing transactions
CREATE TABLE categories (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    color VARCHAR(7), -- Hex color code for UI
    icon VARCHAR(50), -- Icon identifier
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Category goals table for budget targets
CREATE TABLE category_goals (
    id INTEGER PRIMARY KEY AUTOINCREMENT,s
    category_id INTEGER NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    period VARCHAR(20) NOT NULL, -- 'weekly', 'monthly', 'yearly'
    start_date DATE NOT NULL,
    end_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
);

-- Main transactions table
CREATE TABLE transactions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    date DATE NOT NULL,
    description VARCHAR(255) NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    type VARCHAR(10) NOT NULL, -- 'income' or 'expense'
    is_split BOOLEAN DEFAULT FALSE, -- Whether this transaction has splits
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Sub-transactions for splitting a transaction into multiple categories
CREATE TABLE transaction_splits (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    transaction_id INTEGER NOT NULL,
    category_id INTEGER NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (transaction_id) REFERENCES transactions(id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE RESTRICT
);

-- For non-split transactions, direct category assignment
CREATE TABLE transaction_categories (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    transaction_id INTEGER NOT NULL,
    category_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (transaction_id) REFERENCES transactions(id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE RESTRICT,
    UNIQUE(transaction_id) -- One category per non-split transaction
);

-- account

-- Indexes for better query performance
CREATE INDEX idx_transactions_date ON transactions(date);
CREATE INDEX idx_transactions_type ON transactions(type);
CREATE INDEX idx_transaction_splits_transaction ON transaction_splits(transaction_id);
CREATE INDEX idx_transaction_splits_category ON transaction_splits(category_id);
CREATE INDEX idx_transaction_categories_transaction ON transaction_categories(transaction_id);
CREATE INDEX idx_transaction_categories_category ON transaction_categories(category_id);
CREATE INDEX idx_category_goals_category ON category_goals(category_id);

-- Example seed data
INSERT INTO categories (name, description, color) VALUES
('Groceries', 'Food and household items', '#4CAF50'),
('Transportation', 'Gas, public transit, car maintenance', '#2196F3'),
('Dining Out', 'Restaurants and takeout', '#FF9800'),
('Entertainment', 'Movies, games, hobbies', '#9C27B0'),
('Utilities', 'Electric, water, internet', '#F44336'),
('Income', 'Salary and other income', '#8BC34A');

