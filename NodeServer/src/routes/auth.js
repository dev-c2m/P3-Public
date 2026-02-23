const express = require('express');
const router = express.Router();
const authService = require('../services/authService');

router.post('/register', async (req, res) => {
    const { loginId, password, nickname } = req.body;

    if (!loginId || loginId.length < 4 || loginId.length > 20) {
        return res.json({ success: false, message: '아이디는 4~20자여야 합니다.' });
    }

    if (!password || password.length < 4 || password.length > 20) {
        return res.json({ success: false, message: '비밀번호는 4~20자여야 합니다.' });
    }

    if (!nickname || nickname.length < 2 || nickname.length > 12) {
        return res.json({ success: false, message: '닉네임은 2~12자여야 합니다.' });
    }

    try {
        const result = await authService.register(loginId, password, nickname);
        return res.json(result);
    } catch (err) {
        console.error('[Register Error]', err.message);
        return res.json({ success: false, message: '서버 오류가 발생했습니다.' });
    }
});

router.post('/login', async (req, res) => {
    const { loginId, password } = req.body;

    if (!loginId || !password) {
        return res.json({ success: false, message: '아이디와 비밀번호를 입력해주세요.' });
    }

    try {
        const result = await authService.login(loginId, password);
        return res.json(result);
    } catch (err) {
        console.error('[Login Error]', err.message);
        return res.json({ success: false, message: '서버 오류가 발생했습니다.' });
    }
});

module.exports = router;
