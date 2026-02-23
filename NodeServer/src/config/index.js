const mysql = require('mysql2/promise');

const pool = mysql.createPool({
    host: process.env.DB_HOST || 'localhost',
    user: process.env.DB_USER || '',
    password: process.env.DB_PASSWORD || '',
    database: process.env.DB_NAME || '',
    waitForConnections: true,
    connectionLimit: 10,
});

const JWT_SECRET = process.env.JWT_SECRET || 'dj248120fhasv94h1oa7d73489vay760ddjfvhakjhg3498fhsuij3120v98wsa8';
const JWT_EXPIRES_IN = '30s';

module.exports = { pool, JWT_SECRET, JWT_EXPIRES_IN };
