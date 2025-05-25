CREATE TABLE t_todos
(
    id             INT AUTO_INCREMENT PRIMARY KEY,
    title          VARCHAR(100)                             NOT NULL,
    is_completed   TINYINT  DEFAULT 0                    NULL,
    completed_date DATETIME                              NULL,
    created_date   DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL
);