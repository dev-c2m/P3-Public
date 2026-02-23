const { pool } = require('../config');

async function isLoginIdExists(loginId) {
    const [rows] = await pool.execute(
        'SELECT COUNT(*) AS cnt FROM account WHERE user_id = ?',
        [loginId]
    );
    return rows[0].cnt > 0;
}

async function isNicknameExists(nickname) {
    const [rows] = await pool.execute(
        'SELECT COUNT(*) AS cnt FROM account WHERE nickname = ?',
        [nickname]
    );
    return rows[0].cnt > 0;
}

async function createAccount(loginId, password, nickname) {
    const connection = await pool.getConnection();
    try {
        const [insertResult] = await connection.execute(
            'INSERT INTO account (user_id, user_pw, nickname) VALUES (?, ?, ?)',
            [loginId, password, nickname]
        );

        const accountId = insertResult.insertId;

        await connection.execute(
            'INSERT INTO user (account_id, level, exp, hp, mp, map) VALUES (?, 1, 0, ?, ?, 1)',
            [accountId]
        );

        return accountId;
    } finally {
        connection.release();
    }
}

async function validateLogin(loginId, password) {
    const [rows] = await pool.execute(
        'SELECT id, nickname FROM account WHERE user_id = ? AND user_pw = ?',
        [loginId, password]
    );

    if (rows.length === 0) {
        return null;
    }

    return { accountId: rows[0].id, nickname: rows[0].nickname };
}

module.exports = { isLoginIdExists, isNicknameExists, createAccount, validateLogin };
