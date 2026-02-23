const jwt = require('jsonwebtoken');
const { JWT_SECRET, JWT_EXPIRES_IN } = require('../config');
const accountRepository = require('../repositories/accountRepository');

async function register(loginId, password, nickname) {
    const idExists = await accountRepository.isLoginIdExists(loginId);
    if (idExists) {
        return { success: false, message: '이미 존재하는 아이디입니다.' };
    }

    const nickExists = await accountRepository.isNicknameExists(nickname);
    if (nickExists) {
        return { success: false, message: '이미 존재하는 닉네임입니다.' };
    }

    await accountRepository.createAccount(loginId, password, nickname);
    return { success: true, message: '회원가입 성공' };
}

async function login(loginId, password) {
    const account = await accountRepository.validateLogin(loginId, password);
    if (account === null) {
        return { success: false, message: '아이디 또는 비밀번호가 일치하지 않습니다.' };
    }

    const token = jwt.sign(
        { accountId: account.accountId, nickname: account.nickname },
        JWT_SECRET,
        { expiresIn: JWT_EXPIRES_IN }
    );

    return { success: true, token };
}

module.exports = { register, login };
