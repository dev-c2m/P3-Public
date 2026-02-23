const express = require('express');
const dotenv = require('dotenv');
const authRouter = require('./routes/auth');

dotenv.config();

const app = express();
app.use(express.json());

app.use('/api/auth', authRouter);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`API Server started on port ${PORT}`);
});
